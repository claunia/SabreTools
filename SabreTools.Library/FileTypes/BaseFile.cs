using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using SabreTools.Core;
using SabreTools.IO;
using SabreTools.Library.IO;
using SabreTools.Logging;
using Compress.ThreadReaders;

namespace SabreTools.Library.FileTypes
{
    public class BaseFile
    {
        // TODO: Get all of these values automatically so there is no public "set"
        #region Fields

        /// <summary>
        /// Internal type of the represented file
        /// </summary>
        public FileType Type { get; protected set; }

        /// <summary>
        /// Filename or path to the file
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// Direct parent of the file
        /// </summary>
        public string Parent { get; set; }

        /// <summary>
        /// Date stamp of the file
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// Optional size of the file
        /// </summary>
        public long? Size { get; set; }

        /// <summary>
        /// Hashes that are available for the file
        /// </summary>
        public Hash AvailableHashes { get; set; } = Hash.Standard;

        /// <summary>
        /// CRC32 hash of the file
        /// </summary>
        public byte[] CRC { get; set; } = null;

        /// <summary>
        /// MD5 hash of the file
        /// </summary>
        public byte[] MD5 { get; set; } = null;

#if NET_FRAMEWORK
        /// <summary>
        /// RIPEMD160 hash of the file
        /// </summary>
        public byte[] RIPEMD160 { get; set; } = null;
#endif

        /// <summary>
        /// SHA-1 hash of the file
        /// </summary>
        public byte[] SHA1 { get; set; } = null;

        /// <summary>
        /// SHA-256 hash of the file
        /// </summary>
        public byte[] SHA256 { get; set; } = null;

        /// <summary>
        /// SHA-384 hash of the file
        /// </summary>
        public byte[] SHA384 { get; set; } = null;

        /// <summary>
        /// SHA-512 hash of the file
        /// </summary>
        public byte[] SHA512 { get; set; } = null;

