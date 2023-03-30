using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using SabreTools.Core;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.IO;
using SabreTools.IO.Writers;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing and writing of a hashfile such as an SFV, MD5, or SHA-1 file
    /// </summary>
    internal class Hashfile : DatFile
    {
        // Private instance variables specific to Hashfile DATs
        private readonly Hash _hash;

        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        /// <param name="hash">Type of hash that is associated with this DAT</param> 
        public Hashfile(DatFile datFile, Hash hash)
            : base(datFile)
        {
            _hash = hash;
        }

        /// <inheritdoc/>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            // Open a file reader
            Encoding enc = filename.GetEncoding();
            StreamReader sr = new StreamReader(File.OpenRead(filename), enc);

            while (!sr.EndOfStream)
            {
                try
                {
                    string line = sr.ReadLine();

                    // Split the line and get the name and hash
                    string[] split = Regex.Replace(line, @"\s+", " ").Split(' ');
                    string name = string.Empty;
                    string hash = string.Empty;

                    // If we have CRC, then it's an SFV file and the name is first
                    if (_hash.HasFlag(Hash.CRC))
                    {
                        name = split[0].Replace("*", String.Empty);
                        hash = split[1];
                    }
                    // Otherwise, the name is second
                    else
                    {
                        name = split[1].Replace("*", String.Empty);
                        hash = split[0];
                    }

                    // If the name contains a path, use that path as the machine
                    string machine = Path.GetFileNameWithoutExtension(filename);
                    if (name.Contains('/'))
                    {
                        split = name.Split('/');
                        machine = split[0];
                        name = name.Substring(machine.Length + 1);
                    }
                    else if (name.Contains('\\'))
                    {
                        split = name.Split('\\');
                        machine = split[0];
                        name = name.Substring(machine.Length + 1);
                    }

                    Rom rom = new Rom
                    {
                        Name = name,
                        Size = null,
                        CRC = (_hash.HasFlag(Hash.CRC) ? hash : null),
                        MD5 = (_hash.HasFlag(Hash.MD5) ? hash : null),
                        SHA1 = (_hash.HasFlag(Hash.SHA1) ? hash : null),
                        SHA256 = (_hash.HasFlag(Hash.SHA256) ? hash : null),
                        SHA384 = (_hash.HasFlag(Hash.SHA384) ? hash : null),
                        SHA512 = (_hash.HasFlag(Hash.SHA512) ? hash : null),
                        SpamSum = (_hash.HasFlag(Hash.SpamSum) ? hash : null),
                        ItemStatus = ItemStatus.None,

                        Machine = new Machine
                        {
                            Name = machine,
                        },

                        Source = new Source
                        {
                            Index = indexId,
                            Name = filename,
                        },
                    };

                    // Now process and add the rom
                    ParseAddHelper(rom, statsOnly);
                }
                catch (Exception ex) when (!throwOnError)
                {
                    string message = $"'{filename}' - There was an error parsing at position {sr.BaseStream.Position}";
                    logger.Error(ex, message);
                }
            }

            sr.Dispose();
        }

        /// <inheritdoc/>
        protected override ItemType[] GetSupportedTypes()
        {
            return new ItemType[] { ItemType.Disk, ItemType.Media, ItemType.Rom };
        }

        /// <inheritdoc/>
        protected override List<DatItemField> GetMissingRequiredFields(DatItem datItem)
        {
            List<DatItemField> missingFields = new List<DatItemField>();

            // Check item name
            if (string.IsNullOrWhiteSpace(datItem.GetName()))
                missingFields.Add(DatItemField.Name);

            // Check hash linked to specific Hashfile type
            switch (_hash)
            {
                case Hash.CRC:
                    switch (datItem.ItemType)
                    {
                        case ItemType.Rom:
                            if (!string.IsNullOrEmpty((datItem as Rom)?.CRC))
                                missingFields.Add(DatItemField.CRC);
                            break;
                        default:
                            missingFields.Add(DatItemField.CRC);
                            break;
                    }
                    break;
                case Hash.MD5:
                    switch (datItem.ItemType)
                    {
                        case ItemType.Disk:
                            if (!string.IsNullOrEmpty((datItem as Disk)?.MD5))
                                missingFields.Add(DatItemField.MD5);
                            break;
                        case ItemType.Media:
                            if (!string.IsNullOrEmpty((datItem as Media)?.MD5))
                                missingFields.Add(DatItemField.MD5);
                            break;
                        case ItemType.Rom:
                            if (!string.IsNullOrEmpty((datItem as Rom)?.MD5))
                                missingFields.Add(DatItemField.MD5);
                            break;
                        default:
                            missingFields.Add(DatItemField.MD5);
                            break;
                    }
                    break;
                case Hash.SHA1:
                    switch (datItem.ItemType)
                    {
                        case ItemType.Disk:
                            if (!string.IsNullOrEmpty((datItem as Disk)?.SHA1))
                                missingFields.Add(DatItemField.SHA1);
                            break;
                        case ItemType.Media:
                            if (!string.IsNullOrEmpty((datItem as Media)?.SHA1))
                                missingFields.Add(DatItemField.SHA1);
                            break;
                        case ItemType.Rom:
                            if (!string.IsNullOrEmpty((datItem as Rom)?.SHA1))
                                missingFields.Add(DatItemField.SHA1);
                            break;
                        default:
                            missingFields.Add(DatItemField.SHA1);
                            break;
                    }
                    break;
                case Hash.SHA256:
                    switch (datItem.ItemType)
                    {
                        case ItemType.Media:
                            if (!string.IsNullOrEmpty((datItem as Media)?.SHA256))
                                missingFields.Add(DatItemField.SHA256);
                            break;
                        case ItemType.Rom:
                            if (!string.IsNullOrEmpty((datItem as Rom)?.SHA256))
                                missingFields.Add(DatItemField.SHA256);
                            break;
                        default:
                            missingFields.Add(DatItemField.SHA256);
                            break;
                    }
                    break;
                case Hash.SHA384:
                    switch (datItem.ItemType)
                    {
                        case ItemType.Rom:
                            if (!string.IsNullOrEmpty((datItem as Rom)?.SHA384))
                                missingFields.Add(DatItemField.SHA384);
                            break;
                        default:
                            missingFields.Add(DatItemField.SHA384);
                            break;
                    }
                    break;
                case Hash.SHA512:
                    switch (datItem.ItemType)
                    {
                        case ItemType.Rom:
                            if (!string.IsNullOrEmpty((datItem as Rom)?.SHA512))
                                missingFields.Add(DatItemField.SHA512);
                            break;
                        default:
                            missingFields.Add(DatItemField.SHA512);
                            break;
                    }
                    break;
                case Hash.SpamSum:
                    switch (datItem.ItemType)
                    {
                        case ItemType.Media:
                            if (!string.IsNullOrEmpty((datItem as Media)?.SpamSum))
                                missingFields.Add(DatItemField.SpamSum);
                            break;
                        case ItemType.Rom:
                            if (!string.IsNullOrEmpty((datItem as Rom)?.SpamSum))
                                missingFields.Add(DatItemField.SpamSum);
                            break;
                        default:
                            missingFields.Add(DatItemField.SpamSum);
                            break;
                    }
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
                FileStream fs = File.Create(outfile);

                // If we get back null for some reason, just log and return
                if (fs == null)
                {
                    logger.Warning($"File '{outfile}' could not be created for writing! Please check to see if the file is writable");
                    return false;
                }

                SeparatedValueWriter svw = new SeparatedValueWriter(fs, new UTF8Encoding(false))
                {
                    Quotes = false,
                    Separator = ' ',
                    VerifyFieldCount = true
                };

                // Use a sorted list of games to output
                foreach (string key in Items.SortedKeys)
                {
                    ConcurrentList<DatItem> datItems = Items[key];

                    // If this machine doesn't contain any writable items, skip
                    if (!ContainsWritable(datItems))
                        continue;

                    // Resolve the names in the block
                    datItems = DatItem.ResolveNames(datItems);

                    for (int index = 0; index < datItems.Count; index++)
                    {
                        DatItem datItem = datItems[index];

                        // Check for a "null" item
                        datItem = ProcessNullifiedItem(datItem);

                        // Write out the item if we're not ignoring
                        if (!ShouldIgnore(datItem, ignoreblanks))
                            WriteDatItem(svw, datItem);
                    }
                }

                logger.User($"'{outfile}' written!{Environment.NewLine}");
                svw.Dispose();
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
        /// Write out DatItem using the supplied SeparatedValueWriter
        /// </summary>
        /// <param name="svw">SeparatedValueWriter to output to</param>
        /// <param name="datItem">DatItem object to be output</param>
        private void WriteDatItem(SeparatedValueWriter svw, DatItem datItem)
        {
            // Build the state
            string[] fields = new string[2];

            // Get the name field
            string name = string.Empty;
            switch (datItem.ItemType)
            {
                case ItemType.Disk:
                    var disk = datItem as Disk;
                    if (Header.GameName)
                        name = $"{disk.Machine.Name}{Path.DirectorySeparatorChar}";

                    name += disk.Name;
                    break;

                case ItemType.Media:
                    var media = datItem as Media;
                    if (Header.GameName)
                        name = $"{media.Machine.Name}{Path.DirectorySeparatorChar}";

                    name += media.Name;
                    break;

                case ItemType.Rom:
                    var rom = datItem as Rom;
                    if (Header.GameName)
                        name = $"{rom.Machine.Name}{Path.DirectorySeparatorChar}";

                    name += rom.Name;
                    break;
            }

            // Get the hash field and set final fields
            switch (_hash)
            {
                case Hash.CRC:
                    switch (datItem.ItemType)
                    {
                        case ItemType.Rom:
                            var rom = datItem as Rom;
                            fields[0] = name;
                            fields[1] = rom.CRC;
                            break;
                    }

                    break;

                case Hash.MD5:
                    switch (datItem.ItemType)
                    {
                        case ItemType.Disk:
                            var disk = datItem as Disk;
                            fields[0] = disk.MD5;
                            fields[1] = name;
                            break;

                        case ItemType.Media:
                            var media = datItem as Media;
                            fields[0] = media.MD5;
                            fields[1] = name;
                            break;

                        case ItemType.Rom:
                            var rom = datItem as Rom;
                            fields[0] = rom.MD5;
                            fields[1] = name;
                            break;
                    }

                    break;

                case Hash.SHA1:
                    switch (datItem.ItemType)
                    {
                        case ItemType.Disk:
                            var disk = datItem as Disk;
                            fields[0] = disk.SHA1;
                            fields[1] = name;
                            break;

                        case ItemType.Media:
                            var media = datItem as Media;
                            fields[0] = media.SHA1;
                            fields[1] = name;
                            break;

                        case ItemType.Rom:
                            var rom = datItem as Rom;
                            fields[0] = rom.SHA1;
                            fields[1] = name;
                            break;
                    }

                    break;

                case Hash.SHA256:
                    switch (datItem.ItemType)
                    {
                        case ItemType.Media:
                            var media = datItem as Media;
                            fields[0] = media.SHA256;
                            fields[1] = name;
                            break;

                        case ItemType.Rom:
                            var rom = datItem as Rom;
                            fields[0] = rom.SHA256;
                            fields[1] = name;
                            break;
                    }

                    break;

                case Hash.SHA384:
                    switch (datItem.ItemType)
                    {
                        case ItemType.Rom:
                            var rom = datItem as Rom;
                            fields[0] = rom.SHA384;
                            fields[1] = name;
                            break;
                    }

                    break;

                case Hash.SHA512:
                    switch (datItem.ItemType)
                    {
                        case ItemType.Rom:
                            var rom = datItem as Rom;
                            fields[0] = rom.SHA512;
                            fields[1] = name;
                            break;
                    }

                    break;

                case Hash.SpamSum:
                    switch (datItem.ItemType)
                    {
                        case ItemType.Media:
                            var media = datItem as Media;
                            fields[0] = media.SpamSum;
                            fields[1] = name;
                            break;

                        case ItemType.Rom:
                            var rom = datItem as Rom;
                            fields[0] = rom.SpamSum;
                            fields[1] = name;
                            break;
                    }

                    break;
            }

            // If we had at least one field filled in
            if (!string.IsNullOrEmpty(fields[0]) || !string.IsNullOrEmpty(fields[1]))
                svw.WriteValues(fields);

            svw.Flush();
        }
    }
}
