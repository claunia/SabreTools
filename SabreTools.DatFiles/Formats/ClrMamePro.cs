using System;
using System.IO;
using System.Text;

using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.IO;
using SabreTools.IO.Readers;
using SabreTools.IO.Writers;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing and writing of a ClrMamePro DAT
    /// </summary>
    /// <remarks>
    /// TODO: Check and enforce required fields in output
    /// </remarks>
    internal class ClrMamePro : DatFile
    {
        #region Fields

        /// <summary>
        /// Get whether to assume quote usage on read and write or not
        /// </summary>
        public bool Quotes { get; set; } = true;

        #endregion

        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        /// <param name="quotes">Enable quotes on read and write, false otherwise</param>
        public ClrMamePro(DatFile datFile, bool quotes)
            : base(datFile)
        {
            Quotes = quotes;
        }

        /// <inheritdoc/>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            // Open a file reader
            Encoding enc = filename.GetEncoding();
            ClrMameProReader cmpr = new ClrMameProReader(File.OpenRead(filename), enc)
            {
                DosCenter = false,
                Quotes = Quotes,
            };

            while (!cmpr.EndOfStream)
            {
                try
                {
                    cmpr.ReadNextLine();

                    // Ignore everything not top-level
                    if (cmpr.RowType != CmpRowType.TopLevel)
                        continue;

                    // Switch on the top-level name
                    switch (cmpr.TopLevel.ToLowerInvariant())
                    {
                        // Header values
                        case "clrmamepro":
                        case "romvault":
                            ReadHeader(cmpr, keep);
                            break;

                        // Sets
                        case "set":         // Used by the most ancient DATs
                        case "game":        // Used by most CMP DATs
                        case "machine":     // Possibly used by MAME CMP DATs
                            ReadSet(cmpr, false, statsOnly, filename, indexId);
                            break;
                        case "resource":    // Used by some other DATs to denote a BIOS set
                            ReadSet(cmpr, true, statsOnly, filename, indexId);
                            break;

                        default:
                            break;
                    }
                }
                catch (Exception ex) when (!throwOnError)
                {
                    string message = $"'{filename}' - There was an error parsing line {cmpr.LineNumber} '{cmpr.CurrentLine}'";
                    logger.Error(ex, message);
                }
            }

            cmpr.Dispose();
        }

        /// <summary>
        /// Read header information
        /// </summary>
        /// <param name="cmpr">ClrMameProReader to use to parse the header</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        private void ReadHeader(ClrMameProReader cmpr, bool keep)
        {
            bool superdat = false;

            // If there's no subtree to the header, skip it
            if (cmpr == null || cmpr.EndOfStream)
                return;

            // While we don't hit an end element or end of stream
            while (!cmpr.EndOfStream)
            {
                cmpr.ReadNextLine();

                // Ignore comments, internal items, and nothingness
                if (cmpr.RowType == CmpRowType.None || cmpr.RowType == CmpRowType.Comment || cmpr.RowType == CmpRowType.Internal)
                    continue;

                // If we reached the end of a section, break
                if (cmpr.RowType == CmpRowType.EndTopLevel)
                    break;

                // If the standalone value is null, we skip
                if (cmpr.Standalone == null)
                    continue;

                string itemKey = cmpr.Standalone?.Key.ToLowerInvariant();
                string itemVal = cmpr.Standalone?.Value;

                // For all other cases
                switch (itemKey)
                {
                    case "name":
                        Header.Name ??= itemVal;
                        superdat |= itemVal.Contains(" - SuperDAT");

                        if (keep && superdat)
                            Header.Type ??= "SuperDAT";

                        break;
                    case "description":
                        Header.Description ??= itemVal;
                        break;
                    case "rootdir":
                        Header.RootDir ??= itemVal;
                        break;
                    case "category":
                        Header.Category ??= itemVal;
                        break;
                    case "version":
                        Header.Version ??= itemVal;
                        break;
                    case "date":
                        Header.Date ??= itemVal;
                        break;
                    case "author":
                        Header.Author ??= itemVal;
                        break;
                    case "email":
                        Header.Email ??= itemVal;
                        break;
                    case "homepage":
                        Header.Homepage ??= itemVal;
                        break;
                    case "url":
                        Header.Url ??= itemVal;
                        break;
                    case "comment":
                        Header.Comment ??= itemVal;
                        break;
                    case "header":
                        Header.HeaderSkipper ??= itemVal;
                        break;
                    case "type":
                        Header.Type ??= itemVal;
                        superdat |= itemVal.Contains("SuperDAT");
                        break;
                    case "forcemerging":
                        if (Header.ForceMerging == MergingFlag.None)
                            Header.ForceMerging = itemVal.AsMergingFlag();

                        break;
                    case "forcezipping":
                        if (Header.ForcePacking == PackingFlag.None)
                            Header.ForcePacking = itemVal.AsPackingFlag();

                        break;
                    case "forcepacking":
                        if (Header.ForcePacking == PackingFlag.None)
                            Header.ForcePacking = itemVal.AsPackingFlag();

                        break;
                }
            }
        }

        /// <summary>
        /// Read set information
        /// </summary>
        /// <param name="cmpr">ClrMameProReader to use to parse the header</param>
        /// <param name="resource">True if the item is a resource (bios), false otherwise</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        private void ReadSet(
            ClrMameProReader cmpr,
            bool resource,
            bool statsOnly,

            // Standard Dat parsing
            string filename,
            int indexId)
        {
            // Prepare all internal variables
            bool containsItems = false;
            Machine machine = new Machine()
            {
                MachineType = (resource ? MachineType.Bios : MachineType.NULL),
            };

            // If there's no subtree to the header, skip it
            if (cmpr == null || cmpr.EndOfStream)
                return;

            // While we don't hit an end element or end of stream
            while (!cmpr.EndOfStream)
            {
                cmpr.ReadNextLine();

                // Ignore comments and nothingness
                if (cmpr.RowType == CmpRowType.None || cmpr.RowType == CmpRowType.Comment)
                    continue;

                // If we reached the end of a section, break
                if (cmpr.RowType == CmpRowType.EndTopLevel)
                    break;

                // Handle any standalone items
                if (cmpr.RowType == CmpRowType.Standalone && cmpr.Standalone != null)
                {
                    string itemKey = cmpr.Standalone?.Key.ToLowerInvariant();
                    string itemVal = cmpr.Standalone?.Value;

                    switch (itemKey)
                    {
                        case "name":
                            machine.Name = itemVal;
                            break;
                        case "description":
                            machine.Description = itemVal;
                            break;
                        case "year":
                            machine.Year = itemVal;
                            break;
                        case "manufacturer":
                            machine.Manufacturer = itemVal;
                            break;
                        case "category":
                            machine.Category = itemVal;
                            break;
                        case "cloneof":
                            machine.CloneOf = itemVal;
                            break;
                        case "romof":
                            machine.RomOf = itemVal;
                            break;
                        case "sampleof":
                            machine.SampleOf = itemVal;
                            break;
                    }
                }

                // Handle any internal items
                else if (cmpr.RowType == CmpRowType.Internal
                    && !string.IsNullOrWhiteSpace(cmpr.InternalName)
                    && cmpr.Internal != null)
                {
                    containsItems = true;
                    string itemKey = cmpr.InternalName;

                    // Create the proper DatItem based on the type
                    ItemType itemType = itemKey.AsItemType() ?? ItemType.Rom;
                    DatItem item = DatItem.Create(itemType);

                    // Then populate it with information
                    item.CopyMachineInformation(machine);

                    item.Source.Index = indexId;
                    item.Source.Name = filename;

                    // Loop through all of the attributes
                    foreach (var kvp in cmpr.Internal)
                    {
                        string attrKey = kvp.Key;
                        string attrVal = kvp.Value;

                        switch (attrKey)
                        {
                            //If the item is empty, we automatically skip it because it's a fluke
                            case "":
                                continue;

                            // Regular attributes
                            case "name":
                                item.SetName(attrVal);
                                break;

                            case "size":
                                if (item.ItemType == ItemType.Rom)
                                    (item as Rom).Size = Utilities.CleanLong(attrVal);

                                break;
                            case "crc":
                                if (item.ItemType == ItemType.Rom)
                                    (item as Rom).CRC = attrVal;

                                break;
                            case "md5":
                                if (item.ItemType == ItemType.Disk)
                                    (item as Disk).MD5 = attrVal;
                                else if (item.ItemType == ItemType.Media)
                                    (item as Media).MD5 = attrVal;
                                else if (item.ItemType == ItemType.Rom)
                                    (item as Rom).MD5 = attrVal;

                                break;
                            case "sha1":
                                if (item.ItemType == ItemType.Disk)
                                    (item as Disk).SHA1 = attrVal;
                                else if (item.ItemType == ItemType.Media)
                                    (item as Media).SHA1 = attrVal;
                                else if (item.ItemType == ItemType.Rom)
                                    (item as Rom).SHA1 = attrVal;

                                break;
                            case "sha256":
                                if (item.ItemType == ItemType.Media)
                                    (item as Media).SHA256 = attrVal;
                                else if (item.ItemType == ItemType.Rom)
                                    (item as Rom).SHA256 = attrVal;
                                
                                break;
                            case "sha384":
                                if (item.ItemType == ItemType.Rom)
                                    (item as Rom).SHA384 = attrVal;

                                break;
                            case "sha512":
                                if (item.ItemType == ItemType.Rom)
                                    (item as Rom).SHA512 = attrVal;

                                break;
                            case "spamsum":
                                if (item.ItemType == ItemType.Media)
                                    (item as Media).SpamSum = attrVal;
                                else if (item.ItemType == ItemType.Rom)
                                    (item as Rom).SpamSum = attrVal;

                                break;
                            case "status":
                                ItemStatus tempFlagStatus = attrVal.AsItemStatus();
                                if (item.ItemType == ItemType.Disk)
                                    (item as Disk).ItemStatus = tempFlagStatus;
                                else if (item.ItemType == ItemType.Rom)
                                    (item as Rom).ItemStatus = tempFlagStatus;

                                break;
                            case "date":
                                if (item.ItemType == ItemType.Release)
                                    (item as Release).Date = attrVal;
                                else if (item.ItemType == ItemType.Rom)
                                    (item as Rom).Date = attrVal;

                                break;
                            case "default":
                                if (item.ItemType == ItemType.BiosSet)
                                    (item as BiosSet).Default = attrVal.AsYesNo();
                                else if (item.ItemType == ItemType.Release)
                                    (item as Release).Default = attrVal.AsYesNo();

                                break;
                            case "description":
                                if (item.ItemType == ItemType.BiosSet)
                                    (item as BiosSet).Description = attrVal;

                                break;
                            case "region":
                                if (item.ItemType == ItemType.Release)
                                    (item as Release).Region = attrVal;

                                break;
                            case "language":
                                if (item.ItemType == ItemType.Release)
                                    (item as Release).Language = attrVal;

                                break;
                        }
                    }

                    // Now process and add the rom
                    ParseAddHelper(item, statsOnly);
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
                ParseAddHelper(blank, statsOnly);
            }
        }

        /// <inheritdoc/>
        protected override ItemType[] GetSupportedTypes()
        {
            return new ItemType[]
            {
                ItemType.Archive,
                ItemType.BiosSet,
                ItemType.Disk,
                ItemType.Media,
                ItemType.Release,
                ItemType.Rom,
                ItemType.Sample,
            };
        }

        /// <inheritdoc/>
        public override bool WriteToFile(string outfile, bool ignoreblanks = false, bool throwOnError = false)
        {
            try
            {
                logger.User($"Writing to '{outfile}'...");
                FileStream fs = File.Create(outfile);

                // If we get back null for some reason, just log and return
                if (fs == null)
                {
                    logger.Warning($"File '{outfile}' could not be created for writing! Please check to see if the file is writable");
                    return false;
                }

                ClrMameProWriter cmpw = new ClrMameProWriter(fs, new UTF8Encoding(false))
                {
                    Quotes = Quotes
                };

                // Write out the header
                WriteHeader(cmpw);

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
                            WriteEndGame(cmpw, datItem);

                        // If we have a new game, output the beginning of the new item
                        if (lastgame == null || lastgame.ToLowerInvariant() != datItem.Machine.Name.ToLowerInvariant())
                            WriteStartGame(cmpw, datItem);

                        // Check for a "null" item
                        datItem = ProcessNullifiedItem(datItem);

                        // Write out the item if we're not ignoring
                        if (!ShouldIgnore(datItem, ignoreblanks))
                            WriteDatItem(cmpw, datItem);

                        // Set the new data to compare against
                        lastgame = datItem.Machine.Name;
                    }
                }

                // Write the file footer out
                WriteFooter(cmpw);

                logger.User($"'{outfile}' written!{Environment.NewLine}");
                cmpw.Dispose();
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
        /// <param name="cmpw">ClrMameProWriter to output to</param>
        private void WriteHeader(ClrMameProWriter cmpw)
        {
            cmpw.WriteStartElement("clrmamepro");

            cmpw.WriteRequiredStandalone("name", Header.Name);
            cmpw.WriteRequiredStandalone("description", Header.Description);
            cmpw.WriteOptionalStandalone("category", Header.Category);
            cmpw.WriteRequiredStandalone("version", Header.Version);
            cmpw.WriteOptionalStandalone("date", Header.Date);
            cmpw.WriteRequiredStandalone("author", Header.Author);
            cmpw.WriteOptionalStandalone("email", Header.Email);
            cmpw.WriteOptionalStandalone("homepage", Header.Homepage);
            cmpw.WriteOptionalStandalone("url", Header.Url);
            cmpw.WriteOptionalStandalone("comment", Header.Comment);
            cmpw.WriteOptionalStandalone("forcezipping", Header.ForcePacking.FromPackingFlag(true), false);
            cmpw.WriteOptionalStandalone("forcemerging", Header.ForceMerging.FromMergingFlag(false), false);

            // End clrmamepro
            cmpw.WriteEndElement();

            cmpw.Flush();
        }

        /// <summary>
        /// Write out Game start using the supplied StreamWriter
        /// </summary>
        /// <param name="cmpw">ClrMameProWriter to output to</param>
        /// <param name="datItem">DatItem object to be output</param>
        private void WriteStartGame(ClrMameProWriter cmpw, DatItem datItem)
        {
            // No game should start with a path separator
            datItem.Machine.Name = datItem.Machine.Name.TrimStart(Path.DirectorySeparatorChar);

            // Build the state
            cmpw.WriteStartElement(datItem.Machine.MachineType == MachineType.Bios ? "resource" : "game");

            cmpw.WriteRequiredStandalone("name", datItem.Machine.Name);
            cmpw.WriteOptionalStandalone("romof", datItem.Machine.RomOf);
            cmpw.WriteOptionalStandalone("cloneof", datItem.Machine.CloneOf);
            cmpw.WriteOptionalStandalone("description", datItem.Machine.Description ?? datItem.Machine.Name);
            cmpw.WriteOptionalStandalone("year", datItem.Machine.Year);
            cmpw.WriteOptionalStandalone("manufacturer", datItem.Machine.Manufacturer);
            cmpw.WriteOptionalStandalone("category", datItem.Machine.Category);

            cmpw.Flush();
        }

        /// <summary>
        /// Write out Game end using the supplied StreamWriter
        /// </summary>
        /// <param name="cmpw">ClrMameProWriter to output to</param>
        /// <param name="datItem">DatItem object to be output</param>
        private void WriteEndGame(ClrMameProWriter cmpw, DatItem datItem)
        {
            // Build the state
            cmpw.WriteOptionalStandalone("sampleof", datItem.Machine.SampleOf);

            // End game
            cmpw.WriteEndElement();

            cmpw.Flush();
        }

        /// <summary>
        /// Write out DatItem using the supplied StreamWriter
        /// </summary>
        /// <param name="datFile">DatFile to write out from</param>
        /// <param name="cmpw">ClrMameProWriter to output to</param>
        /// <param name="datItem">DatItem object to be output</param>
        private void WriteDatItem(ClrMameProWriter cmpw, DatItem datItem)
        {
            // Pre-process the item name
            ProcessItemName(datItem, true);

            // Build the state
            switch (datItem.ItemType)
            {
                case ItemType.Archive:
                    var archive = datItem as Archive;
                    cmpw.WriteStartElement("archive");
                    cmpw.WriteRequiredAttributeString("name", archive.Name);
                    cmpw.WriteEndElement();
                    break;

                case ItemType.BiosSet:
                    var biosSet = datItem as BiosSet;
                    cmpw.WriteStartElement("biosset");
                    cmpw.WriteRequiredAttributeString("name", biosSet.Name);
                    cmpw.WriteOptionalAttributeString("description", biosSet.Description);
                    cmpw.WriteOptionalAttributeString("default", biosSet.Default?.ToString().ToLowerInvariant());
                    cmpw.WriteEndElement();
                    break;

                case ItemType.Disk:
                    var disk = datItem as Disk;
                    cmpw.WriteStartElement("disk");
                    cmpw.WriteRequiredAttributeString("name", disk.Name);
                    cmpw.WriteOptionalAttributeString("md5", disk.MD5?.ToLowerInvariant(), quoteOverride: false);
                    cmpw.WriteOptionalAttributeString("sha1", disk.SHA1?.ToLowerInvariant(), quoteOverride: false);
                    cmpw.WriteOptionalAttributeString("flags", disk.ItemStatus.FromItemStatus(false));
                    cmpw.WriteEndElement();
                    break;

                case ItemType.Media:
                    var media = datItem as Media;
                    cmpw.WriteStartElement("media");
                    cmpw.WriteRequiredAttributeString("name", media.Name);
                    cmpw.WriteOptionalAttributeString("md5", media.MD5?.ToLowerInvariant(), quoteOverride: false);
                    cmpw.WriteOptionalAttributeString("sha1", media.SHA1?.ToLowerInvariant(), quoteOverride: false);
                    cmpw.WriteOptionalAttributeString("sha256", media.SHA256?.ToLowerInvariant(), quoteOverride: false);
                    cmpw.WriteOptionalAttributeString("spamsum", media.SpamSum?.ToLowerInvariant());
                    cmpw.WriteEndElement();
                    break;

                case ItemType.Release:
                    var release = datItem as Release;
                    cmpw.WriteStartElement("release");
                    cmpw.WriteRequiredAttributeString("name", release.Name);
                    cmpw.WriteOptionalAttributeString("region", release.Region);
                    cmpw.WriteOptionalAttributeString("language", release.Language);
                    cmpw.WriteOptionalAttributeString("date", release.Date);
                    cmpw.WriteOptionalAttributeString("default", release.Default?.ToString().ToLowerInvariant());
                    cmpw.WriteEndElement();
                    break;

                case ItemType.Rom:
                    var rom = datItem as Rom;
                    cmpw.WriteStartElement("rom");
                    cmpw.WriteRequiredAttributeString("name", rom.Name);
                    cmpw.WriteOptionalAttributeString("size", rom.Size?.ToString(), quoteOverride: false);
                    cmpw.WriteOptionalAttributeString("crc", rom.CRC?.ToLowerInvariant(), quoteOverride: false);
                    cmpw.WriteOptionalAttributeString("md5", rom.MD5?.ToLowerInvariant(), quoteOverride: false);
                    cmpw.WriteOptionalAttributeString("sha1", rom.SHA1?.ToLowerInvariant(), quoteOverride: false);
                    cmpw.WriteOptionalAttributeString("sha256", rom.SHA256?.ToLowerInvariant(), quoteOverride: false);
                    cmpw.WriteOptionalAttributeString("sha384", rom.SHA384?.ToLowerInvariant(), quoteOverride: false);
                    cmpw.WriteOptionalAttributeString("sha512", rom.SHA512?.ToLowerInvariant(), quoteOverride: false);
                    cmpw.WriteOptionalAttributeString("spamsum", rom.SpamSum?.ToLowerInvariant(), quoteOverride: false);
                    cmpw.WriteOptionalAttributeString("date", rom.Date);
                    cmpw.WriteOptionalAttributeString("flags", rom.ItemStatus.FromItemStatus(false));
                    cmpw.WriteEndElement();
                    break;

                case ItemType.Sample:
                    var sample = datItem as Sample;
                    cmpw.WriteStartElement("sample");
                    cmpw.WriteRequiredAttributeString("name", sample.Name);
                    cmpw.WriteEndElement();
                    break;
            }

            cmpw.Flush();
        }

        /// <summary>
        /// Write out DAT footer using the supplied StreamWriter
        /// </summary>
        /// <param name="cmpw">ClrMameProWriter to output to</param>
        private void WriteFooter(ClrMameProWriter cmpw)
        {
            // End game
            cmpw.WriteEndElement();

            cmpw.Flush();
        }
    }
}
