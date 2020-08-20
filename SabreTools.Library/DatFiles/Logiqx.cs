using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

using SabreTools.Library.Data;
using SabreTools.Library.DatItems;
using SabreTools.Library.IO;
using SabreTools.Library.Tools;

namespace SabreTools.Library.DatFiles
{
    /// <summary>
    /// Represents parsing and writing of a Logiqx-derived DAT
    /// </summary>
    /// TODO: Add XSD validation for all XML DAT types (maybe?)
    internal class Logiqx : DatFile
    {
        // Private instance variables specific to Logiqx DATs
        private readonly bool _deprecated;

        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        /// <param name="deprecated">True if the output uses "game", false if the output uses "machine"</param>
        public Logiqx(DatFile datFile, bool deprecated)
            : base(datFile)
        {
            _deprecated = deprecated;
        }

        /// <summary>
        /// Parse a Logiqx XML DAT and return all found games and roms within
        /// </summary>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        protected override void ParseFile(
            // Standard Dat parsing
            string filename,
            int indexId,

            // Miscellaneous
            bool keep)
        {
            // Prepare all internal variables
            XmlReader xtr = filename.GetXmlTextReader();
            List<string> dirs = new List<string>();

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
                        // If we're ending a dir, remove the last item from the dirs list, if possible
                        if (xtr.Name == "dir" && dirs.Count > 0)
                            dirs.RemoveAt(dirs.Count - 1);

                        xtr.Read();
                        continue;
                    }

