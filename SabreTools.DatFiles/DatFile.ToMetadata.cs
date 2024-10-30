using System.Collections.Generic;
using System.Linq;
using SabreTools.Core.Tools;
using SabreTools.DatItems;

namespace SabreTools.DatFiles
{
    public partial class DatFile
    {
        #region To Metadata

        /// <summary>
        /// Convert metadata information
        /// </summary>
        public Models.Metadata.MetadataFile? ConvertMetadata(bool ignoreblanks = false)
        {
            // If we don't have items, we can't do anything
            if (Items == null || Items.Count == 0)
                return null;

            // Create an object to hold the data
            var metadataFile = new Models.Metadata.MetadataFile();

            // Convert and assign the header
            var header = ConvertHeader();
            if (header != null)
                metadataFile[Models.Metadata.MetadataFile.HeaderKey] = header;

            // Convert and assign the machines
            var machines = ConvertMachines(ignoreblanks);
            if (machines != null)
                metadataFile[Models.Metadata.MetadataFile.MachineKey] = machines;

            return metadataFile;
        }

        /// <summary>
        /// Convert metadata information
        /// </summary>
        public Models.Metadata.MetadataFile? ConvertMetadataDB(bool ignoreblanks = false)
        {
            // If we don't have items, we can't do anything
            if (ItemsDB == null)
                return null;

            // Create an object to hold the data
            var metadataFile = new Models.Metadata.MetadataFile();

            // Convert and assign the header
            var header = ConvertHeader();
            if (header != null)
                metadataFile[Models.Metadata.MetadataFile.HeaderKey] = header;

            // Convert and assign the machines
            var machines = ConvertMachinesDB(ignoreblanks);
            if (machines != null)
                metadataFile[Models.Metadata.MetadataFile.MachineKey] = machines;

            return metadataFile;
        }

        /// <summary>
        /// Convert header information
        /// </summary>
        private Models.Metadata.Header? ConvertHeader()
        {
            // If the header is invalid, we can't do anything
            if (Header == null)
                return null;

            // Create an internal header
            var header = Header.GetInternalClone();

            // Remove fields with default values
            if (header.Read<MergingFlag>(Models.Metadata.Header.ForceMergingKey) == MergingFlag.None)
                header.Remove(Models.Metadata.Header.ForceMergingKey);
            if (header.Read<NodumpFlag>(Models.Metadata.Header.ForceNodumpKey) == NodumpFlag.None)
                header.Remove(Models.Metadata.Header.ForceNodumpKey);
            if (header.Read<PackingFlag>(Models.Metadata.Header.ForcePackingKey) == PackingFlag.None)
                header.Remove(Models.Metadata.Header.ForcePackingKey);

            // Convert subheader values
            if (Header.CanOpenSpecified)
                header[Models.Metadata.Header.CanOpenKey] = new Models.OfflineList.CanOpen { Extension = Header.GetStringArrayFieldValue(Models.Metadata.Header.CanOpenKey) };
            if (Header.ImagesSpecified)
                header[Models.Metadata.Header.ImagesKey] = Header.GetFieldValue<Models.OfflineList.Images>(Models.Metadata.Header.ImagesKey);
            if (Header.InfosSpecified)
                header[Models.Metadata.Header.InfosKey] = Header.GetFieldValue<Models.OfflineList.Infos>(Models.Metadata.Header.InfosKey);
            if (Header.NewDatSpecified)
            {
                var newDat = new Models.OfflineList.NewDat
                {
                    DatVersionUrl = Header.GetStringFieldValue("DATVERSIONURL"),
                    DatUrl = Header.GetFieldValue<Models.OfflineList.DatUrl?>("DATURL"),
                    ImUrl = Header.GetStringFieldValue("IMURL"),
                };
                header[Models.Metadata.Header.NewDatKey] = newDat;
            }
            if (Header.SearchSpecified)
                header[Models.Metadata.Header.SearchKey] = Header.GetFieldValue<Models.OfflineList.Search>(Models.Metadata.Header.SearchKey);

            return header;
        }

