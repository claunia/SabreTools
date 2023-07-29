namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents an AttractMode DAT
    /// </summary>
    internal partial class AttractMode : DatFile
    {
        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public AttractMode(DatFile datFile)
            : base(datFile)
        {
        }
    }
}
