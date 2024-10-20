using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Models.CHD;

namespace SabreTools.FileTypes.CHD
{
    /// <summary>
    /// CHD V3 File
    /// </summary>
    public class CHDFileV3 : CHDFile
    {
        internal const int HeaderSize = 120;

        /// <summary>
        /// Parse and validate the header as if it's V3
        /// </summary>
        internal static CHDFileV3? Deserialize(Stream stream)
        {
            var header = new HeaderV3();

            byte[] tagBytes = stream.ReadBytes(8);
            header.Tag = Encoding.ASCII.GetString(tagBytes);
            if (header.Tag != Signature)
                return null;

            header.Length = stream.ReadUInt32BigEndian();
            if (header.Length != HeaderSize)
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

            return new CHDFileV3 { _header = header, MD5 = header.MD5, SHA1 = header.SHA1 };
        }

        /// <inheritdoc/>
        /// <remarks>Returns SHA-1 hash or empty array</remarks>
        public override byte[] GetHash()
        {
            return (_header as HeaderV3)?.SHA1 ?? [];
        }
    }
}
