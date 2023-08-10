using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SabreTools.IO.Readers;
using SabreTools.Models.AttractMode;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Separated value deserializer for AttractMode romlists
    /// </summary>
    public partial class AttractMode
    {
        /// <summary>
        /// Deserializes an AttractMode romlist to the defined type
        /// </summary>
        /// <param name="path">Path to the file to deserialize</param>
        /// <returns>Deserialized data on success, null on failure</returns>
        public static MetadataFile? Deserialize(string path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Deserialize(stream);
        }

        /// <summary>
        /// Deserializes an AttractMode romlist in a stream to the defined type
        /// </summary>
        /// <param name="stream">Stream to deserialize</param>
        /// <returns>Deserialized data on success, null on failure</returns>
        public static MetadataFile? Deserialize(Stream? stream)
        {
            // If the stream is null
            if (stream == null)
                return default;

            // Setup the reader and output
            var reader = new SeparatedValueReader(stream, Encoding.UTF8)
            {
                Separator = ';',
                VerifyFieldCount = false,
            };
            var dat = new MetadataFile();

            // Read the header values first
            if (!reader.ReadHeader())
                return null;

            dat.Header = reader.HeaderValues.ToArray();

            // Loop through the rows and parse out values
            var rows = new List<Row>();
            while (!reader.EndOfStream)
            {
                // If we have no next line
                if (!reader.ReadNextLine())
                    break;

                // Parse the line into a row
                Row? row = null;
                if (reader.Line.Count < HeaderWithRomnameCount)
                {
                    row = new Row
                    {
                        Name = reader.Line[0],
                        Title = reader.Line[1],
                        Emulator = reader.Line[2],
                        CloneOf = reader.Line[3],
                        Year = reader.Line[4],
                        Manufacturer = reader.Line[5],
                        Category = reader.Line[6],
                        Players = reader.Line[7],
                        Rotation = reader.Line[8],
                        Control = reader.Line[9],
                        Status = reader.Line[10],
                        DisplayCount = reader.Line[11],
                        DisplayType = reader.Line[12],
                        AltRomname = reader.Line[13],
                        AltTitle = reader.Line[14],
                        Extra = reader.Line[15],
                        Buttons = reader.Line[16],
                    };

                    // If we have additional fields
                    if (reader.Line.Count > HeaderWithoutRomnameCount)
                        row.ADDITIONAL_ELEMENTS = reader.Line.Skip(HeaderWithoutRomnameCount).ToArray();
                }
                else
                {
                    row = new Row
                    {
                        Name = reader.Line[0],
                        Title = reader.Line[1],
                        Emulator = reader.Line[2],
                        CloneOf = reader.Line[3],
                        Year = reader.Line[4],
                        Manufacturer = reader.Line[5],
                        Category = reader.Line[6],
                        Players = reader.Line[7],
                        Rotation = reader.Line[8],
                        Control = reader.Line[9],
                        Status = reader.Line[10],
                        DisplayCount = reader.Line[11],
                        DisplayType = reader.Line[12],
                        AltRomname = reader.Line[13],
                        AltTitle = reader.Line[14],
                        Extra = reader.Line[15],
                        Buttons = reader.Line[16],
                    };

                    // If we have additional fields
                    if (reader.Line.Count > HeaderWithRomnameCount)
                        row.ADDITIONAL_ELEMENTS = reader.Line.Skip(HeaderWithRomnameCount).ToArray();
                }

                rows.Add(row);
            }

            // Assign the rows to the Dat and return
            dat.Row = rows.ToArray();
            return dat;
        }

        #region Internal

        /// <summary>
        /// Convert from <cref="Models.Internal.MetadataFile"/> to <cref="Models.AttractMode.MetadataFile"/>
        /// </summary>
        public static MetadataFile? ConvertFromInternalModel(Models.Internal.MetadataFile? item)
        {
            if (item == null)
                return null;

            var header = item.Read<Models.Internal.Header>(Models.Internal.MetadataFile.HeaderKey);
            var metadataFile = header != null ? ConvertHeaderFromInternalModel(header) : new MetadataFile();

            var machines = item.Read<Models.Internal.Machine[]>(Models.Internal.MetadataFile.MachineKey);
            if (machines != null && machines.Any())
                metadataFile.Row = machines.SelectMany(ConvertMachineFromInternalModel).ToArray();
            
            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Header"/> to <cref="Models.AttractMode.MetadataFile"/>
        /// </summary>
        private static MetadataFile? ConvertHeaderFromInternalModel(Models.Internal.Header? item)
        {
            if (item == null)
                return null;

            var metadataFile = new MetadataFile
            {
                Header = item.ReadStringArray(Models.Internal.Header.HeaderKey),
            };
            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Machine"/> to an array of <cref="Models.AttractMode.Row"/>
        /// </summary>
        private static Row?[]? ConvertMachineFromInternalModel(Models.Internal.Machine? item)
        {
            if (item == null)
                return null;

            var roms = item.Read<Models.Internal.Rom[]>(Models.Internal.Machine.RomKey);
            return roms?.Select(rom =>
            {
                if (rom == null)
                    return null;

                var rowItem = ConvertFromInternalModel(rom);

                rowItem.Name = item.ReadString(Models.Internal.Machine.NameKey);
                rowItem.Emulator = item.ReadString(Models.Internal.Machine.EmulatorKey);
                rowItem.CloneOf = item.ReadString(Models.Internal.Machine.CloneOfKey);
                rowItem.Year = item.ReadString(Models.Internal.Machine.YearKey);
                rowItem.Manufacturer = item.ReadString(Models.Internal.Machine.ManufacturerKey);
                rowItem.Category = item.ReadString(Models.Internal.Machine.CategoryKey);
                rowItem.Players = item.ReadString(Models.Internal.Machine.PlayersKey);
                rowItem.Rotation = item.ReadString(Models.Internal.Machine.RotationKey);
                rowItem.Control = item.ReadString(Models.Internal.Machine.ControlKey);
                rowItem.Status = item.ReadString(Models.Internal.Machine.StatusKey);
                rowItem.DisplayCount = item.ReadString(Models.Internal.Machine.DisplayCountKey);
                rowItem.DisplayType = item.ReadString(Models.Internal.Machine.DisplayTypeKey);
                rowItem.Extra = item.ReadString(Models.Internal.Machine.ExtraKey);
                rowItem.Buttons = item.ReadString(Models.Internal.Machine.ButtonsKey);
                rowItem.Favorite = item.ReadString(Models.Internal.Machine.FavoriteKey);
                rowItem.Tags = item.ReadString(Models.Internal.Machine.TagsKey);
                rowItem.PlayedCount = item.ReadString(Models.Internal.Machine.PlayedCountKey);
                rowItem.PlayedTime = item.ReadString(Models.Internal.Machine.PlayedTimeKey);

                return rowItem;
            })?.ToArray();
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.AttractMode.Row"/>
        /// </summary>
        private static Row? ConvertFromInternalModel(Models.Internal.Rom? item)
        {
            if (item == null)
                return null;

            var row = new Row
            {
                Title = item.ReadString(Models.Internal.Rom.NameKey),
                AltRomname = item.ReadString(Models.Internal.Rom.AltRomnameKey),
                AltTitle = item.ReadString(Models.Internal.Rom.AltTitleKey),
                FileIsAvailable = item.ReadString(Models.Internal.Rom.FileIsAvailableKey),
            };
            return row;
        }

        #endregion
    }
}