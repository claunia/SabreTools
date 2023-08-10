using System;
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
            {
                metadataFile.Row = machines
                    .Where(m => m != null)
                    .SelectMany(ConvertMachineFromInternalModel)
                    .ToArray();
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Header"/> to <cref="Models.AttractMode.MetadataFile"/>
        /// </summary>
        private static MetadataFile ConvertHeaderFromInternalModel(Models.Internal.Header item)
        {
            var metadataFile = new MetadataFile
            {
                Header = item.ReadStringArray(Models.Internal.Header.HeaderKey),
            };
            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Machine"/> to an array of <cref="Models.AttractMode.Row"/>
        /// </summary>
        private static Row[] ConvertMachineFromInternalModel(Models.Internal.Machine item)
        {
            var roms = item.Read<Models.Internal.Rom[]>(Models.Internal.Machine.RomKey);
            if (roms == null || !roms.Any())
                return Array.Empty<Row>();

            return roms
                .Where(r => r != null)
                .Select(rom => ConvertFromInternalModel(rom, item))
                .ToArray();
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.AttractMode.Row"/>
        /// </summary>
        private static Row ConvertFromInternalModel(Models.Internal.Rom item, Models.Internal.Machine parent)
        {
            var row = new Row
            {
                Name = parent.ReadString(Models.Internal.Machine.NameKey),
                Title = item.ReadString(Models.Internal.Rom.NameKey),
                Emulator = parent.ReadString(Models.Internal.Machine.EmulatorKey),
                CloneOf = parent.ReadString(Models.Internal.Machine.CloneOfKey),
                Year = parent.ReadString(Models.Internal.Machine.YearKey),
                Manufacturer = parent.ReadString(Models.Internal.Machine.ManufacturerKey),
                Category = parent.ReadString(Models.Internal.Machine.CategoryKey),
                Players = parent.ReadString(Models.Internal.Machine.PlayersKey),
                Rotation = parent.ReadString(Models.Internal.Machine.RotationKey),
                Control = parent.ReadString(Models.Internal.Machine.ControlKey),
                Status = parent.ReadString(Models.Internal.Machine.StatusKey),
                DisplayCount = parent.ReadString(Models.Internal.Machine.DisplayCountKey),
                DisplayType = parent.ReadString(Models.Internal.Machine.DisplayTypeKey),
                AltRomname = item.ReadString(Models.Internal.Rom.AltRomnameKey),
                AltTitle = item.ReadString(Models.Internal.Rom.AltTitleKey),
                Extra = parent.ReadString(Models.Internal.Machine.ExtraKey),
                Buttons = parent.ReadString(Models.Internal.Machine.ButtonsKey),
                Favorite = parent.ReadString(Models.Internal.Machine.FavoriteKey),
                Tags = parent.ReadString(Models.Internal.Machine.TagsKey),
                PlayedCount = parent.ReadString(Models.Internal.Machine.PlayedCountKey),
                PlayedTime = parent.ReadString(Models.Internal.Machine.PlayedTimeKey),
                FileIsAvailable = item.ReadString(Models.Internal.Rom.FileIsAvailableKey),
            };
            return row;
        }

        #endregion
    }
}