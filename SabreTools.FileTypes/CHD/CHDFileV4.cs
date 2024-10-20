using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Models.CHD;

namespace SabreTools.FileTypes.CHD
{
    /// <summary>
    /// CHD V4 File
    /// </summary>
    public class CHDFileV4 : CHDFile
    {
        internal const int HeaderSize = 108;

        /// <summary>
        /// Parse and validate the header as if it's V4
        /// </summary>
        internal static CHDFileV4? Deserialize(Stream stream)
        {
            var header = new HeaderV4();

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
            if (header.Compression > CompressionType.CHDCOMPRESSION_AV)
                return null;

            header.TotalHunks = stream.ReadUInt32BigEndian();
            header.LogicalBytes = stream.ReadUInt64BigEndian();
            header.MetaOffset = stream.ReadUInt64BigEndian();
            header.HunkBytes = stream.ReadUInt32BigEndian();
            header.SHA1 = stream.ReadBytes(20);
            header.ParentSHA1 = stream.ReadBytes(20);
            header.RawSHA1 = stream.ReadBytes(20);

            return new CHDFileV4 { _header = header, SHA1 = header.SHA1 };
        }

        /// <inheritdoc/>
        /// <remarks>Returns SHA-1 hash or empty array</remarks>
        public override byte[] GetHash()
        {
            return (_header as HeaderV4)?.SHA1 ?? [];
        }
    }
}
