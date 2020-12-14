using System.Collections.Generic;
using System.IO;
using System.Linq;

using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatItems;

namespace SabreTools.DatTools
{
    /// <summary>
    /// Manipulate DatItems
    /// </summary>
    /// TODO: Use these instead of the baked in ones
    public static class DatItemTool
    {
        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="datItem">DatItem to remove fields from</param>
        /// <param name="datItemFields">DatItem fields to remove</param>
        /// <param name="machineFields">Machine fields to remove</param>
        /// <param name="sub">True if this is a subitem, false otherwise</param>
        /// TODO: Extract out setting name to common
        public static void RemoveFields(
            DatItem datItem,
            List<DatItemField> datItemFields = null,
            List<MachineField> machineFields = null,
            bool sub = false)
        {
            if (datItem == null)
                return;

            #region Common

            if (machineFields != null && datItem.Machine != null)
                RemoveFields(datItem.Machine, machineFields);

            if (datItemFields == null)
                return;

            #endregion

            #region Adjuster

            if (datItem is Adjuster adjuster)
            {
                if (datItemFields.Contains(DatItemField.Name))
                    adjuster.Name = null;

                if (datItemFields.Contains(DatItemField.Default))
                    adjuster.Default = null;

                if (adjuster.ConditionsSpecified)
                {
                    foreach (Condition subCondition in adjuster.Conditions)
                    {
                        RemoveFields(subCondition, datItemFields, machineFields, true);
                    }
                }
            }

            #endregion

            #region Analog

            else if (datItem is Analog analog)
            {
                if (datItemFields.Contains(DatItemField.Analog_Mask))
                    analog.Mask = null;
            }

            #endregion

            #region Archive

            else if (datItem is Archive archive)
            {
                if (datItemFields.Contains(DatItemField.Name))
                    archive.Name = null;
            }

            #endregion

            #region BiosSet

            else if (datItem is BiosSet biosSet)
            {
                if (datItemFields.Contains(DatItemField.Name))
                    biosSet.Name = null;

                if (datItemFields.Contains(DatItemField.Description))
                    biosSet.Description = null;

                if (datItemFields.Contains(DatItemField.Default))
                    biosSet.Default = null;
            }

            #endregion

            #region Chip

            else if (datItem is Chip chip)
            {
                if (datItemFields.Contains(DatItemField.Name))
                    chip.Name = null;

                if (datItemFields.Contains(DatItemField.Tag))
                    chip.Tag = null;

                if (datItemFields.Contains(DatItemField.ChipType))
                    chip.ChipType = ChipType.NULL;

                if (datItemFields.Contains(DatItemField.Clock))
                    chip.Clock = null;
            }

            #endregion

            #region Condition

            else if (datItem is Condition condition)
            {
                if (sub)
                {
                    if (datItemFields.Contains(DatItemField.Condition_Tag))
                        condition.Tag = null;

                    if (datItemFields.Contains(DatItemField.Condition_Mask))
                        condition.Mask = null;

                    if (datItemFields.Contains(DatItemField.Condition_Relation))
                        condition.Relation = Relation.NULL;

                    if (datItemFields.Contains(DatItemField.Condition_Value))
                        condition.Value = null;
                }
                else
                {
                    if (datItemFields.Contains(DatItemField.Tag))
                        condition.Tag = null;

                    if (datItemFields.Contains(DatItemField.Mask))
                        condition.Mask = null;

                    if (datItemFields.Contains(DatItemField.Relation))
                        condition.Relation = Relation.NULL;

                    if (datItemFields.Contains(DatItemField.Value))
                        condition.Value = null;
                }
            }

            #endregion

            #region Configuration

            else if (datItem is Configuration configuration)
            {
                // Remove the fields
                if (datItemFields.Contains(DatItemField.Name))
                    configuration.Name = null;

                if (datItemFields.Contains(DatItemField.Tag))
                    configuration.Tag = null;

                if (datItemFields.Contains(DatItemField.Mask))
                    configuration.Mask = null;

                if (configuration.ConditionsSpecified)
                {
                    foreach (Condition subCondition in configuration.Conditions)
                    {
                        RemoveFields(subCondition, datItemFields, machineFields, true);
                    }
                }

                if (configuration.LocationsSpecified)
                {
                    foreach (Location subLocation in configuration.Locations)
                    {
                        RemoveFields(subLocation, datItemFields, machineFields);
                    }
                }

                if (configuration.SettingsSpecified)
                {
                    foreach (Setting subSetting in configuration.Settings)
                    {
                        RemoveFields(subSetting, datItemFields, machineFields);
                    }
                }
            }

            #endregion

            #region Control

            else if (datItem is Control control)
            {
                if (datItemFields.Contains(DatItemField.Control_Type))
                    control.ControlType = ControlType.NULL;

                if (datItemFields.Contains(DatItemField.Control_Player))
                    control.Player = null;

                if (datItemFields.Contains(DatItemField.Control_Buttons))
                    control.Buttons = null;

                if (datItemFields.Contains(DatItemField.Control_RequiredButtons))
                    control.RequiredButtons = null;

                if (datItemFields.Contains(DatItemField.Control_Minimum))
                    control.Minimum = null;

                if (datItemFields.Contains(DatItemField.Control_Maximum))
                    control.Maximum = null;

                if (datItemFields.Contains(DatItemField.Control_Sensitivity))
                    control.Sensitivity = null;

                if (datItemFields.Contains(DatItemField.Control_KeyDelta))
                    control.KeyDelta = null;

                if (datItemFields.Contains(DatItemField.Control_Reverse))
                    control.Reverse = null;

                if (datItemFields.Contains(DatItemField.Control_Ways))
                    control.Ways = null;

                if (datItemFields.Contains(DatItemField.Control_Ways2))
                    control.Ways2 = null;

                if (datItemFields.Contains(DatItemField.Control_Ways3))
                    control.Ways3 = null;
            }

            #endregion

            #region DataArea

            else if (datItem is DataArea dataArea)
            {
                if (datItemFields.Contains(DatItemField.AreaName))
                    dataArea.Name = null;

                if (datItemFields.Contains(DatItemField.AreaSize))
                    dataArea.Size = null;

                if (datItemFields.Contains(DatItemField.AreaWidth))
                    dataArea.Width = null;

                if (datItemFields.Contains(DatItemField.AreaEndianness))
                    dataArea.Endianness = Endianness.NULL;
            }

            #endregion

            #region Device

            else if (datItem is Device device)
            {
                if (datItemFields.Contains(DatItemField.DeviceType))
                    device.DeviceType = DeviceType.NULL;

                if (datItemFields.Contains(DatItemField.Tag))
                    device.Tag = null;

                if (datItemFields.Contains(DatItemField.FixedImage))
                    device.FixedImage = null;

                if (datItemFields.Contains(DatItemField.Mandatory))
                    device.Mandatory = null;

                if (datItemFields.Contains(DatItemField.Interface))
                    device.Interface = null;

                if (device.InstancesSpecified)
                {
                    foreach (Instance subInstance in device.Instances)
                    {
                        RemoveFields(subInstance, datItemFields, machineFields);
                    }
                }

                if (device.ExtensionsSpecified)
                {
                    foreach (Extension subExtension in device.Extensions)
                    {
                        RemoveFields(subExtension, datItemFields, machineFields);
                    }
                }
            }

            #endregion

            #region DeviceReference

            else if (datItem is DeviceReference deviceReference)
            {
                if (datItemFields.Contains(DatItemField.Name))
                    deviceReference.Name = null;
            }

            #endregion
        
            #region DipSwitch

            else if (datItem is DipSwitch dipSwitch)
            {
                #region Common

                if (datItemFields.Contains(DatItemField.Name))
                    dipSwitch.Name = null;

                if (datItemFields.Contains(DatItemField.Tag))
                    dipSwitch.Tag = null;

                if (datItemFields.Contains(DatItemField.Mask))
                    dipSwitch.Mask = null;

                if (dipSwitch.ConditionsSpecified)
                {
                    foreach (Condition subCondition in dipSwitch.Conditions)
                    {
                        RemoveFields(subCondition, datItemFields, machineFields, true);
                    }
                }

                if (dipSwitch.LocationsSpecified)
                {
                    foreach (Location subLocation in dipSwitch.Locations)
                    {
                        RemoveFields(subLocation, datItemFields, machineFields);
                    }
                }

                if (dipSwitch.ValuesSpecified)
                {
                    foreach (Setting subValue in dipSwitch.Values)
                    {
                        RemoveFields(subValue, datItemFields, machineFields);
                    }
                }

                #endregion

                #region SoftwareList

                if (dipSwitch.PartSpecified)
                    RemoveFields(dipSwitch.Part, datItemFields, machineFields);

                #endregion
            }

            #endregion

            #region Disk

            else if (datItem is Disk disk)
            {
                #region Common

                if (datItemFields.Contains(DatItemField.Name))
                    disk.Name = null;

                if (datItemFields.Contains(DatItemField.MD5))
                    disk.MD5 = null;

                if (datItemFields.Contains(DatItemField.SHA1))
                    disk.SHA1 = null;

                if (datItemFields.Contains(DatItemField.Merge))
                    disk.MergeTag = null;

                if (datItemFields.Contains(DatItemField.Region))
                    disk.Region = null;

                if (datItemFields.Contains(DatItemField.Index))
                    disk.Index = null;

                if (datItemFields.Contains(DatItemField.Writable))
                    disk.Writable = null;

                if (datItemFields.Contains(DatItemField.Status))
                    disk.ItemStatus = ItemStatus.NULL;

                if (datItemFields.Contains(DatItemField.Optional))
                    disk.Optional = null;

                #endregion

                #region SoftwareList

                if (disk.DiskAreaSpecified)
                    RemoveFields(disk.DiskArea, datItemFields, machineFields);

                if (disk.PartSpecified)
                    RemoveFields(disk.Part, datItemFields, machineFields);

                #endregion
            }

            #endregion

            #region DiskArea

            else if (datItem is DiskArea diskArea)
            {
                if (datItemFields.Contains(DatItemField.AreaName))
                    diskArea.Name = null;
            }

            #endregion

            #region Display

            else if (datItem is Display display)
            {
                if (datItemFields.Contains(DatItemField.Tag))
                    display.Tag = null;

                if (datItemFields.Contains(DatItemField.DisplayType))
                    display.DisplayType = DisplayType.NULL;

                if (datItemFields.Contains(DatItemField.Rotate))
                    display.Rotate = null;

                if (datItemFields.Contains(DatItemField.FlipX))
                    display.FlipX = null;

                if (datItemFields.Contains(DatItemField.Width))
                    display.Width = null;

                if (datItemFields.Contains(DatItemField.Height))
                    display.Height = null;

                if (datItemFields.Contains(DatItemField.Refresh))
                    display.Refresh = null;

                if (datItemFields.Contains(DatItemField.PixClock))
                    display.PixClock = null;

                if (datItemFields.Contains(DatItemField.HTotal))
                    display.HTotal = null;

                if (datItemFields.Contains(DatItemField.HBEnd))
                    display.HBEnd = null;

                if (datItemFields.Contains(DatItemField.HBStart))
                    display.HBStart = null;

                if (datItemFields.Contains(DatItemField.VTotal))
                    display.VTotal = null;

                if (datItemFields.Contains(DatItemField.VBEnd))
                    display.VBEnd = null;

                if (datItemFields.Contains(DatItemField.VBStart))
                    display.VBStart = null;
            }

            #endregion

            #region Driver

            else if (datItem is Driver driver)
            {
                if (datItemFields.Contains(DatItemField.SupportStatus))
                    driver.Status = SupportStatus.NULL;

                if (datItemFields.Contains(DatItemField.EmulationStatus))
                    driver.Emulation = SupportStatus.NULL;

                if (datItemFields.Contains(DatItemField.CocktailStatus))
                    driver.Cocktail = SupportStatus.NULL;

                if (datItemFields.Contains(DatItemField.SaveStateStatus))
                    driver.SaveState = Supported.NULL;
            }

            #endregion

            #region Extension

            else if (datItem is Extension extension)
            {
                if (datItemFields.Contains(DatItemField.Extension_Name))
                    extension.Name = null;
            }

            #endregion

            #region Feature

            else if (datItem is Feature feature)
            {
                if (datItemFields.Contains(DatItemField.FeatureType))
                    feature.Type = FeatureType.NULL;

                if (datItemFields.Contains(DatItemField.FeatureStatus))
                    feature.Status = FeatureStatus.NULL;

                if (datItemFields.Contains(DatItemField.FeatureOverall))
                    feature.Overall = FeatureStatus.NULL;
            }

            #endregion

            #region Info

            else if (datItem is Info info)
            {
                if (datItemFields.Contains(DatItemField.Name))
                    info.Name = null;

                if (datItemFields.Contains(DatItemField.Value))
                    info.Value = null;
            }

            #endregion

            #region Input

            else if (datItem is Input input)
            {
                if (datItemFields.Contains(DatItemField.Service))
                    input.Service = null;

                if (datItemFields.Contains(DatItemField.Tilt))
                    input.Tilt = null;

                if (datItemFields.Contains(DatItemField.Players))
                    input.Players = 0;

                if (datItemFields.Contains(DatItemField.Coins))
                    input.Coins = null;

                if (input.ControlsSpecified)
                {
                    foreach (Control subControl in input.Controls)
                    {
                        RemoveFields(subControl, datItemFields, machineFields);
                    }
                }
            }

            #endregion

            #region Instance

            else if (datItem is Instance instance)
            {
                if (datItemFields.Contains(DatItemField.Instance_Name))
                    instance.Name = null;

                if (datItemFields.Contains(DatItemField.Instance_BriefName))
                    instance.BriefName = null;
            }

            #endregion

            #region Location

            else if (datItem is Location location)
            {
                if (datItemFields.Contains(DatItemField.Location_Name))
                    location.Name = null;

                if (datItemFields.Contains(DatItemField.Location_Number))
                    location.Number = null;

                if (datItemFields.Contains(DatItemField.Location_Inverted))
                    location.Inverted = null;
            }

            #endregion

            #region Media

            else if (datItem is Media media)
            {
                if (datItemFields.Contains(DatItemField.Name))
                    media.Name = null;

                if (datItemFields.Contains(DatItemField.MD5))
                    media.MD5 = null;

                if (datItemFields.Contains(DatItemField.SHA1))
                    media.SHA1 = null;

                if (datItemFields.Contains(DatItemField.SHA256))
                    media.SHA256 = null;

                if (datItemFields.Contains(DatItemField.SpamSum))
                    media.SpamSum = null;
            }

            #endregion

            #region Part

            else if (datItem is Part part)
            {
                if (datItemFields.Contains(DatItemField.Part_Name))
                    part.Name = null;

                if (datItemFields.Contains(DatItemField.Part_Interface))
                    part.Interface = null;

                if (part.FeaturesSpecified)
                {
                    foreach (PartFeature subPartFeature in part.Features)
                    {
                        RemoveFields(subPartFeature, datItemFields, machineFields);
                    }
                }
            }

            #endregion

            #region PartFeature

            else if (datItem is PartFeature partFeature)
            {
                if (datItemFields.Contains(DatItemField.Part_Feature_Name))
                    partFeature.Name = null;

                if (datItemFields.Contains(DatItemField.Part_Feature_Value))
                    partFeature.Value = null;
            }

            #endregion

            #region Port

            else if (datItem is Port port)
            {
                if (datItemFields.Contains(DatItemField.Tag))
                    port.Tag = null;

                if (port.AnalogsSpecified)
                {
                    foreach (Analog subAnalog in port.Analogs)
                    {
                        RemoveFields(subAnalog, datItemFields, machineFields);
                    }
                }
            }

            #endregion

            #region RamOption

            else if (datItem is RamOption ramOption)
            {
                if (datItemFields.Contains(DatItemField.Name))
                    ramOption.Name = null;

                if (datItemFields.Contains(DatItemField.Default))
                    ramOption.Default = null;

                if (datItemFields.Contains(DatItemField.Content))
                    ramOption.Content = null;
            }

            #endregion

            #region Release

            else if (datItem is Release release)
            {
                if (datItemFields.Contains(DatItemField.Name))
                    release.Name = null;

                if (datItemFields.Contains(DatItemField.Region))
                    release.Region = null;

                if (datItemFields.Contains(DatItemField.Language))
                    release.Language = null;

                if (datItemFields.Contains(DatItemField.Date))
                    release.Date = null;

                if (datItemFields.Contains(DatItemField.Default))
                    release.Default = null;
            }

            #endregion

            #region Rom

            else if (datItem is Rom rom)
            {
                #region Common

                if (datItemFields.Contains(DatItemField.Name))
                    rom.Name = null;

                if (datItemFields.Contains(DatItemField.Bios))
                    rom.Bios = null;

                if (datItemFields.Contains(DatItemField.Size))
                    rom.Size = 0;

                if (datItemFields.Contains(DatItemField.CRC))
                    rom.CRC = null;

                if (datItemFields.Contains(DatItemField.MD5))
                    rom.MD5 = null;

    #if NET_FRAMEWORK
                if (datItemFields.Contains(DatItemField.RIPEMD160))
                    rom.RIPEMD160 = null;
    #endif

                if (datItemFields.Contains(DatItemField.SHA1))
                    rom.SHA1 = null;

                if (datItemFields.Contains(DatItemField.SHA256))
                    rom.SHA256 = null;

                if (datItemFields.Contains(DatItemField.SHA384))
                    rom.SHA384 = null;

                if (datItemFields.Contains(DatItemField.SHA512))
                    rom.SHA512 = null;

                if (datItemFields.Contains(DatItemField.SpamSum))
                    rom.SpamSum = null;

                if (datItemFields.Contains(DatItemField.Merge))
                    rom.MergeTag = null;

                if (datItemFields.Contains(DatItemField.Region))
                    rom.Region = null;

                if (datItemFields.Contains(DatItemField.Offset))
                    rom.Offset = null;

                if (datItemFields.Contains(DatItemField.Date))
                    rom.Date = null;

                if (datItemFields.Contains(DatItemField.Status))
                    rom.ItemStatus = ItemStatus.NULL;

                if (datItemFields.Contains(DatItemField.Optional))
                    rom.Optional = null;

                if (datItemFields.Contains(DatItemField.Inverted))
                    rom.Inverted = null;

                #endregion

                #region AttractMode

                if (datItemFields.Contains(DatItemField.AltName))
                    rom.AltName = null;

                if (datItemFields.Contains(DatItemField.AltTitle))
                    rom.AltTitle = null;

                #endregion

                #region OpenMSX

                if (datItemFields.Contains(DatItemField.Original))
                    rom.Original = null;

                if (datItemFields.Contains(DatItemField.OpenMSXSubType))
                    rom.OpenMSXSubType = OpenMSXSubType.NULL;

                if (datItemFields.Contains(DatItemField.OpenMSXType))
                    rom.OpenMSXType = null;

                if (datItemFields.Contains(DatItemField.Remark))
                    rom.Remark = null;

                if (datItemFields.Contains(DatItemField.Boot))
                    rom.Boot = null;

                #endregion

                #region SoftwareList

                if (datItemFields.Contains(DatItemField.LoadFlag))
                    rom.LoadFlag = LoadFlag.NULL;

                if (datItemFields.Contains(DatItemField.Value))
                    rom.Value = null;

                if (rom.DataAreaSpecified)
                    RemoveFields(rom.DataArea, datItemFields, machineFields);

                if (rom.PartSpecified)
                    RemoveFields(rom.Part, datItemFields, machineFields);

                #endregion
            }

            #endregion

            #region Sample

            else if (datItem is Sample sample)
            {
                if (datItemFields.Contains(DatItemField.Name))
                    sample.Name = null;
            }

            #endregion

            #region Setting

            else if (datItem is Setting setting)
            {
                if (datItemFields.Contains(DatItemField.Setting_Name))
                    setting.Name = null;

                if (datItemFields.Contains(DatItemField.Setting_Value))
                    setting.Value = null;

                if (datItemFields.Contains(DatItemField.Setting_Default))
                    setting.Default = null;

                if (setting.ConditionsSpecified)
                {
                    foreach (Condition subCondition in setting.Conditions)
                    {
                        RemoveFields(subCondition, datItemFields, machineFields, true);
                    }
                }
            }

            #endregion

            #region SharedFeature

            else if (datItem is SharedFeature sharedFeature)
            {
                if (datItemFields.Contains(DatItemField.Name))
                    sharedFeature.Name = null;

                if (datItemFields.Contains(DatItemField.Value))
                    sharedFeature.Value = null;
            }

            #endregion

            #region Slot

            else if (datItem is Slot slot)
            {
                if (datItemFields.Contains(DatItemField.Name))
                    slot.Name = null;

                if (slot.SlotOptionsSpecified)
                {
                    foreach (SlotOption subSlotOption in slot.SlotOptions)
                    {
                        RemoveFields(subSlotOption, datItemFields, machineFields);
                    }
                }
            }

            #endregion

            #region SlotOption

            else if (datItem is SlotOption slotOption)
            {
                if (datItemFields.Contains(DatItemField.SlotOption_Name))
                    slotOption.Name = null;

                if (datItemFields.Contains(DatItemField.SlotOption_DeviceName))
                    slotOption.DeviceName = null;

                if (datItemFields.Contains(DatItemField.SlotOption_Default))
                    slotOption.Default = null;
            }

            #endregion

            #region SoftwareList

            else if (datItem is SoftwareList softwareList)
            {
                if (datItemFields.Contains(DatItemField.Name))
                    softwareList.Name = null;

                if (datItemFields.Contains(DatItemField.SoftwareListStatus))
                    softwareList.Status = SoftwareListStatus.NULL;

                if (datItemFields.Contains(DatItemField.Filter))
                    softwareList.Filter = null;
            }

            #endregion

            #region Sound

            else if (datItem is Sound sound)
            {
                if (datItemFields.Contains(DatItemField.Channels))
                    sound.Channels = null;
            }

            #endregion
        }

