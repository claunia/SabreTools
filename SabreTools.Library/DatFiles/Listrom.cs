using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using SabreTools.Library.DatItems;
using SabreTools.Library.IO;
using SabreTools.Library.Tools;

namespace SabreTools.Library.DatFiles
{
    /// <summary>
    /// Represents parsing and writing of a MAME Listrom DAT
    /// </summary>
    internal class Listrom : DatFile
    {
        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public Listrom(DatFile datFile)
            : base(datFile)
        {
        }

        /// <summary>
        /// Parse a MAME Listrom DAT and return all found games and roms within
        /// </summary>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        /// <remarks>
        /// In a new style MAME listrom DAT, each game has the following format:
        /// 
        /// ROMs required for driver "005".
        /// Name                                   Size Checksum
        /// 1346b.cpu-u25                          2048 CRC(8e68533e) SHA1(a257c556d31691068ed5c991f1fb2b51da4826db)
        /// 6331.sound-u8                            32 BAD CRC(1d298cb0) SHA1(bb0bb62365402543e3154b9a77be9c75010e6abc) BAD_DUMP
        /// 16v8h-blue.u24                          279 NO GOOD DUMP KNOWN
        /// </remarks>
        protected override void ParseFile(string filename, int indexId, bool keep, bool throwOnError = false)
        {
            // Open a file reader
            Encoding enc = FileExtensions.GetEncoding(filename);
            StreamReader sr = new StreamReader(FileExtensions.TryOpenRead(filename), enc);

            string gamename = string.Empty;
            while (!sr.EndOfStream)
            {
                try
                {
                    string line = sr.ReadLine().Trim();

                    // If we have a blank line, we just skip it
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    // If we have the descriptor line, ignore it
                    else if (line == "Name                                   Size Checksum")
                    {
                        continue;
                    }

                    // If we have the beginning of a game, set the name of the game
                    else if (line.StartsWith("ROMs required for"))
                    {
                        gamename = Regex.Match(line, @"^ROMs required for \S*? string.Empty(.*?)string.Empty\.").Groups[1].Value;
                    }

                    // If we have a machine with no required roms (usually internal devices), skip it
                    else if (line.StartsWith("No ROMs required for"))
                    {
                        continue;
                    }

                    // Otherwise, we assume we have a rom that we need to add
                    else
                    {
                        // First, we preprocess the line so that the rom name is consistently correct
                        string[] split = line.Split(new string[] { "    " }, StringSplitOptions.RemoveEmptyEntries);

                        // If the line doesn't have the 4 spaces of padding, check for 3
                        if (split.Length == 1)
                            split = line.Split(new string[] { "   " }, StringSplitOptions.RemoveEmptyEntries);

                        // If the split is still unsuccessful, log it and skip
                        if (split.Length == 1)
                            logger.Warning($"Possibly malformed line: '{line}'");

                        string romname = split[0];
                        line = line.Substring(romname.Length);

                        // Next we separate the ROM into pieces
                        split = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

                        // Standard Disks have 2 pieces (name, sha1)
                        if (split.Length == 1)
                        {
                            Disk disk = new Disk()
                            {
                                Name = romname,
                                SHA1 = Sanitizer.CleanListromHashData(split[0]),

                                Machine = new Machine
                                {
                                    Name = gamename,
                                },

                                Source = new Source
                                {
                                    Index = indexId,
                                    Name = filename,
                                },
                            };

                            ParseAddHelper(disk);
                        }

                        // Baddump Disks have 4 pieces (name, BAD, sha1, BAD_DUMP)
                        else if (split.Length == 3 && line.EndsWith("BAD_DUMP"))
                        {
                            Disk disk = new Disk()
                            {
                                Name = romname,
                                SHA1 = Sanitizer.CleanListromHashData(split[1]),
                                ItemStatus = ItemStatus.BadDump,

                                Machine = new Machine
                                {
                                    Name = gamename,
                                },

                                Source = new Source
                                {
                                    Index = indexId,
                                    Name = filename,
                                },
                            };

                            ParseAddHelper(disk);
                        }

                        // Standard ROMs have 4 pieces (name, size, crc, sha1)
                        else if (split.Length == 3)
                        {
                            Rom rom = new Rom()
                            {
                                Name = romname,
                                Size = Sanitizer.CleanLong(split[0]),
                                CRC = Sanitizer.CleanListromHashData(split[1]),
                                SHA1 = Sanitizer.CleanListromHashData(split[2]),

                                Machine = new Machine
                                {
                                    Name = gamename,
                                },

                                Source = new Source
                                {
                                    Index = indexId,
                                    Name = filename,
                                },
                            };

                            ParseAddHelper(rom);
                        }

                        // Nodump Disks have 5 pieces (name, NO, GOOD, DUMP, KNOWN)
                        else if (split.Length == 4 && line.EndsWith("NO GOOD DUMP KNOWN"))
                        {
                            Disk disk = new Disk()
                            {
                                Name = romname,
                                ItemStatus = ItemStatus.Nodump,

                                Machine = new Machine
                                {
                                    Name = gamename,
                                },

                                Source = new Source
                                {
                                    Index = indexId,
                                    Name = filename,
                                },
                            };

                            ParseAddHelper(disk);
                        }

                        // Baddump ROMs have 6 pieces (name, size, BAD, crc, sha1, BAD_DUMP)
                        else if (split.Length == 5 && line.EndsWith("BAD_DUMP"))
                        {
                            Rom rom = new Rom()
                            {
                                Name = romname,
                                Size = Sanitizer.CleanLong(split[0]),
                                CRC = Sanitizer.CleanListromHashData(split[2]),
                                SHA1 = Sanitizer.CleanListromHashData(split[3]),
                                ItemStatus = ItemStatus.BadDump,

                                Machine = new Machine
                                {
                                    Name = gamename,
                                },

                                Source = new Source
                                {
                                    Index = indexId,
                                    Name = filename,
                                },
                            };

                            ParseAddHelper(rom);
                        }

                        // Nodump ROMs have 6 pieces (name, size, NO, GOOD, DUMP, KNOWN)
                        else if (split.Length == 5 && line.EndsWith("NO GOOD DUMP KNOWN"))
                        {
                            Rom rom = new Rom()
                            {
                                Name = romname,
                                Size = Sanitizer.CleanLong(split[0]),
                                ItemStatus = ItemStatus.Nodump,

                                Machine = new Machine
                                {
                                    Name = gamename,
                                },

                                Source = new Source
                                {
                                    Index = indexId,
                                    Name = filename,
                                },
                            };

                            ParseAddHelper(rom);
                        }

                        // If we have something else, it's invalid
                        else
                        {
                            logger.Warning($"Invalid line detected: '{romname} {line}'");
                        }
                    }
                }
                catch (Exception ex)
                {
                    string message = $"'{filename}' - There was an error parsing at position {sr.BaseStream.Position}";
                    logger.Error(ex, message);
                    if (throwOnError)
                    {
                        sr.Dispose();
                        throw new Exception(message, ex);
                    }
                }
            }
        }

