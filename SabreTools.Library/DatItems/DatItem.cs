using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

using SabreTools.Library.Data;
using SabreTools.Library.DatFiles;
using SabreTools.Library.FileTypes;
using SabreTools.Library.Filtering;
using SabreTools.Library.Tools;
using NaturalSort;
using Newtonsoft.Json;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Base class for all items included in a set
    /// </summary>
    public abstract class DatItem : IEquatable<DatItem>, IComparable<DatItem>, ICloneable
    {
        #region Fields

        #region Common Fields

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Item type for outputting
        /// </summary>
        [JsonIgnore]
        public ItemType ItemType { get; set; }

        /// <summary>
        /// Duplicate type when compared to another item
        /// </summary>
        [JsonIgnore]
        public DupeType DupeType { get; set; }

        #endregion

        #region Machine Fields

        /// <summary>
        /// Machine values
        /// </summary>
        [JsonIgnore]
        public Machine Machine { get; set; } = new Machine();

        #endregion

        #region AttractMode Fields

        /// <summary>
        /// Alternate name for the item
        /// </summary>
        [JsonProperty("alt_romname")]
        public string AltName { get; set; }

        /// <summary>
        /// Alternate title for the item
        /// </summary>
        [JsonProperty("alt_title")]
        public string AltTitle { get; set; }

        #endregion

        #region SoftwareList Fields

        /// <summary>
        /// Original hardware part associated with the item
        /// </summary>
        [JsonProperty("partname")]
        public string PartName { get; set; }

        /// <summary>
        /// Original hardware interface associated with the item
        /// </summary>
        [JsonProperty("partinterface")]
        public string PartInterface { get; set; }

        /// <summary>
        /// Features provided to/by the item
        /// </summary>
        [JsonProperty("features")]
        public List<KeyValuePair<string, string>> Features { get; set; }

        /// <summary>
        /// Original hardware part name within an item
        /// </summary>
        [JsonProperty("areaname")]
        public string AreaName { get; set; }

        /// <summary>
        /// Original hardware size within the part
        /// </summary>
        [JsonProperty("areasize")]
        public long? AreaSize { get; set; }

        /// <summary>
        /// Width of the data area in bytes
        /// </summary>
        /// TODO: Convert to Int32
        [JsonProperty("width")]
        public string AreaWidth { get; set; } // (8|16|32|64) "8"

        /// <summary>
        /// Endianness of the data area
        /// </summary>
        /// TODO: Convert to Enum?
        [JsonProperty("endianness")]
        public string AreaEndianness { get; set; } // (big|little) "little"

        /// <summary>
        /// SoftwareList value associated with the item
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }

        /// <summary>
        /// Loading flag
        /// </summary>
        /// TODO: Convert to Enum?
        [JsonProperty("loadflag")]
        public string LoadFlag { get; set; } // (load16_byte|load16_word|load16_word_swap|load32_byte|load32_word|load32_word_swap|load32_dword|load64_word|load64_word_swap|reload|fill|continue|reload_plain|ignore)

        #endregion

        #region Metadata information

        /// <summary>
        /// Source information
        /// </summary>
        [JsonIgnore]
        public Source Source { get; set; } = new Source();

        /// <summary>
        /// Flag if item should be removed
        /// </summary>
        [JsonIgnore]
        public bool Remove { get; set; }

        #endregion

        #region Static Values

        /// <summary>
        /// Fields unique to a DatItem
        /// </summary>
        public static readonly List<Field> DatItemFields = new List<Field>()
        {
            // Common
            Field.Name,

            // AttractMode
            Field.AltName,
            Field.AltTitle,

            //SoftwareList
            Field.PartName,
            Field.PartInterface,
            Field.Features,
            Field.AreaName,
            Field.AreaSize,
            Field.AreaWidth,
            Field.AreaEndianness,
            Field.Value,
            Field.LoadFlag,

            // BiosSet
            Field.BiosDescription,
            Field.Default,

            // Disk
            Field.MD5,
#if NET_FRAMEWORK
            Field.RIPEMD160,
#endif
            Field.SHA1,
            Field.SHA256,
            Field.SHA384,
            Field.SHA512,
            Field.Merge,
            Field.Region,
            Field.Index,
            Field.Writable,
            Field.Optional,
            Field.Status,

            // Release
            Field.Language,
            Field.Date,

            // Rom
            Field.Bios,
            Field.Size,
            Field.CRC,
            Field.Offset,
            Field.Inverted,
        };

        /// <summary>
        /// Fields unique to a Machine
        /// </summary>
        public static readonly List<Field> MachineFields = new List<Field>()
        {
            // Common
            Field.MachineName,
            Field.Comment,
            Field.Description,
            Field.Year,
            Field.Manufacturer,
            Field.Publisher,
            Field.RomOf,
            Field.CloneOf,
            Field.SampleOf,
            Field.MachineType,

            // AttractMode
            Field.Players,
            Field.Rotation,
            Field.Control,
            Field.SupportStatus,
            Field.DisplayCount,
            Field.DisplayType,
            Field.Buttons,

            // ListXML
            Field.SourceFile,
            Field.Runnable,
            Field.Devices,
            Field.SlotOptions,
            Field.Infos,

            // Logiqx
            Field.Board,
            Field.RebuildTo,

            // Logiqx EmuArc
            Field.TitleID,
            Field.Developer,
            Field.Genre,
            Field.Subgenre,
            Field.Ratings,
            Field.Score,
            Field.Enabled,
            Field.HasCrc,
            Field.RelatedTo,

            // SoftwareList
            Field.Supported,
            Field.SharedFeatures,
        };

        #endregion

        #endregion

        #region Instance Methods

        #region Accessors

        /// <summary>
        /// Get the value of that field as a string, if possible
        /// </summary>
        public virtual string GetField(Field field, List<Field> excludeFields)
        {
            // If the field is to be excluded, return empty string
            if (excludeFields.Contains(field))
                return string.Empty;

            // Try to get the machine field first
            string fieldValue = Machine.GetField(field, excludeFields);
            if (fieldValue != null)
                return fieldValue;

            switch (field)
            {
                case Field.Name:
                    fieldValue = Name;
                    break;
                case Field.AltName:
                    fieldValue = AltName;
                    break;
                case Field.AltTitle:
                    fieldValue = AltTitle;
                    break;
                case Field.PartName:
                    fieldValue = PartName;
                    break;
                case Field.PartInterface:
                    fieldValue = PartInterface;
                    break;
                case Field.Features:
                    fieldValue = string.Join(";", (Features ?? new List<KeyValuePair<string, string>>()).Select(f => $"{f.Key}={f.Value}"));
                    break;
                case Field.AreaName:
                    fieldValue = AreaName;
                    break;
                case Field.AreaSize:
                    fieldValue = AreaSize?.ToString();
                    break;
                case Field.AreaWidth:
                    fieldValue = AreaWidth;
                    break;
                case Field.AreaEndianness:
                    fieldValue = AreaEndianness;
                    break;
                case Field.Value:
                    fieldValue = Value;
                    break;
                case Field.LoadFlag:
                    fieldValue = LoadFlag;
                    break;

                case Field.NULL:
                default:
                    return string.Empty;
            }

            // Make sure we don't return null
            if (string.IsNullOrEmpty(fieldValue))
                fieldValue = string.Empty;

            return fieldValue;
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="mappings">Mappings dictionary</param>
        public virtual void SetFields(Dictionary<Field, string> mappings)
        {
            // Set machine fields
            Machine.SetFields(mappings);

            #region Common

            if (mappings.Keys.Contains(Field.Name))
                Name = mappings[Field.Name];

            #endregion

            #region AttractMode

            if (mappings.Keys.Contains(Field.AltName))
                AltName = mappings[Field.AltName];

            if (mappings.Keys.Contains(Field.AltTitle))
                AltTitle = mappings[Field.AltTitle];

            #endregion

            #region SoftwareList

            if (mappings.Keys.Contains(Field.PartName))
                PartName = mappings[Field.PartName];

            if (mappings.Keys.Contains(Field.PartInterface))
                PartInterface = mappings[Field.PartInterface];

            if (mappings.Keys.Contains(Field.Features))
            {
                if (Features == null)
                    Features = new List<KeyValuePair<string, string>>();

                string[] pairs = mappings[Field.Features].Split(';');
                foreach (string pair in pairs)
                {
                    string[] split = pair.Split('=');
                    Features.Add(new KeyValuePair<string, string>(split[0], split[1]));
                }
            }

            if (mappings.Keys.Contains(Field.AreaName))
                AreaName = mappings[Field.AreaName];

            if (mappings.Keys.Contains(Field.AreaSize))
            {
                if (Int64.TryParse(mappings[Field.AreaSize], out long areaSize))
                    AreaSize = areaSize;
            }

            if (mappings.Keys.Contains(Field.AreaWidth))
                AreaWidth = mappings[Field.AreaWidth];

            if (mappings.Keys.Contains(Field.AreaEndianness))
                AreaEndianness = mappings[Field.AreaEndianness];

            if (mappings.Keys.Contains(Field.Value))
                Value = mappings[Field.Value];

            if (mappings.Keys.Contains(Field.LoadFlag))
                LoadFlag = mappings[Field.LoadFlag];

            #endregion
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a specific type of DatItem to be used based on an ItemType
        /// </summary>
        /// <param name="itemType">Type of the DatItem to be created</param>
        /// <returns>DatItem of the specific internal type that corresponds to the inputs</returns>
        public static DatItem Create(ItemType itemType)
        {
#if NET_FRAMEWORK
            switch (itemType)
            {
                case ItemType.Archive:
                    return new Archive();

                case ItemType.BiosSet:
                    return new BiosSet();

                case ItemType.Blank:
                    return new Blank();

                case ItemType.Disk:
                    return new Disk();

                case ItemType.Release:
                    return new Release();

                case ItemType.Rom:
                    return new Rom();

                case ItemType.Sample:
                    return new Sample();

                default:
                    return new Rom();
            }
#else
            return itemType switch
            {
                ItemType.Archive => new Archive(),
                ItemType.BiosSet => new BiosSet(),
                ItemType.Blank => new Blank(),
                ItemType.Disk => new Disk(),
                ItemType.Release => new Release(),
                ItemType.Rom => new Rom(),
                ItemType.Sample => new Sample(),
                _ => new Rom(),
            };
#endif
        }

        /// <summary>
        /// Create a specific type of DatItem to be used based on a BaseFile
        /// </summary>
        /// <param name="baseFile">BaseFile containing information to be created</param>
        /// <returns>DatItem of the specific internal type that corresponds to the inputs</returns>
        public static DatItem Create(BaseFile baseFile)
        {
            switch (baseFile.Type)
            {
                case FileType.CHD:
                    return new Disk(baseFile);

                case FileType.GZipArchive:
                case FileType.LRZipArchive:
                case FileType.LZ4Archive:
                case FileType.None:
                case FileType.RarArchive:
                case FileType.SevenZipArchive:
                case FileType.TapeArchive:
                case FileType.XZArchive:
                case FileType.ZipArchive:
                case FileType.ZPAQArchive:
                case FileType.ZstdArchive:
                    return new Rom(baseFile);

                case FileType.Folder:
                default:
                    return null;
            }
        }

        #endregion

        #region Cloning Methods

        /// <summary>
        /// Clone the DatItem
        /// </summary>
        /// <returns>Clone of the DatItem</returns>
        public abstract object Clone();

        /// <summary>
        /// Copy all machine information over in one shot
        /// </summary>
        /// <param name="item">Existing item to copy information from</param>
        public void CopyMachineInformation(DatItem item)
        {
            Machine = (Machine)item.Machine.Clone();
        }

        /// <summary>
        /// Copy all machine information over in one shot
        /// </summary>
        /// <param name="machine">Existing machine to copy information from</param>
        public void CopyMachineInformation(Machine machine)
        {
            Machine = (Machine)machine.Clone();
        }

        #endregion

        #region Comparision Methods

        public int CompareTo(DatItem other)
        {
            try
            {
                if (Name == other.Name)
                    return Equals(other) ? 0 : 1;

                return string.Compare(Name, other.Name);
            }
            catch
            {
                return 1;
            }
        }

        /// <summary>
        /// Determine if an item is a duplicate using partial matching logic
        /// </summary>
        /// <param name="other">DatItem to use as a baseline</param>
        /// <returns>True if the roms are duplicates, false otherwise</returns>
        public abstract bool Equals(DatItem other);

        /// <summary>
        /// Return the duplicate status of two items
        /// </summary>
        /// <param name="lastItem">DatItem to check against</param>
        /// <returns>The DupeType corresponding to the relationship between the two</returns>
        public DupeType GetDuplicateStatus(DatItem lastItem)
        {
            DupeType output = 0x00;

            // If we don't have a duplicate at all, return none
            if (!Equals(lastItem))
                return output;

            // If the duplicate is external already or should be, set it
            if (lastItem.DupeType.HasFlag(DupeType.External) || lastItem.Source.Index != Source.Index)
            {
                if (lastItem.Machine.Name == Machine.Name && lastItem.Name == Name)
                    output = DupeType.External | DupeType.All;
                else
                    output = DupeType.External | DupeType.Hash;
            }

            // Otherwise, it's considered an internal dupe
            else
            {
                if (lastItem.Machine.Name == Machine.Name && lastItem.Name == Name)
                    output = DupeType.Internal | DupeType.All;
                else
                    output = DupeType.Internal | DupeType.Hash;
            }

            return output;
        }

        #endregion

        #region Filtering

        /// <summary>
        /// Check to see if a DatItem passes the filter
        /// </summary>
        /// <param name="filter">Filter to check against</param>
        /// <returns>True if the item passed the filter, false otherwise</returns>
        public virtual bool PassesFilter(Filter filter)
        {
            // Filter on machine fields
            if (!Machine.PassesFilter(filter))
                return false;

            #region Common

            // Filter on item name
            if (filter.ItemName.MatchesPositiveSet(Name) == false)
                return false;
            if (filter.ItemName.MatchesNegativeSet(Name) == true)
                return false;

            // Filter on item type
            if (filter.ItemTypes.MatchesPositiveSet(ItemType.ToString()) == false)
                return false;
            if (filter.ItemTypes.MatchesNegativeSet(ItemType.ToString()) == true)
                return false;

            #endregion

            #region AttractMode

            // Filter on alt name
            if (filter.AltName.MatchesPositiveSet(AltName) == false)
                return false;
            if (filter.AltName.MatchesNegativeSet(AltName) == true)
                return false;

            // Filter on alt title
            if (filter.AltTitle.MatchesPositiveSet(AltTitle) == false)
                return false;
            if (filter.AltTitle.MatchesNegativeSet(AltTitle) == true)
                return false;

            #endregion

            #region SoftwareList

            // Filter on part name
            if (filter.PartName.MatchesPositiveSet(PartName) == false)
                return false;
            if (filter.PartName.MatchesNegativeSet(PartName) == true)
                return false;

            // Filter on part interface
            if (filter.PartInterface.MatchesPositiveSet(PartInterface) == false)
                return false;
            if (filter.PartInterface.MatchesNegativeSet(PartInterface) == true)
                return false;

            // Filter on area name
            if (filter.AreaName.MatchesPositiveSet(AreaName) == false)
                return false;
            if (filter.AreaName.MatchesNegativeSet(AreaName) == true)
                return false;

            // Filter on area size
            if (filter.AreaSize.MatchesNeutral(null, AreaSize) == false)
                return false;
            else if (filter.AreaSize.MatchesPositive(null, AreaSize) == false)
                return false;
            else if (filter.AreaSize.MatchesNegative(null, AreaSize) == false)
                return false;

            // Filter on area byte width
            if (filter.AreaWidth.MatchesPositiveSet(AreaWidth) == false)
                return false;
            if (filter.AreaWidth.MatchesNegativeSet(AreaWidth) == true)
                return false;

            // Filter on area endianness
            if (filter.AreaEndianness.MatchesPositiveSet(AreaEndianness) == false)
                return false;
            if (filter.AreaEndianness.MatchesNegativeSet(AreaEndianness) == true)
                return false;

            // Filter on softwarelist value
            if (filter.Value.MatchesPositiveSet(Value) == false)
                return false;
            if (filter.Value.MatchesNegativeSet(Value) == true)
                return false;

            // Filter on load flag
            if (filter.LoadFlag.MatchesPositiveSet(LoadFlag) == false)
                return false;
            if (filter.LoadFlag.MatchesNegativeSet(LoadFlag) == true)
                return false;

            #endregion

            return true;
        }

        /// <summary>
        /// Remove fields from the DatItem
        /// </summary>
        /// <param name="fields">List of Fields to remove</param>
        public virtual void RemoveFields(List<Field> fields)
        {
            // Remove machine fields
            Machine.RemoveFields(fields);

            #region Common

            if (fields.Contains(Field.Name))
                Name = null;

            #endregion

            #region AttractMode

            if (fields.Contains(Field.AltName))
                AltName = null;

            if (fields.Contains(Field.AltTitle))
                AltTitle = null;

            #endregion

            #region SoftwareList

            if (fields.Contains(Field.PartName))
                PartName = null;

            if (fields.Contains(Field.PartInterface))
                PartInterface = null;

            if (fields.Contains(Field.Features))
                Features = null;

            if (fields.Contains(Field.AreaName))
                AreaName = null;

            if (fields.Contains(Field.AreaSize))
                AreaSize = null;

            if (fields.Contains(Field.AreaWidth))
                AreaWidth = null;

            if (fields.Contains(Field.AreaEndianness))
                AreaEndianness = null;

            if (fields.Contains(Field.Value))
                Value = null;

            if (fields.Contains(Field.LoadFlag))
                LoadFlag = null;

            #endregion
        }

        #endregion

        #region Sorting and Merging

        /// <summary>
        /// Get the dictionary key that should be used for a given item and bucketing type
        /// </summary>
        /// <param name="bucketedBy">BucketedBy enum representing what key to get</param>
        /// <param name="lower">True if the key should be lowercased (default), false otherwise</param>
        /// <param name="norename">True if games should only be compared on game and file name, false if system and source are counted</param>
        /// <returns>String representing the key to be used for the DatItem</returns>
        public virtual string GetKey(BucketedBy bucketedBy, bool lower = true, bool norename = true)
        {
            // Set the output key as the default blank string
            string key = string.Empty;

            // Now determine what the key should be based on the bucketedBy value
            switch (bucketedBy)
            {
                case BucketedBy.CRC:
                    key = Constants.CRCZero;
                    break;

                case BucketedBy.Game:
                    key = (norename ? string.Empty
                        : Source.Index.ToString().PadLeft(10, '0')
                            + "-")
                    + (string.IsNullOrWhiteSpace(Machine.Name)
                            ? "Default"
                            : Machine.Name);
                    if (lower)
                        key = key.ToLowerInvariant();

                    if (key == null)
                        key = "null";

                    key = WebUtility.HtmlEncode(key);
                    break;

                case BucketedBy.MD5:
                    key = Constants.MD5Zero;
                    break;

#if NET_FRAMEWORK
                case BucketedBy.RIPEMD160:
                    key = Constants.RIPEMD160Zero;
                    break;
#endif

                case BucketedBy.SHA1:
                    key = Constants.SHA1Zero;
                    break;

                case BucketedBy.SHA256:
                    key = Constants.SHA256Zero;
                    break;

                case BucketedBy.SHA384:
                    key = Constants.SHA384Zero;
                    break;

                case BucketedBy.SHA512:
                    key = Constants.SHA512Zero;
                    break;
            }

            // Double and triple check the key for corner cases
            if (key == null)
                key = string.Empty;

            return key;
        }

        /// <summary>
        /// Replace fields from another item
        /// </summary>
        /// <param name="item">DatItem to pull new information from</param>
        /// <param name="fields">List of Fields representing what should be updated</param>
        public virtual void ReplaceFields(DatItem item, List<Field> fields)
        {
            #region Common

            if (fields.Contains(Field.Name))
                Name = item.Name;

            #endregion

            #region AttractMode

            if (fields.Contains(Field.AltName))
                AltName = item.AltName;

            if (fields.Contains(Field.AltTitle))
                AltTitle = item.AltTitle;

            #endregion

            #region SoftwareList

            if (fields.Contains(Field.PartName))
                PartName = item.PartName;

            if (fields.Contains(Field.PartInterface))
                PartInterface = item.PartInterface;

            if (fields.Contains(Field.Features))
                Features = item.Features;

            if (fields.Contains(Field.AreaName))
                AreaName = item.AreaName;

            if (fields.Contains(Field.AreaSize))
                AreaSize = item.AreaSize;

            if (fields.Contains(Field.AreaWidth))
                AreaWidth = item.AreaWidth;

            if (fields.Contains(Field.AreaEndianness))
                AreaEndianness = item.AreaEndianness;

            if (fields.Contains(Field.Value))
                Value = item.Value;

            if (fields.Contains(Field.LoadFlag))
                LoadFlag = item.LoadFlag;

            #endregion
        }

        #endregion

        #endregion // Instance Methods

        #region Static Methods

        #region Sorting and Merging

        /// <summary>
        /// Determine if two hashes are equal for the purposes of merging
        /// </summary>
        /// <param name="firstHash">First hash to compare</param>
        /// <param name="secondHash">Second hash to compare</param>
        /// <returns>True if either is empty OR the hashes exactly match, false otherwise</returns>
        public static bool ConditionalHashEquals(byte[] firstHash, byte[] secondHash)
        {
            // If either hash is empty, we say they're equal for merging
            if (firstHash.IsNullOrEmpty() || secondHash.IsNullOrEmpty())
                return true;

            // If they're different sizes, they can't match
            if (firstHash.Length != secondHash.Length)
                return false;

            // Otherwise, they need to match exactly
            return Enumerable.SequenceEqual(firstHash, secondHash);
        }

        /// <summary>
        /// Merge an arbitrary set of ROMs based on the supplied information
        /// </summary>
        /// <param name="infiles">List of File objects representing the roms to be merged</param>
        /// <returns>A List of DatItem objects representing the merged roms</returns>
        public static List<DatItem> Merge(List<DatItem> infiles)
        {
            // Check for null or blank roms first
            if (infiles == null || infiles.Count == 0)
                return new List<DatItem>();

            // Create output list
            List<DatItem> outfiles = new List<DatItem>();

            // Then deduplicate them by checking to see if data matches previous saved roms
            int nodumpCount = 0;
            for (int f = 0; f < infiles.Count; f++)
            {
                DatItem file = infiles[f];

                // If we don't have a Rom or a Disk, we skip checking for duplicates
                if (file.ItemType != ItemType.Rom && file.ItemType != ItemType.Disk)
                    continue;

                // If it's a nodump, add and skip
                if (file.ItemType == ItemType.Rom && (file as Rom).ItemStatus == ItemStatus.Nodump)
                {
                    outfiles.Add(file);
                    nodumpCount++;
                    continue;
                }
                else if (file.ItemType == ItemType.Disk && (file as Disk).ItemStatus == ItemStatus.Nodump)
                {
                    outfiles.Add(file);
                    nodumpCount++;
                    continue;
                }
                // If it's the first non-nodump rom in the list, don't touch it
                else if (outfiles.Count == 0 || outfiles.Count == nodumpCount)
                {
                    outfiles.Add(file);
                    continue;
                }

                // Check if the rom is a duplicate
                DupeType dupetype = 0x00;
                DatItem saveditem = new Blank();
                int pos = -1;
                for (int i = 0; i < outfiles.Count; i++)
                {
                    DatItem lastrom = outfiles[i];

                    // Get the duplicate status
                    dupetype = file.GetDuplicateStatus(lastrom);

                    // If it's a duplicate, skip adding it to the output but add any missing information
                    if (dupetype != 0x00)
                    {
                        saveditem = lastrom;
                        pos = i;

                        // Disks and Roms have more information to fill
                        if (file.ItemType == ItemType.Disk)
                            (saveditem as Disk).FillMissingInformation(file as Disk);
                        else if (file.ItemType == ItemType.Rom)
                            (saveditem as Rom).FillMissingInformation(file as Rom);

                        saveditem.DupeType = dupetype;

                        // If the current system has a lower ID than the previous, set the system accordingly
                        if (file.Source.Index < saveditem.Source.Index)
                        {
                            saveditem.Source = file.Source.Clone() as Source;
                            saveditem.CopyMachineInformation(file);
                            saveditem.Name = file.Name;
                        }

                        // If the current machine is a child of the new machine, use the new machine instead
                        if (saveditem.Machine.CloneOf == file.Machine.Name || saveditem.Machine.RomOf == file.Machine.Name)
                        {
                            saveditem.CopyMachineInformation(file);
                            saveditem.Name = file.Name;
                        }

                        break;
                    }
                }

                // If no duplicate is found, add it to the list
                if (dupetype == 0x00)
                {
                    outfiles.Add(file);
                }
                // Otherwise, if a new rom information is found, add that
                else
                {
                    outfiles.RemoveAt(pos);
                    outfiles.Insert(pos, saveditem);
                }
            }

            // Then return the result
            return outfiles;
        }

        /// <summary>
        /// Resolve name duplicates in an arbitrary set of ROMs based on the supplied information
        /// </summary>
        /// <param name="infiles">List of File objects representing the roms to be merged</param>
        /// <returns>A List of DatItem objects representing the renamed roms</returns>
        public static List<DatItem> ResolveNames(List<DatItem> infiles)
        {
            // Create the output list
            List<DatItem> output = new List<DatItem>();

            // First we want to make sure the list is in alphabetical order
            Sort(ref infiles, true);

            // Now we want to loop through and check names
            DatItem lastItem = null;
            string lastrenamed = null;
            int lastid = 0;
            for (int i = 0; i < infiles.Count; i++)
            {
                DatItem datItem = infiles[i];

                // If we have the first item, we automatically add it
                if (lastItem == null)
                {
                    output.Add(datItem);
                    lastItem = datItem;
                    continue;
                }

                // If the current item exactly matches the last item, then we don't add it
                if (datItem.GetDuplicateStatus(lastItem).HasFlag(DupeType.All))
                {
                    Globals.Logger.Verbose($"Exact duplicate found for '{datItem.Name}'");
                    continue;
                }

                // If the current name matches the previous name, rename the current item
                else if (datItem.Name == lastItem.Name)
                {
                    Globals.Logger.Verbose($"Name duplicate found for '{datItem.Name}'");

                    if (datItem.ItemType == ItemType.Disk || datItem.ItemType == ItemType.Rom)
                    {
                        datItem.Name += GetDuplicateSuffix(datItem);
#if NET_FRAMEWORK
                        lastrenamed = lastrenamed ?? datItem.Name;
#else
                        lastrenamed ??= datItem.Name;
#endif
                    }

                    // If we have a conflict with the last renamed item, do the right thing
                    if (datItem.Name == lastrenamed)
                    {
                        lastrenamed = datItem.Name;
                        datItem.Name += (lastid == 0 ? string.Empty : "_" + lastid);
                        lastid++;
                    }
                    // If we have no conflict, then we want to reset the lastrenamed and id
                    else
                    {
                        lastrenamed = null;
                        lastid = 0;
                    }

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
            Sort(ref output, true);

            return output;
        }

        /// <summary>
        /// Get duplicate suffix based on the item type
        /// </summary>
        private static string GetDuplicateSuffix(DatItem datItem)
        {
            if (datItem.ItemType == ItemType.Disk)
                return (datItem as Disk).GetDuplicateSuffix();
            else if (datItem.ItemType == ItemType.Rom)
                return (datItem as Rom).GetDuplicateSuffix();

            return "_1";
        }

        /// <summary>
        /// Sort a list of File objects by SystemID, SourceID, Game, and Name (in order)
        /// </summary>
        /// <param name="roms">List of File objects representing the roms to be sorted</param>
        /// <param name="norename">True if files are not renamed, false otherwise</param>
        /// <returns>True if it sorted correctly, false otherwise</returns>
        public static bool Sort(ref List<DatItem> roms, bool norename)
        {
            roms.Sort(delegate (DatItem x, DatItem y)
            {
                try
                {
                    NaturalComparer nc = new NaturalComparer();
                    if (x.Source.Index == y.Source.Index)
                    {
                        if (x.Machine.Name == y.Machine.Name)
                        {
                            // Special case for comparing a Disk or Rom to another item type
                            if ((x.ItemType == ItemType.Disk || x.ItemType == ItemType.Rom) ^ (y.ItemType == ItemType.Disk || y.ItemType == ItemType.Rom))
                            {
                                if (x.ItemType == ItemType.Disk || x.ItemType == ItemType.Rom)
                                    return -1;
                                else
                                    return 1;
                            }

                            // Otherwise, we compare names naturally
                            else
                            {
                                if (Path.GetDirectoryName(Sanitizer.RemovePathUnsafeCharacters(x.Name)) == Path.GetDirectoryName(Sanitizer.RemovePathUnsafeCharacters(y.Name)))
                                    return nc.Compare(Path.GetFileName(Sanitizer.RemovePathUnsafeCharacters(x.Name)), Path.GetFileName(Sanitizer.RemovePathUnsafeCharacters(y.Name)));

                                return nc.Compare(Path.GetDirectoryName(Sanitizer.RemovePathUnsafeCharacters(x.Name)), Path.GetDirectoryName(Sanitizer.RemovePathUnsafeCharacters(y.Name)));
                            }
                        }

                        return nc.Compare(x.Machine.Name, y.Machine.Name);
                    }

                    return (norename ? nc.Compare(x.Machine.Name, y.Machine.Name) : x.Source.Index - y.Source.Index);
                }
                catch (Exception)
                {
                    // Absorb the error
                    return 0;
                }
            });

            return true;
        }

        #endregion

        #endregion // Static Methods
    }
}
