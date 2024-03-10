using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.FileTypes;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents a generic file within a set
    /// </summary>
    [JsonObject("rom"), XmlRoot("rom")]
    public class Rom : DatItem
    {
        #region Fields

        [JsonIgnore]
        public bool ItemStatusSpecified
        {
            get
            {
                var status = GetFieldValue<ItemStatus>(Models.Metadata.Rom.StatusKey);
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
                var dataArea = GetFieldValue<DataArea?>("DATAAREA");
                return dataArea != null
                    && (!string.IsNullOrEmpty(dataArea.GetName())
                        || dataArea.GetFieldValue<long?>(Models.Metadata.DataArea.SizeKey) != null
                        || dataArea.GetFieldValue<long?>(Models.Metadata.DataArea.WidthKey) != null
                        || dataArea.GetFieldValue<Endianness>(Models.Metadata.DataArea.EndiannessKey) != Endianness.NULL);
            }
        }

        [JsonIgnore]
        public bool PartSpecified
        {
            get
            {
                var part = GetFieldValue<Part?>("PART");
                return part != null
                    && (!string.IsNullOrEmpty(part.GetName())
                        || !string.IsNullOrEmpty(part.GetFieldValue<string?>(Models.Metadata.Part.InterfaceKey)));
            }
        }

        #endregion // Fields

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => GetFieldValue<string>(Models.Metadata.Rom.NameKey);

        /// <inheritdoc/>
        public override void SetName(string? name) => SetFieldValue(Models.Metadata.Rom.NameKey, name);

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Rom object
        /// </summary>
        public Rom()
        {
            _internal = new Models.Metadata.Rom();
            Machine = new Machine();

            SetName(null);
            ItemType = ItemType.Rom;
            DupeType = 0x00;
            SetFieldValue<ItemStatus>(Models.Metadata.Rom.StatusKey, ItemStatus.None);
        }

        /// <summary>
        /// Create a "blank" Rom object
        /// </summary>
        /// <param name="name"></param>
        /// <param name="machineName"></param>
        /// <param name="omitFromScan"></param>
        public Rom(string name, string machineName)
        {
            _internal = new Models.Metadata.Rom();
            SetName(name);
            ItemType = ItemType.Rom;
            SetFieldValue<long?>(Models.Metadata.Rom.SizeKey, null);
            SetFieldValue<ItemStatus>(Models.Metadata.Rom.StatusKey, ItemStatus.None);

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
            _internal = new Models.Metadata.Rom();
            Machine = new Machine();

            SetName(baseFile.Filename);
            SetFieldValue<long?>(Models.Metadata.Rom.SizeKey, baseFile.Size);
            SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, TextHelper.ByteArrayToString(baseFile.CRC));
            SetFieldValue<string?>(Models.Metadata.Rom.MD5Key, TextHelper.ByteArrayToString(baseFile.MD5));
            SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, TextHelper.ByteArrayToString(baseFile.SHA1));
            SetFieldValue<string?>(Models.Metadata.Rom.SHA256Key, TextHelper.ByteArrayToString(baseFile.SHA256));
            SetFieldValue<string?>(Models.Metadata.Rom.SHA384Key, TextHelper.ByteArrayToString(baseFile.SHA384));
            SetFieldValue<string?>(Models.Metadata.Rom.SHA512Key, TextHelper.ByteArrayToString(baseFile.SHA512));
            SetFieldValue<string?>(Models.Metadata.Rom.SpamSumKey, System.Text.Encoding.UTF8.GetString(baseFile.SpamSum ?? []));

            ItemType = ItemType.Rom;
            DupeType = 0x00;
            SetFieldValue<ItemStatus>(Models.Metadata.Rom.StatusKey, ItemStatus.None);
            SetFieldValue<string?>(Models.Metadata.Rom.DateKey, baseFile.Date);
        }

        /// <summary>
        /// Create a Rom object from the internal model
        /// </summary>
        public Rom(Models.Metadata.Rom? item)
        {
            _internal = item ?? [];
            Machine = new Machine();

            ItemType = ItemType.Rom;
            DupeType = 0x00;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Rom()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.Rom ?? [],
            };
        }

        /// <summary>
        /// Convert Rom object to a BaseFile
        /// </summary>
        public BaseFile ConvertToBaseFile()
        {
            return new BaseFile()
            {
                Filename = GetName(),
                Parent = this.Machine.Name,
                Date = GetFieldValue<string?>(Models.Metadata.Rom.DateKey),
                Size = GetFieldValue<long?>(Models.Metadata.Rom.SizeKey),
                CRC = TextHelper.StringToByteArray(GetFieldValue<string?>(Models.Metadata.Rom.CRCKey)),
                MD5 = TextHelper.StringToByteArray(GetFieldValue<string?>(Models.Metadata.Rom.MD5Key)),
                SHA1 = TextHelper.StringToByteArray(GetFieldValue<string?>(Models.Metadata.Rom.SHA1Key)),
                SHA256 = TextHelper.StringToByteArray(GetFieldValue<string?>(Models.Metadata.Rom.SHA256Key)),
                SHA384 = TextHelper.StringToByteArray(GetFieldValue<string?>(Models.Metadata.Rom.SHA384Key)),
                SHA512 = TextHelper.StringToByteArray(GetFieldValue<string?>(Models.Metadata.Rom.SHA512Key)),
                SpamSum = System.Text.Encoding.UTF8.GetBytes(GetFieldValue<string?>(Models.Metadata.Rom.SpamSumKey) ?? string.Empty),
            };
        }

        #endregion

        #region Comparision Methods

        /// <summary>
        /// Fill any missing size and hash information from another Rom
        /// </summary>
        /// <param name="other">Rom to fill information from</param>
        public void FillMissingInformation(Rom other) => _internal.FillMissingHashes(other?._internal);

        /// <summary>
        /// Get unique duplicate suffix on name collision
        /// </summary>
        /// <returns>String representing the suffix</returns>
        public string GetDuplicateSuffix() => _internal.GetDuplicateSuffix();

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
                    key = GetFieldValue<string?>(Models.Metadata.Rom.CRCKey);
                    break;

                case ItemKey.MD5:
                    key = GetFieldValue<string?>(Models.Metadata.Rom.MD5Key);
                    break;

                case ItemKey.SHA1:
                    key = GetFieldValue<string?>(Models.Metadata.Rom.SHA1Key);
                    break;

                case ItemKey.SHA256:
                    key = GetFieldValue<string?>(Models.Metadata.Rom.SHA256Key);
                    break;

                case ItemKey.SHA384:
                    key = GetFieldValue<string?>(Models.Metadata.Rom.SHA384Key);
                    break;

                case ItemKey.SHA512:
                    key = GetFieldValue<string?>(Models.Metadata.Rom.SHA512Key);
                    break;

                case ItemKey.SpamSum:
                    key = GetFieldValue<string?>(Models.Metadata.Rom.SpamSumKey);
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
