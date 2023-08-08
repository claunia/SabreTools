using System.Linq;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for ArchiveDotOrg models to internal structure
    /// </summary>
    public partial class Internal
    {
        #region Serialize

        /// <summary>
        /// Convert from <cref="Models.ArchiveDotOrg.File"/> to <cref="Models.Internal.Machine"/>
        /// </summary>
        public static Models.Internal.Machine ConvertMachineFromArchiveDotOrg(Models.ArchiveDotOrg.File item)
        {
            var machine = new Models.Internal.Machine
            {
                [Models.Internal.Machine.RomKey] = ConvertFromArchiveDotOrg(item),
            };
            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.ArchiveDotOrg.File"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Rom ConvertFromArchiveDotOrg(Models.ArchiveDotOrg.File item)
        {
            var rom = new Models.Internal.Rom
            {
                [Models.Internal.Rom.NameKey] = item.Name,
                [Models.Internal.Rom.SourceKey] = item.Source,
                [Models.Internal.Rom.BitTorrentMagnetHashKey] = item.BitTorrentMagnetHash,
                [Models.Internal.Rom.LastModifiedTimeKey] = item.LastModifiedTime,
                [Models.Internal.Rom.SizeKey] = item.Size,
                [Models.Internal.Rom.MD5Key] = item.MD5,
                [Models.Internal.Rom.CRCKey] = item.CRC32,
                [Models.Internal.Rom.SHA1Key] = item.SHA1,
                [Models.Internal.Rom.FileCountKey] = item.FileCount,
                [Models.Internal.Rom.FormatKey] = item.Format,
                [Models.Internal.Rom.OriginalKey] = item.Original,
                [Models.Internal.Rom.SummationKey] = item.Summation,
                [Models.Internal.Rom.MatrixNumberKey] = item.MatrixNumber,
                [Models.Internal.Rom.CollectionCatalogNumberKey] = item.CollectionCatalogNumber,
                [Models.Internal.Rom.PublisherKey] = item.Publisher,
                [Models.Internal.Rom.CommentKey] = item.Comment,

                [Models.Internal.Rom.ASRDetectedLangKey] = item.ASRDetectedLang,
                [Models.Internal.Rom.ASRDetectedLangConfKey] = item.ASRDetectedLangConf,
                [Models.Internal.Rom.ASRTranscribedLangKey] = item.ASRTranscribedLang,
                [Models.Internal.Rom.WhisperASRModuleVersionKey] = item.WhisperASRModuleVersion,
                [Models.Internal.Rom.WhisperModelHashKey] = item.WhisperModelHash,
                [Models.Internal.Rom.WhisperModelNameKey] = item.WhisperModelName,
                [Models.Internal.Rom.WhisperVersionKey] = item.WhisperVersion,

                [Models.Internal.Rom.ClothCoverDetectionModuleVersionKey] = item.ClothCoverDetectionModuleVersion,
                [Models.Internal.Rom.hOCRCharToWordhOCRVersionKey] = item.hOCRCharToWordhOCRVersion,
                [Models.Internal.Rom.hOCRCharToWordModuleVersionKey] = item.hOCRCharToWordModuleVersion,
                [Models.Internal.Rom.hOCRFtsTexthOCRVersionKey] = item.hOCRFtsTexthOCRVersion,
                [Models.Internal.Rom.hOCRFtsTextModuleVersionKey] = item.hOCRFtsTextModuleVersion,
                [Models.Internal.Rom.hOCRPageIndexhOCRVersionKey] = item.hOCRPageIndexhOCRVersion,
                [Models.Internal.Rom.hOCRPageIndexModuleVersionKey] = item.hOCRPageIndexModuleVersion,
                [Models.Internal.Rom.TesseractOCRKey] = item.TesseractOCR,
                [Models.Internal.Rom.TesseractOCRConvertedKey] = item.TesseractOCRConverted,
                [Models.Internal.Rom.TesseractOCRDetectedLangKey] = item.TesseractOCRDetectedLang,
                [Models.Internal.Rom.TesseractOCRDetectedLangConfKey] = item.TesseractOCRDetectedLangConf,
                [Models.Internal.Rom.TesseractOCRDetectedScriptKey] = item.TesseractOCRDetectedScript,
                [Models.Internal.Rom.TesseractOCRDetectedScriptConfKey] = item.TesseractOCRDetectedScriptConf,
                [Models.Internal.Rom.TesseractOCRModuleVersionKey] = item.TesseractOCRModuleVersion,
                [Models.Internal.Rom.TesseractOCRParametersKey] = item.TesseractOCRParameters,
                [Models.Internal.Rom.PDFModuleVersionKey] = item.PDFModuleVersion,
                [Models.Internal.Rom.WordConfidenceInterval0To10Key] = item.WordConfidenceInterval0To10,
                [Models.Internal.Rom.WordConfidenceInterval11To20Key] = item.WordConfidenceInterval11To20,
                [Models.Internal.Rom.WordConfidenceInterval21To30Key] = item.WordConfidenceInterval21To30,
                [Models.Internal.Rom.WordConfidenceInterval31To40Key] = item.WordConfidenceInterval31To40,
                [Models.Internal.Rom.WordConfidenceInterval41To50Key] = item.WordConfidenceInterval41To50,
                [Models.Internal.Rom.WordConfidenceInterval51To60Key] = item.WordConfidenceInterval51To60,
                [Models.Internal.Rom.WordConfidenceInterval61To70Key] = item.WordConfidenceInterval61To70,
                [Models.Internal.Rom.WordConfidenceInterval71To80Key] = item.WordConfidenceInterval71To80,
                [Models.Internal.Rom.WordConfidenceInterval81To90Key] = item.WordConfidenceInterval81To90,
                [Models.Internal.Rom.WordConfidenceInterval91To100Key] = item.WordConfidenceInterval91To100,

                [Models.Internal.Rom.AlbumKey] = item.Album,
                [Models.Internal.Rom.ArtistKey] = item.Artist,
                [Models.Internal.Rom.BitrateKey] = item.Bitrate,
                [Models.Internal.Rom.CreatorKey] = item.Creator,
                [Models.Internal.Rom.HeightKey] = item.Height,
                [Models.Internal.Rom.LengthKey] = item.Length,
                [Models.Internal.Rom.PreviewImageKey] = item.PreviewImage,
                [Models.Internal.Rom.RotationKey] = item.Rotation,
                [Models.Internal.Rom.TitleKey] = item.Title,
                [Models.Internal.Rom.TrackKey] = item.Track,
                [Models.Internal.Rom.WidthKey] = item.Width,
            };
            return rom;
        }

        #endregion

        #region Deserialize

        /// <summary>
        /// Convert from <cref="Models.Internal.Machine"/> to an array of <cref="Models.ArchiveDotOrg.File"/>
        /// </summary>
        public static Models.ArchiveDotOrg.File?[]? ConvertMachineToArchiveDotOrg(Models.Internal.Machine? item)
        {
            if (item == null)
                return null;

            var roms = item.Read<Models.Internal.Rom[]>(Models.Internal.Machine.RomKey);
            return roms?.Select(ConvertToArchiveDotOrg)?.ToArray();
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.ArchiveDotOrg.File"/>
        /// </summary>
        public static Models.ArchiveDotOrg.File? ConvertToArchiveDotOrg(Models.Internal.Rom? item)
        {
            if (item == null)
                return null;

            var file = new Models.ArchiveDotOrg.File
            {
                Name = item.ReadString(Models.Internal.Rom.NameKey),
                Source = item.ReadString(Models.Internal.Rom.SourceKey),
                BitTorrentMagnetHash = item.ReadString(Models.Internal.Rom.BitTorrentMagnetHashKey),
                LastModifiedTime = item.ReadString(Models.Internal.Rom.LastModifiedTimeKey),
                Size = item.ReadString(Models.Internal.Rom.SizeKey),
                MD5 = item.ReadString(Models.Internal.Rom.MD5Key),
                CRC32 = item.ReadString(Models.Internal.Rom.CRCKey),
                SHA1 = item.ReadString(Models.Internal.Rom.SHA1Key),
                FileCount = item.ReadString(Models.Internal.Rom.FileCountKey),
                Format = item.ReadString(Models.Internal.Rom.FormatKey),
                Original = item.ReadString(Models.Internal.Rom.OriginalKey),
                Summation = item.ReadString(Models.Internal.Rom.SummationKey),
                MatrixNumber = item.ReadString(Models.Internal.Rom.MatrixNumberKey),
                CollectionCatalogNumber = item.ReadString(Models.Internal.Rom.CollectionCatalogNumberKey),
                Comment = item.ReadString(Models.Internal.Rom.CommentKey),

                ASRDetectedLang = item.ReadString(Models.Internal.Rom.ASRDetectedLangKey),
                ASRDetectedLangConf = item.ReadString(Models.Internal.Rom.ASRDetectedLangConfKey),
                ASRTranscribedLang = item.ReadString(Models.Internal.Rom.ASRTranscribedLangKey),
                WhisperASRModuleVersion = item.ReadString(Models.Internal.Rom.WhisperASRModuleVersionKey),
                WhisperModelHash = item.ReadString(Models.Internal.Rom.WhisperModelHashKey),
                WhisperModelName = item.ReadString(Models.Internal.Rom.WhisperModelNameKey),
                WhisperVersion = item.ReadString(Models.Internal.Rom.WhisperVersionKey),

                ClothCoverDetectionModuleVersion = item.ReadString(Models.Internal.Rom.ClothCoverDetectionModuleVersionKey),
                hOCRCharToWordhOCRVersion = item.ReadString(Models.Internal.Rom.hOCRCharToWordhOCRVersionKey),
                hOCRCharToWordModuleVersion = item.ReadString(Models.Internal.Rom.hOCRCharToWordModuleVersionKey),
                hOCRFtsTexthOCRVersion = item.ReadString(Models.Internal.Rom.hOCRFtsTexthOCRVersionKey),
                hOCRFtsTextModuleVersion = item.ReadString(Models.Internal.Rom.hOCRFtsTextModuleVersionKey),
                hOCRPageIndexhOCRVersion = item.ReadString(Models.Internal.Rom.hOCRPageIndexhOCRVersionKey),
                hOCRPageIndexModuleVersion = item.ReadString(Models.Internal.Rom.hOCRPageIndexModuleVersionKey),
                TesseractOCR = item.ReadString(key: Models.Internal.Rom.TesseractOCRKey),
                TesseractOCRConverted = item.ReadString(Models.Internal.Rom.TesseractOCRConvertedKey),
                TesseractOCRDetectedLang = item.ReadString(Models.Internal.Rom.TesseractOCRDetectedLangKey),
                TesseractOCRDetectedLangConf = item.ReadString(Models.Internal.Rom.TesseractOCRDetectedLangConfKey),
                TesseractOCRDetectedScript = item.ReadString(Models.Internal.Rom.TesseractOCRDetectedScriptKey),
                TesseractOCRDetectedScriptConf = item.ReadString(Models.Internal.Rom.TesseractOCRDetectedScriptConfKey),
                TesseractOCRModuleVersion = item.ReadString(Models.Internal.Rom.TesseractOCRModuleVersionKey),
                TesseractOCRParameters = item.ReadString(Models.Internal.Rom.TesseractOCRParametersKey),
                PDFModuleVersion = item.ReadString(Models.Internal.Rom.PDFModuleVersionKey),
                WordConfidenceInterval0To10 = item.ReadString(Models.Internal.Rom.WordConfidenceInterval0To10Key),
                WordConfidenceInterval11To20 = item.ReadString(Models.Internal.Rom.WordConfidenceInterval11To20Key),
                WordConfidenceInterval21To30 = item.ReadString(Models.Internal.Rom.WordConfidenceInterval21To30Key),
                WordConfidenceInterval31To40 = item.ReadString(Models.Internal.Rom.WordConfidenceInterval31To40Key),
                WordConfidenceInterval41To50 = item.ReadString(Models.Internal.Rom.WordConfidenceInterval41To50Key),
                WordConfidenceInterval51To60 = item.ReadString(Models.Internal.Rom.WordConfidenceInterval51To60Key),
                WordConfidenceInterval61To70 = item.ReadString(Models.Internal.Rom.WordConfidenceInterval61To70Key),
                WordConfidenceInterval71To80 = item.ReadString(Models.Internal.Rom.WordConfidenceInterval71To80Key),
                WordConfidenceInterval81To90 = item.ReadString(Models.Internal.Rom.WordConfidenceInterval81To90Key),
                WordConfidenceInterval91To100 = item.ReadString(Models.Internal.Rom.WordConfidenceInterval91To100Key),

                Album = item.ReadString(Models.Internal.Rom.AlbumKey),
                Artist = item.ReadString(Models.Internal.Rom.ArtistKey),
                Bitrate = item.ReadString(Models.Internal.Rom.BitrateKey),
                Creator = item.ReadString(Models.Internal.Rom.CreatorKey),
                Height = item.ReadString(Models.Internal.Rom.HeightKey),
                Length = item.ReadString(Models.Internal.Rom.LengthKey),
                PreviewImage = item.ReadString(Models.Internal.Rom.PreviewImageKey),
                Rotation = item.ReadString(Models.Internal.Rom.RotationKey),
                Title = item.ReadString(Models.Internal.Rom.TitleKey),
                Track = item.ReadString(Models.Internal.Rom.TrackKey),
                Width = item.ReadString(Models.Internal.Rom.WidthKey),
            };
            return file;
        }

        #endregion
    }
}