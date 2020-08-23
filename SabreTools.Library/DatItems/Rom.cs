using System;
using System.Collections.Generic;
using System.Linq;

using SabreTools.Library.Data;
using SabreTools.Library.DatFiles;
using SabreTools.Library.FileTypes;
using SabreTools.Library.Filtering;
using SabreTools.Library.Tools;
using Newtonsoft.Json;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Represents a generic file within a set
    /// </summary>
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

        #endregion

        #region Fields

        /// <summary>
        /// What BIOS is required for this rom
        /// </summary>
        [JsonProperty("bios")]
        public string Bios { get; set; }

        /// <summary>
        /// Byte size of the rom
        /// </summary>
        [JsonProperty("size")]
        public long Size { get; set; }

        /// <summary>
        /// File CRC32 hash
        /// </summary>
        [JsonProperty("crc")]
        public string CRC
        {
            get { return _crc.IsNullOrEmpty() ? null : Utilities.ByteArrayToString(_crc); }
            set { _crc = (value == "null" ? Constants.CRCZeroBytes : Utilities.StringToByteArray(Sanitizer.CleanCRC32(value))); }
        }

        /// <summary>
        /// File MD5 hash
        /// </summary>
        [JsonProperty("md5")]
        public string MD5
        {
            get { return _md5.IsNullOrEmpty() ? null : Utilities.ByteArrayToString(_md5); }
            set { _md5 = Utilities.StringToByteArray(Sanitizer.CleanMD5(value)); }
        }

#if NET_FRAMEWORK
        /// <summary>
        /// File RIPEMD160 hash
        /// </summary>
        [JsonProperty("ripemd160")]
        public string RIPEMD160
        {
            get { return _ripemd160.IsNullOrEmpty() ? null : Utilities.ByteArrayToString(_ripemd160); }
            set { _ripemd160 = Utilities.StringToByteArray(Sanitizer.CleanRIPEMD160(value)); }
        }
