using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatFiles;
using SabreTools.DatItems;
using SabreTools.Logging;

namespace SabreTools.Filtering
{
    /// <summary>
    /// Represents the cleaning operations that need to be performed on a set of items, usually a DAT
    /// </summary>
    public class Cleaner
    {
        #region Exclusion Fields

        /// <summary>
        /// Dictionary of DatHeader fields to exclude from writing
        /// </summary>
        public List<DatHeaderField> ExcludeDatHeaderFields { get; set; } = new List<DatHeaderField>();

        /// <summary>
        /// Dictionary of DatItem fields to exclude from writing
        /// </summary>
        public List<DatItemField> ExcludeDatItemFields { get; set; } = new List<DatItemField>();

        /// <summary>
        /// Dictionary of Machine fields to exclude from writing
        /// </summary>
        public List<MachineField> ExcludeMachineFields { get; set; } = new List<MachineField>();

        #endregion

        #region Filter Fields

        /// <summary>
        /// Filter for DatHeader fields
        /// </summary>
        public DatHeaderFilter DatHeaderFilter { get; set; }

        /// <summary>
        /// Filter for DatItem fields
        /// </summary>
        public DatItemFilter DatItemFilter { get; set; }

        /// <summary>
        /// Filter for Machine fields
        /// </summary>
        public MachineFilter MachineFilter { get; set; }

        #endregion

        #region Flag Fields

        /// <summary>
        /// Clean all names to WoD standards
        /// </summary>
        public bool Clean { get; set; }

        /// <summary>
        /// Deduplicate items using the given method
        /// </summary>
        public DedupeType DedupeRoms { get; set; }

        /// <summary>
        /// Set Machine Description from Machine Name
        /// </summary>
        public bool DescriptionAsName { get; set; }

        /// <summary>
        /// Keep machines that don't contain any items
        /// </summary>
        public bool KeepEmptyGames { get; set; }

        /// <summary>
        /// Enable "One Rom, One Region (1G1R)" mode
        /// </summary>
        public bool OneGamePerRegion { get; set; }

        /// <summary>
        /// Ordered list of regions for "One Rom, One Region (1G1R)" mode
        /// </summary>
        public List<string> RegionList { get; set; }

        /// <summary>
        /// Ensure each rom is in their own game
        /// </summary>
        public bool OneRomPerGame { get; set; }

        /// <summary>
        /// Remove all unicode characters
        /// </summary>
        public bool RemoveUnicode { get; set; }

        /// <summary>
        /// Include root directory when determing trim sizes
        /// </summary>
        public string Root { get; set; }

        /// <summary>
        /// Remove scene dates from the beginning of machine names
        /// </summary>
        public bool SceneDateStrip { get; set; }

        /// <summary>
        /// Change all machine names to "!"
        /// </summary>
        public bool Single { get; set; }

        /// <summary>
        /// Trim total machine and item name to not exceed NTFS limits
        /// </summary>
        public bool Trim { get; set; }
    
        #endregion

        #region Logging

        /// <summary>
        /// Logging object
        /// </summary>
        private readonly Logger logger = new Logger();

        #endregion

        #region Population

        /// <summary>
        /// Populate the exclusion objects using a set of field names
        /// </summary>
        /// <param name="fields">List of field names</param>
        public void PopulateExclusionsFromList(List<string> fields)
        {
            // Instantiate the lists, if necessary
            ExcludeDatHeaderFields ??= new List<DatHeaderField>();
            ExcludeMachineFields ??= new List<MachineField>();
            ExcludeDatItemFields ??= new List<DatItemField>();

            // If the list is null or empty, just return
            if (fields == null || fields.Count == 0)
                return;

            foreach (string field in fields)
            {                
                // If we don't even have a possible field name
                if (field == null)
                    continue;

                // DatHeader fields
                DatHeaderField datHeaderField = field.AsDatHeaderField();
                if (datHeaderField != DatHeaderField.NULL)
                {
                    ExcludeDatHeaderFields.Add(datHeaderField);
                    continue;
                }

                // Machine fields
                MachineField machineField = field.AsMachineField();
                if (machineField != MachineField.NULL)
                {
                    ExcludeMachineFields.Add(machineField);
                    continue;
                }

                // DatItem fields
                DatItemField datItemField = field.AsDatItemField();
                if (datItemField != DatItemField.NULL)
                {
                    ExcludeDatItemFields.Add(datItemField);
                    continue;
                }

                // If we didn't match anything, log an error
                logger.Warning($"The value {field} did not match any known field names. Please check the wiki for more details on supported field names.");
            }
        }

        /// <summary>
        /// Populate the filters objects using a set of key:value filters
        /// </summary>
        /// <param name="filters">List of key:value where ~key/!key is negated</param>
        public void PopulateFiltersFromList(List<string> filters)
        {
            // Instantiate the filters, if necessary
            DatHeaderFilter ??= new DatHeaderFilter();
            MachineFilter ??= new MachineFilter();
            DatItemFilter ??= new DatItemFilter();

            // If the list is null or empty, just return
            if (filters == null || filters.Count == 0)
                return;

            foreach (string filterPair in filters)
            {
                (string field, string value, bool negate) = ProcessFilterPair(filterPair);
                
                // If we don't even have a possible filter pair
                if (field == null && value == null)
                    continue;

                // DatHeader fields
                DatHeaderField datHeaderField = field.AsDatHeaderField();
                if (datHeaderField != DatHeaderField.NULL)
                {
                    DatHeaderFilter.SetFilter(datHeaderField, value, negate);
                    continue;
                }

                // Machine fields
                MachineField machineField = field.AsMachineField();
                if (machineField != MachineField.NULL)
                {
                    MachineFilter.SetFilter(machineField, value, negate);
                    continue;
                }

                // DatItem fields
                DatItemField datItemField = field.AsDatItemField();
                if (datItemField != DatItemField.NULL)
                {
                    DatItemFilter.SetFilter(datItemField, value, negate);
                    continue;
                }

                // If we didn't match anything, log an error
                logger.Warning($"The value {field} did not match any known field names. Please check the wiki for more details on supported field names.");
            }
        }

        #endregion
    
        #region Cleaning

