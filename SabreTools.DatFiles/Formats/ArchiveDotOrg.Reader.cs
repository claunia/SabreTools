using System;
using System.IO;
using System.Linq;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing a Archive.org file list
    /// </summary>
    internal partial class ArchiveDotOrg : DatFile
    {
        /// <inheritdoc/>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            try
            {
                // Deserialize the input file
                var files = new Serialization.Files.ArchiveDotOrg().Deserialize(filename);
                var metadata = new Serialization.CrossModel.ArchiveDotOrg().Serialize(files);

                // Convert the files data to the internal format
                ConvertFiles(files?.File, filename, indexId, statsOnly);
            }
            catch (Exception ex) when (!throwOnError)
            {
                string message = $"'{filename}' - An error occurred during parsing";
                logger.Error(ex, message);
            }
        }

        #region Converters

        /// <summary>
        /// Create a machine from the filename
        /// </summary>
        /// <param name="filename">Filename to derive from</param>
        /// <returns>Filled machine and new filename on success, null on error</returns>
        private static (Machine?, string?) DeriveMachine(string? filename)
        {
            // If the filename is missing, we can't do anything
            if (string.IsNullOrEmpty(filename))
                return (null, null);

            string machineName = Path.GetFileNameWithoutExtension(filename);
            if (filename.Contains('/'))
            {
                string[] split = filename!.Split('/');
                machineName = split[0];
                filename = filename.Substring(machineName.Length + 1);
            }
            else if (filename.Contains('\\'))
            {
                string[] split = filename!.Split('\\');
                machineName = split[0];
                filename = filename.Substring(machineName.Length + 1);
            }

            var machine = new Machine();
            machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, machineName);

            return (machine, filename);
        }

        /// <summary>
        /// Convert files information
        /// </summary>
        /// <param name="files">Array of deserialized models to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertFiles(Models.ArchiveDotOrg.File?[]? files, string filename, int indexId, bool statsOnly)
        {
            // If the files array is missing, we can't do anything
            if (files == null || !files.Any())
                return;

            // Loop through the rows and add
            foreach (var file in files)
            {
                ConvertFile(file, filename, indexId, statsOnly);
            }
        }

        /// <summary>
        /// Convert file information
        /// </summary>
        /// <param name="file">Deserialized model to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertFile(Models.ArchiveDotOrg.File? file, string filename, int indexId, bool statsOnly)
        {
            // If the file is missing, we can't do anything
            if (file == null)
                return;

            (var machine, string? name) = DeriveMachine(file.Name);
            if (machine == null)
            {
                machine = new Machine();
                machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, Path.GetFileNameWithoutExtension(file.Name));
            }

            machine.SetFieldValue<string?>(Models.Metadata.Machine.CommentKey, file.Comment);
            machine.SetFieldValue<string?>(Models.Metadata.Machine.PublisherKey, file.Publisher);

            var rom = new Rom()
            {
                Source = new Source { Index = indexId, Name = filename },
            };
            rom.SetName(name);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.AlbumKey, file.Album);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.ArtistKey, file.Artist);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.ASRDetectedLangKey, file.ASRDetectedLang);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.ASRDetectedLangConfKey, file.ASRDetectedLangConf);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.ASRTranscribedLangKey, file.ASRTranscribedLang);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.BitrateKey, file.Bitrate);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.BitTorrentMagnetHashKey, file.BitTorrentMagnetHash);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.ClothCoverDetectionModuleVersionKey, file.ClothCoverDetectionModuleVersion);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.CollectionCatalogNumberKey, file.CollectionCatalogNumber);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, file.CRC32);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.CreatorKey, file.Creator);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.DateKey, file.LastModifiedTime?.ToString());
            rom.SetFieldValue<string?>(Models.Metadata.Rom.FileCountKey, file.FileCount);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.FormatKey, file.Format);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.HeightKey, file.Height);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.hOCRCharToWordhOCRVersionKey, file.hOCRCharToWordhOCRVersion);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.hOCRCharToWordModuleVersionKey, file.hOCRCharToWordModuleVersion);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.hOCRFtsTexthOCRVersionKey, file.hOCRFtsTexthOCRVersion);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.hOCRFtsTextModuleVersionKey, file.hOCRFtsTextModuleVersion);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.hOCRPageIndexhOCRVersionKey, file.hOCRPageIndexhOCRVersion);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.hOCRPageIndexModuleVersionKey, file.hOCRPageIndexModuleVersion);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.LengthKey, file.Length);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.MatrixNumberKey, file.MatrixNumber);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.MD5Key, file.MD5);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.OriginalKey, file.Original);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.PDFModuleVersionKey, file.PDFModuleVersion);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.PreviewImageKey, file.PreviewImage);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.RotationKey, file.Rotation);
            rom.SetFieldValue<long?>(Models.Metadata.Rom.SizeKey, NumberHelper.ConvertToInt64(file.Size));
            rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, file.SHA1);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.SourceKey, file.Source);
            rom.SetFieldValue<ItemStatus>(Models.Metadata.Rom.StatusKey, ItemStatus.None);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.SummationKey, file.Summation);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.TesseractOCRKey, file.TesseractOCR);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.TesseractOCRConvertedKey, file.TesseractOCRConverted);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.TesseractOCRDetectedLangKey, file.TesseractOCRDetectedLang);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.TesseractOCRDetectedLangConfKey, file.TesseractOCRDetectedLangConf);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.TesseractOCRDetectedScriptKey, file.TesseractOCRDetectedScript);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.TesseractOCRDetectedScriptConfKey, file.TesseractOCRDetectedScriptConf);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.TesseractOCRModuleVersionKey, file.TesseractOCRModuleVersion);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.TesseractOCRParametersKey, file.TesseractOCRParameters);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.TitleKey, file.Title);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.TrackKey, file.Track);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.WhisperASRModuleVersionKey, file.WhisperASRModuleVersion);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.WhisperModelHashKey, file.WhisperModelHash);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.WhisperModelNameKey, file.WhisperModelName);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.WhisperVersionKey, file.WhisperVersion);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.WidthKey, file.Width);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.WordConfidenceInterval0To10Key, file.WordConfidenceInterval0To10);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.WordConfidenceInterval11To20Key, file.WordConfidenceInterval11To20);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.WordConfidenceInterval21To30Key, file.WordConfidenceInterval21To30);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.WordConfidenceInterval31To40Key, file.WordConfidenceInterval31To40);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.WordConfidenceInterval41To50Key, file.WordConfidenceInterval41To50);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.WordConfidenceInterval51To60Key, file.WordConfidenceInterval51To60);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.WordConfidenceInterval61To70Key, file.WordConfidenceInterval61To70);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.WordConfidenceInterval71To80Key, file.WordConfidenceInterval71To80);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.WordConfidenceInterval81To90Key, file.WordConfidenceInterval81To90);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.WordConfidenceInterval91To100Key, file.WordConfidenceInterval91To100);

            // Now process and add the rom
            rom.CopyMachineInformation(machine);
            ParseAddHelper(rom, statsOnly);
        }

        #endregion
    }
}
