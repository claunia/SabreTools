using System;
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
            using var stream = PathProcessor.OpenStream(path);
            return Deserialize(stream);
        }

        /// <summary>
        /// Deserializes an Everdrive SMDB in a stream to the defined type
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

        #region Internal

        /// <summary>
        /// Convert from <cref="Models.Internal.MetadataFile"/> to <cref="Models.EverdriveSMDB.MetadataFile"/>
        /// </summary>
        public static MetadataFile? ConvertFromInternalModel(Models.Internal.MetadataFile? item)
        {
            if (item == null)
                return null;

            var metadataFile = new MetadataFile();

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
        /// Convert from <cref="Models.Internal.Machine"/> to an array of <cref="Models.EverdriveSMDB.Row"/>
        /// </summary>
        private static Row[] ConvertMachineFromInternalModel(Models.Internal.Machine item)
        {
            var roms = item.Read<Models.Internal.Rom[]>(Models.Internal.Machine.RomKey);
            if (roms == null || !roms.Any())
                return Array.Empty<Row>();

            return roms
                .Where(r => r != null)
                .Select(ConvertFromInternalModel)
                .ToArray();
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.EverdriveSMDB.Row"/>
        /// </summary>
        private static Row ConvertFromInternalModel(Models.Internal.Rom item)
        {
            var row = new Row
            {
                SHA256 = item.ReadString(Models.Internal.Rom.SHA256Key),
                Name = item.ReadString(Models.Internal.Rom.NameKey),
                SHA1 = item.ReadString(Models.Internal.Rom.SHA1Key),
                MD5 = item.ReadString(Models.Internal.Rom.MD5Key),
                CRC32 = item.ReadString(Models.Internal.Rom.CRCKey),
                Size = item.ReadString(Models.Internal.Rom.SizeKey),
            };
            return row;
        }

        #endregion
    }
}