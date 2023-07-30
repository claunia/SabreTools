using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SabreTools.Core;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.IO.Writers;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing and writing of an Everdrive SMDB file
    /// </summary>
    internal partial class EverdriveSMDB : DatFile
    {
        /// <inheritdoc/>
        protected override ItemType[] GetSupportedTypes()
        {
            return new ItemType[] { ItemType.Rom };
        }

        /// <inheritdoc/>
        protected override List<DatItemField> GetMissingRequiredFields(DatItem datItem)
        {
            List<DatItemField> missingFields = new();

            // Check item name
            if (string.IsNullOrWhiteSpace(datItem.GetName()))
                missingFields.Add(DatItemField.Name);

            switch (datItem)
            {
                case Rom rom:
                    if (string.IsNullOrWhiteSpace(rom.SHA256))
                        missingFields.Add(DatItemField.SHA256);
                    if (string.IsNullOrWhiteSpace(rom.SHA1))
                        missingFields.Add(DatItemField.SHA1);
                    if (string.IsNullOrWhiteSpace(rom.MD5))
                        missingFields.Add(DatItemField.MD5);
                    if (string.IsNullOrWhiteSpace(rom.CRC))
                        missingFields.Add(DatItemField.CRC);
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
                if (!Serialization.EverdriveSMDB.SerializeToFile(metadataFile, outfile))
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

            return true;
        }

        #region Converters

        /// <summary>
        /// Create a MetadataFile from the current internal information
        /// <summary>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise</param>
        private Models.EverdriveSMDB.MetadataFile CreateMetadataFile(bool ignoreblanks)
        {
            var metadataFile = new Models.EverdriveSMDB.MetadataFile
            {
                Row = CreateRows(ignoreblanks)
            };
            return metadataFile;
        }

        /// <summary>
        /// Create an array of Row from the current internal information
        /// <summary>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise</param>
        private Models.EverdriveSMDB.Row[]? CreateRows(bool ignoreblanks)
        {
            // If we don't have items, we can't do anything
            if (this.Items == null || !this.Items.Any())
                return null;

            // Create a list of hold the rows
            var rows = new List<Models.EverdriveSMDB.Row>();

            // Loop through the sorted items and create games for them
            foreach (string key in Items.SortedKeys)
            {
                var items = Items.FilteredItems(key);
                if (items == null || !items.Any())
                    continue;

                // Loop through and convert the items to respective lists
                foreach (var item in items)
                {
                    // Skip if we're ignoring the item
                    if (ShouldIgnore(item, ignoreblanks))
                        continue;

                    switch (item)
                    {
                        case Rom rom:
                            rows.Add(CreateRow(rom));
                            break;
                    }
                }
            }

            return rows.ToArray();
        }

        /// <summary>
        /// Create a Row from the current Rom DatItem
        /// <summary>
        private static Models.EverdriveSMDB.Row CreateRow(Rom rom)
        {
            var row = new Models.EverdriveSMDB.Row
            {
                SHA256 = rom.SHA256,
                Name = $"{rom.Machine.Name}/{rom.Name}",
                SHA1 = rom.SHA1,
                MD5 = rom.MD5,
                CRC32 = rom.CRC,
                Size = rom.Size?.ToString(),
            };
            return row;
        }

        #endregion
    }
}
