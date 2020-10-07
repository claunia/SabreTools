using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using SabreTools.Library.DatItems;
using SabreTools.Library.IO;
using SabreTools.Library.Tools;

namespace SabreTools.Library.DatFiles
{
    /// <summary>
    /// Represents parsing and writing of a RomCenter DAT
    /// </summary>
    internal class RomCenter : DatFile
    {
        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public RomCenter(DatFile datFile)
            : base(datFile)
        {
        }

        /// <summary>
        /// Parse a RomCenter DAT and return all found games and roms within
        /// </summary>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        protected override void ParseFile(string filename, int indexId, bool keep, bool throwOnError = false)
        {
            // Prepare all intenral variables
            IniReader ir = filename.GetIniReader(false);

            // If we got a null reader, just return
            if (ir == null)
                return;

            // Otherwise, read the file to the end
            try
            {
                ir.ReadNextLine();
                while (!ir.EndOfStream)
                {
                    // We don't care about whitespace or comments
                    if (ir.RowType == IniRowType.None || ir.RowType == IniRowType.Comment)
                    {
                        ir.ReadNextLine();
                        continue;
                    }

                    // If we have a section
                    if (ir.RowType == IniRowType.SectionHeader)
                    {
                        switch (ir.Section.ToLowerInvariant())
                        {
                            case "credits":
                                ReadCreditsSection(ir);
                                break;

                            case "dat":
                                ReadDatSection(ir);
                                break;

                            case "emulator":
                                ReadEmulatorSection(ir);
                                break;

                            case "games":
                                ReadGamesSection(ir, filename, indexId);
                                break;

                            // Unknown section so we ignore it
                            default:
                                ir.ReadNextLine();
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string message = $"'{filename}' - There was an error parsing line {ir.LineNumber} '{ir.CurrentLine}'";
                logger.Error(ex, message);
                if (throwOnError)
                {
                    ir.Dispose();
                    throw new Exception(message, ex);
                }
            }

            ir.Dispose();
        }

        /// <summary>
        /// Read credits information
        /// </summary>
        /// <param name="reader">IniReader to use to parse the credits</param>
        private void ReadCreditsSection(IniReader reader)
        {
            // If the reader is somehow null, skip it
            if (reader == null)
                return;

            reader.ReadNextLine();
            while (!reader.EndOfStream && reader.Section.ToLowerInvariant() == "credits")
            {
                // We don't care about whitespace, comments, or invalid
                if (reader.RowType != IniRowType.KeyValue)
                {
                    reader.ReadNextLine();
                    continue;
                }

                var kvp = reader.KeyValuePair;

                // If the KeyValuePair is invalid, skip it
                if (kvp == null)
                {
                    reader.ReadNextLine();
                    continue;
                }

                // Get all credits items
                switch (kvp?.Key.ToLowerInvariant())
                {
                    case "author":
                        Header.Author = Header.Author ?? kvp?.Value;
                        reader.ReadNextLine();
                        break;

                    case "version":
                        Header.Version = Header.Version ?? kvp?.Value;
                        reader.ReadNextLine();
                        break;

                    case "email":
                        Header.Email = Header.Email ?? kvp?.Value;
                        reader.ReadNextLine();
                        break;

                    case "homepage":
                        Header.Homepage = Header.Homepage ?? kvp?.Value;
                        reader.ReadNextLine();
                        break;

                    case "url":
                        Header.Url = Header.Url ?? kvp?.Value;
                        reader.ReadNextLine();
                        break;

                    case "date":
                        Header.Date = Header.Date ?? kvp?.Value;
                        reader.ReadNextLine();
                        break;

                    case "comment":
                        Header.Comment = Header.Comment ?? kvp?.Value;
                        reader.ReadNextLine();
                        break;

                    // Unknown value, just skip
                    default:
                        reader.ReadNextLine();
                        break;
                }
            }
        }

        /// <summary>
        /// Read dat information
        /// </summary>
        /// <param name="reader">IniReader to use to parse the credits</param>
        private void ReadDatSection(IniReader reader)
        {
            // If the reader is somehow null, skip it
            if (reader == null)
                return;

            reader.ReadNextLine();
            while (!reader.EndOfStream && reader.Section.ToLowerInvariant() == "dat")
            {
                // We don't care about whitespace, comments, or invalid
                if (reader.RowType != IniRowType.KeyValue)
                {
                    reader.ReadNextLine();
                    continue;
                }

                var kvp = reader.KeyValuePair;

                // If the KeyValuePair is invalid, skip it
                if (kvp == null)
                {
                    reader.ReadNextLine();
                    continue;
                }

                // Get all dat items
                switch (kvp?.Key.ToLowerInvariant())
                {
                    case "version":
                        Header.RomCenterVersion = Header.RomCenterVersion ?? (kvp?.Value);
                        reader.ReadNextLine();
                        break;

                    case "plugin":
                        Header.System = (Header.System == null ? kvp?.Value : Header.System);
                        reader.ReadNextLine();
                        break;

                    case "split":
                        if (Header.ForceMerging == MergingFlag.None && kvp?.Value == "1")
                            Header.ForceMerging = MergingFlag.Split;

                        reader.ReadNextLine();
                        break;

                    case "merge":
                        if (Header.ForceMerging == MergingFlag.None && kvp?.Value == "1")
                            Header.ForceMerging = MergingFlag.Merged;

                        reader.ReadNextLine();
                        break;

                    // Unknown value, just skip
                    default:
                        reader.ReadNextLine();
                        break;
                }
            }
        }

        /// <summary>
        /// Read emulator information
        /// </summary>
        /// <param name="reader">IniReader to use to parse the credits</param>
        private void ReadEmulatorSection(IniReader reader)
        {
            // If the reader is somehow null, skip it
            if (reader == null)
                return;

            reader.ReadNextLine();
            while (!reader.EndOfStream && reader.Section.ToLowerInvariant() == "emulator")
            {
                // We don't care about whitespace, comments, or invalid
                if (reader.RowType != IniRowType.KeyValue)
                {
                    reader.ReadNextLine();
                    continue;
                }

                var kvp = reader.KeyValuePair;

                // If the KeyValuePair is invalid, skip it
                if (kvp == null)
                {
                    reader.ReadNextLine();
                    continue;
                }

                // Get all emulator items (ONLY OVERWRITE IF THERE'S NO DATA)
                switch (kvp?.Key.ToLowerInvariant())
                {
                    case "refname":
                        Header.Name = Header.Name == null ? kvp?.Value : Header.Name;
                        reader.ReadNextLine();
                        break;

                    case "version":
                        Header.Description = Header.Description == null ? kvp?.Value : Header.Description;
                        reader.ReadNextLine();
                        break;

                    // Unknown value, just skip
                    default:
                        reader.ReadNextLine();
                        break;
                }
            }
        }

        /// <summary>
        /// Read games information
        /// </summary>
        /// <param name="reader">IniReader to use to parse the credits</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        private void ReadGamesSection(IniReader reader, string filename, int indexId)
        {
            // If the reader is somehow null, skip it
            if (reader == null)
                return;

            reader.ReadNextLine();
            while (!reader.EndOfStream && reader.Section.ToLowerInvariant() == "games")
            {
                // We don't care about whitespace or comments
                // We're keeping keyvalue in case a file has '=' in the row
                if (reader.RowType != IniRowType.Invalid && reader.RowType != IniRowType.KeyValue)
                {
                    reader.ReadNextLine();
                    continue;
                }

                // Roms are not valid row formats, usually
                string line = reader.CurrentLine;

                // If we don't have a valid game, keep reading
                if (!line.StartsWith("¬"))
                {
                    reader.ReadNextLine();
                    continue;
                }

                // Some old RC DATs have this behavior
                if (line.Contains("¬N¬O"))
                    line = line.Replace("¬N¬O", string.Empty) + "¬¬";

                /*
                The rominfo order is as follows:
                1 - parent name
                2 - parent description
                3 - game name
                4 - game description
                5 - rom name
                6 - rom crc
                7 - rom size
                8 - romof name
                9 - merge name
                */
                string[] rominfo = line.Split('¬');
                Rom rom = new Rom
                {
                    Name = rominfo[5],
                    Size = Sanitizer.CleanLong(rominfo[7]),
                    CRC = rominfo[6],
                    ItemStatus = ItemStatus.None,

                    Machine = new Machine
                    {
                        Name = rominfo[3],
                        Description = rominfo[4],
                        CloneOf = rominfo[1],
                        RomOf = rominfo[8],
                    },

                    MergeTag = rominfo[9],

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

                // Now process and add the rom
                ParseAddHelper(rom);

                reader.ReadNextLine();
            }
        }

        /// <inheritdoc/>
        protected override ItemType[] GetSupportedTypes()
        {
            return new ItemType[] { ItemType.Rom };
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

                IniWriter iw = new IniWriter(fs, new UTF8Encoding(false));

                // Write out the header
                WriteHeader(iw);

                // Write out each of the machines and roms
                string lastgame = null;
                List<string> splitpath = new List<string>();

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

                        // Check for a "null" item
                        datItem = ProcessNullifiedItem(datItem);

                        // Write out the item if we're not ignoring
                        if (!ShouldIgnore(datItem, ignoreblanks))
                            WriteDatItem(iw, datItem);

                        // Set the new data to compare against
                        lastgame = datItem.Machine.Name;
                    }
                }

                logger.Verbose("File written!" + Environment.NewLine);
                iw.Dispose();
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
        /// Write out DAT header using the supplied StreamWriter
        /// </summary>
        /// <param name="iw">IniWriter to output to</param>
        private void WriteHeader(IniWriter iw)
        {
            iw.WriteSection("CREDITS");
            iw.WriteKeyValuePair("author", Header.Author);
            iw.WriteKeyValuePair("version", Header.Version);
            iw.WriteKeyValuePair("comment", Header.Comment);

            iw.WriteSection("DAT");
            iw.WriteKeyValuePair("version", Header.RomCenterVersion ?? "2.50");
            iw.WriteKeyValuePair("plugin", Header.System);
            iw.WriteKeyValuePair("split", Header.ForceMerging == MergingFlag.Split ? "1" : "0");
            iw.WriteKeyValuePair("merge", Header.ForceMerging == MergingFlag.Full || Header.ForceMerging == MergingFlag.Merged ? "1" : "0");

            iw.WriteSection("EMULATOR");
            iw.WriteKeyValuePair("refname", Header.Name);
            iw.WriteKeyValuePair("version", Header.Description);

            iw.WriteSection("GAMES");

            iw.Flush();
        }

        /// <summary>
        /// Write out DatItem using the supplied StreamWriter
        /// </summary>
        /// <param name="iw">IniWriter to output to</param>
        /// <param name="datItem">DatItem object to be output</param>
        private void WriteDatItem(IniWriter iw, DatItem datItem)
        {
            /*
            The rominfo order is as follows:
            1 - parent name
            2 - parent description
            3 - game name
            4 - game description
            5 - rom name
            6 - rom crc
            7 - rom size
            8 - romof name
            9 - merge name
            */

            // Pre-process the item name
            ProcessItemName(datItem, true);

            // Build the state
            switch (datItem.ItemType)
            {
                case ItemType.Rom:
                    var rom = datItem as Rom;

                    iw.WriteString($"¬{rom.Machine.CloneOf ?? string.Empty}");
                    iw.WriteString($"¬{rom.Machine.CloneOf ?? string.Empty}");
                    iw.WriteString($"¬{rom.Machine.Name ?? string.Empty}");
                    if (string.IsNullOrWhiteSpace(rom.Machine.Description ?? string.Empty))
                        iw.WriteString($"¬{rom.Machine.Name ?? string.Empty}");
                    else
                        iw.WriteString($"¬{rom.Machine.Description ?? string.Empty}");
                    iw.WriteString($"¬{rom.Name ?? string.Empty}");
                    iw.WriteString($"¬{rom.CRC ?? string.Empty}");
                    iw.WriteString($"¬{rom.Size?.ToString() ?? string.Empty}");
                    iw.WriteString($"¬{rom.Machine.RomOf ?? string.Empty}");
                    iw.WriteString($"¬{rom.MergeTag ?? string.Empty}");
                    iw.WriteString("¬");
                    iw.WriteLine();

                    break;
            }

            iw.Flush();
        }
    }
}
