namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents a RomCenter INI file
    /// </summary>
    internal partial class RomCenter : DatFile
    {
        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public RomCenter(DatFile datFile)
            : base(datFile)
        {
        }
    }
}
