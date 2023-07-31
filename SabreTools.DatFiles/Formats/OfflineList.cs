namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents an OfflineList XML DAT
    /// </summary>
    internal partial class OfflineList : DatFile
    {
        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public OfflineList(DatFile datFile)
            : base(datFile)
        {
        }
    }
}
