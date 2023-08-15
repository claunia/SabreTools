using System;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.FileTypes;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents Aaruformat images which use internal hashes
    /// </summary>
    [JsonObject("media"), XmlRoot("media")]
    public class Media : DatItem
    {
        #region Fields

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name"), XmlElement("name")]
        public string? Name
        {
            get => _internal.ReadString(Models.Internal.Media.NameKey);
            set => _internal[Models.Internal.Media.NameKey] = value;
        }

        /// <summary>
        /// Data MD5 hash
        /// </summary>
        [JsonProperty("md5", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("md5")]
        public string? MD5
        {
            get => _internal.ReadString(Models.Internal.Media.MD5Key);
            set => _internal[Models.Internal.Media.MD5Key] = TextHelper.NormalizeMD5(value);
        }

        /// <summary>
        /// Data SHA-1 hash
        /// </summary>
        [JsonProperty("sha1", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("sha1")]
        public string? SHA1
        {
            get => _internal.ReadString(Models.Internal.Media.SHA1Key);
            set => _internal[Models.Internal.Media.SHA1Key] = TextHelper.NormalizeSHA1(value);
        }

        /// <summary>
        /// Data SHA-256 hash
        /// </summary>
        [JsonProperty("sha256", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("sha256")]
        public string? SHA256
        {
            get => _internal.ReadString(Models.Internal.Media.SHA256Key);
            set => _internal[Models.Internal.Media.SHA256Key] = TextHelper.NormalizeSHA256(value);
        }

        /// <summary>
        /// File SpamSum fuzzy hash
        /// </summary>
        [JsonProperty("spamsum", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("spamsum")]
        public string? SpamSum
        {
            get => _internal.ReadString(Models.Internal.Media.SpamSumKey);
            set => _internal[Models.Internal.Media.SpamSumKey] = value;
        }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Media object
        /// </summary>
        public Media()
        {
            _internal = new Models.Internal.Media();
            Name = string.Empty;
            ItemType = ItemType.Media;
            DupeType = 0x00;
        }

        /// <summary>
        /// Create a Media object from a BaseFile
        /// </summary>
        /// <param name="baseFile"></param>
        public Media(BaseFile baseFile)
        {
            _internal = new Models.Internal.Media();
            Name = baseFile.Filename;
            MD5 = TextHelper.ByteArrayToString(baseFile.MD5);
            SHA1 = TextHelper.ByteArrayToString(baseFile.SHA1);
            SHA256 = TextHelper.ByteArrayToString(baseFile.SHA256);
            SpamSum = System.Text.Encoding.UTF8.GetString(baseFile.SpamSum ?? Array.Empty<byte>());

            ItemType = ItemType.Media;
            DupeType = 0x00;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Media()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine?.Clone() as Machine,
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Internal.Media ?? new Models.Internal.Media(),
            };
        }

        /// <summary>
        /// Convert Media object to a BaseFile
        /// </summary>
        public BaseFile ConvertToBaseFile()
        {
            return new BaseFile()
            {
                Filename = this.Name,
                Parent = this.Machine?.Name,
                MD5 = TextHelper.StringToByteArray(this.MD5),
                SHA1 = TextHelper.StringToByteArray(this.SHA1),
                SHA256 = TextHelper.StringToByteArray(this.SHA256),
                SpamSum = System.Text.Encoding.UTF8.GetBytes(this.SpamSum ?? string.Empty),
            };
        }

        /// <summary>
        /// Convert a media to the closest Rom approximation
        /// </summary>
        /// <returns></returns>
        public Rom ConvertToRom()
        {
            var rom = new Rom(_internal.ConvertToRom())
            {
                ItemType = ItemType.Rom,
                DupeType = this.DupeType,

                Machine = this.Machine?.Clone() as Machine,
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,
            };

            return rom;
        }

        #endregion

        #region Comparision Methods

        /// <summary>
        /// Fill any missing size and hash information from another Media
        /// </summary>
        /// <param name="other">Media to fill information from</param>
        public void FillMissingInformation(Media other) => _internal.FillMissingHashes(other?._internal);

        /// <summary>
        /// Get unique duplicate suffix on name collision
        /// </summary>
        /// <returns>String representing the suffix</returns>
        public string GetDuplicateSuffix() => _internal.GetDuplicateSuffix();

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

                case ItemKey.SHA256:
                    key = SHA256;
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
