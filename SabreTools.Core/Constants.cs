namespace SabreTools.Core
{
    /// <summary>
    /// Constants that are used throughout the library
    /// </summary>
    public static class Constants
    {
        #region 0-byte file constants

        public const long SizeZero = 0;
        public const string CRCZero = "00000000";
        public static readonly byte[] CRCZeroBytes =        [0x00, 0x00, 0x00, 0x00];
        public const string MD5Zero = "d41d8cd98f00b204e9800998ecf8427e";
        public static readonly byte[] MD5ZeroBytes =        [ 0xd4, 0x1d, 0x8c, 0xd9,
                                                              0x8f, 0x00, 0xb2, 0x04,
                                                              0xe9, 0x80, 0x09, 0x98,
                                                              0xec, 0xf8, 0x42, 0x7e ];
        public const string SHA1Zero = "da39a3ee5e6b4b0d3255bfef95601890afd80709";
        public static readonly byte[] SHA1ZeroBytes =       [ 0xda, 0x39, 0xa3, 0xee,
                                                              0x5e, 0x6b, 0x4b, 0x0d,
                                                              0x32, 0x55, 0xbf, 0xef,
                                                              0x95, 0x60, 0x18, 0x90,
                                                              0xaf, 0xd8, 0x07, 0x09 ];
        public const string SHA256Zero = "ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad";
        public static readonly byte[] SHA256ZeroBytes =     [ 0xba, 0x78, 0x16, 0xbf,
                                                              0x8f, 0x01, 0xcf, 0xea,
                                                              0x41, 0x41, 0x40, 0xde,
                                                              0x5d, 0xae, 0x22, 0x23,
                                                              0xb0, 0x03, 0x61, 0xa3,
                                                              0x96, 0x17, 0x7a, 0x9c,
                                                              0xb4, 0x10, 0xff, 0x61,
                                                              0xf2, 0x00, 0x15, 0xad ];
        public const string SHA384Zero = "cb00753f45a35e8bb5a03d699ac65007272c32ab0eded1631a8b605a43ff5bed8086072ba1e7cc2358baeca134c825a7";
        public const string SHA512Zero = "ddaf35a193617abacc417349ae20413112e6fa4e89a97ea20a9eeee64b55d39a2192992a274fc1a836ba3c23a3feebbd454d4423643ce80e2a9ac94fa54ca49f";
        public const string SpamSumZero = "QXX";

        #endregion

        #region Hash string length constants

        public const int CRCLength = 8;
        public const int MD5Length = 32;
        public const int SHA1Length = 40;
        public const int SHA256Length = 64;
        public const int SHA384Length = 96;
        public const int SHA512Length = 128;

        #endregion
    }
}
