using System;
using System.Collections.Generic;
using System.Linq;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;

namespace SabreTools.DatFiles
{
    public partial class DatFile
    {
        // TODO: Make all private methods internal
        // TODO: Create tests for all of these individually
        #region Item Dictionary Passthrough - Splitting

        /// <summary>
        /// Use romof tags to add roms to the children
        /// </summary>
        public void AddRomsFromBios()
        {
            AddRomsFromBiosImpl();
            AddRomsFromBiosImplDB();
        }

        /// <summary>
        /// Use cloneof tags to add roms to the parents, removing the child sets in the process
        /// </summary>
        /// <param name="subfolder">True to add DatItems to subfolder of parent (not including Disk), false otherwise</param>
        /// <param name="skipDedup">True to skip checking for duplicate ROMs in parent, false otherwise</param>
        public void AddRomsFromChildren(bool subfolder, bool skipDedup)
        {
            AddRomsFromChildrenImpl(subfolder, skipDedup);
            AddRomsFromChildrenImplDB(subfolder, skipDedup);
        }

        /// <summary>
        /// Use device_ref and optionally slotoption tags to add roms to the children
        /// </summary>
        /// <param name="dev">True if only child device sets are touched, false for non-device sets</param>
        /// <param name="useSlotOptions">True if slotoptions tags are used as well, false otherwise</param>
        public bool AddRomsFromDevices(bool dev, bool useSlotOptions)
        {
            bool foundnew = AddRomsFromDevicesImpl(dev, useSlotOptions);
            foundnew |= AddRomsFromDevicesImplDB(dev, useSlotOptions);
            return foundnew;
        }

        /// <summary>
        /// Use cloneof tags to add roms to the children, setting the new romof tag in the process
        /// </summary>
        public void AddRomsFromParent()
        {
            AddRomsFromParentImpl();
            AddRomsFromParentImplDB();
        }

        /// <summary>
        /// Remove all BIOS and device sets
        /// </summary>
        public void RemoveBiosAndDeviceSets()
        {
            RemoveBiosAndDeviceSetsImpl();
            RemoveBiosAndDeviceSetsImplDB();
        }

        /// <summary>
        /// Use romof tags to remove bios roms from children
        /// </summary>
        /// <param name="bios">True if only child Bios sets are touched, false for non-bios sets</param>
        public void RemoveBiosRomsFromChild(bool bios)
        {
            RemoveBiosRomsFromChildImpl(bios);
            RemoveBiosRomsFromChildImplDB(bios);
        }

        /// <summary>
        /// Use cloneof tags to remove roms from the children
        /// </summary>
        public void RemoveRomsFromChild()
        {
            RemoveRomsFromChildImpl();
            RemoveRomsFromChildImplDB();
        }

        /// <summary>
        /// Remove all romof and cloneof tags from all games
        /// </summary>
        public void RemoveMachineRelationshipTags()
        {
            RemoveMachineRelationshipTagsImpl();
            RemoveMachineRelationshipTagsImplDB();
        }

        #endregion

        #region Splitting Implementations

        /// <summary>
        /// Use romof tags to add roms to the children
        /// </summary>
        private void AddRomsFromBiosImpl()
        {
            List<string> games = [.. Items.Keys];
            games.Sort();

            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                var items = Items[game];
                if (items == null || items.Count == 0)
                    continue;

                // Get the machine
                var machine = items[0].GetFieldValue<Machine>(DatItem.MachineKey);
                if (machine == null)
                    continue;

                // Get the bios parent
                string? romOf = machine.GetStringFieldValue(Models.Metadata.Machine.RomOfKey);
                if (string.IsNullOrEmpty(romOf))
                    continue;

                // If the parent doesn't have any items, we want to continue
                var parentItems = Items[romOf!];
                if (parentItems == null || parentItems.Count == 0)
                    continue;

                // If the parent exists and has items, we copy the items from the parent to the current game
                DatItem copyFrom = items[0];
                foreach (DatItem item in parentItems)
                {
                    DatItem datItem = (DatItem)item.Clone();
                    datItem.CopyMachineInformation(copyFrom);
                    if (items.FindIndex(i => i.GetName() == datItem.GetName()) == -1 && !items.Contains(datItem))
                        Add(game, datItem);
                }
            }
        }

        /// <summary>
        /// Use romof tags to add roms to the children
        /// </summary>
        private void AddRomsFromBiosImplDB()
        {
            List<string> games = [.. ItemsDB.SortedKeys];
            foreach (string game in games)
            {
                // Get the items for this game
                var items = GetItemsForBucketDB(game);
                if (items == null || items.Count == 0)
                    continue;

                // Get the source for the first item
                var source = ItemsDB.GetSourceForItem(items.First().Key);

                // Get the machine for the first item
                var machine = ItemsDB.GetMachineForItem(items.First().Key);
                if (machine.Value == null)
                    continue;

                // Get the bios parent
                string? romOf = machine.Value.GetStringFieldValue(Models.Metadata.Machine.RomOfKey);
                if (string.IsNullOrEmpty(romOf))
                    continue;

                // If the parent doesn't have any items, we want to continue
                var parentItems = GetItemsForBucketDB(romOf!);
                if (parentItems == null || parentItems.Count == 0)
                    continue;

                // If the parent exists and has items, we copy the items from the parent to the current game
                foreach (var item in parentItems)
                {
                    DatItem datItem = (item.Value.Clone() as DatItem)!;
                    if (items.Any(i => i.Value.GetName() == datItem.GetName())
                        && items.Any(i => i.Value == datItem))
                    {
                        ItemsDB.AddItem(datItem, machine.Key, source.Key);
                    }
                }
            }
        }

