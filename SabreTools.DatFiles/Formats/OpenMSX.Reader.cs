using System;
using System.Linq;
using SabreTools.Core;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing an openMSX softawre list XML DAT
    /// </summary>
    internal partial class OpenMSX : DatFile
    {
        /// <inheritdoc/>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            try
            {
                // Deserialize the input file
                var softwareDb = Serialization.OpenMSX.Deserialize(filename);

                // Convert the header to the internal format
                ConvertHeader(softwareDb);

                // Convert the software data to the internal format
                ConvertSoftwares(softwareDb?.Software, filename, indexId, statsOnly);
            }
            catch (Exception ex) when (!throwOnError)
            {
                string message = $"'{filename}' - An error occurred during parsing";
                logger.Error(ex, message);
            }
        }

        #region Converters

        /// <summary>
        /// Convert header information
        /// </summary>
        /// <param name="datafile">Deserialized model to convert</param>
        private void ConvertHeader(Models.OpenMSX.SoftwareDb? datafile)
        {
            // If the datafile is missing, we can't do anything
            if (datafile == null)
                return;

            Header.Name ??= "openMSX Software List";
            Header.Description ??= Header.Name;
            Header.Date ??= datafile.Timestamp;
        }

        /// <summary>
        /// Convert softwares information
        /// </summary>
        /// <param name="softwares">Array of deserialized models to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertSoftwares(Models.OpenMSX.Software[]? softwares, string filename, int indexId, bool statsOnly)
        {
            // If the software array is missing, we can't do anything
            if (softwares == null || !softwares.Any())
                return;

            // Loop through the software and add
            foreach (var software in softwares)
            {
                ConvertSoftware(software, filename, indexId, statsOnly);
            }
        }

        /// <summary>
        /// Convert software information
        /// </summary>
        /// <param name="software">Deserialized model to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertSoftware(Models.OpenMSX.Software software, string filename, int indexId, bool statsOnly, string dirname = null)
        {
            // If the software is missing, we can't do anything
            if (software == null)
                return;

            // Create the machine for copying information
            var machine = new Machine
            {
                Name = software.Title,
                GenMSXID = software.GenMSXID,
                System = software.System,
                Manufacturer = software.Company,
                Year = software.Year,
                Country = software.Country,
            };

            // Check if there are any items
            bool containsItems = false;
            ConvertDumps(software.Dump, machine, filename, indexId, statsOnly, ref containsItems);

            // If we had no items, create a Blank placeholder
            if (!containsItems)
            {
                var blank = new Blank
                {
                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

                blank.CopyMachineInformation(machine);
                ParseAddHelper(blank, statsOnly);
            }
        }

        /// <summary>
        /// Convert Dump information
        /// </summary>
        /// <param name="dumps">Array of deserialized models to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertDumps(Models.OpenMSX.Dump[]? dumps, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the dumps array is missing, we can't do anything
            if (dumps == null || !dumps.Any())
                return;

            containsItems = true;
            int index = 0;
            foreach (var dump in dumps)
            {
                // If we don't have rom data, we can't do anything
                if (dump?.Rom == null)
                    continue;

                var rom = dump.Rom;

                string name = $"{machine.Name}_{index++}{(!string.IsNullOrWhiteSpace(rom.Remark) ? $" {rom.Remark}" : string.Empty)}";
                var item = new Rom
                {
                    Name = name,
                    Offset = dump.Rom?.Start,
                    OpenMSXType = rom.Type,
                    SHA1 = rom.Hash,
                    Remark = rom.Remark,

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

                if (dump.Original != null)
                {
                    item.Original = new Original
                    {
                        Value = dump.Original.Value,
                        Content = dump.Original.Content,
                    };
                }

                switch (dump.Rom)
                {
                    case Models.OpenMSX.Rom:
                        item.OpenMSXSubType = OpenMSXSubType.Rom;
                        break;
                    case Models.OpenMSX.MegaRom:
                        item.OpenMSXSubType = OpenMSXSubType.MegaRom;
                        break;
                    case Models.OpenMSX.SCCPlusCart:
                        item.OpenMSXSubType = OpenMSXSubType.SCCPlusCart;
                        break;
                }

                item.CopyMachineInformation(machine);
                ParseAddHelper(item, statsOnly);
            }
        }

        #endregion
    }
}
