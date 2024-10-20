using System;
using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Models.CHD;

namespace SabreTools.FileTypes.CHD
{
    /// <summary>
    /// This is code adapted from chd.h and chd.cpp in MAME
    /// Additional archival code from https://github.com/rtissera/libchdr/blob/master/src/chd.h
    /// </summary>
    public class CHDFile : BaseFile
    {
        #region Private instance variables

        /// <summary>
        /// Model representing the correct CHD header
        /// </summary>
        protected Header? _header;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new CHDFile from an input file
        /// </summary>
        /// <param name="filename">Filename respresenting the CHD file</param>
        public static CHDFile? Create(string filename)
        {
            using var fs = File.OpenRead(filename);
            return Create(fs);
        }

        /// <summary>
        /// Create a new CHDFile from an input stream
        /// </summary>
        /// <param name="stream">Stream representing the CHD file</param>
        public static CHDFile? Create(Stream stream)
        {
            try
            {
                // Get the detected CHD version
                uint version = GetVersion(stream);

                // Read and return the current CHD
                return version switch
                {
                    1 => DeserializeV1(stream),
                    2 => DeserializeV2(stream),
                    3 => DeserializeV3(stream),
                    4 => DeserializeV4(stream),
                    5 => DeserializeV5(stream),
                    _ => null,
                };
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Deserializers

        /// <summary>
        /// Parse and validate the header as if it's V1
        /// </summary>
        private static CHDFile? DeserializeV1(Stream stream)
        {
            var header = new HeaderV1();

            byte[] tagBytes = stream.ReadBytes(8);
            header.Tag = Encoding.ASCII.GetString(tagBytes);
            if (header.Tag != Constants.SignatureString)
                return null;

            header.Length = stream.ReadUInt32BigEndian();
            if (header.Length != Constants.HeaderV1Size)
                return null;

            header.Version = stream.ReadUInt32BigEndian();
            header.Flags = (Flags)stream.ReadUInt32BigEndian();
            header.Compression = (CompressionType)stream.ReadUInt32BigEndian();
            if (header.Compression > CompressionType.CHDCOMPRESSION_ZLIB)
                return null;

            header.HunkSize = stream.ReadUInt32BigEndian();
            header.TotalHunks = stream.ReadUInt32BigEndian();
            header.Cylinders = stream.ReadUInt32BigEndian();
            header.Heads = stream.ReadUInt32BigEndian();
            header.Sectors = stream.ReadUInt32BigEndian();
            header.MD5 = stream.ReadBytes(16);
            header.ParentMD5 = stream.ReadBytes(16);

            return new CHDFile { _header = header, MD5 = header.MD5 };
        }

        /// <summary>
        /// Parse and validate the header as if it's V2
        /// </summary>
        private static CHDFile? DeserializeV2(Stream stream)
        {
            var header = new HeaderV2();

            byte[] tagBytes = stream.ReadBytes(8);
            header.Tag = Encoding.ASCII.GetString(tagBytes);
            if (header.Tag != Constants.SignatureString)
                return null;

            header.Length = stream.ReadUInt32BigEndian();
            if (header.Length != Constants.HeaderV2Size)
                return null;

            header.Version = stream.ReadUInt32BigEndian();
            header.Flags = (Flags)stream.ReadUInt32BigEndian();
            header.Compression = (CompressionType)stream.ReadUInt32BigEndian();
            if (header.Compression > CompressionType.CHDCOMPRESSION_ZLIB)
                return null;

            header.HunkSize = stream.ReadUInt32BigEndian();
            header.TotalHunks = stream.ReadUInt32BigEndian();
            header.Cylinders = stream.ReadUInt32BigEndian();
            header.Heads = stream.ReadUInt32BigEndian();
            header.Sectors = stream.ReadUInt32BigEndian();
            header.MD5 = stream.ReadBytes(16);
            header.ParentMD5 = stream.ReadBytes(16);
            header.BytesPerSector = stream.ReadUInt32BigEndian();

            return new CHDFile { _header = header, MD5 = header.MD5 };
        }

        /// <summary>
        /// Parse and validate the header as if it's V2
        /// </summary>
        private static CHDFile? DeserializeV3(Stream stream)
        {
            var header = new HeaderV3();

            byte[] tagBytes = stream.ReadBytes(8);
            header.Tag = Encoding.ASCII.GetString(tagBytes);
            if (header.Tag != Constants.SignatureString)
                return null;

            header.Length = stream.ReadUInt32BigEndian();
            if (header.Length != Constants.HeaderV3Size)
                return null;

            header.Version = stream.ReadUInt32BigEndian();
            header.Flags = (Flags)stream.ReadUInt32BigEndian();
            header.Compression = (CompressionType)stream.ReadUInt32BigEndian();
            if (header.Compression > CompressionType.CHDCOMPRESSION_ZLIB_PLUS)
                return null;

            header.TotalHunks = stream.ReadUInt32BigEndian();
            header.LogicalBytes = stream.ReadUInt64BigEndian();
            header.MetaOffset = stream.ReadUInt64BigEndian();
            header.MD5 = stream.ReadBytes(16);
            header.ParentMD5 = stream.ReadBytes(16);
            header.HunkBytes = stream.ReadUInt32BigEndian();
            header.SHA1 = stream.ReadBytes(20);
            header.ParentSHA1 = stream.ReadBytes(20);

            return new CHDFile { _header = header, MD5 = header.MD5, SHA1 = header.SHA1 };
        }

        /// <summary>
        /// Parse and validate the header as if it's V4
        /// </summary>
        private static CHDFile? DeserializeV4(Stream stream)
        {
            var header = new HeaderV4();

            byte[] tagBytes = stream.ReadBytes(8);
            header.Tag = Encoding.ASCII.GetString(tagBytes);
            if (header.Tag != Constants.SignatureString)
                return null;

            header.Length = stream.ReadUInt32BigEndian();
            if (header.Length != Constants.HeaderV4Size)
                return null;

            header.Version = stream.ReadUInt32BigEndian();
            header.Flags = (Flags)stream.ReadUInt32BigEndian();
            header.Compression = (CompressionType)stream.ReadUInt32BigEndian();
            if (header.Compression > CompressionType.CHDCOMPRESSION_AV)
                return null;

            header.TotalHunks = stream.ReadUInt32BigEndian();
            header.LogicalBytes = stream.ReadUInt64BigEndian();
            header.MetaOffset = stream.ReadUInt64BigEndian();
            header.HunkBytes = stream.ReadUInt32BigEndian();
            header.SHA1 = stream.ReadBytes(20);
            header.ParentSHA1 = stream.ReadBytes(20);
            header.RawSHA1 = stream.ReadBytes(20);

            return new CHDFile { _header = header, SHA1 = header.SHA1 };
        }

        /// <summary>
        /// Parse and validate the header as if it's V5
        /// </summary>
        private static CHDFile? DeserializeV5(Stream stream)
        {
            var header = new HeaderV5();

            byte[] tagBytes = stream.ReadBytes(8);
            header.Tag = Encoding.ASCII.GetString(tagBytes);
            if (header.Tag != Constants.SignatureString)
                return null;

            header.Length = stream.ReadUInt32BigEndian();
            if (header.Length != Constants.HeaderV5Size)
                return null;

            header.Version = stream.ReadUInt32BigEndian();
            header.Compressors = new uint[4];
            for (int i = 0; i < header.Compressors.Length; i++)
            {
                header.Compressors[i] = stream.ReadUInt32BigEndian();
            }

            header.LogicalBytes = stream.ReadUInt64BigEndian();
            header.MapOffset = stream.ReadUInt64BigEndian();
            header.MetaOffset = stream.ReadUInt64BigEndian();
            header.HunkBytes = stream.ReadUInt32BigEndian();
            header.UnitBytes = stream.ReadUInt32BigEndian();
            header.RawSHA1 = stream.ReadBytes(20);
            header.SHA1 = stream.ReadBytes(20);
            header.ParentSHA1 = stream.ReadBytes(20);

            return new CHDFile { _header = header, SHA1 = header.SHA1 };
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Get the matching CHD version, if possible
        /// </summary>
        /// <returns>Matching version, 0 if none</returns>
        private static uint GetVersion(Stream stream)
        {
            // Read the header values
            byte[] tagBytes = stream.ReadBytes(8);
            string tag = Encoding.ASCII.GetString(tagBytes);
            uint length = stream.ReadUInt32BigEndian();
            uint version = stream.ReadUInt32BigEndian();

            // Seek back to start
            stream.SeekIfPossible();

            // Check the signature
            if (!string.Equals(tag, Constants.SignatureString, StringComparison.Ordinal))
                return 0;

            // Match the version to header length
            return (version, length) switch
            {
                (1, Constants.HeaderV1Size) => version,
                (2, Constants.HeaderV2Size) => version,
                (3, Constants.HeaderV3Size) => version,
                (4, Constants.HeaderV4Size) => version,
                (5, Constants.HeaderV5Size) => version,
                _ => 0,
            };
        }

        #endregion
    }
}