        /// <summary>
        /// Convert machines information
        /// </summary>
        private Models.Metadata.Machine[]? ConvertMachines(bool ignoreblanks = false)
        {
            // Create a machine list to hold all outputs
            var machines = new List<Models.Metadata.Machine>();

            // Loop through the sorted items and create games for them
            foreach (string key in Items.SortedKeys)
            {
                var items = Items.FilteredItems(key);
                if (items == null || items.Count == 0)
                    continue;

                // Create a machine to hold everything
                var machine = items[0].GetFieldValue<DatItems.Machine>(DatItems.DatItem.MachineKey)!.GetInternalClone();

                // Handle Trurip object, if it exists
                if (machine.ContainsKey(Models.Metadata.Machine.TruripKey))
                {
                    var trurip = machine.Read<DatItems.Trurip>(Models.Metadata.Machine.TruripKey);
                    if (trurip != null)
                    {
                        var truripItem = trurip.ConvertToLogiqx();
                        truripItem.Publisher = machine.ReadString(Models.Metadata.Machine.PublisherKey);
                        truripItem.Year = machine.ReadString(Models.Metadata.Machine.YearKey);
                        truripItem.Players = machine.ReadString(Models.Metadata.Machine.PlayersKey);
                        truripItem.Source = machine.ReadString(Models.Metadata.Machine.SourceFileKey);
                        truripItem.CloneOf = machine.ReadString(Models.Metadata.Machine.CloneOfKey);
                        machine[Models.Metadata.Machine.TruripKey] = truripItem;
                    }
                }

                // Create mapping dictionaries for the Parts, DataAreas, and DiskAreas associated with this machine
                Dictionary<Models.Metadata.Part, Models.Metadata.DatItem> partMappings = [];
                Dictionary<Models.Metadata.Part, (Models.Metadata.DataArea, Models.Metadata.Rom)> dataAreaMappings = [];
                Dictionary<Models.Metadata.Part, (Models.Metadata.DiskArea, Models.Metadata.Disk)> diskAreaMappings = [];

                // Loop through and convert the items to respective lists
                for (int index = 0; index < items.Count; index++)
                {
                    // Get the item
                    var item = items[index];

                    // Check for a "null" item
                    item = ProcessNullifiedItem(item);

                    // Skip if we're ignoring the item
                    if (ShouldIgnore(item, ignoreblanks))
                        continue;

                    switch (item)
                    {
                        case DatItems.Formats.Adjuster adjuster:
                            var adjusterItem = ProcessItem(adjuster);
                            EnsureMachineKey<Models.Metadata.Adjuster?>(machine, Models.Metadata.Machine.AdjusterKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.AdjusterKey, adjusterItem);
                            break;
                        case DatItems.Formats.Archive archive:
                            var archiveItem = archive.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.Archive?>(machine, Models.Metadata.Machine.ArchiveKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.ArchiveKey, archiveItem);
                            break;
                        case DatItems.Formats.BiosSet biosSet:
                            var biosSetItem = biosSet.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.BiosSet?>(machine, Models.Metadata.Machine.BiosSetKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.BiosSetKey, biosSetItem);
                            break;
                        case DatItems.Formats.Chip chip:
                            var chipItem = chip.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.Chip?>(machine, Models.Metadata.Machine.ChipKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.ChipKey, chipItem);
                            break;
                        case DatItems.Formats.Configuration configuration:
                            var configurationItem = ProcessItem(configuration);
                            EnsureMachineKey<Models.Metadata.Configuration?>(machine, Models.Metadata.Machine.ConfigurationKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.ConfigurationKey, configurationItem);
                            break;
                        case DatItems.Formats.Device device:
                            var deviceItem = ProcessItem(device);
                            EnsureMachineKey<Models.Metadata.Device?>(machine, Models.Metadata.Machine.DeviceKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.DeviceKey, deviceItem);
                            break;
                        case DatItems.Formats.DeviceRef deviceRef:
                            var deviceRefItem = deviceRef.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.DeviceRef?>(machine, Models.Metadata.Machine.DeviceRefKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.DeviceRefKey, deviceRefItem);
                            break;
                        case DatItems.Formats.DipSwitch dipSwitch:
                            var dipSwitchItem = ProcessItem(dipSwitch);
                            EnsureMachineKey<Models.Metadata.DipSwitch?>(machine, Models.Metadata.Machine.DipSwitchKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.DipSwitchKey, dipSwitchItem);

                            // Add Part mapping
                            bool dipSwitchContainsPart = dipSwitchItem.ContainsKey(DatItems.Formats.DipSwitch.PartKey);
                            if (dipSwitchContainsPart)
                            {
                                var partItem = dipSwitchItem.Read<DatItems.Formats.Part>(DatItems.Formats.DipSwitch.PartKey);
                                if (partItem != null)
                                    partMappings[partItem.GetInternalClone()] = dipSwitchItem;
                            }

                            break;
                        case DatItems.Formats.Disk disk:
                            var diskItem = disk.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.Disk?>(machine, Models.Metadata.Machine.DiskKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.DiskKey, diskItem);

                            // Add Part and DiskArea mappings
                            bool diskContainsPart = diskItem.ContainsKey(DatItems.Formats.Disk.PartKey);
                            bool diskContainsDiskArea = diskItem.ContainsKey(DatItems.Formats.Disk.DiskAreaKey);
                            if (diskContainsPart && diskContainsDiskArea)
                            {
                                var partItem = diskItem.Read<DatItems.Formats.Part>(DatItems.Formats.Disk.PartKey);
                                if (partItem != null)
                                {
                                    var partItemInternal = partItem.GetInternalClone();
                                    partMappings[partItemInternal] = diskItem;

                                    var diskAreaItem = diskItem.Read<DatItems.Formats.DiskArea>(DatItems.Formats.Disk.DiskAreaKey);
                                    if (diskAreaItem != null)
                                        diskAreaMappings[partItemInternal] = (diskAreaItem.GetInternalClone(), diskItem);
                                }
                            }
                            break;
                        case DatItems.Formats.Display display:
                            var displayItem = ProcessItem(display, machine);
                            EnsureMachineKey<Models.Metadata.Display?>(machine, Models.Metadata.Machine.DisplayKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.DisplayKey, displayItem);
                            break;
                        case DatItems.Formats.Driver driver:
                            var driverItem = driver.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.Driver?>(machine, Models.Metadata.Machine.DriverKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.DriverKey, driverItem);
                            break;
                        case DatItems.Formats.Feature feature:
                            var featureItem = feature.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.Feature?>(machine, Models.Metadata.Machine.FeatureKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.FeatureKey, featureItem);
                            break;
                        case DatItems.Formats.Info info:
                            var infoItem = info.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.Info?>(machine, Models.Metadata.Machine.InfoKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.InfoKey, infoItem);
                            break;
                        case DatItems.Formats.Input input:
                            var inputItem = ProcessItem(input);
                            EnsureMachineKey<Models.Metadata.Input?>(machine, Models.Metadata.Machine.InputKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.InputKey, inputItem);
                            break;
                        case DatItems.Formats.Media media:
                            var mediaItem = media.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.Media?>(machine, Models.Metadata.Machine.MediaKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.MediaKey, mediaItem);
                            break;
                        case DatItems.Formats.PartFeature partFeature:
                            var partFeatureItem = partFeature.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.Feature?>(machine, Models.Metadata.Machine.FeatureKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.FeatureKey, partFeatureItem);

                            // Add Part mapping
                            bool partFeatureContainsPart = partFeatureItem.ContainsKey(DatItems.Formats.PartFeature.PartKey);
                            if (partFeatureContainsPart)
                            {
                                var partItem = partFeatureItem.Read<DatItems.Formats.Part>(DatItems.Formats.PartFeature.PartKey);
                                if (partItem != null)
                                    partMappings[partItem.GetInternalClone()] = partFeatureItem;
                            }
                            break;
                        case DatItems.Formats.Port port:
                            var portItem = ProcessItem(port);
                            EnsureMachineKey<Models.Metadata.Port?>(machine, Models.Metadata.Machine.PortKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.PortKey, portItem);
                            break;
                        case DatItems.Formats.RamOption ramOption:
                            var ramOptionItem = ramOption.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.RamOption?>(machine, Models.Metadata.Machine.RamOptionKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.RamOptionKey, ramOptionItem);
                            break;
                        case DatItems.Formats.Release release:
                            var releaseItem = release.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.Release?>(machine, Models.Metadata.Machine.ReleaseKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.ReleaseKey, releaseItem);
                            break;
                        case DatItems.Formats.Rom rom:
                            var romItem = ProcessItem(rom, machine);
                            EnsureMachineKey<Models.Metadata.Rom?>(machine, Models.Metadata.Machine.RomKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.RomKey, romItem);

                            // Add Part and DataArea mappings
                            bool romContainsPart = romItem.ContainsKey(DatItems.Formats.Rom.PartKey);
                            bool romContainsDataArea = romItem.ContainsKey(DatItems.Formats.Rom.DataAreaKey);
                            if (romContainsPart && romContainsDataArea)
                            {
                                var partItem = romItem.Read<DatItems.Formats.Part>(DatItems.Formats.Rom.PartKey);
                                if (partItem != null)
                                {
                                    var partItemInternal = partItem.GetInternalClone();
                                    partMappings[partItemInternal] = romItem;

                                    var dataAreaItem = romItem.Read<DatItems.Formats.DataArea>(DatItems.Formats.Rom.DataAreaKey);
                                    if (dataAreaItem != null)
                                        dataAreaMappings[partItemInternal] = (dataAreaItem.GetInternalClone(), romItem);
                                }
                            }
                            break;
                        case DatItems.Formats.Sample sample:
                            var sampleItem = sample.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.Sample?>(machine, Models.Metadata.Machine.SampleKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.SampleKey, sampleItem);
                            break;
                        case DatItems.Formats.SharedFeat sharedFeat:
                            var sharedFeatItem = sharedFeat.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.SharedFeat?>(machine, Models.Metadata.Machine.SharedFeatKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.SharedFeatKey, sharedFeatItem);
                            break;
                        case DatItems.Formats.Slot slot:
                            var slotItem = ProcessItem(slot);
                            EnsureMachineKey<Models.Metadata.Slot?>(machine, Models.Metadata.Machine.SlotKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.SlotKey, slotItem);
                            break;
                        case DatItems.Formats.SoftwareList softwareList:
                            var softwareListItem = softwareList.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.SoftwareList?>(machine, Models.Metadata.Machine.SoftwareListKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.SoftwareListKey, softwareListItem);
                            break;
                        case DatItems.Formats.Sound sound:
                            var soundItem = sound.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.Sound?>(machine, Models.Metadata.Machine.SoundKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.SoundKey, soundItem);
                            break;
                    }
                }

                // Handle Part, DiskItem, and DatItem mappings, if they exist
                if (partMappings.Count != 0)
                {
                    // Create a collection to hold the inverted Parts
                    Dictionary<string, Models.Metadata.Part> partItems = [];

                    // Loop through the Part mappings
                    foreach (var partMapping in partMappings)
                    {
                        // Get the mapping pieces
                        var partItem = partMapping.Key;
                        var datItem = partMapping.Value;

                        // Get the part name and skip if there's none
                        string? partName = partItem.ReadString(Models.Metadata.Part.NameKey);
                        if (partName == null)
                            continue;

                        // Create the part in the dictionary, if needed
                        if (!partItems.ContainsKey(partName))
                            partItems[partName] = [];

                        // Copy over string values
                        partItems[partName][Models.Metadata.Part.NameKey] = partName;
                        if (!partItems[partName].ContainsKey(Models.Metadata.Part.InterfaceKey))
                            partItems[partName][Models.Metadata.Part.InterfaceKey] = partItem.ReadString(Models.Metadata.Part.InterfaceKey);

                        // Clear any empty fields
                        ClearEmptyKeys(partItems[partName]);

                        // If the item has a DataArea mapping
                        if (dataAreaMappings.ContainsKey(partItem))
                        {
                            // Get the mapped items
                            var (dataArea, romItem) = dataAreaMappings[partItem];

                            // Clear any empty fields
                            ClearEmptyKeys(romItem);

                            // Get the data area name and skip if there's none
                            string? dataAreaName = dataArea.ReadString(Models.Metadata.DataArea.NameKey);
                            if (dataAreaName != null)
                            {
                                // Get existing data areas as a list
                                var dataAreasArr = partItems[partName].Read<Models.Metadata.DataArea[]>(Models.Metadata.Part.DataAreaKey) ?? [];
                                var dataAreas = new List<Models.Metadata.DataArea>(dataAreasArr);

                                // Find the existing disk area to append to, otherwise create a new disk area
                                int dataAreaIndex = dataAreas.FindIndex(da => da.ReadString(Models.Metadata.DataArea.NameKey) == dataAreaName);
                                Models.Metadata.DataArea aggregateDataArea;
                                if (dataAreaIndex > -1)
                                {
                                    aggregateDataArea = dataAreas[dataAreaIndex];
                                }
                                else
                                {
                                    aggregateDataArea = [];
                                    aggregateDataArea[Models.Metadata.DataArea.EndiannessKey] = dataArea.ReadString(Models.Metadata.DataArea.EndiannessKey);
                                    aggregateDataArea[Models.Metadata.DataArea.NameKey] = dataArea.ReadString(Models.Metadata.DataArea.NameKey);
                                    aggregateDataArea[Models.Metadata.DataArea.SizeKey] = dataArea.ReadString(Models.Metadata.DataArea.SizeKey);
                                    aggregateDataArea[Models.Metadata.DataArea.WidthKey] = dataArea.ReadString(Models.Metadata.DataArea.WidthKey);
                                }

                                // Clear any empty fields
                                ClearEmptyKeys(aggregateDataArea);

                                // Get existing roms as a list
                                var romsArr = aggregateDataArea.Read<Models.Metadata.Rom[]>(Models.Metadata.DataArea.RomKey) ?? [];
                                var roms = new List<Models.Metadata.Rom>(romsArr);

                                // Add the rom to the data area
                                roms.Add(romItem);

                                // Assign back the roms
                                aggregateDataArea[Models.Metadata.DataArea.RomKey] = roms.ToArray();

                                // Assign back the data area
                                if (dataAreaIndex > -1)
                                    dataAreas[dataAreaIndex] = aggregateDataArea;
                                else
                                    dataAreas.Add(aggregateDataArea);

                                // Assign back the data areas array
                                partItems[partName][Models.Metadata.Part.DataAreaKey] = dataAreas.ToArray();
                            }
                        }

                        // If the item has a DiskArea mapping
                        if (diskAreaMappings.ContainsKey(partItem))
                        {
                            // Get the mapped items
                            var (diskArea, diskItem) = diskAreaMappings[partItem];

                            // Clear any empty fields
                            ClearEmptyKeys(diskItem);

                            // Get the disk area name and skip if there's none
                            string? diskAreaName = diskArea.ReadString(Models.Metadata.DiskArea.NameKey);
                            if (diskAreaName != null)
                            {
                                // Get existing disk areas as a list
                                var diskAreasArr = partItems[partName].Read<Models.Metadata.DiskArea[]>(Models.Metadata.Part.DiskAreaKey) ?? [];
                                var diskAreas = new List<Models.Metadata.DiskArea>(diskAreasArr);

                                // Find the existing disk area to append to, otherwise create a new disk area
                                int diskAreaIndex = diskAreas.FindIndex(da => da.ReadString(Models.Metadata.DiskArea.NameKey) == diskAreaName);
                                Models.Metadata.DiskArea aggregateDiskArea;
                                if (diskAreaIndex > -1)
                                {
                                    aggregateDiskArea = diskAreas[diskAreaIndex];
                                }
                                else
                                {
                                    aggregateDiskArea = [];
                                    aggregateDiskArea[Models.Metadata.DiskArea.NameKey] = diskArea.ReadString(Models.Metadata.DiskArea.NameKey);
                                }

                                // Clear any empty fields
                                ClearEmptyKeys(aggregateDiskArea);

                                // Get existing disks as a list
                                var disksArr = aggregateDiskArea.Read<Models.Metadata.Disk[]>(Models.Metadata.DiskArea.DiskKey) ?? [];
                                var disks = new List<Models.Metadata.Disk>(disksArr);

                                // Add the disk to the data area
                                disks.Add(diskItem);

                                // Assign back the disks
                                aggregateDiskArea[Models.Metadata.DiskArea.DiskKey] = disks.ToArray();

                                // Assign back the disk area
                                if (diskAreaIndex > -1)
                                    diskAreas[diskAreaIndex] = aggregateDiskArea;
                                else
                                    diskAreas.Add(aggregateDiskArea);

                                // Assign back the disk areas array
                                partItems[partName][Models.Metadata.Part.DiskAreaKey] = diskAreas.ToArray();
                            }
                        }

                        // If the item is a DipSwitch
                        if (datItem is Models.Metadata.DipSwitch dipSwitchItem)
                        {
                            // Get existing dipswitches as a list
                            var dipSwitchesArr = partItems[partName].Read<Models.Metadata.DipSwitch[]>(Models.Metadata.Part.DipSwitchKey) ?? [];
                            var dipSwitches = new List<Models.Metadata.DipSwitch>(dipSwitchesArr);

                            // Clear any empty fields
                            ClearEmptyKeys(dipSwitchItem);

                            // Add the dipswitch
                            dipSwitches.Add(dipSwitchItem);

                            // Assign back the dipswitches
                            partItems[partName][Models.Metadata.Part.DipSwitchKey] = dipSwitches.ToArray();
                        }

                        // If the item is a Feature
                        else if (datItem is Models.Metadata.Feature featureItem)
                        {
                            // Get existing features as a list
                            var featuresArr = partItems[partName].Read<Models.Metadata.Feature[]>(Models.Metadata.Part.FeatureKey) ?? [];
                            var features = new List<Models.Metadata.Feature>(featuresArr);

                            // Clear any empty fields
                            ClearEmptyKeys(featureItem);

                            // Add the feature
                            features.Add(featureItem);

                            // Assign back the features
                            partItems[partName][Models.Metadata.Part.FeatureKey] = features.ToArray();
                        }
                    }

                    // Assign the part array to the machine
                    machine[Models.Metadata.Machine.PartKey] = partItems.Values.ToArray();
                }

                // Add the machine to the list
                machines.Add(machine);
            }

            // Return the list of machines
            return [.. machines];
        }

