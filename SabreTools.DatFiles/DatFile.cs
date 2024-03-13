using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if NET40_OR_GREATER || NETCOREAPP
using System.Threading.Tasks;
#endif
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatFiles.Formats;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.Filter;
using SabreTools.Hashing;
using SabreTools.Logging;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.DatFiles
{
    /// <summary>
    /// Represents a format-agnostic DAT
    /// </summary>
    [JsonObject("datfile"), XmlRoot("datfile")]
    public abstract partial class DatFile
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
        public ItemDictionary Items { get; set; } = [];

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
        public DatFile(DatFile? datFile)
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
        public static DatFile Create(DatFormat? datFormat = null, DatFile? baseDat = null, bool quotes = true)
        {
            return datFormat switch
            {
                DatFormat.ArchiveDotOrg => new ArchiveDotOrg(baseDat),
                DatFormat.AttractMode => new AttractMode(baseDat),
                DatFormat.ClrMamePro => new ClrMamePro(baseDat, quotes),
                DatFormat.CSV => new CommaSeparatedValue(baseDat),
                DatFormat.DOSCenter => new DosCenter(baseDat),
                DatFormat.EverdriveSMDB => new EverdriveSMDB(baseDat),
                DatFormat.Listrom => new Listrom(baseDat),
                DatFormat.Listxml => new Listxml(baseDat),
                DatFormat.Logiqx => new Logiqx(baseDat, false),
                DatFormat.LogiqxDeprecated => new Logiqx(baseDat, true),
                DatFormat.MissFile => new Missfile(baseDat),
                DatFormat.OfflineList => new OfflineList(baseDat),
                DatFormat.OpenMSX => new OpenMSX(baseDat),
                DatFormat.RedumpMD5 => new Md5File(baseDat),
                DatFormat.RedumpSFV => new SfvFile(baseDat),
                DatFormat.RedumpSHA1 => new Sha1File(baseDat),
                DatFormat.RedumpSHA256 => new Sha256File(baseDat),
                DatFormat.RedumpSHA384 => new Sha384File(baseDat),
                DatFormat.RedumpSHA512 => new Sha512File(baseDat),
                DatFormat.RedumpSpamSum => new SpamSumFile(baseDat),
                DatFormat.RomCenter => new RomCenter(baseDat),
                DatFormat.SabreJSON => new SabreJSON(baseDat),
                DatFormat.SabreXML => new SabreXML(baseDat),
                DatFormat.SoftwareList => new Formats.SoftwareList(baseDat),
                DatFormat.SSV => new SemicolonSeparatedValue(baseDat),
                DatFormat.TSV => new TabSeparatedValue(baseDat),

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
            DatFile datFile = Create(datHeader.GetFieldValue<DatFormat>(DatHeader.DatFormatKey));
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
            if (string.IsNullOrEmpty(Header.GetStringFieldValue(Models.Metadata.Header.NameKey)) && !string.IsNullOrEmpty(Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey)))
            {
                Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey));
            }

            // If the name is defined but not the description, set the description from the name
            else if (!string.IsNullOrEmpty(Header.GetStringFieldValue(Models.Metadata.Header.NameKey)) && string.IsNullOrEmpty(Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey)))
            {
                Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, Header.GetStringFieldValue(Models.Metadata.Header.NameKey) + (bare ? string.Empty : $" ({Header.GetStringFieldValue(Models.Metadata.Header.DateKey)})"));
            }

            // If neither the name or description are defined, set them from the automatic values
            else if (string.IsNullOrEmpty(Header.GetStringFieldValue(Models.Metadata.Header.NameKey)) && string.IsNullOrEmpty(Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey)))
            {
                string[] splitpath = path.TrimEnd(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);
                Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, splitpath.Last());
                Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, Header.GetStringFieldValue(Models.Metadata.Header.NameKey) + (bare ? string.Empty : $" ({Header.GetStringFieldValue(Models.Metadata.Header.DateKey)})"));
            }
        }

        #endregion

        #region Filtering

        /// <summary>
        /// Execute all filters in a filter runner on the items in the dictionary
        /// </summary>
        /// <param name="filterRunner">Preconfigured filter runner to use</param>
        public void ExecuteFilters(FilterRunner filterRunner)
        {
            List<string> keys = [.. Items.Keys];
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(keys, Globals.ParallelOptions, key =>
#elif NET40_OR_GREATER
            Parallel.ForEach(keys, key =>
#else
            foreach (var key in keys)
#endif
            {
                ConcurrentList<DatItem>? items = Items[key];
                if (items == null)
#if NET40_OR_GREATER || NETCOREAPP
                    return;
#else
                    continue;
#endif

                // Filter all items in the current key
                var newItems = new ConcurrentList<DatItem>();
                foreach (var item in items)
                {
                    if (item.PassesFilter(filterRunner))
                        newItems.Add(item);
                }

                // Set the value in the key to the new set
                Items[key] = newItems;

#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif
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
            if (item is Disk disk)
            {
                // If the file has aboslutely no hashes, skip and log
                if (disk.GetStringFieldValue(Models.Metadata.Disk.StatusKey).AsEnumValue<ItemStatus>() != ItemStatus.Nodump
                    && string.IsNullOrEmpty(disk.GetStringFieldValue(Models.Metadata.Disk.MD5Key))
                    && string.IsNullOrEmpty(disk.GetStringFieldValue(Models.Metadata.Disk.SHA1Key)))
                {
                    logger.Verbose($"Incomplete entry for '{disk.GetName()}' will be output as nodump");
                    disk.SetFieldValue<string?>(Models.Metadata.Disk.StatusKey, ItemStatus.Nodump.AsStringValue());
                }

                item = disk;
            }
            if (item is Media media)
            {
                // If the file has aboslutely no hashes, skip and log
                if (string.IsNullOrEmpty(media.GetStringFieldValue(Models.Metadata.Media.MD5Key))
                    && string.IsNullOrEmpty(media.GetStringFieldValue(Models.Metadata.Media.SHA1Key))
                    && string.IsNullOrEmpty(media.GetStringFieldValue(Models.Metadata.Media.SHA256Key))
                    && string.IsNullOrEmpty(media.GetStringFieldValue(Models.Metadata.Media.SpamSumKey)))
                {
                    logger.Verbose($"Incomplete entry for '{media.GetName()}' will be output as nodump");
                }

                item = media;
            }
            else if (item is Rom rom)
            {
                long? size = rom.GetInt64FieldValue(Models.Metadata.Rom.SizeKey);

                // If we have the case where there is SHA-1 and nothing else, we don't fill in any other part of the data
                if (size == null && !rom.HasHashes())
                {
                    // No-op, just catch it so it doesn't go further
                    logger.Verbose($"{Header.GetStringFieldValue(DatHeader.FileNameKey)}: Entry with only SHA-1 found - '{rom.GetName()}'");
                }

                // If we have a rom and it's missing size AND the hashes match a 0-byte file, fill in the rest of the info
                else if ((size == 0 || size == null)
                    && (string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.CRCKey)) || rom.HasZeroHash()))
                {
                    rom.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, Constants.SizeZero.ToString());
                    rom.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, Constants.CRCZero);
                    rom.SetFieldValue<string?>(Models.Metadata.Rom.MD5Key, Constants.MD5Zero);
                    rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, Constants.SHA1Zero);
                    rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA256Key, null); // Constants.SHA256Zero;
                    rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA384Key, null); // Constants.SHA384Zero;
                    rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA512Key, null); // Constants.SHA512Zero;
                    rom.SetFieldValue<string?>(Models.Metadata.Rom.SpamSumKey, null); // Constants.SpamSumZero;
                }

                // If the file has no size and it's not the above case, skip and log
                else if (rom.GetStringFieldValue(Models.Metadata.Rom.StatusKey).AsEnumValue<ItemStatus>() != ItemStatus.Nodump && (size == 0 || size == null))
                {
                    logger.Verbose($"{Header.GetStringFieldValue(DatHeader.FileNameKey)}: Incomplete entry for '{rom.GetName()}' will be output as nodump");
                    rom.SetFieldValue<string?>(Models.Metadata.Rom.StatusKey, ItemStatus.Nodump.AsStringValue());
                }

                // If the file has a size but aboslutely no hashes, skip and log
                else if (rom.GetStringFieldValue(Models.Metadata.Rom.StatusKey).AsEnumValue<ItemStatus>() != ItemStatus.Nodump
                    && size != null && size > 0
                    && !rom.HasHashes())
                {
                    logger.Verbose($"{Header.GetStringFieldValue(DatHeader.FileNameKey)}: Incomplete entry for '{rom.GetName()}' will be output as nodump");
                    rom.SetFieldValue<string?>(Models.Metadata.Rom.StatusKey, ItemStatus.Nodump.AsStringValue());
                }

                item = rom;
            }

            // Get the key and add the file
            key = item.GetKey(ItemKey.Machine);

            // If only adding statistics, we add an empty key for games and then just item stats
            if (statsOnly)
            {
                Items.EnsureKey(key);
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
        protected static string? CleanDate(string? input)
        {
            // Null in, null out
            if (string.IsNullOrEmpty(input))
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
        protected static string CleanListromHashData(string hash)
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
                game = item.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey) ?? string.Empty,
                name = item.GetName() ?? item.GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>().AsStringValue() ?? string.Empty,
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
                fix = Header.GetStringFieldValue(DatHeader.PrefixKey) + (Header.GetBoolFieldValue(DatHeader.QuotesKey) == true ? "\"" : string.Empty);

            // If we have a postfix
            else
                fix = (Header.GetBoolFieldValue(DatHeader.QuotesKey) == true ? "\"" : string.Empty) + Header.GetStringFieldValue(DatHeader.PostfixKey);

            // Ensure we have the proper values for replacement
            if (item is Disk disk)
            {
                md5 = disk.GetStringFieldValue(Models.Metadata.Disk.MD5Key) ?? string.Empty;
                sha1 = disk.GetStringFieldValue(Models.Metadata.Disk.SHA1Key) ?? string.Empty;
            }
            else if (item is Media media)
            {
                md5 = media.GetStringFieldValue(Models.Metadata.Media.MD5Key) ?? string.Empty;
                sha1 = media.GetStringFieldValue(Models.Metadata.Media.SHA1Key) ?? string.Empty;
                sha256 = media.GetStringFieldValue(Models.Metadata.Media.SHA256Key) ?? string.Empty;
                spamsum = media.GetStringFieldValue(Models.Metadata.Media.SpamSumKey) ?? string.Empty;
            }
            else if (item is Rom rom)
            {
                crc = rom.GetStringFieldValue(Models.Metadata.Rom.CRCKey) ?? string.Empty;
                md5 = rom.GetStringFieldValue(Models.Metadata.Rom.MD5Key) ?? string.Empty;
                sha1 = rom.GetStringFieldValue(Models.Metadata.Rom.SHA1Key) ?? string.Empty;
                sha256 = rom.GetStringFieldValue(Models.Metadata.Rom.SHA256Key) ?? string.Empty;
                sha384 = rom.GetStringFieldValue(Models.Metadata.Rom.SHA384Key) ?? string.Empty;
                sha512 = rom.GetStringFieldValue(Models.Metadata.Rom.SHA512Key) ?? string.Empty;
                size = rom.GetInt64FieldValue(Models.Metadata.Rom.SizeKey).ToString() ?? string.Empty;
                spamsum = rom.GetStringFieldValue(Models.Metadata.Rom.SpamSumKey) ?? string.Empty;
            }

            // Now do bulk replacement where possible
            var machine = item.GetFieldValue<Machine>(DatItem.MachineKey);
            fix = fix
                .Replace("%game%", game)
                .Replace("%machine%", game)
                .Replace("%name%", name)
                .Replace("%manufacturer%", machine!.GetStringFieldValue(Models.Metadata.Machine.ManufacturerKey) ?? string.Empty)
                .Replace("%publisher%", machine!.GetStringFieldValue(Models.Metadata.Machine.PublisherKey) ?? string.Empty)
                .Replace("%category%", machine!.GetStringFieldValue(Models.Metadata.Machine.CategoryKey) ?? string.Empty)
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
            // Backup relevant values and set new ones accordingly
            bool? quotesBackup = Header.GetBoolFieldValue(DatHeader.QuotesKey);
            bool? useRomNameBackup = Header.GetBoolFieldValue(DatHeader.UseRomNameKey);
            if (forceRemoveQuotes)
                Header.SetFieldValue<bool>(DatHeader.QuotesKey, false);
            if (forceRomName)
                Header.SetFieldValue<bool>(DatHeader.UseRomNameKey, true);

            // Get the name to update
            string? name = (Header.GetBoolFieldValue(DatHeader.UseRomNameKey) == true
                ? item.GetName()
                : item.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey)) ?? string.Empty;

            // Create the proper Prefix and Postfix
            string pre = CreatePrefixPostfix(item, true);
            string post = CreatePrefixPostfix(item, false);

            // If we're in Depot mode, take care of that instead
            if (Header.GetFieldValue<DepotInformation?>(DatHeader.OutputDepotKey)?.IsActive == true)
            {
                if (item is Disk disk)
                {
                    // We can only write out if there's a SHA-1
                    if (!string.IsNullOrEmpty(disk.GetStringFieldValue(Models.Metadata.Disk.SHA1Key)))
                    {
                        name = Utilities.GetDepotPath(disk.GetStringFieldValue(Models.Metadata.Disk.SHA1Key), Header.GetFieldValue<DepotInformation?>(DatHeader.OutputDepotKey)!.Depth)?.Replace('\\', '/');
                        item.SetName($"{pre}{name}{post}");
                    }
                }
                else if (item is Media media)
                {
                    // We can only write out if there's a SHA-1
                    if (!string.IsNullOrEmpty(media.GetStringFieldValue(Models.Metadata.Media.SHA1Key)))
                    {
                        name = Utilities.GetDepotPath(media.GetStringFieldValue(Models.Metadata.Media.SHA1Key), Header.GetFieldValue<DepotInformation?>(DatHeader.OutputDepotKey)!.Depth)?.Replace('\\', '/');
                        item.SetName($"{pre}{name}{post}");
                    }
                }
                else if (item is Rom rom)
                {
                    // We can only write out if there's a SHA-1
                    if (!string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA1Key)))
                    {
                        name = Utilities.GetDepotPath(rom.GetStringFieldValue(Models.Metadata.Rom.SHA1Key), Header.GetFieldValue<DepotInformation?>(DatHeader.OutputDepotKey)!.Depth)?.Replace('\\', '/');
                        item.SetName($"{pre}{name}{post}");
                    }
                }

                return;
            }

            if (!string.IsNullOrEmpty(Header.GetStringFieldValue(DatHeader.ReplaceExtensionKey)) || Header.GetBoolFieldValue(DatHeader.RemoveExtensionKey) == true)
            {
                if (Header.GetBoolFieldValue(DatHeader.RemoveExtensionKey) == true)
                    Header.SetFieldValue<string?>(DatHeader.ReplaceExtensionKey, string.Empty);

                string? dir = Path.GetDirectoryName(name);
                if (dir != null)
                {
                    dir = dir.TrimStart(Path.DirectorySeparatorChar);
                    name = Path.Combine(dir, Path.GetFileNameWithoutExtension(name) + Header.GetStringFieldValue(DatHeader.ReplaceExtensionKey));
                }
            }

            if (!string.IsNullOrEmpty(Header.GetStringFieldValue(DatHeader.AddExtensionKey)))
                name += Header.GetStringFieldValue(DatHeader.AddExtensionKey);

            if (Header.GetBoolFieldValue(DatHeader.UseRomNameKey) == true && Header.GetBoolFieldValue(DatHeader.GameNameKey) == true)
                name = Path.Combine(item.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey) ?? string.Empty, name);

            // Now assign back the formatted name
            name = $"{pre}{name}{post}";
            if (Header.GetBoolFieldValue(DatHeader.UseRomNameKey) == true)
                item.SetName(name);
            else if (item.GetFieldValue<Machine>(DatItem.MachineKey) != null)
                item.GetFieldValue<Machine>(DatItem.MachineKey)!.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, name);

            // Restore all relevant values
            if (forceRemoveQuotes)
                Header.SetFieldValue<bool?>(DatHeader.QuotesKey, quotesBackup);

            if (forceRomName)
                Header.SetFieldValue<bool?>(DatHeader.UseRomNameKey, useRomNameBackup);
        }

        /// <summary>
        /// Process any DatItems that are "null", usually created from directory population
        /// </summary>
        /// <param name="datItem">DatItem to check for "null" status</param>
        /// <returns>Cleaned DatItem</returns>
        protected DatItem ProcessNullifiedItem(DatItem datItem)
        {
            // If we don't have a Rom, we can ignore it
            if (datItem is not Rom rom)
                return datItem;

            // If the Rom has "null" characteristics, ensure all fields
            if (rom.GetInt64FieldValue(Models.Metadata.Rom.SizeKey) == null && rom.GetStringFieldValue(Models.Metadata.Rom.CRCKey) == "null")
            {
                logger.Verbose($"Empty folder found: {datItem.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey)}");

                rom.SetName(rom.GetName() == "null" ? "-" : rom.GetName());
                rom.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, Constants.SizeZero.ToString());
                rom.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, rom.GetStringFieldValue(Models.Metadata.Rom.CRCKey) == "null" ? Constants.CRCZero : null);
                rom.SetFieldValue<string?>(Models.Metadata.Rom.MD5Key, rom.GetStringFieldValue(Models.Metadata.Rom.MD5Key) == "null" ? Constants.MD5Zero : null);
                rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, rom.GetStringFieldValue(Models.Metadata.Rom.SHA1Key) == "null" ? Constants.SHA1Zero : null);
                rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA256Key, rom.GetStringFieldValue(Models.Metadata.Rom.SHA256Key) == "null" ? Constants.SHA256Zero : null);
                rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA384Key, rom.GetStringFieldValue(Models.Metadata.Rom.SHA384Key) == "null" ? Constants.SHA384Zero : null);
                rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA512Key, rom.GetStringFieldValue(Models.Metadata.Rom.SHA512Key) == "null" ? Constants.SHA512Zero : null);
                rom.SetFieldValue<string?>(Models.Metadata.Rom.SpamSumKey, rom.GetStringFieldValue(Models.Metadata.Rom.SpamSumKey) == "null" ? Constants.SpamSumZero : null);
            }

            return rom;
        }

        /// <summary>
        /// Get supported types for write
        /// </summary>
        /// <returns>List of supported types for writing</returns>
        protected virtual ItemType[] GetSupportedTypes()
        {
            return Enum.GetValues(typeof(ItemType)) as ItemType[] ?? [];
        }

        /// <summary>
        /// Return list of required fields missing from a DatItem
        /// </summary>
        /// <returns>List of missing required fields, null or empty if none were found</returns>
        protected virtual List<string>? GetMissingRequiredFields(DatItem datItem) => null;

        /// <summary>
        /// Get if a machine contains any writable items
        /// </summary>
        /// <param name="datItems">DatItems to check</param>
        /// <returns>True if the machine contains at least one writable item, false otherwise</returns>
        /// <remarks>Empty machines are kept with this</remarks>
        protected bool ContainsWritable(ConcurrentList<DatItem> datItems)
        {
            // Empty machines are considered writable
            if (datItems == null || datItems.Count == 0)
                return true;

            foreach (DatItem datItem in datItems)
            {
                if (GetSupportedTypes().Contains(datItem.GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>()))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Get if an item should be ignored on write
        /// </summary>
        /// <param name="datItem">DatItem to check</param>
        /// <param name="ignoreBlanks">True if blank roms should be skipped on output, false otherwise</param>
        /// <returns>True if the item should be skipped on write, false otherwise</returns>
        protected bool ShouldIgnore(DatItem? datItem, bool ignoreBlanks)
        {
            // If this is invoked with a null DatItem, we ignore
            if (datItem == null)
            {
                logger?.Verbose($"Item was skipped because it was null");
                return true;
            }

            // If the item is supposed to be removed, we ignore
            if (datItem.GetBoolFieldValue(DatItem.RemoveKey) == true)
            {
                string itemString = JsonConvert.SerializeObject(datItem, Formatting.None);
                logger?.Verbose($"Item '{itemString}' was skipped because it was marked for removal");
                return true;
            }

            // If we have the Blank dat item, we ignore
            if (datItem is Blank)
            {
                string itemString = JsonConvert.SerializeObject(datItem, Formatting.None);
                logger?.Verbose($"Item '{itemString}' was skipped because it was of type 'Blank'");
                return true;
            }

            // If we're ignoring blanks and we have a Rom
            if (ignoreBlanks && datItem is Rom rom)
            {
                // If we have a 0-size or blank rom, then we ignore
                long? size = rom.GetInt64FieldValue(Models.Metadata.Rom.SizeKey);
                if (size == 0 || size == null)
                {
                    string itemString = JsonConvert.SerializeObject(datItem, Formatting.None);
                    logger?.Verbose($"Item '{itemString}' was skipped because it had an invalid size");
                    return true;
                }
            }

            // If we have an item type not in the list of supported values
            if (!GetSupportedTypes().Contains(datItem.GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>()))
            {
                string itemString = JsonConvert.SerializeObject(datItem, Formatting.None);
                logger?.Verbose($"Item '{itemString}' was skipped because it was not supported in {Header?.GetFieldValue<DatFormat>(DatHeader.DatFormatKey)}");
                return true;
            }

            // If we have an item with missing required fields
            List<string>? missingFields = GetMissingRequiredFields(datItem);
            if (missingFields != null && missingFields.Count != 0)
            {
                string itemString = JsonConvert.SerializeObject(datItem, Formatting.None);
#if NET20 || NET35
                logger?.Verbose($"Item '{itemString}' was skipped because it was missing required fields for {Header?.GetFieldValue<DatFormat>(DatHeader.DatFormatKey)}: {string.Join(", ", [.. missingFields])}");
#else
                logger?.Verbose($"Item '{itemString}' was skipped because it was missing required fields for {Header?.GetFieldValue<DatFormat>(DatHeader.DatFormatKey)}: {string.Join(", ", missingFields)}");
#endif
                return true;
            }

            return false;
        }

        #endregion
    }

    /// <summary>
    /// Represents a DAT that can be serialized
    /// </summary>
    /// <typeparam name="T">Base internal model for the DAT type</typeparam>
    /// <typeparam name="U">IFileSerializer type to use for conversion</typeparam>
    /// <typeparam name="V">IModelSerializer for cross-model serialization</typeparam>
    public abstract class SerializableDatFile<T, U, V> : DatFile
        where U : IFileSerializer<T>
        where V : IModelSerializer<T, Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        protected SerializableDatFile(DatFile? datFile) : base(datFile) { }

        /// <inheritdoc/>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            try
            {
                // Deserialize the input file in two steps
                var specificFormat = Activator.CreateInstance<U>().Deserialize(filename);
                var internalFormat = Activator.CreateInstance<V>().Serialize(specificFormat);

                // Convert to the internal format
                ConvertMetadata(internalFormat, filename, indexId, keep, statsOnly);
            }
            catch (Exception ex) when (!throwOnError)
            {
                string message = $"'{filename}' - An error occurred during parsing";
                logger.Error(ex, message);
            }
        }

        /// <inheritdoc/>
        public override bool WriteToFile(string outfile, bool ignoreblanks = false, bool throwOnError = false)
        {
            try
            {
                logger.User($"Writing to '{outfile}'...");

                // Serialize the input file in two steps
                var internalFormat = ConvertMetadata(ignoreblanks);
                var specificFormat = Activator.CreateInstance<V>().Deserialize(internalFormat);
                if (!Activator.CreateInstance<U>().Serialize(specificFormat, outfile))
                {
                    logger.Warning($"File '{outfile}' could not be written! See the log for more details.");
                    return false;
                }
            }
            catch (Exception ex) when (!throwOnError)
            {
                logger.Error(ex);
                return false;
            }

            logger.User($"'{outfile}' written!{Environment.NewLine}");
            return true;
        }
    }
}