        /// <inheritdoc/>
        protected override ItemType[] GetSupportedTypes()
        {
            return new ItemType[] { ItemType.Disk, ItemType.Rom };
        }

        /// <summary>
        /// Create and open an output file for writing direct from a dictionary
        /// </summary>
        /// <param name="outfile">Name of the file to write to</param>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise (default)</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        /// <returns>True if the DAT was written correctly, false otherwise</returns>
        public override bool WriteToFile(string outfile, bool ignoreblanks = false, bool throwOnError = false)
        {
            try
            {
                logger.User($"Opening file for writing: {outfile}");
                FileStream fs = FileExtensions.TryCreate(outfile);

                // If we get back null for some reason, just log and return
                if (fs == null)
                {
                    logger.Warning($"File '{outfile}' could not be created for writing! Please check to see if the file is writable");
                    return false;
                }

                StreamWriter sw = new StreamWriter(fs, new UTF8Encoding(false));

                // Write out each of the machines and roms
                string lastgame = null;

                // Use a sorted list of games to output
                foreach (string key in Items.SortedKeys)
                {
                    List<DatItem> datItems = Items.FilteredItems(key);

                    // If this machine doesn't contain any writable items, skip
                    if (!ContainsWritable(datItems))
                        continue;

                    // Resolve the names in the block
                    datItems = DatItem.ResolveNames(datItems);

                    for (int index = 0; index < datItems.Count; index++)
                    {
                        DatItem datItem = datItems[index];

                        // If we have a different game and we're not at the start of the list, output the end of last item
                        if (lastgame != null && lastgame.ToLowerInvariant() != datItem.Machine.Name.ToLowerInvariant())
                            WriteEndGame(sw);

                        // If we have a new game, output the beginning of the new item
                        if (lastgame == null || lastgame.ToLowerInvariant() != datItem.Machine.Name.ToLowerInvariant())
                            WriteStartGame(sw, datItem);

                        // Check for a "null" item
                        datItem = ProcessNullifiedItem(datItem);

                        // Write out the item if we're not ignoring
                        if (!ShouldIgnore(datItem, ignoreblanks))
                            WriteDatItem(sw, datItem);

                        // Set the new data to compare against
                        lastgame = datItem.Machine.Name;
                    }
                }

                logger.Verbose("File written!" + Environment.NewLine);
                sw.Dispose();
                fs.Dispose();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                if (throwOnError) throw ex;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Write out Game start using the supplied StreamWriter
        /// </summary>
        /// <param name="sw">StreamWriter to output to</param>
        /// <param name="rom">DatItem object to be output</param>
        private void WriteStartGame(StreamWriter sw, DatItem rom)
        {
            // No game should start with a path separator
            rom.Machine.Name = rom.Machine.Name.TrimStart(Path.DirectorySeparatorChar);

            // Build the state
            sw.Write($"ROMs required for driver \"{rom.Machine.Name}\".\n");
            sw.Write("Name                                   Size Checksum\n");

            sw.Flush();
        }

        /// <summary>
        /// Write out Game end using the supplied StreamWriter
        /// </summary>
        /// <param name="sw">StreamWriter to output to</param>
        private void WriteEndGame(StreamWriter sw)
        {
            // End driver
            sw.Write("\n");

            sw.Flush();
        }

        /// <summary>
        /// Write out DatItem using the supplied StreamWriter
        /// </summary>
        /// <param name="sw">StreamWriter to output to</param>
        /// <param name="datItem">DatItem object to be output</param>
        private void WriteDatItem(StreamWriter sw, DatItem datItem)
        {
            // Pre-process the item name
            ProcessItemName(datItem, true);

            // Build the state
            switch (datItem.ItemType)
            {
                case ItemType.Disk:
                    var disk = datItem as Disk;

                    // The name is padded out to a particular length
                    if (disk.Name.Length < 43)
                        sw.Write(disk.Name.PadRight(43, ' '));
                    else
                        sw.Write($"{disk.Name}          ");

                    // If we have a baddump, put the first indicator
                    if (disk.ItemStatus == ItemStatus.BadDump)
                        sw.Write(" BAD");

                    // If we have a nodump, write out the indicator
                    if (disk.ItemStatus == ItemStatus.Nodump)
                        sw.Write(" NO GOOD DUMP KNOWN");

                    // Otherwise, write out the SHA-1 hash
                    else if (!string.IsNullOrWhiteSpace(disk.SHA1))
                        sw.Write($" SHA1({disk.SHA1 ?? string.Empty})");

                    // If we have a baddump, put the second indicator
                    if (disk.ItemStatus == ItemStatus.BadDump)
                        sw.Write(" BAD_DUMP");

                    sw.Write("\n");
                    break;

                case ItemType.Rom:
                    var rom = datItem as Rom;

                    // The name is padded out to a particular length
                    if (rom.Name.Length < 43)
                        sw.Write(rom.Name.PadRight(43 - rom.Size?.ToString().Length ?? 0, ' '));
                    else
                        sw.Write($"{rom.Name}          ");

                    // If we don't have a nodump, write out the size
                    if (rom.ItemStatus != ItemStatus.Nodump)
                        sw.Write(rom.Size?.ToString() ?? string.Empty);

                    // If we have a baddump, put the first indicator
                    if (rom.ItemStatus == ItemStatus.BadDump)
                        sw.Write(" BAD");

                    // If we have a nodump, write out the indicator
                    if (rom.ItemStatus == ItemStatus.Nodump)
                    {
                        sw.Write(" NO GOOD DUMP KNOWN");
                    }
                    // Otherwise, write out the CRC and SHA-1 hashes
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(rom.CRC))
                            sw.Write($" CRC({rom.CRC ?? string.Empty})");
                        if (!string.IsNullOrWhiteSpace(rom.SHA1))
                            sw.Write($" SHA1({rom.SHA1 ?? string.Empty})");
                    }

                    // If we have a baddump, put the second indicator
                    if (rom.ItemStatus == ItemStatus.BadDump)
                        sw.Write(" BAD_DUMP");

                    sw.Write("\n");
                    break;
            }

            sw.Flush();
        }
    }
}
