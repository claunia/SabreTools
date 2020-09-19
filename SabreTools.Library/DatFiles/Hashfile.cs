using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using SabreTools.Library.Data;
using SabreTools.Library.DatItems;
using SabreTools.Library.IO;

namespace SabreTools.Library.DatFiles
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

        /// <summary>
        /// Parse a hashfile or SFV and return all found games and roms within
        /// </summary>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        protected override void ParseFile(string filename, int indexId, bool keep, bool throwOnError = false)
        {
            // Open a file reader
            Encoding enc = FileExtensions.GetEncoding(filename);
            StreamReader sr = new StreamReader(FileExtensions.TryOpenRead(filename), enc);

            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();

                // Split the line and get the name and hash
                string[] split = line.Split(' ');
                string name = string.Empty;
                string hash = string.Empty;

                // If we have CRC, then it's an SFV file and the name is first are
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

                Rom rom = new Rom
                {
                    Name = name,
                    Size = null,
                    CRC = (_hash.HasFlag(Hash.CRC) ? hash : null),
                    MD5 = (_hash.HasFlag(Hash.MD5) ? hash : null),
#if NET_FRAMEWORK
                    RIPEMD160 = (_hash.HasFlag(Hash.RIPEMD160) ? hash : null),
#endif
                    SHA1 = (_hash.HasFlag(Hash.SHA1) ? hash : null),
                    SHA256 = (_hash.HasFlag(Hash.SHA256) ? hash : null),
                    SHA384 = (_hash.HasFlag(Hash.SHA384) ? hash : null),
                    SHA512 = (_hash.HasFlag(Hash.SHA512) ? hash : null),
                    SpamSum = (_hash.HasFlag(Hash.SpamSum) ? hash : null),
                    ItemStatus = ItemStatus.None,

                    Machine = new Machine
                    {
                        Name = Path.GetFileNameWithoutExtension(filename),
                    },

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

                // Now process and add the rom
                ParseAddHelper(rom);
            }

            sr.Dispose();
        }

        /// <inheritdoc/>
        protected override ItemType[] GetSupportedTypes()
        {
            return new ItemType[] { ItemType.Disk, ItemType.Media, ItemType.Rom };
        }

        /// <summary>
        /// Create and open an output file for writing direct from a dictionary
        /// </summary>
        /// <param name="outfile">Name of the file to write to</param>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise (default)</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        /// <returns>True if the DAT was written correctly, false otherwise</returns>
        public override bool WriteToFile(string outfile, bool ignoreblanks = false, bool throwOnError = false)
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

                SeparatedValueWriter svw = new SeparatedValueWriter(fs, new UTF8Encoding(false))
                {
                    Quotes = false,
                    Separator = ' ',
                    VerifyFieldCount = true
                };

                // Use a sorted list of games to output
                foreach (string key in Items.SortedKeys)
                {
                    List<DatItem> datItems = Items[key];

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

                Globals.Logger.Verbose($"File written!{Environment.NewLine}");
                svw.Dispose();
                fs.Dispose();
            }
            catch (Exception ex)
            {
                Globals.Logger.Error(ex);
                if (throwOnError) throw ex;
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
            string hash = string.Empty;
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

#if NET_FRAMEWORK
                case Hash.RIPEMD160:
                    switch (datItem.ItemType)
                    {
                        case ItemType.Rom:
                            var rom = datItem as Rom;
                            fields[0] = rom.RIPEMD160;
                            fields[1] = name;
                            break;
                    }

                    break;
#endif
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
