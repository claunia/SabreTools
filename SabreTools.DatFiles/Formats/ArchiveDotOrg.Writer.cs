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
        protected override List<DatItemField>? GetMissingRequiredFields(DatItem datItem)
        {
            List<DatItemField> missingFields = [];

            // Check item name
            if (string.IsNullOrEmpty(datItem.GetName()))
                missingFields.Add(DatItemField.Name);

            switch (datItem)
            {
                case Rom rom:
                    if (rom.Size == null || rom.Size < 0)
                        missingFields.Add(DatItemField.Size);
                    if (string.IsNullOrEmpty(rom.CRC)
                        && string.IsNullOrEmpty(rom.MD5)
                        && string.IsNullOrEmpty(rom.SHA1))
                    {
                        missingFields.Add(DatItemField.SHA1);
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
                Name = item.Name,
                Source = item.ArchiveDotOrgSource,
                //BitTorrentMagnetHash = item.BitTorrentMagnetHash, // TODO: Add to internal model
                Size = item.Size?.ToString(),
                MD5 = item.MD5,
                CRC32 = item.CRC,
                SHA1 = item.SHA1,
                //FileCount = item.FileCount, // TODO: Add to internal model
                Format = item.ArchiveDotOrgFormat,
                //Original = item.Original, // TODO: Add to internal model
                Summation = item.Summation,
                //MatrixNumber = item.MatrixNumber, // TODO: Add to internal model
                //CollectionCatalogNumber = item.CollectionCatalogNumber, // TODO: Add to internal model

                // ASR-Related
                //ASRDetectedLang = item.ASRDetectedLang, // TODO: Add to internal model
                //ASRDetectedLangConf = item.ASRDetectedLangConf, // TODO: Add to internal model
                //ASRTranscribedLang = item.ASRTranscribedLang, // TODO: Add to internal model
                //WhisperASRModuleVersion = item.WhisperASRModuleVersion, // TODO: Add to internal model
                //WhisperModelHash = item.WhisperModelHash, // TODO: Add to internal model
                //WhisperModelName = item.WhisperModelName, // TODO: Add to internal model
                //WhisperVersion = item.WhisperVersion, // TODO: Add to internal model

                // OCR-Related
                //ClothCoverDetectionModuleVersion = item.ClothCoverDetectionModuleVersions, // TODO: Add to internal model
                //hOCRCharToWordhOCRVersion = item.hOCRCharToWordhOCRVersion, // TODO: Add to internal model
                //hOCRCharToWordModuleVersion = item.hOCRCharToWordModuleVersion, // TODO: Add to internal model
                //hOCRFtsTexthOCRVersion = item.hOCRFtsTexthOCRVersion, // TODO: Add to internal model
                //hOCRFtsTextModuleVersion = item.hOCRFtsTextModuleVersion, // TODO: Add to internal model
                //hOCRPageIndexhOCRVersion = item.hOCRPageIndexhOCRVersion, // TODO: Add to internal model
                //hOCRPageIndexModuleVersion = item.hOCRPageIndexModuleVersion, // TODO: Add to internal model
                //TesseractOCR = item.TesseractOCR, // TODO: Add to internal model
                //TesseractOCRConverted = item.TesseractOCRConverted, // TODO: Add to internal model
                //TesseractOCRDetectedLang = item.TesseractOCRDetectedLang, // TODO: Add to internal model
                //TesseractOCRDetectedLangConf = item.TesseractOCRDetectedLangConf, // TODO: Add to internal model
                //TesseractOCRDetectedScript = item.TesseractOCRDetectedScript, // TODO: Add to internal model
                //TesseractOCRDetectedScriptConf = item.TesseractOCRDetectedScriptConf, // TODO: Add to internal model
                //TesseractOCRParameters = item.TesseractOCRParameters, // TODO: Add to internal model
                //TesseractOCRModuleVersion = item.TesseractOCRModuleVersion, // TODO: Add to internal model
                //PDFModuleVersion = item.PDFModuleVersion, // TODO: Add to internal model
                //WordConfidenceInterval0To10 = item.WordConfidenceInterval0To10, // TODO: Add to internal model
                //WordConfidenceInterval11To20 = item.WordConfidenceInterval11To20, // TODO: Add to internal model
                //WordConfidenceInterval21To30 = item.WordConfidenceInterval21To30, // TODO: Add to internal model
                //WordConfidenceInterval31To40 = item.WordConfidenceInterval31To40, // TODO: Add to internal model
                //WordConfidenceInterval41To50 = item.WordConfidenceInterval41To50, // TODO: Add to internal model
                //WordConfidenceInterval51To60 = item.WordConfidenceInterval51To60, // TODO: Add to internal model
                //WordConfidenceInterval61To70 = item.WordConfidenceInterval61To70, // TODO: Add to internal model
                //WordConfidenceInterval71To80 = item.WordConfidenceInterval71To80, // TODO: Add to internal model
                //WordConfidenceInterval81To90 = item.WordConfidenceInterval81To90, // TODO: Add to internal model
                //WordConfidenceInterval91To100 = item.WordConfidenceInterval91To100, // TODO: Add to internal model

                // Media-Related
                //Album = item.Album, // TODO: Add to internal model
                //Artist = item.Artist, // TODO: Add to internal model
                //Bitrate = item.Bitrate, // TODO: Add to internal model
                //Creator = item.Creator, // TODO: Add to internal model
                //Height = item.Height, // TODO: Add to internal model
                //Length = item.Length, // TODO: Add to internal model
                //PreviewImage = item.PreviewImage, // TODO: Add to internal model
                //Rotation = item.Rotation, // TODO: Add to internal model
                //Title = item.Title, // TODO: Add to internal model
                //Track = item.Track, // TODO: Add to internal model
                //Width = item.Width, // TODO: Add to internal model

            };

            if (long.TryParse(item.Date ?? string.Empty, out long lastModifiedTime))
                file.LastModifiedTime = lastModifiedTime.ToString();

            return file;
        }

        #endregion
    }
}
