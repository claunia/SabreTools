using System.Collections.Generic;
using System.Linq;
using SabreTools.Core.Tools;

namespace SabreTools.DatFiles
{
    public partial class DatFile
    {
        #region To Metadata

        /// <summary>
        /// Convert metadata information
        /// </summary>
        public Models.Metadata.MetadataFile? ConvertMetadata(bool ignoreblanks = false)
        {
            // If we don't have items, we can't do anything
            if (this.Items == null || !this.Items.Any())
                return null;

            // Create an object to hold the data
            var metadataFile = new Models.Metadata.MetadataFile();

            // Convert and assign the header
            var header = ConvertHeader();
            if (header != null)
                metadataFile[Models.Metadata.MetadataFile.HeaderKey] = header;

            // Convert and assign the machines
            var machines = ConvertMachines(ignoreblanks);
            if (machines != null)
                metadataFile[Models.Metadata.MetadataFile.MachineKey] = machines;

            return metadataFile;
        }

        /// <summary>
        /// Convert header information
        /// </summary>
        private Models.Metadata.Header? ConvertHeader()
        {
            // If the header is invalid, we can't do anything
            if (Header == null)
                return null;

            // Create an internal header
            var header = Header.GetInternalClone();

            // Convert subheader values
            if (Header.CanOpenSpecified)
                header[Models.Metadata.Header.CanOpenKey] = Header.GetFieldValue<Models.OfflineList.CanOpen[]?>(Models.Metadata.Header.CanOpenKey);
            // if (Header.ImagesSpecified)
            //     // TODO: Add to internal model
            if (Header.InfosSpecified)
            {
                var infoItem = new Models.OfflineList.Infos();
                var infos = Header.GetFieldValue<Formats.OfflineListInfo[]?>(Models.Metadata.Header.InfosKey)!;
                foreach (var info in infos)
                {
                    switch (info.Name)
                    {
                        case "title":
                            infoItem.Title = new Models.OfflineList.Title
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                        case "location":
                            infoItem.Location = new Models.OfflineList.Location
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                        case "publisher":
                            infoItem.Publisher = new Models.OfflineList.Publisher
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                        case "sourceRom":
                            infoItem.SourceRom = new Models.OfflineList.SourceRom
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                        case "saveType":
                            infoItem.SaveType = new Models.OfflineList.SaveType
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                        case "romSize":
                            infoItem.RomSize = new Models.OfflineList.RomSize
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                        case "releaseNumber":
                            infoItem.ReleaseNumber = new Models.OfflineList.ReleaseNumber
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                        case "languageNumber":
                            infoItem.LanguageNumber = new Models.OfflineList.LanguageNumber
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                        case "comment":
                            infoItem.Comment = new Models.OfflineList.Comment
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                        case "romCRC":
                            infoItem.RomCRC = new Models.OfflineList.RomCRC
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                        case "im1CRC":
                            infoItem.Im1CRC = new Models.OfflineList.Im1CRC
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                        case "im2CRC":
                            infoItem.Im2CRC = new Models.OfflineList.Im2CRC
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                        case "languages":
                            infoItem.Languages = new Models.OfflineList.Languages
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                    }
                }

                header[Models.Metadata.Header.InfosKey] = infoItem;
            }
            if (Header.NewDatSpecified)
            {
                var newDat = new Models.OfflineList.NewDat
                {
                    DatVersionUrl = Header.GetFieldValue<string?>("DATVERSIONURL"),
                    //DatUrl = Header.GetFieldValue<Models.OfflineList.DatUrl?>("DATURL"), // TODO: Add to internal model
                    ImUrl = Header.GetFieldValue<string?>("IMURL"),
                };
                header[Models.Metadata.Header.NewDatKey] = newDat;
            }
            // if (Header.SearchSpecified)
            //     // TODO: Add to internal model

            return header;
        }

