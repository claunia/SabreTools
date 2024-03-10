using System;
using System.Collections.Generic;
using System.Linq;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents writing a SoftwareList
    /// </summary>
    internal partial class SoftwareList : DatFile
    {
        /// <inheritdoc/>
        protected override ItemType[] GetSupportedTypes()
        {
            return
            [
                ItemType.DipSwitch,
                ItemType.Disk,
                ItemType.Info,
                ItemType.Rom,
                ItemType.SharedFeature,
            ];
        }

        /// <inheritdoc/>
        protected override List<string>? GetMissingRequiredFields(DatItem datItem)
        {
            var missingFields = new List<string>();

            switch (datItem)
            {
                case DipSwitch dipSwitch:
                    if (!dipSwitch.PartSpecified)
                    {
                        missingFields.Add(Models.Metadata.Part.NameKey);
                        missingFields.Add(Models.Metadata.Part.InterfaceKey);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(dipSwitch.GetFieldValue<Part?>("PART")!.GetName()))
                            missingFields.Add(Models.Metadata.Part.NameKey);
                        if (string.IsNullOrEmpty(dipSwitch.GetFieldValue<Part?>("PART")!.GetFieldValue<string?>(Models.Metadata.Part.InterfaceKey)))
                            missingFields.Add(Models.Metadata.Part.InterfaceKey);
                    }
                    if (string.IsNullOrEmpty(dipSwitch.GetName()))
                        missingFields.Add(Models.Metadata.DipSwitch.NameKey);
                    if (string.IsNullOrEmpty(dipSwitch.GetFieldValue<string?>(Models.Metadata.DipSwitch.TagKey)))
                        missingFields.Add(Models.Metadata.DipSwitch.TagKey);
                    if (string.IsNullOrEmpty(dipSwitch.GetFieldValue<string?>(Models.Metadata.DipSwitch.MaskKey)))
                        missingFields.Add(Models.Metadata.DipSwitch.MaskKey);
                    if (dipSwitch.ValuesSpecified)
                    {
                        if (dipSwitch.GetFieldValue<DipValue[]?>(Models.Metadata.DipSwitch.DipValueKey)!.Any(dv => string.IsNullOrEmpty(dv.GetName())))
                            missingFields.Add(Models.Metadata.DipValue.NameKey);
                        if (dipSwitch.GetFieldValue<DipValue[]?>(Models.Metadata.DipSwitch.DipValueKey)!.Any(dv => string.IsNullOrEmpty(dv.GetFieldValue<string?>(Models.Metadata.DipValue.ValueKey))))
                            missingFields.Add(Models.Metadata.DipValue.ValueKey);
                    }

                    break;

                case Disk disk:
                    if (!disk.PartSpecified)
                    {
                        missingFields.Add(Models.Metadata.Part.NameKey);
                        missingFields.Add(Models.Metadata.Part.InterfaceKey);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(disk.GetFieldValue<Part?>("PART")!.GetName()))
                            missingFields.Add(Models.Metadata.Part.NameKey);
                        if (string.IsNullOrEmpty(disk.GetFieldValue<Part?>("PART")!.GetFieldValue<string?>(Models.Metadata.Part.InterfaceKey)))
                            missingFields.Add(Models.Metadata.Part.InterfaceKey);
                    }
                    if (!disk.DiskAreaSpecified)
                    {
                        missingFields.Add(Models.Metadata.DiskArea.NameKey);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(disk.GetFieldValue<DiskArea?>("DISKAREA")!.GetName()))
                            missingFields.Add(Models.Metadata.DiskArea.NameKey);
                    }
                    if (string.IsNullOrEmpty(disk.GetName()))
                        missingFields.Add(Models.Metadata.Disk.NameKey);
                    break;

                case Info info:
                    if (string.IsNullOrEmpty(info.GetName()))
                        missingFields.Add(Models.Metadata.Info.NameKey);
                    break;

                case Rom rom:
                    if (!rom.PartSpecified)
                    {
                        missingFields.Add(Models.Metadata.Part.NameKey);
                        missingFields.Add(Models.Metadata.Part.InterfaceKey);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(rom.GetFieldValue<Part?>("PART")!.GetName()))
                            missingFields.Add(Models.Metadata.Part.NameKey);
                        if (string.IsNullOrEmpty(rom.GetFieldValue<Part?>("PART")!.GetFieldValue<string?>(Models.Metadata.Part.InterfaceKey)))
                            missingFields.Add(Models.Metadata.Part.InterfaceKey);
                    }
                    if (!rom.DataAreaSpecified)
                    {
                        missingFields.Add(Models.Metadata.DataArea.NameKey);
                        missingFields.Add(Models.Metadata.DataArea.SizeKey);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(rom.GetFieldValue<DataArea?>("DATAAREA")!.GetName()))
                            missingFields.Add(Models.Metadata.DataArea.NameKey);
                        if (rom.GetFieldValue<DataArea?>("DATAAREA")!.GetFieldValue<long?>(Models.Metadata.DataArea.SizeKey) == null)
                            missingFields.Add(Models.Metadata.DataArea.SizeKey);
                    }
                    break;

                case SharedFeature sharedFeat:
                    if (string.IsNullOrEmpty(sharedFeat.GetName()))
                        missingFields.Add(Models.Metadata.SharedFeat.NameKey);
                    break;
                default:
                    // Unsupported ItemTypes should be caught already
                    return null;
            }

            return missingFields;
        }

        /// <inheritdoc/>
        public override bool WriteToFile(string outfile, bool ignoreblanks = false, bool throwOnError = false)
        {
            try
            {
                logger.User($"Writing to '{outfile}'...");

                var softwarelist = CreateSoftwareList(ignoreblanks);
                if (!(new Serialization.Files.SoftwareList().SerializeToFileWithDocType(softwarelist, outfile)))
                {
                    logger.Warning($"File '{outfile}' could not be written! See the log for more details.");
                    return false;
                }
            }
            catch (Exception ex) when (!throwOnError)
            {
                logger.Error(ex);
                return false;
            }

            logger.User($"'{outfile}' written!{Environment.NewLine}");
            return true;
        }

        #region Converters

        /// <summary>
        /// Create a SoftwareList from the current internal information
        /// <summary>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise</param>
        private Models.SoftwareList.SoftwareList CreateSoftwareList(bool ignoreblanks)
        {
            var softwarelist = new Models.SoftwareList.SoftwareList
            {
                Name = Header.Name,
                Description = Header.Description,
                Notes = Header.Comment,
                Software = CreateSoftware(ignoreblanks),
            };

            return softwarelist;
        }

        /// <summary>
        /// Create an array of Software from the current internal information
        /// <summary>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise</param>
        private Models.SoftwareList.Software[]? CreateSoftware(bool ignoreblanks)
        {
            // If we don't have items, we can't do anything
            if (this.Items == null || !this.Items.Any())
                return null;

            // Create a list of hold the games
            var software = new List<Models.SoftwareList.Software>();

            // Loop through the sorted items and create games for them
            foreach (string key in Items.SortedKeys)
            {
                var items = Items.FilteredItems(key);
                if (items == null || !items.Any())
                    continue;

                // Get the first item for game information
                var machine = items[0].Machine;
                var sw = CreateSoftware(machine!);

                // Create holders for all item types
                var infos = new List<Models.SoftwareList.Info>();
                var sharedfeats = new List<Models.SoftwareList.SharedFeat>();
                var parts = new List<Models.SoftwareList.Part>();

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
                        case Info info:
                            infos.Add(CreateInfo(info));
                            break;
                        case SharedFeature sharedFeature:
                            sharedfeats.Add(CreateSharedFeat(sharedFeature));
                            break;
                        case Rom rom:
                            parts.Add(CreatePart(rom));
                            break;
                        case Disk disk:
                            parts.Add(CreatePart(disk));
                            break;
                        case DipSwitch dipswitch:
                            parts.Add(CreatePart(dipswitch));
                            break;
                    }
                }

                // Process the parts to ensure we don't have duplicates
                parts = SantitizeParts(parts);

                // Assign the values to the game
                sw.Info = [.. infos];
                sw.SharedFeat = [.. sharedfeats];
                sw.Part = [.. parts];

                // Add the game to the list
                software.Add(sw);
            }

            return [.. software];
        }

        /// <summary>
        /// Create a Software from the current internal information
        /// <summary>
        private Models.SoftwareList.Software CreateSoftware(Machine machine)
        {
            var software = new Models.SoftwareList.Software
            {
                Name = machine.GetFieldValue<string?>(Models.Metadata.Machine.NameKey),
                CloneOf = machine.GetFieldValue<string?>(Models.Metadata.Machine.CloneOfKey),
                Supported = machine.GetFieldValue<Supported>(Models.Metadata.Machine.SupportedKey).AsStringValue<Supported>(useSecond: true),
                Description = machine.GetFieldValue<string?>(Models.Metadata.Machine.DescriptionKey),
                Year = machine.GetFieldValue<string?>(Models.Metadata.Machine.YearKey),
                Publisher = machine.GetFieldValue<string?>(Models.Metadata.Machine.PublisherKey),
                Notes = machine.GetFieldValue<string?>(Models.Metadata.Machine.CommentKey),
            };

            return software;
        }

        /// <summary>
        /// Create a Info from the current Info DatItem
        /// <summary>
        private static Models.SoftwareList.Info CreateInfo(Info item)
        {
            var info = new Models.SoftwareList.Info
            {
                Name = item.GetName(),
                Value = item.GetFieldValue<string?>(Models.Metadata.Info.ValueKey),
            };
            return info;
        }

        /// <summary>
        /// Create a SharedFeat from the current SharedFeature DatItem
        /// <summary>
        private static Models.SoftwareList.SharedFeat CreateSharedFeat(SharedFeature item)
        {
            var sharedfeat = new Models.SoftwareList.SharedFeat
            {
                Name = item.GetName(),
                Value = item.GetFieldValue<string?>(Models.Metadata.SharedFeat.ValueKey),
            };
            return sharedfeat;
        }

        /// <summary>
        /// Create a Part from the current Rom DatItem
        /// <summary>
        private static Models.SoftwareList.Part CreatePart(Rom item)
        {
            var part = new Models.SoftwareList.Part
            {
                Name = item.GetFieldValue<Part?>("PART")?.GetName(),
                Interface = item.GetFieldValue<Part?>("PART")?.GetFieldValue<string?>(Models.Metadata.Part.InterfaceKey),
                Feature = CreateFeatures(item.GetFieldValue<Part?>("PART")?.GetFieldValue<PartFeature[]?>(Models.Metadata.Part.FeatureKey)),
                DataArea = CreateDataAreas(item),
                DiskArea = null,
                DipSwitch = null,
            };
            return part;
        }

        /// <summary>
        /// Create a Part from the current Disk DatItem
        /// <summary>
        private static Models.SoftwareList.Part CreatePart(Disk item)
        {
            var part = new Models.SoftwareList.Part
            {
                Name = item.GetFieldValue<Part?>("PART")?.GetName(),
                Interface = item.GetFieldValue<Part?>("PART")?.GetFieldValue<string?>(Models.Metadata.Part.InterfaceKey),
                Feature = CreateFeatures(item.GetFieldValue<Part?>("PART")?.GetFieldValue<PartFeature[]?>(Models.Metadata.Part.FeatureKey)),
                DataArea = null,
                DiskArea = CreateDiskAreas(item),
                DipSwitch = null,
            };
            return part;
        }

        /// <summary>
        /// Create a Part from the current DipSwitch DatItem
        /// <summary>
        private static Models.SoftwareList.Part CreatePart(DipSwitch item)
        {
            var part = new Models.SoftwareList.Part
            {
                Name = item.GetFieldValue<Part?>("PART")?.GetName(),
                Interface = item.GetFieldValue<Part?>("PART")?.GetFieldValue<string?>(Models.Metadata.Part.InterfaceKey),
                Feature = CreateFeatures(item.GetFieldValue<Part?>("PART")?.GetFieldValue<PartFeature[]?>(Models.Metadata.Part.FeatureKey)),
                DataArea = null,
                DiskArea = null,
                DipSwitch = CreateDipSwitches(item),
            };
            return part;
        }

        /// <summary>
        /// Create a Feature array from the current list of PartFeature DatItems
        /// <summary>
        private static Models.SoftwareList.Feature[]? CreateFeatures(PartFeature[]? items)
        {
            // If we don't have features, we can't do anything
            if (items == null || !items.Any())
                return null;

            var features = new List<Models.SoftwareList.Feature>();
            foreach (var item in items)
            {
                var feature = new Models.SoftwareList.Feature
                {
                    Name = item.GetName(),
                    Value = item.GetFieldValue<string?>(Models.Metadata.Feature.ValueKey),
                };
                features.Add(feature);
            }

            return [.. features];
        }

        /// <summary>
        /// Create a DataArea array from the current Rom DatItem
        /// <summary>
        private static Models.SoftwareList.DataArea[]? CreateDataAreas(Rom item)
        {
            var dataArea = new Models.SoftwareList.DataArea
            {
                Name = item.GetFieldValue<DataArea?>("DATAAREA")?.GetName(),
                Size = item.GetFieldValue<DataArea?>("DATAAREA")?.GetFieldValue<long?>(Models.Metadata.DataArea.SizeKey)?.ToString(),
                Width = item.GetFieldValue<DataArea?>("DATAAREA")?.GetFieldValue<long?>(Models.Metadata.DataArea.WidthKey)?.ToString(),
                Endianness = item.GetFieldValue<DataArea?>("DATAAREA")?.GetFieldValue<Endianness>(Models.Metadata.DataArea.EndiannessKey).AsStringValue<Endianness>(),
                Rom = CreateRom(item),
            };
            return [dataArea];
        }

        /// <summary>
        /// Create a Rom array from the current Rom DatItem
        /// <summary>
        private static Models.SoftwareList.Rom[]? CreateRom(Rom item)
        {
            var rom = new Models.SoftwareList.Rom
            {
                Name = item.GetName(),
                Size = item.GetFieldValue<long?>(Models.Metadata.Rom.SizeKey)?.ToString(),
                Length = null,
                CRC = item.GetFieldValue<string?>(Models.Metadata.Rom.CRCKey),
                SHA1 = item.GetFieldValue<string?>(Models.Metadata.Rom.SHA1Key),
                Offset = item.GetFieldValue<string?>(Models.Metadata.Rom.OffsetKey),
                Value = item.GetFieldValue<string?>(Models.Metadata.Rom.ValueKey),
                Status = item.GetFieldValue<ItemStatus>(Models.Metadata.Rom.StatusKey).AsStringValue<ItemStatus>(useSecond: false),
                LoadFlag = item.GetFieldValue<LoadFlag>(Models.Metadata.Rom.LoadFlagKey).AsStringValue<LoadFlag>(),
            };
            return [rom];
        }

        /// <summary>
        /// Create a DiskArea array from the current Disk DatItem
        /// <summary>
        private static Models.SoftwareList.DiskArea[]? CreateDiskAreas(Disk item)
        {
            var diskArea = new Models.SoftwareList.DiskArea
            {
                Disk = CreateDisk(item),
            };
            return [diskArea];
        }

        /// <summary>
        /// Create a Disk array from the current Disk DatItem
        /// <summary>
        private static Models.SoftwareList.Disk[]? CreateDisk(Disk item)
        {
            var disk = new Models.SoftwareList.Disk
            {
                Name = item.GetName(),
                MD5 = item.GetFieldValue<string?>(Models.Metadata.Disk.MD5Key),
                SHA1 = item.GetFieldValue<string?>(Models.Metadata.Disk.SHA1Key),
                Status = item.GetFieldValue<ItemStatus>(Models.Metadata.Disk.StatusKey).AsStringValue<ItemStatus>(useSecond: false),
                Writeable = item.GetFieldValue<bool?>(Models.Metadata.Disk.WritableKey)?.ToString(),
            };
            return [disk];
        }

        /// <summary>
        /// Create a DipSwitch array from the current DipSwitch DatItem
        /// <summary>
        private static Models.SoftwareList.DipSwitch[]? CreateDipSwitches(DipSwitch item)
        {
            var dipValues = new List<Models.SoftwareList.DipValue>();
            foreach (var setting in item.GetFieldValue<DipValue[]?>(Models.Metadata.DipSwitch.DipValueKey) ?? [])
            {
                var dipValue = new Models.SoftwareList.DipValue
                {
                    Name = setting.GetName(),
                    Value = setting.GetFieldValue<string?>(Models.Metadata.DipValue.ValueKey),
                    Default = setting.GetFieldValue<bool?>(Models.Metadata.DipValue.DefaultKey).FromYesNo(),
                };

                dipValues.Add(dipValue);
            }

            var dipSwitch = new Models.SoftwareList.DipSwitch { DipValue = [.. dipValues] };
            return [dipSwitch];
        }

        /// <summary>
        /// Sanitize Parts list to ensure no duplicates exist
        /// <summary>
        private static List<Models.SoftwareList.Part> SantitizeParts(List<Models.SoftwareList.Part> parts)
        {
            // If we have no parts, we can't do anything
            if (!parts.Any())
                return parts;

            var grouped = parts.GroupBy(p => p.Name);

            var tempParts = new List<Models.SoftwareList.Part>();
            foreach (var grouping in grouped)
            {
                var tempPart = new Models.SoftwareList.Part();

                var tempFeatures = new List<Models.SoftwareList.Feature>();
                var tempDataAreas = new List<Models.SoftwareList.DataArea>();
                var tempDiskAreas = new List<Models.SoftwareList.DiskArea>();
                var tempDipSwitches = new List<Models.SoftwareList.DipSwitch>();

                foreach (var part in grouping)
                {
                    tempPart.Name ??= part.Name;
                    tempPart.Interface ??= part.Interface;

                    if (part.Feature != null)
                        tempFeatures.AddRange(part.Feature);
                    if (part.DataArea != null)
                        tempDataAreas.AddRange(part.DataArea);
                    if (part.DiskArea != null)
                        tempDiskAreas.AddRange(part.DiskArea);
                    if (part.DipSwitch != null)
                        tempDipSwitches.AddRange(part.DipSwitch);
                }

                tempDataAreas = SantitizeDataAreas(tempDataAreas);
                tempDiskAreas = SantitizeDiskAreas(tempDiskAreas);

                if (tempFeatures.Count > 0)
                    tempPart.Feature = [.. tempFeatures];
                if (tempDataAreas.Count > 0)
                    tempPart.DataArea = [.. tempDataAreas];
                if (tempDiskAreas.Count > 0)
                    tempPart.DiskArea = [.. tempDiskAreas];
                if (tempDipSwitches.Count > 0)
                    tempPart.DipSwitch = [.. tempDipSwitches];

                tempParts.Add(tempPart);
            }

            return tempParts;
        }

        /// <summary>
        /// Sanitize DataAreas list to ensure no duplicates exist
        /// <summary>
        private static List<Models.SoftwareList.DataArea> SantitizeDataAreas(List<Models.SoftwareList.DataArea> dataAreas)
        {
            // If we have no DataAreas, we can't do anything
            if (!dataAreas.Any())
                return dataAreas;

            var grouped = dataAreas.GroupBy(p => p.Name);

            var tempDataAreas = new List<Models.SoftwareList.DataArea>();
            foreach (var grouping in grouped)
            {
                var tempDataArea = new Models.SoftwareList.DataArea();
                var tempRoms = new List<Models.SoftwareList.Rom>();

                foreach (var dataArea in grouping)
                {
                    tempDataArea.Name ??= dataArea.Name;
                    tempDataArea.Size ??= dataArea.Size;
                    tempDataArea.Width ??= dataArea.Width;
                    tempDataArea.Endianness ??= dataArea.Endianness;

                    if (dataArea.Rom != null)
                        tempRoms.AddRange(dataArea.Rom);
                }

                if (tempRoms.Count > 0)
                    tempDataArea.Rom = [.. tempRoms];

                tempDataAreas.Add(tempDataArea);
            }

            return tempDataAreas;
        }

        /// <summary>
        /// Sanitize DiskArea list to ensure no duplicates exist
        /// <summary>
        private static List<Models.SoftwareList.DiskArea> SantitizeDiskAreas(List<Models.SoftwareList.DiskArea> diskAreas)
        {
            // If we have no DiskAreas, we can't do anything
            if (!diskAreas.Any())
                return diskAreas;

            var grouped = diskAreas.GroupBy(p => p.Name);

            var tempDiskAreas = new List<Models.SoftwareList.DiskArea>();
            foreach (var grouping in grouped)
            {
                var tempDiskArea = new Models.SoftwareList.DiskArea();
                var tempDisks = new List<Models.SoftwareList.Disk>();

                foreach (var dataArea in grouping)
                {
                    tempDiskArea.Name ??= dataArea.Name;
                    if (dataArea.Disk != null)
                        tempDisks.AddRange(dataArea.Disk);
                }

                if (tempDisks.Count > 0)
                    tempDiskArea.Disk = [.. tempDisks];

                tempDiskAreas.Add(tempDiskArea);
            }

            return tempDiskAreas;
        }

        #endregion
    }
}
