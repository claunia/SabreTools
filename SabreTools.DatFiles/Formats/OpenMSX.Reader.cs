using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing an openMSX softawre list XML DAT
    /// </summary>
    internal partial class OpenMSX : DatFile
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
                        case "softwaredb":
                            Header.Name ??= "openMSX Software List";
                            Header.Description ??= Header.Name;
                            Header.Date ??= xtr.GetAttribute("timestamp");
                            xtr.Read();
                            break;

                        // We want to process the entire subtree of the software
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
        /// <param name="reader">XmlReader representing a machine block</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        private void ReadSoftware(XmlReader reader, bool statsOnly, string filename, int indexId)
        {
            // If we have an empty machine, skip it
            if (reader == null)
                return;

            // Otherwise, add what is possible
            reader.MoveToContent();

            int diskno = 0;
            bool containsItems = false;

            // Create a new machine
            Machine machine = new();

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
                    case "title":
                        machine.Name = reader.ReadElementContentAsString();
                        break;

                    case "genmsxid":
                        machine.GenMSXID = reader.ReadElementContentAsString();
                        break;

                    case "system":
                        machine.System = reader.ReadElementContentAsString();
                        break;

                    case "company":
                        machine.Manufacturer = reader.ReadElementContentAsString();
                        break;

                    case "year":
                        machine.Year = reader.ReadElementContentAsString();
                        break;

                    case "country":
                        machine.Country = reader.ReadElementContentAsString();
                        break;

                    case "dump":
                        containsItems = ReadDump(reader.ReadSubtree(), machine, diskno, statsOnly, filename, indexId);
                        diskno++;

                        // Skip the dump now that we've processed it
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
        /// Read dump information
        /// </summary>
        /// <param name="reader">XmlReader representing a part block</param>
        /// <param name="machine">Machine information to pass to contained items</param>
        /// <param name="diskno">Disk number to use when outputting to other DAT formats</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        private bool ReadDump(
            XmlReader reader,
            Machine machine,
            int diskno,
            bool statsOnly,

            // Standard Dat parsing
            string filename,
            int indexId)
        {
            List<DatItem> items = new();
            Original original = null;

            while (!reader.EOF)
            {
                // We only want elements
                if (reader.NodeType != XmlNodeType.Element)
                {
                    reader.Read();
                    continue;
                }

                // Get the elements from the dump
                switch (reader.Name)
                {
                    case "rom":
                        DatItem rom = ReadRom(reader.ReadSubtree(), machine, diskno, filename, indexId);
                        if (rom != null)
                            items.Add(rom);

                        // Skip the rom now that we've processed it
                        reader.Skip();
                        break;

                    case "megarom":
                        DatItem megarom = ReadMegaRom(reader.ReadSubtree(), machine, diskno, filename, indexId);
                        if (megarom != null)
                            items.Add(megarom);

                        // Skip the megarom now that we've processed it
                        reader.Skip();
                        break;

                    case "sccpluscart":
                        DatItem sccpluscart = ReadSccPlusCart(reader.ReadSubtree(), machine, diskno, filename, indexId);
                        if (sccpluscart != null)
                            items.Add(sccpluscart);

                        // Skip the sccpluscart now that we've processed it
                        reader.Skip();
                        break;

                    case "original":
                        original = new Original
                        {
                            Value = reader.GetAttribute("value").AsYesNo(),
                            Content = reader.ReadElementContentAsString()
                        };
                        break;

                    default:
                        reader.Read();
                        break;
                }
            }

            // If we have any items, loop through and add them
            foreach (DatItem item in items)
            {
                switch (item.ItemType)
                {
                    case ItemType.Rom:
                        (item as Rom).Original = original;
                        break;
                }

                item.CopyMachineInformation(machine);
                ParseAddHelper(item, statsOnly);
            }

            return items.Count > 0;
        }

        /// <summary>
        /// Read rom information
        /// </summary>
        /// <param name="reader">XmlReader representing a rom block</param>
        /// <param name="machine">Machine information to pass to contained items</param>
        /// <param name="diskno">Disk number to use when outputting to other DAT formats</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        private DatItem ReadRom(
            XmlReader reader,
            Machine machine,
            int diskno,

            // Standard Dat parsing
            string filename,
            int indexId)
        {
            string hash = string.Empty,
                offset = string.Empty,
                type = string.Empty,
                remark = string.Empty;

            while (!reader.EOF)
            {
                // We only want elements
                if (reader.NodeType != XmlNodeType.Element)
                {
                    reader.Read();
                    continue;
                }

                // Get the elements from the rom
                switch (reader.Name)
                {
                    case "hash":
                        hash = reader.ReadElementContentAsString();
                        break;

                    case "start":
                        offset = reader.ReadElementContentAsString();
                        break;

                    case "type":
                        type = reader.ReadElementContentAsString();
                        break;

                    case "remark":
                        remark = reader.ReadElementContentAsString();
                        break;

                    default:
                        reader.Read();
                        break;
                }
            }

            // If we got a hash, then create and return the item
            if (!string.IsNullOrWhiteSpace(hash))
            {
                return new Rom
                {
                    Name = machine.Name + "_" + diskno + (!string.IsNullOrWhiteSpace(remark) ? " " + remark : string.Empty),
                    Offset = offset,
                    Size = null,
                    SHA1 = hash,

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },

                    OpenMSXSubType = OpenMSXSubType.Rom,
                    OpenMSXType = type,
                    Remark = remark,
                };
            }

            // No valid item means returning null
            return null;
        }

        /// <summary>
        /// Read megarom information
        /// </summary>
        /// <param name="reader">XmlReader representing a megarom block</param>
        /// <param name="machine">Machine information to pass to contained items</param>
        /// <param name="diskno">Disk number to use when outputting to other DAT formats</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        private DatItem ReadMegaRom(
            XmlReader reader,
            Machine machine,
            int diskno,

            // Standard Dat parsing
            string filename,
            int indexId)
        {
            string hash = string.Empty,
                offset = string.Empty,
                type = string.Empty,
                remark = string.Empty;

            while (!reader.EOF)
            {
                // We only want elements
                if (reader.NodeType != XmlNodeType.Element)
                {
                    reader.Read();
                    continue;
                }

                // Get the elements from the dump
                switch (reader.Name)
                {
                    case "hash":
                        hash = reader.ReadElementContentAsString();
                        break;

                    case "start":
                        offset = reader.ReadElementContentAsString();
                        break;

                    case "type":
                        reader.ReadElementContentAsString();
                        break;

                    case "remark":
                        remark = reader.ReadElementContentAsString();
                        break;

                    default:
                        reader.Read();
                        break;
                }
            }

            // If we got a hash, then create and return the item
            if (!string.IsNullOrWhiteSpace(hash))
            {
                return new Rom
                {
                    Name = machine.Name + "_" + diskno + (!string.IsNullOrWhiteSpace(remark) ? " " + remark : string.Empty),
                    Offset = offset,
                    Size = null,
                    SHA1 = hash,

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },

                    OpenMSXSubType = OpenMSXSubType.MegaRom,
                    OpenMSXType = type,
                    Remark = remark,
                };
            }

            // No valid item means returning null
            return null;
        }

        /// <summary>
        /// Read sccpluscart information
        /// </summary>
        /// <param name="reader">XmlReader representing a sccpluscart block</param>
        /// <param name="machine">Machine information to pass to contained items</param>
        /// <param name="diskno">Disk number to use when outputting to other DAT formats</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        private DatItem ReadSccPlusCart(
            XmlReader reader,
            Machine machine,
            int diskno,

            // Standard Dat parsing
            string filename,
            int indexId)
        {
            string boot = string.Empty,
                hash = string.Empty,
                remark = string.Empty;

            while (!reader.EOF)
            {
                // We only want elements
                if (reader.NodeType != XmlNodeType.Element)
                {
                    reader.Read();
                    continue;
                }

                // Get the elements from the dump
                switch (reader.Name)
                {
                    case "boot":
                        boot = reader.ReadElementContentAsString();
                        break;

                    case "hash":
                        hash = reader.ReadElementContentAsString();
                        break;

                    case "remark":
                        remark = reader.ReadElementContentAsString();
                        break;

                    default:
                        reader.Read();
                        break;
                }
            }

            // If we got a hash, then create and return the item
            if (!string.IsNullOrWhiteSpace(hash))
            {
                return new Rom
                {
                    Name = machine.Name + "_" + diskno + (!string.IsNullOrWhiteSpace(remark) ? " " + remark : string.Empty),
                    Size = null,
                    SHA1 = hash,

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },

                    OpenMSXSubType = OpenMSXSubType.SCCPlusCart,
                    Boot = boot,
                    Remark = remark,
                };
            }

            // No valid item means returning null
            return null;
        }
    }
}
