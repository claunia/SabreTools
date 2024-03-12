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
                ItemType.DeviceRef,
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
                    if (string.IsNullOrEmpty(biosset.GetStringFieldValue(Models.Metadata.BiosSet.DescriptionKey)))
                        missingFields.Add(Models.Metadata.BiosSet.DescriptionKey);
                    break;

                case Rom rom:
                    if (string.IsNullOrEmpty(rom.GetName()))
                        missingFields.Add(Models.Metadata.Rom.NameKey);
                    if (rom.GetInt64FieldValue(Models.Metadata.Rom.SizeKey) == null || rom.GetInt64FieldValue(Models.Metadata.Rom.SizeKey) < 0)
                        missingFields.Add(Models.Metadata.Rom.SizeKey);
                    if (string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.CRCKey))
                        && string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA1Key)))
                    {
                        missingFields.Add(Models.Metadata.Rom.SHA1Key);
                    }
                    break;

                case Disk disk:
                    if (string.IsNullOrEmpty(disk.GetName()))
                        missingFields.Add(Models.Metadata.Disk.NameKey);
                    if (string.IsNullOrEmpty(disk.GetStringFieldValue(Models.Metadata.Disk.MD5Key))
                        && string.IsNullOrEmpty(disk.GetStringFieldValue(Models.Metadata.Disk.SHA1Key)))
                    {
                        missingFields.Add(Models.Metadata.Disk.SHA1Key);
                    }
                    break;

                case DeviceRef deviceref:
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
                    if (chip.GetStringFieldValue(Models.Metadata.Chip.ChipTypeKey).AsEnumValue<ChipType>() == ChipType.NULL)
                        missingFields.Add(Models.Metadata.Chip.ChipTypeKey);
                    break;

                case Display display:
                    if (display.GetStringFieldValue(Models.Metadata.Display.DisplayTypeKey).AsEnumValue<DisplayType>() == DisplayType.NULL)
                        missingFields.Add(Models.Metadata.Display.DisplayTypeKey);
                    if (display.GetDoubleFieldValue(Models.Metadata.Display.RefreshKey) == null)
                        missingFields.Add(Models.Metadata.Display.RefreshKey);
                    break;

                case Sound sound:
                    if (sound.GetInt64FieldValue(Models.Metadata.Sound.ChannelsKey) == null)
                        missingFields.Add(Models.Metadata.Sound.ChannelsKey);
                    break;

                case Input input:
                    if (input.GetInt64FieldValue(Models.Metadata.Input.PlayersKey) == null)
                        missingFields.Add(Models.Metadata.Input.PlayersKey);
                    break;

                case DipSwitch dipswitch:
                    if (string.IsNullOrEmpty(dipswitch.GetName()))
                        missingFields.Add(Models.Metadata.DipSwitch.NameKey);
                    if (string.IsNullOrEmpty(dipswitch.GetStringFieldValue(Models.Metadata.DipSwitch.TagKey)))
                        missingFields.Add(Models.Metadata.DipSwitch.TagKey);
                    break;

                case Configuration configuration:
                    if (string.IsNullOrEmpty(configuration.GetName()))
                        missingFields.Add(Models.Metadata.Configuration.NameKey);
                    if (string.IsNullOrEmpty(configuration.GetStringFieldValue(Models.Metadata.Configuration.TagKey)))
                        missingFields.Add(Models.Metadata.Configuration.TagKey);
                    break;

                case Port port:
                    if (string.IsNullOrEmpty(port.GetStringFieldValue(Models.Metadata.Port.TagKey)))
                        missingFields.Add(Models.Metadata.Port.TagKey);
                    break;

                case Adjuster adjuster:
                    if (string.IsNullOrEmpty(adjuster.GetName()))
                        missingFields.Add(Models.Metadata.Adjuster.NameKey);
                    break;

                case Driver driver:
                    if (driver.GetStringFieldValue(Models.Metadata.Driver.StatusKey).AsEnumValue<SupportStatus>() == SupportStatus.NULL)
                        missingFields.Add(Models.Metadata.Driver.StatusKey);
                    if (driver.GetStringFieldValue(Models.Metadata.Driver.EmulationKey).AsEnumValue<SupportStatus>() == SupportStatus.NULL)
                        missingFields.Add(Models.Metadata.Driver.EmulationKey);
                    if (driver.GetStringFieldValue(Models.Metadata.Driver.CocktailKey).AsEnumValue<SupportStatus>() == SupportStatus.NULL)
                        missingFields.Add(Models.Metadata.Driver.CocktailKey);
                    if (driver.GetStringFieldValue(Models.Metadata.Driver.SaveStateKey).AsEnumValue<SupportStatus>() == SupportStatus.NULL)
                        missingFields.Add(Models.Metadata.Driver.SaveStateKey);
                    break;

                case Feature feature:
                    if (feature.GetStringFieldValue(Models.Metadata.Feature.FeatureTypeKey).AsEnumValue<FeatureType>() == FeatureType.NULL)
                        missingFields.Add(Models.Metadata.Feature.FeatureTypeKey);
                    break;

                case Device device:
                    if (device.GetStringFieldValue(Models.Metadata.Device.DeviceTypeKey).AsEnumValue<DeviceType>() != DeviceType.NULL)
                        missingFields.Add(Models.Metadata.Device.DeviceTypeKey);
                    break;

                case Slot slot:
                    if (string.IsNullOrEmpty(slot.GetName()))
                        missingFields.Add(Models.Metadata.Slot.NameKey);
                    break;

                case DatItems.Formats.SoftwareList softwarelist:
                    if (string.IsNullOrEmpty(softwarelist.GetStringFieldValue(Models.Metadata.SoftwareList.TagKey)))
                        missingFields.Add(Models.Metadata.SoftwareList.TagKey);
                    if (string.IsNullOrEmpty(softwarelist.GetName()))
                        missingFields.Add(Models.Metadata.SoftwareList.NameKey);
                    if (softwarelist.GetStringFieldValue(Models.Metadata.SoftwareList.StatusKey).AsEnumValue<SoftwareListStatus>() == SoftwareListStatus.None)
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
                Build = Header.GetStringFieldValue(Models.Metadata.Header.NameKey)
                    ?? Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey)
                    ?? Header.GetStringFieldValue(Models.Metadata.Header.BuildKey),
                Debug = Header.GetBoolFieldValue(Models.Metadata.Header.DebugKey).FromYesNo(),
                MameConfig = Header.GetStringFieldValue(Models.Metadata.Header.MameConfigKey),

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
                var machine = items[0].GetFieldValue<Machine>(DatItem.MachineKey);
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
                        case DeviceRef deviceref:
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
                Name = machine.GetStringFieldValue(Models.Metadata.Machine.NameKey),
                SourceFile = machine.GetStringFieldValue(Models.Metadata.Machine.SourceFileKey),
                Runnable = machine.GetStringFieldValue(Models.Metadata.Machine.RunnableKey).AsEnumValue<Runnable>().AsStringValue(),
                CloneOf = machine.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey),
                RomOf = machine.GetStringFieldValue(Models.Metadata.Machine.RomOfKey),
                SampleOf = machine.GetStringFieldValue(Models.Metadata.Machine.SampleOfKey),
                Description = machine.GetStringFieldValue(Models.Metadata.Machine.DescriptionKey),
                Year = machine.GetStringFieldValue(Models.Metadata.Machine.YearKey),
                Manufacturer = machine.GetStringFieldValue(Models.Metadata.Machine.ManufacturerKey),
                History = machine.GetStringFieldValue(Models.Metadata.Machine.HistoryKey),
            };

            if (machine.GetBoolFieldValue(Models.Metadata.Machine.IsBiosKey) == true)
                game.IsBios = "yes";
            if (machine.GetBoolFieldValue(Models.Metadata.Machine.IsDeviceKey) == true)
                game.IsDevice = "yes";
            if (machine.GetBoolFieldValue(Models.Metadata.Machine.IsMechanicalKey) == true)
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
                Name = item.GetName(),
                Default = item.GetBoolFieldValue(Models.Metadata.BiosSet.DefaultKey).FromYesNo(),
                Description = item.GetStringFieldValue(Models.Metadata.BiosSet.DescriptionKey),
            };

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
                Bios = item.GetStringFieldValue(Models.Metadata.Rom.BiosKey),
                Size = item.GetStringFieldValue(Models.Metadata.Rom.SizeKey),
                CRC = item.GetStringFieldValue(Models.Metadata.Rom.CRCKey),
                SHA1 = item.GetStringFieldValue(Models.Metadata.Rom.SHA1Key),
                Merge = item.GetStringFieldValue(Models.Metadata.Rom.MergeKey),
                Region = item.GetStringFieldValue(Models.Metadata.Rom.RegionKey),
                Offset = item.GetStringFieldValue(Models.Metadata.Rom.OffsetKey),
                Status = item.GetStringFieldValue(Models.Metadata.Rom.StatusKey).AsEnumValue<ItemStatus>().AsStringValue(useSecond: false),
                Optional = item.GetBoolFieldValue(Models.Metadata.Rom.OptionalKey).FromYesNo(),
                Dispose = item.GetBoolFieldValue(Models.Metadata.Rom.DisposeKey).FromYesNo(),
                SoundOnly = item.GetBoolFieldValue(Models.Metadata.Rom.SoundOnlyKey).FromYesNo(),
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
                MD5 = item.GetStringFieldValue(Models.Metadata.Disk.MD5Key),
                SHA1 = item.GetStringFieldValue(Models.Metadata.Disk.SHA1Key),
                Merge = item.GetStringFieldValue(Models.Metadata.Disk.MergeKey),
                Region = item.GetStringFieldValue(Models.Metadata.Disk.RegionKey),
                Index = item.GetStringFieldValue(Models.Metadata.Disk.IndexKey),
                Writable = item.GetBoolFieldValue(Models.Metadata.Disk.WritableKey).FromYesNo(),
                Status = item.GetStringFieldValue(Models.Metadata.Disk.StatusKey).AsEnumValue<ItemStatus>().AsStringValue(useSecond: false),
                Optional = item.GetBoolFieldValue(Models.Metadata.Disk.OptionalKey).FromYesNo(),
            };

            return disk;
        }

        /// <summary>
        /// Create a DeviceRef from the current DeviceReference DatItem
        /// <summary>
        private static Models.Listxml.DeviceRef CreateDeviceRef(DeviceRef item)
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
                Tag = item.GetStringFieldValue(Models.Metadata.Chip.TagKey),
                Type = item.GetStringFieldValue(Models.Metadata.Chip.ChipTypeKey).AsEnumValue<ChipType>().AsStringValue(),
                SoundOnly = item.GetBoolFieldValue(Models.Metadata.Chip.SoundOnlyKey).FromYesNo(),
                Clock = item.GetInt64FieldValue(Models.Metadata.Chip.TagKey)?.ToString(),
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
                Tag = item.GetStringFieldValue(Models.Metadata.Display.TagKey),
                Type = item.GetStringFieldValue(Models.Metadata.Display.DisplayTypeKey).AsEnumValue<DisplayType>().AsStringValue(),
                Rotate = item.GetInt64FieldValue(Models.Metadata.Display.RotateKey)?.ToString(),
                FlipX = item.GetBoolFieldValue(Models.Metadata.Display.FlipXKey).FromYesNo(),
                Width = item.GetStringFieldValue(Models.Metadata.Display.WidthKey)?.ToString(),
                Height = item.GetStringFieldValue(Models.Metadata.Display.HeightKey)?.ToString(),
                Refresh = item.GetDoubleFieldValue(Models.Metadata.Display.RefreshKey)?.ToString(),
                PixClock = item.GetStringFieldValue(Models.Metadata.Display.PixClockKey)?.ToString(),
                HTotal = item.GetStringFieldValue(Models.Metadata.Display.HTotalKey)?.ToString(),
                HBEnd = item.GetStringFieldValue(Models.Metadata.Display.HBEndKey)?.ToString(),
                HBStart = item.GetStringFieldValue(Models.Metadata.Display.HBStartKey)?.ToString(),
                VTotal = item.GetStringFieldValue(Models.Metadata.Display.VTotalKey)?.ToString(),
                VBEnd = item.GetStringFieldValue(Models.Metadata.Display.VBEndKey)?.ToString(),
                VBStart = item.GetStringFieldValue(Models.Metadata.Display.VBStartKey)?.ToString(),
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
                Channels = item.GetInt64FieldValue(Models.Metadata.Sound.ChannelsKey)?.ToString(),
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
                Service = item.GetBoolFieldValue(Models.Metadata.Input.ServiceKey).FromYesNo(),
                Tilt = item.GetBoolFieldValue(Models.Metadata.Input.TiltKey).FromYesNo(),
                Players = item.GetInt64FieldValue(Models.Metadata.Input.PlayersKey)?.ToString(),
                //ControlAttr = item.GetStringFieldValue(Models.Metadata.Input.ControlKey),
                Buttons = item.GetInt64FieldValue(Models.Metadata.Input.ButtonsKey)?.ToString(),
                Coins = item.GetInt64FieldValue(Models.Metadata.Input.CoinsKey)?.ToString(),
            };

            var controls = new List<Models.Listxml.Control>();
            foreach (var controlItem in item.GetFieldValue<Control[]?>(Models.Metadata.Input.ControlKey) ?? [])
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
                Type = item.GetStringFieldValue(Models.Metadata.Control.ControlTypeKey).AsEnumValue<ControlType>().AsStringValue(),
                Player = item.GetInt64FieldValue(Models.Metadata.Control.PlayerKey)?.ToString(),
                Buttons = item.GetInt64FieldValue(Models.Metadata.Control.ButtonsKey)?.ToString(),
                ReqButtons = item.GetInt64FieldValue(Models.Metadata.Control.ReqButtonsKey)?.ToString(),
                Minimum = item.GetInt64FieldValue(Models.Metadata.Control.MinimumKey)?.ToString(),
                Maximum = item.GetInt64FieldValue(Models.Metadata.Control.MaximumKey)?.ToString(),
                Sensitivity = item.GetInt64FieldValue(Models.Metadata.Control.SensitivityKey)?.ToString(),
                KeyDelta = item.GetInt64FieldValue(Models.Metadata.Control.KeyDeltaKey)?.ToString(),
                Reverse = item.GetBoolFieldValue(Models.Metadata.Control.ReverseKey).FromYesNo(),
                Ways = item.GetStringFieldValue(Models.Metadata.Control.WaysKey),
                Ways2 = item.GetStringFieldValue(Models.Metadata.Control.Ways2Key),
                Ways3 = item.GetStringFieldValue(Models.Metadata.Control.Ways3Key),
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
                Tag = item.GetStringFieldValue(Models.Metadata.DipSwitch.TagKey),
                Mask = item.GetStringFieldValue(Models.Metadata.DipSwitch.MaskKey),
            };

            if (item.ConditionsSpecified)
            {
                var conditionItem = item.GetFieldValue<Condition[]?>(Models.Metadata.DipSwitch.ConditionKey)?.FirstOrDefault();
                var condition = new Models.Listxml.Condition
                {
                    Tag = conditionItem?.GetStringFieldValue(Models.Metadata.Condition.TagKey),
                    Mask = conditionItem?.GetStringFieldValue(Models.Metadata.Condition.MaskKey),
                    Relation = conditionItem?.GetStringFieldValue(Models.Metadata.Condition.RelationKey).AsEnumValue<Relation>().AsStringValue(),
                    Value = conditionItem?.GetStringFieldValue(Models.Metadata.Condition.ValueKey),
                };
                dipswitch.Condition = condition;
            }

            var diplocations = new List<Models.Listxml.DipLocation>();
            foreach (var locationItem in item.GetFieldValue<DipLocation[]?>(Models.Metadata.DipSwitch.DipLocationKey) ?? [])
            {
                var control = CreateDipLocation(locationItem);
                diplocations.Add(control);
            }

            if (diplocations.Any())
                dipswitch.DipLocation = [.. diplocations];

            var dipvalues = new List<Models.Listxml.DipValue>();
            foreach (var dipValueItem in item.GetFieldValue<DipValue[]?>(Models.Metadata.DipSwitch.DipValueKey) ?? [])
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
                Number = item.GetInt64FieldValue(Models.Metadata.DipLocation.NumberKey)?.ToString(),
                Inverted = item.GetBoolFieldValue(Models.Metadata.DipLocation.InvertedKey).FromYesNo(),
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
                Value = item.GetStringFieldValue(Models.Metadata.DipValue.ValueKey),
                Default = item.GetBoolFieldValue(Models.Metadata.DipValue.DefaultKey).FromYesNo(),
            };

            if (item.ConditionsSpecified)
            {
                var conditionItem = item.GetFieldValue<Condition[]?>(Models.Metadata.DipValue.ConditionKey)?.FirstOrDefault();
                var condition = new Models.Listxml.Condition
                {
                    Tag = conditionItem?.GetStringFieldValue(Models.Metadata.Condition.TagKey),
                    Mask = conditionItem?.GetStringFieldValue(Models.Metadata.Condition.MaskKey),
                    Relation = conditionItem?.GetStringFieldValue(Models.Metadata.Condition.RelationKey).AsEnumValue<Relation>().AsStringValue(),
                    Value = conditionItem?.GetStringFieldValue(Models.Metadata.Condition.ValueKey),
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
                Tag = item.GetStringFieldValue(Models.Metadata.Configuration.TagKey),
                Mask = item.GetStringFieldValue(Models.Metadata.Configuration.MaskKey),
            };

            if (item.ConditionsSpecified)
            {
                var conditionItem = item.GetFieldValue<Condition[]?>(Models.Metadata.Configuration.ConditionKey)?.FirstOrDefault();
                var condition = new Models.Listxml.Condition
                {
                    Tag = conditionItem?.GetStringFieldValue(Models.Metadata.Condition.TagKey),
                    Mask = conditionItem?.GetStringFieldValue(Models.Metadata.Condition.MaskKey),
                    Relation = conditionItem?.GetStringFieldValue(Models.Metadata.Condition.RelationKey).AsEnumValue<Relation>().AsStringValue(),
                    Value = conditionItem?.GetStringFieldValue(Models.Metadata.Condition.ValueKey),
                };
                configuration.Condition = condition;
            }

            var confLocations = new List<Models.Listxml.ConfLocation>();
            foreach (var location in item.GetFieldValue<ConfLocation[]?>(Models.Metadata.Configuration.ConfLocationKey) ?? [])
            {
                var control = CreateConfLocation(location);
                confLocations.Add(control);
            }

            if (confLocations.Any())
                configuration.ConfLocation = [.. confLocations];

            var confsettings = new List<Models.Listxml.ConfSetting>();
            foreach (var confSettingItem in item.GetFieldValue<ConfSetting[]?>(Models.Metadata.Configuration.ConfSettingKey) ?? [])
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
                Number = item.GetInt64FieldValue(Models.Metadata.ConfLocation.NumberKey)?.ToString(),
                Inverted = item.GetBoolFieldValue(Models.Metadata.ConfLocation.InvertedKey).FromYesNo(),
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
                Value = item.GetStringFieldValue(Models.Metadata.ConfSetting.ValueKey),
                Default = item.GetBoolFieldValue(Models.Metadata.ConfSetting.DefaultKey).FromYesNo(),
            };

            if (item.ConditionsSpecified)
            {
                var conditionItem = item.GetFieldValue<Condition[]?>(Models.Metadata.ConfSetting.ConditionKey)?.FirstOrDefault();
                var condition = new Models.Listxml.Condition
                {
                    Tag = conditionItem?.GetStringFieldValue(Models.Metadata.Condition.TagKey),
                    Mask = conditionItem?.GetStringFieldValue(Models.Metadata.Condition.MaskKey),
                    Relation = conditionItem?.GetStringFieldValue(Models.Metadata.Condition.RelationKey).AsEnumValue<Relation>().AsStringValue(),
                    Value = conditionItem?.GetStringFieldValue(Models.Metadata.Condition.ValueKey),
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
                Tag = item.GetStringFieldValue(Models.Metadata.Port.TagKey),
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
                Default = item.GetBoolFieldValue(Models.Metadata.Adjuster.DefaultKey).FromYesNo(),
            };

            if (item.ConditionsSpecified)
            {
                var conditionItem = item.GetFieldValue<Condition[]?>(Models.Metadata.Adjuster.ConditionKey)?.FirstOrDefault();
                var condition = new Models.Listxml.Condition
                {
                    Tag = conditionItem?.GetStringFieldValue(Models.Metadata.Condition.TagKey),
                    Mask = conditionItem?.GetStringFieldValue(Models.Metadata.Condition.MaskKey),
                    Relation = conditionItem?.GetStringFieldValue(Models.Metadata.Condition.RelationKey).AsEnumValue<Relation>().AsStringValue(),
                    Value = conditionItem?.GetStringFieldValue(Models.Metadata.Condition.ValueKey),
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
                Status = item.GetStringFieldValue(Models.Metadata.Driver.StatusKey).AsEnumValue<SupportStatus>().AsStringValue(),
                Color = item.GetStringFieldValue(Models.Metadata.Driver.ColorKey).AsEnumValue<SupportStatus>().AsStringValue(),
                Sound = item.GetStringFieldValue(Models.Metadata.Driver.SoundKey).AsEnumValue<SupportStatus>().AsStringValue(),
                PaletteSize = item.GetInt64FieldValue(Models.Metadata.Driver.PaletteSizeKey)?.ToString(),
                Emulation = item.GetStringFieldValue(Models.Metadata.Driver.EmulationKey).AsEnumValue<SupportStatus>().AsStringValue(),
                Cocktail = item.GetStringFieldValue(Models.Metadata.Driver.CocktailKey).AsEnumValue<SupportStatus>().AsStringValue(),
                SaveState = item.GetStringFieldValue(Models.Metadata.Driver.SaveStateKey).AsEnumValue<Supported>().AsStringValue(useSecond: true),
                RequiresArtwork = item.GetBoolFieldValue(Models.Metadata.Driver.RequiresArtworkKey).FromYesNo(),
                Unofficial = item.GetBoolFieldValue(Models.Metadata.Driver.UnofficialKey).FromYesNo(),
                NoSoundHardware = item.GetBoolFieldValue(Models.Metadata.Driver.NoSoundHardwareKey).FromYesNo(),
                Incomplete = item.GetBoolFieldValue(Models.Metadata.Driver.IncompleteKey).FromYesNo(),
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
                Type = item.GetStringFieldValue(Models.Metadata.Feature.FeatureTypeKey).AsEnumValue<FeatureType>().AsStringValue(),
                Status = item.GetStringFieldValue(Models.Metadata.Feature.StatusKey).AsEnumValue<FeatureStatus>().AsStringValue(),
                Overall = item.GetStringFieldValue(Models.Metadata.Feature.OverallKey).AsEnumValue<FeatureStatus>().AsStringValue(),
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
                Type = item.GetStringFieldValue(Models.Metadata.Device.DeviceTypeKey).AsEnumValue<DeviceType>().AsStringValue(),
                Tag = item.GetStringFieldValue(Models.Metadata.Device.TagKey),
                FixedImage = item.GetStringFieldValue(Models.Metadata.Device.FixedImageKey),
                Mandatory = item.GetInt64FieldValue(Models.Metadata.Device.MandatoryKey)?.ToString(),
                Interface = item.GetStringFieldValue(Models.Metadata.Device.InterfaceKey),
            };

            if (item.InstancesSpecified)
            {
                var instanceItem = item.GetFieldValue<Instance[]?>(Models.Metadata.Device.InstanceKey)?.FirstOrDefault();
                var instance = new Models.Listxml.Instance
                {
                    Name = instanceItem?.GetName(),
                    BriefName = instanceItem?.GetStringFieldValue(Models.Metadata.Instance.BriefNameKey),
                };
                device.Instance = instance;
            }

            var extensions = new List<Models.Listxml.Extension>();
            foreach (var extensionItem in item.GetFieldValue<Extension[]?>(Models.Metadata.Device.ExtensionKey) ?? [])
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
            foreach (var slotoptionItem in item.GetFieldValue<SlotOption[]?>(Models.Metadata.Slot.SlotOptionKey) ?? [])
            {
                var slotoption = new Models.Listxml.SlotOption
                {
                    Name = slotoptionItem.GetName(),
                    DevName = slotoptionItem.GetStringFieldValue(Models.Metadata.SlotOption.DevNameKey),
                    Default = slotoptionItem.GetBoolFieldValue(Models.Metadata.SlotOption.DefaultKey).FromYesNo(),
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
                Tag = item.GetStringFieldValue(Models.Metadata.SoftwareList.TagKey),
                Name = item.GetName(),
                Status = item.GetStringFieldValue(Models.Metadata.SoftwareList.StatusKey).AsEnumValue<SoftwareListStatus>().AsStringValue(),
                Filter = item.GetStringFieldValue(Models.Metadata.SoftwareList.FilterKey),
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
                Default = item.GetBoolFieldValue(Models.Metadata.RamOption.DefaultKey).FromYesNo(),
                Content = item.GetStringFieldValue(Models.Metadata.RamOption.ContentKey),
            };

            return softwarelist;
        }

        #endregion
    }
}
