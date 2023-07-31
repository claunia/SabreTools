namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents a Archive.org file list
    /// </summary>
    internal partial class ArchiveDotOrg : DatFile
    {
        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public ArchiveDotOrg(DatFile datFile)
            : base(datFile)
        {
        }
    }
}
