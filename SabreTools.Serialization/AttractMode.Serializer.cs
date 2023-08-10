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
        /// Convert from <cref="Models.AttractMode.MetadataFile"/> to <cref="Models.Internal.MetadataFile"/>
        /// </summary>
        public static Models.Internal.MetadataFile? ConvertToInternalModel(MetadataFile? item)
        {
            if (item == null)
                return null;
            
            var metadataFile = new Models.Internal.MetadataFile
            {
                [Models.Internal.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item),
            };

            if (item?.Row != null && item.Row.Any())
            {
                metadataFile[Models.Internal.MetadataFile.MachineKey] = item.Row
                    .Where(r => r != null)
                    .Select(ConvertMachineToInternalModel)
                    .ToArray();
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.AttractMode.MetadataFile"/> to <cref="Models.Internal.Header"/>
        /// </summary>
        private static Models.Internal.Header ConvertHeaderToInternalModel(MetadataFile item)
        {
            var header = new Models.Internal.Header
            {
                [Models.Internal.Header.HeaderKey] = item.Header,
            };
            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.AttractMode.Row"/> to <cref="Models.Internal.Machine"/>
        /// </summary>
        private static Models.Internal.Machine ConvertMachineToInternalModel(Row item)
        {
            var machine = new Models.Internal.Machine
            {
                [Models.Internal.Machine.NameKey] = item.Name,
                [Models.Internal.Machine.EmulatorKey] = item.Emulator,
                [Models.Internal.Machine.CloneOfKey] = item.CloneOf,
                [Models.Internal.Machine.YearKey] = item.Year,
                [Models.Internal.Machine.ManufacturerKey] = item.Manufacturer,
                [Models.Internal.Machine.CategoryKey] = item.Category,
                [Models.Internal.Machine.PlayersKey] = item.Players,
                [Models.Internal.Machine.RotationKey] = item.Rotation,
                [Models.Internal.Machine.ControlKey] = item.Control,
                [Models.Internal.Machine.StatusKey] = item.Status,
                [Models.Internal.Machine.DisplayCountKey] = item.DisplayCount,
                [Models.Internal.Machine.DisplayTypeKey] = item.DisplayType,
                [Models.Internal.Machine.ExtraKey] = item.Extra,
                [Models.Internal.Machine.ButtonsKey] = item.Buttons,
                [Models.Internal.Machine.FavoriteKey] = item.Favorite,
                [Models.Internal.Machine.TagsKey] = item.Tags,
                [Models.Internal.Machine.PlayedCountKey] = item.PlayedCount,
                [Models.Internal.Machine.PlayedTimeKey] = item.PlayedTime,
                [Models.Internal.Machine.PlayedTimeKey] = item.PlayedTime,
                [Models.Internal.Machine.RomKey] = ConvertToInternalModel(item),
            };
            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.AttractMode.Row"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        private static Models.Internal.Rom ConvertToInternalModel(Row item)
        {
            var rom = new Models.Internal.Rom
            {
                [Models.Internal.Rom.NameKey] = item.Title,
                [Models.Internal.Rom.AltRomnameKey] = item.AltRomname,
                [Models.Internal.Rom.AltTitleKey] = item.AltTitle,
                [Models.Internal.Rom.FileIsAvailableKey] = item.FileIsAvailable,
            };
            return rom;
        }

        #endregion
    }
}