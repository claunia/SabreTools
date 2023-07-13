using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.Models.Listrom;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for MAME listrom files
    /// </summary>
    public class Listrom
    {
        /// <summary>
        /// Deserializes a MAME listrom file to the defined type
        /// </summary>
        /// <param name="path">Path to the file to deserialize</param>
        /// <returns>Deserialized data on success, null on failure</returns>
        public static Dat? Deserialize(string path)
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
        /// Deserializes a MAME listrom file in a stream to the defined type
        /// </summary>
        /// <param name="stream">Stream to deserialize</param>
        /// <returns>Deserialized data on success, null on failure</returns>
        public static Dat? Deserialize(Stream? stream)
        {
            try
            {
                // If the stream is null
                if (stream == null)
                    return default;

                // Setup the reader and output
                var reader = new StreamReader(stream, Encoding.UTF8);
                var dat = new Dat();

                Set? set = null;
                var sets = new List<Set?>();
                var rows = new List<Row?>();

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
                    string[]? lineParts = line?.Split("     ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (lineParts?.Length == 1)
                        lineParts = line?.Split("    ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (lineParts?.Length == 1)
                        lineParts = line?.Split("   ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (lineParts?.Length == 1)
                        lineParts = line?.Split("  ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                    // Read the name and set the rest of the line for processing
                    string name = lineParts[0];
                    string trimmedLine = line[name.Length..];

                    lineParts = trimmedLine?.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                    // The number of items in the row explains what type of row it is
                    var row = new Row();
                    switch (lineParts.Length)
                    {
                        // Normal CHD (Name, SHA1)
                        case 1:
                            row.Name = name;
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
                            row.SHA1 = lineParts[1]["SHA1".Length..].Trim('(', ')');
                            break;

                        // Nodump CHD (Name, NO GOOD DUMP KNOWN)
                        case 4 when line.Contains("NO GOOD DUMP KNOWN"):
                            row.Name = name;
                            row.NoGoodDumpKnown = true;
                            break;

                        // Bad ROM (Name, Size, BAD, CRC, SHA1, BAD_DUMP)
                        case 5 when line.Contains("BAD_DUMP"):
                            row.Name = name;
                            row.Size = lineParts[0];
                            row.Bad = true;
                            row.CRC = lineParts[2]["CRC".Length..].Trim('(', ')');
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
            catch
            {
                // TODO: Handle logging the exception
                return default;
            }
        }
    }
}