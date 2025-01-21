namespace SabreTools.DatTools
{
    /// <summary>
    /// Determines the DAT deduplication type
    /// </summary>
    public enum DedupeType
    {
        /// <summary>
        /// No deduplication
        /// </summary>
        None = 0,

        /// <summary>
        /// Deduplicate across all available fields
        /// </summary>
        /// <remarks>Requires sorting by any hash</remarks>
        Full,

        /// <summary>
        /// Deduplicate on a per-machine basis
        /// </summary>
        /// <remarks>Requires sorting by machine</remarks>
        Game,
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
}
