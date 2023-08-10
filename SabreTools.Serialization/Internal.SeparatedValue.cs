using System.Collections.Generic;
using System.Linq;
using SabreTools.Models.Internal;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for SeparatedValue models to internal structure
    /// </summary>
    public partial class Internal
    {
        #region Serialize

        /// <summary>
        /// Convert from <cref="Models.SeparatedValue.MetadataFile"/> to <cref="MetadataFile"/>
        /// </summary>
        public static MetadataFile ConvertToInternalModel(Models.SeparatedValue.MetadataFile item)
        {
            var metadataFile = new MetadataFile
            {
                [MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item),
            };

            if (item?.Row != null && item.Row.Any())
                metadataFile[MetadataFile.MachineKey] = item.Row.Select(ConvertMachineToInternalModel).ToArray();

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.SeparatedValue.MetadataFile"/> to <cref="Header"/>
        /// </summary>
        private static Header ConvertHeaderToInternalModel(Models.SeparatedValue.MetadataFile item)
        {
            var header = new Header
            {
                [Header.HeaderKey] = item.Header,
            };

            if (item.Row != null && item.Row.Any())
            {
                var first = item.Row[0];
                //header[Header.FileNameKey] = first.FileName; // Not possible to map
                header[Header.NameKey] = first.FileName;
                header[Header.DescriptionKey] = first.Description;
            }

            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.SeparatedValue.Row"/> to <cref="Machine"/>
        /// </summary>
        private static Machine ConvertMachineToInternalModel(Models.SeparatedValue.Row item)
        {
            var machine = new Machine
            {
                [Machine.NameKey] = item.GameName,
                [Machine.DescriptionKey] = item.GameDescription,
            };

            var datItem = ConvertToInternalModel(item);
            switch (datItem)
            {
                case Disk disk:
                    machine[Machine.DiskKey] = new Disk[] { disk };
                    break;

                case Media media:
                    machine[Machine.MediaKey] = new Media[] { media };
                    break;

                case Rom rom:
                    machine[Machine.RomKey] = new Rom[] { rom };
                    break;
            }

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.SeparatedValue.Row"/> to <cref="DatItem"/>
        /// </summary>
        private static DatItem? ConvertToInternalModel(Models.SeparatedValue.Row item)
        {
            return item.Type switch
            {
                "disk" => new Disk
                {
                    [Disk.NameKey] = item.DiskName,
                    [Disk.MD5Key] = item.MD5,
                    [Disk.SHA1Key] = item.SHA1,
                    [Disk.StatusKey] = item.Status,
                },
                "media" => new Media
                {
                    [Media.NameKey] = item.DiskName,
                    [Media.MD5Key] = item.MD5,
                    [Media.SHA1Key] = item.SHA1,
                    [Media.SHA256Key] = item.SHA256,
                    [Media.SpamSumKey] = item.SpamSum,
                },
                "rom" => new Rom
                {
                    [Rom.NameKey] = item.RomName,
                    [Rom.SizeKey] = item.Size,
                    [Rom.CRCKey] = item.CRC,
                    [Rom.MD5Key] = item.MD5,
                    [Rom.SHA1Key] = item.SHA1,
                    [Rom.SHA256Key] = item.SHA256,
                    [Rom.SHA384Key] = item.SHA384,
                    [Rom.SHA512Key] = item.SHA512,
                    [Rom.SpamSumKey] = item.SpamSum,
                    [Rom.StatusKey] = item.Status,
                },
                _ => null,
            };
        }

        #endregion

        #region Deserialize

        /// <summary>
        /// Convert from <cref="Header"/> to <cref="Models.SeparatedValue.MetadataFile"/>
        /// </summary>
        public static Models.SeparatedValue.MetadataFile? ConvertHeaderToSeparatedValue(Header? item)
        {
            if (item == null)
                return null;

            var metadataFile = new Models.SeparatedValue.MetadataFile
            {
                Header = item.ReadStringArray(Header.HeaderKey),
            };
            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Machine"/> to an array of <cref="Models.SeparatedValue.Row"/>
        /// </summary>
        public static Models.SeparatedValue.Row[]? ConvertMachineToSeparatedValue(Machine? item)
        {
            if (item == null)
                return null;

            var rowItems = new List<Models.SeparatedValue.Row>();

            var disks = item.Read<Disk[]>(Machine.DiskKey);
            if (disks != null)
                rowItems.AddRange(disks.Select(disk => ConvertToSeparatedValue(disk, item)));

            var media = item.Read<Media[]>(Machine.MediaKey);
            if (media != null)
                rowItems.AddRange(media.Select(medium => ConvertToSeparatedValue(medium, item)));

            var roms = item.Read<Rom[]>(Machine.RomKey);
            if (roms != null)
                rowItems.AddRange(roms.Select(rom => ConvertToSeparatedValue(rom, item)));

            return rowItems.ToArray();
        }

        /// <summary>
        /// Convert from <cref="Disk"/> to <cref="Models.SeparatedValue.Row"/>
        /// </summary>
        private static Models.SeparatedValue.Row? ConvertToSeparatedValue(Disk? item, Machine? parent)
        {
            if (item == null)
                return null;

            var row = new Models.SeparatedValue.Row
            {
                GameName = parent?.ReadString(Machine.NameKey),
                Description = parent?.ReadString(Machine.DescriptionKey),
                Type = "disk",
                DiskName = item.ReadString(Disk.NameKey),
                MD5 = item.ReadString(Disk.MD5Key),
                SHA1 = item.ReadString(Disk.SHA1Key),
                Status = item.ReadString(Disk.StatusKey),
            };
            return row;
        }

        /// <summary>
        /// Convert from <cref="Media"/> to <cref="Models.SeparatedValue.Row"/>
        /// </summary>
        private static Models.SeparatedValue.Row? ConvertToSeparatedValue(Media? item, Machine? parent)
        {
            if (item == null)
                return null;

            var row = new Models.SeparatedValue.Row
            {
                GameName = parent?.ReadString(Machine.NameKey),
                Description = parent?.ReadString(Machine.DescriptionKey),
                Type = "media",
                DiskName = item.ReadString(Media.NameKey),
                MD5 = item.ReadString(Media.MD5Key),
                SHA1 = item.ReadString(Media.SHA1Key),
                SHA256 = item.ReadString(Media.SHA256Key),
                SpamSum = item.ReadString(Media.SpamSumKey),
            };
            return row;
        }

        /// <summary>
        /// Convert from <cref="Rom"/> to <cref="Models.SeparatedValue.Row"/>
        /// </summary>
        private static Models.SeparatedValue.Row? ConvertToSeparatedValue(Rom? item, Machine? parent)
        {
            if (item == null)
                return null;

            var row = new Models.SeparatedValue.Row
            {
                GameName = parent?.ReadString(Machine.NameKey),
                Description = parent?.ReadString(Machine.DescriptionKey),
                Type = "rom",
                RomName = item.ReadString(Rom.NameKey),
                Size = item.ReadString(Rom.SizeKey),
                CRC = item.ReadString(Rom.CRCKey),
                MD5 = item.ReadString(Rom.MD5Key),
                SHA1 = item.ReadString(Rom.SHA1Key),
                SHA256 = item.ReadString(Rom.SHA256Key),
                SHA384 = item.ReadString(Rom.SHA384Key),
                SHA512 = item.ReadString(Rom.SHA512Key),
                SpamSum = item.ReadString(Rom.SpamSumKey),
                Status = item.ReadString(Rom.StatusKey),
            };
            return row;
        }

        #endregion
    }
}