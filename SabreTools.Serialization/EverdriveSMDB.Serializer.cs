using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SabreTools.IO.Writers;
using SabreTools.Models.EverdriveSMDB;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Separated value serializer for Everdrive SMDBs
    /// </summary>
    public partial class EverdriveSMDB
    {
        /// <summary>
        /// Serializes the defined type to an Everdrive SMDB file
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
            var writer = new SeparatedValueWriter(stream, Encoding.UTF8) { Separator = '\t', Quotes = false };

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
                if (row == null)
                    continue;

                var rowArray = new List<string?>
                {
                    row.SHA256,
                    row.Name,
                    row.SHA1,
                    row.MD5,
                    row.CRC32,
                };

                if (row.Size != null)
                    rowArray.Add(row.Size);

                writer.WriteValues(rowArray.ToArray());
                writer.Flush();
            }
        }

        #region Internal

        /// <summary>
        /// Convert from <cref="Models.EverdriveSMDB.MetadataFile"/> to <cref="Models.Metadata.MetadataFile"/>
        /// </summary>
        public static Models.Metadata.MetadataFile? ConvertToInternalModel(MetadataFile? item)
        {
            if (item == null)
                return null;
            
            var metadataFile = new Models.Metadata.MetadataFile
            {
                [Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(),
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
        /// Convert from <cref="Models.EverdriveSMDB.MetadataFile"/> to <cref=Models.Metadata."Header"/>
        /// </summary>
        private static Models.Metadata.Header ConvertHeaderToInternalModel()
        {
            var header = new Models.Metadata.Header
            {
                [Models.Metadata.Header.NameKey] = "Everdrive SMDB",
            };
            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.EverdriveSMDB.Row"/> to <cref="Models.Metadata.Machine"/>
        /// </summary>
        private static Models.Metadata.Machine ConvertMachineToInternalModel(Row item)
        {
            var machine = new Models.Metadata.Machine
            {
                [Models.Metadata.Machine.RomKey] = ConvertToInternalModel(item),
            };
            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.EverdriveSMDB.Row"/> to <cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Models.Metadata.Rom ConvertToInternalModel(Row item)
        {
            var rom = new Models.Metadata.Rom
            {
                [Models.Metadata.Rom.SHA256Key] = item.SHA256,
                [Models.Metadata.Rom.NameKey] = item.Name,
                [Models.Metadata.Rom.SHA1Key] = item.SHA1,
                [Models.Metadata.Rom.MD5Key] = item.MD5,
                [Models.Metadata.Rom.CRCKey] = item.CRC32,
                [Models.Metadata.Rom.SizeKey] = item.Size,
            };
            return rom;
        }

        #endregion
    }
}