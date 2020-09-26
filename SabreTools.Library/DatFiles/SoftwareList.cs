using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

using SabreTools.Library.Data;
using SabreTools.Library.DatItems;
using SabreTools.Library.IO;
using SabreTools.Library.Tools;

// TODO: Use softwarelist.dtd and *try* to make this write more correctly
namespace SabreTools.Library.DatFiles
{
    /// <summary>
    /// Represents parsing and writing of a SoftwareList
    /// </summary>
    internal class SoftwareList : DatFile
    {
        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public SoftwareList(DatFile datFile)
            : base(datFile)
        {
        }

        /// <summary>
        /// Parse an SofwareList XML DAT and return all found games and roms within
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
                        case "softwarelist":
                            Header.Name = (Header.Name == null ? xtr.GetAttribute("name") ?? string.Empty : Header.Name);
                            Header.Description = (Header.Description == null ? xtr.GetAttribute("description") ?? string.Empty : Header.Description);

                            xtr.Read();
                            break;

                        // We want to process the entire subtree of the machine
                        case "software":
                            ReadSoftware(xtr.ReadSubtree(), filename, indexId);

                            // Skip the software now that we've processed it
                            xtr.Skip();
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
        /// Read software information
        /// </summary>
        /// <param name="reader">XmlReader representing a software block</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        private void ReadSoftware(XmlReader reader, string filename, int indexId)
        {
            // If we have an empty software, skip it
            if (reader == null)
                return;

            // Otherwise, add what is possible
            reader.MoveToContent();

            bool containsItems = false;

            // Create a new machine
            Machine machine = new Machine
            {
                Name = reader.GetAttribute("name"),
                CloneOf = reader.GetAttribute("cloneof"),
                Supported = reader.GetAttribute("supported").AsSupported(),
            };

            while (!reader.EOF)
            {
                // We only want elements
                if (reader.NodeType != XmlNodeType.Element)
                {
                    reader.Read();
                    continue;
                }

                // Get the elements from the software
                switch (reader.Name)
                {
                    case "description":
                        machine.Description = reader.ReadElementContentAsString();
                        break;

                    case "year":
                        machine.Year = reader.ReadElementContentAsString();
                        break;

                    case "publisher":
                        machine.Publisher = reader.ReadElementContentAsString();
                        break;

                    case "info":
                        ParseAddHelper(new Info
                        {
                            Name = reader.GetAttribute("name"),
                            Value = reader.GetAttribute("value"),

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        });

                        reader.Read();
                        break;

                    case "sharedfeat":
                        ParseAddHelper(new SharedFeature
                        {
                            Name = reader.GetAttribute("name"),
                            Value = reader.GetAttribute("value"),

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        });

                        reader.Read();
                        break;

                    case "part": // Contains all rom and disk information
                        var part = new Part()
                        {
                            Name = reader.GetAttribute("name"),
                            Interface = reader.GetAttribute("interface"),
                        };

                        // Now read the internal tags
                        containsItems = ReadPart(reader.ReadSubtree(), machine, part, filename, indexId);

                        // Skip the part now that we've processed it
                        reader.Skip();
                        break;

                    default:
                        reader.Read();
                        break;
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

        /// <summary>
        /// Read part information
        /// </summary>
        /// <param name="reader">XmlReader representing a part block</param>
        /// <param name="machine">Machine information to pass to contained items</param>
        /// <param name="part">Part information to pass to contained items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        private bool ReadPart(XmlReader reader, Machine machine, Part part, string filename, int indexId)
        {
            // If we have an empty port, skip it
            if (reader == null)
                return false;

            // Get lists ready
            part.Features = new List<PartFeature>();
            List<DatItem> items = new List<DatItem>();

            while (!reader.EOF)
            {
                // We only want elements
                if (reader.NodeType != XmlNodeType.Element)
                {
                    reader.Read();
                    continue;
                }

                // Get the elements from the software
                switch (reader.Name)
                {
                    case "feature":
                        var feature = new PartFeature()
                        {
                            Name = reader.GetAttribute("name"),
                            Value = reader.GetAttribute("value"),
                        };

                        part.Features.Add(feature);

                        reader.Read();
                        break;

                    case "dataarea":
                        var dataArea = new DataArea
                        {
                            Name = reader.GetAttribute("name"),
                            Size = Sanitizer.CleanLong(reader.GetAttribute("size")),
                            Width = Sanitizer.CleanLong(reader.GetAttribute("width")),
                            Endianness = reader.GetAttribute("endianness").AsEndianness(),
                        };

                        List<DatItem> roms = ReadDataArea(reader.ReadSubtree(), dataArea);

                        // If we got valid roms, add them to the list
                        if (roms != null)
                            items.AddRange(roms);

                        // Skip the dataarea now that we've processed it
                        reader.Skip();
                        break;

                    case "diskarea":
                        var diskArea = new DiskArea
                        {
                            Name = reader.GetAttribute("name"),
                        };

                        List<DatItem> disks = ReadDiskArea(reader.ReadSubtree(), diskArea);

                        // If we got valid disks, add them to the list
                        if (disks != null)
                            items.AddRange(disks);

                        // Skip the diskarea now that we've processed it
                        reader.Skip();
                        break;

                    case "dipswitch":
                        var dipSwitch = new DipSwitch
                        {
                            Name = reader.GetAttribute("name"),
                            Tag = reader.GetAttribute("tag"),
                            Mask = reader.GetAttribute("mask"),
                        };

                        // Now read the internal tags
                        ReadDipSwitch(reader.ReadSubtree(), dipSwitch);

                        items.Add(dipSwitch);

                        // Skip the dipswitch now that we've processed it
                        reader.Skip();
                        break;

                    default:
                        reader.Read();
                        break;
                }
            }

            // Loop over all of the items, if they exist
            foreach (DatItem item in items)
            {
                // Add all missing information
                switch (item.ItemType)
                {
                    case ItemType.DipSwitch:
                        (item as DipSwitch).Part = part;
                        break;
                    case ItemType.Disk:
                        (item as Disk).Part = part;
                        break;
                    case ItemType.Rom:
                        (item as Rom).Part = part;
                        break;
                }

                item.Source = new Source(indexId, filename);
                item.CopyMachineInformation(machine);

                // Finally add each item
                ParseAddHelper(item);
            }

            return items.Any();
        }

        /// <summary>
        /// Read dataarea information
        /// </summary>
        /// <param name="reader">XmlReader representing a dataarea block</param>
        /// <param name="dataArea">DataArea representing the enclosing area</param>
        private List<DatItem> ReadDataArea(XmlReader reader, DataArea dataArea)
        {
            string key = string.Empty;
            List<DatItem> items = new List<DatItem>();

            while (!reader.EOF)
            {
                // We only want elements
                if (reader.NodeType != XmlNodeType.Element)
                {
                    reader.Read();
                    continue;
                }

                // Get the elements from the software
                switch (reader.Name)
                {
                    case "rom":
                        var rom = new Rom
                        {
                            Name = reader.GetAttribute("name"),
                            Size = Sanitizer.CleanLong(reader.GetAttribute("size")),
                            CRC = reader.GetAttribute("crc"),
                            SHA1 = reader.GetAttribute("sha1"),
                            Offset = reader.GetAttribute("offset"),
                            Value = reader.GetAttribute("value"),
                            ItemStatus = reader.GetAttribute("status").AsItemStatus(),
                            LoadFlag = reader.GetAttribute("loadflag").AsLoadFlag(),

                            DataArea = dataArea,
                        };

                        // If the rom is continue or ignore, add the size to the previous rom
                        // TODO: Can this be done on write? We technically lose information this way.
                        // Order is not guaranteed, and since these don't tend to have any way
                        // of determining what the "previous" item was after this, that info would
                        // have to be stored *with* the item somehow
                        if (rom.LoadFlag == LoadFlag.Continue || rom.LoadFlag == LoadFlag.Ignore)
                        {
                            int index = Items[key].Count - 1;
                            DatItem lastrom = Items[key][index];
                            if (lastrom.ItemType == ItemType.Rom)
                                (lastrom as Rom).Size += rom.Size;

                            Items[key].RemoveAt(index);
                            Items[key].Add(lastrom);
                            reader.Read();
                            continue;
                        }

                        items.Add(rom);
                        reader.Read();
                        break;

                    default:
                        reader.Read();
                        break;
                }
            }

            return items;
        }

        /// <summary>
        /// Read diskarea information
        /// </summary>
        /// <param name="reader">XmlReader representing a diskarea block</param>
        /// <param name="diskArea">DiskArea representing the enclosing area</param>
        private List<DatItem> ReadDiskArea(XmlReader reader, DiskArea diskArea)
        {
            List<DatItem> items = new List<DatItem>();

            while (!reader.EOF)
            {
                // We only want elements
                if (reader.NodeType != XmlNodeType.Element)
                {
                    reader.Read();
                    continue;
                }

                // Get the elements from the software
                switch (reader.Name)
                {
                    case "disk":
                        DatItem disk = new Disk
                        {
                            Name = reader.GetAttribute("name"),
                            SHA1 = reader.GetAttribute("sha1"),
                            ItemStatus = reader.GetAttribute("status").AsItemStatus(),
                            Writable = reader.GetAttribute("writable").AsYesNo(),

                            DiskArea = diskArea,
                        };

                        items.Add(disk);
                        reader.Read();
                        break;

                    default:
                        reader.Read();
                        break;
                }
            }

            return items;
        }

        /// <summary>
        /// Read DipSwitch DipValues information
        /// </summary>
        /// <param name="reader">XmlReader representing a diskarea block</param>
        /// <param name="dipSwitch">DipSwitch to populate</param>
        private void ReadDipSwitch(XmlReader reader, DipSwitch dipSwitch)
        {
            // If we have an empty dipswitch, skip it
            if (reader == null)
                return;

            // Get list ready
            dipSwitch.Values = new List<Setting>();

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

                // Get the information from the dipswitch
                switch (reader.Name)
                {
                    case "dipvalue":
                        var dipValue = new Setting
                        {
                            Name = reader.GetAttribute("name"),
                            Value = reader.GetAttribute("value"),
                            Default = reader.GetAttribute("default").AsYesNo(),
                        };

                        dipSwitch.Values.Add(dipValue);

                        reader.Read();
                        break;

                    default:
                        reader.Read();
                        break;
                }
            }
        }

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
                    Indentation = 1
                };

                // Write out the header
                WriteHeader(xtw);

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
                    if (dipSwitch.Values != null)
                    {
                        foreach (Setting dipValue in dipSwitch.Values)
                        {
                            xtw.WriteStartElement("dipvalue");
                            xtw.WriteRequiredAttributeString("name", dipValue.Name);
                            xtw.WriteOptionalAttributeString("value", dipValue.Value);
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

                    if (disk.Part?.Features != null && disk.Part?.Features.Count > 0)
                    {
                        foreach (PartFeature partFeature in disk.Part.Features)
                        {
                            xtw.WriteStartElement("feature");
                            xtw.WriteRequiredAttributeString("name", partFeature.Name);
                            xtw.WriteRequiredAttributeString("value", partFeature.Value);
                            xtw.WriteEndElement();
                        }
                    }

                    xtw.WriteStartElement("diskarea");
                    xtw.WriteRequiredAttributeString("name", diskAreaName);

                    xtw.WriteStartElement("disk");
                    xtw.WriteRequiredAttributeString("name", disk.Name);
                    xtw.WriteOptionalAttributeString("md5", disk.MD5?.ToLowerInvariant());
                    xtw.WriteOptionalAttributeString("sha1", disk.SHA1?.ToLowerInvariant());
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
                    xtw.WriteRequiredAttributeString("value", info.Value);
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

                    if (rom.Part?.Features != null && rom.Part?.Features.Count > 0)
                    {
                        foreach (PartFeature kvp in rom.Part.Features)
                        {
                            xtw.WriteStartElement("feature");
                            xtw.WriteRequiredAttributeString("name", kvp.Name);
                            xtw.WriteRequiredAttributeString("value", kvp.Value);
                            xtw.WriteEndElement();
                        }
                    }

                    xtw.WriteStartElement("dataarea");
                    xtw.WriteRequiredAttributeString("name", dataAreaName);
                    xtw.WriteOptionalAttributeString("size", rom.DataArea?.Size.ToString());
                    xtw.WriteOptionalAttributeString("width", rom.DataArea?.Width?.ToString());
                    xtw.WriteOptionalAttributeString("endianness", rom.DataArea?.Endianness.FromEndianness());

                    xtw.WriteStartElement("rom");
                    xtw.WriteRequiredAttributeString("name", rom.Name);
                    xtw.WriteOptionalAttributeString("size", rom.Size?.ToString());
                    xtw.WriteOptionalAttributeString("crc", rom.CRC?.ToLowerInvariant());
                    xtw.WriteOptionalAttributeString("md5", rom.MD5?.ToLowerInvariant());
#if NET_FRAMEWORK
                    xtw.WriteOptionalAttributeString("ripemd160", rom.RIPEMD160?.ToLowerInvariant());
#endif
                    xtw.WriteOptionalAttributeString("sha1", rom.SHA1?.ToLowerInvariant());
                    xtw.WriteOptionalAttributeString("sha256", rom.SHA256?.ToLowerInvariant());
                    xtw.WriteOptionalAttributeString("sha384", rom.SHA384?.ToLowerInvariant());
                    xtw.WriteOptionalAttributeString("sha512", rom.SHA512?.ToLowerInvariant());
                    xtw.WriteOptionalAttributeString("offset", rom.Offset);
                    xtw.WriteOptionalAttributeString("value", rom.Value);
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
                    xtw.WriteRequiredAttributeString("value", sharedFeature.Value);
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
