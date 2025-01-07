using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core.Filter;
using SabreTools.Core.Tools;
using SabreTools.DatFiles.Formats;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.Hashing;
using SabreTools.IO.Logging;

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
        public DatHeader Header { get; private set; } = new DatHeader();

        /// <summary>
        /// DatItems and related statistics
        /// </summary>
        [JsonProperty("items"), XmlElement("items")]
        public ItemDictionary Items { get; private set; } = [];

        /// <summary>
        /// DatItems and related statistics
        /// </summary>
        [JsonProperty("items"), XmlElement("items")]
        public ItemDictionaryDB ItemsDB { get; private set; } = new ItemDictionaryDB();

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
                ItemsDB = datFile.ItemsDB;
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
            DatFormat format = datHeader.GetFieldValue<DatFormat>(DatHeader.DatFormatKey);
            DatFile datFile = Create(format);
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
            // Get the header strings
            string? name = Header.GetStringFieldValue(Models.Metadata.Header.NameKey);
            string? description = Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey);
            string? date = Header.GetStringFieldValue(Models.Metadata.Header.DateKey);

            // If the description is defined but not the name, set the name from the description
            if (string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(description))
            {
                Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, description);
            }

            // If the name is defined but not the description, set the description from the name
            else if (!string.IsNullOrEmpty(name) && string.IsNullOrEmpty(description))
            {
                Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, name + (bare ? string.Empty : $" ({date})"));
            }

            // If neither the name or description are defined, set them from the automatic values
            else if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(description))
            {
                string[] splitpath = path.TrimEnd(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);
#if NETFRAMEWORK
                Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, splitpath[splitpath.Length - 1]);
                Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, splitpath[splitpath.Length - 1] + (bare ? string.Empty : $" ({date})"));
#else
                Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, splitpath[^1]);
                Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, splitpath[^1] + (bare ? string.Empty : $" ({date})"));
