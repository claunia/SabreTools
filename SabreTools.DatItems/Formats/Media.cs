using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents Aaruformat images which use internal hashes
    /// </summary>
    [JsonObject("media"), XmlRoot("media")]
    public sealed class Media : DatItem<Models.Metadata.Media>
    {
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.Media;

        /// <inheritdoc>/>
        protected override string? NameKey => Models.Metadata.Media.NameKey;

        #endregion

        #region Constructors

        public Media() : base()
        {
            SetFieldValue<DupeType>(DatItem.DupeTypeKey, 0x00);
        }

        public Media(Models.Metadata.Media item) : base(item)
        {
            SetFieldValue<DupeType>(DatItem.DupeTypeKey, 0x00);
        }

        #endregion

        #region Cloning Methods

        /// <summary>
        /// Convert a media to the closest Rom approximation
        /// </summary>
        /// <returns></returns>
        public Rom ConvertToRom()
        {
            var rom = new Rom(_internal.ConvertToRom()!);
            rom.SetFieldValue<DupeType>(DatItem.DupeTypeKey, GetFieldValue<DupeType>(DatItem.DupeTypeKey));
            rom.SetFieldValue<Machine>(DatItem.MachineKey, GetFieldValue<Machine>(DatItem.MachineKey));
            rom.SetFieldValue<bool?>(DatItem.RemoveKey, GetBoolFieldValue(DatItem.RemoveKey));
            rom.SetFieldValue<Source?>(DatItem.SourceKey, GetFieldValue<Source?>(DatItem.SourceKey));

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
                    key = GetStringFieldValue(Models.Metadata.Media.MD5Key);
                    break;

                case ItemKey.SHA1:
                    key = GetStringFieldValue(Models.Metadata.Media.SHA1Key);
                    break;

                case ItemKey.SHA256:
                    key = GetStringFieldValue(Models.Metadata.Media.SHA256Key);
                    break;

                case ItemKey.SpamSum:
                    key = GetStringFieldValue(Models.Metadata.Media.SpamSumKey);
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
