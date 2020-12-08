using System;

namespace SabreTools.Data
{
    #region DatFile

    /// <summary>
    /// DAT output formats
    /// </summary>
    [Flags]
    public enum DatFormat
    {
        #region XML Formats

        /// <summary>
        /// Logiqx XML (using machine)
        /// </summary>
        Logiqx = 1 << 0,

        /// <summary>
        /// Logiqx XML (using game)
        /// </summary>
        LogiqxDeprecated = 1 << 1,

        /// <summary>
        /// MAME Softare List XML
        /// </summary>
        SoftwareList = 1 << 2,

        /// <summary>
        /// MAME Listxml output
        /// </summary>
        Listxml = 1 << 3,

        /// <summary>
        /// OfflineList XML
        /// </summary>
        OfflineList = 1 << 4,

        /// <summary>
        /// SabreDAT XML
        /// </summary>
        SabreXML = 1 << 5,

        /// <summary>
        /// openMSX Software List XML
        /// </summary>
        OpenMSX = 1 << 6,

        #endregion

        #region Propietary Formats

        /// <summary>
        /// ClrMamePro custom
        /// </summary>
        ClrMamePro = 1 << 7,

        /// <summary>
        /// RomCenter INI-based
        /// </summary>
        RomCenter = 1 << 8,

        /// <summary>
        /// DOSCenter custom
        /// </summary>
        DOSCenter = 1 << 9,

        /// <summary>
        /// AttractMode custom
        /// </summary>
        AttractMode = 1 << 10,

        #endregion

        #region Standardized Text Formats

        /// <summary>
        /// ClrMamePro missfile
        /// </summary>
        MissFile = 1 << 11,

        /// <summary>
        /// Comma-Separated Values (standardized)
        /// </summary>
        CSV = 1 << 12,

        /// <summary>
        /// Semicolon-Separated Values (standardized)
        /// </summary>
        SSV = 1 << 13,

        /// <summary>
        /// Tab-Separated Values (standardized)
        /// </summary>
        TSV = 1 << 14,

        /// <summary>
        /// MAME Listrom output
        /// </summary>
        Listrom = 1 << 15,

        /// <summary>
        /// Everdrive Packs SMDB
        /// </summary>
        EverdriveSMDB = 1 << 16,

        /// <summary>
        /// SabreJSON
        /// </summary>
        SabreJSON = 1 << 17,

        #endregion

        #region SFV-similar Formats

        /// <summary>
        /// CRC32 hash list
        /// </summary>
        RedumpSFV = 1 << 18,

        /// <summary>
        /// MD5 hash list
        /// </summary>
        RedumpMD5 = 1 << 19,

#if NET_FRAMEWORK
        /// <summary>
        /// RIPEMD160 hash list
        /// </summary>
        RedumpRIPEMD160 = 1 << 20,
#endif

        /// <summary>
        /// SHA-1 hash list
        /// </summary>
        RedumpSHA1 = 1 << 21,

        /// <summary>
        /// SHA-256 hash list
        /// </summary>
        RedumpSHA256 = 1 << 22,

        /// <summary>
        /// SHA-384 hash list
        /// </summary>
        RedumpSHA384 = 1 << 23,

        /// <summary>
        /// SHA-512 hash list
        /// </summary>
        RedumpSHA512 = 1 << 24,

        /// <summary>
        /// SpamSum hash list
        /// </summary>
        RedumpSpamSum = 1 << 25,

        #endregion

        // Specialty combinations
        ALL = Int32.MaxValue,
    }

    /// <summary>
    /// Determines the DAT deduplication type
    /// </summary>
    public enum DedupeType
    {
        None = 0,
        Full,

        // Force only deduping with certain types
        Game,
        CRC,
        MD5,
#if NET_FRAMEWORK
        RIPEMD160,
#endif
        SHA1,
        SHA256,
        SHA384,
        SHA512,
    }

    /// <summary>
    /// List of valid field types within a DatItem/Machine
    /// </summary>
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

        DatItem_Type,

        #endregion

        #region Item-Specific

        #region Actionable

        // Rom
        DatItem_Name,
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
        DatItem_SpamSum,
        DatItem_Merge,
        DatItem_Region,
        DatItem_Offset,
        DatItem_Date,
        DatItem_Status,
        DatItem_Optional,
        DatItem_Inverted,

        // Rom (AttractMode)
        DatItem_AltName,
        DatItem_AltTitle,

