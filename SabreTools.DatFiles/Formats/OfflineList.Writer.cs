using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using SabreTools.Core;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.IO;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents writing an OfflineList XML DAT
    /// </summary>
    internal partial class OfflineList : DatFile
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
            var missingFields = new List<DatItemField>();

            if (string.IsNullOrWhiteSpace(datItem.GetName()))
                missingFields.Add(DatItemField.Name);

            switch (datItem)
            {
                case Rom rom:
                    if (rom.Size == null || rom.Size < 0)
                        missingFields.Add(DatItemField.Size);
                    if (string.IsNullOrWhiteSpace(rom.CRC))
                    {
                        missingFields.Add(DatItemField.SHA1);
                    }
                    break;
            }

            return missingFields;
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
            xtw.WriteStartDocument(false);

            xtw.WriteStartElement("dat");
            xtw.WriteAttributeString("xsi", "xmlns", "http://www.w3.org/2001/XMLSchema-instance");
            xtw.WriteAttributeString("noNamespaceSchemaLocation", "xsi", "datas.xsd");

            xtw.WriteStartElement("configuration");
            xtw.WriteRequiredElementString("datName", Header.Name);
            xtw.WriteElementString("datVersion", Items.TotalCount.ToString());
            xtw.WriteRequiredElementString("system", Header.System);
            xtw.WriteRequiredElementString("screenshotsWidth", Header.ScreenshotsWidth);
            xtw.WriteRequiredElementString("screenshotsHeight", Header.ScreenshotsHeight);

            if (Header.Infos != null)
            {
                xtw.WriteStartElement("infos");

                foreach (var info in Header.Infos)
                {
                    xtw.WriteStartElement(info.Name);
                    xtw.WriteAttributeString("visible", info.Visible?.ToString());
                    xtw.WriteAttributeString("inNamingOption", info.InNamingOption?.ToString());
                    xtw.WriteAttributeString("default", info.Default?.ToString());
                    xtw.WriteEndElement();
                }

                // End infos
                xtw.WriteEndElement();
            }

            if (Header.CanOpen != null)
            {
                xtw.WriteStartElement("canOpen");

                foreach (string extension in Header.CanOpen)
                {
                    xtw.WriteElementString("extension", extension);
                }

                // End canOpen
                xtw.WriteEndElement();
            }

            xtw.WriteStartElement("newDat");
            xtw.WriteRequiredElementString("datVersionURL", Header.Url);

            xtw.WriteStartElement("datUrl");
            xtw.WriteAttributeString("fileName", $"{Header.FileName ?? string.Empty}.zip");
            xtw.WriteString(Header.Url);
            xtw.WriteEndElement();

            xtw.WriteRequiredElementString("imURL", Header.Url);

            // End newDat
            xtw.WriteEndElement();

            xtw.WriteStartElement("search");

            xtw.WriteStartElement("to");
            xtw.WriteAttributeString("value", "location");
            xtw.WriteAttributeString("default", "true");
            xtw.WriteAttributeString("auto", "true");
            xtw.WriteEndElement();

            xtw.WriteStartElement("to");
            xtw.WriteAttributeString("value", "romSize");
            xtw.WriteAttributeString("default", "true");
            xtw.WriteAttributeString("auto", "false");
            xtw.WriteEndElement();

            xtw.WriteStartElement("to");
            xtw.WriteAttributeString("value", "languages");
            xtw.WriteAttributeString("default", "true");
            xtw.WriteAttributeString("auto", "true");
            xtw.WriteEndElement();

            xtw.WriteStartElement("to");
            xtw.WriteAttributeString("value", "saveType");
            xtw.WriteAttributeString("default", "false");
            xtw.WriteAttributeString("auto", "false");
            xtw.WriteEndElement();

            xtw.WriteStartElement("to");
            xtw.WriteAttributeString("value", "publisher");
            xtw.WriteAttributeString("default", "false");
            xtw.WriteAttributeString("auto", "true");
            xtw.WriteEndElement();

            xtw.WriteStartElement("to");
            xtw.WriteAttributeString("value", "sourceRom");
            xtw.WriteAttributeString("default", "false");
            xtw.WriteAttributeString("auto", "true");
            xtw.WriteEndElement();

            // End search
            xtw.WriteEndElement();

            xtw.WriteRequiredElementString("romTitle", Header.RomTitle ?? "%u - %n");

            // End configuration
            xtw.WriteEndElement();

            xtw.WriteStartElement("games");

            xtw.Flush();
        }

        /// <summary>
        /// Write out DatItem using the supplied StreamWriter
        /// </summary>
        /// <param name="xtw">XmlTextWriter to output to</param>
        /// <param name="datItem">DatItem object to be output</param>
        /// <returns>True if the data was written, false on error</returns>
        private void WriteDatItem(XmlTextWriter xtw, DatItem datItem)
        {
            // Pre-process the item name
            ProcessItemName(datItem, true);

            // Build the state
            xtw.WriteStartElement("game");
            xtw.WriteElementString("imageNumber", "1");
            xtw.WriteElementString("releaseNumber", "1");
            xtw.WriteRequiredElementString("title", datItem.GetName() ?? string.Empty);
            xtw.WriteElementString("saveType", "None");

            if (datItem.ItemType == ItemType.Rom)
            {
                var rom = datItem as Rom;
                xtw.WriteRequiredElementString("romSize", rom.Size?.ToString());
            }

            xtw.WriteRequiredElementString("publisher", datItem.Machine.Publisher);
            xtw.WriteElementString("location", "0");
            xtw.WriteElementString("sourceRom", "None");
            xtw.WriteElementString("language", "0");

            if (datItem.ItemType == ItemType.Rom)
            {
                var rom = datItem as Rom;
                string tempext = "." + rom.Name.GetNormalizedExtension();

                xtw.WriteStartElement("files");
                if (!string.IsNullOrWhiteSpace(rom.CRC))
                {
                    xtw.WriteStartElement("romCRC");
                    xtw.WriteRequiredAttributeString("extension", tempext);
                    xtw.WriteString(rom.CRC?.ToUpperInvariant());
                    xtw.WriteEndElement();
                }

                // End files
                xtw.WriteEndElement();
            }

            xtw.WriteElementString("im1CRC", "00000000");
            xtw.WriteElementString("im2CRC", "00000000");
            xtw.WriteRequiredElementString("comment", datItem.Machine.Comment);
            xtw.WriteRequiredElementString("duplicateID", datItem.Machine.CloneOf);

            // End game
            xtw.WriteEndElement();

            xtw.Flush();
        }

        /// <summary>
        /// Write out DAT footer using the supplied StreamWriter
        /// </summary>
        /// <param name="xtw">XmlTextWriter to output to</param>
        /// <returns>True if the data was written, false on error</returns>
        private void WriteFooter(XmlTextWriter xtw)
        {
            // End games
            xtw.WriteEndElement();

            xtw.WriteStartElement("gui");

            xtw.WriteStartElement("images");
            xtw.WriteAttributeString("width", "487");
            xtw.WriteAttributeString("height", "162");

            xtw.WriteStartElement("image");
            xtw.WriteAttributeString("x", "0");
            xtw.WriteAttributeString("y", "0");
            xtw.WriteAttributeString("width", "240");
            xtw.WriteAttributeString("height", "160");
            xtw.WriteEndElement();

            xtw.WriteStartElement("image");
            xtw.WriteAttributeString("x", "245");
            xtw.WriteAttributeString("y", "0");
            xtw.WriteAttributeString("width", "240");
            xtw.WriteAttributeString("height", "160");
            xtw.WriteEndElement();

            // End images
            xtw.WriteEndElement();

            // End gui
            xtw.WriteEndElement();

            // End dat
            xtw.WriteEndElement();

            xtw.Flush();
        }
    }
}
