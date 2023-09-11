using SabreTools.Core;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents a hashfile such as an SFV, MD5, or SHA-1 file
    /// </summary>
    internal partial class Hashfile : DatFile
    {
        // Private instance variables specific to Hashfile DATs
        private readonly Serialization.Hash _hash;

        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        /// <param name="hash">Type of hash that is associated with this DAT</param> 
        public Hashfile(DatFile? datFile, Hash hash)
            : base(datFile)
        {
            _hash = ConvertHash(hash);
        }

        /// <summary>
        /// Convert hash types between internal and Serialization
        /// </summary>
        private Serialization.Hash ConvertHash(Hash hash)
        {
            return hash switch
            {
                Hash.CRC => Serialization.Hash.CRC,
                Hash.MD5 => Serialization.Hash.MD5,
                Hash.SHA1 => Serialization.Hash.SHA1,
                Hash.SHA256 => Serialization.Hash.SHA256,
                Hash.SHA384 => Serialization.Hash.SHA384,
                Hash.SHA512 => Serialization.Hash.SHA512,
                Hash.SpamSum => Serialization.Hash.SpamSum,
                _ => throw new System.ArgumentOutOfRangeException(),
            };
        }
    }
}
