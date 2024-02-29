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
        protected override List<DatItemField>? GetMissingRequiredFields(DatItem datItem)
        {
            List<DatItemField> missingFields = [];

            // Check item name
            if (string.IsNullOrWhiteSpace(datItem.GetName()))
                missingFields.Add(DatItemField.Name);

            switch (datItem)
            {
                case Disk disk:
                    if (string.IsNullOrWhiteSpace(disk.MD5)
                        && string.IsNullOrWhiteSpace(disk.SHA1))
                    {
                        missingFields.Add(DatItemField.SHA1);
                    }
                    break;

                case Rom rom:
                    if (rom.Size == null || rom.Size < 0)
                        missingFields.Add(DatItemField.Size);
                    if (string.IsNullOrWhiteSpace(rom.CRC))
                        missingFields.Add(DatItemField.CRC);
                    if (string.IsNullOrWhiteSpace(rom.SHA1))
                        missingFields.Add(DatItemField.SHA1);
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
                    Driver = items[0]!.Machine!.MachineType.HasFlag(MachineType.Device) ? items[0]!.Machine!.Name : null,
                    Device = items[0]!.Machine!.MachineType.HasFlag(MachineType.Device) ? items[0]!.Machine!.Name : null,
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
            if (disk.ItemStatus == ItemStatus.Nodump)
            {
                return new Models.Listrom.Row
                {
                    Name = disk.Name,
                    NoGoodDumpKnown = true,
                };
            }
            else if (disk.ItemStatus == ItemStatus.BadDump)
            {
                var row = new Models.Listrom.Row
                {
                    Name = disk.Name,
                    Bad = true,
                };

                if (!string.IsNullOrWhiteSpace(disk.MD5))
                    row.MD5 = disk.MD5;
                else
                    row.SHA1 = disk.SHA1;

                return row;
            }
            else
            {
                var row = new Models.Listrom.Row
                {
                    Name = disk.Name,
                };

                if (!string.IsNullOrWhiteSpace(disk.MD5))
                    row.MD5 = disk.MD5;
                else
                    row.SHA1 = disk.SHA1;

                return row;
            }
        }

        /// <summary>
        /// Create a Row from the current Rom DatItem
        /// <summary>
        private static Models.Listrom.Row? CreateRow(Rom rom)
        {
            if (rom.ItemStatus == ItemStatus.Nodump)
            {
                return new Models.Listrom.Row
                {
                    Name = rom.Name,
                    Size = rom.Size?.ToString(),
                    NoGoodDumpKnown = true,
                };
            }
            else if (rom.ItemStatus == ItemStatus.BadDump)
            {
                return new Models.Listrom.Row
                {
                    Name = rom.Name,
                    Size = rom.Size?.ToString(),
                    Bad = true,
                    CRC = rom.CRC,
                    SHA1 = rom.SHA1,
                };
            }
            else
            {
                return new Models.Listrom.Row
                {
                    Name = rom.Name,
                    Size = rom.Size?.ToString(),
                    CRC = rom.CRC,
                    SHA1 = rom.SHA1,
                };
            }
        }

        #endregion
    }
}
