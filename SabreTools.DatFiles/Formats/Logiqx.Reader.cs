using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing a Logiqx-derived DAT
    /// </summary>
    internal partial class Logiqx : DatFile
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

            List<string> dirs = new();

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
                            Header.Build ??= xtr.GetAttribute("build");
                            Header.Debug ??= xtr.GetAttribute("debug").AsYesNo();
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
                            ReadMachine(xtr.ReadSubtree(), dirs, statsOnly, filename, indexId, keep);

                            // Skip the machine now that we've processed it
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
                    case "id":
                        content = reader.ReadElementContentAsString();
                        Header.NoIntroID ??= content;
                        break;

                    case "name":
                        content = reader.ReadElementContentAsString();
                        Header.Name ??= content;
                        superdat |= content.Contains(" - SuperDAT");
                        if (keep && superdat)
                        {
                            Header.Type ??= "SuperDAT";
                        }
                        break;

                    case "description":
                        content = reader.ReadElementContentAsString();
                        Header.Description ??= content;
                        break;

                    case "rootdir": // This is exclusive to TruRip XML
                        content = reader.ReadElementContentAsString();
                        Header.RootDir ??= content;
                        break;

                    case "category":
                        content = reader.ReadElementContentAsString();
                        Header.Category = (Header.Category == null ? content : $"{Header.Category};{content}");
                        break;

                    case "version":
                        content = reader.ReadElementContentAsString();
                        Header.Version ??= content;
                        break;

                    case "date":
                        content = reader.ReadElementContentAsString();
                        Header.Date ??= content.Replace(".", "/");
                        break;

                    case "author":
                        content = reader.ReadElementContentAsString();
                        Header.Author ??= content;
                        break;

                    case "email":
                        content = reader.ReadElementContentAsString();
                        Header.Email ??= content;
                        break;

                    case "homepage":
                        content = reader.ReadElementContentAsString();
                        Header.Homepage ??= content;
                        break;

                    case "url":
                        content = reader.ReadElementContentAsString();
                        Header.Url ??= content;
                        break;

                    case "comment":
                        content = reader.ReadElementContentAsString();
                        Header.Comment = (Header.Comment ?? content);
                        break;

                    case "type": // This is exclusive to TruRip XML
                        content = reader.ReadElementContentAsString();
                        Header.Type ??= content;
                        superdat |= content.Contains("SuperDAT");
                        break;

                    case "clrmamepro":
                        if (Header.HeaderSkipper == null)
                            Header.HeaderSkipper = reader.GetAttribute("header");

                        if (Header.ForceMerging == MergingFlag.None)
                            Header.ForceMerging = reader.GetAttribute("forcemerging").AsMergingFlag();

                        if (Header.ForceNodump == NodumpFlag.None)
                            Header.ForceNodump = reader.GetAttribute("forcenodump").AsNodumpFlag();

                        if (Header.ForcePacking == PackingFlag.None)
                            Header.ForcePacking = reader.GetAttribute("forcepacking").AsPackingFlag();

                        reader.Read();
                        break;

                    case "romcenter":
                        if (Header.System == null)
                            Header.System = reader.GetAttribute("plugin");

                        if (Header.RomMode == MergingFlag.None)
                            Header.RomMode = reader.GetAttribute("rommode").AsMergingFlag();

                        if (Header.BiosMode == MergingFlag.None)
                            Header.BiosMode = reader.GetAttribute("biosmode").AsMergingFlag();

                        if (Header.SampleMode == MergingFlag.None)
                            Header.SampleMode = reader.GetAttribute("samplemode").AsMergingFlag();

                        if (Header.LockRomMode == null)
                            Header.LockRomMode = reader.GetAttribute("lockrommode").AsYesNo();

                        if (Header.LockBiosMode == null)
                            Header.LockBiosMode = reader.GetAttribute("lockbiosmode").AsYesNo();

                        if (Header.LockSampleMode == null)
                            Header.LockSampleMode = reader.GetAttribute("locksamplemode").AsYesNo();

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
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        private void ReadMachine(
            XmlReader reader,
            List<string> dirs,
            bool statsOnly,

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

            bool containsItems = false;

            // Create a new machine
            MachineType machineType = 0x0;
            if (reader.GetAttribute("isbios").AsYesNo() == true)
                machineType |= MachineType.Bios;

            string dirsString = (dirs != null && dirs.Count > 0 ? string.Join("/", dirs) + "/" : string.Empty);
            Machine machine = new()
            {
                Name = dirsString + reader.GetAttribute("name"),
                Description = dirsString + reader.GetAttribute("name"),
                SourceFile = reader.GetAttribute("sourcefile"),
                Board = reader.GetAttribute("board"),
                RebuildTo = reader.GetAttribute("rebuildto"),
                NoIntroId = reader.GetAttribute("id"),
                NoIntroCloneOfId = reader.GetAttribute("cloneofid"),
                Runnable = reader.GetAttribute("runnable").AsRunnable(), // Used by older DATs

                CloneOf = reader.GetAttribute("cloneof"),
                RomOf = reader.GetAttribute("romof"),
                SampleOf = reader.GetAttribute("sampleof"),

                MachineType = (machineType == 0x0 ? MachineType.None : machineType),
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

                    case "trurip": // This is special metadata unique to EmuArc
                        ReadTruRip(reader.ReadSubtree(), machine);

                        // Skip the trurip node now that we've processed it
                        reader.Skip();
                        break;

                    case "archive":
                        containsItems = true;

                        DatItem archive = new Archive
                        {
                            Name = reader.GetAttribute("name"),

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        };

                        archive.CopyMachineInformation(machine);

                        // Now process and add the archive
                        ParseAddHelper(archive, statsOnly);

                        reader.Read();
                        break;

                    case "biosset":
                        containsItems = true;

                        DatItem biosSet = new BiosSet
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

                        biosSet.CopyMachineInformation(machine);

                        // Now process and add the biosSet
                        ParseAddHelper(biosSet, statsOnly);

                        reader.Read();
                        break;

                    case "disk":
                        containsItems = true;

                        DatItem disk = new Disk
                        {
                            Name = reader.GetAttribute("name"),
                            MD5 = reader.GetAttribute("md5"),
                            SHA1 = reader.GetAttribute("sha1"),
                            MergeTag = reader.GetAttribute("merge"),
                            ItemStatus = reader.GetAttribute("status").AsItemStatus(),

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        };

                        disk.CopyMachineInformation(machine);

                        // Now process and add the disk
                        ParseAddHelper(disk, statsOnly);

                        reader.Read();
                        break;

                    case "media":
                        containsItems = true;

                        DatItem media = new Media
                        {
                            Name = reader.GetAttribute("name"),
                            MD5 = reader.GetAttribute("md5"),
                            SHA1 = reader.GetAttribute("sha1"),
                            SHA256 = reader.GetAttribute("sha256"),
                            SpamSum = reader.GetAttribute("spamsum"),

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        };

                        media.CopyMachineInformation(machine);

                        // Now process and add the media
                        ParseAddHelper(media, statsOnly);

                        reader.Read();
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

                        // Now process and add the release
                        ParseAddHelper(release, statsOnly);

                        reader.Read();
                        break;

                    case "rom":
                        containsItems = true;

                        DatItem rom = new Rom
                        {
                            Name = reader.GetAttribute("name"),
                            Size = Utilities.CleanLong(reader.GetAttribute("size")),
                            CRC = reader.GetAttribute("crc"),
                            MD5 = reader.GetAttribute("md5"),
                            SHA1 = reader.GetAttribute("sha1"),
                            SHA256 = reader.GetAttribute("sha256"),
                            SHA384 = reader.GetAttribute("sha384"),
                            SHA512 = reader.GetAttribute("sha512"),
                            SpamSum = reader.GetAttribute("spamsum"),
                            MergeTag = reader.GetAttribute("merge"),
                            ItemStatus = reader.GetAttribute("status").AsItemStatus(),
                            Date = CleanDate(reader.GetAttribute("date")),
                            Inverted = reader.GetAttribute("inverted").AsYesNo(),
                            MIA = reader.GetAttribute("mia").AsYesNo(),

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        };

                        rom.CopyMachineInformation(machine);

                        // Now process and add the rom
                        ParseAddHelper(rom, statsOnly);

                        reader.Read();
                        break;

                    case "sample":
                        containsItems = true;

                        DatItem sample = new Sample
                        {
                            Name = reader.GetAttribute("name"),

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        };

                        sample.CopyMachineInformation(machine);

                        // Now process and add the sample
                        ParseAddHelper(sample, statsOnly);

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
        /// Read EmuArc information
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
                        machine.TitleID = reader.ReadElementContentAsString();
                        break;

                    case "publisher":
                        machine.Publisher = reader.ReadElementContentAsString();
                        break;

                    case "developer":
                        machine.Developer = reader.ReadElementContentAsString();
                        break;

                    case "year":
                        machine.Year = reader.ReadElementContentAsString();
                        break;

                    case "genre":
                        machine.Genre = reader.ReadElementContentAsString();
                        break;

                    case "subgenre":
                        machine.Subgenre = reader.ReadElementContentAsString();
                        break;

                    case "ratings":
                        machine.Ratings = reader.ReadElementContentAsString();
                        break;

                    case "score":
                        machine.Score = reader.ReadElementContentAsString();
                        break;

                    case "players":
                        machine.Players = reader.ReadElementContentAsString();
                        break;

                    case "enabled":
                        machine.Enabled = reader.ReadElementContentAsString();
                        break;

                    case "crc":
                        machine.Crc = reader.ReadElementContentAsString().AsYesNo();
                        break;

                    case "source":
                        machine.SourceFile = reader.ReadElementContentAsString();
                        break;

                    case "cloneof":
                        machine.CloneOf = reader.ReadElementContentAsString();
                        break;

                    case "relatedto":
                        machine.RelatedTo = reader.ReadElementContentAsString();
                        break;

                    default:
                        reader.Read();
                        break;
                }
            }
        }
    }
}
