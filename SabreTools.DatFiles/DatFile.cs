using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatFiles.Formats;
using SabreTools.DatItems;
using SabreTools.Logging;
using Newtonsoft.Json;

namespace SabreTools.DatFiles
{
    /// <summary>
    /// Represents a format-agnostic DAT
    /// </summary>
    [JsonObject("datfile"), XmlRoot("datfile")]
    public abstract class DatFile
    {
        #region Fields

        /// <summary>
        /// Header values
        /// </summary>
        [JsonProperty("header"), XmlElement("header")]
        public DatHeader Header { get; set; } = new DatHeader();

        /// <summary>
        /// DatItems and related statistics
        /// </summary>
        [JsonProperty("items"), XmlElement("items")]
        public ItemDictionary Items { get; set; } = new ItemDictionary();

        #endregion

        #region Logging

        /// <summary>
        /// Logging object
        /// </summary>
        [JsonIgnore, XmlIgnore]
        protected Logger logger;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new DatFile from an existing one
        /// </summary>
        /// <param name="datFile">DatFile to get the values from</param>
        public DatFile(DatFile datFile)
        {
            logger = new Logger(this);
            if (datFile != null)
            {
                Header = datFile.Header;
                Items = datFile.Items;
            }
        }

        /// <summary>
        /// Create a specific type of DatFile to be used based on a format and a base DAT
        /// </summary>
        /// <param name="datFormat">Format of the DAT to be created</param>
        /// <param name="baseDat">DatFile containing the information to use in specific operations</param>
        /// <param name="quotes">For relevant types, assume the usage of quotes</param>
        /// <returns>DatFile of the specific internal type that corresponds to the inputs</returns>
        public static DatFile Create(DatFormat? datFormat = null, DatFile baseDat = null, bool quotes = true)
        {
            return datFormat switch
            {
                DatFormat.AttractMode => new AttractMode(baseDat),
                DatFormat.ClrMamePro => new ClrMamePro(baseDat, quotes),
                DatFormat.CSV => new SeparatedValue(baseDat, ','),
                DatFormat.DOSCenter => new DosCenter(baseDat),
                DatFormat.EverdriveSMDB => new EverdriveSMDB(baseDat),
                DatFormat.Listrom => new Listrom(baseDat),
                DatFormat.Listxml => new Listxml(baseDat),
                DatFormat.Logiqx => new Logiqx(baseDat, false),
                DatFormat.LogiqxDeprecated => new Logiqx(baseDat, true),
                DatFormat.MissFile => new Missfile(baseDat),
                DatFormat.OfflineList => new OfflineList(baseDat),
                DatFormat.OpenMSX => new OpenMSX(baseDat),
                DatFormat.RedumpMD5 => new Hashfile(baseDat, Hash.MD5),
                DatFormat.RedumpSFV => new Hashfile(baseDat, Hash.CRC),
                DatFormat.RedumpSHA1 => new Hashfile(baseDat, Hash.SHA1),
                DatFormat.RedumpSHA256 => new Hashfile(baseDat, Hash.SHA256),
                DatFormat.RedumpSHA384 => new Hashfile(baseDat, Hash.SHA384),
                DatFormat.RedumpSHA512 => new Hashfile(baseDat, Hash.SHA512),
                DatFormat.RedumpSpamSum => new Hashfile(baseDat, Hash.SpamSum),
                DatFormat.RomCenter => new RomCenter(baseDat),
                DatFormat.SabreJSON => new SabreJSON(baseDat),
                DatFormat.SabreXML => new SabreXML(baseDat),
                DatFormat.SoftwareList => new Formats.SoftwareList(baseDat),
                DatFormat.SSV => new SeparatedValue(baseDat, ';'),
                DatFormat.TSV => new SeparatedValue(baseDat, '\t'),
                
                // We use new-style Logiqx as a backup for generic DatFile
                _ => new Logiqx(baseDat, false),
            };
        }

