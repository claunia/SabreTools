using System;
using System.IO;
using SabreTools.FileTypes.Aaru;
using SabreTools.FileTypes.Archives;
using SabreTools.FileTypes.CHD;
using SabreTools.Hashing;
using SabreTools.IO.Extensions;
using SabreTools.Matching;
using SabreTools.Skippers;

namespace SabreTools.FileTypes
{
    public static class FileTypeTool
    {
        #region Constants

        private static readonly byte[] SevenZipSignature = [0x37, 0x7a, 0xbc, 0xaf, 0x27, 0x1c];
        private static readonly byte[] AaruFormatSignature = [0x41, 0x41, 0x52, 0x55, 0x46, 0x52, 0x4d, 0x54];
        private static readonly byte[] CHDSignature = [0x4d, 0x43, 0x6f, 0x6d, 0x70, 0x72, 0x48, 0x44];
        private static readonly byte[] GzSignature = [0x1f, 0x8b, 0x08];
        private static readonly byte[] RarSignature = [0x52, 0x61, 0x72, 0x21, 0x1a, 0x07, 0x00];
        private static readonly byte[] RarFiveSignature = [0x52, 0x61, 0x72, 0x21, 0x1a, 0x07, 0x01, 0x00];
        private static readonly byte[] TarSignature = [0x75, 0x73, 0x74, 0x61, 0x72, 0x20, 0x20, 0x00];
        private static readonly byte[] TarZeroSignature = [0x75, 0x73, 0x74, 0x61, 0x72, 0x00, 0x30, 0x30];
        private static readonly byte[] XZSignature = [0xfd, 0x37, 0x7a, 0x58, 0x5a, 0x00, 0x00];

        #endregion

        #region File Info

        /// <summary>
        /// Retrieve file information for a single file
        /// </summary>
        /// <param name="input">Filename to get information from</param>
        /// <param name="header">Populated string representing the name of the skipper to use, a blank string to use the first available checker, null otherwise</param>
        /// <param name="hashes">Hashes to include in the information</param>
        /// <param name="asFiles">TreatAsFiles representing special format scanning</param>
        /// <returns>Populated BaseFile object if success, empty one on error</returns>
        public static BaseFile? GetInfo(string input, string? header = null, HashType[]? hashes = null, TreatAsFile asFiles = 0x00)
        {
            // Add safeguard if file doesn't exist
            if (!File.Exists(input))
                return null;

            // If no hashes are set, use the standard array
            hashes ??= [HashType.CRC32, HashType.MD5, HashType.SHA1];

            // Get input information
            var fileType = GetFileType(input);
            Stream inputStream = File.OpenRead(input);

            // Try to match the supplied header skipper
            if (header != null)
            {
                SkipperMatch.Init();
                var rule = SkipperMatch.GetMatchingRule(input, Path.GetFileNameWithoutExtension(header));

                // If there's a match, transform the stream before getting info
                if (rule.Tests != null && rule.Tests.Length != 0)
                {
                    // Create the output stream
                    MemoryStream outputStream = new();

                    // Transform the stream and get the information from it
                    rule.TransformStream(inputStream, outputStream, keepReadOpen: false, keepWriteOpen: true);
                    inputStream = outputStream;
                }
            }

            // Get the info in the proper manner
            BaseFile? baseFile;
#if NET20 || NET35
            if (fileType == FileType.AaruFormat && (asFiles & TreatAsFile.AaruFormat) == 0)
                baseFile = AaruFormat.Create(inputStream);
            else if (fileType == FileType.CHD && (asFiles & TreatAsFile.CHD) == 0)
                baseFile = CHDFile.Create(inputStream);
#else
            if (fileType == FileType.AaruFormat && !asFiles.HasFlag(TreatAsFile.AaruFormat))
                baseFile = AaruFormat.Create(inputStream);
            else if (fileType == FileType.CHD && !asFiles.HasFlag(TreatAsFile.CHD))
                baseFile = CHDFile.Create(inputStream);
#endif
            else
                baseFile = GetInfo(inputStream, hashes: hashes, keepReadOpen: false);

            // Dispose of the input stream
            inputStream?.Dispose();

            // Add unique data from the file
            baseFile!.Filename = Path.GetFileName(input);
            baseFile.Date = new FileInfo(input).LastWriteTime.ToString("yyyy/MM/dd HH:mm:ss");

            return baseFile;
        }

        /// <summary>
        /// Retrieve file information for a single file
        /// </summary>
        /// <param name="input">Filename to get information from</param>
        /// <param name="size">Size of the input stream</param>
        /// <param name="hashes">Hashes to include in the information</param>
        /// <param name="keepReadOpen">True if the underlying read stream should be kept open, false otherwise</param>
        /// <returns>Populated BaseFile object if success, empty one on error</returns>
        public static BaseFile GetInfo(Stream? input, long size = -1, HashType[]? hashes = null, bool keepReadOpen = false)
        {
            // If we have no stream
            if (input == null)
                return new BaseFile();

            // If no hashes are set, use the standard array
            hashes ??= [HashType.CRC32, HashType.MD5, HashType.SHA1];

            // If we want to automatically set the size
            if (size == -1)
                size = input.Length;

            // Run the hashing on the input stream
            var hashDict = HashTool.GetStreamHashes(input, hashes);
            if (hashDict == null)
                return new BaseFile();

            // Create a base file with the resulting hashes
            var baseFile = new BaseFile()
            {
                Size = size,
                CRC = hashDict.ContainsKey(HashType.CRC32) ? hashDict[HashType.CRC32].FromHexString() : null,
                MD5 = hashDict.ContainsKey(HashType.MD5) ? hashDict[HashType.MD5].FromHexString() : null,
                SHA1 = hashDict.ContainsKey(HashType.SHA1) ? hashDict[HashType.SHA1].FromHexString() : null,
                SHA256 = hashDict.ContainsKey(HashType.SHA256) ? hashDict[HashType.SHA256].FromHexString() : null,
                SHA384 = hashDict.ContainsKey(HashType.SHA384) ? hashDict[HashType.SHA384].FromHexString() : null,
                SHA512 = hashDict.ContainsKey(HashType.SHA512) ? hashDict[HashType.SHA512].FromHexString() : null,
                SpamSum = hashDict.ContainsKey(HashType.SpamSum) ? hashDict[HashType.SpamSum].FromHexString() : null,
            };

            // Deal with the input stream
            if (!keepReadOpen)
            {
                input.Close();
                input.Dispose();
            }
            else
            {
                input.SeekIfPossible();
            }

            return baseFile;
        }

