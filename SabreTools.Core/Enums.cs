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

        #region Common

        [Mapping("file", "filename", "file_name")]
        FileName,

        [Mapping("dat", "datname", "dat_name", "internalname", "internal_name")]
        Name,

        [Mapping("desc", "description")]
        Description,

        [Mapping("root", "rootdir", "root_dir", "rootdirectory", "root_directory")]
        RootDir,

        [Mapping("category")]
        Category,

        [Mapping("version")]
        Version,

        [Mapping("date", "timestamp", "time_stamp")]
        Date,

        [Mapping("author")]
        Author,

        [Mapping("email", "e_mail")]
        Email,

        [Mapping("homepage", "home_page")]
        Homepage,

        [Mapping("url")]
        Url,

        [Mapping("comment")]
        Comment,

        [Mapping("header", "headerskipper", "header_skipper", "skipper")]
        HeaderSkipper,

        [Mapping("dattype", "type", "superdat")]
        Type,

        [Mapping("forcemerging", "force_merging")]
        ForceMerging,

        [Mapping("forcenodump", "force_nodump")]
        ForceNodump,

        [Mapping("forcepacking", "force_packing")]
        ForcePacking,

        #endregion

        #region ListXML

        [Mapping("debug")]
        Debug,

        [Mapping("mameconfig", "mame_config")]
        MameConfig,

        #endregion

        #region Logiqx

        [Mapping("id", "nointroid", "no_intro_id")]
        NoIntroID,

        [Mapping("build")]
        Build,

        [Mapping("rommode", "rom_mode")]
        RomMode,

        [Mapping("biosmode", "bios_mode")]
        BiosMode,

        [Mapping("samplemode", "sample_mode")]
        SampleMode,

        [Mapping("lockrommode", "lockrom_mode", "lock_rommode", "lock_rom_mode")]
        LockRomMode,

        [Mapping("lockbiosmode", "lockbios_mode", "lock_biosmode", "lock_bios_mode")]
        LockBiosMode,

        [Mapping("locksamplemode", "locksample_mode", "lock_samplemode", "lock_sample_mode")]
        LockSampleMode,

        #endregion

        #region OfflineList

        /// <remarks>"plugin" is used with RomCenter</remarks>
        [Mapping("system", "plugin")]
        System,

        [Mapping("screenshotwidth", "screenshotswidth", "screenshot_width", "screenshots_width")]
        ScreenshotsWidth,

        [Mapping("screenshotheight", "screenshotsheight", "screenshot_height", "screenshots_height")]
        ScreenshotsHeight,

        [Mapping("canopen", "can_open")]
        CanOpen,

        [Mapping("romtitle", "rom_title")]
        RomTitle,

        // Infos

        [Mapping("info_name", "infos_name")]
        Info_Name,

        [Mapping("info_visible", "infos_visible")]
        Info_Visible,

        [Mapping("info_isnamingoption", "info_is_naming_option", "infos_isnamingoption", "infos_is_naming_option")]
        Info_IsNamingOption,

        [Mapping("info_default", "infos_default")]
        Info_Default,

        #endregion

        #region RomCenter

        [Mapping("rcversion", "rc_version", "romcenterversion", "romcenter_version", "rom_center_version")]
        RomCenterVersion,

        #endregion
    }

    /// <summary>
    /// List of valid field types within a DatItem
    /// </summary>
    public enum DatItemField
    {
        NULL = 0,

        #region Common

        [Mapping("type")]
        Type,

        #endregion

        #region Item-Specific

        #region Actionable

        #region Rom

        [Mapping("name")]
        Name,

        [Mapping("bios")]
        Bios,

        [Mapping("size")]
        Size,

        [Mapping("crc", "crc32")]
        CRC,

        [Mapping("md5", "md5hash", "md5_hash")]
        MD5,

        [Mapping("sha1", "sha_1", "sha1hash", "sha1_hash", "sha_1hash", "sha_1_hash")]
        SHA1,

        [Mapping("sha256", "sha_256", "sha256hash", "sha256_hash", "sha_256hash", "sha_256_hash")]
        SHA256,

        [Mapping("sha384", "sha_384", "sha384hash", "sha384_hash", "sha_384hash", "sha_384_hash")]
        SHA384,

        [Mapping("sha512", "sha_512", "sha512hash", "sha512_hash", "sha_512hash", "sha_512_hash")]
        SHA512,

        [Mapping("spamsum", "spam_sum")]
        SpamSum,

        [Mapping("merge", "mergetag", "merge_tag")]
        Merge,

        [Mapping("region")]
        Region,

        [Mapping("offset")]
        Offset,

        [Mapping("date")]
        Date,

        [Mapping("status")]
        Status,

        [Mapping("optional")]
        Optional,

        [Mapping("inverted")]
        Inverted,

        #endregion

        #region Rom (Archive.org)

        [Mapping("ado_source")]
        ArchiveDotOrgSource,

        [Mapping("ado_format")]
        ArchiveDotOrgFormat,

        [Mapping("original_filename")]
        OriginalFilename,

        [Mapping("rotation")]
        Rotation,

        [Mapping("summation")]
        Summation,

        #endregion

        #region Rom (AttractMode)

        [Mapping("altname", "alt_name", "altromname", "alt_romname", "alt_rom_name")]
        AltName,

        [Mapping("alttitle", "alt_title", "altromtitle", "alt_romtitle", "alt_rom_title")]
        AltTitle,

        #endregion

        #region Rom (Logiqx)

        [Mapping("mia")]
        MIA,

        #endregion

        #region Rom (OpenMSX)

        [Mapping("original")]
        Original,

        [Mapping("subtype", "sub_type", "openmsxsubtype", "openmsx_subtype", "openmsx_sub_type")]
        OpenMSXSubType,

        [Mapping("openmsxtype", "openmsx_type")]
        OpenMSXType,

        [Mapping("remark")]
        Remark,

        [Mapping("boot")]
        Boot,

        #endregion

        #region Rom (SoftwareList)

        [Mapping("loadflag", "load_flag")]
        LoadFlag,

        [Mapping("value")]
        Value,

        #endregion

        #region Disk

        [Mapping("index")]
        Index,

        [Mapping("writable")]
        Writable,

        #endregion

        #endregion

        #region Auxiliary

        #region Adjuster

        [Mapping("default")]
        Default,

        #endregion

        #region Analog

        [Mapping("analog_mask")]
        Analog_Mask,

        #endregion

        #region Archive

        [Mapping("number")]
        Number,

        [Mapping("clone")]
        Clone,

        [Mapping("regparent", "reg_parent")]
        RegParent,

        [Mapping("languages")]
        Languages,

        [Mapping("devstatus", "dev_status")]
        DevStatus,

        [Mapping("physical")]
        Physical,

        [Mapping("complete")]
        Complete,

        [Mapping("categories")]
        Categories,

        #endregion

        #region BiosSet

        [Mapping("description", "biosdescription", "bios_description")]
        Description,

        #endregion

        #region Chip

        [Mapping("tag")]
        Tag,

        [Mapping("chiptype", "chip_type")]
        ChipType,

        [Mapping("clock")]
        Clock,

        #endregion

        #region Condition

        [Mapping("mask")]
        Mask,

        [Mapping("relation")]
        Relation,

        [Mapping("condition_tag")]
        Condition_Tag,

        [Mapping("condition_mask")]
        Condition_Mask,

        [Mapping("condition_relation")]
        Condition_Relation,

        [Mapping("condition_value")]
        Condition_Value,

        #endregion

        #region Control

        [Mapping("control_type")]
        Control_Type,

        [Mapping("control_player")]
        Control_Player,

        [Mapping("control_buttons")]
        Control_Buttons,

        [Mapping("control_reqbuttons", "control_req_buttons")]
        Control_RequiredButtons,

        [Mapping("control_minimum")]
        Control_Minimum,

        [Mapping("control_maximum")]
        Control_Maximum,

        [Mapping("control_sensitivity")]
        Control_Sensitivity,

        [Mapping("control_keydelta", "control_key_delta")]
        Control_KeyDelta,

        [Mapping("control_reverse")]
        Control_Reverse,

        [Mapping("control_ways")]
        Control_Ways,

        [Mapping("control_ways2")]
        Control_Ways2,

        [Mapping("control_ways3")]
        Control_Ways3,

        #endregion

        #region DataArea

        [Mapping("areaname", "area_name")]
        AreaName,

        [Mapping("areasize", "area_size")]
        AreaSize,

        [Mapping("areawidth", "area_width")]
        AreaWidth,

        [Mapping("areaendinanness", "area_endianness")]
        AreaEndianness,

        #endregion

        #region Device

        [Mapping("devicetype", "device_type")]
        DeviceType,

        [Mapping("fixedimage", "fixed_image")]
        FixedImage,

        [Mapping("mandatory")]
        Mandatory,

        [Mapping("interface")]
        Interface,

        #endregion

        #region Display

        [Mapping("displaytype", "display_type")]
        DisplayType,

        [Mapping("rotate")]
        Rotate,

        [Mapping("flipx")]
        FlipX,

        [Mapping("width")]
        Width,

        [Mapping("height")]
        Height,

        [Mapping("refresh")]
        Refresh,

        [Mapping("pixclock", "pix_clock")]
        PixClock,

        [Mapping("htotal")]
        HTotal,

        [Mapping("hbend")]
        HBEnd,

        [Mapping("hbstart")]
        HBStart,

        [Mapping("vtotal")]
        VTotal,

        [Mapping("vbend")]
        VBEnd,

        [Mapping("vbstart")]
        VBStart,

        #endregion

        #region Driver

        [Mapping("supportstatus", "support_status")]
        SupportStatus,

        [Mapping("emulationstatus", "emulation_status")]
        EmulationStatus,

        [Mapping("cocktailstatus", "cocktail_status")]
        CocktailStatus,

        [Mapping("savestatestatus", "savestate_status", "save_state_status")]
        SaveStateStatus,

        [Mapping("requiresartwork", "requires_artwork")]
        RequiresArtwork,

        [Mapping("unofficial")]
        Unofficial,

        [Mapping("nosoundhardware", "no_sound_hardware")]
        NoSoundHardware,

        [Mapping("incomplete")]
        Incomplete,

        #endregion

        #region Extension

        [Mapping("extension_name")]
        Extension_Name,

        #endregion

        #region Feature

        [Mapping("featuretype", "feature_type")]
        FeatureType,

        [Mapping("featurestatus", "feature_status")]
        FeatureStatus,

        [Mapping("featureoverall", "feature_overall")]
        FeatureOverall,

        #endregion

        #region Input

        [Mapping("service")]
        Service,

        [Mapping("tilt")]
        Tilt,

        [Mapping("players")]
        Players,

        [Mapping("coins")]
        Coins,

        #endregion

        #region Instance

        [Mapping("instance_name")]
        Instance_Name,

        [Mapping("instance_briefname", "instance_brief_name")]
        Instance_BriefName,

        #endregion

        #region Location

        [Mapping("location_name")]
        Location_Name,

        [Mapping("location_number")]
        Location_Number,

        [Mapping("location_inverted")]
        Location_Inverted,

        #endregion

        #region Part

        [Mapping("partname", "part_name")]
        Part_Name,

        [Mapping("partinterface", "part_interface")]
        Part_Interface,

        #endregion

        #region PartFeature

        [Mapping("part_feature_name")]
        Part_Feature_Name,

        [Mapping("part_feature_value")]
        Part_Feature_Value,

        #endregion

        #region RamOption

        [Mapping("content")]
        Content,

        #endregion

        #region Release

        [Mapping("language")]
        Language,

        #endregion

        #region Setting

        [Mapping("setting_name", "value_name")]
        Setting_Name,

        [Mapping("setting_value", "value_value")]
        Setting_Value,

        [Mapping("setting_default", "value_default")]
        Setting_Default,

        #endregion

        #region SlotOption

        [Mapping("slotoption_name")]
        SlotOption_Name,

        [Mapping("slotoption_devicename", "slotoption_device_name")]
        SlotOption_DeviceName,

        [Mapping("slotoption_default")]
        SlotOption_Default,

        #endregion

        #region SoftwareList

        [Mapping("softwareliststatus", "softwarelist_status")]
        SoftwareListStatus,

        [Mapping("filter")]
        Filter,

        #endregion

        #region Sound

        [Mapping("channels")]
        Channels,

        #endregion

        #endregion

        #endregion // Item-Specific
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

        #region Common

        [Mapping("name")]
        Name,

        /// <remarks>"extra" is used with AttractMode</remarks>
        [Mapping("comment", "extra")]
        Comment,

        [Mapping("desc", "description")]
        Description,

        [Mapping("year")]
        Year,

        [Mapping("manufacturer")]
        Manufacturer,

        [Mapping("publisher")]
        Publisher,

        [Mapping("category")]
        Category,

        [Mapping("romof", "rom_of")]
        RomOf,

        [Mapping("cloneof", "clone_of")]
        CloneOf,

        [Mapping("sampleof", "sample_of")]
        SampleOf,

        [Mapping("type")]
        Type,

        #endregion

        #region AttractMode

        [Mapping("players")]
        Players,

        [Mapping("rotation")]
        Rotation,

        [Mapping("control")]
        Control,

        [Mapping("amstatus", "am_status", "gamestatus", "supportstatus", "support_status")]
        Status,

        [Mapping("displaycount", "display_count")]
        DisplayCount,

        [Mapping("displaytype", "display_type")]
        DisplayType,

        [Mapping("buttons")]
        Buttons,

        #endregion

        #region ListXML

        [Mapping("history")]
        History,

        [Mapping("sourcefile", "source_file")]
        SourceFile,

        [Mapping("runnable")]
        Runnable,

        #endregion

        #region Logiqx

        [Mapping("board")]
        Board,

        [Mapping("rebuildto", "rebuild_to")]
        RebuildTo,

        [Mapping("id", "nointroid", "nointro_id", "no_intro_id")]
        NoIntroId,

        [Mapping("cloneofid", "nointrocloneofid", "nointro_cloneofid", "no_intro_cloneofid", "no_intro_clone_of_id")]
        NoIntroCloneOfId,

        #endregion

        #region Logiqx EmuArc

        [Mapping("titleid", "title_id")]
        TitleID,

        [Mapping("developer")]
        Developer,

        [Mapping("genre")]
        Genre,

        [Mapping("subgenre", "sub_genre")]
        Subgenre,

        [Mapping("ratings")]
        Ratings,

        [Mapping("score")]
        Score,

        [Mapping("enabled")]
        Enabled,

        [Mapping("crc", "hascrc", "has_crc")]
        CRC,

        [Mapping("relatedto", "related_to")]
        RelatedTo,

        #endregion

        #region OpenMSX

        [Mapping("genmsxid", "genmsx_id", "gen_msxid", "gen_msx_id")]
        GenMSXID,

        [Mapping("system", "msxsystem", "msx_system")]
        System,

        [Mapping("country")]
        Country,

        #endregion

        #region SoftwareList

        [Mapping("supported")]
        Supported,

        #endregion
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
