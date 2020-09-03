using System.Text.RegularExpressions;

using SabreTools.Library.DatFiles;
using SabreTools.Library.DatItems;
using SabreTools.Library.FileTypes;
using SabreTools.Library.Reports;

namespace SabreTools.Library.Tools
{
    public static class Converters
    {
        #region Enum to Enum

        /// <summary>
        /// Get the field associated with each hash type
        /// </summary>
        public static Field AsField(this Hash hash)
        {
            switch (hash)
            {
                case Hash.CRC:
                    return Field.DatItem_CRC;
                case Hash.MD5:
                    return Field.DatItem_MD5;
#if NET_FRAMEWORK
                case Hash.RIPEMD160:
                    return Field.DatItem_RIPEMD160;
#endif
                case Hash.SHA1:
                    return Field.DatItem_SHA1;
                case Hash.SHA256:
                    return Field.DatItem_SHA256;
                case Hash.SHA384:
                    return Field.DatItem_SHA384;
                case Hash.SHA512:
                    return Field.DatItem_SHA512;

                default:
                    return Field.NULL;
            }
        }

        /// <summary>
        /// Get the default OutputFormat associated with each PackingFlag
        /// </summary>
        /// <param name="packing"></param>
        /// <returns></returns>
        public static OutputFormat AsOutputFormat(this PackingFlag packing)
        {
            switch (packing)
            {
                case PackingFlag.Zip:
                    return OutputFormat.TorrentZip;
                case PackingFlag.Unzip:
                case PackingFlag.Partial:
                    return OutputFormat.Folder;
                case PackingFlag.Flat:
                    return OutputFormat.ParentFolder;
                case PackingFlag.None:
                default:
                    return OutputFormat.Folder;
            }
        }

        #endregion

        #region String to Enum

        /// <summary>
        /// Get ChipType value from input string
        /// </summary>
        /// <param name="chipType">String to get value from</param>
        /// <returns>ChipType value corresponding to the string</returns>
        public static ChipType AsChipType(this string chipType)
        {
#if NET_FRAMEWORK
            switch (chipType?.ToLowerInvariant())
            {
                case "cpu":
                    return ChipType.CPU;
                case "audio":
                    return ChipType.Audio;
                default:
                    return ChipType.NULL;
            }
#else
            return chipType?.ToLowerInvariant() switch
            {
                "cpu" => ChipType.CPU,
                "audio" => ChipType.Audio,
                _ => ChipType.NULL,
            };
#endif
        }

        /// <summary>
        /// Get DatFormat value from input string
        /// </summary>
        /// <param name="input">String to get value from</param>
        /// <returns>DatFormat value corresponding to the string</returns>
        public static DatFormat AsDatFormat(this string input)
        {
            switch (input?.Trim().ToLowerInvariant())
            {
                case "all":
                    return DatFormat.ALL;
                case "am":
                case "attractmode":
                    return DatFormat.AttractMode;
                case "cmp":
                case "clrmamepro":
                    return DatFormat.ClrMamePro;
                case "csv":
                    return DatFormat.CSV;
                case "dc":
                case "doscenter":
                    return DatFormat.DOSCenter;
                case "json":
                    return DatFormat.Json;
                case "lr":
                case "listrom":
                    return DatFormat.Listrom;
                case "lx":
                case "listxml":
                    return DatFormat.Listxml;
                case "md5":
                    return DatFormat.RedumpMD5;
                case "miss":
                case "missfile":
                    return DatFormat.MissFile;
                case "msx":
                case "openmsx":
                    return DatFormat.OpenMSX;
                case "ol":
                case "offlinelist":
                    return DatFormat.OfflineList;
                case "rc":
                case "romcenter":
                    return DatFormat.RomCenter;
#if NET_FRAMEWORK
                case "ripemd160":
                    return DatFormat.RedumpRIPEMD160;
#endif
                case "sd":
                case "sabredat":
                    return DatFormat.SabreDat;
                case "sfv":
                    return DatFormat.RedumpSFV;
                case "sha1":
                    return DatFormat.RedumpSHA1;
                case "sha256":
                    return DatFormat.RedumpSHA256;
                case "sha384":
                    return DatFormat.RedumpSHA384;
                case "sha512":
                    return DatFormat.RedumpSHA512;
                case "sl":
                case "softwarelist":
                    return DatFormat.SoftwareList;
                case "smdb":
                case "everdrive":
                    return DatFormat.EverdriveSMDB;
                case "ssv":
                    return DatFormat.SSV;
                case "tsv":
                    return DatFormat.TSV;
                case "xml":
                case "logiqx":
                    return DatFormat.Logiqx;
                default:
                    return 0x0;
            }
        }

        /// <summary>
        /// Get FeatureStatus value from input string
        /// </summary>
        /// <param name="featureStatus">String to get value from</param>
        /// <returns>FeatureStatus value corresponding to the string</returns>
        public static FeatureStatus AsFeatureStatus(this string featureStatus)
        {
#if NET_FRAMEWORK
            switch (featureStatus?.ToLowerInvariant())
            {
                case "unemulated":
                    return FeatureStatus.Unemulated;
                case "imperfect":
                    return FeatureStatus.Imperfect;
                default:
                    return FeatureStatus.NULL;
            }
#else
            return featureStatus?.ToLowerInvariant() switch
            {
                "unemulated" => FeatureStatus.Unemulated,
                "imperfect" => FeatureStatus.Imperfect,
                _ => FeatureStatus.NULL,
            };
#endif
        }

        /// <summary>
        /// Get FeatureType value from input string
        /// </summary>
        /// <param name="emulationStatus">String to get value from</param>
        /// <returns>FeatureType value corresponding to the string</returns>
        public static FeatureType AsFeatureType(this string featureType)
        {
#if NET_FRAMEWORK
            switch (featureType?.ToLowerInvariant())
            {
                case "protection":
                    return FeatureType.Protection;
                case "palette":
                    return FeatureType.Palette;
                case "graphics":
                    return FeatureType.Graphics;
                case "sound":
                    return FeatureType.Sound;
                case "controls":
                    return FeatureType.Controls;
                case "keyboard":
                    return FeatureType.Keyboard;
                case "mouse":
                    return FeatureType.Mouse;
                case "microphone":
                    return FeatureType.Microphone;
                case "camera":
                    return FeatureType.Camera;
                case "disk":
                    return FeatureType.Disk;
                case "printer":
                    return FeatureType.Printer;
                case "lan":
                    return FeatureType.Lan;
                case "wan":
                    return FeatureType.Wan;
                case "timing":
                    return FeatureType.Timing;
                default:
                    return FeatureType.NULL;
            }
#else
            return featureType?.ToLowerInvariant() switch
            {
                "protection" => FeatureType.Protection,
                "palette" => FeatureType.Palette,
                "graphics" => FeatureType.Graphics,
                "sound" => FeatureType.Sound,
                "controls" => FeatureType.Controls,
                "keyboard" => FeatureType.Keyboard,
                "mouse" => FeatureType.Mouse,
                "microphone" => FeatureType.Microphone,
                "camera" => FeatureType.Camera,
                "disk" => FeatureType.Disk,
                "printer" => FeatureType.Printer,
                "lan" => FeatureType.Lan,
                "wan" => FeatureType.Wan,
                "timing" => FeatureType.Timing,
                _ => FeatureType.NULL,
            };
#endif
        }

