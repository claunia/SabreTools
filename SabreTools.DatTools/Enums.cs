using System;

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

    /// <summary>
    /// Determines what sort of files only use external hashes
    /// </summary>
    /// TODO: Can FileType be used instead?
    [Flags]
    public enum TreatAsFile
    {
        CHD = 1 << 0,
        Archive = 1 << 1,
        AaruFormat = 1 << 2,

        NonArchive = CHD | AaruFormat,
        All = CHD | Archive | AaruFormat,
    }
}
