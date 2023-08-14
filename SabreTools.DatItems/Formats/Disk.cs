using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.FileTypes;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents Compressed Hunks of Data (CHD) formatted disks which use internal hashes
    /// </summary>
    [JsonObject("disk"), XmlRoot("disk")]
    public class Disk : DatItem
    {
        #region Fields

        #region Common

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name"), XmlElement("name")]
        public string? Name
        {
            get => _disk.ReadString(Models.Internal.Disk.NameKey);
            set => _disk[Models.Internal.Disk.NameKey] = value;
        }

        /// <summary>
        /// Data MD5 hash
        /// </summary>
        [JsonProperty("md5", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("md5")]
        public string? MD5
        {
            get => _disk.ReadString(Models.Internal.Disk.MD5Key);
            set => _disk[Models.Internal.Disk.MD5Key] = TextHelper.NormalizeMD5(value);
        }

        /// <summary>
        /// Data SHA-1 hash
        /// </summary>
        [JsonProperty("sha1", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("sha1")]
        public string? SHA1
        {
            get => _disk.ReadString(Models.Internal.Disk.SHA1Key);
            set => _disk[Models.Internal.Disk.SHA1Key] = TextHelper.NormalizeSHA1(value);
        }

        /// <summary>
        /// Disk name to merge from parent
        /// </summary>
        [JsonProperty("merge", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("merge")]
        public string? MergeTag
        {
            get => _disk.ReadString(Models.Internal.Disk.MergeKey);
            set => _disk[Models.Internal.Disk.MergeKey] = value;
        }

        /// <summary>
        /// Disk region
        /// </summary>
        [JsonProperty("region", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("region")]
        public string? Region
        {
            get => _disk.ReadString(Models.Internal.Disk.RegionKey);
            set => _disk[Models.Internal.Disk.RegionKey] = value;
        }

        /// <summary>
        /// Disk index
        /// </summary>
        [JsonProperty("index", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("index")]
        public string? Index
        {
            get => _disk.ReadString(Models.Internal.Disk.IndexKey);
            set => _disk[Models.Internal.Disk.IndexKey] = value;
        }

        /// <summary>
        /// Disk writable flag
        /// </summary>
        [JsonProperty("writable", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("writable")]
        public bool? Writable
        {
            get => _disk.ReadBool(Models.Internal.Disk.WritableKey);
            set => _disk[Models.Internal.Disk.WritableKey] = value;
        }

        [JsonIgnore]
        public bool WritableSpecified { get { return Writable != null; } }

        /// <summary>
        /// Disk dump status
        /// </summary>
        [JsonProperty("status", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ItemStatus ItemStatus
        {
            get => _disk.ReadString(Models.Internal.Disk.StatusKey).AsItemStatus();
            set => _disk[Models.Internal.Disk.StatusKey] = value.FromItemStatus(yesno: false);
        }

        [JsonIgnore]
        public bool ItemStatusSpecified { get { return ItemStatus != ItemStatus.NULL; } }

        /// <summary>
        /// Determine if the disk is optional in the set
        /// </summary>
        [JsonProperty("optional", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("optional")]
        public bool? Optional
        {
            get => _disk.ReadBool(Models.Internal.Disk.OptionalKey);
            set => _disk[Models.Internal.Disk.OptionalKey] = value;
        }

        [JsonIgnore]
        public bool OptionalSpecified { get { return Optional != null; } }

        #endregion

        #region SoftwareList

        /// <summary>
        /// Disk area information
        /// </summary>
        /// <remarks>This is inverted from the internal model</remarks>
        [JsonProperty("diskarea", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("diskarea")]
        public DiskArea? DiskArea { get; set; }

        [JsonIgnore]
        public bool DiskAreaSpecified
        {
            get
            {
                return DiskArea != null
                    && !string.IsNullOrEmpty(DiskArea.Name);
            }
        }

        /// <summary>
        /// Original hardware part associated with the item
        /// </summary>
        /// <remarks>This is inverted from the internal model</remarks>
        [JsonProperty("part", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("part")]
        public Part? Part { get; set; }

        [JsonIgnore]
        public bool PartSpecified
        {
            get
            {
                return Part != null
                    && (!string.IsNullOrEmpty(Part.Name)
                        || !string.IsNullOrEmpty(Part.Interface));
            }
        }

        #endregion

        /// <summary>
        /// Internal Disk model
        /// </summary>
        [JsonIgnore]
        private Models.Internal.Disk _disk = new();

        #endregion // Fields

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Disk object
        /// </summary>
        public Disk()
        {
            Name = string.Empty;
            ItemType = ItemType.Disk;
            DupeType = 0x00;
            ItemStatus = ItemStatus.None;
        }

        /// <summary>
        /// Create a Disk object from a BaseFile
        /// </summary>
        /// <param name="baseFile"></param>
        public Disk(BaseFile baseFile)
        {
            Name = baseFile.Filename;
            MD5 = TextHelper.ByteArrayToString(baseFile.MD5);
            SHA1 = TextHelper.ByteArrayToString(baseFile.SHA1);

            ItemType = ItemType.Disk;
            DupeType = 0x00;
            ItemStatus = ItemStatus.None;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Disk()
            {
                Name = this.Name,
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine?.Clone() as Machine,
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _disk = this._disk?.Clone() as Models.Internal.Disk ?? new Models.Internal.Disk(),

                DiskArea = this.DiskArea,
                Part = this.Part,
            };
        }

        /// <summary>
        /// Convert Disk object to a BaseFile
        /// </summary>
        public BaseFile ConvertToBaseFile()
        {
            return new BaseFile()
            {
                Filename = this.Name,
                Parent = this.Machine?.Name,
                MD5 = TextHelper.StringToByteArray(this.MD5),
                SHA1 = TextHelper.StringToByteArray(this.SHA1),
            };
        }

        /// <summary>
        /// Convert a disk to the closest Rom approximation
        /// </summary>
        /// <returns></returns>
        public Rom ConvertToRom()
        {
            var rom = new Rom(_disk.ConvertToRom())
            {
                ItemType = ItemType.Rom,
                DupeType = this.DupeType,

                Machine = this.Machine?.Clone() as Machine,
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                DataArea = new DataArea { Name = this.DiskArea?.Name },
                Part = this.Part,
            };

            return rom;
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If we don't have a Disk, return false
            if (ItemType != other?.ItemType || other is not Disk otherInternal)
                return false;

            // Compare the internal models
            return _disk.EqualTo(otherInternal._disk);
        }

        /// <summary>
        /// Fill any missing size and hash information from another Disk
        /// </summary>
        /// <param name="other">Disk to fill information from</param>
        public void FillMissingInformation(Disk other)
        {
            if (string.IsNullOrWhiteSpace(MD5) && !string.IsNullOrWhiteSpace(other.MD5))
                MD5 = other.MD5;

            if (string.IsNullOrWhiteSpace(SHA1) && !string.IsNullOrWhiteSpace(other.SHA1))
                SHA1 = other.SHA1;
        }

        /// <summary>
        /// Get unique duplicate suffix on name collision
        /// </summary>
        /// <returns>String representing the suffix</returns>
        public string GetDuplicateSuffix()
        {
            if (!string.IsNullOrWhiteSpace(MD5))
                return $"_{MD5}";
            else if (!string.IsNullOrWhiteSpace(SHA1))
                return $"_{SHA1}";
            else
                return "_1";
        }

        #endregion

        #region Sorting and Merging

        /// <inheritdoc/>
        public override string GetKey(ItemKey bucketedBy, bool lower = true, bool norename = true)
        {
            // Set the output key as the default blank string
            string? key;

            // Now determine what the key should be based on the bucketedBy value
            switch (bucketedBy)
            {
                case ItemKey.MD5:
                    key = MD5;
                    break;

                case ItemKey.SHA1:
                    key = SHA1;
                    break;

                // Let the base handle generic stuff
                default:
                    return base.GetKey(bucketedBy, lower, norename);
            }

            // Double and triple check the key for corner cases
            key ??= string.Empty;

            return key;
        }

        #endregion
    }
}
