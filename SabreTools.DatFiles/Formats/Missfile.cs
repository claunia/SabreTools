namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents a Missfile
    /// </summary>
    internal partial class Missfile : DatFile
    {
        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public Missfile(DatFile datFile)
            : base(datFile)
        {
        }
    }
}
