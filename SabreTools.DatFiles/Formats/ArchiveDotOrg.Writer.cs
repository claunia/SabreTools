using System;
using System.Collections.Generic;
using System.Linq;
using SabreTools.Core;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing and writing of a Archive.org file list
    /// </summary>
    internal partial class ArchiveDotOrg : DatFile
    {
        /// <inheritdoc/>
        protected override ItemType[] GetSupportedTypes()
        {
            return
            [
                ItemType.Rom,
            ];
        }

        /// <inheritdoc/>
        protected override List<string>? GetMissingRequiredFields(DatItem datItem)
        {
            var missingFields = new List<string>();

            // Check item name
            if (string.IsNullOrEmpty(datItem.GetName()))
                missingFields.Add(Models.Metadata.Rom.NameKey);

            switch (datItem)
            {
                case Rom rom:
                    if (rom.GetFieldValue<long?>(Models.Metadata.Rom.SizeKey) == null || rom.GetFieldValue<long?>(Models.Metadata.Rom.SizeKey) < 0)
                        missingFields.Add(Models.Metadata.Rom.SizeKey);
                    if (string.IsNullOrEmpty(rom.GetFieldValue<string?>(Models.Metadata.Rom.CRCKey))
                        && string.IsNullOrEmpty(rom.GetFieldValue<string?>(Models.Metadata.Rom.MD5Key))
                        && string.IsNullOrEmpty(rom.GetFieldValue<string?>(Models.Metadata.Rom.SHA1Key)))
                    {
                        missingFields.Add(Models.Metadata.Rom.SHA1Key);
                    }
                    break;
            }

            return missingFields;
        }

        /// <inheritdoc/>
        public override bool WriteToFile(string outfile, bool ignoreblanks = false, bool throwOnError = false)
        {
            try
            {
                logger.User($"Writing to '{outfile}'...");

                var files = CreateFiles(ignoreblanks);
                if (!(new Serialization.Files.ArchiveDotOrg().Serialize(files, outfile)))
                {
                    logger.Warning($"File '{outfile}' could not be written! See the log for more details.");
                    return false;
                }
            }
            catch (Exception ex) when (!throwOnError)
            {
                logger.Error(ex);
                return false;
            }

            logger.User($"'{outfile}' written!{Environment.NewLine}");
            return true;
        }

        #region Converters

        /// <summary>
        /// Create a Files from the current internal information
        /// <summary>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise</param>
        private Models.ArchiveDotOrg.Files? CreateFiles(bool ignoreblanks)
        {
            // If we don't have items, we can't do anything
            if (this.Items == null || !this.Items.Any())
                return null;

            // Create a list of hold the files
            var files = new List<Models.ArchiveDotOrg.File>();

            // Loop through the sorted items and create games for them
            foreach (string key in Items.SortedKeys)
            {
                var items = Items.FilteredItems(key);
                if (items == null || !items.Any())
                    continue;

                // Loop through and convert the items to respective lists
                for (int index = 0; index < items.Count; index++)
                {
                    // Get the item
                    var item = items[index];

                    // Check for a "null" item
                    item = ProcessNullifiedItem(item);

                    // Skip if we're ignoring the item
                    if (ShouldIgnore(item, ignoreblanks))
                        continue;

                    switch (item)
                    {
                        case Rom rom:
                            files.Add(CreateFile(rom));
                            break;
                    }
                }
            }

            return new Models.ArchiveDotOrg.Files { File = [.. files] };
        }

        /// <summary>
        /// Create a File from the current Rom DatItem
        /// <summary>
        private static Models.ArchiveDotOrg.File CreateFile(Rom item)
        {
            var file = new Models.ArchiveDotOrg.File
            {
                Name = item.GetName(),
                Source = item.GetFieldValue<string?>(Models.Metadata.Rom.SourceKey),
                BitTorrentMagnetHash = item.GetFieldValue<string?>(Models.Metadata.Rom.BitTorrentMagnetHashKey),
                Size = item.GetFieldValue<long?>(Models.Metadata.Rom.SizeKey)?.ToString(),
                MD5 = item.GetFieldValue<string?>(Models.Metadata.Rom.MD5Key),
                CRC32 = item.GetFieldValue<string?>(Models.Metadata.Rom.CRCKey),
                SHA1 = item.GetFieldValue<string?>(Models.Metadata.Rom.SHA1Key),
                FileCount = item.GetFieldValue<string?>(Models.Metadata.Rom.FileCountKey),
                Format = item.GetFieldValue<string?>(Models.Metadata.Rom.FormatKey),
                Original = item.GetFieldValue<string?>(Models.Metadata.Rom.OriginalKey),
                Summation = item.GetFieldValue<string?>(Models.Metadata.Rom.SummationKey),
                MatrixNumber = item.GetFieldValue<string?>(Models.Metadata.Rom.MatrixNumberKey),
                CollectionCatalogNumber = item.GetFieldValue<string?>(Models.Metadata.Rom.CollectionCatalogNumberKey),

                // ASR-Related
                ASRDetectedLang = item.GetFieldValue<string?>(Models.Metadata.Rom.ASRDetectedLangKey),
                ASRDetectedLangConf = item.GetFieldValue<string?>(Models.Metadata.Rom.ASRDetectedLangConfKey),
                ASRTranscribedLang = item.GetFieldValue<string?>(Models.Metadata.Rom.ASRTranscribedLangKey),
                WhisperASRModuleVersion = item.GetFieldValue<string?>(Models.Metadata.Rom.WhisperASRModuleVersionKey),
                WhisperModelHash = item.GetFieldValue<string?>(Models.Metadata.Rom.WhisperModelHashKey),
                WhisperModelName = item.GetFieldValue<string?>(Models.Metadata.Rom.WhisperModelNameKey),
                WhisperVersion = item.GetFieldValue<string?>(Models.Metadata.Rom.WhisperVersionKey),

                // OCR-Related
                ClothCoverDetectionModuleVersion = item.GetFieldValue<string?>(Models.Metadata.Rom.ClothCoverDetectionModuleVersionKey),
                hOCRCharToWordhOCRVersion = item.GetFieldValue<string?>(Models.Metadata.Rom.hOCRCharToWordhOCRVersionKey),
                hOCRCharToWordModuleVersion = item.GetFieldValue<string?>(Models.Metadata.Rom.hOCRCharToWordModuleVersionKey),
                hOCRFtsTexthOCRVersion = item.GetFieldValue<string?>(Models.Metadata.Rom.hOCRFtsTexthOCRVersionKey),
                hOCRFtsTextModuleVersion = item.GetFieldValue<string?>(Models.Metadata.Rom.hOCRFtsTextModuleVersionKey),
                hOCRPageIndexhOCRVersion = item.GetFieldValue<string?>(Models.Metadata.Rom.hOCRPageIndexhOCRVersionKey),
                hOCRPageIndexModuleVersion = item.GetFieldValue<string?>(Models.Metadata.Rom.hOCRPageIndexModuleVersionKey),
                TesseractOCR = item.GetFieldValue<string?>(Models.Metadata.Rom.TesseractOCRKey),
                TesseractOCRConverted = item.GetFieldValue<string?>(Models.Metadata.Rom.TesseractOCRConvertedKey),
                TesseractOCRDetectedLang = item.GetFieldValue<string?>(Models.Metadata.Rom.TesseractOCRDetectedLangKey),
                TesseractOCRDetectedLangConf = item.GetFieldValue<string?>(Models.Metadata.Rom.TesseractOCRDetectedLangConfKey),
                TesseractOCRDetectedScript = item.GetFieldValue<string?>(fieldName: Models.Metadata.Rom.TesseractOCRDetectedScriptKey),
                TesseractOCRDetectedScriptConf = item.GetFieldValue<string?>(Models.Metadata.Rom.TesseractOCRDetectedScriptConfKey),
                TesseractOCRParameters = item.GetFieldValue<string?>(Models.Metadata.Rom.TesseractOCRParametersKey),
                TesseractOCRModuleVersion = item.GetFieldValue<string?>(Models.Metadata.Rom.TesseractOCRModuleVersionKey),
                PDFModuleVersion = item.GetFieldValue<string?>(Models.Metadata.Rom.PDFModuleVersionKey),
                WordConfidenceInterval0To10 = item.GetFieldValue<string?>(Models.Metadata.Rom.WordConfidenceInterval0To10Key),
                WordConfidenceInterval11To20 = item.GetFieldValue<string?>(Models.Metadata.Rom.WordConfidenceInterval11To20Key),
                WordConfidenceInterval21To30 = item.GetFieldValue<string?>(Models.Metadata.Rom.WordConfidenceInterval21To30Key),
                WordConfidenceInterval31To40 = item.GetFieldValue<string?>(Models.Metadata.Rom.WordConfidenceInterval31To40Key),
                WordConfidenceInterval41To50 = item.GetFieldValue<string?>(Models.Metadata.Rom.WordConfidenceInterval41To50Key),
                WordConfidenceInterval51To60 = item.GetFieldValue<string?>(Models.Metadata.Rom.WordConfidenceInterval51To60Key),
                WordConfidenceInterval61To70 = item.GetFieldValue<string?>(Models.Metadata.Rom.WordConfidenceInterval61To70Key),
                WordConfidenceInterval71To80 = item.GetFieldValue<string?>(Models.Metadata.Rom.WordConfidenceInterval71To80Key),
                WordConfidenceInterval81To90 = item.GetFieldValue<string?>(Models.Metadata.Rom.WordConfidenceInterval81To90Key),
                WordConfidenceInterval91To100 = item.GetFieldValue<string?>(Models.Metadata.Rom.WordConfidenceInterval91To100Key),

                // Media-Related
                Album = item.GetFieldValue<string?>(Models.Metadata.Rom.AlbumKey),
                Artist = item.GetFieldValue<string?>(Models.Metadata.Rom.ArtistKey),
                Bitrate = item.GetFieldValue<string?>(Models.Metadata.Rom.BitrateKey),
                Creator = item.GetFieldValue<string?>(Models.Metadata.Rom.CreatorKey),
                Height = item.GetFieldValue<string?>(Models.Metadata.Rom.HeightKey),
                Length = item.GetFieldValue<string?>(Models.Metadata.Rom.LengthKey),
                PreviewImage = item.GetFieldValue<string?>(Models.Metadata.Rom.PreviewImageKey),
                Rotation = item.GetFieldValue<string?>(Models.Metadata.Rom.RotationKey),
                Title = item.GetFieldValue<string?>(Models.Metadata.Rom.TitleKey),
                Track = item.GetFieldValue<string?>(Models.Metadata.Rom.TrackKey),
                Width = item.GetFieldValue<string?>(Models.Metadata.Rom.WidthKey),

            };

            if (long.TryParse(item.GetFieldValue<string?>(Models.Metadata.Rom.DateKey) ?? string.Empty, out long lastModifiedTime))
                file.LastModifiedTime = lastModifiedTime.ToString();

            return file;
        }

        #endregion
    }
}
