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
            var machine = new Machine();
            machine.SetFieldValue<string?>(Models.Metadata.Machine.DescriptionKey, row.GameDescription);
            machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, row.GameName);

            // Read item values
            DatItem? item = null;
            switch (row.Type.AsEnumValue<ItemType>())
            {
                case ItemType.Disk:
                    item = new Disk
                    {
                        Source = new Source { Index = indexId, Name = filename },
                    };
                    item.SetName(row.DiskName);
                    item.SetFieldValue<ItemStatus>(Models.Metadata.Disk.StatusKey, row.Status?.AsEnumValue<ItemStatus>() ?? ItemStatus.NULL);
                    item.SetFieldValue<string?>(Models.Metadata.Disk.MD5Key, row.MD5);
                    item.SetFieldValue<string?>(Models.Metadata.Disk.SHA1Key, row.SHA1);
                    break;

                case ItemType.Media:
                    item = new Media
                    {
                        Source = new Source { Index = indexId, Name = filename },
                    };
                    item.SetName(row.DiskName);
                    item.SetFieldValue<string?>(Models.Metadata.Media.MD5Key, row.MD5);
                    item.SetFieldValue<string?>(Models.Metadata.Media.SHA1Key, row.SHA1);
                    item.SetFieldValue<string?>(Models.Metadata.Media.SHA256Key, row.SHA256);
                    item.SetFieldValue<string?>(Models.Metadata.Media.SpamSumKey, row.SpamSum);
                    break;

                case ItemType.Rom:
                    item = new Rom
                    {
                        Source = new Source { Index = indexId, Name = filename },
                    };
                    item.SetName(row.RomName);
                    item.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, row.CRC);
                    item.SetFieldValue<string?>(Models.Metadata.Rom.MD5Key, row.MD5);
                    item.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, row.SHA1);
                    item.SetFieldValue<string?>(Models.Metadata.Rom.SHA256Key, row.SHA256);
                    item.SetFieldValue<string?>(Models.Metadata.Rom.SHA384Key, row.SHA384);
                    item.SetFieldValue<string?>(Models.Metadata.Rom.SHA512Key, row.SHA512);
                    item.SetFieldValue<string?>(Models.Metadata.Rom.SpamSumKey, row.SpamSum);
                    item.SetFieldValue<ItemStatus>(Models.Metadata.Rom.StatusKey, row.Status.AsEnumValue<ItemStatus>());
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
