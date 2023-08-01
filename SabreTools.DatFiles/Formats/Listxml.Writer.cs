using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
            // TODO: Check required fields
            return null;
        }

        /// <inheritdoc/>
        public override bool WriteToFile(string outfile, bool ignoreblanks = false, bool throwOnError = false)
        {
            try
            {
                logger.User($"Writing to '{outfile}'...");
                FileStream fs = System.IO.File.Create(outfile);

                // If we get back null for some reason, just log and return
                if (fs == null)
                {
                    logger.Warning($"File '{outfile}' could not be created for writing! Please check to see if the file is writable");
                    return false;
                }

                XmlTextWriter xtw = new(fs, new UTF8Encoding(false))
                {
                    Formatting = Formatting.Indented,
                    IndentChar = '\t',
                    Indentation = 1
                };

                // Write out the header
                WriteHeader(xtw);

                // Write out each of the machines and roms
                string lastgame = null;

                // Use a sorted list of games to output
                foreach (string key in Items.SortedKeys)
                {
                    ConcurrentList<DatItem> datItems = Items.FilteredItems(key);

                    // If this machine doesn't contain any writable items, skip
                    if (!ContainsWritable(datItems))
                        continue;

                    // Resolve the names in the block
                    datItems = DatItem.ResolveNames(datItems);

                    for (int index = 0; index < datItems.Count; index++)
                    {
                        DatItem datItem = datItems[index];

                        // If we have a different game and we're not at the start of the list, output the end of last item
                        if (lastgame != null && lastgame.ToLowerInvariant() != datItem.Machine.Name.ToLowerInvariant())
                            WriteEndGame(xtw);

                        // If we have a new game, output the beginning of the new item
                        if (lastgame == null || lastgame.ToLowerInvariant() != datItem.Machine.Name.ToLowerInvariant())
                            WriteStartGame(xtw, datItem);

                        // Check for a "null" item
                        datItem = ProcessNullifiedItem(datItem);

                        // Write out the item if we're not ignoring
                        if (!ShouldIgnore(datItem, ignoreblanks))
                            WriteDatItem(xtw, datItem);

                        // Set the new data to compare against
                        lastgame = datItem.Machine.Name;
                    }
                }

                // Write the file footer out
                WriteFooter(xtw);

                logger.User($"'{outfile}' written!{Environment.NewLine}");
                xtw.Dispose();
                fs.Dispose();
            }
            catch (Exception ex) when (!throwOnError)
            {
                logger.Error(ex);
                return false;
            }

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
    }
}
