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
        /// Convert from <cref="Models.SeparatedValue.MetadataFile"/> to <cref="Models.Internal.Header"/>
        /// </summary>
        public static Models.Internal.Header ConvertHeaderFromArchiveDotOrg(Models.SeparatedValue.MetadataFile item)
        {
            var header = new Models.Internal.Header
            {
                [Models.Internal.Header.HeaderKey] = item.Header,
            };

            if (item.Row != null && item.Row.Any())
            {
                var first = item.Row[0];
                //header[Models.Internal.Header.FileNameKey] = first.FileName; // Not possible to map
                header[Models.Internal.Header.NameKey] = first.FileName;
                header[Models.Internal.Header.DescriptionKey] = first.Description;
            }

            return header;
        }

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
        public static Models.SeparatedValue.Row[]? ConvertMachineToSeparatedValue(Models.Internal.Machine? item)
        {
            if (item == null)
                return null;

            var rowItems = new List<Models.SeparatedValue.Row>();

            var disks = item.Read<Models.Internal.Disk[]>(Models.Internal.Machine.DiskKey);
            if (disks != null)
                rowItems.AddRange(disks.Select(disk => ConvertToSeparatedValue(disk, item)));

            var media = item.Read<Models.Internal.Media[]>(Models.Internal.Machine.MediaKey);
            if (media != null)
                rowItems.AddRange(media.Select(medium => ConvertToSeparatedValue(medium, item)));

            var roms = item.Read<Models.Internal.Rom[]>(Models.Internal.Machine.RomKey);
            if (roms != null)
                rowItems.AddRange(roms.Select(rom => ConvertToSeparatedValue(rom, item)));

            return rowItems.ToArray();
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Disk"/> to <cref="Models.SeparatedValue.Row"/>
        /// </summary>
        private static Models.SeparatedValue.Row? ConvertToSeparatedValue(Models.Internal.Disk? item, Models.Internal.Machine? parent)
        {
            if (item == null)
                return null;

            var row = new Models.SeparatedValue.Row
            {
                GameName = parent?.ReadString(Models.Internal.Machine.NameKey),
                Description = parent?.ReadString(Models.Internal.Machine.DescriptionKey),
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
        private static Models.SeparatedValue.Row? ConvertToSeparatedValue(Models.Internal.Media? item, Models.Internal.Machine? parent)
        {
            if (item == null)
                return null;

            var row = new Models.SeparatedValue.Row
            {
                GameName = parent?.ReadString(Models.Internal.Machine.NameKey),
                Description = parent?.ReadString(Models.Internal.Machine.DescriptionKey),
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
        private static Models.SeparatedValue.Row? ConvertToSeparatedValue(Models.Internal.Rom? item, Models.Internal.Machine? parent)
        {
            if (item == null)
                return null;

            var row = new Models.SeparatedValue.Row
            {
                GameName = parent?.ReadString(Models.Internal.Machine.NameKey),
                Description = parent?.ReadString(Models.Internal.Machine.DescriptionKey),
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