        /// <summary>
        /// Create a new DatFile from an existing DatHeader
        /// </summary>
        /// <param name="datHeader">DatHeader to get the values from</param>
        public static DatFile Create(DatHeader datHeader)
        {
            DatFile datFile = Create(datHeader.DatFormat);
            datFile.Header = (DatHeader)datHeader.Clone();
            return datFile;
        }

        /// <summary>
        /// Fill the header values based on existing Header and path
        /// </summary>
        /// <param name="path">Path used for creating a name, if necessary</param>
        /// <param name="bare">True if the date should be omitted from name and description, false otherwise</param>
        public void FillHeaderFromPath(string path, bool bare)
        {
            // If the description is defined but not the name, set the name from the description
            if (string.IsNullOrWhiteSpace(Header.Name) && !string.IsNullOrWhiteSpace(Header.Description))
            {
                Header.Name = Header.Description;
            }

            // If the name is defined but not the description, set the description from the name
            else if (!string.IsNullOrWhiteSpace(Header.Name) && string.IsNullOrWhiteSpace(Header.Description))
            {
                Header.Description = Header.Name + (bare ? string.Empty : $" ({Header.Date})");
            }

            // If neither the name or description are defined, set them from the automatic values
            else if (string.IsNullOrWhiteSpace(Header.Name) && string.IsNullOrWhiteSpace(Header.Description))
            {
                string[] splitpath = path.TrimEnd(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);
                Header.Name = splitpath.Last();
                Header.Description = Header.Name + (bare ? string.Empty : $" ({Header.Date})");
            }
        }

        #endregion
    
        #region Parsing

        /// <summary>
        /// Parse DatFile and return all found games and roms within
        /// </summary>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        public abstract void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false);

        /// <summary>
        /// Add a rom to the Dat after checking
        /// </summary>
        /// <param name="item">Item data to check against</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <returns>The key for the item</returns>
        protected string ParseAddHelper(DatItem item, bool statsOnly)
        {
            string key;

            // If we have a Disk, Media, or Rom, clean the hash data
            if (item.ItemType == ItemType.Disk)
            {
                Disk disk = item as Disk;

                // If the file has aboslutely no hashes, skip and log
                if (disk.ItemStatus != ItemStatus.Nodump
                    && string.IsNullOrWhiteSpace(disk.MD5)
                    && string.IsNullOrWhiteSpace(disk.SHA1))
                {
                    logger.Verbose($"Incomplete entry for '{disk.Name}' will be output as nodump");
                    disk.ItemStatus = ItemStatus.Nodump;
                }

                item = disk;
            }
            else if (item.ItemType == ItemType.Rom)
            {
                Rom rom = item as Rom;

                // If we have the case where there is SHA-1 and nothing else, we don't fill in any other part of the data
                if (rom.Size == null && !rom.HasHashes())
                {
                    // No-op, just catch it so it doesn't go further
                    logger.Verbose($"{Header.FileName}: Entry with only SHA-1 found - '{rom.Name}'");
                }

                // If we have a rom and it's missing size AND the hashes match a 0-byte file, fill in the rest of the info
                else if ((rom.Size == 0 || rom.Size == null)
                    && (string.IsNullOrWhiteSpace(rom.CRC) || rom.HasZeroHash()))
                {
                    // TODO: All instances of Hash.DeepHashes should be made into 0x0 eventually
                    rom.Size = Constants.SizeZero;
                    rom.CRC = Constants.CRCZero;
                    rom.MD5 = Constants.MD5Zero;
                    rom.SHA1 = Constants.SHA1Zero;
                    rom.SHA256 = null; // Constants.SHA256Zero;
                    rom.SHA384 = null; // Constants.SHA384Zero;
                    rom.SHA512 = null; // Constants.SHA512Zero;
                    rom.SpamSum = null; // Constants.SpamSumZero;
                }

                // If the file has no size and it's not the above case, skip and log
                else if (rom.ItemStatus != ItemStatus.Nodump && (rom.Size == 0 || rom.Size == null))
                {
                    logger.Verbose($"{Header.FileName}: Incomplete entry for '{rom.Name}' will be output as nodump");
                    rom.ItemStatus = ItemStatus.Nodump;
                }

                // If the file has a size but aboslutely no hashes, skip and log
                else if (rom.ItemStatus != ItemStatus.Nodump
                    && rom.Size != null && rom.Size > 0
                    && !rom.HasHashes())
                {
                    logger.Verbose($"{Header.FileName}: Incomplete entry for '{rom.Name}' will be output as nodump");
                    rom.ItemStatus = ItemStatus.Nodump;
                }

                item = rom;
            }

            // Get the key and add the file
            key = item.GetKey(ItemKey.Machine);

            // If only adding statistics, we add an empty key for games and then just item stats
            if (statsOnly)
            {
                if (Items.ContainsKey(key))
                    Items.Add(key, new List<DatItem>());

                Items.AddItemStatistics(item);
            }
            else
            {
                Items.Add(key, item);
            }

            return key;
        }