        /// <summary>
        /// Clean a DatItem according to the cleaner
        /// </summary>
        /// <param name="datItem">DatItem to clean</param>
        public void CleanDatItem(DatItem datItem)
        {
            // If we're stripping unicode characters, strip machine name and description
            if (RemoveUnicode)
            {
                datItem.Machine.Name = RemoveUnicodeCharacters(datItem.Machine.Name);
                datItem.Machine.Description = RemoveUnicodeCharacters(datItem.Machine.Description);
                datItem.SetName(RemoveUnicodeCharacters(datItem.GetName()));
            }

            // If we're in cleaning mode, sanitize machine name and description
            if (Clean)
            {
                datItem.Machine.Name = CleanGameName(datItem.Machine.Name);
                datItem.Machine.Description = CleanGameName(datItem.Machine.Description);
            }

            // If we are in single game mode, rename the machine
            if (Single)
                datItem.Machine.Name = "!";

            // If we are in NTFS trim mode, trim the item name
            if (Trim && datItem.GetName() != null)
            {
                // Windows max name length is 260
                int usableLength = 260 - datItem.Machine.Name.Length - (Root?.Length ?? 0);
                if (datItem.GetName().Length > usableLength)
                {
                    string ext = Path.GetExtension(datItem.GetName());
                    datItem.SetName(datItem.GetName().Substring(0, usableLength - ext.Length) + ext);
                }
            }
        }

        /// <summary>
        /// Clean a game (or rom) name to the WoD standard
        /// </summary>
        /// <param name="game">Name of the game to be cleaned</param>
        /// <returns>The cleaned name</returns>
        private string CleanGameName(string game)
        {
            if (game == null)
                return null;

            ///Run the name through the filters to make sure that it's correct
            game = NormalizeChars(game);
            game = RussianToLatin(game);
            game = SearchPattern(game);

            game = new Regex(@"(([[(].*[\)\]] )?([^([]+))").Match(game).Groups[1].Value;
            game = game.TrimStart().TrimEnd();
            return game;
        }

        /// <summary>
        /// Replace accented characters
        /// </summary>
        /// <param name="input">String to be parsed</param>
        /// <returns>String with characters replaced</returns>
        private string NormalizeChars(string input)
        {
            if (input == null)
                return null;

            string[,] charmap = {
                { "Á", "A" },   { "á", "a" },
                { "À", "A" },   { "à", "a" },
                { "Â", "A" },   { "â", "a" },
                { "Ä", "Ae" },  { "ä", "ae" },
                { "Ã", "A" },   { "ã", "a" },
                { "Å", "A" },   { "å", "a" },
                { "Æ", "Ae" },  { "æ", "ae" },
                { "Ç", "C" },   { "ç", "c" },
                { "Ð", "D" },   { "ð", "d" },
                { "É", "E" },   { "é", "e" },
                { "È", "E" },   { "è", "e" },
                { "Ê", "E" },   { "ê", "e" },
                { "Ë", "E" },   { "ë", "e" },
                { "ƒ", "f" },
                { "Í", "I" },   { "í", "i" },
                { "Ì", "I" },   { "ì", "i" },
                { "Î", "I" },   { "î", "i" },
                { "Ï", "I" },   { "ï", "i" },
                { "Ñ", "N" },   { "ñ", "n" },
                { "Ó", "O" },   { "ó", "o" },
                { "Ò", "O" },   { "ò", "o" },
                { "Ô", "O" },   { "ô", "o" },
                { "Ö", "Oe" },  { "ö", "oe" },
                { "Õ", "O" },   { "õ", "o" },
                { "Ø", "O" },   { "ø", "o" },
                { "Š", "S" },   { "š", "s" },
                { "ß", "ss" },
                { "Þ", "B" },   { "þ", "b" },
                { "Ú", "U" },   { "ú", "u" },
                { "Ù", "U" },   { "ù", "u" },
                { "Û", "U" },   { "û", "u" },
                { "Ü", "Ue" },  { "ü", "ue" },
                { "ÿ", "y" },
                { "Ý", "Y" },   { "ý", "y" },
                { "Ž", "Z" },   { "ž", "z" },
            };

            for (int i = 0; i < charmap.GetLength(0); i++)
            {
                input = input.Replace(charmap[i, 0], charmap[i, 1]);
            }

            return input;
        }

        /// <summary>
        /// Remove all unicode-specific chars from a string
        /// </summary>
        /// <param name="s">Input string to clean</param>
        /// <returns>Cleaned string</returns>
        private string RemoveUnicodeCharacters(string s)
        {
            if (s == null)
                return null;

            return new string(s.Where(c => c <= 255).ToArray());
        }  

        /// <summary>
        /// Convert Cyrillic lettering to Latin lettering
        /// </summary>
        /// <param name="input">String to be parsed</param>
        /// <returns>String with characters replaced</returns>
        private string RussianToLatin(string input)
        {
            if (input == null)
                return null;

            string[,] charmap = {
                    { "А", "A" }, { "Б", "B" }, { "В", "V" }, { "Г", "G" }, { "Д", "D" },
                    { "Е", "E" }, { "Ё", "Yo" }, { "Ж", "Zh" }, { "З", "Z" }, { "И", "I" },
                    { "Й", "J" }, { "К", "K" }, { "Л", "L" }, { "М", "M" }, { "Н", "N" },
                    { "О", "O" }, { "П", "P" }, { "Р", "R" }, { "С", "S" }, { "Т", "T" },
                    { "У", "U" }, { "Ф", "f" }, { "Х", "Kh" }, { "Ц", "Ts" }, { "Ч", "Ch" },
                    { "Ш", "Sh" }, { "Щ", "Sch" }, { "Ъ", string.Empty }, { "Ы", "y" }, { "Ь", string.Empty },
                    { "Э", "e" }, { "Ю", "yu" }, { "Я", "ya" }, { "а", "a" }, { "б", "b" },
                    { "в", "v" }, { "г", "g" }, { "д", "d" }, { "е", "e" }, { "ё", "yo" },
                    { "ж", "zh" }, { "з", "z" }, { "и", "i" }, { "й", "j" }, { "к", "k" },
                    { "л", "l" }, { "м", "m" }, { "н", "n" }, { "о", "o" }, { "п", "p" },
                    { "р", "r" }, { "с", "s" }, { "т", "t" }, { "у", "u" }, { "ф", "f" },
                    { "х", "kh" }, { "ц", "ts" }, { "ч", "ch" }, { "ш", "sh" }, { "щ", "sch" },
                    { "ъ", string.Empty }, { "ы", "y" }, { "ь", string.Empty }, { "э", "e" }, { "ю", "yu" },
                    { "я", "ya" },
            };

            for (int i = 0; i < charmap.GetLength(0); i++)
            {
                input = input.Replace(charmap[i, 0], charmap[i, 1]);
            }

            return input;
        }

