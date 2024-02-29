using System;
using System.IO;
using System.Linq;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing and writing of an Everdrive SMDB file
    /// </summary>
    internal partial class EverdriveSMDB : DatFile
    {
        /// <inheritdoc/>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            try
            {
                // Deserialize the input file
                var metadataFile = new Serialization.Files.EverdriveSMDB().Deserialize(filename);

                // Convert the row data to the internal format
                ConvertRows(metadataFile?.Row, filename, indexId, statsOnly);
            }
            catch (Exception ex) when (!throwOnError)
            {
                string message = $"'{filename}' - An error occurred during parsing";
                logger.Error(ex, message);
            }
        }

        #region Converters

        /// <summary>
        /// Create a machine from the filename
        /// </summary>
        /// <param name="filename">Filename to derive from</param>
        /// <returns>Filled machine and new filename on success, null on error</returns>
        private static (Machine?, string?) DeriveMachine(string? filename)
        {
            // If the filename is missing, we can't do anything
            if (string.IsNullOrEmpty(filename))
                return (null, null);

            string machineName = Path.GetFileNameWithoutExtension(filename);
            if (filename.Contains('/'))
            {
                string[] split = filename!.Split('/');
                machineName = split[0];
                filename = filename.Substring(machineName.Length + 1);
            }
            else if (filename.Contains('\\'))
            {
                string[] split = filename!.Split('\\');
                machineName = split[0];
                filename = filename.Substring(machineName.Length + 1);
            }

            var machine = new Machine { Name = machineName };
            return (machine, filename);
        }

        /// <summary>
        /// Convert rows information
        /// </summary>
        /// <param name="rows">Array of deserialized models to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertRows(Models.EverdriveSMDB.Row[]? rows, string filename, int indexId, bool statsOnly)
        {
            // If the rows array is missing, we can't do anything
            if (rows == null || !rows.Any())
                return;

            // Loop through the rows and add
            foreach (var row in rows)
            {
                ConvertRow(row, filename, indexId, statsOnly);
            }
        }

        /// <summary>
        /// Convert row information
        /// </summary>
        /// <param name="row">Deserialized model to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertRow(Models.EverdriveSMDB.Row? row, string filename, int indexId, bool statsOnly)
        {
            // If the row is missing, we can't do anything
            if (row == null)
                return;

            (var machine, string? name) = DeriveMachine(row.Name);
            machine ??= new Machine { Name = Path.GetFileNameWithoutExtension(row.Name) };

            var rom = new Rom()
            {
                Name = name,
                Size = NumberHelper.ConvertToInt64(row.Size),
                CRC = row.CRC32,
                MD5 = row.MD5,
                SHA1 = row.SHA1,
                SHA256 = row.SHA256,
                ItemStatus = ItemStatus.None,

                Source = new Source
                {
                    Index = indexId,
                    Name = filename,
                },
            };

            // Now process and add the rom
            rom.CopyMachineInformation(machine);
            ParseAddHelper(rom, statsOnly);
        }

        #endregion
    }
}
