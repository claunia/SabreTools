using System.Collections.Generic;
using System.Linq;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for Listrom models to internal structure
    /// </summary>
    public partial class Internal
    {
        #region Serialize

        /// <summary>
        /// Convert from <cref="Models.Listrom.Set"/> to <cref="Models.Internal.Machine"/>
        /// </summary>
        public static Models.Internal.Machine ConvertMachineFromListrom(Models.Listrom.Set item)
        {
            var machine = new Models.Internal.Machine();
            if (!string.IsNullOrWhiteSpace(item.Device))
            {
                machine[Models.Internal.Machine.NameKey] = item.Device;
                machine[Models.Internal.Machine.IsDeviceKey] = "yes";
            }
            else
            {
                machine[Models.Internal.Machine.NameKey] = item.Driver;
            }

            if (item.Row != null && item.Row.Any())
            {
                var datItems = new List<Models.Internal.DatItem>();
                foreach (var file in item.Row)
                {
                    datItems.Add(ConvertFromListrom(file));
                }

                machine[Models.Internal.Machine.DiskKey] = datItems.Where(i => i.ReadString(Models.Internal.DatItem.TypeKey) == "disk").ToArray();
                machine[Models.Internal.Machine.RomKey] = datItems.Where(i => i.ReadString(Models.Internal.DatItem.TypeKey) == "rom").ToArray();
            }

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.Listrom.Row"/> to <cref="Models.Internal.DatItem"/>
        /// </summary>
        public static Models.Internal.DatItem ConvertFromListrom(Models.Listrom.Row item)
        {
            if (item.Size == null)
            {
                var disk = new Models.Internal.Disk
                {
                    [Models.Internal.Disk.NameKey] = item.Name,
                    [Models.Internal.Disk.MD5Key] = item.MD5,
                    [Models.Internal.Disk.SHA1Key] = item.SHA1,
                };

                if (item.NoGoodDumpKnown)
                    disk[Models.Internal.Disk.StatusKey] = "nodump";
                else if (item.Bad)
                    disk[Models.Internal.Disk.StatusKey] = "baddump";

                return disk;
            }
            else
            {
                var rom = new Models.Internal.Rom
                {
                    [Models.Internal.Rom.NameKey] = item.Name,
                    [Models.Internal.Rom.SizeKey] = item.Size,
                    [Models.Internal.Rom.CRCKey] = item.CRC,
                    [Models.Internal.Rom.SHA1Key] = item.SHA1,
                };

                if (item.NoGoodDumpKnown)
                    rom[Models.Internal.Rom.StatusKey] = "nodump";
                else if (item.Bad)
                    rom[Models.Internal.Rom.StatusKey] = "baddump";

                return rom;
            }
        }

        #endregion

        #region Deserialize

        /// <summary>
        /// Convert from <cref="Models.Internal.Machine"/> to <cref="Models.Listrom.Set"/>
        /// </summary>
        public static Models.Listrom.Set ConvertMachineToListrom(Models.Internal.Machine item)
        {
            var set = new Models.Listrom.Set();
            if (item.ReadString(Models.Internal.Machine.IsDeviceKey) == "yes")
                set.Device = item.ReadString(Models.Internal.Machine.NameKey);
            else
                set.Driver = item.ReadString(Models.Internal.Machine.NameKey);

            var rowItems = new List<Models.Listrom.Row>();

            if (item.ContainsKey(Models.Internal.Machine.RomKey) && item[Models.Internal.Machine.RomKey] is Models.Internal.Rom[] roms)
            {
                foreach (var rom in roms)
                {
                    rowItems.Add(ConvertToListrom(rom));
                }
            }

            if (item.ContainsKey(Models.Internal.Machine.DiskKey) && item[Models.Internal.Machine.DiskKey] is Models.Internal.Disk[] disks)
            {
                foreach (var disk in disks)
                {
                    rowItems.Add(ConvertToListrom(disk));
                }
            }

            if (rowItems.Any())
                set.Row = rowItems.ToArray();

            return set;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Disk"/> to <cref="Models.Listrom.Row"/>
        /// </summary>
        public static Models.Listrom.Row ConvertToListrom(Models.Internal.Disk item)
        {
            var row = new Models.Listrom.Row
            {
                Name = item.ReadString(Models.Internal.Disk.NameKey),
                MD5 = item.ReadString(Models.Internal.Disk.MD5Key),
                SHA1 = item.ReadString(Models.Internal.Disk.SHA1Key),
            };

            if (item[Models.Internal.Disk.StatusKey] as string == "nodump")
                row.NoGoodDumpKnown = true;
            else if (item[Models.Internal.Disk.StatusKey] as string == "baddump")
                row.Bad = true;

            return row;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.Listrom.Row"/>
        /// </summary>
        public static Models.Listrom.Row ConvertToListrom(Models.Internal.Rom item)
        {
            var row = new Models.Listrom.Row
            {
                Name = item.ReadString(Models.Internal.Rom.NameKey),
                Size = item.ReadString(Models.Internal.Rom.SizeKey),
                CRC = item.ReadString(Models.Internal.Rom.CRCKey),
                SHA1 = item.ReadString(Models.Internal.Rom.SHA1Key),
            };

            if (item[Models.Internal.Rom.StatusKey] as string == "nodump")
                row.NoGoodDumpKnown = true;
            else if (item[Models.Internal.Rom.StatusKey] as string == "baddump")
                row.Bad = true;

            return row;
        }

        #endregion
    }
}