        /// <summary>
        /// Replace special characters and patterns
        /// </summary>
        /// <param name="input">String to be parsed</param>
        /// <returns>String with characters replaced</returns>
        private string SearchPattern(string input)
        {
            if (input == null)
                return null;

            string[,] charmap = {
                { @"~", " - " },
                { @"_", " " },
                { @":", " " },
                { @">", ")" },
                { @"<", "(" },
                { @"\|", "-" },
                { "\"", "'" },
                { @"\*", "." },
                { @"\\", "-" },
                { @"/", "-" },
                { @"\?", " " },
                { @"\(([^)(]*)\(([^)]*)\)([^)(]*)\)", " " },
                { @"\(([^)]+)\)", " " },
                { @"\[([^]]+)\]", " " },
                { @"\{([^}]+)\}", " " },
                { @"(ZZZJUNK|ZZZ-UNK-|ZZZ-UNK |zzz unknow |zzz unk |Copy of |[.][a-z]{3}[.][a-z]{3}[.]|[.][a-z]{3}[.])", " " },
                { @" (r|rev|v|ver)\s*[\d\.]+[^\s]*", " " },
                { @"(( )|(\A))(\d{6}|\d{8})(( )|(\Z))", " " },
                { @"(( )|(\A))(\d{1,2})-(\d{1,2})-(\d{4}|\d{2})", " " },
                { @"(( )|(\A))(\d{4}|\d{2})-(\d{1,2})-(\d{1,2})", " " },
                { @"[-]+", "-" },
                { @"\A\s*\)", " " },
                { @"\A\s*(,|-)", " " },
                { @"\s+", " " },
                { @"\s+,", "," },
                { @"\s*(,|-)\s*\Z", " " },
            };

            for (int i = 0; i < charmap.GetLength(0); i++)
            {
                input = Regex.Replace(input, charmap[i, 0], charmap[i, 1]);
            }

            return input;
        }

        #endregion

        #region Filtering

        /// <summary>
        /// Split the parts of a filter statement
        /// </summary>
        /// <param name="filter">key:value where ~key/!key is negated</param>
        private (string field, string value, bool negate) ProcessFilterPair(string filter)
        {
            // If we don't even have a possible filter pair
            if (!filter.Contains(":"))
            {
                logger.Warning($"'{filter}` is not a valid filter string. Valid filter strings are of the form 'key:value'. Please refer to README.1ST or the help feature for more details.");
                return (null, null, false);
            }

            string filterTrimmed = filter.Trim('"', ' ', '\t');
            bool negate = filterTrimmed.StartsWith("!")
                || filterTrimmed.StartsWith("~")
                || filterTrimmed.StartsWith("not-");
            filterTrimmed = filterTrimmed.TrimStart('!', '~');
            filterTrimmed = filterTrimmed.StartsWith("not-") ? filterTrimmed[4..] : filterTrimmed;

            string filterFieldString = filterTrimmed.Split(':')[0].ToLowerInvariant().Trim('"', ' ', '\t');
            string filterValue = filterTrimmed[(filterFieldString.Length + 1)..].Trim('"', ' ', '\t');
        
            return (filterFieldString, filterValue, negate);
        }

