using System.Linq;
using SabreTools.Models.Internal;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for ArchiveDotOrg models to internal structure
    /// </summary>
    public partial class Internal
    {
        #region Serialize

        /// <summary>
        /// Convert from <cref="Models.ArchiveDotOrg.Files"/> to <cref="MetadataFile"/>
        /// </summary>
        public static MetadataFile ConvertToInternalModel(Models.ArchiveDotOrg.Files item)
        {
            var metadataFile = new MetadataFile
            {
                [MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item),
            };

            if (item?.File != null && item.File.Any())
                metadataFile[MetadataFile.MachineKey] = item.File.Select(ConvertMachineToInternalModel).ToArray();

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.ArchiveDotOrg.Files"/> to <cref="Header"/>
        /// </summary>
        private static Header ConvertHeaderToInternalModel(Models.ArchiveDotOrg.Files item)
        {
            var header = new Header
            {
                [Header.NameKey] = "archive.org",
            };
            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.ArchiveDotOrg.File"/> to <cref="Machine"/>
        /// </summary>
        private static Machine ConvertMachineToInternalModel(Models.ArchiveDotOrg.File item)
        {
            var machine = new Machine
            {
                [Machine.RomKey] = ConvertToInternalModel(item),
            };
            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.ArchiveDotOrg.File"/> to <cref="Rom"/>
        /// </summary>
        private static Rom ConvertToInternalModel(Models.ArchiveDotOrg.File item)
        {
            var rom = new Rom
            {
                [Rom.NameKey] = item.Name,
                [Rom.SourceKey] = item.Source,
                [Rom.BitTorrentMagnetHashKey] = item.BitTorrentMagnetHash,
                [Rom.LastModifiedTimeKey] = item.LastModifiedTime,
                [Rom.SizeKey] = item.Size,
                [Rom.MD5Key] = item.MD5,
                [Rom.CRCKey] = item.CRC32,
                [Rom.SHA1Key] = item.SHA1,
                [Rom.FileCountKey] = item.FileCount,
                [Rom.FormatKey] = item.Format,
                [Rom.OriginalKey] = item.Original,
                [Rom.SummationKey] = item.Summation,
                [Rom.MatrixNumberKey] = item.MatrixNumber,
                [Rom.CollectionCatalogNumberKey] = item.CollectionCatalogNumber,
                [Rom.PublisherKey] = item.Publisher,
                [Rom.CommentKey] = item.Comment,

                [Rom.ASRDetectedLangKey] = item.ASRDetectedLang,
                [Rom.ASRDetectedLangConfKey] = item.ASRDetectedLangConf,
                [Rom.ASRTranscribedLangKey] = item.ASRTranscribedLang,
                [Rom.WhisperASRModuleVersionKey] = item.WhisperASRModuleVersion,
                [Rom.WhisperModelHashKey] = item.WhisperModelHash,
                [Rom.WhisperModelNameKey] = item.WhisperModelName,
                [Rom.WhisperVersionKey] = item.WhisperVersion,

                [Rom.ClothCoverDetectionModuleVersionKey] = item.ClothCoverDetectionModuleVersion,
                [Rom.hOCRCharToWordhOCRVersionKey] = item.hOCRCharToWordhOCRVersion,
                [Rom.hOCRCharToWordModuleVersionKey] = item.hOCRCharToWordModuleVersion,
                [Rom.hOCRFtsTexthOCRVersionKey] = item.hOCRFtsTexthOCRVersion,
                [Rom.hOCRFtsTextModuleVersionKey] = item.hOCRFtsTextModuleVersion,
                [Rom.hOCRPageIndexhOCRVersionKey] = item.hOCRPageIndexhOCRVersion,
                [Rom.hOCRPageIndexModuleVersionKey] = item.hOCRPageIndexModuleVersion,
                [Rom.TesseractOCRKey] = item.TesseractOCR,
                [Rom.TesseractOCRConvertedKey] = item.TesseractOCRConverted,
                [Rom.TesseractOCRDetectedLangKey] = item.TesseractOCRDetectedLang,
                [Rom.TesseractOCRDetectedLangConfKey] = item.TesseractOCRDetectedLangConf,
                [Rom.TesseractOCRDetectedScriptKey] = item.TesseractOCRDetectedScript,
                [Rom.TesseractOCRDetectedScriptConfKey] = item.TesseractOCRDetectedScriptConf,
                [Rom.TesseractOCRModuleVersionKey] = item.TesseractOCRModuleVersion,
                [Rom.TesseractOCRParametersKey] = item.TesseractOCRParameters,
                [Rom.PDFModuleVersionKey] = item.PDFModuleVersion,
                [Rom.WordConfidenceInterval0To10Key] = item.WordConfidenceInterval0To10,
                [Rom.WordConfidenceInterval11To20Key] = item.WordConfidenceInterval11To20,
                [Rom.WordConfidenceInterval21To30Key] = item.WordConfidenceInterval21To30,
                [Rom.WordConfidenceInterval31To40Key] = item.WordConfidenceInterval31To40,
                [Rom.WordConfidenceInterval41To50Key] = item.WordConfidenceInterval41To50,
                [Rom.WordConfidenceInterval51To60Key] = item.WordConfidenceInterval51To60,
                [Rom.WordConfidenceInterval61To70Key] = item.WordConfidenceInterval61To70,
                [Rom.WordConfidenceInterval71To80Key] = item.WordConfidenceInterval71To80,
                [Rom.WordConfidenceInterval81To90Key] = item.WordConfidenceInterval81To90,
                [Rom.WordConfidenceInterval91To100Key] = item.WordConfidenceInterval91To100,

                [Rom.AlbumKey] = item.Album,
                [Rom.ArtistKey] = item.Artist,
                [Rom.BitrateKey] = item.Bitrate,
                [Rom.CreatorKey] = item.Creator,
                [Rom.HeightKey] = item.Height,
                [Rom.LengthKey] = item.Length,
                [Rom.PreviewImageKey] = item.PreviewImage,
                [Rom.RotationKey] = item.Rotation,
                [Rom.TitleKey] = item.Title,
                [Rom.TrackKey] = item.Track,
                [Rom.WidthKey] = item.Width,
            };
            return rom;
        }

        #endregion

        #region Deserialize

        /// <summary>
        /// Convert from <cref="Machine"/> to an array of <cref="Models.ArchiveDotOrg.File"/>
        /// </summary>
        public static Models.ArchiveDotOrg.File?[]? ConvertMachineToArchiveDotOrg(Machine? item)
        {
            if (item == null)
                return null;

            var roms = item.Read<Rom[]>(Machine.RomKey);
            return roms?.Select(ConvertToArchiveDotOrg)?.ToArray();
        }

        /// <summary>
        /// Convert from <cref="Rom"/> to <cref="Models.ArchiveDotOrg.File"/>
        /// </summary>
        private static Models.ArchiveDotOrg.File? ConvertToArchiveDotOrg(Rom? item)
        {
            if (item == null)
                return null;

            var file = new Models.ArchiveDotOrg.File
            {
                Name = item.ReadString(Rom.NameKey),
                Source = item.ReadString(Rom.SourceKey),
                BitTorrentMagnetHash = item.ReadString(Rom.BitTorrentMagnetHashKey),
                LastModifiedTime = item.ReadString(Rom.LastModifiedTimeKey),
                Size = item.ReadString(Rom.SizeKey),
                MD5 = item.ReadString(Rom.MD5Key),
                CRC32 = item.ReadString(Rom.CRCKey),
                SHA1 = item.ReadString(Rom.SHA1Key),
                FileCount = item.ReadString(Rom.FileCountKey),
                Format = item.ReadString(Rom.FormatKey),
                Original = item.ReadString(Rom.OriginalKey),
                Summation = item.ReadString(Rom.SummationKey),
                MatrixNumber = item.ReadString(Rom.MatrixNumberKey),
                CollectionCatalogNumber = item.ReadString(Rom.CollectionCatalogNumberKey),
                Comment = item.ReadString(Rom.CommentKey),

                ASRDetectedLang = item.ReadString(Rom.ASRDetectedLangKey),
                ASRDetectedLangConf = item.ReadString(Rom.ASRDetectedLangConfKey),
                ASRTranscribedLang = item.ReadString(Rom.ASRTranscribedLangKey),
                WhisperASRModuleVersion = item.ReadString(Rom.WhisperASRModuleVersionKey),
                WhisperModelHash = item.ReadString(Rom.WhisperModelHashKey),
                WhisperModelName = item.ReadString(Rom.WhisperModelNameKey),
                WhisperVersion = item.ReadString(Rom.WhisperVersionKey),

                ClothCoverDetectionModuleVersion = item.ReadString(Rom.ClothCoverDetectionModuleVersionKey),
                hOCRCharToWordhOCRVersion = item.ReadString(Rom.hOCRCharToWordhOCRVersionKey),
                hOCRCharToWordModuleVersion = item.ReadString(Rom.hOCRCharToWordModuleVersionKey),
                hOCRFtsTexthOCRVersion = item.ReadString(Rom.hOCRFtsTexthOCRVersionKey),
                hOCRFtsTextModuleVersion = item.ReadString(Rom.hOCRFtsTextModuleVersionKey),
                hOCRPageIndexhOCRVersion = item.ReadString(Rom.hOCRPageIndexhOCRVersionKey),
                hOCRPageIndexModuleVersion = item.ReadString(Rom.hOCRPageIndexModuleVersionKey),
                TesseractOCR = item.ReadString(key: Rom.TesseractOCRKey),
                TesseractOCRConverted = item.ReadString(Rom.TesseractOCRConvertedKey),
                TesseractOCRDetectedLang = item.ReadString(Rom.TesseractOCRDetectedLangKey),
                TesseractOCRDetectedLangConf = item.ReadString(Rom.TesseractOCRDetectedLangConfKey),
                TesseractOCRDetectedScript = item.ReadString(Rom.TesseractOCRDetectedScriptKey),
                TesseractOCRDetectedScriptConf = item.ReadString(Rom.TesseractOCRDetectedScriptConfKey),
                TesseractOCRModuleVersion = item.ReadString(Rom.TesseractOCRModuleVersionKey),
                TesseractOCRParameters = item.ReadString(Rom.TesseractOCRParametersKey),
                PDFModuleVersion = item.ReadString(Rom.PDFModuleVersionKey),
                WordConfidenceInterval0To10 = item.ReadString(Rom.WordConfidenceInterval0To10Key),
                WordConfidenceInterval11To20 = item.ReadString(Rom.WordConfidenceInterval11To20Key),
                WordConfidenceInterval21To30 = item.ReadString(Rom.WordConfidenceInterval21To30Key),
                WordConfidenceInterval31To40 = item.ReadString(Rom.WordConfidenceInterval31To40Key),
                WordConfidenceInterval41To50 = item.ReadString(Rom.WordConfidenceInterval41To50Key),
                WordConfidenceInterval51To60 = item.ReadString(Rom.WordConfidenceInterval51To60Key),
                WordConfidenceInterval61To70 = item.ReadString(Rom.WordConfidenceInterval61To70Key),
                WordConfidenceInterval71To80 = item.ReadString(Rom.WordConfidenceInterval71To80Key),
                WordConfidenceInterval81To90 = item.ReadString(Rom.WordConfidenceInterval81To90Key),
                WordConfidenceInterval91To100 = item.ReadString(Rom.WordConfidenceInterval91To100Key),

                Album = item.ReadString(Rom.AlbumKey),
                Artist = item.ReadString(Rom.ArtistKey),
                Bitrate = item.ReadString(Rom.BitrateKey),
                Creator = item.ReadString(Rom.CreatorKey),
                Height = item.ReadString(Rom.HeightKey),
                Length = item.ReadString(Rom.LengthKey),
                PreviewImage = item.ReadString(Rom.PreviewImageKey),
                Rotation = item.ReadString(Rom.RotationKey),
                Title = item.ReadString(Rom.TitleKey),
                Track = item.ReadString(Rom.TrackKey),
                Width = item.ReadString(Rom.WidthKey),
            };
            return file;
        }

        #endregion
    }
}