using System.Linq;
using SabreTools.Models.OfflineList;

namespace SabreTools.Serialization
{
    /// <summary>
    /// XML serializer for OfflineList metadata files
    /// </summary>
    public partial class OfflineList : XmlSerializer<Dat>
    {
        #region Internal

        /// <summary>
        /// Convert from <cref="Models.OfflineList.Dat"/> to <cref="Models.Internal.MetadataFile"/>
        /// </summary>
        public static Models.Internal.MetadataFile ConvertToInternalModel(Dat item)
        {
            var metadataFile = new Models.Internal.MetadataFile
            {
                [Models.Internal.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item),
            };

            if (item?.Games?.Game != null && item.Games.Game.Any())
                metadataFile[Models.Internal.MetadataFile.MachineKey] = item.Games.Game.Select(ConvertMachineToInternalModel).ToArray();

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.OfflineList.Dat"/> to <cref="Models.Internal.Header"/>
        /// </summary>
        private static Models.Internal.Header ConvertHeaderToInternalModel(Dat item)
        {
            var header = new Models.Internal.Header
            {
                [Models.Internal.Header.SchemaLocationKey] = item.NoNamespaceSchemaLocation,
            };

            if (item.Configuration != null)
            {
                header[Models.Internal.Header.NameKey] = item.Configuration.DatName;
                header[Models.Internal.Header.ImFolderKey] = item.Configuration.ImFolder;
                header[Models.Internal.Header.DatVersionKey] = item.Configuration.DatVersion;
                header[Models.Internal.Header.SystemKey] = item.Configuration.System;
                header[Models.Internal.Header.ScreenshotsWidthKey] = item.Configuration.ScreenshotsWidth;
                header[Models.Internal.Header.ScreenshotsHeightKey] = item.Configuration.ScreenshotsHeight;
                header[Models.Internal.Header.InfosKey] = item.Configuration.Infos;
                header[Models.Internal.Header.CanOpenKey] = item.Configuration.CanOpen;
                header[Models.Internal.Header.NewDatKey] = item.Configuration.NewDat;
                header[Models.Internal.Header.SearchKey] = item.Configuration.Search;
                header[Models.Internal.Header.RomTitleKey] = item.Configuration.RomTitle;
            }

            if (item.GUI != null)
            {
                header[Models.Internal.Header.ImagesKey] = item.GUI.Images;
            }

            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.OfflineList.Game"/> to <cref="Models.Internal.Machine"/>
        /// </summary>
        private static Models.Internal.Machine ConvertMachineToInternalModel(Game item)
        {
            var machine = new Models.Internal.Machine
            {
                [Models.Internal.Machine.ImageNumberKey] = item.ImageNumber,
                [Models.Internal.Machine.ReleaseNumberKey] = item.ReleaseNumber,
                [Models.Internal.Machine.NameKey] = item.Title,
                [Models.Internal.Machine.SaveTypeKey] = item.SaveType,
                [Models.Internal.Machine.PublisherKey] = item.Publisher,
                [Models.Internal.Machine.LocationKey] = item.Location,
                [Models.Internal.Machine.SourceRomKey] = item.SourceRom,
                [Models.Internal.Machine.LanguageKey] = item.Language,
                [Models.Internal.Machine.Im1CRCKey] = item.Im1CRC,
                [Models.Internal.Machine.Im2CRCKey] = item.Im2CRC,
                [Models.Internal.Machine.CommentKey] = item.Comment,
                [Models.Internal.Machine.DuplicateIDKey] = item.DuplicateID,
            };

            if (item.Files?.RomCRC != null && item.Files.RomCRC.Any())
            {
                machine[Models.Internal.Machine.RomKey] = item.Files.RomCRC.Select(romCRC =>
                {
                    var rom = ConvertToInternalModel(romCRC);
                    rom[Models.Internal.Rom.SizeKey] = item.RomSize;
                    return rom;
                }).ToArray();
            }

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.OfflineList.FileRomCRC"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        private static Models.Internal.Rom ConvertToInternalModel(FileRomCRC item)
        {
            var rom = new Models.Internal.Rom
            {
                [Models.Internal.Rom.ExtensionKey] = item.Extension,
                [Models.Internal.Rom.CRCKey] = item.Content,
            };
            return rom;
        }

        #endregion
    }
}