        /// <summary>
        /// Convert machines information
        /// </summary>
        private Models.Metadata.Machine[]? ConvertMachinesDB(bool ignoreblanks = false)
        {
            // Create a machine list to hold all outputs
            var machines = new List<Models.Metadata.Machine>();

            // Loop through the sorted items and create games for them
            foreach (string key in ItemsDB.SortedKeys)
            {
                var items = ItemsDB.GetItemsForBucket(key, filter: true);
                if (items == null || items.Length == 0)
                    continue;

                // Create a machine to hold everything
                var machine = ItemsDB.GetMachineForItem(items[0].Item1).Item2!.GetInternalClone();

                // Handle Trurip object, if it exists
                if (machine.ContainsKey(Models.Metadata.Machine.TruripKey))
                {
                    var trurip = machine.Read<DatItems.Trurip>(Models.Metadata.Machine.TruripKey);
                    if (trurip != null)
                    {
                        var truripItem = trurip.ConvertToLogiqx();
                        truripItem.Publisher = machine.ReadString(Models.Metadata.Machine.PublisherKey);
                        truripItem.Year = machine.ReadString(Models.Metadata.Machine.YearKey);
                        truripItem.Players = machine.ReadString(Models.Metadata.Machine.PlayersKey);
                        truripItem.Source = machine.ReadString(Models.Metadata.Machine.SourceFileKey);
                        truripItem.CloneOf = machine.ReadString(Models.Metadata.Machine.CloneOfKey);
                        machine[Models.Metadata.Machine.TruripKey] = truripItem;
                    }
                }

                // Create mapping dictionaries for the Parts, DataAreas, and DiskAreas associated with this machine
                Dictionary<Models.Metadata.Part, Models.Metadata.DatItem> partMappings = [];
                Dictionary<Models.Metadata.Part, (Models.Metadata.DataArea, Models.Metadata.Rom)> dataAreaMappings = [];
                Dictionary<Models.Metadata.Part, (Models.Metadata.DiskArea, Models.Metadata.Disk)> diskAreaMappings = [];

                // Loop through and convert the items to respective lists
                for (int index = 0; index < items.Length; index++)
                {
                    // Get the item
                    var item = items[index];

                    // Check for a "null" item
                    item = ProcessNullifiedItem(item);

                    // Skip if we're ignoring the item
                    if (ShouldIgnore(item, ignoreblanks))
                        continue;

                    switch (item.Item2)
                    {
                        case DatItems.Formats.Adjuster adjuster:
                            var adjusterItem = ProcessItem(adjuster);
                            EnsureMachineKey<Models.Metadata.Adjuster?>(machine, Models.Metadata.Machine.AdjusterKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.AdjusterKey, adjusterItem);
                            break;
                        case DatItems.Formats.Archive archive:
                            var archiveItem = archive.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.Archive?>(machine, Models.Metadata.Machine.ArchiveKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.ArchiveKey, archiveItem);
                            break;
                        case DatItems.Formats.BiosSet biosSet:
                            var biosSetItem = biosSet.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.BiosSet?>(machine, Models.Metadata.Machine.BiosSetKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.BiosSetKey, biosSetItem);
                            break;
                        case DatItems.Formats.Chip chip:
                            var chipItem = chip.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.Chip?>(machine, Models.Metadata.Machine.ChipKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.ChipKey, chipItem);
                            break;
                        case DatItems.Formats.Configuration configuration:
                            var configurationItem = ProcessItem(configuration);
                            EnsureMachineKey<Models.Metadata.Configuration?>(machine, Models.Metadata.Machine.ConfigurationKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.ConfigurationKey, configurationItem);
                            break;
                        case DatItems.Formats.Device device:
                            var deviceItem = ProcessItem(device);
                            EnsureMachineKey<Models.Metadata.Device?>(machine, Models.Metadata.Machine.DeviceKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.DeviceKey, deviceItem);
                            break;
                        case DatItems.Formats.DeviceRef deviceRef:
                            var deviceRefItem = deviceRef.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.DeviceRef?>(machine, Models.Metadata.Machine.DeviceRefKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.DeviceRefKey, deviceRefItem);
                            break;
                        case DatItems.Formats.DipSwitch dipSwitch:
                            var dipSwitchItem = ProcessItem(dipSwitch);
                            EnsureMachineKey<Models.Metadata.DipSwitch?>(machine, Models.Metadata.Machine.DipSwitchKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.DipSwitchKey, dipSwitchItem);

                            // Add Part mapping
                            bool dipSwitchContainsPart = dipSwitchItem.ContainsKey(DatItems.Formats.DipSwitch.PartKey);
                            if (dipSwitchContainsPart)
                            {
                                var partItem = dipSwitchItem.Read<DatItems.Formats.Part>(DatItems.Formats.DipSwitch.PartKey);
                                if (partItem != null)
                                    partMappings[partItem.GetInternalClone()] = dipSwitchItem;
                            }

                            break;
                        case DatItems.Formats.Disk disk:
                            var diskItem = disk.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.Disk?>(machine, Models.Metadata.Machine.DiskKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.DiskKey, diskItem);

                            // Add Part and DiskArea mappings
                            bool diskContainsPart = diskItem.ContainsKey(DatItems.Formats.Disk.PartKey);
                            bool diskContainsDiskArea = diskItem.ContainsKey(DatItems.Formats.Disk.DiskAreaKey);
                            if (diskContainsPart && diskContainsDiskArea)
                            {
                                var partItem = diskItem.Read<DatItems.Formats.Part>(DatItems.Formats.Disk.PartKey);
                                if (partItem != null)
                                {
                                    var partItemInternal = partItem.GetInternalClone();
                                    partMappings[partItemInternal] = diskItem;

                                    var diskAreaItem = diskItem.Read<DatItems.Formats.DiskArea>(DatItems.Formats.Disk.DiskAreaKey);
                                    if (diskAreaItem != null)
                                        diskAreaMappings[partItemInternal] = (diskAreaItem.GetInternalClone(), diskItem);
                                }
                            }
                            break;
                        case DatItems.Formats.Display display:
                            var displayItem = ProcessItem(display, machine);
                            EnsureMachineKey<Models.Metadata.Display?>(machine, Models.Metadata.Machine.DisplayKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.DisplayKey, displayItem);
                            break;
                        case DatItems.Formats.Driver driver:
                            var driverItem = driver.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.Driver?>(machine, Models.Metadata.Machine.DriverKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.DriverKey, driverItem);
                            break;
                        case DatItems.Formats.Feature feature:
                            var featureItem = feature.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.Feature?>(machine, Models.Metadata.Machine.FeatureKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.FeatureKey, featureItem);
                            break;
                        case DatItems.Formats.Info info:
                            var infoItem = info.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.Info?>(machine, Models.Metadata.Machine.InfoKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.InfoKey, infoItem);
                            break;
                        case DatItems.Formats.Input input:
                            var inputItem = ProcessItem(input);
                            EnsureMachineKey<Models.Metadata.Input?>(machine, Models.Metadata.Machine.InputKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.InputKey, inputItem);
                            break;
                        case DatItems.Formats.Media media:
                            var mediaItem = media.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.Media?>(machine, Models.Metadata.Machine.MediaKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.MediaKey, mediaItem);
                            break;
                        case DatItems.Formats.PartFeature partFeature:
                            var partFeatureItem = partFeature.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.Feature?>(machine, Models.Metadata.Machine.FeatureKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.FeatureKey, partFeatureItem);

                            // Add Part mapping
                            bool partFeatureContainsPart = partFeatureItem.ContainsKey(DatItems.Formats.PartFeature.PartKey);
                            if (partFeatureContainsPart)
                            {
                                var partItem = partFeatureItem.Read<DatItems.Formats.Part>(DatItems.Formats.PartFeature.PartKey);
                                if (partItem != null)
                                    partMappings[partItem.GetInternalClone()] = partFeatureItem;
                            }
                            break;
                        case DatItems.Formats.Port port:
                            var portItem = ProcessItem(port);
                            EnsureMachineKey<Models.Metadata.Port?>(machine, Models.Metadata.Machine.PortKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.PortKey, portItem);
                            break;
                        case DatItems.Formats.RamOption ramOption:
                            var ramOptionItem = ramOption.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.RamOption?>(machine, Models.Metadata.Machine.RamOptionKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.RamOptionKey, ramOptionItem);
                            break;
                        case DatItems.Formats.Release release:
                            var releaseItem = release.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.Release?>(machine, Models.Metadata.Machine.ReleaseKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.ReleaseKey, releaseItem);
                            break;
                        case DatItems.Formats.Rom rom:
                            var romItem = ProcessItem(rom, machine);
                            EnsureMachineKey<Models.Metadata.Rom?>(machine, Models.Metadata.Machine.RomKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.RomKey, romItem);

                            // Add Part and DataArea mappings
                            bool romContainsPart = romItem.ContainsKey(DatItems.Formats.Rom.PartKey);
                            bool romContainsDataArea = romItem.ContainsKey(DatItems.Formats.Rom.DataAreaKey);
                            if (romContainsPart && romContainsDataArea)
                            {
                                var partItem = romItem.Read<DatItems.Formats.Part>(DatItems.Formats.Rom.PartKey);
                                if (partItem != null)
                                {
                                    var partItemInternal = partItem.GetInternalClone();
                                    partMappings[partItemInternal] = romItem;

                                    var dataAreaItem = romItem.Read<DatItems.Formats.DataArea>(DatItems.Formats.Rom.DataAreaKey);
                                    if (dataAreaItem != null)
                                        dataAreaMappings[partItemInternal] = (dataAreaItem.GetInternalClone(), romItem);
                                }
                            }
                            break;
                        case DatItems.Formats.Sample sample:
                            var sampleItem = sample.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.Sample?>(machine, Models.Metadata.Machine.SampleKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.SampleKey, sampleItem);
                            break;
                        case DatItems.Formats.SharedFeat sharedFeat:
                            var sharedFeatItem = sharedFeat.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.SharedFeat?>(machine, Models.Metadata.Machine.SharedFeatKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.SharedFeatKey, sharedFeatItem);
                            break;
                        case DatItems.Formats.Slot slot:
                            var slotItem = ProcessItem(slot);
                            EnsureMachineKey<Models.Metadata.Slot?>(machine, Models.Metadata.Machine.SlotKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.SlotKey, slotItem);
                            break;
                        case DatItems.Formats.SoftwareList softwareList:
                            var softwareListItem = softwareList.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.SoftwareList?>(machine, Models.Metadata.Machine.SoftwareListKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.SoftwareListKey, softwareListItem);
                            break;
                        case DatItems.Formats.Sound sound:
                            var soundItem = sound.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.Sound?>(machine, Models.Metadata.Machine.SoundKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.SoundKey, soundItem);
                            break;
                    }
                }

                // Handle Part, DiskItem, and DatItem mappings, if they exist
                if (partMappings.Count != 0)
                {
                    // Create a collection to hold the inverted Parts
                    Dictionary<string, Models.Metadata.Part> partItems = [];

                    // Loop through the Part mappings
                    foreach (var partMapping in partMappings)
                    {
                        // Get the mapping pieces
                        var partItem = partMapping.Key;
                        var datItem = partMapping.Value;

                        // Get the part name and skip if there's none
                        string? partName = partItem.ReadString(Models.Metadata.Part.NameKey);
                        if (partName == null)
                            continue;

                        // Create the part in the dictionary, if needed
                        if (!partItems.ContainsKey(partName))
                            partItems[partName] = [];

                        // Copy over string values
                        partItems[partName][Models.Metadata.Part.NameKey] = partName;
                        if (!partItems[partName].ContainsKey(Models.Metadata.Part.InterfaceKey))
                            partItems[partName][Models.Metadata.Part.InterfaceKey] = partItem.ReadString(Models.Metadata.Part.InterfaceKey);

                        // Clear any empty fields
                        ClearEmptyKeys(partItems[partName]);

                        // If the item has a DataArea mapping
                        if (dataAreaMappings.ContainsKey(partItem))
                        {
                            // Get the mapped items
                            var (dataArea, romItem) = dataAreaMappings[partItem];

                            // Clear any empty fields
                            ClearEmptyKeys(romItem);

                            // Get the data area name and skip if there's none
                            string? dataAreaName = dataArea.ReadString(Models.Metadata.DataArea.NameKey);
                            if (dataAreaName != null)
                            {
                                // Get existing data areas as a list
                                var dataAreasArr = partItems[partName].Read<Models.Metadata.DataArea[]>(Models.Metadata.Part.DataAreaKey) ?? [];
                                var dataAreas = new List<Models.Metadata.DataArea>(dataAreasArr);

                                // Find the existing disk area to append to, otherwise create a new disk area
                                int dataAreaIndex = dataAreas.FindIndex(da => da.ReadString(Models.Metadata.DataArea.NameKey) == dataAreaName);
                                Models.Metadata.DataArea aggregateDataArea;
                                if (dataAreaIndex > -1)
                                {
                                    aggregateDataArea = dataAreas[dataAreaIndex];
                                }
                                else
                                {
                                    aggregateDataArea = [];
                                    aggregateDataArea[Models.Metadata.DataArea.EndiannessKey] = dataArea.ReadString(Models.Metadata.DataArea.EndiannessKey);
                                    aggregateDataArea[Models.Metadata.DataArea.NameKey] = dataArea.ReadString(Models.Metadata.DataArea.NameKey);
                                    aggregateDataArea[Models.Metadata.DataArea.SizeKey] = dataArea.ReadString(Models.Metadata.DataArea.SizeKey);
                                    aggregateDataArea[Models.Metadata.DataArea.WidthKey] = dataArea.ReadString(Models.Metadata.DataArea.WidthKey);
                                }

                                // Clear any empty fields
                                ClearEmptyKeys(aggregateDataArea);

                                // Get existing roms as a list
                                var romsArr = aggregateDataArea.Read<Models.Metadata.Rom[]>(Models.Metadata.DataArea.RomKey) ?? [];
                                var roms = new List<Models.Metadata.Rom>(romsArr);

                                // Add the rom to the data area
                                roms.Add(romItem);

                                // Assign back the roms
                                aggregateDataArea[Models.Metadata.DataArea.RomKey] = roms.ToArray();

                                // Assign back the data area
                                if (dataAreaIndex > -1)
                                    dataAreas[dataAreaIndex] = aggregateDataArea;
                                else
                                    dataAreas.Add(aggregateDataArea);

                                // Assign back the data areas array
                                partItems[partName][Models.Metadata.Part.DataAreaKey] = dataAreas.ToArray();
                            }
                        }

                        // If the item has a DiskArea mapping
                        if (diskAreaMappings.ContainsKey(partItem))
                        {
                            // Get the mapped items
                            var (diskArea, diskItem) = diskAreaMappings[partItem];

                            // Clear any empty fields
                            ClearEmptyKeys(diskItem);

                            // Get the disk area name and skip if there's none
                            string? diskAreaName = diskArea.ReadString(Models.Metadata.DiskArea.NameKey);
                            if (diskAreaName != null)
                            {
                                // Get existing disk areas as a list
                                var diskAreasArr = partItems[partName].Read<Models.Metadata.DiskArea[]>(Models.Metadata.Part.DiskAreaKey) ?? [];
                                var diskAreas = new List<Models.Metadata.DiskArea>(diskAreasArr);

                                // Find the existing disk area to append to, otherwise create a new disk area
                                int diskAreaIndex = diskAreas.FindIndex(da => da.ReadString(Models.Metadata.DiskArea.NameKey) == diskAreaName);
                                Models.Metadata.DiskArea aggregateDiskArea;
                                if (diskAreaIndex > -1)
                                {
                                    aggregateDiskArea = diskAreas[diskAreaIndex];
                                }
                                else
                                {
                                    aggregateDiskArea = [];
                                    aggregateDiskArea[Models.Metadata.DiskArea.NameKey] = diskArea.ReadString(Models.Metadata.DiskArea.NameKey);
                                }

                                // Clear any empty fields
                                ClearEmptyKeys(aggregateDiskArea);

                                // Get existing disks as a list
                                var disksArr = aggregateDiskArea.Read<Models.Metadata.Disk[]>(Models.Metadata.DiskArea.DiskKey) ?? [];
                                var disks = new List<Models.Metadata.Disk>(disksArr);

                                // Add the disk to the data area
                                disks.Add(diskItem);

                                // Assign back the disks
                                aggregateDiskArea[Models.Metadata.DiskArea.DiskKey] = disks.ToArray();

                                // Assign back the disk area
                                if (diskAreaIndex > -1)
                                    diskAreas[diskAreaIndex] = aggregateDiskArea;
                                else
                                    diskAreas.Add(aggregateDiskArea);

                                // Assign back the disk areas array
                                partItems[partName][Models.Metadata.Part.DiskAreaKey] = diskAreas.ToArray();
                            }
                        }

                        // If the item is a DipSwitch
                        if (datItem is Models.Metadata.DipSwitch dipSwitchItem)
                        {
                            // Get existing dipswitches as a list
                            var dipSwitchesArr = partItems[partName].Read<Models.Metadata.DipSwitch[]>(Models.Metadata.Part.DipSwitchKey) ?? [];
                            var dipSwitches = new List<Models.Metadata.DipSwitch>(dipSwitchesArr);

                            // Clear any empty fields
                            ClearEmptyKeys(dipSwitchItem);

                            // Add the dipswitch
                            dipSwitches.Add(dipSwitchItem);

                            // Assign back the dipswitches
                            partItems[partName][Models.Metadata.Part.DipSwitchKey] = dipSwitches.ToArray();
                        }

                        // If the item is a Feature
                        else if (datItem is Models.Metadata.Feature featureItem)
                        {
                            // Get existing features as a list
                            var featuresArr = partItems[partName].Read<Models.Metadata.Feature[]>(Models.Metadata.Part.FeatureKey) ?? [];
                            var features = new List<Models.Metadata.Feature>(featuresArr);

                            // Clear any empty fields
                            ClearEmptyKeys(featureItem);

                            // Add the feature
                            features.Add(featureItem);

                            // Assign back the features
                            partItems[partName][Models.Metadata.Part.FeatureKey] = features.ToArray();
                        }
                    }

                    // Assign the part array to the machine
                    machine[Models.Metadata.Machine.PartKey] = partItems.Values.ToArray();
                }

                // Add the machine to the list
                machines.Add(machine);
            }

            // Return the list of machines
            return [.. machines];
        }

