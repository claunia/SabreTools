using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using SabreTools.Data;
using SabreTools.Library.DatFiles;
using SabreTools.Library.DatItems;
using SabreTools.Library.IO;
using SabreTools.Library.Tools;
using Compress;
using Compress.gZip;
using Compress.ZipFile.ZLib;

namespace SabreTools.Library.FileTypes
{
    /// <summary>
    /// Represents a TorrentGZip archive for reading and writing
    /// </summary>
    public class GZipArchive : BaseArchive
    {
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

        /// <summary>
        /// Attempt to extract a file as an archive
        /// </summary>
        /// <param name="outDir">Output directory for archive extraction</param>
        /// <returns>True if the extraction was a success, false otherwise</returns>
        public override bool CopyAll(string outDir)
        {
            bool encounteredErrors = true;

            try
            {
                // Create the temp directory
                Directory.CreateDirectory(outDir);

                // Decompress the _filename stream
                FileStream outstream = FileExtensions.TryCreate(Path.Combine(outDir, Path.GetFileNameWithoutExtension(this.Filename)));
                var gz = new gZip();
                ZipReturn ret = gz.ZipFileOpen(this.Filename);
                ret = gz.ZipFileOpenReadStream(0, out Stream gzstream, out ulong streamSize);
                gzstream.CopyTo(outstream);

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

        /// <summary>
        /// Attempt to extract a file from an archive
        /// </summary>
        /// <param name="entryName">Name of the entry to be extracted</param>
        /// <param name="outDir">Output directory for archive extraction</param>
        /// <returns>Name of the extracted file, null on error</returns>
        public override string CopyToFile(string entryName, string outDir)
        {
            // Try to extract a stream using the given information
            (MemoryStream ms, string realEntry) = CopyToStream(entryName);

            // If the memory stream and the entry name are both non-null, we write to file
            if (ms != null && realEntry != null)
            {
                realEntry = Path.Combine(outDir, realEntry);

                // Create the output subfolder now
                Directory.CreateDirectory(Path.GetDirectoryName(realEntry));

                // Now open and write the file if possible
                FileStream fs = FileExtensions.TryCreate(realEntry);
                if (fs != null)
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    byte[] zbuffer = new byte[_bufferSize];
                    int zlen;
                    while ((zlen = ms.Read(zbuffer, 0, _bufferSize)) > 0)
                    {
                        fs.Write(zbuffer, 0, zlen);
                        fs.Flush();
                    }

                    ms?.Dispose();
                    fs?.Dispose();
                }
                else
                {
                    ms?.Dispose();
                    fs?.Dispose();
                    realEntry = null;
                }
            }

            return realEntry;
        }

        /// <summary>
        /// Attempt to extract a stream from an archive
        /// </summary>
        /// <param name="entryName">Name of the entry to be extracted</param>
        /// <param name="realEntry">Output representing the entry name that was found</param>
        /// <returns>MemoryStream representing the entry, null on error</returns>
        public override (MemoryStream, string) CopyToStream(string entryName)
        {
            MemoryStream ms = new MemoryStream();
            string realEntry;

            try
            {
                // Decompress the _filename stream
                realEntry = Path.GetFileNameWithoutExtension(this.Filename);
                var gz = new gZip();
                ZipReturn ret = gz.ZipFileOpen(this.Filename);
                ret = gz.ZipFileOpenReadStream(0, out Stream gzstream, out ulong streamSize);

                // Write the file out
                byte[] gbuffer = new byte[_bufferSize];
                int glen;
                while ((glen = gzstream.Read(gbuffer, 0, _bufferSize)) > 0)
                {

                    ms.Write(gbuffer, 0, glen);
                    ms.Flush();
                }

                // Dispose of the streams
                gzstream.Dispose();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                ms = null;
                realEntry = null;
            }

            return (ms, realEntry);
        }

        #endregion

        #region Information

        /// <summary>
        /// Generate a list of DatItem objects from the header values in an archive
        /// </summary>
        /// <returns>List of DatItem objects representing the found data</returns>
        public override List<BaseFile> GetChildren()
        {
            if (_children == null || _children.Count == 0)
            {
                _children = new List<BaseFile>();

                string gamename = Path.GetFileNameWithoutExtension(this.Filename);

                BaseFile possibleTgz = GetTorrentGZFileInfo();

                // If it was, then add it to the outputs and continue
                if (possibleTgz != null && possibleTgz.Filename != null)
                {
                    _children.Add(possibleTgz);
                }
                else
                {
                    try
                    {
                        // Create a blank item for the entry
                        BaseFile gzipEntryRom = new BaseFile();

                        // Perform a quickscan, if flagged to
                        if (this.AvailableHashes == Hash.CRC)
                        {
                            gzipEntryRom.Filename = gamename;
                            using (BinaryReader br = new BinaryReader(FileExtensions.TryOpenRead(this.Filename)))
                            {
                                br.BaseStream.Seek(-8, SeekOrigin.End);
                                gzipEntryRom.CRC = br.ReadBytesBigEndian(4);
                                gzipEntryRom.Size = br.ReadInt32BigEndian();
                            }
                        }
                        // Otherwise, use the stream directly
                        else
                        {
                            var gz = new gZip();
                            ZipReturn ret = gz.ZipFileOpen(this.Filename);
                            ret = gz.ZipFileOpenReadStream(0, out Stream gzstream, out ulong streamSize);
                            gzipEntryRom = gzstream.GetInfo(hashes: this.AvailableHashes);
                            gzipEntryRom.Filename = gz.Filename(0);
                            gzipEntryRom.Parent = gamename;
                            gzipEntryRom.Date = (gz.TimeStamp > 0 ? gz.TimeStamp.ToString() : null);
                            gzstream.Dispose();
                        }

                        // Fill in comon details and add to the list
                        gzipEntryRom.Parent = gamename;
                        _children.Add(gzipEntryRom);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                        return null;
                    }
                }
            }

            return _children;
        }

        /// <summary>
        /// Generate a list of empty folders in an archive
        /// </summary>
        /// <param name="input">Input file to get data from</param>
        /// <returns>List of empty folders in the archive</returns>
        public override List<string> GetEmptyFolders()
        {
            // GZip files don't contain directories
            return new List<string>();
        }

        /// <summary>
        /// Check whether the input file is a standardized format
        /// </summary>
        public override bool IsTorrent()
        {
            // Check for the file existing first
            if (!File.Exists(this.Filename))
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
                logger.Warning($"Possibly corrupt file '{Path.GetFullPath(this.Filename)}' with size {Utilities.GetBytesReadable(filesize)}");
                return false;
            }

            // Get the Romba-specific header data
            BinaryReader br = new BinaryReader(FileExtensions.TryOpenRead(this.Filename));
            byte[] header = br.ReadBytes(12); // Get preamble header for checking
            br.ReadBytes(16); // headermd5
            br.ReadBytes(4); // headercrc
            br.ReadUInt64(); // headersz
            br.Dispose();

            // If the header is not correct, return a blank rom
            bool correct = true;
            for (int i = 0; i < header.Length; i++)
            {
                // This is a temp fix to ignore the modification time and OS until romba can be fixed
                if (i == 4 || i == 5 || i == 6 || i == 7 || i == 9)
                    continue;

                correct &= (header[i] == Constants.TorrentGZHeader[i]);
            }

            if (!correct)
                return false;

            return true;
        }

