using System.IO;

using SabreTools.Library.DatFiles;
using SabreTools.Library.IO;

namespace SabreTools.Library.FileTypes
{
    public class BaseFile
    {
        // TODO: Get all of these values automatically so there is no public "set"
        #region Fields

        /// <summary>
        /// Internal type of the represented file
        /// </summary>
        public FileType Type { get; protected set; }

        /// <summary>
        /// Filename or path to the file
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// Direct parent of the file
        /// </summary>
        public string Parent { get; set; }

        /// <summary>
        /// Date stamp of the file
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// Optional size of the file
        /// </summary>
        public long? Size { get; set; }

        /// <summary>
        /// Hashes that are available for the file
        /// </summary>
        public Hash AvailableHashes { get; set; } = Hash.Standard;

        /// <summary>
        /// CRC32 hash of the file
        /// </summary>
        public byte[] CRC { get; set; } = null;

        /// <summary>
        /// MD5 hash of the file
        /// </summary>
        public byte[] MD5 { get; set; } = null;

#if NET_FRAMEWORK
        /// <summary>
        /// RIPEMD160 hash of the file
        /// </summary>
        public byte[] RIPEMD160 { get; set; } = null;
#endif

        /// <summary>
        /// SHA-1 hash of the file
        /// </summary>
        public byte[] SHA1 { get; set; } = null;

        /// <summary>
        /// SHA-256 hash of the file
        /// </summary>
        public byte[] SHA256 { get; set; } = null;

        /// <summary>
        /// SHA-384 hash of the file
        /// </summary>
        public byte[] SHA384 { get; set; } = null;

        /// <summary>
        /// SHA-512 hash of the file
        /// </summary>
        public byte[] SHA512 { get; set; } = null;

        /// <summary>
        /// SpamSum fuzzy hash of the file
        /// </summary>
        public byte[] SpamSum { get; set; } = null;

        #endregion

        #region Construtors

        /// <summary>
        /// Create a new BaseFile with no base file
        /// </summary>
        public BaseFile()
        {
        }

        /// <summary>
        /// Create a new BaseFile from the given file
        /// </summary>
        /// <param name="filename">Name of the file to use</param>
        /// <param name="getHashes">True if hashes for this file should be calculated (default), false otherwise</param>
        public BaseFile(string filename, bool getHashes = true)
        {
            this.Filename = filename;

            if (getHashes)
            {
                BaseFile temp = FileExtensions.GetInfo(this.Filename, hashes: this.AvailableHashes);
                if (temp != null)
                {
                    this.Parent = temp.Parent;
                    this.Date = temp.Date;
                    this.CRC = temp.CRC;
                    this.MD5 = temp.MD5;
#if NET_FRAMEWORK
                    this.RIPEMD160 = temp.RIPEMD160;
#endif
                    this.SHA1 = temp.SHA1;
                    this.SHA256 = temp.SHA256;
                    this.SHA384 = temp.SHA384;
                    this.SHA512 = temp.SHA512;
                    this.SpamSum = temp.SpamSum;
                }
            }
        }

        /// <summary>
        /// Create a new BaseFile from the given file
        /// </summary>
        /// <param name="filename">Name of the file to use</param>
        /// <param name="stream">Stream to populate information from</param>
        /// <param name="getHashes">True if hashes for this file should be calculated (default), false otherwise</param>
        public BaseFile(string filename, Stream stream, bool getHashes = true)
        {
            this.Filename = filename;

            if (getHashes)
            {
                BaseFile temp = stream.GetInfo(hashes: this.AvailableHashes);
                if (temp != null)
                {
                    this.Parent = temp.Parent;
                    this.Date = temp.Date;
                    this.CRC = temp.CRC;
                    this.MD5 = temp.MD5;
#if NET_FRAMEWORK
                    this.RIPEMD160 = temp.RIPEMD160;
#endif
                    this.SHA1 = temp.SHA1;
                    this.SHA256 = temp.SHA256;
                    this.SHA384 = temp.SHA384;
                    this.SHA512 = temp.SHA512;
                    this.SpamSum = temp.SpamSum;
                }
            }

        }

        #endregion
    }
}
