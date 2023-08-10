using System.IO;
using System.Linq;
using SabreTools.Models.SoftwareList;

namespace SabreTools.Serialization
{
    /// <summary>
    /// XML serializer for MAME softwarelist files
    /// </summary>
    public partial class SoftawreList : XmlSerializer<SoftwareList>
    {
        /// <inheritdoc cref="SerializeToFile(SoftwareList, string, string?, string?, string?, string?)" />
        public static bool SerializeToFileWithDocType(SoftwareList obj, string path)
            => SerializeToFile(obj, path, DocTypeName, DocTypePubId, DocTypeSysId, DocTypeSysId);

        /// <inheritdoc cref="SerializeToStream(SoftwareList, string?, string?, string?, string?)" />
        public static Stream? SerializeToStreamWithDocType(SoftwareList obj, string path)
            => SerializeToStream(obj, DocTypeName, DocTypePubId, DocTypeSysId, DocTypeSysId);

        #region Internal

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.SoftwareList"/> to <cref="Models.Internal.MetadataFile"/>
        /// </summary>
        public static Models.Internal.MetadataFile ConvertToInternalModel(SoftwareList item)
        {
            var metadataFile = new Models.Internal.MetadataFile
            {
                [Models.Internal.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item),
            };

            if (item?.Software != null && item.Software.Any())
                metadataFile[Models.Internal.MetadataFile.MachineKey] = item.Software.Select(ConvertMachineToInternalModel).ToArray();

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.SoftwareList"/> to <cref="Models.Internal.Header"/>
        /// </summary>
        private static Models.Internal.Header ConvertHeaderToInternalModel(SoftwareList item)
        {
            var header = new Models.Internal.Header
            {
                [Models.Internal.Header.NameKey] = item.Name,
                [Models.Internal.Header.DescriptionKey] = item.Description,
                [Models.Internal.Header.NotesKey] = item.Notes,
            };
            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.Software"/> to <cref="Models.Internal.Machine"/>
        /// </summary>
        private static Models.Internal.Machine ConvertMachineToInternalModel(Software item)
        {
            var machine = new Models.Internal.Machine
            {
                [Models.Internal.Machine.NameKey] = item.Name,
                [Models.Internal.Machine.CloneOfKey] = item.CloneOf,
                [Models.Internal.Machine.SupportedKey] = item.Supported,
                [Models.Internal.Machine.DescriptionKey] = item.Description,
                [Models.Internal.Machine.YearKey] = item.Year,
                [Models.Internal.Machine.PublisherKey] = item.Publisher,
                [Models.Internal.Machine.NotesKey] = item.Notes,
            };

            if (item.Info != null && item.Info.Any())
                machine[Models.Internal.Machine.InfoKey] = item.Info.Select(ConvertToInternalModel).ToArray();

            if (item.SharedFeat != null && item.SharedFeat.Any())
                machine[Models.Internal.Machine.SharedFeatKey] = item.SharedFeat.Select(ConvertToInternalModel).ToArray();

            if (item.Part != null && item.Part.Any())
                machine[Models.Internal.Machine.PartKey] = item.Part.Select(ConvertToInternalModel).ToArray();

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.DataArea"/> to <cref="Models.Internal.DataArea"/>
        /// </summary>
        private static Models.Internal.DataArea ConvertToInternalModel(DataArea item)
        {
            var dataArea = new Models.Internal.DataArea
            {
                [Models.Internal.DataArea.NameKey] = item.Name,
                [Models.Internal.DataArea.SizeKey] = item.Size,
                [Models.Internal.DataArea.WidthKey] = item.Width,
                [Models.Internal.DataArea.EndiannessKey] = item.Endianness,
            };

            if (item.Rom != null && item.Rom.Any())
                dataArea[Models.Internal.DataArea.RomKey] = item.Rom.Select(ConvertToInternalModel).ToArray();

            return dataArea;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.DipSwitch"/> to <cref="Models.Internal.DipSwitch"/>
        /// </summary>
        private static Models.Internal.DipSwitch ConvertToInternalModel(DipSwitch item)
        {
            var dipSwitch = new Models.Internal.DipSwitch
            {
                [Models.Internal.DipSwitch.NameKey] = item.Name,
                [Models.Internal.DipSwitch.TagKey] = item.Tag,
                [Models.Internal.DipSwitch.MaskKey] = item.Mask,
            };

            if (item.DipValue != null && item.DipValue.Any())
                dipSwitch[Models.Internal.DipSwitch.DipValueKey] = item.DipValue.Select(ConvertToInternalModel).ToArray();

            return dipSwitch;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.DipValue"/> to <cref="Models.Internal.DipValue"/>
        /// </summary>
        private static Models.Internal.DipValue ConvertToInternalModel(DipValue item)
        {
            var dipValue = new Models.Internal.DipValue
            {
                [Models.Internal.DipValue.NameKey] = item.Name,
                [Models.Internal.DipValue.ValueKey] = item.Value,
                [Models.Internal.DipValue.DefaultKey] = item.Default,
            };
            return dipValue;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.Disk"/> to <cref="Models.Internal.Disk"/>
        /// </summary>
        private static Models.Internal.Disk ConvertToInternalModel(Disk item)
        {
            var disk = new Models.Internal.Disk
            {
                [Models.Internal.Disk.NameKey] = item.Name,
                [Models.Internal.Disk.MD5Key] = item.MD5,
                [Models.Internal.Disk.SHA1Key] = item.SHA1,
                [Models.Internal.Disk.StatusKey] = item.Status,
                [Models.Internal.Disk.WritableKey] = item.Writeable,
            };
            return disk;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.DiskArea"/> to <cref="Models.Internal.DiskArea"/>
        /// </summary>
        private static Models.Internal.DiskArea ConvertToInternalModel(DiskArea item)
        {
            var diskArea = new Models.Internal.DiskArea
            {
                [Models.Internal.DiskArea.NameKey] = item.Name,
            };

            if (item.Disk != null && item.Disk.Any())
                diskArea[Models.Internal.DiskArea.DiskKey] = item.Disk.Select(ConvertToInternalModel).ToArray();

            return diskArea;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.Feature"/> to <cref="Models.Internal.Feature"/>
        /// </summary>
        private static Models.Internal.Feature ConvertToInternalModel(Feature item)
        {
            var feature = new Models.Internal.Feature
            {
                [Models.Internal.Feature.NameKey] = item.Name,
                [Models.Internal.Feature.ValueKey] = item.Value,
            };
            return feature;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.Info"/> to <cref="Models.Internal.Info"/>
        /// </summary>
        private static Models.Internal.Info ConvertToInternalModel(Info item)
        {
            var info = new Models.Internal.Info
            {
                [Models.Internal.Info.NameKey] = item.Name,
                [Models.Internal.Info.ValueKey] = item.Value,
            };
            return info;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.Part"/> to <cref="Models.Internal.Part"/>
        /// </summary>
        private static Models.Internal.Part ConvertToInternalModel(Part item)
        {
            var part = new Models.Internal.Part
            {
                [Models.Internal.Part.NameKey] = item.Name,
                [Models.Internal.Part.InterfaceKey] = item.Interface,
            };

            if (item.Feature != null && item.Feature.Any())
                part[Models.Internal.Part.FeatureKey] = item.Feature.Select(ConvertToInternalModel).ToArray();

            if (item.DataArea != null && item.DataArea.Any())
                part[Models.Internal.Part.DataAreaKey] = item.DataArea.Select(ConvertToInternalModel).ToArray();

            if (item.DiskArea != null && item.DiskArea.Any())
                part[Models.Internal.Part.DiskAreaKey] = item.DiskArea.Select(ConvertToInternalModel).ToArray();

            if (item.DipSwitch != null && item.DipSwitch.Any())
                part[Models.Internal.Part.DipSwitchKey] = item.DipSwitch.Select(ConvertToInternalModel).ToArray();

            return part;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.Rom"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        private static Models.Internal.Rom ConvertToInternalModel(Rom item)
        {
            var rom = new Models.Internal.Rom
            {
                [Models.Internal.Rom.NameKey] = item.Name,
                [Models.Internal.Rom.SizeKey] = item.Size,
                [Models.Internal.Rom.LengthKey] = item.Length,
                [Models.Internal.Rom.CRCKey] = item.CRC,
                [Models.Internal.Rom.SHA1Key] = item.SHA1,
                [Models.Internal.Rom.OffsetKey] = item.Offset,
                [Models.Internal.Rom.ValueKey] = item.Value,
                [Models.Internal.Rom.StatusKey] = item.Status,
                [Models.Internal.Rom.LoadFlagKey] = item.LoadFlag,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.SharedFeat"/> to <cref="Models.Internal.SharedFeat"/>
        /// </summary>
        private static Models.Internal.SharedFeat ConvertToInternalModel(SharedFeat item)
        {
            var sharedFeat = new Models.Internal.SharedFeat
            {
                [Models.Internal.SharedFeat.NameKey] = item.Name,
                [Models.Internal.SharedFeat.ValueKey] = item.Value,
            };
            return sharedFeat;
        }

        #endregion
    }
}