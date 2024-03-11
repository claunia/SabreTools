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
    /// Represents writing a value-separated DAT
    /// </summary>
    internal partial class SeparatedValue : DatFile
    {
        /// <inheritdoc/>
        protected override ItemType[] GetSupportedTypes()
        {
            return
            [
                ItemType.Disk,
                ItemType.Media,
                ItemType.Rom
            ];
        }

        /// <inheritdoc/>
        protected override List<string>? GetMissingRequiredFields(DatItem datItem)
        {
            var missingFields = new List<string>();

            // Check item name
            if (string.IsNullOrEmpty(datItem.GetName()))
                missingFields.Add(Models.Metadata.Rom.NameKey);

            switch (datItem)
            {
                case Disk disk:
                    if (string.IsNullOrEmpty(disk.GetStringFieldValue(Models.Metadata.Disk.MD5Key))
                        && string.IsNullOrEmpty(disk.GetStringFieldValue(Models.Metadata.Disk.SHA1Key)))
                    {
                        missingFields.Add(Models.Metadata.Disk.SHA1Key);
                    }
                    break;

                case Rom rom:
                    if (rom.GetInt64FieldValue(Models.Metadata.Rom.SizeKey) == null || rom.GetInt64FieldValue(Models.Metadata.Rom.SizeKey) < 0)
                        missingFields.Add(Models.Metadata.Rom.SizeKey);
                    if (string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.CRCKey))
                        && string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.MD5Key))
                        && string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA1Key))
                        && string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA256Key))
                        && string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA384Key))
                        && string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA512Key))
                        && string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SpamSumKey)))
                    {
                        missingFields.Add(Models.Metadata.Rom.SHA1Key);
                    }
                    break;
            }

            return missingFields;
        }

        /// <inheritdoc/>
        public override bool WriteToFile(string outfile, bool ignoreblanks = false, bool throwOnError = false)
        {
            try
            {
                logger.User($"Writing to '{outfile}'...");

                var metadataFile = CreateMetadataFile(ignoreblanks);
                if (!(new Serialization.Files.SeparatedValue().Serialize(metadataFile, outfile, _delim)))
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
        /// Create a MetadataFile from the current internal information
        /// <summary>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise</param>
        private Models.SeparatedValue.MetadataFile CreateMetadataFile(bool ignoreblanks)
        {
            var metadataFile = new Models.SeparatedValue.MetadataFile
            {
                Row = CreateRows(ignoreblanks)
            };
            return metadataFile;
        }

        /// <summary>
        /// Create an array of Row from the current internal information
        /// <summary>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise</param>
        private Models.SeparatedValue.Row[]? CreateRows(bool ignoreblanks)
        {
            // If we don't have items, we can't do anything
            if (this.Items == null || !this.Items.Any())
                return null;

            // Create a list of hold the rows
            var rows = new List<Models.SeparatedValue.Row>();

            // Loop through the sorted items and create games for them
            foreach (string key in Items.SortedKeys)
            {
                var items = Items.FilteredItems(key);
                if (items == null || !items.Any())
                    continue;

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
                        case Disk disk:
                            rows.Add(CreateRow(disk));
                            break;
                        case Media media:
                            rows.Add(CreateRow(media));
                            break;
                        case Rom rom:
                            rows.Add(CreateRow(rom));
                            break;
                    }
                }
            }

            return [.. rows];
        }

        /// <summary>
        /// Create a Row from the current Disk DatItem
        /// <summary>
        private Models.SeparatedValue.Row CreateRow(Disk disk)
        {
            var row = new Models.SeparatedValue.Row
            {
                FileName = Header.GetStringFieldValue(DatHeader.FileNameKey),
                InternalName = Header.GetStringFieldValue(Models.Metadata.Header.NameKey),
                Description = Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey),
                GameName = disk.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey),
                GameDescription = disk.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.DescriptionKey),
                Type = disk.GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>().AsStringValue(),
                RomName = string.Empty,
                DiskName = disk.GetName(),
                Size = string.Empty,
                CRC = string.Empty,
                MD5 = disk.GetStringFieldValue(Models.Metadata.Disk.MD5Key),
                SHA1 = disk.GetStringFieldValue(Models.Metadata.Disk.SHA1Key),
                SHA256 = string.Empty,
                SHA384 = string.Empty,
                SHA512 = string.Empty,
                SpamSum = string.Empty,
                Status = disk.GetStringFieldValue(Models.Metadata.Disk.StatusKey).AsEnumValue<ItemStatus>().AsStringValue(useSecond: false),
            };
            return row;
        }

        /// <summary>
        /// Create a Row from the current Media DatItem
        /// <summary>
        private Models.SeparatedValue.Row CreateRow(Media media)
        {
            var row = new Models.SeparatedValue.Row
            {
                FileName = Header.GetStringFieldValue(DatHeader.FileNameKey),
                InternalName = Header.GetStringFieldValue(Models.Metadata.Header.NameKey),
                Description = Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey),
                GameName = media.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey),
                GameDescription = media.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.DescriptionKey),
                Type = media.GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>().AsStringValue(),
                RomName = string.Empty,
                DiskName = media.GetName(),
                Size = string.Empty,
                CRC = string.Empty,
                MD5 = media.GetStringFieldValue(Models.Metadata.Media.MD5Key),
                SHA1 = media.GetStringFieldValue(Models.Metadata.Media.SHA1Key),
                SHA256 = media.GetStringFieldValue(Models.Metadata.Media.SHA256Key),
                SHA384 = string.Empty,
                SHA512 = string.Empty,
                SpamSum = media.GetStringFieldValue(Models.Metadata.Media.SpamSumKey),
                Status = string.Empty,
            };
            return row;
        }

        /// <summary>
        /// Create a Row from the current Rom DatItem
        /// <summary>
        private Models.SeparatedValue.Row CreateRow(Rom rom)
        {
            var row = new Models.SeparatedValue.Row
            {
                FileName = Header.GetStringFieldValue(DatHeader.FileNameKey),
                InternalName = Header.GetStringFieldValue(Models.Metadata.Header.NameKey),
                Description = Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey),
                GameName = rom.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey),
                GameDescription = rom.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.DescriptionKey),
                Type = rom.GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>().AsStringValue(),
                RomName = rom.GetName(),
                DiskName = string.Empty,
                Size = rom.GetInt64FieldValue(Models.Metadata.Rom.SizeKey).ToString(),
                CRC = rom.GetStringFieldValue(Models.Metadata.Rom.CRCKey),
                MD5 = rom.GetStringFieldValue(Models.Metadata.Rom.MD5Key),
                SHA1 = rom.GetStringFieldValue(Models.Metadata.Rom.SHA1Key),
                SHA256 = rom.GetStringFieldValue(Models.Metadata.Rom.SHA256Key),
                SHA384 = rom.GetStringFieldValue(Models.Metadata.Rom.SHA384Key),
                SHA512 = rom.GetStringFieldValue(Models.Metadata.Rom.SHA512Key),
                SpamSum = rom.GetStringFieldValue(Models.Metadata.Rom.SpamSumKey),
                Status = rom.GetStringFieldValue(Models.Metadata.Rom.StatusKey).AsEnumValue<ItemStatus>().AsStringValue(useSecond: false),
            };
            return row;
        }

        #endregion
    }
}
