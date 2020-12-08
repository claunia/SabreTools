using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using SabreTools.Core;
using SabreTools.Filtering;
using SabreTools.Library.FileTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Represents a generic file within a set
    /// </summary>
    [JsonObject("rom"), XmlRoot("rom")]
    public class Rom : DatItem
    {
        #region Private instance variables

        private byte[] _crc; // 8 bytes
        private byte[] _md5; // 16 bytes
#if NET_FRAMEWORK
        private byte[] _ripemd160; // 20 bytes
#endif
        private byte[] _sha1; // 20 bytes
        private byte[] _sha256; // 32 bytes
        private byte[] _sha384; // 48 bytes
        private byte[] _sha512; // 64 bytes
        private byte[] _spamsum; // variable bytes

        #endregion

        #region Fields

        #region Common

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name")]
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// What BIOS is required for this rom
        /// </summary>
        [JsonProperty("bios", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("bios")]
        public string Bios { get; set; }

        /// <summary>
        /// Byte size of the rom
        /// </summary>
        [JsonProperty("size", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("size")]
        public long? Size { get; set; } = null;

        [JsonIgnore]
        public bool SizeSpecified { get { return Size != null; } }

        /// <summary>
        /// File CRC32 hash
        /// </summary>
        [JsonProperty("crc", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("crc")]
        public string CRC
        {
            get { return _crc.IsNullOrEmpty() ? null : Utilities.ByteArrayToString(_crc); }
            set { _crc = (value == "null" ? Constants.CRCZeroBytes : Utilities.StringToByteArray(Sanitizer.CleanCRC32(value))); }
        }

        /// <summary>
        /// File MD5 hash
        /// </summary>
        [JsonProperty("md5", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("md5")]
        public string MD5
        {
            get { return _md5.IsNullOrEmpty() ? null : Utilities.ByteArrayToString(_md5); }
            set { _md5 = Utilities.StringToByteArray(Sanitizer.CleanMD5(value)); }
        }

#if NET_FRAMEWORK
        /// <summary>
        /// File RIPEMD160 hash
        /// </summary>
        [JsonProperty("ripemd160", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("ripemd160")]
        public string RIPEMD160
        {
            get { return _ripemd160.IsNullOrEmpty() ? null : Utilities.ByteArrayToString(_ripemd160); }
            set { _ripemd160 = Utilities.StringToByteArray(Sanitizer.CleanRIPEMD160(value)); }
        }
#endif

        /// <summary>
        /// File SHA-1 hash
        /// </summary>
        [JsonProperty("sha1", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("sha1")]
        public string SHA1
        {
            get { return _sha1.IsNullOrEmpty() ? null : Utilities.ByteArrayToString(_sha1); }
            set { _sha1 = Utilities.StringToByteArray(Sanitizer.CleanSHA1(value)); }
        }

        /// <summary>
        /// File SHA-256 hash
        /// </summary>
        [JsonProperty("sha256", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("sha256")]
        public string SHA256
        {
            get { return _sha256.IsNullOrEmpty() ? null : Utilities.ByteArrayToString(_sha256); }
            set { _sha256 = Utilities.StringToByteArray(Sanitizer.CleanSHA256(value)); }
        }

        /// <summary>
        /// File SHA-384 hash
        /// </summary>
        [JsonProperty("sha384", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("sha384")]
        public string SHA384
        {
            get { return _sha384.IsNullOrEmpty() ? null : Utilities.ByteArrayToString(_sha384); }
            set { _sha384 = Utilities.StringToByteArray(Sanitizer.CleanSHA384(value)); }
        }

        /// <summary>
        /// File SHA-512 hash
        /// </summary>
        [JsonProperty("sha512", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("sha512")]
        public string SHA512
        {
            get { return _sha512.IsNullOrEmpty() ? null : Utilities.ByteArrayToString(_sha512); }
            set { _sha512 = Utilities.StringToByteArray(Sanitizer.CleanSHA512(value)); }
        }

        /// <summary>
        /// File SpamSum fuzzy hash
        /// </summary>
        [JsonProperty("spamsum", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("spamsum")]
        public string SpamSum
        {
            get { return _spamsum.IsNullOrEmpty() ? null : Encoding.UTF8.GetString(_spamsum); }
            set { _spamsum = Encoding.UTF8.GetBytes(value ?? string.Empty); }
        }

        /// <summary>
        /// Rom name to merge from parent
        /// </summary>
        [JsonProperty("merge", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("merge")]
        public string MergeTag { get; set; }

        /// <summary>
        /// Rom region
        /// </summary>
        [JsonProperty("region", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("biregionos")]
        public string Region { get; set; }

        /// <summary>
        /// Data offset within rom
        /// </summary>
        [JsonProperty("offset", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("offset")]
        public string Offset { get; set; }

        /// <summary>
        /// File created date
        /// </summary>
        [JsonProperty("date", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("date")]
        public string Date { get; set; }

        /// <summary>
        /// Rom dump status
        /// </summary>
        [JsonProperty("status", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlElement("status")]
        public ItemStatus ItemStatus { get; set; }

        [JsonIgnore]
        public bool ItemStatusSpecified { get { return ItemStatus != ItemStatus.NULL && ItemStatus != ItemStatus.None; } }

        /// <summary>
        /// Determine if the rom is optional in the set
        /// </summary>
        [JsonProperty("optional", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("optional")]
        public bool? Optional { get; set; } = null;

        [JsonIgnore]
        public bool OptionalSpecified { get { return Optional != null; } }

        /// <summary>
        /// Determine if the CRC32 hash is inverted
        /// </summary>
        [JsonProperty("inverted", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("inverted")]
        public bool? Inverted { get; set; } = null;

        [JsonIgnore]
        public bool InvertedSpecified { get { return Inverted != null; } }

        #endregion

        #region AttractMode

        /// <summary>
        /// Alternate name for the item
        /// </summary>
        [JsonProperty("alt_romname", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("alt_romname")]
        public string AltName { get; set; }

        /// <summary>
        /// Alternate title for the item
        /// </summary>
        [JsonProperty("alt_title", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("alt_title")]
        public string AltTitle { get; set; }

        #endregion

        #region OpenMSX

        /// <summary>
        /// OpenMSX sub item type
        /// </summary>
        [JsonProperty("original", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("original")]
        public Original Original { get; set; } = null;

        [JsonIgnore]
        public bool OriginalSpecified { get { return Original != null && Original != default; } }

        /// <summary>
        /// OpenMSX sub item type
        /// </summary>
        [JsonProperty("openmsx_subtype", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("openmsx_subtype")]
        [JsonConverter(typeof(StringEnumConverter))]
        public OpenMSXSubType OpenMSXSubType { get; set; }

        [JsonIgnore]
        public bool OpenMSXSubTypeSpecified { get { return OpenMSXSubType != OpenMSXSubType.NULL; } }

        /// <summary>
        /// OpenMSX sub item type
        /// </summary>
        /// <remarks>Not related to the subtype above</remarks>
        [JsonProperty("openmsx_type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("openmsx_type")]
        public string OpenMSXType { get; set; }

        /// <summary>
        /// Item remark (like a comment)
        /// </summary>
        [JsonProperty("remark", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("remark")]
        public string Remark { get; set; }

        /// <summary>
        /// Boot state
        /// </summary>
        [JsonProperty("boot", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("boot")]
        public string Boot { get; set; }

        #endregion

        #region SoftwareList

        /// <summary>
        /// Data area information
        /// </summary>
        [JsonProperty("dataarea", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("dataarea")]
        public DataArea DataArea { get; set; } = null;

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
        [JsonProperty("loadflag", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("loadflag")]
        [JsonConverter(typeof(StringEnumConverter))]
        public LoadFlag LoadFlag { get; set; }

        [JsonIgnore]
        public bool LoadFlagSpecified { get { return LoadFlag != LoadFlag.NULL; } }

        /// <summary>
        /// Original hardware part associated with the item
        /// </summary>
        [JsonProperty("part", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("part")]
        public Part Part { get; set; } = null;

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
        [JsonProperty("value", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("value")]
        public string Value { get; set; }

        #endregion

        #endregion // Fields

        #region Accessors

        /// <summary>
        /// Gets the name to use for a DatItem
        /// </summary>
        /// <returns>Name if available, null otherwise</returns>
        public override string GetName()
        {
            return Name;
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="mappings">Mappings dictionary</param>
        public override void SetFields(Dictionary<Field, string> mappings)
        {
            // Set base fields
            base.SetFields(mappings);

            // Handle Rom-specific fields

            #region Common

            if (mappings.Keys.Contains(Field.DatItem_Name))
                Name = mappings[Field.DatItem_Name];

            if (mappings.Keys.Contains(Field.DatItem_Bios))
                Bios = mappings[Field.DatItem_Bios];

            if (mappings.Keys.Contains(Field.DatItem_Size))
                Size = Sanitizer.CleanLong(mappings[Field.DatItem_Size]);

            if (mappings.Keys.Contains(Field.DatItem_CRC))
                CRC = mappings[Field.DatItem_CRC];

            if (mappings.Keys.Contains(Field.DatItem_MD5))
                MD5 = mappings[Field.DatItem_MD5];

#if NET_FRAMEWORK
            if (mappings.Keys.Contains(Field.DatItem_RIPEMD160))
                RIPEMD160 = mappings[Field.DatItem_RIPEMD160];
#endif

            if (mappings.Keys.Contains(Field.DatItem_SHA1))
                SHA1 = mappings[Field.DatItem_SHA1];

            if (mappings.Keys.Contains(Field.DatItem_SHA256))
                SHA256 = mappings[Field.DatItem_SHA256];

            if (mappings.Keys.Contains(Field.DatItem_SHA384))
                SHA384 = mappings[Field.DatItem_SHA384];

            if (mappings.Keys.Contains(Field.DatItem_SHA512))
                SHA512 = mappings[Field.DatItem_SHA512];

            if (mappings.Keys.Contains(Field.DatItem_SpamSum))
                SpamSum = mappings[Field.DatItem_SpamSum];

            if (mappings.Keys.Contains(Field.DatItem_Merge))
                MergeTag = mappings[Field.DatItem_Merge];

            if (mappings.Keys.Contains(Field.DatItem_Region))
                Region = mappings[Field.DatItem_Region];

            if (mappings.Keys.Contains(Field.DatItem_Offset))
                Offset = mappings[Field.DatItem_Offset];

            if (mappings.Keys.Contains(Field.DatItem_Date))
                Date = mappings[Field.DatItem_Date];

            if (mappings.Keys.Contains(Field.DatItem_Status))
                ItemStatus = mappings[Field.DatItem_Status].AsItemStatus();

            if (mappings.Keys.Contains(Field.DatItem_Optional))
                Optional = mappings[Field.DatItem_Optional].AsYesNo();

            if (mappings.Keys.Contains(Field.DatItem_Inverted))
                Inverted = mappings[Field.DatItem_Optional].AsYesNo();

            #endregion

            #region AttractMode

            if (mappings.Keys.Contains(Field.DatItem_AltName))
                AltName = mappings[Field.DatItem_AltName];

            if (mappings.Keys.Contains(Field.DatItem_AltTitle))
                AltTitle = mappings[Field.DatItem_AltTitle];

            #endregion

            #region OpenMSX

            if (mappings.Keys.Contains(Field.DatItem_Original))
                Original = new Original() { Content = mappings[Field.DatItem_Original] };

            if (mappings.Keys.Contains(Field.DatItem_OpenMSXSubType))
                OpenMSXSubType = mappings[Field.DatItem_OpenMSXSubType].AsOpenMSXSubType();

            if (mappings.Keys.Contains(Field.DatItem_OpenMSXType))
                OpenMSXType = mappings[Field.DatItem_OpenMSXType];

            if (mappings.Keys.Contains(Field.DatItem_Remark))
                Remark = mappings[Field.DatItem_Remark];

            if (mappings.Keys.Contains(Field.DatItem_Boot))
                Boot = mappings[Field.DatItem_Boot];

            #endregion

            #region SoftwareList

            if (mappings.Keys.Contains(Field.DatItem_LoadFlag))
                LoadFlag = mappings[Field.DatItem_LoadFlag].AsLoadFlag();

            if (mappings.Keys.Contains(Field.DatItem_Value))
                Value = mappings[Field.DatItem_Value];

            // Handle DataArea-specific fields
            if (DataArea == null)
                DataArea = new DataArea();

            DataArea.SetFields(mappings);

            // Handle Part-specific fields
            if (Part == null)
                Part = new Part();

            Part.SetFields(mappings);

            #endregion
        }

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
            _crc = baseFile.CRC;
            _md5 = baseFile.MD5;
#if NET_FRAMEWORK
            _ripemd160 = baseFile.RIPEMD160;
#endif
            _sha1 = baseFile.SHA1;
            _sha256 = baseFile.SHA256;
            _sha384 = baseFile.SHA384;
            _sha512 = baseFile.SHA512;
            _spamsum = baseFile.SpamSum;

            ItemType = ItemType.Rom;
            DupeType = 0x00;
            ItemStatus = ItemStatus.None;
            Date = baseFile.Date;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new Rom()
            {
                Name = this.Name,
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Bios = this.Bios,
                Size = this.Size,
                _crc = this._crc,
                _md5 = this._md5,
#if NET_FRAMEWORK
                _ripemd160 = this._ripemd160,
#endif
                _sha1 = this._sha1,
                _sha256 = this._sha256,
                _sha384 = this._sha384,
                _sha512 = this._sha512,
                _spamsum = this._spamsum,
                MergeTag = this.MergeTag,
                Region = this.Region,
                Offset = this.Offset,
                Date = this.Date,
                ItemStatus = this.ItemStatus,
                Optional = this.Optional,
                Inverted = this.Inverted,

                AltName = this.AltName,
                AltTitle = this.AltTitle,

                Original = this.Original,
                OpenMSXSubType = this.OpenMSXSubType,
                OpenMSXType = this.OpenMSXType,
                Remark = this.Remark,
                Boot = this.Boot,

                DataArea = this.DataArea,
                LoadFlag = this.LoadFlag,
                Part = this.Part,
                Value = this.Value,
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
                CRC = this._crc,
                MD5 = this._md5,
#if NET_FRAMEWORK
                RIPEMD160 = this._ripemd160,
#endif
                SHA1 = this._sha1,
                SHA256 = this._sha256,
                SHA384 = this._sha384,
                SHA512 = this._sha512,
                SpamSum = this._spamsum,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            bool dupefound = false;

            // If we don't have a rom, return false
            if (ItemType != other.ItemType)
                return dupefound;

            // Otherwise, treat it as a Rom
            Rom newOther = other as Rom;

            // If all hashes are empty but they're both nodump and the names match, then they're dupes
            if ((ItemStatus == ItemStatus.Nodump && newOther.ItemStatus == ItemStatus.Nodump)
                && Name == newOther.Name
                && !HasHashes() && !newOther.HasHashes())
            {
                dupefound = true;
            }

            // If we have a file that has no known size, rely on the hashes only
            else if (Size == null && HashMatch(newOther))
            {
                dupefound = true;
            }

            // Otherwise if we get a partial match
            else if (Size == newOther.Size && HashMatch(newOther))
            {
                dupefound = true;
            }

            return dupefound;
        }

        /// <summary>
        /// Fill any missing size and hash information from another Rom
        /// </summary>
        /// <param name="other">Rom to fill information from</param>
        public void FillMissingInformation(Rom other)
        {
            if (Size == null && other.Size != null)
                Size = other.Size;

            if (_crc.IsNullOrEmpty() && !other._crc.IsNullOrEmpty())
                _crc = other._crc;

            if (_md5.IsNullOrEmpty() && !other._md5.IsNullOrEmpty())
                _md5 = other._md5;

#if NET_FRAMEWORK
            if (_ripemd160.IsNullOrEmpty() && !other._ripemd160.IsNullOrEmpty())
                _ripemd160 = other._ripemd160;
#endif

            if (_sha1.IsNullOrEmpty() && !other._sha1.IsNullOrEmpty())
                _sha1 = other._sha1;

            if (_sha256.IsNullOrEmpty() && !other._sha256.IsNullOrEmpty())
                _sha256 = other._sha256;

            if (_sha384.IsNullOrEmpty() && !other._sha384.IsNullOrEmpty())
                _sha384 = other._sha384;

            if (_sha512.IsNullOrEmpty() && !other._sha512.IsNullOrEmpty())
                _sha512 = other._sha512;

            if (_spamsum.IsNullOrEmpty() && !other._spamsum.IsNullOrEmpty())
                _spamsum = other._spamsum;
        }

        /// <summary>
        /// Get unique duplicate suffix on name collision
        /// </summary>
        /// <returns>String representing the suffix</returns>
        public string GetDuplicateSuffix()
        {
            if (!_crc.IsNullOrEmpty())
                return $"_{CRC}";
            else if (!_md5.IsNullOrEmpty())
                return $"_{MD5}";
            else if (!_sha1.IsNullOrEmpty())
                return $"_{SHA1}";
            else if (!_sha256.IsNullOrEmpty())
                return $"_{SHA256}";
            else if (!_sha384.IsNullOrEmpty())
                return $"_{SHA384}";
            else if (!_sha512.IsNullOrEmpty())
                return $"_{SHA512}";
            else if (!_spamsum.IsNullOrEmpty())
                return $"_{SpamSum}";
            else
                return "_1";
        }

        /// <summary>
        /// Returns if the Rom contains any hashes
        /// </summary>
        /// <returns>True if any hash exists, false otherwise</returns>
        public bool HasHashes()
        {
            return !_crc.IsNullOrEmpty()
                || !_md5.IsNullOrEmpty()
#if NET_FRAMEWORK
                || !_ripemd160.IsNullOrEmpty()
#endif
                || !_sha1.IsNullOrEmpty()
                || !_sha256.IsNullOrEmpty()
                || !_sha384.IsNullOrEmpty()
                || !_sha512.IsNullOrEmpty()
                || !_spamsum.IsNullOrEmpty();
        }

        /// <summary>
        /// Returns if all of the hashes are set to their 0-byte values
        /// </summary>
        /// <returns>True if any hash matches the 0-byte value, false otherwise</returns>
        public bool HasZeroHash()
        {
            return (_crc != null && _crc.SequenceEqual(Constants.CRCZeroBytes))
                || (_md5 != null && _md5.SequenceEqual(Constants.MD5ZeroBytes))
#if NET_FRAMEWORK
                || (_ripemd160 != null && _ripemd160.SequenceEqual(Constants.RIPEMD160ZeroBytes))
#endif
                || (_sha1 != null && _sha1.SequenceEqual(Constants.SHA1ZeroBytes))
                || (_sha256 != null && _sha256.SequenceEqual(Constants.SHA256ZeroBytes))
                || (_sha384 != null && _sha384.SequenceEqual(Constants.SHA384ZeroBytes))
                || (_sha512 != null && _sha512.SequenceEqual(Constants.SHA512ZeroBytes))
                || (_spamsum != null && _spamsum.SequenceEqual(Constants.SpamSumZeroBytes));
        }

        /// <summary>
        /// Returns if there are no, non-empty hashes in common with another Rom
        /// </summary>
        /// <param name="other">Rom to compare against</param>
        /// <returns>True if at least one hash is not mutually exclusive, false otherwise</returns>
        private bool HasCommonHash(Rom other)
        {
            return !(_crc.IsNullOrEmpty() ^ other._crc.IsNullOrEmpty())
                || !(_md5.IsNullOrEmpty() ^ other._md5.IsNullOrEmpty())
#if NET_FRAMEWORK
                || !(_ripemd160.IsNullOrEmpty() || other._ripemd160.IsNullOrEmpty())
#endif
                || !(_sha1.IsNullOrEmpty() ^ other._sha1.IsNullOrEmpty())
                || !(_sha256.IsNullOrEmpty() ^ other._sha256.IsNullOrEmpty())
                || !(_sha384.IsNullOrEmpty() ^ other._sha384.IsNullOrEmpty())
                || !(_sha512.IsNullOrEmpty() ^ other._sha512.IsNullOrEmpty())
                || !(_spamsum.IsNullOrEmpty() ^ other._spamsum.IsNullOrEmpty());
        }

        /// <summary>
        /// Returns if any hashes are common with another Rom
        /// </summary>
        /// <param name="other">Rom to compare against</param>
        /// <returns>True if any hashes are in common, false otherwise</returns>
        private bool HashMatch(Rom other)
        {
            // If either have no hashes, we return false, otherwise this would be a false positive
            if (!HasHashes() || !other.HasHashes())
                return false;

            // If neither have hashes in common, we return false, otherwise this would be a false positive
            if (!HasCommonHash(other))
                return false;

            // Return if all hashes match according to merge rules
            return ConditionalHashEquals(_crc, other._crc)
                && ConditionalHashEquals(_md5, other._md5)
#if NET_FRAMEWORK
                && ConditionalHashEquals(_ripemd160, other._ripemd160)
#endif
                && ConditionalHashEquals(_sha1, other._sha1)
                && ConditionalHashEquals(_sha256, other._sha256)
                && ConditionalHashEquals(_sha384, other._sha384)
                && ConditionalHashEquals(_sha512, other._sha512)
                && ConditionalHashEquals(_spamsum, other._spamsum);
        }

        #endregion

        #region Filtering

        /// <summary>
        /// Clean a DatItem according to the cleaner
        /// </summary>
        /// <param name="cleaner">Cleaner to implement</param>
        public override void Clean(Cleaner cleaner)
        {
            // Clean common items first
            base.Clean(cleaner);

            // If we're stripping unicode characters, strip item name
            if (cleaner?.RemoveUnicode == true)
                Name = Sanitizer.RemoveUnicodeCharacters(Name);

            // If we are in NTFS trim mode, trim the game name
            if (cleaner?.Trim == true)
            {
                // Windows max name length is 260
                int usableLength = 260 - Machine.Name.Length - (cleaner.Root?.Length ?? 0);
                if (Name.Length > usableLength)
                {
                    string ext = Path.GetExtension(Name);
                    Name = Name.Substring(0, usableLength - ext.Length);
                    Name += ext;
                }
            }
        }

        /// <summary>
        /// Check to see if a DatItem passes the filter
        /// </summary>
        /// <param name="filter">Filter to check against</param>
        /// <param name="sub">True if this is a subitem, false otherwise</param>
        /// <returns>True if the item passed the filter, false otherwise</returns>
        public override bool PassesFilter(Filter filter, bool sub = false)
        {
            // Check common fields first
            if (!base.PassesFilter(filter, sub))
                return false;

            #region Common

            // Filter on item name
            if (!filter.PassStringFilter(filter.DatItem_Name, Name))
                return false;

            // Filter on bios
            if (!filter.PassStringFilter(filter.DatItem_Bios, Bios))
                return false;

            // Filter on rom size
            if (!filter.PassLongFilter(filter.DatItem_Size, Size))
                return false;

            // Filter on CRC
            if (!filter.PassStringFilter(filter.DatItem_CRC, CRC))
                return false;

            // Filter on MD5
            if (!filter.PassStringFilter(filter.DatItem_MD5, MD5))
                return false;

#if NET_FRAMEWORK
            // Filter on RIPEMD160
            if (!filter.PassStringFilter(filter.DatItem_RIPEMD160, RIPEMD160))
                return false;
#endif

            // Filter on SHA-1
            if (!filter.PassStringFilter(filter.DatItem_SHA1, SHA1))
                return false;

            // Filter on SHA-256
            if (!filter.PassStringFilter(filter.DatItem_SHA256, SHA256))
                return false;

            // Filter on SHA-384
            if (!filter.PassStringFilter(filter.DatItem_SHA384, SHA384))
                return false;

            // Filter on SHA-512
            if (!filter.PassStringFilter(filter.DatItem_SHA512, SHA512))
                return false;

            // Filter on SpamSum
            if (!filter.PassStringFilter(filter.DatItem_SpamSum, SpamSum))
                return false;

            // Filter on merge tag
            if (!filter.PassStringFilter(filter.DatItem_Merge, MergeTag))
                return false;

            // Filter on region
            if (!filter.PassStringFilter(filter.DatItem_Region, Region))
                return false;

            // Filter on offset
            if (!filter.PassStringFilter(filter.DatItem_Offset, Offset))
                return false;

            // Filter on date
            if (!filter.PassStringFilter(filter.DatItem_Date, Date))
                return false;

            // Filter on status
            if (filter.DatItem_Status.MatchesPositive(ItemStatus.NULL, ItemStatus) == false)
                return false;
            if (filter.DatItem_Status.MatchesNegative(ItemStatus.NULL, ItemStatus) == true)
                return false;

            // Filter on optional
            if (!filter.PassBoolFilter(filter.DatItem_Optional, Optional))
                return false;

            // Filter on inverted
            if (!filter.PassBoolFilter(filter.DatItem_Inverted, Inverted))
                return false;

            #endregion

            #region AttractMode

            // Filter on alt name
            if (!filter.PassStringFilter(filter.DatItem_AltName, AltName))
                return false;

            // Filter on alt title
            if (!filter.PassStringFilter(filter.DatItem_AltTitle, AltTitle))
                return false;

            #endregion

            #region OpenMSX

            // Filter on original
            if (!filter.PassStringFilter(filter.DatItem_Original, Original?.Content))
                return false;

            // Filter on OpenMSX subtype
            if (filter.DatItem_OpenMSXSubType.MatchesPositive(OpenMSXSubType.NULL, OpenMSXSubType) == false)
                return false;
            if (filter.DatItem_OpenMSXSubType.MatchesNegative(OpenMSXSubType.NULL, OpenMSXSubType) == true)
                return false;

            // Filter on OpenMSX type
            if (!filter.PassStringFilter(filter.DatItem_OpenMSXType, OpenMSXType))
                return false;

            // Filter on remark
            if (!filter.PassStringFilter(filter.DatItem_Remark, Remark))
                return false;

            // Filter on boot
            if (!filter.PassStringFilter(filter.DatItem_Boot, Boot))
                return false;

            #endregion

            #region SoftwareList

            // Filter on load flag
            if (filter.DatItem_LoadFlag.MatchesPositive(LoadFlag.NULL, LoadFlag) == false)
                return false;
            if (filter.DatItem_LoadFlag.MatchesNegative(LoadFlag.NULL, LoadFlag) == true)
                return false;

            // Filter on value
            if (!filter.PassStringFilter(filter.DatItem_Value, Value))
                return false;

            // Filter on DataArea
            if (DataAreaSpecified)
            {
                if (!DataArea.PassesFilter(filter, true))
                    return false;
            }

            // Filter on Part
            if (PartSpecified)
            {
                if (!Part.PassesFilter(filter, true))
                    return false;
            }

            #endregion

            return true;
        }

        /// <summary>
        /// Remove fields from the DatItem
        /// </summary>
        /// <param name="fields">List of Fields to remove</param>
        public override void RemoveFields(List<Field> fields)
        {
            // Remove common fields first
            base.RemoveFields(fields);

            // Remove the fields

            #region Common

            if (fields.Contains(Field.DatItem_Name))
                Name = null;

            if (fields.Contains(Field.DatItem_Bios))
                Bios = null;

            if (fields.Contains(Field.DatItem_Size))
                Size = 0;

            if (fields.Contains(Field.DatItem_CRC))
                CRC = null;

            if (fields.Contains(Field.DatItem_MD5))
                MD5 = null;

#if NET_FRAMEWORK
            if (fields.Contains(Field.DatItem_RIPEMD160))
                RIPEMD160 = null;
#endif

            if (fields.Contains(Field.DatItem_SHA1))
                SHA1 = null;

            if (fields.Contains(Field.DatItem_SHA256))
                SHA256 = null;

            if (fields.Contains(Field.DatItem_SHA384))
                SHA384 = null;

            if (fields.Contains(Field.DatItem_SHA512))
                SHA512 = null;

            if (fields.Contains(Field.DatItem_SpamSum))
                SpamSum = null;

            if (fields.Contains(Field.DatItem_Merge))
                MergeTag = null;

            if (fields.Contains(Field.DatItem_Region))
                Region = null;

            if (fields.Contains(Field.DatItem_Offset))
                Offset = null;

            if (fields.Contains(Field.DatItem_Date))
                Date = null;

            if (fields.Contains(Field.DatItem_Status))
                ItemStatus = ItemStatus.NULL;

            if (fields.Contains(Field.DatItem_Optional))
                Optional = null;

            if (fields.Contains(Field.DatItem_Inverted))
                Inverted = null;

            #endregion

            #region AttractMode

            if (fields.Contains(Field.DatItem_AltName))
                AltName = null;

            if (fields.Contains(Field.DatItem_AltTitle))
                AltTitle = null;

            #endregion

            #region OpenMSX

            if (fields.Contains(Field.DatItem_Original))
                Original = null;

            if (fields.Contains(Field.DatItem_OpenMSXSubType))
                OpenMSXSubType = OpenMSXSubType.NULL;

            if (fields.Contains(Field.DatItem_OpenMSXType))
                OpenMSXType = null;

            if (fields.Contains(Field.DatItem_Remark))
                Remark = null;

            if (fields.Contains(Field.DatItem_Boot))
                Boot = null;

            #endregion

            #region SoftwareList

            if (fields.Contains(Field.DatItem_LoadFlag))
                LoadFlag = LoadFlag.NULL;

            if (fields.Contains(Field.DatItem_Value))
                Value = null;

            if (DataAreaSpecified)
                DataArea.RemoveFields(fields);

            if (PartSpecified)
                Part.RemoveFields(fields);

            #endregion
        }

        /// <summary>
        /// Set internal names to match One Rom Per Game (ORPG) logic
        /// </summary>
        public override void SetOneRomPerGame()
        {
            string[] splitname = Name.Split('.');
            Machine.Name += $"/{string.Join(".", splitname.Take(splitname.Length > 1 ? splitname.Length - 1 : 1))}";
            Name = Path.GetFileName(Name);
        }

        #endregion

        #region Sorting and Merging

        /// <summary>
        /// Get the dictionary key that should be used for a given item and bucketing type
        /// </summary>
        /// <param name="bucketedBy">Field enum representing what key to get</param>
        /// <param name="lower">True if the key should be lowercased (default), false otherwise</param>
        /// <param name="norename">True if games should only be compared on game and file name, false if system and source are counted</param>
        /// <returns>String representing the key to be used for the DatItem</returns>
        public override string GetKey(Field bucketedBy, bool lower = true, bool norename = true)
        {
            // Set the output key as the default blank string
            string key = string.Empty;

            // Now determine what the key should be based on the bucketedBy value
            switch (bucketedBy)
            {
                case Field.DatItem_CRC:
                    key = CRC;
                    break;

                case Field.DatItem_MD5:
                    key = MD5;
                    break;

#if NET_FRAMEWORK
                case Field.DatItem_RIPEMD160:
                    key = RIPEMD160;
                    break;
#endif

                case Field.DatItem_SHA1:
                    key = SHA1;
                    break;

                case Field.DatItem_SHA256:
                    key = SHA256;
                    break;

                case Field.DatItem_SHA384:
                    key = SHA384;
                    break;

                case Field.DatItem_SHA512:
                    key = SHA512;
                    break;

                case Field.DatItem_SpamSum:
                    key = SpamSum;
                    break;

                // Let the base handle generic stuff
                default:
                    return base.GetKey(bucketedBy, lower, norename);
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
        public override void ReplaceFields(DatItem item, List<Field> fields)
        {
            // Replace common fields first
            base.ReplaceFields(item, fields);

            // If we don't have a Rom to replace from, ignore specific fields
            if (item.ItemType != ItemType.Rom)
                return;

            // Cast for easier access
            Rom newItem = item as Rom;

            // Replace the fields

            #region Common

            if (fields.Contains(Field.DatItem_Name))
                Name = newItem.Name;

            if (fields.Contains(Field.DatItem_Bios))
                Bios = newItem.Bios;

            if (fields.Contains(Field.DatItem_Size))
                Size = newItem.Size;

            if (fields.Contains(Field.DatItem_CRC))
            {
                if (string.IsNullOrEmpty(CRC) && !string.IsNullOrEmpty(newItem.CRC))
                    CRC = newItem.CRC;
            }

            if (fields.Contains(Field.DatItem_MD5))
            {
                if (string.IsNullOrEmpty(MD5) && !string.IsNullOrEmpty(newItem.MD5))
                    MD5 = newItem.MD5;
            }

#if NET_FRAMEWORK
            if (fields.Contains(Field.DatItem_RIPEMD160))
            {
                if (string.IsNullOrEmpty(RIPEMD160) && !string.IsNullOrEmpty(newItem.RIPEMD160))
                    RIPEMD160 = newItem.RIPEMD160;
            }
#endif

            if (fields.Contains(Field.DatItem_SHA1))
            {
                if (string.IsNullOrEmpty(SHA1) && !string.IsNullOrEmpty(newItem.SHA1))
                    SHA1 = newItem.SHA1;
            }

            if (fields.Contains(Field.DatItem_SHA256))
            {
                if (string.IsNullOrEmpty(SHA256) && !string.IsNullOrEmpty(newItem.SHA256))
                    SHA256 = newItem.SHA256;
            }

            if (fields.Contains(Field.DatItem_SHA384))
            {
                if (string.IsNullOrEmpty(SHA384) && !string.IsNullOrEmpty(newItem.SHA384))
                    SHA384 = newItem.SHA384;
            }

            if (fields.Contains(Field.DatItem_SHA512))
            {
                if (string.IsNullOrEmpty(SHA512) && !string.IsNullOrEmpty(newItem.SHA512))
                    SHA512 = newItem.SHA512;
            }

            if (fields.Contains(Field.DatItem_SpamSum))
            {
                if (string.IsNullOrEmpty(SpamSum) && !string.IsNullOrEmpty(newItem.SpamSum))
                    SpamSum = newItem.SpamSum;
            }

            if (fields.Contains(Field.DatItem_Merge))
                MergeTag = newItem.MergeTag;

            if (fields.Contains(Field.DatItem_Region))
                Region = newItem.Region;

            if (fields.Contains(Field.DatItem_Offset))
                Offset = newItem.Offset;

            if (fields.Contains(Field.DatItem_Date))
                Date = newItem.Date;

            if (fields.Contains(Field.DatItem_Status))
                ItemStatus = newItem.ItemStatus;

            if (fields.Contains(Field.DatItem_Optional))
                Optional = newItem.Optional;

            if (fields.Contains(Field.DatItem_Inverted))
                Inverted = newItem.Inverted;

            #endregion

            #region AttractMode

            if (fields.Contains(Field.DatItem_AltName))
                AltName = newItem.AltName;

            if (fields.Contains(Field.DatItem_AltTitle))
                AltTitle = newItem.AltTitle;

            #endregion

            #region OpenMSX

            if (fields.Contains(Field.DatItem_Original))
                Original = newItem.Original;

            if (fields.Contains(Field.DatItem_OpenMSXSubType))
                OpenMSXSubType = newItem.OpenMSXSubType;

            if (fields.Contains(Field.DatItem_OpenMSXType))
                OpenMSXType = newItem.OpenMSXType;

            if (fields.Contains(Field.DatItem_Remark))
                Remark = newItem.Remark;

            if (fields.Contains(Field.DatItem_Boot))
                Boot = newItem.Boot;

            #endregion

            #region SoftwareList

            if (fields.Contains(Field.DatItem_LoadFlag))
                LoadFlag = newItem.LoadFlag;

            if (fields.Contains(Field.DatItem_Value))
                Value = newItem.Value;

            if (DataAreaSpecified && newItem.DataAreaSpecified)
                DataArea.ReplaceFields(newItem.DataArea, fields);

            if (PartSpecified && newItem.PartSpecified)
                Part.ReplaceFields(newItem.Part, fields);

            #endregion
        }

        #endregion
    }
}
