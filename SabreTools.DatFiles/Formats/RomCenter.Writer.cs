using System;
using System.Collections.Generic;
using System.Linq;
using SabreTools.Core;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents writing a RomCenter INI file
    /// </summary>
    internal partial class RomCenter : DatFile
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

            switch (datItem)
            {
                case Rom rom:
                    if (string.IsNullOrEmpty(rom.GetFieldValue<string?>(Models.Metadata.Rom.CRCKey)))
                        missingFields.Add(Models.Metadata.Rom.CRCKey);
                    if (rom.GetFieldValue<long?>(Models.Metadata.Rom.SizeKey) == null)
                        missingFields.Add(Models.Metadata.Rom.SizeKey);
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
                if (!(new Serialization.Files.RomCenter().Serialize(metadataFile, outfile)))
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
        private Models.RomCenter.MetadataFile CreateMetadataFile(bool ignoreblanks)
        {
            var metadataFile = new Models.RomCenter.MetadataFile
            {
                Credits = CreateCredits(),
                Dat = CreateDat(),
                Emulator = CreateEmulator(),
                Games = CreateGames(ignoreblanks),
            };
            return metadataFile;
        }

        /// <summary>
        /// Create a Credits from the current internal information
        /// <summary>
        private Models.RomCenter.Credits CreateCredits()
        {
            var credits = new Models.RomCenter.Credits
            {
                Author = Header.GetFieldValue<string?>(Models.Metadata.Header.AuthorKey),
                Version = Header.GetFieldValue<string?>(Models.Metadata.Header.VersionKey),
                Email = Header.GetFieldValue<string?>(Models.Metadata.Header.EmailKey),
                Homepage = Header.GetFieldValue<string?>(Models.Metadata.Header.HomepageKey),
                Url = Header.GetFieldValue<string?>(Models.Metadata.Header.UrlKey),
                Date = Header.GetFieldValue<string?>(Models.Metadata.Header.DateKey),
                Comment = Header.GetFieldValue<string?>(Models.Metadata.Header.CommentKey),
            };
            return credits;
        }

        /// <summary>
        /// Create a Dat from the current internal information
        /// <summary>
        private Models.RomCenter.Dat CreateDat()
        {
            var dat = new Models.RomCenter.Dat
            {
                Version = Header.GetFieldValue<string?>(Models.Metadata.Header.DatVersionKey),
                Plugin = Header.GetFieldValue<string?>(Models.Metadata.Header.SystemKey),
                Split = (Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.ForceMergingKey) == MergingFlag.Split ? "1" : "0"),
                Merge = (Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.ForceMergingKey) == MergingFlag.Merged || Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.ForceMergingKey) == MergingFlag.FullMerged ? "1" : "0"),
            };
            return dat;
        }

        /// <summary>
        /// Create a Emulator from the current internal information
        /// <summary>
        private Models.RomCenter.Emulator CreateEmulator()
        {
            var emulator = new Models.RomCenter.Emulator
            {
                RefName = Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey),
                Version = Header.GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey),
            };
            return emulator;
        }

        /// <summary>
        /// Create a Games from the current internal information
        /// <summary>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise</param>
        private Models.RomCenter.Games? CreateGames(bool ignoreblanks)
        {
            // If we don't have items, we can't do anything
            if (this.Items == null || !this.Items.Any())
                return null;

            // Create a list of hold the roms
            var roms = new List<Models.RomCenter.Rom>();

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
                            roms.Add(CreateRom(rom));
                            break;
                    }
                }
            }

            return new Models.RomCenter.Games { Rom = [.. roms] };
        }

        /// <summary>
        /// Create a Rom from the current Rom DatItem
        /// <summary>
        private static Models.RomCenter.Rom CreateRom(Rom item)
        {
            var rom = new Models.RomCenter.Rom
            {
                ParentName = item.Machine.GetFieldValue<string?>(Models.Metadata.Machine.CloneOfKey),
                //ParentDescription = item.Machine.GetFieldValue<string?>(Models.Metadata.Machine.CloneOfDescription), // TODO: Add to internal model or find mapping
                GameName = item.Machine.GetFieldValue<string?>(Models.Metadata.Machine.NameKey),
                GameDescription = item.Machine.GetFieldValue<string?>(Models.Metadata.Machine.DescriptionKey),
                RomName = item.GetName(),
                RomCRC = item.GetFieldValue<string?>(Models.Metadata.Rom.CRCKey),
                RomSize = item.GetFieldValue<long?>(Models.Metadata.Rom.SizeKey)?.ToString(),
                RomOf = item.Machine.GetFieldValue<string?>(Models.Metadata.Machine.RomOfKey),
                MergeName = item.GetFieldValue<string?>(Models.Metadata.Rom.MergeKey),
            };
            return rom;
        }

        #endregion
    }
}
