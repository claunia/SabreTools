using System;
using System.Collections.Generic;
using System.Linq;

using SabreTools.Core;
using SabreTools.DatFiles;
using SabreTools.DatItems;
using SabreTools.Logging;

namespace SabreTools.Filtering
{
    public class Splitter
    {
        #region Fields

        /// <summary>
        /// Splitting mode to apply
        /// </summary>
        public MergingFlag SplitType { get; set; }

        #endregion

        #region Logging

        /// <summary>
        /// Logging object
        /// </summary>
        private static readonly Logger logger = new Logger();

        #endregion
    
        // TODO: Should any of these create a new DatFile in the process?
        // The reason this comes up is that doing any of the splits or merges
        // is an inherently destructive process. Making it output a new DatFile
        // might make it easier to deal with multiple internal steps. On the other
        // hand, this will increase memory usage significantly and would force the
        // existing paths to behave entirely differently
        #region Running

        /// <summary>
        /// Apply splitting on the DatFile
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        /// <param name="useTags">True if DatFile tags override splitting, false otherwise</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        /// <returns>True if the DatFile was split, false on error</returns>
        public bool ApplySplitting(DatFile datFile, bool useTags, bool throwOnError = false)
        {
            try
            {
                // If we are using tags from the DAT, set the proper input for split type unless overridden
                if (useTags && SplitType == MergingFlag.None)
                    SplitType = datFile.Header.ForceMerging;

                // Run internal splitting
                switch (SplitType)
                {
                    case MergingFlag.None:
                        // No-op
                        break;
                    case MergingFlag.Device:
                        CreateDeviceNonMergedSets(datFile);
                        break;
                    case MergingFlag.Full:
                        CreateFullyNonMergedSets(datFile);
                        break;
                    case MergingFlag.NonMerged:
                        CreateNonMergedSets(datFile);
                        break;
                    case MergingFlag.Merged:
                        CreateMergedSets(datFile);
                        break;
                    case MergingFlag.Split:
                        CreateSplitSets(datFile);
                        break;
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
        /// Use cdevice_ref tags to get full non-merged sets and remove parenting tags
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        internal static void CreateDeviceNonMergedSets(DatFile datFile)
        {
            logger.User("Creating device non-merged sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            datFile.Items.BucketBy(ItemKey.Machine, DedupeType.None, norename: true);

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
        internal static void CreateFullyNonMergedSets(DatFile datFile)
        {
            logger.User("Creating fully non-merged sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            datFile.Items.BucketBy(ItemKey.Machine, DedupeType.None, norename: true);

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
        internal static void CreateMergedSets(DatFile datFile)
        {
            logger.User("Creating merged sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            datFile.Items.BucketBy(ItemKey.Machine, DedupeType.None, norename: true);

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
        internal static void CreateNonMergedSets(DatFile datFile)
        {
            logger.User("Creating non-merged sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            datFile.Items.BucketBy(ItemKey.Machine, DedupeType.None, norename: true);

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
        internal static void CreateSplitSets(DatFile datFile)
        {
            logger.User("Creating split sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            datFile.Items.BucketBy(ItemKey.Machine, DedupeType.None, norename: true);

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
        internal static void AddRomsFromBios(DatFile datFile)
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
        internal static bool AddRomsFromDevices(DatFile datFile, bool dev = false, bool useSlotOptions = false)
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
        internal static void AddRomsFromParent(DatFile datFile)
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
        internal static void AddRomsFromChildren(DatFile datFile, bool subfolder = true)
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
                            item.SetName($"{item.Machine.Name}\\{item.GetName()}");

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
        internal static void RemoveBiosAndDeviceSets(DatFile datFile)
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
        internal static void RemoveBiosRomsFromChild(DatFile datFile, bool bios = false)
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
        internal static void RemoveRomsFromChild(DatFile datFile)
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
        internal static void RemoveTagsFromChild(DatFile datFile)
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