        /// <summary>
        /// Get Field value from input string
        /// </summary>
        /// <param name="input">String to get value from</param>
        /// <returns>Field value corresponding to the string</returns>
        public static Field AsField(this string input)
        {
            // If the input is null, we return null
            if (input == null)
                return Field.NULL;

            // Normalize the input
            input = input.ToLowerInvariant();

            // Create regex strings
            string headerRegex = @"^(dat|header|datheader)[.\-_\s]";
            string machineRegex = @"^(game|machine)[.\-_\s]";
            string datItemRegex = @"^(item|datitem)[.\-_\s]";

            // If we have a header field
            if (Regex.IsMatch(input, headerRegex))
            {
                // Replace the match and re-normalize
                string headerInput = Regex.Replace(input, headerRegex, string.Empty)
                    .Replace(' ', '_')
                    .Replace('-', '_')
                    .Replace('.', '_');

                switch (headerInput)
                {
                    #region Common

                    case "file":
                    case "filename":
                    case "file_name":
                        return Field.DatHeader_FileName;

                    case "dat":
                    case "datname":
                    case "dat_name":
                    case "internalname":
                    case "internal_name":
                        return Field.DatHeader_Name;

                    case "desc":
                    case "description":
                        return Field.DatHeader_Description;

                    case "root":
                    case "rootdir":
                    case "root_dir":
                    case "rootdirectory":
                    case "root_directory":
                        return Field.DatHeader_RootDir;

                    case "category":
                        return Field.DatHeader_Category;

                    case "version":
                        return Field.DatHeader_Version;

                    case "date":
                    case "timestamp":
                    case "time_stamp":
                        return Field.DatHeader_Date;

                    case "author":
                        return Field.DatHeader_Author;

                    case "email":
                    case "e_mail":
                        return Field.DatHeader_Email;

                    case "homepage":
                    case "home_page":
                        return Field.DatHeader_Homepage;

                    case "url":
                        return Field.DatHeader_Url;

                    case "comment":
                        return Field.DatHeader_Comment;

                    case "header":
                    case "headerskipper":
                    case "header_skipper":
                    case "skipper":
                        return Field.DatHeader_HeaderSkipper;

                    case "dattype":
                    case "type":
                    case "superdat":
                        return Field.DatHeader_Type;

                    case "forcemerging":
                    case "force_merging":
                        return Field.DatHeader_ForceMerging;

                    case "forcenodump":
                    case "force_nodump":
                        return Field.DatHeader_ForceNodump;

                    case "forcepacking":
                    case "force_packing":
                        return Field.DatHeader_ForcePacking;

                    #endregion

                    #region ListXML

                    case "debug":
                        return Field.DatHeader_Debug;

                    case "mameconfig":
                    case "mame_config":
                        return Field.DatHeader_MameConfig;

                    #endregion

                    #region Logiqx

                    case "build":
                        return Field.DatHeader_Build;

                    case "rommode":
                    case "rom_mode":
                        return Field.DatHeader_RomMode;

                    case "biosmode":
                    case "bios_mode":
                        return Field.DatHeader_BiosMode;

                    case "samplemode":
                    case "sample_mode":
                        return Field.DatHeader_SampleMode;

                    case "lockrommode":
                    case "lockrom_mode":
                    case "lock_rommode":
                    case "lock_rom_mode":
                        return Field.DatHeader_LockRomMode;

                    case "lockbiosmode":
                    case "lockbios_mode":
                    case "lock_biosmode":
                    case "lock_bios_mode":
                        return Field.DatHeader_LockBiosMode;

                    case "locksamplemode":
                    case "locksample_mode":
                    case "lock_samplemode":
                    case "lock_sample_mode":
                        return Field.DatHeader_LockSampleMode;

                    #endregion

                    #region OfflineList

                    case "system":
                    case "plugin": // Used with RomCenter
                        return Field.DatHeader_System;

                    case "screenshotwidth":
                    case "screenshotswidth":
                    case "screenshot_width":
                    case "screenshots_width":
                        return Field.DatHeader_ScreenshotsWidth;

                    case "screenshotheight":
                    case "screenshotsheight":
                    case "screenshot_height":
                    case "screenshots_height":
                        return Field.DatHeader_ScreenshotsHeight;

                    case "info":
                    case "infos":
                        return Field.DatHeader_Infos;

                    case "info_name":
                    case "infos_name":
                        return Field.DatHeader_Info_Name;

                    case "info_visible":
                    case "infos_visible":
                        return Field.DatHeader_Info_Visible;

                    case "info_isnamingoption":
                    case "info_is_naming_option":
                    case "infos_isnamingoption":
                    case "infos_is_naming_option":
                        return Field.DatHeader_Info_IsNamingOption;

                    case "info_default":
                    case "infos_default":
                        return Field.DatHeader_Info_Default;

                    case "canopen":
                    case "can_open":
                        return Field.DatHeader_CanOpen;

                    case "romtitle":
                    case "rom_title":
                        return Field.DatHeader_RomTitle;

                    #endregion

                    #region RomCenter

                    case "rcversion":
                    case "rc_version":
                    case "romcenterversion":
                    case "romcenter_version":
                    case "rom_center_version":
                        return Field.DatHeader_RomCenterVersion;

                    #endregion
                }
            }

            // If we have a machine field
            else if (Regex.IsMatch(input, machineRegex))
            {
                // Replace the match and re-normalize
                string machineInput = Regex.Replace(input, machineRegex, string.Empty)
                    .Replace(' ', '_')
                    .Replace('-', '_')
                    .Replace('.', '_');

                switch (machineInput)
                {
                    #region Common

                    case "name":
                        return Field.Machine_Name;

                    case "comment":
                    case "extra": // Used with AttractMode
                        return Field.Machine_Comment;

                    case "desc":
                    case "description":
                        return Field.Machine_Description;

                    case "year":
                        return Field.Machine_Year;

                    case "manufacturer":
                        return Field.Machine_Manufacturer;

                    case "publisher":
                        return Field.Machine_Publisher;

                    case "category":
                        return Field.Machine_Category;

                    case "romof":
                    case "rom_of":
                        return Field.Machine_RomOf;

                    case "cloneof":
                    case "clone_of":
                        return Field.Machine_CloneOf;

                    case "sampleof":
                    case "sample_of":
                        return Field.Machine_SampleOf;

                    case "type":
                        return Field.Machine_Type;

                    #endregion

                    #region AttractMode

                    case "players":
                        return Field.Machine_Players;

                    case "rotation":
                        return Field.Machine_Rotation;

                    case "control":
                        return Field.Machine_Control;

                    case "amstatus":
                    case "am_status":
                    case "gamestatus":
                    case "supportstatus":
                    case "support_status":
                        return Field.Machine_Status;

                    case "displaycount":
                        return Field.Machine_DisplayCount;

                    case "displaytype":
                        return Field.Machine_DisplayType;

                    case "buttons":
                        return Field.Machine_Buttons;

                    #endregion

                    #region ListXML

                    case "sourcefile":
                    case "source_file":
                        return Field.Machine_SourceFile;

                    case "runnable":
                        return Field.Machine_Runnable;                    

                    #endregion

                    #region Logiqx

                    case "board":
                        return Field.Machine_Board;

                    case "rebuildto":
                    case "rebuild_to":
                        return Field.Machine_RebuildTo;

                    #endregion

                    #region Logiqx EmuArc

                    case "titleid":
                    case "title_id":
                        return Field.Machine_TitleID;

                    case "developer":
                        return Field.Machine_Developer;

                    case "genre":
                        return Field.Machine_Genre;

                    case "subgenre":
                    case "sub_genre":
                        return Field.Machine_Subgenre;

                    case "ratings":
                        return Field.Machine_Ratings;

                    case "score":
                        return Field.Machine_Score;

                    case "enabled":
                        return Field.Machine_Enabled;

                    case "crc":
                    case "hascrc":
                    case "has_crc":
                        return Field.Machine_CRC;

                    case "relatedto":
                    case "related_to":
                        return Field.Machine_RelatedTo;

                    #endregion

                    #region OpenMSX

                    case "genmsxid":
                    case "genmsx_id":
                    case "gen_msxid":
                    case "gen_msx_id":
                        return Field.Machine_GenMSXID;

                    case "system":
                    case "msxsystem":
                    case "msx_system":
                        return Field.Machine_System;

                    case "country":
                        return Field.Machine_Country;

                    #endregion

                    #region SoftwareList

                    case "supported":
                        return Field.Machine_Supported;

                    #endregion
                }
            }

            // If we have a datitem field
            else if (Regex.IsMatch(input, datItemRegex))
            {
                // Replace the match and re-normalize
                string itemInput = Regex.Replace(input, datItemRegex, string.Empty)
                    .Replace(' ', '_')
                    .Replace('-', '_')
                    .Replace('.', '_');

                switch (itemInput)
                {
                    #region Common

                    case "type":
                        return Field.DatItem_Type;

                    #endregion

                    #region Item-Specific

                    #region Actionable

                    // Rom
                    case "name":
                        return Field.DatItem_Name;

                    case "bios":
                        return Field.DatItem_Bios;

                    case "size":
                        return Field.DatItem_Size;

                    case "crc":
                    case "crc32":
                        return Field.DatItem_CRC;

                    case "md5":
                    case "md5_hash":
                        return Field.DatItem_MD5;

#if NET_FRAMEWORK
                    case "ripemd160":
                    case "ripemd160_hash":
                        return Field.DatItem_RIPEMD160;
#endif

                    case "sha1":
                    case "sha_1":
                    case "sha1hash":
                    case "sha1_hash":
                    case "sha_1hash":
                    case "sha_1_hash":
                        return Field.DatItem_SHA1;

                    case "sha256":
                    case "sha_256":
                    case "sha256hash":
                    case "sha256_hash":
                    case "sha_256hash":
                    case "sha_256_hash":
                        return Field.DatItem_SHA256;

                    case "sha384":
                    case "sha_384":
                    case "sha384hash":
                    case "sha384_hash":
                    case "sha_384hash":
                    case "sha_384_hash":
                        return Field.DatItem_SHA384;

                    case "sha512":
                    case "sha_512":
                    case "sha512hash":
                    case "sha512_hash":
                    case "sha_512hash":
                    case "sha_512_hash":
                        return Field.DatItem_SHA512;

                    case "merge":
                    case "mergetag":
                    case "merge_tag":
                        return Field.DatItem_Merge;

                    case "region":
                        return Field.DatItem_Region;

                    case "offset":
                        return Field.DatItem_Offset;

                    case "date":
                        return Field.DatItem_Date;

                    case "status":
                        return Field.DatItem_Status;

                    case "optional":
                        return Field.DatItem_Optional;

                    case "inverted":
                        return Field.DatItem_Inverted;

                    // Rom (AttractMode)
                    case "altname":
                    case "alt name":
                    case "alt-name":
                    case "altromname":
                    case "alt romname":
                    case "alt-romname":
                        return Field.DatItem_AltName;

                    case "alttitle":
                    case "alt title":
                    case "alt-title":
                    case "altromtitle":
                    case "alt romtitle":
                    case "alt-romtitle":
                        return Field.DatItem_AltTitle;

                    // Rom (OpenMSX)
                    case "original":
                        return Field.DatItem_Original;

                    case "subtype":
                    case "sub_type":
                    case "openmsxsubtype":
                    case "openmsx_subtype":
                        return Field.DatItem_OpenMSXSubType;

                    case "openmsxtype":
                    case "openmsx_type":
                        return Field.DatItem_OpenMSXType;

                    case "remark":
                        return Field.DatItem_Remark;

                    case "boot":
                        return Field.DatItem_Boot;

                    // Rom (SoftwareList)
                    case "areaname":
                    case "area_name":
                        return Field.DatItem_AreaName;

                    case "areasize":
                    case "area_size":
                        return Field.DatItem_AreaSize;

                    case "areawidth":
                    case "area_width":
                        return Field.DatItem_AreaWidth;

                    case "areaendinanness":
                    case "area_endianness":
                        return Field.DatItem_AreaEndianness;

                    case "loadflag":
                    case "load_flag":
                        return Field.DatItem_LoadFlag;

                    case "partname":
                    case "part_name":
                        return Field.DatItem_Part_Name;

                    case "partinterface":
                    case "part_interface":
                        return Field.DatItem_Part_Interface;

                    case "part_feature_name":
                        return Field.DatItem_Part_Feature_Name;

                    case "part_feature_value":
                        return Field.DatItem_Part_Feature_Value;

                    case "value":
                        return Field.DatItem_Value;

                    // Disk
                    case "index":
                        return Field.DatItem_Index;

                    case "writable":
                        return Field.DatItem_Writable;

                    #endregion

                    #region Auxiliary

                    // Adjuster
                    case "default":
                        return Field.DatItem_Default;

                    // Analog
                    case "analog_mask":
                        return Field.DatItem_Analog_Mask;

                    // BiosSet
                    case "description":
                    case "biosdescription":
                    case "bios_description":
                        return Field.DatItem_Description;

                    // Chip
                    case "tag":
                        return Field.DatItem_Tag;

                    case "chiptype":
                    case "chip_type":
                        return Field.DatItem_ChipType;

                    case "clock":
                        return Field.DatItem_Clock;

                    // Condition
                    case "mask":
                        return Field.DatItem_Mask;

                    case "relation":
                        return Field.DatItem_Relation;

                    case "condition_tag":
                        return Field.DatItem_Condition_Tag;

                    case "condition_mask":
                        return Field.DatItem_Condition_Mask;

                    case "condition_relation":
                        return Field.DatItem_Condition_Relation;

                    case "condition_value":
                        return Field.DatItem_Condition_Value;

                    // Control
                    case "control_type":
                        return Field.DatItem_Control_Type;

                    case "control_player":
                        return Field.DatItem_Control_Player;

                    case "control_buttons":
                        return Field.DatItem_Control_Buttons;

                    case "control_regbuttons":
                        return Field.DatItem_Control_RegButtons;

                    case "control_minimum":
                        return Field.DatItem_Control_Minimum;

                    case "control_maximum":
                        return Field.DatItem_Control_Maximum;

                    case "control_sensitivity":
                        return Field.DatItem_Control_Sensitivity;

                    case "control_keydelta":
                        return Field.DatItem_Control_KeyDelta;

                    case "control_reverse":
                        return Field.DatItem_Control_Reverse;

                    case "control_ways":
                        return Field.DatItem_Control_Ways;

                    case "control_ways2":
                        return Field.DatItem_Control_Ways2;

                    case "control_ways3":
                        return Field.DatItem_Control_Ways3;

                    // Device
                    case "devicetype":
                        return Field.DatItem_DeviceType;

                    case "fixedimage":
                        return Field.DatItem_FixedImage;

                    case "mandatory":
                        return Field.DatItem_Mandatory;

                    case "interface":
                        return Field.DatItem_Interface;

                    // Display
                    case "displaytype":
                        return Field.DatItem_DisplayType;

                    case "rotate":
                        return Field.DatItem_Rotate;

                    case "flipx":
                        return Field.DatItem_FlipX;

                    case "width":
                        return Field.DatItem_Width;

                    case "height":
                        return Field.DatItem_Height;

                    case "refresh":
                        return Field.DatItem_Refresh;

                    case "pixclock":
                        return Field.DatItem_PixClock;

                    case "htotal":
                        return Field.DatItem_HTotal;

                    case "hbend":
                        return Field.DatItem_HBEnd;

                    case "hbstart":
                        return Field.DatItem_HBStart;

                    case "vtotal":
                        return Field.DatItem_VTotal;

                    case "vbend":
                        return Field.DatItem_VBEnd;

                    case "vbstart":
                        return Field.DatItem_VBStart;

                    // Driver
                    case "supportstatus":
                        return Field.DatItem_SupportStatus;

                    case "emulationstatus":
                        return Field.DatItem_EmulationStatus;

                    case "cocktailstatus":
                        return Field.DatItem_CocktailStatus;

                    case "savestatestatus":
                        return Field.DatItem_SaveStateStatus;

                    // Extension
                    case "extension_name":
                        return Field.DatItem_Extension_Name;

                    // Feature
                    case "featuretype":
                        return Field.DatItem_FeatureType;

                    case "featurestatus":
                        return Field.DatItem_FeatureStatus;

                    case "featureoverall":
                        return Field.DatItem_FeatureOverall;

                    // Input
                    case "service":
                        return Field.DatItem_Service;

                    case "tilt":
                        return Field.DatItem_Tilt;

                    case "players":
                        return Field.DatItem_Players;

                    case "coins":
                        return Field.DatItem_Coins;

                    // Instance
                    case "instance_name":
                        return Field.DatItem_Instance_Name;

                    case "instance_briefname":
                        return Field.DatItem_Instance_BriefName;

                    // Location
                    case "location_name":
                        return Field.DatItem_Location_Name;

                    case "location_number":
                        return Field.DatItem_Location_Number;

                    case "location_inverted":
                        return Field.DatItem_Location_Inverted;

                    // RamOption
                    case "content":
                        return Field.DatItem_Content;

                    // Release
                    case "language":
                        return Field.DatItem_Language;

                    // Setting
                    case "setting_name":
                    case "value_name":
                        return Field.DatItem_Setting_Name;

                    case "setting_value":
                    case "value_value":
                        return Field.DatItem_Setting_Value;

                    case "setting_default":
                    case "value_default":
                        return Field.DatItem_Setting_Default;

                    // SlotOption
                    case "slotoption_name":
                        return Field.DatItem_SlotOption_Name;

                    case "slotoption_devicename":
                        return Field.DatItem_SlotOption_DeviceName;

                    case "slotoption_default":
                        return Field.DatItem_SlotOption_Default;

                    // SoftwareList
                    case "softwareliststatus":
                    case "softwarelist_status":
                        return Field.DatItem_SoftwareListStatus;

                    case "filter":
                        return Field.DatItem_Filter;

                    // Sound
                    case "channels":
                        return Field.DatItem_Channels;

                    #endregion

                    #endregion // Item-Specific
                }
            }

            // Else, we fall back on the old matching
            // TODO: Remove this entirely
            // TODO: Normalize space replacement
            switch (input)
            {
                #region Machine

                #region Common

                case "game":
                case "gamename":
                case "game-name":
                case "machine":
                case "machinename":
                case "machine-name":
                    return Field.Machine_Name;

                case "comment":
                case "extra":
                    return Field.Machine_Comment;

                case "desc":
                case "description":
                case "gamedesc":
                case "gamedescription":
                case "game-description":
                case "game description":
                case "machinedesc":
                case "machinedescription":
                case "machine-description":
                case "machine description":
                    return Field.Machine_Description;

                case "year":
                    return Field.Machine_Year;

                case "manufacturer":
                    return Field.Machine_Manufacturer;

                case "publisher":
                    return Field.Machine_Publisher;

                case "category":
                case "gamecategory":
                case "game-category":
                case "machinecategory":
                case "machine-category":
                    return Field.Machine_Category;

                case "romof":
                    return Field.Machine_RomOf;

                case "cloneof":
                    return Field.Machine_CloneOf;

                case "sampleof":
                    return Field.Machine_SampleOf;

                case "gametype":
                case "game type":
                case "game-type":
                case "machinetype":
                case "machine type":
                case "machine-type":
                    return Field.Machine_Type;

                #endregion

                #region AttractMode

                case "players":
                    return Field.Machine_Players;

                case "rotation":
                    return Field.Machine_Rotation;

                case "control":
                    return Field.Machine_Control;

                case "amstatus":
                case "am-status":
                case "gamestatus":
                case "game-status":
                case "machinestatus":
                case "machine-status":
                case "supportstatus":
                case "support-status":
                    return Field.Machine_Status;

                case "displaycount":
                case "display-count":
                case "displays":
                    return Field.Machine_DisplayCount;

                case "displaytype":
                case "display-type":
                    return Field.Machine_DisplayType;

                case "buttons":
                    return Field.Machine_Buttons;

                #endregion

                #region ListXML

                case "sourcefile":
                case "source file":
                case "source-file":
                    return Field.Machine_SourceFile;

                case "runnable":
                    return Field.Machine_Runnable;

                #endregion

                #region Logiqx

                case "board":
                    return Field.Machine_Board;

                case "rebuildto":
                case "rebuild to":
                case "rebuild-to":
                    return Field.Machine_RebuildTo;

                #endregion

                #region Logiqx EmuArc

                case "titleid":
                case "title id":
                case "title-id":
                    return Field.Machine_TitleID;

                case "developer":
                    return Field.Machine_Developer;

                case "genre":
                    return Field.Machine_Genre;

                case "subgenre":
                    return Field.Machine_Subgenre;

                case "ratings":
                    return Field.Machine_Ratings;

                case "score":
                    return Field.Machine_Score;

                case "enabled":
                    return Field.Machine_Enabled;

                case "hascrc":
                case "has crc":
                case "has-crc":
                    return Field.Machine_CRC;

                case "relatedto":
                case "related to":
                case "related-to":
                    return Field.Machine_RelatedTo;

                #endregion

                #region OpenMSX

                case "genmsxid":
                case "genmsx id":
                case "genmsx-id":
                case "gen msx id":
                case "gen-msx-id":
                    return Field.Machine_GenMSXID;

                case "system":
                case "msxsystem":
                case "msx system":
                case "msx-system":
                    return Field.Machine_System;

                case "country":
                    return Field.Machine_Country;

                #endregion

                #region SoftwareList

                case "supported":
                    return Field.Machine_Supported;

                #endregion

                #endregion // Machine

                #region DatItem

                #region Common

                case "itemname":
                case "item-name":
                case "name":
                    return Field.DatItem_Name;
                case "itemtype":
                case "item-type":
                case "type":
                    return Field.DatItem_Type;

                #endregion

                #region AttractMode

                case "altname":
                case "alt name":
                case "alt-name":
                case "altromname":
                case "alt romname":
                case "alt-romname":
                    return Field.DatItem_AltName;
                case "alttitle":
                case "alt title":
                case "alt-title":
                case "altromtitle":
                case "alt romtitle":
                case "alt-romtitle":
                    return Field.DatItem_AltTitle;

                #endregion

                #region OpenMSX

                case "original":
                    return Field.DatItem_Original;
                case "subtype":
                case "sub type":
                case "sub-type":
                case "openmsx_subtype":
                    return Field.DatItem_OpenMSXSubType;
                case "openmsx_type":
                    return Field.DatItem_OpenMSXType;
                case "remark":
                    return Field.DatItem_Remark;
                case "boot":
                    return Field.DatItem_Boot;

                #endregion

                #region SoftwareList

                case "partname":
                case "part name":
                case "part-name":
                    return Field.DatItem_Part_Name;
                case "partinterface":
                case "part interface":
                case "part-interface":
                    return Field.DatItem_Part_Interface;
                case "areaname":
                case "area name":
                case "area-name":
                    return Field.DatItem_AreaName;
                case "areasize":
                case "area size":
                case "area-size":
                    return Field.DatItem_AreaSize;
                case "areawidth":
                case "area width":
                case "area-width":
                    return Field.DatItem_AreaWidth;
                case "areaendinanness":
                case "area endianness":
                case "area-endianness":
                    return Field.DatItem_AreaEndianness;
                case "value":
                    return Field.DatItem_Value;
                case "loadflag":
                case "load flag":
                case "load-flag":
                    return Field.DatItem_LoadFlag;

                #endregion

                #region Item-Specific

                #region Actionable

                // Rom
                case "bios":
                    return Field.DatItem_Bios;

                case "equal":
                case "greater":
                case "less":
                case "size":
                    return Field.DatItem_Size;

                case "crc":
                case "crc32":
                    return Field.DatItem_CRC;

                case "md5":
                case "md5_hash":
                    return Field.DatItem_MD5;

#if NET_FRAMEWORK
                case "ripemd160":
                case "ripemd160_hash":
                    return Field.DatItem_RIPEMD160;
#endif

                case "sha1":
                case "sha_1":
                case "sha1hash":
                case "sha1_hash":
                case "sha_1hash":
                case "sha_1_hash":
                    return Field.DatItem_SHA1;

                case "sha256":
                case "sha_256":
                case "sha256hash":
                case "sha256_hash":
                case "sha_256hash":
                case "sha_256_hash":
                    return Field.DatItem_SHA256;

                case "sha384":
                case "sha_384":
                case "sha384hash":
                case "sha384_hash":
                case "sha_384hash":
                case "sha_384_hash":
                    return Field.DatItem_SHA384;

                case "sha512":
                case "sha_512":
                case "sha512hash":
                case "sha512_hash":
                case "sha_512hash":
                case "sha_512_hash":
                    return Field.DatItem_SHA512;

                case "merge":
                case "mergetag":
                case "merge_tag":
                    return Field.DatItem_Merge;

                case "region":
                    return Field.DatItem_Region;

                case "offset":
                    return Field.DatItem_Offset;

                case "date":
                    return Field.DatItem_Date;

                case "itemtatus":
                case "item-status":
                case "status":
                    return Field.DatItem_Status;

                case "optional":
                    return Field.DatItem_Optional;

                case "inverted":
                    return Field.DatItem_Inverted;

                // Disk
                case "index":
                    return Field.DatItem_Index;

                case "writable":
                    return Field.DatItem_Writable;

                #endregion

                #region Auxiliary

                // Adjuster
                case "default":
                    return Field.DatItem_Default;

                case "condition_tag":
                    return Field.DatItem_Condition_Tag;

                case "condition_mask":
                    return Field.DatItem_Condition_Mask;

                case "condition_relation":
                    return Field.DatItem_Condition_Relation;

                case "condition_value":
                    return Field.DatItem_Condition_Value;

                // BiosSet
                case "biosdescription":
                case "bios-description":
                case "biossetdescription":
                case "biosset-description":
                case "bios-set-description":
                    return Field.DatItem_Description;

                // Chip
                case "tag":
                    return Field.DatItem_Tag;

                case "chiptype":
                case "chip_type":
                    return Field.DatItem_ChipType;

                case "clock":
                    return Field.DatItem_Clock;
                    
                // Ram Option
                case "content":
                    return Field.DatItem_Content;

                // Release
                case "language":
                    return Field.DatItem_Language;

                #endregion

                #endregion // Item-Specific

                #endregion

                default:
                    return Field.NULL;
            }
        }

