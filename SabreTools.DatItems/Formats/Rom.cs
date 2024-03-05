using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.FileTypes;
using SabreTools.Filter;

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
            get => _internal.ReadString(Models.Metadata.Rom.NameKey);
            set => _internal[Models.Metadata.Rom.NameKey] = value;
        }

        /// <summary>
        /// What BIOS is required for this rom
        /// </summary>
        [JsonProperty("bios", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("bios")]
        public string? Bios
        {
            get => _internal.ReadString(Models.Metadata.Rom.BiosKey);
            set => _internal[Models.Metadata.Rom.BiosKey] = value;
        }

        /// <summary>
        /// Byte size of the rom
        /// </summary>
        [JsonProperty("size", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("size")]
        public long? Size
        {
            get => _internal.ReadLong(Models.Metadata.Rom.SizeKey);
            set => _internal[Models.Metadata.Rom.SizeKey] = value;
        }

        [JsonIgnore]
        public bool SizeSpecified { get { return Size != null; } }

        /// <summary>
        /// File CRC32 hash
        /// </summary>
        [JsonProperty("crc", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("crc")]
        public string? CRC
        {
            get => _internal.ReadString(Models.Metadata.Rom.CRCKey);
            set => _internal[Models.Metadata.Rom.CRCKey] = TextHelper.NormalizeCRC32(value);
        }

        /// <summary>
        /// File MD5 hash
        /// </summary>
        [JsonProperty("md5", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("md5")]
        public string? MD5
        {
            get => _internal.ReadString(Models.Metadata.Rom.MD5Key);
            set => _internal[Models.Metadata.Rom.MD5Key] = TextHelper.NormalizeMD5(value);
        }

        /// <summary>
        /// File SHA-1 hash
        /// </summary>
        [JsonProperty("sha1", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("sha1")]
        public string? SHA1
        {
            get => _internal.ReadString(Models.Metadata.Rom.SHA1Key);
            set => _internal[Models.Metadata.Rom.SHA1Key] = TextHelper.NormalizeSHA1(value);
        }

        /// <summary>
        /// File SHA-256 hash
        /// </summary>
        [JsonProperty("sha256", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("sha256")]
        public string? SHA256
        {
            get => _internal.ReadString(Models.Metadata.Rom.SHA256Key);
            set => _internal[Models.Metadata.Rom.SHA256Key] = TextHelper.NormalizeSHA256(value);
        }

        /// <summary>
        /// File SHA-384 hash
        /// </summary>
        [JsonProperty("sha384", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("sha384")]
        public string? SHA384
        {
            get => _internal.ReadString(Models.Metadata.Rom.SHA384Key);
            set => _internal[Models.Metadata.Rom.SHA384Key] = TextHelper.NormalizeSHA384(value);
        }

        /// <summary>
        /// File SHA-512 hash
        /// </summary>
        [JsonProperty("sha512", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("sha512")]
        public string? SHA512
        {
            get => _internal.ReadString(Models.Metadata.Rom.SHA512Key);
            set => _internal[Models.Metadata.Rom.SHA512Key] = TextHelper.NormalizeSHA512(value);
        }

        /// <summary>
        /// File SpamSum fuzzy hash
        /// </summary>
        [JsonProperty("spamsum", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("spamsum")]
        public string? SpamSum
        {
            get => _internal.ReadString(Models.Metadata.Rom.SpamSumKey);
            set => _internal[Models.Metadata.Rom.SpamSumKey] = value;
        }

        /// <summary>
        /// Rom name to merge from parent
        /// </summary>
        [JsonProperty("merge", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("merge")]
        public string? MergeTag
        {
            get => _internal.ReadString(Models.Metadata.Rom.MergeKey);
            set => _internal[Models.Metadata.Rom.MergeKey] = value;
        }

        /// <summary>
        /// Rom region
        /// </summary>
        [JsonProperty("region", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("biregionos")]
        public string? Region
        {
            get => _internal.ReadString(Models.Metadata.Rom.RegionKey);
            set => _internal[Models.Metadata.Rom.RegionKey] = value;
        }

        /// <summary>
        /// Data offset within rom
        /// </summary>
        [JsonProperty("offset", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("offset")]
        public string? Offset
        {
            get => _internal.ReadString(Models.Metadata.Rom.OffsetKey);
            set => _internal[Models.Metadata.Rom.OffsetKey] = value;
        }

        /// <summary>
        /// File created date
        /// </summary>
        [JsonProperty("date", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("date")]
        public string? Date
        {
            get => _internal.ReadString(Models.Metadata.Rom.DateKey);
            set => _internal[Models.Metadata.Rom.DateKey] = value;
        }

        /// <summary>
        /// Rom dump status
        /// </summary>
        [JsonProperty("status", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ItemStatus ItemStatus
        {
            get => _internal.ReadString(Models.Metadata.Rom.StatusKey).AsEnumValue<ItemStatus>();
            set => _internal[Models.Metadata.Rom.StatusKey] = value.AsStringValue<ItemStatus>(useSecond: false);
        }

        [JsonIgnore]
        public bool ItemStatusSpecified { get { return ItemStatus != ItemStatus.NULL && ItemStatus != ItemStatus.None; } }

        /// <summary>
        /// Determine if the rom is optional in the set
        /// </summary>
        [JsonProperty("optional", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("optional")]
        public bool? Optional
        {
            get => _internal.ReadBool(Models.Metadata.Rom.OptionalKey);
            set => _internal[Models.Metadata.Rom.OptionalKey] = value;
        }

        [JsonIgnore]
        public bool OptionalSpecified { get { return Optional != null; } }

        /// <summary>
        /// Determine if the CRC32 hash is inverted
        /// </summary>
        [JsonProperty("inverted", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("inverted")]
        public bool? Inverted
        {
            get => _internal.ReadBool(Models.Metadata.Rom.InvertedKey);
            set => _internal[Models.Metadata.Rom.InvertedKey] = value;
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
            get => _internal.ReadString(Models.Metadata.Rom.SourceKey);
            set => _internal[Models.Metadata.Rom.SourceKey] = value;
        }

        /// <summary>
        /// Archive.org recognized file format
        /// </summary>
        [JsonProperty("ado_format", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("ado_format")]
        public string? ArchiveDotOrgFormat
        {
            get => _internal.ReadString(Models.Metadata.Rom.FormatKey);
            set => _internal[Models.Metadata.Rom.FormatKey] = value;
        }

        /// <summary>
        /// Original filename
        /// </summary>
        [JsonProperty("original_filename", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("original_filename")]
        public string? OriginalFilename
        {
            get => _internal.ReadString(Models.Metadata.Rom.OriginalKey);
            set => _internal[Models.Metadata.Rom.OriginalKey] = value;
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
            get => _internal.ReadString(Models.Metadata.Rom.RotationKey);
            set => _internal[Models.Metadata.Rom.RotationKey] = value;
        }

        /// <summary>
        /// Summation value?
        /// </summary>
        [JsonProperty("summation", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("summation")]
        public string? Summation
        {
            get => _internal.ReadString(Models.Metadata.Rom.SummationKey);
            set => _internal[Models.Metadata.Rom.SummationKey] = value;
        }

        #endregion

        #region AttractMode

        /// <summary>
        /// Alternate name for the item
        /// </summary>
        [JsonProperty("alt_internalname", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("alt_internalname")]
        public string? AltName
        {
            get => _internal.ReadString(Models.Metadata.Rom.AltRomnameKey);
            set => _internal[Models.Metadata.Rom.AltRomnameKey] = value;
        }

        /// <summary>
        /// Alternate title for the item
        /// </summary>
        [JsonProperty("alt_title", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("alt_title")]
        public string? AltTitle
        {
            get => _internal.ReadString(Models.Metadata.Rom.AltTitleKey);
            set => _internal[Models.Metadata.Rom.AltTitleKey] = value;
        }

        #endregion

        #region Logiqx

        /// <summary>
        /// Alternate title for the item
        /// </summary>
        [JsonProperty("mia", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("mia")]
        public bool? MIA
        {
            get => _internal.ReadBool(Models.Metadata.Rom.MIAKey);
            set => _internal[Models.Metadata.Rom.MIAKey] = value;
        }

        [JsonIgnore]
        public bool MIASpecified { get { return MIA != null; } }

        #endregion

        #region OpenMSX

        /// <summary>
        /// OpenMSX sub item type
        /// </summary>
        /// <remarks>Hack on top of internal model</remarks>
        [JsonProperty("original", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("original")]
        public Original? Original
        {
            get => _internal.Read<Original>("ORIGINAL");
            set => _internal["ORIGINAL"] = value;
        }

        [JsonIgnore]
        public bool OriginalSpecified { get { return Original != null && Original != default; } }

        /// <summary>
        /// OpenMSX sub item type
        /// </summary>
        [JsonProperty("openmsx_subtype", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("openmsx_subtype")]
        [JsonConverter(typeof(StringEnumConverter))]
        public OpenMSXSubType OpenMSXSubType
        {
            get => _internal.ReadString(Models.Metadata.Rom.OpenMSXMediaType).AsEnumValue<OpenMSXSubType>();
            set => _internal[Models.Metadata.Rom.OpenMSXMediaType] = value.AsStringValue<OpenMSXSubType>();
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
            get => _internal.ReadString(Models.Metadata.Rom.OpenMSXType);
            set => _internal[Models.Metadata.Rom.OpenMSXType] = value;
        }

        /// <summary>
        /// Item remark (like a comment)
        /// </summary>
        [JsonProperty("remark", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("remark")]
        public string? Remark
        {
            get => _internal.ReadString(Models.Metadata.Rom.RemarkKey);
            set => _internal[Models.Metadata.Rom.RemarkKey] = value;
        }

        /// <summary>
        /// Boot state
        /// </summary>
        /// TODO: Investigate where this value came from?
        [JsonProperty("boot", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("boot")]
        public string? Boot
        {
            get => _internal.ReadString("BOOT");
            set => _internal["BOOT"] = value;
        }

        #endregion

        #region SoftwareList

        /// <summary>
        /// Data area information
        /// </summary>
        /// <remarks>Hack on top of internal model</remarks>
        [JsonProperty("dataarea", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("dataarea")]
        public DataArea? DataArea
        {
            get => _internal.Read<DataArea>("DATAAREA");
            set => _internal["DATAAREA"] = value;
        }

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
            get => _internal.ReadString(Models.Metadata.Rom.LoadFlagKey).AsEnumValue<LoadFlag>();
            set => _internal[Models.Metadata.Rom.LoadFlagKey] = value.AsStringValue<LoadFlag>();
        }

        [JsonIgnore]
        public bool LoadFlagSpecified { get { return LoadFlag != LoadFlag.NULL; } }

        /// <summary>
        /// Original hardware part associated with the item
        /// </summary>
        /// <remarks>Hack on top of internal model</remarks>
        [JsonProperty("part", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("part")]
        public Part? Part
        {
            get => _internal.Read<Part>("PART");
            set => _internal["PART"] = value;
        }

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
            get => _internal.ReadString(Models.Metadata.Rom.ValueKey);
            set => _internal[Models.Metadata.Rom.ValueKey] = value;
        }

        #endregion

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
            _internal = new Models.Metadata.Rom();
            Machine = new Machine();

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
            _internal = new Models.Metadata.Rom();
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
            _internal = new Models.Metadata.Rom();
            Machine = new Machine();

            Name = baseFile.Filename;
            Size = baseFile.Size;
            CRC = TextHelper.ByteArrayToString(baseFile.CRC);
            MD5 = TextHelper.ByteArrayToString(baseFile.MD5);
            SHA1 = TextHelper.ByteArrayToString(baseFile.SHA1);
            SHA256 = TextHelper.ByteArrayToString(baseFile.SHA256);
            SHA384 = TextHelper.ByteArrayToString(baseFile.SHA384);
            SHA512 = TextHelper.ByteArrayToString(baseFile.SHA512);
            SpamSum = System.Text.Encoding.UTF8.GetString(baseFile.SpamSum ?? []);

            ItemType = ItemType.Rom;
            DupeType = 0x00;
            ItemStatus = ItemStatus.None;
            Date = baseFile.Date;
        }

        /// <summary>
        /// Create a Rom object from the internal model
        /// </summary>
        public Rom(Models.Metadata.Rom? rom)
        {
            _internal = rom ?? [];

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

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.Rom ?? [],
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
                Parent = this.Machine.Name,
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

        /// <summary>
        /// Fill any missing size and hash information from another Rom
        /// </summary>
        /// <param name="other">Rom to fill information from</param>
        public void FillMissingInformation(Rom other) => _internal.FillMissingHashes(other?._internal);

        /// <summary>
        /// Get unique duplicate suffix on name collision
        /// </summary>
        /// <returns>String representing the suffix</returns>
        public string GetDuplicateSuffix() => _internal.GetDuplicateSuffix();

        /// <summary>
        /// Returns if the Rom contains any hashes
        /// </summary>
        /// <returns>True if any hash exists, false otherwise</returns>
        public bool HasHashes() => _internal.HasHashes();

        /// <summary>
        /// Returns if all of the hashes are set to their 0-byte values
        /// </summary>
        /// <returns>True if any hash matches the 0-byte value, false otherwise</returns>
        public bool HasZeroHash() => _internal.HasZeroHash();

        #endregion

        #region Manipulation

        /// <inheritdoc/>
        public override bool SetField(DatItemField datItemField, string value)
        {
            // Get the correct internal field name
            string? fieldName = datItemField switch
            {
                DatItemField.AltName => Models.Metadata.Rom.AltRomnameKey,
                DatItemField.AltTitle => Models.Metadata.Rom.AltTitleKey,
                DatItemField.ArchiveDotOrgFormat => Models.Metadata.Rom.FormatKey,
                DatItemField.ArchiveDotOrgSource => Models.Metadata.Rom.SourceKey,
                DatItemField.Bios => Models.Metadata.Rom.BiosKey,
                //DatItemField.Boot => Models.Metadata.Rom.BootKey,
                DatItemField.CRC => Models.Metadata.Rom.CRCKey,
                DatItemField.Date => Models.Metadata.Rom.DateKey,
                DatItemField.Inverted => Models.Metadata.Rom.InvertedKey,
                DatItemField.LoadFlag => Models.Metadata.Rom.LoadFlagKey,
                DatItemField.MD5 => Models.Metadata.Rom.MD5Key,
                DatItemField.Merge => Models.Metadata.Rom.MergeKey,
                DatItemField.MIA => Models.Metadata.Rom.MIAKey,
                DatItemField.Offset => Models.Metadata.Rom.OffsetKey,
                DatItemField.OpenMSXSubType => Models.Metadata.Rom.OpenMSXMediaType, // TODO: Fix with Key suffix
                DatItemField.OpenMSXType => Models.Metadata.Rom.OpenMSXType, // TODO: Fix with Key suffix
                DatItemField.Optional => Models.Metadata.Rom.OptionalKey,
                //DatItemField.Original => Models.Metadata.Rom.OriginalKey,
                DatItemField.OriginalFilename => Models.Metadata.Rom.OriginalKey,
                DatItemField.Region => Models.Metadata.Rom.RegionKey,
                DatItemField.Remark => Models.Metadata.Rom.RemarkKey,
                DatItemField.Rotation => Models.Metadata.Rom.RotationKey,
                DatItemField.SHA1 => Models.Metadata.Rom.SHA1Key,
                DatItemField.SHA256 => Models.Metadata.Rom.SHA256Key,
                DatItemField.SHA384 => Models.Metadata.Rom.SHA384Key,
                DatItemField.SHA512 => Models.Metadata.Rom.SHA512Key,
                DatItemField.Size => Models.Metadata.Rom.SizeKey,
                DatItemField.SpamSum => Models.Metadata.Rom.SpamSumKey,
                DatItemField.Status => Models.Metadata.Rom.StatusKey,
                DatItemField.Summation => Models.Metadata.Rom.SummationKey,
                DatItemField.Value => Models.Metadata.Rom.ValueKey,
                _ => null,
            };

            // A null value means special handling is needed
            if (fieldName == null)
            {
                switch (datItemField)
                {
                    case DatItemField.Boot: Boot = value; return true;
                    case DatItemField.Original: Original = new Original { Content = value }; return true;
                }
            }

            // Set the field and return
            return FieldManipulator.SetField(_internal, fieldName, value);
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
