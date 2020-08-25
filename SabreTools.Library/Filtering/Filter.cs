using System;
using System.Collections.Generic;

using SabreTools.Library.Data;
using SabreTools.Library.DatFiles;
using SabreTools.Library.DatItems;
using SabreTools.Library.Tools;

namespace SabreTools.Library.Filtering
{
    /// <summary>
    /// Represents the filtering operations that need to be performed on a set of items, usually a DAT
    /// </summary>
    /// TODO: Can clever use of Filtering allow for easier external splitting methods?
    /// TODO: Field name for filter population needs to be overhauled
    public class Filter
    {
        #region Fields

        #region Machine Filters

        #region Common

        public FilterItem<string> Machine_Name { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Comment { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Description { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Year { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Manufacturer { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Publisher { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Category { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_RomOf { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_CloneOf { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_SampleOf { get; private set; } = new FilterItem<string>();
        public FilterItem<MachineType> Machine_Type { get; private set; } = new FilterItem<MachineType>() { Positive = MachineType.NULL, Negative = MachineType.NULL };

        #endregion

        #region AttractMode

        public FilterItem<string> Machine_Players { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Rotation { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Control { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Status { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_DisplayCount { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_DisplayType { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Buttons { get; private set; } = new FilterItem<string>();

        #endregion

        #region ListXML

        public FilterItem<string> Machine_SourceFile { get; private set; } = new FilterItem<string>();
        public FilterItem<Runnable> Machine_Runnable { get; private set; } = new FilterItem<Runnable>() { Positive = Runnable.NULL, Negative = Runnable.NULL };
        
        // DeviceReferences
        public FilterItem<bool?> Machine_DeviceReferences { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> Machine_DeviceReference_Name { get; private set; } = new FilterItem<string>();
        
        // Chips
        public FilterItem<bool?> Machine_Chips { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> Machine_Chip_Name { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Chip_Tag { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Chip_Type { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Chip_Clock { get; private set; } = new FilterItem<string>();
        
        // Displays
        public FilterItem<bool?> Machine_Displays { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> Machine_Display_Tag { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Display_Type { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Display_Rotate { get; private set; } = new FilterItem<string>();
        public FilterItem<bool?> Machine_Display_FlipX { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> Machine_Display_Width { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Display_Height { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Display_Refresh { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Display_PixClock { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Display_HTotal { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Display_HBEnd { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Display_HBStart { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Display_VTotal { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Display_VBEnd { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Display_VBStart { get; private set; } = new FilterItem<string>();

        // Sounds
        public FilterItem<bool?> Machine_Sounds { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> Machine_Sound_Channels { get; private set; } = new FilterItem<string>();

        // Conditions
        public FilterItem<bool?> Machine_Conditions { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> Machine_Condition_Tag { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Condition_Mask { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Condition_Relation { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Condition_Value { get; private set; } = new FilterItem<string>();

        // Inputs
        public FilterItem<bool?> Machine_Inputs { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<bool?> Machine_Input_Service { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<bool?> Machine_Input_Tilt { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> Machine_Input_Players { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Input_Coins { get; private set; } = new FilterItem<string>();

        // Inputs.Controls
        public FilterItem<bool?> Machine_Input_Controls { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> Machine_Input_Control_Type { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Input_Control_Player { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Input_Control_Buttons { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Input_Control_RegButtons { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Input_Control_Minimum { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Input_Control_Maximum { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Input_Control_Sensitivity { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Input_Control_KeyDelta { get; private set; } = new FilterItem<string>();
        public FilterItem<bool?> Machine_Input_Control_Reverse { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> Machine_Input_Control_Ways { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Input_Control_Ways2 { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Input_Control_Ways3 { get; private set; } = new FilterItem<string>();

        // DipSwitches
        public FilterItem<bool?> Machine_DipSwitches { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> Machine_DipSwitch_Name { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_DipSwitch_Tag { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_DipSwitch_Mask { get; private set; } = new FilterItem<string>();

        // DipSwitches.Locations
        public FilterItem<bool?> Machine_DipSwitch_Locations { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> Machine_DipSwitch_Location_Name { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_DipSwitch_Location_Number { get; private set; } = new FilterItem<string>();
        public FilterItem<bool?> Machine_DipSwitch_Location_Inverted { get; private set; } = new FilterItem<bool?>() { Neutral = null };

        // DipSwitches.Values
        public FilterItem<bool?> Machine_DipSwitch_Values { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> Machine_DipSwitch_Value_Name { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_DipSwitch_Value_Value { get; private set; } = new FilterItem<string>();
        public FilterItem<bool?> Machine_DipSwitch_Value_Default { get; private set; } = new FilterItem<bool?>() { Neutral = null };

        // Configurations
        public FilterItem<bool?> Machine_Configurations { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> Machine_Configuration_Name { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Configuration_Tag { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Configuration_Mask { get; private set; } = new FilterItem<string>();

        // Configurations.Locations
        public FilterItem<bool?> Machine_Configuration_Locations { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> Machine_Configuration_Location_Name { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Configuration_Location_Number { get; private set; } = new FilterItem<string>();
        public FilterItem<bool?> Machine_Configuration_Location_Inverted { get; private set; } = new FilterItem<bool?>() { Neutral = null };

        // Configurations.Settings
        public FilterItem<bool?> Machine_Configuration_Settings { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> Machine_Configuration_Setting_Name { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Configuration_Setting_Value { get; private set; } = new FilterItem<string>();
        public FilterItem<bool?> Machine_Configuration_Setting_Default { get; private set; } = new FilterItem<bool?>() { Neutral = null };

        // Ports
        public FilterItem<bool?> Machine_Ports { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> Machine_Port_Tag { get; private set; } = new FilterItem<string>();

        // Ports.Analogs
        public FilterItem<bool?> Machine_Port_Analogs { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> Machine_Port_Analog_Mask { get; private set; } = new FilterItem<string>();

        // Adjusters
        public FilterItem<bool?> Machine_Adjusters { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> Machine_Adjuster_Name { get; private set; } = new FilterItem<string>();
        public FilterItem<bool?> Machine_Adjuster_Default { get; private set; } = new FilterItem<bool?>() { Neutral = null };

        // Adjusters.Conditions
        public FilterItem<bool?> Machine_Adjuster_Conditions { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> Machine_Adjuster_Condition_Tag { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Adjuster_Condition_Mask { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Adjuster_Condition_Relation { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Adjuster_Condition_Value { get; private set; } = new FilterItem<string>();

        // Drivers
        public FilterItem<bool?> Machine_Drivers { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> Machine_Driver_Status { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Driver_Emulation { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Driver_Cocktail { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Driver_SaveState { get; private set; } = new FilterItem<string>();

        // Features
        public FilterItem<bool?> Machine_Features { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> Machine_Feature_Type { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Feature_Status { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Feature_Overall { get; private set; } = new FilterItem<string>();

        // Devices
        public FilterItem<bool?> Machine_Devices { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> Machine_Device_Type { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Device_Tag { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Device_FixedImage { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Device_Mandatory { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Device_Interface { get; private set; } = new FilterItem<string>();

        // Devices.Instances
        public FilterItem<bool?> Machine_Device_Instances { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> Machine_Device_Instance_Name { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Device_Instance_BriefName { get; private set; } = new FilterItem<string>();

        // Devices.Extensions
        public FilterItem<bool?> Machine_Device_Extensions { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> Machine_Device_Extension_Name { get; private set; } = new FilterItem<string>();

        // Slots
        public FilterItem<bool?> Machine_Slots { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> Machine_Slot_Name { get; private set; } = new FilterItem<string>();

        // Slots.SlotOptions
        public FilterItem<bool?> Machine_Slot_SlotOptions { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> Machine_Slot_SlotOption_Name { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Slot_SlotOption_DeviceName { get; private set; } = new FilterItem<string>();
        public FilterItem<bool?> Machine_Slot_SlotOption_Default { get; private set; } = new FilterItem<bool?>() { Neutral = null };

        // SoftwareLists
        public FilterItem<bool?> Machine_SoftwareLists { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> Machine_SoftwareList_Name { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_SoftwareList_Status { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_SoftwareList_Filter { get; private set; } = new FilterItem<string>();

        // RamOptions
        public FilterItem<bool?> Machine_RamOptions { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<bool?> Machine_RamOption_Default { get; private set; } = new FilterItem<bool?>() { Neutral = null };

        #endregion

        #region Logiqx

        public FilterItem<string> Machine_Board { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_RebuildTo { get; private set; } = new FilterItem<string>();

        #endregion

        #region Logiqx EmuArc

        public FilterItem<string> Machine_TitleID { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Developer { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Genre { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Subgenre { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Ratings { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Score { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Enabled { get; private set; } = new FilterItem<string>();
        public FilterItem<bool?> Machine_CRC { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> Machine_RelatedTo { get; private set; } = new FilterItem<string>();

        #endregion

        #region OpenMSX

        public FilterItem<string> Machine_GenMSXID { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_System { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Country { get; private set; } = new FilterItem<string>();

        #endregion

        #region SoftwareList

        public FilterItem<Supported> Machine_Supported { get; private set; } = new FilterItem<Supported>() { Positive = Supported.NULL, Negative = Supported.NULL };

        // Infos
        public FilterItem<bool?> Machine_Infos { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> Machine_Info_Name { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_Info_Value { get; private set; } = new FilterItem<string>();

        // SharedFeatures
        public FilterItem<bool?> Machine_SharedFeatures { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> Machine_SharedFeature_Name { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Machine_SharedFeature_Value { get; private set; } = new FilterItem<string>();

        #endregion

        #endregion // Machine Filters

        #region DatItem Filters

        #region Common

        /// <summary>
        /// Include or exclude item names
        /// </summary>
        public FilterItem<string> ItemName { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude item types
        /// </summary>
        public FilterItem<string> ItemTypes { get; private set; } = new FilterItem<string>();

        #endregion

        #region AttractMode

        /// <summary>
        /// Include or exclude alt names
        /// </summary>
        public FilterItem<string> AltName { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude alt titles
        /// </summary>
        public FilterItem<string> AltTitle { get; private set; } = new FilterItem<string>();

        #endregion

        #region OpenMSX

        /// <summary>
        /// Include or exclude original value
        /// </summary>
        public FilterItem<string> Original { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude items of a certain OpenMSX subtype
        /// </summary>
        public FilterItem<OpenMSXSubType> SubType { get; private set; } = new FilterItem<OpenMSXSubType>() { Positive = OpenMSXSubType.NULL, Negative = OpenMSXSubType.NULL };

        /// <summary>
        /// Include or exclude OpenMSX type
        /// </summary>
        public FilterItem<string> OpenMSXType { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude remarks
        /// </summary>
        public FilterItem<string> Remark { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude boots
        /// </summary>
        public FilterItem<string> Boot { get; private set; } = new FilterItem<string>();

        #endregion

        #region SoftwareList

        /// <summary>
        /// Include or exclude part names
        /// </summary>
        public FilterItem<string> PartName { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude part interfaces
        /// </summary>
        public FilterItem<string> PartInterface { get; private set; } = new FilterItem<string>();

        // TODO: DatItem.Features - List<SoftwareListFeature>

        /// <summary>
        /// Include or exclude area names
        /// </summary>
        public FilterItem<string> AreaName { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude area sizes
        /// </summary>
        /// <remarks>Positive means "Greater than or equal", Negative means "Less than or equal", Neutral means "Equal"</remarks>
        public FilterItem<long?> AreaSize { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };

        /// <summary>
        /// Include or exclude area byte widths
        /// </summary>
        public FilterItem<string> AreaWidth { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude area endianness
        /// </summary>
        public FilterItem<string> AreaEndianness { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude softwarelist value
        /// </summary>
        public FilterItem<string> Value { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude load flag
        /// </summary>
        public FilterItem<string> LoadFlag { get; private set; } = new FilterItem<string>();

        #endregion

        /// <summary>
        /// Include or exclude items with the "Default" tag
        /// </summary>
        public FilterItem<bool?> Default { get; private set; } = new FilterItem<bool?>() { Neutral = null };

        /// <summary>
        /// Include or exclude descriptions
        /// </summary>
        public FilterItem<string> Description { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude item sizes
        /// </summary>
        /// <remarks>Positive means "Greater than or equal", Negative means "Less than or equal", Neutral means "Equal"</remarks>
        public FilterItem<long> Size { get; private set; } = new FilterItem<long>() { Positive = -1, Negative = -1, Neutral = -1 };

        /// <summary>
        /// Include or exclude CRC32 hashes
        /// </summary>
        public FilterItem<string> CRC { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude MD5 hashes
        /// </summary>
        public FilterItem<string> MD5 { get; private set; } = new FilterItem<string>();

#if NET_FRAMEWORK
        /// <summary>
        /// Include or exclude RIPEMD160 hashes
        /// </summary>
        public FilterItem<string> RIPEMD160 { get; private set; } = new FilterItem<string>();
#endif

        /// <summary>
        /// Include or exclude SHA-1 hashes
        /// </summary>
        public FilterItem<string> SHA1 { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude SHA-256 hashes
        /// </summary>
        public FilterItem<string> SHA256 { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude SHA-384 hashes
        /// </summary>
        public FilterItem<string> SHA384 { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude SHA-512 hashes
        /// </summary>
        public FilterItem<string> SHA512 { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude merge tags
        /// </summary>
        public FilterItem<string> MergeTag { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude regions
        /// </summary>
        public FilterItem<string> Region { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude indexes
        /// </summary>
        public FilterItem<string> Index { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude items with the "Writable" tag
        /// </summary>
        public FilterItem<bool?> Writable { get; private set; } = new FilterItem<bool?>() { Neutral = null };

        /// <summary>
        /// Include or exclude items with the "Writable" tag
        /// </summary>
        public FilterItem<bool?> Optional { get; private set; } = new FilterItem<bool?>() { Neutral = null };

        /// <summary>
        /// Include or exclude item statuses
        /// </summary>
        public FilterItem<ItemStatus> Status { get; private set; } = new FilterItem<ItemStatus>() { Positive = ItemStatus.NULL, Negative = ItemStatus.NULL };

        /// <summary>
        /// Include or exclude languages
        /// </summary>
        public FilterItem<string> Language { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude dates
        /// </summary>
        public FilterItem<string> Date { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude bioses
        /// </summary>
        public FilterItem<string> Bios { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude offsets
        /// </summary>
        public FilterItem<string> Offset { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude offsets
        /// </summary>
        public FilterItem<bool?> Inverted { get; private set; } = new FilterItem<bool?>();

        #endregion // DatItem Filters

        #region Manipulation Flags

        /// <summary>
        /// Clean all names to WoD standards
        /// </summary>
        public bool Clean { get; set; }

        /// <summary>
        /// Set Machine Description from Machine Name
        /// </summary>
        public bool DescriptionAsName { get; set; }

        /// <summary>
        /// Include romof and cloneof when filtering machine names
        /// </summary>
        public bool IncludeOfInGame { get; set; }

        /// <summary>
        /// Internally split a DatFile
        /// </summary>
        public MergingFlag InternalSplit { get; set; }

        /// <summary>
        /// Remove all unicode characters
        /// </summary>
        public bool RemoveUnicode { get; set; }

        /// <summary>
        /// Include root directory when determing trim sizes
        /// </summary>
        public string Root { get; set; }

        /// <summary>
        /// Change all machine names to "!"
        /// </summary>
        public bool Single { get; set; }

        /// <summary>
        /// Trim total machine and item name to not exceed NTFS limits
        /// </summary>
        public bool Trim { get; set; }

        #endregion

        #endregion // Fields

        #region Instance methods

        #region Filter Population

        /// <summary>
        /// Populate the filters object using a set of key:value filters
        /// </summary>
        /// <param name="filters">List of key:value where ~key/!key is negated</param>
        public void PopulateFromList(List<string> filters)
        {
            foreach (string filterPair in filters)
            {
                // If we don't even have a possible filter pair
                if (!filterPair.Contains(":"))
                {
                    Globals.Logger.Warning($"'{filterPair}` is not a valid filter string. Valid filter strings are of the form 'key:value'. Please refer to README.1ST or the help feature for more details.");
                    continue;
                }

                string filterPairTrimmed = filterPair.Trim('"', ' ', '\t');
                bool negate = filterPairTrimmed.StartsWith("!")
                    || filterPairTrimmed.StartsWith("~")
                    || filterPairTrimmed.StartsWith("not-");
                filterPairTrimmed = filterPairTrimmed.TrimStart('!', '~');
                filterPairTrimmed = filterPairTrimmed.StartsWith("not-") ? filterPairTrimmed.Substring(4) : filterPairTrimmed;

                string filterFieldString = filterPairTrimmed.Split(':')[0].ToLowerInvariant().Trim('"', ' ', '\t');
                string filterValue = filterPairTrimmed.Substring(filterFieldString.Length + 1).Trim('"', ' ', '\t');

                Field filterField = filterFieldString.AsField();
                SetFilter(filterField, filterValue, negate);
            }
        }

        /// <summary>
        /// Set multiple filters from key
        /// </summary>
        /// <param name="key">Key for the filter to be set</param>
        /// <param name="values">List of values for the filter</param>
        /// <param name="negate">True if negative filter, false otherwise</param>
        public void SetFilter(Field key, List<string> values, bool negate)
        {
            foreach (string value in values)
            {
                SetFilter(key, value, negate);
            }
        }

        /// <summary>
        /// Set a single filter from key
        /// </summary>
        /// <param name="key">Key for the filter to be set</param>
        /// <param name="value">Value of the filter</param>
        /// <param name="negate">True if negative filter, false otherwise</param>
        public void SetFilter(Field key, string value, bool negate)
        {
            switch (key)
            {
                #region Machine Filters

                #region Common

                case Field.Machine_Name:
                    if (negate)
                        Machine_Name.NegativeSet.Add(value);
                    else
                        Machine_Name.PositiveSet.Add(value);
                    break;

                case Field.Machine_Comment:
                    if (negate)
                        Machine_Comment.NegativeSet.Add(value);
                    else
                        Machine_Comment.PositiveSet.Add(value);
                    break;

                case Field.Machine_Description:
                    if (negate)
                        Machine_Description.NegativeSet.Add(value);
                    else
                        Machine_Description.PositiveSet.Add(value);
                    break;

                case Field.Machine_Year:
                    if (negate)
                        Machine_Year.NegativeSet.Add(value);
                    else
                        Machine_Year.PositiveSet.Add(value);
                    break;

                case Field.Machine_Manufacturer:
                    if (negate)
                        Machine_Manufacturer.NegativeSet.Add(value);
                    else
                        Machine_Manufacturer.PositiveSet.Add(value);
                    break;

                case Field.Machine_Publisher:
                    if (negate)
                        Machine_Publisher.NegativeSet.Add(value);
                    else
                        Machine_Publisher.PositiveSet.Add(value);
                    break;

                case Field.Machine_Category:
                    if (negate)
                        Machine_Category.NegativeSet.Add(value);
                    else
                        Machine_Category.PositiveSet.Add(value);
                    break;

                case Field.Machine_RomOf:
                    if (negate)
                        Machine_RomOf.NegativeSet.Add(value);
                    else
                        Machine_RomOf.PositiveSet.Add(value);
                    break;

                case Field.Machine_CloneOf:
                    if (negate)
                        Machine_CloneOf.NegativeSet.Add(value);
                    else
                        Machine_CloneOf.PositiveSet.Add(value);
                    break;

                case Field.Machine_SampleOf:
                    if (negate)
                        Machine_SampleOf.NegativeSet.Add(value);
                    else
                        Machine_SampleOf.PositiveSet.Add(value);
                    break;

                case Field.Machine_Type:
                    if (negate)
                        Machine_Type.Negative |= value.AsMachineType();
                    else
                        Machine_Type.Positive |= value.AsMachineType();
                    break;

                #endregion

                #region AttractMode

                case Field.Machine_Players:
                    if (negate)
                        Machine_Players.NegativeSet.Add(value);
                    else
                        Machine_Players.PositiveSet.Add(value);
                    break;

                case Field.Machine_Rotation:
                    if (negate)
                        Machine_Rotation.NegativeSet.Add(value);
                    else
                        Machine_Rotation.PositiveSet.Add(value);
                    break;

                case Field.Machine_Control:
                    if (negate)
                        Machine_Control.NegativeSet.Add(value);
                    else
                        Machine_Control.PositiveSet.Add(value);
                    break;

                case Field.Machine_Status:
                    if (negate)
                        Machine_Status.NegativeSet.Add(value);
                    else
                        Machine_Status.PositiveSet.Add(value);
                    break;

                case Field.Machine_DisplayCount:
                    if (negate)
                        Machine_DisplayCount.NegativeSet.Add(value);
                    else
                        Machine_DisplayCount.PositiveSet.Add(value);
                    break;

                case Field.Machine_DisplayType:
                    if (negate)
                        Machine_DisplayType.NegativeSet.Add(value);
                    else
                        Machine_DisplayType.PositiveSet.Add(value);
                    break;

                case Field.Machine_Buttons:
                    if (negate)
                        Machine_Buttons.NegativeSet.Add(value);
                    else
                        Machine_Buttons.PositiveSet.Add(value);
                    break;

                #endregion

                #region ListXML

                case Field.Machine_SourceFile:
                    if (negate)
                        Machine_SourceFile.NegativeSet.Add(value);
                    else
                        Machine_SourceFile.PositiveSet.Add(value);
                    break;

                case Field.Machine_Runnable:
                    if (negate)
                        Machine_Runnable.Negative |= value.AsRunnable();
                    else
                        Machine_Runnable.Positive |= value.AsRunnable();
                    break;

                #endregion

                #region Logiqx

                case Field.Machine_Board:
                    if (negate)
                        Machine_Board.NegativeSet.Add(value);
                    else
                        Machine_Board.PositiveSet.Add(value);
                    break;

                case Field.Machine_RebuildTo:
                    if (negate)
                        Machine_RebuildTo.NegativeSet.Add(value);
                    else
                        Machine_RebuildTo.PositiveSet.Add(value);
                    break;

                #endregion

                #region Logiqx EmuArc

                case Field.Machine_TitleID:
                    if (negate)
                        Machine_TitleID.NegativeSet.Add(value);
                    else
                        Machine_TitleID.PositiveSet.Add(value);
                    break;

                case Field.Machine_Developer:
                    if (negate)
                        Machine_Developer.NegativeSet.Add(value);
                    else
                        Machine_Developer.PositiveSet.Add(value);
                    break;

                case Field.Machine_Genre:
                    if (negate)
                        Machine_Genre.NegativeSet.Add(value);
                    else
                        Machine_Genre.PositiveSet.Add(value);
                    break;

                case Field.Machine_Subgenre:
                    if (negate)
                        Machine_Subgenre.NegativeSet.Add(value);
                    else
                        Machine_Subgenre.PositiveSet.Add(value);
                    break;

                case Field.Machine_Ratings:
                    if (negate)
                        Machine_Ratings.NegativeSet.Add(value);
                    else
                        Machine_Ratings.PositiveSet.Add(value);
                    break;

                case Field.Machine_Score:
                    if (negate)
                        Machine_Score.NegativeSet.Add(value);
                    else
                        Machine_Score.PositiveSet.Add(value);
                    break;

                case Field.Machine_Enabled:
                    if (negate)
                        Machine_Enabled.NegativeSet.Add(value);
                    else
                        Machine_Enabled.PositiveSet.Add(value);
                    break;

                case Field.Machine_CRC:
                    if (negate || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                        Machine_CRC.Neutral = false;
                    else
                        Machine_CRC.Neutral = true;
                    break;

                case Field.Machine_RelatedTo:
                    if (negate)
                        Machine_RelatedTo.NegativeSet.Add(value);
                    else
                        Machine_RelatedTo.PositiveSet.Add(value);
                    break;

                #endregion

                #region OpenMSX

                case Field.Machine_GenMSXID:
                    if (negate)
                        Machine_GenMSXID.NegativeSet.Add(value);
                    else
                        Machine_GenMSXID.PositiveSet.Add(value);
                    break;

                case Field.Machine_System:
                    if (negate)
                        Machine_System.NegativeSet.Add(value);
                    else
                        Machine_System.PositiveSet.Add(value);
                    break;

                case Field.Machine_Country:
                    if (negate)
                        Machine_Country.NegativeSet.Add(value);
                    else
                        Machine_Country.PositiveSet.Add(value);
                    break;

                #endregion

                #region SoftwareList

                case Field.Machine_Supported:
                    if (negate)
                        Machine_Supported.Negative |= value.AsSupported();
                    else
                        Machine_Supported.Positive |= value.AsSupported();
                    break;

                #endregion

                #endregion // Machine Filters

                #region DatItem Filters

                #region Common

                case Field.DatItem_Name:
                    if (negate)
                        ItemName.NegativeSet.Add(value);
                    else
                        ItemName.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Type:
                    if (value.AsItemType() == null)
                        return;

                    if (negate)
                        ItemTypes.NegativeSet.Add(value);
                    else
                        ItemTypes.PositiveSet.Add(value);
                    break;

                #endregion

                #region AttractMode

                case Field.DatItem_AltName:
                    if (negate)
                        AltName.NegativeSet.Add(value);
                    else
                        AltName.PositiveSet.Add(value);
                    break;

                case Field.DatItem_AltTitle:
                    if (negate)
                        AltTitle.NegativeSet.Add(value);
                    else
                        AltTitle.PositiveSet.Add(value);
                    break;

                #endregion

                #region OpenMSX

                case Field.DatItem_Original:
                    if (negate)
                        Original.NegativeSet.Add(value);
                    else
                        Original.PositiveSet.Add(value);
                    break;

                case Field.DatItem_OpenMSXSubType:
                    if (negate)
                        SubType.Negative |= value.AsOpenMSXSubType();
                    else
                        SubType.Positive |= value.AsOpenMSXSubType();
                    break;

                case Field.DatItem_OpenMSXType:
                    if (negate)
                        OpenMSXType.NegativeSet.Add(value);
                    else
                        OpenMSXType.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Remark:
                    if (negate)
                        Remark.NegativeSet.Add(value);
                    else
                        Remark.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Boot:
                    if (negate)
                        Boot.NegativeSet.Add(value);
                    else
                        Boot.PositiveSet.Add(value);
                    break;

                #endregion

                #region SoftwareList

                case Field.DatItem_Part_Name:
                    if (negate)
                        PartName.NegativeSet.Add(value);
                    else
                        PartName.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Part_Interface:
                    if (negate)
                        PartInterface.NegativeSet.Add(value);
                    else
                        PartInterface.PositiveSet.Add(value);
                    break;

                case Field.DatItem_AreaName:
                    if (negate)
                        AreaName.NegativeSet.Add(value);
                    else
                        AreaName.PositiveSet.Add(value);
                    break;

                case Field.DatItem_AreaSize:
                    bool? asOperation = null;
                    if (value.StartsWith(">"))
                        asOperation = true;
                    else if (value.StartsWith("<"))
                        asOperation = false;
                    else if (value.StartsWith("="))
                        asOperation = null;

                    string areasizeString = value.TrimStart('>', '<', '=');
                    if (!Int64.TryParse(areasizeString, out long areasize))
                        return;

                    // Equal
                    if (asOperation == null && !negate)
                    {
                        AreaSize.Neutral = areasize;
                    }

                    // Not Equal
                    else if (asOperation == null && negate)
                    {
                        AreaSize.Negative = areasize - 1;
                        AreaSize.Positive = areasize + 1;
                    }

                    // Greater Than or Equal
                    else if (asOperation == true && !negate)
                    {
                        AreaSize.Positive = areasize;
                    }

                    // Strictly Less Than
                    else if (asOperation == true && negate)
                    {
                        AreaSize.Negative = areasize - 1;
                    }

                    // Less Than or Equal
                    else if (asOperation == false && !negate)
                    {
                        AreaSize.Negative = areasize;
                    }

                    // Strictly Greater Than
                    else if (asOperation == false && negate)
                    {
                        AreaSize.Positive = areasize + 1;
                    }

                    break;

                case Field.DatItem_AreaWidth:
                    if (negate)
                        AreaWidth.NegativeSet.Add(value);
                    else
                        AreaWidth.PositiveSet.Add(value);
                    break;

                case Field.DatItem_AreaEndianness:
                    if (negate)
                        AreaEndianness.NegativeSet.Add(value);
                    else
                        AreaEndianness.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Value:
                    if (negate)
                        Value.NegativeSet.Add(value);
                    else
                        Value.PositiveSet.Add(value);
                    break;

                case Field.DatItem_LoadFlag:
                    if (negate)
                        LoadFlag.NegativeSet.Add(value);
                    else
                        LoadFlag.PositiveSet.Add(value);
                    break;

                #endregion

                case Field.DatItem_Default:
                    if (negate || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                        Default.Neutral = false;
                    else
                        Default.Neutral = true;
                    break;

                case Field.DatItem_Description:
                    if (negate)
                        Description.NegativeSet.Add(value);
                    else
                        Description.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Size:
                    bool? sOperation = null;
                    if (value.StartsWith(">"))
                        sOperation = true;
                    else if (value.StartsWith("<"))
                        sOperation = false;
                    else if (value.StartsWith("="))
                        sOperation = null;

                    string sizeString = value.TrimStart('>', '<', '=');
                    if (!Int64.TryParse(sizeString, out long size))
                        return;

                    // Equal
                    if (sOperation == null && !negate)
                    {
                        Size.Neutral = size;
                    }

                    // Not Equal
                    else if (sOperation == null && negate)
                    {
                        Size.Negative = size - 1;
                        Size.Positive = size + 1;
                    }

                    // Greater Than or Equal
                    else if (sOperation == true && !negate)
                    {
                        Size.Positive = size;
                    }

                    // Strictly Less Than
                    else if (sOperation == true && negate)
                    {
                        Size.Negative = size - 1;
                    }

                    // Less Than or Equal
                    else if (sOperation == false && !negate)
                    {
                        Size.Negative = size;
                    }

                    // Strictly Greater Than
                    else if (sOperation == false && negate)
                    {
                        Size.Positive = size + 1;
                    }

                    break;

                case Field.DatItem_CRC:
                    if (negate)
                        CRC.NegativeSet.Add(value);
                    else
                        CRC.PositiveSet.Add(value);
                    break;

                case Field.DatItem_MD5:
                    if (negate)
                        MD5.NegativeSet.Add(value);
                    else
                        MD5.PositiveSet.Add(value);
                    break;

#if NET_FRAMEWORK
                case Field.DatItem_RIPEMD160:
                    if (negate)
                        RIPEMD160.NegativeSet.Add(value);
                    else
                        RIPEMD160.PositiveSet.Add(value);
                    break;
#endif

                case Field.DatItem_SHA1:
                    if (negate)
                        SHA1.NegativeSet.Add(value);
                    else
                        SHA1.PositiveSet.Add(value);
                    break;

                case Field.DatItem_SHA256:
                    if (negate)
                        SHA256.NegativeSet.Add(value);
                    else
                        SHA256.PositiveSet.Add(value);
                    break;

                case Field.DatItem_SHA384:
                    if (negate)
                        SHA384.NegativeSet.Add(value);
                    else
                        SHA384.PositiveSet.Add(value);
                    break;

                case Field.DatItem_SHA512:
                    if (negate)
                        SHA512.NegativeSet.Add(value);
                    else
                        SHA512.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Merge:
                    if (negate)
                        MergeTag.NegativeSet.Add(value);
                    else
                        MergeTag.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Region:
                    if (negate)
                        Region.NegativeSet.Add(value);
                    else
                        Region.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Index:
                    if (negate)
                        Index.NegativeSet.Add(value);
                    else
                        Index.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Writable:
                    if (negate || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                        Writable.Neutral = false;
                    else
                        Writable.Neutral = true;
                    break;

                case Field.DatItem_Optional:
                    if (negate || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                        Optional.Neutral = false;
                    else
                        Optional.Neutral = true;
                    break;

                case Field.DatItem_Status:
                    if (negate)
                        Status.Negative |= value.AsItemStatus();
                    else
                        Status.Positive |= value.AsItemStatus();
                    break;

                case Field.DatItem_Language:
                    if (negate)
                        Language.NegativeSet.Add(value);
                    else
                        Language.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Date:
                    if (negate)
                        Date.NegativeSet.Add(value);
                    else
                        Date.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Bios:
                    if (negate)
                        Bios.NegativeSet.Add(value);
                    else
                        Bios.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Offset:
                    if (negate)
                        Offset.NegativeSet.Add(value);
                    else
                        Offset.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Inverted:
                    if (negate || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                        Inverted.Neutral = false;
                    else
                        Inverted.Neutral = true;
                    break;

                    #endregion // DatItem Filters
            }
        }

        #endregion

        #endregion // Instance Methods
    }
}
