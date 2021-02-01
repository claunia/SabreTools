using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatFiles;
using SabreTools.DatItems;
using SabreTools.Logging;

[assembly: InternalsVisibleTo("SabreTools.Test")]
namespace SabreTools.Filtering
{
    /// <summary>
    /// Represents the cleaning operations that need to be performed on a set of items, usually a DAT
    /// </summary>

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
        /// Apply cleaning methods to the DatFile
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        /// <returns>True if cleaning was successful, false on error</returns>
        public bool ApplyCleaning(DatFile datFile, bool throwOnError = false)
        {
            try
            {
                // Perform item-level cleaning
                CleanDatItems(datFile);

                // Bucket and dedupe according to the flag
                if (DedupeRoms == DedupeType.Full)
                    datFile.Items.BucketBy(ItemKey.CRC, DedupeRoms);
                else if (DedupeRoms == DedupeType.Game)
                    datFile.Items.BucketBy(ItemKey.Machine, DedupeRoms);

                // Process description to machine name
                if (DescriptionAsName == true)
                    MachineDescriptionToName(datFile);

                // If we are removing scene dates, do that now
                if (SceneDateStrip == true)
                    StripSceneDatesFromItems(datFile);

                // Run the one rom per game logic, if required
                if (OneGamePerRegion == true)
                    SetOneGamePerRegion(datFile);

                // Run the one rom per game logic, if required
                if (OneRomPerGame == true)
                    SetOneRomPerGame(datFile);

                // If we are removing fields, do that now
                RemoveFieldsFromItems(datFile);

                // Remove all marked items
                datFile.Items.ClearMarked();

                // We remove any blanks, if we aren't supposed to have any
                if (KeepEmptyGames == false)
                    datFile.Items.ClearEmpty();
            }
            catch (Exception ex) when (!throwOnError)
            {
                logger.Error(ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Clean individual items based on the current filter
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        internal void CleanDatItems(DatFile datFile)
        {
            List<string> keys = datFile.Items.Keys.ToList();
            foreach (string key in keys)
            {
                // For every item in the current key
                List<DatItem> items = datFile.Items[key];
                foreach (DatItem item in items)
                {
                    // If we have a null item, we can't clean it it
                    if (item == null)
                        continue;

                    // Run cleaning per item
                    CleanDatItem(item);
                }

                // Assign back for caution
                datFile.Items[key] = items;
            }
        }

        /// <summary>
        /// Clean a DatItem according to the cleaner
        /// </summary>
        /// <param name="datItem">DatItem to clean</param>
        internal void CleanDatItem(DatItem datItem)
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
        internal string CleanGameName(string game)
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
        /// Use game descriptions as names in the DAT, updating cloneof/romof/sampleof
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        internal void MachineDescriptionToName(DatFile datFile, bool throwOnError = false)
        {
            try
            {
                // First we want to get a mapping for all games to description
                ConcurrentDictionary<string, string> mapping = new ConcurrentDictionary<string, string>();
                Parallel.ForEach(datFile.Items.Keys, Globals.ParallelOptions, key =>
                {
                    List<DatItem> items = datFile.Items[key];
                    foreach (DatItem item in items)
                    {
                        // If the key mapping doesn't exist, add it
                        mapping.TryAdd(item.Machine.Name, item.Machine.Description.Replace('/', '_').Replace("\"", "''").Replace(":", " -"));
                    }
                });

                // Now we loop through every item and update accordingly
                Parallel.ForEach(datFile.Items.Keys, Globals.ParallelOptions, key =>
                {
                    List<DatItem> items = datFile.Items[key];
                    List<DatItem> newItems = new List<DatItem>();
                    foreach (DatItem item in items)
                    {
                        // Update machine name
                        if (!string.IsNullOrWhiteSpace(item.Machine.Name) && mapping.ContainsKey(item.Machine.Name))
                            item.Machine.Name = mapping[item.Machine.Name];

                        // Update cloneof
                        if (!string.IsNullOrWhiteSpace(item.Machine.CloneOf) && mapping.ContainsKey(item.Machine.CloneOf))
                            item.Machine.CloneOf = mapping[item.Machine.CloneOf];

                        // Update romof
                        if (!string.IsNullOrWhiteSpace(item.Machine.RomOf) && mapping.ContainsKey(item.Machine.RomOf))
                            item.Machine.RomOf = mapping[item.Machine.RomOf];

                        // Update sampleof
                        if (!string.IsNullOrWhiteSpace(item.Machine.SampleOf) && mapping.ContainsKey(item.Machine.SampleOf))
                            item.Machine.SampleOf = mapping[item.Machine.SampleOf];

                        // Add the new item to the output list
                        newItems.Add(item);
                    }

                    // Replace the old list of roms with the new one
                    datFile.Items.Remove(key);
                    datFile.Items.AddRange(key, newItems);
                });
            }
            catch (Exception ex) when (!throwOnError)
            {
                logger.Warning(ex.ToString());
            }
        }

        /// <summary>
        /// Replace accented characters
        /// </summary>
        /// <param name="input">String to be parsed</param>
        /// <returns>String with characters replaced</returns>
        internal string NormalizeChars(string input)
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
        internal string RemoveUnicodeCharacters(string s)
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
        internal string RussianToLatin(string input)
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
        internal string SearchPattern(string input)
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

        /// <summary>
        /// Filter a DAT using 1G1R logic given an ordered set of regions
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        /// <remarks>
        /// In the most technical sense, the way that the region list is being used does not
        /// confine its values to be just regions. Since it's essentially acting like a
        /// specialized version of the machine name filter, anything that is usually encapsulated
        /// in parenthesis would be matched on, including disc numbers, languages, editions,
        /// and anything else commonly used. Please note that, unlike other existing 1G1R 
        /// solutions, this does not have the ability to contain custom mappings of parent
        /// to clone sets based on name, nor does it have the ability to match on the 
        /// Release DatItem type.
        /// </remarks>
        internal void SetOneGamePerRegion(DatFile datFile)
        {
            // If we have null region list, make it empty
            if (RegionList == null)
                RegionList = new List<string>();

            // For sake of ease, the first thing we want to do is bucket by game
            datFile.Items.BucketBy(ItemKey.Machine, DedupeType.None, norename: true);

            // Then we want to get a mapping of all machines to parents
            Dictionary<string, List<string>> parents = new Dictionary<string, List<string>>();
            foreach (string key in datFile.Items.Keys)
            {
                DatItem item = datFile.Items[key][0];

                // Match on CloneOf first
                if (!string.IsNullOrEmpty(item.Machine.CloneOf))
                {
                    if (!parents.ContainsKey(item.Machine.CloneOf.ToLowerInvariant()))
                        parents.Add(item.Machine.CloneOf.ToLowerInvariant(), new List<string>());

                    parents[item.Machine.CloneOf.ToLowerInvariant()].Add(item.Machine.Name.ToLowerInvariant());
                }

                // Then by RomOf
                else if (!string.IsNullOrEmpty(item.Machine.RomOf))
                {
                    if (!parents.ContainsKey(item.Machine.RomOf.ToLowerInvariant()))
                        parents.Add(item.Machine.RomOf.ToLowerInvariant(), new List<string>());

                    parents[item.Machine.RomOf.ToLowerInvariant()].Add(item.Machine.Name.ToLowerInvariant());
                }

                // Otherwise, treat it as a parent
                else
                {
                    if (!parents.ContainsKey(item.Machine.Name.ToLowerInvariant()))
                        parents.Add(item.Machine.Name.ToLowerInvariant(), new List<string>());

                    parents[item.Machine.Name.ToLowerInvariant()].Add(item.Machine.Name.ToLowerInvariant());
                }
            }

            // Once we have the full list of mappings, filter out games to keep
            foreach (string key in parents.Keys)
            {
                // Find the first machine that matches the regions in order, if possible
                string machine = default;
                foreach (string region in RegionList)
                {
                    machine = parents[key].FirstOrDefault(m => Regex.IsMatch(m, @"\(.*" + region + @".*\)", RegexOptions.IgnoreCase));
                    if (machine != default)
                        break;
                }

                // If we didn't get a match, use the parent
                if (machine == default)
                    machine = key;

                // Remove the key from the list
                parents[key].Remove(machine);

                // Remove the rest of the items from this key
                parents[key].ForEach(k => datFile.Items.Remove(k));
            }

            // Finally, strip out the parent tags
            Splitter.RemoveTagsFromChild(datFile);
        }

        /// <summary>
        /// Ensure that all roms are in their own game (or at least try to ensure)
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        internal void SetOneRomPerGame(DatFile datFile)
        {
            // Because this introduces subfolders, we need to set the SuperDAT type
            datFile.Header.Type = "SuperDAT";

            // For each rom, we want to update the game to be "<game name>/<rom name>"
            Parallel.ForEach(datFile.Items.Keys, Globals.ParallelOptions, key =>
            {
                List<DatItem> items = datFile.Items[key];
                for (int i = 0; i < items.Count; i++)
                {
                    DatItemTool.SetOneRomPerGame(items[i]);
                }
            });
        }

        /// <summary>
        /// Strip the dates from the beginning of scene-style set names
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        internal void StripSceneDatesFromItems(DatFile datFile)
        {
            // Output the logging statement
            logger.User("Stripping scene-style dates");

            // Set the regex pattern to use
            string pattern = @"([0-9]{2}\.[0-9]{2}\.[0-9]{2}-)(.*?-.*?)";

            // Now process all of the roms
            Parallel.ForEach(datFile.Items.Keys, Globals.ParallelOptions, key =>
            {
                List<DatItem> items = datFile.Items[key];
                for (int j = 0; j < items.Count; j++)
                {
                    DatItem item = items[j];
                    if (Regex.IsMatch(item.Machine.Name, pattern))
                        item.Machine.Name = Regex.Replace(item.Machine.Name, pattern, "$2");

                    if (Regex.IsMatch(item.Machine.Description, pattern))
                        item.Machine.Description = Regex.Replace(item.Machine.Description, pattern, "$2");

                    items[j] = item;
                }

                datFile.Items.Remove(key);
                datFile.Items.AddRange(key, items);
            });
        }

        #endregion

        #region Filtering

        /// <summary>
        /// Apply a set of Filters on the DatFile
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        /// <param name="perMachine">True if entire machines are considered, false otherwise (default)</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        /// <returns>True if the DatFile was filtered, false on error</returns>
        public bool ApplyFilters(DatFile datFile, bool perMachine = false, bool throwOnError = false)
        {
            // If we have null filters, return false
            if (MachineFilter == null || DatItemFilter == null)
                return false;

            // If we're filtering per machine, bucket by machine first
            if (perMachine)
                datFile.Items.BucketBy(ItemKey.Machine, DedupeType.None);

            try
            {
                // Loop over every key in the dictionary
                List<string> keys = datFile.Items.Keys.ToList();
                foreach (string key in keys)
                {
                    // For every item in the current key
                    bool machinePass = true;
                    List<DatItem> items = datFile.Items[key];
                    foreach (DatItem item in items)
                    {
                        // If we have a null item, we can't pass it
                        if (item == null)
                            continue;

                        // If the item is already filtered out, we skip
                        if (item.Remove)
                            continue;

                        // If the rom doesn't pass the filter, mark for removal
                        if (!PassesFilters(item))
                        {
                            item.Remove = true;

                            // If we're in machine mode, set and break
                            if (perMachine)
                            {
                                machinePass = false;
                                break;
                            }
                        }
                    }

                    // If we didn't pass and we're in machine mode, set all items as remove
                    if (perMachine && !machinePass)
                    {
                        foreach (DatItem item in items)
                        {
                            item.Remove = true;
                        }
                    }

                    // Assign back for caution
                    datFile.Items[key] = items;
                }
            }
            catch (Exception ex) when (!throwOnError)
            {
                logger.Error(ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check to see if a DatItem passes the filters
        /// </summary>
        /// <param name="datItem">DatItem to check</param>
        /// <returns>True if the item passed the filter, false otherwise</returns>
        internal bool PassesFilters(DatItem datItem)
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
