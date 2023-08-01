using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.IO;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents writing a MAME XML DAT
    /// </summary>
    internal partial class Listxml : DatFile
    {
        /// <inheritdoc/>
        protected override ItemType[] GetSupportedTypes()
        {
            return new ItemType[]
            {
                ItemType.Adjuster,
                ItemType.BiosSet,
                ItemType.Chip,
                ItemType.Condition,
                ItemType.Configuration,
                ItemType.Device,
                ItemType.DeviceReference,
                ItemType.DipSwitch,
                ItemType.Disk,
                ItemType.Display,
                ItemType.Driver,
                ItemType.Feature,
                ItemType.Input,
                ItemType.Port,
                ItemType.RamOption,
                ItemType.Rom,
                ItemType.Sample,
                ItemType.Slot,
                ItemType.SoftwareList,
                ItemType.Sound,
            };
        }

        /// <inheritdoc/>
        protected override List<DatItemField> GetMissingRequiredFields(DatItem datItem)
        {
            var missingFields = new List<DatItemField>();
            switch (datItem)
            {
                case BiosSet biosset:
                    if (string.IsNullOrWhiteSpace(biosset.Name))
                        missingFields.Add(DatItemField.Name);
                    if (string.IsNullOrWhiteSpace(biosset.Description))
                        missingFields.Add(DatItemField.Description);
                    break;

                case Rom rom:
                    if (string.IsNullOrWhiteSpace(rom.Name))
                        missingFields.Add(DatItemField.Name);
                    if (rom.Size == null || rom.Size < 0)
                        missingFields.Add(DatItemField.Size);
                    if (string.IsNullOrWhiteSpace(rom.CRC)
                        && string.IsNullOrWhiteSpace(rom.SHA1))
                    {
                        missingFields.Add(DatItemField.SHA1);
                    }
                    break;

                case Disk disk:
                    if (string.IsNullOrWhiteSpace(disk.Name))
                        missingFields.Add(DatItemField.Name);
                    if (string.IsNullOrWhiteSpace(disk.MD5)
                        && string.IsNullOrWhiteSpace(disk.SHA1))
                    {
                        missingFields.Add(DatItemField.SHA1);
                    }
                    break;

                case DeviceReference deviceref:
                    if (string.IsNullOrWhiteSpace(deviceref.Name))
                        missingFields.Add(DatItemField.Name);
                    break;

                case Sample sample:
                    if (string.IsNullOrWhiteSpace(sample.Name))
                        missingFields.Add(DatItemField.Name);
                    break;

                case Chip chip:
                    if (string.IsNullOrWhiteSpace(chip.Name))
                        missingFields.Add(DatItemField.Name);
                    if (!chip.ChipTypeSpecified)
                        missingFields.Add(DatItemField.ChipType);
                    break;

                case Display display:
                    if (!display.DisplayTypeSpecified)
                        missingFields.Add(DatItemField.DisplayType);
                    if (display.Refresh == null)
                        missingFields.Add(DatItemField.Refresh);
                    break;

                case Sound sound:
                    if (sound.Channels == null)
                        missingFields.Add(DatItemField.Channels);
                    break;

                case Input input:
                    if (input.Players == null)
                        missingFields.Add(DatItemField.Players);
                    break;

                case DipSwitch dipswitch:
                    if (string.IsNullOrWhiteSpace(dipswitch.Name))
                        missingFields.Add(DatItemField.Name);
                    if (string.IsNullOrWhiteSpace(dipswitch.Tag))
                        missingFields.Add(DatItemField.Tag);
                    break;

                case Configuration configuration:
                    if (string.IsNullOrWhiteSpace(configuration.Name))
                        missingFields.Add(DatItemField.Name);
                    if (string.IsNullOrWhiteSpace(configuration.Tag))
                        missingFields.Add(DatItemField.Tag);
                    break;

                case Port port:
                    if (string.IsNullOrWhiteSpace(port.Tag))
                        missingFields.Add(DatItemField.Tag);
                    break;

                case Adjuster adjuster:
                    if (string.IsNullOrWhiteSpace(adjuster.Name))
                        missingFields.Add(DatItemField.Name);
                    break;

                case Driver driver:
                    if (!driver.StatusSpecified)
                        missingFields.Add(DatItemField.SupportStatus);
                    if (!driver.EmulationSpecified)
                        missingFields.Add(DatItemField.EmulationStatus);
                    if (!driver.CocktailSpecified)
                        missingFields.Add(DatItemField.CocktailStatus);
                    if (!driver.SaveStateSpecified)
                        missingFields.Add(DatItemField.SaveStateStatus);
                    break;

                case Feature feature:
                    if (!feature.TypeSpecified)
                        missingFields.Add(DatItemField.FeatureType);
                    break;

                case Device device:
                    if (!device.DeviceTypeSpecified)
                        missingFields.Add(DatItemField.DeviceType);
                    break;

                case Slot slot:
                    if (string.IsNullOrWhiteSpace(slot.Name))
                        missingFields.Add(DatItemField.Name);
                    break;

                case DatItems.Formats.SoftwareList softwarelist:
                    if (string.IsNullOrWhiteSpace(softwarelist.Tag))
                        missingFields.Add(DatItemField.Tag);
                    if (string.IsNullOrWhiteSpace(softwarelist.Name))
                        missingFields.Add(DatItemField.Name);
                    if (!softwarelist.StatusSpecified)
                        missingFields.Add(DatItemField.SoftwareListStatus);
                    break;

                case RamOption ramoption:
                    if (string.IsNullOrWhiteSpace(ramoption.Name))
                        missingFields.Add(DatItemField.Name);
                    break;
            }

            return missingFields;
        }

        /// <inheritdoc/>
        public override bool WriteToFile(string outfile, bool ignoreblanks = false, bool throwOnError = false)
        {
            try
            {
                logger.User($"Writing to '{outfile}'...");

                var mame = CreateMame(ignoreblanks);
                if (!Serialization.Listxml.SerializeToFile(mame, outfile))
                {
                    logger.Warning($"File '{outfile}' could not be written! See the log for more details.");
                    return false;
                }
            }
            catch (Exception ex) when (!throwOnError)
            {
                logger.Error(ex);
                return false;
            }

            logger.User($"'{outfile}' written!{Environment.NewLine}");
            return true;
        }

        /// <summary>
        /// Write out DAT header using the supplied StreamWriter
        /// </summary>
        /// <param name="xtw">XmlTextWriter to output to</param>
        private void WriteHeader(XmlTextWriter xtw)
        {
            xtw.WriteStartDocument();

            xtw.WriteStartElement("mame");
            xtw.WriteRequiredAttributeString("build", Header.Name);
            xtw.WriteOptionalAttributeString("debug", Header.Debug.FromYesNo());
            xtw.WriteOptionalAttributeString("mameconfig", Header.MameConfig);

            xtw.Flush();
        }

        /// <summary>
        /// Write out Game start using the supplied StreamWriter
        /// </summary>
        /// <param name="xtw">XmlTextWriter to output to</param>
        /// <param name="datItem">DatItem object to be output</param>
        /// <returns>True if the data was written, false on error</returns>
        private void WriteStartGame(XmlTextWriter xtw, DatItem datItem)
        {
            // No game should start with a path separator
            datItem.Machine.Name = datItem.Machine.Name.TrimStart(Path.DirectorySeparatorChar);

            // Build the state
            xtw.WriteStartElement("machine");
            xtw.WriteRequiredAttributeString("name", datItem.Machine.Name);
            xtw.WriteOptionalAttributeString("sourcefile", datItem.Machine.SourceFile);

            if (datItem.Machine.MachineType.HasFlag(MachineType.Bios))
                xtw.WriteAttributeString("isbios", "yes");
            if (datItem.Machine.MachineType.HasFlag(MachineType.Device))
                xtw.WriteAttributeString("isdevice", "yes");
            if (datItem.Machine.MachineType.HasFlag(MachineType.Mechanical))
                xtw.WriteAttributeString("ismechanical", "yes");

            xtw.WriteOptionalAttributeString("runnable", datItem.Machine.Runnable.FromRunnable());

            if (!string.Equals(datItem.Machine.Name, datItem.Machine.CloneOf, StringComparison.OrdinalIgnoreCase))
                xtw.WriteOptionalAttributeString("cloneof", datItem.Machine.CloneOf);
            if (!string.Equals(datItem.Machine.Name, datItem.Machine.RomOf, StringComparison.OrdinalIgnoreCase))
                xtw.WriteOptionalAttributeString("romof", datItem.Machine.RomOf);
            if (!string.Equals(datItem.Machine.Name, datItem.Machine.SampleOf, StringComparison.OrdinalIgnoreCase))
                xtw.WriteOptionalAttributeString("sampleof", datItem.Machine.SampleOf);

            xtw.WriteOptionalElementString("description", datItem.Machine.Description);
            xtw.WriteOptionalElementString("year", datItem.Machine.Year);
            xtw.WriteOptionalElementString("manufacturer", datItem.Machine.Manufacturer);
            xtw.WriteOptionalElementString("history", datItem.Machine.History);

            xtw.Flush();
        }

        /// <summary>
        /// Write out Game start using the supplied StreamWriter
        /// </summary>
        /// <param name="xtw">XmlTextWriter to output to</param>
        private void WriteEndGame(XmlTextWriter xtw)
        {
            // End machine
            xtw.WriteEndElement();

            xtw.Flush();
        }

        /// <summary>
        /// Write out DatItem using the supplied StreamWriter
        /// </summary>
        /// <param name="xtw">XmlTextWriter to output to</param>
        /// <param name="datItem">DatItem object to be output</param>
        private void WriteDatItem(XmlTextWriter xtw, DatItem datItem)
        {
            // Pre-process the item name
            ProcessItemName(datItem, true);

            // Build the state
            switch (datItem.ItemType)
            {
                case ItemType.Adjuster:
                    var adjuster = datItem as Adjuster;
                    xtw.WriteStartElement("adjuster");
                    xtw.WriteRequiredAttributeString("name", adjuster.Name);
                    xtw.WriteRequiredAttributeString("default", adjuster.Default.FromYesNo());
                    if (adjuster.ConditionsSpecified)
                    {
                        foreach (var adjusterCondition in adjuster.Conditions)
                        {
                            xtw.WriteStartElement("condition");
                            xtw.WriteRequiredAttributeString("tag", adjusterCondition.Tag);
                            xtw.WriteRequiredAttributeString("mask", adjusterCondition.Mask);
                            xtw.WriteRequiredAttributeString("relation", adjusterCondition.Relation.FromRelation());
                            xtw.WriteRequiredAttributeString("value", adjusterCondition.Value);
                            xtw.WriteEndElement();
                        }
                    }
                    xtw.WriteEndElement();
                    break;

                case ItemType.BiosSet:
                    var biosSet = datItem as BiosSet;
                    xtw.WriteStartElement("biosset");
                    xtw.WriteRequiredAttributeString("name", biosSet.Name);
                    xtw.WriteRequiredAttributeString("description", biosSet.Description);
                    xtw.WriteOptionalAttributeString("default", biosSet.Default?.ToString().ToLowerInvariant());
                    xtw.WriteEndElement();
                    break;

                case ItemType.Chip:
                    var chip = datItem as Chip;
                    xtw.WriteStartElement("chip");
                    xtw.WriteRequiredAttributeString("name", chip.Name);
                    xtw.WriteOptionalAttributeString("tag", chip.Tag);
                    xtw.WriteRequiredAttributeString("type", chip.ChipType.FromChipType());
                    xtw.WriteOptionalAttributeString("clock", chip.Clock?.ToString());
                    xtw.WriteEndElement();
                    break;

                case ItemType.Condition:
                    var condition = datItem as Condition;
                    xtw.WriteStartElement("condition");
                    xtw.WriteRequiredAttributeString("tag", condition.Tag);
                    xtw.WriteRequiredAttributeString("mask", condition.Mask);
                    xtw.WriteRequiredAttributeString("relation", condition.Relation.FromRelation());
                    xtw.WriteRequiredAttributeString("value", condition.Value);
                    xtw.WriteEndElement();
                    break;

                case ItemType.Configuration:
                    var configuration = datItem as Configuration;
                    xtw.WriteStartElement("configuration");
                    xtw.WriteRequiredAttributeString("name", configuration.Name);
                    xtw.WriteRequiredAttributeString("tag", configuration.Tag);
                    xtw.WriteRequiredAttributeString("mask", configuration.Mask);

                    if (configuration.ConditionsSpecified)
                    {
                        foreach (var configurationCondition in configuration.Conditions)
                        {
                            xtw.WriteStartElement("condition");
                            xtw.WriteRequiredAttributeString("tag", configurationCondition.Tag);
                            xtw.WriteRequiredAttributeString("mask", configurationCondition.Mask);
                            xtw.WriteRequiredAttributeString("relation", configurationCondition.Relation.FromRelation());
                            xtw.WriteRequiredAttributeString("value", configurationCondition.Value);
                            xtw.WriteEndElement();
                        }
                    }
                    if (configuration.LocationsSpecified)
                    {
                        foreach (var location in configuration.Locations)
                        {
                            xtw.WriteStartElement("conflocation");
                            xtw.WriteRequiredAttributeString("name", location.Name);
                            xtw.WriteRequiredAttributeString("number", location.Number?.ToString());
                            xtw.WriteOptionalAttributeString("inverted", location.Inverted.FromYesNo());
                            xtw.WriteEndElement();
                        }
                    }
                    if (configuration.SettingsSpecified)
                    {
                        foreach (var setting in configuration.Settings)
                        {
                            xtw.WriteStartElement("confsetting");
                            xtw.WriteRequiredAttributeString("name", setting.Name);
                            xtw.WriteRequiredAttributeString("value", setting.Value);
                            xtw.WriteOptionalAttributeString("default", setting.Default.FromYesNo());
                            if (setting.ConditionsSpecified)
                            {
                                foreach (var confsettingCondition in setting.Conditions)
                                {
                                    xtw.WriteStartElement("condition");
                                    xtw.WriteRequiredAttributeString("tag", confsettingCondition.Tag);
                                    xtw.WriteRequiredAttributeString("mask", confsettingCondition.Mask);
                                    xtw.WriteRequiredAttributeString("relation", confsettingCondition.Relation.FromRelation());
                                    xtw.WriteRequiredAttributeString("value", confsettingCondition.Value);
                                    xtw.WriteEndElement();
                                }
                            }
                            xtw.WriteEndElement();
                        }
                    }
                    xtw.WriteEndElement();
                    break;

                case ItemType.Device:
                    var device = datItem as Device;
                    xtw.WriteStartElement("device");
                    xtw.WriteRequiredAttributeString("type", device.DeviceType.FromDeviceType());
                    xtw.WriteOptionalAttributeString("tag", device.Tag);
                    xtw.WriteOptionalAttributeString("fixed_image", device.FixedImage);
                    xtw.WriteOptionalAttributeString("mandatory", device.Mandatory?.ToString());
                    xtw.WriteOptionalAttributeString("interface", device.Interface);
                    if (device.InstancesSpecified)
                    {
                        foreach (var instance in device.Instances)
                        {
                            xtw.WriteStartElement("instance");
                            xtw.WriteRequiredAttributeString("name", instance.Name);
                            xtw.WriteRequiredAttributeString("briefname", instance.BriefName);
                            xtw.WriteEndElement();
                        }
                    }
                    if (device.ExtensionsSpecified)
                    {
                        foreach (var extension in device.Extensions)
                        {
                            xtw.WriteStartElement("extension");
                            xtw.WriteRequiredAttributeString("name", extension.Name);
                            xtw.WriteEndElement();
                        }
                    }
                    xtw.WriteEndElement();
                    break;

                case ItemType.DeviceReference:
                    var deviceRef = datItem as DeviceReference;
                    xtw.WriteStartElement("device_ref");
                    xtw.WriteRequiredAttributeString("name", deviceRef.Name);
                    xtw.WriteEndElement();
                    break;

                case ItemType.DipSwitch:
                    var dipSwitch = datItem as DipSwitch;
                    xtw.WriteStartElement("dipswitch");
                    xtw.WriteRequiredAttributeString("name", dipSwitch.Name);
                    xtw.WriteRequiredAttributeString("tag", dipSwitch.Tag);
                    xtw.WriteRequiredAttributeString("mask", dipSwitch.Mask);
                    if (dipSwitch.ConditionsSpecified)
                    {
                        foreach (var dipSwitchCondition in dipSwitch.Conditions)
                        {
                            xtw.WriteStartElement("condition");
                            xtw.WriteRequiredAttributeString("tag", dipSwitchCondition.Tag);
                            xtw.WriteRequiredAttributeString("mask", dipSwitchCondition.Mask);
                            xtw.WriteRequiredAttributeString("relation", dipSwitchCondition.Relation.FromRelation());
                            xtw.WriteRequiredAttributeString("value", dipSwitchCondition.Value);
                            xtw.WriteEndElement();
                        }
                    }
                    if (dipSwitch.LocationsSpecified)
                    {
                        foreach (var location in dipSwitch.Locations)
                        {
                            xtw.WriteStartElement("diplocation");
                            xtw.WriteRequiredAttributeString("name", location.Name);
                            xtw.WriteRequiredAttributeString("number", location.Number?.ToString());
                            xtw.WriteOptionalAttributeString("inverted", location.Inverted.FromYesNo());
                            xtw.WriteEndElement();
                        }
                    }
                    if (dipSwitch.ValuesSpecified)
                    {
                        foreach (var value in dipSwitch.Values)
                        {
                            xtw.WriteStartElement("dipvalue");
                            xtw.WriteRequiredAttributeString("name", value.Name);
                            xtw.WriteRequiredAttributeString("value", value.Value);
                            xtw.WriteOptionalAttributeString("default", value.Default.FromYesNo());
                            if (value.ConditionsSpecified)
                            {
                                foreach (var dipValueCondition in value.Conditions)
                                {
                                    xtw.WriteStartElement("condition");
                                    xtw.WriteRequiredAttributeString("tag", dipValueCondition.Tag);
                                    xtw.WriteRequiredAttributeString("mask", dipValueCondition.Mask);
                                    xtw.WriteRequiredAttributeString("relation", dipValueCondition.Relation.FromRelation());
                                    xtw.WriteRequiredAttributeString("value", dipValueCondition.Value);
                                    xtw.WriteEndElement();
                                }
                            }
                            xtw.WriteEndElement();
                        }
                    }
                    xtw.WriteEndElement();
                    break;

                case ItemType.Disk:
                    var disk = datItem as Disk;
                    xtw.WriteStartElement("disk");
                    xtw.WriteRequiredAttributeString("name", disk.Name);
                    xtw.WriteOptionalAttributeString("sha1", disk.SHA1?.ToLowerInvariant());
                    xtw.WriteOptionalAttributeString("merge", disk.MergeTag);
                    xtw.WriteOptionalAttributeString("region", disk.Region);
                    xtw.WriteOptionalAttributeString("index", disk.Index);
                    xtw.WriteOptionalAttributeString("writable", disk.Writable.FromYesNo());
                    if (disk.ItemStatus != ItemStatus.None)
                        xtw.WriteOptionalAttributeString("status", disk.ItemStatus.FromItemStatus(false));
                    xtw.WriteOptionalAttributeString("optional", disk.Optional.FromYesNo());
                    xtw.WriteEndElement();
                    break;

                case ItemType.Display:
                    var display = datItem as Display;
                    xtw.WriteStartElement("display");
                    xtw.WriteOptionalAttributeString("tag", display.Tag);
                    xtw.WriteRequiredAttributeString("type", display.DisplayType.FromDisplayType());
                    xtw.WriteOptionalAttributeString("rotate", display.Rotate?.ToString());
                    xtw.WriteOptionalAttributeString("flipx", display.FlipX.FromYesNo());
                    xtw.WriteOptionalAttributeString("width", display.Width?.ToString());
                    xtw.WriteOptionalAttributeString("height", display.Height?.ToString());
                    xtw.WriteRequiredAttributeString("refresh", display.Refresh?.ToString("N6"));
                    xtw.WriteOptionalAttributeString("pixclock", display.PixClock?.ToString());
                    xtw.WriteOptionalAttributeString("htotal", display.HTotal?.ToString());
                    xtw.WriteOptionalAttributeString("hbend", display.HBEnd?.ToString());
                    xtw.WriteOptionalAttributeString("hstart", display.HBStart?.ToString());
                    xtw.WriteOptionalAttributeString("vtotal", display.VTotal?.ToString());
                    xtw.WriteOptionalAttributeString("vbend", display.VBEnd?.ToString());
                    xtw.WriteOptionalAttributeString("vbstart", display.VBStart?.ToString());
                    xtw.WriteEndElement();
                    break;

                case ItemType.Driver:
                    var driver = datItem as Driver;
                    xtw.WriteStartElement("driver");
                    xtw.WriteRequiredAttributeString("status", driver.Status.FromSupportStatus());
                    xtw.WriteRequiredAttributeString("emulation", driver.Emulation.FromSupportStatus());
                    xtw.WriteOptionalAttributeString("cocktail", driver.Cocktail.FromSupportStatus());
                    xtw.WriteRequiredAttributeString("savestate", driver.SaveState.FromSupported(true));
                    xtw.WriteOptionalAttributeString("requiresartwork", driver.RequiresArtwork.FromYesNo());
                    xtw.WriteOptionalAttributeString("unofficial", driver.Unofficial.FromYesNo());
                    xtw.WriteOptionalAttributeString("nosoundhardware", driver.NoSoundHardware.FromYesNo());
                    xtw.WriteOptionalAttributeString("incomplete", driver.Incomplete.FromYesNo());
                    xtw.WriteEndElement();
                    break;

                case ItemType.Feature:
                    var feature = datItem as Feature;
                    xtw.WriteStartElement("feature");
                    xtw.WriteRequiredAttributeString("type", feature.Type.FromFeatureType());
                    xtw.WriteOptionalAttributeString("status", feature.Status.FromFeatureStatus());
                    xtw.WriteOptionalAttributeString("overall", feature.Overall.FromFeatureStatus());
                    xtw.WriteEndElement();
                    break;

                case ItemType.Input:
                    var input = datItem as Input;
                    xtw.WriteStartElement("input");
                    xtw.WriteOptionalAttributeString("service", input.Service.FromYesNo());
                    xtw.WriteOptionalAttributeString("tilt", input.Tilt.FromYesNo());
                    xtw.WriteRequiredAttributeString("players", input.Players?.ToString());
                    xtw.WriteOptionalAttributeString("coins", input.Coins?.ToString());
                    if (input.ControlsSpecified)
                    {
                        foreach (var control in input.Controls)
                        {
                            xtw.WriteStartElement("control");
                            xtw.WriteRequiredAttributeString("type", control.ControlType.FromControlType());
                            xtw.WriteOptionalAttributeString("player", control.Player?.ToString());
                            xtw.WriteOptionalAttributeString("buttons", control.Buttons?.ToString());
                            xtw.WriteOptionalAttributeString("reqbuttons", control.RequiredButtons?.ToString());
                            xtw.WriteOptionalAttributeString("minimum", control.Minimum?.ToString());
                            xtw.WriteOptionalAttributeString("maximum", control.Maximum?.ToString());
                            xtw.WriteOptionalAttributeString("sensitivity", control.Sensitivity?.ToString());
                            xtw.WriteOptionalAttributeString("keydelta", control.KeyDelta?.ToString());
                            xtw.WriteOptionalAttributeString("reverse", control.Reverse.FromYesNo());
                            xtw.WriteOptionalAttributeString("ways", control.Ways);
                            xtw.WriteOptionalAttributeString("ways2", control.Ways2);
                            xtw.WriteOptionalAttributeString("ways3", control.Ways3);
                            xtw.WriteEndElement();
                        }
                    }
                    xtw.WriteEndElement();
                    break;

                case ItemType.Port:
                    var port = datItem as Port;
                    xtw.WriteStartElement("port");
                    xtw.WriteRequiredAttributeString("tag", port.Tag);
                    if (port.AnalogsSpecified)
                    {
                        foreach (var analog in port.Analogs)
                        {
                            xtw.WriteStartElement("analog");
                            xtw.WriteRequiredAttributeString("mask", analog.Mask);
                            xtw.WriteEndElement();
                        }
                    }
                    xtw.WriteEndElement();
                    break;

                case ItemType.RamOption:
                    var ramOption = datItem as RamOption;
                    xtw.WriteStartElement("ramoption");
                    xtw.WriteRequiredAttributeString("name", ramOption.Name);
                    xtw.WriteOptionalAttributeString("default", ramOption.Default.FromYesNo());
                    xtw.WriteRaw(ramOption.Content ?? string.Empty);
                    xtw.WriteFullEndElement();
                    break;

                case ItemType.Rom:
                    var rom = datItem as Rom;
                    xtw.WriteStartElement("rom");
                    xtw.WriteRequiredAttributeString("name", rom.Name);
                    xtw.WriteOptionalAttributeString("bios", rom.Bios);
                    xtw.WriteRequiredAttributeString("size", rom.Size?.ToString());
                    xtw.WriteOptionalAttributeString("crc", rom.CRC?.ToLowerInvariant());
                    xtw.WriteOptionalAttributeString("sha1", rom.SHA1?.ToLowerInvariant());
                    xtw.WriteOptionalAttributeString("merge", rom.MergeTag);
                    xtw.WriteOptionalAttributeString("region", rom.Region);
                    xtw.WriteOptionalAttributeString("offset", rom.Offset);
                    if (rom.ItemStatus != ItemStatus.None)
                        xtw.WriteOptionalAttributeString("status", rom.ItemStatus.FromItemStatus(false));
                    xtw.WriteOptionalAttributeString("optional", rom.Optional.FromYesNo());
                    xtw.WriteEndElement();
                    break;

                case ItemType.Sample:
                    var sample = datItem as Sample;
                    xtw.WriteStartElement("sample");
                    xtw.WriteRequiredAttributeString("name", sample.Name);
                    xtw.WriteEndElement();
                    break;

                case ItemType.Slot:
                    var slot = datItem as Slot;
                    xtw.WriteStartElement("slot");
                    xtw.WriteRequiredAttributeString("name", slot.Name);
                    if (slot.SlotOptionsSpecified)
                    {
                        foreach (var slotOption in slot.SlotOptions)
                        {
                            xtw.WriteStartElement("slotoption");
                            xtw.WriteRequiredAttributeString("name", slotOption.Name);
                            xtw.WriteRequiredAttributeString("devname", slotOption.DeviceName);
                            xtw.WriteOptionalAttributeString("default", slotOption.Default.FromYesNo());
                            xtw.WriteEndElement();
                        }
                    }
                    xtw.WriteEndElement();
                    break;

                case ItemType.SoftwareList:
                    var softwareList = datItem as DatItems.Formats.SoftwareList;
                    xtw.WriteStartElement("softwarelist");
                    xtw.WriteRequiredAttributeString("tag", softwareList.Tag);
                    xtw.WriteRequiredAttributeString("name", softwareList.Name);
                    if (softwareList.Status != SoftwareListStatus.None)
                        xtw.WriteRequiredAttributeString("status", softwareList.Status.FromSoftwareListStatus());
                    xtw.WriteOptionalAttributeString("filter", softwareList.Filter);
                    xtw.WriteEndElement();
                    break;

                case ItemType.Sound:
                    var sound = datItem as Sound;
                    xtw.WriteStartElement("sound");
                    xtw.WriteRequiredAttributeString("channels", sound.Channels?.ToString());
                    xtw.WriteEndElement();
                    break;
            }

            xtw.Flush();
        }

        /// <summary>
        /// Write out DAT footer using the supplied StreamWriter
        /// </summary>
        /// <param name="xtw">XmlTextWriter to output to</param>
        private void WriteFooter(XmlTextWriter xtw)
        {
            // End machine
            xtw.WriteEndElement();

            // End mame
            xtw.WriteEndElement();

            xtw.Flush();
        }

        #region Converters

        /// <summary>
        /// Create a Mame from the current internal information
        /// <summary>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise</param>
        private Models.Listxml.Mame CreateMame(bool ignoreblanks)
        {
            var datafile = new Models.Listxml.Mame
            {
                Build = Header.Name ?? Header.Description ?? Header.Build,
                Debug = Header.Debug.FromYesNo(),
                MameConfig = Header.MameConfig,

                Game = CreateGames(ignoreblanks)
            };

            return datafile;
        }

        /// <summary>
        /// Create an array of GameBase from the current internal information
        /// <summary>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise</param>
        private Models.Listxml.GameBase[]? CreateGames(bool ignoreblanks)
        {
            // If we don't have items, we can't do anything
            if (this.Items == null || !this.Items.Any())
                return null;

            // Create a list of hold the games
            var games = new List<Models.Listxml.GameBase>();

            // Loop through the sorted items and create games for them
            foreach (string key in Items.SortedKeys)
            {
                var items = Items.FilteredItems(key);
                if (items == null || !items.Any())
                    continue;

                // Get the first item for game information
                var machine = items[0].Machine;
                var game = CreateGame(machine);

                // Create holders for all item types
                var biosSets = new List<Models.Listxml.BiosSet>();
                var roms = new List<Models.Listxml.Rom>();
                var disks = new List<Models.Listxml.Disk>();
                var deviceRefs = new List<Models.Listxml.DeviceRef>();
                var samples = new List<Models.Listxml.Sample>();
                var chips = new List<Models.Listxml.Chip>();
                var displays = new List<Models.Listxml.Display>();
                var dipSwitches = new List<Models.Listxml.DipSwitch>();
                var configurations = new List<Models.Listxml.Configuration>();
                var ports = new List<Models.Listxml.Port>();
                var adjusters = new List<Models.Listxml.Adjuster>();
                var features = new List<Models.Listxml.Feature>();
                var devices = new List<Models.Listxml.Device>();
                var slots = new List<Models.Listxml.Slot>();
                var softwareLists = new List<Models.Listxml.SoftwareList>();
                var ramOptions = new List<Models.Listxml.RamOption>();

                // Loop through and convert the items to respective lists
                for (int index = 0; index < items.Count; index++)
                {
                    // Get the item
                    var item = items[index];

                    // Check for a "null" item
                    item = ProcessNullifiedItem(item);

                    // Skip if we're ignoring the item
                    if (ShouldIgnore(item, ignoreblanks))
                        continue;

                    switch (item)
                    {
                        case BiosSet biosset:
                            biosSets.Add(CreateBiosSet(biosset));
                            break;
                        case Rom rom:
                            roms.Add(CreateRom(rom));
                            break;
                        case Disk disk:
                            disks.Add(CreateDisk(disk));
                            break;
                        case DeviceReference deviceref:
                            deviceRefs.Add(CreateDeviceRef(deviceref));
                            break;
                        case Sample sample:
                            samples.Add(CreateSample(sample));
                            break;
                        case Chip chip:
                            chips.Add(CreateChip(chip));
                            break;
                        case Display display:
                            displays.Add(CreateDisplay(display));
                            break;
                        case Sound sound:
                            game.Sound = CreateSound(sound);
                            break;
                        case Input input:
                            game.Input = CreateInput(input);
                            break;
                        case DipSwitch dipswitch:
                            dipSwitches.Add(CreateDipSwitch(dipswitch));
                            break;
                        case Configuration configuration:
                            configurations.Add(CreateConfiguration(configuration));
                            break;
                        case Port port:
                            ports.Add(CreatePort(port));
                            break;
                        case Adjuster adjuster:
                            adjusters.Add(CreateAdjuster(adjuster));
                            break;
                        case Driver driver:
                            game.Driver = CreateDriver(driver);
                            break;
                        case Feature feature:
                            features.Add(CreateFeature(feature));
                            break;
                        case Device device:
                            devices.Add(CreateDevice(device));
                            break;
                        case Slot slot:
                            slots.Add(CreateSlot(slot));
                            break;
                        case DatItems.Formats.SoftwareList softwarelist:
                            softwareLists.Add(CreateSoftwareList(softwarelist));
                            break;
                        case RamOption ramoption:
                            ramOptions.Add(CreateRamOption(ramoption));
                            break;
                    }
                }

                // Assign the values to the game
                game.BiosSet = biosSets.ToArray();
                game.Rom = roms.ToArray();
                game.Disk = disks.ToArray();
                game.DeviceRef = deviceRefs.ToArray();
                game.Sample = samples.ToArray();
                game.Chip = chips.ToArray();
                game.Display = displays.ToArray();
                game.Video = null;
                game.DipSwitch = dipSwitches.ToArray();
                game.Configuration = configurations.ToArray();
                game.Port = ports.ToArray();
                game.Adjuster = adjusters.ToArray();
                game.Feature = features.ToArray();
                game.Device = devices.ToArray();
                game.Slot = slots.ToArray();
                game.SoftwareList = softwareLists.ToArray();
                game.RamOption = ramOptions.ToArray();

                // Add the game to the list
                games.Add(game);
            }

            return games.ToArray();
        }

        /// <summary>
        /// Create a GameBase from the current internal information
        /// <summary>
        private Models.Listxml.GameBase? CreateGame(Machine machine)
        {
            var game = new Models.Listxml.Machine
            {
                Name = machine.Name,
                SourceFile = machine.SourceFile,
                Runnable = machine.Runnable.FromRunnable(),
                CloneOf = machine.CloneOf,
                RomOf = machine.RomOf,
                SampleOf = machine.SampleOf,
                Description = machine.Description,
                Year = machine.Year,
                Manufacturer = machine.Manufacturer,
                History = machine.History,
            };

            if (machine.MachineType.HasFlag(MachineType.Bios))
                game.IsBios = "yes";
            if (machine.MachineType.HasFlag(MachineType.Device))
                game.IsDevice = "yes";
            if (machine.MachineType.HasFlag(MachineType.Mechanical))
                game.IsMechanical = "yes";

            return game;
        }

        /// <summary>
        /// Create a BiosSet from the current BiosSet DatItem
        /// <summary>
        private static Models.Listxml.BiosSet CreateBiosSet(BiosSet item)
        {
            var biosset = new Models.Listxml.BiosSet
            {
                Name = item.Name,
                Description = item.Description,
            };

            if (item.DefaultSpecified)
                biosset.Default = item.Default.FromYesNo();

            return biosset;
        }

        /// <summary>
        /// Create a Rom from the current Rom DatItem
        /// <summary>
        private static Models.Listxml.Rom CreateRom(Rom item)
        {
            var rom = new Models.Listxml.Rom
            {
                Name = item.Name,
                Bios = item.Bios,
                Size = item.Size?.ToString(),
                CRC = item.CRC,
                SHA1 = item.SHA1,
                Merge = item.MergeTag,
                Region = item.Region,
                Offset = item.Offset,
                Status = item.ItemStatus.FromItemStatus(yesno: false),
                Optional = item.Optional.FromYesNo(),
                //Dispose = item.Dispose.FromYesNo(), // TODO: Add to internal model
                //SoundOnly = item.SoundOnly.FromYesNo(), // TODO: Add to internal model
            };

            return rom;
        }

        /// <summary>
        /// Create a Disk from the current Disk DatItem
        /// <summary>
        private static Models.Listxml.Disk CreateDisk(Disk item)
        {
            var disk = new Models.Listxml.Disk
            {
                Name = item.Name,
                MD5 = item.MD5,
                SHA1 = item.SHA1,
                Merge = item.MergeTag,
                Region = item.Region,
                Index = item.Index,
                Writable = item.Writable.FromYesNo(),
                Status = item.ItemStatus.FromItemStatus(yesno: false),
                Optional = item.Optional.FromYesNo(),
            };

            return disk;
        }

        /// <summary>
        /// Create a DeviceRef from the current DeviceReference DatItem
        /// <summary>
        private static Models.Listxml.DeviceRef CreateDeviceRef(DeviceReference item)
        {
            var deviceref = new Models.Listxml.DeviceRef
            {
                Name = item.Name,
            };

            return deviceref;
        }

        /// <summary>
        /// Create a Sample from the current Sample DatItem
        /// <summary>
        private static Models.Listxml.Sample CreateSample(Sample item)
        {
            var sample = new Models.Listxml.Sample
            {
                Name = item.Name,
            };

            return sample;
        }

        /// <summary>
        /// Create a Chip from the current Chip DatItem
        /// <summary>
        private static Models.Listxml.Chip CreateChip(Chip item)
        {
            var chip = new Models.Listxml.Chip
            {
                Name = item.Name,
                Tag = item.Tag,
                Type = item.ChipType.FromChipType(),
                //SoundOnly = item.SoundOnly, // TODO: Add to internal model
                Clock = item.Clock?.ToString(),
            };

            return chip;
        }

        /// <summary>
        /// Create a Display from the current Display DatItem
        /// <summary>
        private static Models.Listxml.Display CreateDisplay(Display item)
        {
            var display = new Models.Listxml.Display
            {
                Tag = item.Tag,
                Type = item.DisplayType.FromDisplayType(),
                Rotate = item.Rotate?.ToString(),
                FlipX = item.FlipX.FromYesNo(),
                Width = item.Width?.ToString(),
                Height = item.Height?.ToString(),
                Refresh = item.Refresh?.ToString(),
                PixClock = item.PixClock?.ToString(),
                HTotal = item.HTotal?.ToString(),
                HBEnd = item.HBEnd?.ToString(),
                HBStart = item.HBStart?.ToString(),
                VTotal = item.VTotal?.ToString(),
                VBEnd = item.VBEnd?.ToString(),
                VBStart = item.VBStart?.ToString(),
            };

            return display;
        }

        /// <summary>
        /// Create a Sound from the current Sound DatItem
        /// <summary>
        private static Models.Listxml.Sound CreateSound(Sound item)
        {
            var sound = new Models.Listxml.Sound
            {
                Channels = item.Channels?.ToString(),
            };

            return sound;
        }

        /// <summary>
        /// Create an Input from the current Input DatItem
        /// <summary>
        private static Models.Listxml.Input CreateInput(Input item)
        {
            var input = new Models.Listxml.Input
            {
                Service = item.Service.FromYesNo(),
                Tilt = item.Tilt.FromYesNo(),
                Players = item.Players?.ToString(),
                //ControlAttr = item.ControlAttr, // TODO: Add to internal model
                //Buttons = item.Buttons, // TODO: Add to internal model
                Coins = item.Coins?.ToString(),
            };

            var controls = new List<Models.Listxml.Control>();
            foreach (var controlItem in item.Controls ?? new List<Control>())
            {
                var control = CreateControl(controlItem);
                controls.Add(control);
            }

            if (controls.Any())
                input.Control = controls.ToArray();

            return input;
        }

        /// <summary>
        /// Create an Control from the current Input DatItem
        /// <summary>
        private static Models.Listxml.Control CreateControl(Control item)
        {
            var control = new Models.Listxml.Control
            {
                Type = item.ControlType.FromControlType(),
                Player = item.Player?.ToString(),
                Buttons = item.Buttons?.ToString(),
                ReqButtons = item.RequiredButtons?.ToString(),
                Minimum = item.Minimum?.ToString(),
                Maximum = item.Maximum?.ToString(),
                Sensitivity = item.Sensitivity?.ToString(),
                KeyDelta = item.KeyDelta?.ToString(),
                Reverse = item.Reverse.FromYesNo(),
                Ways = item.Ways,
                Ways2 = item.Ways2,
                Ways3 = item.Ways3,
            };

            return control;
        }

        /// <summary>
        /// Create an DipSwitch from the current DipSwitch DatItem
        /// <summary>
        private static Models.Listxml.DipSwitch CreateDipSwitch(DipSwitch item)
        {
            var dipswitch = new Models.Listxml.DipSwitch
            {
                Name = item.Name,
                Tag = item.Tag,
                Mask = item.Mask,
            };

            if (item.ConditionsSpecified)
            {
                var conditionItem = item.Conditions[0];
                var condition = new Models.Listxml.Condition
                {
                    Tag = conditionItem.Tag,
                    Mask = conditionItem.Mask,
                    Relation = conditionItem.Relation.FromRelation(),
                    Value = conditionItem.Value,
                };
                dipswitch.Condition = condition;
            }

            var diplocations = new List<Models.Listxml.DipLocation>();
            foreach (var locationItem in item.Locations ?? new List<Location>())
            {
                var control = CreateDipLocation(locationItem);
                diplocations.Add(control);
            }

            if (diplocations.Any())
                dipswitch.DipLocation = diplocations.ToArray();

            var dipvalues = new List<Models.Listxml.DipValue>();
            foreach (var settingItem in item.Values ?? new List<Setting>())
            {
                var dipvalue = CreateDipValue(settingItem);
                dipvalues.Add(dipvalue);
            }

            if (dipvalues.Any())
                dipswitch.DipValue = dipvalues.ToArray();

            return dipswitch;
        }

        /// <summary>
        /// Create a DipLocation from the current Location DatItem
        /// <summary>
        private static Models.Listxml.DipLocation CreateDipLocation(Location item)
        {
            var diplocation = new Models.Listxml.DipLocation
            {
                Name = item.Name,
                Number = item.Number?.ToString(),
                Inverted = item.Inverted.FromYesNo(),
            };

            return diplocation;
        }

        /// <summary>
        /// Create a DipValue from the current Setting DatItem
        /// <summary>
        private static Models.Listxml.DipValue CreateDipValue(Setting item)
        {
            var dipvalue = new Models.Listxml.DipValue
            {
                Name = item.Name,
                Value = item.Value,
                Default = item.Default.FromYesNo(),
            };

            if (item.ConditionsSpecified)
            {
                var conditionItem = item.Conditions[0];
                var condition = new Models.Listxml.Condition
                {
                    Tag = conditionItem.Tag,
                    Mask = conditionItem.Mask,
                    Relation = conditionItem.Relation.FromRelation(),
                    Value = conditionItem.Value,
                };
                dipvalue.Condition = condition;
            }

            return dipvalue;
        }

        /// <summary>
        /// Create an Configuration from the current Configuration DatItem
        /// <summary>
        private static Models.Listxml.Configuration CreateConfiguration(Configuration item)
        {
            var configuration = new Models.Listxml.Configuration
            {
                Name = item.Name,
                Tag = item.Tag,
                Mask = item.Mask,
            };

            if (item.ConditionsSpecified)
            {
                var conditionItem = item.Conditions[0];
                var condition = new Models.Listxml.Condition
                {
                    Tag = conditionItem.Tag,
                    Mask = conditionItem.Mask,
                    Relation = conditionItem.Relation.FromRelation(),
                    Value = conditionItem.Value,
                };
                configuration.Condition = condition;
            }

            var confLocations = new List<Models.Listxml.ConfLocation>();
            foreach (var location in item.Locations ?? new List<Location>())
            {
                var control = CreateConfLocation(location);
                confLocations.Add(control);
            }

            if (confLocations.Any())
                configuration.ConfLocation = confLocations.ToArray();

            var confsettings = new List<Models.Listxml.ConfSetting>();
            foreach (var settingItem in item.Settings ?? new List<Setting>())
            {
                var dipvalue = CreateConfSetting(settingItem);
                confsettings.Add(dipvalue);
            }

            if (confsettings.Any())
                configuration.ConfSetting = confsettings.ToArray();

            return configuration;
        }

        /// <summary>
        /// Create a ConfLocation from the current Location DatItem
        /// <summary>
        private static Models.Listxml.ConfLocation CreateConfLocation(Location item)
        {
            var conflocation = new Models.Listxml.ConfLocation
            {
                Name = item.Name,
                Number = item.Number?.ToString(),
                Inverted = item.Inverted.FromYesNo(),
            };

            return conflocation;
        }

        /// <summary>
        /// Create a ConfSetting from the current Setting DatItem
        /// <summary>
        private static Models.Listxml.ConfSetting CreateConfSetting(Setting item)
        {
            var confsetting = new Models.Listxml.ConfSetting
            {
                Name = item.Name,
                Value = item.Value,
                Default = item.Default.FromYesNo(),
            };

            if (item.ConditionsSpecified)
            {
                var conditionItem = item.Conditions[0];
                var condition = new Models.Listxml.Condition
                {
                    Tag = conditionItem.Tag,
                    Mask = conditionItem.Mask,
                    Relation = conditionItem.Relation.FromRelation(),
                    Value = conditionItem.Value,
                };
                confsetting.Condition = condition;
            }

            return confsetting;
        }

        /// <summary>
        /// Create a Port from the current Port DatItem
        /// <summary>
        private static Models.Listxml.Port CreatePort(Port item)
        {
            var port = new Models.Listxml.Port
            {
                Tag = item.Tag,
            };

            return port;
        }

        /// <summary>
        /// Create a Adjuster from the current Adjuster DatItem
        /// <summary>
        private static Models.Listxml.Adjuster CreateAdjuster(Adjuster item)
        {
            var adjuster = new Models.Listxml.Adjuster
            {
                Name = item.Name,
                Default = item.Default.FromYesNo(),
            };

            if (item.ConditionsSpecified)
            {
                var conditionItem = item.Conditions[0];
                var condition = new Models.Listxml.Condition
                {
                    Tag = conditionItem.Tag,
                    Mask = conditionItem.Mask,
                    Relation = conditionItem.Relation.FromRelation(),
                    Value = conditionItem.Value,
                };
                adjuster.Condition = condition;
            }

            return adjuster;
        }

        /// <summary>
        /// Create a Driver from the current Driver DatItem
        /// <summary>
        private static Models.Listxml.Driver CreateDriver(Driver item)
        {
            var driver = new Models.Listxml.Driver
            {
                Status = item.Status.FromSupportStatus(),
                //Color = item.Color.FromSupportStatus(), // TODO: Add to internal model
                //Sound = item.Sound.FromSupportStatus(), // TODO: Add to internal model
                //PaletteSize = driver.PaletteSize?.ToString(), // TODO: Add to internal model
                Emulation = item.Emulation.FromSupportStatus(),
                Cocktail = item.Cocktail.FromSupportStatus(),
                SaveState = item.SaveState.FromSupported(verbose: true),
                RequiresArtwork = item.RequiresArtwork.FromYesNo(),
                Unofficial = item.Unofficial.FromYesNo(),
                NoSoundHardware = item.NoSoundHardware.FromYesNo(),
                Incomplete = item.Incomplete.FromYesNo(),
            };

            return driver;
        }

        /// <summary>
        /// Create a Feature from the current Feature DatItem
        /// <summary>
        private static Models.Listxml.Feature CreateFeature(Feature item)
        {
            var feature = new Models.Listxml.Feature
            {
                Type = item.Type.FromFeatureType(),
                Status = item.Status.FromFeatureStatus(),
                Overall = item.Overall.FromFeatureStatus(),
            };

            return feature;
        }

        /// <summary>
        /// Create a Device from the current Device DatItem
        /// <summary>
        private static Models.Listxml.Device CreateDevice(Device item)
        {
            var device = new Models.Listxml.Device
            {
                Type = item.DeviceType.FromDeviceType(),
                Tag = item.Tag,
                FixedImage = item.FixedImage,
                Mandatory = item.Mandatory?.ToString(),
                Interface = item.Interface,
            };

            if (item.InstancesSpecified)
            {
                var instanceItem = item.Instances[0];
                var instance = new Models.Listxml.Instance
                {
                    Name = instanceItem.Name,
                    BriefName = instanceItem.BriefName,
                };
                device.Instance = instance;
            }

            var extensions = new List<Models.Listxml.Extension>();
            foreach (var extensionItem in item.Extensions ?? new List<Extension>())
            {
                var extension = new Models.Listxml.Extension
                {
                    Name = extensionItem.Name,
                };
                extensions.Add(extension);
            }

            if (extensions.Any())
                device.Extension = extensions.ToArray();

            return device;
        }

        /// <summary>
        /// Create a Slot from the current Slot DatItem
        /// <summary>
        private static Models.Listxml.Slot CreateSlot(Slot item)
        {
            var slot = new Models.Listxml.Slot
            {
                Name = item.Name,
            };

            var slotoptions = new List<Models.Listxml.SlotOption>();
            foreach (var slotoptionItem in item.SlotOptions ?? new List<SlotOption>())
            {
                var slotoption = new Models.Listxml.SlotOption
                {
                    Name = slotoptionItem.Name,
                    DevName = slotoptionItem.DeviceName,
                    Default = slotoptionItem.Default.FromYesNo(),
                };
                slotoptions.Add(slotoption);
            }

            if (slotoptions.Any())
                slot.SlotOption = slotoptions.ToArray();

            return slot;
        }

        /// <summary>
        /// Create a SoftwareList from the current SoftwareList DatItem
        /// <summary>
        private static Models.Listxml.SoftwareList CreateSoftwareList(DatItems.Formats.SoftwareList item)
        {
            var softwarelist = new Models.Listxml.SoftwareList
            {
                Tag = item.Tag,
                Name = item.Name,
                Status = item.Status.FromSoftwareListStatus(),
                Filter = item.Filter,
            };

            return softwarelist;
        }

        /// <summary>
        /// Create a RamOption from the current RamOption DatItem
        /// <summary>
        private static Models.Listxml.RamOption CreateRamOption(RamOption item)
        {
            var softwarelist = new Models.Listxml.RamOption
            {
                Name = item.Name,
                Default = item.Default.FromYesNo(),
            };

            return softwarelist;
        }

        #endregion
    }
}
