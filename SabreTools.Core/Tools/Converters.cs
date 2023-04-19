using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SabreTools.Core.Tools
{
    public static class Converters
    {
        #region Enum to Enum

        /// <summary>
        /// Get the DatItemFields associated with each hash type
        /// </summary>
        public static List<DatItemField> AsDatItemFields(this Hash hash)
        {
            List<DatItemField> fields = new();

            if (hash.HasFlag(Hash.CRC))
                fields.Add(DatItemField.CRC);
            if (hash.HasFlag(Hash.MD5))
                fields.Add(DatItemField.MD5);
            if (hash.HasFlag(Hash.SHA1))
                fields.Add(DatItemField.SHA1);
            if (hash.HasFlag(Hash.SHA256))
                fields.Add(DatItemField.SHA256);
            if (hash.HasFlag(Hash.SHA384))
                fields.Add(DatItemField.SHA384);
            if (hash.HasFlag(Hash.SHA512))
                fields.Add(DatItemField.SHA512);
            if (hash.HasFlag(Hash.SpamSum))
                fields.Add(DatItemField.SpamSum);

            return fields;
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
            return chipType?.ToLowerInvariant() switch
            {
                "cpu" => ChipType.CPU,
                "audio" => ChipType.Audio,
                _ => ChipType.NULL,
            };
        }

        /// <summary>
        /// Get ControlType value from input string
        /// </summary>
        /// <param name="controlType">String to get value from</param>
        /// <returns>ControlType value corresponding to the string</returns>
        public static ControlType AsControlType(this string controlType)
        {
            return controlType?.ToLowerInvariant() switch
            {
                "joy" => ControlType.Joy,
                "stick" => ControlType.Stick,
                "paddle" => ControlType.Paddle,
                "pedal" => ControlType.Pedal,
                "lightgun" => ControlType.Lightgun,
                "positional" => ControlType.Positional,
                "dial" => ControlType.Dial,
                "trackball" => ControlType.Trackball,
                "mouse" => ControlType.Mouse,
                "only_buttons" => ControlType.OnlyButtons,
                "keypad" => ControlType.Keypad,
                "keyboard" => ControlType.Keyboard,
                "mahjong" => ControlType.Mahjong,
                "hanafuda" => ControlType.Hanafuda,
                "gambling" => ControlType.Gambling,
                _ => ControlType.NULL,
            };
        }

        /// <summary>
        /// Get DatHeaderField value from input string
        /// </summary>
        /// <param name="DatHeaderField">String to get value from</param>
        /// <returns>DatHeaderField value corresponding to the string</returns>
        public static DatHeaderField AsDatHeaderField(this string input)
        {
            // If the input is empty, we return null
            if (string.IsNullOrEmpty(input))
                return DatHeaderField.NULL;

            // Normalize the input
            input = input.ToLowerInvariant();

            // Create regex
            string headerRegex = @"^(dat|header|datheader)[.\-_\s]";

            // If we don't have a header field, skip
            if (!Regex.IsMatch(input, headerRegex))
                return DatHeaderField.NULL;

            // Replace the match and re-normalize
            string headerInput = Regex.Replace(input, headerRegex, string.Empty)
                .Replace(' ', '_')
                .Replace('-', '_')
                .Replace('.', '_');

            return headerInput switch
            {
                #region Common

                "file" => DatHeaderField.FileName,
                "filename" => DatHeaderField.FileName,
                "file_name" => DatHeaderField.FileName,

                "dat" => DatHeaderField.Name,
                "datname" => DatHeaderField.Name,
                "dat_name" => DatHeaderField.Name,
                "internalname" => DatHeaderField.Name,
                "internal_name" => DatHeaderField.Name,

                "desc" => DatHeaderField.Description,
                "description" => DatHeaderField.Description,

                "root" => DatHeaderField.RootDir,
                "rootdir" => DatHeaderField.RootDir,
                "root_dir" => DatHeaderField.RootDir,
                "rootdirectory" => DatHeaderField.RootDir,
                "root_directory" => DatHeaderField.RootDir,

                "category" => DatHeaderField.Category,

                "version" => DatHeaderField.Version,

                "date" => DatHeaderField.Date,
                "timestamp" => DatHeaderField.Date,
                "time_stamp" => DatHeaderField.Date,

                "author" => DatHeaderField.Author,

                "email" => DatHeaderField.Email,
                "e_mail" => DatHeaderField.Email,

                "homepage" => DatHeaderField.Homepage,
                "home_page" => DatHeaderField.Homepage,

                "url" => DatHeaderField.Url,

                "comment" => DatHeaderField.Comment,

                "header" => DatHeaderField.HeaderSkipper,
                "headerskipper" => DatHeaderField.HeaderSkipper,
                "header_skipper" => DatHeaderField.HeaderSkipper,
                "skipper" => DatHeaderField.HeaderSkipper,

                "dattype" => DatHeaderField.Type,
                "type" => DatHeaderField.Type,
                "superdat" => DatHeaderField.Type,

                "forcemerging" => DatHeaderField.ForceMerging,
                "force_merging" => DatHeaderField.ForceMerging,

                "forcenodump" => DatHeaderField.ForceNodump,
                "force_nodump" => DatHeaderField.ForceNodump,

                "forcepacking" => DatHeaderField.ForcePacking,
                "force_packing" => DatHeaderField.ForcePacking,

                #endregion

                #region ListXML

                "debug" => DatHeaderField.Debug,

                "mameconfig" => DatHeaderField.MameConfig,
                "mame_config" => DatHeaderField.MameConfig,

                #endregion

                #region Logiqx

                "id" => DatHeaderField.NoIntroID,
                "nointroid" => DatHeaderField.NoIntroID,
                "no_intro_id" => DatHeaderField.NoIntroID,

                "build" => DatHeaderField.Build,

                "rommode" => DatHeaderField.RomMode,
                "rom_mode" => DatHeaderField.RomMode,

                "biosmode" => DatHeaderField.BiosMode,
                "bios_mode" => DatHeaderField.BiosMode,

                "samplemode" => DatHeaderField.SampleMode,
                "sample_mode" => DatHeaderField.SampleMode,

                "lockrommode" => DatHeaderField.LockRomMode,
                "lockrom_mode" => DatHeaderField.LockRomMode,
                "lock_rommode" => DatHeaderField.LockRomMode,
                "lock_rom_mode" => DatHeaderField.LockRomMode,

                "lockbiosmode" => DatHeaderField.LockBiosMode,
                "lockbios_mode" => DatHeaderField.LockBiosMode,
                "lock_biosmode" => DatHeaderField.LockBiosMode,
                "lock_bios_mode" => DatHeaderField.LockBiosMode,

                "locksamplemode" => DatHeaderField.LockSampleMode,
                "locksample_mode" => DatHeaderField.LockSampleMode,
                "lock_samplemode" => DatHeaderField.LockSampleMode,
                "lock_sample_mode" => DatHeaderField.LockSampleMode,

                #endregion

                #region OfflineList

                "system" => DatHeaderField.System,
                "plugin" => DatHeaderField.System, // Used with RomCenter

                "screenshotwidth" => DatHeaderField.ScreenshotsWidth,
                "screenshotswidth" => DatHeaderField.ScreenshotsWidth,
                "screenshot_width" => DatHeaderField.ScreenshotsWidth,
                "screenshots_width" => DatHeaderField.ScreenshotsWidth,

                "screenshotheight" => DatHeaderField.ScreenshotsHeight,
                "screenshotsheight" => DatHeaderField.ScreenshotsHeight,
                "screenshot_height" => DatHeaderField.ScreenshotsHeight,
                "screenshots_height" => DatHeaderField.ScreenshotsHeight,

                "info_name" => DatHeaderField.Info_Name,
                "infos_name" => DatHeaderField.Info_Name,

                "info_visible" => DatHeaderField.Info_Visible,
                "infos_visible" => DatHeaderField.Info_Visible,

                "info_isnamingoption" => DatHeaderField.Info_IsNamingOption,
                "info_is_naming_option" => DatHeaderField.Info_IsNamingOption,
                "infos_isnamingoption" => DatHeaderField.Info_IsNamingOption,
                "infos_is_naming_option" => DatHeaderField.Info_IsNamingOption,

                "info_default" => DatHeaderField.Info_Default,
                "infos_default" => DatHeaderField.Info_Default,

                "canopen" => DatHeaderField.CanOpen,
                "can_open" => DatHeaderField.CanOpen,

                "romtitle" => DatHeaderField.RomTitle,
                "rom_title" => DatHeaderField.RomTitle,

                #endregion

                #region RomCenter

                "rcversion" => DatHeaderField.RomCenterVersion,
                "rc_version" => DatHeaderField.RomCenterVersion,
                "romcenterversion" => DatHeaderField.RomCenterVersion,
                "romcenter_version" => DatHeaderField.RomCenterVersion,
                "rom_center_version" => DatHeaderField.RomCenterVersion,

                #endregion

                _ => DatHeaderField.NULL,
            };
        }

        /// <summary>
        /// Get DatItemField value from input string
        /// </summary>
        /// <param name="input">String to get value from</param>
        /// <returns>DatItemField value corresponding to the string</returns>
        public static DatItemField AsDatItemField(this string input)
        {
            // If the input is empty, we return null
            if (string.IsNullOrEmpty(input))
                return DatItemField.NULL;

            // Normalize the input
            input = input.ToLowerInvariant();

            // Create regex
            string datItemRegex = @"^(item|datitem)[.\-_\s]";

            // If we don't have an item field, skip
            if (!Regex.IsMatch(input, datItemRegex))
                return DatItemField.NULL;

            // Replace the match and re-normalize
            string itemInput = Regex.Replace(input, datItemRegex, string.Empty)
                .Replace(' ', '_')
                .Replace('-', '_')
                .Replace('.', '_');

            return itemInput switch
            {
                #region Common

                "type" => DatItemField.Type,

                #endregion

                #region Item-Specific

                #region Actionable

                #region Rom

                "name" => DatItemField.Name,

                "bios" => DatItemField.Bios,

                "size" => DatItemField.Size,

                "crc" => DatItemField.CRC,
                "crc32" => DatItemField.CRC,

                "md5" => DatItemField.MD5,
                "md5hash" => DatItemField.MD5,
                "md5_hash" => DatItemField.MD5,

                "sha1" => DatItemField.SHA1,
                "sha_1" => DatItemField.SHA1,
                "sha1hash" => DatItemField.SHA1,
                "sha1_hash" => DatItemField.SHA1,
                "sha_1hash" => DatItemField.SHA1,
                "sha_1_hash" => DatItemField.SHA1,

                "sha256" => DatItemField.SHA256,
                "sha_256" => DatItemField.SHA256,
                "sha256hash" => DatItemField.SHA256,
                "sha256_hash" => DatItemField.SHA256,
                "sha_256hash" => DatItemField.SHA256,
                "sha_256_hash" => DatItemField.SHA256,

                "sha384" => DatItemField.SHA384,
                "sha_384" => DatItemField.SHA384,
                "sha384hash" => DatItemField.SHA384,
                "sha384_hash" => DatItemField.SHA384,
                "sha_384hash" => DatItemField.SHA384,
                "sha_384_hash" => DatItemField.SHA384,

                "sha512" => DatItemField.SHA512,
                "sha_512" => DatItemField.SHA512,
                "sha512hash" => DatItemField.SHA512,
                "sha512_hash" => DatItemField.SHA512,
                "sha_512hash" => DatItemField.SHA512,
                "sha_512_hash" => DatItemField.SHA512,

                "spamsum" => DatItemField.SpamSum,
                "spam_sum" => DatItemField.SpamSum,

                "merge" => DatItemField.Merge,
                "mergetag" => DatItemField.Merge,
                "merge_tag" => DatItemField.Merge,

                "region" => DatItemField.Region,

                "offset" => DatItemField.Offset,

                "date" => DatItemField.Date,

                "status" => DatItemField.Status,

                "optional" => DatItemField.Optional,

                "inverted" => DatItemField.Inverted,

                #endregion

                #region Rom (Archive.org)

                "ado_source" => DatItemField.ArchiveDotOrgSource,

                "ado_format" => DatItemField.ArchiveDotOrgFormat,

                "original_filename" => DatItemField.OriginalFilename,

                "rotation" => DatItemField.Rotation,

                "summation" => DatItemField.Summation,

                #endregion

                #region Rom (AttractMode)

                "altname" => DatItemField.AltName,
                "alt_name" => DatItemField.AltName,
                "altromname" => DatItemField.AltName,
                "alt_romname" => DatItemField.AltName,
                "alt_rom_name" => DatItemField.AltName,

                "alttitle" => DatItemField.AltTitle,
                "alt_title" => DatItemField.AltTitle,
                "altromtitle" => DatItemField.AltTitle,
                "alt_romtitle" => DatItemField.AltTitle,
                "alt_rom_title" => DatItemField.AltTitle,

                #endregion

                #region Rom (Logiqx)

                "mia" => DatItemField.MIA,

                #endregion

                #region Rom (OpenMSX)

                "original" => DatItemField.Original,

                "subtype" => DatItemField.OpenMSXSubType,
                "sub_type" => DatItemField.OpenMSXSubType,
                "openmsxsubtype" => DatItemField.OpenMSXSubType,
                "openmsx_subtype" => DatItemField.OpenMSXSubType,
                "openmsx_sub_type" => DatItemField.OpenMSXSubType,

                "openmsxtype" => DatItemField.OpenMSXType,
                "openmsx_type" => DatItemField.OpenMSXType,

                "remark" => DatItemField.Remark,

                "boot" => DatItemField.Boot,

                #endregion

                #region Rom (SoftwareList)

                "areaname" => DatItemField.AreaName,
                "area_name" => DatItemField.AreaName,

                "areasize" => DatItemField.AreaSize,
                "area_size" => DatItemField.AreaSize,

                "areawidth" => DatItemField.AreaWidth,
                "area_width" => DatItemField.AreaWidth,

                "areaendinanness" => DatItemField.AreaEndianness,
                "area_endianness" => DatItemField.AreaEndianness,

                "loadflag" => DatItemField.LoadFlag,
                "load_flag" => DatItemField.LoadFlag,

                "partname" => DatItemField.Part_Name,
                "part_name" => DatItemField.Part_Name,

                "partinterface" => DatItemField.Part_Interface,
                "part_interface" => DatItemField.Part_Interface,

                "part_feature_name" => DatItemField.Part_Feature_Name,

                "part_feature_value" => DatItemField.Part_Feature_Value,

                "value" => DatItemField.Value,

                #endregion

                #region Disk

                "index" => DatItemField.Index,

                "writable" => DatItemField.Writable,

                #endregion

                #endregion

                #region Auxiliary

                #region Adjuster

                "default" => DatItemField.Default,

                #endregion

                #region Analog

                "analog_mask" => DatItemField.Analog_Mask,

                #endregion

                #region Archive

                "number" => DatItemField.Number,

                "clone" => DatItemField.Clone,

                "regparent" => DatItemField.RegParent,
                "reg_parent" => DatItemField.RegParent,

                "languages" => DatItemField.Languages,

                "devstatus" => DatItemField.DevStatus,
                "dev_status" => DatItemField.DevStatus,

                "physical" => DatItemField.Physical,

                "complete" => DatItemField.Complete,

                "categories" => DatItemField.Categories,

                #endregion

                #region BiosSet

                "description" => DatItemField.Description,
                "biosdescription" => DatItemField.Description,
                "bios_description" => DatItemField.Description,

                #endregion

                #region Chip

                "tag" => DatItemField.Tag,

                "chiptype" => DatItemField.ChipType,
                "chip_type" => DatItemField.ChipType,

                "clock" => DatItemField.Clock,

                #endregion

                #region Condition

                "mask" => DatItemField.Mask,

                "relation" => DatItemField.Relation,

                "condition_tag" => DatItemField.Condition_Tag,

                "condition_mask" => DatItemField.Condition_Mask,

                "condition_relation" => DatItemField.Condition_Relation,

                "condition_value" => DatItemField.Condition_Value,

                #endregion

                #region Control

                "control_type" => DatItemField.Control_Type,

                "control_player" => DatItemField.Control_Player,

                "control_buttons" => DatItemField.Control_Buttons,

                "control_reqbuttons" => DatItemField.Control_RequiredButtons,
                "control_req_buttons" => DatItemField.Control_RequiredButtons,

                "control_minimum" => DatItemField.Control_Minimum,

                "control_maximum" => DatItemField.Control_Maximum,

                "control_sensitivity" => DatItemField.Control_Sensitivity,

                "control_keydelta" => DatItemField.Control_KeyDelta,
                "control_key_delta" => DatItemField.Control_KeyDelta,

                "control_reverse" => DatItemField.Control_Reverse,

                "control_ways" => DatItemField.Control_Ways,

                "control_ways2" => DatItemField.Control_Ways2,

                "control_ways3" => DatItemField.Control_Ways3,

                #endregion

                #region Device

                "devicetype" => DatItemField.DeviceType,
                "device_type" => DatItemField.DeviceType,

                "fixedimage" => DatItemField.FixedImage,
                "fixed_image" => DatItemField.FixedImage,

                "mandatory" => DatItemField.Mandatory,

                "interface" => DatItemField.Interface,

                #endregion

                #region Display

                "displaytype" => DatItemField.DisplayType,
                "display_type" => DatItemField.DisplayType,

                "rotate" => DatItemField.Rotate,

                "flipx" => DatItemField.FlipX,

                "width" => DatItemField.Width,

                "height" => DatItemField.Height,

                "refresh" => DatItemField.Refresh,

                "pixclock" => DatItemField.PixClock,
                "pix_clock" => DatItemField.PixClock,

                "htotal" => DatItemField.HTotal,

                "hbend" => DatItemField.HBEnd,

                "hbstart" => DatItemField.HBStart,

                "vtotal" => DatItemField.VTotal,

                "vbend" => DatItemField.VBEnd,

                "vbstart" => DatItemField.VBStart,

                #endregion

                #region Driver

                "supportstatus" => DatItemField.SupportStatus,
                "support_status" => DatItemField.SupportStatus,

                "emulationstatus" => DatItemField.EmulationStatus,
                "emulation_status" => DatItemField.EmulationStatus,

                "cocktailstatus" => DatItemField.CocktailStatus,
                "cocktail_status" => DatItemField.CocktailStatus,

                "savestatestatus" => DatItemField.SaveStateStatus,
                "savestate_status" => DatItemField.SaveStateStatus,
                "save_state_status" => DatItemField.SaveStateStatus,

                "requiresartwork" => DatItemField.RequiresArtwork,
                "requires_artwork" => DatItemField.RequiresArtwork,

                "unofficial" => DatItemField.Unofficial,

                "nosoundhardware" => DatItemField.NoSoundHardware,
                "no_sound_hardware" => DatItemField.NoSoundHardware,

                "incomplete" => DatItemField.Incomplete,

                #endregion

                #region Extension

                "extension_name" => DatItemField.Extension_Name,

                #endregion

                #region Feature

                "featuretype" => DatItemField.FeatureType,
                "feature_type" => DatItemField.FeatureType,

                "featurestatus" => DatItemField.FeatureStatus,
                "feature_status" => DatItemField.FeatureStatus,

                "featureoverall" => DatItemField.FeatureOverall,
                "feature_overall" => DatItemField.FeatureOverall,

                #endregion

                #region Input

                "service" => DatItemField.Service,

                "tilt" => DatItemField.Tilt,

                "players" => DatItemField.Players,

                "coins" => DatItemField.Coins,

                #endregion

                #region Instance

                "instance_name" => DatItemField.Instance_Name,

                "instance_briefname" => DatItemField.Instance_BriefName,
                "instance_brief_name" => DatItemField.Instance_BriefName,

                #endregion

                #region Location

                "location_name" => DatItemField.Location_Name,

                "location_number" => DatItemField.Location_Number,

                "location_inverted" => DatItemField.Location_Inverted,

                #endregion

                #region RamOption

                "content" => DatItemField.Content,

                #endregion

                #region Release

                "language" => DatItemField.Language,

                #endregion

                #region Setting

                "setting_name" => DatItemField.Setting_Name,
                "value_name" => DatItemField.Setting_Name,

                "setting_value" => DatItemField.Setting_Value,
                "value_value" => DatItemField.Setting_Value,

                "setting_default" => DatItemField.Setting_Default,
                "value_default" => DatItemField.Setting_Default,

                #endregion

                #region SlotOption

                "slotoption_name" => DatItemField.SlotOption_Name,

                "slotoption_devicename" => DatItemField.SlotOption_DeviceName,
                "slotoption_device_name" => DatItemField.SlotOption_DeviceName,

                "slotoption_default" => DatItemField.SlotOption_Default,

                #endregion

                #region SoftwareList

                "softwareliststatus" => DatItemField.SoftwareListStatus,
                "softwarelist_status" => DatItemField.SoftwareListStatus,

                "filter" => DatItemField.Filter,

                #endregion

                #region Sound

                "channels" => DatItemField.Channels,

                #endregion

                #endregion

                #endregion // Item-Specific

                _ => DatItemField.NULL,
            };
        }

        /// <summary>
        /// Get DeviceType value from input string
        /// </summary>
        /// <param name="deviceType">String to get value from</param>
        /// <returns>DeviceType value corresponding to the string</returns>
        public static DeviceType AsDeviceType(this string deviceType)
        {
            return deviceType?.ToLowerInvariant() switch
            {
                "unknown" => DeviceType.Unknown,
                "cartridge" => DeviceType.Cartridge,
                "floppydisk" => DeviceType.FloppyDisk,
                "harddisk" => DeviceType.HardDisk,
                "cylinder" => DeviceType.Cylinder,
                "cassette" => DeviceType.Cassette,
                "punchcard" => DeviceType.PunchCard,
                "punchtape" => DeviceType.PunchTape,
                "printout" => DeviceType.Printout,
                "serial" => DeviceType.Serial,
                "parallel" => DeviceType.Parallel,
                "snapshot" => DeviceType.Snapshot,
                "quickload" => DeviceType.QuickLoad,
                "memcard" => DeviceType.MemCard,
                "cdrom" => DeviceType.CDROM,
                "magtape" => DeviceType.MagTape,
                "romimage" => DeviceType.ROMImage,
                "midiin" => DeviceType.MIDIIn,
                "midiout" => DeviceType.MIDIOut,
                "picture" => DeviceType.Picture,
                "vidfile" => DeviceType.VidFile,
                _ => DeviceType.NULL,
            };
        }

        /// <summary>
        /// Get DisplayType value from input string
        /// </summary>
        /// <param name="displayType">String to get value from</param>
        /// <returns>DisplayType value corresponding to the string</returns>
        public static DisplayType AsDisplayType(this string displayType)
        {
            return displayType?.ToLowerInvariant() switch
            {
                "raster" => DisplayType.Raster,
                "vector" => DisplayType.Vector,
                "lcd" => DisplayType.LCD,
                "svg" => DisplayType.SVG,
                "unknown" => DisplayType.Unknown,
                _ => DisplayType.NULL,
            };
        }

        /// <summary>
        /// Get Endianness value from input string
        /// </summary>
        /// <param name="endianness">String to get value from</param>
        /// <returns>Endianness value corresponding to the string</returns>
        public static Endianness AsEndianness(this string endianness)
        {
            return endianness?.ToLowerInvariant() switch
            {
                "big" => Endianness.Big,
                "little" => Endianness.Little,
                _ => Endianness.NULL,
            };
        }

        /// <summary>
        /// Get FeatureStatus value from input string
        /// </summary>
        /// <param name="featureStatus">String to get value from</param>
        /// <returns>FeatureStatus value corresponding to the string</returns>
        public static FeatureStatus AsFeatureStatus(this string featureStatus)
        {
            return featureStatus?.ToLowerInvariant() switch
            {
                "unemulated" => FeatureStatus.Unemulated,
                "imperfect" => FeatureStatus.Imperfect,
                _ => FeatureStatus.NULL,
            };
        }

        /// <summary>
        /// Get FeatureType value from input string
        /// </summary>
        /// <param name="emulationStatus">String to get value from</param>
        /// <returns>FeatureType value corresponding to the string</returns>
        public static FeatureType AsFeatureType(this string featureType)
        {
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
        }

        /// <summary>
        /// Get ItemStatus value from input string
        /// </summary>
        /// <param name="status">String to get value from</param>
        /// <returns>ItemStatus value corresponding to the string</returns>
        public static ItemStatus AsItemStatus(this string status)
        {
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
        }

        /// <summary>
        /// Get ItemType? value from input string
        /// </summary>
        /// <param name="itemType">String to get value from</param>
        /// <returns>ItemType? value corresponding to the string</returns>
        public static ItemType? AsItemType(this string itemType)
        {
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
                "dataarea" => ItemType.DataArea,
                "device" => ItemType.Device,
                "deviceref" => ItemType.DeviceReference,
                "device_ref" => ItemType.DeviceReference,
                "dipswitch" => ItemType.DipSwitch,
                "disk" => ItemType.Disk,
                "diskarea" => ItemType.DiskArea,
                "display" => ItemType.Display,
                "driver" => ItemType.Driver,
                "extension" => ItemType.Extension,
                "feature" => ItemType.Feature,
                "info" => ItemType.Info,
                "input" => ItemType.Input,
                "instance" => ItemType.Instance,
                "location" => ItemType.Location,
                "media" => ItemType.Media,
                "part" => ItemType.Part,
                "partfeature" => ItemType.PartFeature,
                "part_feature" => ItemType.PartFeature,
                "port" => ItemType.Port,
                "ramoption" => ItemType.RamOption,
                "release" => ItemType.Release,
                "releasedetails" => ItemType.ReleaseDetails,
                "release_details" => ItemType.ReleaseDetails,
                "rom" => ItemType.Rom,
                "sample" => ItemType.Sample,
                "serials" => ItemType.Serials,
                "setting" => ItemType.Setting,
                "sharedfeat" => ItemType.SharedFeature,
                "slot" => ItemType.Slot,
                "slotoption" => ItemType.SlotOption,
                "softwarelist" => ItemType.SoftwareList,
                "sound" => ItemType.Sound,
                "sourcedetails" => ItemType.SourceDetails,
                "source_details" => ItemType.SourceDetails,
                _ => null,
            };
        }

        /// <summary>
        /// Get LoadFlag value from input string
        /// </summary>
        /// <param name="loadFlag">String to get value from</param>
        /// <returns>LoadFlag value corresponding to the string</returns>
        public static LoadFlag AsLoadFlag(this string loadFlag)
        {
            return loadFlag?.ToLowerInvariant() switch
            {
                "load16_byte" => LoadFlag.Load16Byte,
                "load16_word" => LoadFlag.Load16Word,
                "load16_word_swap" => LoadFlag.Load16WordSwap,
                "load32_byte" => LoadFlag.Load32Byte,
                "load32_word" => LoadFlag.Load32Word,
                "load32_word_swap" => LoadFlag.Load32WordSwap,
                "load32_dword" => LoadFlag.Load32DWord,
                "load64_word" => LoadFlag.Load64Word,
                "load64_word_swap" => LoadFlag.Load64WordSwap,
                "reload" => LoadFlag.Reload,
                "fill" => LoadFlag.Fill,
                "continue" => LoadFlag.Continue,
                "reload_plain" => LoadFlag.ReloadPlain,
                "ignore" => LoadFlag.Ignore,
                _ => LoadFlag.NULL,
            };
        }

        /// <summary>
        /// Get LogLevel value from input string
        /// </summary>
        /// <param name="logLevel">String to get value from</param>
        /// <returns>LogLevel value corresponding to the string</returns>
        public static LogLevel AsLogLevel(this string logLevel)
        {
            return logLevel?.ToLowerInvariant() switch
            {
                "verbose" => LogLevel.VERBOSE,
                "user" => LogLevel.USER,
                "warning" => LogLevel.WARNING,
                "error" => LogLevel.ERROR,
                _ => LogLevel.VERBOSE,
            };
        }

        /// <summary>
        /// Get MachineField value from input string
        /// </summary>
        /// <param name="input">String to get value from</param>
        /// <returns>MachineField value corresponding to the string</returns>
        public static MachineField AsMachineField(this string input)
        {
            // If the input is empty, we return null
            if (string.IsNullOrEmpty(input))
                return MachineField.NULL;

            // Normalize the input
            input = input.ToLowerInvariant();

            // Create regex
            string machineRegex = @"^(game|machine)[.\-_\s]";

            // If we don't have a machine field, skip
            if (!Regex.IsMatch(input, machineRegex))
                return MachineField.NULL;

            // Replace the match and re-normalize
            string machineInput = Regex.Replace(input, machineRegex, string.Empty)
                .Replace(' ', '_')
                .Replace('-', '_')
                .Replace('.', '_');

            return machineInput switch
            {
                #region Common

                "name" => MachineField.Name,

                "comment" => MachineField.Comment,
                "extra" => MachineField.Comment, // Used with AttractMode

                "desc" => MachineField.Description,
                "description" => MachineField.Description,

                "year" => MachineField.Year,

                "manufacturer" => MachineField.Manufacturer,

                "publisher" => MachineField.Publisher,

                "category" => MachineField.Category,

                "romof" => MachineField.RomOf,
                "rom_of" => MachineField.RomOf,

                "cloneof" => MachineField.CloneOf,
                "clone_of" => MachineField.CloneOf,

                "sampleof" => MachineField.SampleOf,
                "sample_of" => MachineField.SampleOf,
                
                "type" => MachineField.Type,

                #endregion

                #region AttractMode
                
                "players" => MachineField.Players,

                "rotation" => MachineField.Rotation,

                "control" => MachineField.Control,

                "amstatus" => MachineField.Status,
                "am_status" => MachineField.Status,
                "gamestatus" => MachineField.Status,
                "supportstatus" => MachineField.Status,
                "support_status" => MachineField.Status,

                "displaycount" => MachineField.DisplayCount,
                "display_count" => MachineField.DisplayCount,

                "displaytype" => MachineField.DisplayType,
                "display_type" => MachineField.DisplayType,

                "buttons" => MachineField.Buttons,

                #endregion

                #region ListXML

                "history" => MachineField.History,

                "sourcefile" => MachineField.SourceFile,
                "source_file" => MachineField.SourceFile,

                "runnable" => MachineField.Runnable,

                #endregion

                #region Logiqx

                "board" => MachineField.Board,

                "rebuildto" => MachineField.RebuildTo,
                "rebuild_to" => MachineField.RebuildTo,

                "id" => MachineField.NoIntroId,
                "nointroid" => MachineField.NoIntroId,
                "nointro_id" => MachineField.NoIntroId,
                "no_intro_id" => MachineField.NoIntroId,

                "cloneofid" => MachineField.NoIntroCloneOfId,
                "nointrocloneofid" => MachineField.NoIntroCloneOfId,
                "nointro_cloneofid" => MachineField.NoIntroCloneOfId,
                "no_intro_cloneofid" => MachineField.NoIntroCloneOfId,
                "no_intro_clone_of_id" => MachineField.NoIntroCloneOfId,

                #endregion

                #region Logiqx EmuArc

                "titleid" => MachineField.TitleID,
                "title_id" => MachineField.TitleID,

                "developer" => MachineField.Developer,

                "genre" => MachineField.Genre,

                "subgenre" => MachineField.Subgenre,
                "sub_genre" => MachineField.Subgenre,

                "ratings" => MachineField.Ratings,

                "score" => MachineField.Score,

                "enabled" => MachineField.Enabled,

                "crc" => MachineField.CRC,
                "hascrc" => MachineField.CRC,
                "has_crc" => MachineField.CRC,

                "relatedto" => MachineField.RelatedTo,
                "related_to" => MachineField.RelatedTo,

                #endregion

                #region OpenMSX

                "genmsxid" => MachineField.GenMSXID,
                "genmsx_id" => MachineField.GenMSXID,
                "gen_msxid" => MachineField.GenMSXID,
                "gen_msx_id" => MachineField.GenMSXID,

                "system" => MachineField.System,
                "msxsystem" => MachineField.System,
                "msx_system" => MachineField.System,
                
                "country" => MachineField.Country,

                #endregion

                #region SoftwareList
                
                "supported" => MachineField.Supported,

                #endregion

                _ => MachineField.NULL,
            };
        }

        /// <summary>
        /// Get MachineType value from input string
        /// </summary>
        /// <param name="gametype">String to get value from</param>
        /// <returns>MachineType value corresponding to the string</returns>
        public static MachineType AsMachineType(this string gametype)
        {
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
        }

        /// <summary>
        /// Get MergingFlag value from input string
        /// </summary>
        /// <param name="merging">String to get value from</param>
        /// <returns>MergingFlag value corresponding to the string</returns>
        public static MergingFlag AsMergingFlag(this string merging)
        {
            return merging?.ToLowerInvariant() switch
            {
                "split" => MergingFlag.Split,
                "merged" => MergingFlag.Merged,
                "fullmerged" => MergingFlag.FullMerged,
                "nonmerged" => MergingFlag.NonMerged,
                "unmerged" => MergingFlag.NonMerged,
                "full" => MergingFlag.FullNonMerged,
                "fullnonmerged" => MergingFlag.FullNonMerged,
                "fullunmerged" => MergingFlag.FullNonMerged,
                "device" => MergingFlag.DeviceNonMerged,
                "devicenonmerged" => MergingFlag.DeviceNonMerged,
                "deviceunmerged" => MergingFlag.DeviceNonMerged,
                "none" => MergingFlag.None,
                _ => MergingFlag.None,
            };
        }

        /// <summary>
        /// Get NodumpFlag value from input string
        /// </summary>
        /// <param name="nodump">String to get value from</param>
        /// <returns>NodumpFlag value corresponding to the string</returns>
        public static NodumpFlag AsNodumpFlag(this string nodump)
        {
            return nodump?.ToLowerInvariant() switch
            {
                "obsolete" => NodumpFlag.Obsolete,
                "required" => NodumpFlag.Required,
                "ignore" => NodumpFlag.Ignore,
                "none" => NodumpFlag.None,
                _ => NodumpFlag.None,
            };
        }

        /// <summary>
        /// Get OpenMSXSubType value from input string
        /// </summary>
        /// <param name="itemType">String to get value from</param>
        /// <returns>OpenMSXSubType value corresponding to the string</returns>
        public static OpenMSXSubType AsOpenMSXSubType(this string itemType)
        {
            return itemType?.ToLowerInvariant() switch
            {
                "rom" => OpenMSXSubType.Rom,
                "megarom" => OpenMSXSubType.MegaRom,
                "sccpluscart" => OpenMSXSubType.SCCPlusCart,
                _ => OpenMSXSubType.NULL,
            };
        }

        /// <summary>
        /// Get PackingFlag value from input string
        /// </summary>
        /// <param name="packing">String to get value from</param>
        /// <returns>PackingFlag value corresponding to the string</returns>
        public static PackingFlag AsPackingFlag(this string packing)
        {
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
        }

        /// <summary>
        /// Get Relation value from input string
        /// </summary>
        /// <param name="relation">String to get value from</param>
        /// <returns>Relation value corresponding to the string</returns>
        public static Relation AsRelation(this string relation)
        {
            return relation?.ToLowerInvariant() switch
            {
                "eq" => Relation.Equal,
                "ne" => Relation.NotEqual,
                "gt" => Relation.GreaterThan,
                "le" => Relation.LessThanOrEqual,
                "lt" => Relation.LessThan,
                "ge" => Relation.GreaterThanOrEqual,
                _ => Relation.NULL,
            };
        }

        /// <summary>
        /// Get Runnable value from input string
        /// </summary>
        /// <param name="runnable">String to get value from</param>
        /// <returns>Runnable value corresponding to the string</returns>
        public static Runnable AsRunnable(this string runnable)
        {
            return runnable?.ToLowerInvariant() switch
            {
                "no" => Runnable.No,
                "partial" => Runnable.Partial,
                "yes" => Runnable.Yes,
                _ => Runnable.NULL,
            };
        }

        /// <summary>
        /// Get SoftwareListStatus value from input string
        /// </summary>
        /// <param name="status">String to get value from</param>
        /// <returns>SoftwareListStatus value corresponding to the string</returns>
        public static SoftwareListStatus AsSoftwareListStatus(this string status)
        {
            return status?.ToLowerInvariant() switch
            {
                "original" => SoftwareListStatus.Original,
                "compatible" => SoftwareListStatus.Compatible,
                "none" => SoftwareListStatus.NULL,
                _ => SoftwareListStatus.NULL,
            };
        }

        /// <summary>
        /// Get Supported value from input string
        /// </summary>
        /// <param name="supported">String to get value from</param>
        /// <returns>Supported value corresponding to the string</returns>
        public static Supported AsSupported(this string supported)
        {
            return supported?.ToLowerInvariant() switch
            {
                "no" => Supported.No,
                "unsupported" => Supported.No,
                "partial" => Supported.Partial,
                "yes" => Supported.Yes,
                "supported" => Supported.Yes,
                _ => Supported.NULL,
            };
        }

        /// <summary>
        /// Get SupportStatus value from input string
        /// </summary>
        /// <param name="supported">String to get value from</param>
        /// <returns>SupportStatus value corresponding to the string</returns>
        public static SupportStatus AsSupportStatus(this string supportStatus)
        {
            return supportStatus?.ToLowerInvariant() switch
            {
                "good" => SupportStatus.Good,
                "imperfect" => SupportStatus.Imperfect,
                "preliminary" => SupportStatus.Preliminary,
                _ => SupportStatus.NULL,
            };
        }

        /// <summary>
        /// Get bool? value from input string
        /// </summary>
        /// <param name="yesno">String to get value from</param>
        /// <returns>bool? corresponding to the string</returns>
        public static bool? AsYesNo(this string yesno)
        {
            return yesno?.ToLowerInvariant() switch
            {
                "yes" => true,
                "true" => true,
                "no" => false,
                "false" => false,
                _ => null,
            };
        }

        #endregion

        #region Enum to String

        /// <summary>
        /// Get string value from input ChipType
        /// </summary>
        /// <param name="chipType">ChipType to get value from</param>
        /// <returns>String value corresponding to the ChipType</returns>
        public static string FromChipType(this ChipType chipType)
        {
            return chipType switch
            {
                ChipType.CPU => "cpu",
                ChipType.Audio => "audio",
                _ => null,
            };
        }

        /// <summary>
        /// Get string value from input ControlType
        /// </summary>
        /// <param name="controlType">ControlType to get value from</param>
        /// <returns>String value corresponding to the ControlType</returns>
        public static string FromControlType(this ControlType controlType)
        {
            return controlType switch
            {
                ControlType.Joy => "joy",
                ControlType.Stick => "stick",
                ControlType.Paddle => "paddle",
                ControlType.Pedal => "pedal",
                ControlType.Lightgun => "lightgun",
                ControlType.Positional => "positional",
                ControlType.Dial => "dial",
                ControlType.Trackball => "trackball",
                ControlType.Mouse => "mouse",
                ControlType.OnlyButtons => "only_buttons",
                ControlType.Keypad => "keypad",
                ControlType.Keyboard => "keyboard",
                ControlType.Mahjong => "mahjong",
                ControlType.Hanafuda => "hanafuda",
                ControlType.Gambling => "gambling",
                _ => null,
            };
        }

        /// <summary>
        /// Get string value from input DeviceType
        /// </summary>
        /// <param name="deviceType">vDeviceType to get value from</param>
        /// <returns>String value corresponding to the DeviceType</returns>
        public static string FromDeviceType(this DeviceType deviceType)
        {
            return deviceType switch
            {
                DeviceType.Unknown => "unknown",
                DeviceType.Cartridge => "cartridge",
                DeviceType.FloppyDisk => "floppydisk",
                DeviceType.HardDisk => "harddisk",
                DeviceType.Cylinder => "cylinder",
                DeviceType.Cassette => "cassette",
                DeviceType.PunchCard => "punchcard",
                DeviceType.PunchTape => "punchtape",
                DeviceType.Printout => "printout",
                DeviceType.Serial => "serial",
                DeviceType.Parallel => "parallel",
                DeviceType.Snapshot => "snapshot",
                DeviceType.QuickLoad => "quickload",
                DeviceType.MemCard => "memcard",
                DeviceType.CDROM => "cdrom",
                DeviceType.MagTape => "magtape",
                DeviceType.ROMImage => "romimage",
                DeviceType.MIDIIn => "midiin",
                DeviceType.MIDIOut => "midiout",
                DeviceType.Picture => "picture",
                DeviceType.VidFile => "vidfile",
                _ => null,
            };
        }

        /// <summary>
        /// Get string value from input DisplayType
        /// </summary>
        /// <param name="displayType">DisplayType to get value from</param>
        /// <returns>String value corresponding to the DisplayType</returns>
        public static string FromDisplayType(this DisplayType displayType)
        {
            return displayType switch
            {
                DisplayType.Raster => "raster",
                DisplayType.Vector => "vector",
                DisplayType.LCD => "lcd",
                DisplayType.SVG => "svg",
                DisplayType.Unknown => "unknown",
                _ => null,
            };
        }

        /// <summary>
        /// Get string value from input Endianness
        /// </summary>
        /// <param name="endianness">Endianness to get value from</param>
        /// <returns>String value corresponding to the Endianness</returns>
        public static string FromEndianness(this Endianness endianness)
        {
            return endianness switch
            {
                Endianness.Big => "big",
                Endianness.Little => "little",
                _ => null,
            };
        }

        /// <summary>
        /// Get string value from input FeatureStatus
        /// </summary>
        /// <param name="featureStatus">FeatureStatus to get value from</param>
        /// <returns>String value corresponding to the FeatureStatus</returns>
        public static string FromFeatureStatus(this FeatureStatus featureStatus)
        {
            return featureStatus switch
            {
                FeatureStatus.Unemulated => "unemulated",
                FeatureStatus.Imperfect => "imperfect",
                _ => null,
            };
        }

        /// <summary>
        /// Get string value from input FeatureType
        /// </summary>
        /// <param name="featureType">FeatureType to get value from</param>
        /// <returns>String value corresponding to the FeatureType</returns>
        public static string FromFeatureType(this FeatureType featureType)
        {
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
        }

        /// <summary>
        /// Get string value from input ItemStatus
        /// </summary>
        /// <param name="status">ItemStatus to get value from</param>
        /// <param name="yesno">True to use Yes/No format instead</param>
        /// <returns>String value corresponding to the ItemStatus</returns>
        public static string FromItemStatus(this ItemStatus status, bool yesno)
        {
            return status switch
            {
                ItemStatus.Good => "good",
                ItemStatus.BadDump => "baddump",
                ItemStatus.Nodump => yesno ? "yes" : "nodump",
                ItemStatus.Verified => "verified",
                _ => null,
            };
        }

        /// <summary>
        /// Get string value from input LoadFlag
        /// </summary>
        /// <param name="loadFlag">LoadFlag to get value from</param>
        /// <returns>String value corresponding to the LoadFlag</returns>
        public static string FromLoadFlag(this LoadFlag loadFlag)
        {
            return loadFlag switch
            {
                LoadFlag.Load16Byte => "load16_byte",
                LoadFlag.Load16Word => "load16_word",
                LoadFlag.Load16WordSwap => "load16_word_swap",
                LoadFlag.Load32Byte => "load32_byte",
                LoadFlag.Load32Word => "load32_word",
                LoadFlag.Load32WordSwap => "load32_word_swap",
                LoadFlag.Load32DWord => "load32_dword",
                LoadFlag.Load64Word => "load64_word",
                LoadFlag.Load64WordSwap => "load64_word_swap",
                LoadFlag.Reload => "reload",
                LoadFlag.Fill => "fill",
                LoadFlag.Continue => "continue",
                LoadFlag.ReloadPlain => "reload_plain",
                LoadFlag.Ignore => "ignore",
                _ => null,
            };
        }

        /// <summary>
        /// Get string value from input ItemType?
        /// </summary>
        /// <param name="itemType">ItemType? to get value from</param>
        /// <returns>String value corresponding to the ItemType?</returns>
        public static string FromItemType(this ItemType? itemType)
        {
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
                ItemType.DataArea => "dataarea",
                ItemType.Device => "device",
                ItemType.DeviceReference => "device_ref",
                ItemType.DipSwitch => "dipswitch",
                ItemType.Disk => "disk",
                ItemType.DiskArea => "diskarea",
                ItemType.Display => "display",
                ItemType.Driver => "driver",
                ItemType.Extension => "extension",
                ItemType.Feature => "feature",
                ItemType.Info => "info",
                ItemType.Input => "input",
                ItemType.Instance => "instance",
                ItemType.Location => "location",
                ItemType.Media => "media",
                ItemType.Part => "part",
                ItemType.PartFeature => "part_feature",
                ItemType.Port => "port",
                ItemType.RamOption => "ramoption",
                ItemType.Release => "release",
                ItemType.ReleaseDetails => "release_details",
                ItemType.Rom => "rom",
                ItemType.Sample => "sample",
                ItemType.Serials => "serials",
                ItemType.Setting => "setting",
                ItemType.SharedFeature => "sharedfeat",
                ItemType.Slot => "slot",
                ItemType.SlotOption => "slotoption",
                ItemType.SoftwareList => "softwarelist",
                ItemType.Sound => "sound",
                ItemType.SourceDetails => "source_details",
                _ => null,
            };
        }

        /// <summary>
        /// Get string value from input MachineType
        /// </summary>
        /// <param name="gametype">MachineType to get value from</param>
        /// <param name="romCenter">True to use old naming instead</param>
        /// <returns>String value corresponding to the MachineType</returns>
        public static string FromMachineType(this MachineType gametype, bool old)
        {
            return gametype switch
            {
                MachineType.Bios => "bios",
                MachineType.Device => old ? "dev" : "device",
                MachineType.Mechanical => old ? "mech" : "mechanical",
                _ => null,
            };
        }

        /// <summary>
        /// Get string value from input MergingFlag
        /// </summary>
        /// <param name="merging">MergingFlag to get value from</param>
        /// <param name="romCenter">True to use RomCenter naming instead</param>
        /// <returns>String value corresponding to the MergingFlag</returns>
        public static string FromMergingFlag(this MergingFlag merging, bool romCenter)
        {
            return merging switch
            {
                MergingFlag.Split => "split",
                MergingFlag.Merged => "merged",
                MergingFlag.FullMerged => "fullmerged",
                MergingFlag.NonMerged => romCenter ? "unmerged" : "nonmerged",
                MergingFlag.FullNonMerged => "full",
                MergingFlag.DeviceNonMerged => "device",
                _ => null,
            };
        }

        /// <summary>
        /// Get string value from input NodumpFlag
        /// </summary>
        /// <param name="nodump">NodumpFlag to get value from</param>
        /// <returns>String value corresponding to the NodumpFlag</returns>
        public static string FromNodumpFlag(this NodumpFlag nodump)
        {
            return nodump switch
            {
                NodumpFlag.Obsolete => "obsolete",
                NodumpFlag.Required => "required",
                NodumpFlag.Ignore => "ignore",
                _ => null,
            };
        }

        /// <summary>
        /// Get string value from input OpenMSXSubType
        /// </summary>
        /// <param name="itemType">OpenMSXSubType to get value from</param>
        /// <returns>String value corresponding to the OpenMSXSubType</returns>
        public static string FromOpenMSXSubType(this OpenMSXSubType itemType)
        {
            return itemType switch
            {
                OpenMSXSubType.Rom => "rom",
                OpenMSXSubType.MegaRom => "megarom",
                OpenMSXSubType.SCCPlusCart => "sccpluscart",
                _ => null,
            };
        }

        /// <summary>
        /// Get string value from input PackingFlag
        /// </summary>
        /// <param name="packing">PackingFlag to get value from</param>
        /// <param name="yesno">True to use Yes/No format instead</param>
        /// <returns>String value corresponding to the PackingFlag</returns>
        public static string FromPackingFlag(this PackingFlag packing, bool yesno)
        {
            return packing switch
            {
                PackingFlag.Zip => yesno ? "yes" : "zip",
                PackingFlag.Unzip => yesno ? "no" : "unzip",
                PackingFlag.Partial => "partial",
                PackingFlag.Flat => "flat",
                _ => null,
            };
        }

        /// <summary>
        /// Get string value from input Relation
        /// </summary>
        /// <param name="relation">Relation to get value from</param>
        /// <returns>String value corresponding to the Relation</returns>
        public static string FromRelation(this Relation relation)
        {
            return relation switch
            {
                Relation.Equal => "eq",
                Relation.NotEqual => "ne",
                Relation.GreaterThan => "gt",
                Relation.LessThanOrEqual => "le",
                Relation.LessThan => "lt",
                Relation.GreaterThanOrEqual => "ge",
                _ => null,
            };
        }

        /// <summary>
        /// Get string value from input Runnable
        /// </summary>
        /// <param name="runnable">Runnable to get value from</param>
        /// <returns>String value corresponding to the Runnable</returns>
        public static string FromRunnable(this Runnable runnable)
        {
            return runnable switch
            {
                Runnable.No => "no",
                Runnable.Partial => "partial",
                Runnable.Yes => "yes",
                _ => null,
            };
        }

        /// <summary>
        /// Get string value from input SoftwareListStatus
        /// </summary>
        /// <param name="status">SoftwareListStatus to get value from</param>
        /// <returns>String value corresponding to the SoftwareListStatus</returns>
        public static string FromSoftwareListStatus(this SoftwareListStatus status)
        {
            return status switch
            {
                SoftwareListStatus.Original => "original",
                SoftwareListStatus.Compatible => "compatible",
                _ => null,
            };
        }

        /// <summary>
        /// Get string value from input Supported
        /// </summary>
        /// <param name="supported">Supported to get value from</param>
        /// <param name="verbose">True to use verbose output, false otherwise</param>
        /// <returns>String value corresponding to the Supported</returns>
        public static string FromSupported(this Supported supported, bool verbose)
        {
            return supported switch
            {
                Supported.No => verbose ? "unsupported" : "no",
                Supported.Partial => "partial",
                Supported.Yes => verbose ? "supported" : "yes",
                _ => null,
            };
        }

        /// <summary>
        /// Get string value from input SupportStatus
        /// </summary>
        /// <param name="supportStatus">SupportStatus to get value from</param>
        /// <returns>String value corresponding to the SupportStatus</returns>
        public static string FromSupportStatus(this SupportStatus supportStatus)
        {
            return supportStatus switch
            {
                SupportStatus.Good => "good",
                SupportStatus.Imperfect => "imperfect",
                SupportStatus.Preliminary => "preliminary",
                _ => null,
            };
        }

        /// <summary>
        /// Get string value from input bool?
        /// </summary>
        /// <param name="yesno">bool? to get value from</param>
        /// <returns>String corresponding to the bool?</returns>
        public static string FromYesNo(this bool? yesno)
        {
            return yesno switch
            {
                true => "yes",
                false => "no",
                _ => null,
            };
        }

        #endregion
    }
}