        /// <summary>
        /// Remove fields with given values
        /// </summary>
        /// <param name="machine">Machine to remove fields from</param>
        /// <param name="fields">List of fields to remove</param>
        public static void RemoveFields(Machine machine, List<MachineField> fields)
        {
            #region Common

            if (fields.Contains(MachineField.Name))
                machine.Name = null;

            if (fields.Contains(MachineField.Comment))
                machine.Comment = null;

            if (fields.Contains(MachineField.Description))
                machine.Description = null;

            if (fields.Contains(MachineField.Year))
                machine.Year = null;

            if (fields.Contains(MachineField.Manufacturer))
                machine.Manufacturer = null;

            if (fields.Contains(MachineField.Publisher))
                machine.Publisher = null;

            if (fields.Contains(MachineField.Category))
                machine.Category = null;

            if (fields.Contains(MachineField.RomOf))
                machine.RomOf = null;

            if (fields.Contains(MachineField.CloneOf))
                machine.CloneOf = null;

            if (fields.Contains(MachineField.SampleOf))
                machine.SampleOf = null;

            if (fields.Contains(MachineField.Type))
                machine.MachineType = 0x0;

            #endregion

            #region AttractMode

            if (fields.Contains(MachineField.Players))
                machine.Players = null;

            if (fields.Contains(MachineField.Rotation))
                machine.Rotation = null;

            if (fields.Contains(MachineField.Control))
                machine.Control = null;

            if (fields.Contains(MachineField.Status))
                machine.Status = null;

            if (fields.Contains(MachineField.DisplayCount))
                machine.DisplayCount = null;

            if (fields.Contains(MachineField.DisplayType))
                machine.DisplayType = null;

            if (fields.Contains(MachineField.Buttons))
                machine.Buttons = null;

            #endregion

            #region ListXML

            if (fields.Contains(MachineField.SourceFile))
                machine.SourceFile = null;

            if (fields.Contains(MachineField.Runnable))
                machine.Runnable = Runnable.NULL;

            #endregion

            #region Logiqx

            if (fields.Contains(MachineField.Board))
                machine.Board = null;

            if (fields.Contains(MachineField.RebuildTo))
                machine.RebuildTo = null;

            #endregion

            #region Logiqx EmuArc

            if (fields.Contains(MachineField.TitleID))
                machine.TitleID = null;

            if (fields.Contains(MachineField.Developer))
                machine.Developer = null;

            if (fields.Contains(MachineField.Genre))
                machine.Genre = null;

            if (fields.Contains(MachineField.Subgenre))
                machine.Subgenre = null;

            if (fields.Contains(MachineField.Ratings))
                machine.Ratings = null;

            if (fields.Contains(MachineField.Score))
                machine.Score = null;

            if (fields.Contains(MachineField.Enabled))
                machine.Enabled = null;

            if (fields.Contains(MachineField.CRC))
                machine.Crc = null;

            if (fields.Contains(MachineField.RelatedTo))
                machine.RelatedTo = null;

            #endregion

            #region OpenMSX

            if (fields.Contains(MachineField.GenMSXID))
                machine.GenMSXID = null;

            if (fields.Contains(MachineField.System))
                machine.System = null;

            if (fields.Contains(MachineField.Country))
                machine.Country = null;

            #endregion

            #region SoftwareList

            if (fields.Contains(MachineField.Supported))
                machine.Supported = Supported.NULL;

            #endregion
        }