                    switch (xtr.Name)
                    {
                        // The datafile tag can have some attributes
                        case "datafile":
                            Header.Build = (Header.Build == null ? xtr.GetAttribute("build") : Header.Build);
                            Header.Debug = (Header.Debug == null ? xtr.GetAttribute("debug").AsYesNo() : Header.Debug);
                            xtr.Read();
                            break;

                        // We want to process the entire subtree of the header
                        case "header":
                            ReadHeader(xtr.ReadSubtree(), keep);

                            // Skip the header node now that we've processed it
                            xtr.Skip();
                            break;

                        // Unique to RomVault-created DATs
                        case "dir":
                            Header.Type = "SuperDAT";
                            dirs.Add(xtr.GetAttribute("name") ?? string.Empty);
                            xtr.Read();
                            break;

                        // We want to process the entire subtree of the game
                        case "machine": // New-style Logiqx
                        case "game": // Old-style Logiqx
                            ReadMachine(xtr.ReadSubtree(), dirs, filename, indexId, keep);

                            // Skip the machine now that we've processed it
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

            // Otherwise, add what is possible
            reader.MoveToContent();

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
                        Header.Name = (Header.Name == null ? content : Header.Name);
                        superdat = superdat || content.Contains(" - SuperDAT");
                        if (keep && superdat)
                        {
                            Header.Type = (Header.Type == null ? "SuperDAT" : Header.Type);
                        }
                        break;

                    case "description":
                        content = reader.ReadElementContentAsString();
                        Header.Description = (Header.Description == null ? content : Header.Description);
                        break;

                    case "rootdir": // This is exclusive to TruRip XML
                        content = reader.ReadElementContentAsString();
                        Header.RootDir = (Header.RootDir == null ? content : Header.RootDir);
                        break;

                    case "category":
                        content = reader.ReadElementContentAsString();
                        Header.Category = (Header.Category == null ? content : Header.Category);
                        break;

                    case "version":
                        content = reader.ReadElementContentAsString();
                        Header.Version = (Header.Version == null ? content : Header.Version);
                        break;

                    case "date":
                        content = reader.ReadElementContentAsString();
                        Header.Date = (Header.Date == null ? content.Replace(".", "/") : Header.Date);
                        break;

                    case "author":
                        content = reader.ReadElementContentAsString();
                        Header.Author = (Header.Author == null ? content : Header.Author);
                        break;

                    case "email":
                        content = reader.ReadElementContentAsString();
                        Header.Email = (Header.Email == null ? content : Header.Email);
                        break;

                    case "homepage":
                        content = reader.ReadElementContentAsString();
                        Header.Homepage = (Header.Homepage == null ? content : Header.Homepage);
                        break;

                    case "url":
                        content = reader.ReadElementContentAsString();
                        Header.Url = (Header.Url == null ? content : Header.Url);
                        break;

                    case "comment":
                        content = reader.ReadElementContentAsString();
                        Header.Comment = (Header.Comment == null ? content : Header.Comment);
                        break;

                    case "type": // This is exclusive to TruRip XML
                        content = reader.ReadElementContentAsString();
                        Header.Type = (Header.Type == null ? content : Header.Type);
                        superdat = superdat || content.Contains("SuperDAT");
                        break;

                    case "clrmamepro":
                        if (Header.HeaderSkipper == null)
                            Header.HeaderSkipper = reader.GetAttribute("header");

                        if (Header.ForceMerging == ForceMerging.None)
                            Header.ForceMerging = reader.GetAttribute("forcemerging").AsForceMerging();

                        if (Header.ForceNodump == ForceNodump.None)
                            Header.ForceNodump = reader.GetAttribute("forcenodump").AsForceNodump();

                        if (Header.ForcePacking == ForcePacking.None)
                            Header.ForcePacking = reader.GetAttribute("forcepacking").AsForcePacking();

                        reader.Read();
                        break;

                    case "romcenter":
                        if (reader.GetAttribute("plugin") != null)
                        {
                            // CDATA
                        }

                        if (reader.GetAttribute("rommode") != null)
                        {
                            // (merged|split|unmerged) "split"
                        }

                        if (reader.GetAttribute("biosmode") != null)
                        {
                            // merged|split|unmerged) "split"
                        }

                        if (reader.GetAttribute("samplemode") != null)
                        {
                            // (merged|unmerged) "merged"
                        }

                        if (reader.GetAttribute("lockrommode") != null)
                        {
                            // (yes|no) "no"
                        }

                        if (reader.GetAttribute("lockbiosmode") != null)
                        {
                            // (yes|no) "no"
                        }

                        if (reader.GetAttribute("locksamplemode") != null)
                        {
                            // (yes|no) "no"
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
        /// Read game/machine information
        /// </summary>
        /// <param name="reader">XmlReader to use to parse the machine</param>
        /// <param name="dirs">List of dirs to prepend to the game name</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        private void ReadMachine(
            XmlReader reader,
            List<string> dirs,

            // Standard Dat parsing
            string filename,
            int indexId,

            // Miscellaneous
            bool keep)
        {
            // If we have an empty machine, skip it
            if (reader == null)
                return;

            // Otherwise, add what is possible
            reader.MoveToContent();

            string key = string.Empty;
            string temptype = reader.Name;
            bool containsItems = false;

            // Create a new machine
            MachineType machineType = MachineType.NULL;
            if (reader.GetAttribute("isbios").AsYesNo() == true)
                machineType |= MachineType.Bios;

            if (reader.GetAttribute("isdevice").AsYesNo() == true) // Listxml-specific, used by older DATs
                machineType |= MachineType.Device;

            if (reader.GetAttribute("ismechanical").AsYesNo() == true) // Listxml-specific, used by older DATs
                machineType |= MachineType.Mechanical;

            string dirsString = (dirs != null && dirs.Count() > 0 ? string.Join("/", dirs) + "/" : string.Empty);
            Machine machine = new Machine
            {
                Name = dirsString + reader.GetAttribute("name"),
                Description = dirsString + reader.GetAttribute("name"),
                SourceFile = reader.GetAttribute("sourcefile"),
                Board = reader.GetAttribute("board"),
                RebuildTo = reader.GetAttribute("rebuildto"),
                Runnable = reader.GetAttribute("runnable").AsYesNo(), // Listxml-specific, used by older DATs

                Comment = string.Empty,

                CloneOf = reader.GetAttribute("cloneof") ?? string.Empty,
                RomOf = reader.GetAttribute("romof") ?? string.Empty,
                SampleOf = reader.GetAttribute("sampleof") ?? string.Empty,

                MachineType = (machineType == MachineType.NULL ? MachineType.None : machineType),
            };

            if (Header.Type == "SuperDAT" && !keep)
            {
                string tempout = Regex.Match(machine.Name, @".*?\\(.*)").Groups[1].Value;
                if (!string.IsNullOrWhiteSpace(tempout))
                    machine.Name = tempout;
            }

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
                    case "comment": // There can be multiple comments by spec
                        machine.Comment += reader.ReadElementContentAsString();
                        break;

                    case "description":
                        machine.Description = reader.ReadElementContentAsString();
                        break;

                    case "year":
                        machine.Year = reader.ReadElementContentAsString();
                        break;

                    case "manufacturer":
                        machine.Manufacturer = reader.ReadElementContentAsString();
                        break;

                    case "publisher": // Not technically supported but used by some legacy DATs
                        machine.Publisher = reader.ReadElementContentAsString();
                        break;

                    case "category": // Not technically supported but used by some legacy DATs
                        machine.Category = reader.ReadElementContentAsString();
                        break;

                    case "trurip": // This is special metadata unique to TruRip
                        ReadTruRip(reader.ReadSubtree(), machine);

                        // Skip the trurip node now that we've processed it
                        reader.Skip();
                        break;

                    case "release":
                        containsItems = true;

                        DatItem release = new Release
                        {
                            Name = reader.GetAttribute("name"),
                            Region = reader.GetAttribute("region"),
                            Language = reader.GetAttribute("language"),
                            Date = reader.GetAttribute("date"),
                            Default = reader.GetAttribute("default").AsYesNo(),
                        };

                        release.CopyMachineInformation(machine);

                        // Now process and add the rom
                        key = ParseAddHelper(release);

                        reader.Read();
                        break;

                    case "biosset":
                        containsItems = true;

                        DatItem biosset = new BiosSet
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

                        biosset.CopyMachineInformation(machine);

                        // Now process and add the rom
                        key = ParseAddHelper(biosset);

                        reader.Read();
                        break;

                    case "rom":
                        containsItems = true;

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
                            MergeTag = reader.GetAttribute("merge"),
                            ItemStatus = reader.GetAttribute("status").AsItemStatus(),
                            Date = Sanitizer.CleanDate(reader.GetAttribute("date")),
                            Inverted = reader.GetAttribute("inverted").AsYesNo(),

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        };

                        rom.CopyMachineInformation(machine);

                        // Now process and add the rom
                        key = ParseAddHelper(rom);

                        reader.Read();
                        break;

                    case "disk":
                        containsItems = true;

                        DatItem disk = new Disk
                        {
                            Name = reader.GetAttribute("name"),
                            MD5 = reader.GetAttribute("md5"),
#if NET_FRAMEWORK
                            RIPEMD160 = reader.GetAttribute("ripemd160"),
#endif
                            SHA1 = reader.GetAttribute("sha1"),
                            SHA256 = reader.GetAttribute("sha256"),
                            SHA384 = reader.GetAttribute("sha384"),
                            SHA512 = reader.GetAttribute("sha512"),
                            MergeTag = reader.GetAttribute("merge"),
                            ItemStatus = reader.GetAttribute("status").AsItemStatus(),

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        };

                        disk.CopyMachineInformation(machine);

                        // Now process and add the rom
                        key = ParseAddHelper(disk);

                        reader.Read();
                        break;

                    case "sample":
                        containsItems = true;

                        DatItem samplerom = new Sample
                        {
                            Name = reader.GetAttribute("name"),

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        };

                        samplerom.CopyMachineInformation(machine);

                        // Now process and add the rom
                        key = ParseAddHelper(samplerom);

                        reader.Read();
                        break;

                    case "archive":
                        containsItems = true;

                        DatItem archiverom = new Archive
                        {
                            Name = reader.GetAttribute("name"),

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        };

                        archiverom.CopyMachineInformation(machine);

                        // Now process and add the rom
                        key = ParseAddHelper(archiverom);

                        reader.Read();
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
        /// Read TruRip information
        /// </summary>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        /// <param name="machine">Machine information to pass to contained items</param>
        private void ReadTruRip(XmlReader reader, Machine machine)
        {
            // If we have an empty trurip, skip it
            if (reader == null)
                return;

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

                // Get the information from the trurip
                switch (reader.Name)
                {
                    case "titleid":
                        reader.ReadElementContentAsString();
                        break;

                    case "publisher":
                        machine.Publisher = reader.ReadElementContentAsString();
                        break;

                    case "developer": // Manufacturer is as close as this gets
                        machine.Manufacturer = reader.ReadElementContentAsString();
                        break;

                    case "year":
                        machine.Year = reader.ReadElementContentAsString();
                        break;

                    case "genre":
                        machine.Category = reader.ReadElementContentAsString();
                        break;

                    case "subgenre":
                        reader.ReadElementContentAsString();
                        break;

                    case "ratings":
                        reader.ReadElementContentAsString();
                        break;

                    case "score":
                        reader.ReadElementContentAsString();
                        break;

                    case "players":
                        reader.ReadElementContentAsString();
                        break;

                    case "enabled":
                        reader.ReadElementContentAsString();
                        break;

                    case "crc":
                        reader.ReadElementContentAsString().AsYesNo();
                        break;

                    case "source":
                        machine.SourceFile = reader.ReadElementContentAsString();
                        break;

                    case "cloneof":
                        machine.CloneOf = reader.ReadElementContentAsString();
                        break;

                    case "relatedto":
                        reader.ReadElementContentAsString();
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
                    List<DatItem> roms = Items[key];

                    // Resolve the names in the block
                    roms = DatItem.ResolveNames(roms);

                    for (int index = 0; index < roms.Count; index++)
                    {
                        DatItem rom = roms[index];

                        // There are apparently times when a null rom can skip by, skip them
                        if (rom.Name == null || rom.Machine.Name == null)
                        {
                            Globals.Logger.Warning("Null rom found!");
                            continue;
                        }

                        // If we have a different game and we're not at the start of the list, output the end of last item
                        if (lastgame != null && lastgame.ToLowerInvariant() != rom.Machine.Name.ToLowerInvariant())
                            WriteEndGame(xtw);

                        // If we have a new game, output the beginning of the new item
                        if (lastgame == null || lastgame.ToLowerInvariant() != rom.Machine.Name.ToLowerInvariant())
                            WriteStartGame(xtw, rom);

                        // If we have a "null" game (created by DATFromDir or something similar), log it to file
                        if (rom.ItemType == ItemType.Rom
                            && ((Rom)rom).Size == -1
                            && ((Rom)rom).CRC == "null")
                        {
                            Globals.Logger.Verbose($"Empty folder found: {rom.Machine.Name}");

                            rom.Name = (rom.Name == "null" ? "-" : rom.Name);
                            ((Rom)rom).Size = Constants.SizeZero;
                            ((Rom)rom).CRC = ((Rom)rom).CRC == "null" ? Constants.CRCZero : null;
                            ((Rom)rom).MD5 = ((Rom)rom).MD5 == "null" ? Constants.MD5Zero : null;
#if NET_FRAMEWORK
                            ((Rom)rom).RIPEMD160 = ((Rom)rom).RIPEMD160 == "null" ? Constants.RIPEMD160Zero : null;
#endif
                            ((Rom)rom).SHA1 = ((Rom)rom).SHA1 == "null" ? Constants.SHA1Zero : null;
                            ((Rom)rom).SHA256 = ((Rom)rom).SHA256 == "null" ? Constants.SHA256Zero : null;
                            ((Rom)rom).SHA384 = ((Rom)rom).SHA384 == "null" ? Constants.SHA384Zero : null;
                            ((Rom)rom).SHA512 = ((Rom)rom).SHA512 == "null" ? Constants.SHA512Zero : null;
                        }

                        // Now, output the rom data
                        WriteDatItem(xtw, rom, ignoreblanks);

                        // Set the new data to compare against
                        lastgame = rom.Machine.Name;
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
                xtw.WriteDocType("datafile", "-//Logiqx//DTD ROM Management Datafile//EN", "http://www.logiqx.com/Dats/datafile.dtd", null);

                xtw.WriteStartElement("datafile");

                if (Header.Build != null)
                    xtw.WriteAttributeString("build", Header.Build);
                if (Header.Debug != null)
                {
                    switch (Header.Debug)
                    {
                        case true:
                            xtw.WriteAttributeString("debug", "yes");
                            break;
                        case false:
                            xtw.WriteAttributeString("debug", "no");
                            break;
                    }
                }

                xtw.WriteStartElement("header");
                xtw.WriteElementString("name", Header.Name);
                xtw.WriteElementString("description", Header.Description);
                if (!string.IsNullOrWhiteSpace(Header.RootDir))
                    xtw.WriteElementString("rootdir", Header.RootDir);
                if (!string.IsNullOrWhiteSpace(Header.Category))
                    xtw.WriteElementString("category", Header.Category);
                xtw.WriteElementString("version", Header.Version);
                if (!string.IsNullOrWhiteSpace(Header.Date))
                    xtw.WriteElementString("date", Header.Date);
                xtw.WriteElementString("author", Header.Author);
                if (!string.IsNullOrWhiteSpace(Header.Email))
                    xtw.WriteElementString("email", Header.Email);
                if (!string.IsNullOrWhiteSpace(Header.Homepage))
                    xtw.WriteElementString("homepage", Header.Homepage);
                if (!string.IsNullOrWhiteSpace(Header.Url))
                    xtw.WriteElementString("url", Header.Url);
                if (!string.IsNullOrWhiteSpace(Header.Comment))
                    xtw.WriteElementString("comment", Header.Comment);
                if (!string.IsNullOrWhiteSpace(Header.Type))
                    xtw.WriteElementString("type", Header.Type);

                if (Header.ForcePacking != ForcePacking.None
                    || Header.ForceMerging != ForceMerging.None
                    || Header.ForceNodump != ForceNodump.None
                    || !string.IsNullOrWhiteSpace(Header.HeaderSkipper))
                {
                    xtw.WriteStartElement("clrmamepro");
                    switch (Header.ForcePacking)
                    {
                        case ForcePacking.Unzip:
                            xtw.WriteAttributeString("forcepacking", "unzip");
                            break;
                        case ForcePacking.Zip:
                            xtw.WriteAttributeString("forcepacking", "zip");
                            break;
                    }

                    switch (Header.ForceMerging)
                    {
                        case ForceMerging.Full:
                            xtw.WriteAttributeString("forcemerging", "full");
                            break;
                        case ForceMerging.Split:
                            xtw.WriteAttributeString("forcemerging", "split");
                            break;
                        case ForceMerging.Merged:
                            xtw.WriteAttributeString("forcemerging", "merged");
                            break;
                        case ForceMerging.NonMerged:
                            xtw.WriteAttributeString("forcemerging", "nonmerged");
                            break;
                    }

                    switch (Header.ForceNodump)
                    {
                        case ForceNodump.Ignore:
                            xtw.WriteAttributeString("forcenodump", "ignore");
                            break;
                        case ForceNodump.Obsolete:
                            xtw.WriteAttributeString("forcenodump", "obsolete");
                            break;
                        case ForceNodump.Required:
                            xtw.WriteAttributeString("forcenodump", "required");
                            break;
                    }

                    if (!string.IsNullOrWhiteSpace(Header.HeaderSkipper))
                        xtw.WriteAttributeString("header", Header.HeaderSkipper);

                    // End clrmamepro
                    xtw.WriteEndElement();
                }

                // End header
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

                // Build the state based on excluded fields
                xtw.WriteStartElement(_deprecated ? "game" : "machine");
                xtw.WriteAttributeString("name", datItem.GetField(Field.MachineName, Header.ExcludeFields));
                if (!Header.ExcludeFields.Contains(Field.MachineType))
                {
                    if (datItem.Machine.MachineType.HasFlag(MachineType.Bios))
                        xtw.WriteAttributeString("isbios", "yes");
                    if (datItem.Machine.MachineType.HasFlag(MachineType.Device))
                        xtw.WriteAttributeString("isdevice", "yes");
                    if (datItem.Machine.MachineType.HasFlag(MachineType.Mechanical))
                        xtw.WriteAttributeString("ismechanical", "yes");
                }

                if (!Header.ExcludeFields.Contains(Field.Runnable) && datItem.Machine.Runnable != null)
                {
                    if (datItem.Machine.Runnable == true)
                        xtw.WriteAttributeString("runnable", "yes");
                    else if (datItem.Machine.Runnable == false)
                        xtw.WriteAttributeString("runnable", "no");
                }

                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.CloneOf, Header.ExcludeFields)) && !string.Equals(datItem.Machine.Name, datItem.Machine.CloneOf, StringComparison.OrdinalIgnoreCase))
                    xtw.WriteAttributeString("cloneof", datItem.Machine.CloneOf);
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.RomOf, Header.ExcludeFields)) && !string.Equals(datItem.Machine.Name, datItem.Machine.RomOf, StringComparison.OrdinalIgnoreCase))
                    xtw.WriteAttributeString("romof", datItem.Machine.RomOf);
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.SampleOf, Header.ExcludeFields)) && !string.Equals(datItem.Machine.Name, datItem.Machine.SampleOf, StringComparison.OrdinalIgnoreCase))
                    xtw.WriteAttributeString("sampleof", datItem.Machine.SampleOf);

                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.Comment, Header.ExcludeFields)))
                    xtw.WriteElementString("comment", datItem.Machine.Comment);
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.Description, Header.ExcludeFields)))
                    xtw.WriteElementString("description", datItem.Machine.Description);
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.Year, Header.ExcludeFields)))
                    xtw.WriteElementString("year", datItem.Machine.Year);
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.Publisher, Header.ExcludeFields)))
                    xtw.WriteElementString("publisher", datItem.Machine.Publisher);
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.Manufacturer, Header.ExcludeFields)))
                    xtw.WriteElementString("manufacturer", datItem.Machine.Manufacturer);
                if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.Category, Header.ExcludeFields)))
                    xtw.WriteElementString("category", datItem.Machine.Category);

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
        /// Write out Game end using the supplied StreamWriter
        /// </summary>
        /// <param name="xtw">XmlTextWriter to output to</param>
        /// <returns>True if the data was written, false on error</returns>
        private bool WriteEndGame(XmlTextWriter xtw)
        {
            try
            {
                // End machine
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
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise (default)</param>
        /// <returns>True if the data was written, false on error</returns>
        private bool WriteDatItem(XmlTextWriter xtw, DatItem datItem, bool ignoreblanks = false)
        {
            // If we are in ignore blanks mode AND we have a blank (0-size) rom, skip
            if (ignoreblanks && (datItem.ItemType == ItemType.Rom && ((datItem as Rom).Size == 0 || (datItem as Rom).Size == -1)))
                return true;

            try
            {
                // Pre-process the item name
                ProcessItemName(datItem, true);

                // Build the state based on excluded fields
                switch (datItem.ItemType)
                {
                    case ItemType.Archive:
                        xtw.WriteStartElement("archive");
                        xtw.WriteAttributeString("name", datItem.GetField(Field.Name, Header.ExcludeFields));
                        xtw.WriteEndElement();
                        break;

                    case ItemType.BiosSet:
                        var biosSet = datItem as BiosSet;
                        xtw.WriteStartElement("biosset");
                        xtw.WriteAttributeString("name", biosSet.GetField(Field.Name, Header.ExcludeFields));
                        if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.BiosDescription, Header.ExcludeFields)))
                            xtw.WriteAttributeString("description", biosSet.Description);
                        if (!Header.ExcludeFields.Contains(Field.Default) && biosSet.Default != null)
                            xtw.WriteAttributeString("default", biosSet.Default.ToString().ToLowerInvariant());
                        xtw.WriteEndElement();
                        break;

                    case ItemType.Disk:
                        var disk = datItem as Disk;
                        xtw.WriteStartElement("disk");
                        xtw.WriteAttributeString("name", disk.GetField(Field.Name, Header.ExcludeFields));
                        if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.MD5, Header.ExcludeFields)))
                            xtw.WriteAttributeString("md5", disk.MD5.ToLowerInvariant());