        /// <summary>
        /// Convert machines information
        /// </summary>
        private Models.Metadata.Machine[]? ConvertMachines(bool ignoreblanks = false)
        {
            // Create a machine list to hold all outputs
            var machines = new List<Models.Metadata.Machine>();

            // Loop through the sorted items and create games for them
            foreach (string key in Items.SortedKeys)
            {
                var items = Items.FilteredItems(key);
                if (items == null || !items.Any())
                    continue;

                // Create a machine to hold everything
                var machine = items[0].GetFieldValue<DatItems.Machine>(DatItems.DatItem.MachineKey)!.GetInternalClone();

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
                        case DatItems.Formats.Adjuster adjuster:
                            var adjusterItem = ProcessItem(adjuster);
                            EnsureMachineKey<Models.Metadata.Adjuster?>(machine, Models.Metadata.Machine.AdjusterKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.AdjusterKey, adjusterItem);
                            break;
                        case DatItems.Formats.Archive archive:
                            var archiveItem = archive.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.Archive?>(machine, Models.Metadata.Machine.ArchiveKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.ArchiveKey, archiveItem);
                            break;
                        case DatItems.Formats.BiosSet biosSet:
                            var biosSetItem = biosSet.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.BiosSet?>(machine, Models.Metadata.Machine.BiosSetKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.BiosSetKey, biosSetItem);
                            break;
                        case DatItems.Formats.Chip chip:
                            var chipItem = chip.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.Chip?>(machine, Models.Metadata.Machine.ChipKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.ChipKey, chipItem);
                            break;
                        case DatItems.Formats.Configuration configuration:
                            var configurationItem = ProcessItem(configuration);
                            EnsureMachineKey<Models.Metadata.Configuration?>(machine, Models.Metadata.Machine.ConfigurationKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.ConfigurationKey, configurationItem);
                            break;
                        case DatItems.Formats.Device device:
                            var deviceItem = ProcessItem(device);
                            EnsureMachineKey<Models.Metadata.Device?>(machine, Models.Metadata.Machine.DeviceKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.DeviceKey, deviceItem);
                            break;
                        case DatItems.Formats.DeviceRef deviceRef:
                            var deviceRefItem = deviceRef.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.DeviceRef?>(machine, Models.Metadata.Machine.DeviceRefKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.DeviceRefKey, deviceRefItem);
                            break;
                        case DatItems.Formats.DipSwitch dipSwitch:
                            var dipSwitchItem = ProcessItem(dipSwitch, machine);
                            EnsureMachineKey<Models.Metadata.DipSwitch?>(machine, Models.Metadata.Machine.DipSwitchKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.DipSwitchKey, dipSwitchItem);
                            break;
                        case DatItems.Formats.Disk disk:
                            var diskItem = ProcessItem(disk, machine);
                            EnsureMachineKey<Models.Metadata.Disk?>(machine, Models.Metadata.Machine.DiskKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.DiskKey, diskItem);
                            break;
                        case DatItems.Formats.Display display:
                            // TODO: Handle cases where it's actually a Video
                            var displayItem = display.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.Display?>(machine, Models.Metadata.Machine.DisplayKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.DisplayKey, displayItem);
                            break;
                        case DatItems.Formats.Driver driver:
                            var driverItem = driver.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.Driver?>(machine, Models.Metadata.Machine.DriverKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.DriverKey, driverItem);
                            break;
                        case DatItems.Formats.Feature feature:
                            var featureItem = feature.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.Feature?>(machine, Models.Metadata.Machine.FeatureKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.FeatureKey, featureItem);
                            break;
                        case DatItems.Formats.Info info:
                            var infoItem = info.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.Info?>(machine, Models.Metadata.Machine.InfoKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.InfoKey, infoItem);
                            break;
                        case DatItems.Formats.Input input:
                            var inputItem = ProcessItem(input);
                            EnsureMachineKey<Models.Metadata.Input?>(machine, Models.Metadata.Machine.InputKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.InputKey, inputItem);
                            break;
                        case DatItems.Formats.Media media:
                            var mediaItem = media.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.Media?>(machine, Models.Metadata.Machine.MediaKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.MediaKey, mediaItem);
                            break;
                        case DatItems.Formats.Port port:
                            var portItem = ProcessItem(port);
                            EnsureMachineKey<Models.Metadata.Port?>(machine, Models.Metadata.Machine.PortKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.PortKey, portItem);
                            break;
                        case DatItems.Formats.RamOption ramOption:
                            var ramOptionItem = ramOption.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.RamOption?>(machine, Models.Metadata.Machine.RamOptionKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.RamOptionKey, ramOptionItem);
                            break;
                        case DatItems.Formats.Release release:
                            var releaseItem = release.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.Release?>(machine, Models.Metadata.Machine.ReleaseKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.ReleaseKey, releaseItem);
                            break;
                        case DatItems.Formats.Rom rom:
                            // TODO: Handle cases where it's actually a Dump
                            var romItem = ProcessItem(rom, machine);
                            EnsureMachineKey<Models.Metadata.Rom?>(machine, Models.Metadata.Machine.RomKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.RomKey, romItem);
                            break;
                        case DatItems.Formats.Sample sample:
                            var sampleItem = sample.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.Sample?>(machine, Models.Metadata.Machine.SampleKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.SampleKey, sampleItem);
                            break;
                        case DatItems.Formats.SharedFeat sharedFeat:
                            var sharedFeatItem = sharedFeat.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.SharedFeat?>(machine, Models.Metadata.Machine.SharedFeatKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.SharedFeatKey, sharedFeatItem);
                            break;
                        case DatItems.Formats.Slot slot:
                            var slotItem = ProcessItem(slot);
                            EnsureMachineKey<Models.Metadata.Slot?>(machine, Models.Metadata.Machine.SlotKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.SlotKey, slotItem);
                            break;
                        case DatItems.Formats.SoftwareList softwareList:
                            var softwareListItem = softwareList.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.SoftwareList?>(machine, Models.Metadata.Machine.SoftwareListKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.SoftwareListKey, softwareListItem);
                            break;
                        case DatItems.Formats.Sound sound:
                            var soundItem = sound.GetInternalClone();
                            EnsureMachineKey<Models.Metadata.Sound?>(machine, Models.Metadata.Machine.SoundKey);
                            AppendToMachineKey(machine, Models.Metadata.Machine.SoundKey, soundItem);
                            break;
                    }
                }

