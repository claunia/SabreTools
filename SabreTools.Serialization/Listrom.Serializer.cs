using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SabreTools.Models.Listrom;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Deserializer for MAME listrom files
    /// </summary>
    public partial class Listrom
    {
        /// <summary>
        /// Serializes the defined type to a MAME listrom file
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
            var writer = new StreamWriter(stream, Encoding.UTF8);

            // Write out the sets, if they exist
            WriteSets(metadataFile.Set, writer);

            // Return the stream
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// Write sets information to the current writer
        /// </summary>
        /// <param name="sets">Array of Set objects representing the sets information</param>
        /// <param name="writer">StreamWriter representing the output</param>
        private static void WriteSets(Set[]? sets, StreamWriter writer)
        {
            // If the games information is missing, we can't do anything
            if (sets == null || !sets.Any())
                return;

            // Loop through and write out the games
            foreach (var set in sets)
            {
                WriteSet(set, writer);
                writer.Flush();
            }
        }

        /// <summary>
        /// Write set information to the current writer
        /// </summary>
        /// <param name="set">Set object representing the set information</param>
        /// <param name="writer">StreamWriter representing the output</param>
        private static void WriteSet(Set set, StreamWriter writer)
        {
            // If the set information is missing, we can't do anything
            if (set == null)
                return;

            if (!string.IsNullOrWhiteSpace(set.Driver))
            {
                if (set.Row != null && set.Row.Any())
                {
                    writer.WriteLine($"ROMs required for driver \"{set.Driver}\".");
                    writer.WriteLine("Name                                   Size Checksum");
                    writer.Flush();

                    WriteRows(set.Row, writer);

                    writer.WriteLine();
                    writer.Flush();
                }
                else
                {
                    writer.WriteLine($"No ROMs required for driver \"{set.Driver}\".");
                    writer.WriteLine();
                    writer.Flush();
                }
            }
            else if (!string.IsNullOrWhiteSpace(set.Device))
            {
                if (set.Row != null && set.Row.Any())
                {
                    writer.WriteLine($"ROMs required for device \"{set.Device}\".");
                    writer.WriteLine("Name                                   Size Checksum");
                    writer.Flush();

                    WriteRows(set.Row, writer);

                    writer.WriteLine();
                    writer.Flush();
                }
                else
                {
                    writer.WriteLine($"No ROMs required for device \"{set.Device}\".");
                    writer.WriteLine();
                    writer.Flush();
                }
            }
        }

        /// <summary>
        /// Write rows information to the current writer
        /// </summary>
        /// <param name="rows">Array of Row objects to write</param>
        /// <param name="writer">StreamWriter representing the output</param>
        private static void WriteRows(Row[]? rows, StreamWriter writer)
        {
            // If the array is missing, we can't do anything
            if (rows == null)
                return;

            foreach (var row in rows)
            {
                var rowBuilder = new StringBuilder();

                int padding = 40 - (row.Size?.Length ?? 0);
                if (padding < row.Name.Length)
                    padding = row.Name.Length + 2;

                rowBuilder.Append($"{row.Name.PadRight(padding, ' ')}");
                if (row.Size != null)
                    rowBuilder.Append($"{row.Size} ");

                if (row.NoGoodDumpKnown)
                {
                    rowBuilder.Append("NO GOOD DUMP KNOWN");
                }
                else
                {
                    if (row.Bad)
                        rowBuilder.Append("BAD ");

                    if (row.Size != null)
                    {
                        rowBuilder.Append($"CRC({row.CRC}) ");
                        rowBuilder.Append($"SHA1({row.SHA1}) ");
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(row.MD5))
                            rowBuilder.Append($"MD5({row.MD5}) ");
                        else
                            rowBuilder.Append($"SHA1({row.SHA1}) ");
                    }

                    if (row.Bad)
                        rowBuilder.Append("BAD_DUMP");
                }

                writer.WriteLine(rowBuilder.ToString().TrimEnd());
                writer.Flush();
            }
        }

        #region Internal

        /// <summary>
        /// Convert from <cref="Models.Listrom.MetadataFile"/> to <cref="Models.Internal.MetadataFile"/>
        /// </summary>
        public static Models.Internal.MetadataFile ConvertToInternalModel(MetadataFile item)
        {
            var metadataFile = new Models.Internal.MetadataFile
            {
                [Models.Internal.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item),
            };

            if (item?.Set != null && item.Set.Any())
                metadataFile[Models.Internal.MetadataFile.MachineKey] = item.Set.Select(ConvertMachineToInternalModel).ToArray();

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.Listrom.MetadataFile"/> to <cref="Header"/>
        /// </summary>
        private static Models.Internal.Header ConvertHeaderToInternalModel(MetadataFile item)
        {
            var header = new Models.Internal.Header
            {
                [Models.Internal.Header.NameKey] = "MAME Listrom",
            };
            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.Listrom.Set"/> to <cref="Models.Internal.Machine"/>
        /// </summary>
        private static Models.Internal.Machine ConvertMachineToInternalModel(Set item)
        {
            var machine = new Models.Internal.Machine();
            if (!string.IsNullOrWhiteSpace(item.Device))
            {
                machine[Models.Internal.Machine.NameKey] = item.Device;
                machine[Models.Internal.Machine.IsDeviceKey] = "yes";
            }
            else
            {
                machine[Models.Internal.Machine.NameKey] = item.Driver;
            }

            if (item.Row != null && item.Row.Any())
            {
                var datItems = new List<Models.Internal.DatItem>();
                foreach (var file in item.Row)
                {
                    datItems.Add(ConvertToInternalModel(file));
                }

                machine[Models.Internal.Machine.DiskKey] = datItems.Where(i => i.ReadString(Models.Internal.DatItem.TypeKey) == "disk")?.ToArray();
                machine[Models.Internal.Machine.RomKey] = datItems.Where(i => i.ReadString(Models.Internal.DatItem.TypeKey) == "rom")?.ToArray();
            }

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.Listrom.Row"/> to <cref="Models.Internal.DatItem"/>
        /// </summary>
        private static Models.Internal.DatItem ConvertToInternalModel(Row item)
        {
            if (item.Size == null)
            {
                var disk = new Models.Internal.Disk
                {
                    [Models.Internal.Disk.NameKey] = item.Name,
                    [Models.Internal.Disk.MD5Key] = item.MD5,
                    [Models.Internal.Disk.SHA1Key] = item.SHA1,
                };

                if (item.NoGoodDumpKnown)
                    disk[Models.Internal.Disk.StatusKey] = "nodump";
                else if (item.Bad)
                    disk[Models.Internal.Disk.StatusKey] = "baddump";

                return disk;
            }
            else
            {
                var rom = new Models.Internal.Rom
                {
                    [Models.Internal.Rom.NameKey] = item.Name,
                    [Models.Internal.Rom.SizeKey] = item.Size,
                    [Models.Internal.Rom.CRCKey] = item.CRC,
                    [Models.Internal.Rom.SHA1Key] = item.SHA1,
                };

                if (item.NoGoodDumpKnown)
                    rom[Models.Internal.Rom.StatusKey] = "nodump";
                else if (item.Bad)
                    rom[Models.Internal.Rom.StatusKey] = "baddump";

                return rom;
            }
        }

        #endregion
    }
}