using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using SabreTools.Core;
using SabreTools.DatItems;
using SabreTools.Filtering;
using SabreTools.IO;

// This file represents all methods related to the Filtering namespace
namespace SabreTools.DatFiles
{
    public abstract partial class DatFile
    {
        /// <summary>
        /// Apply cleaning methods to the DatFile
        /// </summary>
        /// <param name="cleaner">Cleaner to use</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        /// <returns>True if cleaning was successful, false on error</returns>
        public bool ApplyCleaning(Cleaner cleaner, bool throwOnError = false)
        {
            try
            {
                // Perform item-level cleaning
                CleanDatItems(cleaner);

                // Bucket and dedupe according to the flag
                if (cleaner?.DedupeRoms == DedupeType.Full)
                    Items.BucketBy(Field.DatItem_CRC, cleaner.DedupeRoms);
                else if (cleaner?.DedupeRoms == DedupeType.Game)
                    Items.BucketBy(Field.Machine_Name, cleaner.DedupeRoms);

                // Process description to machine name
                if (cleaner?.DescriptionAsName == true)
                    MachineDescriptionToName();

                // If we are removing scene dates, do that now
                if (cleaner?.SceneDateStrip == true)
                    StripSceneDatesFromItems();

                // Run the one rom per game logic, if required
                if (cleaner?.OneGamePerRegion == true)
                    OneGamePerRegion(cleaner.RegionList);

                // Run the one rom per game logic, if required
                if (cleaner?.OneRomPerGame == true)
                    OneRomPerGame();

                // If we are removing fields, do that now
                if (cleaner.ExcludeFields != null && cleaner.ExcludeFields.Any())
                    RemoveFieldsFromItems(cleaner.ExcludeFields);

                // Remove all marked items
                Items.ClearMarked();

                // We remove any blanks, if we aren't supposed to have any
                if (cleaner?.KeepEmptyGames == false)
                    Items.ClearEmpty();
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
        /// <param name="extras">ExtrasIni to use</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        /// <returns>True if the extras were applied, false on error</returns>
        public bool ApplyExtras(ExtraIni extras, bool throwOnError = false)
        {
            try
            {
                // Bucket by game first
                Items.BucketBy(Field.Machine_Name, DedupeType.None);

                // Create a new set of mappings based on the items
                var map = new Dictionary<string, Dictionary<Field, string>>();

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
                            if (!map.ContainsKey(machine))
                                map[machine] = new Dictionary<Field, string>();

                            map[machine][item.Field] = key;
                        }
                    }
                }

                // Now apply the new set of mappings
                foreach (string key in map.Keys)
                {
                    // If the key doesn't exist, continue
                    if (!Items.ContainsKey(key))
                        continue;

                    List<DatItem> datItems = Items[key];
                    var mappings = map[key];

                    foreach (var datItem in datItems)
                    {
                        datItem.SetFields(mappings);
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
        /// Apply a Filter on the DatFile
        /// </summary>
        /// <param name="filter">Filter to use</param>
        /// <param name="perMachine">True if entire machines are considered, false otherwise (default)</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        /// <returns>True if the DatFile was filtered, false on error</returns>
        public bool ApplyFilter(Filter filter, bool perMachine = false, bool throwOnError = false)
        {
            // If we have a null filter, return false
            if (filter == null)
                return false;

            // If we're filtering per machine, bucket by machine first
            if (perMachine)
                Items.BucketBy(Field.Machine_Name, DedupeType.None);

            try
            {
                // Loop over every key in the dictionary
                List<string> keys = Items.Keys.ToList();
                foreach (string key in keys)
                {
                    // For every item in the current key
                    bool machinePass = true;
                    List<DatItem> items = Items[key];
                    foreach (DatItem item in items)
                    {
                        // If we have a null item, we can't pass it
                        if (item == null)
                            continue;

                        // If the item is already filtered out, we skip
                        if (item.Remove)
                            continue;

                        // If the rom doesn't pass the filter, mark for removal
                        if (!item.PassesFilter(filter))
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
                    Items[key] = items;
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
        /// <param name="splitType">Split type to try</param>
        /// <param name="useTags">True if DatFile tags override splitting, false otherwise</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        /// <returns>True if the DatFile was split, false on error</returns>
        public bool ApplySplitting(MergingFlag splitType, bool useTags, bool throwOnError = false)
        {
            try
            {
                // If we are using tags from the DAT, set the proper input for split type unless overridden
                if (useTags && splitType == MergingFlag.None)
                    splitType = Header.ForceMerging;

                // Run internal splitting
                switch (splitType)
                {
                    case MergingFlag.None:
                        // No-op
                        break;
                    case MergingFlag.Device:
                        CreateDeviceNonMergedSets(DedupeType.None);
                        break;
                    case MergingFlag.Full:
                        CreateFullyNonMergedSets(DedupeType.None);
                        break;
                    case MergingFlag.NonMerged:
                        CreateNonMergedSets(DedupeType.None);
                        break;
                    case MergingFlag.Merged:
                        CreateMergedSets(DedupeType.None);
                        break;
                    case MergingFlag.Split:
                        CreateSplitSets(DedupeType.None);
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
        /// <param name="inputs">List of inputs to use for renaming</param>
        public void ApplySuperDAT(List<ParentablePath> inputs)
        {
            List<string> keys = Items.Keys.ToList();
            Parallel.ForEach(keys, Globals.ParallelOptions, key =>
            {
                List<DatItem> items = Items[key].ToList();
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

                Items.Remove(key);
                Items.AddRange(key, newItems);
            });
        }

        /// <summary>
        /// Clean individual items based on the current filter
        /// </summary>
        /// <param name="cleaner">Cleaner to use</param>
        public void CleanDatItems(Cleaner cleaner)
        {
            List<string> keys = Items.Keys.ToList();
            foreach (string key in keys)
            {
                // For every item in the current key
                List<DatItem> items = Items[key];
                foreach (DatItem item in items)
                {
                    // If we have a null item, we can't clean it it
                    if (item == null)
                        continue;

                    // Run cleaning per item
                    item.Clean(cleaner);
                }

                // Assign back for caution
                Items[key] = items;
            }
        }

        /// <summary>
        /// Use game descriptions as names in the DAT, updating cloneof/romof/sampleof
        /// </summary>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        public void MachineDescriptionToName(bool throwOnError = false)
        {
            try
            {
                // First we want to get a mapping for all games to description
                ConcurrentDictionary<string, string> mapping = new ConcurrentDictionary<string, string>();
                Parallel.ForEach(Items.Keys, Globals.ParallelOptions, key =>
                {
                    List<DatItem> items = Items[key];
                    foreach (DatItem item in items)
                    {
                        // If the key mapping doesn't exist, add it
                        mapping.TryAdd(item.Machine.Name, item.Machine.Description.Replace('/', '_').Replace("\"", "''").Replace(":", " -"));
                    }
                });

                // Now we loop through every item and update accordingly
                Parallel.ForEach(Items.Keys, Globals.ParallelOptions, key =>
                {
                    List<DatItem> items = Items[key];
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
                    Items.Remove(key);
                    Items.AddRange(key, newItems);
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
        public void OneGamePerRegion(List<string> regions)
        {
            // If we have null region list, make it empty
            if (regions == null)
                regions = new List<string>();

            // For sake of ease, the first thing we want to do is bucket by game
            Items.BucketBy(Field.Machine_Name, DedupeType.None, norename: true);

            // Then we want to get a mapping of all machines to parents
            Dictionary<string, List<string>> parents = new Dictionary<string, List<string>>();
            foreach (string key in Items.Keys)
            {
                DatItem item = Items[key][0];

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
                parents[key].ForEach(k => Items.Remove(k));
            }

            // Finally, strip out the parent tags
            RemoveTagsFromChild();
        }

        /// <summary>
        /// Ensure that all roms are in their own game (or at least try to ensure)
        /// </summary>
        public void OneRomPerGame()
        {
            // Because this introduces subfolders, we need to set the SuperDAT type
            Header.Type = "SuperDAT";

            // For each rom, we want to update the game to be "<game name>/<rom name>"
            Parallel.ForEach(Items.Keys, Globals.ParallelOptions, key =>
            {
                List<DatItem> items = Items[key];
                for (int i = 0; i < items.Count; i++)
                {
                    items[i].SetOneRomPerGame();
                }
            });
        }

        /// <summary>
        /// Remove fields as per the header
        /// </summary>
        /// <param name="fields">List of fields to use</param>
        public void RemoveFieldsFromItems(List<Field> fields)
        {
            // If we have null field list, make it empty
            if (fields == null)
                fields = new List<Field>();

            // Output the logging statement
            logger.User("Removing filtered fields");

            // Now process all of the roms
            Parallel.ForEach(Items.Keys, Globals.ParallelOptions, key =>
            {
                List<DatItem> items = Items[key];
                for (int j = 0; j < items.Count; j++)
                {
                    items[j].RemoveFields(fields);
                }

                Items.Remove(key);
                Items.AddRange(key, items);
            });
        }

        /// <summary>
        /// Strip the dates from the beginning of scene-style set names
        /// </summary>
        public void StripSceneDatesFromItems()
        {
            // Output the logging statement
            logger.User("Stripping scene-style dates");

            // Set the regex pattern to use
            string pattern = @"([0-9]{2}\.[0-9]{2}\.[0-9]{2}-)(.*?-.*?)";

            // Now process all of the roms
            Parallel.ForEach(Items.Keys, Globals.ParallelOptions, key =>
            {
                List<DatItem> items = Items[key];
                for (int j = 0; j < items.Count; j++)
                {
                    DatItem item = items[j];
                    if (Regex.IsMatch(item.Machine.Name, pattern))
                        item.Machine.Name = Regex.Replace(item.Machine.Name, pattern, "$2");

                    if (Regex.IsMatch(item.Machine.Description, pattern))
                        item.Machine.Description = Regex.Replace(item.Machine.Description, pattern, "$2");

                    items[j] = item;
                }

                Items.Remove(key);
                Items.AddRange(key, items);
            });
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
        /// <param name="mergeroms">Dedupe type to be used</param>
        private void CreateDeviceNonMergedSets(DedupeType mergeroms)
        {
            logger.User("Creating device non-merged sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            Items.BucketBy(Field.Machine_Name, mergeroms, norename: true);

            // Now we want to loop through all of the games and set the correct information
            while (AddRomsFromDevices(false, false)) ;
            while (AddRomsFromDevices(true, false)) ;

            // Then, remove the romof and cloneof tags so it's not picked up by the manager
            RemoveTagsFromChild();
        }

        /// <summary>
        /// Use cloneof tags to create non-merged sets and remove the tags plus using the device_ref tags to get full sets
        /// </summary>
        /// <param name="mergeroms">Dedupe type to be used</param>
        private void CreateFullyNonMergedSets(DedupeType mergeroms)
        {
            logger.User("Creating fully non-merged sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            Items.BucketBy(Field.Machine_Name, mergeroms, norename: true);

            // Now we want to loop through all of the games and set the correct information
            while (AddRomsFromDevices(true, true)) ;
            AddRomsFromDevices(false, true);
            AddRomsFromParent();

            // Now that we have looped through the cloneof tags, we loop through the romof tags
            AddRomsFromBios();

            // Then, remove the romof and cloneof tags so it's not picked up by the manager
            RemoveTagsFromChild();
        }

        /// <summary>
        /// Use cloneof tags to create merged sets and remove the tags
        /// </summary>
        /// <param name="mergeroms">Dedupe type to be used</param>
        private void CreateMergedSets(DedupeType mergeroms)
        {
            logger.User("Creating merged sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            Items.BucketBy(Field.Machine_Name, mergeroms, norename: true);

            // Now we want to loop through all of the games and set the correct information
            AddRomsFromChildren();

            // Now that we have looped through the cloneof tags, we loop through the romof tags
            RemoveBiosRomsFromChild(false);
            RemoveBiosRomsFromChild(true);

            // Finally, remove the romof and cloneof tags so it's not picked up by the manager
            RemoveTagsFromChild();
        }

        /// <summary>
        /// Use cloneof tags to create non-merged sets and remove the tags
        /// </summary>
        /// <param name="mergeroms">Dedupe type to be used</param>
        private void CreateNonMergedSets(DedupeType mergeroms)
        {
            logger.User("Creating non-merged sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            Items.BucketBy(Field.Machine_Name, mergeroms, norename: true);

            // Now we want to loop through all of the games and set the correct information
            AddRomsFromParent();

            // Now that we have looped through the cloneof tags, we loop through the romof tags
            RemoveBiosRomsFromChild(false);
            RemoveBiosRomsFromChild(true);

            // Finally, remove the romof and cloneof tags so it's not picked up by the manager
            RemoveTagsFromChild();
        }

        /// <summary>
        /// Use cloneof and romof tags to create split sets and remove the tags
        /// </summary>
        /// <param name="mergeroms">Dedupe type to be used</param>
        private void CreateSplitSets(DedupeType mergeroms)
        {
            logger.User("Creating split sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            Items.BucketBy(Field.Machine_Name, mergeroms, norename: true);

            // Now we want to loop through all of the games and set the correct information
            RemoveRomsFromChild();

            // Now that we have looped through the cloneof tags, we loop through the romof tags
            RemoveBiosRomsFromChild(false);
            RemoveBiosRomsFromChild(true);

            // Finally, remove the romof and cloneof tags so it's not picked up by the manager
            RemoveTagsFromChild();
        }

        /// <summary>
        /// Use romof tags to add roms to the children
        /// </summary>
        private void AddRomsFromBios()
        {
            List<string> games = Items.Keys.OrderBy(g => g).ToList();
            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                if (Items[game].Count == 0)
                    continue;

                // Determine if the game has a parent or not
                string parent = null;
                if (!string.IsNullOrWhiteSpace(Items[game][0].Machine.RomOf))
                    parent = Items[game][0].Machine.RomOf;

                // If the parent doesnt exist, we want to continue
                if (string.IsNullOrWhiteSpace(parent))
                    continue;

                // If the parent doesn't have any items, we want to continue
                if (Items[parent].Count == 0)
                    continue;

                // If the parent exists and has items, we copy the items from the parent to the current game
                DatItem copyFrom = Items[game][0];
                List<DatItem> parentItems = Items[parent];
                foreach (DatItem item in parentItems)
                {
                    DatItem datItem = (DatItem)item.Clone();
                    datItem.CopyMachineInformation(copyFrom);
                    if (Items[game].Where(i => i.GetName() == datItem.GetName()).Count() == 0 && !Items[game].Contains(datItem))
                        Items.Add(game, datItem);
                }
            }
        }

        /// <summary>
        /// Use device_ref and optionally slotoption tags to add roms to the children
        /// </summary>
        /// <param name="dev">True if only child device sets are touched, false for non-device sets (default)</param>
        /// <param name="useSlotOptions">True if slotoptions tags are used as well, false otherwise</param>
        private bool AddRomsFromDevices(bool dev = false, bool useSlotOptions = false)
        {
            bool foundnew = false;
            List<string> machines = Items.Keys.OrderBy(g => g).ToList();
            foreach (string machine in machines)
            {
                // If the machine doesn't have items, we continue
                if (Items[machine] == null || Items[machine].Count == 0)
                    continue;

                // If the machine (is/is not) a device, we want to continue
                if (dev ^ (Items[machine][0].Machine.MachineType.HasFlag(MachineType.Device)))
                    continue;

                // Get all device reference names from the current machine
                List<string> deviceReferences = Items[machine]
                    .Where(i => i.ItemType == ItemType.DeviceReference)
                    .Select(i => i as DeviceReference)
                    .Select(dr => dr.Name)
                    .Distinct()
                    .ToList();

                // Get all slot option names from the current machine
                List<string> slotOptions = Items[machine]
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
                        if (Items[deviceReference] == null || Items[deviceReference].Count == 0)
                            continue;

                        // Add to the list of new device reference names
                        List<DatItem> devItems = Items[deviceReference];
                        newDeviceReferences.AddRange(devItems
                            .Where(i => i.ItemType == ItemType.DeviceReference)
                            .Select(i => (i as DeviceReference).Name));

                        // Set new machine information and add to the current machine
                        DatItem copyFrom = Items[machine][0];
                        foreach (DatItem item in devItems)
                        {
                            // If the parent machine doesn't already contain this item, add it
                            if (!Items[machine].Any(i => i.ItemType == item.ItemType && i.GetName() == item.GetName()))
                            {
                                // Set that we found new items
                                foundnew = true;

                                // Clone the item and then add it
                                DatItem datItem = (DatItem)item.Clone();
                                datItem.CopyMachineInformation(copyFrom);
                                Items.Add(machine, datItem);
                            }
                        }
                    }

                    // Now that every device reference is accounted for, add the new list of device references, if they don't already exist
                    foreach (string deviceReference in newDeviceReferences.Distinct())
                    {
                        if (!deviceReferences.Contains(deviceReference))
                            Items[machine].Add(new DeviceReference() { Name = deviceReference });
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
                        if (Items[slotOption] == null || Items[slotOption].Count == 0)
                            continue;

                        // Add to the list of new slot option names
                        List<DatItem> slotItems = Items[slotOption];
                        newSlotOptions.AddRange(slotItems
                            .Where(i => i.ItemType == ItemType.Slot)
                            .Where(s => (s as Slot).SlotOptionsSpecified)
                            .SelectMany(s => (s as Slot).SlotOptions)
                            .Select(o => o.DeviceName));

                        // Set new machine information and add to the current machine
                        DatItem copyFrom = Items[machine][0];
                        foreach (DatItem item in slotItems)
                        {
                            // If the parent machine doesn't already contain this item, add it
                            if (!Items[machine].Any(i => i.ItemType == item.ItemType && i.GetName() == item.GetName()))
                            {
                                // Set that we found new items
                                foundnew = true;

                                // Clone the item and then add it
                                DatItem datItem = (DatItem)item.Clone();
                                datItem.CopyMachineInformation(copyFrom);
                                Items.Add(machine, datItem);
                            }
                        }
                    }

                    // Now that every device is accounted for, add the new list of slot options, if they don't already exist
                    foreach (string slotOption in newSlotOptions.Distinct())
                    {
                        if (!slotOptions.Contains(slotOption))
                            Items[machine].Add(new Slot() { SlotOptions = new List<SlotOption> { new SlotOption { DeviceName = slotOption } } });
                    }
                }
            }

            return foundnew;
        }

        /// <summary>
        /// Use cloneof tags to add roms to the children, setting the new romof tag in the process
        /// </summary>
        private void AddRomsFromParent()
        {
            List<string> games = Items.Keys.OrderBy(g => g).ToList();
            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                if (Items[game].Count == 0)
                    continue;

                // Determine if the game has a parent or not
                string parent = null;
                if (!string.IsNullOrWhiteSpace(Items[game][0].Machine.CloneOf))
                    parent = Items[game][0].Machine.CloneOf;

                // If the parent doesnt exist, we want to continue
                if (string.IsNullOrWhiteSpace(parent))
                    continue;

                // If the parent doesn't have any items, we want to continue
                if (Items[parent].Count == 0)
                    continue;

                // If the parent exists and has items, we copy the items from the parent to the current game
                DatItem copyFrom = Items[game][0];
                List<DatItem> parentItems = Items[parent];
                foreach (DatItem item in parentItems)
                {
                    DatItem datItem = (DatItem)item.Clone();
                    datItem.CopyMachineInformation(copyFrom);
                    if (Items[game].Where(i => i.GetName()?.ToLowerInvariant() == datItem.GetName()?.ToLowerInvariant()).Count() == 0
                        && !Items[game].Contains(datItem))
                    {
                        Items.Add(game, datItem);
                    }
                }

                // Now we want to get the parent romof tag and put it in each of the items
                List<DatItem> items = Items[game];
                string romof = Items[parent][0].Machine.RomOf;
                foreach (DatItem item in items)
                {
                    item.Machine.RomOf = romof;
                }
            }
        }

        /// <summary>
        /// Use cloneof tags to add roms to the parents, removing the child sets in the process
        /// </summary>
        /// <param name="subfolder">True to add DatItems to subfolder of parent (not including Disk), false otherwise</param>
        private void AddRomsFromChildren(bool subfolder = true)
        {
            List<string> games = Items.Keys.OrderBy(g => g).ToList();
            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                if (Items[game].Count == 0)
                    continue;

                // Determine if the game has a parent or not
                string parent = null;
                if (!string.IsNullOrWhiteSpace(Items[game][0].Machine.CloneOf))
                    parent = Items[game][0].Machine.CloneOf;

                // If there is no parent, then we continue
                if (string.IsNullOrWhiteSpace(parent))
                    continue;

                // Otherwise, move the items from the current game to a subfolder of the parent game
                DatItem copyFrom;
                if (Items[parent].Count == 0)
                {
                    copyFrom = new Rom();
                    copyFrom.Machine.Name = parent;
                    copyFrom.Machine.Description = parent;
                }
                else
                {
                    copyFrom = Items[parent][0];
                }

                List<DatItem> items = Items[game];
                foreach (DatItem item in items)
                {
                    // Special disk handling
                    if (item.ItemType == ItemType.Disk)
                    {
                        Disk disk = item as Disk;

                        // If the merge tag exists and the parent already contains it, skip
                        if (disk.MergeTag != null && Items[parent].Where(i => i.ItemType == ItemType.Disk).Select(i => (i as Disk).Name).Contains(disk.MergeTag))
                        {
                            continue;
                        }

                        // If the merge tag exists but the parent doesn't contain it, add to parent
                        else if (disk.MergeTag != null && !Items[parent].Where(i => i.ItemType == ItemType.Disk).Select(i => (i as Disk).Name).Contains(disk.MergeTag))
                        {
                            disk.CopyMachineInformation(copyFrom);
                            Items.Add(parent, disk);
                        }

                        // If there is no merge tag, add to parent
                        else if (disk.MergeTag == null)
                        {
                            disk.CopyMachineInformation(copyFrom);
                            Items.Add(parent, disk);
                        }
                    }

                    // Special rom handling
                    else if (item.ItemType == ItemType.Rom)
                    {
                        Rom rom = item as Rom;

                        // If the merge tag exists and the parent already contains it, skip
                        if (rom.MergeTag != null && Items[parent].Where(i => i.ItemType == ItemType.Rom).Select(i => (i as Rom).Name).Contains(rom.MergeTag))
                        {
                            continue;
                        }

                        // If the merge tag exists but the parent doesn't contain it, add to subfolder of parent
                        else if (rom.MergeTag != null && !Items[parent].Where(i => i.ItemType == ItemType.Rom).Select(i => (i as Rom).Name).Contains(rom.MergeTag))
                        {
                            if (subfolder)
                                rom.Name = $"{rom.Machine.Name}\\{rom.Name}";

                            rom.CopyMachineInformation(copyFrom);
                            Items.Add(parent, rom);
                        }

                        // If the parent doesn't already contain this item, add to subfolder of parent
                        else if (!Items[parent].Contains(item))
                        {
                            if (subfolder)
                                rom.Name = $"{item.Machine.Name}\\{rom.Name}";

                            rom.CopyMachineInformation(copyFrom);
                            Items.Add(parent, rom);
                        }
                    }

                    // All other that would be missing to subfolder of parent
                    else if (!Items[parent].Contains(item))
                    {
                        if (subfolder)
                            item.SetFields(new Dictionary<Field, string> { [Field.DatItem_Name] = $"{item.Machine.Name}\\{item.GetName()}" });

                        item.CopyMachineInformation(copyFrom);
                        Items.Add(parent, item);
                    }
                }

                // Then, remove the old game so it's not picked up by the writer
                Items.Remove(game);
            }
        }

        /// <summary>
        /// Remove all BIOS and device sets
        /// </summary>
        private void RemoveBiosAndDeviceSets()
        {
            List<string> games = Items.Keys.OrderBy(g => g).ToList();
            foreach (string game in games)
            {
                if (Items[game].Count > 0
                    && (Items[game][0].Machine.MachineType.HasFlag(MachineType.Bios)
                        || Items[game][0].Machine.MachineType.HasFlag(MachineType.Device)))
                {
                    Items.Remove(game);
                }
            }
        }

        /// <summary>
        /// Use romof tags to remove bios roms from children
        /// </summary>
        /// <param name="bios">True if only child Bios sets are touched, false for non-bios sets (default)</param>
        private void RemoveBiosRomsFromChild(bool bios = false)
        {
            // Loop through the romof tags
            List<string> games = Items.Keys.OrderBy(g => g).ToList();
            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                if (Items[game].Count == 0)
                    continue;

                // If the game (is/is not) a bios, we want to continue
                if (bios ^ Items[game][0].Machine.MachineType.HasFlag(MachineType.Bios))
                    continue;

                // Determine if the game has a parent or not
                string parent = null;
                if (!string.IsNullOrWhiteSpace(Items[game][0].Machine.RomOf))
                    parent = Items[game][0].Machine.RomOf;

                // If the parent doesnt exist, we want to continue
                if (string.IsNullOrWhiteSpace(parent))
                    continue;

                // If the parent doesn't have any items, we want to continue
                if (Items[parent].Count == 0)
                    continue;

                // If the parent exists and has items, we remove the items that are in the parent from the current game
                List<DatItem> parentItems = Items[parent];
                foreach (DatItem item in parentItems)
                {
                    DatItem datItem = (DatItem)item.Clone();
                    while (Items[game].Contains(datItem))
                    {
                        Items.Remove(game, datItem);
                    }
                }
            }
        }

        /// <summary>
        /// Use cloneof tags to remove roms from the children
        /// </summary>
        private void RemoveRomsFromChild()
        {
            List<string> games = Items.Keys.OrderBy(g => g).ToList();
            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                if (Items[game].Count == 0)
                    continue;

                // Determine if the game has a parent or not
                string parent = null;
                if (!string.IsNullOrWhiteSpace(Items[game][0].Machine.CloneOf))
                    parent = Items[game][0].Machine.CloneOf;

                // If the parent doesnt exist, we want to continue
                if (string.IsNullOrWhiteSpace(parent))
                    continue;

                // If the parent doesn't have any items, we want to continue
                if (Items[parent].Count == 0)
                    continue;

                // If the parent exists and has items, we remove the parent items from the current game
                List<DatItem> parentItems = Items[parent];
                foreach (DatItem item in parentItems)
                {
                    DatItem datItem = (DatItem)item.Clone();
                    while (Items[game].Contains(datItem))
                    {
                        Items.Remove(game, datItem);
                    }
                }

                // Now we want to get the parent romof tag and put it in each of the remaining items
                List<DatItem> items = Items[game];
                string romof = Items[parent][0].Machine.RomOf;
                foreach (DatItem item in items)
                {
                    item.Machine.RomOf = romof;
                }
            }
        }

        /// <summary>
        /// Remove all romof and cloneof tags from all games
        /// </summary>
        private void RemoveTagsFromChild()
        {
            List<string> games = Items.Keys.OrderBy(g => g).ToList();
            foreach (string game in games)
            {
                List<DatItem> items = Items[game];
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