        /// <summary>
        /// Convert Adjuster information
        /// </summary>
        /// <param name="item">Item to convert</param>
        private static Models.Metadata.Adjuster ProcessItem(DatItems.Formats.Adjuster item)
        {
            var adjusterItem = item.GetInternalClone();

            var condition = item.GetFieldValue<DatItems.Formats.Condition?>(Models.Metadata.Adjuster.ConditionKey);
            if (condition != null)
                adjusterItem[Models.Metadata.Adjuster.ConditionKey] = condition.GetInternalClone();

            return adjusterItem;
        }

        /// <summary>
        /// Convert Configuration information
        /// </summary>
        /// <param name="item">Item to convert</param>
        private static Models.Metadata.Configuration ProcessItem(DatItems.Formats.Configuration item)
        {
            var configurationItem = item.GetInternalClone();

            var condition = item.GetFieldValue<DatItems.Formats.Condition?>(Models.Metadata.Configuration.ConditionKey);
            if (condition != null)
                configurationItem[Models.Metadata.Configuration.ConditionKey] = condition.GetInternalClone();

            var confLocations = item.GetFieldValue<DatItems.Formats.ConfLocation[]?>(Models.Metadata.Configuration.ConfLocationKey);
            if (confLocations != null)
            {
                var confLocationItems = new List<Models.Metadata.ConfLocation>();
                foreach (var confLocation in confLocations)
                {
                    var confLocationItem = confLocation.GetInternalClone();
                    confLocationItems.Add(confLocationItem);
                }

                configurationItem[Models.Metadata.Configuration.ConfLocationKey] = confLocationItems.ToArray();
            }

            var confSettings = item.GetFieldValue<DatItems.Formats.ConfSetting[]?>(Models.Metadata.Configuration.ConfSettingKey);
            if (confSettings != null)
            {
                var confSettingItems = new List<Models.Metadata.ConfSetting>();
                foreach (var confSetting in confSettings)
                {
                    var confSettingItem = confSetting.GetInternalClone();
                    confSettingItems.Add(confSettingItem);
                }

                configurationItem[Models.Metadata.Configuration.ConfSettingKey] = confSettingItems.ToArray();
            }

            return configurationItem;
        }

