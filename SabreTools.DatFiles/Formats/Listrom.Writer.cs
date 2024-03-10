using System;
using System.Collections.Generic;
using System.Linq;
using SabreTools.Core;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents writing a MAME Listrom file
    /// </summary>
    internal partial class Listrom : DatFile
    {
        /// <inheritdoc/>
        protected override ItemType[] GetSupportedTypes()
        {
            return
            [
                ItemType.Disk,
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
                    if (string.IsNullOrEmpty(disk.GetFieldValue<string?>(Models.Metadata.Disk.MD5Key))
                        && string.IsNullOrEmpty(disk.GetFieldValue<string?>(Models.Metadata.Disk.SHA1Key)))
                    {
                        missingFields.Add(Models.Metadata.Disk.SHA1Key);
                    }
                    break;

                case Rom rom:
                    if (rom.GetFieldValue<long?>(Models.Metadata.Rom.SizeKey) == null || rom.GetFieldValue<long?>(Models.Metadata.Rom.SizeKey) < 0)
                        missingFields.Add(Models.Metadata.Rom.SizeKey);
                    if (string.IsNullOrEmpty(rom.GetFieldValue<string?>(Models.Metadata.Rom.CRCKey)))
                        missingFields.Add(Models.Metadata.Rom.CRCKey);
                    if (string.IsNullOrEmpty(rom.GetFieldValue<string?>(Models.Metadata.Rom.SHA1Key)))
                        missingFields.Add(Models.Metadata.Rom.SHA1Key);
                    break;
            }

            return null;
        }

        /// <inheritdoc/>
        public override bool WriteToFile(string outfile, bool ignoreblanks = false, bool throwOnError = false)
        {
            try
            {
                logger.User($"Writing to '{outfile}'...");

                var metadataFile = CreateMetadataFile(ignoreblanks);
                if (!(new Serialization.Files.Listrom().Serialize(metadataFile, outfile)))
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
        private Models.Listrom.MetadataFile CreateMetadataFile(bool ignoreblanks)
        {
            var metadataFile = new Models.Listrom.MetadataFile
            {
                Set = CreateSets(ignoreblanks)
            };
            return metadataFile;
        }

        /// <summary>
        /// Create an array of Set from the current internal information
        /// <summary>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise</param>
        private Models.Listrom.Set[]? CreateSets(bool ignoreblanks)
        {
            // If we don't have items, we can't do anything
            if (this.Items == null || !this.Items.Any())
                return null;

            // Create lists to hold the data
            var sets = new List<Models.Listrom.Set>();
            var rows = new List<Models.Listrom.Row>();

            // Loop through the sorted items and create games for them
            foreach (string key in Items.SortedKeys)
            {
                var items = Items.FilteredItems(key);
                if (items == null || !items.Any())
                    continue;

                var set = new Models.Listrom.Set
                {
#if NETFRAMEWORK
                    Driver = (items[0]!.Machine!.MachineType & MachineType.Device) != 0 ? items[0]!.Machine!.Name : null,
                    Device = (items[0]!.Machine!.MachineType & MachineType.Device) != 0 ? items[0]!.Machine!.Name : null,
#else
                    Driver = items[0]!.Machine!.MachineType.HasFlag(MachineType.Device) ? items[0]!.Machine!.Name : null,
                    Device = items[0]!.Machine!.MachineType.HasFlag(MachineType.Device) ? items[0]!.Machine!.Name : null,
#endif
                };

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
                            var diskRow = CreateRow(disk);
                            if (diskRow != null)
                                rows.Add(diskRow);
                            break;
                        case Rom rom:
                            var romRow = CreateRow(rom);
                            if (romRow != null)
                                rows.Add(romRow);
                            break;
                    }
                }

                set.Row = [.. rows];
                sets.Add(set);
                rows.Clear();
            }

            return [.. sets];
        }

        /// <summary>
        /// Create a Row from the current Disk DatItem
        /// <summary>
        private static Models.Listrom.Row? CreateRow(Disk disk)
        {
            if (disk.GetFieldValue<ItemStatus>(Models.Metadata.Disk.StatusKey) == ItemStatus.Nodump)
            {
                return new Models.Listrom.Row
                {
                    Name = disk.GetName(),
                    NoGoodDumpKnown = true,
                };
            }
            else if (disk.GetFieldValue<ItemStatus>(Models.Metadata.Disk.StatusKey) == ItemStatus.BadDump)
            {
                var row = new Models.Listrom.Row
                {
                    Name = disk.GetName(),
                    Bad = true,
                };

                if (!string.IsNullOrEmpty(disk.GetFieldValue<string?>(Models.Metadata.Disk.MD5Key)))
                    row.MD5 = disk.GetFieldValue<string?>(Models.Metadata.Disk.MD5Key);
                else
                    row.SHA1 = disk.GetFieldValue<string?>(Models.Metadata.Disk.SHA1Key);

                return row;
            }
            else
            {
                var row = new Models.Listrom.Row
                {
                    Name = disk.GetName(),
                };

                if (!string.IsNullOrEmpty(disk.GetFieldValue<string?>(Models.Metadata.Disk.MD5Key)))
                    row.MD5 = disk.GetFieldValue<string?>(Models.Metadata.Disk.MD5Key);
                else
                    row.SHA1 = disk.GetFieldValue<string?>(Models.Metadata.Disk.SHA1Key);

                return row;
            }
        }

        /// <summary>
        /// Create a Row from the current Rom DatItem
        /// <summary>
        private static Models.Listrom.Row? CreateRow(Rom rom)
        {
            if (rom.GetFieldValue<ItemStatus>(Models.Metadata.Rom.StatusKey) == ItemStatus.Nodump)
            {
                return new Models.Listrom.Row
                {
                    Name = rom.GetName(),
                    Size = rom.GetFieldValue<long?>(Models.Metadata.Rom.SizeKey)?.ToString(),
                    NoGoodDumpKnown = true,
                };
            }
            else if (rom.GetFieldValue<ItemStatus>(Models.Metadata.Rom.StatusKey) == ItemStatus.BadDump)
            {
                return new Models.Listrom.Row
                {
                    Name = rom.GetName(),
                    Size = rom.GetFieldValue<long?>(Models.Metadata.Rom.SizeKey)?.ToString(),
                    Bad = true,
                    CRC = rom.GetFieldValue<string?>(Models.Metadata.Rom.CRCKey),
                    SHA1 = rom.GetFieldValue<string?>(Models.Metadata.Rom.SHA1Key),
                };
            }
            else
            {
                return new Models.Listrom.Row
                {
                    Name = rom.GetName(),
                    Size = rom.GetFieldValue<long?>(Models.Metadata.Rom.SizeKey)?.ToString(),
                    CRC = rom.GetFieldValue<string?>(Models.Metadata.Rom.CRCKey),
                    SHA1 = rom.GetFieldValue<string?>(Models.Metadata.Rom.SHA1Key),
                };
            }
        }

        #endregion
    }
}
