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
            return
            [
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

            return missingFields;
        }

        /// <inheritdoc/>
        public override bool WriteToFile(string outfile, bool ignoreblanks = false, bool throwOnError = false)
        {
            try
            {
                logger.User($"Writing to '{outfile}'...");

                var metadataFile = CreateMetadataFile(ignoreblanks);
                if (!(new Serialization.Files.AttractMode().Serialize(metadataFile, outfile)))
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

            return [.. rows];
        }

        /// <summary>
        /// Create a Row from the current Rom DatItem
        /// <summary>
        private Models.AttractMode.Row CreateRow(Rom rom)
        {
            var row = new Models.AttractMode.Row
            {
                Name = rom.Machine.GetFieldValue<string?>(Models.Metadata.Machine.NameKey),
                Title = rom.Machine.GetFieldValue<string?>(Models.Metadata.Machine.DescriptionKey),
                Emulator = Header.FileName,
                CloneOf = rom.Machine.GetFieldValue<string?>(Models.Metadata.Machine.CloneOfKey),
                Year = rom.Machine.GetFieldValue<string?>(Models.Metadata.Machine.YearKey),
                Manufacturer = rom.Machine.GetFieldValue<string?>(Models.Metadata.Machine.ManufacturerKey),
                Category = rom.Machine.GetFieldValue<string?>(Models.Metadata.Machine.CategoryKey),
                Players = rom.Machine.GetFieldValue<string?>(Models.Metadata.Machine.PlayersKey),
                Rotation = rom.Machine.GetFieldValue<string?>(Models.Metadata.Machine.RotationKey),
                Control = rom.Machine.GetFieldValue<string?>(Models.Metadata.Machine.ControlKey),
                Status = rom.Machine.GetFieldValue<string?>(Models.Metadata.Machine.StatusKey),
                DisplayCount = rom.Machine.GetFieldValue<string?>(Models.Metadata.Machine.DisplayCountKey),
                DisplayType = rom.Machine.GetFieldValue<string?>(Models.Metadata.Machine.DisplayTypeKey),
                AltRomname = rom.GetFieldValue<string?>(Models.Metadata.Rom.AltRomnameKey),
                AltTitle = rom.GetFieldValue<string?>(Models.Metadata.Rom.AltTitleKey),
                Extra = rom.Machine.GetFieldValue<string?>(Models.Metadata.Machine.CommentKey),
                Buttons = rom.Machine.GetFieldValue<string?>(Models.Metadata.Machine.ButtonsKey),
                // TODO: Add extended fields
            };
            return row;
        }

        #endregion
    }
}