        /// <summary>
        /// Convert Device information
        /// </summary>
        /// <param name="item">Item to convert</param>
        private static Models.Metadata.Device ProcessItem(DatItems.Formats.Device item)
        {
            var deviceItem = item.GetInternalClone();

            var instance = item.GetFieldValue<DatItems.Formats.Instance?>(Models.Metadata.Device.InstanceKey);
            if (instance != null)
                deviceItem[Models.Metadata.Device.InstanceKey] = instance.GetInternalClone();

            var extensions = item.GetFieldValue<DatItems.Formats.Extension[]?>(Models.Metadata.Device.ExtensionKey);
            if (extensions != null)
            {
                var extensionItems = new List<Models.Metadata.Extension>();
                foreach (var extension in extensions)
                {
                    var extensionItem = extension.GetInternalClone();
                    extensionItems.Add(extensionItem);
                }

                deviceItem[Models.Metadata.Device.ExtensionKey] = extensionItems.ToArray();
            }

            return deviceItem;
        }

        /// <summary>
        /// Convert DipSwitch information
        /// </summary>
        /// <param name="item">Item to convert</param>
        /// <param name="machine">Machine to use for Part</param>
        private static Models.Metadata.DipSwitch ProcessItem(DatItems.Formats.DipSwitch item)
        {
            var dipSwitchItem = item.GetInternalClone();

            var condition = item.GetFieldValue<DatItems.Formats.Condition?>(Models.Metadata.DipSwitch.ConditionKey);
            if (condition != null)
                dipSwitchItem[Models.Metadata.DipSwitch.ConditionKey] = condition.GetInternalClone();

            var dipLocations = item.GetFieldValue<DatItems.Formats.DipLocation[]?>(Models.Metadata.DipSwitch.DipLocationKey);
            if (dipLocations != null)
            {
                var dipLocationItems = new List<Models.Metadata.DipLocation>();
                foreach (var dipLocation in dipLocations)
                {
                    var extensionItem = dipLocation.GetInternalClone();
                    dipLocationItems.Add(extensionItem);
                }

                dipSwitchItem[Models.Metadata.DipSwitch.DipLocationKey] = dipLocationItems.ToArray();
            }

            var dipValues = item.GetFieldValue<DatItems.Formats.DipValue[]?>(Models.Metadata.DipSwitch.DipValueKey);
            if (dipValues != null)
            {
                var dipValueItems = new List<Models.Metadata.DipValue>();
                foreach (var dipValue in dipValues)
                {
                    var extensionItem = dipValue.GetInternalClone();
                    dipValueItems.Add(extensionItem);
                }

                dipSwitchItem[Models.Metadata.DipSwitch.DipValueKey] = dipValueItems.ToArray();
            }

            return dipSwitchItem;
        }