        #endregion

        #region File Type

        /// <summary>
        /// Create an archive object from a filename, if possible
        /// </summary>
        /// <param name="input">Name of the file to create the archive from</param>
        /// <returns>Archive object representing the inputs</returns>
        public static BaseArchive? CreateArchiveType(string input)
        {
            FileType? fileType = GetFileType(input);
            return fileType switch
            {
                FileType.GZipArchive => new GZipArchive(input),
                FileType.RarArchive => new RarArchive(input),
                FileType.SevenZipArchive => new SevenZipArchive(input),
                FileType.TapeArchive => new TapeArchive(input),
                FileType.ZipArchive => new ZipArchive(input),
                _ => null,
            };
        }

        /// <summary>
        /// Create an Folder object of the specified type, if possible
        /// </summary>
        /// <param name="outputFormat">OutputFormat representing the archive to create</param>
        /// <returns>Archive object representing the inputs</returns>
        public static Folder? CreateFolderType(OutputFormat outputFormat)
        {
            return outputFormat switch
            {
                OutputFormat.Folder => new Folder(false),
                OutputFormat.ParentFolder => new Folder(true),
                OutputFormat.TapeArchive => new TapeArchive(),
                OutputFormat.Torrent7Zip => new SevenZipArchive(),
                OutputFormat.TorrentGzip => new GZipArchive(),
                OutputFormat.TorrentGzipRomba => new GZipArchive(),
                OutputFormat.TorrentRar => new RarArchive(),
                OutputFormat.TorrentXZ => new XZArchive(),
                OutputFormat.TorrentXZRomba => new XZArchive(),
                OutputFormat.TorrentZip => new ZipArchive(),
                _ => null,
            };
        }

        /// <summary>
        /// Returns the file type of an input file
        /// </summary>
        /// <param name="input">Input file to check</param>
        /// <returns>FileType of inputted file (null on error)</returns>
        public static FileType? GetFileType(string input)
        {
            FileType? outFileType = null;

            // If the file is null, then we have no archive type
            if (input == null)
                return outFileType;

            // First line of defense is going to be the extension, for better or worse
            if (!HasValidArchiveExtension(input))
                return outFileType;

            // Read the first bytes of the file and get the magic number
            BinaryReader br = new(File.OpenRead(input));
            byte[] magic = br.ReadBytes(8);
#if NET40_OR_GREATER
            br.Dispose();
#endif

            // Now try to match it to a known signature
            if (magic.StartsWith(SevenZipSignature))
            {
                outFileType = FileType.SevenZipArchive;
            }
            else if (magic.StartsWith(AaruFormatSignature))
            {
                outFileType = FileType.AaruFormat;
            }
            else if (magic.StartsWith(CHDSignature))
            {
                outFileType = FileType.CHD;
            }
            else if (magic.StartsWith(GzSignature))
            {
                outFileType = FileType.GZipArchive;
            }
            else if (magic.StartsWith(RarSignature)
                || magic.StartsWith(RarFiveSignature))
            {
                outFileType = FileType.RarArchive;
            }
            else if (magic.StartsWith(TarSignature)
                || magic.StartsWith(TarZeroSignature))
            {
                outFileType = FileType.TapeArchive;
            }
            else if (magic.StartsWith(XZSignature))
            {
                outFileType = FileType.XZArchive;
            }
            else if (magic.StartsWith(Models.PKZIP.Constants.LocalFileHeaderSignatureBytes)
                || magic.StartsWith(Models.PKZIP.Constants.EndOfCentralDirectoryRecordSignatureBytes)
                || magic.StartsWith(Models.PKZIP.Constants.DataDescriptorSignatureBytes))
            {
                outFileType = FileType.ZipArchive;
            }

            return outFileType;
        }

        /// <summary>
        /// Get if the given path has a valid DAT extension
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <returns>True if the extension is valid, false otherwise</returns>
        private static bool HasValidArchiveExtension(string path)
        {
            // Get the extension from the path, if possible
            string? ext = path.GetNormalizedExtension();

            // Check against the list of known archive extensions
            return ext switch
            {
                // Aaruformat
                "aaru" => true,
                "aaruf" => true,
                "aaruformat" => true,
                "aif" => true,
                "dicf" => true,

                // Archive
                "7z" => true,
                "gz" => true,
                "lzma" => true,
                "rar" => true,
                "rev" => true,
                "r00" => true,
                "r01" => true,
                "tar" => true,
                "tgz" => true,
                "tlz" => true,
                "zip" => true,
                "zipx" => true,

                // CHD
                "chd" => true,

                _ => false,
            };
        }

        #endregion
    }
}