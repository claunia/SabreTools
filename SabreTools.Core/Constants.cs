using System.IO;
using System.Reflection;

namespace SabreTools.Core
{
    /// <summary>
    /// Constants that are used throughout the library
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The current toolset version to be used by all child applications
        /// </summary>
        //public readonly static string Version = $"v1.0.4";
        public readonly static string Version = $"v1.0.4-{File.GetCreationTime(Assembly.GetExecutingAssembly().Location):yyyy-MM-dd HH:mm:ss}";
        public const int HeaderHeight = 3;

        #region 0-byte file constants

        public const long SizeZero = 0;
        public const string CRCZero = "00000000";
        public static readonly byte[] CRCZeroBytes =        { 0x00, 0x00, 0x00, 0x00 };
        public const string MD5Zero = "d41d8cd98f00b204e9800998ecf8427e";
        public static readonly byte[] MD5ZeroBytes =        { 0xd4, 0x1d, 0x8c, 0xd9,
                                                              0x8f, 0x00, 0xb2, 0x04,
                                                              0xe9, 0x80, 0x09, 0x98,
                                                              0xec, 0xf8, 0x42, 0x7e };
#if NET_FRAMEWORK
        public const string RIPEMD160Zero = "9c1185a5c5e9fc54612808977ee8f548b2258d31";
        public static readonly byte[] RIPEMD160ZeroBytes =  { 0x9c, 0x11, 0x85, 0xa5,
                                                              0xc5, 0xe9, 0xfc, 0x54,
                                                              0x61, 0x28, 0x08, 0x97,
                                                              0x7e, 0xe8, 0xf5, 0x48,
                                                              0xb2, 0x25, 0x8d, 0x31 };
#endif
        public const string SHA1Zero = "da39a3ee5e6b4b0d3255bfef95601890afd80709";
        public static readonly byte[] SHA1ZeroBytes =       { 0xda, 0x39, 0xa3, 0xee,
                                                              0x5e, 0x6b, 0x4b, 0x0d,
                                                              0x32, 0x55, 0xbf, 0xef,
                                                              0x95, 0x60, 0x18, 0x90,
                                                              0xaf, 0xd8, 0x07, 0x09 };
        public const string SHA256Zero = "ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad";
        public static readonly byte[] SHA256ZeroBytes =     { 0xba, 0x78, 0x16, 0xbf,
                                                              0x8f, 0x01, 0xcf, 0xea,
                                                              0x41, 0x41, 0x40, 0xde,
                                                              0x5d, 0xae, 0x22, 0x23,
                                                              0xb0, 0x03, 0x61, 0xa3,
                                                              0x96, 0x17, 0x7a, 0x9c,
                                                              0xb4, 0x10, 0xff, 0x61,
                                                              0xf2, 0x00, 0x15, 0xad };
        public const string SHA384Zero = "cb00753f45a35e8bb5a03d699ac65007272c32ab0eded1631a8b605a43ff5bed8086072ba1e7cc2358baeca134c825a7";
        public static readonly byte[] SHA384ZeroBytes =     { 0xcb, 0x00, 0x75, 0x3f,
                                                              0x45, 0xa3, 0x5e, 0x8b,
                                                              0xb5, 0xa0, 0x3d, 0x69,
                                                              0x9a, 0xc6, 0x50, 0x07,
                                                              0x27, 0x2c, 0x32, 0xab,
                                                              0x0e, 0xde, 0xd1, 0x63,
                                                              0x1a, 0x8b, 0x60, 0x5a,
                                                              0x43, 0xff, 0x5b, 0xed,
                                                              0x80, 0x86, 0x07, 0x2b,
                                                              0xa1, 0xe7, 0xcc, 0x23,
                                                              0x58, 0xba, 0xec, 0xa1,
                                                              0x34, 0xc8, 0x25, 0xa7 };
        public const string SHA512Zero = "ddaf35a193617abacc417349ae20413112e6fa4e89a97ea20a9eeee64b55d39a2192992a274fc1a836ba3c23a3feebbd454d4423643ce80e2a9ac94fa54ca49f";
        public static readonly byte[] SHA512ZeroBytes =     { 0xdd, 0xaf, 0x35, 0xa1,
                                                              0x93, 0x61, 0x7a, 0xba,
                                                              0xcc, 0x41, 0x73, 0x49,
                                                              0xae, 0x20, 0x41, 0x31,
                                                              0x12, 0xe6, 0xfa, 0x4e,
                                                              0x89, 0xa9, 0x7e, 0xa2,
                                                              0x0a, 0x9e, 0xee, 0xe6,
                                                              0x4b, 0x55, 0xd3, 0x9a,
                                                              0x21, 0x92, 0x99, 0x2a,
                                                              0x27, 0x4f, 0xc1, 0xa8,
                                                              0x36, 0xba, 0x3c, 0x23,
                                                              0xa3, 0xfe, 0xeb, 0xbd,
                                                              0x45, 0x4d, 0x44, 0x23,
                                                              0x64, 0x3c, 0xe8, 0x0e,
                                                              0x2a, 0x9a, 0xc9, 0x4f,
                                                              0xa5, 0x4c, 0xa4, 0x9f };
        public const string SpamSumZero = "QXX";
        public static readonly byte[] SpamSumZeroBytes = { 0x51, 0x58, 0x58 };

