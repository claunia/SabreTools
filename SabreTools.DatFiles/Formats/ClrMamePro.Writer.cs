using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.IO.Writers;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents writing of a ClrMamePro DAT
    /// </summary>
    internal partial class ClrMamePro : DatFile
    {
        /// <inheritdoc/>
        protected override ItemType[] GetSupportedTypes()
        {
            return new ItemType[]
            {
                ItemType.Archive,
                ItemType.BiosSet,
                //ItemType.Chip,
                //ItemType.DipSwitch,
                ItemType.Disk,
                //ItemType.Display,
                //ItemType.Driver,
                //ItemType.Input,
                ItemType.Media,
                ItemType.Release,
                ItemType.Rom,
                ItemType.Sample,
                //ItemType.Sound,
            };
        }

        /// <inheritdoc/>
        protected override List<DatItemField> GetMissingRequiredFields(DatItem datItem)
        {
            // TODO: Check required fields
            return null;
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

                ClrMameProWriter cmpw = new(fs, new UTF8Encoding(false))
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
            if (Header.ForcePacking != PackingFlag.None)
                cmpw.WriteOptionalStandalone("forcezipping", Header.ForcePacking.FromPackingFlag(true), false);
            if (Header.ForceMerging != MergingFlag.None)
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
        private static void WriteStartGame(ClrMameProWriter cmpw, DatItem datItem)
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
        private static void WriteEndGame(ClrMameProWriter cmpw, DatItem datItem)
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
                    if (disk.ItemStatus != ItemStatus.None)
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
                    if (rom.ItemStatus != ItemStatus.None)
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
        private static void WriteFooter(ClrMameProWriter cmpw)
        {
            // End game
            cmpw.WriteEndElement();

            cmpw.Flush();
        }
    }
}
