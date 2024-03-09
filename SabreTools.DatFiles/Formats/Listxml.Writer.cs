using System;
using System.Collections.Generic;
using System.Linq;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;

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
            return
            [
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
            ];
        }

        /// <inheritdoc/>
        protected override List<string>? GetMissingRequiredFields(DatItem datItem)
        {
            var missingFields = new List<string>();
            switch (datItem)
            {
                case BiosSet biosset:
                    if (string.IsNullOrEmpty(biosset.GetName()))
                        missingFields.Add(Models.Metadata.BiosSet.NameKey);
                    if (string.IsNullOrEmpty(biosset.Description))
                        missingFields.Add(Models.Metadata.BiosSet.DescriptionKey);
                    break;

                case Rom rom:
                    if (string.IsNullOrEmpty(rom.GetName()))
                        missingFields.Add(Models.Metadata.Rom.NameKey);
                    if (rom.Size == null || rom.Size < 0)
                        missingFields.Add(Models.Metadata.Rom.SizeKey);
                    if (string.IsNullOrEmpty(rom.CRC)
                        && string.IsNullOrEmpty(rom.SHA1))
                    {
                        missingFields.Add(Models.Metadata.Rom.SHA1Key);
                    }
                    break;

                case Disk disk:
                    if (string.IsNullOrEmpty(disk.GetName()))
                        missingFields.Add(Models.Metadata.Disk.NameKey);
                    if (string.IsNullOrEmpty(disk.MD5)
                        && string.IsNullOrEmpty(disk.SHA1))
                    {
                        missingFields.Add(Models.Metadata.Disk.SHA1Key);
                    }
                    break;

                case DeviceReference deviceref:
                    if (string.IsNullOrEmpty(deviceref.GetName()))
                        missingFields.Add(Models.Metadata.DeviceRef.NameKey);
                    break;

                case Sample sample:
                    if (string.IsNullOrEmpty(sample.GetName()))
                        missingFields.Add(Models.Metadata.Sample.NameKey);
                    break;

                case Chip chip:
                    if (string.IsNullOrEmpty(chip.GetName()))
                        missingFields.Add(Models.Metadata.Chip.NameKey);
                    if (!chip.ChipTypeSpecified)
                        missingFields.Add(Models.Metadata.Chip.ChipTypeKey);
                    break;

                case Display display:
                    if (!display.DisplayTypeSpecified)
                        missingFields.Add(Models.Metadata.Display.DisplayTypeKey);
                    if (display.Refresh == null)
                        missingFields.Add(Models.Metadata.Display.RefreshKey);
                    break;

                case Sound sound:
                    if (sound.Channels == null)
                        missingFields.Add(Models.Metadata.Sound.ChannelsKey);
                    break;

                case Input input:
                    if (input.Players == null)
                        missingFields.Add(Models.Metadata.Input.PlayersKey);
                    break;

                case DipSwitch dipswitch:
                    if (string.IsNullOrEmpty(dipswitch.GetName()))
                        missingFields.Add(Models.Metadata.DipSwitch.NameKey);
                    if (string.IsNullOrEmpty(dipswitch.Tag))
                        missingFields.Add(Models.Metadata.DipSwitch.TagKey);
                    break;

                case Configuration configuration:
                    if (string.IsNullOrEmpty(configuration.GetName()))
                        missingFields.Add(Models.Metadata.Configuration.NameKey);
                    if (string.IsNullOrEmpty(configuration.Tag))
                        missingFields.Add(Models.Metadata.Configuration.TagKey);
                    break;

                case Port port:
                    if (string.IsNullOrEmpty(port.Tag))
                        missingFields.Add(Models.Metadata.Port.TagKey);
                    break;

                case Adjuster adjuster:
                    if (string.IsNullOrEmpty(adjuster.GetName()))
                        missingFields.Add(Models.Metadata.Adjuster.NameKey);
                    break;

                case Driver driver:
                    if (!driver.StatusSpecified)
                        missingFields.Add(Models.Metadata.Driver.StatusKey);
                    if (!driver.EmulationSpecified)
                        missingFields.Add(Models.Metadata.Driver.EmulationKey);
                    if (!driver.CocktailSpecified)
                        missingFields.Add(Models.Metadata.Driver.CocktailKey);
                    if (!driver.SaveStateSpecified)
                        missingFields.Add(Models.Metadata.Driver.SaveStateKey);
                    break;

                case Feature feature:
                    if (!feature.TypeSpecified)
                        missingFields.Add(Models.Metadata.Feature.FeatureTypeKey);
                    break;

                case Device device:
                    if (!device.DeviceTypeSpecified)
                        missingFields.Add(Models.Metadata.Device.DeviceTypeKey);
                    break;

                case Slot slot:
                    if (string.IsNullOrEmpty(slot.GetName()))
                        missingFields.Add(Models.Metadata.Slot.NameKey);
                    break;

                case DatItems.Formats.SoftwareList softwarelist:
                    if (string.IsNullOrEmpty(softwarelist.Tag))
                        missingFields.Add(Models.Metadata.SoftwareList.TagKey);
                    if (string.IsNullOrEmpty(softwarelist.GetName()))
                        missingFields.Add(Models.Metadata.SoftwareList.NameKey);
                    if (!softwarelist.StatusSpecified)
                        missingFields.Add(Models.Metadata.SoftwareList.StatusKey);
                    break;

                case RamOption ramoption:
                    if (string.IsNullOrEmpty(ramoption.GetName()))
                        missingFields.Add(Models.Metadata.RamOption.NameKey);
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
                if (!(new Serialization.Files.Listxml().Serialize(mame, outfile)))
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
                var game = Listxml.CreateGame(machine!);

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
                game.BiosSet = [.. biosSets];
                game.Rom = [.. roms];
                game.Disk = [.. disks];
                game.DeviceRef = [.. deviceRefs];
                game.Sample = [.. samples];
                game.Chip = [.. chips];
                game.Display = [.. displays];
                game.Video = null;
                game.DipSwitch = [.. dipSwitches];
                game.Configuration = [.. configurations];
                game.Port = [.. ports];
                game.Adjuster = [.. adjusters];
                game.Feature = [.. features];
                game.Device = [.. devices];
                game.Slot = [.. slots];
                game.SoftwareList = [.. softwareLists];
                game.RamOption = [.. ramOptions];

                // Add the game to the list
                games.Add(game);
            }

            return [.. games];
        }

        /// <summary>
        /// Create a GameBase from the current internal information
        /// <summary>
        private static Models.Listxml.GameBase CreateGame(Machine machine)
        {
            var game = new Models.Listxml.Machine
            {
                Name = machine.Name,
                SourceFile = machine.SourceFile,
                Runnable = machine.Runnable.AsStringValue<Runnable>(),
                CloneOf = machine.CloneOf,
                RomOf = machine.RomOf,
                SampleOf = machine.SampleOf,
                Description = machine.Description,
                Year = machine.Year,
                Manufacturer = machine.Manufacturer,
                History = machine.History,
            };

#if NETFRAMEWORK
            if ((machine.MachineType & MachineType.Bios) != 0)
                game.IsBios = "yes";
            if ((machine.MachineType & MachineType.Device) != 0)
                game.IsDevice = "yes";
            if ((machine.MachineType & MachineType.Mechanical) != 0)
                game.IsMechanical = "yes";
#else
            if (machine.MachineType.HasFlag(MachineType.Bios))
                game.IsBios = "yes";
            if (machine.MachineType.HasFlag(MachineType.Device))
                game.IsDevice = "yes";
            if (machine.MachineType.HasFlag(MachineType.Mechanical))
                game.IsMechanical = "yes";
#endif

            return game;
        }

        /// <summary>
        /// Create a BiosSet from the current BiosSet DatItem
        /// <summary>
        private static Models.Listxml.BiosSet CreateBiosSet(BiosSet item)
        {
            var biosset = new Models.Listxml.BiosSet
            {
                Name = item.GetName(),
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
                Name = item.GetName(),
                Bios = item.Bios,
                Size = item.Size?.ToString(),
                CRC = item.CRC,
                SHA1 = item.SHA1,
                Merge = item.MergeTag,
                Region = item.Region,
                Offset = item.Offset,
                Status = item.ItemStatus.AsStringValue<ItemStatus>(useSecond: false),
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
                Name = item.GetName(),
                MD5 = item.MD5,
                SHA1 = item.SHA1,
                Merge = item.MergeTag,
                Region = item.Region,
                Index = item.Index,
                Writable = item.Writable.FromYesNo(),
                Status = item.ItemStatus.AsStringValue<ItemStatus>(useSecond: false),
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
                Name = item.GetName(),
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
                Name = item.GetName(),
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
                Name = item.GetName(),
                Tag = item.Tag,
                Type = item.ChipType.AsStringValue<ChipType>(),
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
                Type = item.DisplayType.AsStringValue<DisplayType>(),
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
            foreach (var controlItem in item.Controls ?? [])
            {
                var control = CreateControl(controlItem);
                controls.Add(control);
            }

            if (controls.Any())
                input.Control = [.. controls];

            return input;
        }

        /// <summary>
        /// Create an Control from the current Input DatItem
        /// <summary>
        private static Models.Listxml.Control CreateControl(Control item)
        {
            var control = new Models.Listxml.Control
            {
                Type = item.ControlType.AsStringValue<ControlType>(),
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
                Name = item.GetName(),
                Tag = item.Tag,
                Mask = item.Mask,
            };

            if (item.ConditionsSpecified)
            {
                var conditionItem = item.Conditions?.FirstOrDefault();
                var condition = new Models.Listxml.Condition
                {
                    Tag = conditionItem?.Tag,
                    Mask = conditionItem?.Mask,
                    Relation = conditionItem?.Relation.AsStringValue<Relation>(),
                    Value = conditionItem?.Value,
                };
                dipswitch.Condition = condition;
            }

            var diplocations = new List<Models.Listxml.DipLocation>();
            foreach (var locationItem in item.Locations ?? [])
            {
                var control = CreateDipLocation(locationItem);
                diplocations.Add(control);
            }

            if (diplocations.Any())
                dipswitch.DipLocation = [.. diplocations];

            var dipvalues = new List<Models.Listxml.DipValue>();
            foreach (var dipValueItem in item.Values ?? [])
            {
                var dipvalue = CreateDipValue(dipValueItem);
                dipvalues.Add(dipvalue);
            }

            if (dipvalues.Any())
                dipswitch.DipValue = [.. dipvalues];

            return dipswitch;
        }

        /// <summary>
        /// Create a DipLocation from the current DipLocation DatItem
        /// <summary>
        private static Models.Listxml.DipLocation CreateDipLocation(DipLocation item)
        {
            var diplocation = new Models.Listxml.DipLocation
            {
                Name = item.GetName(),
                Number = item.Number?.ToString(),
                Inverted = item.Inverted.FromYesNo(),
            };

            return diplocation;
        }

        /// <summary>
        /// Create a DipValue from the current DipValue DatItem
        /// <summary>
        private static Models.Listxml.DipValue CreateDipValue(DipValue item)
        {
            var dipvalue = new Models.Listxml.DipValue
            {
                Name = item.GetName(),
                Value = item.Value,
                Default = item.Default.FromYesNo(),
            };

            if (item.ConditionsSpecified)
            {
                var conditionItem = item.Conditions?.FirstOrDefault();
                var condition = new Models.Listxml.Condition
                {
                    Tag = conditionItem?.Tag,
                    Mask = conditionItem?.Mask,
                    Relation = conditionItem?.Relation.AsStringValue<Relation>(),
                    Value = conditionItem?.Value,
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
                Name = item.GetName(),
                Tag = item.Tag,
                Mask = item.Mask,
            };

            if (item.ConditionsSpecified)
            {
                var conditionItem = item.Conditions?.FirstOrDefault();
                var condition = new Models.Listxml.Condition
                {
                    Tag = conditionItem?.Tag,
                    Mask = conditionItem?.Mask,
                    Relation = conditionItem?.Relation.AsStringValue<Relation>(),
                    Value = conditionItem?.Value,
                };
                configuration.Condition = condition;
            }

            var confLocations = new List<Models.Listxml.ConfLocation>();
            foreach (var location in item.Locations ?? [])
            {
                var control = CreateConfLocation(location);
                confLocations.Add(control);
            }

            if (confLocations.Any())
                configuration.ConfLocation = [.. confLocations];

            var confsettings = new List<Models.Listxml.ConfSetting>();
            foreach (var confSettingItem in item.Settings ?? [])
            {
                var dipvalue = CreateConfSetting(confSettingItem);
                confsettings.Add(dipvalue);
            }

            if (confsettings.Any())
                configuration.ConfSetting = [.. confsettings];

            return configuration;
        }

        /// <summary>
        /// Create a ConfLocation from the current ConfLocation DatItem
        /// <summary>
        private static Models.Listxml.ConfLocation CreateConfLocation(ConfLocation item)
        {
            var conflocation = new Models.Listxml.ConfLocation
            {
                Name = item.GetName(),
                Number = item.Number?.ToString(),
                Inverted = item.Inverted.FromYesNo(),
            };

            return conflocation;
        }

        /// <summary>
        /// Create a ConfSetting from the current ConfSetting DatItem
        /// <summary>
        private static Models.Listxml.ConfSetting CreateConfSetting(ConfSetting item)
        {
            var confsetting = new Models.Listxml.ConfSetting
            {
                Name = item.GetName(),
                Value = item.Value,
                Default = item.Default.FromYesNo(),
            };

            if (item.ConditionsSpecified)
            {
                var conditionItem = item.Conditions?.FirstOrDefault();
                var condition = new Models.Listxml.Condition
                {
                    Tag = conditionItem?.Tag,
                    Mask = conditionItem?.Mask,
                    Relation = conditionItem?.Relation.AsStringValue<Relation>(),
                    Value = conditionItem?.Value,
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
                Name = item.GetName(),
                Default = item.Default.FromYesNo(),
            };

            if (item.ConditionsSpecified)
            {
                var conditionItem = item.Conditions?.FirstOrDefault();
                var condition = new Models.Listxml.Condition
                {
                    Tag = conditionItem?.Tag,
                    Mask = conditionItem?.Mask,
                    Relation = conditionItem?.Relation.AsStringValue<Relation>(),
                    Value = conditionItem?.Value,
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
                Status = item.Status.AsStringValue<SupportStatus>(),
                //Color = item.Color.AsStringValue<SupportStatus>(), // TODO: Add to internal model
                //Sound = item.Sound.AsStringValue<SupportStatus>(), // TODO: Add to internal model
                //PaletteSize = driver.PaletteSize?.ToString(), // TODO: Add to internal model
                Emulation = item.Emulation.AsStringValue<SupportStatus>(),
                Cocktail = item.Cocktail.AsStringValue<SupportStatus>(),
                SaveState = item.SaveState.AsStringValue<Supported>(useSecond: true),
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
                Type = item.Type.AsStringValue<FeatureType>(),
                Status = item.Status.AsStringValue<FeatureStatus>(),
                Overall = item.Overall.AsStringValue<FeatureStatus>(),
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
                Type = item.DeviceType.AsStringValue<DeviceType>(),
                Tag = item.Tag,
                FixedImage = item.FixedImage,
                Mandatory = item.Mandatory?.ToString(),
                Interface = item.Interface,
            };

            if (item.InstancesSpecified)
            {
                var instanceItem = item.Instances?.FirstOrDefault();
                var instance = new Models.Listxml.Instance
                {
                    Name = instanceItem?.GetName(),
                    BriefName = instanceItem?.BriefName,
                };
                device.Instance = instance;
            }

            var extensions = new List<Models.Listxml.Extension>();
            foreach (var extensionItem in item.Extensions ?? [])
            {
                var extension = new Models.Listxml.Extension
                {
                    Name = extensionItem.GetName(),
                };
                extensions.Add(extension);
            }

            if (extensions.Any())
                device.Extension = [.. extensions];

            return device;
        }

        /// <summary>
        /// Create a Slot from the current Slot DatItem
        /// <summary>
        private static Models.Listxml.Slot CreateSlot(Slot item)
        {
            var slot = new Models.Listxml.Slot
            {
                Name = item.GetName(),
            };

            var slotoptions = new List<Models.Listxml.SlotOption>();
            foreach (var slotoptionItem in item.SlotOptions ?? [])
            {
                var slotoption = new Models.Listxml.SlotOption
                {
                    Name = slotoptionItem.GetName(),
                    DevName = slotoptionItem.DeviceName,
                    Default = slotoptionItem.Default.FromYesNo(),
                };
                slotoptions.Add(slotoption);
            }

            if (slotoptions.Any())
                slot.SlotOption = [.. slotoptions];

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
                Name = item.GetName(),
                Status = item.Status.AsStringValue<SoftwareListStatus>(),
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
                Name = item.GetName(),
                Default = item.Default.FromYesNo(),
                Content = item.Content,
            };

            return softwarelist;
        }

        #endregion
    }
}
