using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SabreTools.Core;
#if NET462_OR_GREATER || NETCOREAPP
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Readers;
#endif

namespace SabreTools.FileTypes.Archives
{
    /// <summary>
    /// Represents a TorrentRAR archive for reading and writing
    /// </summary>
    public class RarArchive : BaseArchive
    {
        #region Constructors

        /// <summary>
        /// Create a new TorrentRARArchive with no base file
        /// </summary>
        public RarArchive()
            : base()
        {
            this.Type = FileType.RarArchive;
        }

        /// <summary>
        /// Create a new TorrentRARArchive from the given file
        /// </summary>
        /// <param name="filename">Name of the file to use as an archive</param>
        /// <param name="read">True for opening file as read, false for opening file as write</param>
        /// <param name="getHashes">True if hashes for this file should be calculated, false otherwise (default)</param>
        public RarArchive(string filename, bool getHashes = false)
            : base(filename, getHashes)
        {
            this.Type = FileType.RarArchive;
        }

        #endregion

        #region Extraction

        /// <inheritdoc/>
        public override bool CopyAll(string outDir)
        {
#if NET462_OR_GREATER || NETCOREAPP
            bool encounteredErrors = true;

            // If we have an invalid file
            if (this.Filename == null)
                return true;

            try
            {
                // Create the temp directory
                Directory.CreateDirectory(outDir);

                // Extract all files to the temp directory
                SharpCompress.Archives.Rar.RarArchive ra = SharpCompress.Archives.Rar.RarArchive.Open(this.Filename);
                foreach (RarArchiveEntry entry in ra.Entries)
                {
                    entry.WriteToDirectory(outDir, new SharpCompress.Common.ExtractionOptions { PreserveFileTime = true, ExtractFullPath = true, Overwrite = true });
                }
                encounteredErrors = false;
                ra.Dispose();
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
#else
            // TODO: Support RAR archives in old .NET
            return true;
#endif
        }

        /// <inheritdoc/>
        public override string? CopyToFile(string entryName, string outDir)
        {
            // Try to extract a stream using the given information
            (MemoryStream? ms, string? realEntry) = CopyToStream(entryName);

            // If the memory stream and the entry name are both non-null, we write to file
            if (ms != null && realEntry != null)
            {
                realEntry = Path.Combine(outDir, realEntry);

                // Create the output subfolder now
                Directory.CreateDirectory(Path.GetDirectoryName(realEntry) ?? string.Empty);

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
        public override (MemoryStream?, string?) CopyToStream(string entryName)
        {
#if NET462_OR_GREATER || NETCOREAPP
            MemoryStream? ms = new();
            string? realEntry = null;

            // If we have an invalid file
            if (this.Filename == null)
                return (null, null);

            try
            {
                SharpCompress.Archives.Rar.RarArchive ra = SharpCompress.Archives.Rar.RarArchive.Open(this.Filename, new ReaderOptions { LeaveStreamOpen = false, });
                foreach (RarArchiveEntry entry in ra.Entries)
                {
                    if (entry != null && !entry.IsDirectory && entry.Key.Contains(entryName))
                    {
                        // Write the file out
                        realEntry = entry.Key;
                        entry.WriteTo(ms);
                    }
                }
                ra.Dispose();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                ms = null;
                realEntry = null;
            }

            return (ms, realEntry);
#else
            // TODO: Support RAR archives in old .NET
            return (null, null);
#endif
        }

        #endregion

        #region Information

        /// <inheritdoc/>
        public override List<BaseFile>? GetChildren()
        {
#if NET462_OR_GREATER || NETCOREAPP
            // If we have an invalid file
            if (this.Filename == null)
                return null;

            List<BaseFile> found = [];
            string? gamename = Path.GetFileNameWithoutExtension(this.Filename);

            try
            {
                SharpCompress.Archives.Rar.RarArchive ra = SharpCompress.Archives.Rar.RarArchive.Open(File.OpenRead(this.Filename));
                foreach (RarArchiveEntry entry in ra.Entries.Where(e => e != null && !e.IsDirectory))
                {
                    // Create a blank item for the entry
                    BaseFile rarEntryRom = new();

                    // Perform a quickscan, if flagged to
                    if (this.AvailableHashes == Hash.CRC)
                    {
                        rarEntryRom.Size = entry.Size;
                        rarEntryRom.CRC = BitConverter.GetBytes(entry.Crc);
                    }
                    // Otherwise, use the stream directly
                    else
                    {
                        using Stream entryStream = entry.OpenEntryStream();
                        rarEntryRom = GetInfo(entryStream, size: entry.Size, hashes: this.AvailableHashes);
                    }

                    // Fill in comon details and add to the list
                    rarEntryRom.Filename = entry.Key;
                    rarEntryRom.Parent = gamename;
                    rarEntryRom.Date = entry.LastModifiedTime?.ToString("yyyy/MM/dd hh:mm:ss");
                    found.Add(rarEntryRom);
                }

                // Dispose of the archive
                ra.Dispose();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }

            return found;
#else
            // TODO: Support RAR archives in old .NET
            return null;
#endif
        }

        /// <inheritdoc/>
        public override List<string> GetEmptyFolders()
        {
#if NET462_OR_GREATER || NETCOREAPP
            List<string> empties = [];

            // If we have an invalid file
            if (this.Filename == null)
                return empties;

            try
            {
                SharpCompress.Archives.Rar.RarArchive ra = SharpCompress.Archives.Rar.RarArchive.Open(this.Filename, new ReaderOptions { LeaveStreamOpen = false });
                List<RarArchiveEntry> rarEntries = ra.Entries.OrderBy(e => e.Key, new NaturalSort.NaturalReversedComparer()).ToList();
                string? lastRarEntry = null;
                foreach (RarArchiveEntry entry in rarEntries)
                {
                    if (entry != null)
                    {
                        // If the current is a superset of last, we skip it
                        if (lastRarEntry != null && lastRarEntry.StartsWith(entry.Key))
                        {
                            // No-op
                        }
                        // If the entry is a directory, we add it
                        else if (entry.IsDirectory)
                        {
                            empties.Add(entry.Key);
                            lastRarEntry = entry.Key;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return empties;
#else
            // TODO: Support RAR archives in old .NET
            return [];
#endif
        }

        /// <inheritdoc/>
        public override bool IsTorrent()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Writing

        /// <inheritdoc/>
        public override bool Write(string inputFile, string outDir, BaseFile? baseFile)
        {
            // Get the file stream for the file and write out
            return Write(File.OpenRead(inputFile), outDir, baseFile);
        }

        /// <inheritdoc/>
        public override bool Write(Stream? inputStream, string outDir, BaseFile? baseFile)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override bool Write(List<string> inputFiles, string outDir, List<BaseFile>? roms)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
