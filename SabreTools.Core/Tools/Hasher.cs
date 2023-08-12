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
        private IDisposable? _hasher;

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
            _hasher?.Dispose();
        }

        /// <summary>
        /// Process a buffer of some length with the internal hash algorithm
        /// </summary>
        public void Process(byte[] buffer, int size)
        {
            if (_hasher == null)
                return;

            switch (_hasher)
            {
                case OptimizedCRC.OptimizedCRC crc:
                    crc.Update(buffer, 0, size);
                    break;

                case MD5 md5:
                    md5.TransformBlock(buffer, 0, size, null, 0);
                    break;

                case SHA1 sha1:
                    sha1.TransformBlock(buffer, 0, size, null, 0);
                    break;

                case SHA256 sha256:
                    sha256.TransformBlock(buffer, 0, size, null, 0);
                    break;

                case SHA384 sha384:
                    sha384.TransformBlock(buffer, 0, size, null, 0);
                    break;

                case SHA512 sha512:
                    sha512.TransformBlock(buffer, 0, size, null, 0);
                    break;

                case SpamSumContext spamSum:
                    spamSum.Update(buffer);
                    break;
            }
        }

        /// <summary>
        /// Terminate the internal hash algorigthm
        /// </summary>
        public void Terminate()
        {
            if (_hasher == null)
                return;

            byte[] emptyBuffer = Array.Empty<byte>();
            switch (_hasher)
            {
                case OptimizedCRC.OptimizedCRC crc:
                    crc.Update(emptyBuffer, 0, 0);
                    break;

                case MD5 md5:
                    md5.TransformFinalBlock(emptyBuffer, 0, 0);
                    break;

                case SHA1 sha1:
                    sha1.TransformFinalBlock(emptyBuffer, 0, 0);
                    break;

                case SHA256 sha256:
                    sha256.TransformFinalBlock(emptyBuffer, 0, 0);
                    break;

                case SHA384 sha384:
                    sha384.TransformFinalBlock(emptyBuffer, 0, 0);
                    break;

                case SHA512 sha512:
                    sha512.TransformFinalBlock(emptyBuffer, 0, 0);
                    break;

                case SpamSumContext:
                    // No finalization step needed
                    break;
            }
        }

        /// <summary>
        /// Get internal hash as a byte array
        /// </summary>
        public byte[]? GetHash()
        {
            return _hasher switch
            {
                OptimizedCRC.OptimizedCRC crc => BitConverter.GetBytes(crc.Value).Reverse().ToArray(),
                MD5 md5 => md5.Hash,
                SHA1 sha1 => sha1.Hash,
                SHA256 sha256 => sha256.Hash,
                SHA384 sha384 => sha384.Hash,
                SHA512 sha512 => sha512.Hash,
                SpamSumContext spamSum => spamSum.Final(),
                _ => null,
            };
        }
    }
}
