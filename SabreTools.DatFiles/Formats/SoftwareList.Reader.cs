using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing a SoftwareList
    /// </summary>
    internal partial class SoftwareList : DatFile
    {
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
                        case "softwarelist":
                            Header.Name ??= xtr.GetAttribute("name") ?? string.Empty;
                            Header.Description ??= xtr.GetAttribute("description") ?? string.Empty;

                            xtr.Read();
                            break;

                        // We want to process the entire subtree of the machine
                        case "software":
                            ReadSoftware(xtr.ReadSubtree(), statsOnly, filename, indexId);

                            // Skip the software now that we've processed it
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
        /// Read software information
        /// </summary>
        /// <param name="reader">XmlReader representing a software block</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        private void ReadSoftware(XmlReader reader, bool statsOnly, string filename, int indexId)
        {
            // If we have an empty software, skip it
            if (reader == null)
                return;

            // Otherwise, add what is possible
            reader.MoveToContent();

            bool containsItems = false;

            // Create a new machine
            Machine machine = new()
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
                        }, statsOnly);

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
                        }, statsOnly);

                        reader.Read();
                        break;

                    case "part": // Contains all rom and disk information
                        var part = new Part()
                        {
                            Name = reader.GetAttribute("name"),
                            Interface = reader.GetAttribute("interface"),
                        };

                        // Now read the internal tags
                        containsItems = ReadPart(reader.ReadSubtree(), machine, part, statsOnly, filename, indexId);

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
                Blank blank = new()
                {
                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

                blank.CopyMachineInformation(machine);

                // Now process and add the rom
                ParseAddHelper(blank, statsOnly);
            }
        }

        /// <summary>
        /// Read part information
        /// </summary>
        /// <param name="reader">XmlReader representing a part block</param>
        /// <param name="machine">Machine information to pass to contained items</param>
        /// <param name="part">Part information to pass to contained items</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        private bool ReadPart(XmlReader reader, Machine machine, Part part, bool statsOnly, string filename, int indexId)
        {
            // If we have an empty port, skip it
            if (reader == null)
                return false;

            // Get lists ready
            part.Features = new List<PartFeature>();
            List<DatItem> items = new();

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
                            Size = Utilities.CleanLong(reader.GetAttribute("size")),
                            Width = Utilities.CleanLong(reader.GetAttribute("width")),
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
            string key = string.Empty;
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

                        // If the rom is continue or ignore, add the size to the previous rom
                        // TODO: Can this be done on write? We technically lose information this way.
                        // Order is not guaranteed, and since these don't tend to have any way
                        // of determining what the "previous" item was after this, that info would
                        // have to be stored *with* the item somehow
                        if ((item as Rom).LoadFlag == LoadFlag.Continue || (item as Rom).LoadFlag == LoadFlag.Ignore)
                        {
                            int index = Items[key].Count - 1;
                            DatItem lastrom = Items[key][index];
                            if (lastrom.ItemType == ItemType.Rom)
                            {
                                (lastrom as Rom).Size += (item as Rom).Size;
                                Items[key].RemoveAt(index);
                                Items[key].Add(lastrom);
                            }

                            continue;
                        }

                        break;
                }

                item.Source = new Source(indexId, filename);
                item.CopyMachineInformation(machine);

                // Finally add each item
                key = ParseAddHelper(item, statsOnly);
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
            List<DatItem> items = new();

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
                            Size = Utilities.CleanLong(reader.GetAttribute("size")),
                            CRC = reader.GetAttribute("crc"),
                            SHA1 = reader.GetAttribute("sha1"),
                            Offset = reader.GetAttribute("offset"),
                            Value = reader.GetAttribute("value"),
                            ItemStatus = reader.GetAttribute("status").AsItemStatus(),
                            LoadFlag = reader.GetAttribute("loadflag").AsLoadFlag(),

                            DataArea = dataArea,
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
        /// <param name="diskArea">DiskArea representing the enclosing area</param>
        private List<DatItem> ReadDiskArea(XmlReader reader, DiskArea diskArea)
        {
            List<DatItem> items = new();

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
    }
}
