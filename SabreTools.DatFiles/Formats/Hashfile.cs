using SabreTools.Core;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents a hashfile such as an SFV, MD5, or SHA-1 file
    /// </summary>
    internal partial class Hashfile : DatFile
    {
        // Private instance variables specific to Hashfile DATs
        private readonly Hash _hash;

        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        /// <param name="hash">Type of hash that is associated with this DAT</param> 
        public Hashfile(DatFile? datFile, Hash hash)
            : base(datFile)
        {
            _hash = hash;
        }
    }
}
