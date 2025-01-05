using System.Collections.Generic;
using System.IO;

namespace SabreTools.FileTypes
{
    public abstract class BaseArchive : Folder
    {
        #region Protected instance variables

        /// <summary>
        /// Determines if real dates are written
        /// </summary>
        protected bool _realDates = false;

        /// <summary>
        /// Buffer size used by archives
        /// </summary>
        protected const int _bufferSize = 4096 * 128;

        #endregion

        #region Construtors

        /// <summary>
        /// Create a new Archive with no base file
        /// </summary>
        public BaseArchive() : base() { }

        /// <summary>
        /// Create a new BaseArchive from the given file
        /// </summary>
        /// <param name="filename">Name of the file to use</param>
        public BaseArchive(string filename) : base(filename) { }

        #endregion

        #region Extraction

        /// <inheritdoc/>
        public override abstract bool CopyAll(string outDir);

        /// <inheritdoc/>
        public override abstract string? CopyToFile(string entryName, string outDir);

        /// <inheritdoc/>
        public override abstract (Stream?, string?) GetEntryStream(string entryName);

        #endregion

        #region Information

        /// <inheritdoc/>
        public override abstract List<BaseFile>? GetChildren();

        /// <inheritdoc/>
        public override abstract List<string> GetEmptyFolders();

        /// <summary>
        /// Check whether the input file is a standardized format
        /// </summary>
        public abstract bool IsTorrent();

        #endregion

        #region Writing

        /// <summary>
        /// Set if real dates are written
        /// </summary>
        public void SetRealDates(bool realDates)
        {
            _realDates = realDates;
        }

        /// <inheritdoc/>
        public override abstract bool Write(string inputFile, string outDir, BaseFile? baseFile);

        /// <inheritdoc/>
        public override abstract bool Write(Stream? inputStream, string outDir, BaseFile? baseFile);

        /// <inheritdoc/>
        public override abstract bool Write(List<string> inputFiles, string outDir, List<BaseFile>? baseFiles);

        #endregion
    }
}
