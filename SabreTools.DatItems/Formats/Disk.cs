using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.FileTypes;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents Compressed Hunks of Data (CHD) formatted disks which use internal hashes
    /// </summary>
    [JsonObject("disk"), XmlRoot("disk")]
    public class Disk : DatItem
    {
        #region Fields

        [JsonIgnore]
        public bool DiskAreaSpecified
        {
            get
            {
                var diskArea = GetFieldValue<DiskArea?>("DISKAREA");
                return diskArea != null && !string.IsNullOrEmpty(diskArea.GetName());
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

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => GetFieldValue<string>(Models.Metadata.Disk.NameKey);

        /// <inheritdoc/>
        public override void SetName(string? name) => SetFieldValue(Models.Metadata.Disk.NameKey, name);

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Disk object
        /// </summary>
        public Disk()
        {
            _internal = new Models.Metadata.Disk();
            Machine = new Machine();

            SetName(string.Empty);
            ItemType = ItemType.Disk;
            DupeType = 0x00;
            SetFieldValue<ItemStatus>(Models.Metadata.Disk.StatusKey, ItemStatus.None);
        }

        /// <summary>
        /// Create a Disk object from a BaseFile
        /// </summary>
        public Disk(BaseFile baseFile)
        {
            _internal = new Models.Metadata.Disk();
            Machine = new Machine();

            SetName(baseFile.Filename);
            SetFieldValue<string?>(Models.Metadata.Disk.MD5Key, TextHelper.ByteArrayToString(baseFile.MD5));
            SetFieldValue<string?>(Models.Metadata.Disk.SHA1Key, TextHelper.ByteArrayToString(baseFile.SHA1));

            ItemType = ItemType.Disk;
            DupeType = 0x00;
            SetFieldValue<ItemStatus>(Models.Metadata.Disk.StatusKey, ItemStatus.None);
        }

        /// <summary>
        /// Create a Disk object from the internal model
        /// </summary>
        public Disk(Models.Metadata.Disk? item)
        {
            _internal = item ?? [];
            Machine = new Machine();

            ItemType = ItemType.Disk;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Disk()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.Disk ?? [],
            };
        }

        /// <summary>
        /// Convert Disk object to a BaseFile
        /// </summary>
        public BaseFile ConvertToBaseFile()
        {
            return new BaseFile()
            {
                Filename = this.GetName(),
                Parent = this.Machine.GetFieldValue<string?>(Models.Metadata.Machine.NameKey),
                MD5 = TextHelper.StringToByteArray(GetFieldValue<string?>(Models.Metadata.Disk.MD5Key)),
                SHA1 = TextHelper.StringToByteArray(GetFieldValue<string?>(Models.Metadata.Disk.SHA1Key)),
            };
        }

        /// <summary>
        /// Convert a disk to the closest Rom approximation
        /// </summary>
        /// <returns></returns>
        public Rom ConvertToRom()
        {
            var rom = new Rom(_internal.ConvertToRom())
            {
                ItemType = ItemType.Rom,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,
            };

            rom.GetFieldValue<DataArea?>("DATAAREA")?.SetName(this.GetFieldValue<DiskArea?>("DISKAREA")?.GetName());

            return rom;
        }

        #endregion

        #region Comparision Methods

        /// <summary>
        /// Fill any missing size and hash information from another Disk
        /// </summary>
        /// <param name="other">Disk to fill information from</param>
        public void FillMissingInformation(Disk other) => _internal.FillMissingHashes(other?._internal);

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
                    key = GetFieldValue<string?>(Models.Metadata.Disk.MD5Key);
                    break;

                case ItemKey.SHA1:
                    key = GetFieldValue<string?>(Models.Metadata.Disk.SHA1Key);
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
