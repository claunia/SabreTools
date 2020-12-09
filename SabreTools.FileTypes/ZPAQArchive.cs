using System;
using System.Collections.Generic;
using System.IO;

using SabreTools.Core;

namespace SabreTools.FileTypes
{
    /// <summary>
    /// Represents a ZPAQArchive archive for reading and writing
    /// </summary>
    /// TODO: Implement from source at https://github.com/zpaq/zpaq - In progress as external DLL
    public class ZPAQArchive : BaseArchive
    {
        #region Constructors

        /// <summary>
        /// Create a new ZPAQArchive with no base file
        /// </summary>
        public ZPAQArchive()
            : base()
        {
            this.Type = FileType.ZPAQArchive;
        }

        /// <summary>
        /// Create a new ZPAQArchive from the given file
        /// </summary>
        /// <param name="filename">Name of the file to use as an archive</param>
        /// <param name="getHashes">True if hashes for this file should be calculated, false otherwise (default)</param>
        public ZPAQArchive(string filename, bool getHashes = false)
            : base(filename, getHashes)
        {
            this.Type = FileType.ZPAQArchive;
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
