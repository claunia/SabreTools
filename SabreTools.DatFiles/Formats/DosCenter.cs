namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents a DosCenter DAT
    /// </summary>
    internal partial class DosCenter : DatFile
    {
        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public DosCenter(DatFile? datFile)
            : base(datFile)
        {
        }
    }
}
