using System.Collections.Generic;
using System.Linq;
using SabreTools.Models.Internal;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for DosCenter models to internal structure
    /// </summary>
    public partial class Internal
    {
        #region Serialize

        /// <summary>
        /// Convert from <cref="Models.DosCenter.MetadataFile"/> to <cref="MetadataFile"/>
        /// </summary>
        public static MetadataFile ConvertFromDosCenter(Models.DosCenter.MetadataFile item)
        {
            var metadataFile = new MetadataFile();

            if (item?.DosCenter != null)
                metadataFile[MetadataFile.HeaderKey] = ConvertHeaderFromDosCenter(item.DosCenter);

            if (item?.Game != null && item.Game.Any())
                metadataFile[MetadataFile.MachineKey] = item.Game.Select(ConvertMachineFromDosCenter).ToArray();

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.DosCenter.DosCenter"/> to <cref="Header"/>
        /// </summary>
        private static Header ConvertHeaderFromDosCenter(Models.DosCenter.DosCenter item)
        {
            var header = new Header
            {
                [Header.NameKey] = item.Name,
                [Header.DescriptionKey] = item.Description,
                [Header.VersionKey] = item.Version,
                [Header.DateKey] = item.Date,
                [Header.AuthorKey] = item.Author,
                [Header.HomepageKey] = item.Homepage,
                [Header.CommentKey] = item.Comment,
            };
            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.DosCenter.Game"/> to <cref="Machine"/>
        /// </summary>
        private static Machine ConvertMachineFromDosCenter(Models.DosCenter.Game item)
        {
            var machine = new Machine
            {
                [Machine.NameKey] = item.Name,
            };

            if (item.File != null && item.File.Any())
            {
                var roms = new List<Rom>();
                foreach (var file in item.File)
                {
                    roms.Add(ConvertFromDosCenter(file));
                }
                machine[Machine.RomKey] = roms.ToArray();
            }

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.DosCenter.File"/> to <cref="Rom"/>
        /// </summary>
        private static Rom ConvertFromDosCenter(Models.DosCenter.File item)
        {
            var rom = new Rom
            {
                [Rom.NameKey] = item.Name,
                [Rom.SizeKey] = item.Size,
                [Rom.CRCKey] = item.CRC,
                [Rom.DateKey] = item.Date,
            };
            return rom;
        }

        #endregion

        #region Deserialize

        /// <summary>
        /// Convert from <cref="Header"/> to <cref="Models.DosCenter.DosCenter"/>
        /// </summary>
        public static Models.DosCenter.DosCenter? ConvertHeaderToDosCenter(Header? item)
        {
            if (item == null)
                return null;

            var dosCenter = new Models.DosCenter.DosCenter
            {
                Name = item.ReadString(Header.NameKey),
                Description = item.ReadString(Header.DescriptionKey),
                Version = item.ReadString(Header.VersionKey),
                Date = item.ReadString(Header.DateKey),
                Author = item.ReadString(Header.AuthorKey),
                Homepage = item.ReadString(Header.HomepageKey),
                Comment = item.ReadString(Header.CommentKey),
            };
            return dosCenter;
        }

        /// <summary>
        /// Convert from <cref="Machine"/> to <cref="Models.DosCenter.Game"/>
        /// </summary>
        public static Models.DosCenter.Game? ConvertMachineToDosCenter(Machine? item)
        {
            if (item == null)
                return null;

            var game = new Models.DosCenter.Game
            {
                Name = item.ReadString(Machine.NameKey),
            };

            var roms = item.Read<Rom[]>(Machine.RomKey);
            game.File = roms?.Select(ConvertToDosCenter)?.ToArray();

            return game;
        }

        /// <summary>
        /// Convert from <cref="Rom"/> to <cref="Models.DosCenter.File"/>
        /// </summary>
        private static Models.DosCenter.File? ConvertToDosCenter(Rom? item)
        {
            if (item == null)
                return null;

            var file = new Models.DosCenter.File
            {
                Name = item.ReadString(Rom.NameKey),
                Size = item.ReadString(Rom.SizeKey),
                CRC = item.ReadString(Rom.CRCKey),
                Date = item.ReadString(Rom.DateKey),
            };
            return file;
        }

        #endregion
    }
}