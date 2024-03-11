using System.Collections.Generic;
using System.Linq;
using SabreTools.Core;
using SabreTools.Core.Tools;

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
        public void ConvertMetadata(Models.Metadata.MetadataFile? item, string filename, int indexId, bool keep, bool statsOnly)
        {
            // If the metadata file is invalid, we can't do anything
            if (item == null || !item.Any())
                return;

            // Get the header from the metadata
            var header = item.Read<Models.Metadata.Header>(Models.Metadata.MetadataFile.HeaderKey);
            if (header != null)
                ConvertHeader(header, keep);

            // Get the machines from the metadata
            var machines = ReadItemArray<Models.Metadata.Machine>(item, Models.Metadata.MetadataFile.MachineKey);
            if (machines != null)
                ConvertMachines(machines, filename, indexId, statsOnly);
        }

        /// <summary>
        /// Convert header information
        /// </summary>
        /// <param name="item">Header to convert</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise</param>
        private void ConvertHeader(Models.Metadata.Header? item, bool keep)
        {
            // If the header is invalid, we can't do anything
            if (item == null || !item.Any())
                return;

            // Create an internal header
            var header = new DatFiles.DatHeader(item);

            // Convert subheader values
            if (item.ContainsKey(Models.Metadata.Header.CanOpenKey))
            {
                var canOpen = item.Read<Models.OfflineList.CanOpen>(Models.Metadata.Header.CanOpenKey);
                if (canOpen?.Extension != null)
                    Header.SetFieldValue<string[]?>(Models.Metadata.Header.CanOpenKey, canOpen.Extension);
            }
            if (item.ContainsKey(Models.Metadata.Header.ImagesKey))
            {
                // TODO: Add to internal model
            }
            if (item.ContainsKey(Models.Metadata.Header.InfosKey))
            {
                var infos = item.Read<Models.OfflineList.Infos>(Models.Metadata.Header.InfosKey);
                if (infos != null)
                {
                    var offlineListInfos = new List<Formats.OfflineListInfo>();

                    if (infos.Title != null)
                    {
                        offlineListInfos.Add(new Formats.OfflineListInfo
                        {
                            Name = "title",
                            Visible = infos.Title.Visible.AsYesNo(),
                            InNamingOption = infos.Title.InNamingOption.AsYesNo(),
                            Default = infos.Title.Default.AsYesNo(),
                        });
                    }
                    if (infos.Location != null)
                    {
                        offlineListInfos.Add(new Formats.OfflineListInfo
                        {
                            Name = "location",
                            Visible = infos.Location.Visible.AsYesNo(),
                            InNamingOption = infos.Location.InNamingOption.AsYesNo(),
                            Default = infos.Location.Default.AsYesNo(),
                        });
                    }
                    if (infos.Publisher != null)
                    {
                        offlineListInfos.Add(new Formats.OfflineListInfo
                        {
                            Name = "publisher",
                            Visible = infos.Publisher.Visible.AsYesNo(),
                            InNamingOption = infos.Publisher.InNamingOption.AsYesNo(),
                            Default = infos.Publisher.Default.AsYesNo(),
                        });
                    }
                    if (infos.SourceRom != null)
                    {
                        offlineListInfos.Add(new Formats.OfflineListInfo
                        {
                            Name = "sourceRom",
                            Visible = infos.SourceRom.Visible.AsYesNo(),
                            InNamingOption = infos.SourceRom.InNamingOption.AsYesNo(),
                            Default = infos.SourceRom.Default.AsYesNo(),
                        });
                    }
                    if (infos.SaveType != null)
                    {
                        offlineListInfos.Add(new Formats.OfflineListInfo
                        {
                            Name = "saveType",
                            Visible = infos.SaveType.Visible.AsYesNo(),
                            InNamingOption = infos.SaveType.InNamingOption.AsYesNo(),
                            Default = infos.SaveType.Default.AsYesNo(),
                        });
                    }
                    if (infos.RomSize != null)
                    {
                        offlineListInfos.Add(new Formats.OfflineListInfo
                        {
                            Name = "romSize",
                            Visible = infos.RomSize.Visible.AsYesNo(),
                            InNamingOption = infos.RomSize.InNamingOption.AsYesNo(),
                            Default = infos.RomSize.Default.AsYesNo(),
                        });
                    }
                    if (infos.ReleaseNumber != null)
                    {
                        offlineListInfos.Add(new Formats.OfflineListInfo
                        {
                            Name = "releaseNumber",
                            Visible = infos.ReleaseNumber.Visible.AsYesNo(),
                            InNamingOption = infos.ReleaseNumber.InNamingOption.AsYesNo(),
                            Default = infos.ReleaseNumber.Default.AsYesNo(),
                        });
                    }
                    if (infos.LanguageNumber != null)
                    {
                        offlineListInfos.Add(new Formats.OfflineListInfo
                        {
                            Name = "languageNumber",
                            Visible = infos.LanguageNumber.Visible.AsYesNo(),
                            InNamingOption = infos.LanguageNumber.InNamingOption.AsYesNo(),
                            Default = infos.LanguageNumber.Default.AsYesNo(),
                        });
                    }
                    if (infos.Comment != null)
                    {
                        offlineListInfos.Add(new Formats.OfflineListInfo
                        {
                            Name = "comment",
                            Visible = infos.Comment.Visible.AsYesNo(),
                            InNamingOption = infos.Comment.InNamingOption.AsYesNo(),
                            Default = infos.Comment.Default.AsYesNo(),
                        });
                    }
                    if (infos.RomCRC != null)
                    {
                        offlineListInfos.Add(new Formats.OfflineListInfo
                        {
                            Name = "romCRC",
                            Visible = infos.RomCRC.Visible.AsYesNo(),
                            InNamingOption = infos.RomCRC.InNamingOption.AsYesNo(),
                            Default = infos.RomCRC.Default.AsYesNo(),
                        });
                    }
                    if (infos.Im1CRC != null)
                    {
                        offlineListInfos.Add(new Formats.OfflineListInfo
                        {
                            Name = "im1CRC",
                            Visible = infos.Im1CRC.Visible.AsYesNo(),
                            InNamingOption = infos.Im1CRC.InNamingOption.AsYesNo(),
                            Default = infos.Im1CRC.Default.AsYesNo(),
                        });
                    }
                    if (infos.Im2CRC != null)
                    {
                        offlineListInfos.Add(new Formats.OfflineListInfo
                        {
                            Name = "im2CRC",
                            Visible = infos.Im2CRC.Visible.AsYesNo(),
                            InNamingOption = infos.Im2CRC.InNamingOption.AsYesNo(),
                            Default = infos.Im2CRC.Default.AsYesNo(),
                        });
                    }
                    if (infos.Languages != null)
                    {
                        offlineListInfos.Add(new Formats.OfflineListInfo
                        {
                            Name = "languages",
                            Visible = infos.Languages.Visible.AsYesNo(),
                            InNamingOption = infos.Languages.InNamingOption.AsYesNo(),
                            Default = infos.Languages.Default.AsYesNo(),
                        });
                    }

                    Header.SetFieldValue<Formats.OfflineListInfo[]?>(Models.Metadata.Header.InfosKey, [.. offlineListInfos]);
                }
            }
            if (item.ContainsKey(Models.Metadata.Header.NewDatKey))
            {
                var newDat = item.Read<Models.OfflineList.NewDat>(Models.Metadata.Header.NewDatKey);
                if (newDat != null)
                {
                    Header.SetFieldValue<string?>("DATVERSIONURL", newDat.DatVersionUrl);
                    //Header.SetFieldValue<Models.OfflineList.DatUrl?>("DATURL", newDat.DatUrl); // TODO: Add to internal model
                    Header.SetFieldValue<string?>("IMURL", newDat.ImUrl);
                }
            }
            if (item.ContainsKey(Models.Metadata.Header.SearchKey))
            {
                // TODO: Add to internal model
            }

            // Selectively set all possible fields -- TODO: Figure out how to make this less manual
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.AuthorKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.AuthorKey, header.GetFieldValue<string?>(Models.Metadata.Header.AuthorKey));
            if (Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.BiosModeKey) == MergingFlag.None)
                Header.SetFieldValue<MergingFlag>(Models.Metadata.Header.BiosModeKey, header.GetFieldValue<MergingFlag>(Models.Metadata.Header.BiosModeKey));
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.BuildKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.BuildKey, header.GetFieldValue<string?>(Models.Metadata.Header.BuildKey));
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.CategoryKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.CategoryKey, header.GetFieldValue<string?>(Models.Metadata.Header.CategoryKey));
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.CommentKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.CommentKey, header.GetFieldValue<string?>(Models.Metadata.Header.CommentKey));
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.DateKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.DateKey, header.GetFieldValue<string?>(Models.Metadata.Header.DateKey));
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.DatVersionKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.DatVersionKey, header.GetFieldValue<string?>(Models.Metadata.Header.DatVersionKey));
            if (Header.GetFieldValue<bool?>(Models.Metadata.Header.DebugKey) == null)
                Header.SetFieldValue<bool?>(Models.Metadata.Header.DebugKey, header.GetFieldValue<bool?>(Models.Metadata.Header.DebugKey));
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, header.GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey));
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.EmailKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.EmailKey, header.GetFieldValue<string?>(Models.Metadata.Header.EmailKey));
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.EmulatorVersionKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.EmulatorVersionKey, header.GetFieldValue<string?>(Models.Metadata.Header.EmulatorVersionKey));
            if (Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.ForceMergingKey) == MergingFlag.None)
                Header.SetFieldValue<MergingFlag>(Models.Metadata.Header.ForceMergingKey, header.GetFieldValue<MergingFlag>(Models.Metadata.Header.ForceMergingKey));
            if (Header.GetFieldValue<NodumpFlag>(Models.Metadata.Header.ForceNodumpKey) == NodumpFlag.None)
                Header.SetFieldValue<NodumpFlag>(Models.Metadata.Header.ForceNodumpKey, header.GetFieldValue<NodumpFlag>(Models.Metadata.Header.ForceNodumpKey));
            if (Header.GetFieldValue<PackingFlag>(Models.Metadata.Header.ForcePackingKey) == PackingFlag.None)
                Header.SetFieldValue<PackingFlag>(Models.Metadata.Header.ForcePackingKey, header.GetFieldValue<PackingFlag>(Models.Metadata.Header.ForcePackingKey));
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.HeaderKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.HeaderKey, header.GetFieldValue<string?>(Models.Metadata.Header.HeaderKey));
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.HomepageKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.HomepageKey, header.GetFieldValue<string?>(Models.Metadata.Header.HomepageKey));
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.IdKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.IdKey, header.GetFieldValue<string?>(Models.Metadata.Header.IdKey));
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.ImFolderKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.ImFolderKey, header.GetFieldValue<string?>(Models.Metadata.Header.ImFolderKey));
            if (Header.GetFieldValue<bool?>(Models.Metadata.Header.LockBiosModeKey) == null)
                Header.SetFieldValue<bool?>(Models.Metadata.Header.LockBiosModeKey, header.GetFieldValue<bool?>(Models.Metadata.Header.LockBiosModeKey));
            if (Header.GetFieldValue<bool?>(Models.Metadata.Header.LockRomModeKey) == null)
                Header.SetFieldValue<bool?>(Models.Metadata.Header.LockRomModeKey, header.GetFieldValue<bool?>(Models.Metadata.Header.LockRomModeKey));
            if (Header.GetFieldValue<bool?>(Models.Metadata.Header.LockSampleModeKey) == null)
                Header.SetFieldValue<bool?>(Models.Metadata.Header.LockSampleModeKey, header.GetFieldValue<bool?>(Models.Metadata.Header.LockSampleModeKey));
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.MameConfigKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.MameConfigKey, header.GetFieldValue<string?>(Models.Metadata.Header.NameKey));
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, header.GetFieldValue<string?>(Models.Metadata.Header.MameConfigKey));
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.PluginKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.PluginKey, header.GetFieldValue<string?>(Models.Metadata.Header.PluginKey));
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.RefNameKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.RefNameKey, header.GetFieldValue<string?>(Models.Metadata.Header.RefNameKey));
            if (Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.RomModeKey) == MergingFlag.None)
                Header.SetFieldValue<MergingFlag>(Models.Metadata.Header.RomModeKey, header.GetFieldValue<MergingFlag>(Models.Metadata.Header.RomModeKey));
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.RomTitleKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.RomTitleKey, header.GetFieldValue<string?>(Models.Metadata.Header.RomTitleKey));
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.RootDirKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.RootDirKey, header.GetFieldValue<string?>(Models.Metadata.Header.RootDirKey));
            if (Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.SampleModeKey) == MergingFlag.None)
                Header.SetFieldValue<MergingFlag>(Models.Metadata.Header.SampleModeKey, header.GetFieldValue<MergingFlag>(Models.Metadata.Header.SampleModeKey));
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.ScreenshotsHeightKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.ScreenshotsHeightKey, header.GetFieldValue<string?>(Models.Metadata.Header.ScreenshotsHeightKey));
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.ScreenshotsWidthKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.ScreenshotsWidthKey, header.GetFieldValue<string?>(Models.Metadata.Header.ScreenshotsWidthKey));
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.SystemKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.SystemKey, header.GetFieldValue<string?>(Models.Metadata.Header.SystemKey));
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.TypeKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.TypeKey, header.GetFieldValue<string?>(Models.Metadata.Header.TypeKey));
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.UrlKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.UrlKey, header.GetFieldValue<string?>(Models.Metadata.Header.UrlKey));
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.VersionKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.VersionKey, header.GetFieldValue<string?>(Models.Metadata.Header.VersionKey));

            // Handle implied SuperDAT
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey)?.Contains(" - SuperDAT") == true && keep)
            {
                if (Header.GetFieldValue<string?>(Models.Metadata.Header.TypeKey) == null)
                    Header.SetFieldValue<string?>(Models.Metadata.Header.TypeKey, "SuperDAT");
            }
        }

        /// <summary>
        /// Convert machines information
        /// </summary>
        /// <param name="items">Machine array to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertMachines(Models.Metadata.Machine[]? items, string filename, int indexId, bool statsOnly)
        {
            // If the array is invalid, we can't do anything
            if (items == null || !items.Any())
                return;

            // Loop through the machines and add
            foreach (var machine in items)
            {
                ConvertMachine(machine, filename, indexId, statsOnly);
            }
        }

        /// <summary>
        /// Convert machine information
        /// </summary>
        /// <param name="item">Machine to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertMachine(Models.Metadata.Machine? item, string filename, int indexId, bool statsOnly)
        {
            // If the machine is invalid, we can't do anything
            if (item == null || !item.Any())
                return;

            // Create an internal machine
            var machine = new DatItems.Machine(item);

            // Convert items in the machine
            if (item.ContainsKey(Models.Metadata.Machine.AdjusterKey))
            {
                var items = ReadItemArray<Models.Metadata.Adjuster>(item, Models.Metadata.Machine.AdjusterKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.ArchiveKey))
            {
                var items = ReadItemArray<Models.Metadata.Archive>(item, Models.Metadata.Machine.ArchiveKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.BiosSetKey))
            {
                var items = ReadItemArray<Models.Metadata.BiosSet>(item, Models.Metadata.Machine.BiosSetKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.ChipKey))
            {
                var items = ReadItemArray<Models.Metadata.Chip>(item, Models.Metadata.Machine.ChipKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.ConfigurationKey))
            {
                var items = ReadItemArray<Models.Metadata.Configuration>(item, Models.Metadata.Machine.ConfigurationKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.DeviceKey))
            {
                var items = ReadItemArray<Models.Metadata.Device>(item, Models.Metadata.Machine.DeviceKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.DeviceRefKey))
            {
                var items = ReadItemArray<Models.Metadata.DeviceRef>(item, Models.Metadata.Machine.DeviceRefKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.DipSwitchKey))
            {
                var items = ReadItemArray<Models.Metadata.DipSwitch>(item, Models.Metadata.Machine.DipSwitchKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.DiskKey))
            {
                var items = ReadItemArray<Models.Metadata.Disk>(item, Models.Metadata.Machine.DiskKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.DisplayKey))
            {
                var items = ReadItemArray<Models.Metadata.Display>(item, Models.Metadata.Machine.DisplayKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.DriverKey))
            {
                var items = ReadItemArray<Models.Metadata.Driver>(item, Models.Metadata.Machine.DriverKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.DumpKey))
            {
                var items = ReadItemArray<Models.Metadata.Dump>(item, Models.Metadata.Machine.DumpKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.FeatureKey))
            {
                var items = ReadItemArray<Models.Metadata.Feature>(item, Models.Metadata.Machine.FeatureKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.InfoKey))
            {
                var items = ReadItemArray<Models.Metadata.Info>(item, Models.Metadata.Machine.InfoKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.InputKey))
            {
                var items = ReadItemArray<Models.Metadata.Input>(item, Models.Metadata.Machine.InputKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.MediaKey))
            {
                var items = ReadItemArray<Models.Metadata.Media>(item, Models.Metadata.Machine.MediaKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.PartKey))
            {
                var items = ReadItemArray<Models.Metadata.Part>(item, Models.Metadata.Machine.PartKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.PortKey))
            {
                var items = ReadItemArray<Models.Metadata.Port>(item, Models.Metadata.Machine.PortKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.RamOptionKey))
            {
                var items = ReadItemArray<Models.Metadata.RamOption>(item, Models.Metadata.Machine.RamOptionKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.ReleaseKey))
            {
                var items = ReadItemArray<Models.Metadata.Release>(item, Models.Metadata.Machine.ReleaseKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.RomKey))
            {
                var items = ReadItemArray<Models.Metadata.Rom>(item, Models.Metadata.Machine.RomKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.SampleKey))
            {
                var items = ReadItemArray<Models.Metadata.Sample>(item, Models.Metadata.Machine.SampleKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.SharedFeatKey))
            {
                var items = ReadItemArray<Models.Metadata.SharedFeat>(item, Models.Metadata.Machine.SharedFeatKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.SlotKey))
            {
                var items = ReadItemArray<Models.Metadata.Slot>(item, Models.Metadata.Machine.SlotKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.SoftwareListKey))
            {
                var items = ReadItemArray<Models.Metadata.SoftwareList>(item, Models.Metadata.Machine.SoftwareListKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.SoundKey))
            {
                var items = ReadItemArray<Models.Metadata.Sound>(item, Models.Metadata.Machine.SoundKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.TruripKey))
            {
                // TODO: Figure out what this maps to
            }
            if (item.ContainsKey(Models.Metadata.Machine.VideoKey))
            {
                var items = ReadItemArray<Models.Metadata.Video>(item, Models.Metadata.Machine.VideoKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
        }

        /// <summary>
        /// Convert Adjuster information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Adjuster[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Adjuster(item);
                datItem.SetFieldValue<DatItems.Source?>(DatItems.DatItem.SourceKey, new DatItems.Source { Index = indexId, Name = filename });
                datItem.CopyMachineInformation(machine);

                // Handle subitems
                var condition = item.Read<Models.Metadata.Condition>(Models.Metadata.Adjuster.ConditionKey);
                if (condition != null)
                {
                    var subItem = new DatItems.Formats.Condition(condition);
                    datItem.SetFieldValue<DatItems.Formats.Condition?>(Models.Metadata.Adjuster.ConditionKey, subItem);
                }

                ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert Archive information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Archive[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Archive(item);
                datItem.SetFieldValue<DatItems.Source?>(DatItems.DatItem.SourceKey, new DatItems.Source { Index = indexId, Name = filename });
                datItem.CopyMachineInformation(machine);
                ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert BiosSet information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.BiosSet[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.BiosSet(item);
                datItem.SetFieldValue<DatItems.Source?>(DatItems.DatItem.SourceKey, new DatItems.Source { Index = indexId, Name = filename });
                datItem.CopyMachineInformation(machine);
                ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert Chip information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Chip[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Chip(item);
                datItem.SetFieldValue<DatItems.Source?>(DatItems.DatItem.SourceKey, new DatItems.Source { Index = indexId, Name = filename });
                datItem.CopyMachineInformation(machine);
                ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert Configuration information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Configuration[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Configuration(item);
                datItem.SetFieldValue<DatItems.Source?>(DatItems.DatItem.SourceKey, new DatItems.Source { Index = indexId, Name = filename });
                datItem.CopyMachineInformation(machine);

                // Handle subitems
                var condition = item.Read<Models.Metadata.Condition>(Models.Metadata.Configuration.ConditionKey);
                if (condition != null)
                {
                    var subItem = new DatItems.Formats.Condition(condition);
                    datItem.SetFieldValue<DatItems.Formats.Condition?>(Models.Metadata.Configuration.ConditionKey, subItem);
                }

                var confLocations = ReadItemArray<Models.Metadata.ConfLocation>(item, Models.Metadata.Configuration.ConfLocationKey);
                if (confLocations != null)
                {
                    var subLocations = new List<DatItems.Formats.ConfLocation>();
                    foreach (var location in confLocations)
                    {
                        var subItem = new DatItems.Formats.ConfLocation(location);
                        subLocations.Add(subItem);
                    }

                    datItem.SetFieldValue<DatItems.Formats.ConfLocation[]?>(Models.Metadata.Configuration.ConfLocationKey, [.. subLocations]);
                }

                var confSettings = ReadItemArray<Models.Metadata.ConfSetting>(item, Models.Metadata.Configuration.ConfSettingKey);
                if (confSettings != null)
                {
                    var subValues = new List<DatItems.Formats.ConfSetting>();
                    foreach (var setting in confSettings)
                    {
                        var subItem = new DatItems.Formats.ConfSetting(setting);

                        var subCondition = item.Read<Models.Metadata.Condition>(Models.Metadata.ConfSetting.ConditionKey);
                        if (subCondition != null)
                        {
                            var subSubItem = new DatItems.Formats.Condition(subCondition);
                            subItem.SetFieldValue<DatItems.Formats.Condition?>(Models.Metadata.ConfSetting.ConditionKey, subSubItem);
                        }

                        subValues.Add(subItem);
                    }

                    datItem.SetFieldValue<DatItems.Formats.ConfSetting[]?>(Models.Metadata.Configuration.ConfSettingKey, [.. subValues]);
                }

                ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert Device information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Device[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Device(item);
                datItem.SetFieldValue<DatItems.Source?>(DatItems.DatItem.SourceKey, new DatItems.Source { Index = indexId, Name = filename });
                datItem.CopyMachineInformation(machine);

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
                    var subExtensions = new List<DatItems.Formats.Extension>();
                    foreach (var extension in extensions)
                    {
                        var subItem = new DatItems.Formats.Extension(extension);
                        subExtensions.Add(subItem);
                    }

                    datItem.SetFieldValue<DatItems.Formats.Extension[]?>(Models.Metadata.Device.ExtensionKey, [.. subExtensions]);
                }

                ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert DeviceRef information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.DeviceRef[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.DeviceRef(item);
                datItem.SetFieldValue<DatItems.Source?>(DatItems.DatItem.SourceKey, new DatItems.Source { Index = indexId, Name = filename });
                datItem.CopyMachineInformation(machine);
                ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert DipSwitch information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.DipSwitch[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.DipSwitch(item);
                datItem.SetFieldValue<DatItems.Source?>(DatItems.DatItem.SourceKey, new DatItems.Source { Index = indexId, Name = filename });
                datItem.CopyMachineInformation(machine);

                // Handle subitems
                var condition = item.Read<Models.Metadata.Condition>(Models.Metadata.DipSwitch.ConditionKey);
                if (condition != null)
                {
                    var subItem = new DatItems.Formats.Condition(condition);
                    datItem.SetFieldValue<DatItems.Formats.Condition?>(Models.Metadata.DipSwitch.ConditionKey, subItem);
                }

                var dipLocations = ReadItemArray<Models.Metadata.DipLocation>(item, Models.Metadata.DipSwitch.DipLocationKey);
                if (dipLocations != null)
                {
                    var subLocations = new List<DatItems.Formats.DipLocation>();
                    foreach (var location in dipLocations)
                    {
                        var subItem = new DatItems.Formats.DipLocation(location);
                        subLocations.Add(subItem);
                    }

                    datItem.SetFieldValue<DatItems.Formats.DipLocation[]?>(Models.Metadata.DipSwitch.DipLocationKey, [.. subLocations]);
                }

                var dipValues = ReadItemArray<Models.Metadata.DipValue>(item, Models.Metadata.DipSwitch.DipValueKey);
                if (dipValues != null)
                {
                    var subValues = new List<DatItems.Formats.DipValue>();
                    foreach (var value in dipValues)
                    {
                        var subItem = new DatItems.Formats.DipValue(value);

                        var subCondition = item.Read<Models.Metadata.Condition>(Models.Metadata.DipValue.ConditionKey);
                        if (subCondition != null)
                        {
                            var subSubItem = new DatItems.Formats.Condition(subCondition);
                            subItem.SetFieldValue<DatItems.Formats.Condition?>(Models.Metadata.DipValue.ConditionKey, subSubItem);
                        }

                        subValues.Add(subItem);
                    }

                    datItem.SetFieldValue<DatItems.Formats.DipValue[]?>(Models.Metadata.DipSwitch.DipValueKey, [.. subValues]);
                }

                ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert Disk information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Disk[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Disk(item);
                datItem.SetFieldValue<DatItems.Source?>(DatItems.DatItem.SourceKey, new DatItems.Source { Index = indexId, Name = filename });
                datItem.CopyMachineInformation(machine);
                ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert Display information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Display[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Display(item);
                datItem.SetFieldValue<DatItems.Source?>(DatItems.DatItem.SourceKey, new DatItems.Source { Index = indexId, Name = filename });
                datItem.CopyMachineInformation(machine);
                ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert Driver information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Driver[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Driver(item);
                datItem.SetFieldValue<DatItems.Source?>(DatItems.DatItem.SourceKey, new DatItems.Source { Index = indexId, Name = filename });
                datItem.CopyMachineInformation(machine);
                ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert Dump information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Dump[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
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

                string name = $"{machine.GetFieldValue<string?>(Models.Metadata.Machine.NameKey)}_{index++}{(!string.IsNullOrEmpty(rom!.ReadString(Models.Metadata.Rom.RemarkKey)) ? $" {rom.ReadString(Models.Metadata.Rom.RemarkKey)}" : string.Empty)}";

                var item = new DatItems.Formats.Rom();
                item.SetName(name);
                item.SetFieldValue<string?>(Models.Metadata.Rom.OffsetKey, rom.ReadString(Models.Metadata.Rom.OffsetKey));
                item.SetFieldValue<OpenMSXSubType>(Models.Metadata.Rom.OpenMSXMediaType, subType);
                item.SetFieldValue<string?>(Models.Metadata.Rom.OpenMSXType, rom.ReadString(Models.Metadata.Rom.OpenMSXType));
                item.SetFieldValue<string?>(Models.Metadata.Rom.RemarkKey, rom.ReadString(Models.Metadata.Rom.RemarkKey));
                item.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, rom.ReadString(Models.Metadata.Rom.SHA1Key));
                item.SetFieldValue<DatItems.Source?>(DatItems.DatItem.SourceKey, new DatItems.Source { Index = indexId, Name = filename });

                if (dump.Read<Models.OpenMSX.Original>(Models.Metadata.Dump.OriginalKey) != null)
                {
                    var original = dump.Read<Models.OpenMSX.Original>(Models.Metadata.Dump.OriginalKey)!;
                    item.SetFieldValue<DatItems.Formats.Original?>("ORIGINAL", new DatItems.Formats.Original
                    {
                        Value = original.Value.AsYesNo(),
                        Content = original.Content,
                    });
                }

                item.CopyMachineInformation(machine);
                ParseAddHelper(item, statsOnly);
            }
        }

        /// <summary>
        /// Convert Feature information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Feature[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Feature(item);
                datItem.SetFieldValue<DatItems.Source?>(DatItems.DatItem.SourceKey, new DatItems.Source { Index = indexId, Name = filename });
                datItem.CopyMachineInformation(machine);
                ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert Info information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Info[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Info(item);
                datItem.SetFieldValue<DatItems.Source?>(DatItems.DatItem.SourceKey, new DatItems.Source { Index = indexId, Name = filename });
                datItem.CopyMachineInformation(machine);
                ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert Input information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Input[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Input(item);
                datItem.SetFieldValue<DatItems.Source?>(DatItems.DatItem.SourceKey, new DatItems.Source { Index = indexId, Name = filename });
                datItem.CopyMachineInformation(machine);

                // Handle subitems
                var controls = ReadItemArray<Models.Metadata.Control>(item, Models.Metadata.Input.ControlKey);
                if (controls != null)
                {
                    var subControls = new List<DatItems.Formats.Control>();
                    foreach (var control in controls)
                    {
                        var subItem = new DatItems.Formats.Control(control);
                        subControls.Add(subItem);
                    }

                    datItem.SetFieldValue<DatItems.Formats.Control[]?>(Models.Metadata.Input.ControlKey, [.. subControls]);
                }

                ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert Media information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Media[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Media(item);
                datItem.SetFieldValue<DatItems.Source?>(DatItems.DatItem.SourceKey, new DatItems.Source { Index = indexId, Name = filename });
                datItem.CopyMachineInformation(machine);
                ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert Part information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Part[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
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

                        foreach (var rom in roms)
                        {
                            var romItem = new DatItems.Formats.Rom(rom);
                            romItem.SetFieldValue<DatItems.Formats.DataArea?>(DatItems.Formats.Rom.DataAreaKey, dataAreaItem);
                            romItem.SetFieldValue<DatItems.Formats.Part?>(DatItems.Formats.Rom.PartKey, partItem);
                            romItem.SetFieldValue<DatItems.Source?>(DatItems.DatItem.SourceKey, new DatItems.Source { Index = indexId, Name = filename });
                            romItem.CopyMachineInformation(machine);
                            ParseAddHelper(romItem, statsOnly);
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
                            diskItem.SetFieldValue<DatItems.Source?>(DatItems.DatItem.SourceKey, new DatItems.Source { Index = indexId, Name = filename });
                            diskItem.CopyMachineInformation(machine);
                            ParseAddHelper(diskItem, statsOnly);
                        }
                    }

                    var dipSwitches = ReadItemArray<Models.Metadata.DipSwitch>(item, Models.Metadata.Part.DipSwitchKey);
                    if (dipSwitches != null)
                    {
                        foreach (var dipSwitch in dipSwitches)
                        {
                            var dipSwitchItem = new DatItems.Formats.DipSwitch(dipSwitch);
                            dipSwitchItem.SetFieldValue<DatItems.Source?>(DatItems.DatItem.SourceKey, new DatItems.Source { Index = indexId, Name = filename });
                            dipSwitchItem.CopyMachineInformation(machine);

                            var dipValues = ReadItemArray<Models.Metadata.DipValue>(dipSwitch, Models.Metadata.DipSwitch.DipValueKey);
                            if (dipValues != null)
                            {
                                var subValues = new List<DatItems.Formats.DipValue>();
                                foreach (var value in dipValues)
                                {
                                    var subItem = new DatItems.Formats.DipValue(value);
                                    subValues.Add(subItem);
                                }

                                dipSwitchItem.SetFieldValue<DatItems.Formats.DipValue[]?>(Models.Metadata.DipSwitch.DipValueKey, [.. subValues]);
                            }

                            ParseAddHelper(dipSwitchItem, statsOnly);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Convert Port information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Port[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Port(item);
                datItem.SetFieldValue<DatItems.Source?>(DatItems.DatItem.SourceKey, new DatItems.Source { Index = indexId, Name = filename });
                datItem.CopyMachineInformation(machine);

                // Handle subitems
                var analogs = ReadItemArray<Models.Metadata.Analog>(item, Models.Metadata.Port.AnalogKey);
                if (analogs != null)
                {
                    var subAnalogs = new List<DatItems.Formats.Analog>();
                    foreach (var analog in analogs)
                    {
                        var subItem = new DatItems.Formats.Analog(analog);
                        subAnalogs.Add(subItem);
                    }

                    datItem.SetFieldValue<DatItems.Formats.Analog[]?>(Models.Metadata.Port.AnalogKey, [.. subAnalogs]);
                }

                ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert RamOption information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.RamOption[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.RamOption(item);
                datItem.SetFieldValue<DatItems.Source?>(DatItems.DatItem.SourceKey, new DatItems.Source { Index = indexId, Name = filename });
                datItem.CopyMachineInformation(machine);
                ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert Release information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Release[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Release(item);
                datItem.SetFieldValue<DatItems.Source?>(DatItems.DatItem.SourceKey, new DatItems.Source { Index = indexId, Name = filename });
                datItem.CopyMachineInformation(machine);
                ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert Rom information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Rom[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Rom(item);
                datItem.SetFieldValue<DatItems.Source?>(DatItems.DatItem.SourceKey, new DatItems.Source { Index = indexId, Name = filename });
                datItem.CopyMachineInformation(machine);
                ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert Sample information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Sample[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Sample(item);
                datItem.SetFieldValue<DatItems.Source?>(DatItems.DatItem.SourceKey, new DatItems.Source { Index = indexId, Name = filename });
                datItem.CopyMachineInformation(machine);
                ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert SharedFeat information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.SharedFeat[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.SharedFeat(item);
                datItem.SetFieldValue<DatItems.Source?>(DatItems.DatItem.SourceKey, new DatItems.Source { Index = indexId, Name = filename });
                datItem.CopyMachineInformation(machine);
                ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert Slot information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Slot[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Slot(item);
                datItem.SetFieldValue<DatItems.Source?>(DatItems.DatItem.SourceKey, new DatItems.Source { Index = indexId, Name = filename });
                datItem.CopyMachineInformation(machine);

                // Handle subitems
                var slotOptions = ReadItemArray<Models.Metadata.SlotOption>(item, Models.Metadata.Slot.SlotOptionKey);
                if (slotOptions != null)
                {
                    var subOptions = new List<DatItems.Formats.SlotOption>();
                    foreach (var slotOption in slotOptions)
                    {
                        var subItem = new DatItems.Formats.SlotOption(slotOption);
                        subOptions.Add(subItem);
                    }

                    datItem.SetFieldValue<DatItems.Formats.SlotOption[]?>(Models.Metadata.Slot.SlotOptionKey, [.. subOptions]);
                }

                ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert SoftwareList information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.SoftwareList[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.SoftwareList(item);
                datItem.SetFieldValue<DatItems.Source?>(DatItems.DatItem.SourceKey, new DatItems.Source { Index = indexId, Name = filename });
                datItem.CopyMachineInformation(machine);
                ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert Sound information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Sound[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Sound(item);
                datItem.SetFieldValue<DatItems.Source?>(DatItems.DatItem.SourceKey, new DatItems.Source { Index = indexId, Name = filename });
                datItem.CopyMachineInformation(machine);
                ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert Video information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Video[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Display(item);
                datItem.SetFieldValue<DatItems.Source?>(DatItems.DatItem.SourceKey, new DatItems.Source { Index = indexId, Name = filename });
                datItem.CopyMachineInformation(machine);
                ParseAddHelper(datItem, statsOnly);
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

        #region To Metadata

        /// <summary>
        /// Convert metadata information
        /// </summary>
        public Models.Metadata.MetadataFile? ConvertMetadata()
        {
            // If we don't have items, we can't do anything
            if (this.Items == null || !this.Items.Any())
                return null;

            // Create an object to hold the data
            var metadataFile = new Models.Metadata.MetadataFile();

            // Convert and assign the header
            var header = ConvertHeader();
            if (header != null)
                metadataFile[Models.Metadata.MetadataFile.HeaderKey] = header;

            // TODO: Implement machine creation

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
            var header = new Models.Metadata.Header();

            if (Header.GetFieldValue<string?>(Models.Metadata.Header.AuthorKey) == null)
                header[Models.Metadata.Header.AuthorKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.AuthorKey);
            if (Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.BiosModeKey) == MergingFlag.None)
                header[Models.Metadata.Header.BiosModeKey] = Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.BiosModeKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.BuildKey) == null)
                header[Models.Metadata.Header.BuildKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.BuildKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.CategoryKey) == null)
                header[Models.Metadata.Header.CategoryKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.CategoryKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.CommentKey) == null)
                header[Models.Metadata.Header.CommentKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.CommentKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.DateKey) == null)
                header[Models.Metadata.Header.DateKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.DateKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.DatVersionKey) == null)
                header[Models.Metadata.Header.DatVersionKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.DatVersionKey);
            if (Header.GetFieldValue<bool?>(Models.Metadata.Header.DebugKey) == null)
                header[Models.Metadata.Header.DebugKey] = Header.GetFieldValue<bool?>(Models.Metadata.Header.DebugKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey) == null)
                header[Models.Metadata.Header.DescriptionKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.EmailKey) == null)
                header[Models.Metadata.Header.EmailKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.EmailKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.EmulatorVersionKey) == null)
                header[Models.Metadata.Header.EmulatorVersionKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.EmulatorVersionKey);
            if (Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.ForceMergingKey) == MergingFlag.None)
                header[Models.Metadata.Header.ForceMergingKey] = Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.ForceMergingKey);
            if (Header.GetFieldValue<NodumpFlag>(Models.Metadata.Header.ForceNodumpKey) == NodumpFlag.None)
                header[Models.Metadata.Header.ForceNodumpKey] = Header.GetFieldValue<NodumpFlag>(Models.Metadata.Header.ForceNodumpKey);
            if (Header.GetFieldValue<PackingFlag>(Models.Metadata.Header.ForcePackingKey) == PackingFlag.None)
                header[Models.Metadata.Header.ForcePackingKey] = Header.GetFieldValue<PackingFlag>(Models.Metadata.Header.ForcePackingKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.HeaderKey) == null)
                header[Models.Metadata.Header.HeaderKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.HeaderKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.HomepageKey) == null)
                header[Models.Metadata.Header.HomepageKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.HomepageKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.IdKey) == null)
                header[Models.Metadata.Header.IdKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.IdKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.ImFolderKey) == null)
                header[Models.Metadata.Header.ImFolderKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.ImFolderKey);
            if (Header.GetFieldValue<bool?>(Models.Metadata.Header.LockBiosModeKey) == null)
                header[Models.Metadata.Header.LockBiosModeKey] = Header.GetFieldValue<bool?>(Models.Metadata.Header.LockBiosModeKey);
            if (Header.GetFieldValue<bool?>(Models.Metadata.Header.LockRomModeKey) == null)
                header[Models.Metadata.Header.LockRomModeKey] = Header.GetFieldValue<bool?>(Models.Metadata.Header.LockRomModeKey);
            if (Header.GetFieldValue<bool?>(Models.Metadata.Header.LockSampleModeKey) == null)
                header[Models.Metadata.Header.LockSampleModeKey] = Header.GetFieldValue<bool?>(Models.Metadata.Header.LockSampleModeKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.MameConfigKey) == null)
                header[Models.Metadata.Header.MameConfigKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey) == null)
                header[Models.Metadata.Header.NameKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.MameConfigKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.PluginKey) == null)
                header[Models.Metadata.Header.PluginKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.PluginKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.RefNameKey) == null)
                header[Models.Metadata.Header.RefNameKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.RefNameKey);
            if (Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.RomModeKey) == MergingFlag.None)
                header[Models.Metadata.Header.RomModeKey] = Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.RomModeKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.RomTitleKey) == null)
                header[Models.Metadata.Header.RomTitleKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.RomTitleKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.RootDirKey) == null)
                header[Models.Metadata.Header.RootDirKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.RootDirKey);
            if (Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.SampleModeKey) == MergingFlag.None)
                header[Models.Metadata.Header.SampleModeKey] = Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.SampleModeKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.ScreenshotsHeightKey) == null)
                header[Models.Metadata.Header.ScreenshotsHeightKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.ScreenshotsHeightKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.ScreenshotsWidthKey) == null)
                header[Models.Metadata.Header.ScreenshotsWidthKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.ScreenshotsWidthKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.SystemKey) == null)
                header[Models.Metadata.Header.SystemKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.SystemKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.TypeKey) == null)
                header[Models.Metadata.Header.TypeKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.TypeKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.UrlKey) == null)
                header[Models.Metadata.Header.UrlKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.UrlKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.VersionKey) == null)
                header[Models.Metadata.Header.VersionKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.VersionKey);

            // Convert subheader values
            if (Header.CanOpenSpecified)
                header[Models.Metadata.Header.CanOpenKey] = Header.GetFieldValue<Models.OfflineList.CanOpen[]?>(Models.Metadata.Header.CanOpenKey);
            // if (Header.ImagesSpecified)
            //     // TODO: Add to internal model
            if (Header.InfosSpecified)
            {
                var infoItem = new Models.OfflineList.Infos();
                var infos = Header.GetFieldValue<Formats.OfflineListInfo[]?>(Models.Metadata.Header.InfosKey)!;
                foreach (var info in infos)
                {
                    switch (info.Name)
                    {
                        case "title":
                            infoItem.Title = new Models.OfflineList.Title
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                        case "location":
                            infoItem.Location = new Models.OfflineList.Location
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                        case "publisher":
                            infoItem.Publisher = new Models.OfflineList.Publisher
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                        case "sourceRom":
                            infoItem.SourceRom = new Models.OfflineList.SourceRom
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                        case "saveType":
                            infoItem.SaveType = new Models.OfflineList.SaveType
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                        case "romSize":
                            infoItem.RomSize = new Models.OfflineList.RomSize
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                        case "releaseNumber":
                            infoItem.ReleaseNumber = new Models.OfflineList.ReleaseNumber
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                        case "languageNumber":
                            infoItem.LanguageNumber = new Models.OfflineList.LanguageNumber
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                        case "comment":
                            infoItem.Comment = new Models.OfflineList.Comment
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                        case "romCRC":
                            infoItem.RomCRC = new Models.OfflineList.RomCRC
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                        case "im1CRC":
                            infoItem.Im1CRC = new Models.OfflineList.Im1CRC
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                        case "im2CRC":
                            infoItem.Im2CRC = new Models.OfflineList.Im2CRC
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                        case "languages":
                            infoItem.Languages = new Models.OfflineList.Languages
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                    }
                }

                header[Models.Metadata.Header.InfosKey] = infoItem;
            }
            if (Header.NewDatSpecified)
            {
                var newDat = new Models.OfflineList.NewDat
                {
                    DatVersionUrl = Header.GetFieldValue<string?>("DATVERSIONURL"),
                    //DatUrl = Header.GetFieldValue<Models.OfflineList.DatUrl?>("DATURL"), // TODO: Add to internal model
                    ImUrl = Header.GetFieldValue<string?>("IMURL"),
                };
                header[Models.Metadata.Header.NewDatKey] = newDat;
            }
            // if (Header.SearchSpecified)
            //     // TODO: Add to internal model

            return header;
        }

        #endregion
    }
}