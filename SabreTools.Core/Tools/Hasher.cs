using System;
using System.Linq;
using System.Security.Cryptography;

using Aaru.Checksums;

namespace SabreTools.Core.Tools
{
    /// <summary>
    /// Async hashing class wraper
    /// </summary>
    public class Hasher
    {
        public Hash HashType { get; private set; }
        private IDisposable _hasher; 

        public Hasher(Hash hashType)
        {
            this.HashType = hashType;
            GetHasher();
        }

        /// <summary>
        /// Generate the correct hashing class based on the hash type
        /// </summary>
        private void GetHasher()
        {
            switch (HashType)
            {
                case Hash.CRC:
                    _hasher = new OptimizedCRC.OptimizedCRC();
                    break;

                case Hash.MD5:
                    _hasher = MD5.Create();
                    break;

                case Hash.SHA1:
                    _hasher = SHA1.Create();
                    break;

                case Hash.SHA256:
                    _hasher = SHA256.Create();
                    break;

                case Hash.SHA384:
                    _hasher = SHA384.Create();
                    break;

                case Hash.SHA512:
                    _hasher = SHA512.Create();
                    break;

                case Hash.SpamSum:
                    _hasher = new SpamSumContext();
                    break;
            }
        }

        public void Dispose()
        {
            _hasher.Dispose();
        }

        /// <summary>
        /// Process a buffer of some length with the internal hash algorithm
        /// </summary>
        public void Process(byte[] buffer, int size)
        {
            switch (HashType)
            {
                case Hash.CRC:
                    (_hasher as OptimizedCRC.OptimizedCRC).Update(buffer, 0, size);
                    break;

                case Hash.MD5:
                case Hash.SHA1:
                case Hash.SHA256:
                case Hash.SHA384:
                case Hash.SHA512:
                    (_hasher as HashAlgorithm).TransformBlock(buffer, 0, size, null, 0);
                    break;

                case Hash.SpamSum:
                    (_hasher as SpamSumContext).Update(buffer);
                    break;
            }
        }

        /// <summary>
        /// Finalize the internal hash algorigthm
        /// </summary>
        public void Finalize()
        {
            byte[] emptyBuffer = new byte[0];
            switch (HashType)
            {
                case Hash.CRC:
                    (_hasher as OptimizedCRC.OptimizedCRC).Update(emptyBuffer, 0, 0);
                    break;

                case Hash.MD5:
                case Hash.SHA1:
                case Hash.SHA256:
                case Hash.SHA384:
                case Hash.SHA512:
                    (_hasher as HashAlgorithm).TransformFinalBlock(emptyBuffer, 0, 0);
                    break;

                case Hash.SpamSum:
                    // No finalization step needed
                    break;
            }
        }

        /// <summary>
        /// Get internal hash as a byte array
        /// </summary>
        public byte[] GetHash()
        {
            switch (HashType)
            {
                case Hash.CRC:
                    return BitConverter.GetBytes((_hasher as OptimizedCRC.OptimizedCRC).Value).Reverse().ToArray();

                case Hash.MD5:
                case Hash.SHA1:
                case Hash.SHA256:
                case Hash.SHA384:
                case Hash.SHA512:
                    return (_hasher as HashAlgorithm).Hash;

                case Hash.SpamSum:
                    return (_hasher as SpamSumContext).Final();
            }

            return null;
        }
    }
}
