using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
    /// TODO: Split out helper classes
    public class Cleaner
    {
        #region Exclusion Fields

        /// <summary>
        /// DatItemRemover to remove fields from DatHeaders
        /// </summary>
        public DatHeaderRemover DatHeaderRemover { get; set; }

        /// <summary>
        /// DatItemRemover to remove fields from DatItems
        /// </summary>
        public DatItemRemover DatItemRemover { get; set; }

        #endregion

        #region Filter Fields

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
            // Instantiate the removers, if necessary
            DatHeaderRemover ??= new DatHeaderRemover();
            DatItemRemover ??= new DatItemRemover();

            // If the list is null or empty, just return
            if (fields == null || fields.Count == 0)
                return;

            foreach (string field in fields)
            {                
                // If we don't even have a possible field name
                if (field == null)
                    continue;

                // DatHeader fields
                if (DatHeaderRemover.SetRemover(field))
                    continue;

                // Machine and DatItem fields
                if (DatItemRemover.SetRemover(field))
                    continue;

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
                logger.Warning($"The value {field} did not match any filterable field names. Please check the wiki for more details on supported field names.");
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
        /// <returns>True if the item passed the filter, false otherwise</returns>
        public bool PassesFilters(DatItem datItem)
        {
            // Null item means it will never pass
            if (datItem == null)
                return false;

            // Filter on Machine fields
            if (!MachineFilter.PassesFilters(datItem.Machine))
                return false;

            // Filter on DatItem fields
            return DatItemFilter.PassesFilters(datItem);
        }

        #endregion
    
        #region Removal

        /// <summary>
        /// Remove fields as per the header
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        public void RemoveFieldsFromItems(DatFile datFile)
        {
            // If the removers don't exist, we can't use it
            if (DatHeaderRemover == null && DatItemRemover == null)
                return;

            // Output the logging statement
            logger.User("Removing filtered fields");

            // Remove DatHeader fields
            if (DatHeaderRemover != null)
                DatHeaderRemover.RemoveFields(datFile.Header);

            // Remove DatItem and Machine fields
            if (DatItemRemover != null)
            {
                Parallel.ForEach(datFile.Items.Keys, Globals.ParallelOptions, key =>
                {
                    List<DatItem> items = datFile.Items[key];
                    for (int j = 0; j < items.Count; j++)
                    {
                        DatItemRemover.RemoveFields(items[j]);
                    }

                    datFile.Items.Remove(key);
                    datFile.Items.AddRange(key, items);
                });
            }
        }

        #endregion
    }
}
