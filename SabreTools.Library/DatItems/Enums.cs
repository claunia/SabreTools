namespace SabreTools.Library.DatItems
{
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
}
