using System.Collections.Generic;
using SabreTools.Library.DatFiles;
using SabreTools.Library.FileTypes;
using SabreTools.Library.Tools;
using Newtonsoft.Json;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Represents Compressed Hunks of Data (CHD) formatted disks which use internal hashes
    /// </summary>
    public class Disk : DatItem
    {
        #region Private instance variables

        private byte[] _md5; // 16 bytes
#if NET_FRAMEWORK
        private byte[] _ripemd160; // 20 bytes
#endif
        private byte[] _sha1; // 20 bytes
        private byte[] _sha256; // 32 bytes
        private byte[] _sha384; // 48 bytes
        private byte[] _sha512; // 64 bytes

        #endregion

        #region Publicly facing variables

        /// <summary>
        /// Data MD5 hash
        /// </summary>
        [JsonProperty("md5")]
        public string MD5
        {
            get { return _md5.IsNullOrEmpty() ? null : Utilities.ByteArrayToString(_md5); }
            set { _md5 = Utilities.StringToByteArray(Sanitizer.CleanMD5(value)); }
        }

#if NET_FRAMEWORK
        /// <summary>
        /// Data RIPEMD160 hash
        /// </summary>
        [JsonProperty("ripemd160")]
        public string RIPEMD160
        {
            get { return _ripemd160.IsNullOrEmpty() ? null : Utilities.ByteArrayToString(_ripemd160); }
            set { _ripemd160 = Utilities.StringToByteArray(Sanitizer.CleanRIPEMD160(value)); }
        }
#endif

        /// <summary>
        /// Data SHA-1 hash
        /// </summary>
        [JsonProperty("sha1")]
        public string SHA1
        {
            get { return _sha1.IsNullOrEmpty() ? null : Utilities.ByteArrayToString(_sha1); }
            set { _sha1 = Utilities.StringToByteArray(Sanitizer.CleanSHA1(value)); }
        }

        /// <summary>
        /// Data SHA-256 hash
        /// </summary>
        [JsonProperty("sha256")]
        public string SHA256
        {
            get { return _sha256.IsNullOrEmpty() ? null : Utilities.ByteArrayToString(_sha256); }
            set { _sha256 = Utilities.StringToByteArray(Sanitizer.CleanSHA256(value)); }
        }

        /// <summary>
        /// Data SHA-384 hash
        /// </summary>
        [JsonProperty("sha384")]
        public string SHA384
        {
            get { return _sha384.IsNullOrEmpty() ? null : Utilities.ByteArrayToString(_sha384); }
            set { _sha384 = Utilities.StringToByteArray(Sanitizer.CleanSHA384(value)); }
        }

        /// <summary>
        /// Data SHA-512 hash
        /// </summary>
        [JsonProperty("sha512")]
        public string SHA512
        {
            get { return _sha512.IsNullOrEmpty() ? null : Utilities.ByteArrayToString(_sha512); }
            set { _sha512 = Utilities.StringToByteArray(Sanitizer.CleanSHA512(value)); }
        }

        /// <summary>
        /// Disk name to merge from parent
        /// </summary>
        [JsonProperty("merge")]
        public string MergeTag { get; set; }

        /// <summary>
        /// Disk region
        /// </summary>
        [JsonProperty("region")]
        public string Region { get; set; }

        /// <summary>
        /// Disk index
        /// </summary>
        [JsonProperty("index")]
        public string Index { get; set; }

        /// <summary>
        /// Disk writable flag
        /// </summary>
        [JsonProperty("writable")]
        public bool? Writable { get; set; }

        /// <summary>
        /// Disk dump status
        /// </summary>
        [JsonProperty("status")]
        public ItemStatus ItemStatus { get; set; }

        /// <summary>
        /// Determine if the disk is optional in the set
        /// </summary>
        [JsonProperty("optional")]
        public bool? Optional { get; set; }

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

            // Handle Disk-specific fields
            string fieldValue;
            switch (field)
            {
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
                case Field.Index:
                    fieldValue = Index;
                    break;
                case Field.Writable:
                    fieldValue = Writable?.ToString();
                    break;
                case Field.Optional:
                    fieldValue = Optional?.ToString();
                    break;
                case Field.Status:
                    fieldValue = ItemStatus.ToString();
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

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Disk object
        /// </summary>
        public Disk()
        {
            this.Name = string.Empty;
            this.ItemType = ItemType.Disk;
            this.DupeType = 0x00;
            this.ItemStatus = ItemStatus.None;
        }

        /// <summary>
        /// Create a Rom object from a BaseFile
        /// </summary>
        /// <param name="baseFile"></param>
        public Disk(BaseFile baseFile)
        {
            this.Name = baseFile.Filename;
            _md5 = baseFile.MD5;
#if NET_FRAMEWORK
            _ripemd160 = baseFile.RIPEMD160;
#endif
            _sha1 = baseFile.SHA1;
            _sha256 = baseFile.SHA256;
            _sha384 = baseFile.SHA384;
            _sha512 = baseFile.SHA512;

            this.ItemType = ItemType.Disk;
            this.DupeType = 0x00;
            this.ItemStatus = ItemStatus.None;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new Disk()
            {
                Name = this.Name,
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Supported = this.Supported,
                Publisher = this.Publisher,
                Category = this.Category,
                Infos = this.Infos,
                PartName = this.PartName,
                PartInterface = this.PartInterface,
                Features = this.Features,
                AreaName = this.AreaName,
                AreaSize = this.AreaSize,

                MachineName = this.MachineName,
                Comment = this.Comment,
                MachineDescription = this.MachineDescription,
                Year = this.Year,
                Manufacturer = this.Manufacturer,
                RomOf = this.RomOf,
                CloneOf = this.CloneOf,
                SampleOf = this.SampleOf,
                SourceFile = this.SourceFile,
                Runnable = this.Runnable,
                Board = this.Board,
                RebuildTo = this.RebuildTo,
                Devices = this.Devices,
                MachineType = this.MachineType,

                IndexId = this.IndexId,
                IndexSource = this.IndexSource,

                _md5 = this._md5,
#if NET_FRAMEWORK
                _ripemd160 = this._ripemd160,
#endif
                _sha1 = this._sha1,
                _sha256 = this._sha256,
                _sha384 = this._sha384,
                _sha512 = this._sha512,
                ItemStatus = this.ItemStatus,
            };
        }

        /// <summary>
        /// Convert a disk to the closest Rom approximation
        /// </summary>
        /// <returns></returns>
        public Rom ConvertToRom()
        {
            var rom = new Rom()
            {
                Name = this.Name,
                ItemType = ItemType.Rom,
                DupeType = this.DupeType,

                CRC = null,
                MD5 = this.MD5,
#if NET_FRAMEWORK
                RIPEMD160 = this.RIPEMD160,
#endif
                SHA1 = this.SHA1,
                SHA256 = this.SHA256,
                SHA384 = this.SHA384,
                SHA512 = this.SHA512,

                MergeTag = this.MergeTag,
                Region = this.Region,
                ItemStatus = this.ItemStatus,
                Optional = this.Optional,

                MachineName = this.MachineName,
                Comment = this.Comment,
                MachineDescription = this.MachineDescription,
                Year = this.Year,
                Manufacturer = this.Manufacturer,
                Publisher = this.Publisher,
                Category = this.Category,
                RomOf = this.RomOf,
                CloneOf = this.CloneOf,
                SampleOf = this.SampleOf,
                Supported = this.Supported,
                SourceFile = this.SourceFile,
                Runnable = this.Runnable,
                Board = this.Board,
                RebuildTo = this.RebuildTo,
                Devices = this.Devices,
                SlotOptions = this.SlotOptions,
                Infos = this.Infos,
                MachineType = this.MachineType,

                PartName = this.PartName,
                PartInterface = this.PartInterface,
                Features = this.Features,
                AreaName = this.AreaName,
                AreaSize = this.AreaSize,

                IndexId = this.IndexId,
                IndexSource = this.IndexSource,
                Remove = this.Remove,
            };

            return rom;
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            bool dupefound = false;

            // If we don't have a rom, return false
            if (ItemType != other.ItemType)
                return dupefound;

            // Otherwise, treat it as a Disk
            Disk newOther = other as Disk;

            // If all hashes are empty but they're both nodump and the names match, then they're dupes
            if ((ItemStatus == ItemStatus.Nodump && newOther.ItemStatus == ItemStatus.Nodump)
                && Name == newOther.Name
                && !HasHashes() && !newOther.HasHashes())
            {
                dupefound = true;
            }

            // Otherwise if we get a partial match
            else if (HashMatch(newOther))
            {
                dupefound = true;
            }

            return dupefound;
        }

        /// <summary>
        /// Fill any missing size and hash information from another Disk
        /// </summary>
        /// <param name="other">Disk to fill information from</param>
        public void FillMissingInformation(Disk other)
        {
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
            if (!_md5.IsNullOrEmpty())
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
        /// Returns if there are no, non-empty hashes in common with another Disk
        /// </summary>
        /// <param name="other">Disk to compare against</param>
        /// <returns>True if at least one hash is not mutually exclusive, false otherwise</returns>
        private bool HasCommonHash(Disk other)
        {
            return !(_md5.IsNullOrEmpty() ^ other._md5.IsNullOrEmpty())
#if NET_FRAMEWORK
                || !(_ripemd160.IsNullOrEmpty() || other._ripemd160.IsNullOrEmpty())
#endif
                || !(_sha1.IsNullOrEmpty() ^ other._sha1.IsNullOrEmpty())
                || !(_sha256.IsNullOrEmpty() ^ other._sha256.IsNullOrEmpty())
                || !(_sha384.IsNullOrEmpty() ^ other._sha384.IsNullOrEmpty())
                || !(_sha512.IsNullOrEmpty() ^ other._sha512.IsNullOrEmpty());
        }

        /// <summary>
        /// Returns if the Disk contains any hashes
        /// </summary>
        /// <returns>True if any hash exists, false otherwise</returns>
        private bool HasHashes()
        {
            return !_md5.IsNullOrEmpty()
#if NET_FRAMEWORK
                || !_ripemd160.IsNullOrEmpty()
#endif
                || !_sha1.IsNullOrEmpty()
                || !_sha256.IsNullOrEmpty()
                || !_sha384.IsNullOrEmpty()
                || !_sha512.IsNullOrEmpty();
        }

        /// <summary>
        /// Returns if any hashes are common with another Disk
        /// </summary>
        /// <param name="other">Disk to compare against</param>
        /// <returns>True if any hashes are in common, false otherwise</returns>
        private bool HashMatch(Disk other)
        {
            // If either have no hashes, we return false, otherwise this would be a false positive
            if (!HasHashes() || !other.HasHashes())
                return false;

            // If neither have hashes in common, we return false, otherwise this would be a false positive
            if (!HasCommonHash(other))
                return false;

            // Return if all hashes match according to merge rules
            return ConditionalHashEquals(_md5, other._md5)
#if NET_FRAMEWORK
                && ConditionalHashEquals(_ripemd160, other._ripemd160)
#endif
                && ConditionalHashEquals(_sha1, other._sha1)
                && ConditionalHashEquals(_sha256, other._sha256)
                && ConditionalHashEquals(_sha384, other._sha384)
                && ConditionalHashEquals(_sha512, other._sha512);
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
        public override string GetKey(BucketedBy bucketedBy, bool lower = true, bool norename = true)
        {
            // Set the output key as the default blank string
            string key = string.Empty;

            // Now determine what the key should be based on the bucketedBy value
            switch (bucketedBy)
            {
                case BucketedBy.MD5:
                    key = MD5;
                    break;

#if NET_FRAMEWORK
                case BucketedBy.RIPEMD160:
                    key = RIPEMD160;
                    break;
#endif

                case BucketedBy.SHA1:
                    key = SHA1;
                    break;

                case BucketedBy.SHA256:
                    key = SHA256;
                    break;

                case BucketedBy.SHA384:
                    key = SHA384;
                    break;

                case BucketedBy.SHA512:
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
        /// <param name="updateFields">List of Fields representing what should be updated</param>
        public override void ReplaceFields(DatItem item, List<Field> updateFields)
        {
            // Replace common fields first
            base.ReplaceFields(item, updateFields);

            // If we don't have a Disk to replace from, ignore specific fields
            if (item.ItemType != ItemType.Disk)
                return;

            // Cast for easier access
            Disk newItem = item as Disk;

            // Replace the fields
            if (updateFields.Contains(Field.MD5))
            {
                if (string.IsNullOrEmpty(MD5) && !string.IsNullOrEmpty(newItem.MD5))
                    MD5 = newItem.MD5;
            }

#if NET_FRAMEWORK
            if (updateFields.Contains(Field.RIPEMD160))
            {
                if (string.IsNullOrEmpty(RIPEMD160) && !string.IsNullOrEmpty(newItem.RIPEMD160))
                    RIPEMD160 = newItem.RIPEMD160;
            }
#endif

            if (updateFields.Contains(Field.SHA1))
            {
                if (string.IsNullOrEmpty(SHA1) && !string.IsNullOrEmpty(newItem.SHA1))
                    SHA1 = newItem.SHA1;
            }

            if (updateFields.Contains(Field.SHA256))
            {
                if (string.IsNullOrEmpty(SHA256) && !string.IsNullOrEmpty(newItem.SHA256))
                    SHA256 = newItem.SHA256;
            }

            if (updateFields.Contains(Field.SHA384))
            {
                if (string.IsNullOrEmpty(SHA384) && !string.IsNullOrEmpty(newItem.SHA384))
                    SHA384 = newItem.SHA384;
            }

            if (updateFields.Contains(Field.SHA512))
            {
                if (string.IsNullOrEmpty(SHA512) && !string.IsNullOrEmpty(newItem.SHA512))
                    SHA512 = newItem.SHA512;
            }

            if (updateFields.Contains(Field.Merge))
                MergeTag = newItem.MergeTag;

            if (updateFields.Contains(Field.Region))
                Region = newItem.Region;

            if (updateFields.Contains(Field.Index))
                Index = newItem.Index;

            if (updateFields.Contains(Field.Writable))
                Writable = newItem.Writable;

            if (updateFields.Contains(Field.Status))
                ItemStatus = newItem.ItemStatus;

            if (updateFields.Contains(Field.Optional))
                Optional = newItem.Optional;
        }

        #endregion
    }
}
