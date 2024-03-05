﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
#if NET40_OR_GREATER || NETCOREAPP
using System.Threading.Tasks;
#endif
using Compress.Support.Compression.LZMA;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.FileTypes.Aaru;
using SabreTools.FileTypes.CHD;
using SabreTools.Hashing;
using SabreTools.IO;
using SabreTools.Logging;
using SabreTools.Matching;
using SabreTools.Skippers;

namespace SabreTools.FileTypes
{
    public class BaseFile
    {
        #region Constants

        protected static readonly byte[] SevenZipSignature = { 0x37, 0x7a, 0xbc, 0xaf, 0x27, 0x1c };
        protected static readonly byte[] AaruFormatSignature = { 0x41, 0x41, 0x52, 0x55, 0x46, 0x52, 0x4d, 0x54 };
        protected static readonly byte[] BZ2Signature = { 0x42, 0x5a, 0x68 };
        protected static readonly byte[] CabinetSignature = { 0x4d, 0x53, 0x43, 0x46 };
        protected static readonly byte[] CHDSignature = { 0x4d, 0x43, 0x6f, 0x6d, 0x70, 0x72, 0x48, 0x44 };
        protected static readonly byte[] ELFSignature = { 0x7f, 0x45, 0x4c, 0x46 };
        protected static readonly byte[] FreeArcSignature = { 0x41, 0x72, 0x43, 0x01 };
        protected static readonly byte[] GzSignature = { 0x1f, 0x8b, 0x08 };
        protected static readonly byte[] LRZipSignature = { 0x4c, 0x52, 0x5a, 0x49 };
        protected static readonly byte[] LZ4Signature = { 0x18, 0x4d, 0x22, 0x04 };
        protected static readonly byte[] LZ4SkippableMinSignature = { 0x18, 0x4d, 0x22, 0x04 };
        protected static readonly byte[] LZ4SkippableMaxSignature = { 0x18, 0x4d, 0x2a, 0x5f };
        protected static readonly byte[] PESignature = { 0x4d, 0x5a };
        protected static readonly byte[] RarSignature = { 0x52, 0x61, 0x72, 0x21, 0x1a, 0x07, 0x00 };
        protected static readonly byte[] RarFiveSignature = { 0x52, 0x61, 0x72, 0x21, 0x1a, 0x07, 0x01, 0x00 };
        protected static readonly byte[] TarSignature = { 0x75, 0x73, 0x74, 0x61, 0x72, 0x20, 0x20, 0x00 };
        protected static readonly byte[] TarZeroSignature = { 0x75, 0x73, 0x74, 0x61, 0x72, 0x00, 0x30, 0x30 };
        protected static readonly byte[] XZSignature = { 0xfd, 0x37, 0x7a, 0x58, 0x5a, 0x00, 0x00 };
        protected static readonly byte[] ZipSignature = { 0x50, 0x4b, 0x03, 0x04 };
        protected static readonly byte[] ZipSignatureEmpty = { 0x50, 0x4b, 0x05, 0x06 };
        protected static readonly byte[] ZipSignatureSpanned = { 0x50, 0x4b, 0x07, 0x08 };
        protected static readonly byte[] ZPAQSignature = { 0x7a, 0x50, 0x51 };
        protected static readonly byte[] ZstdSignature = { 0xfd, 0x2f, 0xb5 };

        #endregion

        // TODO: Get all of these values automatically so there is no public "set"
        #region Fields

        /// <summary>
        /// Internal type of the represented file
        /// </summary>
        public FileType Type { get; protected set; }

        /// <summary>
        /// Filename or path to the file
        /// </summary>
        public string? Filename { get; set; }

        /// <summary>
        /// Direct parent of the file
        /// </summary>
        public string? Parent { get; set; }

        /// <summary>
        /// Date stamp of the file
        /// </summary>
        public string? Date { get; set; }

        /// <summary>
        /// Optional size of the file
        /// </summary>
        public long? Size { get; set; }

        /// <summary>
        /// Hashes that are available for the file
        /// </summary>
        public HashType[] AvailableHashTypes { get; set; } = [HashType.CRC32, HashType.MD5, HashType.SHA1];

        /// <summary>
        /// CRC32 hash of the file
        /// </summary>
        public byte[]? CRC { get; set; } = null;

        /// <summary>
        /// MD5 hash of the file
        /// </summary>
        public byte[]? MD5 { get; set; } = null;

        /// <summary>
        /// SHA-1 hash of the file
        /// </summary>
        public byte[]? SHA1 { get; set; } = null;

        /// <summary>
        /// SHA-256 hash of the file
        /// </summary>
        public byte[]? SHA256 { get; set; } = null;