        /// <summary>
        /// Get ItemStatus value from input string
        /// </summary>
        /// <param name="status">String to get value from</param>
        /// <returns>ItemStatus value corresponding to the string</returns>
        public static ItemStatus AsItemStatus(this string status)
        {
#if NET_FRAMEWORK
            switch (status?.ToLowerInvariant())
            {
                case "good":
                    return ItemStatus.Good;
                case "baddump":
                    return ItemStatus.BadDump;
                case "nodump":
                case "yes":
                    return ItemStatus.Nodump;
                case "verified":
                    return ItemStatus.Verified;
                case "none":
                case "no":
                default:
                    return ItemStatus.None;
            }
#else
            return status?.ToLowerInvariant() switch
            {
                "good" => ItemStatus.Good,
                "baddump" => ItemStatus.BadDump,
                "nodump" => ItemStatus.Nodump,
                "yes" => ItemStatus.Nodump,
                "verified" => ItemStatus.Verified,
                "none" => ItemStatus.None,
                "no" => ItemStatus.None,
                _ => ItemStatus.None,
            };
#endif
        }

        /// <summary>
        /// Get ItemType? value from input string
        /// </summary>
        /// <param name="itemType">String to get value from</param>
        /// <returns>ItemType? value corresponding to the string</returns>
        public static ItemType? AsItemType(this string itemType)
        {
#if NET_FRAMEWORK
            switch (itemType?.ToLowerInvariant())
            {
                case "adjuster":
                    return ItemType.Adjuster;
                case "analog":
                    return ItemType.Analog;
                case "archive":
                    return ItemType.Archive;
                case "biosset":
                    return ItemType.BiosSet;
                case "blank":
                    return ItemType.Blank;
                case "chip":
                    return ItemType.Chip;
                case "condition":
                    return ItemType.Condition;
                case "configuration":
                    return ItemType.Configuration;
                case "control":
                    return ItemType.Control;
                case "device":
                    return ItemType.Device;
                case "device_ref":
                    return ItemType.DeviceReference;
                case "dipswitch":
                    return ItemType.DipSwitch;
                case "disk":
                    return ItemType.Disk;
                case "display":
                    return ItemType.Display;
                case "driver":
                    return ItemType.Driver;
                case "extension":
                    return ItemType.Extension;
                case "feature":
                    return ItemType.Feature;
                case "info":
                    return ItemType.Info;
                case "input":
                    return ItemType.Input;
                case "instance":
                    return ItemType.Instance;
                case "location":
                    return ItemType.Location;
                case "media":
                    return ItemType.Media;
                case "partfeature":
                case "part_feature":
                    return ItemType.PartFeature;
                case "port":
                    return ItemType.Port;
                case "ramoption":
                    return ItemType.RamOption;
                case "release":
                    return ItemType.Release;
                case "rom":
                    return ItemType.Rom;
                case "sample":
                    return ItemType.Sample;
                case "setting":
                    return ItemType.Setting;
                case "sharedfeat":
                    return ItemType.SharedFeature;
                case "slot":
                    return ItemType.Slot;
                case "slotoption":
                    return ItemType.SlotOption;
                case "softwarelist":
                    return ItemType.SoftwareList;
                case "sound":
                    return ItemType.Sound;
                default:
                    return null;
            }
#else
            return itemType?.ToLowerInvariant() switch
            {
                "adjuster" => ItemType.Adjuster,
                "analog" => ItemType.Analog,
                "archive" => ItemType.Archive,
                "biosset" => ItemType.BiosSet,
                "blank" => ItemType.Blank,
                "chip" => ItemType.Chip,
                "condition" => ItemType.Condition,
                "configuration" => ItemType.Configuration,
                "control" => ItemType.Control,
                "device" => ItemType.Device,
                "device_ref" => ItemType.DeviceReference,
                "dipswitch" => ItemType.DipSwitch,
                "disk" => ItemType.Disk,
                "display" => ItemType.Display,
                "driver" => ItemType.Driver,
                "extension" => ItemType.Extension,
                "feature" => ItemType.Feature,
                "info" => ItemType.Info,
                "input" => ItemType.Input,
                "instance" => ItemType.Instance,
                "location" => ItemType.Location,
                "media" => ItemType.Media,
                "partfeature" => ItemType.PartFeature,
                "part_feature" => ItemType.PartFeature,
                "port" => ItemType.Port,
                "ramoption" => ItemType.RamOption,
                "release" => ItemType.Release,
                "rom" => ItemType.Rom,
                "sample" => ItemType.Sample,
                "setting" => ItemType.Setting,
                "sharedfeat" => ItemType.SharedFeature,
                "slot" => ItemType.Slot,
                "slotoption" => ItemType.SlotOption,
                "softwarelist" => ItemType.SoftwareList,
                "sound" => ItemType.Sound,
                _ => null,
            };
#endif
        }

