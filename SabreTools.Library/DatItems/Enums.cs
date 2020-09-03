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
    /// List of valid field types within a DatItem/Machine
    /// </summary>
    /// TODO: Move this to a more common location
    /// TODO: Ensure the order of these after all shuffling
    public enum Field : int
    {
        NULL = 0,

        #region DatHeader

        #region Common

        DatHeader_FileName,
        DatHeader_Name,
        DatHeader_Description,
        DatHeader_RootDir,
        DatHeader_Category,
        DatHeader_Version,
        DatHeader_Date,
        DatHeader_Author,
        DatHeader_Email,
        DatHeader_Homepage,
        DatHeader_Url,
        DatHeader_Comment,
        DatHeader_HeaderSkipper,
        DatHeader_Type,
        DatHeader_ForceMerging,
        DatHeader_ForceNodump,
        DatHeader_ForcePacking,

        #endregion

        #region ListXML

        DatHeader_Debug,
        DatHeader_MameConfig,

        #endregion

        #region Logiqx

        DatHeader_Build,
        DatHeader_RomMode,
        DatHeader_BiosMode,
        DatHeader_SampleMode,
        DatHeader_LockRomMode,
        DatHeader_LockBiosMode,
        DatHeader_LockSampleMode,

        #endregion

        #region OfflineList

        DatHeader_System,
        DatHeader_ScreenshotsWidth,
        DatHeader_ScreenshotsHeight,
        DatHeader_CanOpen,
        DatHeader_RomTitle,

        // Infos
        DatHeader_Infos,
        DatHeader_Info_Name,
        DatHeader_Info_Visible,
        DatHeader_Info_IsNamingOption,
        DatHeader_Info_Default,

        #endregion

        #region RomCenter

        DatHeader_RomCenterVersion,

        #endregion

        #endregion // DatHeader

        #region Machine

        #region Common

        Machine_Name,
        Machine_Comment,
        Machine_Description,
        Machine_Year,
        Machine_Manufacturer,
        Machine_Publisher,
        Machine_Category,
        Machine_RomOf,
        Machine_CloneOf,
        Machine_SampleOf,
        Machine_Type,

        #endregion

        #region AttractMode

        Machine_Players,
        Machine_Rotation,
        Machine_Control,
        Machine_Status,
        Machine_DisplayCount,
        Machine_DisplayType,
        Machine_Buttons,

        #endregion

        #region ListXML

        Machine_SourceFile,
        Machine_Runnable,

        #endregion

        #region Logiqx

        Machine_Board,
        Machine_RebuildTo,

        #endregion

        #region Logiqx EmuArc

        Machine_TitleID,
        Machine_Developer,
        Machine_Genre,
        Machine_Subgenre,
        Machine_Ratings,
        Machine_Score,
        Machine_Enabled,
        Machine_CRC,
        Machine_RelatedTo,

        #endregion

        #region OpenMSX

        Machine_GenMSXID,
        Machine_System,
        Machine_Country,

        #endregion

        #region SoftwareList

        Machine_Supported,

        #endregion

        #endregion // Machine

        #region DatItem

        #region Common

        DatItem_Name,
        DatItem_Type,

        #endregion

        #region AttractMode

        DatItem_AltName,
        DatItem_AltTitle,

        #endregion

        #region SoftwareList

        // Part
        DatItem_Part,
        DatItem_Part_Name,
        DatItem_Part_Interface,

        // Feature
        DatItem_Features,
        DatItem_Feature_Name,
        DatItem_Feature_Value,

        DatItem_LoadFlag,

        #endregion

        #region Item-Specific

        #region Actionable

        // Rom
        DatItem_Bios,
        DatItem_Size,
        DatItem_CRC,
        DatItem_MD5,
#if NET_FRAMEWORK
        DatItem_RIPEMD160,
#endif
        DatItem_SHA1,
        DatItem_SHA256,
        DatItem_SHA384,
        DatItem_SHA512,
        DatItem_Merge,
        DatItem_Region,
        DatItem_Offset,
        DatItem_Date,
        DatItem_Status,
        DatItem_Optional,
        DatItem_Inverted,

        // Rom (OpenMSX)
        DatItem_Original,
        DatItem_OpenMSXSubType,
        DatItem_OpenMSXType,
        DatItem_Remark,
        DatItem_Boot,

        // Rom (SoftwareList)
        DatItem_AreaName,
        DatItem_AreaSize,
        DatItem_AreaWidth,
        DatItem_AreaEndianness,
        DatItem_Value,

        // Disk
        DatItem_Index,
        DatItem_Writable,

        #endregion

        #region Auxiliary

        // Adjuster
        DatItem_Default,
        DatItem_Conditions,
        DatItem_Condition_Tag,
        DatItem_Condition_Mask,
        DatItem_Condition_Relation,
        DatItem_Condition_Value,

        // Analog
        DatItem_Mask,

        // BiosSet
        DatItem_Description,

        // Chip
        DatItem_Tag,
        DatItem_ChipType,
        DatItem_Clock,

        // Condition
        DatItem_Relation,

        // Configuration.Locations
        DatItem_Locations,
        DatItem_Location_Name,
        DatItem_Location_Number,
        DatItem_Location_Inverted,

        // Configuration.Settings
        DatItem_Settings,
        DatItem_Setting_Name,
        DatItem_Setting_Value,
        DatItem_Setting_Default,

        // Device
        DatItem_DeviceType,
        DatItem_FixedImage,
        DatItem_Mandatory,
        DatItem_Interface,

        // Device.Instances
        DatItem_Instances,
        DatItem_Instance_Name,
        DatItem_Instance_BriefName,

        // Device.Extensions
        DatItem_Extensions,
        DatItem_Extension_Name,

        // DipSwitch.Values
        DatItem_Values,
        DatItem_Value_Name,
        DatItem_Value_Value,
        DatItem_Value_Default,

        // Display
        DatItem_DisplayType,
        DatItem_Rotate,
        DatItem_FlipX,
        DatItem_Width,
        DatItem_Height,
        DatItem_Refresh,
        DatItem_PixClock,
        DatItem_HTotal,
        DatItem_HBEnd,
        DatItem_HBStart,
        DatItem_VTotal,
        DatItem_VBEnd,
        DatItem_VBStart,

        // Driver
        DatItem_SupportStatus,
        DatItem_EmulationStatus,
        DatItem_CocktailStatus,
        DatItem_SaveStateStatus,

        // Feature
        DatItem_FeatureType,
        DatItem_FeatureStatus,
        DatItem_FeatureOverall,

        // Inputs
        DatItem_Service,
        DatItem_Tilt,
        DatItem_Players,
        DatItem_Coins,

        // Inputs.Controls
        DatItem_Controls,
        DatItem_Control_Type,
        DatItem_Control_Player,
        DatItem_Control_Buttons,
        DatItem_Control_RegButtons,
        DatItem_Control_Minimum,
        DatItem_Control_Maximum,
        DatItem_Control_Sensitivity,
        DatItem_Control_KeyDelta,
        DatItem_Control_Reverse,
        DatItem_Control_Ways,
        DatItem_Control_Ways2,
        DatItem_Control_Ways3,

        // Port.Analogs
        DatItem_Analogs,
        DatItem_Analog_Mask,

        // Ram Option
        DatItem_Content,

        // Release
        DatItem_Language,

        // Slot.SlotOptions
        DatItem_SlotOptions,
        DatItem_SlotOption_Name,
        DatItem_SlotOption_DeviceName,
        DatItem_SlotOption_Default,

        // Software List
        DatItem_SoftwareListStatus,
        DatItem_Filter,

        // Sounds
        DatItem_Channels,

        #endregion

        #endregion // Item-Specific

        #endregion // DatItem
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
        Device,
        DeviceReference,
        DipSwitch,
        Display,
        Driver,
        Extension,
        Feature,
        Info,
        Input,
        Instance,
        Location,
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