        /// <summary>
        /// SHA-384 hash of the file
        /// </summary>
        public byte[]? SHA384 { get; set; } = null;

        /// <summary>
        /// SHA-512 hash of the file
        /// </summary>
        public byte[]? SHA512 { get; set; } = null;

        /// <summary>
        /// SpamSum fuzzy hash of the file
        /// </summary>
        public byte[]? SpamSum { get; set; } = null;

        #endregion

        #region Construtors

        /// <summary>
        /// Create a new BaseFile with no base file
        /// </summary>
        public BaseFile()
        {
        }

        /// <summary>
        /// Create a new BaseFile from the given file
        /// </summary>
        /// <param name="filename">Name of the file to use</param>
        /// <param name="getHashes">True if hashes for this file should be calculated (default), false otherwise</param>
        public BaseFile(string filename, bool getHashes = true)
        {
            this.Filename = filename;

            if (getHashes)
            {
                BaseFile? temp = GetInfo(this.Filename, hashes: this.AvailableHashTypes);
                if (temp != null)
                {
                    this.Parent = temp.Parent;
                    this.Date = temp.Date;
                    this.CRC = temp.CRC;
                    this.MD5 = temp.MD5;
                    this.SHA1 = temp.SHA1;
                    this.SHA256 = temp.SHA256;
                    this.SHA384 = temp.SHA384;
                    this.SHA512 = temp.SHA512;
                    this.SpamSum = temp.SpamSum;
                }
            }
        }

        /// <summary>
        /// Create a new BaseFile from the given file
        /// </summary>
        /// <param name="filename">Name of the file to use</param>
        /// <param name="stream">Stream to populate information from</param>
        /// <param name="getHashes">True if hashes for this file should be calculated (default), false otherwise</param>
        public BaseFile(string filename, Stream stream, bool getHashes = true)
        {
            this.Filename = filename;

            if (getHashes)
            {
                BaseFile temp = GetInfo(stream, hashes: this.AvailableHashTypes);
                if (temp != null)
                {
                    this.Parent = temp.Parent;
                    this.Date = temp.Date;
                    this.CRC = temp.CRC;
                    this.MD5 = temp.MD5;
                    this.SHA1 = temp.SHA1;
                    this.SHA256 = temp.SHA256;
                    this.SHA384 = temp.SHA384;
                    this.SHA512 = temp.SHA512;
                    this.SpamSum = temp.SpamSum;
                }
            }

        }

        #endregion

        #region Static Methods

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
            else if (magic.StartsWith(LRZipSignature))
            {
                outFileType = FileType.LRZipArchive;
            }
            else if (magic.StartsWith(LZ4Signature)
                || magic.StartsWith(LZ4SkippableMinSignature)
                || magic.StartsWith(LZ4SkippableMaxSignature))
            {
                outFileType = FileType.LZ4Archive;
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
            else if (magic.StartsWith(ZipSignature)
                || magic.StartsWith(ZipSignatureEmpty)
                || magic.StartsWith(ZipSignatureSpanned))
            {
                outFileType = FileType.ZipArchive;
            }
            else if (magic.StartsWith(ZPAQSignature))
            {
                outFileType = FileType.ZPAQArchive;
            }
            else if (magic.StartsWith(ZstdSignature))
            {
                outFileType = FileType.ZstdArchive;
            }

            return outFileType;
        }

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
#if NETFRAMEWORK
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
                CRC = hashes.Contains(HashType.CRC32) ? TextHelper.StringToByteArray(hashDict[HashType.CRC32]) : null,
                MD5 = hashes.Contains(HashType.MD5) ? TextHelper.StringToByteArray(hashDict[HashType.MD5]) : null,
                SHA1 = hashes.Contains(HashType.SHA1) ? TextHelper.StringToByteArray(hashDict[HashType.SHA1]) : null,
                SHA256 = hashes.Contains(HashType.SHA256) ? TextHelper.StringToByteArray(hashDict[HashType.SHA256]) : null,
                SHA384 = hashes.Contains(HashType.SHA384) ? TextHelper.StringToByteArray(hashDict[HashType.SHA384]) : null,
                SHA512 = hashes.Contains(HashType.SHA512) ? TextHelper.StringToByteArray(hashDict[HashType.SHA512]) : null,
                SpamSum = hashes.Contains(HashType.SpamSum) ? TextHelper.StringToByteArray(hashDict[HashType.SpamSum]) : null,
            };

            // Deal with the input stream
            if (!keepReadOpen)
                input.Dispose();
            else
                input.SeekIfPossible();

            return baseFile;
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
