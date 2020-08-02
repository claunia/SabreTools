namespace SabreTools.Library.Data
{
    #region Archival

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

    #endregion
}
