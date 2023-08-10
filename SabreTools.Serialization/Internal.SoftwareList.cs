using System.Collections.Generic;
using System.Linq;
using SabreTools.Models.Internal;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for SoftwareList models to internal structure
    /// </summary>
    public partial class Internal
    {
        #region Serialize

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.SoftwareList"/> to <cref="Header"/>
        /// </summary>
        public static Header ConvertHeaderFromSoftwareList(Models.SoftwareList.SoftwareList item)
        {
            var header = new Header
            {
                [Header.NameKey] = item.Name,
                [Header.DescriptionKey] = item.Description,
                [Header.NotesKey] = item.Notes,
            };
            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.Software"/> to <cref="Machine"/>
        /// </summary>
        public static Machine ConvertMachineFromSoftwareList(Models.SoftwareList.Software item)
        {
            var machine = new Machine
            {
                [Machine.NameKey] = item.Name,
                [Machine.CloneOfKey] = item.CloneOf,
                [Machine.SupportedKey] = item.Supported,
                [Machine.DescriptionKey] = item.Description,
                [Machine.YearKey] = item.Year,
                [Machine.PublisherKey] = item.Publisher,
                [Machine.NotesKey] = item.Notes,
            };

            if (item.Info != null && item.Info.Any())
            {
                var infos = new List<Info>();
                foreach (var info in item.Info)
                {
                    infos.Add(ConvertFromSoftwareList(info));
                }
                machine[Machine.InfoKey] = infos.ToArray();
            }

            if (item.SharedFeat != null && item.SharedFeat.Any())
            {
                var sharedFeats = new List<SharedFeat>();
                foreach (var sharedFeat in item.SharedFeat)
                {
                    sharedFeats.Add(ConvertFromSoftwareList(sharedFeat));
                }
                machine[Machine.SharedFeatKey] = sharedFeats.ToArray();
            }

            if (item.Part != null && item.Part.Any())
            {
                var parts = new List<Part>();
                foreach (var part in item.Part)
                {
                    parts.Add(ConvertFromSoftwareList(part));
                }
                machine[Machine.PartKey] = parts.ToArray();
            }

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.DataArea"/> to <cref="DataArea"/>
        /// </summary>
        public static DataArea ConvertFromSoftwareList(Models.SoftwareList.DataArea item)
        {
            var dataArea = new DataArea
            {
                [DataArea.NameKey] = item.Name,
                [DataArea.SizeKey] = item.Size,
                [DataArea.WidthKey] = item.Width,
                [DataArea.EndiannessKey] = item.Endianness,
            };

            if (item.Rom != null && item.Rom.Any())
            {
                var roms = new List<Rom>();
                foreach (var rom in item.Rom)
                {
                    roms.Add(ConvertFromSoftwareList(rom));
                }
                dataArea[DataArea.RomKey] = roms.ToArray();
            }

            return dataArea;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.DipSwitch"/> to <cref="DipSwitch"/>
        /// </summary>
        public static DipSwitch ConvertFromSoftwareList(Models.SoftwareList.DipSwitch item)
        {
            var dipSwitch = new DipSwitch
            {
                [DipSwitch.NameKey] = item.Name,
                [DipSwitch.TagKey] = item.Tag,
                [DipSwitch.MaskKey] = item.Mask,
            };

            if (item.DipValue != null && item.DipValue.Any())
            {
                var dipValues = new List<DipValue>();
                foreach (var dipValue in item.DipValue)
                {
                    dipValues.Add(ConvertFromSoftwareList(dipValue));
                }
                dipSwitch[DipSwitch.DipValueKey] = dipValues.ToArray();
            }

            return dipSwitch;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.DipValue"/> to <cref="DipValue"/>
        /// </summary>
        public static DipValue ConvertFromSoftwareList(Models.SoftwareList.DipValue item)
        {
            var dipValue = new DipValue
            {
                [DipValue.NameKey] = item.Name,
                [DipValue.ValueKey] = item.Value,
                [DipValue.DefaultKey] = item.Default,
            };
            return dipValue;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.Disk"/> to <cref="Disk"/>
        /// </summary>
        public static Disk ConvertFromSoftwareList(Models.SoftwareList.Disk item)
        {
            var disk = new Disk
            {
                [Disk.NameKey] = item.Name,
                [Disk.MD5Key] = item.MD5,
                [Disk.SHA1Key] = item.SHA1,
                [Disk.StatusKey] = item.Status,
                [Disk.WritableKey] = item.Writeable,
            };
            return disk;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.DiskArea"/> to <cref="DiskArea"/>
        /// </summary>
        public static DiskArea ConvertFromSoftwareList(Models.SoftwareList.DiskArea item)
        {
            var diskArea = new DiskArea
            {
                [DiskArea.NameKey] = item.Name,
            };

            if (item.Disk != null && item.Disk.Any())
            {
                var roms = new List<Disk>();
                foreach (var disk in item.Disk)
                {
                    roms.Add(ConvertFromSoftwareList(disk));
                }
                diskArea[DiskArea.DiskKey] = roms.ToArray();
            }

            return diskArea;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.Feature"/> to <cref="Feature"/>
        /// </summary>
        public static Feature ConvertFromSoftwareList(Models.SoftwareList.Feature item)
        {
            var feature = new Feature
            {
                [Feature.NameKey] = item.Name,
                [Feature.ValueKey] = item.Value,
            };
            return feature;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.Info"/> to <cref="Info"/>
        /// </summary>
        public static Info ConvertFromSoftwareList(Models.SoftwareList.Info item)
        {
            var info = new Info
            {
                [Info.NameKey] = item.Name,
                [Info.ValueKey] = item.Value,
            };
            return info;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.Part"/> to <cref="Part"/>
        /// </summary>
        public static Part ConvertFromSoftwareList(Models.SoftwareList.Part item)
        {
            var part = new Part
            {
                [Part.NameKey] = item.Name,
                [Part.InterfaceKey] = item.Interface,
            };

            if (item.Feature != null && item.Feature.Any())
            {
                var features = new List<Feature>();
                foreach (var feature in item.Feature)
                {
                    features.Add(ConvertFromSoftwareList(feature));
                }
                part[Part.FeatureKey] = features.ToArray();
            }

            if (item.DataArea != null && item.DataArea.Any())
            {
                var dataAreas = new List<DataArea>();
                foreach (var dataArea in item.DataArea)
                {
                    dataAreas.Add(ConvertFromSoftwareList(dataArea));
                }
                part[Part.DataAreaKey] = dataAreas.ToArray();
            }

            if (item.DiskArea != null && item.DiskArea.Any())
            {
                var diskAreas = new List<DiskArea>();
                foreach (var diskArea in item.DiskArea)
                {
                    diskAreas.Add(ConvertFromSoftwareList(diskArea));
                }
                part[Part.DiskAreaKey] = diskAreas.ToArray();
            }

            if (item.DipSwitch != null && item.DipSwitch.Any())
            {
                var dipSwitches = new List<DipSwitch>();
                foreach (var rom in item.DipSwitch)
                {
                    dipSwitches.Add(ConvertFromSoftwareList(rom));
                }
                part[Part.DipSwitchKey] = dipSwitches.ToArray();
            }

            return part;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.Rom"/> to <cref="Rom"/>
        /// </summary>
        public static Rom ConvertFromSoftwareList(Models.SoftwareList.Rom item)
        {
            var rom = new Rom
            {
                [Rom.NameKey] = item.Name,
                [Rom.SizeKey] = item.Size,
                [Rom.LengthKey] = item.Length,
                [Rom.CRCKey] = item.CRC,
                [Rom.SHA1Key] = item.SHA1,
                [Rom.OffsetKey] = item.Offset,
                [Rom.ValueKey] = item.Value,
                [Rom.StatusKey] = item.Status,
                [Rom.LoadFlagKey] = item.LoadFlag,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.SoftwareList.SharedFeat"/> to <cref="SharedFeat"/>
        /// </summary>
        public static SharedFeat ConvertFromSoftwareList(Models.SoftwareList.SharedFeat item)
        {
            var sharedFeat = new SharedFeat
            {
                [SharedFeat.NameKey] = item.Name,
                [SharedFeat.ValueKey] = item.Value,
            };
            return sharedFeat;
        }

        #endregion

        #region Deserialize

        /// <summary>
        /// Convert from <cref="Header"/> to <cref="Models.SoftwareList.SoftwareList"/>
        /// </summary>
        public static Models.SoftwareList.SoftwareList? ConvertHeaderToSoftwareList(Header? item)
        {
            if (item == null)
                return null;

            var softwareList = new Models.SoftwareList.SoftwareList
            {
                Name = item.ReadString(Header.NameKey),
                Description = item.ReadString(Header.DescriptionKey),
                Notes = item.ReadString(Header.NotesKey),
            };
            return softwareList;
        }

        /// <summary>
        /// Convert from <cref="Machine"/> to <cref="Models.SoftwareList.Software"/>
        /// </summary>
        public static Models.SoftwareList.Software? ConvertMachineToSoftwareList(Machine? item)
        {
            if (item == null)
                return null;
            
            var software = new Models.SoftwareList.Software
            {
                Name = item.ReadString(Machine.NameKey),
                CloneOf = item.ReadString(Machine.CloneOfKey),
                Supported = item.ReadString(Machine.SupportedKey),
                Description = item.ReadString(Machine.DescriptionKey),
                Year = item.ReadString(Machine.YearKey),
                Publisher = item.ReadString(Machine.PublisherKey),
                Notes = item.ReadString(Machine.NotesKey),
            };

            var infos = item.Read<Info[]>(Machine.InfoKey);
            software.Info = infos?.Select(ConvertToSoftwareList)?.ToArray();

            var sharedFeats = item.Read<SharedFeat[]>(Machine.SharedFeatKey);
            software.SharedFeat = sharedFeats?.Select(ConvertToSoftwareList)?.ToArray();

            var parts = item.Read<Part[]>(Machine.PartKey);
            software.Part = parts?.Select(ConvertToSoftwareList)?.ToArray();

            return software;
        }

        /// <summary>
        /// Convert from <cref="DataArea"/> to <cref="Models.SoftwareList.DataArea"/>
        /// </summary>
        private static Models.SoftwareList.DataArea? ConvertToSoftwareList(DataArea? item)
        {
            if (item == null)
                return null;
            
            var dataArea = new Models.SoftwareList.DataArea
            {
                Name = item.ReadString(DataArea.NameKey),
                Size = item.ReadString(DataArea.SizeKey),
                Width = item.ReadString(DataArea.WidthKey),
                Endianness = item.ReadString(DataArea.EndiannessKey),
            };

            var roms = item.Read<Rom[]>(DataArea.RomKey);
            dataArea.Rom = roms?.Select(ConvertToSoftwareList)?.ToArray();

            return dataArea;
        }

        /// <summary>
        /// Convert from <cref="DipSwitch"/> to <cref="Models.SoftwareList.DipSwitch"/>
        /// </summary>
        private static Models.SoftwareList.DipSwitch? ConvertToSoftwareList(DipSwitch? item)
        {
            if (item == null)
                return null;
            
            var dipSwitch = new Models.SoftwareList.DipSwitch
            {
                Name = item.ReadString(DipSwitch.NameKey),
                Tag = item.ReadString(DipSwitch.TagKey),
                Mask = item.ReadString(DipSwitch.MaskKey),
            };

            var dipValues = item.Read<DipValue[]>(DipSwitch.DipValueKey);
            dipSwitch.DipValue = dipValues?.Select(ConvertToSoftwareList)?.ToArray();

            return dipSwitch;
        }

        /// <summary>
        /// Convert from <cref="DipValue"/> to <cref="Models.SoftwareList.DipValue"/>
        /// </summary>
        private static Models.SoftwareList.DipValue? ConvertToSoftwareList(DipValue? item)
        {
            if (item == null)
                return null;
            
            var dipValue = new Models.SoftwareList.DipValue
            {
                Name = item.ReadString(DipValue.NameKey),
                Value = item.ReadString(DipValue.ValueKey),
                Default = item.ReadString(DipValue.DefaultKey),
            };
            return dipValue;
        }

        /// <summary>
        /// Convert from <cref="Disk"/> to <cref="Models.SoftwareList.Disk"/>
        /// </summary>
        private static Models.SoftwareList.Disk? ConvertToSoftwareList(Disk? item)
        {
            if (item == null)
                return null;
            
            var disk = new Models.SoftwareList.Disk
            {
                Name = item.ReadString(Disk.NameKey),
                MD5 = item.ReadString(Disk.MD5Key),
                SHA1 = item.ReadString(Disk.SHA1Key),
                Status = item.ReadString(Disk.StatusKey),
                Writeable = item.ReadString(Disk.WritableKey),
            };
            return disk;
        }

        /// <summary>
        /// Convert from <cref="DiskArea"/> to <cref="Models.SoftwareList.DiskArea"/>
        /// </summary>
        private static Models.SoftwareList.DiskArea? ConvertToSoftwareList(DiskArea? item)
        {
            if (item == null)
                return null;
            
            var diskArea = new Models.SoftwareList.DiskArea
            {
                Name = item.ReadString(DiskArea.NameKey),
            };

            var disks = item.Read<Disk[]>(DiskArea.DiskKey);
            diskArea.Disk = disks?.Select(ConvertToSoftwareList)?.ToArray();

            return diskArea;
        }

        /// <summary>
        /// Convert from <cref="Feature"/> to <cref="Models.SoftwareList.Feature"/>
        /// </summary>
        private static Models.SoftwareList.Feature? ConvertToSoftwareList(Feature? item)
        {
            if (item == null)
                return null;
            
            var feature = new Models.SoftwareList.Feature
            {
                Name = item.ReadString(Feature.NameKey),
                Value = item.ReadString(Feature.ValueKey),
            };
            return feature;
        }

        /// <summary>
        /// Convert from <cref="Info"/> to <cref="Models.SoftwareList.Info"/>
        /// </summary>
        private static Models.SoftwareList.Info? ConvertToSoftwareList(Info? item)
        {
            if (item == null)
                return null;
            
            var info = new Models.SoftwareList.Info
            {
                Name = item.ReadString(Info.NameKey),
                Value = item.ReadString(Info.ValueKey),
            };
            return info;
        }

        /// <summary>
        /// Convert from <cref="Part"/> to <cref="Models.SoftwareList.Part"/>
        /// </summary>
        private static Models.SoftwareList.Part? ConvertToSoftwareList(Part? item)
        {
            if (item == null)
                return null;
            
            var part = new Models.SoftwareList.Part
            {
                Name = item.ReadString(Part.NameKey),
                Interface = item.ReadString(Part.InterfaceKey),
            };

            var features = item.Read<Feature[]>(Part.FeatureKey);
            part.Feature = features?.Select(ConvertToSoftwareList)?.ToArray();

            var dataAreas = item.Read<DataArea[]>(Part.DataAreaKey);
            part.DataArea = dataAreas?.Select(ConvertToSoftwareList)?.ToArray();

            var diskAreas = item.Read<DiskArea[]>(Part.DiskAreaKey);
            part.DiskArea = diskAreas?.Select(ConvertToSoftwareList)?.ToArray();

            var dipSwitches = item.Read<DipSwitch[]>(Part.DipSwitchKey);
            part.DipSwitch = dipSwitches?.Select(ConvertToSoftwareList)?.ToArray();

            return part;
        }

        /// <summary>
        /// Convert from <cref="Rom"/> to <cref="Models.SoftwareList.Rom"/>
        /// </summary>
        private static Models.SoftwareList.Rom? ConvertToSoftwareList(Rom? item)
        {
            if (item == null)
                return null;
            
            var rom = new Models.SoftwareList.Rom
            {
                Name = item.ReadString(Rom.NameKey),
                Size = item.ReadString(Rom.SizeKey),
                Length = item.ReadString(Rom.LengthKey),
                CRC = item.ReadString(Rom.CRCKey),
                SHA1 = item.ReadString(Rom.SHA1Key),
                Offset = item.ReadString(Rom.OffsetKey),
                Value = item.ReadString(Rom.ValueKey),
                Status = item.ReadString(Rom.StatusKey),
                LoadFlag = item.ReadString(Rom.LoadFlagKey),
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="SharedFeat"/> to <cref="Models.SoftwareList.SharedFeat"/>
        /// </summary>
        private static Models.SoftwareList.SharedFeat? ConvertToSoftwareList(SharedFeat? item)
        {
            if (item == null)
                return null;
            
            var sharedFeat = new Models.SoftwareList.SharedFeat
            {
                Name = item.ReadString(SharedFeat.NameKey),
                Value = item.ReadString(SharedFeat.ValueKey),
            };
            return sharedFeat;
        }

        #endregion
    }
}