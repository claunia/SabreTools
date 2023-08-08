using System.Collections.Generic;
using System.Linq;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for SoftwareList models to internal structure
    /// </summary>
    public partial class Internal
    {
        #region Serialize

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.Software"/> to <cref="Models.Internal.Machine"/>
        /// </summary>
        public static Models.Internal.Machine ConvertMachineFromSoftwareList(Models.SoftwareList.Software item)
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
            {
                var infos = new List<Models.Internal.Info>();
                foreach (var info in item.Info)
                {
                    infos.Add(ConvertFromSoftwareList(info));
                }
                machine[Models.Internal.Machine.InfoKey] = infos.ToArray();
            }

            if (item.SharedFeat != null && item.SharedFeat.Any())
            {
                var sharedFeats = new List<Models.Internal.SharedFeat>();
                foreach (var sharedFeat in item.SharedFeat)
                {
                    sharedFeats.Add(ConvertFromSoftwareList(sharedFeat));
                }
                machine[Models.Internal.Machine.SharedFeatKey] = sharedFeats.ToArray();
            }

            if (item.Part != null && item.Part.Any())
            {
                var parts = new List<Models.Internal.Part>();
                foreach (var part in item.Part)
                {
                    parts.Add(ConvertFromSoftwareList(part));
                }
                machine[Models.Internal.Machine.PartKey] = parts.ToArray();
            }

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.DataArea"/> to <cref="Models.Internal.DataArea"/>
        /// </summary>
        public static Models.Internal.DataArea ConvertFromSoftwareList(Models.SoftwareList.DataArea item)
        {
            var dataArea = new Models.Internal.DataArea
            {
                [Models.Internal.DataArea.NameKey] = item.Name,
                [Models.Internal.DataArea.SizeKey] = item.Size,
                [Models.Internal.DataArea.WidthKey] = item.Width,
                [Models.Internal.DataArea.EndiannessKey] = item.Endianness,
            };

            if (item.Rom != null && item.Rom.Any())
            {
                var roms = new List<Models.Internal.Rom>();
                foreach (var rom in item.Rom)
                {
                    roms.Add(ConvertFromSoftwareList(rom));
                }
                dataArea[Models.Internal.DataArea.RomKey] = roms.ToArray();
            }

            return dataArea;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.DipSwitch"/> to <cref="Models.Internal.DipSwitch"/>
        /// </summary>
        public static Models.Internal.DipSwitch ConvertFromSoftwareList(Models.SoftwareList.DipSwitch item)
        {
            var dipSwitch = new Models.Internal.DipSwitch
            {
                [Models.Internal.DipSwitch.NameKey] = item.Name,
                [Models.Internal.DipSwitch.TagKey] = item.Tag,
                [Models.Internal.DipSwitch.MaskKey] = item.Mask,
            };

            if (item.DipValue != null && item.DipValue.Any())
            {
                var dipValues = new List<Models.Internal.DipValue>();
                foreach (var dipValue in item.DipValue)
                {
                    dipValues.Add(ConvertFromSoftwareList(dipValue));
                }
                dipSwitch[Models.Internal.DipSwitch.DipValueKey] = dipValues.ToArray();
            }

            return dipSwitch;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.DipValue"/> to <cref="Models.Internal.DipValue"/>
        /// </summary>
        public static Models.Internal.DipValue ConvertFromSoftwareList(Models.SoftwareList.DipValue item)
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
        public static Models.Internal.Disk ConvertFromSoftwareList(Models.SoftwareList.Disk item)
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
        public static Models.Internal.DiskArea ConvertFromSoftwareList(Models.SoftwareList.DiskArea item)
        {
            var diskArea = new Models.Internal.DiskArea
            {
                [Models.Internal.DiskArea.NameKey] = item.Name,
            };

            if (item.Disk != null && item.Disk.Any())
            {
                var roms = new List<Models.Internal.Disk>();
                foreach (var disk in item.Disk)
                {
                    roms.Add(ConvertFromSoftwareList(disk));
                }
                diskArea[Models.Internal.DiskArea.DiskKey] = roms.ToArray();
            }

            return diskArea;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.Feature"/> to <cref="Models.Internal.Feature"/>
        /// </summary>
        public static Models.Internal.Feature ConvertFromSoftwareList(Models.SoftwareList.Feature item)
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
        public static Models.Internal.Info ConvertFromSoftwareList(Models.SoftwareList.Info item)
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
        public static Models.Internal.Part ConvertFromSoftwareList(Models.SoftwareList.Part item)
        {
            var part = new Models.Internal.Part
            {
                [Models.Internal.Part.NameKey] = item.Name,
                [Models.Internal.Part.InterfaceKey] = item.Interface,
            };

            if (item.Feature != null && item.Feature.Any())
            {
                var features = new List<Models.Internal.Feature>();
                foreach (var feature in item.Feature)
                {
                    features.Add(ConvertFromSoftwareList(feature));
                }
                part[Models.Internal.Part.FeatureKey] = features.ToArray();
            }

            if (item.DataArea != null && item.DataArea.Any())
            {
                var dataAreas = new List<Models.Internal.DataArea>();
                foreach (var dataArea in item.DataArea)
                {
                    dataAreas.Add(ConvertFromSoftwareList(dataArea));
                }
                part[Models.Internal.Part.DataAreaKey] = dataAreas.ToArray();
            }

            if (item.DiskArea != null && item.DiskArea.Any())
            {
                var diskAreas = new List<Models.Internal.DiskArea>();
                foreach (var diskArea in item.DiskArea)
                {
                    diskAreas.Add(ConvertFromSoftwareList(diskArea));
                }
                part[Models.Internal.Part.DiskAreaKey] = diskAreas.ToArray();
            }

            if (item.DipSwitch != null && item.DipSwitch.Any())
            {
                var dipSwitches = new List<Models.Internal.DipSwitch>();
                foreach (var rom in item.DipSwitch)
                {
                    dipSwitches.Add(ConvertFromSoftwareList(rom));
                }
                part[Models.Internal.Part.DipSwitchKey] = dipSwitches.ToArray();
            }

            return part;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.Rom"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Rom ConvertFromSoftwareList(Models.SoftwareList.Rom item)
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
        public static Models.Internal.SharedFeat ConvertFromSoftwareList(Models.SoftwareList.SharedFeat item)
        {
            var sharedFeat = new Models.Internal.SharedFeat
            {
                [Models.Internal.SharedFeat.NameKey] = item.Name,
                [Models.Internal.SharedFeat.ValueKey] = item.Value,
            };
            return sharedFeat;
        }

        #endregion

        #region Deserialize

        /// <summary>
        /// Convert from <cref="Models.Internal.Machine"/> to <cref="Models.SoftwareList.Software"/>
        /// </summary>
        public static Models.SoftwareList.Software? ConvertMachineToSoftwareList(Models.Internal.Machine? item)
        {
            if (item == null)
                return null;
            
            var software = new Models.SoftwareList.Software
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
            software.Info = infos?.Select(ConvertToSoftwareList)?.ToArray();

            var sharedFeats = item.Read<Models.Internal.SharedFeat[]>(Models.Internal.Machine.SharedFeatKey);
            software.SharedFeat = sharedFeats?.Select(ConvertToSoftwareList)?.ToArray();

            var parts = item.Read<Models.Internal.Part[]>(Models.Internal.Machine.PartKey);
            software.Part = parts?.Select(ConvertToSoftwareList)?.ToArray();

            return software;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.DataArea"/> to <cref="Models.SoftwareList.DataArea"/>
        /// </summary>
        private static Models.SoftwareList.DataArea? ConvertToSoftwareList(Models.Internal.DataArea? item)
        {
            if (item == null)
                return null;
            
            var dataArea = new Models.SoftwareList.DataArea
            {
                Name = item.ReadString(Models.Internal.DataArea.NameKey),
                Size = item.ReadString(Models.Internal.DataArea.SizeKey),
                Width = item.ReadString(Models.Internal.DataArea.WidthKey),
                Endianness = item.ReadString(Models.Internal.DataArea.EndiannessKey),
            };

            var roms = item.Read<Models.Internal.Rom[]>(Models.Internal.DataArea.RomKey);
            dataArea.Rom = roms?.Select(ConvertToSoftwareList)?.ToArray();

            return dataArea;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.DipSwitch"/> to <cref="Models.SoftwareList.DipSwitch"/>
        /// </summary>
        private static Models.SoftwareList.DipSwitch? ConvertToSoftwareList(Models.Internal.DipSwitch? item)
        {
            if (item == null)
                return null;
            
            var dipSwitch = new Models.SoftwareList.DipSwitch
            {
                Name = item.ReadString(Models.Internal.DipSwitch.NameKey),
                Tag = item.ReadString(Models.Internal.DipSwitch.TagKey),
                Mask = item.ReadString(Models.Internal.DipSwitch.MaskKey),
            };

            var dipValues = item.Read<Models.Internal.DipValue[]>(Models.Internal.DipSwitch.DipValueKey);
            dipSwitch.DipValue = dipValues?.Select(ConvertToSoftwareList)?.ToArray();

            return dipSwitch;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.DipValue"/> to <cref="Models.SoftwareList.DipValue"/>
        /// </summary>
        private static Models.SoftwareList.DipValue? ConvertToSoftwareList(Models.Internal.DipValue? item)
        {
            if (item == null)
                return null;
            
            var dipValue = new Models.SoftwareList.DipValue
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
        private static Models.SoftwareList.Disk? ConvertToSoftwareList(Models.Internal.Disk? item)
        {
            if (item == null)
                return null;
            
            var disk = new Models.SoftwareList.Disk
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
        private static Models.SoftwareList.DiskArea? ConvertToSoftwareList(Models.Internal.DiskArea? item)
        {
            if (item == null)
                return null;
            
            var diskArea = new Models.SoftwareList.DiskArea
            {
                Name = item.ReadString(Models.Internal.DiskArea.NameKey),
            };

            var disks = item.Read<Models.Internal.Disk[]>(Models.Internal.DiskArea.DiskKey);
            diskArea.Disk = disks?.Select(ConvertToSoftwareList)?.ToArray();

            return diskArea;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Feature"/> to <cref="Models.SoftwareList.Feature"/>
        /// </summary>
        private static Models.SoftwareList.Feature? ConvertToSoftwareList(Models.Internal.Feature? item)
        {
            if (item == null)
                return null;
            
            var feature = new Models.SoftwareList.Feature
            {
                Name = item.ReadString(Models.Internal.Feature.NameKey),
                Value = item.ReadString(Models.Internal.Feature.ValueKey),
            };
            return feature;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Info"/> to <cref="Models.SoftwareList.Info"/>
        /// </summary>
        private static Models.SoftwareList.Info? ConvertToSoftwareList(Models.Internal.Info? item)
        {
            if (item == null)
                return null;
            
            var info = new Models.SoftwareList.Info
            {
                Name = item.ReadString(Models.Internal.Info.NameKey),
                Value = item.ReadString(Models.Internal.Info.ValueKey),
            };
            return info;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Part"/> to <cref="Models.SoftwareList.Part"/>
        /// </summary>
        private static Models.SoftwareList.Part? ConvertToSoftwareList(Models.Internal.Part? item)
        {
            if (item == null)
                return null;
            
            var part = new Models.SoftwareList.Part
            {
                Name = item.ReadString(Models.Internal.Part.NameKey),
                Interface = item.ReadString(Models.Internal.Part.InterfaceKey),
            };

            var features = item.Read<Models.Internal.Feature[]>(Models.Internal.Part.FeatureKey);
            part.Feature = features?.Select(ConvertToSoftwareList)?.ToArray();

            var dataAreas = item.Read<Models.Internal.DataArea[]>(Models.Internal.Part.DataAreaKey);
            part.DataArea = dataAreas?.Select(ConvertToSoftwareList)?.ToArray();

            var diskAreas = item.Read<Models.Internal.DiskArea[]>(Models.Internal.Part.DiskAreaKey);
            part.DiskArea = diskAreas?.Select(ConvertToSoftwareList)?.ToArray();

            var dipSwitches = item.Read<Models.Internal.DipSwitch[]>(Models.Internal.Part.DipSwitchKey);
            part.DipSwitch = dipSwitches?.Select(ConvertToSoftwareList)?.ToArray();

            return part;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.SoftwareList.Rom"/>
        /// </summary>
        private static Models.SoftwareList.Rom? ConvertToSoftwareList(Models.Internal.Rom? item)
        {
            if (item == null)
                return null;
            
            var rom = new Models.SoftwareList.Rom
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
        private static Models.SoftwareList.SharedFeat? ConvertToSoftwareList(Models.Internal.SharedFeat? item)
        {
            if (item == null)
                return null;
            
            var sharedFeat = new Models.SoftwareList.SharedFeat
            {
                Name = item.ReadString(Models.Internal.SharedFeat.NameKey),
                Value = item.ReadString(Models.Internal.SharedFeat.ValueKey),
            };
            return sharedFeat;
        }

        #endregion
    }
}