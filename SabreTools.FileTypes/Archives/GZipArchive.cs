using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Compress;
using Compress.gZip;
using Compress.Support.Compression.Deflate;
using SabreTools.Core.Tools;
using SabreTools.Hashing;
using SabreTools.IO.Extensions;

namespace SabreTools.FileTypes.Archives
{
    /// <summary>
    /// Represents a TorrentGZip archive for reading and writing
    /// </summary>
    public class GZipArchive : BaseArchive
    {
        #region Constants

        /* (Torrent)GZ Header Format
            https://tools.ietf.org/html/rfc1952

            00-01		Identification (0x1F, 0x8B) GzSignature
            02			Compression Method (0-7 reserved, 8 deflate; 0x08)
            03			Flags (0 FTEXT, 1 FHCRC, 2 FEXTRA, 3 FNAME, 4 FCOMMENT, 5 reserved, 6 reserved, 7 reserved; 0x04)
            04-07		Modification time (Unix format; 0x00, 0x00, 0x00, 0x00)
            08			Extra Flags (2 maximum compression, 4 fastest algorithm; 0x00)
            09			OS (See list on https://tools.ietf.org/html/rfc1952; 0x00)
            0A-0B		Length of extra field (mirrored; 0x1C, 0x00)
            0C-27		Extra field
                0C-1B	MD5 Hash
                1C-1F	CRC hash
                20-27	Int64 size (mirrored)
        */
        private readonly static byte[] TorrentGZHeader = [0x1f, 0x8b, 0x08, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1c, 0x00];

        #endregion

        #region Fields

        /// <summary>
        /// Positive value for depth of the output depot, defaults to 4
        /// </summary>
        public int Depth { get; set; } = 4;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new TorrentGZipArchive with no base file
        /// </summary>
        public GZipArchive()
            : base()
        {
            this.Type = FileType.GZipArchive;
        }

        /// <summary>
        /// Create a new TorrentGZipArchive from the given file
        /// </summary>
        /// <param name="filename">Name of the file to use as an archive</param>
        /// <param name="read">True for opening file as read, false for opening file as write</param>
        /// <param name="getHashes">True if hashes for this file should be calculated, false otherwise (default)</param>
        public GZipArchive(string filename, bool getHashes = false)
            : base(filename, getHashes)
        {
            this.Type = FileType.GZipArchive;
        }

        #endregion

        #region Extraction

        /// <inheritdoc/>
        public override bool CopyAll(string outDir)
        {
            bool encounteredErrors = true;

            // If we have an invalid file
            if (this.Filename == null)
                return true;

            try
            {
                // Create the temp directory
                Directory.CreateDirectory(outDir);

                // Decompress the _filename stream
                FileStream outstream = File.Create(Path.Combine(outDir, Path.GetFileNameWithoutExtension(this.Filename)));
                var gz = new gZip();
                ZipReturn ret = gz.ZipFileOpen(this.Filename);
                ret = gz.ZipFileOpenReadStream(0, out Stream? gzstream, out ulong streamSize);
                byte[] buffer = new byte[32768];
                int read;
                while ((read = gzstream!.Read(buffer, 0, buffer.Length)) > 0)
                {
                    outstream.Write(buffer, 0, read);
                }

                // Dispose of the streams
                outstream.Dispose();
                ret = gz.ZipFileCloseReadStream();
                gz.ZipFileClose();

                encounteredErrors = false;
            }
            catch (EndOfStreamException ex)
            {
                // Catch this but don't count it as an error because SharpCompress is unsafe
                logger.Verbose(ex);
            }
            catch (InvalidOperationException ex)
            {
                logger.Warning(ex);
                encounteredErrors = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                encounteredErrors = true;
            }

            return encounteredErrors;
        }

        /// <inheritdoc/>
        public override string? CopyToFile(string entryName, string outDir)
        {
            // Try to extract a stream using the given information
            (Stream? stream, string? realEntry) = GetEntryStream(entryName);
            if (stream == null || realEntry == null)
                return null;

            // If the stream and the entry name are both non-null, we write to file
            realEntry = Path.Combine(outDir, realEntry);

            // Create the output subfolder now
            string? dir = Path.GetDirectoryName(realEntry);
            if (dir != null)
                Directory.CreateDirectory(dir);

            // Now open and write the file if possible
            FileStream fs = File.Create(realEntry);
            if (fs != null)
            {
                if (stream.CanSeek)
                    stream.Seek(0, SeekOrigin.Begin);

                byte[] zbuffer = new byte[_bufferSize];
                int zlen;
                while ((zlen = stream.Read(zbuffer, 0, _bufferSize)) > 0)
                {
                    fs.Write(zbuffer, 0, zlen);
                    fs.Flush();
                }

                stream?.Dispose();
                fs?.Dispose();
            }
            else
            {
                stream?.Dispose();
                fs?.Dispose();
                realEntry = null;
            }

            return realEntry;
        }

