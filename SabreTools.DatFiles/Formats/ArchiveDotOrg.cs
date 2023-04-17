using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.IO;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing and writing of a Archive.org file list
    /// </summary>
    internal class ArchiveDotOrg : DatFile
    {
        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public ArchiveDotOrg(DatFile datFile)
            : base(datFile)
        {
        }

        /// <inheritdoc/>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            // Prepare all internal variables
            XmlReader xtr = XmlReader.Create(filename, new XmlReaderSettings
            {
                CheckCharacters = false,
                DtdProcessing = DtdProcessing.Ignore,
                IgnoreComments = true,
                IgnoreWhitespace = true,
                ValidationFlags = XmlSchemaValidationFlags.None,
                ValidationType = ValidationType.None,
            });

            // If we got a null reader, just return
            if (xtr == null)
                return;

            // Otherwise, read the file to the end
            try
            {
                xtr.MoveToContent();
                while (!xtr.EOF)
                {
                    // We only want elements
                    if (xtr.NodeType != XmlNodeType.Element)
                    {
                        xtr.Read();
                        continue;
                    }

                    switch (xtr.Name)
                    {
                        case "files":
                            ReadFiles(xtr.ReadSubtree(), statsOnly, filename, indexId, keep);

                            // Skip the machine now that we've processed it
                            xtr.Skip();
                            break;

                        default:
                            xtr.Read();
                            break;
                    }
                }
            }
            catch (Exception ex) when (!throwOnError)
            {
                logger.Warning(ex, $"Exception found while parsing '{filename}'");

                // For XML errors, just skip the affected node
                xtr?.Read();
            }

            xtr.Dispose();
        }

        /// <summary>
        /// Read files information
        /// </summary>
        /// <param name="reader">XmlReader to use to parse the machine</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        private void ReadFiles(
            XmlReader reader,
            bool statsOnly,

            // Standard Dat parsing
            string filename,
            int indexId,

            // Miscellaneous
            bool keep)
        {
            // If we have an empty machine, skip it
            if (reader == null)
                return;

            // Otherwise, add what is possible
            reader.MoveToContent();

            while (!reader.EOF)
            {
                // We only want elements
                if (reader.NodeType != XmlNodeType.Element)
                {
                    reader.Read();
                    continue;
                }

                // Get the files from the list
                switch (reader.Name)
                {
                    case "file":
                        ReadFile(reader.ReadSubtree(), statsOnly, filename, indexId, keep);

                        // Skip the file node now that we've processed it
                        reader.Skip();
                        break;

                    default:
                        reader.Read();
                        break;
                }
            }
        }

        /// <summary>
        /// Read file information
        /// </summary>
        /// <param name="reader">XmlReader to use to parse the machine</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        private void ReadFile(
            XmlReader reader,
            bool statsOnly,

            // Standard Dat parsing
            string filename,
            int indexId,

            // Miscellaneous
            bool keep)
        {
            // If we have an empty machine, skip it
            if (reader == null)
                return;

            // Otherwise, add what is possible
            reader.MoveToContent();

            // Create the Rom to store the info
            Rom rom = new Rom
            {
                Name = reader.GetAttribute("name"),
                ArchiveDotOrgSource = reader.GetAttribute("source"),

                Machine = new Machine
                {
                    Name = "Default",
                    Description = "Default",
                },

                Source = new Source
                {
                    Index = indexId,
                    Name = filename,
                }
            };

            // If we have a path, update the machine name and description
            if (rom.Name.Contains('/'))
            {
                string[] splitpath = rom.Name.Split('/');
                rom.Machine.Name = splitpath[0];
                rom.Machine.Description = splitpath[0];

                rom.Name = rom.Name.Substring(splitpath[0].Length + 1);
            }

            // TODO: Handle SuperDAT
            //if (Header.Type == "SuperDAT" && !keep)
            //{
            //    string tempout = Regex.Match(machine.Name, @".*?\\(.*)").Groups[1].Value;
            //    if (!string.IsNullOrWhiteSpace(tempout))
            //        machine.Name = tempout;
            //}

            while (!reader.EOF)
            {
                // We only want elements
                if (reader.NodeType != XmlNodeType.Element)
                {
                    reader.Read();
                    continue;
                }

                // Get the roms from the machine
                switch (reader.Name)
                {
                    case "crc32":
                        rom.CRC = reader.ReadElementContentAsString();
                        break;

                    case "md5":
                        rom.MD5 = reader.ReadElementContentAsString();
                        break;

                    case "mtime":
                        rom.Date = reader.ReadElementContentAsString();
                        break;

                    case "sha1":
                        rom.SHA1 = reader.ReadElementContentAsString();
                        break;

                    case "size":
                        rom.Size = Utilities.CleanLong(reader.ReadElementContentAsString());
                        break;

                    case "format":
                        rom.ArchiveDotOrgFormat = reader.ReadElementContentAsString();
                        break;

                    case "original":
                        rom.OriginalFilename = reader.ReadElementContentAsString();
                        break;

                    case "rotation":
                        rom.Rotation = reader.ReadElementContentAsString();
                        break;

                    case "summation":
                        rom.Summation = reader.ReadElementContentAsString();
                        break;

                    default:
                        reader.Read();
                        break;
                }
            }

            // Now process and add the rom
            ParseAddHelper(rom, statsOnly);
        }

        /// <inheritdoc/>
        protected override ItemType[] GetSupportedTypes()
        {
            return new ItemType[]
            {
                ItemType.Rom,
            };
        }

        /// <inheritdoc/>
        protected override List<DatItemField> GetMissingRequiredFields(DatItem datItem)
        {
            // TODO: Check required fields
            return null;
        }

        /// <inheritdoc/>
        public override bool WriteToFile(string outfile, bool ignoreblanks = false, bool throwOnError = false)
        {
            try
            {
                logger.User($"Writing to '{outfile}'...");
                FileStream fs = System.IO.File.Create(outfile);

                // If we get back null for some reason, just log and return
                if (fs == null)
                {
                    logger.Warning($"File '{outfile}' could not be created for writing! Please check to see if the file is writable");
                    return false;
                }

                XmlTextWriter xtw = new XmlTextWriter(fs, new UTF8Encoding(false))
                {
                    Formatting = Formatting.Indented,
                    IndentChar = ' ',
                    Indentation = 2
                };

                // Write out the header
                WriteHeader(xtw);

                // Write out each of the machines and roms
                string lastgame = null;

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
                            WriteDatItem(xtw, datItem);

                        // Set the new data to compare against
                        lastgame = datItem.Machine.Name;
                    }
                }

                // Write the file footer out
                WriteFooter(xtw);

                logger.User($"'{outfile}' written!{Environment.NewLine}");
                xtw.Dispose();
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
        /// <param name="xtw">XmlTextWriter to output to</param>
        private void WriteHeader(XmlTextWriter xtw)
        {
            xtw.WriteStartDocument();

            xtw.WriteStartElement("files");

            xtw.Flush();
        }

        /// <summary>
        /// Write out DatItem using the supplied StreamWriter
        /// </summary>
        /// <param name="xtw">XmlTextWriter to output to</param>
        /// <param name="datItem">DatItem object to be output</param>
        private void WriteDatItem(XmlTextWriter xtw, DatItem datItem)
        {
            // Pre-process the item name
            ProcessItemName(datItem, true);

            // Build the state
            switch (datItem.ItemType)
            {
                case ItemType.Rom:
                    var rom = datItem as Rom;
                    xtw.WriteStartElement("file");

                    // Filename has to be derived from both machine and item, if possible
                    string filename = rom.Name;
                    if (!string.IsNullOrWhiteSpace(rom.Machine?.Name) && !rom.Machine.Name.Equals("Default", StringComparison.OrdinalIgnoreCase))
                        filename = $"{rom.Machine.Name}/{filename}";

                    xtw.WriteRequiredAttributeString("name", filename);
                    xtw.WriteOptionalAttributeString("source", rom.ArchiveDotOrgSource);

                    xtw.WriteOptionalElementString("mtime", rom.Date);
                    xtw.WriteOptionalElementString("size", rom.Size?.ToString());
                    xtw.WriteOptionalElementString("md5", rom.MD5?.ToLowerInvariant());
                    xtw.WriteOptionalElementString("crc32", rom.CRC?.ToLowerInvariant());
                    xtw.WriteOptionalElementString("sha1", rom.SHA1?.ToLowerInvariant());
                    xtw.WriteOptionalElementString("format", rom.ArchiveDotOrgFormat);
                    xtw.WriteOptionalElementString("original", rom.OriginalFilename);
                    xtw.WriteOptionalElementString("rotation", rom.Rotation?.ToString());
                    xtw.WriteOptionalElementString("summation", rom.Summation);

                    // End file
                    xtw.WriteEndElement();
                    break;
            }

            xtw.Flush();
        }

        /// <summary>
        /// Write out DAT footer using the supplied StreamWriter
        /// </summary>
        /// <param name="xtw">XmlTextWriter to output to</param>
        private void WriteFooter(XmlTextWriter xtw)
        {
            // End files
            xtw.WriteEndElement();

            xtw.Flush();
        }
    }
}