        /// <summary>
        /// Retrieve file information for a single torrent GZ file
        /// </summary>
        /// <returns>Populated DatItem object if success, empty one on error</returns>
        public BaseFile GetTorrentGZFileInfo()
        {
            // Check for the file existing first
            if (!File.Exists(this.Filename))
            {
                return null;
            }

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
                logger.Warning($"Possibly corrupt file '{Path.GetFullPath(this.Filename)}' with size {Utilities.GetBytesReadable(filesize)}");
                return null;
            }

            // Get the Romba-specific header data
            byte[] header; // Get preamble header for checking
            byte[] headermd5; // MD5
            byte[] headercrc; // CRC
            ulong headersz; // Int64 size
            BinaryReader br = new BinaryReader(FileExtensions.TryOpenRead(this.Filename));
            header = br.ReadBytes(12);
            headermd5 = br.ReadBytes(16);
            headercrc = br.ReadBytes(4);
            headersz = br.ReadUInt64();
            br.Dispose();

            // If the header is not correct, return a blank rom
            bool correct = true;
            for (int i = 0; i < header.Length; i++)
            {
                // This is a temp fix to ignore the modification time and OS until romba can be fixed
                if (i == 4 || i == 5 || i == 6 || i == 7 || i == 9)
                {
                    continue;
                }
                correct &= (header[i] == Constants.TorrentGZHeader[i]);
            }
            if (!correct)
            {
                return null;
            }

