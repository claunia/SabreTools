using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Models.CHD;

namespace SabreTools.FileTypes.CHD
{
    /// <summary>
    /// CHD V5 File
    /// </summary>
    public class CHDFileV5 : CHDFile
    {
        internal const int HeaderSize = 124;

        /// <summary>
        /// Parse and validate the header as if it's V5
        /// </summary>
        internal static CHDFileV5? Deserialize(Stream stream)
        {
            var header = new HeaderV5();

            byte[] tagBytes = stream.ReadBytes(8);
            header.Tag = Encoding.ASCII.GetString(tagBytes);
            if (header.Tag != Signature)
                return null;

            header.Length = stream.ReadUInt32BigEndian();
            if (header.Length != HeaderSize)
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

            return new CHDFileV5 { _header = header, SHA1 = header.SHA1 };
        }

        /// <inheritdoc/>
        /// <remarks>Returns SHA-1 hash or empty array</remarks>
        public override byte[] GetHash()
        {
            return (_header as HeaderV5)?.SHA1 ?? [];
        }
    }
}
