using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using SabreTools.Core;
using SabreTools.IO;
using SharpCompress.Compressors.Xz;

namespace SabreTools.Library.FileTypes
{
    /// <summary>
    /// Represents a TorrentXZ archive for reading and writing
    /// </summary>
    public class XZArchive : BaseArchive
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
        public XZArchive()
            : base()
        {
            this.Type = FileType.XZArchive;
        }

        /// <summary>
        /// Create a new TorrentGZipArchive from the given file
        /// </summary>
        /// <param name="filename">Name of the file to use as an archive</param>
        /// <param name="read">True for opening file as read, false for opening file as write</param>
        /// <param name="getHashes">True if hashes for this file should be calculated, false otherwise (default)</param>
        public XZArchive(string filename, bool getHashes = false)
            : base(filename, getHashes)
        {
            this.Type = FileType.XZArchive;
        }

        #endregion

        #region Extraction

        /// <inheritdoc/>
        public override bool CopyAll(string outDir)
        {
            bool encounteredErrors = true;

            try
            {
                // Create the temp directory
                Directory.CreateDirectory(outDir);

                // Decompress the _filename stream
                FileStream outstream = File.Create(Path.Combine(outDir, Path.GetFileNameWithoutExtension(this.Filename)));
                var xz = new XZStream(File.OpenRead(this.Filename));
                xz.CopyTo(outstream);

                // Dispose of the streams
                outstream.Dispose();
                xz.Dispose();

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
                FileStream fs = File.Create(realEntry);
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

        /// <inheritdoc/>
        public override (MemoryStream, string) CopyToStream(string entryName)
        {
            MemoryStream ms = new MemoryStream();
            string realEntry;

            try
            {
                // Decompress the _filename stream
                realEntry = Path.GetFileNameWithoutExtension(this.Filename);
                var xz = new XZStream(File.OpenRead(this.Filename));

                // Write the file out
                byte[] xbuffer = new byte[_bufferSize];
                int xlen;
                while ((xlen = xz.Read(xbuffer, 0, _bufferSize)) > 0)
                {

                    ms.Write(xbuffer, 0, xlen);
                    ms.Flush();
                }

                // Dispose of the streams
                xz.Dispose();
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

        /// <inheritdoc/>
        public override List<BaseFile> GetChildren()
        {
            if (_children == null || _children.Count == 0)
            {
                _children = new List<BaseFile>();

                string gamename = Path.GetFileNameWithoutExtension(this.Filename);

                BaseFile possibleTxz = GetTorrentXZFileInfo();

                // If it was, then add it to the outputs and continue
                if (possibleTxz != null && possibleTxz.Filename != null)
                {
                    _children.Add(possibleTxz);
                }
                else
                {
                    try
                    {
                        // Create a blank item for the entry
                        BaseFile xzEntryRom = new BaseFile();

                        // Perform a quickscan, if flagged to
                        if (this.AvailableHashes == Hash.CRC)
                        {
                            xzEntryRom.Filename = gamename;
                            using (BinaryReader br = new BinaryReader(File.OpenRead(this.Filename)))
                            {
                                br.BaseStream.Seek(-8, SeekOrigin.End);
                                xzEntryRom.CRC = br.ReadBytesBigEndian(4);
                                xzEntryRom.Size = br.ReadInt32BigEndian();
                            }
                        }
                        // Otherwise, use the stream directly
                        else
                        {
                            var xzStream = new XZStream(File.OpenRead(this.Filename));
                            xzEntryRom = GetInfo(xzStream, hashes: this.AvailableHashes);
                            xzEntryRom.Filename = gamename;
                            xzStream.Dispose();
                        }

                        // Fill in comon details and add to the list
                        xzEntryRom.Parent = gamename;
                        _children.Add(xzEntryRom);
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

        /// <inheritdoc/>
        public override List<string> GetEmptyFolders()
        {
            // XZ files don't contain directories
            return new List<string>();
        }

        /// <inheritdoc/>
        public override bool IsTorrent()
        {
            // Check for the file existing first
            if (!File.Exists(this.Filename))
                return false;

            string datum = Path.GetFileName(this.Filename).ToLowerInvariant();

            // Check if the name is the right length
            if (!Regex.IsMatch(datum, @"^[0-9a-f]{" + Constants.SHA1Length + @"}\.xz"))
            {
                logger.Warning($"Non SHA-1 filename found, skipping: '{Path.GetFullPath(this.Filename)}'");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Retrieve file information for a single torrent XZ file
        /// </summary>
        /// <returns>Populated DatItem object if success, empty one on error</returns>
        public BaseFile GetTorrentXZFileInfo()
        {
            // Check for the file existing first
            if (!File.Exists(this.Filename))
                return null;

            string datum = Path.GetFileName(this.Filename).ToLowerInvariant();

            // Check if the name is the right length
            if (!Regex.IsMatch(datum, @"^[0-9a-f]{" + Constants.SHA1Length + @"}\.xz"))
            {
                logger.Warning($"Non SHA-1 filename found, skipping: '{Path.GetFullPath(this.Filename)}'");
                return null;
            }

            BaseFile baseFile = new BaseFile
            {
                Filename = Path.GetFileNameWithoutExtension(this.Filename).ToLowerInvariant(),
                SHA1 = Utilities.StringToByteArray(Path.GetFileNameWithoutExtension(this.Filename)),

                Parent = Path.GetFileNameWithoutExtension(this.Filename).ToLowerInvariant(),
            };

            return baseFile;
        }

        #endregion

        #region Writing

        /// <inheritdoc/>
        public override bool Write(string inputFile, string outDir, BaseFile baseFile)
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
        public override bool Write(Stream inputStream, string outDir, BaseFile baseFile)
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
            baseFile = GetInfo(inputStream, keepReadOpen: true);

            // Get the output file name
            string outfile = Path.Combine(outDir, PathExtensions.GetDepotPath(Utilities.ByteArrayToString(baseFile.SHA1), Depth));
            outfile = outfile.Replace(".gz", ".xz");

            // Check to see if the folder needs to be created
            if (!Directory.Exists(Path.GetDirectoryName(outfile)))
                Directory.CreateDirectory(Path.GetDirectoryName(outfile));

            // If the output file exists, don't try to write again
            if (!File.Exists(outfile))
            {
                // Compress the input stream
                XZStream outputStream = new XZStream(File.Create(outfile));
                inputStream.CopyTo(outputStream);

                // Dispose of everything
                outputStream.Dispose();
            }

            return true;
        }

        /// <inheritdoc/>
        public override bool Write(List<string> inputFiles, string outDir, List<BaseFile> baseFiles)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
