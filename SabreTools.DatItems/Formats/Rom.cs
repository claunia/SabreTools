using System;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.FileTypes;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents a generic file within a set
    /// </summary>
    [JsonObject("rom"), XmlRoot("rom")]
    public class Rom : DatItem
    {
        #region Fields

        #region Common

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name"), XmlElement("name")]
        public string? Name
        {
            get => _rom.ReadString(Models.Internal.Rom.NameKey);
            set => _rom[Models.Internal.Rom.NameKey] = value;
        }

        /// <summary>
        /// What BIOS is required for this rom
        /// </summary>
        [JsonProperty("bios", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("bios")]
        public string? Bios
        {
            get => _rom.ReadString(Models.Internal.Rom.BiosKey);
            set => _rom[Models.Internal.Rom.BiosKey] = value;
        }

        /// <summary>
        /// Byte size of the rom
        /// </summary>
        [JsonProperty("size", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("size")]
        public long? Size
        {
            get => _rom.ReadLong(Models.Internal.Rom.SizeKey);
            set => _rom[Models.Internal.Rom.SizeKey] = value;
        }

        [JsonIgnore]
        public bool SizeSpecified { get { return Size != null; } }

        /// <summary>
        /// File CRC32 hash
        /// </summary>
        [JsonProperty("crc", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("crc")]
        public string? CRC
        {
            get => _rom.ReadString(Models.Internal.Rom.CRCKey);
            set => _rom[Models.Internal.Rom.CRCKey] = TextHelper.NormalizeCRC32(value);
        }

        /// <summary>
        /// File MD5 hash
        /// </summary>
        [JsonProperty("md5", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("md5")]
        public string? MD5
        {
            get => _rom.ReadString(Models.Internal.Rom.MD5Key);
            set => _rom[Models.Internal.Rom.MD5Key] = TextHelper.NormalizeMD5(value);
        }

        /// <summary>
        /// File SHA-1 hash
        /// </summary>
        [JsonProperty("sha1", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("sha1")]
        public string? SHA1
        {
            get => _rom.ReadString(Models.Internal.Rom.SHA1Key);
            set => _rom[Models.Internal.Rom.SHA1Key] = TextHelper.NormalizeSHA1(value);
        }

        /// <summary>
        /// File SHA-256 hash
        /// </summary>
        [JsonProperty("sha256", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("sha256")]
        public string? SHA256
        {
            get => _rom.ReadString(Models.Internal.Rom.SHA256Key);
            set => _rom[Models.Internal.Rom.SHA256Key] = TextHelper.NormalizeSHA256(value);
        }

        /// <summary>
        /// File SHA-384 hash
        /// </summary>
        [JsonProperty("sha384", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("sha384")]
        public string? SHA384
        {
            get => _rom.ReadString(Models.Internal.Rom.SHA384Key);
            set => _rom[Models.Internal.Rom.SHA384Key] = TextHelper.NormalizeSHA384(value);
        }

        /// <summary>
        /// File SHA-512 hash
        /// </summary>
        [JsonProperty("sha512", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("sha512")]
        public string? SHA512
        {
            get => _rom.ReadString(Models.Internal.Rom.SHA512Key);
            set => _rom[Models.Internal.Rom.SHA512Key] = TextHelper.NormalizeSHA512(value);
        }

        /// <summary>
        /// File SpamSum fuzzy hash
        /// </summary>
        [JsonProperty("spamsum", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("spamsum")]
        public string? SpamSum
        {
            get => _rom.ReadString(Models.Internal.Rom.SpamSumKey);
            set => _rom[Models.Internal.Rom.SpamSumKey] = value;
        }

        /// <summary>
        /// Rom name to merge from parent
        /// </summary>
        [JsonProperty("merge", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("merge")]
        public string? MergeTag
        {
            get => _rom.ReadString(Models.Internal.Rom.MergeKey);
            set => _rom[Models.Internal.Rom.MergeKey] = value;
        }

        /// <summary>
        /// Rom region
        /// </summary>
        [JsonProperty("region", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("biregionos")]
        public string? Region
        {
            get => _rom.ReadString(Models.Internal.Rom.RegionKey);
            set => _rom[Models.Internal.Rom.RegionKey] = value;
        }

        /// <summary>
        /// Data offset within rom
        /// </summary>
        [JsonProperty("offset", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("offset")]
        public string? Offset
        {
            get => _rom.ReadString(Models.Internal.Rom.OffsetKey);
            set => _rom[Models.Internal.Rom.OffsetKey] = value;
        }

        /// <summary>
        /// File created date
        /// </summary>
        [JsonProperty("date", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("date")]
        public string? Date
        {
            get => _rom.ReadString(Models.Internal.Rom.DateKey);
            set => _rom[Models.Internal.Rom.DateKey] = value;
        }

        /// <summary>
        /// Rom dump status
        /// </summary>
        [JsonProperty("status", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ItemStatus ItemStatus
        {
            get => _rom.ReadString(Models.Internal.Rom.StatusKey).AsItemStatus();
            set => _rom[Models.Internal.Rom.StatusKey] = value.FromItemStatus(yesno: false);
        }

        [JsonIgnore]
        public bool ItemStatusSpecified { get { return ItemStatus != ItemStatus.NULL && ItemStatus != ItemStatus.None; } }

        /// <summary>
        /// Determine if the rom is optional in the set
        /// </summary>
        [JsonProperty("optional", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("optional")]
        public bool? Optional
        {
            get => _rom.ReadBool(Models.Internal.Rom.OptionalKey);
            set => _rom[Models.Internal.Rom.OptionalKey] = value;
        }

        [JsonIgnore]
        public bool OptionalSpecified { get { return Optional != null; } }

        /// <summary>
        /// Determine if the CRC32 hash is inverted
        /// </summary>
        [JsonProperty("inverted", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("inverted")]
        public bool? Inverted
        {
            get => _rom.ReadBool(Models.Internal.Rom.InvertedKey);
            set => _rom[Models.Internal.Rom.InvertedKey] = value;
        }

        [JsonIgnore]
        public bool InvertedSpecified { get { return Inverted != null; } }

        #endregion

        #region Archive.org

        /// <summary>
        /// Source of file
        /// </summary>
        [JsonProperty("ado_source", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("ado_source")]
        public string? ArchiveDotOrgSource
        {
            get => _rom.ReadString(Models.Internal.Rom.SourceKey);
            set => _rom[Models.Internal.Rom.SourceKey] = value;
        }

        /// <summary>
        /// Archive.org recognized file format
        /// </summary>
        [JsonProperty("ado_format", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("ado_format")]
        public string? ArchiveDotOrgFormat
        {
            get => _rom.ReadString(Models.Internal.Rom.FormatKey);
            set => _rom[Models.Internal.Rom.FormatKey] = value;
        }

        /// <summary>
        /// Original filename
        /// </summary>
        [JsonProperty("original_filename", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("original_filename")]
        public string? OriginalFilename
        {
            get => _rom.ReadString(Models.Internal.Rom.OriginalKey);
            set => _rom[Models.Internal.Rom.OriginalKey] = value;
        }

        /// <summary>
        /// Image rotation
        /// </summary>
        /// <remarks>
        /// TODO: This might be Int32?
        /// </remarks>
        [JsonProperty("rotation", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("rotation")]
        public string? Rotation
        {
            get => _rom.ReadString(Models.Internal.Rom.RotationKey);
            set => _rom[Models.Internal.Rom.RotationKey] = value;
        }

        /// <summary>
        /// Summation value?
        /// </summary>
        [JsonProperty("summation", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("summation")]
        public string? Summation
        {
            get => _rom.ReadString(Models.Internal.Rom.SummationKey);
            set => _rom[Models.Internal.Rom.SummationKey] = value;
        }

        #endregion

        #region AttractMode

        /// <summary>
        /// Alternate name for the item
        /// </summary>
        [JsonProperty("alt_romname", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("alt_romname")]
        public string? AltName
        {
            get => _rom.ReadString(Models.Internal.Rom.AltRomnameKey);
            set => _rom[Models.Internal.Rom.AltRomnameKey] = value;
        }

        /// <summary>
        /// Alternate title for the item
        /// </summary>
        [JsonProperty("alt_title", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("alt_title")]
        public string? AltTitle
        {
            get => _rom.ReadString(Models.Internal.Rom.AltTitleKey);
            set => _rom[Models.Internal.Rom.AltTitleKey] = value;
        }

        #endregion

        #region Logiqx

        /// <summary>
        /// Alternate title for the item
        /// </summary>
        [JsonProperty("mia", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("mia")]
        public bool? MIA
        {
            get => _rom.ReadBool(Models.Internal.Rom.MIAKey);
            set => _rom[Models.Internal.Rom.MIAKey] = value;
        }

        [JsonIgnore]
        public bool MIASpecified { get { return MIA != null; } }

        #endregion

        #region OpenMSX

        /// <summary>
        /// OpenMSX sub item type
        /// </summary>
        /// <remarks>This is inverted from the internal model</remarks>
        [JsonProperty("original", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("original")]
        public Original? Original { get; set; }

        [JsonIgnore]
        public bool OriginalSpecified { get { return Original != null && Original != default; } }

        /// <summary>
        /// OpenMSX sub item type
        /// </summary>
        [JsonProperty("openmsx_subtype", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("openmsx_subtype")]
        [JsonConverter(typeof(StringEnumConverter))]
        public OpenMSXSubType OpenMSXSubType
        {
            get => _rom.ReadString(Models.Internal.Rom.OpenMSXMediaType).AsOpenMSXSubType();
            set => _rom[Models.Internal.Rom.OpenMSXMediaType] = value.FromOpenMSXSubType();
        }

        [JsonIgnore]
        public bool OpenMSXSubTypeSpecified { get { return OpenMSXSubType != OpenMSXSubType.NULL; } }

        /// <summary>
        /// OpenMSX sub item type
        /// </summary>
        /// <remarks>Not related to the subtype above</remarks>
        [JsonProperty("openmsx_type", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("openmsx_type")]
        public string? OpenMSXType
        {
            get => _rom.ReadString(Models.Internal.Rom.OpenMSXType);
            set => _rom[Models.Internal.Rom.OpenMSXType] = value;
        }

        /// <summary>
        /// Item remark (like a comment)
        /// </summary>
        [JsonProperty("remark", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("remark")]
        public string? Remark
        {
            get => _rom.ReadString(Models.Internal.Rom.RemarkKey);
            set => _rom[Models.Internal.Rom.RemarkKey] = value;
        }

        /// <summary>
        /// Boot state
        /// </summary>
        /// TODO: Investigate where this value came from?
        [JsonProperty("boot", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("boot")]
        public string? Boot { get; set; }

        #endregion

        #region SoftwareList

        /// <summary>
        /// Data area information
        /// </summary>
        /// <remarks>This is inverted from the internal model</remarks>
        [JsonProperty("dataarea", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("dataarea")]
        public DataArea? DataArea { get; set; } = null;

        [JsonIgnore]
        public bool DataAreaSpecified
        {
            get
            {
                return DataArea != null
                    && (!string.IsNullOrEmpty(DataArea.Name)
                        || DataArea.SizeSpecified
                        || DataArea.WidthSpecified
                        || DataArea.EndiannessSpecified);
            }
        }

        /// <summary>
        /// Loading flag
        /// </summary>
        [JsonProperty("loadflag", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("loadflag")]
        [JsonConverter(typeof(StringEnumConverter))]
        public LoadFlag LoadFlag
        {
            get => _rom.ReadString(Models.Internal.Rom.LoadFlagKey).AsLoadFlag();
            set => _rom[Models.Internal.Rom.LoadFlagKey] = value.FromLoadFlag();
        }

        [JsonIgnore]
        public bool LoadFlagSpecified { get { return LoadFlag != LoadFlag.NULL; } }

        /// <summary>
        /// Original hardware part associated with the item
        /// </summary>
        /// <remarks>This is inverted from the internal model</remarks>
        [JsonProperty("part", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("part")]
        public Part? Part { get; set; } = null;

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

        /// <summary>
        /// SoftwareList value associated with the item
        /// </summary>
        [JsonProperty("value", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("value")]
        public string? Value
        {
            get => _rom.ReadString(Models.Internal.Rom.ValueKey);
            set => _rom[Models.Internal.Rom.ValueKey] = value;
        }

        #endregion

        /// <summary>
        /// Internal Rom model
        /// </summary>
        [JsonIgnore]
        private Models.Internal.Rom _rom = new();

        #endregion // Fields

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Rom object
        /// </summary>
        public Rom()
        {
            Name = null;
            ItemType = ItemType.Rom;
            DupeType = 0x00;
            ItemStatus = ItemStatus.None;
        }

        /// <summary>
        /// Create a "blank" Rom object
        /// </summary>
        /// <param name="name"></param>
        /// <param name="machineName"></param>
        /// <param name="omitFromScan"></param>
        public Rom(string name, string machineName)
        {
            Name = name;
            ItemType = ItemType.Rom;
            Size = null;
            ItemStatus = ItemStatus.None;

            Machine = new Machine
            {
                Name = machineName,
                Description = machineName,
            };
        }

        /// <summary>
        /// Create a Rom object from a BaseFile
        /// </summary>
        /// <param name="baseFile"></param>
        public Rom(BaseFile baseFile)
        {
            Name = baseFile.Filename;
            Size = baseFile.Size;
            CRC = TextHelper.ByteArrayToString(baseFile.CRC);
            MD5 = TextHelper.ByteArrayToString(baseFile.MD5);
            SHA1 = TextHelper.ByteArrayToString(baseFile.SHA1);
            SHA256 = TextHelper.ByteArrayToString(baseFile.SHA256);
            SHA384 = TextHelper.ByteArrayToString(baseFile.SHA384);
            SHA512 = TextHelper.ByteArrayToString(baseFile.SHA512);
            SpamSum = System.Text.Encoding.UTF8.GetString(baseFile.SpamSum ?? Array.Empty<byte>());

            ItemType = ItemType.Rom;
            DupeType = 0x00;
            ItemStatus = ItemStatus.None;
            Date = baseFile.Date;
        }

        /// <summary>
        /// Create a Rom object from the internal model
        /// </summary>
        public Rom(Models.Internal.Rom? rom)
        {
            _rom = rom ?? new Models.Internal.Rom();

            ItemType = ItemType.Rom;
            DupeType = 0x00;
            ItemStatus = ItemStatus.None;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Rom()
            {
                Name = this.Name,
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine?.Clone() as Machine,
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _rom = this._rom?.Clone() as Models.Internal.Rom ?? new Models.Internal.Rom(),
            
                DataArea = this.DataArea,
                Part = this.Part,
            };
        }

        /// <summary>
        /// Convert Rom object to a BaseFile
        /// </summary>
        public BaseFile ConvertToBaseFile()
        {
            return new BaseFile()
            {
                Filename = this.Name,
                Parent = this.Machine?.Name,
                Date = this.Date,
                Size = this.Size,
                CRC = TextHelper.StringToByteArray(this.CRC),
                MD5 = TextHelper.StringToByteArray(this.MD5),
                SHA1 = TextHelper.StringToByteArray(this.SHA1),
                SHA256 = TextHelper.StringToByteArray(this.SHA256),
                SHA384 = TextHelper.StringToByteArray(this.SHA384),
                SHA512 = TextHelper.StringToByteArray(this.SHA512),
                SpamSum = System.Text.Encoding.UTF8.GetBytes(this.SpamSum ?? string.Empty),
            };
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If we don't have a Rom, return false
            if (ItemType != other?.ItemType || other is not Rom otherInternal)
                return false;

            // Compare the internal models
            return _rom.EqualTo(otherInternal._rom);
        }

        /// <summary>
        /// Fill any missing size and hash information from another Rom
        /// </summary>
        /// <param name="other">Rom to fill information from</param>
        public void FillMissingInformation(Rom other) => _rom?.FillMissingHashes(other?._rom);

        /// <summary>
        /// Get unique duplicate suffix on name collision
        /// </summary>
        /// <returns>String representing the suffix</returns>
        public string GetDuplicateSuffix()
        {
            if (!string.IsNullOrWhiteSpace(CRC))
                return $"_{CRC}";
            else if (!string.IsNullOrWhiteSpace(MD5))
                return $"_{MD5}";
            else if (!string.IsNullOrWhiteSpace(SHA1))
                return $"_{SHA1}";
            else if (!string.IsNullOrWhiteSpace(SHA256))
                return $"_{SHA256}";
            else if (!string.IsNullOrWhiteSpace(SHA384))
                return $"_{SHA384}";
            else if (!string.IsNullOrWhiteSpace(SHA512))
                return $"_{SHA512}";
            else if (!string.IsNullOrWhiteSpace(SpamSum))
                return $"_{SpamSum}";
            else
                return "_1";
        }

        /// <summary>
        /// Returns if the Rom contains any hashes
        /// </summary>
        /// <returns>True if any hash exists, false otherwise</returns>
        public bool HasHashes() => _rom.HasHashes();

        /// <summary>
        /// Returns if all of the hashes are set to their 0-byte values
        /// </summary>
        /// <returns>True if any hash matches the 0-byte value, false otherwise</returns>
        public bool HasZeroHash() => _rom.HasZeroHash();

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
                case ItemKey.CRC:
                    key = CRC;
                    break;

                case ItemKey.MD5:
                    key = MD5;
                    break;

                case ItemKey.SHA1:
                    key = SHA1;
                    break;

                case ItemKey.SHA256:
                    key = SHA256;
                    break;

                case ItemKey.SHA384:
                    key = SHA384;
                    break;

                case ItemKey.SHA512:
                    key = SHA512;
                    break;

                case ItemKey.SpamSum:
                    key = SpamSum;
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
