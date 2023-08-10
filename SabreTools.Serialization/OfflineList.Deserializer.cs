using System.Linq;
using SabreTools.Models.OfflineList;

namespace SabreTools.Serialization
{
    /// <summary>
    /// XML deserializer for OfflineList metadata files
    /// </summary>
    public partial class OfflineList : XmlSerializer<Dat>
    {
        // TODO: Add deserialization of entire Dat
        #region Internal

        /// <summary>
        /// Convert from <cref="Models.Internal.Header"/> to <cref="Models.OfflineList.Dat"/>
        /// </summary>
        public static Dat? ConvertHeaderFromInternalModel(Models.Internal.Header? item)
        {
            if (item == null)
                return null;

            var dat = new Dat
            {
                NoNamespaceSchemaLocation = item.ReadString(Models.Internal.Header.SchemaLocationKey),
            };

            if (item.ContainsKey(Models.Internal.Header.NameKey)
                || item.ContainsKey(Models.Internal.Header.ImFolderKey)
                || item.ContainsKey(Models.Internal.Header.DatVersionKey)
                || item.ContainsKey(Models.Internal.Header.SystemKey)
                || item.ContainsKey(Models.Internal.Header.ScreenshotsWidthKey)
                || item.ContainsKey(Models.Internal.Header.ScreenshotsHeightKey)
                || item.ContainsKey(Models.Internal.Header.InfosKey)
                || item.ContainsKey(Models.Internal.Header.CanOpenKey)
                || item.ContainsKey(Models.Internal.Header.NewDatKey)
                || item.ContainsKey(Models.Internal.Header.SearchKey)
                || item.ContainsKey(Models.Internal.Header.RomTitleKey))
            {
                dat.Configuration = new Configuration
                {
                    DatName = item.ReadString(Models.Internal.Header.NameKey),
                    ImFolder = item.ReadString(Models.Internal.Header.ImFolderKey),
                    DatVersion = item.ReadString(Models.Internal.Header.DatVersionKey),
                    System = item.ReadString(Models.Internal.Header.SystemKey),
                    ScreenshotsWidth = item.ReadString(Models.Internal.Header.ScreenshotsWidthKey),
                    ScreenshotsHeight = item.ReadString(Models.Internal.Header.ScreenshotsHeightKey),
                    Infos = item.Read<Infos>(Models.Internal.Header.InfosKey),
                    CanOpen = item.Read<CanOpen>(Models.Internal.Header.CanOpenKey),
                    NewDat = item.Read<NewDat>(Models.Internal.Header.NewDatKey),
                    Search = item.Read<Search>(Models.Internal.Header.SearchKey),
                    RomTitle = item.ReadString(Models.Internal.Header.RomTitleKey),
                };
            }

            if (item.ContainsKey(Models.Internal.Header.ImagesKey))
            {
                dat.GUI = new GUI
                {
                    Images = item.Read<Images>(Models.Internal.Header.ImagesKey),
                };
            }

            return dat;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Machine"/> to <cref="Models.OfflineList.Game"/>
        /// </summary>
        public static Game? ConvertMachineFromInternalModel(Models.Internal.Machine? item)
        {
            if (item == null)
                return null;

            var game = new Game
            {
                ImageNumber = item.ReadString(Models.Internal.Machine.ImageNumberKey),
                ReleaseNumber = item.ReadString(Models.Internal.Machine.ReleaseNumberKey),
                Title = item.ReadString(Models.Internal.Machine.NameKey),
                SaveType = item.ReadString(Models.Internal.Machine.SaveTypeKey),
                Publisher = item.ReadString(Models.Internal.Machine.PublisherKey),
                Location = item.ReadString(Models.Internal.Machine.LocationKey),
                SourceRom = item.ReadString(Models.Internal.Machine.SourceRomKey),
                Language = item.ReadString(Models.Internal.Machine.LanguageKey),
                Im1CRC = item.ReadString(Models.Internal.Machine.Im1CRCKey),
                Im2CRC = item.ReadString(Models.Internal.Machine.Im2CRCKey),
                Comment = item.ReadString(Models.Internal.Machine.CommentKey),
                DuplicateID = item.ReadString(Models.Internal.Machine.DuplicateIDKey),
            };

            var roms = item.Read<Models.Internal.Rom[]>(Models.Internal.Machine.RomKey);
            game.RomSize = roms?
                .Select(rom => rom.ReadString(Models.Internal.Rom.SizeKey))?
                .FirstOrDefault(s => s != null);
            var romCRCs = roms?.Select(ConvertFromInternalModel).ToArray();
            game.Files = new Files { RomCRC = romCRCs };

            return game;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.OfflineList.FileRomCRC"/>
        /// </summary>
        private static FileRomCRC? ConvertFromInternalModel(Models.Internal.Rom? item)
        {
            if (item == null)
                return null;

            var fileRomCRC = new FileRomCRC
            {
                Extension = item.ReadString(Models.Internal.Rom.ExtensionKey),
                Content = item.ReadString(Models.Internal.Rom.CRCKey),
            };
            return fileRomCRC;
        }

        #endregion
    }
}