#endif
            }
        }

        #endregion

        #region Accessors

        /// <summary>
        /// Reset the internal item dictionary
        /// </summary>
        public void ResetDictionary()
        {
            Items.Clear();
            ItemsDB = new ItemDictionaryDB();
        }

        #endregion

        #region Filtering

        /// <summary>
        /// Execute all filters in a filter runner on the items in the dictionary
        /// </summary>
        /// <param name="filterRunner">Preconfigured filter runner to use</param>
        public void ExecuteFilters(FilterRunner filterRunner)
        {
            Items.ExecuteFilters(filterRunner);
            ItemsDB.ExecuteFilters(filterRunner);
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
        /// Add a DatItem to the dictionary after checking
        /// </summary>
        /// <param name="item">Item data to check against</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <returns>The key for the item</returns>
        protected string ParseAddHelper(DatItem item, bool statsOnly)
            => Items.AddItem(item, statsOnly);

        /// <summary>
        /// Add a DatItem to the dictionary after checking
        /// </summary>
        /// <param name="item">Item data to check against</param>
        /// <param name="machineIndex">Index of the machine to map the DatItem to</param>
        /// <param name="sourceIndex">Index of the source to map the DatItem to</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <returns>The key for the item</returns>
        protected long ParseAddHelper(DatItem item, long machineIndex, long sourceIndex, bool statsOnly)
            => ItemsDB.AddItem(item, machineIndex, sourceIndex, statsOnly);

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
        /// Create and open an output file for writing direct from a dictionary
        /// </summary>
        /// <param name="outfile">Name of the file to write to</param>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise (default)</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        /// <returns>True if the DAT was written correctly, false otherwise</returns>
        public abstract bool WriteToFileDB(string outfile, bool ignoreblanks = false, bool throwOnError = false);

        /// <summary>
        /// Create a prefix or postfix from inputs
        /// </summary>
        /// <param name="item">DatItem to create a prefix/postfix for</param>
        /// <param name="prefix">True for prefix, false for postfix</param>
        /// <returns>Sanitized string representing the postfix or prefix</returns>
        protected string CreatePrefixPostfix(DatItem item, bool prefix)
        {
            // Get machine for the item
            var machine = item.GetFieldValue<Machine>(DatItem.MachineKey);
            if (machine == null)
                return string.Empty;

            // Initialize strings
            string? type = item.GetStringFieldValue(Models.Metadata.DatItem.TypeKey);
            string fix,
                game = machine.GetStringFieldValue(Models.Metadata.Machine.NameKey) ?? string.Empty,
                manufacturer = machine.GetStringFieldValue(Models.Metadata.Machine.ManufacturerKey) ?? string.Empty,
                publisher = machine.GetStringFieldValue(Models.Metadata.Machine.PublisherKey) ?? string.Empty,
                category = machine.GetStringFieldValue(Models.Metadata.Machine.CategoryKey) ?? string.Empty,
                name = item.GetName() ?? type.AsEnumValue<ItemType>().AsStringValue() ?? string.Empty,
                crc = string.Empty,
                md5 = string.Empty,
                sha1 = string.Empty,
                sha256 = string.Empty,
                sha384 = string.Empty,
                sha512 = string.Empty,
                size = string.Empty,
                spamsum = string.Empty;

            // Check for quotes
            bool? quotes = Header.GetBoolFieldValue(DatHeader.QuotesKey);

            // If we have a prefix
            if (prefix)
            {
                string? prefixString = Header.GetStringFieldValue(DatHeader.PrefixKey);
                fix = prefixString + (quotes == true ? "\"" : string.Empty);
            }

            // If we have a postfix
            else
            {
                string? postfixString = Header.GetStringFieldValue(DatHeader.PostfixKey);
                fix = (quotes == true ? "\"" : string.Empty) + postfixString;
            }

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
            fix = fix
                .Replace("%game%", game)
                .Replace("%machine%", game)
                .Replace("%name%", name)
                .Replace("%manufacturer%", manufacturer)
                .Replace("%publisher%", publisher)
                .Replace("%category%", category)
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
        /// Create a prefix or postfix from inputs
        /// </summary>
        /// <param name="item">DatItem to create a prefix/postfix for</param>
        /// <param name="prefix">True for prefix, false for postfix</param>
        /// <returns>Sanitized string representing the postfix or prefix</returns>
        protected string CreatePrefixPostfixDB(KeyValuePair<long, DatItem> item, bool prefix)
        {
            // Get machine for the item
            var machine = ItemsDB.GetMachineForItem(item.Key);
            if (machine.Value == null)
                return string.Empty;

            // Initialize strings
            string? type = item.Value.GetStringFieldValue(Models.Metadata.DatItem.TypeKey);
            string fix,
                game = machine.Value.GetStringFieldValue(Models.Metadata.Machine.NameKey) ?? string.Empty,
                manufacturer = machine.Value.GetStringFieldValue(Models.Metadata.Machine.ManufacturerKey) ?? string.Empty,
                publisher = machine.Value.GetStringFieldValue(Models.Metadata.Machine.PublisherKey) ?? string.Empty,
                category = machine.Value.GetStringFieldValue(Models.Metadata.Machine.CategoryKey) ?? string.Empty,
                name = item.Value.GetName() ?? type.AsEnumValue<ItemType>().AsStringValue() ?? string.Empty,
                crc = string.Empty,
                md5 = string.Empty,
                sha1 = string.Empty,
                sha256 = string.Empty,
                sha384 = string.Empty,
                sha512 = string.Empty,
                size = string.Empty,
                spamsum = string.Empty;

            // Check for quotes
            bool? quotes = Header.GetBoolFieldValue(DatHeader.QuotesKey);

            // If we have a prefix
            if (prefix)
            {
                string? prefixString = Header.GetStringFieldValue(DatHeader.PrefixKey);
                fix = prefixString + (quotes == true ? "\"" : string.Empty);
            }

            // If we have a postfix
            else
            {
                string? postfixString = Header.GetStringFieldValue(DatHeader.PostfixKey);
                fix = (quotes == true ? "\"" : string.Empty) + postfixString;
            }

            // Ensure we have the proper values for replacement
            if (item.Value is Disk disk)
            {
                md5 = disk.GetStringFieldValue(Models.Metadata.Disk.MD5Key) ?? string.Empty;
                sha1 = disk.GetStringFieldValue(Models.Metadata.Disk.SHA1Key) ?? string.Empty;
            }
            else if (item.Value is Media media)
            {
                md5 = media.GetStringFieldValue(Models.Metadata.Media.MD5Key) ?? string.Empty;
                sha1 = media.GetStringFieldValue(Models.Metadata.Media.SHA1Key) ?? string.Empty;
                sha256 = media.GetStringFieldValue(Models.Metadata.Media.SHA256Key) ?? string.Empty;
                spamsum = media.GetStringFieldValue(Models.Metadata.Media.SpamSumKey) ?? string.Empty;
            }
            else if (item.Value is Rom rom)
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
            fix = fix
                .Replace("%game%", game)
                .Replace("%machine%", game)
                .Replace("%name%", name)
                .Replace("%manufacturer%", manufacturer)
                .Replace("%publisher%", publisher)
                .Replace("%category%", category)
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

            // Get the machine
            var machine = item.GetFieldValue<Machine>(DatItem.MachineKey);
            if (machine == null)
                return;

            // Get the name to update
            string? name = (Header.GetBoolFieldValue(DatHeader.UseRomNameKey) == true
                ? item.GetName()
                : machine.GetStringFieldValue(Models.Metadata.Machine.NameKey)) ?? string.Empty;

            // Create the proper Prefix and Postfix
            string pre = CreatePrefixPostfix(item, true);
            string post = CreatePrefixPostfix(item, false);

            // If we're in Depot mode, take care of that instead
            var outputDepot = Header.GetFieldValue<DepotInformation?>(DatHeader.OutputDepotKey);
            if (outputDepot?.IsActive == true)
            {
                if (item is Disk disk)
                {
                    // We can only write out if there's a SHA-1
                    string? sha1 = disk.GetStringFieldValue(Models.Metadata.Disk.SHA1Key);
                    if (!string.IsNullOrEmpty(sha1))
                    {
                        name = Utilities.GetDepotPath(sha1, outputDepot.Depth)?.Replace('\\', '/');
                        item.SetName($"{pre}{name}{post}");
                    }
                }
                else if (item is Media media)
                {
                    // We can only write out if there's a SHA-1
                    string? sha1 = media.GetStringFieldValue(Models.Metadata.Media.SHA1Key);
                    if (!string.IsNullOrEmpty(sha1))
                    {
                        name = Utilities.GetDepotPath(sha1, outputDepot.Depth)?.Replace('\\', '/');
                        item.SetName($"{pre}{name}{post}");
                    }
                }
                else if (item is Rom rom)
                {
                    // We can only write out if there's a SHA-1
                    string? sha1 = rom.GetStringFieldValue(Models.Metadata.Rom.SHA1Key);
                    if (!string.IsNullOrEmpty(sha1))
                    {
                        name = Utilities.GetDepotPath(sha1, outputDepot.Depth)?.Replace('\\', '/');
                        item.SetName($"{pre}{name}{post}");
                    }
                }

                return;
            }

            string? replaceExtension = Header.GetStringFieldValue(DatHeader.ReplaceExtensionKey);
            bool? removeExtension = Header.GetBoolFieldValue(DatHeader.RemoveExtensionKey);
            if (!string.IsNullOrEmpty(replaceExtension) || removeExtension == true)
            {
                if (removeExtension == true)
                    Header.SetFieldValue<string?>(DatHeader.ReplaceExtensionKey, string.Empty);

                string? dir = Path.GetDirectoryName(name);
                if (dir != null)
                {
                    dir = dir.TrimStart(Path.DirectorySeparatorChar);
                    name = Path.Combine(dir, Path.GetFileNameWithoutExtension(name) + replaceExtension);
                }
            }

            string? addExtension = Header.GetStringFieldValue(DatHeader.AddExtensionKey);
            if (!string.IsNullOrEmpty(addExtension))
                name += addExtension;

            if (Header.GetBoolFieldValue(DatHeader.UseRomNameKey) == true && Header.GetBoolFieldValue(DatHeader.GameNameKey) == true)
                name = Path.Combine(machine.GetStringFieldValue(Models.Metadata.Machine.NameKey) ?? string.Empty, name);

            // Now assign back the formatted name
            name = $"{pre}{name}{post}";
            if (Header.GetBoolFieldValue(DatHeader.UseRomNameKey) == true)
                item.SetName(name);
            else
                machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, name);

            // Restore all relevant values
            if (forceRemoveQuotes)
                Header.SetFieldValue<bool?>(DatHeader.QuotesKey, quotesBackup);

            if (forceRomName)
                Header.SetFieldValue<bool?>(DatHeader.UseRomNameKey, useRomNameBackup);
        }

        /// <summary>
        /// Process an item and correctly set the item name
        /// </summary>
        /// <param name="item">DatItem to update</param>
        /// <param name="forceRemoveQuotes">True if the Quotes flag should be ignored, false otherwise</param>
        /// <param name="forceRomName">True if the UseRomName should be always on (default), false otherwise</param>
        protected void ProcessItemNameDB(KeyValuePair<long, DatItem> item, bool forceRemoveQuotes, bool forceRomName = true)
        {
            // Backup relevant values and set new ones accordingly
            bool? quotesBackup = Header.GetBoolFieldValue(DatHeader.QuotesKey);
            bool? useRomNameBackup = Header.GetBoolFieldValue(DatHeader.UseRomNameKey);
            if (forceRemoveQuotes)
                Header.SetFieldValue<bool>(DatHeader.QuotesKey, false);
            if (forceRomName)
                Header.SetFieldValue<bool>(DatHeader.UseRomNameKey, true);

            // Get machine for the item
            var machine = ItemsDB.GetMachineForItem(item.Key);

            // Get the name to update
            string? name = (Header.GetBoolFieldValue(DatHeader.UseRomNameKey) == true
                ? item.Value.GetName()
                : machine.Value!.GetStringFieldValue(Models.Metadata.Machine.NameKey)) ?? string.Empty;

            // Create the proper Prefix and Postfix
            string pre = CreatePrefixPostfixDB(item, true);
            string post = CreatePrefixPostfixDB(item, false);

            // If we're in Depot mode, take care of that instead
            var outputDepot = Header.GetFieldValue<DepotInformation?>(DatHeader.OutputDepotKey);
            if (outputDepot?.IsActive == true)
            {
                if (item.Value is Disk disk)
                {
                    // We can only write out if there's a SHA-1
                    string? sha1 = disk.GetStringFieldValue(Models.Metadata.Disk.SHA1Key);
                    if (!string.IsNullOrEmpty(sha1))
                    {
                        name = Utilities.GetDepotPath(sha1, outputDepot.Depth)?.Replace('\\', '/');
                        item.Value.SetName($"{pre}{name}{post}");
                    }
                }
                else if (item.Value is Media media)
                {
                    // We can only write out if there's a SHA-1
                    string? sha1 = media.GetStringFieldValue(Models.Metadata.Media.SHA1Key);
                    if (!string.IsNullOrEmpty(sha1))
                    {
                        name = Utilities.GetDepotPath(sha1, outputDepot.Depth)?.Replace('\\', '/');
                        item.Value.SetName($"{pre}{name}{post}");
                    }
                }
                else if (item.Value is Rom rom)
                {
                    // We can only write out if there's a SHA-1
                    string? sha1 = rom.GetStringFieldValue(Models.Metadata.Rom.SHA1Key);
                    if (!string.IsNullOrEmpty(sha1))
                    {
                        name = Utilities.GetDepotPath(sha1, outputDepot.Depth)?.Replace('\\', '/');
                        item.Value.SetName($"{pre}{name}{post}");
                    }
                }

                return;
            }

            string? replaceExtension = Header.GetStringFieldValue(DatHeader.ReplaceExtensionKey);
            bool? removeExtension = Header.GetBoolFieldValue(DatHeader.RemoveExtensionKey);
            if (!string.IsNullOrEmpty(replaceExtension) || removeExtension == true)
            {
                if (removeExtension == true)
                    Header.SetFieldValue<string?>(DatHeader.ReplaceExtensionKey, string.Empty);

                string? dir = Path.GetDirectoryName(name);
                if (dir != null)
                {
                    dir = dir.TrimStart(Path.DirectorySeparatorChar);
                    name = Path.Combine(dir, Path.GetFileNameWithoutExtension(name) + replaceExtension);
                }
            }

            string? addExtension = Header.GetStringFieldValue(DatHeader.AddExtensionKey);
            if (!string.IsNullOrEmpty(addExtension))
                name += addExtension;

            if (Header.GetBoolFieldValue(DatHeader.UseRomNameKey) == true && Header.GetBoolFieldValue(DatHeader.GameNameKey) == true)
                name = Path.Combine(machine.Value!.GetStringFieldValue(Models.Metadata.Machine.NameKey) ?? string.Empty, name);

            // Now assign back the formatted name
            name = $"{pre}{name}{post}";
            if (Header.GetBoolFieldValue(DatHeader.UseRomNameKey) == true)
                item.Value.SetName(name);
            else if (machine.Value != null)
                machine.Value.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, name);

            // Restore all relevant values
            if (forceRemoveQuotes)
                Header.SetFieldValue<bool?>(DatHeader.QuotesKey, quotesBackup);

            if (forceRomName)
                Header.SetFieldValue<bool?>(DatHeader.UseRomNameKey, useRomNameBackup);
        }

        /// <summary>
        /// Process any DatItems that are "null", usually created from directory population
        /// </summary>
        /// <param name="item">DatItem to check for "null" status</param>
        /// <returns>Cleaned DatItem</returns>
        protected DatItem ProcessNullifiedItem(DatItem item)
        {
            // If we don't have a Rom, we can ignore it
            if (item is not Rom rom)
                return item;

            // Get machine for the item
            var machine = item.GetFieldValue<Machine>(DatItem.MachineKey);
            if (machine == null)
                return item;

            // If the Rom has "null" characteristics, ensure all fields
            if (rom.GetInt64FieldValue(Models.Metadata.Rom.SizeKey) == null
                && rom.GetStringFieldValue(Models.Metadata.Rom.CRCKey) == "null")
            {
                logger.Verbose($"Empty folder found: {machine.GetStringFieldValue(Models.Metadata.Machine.NameKey)}");

                rom.SetName(rom.GetName() == "null" ? "-" : rom.GetName());
                rom.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, Constants.SizeZero.ToString());
                rom.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey,
                    rom.GetStringFieldValue(Models.Metadata.Rom.CRCKey) == "null" ? ZeroHash.CRC32Str : null);
                rom.SetFieldValue<string?>(Models.Metadata.Rom.MD5Key,
                    rom.GetStringFieldValue(Models.Metadata.Rom.MD5Key) == "null" ? ZeroHash.MD5Str : null);
                rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key,
                    rom.GetStringFieldValue(Models.Metadata.Rom.SHA1Key) == "null" ? ZeroHash.SHA1Str : null);
                rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA256Key,
                    rom.GetStringFieldValue(Models.Metadata.Rom.SHA256Key) == "null" ? ZeroHash.SHA256Str : null);
                rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA384Key,
                    rom.GetStringFieldValue(Models.Metadata.Rom.SHA384Key) == "null" ? ZeroHash.SHA384Str : null);
                rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA512Key,
                    rom.GetStringFieldValue(Models.Metadata.Rom.SHA512Key) == "null" ? ZeroHash.SHA512Str : null);
                rom.SetFieldValue<string?>(Models.Metadata.Rom.SpamSumKey,
                    rom.GetStringFieldValue(Models.Metadata.Rom.SpamSumKey) == "null" ? ZeroHash.SpamSumStr : null);
            }

            return rom;
        }

        /// <summary>
        /// Process any DatItems that are "null", usually created from directory population
        /// </summary>
        /// <param name="item">DatItem to check for "null" status</param>
        /// <returns>Cleaned DatItem</returns>
        protected KeyValuePair<long, DatItem> ProcessNullifiedItem(KeyValuePair<long, DatItem> item)
        {
            // If we don't have a Rom, we can ignore it
            if (item.Value is not Rom rom)
                return item;

            // Get machine for the item
            var machine = ItemsDB.GetMachineForItem(item.Key);
            var machineObj = machine.Value;
            if (machineObj == null)
                return item;

            // If the Rom has "null" characteristics, ensure all fields
            if (rom.GetInt64FieldValue(Models.Metadata.Rom.SizeKey) == null
                && rom.GetStringFieldValue(Models.Metadata.Rom.CRCKey) == "null")
            {
                logger.Verbose($"Empty folder found: {machineObj.GetStringFieldValue(Models.Metadata.Machine.NameKey)}");

                rom.SetName(rom.GetName() == "null" ? "-" : rom.GetName());
                rom.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, Constants.SizeZero.ToString());
                rom.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey,
                    rom.GetStringFieldValue(Models.Metadata.Rom.CRCKey) == "null" ? ZeroHash.CRC32Str : null);
                rom.SetFieldValue<string?>(Models.Metadata.Rom.MD5Key,
                    rom.GetStringFieldValue(Models.Metadata.Rom.MD5Key) == "null" ? ZeroHash.MD5Str : null);
                rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key,
                    rom.GetStringFieldValue(Models.Metadata.Rom.SHA1Key) == "null" ? ZeroHash.SHA1Str : null);
                rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA256Key,
                    rom.GetStringFieldValue(Models.Metadata.Rom.SHA256Key) == "null" ? ZeroHash.SHA256Str : null);
                rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA384Key,
                    rom.GetStringFieldValue(Models.Metadata.Rom.SHA384Key) == "null" ? ZeroHash.SHA384Str : null);
                rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA512Key,
                    rom.GetStringFieldValue(Models.Metadata.Rom.SHA512Key) == "null" ? ZeroHash.SHA512Str : null);
                rom.SetFieldValue<string?>(Models.Metadata.Rom.SpamSumKey,
                    rom.GetStringFieldValue(Models.Metadata.Rom.SpamSumKey) == "null" ? ZeroHash.SpamSumStr : null);
            }

            return new KeyValuePair<long, DatItem>(item.Key, rom);
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
        protected bool ContainsWritable(List<DatItem> datItems)
        {
            // Empty machines are considered writable
            if (datItems == null || datItems.Count == 0)
                return true;

            foreach (DatItem datItem in datItems)
            {
                ItemType itemType = datItem.GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>();
                if (Array.Exists(GetSupportedTypes(), t => t == itemType))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Get if a machine contains any writable items
        /// </summary>
        /// <param name="datItems">DatItems to check</param>
        /// <returns>True if the machine contains at least one writable item, false otherwise</returns>
        /// <remarks>Empty machines are kept with this</remarks>
        protected bool ContainsWritable(Dictionary<long, DatItem>? datItems)
        {
            // Empty machines are considered writable
            if (datItems == null || datItems.Count == 0)
                return true;

            foreach (var datItem in datItems)
            {
                ItemType itemType = datItem.Value.GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>();
                if (Array.Exists(GetSupportedTypes(), t => t == itemType))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Resolve name duplicates in an arbitrary set of DatItems based on the supplied information
        /// </summary>
        /// <param name="items">List of DatItem objects representing the items to be merged</param>
        /// <returns>A List of DatItem objects representing the renamed items</returns>
        protected List<DatItem> ResolveNames(List<DatItem> items)
        {
            // Create the output list
            List<DatItem> output = [];

            // First we want to make sure the list is in alphabetical order
            DatItemTool.Sort(ref items, true);

            // Now we want to loop through and check names
            DatItem? lastItem = null;
            string? lastrenamed = null;
            int lastid = 0;
            for (int i = 0; i < items.Count; i++)
            {
                DatItem datItem = items[i];

                // If we have the first item, we automatically add it
                if (lastItem == null)
                {
                    output.Add(datItem);
                    lastItem = datItem;
                    continue;
                }

                // Get the last item name, if applicable
                string lastItemName = lastItem.GetName()
                    ?? lastItem.GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>().AsStringValue()
                    ?? string.Empty;

                // Get the current item name, if applicable
                string datItemName = datItem.GetName()
                    ?? datItem.GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>().AsStringValue()
                    ?? string.Empty;

                // If the current item exactly matches the last item, then we don't add it
#if NET20 || NET35
                if ((datItem.GetDuplicateStatus(lastItem) & DupeType.All) != 0)
#else
                if (datItem.GetDuplicateStatus(lastItem).HasFlag(DupeType.All))
#endif
                {
                    logger.Verbose($"Exact duplicate found for '{datItemName}'");
                    continue;
                }

                // If the current name matches the previous name, rename the current item
                else if (datItemName == lastItemName)
                {
                    logger.Verbose($"Name duplicate found for '{datItemName}'");

                    if (datItem is Disk || datItem is DatItems.Formats.File || datItem is Media || datItem is Rom)
                    {
                        datItemName += GetDuplicateSuffix(datItem);
                        lastrenamed ??= datItemName;
                    }

                    // If we have a conflict with the last renamed item, do the right thing
                    if (datItemName == lastrenamed)
                    {
                        lastrenamed = datItemName;
                        datItemName += (lastid == 0 ? string.Empty : "_" + lastid);
                        lastid++;
                    }
                    // If we have no conflict, then we want to reset the lastrenamed and id
                    else
                    {
                        lastrenamed = null;
                        lastid = 0;
                    }

                    // Set the item name back to the datItem
                    datItem.SetName(datItemName);

                    output.Add(datItem);
                }

                // Otherwise, we say that we have a valid named file
                else
                {
                    output.Add(datItem);
                    lastItem = datItem;
                    lastrenamed = null;
                    lastid = 0;
                }
            }

            // One last sort to make sure this is ordered
            DatItemTool.Sort(ref output, true);

            return output;
        }

        /// <summary>
        /// Resolve name duplicates in an arbitrary set of DatItems based on the supplied information
        /// </summary>
        /// <param name="mappings">List of item ID to DatItem mappings representing the items to be merged</param>
        /// <returns>A List of DatItem objects representing the renamed items</returns>
        protected List<KeyValuePair<long, DatItem>> ResolveNamesDB(List<KeyValuePair<long, DatItem>> mappings)
        {
            // Create the output dict
            List<KeyValuePair<long, DatItem>> output = [];

            // First we want to make sure the list is in alphabetical order
            DatItemTool.SortDB(ref mappings, true);

            // Now we want to loop through and check names
            DatItem? lastItem = null;
            string? lastrenamed = null;
            int lastid = 0;
            foreach (var datItem in mappings)
            {
                // If we have the first item, we automatically add it
                if (lastItem == null)
                {
                    output.Add(datItem);
                    lastItem = datItem.Value;
                    continue;
                }

                // Get the last item name, if applicable
                string lastItemName = lastItem.GetName()
                    ?? lastItem.GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>().AsStringValue()
                    ?? string.Empty;

                // Get the current item name, if applicable
                string datItemName = datItem.Value.GetName()
                    ?? datItem.Value.GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>().AsStringValue()
                    ?? string.Empty;

                // If the current item exactly matches the last item, then we don't add it
#if NET20 || NET35
                if ((datItem.Value.GetDuplicateStatus(lastItem) & DupeType.All) != 0)
#else
                if (datItem.Value.GetDuplicateStatus(lastItem).HasFlag(DupeType.All))
#endif
                {
                    logger.Verbose($"Exact duplicate found for '{datItemName}'");
                    continue;
                }

                // If the current name matches the previous name, rename the current item
                else if (datItemName == lastItemName)
                {
                    logger.Verbose($"Name duplicate found for '{datItemName}'");

                    if (datItem.Value is Disk || datItem.Value is DatItems.Formats.File || datItem.Value is Media || datItem.Value is Rom)
                    {
                        datItemName += GetDuplicateSuffix(datItem.Value);
                        lastrenamed ??= datItemName;
                    }

                    // If we have a conflict with the last renamed item, do the right thing
                    if (datItemName == lastrenamed)
                    {
                        lastrenamed = datItemName;
                        datItemName += (lastid == 0 ? string.Empty : "_" + lastid);
                        lastid++;
                    }
                    // If we have no conflict, then we want to reset the lastrenamed and id
                    else
                    {
                        lastrenamed = null;
                        lastid = 0;
                    }

                    // Set the item name back to the datItem
                    datItem.Value.SetName(datItemName);
                    output.Add(datItem);
                }

                // Otherwise, we say that we have a valid named file
                else
                {
                    output.Add(datItem);
                    lastItem = datItem.Value;
                    lastrenamed = null;
                    lastid = 0;
                }
            }

            // One last sort to make sure this is ordered
            DatItemTool.SortDB(ref output, true);

            return output;
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
            string datFormat = Header?.GetFieldValue<DatFormat>(DatHeader.DatFormatKey).ToString() ?? "Unknown Format";
            ItemType itemType = datItem.GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>();
            if (!Array.Exists(GetSupportedTypes(), t => t == itemType))
            {
                string itemString = JsonConvert.SerializeObject(datItem, Formatting.None);
                logger?.Verbose($"Item '{itemString}' was skipped because it was not supported in {datFormat}");
                return true;
            }

            // If we have an item with missing required fields
            List<string>? missingFields = GetMissingRequiredFields(datItem);
            if (missingFields != null && missingFields.Count != 0)
            {
                string itemString = JsonConvert.SerializeObject(datItem, Formatting.None);
#if NET20 || NET35
                logger?.Verbose($"Item '{itemString}' was skipped because it was missing required fields for {datFormat}: {string.Join(", ", [.. missingFields])}");
#else
                logger?.Verbose($"Item '{itemString}' was skipped because it was missing required fields for {datFormat}: {string.Join(", ", missingFields)}");
#endif
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get duplicate suffix based on the item type
        /// </summary>
        private static string GetDuplicateSuffix(DatItem datItem)
        {
            return datItem switch
            {
                Disk disk => disk.GetDuplicateSuffix(),
                DatItems.Formats.File file => file.GetDuplicateSuffix(),
                Media media => media.GetDuplicateSuffix(),
                Rom rom => rom.GetDuplicateSuffix(),
                _ => "_1",
            };
        }

        #endregion
    }
}