        /// <summary>
        /// SpamSum fuzzy hash of the file
        /// </summary>
        public byte[] SpamSum { get; set; } = null;

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
                BaseFile temp = GetInfo(this.Filename, hashes: this.AvailableHashes);
                if (temp != null)
                {
                    this.Parent = temp.Parent;
                    this.Date = temp.Date;
                    this.CRC = temp.CRC;
                    this.MD5 = temp.MD5;
#if NET_FRAMEWORK
                    this.RIPEMD160 = temp.RIPEMD160;
#endif
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
                BaseFile temp = GetInfo(stream, hashes: this.AvailableHashes);
                if (temp != null)
                {
                    this.Parent = temp.Parent;
                    this.Date = temp.Date;
                    this.CRC = temp.CRC;
                    this.MD5 = temp.MD5;
#if NET_FRAMEWORK
                    this.RIPEMD160 = temp.RIPEMD160;
#endif
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
            if (!PathExtensions.HasValidArchiveExtension(input))
                return outFileType;

            // Read the first bytes of the file and get the magic number
            BinaryReader br = new BinaryReader(File.OpenRead(input));
            byte[] magic = br.ReadBytes(8);
            br.Dispose();

            // Now try to match it to a known signature
            if (magic.StartsWith(Constants.SevenZipSignature))
            {
                outFileType = FileType.SevenZipArchive;
            }
            else if (magic.StartsWith(Constants.AaruFormatSignature))
            {
                outFileType = FileType.AaruFormat;
            }
            else if (magic.StartsWith(Constants.CHDSignature))
            {
                outFileType = FileType.CHD;
            }
            else if (magic.StartsWith(Constants.GzSignature))
            {
                outFileType = FileType.GZipArchive;
            }
            else if (magic.StartsWith(Constants.LRZipSignature))
            {
                outFileType = FileType.LRZipArchive;
            }
            else if (magic.StartsWith(Constants.LZ4Signature)
                || magic.StartsWith(Constants.LZ4SkippableMinSignature)
                || magic.StartsWith(Constants.LZ4SkippableMaxSignature))
            {
                outFileType = FileType.LZ4Archive;
            }
            else if (magic.StartsWith(Constants.RarSignature)
                || magic.StartsWith(Constants.RarFiveSignature))
            {
                outFileType = FileType.RarArchive;
            }
            else if (magic.StartsWith(Constants.TarSignature)
                || magic.StartsWith(Constants.TarZeroSignature))
            {
                outFileType = FileType.TapeArchive;
            }
            else if (magic.StartsWith(Constants.XZSignature))
            {
                outFileType = FileType.XZArchive;
            }
            else if (magic.StartsWith(Constants.ZipSignature)
                || magic.StartsWith(Constants.ZipSignatureEmpty)
                || magic.StartsWith(Constants.ZipSignatureSpanned))
            {
                outFileType = FileType.ZipArchive;
            }
            else if (magic.StartsWith(Constants.ZPAQSignature))
            {
                outFileType = FileType.ZPAQArchive;
            }
            else if (magic.StartsWith(Constants.ZstdSignature))
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
        public static BaseFile GetInfo(string input, string header = null, Hash hashes = Hash.Standard, TreatAsFile asFiles = 0x00)
        {
            // Add safeguard if file doesn't exist
            if (!File.Exists(input))
                return null;

            // Get input information
            var fileType = GetFileType(input);
            Stream inputStream = File.OpenRead(input);

            // Try to match the supplied header skipper
            if (header != null)
            {
                var rule = Transform.GetMatchingRule(input, Path.GetFileNameWithoutExtension(header));

                // If there's a match, transform the stream before getting info
                if (rule.Tests != null && rule.Tests.Count != 0)
                {
                    // Create the output stream
                    MemoryStream outputStream = new MemoryStream();

                    // Transform the stream and get the information from it
                    rule.TransformStream(inputStream, outputStream, keepReadOpen: false, keepWriteOpen: true);
                    inputStream = outputStream;
                }
            }

            // Get the info in the proper manner
            BaseFile baseFile;
            if (fileType == FileType.AaruFormat && !asFiles.HasFlag(TreatAsFile.AaruFormat))
                baseFile = AaruFormat.Create(inputStream);
            else if (fileType == FileType.CHD && !asFiles.HasFlag(TreatAsFile.CHD))
                baseFile = CHDFile.Create(inputStream);
            else
                baseFile = GetInfo(inputStream, hashes: hashes, keepReadOpen: false);

            // Dispose of the input stream
            inputStream?.Dispose();

            // Add unique data from the file
            baseFile.Filename = Path.GetFileName(input);
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
        public static BaseFile GetInfo(Stream input, long size = -1, Hash hashes = Hash.Standard, bool keepReadOpen = false)
        {
            // If we want to automatically set the size
            if (size == -1)
                size = input.Length;

            try
            {
                // Get a list of hashers to run over the buffer
                List<Hasher> hashers = new List<Hasher>();

                if (hashes.HasFlag(Hash.CRC))
                    hashers.Add(new Hasher(Hash.CRC));
                if (hashes.HasFlag(Hash.MD5))
                    hashers.Add(new Hasher(Hash.MD5));
#if NET_FRAMEWORK
                if (hashes.HasFlag(Hash.RIPEMD160))
                    hashers.Add(new Hasher(Hash.RIPEMD160));
#endif
                if (hashes.HasFlag(Hash.SHA1))
                    hashers.Add(new Hasher(Hash.SHA1));
                if (hashes.HasFlag(Hash.SHA256))
                    hashers.Add(new Hasher(Hash.SHA256));
                if (hashes.HasFlag(Hash.SHA384))
                    hashers.Add(new Hasher(Hash.SHA384));
                if (hashes.HasFlag(Hash.SHA512))
                    hashers.Add(new Hasher(Hash.SHA512));
                if (hashes.HasFlag(Hash.SpamSum))
                    hashers.Add(new Hasher(Hash.SpamSum));

                // Initialize the hashing helpers
                var loadBuffer = new ThreadLoadBuffer(input);
                int buffersize = 3 * 1024 * 1024;
                byte[] buffer0 = new byte[buffersize];
                byte[] buffer1 = new byte[buffersize];

                /*
                Please note that some of the following code is adapted from
                RomVault. This is a modified version of how RomVault does
                threaded hashing. As such, some of the terminology and code
                is the same, though variable names and comments may have
                been tweaked to better fit this code base.
                */

                // Pre load the first buffer
                long refsize = size;
                int next = refsize > buffersize ? buffersize : (int)refsize;
                input.Read(buffer0, 0, next);
                int current = next;
                refsize -= next;
                bool bufferSelect = true;

                while (current > 0)
                {
                    // Trigger the buffer load on the second buffer
                    next = refsize > buffersize ? buffersize : (int)refsize;
                    if (next > 0)
                        loadBuffer.Trigger(bufferSelect ? buffer1 : buffer0, next);

                    byte[] buffer = bufferSelect ? buffer0 : buffer1;

                    // Run hashes in parallel
                    Parallel.ForEach(hashers, Globals.ParallelOptions, h => h.Process(buffer, current));

                    // Wait for the load buffer worker, if needed
                    if (next > 0)
                        loadBuffer.Wait();

                    // Setup for the next hashing step
                    current = next;
                    refsize -= next;
                    bufferSelect = !bufferSelect;
                }

                // Finalize all hashing helpers
                loadBuffer.Finish();
                Parallel.ForEach(hashers, Globals.ParallelOptions, h => h.Finalize());

                // Get the results
                BaseFile baseFile = new BaseFile()
                {
                    Size = size,
                    CRC = hashes.HasFlag(Hash.CRC) ? hashers.First(h => h.HashType == Hash.CRC).GetHash() : null,
                    MD5 = hashes.HasFlag(Hash.MD5) ? hashers.First(h => h.HashType == Hash.MD5).GetHash() : null,
#if NET_FRAMEWORK
                    RIPEMD160 = hashes.HasFlag(Hash.RIPEMD160) ? hashers.First(h => h.HashType == Hash.RIPEMD160).GetHash() : null,
#endif
                    SHA1 = hashes.HasFlag(Hash.SHA1) ? hashers.First(h => h.HashType == Hash.SHA1).GetHash() : null,
                    SHA256 = hashes.HasFlag(Hash.SHA256) ? hashers.First(h => h.HashType == Hash.SHA256).GetHash() : null,
                    SHA384 = hashes.HasFlag(Hash.SHA384) ? hashers.First(h => h.HashType == Hash.SHA384).GetHash() : null,
                    SHA512 = hashes.HasFlag(Hash.SHA512) ? hashers.First(h => h.HashType == Hash.SHA512).GetHash() : null,
                    SpamSum = hashes.HasFlag(Hash.SpamSum) ? hashers.First(h => h.HashType == Hash.SpamSum).GetHash() : null,
                };

                // Dispose of the hashers
                loadBuffer.Dispose();
                hashers.ForEach(h => h.Dispose());

                return baseFile;
            }
            catch (IOException ex)
            {
                LoggerImpl.Warning(ex, "An exception occurred during hashing.");
                return new BaseFile();
            }
            finally
            {
                if (!keepReadOpen)
                    input.Dispose();
                else
                    input.SeekIfPossible();
            }
        }

        #endregion
    }
}