            // Now convert the data and get the right position
            long extractedsize = (long)headersz;

            BaseFile baseFile = new BaseFile
            {
                Filename = Path.GetFileNameWithoutExtension(this.Filename).ToLowerInvariant(),
                Size = extractedsize,
                CRC = headercrc,
                MD5 = headermd5,
                SHA1 = Utilities.StringToByteArray(Path.GetFileNameWithoutExtension(this.Filename)),

                Parent = Path.GetFileNameWithoutExtension(this.Filename).ToLowerInvariant(),
            };

            return baseFile;
        }

        #endregion

        #region Writing

        /// <summary>
        /// Write an input file to a torrent GZ file
        /// </summary>
        /// <param name="inputFile">Input filename to be moved</param>
        /// <param name="outDir">Output directory to build to</param>
        /// <param name="rom">DatItem representing the new information</param>
        /// <returns>True if the write was a success, false otherwise</returns>
        /// <remarks>This works for now, but it can be sped up by using Ionic.Zip or another zlib wrapper that allows for header values built-in. See edc's code.</remarks>
        public override bool Write(string inputFile, string outDir, Rom rom = null)
        {
            // Check that the input file exists
            if (!File.Exists(inputFile))
            {
                logger.Warning($"File '{inputFile}' does not exist!");
                return false;
            }

            inputFile = Path.GetFullPath(inputFile);

            // Get the file stream for the file and write out
            return Write(FileExtensions.TryOpenRead(inputFile), outDir, rom);
        }

        /// <summary>
        /// Write an input stream to a torrent GZ file
        /// </summary>
        /// <param name="inputStream">Input stream to be moved</param>
        /// <param name="outDir">Output directory to build to</param>
        /// <param name="rom">DatItem representing the new information</param>
        /// <returns>True if the write was a success, false otherwise</returns>
        /// <remarks>This works for now, but it can be sped up by using Ionic.Zip or another zlib wrapper that allows for header values built-in. See edc's code.</remarks>
        public override bool Write(Stream inputStream, string outDir, Rom rom = null)
        {
            bool success = false;

            // If the stream is not readable, return
            if (!inputStream.CanRead)
                return success;

            // Make sure the output directory exists
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);

            outDir = Path.GetFullPath(outDir);

            // Now get the Rom info for the file so we have hashes and size
            rom = new Rom(inputStream.GetInfo(keepReadOpen: true));

            // Get the output file name
            string outfile = Path.Combine(outDir, PathExtensions.GetDepotPath(rom.SHA1, Depth));

            // Check to see if the folder needs to be created
            if (!Directory.Exists(Path.GetDirectoryName(outfile)))
                Directory.CreateDirectory(Path.GetDirectoryName(outfile));

            // If the output file exists, don't try to write again
            if (!File.Exists(outfile))
            {
                // Compress the input stream
                FileStream outputStream = FileExtensions.TryCreate(outfile);

                // Open the output file for writing
                BinaryWriter sw = new BinaryWriter(outputStream);

                // Write standard header and TGZ info
                byte[] data = Constants.TorrentGZHeader
                                .Concat(Utilities.StringToByteArray(rom.MD5)) // MD5
                                .Concat(Utilities.StringToByteArray(rom.CRC)) // CRC
                            .ToArray();
                sw.Write(data);
                sw.Write((ulong)(rom.Size ?? 0)); // Long size (Unsigned, Mirrored)

                // Now create a deflatestream from the input file
                ZlibBaseStream ds = new ZlibBaseStream(outputStream, CompressionMode.Compress, CompressionLevel.BestCompression, ZlibStreamFlavor.DEFLATE, true);

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
                sw.Write(Utilities.StringToByteArray(rom.CRC).Reverse().ToArray());
                sw.Write((uint)(rom.Size ?? 0));

                // Dispose of everything
                sw.Dispose();
                outputStream.Dispose();
            }

            return true;
        }

        /// <summary>
        /// Write a set of input files to a torrent GZ archive (assuming the same output archive name)
        /// </summary>
        /// <param name="inputFiles">Input files to be moved</param>
        /// <param name="outDir">Output directory to build to</param>
        /// <param name="rom">DatItem representing the new information</param>
        /// <returns>True if the archive was written properly, false otherwise</returns>
        public override bool Write(List<string> inputFiles, string outDir, List<Rom> roms)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
