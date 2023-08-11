namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents a MAME Listrom file
    /// </summary>
    internal partial class Listrom : DatFile
    {
        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public Listrom(DatFile? datFile)
            : base(datFile)
        {
        }
    }
}