        #endregion

        #region Hash string length constants

        public const int CRCLength = 8;
        public const int MD5Length = 32;
#if NET_FRAMEWORK
        public const int RIPEMD160Length = 40;
#endif
        public const int SHA1Length = 40;
        public const int SHA256Length = 64;
        public const int SHA384Length = 96;
        public const int SHA512Length = 128;

        #endregion

        #region TorrentZip, T7z, and TGZ headers

        /* TorrentZip Header Format
            https://pkware.cachefly.net/webdocs/APPNOTE/APPNOTE_6.2.0.txt
            http://www.romvault.com/trrntzip_explained.doc

            00-03		Local file header signature (0x50, 0x4B, 0x03, 0x04) ZipSignature
            04-05		Version needed to extract (0x14, 0x00)
            06-07		General purpose bit flag (0x02, 0x00)
            08-09		Compression method (0x08, 0x00)
            0A-0B		Last mod file time (0x00, 0xBC)
            0C-0D		Last mod file date (0x98, 0x21)
        */
        public readonly static byte[] TorrentZipHeader = new byte[] { 0x50, 0x4b, 0x03, 0x04, 0x14, 0x00, 0x02, 0x00, 0x08, 0x00, 0x00, 0xbc, 0x98, 0x21 };

        /* Torrent7z Header Format
            http://cpansearch.perl.org/src/BJOERN/Compress-Deflate7-1.0/7zip/DOC/7zFormat.txt

            00-05		Local file header signature (0x37, 0x7A, 0xBC, 0xAF, 0x27, 0x1C) SevenZipSignature
            06-07		ArchiveVersion (0x00, 0x03)
            The rest is unknown
        */
        public readonly static byte[] Torrent7ZipHeader = new byte[] { 0x37, 0x7a, 0xbc, 0xaf, 0x27, 0x1c, 0x00, 0x03 };
        public readonly static byte[] Torrent7ZipSignature = new byte[] { 0xa9, 0xa9, 0x9f, 0xd1, 0x57, 0x08, 0xa9, 0xd7, 0xea, 0x29, 0x64, 0xb2,
            0x36, 0x1b, 0x83, 0x52, 0x33, 0x00, 0x74, 0x6f, 0x72, 0x72, 0x65, 0x6e, 0x74, 0x37, 0x7a, 0x5f, 0x30, 0x2e, 0x39, 0x62, 0x65, 0x74, 0x61 };

        /* (Torrent)GZ Header Format
            https://tools.ietf.org/html/rfc1952

            00-01		Identification (0x1F, 0x8B) GzSignature
            02			Compression Method (0-7 reserved, 8 deflate; 0x08)
            03			Flags (0 FTEXT, 1 FHCRC, 2 FEXTRA, 3 FNAME, 4 FCOMMENT, 5 reserved, 6 reserved, 7 reserved; 0x04)
            04-07		Modification time (Unix format; 0x00, 0x00, 0x00, 0x00)
            08			Extra Flags (2 maximum compression, 4 fastest algorithm; 0x00)
            09			OS (See list on https://tools.ietf.org/html/rfc1952; 0x00)
            0A-0B		Length of extra field (mirrored; 0x1C, 0x00)
            0C-27		Extra field
                0C-1B	MD5 Hash
                1C-1F	CRC hash
                20-27	Int64 size (mirrored)
        */
        public readonly static byte[] TorrentGZHeader = new byte[] { 0x1f, 0x8b, 0x08, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1c, 0x00 };

        /* (Torrent)XZ Header Format
            https://tukaani.org/xz/xz-file-format.txt

            00-05		Identification (0xFD, '7', 'z', 'X', 'Z', 0x00) XzSignature
            06			Flags (0x01 - CRC32, 0x04 - CRC64, 0x0A - SHA-256)
            07-0A		Flags CRC32 (uint, little-endian)
        */
        public readonly static byte[] TorrentXZHeader = new byte[] { 0xfd, 0x37, 0x7a, 0x58, 0x5a, 0x00, 0x01, 0x69, 0x22, 0xde, 0x36 };

        #endregion

        #region ZIP internal signatures

        public const uint LocalFileHeaderSignature = 0x04034b50;
        public const uint EndOfLocalFileHeaderSignature = 0x08074b50;
        public const uint CentralDirectoryHeaderSignature = 0x02014b50;
        public const uint EndOfCentralDirSignature = 0x06054b50;
        public const uint Zip64EndOfCentralDirSignature = 0x06064b50;
        public const uint Zip64EndOfCentralDirectoryLocator = 0x07064b50;
        public const uint TorrentZipFileDateTime = 0x2198BC00;

        #endregion
    }
}
