using System.IO;
using System.Linq;
using System.Text;
using SabreTools.IO.Writers;
using SabreTools.Models.SeparatedValue;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for separated-value variants
    /// </summary>
    public partial class SeparatedValue
    {
        /// <summary>
        /// Serializes the defined type to a separated-value variant
        /// </summary>
        /// <param name="metadataFile">Data to serialize</param>
        /// <param name="path">Path to the file to serialize to</param>
        /// <param name="delim">Character delimiter between values</param>
        /// <returns>True on successful serialization, false otherwise</returns>
        public static bool SerializeToFile(MetadataFile? metadataFile, string path, char delim)
        {
            using var stream = SerializeToStream(metadataFile, delim);
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
        /// <param name="delim">Character delimiter between values</param>
        /// <returns>Stream containing serialized data on success, null otherwise</returns>
        public static Stream? SerializeToStream(MetadataFile? metadataFile, char delim)
        {
            // If the metadata file is null
            if (metadataFile == null)
                return null;

            // Setup the writer and output
            var stream = new MemoryStream();
            var writer = new SeparatedValueWriter(stream, Encoding.UTF8) { Separator = delim, Quotes = true };

            // TODO: Include flag to write out long or short header
            // Write the short header
            WriteHeader(writer);

            // Write out the rows, if they exist
            WriteRows(metadataFile.Row, writer);

            // Return the stream
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// Write header information to the current writer
        /// </summary>
        /// <param name="writer">SeparatedValueWriter representing the output</param>
        private static void WriteHeader(SeparatedValueWriter writer)
        {
            var headerArray = new string?[]
            {
                "File Name",
                "Internal Name",
                "Description",
                "Game Name",
                "Game Description",
                "Type",
                "Rom Name",
                "Disk Name",
                "Size",
                "CRC",
                "MD5",
                "SHA1",
                "SHA256",
                //"SHA384",
                //"SHA512",
                //"SpamSum",
                "Status",
            };

            writer.WriteHeader(headerArray);
            writer.Flush();
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
                    row.FileName,
                    row.InternalName,
                    row.Description,
                    row.GameName,
                    row.GameDescription,
                    row.Type,
                    row.RomName,
                    row.DiskName,
                    row.Size,
                    row.CRC,
                    row.MD5,
                    row.SHA1,
                    row.SHA256,
                    //row.SHA384,
                    //row.SHA512,
                    //row.SpamSum,
                    row.Status,
                };

                writer.WriteValues(rowArray);
                writer.Flush();
            }
        }

        #region Internal

        /// <summary>
        /// Convert from <cref="Models.SeparatedValue.MetadataFile"/> to <cref="Models.Metadata.MetadataFile"/>
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
                metadataFile[Models.Metadata.MetadataFile.MachineKey] = item.Row.Select(ConvertMachineToInternalModel).ToArray();

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.SeparatedValue.MetadataFile"/> to <cref="Models.Metadata.Header"/>
        /// </summary>
        private static Models.Metadata.Header ConvertHeaderToInternalModel(MetadataFile item)
        {
            var header = new Models.Metadata.Header
            {
                [Models.Metadata.Header.HeaderKey] = item.Header,
            };

            if (item.Row != null && item.Row.Any())
            {
                var first = item.Row[0];
                //header[Models.Metadata.Header.FileNameKey] = first.FileName; // Not possible to map
                header[Models.Metadata.Header.NameKey] = first.FileName;
                header[Models.Metadata.Header.DescriptionKey] = first.Description;
            }

            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.SeparatedValue.Row"/> to <cref="Models.Metadata.Machine"/>
        /// </summary>
        private static Models.Metadata.Machine ConvertMachineToInternalModel(Row item)
        {
            var machine = new Models.Metadata.Machine
            {
                [Models.Metadata.Machine.NameKey] = item.GameName,
                [Models.Metadata.Machine.DescriptionKey] = item.GameDescription,
            };

            var datItem = ConvertToInternalModel(item);
            switch (datItem)
            {
                case Models.Metadata.Disk disk:
                    machine[Models.Metadata.Machine.DiskKey] = new Models.Metadata.Disk[] { disk };
                    break;

                case Models.Metadata.Media media:
                    machine[Models.Metadata.Machine.MediaKey] = new Models.Metadata.Media[] { media };
                    break;

                case Models.Metadata.Rom rom:
                    machine[Models.Metadata.Machine.RomKey] = new Models.Metadata.Rom[] { rom };
                    break;
            }

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.SeparatedValue.Row"/> to <cref="Models.Metadata.DatItem"/>
        /// </summary>
        private static Models.Metadata.DatItem? ConvertToInternalModel(Row item)
        {
            return item.Type switch
            {
                "disk" => new Models.Metadata.Disk
                {
                    [Models.Metadata.Disk.NameKey] = item.DiskName,
                    [Models.Metadata.Disk.MD5Key] = item.MD5,
                    [Models.Metadata.Disk.SHA1Key] = item.SHA1,
                    [Models.Metadata.Disk.StatusKey] = item.Status,
                },
                "media" => new Models.Metadata.Media
                {
                    [Models.Metadata.Media.NameKey] = item.DiskName,
                    [Models.Metadata.Media.MD5Key] = item.MD5,
                    [Models.Metadata.Media.SHA1Key] = item.SHA1,
                    [Models.Metadata.Media.SHA256Key] = item.SHA256,
                    [Models.Metadata.Media.SpamSumKey] = item.SpamSum,
                },
                "rom" => new Models.Metadata.Rom
                {
                    [Models.Metadata.Rom.NameKey] = item.RomName,
                    [Models.Metadata.Rom.SizeKey] = item.Size,
                    [Models.Metadata.Rom.CRCKey] = item.CRC,
                    [Models.Metadata.Rom.MD5Key] = item.MD5,
                    [Models.Metadata.Rom.SHA1Key] = item.SHA1,
                    [Models.Metadata.Rom.SHA256Key] = item.SHA256,
                    [Models.Metadata.Rom.SHA384Key] = item.SHA384,
                    [Models.Metadata.Rom.SHA512Key] = item.SHA512,
                    [Models.Metadata.Rom.SpamSumKey] = item.SpamSum,
                    [Models.Metadata.Rom.StatusKey] = item.Status,
                },
                _ => null,
            };
        }

        #endregion
    }
}