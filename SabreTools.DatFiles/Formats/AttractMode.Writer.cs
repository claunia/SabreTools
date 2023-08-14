using System;
using System.Collections.Generic;
using System.Linq;
using SabreTools.Core;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing and writing of an AttractMode DAT
    /// </summary>
    internal partial class AttractMode : DatFile
    {
        /// <inheritdoc/>
        protected override ItemType[] GetSupportedTypes()
        {
            return new ItemType[] { ItemType.Rom };
        }

        /// <inheritdoc/>
        protected override List<DatItemField>? GetMissingRequiredFields(DatItem datItem)
        {
            List<DatItemField> missingFields = new();

            // Check item name
            if (string.IsNullOrWhiteSpace(datItem.GetName()))
                missingFields.Add(DatItemField.Name);

            return missingFields;
        }

        /// <inheritdoc/>
        public override bool WriteToFile(string outfile, bool ignoreblanks = false, bool throwOnError = false)
        {
            try
            {
                logger.User($"Writing to '{outfile}'...");

                var metadataFile = CreateMetadataFile(ignoreblanks);
                if (!Serialization.AttractMode.SerializeToFile(metadataFile, outfile))
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
        private Models.AttractMode.MetadataFile CreateMetadataFile(bool ignoreblanks)
        {
            var metadataFile = new Models.AttractMode.MetadataFile
            {
                Row = CreateRows(ignoreblanks)
            };
            return metadataFile;
        }

        /// <summary>
        /// Create an array of Row from the current internal information
        /// <summary>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise</param>
        private Models.AttractMode.Row[]? CreateRows(bool ignoreblanks)
        {
            // If we don't have items, we can't do anything
            if (this.Items == null || !this.Items.Any())
                return null;

            // Create a list of hold the rows
            var rows = new List<Models.AttractMode.Row>();

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
        private Models.AttractMode.Row CreateRow(Rom rom)
        {
            var row = new Models.AttractMode.Row
            {
                Name = rom.Machine?.Name,
                Title = rom.Machine?.Description,
                Emulator = Header.FileName,
                CloneOf = rom.Machine?.CloneOf,
                Year = rom.Machine?.Year,
                Manufacturer = rom.Machine?.Manufacturer,
                Category = rom.Machine?.Category,
                Players = rom.Machine?.Players,
                Rotation = rom.Machine?.Rotation,
                Control = rom.Machine?.Control,
                Status = rom.Machine?.Status,
                DisplayCount = rom.Machine?.DisplayCount,
                DisplayType = rom.Machine?.DisplayType,
                AltRomname = rom.AltName,
                AltTitle = rom.AltTitle,
                Extra = rom.Machine?.Comment,
                Buttons = rom.Machine?.Buttons,
                // TODO: Add extended fields
            };
            return row;
        }

        #endregion
    }
}
