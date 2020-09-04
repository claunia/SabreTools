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

namespace SabreTools.Library.DatFiles
{
    /// <summary>
    /// Represents parsing and writing of an SabreDat XML DAT
    /// </summary>
    internal class SabreDat : DatFile
    {
        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public SabreDat(DatFile datFile)
            : base(datFile)
        {
        }

        /// <summary>
        /// Parse an SabreDat XML DAT and return all found directories and files within
        /// </summary>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        protected override void ParseFile(string filename, int indexId, bool keep)
        {
            // Prepare all internal variables
            bool empty = true;
            string key;
            List<string> parent = new List<string>();

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
                    // If we're ending a folder or game, take care of possibly empty games and removing from the parent
                    if (xtr.NodeType == XmlNodeType.EndElement && (xtr.Name == "directory" || xtr.Name == "dir"))
                    {
                        // If we didn't find any items in the folder, make sure to add the blank rom
                        if (empty)
                        {
                            string tempgame = string.Join("\\", parent);
                            Rom rom = new Rom("null", tempgame);

                            // Now process and add the rom
                            key = ParseAddHelper(rom);
                        }

                        // Regardless, end the current folder
                        int parentcount = parent.Count;
                        if (parentcount == 0)
                        {
                            Globals.Logger.Verbose($"Empty parent '{string.Join("\\", parent)}' found in '{filename}'");
                            empty = true;
                        }

                        // If we have an end folder element, remove one item from the parent, if possible
                        if (parentcount > 0)
                        {
                            parent.RemoveAt(parent.Count - 1);
                            if (keep && parentcount > 1)
                                Header.Type = (string.IsNullOrWhiteSpace(Header.Type) ? "SuperDAT" : Header.Type);
                        }
                    }

                    // We only want elements
                    if (xtr.NodeType != XmlNodeType.Element)
                    {
                        xtr.Read();
                        continue;
                    }

                    switch (xtr.Name)
                    {
                        // We want to process the entire subtree of the header
                        case "header":
                            ReadHeader(xtr.ReadSubtree(), keep);

                            // Skip the header node now that we've processed it
                            xtr.Skip();
                            break;

                        case "dir":
                        case "directory":
                            empty = ReadDirectory(xtr.ReadSubtree(), parent, filename, indexId, keep);

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
                Globals.Logger.Warning($"Exception found while parsing '{filename}': {ex}");

                // For XML errors, just skip the affected node
                xtr?.Read();
            }

            xtr.Dispose();
        }

        /// <summary>
        /// Read header information
        /// </summary>
        /// <param name="reader">XmlReader to use to parse the header</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        private void ReadHeader(XmlReader reader, bool keep)
        {
            bool superdat = false;

            // If there's no subtree to the header, skip it
            if (reader == null)
                return;

            // Otherwise, read what we can from the header
            while (!reader.EOF)
            {
                // We only want elements
                if (reader.NodeType != XmlNodeType.Element || reader.Name == "header")
                {
                    reader.Read();
                    continue;
                }

                // Get all header items (ONLY OVERWRITE IF THERE'S NO DATA)
                string content;
                switch (reader.Name)
                {
                    case "name":
                        content = reader.ReadElementContentAsString(); ;
                        Header.Name = (Header.Name== null ? content : Header.Name);
                        superdat = superdat || content.Contains(" - SuperDAT");
                        if (keep && superdat)
                        {
                            Header.Type = (Header.Type == null ? "SuperDAT" : Header.Type);
                        }
                        break;

                    case "description":
                        content = reader.ReadElementContentAsString();
                        Header.Description = (Header.Description== null ? content : Header.Description);
                        break;

                    case "rootdir":
                        content = reader.ReadElementContentAsString();
                        Header.RootDir = (Header.RootDir== null ? content : Header.RootDir);
                        break;

                    case "category":
                        content = reader.ReadElementContentAsString();
                        Header.Category = (Header.Category== null ? content : Header.Category);
                        break;

                    case "version":
                        content = reader.ReadElementContentAsString();
                        Header.Version = (Header.Version== null ? content : Header.Version);
                        break;

                    case "date":
                        content = reader.ReadElementContentAsString();
                        Header.Date = (Header.Date== null ? content.Replace(".", "/") : Header.Date);
                        break;

                    case "author":
                        content = reader.ReadElementContentAsString();
                        Header.Author = (Header.Author== null ? content : Header.Author);
                        Header.Email = (Header.Email == null ? reader.GetAttribute("email") : Header.Email);
                        Header.Homepage = (Header.Homepage == null ? reader.GetAttribute("homepage") : Header.Homepage);
                        Header.Url = (Header.Url == null ? reader.GetAttribute("url") : Header.Url);
                        break;

                    case "comment":
                        content = reader.ReadElementContentAsString();
                        Header.Comment = (Header.Comment== null ? content : Header.Comment);
                        break;

                    case "flags":
                        ReadFlags(reader.ReadSubtree(), superdat);

                        // Skip the flags node now that we've processed it
                        reader.Skip();
                        break;

                    default:
                        reader.Read();
                        break;
                }
            }
        }