        /// <summary>
        /// Use cloneof tags to add roms to the parents, removing the child sets in the process
        /// </summary>
        /// <param name="subfolder">True to add DatItems to subfolder of parent (not including Disk), false otherwise</param>
        /// <param name="skipDedup">True to skip checking for duplicate ROMs in parent, false otherwise</param>
        private void AddRomsFromChildrenImpl(bool subfolder, bool skipDedup)
        {
            List<string> games = [.. Items.Keys];
            games.Sort();

            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                var items = Items[game];
                if (items == null || items.Count == 0)
                    continue;

                // Get the machine
                var machine = items[0].GetFieldValue<Machine>(DatItem.MachineKey);
                if (machine == null)
                    continue;

                // Get the clone parent
                string? cloneOf = machine.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey);
                if (string.IsNullOrEmpty(cloneOf))
                    continue;

                // Get the parent items
                var parentItems = Items[cloneOf!];

                // Otherwise, move the items from the current game to a subfolder of the parent game
                DatItem copyFrom;
                if (parentItems == null || parentItems.Count == 0)
                {
                    copyFrom = new Rom();
                    copyFrom.GetFieldValue<Machine>(DatItem.MachineKey)!.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, cloneOf);
                    copyFrom.GetFieldValue<Machine>(DatItem.MachineKey)!.SetFieldValue<string?>(Models.Metadata.Machine.DescriptionKey, cloneOf);
                }
                else
                {
                    copyFrom = parentItems[0];
                }

                items = Items[game];
                foreach (DatItem item in items!)
                {
                    // Special disk handling
                    if (item is Disk disk)
                    {
                        string? mergeTag = disk.GetStringFieldValue(Models.Metadata.Disk.MergeKey);

                        // If the merge tag exists and the parent already contains it, skip
                        if (mergeTag != null && Items[cloneOf!]!
                            .FindAll(i => i is Disk)
                            .ConvertAll(i => (i as Disk)!.GetName()).Contains(mergeTag))
                        {
                            continue;
                        }

                        // If the merge tag exists but the parent doesn't contain it, add to parent
                        else if (mergeTag != null && !Items[cloneOf!]!
                            .FindAll(i => i is Disk)
                            .ConvertAll(i => (i as Disk)!.GetName()).Contains(mergeTag))
                        {
                            disk.CopyMachineInformation(copyFrom);
                            Add(cloneOf!, disk);
                        }

                        // If there is no merge tag, add to parent
                        else if (mergeTag == null)
                        {
                            disk.CopyMachineInformation(copyFrom);
                            Add(cloneOf!, disk);
                        }
                    }

                    // Special rom handling
                    else if (item is Rom rom)
                    {
                        // If the merge tag exists and the parent already contains it, skip
                        if (rom.GetStringFieldValue(Models.Metadata.Rom.MergeKey) != null && Items[cloneOf!]!
                            .FindAll(i => i is Rom)
                            .ConvertAll(i => (i as Rom)!.GetName())
                            .Contains(rom.GetStringFieldValue(Models.Metadata.Rom.MergeKey)))
                        {
                            continue;
                        }

                        // If the merge tag exists but the parent doesn't contain it, add to subfolder of parent
                        else if (rom.GetStringFieldValue(Models.Metadata.Rom.MergeKey) != null && !Items[cloneOf!]!
                            .FindAll(i => i is Rom)
                            .ConvertAll(i => (i as Rom)!.GetName())
                            .Contains(rom.GetStringFieldValue(Models.Metadata.Rom.MergeKey)))
                        {
                            if (subfolder)
                                rom.SetName($"{rom.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey)}\\{rom.GetName()}");

                            rom.CopyMachineInformation(copyFrom);
                            Add(cloneOf!, rom);
                        }

                        // If the parent doesn't already contain this item, add to subfolder of parent
                        else if (!Items[cloneOf!]!.Contains(item) || skipDedup)
                        {
                            if (subfolder)
                                rom.SetName($"{item.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey)}\\{rom.GetName()}");

                            rom.CopyMachineInformation(copyFrom);
                            Add(cloneOf!, rom);
                        }
                    }

                    // All other that would be missing to subfolder of parent
                    else if (!Items[cloneOf!]!.Contains(item))
                    {
                        if (subfolder)
                            item.SetName($"{item.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey)}\\{item.GetName()}");

                        item.CopyMachineInformation(copyFrom);
                        Add(cloneOf!, item);
                    }
                }

                // Then, remove the old game so it's not picked up by the writer
                Remove(game);
            }
        }

        /// <summary>
        /// Use cloneof tags to add roms to the parents, removing the child sets in the process
        /// </summary>
        /// <param name="subfolder">True to add DatItems to subfolder of parent (not including Disk), false otherwise</param>
        /// <param name="skipDedup">True to skip checking for duplicate ROMs in parent, false otherwise</param>
        private void AddRomsFromChildrenImplDB(bool subfolder, bool skipDedup)
        {
            List<string> games = [.. ItemsDB.SortedKeys];
            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                var items = GetItemsForBucketDB(game);
                if (items == null || items.Count == 0)
                    continue;

                // Get the machine for the first item
                var machine = ItemsDB.GetMachineForItem(items.First().Key);
                if (machine.Value == null)
                    continue;

                // Get the clone parent
                string? cloneOf = machine.Value.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey);
                if (string.IsNullOrEmpty(cloneOf))
                    continue;

                // Get the clone parent machine
                var cloneOfMachine = ItemsDB.GetMachine(cloneOf);
                if (cloneOfMachine.Value == null)
                    continue;

                items = GetItemsForBucketDB(game);
                foreach (var item in items)
                {
                    // Get the parent items and current machine name
                    var parentItems = GetItemsForBucketDB(cloneOf!);
                    if (parentItems == null || parentItems.Count == 0)
                        continue;

                    string? machineName = ItemsDB.GetMachineForItem(item.Key).Value?
                        .GetStringFieldValue(Models.Metadata.Machine.NameKey);

                    // Special disk handling
                    if (item.Value is Disk disk)
                    {
                        string? mergeTag = disk.GetStringFieldValue(Models.Metadata.Disk.MergeKey);

                        // If the merge tag exists and the parent already contains it, skip
                        if (mergeTag != null && parentItems.Values
                            .Where(i => i is Disk)
                            .Select(i => (i as Disk)!.GetName())
                            .Contains(mergeTag))
                        {
                            continue;
                        }

                        // If the merge tag exists but the parent doesn't contain it, add to parent
                        else if (mergeTag != null && !parentItems.Values
                            .Where(i => i is Disk)
                            .Select(i => (i as Disk)!.GetName())
                            .Contains(mergeTag))
                        {
                            ItemsDB._itemToMachineMapping[item.Key] = cloneOfMachine.Key;
                            ItemsDB._buckets[cloneOf!].Add(item.Key);
                        }

                        // If there is no merge tag, add to parent
                        else if (mergeTag == null)
                        {
                            ItemsDB._itemToMachineMapping[item.Key] = cloneOfMachine.Key;
                            ItemsDB._buckets[cloneOf!].Add(item.Key);
                        }
                    }

                    // Special rom handling
                    else if (item.Value is Rom rom)
                    {
                        // If the merge tag exists and the parent already contains it, skip
                        if (rom.GetStringFieldValue(Models.Metadata.Rom.MergeKey) != null && parentItems.Values
                            .Where(i => i is Rom)
                            .Select(i => (i as Rom)!.GetName())
                            .Contains(rom.GetStringFieldValue(Models.Metadata.Rom.MergeKey)))
                        {
                            continue;
                        }

                        // If the merge tag exists but the parent doesn't contain it, add to subfolder of parent
                        else if (rom.GetStringFieldValue(Models.Metadata.Rom.MergeKey) != null && !parentItems.Values
                            .Where(i => i is Rom)
                            .Select(i => (i as Rom)!.GetName())
                            .Contains(rom.GetStringFieldValue(Models.Metadata.Rom.MergeKey)))
                        {
                            if (subfolder)
                                rom.SetName($"{machineName}\\{rom.GetName()}");

                            ItemsDB._itemToMachineMapping[item.Key] = cloneOfMachine.Key;
                            ItemsDB._buckets[cloneOf!].Add(item.Key);
                        }

                        // If the parent doesn't already contain this item, add to subfolder of parent
                        else if (!parentItems.Contains(item) || skipDedup)
                        {
                            if (subfolder)
                                rom.SetName($"{machineName}\\{rom.GetName()}");

                            ItemsDB._itemToMachineMapping[item.Key] = cloneOfMachine.Key;
                            ItemsDB._buckets[cloneOf!].Add(item.Key);
                        }
                    }

                    // All other that would be missing to subfolder of parent
                    else if (!parentItems.Contains(item))
                    {
                        if (subfolder)
                            item.Value.SetName($"{machineName}\\{item.Value.GetName()}");

                        ItemsDB._itemToMachineMapping[item.Key] = cloneOfMachine.Key;
                        ItemsDB._buckets[cloneOf!].Add(item.Key);
                    }
                }

                // Then, remove the old game so it's not picked up by the writer
