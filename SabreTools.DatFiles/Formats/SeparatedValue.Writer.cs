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
                    if (string.IsNullOrEmpty(disk.MD5)
                        && string.IsNullOrEmpty(disk.SHA1))
                    {
                        missingFields.Add(Models.Metadata.Disk.SHA1Key);
                    }
                    break;

                case Rom rom:
                    if (rom.Size == null || rom.Size < 0)
                        missingFields.Add(Models.Metadata.Rom.SizeKey);
                    if (string.IsNullOrEmpty(rom.CRC)
                        && string.IsNullOrEmpty(rom.MD5)
                        && string.IsNullOrEmpty(rom.SHA1)
                        && string.IsNullOrEmpty(rom.SHA256)
                        && string.IsNullOrEmpty(rom.SHA384)
                        && string.IsNullOrEmpty(rom.SHA512)
                        && string.IsNullOrEmpty(rom.SpamSum))
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
                FileName = Header.FileName,
                InternalName = Header.Name,
                Description = Header.Description,
                GameName = disk.Machine.Name,
                GameDescription = disk.Machine.Description,
                Type = disk.ItemType.AsStringValue<ItemType>(),
                RomName = string.Empty,
                DiskName = disk.Name,
                Size = string.Empty,
                CRC = string.Empty,
                MD5 = disk.MD5,
                SHA1 = disk.SHA1,
                SHA256 = string.Empty,
                SHA384 = string.Empty,
                SHA512 = string.Empty,
                SpamSum = string.Empty,
                Status = disk.ItemStatus.AsStringValue<ItemStatus>(useSecond: false),
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
                FileName = Header.FileName,
                InternalName = Header.Name,
                Description = Header.Description,
                GameName = media.Machine.Name,
                GameDescription = media.Machine.Description,
                Type = media.ItemType.AsStringValue<ItemType>(),
                RomName = string.Empty,
                DiskName = media.Name,
                Size = string.Empty,
                CRC = string.Empty,
                MD5 = media.MD5,
                SHA1 = media.SHA1,
                SHA256 = media.SHA256,
                SHA384 = string.Empty,
                SHA512 = string.Empty,
                SpamSum = media.SpamSum,
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
                FileName = Header.FileName,
                InternalName = Header.Name,
                Description = Header.Description,
                GameName = rom.Machine.Name,
                GameDescription = rom.Machine.Description,
                Type = rom.ItemType.AsStringValue<ItemType>(),
                RomName = rom.Name,
                DiskName = string.Empty,
                Size = rom.Size?.ToString(),
                CRC = rom.CRC,
                MD5 = rom.MD5,
                SHA1 = rom.SHA1,
                SHA256 = rom.SHA256,
                SHA384 = rom.SHA384,
                SHA512 = rom.SHA512,
                SpamSum = rom.SpamSum,
                Status = rom.ItemStatus.AsStringValue<ItemStatus>(useSecond: false),
            };
            return row;
        }

        #endregion
    }
}
