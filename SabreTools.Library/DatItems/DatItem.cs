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
        // TODO: Can internal fields be mapped to Field in a more reasonable way?
        #region Protected instance variables

        [JsonIgnore]
        protected Machine _machine = new Machine();

        #endregion

        #region Publicly facing variables

        #region Standard item information

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

        #region Machine information

        /// <summary>
        /// Name of the machine associated with the item
        /// </summary>
        [JsonIgnore]
        public string MachineName
        {
            get
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                return _machine.Name;
            }
            set
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                _machine.Name = value;
            }
        }

        /// <summary>
        /// Additional notes on the machine
        /// </summary>
        [JsonIgnore]
        public string Comment
        {
            get
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                return _machine.Comment;
            }
            set
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                _machine.Comment = value;
            }
        }

        /// <summary>
        /// Extended description of the machine
        /// </summary>
        [JsonIgnore]
        public string MachineDescription
        {
            get
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                return _machine.Description;
            }
            set
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                _machine.Description = value;
            }
        }

        /// <summary>
        /// Machine year(s) of release/manufacture
        /// </summary>
        [JsonIgnore]
        public string Year
        {
            get
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                return _machine.Year;
            }
            set
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                _machine.Year = value;
            }
        }

        /// <summary>
        /// Machine manufacturer, if available
        /// </summary>
        [JsonIgnore]
        public string Manufacturer
        {
            get
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                return _machine.Manufacturer;
            }
            set
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                _machine.Manufacturer = value;
            }
        }

        /// <summary>
        /// Machine publisher, if available
        /// </summary>
        [JsonIgnore]
        public string Publisher
        {
            get
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                return _machine.Publisher;
            }
            set
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                _machine.Publisher = value;
            }
        }

        /// <summary>
        /// Machine category, if available
        /// </summary>
        [JsonIgnore]
        public string Category
        {
            get
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                return _machine.Category;
            }
            set
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                _machine.Category = value;
            }
        }

        /// <summary>
        /// Machine romof parent
        /// </summary>
        [JsonIgnore]
        public string RomOf
        {
            get
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                return _machine.RomOf;
            }
            set
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                _machine.RomOf = value;
            }
        }

        /// <summary>
        /// Machine cloneof parent
        /// </summary>
        [JsonIgnore]
        public string CloneOf
        {
            get
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                return _machine.CloneOf;
            }
            set
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                _machine.CloneOf = value;
            }
        }

        /// <summary>
        /// Machine sampleof parent
        /// </summary>
        [JsonIgnore]
        public string SampleOf
        {
            get
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                return _machine.SampleOf;
            }
            set
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                _machine.SampleOf = value;
            }
        }

        /// <summary>
        /// Machine support status
        /// </summary>
        /// <remarks>yes = true, partial = null, no = false</remarks>
        [JsonIgnore]
        public bool? Supported
        {
            get
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                return _machine.Supported;
            }
            set
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                _machine.Supported = value;
            }
        }

        /// <summary>
        /// Emulator source file related to the machine
        /// </summary>
        [JsonIgnore]
        public string SourceFile
        {
            get
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                return _machine.SourceFile;
            }
            set
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                _machine.SourceFile = value;
            }
        }

        /// <summary>
        /// Machine runnable status
        /// </summary>
        /// <remarks>yes = true, partial = null, no = false</remarks>
        [JsonIgnore]
        public bool? Runnable
        {
            get
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                return _machine.Runnable;
            }
            set
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                _machine.Runnable = value;
            }
        }

        /// <summary>
        /// Machine board name
        /// </summary>
        [JsonIgnore]
        public string Board
        {
            get
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                return _machine.Board;
            }
            set
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                _machine.Board = value;
            }
        }

        /// <summary>
        /// Rebuild location if different than machine name
        /// </summary>
        [JsonIgnore]
        public string RebuildTo
        {
            get
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                return _machine.RebuildTo;
            }
            set
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                _machine.RebuildTo = value;
            }
        }

        /// <summary>
        /// List of associated device names
        /// </summary>
        [JsonIgnore]
        public List<string> Devices
        {
            get
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                return _machine.Devices;
            }
            set
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                _machine.Devices = value;
            }
        }

        /// <summary>
        /// List of slot options
        /// </summary>
        [JsonIgnore]
        public List<string> SlotOptions
        {
            get
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                return _machine.SlotOptions;
            }
            set
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                _machine.SlotOptions = value;
            }
        }

        /// <summary>
        /// List of info items
        /// </summary>
        [JsonIgnore]
        public List<KeyValuePair<string, string>> Infos
        {
            get
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                return _machine.Infos;
            }
            set
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                _machine.Infos = value;
            }
        }

        /// <summary>
        /// Type of the associated machine
        /// </summary>
        [JsonIgnore]
        public MachineType MachineType
        {
            get
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                return _machine.MachineType;
            }
            set
            {
                if (_machine == null)
                {
                    _machine = new Machine();
                }

                _machine.MachineType = value;
            }
        }

        #endregion

        #region Software list information

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

        #endregion

        #region Source metadata information

        /// <summary>
        /// Internal DatFile index for organization
        /// </summary>
        [JsonIgnore]
        public int IndexId { get; set; }

        /// <summary>
        /// Internal DatFile name for organization
        /// </summary>
        [JsonIgnore]
        public string IndexSource { get; set; }

        /// <summary>
        /// Flag if item should be removed
        /// </summary>
        [JsonIgnore]
        public bool Remove { get; set; }

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

            string fieldValue = null;
            switch (field)
            {
                case Field.Name:
                    fieldValue = Name;
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

                case Field.MachineName:
                    fieldValue = MachineName;
                    break;
                case Field.Comment:
                    fieldValue = Comment;
                    break;
                case Field.Description:
                    fieldValue = MachineDescription;
                    break;
                case Field.Year:
                    fieldValue = Year;
                    break;
                case Field.Manufacturer:
                    fieldValue = Manufacturer;
                    break;
                case Field.Publisher:
                    fieldValue = Publisher;
                    break;
                case Field.Category:
                    fieldValue = Category;
                    break;
                case Field.RomOf:
                    fieldValue = RomOf;
                    break;
                case Field.CloneOf:
                    fieldValue = CloneOf;
                    break;
                case Field.SampleOf:
                    fieldValue = SampleOf;
                    break;
                case Field.Supported:
                    fieldValue = Supported?.ToString();
                    break;
                case Field.SourceFile:
                    fieldValue = SourceFile;
                    break;
                case Field.Runnable:
                    fieldValue = Runnable?.ToString();
                    break;
                case Field.Board:
                    fieldValue = Board;
                    break;
                case Field.RebuildTo:
                    fieldValue = RebuildTo;
                    break;
                case Field.Devices:
                    fieldValue = string.Join(";", Devices ?? new List<string>());
                    break;
                case Field.SlotOptions:
                    fieldValue = string.Join(";", SlotOptions ?? new List<string>());
                    break;
                case Field.Infos:
                    fieldValue = string.Join(";", (Infos ?? new List<KeyValuePair<string, string>>()).Select(i => $"{i.Key}={i.Value}"));
                    break;
                case Field.MachineType:
                    fieldValue = MachineType.ToString();
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
            _machine = (Machine)item._machine.Clone();
        }

        /// <summary>
        /// Copy all machine information over in one shot
        /// </summary>
        /// <param name="machine">Existing machine to copy information from</param>
        public void CopyMachineInformation(Machine machine)
        {
            _machine = (Machine)machine.Clone();
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
            if (lastItem.DupeType.HasFlag(DupeType.External) || lastItem.IndexId != IndexId)
            {
                if (lastItem.MachineName == MachineName && lastItem.Name == Name)
                    output = DupeType.External | DupeType.All;
                else
                    output = DupeType.External | DupeType.Hash;
            }

            // Otherwise, it's considered an internal dupe
            else
            {
                if (lastItem.MachineName == MachineName && lastItem.Name == Name)
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
            #region Machine Filters

            // Filter on machine name
            bool? machineNameFound = filter.MachineName.MatchesPositiveSet(MachineName);
            if (filter.IncludeOfInGame)
            {
                machineNameFound |= (filter.MachineName.MatchesPositiveSet(CloneOf) == true);
                machineNameFound |= (filter.MachineName.MatchesPositiveSet(RomOf) == true);
            }
            if (machineNameFound == false)
                return false;

            machineNameFound = filter.MachineName.MatchesNegativeSet(MachineName);
            if (filter.IncludeOfInGame)
            {
                machineNameFound |= (filter.MachineName.MatchesNegativeSet(CloneOf) == true);
                machineNameFound |= (filter.MachineName.MatchesNegativeSet(RomOf) == true);
            }
            if (machineNameFound == false)
                return false;

            // Filter on comment
            if (filter.Comment.MatchesPositiveSet(Comment) == false)
                return false;
            if (filter.Comment.MatchesNegativeSet(Comment) == true)
                return false;

            // Filter on machine description
            if (filter.MachineDescription.MatchesPositiveSet(MachineDescription) == false)
                return false;
            if (filter.MachineDescription.MatchesNegativeSet(MachineDescription) == true)
                return false;

            // Filter on year
            if (filter.Year.MatchesPositiveSet(Year) == false)
                return false;
            if (filter.Year.MatchesNegativeSet(Year) == true)
                return false;

            // Filter on manufacturer
            if (filter.Manufacturer.MatchesPositiveSet(Manufacturer) == false)
                return false;
            if (filter.Manufacturer.MatchesNegativeSet(Manufacturer) == true)
                return false;

            // Filter on publisher
            if (filter.Publisher.MatchesPositiveSet(Publisher) == false)
                return false;
            if (filter.Publisher.MatchesNegativeSet(Publisher) == true)
                return false;

            // Filter on category
            if (filter.Category.MatchesPositiveSet(Category) == false)
                return false;
            if (filter.Category.MatchesNegativeSet(Category) == true)
                return false;

            // Filter on romof
            if (filter.RomOf.MatchesPositiveSet(RomOf) == false)
                return false;
            if (filter.RomOf.MatchesNegativeSet(RomOf) == true)
                return false;

            // Filter on cloneof
            if (filter.CloneOf.MatchesPositiveSet(CloneOf) == false)
                return false;
            if (filter.CloneOf.MatchesNegativeSet(CloneOf) == true)
                return false;

            // Filter on sampleof
            if (filter.SampleOf.MatchesPositiveSet(SampleOf) == false)
                return false;
            if (filter.SampleOf.MatchesNegativeSet(SampleOf) == true)
                return false;

            // Filter on supported
            if (filter.Supported.MatchesNeutral(null, Supported) == false)
                return false;

            // Filter on source file
            if (filter.SourceFile.MatchesPositiveSet(SourceFile) == false)
                return false;
            if (filter.SourceFile.MatchesNegativeSet(SourceFile) == true)
                return false;

            // Filter on runnable
            if (filter.Runnable.MatchesNeutral(null, Runnable) == false)
                return false;

            // Filter on board
            if (filter.Board.MatchesPositiveSet(Board) == false)
                return false;
            if (filter.Board.MatchesNegativeSet(Board) == true)
                return false;

            // Filter on rebuildto
            if (filter.RebuildTo.MatchesPositiveSet(RebuildTo) == false)
                return false;
            if (filter.RebuildTo.MatchesNegativeSet(RebuildTo) == true)
                return false;

            // Filter on devices
            if (Devices != null && Devices.Any())
            {
                bool anyPositiveDevice = false;
                bool anyNegativeDevice = false;
                foreach (string device in Devices)
                {
                    anyPositiveDevice |= filter.Devices.MatchesPositiveSet(device) != false;
                    anyNegativeDevice |= filter.Devices.MatchesNegativeSet(device) == false;
                }

                if (!anyPositiveDevice || anyNegativeDevice)
                    return false;
            }

            // Filter on slot options
            if (SlotOptions != null && SlotOptions.Any())
            {
                bool anyPositiveSlotOption = false;
                bool anyNegativeSlotOption = false;
                foreach (string slotOption in SlotOptions)
                {
                    anyPositiveSlotOption |= filter.SlotOptions.MatchesPositiveSet(slotOption) != false;
                    anyNegativeSlotOption |= filter.SlotOptions.MatchesNegativeSet(slotOption) == false;
                }

                if (!anyPositiveSlotOption || anyNegativeSlotOption)
                    return false;
            }

            // Filter on machine type
            if (filter.MachineTypes.MatchesPositive(MachineType.NULL, MachineType) == false)
                return false;
            if (filter.MachineTypes.MatchesNegative(MachineType.NULL, MachineType) == true)
                return false;

            #endregion

            #region DatItem Filters

            // Filter on item type
            if (filter.ItemTypes.MatchesPositiveSet(ItemType.ToString()) == false)
                return false;
            if (filter.ItemTypes.MatchesNegativeSet(ItemType.ToString()) == true)
                return false;

            // Filter on item name
            if (filter.ItemName.MatchesPositiveSet(Name) == false)
                return false;
            if (filter.ItemName.MatchesNegativeSet(Name) == true)
                return false;

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

            #endregion

            return true;
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
                        : IndexId.ToString().PadLeft(10, '0')
                            + "-")
                    + (string.IsNullOrWhiteSpace(MachineName)
                            ? "Default"
                            : MachineName);
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
        /// <param name="updateFields">List of Fields representing what should be updated</param>
        public virtual void ReplaceFields(DatItem item, List<Field> updateFields)
        {
            if (updateFields.Contains(Field.Name))
                Name = item.Name;

            if (updateFields.Contains(Field.PartName))
                PartName = item.PartName;

            if (updateFields.Contains(Field.PartInterface))
                PartInterface = item.PartInterface;

            if (updateFields.Contains(Field.Features))
                Features = item.Features;

            if (updateFields.Contains(Field.AreaName))
                AreaName = item.AreaName;

            if (updateFields.Contains(Field.AreaSize))
                AreaSize = item.AreaSize;
        }

        /// <summary>
        /// Replace machine fields from another item
        /// </summary>
        /// <param name="item">DatItem to pull new information from</param>
        /// <param name="updateFields">List of Fields representing what should be updated</param>
        /// <param name="onlySame">True if descriptions should only be replaced if the game name is the same, false otherwise</param>
        public void ReplaceMachineFields(DatItem item, List<Field> updateFields, bool onlySame)
        {
            if (updateFields.Contains(Field.MachineName))
                MachineName = item.MachineName;

            if (updateFields.Contains(Field.Comment))
                Comment = item.Comment;

            if (updateFields.Contains(Field.Description))
            {
                if (!onlySame || (onlySame && MachineName == MachineDescription))
                    MachineDescription = item.MachineDescription;
            }

            if (updateFields.Contains(Field.Year))
                Year = item.Year;

            if (updateFields.Contains(Field.Manufacturer))
                Manufacturer = item.Manufacturer;

            if (updateFields.Contains(Field.Publisher))
                Publisher = item.Publisher;

            if (updateFields.Contains(Field.Category))
                Category = item.Category;

            if (updateFields.Contains(Field.RomOf))
                RomOf = item.RomOf;

            if (updateFields.Contains(Field.CloneOf))
                CloneOf = item.CloneOf;

            if (updateFields.Contains(Field.SampleOf))
                SampleOf = item.SampleOf;

            if (updateFields.Contains(Field.Supported))
                Supported = item.Supported;

            if (updateFields.Contains(Field.SourceFile))
                SourceFile = item.SourceFile;

            if (updateFields.Contains(Field.Runnable))
                Runnable = item.Runnable;

            if (updateFields.Contains(Field.Board))
                Board = item.Board;

            if (updateFields.Contains(Field.RebuildTo))
                RebuildTo = item.RebuildTo;

            if (updateFields.Contains(Field.Devices))
                Devices = item.Devices;

            if (updateFields.Contains(Field.SlotOptions))
                SlotOptions = item.SlotOptions;

            if (updateFields.Contains(Field.Infos))
                Infos = item.Infos;

            if (updateFields.Contains(Field.MachineType))
                MachineType = item.MachineType;
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
                        if (file.IndexId < saveditem.IndexId)
                        {
                            saveditem.IndexId = file.IndexId;
                            saveditem.IndexSource = file.IndexSource;
                            saveditem.CopyMachineInformation(file);
                            saveditem.Name = file.Name;
                        }

                        // If the current machine is a child of the new machine, use the new machine instead
                        if (saveditem.CloneOf == file.MachineName || saveditem.RomOf == file.MachineName)
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
                    if (x.IndexId == y.IndexId)
                    {
                        if (x.MachineName == y.MachineName)
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

                        return nc.Compare(x.MachineName, y.MachineName);
                    }

                    return (norename ? nc.Compare(x.MachineName, y.MachineName) : x.IndexId - y.IndexId);
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
