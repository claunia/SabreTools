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
            if (!reader.ReadHeader() || reader.HeaderValues == null)
                return null;

            dat.Header = reader.HeaderValues.ToArray();

            // Loop through the rows and parse out values
            var rows = new List<Row>();
            while (!reader.EndOfStream)
            {
                // If we have no next line
                if (!reader.ReadNextLine() || reader.Line == null)
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

        #region Internal

        /// <summary>
        /// Convert from <cref="Models.Internal.MetadataFile"/> to <cref="Models.SeparatedValue.MetadataFile"/>
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
        /// Convert from <cref="Models.Internal.Header"/> to <cref="Models.SeparatedValue.MetadataFile"/>
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
        /// Convert from <cref="Models.Internal.Machine"/> to an array of <cref="Models.SeparatedValue.Row"/>
        /// </summary>
        private static Row[] ConvertMachineFromInternalModel(Models.Internal.Machine item)
        {
            var rowItems = new List<Row>();

            var roms = item.Read<Models.Internal.Rom[]>(Models.Internal.Machine.RomKey);
            if (roms != null && roms.Any())
            {
                rowItems.AddRange(roms
                    .Where(r => r != null)
                    .Select(rom => ConvertFromInternalModel(rom, item)));
            }

            var disks = item.Read<Models.Internal.Disk[]>(Models.Internal.Machine.DiskKey);
            if (disks != null && disks.Any())
            {
                rowItems.AddRange(disks
                    .Where(d => d != null)
                    .Select(disk => ConvertFromInternalModel(disk, item)));
            }

            var media = item.Read<Models.Internal.Media[]>(Models.Internal.Machine.MediaKey);
            if (media != null && media.Any())
            {
                rowItems.AddRange(media
                    .Where(m => m != null)
                    .Select(medium => ConvertFromInternalModel(medium, item)));
            }

            return rowItems.ToArray();
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Disk"/> to <cref="Models.SeparatedValue.Row"/>
        /// </summary>
        private static Row ConvertFromInternalModel(Models.Internal.Disk item, Models.Internal.Machine parent)
        {
            var row = new Row
            {
                GameName = parent.ReadString(Models.Internal.Machine.NameKey),
                Description = parent.ReadString(Models.Internal.Machine.DescriptionKey),
                Type = "disk",
                DiskName = item.ReadString(Models.Internal.Disk.NameKey),
                MD5 = item.ReadString(Models.Internal.Disk.MD5Key),
                SHA1 = item.ReadString(Models.Internal.Disk.SHA1Key),
                Status = item.ReadString(Models.Internal.Disk.StatusKey),
            };
            return row;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Media"/> to <cref="Models.SeparatedValue.Row"/>
        /// </summary>
        private static Row ConvertFromInternalModel(Models.Internal.Media item, Models.Internal.Machine parent)
        {
            var row = new Row
            {
                GameName = parent.ReadString(Models.Internal.Machine.NameKey),
                Description = parent.ReadString(Models.Internal.Machine.DescriptionKey),
                Type = "media",
                DiskName = item.ReadString(Models.Internal.Media.NameKey),
                MD5 = item.ReadString(Models.Internal.Media.MD5Key),
                SHA1 = item.ReadString(Models.Internal.Media.SHA1Key),
                SHA256 = item.ReadString(Models.Internal.Media.SHA256Key),
                SpamSum = item.ReadString(Models.Internal.Media.SpamSumKey),
            };
            return row;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.SeparatedValue.Row"/>
        /// </summary>
        private static Row ConvertFromInternalModel(Models.Internal.Rom item, Models.Internal.Machine parent)
        {
            var row = new Row
            {
                GameName = parent?.ReadString(Models.Internal.Machine.NameKey),
                Description = parent?.ReadString(Models.Internal.Machine.DescriptionKey),
                Type = "rom",
                RomName = item.ReadString(Models.Internal.Rom.NameKey),
                Size = item.ReadString(Models.Internal.Rom.SizeKey),
                CRC = item.ReadString(Models.Internal.Rom.CRCKey),
                MD5 = item.ReadString(Models.Internal.Rom.MD5Key),
                SHA1 = item.ReadString(Models.Internal.Rom.SHA1Key),
                SHA256 = item.ReadString(Models.Internal.Rom.SHA256Key),
                SHA384 = item.ReadString(Models.Internal.Rom.SHA384Key),
                SHA512 = item.ReadString(Models.Internal.Rom.SHA512Key),
                SpamSum = item.ReadString(Models.Internal.Rom.SpamSumKey),
                Status = item.ReadString(Models.Internal.Rom.StatusKey),
            };
            return row;
        }

        #endregion
    }
}