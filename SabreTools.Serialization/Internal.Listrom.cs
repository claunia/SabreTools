using System.Collections.Generic;
using System.Linq;
using SabreTools.Models.Internal;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for Listrom models to internal structure
    /// </summary>
    public partial class Internal
    {
        #region Serialize

        /// <summary>
        /// Convert from <cref="Models.Listrom.MetadataFile"/> to <cref="MetadataFile"/>
        /// </summary>
        public static MetadataFile ConvertToInternalModel(Models.Listrom.MetadataFile item)
        {
            var metadataFile = new MetadataFile
            {
                [MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item),
            };

            if (item?.Set != null && item.Set.Any())
                metadataFile[MetadataFile.MachineKey] = item.Set.Select(ConvertMachineToInternalModel).ToArray();

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.Listrom.MetadataFile"/> to <cref="Header"/>
        /// </summary>
        private static Header ConvertHeaderToInternalModel(Models.Listrom.MetadataFile item)
        {
            var header = new Header
            {
                [Header.NameKey] = "MAME Listrom",
            };
            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.Listrom.Set"/> to <cref="Machine"/>
        /// </summary>
        private static Machine ConvertMachineToInternalModel(Models.Listrom.Set item)
        {
            var machine = new Machine();
            if (!string.IsNullOrWhiteSpace(item.Device))
            {
                machine[Machine.NameKey] = item.Device;
                machine[Machine.IsDeviceKey] = "yes";
            }
            else
            {
                machine[Machine.NameKey] = item.Driver;
            }

            if (item.Row != null && item.Row.Any())
            {
                var datItems = new List<DatItem>();
                foreach (var file in item.Row)
                {
                    datItems.Add(ConvertToInternalModel(file));
                }

                machine[Machine.DiskKey] = datItems.Where(i => i.ReadString(DatItem.TypeKey) == "disk")?.ToArray();
                machine[Machine.RomKey] = datItems.Where(i => i.ReadString(DatItem.TypeKey) == "rom")?.ToArray();
            }

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.Listrom.Row"/> to <cref="DatItem"/>
        /// </summary>
        private static DatItem ConvertToInternalModel(Models.Listrom.Row item)
        {
            if (item.Size == null)
            {
                var disk = new Disk
                {
                    [Disk.NameKey] = item.Name,
                    [Disk.MD5Key] = item.MD5,
                    [Disk.SHA1Key] = item.SHA1,
                };

                if (item.NoGoodDumpKnown)
                    disk[Disk.StatusKey] = "nodump";
                else if (item.Bad)
                    disk[Disk.StatusKey] = "baddump";

                return disk;
            }
            else
            {
                var rom = new Rom
                {
                    [Rom.NameKey] = item.Name,
                    [Rom.SizeKey] = item.Size,
                    [Rom.CRCKey] = item.CRC,
                    [Rom.SHA1Key] = item.SHA1,
                };

                if (item.NoGoodDumpKnown)
                    rom[Rom.StatusKey] = "nodump";
                else if (item.Bad)
                    rom[Rom.StatusKey] = "baddump";

                return rom;
            }
        }

        #endregion

        #region Deserialize

        /// <summary>
        /// Convert from <cref="Machine"/> to <cref="Models.Listrom.Set"/>
        /// </summary>
        public static Models.Listrom.Set? ConvertMachineToListrom(Machine? item)
        {
            if (item == null)
                return null;

            var set = new Models.Listrom.Set();
            if (item.ReadString(Machine.IsDeviceKey) == "yes")
                set.Device = item.ReadString(Machine.NameKey);
            else
                set.Driver = item.ReadString(Machine.NameKey);

            var rowItems = new List<Models.Listrom.Row>();

            var roms = item.Read<Rom[]>(Machine.RomKey);
            if (roms != null)
                rowItems.AddRange(roms.Select(ConvertToListrom));

            var disks = item.Read<Disk[]>(Machine.DiskKey);
            if (disks != null)
                rowItems.AddRange(disks.Select(ConvertToListrom));

            set.Row = rowItems.ToArray();
            return set;
        }

        /// <summary>
        /// Convert from <cref="Disk"/> to <cref="Models.Listrom.Row"/>
        /// </summary>
        private static Models.Listrom.Row? ConvertToListrom(Disk? item)
        {
            if (item == null)
                return null;
            
            var row = new Models.Listrom.Row
            {
                Name = item.ReadString(Disk.NameKey),
                MD5 = item.ReadString(Disk.MD5Key),
                SHA1 = item.ReadString(Disk.SHA1Key),
            };

            if (item[Disk.StatusKey] as string == "nodump")
                row.NoGoodDumpKnown = true;
            else if (item[Disk.StatusKey] as string == "baddump")
                row.Bad = true;

            return row;
        }

        /// <summary>
        /// Convert from <cref="Rom"/> to <cref="Models.Listrom.Row"/>
        /// </summary>
        private static Models.Listrom.Row? ConvertToListrom(Rom? item)
        {
            if (item == null)
                return null;
            
            var row = new Models.Listrom.Row
            {
                Name = item.ReadString(Rom.NameKey),
                Size = item.ReadString(Rom.SizeKey),
                CRC = item.ReadString(Rom.CRCKey),
                SHA1 = item.ReadString(Rom.SHA1Key),
            };

            if (item[Rom.StatusKey] as string == "nodump")
                row.NoGoodDumpKnown = true;
            else if (item[Rom.StatusKey] as string == "baddump")
                row.Bad = true;

            return row;
        }

        #endregion
    }
}