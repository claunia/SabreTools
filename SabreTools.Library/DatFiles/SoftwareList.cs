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
    /// Represents parsing and writing of a SofwareList, M1, or MAME XML DAT
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
        protected override void ParseFile(string filename, int indexId, bool keep)
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
                            if (Header.ForceMerging == MergingFlag.None)
                                Header.ForceMerging = xtr.GetAttribute("forcemerging").AsMergingFlag();

                            if (Header.ForceNodump == NodumpFlag.None)
                                Header.ForceNodump = xtr.GetAttribute("forcenodump").AsNodumpFlag();

                            if (Header.ForcePacking == PackingFlag.None)
                                Header.ForcePacking = xtr.GetAttribute("forcepacking").AsPackingFlag();

                            xtr.Read();
                            break;

                        // We want to process the entire subtree of the machine
                        case "software":
                            ReadSoftware(xtr.ReadSubtree(), filename, indexId, keep);

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
                Globals.Logger.Warning($"Exception found while parsing '{filename}': {ex}");

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
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        private void ReadSoftware(XmlReader reader, string filename, int indexId, bool keep)
        {
            // If we have an empty software, skip it
            if (reader == null)
                return;

            // Otherwise, add what is possible
            reader.MoveToContent();

            bool containsItems = false;

            // Create a new machine
            MachineType machineType = 0x0;
            if (reader.GetAttribute("isbios").AsYesNo() == true)
                machineType |= MachineType.Bios;

            if (reader.GetAttribute("isdevice").AsYesNo() == true)
                machineType |= MachineType.Device;

            if (reader.GetAttribute("ismechanical").AsYesNo() == true)
                machineType |= MachineType.Mechanical;

            Machine machine = new Machine
            {
                Name = reader.GetAttribute("name"),
                Description = reader.GetAttribute("name"),
                Supported = reader.GetAttribute("supported").AsSupported(),

                CloneOf = reader.GetAttribute("cloneof"),
                MachineType = (machineType == 0x0 ? MachineType.NULL : machineType),
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

                    case "category":
                        machine.Category = reader.ReadElementContentAsString();
                        break;

                    case "info":
                        ParseAddHelper(new Info
                        {
                            Name = reader.GetAttribute("name"),
                            InfoValue = reader.GetAttribute("value"),

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
                        containsItems = ReadPart(reader.ReadSubtree(), machine, filename, indexId, keep);

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
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        private bool ReadPart(
            XmlReader reader,
            Machine machine,

            // Standard Dat parsing
            string filename,
            int indexId,

            // Miscellaneous
            bool keep)
        {
            string areaname,
                areaWidth,
                areaEndinaness;
            long? areasize = null;
            SoftwareListPart part = null;
            List<PartFeature> features = null;
            List<DatItem> items = new List<DatItem>();

            while (!reader.EOF)
            {
                // We only want elements
                if (reader.NodeType != XmlNodeType.Element)
                {
                    if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "part")
                    {
                        part = null;
                        features = null;
                    }

                    if (reader.NodeType == XmlNodeType.EndElement && (reader.Name == "dataarea" || reader.Name == "diskarea"))
                        areasize = null;

                    reader.Read();
                    continue;
                }

                // Get the elements from the software
                switch (reader.Name)
                {
                    case "part":
                        part = new SoftwareListPart();
                        part.Name = reader.GetAttribute("name");
                        part.Interface = reader.GetAttribute("interface");
                        reader.Read();
                        break;

                    case "feature":
                        var feature = new PartFeature();
                        feature.Name = reader.GetAttribute("name");
                        feature.Value = reader.GetAttribute("value");

                        // Ensure the list exists
                        if (features == null)
                            features = new List<PartFeature>();

                        features.Add(feature);

                        reader.Read();
                        break;

                    case "dataarea":
                        areaname = reader.GetAttribute("name");
                        if (reader.GetAttribute("size") != null)
                        {
                            if (Int64.TryParse(reader.GetAttribute("size"), out long tempas))
                                areasize = tempas;
                        }

                        areaWidth = reader.GetAttribute("width");
                        areaEndinaness = reader.GetAttribute("endianness");

                        List<DatItem> roms = ReadDataArea(
                            reader.ReadSubtree(),
                            areaname,
                            areasize,
                            areaWidth,
                            areaEndinaness,
                            keep);

                        // If we got valid roms, add them to the list
                        if (roms != null)
                            items.AddRange(roms);

                        // Skip the dataarea now that we've processed it
                        reader.Skip();
                        break;

                    case "diskarea":
                        areaname = reader.GetAttribute("name");

                        List<DatItem> disks = ReadDiskArea(
                            reader.ReadSubtree(),
                            areaname,
                            areasize);

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
                            Conditions = new List<Condition>(),
                            Locations = new List<Location>(),
                            Values = new List<Setting>(),
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
                item.Features = features;
                item.Part = part;
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
        /// <param name="areaName">Name of the containing area</param>
        /// <param name="areaSize">Size of the containing area</param>
        /// <param name="areaWidth">Byte width of the containing area</param>
        /// <param name="areaEndianness">Endianness of the containing area</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        private List<DatItem> ReadDataArea(
            XmlReader reader,
            string areaName,
            long? areaSize,
            string areaWidth,
            string areaEndianness,

            // Miscellaneous
            bool keep)
        {
            string key = string.Empty;
            string temptype = reader.Name;
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
                        // If the rom is continue or ignore, add the size to the previous rom
                        if (reader.GetAttribute("loadflag") == "continue" || reader.GetAttribute("loadflag") == "ignore")
                        {
                            int index = Items[key].Count - 1;
                            DatItem lastrom = Items[key][index];
                            if (lastrom.ItemType == ItemType.Rom)
                            {
                                ((Rom)lastrom).Size += Sanitizer.CleanSize(reader.GetAttribute("size"));
                            }

                            Items[key].RemoveAt(index);
                            Items[key].Add(lastrom);
                            reader.Read();
                            continue;
                        }

                        DatItem rom = new Rom
                        {
                            Name = reader.GetAttribute("name"),
                            Size = Sanitizer.CleanSize(reader.GetAttribute("size")),
                            CRC = reader.GetAttribute("crc"),
                            MD5 = reader.GetAttribute("md5"),
#if NET_FRAMEWORK
                            RIPEMD160 = reader.GetAttribute("ripemd160"),
#endif
                            SHA1 = reader.GetAttribute("sha1"),
                            SHA256 = reader.GetAttribute("sha256"),
                            SHA384 = reader.GetAttribute("sha384"),
                            SHA512 = reader.GetAttribute("sha512"),
                            Offset = reader.GetAttribute("offset"),
                            ItemStatus = reader.GetAttribute("status").AsItemStatus(),

                            AreaName = areaName,
                            AreaSize = areaSize,
                            AreaWidth = areaWidth,
                            AreaEndianness = areaEndianness,
                            Value = reader.GetAttribute("value"),
                            LoadFlag = reader.GetAttribute("loadflag"),
                        };

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
        /// <param name="areaname">Name of the containing area</param>
        /// <param name="areasize">Size of the containing area</param>
        private List<DatItem> ReadDiskArea(XmlReader reader, string areaname, long? areasize)
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
                            MD5 = reader.GetAttribute("md5"),
                            SHA1 = reader.GetAttribute("sha1"),
                            ItemStatus = reader.GetAttribute("status").AsItemStatus(),
                            Writable = reader.GetAttribute("writable").AsYesNo(),

                            AreaName = areaname,
                            AreaSize = areasize,
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
                        var dipValue = new Setting();
                        dipValue.Name = reader.GetAttribute("name");
                        dipValue.Value = reader.GetAttribute("value");
                        dipValue.Default = reader.GetAttribute("default").AsYesNo();

                        dipSwitch.Values.Add(dipValue);

                        reader.Read();
                        break;

                    default:
                        reader.Read();
                        break;
                }
            }
        }

        /// <summary>
        /// Create and open an output file for writing direct from a dictionary
        /// </summary>
        /// <param name="outfile">Name of the file to write to</param>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise (default)</param>
        /// <returns>True if the DAT was written correctly, false otherwise</returns>
        public override bool WriteToFile(string outfile, bool ignoreblanks = false)
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
                Globals.Logger.Error(ex.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Write out DAT header using the supplied StreamWriter
        /// </summary>
        /// <param name="xtw">XmlTextWriter to output to</param>
        /// <returns>True if the data was written, false on error</returns>
        private bool WriteHeader(XmlTextWriter xtw)
        {
            try
            {
                xtw.WriteStartDocument();
                xtw.WriteDocType("softwarelist", null, "softwarelist.dtd", null);

                xtw.WriteStartElement("softwarelist");
                xtw.WriteRequiredAttributeString("name", Header.Name);
                xtw.WriteRequiredAttributeString("description", Header.Description);

                xtw.Flush();
            }
            catch (Exception ex)
            {
                Globals.Logger.Error(ex.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Write out Game start using the supplied StreamWriter
        /// </summary>
        /// <param name="xtw">XmlTextWriter to output to</param>
        /// <param name="datItem">DatItem object to be output</param>
        /// <returns>True if the data was written, false on error</returns>
        private bool WriteStartGame(XmlTextWriter xtw, DatItem datItem)
        {
            try
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
            catch (Exception ex)
            {
                Globals.Logger.Error(ex.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Write out Game start using the supplied StreamWriter
        /// </summary>
        /// <param name="xtw">XmlTextWriter to output to</param>
        /// <returns>True if the data was written, false on error</returns>
        private bool WriteEndGame(XmlTextWriter xtw)
        {
            try
            {
                // End software
                xtw.WriteEndElement();

                xtw.Flush();
            }
            catch (Exception ex)
            {
                Globals.Logger.Error(ex.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Write out DatItem using the supplied StreamWriter
        /// </summary>
        /// <param name="xtw">XmlTextWriter to output to</param>
        /// <param name="datItem">DatItem object to be output</param>
        /// <returns>True if the data was written, false on error</returns>
        private bool WriteDatItem(XmlTextWriter xtw, DatItem datItem)
        {
            try
            {
                // Pre-process the item name
                ProcessItemName(datItem, true);

                // Build the state
                string areaName = datItem.AreaName;
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
                        if (string.IsNullOrWhiteSpace(areaName))
                            areaName = "cdrom";

                        xtw.WriteStartElement("part");
                        xtw.WriteRequiredAttributeString("name", datItem.Part?.Name);
                        xtw.WriteRequiredAttributeString("interface", datItem.Part?.Interface);

                        if (datItem.Features != null && datItem.Features.Count > 0)
                        {
                            foreach (PartFeature partFeature in datItem.Features)
                            {
                                xtw.WriteStartElement("feature");
                                xtw.WriteRequiredAttributeString("name", partFeature.Name);
                                xtw.WriteRequiredAttributeString("value", partFeature.Value);
                                xtw.WriteEndElement();
                            }
                        }

                        xtw.WriteStartElement("diskarea");
                        xtw.WriteRequiredAttributeString("name", areaName);
                        xtw.WriteOptionalAttributeString("size", disk.AreaSize.ToString());

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
                        xtw.WriteRequiredAttributeString("value", info.InfoValue);
                        xtw.WriteEndElement();
                        break;

                    case ItemType.Rom:
                        var rom = datItem as Rom;
                        if (string.IsNullOrWhiteSpace(areaName))
                            areaName = "rom";

                        xtw.WriteStartElement("part");
                        xtw.WriteRequiredAttributeString("name", datItem.Part?.Name);
                        xtw.WriteRequiredAttributeString("interface", datItem.Part?.Interface);

                        if (datItem.Features != null && datItem.Features.Count > 0)
                        {
                            foreach (PartFeature kvp in datItem.Features)
                            {
                                xtw.WriteStartElement("feature");
                                xtw.WriteRequiredAttributeString("name", kvp.Name);
                                xtw.WriteRequiredAttributeString("value", kvp.Value);
                                xtw.WriteEndElement();
                            }
                        }

                        xtw.WriteStartElement("dataarea");
                        xtw.WriteRequiredAttributeString("name", areaName);
                        xtw.WriteOptionalAttributeString("size", rom.AreaSize.ToString());
                        xtw.WriteOptionalAttributeString("width", rom.AreaWidth);
                        xtw.WriteOptionalAttributeString("endianness", rom.AreaEndianness);

                        xtw.WriteStartElement("rom");
                        xtw.WriteRequiredAttributeString("name", rom.Name);
                        if (rom.Size != -1) xtw.WriteAttributeString("size", rom.Size.ToString());
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
                        xtw.WriteOptionalAttributeString("loadflag", rom.LoadFlag);
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
            catch (Exception ex)
            {
                Globals.Logger.Error(ex.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Write out DAT footer using the supplied StreamWriter
        /// </summary>
        /// <param name="xtw">XmlTextWriter to output to</param>
        /// <returns>True if the data was written, false on error</returns>
        private bool WriteFooter(XmlTextWriter xtw)
        {
            try
            {
                // End software
                xtw.WriteEndElement();

                // End softwarelist
                xtw.WriteEndElement();

                xtw.Flush();
            }
            catch (Exception ex)
            {
                Globals.Logger.Error(ex.ToString());
                return false;
            }

            return true;
        }
    }
}
