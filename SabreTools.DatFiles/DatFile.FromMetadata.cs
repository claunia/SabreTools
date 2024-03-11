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
            if (Header.GetStringFieldValue(Models.Metadata.Header.AuthorKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.AuthorKey, header.GetStringFieldValue(Models.Metadata.Header.AuthorKey));
            if (Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.BiosModeKey) == MergingFlag.None)
                Header.SetFieldValue<MergingFlag>(Models.Metadata.Header.BiosModeKey, header.GetFieldValue<MergingFlag>(Models.Metadata.Header.BiosModeKey));
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
            if (Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.ForceMergingKey) == MergingFlag.None)
                Header.SetFieldValue<MergingFlag>(Models.Metadata.Header.ForceMergingKey, header.GetFieldValue<MergingFlag>(Models.Metadata.Header.ForceMergingKey));
            if (Header.GetFieldValue<NodumpFlag>(Models.Metadata.Header.ForceNodumpKey) == NodumpFlag.None)
                Header.SetFieldValue<NodumpFlag>(Models.Metadata.Header.ForceNodumpKey, header.GetFieldValue<NodumpFlag>(Models.Metadata.Header.ForceNodumpKey));
            if (Header.GetFieldValue<PackingFlag>(Models.Metadata.Header.ForcePackingKey) == PackingFlag.None)
                Header.SetFieldValue<PackingFlag>(Models.Metadata.Header.ForcePackingKey, header.GetFieldValue<PackingFlag>(Models.Metadata.Header.ForcePackingKey));
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
            if (Header.GetStringFieldValue(Models.Metadata.Header.PluginKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.PluginKey, header.GetStringFieldValue(Models.Metadata.Header.PluginKey));
            if (Header.GetStringFieldValue(Models.Metadata.Header.RefNameKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.RefNameKey, header.GetStringFieldValue(Models.Metadata.Header.RefNameKey));
            if (Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.RomModeKey) == MergingFlag.None)
                Header.SetFieldValue<MergingFlag>(Models.Metadata.Header.RomModeKey, header.GetFieldValue<MergingFlag>(Models.Metadata.Header.RomModeKey));
            if (Header.GetStringFieldValue(Models.Metadata.Header.RomTitleKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.RomTitleKey, header.GetStringFieldValue(Models.Metadata.Header.RomTitleKey));
            if (Header.GetStringFieldValue(Models.Metadata.Header.RootDirKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.RootDirKey, header.GetStringFieldValue(Models.Metadata.Header.RootDirKey));
            if (Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.SampleModeKey) == MergingFlag.None)
                Header.SetFieldValue<MergingFlag>(Models.Metadata.Header.SampleModeKey, header.GetFieldValue<MergingFlag>(Models.Metadata.Header.SampleModeKey));
            if (Header.GetStringFieldValue(Models.Metadata.Header.ScreenshotsHeightKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.ScreenshotsHeightKey, header.GetStringFieldValue(Models.Metadata.Header.ScreenshotsHeightKey));
            if (Header.GetStringFieldValue(Models.Metadata.Header.ScreenshotsWidthKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.ScreenshotsWidthKey, header.GetStringFieldValue(Models.Metadata.Header.ScreenshotsWidthKey));
            if (Header.GetStringFieldValue(Models.Metadata.Header.SystemKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.SystemKey, header.GetStringFieldValue(Models.Metadata.Header.SystemKey));
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

                // Process hash values
                if (datItem.GetStringFieldValue(Models.Metadata.Disk.MD5Key) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Disk.MD5Key, TextHelper.NormalizeMD5(datItem.GetStringFieldValue(Models.Metadata.Disk.MD5Key)));
                if (datItem.GetStringFieldValue(Models.Metadata.Disk.SHA1Key) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Disk.SHA1Key, TextHelper.NormalizeSHA1(datItem.GetStringFieldValue(Models.Metadata.Disk.SHA1Key)));

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

                string name = $"{machine.GetStringFieldValue(Models.Metadata.Machine.NameKey)}_{index++}{(!string.IsNullOrEmpty(rom!.ReadString(Models.Metadata.Rom.RemarkKey)) ? $" {rom.ReadString(Models.Metadata.Rom.RemarkKey)}" : string.Empty)}";

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

                // Process hash values
                if (item.GetInt64FieldValue(Models.Metadata.Rom.SizeKey) != null)
                    item.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, item.GetInt64FieldValue(Models.Metadata.Rom.SizeKey).ToString());
                if (item.GetStringFieldValue(Models.Metadata.Rom.CRCKey) != null)
                    item.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, TextHelper.NormalizeCRC32(item.GetStringFieldValue(Models.Metadata.Rom.CRCKey)));
                if (item.GetStringFieldValue(Models.Metadata.Rom.MD5Key) != null)
                    item.SetFieldValue<string?>(Models.Metadata.Rom.MD5Key, TextHelper.NormalizeMD5(item.GetStringFieldValue(Models.Metadata.Rom.MD5Key)));
                if (item.GetStringFieldValue(Models.Metadata.Rom.SHA1Key) != null)
                    item.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, TextHelper.NormalizeSHA1(item.GetStringFieldValue(Models.Metadata.Rom.SHA1Key)));
                if (item.GetStringFieldValue(Models.Metadata.Rom.SHA256Key) != null)
                    item.SetFieldValue<string?>(Models.Metadata.Rom.SHA256Key, TextHelper.NormalizeSHA256(item.GetStringFieldValue(Models.Metadata.Rom.SHA256Key)));
                if (item.GetStringFieldValue(Models.Metadata.Rom.SHA384Key) != null)
                    item.SetFieldValue<string?>(Models.Metadata.Rom.SHA384Key, TextHelper.NormalizeSHA384(item.GetStringFieldValue(Models.Metadata.Rom.SHA384Key)));
                if (item.GetStringFieldValue(Models.Metadata.Rom.SHA512Key) != null)
                    item.SetFieldValue<string?>(Models.Metadata.Rom.SHA512Key, TextHelper.NormalizeSHA512(item.GetStringFieldValue(Models.Metadata.Rom.SHA512Key)));

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

                // Process hash values
                if (datItem.GetStringFieldValue(Models.Metadata.Media.MD5Key) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Media.MD5Key, TextHelper.NormalizeMD5(datItem.GetStringFieldValue(Models.Metadata.Media.MD5Key)));
                if (datItem.GetStringFieldValue(Models.Metadata.Media.SHA1Key) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Media.SHA1Key, TextHelper.NormalizeSHA1(datItem.GetStringFieldValue(Models.Metadata.Media.SHA1Key)));
                if (datItem.GetStringFieldValue(Models.Metadata.Media.SHA256Key) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Media.SHA256Key, TextHelper.NormalizeSHA256(datItem.GetStringFieldValue(Models.Metadata.Media.SHA256Key)));

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

                            // Process hash values
                            if (romItem.GetInt64FieldValue(Models.Metadata.Rom.SizeKey) != null)
                                romItem.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, romItem.GetInt64FieldValue(Models.Metadata.Rom.SizeKey).ToString());
                            if (romItem.GetStringFieldValue(Models.Metadata.Rom.CRCKey) != null)
                                romItem.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, TextHelper.NormalizeCRC32(romItem.GetStringFieldValue(Models.Metadata.Rom.CRCKey)));
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

                            // Process hash values
                            if (diskItem.GetStringFieldValue(Models.Metadata.Disk.MD5Key) != null)
                                diskItem.SetFieldValue<string?>(Models.Metadata.Disk.MD5Key, TextHelper.NormalizeMD5(diskItem.GetStringFieldValue(Models.Metadata.Disk.MD5Key)));
                            if (diskItem.GetStringFieldValue(Models.Metadata.Disk.SHA1Key) != null)
                                diskItem.SetFieldValue<string?>(Models.Metadata.Disk.SHA1Key, TextHelper.NormalizeSHA1(diskItem.GetStringFieldValue(Models.Metadata.Disk.SHA1Key)));

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

                // Process hash values
                if (datItem.GetInt64FieldValue(Models.Metadata.Rom.SizeKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, datItem.GetInt64FieldValue(Models.Metadata.Rom.SizeKey).ToString());
                if (datItem.GetStringFieldValue(Models.Metadata.Rom.CRCKey) != null)
                    datItem.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, TextHelper.NormalizeCRC32(datItem.GetStringFieldValue(Models.Metadata.Rom.CRCKey)));
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
    }
}