        /// <summary>
        /// Read directory information
        /// </summary>
        /// <param name="reader">XmlReader to use to parse the header</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        /// TODO: This is horrendeously out of date. Once all done promoting, try to make this like JSON
        private bool ReadDirectory(
            XmlReader reader,
            List<string> parent,

            // Standard Dat parsing
            string filename,
            int indexId,

            // Miscellaneous
            bool keep)
        {
            // Prepare all internal variables
            XmlReader flagreader;
            bool empty = true;
            string key = string.Empty, date = string.Empty;
            long size = -1;
            ItemStatus its = ItemStatus.None;

            // If there's no subtree to the header, skip it
            if (reader == null)
                return empty;

            string foldername = (reader.GetAttribute("name") ?? string.Empty);
            if (!string.IsNullOrWhiteSpace(foldername))
                parent.Add(foldername);

            // Otherwise, read what we can from the directory
            while (!reader.EOF)
            {
                // If we're ending a folder or game, take care of possibly empty games and removing from the parent
                if (reader.NodeType == XmlNodeType.EndElement && (reader.Name == "directory" || reader.Name == "dir"))
                {
                    // If we didn't find any items in the folder, make sure to add the blank rom
                    if (empty)
                    {
                        string tempgame = string.Join("\\", parent);
                        Rom rom = new Rom("null", tempgame);

                        // Now process and add the rom
                        key = ParseAddHelper(rom);
                    }

                    // Regardless, end the current folder
                    int parentcount = parent.Count;
                    if (parentcount == 0)
                    {
                        Globals.Logger.Verbose($"Empty parent '{string.Join("\\", parent)}' found in '{filename}'");
                        empty = true;
                    }

                    // If we have an end folder element, remove one item from the parent, if possible
                    if (parentcount > 0)
                    {
                        parent.RemoveAt(parent.Count - 1);
                        if (keep && parentcount > 1)
                            Header.Type = (string.IsNullOrWhiteSpace(Header.Type) ? "SuperDAT" : Header.Type);
                    }
                }

                // We only want elements
                if (reader.NodeType != XmlNodeType.Element)
                {
                    reader.Read();
                    continue;
                }

                // Get all directory items
                string content = string.Empty;
                switch (reader.Name)
                {
                    // Directories can contain directories
                    case "dir":
                    case "directory":
                        ReadDirectory(reader.ReadSubtree(), parent, filename, indexId, keep);

                        // Skip the directory node now that we've processed it
                        reader.Read();
                        break;

                    case "file":
                        empty = false;

                        // If the rom is itemStatus, flag it
                        its = ItemStatus.None;
                        flagreader = reader.ReadSubtree();

                        // If the subtree is empty, skip it
                        if (flagreader == null)
                        {
                            reader.Skip();
                            continue;
                        }

                        while (!flagreader.EOF)
                        {
                            // We only want elements
                            if (flagreader.NodeType != XmlNodeType.Element || flagreader.Name == "flags")
                            {
                                flagreader.Read();
                                continue;
                            }

                            switch (flagreader.Name)
                            {
                                case "flag":
                                    if (flagreader.GetAttribute("name") != null && flagreader.GetAttribute("value") != null)
                                    {
                                        content = flagreader.GetAttribute("value");
                                        its = flagreader.GetAttribute("name").AsItemStatus();
                                    }
                                    break;
                            }

                            flagreader.Read();
                        }

                        // If the rom has a Date attached, read it in and then sanitize it
                        date = Sanitizer.CleanDate(reader.GetAttribute("date"));

                        // Take care of hex-sized files
                        size = Sanitizer.CleanSize(reader.GetAttribute("size"));

                        Machine dir = new Machine
                        {
                            // Get the name of the game from the parent
                            Name = string.Join("\\", parent),
                            Description = string.Join("\\", parent),
                        };

                        DatItem datItem;
                        switch (reader.GetAttribute("type").ToLowerInvariant())
                        {
                            case "adjuster":
                                datItem = new Adjuster
                                {
                                    Name = reader.GetAttribute("name"),
                                    Default = reader.GetAttribute("default").AsYesNo(),
                                    Conditions = new List<Condition>(),
                                };

                                // Now read the internal tags
                                ReadAdjuster(reader.ReadSubtree(), datItem);

                                // Skip the adjuster now that we've processed it
                                reader.Skip();
                                break;

                            case "archive":
                                datItem = new Archive
                                {
                                    Name = reader.GetAttribute("name"),

                                    Source = new Source
                                    {
                                        Index = indexId,
                                        Name = filename,
                                    },
                                };
                                break;

                            case "biosset":
                                datItem = new BiosSet
                                {
                                    Name = reader.GetAttribute("name"),
                                    Description = reader.GetAttribute("description"),
                                    Default = reader.GetAttribute("default").AsYesNo(),

                                    Source = new Source
                                    {
                                        Index = indexId,
                                        Name = filename,
                                    },
                                };
                                break;

                            case "chip":
                                datItem = new Chip
                                {
                                    Name = reader.GetAttribute("name"),
                                    Tag = reader.GetAttribute("tag"),
                                    ChipType = reader.GetAttribute("chiptype").AsChipType(),
                                    Clock = reader.GetAttribute("clock"),

                                    Source = new Source
                                    {
                                        Index = indexId,
                                        Name = filename,
                                    },
                                };
                                break;

                            case "configuration":
                                datItem = new Configuration
                                {
                                    Name = reader.GetAttribute("name"),
                                    Tag = reader.GetAttribute("tag"),
                                    Mask = reader.GetAttribute("mask"),
                                    Conditions = new List<Condition>(),
                                    Locations = new List<Location>(),
                                    Settings = new List<Setting>(),
                                };

                                // Now read the internal tags
                                ReadConfiguration(reader.ReadSubtree(), datItem);

                                // Skip the configuration now that we've processed it
                                reader.Skip();
                                break;

                            case "device_ref":
                                datItem = new DeviceReference
                                {
                                    Name = reader.GetAttribute("name"),
                                };

                                reader.Read();
                                break;

                            case "dipswitch":
                                datItem = new DipSwitch
                                {
                                    Name = reader.GetAttribute("name"),
                                    Tag = reader.GetAttribute("tag"),
                                    Mask = reader.GetAttribute("mask"),
                                    Conditions = new List<Condition>(),
                                    Locations = new List<Location>(),
                                    Values = new List<Setting>(),
                                };

                                // Now read the internal tags
                                ReadDipSwitch(reader.ReadSubtree(), datItem);

                                // Skip the dipswitch now that we've processed it
                                reader.Skip();
                                break;

                            case "disk":
                                datItem = new Disk
                                {
                                    Name = reader.GetAttribute("name"),
                                    MD5 = reader.GetAttribute("md5"),
                                    SHA1 = reader.GetAttribute("sha1"),
                                    ItemStatus = its,

                                    Source = new Source
                                    {
                                        Index = indexId,
                                        Name = filename,
                                    },
                                };
                                break;

                            case "media":
                                datItem = new Media
                                {
                                    Name = reader.GetAttribute("name"),
                                    MD5 = reader.GetAttribute("md5"),
                                    SHA1 = reader.GetAttribute("sha1"),
                                    SHA256 = reader.GetAttribute("sha256"),

                                    Source = new Source
                                    {
                                        Index = indexId,
                                        Name = filename,
                                    },
                                };
                                break;

                            case "release":
                                datItem = new Release
                                {
                                    Name = reader.GetAttribute("name"),
                                    Region = reader.GetAttribute("region"),
                                    Language = reader.GetAttribute("language"),
                                    Date = reader.GetAttribute("date"),
                                    Default = reader.GetAttribute("default").AsYesNo(),

                                    Source = new Source
                                    {
                                        Index = indexId,
                                        Name = filename,
                                    },
                                };
                                break;

                            case "rom":
                                datItem = new Rom
                                {
                                    Name = reader.GetAttribute("name"),
                                    Size = size,
                                    CRC = reader.GetAttribute("crc"),
                                    MD5 = reader.GetAttribute("md5"),
#if NET_FRAMEWORK
                                    RIPEMD160 = reader.GetAttribute("ripemd160"),
#endif
                                    SHA1 = reader.GetAttribute("sha1"),
                                    SHA256 = reader.GetAttribute("sha256"),
                                    SHA384 = reader.GetAttribute("sha384"),
                                    SHA512 = reader.GetAttribute("sha512"),
                                    ItemStatus = its,
                                    Date = date,

                                    Source = new Source
                                    {
                                        Index = indexId,
                                        Name = filename,
                                    },
                                };
                                break;

                            case "sample":
                                datItem = new Sample
                                {
                                    Name = reader.GetAttribute("name"),

                                    Source = new Source
                                    {
                                        Index = indexId,
                                        Name = filename,
                                    },
                                };
                                break;

                            default:
                                // By default, create a new Blank, just in case
                                datItem = new Blank();
                                break;
                        }

                        datItem?.CopyMachineInformation(dir);

                        // Now process and add the rom
                        key = ParseAddHelper(datItem);

                        reader.Read();
                        break;
                }
            }

            return empty;
        }

