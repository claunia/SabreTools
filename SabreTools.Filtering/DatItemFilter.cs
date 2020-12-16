using System.Collections.Generic;

using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.Logging;

namespace SabreTools.Filtering
{
    /// <summary>
    /// Represents the filtering operations that need to be performed on a set of items, usually a DAT
    /// </summary>
    public class DatItemFilter : Filter
    {
        #region Fields

        #region Common

        public FilterItem<string> Type { get; private set; } = new FilterItem<string>();

        #endregion

        #region Item-Specific

        #region Actionable

        // Rom
        public FilterItem<string> Name { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Bios { get; private set; } = new FilterItem<string>();
        public FilterItem<long?> Size { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };
        public FilterItem<string> CRC { get; private set; } = new FilterItem<string>();
        public FilterItem<string> MD5 { get; private set; } = new FilterItem<string>();
        public FilterItem<string> SHA1 { get; private set; } = new FilterItem<string>();
        public FilterItem<string> SHA256 { get; private set; } = new FilterItem<string>();
        public FilterItem<string> SHA384 { get; private set; } = new FilterItem<string>();
        public FilterItem<string> SHA512 { get; private set; } = new FilterItem<string>();
        public FilterItem<string> SpamSum { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Merge { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Region { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Offset { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Date { get; private set; } = new FilterItem<string>();
        public FilterItem<ItemStatus> Status { get; private set; } = new FilterItem<ItemStatus>() { Positive = ItemStatus.NULL, Negative = ItemStatus.NULL };
        public FilterItem<bool?> Optional { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<bool?> Inverted { get; private set; } = new FilterItem<bool?>();

        // Rom (AttractMode)
        public FilterItem<string> AltName { get; private set; } = new FilterItem<string>();
        public FilterItem<string> AltTitle { get; private set; } = new FilterItem<string>();

        // Rom (OpenMSX)
        public FilterItem<string> Original { get; private set; } = new FilterItem<string>();
        public FilterItem<OpenMSXSubType> OpenMSXSubType { get; private set; } = new FilterItem<OpenMSXSubType>() { Positive = Core.OpenMSXSubType.NULL, Negative = Core.OpenMSXSubType.NULL };
        public FilterItem<string> OpenMSXType { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Remark { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Boot { get; private set; } = new FilterItem<string>();

        // Rom (SoftwareList)
        public FilterItem<LoadFlag> LoadFlag { get; private set; } = new FilterItem<LoadFlag>() { Positive = Core.LoadFlag.NULL, Negative = Core.LoadFlag.NULL };
        public FilterItem<string> Value { get; private set; } = new FilterItem<string>();

        // Disk
        public FilterItem<string> Index { get; private set; } = new FilterItem<string>();
        public FilterItem<bool?> Writable { get; private set; } = new FilterItem<bool?>() { Neutral = null };

        #endregion

        #region Auxiliary

        // Adjuster
        public FilterItem<bool?> Default { get; private set; } = new FilterItem<bool?>() { Neutral = null };

        // Analog
        public FilterItem<string> Analog_Mask { get; private set; } = new FilterItem<string>();

        // BiosSet
        public FilterItem<string> Description { get; private set; } = new FilterItem<string>();

        // Chip
        public FilterItem<string> Tag { get; private set; } = new FilterItem<string>();
        public FilterItem<ChipType> ChipType { get; private set; } = new FilterItem<ChipType>() { Positive = Core.ChipType.NULL, Negative = Core.ChipType.NULL };
        public FilterItem<long?> Clock { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };

        // Condition
        public FilterItem<string> Mask { get; private set; } = new FilterItem<string>();
        public FilterItem<Relation> Relation { get; private set; } = new FilterItem<Relation>() { Positive = Core.Relation.NULL, Negative = Core.Relation.NULL };
        public FilterItem<string> Condition_Tag { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Condition_Mask { get; private set; } = new FilterItem<string>();
        public FilterItem<Relation> Condition_Relation { get; private set; } = new FilterItem<Relation>() { Positive = Core.Relation.NULL, Negative = Core.Relation.NULL };
        public FilterItem<string> Condition_Value { get; private set; } = new FilterItem<string>();

        // Control
        public FilterItem<ControlType> Control_Type { get; private set; } = new FilterItem<ControlType>() { Positive = ControlType.NULL, Negative = ControlType.NULL };
        public FilterItem<long?> Control_Player { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };
        public FilterItem<long?> Control_Buttons { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };
        public FilterItem<long?> Control_ReqButtons { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };
        public FilterItem<long?> Control_Minimum { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };
        public FilterItem<long?> Control_Maximum { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };
        public FilterItem<long?> Control_Sensitivity { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };
        public FilterItem<long?> Control_KeyDelta { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };
        public FilterItem<bool?> Control_Reverse { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> Control_Ways { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Control_Ways2 { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Control_Ways3 { get; private set; } = new FilterItem<string>();

        // DataArea
        public FilterItem<string> AreaName { get; private set; } = new FilterItem<string>();
        public FilterItem<long?> AreaSize { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };
        public FilterItem<long?> AreaWidth { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };
        public FilterItem<Endianness> AreaEndianness { get; private set; } = new FilterItem<Endianness>() { Positive = Endianness.NULL, Negative = Endianness.NULL };

        // Device
        public FilterItem<DeviceType> DeviceType { get; private set; } = new FilterItem<DeviceType>() { Positive = Core.DeviceType.NULL, Negative = Core.DeviceType.NULL };
        public FilterItem<string> FixedImage { get; private set; } = new FilterItem<string>();
        public FilterItem<long?> Mandatory { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };
        public FilterItem<string> Interface { get; private set; } = new FilterItem<string>();

        // Display
        public FilterItem<DisplayType> DisplayType { get; private set; } = new FilterItem<DisplayType>() { Positive = Core.DisplayType.NULL, Negative = Core.DisplayType.NULL };
        public FilterItem<long?> Rotate { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };
        public FilterItem<bool?> FlipX { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<long?> Width { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };
        public FilterItem<long?> Height { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };
        public FilterItem<double?> Refresh { get; private set; } = new FilterItem<double?>() { Positive = null, Negative = null, Neutral = null };
        public FilterItem<long?> PixClock { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };
        public FilterItem<long?> HTotal { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };
        public FilterItem<long?> HBEnd { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };
        public FilterItem<long?> HBStart { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };
        public FilterItem<long?> VTotal { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };
        public FilterItem<long?> VBEnd { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };
        public FilterItem<long?> VBStart { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };

        // Driver
        public FilterItem<SupportStatus> SupportStatus { get; private set; } = new FilterItem<SupportStatus>() { Positive = Core.SupportStatus.NULL, Negative = Core.SupportStatus.NULL };
        public FilterItem<SupportStatus> EmulationStatus { get; private set; } = new FilterItem<SupportStatus>() { Positive = Core.SupportStatus.NULL, Negative = Core.SupportStatus.NULL };
        public FilterItem<SupportStatus> CocktailStatus { get; private set; } = new FilterItem<SupportStatus>() { Positive = Core.SupportStatus.NULL, Negative = Core.SupportStatus.NULL };
        public FilterItem<Supported> SaveStateStatus { get; private set; } = new FilterItem<Supported>() { Positive = Supported.NULL, Negative = Supported.NULL };

        // Extension
        public FilterItem<string> Extension_Name { get; private set; } = new FilterItem<string>();

        // Feature
        public FilterItem<FeatureType> FeatureType { get; private set; } = new FilterItem<FeatureType>() { Positive = Core.FeatureType.NULL, Negative = Core.FeatureType.NULL };
        public FilterItem<FeatureStatus> FeatureStatus { get; private set; } = new FilterItem<FeatureStatus>() { Positive = Core.FeatureStatus.NULL, Negative = Core.FeatureStatus.NULL };
        public FilterItem<FeatureStatus> FeatureOverall { get; private set; } = new FilterItem<FeatureStatus>() { Positive = Core.FeatureStatus.NULL, Negative = Core.FeatureStatus.NULL };

        // Input
        public FilterItem<bool?> Service { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<bool?> Tilt { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<long?> Players { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };
        public FilterItem<long?> Coins { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };

        // Instance
        public FilterItem<string> Instance_Name { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Instance_BriefName { get; private set; } = new FilterItem<string>();

        // Location
        public FilterItem<string> Location_Name { get; private set; } = new FilterItem<string>();
        public FilterItem<long?> Location_Number { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };
        public FilterItem<bool?> Location_Inverted { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        
        // Part
        public FilterItem<string> Part_Name { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Part_Interface { get; private set; } = new FilterItem<string>();
        
        // PartFeature
        public FilterItem<string> Part_Feature_Name { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Part_Feature_Value { get; private set; } = new FilterItem<string>();

        // RamOption
        public FilterItem<string> Content { get; private set; } = new FilterItem<string>();

        // Release
        public FilterItem<string> Language { get; private set; } = new FilterItem<string>();

        // Setting
        public FilterItem<string> Setting_Name { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Setting_Value { get; private set; } = new FilterItem<string>();
        public FilterItem<bool?> Setting_Default { get; private set; } = new FilterItem<bool?>() { Neutral = null };

        // SlotOption
        public FilterItem<string> SlotOption_Name { get; private set; } = new FilterItem<string>();
        public FilterItem<string> SlotOption_DeviceName { get; private set; } = new FilterItem<string>();
        public FilterItem<bool?> SlotOption_Default { get; private set; } = new FilterItem<bool?>() { Neutral = null };

        // SoftwareList
        public FilterItem<SoftwareListStatus> SoftwareListStatus { get; private set; } = new FilterItem<SoftwareListStatus>() { Positive = Core.SoftwareListStatus.NULL, Negative = Core.SoftwareListStatus.NULL };
        public FilterItem<string> Filter { get; private set; } = new FilterItem<string>();

        // Sound
        public FilterItem<long?> Channels { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };

        #endregion

        #endregion // Item-Specific

        #endregion // Fields

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public DatItemFilter()
        {
            logger = new Logger(this);
        }

        #endregion

        #region Filter Population

        /// <summary>
        /// Set multiple filters from key
        /// </summary>
        /// <param name="key">Key for the filter to be set</param>
        /// <param name="values">List of values for the filter</param>
        /// <param name="negate">True if negative filter, false otherwise</param>
        public void SetFilter(DatItemField key, List<string> values, bool negate)
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
        public void SetFilter(DatItemField key, string value, bool negate)
        {
            switch (key)
            {
                #region Common

                case DatItemField.Type:
                    if (value.AsItemType() == null)
                        return;

                    SetStringFilter(Type, value, negate);
                    break;

                #endregion

                #region Item-Specific

                #region Actionable

                // Rom
                case DatItemField.Name:
                    SetStringFilter(Name, value, negate);
                    break;

                case DatItemField.Bios:
                    SetStringFilter(Bios, value, negate);
                    break;

                case DatItemField.Size:
                    SetLongFilter(Size, value, negate);
                    break;

                case DatItemField.CRC:
                    SetStringFilter(CRC, value, negate);
                    break;

                case DatItemField.MD5:
                    SetStringFilter(MD5, value, negate);
                    break;

                case DatItemField.SHA1:
                    SetStringFilter(SHA1, value, negate);
                    break;

                case DatItemField.SHA256:
                    SetStringFilter(SHA256, value, negate);
                    break;

                case DatItemField.SHA384:
                    SetStringFilter(SHA384, value, negate);
                    break;

                case DatItemField.SHA512:
                    SetStringFilter(SHA512, value, negate);
                    break;

                case DatItemField.SpamSum:
                    SetStringFilter(SpamSum, value, negate);
                    break;

                case DatItemField.Merge:
                    SetStringFilter(Merge, value, negate);
                    break;

                case DatItemField.Region:
                    SetStringFilter(Region, value, negate);
                    break;

                case DatItemField.Offset:
                    SetStringFilter(Offset, value, negate);
                    break;

                case DatItemField.Date:
                    SetStringFilter(Date, value, negate);
                    break;

                case DatItemField.Status:
                    if (negate)
                        Status.Negative |= value.AsItemStatus();
                    else
                        Status.Positive |= value.AsItemStatus();
                    break;

                case DatItemField.Optional:
                    SetBooleanFilter(Optional, value, negate);
                    break;

                case DatItemField.Inverted:
                    SetBooleanFilter(Inverted, value, negate);
                    break;

                // Rom (AttractMode)
                case DatItemField.AltName:
                    SetStringFilter(AltName, value, negate);
                    break;

                case DatItemField.AltTitle:
                    SetStringFilter(AltTitle, value, negate);
                    break;

                // Rom (OpenMSX)
                case DatItemField.Original:
                    SetStringFilter(Original, value, negate);
                    break;

                case DatItemField.OpenMSXSubType:
                    if (negate)
                        OpenMSXSubType.Negative |= value.AsOpenMSXSubType();
                    else
                        OpenMSXSubType.Positive |= value.AsOpenMSXSubType();
                    break;

                case DatItemField.OpenMSXType:
                    SetStringFilter(OpenMSXType, value, negate);
                    break;

                case DatItemField.Remark:
                    SetStringFilter(Remark, value, negate);
                    break;

                case DatItemField.Boot:
                    SetStringFilter(Boot, value, negate);
                    break;

                // Rom (SoftwareList)
                case DatItemField.LoadFlag:
                    if (negate)
                        LoadFlag.Negative |= value.AsLoadFlag();
                    else
                        LoadFlag.Positive |= value.AsLoadFlag();
                    break;

                case DatItemField.Value:
                    SetStringFilter(Value, value, negate);
                    break;

                // Disk
                case DatItemField.Index:
                    SetStringFilter(Index, value, negate);
                    break;

                case DatItemField.Writable:
                    SetBooleanFilter(Writable, value, negate);
                    break;

                #endregion

                #region Auxiliary

                // Adjuster
                case DatItemField.Default:
                    SetBooleanFilter(Default, value, negate);
                    break;

                // Analog
                case DatItemField.Analog_Mask:
                    SetStringFilter(Analog_Mask, value, negate);
                    break;

                // BiosSet
                case DatItemField.Description:
                    SetStringFilter(Description, value, negate);
                    break;

                // Chip
                case DatItemField.Tag:
                    SetStringFilter(Tag, value, negate);
                    break;

                case DatItemField.ChipType:
                    if (negate)
                        ChipType.Negative |= value.AsChipType();
                    else
                        ChipType.Positive |= value.AsChipType();
                    break;

                case DatItemField.Clock:
                    SetLongFilter(Clock, value, negate);
                    break;

                // Condition
                case DatItemField.Mask:
                    SetStringFilter(Mask, value, negate);
                    break;

                case DatItemField.Relation:
                    if (negate)
                        Relation.Negative |= value.AsRelation();
                    else
                        Relation.Positive |= value.AsRelation();
                    break;

                case DatItemField.Condition_Tag:
                    SetStringFilter(Condition_Tag, value, negate);
                    break;

                case DatItemField.Condition_Mask:
                    SetStringFilter(Condition_Mask, value, negate);
                    break;

                case DatItemField.Condition_Relation:
                    if (negate)
                        Condition_Relation.Negative |= value.AsRelation();
                    else
                        Condition_Relation.Positive |= value.AsRelation();
                    break;

                case DatItemField.Condition_Value:
                    SetStringFilter(Condition_Value, value, negate);
                    break;

                // Control
                case DatItemField.Control_Type:

                    if (negate)
                        Control_Type.Negative |= value.AsControlType();
                    else
                        Control_Type.Positive |= value.AsControlType();
                    break;

                case DatItemField.Control_Player:
                    SetLongFilter(Control_Player, value, negate);
                    break;

                case DatItemField.Control_Buttons:
                    SetLongFilter(Control_Buttons, value, negate);
                    break;

                case DatItemField.Control_RequiredButtons:
                    SetLongFilter(Control_ReqButtons, value, negate);
                    break;

                case DatItemField.Control_Minimum:
                    SetLongFilter(Control_Minimum, value, negate);
                    break;

                case DatItemField.Control_Maximum:
                    SetLongFilter(Control_Maximum, value, negate);
                    break;

                case DatItemField.Control_Sensitivity:
                    SetLongFilter(Control_Sensitivity, value, negate);
                    break;

                case DatItemField.Control_KeyDelta:
                    SetLongFilter(Control_KeyDelta, value, negate);
                    break;

                case DatItemField.Control_Reverse:
                    SetBooleanFilter(Control_Reverse, value, negate);
                    break;

                case DatItemField.Control_Ways:
                    SetStringFilter(Control_Ways, value, negate);
                    break;

                case DatItemField.Control_Ways2:
                    SetStringFilter(Control_Ways2, value, negate);
                    break;

                case DatItemField.Control_Ways3:
                    SetStringFilter(Control_Ways3, value, negate);
                    break;

                // DataArea
                case DatItemField.AreaName:
                    SetStringFilter(AreaName, value, negate);
                    break;

                case DatItemField.AreaSize:
                    SetLongFilter(AreaSize, value, negate);
                    break;

                case DatItemField.AreaWidth:
                    SetLongFilter(AreaWidth, value, negate);
                    break;

                case DatItemField.AreaEndianness:
                    if (negate)
                        AreaEndianness.Negative |= value.AsEndianness();
                    else
                        AreaEndianness.Positive |= value.AsEndianness();
                    break;

                // Device
                case DatItemField.DeviceType:
                    if (negate)
                        DeviceType.Negative |= value.AsDeviceType();
                    else
                        DeviceType.Positive |= value.AsDeviceType();
                    break;

                case DatItemField.FixedImage:
                    SetStringFilter(FixedImage, value, negate);
                    break;

                case DatItemField.Mandatory:
                    SetLongFilter(Mandatory, value, negate);
                    break;

                case DatItemField.Interface:
                    SetStringFilter(Interface, value, negate);
                    break;

                // Display
                case DatItemField.DisplayType:
                    if (negate)
                        DisplayType.Negative |= value.AsDisplayType();
                    else
                        DisplayType.Positive |= value.AsDisplayType();
                    break;

                case DatItemField.Rotate:
                    SetLongFilter(Rotate, value, negate);
                    break;

                case DatItemField.FlipX:
                    SetBooleanFilter(FlipX, value, negate);
                    break;

                case DatItemField.Width:
                    SetLongFilter(Width, value, negate);
                    break;

                case DatItemField.Height:
                    SetLongFilter(Height, value, negate);
                    break;

                case DatItemField.Refresh:
                    SetDoubleFilter(Refresh, value, negate);
                    break;

                case DatItemField.PixClock:
                    SetLongFilter(PixClock, value, negate);
                    break;

                case DatItemField.HTotal:
                    SetLongFilter(HTotal, value, negate);
                    break;

                case DatItemField.HBEnd:
                    SetLongFilter(HBEnd, value, negate);
                    break;

                case DatItemField.HBStart:
                    SetLongFilter(HBStart, value, negate);
                    break;

                case DatItemField.VTotal:
                    SetLongFilter(VTotal, value, negate);
                    break;

                case DatItemField.VBEnd:
                    SetLongFilter(VBEnd, value, negate);
                    break;

                case DatItemField.VBStart:
                    SetLongFilter(VBStart, value, negate);
                    break;

                // Driver
                case DatItemField.SupportStatus:
                    if (negate)
                        SupportStatus.Negative |= value.AsSupportStatus();
                    else
                        SupportStatus.Positive |= value.AsSupportStatus();
                    break;

                case DatItemField.EmulationStatus:
                    if (negate)
                        EmulationStatus.Negative |= value.AsSupportStatus();
                    else
                        EmulationStatus.Positive |= value.AsSupportStatus();
                    break;

                case DatItemField.CocktailStatus:
                    if (negate)
                        CocktailStatus.Negative |= value.AsSupportStatus();
                    else
                        CocktailStatus.Positive |= value.AsSupportStatus();
                    break;

                case DatItemField.SaveStateStatus:
                    if (negate)
                        SaveStateStatus.Negative |= value.AsSupported();
                    else
                        SaveStateStatus.Positive |= value.AsSupported();
                    break;

                // Extension
                case DatItemField.Extension_Name:
                    SetStringFilter(Extension_Name, value, negate);
                    break;

                // Feature
                case DatItemField.FeatureType:
                    if (negate)
                        FeatureType.Negative |= value.AsFeatureType();
                    else
                        FeatureType.Positive |= value.AsFeatureType();
                    break;

                case DatItemField.FeatureStatus:
                    if (negate)
                        FeatureStatus.Negative |= value.AsFeatureStatus();
                    else
                        FeatureStatus.Positive |= value.AsFeatureStatus();
                    break;

                case DatItemField.FeatureOverall:
                    if (negate)
                        FeatureOverall.Negative |= value.AsFeatureStatus();
                    else
                        FeatureOverall.Positive |= value.AsFeatureStatus();
                    break;

                // Input
                case DatItemField.Service:
                    SetBooleanFilter(Service, value, negate);
                    break;

                case DatItemField.Tilt:
                    SetBooleanFilter(Tilt, value, negate);
                    break;

                case DatItemField.Players:
                    SetLongFilter(Players, value, negate);
                    break;

                case DatItemField.Coins:
                    SetLongFilter(Coins, value, negate);
                    break;

                // Instance
                case DatItemField.Instance_Name:
                    SetStringFilter(Instance_Name, value, negate);
                    break;

                case DatItemField.Instance_BriefName:
                    SetStringFilter(Instance_BriefName, value, negate);
                    break;

                // Location
                case DatItemField.Location_Name:
                    SetStringFilter(Location_Name, value, negate);
                    break;

                case DatItemField.Location_Number:
                    SetLongFilter(Location_Number, value, negate);
                    break;

                case DatItemField.Location_Inverted:
                    SetBooleanFilter(Location_Inverted, value, negate);
                    break;

                // Part
                case DatItemField.Part_Name:
                    SetStringFilter(Part_Name, value, negate);
                    break;

                case DatItemField.Part_Interface:
                    SetStringFilter(Part_Interface, value, negate);
                    break;

                // PartFeature
                case DatItemField.Part_Feature_Name:
                    SetStringFilter(Part_Feature_Name, value, negate);
                    break;

                case DatItemField.Part_Feature_Value:
                    SetStringFilter(Part_Feature_Value, value, negate);
                    break;

                // RamOption
                case DatItemField.Content:
                    SetStringFilter(Content, value, negate);
                    break;

                // Release
                case DatItemField.Language:
                    SetStringFilter(Language, value, negate);
                    break;

                // Setting
                case DatItemField.Setting_Name:
                    SetStringFilter(Setting_Name, value, negate);
                    break;

                case DatItemField.Setting_Value:
                    SetStringFilter(Setting_Value, value, negate);
                    break;

                case DatItemField.Setting_Default:
                    SetBooleanFilter(Setting_Default, value, negate);
                    break;

                // SlotOption
                case DatItemField.SlotOption_Name:
                    SetStringFilter(SlotOption_Name, value, negate);
                    break;

                case DatItemField.SlotOption_DeviceName:
                    SetStringFilter(SlotOption_DeviceName, value, negate);
                    break;

                case DatItemField.SlotOption_Default:
                    SetBooleanFilter(SlotOption_Default, value, negate);
                    break;

                // SoftwareList
                case DatItemField.SoftwareListStatus:
                    if (negate)
                        SoftwareListStatus.Negative |= value.AsSoftwareListStatus();
                    else
                        SoftwareListStatus.Positive |= value.AsSoftwareListStatus();
                    break;

                case DatItemField.Filter:
                    SetStringFilter(Filter, value, negate);
                    break;

                // Sound
                case DatItemField.Channels:
                    SetLongFilter(Channels, value, negate);
                    break;

                #endregion

                #endregion // Item-Specific
            }
        }
    
        #endregion
    }
}
