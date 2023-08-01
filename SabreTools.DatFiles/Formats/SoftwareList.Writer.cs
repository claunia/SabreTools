using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    /// Represents writing a SoftwareList
    /// </summary>
    internal partial class SoftwareList : DatFile
    {
        /// <inheritdoc/>
        protected override ItemType[] GetSupportedTypes()
        {
            return new ItemType[]
            {
                ItemType.DipSwitch,
                ItemType.Disk,
                ItemType.Info,
                ItemType.Rom,
                ItemType.SharedFeature,
            };
        }

        /// <inheritdoc/>
        protected override List<DatItemField> GetMissingRequiredFields(DatItem datItem)
        {
            List<DatItemField> missingFields = new();

            switch (datItem.ItemType)
            {
                case ItemType.DipSwitch:
                    DipSwitch dipSwitch = datItem as DipSwitch;
                    if (!dipSwitch.PartSpecified)
                    {
                        missingFields.Add(DatItemField.Part_Name);
                        missingFields.Add(DatItemField.Part_Interface);
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(dipSwitch.Part.Name))
                            missingFields.Add(DatItemField.Part_Name);
                        if (string.IsNullOrWhiteSpace(dipSwitch.Part.Interface))
                            missingFields.Add(DatItemField.Part_Interface);
                    }
                    if (string.IsNullOrWhiteSpace(dipSwitch.Name))
                        missingFields.Add(DatItemField.Name);
                    if (string.IsNullOrWhiteSpace(dipSwitch.Tag))
                        missingFields.Add(DatItemField.Tag);
                    if (string.IsNullOrWhiteSpace(dipSwitch.Mask))
                        missingFields.Add(DatItemField.Mask);
                    if (dipSwitch.ValuesSpecified)
                    {
                        if (dipSwitch.Values.Any(dv => string.IsNullOrWhiteSpace(dv.Name)))
                            missingFields.Add(DatItemField.Part_Feature_Name);
                        if (dipSwitch.Values.Any(dv => string.IsNullOrWhiteSpace(dv.Value)))
                            missingFields.Add(DatItemField.Part_Feature_Value);
                    }

                    break;

                case ItemType.Disk:
                    Disk disk = datItem as Disk;
                    if (!disk.PartSpecified)
                    {
                        missingFields.Add(DatItemField.Part_Name);
                        missingFields.Add(DatItemField.Part_Interface);
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(disk.Part.Name))
                            missingFields.Add(DatItemField.Part_Name);
                        if (string.IsNullOrWhiteSpace(disk.Part.Interface))
                            missingFields.Add(DatItemField.Part_Interface);
                    }
                    if (!disk.DiskAreaSpecified)
                    {
                        missingFields.Add(DatItemField.AreaName);
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(disk.DiskArea.Name))
                            missingFields.Add(DatItemField.AreaName);
                    }
                    if (string.IsNullOrWhiteSpace(disk.Name))
                        missingFields.Add(DatItemField.Name);
                    break;

                case ItemType.Info:
                    Info info = datItem as Info;
                    if (string.IsNullOrWhiteSpace(info.Name))
                        missingFields.Add(DatItemField.Name);
                    break;

                case ItemType.Rom:
                    Rom rom = datItem as Rom;
                    if (!rom.PartSpecified)
                    {
                        missingFields.Add(DatItemField.Part_Name);
                        missingFields.Add(DatItemField.Part_Interface);
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(rom.Part.Name))
                            missingFields.Add(DatItemField.Part_Name);
                        if (string.IsNullOrWhiteSpace(rom.Part.Interface))
                            missingFields.Add(DatItemField.Part_Interface);
                    }
                    if (!rom.DataAreaSpecified)
                    {
                        missingFields.Add(DatItemField.AreaName);
                        missingFields.Add(DatItemField.AreaSize);
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(rom.DataArea.Name))
                            missingFields.Add(DatItemField.AreaName);
                        if (!rom.DataArea.SizeSpecified)
                            missingFields.Add(DatItemField.AreaSize);
                    }
                    break;

                case ItemType.SharedFeature:
                    SharedFeature sharedFeature = datItem as SharedFeature;
                    if (string.IsNullOrWhiteSpace(sharedFeature.Name))
                        missingFields.Add(DatItemField.Name);
                    break;
                default:
                    // Unsupported ItemTypes should be caught already
                    return null;
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
            xtw.WriteDocType("softwarelist", null, "softwarelist.dtd", null);

            xtw.WriteStartElement("softwarelist");
            xtw.WriteRequiredAttributeString("name", Header.Name);
            xtw.WriteRequiredAttributeString("description", Header.Description);

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
            xtw.WriteRequiredAttributeString("name", datItem.Machine.Name);
            if (!string.Equals(datItem.Machine.Name, datItem.Machine.CloneOf, StringComparison.OrdinalIgnoreCase))
                xtw.WriteOptionalAttributeString("cloneof", datItem.Machine.CloneOf);
            xtw.WriteOptionalAttributeString("supported", datItem.Machine.Supported.FromSupported(false));

            xtw.WriteOptionalElementString("description", datItem.Machine.Description);
            xtw.WriteOptionalElementString("year", datItem.Machine.Year);
            xtw.WriteOptionalElementString("publisher", datItem.Machine.Publisher);

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
                case ItemType.DipSwitch:
                    var dipSwitch = datItem as DipSwitch;
                    xtw.WriteStartElement("dipswitch");
                    xtw.WriteRequiredAttributeString("name", dipSwitch.Name);
                    xtw.WriteRequiredAttributeString("tag", dipSwitch.Tag);
                    xtw.WriteRequiredAttributeString("mask", dipSwitch.Mask);
                    if (dipSwitch.ValuesSpecified)
                    {
                        foreach (Setting dipValue in dipSwitch.Values)
                        {
                            xtw.WriteStartElement("dipvalue");
                            xtw.WriteRequiredAttributeString("name", dipValue.Name);
                            xtw.WriteRequiredAttributeString("value", dipValue.Value);
                            xtw.WriteOptionalAttributeString("default", dipValue.Default.FromYesNo());
                            xtw.WriteEndElement();
                        }
                    }
                    xtw.WriteEndElement();
                    break;

                case ItemType.Disk:
                    var disk = datItem as Disk;
                    string diskAreaName = disk.DiskArea?.Name;
                    if (string.IsNullOrWhiteSpace(diskAreaName))
                        diskAreaName = "cdrom";

                    xtw.WriteStartElement("part");
                    xtw.WriteRequiredAttributeString("name", disk.Part?.Name);
                    xtw.WriteRequiredAttributeString("interface", disk.Part?.Interface);

                    if (disk.Part?.FeaturesSpecified == true)
                    {
                        foreach (PartFeature partFeature in disk.Part.Features)
                        {
                            xtw.WriteStartElement("feature");
                            xtw.WriteRequiredAttributeString("name", partFeature.Name);
                            xtw.WriteOptionalAttributeString("value", partFeature.Value);
                            xtw.WriteEndElement();
                        }
                    }

                    xtw.WriteStartElement("diskarea");
                    xtw.WriteRequiredAttributeString("name", diskAreaName);

                    xtw.WriteStartElement("disk");
                    xtw.WriteRequiredAttributeString("name", disk.Name);
                    xtw.WriteOptionalAttributeString("md5", disk.MD5?.ToLowerInvariant());
                    xtw.WriteOptionalAttributeString("sha1", disk.SHA1?.ToLowerInvariant());
                    if (disk.ItemStatus != ItemStatus.None)
                        xtw.WriteOptionalAttributeString("status", disk.ItemStatus.FromItemStatus(false));
                    xtw.WriteOptionalAttributeString("writable", disk.Writable.FromYesNo());
                    xtw.WriteEndElement();

                    // End diskarea
                    xtw.WriteEndElement();

                    // End part
                    xtw.WriteEndElement();
                    break;

                case ItemType.Info:
                    var info = datItem as Info;
                    xtw.WriteStartElement("info");
                    xtw.WriteRequiredAttributeString("name", info.Name);
                    xtw.WriteOptionalAttributeString("value", info.Value);
                    xtw.WriteEndElement();
                    break;

                case ItemType.Rom:
                    var rom = datItem as Rom;
                    string dataAreaName = rom.DataArea?.Name;
                    if (string.IsNullOrWhiteSpace(dataAreaName))
                        dataAreaName = "rom";

                    xtw.WriteStartElement("part");
                    xtw.WriteRequiredAttributeString("name", rom.Part?.Name);
                    xtw.WriteRequiredAttributeString("interface", rom.Part?.Interface);

                    if (rom.Part?.FeaturesSpecified == true)
                    {
                        foreach (PartFeature kvp in rom.Part.Features)
                        {
                            xtw.WriteStartElement("feature");
                            xtw.WriteRequiredAttributeString("name", kvp.Name);
                            xtw.WriteOptionalAttributeString("value", kvp.Value);
                            xtw.WriteEndElement();
                        }
                    }

                    xtw.WriteStartElement("dataarea");
                    xtw.WriteOptionalAttributeString("name", dataAreaName);
                    xtw.WriteOptionalAttributeString("size", rom.DataArea?.Size.ToString());
                    xtw.WriteOptionalAttributeString("width", rom.DataArea?.Width?.ToString());
                    xtw.WriteOptionalAttributeString("endianness", rom.DataArea?.Endianness.FromEndianness());

                    xtw.WriteStartElement("rom");
                    xtw.WriteRequiredAttributeString("name", rom.Name);
                    xtw.WriteOptionalAttributeString("size", rom.Size?.ToString());
                    xtw.WriteOptionalAttributeString("crc", rom.CRC?.ToLowerInvariant());
                    xtw.WriteOptionalAttributeString("md5", rom.MD5?.ToLowerInvariant());
                    xtw.WriteOptionalAttributeString("sha1", rom.SHA1?.ToLowerInvariant());
                    xtw.WriteOptionalAttributeString("sha256", rom.SHA256?.ToLowerInvariant());
                    xtw.WriteOptionalAttributeString("sha384", rom.SHA384?.ToLowerInvariant());
                    xtw.WriteOptionalAttributeString("sha512", rom.SHA512?.ToLowerInvariant());
                    xtw.WriteOptionalAttributeString("offset", rom.Offset);
                    xtw.WriteOptionalAttributeString("value", rom.Value);
                    if (rom.ItemStatus != ItemStatus.None)
                        xtw.WriteOptionalAttributeString("status", rom.ItemStatus.FromItemStatus(false));
                    xtw.WriteOptionalAttributeString("loadflag", rom.LoadFlag.FromLoadFlag());
                    xtw.WriteEndElement();

                    // End dataarea
                    xtw.WriteEndElement();

                    // End part
                    xtw.WriteEndElement();
                    break;

                case ItemType.SharedFeature:
                    var sharedFeature = datItem as SharedFeature;
                    xtw.WriteStartElement("sharedfeat");
                    xtw.WriteRequiredAttributeString("name", sharedFeature.Name);
                    xtw.WriteOptionalAttributeString("value", sharedFeature.Value);
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

            // End softwarelist
            xtw.WriteEndElement();

            xtw.Flush();
        }
    }
}
