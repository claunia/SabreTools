using System;
using System.Linq;
using SabreTools.Models.ArchiveDotOrg;

namespace SabreTools.Serialization
{
    /// <summary>
    /// XML deserializer for archive.org metadata files
    /// </summary>
    public partial class ArchiveDotOrg : XmlSerializer<Files>
    {
        #region Internal

        /// <summary>
        /// Convert from <cref="Models.Internal.MetadataFile"/> to an array of <cref="Models.ArchiveDotOrg.Files"/>
        /// </summary>
        public static Files? ConvertFromInternalModel(Models.Internal.MetadataFile? item)
        {
            if (item == null)
                return null;

            var files = new Files();

            var machines = item.Read<Models.Internal.Machine[]>(Models.Internal.MetadataFile.MachineKey);
            if (machines != null && machines.Any())
            {
                files.File = machines
                .Where(m => m != null)
                .SelectMany(ConvertFromInternalModel)
                .ToArray();
            }

            return files;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Machine"/> to an array of <cref="Models.ArchiveDotOrg.File"/>
        /// </summary>
        private static File[] ConvertFromInternalModel(Models.Internal.Machine item)
        {
            var roms = item.Read<Models.Internal.Rom[]>(Models.Internal.Machine.RomKey);
            if (roms == null)
                return Array.Empty<File>();

            return roms
                .Where(r => r != null)
                .Select(ConvertFromInternalModel).ToArray();
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.ArchiveDotOrg.File"/>
        /// </summary>
        private static File ConvertFromInternalModel(Models.Internal.Rom item)
        {
            var file = new File
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
                TesseractOCR = item.ReadString(Models.Internal.Rom.TesseractOCRKey),
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