        /// <summary>
        /// Check to see if a DatItem passes the filters
        /// </summary>
        /// <param name="datItem">DatItem to check</param>
        /// <param name="sub">True if this is a subitem, false otherwise</param>
        /// <returns>True if the item passed the filter, false otherwise</returns>
        /// TODO: Name can be made into a common one if name exists
        public bool PassesFilters(DatItem datItem, bool sub = false)
        {
            if (datItem == null)
                return false;

            #region Common

            // Filter on machine fields
            if (!PassesFilters(datItem.Machine))
                return false;

            // Filters for if we're a top-level item
            if (!sub)
            {
                // Filter on item type
                if (!Filter.PassStringFilter(DatItemFilter.Type, datItem.ItemType.ToString()))
                    return false;
            }

            #endregion
        
            #region Adjuster

            if (datItem is Adjuster adjuster)
            {
                // Filter on item name
                if (!Filter.PassStringFilter(DatItemFilter.Name, adjuster.Name))
                    return false;

                // Filter on default
                if (!Filter.PassBoolFilter(DatItemFilter.Default, adjuster.Default))
                    return false;

                // Filter on individual conditions
                if (adjuster.ConditionsSpecified)
                {
                    foreach (Condition condition in adjuster.Conditions)
                    {
                        if (!PassesFilters(condition, true))
                            return false;
                    }
                }
            }

            #endregion

            #region Analog

            else if (datItem is Analog analog)
            {
                // Filter on mask
                if (!Filter.PassStringFilter(DatItemFilter.Analog_Mask, analog.Mask))
                    return false;
            }

            #endregion

            #region Archive

            else if (datItem is Archive archive)
            {
                // Filter on item name
                if (!Filter.PassStringFilter(DatItemFilter.Name, archive.Name))
                    return false;
            }

            #endregion
            
            #region BiosSet

            else if (datItem is BiosSet biosSet)
            {
                // Filter on item name
                if (!Filter.PassStringFilter(DatItemFilter.Name, biosSet.Name))
                    return false;

                // Filter on description
                if (!Filter.PassStringFilter(DatItemFilter.Description, biosSet.Description))
                    return false;

                // Filter on default
                if (!Filter.PassBoolFilter(DatItemFilter.Default, biosSet.Default))
                    return false;
            }

            #endregion

            #region Chip

            else if (datItem is Chip chip)
            {
                // Filter on item name
                if (!Filter.PassStringFilter(DatItemFilter.Name, chip.Name))
                    return false;

                // DatItem_Tag
                if (!Filter.PassStringFilter(DatItemFilter.Tag, chip.Tag))
                    return false;

                // DatItem_ChipType
                if (DatItemFilter.ChipType.MatchesPositive(ChipType.NULL, chip.ChipType) == false)
                    return false;
                if (DatItemFilter.ChipType.MatchesNegative(ChipType.NULL, chip.ChipType) == true)
                    return false;

                // DatItem_Clock
                if (!Filter.PassLongFilter(DatItemFilter.Clock, chip.Clock))
                    return false;
            }

            #endregion

            #region Condition

            else if (datItem is Condition condition)
            {
                if (sub)
                {
                    // Filter on tag
                    if (!Filter.PassStringFilter(DatItemFilter.Condition_Tag, condition.Tag))
                        return false;

                    // Filter on mask
                    if (!Filter.PassStringFilter(DatItemFilter.Condition_Mask, condition.Mask))
                        return false;

                    // Filter on relation
                    if (DatItemFilter.Condition_Relation.MatchesPositive(Relation.NULL, condition.Relation) == false)
                        return false;
                    if (DatItemFilter.Condition_Relation.MatchesNegative(Relation.NULL, condition.Relation) == true)
                        return false;

                    // Filter on value
                    if (!Filter.PassStringFilter(DatItemFilter.Condition_Value, condition.Value))
                        return false;
                }
                else
                {
                    // Filter on tag
                    if (!Filter.PassStringFilter(DatItemFilter.Tag, condition.Tag))
                        return false;

                    // Filter on mask
                    if (!Filter.PassStringFilter(DatItemFilter.Mask, condition.Mask))
                        return false;

                    // Filter on relation
                    if (DatItemFilter.Relation.MatchesPositive(Relation.NULL, condition.Relation) == false)
                        return false;
                    if (DatItemFilter.Relation.MatchesNegative(Relation.NULL, condition.Relation) == true)
                        return false;

                    // Filter on value
                    if (!Filter.PassStringFilter(DatItemFilter.Value, condition.Value))
                        return false;
                }
            }

            #endregion

            #region Configuration

            else if (datItem is Configuration configuration)
            {
                // Filter on item name
                if (!Filter.PassStringFilter(DatItemFilter.Name, configuration.Name))
                    return false;

                // Filter on tag
                if (!Filter.PassStringFilter(DatItemFilter.Tag, configuration.Tag))
                    return false;

                // Filter on mask
                if (!Filter.PassStringFilter(DatItemFilter.Mask, configuration.Mask))
                    return false;

                // Filter on individual conditions
                if (configuration.ConditionsSpecified)
                {
                    foreach (Condition subCondition in configuration.Conditions)
                    {
                        if (!PassesFilters(subCondition, true))
                            return false;
                    }
                }

                // Filter on individual locations
                if (configuration.LocationsSpecified)
                {
                    foreach (Location subLocation in configuration.Locations)
                    {
                        if (!PassesFilters(subLocation, true))
                            return false;
                    }
                }

                // Filter on individual conditions
                if (configuration.SettingsSpecified)
                {
                    foreach (Setting subSetting in configuration.Settings)
                    {
                        if (!PassesFilters(subSetting, true))
                            return false;
                    }
                }
            }

            #endregion

            #region Control

            else if (datItem is Control control)
            {
                // Filter on control type
                if (DatItemFilter.Control_Type.MatchesPositive(ControlType.NULL, control.ControlType) == false)
                    return false;
                if (DatItemFilter.Control_Type.MatchesNegative(ControlType.NULL, control.ControlType) == true)
                    return false;

                // Filter on player
                if (!Filter.PassLongFilter(DatItemFilter.Control_Player, control.Player))
                    return false;

                // Filter on buttons
                if (!Filter.PassLongFilter(DatItemFilter.Control_Buttons, control.Buttons))
                    return false;

                // Filter on reqbuttons
                if (!Filter.PassLongFilter(DatItemFilter.Control_ReqButtons, control.RequiredButtons))
                    return false;

                // Filter on minimum
                if (!Filter.PassLongFilter(DatItemFilter.Control_Minimum, control.Minimum))
                    return false;

                // Filter on maximum
                if (!Filter.PassLongFilter(DatItemFilter.Control_Maximum, control.Maximum))
                    return false;

                // Filter on sensitivity
                if (!Filter.PassLongFilter(DatItemFilter.Control_Sensitivity, control.Sensitivity))
                    return false;

                // Filter on keydelta
                if (!Filter.PassLongFilter(DatItemFilter.Control_KeyDelta, control.KeyDelta))
                    return false;

                // Filter on reverse
                if (!Filter.PassBoolFilter(DatItemFilter.Control_Reverse, control.Reverse))
                    return false;

                // Filter on ways
                if (!Filter.PassStringFilter(DatItemFilter.Control_Ways, control.Ways))
                    return false;

                // Filter on ways2
                if (!Filter.PassStringFilter(DatItemFilter.Control_Ways2, control.Ways2))
                    return false;

                // Filter on ways3
                if (!Filter.PassStringFilter(DatItemFilter.Control_Ways3, control.Ways3))
                    return false;
            }

            #endregion

            #region DataArea

            else if (datItem is DataArea dataArea)
            {
                // Filter on area name
                if (!Filter.PassStringFilter(DatItemFilter.AreaName, dataArea.Name))
                    return false;

                // Filter on area size
                if (!Filter.PassLongFilter(DatItemFilter.AreaSize, dataArea.Size))
                    return false;

                // Filter on area width
                if (!Filter.PassLongFilter(DatItemFilter.AreaWidth, dataArea.Width))
                    return false;

                // Filter on area endianness
                if (DatItemFilter.AreaEndianness.MatchesPositive(Endianness.NULL, dataArea.Endianness) == false)
                    return false;
                if (DatItemFilter.AreaEndianness.MatchesNegative(Endianness.NULL, dataArea.Endianness) == true)
                    return false;
            }

            #endregion

            #region Device

            else if (datItem is Device device)
            {
                // Filter on device type
                if (DatItemFilter.DeviceType.MatchesPositive(DeviceType.NULL, device.DeviceType) == false)
                    return false;
                if (DatItemFilter.DeviceType.MatchesNegative(DeviceType.NULL, device.DeviceType) == true)
                    return false;

                // Filter on tag
                if (!Filter.PassStringFilter(DatItemFilter.Tag, device.Tag))
                    return false;

                // Filter on fixed image
                if (!Filter.PassStringFilter(DatItemFilter.FixedImage, device.FixedImage))
                    return false;

                // Filter on mandatory
                if (!Filter.PassLongFilter(DatItemFilter.Mandatory, device.Mandatory))
                    return false;

                // Filter on interface
                if (!Filter.PassStringFilter(DatItemFilter.Interface, device.Interface))
                    return false;

                // Filter on individual instances
                if (device.InstancesSpecified)
                {
                    foreach (Instance subInstance in device.Instances)
                    {
                        if (!PassesFilters(subInstance, true))
                            return false;
                    }
                }

                // Filter on individual extensions
                if (device.ExtensionsSpecified)
                {
                    foreach (Extension subExtension in device.Extensions)
                    {
                        if (!PassesFilters(subExtension, true))
                            return false;
                    }
                }
            }

            #endregion

            #region DeviceReference

            else if (datItem is DeviceReference deviceReference)
            {
                // Filter on item name
                if (!Filter.PassStringFilter(DatItemFilter.Name, deviceReference.Name))
                    return false;
            }

            #endregion

            #region DipSwitch

            else if (datItem is DipSwitch dipSwitch)
            {
                #region Common

                // Filter on item name
                if (!Filter.PassStringFilter(DatItemFilter.Name, dipSwitch.Name))
                    return false;

                // Filter on tag
                if (!Filter.PassStringFilter(DatItemFilter.Tag, dipSwitch.Tag))
                    return false;

                // Filter on mask
                if (!Filter.PassStringFilter(DatItemFilter.Mask, dipSwitch.Mask))
                    return false;

                // Filter on individual conditions
                if (dipSwitch.ConditionsSpecified)
                {
                    foreach (Condition subCondition in dipSwitch.Conditions)
                    {
                        if (!PassesFilters(subCondition, true))
                            return false;
                    }
                }

                // Filter on individual locations
                if (dipSwitch.LocationsSpecified)
                {
                    foreach (Location subLocation in dipSwitch.Locations)
                    {
                        if (!PassesFilters(subLocation, true))
                            return false;
                    }
                }

                // Filter on individual conditions
                if (dipSwitch.ValuesSpecified)
                {
                    foreach (Setting subValue in dipSwitch.Values)
                    {
                        if (!PassesFilters(subValue, true))
                            return false;
                    }
                }

                #endregion

                #region SoftwareList

                // Filter on Part
                if (dipSwitch.PartSpecified)
                {
                    if (!PassesFilters(dipSwitch.Part, true))
                        return false;
                }

                #endregion
            }

            #endregion

            #region Disk

            else if (datItem is Disk disk)
            {
                #region Common

                // Filter on item name
                if (!Filter.PassStringFilter(DatItemFilter.Name, disk.Name))
                    return false;

                // Filter on MD5
                if (!Filter.PassStringFilter(DatItemFilter.MD5, disk.MD5))
                    return false;

                // Filter on SHA-1
                if (!Filter.PassStringFilter(DatItemFilter.SHA1, disk.SHA1))
                    return false;

                // Filter on merge tag
                if (!Filter.PassStringFilter(DatItemFilter.Merge, disk.MergeTag))
                    return false;

                // Filter on region
                if (!Filter.PassStringFilter(DatItemFilter.Region, disk.Region))
                    return false;

                // Filter on index
                if (!Filter.PassStringFilter(DatItemFilter.Index, disk.Index))
                    return false;

                // Filter on writable
                if (!Filter.PassBoolFilter(DatItemFilter.Writable, disk.Writable))
                    return false;

                // Filter on status
                if (DatItemFilter.Status.MatchesPositive(ItemStatus.NULL, disk.ItemStatus) == false)
                    return false;
                if (DatItemFilter.Status.MatchesNegative(ItemStatus.NULL, disk.ItemStatus) == true)
                    return false;

                // Filter on optional
                if (!Filter.PassBoolFilter(DatItemFilter.Optional, disk.Optional))
                    return false;

                #endregion

                #region SoftwareList

                // Filter on DiskArea
                if (disk.DiskAreaSpecified)
                {
                    if (!PassesFilters(disk.DiskArea, true))
                        return false;
                }

                // Filter on Part
                if (disk.PartSpecified)
                {
                    if (!PassesFilters(disk.Part, true))
                        return false;
                }

                #endregion
            }

            #endregion

            #region DiskArea

            else if (datItem is DiskArea diskArea)
            {
                // Filter on area name
                if (!Filter.PassStringFilter(DatItemFilter.AreaName, diskArea.Name))
                    return false;
            }

            #endregion

            #region Display

            else if (datItem is Display display)
            {
                // Filter on tag
                if (!Filter.PassStringFilter(DatItemFilter.Tag, display.Tag))
                    return false;

                // Filter on display type
                if (DatItemFilter.DisplayType.MatchesPositive(DisplayType.NULL, display.DisplayType) == false)
                    return false;
                if (DatItemFilter.DisplayType.MatchesNegative(DisplayType.NULL, display.DisplayType) == true)
                    return false;

                // Filter on rotation
                if (!Filter.PassLongFilter(DatItemFilter.Rotate, display.Rotate))
                    return false;

                // Filter on flipx
                if (!Filter.PassBoolFilter(DatItemFilter.FlipX, display.FlipX))
                    return false;

                // Filter on width
                if (!Filter.PassLongFilter(DatItemFilter.Width, display.Width))
                    return false;

                // Filter on height
                if (!Filter.PassLongFilter(DatItemFilter.Height, display.Height))
                    return false;

                // Filter on refresh
                if (!Filter.PassDoubleFilter(DatItemFilter.Refresh, display.Refresh))
                    return false;

                // Filter on pixclock
                if (!Filter.PassLongFilter(DatItemFilter.PixClock, display.PixClock))
                    return false;

                // Filter on htotal
                if (!Filter.PassLongFilter(DatItemFilter.HTotal, display.HTotal))
                    return false;

                // Filter on hbend
                if (!Filter.PassLongFilter(DatItemFilter.HBEnd, display.HBEnd))
                    return false;

                // Filter on hbstart
                if (!Filter.PassLongFilter(DatItemFilter.HBStart, display.HBStart))
                    return false;

                // Filter on vtotal
                if (!Filter.PassLongFilter(DatItemFilter.VTotal, display.VTotal))
                    return false;

                // Filter on vbend
                if (!Filter.PassLongFilter(DatItemFilter.VBEnd, display.VBEnd))
                    return false;

                // Filter on vbstart
                if (!Filter.PassLongFilter(DatItemFilter.VBStart, display.VBStart))
                    return false;
            }

            #endregion

            #region Driver

            else if (datItem is Driver driver)
            {
                // Filter on status
                if (DatItemFilter.SupportStatus.MatchesPositive(SupportStatus.NULL, driver.Status) == false)
                    return false;
                if (DatItemFilter.SupportStatus.MatchesNegative(SupportStatus.NULL, driver.Status) == true)
                    return false;

                // Filter on emulation
                if (DatItemFilter.EmulationStatus.MatchesPositive(SupportStatus.NULL, driver.Emulation) == false)
                    return false;
                if (DatItemFilter.EmulationStatus.MatchesNegative(SupportStatus.NULL, driver.Emulation) == true)
                    return false;

                // Filter on cocktail
                if (DatItemFilter.CocktailStatus.MatchesPositive(SupportStatus.NULL, driver.Cocktail) == false)
                    return false;
                if (DatItemFilter.CocktailStatus.MatchesNegative(SupportStatus.NULL, driver.Cocktail) == true)
                    return false;

                // Filter on savestate
                if (DatItemFilter.SaveStateStatus.MatchesPositive(Supported.NULL, driver.SaveState) == false)
                    return false;
                if (DatItemFilter.SaveStateStatus.MatchesNegative(Supported.NULL, driver.SaveState) == true)
                    return false;
            }

            #endregion

            #region Extension

            else if (datItem is Extension extension)
            {
                // Filter on item name
                if (!Filter.PassStringFilter(DatItemFilter.Extension_Name, extension.Name))
                    return false;
            }

            #endregion

            #region Feature

            else if (datItem is Feature feature)
            {
                // Filter on type
                if (DatItemFilter.FeatureType.MatchesPositive(FeatureType.NULL, feature.Type) == false)
                    return false;
                if (DatItemFilter.FeatureType.MatchesNegative(FeatureType.NULL, feature.Type) == true)
                    return false;

                // Filter on status
                if (DatItemFilter.FeatureStatus.MatchesPositive(FeatureStatus.NULL, feature.Status) == false)
                    return false;
                if (DatItemFilter.FeatureStatus.MatchesNegative(FeatureStatus.NULL, feature.Status) == true)
                    return false;

                // Filter on overall
                if (DatItemFilter.FeatureOverall.MatchesPositive(FeatureStatus.NULL, feature.Overall) == false)
                    return false;
                if (DatItemFilter.FeatureOverall.MatchesNegative(FeatureStatus.NULL, feature.Overall) == true)
                    return false;
            }

            #endregion

            #region Info

            else if (datItem is Info info)
            {
                // Filter on item name
                if (!Filter.PassStringFilter(DatItemFilter.Name, info.Name))
                    return false;

                // Filter on info value
                if (!Filter.PassStringFilter(DatItemFilter.Value, info.Value))
                    return false;
            }

            #endregion

            #region Input

            else if (datItem is Input input)
            {
                // Filter on service
                if (!Filter.PassBoolFilter(DatItemFilter.Service, input.Service))
                    return false;

                // Filter on tilt
                if (!Filter.PassBoolFilter(DatItemFilter.Tilt, input.Tilt))
                    return false;

                // Filter on players
                if (!Filter.PassLongFilter(DatItemFilter.Players, input.Players))
                    return false;

                // Filter on coins
                if (!Filter.PassLongFilter(DatItemFilter.Coins, input.Coins))
                    return false;

                // Filter on individual controls
                if (input.ControlsSpecified)
                {
                    foreach (Control subControl in input.Controls)
                    {
                        if (!PassesFilters(subControl, true))
                            return false;
                    }
                }
            }

            #endregion

            #region Instance

            else if (datItem is Instance instance)
            {
                // Filter on item name
                if (!Filter.PassStringFilter(DatItemFilter.Instance_Name, instance.Name))
                    return false;

                // Filter on brief name
                if (!Filter.PassStringFilter(DatItemFilter.Instance_BriefName, instance.BriefName))
                    return false;
            }

            #endregion

            #region Location

            else if (datItem is Location location)
            {
                // Filter on item name
                if (!Filter.PassStringFilter(DatItemFilter.Location_Name, location.Name))
                    return false;

                // Filter on number
                if (!Filter.PassLongFilter(DatItemFilter.Location_Number, location.Number))
                    return false;

                // Filter on inverted
                if (!Filter.PassBoolFilter(DatItemFilter.Location_Inverted, location.Inverted))
                    return false;
            }

            #endregion

            #region Media

            else if (datItem is Media media)
            {
                // Filter on item name
                if (!Filter.PassStringFilter(DatItemFilter.Name, media.Name))
                    return false;

                // Filter on MD5
                if (!Filter.PassStringFilter(DatItemFilter.MD5, media.MD5))
                    return false;

                // Filter on SHA-1
                if (!Filter.PassStringFilter(DatItemFilter.SHA1, media.SHA1))
                    return false;

                // Filter on SHA-256
                if (!Filter.PassStringFilter(DatItemFilter.SHA256, media.SHA256))
                    return false;

                // Filter on SpamSum
                if (!Filter.PassStringFilter(DatItemFilter.SpamSum, media.SpamSum))
                    return false;
            }

            #endregion

            #region Part

            else if (datItem is Part part)
            {
                // Filter on part name
                if (!Filter.PassStringFilter(DatItemFilter.Part_Name, part.Name))
                    return false;

                // Filter on part interface
                if (!Filter.PassStringFilter(DatItemFilter.Part_Interface, part.Interface))
                    return false;

                // Filter on features
                if (part.FeaturesSpecified)
                {
                    foreach (PartFeature subPartFeature in part.Features)
                    {
                        if (!PassesFilters(subPartFeature, true))
                            return false;
                    }
                }
            }

            #endregion

            #region PartFeature

            else if (datItem is PartFeature partFeature)
            {
                // Filter on name
                if (!Filter.PassStringFilter(DatItemFilter.Part_Feature_Name, partFeature.Name))
                    return false;

                // Filter on value
                if (!Filter.PassStringFilter(DatItemFilter.Part_Feature_Value, partFeature.Value))
                    return false;
            }

            #endregion

            #region Port

            else if (datItem is Port port)
            {
                // Filter on tag
                if (!Filter.PassStringFilter(DatItemFilter.Tag, port.Tag))
                    return false;

                // Filter on individual analogs
                if (port.AnalogsSpecified)
                {
                    foreach (Analog subAnalog in port.Analogs)
                    {
                        if (!PassesFilters(subAnalog, true))
                            return false;
                    }
                }
            }

            #endregion

            #region RamOption

            else if (datItem is RamOption ramOption)
            {
                // Filter on item name
                if (!Filter.PassStringFilter(DatItemFilter.Name, ramOption.Name))
                    return false;

                // Filter on default
                if (!Filter.PassBoolFilter(DatItemFilter.Default, ramOption.Default))
                    return false;

                // Filter on content
                if (!Filter.PassStringFilter(DatItemFilter.Content, ramOption.Content))
                    return false;
            }

            #endregion

            #region Release

            else if (datItem is Release release)
            {
                // Filter on item name
                if (!Filter.PassStringFilter(DatItemFilter.Name, release.Name))
                    return false;

                // Filter on region
                if (!Filter.PassStringFilter(DatItemFilter.Region, release.Region))
                    return false;

                // Filter on language
                if (!Filter.PassStringFilter(DatItemFilter.Language, release.Language))
                    return false;

                // Filter on date
                if (!Filter.PassStringFilter(DatItemFilter.Date, release.Date))
                    return false;

                // Filter on default
                if (!Filter.PassBoolFilter(DatItemFilter.Default, release.Default))
                    return false;
            }

            #endregion

            #region Rom

            else if (datItem is Rom rom)
            {
                #region Common

                // Filter on item name
                if (!Filter.PassStringFilter(DatItemFilter.Name, rom.Name))
                    return false;

                // Filter on bios
                if (!Filter.PassStringFilter(DatItemFilter.Bios, rom.Bios))
                    return false;

                // Filter on rom size
                if (!Filter.PassLongFilter(DatItemFilter.Size, rom.Size))
                    return false;

                // Filter on CRC
                if (!Filter.PassStringFilter(DatItemFilter.CRC, rom.CRC))
                    return false;

                // Filter on MD5
                if (!Filter.PassStringFilter(DatItemFilter.MD5, rom.MD5))
                    return false;

                // Filter on SHA-1
                if (!Filter.PassStringFilter(DatItemFilter.SHA1, rom.SHA1))
                    return false;

                // Filter on SHA-256
                if (!Filter.PassStringFilter(DatItemFilter.SHA256, rom.SHA256))
                    return false;

                // Filter on SHA-384
                if (!Filter.PassStringFilter(DatItemFilter.SHA384, rom.SHA384))
                    return false;

                // Filter on SHA-512
                if (!Filter.PassStringFilter(DatItemFilter.SHA512, rom.SHA512))
                    return false;

                // Filter on SpamSum
                if (!Filter.PassStringFilter(DatItemFilter.SpamSum, rom.SpamSum))
                    return false;

                // Filter on merge tag
                if (!Filter.PassStringFilter(DatItemFilter.Merge, rom.MergeTag))
                    return false;

                // Filter on region
                if (!Filter.PassStringFilter(DatItemFilter.Region, rom.Region))
                    return false;

                // Filter on offset
                if (!Filter.PassStringFilter(DatItemFilter.Offset, rom.Offset))
                    return false;

                // Filter on date
                if (!Filter.PassStringFilter(DatItemFilter.Date, rom.Date))
                    return false;

                // Filter on status
                if (DatItemFilter.Status.MatchesPositive(ItemStatus.NULL, rom.ItemStatus) == false)
                    return false;
                if (DatItemFilter.Status.MatchesNegative(ItemStatus.NULL, rom.ItemStatus) == true)
                    return false;

                // Filter on optional
                if (!Filter.PassBoolFilter(DatItemFilter.Optional, rom.Optional))
                    return false;

                // Filter on inverted
                if (!Filter.PassBoolFilter(DatItemFilter.Inverted, rom.Inverted))
                    return false;

                #endregion

                #region AttractMode

                // Filter on alt name
                if (!Filter.PassStringFilter(DatItemFilter.AltName, rom.AltName))
                    return false;

                // Filter on alt title
                if (!Filter.PassStringFilter(DatItemFilter.AltTitle, rom.AltTitle))
                    return false;

                #endregion

                #region OpenMSX

                // Filter on original
                if (!Filter.PassStringFilter(DatItemFilter.Original, rom.Original?.Content))
                    return false;

                // Filter on OpenMSX subtype
                if (DatItemFilter.OpenMSXSubType.MatchesPositive(OpenMSXSubType.NULL, rom.OpenMSXSubType) == false)
                    return false;
                if (DatItemFilter.OpenMSXSubType.MatchesNegative(OpenMSXSubType.NULL, rom.OpenMSXSubType) == true)
                    return false;

                // Filter on OpenMSX type
                if (!Filter.PassStringFilter(DatItemFilter.OpenMSXType, rom.OpenMSXType))
                    return false;

                // Filter on remark
                if (!Filter.PassStringFilter(DatItemFilter.Remark, rom.Remark))
                    return false;

                // Filter on boot
                if (!Filter.PassStringFilter(DatItemFilter.Boot, rom.Boot))
                    return false;

                #endregion

                #region SoftwareList

                // Filter on load flag
                if (DatItemFilter.LoadFlag.MatchesPositive(LoadFlag.NULL, rom.LoadFlag) == false)
                    return false;
                if (DatItemFilter.LoadFlag.MatchesNegative(LoadFlag.NULL, rom.LoadFlag) == true)
                    return false;

                // Filter on value
                if (!Filter.PassStringFilter(DatItemFilter.Value, rom.Value))
                    return false;

                // Filter on DataArea
                if (rom.DataAreaSpecified)
                {
                    if (!PassesFilters(rom.DataArea, true))
                        return false;
                }

                // Filter on Part
                if (rom.PartSpecified)
                {
                    if (!PassesFilters(rom.Part, true))
                        return false;
                }

                #endregion
            }

            #endregion

            #region Sample

            else if (datItem is Sample sample)
            {
                // Filter on item name
                if (!Filter.PassStringFilter(DatItemFilter.Name, sample.Name))
                    return false;
            }

            #endregion

            #region Setting

            else if (datItem is Setting setting)
            {
                // Filter on item name
                if (!Filter.PassStringFilter(DatItemFilter.Setting_Name, setting.Name))
                    return false;

                // Filter on value
                if (!Filter.PassStringFilter(DatItemFilter.Setting_Value, setting.Value))
                    return false;

                // Filter on default
                if (!Filter.PassBoolFilter(DatItemFilter.Setting_Default, setting.Default))
                    return false;

                // Filter on individual conditions
                if (setting.ConditionsSpecified)
                {
                    foreach (Condition subCondition in setting.Conditions)
                    {
                        if (!PassesFilters(subCondition, true))
                            return false;
                    }
                }
            }

            #endregion

            #region SharedFeature

            else if (datItem is SharedFeature sharedFeature)
            {
                // Filter on item name
                if (!Filter.PassStringFilter(DatItemFilter.Name, sharedFeature.Name))
                    return false;

                // Filter on value
                if (!Filter.PassStringFilter(DatItemFilter.Value, sharedFeature.Value))
                    return false;
            }

            #endregion

            #region Slot

            else if (datItem is Slot slot)
            {
                // Filter on item name
                if (!Filter.PassStringFilter(DatItemFilter.Name, slot.Name))
                    return false;

                // Filter on individual slot options
                if (slot.SlotOptionsSpecified)
                {
                    foreach (SlotOption subSlotOption in slot.SlotOptions)
                    {
                        if (!PassesFilters(subSlotOption, true))
                            return false;
                    }
                }
            }

            #endregion

            #region SlotOption

            else if (datItem is SlotOption slotOption)
            {
                // Filter on item name
                if (!Filter.PassStringFilter(DatItemFilter.SlotOption_Name, slotOption.Name))
                    return false;

                // Filter on device name
                if (!Filter.PassStringFilter(DatItemFilter.SlotOption_DeviceName, slotOption.DeviceName))
                    return false;

                // Filter on default
                if (!Filter.PassBoolFilter(DatItemFilter.SlotOption_Default, slotOption.Default))
                    return false;
            }

            #endregion

            #region SoftwareList

            else if (datItem is SoftwareList softwareList)
            {
                // Filter on item name
                if (!Filter.PassStringFilter(DatItemFilter.Name, softwareList.Name))
                    return false;

                // Filter on status
                if (DatItemFilter.SoftwareListStatus.MatchesPositive(SoftwareListStatus.NULL, softwareList.Status) == false)
                    return false;
                if (DatItemFilter.SoftwareListStatus.MatchesNegative(SoftwareListStatus.NULL, softwareList.Status) == true)
                    return false;

                // Filter on filter
                if (!Filter.PassStringFilter(DatItemFilter.Filter, softwareList.Filter))
                    return false;
            }

            #endregion

            #region Sound

            else if (datItem is Sound sound)
            {
                // Filter on channels
                if (!Filter.PassLongFilter(DatItemFilter.Channels, sound.Channels))
                    return false;
            }

            #endregion

            return true;
        }

