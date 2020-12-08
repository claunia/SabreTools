using System;

namespace SabreTools.Library.DatFiles
{
    /// <summary>
    /// Determines merging tag handling for DAT output
    /// </summary>
    public enum MergingFlag
    {
        None = 0,
        Split,
        Merged,
        NonMerged,
        Full,

        /// <remarks>This is not usually defined for Merging flags</remarks>
        Device,
    }

    /// <summary>
    /// Determines nodump tag handling for DAT output
    /// </summary>
    public enum NodumpFlag
    {
        None = 0,
        Obsolete,
        Required,
        Ignore,
    }

    /// <summary>
    /// Determines packing tag handling for DAT output
    /// </summary>
    public enum PackingFlag
    {
        None = 0,

        /// <summary>
        /// Force all sets to be in archives, except disk and media
        /// </summary>
        Zip,

        /// <summary>
        /// Force all sets to be extracted into subfolders
        /// </summary>
        Unzip,

        /// <summary>
        /// Force sets with single items to be extracted to the parent folder
        /// </summary>
        Partial,

        /// <summary>
        /// Force all sets to be extracted to the parent folder
        /// </summary>
        Flat,
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
