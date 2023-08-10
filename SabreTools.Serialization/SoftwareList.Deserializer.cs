using System.Linq;
using SabreTools.Models.SoftwareList;

namespace SabreTools.Serialization
{
    /// <summary>
    /// XML deserializer for MAME softwarelist files
    /// </summary>
    public partial class SoftawreList : XmlSerializer<SoftwareList>
    {
        // TODO: Add deserialization of entire SoftwareList
        #region Deserialize

        /// <summary>
        /// Convert from <cref="Models.Internal.Header"/> to <cref="Models.SoftwareList.SoftwareList"/>
        /// </summary>
        public static SoftwareList? ConvertHeaderFromInternalModel(Models.Internal.Header? item)
        {
            if (item == null)
                return null;

            var softwareList = new SoftwareList
            {
                Name = item.ReadString(Models.Internal.Header.NameKey),
                Description = item.ReadString(Models.Internal.Header.DescriptionKey),
                Notes = item.ReadString(Models.Internal.Header.NotesKey),
            };
            return softwareList;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Machine"/> to <cref="Models.SoftwareList.Software"/>
        /// </summary>
        public static Software? ConvertMachineFromInternalModel(Models.Internal.Machine? item)
        {
            if (item == null)
                return null;
            
            var software = new Software
            {
                Name = item.ReadString(Models.Internal.Machine.NameKey),
                CloneOf = item.ReadString(Models.Internal.Machine.CloneOfKey),
                Supported = item.ReadString(Models.Internal.Machine.SupportedKey),
                Description = item.ReadString(Models.Internal.Machine.DescriptionKey),
                Year = item.ReadString(Models.Internal.Machine.YearKey),
                Publisher = item.ReadString(Models.Internal.Machine.PublisherKey),
                Notes = item.ReadString(Models.Internal.Machine.NotesKey),
            };

            var infos = item.Read<Models.Internal.Info[]>(Models.Internal.Machine.InfoKey);
            software.Info = infos?.Select(ConvertFromInternalModel)?.ToArray();

            var sharedFeats = item.Read<Models.Internal.SharedFeat[]>(Models.Internal.Machine.SharedFeatKey);
            software.SharedFeat = sharedFeats?.Select(ConvertFromInternalModel)?.ToArray();

            var parts = item.Read<Models.Internal.Part[]>(Models.Internal.Machine.PartKey);
            software.Part = parts?.Select(ConvertFromInternalModel)?.ToArray();

            return software;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.DataArea"/> to <cref="Models.SoftwareList.DataArea"/>
        /// </summary>
        private static DataArea? ConvertFromInternalModel(Models.Internal.DataArea? item)
        {
            if (item == null)
                return null;
            
            var dataArea = new DataArea
            {
                Name = item.ReadString(Models.Internal.DataArea.NameKey),
                Size = item.ReadString(Models.Internal.DataArea.SizeKey),
                Width = item.ReadString(Models.Internal.DataArea.WidthKey),
                Endianness = item.ReadString(Models.Internal.DataArea.EndiannessKey),
            };

            var roms = item.Read<Models.Internal.Rom[]>(Models.Internal.DataArea.RomKey);
            dataArea.Rom = roms?.Select(ConvertFromInternalModel)?.ToArray();

            return dataArea;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.DipSwitch"/> to <cref="Models.SoftwareList.DipSwitch"/>
        /// </summary>
        private static DipSwitch? ConvertFromInternalModel(Models.Internal.DipSwitch? item)
        {
            if (item == null)
                return null;
            
            var dipSwitch = new DipSwitch
            {
                Name = item.ReadString(Models.Internal.DipSwitch.NameKey),
                Tag = item.ReadString(Models.Internal.DipSwitch.TagKey),
                Mask = item.ReadString(Models.Internal.DipSwitch.MaskKey),
            };

            var dipValues = item.Read<Models.Internal.DipValue[]>(Models.Internal.DipSwitch.DipValueKey);
            dipSwitch.DipValue = dipValues?.Select(ConvertFromInternalModel)?.ToArray();

            return dipSwitch;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.DipValue"/> to <cref="Models.SoftwareList.DipValue"/>
        /// </summary>
        private static DipValue? ConvertFromInternalModel(Models.Internal.DipValue? item)
        {
            if (item == null)
                return null;
            
            var dipValue = new DipValue
            {
                Name = item.ReadString(Models.Internal.DipValue.NameKey),
                Value = item.ReadString(Models.Internal.DipValue.ValueKey),
                Default = item.ReadString(Models.Internal.DipValue.DefaultKey),
            };
            return dipValue;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Disk"/> to <cref="Models.SoftwareList.Disk"/>
        /// </summary>
        private static Disk? ConvertFromInternalModel(Models.Internal.Disk? item)
        {
            if (item == null)
                return null;
            
            var disk = new Disk
            {
                Name = item.ReadString(Models.Internal.Disk.NameKey),
                MD5 = item.ReadString(Models.Internal.Disk.MD5Key),
                SHA1 = item.ReadString(Models.Internal.Disk.SHA1Key),
                Status = item.ReadString(Models.Internal.Disk.StatusKey),
                Writeable = item.ReadString(Models.Internal.Disk.WritableKey),
            };
            return disk;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.DiskArea"/> to <cref="Models.SoftwareList.DiskArea"/>
        /// </summary>
        private static DiskArea? ConvertFromInternalModel(Models.Internal.DiskArea? item)
        {
            if (item == null)
                return null;
            
            var diskArea = new DiskArea
            {
                Name = item.ReadString(Models.Internal.DiskArea.NameKey),
            };

            var disks = item.Read<Models.Internal.Disk[]>(Models.Internal.DiskArea.DiskKey);
            diskArea.Disk = disks?.Select(ConvertFromInternalModel)?.ToArray();

            return diskArea;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Feature"/> to <cref="Models.SoftwareList.Feature"/>
        /// </summary>
        private static Feature? ConvertFromInternalModel(Models.Internal.Feature? item)
        {
            if (item == null)
                return null;
            
            var feature = new Feature
            {
                Name = item.ReadString(Models.Internal.Feature.NameKey),
                Value = item.ReadString(Models.Internal.Feature.ValueKey),
            };
            return feature;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Info"/> to <cref="Models.SoftwareList.Info"/>
        /// </summary>
        private static Info? ConvertFromInternalModel(Models.Internal.Info? item)
        {
            if (item == null)
                return null;
            
            var info = new Info
            {
                Name = item.ReadString(Models.Internal.Info.NameKey),
                Value = item.ReadString(Models.Internal.Info.ValueKey),
            };
            return info;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Part"/> to <cref="Models.SoftwareList.Part"/>
        /// </summary>
        private static Part? ConvertFromInternalModel(Models.Internal.Part? item)
        {
            if (item == null)
                return null;
            
            var part = new Part
            {
                Name = item.ReadString(Models.Internal.Part.NameKey),
                Interface = item.ReadString(Models.Internal.Part.InterfaceKey),
            };

            var features = item.Read<Models.Internal.Feature[]>(Models.Internal.Part.FeatureKey);
            part.Feature = features?.Select(ConvertFromInternalModel)?.ToArray();

            var dataAreas = item.Read<Models.Internal.DataArea[]>(Models.Internal.Part.DataAreaKey);
            part.DataArea = dataAreas?.Select(ConvertFromInternalModel)?.ToArray();

            var diskAreas = item.Read<Models.Internal.DiskArea[]>(Models.Internal.Part.DiskAreaKey);
            part.DiskArea = diskAreas?.Select(ConvertFromInternalModel)?.ToArray();

            var dipSwitches = item.Read<Models.Internal.DipSwitch[]>(Models.Internal.Part.DipSwitchKey);
            part.DipSwitch = dipSwitches?.Select(ConvertFromInternalModel)?.ToArray();

            return part;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.SoftwareList.Rom"/>
        /// </summary>
        private static Rom? ConvertFromInternalModel(Models.Internal.Rom? item)
        {
            if (item == null)
                return null;
            
            var rom = new Rom
            {
                Name = item.ReadString(Models.Internal.Rom.NameKey),
                Size = item.ReadString(Models.Internal.Rom.SizeKey),
                Length = item.ReadString(Models.Internal.Rom.LengthKey),
                CRC = item.ReadString(Models.Internal.Rom.CRCKey),
                SHA1 = item.ReadString(Models.Internal.Rom.SHA1Key),
                Offset = item.ReadString(Models.Internal.Rom.OffsetKey),
                Value = item.ReadString(Models.Internal.Rom.ValueKey),
                Status = item.ReadString(Models.Internal.Rom.StatusKey),
                LoadFlag = item.ReadString(Models.Internal.Rom.LoadFlagKey),
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.SharedFeat"/> to <cref="Models.SoftwareList.SharedFeat"/>
        /// </summary>
        private static SharedFeat? ConvertFromInternalModel(Models.Internal.SharedFeat? item)
        {
            if (item == null)
                return null;
            
            var sharedFeat = new SharedFeat
            {
                Name = item.ReadString(Models.Internal.SharedFeat.NameKey),
                Value = item.ReadString(Models.Internal.SharedFeat.ValueKey),
            };
            return sharedFeat;
        }

        #endregion
    }
}