#endif

        /// <summary>
        /// File SHA-1 hash
        /// </summary>
        [JsonProperty("sha1")]
        public string SHA1
        {
            get { return _sha1.IsNullOrEmpty() ? null : Utilities.ByteArrayToString(_sha1); }
            set { _sha1 = Utilities.StringToByteArray(Sanitizer.CleanSHA1(value)); }
        }

        /// <summary>
        /// File SHA-256 hash
        /// </summary>
        [JsonProperty("sha256")]
        public string SHA256
        {
            get { return _sha256.IsNullOrEmpty() ? null : Utilities.ByteArrayToString(_sha256); }
            set { _sha256 = Utilities.StringToByteArray(Sanitizer.CleanSHA256(value)); }
        }

        /// <summary>
        /// File SHA-384 hash
        /// </summary>
        [JsonProperty("sha384")]
        public string SHA384
        {
            get { return _sha384.IsNullOrEmpty() ? null : Utilities.ByteArrayToString(_sha384); }
            set { _sha384 = Utilities.StringToByteArray(Sanitizer.CleanSHA384(value)); }
        }

        /// <summary>
        /// File SHA-512 hash
        /// </summary>
        [JsonProperty("sha512")]
        public string SHA512
        {
            get { return _sha512.IsNullOrEmpty() ? null : Utilities.ByteArrayToString(_sha512); }
            set { _sha512 = Utilities.StringToByteArray(Sanitizer.CleanSHA512(value)); }
        }

        /// <summary>
        /// Rom name to merge from parent
        /// </summary>
        [JsonProperty("merge")]
        public string MergeTag { get; set; }

        /// <summary>
        /// Rom region
        /// </summary>
        [JsonProperty("region")]
        public string Region { get; set; }

        /// <summary>
        /// Data offset within rom
        /// </summary>
        [JsonProperty("offset")]
        public string Offset { get; set; }

        /// <summary>
        /// File created date
        /// </summary>
        [JsonProperty("date")]
        public string Date { get; set; }

        /// <summary>
        /// Rom dump status
        /// </summary>
        [JsonProperty("status")]
        public ItemStatus ItemStatus { get; set; }

        /// <summary>
        /// Determine if the rom is optional in the set
        /// </summary>
        [JsonProperty("optional")]
        public bool? Optional { get; set; }

        /// <summary>
        /// Determine if the CRC32 hash is inverted
        /// </summary>
        [JsonProperty("inverted")]
        public bool? Inverted { get; set; }

        #endregion

        #region Accessors

        /// <summary>
        /// Get the value of that field as a string, if possible
        /// </summary>
        public override string GetField(Field field, List<Field> excludeFields)
        {
            // If the field is to be excluded, return empty string
            if (excludeFields.Contains(field))
                return string.Empty;

            // Handle Rom-specific fields
            string fieldValue;
            switch (field)
            {
                case Field.Bios:
                    fieldValue = Bios;
                    break;
                case Field.Size:
                    fieldValue = Size.ToString();
                    break;
                case Field.CRC:
                    fieldValue = CRC;
                    break;
                case Field.MD5:
                    fieldValue = MD5;
                    break;
#if NET_FRAMEWORK
                case Field.RIPEMD160:
                    fieldValue = RIPEMD160;
                    break;
#endif
                case Field.SHA1:
                    fieldValue = SHA1;
                    break;
                case Field.SHA256:
                    fieldValue = SHA256;
                    break;
                case Field.SHA384:
                    fieldValue = SHA384;
                    break;
                case Field.SHA512:
                    fieldValue = SHA512;
                    break;
                case Field.Merge:
                    fieldValue = MergeTag;
                    break;
                case Field.Region:
                    fieldValue = Region;
                    break;
                case Field.Offset:
                    fieldValue = Offset;
                    break;
                case Field.Date:
                    fieldValue = Date;
                    break;
                case Field.Status:
                    fieldValue = ItemStatus.ToString();
                    break;
                case Field.Optional:
                    fieldValue = Optional?.ToString();
                    break;
                case Field.Inverted:
                    fieldValue = Inverted?.ToString();
                    break;

                // For everything else, use the base method
                default:
                    return base.GetField(field, excludeFields);
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
        public override void SetFields(Dictionary<Field, string> mappings)
        {
            // Set base fields
            base.SetFields(mappings);

            // Handle Rom-specific fields
            if (mappings.Keys.Contains(Field.Bios))
                Bios = mappings[Field.Bios];

            if (mappings.Keys.Contains(Field.Size))
            {
                if (Int64.TryParse(mappings[Field.Size], out long size))
                    Size = size;
            }

            if (mappings.Keys.Contains(Field.CRC))
                CRC = mappings[Field.CRC];

            if (mappings.Keys.Contains(Field.MD5))
                MD5 = mappings[Field.MD5];

#if NET_FRAMEWORK
            if (mappings.Keys.Contains(Field.RIPEMD160))
                RIPEMD160 = mappings[Field.RIPEMD160];
#endif

            if (mappings.Keys.Contains(Field.SHA1))
                SHA1 = mappings[Field.SHA1];

            if (mappings.Keys.Contains(Field.SHA256))
                SHA256 = mappings[Field.SHA256];

            if (mappings.Keys.Contains(Field.SHA384))
                SHA384 = mappings[Field.SHA384];

            if (mappings.Keys.Contains(Field.SHA512))
                SHA512 = mappings[Field.SHA512];

            if (mappings.Keys.Contains(Field.Merge))
                MergeTag = mappings[Field.Merge];

            if (mappings.Keys.Contains(Field.Region))
                Region = mappings[Field.Region];

            if (mappings.Keys.Contains(Field.Offset))
                Offset = mappings[Field.Offset];

            if (mappings.Keys.Contains(Field.Date))
                Date = mappings[Field.Date];

            if (mappings.Keys.Contains(Field.Status))
                ItemStatus = mappings[Field.Status].AsItemStatus();

            if (mappings.Keys.Contains(Field.Optional))
                Optional = mappings[Field.Optional].AsYesNo();

            if (mappings.Keys.Contains(Field.Inverted))
                Inverted = mappings[Field.Optional].AsYesNo();
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Rom object
        /// </summary>
        public Rom()
        {
            Name = string.Empty;
            ItemType = ItemType.Rom;
            DupeType = 0x00;
            ItemStatus = ItemStatus.None;
            Date = string.Empty;
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
            Size = -1;
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
            Size = baseFile.Size ?? -1;
            _crc = baseFile.CRC;
            _md5 = baseFile.MD5;
#if NET_FRAMEWORK
            _ripemd160 = baseFile.RIPEMD160;
#endif
            _sha1 = baseFile.SHA1;
            _sha256 = baseFile.SHA256;
            _sha384 = baseFile.SHA384;
            _sha512 = baseFile.SHA512;

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

                AltName = this.AltName,
                AltTitle = this.AltTitle,

                Original = this.Original,
                OpenMSXSubType = this.OpenMSXSubType,
                OpenMSXType = this.OpenMSXType,
                Remark = this.Remark,
                Boot = this.Boot,

                PartName = this.PartName,
                PartInterface = this.PartInterface,
                Features = this.Features,
                AreaName = this.AreaName,
                AreaSize = this.AreaSize,
                AreaWidth = this.AreaWidth,
                AreaEndianness = this.AreaEndianness,
                Value = this.Value,
                LoadFlag = this.LoadFlag,

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
                MergeTag = this.MergeTag,
                Region = this.Region,
                Offset = this.Offset,
                Date = this.Date,
                ItemStatus = this.ItemStatus,
                Optional = this.Optional,
                Inverted = this.Inverted,
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
            else if (Size == -1 && HashMatch(newOther))
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
            if (Size == -1 && other.Size != -1)
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
            else
                return "_1";
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
                || !(_sha512.IsNullOrEmpty() ^ other._sha512.IsNullOrEmpty());
        }

        /// <summary>
        /// Returns if the Rom contains any hashes
        /// </summary>
        /// <returns>True if any hash exists, false otherwise</returns>
        private bool HasHashes()
        {
            return !_crc.IsNullOrEmpty()
                || !_md5.IsNullOrEmpty()
#if NET_FRAMEWORK
                || !_ripemd160.IsNullOrEmpty()
#endif
                || !_sha1.IsNullOrEmpty()
                || !_sha256.IsNullOrEmpty()
                || !_sha384.IsNullOrEmpty()
                || !_sha512.IsNullOrEmpty();
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
                && ConditionalHashEquals(_sha512, other._sha512);
        }

        #endregion

        #region Filtering

        /// <summary>
        /// Check to see if a DatItem passes the filter
        /// </summary>
        /// <param name="filter">Filter to check against</param>
        /// <returns>True if the item passed the filter, false otherwise</returns>
        public override bool PassesFilter(Filter filter)
        {
            // Check common fields first
            if (!base.PassesFilter(filter))
                return false;

            // Filter on bios
            if (filter.Bios.MatchesPositiveSet(Bios) == false)
                return false;
            if (filter.Bios.MatchesNegativeSet(Bios) == true)
                return false;

            // Filter on rom size
            if (filter.Size.MatchesNeutral(-1, Size) == false)
                return false;
            else if (filter.Size.MatchesPositive(-1, Size) == false)
                return false;
            else if (filter.Size.MatchesNegative(-1, Size) == false)
                return false;

            // Filter on CRC
            if (filter.CRC.MatchesPositiveSet(CRC) == false)
                return false;
            if (filter.CRC.MatchesNegativeSet(CRC) == true)
                return false;

            // Filter on MD5
            if (filter.MD5.MatchesPositiveSet(MD5) == false)
                return false;
            if (filter.MD5.MatchesNegativeSet(MD5) == true)
                return false;

#if NET_FRAMEWORK
            // Filter on RIPEMD160
            if (filter.RIPEMD160.MatchesPositiveSet(RIPEMD160) == false)
                return false;
            if (filter.RIPEMD160.MatchesNegativeSet(RIPEMD160) == true)
                return false;
#endif

            // Filter on SHA-1
            if (filter.SHA1.MatchesPositiveSet(SHA1) == false)
                return false;
            if (filter.SHA1.MatchesNegativeSet(SHA1) == true)
                return false;

            // Filter on SHA-256
            if (filter.SHA256.MatchesPositiveSet(SHA256) == false)
                return false;
            if (filter.SHA256.MatchesNegativeSet(SHA256) == true)
                return false;

            // Filter on SHA-384
            if (filter.SHA384.MatchesPositiveSet(SHA384) == false)
                return false;
            if (filter.SHA384.MatchesNegativeSet(SHA384) == true)
                return false;

            // Filter on SHA-512
            if (filter.SHA512.MatchesPositiveSet(SHA512) == false)
                return false;
            if (filter.SHA512.MatchesNegativeSet(SHA512) == true)
                return false;

            // Filter on merge tag
            if (filter.MergeTag.MatchesPositiveSet(MergeTag) == false)
                return false;
            if (filter.MergeTag.MatchesNegativeSet(MergeTag) == true)
                return false;

            // Filter on region
            if (filter.Region.MatchesPositiveSet(Region) == false)
                return false;
            if (filter.Region.MatchesNegativeSet(Region) == true)
                return false;

            // Filter on offset
            if (filter.Offset.MatchesPositiveSet(Offset) == false)
                return false;
            if (filter.Offset.MatchesNegativeSet(Offset) == true)
                return false;

            // Filter on date
            if (filter.Date.MatchesPositiveSet(Date) == false)
                return false;
            if (filter.Date.MatchesNegativeSet(Date) == true)
                return false;

            // Filter on status
            if (filter.Status.MatchesPositive(ItemStatus.NULL, ItemStatus) == false)
                return false;
            if (filter.Status.MatchesNegative(ItemStatus.NULL, ItemStatus) == true)
                return false;

            // Filter on optional
            if (filter.Optional.MatchesNeutral(null, Optional) == false)
                return false;

            // Filter on inverted
            if (filter.Inverted.MatchesNeutral(null, Inverted) == false)
                return false;

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
            if (fields.Contains(Field.Bios))
                Bios = null;

            if (fields.Contains(Field.Size))
                Size = 0;

            if (fields.Contains(Field.CRC))
                CRC = null;

            if (fields.Contains(Field.MD5))
                MD5 = null;

#if NET_FRAMEWORK
            if (fields.Contains(Field.RIPEMD160))
                RIPEMD160 = null;
#endif

            if (fields.Contains(Field.SHA1))
                SHA1 = null;

            if (fields.Contains(Field.SHA256))
                SHA256 = null;

            if (fields.Contains(Field.SHA384))
                SHA384 = null;

            if (fields.Contains(Field.SHA512))
                SHA512 = null;

            if (fields.Contains(Field.Merge))
                MergeTag = null;

            if (fields.Contains(Field.Region))
                Region = null;

            if (fields.Contains(Field.Offset))
                Offset = null;

            if (fields.Contains(Field.Date))
                Date = null;

            if (fields.Contains(Field.Status))
                ItemStatus = ItemStatus.NULL;

            if (fields.Contains(Field.Optional))
                Optional = null;

            if (fields.Contains(Field.Inverted))
                Inverted = null;
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
                case Field.CRC:
                    key = CRC;
                    break;

                case Field.MD5:
                    key = MD5;
                    break;

#if NET_FRAMEWORK
                case Field.RIPEMD160:
                    key = RIPEMD160;
                    break;
#endif

                case Field.SHA1:
                    key = SHA1;
                    break;

                case Field.SHA256:
                    key = SHA256;
                    break;

                case Field.SHA384:
                    key = SHA384;
                    break;

                case Field.SHA512:
                    key = SHA512;
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
            if (fields.Contains(Field.Bios))
                Bios = newItem.Bios;

            if (fields.Contains(Field.Size))
                Size = newItem.Size;

            if (fields.Contains(Field.CRC))
            {
                if (string.IsNullOrEmpty(CRC) && !string.IsNullOrEmpty(newItem.CRC))
                    CRC = newItem.CRC;
            }

            if (fields.Contains(Field.MD5))
            {
                if (string.IsNullOrEmpty(MD5) && !string.IsNullOrEmpty(newItem.MD5))
                    MD5 = newItem.MD5;
            }

#if NET_FRAMEWORK
            if (fields.Contains(Field.RIPEMD160))
            {
                if (string.IsNullOrEmpty(RIPEMD160) && !string.IsNullOrEmpty(newItem.RIPEMD160))
                    RIPEMD160 = newItem.RIPEMD160;
            }
#endif

            if (fields.Contains(Field.SHA1))
            {
                if (string.IsNullOrEmpty(SHA1) && !string.IsNullOrEmpty(newItem.SHA1))
                    SHA1 = newItem.SHA1;
            }

            if (fields.Contains(Field.SHA256))
            {
                if (string.IsNullOrEmpty(SHA256) && !string.IsNullOrEmpty(newItem.SHA256))
                    SHA256 = newItem.SHA256;
            }

            if (fields.Contains(Field.SHA384))
            {
                if (string.IsNullOrEmpty(SHA384) && !string.IsNullOrEmpty(newItem.SHA384))
                    SHA384 = newItem.SHA384;
            }

            if (fields.Contains(Field.SHA512))
            {
                if (string.IsNullOrEmpty(SHA512) && !string.IsNullOrEmpty(newItem.SHA512))
                    SHA512 = newItem.SHA512;
            }

            if (fields.Contains(Field.Merge))
                MergeTag = newItem.MergeTag;

            if (fields.Contains(Field.Region))
                Region = newItem.Region;

            if (fields.Contains(Field.Offset))
                Offset = newItem.Offset;

            if (fields.Contains(Field.Date))
                Date = newItem.Date;

            if (fields.Contains(Field.Status))
                ItemStatus = newItem.ItemStatus;

            if (fields.Contains(Field.Optional))
                Optional = newItem.Optional;

            if (fields.Contains(Field.Inverted))
                Inverted = newItem.Inverted;
        }

        #endregion
    }
}
