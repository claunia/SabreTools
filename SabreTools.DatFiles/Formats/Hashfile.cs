using SabreTools.Hashing;

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
        public Hashfile(DatFile? datFile, HashType hash)
            : base(datFile)
        {
            _hash = ConvertHash(hash);
        }

        /// <summary>
        /// Convert hash types between internal and Serialization
        /// </summary>
        private static Serialization.Hash ConvertHash(HashType hash)
        {
            return hash switch
            {
                HashType.CRC32 => Serialization.Hash.CRC,
                HashType.MD5 => Serialization.Hash.MD5,
                HashType.SHA1 => Serialization.Hash.SHA1,
                HashType.SHA256 => Serialization.Hash.SHA256,
                HashType.SHA384 => Serialization.Hash.SHA384,
                HashType.SHA512 => Serialization.Hash.SHA512,
                HashType.SpamSum => Serialization.Hash.SpamSum,
                _ => throw new System.ArgumentOutOfRangeException(nameof(hash)),
            };
        }
    }
}
