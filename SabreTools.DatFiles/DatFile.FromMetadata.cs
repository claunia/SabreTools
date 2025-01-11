using System.Collections.Generic;
using SabreTools.Core.Tools;
using SabreTools.DatItems;

namespace SabreTools.DatFiles
{
    public partial class DatFile
    {
        #region From Metadata

        /// <summary>
        /// Convert metadata information
        /// </summary>
        /// <param name="item">Metadata file to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        internal void ConvertFromMetadata(Models.Metadata.MetadataFile? item, string filename, int indexId, bool keep, bool statsOnly)
        {
            // If the metadata file is invalid, we can't do anything
            if (item == null || item.Count == 0)
                return;

            // Create an internal source and add to the dictionary
            var source = new DatItems.Source(indexId, filename);
            long sourceIndex = ItemsDB.AddSource(source);

            // Get the header from the metadata
            var header = item.Read<Models.Metadata.Header>(Models.Metadata.MetadataFile.HeaderKey);
            if (header != null)
                ConvertHeader(header, keep);

            // Get the machines from the metadata
            var machines = ReadItemArray<Models.Metadata.Machine>(item, Models.Metadata.MetadataFile.MachineKey);
            if (machines != null)
                ConvertMachines(machines, source, sourceIndex, statsOnly);
        }

