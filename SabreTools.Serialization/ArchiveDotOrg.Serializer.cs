using System.Linq;
using SabreTools.Models.ArchiveDotOrg;

namespace SabreTools.Serialization
{
    /// <summary>
    /// XML serializer for archive.org metadata files
    /// </summary>
    public partial class ArchiveDotOrg : XmlSerializer<Files>
    {
        #region Internal

        /// <summary>
        /// Convert from <cref="Models.ArchiveDotOrg.Files"/> to <cref="Models.Internal.MetadataFile"/>
        /// </summary>
        public static Models.Internal.MetadataFile ConvertToInternalModel(Files item)
        {
            var metadataFile = new Models.Internal.MetadataFile
            {
                [Models.Internal.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item),
            };

            if (item?.File != null && item.File.Any())
                metadataFile[Models.Internal.MetadataFile.MachineKey] = item.File.Select(ConvertMachineToInternalModel).ToArray();

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.ArchiveDotOrg.Files"/> to <cref="Models.Internal.Header"/>
        /// </summary>
        private static Models.Internal.Header ConvertHeaderToInternalModel(Files item)
        {
            var header = new Models.Internal.Header
            {
                [Models.Internal.Header.NameKey] = "archive.org",
            };
            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.ArchiveDotOrg.File"/> to <cref="Models.Internal.Machine"/>
        /// </summary>
        private static Models.Internal.Machine ConvertMachineToInternalModel(File item)
        {
            var machine = new Models.Internal.Machine
            {
                [Models.Internal.Machine.RomKey] = ConvertToInternalModel(item),
            };
            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.ArchiveDotOrg.File"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        private static Models.Internal.Rom ConvertToInternalModel(File item)
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
    }
}