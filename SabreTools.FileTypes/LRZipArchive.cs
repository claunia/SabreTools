using System;
using System.Collections.Generic;
using System.IO;

namespace SabreTools.FileTypes
{
    /// <summary>
    /// Represents a TorrentLRZip archive for reading and writing
    /// </summary>
    /// TODO: Implement from source at https://github.com/ckolivas/lrzip
    public class LRZipArchive : BaseArchive
    {
        #region Constructors

        /// <summary>
        /// Create a new LRZipArchive with no base file
        /// </summary>
        public LRZipArchive()
            : base()
        {
            this.Type = FileType.LRZipArchive;
        }

        /// <summary>
        /// Create a new LRZipArchive from the given file
        /// </summary>
        /// <param name="filename">Name of the file to use as an archive</param>
        /// <param name="getHashes">True if hashes for this file should be calculated, false otherwise (default)</param>
        public LRZipArchive(string filename, bool getHashes = false)
            : base(filename, getHashes)
        {
            this.Type = FileType.LRZipArchive;
        }

        #endregion

        #region Extraction

        /// <inheritdoc/>
        public override bool CopyAll(string outDir)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override string CopyToFile(string entryName, string outDir)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override (MemoryStream, string) CopyToStream(string entryName)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Information

        /// <inheritdoc/>
        public override List<BaseFile> GetChildren()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override List<string> GetEmptyFolders()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override bool IsTorrent()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Writing

        /// <inheritdoc/>
        public override bool Write(string inputFile, string outDir, BaseFile baseFile)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override bool Write(Stream inputStream, string outDir, BaseFile baseFile)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override bool Write(List<string> inputFiles, string outDir, List<BaseFile> baseFiles)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