        /// <summary>
        /// Get MachineType value from input string
        /// </summary>
        /// <param name="gametype">String to get value from</param>
        /// <returns>MachineType value corresponding to the string</returns>
        public static MachineType AsMachineType(this string gametype)
        {
#if NET_FRAMEWORK
            switch (gametype?.ToLowerInvariant())
            {
                case "bios":
                    return MachineType.Bios;
                case "dev":
                case "device":
                    return MachineType.Device;
                case "mech":
                case "mechanical":
                    return MachineType.Mechanical;
                case "none":
                default:
                    return MachineType.NULL;
            }
#else
            return gametype?.ToLowerInvariant() switch
            {
                "bios" => MachineType.Bios,
                "dev" => MachineType.Device,
                "device" => MachineType.Device,
                "mech" => MachineType.Mechanical,
                "mechanical" => MachineType.Mechanical,
                "none" => MachineType.NULL,
                _ => MachineType.NULL,
            };
#endif
        }

        /// <summary>
        /// Get MergingFlag value from input string
        /// </summary>
        /// <param name="merging">String to get value from</param>
        /// <returns>MergingFlag value corresponding to the string</returns>
        public static MergingFlag AsMergingFlag(this string merging)
        {
#if NET_FRAMEWORK
            switch (merging?.ToLowerInvariant())
            {
                case "split":
                    return MergingFlag.Split;
                case "merged":
                    return MergingFlag.Merged;
                case "nonmerged":
                case "unmerged":
                    return MergingFlag.NonMerged;
                case "full":
                    return MergingFlag.Full;
                case "device":
                    return MergingFlag.Device;
                case "none":
                default:
                    return MergingFlag.None;
            }
#else
            return merging?.ToLowerInvariant() switch
            {
                "split" => MergingFlag.Split,
                "merged" => MergingFlag.Merged,
                "nonmerged" => MergingFlag.NonMerged,
                "unmerged" => MergingFlag.NonMerged,
                "full" => MergingFlag.Full,
                "none" => MergingFlag.None,
                _ => MergingFlag.None,
            };
#endif
        }

