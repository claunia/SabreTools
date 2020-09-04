using System;
using System.Collections.Generic;

using SabreTools.Library.Data;
using SabreTools.Library.DatItems;
using SabreTools.Library.Tools;

namespace SabreTools.Library.Filtering
{
    /// <summary>
    /// Represents the filtering operations that need to be performed on a set of items, usually a DAT
    /// </summary>
    /// TODO: Can clever use of Filtering allow for easier external splitting methods?
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
        public FilterItem<MachineType> Machine_Type { get; private set; } = new FilterItem<MachineType>() { Positive = 0x0, Negative = 0x0 };

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

        #endregion

        #endregion // Machine Filters

        #region DatItem Filters

        #region Common

        public FilterItem<string> DatItem_Type { get; private set; } = new FilterItem<string>();

        #endregion

        #region Item-Specific

        #region Actionable

        // Rom
        public FilterItem<string> DatItem_Name { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Bios { get; private set; } = new FilterItem<string>();
        public FilterItem<long> DatItem_Size { get; private set; } = new FilterItem<long>() { Positive = -1, Negative = -1, Neutral = -1 };
        public FilterItem<string> DatItem_CRC { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_MD5 { get; private set; } = new FilterItem<string>();
#if NET_FRAMEWORK
        public FilterItem<string> DatItem_RIPEMD160 { get; private set; } = new FilterItem<string>();
#endif
        public FilterItem<string> DatItem_SHA1 { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_SHA256 { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_SHA384 { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_SHA512 { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Merge { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Region { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Offset { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Date { get; private set; } = new FilterItem<string>();
        public FilterItem<ItemStatus> DatItem_Status { get; private set; } = new FilterItem<ItemStatus>() { Positive = ItemStatus.NULL, Negative = ItemStatus.NULL };
        public FilterItem<bool?> DatItem_Optional { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<bool?> DatItem_Inverted { get; private set; } = new FilterItem<bool?>();

        // Rom (AttractMode)
        public FilterItem<string> DatItem_AltName { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_AltTitle { get; private set; } = new FilterItem<string>();

        // Rom (OpenMSX)
        public FilterItem<string> DatItem_Original { get; private set; } = new FilterItem<string>();
        public FilterItem<OpenMSXSubType> DatItem_OpenMSXSubType { get; private set; } = new FilterItem<OpenMSXSubType>() { Positive = OpenMSXSubType.NULL, Negative = OpenMSXSubType.NULL };
        public FilterItem<string> DatItem_OpenMSXType { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Remark { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Boot { get; private set; } = new FilterItem<string>();

        // Rom (SoftwareList)
        public FilterItem<string> DatItem_AreaName { get; private set; } = new FilterItem<string>();
        public FilterItem<long?> DatItem_AreaSize { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };
        public FilterItem<long?> DatItem_AreaWidth { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };
        public FilterItem<Endianness> DatItem_AreaEndianness { get; private set; } = new FilterItem<Endianness>() { Positive = Endianness.NULL, Negative = Endianness.NULL };
        public FilterItem<LoadFlag> DatItem_LoadFlag { get; private set; } = new FilterItem<LoadFlag>() { Positive = LoadFlag.NULL, Negative = LoadFlag.NULL };
        public FilterItem<string> DatItem_Part_Name { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Part_Interface { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Part_Feature_Name { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Part_Feature_Value { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Value { get; private set; } = new FilterItem<string>();

        // Disk
        public FilterItem<string> DatItem_Index { get; private set; } = new FilterItem<string>();
        public FilterItem<bool?> DatItem_Writable { get; private set; } = new FilterItem<bool?>() { Neutral = null };

        #endregion

        #region Auxiliary

        // Adjuster
        public FilterItem<bool?> DatItem_Default { get; private set; } = new FilterItem<bool?>() { Neutral = null };

        // Analog
        public FilterItem<string> DatItem_Analog_Mask { get; private set; } = new FilterItem<string>();

        // BiosSet
        public FilterItem<string> DatItem_Description { get; private set; } = new FilterItem<string>();

        // Chip
        public FilterItem<string> DatItem_Tag { get; private set; } = new FilterItem<string>();
        public FilterItem<ChipType> DatItem_ChipType { get; private set; } = new FilterItem<ChipType>() { Positive = ChipType.NULL, Negative = ChipType.NULL };
        public FilterItem<string> DatItem_Clock { get; private set; } = new FilterItem<string>();

        // Condition
        public FilterItem<string> DatItem_Mask { get; private set; } = new FilterItem<string>();
        public FilterItem<Relation> DatItem_Relation { get; private set; } = new FilterItem<Relation>() { Positive = Relation.NULL, Negative = Relation.NULL };
        public FilterItem<string> DatItem_Condition_Tag { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Condition_Mask { get; private set; } = new FilterItem<string>();
        public FilterItem<Relation> DatItem_Condition_Relation { get; private set; } = new FilterItem<Relation>() { Positive = Relation.NULL, Negative = Relation.NULL };
        public FilterItem<string> DatItem_Condition_Value { get; private set; } = new FilterItem<string>();

        // Control
        public FilterItem<string> DatItem_Control_Type { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Control_Player { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Control_Buttons { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Control_RegButtons { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Control_Minimum { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Control_Maximum { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Control_Sensitivity { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Control_KeyDelta { get; private set; } = new FilterItem<string>();
        public FilterItem<bool?> DatItem_Control_Reverse { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> DatItem_Control_Ways { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Control_Ways2 { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Control_Ways3 { get; private set; } = new FilterItem<string>();

        // Device
        public FilterItem<string> DatItem_DeviceType { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_FixedImage { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Mandatory { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Interface { get; private set; } = new FilterItem<string>();

        // Display
        public FilterItem<DisplayType> DatItem_DisplayType { get; private set; } = new FilterItem<DisplayType>() { Positive = DisplayType.NULL, Negative = DisplayType.NULL };
        public FilterItem<long?> DatItem_Rotate { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };
        public FilterItem<bool?> DatItem_FlipX { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<long?> DatItem_Width { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };
        public FilterItem<long?> DatItem_Height { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };
        public FilterItem<string> DatItem_Refresh { get; private set; } = new FilterItem<string>();
        public FilterItem<long?> DatItem_PixClock { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };
        public FilterItem<string> DatItem_HTotal { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_HBEnd { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_HBStart { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_VTotal { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_VBEnd { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_VBStart { get; private set; } = new FilterItem<string>();

        // Driver
        public FilterItem<SupportStatus> DatItem_SupportStatus { get; private set; } = new FilterItem<SupportStatus>() { Positive = SupportStatus.NULL, Negative = SupportStatus.NULL };
        public FilterItem<SupportStatus> DatItem_EmulationStatus { get; private set; } = new FilterItem<SupportStatus>() { Positive = SupportStatus.NULL, Negative = SupportStatus.NULL };
        public FilterItem<SupportStatus> DatItem_CocktailStatus { get; private set; } = new FilterItem<SupportStatus>() { Positive = SupportStatus.NULL, Negative = SupportStatus.NULL };
        public FilterItem<Supported> DatItem_SaveStateStatus { get; private set; } = new FilterItem<Supported>() { Positive = Supported.NULL, Negative = Supported.NULL };

        // Extension
        public FilterItem<string> DatItem_Extension_Name { get; private set; } = new FilterItem<string>();

        // Feature
        public FilterItem<FeatureType> DatItem_FeatureType { get; private set; } = new FilterItem<FeatureType>() { Positive = FeatureType.NULL, Negative = FeatureType.NULL };
        public FilterItem<FeatureStatus> DatItem_FeatureStatus { get; private set; } = new FilterItem<FeatureStatus>() { Positive = FeatureStatus.NULL, Negative = FeatureStatus.NULL };
        public FilterItem<FeatureStatus> DatItem_FeatureOverall { get; private set; } = new FilterItem<FeatureStatus>() { Positive = FeatureStatus.NULL, Negative = FeatureStatus.NULL };

        // Input
        public FilterItem<bool?> DatItem_Service { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<bool?> DatItem_Tilt { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<long?> DatItem_Players { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };
        public FilterItem<long?> DatItem_Coins { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };

        // Instance
        public FilterItem<string> DatItem_Instance_Name { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Instance_BriefName { get; private set; } = new FilterItem<string>();

        // Location
        public FilterItem<string> DatItem_Location_Name { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Location_Number { get; private set; } = new FilterItem<string>();
        public FilterItem<bool?> DatItem_Location_Inverted { get; private set; } = new FilterItem<bool?>() { Neutral = null };

        // RamOption
        public FilterItem<string> DatItem_Content { get; private set; } = new FilterItem<string>();

        // Release
        public FilterItem<string> DatItem_Language { get; private set; } = new FilterItem<string>();

        // Setting
        public FilterItem<string> DatItem_Setting_Name { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Setting_Value { get; private set; } = new FilterItem<string>();
        public FilterItem<bool?> DatItem_Setting_Default { get; private set; } = new FilterItem<bool?>() { Neutral = null };

        // SlotOption
        public FilterItem<string> DatItem_SlotOption_Name { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_SlotOption_DeviceName { get; private set; } = new FilterItem<string>();
        public FilterItem<bool?> DatItem_SlotOption_Default { get; private set; } = new FilterItem<bool?>() { Neutral = null };

        // SoftwareList
        public FilterItem<SoftwareListStatus> DatItem_SoftwareListStatus { get; private set; } = new FilterItem<SoftwareListStatus>() { Positive = SoftwareListStatus.NULL, Negative = SoftwareListStatus.NULL };
        public FilterItem<string> DatItem_Filter { get; private set; } = new FilterItem<string>();

        // Sound
        public FilterItem<long?> DatItem_Channels { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };

        #endregion

        #endregion // Item-Specific

        #endregion // DatItem Filters

        #region Additional Flags

        /// <summary>
        /// Include romof and cloneof when filtering machine names
        /// </summary>
        public bool IncludeOfInGame { get; set; }

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
                    SetStringFilter(Machine_Name, value, negate);
                    break;

                case Field.Machine_Comment:
                    SetStringFilter(Machine_Comment, value, negate);
                    break;

                case Field.Machine_Description:
                    SetStringFilter(Machine_Description, value, negate);
                    break;

                case Field.Machine_Year:
                    SetStringFilter(Machine_Year, value, negate);
                    break;

                case Field.Machine_Manufacturer:
                    SetStringFilter(Machine_Manufacturer, value, negate);
                    break;

                case Field.Machine_Publisher:
                    SetStringFilter(Machine_Publisher, value, negate);
                    break;

                case Field.Machine_Category:
                    SetStringFilter(Machine_Category, value, negate);
                    break;

                case Field.Machine_RomOf:
                    SetStringFilter(Machine_RomOf, value, negate);
                    break;

                case Field.Machine_CloneOf:
                    SetStringFilter(Machine_CloneOf, value, negate);
                    break;

                case Field.Machine_SampleOf:
                    SetStringFilter(Machine_SampleOf, value, negate);
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
                    SetStringFilter(Machine_Players, value, negate);
                    break;

                case Field.Machine_Rotation:
                    SetStringFilter(Machine_Rotation, value, negate);
                    break;

                case Field.Machine_Control:
                    SetStringFilter(Machine_Control, value, negate);
                    break;

                case Field.Machine_Status:
                    SetStringFilter(Machine_Status, value, negate);
                    break;

                case Field.Machine_DisplayCount:
                    SetStringFilter(Machine_DisplayCount, value, negate);
                    break;

                case Field.Machine_DisplayType:
                    SetStringFilter(Machine_DisplayType, value, negate);
                    break;

                case Field.Machine_Buttons:
                    SetStringFilter(Machine_Buttons, value, negate);
                    break;

                #endregion

                #region ListXML

                case Field.Machine_SourceFile:
                    SetStringFilter(Machine_SourceFile, value, negate);
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
                    SetStringFilter(Machine_Board, value, negate);
                    break;

                case Field.Machine_RebuildTo:
                    SetStringFilter(Machine_RebuildTo, value, negate);
                    break;

                #endregion

                #region Logiqx EmuArc

                case Field.Machine_TitleID:
                    SetStringFilter(Machine_TitleID, value, negate);
                    break;

                case Field.Machine_Developer:
                    SetStringFilter(Machine_Developer, value, negate);
                    break;

                case Field.Machine_Genre:
                    SetStringFilter(Machine_Genre, value, negate);
                    break;

                case Field.Machine_Subgenre:
                    SetStringFilter(Machine_Subgenre, value, negate);
                    break;

                case Field.Machine_Ratings:
                    SetStringFilter(Machine_Ratings, value, negate);
                    break;

                case Field.Machine_Score:
                    SetStringFilter(Machine_Score, value, negate);
                    break;

                case Field.Machine_Enabled:
                    SetStringFilter(Machine_Enabled, value, negate);
                    break;

                case Field.Machine_CRC:
                    SetBooleanFilter(Machine_CRC, value, negate);
                    break;

                case Field.Machine_RelatedTo:
                    SetStringFilter(Machine_RelatedTo, value, negate);
                    break;

                #endregion

                #region OpenMSX

                case Field.Machine_GenMSXID:
                    SetStringFilter(Machine_GenMSXID, value, negate);
                    break;

                case Field.Machine_System:
                    SetStringFilter(Machine_System, value, negate);
                    break;

                case Field.Machine_Country:
                    SetStringFilter(Machine_Country, value, negate);
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

                case Field.DatItem_Type:
                    if (value.AsItemType() == null)
                        return;

                    SetStringFilter(DatItem_Type, value, negate);
                    break;

                #endregion

                #region Item-Specific

                #region Actionable

                // Rom
                case Field.DatItem_Name:
                    SetStringFilter(DatItem_Name, value, negate);
                    break;

                case Field.DatItem_Bios:
                    SetStringFilter(DatItem_Bios, value, negate);
                    break;

                case Field.DatItem_Size:
                    SetLongFilter(DatItem_Size, value, negate);
                    break;

                case Field.DatItem_CRC:
                    SetStringFilter(DatItem_CRC, value, negate);
                    break;

                case Field.DatItem_MD5:
                    SetStringFilter(DatItem_MD5, value, negate);
                    break;

#if NET_FRAMEWORK
                case Field.DatItem_RIPEMD160:
                    SetStringFilter(DatItem_RIPEMD160, value, negate);
                    break;
#endif

                case Field.DatItem_SHA1:
                    SetStringFilter(DatItem_SHA1, value, negate);
                    break;

                case Field.DatItem_SHA256:
                    SetStringFilter(DatItem_SHA256, value, negate);
                    break;

                case Field.DatItem_SHA384:
                    SetStringFilter(DatItem_SHA384, value, negate);
                    break;

                case Field.DatItem_SHA512:
                    SetStringFilter(DatItem_SHA512, value, negate);
                    break;

                case Field.DatItem_Merge:
                    SetStringFilter(DatItem_Merge, value, negate);
                    break;

                case Field.DatItem_Region:
                    SetStringFilter(DatItem_Region, value, negate);
                    break;

                case Field.DatItem_Offset:
                    SetStringFilter(DatItem_Offset, value, negate);
                    break;

                case Field.DatItem_Date:
                    SetStringFilter(DatItem_Date, value, negate);
                    break;

                case Field.DatItem_Status:
                    if (negate)
                        DatItem_Status.Negative |= value.AsItemStatus();
                    else
                        DatItem_Status.Positive |= value.AsItemStatus();
                    break;

                case Field.DatItem_Optional:
                    SetBooleanFilter(DatItem_Optional, value, negate);
                    break;

                case Field.DatItem_Inverted:
                    SetBooleanFilter(DatItem_Inverted, value, negate);
                    break;

                // Rom (AttractMode)
                case Field.DatItem_AltName:
                    SetStringFilter(DatItem_AltName, value, negate);
                    break;

                case Field.DatItem_AltTitle:
                    SetStringFilter(DatItem_AltTitle, value, negate);
                    break;

                // Rom (OpenMSX)
                case Field.DatItem_Original:
                    SetStringFilter(DatItem_Original, value, negate);
                    break;

                case Field.DatItem_OpenMSXSubType:
                    if (negate)
                        DatItem_OpenMSXSubType.Negative |= value.AsOpenMSXSubType();
                    else
                        DatItem_OpenMSXSubType.Positive |= value.AsOpenMSXSubType();
                    break;

                case Field.DatItem_OpenMSXType:
                    SetStringFilter(DatItem_OpenMSXType, value, negate);
                    break;

                case Field.DatItem_Remark:
                    SetStringFilter(DatItem_Remark, value, negate);
                    break;

                case Field.DatItem_Boot:
                    SetStringFilter(DatItem_Boot, value, negate);
                    break;

                // Rom (SoftwareList)
                case Field.DatItem_AreaName:
                    SetStringFilter(DatItem_AreaName, value, negate);
                    break;

                case Field.DatItem_AreaSize:
                    SetOptionalLongFilter(DatItem_AreaSize, value, negate);
                    break;

                case Field.DatItem_AreaWidth:
                    SetOptionalLongFilter(DatItem_AreaWidth, value, negate);
                    break;

                case Field.DatItem_AreaEndianness:
                    if (negate)
                        DatItem_AreaEndianness.Negative |= value.AsEndianness();
                    else
                        DatItem_AreaEndianness.Positive |= value.AsEndianness();
                    break;

                case Field.DatItem_LoadFlag:
                    if (negate)
                        DatItem_LoadFlag.Negative |= value.AsLoadFlag();
                    else
                        DatItem_LoadFlag.Positive |= value.AsLoadFlag();
                    break;

                case Field.DatItem_Part_Name:
                    SetStringFilter(DatItem_Part_Name, value, negate);
                    break;

                case Field.DatItem_Part_Interface:
                    SetStringFilter(DatItem_Part_Interface, value, negate);
                    break;

                case Field.DatItem_Part_Feature_Name:
                    SetStringFilter(DatItem_Part_Feature_Name, value, negate);
                    break;

                case Field.DatItem_Part_Feature_Value:
                    SetStringFilter(DatItem_Part_Feature_Value, value, negate);
                    break;

                case Field.DatItem_Value:
                    SetStringFilter(DatItem_Value, value, negate);
                    break;

                // Disk
                case Field.DatItem_Index:
                    SetStringFilter(DatItem_Index, value, negate);
                    break;

                case Field.DatItem_Writable:
                    SetBooleanFilter(DatItem_Writable, value, negate);
                    break;

                #endregion

                #region Auxiliary

                // Adjuster
                case Field.DatItem_Default:
                    SetBooleanFilter(DatItem_Default, value, negate);
                    break;

                // Analog
                case Field.DatItem_Analog_Mask:
                    SetStringFilter(DatItem_Analog_Mask, value, negate);
                    break;

                // BiosSet
                case Field.DatItem_Description:
                    SetStringFilter(DatItem_Description, value, negate);
                    break;

                // Chip
                case Field.DatItem_Tag:
                    SetStringFilter(DatItem_Tag, value, negate);
                    break;

                case Field.DatItem_ChipType:
                    if (negate)
                        DatItem_ChipType.Negative |= value.AsChipType();
                    else
                        DatItem_ChipType.Positive |= value.AsChipType();
                    break;

                case Field.DatItem_Clock:
                    SetStringFilter(DatItem_Clock, value, negate);
                    break;

                // Condition
                case Field.DatItem_Mask:
                    SetStringFilter(DatItem_Mask, value, negate);
                    break;

                case Field.DatItem_Relation:
                    if (negate)
                        DatItem_Relation.Negative |= value.AsRelation();
                    else
                        DatItem_Relation.Positive |= value.AsRelation();
                    break;

                case Field.DatItem_Condition_Tag:
                    SetStringFilter(DatItem_Condition_Tag, value, negate);
                    break;

                case Field.DatItem_Condition_Mask:
                    SetStringFilter(DatItem_Condition_Mask, value, negate);
                    break;

                case Field.DatItem_Condition_Relation:
                    if (negate)
                        DatItem_Condition_Relation.Negative |= value.AsRelation();
                    else
                        DatItem_Condition_Relation.Positive |= value.AsRelation();
                    break;

                case Field.DatItem_Condition_Value:
                    SetStringFilter(DatItem_Condition_Value, value, negate);
                    break;

                // Control
                case Field.DatItem_Control_Type:
                    SetStringFilter(DatItem_Control_Type, value, negate);
                    break;

                case Field.DatItem_Control_Player:
                    SetStringFilter(DatItem_Control_Player, value, negate);
                    break;

                case Field.DatItem_Control_Buttons:
                    SetStringFilter(DatItem_Control_Buttons, value, negate);
                    break;

                case Field.DatItem_Control_RegButtons:
                    SetStringFilter(DatItem_Control_RegButtons, value, negate);
                    break;

                case Field.DatItem_Control_Minimum:
                    SetStringFilter(DatItem_Control_Minimum, value, negate);
                    break;

                case Field.DatItem_Control_Maximum:
                    SetStringFilter(DatItem_Control_Maximum, value, negate);
                    break;

                case Field.DatItem_Control_Sensitivity:
                    SetStringFilter(DatItem_Control_Sensitivity, value, negate);
                    break;

                case Field.DatItem_Control_KeyDelta:
                    SetStringFilter(DatItem_Control_KeyDelta, value, negate);
                    break;

                case Field.DatItem_Control_Reverse:
                    SetBooleanFilter(DatItem_Control_Reverse, value, negate);
                    break;

                case Field.DatItem_Control_Ways:
                    SetStringFilter(DatItem_Control_Ways, value, negate);
                    break;

                case Field.DatItem_Control_Ways2:
                    SetStringFilter(DatItem_Control_Ways2, value, negate);
                    break;

                case Field.DatItem_Control_Ways3:
                    SetStringFilter(DatItem_Control_Ways3, value, negate);
                    break;

                // Device
                case Field.DatItem_DeviceType:
                    SetStringFilter(DatItem_DeviceType, value, negate);
                    break;

                case Field.DatItem_FixedImage:
                    SetStringFilter(DatItem_FixedImage, value, negate);
                    break;

                case Field.DatItem_Mandatory:
                    SetStringFilter(DatItem_Mandatory, value, negate);
                    break;

                case Field.DatItem_Interface:
                    SetStringFilter(DatItem_Interface, value, negate);
                    break;

                // Display
                case Field.DatItem_DisplayType:
                    if (negate)
                        DatItem_DisplayType.Negative |= value.AsDisplayType();
                    else
                        DatItem_DisplayType.Positive |= value.AsDisplayType();
                    break;

                case Field.DatItem_Rotate:
                    SetOptionalLongFilter(DatItem_Rotate, value, negate);
                    break;

                case Field.DatItem_FlipX:
                    SetBooleanFilter(DatItem_FlipX, value, negate);
                    break;

                case Field.DatItem_Width:
                    SetOptionalLongFilter(DatItem_Width, value, negate);
                    break;

                case Field.DatItem_Height:
                    SetOptionalLongFilter(DatItem_Height, value, negate);
                    break;

                case Field.DatItem_Refresh:
                    SetStringFilter(DatItem_Refresh, value, negate);
                    break;

                case Field.DatItem_PixClock:
                    SetOptionalLongFilter(DatItem_PixClock, value, negate);
                    break;

                case Field.DatItem_HTotal:
                    SetStringFilter(DatItem_HTotal, value, negate);
                    break;

                case Field.DatItem_HBEnd:
                    SetStringFilter(DatItem_HBEnd, value, negate);
                    break;

                case Field.DatItem_HBStart:
                    SetStringFilter(DatItem_HBStart, value, negate);
                    break;

                case Field.DatItem_VTotal:
                    SetStringFilter(DatItem_VTotal, value, negate);
                    break;

                case Field.DatItem_VBEnd:
                    SetStringFilter(DatItem_VBEnd, value, negate);
                    break;

                case Field.DatItem_VBStart:
                    SetStringFilter(DatItem_VBStart, value, negate);
                    break;

                // Driver
                case Field.DatItem_SupportStatus:
                    if (negate)
                        DatItem_SupportStatus.Negative |= value.AsSupportStatus();
                    else
                        DatItem_SupportStatus.Positive |= value.AsSupportStatus();
                    break;

                case Field.DatItem_EmulationStatus:
                    if (negate)
                        DatItem_EmulationStatus.Negative |= value.AsSupportStatus();
                    else
                        DatItem_EmulationStatus.Positive |= value.AsSupportStatus();
                    break;

                case Field.DatItem_CocktailStatus:
                    if (negate)
                        DatItem_CocktailStatus.Negative |= value.AsSupportStatus();
                    else
                        DatItem_CocktailStatus.Positive |= value.AsSupportStatus();
                    break;

                case Field.DatItem_SaveStateStatus:
                    if (negate)
                        DatItem_SaveStateStatus.Negative |= value.AsSupported();
                    else
                        DatItem_SaveStateStatus.Positive |= value.AsSupported();
                    break;

                // Extension
                case Field.DatItem_Extension_Name:
                    SetStringFilter(DatItem_Extension_Name, value, negate);
                    break;

                // Feature
                case Field.DatItem_FeatureType:
                    if (negate)
                        DatItem_FeatureType.Negative |= value.AsFeatureType();
                    else
                        DatItem_FeatureType.Positive |= value.AsFeatureType();
                    break;

                case Field.DatItem_FeatureStatus:
                    if (negate)
                        DatItem_FeatureStatus.Negative |= value.AsFeatureStatus();
                    else
                        DatItem_FeatureStatus.Positive |= value.AsFeatureStatus();
                    break;

                case Field.DatItem_FeatureOverall:
                    if (negate)
                        DatItem_FeatureOverall.Negative |= value.AsFeatureStatus();
                    else
                        DatItem_FeatureOverall.Positive |= value.AsFeatureStatus();
                    break;

                // Input
                case Field.DatItem_Service:
                    SetBooleanFilter(DatItem_Service, value, negate);
                    break;

                case Field.DatItem_Tilt:
                    SetBooleanFilter(DatItem_Tilt, value, negate);
                    break;

                case Field.DatItem_Players:
                    SetOptionalLongFilter(DatItem_Players, value, negate);
                    break;

                case Field.DatItem_Coins:
                    SetOptionalLongFilter(DatItem_Coins, value, negate);
                    break;

                // Instance
                case Field.DatItem_Instance_Name:
                    SetStringFilter(DatItem_Instance_Name, value, negate);
                    break;

                case Field.DatItem_Instance_BriefName:
                    SetStringFilter(DatItem_Instance_BriefName, value, negate);
                    break;

                // Location
                case Field.DatItem_Location_Name:
                    SetStringFilter(DatItem_Location_Name, value, negate);
                    break;

                case Field.DatItem_Location_Number:
                    SetStringFilter(DatItem_Location_Number, value, negate);
                    break;

                case Field.DatItem_Location_Inverted:
                    SetBooleanFilter(DatItem_Location_Inverted, value, negate);
                    break;

                // RamOption
                case Field.DatItem_Content:
                    SetStringFilter(DatItem_Content, value, negate);
                    break;

                // Release
                case Field.DatItem_Language:
                    SetStringFilter(DatItem_Language, value, negate);
                    break;

                // Setting
                case Field.DatItem_Setting_Name:
                    SetStringFilter(DatItem_Setting_Name, value, negate);
                    break;

                case Field.DatItem_Setting_Value:
                    SetStringFilter(DatItem_Setting_Value, value, negate);
                    break;

                case Field.DatItem_Setting_Default:
                    SetBooleanFilter(DatItem_Setting_Default, value, negate);
                    break;

                // SlotOption
                case Field.DatItem_SlotOption_Name:
                    SetStringFilter(DatItem_SlotOption_Name, value, negate);
                    break;

                case Field.DatItem_SlotOption_DeviceName:
                    SetStringFilter(DatItem_SlotOption_DeviceName, value, negate);
                    break;

                case Field.DatItem_SlotOption_Default:
                    SetBooleanFilter(DatItem_SlotOption_Default, value, negate);
                    break;

                // SoftwareList
                case Field.DatItem_SoftwareListStatus:
                    if (negate)
                        DatItem_SoftwareListStatus.Negative |= value.AsSoftwareListStatus();
                    else
                        DatItem_SoftwareListStatus.Positive |= value.AsSoftwareListStatus();
                    break;

                case Field.DatItem_Filter:
                    SetStringFilter(DatItem_Filter, value, negate);
                    break;

                // Sound
                case Field.DatItem_Channels:
                    SetOptionalLongFilter(DatItem_Channels, value, negate);
                    break;

                #endregion

                #endregion // Item-Specific

                #endregion // DatItem Filters
            }
        }

        /// <summary>
        /// Set a bool? filter
        /// </summary>
        /// <param name="filterItem">FilterItem to populate</param>
        /// <param name="value">String value to add</param>
        /// <param name="negate">True to set negative filter, false otherwise</param>
        private void SetBooleanFilter(FilterItem<bool?> filterItem, string value, bool negate)
        {
            if (negate || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                filterItem.Neutral = false;
            else
                filterItem.Neutral = true;
        }

        /// <summary>
        /// Set a long filter
        /// </summary>
        /// <param name="filterItem">FilterItem to populate</param>
        /// <param name="value">String value to add</param>
        /// <param name="negate">True to set negative filter, false otherwise</param>
        /// TODO: Can anything using this go with SetOptionalLongFilter instead?
        private void SetLongFilter(FilterItem<long> filterItem, string value, bool negate)
        {
            bool? operation = null;
            if (value.StartsWith(">"))
                operation = true;
            else if (value.StartsWith("<"))
                operation = false;
            else if (value.StartsWith("="))
                operation = null;

            string valueString = value.TrimStart('>', '<', '=');
            if (!Int64.TryParse(valueString, out long valueLong))
                return;

            // Equal
            if (operation == null && !negate)
            {
                filterItem.Neutral = valueLong;
            }

            // Not Equal
            else if (operation == null && negate)
            {
                filterItem.Negative = valueLong - 1;
                filterItem.Positive = valueLong + 1;
            }

            // Greater Than or Equal
            else if (operation == true && !negate)
            {
                filterItem.Positive = valueLong;
            }

            // Strictly Less Than
            else if (operation == true && negate)
            {
                filterItem.Negative = valueLong - 1;
            }

            // Less Than or Equal
            else if (operation == false && !negate)
            {
                filterItem.Negative = valueLong;
            }

            // Strictly Greater Than
            else if (operation == false && negate)
            {
                filterItem.Positive = valueLong + 1;
            }
        }

        /// <summary>
        /// Set a long? filter
        /// </summary>
        /// <param name="filterItem">FilterItem to populate</param>
        /// <param name="value">String value to add</param>
        /// <param name="negate">True to set negative filter, false otherwise</param>
        private void SetOptionalLongFilter(FilterItem<long?> filterItem, string value, bool negate)
        {
            bool? operation = null;
            if (value.StartsWith(">"))
                operation = true;
            else if (value.StartsWith("<"))
                operation = false;
            else if (value.StartsWith("="))
                operation = null;

            string valueString = value.TrimStart('>', '<', '=');
            if (!Int64.TryParse(valueString, out long valueLong))
                return;

            // Equal
            if (operation == null && !negate)
            {
                filterItem.Neutral = valueLong;
            }

            // Not Equal
            else if (operation == null && negate)
            {
                filterItem.Negative = valueLong - 1;
                filterItem.Positive = valueLong + 1;
            }

            // Greater Than or Equal
            else if (operation == true && !negate)
            {
                filterItem.Positive = valueLong;
            }

            // Strictly Less Than
            else if (operation == true && negate)
            {
                filterItem.Negative = valueLong - 1;
            }

            // Less Than or Equal
            else if (operation == false && !negate)
            {
                filterItem.Negative = valueLong;
            }

            // Strictly Greater Than
            else if (operation == false && negate)
            {
                filterItem.Positive = valueLong + 1;
            }
        }

        /// <summary>
        /// Set a string filter
        /// </summary>
        /// <param name="filterItem">FilterItem to populate</param>
        /// <param name="value">String value to add</param>
        /// <param name="negate">True to set negative filter, false otherwise</param>
        private void SetStringFilter(FilterItem<string> filterItem, string value, bool negate)
        {
            if (negate)
                filterItem.NegativeSet.Add(value);
            else
                filterItem.PositiveSet.Add(value);
        }

        #endregion

        #endregion // Instance Methods
    }
}
