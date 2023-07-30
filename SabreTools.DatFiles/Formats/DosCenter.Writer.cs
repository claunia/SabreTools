using System;
using System.Collections.Generic;
using System.Linq;
using SabreTools.Core;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing and writing of a DosCenter DAT
    /// </summary>
    internal partial class DosCenter : DatFile
    {
        /// <inheritdoc/>
        protected override ItemType[] GetSupportedTypes()
        {
            return new ItemType[]
            {
                ItemType.Rom
            };
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
                    if (!rom.SizeSpecified)
                        missingFields.Add(DatItemField.Size);
                    // if (string.IsNullOrWhiteSpace(rom.Date))
                    //     missingFields.Add(DatItemField.Date);
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
                if (!Serialization.DosCenter.SerializeToFile(metadataFile, outfile))
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
        private Models.DosCenter.MetadataFile CreateMetadataFile(bool ignoreblanks)
        {
            var metadataFile = new Models.DosCenter.MetadataFile
            {
                DosCenter = CreateDosCenter(),
                Game = CreateGames(ignoreblanks)
            };
            return metadataFile;
        }

        /// <summary>
        /// Create a DosCenter from the current internal information
        /// <summary>
        private Models.DosCenter.DosCenter? CreateDosCenter()
        {
            // If we don't have a header, we can't do anything
            if (this.Header == null)
                return null;

            var clrMamePro = new Models.DosCenter.DosCenter
            {
                Name = Header.Name,
                Description = Header.Description,
                Version = Header.Version,
                Date = Header.Date,
                Author = Header.Author,
                Homepage = Header.Homepage,
                Comment = Header.Comment,
            };

            return clrMamePro;
        }

        /// <summary>
        /// Create an array of GameBase from the current internal information
        /// <summary>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise</param>
        private Models.DosCenter.Game[]? CreateGames(bool ignoreblanks)
        {
            // If we don't have items, we can't do anything
            if (this.Items == null || !this.Items.Any())
                return null;

            // Create a list of hold the games
            var games = new List<Models.DosCenter.Game>();

            // Loop through the sorted items and create games for them
            foreach (string key in Items.SortedKeys)
            {
                var items = Items.FilteredItems(key);
                if (items == null || !items.Any())
                    continue;

                // Get the first item for game information
                var machine = items[0].Machine;

                // We re-add the missing parts of the game name
                var game = new Models.DosCenter.Game
                {
                    Name = $"\"{machine.Name}.zip\""
                };

                // Create holders for all item types
                var files = new List<Models.DosCenter.File>();

                // Loop through and convert the items to respective lists
                foreach (var item in items)
                {
                    // Skip if we're ignoring the item
                    if (ShouldIgnore(item, ignoreblanks))
                        continue;

                    switch (item)
                    {
                        case Rom rom:
                            files.Add(CreateFile(rom));
                            break;
                    }
                }

                // Assign the values to the game
                game.File = files.ToArray();

                // Add the game to the list
                games.Add(game);
            }

            return games.ToArray();
        }

        /// <summary>
        /// Create a File from the current Rom DatItem
        /// <summary>
        private static Models.DosCenter.File CreateFile(Rom item)
        {
            var rom = new Models.DosCenter.File
            {
                Name = item.Name,
                Size = item.Size?.ToString(),
                CRC = item.CRC,
                Date = item.Date,
            };
            return rom;
        }

        #endregion
    }
}
