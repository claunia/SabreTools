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
        /// Convert from <cref="Models.Metadata.MetadataFile"/> to <cref="Models.SeparatedValue.MetadataFile"/>
        /// </summary>
        public static MetadataFile? ConvertFromInternalModel(Models.Metadata.MetadataFile? item)
        {
            if (item == null)
                return null;

            var header = item.Read<Models.Metadata.Header>(Models.Metadata.MetadataFile.HeaderKey);
            var metadataFile = header != null ? ConvertHeaderFromInternalModel(header) : new MetadataFile();

            var machines = item.Read<Models.Metadata.Machine[]>(Models.Metadata.MetadataFile.MachineKey);
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
        /// Convert from <cref="Models.Metadata.Header"/> to <cref="Models.SeparatedValue.MetadataFile"/>
        /// </summary>
        private static MetadataFile ConvertHeaderFromInternalModel(Models.Metadata.Header item)
        {
            var metadataFile = new MetadataFile
            {
                Header = item.ReadStringArray(Models.Metadata.Header.HeaderKey),
            };
            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Machine"/> to an array of <cref="Models.SeparatedValue.Row"/>
        /// </summary>
        private static Row[] ConvertMachineFromInternalModel(Models.Metadata.Machine item)
        {
            var rowItems = new List<Row>();

            var roms = item.Read<Models.Metadata.Rom[]>(Models.Metadata.Machine.RomKey);
            if (roms != null && roms.Any())
            {
                rowItems.AddRange(roms
                    .Where(r => r != null)
                    .Select(rom => ConvertFromInternalModel(rom, item)));
            }

            var disks = item.Read<Models.Metadata.Disk[]>(Models.Metadata.Machine.DiskKey);
            if (disks != null && disks.Any())
            {
                rowItems.AddRange(disks
                    .Where(d => d != null)
                    .Select(disk => ConvertFromInternalModel(disk, item)));
            }

            var media = item.Read<Models.Metadata.Media[]>(Models.Metadata.Machine.MediaKey);
            if (media != null && media.Any())
            {
                rowItems.AddRange(media
                    .Where(m => m != null)
                    .Select(medium => ConvertFromInternalModel(medium, item)));
            }

            return rowItems.ToArray();
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Disk"/> to <cref="Models.SeparatedValue.Row"/>
        /// </summary>
        private static Row ConvertFromInternalModel(Models.Metadata.Disk item, Models.Metadata.Machine parent)
        {
            var row = new Row
            {
                GameName = parent.ReadString(Models.Metadata.Machine.NameKey),
                Description = parent.ReadString(Models.Metadata.Machine.DescriptionKey),
                Type = "disk",
                DiskName = item.ReadString(Models.Metadata.Disk.NameKey),
                MD5 = item.ReadString(Models.Metadata.Disk.MD5Key),
                SHA1 = item.ReadString(Models.Metadata.Disk.SHA1Key),
                Status = item.ReadString(Models.Metadata.Disk.StatusKey),
            };
            return row;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Media"/> to <cref="Models.SeparatedValue.Row"/>
        /// </summary>
        private static Row ConvertFromInternalModel(Models.Metadata.Media item, Models.Metadata.Machine parent)
        {
            var row = new Row
            {
                GameName = parent.ReadString(Models.Metadata.Machine.NameKey),
                Description = parent.ReadString(Models.Metadata.Machine.DescriptionKey),
                Type = "media",
                DiskName = item.ReadString(Models.Metadata.Media.NameKey),
                MD5 = item.ReadString(Models.Metadata.Media.MD5Key),
                SHA1 = item.ReadString(Models.Metadata.Media.SHA1Key),
                SHA256 = item.ReadString(Models.Metadata.Media.SHA256Key),
                SpamSum = item.ReadString(Models.Metadata.Media.SpamSumKey),
            };
            return row;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Rom"/> to <cref="Models.SeparatedValue.Row"/>
        /// </summary>
        private static Row ConvertFromInternalModel(Models.Metadata.Rom item, Models.Metadata.Machine parent)
        {
            var row = new Row
            {
                GameName = parent?.ReadString(Models.Metadata.Machine.NameKey),
                Description = parent?.ReadString(Models.Metadata.Machine.DescriptionKey),
                Type = "rom",
                RomName = item.ReadString(Models.Metadata.Rom.NameKey),
                Size = item.ReadString(Models.Metadata.Rom.SizeKey),
                CRC = item.ReadString(Models.Metadata.Rom.CRCKey),
                MD5 = item.ReadString(Models.Metadata.Rom.MD5Key),
                SHA1 = item.ReadString(Models.Metadata.Rom.SHA1Key),
                SHA256 = item.ReadString(Models.Metadata.Rom.SHA256Key),
                SHA384 = item.ReadString(Models.Metadata.Rom.SHA384Key),
                SHA512 = item.ReadString(Models.Metadata.Rom.SHA512Key),
                SpamSum = item.ReadString(Models.Metadata.Rom.SpamSumKey),
                Status = item.ReadString(Models.Metadata.Rom.StatusKey),
            };
            return row;
        }

        #endregion
    }
}