using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Models.CHD;

namespace SabreTools.FileTypes.CHD
{
    /// <summary>
    /// CHD V1 File
    /// </summary>
    public class CHDFileV1 : CHDFile
    {
        internal const int HeaderSize = 76;

        /// <summary>
        /// Parse and validate the header as if it's V1
        /// </summary>
        internal static CHDFileV1? Deserialize(Stream stream)
        {
            var header = new HeaderV1();

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
            if (header.Compression > CompressionType.CHDCOMPRESSION_ZLIB)
                return null;

            header.HunkSize = stream.ReadUInt32BigEndian();
            header.TotalHunks = stream.ReadUInt32BigEndian();
            header.Cylinders = stream.ReadUInt32BigEndian();
            header.Heads = stream.ReadUInt32BigEndian();
            header.Sectors = stream.ReadUInt32BigEndian();
            header.MD5 = stream.ReadBytes(16);
            header.ParentMD5 = stream.ReadBytes(16);

            return new CHDFileV1 { _header = header, MD5 = header.MD5 };
        }

        /// <inheritdoc/>
        /// <remarks>Returns MD5 hash or empty array</remarks>
        public override byte[] GetHash()
        {
            return (_header as HeaderV1)?.MD5 ?? [];
        }
    }
}
