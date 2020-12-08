using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using SabreTools.Core;
using SabreTools.DatItems;
using SabreTools.IO;

namespace SabreTools.Library.DatFiles
{
    /// <summary>
    /// Represents parsing and writing of a value-separated DAT
    /// </summary>
    internal class SeparatedValue : DatFile
    {
        // Private instance variables specific to Separated Value DATs
        private readonly char _delim;

        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        /// <param name="delim">Delimiter for parsing individual lines</param>
        public SeparatedValue(DatFile datFile, char delim)
            : base(datFile)
        {
            _delim = delim;
        }

        /// <summary>
        /// Parse a character-separated value DAT and return all found games and roms within
        /// </summary>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        protected override void ParseFile(string filename, int indexId, bool keep, bool throwOnError = false)
        {
            // Open a file reader
            Encoding enc = FileExtensions.GetEncoding(filename);
            SeparatedValueReader svr = new SeparatedValueReader(File.OpenRead(filename), enc)
            {
                Header = true,
                Quotes = true,
                Separator = _delim,
                VerifyFieldCount = true,
            };

            // If we're somehow at the end of the stream already, we can't do anything
            if (svr.EndOfStream)
                return;

            // Read in the header
            svr.ReadHeader();

            // Loop through all of the data lines
            while (!svr.EndOfStream)
            {
                try
                {
                    // Get the current line, split and parse
                    svr.ReadNextLine();

                    // Create mapping dictionary
                    var mappings = new Dictionary<Field, string>();

                    // Now we loop through and get values for everything
                    for (int i = 0; i < svr.HeaderValues.Count; i++)
                    {
                        Field key = svr.HeaderValues[i].AsField();
                        string value = svr.Line[i];
                        mappings[key] = value;
                    }

                    // Set DatHeader fields
                    DatHeader header = new DatHeader();
                    header.SetFields(mappings);
                    Header.ConditionalCopy(header);

                    // Set Machine and DatItem fields
                    if (mappings.ContainsKey(Field.DatItem_Type))
                    {
                        DatItem datItem = DatItem.Create(mappings[Field.DatItem_Type].AsItemType());
                        datItem.SetFields(mappings);
                        datItem.Source = new Source(indexId, filename);
                        ParseAddHelper(datItem);
                    }
                }
                catch (Exception ex)
                {
                    string message = $"'{filename}' - There was an error parsing line {svr.LineNumber} '{svr.CurrentLine}'";
                    logger.Error(ex, message);
                    if (throwOnError)
                    {
                        svr.Dispose();
                        throw new Exception(message, ex);
                    }
                }
            }

            svr.Dispose();
        }

        /// <inheritdoc/>
        protected override ItemType[] GetSupportedTypes()
        {
            return new ItemType[] { ItemType.Disk, ItemType.Media, ItemType.Rom };
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
                FileStream fs = File.Create(outfile);

                // If we get back null for some reason, just log and return
                if (fs == null)
                {
                    logger.Warning($"File '{outfile}' could not be created for writing! Please check to see if the file is writable");
                    return false;
                }

                SeparatedValueWriter svw = new SeparatedValueWriter(fs, new UTF8Encoding(false))
                {
                    Quotes = true,
                    Separator = this._delim,
                    VerifyFieldCount = true
                };

                // Write out the header
                WriteHeader(svw);

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
                            WriteDatItem(svw, datItem);
                    }
                }

                logger.Verbose("File written!" + Environment.NewLine);
                svw.Dispose();
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
        /// <param name="svw">SeparatedValueWriter to output to</param>
        private void WriteHeader(SeparatedValueWriter svw)
        {
            string[] headers = new string[]
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
                    //"RIPEMD160",
                    "SHA1",
                    "SHA256",
                    //"SHA384",
                    //"SHA512",
                    //"SpamSum",
                    "Nodump",
                            };

            svw.WriteHeader(headers);

            svw.Flush();
        }

        /// <summary>
        /// Write out DatItem using the supplied StreamWriter
        /// </summary>
        /// <param name="svw">SeparatedValueWriter to output to</param>
        /// <param name="datItem">DatItem object to be output</param>
        private void WriteDatItem(SeparatedValueWriter svw, DatItem datItem)
        {
            // Separated values should only output Rom and Disk
            if (datItem.ItemType != ItemType.Disk && datItem.ItemType != ItemType.Rom)
                return;

            // Build the state
            // TODO: Can we have some way of saying what fields to write out? Support for read extends to all fields now
            string[] fields = new string[14]; // 18;
            fields[0] = Header.FileName;
            fields[1] = Header.Name;
            fields[2] = Header.Description;
            fields[3] = datItem.Machine.Name;
            fields[4] = datItem.Machine.Description;

            switch (datItem.ItemType)
            {
                case ItemType.Disk:
                    var disk = datItem as Disk;
                    fields[5] = "disk";
                    fields[6] = string.Empty;
                    fields[7] = disk.Name;
                    fields[8] = string.Empty;
                    fields[9] = string.Empty;
                    fields[10] = disk.MD5?.ToLowerInvariant();
                    //fields[11] = string.Empty;
                    fields[11] = disk.SHA1?.ToLowerInvariant();
                    fields[12] = string.Empty;
                    //fields[13] = string.Empty;
                    //fields[14] = string.Empty;
                    //fields[15] = string.Empty;
                    fields[13] = disk.ItemStatus.ToString();
                    break;

                case ItemType.Media:
                    var media = datItem as Media;
                    fields[5] = "media";
                    fields[6] = string.Empty;
                    fields[7] = media.Name;
                    fields[8] = string.Empty;
                    fields[9] = string.Empty;
                    fields[10] = media.MD5?.ToLowerInvariant();
                    //fields[11] = string.Empty;
                    fields[11] = media.SHA1?.ToLowerInvariant();
                    fields[12] = media.SHA256?.ToLowerInvariant();
                    //fields[13] = string.Empty;
                    //fields[14] = string.Empty;
                    //fields[15] = media.SpamSum?.ToLowerInvariant();
                    fields[13] = string.Empty;
                    break;

                case ItemType.Rom:
                    var rom = datItem as Rom;
                    fields[5] = "rom";
                    fields[6] = rom.Name;
                    fields[7] = string.Empty;
                    fields[8] = rom.Size?.ToString();
                    fields[9] = rom.CRC?.ToLowerInvariant();
                    fields[10] = rom.MD5?.ToLowerInvariant();
                    //fields[11] = rom.RIPEMD160?.ToLowerInvariant();
                    fields[11] = rom.SHA1?.ToLowerInvariant();
                    fields[12] = rom.SHA256?.ToLowerInvariant();
                    //fields[13] = rom.SHA384?.ToLowerInvariant();
                    //fields[14] = rom.SHA512?.ToLowerInvariant();
                    //fields[15] = rom.SpamSum?.ToLowerInvariant();
                    fields[13] = rom.ItemStatus.ToString();
                    break;
            }

            svw.WriteString(CreatePrefixPostfix(datItem, true));
            svw.WriteValues(fields, false);
            svw.WriteString(CreatePrefixPostfix(datItem, false));
            svw.WriteLine();

            svw.Flush();
        }
    }
}
