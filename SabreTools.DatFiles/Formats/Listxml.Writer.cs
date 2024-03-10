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
                    if (string.IsNullOrEmpty(biosset.GetFieldValue<string?>(Models.Metadata.BiosSet.DescriptionKey)))
                        missingFields.Add(Models.Metadata.BiosSet.DescriptionKey);
                    break;

                case Rom rom:
                    if (string.IsNullOrEmpty(rom.GetName()))
                        missingFields.Add(Models.Metadata.Rom.NameKey);
                    if (rom.GetFieldValue<long?>(Models.Metadata.Rom.SizeKey) == null || rom.GetFieldValue<long?>(Models.Metadata.Rom.SizeKey) < 0)
                        missingFields.Add(Models.Metadata.Rom.SizeKey);
                    if (string.IsNullOrEmpty(rom.GetFieldValue<string?>(Models.Metadata.Rom.CRCKey))
                        && string.IsNullOrEmpty(rom.GetFieldValue<string?>(Models.Metadata.Rom.SHA1Key)))
                    {
                        missingFields.Add(Models.Metadata.Rom.SHA1Key);
                    }
                    break;

                case Disk disk:
                    if (string.IsNullOrEmpty(disk.GetName()))
                        missingFields.Add(Models.Metadata.Disk.NameKey);
                    if (string.IsNullOrEmpty(disk.GetFieldValue<string?>(Models.Metadata.Disk.MD5Key))
                        && string.IsNullOrEmpty(disk.GetFieldValue<string?>(Models.Metadata.Disk.SHA1Key)))
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
                    if (chip.GetFieldValue<ChipType>(Models.Metadata.Chip.ChipTypeKey) == ChipType.NULL)
                        missingFields.Add(Models.Metadata.Chip.ChipTypeKey);
                    break;

                case Display display:
                    if (display.GetFieldValue<DisplayType>(Models.Metadata.Display.DisplayTypeKey) == DisplayType.NULL)
                        missingFields.Add(Models.Metadata.Display.DisplayTypeKey);
                    if (display.GetFieldValue<double?>(Models.Metadata.Display.RefreshKey) == null)
                        missingFields.Add(Models.Metadata.Display.RefreshKey);
                    break;

                case Sound sound:
                    if (sound.GetFieldValue<long?>(Models.Metadata.Sound.ChannelsKey) == null)
                        missingFields.Add(Models.Metadata.Sound.ChannelsKey);
                    break;

                case Input input:
                    if (input.GetFieldValue<long?>(Models.Metadata.Input.PlayersKey) == null)
                        missingFields.Add(Models.Metadata.Input.PlayersKey);
                    break;

                case DipSwitch dipswitch:
                    if (string.IsNullOrEmpty(dipswitch.GetName()))
                        missingFields.Add(Models.Metadata.DipSwitch.NameKey);
                    if (string.IsNullOrEmpty(dipswitch.GetFieldValue<string?>(Models.Metadata.DipSwitch.TagKey)))
                        missingFields.Add(Models.Metadata.DipSwitch.TagKey);
                    break;

                case Configuration configuration:
                    if (string.IsNullOrEmpty(configuration.GetName()))
                        missingFields.Add(Models.Metadata.Configuration.NameKey);
                    if (string.IsNullOrEmpty(configuration.GetFieldValue<string>(Models.Metadata.Configuration.TagKey)))
                        missingFields.Add(Models.Metadata.Configuration.TagKey);
                    break;

                case Port port:
                    if (string.IsNullOrEmpty(port.GetFieldValue<string>(Models.Metadata.Port.TagKey)))
                        missingFields.Add(Models.Metadata.Port.TagKey);
                    break;

                case Adjuster adjuster:
                    if (string.IsNullOrEmpty(adjuster.GetName()))
                        missingFields.Add(Models.Metadata.Adjuster.NameKey);
                    break;

                case Driver driver:
                    if (driver.GetFieldValue<SupportStatus>(Models.Metadata.Driver.StatusKey) == SupportStatus.NULL)
                        missingFields.Add(Models.Metadata.Driver.StatusKey);
                    if (driver.GetFieldValue<SupportStatus>(Models.Metadata.Driver.EmulationKey) == SupportStatus.NULL)
                        missingFields.Add(Models.Metadata.Driver.EmulationKey);
                    if (driver.GetFieldValue<SupportStatus>(Models.Metadata.Driver.CocktailKey) == SupportStatus.NULL)
                        missingFields.Add(Models.Metadata.Driver.CocktailKey);
                    if (driver.GetFieldValue<SupportStatus>(Models.Metadata.Driver.SaveStateKey) == SupportStatus.NULL)
                        missingFields.Add(Models.Metadata.Driver.SaveStateKey);
                    break;

                case Feature feature:
                    if (feature.GetFieldValue<FeatureType>(Models.Metadata.Feature.FeatureTypeKey) == FeatureType.NULL)
                        missingFields.Add(Models.Metadata.Feature.FeatureTypeKey);
                    break;

                case Device device:
                    if (device.GetFieldValue<DeviceType>(Models.Metadata.Device.DeviceTypeKey) != DeviceType.NULL)
                        missingFields.Add(Models.Metadata.Device.DeviceTypeKey);
                    break;

                case Slot slot:
                    if (string.IsNullOrEmpty(slot.GetName()))
                        missingFields.Add(Models.Metadata.Slot.NameKey);
                    break;

                case DatItems.Formats.SoftwareList softwarelist:
                    if (string.IsNullOrEmpty(softwarelist.GetFieldValue<string?>(Models.Metadata.SoftwareList.TagKey)))
                        missingFields.Add(Models.Metadata.SoftwareList.TagKey);
                    if (string.IsNullOrEmpty(softwarelist.GetName()))
                        missingFields.Add(Models.Metadata.SoftwareList.NameKey);
                    if (softwarelist.GetFieldValue<SoftwareListStatus?>(Models.Metadata.SoftwareList.StatusKey) == SoftwareListStatus.None)
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
                Build = Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey)
                    ?? Header.GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey)
                    ?? Header.GetFieldValue<string?>(Models.Metadata.Header.BuildKey),
                Debug = Header.GetFieldValue<bool?>(Models.Metadata.Header.DebugKey).FromYesNo(),
                MameConfig = Header.GetFieldValue<string?>(Models.Metadata.Header.MameConfigKey),

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
                Name = machine.GetFieldValue<string?>(Models.Metadata.Machine.NameKey),
                SourceFile = machine.GetFieldValue<string?>(Models.Metadata.Machine.SourceFileKey),
                Runnable = machine.GetFieldValue<Runnable>(Models.Metadata.Machine.RunnableKey).AsStringValue<Runnable>(),
                CloneOf = machine.GetFieldValue<string?>(Models.Metadata.Machine.CloneOfKey),
                RomOf = machine.GetFieldValue<string?>(Models.Metadata.Machine.RomOfKey),
                SampleOf = machine.GetFieldValue<string?>(Models.Metadata.Machine.SampleOfKey),
                Description = machine.GetFieldValue<string?>(Models.Metadata.Machine.DescriptionKey),
                Year = machine.GetFieldValue<string?>(Models.Metadata.Machine.YearKey),
                Manufacturer = machine.GetFieldValue<string?>(Models.Metadata.Machine.ManufacturerKey),
                History = machine.GetFieldValue<string?>(Models.Metadata.Machine.HistoryKey),
            };

            if (machine.GetFieldValue<bool?>(Models.Metadata.Machine.IsBiosKey) == true)
                game.IsBios = "yes";
            if (machine.GetFieldValue<bool?>(Models.Metadata.Machine.IsDeviceKey) == true)
                game.IsDevice = "yes";
            if (machine.GetFieldValue<bool?>(Models.Metadata.Machine.IsMechanicalKey) == true)
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
                Default = item.GetFieldValue<bool?>(Models.Metadata.BiosSet.DefaultKey).FromYesNo(),
                Description = item.GetFieldValue<string?>(Models.Metadata.BiosSet.DescriptionKey),
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
                Bios = item.GetFieldValue<string?>(Models.Metadata.Rom.BiosKey),
                Size = item.GetFieldValue<long?>(Models.Metadata.Rom.SizeKey)?.ToString(),
                CRC = item.GetFieldValue<string?>(Models.Metadata.Rom.CRCKey),
                SHA1 = item.GetFieldValue<string?>(Models.Metadata.Rom.SHA1Key),
                Merge = item.GetFieldValue<string?>(Models.Metadata.Rom.MergeKey),
                Region = item.GetFieldValue<string?>(Models.Metadata.Rom.RegionKey),
                Offset = item.GetFieldValue<string?>(Models.Metadata.Rom.OffsetKey),
                Status = item.GetFieldValue<ItemStatus>(Models.Metadata.Rom.StatusKey).AsStringValue<ItemStatus>(useSecond: false),
                Optional = item.GetFieldValue<bool?>(Models.Metadata.Rom.OptionalKey).FromYesNo(),
                Dispose = item.GetFieldValue<bool?>(Models.Metadata.Rom.DisposeKey).FromYesNo(),
                SoundOnly = item.GetFieldValue<bool?>(Models.Metadata.Rom.SoundOnlyKey).FromYesNo(),
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
                MD5 = item.GetFieldValue<string?>(Models.Metadata.Disk.MD5Key),
                SHA1 = item.GetFieldValue<string?>(Models.Metadata.Disk.SHA1Key),
                Merge = item.GetFieldValue<string?>(Models.Metadata.Disk.MergeKey),
                Region = item.GetFieldValue<string?>(Models.Metadata.Disk.RegionKey),
                Index = item.GetFieldValue<string?>(Models.Metadata.Disk.IndexKey),
                Writable = item.GetFieldValue<bool?>(Models.Metadata.Disk.WritableKey).FromYesNo(),
                Status = item.GetFieldValue<ItemStatus>(Models.Metadata.Disk.StatusKey).AsStringValue<ItemStatus>(useSecond: false),
                Optional = item.GetFieldValue<bool?>(Models.Metadata.Disk.OptionalKey).FromYesNo(),
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
                Tag = item.GetFieldValue<string?>(Models.Metadata.Chip.TagKey),
                Type = item.GetFieldValue<ChipType>(Models.Metadata.Chip.ChipTypeKey).AsStringValue<ChipType>(),
                SoundOnly = item.GetFieldValue<bool?>(Models.Metadata.Chip.SoundOnlyKey).FromYesNo(),
                Clock = item.GetFieldValue<long?>(Models.Metadata.Chip.TagKey)?.ToString(),
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
                Tag = item.GetFieldValue<string?>(Models.Metadata.Display.TagKey),
                Type = item.GetFieldValue<DisplayType>(Models.Metadata.Display.DisplayTypeKey).AsStringValue<DisplayType>(),
                Rotate = item.GetFieldValue<long?>(Models.Metadata.Display.RotateKey)?.ToString(),
                FlipX = item.GetFieldValue<bool?>(Models.Metadata.Display.FlipXKey).FromYesNo(),
                Width = item.GetFieldValue<string?>(Models.Metadata.Display.WidthKey)?.ToString(),
                Height = item.GetFieldValue<string?>(Models.Metadata.Display.HeightKey)?.ToString(),
                Refresh = item.GetFieldValue<double?>(Models.Metadata.Display.RefreshKey)?.ToString(),
                PixClock = item.GetFieldValue<string?>(Models.Metadata.Display.PixClockKey)?.ToString(),
                HTotal = item.GetFieldValue<string?>(Models.Metadata.Display.HTotalKey)?.ToString(),
                HBEnd = item.GetFieldValue<string?>(Models.Metadata.Display.HBEndKey)?.ToString(),
                HBStart = item.GetFieldValue<string?>(Models.Metadata.Display.HBStartKey)?.ToString(),
                VTotal = item.GetFieldValue<string?>(Models.Metadata.Display.VTotalKey)?.ToString(),
                VBEnd = item.GetFieldValue<string?>(Models.Metadata.Display.VBEndKey)?.ToString(),
                VBStart = item.GetFieldValue<string?>(Models.Metadata.Display.VBStartKey)?.ToString(),
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
                Channels = item.GetFieldValue<long?>(Models.Metadata.Sound.ChannelsKey)?.ToString(),
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
                Service = item.GetFieldValue<bool?>(Models.Metadata.Input.ServiceKey).FromYesNo(),
                Tilt = item.GetFieldValue<bool?>(Models.Metadata.Input.TiltKey).FromYesNo(),
                Players = item.GetFieldValue<long?>(Models.Metadata.Input.PlayersKey)?.ToString(),
                //ControlAttr = item.GetFieldValue<string?>(Models.Metadata.Input.ControlKey),
                Buttons = item.GetFieldValue<long?>(Models.Metadata.Input.ButtonsKey)?.ToString(),
                Coins = item.GetFieldValue<long?>(Models.Metadata.Input.CoinsKey)?.ToString(),
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
                Type = item.GetFieldValue<ControlType>(Models.Metadata.Control.ControlTypeKey).AsStringValue<ControlType>(),
                Player = item.GetFieldValue<long?>(Models.Metadata.Control.PlayerKey)?.ToString(),
                Buttons = item.GetFieldValue<long?>(Models.Metadata.Control.ButtonsKey)?.ToString(),
                ReqButtons = item.GetFieldValue<long?>(Models.Metadata.Control.ReqButtonsKey)?.ToString(),
                Minimum = item.GetFieldValue<long?>(Models.Metadata.Control.MinimumKey)?.ToString(),
                Maximum = item.GetFieldValue<long?>(Models.Metadata.Control.MaximumKey)?.ToString(),
                Sensitivity = item.GetFieldValue<long?>(Models.Metadata.Control.SensitivityKey)?.ToString(),
                KeyDelta = item.GetFieldValue<long?>(Models.Metadata.Control.KeyDeltaKey)?.ToString(),
                Reverse = item.GetFieldValue<bool?>(Models.Metadata.Control.ReverseKey).FromYesNo(),
                Ways = item.GetFieldValue<string?>(Models.Metadata.Control.WaysKey),
                Ways2 = item.GetFieldValue<string?>(Models.Metadata.Control.Ways2Key),
                Ways3 = item.GetFieldValue<string?>(Models.Metadata.Control.Ways3Key),
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
                Tag = item.GetFieldValue<string?>(Models.Metadata.DipSwitch.TagKey),
                Mask = item.GetFieldValue<string?>(Models.Metadata.DipSwitch.MaskKey),
            };

            if (item.ConditionsSpecified)
            {
                var conditionItem = item.GetFieldValue<Condition[]?>(Models.Metadata.DipSwitch.ConditionKey)?.FirstOrDefault();
                var condition = new Models.Listxml.Condition
                {
                    Tag = conditionItem?.GetFieldValue<string?>(Models.Metadata.Condition.TagKey),
                    Mask = conditionItem?.GetFieldValue<string?>(Models.Metadata.Condition.MaskKey),
                    Relation = conditionItem?.GetFieldValue<Relation>(Models.Metadata.Condition.RelationKey).AsStringValue<Relation>(),
                    Value = conditionItem?.GetFieldValue<string?>(Models.Metadata.Condition.ValueKey),
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
                Number = item.GetFieldValue<long?>(Models.Metadata.DipLocation.NumberKey)?.ToString(),
                Inverted = item.GetFieldValue<bool?>(Models.Metadata.DipLocation.InvertedKey).FromYesNo(),
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
                Value = item.GetFieldValue<string?>(Models.Metadata.DipValue.ValueKey),
                Default = item.GetFieldValue<bool?>(Models.Metadata.DipValue.DefaultKey).FromYesNo(),
            };

            if (item.ConditionsSpecified)
            {
                var conditionItem = item.GetFieldValue<Condition[]?>(Models.Metadata.DipValue.ConditionKey)?.FirstOrDefault();
                var condition = new Models.Listxml.Condition
                {
                    Tag = conditionItem?.GetFieldValue<string?>(Models.Metadata.Condition.TagKey),
                    Mask = conditionItem?.GetFieldValue<string?>(Models.Metadata.Condition.MaskKey),
                    Relation = conditionItem?.GetFieldValue<Relation>(Models.Metadata.Condition.RelationKey).AsStringValue<Relation>(),
                    Value = conditionItem?.GetFieldValue<string?>(Models.Metadata.Condition.ValueKey),
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
                Tag = item.GetFieldValue<string>(Models.Metadata.Configuration.TagKey),
                Mask = item.GetFieldValue<string>(Models.Metadata.Configuration.MaskKey),
            };

            if (item.ConditionsSpecified)
            {
                var conditionItem = item.GetFieldValue<Condition[]?>(Models.Metadata.Configuration.ConditionKey)?.FirstOrDefault();
                var condition = new Models.Listxml.Condition
                {
                    Tag = conditionItem?.GetFieldValue<string?>(Models.Metadata.Condition.TagKey),
                    Mask = conditionItem?.GetFieldValue<string?>(Models.Metadata.Condition.MaskKey),
                    Relation = conditionItem?.GetFieldValue<Relation>(Models.Metadata.Condition.RelationKey).AsStringValue<Relation>(),
                    Value = conditionItem?.GetFieldValue<string?>(Models.Metadata.Condition.ValueKey),
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
                Number = item.GetFieldValue<long?>(Models.Metadata.ConfLocation.NumberKey)?.ToString(),
                Inverted = item.GetFieldValue<bool?>(Models.Metadata.ConfLocation.InvertedKey).FromYesNo(),
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
                Value = item.GetFieldValue<string?>(Models.Metadata.ConfSetting.ValueKey),
                Default = item.GetFieldValue<bool?>(Models.Metadata.ConfSetting.DefaultKey).FromYesNo(),
            };

            if (item.ConditionsSpecified)
            {
                var conditionItem = item.GetFieldValue<Condition[]?>(Models.Metadata.ConfSetting.ConditionKey)?.FirstOrDefault();
                var condition = new Models.Listxml.Condition
                {
                    Tag = conditionItem?.GetFieldValue<string?>(Models.Metadata.Condition.TagKey),
                    Mask = conditionItem?.GetFieldValue<string?>(Models.Metadata.Condition.MaskKey),
                    Relation = conditionItem?.GetFieldValue<Relation>(Models.Metadata.Condition.RelationKey).AsStringValue<Relation>(),
                    Value = conditionItem?.GetFieldValue<string?>(Models.Metadata.Condition.ValueKey),
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
                Tag = item.GetFieldValue<string>(Models.Metadata.Port.TagKey),
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
                Default = item.GetFieldValue<bool?>(Models.Metadata.Adjuster.DefaultKey).FromYesNo(),
            };

            if (item.ConditionsSpecified)
            {
                var conditionItem = item.GetFieldValue<Condition[]?>(Models.Metadata.Adjuster.ConditionKey)?.FirstOrDefault();
                var condition = new Models.Listxml.Condition
                {
                    Tag = conditionItem?.GetFieldValue<string?>(Models.Metadata.Condition.TagKey),
                    Mask = conditionItem?.GetFieldValue<string?>(Models.Metadata.Condition.MaskKey),
                    Relation = conditionItem?.GetFieldValue<Relation>(Models.Metadata.Condition.RelationKey).AsStringValue<Relation>(),
                    Value = conditionItem?.GetFieldValue<string?>(Models.Metadata.Condition.ValueKey),
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
                Status = item.GetFieldValue<SupportStatus>(Models.Metadata.Driver.StatusKey).AsStringValue<SupportStatus>(),
                Color = item.GetFieldValue<SupportStatus>(Models.Metadata.Driver.ColorKey).AsStringValue<SupportStatus>(),
                Sound = item.GetFieldValue<SupportStatus>(Models.Metadata.Driver.SoundKey).AsStringValue<SupportStatus>(),
                PaletteSize = item.GetFieldValue<long?>(Models.Metadata.Driver.PaletteSizeKey)?.ToString(),
                Emulation = item.GetFieldValue<SupportStatus>(Models.Metadata.Driver.EmulationKey).AsStringValue<SupportStatus>(),
                Cocktail = item.GetFieldValue<SupportStatus>(Models.Metadata.Driver.CocktailKey).AsStringValue<SupportStatus>(),
                SaveState = item.GetFieldValue<Supported>(Models.Metadata.Driver.SaveStateKey).AsStringValue<Supported>(useSecond: true),
                RequiresArtwork = item.GetFieldValue<bool?>(Models.Metadata.Driver.RequiresArtworkKey).FromYesNo(),
                Unofficial = item.GetFieldValue<bool?>(Models.Metadata.Driver.UnofficialKey).FromYesNo(),
                NoSoundHardware = item.GetFieldValue<bool?>(Models.Metadata.Driver.NoSoundHardwareKey).FromYesNo(),
                Incomplete = item.GetFieldValue<bool?>(Models.Metadata.Driver.IncompleteKey).FromYesNo(),
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
                Type = item.GetFieldValue<FeatureType>(Models.Metadata.Feature.FeatureTypeKey).AsStringValue<FeatureType>(),
                Status = item.GetFieldValue<FeatureStatus>(Models.Metadata.Feature.StatusKey).AsStringValue<FeatureStatus>(),
                Overall = item.GetFieldValue<FeatureStatus>(Models.Metadata.Feature.OverallKey).AsStringValue<FeatureStatus>(),
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
                Type = item.GetFieldValue<DeviceType>(Models.Metadata.Device.DeviceTypeKey).AsStringValue<DeviceType>(),
                Tag = item.GetFieldValue<string?>(Models.Metadata.Device.TagKey),
                FixedImage = item.GetFieldValue<string?>(Models.Metadata.Device.FixedImageKey),
                Mandatory = item.GetFieldValue<long?>(Models.Metadata.Device.MandatoryKey)?.ToString(),
                Interface = item.GetFieldValue<string?>(Models.Metadata.Device.InterfaceKey),
            };

            if (item.InstancesSpecified)
            {
                var instanceItem = item.GetFieldValue<Instance[]?>(Models.Metadata.Device.InstanceKey)?.FirstOrDefault();
                var instance = new Models.Listxml.Instance
                {
                    Name = instanceItem?.GetName(),
                    BriefName = instanceItem?.GetFieldValue<string?>(Models.Metadata.Instance.BriefNameKey),
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
                    DevName = slotoptionItem.GetFieldValue<string?>(Models.Metadata.SlotOption.DevNameKey),
                    Default = slotoptionItem.GetFieldValue<bool?>(Models.Metadata.SlotOption.DefaultKey).FromYesNo(),
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
                Tag = item.GetFieldValue<string?>(Models.Metadata.SoftwareList.TagKey),
                Name = item.GetName(),
                Status = item.GetFieldValue<SoftwareListStatus>(Models.Metadata.SoftwareList.StatusKey).AsStringValue<SoftwareListStatus>(),
                Filter = item.GetFieldValue<string?>(Models.Metadata.SoftwareList.FilterKey),
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
                Default = item.GetFieldValue<bool?>(Models.Metadata.RamOption.DefaultKey).FromYesNo(),
                Content = item.GetFieldValue<string?>(Models.Metadata.RamOption.ContentKey),
            };

            return softwarelist;
        }

        #endregion
    }
}
