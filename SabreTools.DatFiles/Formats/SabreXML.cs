using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using SabreTools.Core;
using SabreTools.DatItems;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing and writing of a SabreDAT XML
    /// </summary>
    internal class SabreXML : DatFile
    {
        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public SabreXML(DatFile? datFile)
            : base(datFile)
        {
        }

        /// <inheritdoc/>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            // Prepare all internal variables
            XmlReader? xtr = XmlReader.Create(filename, new XmlReaderSettings
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
                        case "header":
                            XmlSerializer xs = new(typeof(DatHeader));
                            DatHeader? header = xs.Deserialize(xtr.ReadSubtree()) as DatHeader;
                            Header.ConditionalCopy(header);
                            xtr.Skip();
                            break;

                        case "directory":
                            ReadDirectory(xtr.ReadSubtree(), statsOnly, filename, indexId);

                            // Skip the directory node now that we've processed it
                            xtr.Read();
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

            xtr?.Dispose();
        }

        /// <summary>
        /// Read directory information
        /// </summary>
        /// <param name="xtr">XmlReader to use to parse the header</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        private void ReadDirectory(XmlReader xtr, bool statsOnly, string filename, int indexId)
        {
            // If the reader is invalid, skip
            if (xtr == null)
                return;

            // Prepare internal variables
            Machine? machine = null;

            // Otherwise, read the directory
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
                    case "machine":
                        XmlSerializer xs = new(typeof(Machine));
                        machine = xs?.Deserialize(xtr.ReadSubtree()) as Machine;
                        xtr.Skip();
                        break;

                    case "files":
                        ReadFiles(xtr.ReadSubtree(), machine, statsOnly, filename, indexId);

                        // Skip the directory node now that we've processed it
                        xtr.Read();
                        break;
                    default:
                        xtr.Read();
                        break;
                }
            }
        }

        /// <summary>
        /// Read Files information
        /// </summary>
        /// <param name="xtr">XmlReader to use to parse the header</param>
        /// <param name="machine">Machine to copy information from</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        private void ReadFiles(XmlReader xtr, Machine? machine, bool statsOnly, string filename, int indexId)
        {
            // If the reader is invalid, skip
            if (xtr == null)
                return;

            // Otherwise, read the items
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
                    case "datitem":
                        XmlSerializer xs = new(typeof(DatItem));
                        if (xs.Deserialize(xtr.ReadSubtree()) is DatItem item)
                        {
                            item.CopyMachineInformation(machine);
                            item.Source = new Source { Name = filename, Index = indexId };
                            ParseAddHelper(item, statsOnly);
                        }
                        xtr.Skip();
                        break;
                    default:
                        xtr.Read();
                        break;
                }
            }
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

                XmlTextWriter xtw = new(fs, new UTF8Encoding(false))
                {
                    Formatting = Formatting.Indented,
                    IndentChar = '\t',
                    Indentation = 1,
                };

                // Write out the header
                WriteHeader(xtw);

                // Write out each of the machines and roms
                string? lastgame = null;

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

                        // If we have a different game and we're not at the start of the list, output the end of last item
                        if (lastgame != null && lastgame.ToLowerInvariant() != datItem.Machine.Name?.ToLowerInvariant())
                            WriteEndGame(xtw);

                        // If we have a new game, output the beginning of the new item
                        if (lastgame == null || lastgame.ToLowerInvariant() != datItem.Machine.Name?.ToLowerInvariant())
                            WriteStartGame(xtw, datItem);

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

            xtw.WriteStartElement("datafile");

            XmlSerializer xs = new(typeof(DatHeader));
            XmlSerializerNamespaces ns = new();
            ns.Add("", "");
            xs.Serialize(xtw, Header, ns);

            xtw.WriteStartElement("data");

            xtw.Flush();
        }

        /// <summary>
        /// Write out Game start using the supplied StreamWriter
        /// </summary>
        /// <param name="xtw">XmlTextWriter to output to</param>
        /// <param name="datItem">DatItem object to be output</param>
        private static void WriteStartGame(XmlTextWriter xtw, DatItem datItem)
        {
            // No game should start with a path separator
            datItem.Machine!.Name = datItem.Machine.Name?.TrimStart(Path.DirectorySeparatorChar) ?? string.Empty;

            // Write the machine
            xtw.WriteStartElement("directory");
            XmlSerializer xs = new(typeof(Machine));
            XmlSerializerNamespaces ns = new();
            ns.Add("", "");
            xs.Serialize(xtw, datItem.Machine, ns);

            xtw.WriteStartElement("files");

            xtw.Flush();
        }

        /// <summary>
        /// Write out Game start using the supplied StreamWriter
        /// </summary>
        /// <param name="xtw">XmlTextWriter to output to</param>
        private static void WriteEndGame(XmlTextWriter xtw)
        {
            // End files
            xtw.WriteEndElement();

            // End directory
            xtw.WriteEndElement();

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

            // Write the DatItem
            XmlSerializer xs = new(typeof(DatItem));
            XmlSerializerNamespaces ns = new();
            ns.Add("", "");
            xs.Serialize(xtw, datItem, ns);

            xtw.Flush();
        }

        /// <summary>
        /// Write out DAT footer using the supplied StreamWriter
        /// </summary>
        /// <param name="xtw">XmlTextWriter to output to</param>
        private static void WriteFooter(XmlTextWriter xtw)
        {
            // End files
            xtw.WriteEndElement();

            // End directory
            xtw.WriteEndElement();

            // End data
            xtw.WriteEndElement();

            // End datafile
            xtw.WriteEndElement();

            xtw.Flush();
        }
    }
}
