using System;
using System.Collections.Generic;
using System.Linq;
using SabreTools.Core;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents writing an openMSX softawre list XML DAT
    /// </summary>
    internal partial class OpenMSX : DatFile
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
                    if (string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA1Key)))
                        missingFields.Add(Models.Metadata.Rom.SHA1Key);
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

                // TODO: Write out comment prefix somehow
                var softwaredb = CreateSoftwareDb(ignoreblanks);
                if (!(new Serialization.Files.OpenMSX().SerializeToFileWithDocType(softwaredb, outfile)))
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
        /// Create a SoftwareDb from the current internal information
        /// <summary>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise</param>
        private Models.OpenMSX.SoftwareDb CreateSoftwareDb(bool ignoreblanks)
        {
            var softwaredb = new Models.OpenMSX.SoftwareDb
            {
                Timestamp = Header.GetStringFieldValue(Models.Metadata.Header.DateKey),
                Software = CreateSoftwares(ignoreblanks)
            };
            return softwaredb;
        }

        /// <summary>
        /// Create an array of Software from the current internal information
        /// <summary>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise</param>
        private Models.OpenMSX.Software[]? CreateSoftwares(bool ignoreblanks)
        {
            // If we don't have items, we can't do anything
            if (this.Items == null || !this.Items.Any())
                return null;

            // Create a list of hold the games
            var softwares = new List<Models.OpenMSX.Software>();

            // Loop through the sorted items and create games for them
            foreach (string key in Items.SortedKeys)
            {
                var items = Items.FilteredItems(key);
                if (items == null || !items.Any())
                    continue;

                // Get the first item for game information
                var machine = items[0].GetFieldValue<Machine>(DatItem.MachineKey);
                var software = new Models.OpenMSX.Software
                {
                    Title = machine?.GetStringFieldValue(Models.Metadata.Machine.NameKey),
                    GenMSXID = machine?.GetStringFieldValue(Models.Metadata.Machine.GenMSXIDKey),
                    System = machine?.GetStringFieldValue(Models.Metadata.Machine.SystemKey),
                    Company = machine?.GetStringFieldValue(Models.Metadata.Machine.ManufacturerKey),
                    Year = machine?.GetStringFieldValue(Models.Metadata.Machine.YearKey),
                    Country = machine?.GetStringFieldValue(Models.Metadata.Machine.CountryKey),
                };

                // Create holder for dumps
                var dumps = new List<Models.OpenMSX.Dump>();

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
                            dumps.Add(CreateDump(rom));
                            break;
                    }
                }

                software.Dump = [.. dumps];
                softwares.Add(software);
            }

            return [.. softwares];
        }

        /// <summary>
        /// Create a Dump from the current Rom DatItem
        /// <summary>
        private static Models.OpenMSX.Dump CreateDump(Rom item)
        {
            Models.OpenMSX.Original? original = null;
            if (item.OriginalSpecified && item.GetFieldValue<Original?>("ORIGINAL") != null)
            {
                original = new Models.OpenMSX.Original { Content = item.GetFieldValue<Original?>("ORIGINAL")!.Content };
                if (item.GetFieldValue<Original?>("ORIGINAL")!.Value != null)
                    original.Value = item.GetFieldValue<Original?>("ORIGINAL")!.Value.ToString();
            }

            Models.OpenMSX.RomBase rom = item.GetFieldValue<OpenMSXSubType>(Models.Metadata.Rom.OpenMSXMediaType) switch
            {
                OpenMSXSubType.MegaRom => new Models.OpenMSX.MegaRom(),
                OpenMSXSubType.SCCPlusCart => new Models.OpenMSX.SCCPlusCart(),
                _ => new Models.OpenMSX.Rom(),
            };

            rom.Start = item.GetStringFieldValue(Models.Metadata.Rom.OffsetKey);
            rom.Type = item.GetStringFieldValue(Models.Metadata.Rom.OpenMSXType);
            rom.Hash = item.GetStringFieldValue(Models.Metadata.Rom.SHA1Key);
            rom.Remark = item.GetStringFieldValue(Models.Metadata.Rom.RemarkKey);

            var dump = new Models.OpenMSX.Dump
            {
                Original = original,
                Rom = rom,
            };

            return dump;
        }

        #endregion
    }
}
