using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.FileTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SabreTools.DatItems
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
            set { _crc = (value == "null" ? Constants.CRCZeroBytes : Utilities.StringToByteArray(CleanCRC32(value))); }
        }

        /// <summary>
        /// File MD5 hash
        /// </summary>
        [JsonProperty("md5", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("md5")]
        public string MD5
        {
            get { return _md5.IsNullOrEmpty() ? null : Utilities.ByteArrayToString(_md5); }
            set { _md5 = Utilities.StringToByteArray(CleanMD5(value)); }
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
            set { _ripemd160 = Utilities.StringToByteArray(CleanRIPEMD160(value)); }
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
            set { _sha1 = Utilities.StringToByteArray(CleanSHA1(value)); }
        }

        /// <summary>
        /// File SHA-256 hash
        /// </summary>
        [JsonProperty("sha256", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("sha256")]
        public string SHA256
        {
            get { return _sha256.IsNullOrEmpty() ? null : Utilities.ByteArrayToString(_sha256); }
            set { _sha256 = Utilities.StringToByteArray(CleanSHA256(value)); }
        }

        /// <summary>
        /// File SHA-384 hash
        /// </summary>
        [JsonProperty("sha384", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("sha384")]
        public string SHA384
        {
            get { return _sha384.IsNullOrEmpty() ? null : Utilities.ByteArrayToString(_sha384); }
            set { _sha384 = Utilities.StringToByteArray(CleanSHA384(value)); }
        }

        /// <summary>
        /// File SHA-512 hash
        /// </summary>
        [JsonProperty("sha512", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("sha512")]
        public string SHA512
        {
            get { return _sha512.IsNullOrEmpty() ? null : Utilities.ByteArrayToString(_sha512); }
            set { _sha512 = Utilities.StringToByteArray(CleanSHA512(value)); }
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

        /// <inheritdoc/>
        public override string GetName()
        {
            return Name;
        }

        /// <inheritdoc/>
        public override void SetName(string name)
        {
            Name = name;
        }

        /// <inheritdoc/>
        public override void SetFields(
            Dictionary<DatItemField, string> datItemMappings,
            Dictionary<MachineField, string> machineMappings)
        {
            // Set base fields
            base.SetFields(datItemMappings, machineMappings);

            // Handle Rom-specific fields

            #region Common

            if (datItemMappings.Keys.Contains(DatItemField.Name))
                Name = datItemMappings[DatItemField.Name];

            if (datItemMappings.Keys.Contains(DatItemField.Bios))
                Bios = datItemMappings[DatItemField.Bios];

            if (datItemMappings.Keys.Contains(DatItemField.Size))
                Size = Utilities.CleanLong(datItemMappings[DatItemField.Size]);

            if (datItemMappings.Keys.Contains(DatItemField.CRC))
                CRC = datItemMappings[DatItemField.CRC];

            if (datItemMappings.Keys.Contains(DatItemField.MD5))
                MD5 = datItemMappings[DatItemField.MD5];

#if NET_FRAMEWORK
            if (datItemMappings.Keys.Contains(DatItemField.RIPEMD160))
                RIPEMD160 = datItemMappings[DatItemField.RIPEMD160];
#endif

            if (datItemMappings.Keys.Contains(DatItemField.SHA1))
                SHA1 = datItemMappings[DatItemField.SHA1];

            if (datItemMappings.Keys.Contains(DatItemField.SHA256))
                SHA256 = datItemMappings[DatItemField.SHA256];

            if (datItemMappings.Keys.Contains(DatItemField.SHA384))
                SHA384 = datItemMappings[DatItemField.SHA384];

            if (datItemMappings.Keys.Contains(DatItemField.SHA512))
                SHA512 = datItemMappings[DatItemField.SHA512];

            if (datItemMappings.Keys.Contains(DatItemField.SpamSum))
                SpamSum = datItemMappings[DatItemField.SpamSum];

            if (datItemMappings.Keys.Contains(DatItemField.Merge))
                MergeTag = datItemMappings[DatItemField.Merge];

            if (datItemMappings.Keys.Contains(DatItemField.Region))
                Region = datItemMappings[DatItemField.Region];

            if (datItemMappings.Keys.Contains(DatItemField.Offset))
                Offset = datItemMappings[DatItemField.Offset];

            if (datItemMappings.Keys.Contains(DatItemField.Date))
                Date = datItemMappings[DatItemField.Date];

            if (datItemMappings.Keys.Contains(DatItemField.Status))
                ItemStatus = datItemMappings[DatItemField.Status].AsItemStatus();

            if (datItemMappings.Keys.Contains(DatItemField.Optional))
                Optional = datItemMappings[DatItemField.Optional].AsYesNo();

            if (datItemMappings.Keys.Contains(DatItemField.Inverted))
                Inverted = datItemMappings[DatItemField.Optional].AsYesNo();

            #endregion

            #region AttractMode

            if (datItemMappings.Keys.Contains(DatItemField.AltName))
                AltName = datItemMappings[DatItemField.AltName];

            if (datItemMappings.Keys.Contains(DatItemField.AltTitle))
                AltTitle = datItemMappings[DatItemField.AltTitle];

            #endregion

            #region OpenMSX

            if (datItemMappings.Keys.Contains(DatItemField.Original))
                Original = new Original() { Content = datItemMappings[DatItemField.Original] };

            if (datItemMappings.Keys.Contains(DatItemField.OpenMSXSubType))
                OpenMSXSubType = datItemMappings[DatItemField.OpenMSXSubType].AsOpenMSXSubType();

            if (datItemMappings.Keys.Contains(DatItemField.OpenMSXType))
                OpenMSXType = datItemMappings[DatItemField.OpenMSXType];

            if (datItemMappings.Keys.Contains(DatItemField.Remark))
                Remark = datItemMappings[DatItemField.Remark];

            if (datItemMappings.Keys.Contains(DatItemField.Boot))
                Boot = datItemMappings[DatItemField.Boot];

            #endregion

            #region SoftwareList

            if (datItemMappings.Keys.Contains(DatItemField.LoadFlag))
                LoadFlag = datItemMappings[DatItemField.LoadFlag].AsLoadFlag();

            if (datItemMappings.Keys.Contains(DatItemField.Value))
                Value = datItemMappings[DatItemField.Value];

            // Handle DataArea-specific fields
            if (DataArea == null)
                DataArea = new DataArea();

            DataArea.SetFields(datItemMappings, machineMappings);

            // Handle Part-specific fields
            if (Part == null)
                Part = new Part();

            Part.SetFields(datItemMappings, machineMappings);

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

        /// <inheritdoc/>
        public override void ReplaceFields(
            DatItem item,
            List<DatItemField> datItemFields,
            List<MachineField> machineFields)
        {
            // Replace common fields first
            base.ReplaceFields(item, datItemFields, machineFields);

            // If we don't have a Rom to replace from, ignore specific fields
            if (item.ItemType != ItemType.Rom)
                return;

            // Cast for easier access
            Rom newItem = item as Rom;

            // Replace the fields

            #region Common

            if (datItemFields.Contains(DatItemField.Name))
                Name = newItem.Name;

            if (datItemFields.Contains(DatItemField.Bios))
                Bios = newItem.Bios;

            if (datItemFields.Contains(DatItemField.Size))
                Size = newItem.Size;

            if (datItemFields.Contains(DatItemField.CRC))
            {
                if (string.IsNullOrEmpty(CRC) && !string.IsNullOrEmpty(newItem.CRC))
                    CRC = newItem.CRC;
            }

            if (datItemFields.Contains(DatItemField.MD5))
            {
                if (string.IsNullOrEmpty(MD5) && !string.IsNullOrEmpty(newItem.MD5))
                    MD5 = newItem.MD5;
            }

#if NET_FRAMEWORK
            if (datItemFields.Contains(DatItemField.RIPEMD160))
            {
                if (string.IsNullOrEmpty(RIPEMD160) && !string.IsNullOrEmpty(newItem.RIPEMD160))
                    RIPEMD160 = newItem.RIPEMD160;
            }
#endif

            if (datItemFields.Contains(DatItemField.SHA1))
            {
                if (string.IsNullOrEmpty(SHA1) && !string.IsNullOrEmpty(newItem.SHA1))
                    SHA1 = newItem.SHA1;
            }

            if (datItemFields.Contains(DatItemField.SHA256))
            {
                if (string.IsNullOrEmpty(SHA256) && !string.IsNullOrEmpty(newItem.SHA256))
                    SHA256 = newItem.SHA256;
            }

            if (datItemFields.Contains(DatItemField.SHA384))
            {
                if (string.IsNullOrEmpty(SHA384) && !string.IsNullOrEmpty(newItem.SHA384))
                    SHA384 = newItem.SHA384;
            }

            if (datItemFields.Contains(DatItemField.SHA512))
            {
                if (string.IsNullOrEmpty(SHA512) && !string.IsNullOrEmpty(newItem.SHA512))
                    SHA512 = newItem.SHA512;
            }

            if (datItemFields.Contains(DatItemField.SpamSum))
            {
                if (string.IsNullOrEmpty(SpamSum) && !string.IsNullOrEmpty(newItem.SpamSum))
                    SpamSum = newItem.SpamSum;
            }

            if (datItemFields.Contains(DatItemField.Merge))
                MergeTag = newItem.MergeTag;

            if (datItemFields.Contains(DatItemField.Region))
                Region = newItem.Region;

            if (datItemFields.Contains(DatItemField.Offset))
                Offset = newItem.Offset;

            if (datItemFields.Contains(DatItemField.Date))
                Date = newItem.Date;

            if (datItemFields.Contains(DatItemField.Status))
                ItemStatus = newItem.ItemStatus;

            if (datItemFields.Contains(DatItemField.Optional))
                Optional = newItem.Optional;

            if (datItemFields.Contains(DatItemField.Inverted))
                Inverted = newItem.Inverted;

            #endregion

            #region AttractMode

            if (datItemFields.Contains(DatItemField.AltName))
                AltName = newItem.AltName;

            if (datItemFields.Contains(DatItemField.AltTitle))
                AltTitle = newItem.AltTitle;

            #endregion

            #region OpenMSX

            if (datItemFields.Contains(DatItemField.Original))
                Original = newItem.Original;

            if (datItemFields.Contains(DatItemField.OpenMSXSubType))
                OpenMSXSubType = newItem.OpenMSXSubType;

            if (datItemFields.Contains(DatItemField.OpenMSXType))
                OpenMSXType = newItem.OpenMSXType;

            if (datItemFields.Contains(DatItemField.Remark))
                Remark = newItem.Remark;

            if (datItemFields.Contains(DatItemField.Boot))
                Boot = newItem.Boot;

            #endregion

            #region SoftwareList

            if (datItemFields.Contains(DatItemField.LoadFlag))
                LoadFlag = newItem.LoadFlag;

            if (datItemFields.Contains(DatItemField.Value))
                Value = newItem.Value;

            if (DataAreaSpecified && newItem.DataAreaSpecified)
                DataArea.ReplaceFields(newItem.DataArea, datItemFields, machineFields);

            if (PartSpecified && newItem.PartSpecified)
                Part.ReplaceFields(newItem.Part, datItemFields, machineFields);

            #endregion
        }

        #endregion
    }
}