#if NET_FRAMEWORK
                        if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.RIPEMD160, Header.ExcludeFields)))
                            xtw.WriteAttributeString("ripemd160", disk.RIPEMD160.ToLowerInvariant());
#endif
                        if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.SHA1, Header.ExcludeFields)))
                            xtw.WriteAttributeString("sha1", disk.SHA1.ToLowerInvariant());
                        if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.SHA256, Header.ExcludeFields)))
                            xtw.WriteAttributeString("sha256", disk.SHA256.ToLowerInvariant());
                        if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.SHA384, Header.ExcludeFields)))
                            xtw.WriteAttributeString("sha384", disk.SHA384.ToLowerInvariant());
                        if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.SHA512, Header.ExcludeFields)))
                            xtw.WriteAttributeString("sha512", disk.SHA512.ToLowerInvariant());
                        if (!Header.ExcludeFields.Contains(Field.Status) && disk.ItemStatus != ItemStatus.None)
                            xtw.WriteAttributeString("status", disk.ItemStatus.ToString().ToLowerInvariant());
                        xtw.WriteEndElement();
                        break;

                    case ItemType.Release:
                        var release = datItem as Release;
                        xtw.WriteStartElement("release");
                        xtw.WriteAttributeString("name", release.GetField(Field.Name, Header.ExcludeFields));
                        if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.Region, Header.ExcludeFields)))
                            xtw.WriteAttributeString("region", release.Region);
                        if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.Language, Header.ExcludeFields)))
                            xtw.WriteAttributeString("language", release.Language);
                        if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.Date, Header.ExcludeFields)))
                            xtw.WriteAttributeString("date", release.Date);
                        if (!Header.ExcludeFields.Contains(Field.Default) && release.Default != null)
                            xtw.WriteAttributeString("default", release.Default.ToString().ToLowerInvariant());
                        xtw.WriteEndElement();
                        break;

                    case ItemType.Rom:
                        var rom = datItem as Rom;
                        xtw.WriteStartElement("rom");
                        xtw.WriteAttributeString("name", rom.GetField(Field.Name, Header.ExcludeFields));
                        if (!Header.ExcludeFields.Contains(Field.Size) && rom.Size != -1)
                            xtw.WriteAttributeString("size", rom.Size.ToString());
                        if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.CRC, Header.ExcludeFields)))
                            xtw.WriteAttributeString("crc", rom.CRC.ToLowerInvariant());
                        if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.MD5, Header.ExcludeFields)))
                            xtw.WriteAttributeString("md5", rom.MD5.ToLowerInvariant());
