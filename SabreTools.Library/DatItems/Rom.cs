using System.Linq;

using SabreTools.Library.Data;
using SabreTools.Library.FileTypes;
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

        #region Publicly facing variables

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

        #region Constructors

        /// <summary>
        /// Create a default, empty Rom object
        /// </summary>
        public Rom()
        {
            this.Name = string.Empty;
            this.ItemType = ItemType.Rom;
            this.DupeType = 0x00;
            this.ItemStatus = ItemStatus.None;
            this.Date = string.Empty;
        }

        /// <summary>
        /// Create a "blank" Rom object
        /// </summary>
        /// <param name="name"></param>
        /// <param name="machineName"></param>
        /// <param name="omitFromScan"></param>
        public Rom(string name, string machineName)
        {
            this.Name = name;
            this.ItemType = ItemType.Rom;
            this.Size = -1;
            this.ItemStatus = ItemStatus.None;

            _machine = new Machine
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
            this.Name = baseFile.Filename;
            this.Size = baseFile.Size ?? -1;
            _crc = baseFile.CRC;
            _md5 = baseFile.MD5;
#if NET_FRAMEWORK
            _ripemd160 = baseFile.RIPEMD160;
#endif
            _sha1 = baseFile.SHA1;
            _sha256 = baseFile.SHA256;
            _sha384 = baseFile.SHA384;
            _sha512 = baseFile.SHA512;

            this.ItemType = ItemType.Rom;
            this.DupeType = 0x00;
            this.ItemStatus = ItemStatus.None;
            this.Date = baseFile.Date;
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
                ItemStatus = this.ItemStatus,
                Date = this.Date,
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
    }
}
