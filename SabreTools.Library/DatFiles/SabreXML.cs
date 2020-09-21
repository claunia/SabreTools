using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using SabreTools.Library.Data;
using SabreTools.Library.DatItems;
using SabreTools.Library.IO;

namespace SabreTools.Library.DatFiles
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
        public SabreXML(DatFile datFile)
            : base(datFile)
        {
        }

        /// <summary>
        /// Parse an SabreDat XML DAT and return all found directories and files within
        /// </summary>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        protected override void ParseFile(string filename, int indexId, bool keep, bool throwOnError = false)
        {
            // Prepare all internal variables
            XmlReader xtr = filename.GetXmlTextReader();

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
                            XmlSerializer xs = new XmlSerializer(typeof(DatHeader));
                            DatHeader header = xs.Deserialize(xtr.ReadSubtree()) as DatHeader;
                            Header.ConditionalCopy(header);
                            xtr.Skip();
                            break;

                        case "directory":
                            ReadDirectory(xtr.ReadSubtree(), filename, indexId);

                            // Skip the directory node now that we've processed it
                            xtr.Read();
                            break;
                        default:
                            xtr.Read();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Globals.Logger.Warning(ex, $"Exception found while parsing '{filename}'");
                if (throwOnError)
                {
                    xtr.Dispose();
                    throw ex;
                }
                
                // For XML errors, just skip the affected node
                xtr?.Read();
            }

            xtr.Dispose();
        }

        /// <summary>
        /// Read directory information
        /// </summary>
        /// <param name="xtr">XmlReader to use to parse the header</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        private void ReadDirectory(XmlReader xtr, string filename, int indexId)
        {
            // If the reader is invalid, skip
            if (xtr == null)
                return;

            // Prepare internal variables
            Machine machine = null;

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
                        XmlSerializer xs = new XmlSerializer(typeof(Machine));
                        machine = xs.Deserialize(xtr.ReadSubtree()) as Machine;
                        xtr.Skip();
                        break;

                    case "files":
                        ReadFiles(xtr.ReadSubtree(), machine, filename, indexId);

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
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        private void ReadFiles(XmlReader xtr, Machine machine, string filename, int indexId)
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
                        XmlSerializer xs = new XmlSerializer(typeof(DatItem));
                        DatItem item = xs.Deserialize(xtr.ReadSubtree()) as DatItem;
                        item.CopyMachineInformation(machine);
                        item.Source = new Source { Name = filename, Index = indexId };
                        ParseAddHelper(item);
                        xtr.Skip();
                        break;
                    default:
                        xtr.Read();
                        break;
                }
            }
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
                Globals.Logger.User($"Opening file for writing: {outfile}");
                FileStream fs = FileExtensions.TryCreate(outfile);

                // If we get back null for some reason, just log and return
                if (fs == null)
                {
                    Globals.Logger.Warning($"File '{outfile}' could not be created for writing! Please check to see if the file is writable");
                    return false;
                }

                XmlTextWriter xtw = new XmlTextWriter(fs, new UTF8Encoding(false))
                {
                    Formatting = Formatting.Indented,
                    IndentChar = '\t',
                    Indentation = 1,
                };

                // Write out the header
                WriteHeader(xtw);

                // Write out each of the machines and roms
                string lastgame = null;

                // Use a sorted list of games to output
                foreach (string key in Items.SortedKeys)
                {
                    List<DatItem> datItems = Items.FilteredItems(key);

                    // Resolve the names in the block
                    datItems = DatItem.ResolveNames(datItems);

                    for (int index = 0; index < datItems.Count; index++)
                    {
                        DatItem datItem = datItems[index];

                        // If we have a different game and we're not at the start of the list, output the end of last item
                        if (lastgame != null && lastgame.ToLowerInvariant() != datItem.Machine.Name.ToLowerInvariant())
                            WriteEndGame(xtw);

                        // If we have a new game, output the beginning of the new item
                        if (lastgame == null || lastgame.ToLowerInvariant() != datItem.Machine.Name.ToLowerInvariant())
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

                Globals.Logger.Verbose("File written!" + Environment.NewLine);
                xtw.Dispose();
                fs.Dispose();
            }
            catch (Exception ex)
            {
                Globals.Logger.Error(ex);
                if (throwOnError) throw ex;
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

            XmlSerializer xs = new XmlSerializer(typeof(DatHeader));
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
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
        private void WriteStartGame(XmlTextWriter xtw, DatItem datItem)
        {
            // No game should start with a path separator
            datItem.Machine.Name = datItem.Machine.Name?.TrimStart(Path.DirectorySeparatorChar) ?? string.Empty;

            // Write the machine
            xtw.WriteStartElement("directory");
            XmlSerializer xs = new XmlSerializer(typeof(Machine));
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            xs.Serialize(xtw, datItem.Machine, ns);

            xtw.WriteStartElement("files");

            xtw.Flush();
        }

        /// <summary>
        /// Write out Game start using the supplied StreamWriter
        /// </summary>
        /// <param name="xtw">XmlTextWriter to output to</param>
        private void WriteEndGame(XmlTextWriter xtw)
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
            XmlSerializer xs = new XmlSerializer(typeof(DatItem));
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            xs.Serialize(xtw, datItem, ns);

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
