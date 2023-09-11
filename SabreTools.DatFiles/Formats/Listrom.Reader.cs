using System;
using System.Linq;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing a MAME Listrom file
    /// </summary>
    internal partial class Listrom : DatFile
    {
        /// <inheritdoc/>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            try
            {
                // Deserialize the input file
                var metadataFile = new Serialization.Files.Listrom().Deserialize(filename);

                // Convert the set data to the internal format
                ConvertSets(metadataFile?.Set, filename, indexId, statsOnly);
            }
            catch (Exception ex) when (!throwOnError)
            {
                string message = $"'{filename}' - An error occurred during parsing";
                logger.Error(ex, message);
            }
        }

        #region Converters

        /// <summary>
        /// Convert sets information
        /// </summary>
        /// <param name="sets">Array of deserialized models to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertSets(Models.Listrom.Set[]? sets, string filename, int indexId, bool statsOnly)
        {
            // If the rows array is missing, we can't do anything
            if (sets == null || !sets.Any())
                return;

            // Loop through the sets and add
            foreach (var set in sets)
            {
                ConvertSet(set, filename, indexId, statsOnly);
            }
        }

        /// <summary>
        /// Convert set information
        /// </summary>
        /// <param name="set">Deserialized model to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertSet(Models.Listrom.Set? set, string filename, int indexId, bool statsOnly)
        {
            // If the set is missing, we can't do anything
            if (set == null)
                return;

            // Create the machine
            Machine machine;
            if (!string.IsNullOrWhiteSpace(set.Device))
            {
                machine = new Machine
                {
                    Name = set.Device,
                    MachineType = MachineType.Device,
                };
            }
            else if (!string.IsNullOrWhiteSpace(set.Driver))
            {
                machine = new Machine
                {
                    Name = set.Driver,
                    MachineType = MachineType.None,
                };
            }
            else
            {
                return;
            }

            foreach (var row in set.Row ?? Array.Empty<Models.Listrom.Row>())
            {
                ConvertRow(row, machine, filename, indexId, statsOnly);
            }
        }

        /// <summary>
        /// Convert row information
        /// </summary>
        /// <param name="row">Deserialized model to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertRow(Models.Listrom.Row? row, Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the row is missing, we can't do anything
            if (row == null)
                return;

            // Normal CHD
            if (row.Size == null
                && !row.NoGoodDumpKnown
                && !row.Bad
                && (!string.IsNullOrWhiteSpace(row.MD5)
                    || !string.IsNullOrWhiteSpace(row.SHA1)))
            {
                var disk = new Disk
                {
                    Name = row.Name,
                    ItemStatus = ItemStatus.None,

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

                if (!string.IsNullOrWhiteSpace(row.MD5))
                    disk.MD5 = row.MD5;
                else
                    disk.SHA1 = row.SHA1;

                // Now process and add the item
                disk.CopyMachineInformation(machine);
                ParseAddHelper(disk, statsOnly);
            }

            // Normal ROM
            else if (row.Size != null
                && !row.NoGoodDumpKnown
                && !row.Bad)
            {
                var rom = new Rom
                {
                    Name = row.Name,
                    Size = NumberHelper.ConvertToInt64(row.Size),
                    CRC = row.CRC,
                    SHA1 = row.SHA1,
                    ItemStatus = ItemStatus.None,

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

                // Now process and add the item
                rom.CopyMachineInformation(machine);
                ParseAddHelper(rom, statsOnly);
            }

            // Bad CHD
            else if (row.Size == null
                && !row.NoGoodDumpKnown
                && row.Bad
                && (!string.IsNullOrWhiteSpace(row.MD5)
                    || !string.IsNullOrWhiteSpace(row.SHA1)))
            {
                var disk = new Disk
                {
                    Name = row.Name,
                    ItemStatus = ItemStatus.BadDump,

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

                if (!string.IsNullOrWhiteSpace(row.MD5))
                    disk.MD5 = row.MD5;
                else
                    disk.SHA1 = row.SHA1;

                // Now process and add the item
                disk.CopyMachineInformation(machine);
                ParseAddHelper(disk, statsOnly);
            }

            // Nodump CHD
            else if (row.Size == null
                && row.NoGoodDumpKnown)
            {
                var disk = new Disk
                {
                    Name = row.Name,
                    MD5 = null,
                    SHA1 = null,
                    ItemStatus = ItemStatus.Nodump,

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

                // Now process and add the item
                disk.CopyMachineInformation(machine);
                ParseAddHelper(disk, statsOnly);
            }

            // Bad ROM
            else if (row.Size != null
                && !row.NoGoodDumpKnown
                && row.Bad)
            {
                var rom = new Rom
                {
                    Name = row.Name,
                    Size = NumberHelper.ConvertToInt64(row.Size),
                    CRC = row.CRC,
                    SHA1 = row.SHA1,
                    ItemStatus = ItemStatus.BadDump,

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

                // Now process and add the item
                rom.CopyMachineInformation(machine);
                ParseAddHelper(rom, statsOnly);
            }

            // Nodump ROM
            else if (row.Size != null
                && row.NoGoodDumpKnown)
            {
                var rom = new Rom
                {
                    Name = row.Name,
                    Size = NumberHelper.ConvertToInt64(row.Size),
                    CRC = null,
                    SHA1 = null,
                    ItemStatus = ItemStatus.Nodump,

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

                // Now process and add the item
                rom.CopyMachineInformation(machine);
                ParseAddHelper(rom, statsOnly);
            }
        }

        #endregion
    }
}
