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
    public sealed class Disk : DatItem<Models.Metadata.Disk>
    {
        #region Constants

        /// <summary>
        /// Non-standard key for inverted logic
        /// </summary>
        public const string DiskAreaKey = "DISKAREA";

        /// <summary>
        /// Non-standard key for inverted logic
        /// </summary>
        public const string PartKey = "PART";

        #endregion

        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.Disk;

        /// <inheritdoc>/>
        protected override string? NameKey => Models.Metadata.Disk.NameKey;

        [JsonIgnore]
        public bool DiskAreaSpecified
        {
            get
            {
                var diskArea = GetFieldValue<DiskArea?>(Disk.DiskAreaKey);
                return diskArea != null && !string.IsNullOrEmpty(diskArea.GetName());
            }
        }

        [JsonIgnore]
        public bool PartSpecified
        {
            get
            {
                var part = GetFieldValue<Part?>(Disk.PartKey);
                return part != null
                    && (!string.IsNullOrEmpty(part.GetName())
                        || !string.IsNullOrEmpty(part.GetStringFieldValue(Models.Metadata.Part.InterfaceKey)));
            }
        }

        #endregion

        #region Constructors

        public Disk() : base()
        {
            SetFieldValue<DupeType>(DatItem.DupeTypeKey, 0x00);
            SetFieldValue<string?>(Models.Metadata.Disk.StatusKey, ItemStatus.None.AsStringValue());
        }

        public Disk(Models.Metadata.Disk item) : base(item)
        {
            SetFieldValue<DupeType>(DatItem.DupeTypeKey, 0x00);
        }

        public Disk(BaseFile baseFile) : base()
        {
            SetName(baseFile.Filename);
            SetFieldValue<string?>(Models.Metadata.Disk.MD5Key, TextHelper.ByteArrayToString(baseFile.MD5));
            SetFieldValue<string?>(Models.Metadata.Disk.SHA1Key, TextHelper.ByteArrayToString(baseFile.SHA1));

            SetFieldValue<DupeType>(DatItem.DupeTypeKey, 0x00);
            SetFieldValue<string?>(Models.Metadata.Disk.StatusKey, ItemStatus.None.AsStringValue());
        }

        #endregion

        #region Cloning Methods

        /// <summary>
        /// Convert Disk object to a BaseFile
        /// </summary>
        public BaseFile ConvertToBaseFile()
        {
            return new BaseFile()
            {
                Filename = this.GetName(),
                Parent = GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey),
                MD5 = TextHelper.StringToByteArray(GetStringFieldValue(Models.Metadata.Disk.MD5Key)),
                SHA1 = TextHelper.StringToByteArray(GetStringFieldValue(Models.Metadata.Disk.SHA1Key)),
            };
        }

        /// <summary>
        /// Convert a disk to the closest Rom approximation
        /// </summary>
        /// <returns></returns>
        public Rom ConvertToRom()
        {
            var rom = new Rom(_internal.ConvertToRom()!);
            rom.GetFieldValue<DataArea?>(Rom.DataAreaKey)?.SetName(this.GetFieldValue<DiskArea?>(Disk.DiskAreaKey)?.GetName());
            rom.SetFieldValue<DupeType>(DatItem.DupeTypeKey, GetFieldValue<DupeType>(DatItem.DupeTypeKey));
            rom.SetFieldValue<Machine>(DatItem.MachineKey, GetFieldValue<Machine>(DatItem.MachineKey)!.Clone() as Machine ?? new Machine());
            rom.SetFieldValue<bool?>(DatItem.RemoveKey, GetBoolFieldValue(DatItem.RemoveKey));
            rom.SetFieldValue<Source?>(DatItem.SourceKey, GetFieldValue<Source?>(DatItem.SourceKey));

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
                    key = GetStringFieldValue(Models.Metadata.Disk.MD5Key);
                    break;

                case ItemKey.SHA1:
                    key = GetStringFieldValue(Models.Metadata.Disk.SHA1Key);
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
