using System;
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
        /// Deserializes a MAME listrom file to the defined type
        /// </summary>
        /// <param name="path">Path to the file to deserialize</param>
        /// <returns>Deserialized data on success, null on failure</returns>
        public static MetadataFile? Deserialize(string path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Deserialize(stream);
        }

        /// <summary>
        /// Deserializes a MAME listrom file in a stream to the defined type
        /// </summary>
        /// <param name="stream">Stream to deserialize</param>
        /// <returns>Deserialized data on success, null on failure</returns>
        public static MetadataFile? Deserialize(Stream? stream)
        {
            // If the stream is null
            if (stream == null)
                return default;

            // Setup the reader and output
            var reader = new StreamReader(stream, Encoding.UTF8);
            var dat = new MetadataFile();

            Set? set = null;
            var sets = new List<Set>();
            var rows = new List<Row>();

            var additional = new List<string>();
            while (!reader.EndOfStream)
            {
                // Read the line and don't split yet
                string? line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                {
                    // If we have a set to process
                    if (set != null)
                    {
                        set.Row = rows.ToArray();
                        sets.Add(set);
                        set = null;
                        rows.Clear();
                    }

                    continue;
                }

                // Set lines are unique
                if (line.StartsWith("ROMs required for driver"))
                {
                    string driver = line["ROMs required for driver".Length..].Trim('"', ' ', '.');
                    set = new Set { Driver = driver };
                    continue;
                }
                else if (line.StartsWith("No ROMs required for driver"))
                {
                    string driver = line["No ROMs required for driver".Length..].Trim('"', ' ', '.');
                    set = new Set { Driver = driver };
                    continue;
                }
                else if (line.StartsWith("ROMs required for device"))
                {
                    string device = line["ROMs required for device".Length..].Trim('"', ' ', '.');
                    set = new Set { Device = device };
                    continue;
                }
                else if (line.StartsWith("No ROMs required for device"))
                {
                    string device = line["No ROMs required for device".Length..].Trim('"', ' ', '.');
                    set = new Set { Device = device };
                    continue;
                }
                else if (line.Equals("Name                                   Size Checksum", StringComparison.OrdinalIgnoreCase))
                {
                    // No-op
                    continue;
                }

                // Split the line for the name iteratively
                string[] lineParts = line.Split("     ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (lineParts.Length == 1)
                    lineParts = line.Split("    ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (lineParts.Length == 1)
                    lineParts = line.Split("   ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (lineParts.Length == 1)
                    lineParts = line.Split("  ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                // Read the name and set the rest of the line for processing
                string name = lineParts[0];
                string trimmedLine = line[name.Length..];
                if (trimmedLine == null)
                    continue;

                lineParts = trimmedLine.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                // The number of items in the row explains what type of row it is
                var row = new Row();
                switch (lineParts.Length)
                {
                    // Normal CHD (Name, MD5/SHA1)
                    case 1:
                        row.Name = name;
                        if (line.Contains("MD5("))
                            row.MD5 = lineParts[0]["MD5".Length..].Trim('(', ')');
                        else
                            row.SHA1 = lineParts[0]["SHA1".Length..].Trim('(', ')');
                        break;

                    // Normal ROM (Name, Size, CRC, SHA1)
                    case 3 when line.Contains("CRC"):
                        row.Name = name;
                        row.Size = lineParts[0];
                        row.CRC = lineParts[1]["CRC".Length..].Trim('(', ')');
                        row.SHA1 = lineParts[2]["SHA1".Length..].Trim('(', ')');
                        break;

                    // Bad CHD (Name, BAD, SHA1, BAD_DUMP)
                    case 3 when line.Contains("BAD_DUMP"):
                        row.Name = name;
                        row.Bad = true;
                        if (line.Contains("MD5("))
                            row.MD5 = lineParts[1]["MD5".Length..].Trim('(', ')');
                        else
                            row.SHA1 = lineParts[1]["SHA1".Length..].Trim('(', ')');
                        break;

                    // Nodump CHD (Name, NO GOOD DUMP KNOWN)
                    case 4 when line.Contains("NO GOOD DUMP KNOWN"):
                        row.Name = name;
                        row.NoGoodDumpKnown = true;
                        break;

                    // Bad ROM (Name, Size, BAD, CRC, MD5/SHA1, BAD_DUMP)
                    case 5 when line.Contains("BAD_DUMP"):
                        row.Name = name;
                        row.Size = lineParts[0];
                        row.Bad = true;
                        row.CRC = lineParts[2]["CRC".Length..].Trim('(', ')');
                        if (line.Contains("MD5("))
                            row.SHA1 = lineParts[3]["MD5".Length..].Trim('(', ')');
                        else
                            row.SHA1 = lineParts[3]["SHA1".Length..].Trim('(', ')');
                        break;

                    // Nodump ROM (Name, Size, NO GOOD DUMP KNOWN)
                    case 5 when line.Contains("NO GOOD DUMP KNOWN"):
                        row.Name = name;
                        row.Size = lineParts[0];
                        row.NoGoodDumpKnown = true;
                        break;

                    default:
                        row = null;
                        additional.Add(line);
                        break;
                }

                if (row != null)
                    rows.Add(row);
            }

            // If we have a set to process
            if (set != null)
            {
                set.Row = rows.ToArray();
                sets.Add(set);
                set = null;
                rows.Clear();
            }

            // Add extra pieces and return
            dat.Set = sets.ToArray();
            dat.ADDITIONAL_ELEMENTS = additional.ToArray();
            return dat;
        }
    
        #region Internal

        /// <summary>
        /// Convert from <cref="Models.Metadata.MetadataFile"/> to <cref="Models.Listrom.MetadataFile"/>
        /// </summary>
        public static MetadataFile? ConvertFromInternalModel(Models.Metadata.MetadataFile? item)
        {
            if (item == null)
                return null;

            var metadataFile = new MetadataFile();

            var machines = item.Read<Models.Metadata.Machine[]>(Models.Metadata.MetadataFile.MachineKey);
            if (machines != null && machines.Any())
            {
                metadataFile.Set = machines
                    .Where(m => m != null)
                    .Select(ConvertMachineFromInternalModel)
                    .ToArray();
            }
            
            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Machine"/> to <cref="Models.Listrom.Set"/>
        /// </summary>
        private static Set ConvertMachineFromInternalModel(Models.Metadata.Machine item)
        {
            var set = new Set();
            if (item.ReadString(Models.Metadata.Machine.IsDeviceKey) == "yes")
                set.Device = item.ReadString(Models.Metadata.Machine.NameKey);
            else
                set.Driver = item.ReadString(Models.Metadata.Machine.NameKey);

            var rowItems = new List<Row>();

            var roms = item.Read<Models.Metadata.Rom[]>(Models.Metadata.Machine.RomKey);
            if (roms != null)
            {
                rowItems.AddRange(roms.Where(r => r != null).Select(ConvertFromInternalModel));
            }

            var disks = item.Read<Models.Metadata.Disk[]>(Models.Metadata.Machine.DiskKey);
            if (disks != null)
                rowItems.AddRange(disks.Where(d => d != null).Select(ConvertFromInternalModel));

            set.Row = rowItems.ToArray();
            return set;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Disk"/> to <cref="Models.Listrom.Row"/>
        /// </summary>
        private static Row ConvertFromInternalModel(Models.Metadata.Disk item)
        {
            var row = new Row
            {
                Name = item.ReadString(Models.Metadata.Disk.NameKey),
                MD5 = item.ReadString(Models.Metadata.Disk.MD5Key),
                SHA1 = item.ReadString(Models.Metadata.Disk.SHA1Key),
            };

            if (item[Models.Metadata.Disk.StatusKey] as string == "nodump")
                row.NoGoodDumpKnown = true;
            else if (item[Models.Metadata.Disk.StatusKey] as string == "baddump")
                row.Bad = true;

            return row;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Rom"/> to <cref="Models.Listrom.Row"/>
        /// </summary>
        private static Row ConvertFromInternalModel(Models.Metadata.Rom item)
        {
            var row = new Row
            {
                Name = item.ReadString(Models.Metadata.Rom.NameKey),
                Size = item.ReadString(Models.Metadata.Rom.SizeKey),
                CRC = item.ReadString(Models.Metadata.Rom.CRCKey),
                SHA1 = item.ReadString(Models.Metadata.Rom.SHA1Key),
            };

            if (item[Models.Metadata.Rom.StatusKey] as string == "nodump")
                row.NoGoodDumpKnown = true;
            else if (item[Models.Metadata.Rom.StatusKey] as string == "baddump")
                row.Bad = true;

            return row;
        }

        #endregion
    }
}