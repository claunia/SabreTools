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
        // TODO: Reorder once all reorganization is done
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

        public FilterItem<string> DatItem_Name { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Type { get; private set; } = new FilterItem<string>();

        #endregion

        #region AttractMode

        public FilterItem<string> DatItem_AltName { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_AltTitle { get; private set; } = new FilterItem<string>();

        #endregion

        #region OpenMSX

        public FilterItem<string> DatItem_Original { get; private set; } = new FilterItem<string>();
        public FilterItem<OpenMSXSubType> DatItem_OpenMSXSubType { get; private set; } = new FilterItem<OpenMSXSubType>() { Positive = OpenMSXSubType.NULL, Negative = OpenMSXSubType.NULL };
        public FilterItem<string> DatItem_OpenMSXType { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Remark { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Boot { get; private set; } = new FilterItem<string>();

        #endregion

        #region SoftwareList

        // Part
        public FilterItem<bool?> DatItem_Part { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> DatItem_Part_Name { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Part_Interface { get; private set; } = new FilterItem<string>();

        // Feature
        public FilterItem<bool?> DatItem_Features { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> DatItem_Feature_Name { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Feature_Value { get; private set; } = new FilterItem<string>();

        public FilterItem<string> DatItem_LoadFlag { get; private set; } = new FilterItem<string>();

        #endregion

        #region Item-Specific

        #region Actionable

        // Rom
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

        // Rom (SoftwareList)
        public FilterItem<string> DatItem_AreaName { get; private set; } = new FilterItem<string>();
        public FilterItem<long?> DatItem_AreaSize { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };
        public FilterItem<string> DatItem_AreaWidth { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_AreaEndianness { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Value { get; private set; } = new FilterItem<string>();

        // Disk
        public FilterItem<string> DatItem_Index { get; private set; } = new FilterItem<string>();
        public FilterItem<bool?> DatItem_Writable { get; private set; } = new FilterItem<bool?>() { Neutral = null };

        #endregion

        #region Auxiliary

        // Adjuster
        public FilterItem<bool?> DatItem_Conditions { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> DatItem_Condition_Tag { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Condition_Mask { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Condition_Relation { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Condition_Value { get; private set; } = new FilterItem<string>();

        // Analog
        public FilterItem<string> DatItem_Mask { get; private set; } = new FilterItem<string>();

        // BiosSet
        public FilterItem<string> DatItem_Description { get; private set; } = new FilterItem<string>();
        public FilterItem<bool?> DatItem_Default { get; private set; } = new FilterItem<bool?>() { Neutral = null };

        // Chip
        public FilterItem<string> DatItem_Tag { get; private set; } = new FilterItem<string>();
        public FilterItem<ChipType> DatItem_ChipType { get; private set; } = new FilterItem<ChipType>() { Positive = ChipType.NULL, Negative = ChipType.NULL };
        public FilterItem<string> DatItem_Clock { get; private set; } = new FilterItem<string>();

        // Condition
        public FilterItem<string> DatItem_Relation { get; private set; } = new FilterItem<string>();

        // Configuration.Locations
        public FilterItem<bool?> DatItem_Locations { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> DatItem_Location_Name { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Location_Number { get; private set; } = new FilterItem<string>();
        public FilterItem<bool?> DatItem_Location_Inverted { get; private set; } = new FilterItem<bool?>() { Neutral = null };

        // Configuration.Settings
        public FilterItem<bool?> DatItem_Settings { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> DatItem_Setting_Name { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Setting_Value { get; private set; } = new FilterItem<string>();
        public FilterItem<bool?> DatItem_Setting_Default { get; private set; } = new FilterItem<bool?>() { Neutral = null };

        // Device
        public FilterItem<string> DatItem_DeviceType { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_FixedImage { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Mandatory { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Interface { get; private set; } = new FilterItem<string>();

        // Device.Instances
        public FilterItem<bool?> DatItem_Instances { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> DatItem_Instance_Name { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Instance_BriefName { get; private set; } = new FilterItem<string>();

        // Device.Extensions
        public FilterItem<bool?> DatItem_Extensions { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> DatItem_Extension_Name { get; private set; } = new FilterItem<string>();

        // DipSwitch.Values
        public FilterItem<bool?> DatItem_Values { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> DatItem_Value_Name { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Value_Value { get; private set; } = new FilterItem<string>();
        public FilterItem<bool?> DatItem_Value_Default { get; private set; } = new FilterItem<bool?>() { Neutral = null };

        // Display
        public FilterItem<string> DatItem_DisplayType { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Rotate { get; private set; } = new FilterItem<string>();
        public FilterItem<bool?> DatItem_FlipX { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> DatItem_Width { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Height { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Refresh { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_PixClock { get; private set; } = new FilterItem<string>();
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

        // Feature
        public FilterItem<FeatureType> DatItem_FeatureType { get; private set; } = new FilterItem<FeatureType>() { Positive = FeatureType.NULL, Negative = FeatureType.NULL };
        public FilterItem<FeatureStatus> DatItem_FeatureStatus { get; private set; } = new FilterItem<FeatureStatus>() { Positive = FeatureStatus.NULL, Negative = FeatureStatus.NULL };
        public FilterItem<FeatureStatus> DatItem_FeatureOverall { get; private set; } = new FilterItem<FeatureStatus>() { Positive = FeatureStatus.NULL, Negative = FeatureStatus.NULL };

        // Input
        public FilterItem<bool?> DatItem_Service { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<bool?> DatItem_Tilt { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> DatItem_Players { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_Coins { get; private set; } = new FilterItem<string>();

        // Input.Controls
        public FilterItem<bool?> DatItem_Controls { get; private set; } = new FilterItem<bool?>() { Neutral = null };
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

        // Port.Analogs
        public FilterItem<bool?> DatItem_Analogs { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> DatItem_Analog_Mask { get; private set; } = new FilterItem<string>();

        // Ram Option
        public FilterItem<string> DatItem_Content { get; private set; } = new FilterItem<string>();

        // Release
        public FilterItem<string> DatItem_Language { get; private set; } = new FilterItem<string>();

        // Slots.SlotOptions
        public FilterItem<bool?> DatItem_SlotOptions { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> DatItem_SlotOption_Name { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DatItem_SlotOption_DeviceName { get; private set; } = new FilterItem<string>();
        public FilterItem<bool?> DatItem_SlotOption_Default { get; private set; } = new FilterItem<bool?>() { Neutral = null };

        // Software List
        public FilterItem<SoftwareListStatus> DatItem_SoftwareListStatus { get; private set; } = new FilterItem<SoftwareListStatus>() { Positive = SoftwareListStatus.NULL, Negative = SoftwareListStatus.NULL };
        public FilterItem<string> DatItem_Filter { get; private set; } = new FilterItem<string>();

        // Sound
        public FilterItem<string> DatItem_Channels { get; private set; } = new FilterItem<string>();

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
                        DatItem_Name.NegativeSet.Add(value);
                    else
                        DatItem_Name.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Type:
                    if (value.AsItemType() == null)
                        return;

                    if (negate)
                        DatItem_Type.NegativeSet.Add(value);
                    else
                        DatItem_Type.PositiveSet.Add(value);
                    break;

                #endregion

                #region AttractMode

                case Field.DatItem_AltName:
                    if (negate)
                        DatItem_AltName.NegativeSet.Add(value);
                    else
                        DatItem_AltName.PositiveSet.Add(value);
                    break;

                case Field.DatItem_AltTitle:
                    if (negate)
                        DatItem_AltTitle.NegativeSet.Add(value);
                    else
                        DatItem_AltTitle.PositiveSet.Add(value);
                    break;

                #endregion

                #region OpenMSX

                case Field.DatItem_Original:
                    if (negate)
                        DatItem_Original.NegativeSet.Add(value);
                    else
                        DatItem_Original.PositiveSet.Add(value);
                    break;

                case Field.DatItem_OpenMSXSubType:
                    if (negate)
                        DatItem_OpenMSXSubType.Negative |= value.AsOpenMSXSubType();
                    else
                        DatItem_OpenMSXSubType.Positive |= value.AsOpenMSXSubType();
                    break;

                case Field.DatItem_OpenMSXType:
                    if (negate)
                        DatItem_OpenMSXType.NegativeSet.Add(value);
                    else
                        DatItem_OpenMSXType.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Remark:
                    if (negate)
                        DatItem_Remark.NegativeSet.Add(value);
                    else
                        DatItem_Remark.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Boot:
                    if (negate)
                        DatItem_Boot.NegativeSet.Add(value);
                    else
                        DatItem_Boot.PositiveSet.Add(value);
                    break;

                #endregion

                #region SoftwareList

                // Part
                case Field.DatItem_Part:
                    if (negate || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                        DatItem_Part.Neutral = false;
                    else
                        DatItem_Part.Neutral = true;
                    break;

                case Field.DatItem_Part_Name:
                    if (negate)
                        DatItem_Part_Name.NegativeSet.Add(value);
                    else
                        DatItem_Part_Name.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Part_Interface:
                    if (negate)
                        DatItem_Part_Interface.NegativeSet.Add(value);
                    else
                        DatItem_Part_Interface.PositiveSet.Add(value);
                    break;

                // Feature
                case Field.DatItem_Features:
                    if (negate || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                        DatItem_Features.Neutral = false;
                    else
                        DatItem_Features.Neutral = true;
                    break;

                case Field.DatItem_Feature_Name:
                    if (negate)
                        DatItem_Feature_Name.NegativeSet.Add(value);
                    else
                        DatItem_Feature_Name.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Feature_Value:
                    if (negate)
                        DatItem_Feature_Value.NegativeSet.Add(value);
                    else
                        DatItem_Feature_Value.PositiveSet.Add(value);
                    break;

                case Field.DatItem_AreaName:
                    if (negate)
                        DatItem_AreaName.NegativeSet.Add(value);
                    else
                        DatItem_AreaName.PositiveSet.Add(value);
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
                        DatItem_AreaSize.Neutral = areasize;
                    }

                    // Not Equal
                    else if (asOperation == null && negate)
                    {
                        DatItem_AreaSize.Negative = areasize - 1;
                        DatItem_AreaSize.Positive = areasize + 1;
                    }

                    // Greater Than or Equal
                    else if (asOperation == true && !negate)
                    {
                        DatItem_AreaSize.Positive = areasize;
                    }

                    // Strictly Less Than
                    else if (asOperation == true && negate)
                    {
                        DatItem_AreaSize.Negative = areasize - 1;
                    }

                    // Less Than or Equal
                    else if (asOperation == false && !negate)
                    {
                        DatItem_AreaSize.Negative = areasize;
                    }

                    // Strictly Greater Than
                    else if (asOperation == false && negate)
                    {
                        DatItem_AreaSize.Positive = areasize + 1;
                    }

                    break;

                case Field.DatItem_AreaWidth:
                    if (negate)
                        DatItem_AreaWidth.NegativeSet.Add(value);
                    else
                        DatItem_AreaWidth.PositiveSet.Add(value);
                    break;

                case Field.DatItem_AreaEndianness:
                    if (negate)
                        DatItem_AreaEndianness.NegativeSet.Add(value);
                    else
                        DatItem_AreaEndianness.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Value:
                    if (negate)
                        DatItem_Value.NegativeSet.Add(value);
                    else
                        DatItem_Value.PositiveSet.Add(value);
                    break;

                case Field.DatItem_LoadFlag:
                    if (negate)
                        DatItem_LoadFlag.NegativeSet.Add(value);
                    else
                        DatItem_LoadFlag.PositiveSet.Add(value);
                    break;

                #endregion

                #region Item-Specific

                #region Actionable

                // Rom
                case Field.DatItem_Bios:
                    if (negate)
                        DatItem_Bios.NegativeSet.Add(value);
                    else
                        DatItem_Bios.PositiveSet.Add(value);
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
                        DatItem_Size.Neutral = size;
                    }

                    // Not Equal
                    else if (sOperation == null && negate)
                    {
                        DatItem_Size.Negative = size - 1;
                        DatItem_Size.Positive = size + 1;
                    }

                    // Greater Than or Equal
                    else if (sOperation == true && !negate)
                    {
                        DatItem_Size.Positive = size;
                    }

                    // Strictly Less Than
                    else if (sOperation == true && negate)
                    {
                        DatItem_Size.Negative = size - 1;
                    }

                    // Less Than or Equal
                    else if (sOperation == false && !negate)
                    {
                        DatItem_Size.Negative = size;
                    }

                    // Strictly Greater Than
                    else if (sOperation == false && negate)
                    {
                        DatItem_Size.Positive = size + 1;
                    }

                    break;

                case Field.DatItem_CRC:
                    if (negate)
                        DatItem_CRC.NegativeSet.Add(value);
                    else
                        DatItem_CRC.PositiveSet.Add(value);
                    break;

                case Field.DatItem_MD5:
                    if (negate)
                        DatItem_MD5.NegativeSet.Add(value);
                    else
                        DatItem_MD5.PositiveSet.Add(value);
                    break;

#if NET_FRAMEWORK
                case Field.DatItem_RIPEMD160:
                    if (negate)
                        DatItem_RIPEMD160.NegativeSet.Add(value);
                    else
                        DatItem_RIPEMD160.PositiveSet.Add(value);
                    break;
#endif

                case Field.DatItem_SHA1:
                    if (negate)
                        DatItem_SHA1.NegativeSet.Add(value);
                    else
                        DatItem_SHA1.PositiveSet.Add(value);
                    break;

                case Field.DatItem_SHA256:
                    if (negate)
                        DatItem_SHA256.NegativeSet.Add(value);
                    else
                        DatItem_SHA256.PositiveSet.Add(value);
                    break;

                case Field.DatItem_SHA384:
                    if (negate)
                        DatItem_SHA384.NegativeSet.Add(value);
                    else
                        DatItem_SHA384.PositiveSet.Add(value);
                    break;

                case Field.DatItem_SHA512:
                    if (negate)
                        DatItem_SHA512.NegativeSet.Add(value);
                    else
                        DatItem_SHA512.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Merge:
                    if (negate)
                        DatItem_Merge.NegativeSet.Add(value);
                    else
                        DatItem_Merge.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Region:
                    if (negate)
                        DatItem_Region.NegativeSet.Add(value);
                    else
                        DatItem_Region.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Offset:
                    if (negate)
                        DatItem_Offset.NegativeSet.Add(value);
                    else
                        DatItem_Offset.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Date:
                    if (negate)
                        DatItem_Date.NegativeSet.Add(value);
                    else
                        DatItem_Date.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Status:
                    if (negate)
                        DatItem_Status.Negative |= value.AsItemStatus();
                    else
                        DatItem_Status.Positive |= value.AsItemStatus();
                    break;

                case Field.DatItem_Optional:
                    if (negate || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                        DatItem_Optional.Neutral = false;
                    else
                        DatItem_Optional.Neutral = true;
                    break;

                case Field.DatItem_Inverted:
                    if (negate || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                        DatItem_Inverted.Neutral = false;
                    else
                        DatItem_Inverted.Neutral = true;
                    break;

                // Disk
                case Field.DatItem_Index:
                    if (negate)
                        DatItem_Index.NegativeSet.Add(value);
                    else
                        DatItem_Index.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Writable:
                    if (negate || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                        DatItem_Writable.Neutral = false;
                    else
                        DatItem_Writable.Neutral = true;
                    break;

                #endregion

                #region Auxiliary

                // Adjuster
                case Field.DatItem_Default:
                    if (negate || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                        DatItem_Default.Neutral = false;
                    else
                        DatItem_Default.Neutral = true;
                    break;

                case Field.DatItem_Conditions:
                    if (negate || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                        DatItem_Conditions.Neutral = false;
                    else
                        DatItem_Conditions.Neutral = true;
                    break;

                case Field.DatItem_Condition_Tag:
                    if (negate)
                        DatItem_Condition_Tag.NegativeSet.Add(value);
                    else
                        DatItem_Condition_Tag.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Condition_Mask:
                    if (negate)
                        DatItem_Condition_Mask.NegativeSet.Add(value);
                    else
                        DatItem_Condition_Mask.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Condition_Relation:
                    if (negate)
                        DatItem_Condition_Relation.NegativeSet.Add(value);
                    else
                        DatItem_Condition_Relation.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Condition_Value:
                    if (negate)
                        DatItem_Condition_Value.NegativeSet.Add(value);
                    else
                        DatItem_Condition_Value.PositiveSet.Add(value);
                    break;

                // Analog
                case Field.DatItem_Mask:
                    if (negate)
                        DatItem_Mask.NegativeSet.Add(value);
                    else
                        DatItem_Mask.PositiveSet.Add(value);
                    break;

                // BiosSet
                case Field.DatItem_Description:
                    if (negate)
                        DatItem_Description.NegativeSet.Add(value);
                    else
                        DatItem_Description.PositiveSet.Add(value);
                    break;

                // Chip
                case Field.DatItem_Tag:
                    if (negate)
                        DatItem_Tag.NegativeSet.Add(value);
                    else
                        DatItem_Tag.PositiveSet.Add(value);
                    break;

                case Field.DatItem_ChipType:
                    if (negate)
                        DatItem_ChipType.Negative |= value.AsChipType();
                    else
                        DatItem_ChipType.Positive |= value.AsChipType();
                    break;

                case Field.DatItem_Clock:
                    if (negate)
                        DatItem_Clock.NegativeSet.Add(value);
                    else
                        DatItem_Clock.PositiveSet.Add(value);
                    break;

                // Condition
                case Field.DatItem_Relation:
                    if (negate)
                        DatItem_Relation.NegativeSet.Add(value);
                    else
                        DatItem_Relation.PositiveSet.Add(value);
                    break;

                // Configurations.Locations
                case Field.DatItem_Locations:
                    if (negate || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                        DatItem_Locations.Neutral = false;
                    else
                        DatItem_Locations.Neutral = true;
                    break;

                case Field.DatItem_Location_Name:
                    if (negate)
                        DatItem_Location_Name.NegativeSet.Add(value);
                    else
                        DatItem_Location_Name.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Location_Number:
                    if (negate)
                        DatItem_Location_Number.NegativeSet.Add(value);
                    else
                        DatItem_Location_Number.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Location_Inverted:
                    if (negate || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                        DatItem_Location_Inverted.Neutral = false;
                    else
                        DatItem_Location_Inverted.Neutral = true;
                    break;

                // Configurations.Settings
                case Field.DatItem_Settings:
                    if (negate || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                        DatItem_Settings.Neutral = false;
                    else
                        DatItem_Settings.Neutral = true;
                    break;

                case Field.DatItem_Setting_Name:
                    if (negate)
                        DatItem_Setting_Name.NegativeSet.Add(value);
                    else
                        DatItem_Setting_Name.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Setting_Value:
                    if (negate)
                        DatItem_Setting_Value.NegativeSet.Add(value);
                    else
                        DatItem_Setting_Value.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Setting_Default:
                    if (negate || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                        DatItem_Setting_Default.Neutral = false;
                    else
                        DatItem_Setting_Default.Neutral = true;
                    break;

                // Device
                case Field.DatItem_DeviceType:
                    if (negate)
                        DatItem_DeviceType.NegativeSet.Add(value);
                    else
                        DatItem_DeviceType.PositiveSet.Add(value);
                    break;

                case Field.DatItem_FixedImage:
                    if (negate)
                        DatItem_FixedImage.NegativeSet.Add(value);
                    else
                        DatItem_FixedImage.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Mandatory:
                    if (negate)
                        DatItem_Mandatory.NegativeSet.Add(value);
                    else
                        DatItem_Mandatory.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Interface:
                    if (negate)
                        DatItem_Interface.NegativeSet.Add(value);
                    else
                        DatItem_Interface.PositiveSet.Add(value);
                    break;

                // Devices.Instances
                case Field.DatItem_Instances:
                    if (negate || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                        DatItem_Instances.Neutral = false;
                    else
                        DatItem_Instances.Neutral = true;
                    break;

                case Field.DatItem_Instance_Name:
                    if (negate)
                        DatItem_Instance_Name.NegativeSet.Add(value);
                    else
                        DatItem_Instance_Name.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Instance_BriefName:
                    if (negate)
                        DatItem_Instance_BriefName.NegativeSet.Add(value);
                    else
                        DatItem_Instance_BriefName.PositiveSet.Add(value);
                    break;

                // Devices.Extensions
                case Field.DatItem_Extensions:
                    if (negate || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                        DatItem_Extensions.Neutral = false;
                    else
                        DatItem_Extensions.Neutral = true;
                    break;

                case Field.DatItem_Extension_Name:
                    if (negate)
                        DatItem_Extension_Name.NegativeSet.Add(value);
                    else
                        DatItem_Extension_Name.PositiveSet.Add(value);
                    break;

                // DipSwitches.Values
                case Field.DatItem_Values:
                    if (negate || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                        DatItem_Values.Neutral = false;
                    else
                        DatItem_Values.Neutral = true;
                    break;

                case Field.DatItem_Value_Name:
                    if (negate)
                        DatItem_Value_Name.NegativeSet.Add(value);
                    else
                        DatItem_Value_Name.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Value_Value:
                    if (negate)
                        DatItem_Value_Value.NegativeSet.Add(value);
                    else
                        DatItem_Value_Value.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Value_Default:
                    if (negate || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                        DatItem_Value_Default.Neutral = false;
                    else
                        DatItem_Value_Default.Neutral = true;
                    break;

                // Display
                case Field.DatItem_DisplayType:
                    if (negate)
                        DatItem_DisplayType.NegativeSet.Add(value);
                    else
                        DatItem_DisplayType.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Rotate:
                    if (negate)
                        DatItem_Rotate.NegativeSet.Add(value);
                    else
                        DatItem_Rotate.PositiveSet.Add(value);
                    break;

                case Field.DatItem_FlipX:
                    if (negate || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                        DatItem_FlipX.Neutral = false;
                    else
                        DatItem_FlipX.Neutral = true;
                    break;

                case Field.DatItem_Width:
                    if (negate)
                        DatItem_Width.NegativeSet.Add(value);
                    else
                        DatItem_Width.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Height:
                    if (negate)
                        DatItem_Height.NegativeSet.Add(value);
                    else
                        DatItem_Height.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Refresh:
                    if (negate)
                        DatItem_Refresh.NegativeSet.Add(value);
                    else
                        DatItem_Refresh.PositiveSet.Add(value);
                    break;

                case Field.DatItem_PixClock:
                    if (negate)
                        DatItem_PixClock.NegativeSet.Add(value);
                    else
                        DatItem_PixClock.PositiveSet.Add(value);
                    break;

                case Field.DatItem_HTotal:
                    if (negate)
                        DatItem_HTotal.NegativeSet.Add(value);
                    else
                        DatItem_HTotal.PositiveSet.Add(value);
                    break;

                case Field.DatItem_HBEnd:
                    if (negate)
                        DatItem_HBEnd.NegativeSet.Add(value);
                    else
                        DatItem_HBEnd.PositiveSet.Add(value);
                    break;

                case Field.DatItem_HBStart:
                    if (negate)
                        DatItem_HBStart.NegativeSet.Add(value);
                    else
                        DatItem_HBStart.PositiveSet.Add(value);
                    break;

                case Field.DatItem_VTotal:
                    if (negate)
                        DatItem_VTotal.NegativeSet.Add(value);
                    else
                        DatItem_VTotal.PositiveSet.Add(value);
                    break;

                case Field.DatItem_VBEnd:
                    if (negate)
                        DatItem_VBEnd.NegativeSet.Add(value);
                    else
                        DatItem_VBEnd.PositiveSet.Add(value);
                    break;

                case Field.DatItem_VBStart:
                    if (negate)
                        DatItem_VBStart.NegativeSet.Add(value);
                    else
                        DatItem_VBStart.PositiveSet.Add(value);
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
                    if (negate || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                        DatItem_Service.Neutral = false;
                    else
                        DatItem_Service.Neutral = true;
                    break;

                case Field.DatItem_Tilt:
                    if (negate || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                        DatItem_Tilt.Neutral = false;
                    else
                        DatItem_Tilt.Neutral = true;
                    break;

                case Field.DatItem_Players:
                    if (negate)
                        DatItem_Players.NegativeSet.Add(value);
                    else
                        DatItem_Players.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Coins:
                    if (negate)
                        DatItem_Coins.NegativeSet.Add(value);
                    else
                        DatItem_Coins.PositiveSet.Add(value);
                    break;

                // Input.Controls
                case Field.DatItem_Controls:
                    if (negate || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                        DatItem_Controls.Neutral = false;
                    else
                        DatItem_Controls.Neutral = true;
                    break;

                case Field.DatItem_Control_Type:
                    if (negate)
                        DatItem_Control_Type.NegativeSet.Add(value);
                    else
                        DatItem_Control_Type.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Control_Player:
                    if (negate)
                        DatItem_Control_Player.NegativeSet.Add(value);
                    else
                        DatItem_Control_Player.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Control_Buttons:
                    if (negate)
                        DatItem_Control_Buttons.NegativeSet.Add(value);
                    else
                        DatItem_Control_Buttons.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Control_RegButtons:
                    if (negate)
                        DatItem_Control_RegButtons.NegativeSet.Add(value);
                    else
                        DatItem_Control_RegButtons.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Control_Minimum:
                    if (negate)
                        DatItem_Control_Minimum.NegativeSet.Add(value);
                    else
                        DatItem_Control_Minimum.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Control_Maximum:
                    if (negate)
                        DatItem_Control_Maximum.NegativeSet.Add(value);
                    else
                        DatItem_Control_Maximum.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Control_Sensitivity:
                    if (negate)
                        DatItem_Control_Sensitivity.NegativeSet.Add(value);
                    else
                        DatItem_Control_Sensitivity.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Control_KeyDelta:
                    if (negate)
                        DatItem_Control_KeyDelta.NegativeSet.Add(value);
                    else
                        DatItem_Control_KeyDelta.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Control_Reverse:
                    if (negate || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                        DatItem_Control_Reverse.Neutral = false;
                    else
                        DatItem_Control_Reverse.Neutral = true;
                    break;

                case Field.DatItem_Control_Ways:
                    if (negate)
                        DatItem_Control_Ways.NegativeSet.Add(value);
                    else
                        DatItem_Control_Ways.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Control_Ways2:
                    if (negate)
                        DatItem_Control_Ways2.NegativeSet.Add(value);
                    else
                        DatItem_Control_Ways2.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Control_Ways3:
                    if (negate)
                        DatItem_Control_Ways3.NegativeSet.Add(value);
                    else
                        DatItem_Control_Ways3.PositiveSet.Add(value);
                    break;

                // Port.Analogs
                case Field.DatItem_Analogs:
                    if (negate || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                        DatItem_Analogs.Neutral = false;
                    else
                        DatItem_Analogs.Neutral = true;
                    break;

                case Field.DatItem_Analog_Mask:
                    if (negate)
                        DatItem_Analog_Mask.NegativeSet.Add(value);
                    else
                        DatItem_Analog_Mask.PositiveSet.Add(value);
                    break;

                // Ram Option
                case Field.DatItem_Content:
                    if (negate)
                        DatItem_Content.NegativeSet.Add(value);
                    else
                        DatItem_Content.PositiveSet.Add(value);
                    break;

                // Release
                case Field.DatItem_Language:
                    if (negate)
                        DatItem_Language.NegativeSet.Add(value);
                    else
                        DatItem_Language.PositiveSet.Add(value);
                    break;

                // Slots.SlotOptions
                case Field.DatItem_SlotOptions:
                    if (negate || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                        DatItem_SlotOptions.Neutral = false;
                    else
                        DatItem_SlotOptions.Neutral = true;
                    break;

                case Field.DatItem_SlotOption_Name:
                    if (negate)
                        DatItem_SlotOption_Name.NegativeSet.Add(value);
                    else
                        DatItem_SlotOption_Name.PositiveSet.Add(value);
                    break;

                case Field.DatItem_SlotOption_DeviceName:
                    if (negate)
                        DatItem_SlotOption_DeviceName.NegativeSet.Add(value);
                    else
                        DatItem_SlotOption_DeviceName.PositiveSet.Add(value);
                    break;

                case Field.DatItem_SlotOption_Default:
                    if (negate || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                        DatItem_SlotOption_Default.Neutral = false;
                    else
                        DatItem_SlotOption_Default.Neutral = true;
                    break;

                // Software List
                case Field.DatItem_SoftwareListStatus:
                    if (negate)
                        DatItem_SoftwareListStatus.Negative |= value.AsSoftwareListStatus();
                    else
                        DatItem_SoftwareListStatus.Positive |= value.AsSoftwareListStatus();
                    break;

                case Field.DatItem_Filter:
                    if (negate)
                        DatItem_Filter.NegativeSet.Add(value);
                    else
                        DatItem_Filter.PositiveSet.Add(value);
                    break;

                // Sound
                case Field.DatItem_Channels:
                    if (negate)
                        DatItem_Channels.NegativeSet.Add(value);
                    else
                        DatItem_Channels.PositiveSet.Add(value);
                    break;

                    #endregion

                #endregion // Item-Specifics

                #endregion // DatItem Filters
            }
        }

        #endregion

        #endregion // Instance Methods
    }
}
