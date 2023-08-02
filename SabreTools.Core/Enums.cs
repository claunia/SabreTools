using System;

// TODO: Figure out the best way to put these back in their libraries
namespace SabreTools.Core
{
    #region DatFiles

    /// <summary>
    /// Available hashing types
    /// </summary>
    [Flags]
    public enum Hash
    {
        CRC = 1 << 0,
        MD5 = 1 << 1,
        SHA1 = 1 << 2,
        SHA256 = 1 << 3,
        SHA384 = 1 << 4,
        SHA512 = 1 << 5,
        SpamSum = 1 << 6,

        // Special combinations
        Standard = CRC | MD5 | SHA1,
        DeepHashes = SHA256 | SHA384 | SHA512 | SpamSum,
        SecureHashes = MD5 | SHA1 | SHA256 | SHA384 | SHA512 | SpamSum,
        All = CRC | MD5 | SHA1 | SHA256 | SHA384 | SHA512 | SpamSum,
    }

    /// <summary>
    /// Determines merging tag handling for DAT output
    /// </summary>
    public enum MergingFlag
    {
        [Mapping("none")]
        None = 0,

        [Mapping("split")]
        Split,

        [Mapping("merged")]
        Merged,

        [Mapping("nonmerged", "unmerged")]
        NonMerged,

        /// <remarks>This is not usually defined for Merging flags</remarks>
        [Mapping("fullmerged")]
        FullMerged,

        /// <remarks>This is not usually defined for Merging flags</remarks>
        [Mapping("device", "deviceunmerged", "devicenonmerged")]
        DeviceNonMerged,

        /// <remarks>This is not usually defined for Merging flags</remarks>
        [Mapping("full", "fullunmerged", "fullnonmerged")]
        FullNonMerged,
    }

    /// <summary>
    /// Determines nodump tag handling for DAT output
    /// </summary>
    public enum NodumpFlag
    {
        [Mapping("none")]
        None = 0,

        [Mapping("obsolete")]
        Obsolete,

        [Mapping("required")]
        Required,

        [Mapping("ignore")]
        Ignore,
    }

    /// <summary>
    /// Determines packing tag handling for DAT output
    /// </summary>
    public enum PackingFlag
    {
        [Mapping("none")]
        None = 0,

        /// <summary>
        /// Force all sets to be in archives, except disk and media
        /// </summary>
        [Mapping("zip", "yes")]
        Zip,

        /// <summary>
        /// Force all sets to be extracted into subfolders
        /// </summary>
        [Mapping("unzip", "no")]
        Unzip,

        /// <summary>
        /// Force sets with single items to be extracted to the parent folder
        /// </summary>
        [Mapping("partial")]
        Partial,

        /// <summary>
        /// Force all sets to be extracted to the parent folder
        /// </summary>
        [Mapping("flat")]
        Flat,
    }

    #endregion

    #region DatItems

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

        [Mapping("cpu")]
        CPU = 1 << 0,

        [Mapping("audio")]
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

        [Mapping("joy")]
        Joy = 1 << 0,

        [Mapping("stick")]
        Stick = 1 << 1,

        [Mapping("paddle")]
        Paddle = 1 << 2,

        [Mapping("pedal")]
        Pedal = 1 << 3,

        [Mapping("lightgun")]
        Lightgun = 1 << 4,

        [Mapping("positional")]
        Positional = 1 << 5,

        [Mapping("dial")]
        Dial = 1 << 6,

        [Mapping("trackball")]
        Trackball = 1 << 7,

        [Mapping("mouse")]
        Mouse = 1 << 8,

        [Mapping("only_buttons")]
        OnlyButtons = 1 << 9,

        [Mapping("keypad")]
        Keypad = 1 << 10,

        [Mapping("keyboard")]
        Keyboard = 1 << 11,

        [Mapping("mahjong")]
        Mahjong = 1 << 12,

        [Mapping("hanafuda")]
        Hanafuda = 1 << 13,

        [Mapping("gambling")]
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

        [Mapping("unknown")]
        Unknown = 1 << 0,

        [Mapping("cartridge")]
        Cartridge = 1 << 1,

        [Mapping("floppydisk")]
        FloppyDisk = 1 << 2,

        [Mapping("harddisk")]
        HardDisk = 1 << 3,

        [Mapping("cylinder")]
        Cylinder = 1 << 4,

        [Mapping("cassette")]
        Cassette = 1 << 5,

        [Mapping("punchcard")]
        PunchCard = 1 << 6,

        [Mapping("punchtape")]
        PunchTape = 1 << 7,

        [Mapping("printout")]
        Printout = 1 << 8,

        [Mapping("serial")]
        Serial = 1 << 9,

        [Mapping("parallel")]
        Parallel = 1 << 10,

        [Mapping("snapshot")]
        Snapshot = 1 << 11,

        [Mapping("quickload")]
        QuickLoad = 1 << 12,

        [Mapping("memcard")]
        MemCard = 1 << 13,

