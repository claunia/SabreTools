using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SabreTools.IO.Readers;
using SabreTools.Models.AttractMode;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Separated value serializer for AttractMode romlists
    /// </summary>
    public class AttractMode
    {
        private const string HeaderWithoutRomname = "#Name;Title;Emulator;CloneOf;Year;Manufacturer;Category;Players;Rotation;Control;Status;DisplayCount;DisplayType;AltRomname;AltTitle;Extra;Buttons";
        private const int HeaderWithoutRomnameCount = 17;

        private const string HeaderWithRomname = "#Romname;Title;Emulator;Cloneof;Year;Manufacturer;Category;Players;Rotation;Control;Status;DisplayCount;DisplayType;AltRomname;AltTitle;Extra;Buttons;Favourite;Tags;PlayedCount;PlayedTime;FileIsAvailable";
        private const int HeaderWithRomnameCount = 22;

        /// <summary>
        /// Deserializes an AttractMode romlist to the defined type
        /// </summary>
        /// <param name="path">Path to the file to deserialize</param>
        /// <returns>Deserialized data on success, null on failure</returns>
        public static MetadataFile? Deserialize(string path)
        {
            try
            {
                using var stream = PathProcessor.OpenStream(path);
                return Deserialize(stream);
            }
            catch
            {
                // TODO: Handle logging the exception
                return default;
            }
        }

        /// <summary>
        /// Deserializes an AttractMode romlist in a stream to the defined type
        /// </summary>
        /// <param name="stream">Stream to deserialize</param>
        /// <returns>Deserialized data on success, null on failure</returns>
        public static MetadataFile? Deserialize(Stream? stream)
        {
            try
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
                    Row row = null;
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
            catch
            {
                // TODO: Handle logging the exception
                return default;
            }
        }
    }
}