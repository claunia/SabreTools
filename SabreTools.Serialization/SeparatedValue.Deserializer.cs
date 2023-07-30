using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SabreTools.IO.Readers;
using SabreTools.Models.SeparatedValue;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Deserializer for separated-value variants
    /// </summary>
    public partial class SeparatedValue
    {
        /// <summary>
        /// Deserializes a separated-value variant to the defined type
        /// </summary>
        /// <param name="path">Path to the file to deserialize</param>
        /// <param name="delim">Character delimiter between values</param>
        /// <returns>Deserialized data on success, null on failure</returns>
        public static MetadataFile? Deserialize(string path, char delim)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Deserialize(stream, delim);
        }

        /// <summary>
        /// Deserializes a separated-value variant in a stream to the defined type
        /// </summary>
        /// <param name="stream">Stream to deserialize</param>
        /// <param name="delim">Character delimiter between values</param>
        /// <returns>Deserialized data on success, null on failure</returns>
        public static MetadataFile? Deserialize(Stream? stream, char delim)
        {
            // If the stream is null
            if (stream == null)
                return default;

            // Setup the reader and output
            var reader = new SeparatedValueReader(stream, Encoding.UTF8)
            {
                Header = true,
                Separator = delim,
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
                if (reader.Line.Count < HeaderWithExtendedHashesCount)
                {
                    row = new Row
                    {
                        FileName = reader.Line[0],
                        InternalName = reader.Line[1],
                        Description = reader.Line[2],
                        GameName = reader.Line[3],
                        GameDescription = reader.Line[4],
                        Type = reader.Line[5],
                        RomName = reader.Line[6],
                        DiskName = reader.Line[7],
                        Size = reader.Line[8],
                        CRC = reader.Line[9],
                        MD5 = reader.Line[10],
                        SHA1 = reader.Line[11],
                        SHA256 = reader.Line[12],
                        Status = reader.Line[13],
                    };

                    // If we have additional fields
                    if (reader.Line.Count > HeaderWithoutExtendedHashesCount)
                        row.ADDITIONAL_ELEMENTS = reader.Line.Skip(HeaderWithoutExtendedHashesCount).ToArray();
                }
                else
                {
                    row = new Row
                    {
                        FileName = reader.Line[0],
                        InternalName = reader.Line[1],
                        Description = reader.Line[2],
                        GameName = reader.Line[3],
                        GameDescription = reader.Line[4],
                        Type = reader.Line[5],
                        RomName = reader.Line[6],
                        DiskName = reader.Line[7],
                        Size = reader.Line[8],
                        CRC = reader.Line[9],
                        MD5 = reader.Line[10],
                        SHA1 = reader.Line[11],
                        SHA256 = reader.Line[12],
                        SHA384 = reader.Line[13],
                        SHA512 = reader.Line[14],
                        SpamSum = reader.Line[15],
                        Status = reader.Line[16],
                    };

                    // If we have additional fields
                    if (reader.Line.Count > HeaderWithExtendedHashesCount)
                        row.ADDITIONAL_ELEMENTS = reader.Line.Skip(HeaderWithExtendedHashesCount).ToArray();
                }
                rows.Add(row);
            }

            // Assign the rows to the Dat and return
            dat.Row = rows.ToArray();
            return dat;
        }
    }
}