        /// <summary>
        /// Convert Display information
        /// </summary>
        /// <param name="item">Item to convert</param>
        /// <param name="machine">Machine to use for Video</param>
        private static Models.Metadata.Display ProcessItem(DatItems.Formats.Display item, Models.Metadata.Machine machine)
        {
            var displayItem = item.GetInternalClone();

            // Create a Video for any item that has specific fields
            if (displayItem.ContainsKey(Models.Metadata.Video.AspectXKey))
            {
                var videoItem = new Models.Metadata.Video();
                videoItem[Models.Metadata.Video.AspectXKey] = displayItem.ReadLong(Models.Metadata.Video.AspectXKey).ToString();
                videoItem[Models.Metadata.Video.AspectYKey] = displayItem.ReadLong(Models.Metadata.Video.AspectYKey).ToString();
                videoItem[Models.Metadata.Video.HeightKey] = displayItem.ReadLong(Models.Metadata.Display.HeightKey).ToString();
                videoItem[Models.Metadata.Video.RefreshKey] = displayItem.ReadDouble(Models.Metadata.Display.RefreshKey).ToString();
                videoItem[Models.Metadata.Video.ScreenKey] = displayItem.ReadString(Models.Metadata.Display.DisplayTypeKey).AsEnumValue<DisplayType>().AsStringValue();
                videoItem[Models.Metadata.Video.WidthKey] = displayItem.ReadLong(Models.Metadata.Display.WidthKey).ToString();

                EnsureMachineKey<Models.Metadata.Video?>(machine, Models.Metadata.Machine.VideoKey);
                AppendToMachineKey(machine, Models.Metadata.Machine.VideoKey, videoItem);
            }

            return displayItem;
        }

