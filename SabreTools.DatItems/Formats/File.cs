using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.FileTypes;
using SabreTools.Hashing;
using SabreTools.Matching;

// TODO: Add item mappings for all fields
namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents a single file item
    /// </summary>
    [JsonObject("file"), XmlRoot("file")]
    public class File : DatItem
    {
        #region Private instance variables

        private byte[]? _crc; // 8 bytes
        private byte[]? _md5; // 16 bytes
        private byte[]? _sha1; // 20 bytes
        private byte[]? _sha256; // 32 bytes

        #endregion

        #region Fields

        /// <summary>
        /// ID value
        /// </summary>
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("id")]
        public string? Id { get; set; }

        /// <summary>
        /// Extension value
        /// </summary>
        [JsonProperty("extension", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("extension")]
        public string? Extension { get; set; }

        /// <summary>
        /// Byte size of the rom
        /// </summary>
        [JsonProperty("size", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("size")]
        public long? Size { get; set; } = null;

        /// <summary>
        /// File CRC32 hash
        /// </summary>
        [JsonProperty("crc", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("crc")]
        public string? CRC
        {
            get { return _crc.IsNullOrEmpty() ? null : TextHelper.ByteArrayToString(_crc); }
            set { _crc = (value == "null" ? Constants.CRCZeroBytes : TextHelper.StringToByteArray(TextHelper.NormalizeCRC32(value))); }
        }

        /// <summary>
        /// File MD5 hash
        /// </summary>
        [JsonProperty("md5", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("md5")]
        public string? MD5
        {
            get { return _md5.IsNullOrEmpty() ? null : TextHelper.ByteArrayToString(_md5); }
            set { _md5 = TextHelper.StringToByteArray(TextHelper.NormalizeMD5(value)); }
        }

        /// <summary>
        /// File SHA-1 hash
        /// </summary>
        [JsonProperty("sha1", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("sha1")]
        public string? SHA1
        {
            get { return _sha1.IsNullOrEmpty() ? null : TextHelper.ByteArrayToString(_sha1); }
            set { _sha1 = TextHelper.StringToByteArray(TextHelper.NormalizeSHA1(value)); }
        }

        /// <summary>
        /// File SHA-256 hash
        /// </summary>
        [JsonProperty("sha256", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("sha256")]
        public string? SHA256
        {
            get { return _sha256.IsNullOrEmpty() ? null : TextHelper.ByteArrayToString(_sha256); }
            set { _sha256 = TextHelper.StringToByteArray(TextHelper.NormalizeSHA256(value)); }
        }

        /// <summary>
        /// Format value
        /// </summary>
        [JsonProperty("format", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("format")]
        public string? Format { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty File object
        /// </summary>
        public File()
        {
            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.File);
        }

        /// <summary>
        /// Create a File object from a BaseFile
        /// </summary>
        /// <param name="baseFile"></param>
        public File(BaseFile baseFile)
        {
            _crc = baseFile.CRC;
            _md5 = baseFile.MD5;
            _sha1 = baseFile.SHA1;
            _sha256 = baseFile.SHA256;

            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.File);
            SetFieldValue<DupeType>(DatItem.DupeTypeKey, 0x00);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            var file = new File()
            {
                Id = this.Id,
                Extension = this.Extension,
                Size = this.Size,
                _crc = this._crc,
                _md5 = this._md5,
                _sha1 = this._sha1,
                _sha256 = this._sha256,
                Format = this.Format,
            };
            file.SetFieldValue<DupeType>(DatItem.DupeTypeKey, GetFieldValue<DupeType>(DatItem.DupeTypeKey));
            file.SetFieldValue<Machine>(DatItem.MachineKey, GetFieldValue<Machine>(DatItem.MachineKey)!.Clone() as Machine ?? new Machine());
            file.SetFieldValue<bool>(DatItem.RemoveKey, GetFieldValue<bool>(DatItem.RemoveKey));
            file.SetFieldValue<Source?>(DatItem.SourceKey, GetFieldValue<Source?>(DatItem.SourceKey));

            return file;
        }

        /// <summary>
        /// Convert Disk object to a BaseFile
        /// </summary>
        public BaseFile ConvertToBaseFile()
        {
            return new BaseFile()
            {
                Parent = GetFieldValue<Machine>(DatItem.MachineKey)!.GetFieldValue<string?>(Models.Metadata.Machine.NameKey),
                CRC = this._crc,
                MD5 = this._md5,
                SHA1 = this._sha1,
                SHA256 = this._sha256,
            };
        }

        /// <summary>
        /// Convert a disk to the closest Rom approximation
        /// </summary>
        /// <returns></returns>
        public Rom ConvertToRom()
        {
            var rom = new Rom();
            rom.SetName($"{this.Id}.{this.Extension}");
            rom.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, CRC);
            rom.SetFieldValue<DupeType>(DatItem.DupeTypeKey, GetFieldValue<DupeType>(DatItem.DupeTypeKey));
            rom.SetFieldValue<Machine>(DatItem.MachineKey, GetFieldValue<Machine>(DatItem.MachineKey)!.Clone() as Machine ?? new Machine());
            rom.SetFieldValue<string?>(Models.Metadata.Rom.MD5Key, MD5);
            rom.SetFieldValue<bool>(DatItem.RemoveKey, GetFieldValue<bool>(DatItem.RemoveKey));
            rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, SHA1);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA256Key, SHA256);
            rom.SetFieldValue<Source?>(DatItem.SourceKey, GetFieldValue<Source?>(DatItem.SourceKey));

            return rom;
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            bool dupefound = false;

            // If we don't have a file, return false
            if (GetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey) != other?.GetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey))
                return dupefound;

            // Otherwise, treat it as a File
            File? newOther = other as File;

            // If all hashes are empty, then they're dupes
            if (!HasHashes() && !newOther!.HasHashes())
            {
                dupefound = true;
            }

            // If we have a file that has no known size, rely on the hashes only
            else if (Size == null && HashMatch(newOther!))
            {
                dupefound = true;
            }

            // Otherwise if we get a partial match
            else if (Size == newOther!.Size && HashMatch(newOther))
            {
                dupefound = true;
            }

            return dupefound;
        }

        /// <summary>
        /// Fill any missing size and hash information from another Rom
        /// </summary>
        /// <param name="other">File to fill information from</param>
        public void FillMissingInformation(File other)
        {
            if (Size == null && other.Size != null)
                Size = other.Size;

            if (_crc.IsNullOrEmpty() && !other._crc.IsNullOrEmpty())
                _crc = other._crc;

            if (_md5.IsNullOrEmpty() && !other._md5.IsNullOrEmpty())
                _md5 = other._md5;

            if (_sha1.IsNullOrEmpty() && !other._sha1.IsNullOrEmpty())
                _sha1 = other._sha1;

            if (_sha256.IsNullOrEmpty() && !other._sha256.IsNullOrEmpty())
                _sha256 = other._sha256;
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
            else
                return "_1";
        }

        /// <summary>
        /// Returns if the File contains any hashes
        /// </summary>
        /// <returns>True if any hash exists, false otherwise</returns>
        public bool HasHashes()
        {
            return !_crc.IsNullOrEmpty()
                || !_md5.IsNullOrEmpty()
                || !_sha1.IsNullOrEmpty()
                || !_sha256.IsNullOrEmpty();
        }

        /// <summary>
        /// Returns if all of the hashes are set to their 0-byte values
        /// </summary>
        /// <returns>True if any hash matches the 0-byte value, false otherwise</returns>
        public bool HasZeroHash()
        {
            return (_crc != null && _crc.SequenceEqual(Constants.CRCZeroBytes))
                || (_md5 != null && _md5.SequenceEqual(Constants.MD5ZeroBytes))
                || (_sha1 != null && _sha1.SequenceEqual(Constants.SHA1ZeroBytes))
                || (_sha256 != null && _sha256.SequenceEqual(Constants.SHA256ZeroBytes));
        }

        /// <summary>
        /// Returns if there are no, non-empty hashes in common with another File
        /// </summary>
        /// <param name="other">File to compare against</param>
        /// <returns>True if at least one hash is not mutually exclusive, false otherwise</returns>
        private bool HasCommonHash(File other)
        {
            return !(_crc.IsNullOrEmpty() ^ other._crc.IsNullOrEmpty())
                || !(_md5.IsNullOrEmpty() ^ other._md5.IsNullOrEmpty())
                || !(_sha1.IsNullOrEmpty() ^ other._sha1.IsNullOrEmpty())
                || !(_sha256.IsNullOrEmpty() ^ other._sha256.IsNullOrEmpty());
        }

        /// <summary>
        /// Returns if any hashes are common with another File
        /// </summary>
        /// <param name="other">File to compare against</param>
        /// <returns>True if any hashes are in common, false otherwise</returns>
        private bool HashMatch(File other)
        {
            // If either have no hashes, we return false, otherwise this would be a false positive
            if (!HasHashes() || !other.HasHashes())
                return false;

            // If neither have hashes in common, we return false, otherwise this would be a false positive
            if (!HasCommonHash(other))
                return false;

            // Return if all hashes match according to merge rules
            return Utilities.ConditionalHashEquals(_crc, other._crc)
                && Utilities.ConditionalHashEquals(_md5, other._md5)
                && Utilities.ConditionalHashEquals(_sha1, other._sha1)
                && Utilities.ConditionalHashEquals(_sha256, other._sha256);
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
