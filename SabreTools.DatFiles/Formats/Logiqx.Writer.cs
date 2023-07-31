using System;
using System.Collections.Generic;
using System.IO;
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
    /// Represents parsing and writing of a Logiqx-derived DAT
    /// </summary>
    internal partial class Logiqx : DatFile
    {
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
        protected override List<DatItemField> GetMissingRequiredFields(DatItem datItem)
        {
           var missingFields = new List<DatItemField>();
            switch (datItem)
            {
                case Release release:
                    if (string.IsNullOrWhiteSpace(release.Name))
                        missingFields.Add(DatItemField.Name);
                    if (string.IsNullOrWhiteSpace(release.Region))
                        missingFields.Add(DatItemField.Region);
                    break;

                case BiosSet biosset:
                    if (string.IsNullOrWhiteSpace(biosset.Name))
                        missingFields.Add(DatItemField.Name);
                    if (string.IsNullOrWhiteSpace(biosset.Description))
                        missingFields.Add(DatItemField.Description);
                    break;

                case Rom rom:
                    if (string.IsNullOrWhiteSpace(rom.Name))
                        missingFields.Add(DatItemField.Name);
                    if (rom.Size == null || rom.Size < 0)
                        missingFields.Add(DatItemField.Size);
                    if (string.IsNullOrWhiteSpace(rom.CRC)
                        && string.IsNullOrWhiteSpace(rom.MD5)
                        && string.IsNullOrWhiteSpace(rom.SHA1)
                        && string.IsNullOrWhiteSpace(rom.SHA256)
                        && string.IsNullOrWhiteSpace(rom.SHA384)
                        && string.IsNullOrWhiteSpace(rom.SHA512)
                        && string.IsNullOrWhiteSpace(rom.SpamSum))
                    {
                        missingFields.Add(DatItemField.SHA1);
                    }
                    break;

                case Disk disk:
                    if (string.IsNullOrWhiteSpace(disk.Name))
                        missingFields.Add(DatItemField.Name);
                    if (string.IsNullOrWhiteSpace(disk.MD5)
                        && string.IsNullOrWhiteSpace(disk.SHA1))
                    {
                        missingFields.Add(DatItemField.SHA1);
                    }
                    break;

                case Media media:
                    if (string.IsNullOrWhiteSpace(media.Name))
                        missingFields.Add(DatItemField.Name);
                    if (string.IsNullOrWhiteSpace(media.MD5)
                        && string.IsNullOrWhiteSpace(media.SHA1)
                        && string.IsNullOrWhiteSpace(media.SHA256)
                        && string.IsNullOrWhiteSpace(media.SpamSum))
                    {
                        missingFields.Add(DatItemField.SHA1);
                    }
                    break;

                case DeviceReference deviceref:
                    if (string.IsNullOrWhiteSpace(deviceref.Name))
                        missingFields.Add(DatItemField.Name);
                    break;

                case Sample sample:
                    if (string.IsNullOrWhiteSpace(sample.Name))
                        missingFields.Add(DatItemField.Name);
                    break;

                case Archive archive:
                    if (string.IsNullOrWhiteSpace(archive.Name))
                        missingFields.Add(DatItemField.Name);
                    break;

                case Driver driver:
                    if (!driver.StatusSpecified)
                        missingFields.Add(DatItemField.SupportStatus);
                    if (!driver.EmulationSpecified)
                        missingFields.Add(DatItemField.EmulationStatus);
                    if (!driver.CocktailSpecified)
                        missingFields.Add(DatItemField.CocktailStatus);
                    if (!driver.SaveStateSpecified)
                        missingFields.Add(DatItemField.SaveStateStatus);
                    break;

                case DatItems.Formats.SoftwareList softwarelist:
                    if (string.IsNullOrWhiteSpace(softwarelist.Tag))
                        missingFields.Add(DatItemField.Tag);
                    if (string.IsNullOrWhiteSpace(softwarelist.Name))
                        missingFields.Add(DatItemField.Name);
                    if (!softwarelist.StatusSpecified)
                        missingFields.Add(DatItemField.SoftwareListStatus);
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
            if (Header.NoIntroID == null)
                xtw.WriteDocType("datafile", "-//Logiqx//DTD ROM Management Datafile//EN", "http://www.logiqx.com/Dats/datafile.dtd", null);

            xtw.WriteStartElement("datafile");
            xtw.WriteOptionalAttributeString("build", Header.Build);
            xtw.WriteOptionalAttributeString("debug", Header.Debug.FromYesNo());
            if (Header.NoIntroID != null)
            {
                xtw.WriteRequiredAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
                xtw.WriteRequiredAttributeString("xsi:schemaLocation", "https://datomatic.no-intro.org/stuff https://datomatic.no-intro.org/stuff/schema_nointro_datfile_v3.xsd");
            }

            xtw.WriteStartElement("header");

            xtw.WriteOptionalElementString("id", Header.NoIntroID);
            xtw.WriteRequiredElementString("name", Header.Name);
            xtw.WriteRequiredElementString("description", Header.Description);
            xtw.WriteOptionalElementString("rootdir", Header.RootDir);
            if (!string.IsNullOrWhiteSpace(Header.Category))
            {
                var categories = Header.Category.Split(';');
                foreach (string category in categories)
                {
                    xtw.WriteOptionalElementString("category", category);
                }
            }
            xtw.WriteRequiredElementString("version", Header.Version);
            xtw.WriteOptionalElementString("date", Header.Date);
            xtw.WriteRequiredElementString("author", Header.Author);
            xtw.WriteOptionalElementString("email", Header.Email);
            xtw.WriteOptionalElementString("homepage", Header.Homepage);
            xtw.WriteOptionalElementString("url", Header.Url);
            xtw.WriteOptionalElementString("comment", Header.Comment);
            xtw.WriteOptionalElementString("type", Header.Type);

            if (Header.ForcePacking != PackingFlag.None
                || Header.ForceMerging != MergingFlag.None
                || Header.ForceNodump != NodumpFlag.None
                || !string.IsNullOrWhiteSpace(Header.HeaderSkipper))
            {
                xtw.WriteStartElement("clrmamepro");

                if (Header.ForcePacking != PackingFlag.None)
                    xtw.WriteOptionalAttributeString("forcepacking", Header.ForcePacking.FromPackingFlag(false));
                if (Header.ForceMerging != MergingFlag.None)
                    xtw.WriteOptionalAttributeString("forcemerging", Header.ForceMerging.FromMergingFlag(false));
                if (Header.ForceNodump != NodumpFlag.None)
                    xtw.WriteOptionalAttributeString("forcenodump", Header.ForceNodump.FromNodumpFlag());
                xtw.WriteOptionalAttributeString("header", Header.HeaderSkipper);

                // End clrmamepro
                xtw.WriteEndElement();
            }

            if (Header.System != null
                || Header.RomMode != MergingFlag.None || Header.LockRomMode != null
                || Header.BiosMode != MergingFlag.None || Header.LockBiosMode != null
                || Header.SampleMode != MergingFlag.None || Header.LockSampleMode != null)
            {
                xtw.WriteStartElement("romcenter");

                xtw.WriteOptionalAttributeString("plugin", Header.System);
                if (Header.RomMode != MergingFlag.None)
                    xtw.WriteOptionalAttributeString("rommode", Header.RomMode.FromMergingFlag(true));
                if (Header.BiosMode != MergingFlag.None)
                    xtw.WriteOptionalAttributeString("biosmode", Header.BiosMode.FromMergingFlag(true));
                if (Header.SampleMode != MergingFlag.None)
                    xtw.WriteOptionalAttributeString("samplemode", Header.SampleMode.FromMergingFlag(true));
                xtw.WriteOptionalAttributeString("lockrommode", Header.LockRomMode.FromYesNo());
                xtw.WriteOptionalAttributeString("lockbiosmode", Header.LockBiosMode.FromYesNo());
                xtw.WriteOptionalAttributeString("locksamplemode", Header.LockSampleMode.FromYesNo());

                // End romcenter
                xtw.WriteEndElement();
            }

            // End header
            xtw.WriteEndElement();

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
            xtw.WriteStartElement(_deprecated ? "game" : "machine");
            xtw.WriteRequiredAttributeString("name", datItem.Machine.Name);

            if (datItem.Machine.MachineType.HasFlag(MachineType.Bios))
                xtw.WriteAttributeString("isbios", "yes");
            if (datItem.Machine.MachineType.HasFlag(MachineType.Device))
                xtw.WriteAttributeString("isdevice", "yes");
            if (datItem.Machine.MachineType.HasFlag(MachineType.Mechanical))
                xtw.WriteAttributeString("ismechanical", "yes");

            xtw.WriteOptionalAttributeString("runnable", datItem.Machine.Runnable.FromRunnable());
            xtw.WriteOptionalAttributeString("id", datItem.Machine.NoIntroId);
            xtw.WriteOptionalAttributeString("cloneofid", datItem.Machine.NoIntroCloneOfId);

            if (!string.Equals(datItem.Machine.Name, datItem.Machine.CloneOf, StringComparison.OrdinalIgnoreCase))
                xtw.WriteOptionalAttributeString("cloneof", datItem.Machine.CloneOf);
            if (!string.Equals(datItem.Machine.Name, datItem.Machine.RomOf, StringComparison.OrdinalIgnoreCase))
                xtw.WriteOptionalAttributeString("romof", datItem.Machine.RomOf);
            if (!string.Equals(datItem.Machine.Name, datItem.Machine.SampleOf, StringComparison.OrdinalIgnoreCase))
                xtw.WriteOptionalAttributeString("sampleof", datItem.Machine.SampleOf);

            xtw.WriteOptionalElementString("comment", datItem.Machine.Comment);
            xtw.WriteOptionalElementString("description", datItem.Machine.Description);
            xtw.WriteOptionalElementString("year", datItem.Machine.Year);
            xtw.WriteOptionalElementString("publisher", datItem.Machine.Publisher);
            xtw.WriteOptionalElementString("manufacturer", datItem.Machine.Manufacturer);
            xtw.WriteOptionalElementString("category", datItem.Machine.Category);

            if (datItem.Machine.TitleID != null
                || datItem.Machine.Developer != null
                || datItem.Machine.Genre != null
                || datItem.Machine.Subgenre != null
                || datItem.Machine.Ratings != null
                || datItem.Machine.Score != null
                || datItem.Machine.Enabled != null
                || datItem.Machine.Crc != null
                || datItem.Machine.RelatedTo != null)
            {
                xtw.WriteStartElement("trurip");

                xtw.WriteOptionalElementString("titleid", datItem.Machine.TitleID);
                xtw.WriteOptionalElementString("publisher", datItem.Machine.Publisher);
                xtw.WriteOptionalElementString("developer", datItem.Machine.Developer);
                xtw.WriteOptionalElementString("year", datItem.Machine.Year);
                xtw.WriteOptionalElementString("genre", datItem.Machine.Genre);
                xtw.WriteOptionalElementString("subgenre", datItem.Machine.Subgenre);
                xtw.WriteOptionalElementString("ratings", datItem.Machine.Ratings);
                xtw.WriteOptionalElementString("score", datItem.Machine.Score);
                xtw.WriteOptionalElementString("players", datItem.Machine.Players);
                xtw.WriteOptionalElementString("enabled", datItem.Machine.Enabled);
                xtw.WriteOptionalElementString("titleid", datItem.Machine.TitleID);
                xtw.WriteOptionalElementString("crc", datItem.Machine.Crc.FromYesNo());
                xtw.WriteOptionalElementString("source", datItem.Machine.SourceFile);
                xtw.WriteOptionalElementString("cloneof", datItem.Machine.CloneOf);
                xtw.WriteOptionalElementString("relatedto", datItem.Machine.RelatedTo);

                // End trurip
                xtw.WriteEndElement();
            }

            xtw.Flush();
        }

        /// <summary>
        /// Write out Game end using the supplied StreamWriter
        /// </summary>
        /// <param name="xtw">XmlTextWriter to output to</param>
        private void WriteEndGame(XmlTextWriter xtw)
        {
            // End machine
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
                case ItemType.Archive:
                    var archive = datItem as Archive;
                    xtw.WriteStartElement("archive");
                    xtw.WriteRequiredAttributeString("name", archive.Name);
                    xtw.WriteEndElement();
                    break;

                case ItemType.BiosSet:
                    var biosSet = datItem as BiosSet;
                    xtw.WriteStartElement("biosset");
                    xtw.WriteRequiredAttributeString("name", biosSet.Name);
                    xtw.WriteOptionalAttributeString("description", biosSet.Description);
                    xtw.WriteOptionalAttributeString("default", biosSet.Default.FromYesNo());
                    xtw.WriteEndElement();
                    break;

                case ItemType.Disk:
                    var disk = datItem as Disk;
                    xtw.WriteStartElement("disk");
                    xtw.WriteRequiredAttributeString("name", disk.Name);
                    xtw.WriteOptionalAttributeString("md5", disk.MD5?.ToLowerInvariant());
                    xtw.WriteOptionalAttributeString("sha1", disk.SHA1?.ToLowerInvariant());
                    if (disk.ItemStatus != ItemStatus.None)
                        xtw.WriteOptionalAttributeString("status", disk.ItemStatus.FromItemStatus(false));
                    xtw.WriteEndElement();
                    break;

                case ItemType.Media:
                    var media = datItem as Media;
                    xtw.WriteStartElement("media");
                    xtw.WriteRequiredAttributeString("name", media.Name);
                    xtw.WriteOptionalAttributeString("md5", media.MD5?.ToLowerInvariant());
                    xtw.WriteOptionalAttributeString("sha1", media.SHA1?.ToLowerInvariant());
                    xtw.WriteOptionalAttributeString("sha256", media.SHA256?.ToLowerInvariant());
                    xtw.WriteOptionalAttributeString("spamsum", media.SpamSum?.ToLowerInvariant());
                    xtw.WriteEndElement();
                    break;

                case ItemType.Release:
                    var release = datItem as Release;
                    xtw.WriteStartElement("release");
                    xtw.WriteRequiredAttributeString("name", release.Name);
                    xtw.WriteOptionalAttributeString("region", release.Region);
                    xtw.WriteOptionalAttributeString("language", release.Language);
                    xtw.WriteOptionalAttributeString("date", release.Date);
                    xtw.WriteOptionalAttributeString("default", release.Default.FromYesNo());
                    xtw.WriteEndElement();
                    break;

                case ItemType.Rom:
                    var rom = datItem as Rom;
                    xtw.WriteStartElement("rom");
                    xtw.WriteRequiredAttributeString("name", rom.Name);
                    xtw.WriteAttributeString("size", rom.Size?.ToString());
                    xtw.WriteOptionalAttributeString("crc", rom.CRC?.ToLowerInvariant());
                    xtw.WriteOptionalAttributeString("md5", rom.MD5?.ToLowerInvariant());
                    xtw.WriteOptionalAttributeString("sha1", rom.SHA1?.ToLowerInvariant());
                    xtw.WriteOptionalAttributeString("sha256", rom.SHA256?.ToLowerInvariant());
                    xtw.WriteOptionalAttributeString("sha384", rom.SHA384?.ToLowerInvariant());
                    xtw.WriteOptionalAttributeString("sha512", rom.SHA512?.ToLowerInvariant());
                    xtw.WriteOptionalAttributeString("spamsum", rom.SpamSum?.ToLowerInvariant());
                    xtw.WriteOptionalAttributeString("date", rom.Date);
                    if (rom.ItemStatus != ItemStatus.None)
                        xtw.WriteOptionalAttributeString("status", rom.ItemStatus.FromItemStatus(false));
                    xtw.WriteOptionalAttributeString("inverted", rom.Inverted.FromYesNo());
                    xtw.WriteOptionalAttributeString("mia", rom.MIA.FromYesNo());
                    xtw.WriteEndElement();
                    break;

                case ItemType.Sample:
                    var sample = datItem as Sample;
                    xtw.WriteStartElement("sample");
                    xtw.WriteRequiredAttributeString("name", sample.Name);
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
            // End machine
            xtw.WriteEndElement();

            // End datafile
            xtw.WriteEndElement();

            xtw.Flush();
        }
    }
}
