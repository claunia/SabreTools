using System.Linq;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for RomCenter models to internal structure
    /// </summary>
    public partial class Internal
    {
        #region Serialize

        /// <summary>
        /// Convert from <cref="Models.RomCenter.MetadataFile"/> to <cref="Models.Internal.Header"/>
        /// </summary>
        public static Models.Internal.Header ConvertHeaderFromRomCenter(Models.RomCenter.MetadataFile item)
        {
            var header = new Models.Internal.Header();

            if (item.Credits != null)
            {
                header[Models.Internal.Header.AuthorKey] = item.Credits.Author;
                header[Models.Internal.Header.VersionKey] = item.Credits.Version;
                header[Models.Internal.Header.EmailKey] = item.Credits.Email;
                header[Models.Internal.Header.HomepageKey] = item.Credits.Homepage;
                header[Models.Internal.Header.UrlKey] = item.Credits.Url;
                header[Models.Internal.Header.DateKey] = item.Credits.Date;
                header[Models.Internal.Header.CommentKey] = item.Credits.Comment;
            }

            if (item.Dat != null)
            {
                header[Models.Internal.Header.DatVersionKey] = item.Dat.Version;
                header[Models.Internal.Header.PluginKey] = item.Dat.Plugin;

                if (item.Dat.Split == "yes" || item.Dat.Split == "1")
                    header[Models.Internal.Header.ForceMergingKey] = "split";
                else if (item.Dat.Merge == "yes" || item.Dat.Merge == "1")
                    header[Models.Internal.Header.ForceMergingKey] = "merge";
            }

            if (item.Emulator != null)
            {
                header[Models.Internal.Header.RefNameKey] = item.Emulator.RefName;
                header[Models.Internal.Header.EmulatorVersionKey] = item.Emulator.Version;
            }

            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.RomCenter.Game"/> to <cref="Models.Internal.Machine"/>
        /// </summary>
        public static Models.Internal.Machine ConvertMachineFromRomCenter(Models.RomCenter.Rom item)
        {
            var machine = new Models.Internal.Machine
            {
                [Models.Internal.Machine.RomOfKey] = item.ParentName,
                //[Models.Internal.Machine.ParentDescriptionKey] = item.ParentDescription, // This is unmappable
                [Models.Internal.Machine.NameKey] = item.GameName,
                [Models.Internal.Machine.DescriptionKey] = item.GameDescription,
                [Models.Internal.Machine.RomKey] = new Models.Internal.Rom[] { ConvertFromRomCenter(item) },
            };

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.RomCenter.Rom"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Rom ConvertFromRomCenter(Models.RomCenter.Rom item)
        {
            var rom = new Models.Internal.Rom
            {
                [Models.Internal.Rom.NameKey] = item.RomName,
                [Models.Internal.Rom.CRCKey] = item.RomCRC,
                [Models.Internal.Rom.SizeKey] = item.RomSize,
                [Models.Internal.Rom.MergeKey] = item.MergeName,
            };
            return rom;
        }

        #endregion

        #region Deserialize

        /// <summary>
        /// Convert from <cref="Models.Internal.Machine"/> to an array of <cref="Models.RomCenter.Rom"/>
        /// </summary>
        public static Models.RomCenter.Rom?[]? ConvertMachineToRomCenter(Models.Internal.Machine? item)
        {
            if (item == null)
                return null;

            var roms = item.Read<Models.Internal.Rom[]>(Models.Internal.Machine.RomKey);
            return roms?.Select(rom => ConvertToRomCenter(rom, item))?.ToArray();
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.RomCenter.Rom"/>
        /// </summary>
        private static Models.RomCenter.Rom? ConvertToRomCenter(Models.Internal.Rom? item, Models.Internal.Machine? parent)
        {
            if (item == null)
                return null;

            var row = new Models.RomCenter.Rom
            {
                RomName = item.ReadString(Models.Internal.Rom.NameKey),
                RomCRC = item.ReadString(Models.Internal.Rom.CRCKey),
                RomSize = item.ReadString(Models.Internal.Rom.SizeKey),
                MergeName = item.ReadString(Models.Internal.Rom.MergeKey),

                ParentName = parent?.ReadString(Models.Internal.Machine.RomOfKey),
                //ParentDescription = parent?.ReadString(Models.Internal.Machine.ParentDescriptionKey), // This is unmappable
                GameName = parent?.ReadString(Models.Internal.Machine.NameKey),
                GameDescription = parent?.ReadString(Models.Internal.Machine.DescriptionKey),
            };
            return row;
        }

        #endregion
    }
}