        [Mapping("cdrom")]
        CDROM = 1 << 14,

        [Mapping("magtape")]
        MagTape = 1 << 15,

        [Mapping("romimage")]
        ROMImage = 1 << 16,

        [Mapping("midiin")]
        MIDIIn = 1 << 17,

        [Mapping("midiout")]
        MIDIOut = 1 << 18,

        [Mapping("picture")]
        Picture = 1 << 19,

        [Mapping("vidfile")]
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

        [Mapping("raster")]
        Raster = 1 << 0,

        [Mapping("vector")]
        Vector = 1 << 1,

        [Mapping("lcd")]
        LCD = 1 << 2,

        [Mapping("svg")]
        SVG = 1 << 3,

        [Mapping("unknown")]
        Unknown = 1 << 4,
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

        [Mapping("big")]
        Big = 1 << 0,

        [Mapping("little")]
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

        [Mapping("unemulated")]
        Unemulated = 1 << 0,

        [Mapping("imperfect")]
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

        [Mapping("protection")]
        Protection = 1 << 0,

        [Mapping("palette")]
        Palette = 1 << 1,

        [Mapping("graphics")]
        Graphics = 1 << 2,

        [Mapping("sound")]
        Sound = 1 << 3,

        [Mapping("controls")]
        Controls = 1 << 4,

        [Mapping("keyboard")]
        Keyboard = 1 << 5,

        [Mapping("mouse")]
        Mouse = 1 << 6,

        [Mapping("microphone")]
        Microphone = 1 << 7,

        [Mapping("camera")]
        Camera = 1 << 8,

        [Mapping("disk")]
        Disk = 1 << 9,

        [Mapping("printer")]
        Printer = 1 << 10,

        [Mapping("lan")]
        Lan = 1 << 11,

        [Mapping("wan")]
        Wan = 1 << 12,

        [Mapping("timing")]
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

        [Mapping("none", "no")]
        None = 1 << 0,

        [Mapping("good")]
        Good = 1 << 1,

        [Mapping("baddump")]
        BadDump = 1 << 2,

        [Mapping("nodump", "yes")]
        Nodump = 1 << 3,