#if NET40_OR_GREATER || NETCOREAPP
                ItemsDB._buckets.TryRemove(game, out _);
#else
                ItemsDB._buckets.Remove(game);
#endif
            }
        }

        /// <summary>
        /// Use device_ref and optionally slotoption tags to add roms to the children
        /// </summary>
        /// <param name="dev">True if only child device sets are touched, false for non-device sets (default)</param>
        /// <param name="useSlotOptions">True if slotoptions tags are used as well, false otherwise</param>
        private bool AddRomsFromDevicesImpl(bool dev, bool useSlotOptions)
        {
            bool foundnew = false;
            List<string> machines = [.. Items.Keys];
            machines.Sort();

            foreach (string machine in machines)
            {
                // If the machine doesn't have items, we continue
                var datItems = Items[machine];
                if (datItems == null || datItems.Count == 0)
                    continue;

                // If the machine (is/is not) a device, we want to continue
                if (dev ^ (datItems[0].GetFieldValue<Machine>(DatItem.MachineKey)!.GetBoolFieldValue(Models.Metadata.Machine.IsDeviceKey) == true))
                    continue;

                // Get all device reference names from the current machine
                List<string?> deviceReferences = datItems
                    .FindAll(i => i is DeviceRef)
                    .ConvertAll(i => i as DeviceRef)
                    .ConvertAll(dr => dr!.GetName())
                    .Distinct()
                    .ToList();

                // Get all slot option names from the current machine
                List<string?> slotOptions = datItems
                    .FindAll(i => i is Slot)
                    .ConvertAll(i => i as Slot)
                    .FindAll(s => s!.SlotOptionsSpecified)
                    .SelectMany(s => s!.GetFieldValue<SlotOption[]?>(Models.Metadata.Slot.SlotOptionKey)!)
                    .Select(so => so.GetStringFieldValue(Models.Metadata.SlotOption.DevNameKey))
                    .Distinct()
                    .ToList();

                // If we're checking device references
                if (deviceReferences.Count > 0)
                {
                    // Loop through all names and check the corresponding machines
                    var newDeviceReferences = new HashSet<string>();
                    foreach (string? deviceReference in deviceReferences)
                    {
                        // If the device reference is missing
                        if (string.IsNullOrEmpty(deviceReference))
                            continue;

                        // Add to the list of new device reference names
                        var devItems = Items[deviceReference!];
                        if (devItems == null || devItems.Count == 0)
                            continue;

                        newDeviceReferences.UnionWith(devItems
                            .FindAll(i => i is DeviceRef)
                            .ConvertAll(i => (i as DeviceRef)!.GetName()!));

                        // Set new machine information and add to the current machine
                        DatItem copyFrom = datItems[0];
                        foreach (DatItem item in devItems)
                        {
                            // If the parent machine doesn't already contain this item, add it
                            if (!datItems.Exists(i => i.GetStringFieldValue(Models.Metadata.DatItem.TypeKey) == item.GetStringFieldValue(Models.Metadata.DatItem.TypeKey) && i.GetName() == item.GetName()))
                            {
                                // Set that we found new items
                                foundnew = true;

                                // Clone the item and then add it
                                DatItem datItem = (DatItem)item.Clone();
                                datItem.CopyMachineInformation(copyFrom);
                                Add(machine, datItem);
                            }
                        }
                    }

                    // Now that every device reference is accounted for, add the new list of device references, if they don't already exist
                    foreach (string deviceReference in newDeviceReferences)
                    {
                        if (!deviceReferences.Contains(deviceReference))
                        {
                            var deviceRef = new DeviceRef();
                            deviceRef.SetName(deviceReference);
                            datItems.Add(deviceRef);
                        }
                    }
                }

                // If we're checking slotoptions
                if (useSlotOptions && slotOptions.Count > 0)
                {
                    // Loop through all names and check the corresponding machines
                    var newSlotOptions = new HashSet<string>();
                    foreach (string? slotOption in slotOptions)
                    {
                        // If the slot option is missing
                        if (string.IsNullOrEmpty(slotOption))
                            // If the machine doesn't exist then we continue
                            continue;

                        // Add to the list of new slot option names
                        var slotItems = Items[slotOption!];
                        if (slotItems == null || slotItems.Count == 0)
                            continue;

                        newSlotOptions.UnionWith(slotItems
                            .FindAll(i => i is Slot)
                            .FindAll(s => (s as Slot)!.SlotOptionsSpecified)
                            .SelectMany(s => (s as Slot)!.GetFieldValue<SlotOption[]?>(Models.Metadata.Slot.SlotOptionKey)!)
                            .Select(o => o.GetStringFieldValue(Models.Metadata.SlotOption.DevNameKey)!));

                        // Set new machine information and add to the current machine
                        DatItem copyFrom = datItems[0];
                        foreach (DatItem item in slotItems)
                        {
                            // If the parent machine doesn't already contain this item, add it
                            if (!datItems.Exists(i => i.GetStringFieldValue(Models.Metadata.DatItem.TypeKey) == item.GetStringFieldValue(Models.Metadata.DatItem.TypeKey) && i.GetName() == item.GetName()))
                            {
                                // Set that we found new items
                                foundnew = true;

                                // Clone the item and then add it
                                DatItem datItem = (DatItem)item.Clone();
                                datItem.CopyMachineInformation(copyFrom);
                                Add(machine, datItem);
                            }
                        }
                    }

                    // Now that every device is accounted for, add the new list of slot options, if they don't already exist
                    foreach (string slotOption in newSlotOptions)
                    {
                        if (!slotOptions.Contains(slotOption))
                        {
                            var slotOptionItem = new SlotOption();
                            slotOptionItem.SetFieldValue<string?>(Models.Metadata.SlotOption.DevNameKey, slotOption);

                            var slotItem = new Slot();
                            slotItem.SetFieldValue<SlotOption[]?>(Models.Metadata.Slot.SlotOptionKey, [slotOptionItem]);

                            datItems.Add(slotItem);
                        }
                    }
                }
            }

            return foundnew;
        }

        /// <summary>
        /// Use device_ref and optionally slotoption tags to add roms to the children
        /// </summary>
        /// <param name="dev">True if only child device sets are touched, false for non-device sets</param>
        /// <param name="useSlotOptions">True if slotoptions tags are used as well, false otherwise</param>
        private bool AddRomsFromDevicesImplDB(bool dev, bool useSlotOptions)
        {
            bool foundnew = false;
            List<string> games = [.. ItemsDB.SortedKeys];
            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                var items = GetItemsForBucketDB(game);
                if (items == null || items.Count == 0)
                    continue;

                // Get the source for the first item
                var source = ItemsDB.GetSourceForItem(items.First().Key);

                // Get the machine for the first item
                var machine = ItemsDB.GetMachineForItem(items.First().Key);
                if (machine.Value == null)
                    continue;

                // If the machine (is/is not) a device, we want to continue
                if (dev ^ (machine.Value.GetBoolFieldValue(Models.Metadata.Machine.IsDeviceKey) == true))
                    continue;

                // Get all device reference names from the current machine
                List<string?> deviceReferences = items.Values
                    .Where(i => i is DeviceRef)
                    .Select(i => i as DeviceRef)
                    .Select(dr => dr!.GetName())
                    .Distinct()
                    .ToList();

                // Get all slot option names from the current machine
                List<string?> slotOptions = items.Values
                    .Where(i => i is Slot)
                    .Select(i => i as Slot)
                    .Where(s => s!.SlotOptionsSpecified)
                    .SelectMany(s => s!.GetFieldValue<SlotOption[]?>(Models.Metadata.Slot.SlotOptionKey)!)
                    .Select(so => so.GetStringFieldValue(Models.Metadata.SlotOption.DevNameKey))
                    .Distinct()
                    .ToList();

                // If we're checking device references
                if (deviceReferences.Count > 0)
                {
                    // Loop through all names and check the corresponding machines
                    var newDeviceReferences = new HashSet<string>();
                    foreach (string? deviceReference in deviceReferences)
                    {
                        // If the device reference is invalid
                        if (deviceReference == null)
                            continue;

                        // If the machine doesn't exist then we continue
                        var devItems = GetItemsForBucketDB(deviceReference);
                        if (devItems == null || devItems.Count == 0)
                            continue;

                        // Add to the list of new device reference names
                        newDeviceReferences.UnionWith(devItems.Values
                            .Where(i => i is DeviceRef)
                            .Select(i => (i as DeviceRef)!.GetName()!));

                        // Set new machine information and add to the current machine
                        var copyFrom = ItemsDB.GetMachineForItem(items.First().Key);
                        if (copyFrom.Value == null)
                            continue;

                        foreach (var item in devItems.Values)
                        {
                            // If the parent machine doesn't already contain this item, add it
                            if (!items.Values.Any(i => i.GetStringFieldValue(Models.Metadata.DatItem.TypeKey) == item.GetStringFieldValue(Models.Metadata.DatItem.TypeKey)
                                && i.GetName() == item.GetName()))
                            {
                                // Set that we found new items
                                foundnew = true;

                                // Clone the item and then add it
                                DatItem datItem = (item.Clone() as DatItem)!;
                                ItemsDB.AddItem(datItem, machine.Key, source.Key);
                            }
                        }
                    }

                    // Now that every device reference is accounted for, add the new list of device references, if they don't already exist
                    foreach (string deviceReference in newDeviceReferences)
                    {
                        if (!deviceReferences.Contains(deviceReference))
                        {
                            var deviceRef = new DeviceRef();
                            deviceRef.SetName(deviceReference);
                            ItemsDB.AddItem(deviceRef, machine.Key, source.Key);
                        }
                    }
                }

                // If we're checking slotoptions
                if (useSlotOptions && slotOptions.Count > 0)
                {
                    // Loop through all names and check the corresponding machines
                    var newSlotOptions = new HashSet<string>();
                    foreach (string? slotOption in slotOptions)
                    {
                        // If the slot option is invalid
                        if (slotOption == null)
                            continue;

                        // If the machine doesn't exist then we continue
                        var slotItems = GetItemsForBucketDB(slotOption);
                        if (slotItems == null || slotItems.Count == 0)
                            continue;

                        // Add to the list of new slot option names
                        newSlotOptions.UnionWith(slotItems.Values
                            .Where(i => i is Slot)
                            .Where(s => (s as Slot)!.SlotOptionsSpecified)
                            .SelectMany(s => (s as Slot)!.GetFieldValue<SlotOption[]?>(Models.Metadata.Slot.SlotOptionKey)!)
                            .Select(o => o.GetStringFieldValue(Models.Metadata.SlotOption.DevNameKey)!));

                        // Set new machine information and add to the current machine
                        var copyFrom = ItemsDB.GetMachineForItem(GetItemsForBucketDB(game)!.First().Key);
                        if (copyFrom.Value == null)
                            continue;

                        foreach (var item in slotItems.Values)
                        {
                            // If the parent machine doesn't already contain this item, add it
                            if (!items.Values.Any(i => i.GetStringFieldValue(Models.Metadata.DatItem.TypeKey) == item.GetStringFieldValue(Models.Metadata.DatItem.TypeKey)
                                && i.GetName() == item.GetName()))
                            {
                                // Set that we found new items
                                foundnew = true;

                                // Clone the item and then add it
                                DatItem datItem = (item.Clone() as DatItem)!;
                                ItemsDB.AddItem(datItem, machine.Key, source.Key);
                            }
                        }
                    }

                    // Now that every device is accounted for, add the new list of slot options, if they don't already exist
                    foreach (string slotOption in newSlotOptions)
                    {
                        if (!slotOptions.Contains(slotOption))
                        {
                            var slotOptionItem = new SlotOption();
                            slotOptionItem.SetFieldValue<string?>(Models.Metadata.SlotOption.DevNameKey, slotOption);

                            var slotItem = new Slot();
                            slotItem.SetFieldValue<SlotOption[]?>(Models.Metadata.Slot.SlotOptionKey, [slotOptionItem]);

                            ItemsDB.AddItem(slotItem, machine.Key, source.Key);
                        }
                    }
                }
            }

            return foundnew;
        }

        /// <summary>
        /// Use cloneof tags to add roms to the children, setting the new romof tag in the process
        /// </summary>
        private void AddRomsFromParentImpl()
        {
            List<string> games = [.. Items.Keys];
            games.Sort();

            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                var items = Items[game];
                if (items == null || items.Count == 0)
                    continue;

                // Get the machine
                var machine = items[0].GetFieldValue<Machine>(DatItem.MachineKey);
                if (machine == null)
                    continue;

                // Get the clone parent
                string? cloneOf = machine.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey);
                if (string.IsNullOrEmpty(cloneOf))
                    continue;

                // If the parent doesn't have any items, we want to continue
                var parentItems = Items[cloneOf!];
                if (parentItems == null || parentItems.Count == 0)
                    continue;

                // If the parent exists and has items, we copy the items from the parent to the current game
                DatItem copyFrom = items[0];
                foreach (DatItem item in parentItems)
                {
                    DatItem datItem = (DatItem)item.Clone();
                    datItem.CopyMachineInformation(copyFrom);
                    if (items.FindIndex(i => string.Equals(i.GetName(), datItem.GetName(), StringComparison.OrdinalIgnoreCase)) == -1
                        && !items.Contains(datItem))
                    {
                        Add(game, datItem);
                    }
                }

                // Now we want to get the parent romof tag and put it in each of the items
                items = Items[game];
                string? romof = Items[cloneOf!]![0].GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.RomOfKey);
                foreach (DatItem item in items!)
                {
                    item.GetFieldValue<Machine>(DatItem.MachineKey)!.SetFieldValue<string?>(Models.Metadata.Machine.RomOfKey, romof);
                }
            }
        }

        /// <summary>
        /// Use cloneof tags to add roms to the children, setting the new romof tag in the process
        /// </summary>
        private void AddRomsFromParentImplDB()
        {
            List<string> games = [.. ItemsDB.SortedKeys];
            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                var items = GetItemsForBucketDB(game);
                if (items == null || items.Count == 0)
                    continue;

                // Get the source for the first item
                var source = ItemsDB.GetSourceForItem(items.First().Key);

                // Get the machine for the first item in the list
                var machine = ItemsDB.GetMachineForItem(items.First().Key);
                if (machine.Value == null)
                    continue;

                // Get the clone parent
                string? cloneOf = machine.Value.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey);
                if (string.IsNullOrEmpty(cloneOf))
                    continue;

                // If the parent doesn't have any items, we want to continue
                var parentItems = GetItemsForBucketDB(cloneOf!);
                if (parentItems == null || parentItems.Count == 0)
                    continue;

                // If the parent exists and has items, we copy the items from the parent to the current game
                foreach (var item in parentItems)
                {
                    DatItem datItem = (DatItem)item.Value.Clone();
                    if (items.Values.Any(i => i.GetName()?.ToLowerInvariant() == datItem.GetName()?.ToLowerInvariant())
                        && items.Values.Any(i => i == datItem))
                    {
                        ItemsDB.AddItem(datItem, machine.Key, source.Key);
                    }
                }

                // Get the parent machine
                var parentMachine = ItemsDB.GetMachineForItem(GetItemsForBucketDB(cloneOf!)!.First().Key);
                if (parentMachine.Value == null)
                    continue;

                // Now we want to get the parent romof tag and put it in each of the items
                items = GetItemsForBucketDB(game);
                string? romof = parentMachine.Value.GetStringFieldValue(Models.Metadata.Machine.RomOfKey);
                foreach (var key in items.Keys)
                {
                    var itemMachine = ItemsDB.GetMachineForItem(key);
                    if (itemMachine.Value == null)
                        continue;

                    itemMachine.Value.SetFieldValue<string?>(Models.Metadata.Machine.RomOfKey, romof);
                }
            }
        }

        /// <summary>
        /// Remove all BIOS and device sets
        /// </summary>
        private void RemoveBiosAndDeviceSetsImpl()
        {
            List<string> games = [.. Items.Keys];
            games.Sort();

            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                var items = Items[game];
                if (items == null || items.Count == 0)
                    continue;

                // Get the machine
                var machine = items[0].GetFieldValue<Machine>(DatItem.MachineKey);
                if (machine == null)
                    continue;

                // Remove flagged items
                if ((machine.GetBoolFieldValue(Models.Metadata.Machine.IsBiosKey) == true)
                    || (machine.GetBoolFieldValue(Models.Metadata.Machine.IsDeviceKey) == true))
                {
                    Remove(game);
                }
            }
        }

        /// <summary>
        /// Remove all BIOS and device sets
        /// </summary>
        private void RemoveBiosAndDeviceSetsImplDB()
        {
            List<string> games = [.. ItemsDB.SortedKeys];
            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                var items = GetItemsForBucketDB(game);
                if (items == null || items.Count == 0)
                    continue;

                // Get the machine
                var machine = ItemsDB.GetMachineForItem(items.First().Key);
                if (machine.Value == null)
                    continue;

                // Remove flagged items
                if ((machine.Value.GetBoolFieldValue(Models.Metadata.Machine.IsBiosKey) == true)
                    || (machine.Value.GetBoolFieldValue(Models.Metadata.Machine.IsDeviceKey) == true))
                {
                    foreach (var key in items.Keys)
                    {
                        ItemsDB.RemoveItem(key);
                    }
                }
            }
        }

        /// <summary>
        /// Use romof tags to remove bios roms from children
        /// </summary>
        /// <param name="bios">True if only child Bios sets are touched, false for non-bios sets</param>
        private void RemoveBiosRomsFromChildImpl(bool bios)
        {
            // Loop through the romof tags
            List<string> games = [.. Items.Keys];
            games.Sort();

            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                var items = Items[game];
                if (items == null || items.Count == 0)
                    continue;

                // Get the machine
                var machine = items[0].GetFieldValue<Machine>(DatItem.MachineKey);
                if (machine == null)
                    continue;

                // If the game (is/is not) a bios, we want to continue
                if (bios ^ (machine.GetBoolFieldValue(Models.Metadata.Machine.IsBiosKey) == true))
                    continue;

                // Get the bios parent
                string? romOf = machine.GetStringFieldValue(Models.Metadata.Machine.RomOfKey);
                if (string.IsNullOrEmpty(romOf))
                    continue;

                // If the parent doesn't have any items, we want to continue
                var parentItems = Items[romOf!];
                if (parentItems == null || parentItems.Count == 0)
                    continue;

                // If the parent exists and has items, we remove the items that are in the parent from the current game
                foreach (DatItem item in parentItems)
                {
                    DatItem datItem = (DatItem)item.Clone();
                    while (items.Contains(datItem))
                    {
                        Items.Remove(game, datItem);
                    }
                }
            }
        }

        /// <summary>
        /// Use romof tags to remove bios roms from children
        /// </summary>
        /// <param name="bios">True if only child Bios sets are touched, false for non-bios sets</param>
        internal void RemoveBiosRomsFromChildImplDB(bool bios)
        {
            // Loop through the romof tags
            List<string> games = [.. ItemsDB.SortedKeys];
            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                var items = GetItemsForBucketDB(game);
                if (items == null || items.Count == 0)
                    continue;

                // Get the machine for the item
                var machine = ItemsDB.GetMachineForItem(items.First().Key);
                if (machine.Value == null)
                    continue;

                // If the game (is/is not) a bios, we want to continue
                if (bios ^ (machine.Value.GetBoolFieldValue(Models.Metadata.Machine.IsBiosKey) == true))
                    continue;

                // Get the bios parent
                string? romOf = machine.Value.GetStringFieldValue(Models.Metadata.Machine.RomOfKey);
                if (string.IsNullOrEmpty(romOf))
                    continue;

                // If the parent doesn't have any items, we want to continue
                var parentItems = GetItemsForBucketDB(romOf!);
                if (parentItems == null || parentItems.Count == 0)
                    continue;

                // If the parent exists and has items, we remove the items that are in the parent from the current game
                foreach (var item in parentItems)
                {
                    var matchedItems = items.Where(i => i.Value == item.Value);
                    foreach (var match in matchedItems)
                    {
                        ItemsDB.RemoveItem(match.Key);
                    }
                }
            }
        }

        /// <summary>
        /// Use cloneof tags to remove roms from the children
        /// </summary>
        private void RemoveRomsFromChildImpl()
        {
            List<string> games = [.. Items.Keys];
            games.Sort();

            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                var items = Items[game];
                if (items == null || items.Count == 0)
                    continue;

                // Get the machine
                var machine = items[0].GetFieldValue<Machine>(DatItem.MachineKey);
                if (machine == null)
                    continue;

                // Get the clone parent
                string? cloneOf = machine.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey);
                if (string.IsNullOrEmpty(cloneOf))
                    continue;

                // If the parent doesn't have any items, we want to continue
                var parentItems = Items[cloneOf!];
                if (parentItems == null || parentItems.Count == 0)
                    continue;

                // If the parent exists and has items, we remove the parent items from the current game
                foreach (DatItem item in parentItems!)
                {
                    DatItem datItem = (DatItem)item.Clone();
                    while (items.Contains(datItem))
                    {
                        Items.Remove(game, datItem);
                    }
                }

                // Now we want to get the parent romof tag and put it in each of the remaining items
                items = Items[game];
                string? romof = Items[cloneOf!]![0].GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.RomOfKey);
                foreach (DatItem item in items!)
                {
                    item.GetFieldValue<Machine>(DatItem.MachineKey)!.SetFieldValue<string?>(Models.Metadata.Machine.RomOfKey, romof);
                }
            }
        }

        /// <summary>
        /// Use cloneof tags to remove roms from the children
        /// </summary>
        internal void RemoveRomsFromChildImplDB()
        {
            List<string> games = [.. ItemsDB.SortedKeys];
            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                var items = GetItemsForBucketDB(game);
                if (items == null || items.Count == 0)
                    continue;

                // Get the machine for the first item
                var machine = ItemsDB.GetMachineForItem(items.First().Key);
                if (machine.Value == null)
                    continue;

                // Get the clone parent
                string? cloneOf = machine.Value.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey);
                if (string.IsNullOrEmpty(cloneOf))
                    continue;

                // If the parent doesn't have any items, we want to continue
                var parentItems = GetItemsForBucketDB(cloneOf!);
                if (parentItems == null || parentItems.Count == 0)
                    continue;

                // If the parent exists and has items, we remove the parent items from the current game
                foreach (var item in parentItems)
                {
                    var matchedItems = items.Where(i => i.Value == item.Value);
                    foreach (var match in matchedItems)
                    {
                        ItemsDB.RemoveItem(match.Key);
                    }
                }

                // Now we want to get the parent romof tag and put it in each of the remaining items
                items = GetItemsForBucketDB(game);
                machine = ItemsDB.GetMachineForItem(GetItemsForBucketDB(cloneOf!)!.First().Key);
                if (machine.Value == null)
                    continue;

                string? romof = machine.Value.GetStringFieldValue(Models.Metadata.Machine.RomOfKey);
                foreach (var item in items!)
                {
                    machine = ItemsDB.GetMachineForItem(item.Key);
                    if (machine.Value == null)
                        continue;

                    machine.Value.SetFieldValue<string?>(Models.Metadata.Machine.RomOfKey, romof);
                }
            }
        }

        /// <summary>
        /// Remove all romof and cloneof tags from all games
        /// </summary>
        /// <remarks>Applies to <see cref="Items"/></remarks>
        private void RemoveMachineRelationshipTagsImpl()
        {
            List<string> games = [.. Items.Keys];
            games.Sort();

            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                var items = Items[game];
                if (items == null || items.Count == 0)
                    continue;

                foreach (DatItem item in items)
                {
                    // Get the machine
                    var machine = item.GetFieldValue<Machine>(DatItem.MachineKey);
                    if (machine == null)
                        continue;
                
                    machine.SetFieldValue<string?>(Models.Metadata.Machine.CloneOfKey, null);
                    machine.SetFieldValue<string?>(Models.Metadata.Machine.RomOfKey, null);
                    machine.SetFieldValue<string?>(Models.Metadata.Machine.SampleOfKey, null);
                }
            }
        }

        /// <summary>
        /// Remove all romof and cloneof tags from all games
        /// </summary>
        /// <remarks>Applies to <see cref="ItemsDB"/></remarks>
        private void RemoveMachineRelationshipTagsImplDB()
        {
            var machines = ItemsDB.GetMachines();
            foreach (var machine in machines)
            {
                // Get the machine
                if (machine.Value == null)
                    continue;

                machine.Value.SetFieldValue<string?>(Models.Metadata.Machine.CloneOfKey, null);
                machine.Value.SetFieldValue<string?>(Models.Metadata.Machine.RomOfKey, null);
                machine.Value.SetFieldValue<string?>(Models.Metadata.Machine.SampleOfKey, null);
            }
        }

        #endregion
    }
}