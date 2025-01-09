using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;
using SabreTools.Core.Tools;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents a generic file within a set
    /// </summary>
    [JsonObject("rom"), XmlRoot("rom")]
    public sealed class Rom : DatItem<Models.Metadata.Rom>
    {
        #region Constants

        /// <summary>
        /// Non-standard key for inverted logic
        /// </summary>
        public const string DataAreaKey = "DATAAREA";

        /// <summary>
        /// Non-standard key for inverted logic
        /// </summary>
        public const string PartKey = "PART";

        #endregion

        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.Rom;

        /// <inheritdoc>/>
        protected override string? NameKey => Models.Metadata.Rom.NameKey;

        [JsonIgnore]
        public bool ItemStatusSpecified
        {
            get
            {
                var status = GetStringFieldValue(Models.Metadata.Rom.StatusKey).AsEnumValue<ItemStatus>();
                return status != ItemStatus.NULL && status != ItemStatus.None;
            }
        }

        [JsonIgnore]
        public bool OriginalSpecified
        {
            get
            {
                var original = GetFieldValue<Original?>("ORIGINAL");
                return original != null && original != default;
            }
        }

        [JsonIgnore]
        public bool DataAreaSpecified
        {
            get
            {
                var dataArea = GetFieldValue<DataArea?>(Rom.DataAreaKey);
                return dataArea != null
                    && (!string.IsNullOrEmpty(dataArea.GetName())
                        || dataArea.GetInt64FieldValue(Models.Metadata.DataArea.SizeKey) != null
                        || dataArea.GetInt64FieldValue(Models.Metadata.DataArea.WidthKey) != null
                        || dataArea.GetStringFieldValue(Models.Metadata.DataArea.EndiannessKey).AsEnumValue<Endianness>() != Endianness.NULL);
            }
        }

        [JsonIgnore]
        public bool PartSpecified
        {
            get
            {
                var part = GetFieldValue<Part?>(Rom.PartKey);
                return part != null
                    && (!string.IsNullOrEmpty(part.GetName())
                        || !string.IsNullOrEmpty(part.GetStringFieldValue(Models.Metadata.Part.InterfaceKey)));
            }
        }

        #endregion

        #region Constructors

        public Rom() : base()
        {
            SetFieldValue<DupeType>(DatItem.DupeTypeKey, 0x00);
            SetFieldValue<string?>(Models.Metadata.Rom.StatusKey, ItemStatus.None.AsStringValue());
        }

        public Rom(Models.Metadata.Rom item) : base(item)
        {
            SetFieldValue<DupeType>(DatItem.DupeTypeKey, 0x00);
        }

        #endregion

        #region Comparision Methods

        /// <summary>
        /// Fill any missing size and hash information from another Rom
        /// </summary>
        /// <param name="other">Rom to fill information from</param>
        public void FillMissingInformation(Rom other)
            => _internal.FillMissingHashes(other._internal);

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
                    key = GetStringFieldValue(Models.Metadata.Rom.CRCKey);
                    break;

                case ItemKey.MD2:
                    key = GetStringFieldValue(Models.Metadata.Rom.MD2Key);
                    break;

                case ItemKey.MD4:
                    key = GetStringFieldValue(Models.Metadata.Rom.MD4Key);
                    break;

                case ItemKey.MD5:
                    key = GetStringFieldValue(Models.Metadata.Rom.MD5Key);
                    break;

                case ItemKey.SHA1:
                    key = GetStringFieldValue(Models.Metadata.Rom.SHA1Key);
                    break;

                case ItemKey.SHA256:
                    key = GetStringFieldValue(Models.Metadata.Rom.SHA256Key);
                    break;

                case ItemKey.SHA384:
                    key = GetStringFieldValue(Models.Metadata.Rom.SHA384Key);
                    break;

                case ItemKey.SHA512:
                    key = GetStringFieldValue(Models.Metadata.Rom.SHA512Key);
                    break;

                case ItemKey.SpamSum:
                    key = GetStringFieldValue(Models.Metadata.Rom.SpamSumKey);
                    break;

                // Let the base handle generic stuff
                default:
                    return base.GetKey(bucketedBy, lower, norename);
            }

            // Double and triple check the key for corner cases
            key ??= string.Empty;
            if (lower)
                key = key.ToLowerInvariant();

            return key;
        }

        /// <inheritdoc/>
        public override string GetKeyDB(ItemKey bucketedBy, Source? source, bool lower = true, bool norename = true)
        {
            // Set the output key as the default blank string
            string? key;

            // Now determine what the key should be based on the bucketedBy value
            switch (bucketedBy)
            {
                case ItemKey.CRC:
                    key = GetStringFieldValue(Models.Metadata.Rom.CRCKey);
                    break;

                case ItemKey.MD2:
                    key = GetStringFieldValue(Models.Metadata.Rom.MD2Key);
                    break;

                case ItemKey.MD4:
                    key = GetStringFieldValue(Models.Metadata.Rom.MD4Key);
                    break;

                case ItemKey.MD5:
                    key = GetStringFieldValue(Models.Metadata.Rom.MD5Key);
                    break;

                case ItemKey.SHA1:
                    key = GetStringFieldValue(Models.Metadata.Rom.SHA1Key);
                    break;

                case ItemKey.SHA256:
                    key = GetStringFieldValue(Models.Metadata.Rom.SHA256Key);
                    break;

                case ItemKey.SHA384:
                    key = GetStringFieldValue(Models.Metadata.Rom.SHA384Key);
                    break;

                case ItemKey.SHA512:
                    key = GetStringFieldValue(Models.Metadata.Rom.SHA512Key);
                    break;

                case ItemKey.SpamSum:
                    key = GetStringFieldValue(Models.Metadata.Rom.SpamSumKey);
                    break;

                // Let the base handle generic stuff
                default:
                    return base.GetKeyDB(bucketedBy, source, lower, norename);
            }

            // Double and triple check the key for corner cases
            key ??= string.Empty;
            if (lower)
                key = key.ToLowerInvariant();

            return key;
        }

        #endregion
    }
}
