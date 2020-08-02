﻿using System;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Determines which type of duplicate a file is
    /// </summary>
    [Flags]
    public enum DupeType
    {
        // Type of match
        Hash = 1 << 0,
        All = 1 << 1,

        // Location of match
        Internal = 1 << 2,
        External = 1 << 3,
    }

    /// <summary>
    /// List of valid field types within a DatItem/Machine
    /// </summary>
    public enum Field : int
    {
        NULL = 0,

        // Generic DatItem
        ItemType,
        Name,
        PartName,
        PartInterface,
        Features,
        AreaName,
        AreaSize,

        // Machine
        MachineName,
        Comment,
        Description,
        Year,
        Manufacturer,
        Publisher,
        Category,
        RomOf,
        CloneOf,
        SampleOf,
        Supported,
        SourceFile,
        Runnable,
        Board,
        RebuildTo,
        Devices,
        SlotOptions,
        Infos,
        MachineType,

        // BiosSet
        Default,
        BiosDescription,

        // Disk
        MD5,
#if NET_FRAMEWORK
        RIPEMD160,
#endif
        SHA1,
        SHA256,
        SHA384,
        SHA512,
        Merge,
        Region,
        Index,
        Writable,
        Optional,
        Status,

        // Release
        Language,
        Date,

        // Rom
        Bios,
        Size,
        CRC,
        Offset,
        Inverted,
    }

    /// <summary>
    /// Determine the status of the item
    /// </summary>
    [Flags]
    public enum ItemStatus
    {
        /// <summary>
        /// This is a fake flag that is used for filter only
        /// </summary>
        NULL = 0x00,

        None = 1 << 0,
        Good = 1 << 1,
        BadDump = 1 << 2,
        Nodump = 1 << 3,
        Verified = 1 << 4,
    }

    /// <summary>
    /// Determine what type of file an item is
    /// </summary>
    public enum ItemType
    {
        Rom = 0,
        Disk = 1,
        Sample = 2,
        Release = 3,
        BiosSet = 4,
        Archive = 5,

        Blank = 99, // This is not a real type, only used internally
    }

    /// <summary>
    /// Determine what type of machine it is
    /// </summary>
    [Flags]
    public enum MachineType
    {
        /// <summary>
        /// This is a fake flag that is used for filter only
        /// </summary>
        NULL = 0x00,

        None = 1 << 0,
        Bios = 1 << 1,
        Device = 1 << 2,
        Mechanical = 1 << 3,
    }
}
