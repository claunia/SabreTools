using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using SabreTools.Core;
using SabreTools.DatFiles.Formats;
using SabreTools.DatItems;
using SabreTools.Logging;
using Newtonsoft.Json;

namespace SabreTools.DatFiles
{
    /// <summary>
    /// Represents a format-agnostic DAT
    /// </summary>
    /// <remarks>
    /// The fact that this one class could be separated into as many partial
    /// classes as it did means that the functionality here should probably
    /// be split out into either separate classes or even an entirely separate
    /// namespace. Also, with that in mind, each of the individual DatFile types
    /// probably should only need to inherit from a thin abstract class and
    /// should not be exposed as part of the library, instead being taken care
    /// of behind the scenes as part of the reading and writing.
    /// </remarks>
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
            switch (datFormat)
            {
                case DatFormat.AttractMode:
                    return new AttractMode(baseDat);

                case DatFormat.ClrMamePro:
                    return new ClrMamePro(baseDat, quotes);

                case DatFormat.CSV:
                    return new SeparatedValue(baseDat, ',');

                case DatFormat.DOSCenter:
                    return new DosCenter(baseDat);

                case DatFormat.EverdriveSMDB:
                    return new EverdriveSMDB(baseDat);

                case DatFormat.Listrom:
                    return new Listrom(baseDat);

                case DatFormat.Listxml:
                    return new Listxml(baseDat);

                case DatFormat.Logiqx:
                    return new Logiqx(baseDat, false);

                case DatFormat.LogiqxDeprecated:
                    return new Logiqx(baseDat, true);

                case DatFormat.MissFile:
                    return new Missfile(baseDat);

                case DatFormat.OfflineList:
                    return new OfflineList(baseDat);

                case DatFormat.OpenMSX:
                    return new OpenMSX(baseDat);

                case DatFormat.RedumpMD5:
                    return new Hashfile(baseDat, Hash.MD5);

#if NET_FRAMEWORK
                case DatFormat.RedumpRIPEMD160:
                    return new Hashfile(baseDat, Hash.RIPEMD160);
#endif

                case DatFormat.RedumpSFV:
                    return new Hashfile(baseDat, Hash.CRC);

                case DatFormat.RedumpSHA1:
                    return new Hashfile(baseDat, Hash.SHA1);

                case DatFormat.RedumpSHA256:
                    return new Hashfile(baseDat, Hash.SHA256);

                case DatFormat.RedumpSHA384:
                    return new Hashfile(baseDat, Hash.SHA384);

                case DatFormat.RedumpSHA512:
                    return new Hashfile(baseDat, Hash.SHA512);

                case DatFormat.RedumpSpamSum:
                    return new Hashfile(baseDat, Hash.SpamSum);

                case DatFormat.RomCenter:
                    return new RomCenter(baseDat);

                case DatFormat.SabreJSON:
                    return new SabreJSON(baseDat);

                case DatFormat.SabreXML:
                    return new SabreXML(baseDat);

                case DatFormat.SoftwareList:
                    return new Formats.SoftwareList(baseDat);

                case DatFormat.SSV:
                    return new SeparatedValue(baseDat, ';');

                case DatFormat.TSV:
                    return new SeparatedValue(baseDat, '\t');

                // We use new-style Logiqx as a backup for generic DatFile
                case null:
                default:
                    return new Logiqx(baseDat, false);
            }
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
        /// Add items from another DatFile to the existing DatFile
        /// </summary>
        /// <param name="datFile">DatFile to add from</param>
        /// <param name="delete">If items should be deleted from the source DatFile</param>
        public void AddFromExisting(DatFile datFile, bool delete = false)
        {
            // Get the list of keys from the DAT
            var keys = datFile.Items.Keys.ToList();
            foreach (string key in keys)
            {
                // Add everything from the key to the internal DAT
                Items.AddRange(key, datFile.Items[key]);

                // Now remove the key from the source DAT
                if (delete)
                    datFile.Items.Remove(key);
            }

            // Now remove the file dictionary from the source DAT
            if (delete)
                datFile.Items = null;
        }

        /// <summary>
        /// Apply a DatHeader to an existing DatFile
        /// </summary>
        /// <param name="datHeader">DatHeader to get the values from</param>
        public void ApplyDatHeader(DatHeader datHeader)
        {
            Header.ConditionalCopy(datHeader);
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
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        public abstract void ParseFile(string filename, int indexId, bool keep, bool throwOnError = false);

        /// <summary>
        /// Add a rom to the Dat after checking
        /// </summary>
        /// <param name="item">Item data to check against</param>
        /// <returns>The key for the item</returns>
        protected string ParseAddHelper(DatItem item)
        {
            string key = string.Empty;

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
#if NET_FRAMEWORK
                    rom.RIPEMD160 = null; // Constants.RIPEMD160Zero;
#endif
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
            key = item.GetKey(Field.Machine_Name);
            Items.Add(key, item);

            return key;
        }

        #region Input Sanitization

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

        #endregion
    }
}