        /// <summary>
        /// Read flags information
        /// </summary>
        /// <param name="reader">XmlReader to use to parse the header</param>
        /// <param name="superdat">True if superdat has already been set externally, false otherwise</param>
        private void ReadFlags(XmlReader reader, bool superdat)
        {
            // Prepare all internal variables
            string content;

            // If we somehow have a null flag section, skip it
            if (reader == null)
                return;

            while (!reader.EOF)
            {
                // We only want elements
                if (reader.NodeType != XmlNodeType.Element || reader.Name == "flags")
                {
                    reader.Read();
                    continue;
                }

                switch (reader.Name)
                {
                    case "flag":
                        if (reader.GetAttribute("name") != null && reader.GetAttribute("value") != null)
                        {
                            content = reader.GetAttribute("value");
                            switch (reader.GetAttribute("name").ToLowerInvariant())
                            {
                                case "type":
                                    Header.Type = (Header.Type == null ? content : Header.Type);
                                    superdat = superdat || content.Contains("SuperDAT");
                                    break;

                                case "forcemerging":
                                    if (Header.ForceMerging == MergingFlag.None)
                                        Header.ForceMerging = content.AsMergingFlag();

                                    break;

                                case "forcenodump":
                                    if (Header.ForceNodump == NodumpFlag.None)
                                        Header.ForceNodump = content.AsNodumpFlag();

                                    break;

                                case "forcepacking":
                                    if (Header.ForcePacking == PackingFlag.None)
                                        Header.ForcePacking = content.AsPackingFlag();

                                    break;
                            }
                        }

                        reader.Read();
                        break;

                    default:
                        reader.Read();
                        break;
                }
            }
        }

        /// <summary>
        /// Read Adjuster information
        /// </summary>
        /// <param name="reader">XmlReader representing a diskarea block</param>
        /// <param name="adjuster">Adjuster to populate</param>
        private void ReadAdjuster(XmlReader reader, DatItem adjuster)
        {
            // If we have an empty port, skip it
            if (reader == null)
                return;

            // If the DatItem isn't an Adjuster, skip it
            if (adjuster.ItemType != ItemType.Adjuster)
                return;

            // Get list ready
            (adjuster as Adjuster).Conditions = new List<Condition>();

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

                // Get the information from the adjuster
                switch (reader.Name)
                {
                    case "condition":
                        var condition = new Condition();
                        condition.Tag = reader.GetAttribute("tag");
                        condition.Mask = reader.GetAttribute("mask");
                        condition.Relation = reader.GetAttribute("relation").AsRelation();
                        condition.Value = reader.GetAttribute("value");

                        (adjuster as Adjuster).Conditions.Add(condition);

                        reader.Read();
                        break;

                    default:
                        reader.Read();
                        break;
                }
            }
        }

