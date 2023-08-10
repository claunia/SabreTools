using System.Linq;
using SabreTools.Models.Internal;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for RomCenter models to internal structure
    /// </summary>
    public partial class Internal
    {
        #region Serialize

        /// <summary>
        /// Convert from <cref="Models.RomCenter.MetadataFile"/> to <cref="MetadataFile"/>
        /// </summary>
        public static MetadataFile ConvertFromRomCenter(Models.RomCenter.MetadataFile item)
        {
            var metadataFile = new MetadataFile
            {
                [MetadataFile.HeaderKey] = ConvertHeaderFromRomCenter(item),
            };

            if (item?.Games?.Rom != null && item.Games.Rom.Any())
                metadataFile[MetadataFile.MachineKey] = item.Games.Rom.Select(ConvertMachineFromRomCenter).ToArray();

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.RomCenter.MetadataFile"/> to <cref="Header"/>
        /// </summary>
        private static Header ConvertHeaderFromRomCenter(Models.RomCenter.MetadataFile item)
        {
            var header = new Header();

            if (item.Credits != null)
            {
                header[Header.AuthorKey] = item.Credits.Author;
                header[Header.VersionKey] = item.Credits.Version;
                header[Header.EmailKey] = item.Credits.Email;
                header[Header.HomepageKey] = item.Credits.Homepage;
                header[Header.UrlKey] = item.Credits.Url;
                header[Header.DateKey] = item.Credits.Date;
                header[Header.CommentKey] = item.Credits.Comment;
            }

            if (item.Dat != null)
            {
                header[Header.DatVersionKey] = item.Dat.Version;
                header[Header.PluginKey] = item.Dat.Plugin;

                if (item.Dat.Split == "yes" || item.Dat.Split == "1")
                    header[Header.ForceMergingKey] = "split";
                else if (item.Dat.Merge == "yes" || item.Dat.Merge == "1")
                    header[Header.ForceMergingKey] = "merge";
            }

            if (item.Emulator != null)
            {
                header[Header.RefNameKey] = item.Emulator.RefName;
                header[Header.EmulatorVersionKey] = item.Emulator.Version;
            }

            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.RomCenter.Game"/> to <cref="Machine"/>
        /// </summary>
        private static Machine ConvertMachineFromRomCenter(Models.RomCenter.Rom item)
        {
            var machine = new Machine
            {
                [Machine.RomOfKey] = item.ParentName,
                //[Machine.ParentDescriptionKey] = item.ParentDescription, // This is unmappable
                [Machine.NameKey] = item.GameName,
                [Machine.DescriptionKey] = item.GameDescription,
                [Machine.RomKey] = new Rom[] { ConvertFromRomCenter(item) },
            };

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.RomCenter.Rom"/> to <cref="Rom"/>
        /// </summary>
        private static Rom ConvertFromRomCenter(Models.RomCenter.Rom item)
        {
            var rom = new Rom
            {
                [Rom.NameKey] = item.RomName,
                [Rom.CRCKey] = item.RomCRC,
                [Rom.SizeKey] = item.RomSize,
                [Rom.MergeKey] = item.MergeName,
            };
            return rom;
        }

        #endregion

        #region Deserialize

        /// <summary>
        /// Convert from <cref="Header"/> to <cref="Models.RomCenter.MetadataFile"/>
        /// </summary>
        public static Models.RomCenter.MetadataFile? ConvertHeaderToRomCenter(Header? item)
        {
            if (item == null)
                return null;

            var metadataFile = new Models.RomCenter.MetadataFile();

            if (item.ContainsKey(Header.AuthorKey)
                || item.ContainsKey(Header.VersionKey)
                || item.ContainsKey(Header.EmailKey)
                || item.ContainsKey(Header.HomepageKey)
                || item.ContainsKey(Header.UrlKey)
                || item.ContainsKey(Header.DateKey)
                || item.ContainsKey(Header.CommentKey))
            {
                metadataFile.Credits = new Models.RomCenter.Credits
                {
                    Author = item.ReadString(Header.AuthorKey),
                    Version = item.ReadString(Header.VersionKey),
                    Email = item.ReadString(Header.EmailKey),
                    Homepage = item.ReadString(Header.HomepageKey),
                    Url = item.ReadString(Header.UrlKey),
                    Date = item.ReadString(Header.DateKey),
                    Comment = item.ReadString(Header.CommentKey),
                };
            }

            if (item.ContainsKey(Header.DatVersionKey)
                || item.ContainsKey(Header.PluginKey)
                || item.ContainsKey(Header.ForceMergingKey))
            {
                metadataFile.Dat = new Models.RomCenter.Dat
                {
                    Version = item.ReadString(Header.DatVersionKey),
                    Plugin = item.ReadString(Header.PluginKey),
                    Split = item.ReadString(Header.ForceMergingKey) == "split" ? "yes" : "no",
                    Merge = item.ReadString(Header.ForceMergingKey) == "merge" ? "yes" : "no",
                };
            }

            if (item.ContainsKey(Header.RefNameKey)
                || item.ContainsKey(Header.EmulatorVersionKey))
            {
                metadataFile.Emulator = new Models.RomCenter.Emulator
                {
                    RefName = item.ReadString(Header.RefNameKey),
                    Version = item.ReadString(Header.EmulatorVersionKey),
                };
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Machine"/> to an array of <cref="Models.RomCenter.Rom"/>
        /// </summary>
        public static Models.RomCenter.Rom?[]? ConvertMachineToRomCenter(Machine? item)
        {
            if (item == null)
                return null;

            var roms = item.Read<Rom[]>(Machine.RomKey);
            return roms?.Select(rom => ConvertToRomCenter(rom, item))?.ToArray();
        }

        /// <summary>
        /// Convert from <cref="Rom"/> to <cref="Models.RomCenter.Rom"/>
        /// </summary>
        private static Models.RomCenter.Rom? ConvertToRomCenter(Rom? item, Machine? parent)
        {
            if (item == null)
                return null;

            var row = new Models.RomCenter.Rom
            {
                RomName = item.ReadString(Rom.NameKey),
                RomCRC = item.ReadString(Rom.CRCKey),
                RomSize = item.ReadString(Rom.SizeKey),
                MergeName = item.ReadString(Rom.MergeKey),

                ParentName = parent?.ReadString(Machine.RomOfKey),
                //ParentDescription = parent?.ReadString(Machine.ParentDescriptionKey), // This is unmappable
                GameName = parent?.ReadString(Machine.NameKey),
                GameDescription = parent?.ReadString(Machine.DescriptionKey),
            };
            return row;
        }

        #endregion
    }
}