using System.Collections.Generic;
using System.Linq;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for DosCenter models to internal structure
    /// </summary>
    public partial class Internal
    {
        #region Serialize

        /// <summary>
        /// Convert from <cref="Models.DosCenter.DosCenter"/> to <cref="Models.Internal.Header"/>
        /// </summary>
        public static Models.Internal.Header ConvertHeaderFromDosCenter(Models.DosCenter.DosCenter item)
        {
            var header = new Models.Internal.Header
            {
                [Models.Internal.Header.NameKey] = item.Name,
                [Models.Internal.Header.DescriptionKey] = item.Description,
                [Models.Internal.Header.VersionKey] = item.Version,
                [Models.Internal.Header.DateKey] = item.Date,
                [Models.Internal.Header.AuthorKey] = item.Author,
                [Models.Internal.Header.HomepageKey] = item.Homepage,
                [Models.Internal.Header.CommentKey] = item.Comment,
            };
            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.DosCenter.Game"/> to <cref="Models.Internal.Machine"/>
        /// </summary>
        public static Models.Internal.Machine ConvertMachineFromDosCenter(Models.DosCenter.Game item)
        {
            var machine = new Models.Internal.Machine
            {
                [Models.Internal.Machine.NameKey] = item.Name,
            };

            if (item.File != null && item.File.Any())
            {
                var roms = new List<Models.Internal.Rom>();
                foreach (var file in item.File)
                {
                    roms.Add(ConvertFromDosCenter(file));
                }
                machine[Models.Internal.Machine.RomKey] = roms.ToArray();
            }

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.DosCenter.File"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Rom ConvertFromDosCenter(Models.DosCenter.File item)
        {
            var rom = new Models.Internal.Rom
            {
                [Models.Internal.Rom.NameKey] = item.Name,
                [Models.Internal.Rom.SizeKey] = item.Size,
                [Models.Internal.Rom.CRCKey] = item.CRC,
                [Models.Internal.Rom.DateKey] = item.Date,
            };
            return rom;
        }

        #endregion

        #region Deserialize

        /// <summary>
        /// Convert from <cref="Models.Internal.Header"/> to <cref="Models.DosCenter.DosCenter"/>
        /// </summary>
        public static Models.DosCenter.DosCenter? ConvertHeaderToDosCenter(Models.Internal.Header? item)
        {
            if (item == null)
                return null;

            var dosCenter = new Models.DosCenter.DosCenter
            {
                Name = item.ReadString(Models.Internal.Header.NameKey),
                Description = item.ReadString(Models.Internal.Header.DescriptionKey),
                Version = item.ReadString(Models.Internal.Header.VersionKey),
                Date = item.ReadString(Models.Internal.Header.DateKey),
                Author = item.ReadString(Models.Internal.Header.AuthorKey),
                Homepage = item.ReadString(Models.Internal.Header.HomepageKey),
                Comment = item.ReadString(Models.Internal.Header.CommentKey),
            };
            return dosCenter;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Machine"/> to <cref="Models.DosCenter.Game"/>
        /// </summary>
        public static Models.DosCenter.Game? ConvertMachineToDosCenter(Models.Internal.Machine? item)
        {
            if (item == null)
                return null;

            var game = new Models.DosCenter.Game
            {
                Name = item.ReadString(Models.Internal.Machine.NameKey),
            };

            var roms = item.Read<Models.Internal.Rom[]>(Models.Internal.Machine.RomKey);
            game.File = roms?.Select(ConvertToDosCenter)?.ToArray();

            return game;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.DosCenter.File"/>
        /// </summary>
        private static Models.DosCenter.File? ConvertToDosCenter(Models.Internal.Rom? item)
        {
            if (item == null)
                return null;

            var file = new Models.DosCenter.File
            {
                Name = item.ReadString(Models.Internal.Rom.NameKey),
                Size = item.ReadString(Models.Internal.Rom.SizeKey),
                CRC = item.ReadString(Models.Internal.Rom.CRCKey),
                Date = item.ReadString(Models.Internal.Rom.DateKey),
            };
            return file;
        }

        #endregion
    }
}