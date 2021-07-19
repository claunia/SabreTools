using System;
using System.IO;
using System.Text;

using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.IO;
using SabreTools.IO.Readers;
using SabreTools.IO.Writers;

namespace SabreTools.DatFiles.Formats
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

        /// <inheritdoc/>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            // Open a file reader
            Encoding enc = filename.GetEncoding();
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

                    // Create mapping dictionaries
                    Setter setter = new Setter();
                    setter.PopulateSettersFromList(svr.HeaderValues, svr.Line);

                    // Set DatHeader fields
                    DatHeader datHeader = new DatHeader();
                    setter.SetFields(datHeader);
                    Header.ConditionalCopy(datHeader);

                    // Set Machine and DatItem fields
                    if (setter.DatItemMappings.ContainsKey(DatItemField.Type))
                    {
                        DatItem datItem = DatItem.Create(setter.DatItemMappings[DatItemField.Type].AsItemType());
                        setter.SetFields(datItem);
                        datItem.Machine = new Machine();
                        setter.SetFields(datItem.Machine);
                        datItem.Source = new Source(indexId, filename);
                        ParseAddHelper(datItem, statsOnly);
                    }
                }
                catch (Exception ex) when (!throwOnError)
                {
                    string message = $"'{filename}' - There was an error parsing line {svr.LineNumber} '{svr.CurrentLine}'";
                    logger.Error(ex, message);
                }
            }

            svr.Dispose();
        }

        /// <inheritdoc/>
        protected override ItemType[] GetSupportedTypes()
        {
            return new ItemType[] { ItemType.Disk, ItemType.Media, ItemType.Rom };
        }

        /// <inheritdoc/>
        public override bool WriteToFile(string outfile, bool ignoreblanks = false, bool throwOnError = false)
        {
            try
            {
                logger.User($"Writing to '{outfile}'...");
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
                    ConcurrentList<DatItem> datItems = Items.FilteredItems(key);

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

                logger.User($"'{outfile}' written!{Environment.NewLine}");
                svw.Dispose();
                fs.Dispose();
            }
            catch (Exception ex) when (!throwOnError)
            {
                logger.Error(ex);
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