        /// <summary>
        /// Get NodumpFlag value from input string
        /// </summary>
        /// <param name="nodump">String to get value from</param>
        /// <returns>NodumpFlag value corresponding to the string</returns>
        public static NodumpFlag AsNodumpFlag(this string nodump)
        {
#if NET_FRAMEWORK
            switch (nodump?.ToLowerInvariant())
            {
                case "obsolete":
                    return NodumpFlag.Obsolete;
                case "required":
                    return NodumpFlag.Required;
                case "ignore":
                    return NodumpFlag.Ignore;
                case "none":
                default:
                    return NodumpFlag.None;
            }
#else
            return nodump?.ToLowerInvariant() switch
            {
                "obsolete" => NodumpFlag.Obsolete,
                "required" => NodumpFlag.Required,
                "ignore" => NodumpFlag.Ignore,
                "none" => NodumpFlag.None,
                _ => NodumpFlag.None,
            };
#endif
        }

        /// <summary>
        /// Get OpenMSXSubType value from input string
        /// </summary>
        /// <param name="itemType">String to get value from</param>
        /// <returns>OpenMSXSubType value corresponding to the string</returns>
        public static OpenMSXSubType AsOpenMSXSubType(this string itemType)
        {
#if NET_FRAMEWORK
            switch (itemType?.ToLowerInvariant())
            {
                case "rom":
                    return OpenMSXSubType.Rom;
                case "megarom":
                    return OpenMSXSubType.MegaRom;
                case "sccpluscart":
                    return OpenMSXSubType.SCCPlusCart;
                default:
                    return OpenMSXSubType.NULL;
            }
#else
            return itemType?.ToLowerInvariant() switch
            {
                "rom" => OpenMSXSubType.Rom,
                "megarom" => OpenMSXSubType.MegaRom,
                "sccpluscart" => OpenMSXSubType.SCCPlusCart,
                _ => OpenMSXSubType.NULL,
            };
#endif
        }

