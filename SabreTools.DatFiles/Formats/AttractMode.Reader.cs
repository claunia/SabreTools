using System;
using System.Linq;
using SabreTools.Core;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.Hashing;

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
                var metadataFile = new Serialization.Files.AttractMode().Deserialize(filename);

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
        private void ConvertRows(Models.AttractMode.Row?[]? rows, string filename, int indexId, bool statsOnly)
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

            var machine = new Machine();
            machine.SetFieldValue<string?>(Models.Metadata.Machine.ButtonsKey, row.Buttons);
            machine.SetFieldValue<string?>(Models.Metadata.Machine.CategoryKey, row.Category);
            machine.SetFieldValue<string?>(Models.Metadata.Machine.CloneOfKey, row.CloneOf);
            machine.SetFieldValue<string?>(Models.Metadata.Machine.CommentKey, row.Extra);
            machine.SetFieldValue<string?>(Models.Metadata.Machine.ControlKey, row.Control);
            machine.SetFieldValue<string?>(Models.Metadata.Machine.DescriptionKey, row.Title);
            machine.SetFieldValue<string?>(Models.Metadata.Machine.DisplayCountKey, row.DisplayCount);
            machine.SetFieldValue<string?>(Models.Metadata.Machine.DisplayTypeKey, row.DisplayType);
            machine.SetFieldValue<string?>(Models.Metadata.Machine.ManufacturerKey, row.Manufacturer);
            machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, row.Name);
            machine.SetFieldValue<string?>(Models.Metadata.Machine.PlayersKey, row.Players);
            machine.SetFieldValue<string?>(Models.Metadata.Machine.RotationKey, row.Rotation);
            machine.SetFieldValue<string?>(Models.Metadata.Machine.StatusKey, row.Status);
            machine.SetFieldValue<string?>(Models.Metadata.Machine.YearKey, row.Year);

            var rom = new Rom()
            {
                Source = new Source { Index = indexId, Name = filename },
            };
            rom.SetName("-");
            rom.SetFieldValue<string?>(Models.Metadata.Rom.AltRomnameKey, row.AltRomname);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.AltTitleKey, row.AltTitle);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, Constants.CRCZero);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.MD5Key, Constants.MD5Zero);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, Constants.SHA1Zero);
            rom.SetFieldValue<long?>(Models.Metadata.Rom.SizeKey, Constants.SizeZero);
            rom.SetFieldValue<ItemStatus?>(Models.Metadata.Rom.StatusKey, ItemStatus.None);

            // Now process and add the rom
            rom.CopyMachineInformation(machine);
            ParseAddHelper(rom, statsOnly);
        }

        #endregion
    }
}
