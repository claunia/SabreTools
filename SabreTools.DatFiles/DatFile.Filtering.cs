using System;
using System.Collections.Generic;
#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#endif
#if NET40_OR_GREATER || NETCOREAPP
using System.Threading.Tasks;
#endif
using SabreTools.Core;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace SabreTools.DatFiles
{
    public partial class DatFile
    {
        #region Constants

        /// <summary>
        /// Scene name Regex pattern
        /// </summary>
        private const string SceneNamePattern = @"([0-9]{2}\.[0-9]{2}\.[0-9]{2}-)(.*?-.*?)";

        #endregion

        #region Filtering

        /// <summary>
        /// Use game descriptions as names, updating cloneof/romof/sampleof
        /// </summary>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        public void MachineDescriptionToName(bool throwOnError = false)
        {
            MachineDescriptionToNameImpl(throwOnError);
            MachineDescriptionToNameImplDB(throwOnError);
        }

        /// <summary>
        /// Ensure that all roms are in their own game (or at least try to ensure)
        /// </summary>
        public void SetOneRomPerGame()
        {
            SetOneRomPerGameImpl();
            SetOneRomPerGameImplDB();
        }

        /// <summary>
        /// Filter a DAT using 1G1R logic given an ordered set of regions
        /// </summary>
        /// <param name="regionList">List of regions in order of priority</param>
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
        public void SetOneGamePerRegion(List<string> regionList)
        {
            SetOneGamePerRegionImpl(regionList);
            SetOneGamePerRegionImplDB(regionList);
        }

        /// <summary>
        /// Strip the dates from the beginning of scene-style set names
        /// </summary>
        public void StripSceneDatesFromItems()
        {
            StripSceneDatesFromItemsImpl();
            StripSceneDatesFromItemsImplDB();
        }

        #endregion

        #region Filtering Implementations

        /// <summary>
        /// Create machine to description mapping dictionary
        /// </summary>
        /// <remarks>Applies to <see cref="Items"/></remarks>
        private IDictionary<string, string> CreateMachineToDescriptionMapping()
        {
#if NET40_OR_GREATER || NETCOREAPP
            ConcurrentDictionary<string, string> mapping = new();
#else
            Dictionary<string, string> mapping = [];
#endif
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(Items.Keys, Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(Items.Keys, key =>
#else
            foreach (var key in Items.Keys)
#endif
            {
                var items = GetItemsForBucket(key);
                if (items == null)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif

                foreach (DatItem item in items)
                {
                    // Get the current machine
                    var machine = item.GetFieldValue<Machine>(DatItem.MachineKey);
                    if (machine == null)
                        continue;

                    // Get the values to check against
                    string? machineName = machine.GetStringFieldValue(Models.Metadata.Machine.NameKey);
                    string? machineDesc = machine.GetStringFieldValue(Models.Metadata.Machine.DescriptionKey);
                    if (machineName == null || machineDesc == null)
                        continue;

                    // Adjust the description
                    machineDesc = machineDesc.Replace('/', '_').Replace("\"", "''").Replace(":", " -");
                    if (machineName == machineDesc)
                        continue;

                    // If the key mapping doesn't exist, add it
#if NET40_OR_GREATER || NETCOREAPP
                    mapping.TryAdd(machineName, machineDesc);
#else
                    if (!mapping.ContainsKey(machineName))
                        mapping[machineName] = machineDesc;
#endif
                }
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif

            return mapping;
        }

        /// <summary>
        /// Create machine to description mapping dictionary
        /// </summary>
        /// <remarks>Applies to <see cref="ItemsDB"/></remarks>
        private Dictionary<string, string> CreateMachineToDescriptionMappingDB()
        {
            Dictionary<string, string> mapping = [];
            foreach (var machine in GetMachinesDB())
            {
                // Get the current machine
                if (machine.Value == null)
                    continue;

                // Get the values to check against
                string? machineName = machine.Value.GetStringFieldValue(Models.Metadata.Machine.NameKey);
                string? machineDesc = machine.Value.GetStringFieldValue(Models.Metadata.Machine.DescriptionKey);
                if (machineName == null || machineDesc == null)
                    continue;

                // Adjust the description
                machineDesc = machineDesc.Replace('/', '_').Replace("\"", "''").Replace(":", " -");
                if (machineName == machineDesc)
                    continue;

                // If the key mapping doesn't exist, add it
                if (!mapping.ContainsKey(machineName))
                    mapping[machineName] = machineDesc;
            }

            return mapping;
        }

        /// <summary>
        /// Use game descriptions as names, updating cloneof/romof/sampleof
        /// </summary>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        /// <remarks>Applies to <see cref="Items"/></remarks>
        private void MachineDescriptionToNameImpl(bool throwOnError = false)
        {
            try
            {
                // First we want to get a mapping for all games to description
                var mapping = CreateMachineToDescriptionMapping();

                // Now we loop through every item and update accordingly
                UpdateMachineNamesFromDescriptions(mapping);
            }
            catch (Exception ex) when (!throwOnError)
            {
                _logger.Warning(ex.ToString());
            }
        }

        /// <summary>
        /// Use game descriptions as names, updating cloneof/romof/sampleof
        /// </summary>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        /// <remarks>Applies to <see cref="ItemsDB"/></remarks>
        private void MachineDescriptionToNameImplDB(bool throwOnError = false)
        {
            try
            {
                // First we want to get a mapping for all games to description
                var mapping = CreateMachineToDescriptionMappingDB();

                // Now we loop through every item and update accordingly
                UpdateMachineNamesFromDescriptionsDB(mapping);
            }
            catch (Exception ex) when (!throwOnError)
            {
                _logger.Warning(ex.ToString());
            }
        }

        /// <summary>
        /// Filter a DAT using 1G1R logic given an ordered set of regions
        /// </summary>
        /// <param name="regionList">List of regions in order of priority</param>
        /// <remarks>Applies to <see cref="Items"/></remarks>
        private void SetOneGamePerRegionImpl(List<string> regionList)
        {
            // If we have null region list, make it empty
            regionList ??= [];

            // For sake of ease, the first thing we want to do is bucket by game
            BucketBy(ItemKey.Machine, DedupeType.None, norename: true);

            // Then we want to get a mapping of all machines to parents
            Dictionary<string, List<string>> parents = [];
            foreach (string key in Items.Keys)
            {
                DatItem item = GetItemsForBucket(key)[0];

                // Get machine information
                Machine? machine = item.GetFieldValue<Machine>(DatItem.MachineKey);
                string? machineName = machine?.GetStringFieldValue(Models.Metadata.Machine.NameKey)?.ToLowerInvariant();
                if (machine == null || machineName == null)
                    continue;

                // Get the string values
                string? cloneOf = machine.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey)?.ToLowerInvariant();
                string? romOf = machine.GetStringFieldValue(Models.Metadata.Machine.RomOfKey)?.ToLowerInvariant();

                // Match on CloneOf first
                if (!string.IsNullOrEmpty(cloneOf))
                {
                    if (!parents.ContainsKey(cloneOf!))
                        parents.Add(cloneOf!, []);

                    parents[cloneOf!].Add(machineName);
                }

                // Then by RomOf
                else if (!string.IsNullOrEmpty(romOf))
                {
                    if (!parents.ContainsKey(romOf!))
                        parents.Add(romOf!, []);

                    parents[romOf!].Add(machineName);
                }

                // Otherwise, treat it as a parent
                else
                {
                    if (!parents.ContainsKey(machineName))
                        parents.Add(machineName, []);

                    parents[machineName].Add(machineName);
                }
            }

            // Once we have the full list of mappings, filter out games to keep
            foreach (string key in parents.Keys)
            {
                // Find the first machine that matches the regions in order, if possible
                string? machine = default;
                foreach (string region in regionList)
                {
                    machine = parents[key].Find(m => Regex.IsMatch(m, @"\(.*" + region + @".*\)", RegexOptions.IgnoreCase));
                    if (machine != default)
                        break;
                }

                // If we didn't get a match, use the parent
                if (machine == default)
                    machine = key;

                // Remove the key from the list
                parents[key].Remove(machine);

                // Remove the rest of the items from this key
                parents[key].ForEach(k => Remove(k));
            }

            // Finally, strip out the parent tags
            RemoveMachineRelationshipTagsImpl();
        }

        /// <summary>
        /// Filter a DAT using 1G1R logic given an ordered set of regions
        /// </summary>
        /// <param name="regionList">List of regions in order of priority</param>
        /// <remarks>Applies to <see cref="ItemsDB"/></remarks>
        private void SetOneGamePerRegionImplDB(List<string> regionList)
        {
            // If we have null region list, make it empty
            regionList ??= [];

            // For sake of ease, the first thing we want to do is bucket by game
            BucketBy(ItemKey.Machine, DedupeType.None, norename: true);

            // Then we want to get a mapping of all machines to parents
            Dictionary<string, List<string>> parents = [];
            foreach (string key in ItemsDB.SortedKeys)
            {
                var items = GetItemsForBucketDB(key);
                if (items == null || items.Count == 0)
                    continue;

                var item = items.First();
                var machine = ItemsDB.GetMachineForItem(item.Key);
                if (machine.Value == null)
                    continue;

                // Get machine information
                Machine? machineObj = machine.Value.GetFieldValue<Machine>(DatItem.MachineKey);
                string? machineName = machineObj?.GetStringFieldValue(Models.Metadata.Machine.NameKey)?.ToLowerInvariant();
                if (machineObj == null || machineName == null)
                    continue;

                // Get the string values
                string? cloneOf = machineObj.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey)?.ToLowerInvariant();
                string? romOf = machineObj.GetStringFieldValue(Models.Metadata.Machine.RomOfKey)?.ToLowerInvariant();

                // Match on CloneOf first
                if (!string.IsNullOrEmpty(cloneOf))
                {
                    if (!parents.ContainsKey(cloneOf!))
                        parents.Add(cloneOf!, []);

                    parents[cloneOf!].Add(machineName);
                }

                // Then by RomOf
                else if (!string.IsNullOrEmpty(romOf))
                {
                    if (!parents.ContainsKey(romOf!))
                        parents.Add(romOf!, []);

                    parents[romOf!].Add(machineName);
                }

                // Otherwise, treat it as a parent
                else
                {
                    if (!parents.ContainsKey(machineName))
                        parents.Add(machineName, []);

                    parents[machineName].Add(machineName);
                }
            }

            // Once we have the full list of mappings, filter out games to keep
            foreach (string key in parents.Keys)
            {
                // Find the first machine that matches the regions in order, if possible
                string? machine = default;
                foreach (string region in regionList)
                {
                    machine = parents[key].Find(m => Regex.IsMatch(m, @"\(.*" + region + @".*\)", RegexOptions.IgnoreCase));
                    if (machine != default)
                        break;
                }

                // If we didn't get a match, use the parent
                if (machine == default)
                    machine = key;

                // Remove the key from the list
                parents[key].Remove(machine);

                // Remove the rest of the items from this key
                parents[key].ForEach(k => ItemsDB.RemoveMachine(k));
            }

            // Finally, strip out the parent tags
            RemoveMachineRelationshipTagsImplDB();
        }

        /// <summary>
        /// Ensure that all roms are in their own game (or at least try to ensure)
        /// </summary>
        /// <remarks>Applies to <see cref="Items"/></remarks>
        private void SetOneRomPerGameImpl()
        {
            // For each rom, we want to update the game to be "<game name>/<rom name>"
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(Items.Keys, Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(Items.Keys, key =>
#else
            foreach (var key in Items.Keys)
#endif
            {
                var items = GetItemsForBucket(key);
                if (items == null)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif

                for (int i = 0; i < items.Count; i++)
                {
                    SetOneRomPerGameImpl(items[i]);
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
        /// <remarks>Applies to <see cref="Items"/></remarks>
        private static void SetOneRomPerGameImpl(DatItem datItem)
        {
            // If the item name is null
            string? itemName = datItem.GetName();
            if (itemName == null)
                return;

            // Get the current machine
            var machine = datItem.GetFieldValue<Machine>(DatItem.MachineKey);
            if (machine == null)
                return;

            // Clone current machine to avoid conflict
            machine = (Machine)machine.Clone();

            // Reassign the item to the new machine
            datItem.SetFieldValue<Machine>(DatItem.MachineKey, machine);

            // Remove extensions from File and Rom items
            if (datItem is DatItems.Formats.File || datItem is Rom)
            {
                string[] splitname = itemName.Split('.');
                itemName = machine.GetStringFieldValue(Models.Metadata.Machine.NameKey)
                    + $"/{string.Join(".", splitname, 0, splitname.Length > 1 ? splitname.Length - 1 : 1)}";
            }
            else
            {
                itemName = machine.GetStringFieldValue(Models.Metadata.Machine.NameKey) + $"/{itemName}";
            }

            // Strip off "Default" prefix only for ORPG
            if (itemName.StartsWith("Default"))
                itemName = itemName.Substring("Default".Length + 1);

            machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, itemName);
            datItem.SetName(Path.GetFileName(datItem.GetName()));
        }

        /// <summary>
        /// Ensure that all roms are in their own game (or at least try to ensure)
        /// </summary>
        /// <remarks>Applies to <see cref="ItemsDB"/></remarks>
        private void SetOneRomPerGameImplDB()
        {
            // For each rom, we want to update the game to be "<game name>/<rom name>"
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(ItemsDB.SortedKeys, Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(ItemsDB.SortedKeys, key =>
#else
            foreach (var key in ItemsDB.SortedKeys)
#endif
            {
                var items = GetItemsForBucketDB(key);
                if (items == null)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif

                foreach (var item in items)
                {
                    SetOneRomPerGameImplDB(item);
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
        /// <remarks>Applies to <see cref="ItemsDB"/></remarks>
        private void SetOneRomPerGameImplDB(KeyValuePair<long, DatItem> datItem)
        {
            // If the item name is null
            string? itemName = datItem.Value.GetName();
            if (datItem.Key < 0 || itemName == null)
                return;

            // Get the current machine
            var machine = ItemsDB.GetMachineForItem(datItem.Key);
            if (machine.Value == null)
                return;

            // Clone current machine to avoid conflict
            long newMachineIndex = AddMachineDB((Machine)machine.Value.Clone());
            machine = new KeyValuePair<long, Machine?>(newMachineIndex, ItemsDB.GetMachine(newMachineIndex));
            if (machine.Value == null)
                return;

            // Reassign the item to the new machine
            ItemsDB._itemToMachineMapping[datItem.Key] = newMachineIndex;

            // Remove extensions from File and Rom items
            if (datItem.Value is DatItems.Formats.File || datItem.Value is Rom)
            {
                string[] splitname = itemName.Split('.');
                itemName = machine.Value.GetStringFieldValue(Models.Metadata.Machine.NameKey)
                    + $"/{string.Join(".", splitname, 0, splitname.Length > 1 ? splitname.Length - 1 : 1)}";
            }
            else
            {
                itemName = machine.Value.GetStringFieldValue(Models.Metadata.Machine.NameKey) + $"/{itemName}";
            }

            // Strip off "Default" prefix only for ORPG
            if (itemName.StartsWith("Default"))
                itemName = itemName.Substring("Default".Length + 1);

            machine.Value.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, itemName);
            datItem.Value.SetName(Path.GetFileName(datItem.Value.GetName()));
        }

        /// <summary>
        /// Strip the dates from the beginning of scene-style set names
        /// </summary>
        /// <remarks>Applies to <see cref="Items"/></remarks>
        private void StripSceneDatesFromItemsImpl()
        {
            // Now process all of the roms
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(Items.Keys, Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(Items.Keys, key =>
#else
            foreach (var key in Items.Keys)
#endif
            {
                var items = GetItemsForBucket(key);
                if (items == null)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif

                foreach (DatItem item in items)
                {
                    // Get the current machine
                    var machine = item.GetFieldValue<Machine>(DatItem.MachineKey);
                    if (machine == null)
                        continue;

                    // Get the values to check against
                    string? machineName = machine.GetStringFieldValue(Models.Metadata.Machine.NameKey);
                    string? machineDesc = machine.GetStringFieldValue(Models.Metadata.Machine.DescriptionKey);

                    if (machineName != null && Regex.IsMatch(machineName, SceneNamePattern))
                        item.GetFieldValue<Machine>(DatItem.MachineKey)!.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, Regex.Replace(machineName, SceneNamePattern, "$2"));

                    if (machineDesc != null && Regex.IsMatch(machineDesc, SceneNamePattern))
                        item.GetFieldValue<Machine>(DatItem.MachineKey)!.SetFieldValue<string?>(Models.Metadata.Machine.DescriptionKey, Regex.Replace(machineDesc, SceneNamePattern, "$2"));
                }
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif
        }

        /// <summary>
        /// Strip the dates from the beginning of scene-style set names
        /// </summary>
        /// <remarks>Applies to <see cref="ItemsDB"/></remarks>
        private void StripSceneDatesFromItemsImplDB()
        {
            // Now process all of the machines
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(GetMachinesDB(), Core.Globals.ParallelOptions, machine =>
#elif NET40_OR_GREATER
            Parallel.ForEach(GetMachinesDB(), machine =>
#else
            foreach (var machine in GetMachinesDB())
#endif
            {
                // Get the current machine
                if (machine.Value == null)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif

                // Get the values to check against
                string? machineName = machine.Value.GetStringFieldValue(Models.Metadata.Machine.NameKey);
                string? machineDesc = machine.Value.GetStringFieldValue(Models.Metadata.Machine.DescriptionKey);

                if (machineName != null && Regex.IsMatch(machineName, SceneNamePattern))
                    machine.Value.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, Regex.Replace(machineName, SceneNamePattern, "$2"));

                if (machineDesc != null && Regex.IsMatch(machineDesc, SceneNamePattern))
                    machine.Value.SetFieldValue<string?>(Models.Metadata.Machine.DescriptionKey, Regex.Replace(machineDesc, SceneNamePattern, "$2"));
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif
        }

        /// <summary>
        /// Update machine names from descriptions according to mappings
        /// </summary>
        /// <remarks>Applies to <see cref="Items"/></remarks>
        private void UpdateMachineNamesFromDescriptions(IDictionary<string, string> mapping)
        {
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(Items.Keys, Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(Items.Keys, key =>
#else
            foreach (var key in Items.Keys)
#endif
            {
                var items = GetItemsForBucket(key);
                if (items == null)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif

                foreach (DatItem item in items)
                {
                    // Get the current machine
                    var machine = item.GetFieldValue<Machine>(DatItem.MachineKey);
                    if (machine == null)
                        continue;

                    // Get the values to check against
                    string? machineName = machine.GetStringFieldValue(Models.Metadata.Machine.NameKey);
                    string? cloneOf = machine.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey);
                    string? romOf = machine.GetStringFieldValue(Models.Metadata.Machine.RomOfKey);
                    string? sampleOf = machine.GetStringFieldValue(Models.Metadata.Machine.SampleOfKey);

                    // Update machine name
                    if (machineName != null && mapping.ContainsKey(machineName))
                        machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, mapping[machineName]);

                    // Update cloneof
                    if (cloneOf != null && mapping.ContainsKey(cloneOf))
                        machine.SetFieldValue<string?>(Models.Metadata.Machine.CloneOfKey, mapping[cloneOf]);

                    // Update romof
                    if (romOf != null && mapping.ContainsKey(romOf))
                        machine.SetFieldValue<string?>(Models.Metadata.Machine.RomOfKey, mapping[romOf]);

                    // Update sampleof
                    if (sampleOf != null && mapping.ContainsKey(sampleOf))
                        machine.SetFieldValue<string?>(Models.Metadata.Machine.SampleOfKey, mapping[sampleOf]);
                }
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif
        }

        /// <summary>
        /// Update machine names from descriptions according to mappings
        /// </summary>
        /// <remarks>Applies to <see cref="ItemsDB"/></remarks>
        private void UpdateMachineNamesFromDescriptionsDB(Dictionary<string, string> mapping)
        {
            foreach (var machine in GetMachinesDB())
            {
                // Get the current machine
                if (machine.Value == null)
                    continue;

                // Get the values to check against
                string? machineName = machine.Value.GetStringFieldValue(Models.Metadata.Machine.NameKey);
                string? cloneOf = machine.Value.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey);
                string? romOf = machine.Value.GetStringFieldValue(Models.Metadata.Machine.RomOfKey);
                string? sampleOf = machine.Value.GetStringFieldValue(Models.Metadata.Machine.SampleOfKey);

                // Update machine name
                if (machineName != null && mapping.ContainsKey(machineName))
                    machine.Value.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, mapping[machineName]);

                // Update cloneof
                if (cloneOf != null && mapping.ContainsKey(cloneOf))
                    machine.Value.SetFieldValue<string?>(Models.Metadata.Machine.CloneOfKey, mapping[cloneOf]);

                // Update romof
                if (romOf != null && mapping.ContainsKey(romOf))
                    machine.Value.SetFieldValue<string?>(Models.Metadata.Machine.RomOfKey, mapping[romOf]);

                // Update sampleof
                if (sampleOf != null && mapping.ContainsKey(sampleOf))
                    machine.Value.SetFieldValue<string?>(Models.Metadata.Machine.SampleOfKey, mapping[sampleOf]);
            }
        }

        #endregion
    }
}