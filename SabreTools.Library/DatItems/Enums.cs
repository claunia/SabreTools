using System;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Determine the chip type
    /// </summary>
    [Flags]
    public enum ChipType
    {
        /// <summary>
        /// This is a fake flag that is used for filter only
        /// </summary>
        NULL = 0,

        CPU = 1 << 0,
        Audio = 1 << 1,
    }

    /// <summary>
    /// Determine the control type
    /// </summary>
    [Flags]
    public enum ControlType
    {
        /// <summary>
        /// This is a fake flag that is used for filter only
        /// </summary>
        NULL = 0,

        Joy = 1 << 0,
        Stick = 1 << 1,
        Paddle = 1 << 2,
        Pedal = 1 << 3,
        Lightgun = 1 << 4,
        Positional = 1 << 5,
        Dial = 1 << 6,
        Trackball = 1 << 7,
        Mouse = 1 << 8,
        OnlyButtons = 1 << 9,
        Keypad = 1 << 10,
        Keyboard = 1 << 11,
        Mahjong = 1 << 12,
        Hanafuda = 1 << 13,
        Gambling = 1 << 14,
    }

    /// <summary>
    /// Determine the device type
    /// </summary>
    [Flags]
    public enum DeviceType
    {
        /// <summary>
        /// This is a fake flag that is used for filter only
        /// </summary>
        NULL = 0,

        Unknown = 1 << 0,
        Cartridge = 1 << 1,
        FloppyDisk = 1 << 2,
        HardDisk = 1 << 3,
        Cylinder = 1 << 4,
        Cassette = 1 << 5,
        PunchCard = 1 << 6,
        PunchTape = 1 << 7,
        Printout = 1 << 8,
        Serial = 1 << 9,
        Parallel = 1 << 10,
        Snapshot = 1 << 11,
        QuickLoad = 1 << 12,
        MemCard = 1 << 13,
        CDROM = 1 << 14,
        MagTape = 1 << 15,
        ROMImage = 1 << 16,
        MIDIIn = 1 << 17,
        MIDIOut = 1 << 18,
        Picture = 1 << 19,
        VidFile = 1 << 20,
    }

    /// <summary>
    /// Determine the display type
    /// </summary>
    [Flags]
    public enum DisplayType
    {
        /// <summary>
        /// This is a fake flag that is used for filter only
        /// </summary>
        NULL = 0,

        Raster = 1 << 0,
        Vector = 1 << 1,
        LCD = 1 << 2,
        SVG = 1 << 3,
        Unknown = 1 << 4,
    }

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
    /// Determine the endianness
    /// </summary>
    [Flags]
    public enum Endianness
    {
        /// <summary>
        /// This is a fake flag that is used for filter only
        /// </summary>
        NULL = 0,

        Big = 1 << 0,
        Little = 1 << 1,
    }

    /// <summary>
    /// Determine the emulation status
    /// </summary>
    [Flags]
    public enum FeatureStatus
    {
        /// <summary>
        /// This is a fake flag that is used for filter only
        /// </summary>
        NULL = 0,

        Unemulated = 1 << 0,
        Imperfect = 1 << 1,
    }

    /// <summary>
    /// Determine the feature type
    /// </summary>
    [Flags]
    public enum FeatureType
    {
        /// <summary>
        /// This is a fake flag that is used for filter only
        /// </summary>
        NULL = 0,

        Protection = 1 << 0,
        Palette = 1 << 1,
        Graphics = 1 << 2,
        Sound = 1 << 3,
        Controls = 1 << 4,
        Keyboard = 1 << 5,
        Mouse = 1 << 6,
        Microphone = 1 << 7,
        Camera = 1 << 8,
        Disk = 1 << 9,
        Printer = 1 << 10,
        Lan = 1 << 11,
        Wan = 1 << 12,
        Timing = 1 << 13,
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
        NULL = 0,

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
        // "Actionable" item types
        Rom,
        Disk,
        Media,

        // "Auxiliary" item types
        Adjuster,
        Analog,
        Archive,
        BiosSet,
        Chip,
        Condition,
        Configuration,
        Control,
        DataArea,
        Device,
        DeviceReference,
        DipSwitch,
        DiskArea,
        Display,
        Driver,
        Extension,
        Feature,
        Info,
        Input,
        Instance,
        Location,
        Part,
        PartFeature,
        Port,
        RamOption,
        Release,
        Sample,
        Setting,
        SharedFeature,
        Slot,
        SlotOption,
        SoftwareList,
        Sound,

        Blank = 99, // This is not a real type, only used internally
    }

    /// <summary>
    /// Determine the loadflag value
    /// </summary>
    [Flags]
    public enum LoadFlag
    {
        /// <summary>
        /// This is a fake flag that is used for filter only
        /// </summary>
        NULL = 0,

        Load16Byte = 1 << 0,
        Load16Word = 1 << 1,
        Load16WordSwap = 1 << 2,
        Load32Byte = 1 << 3,
        Load32Word = 1 << 4,
        Load32WordSwap = 1 << 5,
        Load32DWord = 1 << 6,
        Load64Word = 1 << 7,
        Load64WordSwap = 1 << 8,
        Reload = 1 << 9,
        Fill = 1 << 10,
        Continue = 1 << 11,
        ReloadPlain = 1 << 12,
        Ignore = 1 << 13,
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
        NULL = 1 << 0,

        Bios = 1 << 1,
        Device = 1 << 2,
        Mechanical = 1 << 3,
    }

    /// <summary>
    /// Determine which OpenMSX subtype an item is
    /// </summary>
    [Flags]
    public enum OpenMSXSubType
    {
        /// <summary>
        /// This is a fake flag that is used for filter only
        /// </summary>
        NULL = 0,

        Rom = 1 << 0,
        MegaRom = 1 << 1,
        SCCPlusCart = 1 << 2,
    }

    /// <summary>
    /// Determine relation of value to condition
    /// </summary>
    [Flags]
    public enum Relation
    {
        /// <summary>
        /// This is a fake flag that is used for filter only
        /// </summary>
        NULL = 0,

        Equal = 1 << 0,
        NotEqual = 1 << 1,
        GreaterThan = 1 << 2,
        LessThanOrEqual = 1 << 3,
        LessThan = 1 << 4,
        GreaterThanOrEqual = 1 << 5,
    }

    /// <summary>
    /// Determine machine runnable status
    /// </summary>
    [Flags]
    public enum Runnable
    {
        /// <summary>
        /// This is a fake flag that is used for filter only
        /// </summary>
        NULL = 0,

        No = 1 << 0,
        Partial = 1 << 1,
        Yes = 1 << 2,
    }

    /// <summary>
    /// Determine software list status
    /// </summary>
    [Flags]
    public enum SoftwareListStatus
    {
        /// <summary>
        /// This is a fake flag that is used for filter only
        /// </summary>
        NULL = 0,

        Original = 1 << 0,
        Compatible = 1 << 1,
    }

    /// <summary>
    /// Determine machine support status
    /// </summary>
    [Flags]
    public enum Supported
    {
        /// <summary>
        /// This is a fake flag that is used for filter only
        /// </summary>
        NULL = 0,

        No = 1 << 0,
        Partial = 1 << 1,
        Yes = 1 << 2,
    }

    /// <summary>
    /// Determine driver support statuses
    /// </summary>
    [Flags]
    public enum SupportStatus
    {
        /// <summary>
        /// This is a fake flag that is used for filter only
        /// </summary>
        NULL = 0,

        Good = 1 << 0,
        Imperfect = 1 << 1,
        Preliminary = 1 << 2,
    }
}