        /// <summary>
        /// Get a sanitized Date from an input string
        /// </summary>
        /// <param name="input">String to get value from</param>
        /// <returns>Date as a string, if possible</returns>
        protected string CleanDate(string input)
        {
            // Null in, null out
            if (input == null)
                return null;

            string date = string.Empty;
            if (input != null)
            {
                if (DateTime.TryParse(input, out DateTime dateTime))
                    date = dateTime.ToString();
                else
                    date = input;
            }

            return date;
        }
    
        /// <summary>
        /// Clean a hash string from a Listrom DAT
        /// </summary>
        /// <param name="hash">Hash string to sanitize</param>
        /// <returns>Cleaned string</returns>
        protected string CleanListromHashData(string hash)
        {
            if (hash.StartsWith("CRC"))
                return hash.Substring(4, 8).ToLowerInvariant();

            else if (hash.StartsWith("SHA1"))
                return hash.Substring(5, 40).ToLowerInvariant();

            return hash;
        }

        #endregion
    
        #region Writing

        /// <summary>
        /// Create and open an output file for writing direct from a dictionary
        /// </summary>
        /// <param name="outfile">Name of the file to write to</param>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise (default)</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        /// <returns>True if the DAT was written correctly, false otherwise</returns>
        public abstract bool WriteToFile(string outfile, bool ignoreblanks = false, bool throwOnError = false);

        /// <summary>
        /// Create a prefix or postfix from inputs
        /// </summary>
        /// <param name="item">DatItem to create a prefix/postfix for</param>
        /// <param name="prefix">True for prefix, false for postfix</param>
        /// <returns>Sanitized string representing the postfix or prefix</returns>
        protected string CreatePrefixPostfix(DatItem item, bool prefix)
        {
            // Initialize strings
            string fix,
                game = item.Machine.Name,
                name = item.GetName() ?? item.ItemType.ToString(),
                crc = string.Empty,
                md5 = string.Empty,
                sha1 = string.Empty,
                sha256 = string.Empty,
                sha384 = string.Empty,
                sha512 = string.Empty,
                size = string.Empty,
                spamsum = string.Empty;

            // If we have a prefix
            if (prefix)
                fix = Header.Prefix + (Header.Quotes ? "\"" : string.Empty);

            // If we have a postfix
            else
                fix = (Header.Quotes ? "\"" : string.Empty) + Header.Postfix;

            // Ensure we have the proper values for replacement
            if (item.ItemType == ItemType.Disk)
            {
                md5 = (item as Disk).MD5 ?? string.Empty;
                sha1 = (item as Disk).SHA1 ?? string.Empty;
            }
            else if (item.ItemType == ItemType.Media)
            {
                md5 = (item as Media).MD5 ?? string.Empty;
                sha1 = (item as Media).SHA1 ?? string.Empty;
                sha256 = (item as Media).SHA256 ?? string.Empty;
                spamsum = (item as Media).SpamSum ?? string.Empty;
            }
            else if (item.ItemType == ItemType.Rom)
            {
                crc = (item as Rom).CRC ?? string.Empty;
                md5 = (item as Rom).MD5 ?? string.Empty;
                sha1 = (item as Rom).SHA1 ?? string.Empty;
                sha256 = (item as Rom).SHA256 ?? string.Empty;
                sha384 = (item as Rom).SHA384 ?? string.Empty;
                sha512 = (item as Rom).SHA512 ?? string.Empty;
                size = (item as Rom).Size?.ToString() ?? string.Empty;
                spamsum = (item as Rom).SpamSum ?? string.Empty;
            }

            // Now do bulk replacement where possible
            fix = fix
                .Replace("%game%", game)
                .Replace("%machine%", game)
                .Replace("%name%", name)
                .Replace("%manufacturer%", item.Machine.Manufacturer ?? string.Empty)
                .Replace("%publisher%", item.Machine.Publisher ?? string.Empty)
                .Replace("%category%", item.Machine.Category ?? string.Empty)
                .Replace("%crc%", crc)
                .Replace("%md5%", md5)
                .Replace("%sha1%", sha1)
                .Replace("%sha256%", sha256)
                .Replace("%sha384%", sha384)
                .Replace("%sha512%", sha512)
                .Replace("%size%", size)
                .Replace("%spamsum%", spamsum);

            return fix;
        }