        /// <summary>
        /// Replace fields with given values
        /// </summary>
        /// <param name="datItem">DatItem to replace fields in</param>
        /// <param name="repMachine">DatItem to pull new information from</param>
        /// <param name="fields">List of fields representing what should be updated</param>
        public static void ReplaceFields(DatItem datItem, DatItem repDatItem, List<DatItemField> fields)
        {

        }

        /// <summary>
        /// Replace fields with given values
        /// </summary>
        /// <param name="machine">Machine to replace fields in</param>
        /// <param name="repMachine">Machine to pull new information from</param>
        /// <param name="fields">List of fields representing what should be updated</param>
        /// <param name="onlySame">True if descriptions should only be replaced if the game name is the same, false otherwise</param>
        public static void ReplaceFields(Machine machine, Machine repMachine, List<MachineField> fields, bool onlySame)
        {
            #region Common

            if (fields.Contains(MachineField.Name))
                machine.Name = repMachine.Name;

            if (fields.Contains(MachineField.Comment))
                machine.Comment = repMachine.Comment;

            if (fields.Contains(MachineField.Description))
            {
                if (!onlySame || (onlySame && machine.Name == machine.Description))
                    machine.Description = repMachine.Description;
            }

            if (fields.Contains(MachineField.Year))
                machine.Year = repMachine.Year;

            if (fields.Contains(MachineField.Manufacturer))
                machine.Manufacturer = repMachine.Manufacturer;

            if (fields.Contains(MachineField.Publisher))
                machine.Publisher = repMachine.Publisher;

            if (fields.Contains(MachineField.Category))
                machine.Category = repMachine.Category;

            if (fields.Contains(MachineField.RomOf))
                machine.RomOf = repMachine.RomOf;

            if (fields.Contains(MachineField.CloneOf))
                machine.CloneOf = repMachine.CloneOf;

            if (fields.Contains(MachineField.SampleOf))
                machine.SampleOf = repMachine.SampleOf;

            if (fields.Contains(MachineField.Type))
                machine.MachineType = repMachine.MachineType;

            #endregion

            #region AttractMode

            if (fields.Contains(MachineField.Players))
                machine.Players = repMachine.Players;

            if (fields.Contains(MachineField.Rotation))
                machine.Rotation = repMachine.Rotation;

            if (fields.Contains(MachineField.Control))
                machine.Control = repMachine.Control;

            if (fields.Contains(MachineField.Status))
                machine.Status = repMachine.Status;

            if (fields.Contains(MachineField.DisplayCount))
                machine.DisplayCount = repMachine.DisplayCount;

            if (fields.Contains(MachineField.DisplayType))
                machine.DisplayType = repMachine.DisplayType;

            if (fields.Contains(MachineField.Buttons))
                machine.Buttons = repMachine.Buttons;

            #endregion

            #region ListXML

            if (fields.Contains(MachineField.SourceFile))
                machine.SourceFile = repMachine.SourceFile;

            if (fields.Contains(MachineField.Runnable))
                machine.Runnable = repMachine.Runnable;

            #endregion

            #region Logiqx

            if (fields.Contains(MachineField.Board))
                machine.Board = repMachine.Board;

            if (fields.Contains(MachineField.RebuildTo))
                machine.RebuildTo = repMachine.RebuildTo;

            #endregion

            #region Logiqx EmuArc

            if (fields.Contains(MachineField.TitleID))
                machine.TitleID = repMachine.TitleID;

            if (fields.Contains(MachineField.Developer))
                machine.Developer = repMachine.Developer;

            if (fields.Contains(MachineField.Genre))
                machine.Genre = repMachine.Genre;

            if (fields.Contains(MachineField.Subgenre))
                machine.Subgenre = repMachine.Subgenre;

            if (fields.Contains(MachineField.Ratings))
                machine.Ratings = repMachine.Ratings;

            if (fields.Contains(MachineField.Score))
                machine.Score = repMachine.Score;

            if (fields.Contains(MachineField.Enabled))
                machine.Enabled = repMachine.Enabled;

            if (fields.Contains(MachineField.CRC))
                machine.Crc = repMachine.Crc;

            if (fields.Contains(MachineField.RelatedTo))
                machine.RelatedTo = repMachine.RelatedTo;

            #endregion

            #region OpenMSX

            if (fields.Contains(MachineField.GenMSXID))
                machine.GenMSXID = repMachine.GenMSXID;

            if (fields.Contains(MachineField.System))
                machine.System = repMachine.System;

            if (fields.Contains(MachineField.Country))
                machine.Country = repMachine.Country;

            #endregion

            #region SoftwareList

            if (fields.Contains(MachineField.Supported))
                machine.Supported = repMachine.Supported;

            #endregion
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="datItem">DatItem to set fields on</param>
        /// <param name="mappings">Mappings dictionary</param>
        public static void SetFields(DatItem datItem, Dictionary<DatItemField, string> mappings)
        {

        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="machine">Machine to set fields on</param>
        /// <param name="mappings">Mappings dictionary</param>
        public static void SetFields(Machine machine, Dictionary<MachineField, string> mappings)
        {
            if (machine == null || mappings == null)
                return;

            #region Common

            if (mappings.Keys.Contains(MachineField.Name))
                machine.Name = mappings[MachineField.Name];

            if (mappings.Keys.Contains(MachineField.Comment))
                machine.Comment = mappings[MachineField.Comment];

            if (mappings.Keys.Contains(MachineField.Description))
                machine.Description = mappings[MachineField.Description];

            if (mappings.Keys.Contains(MachineField.Year))
                machine.Year = mappings[MachineField.Year];

            if (mappings.Keys.Contains(MachineField.Manufacturer))
                machine.Manufacturer = mappings[MachineField.Manufacturer];

            if (mappings.Keys.Contains(MachineField.Publisher))
                machine.Publisher = mappings[MachineField.Publisher];

            if (mappings.Keys.Contains(MachineField.Category))
                machine.Category = mappings[MachineField.Category];

            if (mappings.Keys.Contains(MachineField.RomOf))
                machine.RomOf = mappings[MachineField.RomOf];

            if (mappings.Keys.Contains(MachineField.CloneOf))
                machine.CloneOf = mappings[MachineField.CloneOf];

            if (mappings.Keys.Contains(MachineField.SampleOf))
                machine.SampleOf = mappings[MachineField.SampleOf];

            if (mappings.Keys.Contains(MachineField.Type))
                machine.MachineType = mappings[MachineField.Type].AsMachineType();

            #endregion

            #region AttractMode

            if (mappings.Keys.Contains(MachineField.Players))
                machine.Players = mappings[MachineField.Players];

            if (mappings.Keys.Contains(MachineField.Rotation))
                machine.Rotation = mappings[MachineField.Rotation];

            if (mappings.Keys.Contains(MachineField.Control))
                machine.Control = mappings[MachineField.Control];

            if (mappings.Keys.Contains(MachineField.Status))
                machine.Status = mappings[MachineField.Status];

            if (mappings.Keys.Contains(MachineField.DisplayCount))
                machine.DisplayCount = mappings[MachineField.DisplayCount];

            if (mappings.Keys.Contains(MachineField.DisplayType))
                machine.DisplayType = mappings[MachineField.DisplayType];

            if (mappings.Keys.Contains(MachineField.Buttons))
                machine.Buttons = mappings[MachineField.Buttons];

            #endregion

            #region ListXML

            if (mappings.Keys.Contains(MachineField.SourceFile))
                machine.SourceFile = mappings[MachineField.SourceFile];

            if (mappings.Keys.Contains(MachineField.Runnable))
                machine.Runnable = mappings[MachineField.Runnable].AsRunnable();

            #endregion

            #region Logiqx

            if (mappings.Keys.Contains(MachineField.Board))
                machine.Board = mappings[MachineField.Board];

            if (mappings.Keys.Contains(MachineField.RebuildTo))
                machine.RebuildTo = mappings[MachineField.RebuildTo];

            #endregion

            #region Logiqx EmuArc

            if (mappings.Keys.Contains(MachineField.TitleID))
                machine.TitleID = mappings[MachineField.TitleID];

            if (mappings.Keys.Contains(MachineField.Developer))
                machine.Developer = mappings[MachineField.Developer];

            if (mappings.Keys.Contains(MachineField.Genre))
                machine.Genre = mappings[MachineField.Genre];

            if (mappings.Keys.Contains(MachineField.Subgenre))
                machine.Subgenre = mappings[MachineField.Subgenre];

            if (mappings.Keys.Contains(MachineField.Ratings))
                machine.Ratings = mappings[MachineField.Ratings];

            if (mappings.Keys.Contains(MachineField.Score))
                machine.Score = mappings[MachineField.Score];

            if (mappings.Keys.Contains(MachineField.Enabled))
                machine.Enabled = mappings[MachineField.Enabled];

            if (mappings.Keys.Contains(MachineField.CRC))
                machine.Crc = mappings[MachineField.CRC].AsYesNo();

            if (mappings.Keys.Contains(MachineField.RelatedTo))
                machine.RelatedTo = mappings[MachineField.RelatedTo];

            #endregion

            #region OpenMSX

            if (mappings.Keys.Contains(MachineField.GenMSXID))
                machine.GenMSXID = mappings[MachineField.GenMSXID];

            if (mappings.Keys.Contains(MachineField.System))
                machine.System = mappings[MachineField.System];

            if (mappings.Keys.Contains(MachineField.Country))
                machine.Country = mappings[MachineField.Country];

            #endregion

            #region SoftwareList

            if (mappings.Keys.Contains(MachineField.Supported))
                machine.Supported = mappings[MachineField.Supported].AsSupported();

            #endregion
        }
    
        /// <summary>
        /// Set internal names to match One Rom Per Game (ORPG) logic
        /// </summary>
        /// <param name="datItem">DatItem to run logic on</param>
        public static void SetOneRomPerGame(DatItem datItem)
        {
            if (datItem.GetName() == null)
                return;

            string[] splitname = datItem.GetName().Split('.');
            datItem.Machine.Name += $"/{string.Join(".", splitname.Take(splitname.Length > 1 ? splitname.Length - 1 : 1))}";
            datItem.SetName(Path.GetFileName(datItem.GetName()));
        }   
    }
}