        /// <inheritdoc/>
        public override (Stream?, string?) GetEntryStream(string entryName)
        {
            // If we have an invalid file
            if (this.Filename == null)
                return (null, null);

            try
            {
                // Open the entry stream
                string realEntry = Path.GetFileNameWithoutExtension(this.Filename);
                var gz = new gZip();
                ZipReturn ret = gz.ZipFileOpen(this.Filename);
                ret = gz.ZipFileOpenReadStream(0, out Stream? stream, out ulong streamSize);

                // Return the stream
                return (stream, realEntry);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return (null, null);
            }
        }

        #endregion

        #region Information

        /// <inheritdoc/>
        public override List<BaseFile>? GetChildren()
        {
            // If we have an invalid file
            if (this.Filename == null)
                return null;

            // If we have children cached already
            if (_children != null && _children.Count > 0)
                return _children;

            _children = [];

            string gamename = Path.GetFileNameWithoutExtension(this.Filename);

            BaseFile? possibleTgz = GetTorrentGZFileInfo();

            // If it was, then add it to the outputs and continue
            if (possibleTgz != null && possibleTgz.Filename != null)
            {
                _children.Add(possibleTgz);
                return _children;
            }

            try
            {
                // Create a blank item for the entry
                BaseFile gzipEntryRom = new();

                // Perform a quickscan, if flagged to
                if (this.AvailableHashTypes.Length == 1 && this.AvailableHashTypes[0] == HashType.CRC32)
                {
                    gzipEntryRom.Filename = gamename;

                    using BinaryReader br = new(File.OpenRead(this.Filename));
                    br.BaseStream.Seek(-8, SeekOrigin.End);
                    gzipEntryRom.CRC = br.ReadBytesBigEndian(4);
                    gzipEntryRom.Size = br.ReadInt32BigEndian();
                }
                // Otherwise, use the stream directly
                else
                {
                    var gz = new gZip();
                    ZipReturn ret = gz.ZipFileOpen(this.Filename);
                    ret = gz.ZipFileOpenReadStream(0, out Stream? gzstream, out ulong streamSize);
                    gzipEntryRom = GetInfo(gzstream, hashes: this.AvailableHashTypes);
                    gzipEntryRom.Filename = gz.GetLocalFile(0).Filename;
                    gzipEntryRom.Parent = gamename;
                    gzipEntryRom.Date = (gz.TimeStamp > 0 ? gz.TimeStamp.ToString() : null);
                    gzstream!.Dispose();
                }

                // Fill in common details and add to the list
                gzipEntryRom.Parent = gamename;
                _children.Add(gzipEntryRom);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }

            return _children;
        }

        /// <inheritdoc/>
        public override List<string> GetEmptyFolders()
        {
            // GZip files don't contain directories
            return [];
        }

        /// <inheritdoc/>
        public override bool IsTorrent()
        {
            // Check for the file existing first
            if (this.Filename == null || !File.Exists(this.Filename))
                return false;

            string datum = Path.GetFileName(this.Filename).ToLowerInvariant();
            long filesize = new FileInfo(this.Filename).Length;

            // If we have the romba depot files, just skip them gracefully
            if (datum == ".romba_size" || datum == ".romba_size.backup")
            {
                logger.Verbose($"Romba depot file found, skipping: {this.Filename}");
                return false;
            }

            // Check if the name is the right length
            if (!Regex.IsMatch(datum, @"^[0-9a-f]{" + Constants.SHA1Length + @"}\.gz"))
            {
                logger.Warning($"Non SHA-1 filename found, skipping: '{Path.GetFullPath(this.Filename)}'");
                return false;
            }

            // Check if the file is at least the minimum length
            if (filesize < 40 /* bytes */)
            {
                logger.Warning($"Possibly corrupt file '{Path.GetFullPath(this.Filename)}' with size {filesize}");
                return false;
            }

            // Get the Romba-specific header data
            BinaryReader br = new(File.OpenRead(this.Filename));
            byte[] header = br.ReadBytes(12); // Get preamble header for checking
            br.ReadBytes(16); // headermd5
            br.ReadBytes(4); // headercrc
            br.ReadUInt64(); // headersz
#if NET40_OR_GREATER
            br.Dispose();
#endif

            // If the header is not correct, return a blank rom
            bool correct = true;
            for (int i = 0; i < header.Length; i++)
            {
                // This is a temp fix to ignore the modification time and OS until romba can be fixed
                if (i == 4 || i == 5 || i == 6 || i == 7 || i == 9)
                    continue;

                correct &= (header[i] == TorrentGZHeader[i]);
            }

            if (!correct)
                return false;

            return true;
        }