        /// <summary>
        /// Get PackingFlag value from input string
        /// </summary>
        /// <param name="packing">String to get value from</param>
        /// <returns>PackingFlag value corresponding to the string</returns>
        public static PackingFlag AsPackingFlag(this string packing)
        {
#if NET_FRAMEWORK
            switch (packing?.ToLowerInvariant())
            {
                case "yes":
                case "zip":
                    return PackingFlag.Zip;
                case "no":
                case "unzip":
                    return PackingFlag.Unzip;
                case "partial":
                    return PackingFlag.Partial;
                case "flat":
                    return PackingFlag.Flat;
                case "none":
                default:
                    return PackingFlag.None;
            }
#else
            return packing?.ToLowerInvariant() switch
            {
                "yes" => PackingFlag.Zip,
                "zip" => PackingFlag.Zip,
                "no" => PackingFlag.Unzip,
                "unzip" => PackingFlag.Unzip,
                "partial" => PackingFlag.Partial,
                "flat" => PackingFlag.Flat,
                "none" => PackingFlag.None,
                _ => PackingFlag.None,
            };
#endif
        }

        /// <summary>
        /// Get Runnable value from input string
        /// </summary>
        /// <param name="runnable">String to get value from</param>
        /// <returns>Runnable value corresponding to the string</returns>
        public static Runnable AsRunnable(this string runnable)
        {
#if NET_FRAMEWORK
            switch (runnable?.ToLowerInvariant())
            {
                case "no":
                    return Runnable.No;
                case "partial":
                    return Runnable.Partial;
                case "yes":
                    return Runnable.Yes;
                default:
                    return Runnable.NULL;
            }
#else
            return runnable?.ToLowerInvariant() switch
            {
                "no" => Runnable.No,
                "partial" => Runnable.Partial,
                "yes" => Runnable.Yes,
                _ => Runnable.NULL,
            };
#endif
        }

        /// <summary>
        /// Get SoftwareListStatus value from input string
        /// </summary>
        /// <param name="status">String to get value from</param>
        /// <returns>SoftwareListStatus value corresponding to the string</returns>
        public static SoftwareListStatus AsSoftwareListStatus(this string status)
        {
#if NET_FRAMEWORK
            switch (status?.ToLowerInvariant())
            {
                case "original":
                    return SoftwareListStatus.Original;
                case "compatible":
                    return SoftwareListStatus.Compatible;
                case "none":
                default:
                    return SoftwareListStatus.NULL;
            }
#else
            return status?.ToLowerInvariant() switch
            {
                "original" => SoftwareListStatus.Original,
                "compatible" => SoftwareListStatus.Compatible,
                "none" => SoftwareListStatus.NULL,
                _ => SoftwareListStatus.NULL,
            };
#endif
        }

        /// <summary>
        /// Get StatReportFormat value from input string
        /// </summary>
        /// <param name="input">String to get value from</param>
        /// <returns>StatReportFormat value corresponding to the string</returns>
        public static StatReportFormat AsStatReportFormat(this string input)
        {
#if NET_FRAMEWORK
            switch (input?.Trim().ToLowerInvariant())
            {
                case "all":
                    return StatReportFormat.All;
                case "csv":
                    return StatReportFormat.CSV;
                case "html":
                    return StatReportFormat.HTML;
                case "ssv":
                    return StatReportFormat.SSV;
                case "text":
                    return StatReportFormat.Textfile;
                case "tsv":
                    return StatReportFormat.TSV;
                default:
                    return 0x0;
            }
#else
            return input?.Trim().ToLowerInvariant() switch
            {
                "all" => StatReportFormat.All,
                "csv" => StatReportFormat.CSV,
                "html" => StatReportFormat.HTML,
                "ssv" => StatReportFormat.SSV,
                "text" => StatReportFormat.Textfile,
                "tsv" => StatReportFormat.TSV,
                _ => 0x0,
            };
#endif
        }

        /// <summary>
        /// Get Supported value from input string
        /// </summary>
        /// <param name="supported">String to get value from</param>
        /// <returns>Supported value corresponding to the string</returns>
        public static Supported AsSupported(this string supported)
        {
#if NET_FRAMEWORK
            switch (supported?.ToLowerInvariant())
            {
                case "no":
                case "unsupported":
                    return Supported.No;
                case "partial":
                    return Supported.Partial;
                case "yes":
                case "supported":
                    return Supported.Yes;
                default:
                    return Supported.NULL;
            }
#else
            return supported?.ToLowerInvariant() switch
            {
                "no" => Supported.No,
                "unsupported" => Supported.No,
                "partial" => Supported.Partial,
                "yes" => Supported.Yes,
                "supported" => Supported.Yes,
                _ => Supported.NULL,
            };
#endif
        }

        /// <summary>
        /// Get SupportStatus value from input string
        /// </summary>
        /// <param name="supported">String to get value from</param>
        /// <returns>SupportStatus value corresponding to the string</returns>
        public static SupportStatus AsSupportStatus(this string supportStatus)
        {
#if NET_FRAMEWORK
            switch (supportStatus?.ToLowerInvariant())
            {
                case "good":
                    return SupportStatus.Good;
                case "imperfect":
                    return SupportStatus.Imperfect;
                case "preliminary":
                    return SupportStatus.Preliminary;
                default:
                    return SupportStatus.NULL;
            }
#else
            return supportStatus?.ToLowerInvariant() switch
            {
                "good" => SupportStatus.Good,
                "imperfect" => SupportStatus.Imperfect,
                "preliminary" => SupportStatus.Preliminary,
                _ => SupportStatus.NULL,
            };
#endif
        }

        /// <summary>
        /// Get bool? value from input string
        /// </summary>
        /// <param name="yesno">String to get value from</param>
        /// <returns>bool? corresponding to the string</returns>
        public static bool? AsYesNo(this string yesno)
        {
#if NET_FRAMEWORK
            switch (yesno?.ToLowerInvariant())
            {
                case "yes":
                case "true":
                    return true;
                case "no":
                case "false":
                    return false;
                default:
                    return null;
            }
#else
            return yesno?.ToLowerInvariant() switch
            {
                "yes" => true,
                "true" => true,
                "no" => false,
                "false" => false,
                _ => null,
            };
#endif
        }

        #endregion

        #region Enum to String

        // TODO: DatFormat -> string
        // TODO: Field -> string

        /// <summary>
        /// Get string value from input ChipType
        /// </summary>
        /// <param name="chipType">ChipType to get value from</param>
        /// <returns>String value corresponding to the ChipType</returns>
        public static string FromChipType(this ChipType chipType)
        {
#if NET_FRAMEWORK
            switch (chipType)
            {
                case ChipType.CPU:
                    return "cpu";
                case ChipType.Audio:
                    return "audio";
                default:
                    return null;
            }
#else
            return chipType switch
            {
                ChipType.CPU => "cpu",
                ChipType.Audio => "audio",
                _ => null,
            };
#endif
        }

        /// <summary>
        /// Get string value from input FeatureStatus
        /// </summary>
        /// <param name="featureStatus">FeatureStatus to get value from</param>
        /// <returns>String value corresponding to the FeatureStatus</returns>
        public static string FromFeatureStatus(this FeatureStatus featureStatus)
        {
#if NET_FRAMEWORK
            switch (featureStatus)
            {
                case FeatureStatus.Unemulated:
                    return "unemulated";
                case FeatureStatus.Imperfect:
                    return "imperfect";
                default:
                    return null;
            }
#else
            return featureStatus switch
            {
                FeatureStatus.Unemulated => "unemulated",
                FeatureStatus.Imperfect => "imperfect",
                _ => null,
            };
#endif
        }

