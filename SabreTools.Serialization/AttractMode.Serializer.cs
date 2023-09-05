using System.IO;
using System.Linq;
using System.Text;
using SabreTools.IO.Writers;
using SabreTools.Models.AttractMode;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Separated value serializer for AttractMode romlists
    /// </summary>
    public partial class AttractMode
    {
        /// <summary>
        /// Serializes the defined type to an AttractMode romlist
        /// </summary>
        /// <param name="metadataFile">Data to serialize</param>
        /// <param name="path">Path to the file to serialize to</param>
        /// <returns>True on successful serialization, false otherwise</returns>
        public static bool SerializeToFile(MetadataFile? metadataFile, string path)
        {
            using var stream = SerializeToStream(metadataFile);
            if (stream == null)
                return false;

            using var fs = File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }

        /// <summary>
        /// Serializes the defined type to a stream
        /// </summary>
        /// <param name="metadataFile">Data to serialize</param>
        /// <returns>Stream containing serialized data on success, null otherwise</returns>
        public static Stream? SerializeToStream(MetadataFile? metadataFile)
        {
            // If the metadata file is null
            if (metadataFile == null)
                return null;

            // Setup the writer and output
            var stream = new MemoryStream();
            var writer = new SeparatedValueWriter(stream, Encoding.UTF8)
            {
                Separator = ';',
                Quotes = false,
                VerifyFieldCount = false,
            };

            // TODO: Include flag to write out long or short header
            // Write the short header
            writer.WriteString(HeaderWithoutRomname); // TODO: Convert to array of values

            // Write out the rows, if they exist
            WriteRows(metadataFile.Row, writer);

            // Return the stream
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// Write rows information to the current writer
        /// </summary>
        /// <param name="rows">Array of Row objects representing the rows information</param>
        /// <param name="writer">SeparatedValueWriter representing the output</param>
        private static void WriteRows(Row[]? rows, SeparatedValueWriter writer)
        {
            // If the games information is missing, we can't do anything
            if (rows == null || !rows.Any())
                return;

            // Loop through and write out the rows
            foreach (var row in rows)
            {
                var rowArray = new string?[]
                {
                    row.Name,
                    row.Title,
                    row.Emulator,
                    row.CloneOf,
                    row.Year,
                    row.Manufacturer,
                    row.Category,
                    row.Players,
                    row.Rotation,
                    row.Control,
                    row.Status,
                    row.DisplayCount,
                    row.DisplayType,
                    row.AltRomname,
                    row.AltTitle,
                    row.Extra,
                    row.Buttons,
                };

                writer.WriteValues(rowArray);
                writer.Flush();
            }
        }

        #region Internal

        /// <summary>
        /// Convert from <cref="Models.AttractMode.MetadataFile"/> to <cref="Models.Metadata.MetadataFile"/>
        /// </summary>
        public static Models.Metadata.MetadataFile? ConvertToInternalModel(MetadataFile? item)
        {
            if (item == null)
                return null;
            
            var metadataFile = new Models.Metadata.MetadataFile
            {
                [Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item),
            };

            if (item?.Row != null && item.Row.Any())
            {
                metadataFile[Models.Metadata.MetadataFile.MachineKey] = item.Row
                    .Where(r => r != null)
                    .Select(ConvertMachineToInternalModel)
                    .ToArray();
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.AttractMode.MetadataFile"/> to <cref="Models.Metadata.Header"/>
        /// </summary>
        private static Models.Metadata.Header ConvertHeaderToInternalModel(MetadataFile item)
        {
            var header = new Models.Metadata.Header
            {
                [Models.Metadata.Header.HeaderKey] = item.Header,
            };
            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.AttractMode.Row"/> to <cref="Models.Metadata.Machine"/>
        /// </summary>
        private static Models.Metadata.Machine ConvertMachineToInternalModel(Row item)
        {
            var machine = new Models.Metadata.Machine
            {
                [Models.Metadata.Machine.NameKey] = item.Name,
                [Models.Metadata.Machine.EmulatorKey] = item.Emulator,
                [Models.Metadata.Machine.CloneOfKey] = item.CloneOf,
                [Models.Metadata.Machine.YearKey] = item.Year,
                [Models.Metadata.Machine.ManufacturerKey] = item.Manufacturer,
                [Models.Metadata.Machine.CategoryKey] = item.Category,
                [Models.Metadata.Machine.PlayersKey] = item.Players,
                [Models.Metadata.Machine.RotationKey] = item.Rotation,
                [Models.Metadata.Machine.ControlKey] = item.Control,
                [Models.Metadata.Machine.StatusKey] = item.Status,
                [Models.Metadata.Machine.DisplayCountKey] = item.DisplayCount,
                [Models.Metadata.Machine.DisplayTypeKey] = item.DisplayType,
                [Models.Metadata.Machine.ExtraKey] = item.Extra,
                [Models.Metadata.Machine.ButtonsKey] = item.Buttons,
                [Models.Metadata.Machine.FavoriteKey] = item.Favorite,
                [Models.Metadata.Machine.TagsKey] = item.Tags,
                [Models.Metadata.Machine.PlayedCountKey] = item.PlayedCount,
                [Models.Metadata.Machine.PlayedTimeKey] = item.PlayedTime,
                [Models.Metadata.Machine.PlayedTimeKey] = item.PlayedTime,
                [Models.Metadata.Machine.RomKey] = ConvertToInternalModel(item),
            };
            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.AttractMode.Row"/> to <cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Models.Metadata.Rom ConvertToInternalModel(Row item)
        {
            var rom = new Models.Metadata.Rom
            {
                [Models.Metadata.Rom.NameKey] = item.Title,
                [Models.Metadata.Rom.AltRomnameKey] = item.AltRomname,
                [Models.Metadata.Rom.AltTitleKey] = item.AltTitle,
                [Models.Metadata.Rom.FileIsAvailableKey] = item.FileIsAvailable,
            };
            return rom;
        }

        #endregion
    }
}