#if NET_FRAMEWORK
                        if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.RIPEMD160, Header.ExcludeFields)))
                            xtw.WriteAttributeString("ripemd160", rom.RIPEMD160.ToLowerInvariant());
#endif
                        if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.SHA1, Header.ExcludeFields)))
                            xtw.WriteAttributeString("sha1", rom.SHA1.ToLowerInvariant());
                        if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.SHA256, Header.ExcludeFields)))
                            xtw.WriteAttributeString("sha256", rom.SHA256.ToLowerInvariant());
                        if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.SHA384, Header.ExcludeFields)))
                            xtw.WriteAttributeString("sha384", rom.SHA384.ToLowerInvariant());
                        if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.SHA512, Header.ExcludeFields)))
                            xtw.WriteAttributeString("sha512", rom.SHA512.ToLowerInvariant());
                        if (!string.IsNullOrWhiteSpace(datItem.GetField(Field.Date, Header.ExcludeFields)))
                            xtw.WriteAttributeString("date", rom.Date);
                        if (!Header.ExcludeFields.Contains(Field.Status) && rom.ItemStatus != ItemStatus.None)
                            xtw.WriteAttributeString("status", rom.ItemStatus.ToString().ToLowerInvariant());
                        if (!Header.ExcludeFields.Contains(Field.Inverted) && rom.Inverted != null)
                            xtw.WriteAttributeString("inverted", rom.Inverted.ToString().ToLowerInvariant());
                        xtw.WriteEndElement();
                        break;

                    case ItemType.Sample:
                        xtw.WriteStartElement("sample");
                        xtw.WriteAttributeString("name", datItem.GetField(Field.Name, Header.ExcludeFields));
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
                // End machine
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