        /// <summary>
        /// Get string value from input FeatureType
        /// </summary>
        /// <param name="featureType">FeatureType to get value from</param>
        /// <returns>String value corresponding to the FeatureType</returns>
        public static string FromFeatureType(this FeatureType featureType)
        {
#if NET_FRAMEWORK
            switch (featureType)
            {
                case FeatureType.Protection:
                    return "protection";
                case FeatureType.Palette:
                    return "palette";
                case FeatureType.Graphics:
                    return "graphics";
                case FeatureType.Sound:
                    return "sound";
                case FeatureType.Controls:
                    return "controls";
                case FeatureType.Keyboard:
                    return "keyboard";
                case FeatureType.Mouse:
                    return "mouse";
                case FeatureType.Microphone:
                    return "microphone";
                case FeatureType.Camera:
                    return "camera";
                case FeatureType.Disk:
                    return "disk";
                case FeatureType.Printer:
                    return "printer";
                case FeatureType.Lan:
                    return "lan";
                case FeatureType.Wan:
                    return "wan";
                case FeatureType.Timing:
                    return "timing";
                default:
                    return null;
            }
#else
            return featureType switch
            {
                FeatureType.Protection => "protection",
                FeatureType.Palette => "palette",
                FeatureType.Graphics => "graphics",
                FeatureType.Sound => "sound",
                FeatureType.Controls => "controls",
                FeatureType.Keyboard => "keyboard",
                FeatureType.Mouse => "mouse",
                FeatureType.Microphone => "microphone",
                FeatureType.Camera => "camera",
                FeatureType.Disk => "disk",
                FeatureType.Printer => "printer",
                FeatureType.Lan => "lan",
                FeatureType.Wan => "wan",
                FeatureType.Timing => "timing",
                _ => null,
            };
#endif
        }

        /// <summary>
        /// Get string value from input ItemStatus
        /// </summary>
        /// <param name="status">ItemStatus to get value from</param>
        /// <param name="yesno">True to use Yes/No format instead</param>
        /// <returns>String value corresponding to the ItemStatus</returns>
        public static string FromItemStatus(this ItemStatus status, bool yesno)
        {
#if NET_FRAMEWORK
            switch (status)
            {
                case ItemStatus.Good:
                    return "good";
                case ItemStatus.BadDump:
                    return "baddump";
                case ItemStatus.Nodump:
                    return yesno ? "yes" : "nodump";
                case ItemStatus.Verified:
                    return "verified";
                default:
                    return null;
            }
#else
            return status switch
            {
                ItemStatus.Good => "good",
                ItemStatus.BadDump => "baddump",
                ItemStatus.Nodump => yesno ? "yes" : "nodump",
                ItemStatus.Verified => "verified",
                _ => null,
            };
#endif
        }

        /// <summary>
        /// Get string value from input ItemType?
        /// </summary>
        /// <param name="itemType">ItemType? to get value from</param>
        /// <returns>String value corresponding to the ItemType?</returns>
        public static string FromItemType(this ItemType? itemType)
        {
#if NET_FRAMEWORK
            switch (itemType)
            {
                case ItemType.Adjuster:
                    return "adjuster";
                case ItemType.Analog:
                    return "analog";
                case ItemType.Archive:
                    return "archive";
                case ItemType.BiosSet:
                    return "biosset";
                case ItemType.Blank:
                    return "blank";
                case ItemType.Chip:
                    return "chip";
                case ItemType.Condition:
                    return "condition";
                case ItemType.Configuration:
                    return "configuration";
                case ItemType.Control:
                    return "control";
                case ItemType.Device:
                    return "device";
                case ItemType.DeviceReference:
                    return "device_ref";
                case ItemType.DipSwitch:
                    return "dipswitch";
                case ItemType.Disk:
                    return "disk";
                case ItemType.Display:
                    return "display";
                case ItemType.Driver:
                    return "driver";
                case ItemType.Extension:
                    return "extension";
                case ItemType.Feature:
                    return "feature";
                case ItemType.Info:
                    return "info";
                case ItemType.Input:
                    return "input";
                case ItemType.Instance:
                    return "instance";
                case ItemType.Location:
                    return "location";
                case ItemType.Media:
                    return "media";
                case ItemType.PartFeature:
                    return "part_feature";
                case ItemType.Port:
                    return "port";
                case ItemType.RamOption:
                    return "ramoption";
                case ItemType.Release:
                    return "release";
                case ItemType.Rom:
                    return "rom";
                case ItemType.Sample:
                    return "sample";
                case ItemType.Setting:
                    return "setting";
                case ItemType.SharedFeature:
                    return "sharedfeat";
                case ItemType.Slot:
                    return "slot";
                case ItemType.SlotOption:
                    return "slotoption";
                case ItemType.SoftwareList:
                    return "softwarelist";
                case ItemType.Sound:
                    return "sound";
                default:
                    return null;
            }
#else
            return itemType switch
            {
                ItemType.Adjuster => "adjuster",
                ItemType.Analog => "analog",
                ItemType.Archive => "archive",
                ItemType.BiosSet => "biosset",
                ItemType.Blank => "blank",
                ItemType.Chip => "chip",
                ItemType.Condition => "condition",
                ItemType.Configuration => "configuration",
                ItemType.Control => "control",
                ItemType.Device => "device",
                ItemType.DeviceReference => "device_ref",
                ItemType.DipSwitch => "dipswitch",
                ItemType.Disk => "disk",
                ItemType.Display => "display",
                ItemType.Driver => "driver",
                ItemType.Extension => "extension",
                ItemType.Feature => "feature",
                ItemType.Info => "info",
                ItemType.Input => "input",
                ItemType.Instance => "instance",
                ItemType.Location => "location",
                ItemType.Media => "media",
                ItemType.PartFeature => "part_feature",
                ItemType.Port => "port",
                ItemType.RamOption => "ramoption",
                ItemType.Release => "release",
                ItemType.Rom => "rom",
                ItemType.Sample => "sample",
                ItemType.Setting => "setting",
                ItemType.SharedFeature => "sharedfeat",
                ItemType.Slot => "slot",
                ItemType.SlotOption => "slotoption",
                ItemType.SoftwareList => "softwarelist",
                ItemType.Sound => "sound",
                _ => null,
            };
#endif
        }

        /// <summary>
        /// Get string value from input MachineType
        /// </summary>
        /// <param name="gametype">MachineType to get value from</param>
        /// <param name="romCenter">True to use old naming instead</param>
        /// <returns>String value corresponding to the MachineType</returns>
        public static string FromMachineType(this MachineType gametype, bool old)
        {
#if NET_FRAMEWORK
            switch (gametype)
            {
                case MachineType.Bios:
                    return "bios";
                case MachineType.Device:
                    return old ? "dev" : "device";
                case MachineType.Mechanical:
                    return old ? "mech" : "mechanical";
                default:
                    return null;
            }
#else
            return gametype switch
            {
                MachineType.Bios => "bios",
                MachineType.Device => old ? "dev" : "device",
                MachineType.Mechanical => old ? "mech" : "mechanical",
                _ => null,
            };
#endif
        }

        /// <summary>
        /// Get string value from input MergingFlag
        /// </summary>
        /// <param name="merging">MergingFlag to get value from</param>
        /// <param name="romCenter">True to use RomCenter naming instead</param>
        /// <returns>String value corresponding to the MergingFlag</returns>
        public static string FromMergingFlag(this MergingFlag merging, bool romCenter)
        {
#if NET_FRAMEWORK
            switch (merging)
            {
                case MergingFlag.Split:
                    return "split";
                case MergingFlag.Merged:
                    return "merged";
                case MergingFlag.NonMerged:
                    return romCenter ? "unmerged" : "nonmerged";
                case MergingFlag.Full:
                    return "full";
                case MergingFlag.Device:
                    return "device";
                default:
                    return null;
            }
#else
            return merging switch
            {
                MergingFlag.Split => "split",
                MergingFlag.Merged => "merged",
                MergingFlag.NonMerged => romCenter ? "unmerged" : "nonmerged",
                MergingFlag.Full => "full",
                MergingFlag.Device => "device",
                _ => null,
            };
#endif
        }

        /// <summary>
        /// Get string value from input NodumpFlag
        /// </summary>
        /// <param name="nodump">NodumpFlag to get value from</param>
        /// <returns>String value corresponding to the NodumpFlag</returns>
        public static string FromNodumpFlag(this NodumpFlag nodump)
        {
#if NET_FRAMEWORK
            switch (nodump)
            {
                case NodumpFlag.Obsolete:
                    return "obsolete";
                case NodumpFlag.Required:
                    return "required";
                case NodumpFlag.Ignore:
                    return "ignore";
                default:
                    return null;
            }
#else
            return nodump switch
            {
                NodumpFlag.Obsolete => "obsolete",
                NodumpFlag.Required => "required",
                NodumpFlag.Ignore => "ignore",
                _ => null,
            };
#endif
        }

