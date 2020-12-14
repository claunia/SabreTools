using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using SabreTools.Core;
using SabreTools.DatFiles;
using SabreTools.DatItems;
using SabreTools.Filtering;
using SabreTools.IO;
using SabreTools.Logging;

// This file represents all methods related to the Filtering namespace
namespace SabreTools.DatTools
{
    public class Modification
    {
        #region Logging

        /// <summary>
        /// Logging object
        /// </summary>
        private static readonly Logger logger = new Logger();

        #endregion

        /// <summary>
        /// Apply cleaning methods to the DatFile
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        /// <param name="cleaner">Cleaner to use</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        /// <returns>True if cleaning was successful, false on error</returns>
        public static bool ApplyCleaning(DatFile datFile, Cleaner cleaner, bool throwOnError = false)
        {
            try
            {
                // Perform item-level cleaning
                CleanDatItems(datFile, cleaner);

                // Bucket and dedupe according to the flag
                if (cleaner?.DedupeRoms == DedupeType.Full)
                    datFile.Items.BucketBy(Field.DatItem_CRC, cleaner.DedupeRoms);
                else if (cleaner?.DedupeRoms == DedupeType.Game)
                    datFile.Items.BucketBy(Field.Machine_Name, cleaner.DedupeRoms);

                // Process description to machine name
                if (cleaner?.DescriptionAsName == true)
                    MachineDescriptionToName(datFile);

                // If we are removing scene dates, do that now
                if (cleaner?.SceneDateStrip == true)
                    StripSceneDatesFromItems(datFile);

                // Run the one rom per game logic, if required
                if (cleaner?.OneGamePerRegion == true)
                    OneGamePerRegion(datFile, cleaner.RegionList);

                // Run the one rom per game logic, if required
                if (cleaner?.OneRomPerGame == true)
                    OneRomPerGame(datFile);

                // If we are removing fields, do that now
                if ((cleaner.ExcludeMachineFields != null && cleaner.ExcludeMachineFields.Any())
                    || cleaner.ExcludeDatItemFields != null && cleaner.ExcludeDatItemFields.Any())
                {
                    RemoveFieldsFromItems(datFile, cleaner.ExcludeDatItemFields, cleaner.ExcludeMachineFields);
                }

                // Remove all marked items
                datFile.Items.ClearMarked();

                // We remove any blanks, if we aren't supposed to have any
                if (cleaner?.KeepEmptyGames == false)
                    datFile.Items.ClearEmpty();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                if (throwOnError) throw ex;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Apply a set of Extra INIs on the DatFile
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        /// <param name="extras">ExtrasIni to use</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        /// <returns>True if the extras were applied, false on error</returns>
        public static bool ApplyExtras(DatFile datFile, ExtraIni extras, bool throwOnError = false)
        {
            try
            {
                // Bucket by game first
                datFile.Items.BucketBy(Field.Machine_Name, DedupeType.None);

                // Create a new set of mappings based on the items
                var machineMap = new Dictionary<string, Dictionary<MachineField, string>>();
                var datItemMap = new Dictionary<string, Dictionary<DatItemField, string>>();

                // Loop through each of the extras
                foreach (ExtraIniItem item in extras.Items)
                {
                    foreach (var mapping in item.Mappings)
                    {
                        string key = mapping.Key;
                        List<string> machineNames = mapping.Value;

                        // Loop through the machines and add the new mappings
                        foreach (string machine in machineNames)
                        {
                            if (item.MachineField != MachineField.NULL)
                            {
                                if (!machineMap.ContainsKey(machine))
                                    machineMap[machine] = new Dictionary<MachineField, string>();

                                machineMap[machine][item.MachineField] = key;
                            }
                            else if (item.DatItemField != DatItemField.NULL)
                            {
                                if (!datItemMap.ContainsKey(machine))
                                    datItemMap[machine] = new Dictionary<DatItemField, string>();

                                datItemMap[machine][item.DatItemField] = key;
                            }
                        }
                    }
                }

                // Now apply the new set of Machine mappings
                foreach (string key in machineMap.Keys)
                {
                    // If the key doesn't exist, continue
                    if (!datFile.Items.ContainsKey(key))
                        continue;

                    List<DatItem> datItems = datFile.Items[key];
                    var mappings = machineMap[key];

                    foreach (var datItem in datItems)
                    {
                        datItem.SetFields(null, mappings);
                    }
                }

                // Now apply the new set of DatItem mappings
                foreach (string key in datItemMap.Keys)
                {
                    // If the key doesn't exist, continue
                    if (!datFile.Items.ContainsKey(key))
                        continue;

                    List<DatItem> datItems = datFile.Items[key];
                    var mappings = datItemMap[key];

                    foreach (var datItem in datItems)
                    {
                        datItem.SetFields(mappings, null);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                if (throwOnError) throw ex;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Apply a set of Filters on the DatFile
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        /// <param name="cleaner">Cleaner to use</param>
        /// <param name="perMachine">True if entire machines are considered, false otherwise (default)</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        /// <returns>True if the DatFile was filtered, false on error</returns>
        public static bool ApplyFilters(DatFile datFile, Cleaner cleaner, bool perMachine = false, bool throwOnError = false)
        {
            // If we have a null cleaner or filters, return false
            if (cleaner == null || cleaner.DatHeaderFilter == null || cleaner.MachineFilter == null || cleaner.DatItemFilter == null)
                return false;

            // If we're filtering per machine, bucket by machine first
            if (perMachine)
                datFile.Items.BucketBy(Field.Machine_Name, DedupeType.None);

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
                        if (!cleaner.PassesFilters(item))
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
            catch (Exception ex)
            {
                logger.Error(ex);
                if (throwOnError) throw ex;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Apply splitting on the DatFile
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        /// <param name="splitType">Split type to try</param>
        /// <param name="useTags">True if DatFile tags override splitting, false otherwise</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        /// <returns>True if the DatFile was split, false on error</returns>
        public static bool ApplySplitting(DatFile datFile, MergingFlag splitType, bool useTags, bool throwOnError = false)
        {
            try
            {
                // If we are using tags from the DAT, set the proper input for split type unless overridden
                if (useTags && splitType == MergingFlag.None)
                    splitType = datFile.Header.ForceMerging;

                // Run internal splitting
                switch (splitType)
                {
                    case MergingFlag.None:
                        // No-op
                        break;
                    case MergingFlag.Device:
                        CreateDeviceNonMergedSets(datFile, DedupeType.None);
                        break;
                    case MergingFlag.Full:
                        CreateFullyNonMergedSets(datFile, DedupeType.None);
                        break;
                    case MergingFlag.NonMerged:
                        CreateNonMergedSets(datFile, DedupeType.None);
                        break;
                    case MergingFlag.Merged:
                        CreateMergedSets(datFile, DedupeType.None);
                        break;
                    case MergingFlag.Split:
                        CreateSplitSets(datFile, DedupeType.None);
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                if (throwOnError) throw ex;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Apply SuperDAT naming logic to a merged DatFile
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        /// <param name="inputs">List of inputs to use for renaming</param>
        public static void ApplySuperDAT(DatFile datFile, List<ParentablePath> inputs)
        {
            List<string> keys = datFile.Items.Keys.ToList();
            Parallel.ForEach(keys, Globals.ParallelOptions, key =>
            {
                List<DatItem> items = datFile.Items[key].ToList();
                List<DatItem> newItems = new List<DatItem>();
                foreach (DatItem item in items)
                {
                    DatItem newItem = item;
                    string filename = inputs[newItem.Source.Index].CurrentPath;
                    string rootpath = inputs[newItem.Source.Index].ParentPath;

                    if (!string.IsNullOrWhiteSpace(rootpath))
                        rootpath += Path.DirectorySeparatorChar.ToString();

                    filename = filename.Remove(0, rootpath.Length);
                    newItem.Machine.Name = Path.GetDirectoryName(filename) + Path.DirectorySeparatorChar
                        + Path.GetFileNameWithoutExtension(filename) + Path.DirectorySeparatorChar
                        + newItem.Machine.Name;

                    newItems.Add(newItem);
                }

                datFile.Items.Remove(key);
                datFile.Items.AddRange(key, newItems);
            });
        }

        /// <summary>
        /// Use game descriptions as names in the DAT, updating cloneof/romof/sampleof
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        public static void MachineDescriptionToName(DatFile datFile, bool throwOnError = false)
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
            catch (Exception ex)
            {
                logger.Warning(ex.ToString());
                if (throwOnError) throw ex;
            }
        }

        /// <summary>
        /// Filter a DAT using 1G1R logic given an ordered set of regions
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        /// <param name="regions">Ordered list of regions to use</param>
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
        public static void OneGamePerRegion(DatFile datFile, List<string> regions)
        {
            // If we have null region list, make it empty
            if (regions == null)
                regions = new List<string>();

            // For sake of ease, the first thing we want to do is bucket by game
            datFile.Items.BucketBy(Field.Machine_Name, DedupeType.None, norename: true);

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
                foreach (string region in regions)
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
            RemoveTagsFromChild(datFile);
        }

        /// <summary>
        /// Ensure that all roms are in their own game (or at least try to ensure)
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        public static void OneRomPerGame(DatFile datFile)
        {
            // Because this introduces subfolders, we need to set the SuperDAT type
            datFile.Header.Type = "SuperDAT";

            // For each rom, we want to update the game to be "<game name>/<rom name>"
            Parallel.ForEach(datFile.Items.Keys, Globals.ParallelOptions, key =>
            {
                List<DatItem> items = datFile.Items[key];
                for (int i = 0; i < items.Count; i++)
                {
                    items[i].SetOneRomPerGame();
                }
            });
        }

        /// <summary>
        /// Remove fields as per the header
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        /// <param name="datItemFields">DatItem fields to remove</param>
        /// <param name="machineFields">Machine fields to remove</param>
        public static void RemoveFieldsFromItems(
            DatFile datFile,
            List<DatItemField> datItemFields,
            List<MachineField> machineFields)
        {
            // If we have null field list, make it empty
            if (datItemFields == null)
                datItemFields = new List<DatItemField>();
            if (machineFields == null)
                machineFields = new List<MachineField>();

            // Output the logging statement
            logger.User("Removing filtered fields");

            // Now process all of the roms
            Parallel.ForEach(datFile.Items.Keys, Globals.ParallelOptions, key =>
            {
                List<DatItem> items = datFile.Items[key];
                for (int j = 0; j < items.Count; j++)
                {
                    items[j].RemoveFields(datItemFields, machineFields);
                }

                datFile.Items.Remove(key);
                datFile.Items.AddRange(key, items);
            });
        }

        /// <summary>
        /// Strip the dates from the beginning of scene-style set names
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        public static void StripSceneDatesFromItems(DatFile datFile)
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
    
        /// <summary>
        /// Clean individual items based on the current filter
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        /// <param name="cleaner">Cleaner to use</param>
        private static void CleanDatItems(DatFile datFile, Cleaner cleaner)
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
                    cleaner.CleanDatItem(item);
                }

                // Assign back for caution
                datFile.Items[key] = items;
            }
        }

        // TODO: Should any of these create a new DatFile in the process?
        // The reason this comes up is that doing any of the splits or merges
        // is an inherently destructive process. Making it output a new DatFile
        // might make it easier to deal with multiple internal steps. On the other
        // hand, this will increase memory usage significantly and would force the
        // existing paths to behave entirely differently
        #region Internal Splitting/Merging

        /// <summary>
        /// Use cdevice_ref tags to get full non-merged sets and remove parenting tags
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        /// <param name="mergeroms">Dedupe type to be used</param>
        private static void CreateDeviceNonMergedSets(DatFile datFile, DedupeType mergeroms)
        {
            logger.User("Creating device non-merged sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            datFile.Items.BucketBy(Field.Machine_Name, mergeroms, norename: true);

            // Now we want to loop through all of the games and set the correct information
            while (AddRomsFromDevices(datFile, false, false)) ;
            while (AddRomsFromDevices(datFile, true, false)) ;

            // Then, remove the romof and cloneof tags so it's not picked up by the manager
            RemoveTagsFromChild(datFile);
        }

        /// <summary>
        /// Use cloneof tags to create non-merged sets and remove the tags plus using the device_ref tags to get full sets
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        /// <param name="mergeroms">Dedupe type to be used</param>
        private static void CreateFullyNonMergedSets(DatFile datFile, DedupeType mergeroms)
        {
            logger.User("Creating fully non-merged sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            datFile.Items.BucketBy(Field.Machine_Name, mergeroms, norename: true);

            // Now we want to loop through all of the games and set the correct information
            while (AddRomsFromDevices(datFile, true, true)) ;
            AddRomsFromDevices(datFile, false, true);
            AddRomsFromParent(datFile);

            // Now that we have looped through the cloneof tags, we loop through the romof tags
            AddRomsFromBios(datFile);

            // Then, remove the romof and cloneof tags so it's not picked up by the manager
            RemoveTagsFromChild(datFile);
        }

        /// <summary>
        /// Use cloneof tags to create merged sets and remove the tags
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        /// <param name="mergeroms">Dedupe type to be used</param>
        private static void CreateMergedSets(DatFile datFile, DedupeType mergeroms)
        {
            logger.User("Creating merged sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            datFile.Items.BucketBy(Field.Machine_Name, mergeroms, norename: true);

            // Now we want to loop through all of the games and set the correct information
            AddRomsFromChildren(datFile);

            // Now that we have looped through the cloneof tags, we loop through the romof tags
            RemoveBiosRomsFromChild(datFile, false);
            RemoveBiosRomsFromChild(datFile, true);

            // Finally, remove the romof and cloneof tags so it's not picked up by the manager
            RemoveTagsFromChild(datFile);
        }

        /// <summary>
        /// Use cloneof tags to create non-merged sets and remove the tags
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        /// <param name="mergeroms">Dedupe type to be used</param>
        private static void CreateNonMergedSets(DatFile datFile, DedupeType mergeroms)
        {
            logger.User("Creating non-merged sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            datFile.Items.BucketBy(Field.Machine_Name, mergeroms, norename: true);

            // Now we want to loop through all of the games and set the correct information
            AddRomsFromParent(datFile);

            // Now that we have looped through the cloneof tags, we loop through the romof tags
            RemoveBiosRomsFromChild(datFile, false);
            RemoveBiosRomsFromChild(datFile, true);

            // Finally, remove the romof and cloneof tags so it's not picked up by the manager
            RemoveTagsFromChild(datFile);
        }

        /// <summary>
        /// Use cloneof and romof tags to create split sets and remove the tags
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        /// <param name="mergeroms">Dedupe type to be used</param>
        private static void CreateSplitSets(DatFile datFile, DedupeType mergeroms)
        {
            logger.User("Creating split sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            datFile.Items.BucketBy(Field.Machine_Name, mergeroms, norename: true);

            // Now we want to loop through all of the games and set the correct information
            RemoveRomsFromChild(datFile);

            // Now that we have looped through the cloneof tags, we loop through the romof tags
            RemoveBiosRomsFromChild(datFile, false);
            RemoveBiosRomsFromChild(datFile, true);

            // Finally, remove the romof and cloneof tags so it's not picked up by the manager
            RemoveTagsFromChild(datFile);
        }

        /// <summary>
        /// Use romof tags to add roms to the children
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        private static void AddRomsFromBios(DatFile datFile)
        {
            List<string> games = datFile.Items.Keys.OrderBy(g => g).ToList();
            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                if (datFile.Items[game].Count == 0)
                    continue;

                // Determine if the game has a parent or not
                string parent = null;
                if (!string.IsNullOrWhiteSpace(datFile.Items[game][0].Machine.RomOf))
                    parent = datFile.Items[game][0].Machine.RomOf;

                // If the parent doesnt exist, we want to continue
                if (string.IsNullOrWhiteSpace(parent))
                    continue;

                // If the parent doesn't have any items, we want to continue
                if (datFile.Items[parent].Count == 0)
                    continue;

                // If the parent exists and has items, we copy the items from the parent to the current game
                DatItem copyFrom = datFile.Items[game][0];
                List<DatItem> parentItems = datFile.Items[parent];
                foreach (DatItem item in parentItems)
                {
                    DatItem datItem = (DatItem)item.Clone();
                    datItem.CopyMachineInformation(copyFrom);
                    if (datFile.Items[game].Where(i => i.GetName() == datItem.GetName()).Count() == 0 && !datFile.Items[game].Contains(datItem))
                        datFile.Items.Add(game, datItem);
                }
            }
        }

        /// <summary>
        /// Use device_ref and optionally slotoption tags to add roms to the children
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        /// <param name="dev">True if only child device sets are touched, false for non-device sets (default)</param>
        /// <param name="useSlotOptions">True if slotoptions tags are used as well, false otherwise</param>
        private static bool AddRomsFromDevices(DatFile datFile, bool dev = false, bool useSlotOptions = false)
        {
            bool foundnew = false;
            List<string> machines = datFile.Items.Keys.OrderBy(g => g).ToList();
            foreach (string machine in machines)
            {
                // If the machine doesn't have items, we continue
                if (datFile.Items[machine] == null || datFile.Items[machine].Count == 0)
                    continue;

                // If the machine (is/is not) a device, we want to continue
                if (dev ^ (datFile.Items[machine][0].Machine.MachineType.HasFlag(MachineType.Device)))
                    continue;

                // Get all device reference names from the current machine
                List<string> deviceReferences = datFile.Items[machine]
                    .Where(i => i.ItemType == ItemType.DeviceReference)
                    .Select(i => i as DeviceReference)
                    .Select(dr => dr.Name)
                    .Distinct()
                    .ToList();

                // Get all slot option names from the current machine
                List<string> slotOptions = datFile.Items[machine]
                    .Where(i => i.ItemType == ItemType.Slot)
                    .Select(i => i as Slot)
                    .Where(s => s.SlotOptionsSpecified)
                    .SelectMany(s => s.SlotOptions)
                    .Select(so => so.DeviceName)
                    .Distinct()
                    .ToList();

                // If we're checking device references
                if (deviceReferences.Any())
                {
                    // Loop through all names and check the corresponding machines
                    List<string> newDeviceReferences = new List<string>();
                    foreach (string deviceReference in deviceReferences)
                    {
                        // If the machine doesn't exist then we continue
                        if (datFile.Items[deviceReference] == null || datFile.Items[deviceReference].Count == 0)
                            continue;

                        // Add to the list of new device reference names
                        List<DatItem> devItems = datFile.Items[deviceReference];
                        newDeviceReferences.AddRange(devItems
                            .Where(i => i.ItemType == ItemType.DeviceReference)
                            .Select(i => (i as DeviceReference).Name));

                        // Set new machine information and add to the current machine
                        DatItem copyFrom = datFile.Items[machine][0];
                        foreach (DatItem item in devItems)
                        {
                            // If the parent machine doesn't already contain this item, add it
                            if (!datFile.Items[machine].Any(i => i.ItemType == item.ItemType && i.GetName() == item.GetName()))
                            {
                                // Set that we found new items
                                foundnew = true;

                                // Clone the item and then add it
                                DatItem datItem = (DatItem)item.Clone();
                                datItem.CopyMachineInformation(copyFrom);
                                datFile.Items.Add(machine, datItem);
                            }
                        }
                    }

                    // Now that every device reference is accounted for, add the new list of device references, if they don't already exist
                    foreach (string deviceReference in newDeviceReferences.Distinct())
                    {
                        if (!deviceReferences.Contains(deviceReference))
                            datFile.Items[machine].Add(new DeviceReference() { Name = deviceReference });
                    }
                }

                // If we're checking slotoptions
                if (useSlotOptions && slotOptions.Any())
                {
                    // Loop through all names and check the corresponding machines
                    List<string> newSlotOptions = new List<string>();
                    foreach (string slotOption in slotOptions)
                    {
                        // If the machine doesn't exist then we continue
                        if (datFile.Items[slotOption] == null || datFile.Items[slotOption].Count == 0)
                            continue;

                        // Add to the list of new slot option names
                        List<DatItem> slotItems = datFile.Items[slotOption];
                        newSlotOptions.AddRange(slotItems
                            .Where(i => i.ItemType == ItemType.Slot)
                            .Where(s => (s as Slot).SlotOptionsSpecified)
                            .SelectMany(s => (s as Slot).SlotOptions)
                            .Select(o => o.DeviceName));

                        // Set new machine information and add to the current machine
                        DatItem copyFrom = datFile.Items[machine][0];
                        foreach (DatItem item in slotItems)
                        {
                            // If the parent machine doesn't already contain this item, add it
                            if (!datFile.Items[machine].Any(i => i.ItemType == item.ItemType && i.GetName() == item.GetName()))
                            {
                                // Set that we found new items
                                foundnew = true;

                                // Clone the item and then add it
                                DatItem datItem = (DatItem)item.Clone();
                                datItem.CopyMachineInformation(copyFrom);
                                datFile.Items.Add(machine, datItem);
                            }
                        }
                    }

                    // Now that every device is accounted for, add the new list of slot options, if they don't already exist
                    foreach (string slotOption in newSlotOptions.Distinct())
                    {
                        if (!slotOptions.Contains(slotOption))
                            datFile.Items[machine].Add(new Slot() { SlotOptions = new List<SlotOption> { new SlotOption { DeviceName = slotOption } } });
                    }
                }
            }

            return foundnew;
        }

        /// <summary>
        /// Use cloneof tags to add roms to the children, setting the new romof tag in the process
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        private static void AddRomsFromParent(DatFile datFile)
        {
            List<string> games = datFile.Items.Keys.OrderBy(g => g).ToList();
            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                if (datFile.Items[game].Count == 0)
                    continue;

                // Determine if the game has a parent or not
                string parent = null;
                if (!string.IsNullOrWhiteSpace(datFile.Items[game][0].Machine.CloneOf))
                    parent = datFile.Items[game][0].Machine.CloneOf;

                // If the parent doesnt exist, we want to continue
                if (string.IsNullOrWhiteSpace(parent))
                    continue;

                // If the parent doesn't have any items, we want to continue
                if (datFile.Items[parent].Count == 0)
                    continue;

                // If the parent exists and has items, we copy the items from the parent to the current game
                DatItem copyFrom = datFile.Items[game][0];
                List<DatItem> parentItems = datFile.Items[parent];
                foreach (DatItem item in parentItems)
                {
                    DatItem datItem = (DatItem)item.Clone();
                    datItem.CopyMachineInformation(copyFrom);
                    if (datFile.Items[game].Where(i => i.GetName()?.ToLowerInvariant() == datItem.GetName()?.ToLowerInvariant()).Count() == 0
                        && !datFile.Items[game].Contains(datItem))
                    {
                        datFile.Items.Add(game, datItem);
                    }
                }

                // Now we want to get the parent romof tag and put it in each of the items
                List<DatItem> items = datFile.Items[game];
                string romof = datFile.Items[parent][0].Machine.RomOf;
                foreach (DatItem item in items)
                {
                    item.Machine.RomOf = romof;
                }
            }
        }

        /// <summary>
        /// Use cloneof tags to add roms to the parents, removing the child sets in the process
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        /// <param name="subfolder">True to add DatItems to subfolder of parent (not including Disk), false otherwise</param>
        private static void AddRomsFromChildren(DatFile datFile, bool subfolder = true)
        {
            List<string> games = datFile.Items.Keys.OrderBy(g => g).ToList();
            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                if (datFile.Items[game].Count == 0)
                    continue;

                // Determine if the game has a parent or not
                string parent = null;
                if (!string.IsNullOrWhiteSpace(datFile.Items[game][0].Machine.CloneOf))
                    parent = datFile.Items[game][0].Machine.CloneOf;

                // If there is no parent, then we continue
                if (string.IsNullOrWhiteSpace(parent))
                    continue;

                // Otherwise, move the items from the current game to a subfolder of the parent game
                DatItem copyFrom;
                if (datFile.Items[parent].Count == 0)
                {
                    copyFrom = new Rom();
                    copyFrom.Machine.Name = parent;
                    copyFrom.Machine.Description = parent;
                }
                else
                {
                    copyFrom = datFile.Items[parent][0];
                }

                List<DatItem> items = datFile.Items[game];
                foreach (DatItem item in items)
                {
                    // Special disk handling
                    if (item.ItemType == ItemType.Disk)
                    {
                        Disk disk = item as Disk;

                        // If the merge tag exists and the parent already contains it, skip
                        if (disk.MergeTag != null && datFile.Items[parent].Where(i => i.ItemType == ItemType.Disk).Select(i => (i as Disk).Name).Contains(disk.MergeTag))
                        {
                            continue;
                        }

                        // If the merge tag exists but the parent doesn't contain it, add to parent
                        else if (disk.MergeTag != null && !datFile.Items[parent].Where(i => i.ItemType == ItemType.Disk).Select(i => (i as Disk).Name).Contains(disk.MergeTag))
                        {
                            disk.CopyMachineInformation(copyFrom);
                            datFile.Items.Add(parent, disk);
                        }

                        // If there is no merge tag, add to parent
                        else if (disk.MergeTag == null)
                        {
                            disk.CopyMachineInformation(copyFrom);
                            datFile.Items.Add(parent, disk);
                        }
                    }

                    // Special rom handling
                    else if (item.ItemType == ItemType.Rom)
                    {
                        Rom rom = item as Rom;

                        // If the merge tag exists and the parent already contains it, skip
                        if (rom.MergeTag != null && datFile.Items[parent].Where(i => i.ItemType == ItemType.Rom).Select(i => (i as Rom).Name).Contains(rom.MergeTag))
                        {
                            continue;
                        }

                        // If the merge tag exists but the parent doesn't contain it, add to subfolder of parent
                        else if (rom.MergeTag != null && !datFile.Items[parent].Where(i => i.ItemType == ItemType.Rom).Select(i => (i as Rom).Name).Contains(rom.MergeTag))
                        {
                            if (subfolder)
                                rom.Name = $"{rom.Machine.Name}\\{rom.Name}";

                            rom.CopyMachineInformation(copyFrom);
                            datFile.Items.Add(parent, rom);
                        }

                        // If the parent doesn't already contain this item, add to subfolder of parent
                        else if (!datFile.Items[parent].Contains(item))
                        {
                            if (subfolder)
                                rom.Name = $"{item.Machine.Name}\\{rom.Name}";

                            rom.CopyMachineInformation(copyFrom);
                            datFile.Items.Add(parent, rom);
                        }
                    }

                    // All other that would be missing to subfolder of parent
                    else if (!datFile.Items[parent].Contains(item))
                    {
                        if (subfolder)
                            item.SetFields(new Dictionary<DatItemField, string> { [DatItemField.Name] = $"{item.Machine.Name}\\{item.GetName()}" }, null);

                        item.CopyMachineInformation(copyFrom);
                        datFile.Items.Add(parent, item);
                    }
                }

                // Then, remove the old game so it's not picked up by the writer
                datFile.Items.Remove(game);
            }
        }

        /// <summary>
        /// Remove all BIOS and device sets
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        private static void RemoveBiosAndDeviceSets(DatFile datFile)
        {
            List<string> games = datFile.Items.Keys.OrderBy(g => g).ToList();
            foreach (string game in games)
            {
                if (datFile.Items[game].Count > 0
                    && (datFile.Items[game][0].Machine.MachineType.HasFlag(MachineType.Bios)
                        || datFile.Items[game][0].Machine.MachineType.HasFlag(MachineType.Device)))
                {
                    datFile.Items.Remove(game);
                }
            }
        }

        /// <summary>
        /// Use romof tags to remove bios roms from children
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        /// <param name="bios">True if only child Bios sets are touched, false for non-bios sets (default)</param>
        private static void RemoveBiosRomsFromChild(DatFile datFile, bool bios = false)
        {
            // Loop through the romof tags
            List<string> games = datFile.Items.Keys.OrderBy(g => g).ToList();
            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                if (datFile.Items[game].Count == 0)
                    continue;

                // If the game (is/is not) a bios, we want to continue
                if (bios ^ datFile.Items[game][0].Machine.MachineType.HasFlag(MachineType.Bios))
                    continue;

                // Determine if the game has a parent or not
                string parent = null;
                if (!string.IsNullOrWhiteSpace(datFile.Items[game][0].Machine.RomOf))
                    parent = datFile.Items[game][0].Machine.RomOf;

                // If the parent doesnt exist, we want to continue
                if (string.IsNullOrWhiteSpace(parent))
                    continue;

                // If the parent doesn't have any items, we want to continue
                if (datFile.Items[parent].Count == 0)
                    continue;

                // If the parent exists and has items, we remove the items that are in the parent from the current game
                List<DatItem> parentItems = datFile.Items[parent];
                foreach (DatItem item in parentItems)
                {
                    DatItem datItem = (DatItem)item.Clone();
                    while (datFile.Items[game].Contains(datItem))
                    {
                        datFile.Items.Remove(game, datItem);
                    }
                }
            }
        }

        /// <summary>
        /// Use cloneof tags to remove roms from the children
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        private static void RemoveRomsFromChild(DatFile datFile)
        {
            List<string> games = datFile.Items.Keys.OrderBy(g => g).ToList();
            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                if (datFile.Items[game].Count == 0)
                    continue;

                // Determine if the game has a parent or not
                string parent = null;
                if (!string.IsNullOrWhiteSpace(datFile.Items[game][0].Machine.CloneOf))
                    parent = datFile.Items[game][0].Machine.CloneOf;

                // If the parent doesnt exist, we want to continue
                if (string.IsNullOrWhiteSpace(parent))
                    continue;

                // If the parent doesn't have any items, we want to continue
                if (datFile.Items[parent].Count == 0)
                    continue;

                // If the parent exists and has items, we remove the parent items from the current game
                List<DatItem> parentItems = datFile.Items[parent];
                foreach (DatItem item in parentItems)
                {
                    DatItem datItem = (DatItem)item.Clone();
                    while (datFile.Items[game].Contains(datItem))
                    {
                        datFile.Items.Remove(game, datItem);
                    }
                }

                // Now we want to get the parent romof tag and put it in each of the remaining items
                List<DatItem> items = datFile.Items[game];
                string romof = datFile.Items[parent][0].Machine.RomOf;
                foreach (DatItem item in items)
                {
                    item.Machine.RomOf = romof;
                }
            }
        }

        /// <summary>
        /// Remove all romof and cloneof tags from all games
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        private static void RemoveTagsFromChild(DatFile datFile)
        {
            List<string> games = datFile.Items.Keys.OrderBy(g => g).ToList();
            foreach (string game in games)
            {
                List<DatItem> items = datFile.Items[game];
                foreach (DatItem item in items)
                {
                    item.Machine.CloneOf = null;
                    item.Machine.RomOf = null;
                    item.Machine.SampleOf = null;
                }
            }
        }

        #endregion
    }
}