namespace SabreTools.Library.Data
{
    #region Archival

    /// <summary>
    /// Compression being used in CHD
    /// </summary>
    public enum CHDCompression : uint
    {
        CHDCOMPRESSION_NONE = 0,
        CHDCOMPRESSION_ZLIB = 1,
        CHDCOMPRESSION_ZLIB_PLUS = 2,
        CHDCOMPRESSION_AV = 3,
    }

    /// <summary>
    /// Availible CHD codec formats
    /// </summary>
    public enum CHD_CODEC : uint
    {
        NONE = 0,

        #region General Codecs

        ZLIB = 0x7a6c6962, // zlib
        LZMA = 0x6c7a6d61, // lzma
        HUFFMAN = 0x68756666, // huff
        FLAC = 0x666c6163, // flac

        #endregion

        #region General Codecs with CD Frontend

        CD_ZLIB = 0x63647a6c, // cdzl
        CD_LZMA = 0x63646c7a, // cdlz
        CD_FLAC = 0x6364666c, // cdfl

        #endregion

        #region A/V Codecs

        AVHUFF = 0x61766875, // avhu

        #endregion

        #region Pseudo-Codecs Returned by hunk_info

        SELF = 1,  // copy of another hunk
        PARENT = 2,  // copy of a parent's hunk
        MINI = 3,  // legacy "mini" 8-byte repeat

        #endregion
    }

    /// <summary>
    /// Compression method based on flag
    /// </summary>
    public enum CompressionMethod : ushort
    {
        Stored = 0,
        Shrunk = 1,
        ReducedCompressionFactor1 = 2,
        ReducedCompressionFactor2 = 3,
        ReducedCompressionFactor3 = 4,
        ReducedCompressionFactor4 = 5,
        Imploded = 6,
        Tokenizing = 7,
        Deflated = 8,
        Delfate64 = 9,
        PKWAREDataCompressionLibrary = 10,
        Type11 = 11, // Reserved and unused (SHOULD NOT BE USED)
        BZIP2 = 12,
        Type13 = 13, // Reserved and unused (SHOULD NOT BE USED)
        LZMA = 14,
        Type15 = 15, // Reserved and unused (SHOULD NOT BE USED)
        Type16 = 16, // Reserved and unused (SHOULD NOT BE USED)
        Type17 = 17, // Reserved and unused (SHOULD NOT BE USED)
        IBMTERSE = 18,
        IBMLZ77 = 19,
        WavPak = 97,
        PPMdVersionIRev1 = 98,
    }

    /// <summary>
    /// Type of file that is being looked at
    /// </summary>
    public enum FileType
    {
        // Singleton
        None = 0,
        CHD,

        // Can contain children
        Folder,
        SevenZipArchive,
        GZipArchive,
        LRZipArchive,
        LZ4Archive,
        RarArchive,
        TapeArchive,
        XZArchive,
        ZipArchive,
        ZPAQArchive,
        ZstdArchive,
    }

    /// <summary>
    /// Output format for rebuilt files
    /// </summary>
    public enum OutputFormat
    {
        // Currently implemented
        Folder,
        TorrentZip,
        TorrentGzip,
        TorrentGzipRomba,
        TorrentXZ,
        TorrentXZRomba,
        TapeArchive,

        // Currently unimplemented fully
        Torrent7Zip,
        TorrentRar,
        TorrentLRZip,
        TorrentLZ4,
        TorrentZstd,
        TorrentZPAQ,
    }

    #endregion

    #region Logging related

    /// <summary>
    /// Severity of the logging statement
    /// </summary>
    public enum LogLevel
    {
        VERBOSE = 0,
        USER,
        WARNING,
        ERROR,
    }

    #endregion

    #region Reader related

    /// <summary>
    /// Different types of CMP rows being parsed
    /// </summary>
    public enum CmpRowType
    {
        None,
        TopLevel,
        Standalone,
        Internal,
        Comment,
        EndTopLevel,
    }

    /// <summary>
    /// Different types of INI rows being parsed
    /// </summary>
    public enum IniRowType
    {
        None,
        SectionHeader,
        KeyValue,
        Comment,
        Invalid,
    }

    #endregion
}