        /// <summary>
        /// Get string value from input OpenMSXSubType
        /// </summary>
        /// <param name="itemType">OpenMSXSubType to get value from</param>
        /// <returns>String value corresponding to the OpenMSXSubType</returns>
        public static string FromOpenMSXSubType(this OpenMSXSubType itemType)
        {
#if NET_FRAMEWORK
            switch (itemType)
            {
                case OpenMSXSubType.Rom:
                    return "rom";
                case OpenMSXSubType.MegaRom:
                    return "megarom";
                case OpenMSXSubType.SCCPlusCart:
                    return "sccpluscart";
                default:
                    return null;
            }
#else
            return itemType switch
            {
                OpenMSXSubType.Rom => "rom",
                OpenMSXSubType.MegaRom => "megarom",
                OpenMSXSubType.SCCPlusCart => "sccpluscart",
                _ => null,
            };
#endif
        }

        /// <summary>
        /// Get string value from input OutputFormat
        /// </summary>
        /// <param name="itemType">OutputFormat to get value from</param>
        /// <returns>String value corresponding to the OutputFormat</returns>
        public static string FromOutputFormat(this OutputFormat itemType)
        {
#if NET_FRAMEWORK
            switch (itemType)
            {
                case OutputFormat.Folder:
                case OutputFormat.ParentFolder:
                    return "directory";
                case OutputFormat.TapeArchive:
                    return "TAR";
                case OutputFormat.Torrent7Zip:
                    return "Torrent7Z";
                case OutputFormat.TorrentGzip:
                case OutputFormat.TorrentGzipRomba:
                    return "TorrentGZ";
                case OutputFormat.TorrentLRZip:
                    return "TorrentLRZ";
                case OutputFormat.TorrentRar:
                    return "TorrentRAR";
                case OutputFormat.TorrentXZ:
                case OutputFormat.TorrentXZRomba:
                    return "TorrentXZ";
                case OutputFormat.TorrentZip:
                    return "TorrentZip";
                default:
                    return null;
            }
#else
            return itemType switch
            {
                OutputFormat.Folder => "directory",
                OutputFormat.ParentFolder => "directory",
                OutputFormat.TapeArchive => "TAR",
                OutputFormat.Torrent7Zip => "Torrent7Z",
                OutputFormat.TorrentGzip => "TorrentGZ",
                OutputFormat.TorrentGzipRomba => "TorrentGZ",
                OutputFormat.TorrentLRZip => "TorrentLRZ",
                OutputFormat.TorrentRar => "TorrentRAR",
                OutputFormat.TorrentXZ => "TorrentXZ",
                OutputFormat.TorrentXZRomba => "TorrentXZ",
                OutputFormat.TorrentZip => "TorrentZip",
                _ => null,
            };
#endif
        }

        /// <summary>
        /// Get string value from input PackingFlag
        /// </summary>
        /// <param name="packing">PackingFlag to get value from</param>
        /// <param name="yesno">True to use Yes/No format instead</param>
        /// <returns>String value corresponding to the PackingFlag</returns>
        public static string FromPackingFlag(this PackingFlag packing, bool yesno)
        {
#if NET_FRAMEWORK
            switch (packing)
            {
                case PackingFlag.Zip:
                    return yesno ? "yes" : "zip";
                case PackingFlag.Unzip:
                    return yesno ? "no" : "unzip";
                case PackingFlag.Partial:
                    return "partial";
                case PackingFlag.Flat:
                    return "flat";
                default:
                    return null;
            }
#else
            return packing switch
            {
                PackingFlag.Zip => yesno ? "yes" : "zip",
                PackingFlag.Unzip => yesno ? "yes" : "zip",
                PackingFlag.Partial => "partial",
                PackingFlag.Flat => "flat",
                _ => null,
            };
#endif
        }

        /// <summary>
        /// Get string value from input Runnable
        /// </summary>
        /// <param name="runnable">Runnable to get value from</param>
        /// <returns>String value corresponding to the Runnable</returns>
        public static string FromRunnable(this Runnable runnable)
        {
#if NET_FRAMEWORK
            switch (runnable)
            {
                case Runnable.No:
                    return "no";
                case Runnable.Partial:
                    return "partial";
                case Runnable.Yes:
                    return "yes";
                default:
                    return null;
            }
#else
            return runnable switch
            {
                Runnable.No => "no",
                Runnable.Partial => "partial",
                Runnable.Yes => "yes",
                _ => null,
            };
#endif
        }

        /// <summary>
        /// Get string value from input SoftwareListStatus
        /// </summary>
        /// <param name="status">SoftwareListStatus to get value from</param>
        /// <returns>String value corresponding to the SoftwareListStatus</returns>
        public static string FromSoftwareListStatus(this SoftwareListStatus status)
        {
#if NET_FRAMEWORK
            switch (status)
            {
                case SoftwareListStatus.Original:
                    return "original";
                case SoftwareListStatus.Compatible:
                    return "compatible";
                default:
                    return null;
            }
#else
            return status switch
            {
                SoftwareListStatus.Original => "original",
                SoftwareListStatus.Compatible => "compatible",
                _ => null,
            };
#endif
        }

        /// <summary>
        /// Get string value from input StatReportFormat
        /// </summary>
        /// <param name="input">StatReportFormat to get value from</param>
        /// <returns>String value corresponding to the StatReportFormat</returns>
        public static string FromStatReportFormat(this StatReportFormat input)
        {
#if NET_FRAMEWORK
            switch (input)
            {
                case StatReportFormat.All:
                    return "all";
                case StatReportFormat.CSV:
                    return "csv";
                case StatReportFormat.HTML:
                    return "html";
                case StatReportFormat.SSV:
                    return "ssv";
                case StatReportFormat.Textfile:
                    return "text";
                case StatReportFormat.TSV:
                    return "tsv";
                default:
                    return null;
            }
#else
            return input switch
            {
                StatReportFormat.All => "all",
                StatReportFormat.CSV => "csv",
                StatReportFormat.HTML => "html",
                StatReportFormat.SSV => "ssv",
                StatReportFormat.Textfile => "text",
                StatReportFormat.TSV => "tsv",
                _ => null,
            };
#endif
        }

        /// <summary>
        /// Get string value from input Supported
        /// </summary>
        /// <param name="supported">Supported to get value from</param>
        /// <param name="verbose">True to use verbose output, false otherwise</param>
        /// <returns>String value corresponding to the Supported</returns>
        public static string FromSupported(this Supported supported, bool verbose)
        {
#if NET_FRAMEWORK
            switch (supported)
            {
                case Supported.No:
                    return verbose ? "unsupported" : "no";
                case Supported.Partial:
                    return "partial";
                case Supported.Yes:
                    return verbose ? "supported" : "yes";
                default:
                    return null;
            }
#else
            return supported switch
            {
                Supported.No => "no",
                Supported.Partial => "partial",
                Supported.Yes => "yes",
                _ => null,
            };
#endif
        }

        /// <summary>
        /// Get string value from input SupportStatus
        /// </summary>
        /// <param name="supportStatus">SupportStatus to get value from</param>
        /// <returns>String value corresponding to the SupportStatus</returns>
        public static string FromSupportStatus(this SupportStatus supportStatus)
        {
#if NET_FRAMEWORK
            switch (supportStatus)
            {
                case SupportStatus.Good:
                    return "good";
                case SupportStatus.Imperfect:
                    return "imperfect";
                case SupportStatus.Preliminary:
                    return "preliminary";
                default:
                    return null;
            }
#else
            return supportStatus switch
            {
                SupportStatus.Good => "good",
                SupportStatus.Imperfect => "imperfect",
                SupportStatus.Preliminary => "preliminary",
                _ => null,
            };
#endif
        }

        /// <summary>
        /// Get string value from input bool?
        /// </summary>
        /// <param name="yesno">bool? to get value from</param>
        /// <returns>String corresponding to the bool?</returns>
        public static string FromYesNo(this bool? yesno)
        {
#if NET_FRAMEWORK
            switch (yesno)
            {
                case true:
                    return "yes";
                case false:
                    return "no";
                default:
                    return null;
            }
#else
            return yesno switch
            {
                true => "yes",
                false => "no",
                _ => null,
            };
#endif
        }

        #endregion
    }
}
