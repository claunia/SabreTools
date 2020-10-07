using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using SabreTools.Library.DatItems;
using SabreTools.Library.IO;
using SabreTools.Library.Tools;

namespace SabreTools.Library.DatFiles
{
    /// <summary>
    /// Represents parsing and writing of a DosCenter DAT
    /// </summary>
    internal class DosCenter : DatFile
    {
        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public DosCenter(DatFile datFile)
            : base(datFile)
        {
        }

        /// <summary>
        /// Parse a DOSCenter DAT and return all found games and roms within
        /// </summary>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        protected override void ParseFile(string filename, int indexId, bool keep, bool throwOnError = false)
        {
            // Open a file reader
            Encoding enc = FileExtensions.GetEncoding(filename);
            ClrMameProReader cmpr = new ClrMameProReader(FileExtensions.TryOpenRead(filename), enc)
            {
                DosCenter = true
            };

            while (!cmpr.EndOfStream)
            {
                try
                {
                    cmpr.ReadNextLine();

                    // Ignore everything not top-level
                    if (cmpr.RowType != CmpRowType.TopLevel)
                        continue;

                    // Switch on the top-level name
                    switch (cmpr.TopLevel.ToLowerInvariant())
                    {
                        // Header values
                        case "doscenter":
                            ReadHeader(cmpr);
                            break;

                        // Sets
                        case "game":
                            ReadGame(cmpr, filename, indexId);
                            break;

                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    string message = $"'{filename}' - There was an error parsing line {cmpr.LineNumber} '{cmpr.CurrentLine}'";
                    logger.Error(ex, message);
                    if (throwOnError)
                    {
                        cmpr.Dispose();
                        throw new Exception(message, ex);
                    }
                }
            }

            cmpr.Dispose();
        }

        /// <summary>
        /// Read header information
        /// </summary>
        /// <param name="cmpr">ClrMameProReader to use to parse the header</param>
        private void ReadHeader(ClrMameProReader cmpr)
        {
            // If there's no subtree to the header, skip it
            if (cmpr == null || cmpr.EndOfStream)
                return;

            // While we don't hit an end element or end of stream
            while (!cmpr.EndOfStream)
            {
                cmpr.ReadNextLine();

                // Ignore comments, internal items, and nothingness
                if (cmpr.RowType == CmpRowType.None || cmpr.RowType == CmpRowType.Comment || cmpr.RowType == CmpRowType.Internal)
                    continue;

                // If we reached the end of a section, break
                if (cmpr.RowType == CmpRowType.EndTopLevel)
                    break;

                // If the standalone value is null, we skip
                if (cmpr.Standalone == null)
                    continue;

                string itemKey = cmpr.Standalone?.Key.ToLowerInvariant().TrimEnd(':');
                string itemVal = cmpr.Standalone?.Value;

                // For all other cases
                switch (itemKey)
                {
                    case "name":
                        Header.Name = (Header.Name == null ? itemVal : Header.Name);
                        break;
                    case "description":
                        Header.Description = (Header.Description == null ? itemVal : Header.Description);
                        break;
                    case "dersion":
                        Header.Version = (Header.Version == null ? itemVal : Header.Version);
                        break;
                    case "date":
                        Header.Date = (Header.Date == null ? itemVal : Header.Date);
                        break;
                    case "author":
                        Header.Author = (Header.Author == null ? itemVal : Header.Author);
                        break;
                    case "homepage":
                        Header.Homepage = (Header.Homepage == null ? itemVal : Header.Homepage);
                        break;
                    case "comment":
                        Header.Comment = (Header.Comment == null ? itemVal : Header.Comment);
                        break;
                }
            }
        }

        /// <summary>
        /// Read set information
        /// </summary>
        /// <param name="cmpr">ClrMameProReader to use to parse the header</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        private void ReadGame(ClrMameProReader cmpr, string filename, int indexId)
        {
            // Prepare all internal variables
            bool containsItems = false;
            Machine machine = new Machine()
            {
                MachineType = MachineType.NULL,
            };

            // If there's no subtree to the header, skip it
            if (cmpr == null || cmpr.EndOfStream)
                return;

            // While we don't hit an end element or end of stream
            while (!cmpr.EndOfStream)
            {
                cmpr.ReadNextLine();

                // Ignore comments and nothingness
                if (cmpr.RowType == CmpRowType.None || cmpr.RowType == CmpRowType.Comment)
                    continue;

                // If we reached the end of a section, break
                if (cmpr.RowType == CmpRowType.EndTopLevel)
                    break;

                // Handle any standalone items
                if (cmpr.RowType == CmpRowType.Standalone && cmpr.Standalone != null)
                {
                    string itemKey = cmpr.Standalone?.Key.ToLowerInvariant();
                    string itemVal = cmpr.Standalone?.Value;

                    switch (itemKey)
                    {
                        case "name":
                            machine.Name = (itemVal.ToLowerInvariant().EndsWith(".zip") ? itemVal.Remove(itemVal.Length - 4) : itemVal);
                            machine.Description = (itemVal.ToLowerInvariant().EndsWith(".zip") ? itemVal.Remove(itemVal.Length - 4) : itemVal);
                            break;
                    }
                }

                // Handle any internal items
                else if (cmpr.RowType == CmpRowType.Internal
                    && string.Equals(cmpr.InternalName, "file", StringComparison.OrdinalIgnoreCase)
                    && cmpr.Internal != null)
                {
                    containsItems = true;

                    // Create the proper DatItem based on the type
                    Rom item = DatItem.Create(ItemType.Rom) as Rom;

                    // Then populate it with information
                    item.CopyMachineInformation(machine);
                    item.Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    };

                    // Loop through all of the attributes
                    foreach (var kvp in cmpr.Internal)
                    {
                        string attrKey = kvp.Key;
                        string attrVal = kvp.Value;

                        switch (attrKey)
                        {
                            //If the item is empty, we automatically skip it because it's a fluke
                            case "":
                                continue;

                            // Regular attributes
                            case "name":
                                item.Name = attrVal;
                                break;

                            case "size":
                                item.Size = Sanitizer.CleanLong(attrVal);
                                break;

                            case "crc":
                                item.CRC = attrVal;
                                break;
                            case "date":
                                item.Date = attrVal;
                                break;
                        }
                    }

                    // Now process and add the rom
                    ParseAddHelper(item);
                }
            }

            // If no items were found for this machine, add a Blank placeholder
            if (!containsItems)
            {
                Blank blank = new Blank()
                {
                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

                blank.CopyMachineInformation(machine);

                // Now process and add the rom
                ParseAddHelper(blank);
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

                ClrMameProWriter cmpw = new ClrMameProWriter(fs, new UTF8Encoding(false))
                {
                    Quotes = false
                };

                // Write out the header
                WriteHeader(cmpw);

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

                        List<string> newsplit = datItem.Machine.Name.Split('\\').ToList();

                        // If we have a different game and we're not at the start of the list, output the end of last item
                        if (lastgame != null && lastgame.ToLowerInvariant() != datItem.Machine.Name.ToLowerInvariant())
                            WriteEndGame(cmpw);

                        // If we have a new game, output the beginning of the new item
                        if (lastgame == null || lastgame.ToLowerInvariant() != datItem.Machine.Name.ToLowerInvariant())
                            WriteStartGame(cmpw, datItem);

                        // Check for a "null" item
                        datItem = ProcessNullifiedItem(datItem);

                        // Write out the item if we're not ignoring
                        if (!ShouldIgnore(datItem, ignoreblanks))
                            WriteDatItem(cmpw, datItem);

                        // Set the new data to compare against
                        lastgame = datItem.Machine.Name;
                    }
                }

                // Write the file footer out
                WriteFooter(cmpw);

                logger.Verbose($"File written!{Environment.NewLine}");
                cmpw.Dispose();
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
        /// <param name="cmpw">ClrMameProWriter to output to</param>
        private void WriteHeader(ClrMameProWriter cmpw)
        {
            // Build the state
            cmpw.WriteStartElement("DOSCenter");

            cmpw.WriteRequiredStandalone("Name:", Header.Name, false);
            cmpw.WriteRequiredStandalone("Description:", Header.Description, false);
            cmpw.WriteRequiredStandalone("Version:", Header.Version, false);
            cmpw.WriteRequiredStandalone("Date:", Header.Date, false);
            cmpw.WriteRequiredStandalone("Author:", Header.Author, false);
            cmpw.WriteRequiredStandalone("Homepage:", Header.Homepage, false);
            cmpw.WriteRequiredStandalone("Comment:", Header.Comment, false);

            cmpw.WriteEndElement();

            cmpw.Flush();
        }

        /// <summary>
        /// Write out Game start using the supplied StreamWriter
        /// </summary>
        /// <param name="cmpw">ClrMameProWriter to output to</param>
        /// <param name="datItem">DatItem object to be output</param>
        private void WriteStartGame(ClrMameProWriter cmpw, DatItem datItem)
        {
            // No game should start with a path separator
            datItem.Machine.Name = datItem.Machine.Name.TrimStart(Path.DirectorySeparatorChar);

            // Build the state
            cmpw.WriteStartElement("game");
            cmpw.WriteRequiredStandalone("name", $"{datItem.Machine.Name}.zip", true);

            cmpw.Flush();
        }

        /// <summary>
        /// Write out Game end using the supplied StreamWriter
        /// </summary>
        /// <param name="cmpw">ClrMameProWriter to output to</param>
        private void WriteEndGame(ClrMameProWriter cmpw)
        {
            // End game
            cmpw.WriteEndElement();

            cmpw.Flush();
        }

        /// <summary>
        /// Write out DatItem using the supplied StreamWriter
        /// </summary>
        /// <param name="cmpw">ClrMameProWriter to output to</param>
        /// <param name="datItem">DatItem object to be output</param>
        private void WriteDatItem(ClrMameProWriter cmpw, DatItem datItem)
        {
            // Pre-process the item name
            ProcessItemName(datItem, true);

            // Build the state
            switch (datItem.ItemType)
            {
                case ItemType.Rom:
                    var rom = datItem as Rom;
                    cmpw.WriteStartElement("file");
                    cmpw.WriteRequiredAttributeString("name", rom.Name);
                    cmpw.WriteOptionalAttributeString("size", rom.Size?.ToString());
                    cmpw.WriteOptionalAttributeString("date", rom.Date);
                    cmpw.WriteOptionalAttributeString("crc", rom.CRC?.ToLowerInvariant());
                    cmpw.WriteEndElement();
                    break;
            }

            cmpw.Flush();
        }

        /// <summary>
        /// Write out DAT footer using the supplied StreamWriter
        /// </summary>
        /// <param name="cmpw">ClrMameProWriter to output to</param>
        /// <returns>True if the data was written, false on error</returns>
        private void WriteFooter(ClrMameProWriter cmpw)
        {
            // End game
            cmpw.WriteEndElement();

            cmpw.Flush();
        }
    }
}
