namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing and writing of an Everdrive SMDB file
    /// </summary>
    internal partial class EverdriveSMDB : DatFile
    {
        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public EverdriveSMDB(DatFile datFile)
            : base(datFile)
        {
        }
    }
}
