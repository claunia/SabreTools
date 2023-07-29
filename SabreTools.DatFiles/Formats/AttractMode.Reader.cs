using System;
using System.Linq;
using SabreTools.Core;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing of an AttractMode DAT
    /// </summary>
    internal partial class AttractMode : DatFile
    {
        /// <inheritdoc/>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            try
            {
                // Deserialize the input file
                var metadataFile = Serialization.AttractMode.Deserialize(filename);

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
        private void ConvertRows(Models.AttractMode.Row[]? rows, string filename, int indexId, bool statsOnly)
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
        private void ConvertRow(Models.AttractMode.Row? row, string filename, int indexId, bool statsOnly)
        {
            // If the row is missing, we can't do anything
            if (row == null)
                return;

            var rom = new Rom()
            {
                Name = "-",
                Size = Constants.SizeZero,
                CRC = Constants.CRCZero,
                MD5 = Constants.MD5Zero,
                SHA1 = Constants.SHA1Zero,
                ItemStatus = ItemStatus.None,

                Machine = new Machine
                {
                    Name = row.Name,
                    Description = row.Title,
                    CloneOf = row.CloneOf,
                    Year = row.Year,
                    Manufacturer = row.Manufacturer,
                    Category = row.Category,
                    Players = row.Players,
                    Rotation = row.Rotation,
                    Control = row.Control,
                    Status = row.Status,
                    DisplayCount = row.DisplayCount,
                    DisplayType = row.DisplayType,
                    Comment = row.Extra,
                    Buttons = row.Buttons
                },

                AltName = row.AltRomname,
                AltTitle = row.AltTitle,
                // TODO: Add extended fields

                Source = new Source
                {
                    Index = indexId,
                    Name = filename,
                },
            };

            // Now process and add the rom
            ParseAddHelper(rom, statsOnly);
        }

        #endregion
    }
}