        /// <summary>
        /// Check to see if a Machine passes the filters
        /// </summary>
        /// <param name="machine">Machine to check</param>
        /// <returns>True if the machine passed the filter, false otherwise</returns>
        public bool PassesFilters(Machine machine)
        {
            if (machine == null)
                return false;

            #region Common

            // Machine_Name
            bool passes = Filter.PassStringFilter(MachineFilter.Name, machine.Name);
            if (MachineFilter.IncludeOfInGame)
            {
                passes |= Filter.PassStringFilter(MachineFilter.Name, machine.CloneOf);
                passes |= Filter.PassStringFilter(MachineFilter.Name, machine.RomOf);
            }
            if (!passes)
                return false;

            // Machine_Comment
            if (!Filter.PassStringFilter(MachineFilter.Comment, machine.Comment))
                return false;

            // Machine_Description
            if (!Filter.PassStringFilter(MachineFilter.Description, machine.Description))
                return false;

            // Machine_Year
            if (!Filter.PassStringFilter(MachineFilter.Year, machine.Year))
                return false;

            // Machine_Manufacturer
            if (!Filter.PassStringFilter(MachineFilter.Manufacturer, machine.Manufacturer))
                return false;

            // Machine_Publisher
            if (!Filter.PassStringFilter(MachineFilter.Publisher, machine.Publisher))
                return false;

            // Machine_Category
            if (!Filter.PassStringFilter(MachineFilter.Category, machine.Category))
                return false;

            // Machine_RomOf
            if (!Filter.PassStringFilter(MachineFilter.RomOf, machine.RomOf))
                return false;

            // Machine_CloneOf
            if (!Filter.PassStringFilter(MachineFilter.CloneOf, machine.CloneOf))
                return false;

            // Machine_SampleOf
            if (!Filter.PassStringFilter(MachineFilter.SampleOf, machine.SampleOf))
                return false;

            // Machine_Type
            if (MachineFilter.Type.MatchesPositive(0x0, machine.MachineType) == false)
                return false;
            if (MachineFilter.Type.MatchesNegative(0x0, machine.MachineType) == true)
                return false;

            #endregion

            #region AttractMode

            // Machine_Players
            if (!Filter.PassStringFilter(MachineFilter.Players, machine.Players))
                return false;

            // Machine_Rotation
            if (!Filter.PassStringFilter(MachineFilter.Rotation, machine.Rotation))
                return false;

            // Machine_Control
            if (!Filter.PassStringFilter(MachineFilter.Control, machine.Control))
                return false;

            // Machine_Status
            if (!Filter.PassStringFilter(MachineFilter.Status, machine.Status))
                return false;

            // Machine_DisplayCount
            if (!Filter.PassStringFilter(MachineFilter.DisplayCount, machine.DisplayCount))
                return false;

            // Machine_DisplayType
            if (!Filter.PassStringFilter(MachineFilter.DisplayType, machine.DisplayType))
                return false;

            // Machine_Buttons
            if (!Filter.PassStringFilter(MachineFilter.Buttons, machine.Buttons))
                return false;

            #endregion

            #region ListXML

            // Machine_History
            if (!Filter.PassStringFilter(MachineFilter.History, machine.History))
                return false;

            // Machine_SourceFile
            if (!Filter.PassStringFilter(MachineFilter.SourceFile, machine.SourceFile))
                return false;

            // Machine_Runnable
            if (MachineFilter.Runnable.MatchesPositive(Runnable.NULL, machine.Runnable) == false)
                return false;
            if (MachineFilter.Runnable.MatchesNegative(Runnable.NULL, machine.Runnable) == true)
                return false;

            #endregion

            #region Logiqx

            // Machine_Board
            if (!Filter.PassStringFilter(MachineFilter.Board, machine.Board))
                return false;

            // Machine_RebuildTo
            if (!Filter.PassStringFilter(MachineFilter.RebuildTo, machine.RebuildTo))
                return false;

            #endregion

            #region Logiqx EmuArc

            // Machine_TitleID
            if (!Filter.PassStringFilter(MachineFilter.TitleID, machine.TitleID))
                return false;

            // Machine_Developer
            if (!Filter.PassStringFilter(MachineFilter.Developer, machine.Developer))
                return false;

            // Machine_Genre
            if (!Filter.PassStringFilter(MachineFilter.Genre, machine.Genre))
                return false;

            // Machine_Subgenre
            if (!Filter.PassStringFilter(MachineFilter.Subgenre, machine.Subgenre))
                return false;

            // Machine_Ratings
            if (!Filter.PassStringFilter(MachineFilter.Ratings, machine.Ratings))
                return false;

            // Machine_Score
            if (!Filter.PassStringFilter(MachineFilter.Score, machine.Score))
                return false;

            // Machine_Enabled
            if (!Filter.PassStringFilter(MachineFilter.Enabled, machine.Enabled))
                return false;

            // Machine_CRC
            if (!Filter.PassBoolFilter(MachineFilter.CRC, machine.Crc))
                return false;

            // Machine_RelatedTo
            if (!Filter.PassStringFilter(MachineFilter.RelatedTo, machine.RelatedTo))
                return false;

            #endregion

            #region OpenMSX

            // Machine_GenMSXID
            if (!Filter.PassStringFilter(MachineFilter.GenMSXID, machine.GenMSXID))
                return false;

            // Machine_System
            if (!Filter.PassStringFilter(MachineFilter.System, machine.System))
                return false;

            // Machine_Country
            if (!Filter.PassStringFilter(MachineFilter.Country, machine.Country))
                return false;

            #endregion

            #region SoftwareList

            // Machine_Supported
            if (MachineFilter.Supported.MatchesPositive(Supported.NULL, machine.Supported) == false)
                return false;
            if (MachineFilter.Supported.MatchesNegative(Supported.NULL, machine.Supported) == true)
                return false;

            #endregion // SoftwareList

            return true;
        }

        #endregion
    }
}
