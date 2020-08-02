namespace SabreTools.Library.DatFiles
{
    /// <summary>
    /// Determines how the current dictionary is bucketed by
    /// </summary>
    public enum BucketedBy
    {
        Default = 0,
        Size,
        CRC,
        MD5,
#if NET_FRAMEWORK
        RIPEMD160,
#endif
        SHA1,
        SHA256,
        SHA384,
        SHA512,
        Game,
    }

    /// <summary>
    /// Determines the DAT deduplication type
    /// </summary>
    public enum DedupeType
    {
        None = 0,
        Full,

        // Force only deduping with certain types
        Game,
        CRC,
        MD5,
#if NET_FRAMEWORK
        RIPEMD160,
#endif
        SHA1,
        SHA256,
        SHA384,
        SHA512,
    }

    /// <summary>
    /// Determines forcemerging tag for DAT output
    /// </summary>
    public enum ForceMerging
    {
        None = 0,
        Split,
        Merged,
        NonMerged,
        Full,
    }

    /// <summary>
    /// Determines forcenodump tag for DAT output
    /// </summary>
    public enum ForceNodump
    {
        None = 0,
        Obsolete,
        Required,
        Ignore,
    }

    /// <summary>
    /// Determines forcepacking tag for DAT output
    /// </summary>
    public enum ForcePacking
    {
        None = 0,
        Zip,
        Unzip,
    }

    /// <summary>
    /// Determines which files should be skipped in DFD
    /// </summary>
    public enum SkipFileType
    {
        None = 0,
        Archive,
        File,
    }

    /// <summary>
    /// Determines how a DAT will be split internally
    /// </summary>
    public enum SplitType
    {
        None = 0,
        NonMerged,
        Merged,
        FullNonMerged,
        Split,
        DeviceNonMerged
    }
}