        /// <summary>
        /// Process an item and correctly set the item name
        /// </summary>
        /// <param name="item">DatItem to update</param>
        /// <param name="forceRemoveQuotes">True if the Quotes flag should be ignored, false otherwise</param>
        /// <param name="forceRomName">True if the UseRomName should be always on (default), false otherwise</param>
        protected void ProcessItemName(DatItem item, bool forceRemoveQuotes, bool forceRomName = true)
        {
            string name = item.GetName() ?? string.Empty;

            // Backup relevant values and set new ones accordingly
            bool quotesBackup = Header.Quotes;
            bool useRomNameBackup = Header.UseRomName;
            if (forceRemoveQuotes)
                Header.Quotes = false;

            if (forceRomName)
                Header.UseRomName = true;

            // Create the proper Prefix and Postfix
            string pre = CreatePrefixPostfix(item, true);
            string post = CreatePrefixPostfix(item, false);

            // If we're in Depot mode, take care of that instead
            if (Header.OutputDepot?.IsActive == true)
            {
                if (item.ItemType == ItemType.Disk)
                {
                    Disk disk = item as Disk;

                    // We can only write out if there's a SHA-1
                    if (!string.IsNullOrWhiteSpace(disk.SHA1))
                    {
                        name = Utilities.GetDepotPath(disk.SHA1, Header.OutputDepot.Depth).Replace('\\', '/');
                        item.SetName($"{pre}{name}{post}");
                    }
                }
                else if (item.ItemType == ItemType.Media)
                {
                    Media media = item as Media;

                    // We can only write out if there's a SHA-1
                    if (!string.IsNullOrWhiteSpace(media.SHA1))
                    {
                        name = Utilities.GetDepotPath(media.SHA1, Header.OutputDepot.Depth).Replace('\\', '/');
                        item.SetName($"{pre}{name}{post}");
                    }
                }
                else if (item.ItemType == ItemType.Rom)
                {
                    Rom rom = item as Rom;

                    // We can only write out if there's a SHA-1
                    if (!string.IsNullOrWhiteSpace(rom.SHA1))
                    {
                        name = Utilities.GetDepotPath(rom.SHA1, Header.OutputDepot.Depth).Replace('\\', '/');
                        item.SetName($"{pre}{name}{post}");
                    }
                }

                return;
            }

            if (!string.IsNullOrWhiteSpace(Header.ReplaceExtension) || Header.RemoveExtension)
            {
                if (Header.RemoveExtension)
                    Header.ReplaceExtension = string.Empty;

                string dir = Path.GetDirectoryName(name);
                dir = dir.TrimStart(Path.DirectorySeparatorChar);
                name = Path.Combine(dir, Path.GetFileNameWithoutExtension(name) + Header.ReplaceExtension);
            }

            if (!string.IsNullOrWhiteSpace(Header.AddExtension))
                name += Header.AddExtension;

            if (Header.UseRomName && Header.GameName)
                name = Path.Combine(item.Machine.Name, name);

            // Now assign back the item name
            item.SetName($"{pre}{name}{post}");

            // Restore all relevant values
            if (forceRemoveQuotes)
                Header.Quotes = quotesBackup;

            if (forceRomName)
                Header.UseRomName = useRomNameBackup;
        }

