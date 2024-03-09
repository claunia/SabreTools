using System;
using System.Linq;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing a value-separated DAT
    /// </summary>
    internal partial class SeparatedValue : DatFile
    {
        /// <inheritdoc/>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            try
            {
                // Deserialize the input file
                var metadataFile = new Serialization.Files.SeparatedValue().Deserialize(filename, _delim);

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
        /// Convert rows information
        /// </summary>
        /// <param name="rows">Array of deserialized models to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertRows(Models.SeparatedValue.Row[]? rows, string filename, int indexId, bool statsOnly)
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
        /// Convert rows information
        /// </summary>
        /// <param name="row">Deserialized model to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertRow(Models.SeparatedValue.Row? row, string filename, int indexId, bool statsOnly)
        {
            // If the row is missing, we can't do anything
            if (row == null)
                return;

            // Read DAT-level values
            //Header.FileName ??= row.FileName;
            Header.Name ??= row.InternalName;
            Header.Description ??= row.Description;

            // Read Machine values
            var machine = new Machine
            {
                Name = row.GameName,
                Description = row.GameDescription,
            };

            // Read item values
            DatItem? item = null;
            switch (row.Type.AsEnumValue<ItemType>())
            {
                case ItemType.Disk:
                    item = new Disk
                    {
                        MD5 = row.MD5,
                        SHA1 = row.SHA1,
                        ItemStatus = row.Status.AsEnumValue<ItemStatus>(),

                        Source = new Source { Index = indexId, Name = filename },
                    };
                    item.SetName(row.DiskName);
                    break;

                case ItemType.Media:
                    item = new Media
                    {
                        MD5 = row.MD5,
                        SHA1 = row.SHA1,
                        SHA256 = row.SHA256,
                        SpamSum = row.SpamSum,

                        Source = new Source { Index = indexId, Name = filename },
                    };
                    item.SetName(row.DiskName);
                    break;

                case ItemType.Rom:
                    item = new Rom
                    {
                        CRC = row.CRC,
                        MD5 = row.MD5,
                        SHA1 = row.SHA1,
                        SHA256 = row.SHA256,
                        SHA384 = row.SHA384,
                        SHA512 = row.SHA512,
                        SpamSum = row.SpamSum,
                        ItemStatus = row.Status.AsEnumValue<ItemStatus>(),

                        Source = new Source { Index = indexId, Name = filename },
                    };
                    item.SetName(row.RomName);
                    break;
            }

            // Now process and add the item
            if (item != null)
            {
                item.CopyMachineInformation(machine);
                ParseAddHelper(item, statsOnly);
            }
        }

        #endregion
    }
}
