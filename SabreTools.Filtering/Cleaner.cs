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
        #region Fields

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
        public List<string>? RegionList { get; set; }

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
        public string? Root { get; set; }

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
        private readonly Logger logger = new();

        #endregion

        #region Running

        /// <summary>
        /// Apply cleaning methods to the DatFile
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        /// <returns>True if cleaning was successful, false on error</returns>
        public bool ApplyCleaning(DatFile datFile, bool throwOnError = false)
        {
            InternalStopwatch watch = new("Applying cleaning steps to DAT");

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
            finally
            {
                watch.Stop();
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
                var items = datFile.Items[key];
                if (items == null)
                    continue;

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
                datItem.Machine.Name = TextHelper.RemoveUnicodeCharacters(datItem.Machine.Name);
                datItem.Machine.Description = TextHelper.RemoveUnicodeCharacters(datItem.Machine.Description);
                datItem.SetName(TextHelper.RemoveUnicodeCharacters(datItem.GetName()));
            }

            // If we're in cleaning mode, sanitize machine name and description
            if (Clean)
            {
                datItem.Machine.Name = TextHelper.NormalizeCharacters(datItem.Machine.Name);
                datItem.Machine.Description = TextHelper.NormalizeCharacters(datItem.Machine.Description);
            }

            // If we are in single game mode, rename the machine
            if (Single)
                datItem.Machine.Name = "!";

            // If we are in NTFS trim mode, trim the item name
            if (Trim && datItem.GetName() != null)
            {
                // Windows max name length is 260
                int usableLength = 260 - datItem.Machine.Name!.Length - (Root?.Length ?? 0);
                if (datItem.GetName()!.Length > usableLength)
                {
                    string ext = Path.GetExtension(datItem.GetName()!);
                    datItem.SetName(datItem.GetName()![..(usableLength - ext.Length)] + ext);
                }
            }
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
                ConcurrentDictionary<string, string> concurrentDictionary = new();
                ConcurrentDictionary<string, string> mapping = concurrentDictionary;
#if NET452_OR_GREATER || NETCOREAPP
                Parallel.ForEach(datFile.Items.Keys, Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
                Parallel.ForEach(datFile.Items.Keys, key =>
#else
                foreach (var key in datFile.Items.Keys)
#endif
                {
                    var items = datFile.Items[key];
                    if (items == null)
                        return;

                    foreach (DatItem item in items)
                    {
                        // If the key mapping doesn't exist, add it
                        mapping.TryAdd(item.Machine.Name!, item.Machine.Description!.Replace('/', '_').Replace("\"", "''").Replace(":", " -"));
                    }
#if NET40_OR_GREATER || NETCOREAPP
                });
#else
                }
#endif

                // Now we loop through every item and update accordingly
#if NET452_OR_GREATER || NETCOREAPP
                Parallel.ForEach(datFile.Items.Keys, Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
                Parallel.ForEach(datFile.Items.Keys, key =>
#else
                foreach (var key in datFile.Items.Keys)
#endif
                {
                    var items = datFile.Items[key];
                    if (items == null)
                        return;

                    ConcurrentList<DatItem> newItems = [];
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
#if NET40_OR_GREATER || NETCOREAPP
                });
#else
                }
#endif
            }
            catch (Exception ex) when (!throwOnError)
            {
                logger.Warning(ex.ToString());
            }
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
            RegionList ??= [];

            // For sake of ease, the first thing we want to do is bucket by game
            datFile.Items.BucketBy(ItemKey.Machine, DedupeType.None, norename: true);

            // Then we want to get a mapping of all machines to parents
            Dictionary<string, List<string>> parents = [];
            foreach (string key in datFile.Items.Keys)
            {
                DatItem item = datFile.Items[key]![0];

                // Match on CloneOf first
                if (!string.IsNullOrEmpty(item.Machine.CloneOf))
                {
                    if (!parents.ContainsKey(item.Machine.CloneOf.ToLowerInvariant()))
                        parents.Add(item.Machine.CloneOf.ToLowerInvariant(), new List<string>());

                    parents[item.Machine.CloneOf.ToLowerInvariant()].Add(item.Machine.Name!.ToLowerInvariant());
                }

                // Then by RomOf
                else if (!string.IsNullOrEmpty(item.Machine.RomOf))
                {
                    if (!parents.ContainsKey(item.Machine.RomOf.ToLowerInvariant()))
                        parents.Add(item.Machine.RomOf.ToLowerInvariant(), new List<string>());

                    parents[item.Machine.RomOf.ToLowerInvariant()].Add(item.Machine.Name!.ToLowerInvariant());
                }

                // Otherwise, treat it as a parent
                else
                {
                    if (!parents.ContainsKey(item.Machine.Name!.ToLowerInvariant()))
                        parents.Add(item.Machine.Name.ToLowerInvariant(), new List<string>());

                    parents[item.Machine.Name.ToLowerInvariant()].Add(item.Machine.Name.ToLowerInvariant());
                }
            }

            // Once we have the full list of mappings, filter out games to keep
            foreach (string key in parents.Keys)
            {
                // Find the first machine that matches the regions in order, if possible
                string? machine = default;
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
        internal static void SetOneRomPerGame(DatFile datFile)
        {
            // Because this introduces subfolders, we need to set the SuperDAT type
            datFile.Header.Type = "SuperDAT";

            // For each rom, we want to update the game to be "<game name>/<rom name>"
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(datFile.Items.Keys, Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(datFile.Items.Keys, key =>
#else
            foreach (var key in datFile.Items.Keys)
#endif
            {
                var items = datFile.Items[key];
                if (items == null)
                    return;

                for (int i = 0; i < items.Count; i++)
                {
                    SetOneRomPerGame(items[i]);
                }
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif
        }

        /// <summary>
        /// Set internal names to match One Rom Per Game (ORPG) logic
        /// </summary>
        /// <param name="datItem">DatItem to run logic on</param>
        internal static void SetOneRomPerGame(DatItem datItem)
        {
            if (datItem.GetName() == null)
                return;

            string[] splitname = datItem.GetName()!.Split('.');
            datItem.Machine.Name += $"/{string.Join(".", splitname.Take(splitname.Length > 1 ? splitname.Length - 1 : 1))}";
            datItem.SetName(Path.GetFileName(datItem.GetName()));
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
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(datFile.Items.Keys, Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(datFile.Items.Keys, key =>
#else
            foreach (var key in datFile.Items.Keys)
#endif
            {
                var items = datFile.Items[key];
                if (items == null)
                    return;

                for (int j = 0; j < items.Count; j++)
                {
                    DatItem item = items[j];
                    if (Regex.IsMatch(item.Machine.Name!, pattern))
                        item.Machine.Name = Regex.Replace(item.Machine.Name!, pattern, "$2");

                    if (Regex.IsMatch(item.Machine.Description!, pattern))
                        item.Machine.Description = Regex.Replace(item.Machine.Description!, pattern, "$2");

                    items[j] = item;
                }

                datFile.Items.Remove(key);
                datFile.Items.AddRange(key, items);
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif
        }

        #endregion
    }
}