        /// <summary>
        /// Convert Input information
        /// </summary>
        /// <param name="item">Item to convert</param>
        private static Models.Metadata.Input ProcessItem(DatItems.Formats.Input item)
        {
            var inputItem = item.GetInternalClone();

            var controls = item.GetFieldValue<DatItems.Formats.Control[]?>(Models.Metadata.Input.ControlKey);
            if (controls != null)
            {
                var controlItems = new List<Models.Metadata.Control>();
                foreach (var control in controls)
                {
                    var controlItem = control.GetInternalClone();
                    controlItems.Add(controlItem);
                }

                inputItem[Models.Metadata.Input.ControlKey] = controlItems.ToArray();
            }

            return inputItem;
        }

        /// <summary>
        /// Convert Port information
        /// </summary>
        /// <param name="item">Item to convert</param>
        private static Models.Metadata.Port ProcessItem(DatItems.Formats.Port item)
        {
            var slotItem = item.GetInternalClone();

            var analogs = item.GetFieldValue<DatItems.Formats.Analog[]?>(Models.Metadata.Port.AnalogKey);
            if (analogs != null)
            {
                var analogItems = new List<Models.Metadata.Analog>();
                foreach (var analog in analogs)
                {
                    var extensionItem = analog.GetInternalClone();
                    analogItems.Add(extensionItem);
                }

                slotItem[Models.Metadata.Port.AnalogKey] = analogItems.ToArray();
            }

            return slotItem;
        }