        /// <summary>
        /// Read Configuration information
        /// </summary>
        /// <param name="reader">XmlReader representing a diskarea block</param>
        /// <param name="configuration">Configuration to populate</param>
        private void ReadConfiguration(XmlReader reader, DatItem configuration)
        {
            // If we have an empty configuration, skip it
            if (reader == null)
                return;

            // If the DatItem isn't an Configuration, skip it
            if (configuration.ItemType != ItemType.Configuration)
                return;

            // Get lists ready
            (configuration as Configuration).Conditions = new List<Condition>();
            (configuration as Configuration).Locations = new List<Location>();
            (configuration as Configuration).Settings = new List<Setting>();

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
                    case "condition":
                        var condition = new Condition();
                        condition.Tag = reader.GetAttribute("tag");
                        condition.Mask = reader.GetAttribute("mask");
                        condition.Relation = reader.GetAttribute("relation").AsRelation();
                        condition.Value = reader.GetAttribute("value");

                        (configuration as Configuration).Conditions.Add(condition);

                        reader.Read();
                        break;

                    case "conflocation":
                        var confLocation = new Location();
                        confLocation.Name = reader.GetAttribute("name");
                        confLocation.Number = reader.GetAttribute("number");
                        confLocation.Inverted = reader.GetAttribute("inverted").AsYesNo();

                        (configuration as Configuration).Locations.Add(confLocation);

                        reader.Read();
                        break;

                    case "confsetting":
                        var confSetting = new Setting();
                        confSetting.Name = reader.GetAttribute("name");
                        confSetting.Value = reader.GetAttribute("value");
                        confSetting.Default = reader.GetAttribute("default").AsYesNo();

                        // Now read the internal tags
                        ReadConfSetting(reader, confSetting);

                        (configuration as Configuration).Settings.Add(confSetting);

                        // Skip the dipvalue now that we've processed it
                        reader.Read();
                        break;

                    default:
                        reader.Read();
                        break;
                }
            }
        }

        /// <summary>
        /// Read ConfSetting information
        /// </summary>
        /// <param name="reader">XmlReader representing a diskarea block</param>
        /// <param name="confSetting">ListXmlConfSetting to populate</param>
        private void ReadConfSetting(XmlReader reader, Setting confSetting)
        {
            // If we have an empty confsetting, skip it
            if (reader == null)
                return;

            // Get list ready
            confSetting.Conditions = new List<Condition>();

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

                // Get the information from the confsetting
                switch (reader.Name)
                {
                    case "condition":
                        var condition = new Condition();
                        condition.Tag = reader.GetAttribute("tag");
                        condition.Mask = reader.GetAttribute("mask");
                        condition.Relation = reader.GetAttribute("relation").AsRelation();
                        condition.Value = reader.GetAttribute("value");

                        confSetting.Conditions.Add(condition);

                        reader.Read();
                        break;

                    default:
                        reader.Read();
                        break;
                }
            }
        }

        /// <summary>
        /// Read DipSwitch information
        /// </summary>
        /// <param name="reader">XmlReader representing a diskarea block</param>
        /// <param name="dipSwitch">DipSwitch to populate</param>
        private void ReadDipSwitch(XmlReader reader, DatItem dipSwitch)
        {
            // If we have an empty dipswitch, skip it
            if (reader == null)
                return;

            // If the DatItem isn't an DipSwitch, skip it
            if (dipSwitch.ItemType != ItemType.DipSwitch)
                return;

            // Get lists ready
            (dipSwitch as DipSwitch).Conditions = new List<Condition>();
            (dipSwitch as DipSwitch).Locations = new List<Location>();
            (dipSwitch as DipSwitch).Values = new List<Setting>();

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
                    case "condition":
                        var condition = new Condition();
                        condition.Tag = reader.GetAttribute("tag");
                        condition.Mask = reader.GetAttribute("mask");
                        condition.Relation = reader.GetAttribute("relation").AsRelation();
                        condition.Value = reader.GetAttribute("value");

                        (dipSwitch as DipSwitch).Conditions.Add(condition);

                        reader.Read();
                        break;

                    case "diplocation":
                        var dipLocation = new Location();
                        dipLocation.Name = reader.GetAttribute("name");
                        dipLocation.Number = reader.GetAttribute("number");
                        dipLocation.Inverted = reader.GetAttribute("inverted").AsYesNo();

                        (dipSwitch as DipSwitch).Locations.Add(dipLocation);

                        reader.Read();
                        break;

                    case "dipvalue":
                        var dipValue = new Setting();
                        dipValue.Name = reader.GetAttribute("name");
                        dipValue.Value = reader.GetAttribute("value");
                        dipValue.Default = reader.GetAttribute("default").AsYesNo();

                        // Now read the internal tags
                        ReadDipValue(reader, dipValue);

                        (dipSwitch as DipSwitch).Values.Add(dipValue);

                        // Skip the dipvalue now that we've processed it
                        reader.Read();
                        break;

                    default:
                        reader.Read();
                        break;
                }
            }
        }

        /// <summary>
        /// Read DipValue information
        /// </summary>
        /// <param name="reader">XmlReader representing a diskarea block</param>
        /// <param name="dipValue">Setting to populate</param>
        private void ReadDipValue(XmlReader reader, Setting dipValue)
        {
            // If we have an empty dipvalue, skip it
            if (reader == null)
                return;

            // Get list ready
            dipValue.Conditions = new List<Condition>();

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

                // Get the information from the dipvalue
                switch (reader.Name)
                {
                    case "condition":
                        var condition = new Condition();
                        condition.Tag = reader.GetAttribute("tag");
                        condition.Mask = reader.GetAttribute("mask");
                        condition.Relation = reader.GetAttribute("relation").AsRelation();
                        condition.Value = reader.GetAttribute("value");

                        dipValue.Conditions.Add(condition);

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
        /// TODO: Fix writing out files that have a path in the name
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
                int depth = 2, last = -1;
                string lastgame = null;
                List<string> splitpath = new List<string>();

                // Use a sorted list of games to output
                foreach (string key in Items.SortedKeys)
                {
                    List<DatItem> datItems = Items.FilteredItems(key);

                    // Resolve the names in the block
                    datItems = DatItem.ResolveNames(datItems);

                    for (int index = 0; index < datItems.Count; index++)
                    {
                        DatItem datItem = datItems[index];

                        List<string> newsplit = datItem.Machine.Name.Split('\\').ToList();

                        // If we have a different game and we're not at the start of the list, output the end of last item
                        if (lastgame != null && lastgame.ToLowerInvariant() != datItem.Machine.Name.ToLowerInvariant())
                            depth = WriteEndGame(xtw, splitpath, newsplit, depth, out last);

                        // If we have a new game, output the beginning of the new item
                        if (lastgame == null || lastgame.ToLowerInvariant() != datItem.Machine.Name.ToLowerInvariant())
                            depth = WriteStartGame(xtw, datItem, newsplit, depth, last);

                        // Check for a "null" item
                        datItem = ProcessNullifiedItem(datItem);

                        // Write out the item if we're not ignoring
                        if (!ShouldIgnore(datItem, ignoreblanks))
                            WriteDatItem(xtw, datItem, depth);

                        // Set the new data to compare against
                        splitpath = newsplit;
                        lastgame = datItem.Machine.Name;
                    }
                }

                // Write the file footer out
                WriteFooter(xtw, depth);

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
                xtw.WriteDocType("sabredat", null, "newdat.xsd", null);

                xtw.WriteStartElement("datafile");

                xtw.WriteStartElement("header");

                xtw.WriteRequiredElementString("name", Header.Name);
                xtw.WriteRequiredElementString("description", Header.Description);
                xtw.WriteOptionalElementString("rootdir", Header.RootDir);
                xtw.WriteOptionalElementString("category", Header.Category);
                xtw.WriteRequiredElementString("version", Header.Version);
                xtw.WriteOptionalElementString("date", Header.Date);
                xtw.WriteRequiredElementString("author", Header.Author);
                xtw.WriteOptionalElementString("comment", Header.Comment);
                if (!string.IsNullOrWhiteSpace(Header.Type)
                    || Header.ForcePacking != PackingFlag.None
                    || Header.ForceMerging != MergingFlag.None
                    || Header.ForceNodump != NodumpFlag.None)
                {
                    xtw.WriteStartElement("flags");

                    if (!string.IsNullOrWhiteSpace(Header.Type))
                    {
                        xtw.WriteStartElement("flag");
                        xtw.WriteAttributeString("name", "type");
                        xtw.WriteRequiredAttributeString("value", Header.Type);
                        xtw.WriteEndElement();
                    }

                    if (Header.ForcePacking != PackingFlag.None)
                    {
                        xtw.WriteStartElement("flag");
                        xtw.WriteAttributeString("name", "forcepacking");
                        xtw.WriteOptionalAttributeString("value", Header.ForcePacking.FromPackingFlag(false));
                        xtw.WriteEndElement();
                    }

                    if (Header.ForceMerging != MergingFlag.None)
                    {
                        xtw.WriteStartElement("flag");
                        xtw.WriteAttributeString("name", "forcemerging");
                        xtw.WriteAttributeString("value", Header.ForceMerging.FromMergingFlag(false));
                        xtw.WriteEndElement();
                    }

                    if (Header.ForceNodump != NodumpFlag.None)
                    {
                        xtw.WriteStartElement("flag");
                        xtw.WriteAttributeString("name", "forcenodump");
                        xtw.WriteAttributeString("value", Header.ForceNodump.FromNodumpFlag());
                        xtw.WriteEndElement();
                    }

                    // End flags
                    xtw.WriteEndElement();
                }

                // End header
                xtw.WriteEndElement();

                xtw.WriteStartElement("data");

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
        /// <param name="newsplit">Split path representing the parent game (SabreDAT only)</param>
        /// <param name="depth">Current depth to output file at (SabreDAT only)</param>
        /// <param name="last">Last known depth to cycle back from (SabreDAT only)</param>
        /// <returns>The new depth of the tag</returns>
        private int WriteStartGame(XmlTextWriter xtw, DatItem datItem, List<string> newsplit, int depth, int last)
        {
            try
            {
                // No game should start with a path separator
                datItem.Machine.Name = datItem.Machine.Name?.TrimStart(Path.DirectorySeparatorChar) ?? string.Empty;

                // Build the state
                for (int i = (last == -1 ? 0 : last); i < newsplit.Count; i++)
                {
                    xtw.WriteStartElement("directory");
                    xtw.WriteRequiredAttributeString("name", newsplit[i]);
                    xtw.WriteRequiredAttributeString("description", newsplit[i]);
                }

                depth = depth - (last == -1 ? 0 : last) + newsplit.Count;

                xtw.Flush();
            }
            catch (Exception ex)
            {
                Globals.Logger.Error(ex.ToString());
                return depth;
            }

            return depth;
        }

        /// <summary>
        /// Write out Game start using the supplied StreamWriter
        /// </summary>
        /// <param name="xtw">XmlTextWriter to output to</param>
        /// <param name="splitpath">Split path representing last kwown parent game (SabreDAT only)</param>
        /// <param name="newsplit">Split path representing the parent game (SabreDAT only)</param>
        /// <param name="depth">Current depth to output file at (SabreDAT only)</param>
        /// <param name="last">Last known depth to cycle back from (SabreDAT only)</param>
        /// <returns>The new depth of the tag</returns>
        private int WriteEndGame(XmlTextWriter xtw, List<string> splitpath, List<string> newsplit, int depth, out int last)
        {
            last = 0;

            try
            {
                if (splitpath != null)
                {
                    for (int i = 0; i < newsplit.Count && i < splitpath.Count; i++)
                    {
                        // Always keep track of the last seen item
                        last = i;

                        // If we find a difference, break
                        if (newsplit[i] != splitpath[i])
                            break;
                    }

                    // Now that we have the last known position, take down all open folders
                    for (int i = depth - 1; i > last + 1; i--)
                    {
                        // End directory
                        xtw.WriteEndElement();
                    }

                    // Reset the current depth
                    depth = 2 + last;
                }

                xtw.Flush();
            }
            catch (Exception ex)
            {
                Globals.Logger.Error(ex.ToString());
                return depth;
            }

            return depth;
        }

        /// <summary>
        /// Write out DatItem using the supplied StreamWriter
        /// </summary>
        /// <param name="xtw">XmlTextWriter to output to</param>
        /// <param name="datItem">DatItem object to be output</param>
        /// <param name="depth">Current depth to output file at (SabreDAT only)</param>
        /// <returns>True if the data was written, false on error</returns>
        private bool WriteDatItem(XmlTextWriter xtw, DatItem datItem, int depth)
        {
            try
            {
                // Pre-process the item name
                ProcessItemName(datItem, true);

                // Build the state
                switch (datItem.ItemType)
                {
                    case ItemType.Adjuster:
                        var adjuster = datItem as Adjuster;
                        xtw.WriteStartElement("file");
                        xtw.WriteAttributeString("type", "adjuster");
                        xtw.WriteRequiredAttributeString("name", adjuster.Name);
                        xtw.WriteOptionalAttributeString("default", adjuster.Default.FromYesNo());
                        if (adjuster.Conditions != null)
                        {
                            foreach (var adjusterCondition in adjuster.Conditions)
                            {
                                xtw.WriteStartElement("condition");
                                xtw.WriteOptionalAttributeString("tag", adjusterCondition.Tag);
                                xtw.WriteOptionalAttributeString("mask", adjusterCondition.Mask);
                                xtw.WriteOptionalAttributeString("relation", adjusterCondition.Relation.FromRelation());
                                xtw.WriteOptionalAttributeString("value", adjusterCondition.Value);
                                xtw.WriteEndElement();
                            }
                        }
                        xtw.WriteEndElement();
                        break;

                    case ItemType.Archive:
                        var archive = datItem as Archive;
                        xtw.WriteStartElement("file");
                        xtw.WriteAttributeString("type", "archive");
                        xtw.WriteRequiredAttributeString("name", archive.Name);
                        xtw.WriteEndElement();
                        break;

                    case ItemType.BiosSet:
                        var biosSet = datItem as BiosSet;
                        xtw.WriteStartElement("file");
                        xtw.WriteAttributeString("type", "biosset");
                        xtw.WriteRequiredAttributeString("name", biosSet.Name);
                        xtw.WriteOptionalAttributeString("description", biosSet.Description);
                        xtw.WriteOptionalAttributeString("default", biosSet.Default.FromYesNo());
                        xtw.WriteEndElement();
                        break;

                    case ItemType.Chip:
                        var chip = datItem as Chip;
                        xtw.WriteStartElement("file");
                        xtw.WriteAttributeString("type", "chip");
                        xtw.WriteRequiredAttributeString("name", chip.Name);
                        xtw.WriteOptionalAttributeString("tag", chip.Tag);
                        xtw.WriteOptionalAttributeString("chiptype", chip.ChipType.FromChipType());
                        xtw.WriteOptionalAttributeString("clock", chip.Clock);
                        xtw.WriteEndElement();
                        break;

                    case ItemType.Condition:
                        var condition = datItem as Condition;
                        xtw.WriteStartElement("file");
                        xtw.WriteAttributeString("type", "condition");
                        xtw.WriteOptionalAttributeString("tag", condition.Tag);
                        xtw.WriteOptionalAttributeString("mask", condition.Mask);
                        xtw.WriteOptionalAttributeString("relation", condition.Relation.FromRelation());
                        xtw.WriteOptionalAttributeString("value", condition.Value);
                        xtw.WriteEndElement();
                        break;

                    case ItemType.Configuration:
                        var configuration = datItem as Configuration;
                        xtw.WriteStartElement("file");
                        xtw.WriteAttributeString("type", "configuration");
                        xtw.WriteOptionalAttributeString("name", configuration.Name);
                        xtw.WriteOptionalAttributeString("tag", configuration.Tag);
                        xtw.WriteOptionalAttributeString("mask", configuration.Mask);

                        if (configuration.Conditions != null)
                        {
                            foreach (var configurationCondition in configuration.Conditions)
                            {
                                xtw.WriteStartElement("condition");
                                xtw.WriteOptionalAttributeString("tag", configurationCondition.Tag);
                                xtw.WriteOptionalAttributeString("mask", configurationCondition.Mask);
                                xtw.WriteOptionalAttributeString("relation", configurationCondition.Relation.FromRelation());
                                xtw.WriteOptionalAttributeString("value", configurationCondition.Value);
                                xtw.WriteEndElement();
                            }
                        }
                        if (configuration.Locations != null)
                        {
                            foreach (var location in configuration.Locations)
                            {
                                xtw.WriteStartElement("conflocation");
                                xtw.WriteOptionalAttributeString("name", location.Name);
                                xtw.WriteOptionalAttributeString("number", location.Number);
                                xtw.WriteOptionalAttributeString("inverted", location.Inverted.FromYesNo());
                                xtw.WriteEndElement();
                            }
                        }
                        if (configuration.Settings != null)
                        {
                            foreach (var setting in configuration.Settings)
                            {
                                xtw.WriteStartElement("confsetting");
                                xtw.WriteOptionalAttributeString("name", setting.Name);
                                xtw.WriteOptionalAttributeString("value", setting.Value);
                                xtw.WriteOptionalAttributeString("default", setting.Default.FromYesNo());
                                xtw.WriteEndElement();
                            }
                        }
                        xtw.WriteEndElement();
                        break;

                    case ItemType.Device:
                        var device = datItem as Device;
                        xtw.WriteStartElement("file");
                        xtw.WriteAttributeString("type", "device");
                        xtw.WriteOptionalAttributeString("type", device.DeviceType);
                        xtw.WriteOptionalAttributeString("tag", device.Tag);
                        xtw.WriteOptionalAttributeString("fixed_image", device.FixedImage);
                        xtw.WriteOptionalAttributeString("mandatory", device.Mandatory);
                        xtw.WriteOptionalAttributeString("interface", device.Interface);
                        if (device.Instances != null)
                        {
                            foreach (var instance in device.Instances)
                            {
                                xtw.WriteStartElement("instance");
                                xtw.WriteOptionalAttributeString("name", instance.Name);
                                xtw.WriteOptionalAttributeString("briefname", instance.BriefName);
                                xtw.WriteEndElement();
                            }
                        }
                        if (device.Extensions != null)
                        {
                            foreach (var extension in device.Extensions)
                            {
                                xtw.WriteStartElement("extension");
                                xtw.WriteOptionalAttributeString("name", extension.Name);
                                xtw.WriteEndElement();
                            }
                        }
                        xtw.WriteEndElement();
                        break;

                    case ItemType.DeviceReference:
                        var deviceRef = datItem as DeviceReference;
                        xtw.WriteStartElement("file");
                        xtw.WriteAttributeString("type", "device_ref");
                        xtw.WriteRequiredAttributeString("name", deviceRef.Name);
                        xtw.WriteEndElement();
                        break;

                    case ItemType.DipSwitch:
                        var dipSwitch = datItem as DipSwitch;
                        xtw.WriteStartElement("file");
                        xtw.WriteAttributeString("type", "dipswitch");
                        xtw.WriteOptionalAttributeString("name", dipSwitch.Name);
                        xtw.WriteOptionalAttributeString("tag", dipSwitch.Tag);
                        xtw.WriteOptionalAttributeString("mask", dipSwitch.Mask);
                        if (dipSwitch.Conditions != null)
                        {
                            foreach (var dipSwitchCondition in dipSwitch.Conditions)
                            {
                                xtw.WriteStartElement("condition");
                                xtw.WriteOptionalAttributeString("tag", dipSwitchCondition.Tag);
                                xtw.WriteOptionalAttributeString("mask", dipSwitchCondition.Mask);
                                xtw.WriteOptionalAttributeString("relation", dipSwitchCondition.Relation.FromRelation());
                                xtw.WriteOptionalAttributeString("value", dipSwitchCondition.Value);
                                xtw.WriteEndElement();
                            }
                        }
                        if (dipSwitch.Locations != null)
                        {
                            foreach (var location in dipSwitch.Locations)
                            {
                                xtw.WriteStartElement("diplocation");
                                xtw.WriteOptionalAttributeString("name", location.Name);
                                xtw.WriteOptionalAttributeString("number", location.Number);
                                xtw.WriteOptionalAttributeString("inverted", location.Inverted.FromYesNo());
                                xtw.WriteEndElement();
                            }
                        }
                        if (dipSwitch.Values != null)
                        {
                            foreach (var value in dipSwitch.Values)
                            {
                                xtw.WriteStartElement("dipvalue");
                                xtw.WriteOptionalAttributeString("name", value.Name);
                                xtw.WriteOptionalAttributeString("value", value.Value);
                                xtw.WriteOptionalAttributeString("default", value.Default.FromYesNo());
                                if (value.Conditions != null)
                                {
                                    foreach (var dipValueCondition in value.Conditions)
                                    {
                                        xtw.WriteStartElement("condition");
                                        xtw.WriteOptionalAttributeString("tag", dipValueCondition.Tag);
                                        xtw.WriteOptionalAttributeString("mask", dipValueCondition.Mask);
                                        xtw.WriteOptionalAttributeString("relation", dipValueCondition.Relation.FromRelation());
                                        xtw.WriteOptionalAttributeString("value", dipValueCondition.Value);
                                        xtw.WriteEndElement();
                                    }
                                }
                                xtw.WriteEndElement();
                            }
                        }
                        xtw.WriteEndElement();
                        break;

                    case ItemType.Disk:
                        var disk = datItem as Disk;
                        xtw.WriteStartElement("file");
                        xtw.WriteAttributeString("type", "disk");
                        xtw.WriteRequiredAttributeString("name", disk.Name);
                        xtw.WriteOptionalAttributeString("md5", disk.MD5?.ToLowerInvariant());
                        xtw.WriteOptionalAttributeString("sha1", disk.SHA1?.ToLowerInvariant());
                        if (disk.ItemStatus != ItemStatus.None)
                        {
                            xtw.WriteStartElement("flags");

                            xtw.WriteStartElement("flag");
                            xtw.WriteAttributeString("name", "status");
                            xtw.WriteAttributeString("value", disk.ItemStatus.FromItemStatus(false));
                            xtw.WriteEndElement();

                            // End flags
                            xtw.WriteEndElement();
                        }
                        xtw.WriteEndElement();
                        break;

                    case ItemType.Display:
                        var display = datItem as Display;
                        xtw.WriteStartElement("file");
                        xtw.WriteAttributeString("type", "display");
                        xtw.WriteOptionalAttributeString("tag", display.Tag);
                        xtw.WriteOptionalAttributeString("type", display.DisplayType.FromDisplayType());
                        xtw.WriteOptionalAttributeString("rotate", display.Rotate);
                        xtw.WriteOptionalAttributeString("flipx", display.FlipX.FromYesNo());
                        xtw.WriteOptionalAttributeString("width", display.Width);
                        xtw.WriteOptionalAttributeString("height", display.Height);
                        xtw.WriteOptionalAttributeString("refresh", display.Refresh);
                        xtw.WriteOptionalAttributeString("pixclock", display.PixClock);
                        xtw.WriteOptionalAttributeString("htotal", display.HTotal);
                        xtw.WriteOptionalAttributeString("hbend", display.HBEnd);
                        xtw.WriteOptionalAttributeString("hstart", display.HBStart);
                        xtw.WriteOptionalAttributeString("vtotal", display.VTotal);
                        xtw.WriteOptionalAttributeString("vbend", display.VBEnd);
                        xtw.WriteOptionalAttributeString("vbstart", display.VBStart);
                        xtw.WriteEndElement();
                        break;

                    case ItemType.Driver:
                        var driver = datItem as Driver;
                        xtw.WriteStartElement("file");
                        xtw.WriteAttributeString("type", "driver");
                        xtw.WriteOptionalAttributeString("status", driver.Status.FromSupportStatus());
                        xtw.WriteOptionalAttributeString("emulation", driver.Emulation.FromSupportStatus());
                        xtw.WriteOptionalAttributeString("cocktail", driver.Cocktail.FromSupportStatus());
                        xtw.WriteOptionalAttributeString("savestate", driver.SaveState.FromSupported(true));
                        xtw.WriteEndElement();
                        break;

                    case ItemType.Feature:
                        var feature = datItem as Feature;
                        xtw.WriteStartElement("file");
                        xtw.WriteAttributeString("type", "feature");
                        xtw.WriteOptionalAttributeString("type", feature.Type.FromFeatureType());
                        xtw.WriteOptionalAttributeString("status", feature.Status.FromFeatureStatus());
                        xtw.WriteOptionalAttributeString("overall", feature.Overall.FromFeatureStatus());
                        xtw.WriteEndElement();
                        break;

                    case ItemType.Info:
                        var info = datItem as Info;
                        xtw.WriteStartElement("file");
                        xtw.WriteAttributeString("type", "info");
                        xtw.WriteRequiredAttributeString("name", info.Name);
                        xtw.WriteRequiredAttributeString("value", info.Value);
                        xtw.WriteEndElement();
                        break;

                    case ItemType.Input:
                        var input = datItem as Input;
                        xtw.WriteStartElement("file");
                        xtw.WriteAttributeString("type", "input");
                        xtw.WriteOptionalAttributeString("service", input.Service.FromYesNo());
                        xtw.WriteOptionalAttributeString("tilt", input.Tilt.FromYesNo());
                        xtw.WriteOptionalAttributeString("players", input.Players);
                        xtw.WriteOptionalAttributeString("coins", input.Coins);
                        if (input.Controls != null)
                        {
                            foreach (var control in input.Controls)
                            {
                                xtw.WriteStartElement("control");
                                xtw.WriteOptionalAttributeString("type", control.ControlType);
                                xtw.WriteOptionalAttributeString("player", control.Player);
                                xtw.WriteOptionalAttributeString("buttons", control.Buttons);
                                xtw.WriteOptionalAttributeString("regbuttons", control.RegButtons);
                                xtw.WriteOptionalAttributeString("minimum", control.Minimum);
                                xtw.WriteOptionalAttributeString("maximum", control.Maximum);
                                xtw.WriteOptionalAttributeString("sensitivity", control.Sensitivity);
                                xtw.WriteOptionalAttributeString("keydelta", control.KeyDelta);
                                xtw.WriteOptionalAttributeString("reverse", control.Reverse.FromYesNo());
                                xtw.WriteOptionalAttributeString("ways", control.Ways);
                                xtw.WriteOptionalAttributeString("ways2", control.Ways2);
                                xtw.WriteOptionalAttributeString("ways3", control.Ways3);
                                xtw.WriteEndElement();
                            }
                        }
                        xtw.WriteEndElement();
                        break;

                    case ItemType.Media:
                        var media = datItem as Media;
                        xtw.WriteStartElement("file");
                        xtw.WriteAttributeString("type", "media");
                        xtw.WriteRequiredAttributeString("name", media.Name);
                        xtw.WriteOptionalAttributeString("md5", media.MD5?.ToLowerInvariant());
                        xtw.WriteOptionalAttributeString("sha1", media.SHA1?.ToLowerInvariant());
                        xtw.WriteOptionalAttributeString("sha256", media.SHA256?.ToLowerInvariant());
                        xtw.WriteEndElement();
                        break;

                    case ItemType.Port:
                        var port = datItem as Port;
                        xtw.WriteStartElement("file");
                        xtw.WriteAttributeString("type", "port");
                        xtw.WriteOptionalAttributeString("tag", port.Tag);
                        if (port.Analogs != null)
                        {
                            foreach (var analog in port.Analogs)
                            {
                                xtw.WriteStartElement("analog");
                                xtw.WriteOptionalAttributeString("mask", analog.Mask);
                                xtw.WriteEndElement();
                            }
                        }
                        xtw.WriteEndElement();
                        break;

                    case ItemType.RamOption:
                        var ramOption = datItem as RamOption;
                        xtw.WriteStartElement("file");
                        xtw.WriteAttributeString("type", "ramoption");
                        xtw.WriteRequiredAttributeString("name", ramOption.Name);
                        xtw.WriteOptionalAttributeString("default", ramOption.Default.FromYesNo());
                        xtw.WriteRaw(ramOption.Content ?? string.Empty);
                        xtw.WriteFullEndElement();
                        break;

                    case ItemType.Release:
                        var release = datItem as Release;
                        xtw.WriteStartElement("file");
                        xtw.WriteAttributeString("type", "release");
                        xtw.WriteRequiredAttributeString("name", release.Name);
                        xtw.WriteOptionalAttributeString("region", release.Region);
                        xtw.WriteOptionalAttributeString("language", release.Language);
                        xtw.WriteOptionalAttributeString("date", release.Date);
                        xtw.WriteOptionalAttributeString("default", release.Default.FromYesNo());
                        xtw.WriteEndElement();
                        break;

                    case ItemType.Rom:
                        var rom = datItem as Rom;
                        xtw.WriteStartElement("file");
                        xtw.WriteAttributeString("type", "rom");
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
                        xtw.WriteOptionalAttributeString("date", rom.Date);
                        if (rom.ItemStatus != ItemStatus.None)
                        {
                            xtw.WriteStartElement("flags");

                            xtw.WriteStartElement("flag");
                            xtw.WriteAttributeString("name", "status");
                            xtw.WriteAttributeString("value", rom.ItemStatus.FromItemStatus(false));
                            xtw.WriteEndElement();

                            // End flags
                            xtw.WriteEndElement();
                        }
                        xtw.WriteEndElement();
                        break;

                    case ItemType.Sample:
                        var sample = datItem as Sample;
                        xtw.WriteStartElement("file");
                        xtw.WriteAttributeString("type", "sample");
                        xtw.WriteRequiredAttributeString("name", sample.Name);
                        xtw.WriteEndElement();
                        break;

                    case ItemType.SharedFeature:
                        var sharedFeature = datItem as SharedFeature;
                        xtw.WriteStartElement("file");
                        xtw.WriteAttributeString("type", "sharedfeat");
                        xtw.WriteRequiredAttributeString("name", sharedFeature.Name);
                        xtw.WriteRequiredAttributeString("value", sharedFeature.Value);
                        xtw.WriteEndElement();
                        break;

                    case ItemType.Slot:
                        var slot = datItem as Slot;
                        xtw.WriteStartElement("file");
                        xtw.WriteAttributeString("type", "slot");
                        xtw.WriteOptionalAttributeString("name", slot.Name);
                        if (slot.SlotOptions != null)
                        {
                            foreach (var slotOption in slot.SlotOptions)
                            {
                                xtw.WriteStartElement("slotoption");
                                xtw.WriteOptionalAttributeString("name", slotOption.Name);
                                xtw.WriteOptionalAttributeString("devname", slotOption.DeviceName);
                                xtw.WriteOptionalAttributeString("default", slotOption.Default.FromYesNo());
                                xtw.WriteEndElement();
                            }
                        }
                        xtw.WriteEndElement();
                        break;

                    case ItemType.SoftwareList:
                        var softwareList = datItem as DatItems.SoftwareList;
                        xtw.WriteStartElement("file");
                        xtw.WriteAttributeString("type", "softwarelist");
                        xtw.WriteRequiredAttributeString("name", softwareList.Name);
                        xtw.WriteOptionalAttributeString("status", softwareList.Status.FromSoftwareListStatus());
                        xtw.WriteOptionalAttributeString("sha512", softwareList.Filter);
                        xtw.WriteEndElement();
                        break;

                    case ItemType.Sound:
                        var sound = datItem as Sound;
                        xtw.WriteStartElement("file");
                        xtw.WriteAttributeString("type", "sound");
                        xtw.WriteOptionalAttributeString("channels", sound.Channels);
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
        /// <param name="depth">Current depth to output file at (SabreDAT only)</param>
        /// <returns>True if the data was written, false on error</returns>
        private bool WriteFooter(XmlTextWriter xtw, int depth)
        {
            try
            {
                for (int i = depth - 1; i >= 2; i--)
                {
                    // End directory
                    xtw.WriteEndElement();
                }

                // End data
                xtw.WriteEndElement();

                // End datafile
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
