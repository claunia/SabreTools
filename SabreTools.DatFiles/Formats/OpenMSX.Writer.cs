using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.IO;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents writing an openMSX softawre list XML DAT
    /// </summary>
    internal partial class OpenMSX : DatFile
    {
        /// <inheritdoc/>
        protected override ItemType[] GetSupportedTypes()
        {
            return new ItemType[]
            {
                ItemType.Rom
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

                XmlTextWriter xtw = new(fs, new UTF8Encoding(false))
                {
                    Formatting = Formatting.Indented,
                    IndentChar = '\t',
                    Indentation = 1
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
            xtw.WriteDocType("softwaredb", null, "softwaredb1.dtd", null);

            xtw.WriteStartElement("softwaredb");
            xtw.WriteRequiredAttributeString("timestamp", Header.Date);

            //TODO: Figure out how to fix the issue with removed formatting after this point
//                xtw.WriteComment("Credits");
//                xtw.WriteCData(@"The softwaredb.xml file contains information about rom mapper types

//-Copyright 2003 Nicolas Beyaert(Initial Database)
//-Copyright 2004 - 2013 BlueMSX Team
//-Copyright 2005 - 2020 openMSX Team
//-Generation MSXIDs by www.generation - msx.nl

//- Thanks go out to:
//-Generation MSX / Sylvester for the incredible source of information
//- p_gimeno and diedel for their help adding and valdiating ROM additions
//- GDX for additional ROM info and validations and corrections");

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
            datItem.Machine.Name = datItem.Machine.Name.TrimStart(Path.DirectorySeparatorChar);

            // Build the state
            xtw.WriteStartElement("software");
            xtw.WriteRequiredElementString("title", datItem.Machine.Name);
            xtw.WriteRequiredElementString("genmsxid", datItem.Machine.GenMSXID);
            xtw.WriteRequiredElementString("system", datItem.Machine.System);
            xtw.WriteRequiredElementString("company", datItem.Machine.Manufacturer);
            xtw.WriteRequiredElementString("year", datItem.Machine.Year);
            xtw.WriteRequiredElementString("country", datItem.Machine.Country);

            xtw.Flush();
        }

        /// <summary>
        /// Write out Game start using the supplied StreamWriter
        /// </summary>
        /// <param name="xtw">XmlTextWriter to output to</param>
        private void WriteEndGame(XmlTextWriter xtw)
        {
            // End software
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

            // Build the state
            switch (datItem.ItemType)
            {
                case ItemType.Rom:
                    var rom = datItem as Rom;
                    xtw.WriteStartElement("dump");

                    if (rom.Original != null)
                    {
                        xtw.WriteStartElement("original");
                        xtw.WriteAttributeString("value", rom.Original.Value == true ? "true" : "false");
                        xtw.WriteString(rom.Original.Content);
                        xtw.WriteEndElement();
                    }

                    switch (rom.OpenMSXSubType)
                    {
                        // Default to Rom for converting from other formats
                        case OpenMSXSubType.Rom:
                        case OpenMSXSubType.NULL:
                            xtw.WriteStartElement(rom.OpenMSXSubType.FromOpenMSXSubType());
                            xtw.WriteRequiredElementString("hash", rom.SHA1?.ToLowerInvariant());
                            xtw.WriteOptionalElementString("start", rom.Offset);
                            xtw.WriteOptionalElementString("type", rom.OpenMSXType);
                            xtw.WriteOptionalElementString("remark", rom.Remark);
                            xtw.WriteEndElement();
                            break;

                        case OpenMSXSubType.MegaRom:
                            xtw.WriteStartElement(rom.OpenMSXSubType.FromOpenMSXSubType());
                            xtw.WriteRequiredElementString("hash", rom.SHA1?.ToLowerInvariant());
                            xtw.WriteOptionalElementString("start", rom.Offset);
                            xtw.WriteOptionalElementString("type", rom.OpenMSXType);
                            xtw.WriteOptionalElementString("remark", rom.Remark);
                            xtw.WriteEndElement();
                            break;

                        case OpenMSXSubType.SCCPlusCart:
                            xtw.WriteStartElement(rom.OpenMSXSubType.FromOpenMSXSubType());
                            xtw.WriteOptionalElementString("boot", rom.Boot);
                            xtw.WriteRequiredElementString("hash", rom.SHA1?.ToLowerInvariant());
                            xtw.WriteOptionalElementString("remark", rom.Remark);
                            xtw.WriteEndElement();
                            break;
                    }

                    // End dump
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
            // End software
            xtw.WriteEndElement();

            // End softwaredb
            xtw.WriteEndElement();

            xtw.Flush();
        }
    }
}