        /// <summary>
        /// Convert Rom information
        /// </summary>
        /// <param name="item">Item to convert</param>
        /// <param name="machine">Machine to use for Part and DataArea</param>
        private static Models.Metadata.Rom ProcessItem(DatItems.Formats.Rom item, Models.Metadata.Machine machine)
        {
            var romItem = item.GetInternalClone();

            // Create a Dump for every Rom that has a subtype
            switch (romItem.ReadString(Models.Metadata.Rom.OpenMSXMediaType).AsEnumValue<OpenMSXSubType>())
            {
                case OpenMSXSubType.Rom:
                    var dumpRom = new Models.Metadata.Dump();
                    var rom = new Models.Metadata.Rom();

                    rom[Models.Metadata.Rom.NameKey] = romItem.ReadString(Models.Metadata.Rom.NameKey);
                    rom[Models.Metadata.Rom.OffsetKey] = romItem.ReadString(Models.Metadata.Rom.StartKey) ?? romItem.ReadString(Models.Metadata.Rom.OffsetKey);
                    rom[Models.Metadata.Rom.OpenMSXType] = romItem.ReadString(Models.Metadata.Rom.OpenMSXType);
                    rom[Models.Metadata.Rom.RemarkKey] = romItem.ReadString(Models.Metadata.Rom.RemarkKey);
                    rom[Models.Metadata.Rom.SHA1Key] = romItem.ReadString(Models.Metadata.Rom.SHA1Key);
                    rom[Models.Metadata.Rom.StartKey] = romItem.ReadString(Models.Metadata.Rom.StartKey) ?? romItem.ReadString(Models.Metadata.Rom.OffsetKey);

                    dumpRom[Models.Metadata.Dump.RomKey] = rom;

                    var romOriginal = romItem.Read<DatItems.Formats.Original>("ORIGINAL");
                    if (romOriginal != null)
                    {
                        var newOriginal = new Models.Metadata.Original
                        {
                            [Models.Metadata.Original.ValueKey] = romOriginal.Value.FromYesNo(),
                            [Models.Metadata.Original.ContentKey] = romOriginal.Content,
                        };
                        dumpRom[Models.Metadata.Dump.OriginalKey] = newOriginal;
                    }

                    EnsureMachineKey<Models.Metadata.Dump?>(machine, Models.Metadata.Machine.DumpKey);
                    AppendToMachineKey(machine, Models.Metadata.Machine.DumpKey, dumpRom);
                    break;

                case OpenMSXSubType.MegaRom:
                    var dumpMegaRom = new Models.Metadata.Dump();
                    var megaRom = new Models.Metadata.Rom();

                    megaRom[Models.Metadata.Rom.NameKey] = romItem.ReadString(Models.Metadata.Rom.NameKey);
                    megaRom[Models.Metadata.Rom.OffsetKey] = romItem.ReadString(Models.Metadata.Rom.StartKey) ?? romItem.ReadString(Models.Metadata.Rom.OffsetKey);
                    megaRom[Models.Metadata.Rom.OpenMSXType] = romItem.ReadString(Models.Metadata.Rom.OpenMSXType);
                    megaRom[Models.Metadata.Rom.RemarkKey] = romItem.ReadString(Models.Metadata.Rom.RemarkKey);
                    megaRom[Models.Metadata.Rom.SHA1Key] = romItem.ReadString(Models.Metadata.Rom.SHA1Key);
                    megaRom[Models.Metadata.Rom.StartKey] = romItem.ReadString(Models.Metadata.Rom.StartKey) ?? romItem.ReadString(Models.Metadata.Rom.OffsetKey);

                    dumpMegaRom[Models.Metadata.Dump.MegaRomKey] = megaRom;

                    var megaRomOriginal = romItem.Read<DatItems.Formats.Original>("ORIGINAL");
                    if (megaRomOriginal != null)
                    {
                        var newOriginal = new Models.Metadata.Original
                        {
                            [Models.Metadata.Original.ValueKey] = megaRomOriginal.Value.FromYesNo(),
                            [Models.Metadata.Original.ContentKey] = megaRomOriginal.Content,
                        };
                        dumpMegaRom[Models.Metadata.Dump.OriginalKey] = newOriginal;
                    }

                    EnsureMachineKey<Models.Metadata.Dump?>(machine, Models.Metadata.Machine.DumpKey);
                    AppendToMachineKey(machine, Models.Metadata.Machine.DumpKey, dumpMegaRom);
                    break;

                case OpenMSXSubType.SCCPlusCart:
                    var dumpSccPlusCart = new Models.Metadata.Dump();
                    var sccPlusCart = new Models.Metadata.Rom();

                    sccPlusCart[Models.Metadata.Rom.NameKey] = romItem.ReadString(Models.Metadata.Rom.NameKey);
                    sccPlusCart[Models.Metadata.Rom.OffsetKey] = romItem.ReadString(Models.Metadata.Rom.StartKey) ?? romItem.ReadString(Models.Metadata.Rom.OffsetKey);
                    sccPlusCart[Models.Metadata.Rom.OpenMSXType] = romItem.ReadString(Models.Metadata.Rom.OpenMSXType);
                    sccPlusCart[Models.Metadata.Rom.RemarkKey] = romItem.ReadString(Models.Metadata.Rom.RemarkKey);
                    sccPlusCart[Models.Metadata.Rom.SHA1Key] = romItem.ReadString(Models.Metadata.Rom.SHA1Key);
                    sccPlusCart[Models.Metadata.Rom.StartKey] = romItem.ReadString(Models.Metadata.Rom.StartKey) ?? romItem.ReadString(Models.Metadata.Rom.OffsetKey);

                    dumpSccPlusCart[Models.Metadata.Dump.RomKey] = sccPlusCart;

                    var sccPlusCartOriginal = romItem.Read<DatItems.Formats.Original>("ORIGINAL");
                    if (sccPlusCartOriginal != null)
                    {
                        var newOriginal = new Models.Metadata.Original
                        {
                            [Models.Metadata.Original.ValueKey] = sccPlusCartOriginal.Value.FromYesNo(),
                            [Models.Metadata.Original.ContentKey] = sccPlusCartOriginal.Content,
                        };
                        dumpSccPlusCart[Models.Metadata.Dump.OriginalKey] = newOriginal;
                    }

                    EnsureMachineKey<Models.Metadata.Dump?>(machine, Models.Metadata.Machine.DumpKey);
                    AppendToMachineKey(machine, Models.Metadata.Machine.DumpKey, dumpSccPlusCart);
                    break;
            }

            return romItem;
        }

        /// <summary>
        /// Convert Slot information
        /// </summary>
        /// <param name="item">Item to convert</param>
        private static Models.Metadata.Slot ProcessItem(DatItems.Formats.Slot item)
        {
            var slotItem = item.GetInternalClone();

            var slotOptions = item.GetFieldValue<DatItems.Formats.SlotOption[]?>(Models.Metadata.Slot.SlotOptionKey);
            if (slotOptions != null)
            {
                var slotOptionItems = new List<Models.Metadata.SlotOption>();
                foreach (var slotOption in slotOptions)
                {
                    var extensionItem = slotOption.GetInternalClone();
                    slotOptionItems.Add(extensionItem);
                }

                slotItem[Models.Metadata.Slot.SlotOptionKey] = slotOptionItems.ToArray();
            }

            return slotItem;
        }

        /// <summary>
        /// Ensure a key in a machine
        /// </summary>
        private static void EnsureMachineKey<T>(Models.Metadata.Machine machine, string key)
        {
            if (machine.Read<T[]?>(key) == null)
#if NET20 || NET35 || NET40 || NET452
                machine[key] = new T[0];
#else
                machine[key] = System.Array.Empty<T>();
#endif
        }

        /// <summary>
        /// Append to a machine key as if its an array
        /// </summary>
        private static void AppendToMachineKey<T>(Models.Metadata.Machine machine, string key, T value) where T : Models.Metadata.DatItem
        {
            // Get the existing array
            var arr = machine.Read<T[]>(key);
            if (arr == null)
                return;

            // Trim all null fields
            ClearEmptyKeys(value);

            // Add to the array
            List<T> list = [.. arr];
            list.Add(value);
            machine[key] = list.ToArray();
        }

        /// <summary>
        /// Clear empty keys from a DictionaryBase object
        /// </summary>
        private static void ClearEmptyKeys(Models.Metadata.DictionaryBase obj)
        {
            string[] fieldNames = [.. obj.Keys];
            foreach (string fieldName in fieldNames)
            {
                if (obj[fieldName] == null)
                    obj.Remove(fieldName);
            }
        }

        #endregion
    }
}