        [Mapping("verified")]
        Verified = 1 << 4,
    }

    /// <summary>
    /// Determine what type of file an item is
    /// </summary>
    public enum ItemType
    {
        /// <summary>
        /// This is a fake flag that is used for filter only
        /// </summary>
        NULL = 0,

        // "Actionable" item types

        [Mapping("rom")]
        Rom,

        [Mapping("disk")]
        Disk,

        [Mapping("file")]
        File,

        [Mapping("media")]
        Media,

        // "Auxiliary" item types

        [Mapping("adjuster")]
        Adjuster,

        [Mapping("analog")]
        Analog,

        [Mapping("archive")]
        Archive,

        [Mapping("biosset")]
        BiosSet,

        [Mapping("chip")]
        Chip,

        [Mapping("condition")]
        Condition,

        [Mapping("configuration")]
        Configuration,

        [Mapping("control")]
        Control,

        [Mapping("dataarea")]
        DataArea,

        [Mapping("device")]
        Device,

        [Mapping("device_ref", "deviceref")]
        DeviceReference,

        [Mapping("dipswitch")]
        DipSwitch,

        [Mapping("diskarea")]
        DiskArea,

        [Mapping("display")]
        Display,

        [Mapping("driver")]
        Driver,

        [Mapping("extension")]
        Extension,

        [Mapping("feature")]
        Feature,

        [Mapping("info")]
        Info,

        [Mapping("input")]
        Input,

        [Mapping("instance")]
        Instance,

        [Mapping("location")]
        Location,

        [Mapping("original")]
        Original,

        [Mapping("part")]
        Part,

        [Mapping("part_feature", "partfeature")]
        PartFeature,

        [Mapping("port")]
        Port,

        [Mapping("ramoption", "ram_option")]
        RamOption,

        [Mapping("release")]
        Release,

        [Mapping("release_details", "releasedetails")]
        ReleaseDetails,

        [Mapping("sample")]
        Sample,

        [Mapping("serials")]
        Serials,

        [Mapping("setting")]
        Setting,

        [Mapping("sharedfeat", "shared_feat", "sharedfeature", "shared_feature")]
        SharedFeature,

        [Mapping("slot")]
        Slot,

        [Mapping("slotoption", "slot_option")]
        SlotOption,

        [Mapping("softwarelist", "software_list")]
        SoftwareList,

        [Mapping("sound")]
        Sound,

        [Mapping("source_details", "sourcedetails")]
        SourceDetails,

        [Mapping("blank")]
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

        [Mapping("load16_byte")]
        Load16Byte = 1 << 0,

        [Mapping("load16_word")]
        Load16Word = 1 << 1,

        [Mapping("load16_word_swap")]
        Load16WordSwap = 1 << 2,

        [Mapping("load32_byte")]
        Load32Byte = 1 << 3,

        [Mapping("load32_word")]
        Load32Word = 1 << 4,

        [Mapping("load32_word_swap")]
        Load32WordSwap = 1 << 5,

        [Mapping("load32_dword")]
        Load32DWord = 1 << 6,

        [Mapping("load64_word")]
        Load64Word = 1 << 7,

        [Mapping("load64_word_swap")]
        Load64WordSwap = 1 << 8,

        [Mapping("reload")]
        Reload = 1 << 9,

        [Mapping("fill")]
        Fill = 1 << 10,

        [Mapping("continue")]
        Continue = 1 << 11,

        [Mapping("reload_plain")]
        ReloadPlain = 1 << 12,

        [Mapping("ignore")]
        Ignore = 1 << 13,
    }

    /// <summary>
    /// Determine what type of machine it is
    /// </summary>
    [Flags]
    public enum MachineType
    {
        [Mapping("none")]
        None = 0,

        [Mapping("bios")]
        Bios = 1 << 0,

        [Mapping("device", "dev")]
        Device = 1 << 1,

        [Mapping("mechanical", "mech")]
        Mechanical = 1 << 2,
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

        [Mapping("rom")]
        Rom = 1 << 0,

        [Mapping("megarom")]
        MegaRom = 1 << 1,

        [Mapping("sccpluscart")]
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

        [Mapping("eq")]
        Equal = 1 << 0,

        [Mapping("ne")]
        NotEqual = 1 << 1,

        [Mapping("gt")]
        GreaterThan = 1 << 2,

        [Mapping("le")]
        LessThanOrEqual = 1 << 3,

        [Mapping("lt")]
        LessThan = 1 << 4,

        [Mapping("ge")]
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

        [Mapping("no")]
        No = 1 << 0,

        [Mapping("partial")]
        Partial = 1 << 1,

        [Mapping("yes")]
        Yes = 1 << 2,
    }

    /// <summary>
    /// Determine software list status
    /// </summary>
    [Flags]
    public enum SoftwareListStatus
    {
        [Mapping("none")]
        None = 0,

        [Mapping("original")]
        Original = 1 << 0,

        [Mapping("compatible")]
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

        [Mapping("no", "unsupported")]
        No = 1 << 0,

        [Mapping("partial")]
        Partial = 1 << 1,

        [Mapping("yes", "supported")]
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

        [Mapping("good")]
        Good = 1 << 0,

        [Mapping("imperfect")]
        Imperfect = 1 << 1,

        [Mapping("preliminary")]
        Preliminary = 1 << 2,
    }

    #endregion

    #region Fields

    /// <summary>
    /// List of valid field types within a DatHeader
    /// </summary>
    public enum DatHeaderField
    {
        /// <summary>
        /// This is a fake flag that is used for filter only
        /// </summary>
        NULL = 0,

        /// <remarks>Used in ClrMamePro, DOSCenter, Logiqx, and RomCenter</remarks>
        [Mapping("author")]
        Author,

        /// <remarks>Used in Logiqx</remarks>
        [Mapping("biosmode", "bios_mode")]
        BiosMode,

        /// <remarks>Used in Logiqx</remarks>
        [Mapping("build")]
        Build,

        /// <remarks>Used with OfflineList</remarks>
        [Mapping("canopen", "can_open")]
        CanOpen,

        /// <remarks>Used in ClrMamePro and Logiqx</remarks>
        [Mapping("category")]
        Category,

        /// <remarks>Used in ClrMamePro, DOSCenter, Logiqx, and RomCenter</remarks>
        [Mapping("comment")]
        Comment,

        /// <remarks>Used in ClrMamePro, DOSCenter, Logiqx, OpenMSX, and RomCenter</remarks>
        [Mapping("date", "timestamp", "time_stamp")]
        Date,

        /// <remarks>Used in Logiqx and ListXML</remarks>
        [Mapping("debug")]
        Debug,

        /// <remarks>Used in ClrMamePro, DOSCenter, ListXML, Logiqx, OpenMSX, RomCenter, Separated Value, and Software List</remarks>
        [Mapping("desc", "description")]
        Description,

        /// <remarks>Used in ClrMamePro, Logiqx, and RomCenter</remarks>
        [Mapping("email", "e_mail")]
        Email,

        /// <remarks>Used in AttractMode, OfflineList, and Separated Value</remarks>
        [Mapping("file", "filename", "file_name")]
        FileName,

        /// <remarks>Used in ClrMamePro, Logiqx, and RomCenter</remarks>
        [Mapping("forcemerging", "force_merging")]
        ForceMerging,

        /// <remarks>Used in Logiqx</remarks>
        [Mapping("forcenodump", "force_nodump")]
        ForceNodump,

        /// <remarks>Used in ClrMamePro and Logiqx</remarks>
        [Mapping("forcepacking", "force_packing")]
        ForcePacking,

        /// <remarks>Used in ClrMamePro and Logiqx</remarks>
        [Mapping("header", "headerskipper", "header_skipper", "skipper")]
        HeaderSkipper,

        /// <remarks>Used in ClrMamePro, DOSCenter, Logiqx, and RomCenter</remarks>
        [Mapping("homepage", "home_page")]
        Homepage,

        /// <remarks>Used in Logiqx</remarks>
        [Mapping("id", "nointroid", "no_intro_id")]
        ID,

        /// <remarks>Used with OfflineList; Part of "Info" object</remarks>
        [Mapping("info_default", "infos_default")]
        Info_Default,

        /// <remarks>Used with OfflineList; Part of "Info" object</remarks>
        [Mapping("info_isnamingoption", "info_is_naming_option", "infos_isnamingoption", "infos_is_naming_option")]
        Info_IsNamingOption,

        /// <remarks>Used with OfflineList; Part of "Info" object</remarks>
        [Mapping("info_name", "infos_name")]
        Info_Name,

        /// <remarks>Used with OfflineList; Part of "Info" object</remarks>
        [Mapping("info_visible", "infos_visible")]
        Info_Visible,

        /// <remarks>Used in Logiqx</remarks>
        [Mapping("lockbiosmode", "lockbios_mode", "lock_biosmode", "lock_bios_mode")]
        LockBiosMode,

        /// <remarks>Used in Logiqx</remarks>
        [Mapping("lockrommode", "lockrom_mode", "lock_rommode", "lock_rom_mode")]
        LockRomMode,

        /// <remarks>Used in Logiqx</remarks>
        [Mapping("locksamplemode", "locksample_mode", "lock_samplemode", "lock_sample_mode")]
        LockSampleMode,

        /// <remarks>Used in ListXML</remarks>
        [Mapping("mameconfig", "mame_config")]
        MameConfig,

        /// <remarks>Used in ClrMamePro, DOSCenter, ListXML, Logiqx, OfflineList, OpenMSX, RomCenter, Separated Value, and Software List</remarks>
        [Mapping("dat", "datname", "dat_name", "internalname", "internal_name")]
        Name,

        /// <remarks>Used with RomCenter</remarks>
        [Mapping("rcversion", "rc_version", "romcenterversion", "romcenter_version", "rom_center_version")]
        RomCenterVersion,

        /// <remarks>Used in Logiqx</remarks>
        [Mapping("rommode", "rom_mode")]
        RomMode,

        /// <remarks>Used with OfflineList</remarks>
        [Mapping("romtitle", "rom_title")]
        RomTitle,

        /// <remarks>Used with ClrMamePro and Logiqx</remarks>
        [Mapping("root", "rootdir", "root_dir", "rootdirectory", "root_directory")]
        RootDir,

        /// <remarks>Used in Logiqx</remarks>
        [Mapping("samplemode", "sample_mode")]
        SampleMode,

        /// <remarks>Used with OfflineList</remarks>
        [Mapping("screenshotheight", "screenshotsheight", "screenshot_height", "screenshots_height")]
        ScreenshotsHeight,

        /// <remarks>Used with OfflineList</remarks>
        [Mapping("screenshotwidth", "screenshotswidth", "screenshot_width", "screenshots_width")]
        ScreenshotsWidth,

        /// <remarks>Used with Logiqx, OfflineList, and RomCenter; "plugin" is used for RomCenter</remarks>
        [Mapping("system", "plugin")]
        System,

        /// <remarks>Used with ClrMamePro, Logiqx, and OfflineList</remarks>
        [Mapping("dattype", "type", "superdat")]
        Type,

        /// <remarks>Used with ClrMamePro, Logiqx, OfflineList, and RomCenter</remarks>
        [Mapping("url")]
        Url,

        /// <remarks>Used with ClrMamePro, DOSCenter, ListXML, Logiqx, OfflineList, and RomCenter</remarks>
        [Mapping("version")]
        Version,
    }

    /// <summary>
    /// List of valid field types within a DatItem
    /// </summary>
    public enum DatItemField
    {
        /// <summary>
        /// This is a fake flag that is used for filter only
        /// </summary>
        NULL = 0,

        /// <remarks>Used in Rom</remarks>
        [Mapping("altname", "alt_name", "altromname", "alt_romname", "alt_rom_name")]
        AltName,

        /// <remarks>Used in Rom</remarks>
        [Mapping("alttitle", "alt_title", "altromtitle", "alt_romtitle", "alt_rom_title")]
        AltTitle,

        /// <remarks>Used in Analog</remarks>
        [Mapping("analog_mask")]
        Analog_Mask,

        /// <remarks>Used in Rom</remarks>
        [Mapping("ado_format")]
        ArchiveDotOrgFormat,

        /// <remarks>Used in Rom</remarks>
        [Mapping("ado_source")]
        ArchiveDotOrgSource,

        /// <remarks>Used in DataArea</remarks>
        [Mapping("areaendinanness", "area_endianness")]
        AreaEndianness,

        /// <remarks>Used in DataArea and DiskArea</remarks>
        [Mapping("areaname", "area_name")]
        AreaName,

        /// <remarks>Used in DataArea</remarks>
        [Mapping("areasize", "area_size")]
        AreaSize,

        /// <remarks>Used in DataArea</remarks>
        [Mapping("areawidth", "area_width")]
        AreaWidth,

        /// <remarks>Used in Rom</remarks>
        [Mapping("bios")]
        Bios,

        /// <remarks>Used in Rom</remarks>
        [Mapping("boot")]
        Boot,

        /// <remarks>Used in Archive</remarks>
        [Mapping("categories")]
        Categories,

        /// <remarks>Used in Sound</remarks>
        [Mapping("channels")]
        Channels,

        /// <remarks>Used in Chip</remarks>
        [Mapping("chiptype", "chip_type")]
        ChipType,

        /// <remarks>Used in Chip</remarks>
        [Mapping("clock")]
        Clock,

        /// <remarks>Used in Archive</remarks>
        [Mapping("clone")]
        Clone,

        /// <remarks>Used in Driver</remarks>
        [Mapping("cocktailstatus", "cocktail_status")]
        CocktailStatus,

        /// <remarks>Used in Input</remarks>
        [Mapping("coins")]
        Coins,

        /// <remarks>Used in Archive</remarks>
        [Mapping("complete")]
        Complete,

        /// <remarks>Used in Condition</remarks>
        [Mapping("condition_mask")]
        Condition_Mask,

        /// <remarks>Used in Condition</remarks>
        [Mapping("condition_relation")]
        Condition_Relation,

        /// <remarks>Used in Condition</remarks>
        [Mapping("condition_tag")]
        Condition_Tag,

        /// <remarks>Used in Condition</remarks>
        [Mapping("condition_value")]
        Condition_Value,

        /// <remarks>Used in RamOption</remarks>
        [Mapping("content")]
        Content,

        /// <remarks>Used in Control</remarks>
        [Mapping("control_buttons")]
        Control_Buttons,

        /// <remarks>Used in Control</remarks>
        [Mapping("control_keydelta", "control_key_delta")]
        Control_KeyDelta,

        /// <remarks>Used in Control</remarks>
        [Mapping("control_maximum")]
        Control_Maximum,

        /// <remarks>Used in Control</remarks>
        [Mapping("control_minimum")]
        Control_Minimum,

        /// <remarks>Used in Control</remarks>
        [Mapping("control_player")]
        Control_Player,

        /// <remarks>Used in Control</remarks>
        [Mapping("control_reqbuttons", "control_req_buttons")]
        Control_RequiredButtons,

        /// <remarks>Used in Control</remarks>
        [Mapping("control_reverse")]
        Control_Reverse,

        /// <remarks>Used in Control</remarks>
        [Mapping("control_sensitivity")]
        Control_Sensitivity,

        /// <remarks>Used in Control</remarks>
        [Mapping("control_type")]
        Control_Type,

        /// <remarks>Used in Control</remarks>
        [Mapping("control_ways")]
        Control_Ways,

        /// <remarks>Used in Control</remarks>
        [Mapping("control_ways2")]
        Control_Ways2,

        /// <remarks>Used in Control</remarks>
        [Mapping("control_ways3")]
        Control_Ways3,

        /// <remarks>Used in Rom</remarks>
        [Mapping("crc", "crc32")]
        CRC,

        /// <remarks>Used in Release and Rom</remarks>
        [Mapping("date")]
        Date,

        /// <remarks>Used in Adjuster, BiosSet, RamOption, and Release</remarks>
        [Mapping("default")]
        Default,

        /// <remarks>Used in BiosSet</remarks>
        [Mapping("description", "biosdescription", "bios_description")]
        Description,

        /// <remarks>Used in Device</remarks>
        [Mapping("devicetype", "device_type")]
        DeviceType,

        /// <remarks>Used in Archive</remarks>
        [Mapping("devstatus", "dev_status")]
        DevStatus,

        /// <remarks>Used in Display</remarks>
        [Mapping("displaytype", "display_type")]
        DisplayType,

        /// <remarks>Used in Driver</remarks>
        [Mapping("emulationstatus", "emulation_status")]
        EmulationStatus,

        /// <remarks>Used in Extension</remarks>
        [Mapping("extension_name")]
        Extension_Name,

        /// <remarks>Used in Feature</remarks>
        [Mapping("featureoverall", "feature_overall")]
        FeatureOverall,

        /// <remarks>Used in Feature</remarks>
        [Mapping("featurestatus", "feature_status")]
        FeatureStatus,

        /// <remarks>Used in Feature</remarks>
        [Mapping("featuretype", "feature_type")]
        FeatureType,

        /// <remarks>Used in SoftwareList</remarks>
        [Mapping("filter")]
        Filter,

        /// <remarks>Used in Device</remarks>
        [Mapping("fixedimage", "fixed_image")]
        FixedImage,

        /// <remarks>Used in Display</remarks>
        [Mapping("flipx")]
        FlipX,

        /// <remarks>Used in Display</remarks>
        [Mapping("hbend")]
        HBEnd,

        /// <remarks>Used in Display</remarks>
        [Mapping("hbstart")]
        HBStart,

        /// <remarks>Used in Display</remarks>
        [Mapping("height")]
        Height,

        /// <remarks>Used in Display</remarks>
        [Mapping("htotal")]
        HTotal,

        /// <remarks>Used in Driver</remarks>
        [Mapping("incomplete")]
        Incomplete,

        /// <remarks>Used in Disk</remarks>
        [Mapping("index")]
        Index,

        /// <remarks>Used in Instance</remarks>
        [Mapping("instance_briefname", "instance_brief_name")]
        Instance_BriefName,

        /// <remarks>Used in Instance</remarks>
        [Mapping("instance_name")]
        Instance_Name,

        /// <remarks>Used in Device</remarks>
        [Mapping("interface")]
        Interface,

        /// <remarks>Used in Rom</remarks>
        [Mapping("inverted")]
        Inverted,

        /// <remarks>Used in Release</remarks>
        [Mapping("language")]
        Language,

        /// <remarks>Used in Archive</remarks>
        [Mapping("languages")]
        Languages,

        /// <remarks>Used in Rom</remarks>
        [Mapping("loadflag", "load_flag")]
        LoadFlag,

        /// <remarks>Used in Location</remarks>
        [Mapping("location_inverted")]
        Location_Inverted,

        /// <remarks>Used in Location</remarks>
        [Mapping("location_name")]
        Location_Name,

        /// <remarks>Used in Location</remarks>
        [Mapping("location_number")]
        Location_Number,

        /// <remarks>Used in Device</remarks>
        [Mapping("mandatory")]
        Mandatory,

        /// <remarks>Used in Condition, Configuration, and DipSwitch</remarks>
        [Mapping("mask")]
        Mask,

        /// <remarks>Used in Disk, Media, and Rom</remarks>
        [Mapping("md5", "md5hash", "md5_hash")]
        MD5,

        /// <remarks>Used in Disk and Rom</remarks>
        [Mapping("merge", "mergetag", "merge_tag")]
        Merge,

        /// <remarks>Used in Rom</remarks>
        [Mapping("mia")]
        MIA,

        /// <remarks>Used in Adjuster, Archive, BiosSet, Chip, Configuration, DataArea, DeviceReference, DipSwitch, Disk, DiskArea, Extension, Info, Instance, Location, Media, Part, PartFeature, RamOption, Release, Rom, Sample, Setting, SharedFeature, Slot, SlotOption, and SoftwareList</remarks>
        [Mapping("name")]
        Name,

        /// <remarks>Used in Driver</remarks>
        [Mapping("nosoundhardware", "no_sound_hardware")]
        NoSoundHardware,

        /// <remarks>Used in Archive</remarks>
        [Mapping("number")]
        Number,

        /// <remarks>Used in Rom</remarks>
        [Mapping("offset")]
        Offset,

        /// <remarks>Used in Rom</remarks>
        [Mapping("subtype", "sub_type", "openmsxsubtype", "openmsx_subtype", "openmsx_sub_type")]
        OpenMSXSubType,

        /// <remarks>Used in Rom</remarks>
        [Mapping("openmsxtype", "openmsx_type")]
        OpenMSXType,

        /// <remarks>Used in Disk and Rom</remarks>
        [Mapping("optional")]
        Optional,

        /// <remarks>Used in Rom</remarks>
        [Mapping("original")]
        Original,

        /// <remarks>Used in Rom</remarks>
        [Mapping("original_filename")]
        OriginalFilename,

        /// <remarks>Used in PartFeature</remarks>
        [Mapping("part_feature_name")]
        Part_Feature_Name,

        /// <remarks>Used in PartFeature</remarks>
        [Mapping("part_feature_value")]
        Part_Feature_Value,

        /// <remarks>Used in Part</remarks>
        [Mapping("partinterface", "part_interface")]
        Part_Interface,

        /// <remarks>Used in Part</remarks>
        [Mapping("partname", "part_name")]
        Part_Name,

        /// <remarks>Used in Archive</remarks>
        [Mapping("physical")]
        Physical,

        /// <remarks>Used in Display</remarks>
        [Mapping("pixclock", "pix_clock")]
        PixClock,

        /// <remarks>Used in Input</remarks>
        [Mapping("players")]
        Players,

        /// <remarks>Used in Display</remarks>
        [Mapping("refresh")]
        Refresh,

        /// <remarks>Used in Archive, Disk, Release, and Rom</remarks>
        [Mapping("region")]
        Region,

        /// <remarks>Used in Archive</remarks>
        [Mapping("regparent", "reg_parent")]
        RegParent,

        /// <remarks>Used in Condition</remarks>
        [Mapping("relation")]
        Relation,

        /// <remarks>Used in Rom</remarks>
        [Mapping("remark")]
        Remark,

        /// <remarks>Used in Driver</remarks>
        [Mapping("requiresartwork", "requires_artwork")]
        RequiresArtwork,

        /// <remarks>Used in Display</remarks>
        [Mapping("rotate")]
        Rotate,

        /// <remarks>Used in Rom</remarks>
        [Mapping("rotation")]
        Rotation,

        /// <remarks>Used in Driver</remarks>
        [Mapping("savestatestatus", "savestate_status", "save_state_status")]
        SaveStateStatus,

        /// <remarks>Used in Input</remarks>
        [Mapping("service")]
        Service,

        /// <remarks>Used in Setting</remarks>
        [Mapping("setting_default", "value_default")]
        Setting_Default,

        /// <remarks>Used in Setting</remarks>
        [Mapping("setting_name", "value_name")]
        Setting_Name,

        /// <remarks>Used in Setting</remarks>
        [Mapping("setting_value", "value_value")]
        Setting_Value,

        /// <remarks>Used in Disk, Media, and Rom</remarks>
        [Mapping("sha1", "sha_1", "sha1hash", "sha1_hash", "sha_1hash", "sha_1_hash")]
        SHA1,

        /// <remarks>Used in Media and Rom</remarks>
        [Mapping("sha256", "sha_256", "sha256hash", "sha256_hash", "sha_256hash", "sha_256_hash")]
        SHA256,

        /// <remarks>Used in Rom</remarks>
        [Mapping("sha384", "sha_384", "sha384hash", "sha384_hash", "sha_384hash", "sha_384_hash")]
        SHA384,

        /// <remarks>Used in Rom</remarks>
        [Mapping("sha512", "sha_512", "sha512hash", "sha512_hash", "sha_512hash", "sha_512_hash")]
        SHA512,

        /// <remarks>Used in Rom</remarks>
        [Mapping("size")]
        Size,

        /// <remarks>Used in SlotOption</remarks>
        [Mapping("slotoption_default")]
        SlotOption_Default,

        /// <remarks>Used in SlotOption</remarks>
        [Mapping("slotoption_devicename", "slotoption_device_name")]
        SlotOption_DeviceName,

        /// <remarks>Used in SlotOption</remarks>
        [Mapping("slotoption_name")]
        SlotOption_Name,

        /// <remarks>Used in SoftwareList</remarks>
        [Mapping("softwareliststatus", "softwarelist_status")]
        SoftwareListStatus,

        /// <remarks>Used in Media and Rom</remarks>
        [Mapping("spamsum", "spam_sum")]
        SpamSum,

        /// <remarks>Used in Disk and Rom</remarks>
        [Mapping("status")]
        Status,

        /// <remarks>Used in Rom</remarks>
        [Mapping("summation")]
        Summation,

        /// <remarks>Used in Driver</remarks>
        [Mapping("supportstatus", "support_status")]
        SupportStatus,

        /// <remarks>Used in Chip, Condition, Configuration, Device, DipSwitch, Display, Port, and SoftwareList</remarks>
        [Mapping("tag")]
        Tag,

        /// <remarks>Used in Input</remarks>
        [Mapping("tilt")]
        Tilt,

        /// <remarks>Internal value, common to all DatItems</remarks>
        [Mapping("type")]
        Type,

        /// <remarks>Used in Driver</remarks>
        [Mapping("unofficial")]
        Unofficial,

        /// <remarks>Used in Condition, Info, Rom, and SharedFeature</remarks>
        [Mapping("value")]
        Value,

        /// <remarks>Used in Display</remarks>
        [Mapping("vbend")]
        VBEnd,

        /// <remarks>Used in Display</remarks>
        [Mapping("vbstart")]
        VBStart,

        /// <remarks>Used in Display</remarks>
        [Mapping("vtotal")]
        VTotal,

        /// <remarks>Used in Display</remarks>
        [Mapping("width")]
        Width,

        /// <remarks>Used in Disk</remarks>
        [Mapping("writable")]
        Writable,
    }

    /// <summary>
    /// List of valid field types within a Machine
    /// </summary>
    public enum MachineField
    {
        /// <summary>
        /// This is a fake flag that is used for filter only
        /// </summary>
        NULL = 0,

        /// <remarks>Used in Logiqx</remarks>
        [Mapping("board")]
        Board,

        /// <remarks>Used in AttractMode</remarks>
        [Mapping("buttons")]
        Buttons,

        /// <remarks>Used in AttractMode, ClrMamePro, and Logiqx</remarks>
        [Mapping("category")]
        Category,

        /// <remarks>Used in AttractMode, ClrMamePro, ListXML, Logiqx, OfflineList, RomCenter, and Software List</remarks>
        [Mapping("cloneof", "clone_of")]
        CloneOf,

        /// <remarks>Used in Logiqx</remarks>
        [Mapping("cloneofid", "nointrocloneofid", "nointro_cloneofid", "no_intro_cloneofid", "no_intro_clone_of_id")]
        CloneOfID,

        /// <remarks>Used in AttractMode, Logiqx, and OfflineList; "extra" is used with AttractMode</remarks>
        [Mapping("comment", "extra")]
        Comment,

        /// <remarks>Used in AttractMode</remarks>
        [Mapping("control")]
        Control,

        /// <remarks>Used in OpenMSX</remarks>
        [Mapping("country")]
        Country,

        /// <remarks>Used in Logiqx (EmuArc Extension)</remarks>
        [Mapping("crc", "hascrc", "has_crc")]
        CRC,

        /// <remarks>Used in ArchiveDotOrg, AttractMode, ClrMamePro, DOSCenter, Everdrive SMDB, ListXML, Logiqx, RomCenter, Separated Value, and Software List</remarks>
        [Mapping("desc", "description")]
        Description,

        /// <remarks>Used in Logiqx (EmuArc Extension)</remarks>
        [Mapping("developer")]
        Developer,

        /// <remarks>Used in AttractMode</remarks>
        [Mapping("displaycount", "display_count")]
        DisplayCount,

        /// <remarks>Used in AttractMode</remarks>
        [Mapping("displaytype", "display_type")]
        DisplayType,

        /// <remarks>Used in Logiqx (EmuArc Extension)</remarks>
        [Mapping("enabled")]
        Enabled,

        /// <remarks>Used in OpenMSX</remarks>
        [Mapping("genmsxid", "genmsx_id", "gen_msxid", "gen_msx_id")]
        GenMSXID,

        /// <remarks>Used in Logiqx (EmuArc Extension)</remarks>
        [Mapping("genre")]
        Genre,

        /// <remarks>Used in ListXML</remarks>
        [Mapping("history")]
        History,

        /// <remarks>Used in Logiqx</remarks>
        [Mapping("id", "nointroid", "nointro_id", "no_intro_id")]
        ID,

        /// <remarks>Used in AttractMode, ClrMamePro, ListXML, Logiqx, and OpenMSX</remarks>
        [Mapping("manufacturer")]
        Manufacturer,

        /// <remarks>Used in ArchiveDotOrg, AttractMode, ClrMamePro, DOSCenter, Everdrive SMDB, Hashfile, ListROM, ListXML, Logiqx, Missfile, OfflineList, OpenMSX, RomCenter, Separated Value, and Software List</remarks>
        [Mapping("name")]
        Name,

        /// <remarks>Used in AttractMode and Logiqx</remarks>
        [Mapping("players")]
        Players,

        /// <remarks>Used in Logiqx, OfflineList, and Software List</remarks>
        [Mapping("publisher")]
        Publisher,

        /// <remarks>Used in Logiqx (EmuArc Extension)</remarks>
        [Mapping("ratings")]
        Ratings,

        /// <remarks>Used in Logiqx</remarks>
        [Mapping("rebuildto", "rebuild_to")]
        RebuildTo,

        /// <remarks>Used in Logiqx (EmuArc Extension)</remarks>
        [Mapping("relatedto", "related_to")]
        RelatedTo,

        /// <remarks>Used in ClrMamePro, ListXML, Logiqx, and RomCenter</remarks>
        [Mapping("romof", "rom_of")]
        RomOf,

        /// <remarks>Used in AttractMode</remarks>
        [Mapping("rotation")]
        Rotation,

        /// <remarks>Used in ListXML and Logiqx</remarks>
        [Mapping("runnable")]
        Runnable,

        /// <remarks>Used in ClrMamePro, ListXML, and Logiqx</remarks>
        [Mapping("sampleof", "sample_of")]
        SampleOf,

        /// <remarks>Used in Logiqx (EmuArc Extension)</remarks>
        [Mapping("score")]
        Score,

        /// <remarks>Used in ListXML and Logiqx</remarks>
        [Mapping("sourcefile", "source_file")]
        SourceFile,

        /// <remarks>Used in AttractMode</remarks>
        [Mapping("amstatus", "am_status", "gamestatus", "supportstatus", "support_status")]
        Status,

        /// <remarks>Used in Logiqx (EmuArc Extension)</remarks>
        [Mapping("subgenre", "sub_genre")]
        Subgenre,

        /// <remarks>Used in Software List</remarks>
        [Mapping("supported")]
        Supported,

        /// <remarks>Used in OpenMSX</remarks>
        [Mapping("system", "msxsystem", "msx_system")]
        System,

        /// <remarks>Used in Logiqx (EmuArc Extension)</remarks>
        [Mapping("titleid", "title_id")]
        TitleID,

        /// <remarks>Used in ClrMamePro, DOSCenter, ListXML, and Logiqx</remarks>
        [Mapping("type")]
        Type,

        /// <remarks>Used in AttractMode, ClrMamePro, ListXML, Logiqx, OpenMSX, and Software List</remarks>
        [Mapping("year")]
        Year,
    }

#endregion

    #region Logging

    /// <summary>
    /// Severity of the logging statement
    /// </summary>
    public enum LogLevel
    {
        [Mapping("verbose")]
        VERBOSE = 0,

        [Mapping("user")]
        USER,

        [Mapping("warning")]
        WARNING,

        [Mapping("error")]
        ERROR,
    }

    #endregion
}
