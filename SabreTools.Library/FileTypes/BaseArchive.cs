using System.Collections.Generic;
using System.IO;

using SabreTools.Library.DatItems;
using SabreTools.Library.IO;
using SabreTools.Library.Logging;

namespace SabreTools.Library.FileTypes
{
    public abstract class BaseArchive : Folder
    {
        #region Fields

        /// <summary>
        /// Determines if dates are read or written
        /// </summary>
        public bool UseDates { get; set; } = false;

        #endregion

        #region Protected instance variables

        /// <summary>
        /// Buffer size used by archives
        /// </summary>
        protected const int _bufferSize = 4096 * 128;

        #endregion

        #region Construtors

        /// <summary>
        /// Create a new Archive with no base file
        /// </summary>
        public BaseArchive()
        {
        }

        /// <summary>
        /// Create a new Archive from the given file
        /// </summary>
        /// <param name="filename">Name of the file to use as an archive</param>
        /// <param name="getHashes">True if hashes for this file should be calculated, false otherwise (default)</param>
        public BaseArchive(string filename, bool getHashes = false)
            : base(filename, getHashes)
        {
        }

        /// <summary>
        /// Create an archive object from a filename, if possible
        /// </summary>
        /// <param name="input">Name of the file to create the archive from</param>
        /// <returns>Archive object representing the inputs</returns>
        public static BaseArchive Create(string input)
        {
            BaseArchive archive = null;

            // First get the archive type
            FileType? at = input.GetFileType();

            // If we got back null, then it's not an archive, so we we return
            if (at == null)
                return archive;

            // Create the archive based on the type
            logger.Verbose($"Found archive of type: {at}");
            switch (at)
            {
                case FileType.GZipArchive:
                    archive = new GZipArchive(input);
                    break;

                case FileType.RarArchive:
                    archive = new RarArchive(input);
                    break;

                case FileType.SevenZipArchive:
                    archive = new SevenZipArchive(input);
                    break;

                case FileType.TapeArchive:
                    archive = new TapeArchive(input);
                    break;

                case FileType.ZipArchive:
                    archive = new ZipArchive(input);
                    break;

                default:
                    // We ignore all other types for now
                    break;
            }

            return archive;
        }

        /// <summary>
        /// Create an archive object of the specified type, if possible
        /// </summary>
        /// <param name="archiveType">SharpCompress.Common.ArchiveType representing the archive to create</param>
        /// <returns>Archive object representing the inputs</returns>
        public static BaseArchive Create(FileType archiveType)
        {
            switch (archiveType)
            {
                case FileType.GZipArchive:
                    return new GZipArchive();

                case FileType.RarArchive:
                    return new RarArchive();

                case FileType.SevenZipArchive:
                    return new SevenZipArchive();

                case FileType.TapeArchive:
                    return new TapeArchive();

                case FileType.ZipArchive:
                    return new ZipArchive();

                default:
                    return null;
            }
        }

        #endregion

        #region Extraction

        /// <summary>
        /// Attempt to extract a file as an archive
        /// </summary>
        /// <param name="outDir">Output directory for archive extraction</param>
        /// <returns>True if the extraction was a success, false otherwise</returns>
        public override abstract bool CopyAll(string outDir);

        /// <summary>
        /// Attempt to extract an entry from an archive
        /// </summary>
        /// <param name="entryName">Name of the entry to be extracted</param>
        /// <param name="outDir">Output directory for archive extraction</param>
        /// <returns>Name of the extracted file, null on error</returns>
        public override abstract string CopyToFile(string entryName, string outDir);

        /// <summary>
        /// Attempt to extract a stream from an archive
        /// </summary>
        /// <param name="entryName">Name of the entry to be extracted</param>
        /// <param name="realEntry">Output representing the entry name that was found</param>
        /// <returns>MemoryStream representing the entry, null on error</returns>
        public override abstract (MemoryStream, string) CopyToStream(string entryName);

        #endregion

        #region Information

        /// <summary>
        /// Generate a list of DatItem objects from the header values in an archive
        /// </summary>
        /// <returns>List of DatItem objects representing the found data</returns>
        public override abstract List<BaseFile> GetChildren();

        /// <summary>
        /// Generate a list of empty folders in an archive
        /// </summary>
        /// <param name="input">Input file to get data from</param>
        /// <returns>List of empty folders in the archive</returns>
        public override abstract List<string> GetEmptyFolders();

        /// <summary>
        /// Check whether the input file is a standardized format
        /// </summary>
        public abstract bool IsTorrent();

        #endregion

        #region Writing

        /// <summary>
        /// Write an input file to an archive
        /// </summary>
        /// <param name="inputFile">Input filename to be moved</param>
        /// <param name="outDir">Output directory to build to</param>
        /// <param name="rom">DatItem representing the new information</param>
        /// <returns>True if the archive was written properly, false otherwise</returns>
        public override abstract bool Write(string inputFile, string outDir, Rom rom);

        /// <summary>
        /// Write an input stream to an archive
        /// </summary>
        /// <param name="inputStream">Input stream to be moved</param>
        /// <param name="outDir">Output directory to build to</param>
        /// <param name="rom">DatItem representing the new information</param>
        /// <returns>True if the archive was written properly, false otherwise</returns>
        public override abstract bool Write(Stream inputStream, string outDir, Rom rom);

        /// <summary>
        /// Write a set of input files to an archive (assuming the same output archive name)
        /// </summary>
        /// <param name="inputFiles">Input files to be moved</param>
        /// <param name="outDir">Output directory to build to</param>
        /// <param name="rom">DatItem representing the new information</param>
        /// <returns>True if the archive was written properly, false otherwise</returns>
        public override abstract bool Write(List<string> inputFiles, string outDir, List<Rom> roms);

        #endregion
    }
}
