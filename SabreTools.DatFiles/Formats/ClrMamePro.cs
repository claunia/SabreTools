namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents a ClrMamePro DAT
    /// </summary>
    internal partial class ClrMamePro : DatFile
    {
        #region Fields

        /// <summary>
        /// Get whether to assume quote usage on read and write or not
        /// </summary>
        public bool Quotes { get; set; } = true;

        #endregion

        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        /// <param name="quotes">Enable quotes on read and write, false otherwise</param>
        public ClrMamePro(DatFile? datFile, bool quotes)
            : base(datFile)
        {
            Quotes = quotes;
        }
    }
}
