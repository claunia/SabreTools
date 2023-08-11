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
                var files = Serialization.ArchiveDotOrg.Deserialize(filename);

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
            if (string.IsNullOrWhiteSpace(filename))
                return (null, null);

            string machineName = Path.GetFileNameWithoutExtension(filename);
            if (filename.Contains('/'))
            {
                string[] split = filename.Split('/');
                machineName = split[0];
                filename = filename[(machineName.Length + 1)..];
            }
            else if (filename.Contains('\\'))
            {
                string[] split = filename.Split('\\');
                machineName = split[0];
                filename = filename[(machineName.Length + 1)..];
            }

            var machine = new Machine { Name = machineName };
            return (machine, filename);
        }

        /// <summary>
        /// Convert files information
        /// </summary>
        /// <param name="files">Array of deserialized models to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertFiles(Models.ArchiveDotOrg.File[]? files, string filename, int indexId, bool statsOnly)
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
            machine ??= new Machine { Name = Path.GetFileNameWithoutExtension(file.Name) };

            machine.Publisher = file.Publisher;
            machine.Comment = file.Comment;

            var rom = new Rom()
            {
                Name = name,
                ArchiveDotOrgSource = file.Source,
                //BitTorrentMagnetHash = file.BitTorrentMagnetHash, // TODO: Add to internal model
                Date = file.LastModifiedTime?.ToString(),
                Size = Utilities.CleanLong(file.Size),
                MD5 = file.MD5,
                CRC = file.CRC32,
                SHA1 = file.SHA1,
                //FileCount = file.FileCount, // TODO: Add to internal model
                ArchiveDotOrgFormat = file.Format,
                //Original = file.Original, // TODO: Add to internal model
                Summation = file.Summation,
                //MatrixNumber = file.MatrixNumber, // TODO: Add to internal model
                //CollectionCatalogNumber = file.CollectionCatalogNumber, // TODO: Add to internal model

                // ASR-Related
                //ASRDetectedLang = file.ASRDetectedLang, // TODO: Add to internal model
                //ASRDetectedLangConf = file.ASRDetectedLangConf, // TODO: Add to internal model
                //ASRTranscribedLang = file.ASRTranscribedLang, // TODO: Add to internal model
                //WhisperASRModuleVersion = file.WhisperASRModuleVersion, // TODO: Add to internal model
                //WhisperModelHash = file.WhisperModelHash, // TODO: Add to internal model
                //WhisperModelName = file.WhisperModelName, // TODO: Add to internal model
                //WhisperVersion = file.WhisperVersion, // TODO: Add to internal model

                // OCR-Related
                //ClothCoverDetectionModuleVersion = file.ClothCoverDetectionModuleVersions, // TODO: Add to internal model
                //hOCRCharToWordhOCRVersion = file.hOCRCharToWordhOCRVersion, // TODO: Add to internal model
                //hOCRCharToWordModuleVersion = file.hOCRCharToWordModuleVersion, // TODO: Add to internal model
                //hOCRFtsTexthOCRVersion = file.hOCRFtsTexthOCRVersion, // TODO: Add to internal model
                //hOCRFtsTextModuleVersion = file.hOCRFtsTextModuleVersion, // TODO: Add to internal model
                //hOCRPageIndexhOCRVersion = file.hOCRPageIndexhOCRVersion, // TODO: Add to internal model
                //hOCRPageIndexModuleVersion = file.hOCRPageIndexModuleVersion, // TODO: Add to internal model
                //TesseractOCR = file.TesseractOCR, // TODO: Add to internal model
                //TesseractOCRConverted = file.TesseractOCRConverted, // TODO: Add to internal model
                //TesseractOCRDetectedLang = file.TesseractOCRDetectedLang, // TODO: Add to internal model
                //TesseractOCRDetectedLangConf = file.TesseractOCRDetectedLangConf, // TODO: Add to internal model
                //TesseractOCRDetectedScript = file.TesseractOCRDetectedScript, // TODO: Add to internal model
                //TesseractOCRDetectedScriptConf = file.TesseractOCRDetectedScriptConf, // TODO: Add to internal model
                //TesseractOCRParameters = file.TesseractOCRParameters, // TODO: Add to internal model
                //TesseractOCRModuleVersion = file.TesseractOCRModuleVersion, // TODO: Add to internal model
                //PDFModuleVersion = file.PDFModuleVersion, // TODO: Add to internal model
                //WordConfidenceInterval0To10 = file.WordConfidenceInterval0To10, // TODO: Add to internal model
                //WordConfidenceInterval11To20 = file.WordConfidenceInterval11To20, // TODO: Add to internal model
                //WordConfidenceInterval21To30 = file.WordConfidenceInterval21To30, // TODO: Add to internal model
                //WordConfidenceInterval31To40 = file.WordConfidenceInterval31To40, // TODO: Add to internal model
                //WordConfidenceInterval41To50 = file.WordConfidenceInterval41To50, // TODO: Add to internal model
                //WordConfidenceInterval51To60 = file.WordConfidenceInterval51To60, // TODO: Add to internal model
                //WordConfidenceInterval61To70 = file.WordConfidenceInterval61To70, // TODO: Add to internal model
                //WordConfidenceInterval71To80 = file.WordConfidenceInterval71To80, // TODO: Add to internal model
                //WordConfidenceInterval81To90 = file.WordConfidenceInterval81To90, // TODO: Add to internal model
                //WordConfidenceInterval91To100 = file.WordConfidenceInterval91To100, // TODO: Add to internal model

                // Media-Related
                //Album = file.Album, // TODO: Add to internal model
                //Artist = file.Artist, // TODO: Add to internal model
                //Bitrate = file.Bitrate, // TODO: Add to internal model
                //Creator = file.Creator, // TODO: Add to internal model
                //Height = file.Height, // TODO: Add to internal model
                //Length = file.Length, // TODO: Add to internal model
                //PreviewImage = file.PreviewImage, // TODO: Add to internal model
                //Rotation = file.Rotation, // TODO: Add to internal model
                //Title = file.Title, // TODO: Add to internal model
                //Track = file.Track, // TODO: Add to internal model
                //Width = file.Width, // TODO: Add to internal model

                ItemStatus = ItemStatus.None,

                Source = new Source
                {
                    Index = indexId,
                    Name = filename,
                },
            };

            // Now process and add the rom
            rom.CopyMachineInformation(machine);
            ParseAddHelper(rom, statsOnly);
        }

        #endregion
    }
}