        /// <summary>
        /// Retrieve file information for a single torrent GZ file
        /// </summary>
        /// <returns>Populated DatItem object if success, empty one on error</returns>
        public BaseFile? GetTorrentGZFileInfo()
        {
            // Check for the file existing first
            if (this.Filename == null || !File.Exists(this.Filename))
                return null;

            string datum = Path.GetFileName(this.Filename).ToLowerInvariant();
            long filesize = new FileInfo(this.Filename).Length;

            // If we have the romba depot files, just skip them gracefully
            if (datum == ".romba_size" || datum == ".romba_size.backup")
            {
                logger.Verbose($"Romba depot file found, skipping: {this.Filename}");
                return null;
            }

            // Check if the name is the right length
            if (!Regex.IsMatch(datum, @"^[0-9a-f]{" + Constants.SHA1Length + @"}\.gz"))
            {
                logger.Warning($"Non SHA-1 filename found, skipping: '{Path.GetFullPath(this.Filename)}'");
                return null;
            }

            // Check if the file is at least the minimum length
            if (filesize < 40 /* bytes */)
            {
                logger.Warning($"Possibly corrupt file '{Path.GetFullPath(this.Filename)}' with size {filesize}");
                return null;
            }

            // Get the Romba-specific header data
            byte[] header; // Get preamble header for checking
            byte[] headermd5; // MD5
            byte[] headercrc; // CRC
            ulong headersz; // Int64 size
            BinaryReader br = new(File.OpenRead(this.Filename));
            header = br.ReadBytes(12);
            headermd5 = br.ReadBytes(16);
            headercrc = br.ReadBytes(4);
            headersz = br.ReadUInt64();
#if NET40_OR_GREATER
            br.Dispose();
#endif

            // If the header is not correct, return a blank rom
            bool correct = true;
            for (int i = 0; i < header.Length; i++)
            {
                // This is a temp fix to ignore the modification time and OS until romba can be fixed
                if (i == 4 || i == 5 || i == 6 || i == 7 || i == 9)
                {
                    continue;
                }
                correct &= (header[i] == TorrentGZHeader[i]);
            }

            if (!correct)
                return null;

            // Now convert the data and get the right position
            long extractedsize = (long)headersz;

            BaseFile baseFile = new()
            {
                Filename = Path.GetFileNameWithoutExtension(this.Filename).ToLowerInvariant(),
                Size = extractedsize,
                CRC = headercrc,
                MD5 = headermd5,
                SHA1 = TextHelper.StringToByteArray(Path.GetFileNameWithoutExtension(this.Filename)),

                Parent = Path.GetFileNameWithoutExtension(this.Filename).ToLowerInvariant(),
            };

            return baseFile;
        }

        #endregion

        #region Writing

        /// <inheritdoc/>
        public override bool Write(string inputFile, string outDir, BaseFile? baseFile = null)
        {
            // Check that the input file exists
            if (!File.Exists(inputFile))
            {
                logger.Warning($"File '{inputFile}' does not exist!");
                return false;
            }

            inputFile = Path.GetFullPath(inputFile);

            // Get the file stream for the file and write out
            return Write(File.OpenRead(inputFile), outDir, baseFile);
        }

        /// <inheritdoc/>
        public override bool Write(Stream? inputStream, string outDir, BaseFile? baseFile = null)
        {
            bool success = false;

            // If the stream is not readable, return
            if (inputStream == null || !inputStream.CanRead)
                return success;

            // Make sure the output directory exists
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);

            outDir = Path.GetFullPath(outDir);

            // Now get the Rom info for the file so we have hashes and size
            baseFile = GetInfo(inputStream, keepReadOpen: true);

            // Get the output file name
            string outfile = Path.Combine(outDir, Utilities.GetDepotPath(TextHelper.ByteArrayToString(baseFile.SHA1), Depth) ?? string.Empty);

            // Check to see if the folder needs to be created
            if (!Directory.Exists(Path.GetDirectoryName(outfile)))
                Directory.CreateDirectory(Path.GetDirectoryName(outfile)!);

            // If the output file exists, don't try to write again
            if (!File.Exists(outfile))
            {
                // Compress the input stream
                FileStream outputStream = File.Create(outfile);

                // Open the output file for writing
                BinaryWriter sw = new(outputStream);

                // Write standard header and TGZ info
                byte[] data = TorrentGZHeader
                            .Concat(baseFile.MD5!) // MD5
                            .Concat(baseFile.CRC!) // CRC
                            .ToArray();
                sw.Write(data);
                sw.Write((ulong)(baseFile.Size ?? 0)); // Long size (Unsigned, Mirrored)

                // Now create a deflatestream from the input file
                ZlibBaseStream ds = new(outputStream, CompressionMode.Compress, CompressionLevel.BestCompression, ZlibStreamFlavor.DEFLATE, true);

                // Copy the input stream to the output
                byte[] ibuffer = new byte[_bufferSize];
                int ilen;
                while ((ilen = inputStream.Read(ibuffer, 0, _bufferSize)) > 0)
                {
                    ds.Write(ibuffer, 0, ilen);
                    ds.Flush();
                }

                ds.Dispose();

                // Now write the standard footer
                sw.Write(baseFile.CRC!.Reverse().ToArray());
                sw.Write((uint)(baseFile.Size ?? 0));

                // Dispose of everything
#if NET40_OR_GREATER
                sw.Dispose();
#endif
                outputStream.Dispose();
            }

            return true;
        }

        /// <inheritdoc/>
        public override bool Write(List<string> inputFiles, string outDir, List<BaseFile>? baseFile)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
