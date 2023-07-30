using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SabreTools.IO.Readers;
using SabreTools.Models.EverdriveSMDB;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Separated value deserializer for Everdrive SMDBs
    /// </summary>
    public partial class EverdriveSMDB
    {
        /// <summary>
        /// Deserializes an Everdrive SMDB to the defined type
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
        /// Deserializes an Everdrive SMDB in a stream to the defined type
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
                    Header = false,
                    Separator = '\t',
                    VerifyFieldCount = false,
                };
                var dat = new MetadataFile();

                // Loop through the rows and parse out values
                var rows = new List<Row>();
                while (!reader.EndOfStream)
                {
                    // If we have no next line
                    if (!reader.ReadNextLine())
                        break;

                    // Parse the line into a row
                    var row = new Row
                    {
                        SHA256 = reader.Line[0],
                        Name = reader.Line[1],
                        SHA1 = reader.Line[2],
                        MD5 = reader.Line[3],
                        CRC32 = reader.Line[4],
                    };

                    // If we have the size field
                    if (reader.Line.Count > 5)
                        row.Size = reader.Line[5];

                    // If we have additional fields
                    if (reader.Line.Count > 6)
                        row.ADDITIONAL_ELEMENTS = reader.Line.Skip(5).ToArray();

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