        // Rom (OpenMSX)
        DatItem_Original,
        DatItem_OpenMSXSubType,
        DatItem_OpenMSXType,
        DatItem_Remark,
        DatItem_Boot,

        // Rom (SoftwareList)
        DatItem_LoadFlag,
        DatItem_Value,

        // Disk
        DatItem_Index,
        DatItem_Writable,

        #endregion

        #region Auxiliary

        // Adjuster
        DatItem_Default,

        // Analog
        DatItem_Analog_Mask,

        // BiosSet
        DatItem_Description,

        // Chip
        DatItem_Tag,
        DatItem_ChipType,
        DatItem_Clock,

        // Condition
        DatItem_Mask,
        DatItem_Relation,
        DatItem_Condition_Tag,
        DatItem_Condition_Mask,
        DatItem_Condition_Relation,
        DatItem_Condition_Value,

        // Control
        DatItem_Control_Type,
        DatItem_Control_Player,
        DatItem_Control_Buttons,
        DatItem_Control_RequiredButtons,
        DatItem_Control_Minimum,
        DatItem_Control_Maximum,
        DatItem_Control_Sensitivity,
        DatItem_Control_KeyDelta,
        DatItem_Control_Reverse,
        DatItem_Control_Ways,
        DatItem_Control_Ways2,
        DatItem_Control_Ways3,

        // DataArea
        DatItem_AreaName,
        DatItem_AreaSize,
        DatItem_AreaWidth,
        DatItem_AreaEndianness,

        // Device
        DatItem_DeviceType,
        DatItem_FixedImage,
        DatItem_Mandatory,
        DatItem_Interface,

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

        // Extension
        DatItem_Extension_Name,

        // Feature
        DatItem_FeatureType,
        DatItem_FeatureStatus,
        DatItem_FeatureOverall,

        // Input
        DatItem_Service,
        DatItem_Tilt,
        DatItem_Players,
        DatItem_Coins,

        // Instance
        DatItem_Instance_Name,
        DatItem_Instance_BriefName,

        // Location
        DatItem_Location_Name,
        DatItem_Location_Number,
        DatItem_Location_Inverted,

        // Part
        DatItem_Part_Name,
        DatItem_Part_Interface,

        // PartFeature
        DatItem_Part_Feature_Name,
        DatItem_Part_Feature_Value,

        // RamOption
        DatItem_Content,

        // Release
        DatItem_Language,

        // Setting
        DatItem_Setting_Name,
        DatItem_Setting_Value,
        DatItem_Setting_Default,

        // SlotOption
        DatItem_SlotOption_Name,
        DatItem_SlotOption_DeviceName,
        DatItem_SlotOption_Default,

        // SoftwareList
        DatItem_SoftwareListStatus,
        DatItem_Filter,

        // Sound
        DatItem_Channels,

        #endregion

        #endregion // Item-Specific

        #endregion // DatItem
    }

    /// <summary>
    /// Available hashing types
    /// </summary>
    [Flags]
    public enum Hash
    {
        CRC = 1 << 0,
        MD5 = 1 << 1,
#if NET_FRAMEWORK
        RIPEMD160 = 1 << 2,
#endif
        SHA1 = 1 << 3,
        SHA256 = 1 << 4,
        SHA384 = 1 << 5,
        SHA512 = 1 << 6,
        SpamSum = 1 << 7,

        // Special combinations
        Standard = CRC | MD5 | SHA1,
#if NET_FRAMEWORK
        DeepHashes = RIPEMD160 | SHA256 | SHA384 | SHA512 | SpamSum,
        SecureHashes = MD5 | RIPEMD160 | SHA1 | SHA256 | SHA384 | SHA512 | SpamSum,
#else
        DeepHashes = SHA256 | SHA384 | SHA512 | SpamSum,
        SecureHashes = MD5 | SHA1 | SHA256 | SHA384 | SHA512 | SpamSum,
#endif
    }

    /// <summary>
    /// Determines what sort of files get externally hashed
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

    #endregion

    #region FileTypes

    /// <summary>
    /// Type of file that is being looked at
    /// </summary>
    public enum FileType
    {
        // Singleton
        None = 0,
        AaruFormat,
        CHD,

        // Can contain children
        Folder,
        SevenZipArchive,
        GZipArchive,
        LRZipArchive,
        LZ4Archive,
        RarArchive,
        TapeArchive,
        XZArchive,
        ZipArchive,
        ZPAQArchive,
        ZstdArchive,
    }

    #endregion
}
