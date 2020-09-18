using System.IO;
using SabreTools.Library.DatFiles;
using SabreTools.Library.IO;

namespace SabreTools.Library.FileTypes
{
    public class BaseFile
    {
        #region Publicly facing variables

        // TODO: Get all of these values automatically so there is no public "set"
        public FileType Type { get; protected set; }
        public string Filename { get; set; }
        public string Parent { get; set; }
        public string Date { get; set; }
        public long? Size { get; set; }
        public byte[] CRC { get; set; }
        public byte[] MD5 { get; set; }
#if NET_FRAMEWORK
        public byte[] RIPEMD160 { get; set; }
#endif
        public byte[] SHA1 { get; set; }
        public byte[] SHA256 { get; set; }
        public byte[] SHA384 { get; set; }
        public byte[] SHA512 { get; set; }
        public byte[] SpamSum { get; set; }

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
                BaseFile temp = FileExtensions.GetInfo(this.Filename);
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
                BaseFile temp = stream.GetInfo();
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

        #region Manipulation

        /// <summary>
        /// Remove hashes from the metadata
        /// </summary>
        /// <param name="remove">Hash representing hashes to remove</param>
        public void RemoveHashes(Hash remove)
        {
            if (remove.HasFlag(Hash.CRC))
                CRC = null;
            if (remove.HasFlag(Hash.MD5))
                MD5 = null;
#if NET_FRAMEWORK
            if (remove.HasFlag(Hash.RIPEMD160))
                RIPEMD160 = null;
#endif
            if (remove.HasFlag(Hash.SHA1))
                SHA1 = null;
            if (remove.HasFlag(Hash.SHA256))
                SHA256 = null;
            if (remove.HasFlag(Hash.SHA384))
                SHA384 = null;
            if (remove.HasFlag(Hash.SHA512))
                SHA512 = null;
            if (remove.HasFlag(Hash.SpamSum))
                SpamSum = null;
        }

        #endregion
    }
}
