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
        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => GetFieldValue<string>(Models.Metadata.Media.NameKey);

        /// <inheritdoc/>
        public override void SetName(string? name) => SetFieldValue(Models.Metadata.Media.NameKey, name);

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Media object
        /// </summary>
        public Media()
        {
            _internal = new Models.Metadata.Media();

            SetName(string.Empty);
            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.Media);
            SetFieldValue<DupeType>(DatItem.DupeTypeKey, 0x00);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        /// <summary>
        /// Create a Media object from the internal model
        /// </summary>
        public Media(Models.Metadata.Media? item)
        {
            _internal = item ?? [];

            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.Media);
            SetFieldValue<DupeType>(DatItem.DupeTypeKey, 0x00);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        /// <summary>
        /// Create a Media object from a BaseFile
        /// </summary>
        /// <param name="baseFile"></param>
        public Media(BaseFile baseFile)
        {
            _internal = new Models.Metadata.Media();

            SetName(baseFile.Filename);
            SetFieldValue<string?>(Models.Metadata.Media.MD5Key, TextHelper.ByteArrayToString(baseFile.MD5));
            SetFieldValue<string?>(Models.Metadata.Media.SHA1Key, TextHelper.ByteArrayToString(baseFile.SHA1));
            SetFieldValue<string?>(Models.Metadata.Media.SHA256Key, TextHelper.ByteArrayToString(baseFile.SHA256));
            SetFieldValue<string?>(Models.Metadata.Media.SpamSumKey, System.Text.Encoding.UTF8.GetString(baseFile.SpamSum ?? []));

            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.Media);
            SetFieldValue<DupeType>(DatItem.DupeTypeKey, 0x00);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Media()
            {
                _internal = this._internal?.Clone() as Models.Metadata.Media ?? [],
            };
        }

        /// <summary>
        /// Convert Media object to a BaseFile
        /// </summary>
        public BaseFile ConvertToBaseFile()
        {
            return new BaseFile()
            {
                Filename = this.GetName(),
                Parent = GetFieldValue<Machine>(DatItem.MachineKey)!.GetFieldValue<string?>(Models.Metadata.Machine.NameKey),
                MD5 = TextHelper.StringToByteArray(GetFieldValue<string?>(Models.Metadata.Media.MD5Key)),
                SHA1 = TextHelper.StringToByteArray(GetFieldValue<string?>(Models.Metadata.Media.SHA1Key)),
                SHA256 = TextHelper.StringToByteArray(GetFieldValue<string?>(Models.Metadata.Media.SHA256Key)),
                SpamSum = System.Text.Encoding.UTF8.GetBytes(GetFieldValue<string?>(Models.Metadata.Media.SpamSumKey) ?? string.Empty),
            };
        }

        /// <summary>
        /// Convert a media to the closest Rom approximation
        /// </summary>
        /// <returns></returns>
        public Rom ConvertToRom()
        {
            var rom = new Rom(_internal.ConvertToRom());
            rom.SetFieldValue<DupeType>(DatItem.DupeTypeKey, GetFieldValue<DupeType>(DatItem.DupeTypeKey));
            rom.SetFieldValue<Machine>(DatItem.MachineKey, GetFieldValue<Machine>(DatItem.MachineKey));
            rom.SetFieldValue<bool>(DatItem.RemoveKey, GetFieldValue<bool>(DatItem.RemoveKey));
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
                    key = GetFieldValue<string?>(Models.Metadata.Media.MD5Key);
                    break;

                case ItemKey.SHA1:
                    key = GetFieldValue<string?>(Models.Metadata.Media.SHA1Key);
                    break;

                case ItemKey.SHA256:
                    key = GetFieldValue<string?>(Models.Metadata.Media.SHA256Key);
                    break;

                case ItemKey.SpamSum:
                    key = GetFieldValue<string?>(Models.Metadata.Media.SpamSumKey);
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