        /// <summary>
        /// Convert header information
        /// </summary>
        /// <param name="item">Header to convert</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise</param>
        private void ConvertHeader(Models.Metadata.Header? item, bool keep)
        {
            // If the header is invalid, we can't do anything
            if (item == null || item.Count == 0)
                return;

            // Create an internal header
            var header = new DatHeader(item);

            // Convert subheader values
            if (item.ContainsKey(Models.Metadata.Header.CanOpenKey))
            {
                var canOpen = item.Read<Models.OfflineList.CanOpen>(Models.Metadata.Header.CanOpenKey);
                if (canOpen?.Extension != null)
                    Header.SetFieldValue<string[]?>(Models.Metadata.Header.CanOpenKey, canOpen.Extension);
            }
            if (item.ContainsKey(Models.Metadata.Header.ImagesKey))
            {
                var images = item.Read<Models.OfflineList.Images>(Models.Metadata.Header.ImagesKey);
                Header.SetFieldValue<Models.OfflineList.Images?>(Models.Metadata.Header.ImagesKey, images);
            }
            if (item.ContainsKey(Models.Metadata.Header.InfosKey))
            {
                var infos = item.Read<Models.OfflineList.Infos>(Models.Metadata.Header.InfosKey);
                Header.SetFieldValue<Models.OfflineList.Infos?>(Models.Metadata.Header.InfosKey, infos);
            }
            if (item.ContainsKey(Models.Metadata.Header.NewDatKey))
            {
                var newDat = item.Read<Models.OfflineList.NewDat>(Models.Metadata.Header.NewDatKey);
                Header.SetFieldValue<Models.OfflineList.NewDat?>(Models.Metadata.Header.NewDatKey, newDat);
            }
            if (item.ContainsKey(Models.Metadata.Header.SearchKey))
            {
                var search = item.Read<Models.OfflineList.Search>(Models.Metadata.Header.SearchKey);
                Header.SetFieldValue<Models.OfflineList.Search?>(Models.Metadata.Header.SearchKey, search);
            }

            // Selectively set all possible fields -- TODO: Figure out how to make this less manual
            if (Header.GetStringFieldValue(Models.Metadata.Header.AuthorKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.AuthorKey, header.GetStringFieldValue(Models.Metadata.Header.AuthorKey));
            if (Header.GetStringFieldValue(Models.Metadata.Header.BiosModeKey).AsEnumValue<MergingFlag>() == MergingFlag.None)
                Header.SetFieldValue<string?>(Models.Metadata.Header.BiosModeKey, header.GetStringFieldValue(Models.Metadata.Header.BiosModeKey).AsEnumValue<MergingFlag>().AsStringValue());
            if (Header.GetStringFieldValue(Models.Metadata.Header.BuildKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.BuildKey, header.GetStringFieldValue(Models.Metadata.Header.BuildKey));
            if (Header.GetStringFieldValue(Models.Metadata.Header.CategoryKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.CategoryKey, header.GetStringFieldValue(Models.Metadata.Header.CategoryKey));
            if (Header.GetStringFieldValue(Models.Metadata.Header.CommentKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.CommentKey, header.GetStringFieldValue(Models.Metadata.Header.CommentKey));
            if (Header.GetStringFieldValue(Models.Metadata.Header.DateKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.DateKey, header.GetStringFieldValue(Models.Metadata.Header.DateKey));
            if (Header.GetStringFieldValue(Models.Metadata.Header.DatVersionKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.DatVersionKey, header.GetStringFieldValue(Models.Metadata.Header.DatVersionKey));
            if (Header.GetBoolFieldValue(Models.Metadata.Header.DebugKey) == null)
                Header.SetFieldValue<bool?>(Models.Metadata.Header.DebugKey, header.GetBoolFieldValue(Models.Metadata.Header.DebugKey));
            if (Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey));
            if (Header.GetStringFieldValue(Models.Metadata.Header.EmailKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.EmailKey, header.GetStringFieldValue(Models.Metadata.Header.EmailKey));
            if (Header.GetStringFieldValue(Models.Metadata.Header.EmulatorVersionKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.EmulatorVersionKey, header.GetStringFieldValue(Models.Metadata.Header.EmulatorVersionKey));
            if (Header.GetStringFieldValue(Models.Metadata.Header.ForceMergingKey).AsEnumValue<MergingFlag>() == MergingFlag.None)
                Header.SetFieldValue<string?>(Models.Metadata.Header.ForceMergingKey, header.GetStringFieldValue(Models.Metadata.Header.ForceMergingKey).AsEnumValue<MergingFlag>().AsStringValue());
            if (Header.GetStringFieldValue(Models.Metadata.Header.ForceNodumpKey).AsEnumValue<NodumpFlag>() == NodumpFlag.None)
                Header.SetFieldValue<string?>(Models.Metadata.Header.ForceNodumpKey, header.GetStringFieldValue(Models.Metadata.Header.ForceNodumpKey).AsEnumValue<NodumpFlag>().AsStringValue());
            if (Header.GetStringFieldValue(Models.Metadata.Header.ForcePackingKey).AsEnumValue<PackingFlag>() == PackingFlag.None)
                Header.SetFieldValue<string?>(Models.Metadata.Header.ForcePackingKey, header.GetStringFieldValue(Models.Metadata.Header.ForcePackingKey).AsEnumValue<PackingFlag>().AsStringValue());
            if (Header.GetBoolFieldValue(Models.Metadata.Header.ForceZippingKey) == null)
                Header.SetFieldValue<bool?>(Models.Metadata.Header.ForceZippingKey, header.GetBoolFieldValue(Models.Metadata.Header.ForceZippingKey));
            if (Header.GetStringFieldValue(Models.Metadata.Header.HeaderKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.HeaderKey, header.GetStringFieldValue(Models.Metadata.Header.HeaderKey));
            if (Header.GetStringFieldValue(Models.Metadata.Header.HomepageKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.HomepageKey, header.GetStringFieldValue(Models.Metadata.Header.HomepageKey));
            if (Header.GetStringFieldValue(Models.Metadata.Header.IdKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.IdKey, header.GetStringFieldValue(Models.Metadata.Header.IdKey));
            if (Header.GetStringFieldValue(Models.Metadata.Header.ImFolderKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.ImFolderKey, header.GetStringFieldValue(Models.Metadata.Header.ImFolderKey));
            if (Header.GetBoolFieldValue(Models.Metadata.Header.LockBiosModeKey) == null)
                Header.SetFieldValue<bool?>(Models.Metadata.Header.LockBiosModeKey, header.GetBoolFieldValue(Models.Metadata.Header.LockBiosModeKey));
            if (Header.GetBoolFieldValue(Models.Metadata.Header.LockRomModeKey) == null)
                Header.SetFieldValue<bool?>(Models.Metadata.Header.LockRomModeKey, header.GetBoolFieldValue(Models.Metadata.Header.LockRomModeKey));
            if (Header.GetBoolFieldValue(Models.Metadata.Header.LockSampleModeKey) == null)
                Header.SetFieldValue<bool?>(Models.Metadata.Header.LockSampleModeKey, header.GetBoolFieldValue(Models.Metadata.Header.LockSampleModeKey));
            if (Header.GetStringFieldValue(Models.Metadata.Header.MameConfigKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.MameConfigKey, header.GetStringFieldValue(Models.Metadata.Header.MameConfigKey));
            if (Header.GetStringFieldValue(Models.Metadata.Header.NameKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, header.GetStringFieldValue(Models.Metadata.Header.NameKey));
            if (Header.GetStringFieldValue(Models.Metadata.Header.NotesKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.NotesKey, header.GetStringFieldValue(Models.Metadata.Header.NotesKey));
            if (Header.GetStringFieldValue(Models.Metadata.Header.PluginKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.PluginKey, header.GetStringFieldValue(Models.Metadata.Header.PluginKey));
            if (Header.GetStringFieldValue(Models.Metadata.Header.RefNameKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.RefNameKey, header.GetStringFieldValue(Models.Metadata.Header.RefNameKey));
            if (Header.GetStringFieldValue(Models.Metadata.Header.RomModeKey).AsEnumValue<MergingFlag>() == MergingFlag.None)
                Header.SetFieldValue<string?>(Models.Metadata.Header.RomModeKey, header.GetStringFieldValue(Models.Metadata.Header.RomModeKey).AsEnumValue<MergingFlag>().AsStringValue());
            if (Header.GetStringFieldValue(Models.Metadata.Header.RomTitleKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.RomTitleKey, header.GetStringFieldValue(Models.Metadata.Header.RomTitleKey));
            if (Header.GetStringFieldValue(Models.Metadata.Header.RootDirKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.RootDirKey, header.GetStringFieldValue(Models.Metadata.Header.RootDirKey));
            if (Header.GetStringFieldValue(Models.Metadata.Header.SampleModeKey).AsEnumValue<MergingFlag>() == MergingFlag.None)
                Header.SetFieldValue<string?>(Models.Metadata.Header.SampleModeKey, header.GetStringFieldValue(Models.Metadata.Header.SampleModeKey).AsEnumValue<MergingFlag>().AsStringValue());
            if (Header.GetStringFieldValue(Models.Metadata.Header.SchemaLocationKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.SchemaLocationKey, header.GetStringFieldValue(Models.Metadata.Header.SchemaLocationKey));
            if (Header.GetStringFieldValue(Models.Metadata.Header.ScreenshotsHeightKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.ScreenshotsHeightKey, header.GetStringFieldValue(Models.Metadata.Header.ScreenshotsHeightKey));
            if (Header.GetStringFieldValue(Models.Metadata.Header.ScreenshotsWidthKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.ScreenshotsWidthKey, header.GetStringFieldValue(Models.Metadata.Header.ScreenshotsWidthKey));
            if (Header.GetStringFieldValue(Models.Metadata.Header.SystemKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.SystemKey, header.GetStringFieldValue(Models.Metadata.Header.SystemKey));
            if (Header.GetStringFieldValue(Models.Metadata.Header.TimestampKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.TimestampKey, header.GetStringFieldValue(Models.Metadata.Header.TimestampKey));
            if (Header.GetStringFieldValue(Models.Metadata.Header.TypeKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.TypeKey, header.GetStringFieldValue(Models.Metadata.Header.TypeKey));
            if (Header.GetStringFieldValue(Models.Metadata.Header.UrlKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.UrlKey, header.GetStringFieldValue(Models.Metadata.Header.UrlKey));
            if (Header.GetStringFieldValue(Models.Metadata.Header.VersionKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.VersionKey, header.GetStringFieldValue(Models.Metadata.Header.VersionKey));

            // Handle implied SuperDAT
            if (Header.GetStringFieldValue(Models.Metadata.Header.NameKey)?.Contains(" - SuperDAT") == true && keep)
            {
                if (Header.GetStringFieldValue(Models.Metadata.Header.TypeKey) == null)
                    Header.SetFieldValue<string?>(Models.Metadata.Header.TypeKey, "SuperDAT");
            }
        }

        /// <summary>
        /// Convert machines information
        /// </summary>
        /// <param name="items">Machine array to convert</param>
        /// <param name="source">Source to use with the converted items</param>
        /// <param name="sourceIndex">Index of the Source to use with the converted items</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertMachines(Models.Metadata.Machine[]? items, Source source, long sourceIndex, bool statsOnly)
        {
            // If the array is invalid, we can't do anything
            if (items == null || items.Length == 0)
                return;

            // Loop through the machines and add
            foreach (var machine in items)
            {
                ConvertMachine(machine, source, sourceIndex, statsOnly);
            }
        }

        /// <summary>
        /// Convert machine information
        /// </summary>
        /// <param name="item">Machine to convert</param>
        /// <param name="source">Source to use with the converted items</param>
        /// <param name="sourceIndex">Index of the Source to use with the converted items</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertMachine(Models.Metadata.Machine? item, Source source, long sourceIndex, bool statsOnly)
        {
            // If the machine is invalid, we can't do anything
            if (item == null || item.Count == 0)
                return;

            // Create an internal machine
            var machine = new Machine(item);

            // Process flag values
            if (machine.GetStringFieldValue(Models.Metadata.Machine.Im1CRCKey) != null)
                machine.SetFieldValue<string?>(Models.Metadata.Machine.Im1CRCKey, TextHelper.NormalizeCRC32(machine.GetStringFieldValue(Models.Metadata.Machine.Im1CRCKey)));
            if (machine.GetStringFieldValue(Models.Metadata.Machine.Im2CRCKey) != null)
                machine.SetFieldValue<string?>(Models.Metadata.Machine.Im2CRCKey, TextHelper.NormalizeCRC32(machine.GetStringFieldValue(Models.Metadata.Machine.Im2CRCKey)));
            if (machine.GetBoolFieldValue(Models.Metadata.Machine.IsBiosKey) != null)
                machine.SetFieldValue<string?>(Models.Metadata.Machine.IsBiosKey, machine.GetBoolFieldValue(Models.Metadata.Machine.IsBiosKey).FromYesNo());
            if (machine.GetBoolFieldValue(Models.Metadata.Machine.IsDeviceKey) != null)
                machine.SetFieldValue<string?>(Models.Metadata.Machine.IsDeviceKey, machine.GetBoolFieldValue(Models.Metadata.Machine.IsDeviceKey).FromYesNo());
            if (machine.GetBoolFieldValue(Models.Metadata.Machine.IsMechanicalKey) != null)
                machine.SetFieldValue<string?>(Models.Metadata.Machine.IsMechanicalKey, machine.GetBoolFieldValue(Models.Metadata.Machine.IsMechanicalKey).FromYesNo());
            if (machine.GetStringFieldValue(Models.Metadata.Machine.SupportedKey) != null)
                machine.SetFieldValue<string?>(Models.Metadata.Machine.SupportedKey, machine.GetStringFieldValue(Models.Metadata.Machine.SupportedKey).AsEnumValue<Supported>().AsStringValue());

            // Handle Trurip object, if it exists
            if (item.ContainsKey(Models.Metadata.Machine.TruripKey))
            {
                var truripItem = item.Read<Models.Logiqx.Trurip>(Models.Metadata.Machine.TruripKey);
                if (truripItem != null)
                {
                    var trurip = new DatItems.Trurip(truripItem);
                    machine.SetFieldValue<DatItems.Trurip>(Models.Metadata.Machine.TruripKey, trurip);
                }
            }

            // Add the machine to the dictionary
            long machineIndex = ItemsDB.AddMachine(machine);

            // Convert items in the machine
            if (item.ContainsKey(Models.Metadata.Machine.AdjusterKey))
            {
                var items = ReadItemArray<Models.Metadata.Adjuster>(item, Models.Metadata.Machine.AdjusterKey);
                ProcessItems(items, machine, machineIndex, source, sourceIndex, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.ArchiveKey))
            {
                var items = ReadItemArray<Models.Metadata.Archive>(item, Models.Metadata.Machine.ArchiveKey);
                ProcessItems(items, machine, machineIndex, source, sourceIndex, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.BiosSetKey))
            {
                var items = ReadItemArray<Models.Metadata.BiosSet>(item, Models.Metadata.Machine.BiosSetKey);
                ProcessItems(items, machine, machineIndex, source, sourceIndex, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.ChipKey))
            {
                var items = ReadItemArray<Models.Metadata.Chip>(item, Models.Metadata.Machine.ChipKey);
                ProcessItems(items, machine, machineIndex, source, sourceIndex, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.ConfigurationKey))
            {
                var items = ReadItemArray<Models.Metadata.Configuration>(item, Models.Metadata.Machine.ConfigurationKey);
                ProcessItems(items, machine, machineIndex, source, sourceIndex, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.DeviceKey))
            {
                var items = ReadItemArray<Models.Metadata.Device>(item, Models.Metadata.Machine.DeviceKey);
                ProcessItems(items, machine, machineIndex, source, sourceIndex, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.DeviceRefKey))
            {
                var items = ReadItemArray<Models.Metadata.DeviceRef>(item, Models.Metadata.Machine.DeviceRefKey);
                ProcessItems(items, machine, machineIndex, source, sourceIndex, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.DipSwitchKey))
            {
                var items = ReadItemArray<Models.Metadata.DipSwitch>(item, Models.Metadata.Machine.DipSwitchKey);
                ProcessItems(items, machine, machineIndex, source, sourceIndex, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.DiskKey))
            {
                var items = ReadItemArray<Models.Metadata.Disk>(item, Models.Metadata.Machine.DiskKey);
                ProcessItems(items, machine, machineIndex, source, sourceIndex, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.DisplayKey))
            {
                var items = ReadItemArray<Models.Metadata.Display>(item, Models.Metadata.Machine.DisplayKey);
                ProcessItems(items, machine, machineIndex, source, sourceIndex, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.DriverKey))
            {
                var items = ReadItemArray<Models.Metadata.Driver>(item, Models.Metadata.Machine.DriverKey);
                ProcessItems(items, machine, machineIndex, source, sourceIndex, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.DumpKey))
            {
                var items = ReadItemArray<Models.Metadata.Dump>(item, Models.Metadata.Machine.DumpKey);
                string? machineName = machine.GetStringFieldValue(Models.Metadata.Machine.NameKey);
                ProcessItems(items, machine, machineIndex, source, sourceIndex, statsOnly, machineName);
            }
            if (item.ContainsKey(Models.Metadata.Machine.FeatureKey))
            {
                var items = ReadItemArray<Models.Metadata.Feature>(item, Models.Metadata.Machine.FeatureKey);
                ProcessItems(items, machine, machineIndex, source, sourceIndex, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.InfoKey))
            {
                var items = ReadItemArray<Models.Metadata.Info>(item, Models.Metadata.Machine.InfoKey);
                ProcessItems(items, machine, machineIndex, source, sourceIndex, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.InputKey))
            {
                var items = ReadItemArray<Models.Metadata.Input>(item, Models.Metadata.Machine.InputKey);
                ProcessItems(items, machine, machineIndex, source, sourceIndex, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.MediaKey))
            {
                var items = ReadItemArray<Models.Metadata.Media>(item, Models.Metadata.Machine.MediaKey);
                ProcessItems(items, machine, machineIndex, source, sourceIndex, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.PartKey))
            {
                var items = ReadItemArray<Models.Metadata.Part>(item, Models.Metadata.Machine.PartKey);
                ProcessItems(items, machine, machineIndex, source, sourceIndex, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.PortKey))
            {
                var items = ReadItemArray<Models.Metadata.Port>(item, Models.Metadata.Machine.PortKey);
                ProcessItems(items, machine, machineIndex, source, sourceIndex, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.RamOptionKey))
            {
                var items = ReadItemArray<Models.Metadata.RamOption>(item, Models.Metadata.Machine.RamOptionKey);
                ProcessItems(items, machine, machineIndex, source, sourceIndex, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.ReleaseKey))
            {
                var items = ReadItemArray<Models.Metadata.Release>(item, Models.Metadata.Machine.ReleaseKey);
                ProcessItems(items, machine, machineIndex, source, sourceIndex, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.RomKey))
            {
                var items = ReadItemArray<Models.Metadata.Rom>(item, Models.Metadata.Machine.RomKey);
                ProcessItems(items, machine, machineIndex, source, sourceIndex, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.SampleKey))
            {
                var items = ReadItemArray<Models.Metadata.Sample>(item, Models.Metadata.Machine.SampleKey);
                ProcessItems(items, machine, machineIndex, source, sourceIndex, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.SharedFeatKey))
            {
                var items = ReadItemArray<Models.Metadata.SharedFeat>(item, Models.Metadata.Machine.SharedFeatKey);
                ProcessItems(items, machine, machineIndex, source, sourceIndex, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.SlotKey))
            {
                var items = ReadItemArray<Models.Metadata.Slot>(item, Models.Metadata.Machine.SlotKey);
                ProcessItems(items, machine, machineIndex, source, sourceIndex, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.SoftwareListKey))
            {
                var items = ReadItemArray<Models.Metadata.SoftwareList>(item, Models.Metadata.Machine.SoftwareListKey);
                ProcessItems(items, machine, machineIndex, source, sourceIndex, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.SoundKey))
            {
                var items = ReadItemArray<Models.Metadata.Sound>(item, Models.Metadata.Machine.SoundKey);
                ProcessItems(items, machine, machineIndex, source, sourceIndex, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.VideoKey))
            {
                var items = ReadItemArray<Models.Metadata.Video>(item, Models.Metadata.Machine.VideoKey);
                ProcessItems(items, machine, machineIndex, source, sourceIndex, statsOnly);
            }
        }

        /// <summary>
        /// Convert Adjuster information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="machineIndex">Index of the Machine to use with the converted items</param>
        /// <param name="source">Source to use with the converted items</param>
        /// <param name="sourceIndex">Index of the Source to use with the converted items</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Adjuster[]? items, Machine machine, long machineIndex, Source source, long sourceIndex, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Adjuster(item);
                datItem.SetFieldValue<Source?>(DatItems.DatItem.SourceKey, source);
                datItem.CopyMachineInformation(machine);

                // Process flag values
                if (datItem.GetBoolFieldValue(Models.Metadata.Adjuster.DefaultKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Adjuster.DefaultKey, datItem.GetBoolFieldValue(Models.Metadata.Adjuster.DefaultKey).FromYesNo());

                // Handle subitems
                var condition = item.Read<Models.Metadata.Condition>(Models.Metadata.Adjuster.ConditionKey);
                if (condition != null)
                {
                    var subItem = new DatItems.Formats.Condition(condition);

                    // Process flag values
                    if (subItem.GetStringFieldValue(Models.Metadata.Condition.RelationKey) != null)
                        subItem.SetFieldValue<string?>(Models.Metadata.Condition.RelationKey, subItem.GetStringFieldValue(Models.Metadata.Condition.RelationKey).AsEnumValue<Relation>().AsStringValue());

                    datItem.SetFieldValue<DatItems.Formats.Condition?>(Models.Metadata.Adjuster.ConditionKey, subItem);
                }

                Items.AddItem(datItem, statsOnly);
                ItemsDB.AddItem(datItem, machineIndex, sourceIndex, statsOnly);
            }
        }

        /// <summary>
        /// Convert Archive information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="machineIndex">Index of the Machine to use with the converted items</param>
        /// <param name="source">Source to use with the converted items</param>
        /// <param name="sourceIndex">Index of the Source to use with the converted items</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Archive[]? items, Machine machine, long machineIndex, Source source, long sourceIndex, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Archive(item);
                datItem.SetFieldValue<Source?>(DatItems.DatItem.SourceKey, source);
                datItem.CopyMachineInformation(machine);
                Items.AddItem(datItem, statsOnly);
                ItemsDB.AddItem(datItem, machineIndex, sourceIndex, statsOnly);
            }
        }

        /// <summary>
        /// Convert BiosSet information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="machineIndex">Index of the Machine to use with the converted items</param>
        /// <param name="source">Source to use with the converted items</param>
        /// <param name="sourceIndex">Index of the Source to use with the converted items</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.BiosSet[]? items, Machine machine, long machineIndex, Source source, long sourceIndex, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.BiosSet(item);
                datItem.SetFieldValue<Source?>(DatItems.DatItem.SourceKey, source);
                datItem.CopyMachineInformation(machine);

                // Process flag values
                if (datItem.GetBoolFieldValue(Models.Metadata.BiosSet.DefaultKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.BiosSet.DefaultKey, datItem.GetBoolFieldValue(Models.Metadata.BiosSet.DefaultKey).FromYesNo());

                Items.AddItem(datItem, statsOnly);
                ItemsDB.AddItem(datItem, machineIndex, sourceIndex, statsOnly);
            }
        }

        /// <summary>
        /// Convert Chip information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="machineIndex">Index of the Machine to use with the converted items</param>
        /// <param name="source">Source to use with the converted items</param>
        /// <param name="sourceIndex">Index of the Source to use with the converted items</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Chip[]? items, Machine machine, long machineIndex, Source source, long sourceIndex, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Chip(item);
                datItem.SetFieldValue<Source?>(DatItems.DatItem.SourceKey, source);
                datItem.CopyMachineInformation(machine);

                // Process flag values
                if (datItem.GetBoolFieldValue(Models.Metadata.Chip.SoundOnlyKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Chip.SoundOnlyKey, datItem.GetBoolFieldValue(Models.Metadata.Chip.SoundOnlyKey).FromYesNo());
                if (datItem.GetStringFieldValue(Models.Metadata.Chip.ChipTypeKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Chip.ChipTypeKey, datItem.GetStringFieldValue(Models.Metadata.Chip.ChipTypeKey).AsEnumValue<ChipType>().AsStringValue());

                Items.AddItem(datItem, statsOnly);
                ItemsDB.AddItem(datItem, machineIndex, sourceIndex, statsOnly);
            }
        }

        /// <summary>
        /// Convert Configuration information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="machineIndex">Index of the Machine to use with the converted items</param>
        /// <param name="source">Source to use with the converted items</param>
        /// <param name="sourceIndex">Index of the Source to use with the converted items</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Configuration[]? items, Machine machine, long machineIndex, Source source, long sourceIndex, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Configuration(item);
                datItem.SetFieldValue<Source?>(DatItems.DatItem.SourceKey, source);
                datItem.CopyMachineInformation(machine);

                // Handle subitems
                var condition = item.Read<Models.Metadata.Condition>(Models.Metadata.Configuration.ConditionKey);
                if (condition != null)
                {
                    var subItem = new DatItems.Formats.Condition(condition);

                    // Process flag values
                    if (subItem.GetStringFieldValue(Models.Metadata.Condition.RelationKey) != null)
                        subItem.SetFieldValue<string?>(Models.Metadata.Condition.RelationKey, subItem.GetStringFieldValue(Models.Metadata.Condition.RelationKey).AsEnumValue<Relation>().AsStringValue());

                    datItem.SetFieldValue<DatItems.Formats.Condition?>(Models.Metadata.Configuration.ConditionKey, subItem);
                }

                var confLocations = ReadItemArray<Models.Metadata.ConfLocation>(item, Models.Metadata.Configuration.ConfLocationKey);
                if (confLocations != null)
                {
                    List<DatItems.Formats.ConfLocation> subLocations = [];
                    foreach (var location in confLocations)
                    {
                        var subItem = new DatItems.Formats.ConfLocation(location);

                        // Process flag values
                        if (subItem.GetBoolFieldValue(Models.Metadata.ConfLocation.InvertedKey) != null)
                            subItem.SetFieldValue<string?>(Models.Metadata.ConfLocation.InvertedKey, subItem.GetBoolFieldValue(Models.Metadata.ConfLocation.InvertedKey).FromYesNo());

                        subLocations.Add(subItem);
                    }

                    datItem.SetFieldValue<DatItems.Formats.ConfLocation[]?>(Models.Metadata.Configuration.ConfLocationKey, [.. subLocations]);
                }

                var confSettings = ReadItemArray<Models.Metadata.ConfSetting>(item, Models.Metadata.Configuration.ConfSettingKey);
                if (confSettings != null)
                {
                    List<DatItems.Formats.ConfSetting> subValues = [];
                    foreach (var setting in confSettings)
                    {
                        var subItem = new DatItems.Formats.ConfSetting(setting);

                        // Process flag values
                        if (subItem.GetBoolFieldValue(Models.Metadata.ConfSetting.DefaultKey) != null)
                            subItem.SetFieldValue<string?>(Models.Metadata.ConfSetting.DefaultKey, subItem.GetBoolFieldValue(Models.Metadata.ConfSetting.DefaultKey).FromYesNo());

                        var subCondition = subItem.GetFieldValue<Models.Metadata.Condition>(Models.Metadata.ConfSetting.ConditionKey);
                        if (subCondition != null)
                        {
                            var subSubItem = new DatItems.Formats.Condition(subCondition);

                            // Process flag values
                            if (subSubItem.GetStringFieldValue(Models.Metadata.Condition.RelationKey) != null)
                                subSubItem.SetFieldValue<string?>(Models.Metadata.Condition.RelationKey, subSubItem.GetStringFieldValue(Models.Metadata.Condition.RelationKey).AsEnumValue<Relation>().AsStringValue());

                            subItem.SetFieldValue<DatItems.Formats.Condition?>(Models.Metadata.ConfSetting.ConditionKey, subSubItem);
                        }

                        subValues.Add(subItem);
                    }

                    datItem.SetFieldValue<DatItems.Formats.ConfSetting[]?>(Models.Metadata.Configuration.ConfSettingKey, [.. subValues]);
                }

                Items.AddItem(datItem, statsOnly);
                ItemsDB.AddItem(datItem, machineIndex, sourceIndex, statsOnly);
            }
        }

        /// <summary>
        /// Convert Device information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="machineIndex">Index of the Machine to use with the converted items</param>
        /// <param name="source">Source to use with the converted items</param>
        /// <param name="sourceIndex">Index of the Source to use with the converted items</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Device[]? items, Machine machine, long machineIndex, Source source, long sourceIndex, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Device(item);
                datItem.SetFieldValue<Source?>(DatItems.DatItem.SourceKey, source);
                datItem.CopyMachineInformation(machine);

                // Process flag values
                if (datItem.GetBoolFieldValue(Models.Metadata.Device.MandatoryKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Device.MandatoryKey, datItem.GetBoolFieldValue(Models.Metadata.Device.MandatoryKey).FromYesNo());
                if (datItem.GetStringFieldValue(Models.Metadata.Device.DeviceTypeKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Device.DeviceTypeKey, datItem.GetStringFieldValue(Models.Metadata.Device.DeviceTypeKey).AsEnumValue<DeviceType>().AsStringValue());

                // Handle subitems
                var instance = item.Read<Models.Metadata.Instance>(Models.Metadata.Device.InstanceKey);
                if (instance != null)
                {
                    var subItem = new DatItems.Formats.Instance(instance);
                    datItem.SetFieldValue<DatItems.Formats.Instance?>(Models.Metadata.Device.InstanceKey, subItem);
                }

                var extensions = ReadItemArray<Models.Metadata.Extension>(item, Models.Metadata.Device.ExtensionKey);
                if (extensions != null)
                {
                    List<DatItems.Formats.Extension> subExtensions = [];
                    foreach (var extension in extensions)
                    {
                        var subItem = new DatItems.Formats.Extension(extension);
                        subExtensions.Add(subItem);
                    }

                    datItem.SetFieldValue<DatItems.Formats.Extension[]?>(Models.Metadata.Device.ExtensionKey, [.. subExtensions]);
                }

                Items.AddItem(datItem, statsOnly);
                ItemsDB.AddItem(datItem, machineIndex, sourceIndex, statsOnly);
            }
        }

        /// <summary>
        /// Convert DeviceRef information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="machineIndex">Index of the Machine to use with the converted items</param>
        /// <param name="source">Source to use with the converted items</param>
        /// <param name="sourceIndex">Index of the Source to use with the converted items</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.DeviceRef[]? items, Machine machine, long machineIndex, Source source, long sourceIndex, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.DeviceRef(item);
                datItem.SetFieldValue<Source?>(DatItems.DatItem.SourceKey, source);
                datItem.CopyMachineInformation(machine);
                Items.AddItem(datItem, statsOnly);
                ItemsDB.AddItem(datItem, machineIndex, sourceIndex, statsOnly);
            }
        }

        /// <summary>
        /// Convert DipSwitch information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="machineIndex">Index of the Machine to use with the converted items</param>
        /// <param name="source">Source to use with the converted items</param>
        /// <param name="sourceIndex">Index of the Source to use with the converted items</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.DipSwitch[]? items, Machine machine, long machineIndex, Source source, long sourceIndex, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.DipSwitch(item);
                datItem.SetFieldValue<Source?>(DatItems.DatItem.SourceKey, source);
                datItem.CopyMachineInformation(machine);

                // Process flag values
                if (datItem.GetBoolFieldValue(Models.Metadata.DipSwitch.DefaultKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.DipSwitch.DefaultKey, datItem.GetBoolFieldValue(Models.Metadata.DipSwitch.DefaultKey).FromYesNo());

                // Handle subitems
                var condition = item.Read<Models.Metadata.Condition>(Models.Metadata.DipSwitch.ConditionKey);
                if (condition != null)
                {
                    var subItem = new DatItems.Formats.Condition(condition);

                    // Process flag values
                    if (subItem.GetStringFieldValue(Models.Metadata.Condition.RelationKey) != null)
                        subItem.SetFieldValue<string?>(Models.Metadata.Condition.RelationKey, subItem.GetStringFieldValue(Models.Metadata.Condition.RelationKey).AsEnumValue<Relation>().AsStringValue());

                    datItem.SetFieldValue<DatItems.Formats.Condition?>(Models.Metadata.DipSwitch.ConditionKey, subItem);
                }

                var dipLocations = ReadItemArray<Models.Metadata.DipLocation>(item, Models.Metadata.DipSwitch.DipLocationKey);
                if (dipLocations != null)
                {
                    List<DatItems.Formats.DipLocation> subLocations = [];
                    foreach (var location in dipLocations)
                    {
                        var subItem = new DatItems.Formats.DipLocation(location);

                        // Process flag values
                        if (subItem.GetBoolFieldValue(Models.Metadata.DipLocation.InvertedKey) != null)
                            subItem.SetFieldValue<string?>(Models.Metadata.DipLocation.InvertedKey, subItem.GetBoolFieldValue(Models.Metadata.DipLocation.InvertedKey).FromYesNo());

                        subLocations.Add(subItem);
                    }

                    datItem.SetFieldValue<DatItems.Formats.DipLocation[]?>(Models.Metadata.DipSwitch.DipLocationKey, [.. subLocations]);
                }

                var dipValues = ReadItemArray<Models.Metadata.DipValue>(item, Models.Metadata.DipSwitch.DipValueKey);
                if (dipValues != null)
                {
                    List<DatItems.Formats.DipValue> subValues = [];
                    foreach (var value in dipValues)
                    {
                        var subItem = new DatItems.Formats.DipValue(value);

                        // Process flag values
                        if (subItem.GetBoolFieldValue(Models.Metadata.DipValue.DefaultKey) != null)
                            subItem.SetFieldValue<string?>(Models.Metadata.DipValue.DefaultKey, subItem.GetBoolFieldValue(Models.Metadata.DipValue.DefaultKey).FromYesNo());

                        var subCondition = subItem.GetFieldValue<Models.Metadata.Condition>(Models.Metadata.DipValue.ConditionKey);
                        if (subCondition != null)
                        {
                            var subSubItem = new DatItems.Formats.Condition(subCondition);

                            // Process flag values
                            if (subSubItem.GetStringFieldValue(Models.Metadata.Condition.RelationKey) != null)
                                subSubItem.SetFieldValue<string?>(Models.Metadata.Condition.RelationKey, subSubItem.GetStringFieldValue(Models.Metadata.Condition.RelationKey).AsEnumValue<Relation>().AsStringValue());

                            subItem.SetFieldValue<DatItems.Formats.Condition?>(Models.Metadata.DipValue.ConditionKey, subSubItem);
                        }

                        subValues.Add(subItem);
                    }

                    datItem.SetFieldValue<DatItems.Formats.DipValue[]?>(Models.Metadata.DipSwitch.DipValueKey, [.. subValues]);
                }

                Items.AddItem(datItem, statsOnly);
                ItemsDB.AddItem(datItem, machineIndex, sourceIndex, statsOnly);
            }
        }

        /// <summary>
        /// Convert Disk information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="machineIndex">Index of the Machine to use with the converted items</param>
        /// <param name="source">Source to use with the converted items</param>
        /// <param name="sourceIndex">Index of the Source to use with the converted items</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Disk[]? items, Machine machine, long machineIndex, Source source, long sourceIndex, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Disk(item);
                datItem.SetFieldValue<Source?>(DatItems.DatItem.SourceKey, source);
                datItem.CopyMachineInformation(machine);

                // Process flag values
                if (datItem.GetBoolFieldValue(Models.Metadata.Disk.OptionalKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Disk.OptionalKey, datItem.GetBoolFieldValue(Models.Metadata.Disk.OptionalKey).FromYesNo());
                if (datItem.GetStringFieldValue(Models.Metadata.Disk.StatusKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Disk.StatusKey, datItem.GetStringFieldValue(Models.Metadata.Disk.StatusKey).AsEnumValue<ItemStatus>().AsStringValue());
                if (datItem.GetBoolFieldValue(Models.Metadata.Disk.WritableKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Disk.WritableKey, datItem.GetBoolFieldValue(Models.Metadata.Disk.WritableKey).FromYesNo());

                // Process hash values
                if (datItem.GetStringFieldValue(Models.Metadata.Disk.MD5Key) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Disk.MD5Key, TextHelper.NormalizeMD5(datItem.GetStringFieldValue(Models.Metadata.Disk.MD5Key)));
                if (datItem.GetStringFieldValue(Models.Metadata.Disk.SHA1Key) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Disk.SHA1Key, TextHelper.NormalizeSHA1(datItem.GetStringFieldValue(Models.Metadata.Disk.SHA1Key)));

                Items.AddItem(datItem, statsOnly);
                ItemsDB.AddItem(datItem, machineIndex, sourceIndex, statsOnly);
            }
        }

        /// <summary>
        /// Convert Display information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="machineIndex">Index of the Machine to use with the converted items</param>
        /// <param name="source">Source to use with the converted items</param>
        /// <param name="sourceIndex">Index of the Source to use with the converted items</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Display[]? items, Machine machine, long machineIndex, Source source, long sourceIndex, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Display(item);
                datItem.SetFieldValue<Source?>(DatItems.DatItem.SourceKey, source);
                datItem.CopyMachineInformation(machine);

                // Process flag values
                if (datItem.GetBoolFieldValue(Models.Metadata.Display.FlipXKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Display.FlipXKey, datItem.GetBoolFieldValue(Models.Metadata.Display.FlipXKey).FromYesNo());
                if (datItem.GetInt64FieldValue(Models.Metadata.Display.HBEndKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Display.HBEndKey, datItem.GetInt64FieldValue(Models.Metadata.Display.HBEndKey).ToString());
                if (datItem.GetInt64FieldValue(Models.Metadata.Display.HBStartKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Display.HBStartKey, datItem.GetInt64FieldValue(Models.Metadata.Display.HBStartKey).ToString());
                if (datItem.GetInt64FieldValue(Models.Metadata.Display.HeightKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Display.HeightKey, datItem.GetInt64FieldValue(Models.Metadata.Display.HeightKey).ToString());
                if (datItem.GetInt64FieldValue(Models.Metadata.Display.HTotalKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Display.HTotalKey, datItem.GetInt64FieldValue(Models.Metadata.Display.HTotalKey).ToString());
                if (datItem.GetInt64FieldValue(Models.Metadata.Display.PixClockKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Display.PixClockKey, datItem.GetInt64FieldValue(Models.Metadata.Display.PixClockKey).ToString());
                if (datItem.GetDoubleFieldValue(Models.Metadata.Display.RefreshKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Display.RefreshKey, datItem.GetDoubleFieldValue(Models.Metadata.Display.RefreshKey).ToString());
                if (datItem.GetInt64FieldValue(Models.Metadata.Display.RotateKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Display.RotateKey, datItem.GetInt64FieldValue(Models.Metadata.Display.RotateKey).ToString());
                if (datItem.GetStringFieldValue(Models.Metadata.Display.DisplayTypeKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Display.DisplayTypeKey, datItem.GetStringFieldValue(Models.Metadata.Display.DisplayTypeKey).AsEnumValue<DisplayType>().AsStringValue());
                if (datItem.GetInt64FieldValue(Models.Metadata.Display.VBEndKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Display.VBEndKey, datItem.GetInt64FieldValue(Models.Metadata.Display.VBEndKey).ToString());
                if (datItem.GetInt64FieldValue(Models.Metadata.Display.VBStartKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Display.VBStartKey, datItem.GetInt64FieldValue(Models.Metadata.Display.VBStartKey).ToString());
                if (datItem.GetInt64FieldValue(Models.Metadata.Display.VTotalKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Display.VTotalKey, datItem.GetInt64FieldValue(Models.Metadata.Display.VTotalKey).ToString());
                if (datItem.GetInt64FieldValue(Models.Metadata.Display.WidthKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Display.WidthKey, datItem.GetInt64FieldValue(Models.Metadata.Display.WidthKey).ToString());

                Items.AddItem(datItem, statsOnly);
                ItemsDB.AddItem(datItem, machineIndex, sourceIndex, statsOnly);
            }
        }

        /// <summary>
        /// Convert Driver information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="machineIndex">Index of the Machine to use with the converted items</param>
        /// <param name="source">Source to use with the converted items</param>
        /// <param name="sourceIndex">Index of the Source to use with the converted items</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Driver[]? items, Machine machine, long machineIndex, Source source, long sourceIndex, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Driver(item);
                datItem.SetFieldValue<Source?>(DatItems.DatItem.SourceKey, source);
                datItem.CopyMachineInformation(machine);

                // Process flag values
                if (datItem.GetStringFieldValue(Models.Metadata.Driver.CocktailKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Driver.CocktailKey, datItem.GetStringFieldValue(Models.Metadata.Driver.CocktailKey).AsEnumValue<SupportStatus>().AsStringValue());
                if (datItem.GetStringFieldValue(Models.Metadata.Driver.ColorKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Driver.ColorKey, datItem.GetStringFieldValue(Models.Metadata.Driver.ColorKey).AsEnumValue<SupportStatus>().AsStringValue());
                if (datItem.GetStringFieldValue(Models.Metadata.Driver.EmulationKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Driver.EmulationKey, datItem.GetStringFieldValue(Models.Metadata.Driver.EmulationKey).AsEnumValue<SupportStatus>().AsStringValue());
                if (datItem.GetBoolFieldValue(Models.Metadata.Driver.IncompleteKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Driver.IncompleteKey, datItem.GetBoolFieldValue(Models.Metadata.Driver.IncompleteKey).FromYesNo());
                if (datItem.GetBoolFieldValue(Models.Metadata.Driver.NoSoundHardwareKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Driver.NoSoundHardwareKey, datItem.GetBoolFieldValue(Models.Metadata.Driver.NoSoundHardwareKey).FromYesNo());
                if (datItem.GetInt64FieldValue(Models.Metadata.Driver.PaletteSizeKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Driver.PaletteSizeKey, datItem.GetInt64FieldValue(Models.Metadata.Driver.PaletteSizeKey).ToString());
                if (datItem.GetBoolFieldValue(Models.Metadata.Driver.RequiresArtworkKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Driver.RequiresArtworkKey, datItem.GetBoolFieldValue(Models.Metadata.Driver.RequiresArtworkKey).FromYesNo());
                if (datItem.GetStringFieldValue(Models.Metadata.Driver.SaveStateKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Driver.SaveStateKey, datItem.GetStringFieldValue(Models.Metadata.Driver.SaveStateKey).AsEnumValue<Supported>().AsStringValue(useSecond: true));
                if (datItem.GetStringFieldValue(Models.Metadata.Driver.SoundKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Driver.SoundKey, datItem.GetStringFieldValue(Models.Metadata.Driver.SoundKey).AsEnumValue<SupportStatus>().AsStringValue());
                if (datItem.GetStringFieldValue(Models.Metadata.Driver.StatusKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Driver.StatusKey, datItem.GetStringFieldValue(Models.Metadata.Driver.StatusKey).AsEnumValue<SupportStatus>().AsStringValue());
                if (datItem.GetBoolFieldValue(Models.Metadata.Driver.UnofficialKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Driver.UnofficialKey, datItem.GetBoolFieldValue(Models.Metadata.Driver.UnofficialKey).FromYesNo());

                Items.AddItem(datItem, statsOnly);
                ItemsDB.AddItem(datItem, machineIndex, sourceIndex, statsOnly);
            }
        }

        /// <summary>
        /// Convert Dump information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="machineIndex">Index of the Machine to use with the converted items</param>
        /// <param name="source">Source to use with the converted items</param>
        /// <param name="sourceIndex">Index of the Source to use with the converted items</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="machineName">Machine name to use when constructing item names</param>
        private void ProcessItems(Models.Metadata.Dump[]? items, Machine machine, long machineIndex, Source source, long sourceIndex, bool statsOnly, string? machineName)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            int index = 0;
            foreach (var dump in items)
            {
                // If we don't have rom data, we can't do anything
                Models.Metadata.Rom? rom = null;
                OpenMSXSubType subType = OpenMSXSubType.NULL;
                if (dump?.Read<Models.Metadata.Rom>(Models.Metadata.Dump.RomKey) != null)
                {
                    rom = dump.Read<Models.Metadata.Rom>(Models.Metadata.Dump.RomKey);
                    subType = OpenMSXSubType.Rom;
                }
                else if (dump?.Read<Models.Metadata.Rom>(Models.Metadata.Dump.MegaRomKey) != null)
                {
                    rom = dump.Read<Models.Metadata.Rom>(Models.Metadata.Dump.MegaRomKey);
                    subType = OpenMSXSubType.MegaRom;
                }
                else if (dump?.Read<Models.Metadata.Rom>(Models.Metadata.Dump.SCCPlusCartKey) != null)
                {
                    rom = dump.Read<Models.Metadata.Rom>(Models.Metadata.Dump.SCCPlusCartKey);
                    subType = OpenMSXSubType.SCCPlusCart;
                }
                else
                {
                    continue;
                }

                string name = $"{machineName}_{index++}{(!string.IsNullOrEmpty(rom!.ReadString(Models.Metadata.Rom.RemarkKey)) ? $" {rom.ReadString(Models.Metadata.Rom.RemarkKey)}" : string.Empty)}";

                var datItem = new DatItems.Formats.Rom();
                datItem.SetName(name);
                datItem.SetFieldValue<string?>(Models.Metadata.Rom.OffsetKey, rom.ReadString(Models.Metadata.Rom.StartKey));
                datItem.SetFieldValue<string?>(Models.Metadata.Rom.OpenMSXMediaType, subType.AsStringValue());
                datItem.SetFieldValue<string?>(Models.Metadata.Rom.OpenMSXType, rom.ReadString(Models.Metadata.Rom.OpenMSXType) ?? rom.ReadString(Models.Metadata.Rom.TypeKey));
                datItem.SetFieldValue<string?>(Models.Metadata.Rom.RemarkKey, rom.ReadString(Models.Metadata.Rom.RemarkKey));
                datItem.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, rom.ReadString(Models.Metadata.Rom.SHA1Key));
                datItem.SetFieldValue<string?>(Models.Metadata.Rom.StartKey, rom.ReadString(Models.Metadata.Rom.StartKey));
                datItem.SetFieldValue<Source?>(DatItems.DatItem.SourceKey, source);

                if (dump.Read<Models.Metadata.Original>(Models.Metadata.Dump.OriginalKey) != null)
                {
                    var original = dump.Read<Models.Metadata.Original>(Models.Metadata.Dump.OriginalKey)!;
                    datItem.SetFieldValue<DatItems.Formats.Original?>("ORIGINAL", new DatItems.Formats.Original
                    {
                        Value = original.ReadBool(Models.Metadata.Original.ValueKey),
                        Content = original.ReadString(Models.Metadata.Original.ContentKey),
                    });
                }

                datItem.CopyMachineInformation(machine);

                // Process hash values
                if (datItem.GetInt64FieldValue(Models.Metadata.Rom.SizeKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, datItem.GetInt64FieldValue(Models.Metadata.Rom.SizeKey).ToString());
                if (datItem.GetStringFieldValue(Models.Metadata.Rom.CRCKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, TextHelper.NormalizeCRC32(datItem.GetStringFieldValue(Models.Metadata.Rom.CRCKey)));
                if (datItem.GetStringFieldValue(Models.Metadata.Rom.MD2Key) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Rom.MD2Key, TextHelper.NormalizeMD2(datItem.GetStringFieldValue(Models.Metadata.Rom.MD2Key)));
                if (datItem.GetStringFieldValue(Models.Metadata.Rom.MD4Key) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Rom.MD4Key, TextHelper.NormalizeMD5(datItem.GetStringFieldValue(Models.Metadata.Rom.MD4Key)));
                if (datItem.GetStringFieldValue(Models.Metadata.Rom.MD5Key) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Rom.MD5Key, TextHelper.NormalizeMD5(datItem.GetStringFieldValue(Models.Metadata.Rom.MD5Key)));
                if (datItem.GetStringFieldValue(Models.Metadata.Rom.SHA1Key) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, TextHelper.NormalizeSHA1(datItem.GetStringFieldValue(Models.Metadata.Rom.SHA1Key)));
                if (datItem.GetStringFieldValue(Models.Metadata.Rom.SHA256Key) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Rom.SHA256Key, TextHelper.NormalizeSHA256(datItem.GetStringFieldValue(Models.Metadata.Rom.SHA256Key)));
                if (datItem.GetStringFieldValue(Models.Metadata.Rom.SHA384Key) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Rom.SHA384Key, TextHelper.NormalizeSHA384(datItem.GetStringFieldValue(Models.Metadata.Rom.SHA384Key)));
                if (datItem.GetStringFieldValue(Models.Metadata.Rom.SHA512Key) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Rom.SHA512Key, TextHelper.NormalizeSHA512(datItem.GetStringFieldValue(Models.Metadata.Rom.SHA512Key)));

                Items.AddItem(datItem, statsOnly);
                ItemsDB.AddItem(datItem, machineIndex, sourceIndex, statsOnly);
            }
        }

        /// <summary>
        /// Convert Feature information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="machineIndex">Index of the Machine to use with the converted items</param>
        /// <param name="source">Source to use with the converted items</param>
        /// <param name="sourceIndex">Index of the Source to use with the converted items</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Feature[]? items, Machine machine, long machineIndex, Source source, long sourceIndex, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Feature(item);
                datItem.SetFieldValue<Source?>(DatItems.DatItem.SourceKey, source);
                datItem.CopyMachineInformation(machine);

                // Process flag values
                if (datItem.GetStringFieldValue(Models.Metadata.Feature.OverallKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Feature.OverallKey, datItem.GetStringFieldValue(Models.Metadata.Feature.OverallKey).AsEnumValue<FeatureStatus>().AsStringValue());
                if (datItem.GetStringFieldValue(Models.Metadata.Feature.StatusKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Feature.StatusKey, datItem.GetStringFieldValue(Models.Metadata.Feature.StatusKey).AsEnumValue<FeatureStatus>().AsStringValue());
                if (datItem.GetStringFieldValue(Models.Metadata.Feature.FeatureTypeKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Feature.FeatureTypeKey, datItem.GetStringFieldValue(Models.Metadata.Feature.FeatureTypeKey).AsEnumValue<FeatureType>().AsStringValue());

                Items.AddItem(datItem, statsOnly);
                ItemsDB.AddItem(datItem, machineIndex, sourceIndex, statsOnly);
            }
        }

        /// <summary>
        /// Convert Info information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="machineIndex">Index of the Machine to use with the converted items</param>
        /// <param name="source">Source to use with the converted items</param>
        /// <param name="sourceIndex">Index of the Source to use with the converted items</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Info[]? items, Machine machine, long machineIndex, Source source, long sourceIndex, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Info(item);
                datItem.SetFieldValue<Source?>(DatItems.DatItem.SourceKey, source);
                datItem.CopyMachineInformation(machine);
                Items.AddItem(datItem, statsOnly);
                ItemsDB.AddItem(datItem, machineIndex, sourceIndex, statsOnly);
            }
        }

        /// <summary>
        /// Convert Input information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="machineIndex">Index of the Machine to use with the converted items</param>
        /// <param name="source">Source to use with the converted items</param>
        /// <param name="sourceIndex">Index of the Source to use with the converted items</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Input[]? items, Machine machine, long machineIndex, Source source, long sourceIndex, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Input(item);
                datItem.SetFieldValue<Source?>(DatItems.DatItem.SourceKey, source);
                datItem.CopyMachineInformation(machine);

                // Process flag values
                if (datItem.GetInt64FieldValue(Models.Metadata.Input.ButtonsKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Input.ButtonsKey, datItem.GetInt64FieldValue(Models.Metadata.Input.ButtonsKey).ToString());
                if (datItem.GetInt64FieldValue(Models.Metadata.Input.CoinsKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Input.CoinsKey, datItem.GetInt64FieldValue(Models.Metadata.Input.CoinsKey).ToString());
                if (datItem.GetInt64FieldValue(Models.Metadata.Input.PlayersKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Input.PlayersKey, datItem.GetInt64FieldValue(Models.Metadata.Input.PlayersKey).ToString());
                if (datItem.GetBoolFieldValue(Models.Metadata.Input.ServiceKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Input.ServiceKey, datItem.GetBoolFieldValue(Models.Metadata.Input.ServiceKey).FromYesNo());
                if (datItem.GetBoolFieldValue(Models.Metadata.Input.TiltKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Input.TiltKey, datItem.GetBoolFieldValue(Models.Metadata.Input.TiltKey).FromYesNo());

                // Handle subitems
                var controls = ReadItemArray<Models.Metadata.Control>(item, Models.Metadata.Input.ControlKey);
                if (controls != null)
                {
                    List<DatItems.Formats.Control> subControls = [];
                    foreach (var control in controls)
                    {
                        var subItem = new DatItems.Formats.Control(control);

                        // Process flag values
                        if (subItem.GetInt64FieldValue(Models.Metadata.Control.ButtonsKey) != null)
                            subItem.SetFieldValue<string?>(Models.Metadata.Control.ButtonsKey, subItem.GetInt64FieldValue(Models.Metadata.Control.ButtonsKey).ToString());
                        if (subItem.GetInt64FieldValue(Models.Metadata.Control.KeyDeltaKey) != null)
                            subItem.SetFieldValue<string?>(Models.Metadata.Control.KeyDeltaKey, subItem.GetInt64FieldValue(Models.Metadata.Control.KeyDeltaKey).ToString());
                        if (subItem.GetInt64FieldValue(Models.Metadata.Control.MaximumKey) != null)
                            subItem.SetFieldValue<string?>(Models.Metadata.Control.MaximumKey, subItem.GetInt64FieldValue(Models.Metadata.Control.MaximumKey).ToString());
                        if (subItem.GetInt64FieldValue(Models.Metadata.Control.MinimumKey) != null)
                            subItem.SetFieldValue<string?>(Models.Metadata.Control.MinimumKey, subItem.GetInt64FieldValue(Models.Metadata.Control.MinimumKey).ToString());
                        if (subItem.GetInt64FieldValue(Models.Metadata.Control.PlayerKey) != null)
                            subItem.SetFieldValue<string?>(Models.Metadata.Control.PlayerKey, subItem.GetInt64FieldValue(Models.Metadata.Control.PlayerKey).ToString());
                        if (subItem.GetInt64FieldValue(Models.Metadata.Control.ReqButtonsKey) != null)
                            subItem.SetFieldValue<string?>(Models.Metadata.Control.ReqButtonsKey, subItem.GetInt64FieldValue(Models.Metadata.Control.ReqButtonsKey).ToString());
                        if (subItem.GetBoolFieldValue(Models.Metadata.Control.ReverseKey) != null)
                            subItem.SetFieldValue<string?>(Models.Metadata.Control.ReverseKey, subItem.GetBoolFieldValue(Models.Metadata.Control.ReverseKey).FromYesNo());
                        if (subItem.GetInt64FieldValue(Models.Metadata.Control.SensitivityKey) != null)
                            subItem.SetFieldValue<string?>(Models.Metadata.Control.SensitivityKey, subItem.GetInt64FieldValue(Models.Metadata.Control.SensitivityKey).ToString());
                        if (subItem.GetStringFieldValue(Models.Metadata.Control.ControlTypeKey) != null)
                            subItem.SetFieldValue<string?>(Models.Metadata.Control.ControlTypeKey, subItem.GetStringFieldValue(Models.Metadata.Control.ControlTypeKey).AsEnumValue<ControlType>().AsStringValue());

                        subControls.Add(subItem);
                    }

                    datItem.SetFieldValue<DatItems.Formats.Control[]?>(Models.Metadata.Input.ControlKey, [.. subControls]);
                }

                Items.AddItem(datItem, statsOnly);
                ItemsDB.AddItem(datItem, machineIndex, sourceIndex, statsOnly);
            }
        }

        /// <summary>
        /// Convert Media information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="machineIndex">Index of the Machine to use with the converted items</param>
        /// <param name="source">Source to use with the converted items</param>
        /// <param name="sourceIndex">Index of the Source to use with the converted items</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Media[]? items, Machine machine, long machineIndex, Source source, long sourceIndex, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Media(item);
                datItem.SetFieldValue<Source?>(DatItems.DatItem.SourceKey, source);
                datItem.CopyMachineInformation(machine);

                // Process hash values
                if (datItem.GetStringFieldValue(Models.Metadata.Media.MD5Key) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Media.MD5Key, TextHelper.NormalizeMD5(datItem.GetStringFieldValue(Models.Metadata.Media.MD5Key)));
                if (datItem.GetStringFieldValue(Models.Metadata.Media.SHA1Key) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Media.SHA1Key, TextHelper.NormalizeSHA1(datItem.GetStringFieldValue(Models.Metadata.Media.SHA1Key)));
                if (datItem.GetStringFieldValue(Models.Metadata.Media.SHA256Key) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Media.SHA256Key, TextHelper.NormalizeSHA256(datItem.GetStringFieldValue(Models.Metadata.Media.SHA256Key)));

                Items.AddItem(datItem, statsOnly);
                ItemsDB.AddItem(datItem, machineIndex, sourceIndex, statsOnly);
            }
        }

        /// <summary>
        /// Convert Part information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="machineIndex">Index of the Machine to use with the converted items</param>
        /// <param name="source">Source to use with the converted items</param>
        /// <param name="sourceIndex">Index of the Source to use with the converted items</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Part[]? items, Machine machine, long machineIndex, Source source, long sourceIndex, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var partItem = new DatItems.Formats.Part(item);

                // Handle subitems
                var dataAreas = ReadItemArray<Models.Metadata.DataArea>(item, Models.Metadata.Part.DataAreaKey);
                if (dataAreas != null)
                {
                    foreach (var dataArea in dataAreas)
                    {
                        var dataAreaItem = new DatItems.Formats.DataArea(dataArea);
                        var roms = ReadItemArray<Models.Metadata.Rom>(dataArea, Models.Metadata.DataArea.RomKey);
                        if (roms == null)
                            continue;

                        // Process flag values
                        if (dataAreaItem.GetStringFieldValue(Models.Metadata.DataArea.EndiannessKey) != null)
                            dataAreaItem.SetFieldValue<string?>(Models.Metadata.DataArea.EndiannessKey, dataAreaItem.GetStringFieldValue(Models.Metadata.DataArea.EndiannessKey).AsEnumValue<Endianness>().AsStringValue());
                        if (dataAreaItem.GetInt64FieldValue(Models.Metadata.DataArea.SizeKey) != null)
                            dataAreaItem.SetFieldValue<string?>(Models.Metadata.DataArea.SizeKey, dataAreaItem.GetInt64FieldValue(Models.Metadata.DataArea.SizeKey).ToString());
                        if (dataAreaItem.GetInt64FieldValue(Models.Metadata.DataArea.WidthKey) != null)
                            dataAreaItem.SetFieldValue<string?>(Models.Metadata.DataArea.WidthKey, dataAreaItem.GetInt64FieldValue(Models.Metadata.DataArea.WidthKey).ToString());

                        foreach (var rom in roms)
                        {
                            var romItem = new DatItems.Formats.Rom(rom);
                            romItem.SetFieldValue<DatItems.Formats.DataArea?>(DatItems.Formats.Rom.DataAreaKey, dataAreaItem);
                            romItem.SetFieldValue<DatItems.Formats.Part?>(DatItems.Formats.Rom.PartKey, partItem);
                            romItem.SetFieldValue<Source?>(DatItems.DatItem.SourceKey, source);
                            romItem.CopyMachineInformation(machine);

                            // Process flag values
                            if (romItem.GetBoolFieldValue(Models.Metadata.Rom.DisposeKey) != null)
                                romItem.SetFieldValue<string?>(Models.Metadata.Rom.DisposeKey, romItem.GetBoolFieldValue(Models.Metadata.Rom.DisposeKey).FromYesNo());
                            if (romItem.GetBoolFieldValue(Models.Metadata.Rom.InvertedKey) != null)
                                romItem.SetFieldValue<string?>(Models.Metadata.Rom.InvertedKey, romItem.GetBoolFieldValue(Models.Metadata.Rom.InvertedKey).FromYesNo());
                            if (romItem.GetStringFieldValue(Models.Metadata.Rom.LoadFlagKey) != null)
                                romItem.SetFieldValue<string?>(Models.Metadata.Rom.LoadFlagKey, romItem.GetStringFieldValue(Models.Metadata.Rom.LoadFlagKey).AsEnumValue<LoadFlag>().AsStringValue());
                            if (romItem.GetStringFieldValue(Models.Metadata.Rom.OpenMSXMediaType) != null)
                                romItem.SetFieldValue<string?>(Models.Metadata.Rom.OpenMSXMediaType, romItem.GetStringFieldValue(Models.Metadata.Rom.OpenMSXMediaType).AsEnumValue<OpenMSXSubType>().AsStringValue());
                            if (romItem.GetBoolFieldValue(Models.Metadata.Rom.MIAKey) != null)
                                romItem.SetFieldValue<string?>(Models.Metadata.Rom.MIAKey, romItem.GetBoolFieldValue(Models.Metadata.Rom.MIAKey).FromYesNo());
                            if (romItem.GetBoolFieldValue(Models.Metadata.Rom.OptionalKey) != null)
                                romItem.SetFieldValue<string?>(Models.Metadata.Rom.OptionalKey, romItem.GetBoolFieldValue(Models.Metadata.Rom.OptionalKey).FromYesNo());
                            if (romItem.GetBoolFieldValue(Models.Metadata.Rom.SoundOnlyKey) != null)
                                romItem.SetFieldValue<string?>(Models.Metadata.Rom.SoundOnlyKey, romItem.GetBoolFieldValue(Models.Metadata.Rom.SoundOnlyKey).FromYesNo());
                            if (romItem.GetStringFieldValue(Models.Metadata.Rom.StatusKey) != null)
                                romItem.SetFieldValue<string?>(Models.Metadata.Rom.StatusKey, romItem.GetStringFieldValue(Models.Metadata.Rom.StatusKey).AsEnumValue<ItemStatus>().AsStringValue());

                            // Process hash values
                            if (romItem.GetInt64FieldValue(Models.Metadata.Rom.SizeKey) != null)
                                romItem.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, romItem.GetInt64FieldValue(Models.Metadata.Rom.SizeKey).ToString());
                            if (romItem.GetStringFieldValue(Models.Metadata.Rom.CRCKey) != null)
                                romItem.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, TextHelper.NormalizeCRC32(romItem.GetStringFieldValue(Models.Metadata.Rom.CRCKey)));
                            if (romItem.GetStringFieldValue(Models.Metadata.Rom.MD2Key) != null)
                                romItem.SetFieldValue<string?>(Models.Metadata.Rom.MD2Key, TextHelper.NormalizeMD2(romItem.GetStringFieldValue(Models.Metadata.Rom.MD2Key)));
                            if (romItem.GetStringFieldValue(Models.Metadata.Rom.MD4Key) != null)
                                romItem.SetFieldValue<string?>(Models.Metadata.Rom.MD4Key, TextHelper.NormalizeMD4(romItem.GetStringFieldValue(Models.Metadata.Rom.MD4Key)));
                            if (romItem.GetStringFieldValue(Models.Metadata.Rom.MD5Key) != null)
                                romItem.SetFieldValue<string?>(Models.Metadata.Rom.MD5Key, TextHelper.NormalizeMD5(romItem.GetStringFieldValue(Models.Metadata.Rom.MD5Key)));
                            if (romItem.GetStringFieldValue(Models.Metadata.Rom.SHA1Key) != null)
                                romItem.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, TextHelper.NormalizeSHA1(romItem.GetStringFieldValue(Models.Metadata.Rom.SHA1Key)));
                            if (romItem.GetStringFieldValue(Models.Metadata.Rom.SHA256Key) != null)
                                romItem.SetFieldValue<string?>(Models.Metadata.Rom.SHA256Key, TextHelper.NormalizeSHA256(romItem.GetStringFieldValue(Models.Metadata.Rom.SHA256Key)));
                            if (romItem.GetStringFieldValue(Models.Metadata.Rom.SHA384Key) != null)
                                romItem.SetFieldValue<string?>(Models.Metadata.Rom.SHA384Key, TextHelper.NormalizeSHA384(romItem.GetStringFieldValue(Models.Metadata.Rom.SHA384Key)));
                            if (romItem.GetStringFieldValue(Models.Metadata.Rom.SHA512Key) != null)
                                romItem.SetFieldValue<string?>(Models.Metadata.Rom.SHA512Key, TextHelper.NormalizeSHA512(romItem.GetStringFieldValue(Models.Metadata.Rom.SHA512Key)));

                            Items.AddItem(romItem, statsOnly);
                            ItemsDB.AddItem(romItem, machineIndex, sourceIndex, statsOnly);
                        }
                    }
                }

                var diskAreas = ReadItemArray<Models.Metadata.DiskArea>(item, Models.Metadata.Part.DiskAreaKey);
                if (diskAreas != null)
                {
                    foreach (var diskArea in diskAreas)
                    {
                        var diskAreaitem = new DatItems.Formats.DiskArea(diskArea);
                        var disks = ReadItemArray<Models.Metadata.Disk>(diskArea, Models.Metadata.DiskArea.DiskKey);
                        if (disks == null)
                            continue;

                        foreach (var disk in disks)
                        {
                            var diskItem = new DatItems.Formats.Disk(disk);
                            diskItem.SetFieldValue<DatItems.Formats.DiskArea?>(DatItems.Formats.Disk.DiskAreaKey, diskAreaitem);
                            diskItem.SetFieldValue<DatItems.Formats.Part?>(DatItems.Formats.Disk.PartKey, partItem);
                            diskItem.SetFieldValue<Source?>(DatItems.DatItem.SourceKey, source);
                            diskItem.CopyMachineInformation(machine);

                            // Process flag values
                            if (diskItem.GetBoolFieldValue(Models.Metadata.Disk.OptionalKey) != null)
                                diskItem.SetFieldValue<string?>(Models.Metadata.Disk.OptionalKey, diskItem.GetBoolFieldValue(Models.Metadata.Disk.OptionalKey).FromYesNo());
                            if (diskItem.GetStringFieldValue(Models.Metadata.Disk.StatusKey) != null)
                                diskItem.SetFieldValue<string?>(Models.Metadata.Disk.StatusKey, diskItem.GetStringFieldValue(Models.Metadata.Disk.StatusKey).AsEnumValue<ItemStatus>().AsStringValue());
                            if (diskItem.GetBoolFieldValue(Models.Metadata.Disk.WritableKey) != null)
                                diskItem.SetFieldValue<string?>(Models.Metadata.Disk.WritableKey, diskItem.GetBoolFieldValue(Models.Metadata.Disk.WritableKey).FromYesNo());

                            // Process hash values
                            if (diskItem.GetStringFieldValue(Models.Metadata.Disk.MD5Key) != null)
                                diskItem.SetFieldValue<string?>(Models.Metadata.Disk.MD5Key, TextHelper.NormalizeMD5(diskItem.GetStringFieldValue(Models.Metadata.Disk.MD5Key)));
                            if (diskItem.GetStringFieldValue(Models.Metadata.Disk.SHA1Key) != null)
                                diskItem.SetFieldValue<string?>(Models.Metadata.Disk.SHA1Key, TextHelper.NormalizeSHA1(diskItem.GetStringFieldValue(Models.Metadata.Disk.SHA1Key)));

                            Items.AddItem(diskItem, statsOnly);
                            ItemsDB.AddItem(diskItem, machineIndex, sourceIndex, statsOnly);
                        }
                    }
                }

                var dipSwitches = ReadItemArray<Models.Metadata.DipSwitch>(item, Models.Metadata.Part.DipSwitchKey);
                if (dipSwitches != null)
                {
                    foreach (var dipSwitch in dipSwitches)
                    {
                        var dipSwitchItem = new DatItems.Formats.DipSwitch(dipSwitch);
                        dipSwitchItem.SetFieldValue<DatItems.Formats.Part?>(DatItems.Formats.DipSwitch.PartKey, partItem);
                        dipSwitchItem.SetFieldValue<Source?>(DatItems.DatItem.SourceKey, source);
                        dipSwitchItem.CopyMachineInformation(machine);

                        // Process flag values
                        if (dipSwitchItem.GetBoolFieldValue(Models.Metadata.DipSwitch.DefaultKey) != null)
                            dipSwitchItem.SetFieldValue<string?>(Models.Metadata.DipSwitch.DefaultKey, dipSwitchItem.GetBoolFieldValue(Models.Metadata.DipSwitch.DefaultKey).FromYesNo());

                        // Handle subitems
                        var condition = dipSwitch.Read<Models.Metadata.Condition>(Models.Metadata.DipSwitch.ConditionKey);
                        if (condition != null)
                        {
                            var subItem = new DatItems.Formats.Condition(condition);

                            // Process flag values
                            if (subItem.GetStringFieldValue(Models.Metadata.Condition.RelationKey) != null)
                                subItem.SetFieldValue<string?>(Models.Metadata.Condition.RelationKey, subItem.GetStringFieldValue(Models.Metadata.Condition.RelationKey).AsEnumValue<Relation>().AsStringValue());

                            dipSwitchItem.SetFieldValue<DatItems.Formats.Condition?>(Models.Metadata.DipSwitch.ConditionKey, subItem);
                        }

                        var dipLocations = ReadItemArray<Models.Metadata.DipLocation>(dipSwitch, Models.Metadata.DipSwitch.DipLocationKey);
                        if (dipLocations != null)
                        {
                            List<DatItems.Formats.DipLocation> subLocations = [];
                            foreach (var location in dipLocations)
                            {
                                var subItem = new DatItems.Formats.DipLocation(location);

                                // Process flag values
                                if (subItem.GetBoolFieldValue(Models.Metadata.DipLocation.InvertedKey) != null)
                                    subItem.SetFieldValue<string?>(Models.Metadata.DipLocation.InvertedKey, subItem.GetBoolFieldValue(Models.Metadata.DipLocation.InvertedKey).FromYesNo());

                                subLocations.Add(subItem);
                            }

                            dipSwitchItem.SetFieldValue<DatItems.Formats.DipLocation[]?>(Models.Metadata.DipSwitch.DipLocationKey, [.. subLocations]);
                        }

                        var dipValues = ReadItemArray<Models.Metadata.DipValue>(dipSwitch, Models.Metadata.DipSwitch.DipValueKey);
                        if (dipValues != null)
                        {
                            List<DatItems.Formats.DipValue> subValues = [];
                            foreach (var value in dipValues)
                            {
                                var subItem = new DatItems.Formats.DipValue(value);

                                // Process flag values
                                if (subItem.GetBoolFieldValue(Models.Metadata.DipValue.DefaultKey) != null)
                                    subItem.SetFieldValue<string?>(Models.Metadata.DipValue.DefaultKey, subItem.GetBoolFieldValue(Models.Metadata.DipValue.DefaultKey).FromYesNo());

                                var subCondition = dipSwitch.Read<Models.Metadata.Condition>(Models.Metadata.DipValue.ConditionKey);
                                if (subCondition != null)
                                {
                                    var subSubItem = new DatItems.Formats.Condition(subCondition);

                                    // Process flag values
                                    if (subSubItem.GetStringFieldValue(Models.Metadata.Condition.RelationKey) != null)
                                        subSubItem.SetFieldValue<string?>(Models.Metadata.Condition.RelationKey, subSubItem.GetStringFieldValue(Models.Metadata.Condition.RelationKey).AsEnumValue<Relation>().AsStringValue());

                                    subItem.SetFieldValue<DatItems.Formats.Condition?>(Models.Metadata.DipValue.ConditionKey, subSubItem);
                                }

                                subValues.Add(subItem);
                            }

                            dipSwitchItem.SetFieldValue<DatItems.Formats.DipValue[]?>(Models.Metadata.DipSwitch.DipValueKey, [.. subValues]);
                        }

                        Items.AddItem(dipSwitchItem, statsOnly);
                        ItemsDB.AddItem(dipSwitchItem, machineIndex, sourceIndex, statsOnly);
                    }
                }

                var partFeatures = ReadItemArray<Models.Metadata.Feature>(item, Models.Metadata.Part.FeatureKey);
                if (partFeatures != null)
                {
                    foreach (var partFeature in partFeatures)
                    {
                        var partFeatureItem = new DatItems.Formats.PartFeature(partFeature);
                        partFeatureItem.SetFieldValue<DatItems.Formats.Part?>(DatItems.Formats.DipSwitch.PartKey, partItem);
                        partFeatureItem.SetFieldValue<Source?>(DatItems.DatItem.SourceKey, source);
                        partFeatureItem.CopyMachineInformation(machine);

                        // Process flag values
                        if (partFeatureItem.GetStringFieldValue(Models.Metadata.Feature.OverallKey) != null)
                            partFeatureItem.SetFieldValue<string?>(Models.Metadata.Feature.OverallKey, partFeatureItem.GetStringFieldValue(Models.Metadata.Feature.OverallKey).AsEnumValue<FeatureStatus>().AsStringValue());
                        if (partFeatureItem.GetStringFieldValue(Models.Metadata.Feature.StatusKey) != null)
                            partFeatureItem.SetFieldValue<string?>(Models.Metadata.Feature.StatusKey, partFeatureItem.GetStringFieldValue(Models.Metadata.Feature.StatusKey).AsEnumValue<FeatureStatus>().AsStringValue());
                        if (partFeatureItem.GetStringFieldValue(Models.Metadata.Feature.FeatureTypeKey) != null)
                            partFeatureItem.SetFieldValue<string?>(Models.Metadata.Feature.FeatureTypeKey, partFeatureItem.GetStringFieldValue(Models.Metadata.Feature.FeatureTypeKey).AsEnumValue<FeatureType>().AsStringValue());

                        Items.AddItem(partFeatureItem, statsOnly);
                        ItemsDB.AddItem(partFeatureItem, machineIndex, sourceIndex, statsOnly);
                    }
                }
            }
        }

        /// <summary>
        /// Convert Port information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="machineIndex">Index of the Machine to use with the converted items</param>
        /// <param name="source">Source to use with the converted items</param>
        /// <param name="sourceIndex">Index of the Source to use with the converted items</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Port[]? items, Machine machine, long machineIndex, Source source, long sourceIndex, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Port(item);
                datItem.SetFieldValue<Source?>(DatItems.DatItem.SourceKey, source);
                datItem.CopyMachineInformation(machine);

                // Handle subitems
                var analogs = ReadItemArray<Models.Metadata.Analog>(item, Models.Metadata.Port.AnalogKey);
                if (analogs != null)
                {
                    List<DatItems.Formats.Analog> subAnalogs = [];
                    foreach (var analog in analogs)
                    {
                        var subItem = new DatItems.Formats.Analog(analog);
                        subAnalogs.Add(subItem);
                    }

                    datItem.SetFieldValue<DatItems.Formats.Analog[]?>(Models.Metadata.Port.AnalogKey, [.. subAnalogs]);
                }

                Items.AddItem(datItem, statsOnly);
                ItemsDB.AddItem(datItem, machineIndex, sourceIndex, statsOnly);
            }
        }

        /// <summary>
        /// Convert RamOption information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="machineIndex">Index of the Machine to use with the converted items</param>
        /// <param name="source">Source to use with the converted items</param>
        /// <param name="sourceIndex">Index of the Source to use with the converted items</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.RamOption[]? items, Machine machine, long machineIndex, Source source, long sourceIndex, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.RamOption(item);
                datItem.SetFieldValue<Source?>(DatItems.DatItem.SourceKey, source);
                datItem.CopyMachineInformation(machine);

                // Process flag values
                if (datItem.GetBoolFieldValue(Models.Metadata.RamOption.DefaultKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.RamOption.DefaultKey, datItem.GetBoolFieldValue(Models.Metadata.RamOption.DefaultKey).FromYesNo());

                Items.AddItem(datItem, statsOnly);
                ItemsDB.AddItem(datItem, machineIndex, sourceIndex, statsOnly);
            }
        }

        /// <summary>
        /// Convert Release information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="machineIndex">Index of the Machine to use with the converted items</param>
        /// <param name="source">Source to use with the converted items</param>
        /// <param name="sourceIndex">Index of the Source to use with the converted items</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Release[]? items, Machine machine, long machineIndex, Source source, long sourceIndex, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Release(item);
                datItem.SetFieldValue<Source?>(DatItems.DatItem.SourceKey, source);
                datItem.CopyMachineInformation(machine);

                // Process flag values
                if (datItem.GetBoolFieldValue(Models.Metadata.Release.DefaultKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Release.DefaultKey, datItem.GetBoolFieldValue(Models.Metadata.Release.DefaultKey).FromYesNo());

                Items.AddItem(datItem, statsOnly);
                ItemsDB.AddItem(datItem, machineIndex, sourceIndex, statsOnly);
            }
        }

        /// <summary>
        /// Convert Rom information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="machineIndex">Index of the Machine to use with the converted items</param>
        /// <param name="source">Source to use with the converted items</param>
        /// <param name="sourceIndex">Index of the Source to use with the converted items</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Rom[]? items, Machine machine, long machineIndex, Source source, long sourceIndex, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Rom(item);
                datItem.SetFieldValue<Source?>(DatItems.DatItem.SourceKey, source);
                datItem.CopyMachineInformation(machine);

                // Process flag values
                if (datItem.GetBoolFieldValue(Models.Metadata.Rom.DisposeKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Rom.DisposeKey, datItem.GetBoolFieldValue(Models.Metadata.Rom.DisposeKey).FromYesNo());
                if (datItem.GetBoolFieldValue(Models.Metadata.Rom.InvertedKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Rom.InvertedKey, datItem.GetBoolFieldValue(Models.Metadata.Rom.InvertedKey).FromYesNo());
                if (datItem.GetStringFieldValue(Models.Metadata.Rom.LoadFlagKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Rom.LoadFlagKey, datItem.GetStringFieldValue(Models.Metadata.Rom.LoadFlagKey).AsEnumValue<LoadFlag>().AsStringValue());
                if (datItem.GetStringFieldValue(Models.Metadata.Rom.OpenMSXMediaType) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Rom.OpenMSXMediaType, datItem.GetStringFieldValue(Models.Metadata.Rom.OpenMSXMediaType).AsEnumValue<OpenMSXSubType>().AsStringValue());
                if (datItem.GetBoolFieldValue(Models.Metadata.Rom.MIAKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Rom.MIAKey, datItem.GetBoolFieldValue(Models.Metadata.Rom.MIAKey).FromYesNo());
                if (datItem.GetBoolFieldValue(Models.Metadata.Rom.OptionalKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Rom.OptionalKey, datItem.GetBoolFieldValue(Models.Metadata.Rom.OptionalKey).FromYesNo());
                if (datItem.GetBoolFieldValue(Models.Metadata.Rom.SoundOnlyKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Rom.SoundOnlyKey, datItem.GetBoolFieldValue(Models.Metadata.Rom.SoundOnlyKey).FromYesNo());
                if (datItem.GetStringFieldValue(Models.Metadata.Rom.StatusKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Rom.StatusKey, datItem.GetStringFieldValue(Models.Metadata.Rom.StatusKey).AsEnumValue<ItemStatus>().AsStringValue());

                // Process hash values
                if (datItem.GetInt64FieldValue(Models.Metadata.Rom.SizeKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, datItem.GetInt64FieldValue(Models.Metadata.Rom.SizeKey).ToString());
                if (datItem.GetStringFieldValue(Models.Metadata.Rom.CRCKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, TextHelper.NormalizeCRC32(datItem.GetStringFieldValue(Models.Metadata.Rom.CRCKey)));
                if (datItem.GetStringFieldValue(Models.Metadata.Rom.MD2Key) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Rom.MD2Key, TextHelper.NormalizeMD2(datItem.GetStringFieldValue(Models.Metadata.Rom.MD2Key)));
                if (datItem.GetStringFieldValue(Models.Metadata.Rom.MD4Key) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Rom.MD4Key, TextHelper.NormalizeMD4(datItem.GetStringFieldValue(Models.Metadata.Rom.MD4Key)));
                if (datItem.GetStringFieldValue(Models.Metadata.Rom.MD5Key) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Rom.MD5Key, TextHelper.NormalizeMD5(datItem.GetStringFieldValue(Models.Metadata.Rom.MD5Key)));
                if (datItem.GetStringFieldValue(Models.Metadata.Rom.SHA1Key) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, TextHelper.NormalizeSHA1(datItem.GetStringFieldValue(Models.Metadata.Rom.SHA1Key)));
                if (datItem.GetStringFieldValue(Models.Metadata.Rom.SHA256Key) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Rom.SHA256Key, TextHelper.NormalizeSHA256(datItem.GetStringFieldValue(Models.Metadata.Rom.SHA256Key)));
                if (datItem.GetStringFieldValue(Models.Metadata.Rom.SHA384Key) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Rom.SHA384Key, TextHelper.NormalizeSHA384(datItem.GetStringFieldValue(Models.Metadata.Rom.SHA384Key)));
                if (datItem.GetStringFieldValue(Models.Metadata.Rom.SHA512Key) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Rom.SHA512Key, TextHelper.NormalizeSHA512(datItem.GetStringFieldValue(Models.Metadata.Rom.SHA512Key)));

                Items.AddItem(datItem, statsOnly);
                ItemsDB.AddItem(datItem, machineIndex, sourceIndex, statsOnly);
            }
        }

        /// <summary>
        /// Convert Sample information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="machineIndex">Index of the Machine to use with the converted items</param>
        /// <param name="source">Source to use with the converted items</param>
        /// <param name="sourceIndex">Index of the Source to use with the converted items</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Sample[]? items, Machine machine, long machineIndex, Source source, long sourceIndex, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Sample(item);
                datItem.SetFieldValue<Source?>(DatItems.DatItem.SourceKey, source);
                datItem.CopyMachineInformation(machine);
                Items.AddItem(datItem, statsOnly);
                ItemsDB.AddItem(datItem, machineIndex, sourceIndex, statsOnly);
            }
        }

        /// <summary>
        /// Convert SharedFeat information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="machineIndex">Index of the Machine to use with the converted items</param>
        /// <param name="source">Source to use with the converted items</param>
        /// <param name="sourceIndex">Index of the Source to use with the converted items</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.SharedFeat[]? items, Machine machine, long machineIndex, Source source, long sourceIndex, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.SharedFeat(item);
                datItem.SetFieldValue<Source?>(DatItems.DatItem.SourceKey, source);
                datItem.CopyMachineInformation(machine);
                Items.AddItem(datItem, statsOnly);
                ItemsDB.AddItem(datItem, machineIndex, sourceIndex, statsOnly);
            }
        }

        /// <summary>
        /// Convert Slot information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="machineIndex">Index of the Machine to use with the converted items</param>
        /// <param name="source">Source to use with the converted items</param>
        /// <param name="sourceIndex">Index of the Source to use with the converted items</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Slot[]? items, Machine machine, long machineIndex, Source source, long sourceIndex, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Slot(item);
                datItem.SetFieldValue<Source?>(DatItems.DatItem.SourceKey, source);
                datItem.CopyMachineInformation(machine);

                // Handle subitems
                var slotOptions = ReadItemArray<Models.Metadata.SlotOption>(item, Models.Metadata.Slot.SlotOptionKey);
                if (slotOptions != null)
                {
                    List<DatItems.Formats.SlotOption> subOptions = [];
                    foreach (var slotOption in slotOptions)
                    {
                        var subItem = new DatItems.Formats.SlotOption(slotOption);

                        // Process flag values
                        if (subItem.GetBoolFieldValue(Models.Metadata.SlotOption.DefaultKey) != null)
                            subItem.SetFieldValue<string?>(Models.Metadata.SlotOption.DefaultKey, subItem.GetBoolFieldValue(Models.Metadata.SlotOption.DefaultKey).FromYesNo());

                        subOptions.Add(subItem);
                    }

                    datItem.SetFieldValue<DatItems.Formats.SlotOption[]?>(Models.Metadata.Slot.SlotOptionKey, [.. subOptions]);
                }

                Items.AddItem(datItem, statsOnly);
                ItemsDB.AddItem(datItem, machineIndex, sourceIndex, statsOnly);
            }
        }

        /// <summary>
        /// Convert SoftwareList information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="machineIndex">Index of the Machine to use with the converted items</param>
        /// <param name="source">Source to use with the converted items</param>
        /// <param name="sourceIndex">Index of the Source to use with the converted items</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.SoftwareList[]? items, Machine machine, long machineIndex, Source source, long sourceIndex, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.SoftwareList(item);
                datItem.SetFieldValue<Source?>(DatItems.DatItem.SourceKey, source);
                datItem.CopyMachineInformation(machine);

                // Process flag values
                if (datItem.GetStringFieldValue(Models.Metadata.SoftwareList.StatusKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.SoftwareList.StatusKey, datItem.GetStringFieldValue(Models.Metadata.SoftwareList.StatusKey).AsEnumValue<SoftwareListStatus>().AsStringValue());

                Items.AddItem(datItem, statsOnly);
                ItemsDB.AddItem(datItem, machineIndex, sourceIndex, statsOnly);
            }
        }

        /// <summary>
        /// Convert Sound information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="machineIndex">Index of the Machine to use with the converted items</param>
        /// <param name="source">Source to use with the converted items</param>
        /// <param name="sourceIndex">Index of the Source to use with the converted items</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Sound[]? items, Machine machine, long machineIndex, Source source, long sourceIndex, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Sound(item);
                datItem.SetFieldValue<Source?>(DatItems.DatItem.SourceKey, source);
                datItem.CopyMachineInformation(machine);

                // Process flag values
                if (datItem.GetInt64FieldValue(Models.Metadata.Sound.ChannelsKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Sound.ChannelsKey, datItem.GetInt64FieldValue(Models.Metadata.Sound.ChannelsKey).ToString());

                Items.AddItem(datItem, statsOnly);
                ItemsDB.AddItem(datItem, machineIndex, sourceIndex, statsOnly);
            }
        }

        /// <summary>
        /// Convert Video information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="machineIndex">Index of the Machine to use with the converted items</param>
        /// <param name="source">Source to use with the converted items</param>
        /// <param name="sourceIndex">Index of the Source to use with the converted items</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Video[]? items, Machine machine, long machineIndex, Source source, long sourceIndex, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Display(item);
                datItem.SetFieldValue<Source?>(DatItems.DatItem.SourceKey, source);
                datItem.CopyMachineInformation(machine);

                // Process flag values
                if (datItem.GetInt64FieldValue(Models.Metadata.Video.AspectXKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Video.AspectXKey, datItem.GetInt64FieldValue(Models.Metadata.Video.AspectXKey).ToString());
                if (datItem.GetInt64FieldValue(Models.Metadata.Video.AspectYKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Video.AspectYKey, datItem.GetInt64FieldValue(Models.Metadata.Video.AspectYKey).ToString());
                if (datItem.GetInt64FieldValue(Models.Metadata.Video.HeightKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Display.HeightKey, datItem.GetInt64FieldValue(Models.Metadata.Video.HeightKey).ToString());
                if (datItem.GetDoubleFieldValue(Models.Metadata.Video.RefreshKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Display.RefreshKey, datItem.GetDoubleFieldValue(Models.Metadata.Video.RefreshKey).ToString());
                if (datItem.GetStringFieldValue(Models.Metadata.Video.ScreenKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Display.DisplayTypeKey, datItem.GetStringFieldValue(Models.Metadata.Video.ScreenKey).AsEnumValue<DisplayType>().AsStringValue());
                if (datItem.GetInt64FieldValue(Models.Metadata.Video.WidthKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Display.WidthKey, datItem.GetInt64FieldValue(Models.Metadata.Video.WidthKey).ToString());

                Items.AddItem(datItem, statsOnly);
                ItemsDB.AddItem(datItem, machineIndex, sourceIndex, statsOnly);
            }
        }

        /// <summary>
        /// Read an item array from a given key, if possible
        /// </summary>
        private static T[]? ReadItemArray<T>(Models.Metadata.DictionaryBase item, string key) where T : Models.Metadata.DictionaryBase
        {
            var items = item.Read<T[]>(key);
            if (items == default)
            {
                var single = item.Read<T>(key);
                if (single != default)
                    items = [single];
            }

            return items;
        }

        #endregion
    }
}