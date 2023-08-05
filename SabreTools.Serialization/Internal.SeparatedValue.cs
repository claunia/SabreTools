using System.Collections.Generic;
using System.Linq;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for SeparatedValue models to internal structure
    /// </summary>
    public partial class Internal
    {
        #region Serialize

        /// <summary>
        /// Convert from <cref="Models.SeparatedValue.Row"/> to <cref="Models.Internal.Machine"/>
        /// </summary>
        public static Models.Internal.Machine ConvertMachineFromSeparatedValue(Models.SeparatedValue.Row item)
        {
            var machine = new Models.Internal.Machine
            {
                [Models.Internal.Machine.NameKey] = item.GameName,
                [Models.Internal.Machine.DescriptionKey] = item.GameDescription,
            };

            var datItem = ConvertFromSeparatedValue(item);
            switch (datItem)
            {
                case Models.Internal.Disk disk:
                    machine[Models.Internal.Machine.DiskKey] = new Models.Internal.Disk[] { disk };
                    break;

                case Models.Internal.Media media:
                    machine[Models.Internal.Machine.MediaKey] = new Models.Internal.Media[] { media };
                    break;
                    
                case Models.Internal.Rom rom:
                    machine[Models.Internal.Machine.RomKey] = new Models.Internal.Rom[] { rom };
                    break;
            }

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.SeparatedValue.Row"/> to <cref="Models.Internal.DatItem"/>
        /// </summary>
        public static Models.Internal.DatItem? ConvertFromSeparatedValue(Models.SeparatedValue.Row item)
        {
            return item.Type switch
            {
                "disk" => new Models.Internal.Disk
                {
                    [Models.Internal.Disk.NameKey] = item.DiskName,
                    [Models.Internal.Disk.MD5Key] = item.MD5,
                    [Models.Internal.Disk.SHA1Key] = item.SHA1,
                    [Models.Internal.Disk.StatusKey] = item.Status,
                },
                "media" => new Models.Internal.Media
                {
                    [Models.Internal.Media.NameKey] = item.DiskName,
                    [Models.Internal.Media.MD5Key] = item.MD5,
                    [Models.Internal.Media.SHA1Key] = item.SHA1,
                    [Models.Internal.Media.SHA256Key] = item.SHA256,
                    [Models.Internal.Media.SpamSumKey] = item.SpamSum,
                },
                "rom" => new Models.Internal.Rom
                {
                    [Models.Internal.Rom.NameKey] = item.RomName,
                    [Models.Internal.Rom.SizeKey] = item.Size,
                    [Models.Internal.Rom.CRCKey] = item.CRC,
                    [Models.Internal.Rom.MD5Key] = item.MD5,
                    [Models.Internal.Rom.SHA1Key] = item.SHA1,
                    [Models.Internal.Rom.SHA256Key] = item.SHA256,
                    [Models.Internal.Rom.SHA384Key] = item.SHA384,
                    [Models.Internal.Rom.SHA512Key] = item.SHA512,
                    [Models.Internal.Rom.SpamSumKey] = item.SpamSum,
                    [Models.Internal.Rom.StatusKey] = item.Status,
                },
                _ => null,
            };
        }

        #endregion

        #region Deserialize

        /// <summary>
        /// Convert from <cref="Models.Internal.Machine"/> to an array of <cref="Models.SeparatedValue.Row"/>
        /// </summary>
        public static Models.SeparatedValue.Row[] ConvertMachineToSeparatedValue(Models.Internal.Machine item)
        {
            var rowItems = new List<Models.SeparatedValue.Row>();

            if (item.ContainsKey(Models.Internal.Machine.DiskKey) && item[Models.Internal.Machine.DiskKey] is Models.Internal.Disk[] disks)
            {
                foreach (var disk in disks)
                {
                    var rowItem = ConvertToSeparatedValue(disk);
                    rowItem.GameName = item.ReadString(Models.Internal.Machine.NameKey);
                    rowItem.Description = item.ReadString(Models.Internal.Machine.DescriptionKey);
                    rowItems.Add(rowItem);
                }
            }

            if (item.ContainsKey(Models.Internal.Machine.MediaKey) && item[Models.Internal.Machine.MediaKey] is Models.Internal.Media[] media)
            {
                foreach (var medium in media)
                {
                    var rowItem = ConvertToSeparatedValue(medium);
                    rowItem.GameName = item.ReadString(Models.Internal.Machine.NameKey);
                    rowItem.Description = item.ReadString(Models.Internal.Machine.DescriptionKey);
                    rowItems.Add(rowItem);
                }
            }

            if (item.ContainsKey(Models.Internal.Machine.RomKey) && item[Models.Internal.Machine.RomKey] is Models.Internal.Rom[] roms)
            {
                foreach (var rom in roms)
                {
                    var rowItem = ConvertToSeparatedValue(rom);
                    rowItem.GameName = item.ReadString(Models.Internal.Machine.NameKey);
                    rowItem.Description = item.ReadString(Models.Internal.Machine.DescriptionKey);
                    rowItems.Add(rowItem);
                }
            }

            return rowItems.ToArray();
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Disk"/> to <cref="Models.SeparatedValue.Row"/>
        /// </summary>
        public static Models.SeparatedValue.Row ConvertToSeparatedValue(Models.Internal.Disk item)
        {
            var row = new Models.SeparatedValue.Row
            {
                Type = "disk",
                DiskName = item.ReadString(Models.Internal.Disk.NameKey),
                MD5 = item.ReadString(Models.Internal.Disk.MD5Key),
                SHA1 = item.ReadString(Models.Internal.Disk.SHA1Key),
                Status = item.ReadString(Models.Internal.Disk.StatusKey),
            };
            return row;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Media"/> to <cref="Models.SeparatedValue.Row"/>
        /// </summary>
        public static Models.SeparatedValue.Row ConvertToSeparatedValue(Models.Internal.Media item)
        {
            var row = new Models.SeparatedValue.Row
            {
                Type = "media",
                DiskName = item.ReadString(Models.Internal.Media.NameKey),
                MD5 = item.ReadString(Models.Internal.Media.MD5Key),
                SHA1 = item.ReadString(Models.Internal.Media.SHA1Key),
                SHA256 = item.ReadString(Models.Internal.Media.SHA256Key),
                SpamSum = item.ReadString(Models.Internal.Media.SpamSumKey),
            };
            return row;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.SeparatedValue.Row"/>
        /// </summary>
        public static Models.SeparatedValue.Row ConvertToSeparatedValue(Models.Internal.Rom item)
        {
            var row = new Models.SeparatedValue.Row
            {
                Type = "rom",
                RomName = item.ReadString(Models.Internal.Rom.NameKey),
                Size = item.ReadString(Models.Internal.Rom.SizeKey),
                CRC = item.ReadString(Models.Internal.Rom.CRCKey),
                MD5 = item.ReadString(Models.Internal.Rom.MD5Key),
                SHA1 = item.ReadString(Models.Internal.Rom.SHA1Key),
                SHA256 = item.ReadString(Models.Internal.Rom.SHA256Key),
                SHA384 = item.ReadString(Models.Internal.Rom.SHA384Key),
                SHA512 = item.ReadString(Models.Internal.Rom.SHA512Key),
                SpamSum = item.ReadString(Models.Internal.Rom.SpamSumKey),
                Status = item.ReadString(Models.Internal.Rom.StatusKey),
            };
            return row;
        }

        #endregion
    }
}