        /// <summary>
        /// Process any DatItems that are "null", usually created from directory population
        /// </summary>
        /// <param name="datItem">DatItem to check for "null" status</param>
        /// <returns>Cleaned DatItem</returns>
        protected DatItem ProcessNullifiedItem(DatItem datItem)
        {
            // If we don't have a Rom, we can ignore it
            if (datItem.ItemType != ItemType.Rom)
                return datItem;

            // Cast for easier parsing
            Rom rom = datItem as Rom;

            // If the Rom has "null" characteristics, ensure all fields
            if (rom.Size == null && rom.CRC == "null")
            {
                logger.Verbose($"Empty folder found: {datItem.Machine.Name}");

                rom.Name = (rom.Name == "null" ? "-" : rom.Name);
                rom.Size = Constants.SizeZero;
                rom.CRC = rom.CRC == "null" ? Constants.CRCZero : null;
                rom.MD5 = rom.MD5 == "null" ? Constants.MD5Zero : null;
                rom.SHA1 = rom.SHA1 == "null" ? Constants.SHA1Zero : null;
                rom.SHA256 = rom.SHA256 == "null" ? Constants.SHA256Zero : null;
                rom.SHA384 = rom.SHA384 == "null" ? Constants.SHA384Zero : null;
                rom.SHA512 = rom.SHA512 == "null" ? Constants.SHA512Zero : null;
                rom.SpamSum = rom.SpamSum == "null" ? Constants.SpamSumZero : null;
            }

            return rom;
        }

        /// <summary>
        /// Get supported types for write
        /// </summary>
        /// <returns>List of supported types for writing</returns>
        protected virtual ItemType[] GetSupportedTypes()
        {
            return Enum.GetValues(typeof(ItemType)) as ItemType[];
        }

        /// <summary>
        /// Get if a machine contains any writable items
        /// </summary>
        /// <param name="datItems">DatItems to check</param>
        /// <returns>True if the machine contains at least one writable item, false otherwise</returns>
        /// <remarks>Empty machines are kept with this</remarks>
        protected bool ContainsWritable(List<DatItem> datItems)
        {
            // Empty machines are considered writable
            if (datItems == null || datItems.Count == 0)
                return true;

            foreach (DatItem datItem in datItems)
            {
                if (GetSupportedTypes().Contains(datItem.ItemType))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Get if an item should be ignored on write
        /// </summary>
        /// <param name="datItem">DatItem to check</param>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise</param>
        /// <returns>True if the item should be skipped on write, false otherwise</returns>
        protected bool ShouldIgnore(DatItem datItem, bool ignoreBlanks)
        {
            // If the item is supposed to be removed, we ignore
            if (datItem.Remove)
                return true;

            // If we have the Blank dat item, we ignore
            if (datItem.ItemType == ItemType.Blank)
                return true;

            // If we're ignoring blanks and we have a Rom
            if (ignoreBlanks && datItem.ItemType == ItemType.Rom)
            {
                Rom rom = datItem as Rom;

                // If we have a 0-size or blank rom, then we ignore
                if (rom.Size == 0 || rom.Size == null)
                    return true;
            }

            // If we have an item type not in the list of supported values
            if (!GetSupportedTypes().Contains(datItem.ItemType))
                return true;

            return false;
        }

        #endregion
    }
}
