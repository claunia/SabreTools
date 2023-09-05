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
                if (string.IsNullOrWhiteSpace(row.Name))
                    continue;

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
        /// Convert from <cref="Models.Listrom.MetadataFile"/> to <cref="Models.Metadata.MetadataFile"/>
        /// </summary>
        public static Models.Metadata.MetadataFile? ConvertToInternalModel(MetadataFile? item)
        {
            if (item == null)
                return null;
            
            var metadataFile = new Models.Metadata.MetadataFile
            {
                [Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(),
            };

            if (item?.Set != null && item.Set.Any())
            {
                metadataFile[Models.Metadata.MetadataFile.MachineKey] = item.Set
                    .Where(s => s != null)
                    .Select(ConvertMachineToInternalModel)
                    .ToArray();
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.Listrom.MetadataFile"/> to <cref="Header"/>
        /// </summary>
        private static Models.Metadata.Header ConvertHeaderToInternalModel()
        {
            var header = new Models.Metadata.Header
            {
                [Models.Metadata.Header.NameKey] = "MAME Listrom",
            };
            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.Listrom.Set"/> to <cref="Models.Metadata.Machine"/>
        /// </summary>
        private static Models.Metadata.Machine ConvertMachineToInternalModel(Set item)
        {
            var machine = new Models.Metadata.Machine();
            if (!string.IsNullOrWhiteSpace(item.Device))
            {
                machine[Models.Metadata.Machine.NameKey] = item.Device;
                machine[Models.Metadata.Machine.IsDeviceKey] = "yes";
            }
            else
            {
                machine[Models.Metadata.Machine.NameKey] = item.Driver;
            }

            if (item.Row != null && item.Row.Any())
            {
                var datItems = new List<Models.Metadata.DatItem>();
                foreach (var file in item.Row)
                {
                    datItems.Add(ConvertToInternalModel(file));
                }

                machine[Models.Metadata.Machine.DiskKey] = datItems.Where(i => i.ReadString(Models.Metadata.DatItem.TypeKey) == "disk")?.ToArray();
                machine[Models.Metadata.Machine.RomKey] = datItems.Where(i => i.ReadString(Models.Metadata.DatItem.TypeKey) == "rom")?.ToArray();
            }

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.Listrom.Row"/> to <cref="Models.Metadata.DatItem"/>
        /// </summary>
        private static Models.Metadata.DatItem ConvertToInternalModel(Row item)
        {
            if (item.Size == null)
            {
                var disk = new Models.Metadata.Disk
                {
                    [Models.Metadata.Disk.NameKey] = item.Name,
                    [Models.Metadata.Disk.MD5Key] = item.MD5,
                    [Models.Metadata.Disk.SHA1Key] = item.SHA1,
                };

                if (item.NoGoodDumpKnown)
                    disk[Models.Metadata.Disk.StatusKey] = "nodump";
                else if (item.Bad)
                    disk[Models.Metadata.Disk.StatusKey] = "baddump";

                return disk;
            }
            else
            {
                var rom = new Models.Metadata.Rom
                {
                    [Models.Metadata.Rom.NameKey] = item.Name,
                    [Models.Metadata.Rom.SizeKey] = item.Size,
                    [Models.Metadata.Rom.CRCKey] = item.CRC,
                    [Models.Metadata.Rom.SHA1Key] = item.SHA1,
                };

                if (item.NoGoodDumpKnown)
                    rom[Models.Metadata.Rom.StatusKey] = "nodump";
                else if (item.Bad)
                    rom[Models.Metadata.Rom.StatusKey] = "baddump";

                return rom;
            }
        }

        #endregion
    }
}