                // Add the machine to the list
                machines.Add(machine);
            }

            // Return the list of machines
            return [.. machines];
        }

        /// <summary>
        /// Convert Adjuster information
        /// </summary>
        /// <param name="item">Item to convert</param>
        private static Models.Metadata.Adjuster ProcessItem(DatItems.Formats.Adjuster item)
        {
            var adjusterItem = item.GetInternalClone();

            var condition = item.GetFieldValue<DatItems.Formats.Condition?>(Models.Metadata.Adjuster.ConditionKey);
            if (condition != null)
                adjusterItem[Models.Metadata.Adjuster.ConditionKey] = condition.GetInternalClone();

            return adjusterItem;
        }

        /// <summary>
        /// Convert Configuration information
        /// </summary>
        /// <param name="item">Item to convert</param>
        private static Models.Metadata.Configuration ProcessItem(DatItems.Formats.Configuration item)
        {
            var configurationItem = item.GetInternalClone();

            var condition = item.GetFieldValue<DatItems.Formats.Condition?>(Models.Metadata.Configuration.ConditionKey);
            if (condition != null)
                configurationItem[Models.Metadata.Configuration.ConditionKey] = condition.GetInternalClone();

            var confLocations = item.GetFieldValue<DatItems.Formats.ConfLocation[]?>(Models.Metadata.Configuration.ConfLocationKey);
            if (confLocations != null)
            {
                var confLocationItems = new List<Models.Metadata.ConfLocation>();
                foreach (var confLocation in confLocations)
                {
                    var confLocationItem = confLocation.GetInternalClone();
                    confLocationItems.Add(confLocationItem);
                }

                configurationItem[Models.Metadata.Configuration.ConfLocationKey] = confLocationItems.ToArray();
            }

            var confSettings = item.GetFieldValue<DatItems.Formats.ConfSetting[]?>(Models.Metadata.Configuration.ConfSettingKey);
            if (confSettings != null)
            {
                var confSettingItems = new List<Models.Metadata.ConfSetting>();
                foreach (var confSetting in confSettings)
                {
                    var confSettingItem = confSetting.GetInternalClone();
                    confSettingItems.Add(confSettingItem);
                }

                configurationItem[Models.Metadata.Configuration.ConfSettingKey] = confSettingItems.ToArray();
            }

            return configurationItem;
        }

        /// <summary>
        /// Convert Device information
        /// </summary>
        /// <param name="item">Item to convert</param>
        private static Models.Metadata.Device ProcessItem(DatItems.Formats.Device item)
        {
            var deviceItem = item.GetInternalClone();

            var instance = item.GetFieldValue<DatItems.Formats.Instance?>(Models.Metadata.Device.InstanceKey);
            if (instance != null)
                deviceItem[Models.Metadata.Device.InstanceKey] = instance.GetInternalClone();

            var extensions = item.GetFieldValue<DatItems.Formats.Extension[]?>(Models.Metadata.Device.ExtensionKey);
            if (extensions != null)
            {
                var extensionItems = new List<Models.Metadata.Extension>();
                foreach (var extension in extensions)
                {
                    var extensionItem = extension.GetInternalClone();
                    extensionItems.Add(extensionItem);
                }

                deviceItem[Models.Metadata.Device.ExtensionKey] = extensionItems.ToArray();
            }

            return deviceItem;
        }

        /// <summary>
        /// Convert DipSwitch information
        /// </summary>
        /// <param name="item">Item to convert</param>
        /// <param name="machine">Machine to use for Part</param>
        private static Models.Metadata.DipSwitch ProcessItem(DatItems.Formats.DipSwitch item, Models.Metadata.Machine machine)
        {
            var dipSwitchItem = item.GetInternalClone();

            var condition = item.GetFieldValue<DatItems.Formats.Condition?>(Models.Metadata.DipSwitch.ConditionKey);
            if (condition != null)
                dipSwitchItem[Models.Metadata.DipSwitch.ConditionKey] = condition.GetInternalClone();

            var dipLocations = item.GetFieldValue<DatItems.Formats.DipLocation[]?>(Models.Metadata.DipSwitch.DipLocationKey);
            if (dipLocations != null)
            {
                var dipLocationItems = new List<Models.Metadata.DipLocation>();
                foreach (var dipLocation in dipLocations)
                {
                    var extensionItem = dipLocation.GetInternalClone();
                    dipLocationItems.Add(extensionItem);
                }

                dipSwitchItem[Models.Metadata.DipSwitch.DipLocationKey] = dipLocationItems.ToArray();
            }

            var dipValues = item.GetFieldValue<DatItems.Formats.DipValue[]?>(Models.Metadata.DipSwitch.DipValueKey);
            if (dipValues != null)
            {
                var dipValueItems = new List<Models.Metadata.DipValue>();
                foreach (var dipValue in dipValues)
                {
                    var extensionItem = dipValue.GetInternalClone();
                    dipValueItems.Add(extensionItem);
                }

                dipSwitchItem[Models.Metadata.DipSwitch.DipValueKey] = dipValueItems.ToArray();
            }

            // TODO: Handle DipSwitch in Part inversion

            return dipSwitchItem;
        }

        /// <summary>
        /// Convert Disk information
        /// </summary>
        /// <param name="item">Item to convert</param>
        /// <param name="machine">Machine to use for Part and DiskArea</param>
        private static Models.Metadata.Disk ProcessItem(DatItems.Formats.Disk item, Models.Metadata.Machine machine)
        {
            var diskItem = item.GetInternalClone();

            // TODO: Handle DipSwitch in Part inversion
            // TODO: Handle DipSwitch in DiskArea inversion

            return diskItem;
        }

        /// <summary>
        /// Convert Input information
        /// </summary>
        /// <param name="item">Item to convert</param>
        private static Models.Metadata.Input ProcessItem(DatItems.Formats.Input item)
        {
            var inputItem = item.GetInternalClone();

            var controls = item.GetFieldValue<DatItems.Formats.Control[]?>(Models.Metadata.Input.ControlKey);
            if (controls != null)
            {
                var controlItems = new List<Models.Metadata.Control>();
                foreach (var control in controls)
                {
                    var controlItem = control.GetInternalClone();
                    controlItems.Add(controlItem);
                }

                inputItem[Models.Metadata.Input.ControlKey] = controlItems.ToArray();
            }

            return inputItem;
        }

        /// <summary>
        /// Convert Port information
        /// </summary>
        /// <param name="item">Item to convert</param>
        private static Models.Metadata.Port ProcessItem(DatItems.Formats.Port item)
        {
            var slotItem = item.GetInternalClone();

            var analogs = item.GetFieldValue<DatItems.Formats.Analog[]?>(Models.Metadata.Port.AnalogKey);
            if (analogs != null)
            {
                var analogItems = new List<Models.Metadata.Analog>();
                foreach (var analog in analogs)
                {
                    var extensionItem = analog.GetInternalClone();
                    analogItems.Add(extensionItem);
                }

                slotItem[Models.Metadata.Port.AnalogKey] = analogItems.ToArray();
            }

            return slotItem;
        }

        /// <summary>
        /// Convert Rom information
        /// </summary>
        /// <param name="item">Item to convert</param>
        /// <param name="machine">Machine to use for Part and DataArea</param>
        private static Models.Metadata.Rom ProcessItem(DatItems.Formats.Rom item, Models.Metadata.Machine machine)
        {
            var romItem = item.GetInternalClone();

            // TODO: Handle DipSwitch in Part inversion
            // TODO: Handle DipSwitch in DataArea inversion

            return romItem;
        }

        /// <summary>
        /// Convert Slot information
        /// </summary>
        /// <param name="item">Item to convert</param>
        private static Models.Metadata.Slot ProcessItem(DatItems.Formats.Slot item)
        {
            var slotItem = item.GetInternalClone();

            var slotOptions = item.GetFieldValue<DatItems.Formats.SlotOption[]?>(Models.Metadata.Slot.SlotOptionKey);
            if (slotOptions != null)
            {
                var slotOptionItems = new List<Models.Metadata.SlotOption>();
                foreach (var slotOption in slotOptions)
                {
                    var extensionItem = slotOption.GetInternalClone();
                    slotOptionItems.Add(extensionItem);
                }

                slotItem[Models.Metadata.Slot.SlotOptionKey] = slotOptionItems.ToArray();
            }

            return slotItem;
        }

        /// <summary>
        /// Ensure a key in a machine
        /// </summary>
        private static void EnsureMachineKey<T>(Models.Metadata.Machine machine, string key)
        {
            if (machine.Read<T[]?>(key) == null)
                machine[key] = new T[0];
        }

        /// <summary>
        /// Append to a machine key as if its an array
        /// </summary>
        private static void AppendToMachineKey<T>(Models.Metadata.Machine machine, string key, T value)
        {
            var arr = machine.Read<T[]>(key);
            if (arr == null)
                return;

            List<T> list = [.. arr];
            list.Add(value);
            machine[key] = list.ToArray();
        }

        #endregion
    }
}