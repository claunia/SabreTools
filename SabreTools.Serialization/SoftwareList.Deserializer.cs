using System.Linq;
using SabreTools.Models.SoftwareList;

namespace SabreTools.Serialization
{
    /// <summary>
    /// XML deserializer for MAME softwarelist files
    /// </summary>
    public partial class SoftawreList : XmlSerializer<SoftwareList>
    {
        #region Internal

        /// <summary>
        /// Convert from <cref="Models.Internal.MetadataFile"/> to <cref="Models.SoftawreList.SoftwareList"/>
        /// </summary>
        public static SoftwareList? ConvertFromInternalModel(Models.Internal.MetadataFile? item)
        {
            if (item == null)
                return null;

            var header = item.Read<Models.Internal.Header>(Models.Internal.MetadataFile.HeaderKey);
            var metadataFile = header != null ? ConvertHeaderFromInternalModel(header) : new SoftwareList();

            var machines = item.Read<Models.Internal.Machine[]>(Models.Internal.MetadataFile.MachineKey);
            if (machines != null && machines.Any())
            {
                metadataFile.Software = machines
                    .Where(m => m != null)
                    .Select(ConvertMachineFromInternalModel)
                    .ToArray();
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Header"/> to <cref="Models.SoftwareList.SoftwareList"/>
        /// </summary>
        private static SoftwareList ConvertHeaderFromInternalModel(Models.Internal.Header item)
        {
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
        private static Software ConvertMachineFromInternalModel(Models.Internal.Machine item)
        {
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
            if (infos != null && infos.Any())
            {
                software.Info = infos
                    .Where(i => i != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var sharedFeats = item.Read<Models.Internal.SharedFeat[]>(Models.Internal.Machine.SharedFeatKey);
            if (sharedFeats != null && sharedFeats.Any())
            {
                software.SharedFeat = sharedFeats
                    .Where(s => s != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var parts = item.Read<Models.Internal.Part[]>(Models.Internal.Machine.PartKey);
            if (parts != null && parts.Any())
            {
                software.Part = parts
                    .Where(p => p != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            return software;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.DataArea"/> to <cref="Models.SoftwareList.DataArea"/>
        /// </summary>
        private static DataArea ConvertFromInternalModel(Models.Internal.DataArea item)
        {
            var dataArea = new DataArea
            {
                Name = item.ReadString(Models.Internal.DataArea.NameKey),
                Size = item.ReadString(Models.Internal.DataArea.SizeKey),
                Width = item.ReadString(Models.Internal.DataArea.WidthKey),
                Endianness = item.ReadString(Models.Internal.DataArea.EndiannessKey),
            };

            var roms = item.Read<Models.Internal.Rom[]>(Models.Internal.DataArea.RomKey);
            if (roms != null && roms.Any())
            {
                dataArea.Rom = roms
                    .Where(r => r != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            return dataArea;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.DipSwitch"/> to <cref="Models.SoftwareList.DipSwitch"/>
        /// </summary>
        private static DipSwitch ConvertFromInternalModel(Models.Internal.DipSwitch item)
        {
            var dipSwitch = new DipSwitch
            {
                Name = item.ReadString(Models.Internal.DipSwitch.NameKey),
                Tag = item.ReadString(Models.Internal.DipSwitch.TagKey),
                Mask = item.ReadString(Models.Internal.DipSwitch.MaskKey),
            };

            var dipValues = item.Read<Models.Internal.DipValue[]>(Models.Internal.DipSwitch.DipValueKey);
            if (dipValues != null && dipValues.Any())
            {
                dipSwitch.DipValue = dipValues
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            return dipSwitch;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.DipValue"/> to <cref="Models.SoftwareList.DipValue"/>
        /// </summary>
        private static DipValue ConvertFromInternalModel(Models.Internal.DipValue item)
        {
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
        private static Disk ConvertFromInternalModel(Models.Internal.Disk item)
        {
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
        private static DiskArea ConvertFromInternalModel(Models.Internal.DiskArea item)
        {
            var diskArea = new DiskArea
            {
                Name = item.ReadString(Models.Internal.DiskArea.NameKey),
            };

            var disks = item.Read<Models.Internal.Disk[]>(Models.Internal.DiskArea.DiskKey);
            if (disks != null && disks.Any())
            {
                diskArea.Disk = disks
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            return diskArea;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Feature"/> to <cref="Models.SoftwareList.Feature"/>
        /// </summary>
        private static Feature ConvertFromInternalModel(Models.Internal.Feature item)
        {
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
        private static Info ConvertFromInternalModel(Models.Internal.Info item)
        {
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
        private static Part ConvertFromInternalModel(Models.Internal.Part item)
        {
            var part = new Part
            {
                Name = item.ReadString(Models.Internal.Part.NameKey),
                Interface = item.ReadString(Models.Internal.Part.InterfaceKey),
            };

            var features = item.Read<Models.Internal.Feature[]>(Models.Internal.Part.FeatureKey);
            if (features != null && features.Any())
            {
                part.Feature = features
                    .Where(f => f != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var dataAreas = item.Read<Models.Internal.DataArea[]>(Models.Internal.Part.DataAreaKey);
            if (dataAreas != null && dataAreas.Any())
            {
                part.DataArea = dataAreas
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var diskAreas = item.Read<Models.Internal.DiskArea[]>(Models.Internal.Part.DiskAreaKey);
            if (diskAreas != null && diskAreas.Any())
            {
                part.DiskArea = diskAreas
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var dipSwitches = item.Read<Models.Internal.DipSwitch[]>(Models.Internal.Part.DipSwitchKey);
            if (dipSwitches != null && dipSwitches.Any())
            {
                part.DipSwitch = dipSwitches
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            return part;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.SoftwareList.Rom"/>
        /// </summary>
        private static Rom ConvertFromInternalModel(Models.Internal.Rom item)
        {
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
        private static SharedFeat ConvertFromInternalModel(Models.Internal.SharedFeat item)
        {
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