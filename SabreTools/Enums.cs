using System;

namespace SabreTools
{
    /// <summary>
    /// Determines how the DAT will be split on output
    /// </summary>
    [Flags]
    public enum SplittingMode
    {
        None = 0x00,

        Extension = 1 << 0,
        Hash = 1 << 1,
        Level = 1 << 2,
        Type = 1 << 3,
        Size = 1 << 4,
        TotalSize = 1 << 5,
    }

    /// <summary>
    /// Determines special update modes
    /// </summary>
    [Flags]
    public enum UpdateMode
    {
        None = 0x00,

        // Standard diffs
        DiffDupesOnly = 1 << 0,
        DiffNoDupesOnly = 1 << 1,
        DiffIndividualsOnly = 1 << 2,

        // Cascaded diffs
        DiffCascade = 1 << 3,
        DiffReverseCascade = 1 << 4,

        // Base diffs
        DiffAgainst = 1 << 5,

        // Special update modes
        Merge = 1 << 6,
        BaseReplace = 1 << 7,
        ReverseBaseReplace = 1 << 8,

        // Combinations
        AllDiffs = DiffDupesOnly | DiffNoDupesOnly | DiffIndividualsOnly,
    }
}
