using System.Collections.Generic;
using System.Linq;
using SabreTools.Models.Internal;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for Listxml models to internal structure
    /// </summary>
    public partial class Internal
    {
        #region Serialize

        /// <summary>
        /// Convert from <cref="Models.Listxml.M1"/> to <cref="Header"/>
        /// </summary>
        public static Header ConvertHeaderFromListxml(Models.Listxml.M1 item)
        {
            var header = new Header
            {
                [Header.VersionKey] = item.Version,
            };
            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Mame"/> to <cref="Header"/>
        /// </summary>
        public static Header ConvertHeaderFromListxml(Models.Listxml.Mame item)
        {
            var header = new Header
            {
                [Header.BuildKey] = item.Build,
                [Header.DebugKey] = item.Debug,
                [Header.MameConfigKey] = item.MameConfig,
            };
            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.GameBase"/> to <cref="Machine"/>
        /// </summary>
        public static Machine ConvertMachineFromListxml(Models.Listxml.GameBase item)
        {
            var machine = new Machine
            {
                [Machine.NameKey] = item.Name,
                [Machine.SourceFileKey] = item.SourceFile,
                [Machine.IsBiosKey] = item.IsBios,
                [Machine.IsDeviceKey] = item.IsDevice,
                [Machine.IsMechanicalKey] = item.IsMechanical,
                [Machine.RunnableKey] = item.Runnable,
                [Machine.CloneOfKey] = item.CloneOf,
                [Machine.RomOfKey] = item.RomOf,
                [Machine.SampleOfKey] = item.SampleOf,
                [Machine.DescriptionKey] = item.Description,
                [Machine.YearKey] = item.Year,
                [Machine.ManufacturerKey] = item.Manufacturer,
                [Machine.HistoryKey] = item.History,
            };

            if (item.BiosSet != null && item.BiosSet.Any())
            {
                var biosSets = new List<BiosSet>();
                foreach (var biosSet in item.BiosSet)
                {
                    biosSets.Add(ConvertFromListxml(biosSet));
                }
                machine[Machine.BiosSetKey] = biosSets.ToArray();
            }

            if (item.Rom != null && item.Rom.Any())
            {
                var roms = new List<Rom>();
                foreach (var rom in item.Rom)
                {
                    roms.Add(ConvertFromListxml(rom));
                }
                machine[Machine.RomKey] = roms.ToArray();
            }

            if (item.Disk != null && item.Disk.Any())
            {
                var disks = new List<Disk>();
                foreach (var disk in item.Disk)
                {
                    disks.Add(ConvertFromListxml(disk));
                }
                machine[Machine.DiskKey] = disks.ToArray();
            }

            if (item.DeviceRef != null && item.DeviceRef.Any())
            {
                var deviceRefs = new List<DeviceRef>();
                foreach (var deviceRef in item.DeviceRef)
                {
                    deviceRefs.Add(ConvertFromListxml(deviceRef));
                }
                machine[Machine.DeviceRefKey] = deviceRefs.ToArray();
            }

            if (item.Sample != null && item.Sample.Any())
            {
                var samples = new List<Sample>();
                foreach (var sample in item.Sample)
                {
                    samples.Add(ConvertFromListxml(sample));
                }
                machine[Machine.SampleKey] = samples.ToArray();
            }

            if (item.Chip != null && item.Chip.Any())
            {
                var chips = new List<Chip>();
                foreach (var chip in item.Chip)
                {
                    chips.Add(ConvertFromListxml(chip));
                }
                machine[Machine.ChipKey] = chips.ToArray();
            }

            if (item.Display != null && item.Display.Any())
            {
                var displays = new List<Display>();
                foreach (var display in item.Display)
                {
                    displays.Add(ConvertFromListxml(display));
                }
                machine[Machine.DisplayKey] = displays.ToArray();
            }

            if (item.Video != null && item.Video.Any())
            {
                var videos = new List<Video>();
                foreach (var video in item.Video)
                {
                    videos.Add(ConvertFromListxml(video));
                }
                machine[Machine.VideoKey] = videos.ToArray();
            }

            if (item.Sound != null)
                machine[Machine.SoundKey] = ConvertFromListxml(item.Sound);

            if (item.Input != null)
                machine[Machine.InputKey] = ConvertFromListxml(item.Input);

            if (item.DipSwitch != null && item.DipSwitch.Any())
            {
                var dipSwitches = new List<DipSwitch>();
                foreach (var dipSwitch in item.DipSwitch)
                {
                    dipSwitches.Add(ConvertFromListxml(dipSwitch));
                }
                machine[Machine.DipSwitchKey] = dipSwitches.ToArray();
            }

            if (item.Configuration != null && item.Configuration.Any())
            {
                var configurations = new List<Configuration>();
                foreach (var configuration in item.Configuration)
                {
                    configurations.Add(ConvertFromListxml(configuration));
                }
                machine[Machine.ConfigurationKey] = configurations.ToArray();
            }

            if (item.Port != null && item.Port.Any())
            {
                var ports = new List<Port>();
                foreach (var port in item.Port)
                {
                    ports.Add(ConvertFromListxml(port));
                }
                machine[Machine.PortKey] = ports.ToArray();
            }

            if (item.Adjuster != null && item.Adjuster.Any())
            {
                var adjusters = new List<Adjuster>();
                foreach (var adjuster in item.Adjuster)
                {
                    adjusters.Add(ConvertFromListxml(adjuster));
                }
                machine[Machine.AdjusterKey] = adjusters.ToArray();
            }

            if (item.Driver != null)
                machine[Machine.DriverKey] = ConvertFromListxml(item.Driver);

            if (item.Feature != null && item.Feature.Any())
            {
                var features = new List<Feature>();
                foreach (var feature in item.Feature)
                {
                    features.Add(ConvertFromListxml(feature));
                }
                machine[Machine.FeatureKey] = features.ToArray();
            }

            if (item.Device != null && item.Device.Any())
            {
                var devices = new List<Device>();
                foreach (var device in item.Device)
                {
                    devices.Add(ConvertFromListxml(device));
                }
                machine[Machine.DeviceKey] = devices.ToArray();
            }

            if (item.Slot != null && item.Slot.Any())
            {
                var slots = new List<Slot>();
                foreach (var slot in item.Slot)
                {
                    slots.Add(ConvertFromListxml(slot));
                }
                machine[Machine.SlotKey] = slots.ToArray();
            }

            if (item.SoftwareList != null && item.SoftwareList.Any())
            {
                var softwareLists = new List<SoftwareList>();
                foreach (var softwareList in item.SoftwareList)
                {
                    softwareLists.Add(ConvertFromListxml(softwareList));
                }
                machine[Machine.SoftwareListKey] = softwareLists.ToArray();
            }

            if (item.RamOption != null && item.RamOption.Any())
            {
                var ramOptions = new List<RamOption>();
                foreach (var ramOption in item.RamOption)
                {
                    ramOptions.Add(ConvertFromListxml(ramOption));
                }
                machine[Machine.RamOptionKey] = ramOptions.ToArray();
            }

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Adjuster"/> to <cref="Adjuster"/>
        /// </summary>
        public static Adjuster ConvertFromListxml(Models.Listxml.Adjuster item)
        {
            var adjuster = new Adjuster
            {
                [Adjuster.NameKey] = item.Name,
                [Adjuster.DefaultKey] = item.Default,
            };

            if (item.Condition != null)
                adjuster[Adjuster.ConditionKey] = ConvertFromListxml(item.Condition);

            return adjuster;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Analog"/> to <cref="Analog"/>
        /// </summary>
        public static Analog ConvertFromListxml(Models.Listxml.Analog item)
        {
            var analog = new Analog
            {
                [Analog.MaskKey] = item.Mask,
            };
            return analog;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.BiosSet"/> to <cref="BiosSet"/>
        /// </summary>
        public static BiosSet ConvertFromListxml(Models.Listxml.BiosSet item)
        {
            var biosset = new BiosSet
            {
                [BiosSet.NameKey] = item.Name,
                [BiosSet.DescriptionKey] = item.Description,
                [BiosSet.DefaultKey] = item.Default,
            };
            return biosset;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Chip"/> to <cref="Chip"/>
        /// </summary>
        public static Chip ConvertFromListxml(Models.Listxml.Chip item)
        {
            var chip = new Chip
            {
                [Chip.NameKey] = item.Name,
                [Chip.TagKey] = item.Tag,
                [Chip.TypeKey] = item.Type,
                [Chip.SoundOnlyKey] = item.SoundOnly,
                [Chip.ClockKey] = item.Clock,
            };
            return chip;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Condition"/> to <cref="Condition"/>
        /// </summary>
        public static Condition ConvertFromListxml(Models.Listxml.Condition item)
        {
            var condition = new Condition
            {
                [Condition.TagKey] = item.Tag,
                [Condition.MaskKey] = item.Mask,
                [Condition.RelationKey] = item.Relation,
                [Condition.ValueKey] = item.Value,
            };
            return condition;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Configuration"/> to <cref="Configuration"/>
        /// </summary>
        public static Configuration ConvertFromListxml(Models.Listxml.Configuration item)
        {
            var configuration = new Configuration
            {
                [Configuration.NameKey] = item.Name,
                [Configuration.TagKey] = item.Tag,
                [Configuration.MaskKey] = item.Mask,
            };

            if (item.Condition != null)
                configuration[Configuration.ConditionKey] = ConvertFromListxml(item.Condition);

            if (item.ConfLocation != null && item.ConfLocation.Any())
            {
                var confLocations = new List<ConfLocation>();
                foreach (var confLocation in item.ConfLocation)
                {
                    confLocations.Add(ConvertFromListxml(confLocation));
                }
                configuration[Configuration.ConfLocationKey] = confLocations.ToArray();
            }

            if (item.ConfSetting != null && item.ConfSetting.Any())
            {
                var confSettings = new List<ConfSetting>();
                foreach (var confSetting in item.ConfSetting)
                {
                    confSettings.Add(ConvertFromListxml(confSetting));
                }
                configuration[Configuration.ConfSettingKey] = confSettings.ToArray();
            }

            return configuration;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.ConfLocation"/> to <cref="ConfLocation"/>
        /// </summary>
        public static ConfLocation ConvertFromListxml(Models.Listxml.ConfLocation item)
        {
            var confLocation = new ConfLocation
            {
                [ConfLocation.NameKey] = item.Name,
                [ConfLocation.NumberKey] = item.Number,
                [ConfLocation.InvertedKey] = item.Inverted,
            };
            return confLocation;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.ConfSetting"/> to <cref="ConfSetting"/>
        /// </summary>
        public static ConfSetting ConvertFromListxml(Models.Listxml.ConfSetting item)
        {
            var confSetting = new ConfSetting
            {
                [ConfSetting.NameKey] = item.Name,
                [ConfSetting.ValueKey] = item.Value,
                [ConfSetting.DefaultKey] = item.Default,
            };

            if (item.Condition != null)
                confSetting[ConfSetting.ConditionKey] = ConvertFromListxml(item.Condition);

            return confSetting;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Control"/> to <cref="Control"/>
        /// </summary>
        public static Control ConvertFromListxml(Models.Listxml.Control item)
        {
            var control = new Control
            {
                [Control.TypeKey] = item.Type,
                [Control.PlayerKey] = item.Player,
                [Control.ButtonsKey] = item.Buttons,
                [Control.ReqButtonsKey] = item.ReqButtons,
                [Control.MinimumKey] = item.Minimum,
                [Control.MaximumKey] = item.Maximum,
                [Control.SensitivityKey] = item.Sensitivity,
                [Control.KeyDeltaKey] = item.KeyDelta,
                [Control.ReverseKey] = item.Reverse,
                [Control.WaysKey] = item.Ways,
                [Control.Ways2Key] = item.Ways2,
                [Control.Ways3Key] = item.Ways3,
            };
            return control;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Device"/> to <cref="Device"/>
        /// </summary>
        public static Device ConvertFromListxml(Models.Listxml.Device item)
        {
            var device = new Device
            {
                [Device.TypeKey] = item.Type,
                [Device.TagKey] = item.Tag,
                [Device.FixedImageKey] = item.FixedImage,
                [Device.MandatoryKey] = item.Mandatory,
                [Device.InterfaceKey] = item.Interface,
            };

            if (item.Instance != null)
                device[Device.InstanceKey] = ConvertFromListxml(item.Instance);

            if (item.Extension != null && item.Extension.Any())
            {
                var extensions = new List<Extension>();
                foreach (var extension in item.Extension)
                {
                    extensions.Add(ConvertFromListxml(extension));
                }
                device[Device.ExtensionKey] = extensions.ToArray();
            }

            return device;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.DeviceRef"/> to <cref="DeviceRef"/>
        /// </summary>
        public static DeviceRef ConvertFromListxml(Models.Listxml.DeviceRef item)
        {
            var deviceRef = new DeviceRef
            {
                [DeviceRef.NameKey] = item.Name,
            };
            return deviceRef;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.DipLocation"/> to <cref="DipLocation"/>
        /// </summary>
        public static DipLocation ConvertFromListxml(Models.Listxml.DipLocation item)
        {
            var dipLocation = new DipLocation
            {
                [DipLocation.NameKey] = item.Name,
                [DipLocation.NumberKey] = item.Number,
                [DipLocation.InvertedKey] = item.Inverted,
            };
            return dipLocation;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.DipSwitch"/> to <cref="DipSwitch"/>
        /// </summary>
        public static DipSwitch ConvertFromListxml(Models.Listxml.DipSwitch item)
        {
            var dipSwitch = new DipSwitch
            {
                [DipSwitch.NameKey] = item.Name,
                [DipSwitch.TagKey] = item.Tag,
                [DipSwitch.MaskKey] = item.Mask,
            };

            if (item.Condition != null)
                dipSwitch[DipSwitch.ConditionKey] = ConvertFromListxml(item.Condition);

            if (item.DipLocation != null && item.DipLocation.Any())
            {
                var dipLocations = new List<DipLocation>();
                foreach (var dipLocation in item.DipLocation)
                {
                    dipLocations.Add(ConvertFromListxml(dipLocation));
                }
                dipSwitch[DipSwitch.DipLocationKey] = dipLocations.ToArray();
            }

            if (item.DipValue != null && item.DipValue.Any())
            {
                var dipValues = new List<DipValue>();
                foreach (var dipValue in item.DipValue)
                {
                    dipValues.Add(ConvertFromListxml(dipValue));
                }
                dipSwitch[DipSwitch.DipValueKey] = dipValues.ToArray();
            }

            return dipSwitch;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.DipValue"/> to <cref="DipValue"/>
        /// </summary>
        public static DipValue ConvertFromListxml(Models.Listxml.DipValue item)
        {
            var dipValue = new DipValue
            {
                [DipValue.NameKey] = item.Name,
                [DipValue.ValueKey] = item.Value,
                [DipValue.DefaultKey] = item.Default,
            };

            if (item.Condition != null)
                dipValue[DipValue.ConditionKey] = ConvertFromListxml(item.Condition);

            return dipValue;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Disk"/> to <cref="Disk"/>
        /// </summary>
        public static Disk ConvertFromListxml(Models.Listxml.Disk item)
        {
            var disk = new Disk
            {
                [Disk.NameKey] = item.Name,
                [Disk.MD5Key] = item.MD5,
                [Disk.SHA1Key] = item.SHA1,
                [Disk.MergeKey] = item.Merge,
                [Disk.RegionKey] = item.Region,
                [Disk.IndexKey] = item.Index,
                [Disk.WritableKey] = item.Writable,
                [Disk.StatusKey] = item.Status,
                [Disk.OptionalKey] = item.Optional,
            };
            return disk;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Display"/> to <cref="Display"/>
        /// </summary>
        public static Display ConvertFromListxml(Models.Listxml.Display item)
        {
            var display = new Display
            {
                [Display.TagKey] = item.Tag,
                [Display.TypeKey] = item.Type,
                [Display.RotateKey] = item.Rotate,
                [Display.FlipXKey] = item.FlipX,
                [Display.WidthKey] = item.Width,
                [Display.HeightKey] = item.Height,
                [Display.RefreshKey] = item.Refresh,
                [Display.PixClockKey] = item.PixClock,
                [Display.HTotalKey] = item.HTotal,
                [Display.HBEndKey] = item.HBEnd,
                [Display.HBStartKey] = item.HBStart,
                [Display.VTotalKey] = item.VTotal,
                [Display.VBEndKey] = item.VBEnd,
                [Display.VBStartKey] = item.VBStart,
            };
            return display;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Driver"/> to <cref="Driver"/>
        /// </summary>
        public static Driver ConvertFromListxml(Models.Listxml.Driver item)
        {
            var driver = new Driver
            {
                [Driver.StatusKey] = item.Status,
                [Driver.ColorKey] = item.Color,
                [Driver.SoundKey] = item.Sound,
                [Driver.PaletteSizeKey] = item.PaletteSize,
                [Driver.EmulationKey] = item.Emulation,
                [Driver.CocktailKey] = item.Cocktail,
                [Driver.SaveStateKey] = item.SaveState,
                [Driver.RequiresArtworkKey] = item.RequiresArtwork,
                [Driver.UnofficialKey] = item.Unofficial,
                [Driver.NoSoundHardwareKey] = item.NoSoundHardware,
                [Driver.IncompleteKey] = item.Incomplete,
            };
            return driver;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Extension"/> to <cref="Extension"/>
        /// </summary>
        public static Extension ConvertFromListxml(Models.Listxml.Extension item)
        {
            var extension = new Extension
            {
                [Extension.NameKey] = item.Name,
            };
            return extension;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Feature"/> to <cref="Feature"/>
        /// </summary>
        public static Feature ConvertFromListxml(Models.Listxml.Feature item)
        {
            var feature = new Feature
            {
                [Feature.TypeKey] = item.Type,
                [Feature.StatusKey] = item.Status,
                [Feature.OverallKey] = item.Overall,
            };
            return feature;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Input"/> to <cref="Input"/>
        /// </summary>
        public static Input ConvertFromListxml(Models.Listxml.Input item)
        {
            var input = new Input
            {
                [Input.ServiceKey] = item.Service,
                [Input.TiltKey] = item.Tilt,
                [Input.PlayersKey] = item.Players,
                [Input.ControlKey] = item.ControlAttr,
                [Input.ButtonsKey] = item.Buttons,
                [Input.CoinsKey] = item.Coins,
            };

            if (item.Control != null && item.Control.Any())
            {
                var controls = new List<Control>();
                foreach (var control in item.Control)
                {
                    controls.Add(ConvertFromListxml(control));
                }
                input[Input.ControlKey] = controls.ToArray();
            }

            return input;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Instance"/> to <cref="Instance"/>
        /// </summary>
        public static Instance ConvertFromListxml(Models.Listxml.Instance item)
        {
            var instance = new Instance
            {
                [Instance.NameKey] = item.Name,
                [Instance.BriefNameKey] = item.BriefName,
            };
            return instance;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Port"/> to <cref="Port"/>
        /// </summary>
        public static Port ConvertFromListxml(Models.Listxml.Port item)
        {
            var port = new Port
            {
                [Port.TagKey] = item.Tag,
            };

            if (item.Analog != null && item.Analog.Any())
            {
                var analogs = new List<Analog>();
                foreach (var analog in item.Analog)
                {
                    analogs.Add(ConvertFromListxml(analog));
                }
                port[Port.AnalogKey] = analogs.ToArray();
            }

            return port;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.RamOption"/> to <cref="RamOption"/>
        /// </summary>
        public static RamOption ConvertFromListxml(Models.Listxml.RamOption item)
        {
            var ramOption = new RamOption
            {
                [RamOption.NameKey] = item.Name,
                [RamOption.DefaultKey] = item.Default,
            };
            return ramOption;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Rom"/> to <cref="Rom"/>
        /// </summary>
        public static Rom ConvertFromListxml(Models.Listxml.Rom item)
        {
            var rom = new Rom
            {
                [Rom.NameKey] = item.Name,
                [Rom.BiosKey] = item.Bios,
                [Rom.SizeKey] = item.Size,
                [Rom.CRCKey] = item.CRC,
                [Rom.SHA1Key] = item.SHA1,
                [Rom.MergeKey] = item.Merge,
                [Rom.RegionKey] = item.Region,
                [Rom.OffsetKey] = item.Offset,
                [Rom.StatusKey] = item.Status,
                [Rom.OptionalKey] = item.Optional,
                [Rom.DisposeKey] = item.Dispose,
                [Rom.SoundOnlyKey] = item.SoundOnly,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Sample"/> to <cref="Sample"/>
        /// </summary>
        public static Sample ConvertFromListxml(Models.Listxml.Sample item)
        {
            var sample = new Sample
            {
                [Sample.NameKey] = item.Name,
            };
            return sample;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Slot"/> to <cref="Slot"/>
        /// </summary>
        public static Slot ConvertFromListxml(Models.Listxml.Slot item)
        {
            var slot = new Slot
            {
                [Slot.NameKey] = item.Name,
            };

            if (item.SlotOption != null && item.SlotOption.Any())
            {
                var slotOptions = new List<SlotOption>();
                foreach (var slotOption in item.SlotOption)
                {
                    slotOptions.Add(ConvertFromListxml(slotOption));
                }
                slot[Slot.SlotOptionKey] = slotOptions.ToArray();
            }

            return slot;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.SlotOption"/> to <cref="SlotOption"/>
        /// </summary>
        public static SlotOption ConvertFromListxml(Models.Listxml.SlotOption item)
        {
            var slotOption = new SlotOption
            {
                [SlotOption.NameKey] = item.Name,
                [SlotOption.DevNameKey] = item.DevName,
                [SlotOption.DefaultKey] = item.Default,
            };
            return slotOption;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.SoftwareList"/> to <cref="SoftwareList"/>
        /// </summary>
        public static SoftwareList ConvertFromListxml(Models.Listxml.SoftwareList item)
        {
            var softwareList = new SoftwareList
            {
                [SoftwareList.TagKey] = item.Tag,
                [SoftwareList.NameKey] = item.Name,
                [SoftwareList.StatusKey] = item.Status,
                [SoftwareList.FilterKey] = item.Filter,
            };
            return softwareList;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Sound"/> to <cref="Sound"/>
        /// </summary>
        public static Sound ConvertFromListxml(Models.Listxml.Sound item)
        {
            var sound = new Sound
            {
                [Sound.ChannelsKey] = item.Channels,
            };
            return sound;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Video"/> to <cref="Video"/>
        /// </summary>
        public static Video ConvertFromListxml(Models.Listxml.Video item)
        {
            var video = new Video
            {
                [Video.ScreenKey] = item.Screen,
                [Video.OrientationKey] = item.Orientation,
                [Video.WidthKey] = item.Width,
                [Video.HeightKey] = item.Height,
                [Video.AspectXKey] = item.AspectX,
                [Video.AspectYKey] = item.AspectY,
                [Video.RefreshKey] = item.Refresh,
            };
            return video;
        }

        #endregion

        #region Deserialize

        /// <summary>
        /// Convert from <cref="Header"/> to <cref="Models.Listxml.M1"/>
        /// </summary>
        public static Models.Listxml.M1? ConvertHeaderToListxmlM1(Header? item)
        {
            if (item == null)
                return null;

            var m1 = new Models.Listxml.M1
            {
                Version = item.ReadString(Header.VersionKey),
            };
            return m1;
        }

        /// <summary>
        /// Convert from <cref="Header"/> to <cref="Models.Listxml.Mame"/>
        /// </summary>
        public static Models.Listxml.Mame? ConvertHeaderToListxmlMame(Header? item)
        {
            if (item == null)
                return null;

            var mame = new Models.Listxml.Mame
            {
                Build = item.ReadString(Header.BuildKey),
                Debug = item.ReadString(Header.DebugKey),
                MameConfig = item.ReadString(Header.MameConfigKey),
            };
            return mame;
        }

        /// <summary>
        /// Convert from <cref="Machine"/> to <cref="Models.Listxml.GameBase"/>
        /// </summary>
        public static Models.Listxml.GameBase? ConvertMachineToListxml(Machine? item)
        {
            if (item == null)
                return null;

            var machine = new Models.Listxml.Machine
            {
                Name = item.ReadString(Machine.NameKey),
                SourceFile = item.ReadString(Machine.SourceFileKey),
                IsBios = item.ReadString(Machine.IsBiosKey),
                IsDevice = item.ReadString(Machine.IsDeviceKey),
                IsMechanical = item.ReadString(Machine.IsMechanicalKey),
                Runnable = item.ReadString(Machine.RunnableKey),
                CloneOf = item.ReadString(Machine.CloneOfKey),
                RomOf = item.ReadString(Machine.RomOfKey),
                SampleOf = item.ReadString(Machine.SampleOfKey),
                Description = item.ReadString(Machine.DescriptionKey),
                Year = item.ReadString(Machine.YearKey),
                Manufacturer = item.ReadString(Machine.ManufacturerKey),
                History = item.ReadString(Machine.HistoryKey),
            };

            var biosSets = item.Read<BiosSet[]>(Machine.BiosSetKey);
            machine.BiosSet = biosSets?.Select(ConvertToListxml)?.ToArray();

            var roms = item.Read<Rom[]>(Machine.RomKey);
            machine.Rom = roms?.Select(ConvertToListxml)?.ToArray();

            var disks = item.Read<Disk[]>(Machine.DiskKey);
            machine.Disk = disks?.Select(ConvertToListxml)?.ToArray();

            var deviceRefs = item.Read<DeviceRef[]>(Machine.DeviceRefKey);
            machine.DeviceRef = deviceRefs?.Select(ConvertToListxml)?.ToArray();

            var samples = item.Read<Sample[]>(Machine.SampleKey);
            machine.Sample = samples?.Select(ConvertToListxml)?.ToArray();

            var chips = item.Read<Chip[]>(Machine.ChipKey);
            machine.Chip = chips?.Select(ConvertToListxml)?.ToArray();

            var displays = item.Read<Display[]>(Machine.DisplayKey);
            machine.Display = displays?.Select(ConvertToListxml)?.ToArray();

            var videos = item.Read<Video[]>(Machine.VideoKey);
            machine.Video = videos?.Select(ConvertToListxml)?.ToArray();

            var sound = item.Read<Sound>(Machine.SoundKey);
            machine.Sound = ConvertToListxml(sound);

            var input = item.Read<Input>(Machine.InputKey);
            machine.Input = ConvertToListxml(input);

            var dipSwitches = item.Read<DipSwitch[]>(Machine.DipSwitchKey);
            machine.DipSwitch = dipSwitches?.Select(ConvertToListxml)?.ToArray();

            var configurations = item.Read<Configuration[]>(Machine.ConfigurationKey);
            machine.Configuration = configurations?.Select(ConvertToListxml)?.ToArray();

            var ports = item.Read<Port[]>(Machine.PortKey);
            machine.Port = ports?.Select(ConvertToListxml)?.ToArray();

            var adjusters = item.Read<Adjuster[]>(Machine.AdjusterKey);
            machine.Adjuster = adjusters?.Select(ConvertToListxml)?.ToArray();

            var driver = item.Read<Driver>(Machine.DriverKey);
            machine.Driver = ConvertToListxml(driver);

            var features = item.Read<Feature[]>(Machine.FeatureKey);
            machine.Feature = features?.Select(ConvertToListxml)?.ToArray();

            var devices = item.Read<Device[]>(Machine.DeviceKey);
            machine.Device = devices?.Select(ConvertToListxml)?.ToArray();

            var slots = item.Read<Slot[]>(Machine.SlotKey);
            machine.Slot = slots?.Select(ConvertToListxml)?.ToArray();

            var softwareLists = item.Read<SoftwareList[]>(Machine.SoftwareListKey);
            machine.SoftwareList = softwareLists?.Select(ConvertToListxml)?.ToArray();

            var ramOptions = item.Read<RamOption[]>(Machine.RamOptionKey);
            machine.RamOption = ramOptions?.Select(ConvertToListxml)?.ToArray();

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Adjuster"/> to <cref="Models.Listxml.Adjuster"/>
        /// </summary>
        private static Models.Listxml.Adjuster? ConvertToListxml(Adjuster? item)
        {
            if (item == null)
                return null;

            var adjuster = new Models.Listxml.Adjuster
            {
                Name = item.ReadString(Adjuster.NameKey),
                Default = item.ReadString(Adjuster.DefaultKey),
            };

            var condition = item.Read<Condition>(Adjuster.ConditionKey);
            adjuster.Condition = ConvertToListxml(condition);

            return adjuster;
        }

        /// <summary>
        /// Convert from <cref="Analog"/> to <cref="Models.Listxml.Analog"/>
        /// </summary>
        private static Models.Listxml.Analog? ConvertToListxml(Analog? item)
        {
            if (item == null)
                return null;

            var analog = new Models.Listxml.Analog
            {
                Mask = item.ReadString(Analog.MaskKey),
            };
            return analog;
        }

        /// <summary>
        /// Convert from <cref="BiosSet"/> to <cref="Models.Listxml.BiosSet"/>
        /// </summary>
        private static Models.Listxml.BiosSet? ConvertToListxml(BiosSet? item)
        {
            if (item == null)
                return null;

            var biosset = new Models.Listxml.BiosSet
            {
                Name = item.ReadString(BiosSet.NameKey),
                Description = item.ReadString(BiosSet.DescriptionKey),
                Default = item.ReadString(BiosSet.DefaultKey),
            };
            return biosset;
        }

        /// <summary>
        /// Convert from <cref="Chip"/> to <cref="Models.Listxml.Chip"/>
        /// </summary>
        private static Models.Listxml.Chip? ConvertToListxml(Chip? item)
        {
            if (item == null)
                return null;

            var chip = new Models.Listxml.Chip
            {
                Name = item.ReadString(Chip.NameKey),
                Tag = item.ReadString(Chip.TagKey),
                Type = item.ReadString(Chip.TypeKey),
                SoundOnly = item.ReadString(Chip.SoundOnlyKey),
                Clock = item.ReadString(Chip.ClockKey),
            };
            return chip;
        }

        /// <summary>
        /// Convert from <cref="Condition"/> to <cref="Models.Listxml.Condition"/>
        /// </summary>
        private static Models.Listxml.Condition? ConvertToListxml(Condition? item)
        {
            if (item == null)
                return null;

            var condition = new Models.Listxml.Condition
            {
                Tag = item.ReadString(Condition.TagKey),
                Mask = item.ReadString(Condition.MaskKey),
                Relation = item.ReadString(Condition.RelationKey),
                Value = item.ReadString(Condition.ValueKey),
            };
            return condition;
        }

        /// <summary>
        /// Convert from <cref="Configuration"/> to <cref="Models.Listxml.Configuration"/>
        /// </summary>
        private static Models.Listxml.Configuration? ConvertToListxml(Configuration? item)
        {
            if (item == null)
                return null;

            var configuration = new Models.Listxml.Configuration
            {
                Name = item.ReadString(Configuration.NameKey),
                Tag = item.ReadString(Configuration.TagKey),
                Mask = item.ReadString(Configuration.MaskKey),
            };

            var condition = item.Read<Condition>(Configuration.ConditionKey);
            configuration.Condition = ConvertToListxml(condition);

            var confLocations = item.Read<ConfLocation[]>(Configuration.ConfLocationKey);
            configuration.ConfLocation = confLocations?.Select(ConvertToListxml)?.ToArray();

            var confSettings = item.Read<ConfSetting[]>(Configuration.ConfSettingKey);
            configuration.ConfSetting = confSettings?.Select(ConvertToListxml)?.ToArray();

            return configuration;
        }

        /// <summary>
        /// Convert from <cref="ConfLocation"/> to <cref="Models.Listxml.ConfLocation"/>
        /// </summary>
        private static Models.Listxml.ConfLocation? ConvertToListxml(ConfLocation? item)
        {
            if (item == null)
                return null;

            var confLocation = new Models.Listxml.ConfLocation
            {
                Name = item.ReadString(ConfLocation.NameKey),
                Number = item.ReadString(ConfLocation.NumberKey),
                Inverted = item.ReadString(ConfLocation.InvertedKey),
            };
            return confLocation;
        }

        /// <summary>
        /// Convert from <cref="ConfSetting"/> to <cref="Models.Listxml.ConfSetting"/>
        /// </summary>
        private static Models.Listxml.ConfSetting? ConvertToListxml(ConfSetting? item)
        {
            if (item == null)
                return null;

            var confSetting = new Models.Listxml.ConfSetting
            {
                Name = item.ReadString(ConfSetting.NameKey),
                Value = item.ReadString(ConfSetting.ValueKey),
                Default = item.ReadString(ConfSetting.DefaultKey),
            };

            var condition = item.Read<Condition>(ConfSetting.ConditionKey);
            confSetting.Condition = ConvertToListxml(condition);

            return confSetting;
        }

        /// <summary>
        /// Convert from <cref="Control"/> to <cref="Models.Listxml.Control"/>
        /// </summary>
        private static Models.Listxml.Control? ConvertToListxml(Control? item)
        {
            if (item == null)
                return null;

            var control = new Models.Listxml.Control
            {
                Type = item.ReadString(Control.TypeKey),
                Player = item.ReadString(Control.PlayerKey),
                Buttons = item.ReadString(Control.ButtonsKey),
                ReqButtons = item.ReadString(Control.ReqButtonsKey),
                Minimum = item.ReadString(Control.MinimumKey),
                Maximum = item.ReadString(Control.MaximumKey),
                Sensitivity = item.ReadString(Control.SensitivityKey),
                KeyDelta = item.ReadString(Control.KeyDeltaKey),
                Reverse = item.ReadString(Control.ReverseKey),
                Ways = item.ReadString(Control.WaysKey),
                Ways2 = item.ReadString(Control.Ways2Key),
                Ways3 = item.ReadString(Control.Ways3Key),
            };
            return control;
        }

        /// <summary>
        /// Convert from <cref="Device"/> to <cref="Models.Listxml.Device"/>
        /// </summary>
        private static Models.Listxml.Device? ConvertToListxml(Device? item)
        {
            if (item == null)
                return null;

            var device = new Models.Listxml.Device
            {
                Type = item.ReadString(Device.TypeKey),
                Tag = item.ReadString(Device.TagKey),
                FixedImage = item.ReadString(Device.FixedImageKey),
                Mandatory = item.ReadString(Device.MandatoryKey),
                Interface = item.ReadString(Device.InterfaceKey),
            };

            var instance = item.Read<Instance>(Device.InstanceKey);
            device.Instance = ConvertToListxml(instance);

            var extensions = item.Read<Extension[]>(Device.ExtensionKey);
            device.Extension = extensions?.Select(ConvertToListxml)?.ToArray();

            return device;
        }

        /// <summary>
        /// Convert from <cref="DeviceRef"/> to <cref="Models.Listxml.DeviceRef"/>
        /// </summary>
        private static Models.Listxml.DeviceRef? ConvertToListxml(DeviceRef? item)
        {
            if (item == null)
                return null;

            var deviceRef = new Models.Listxml.DeviceRef
            {
                Name = item.ReadString(DeviceRef.NameKey),
            };
            return deviceRef;
        }

        /// <summary>
        /// Convert from <cref="DipLocation"/> to <cref="Models.Listxml.DipLocation"/>
        /// </summary>
        private static Models.Listxml.DipLocation? ConvertToListxml(DipLocation? item)
        {
            if (item == null)
                return null;

            var dipLocation = new Models.Listxml.DipLocation
            {
                Name = item.ReadString(DipLocation.NameKey),
                Number = item.ReadString(DipLocation.NumberKey),
                Inverted = item.ReadString(DipLocation.InvertedKey),
            };
            return dipLocation;
        }

        /// <summary>
        /// Convert from <cref="DipSwitch"/> to <cref="Models.Listxml.DipSwitch"/>
        /// </summary>
        private static Models.Listxml.DipSwitch? ConvertToListxml(DipSwitch? item)
        {
            if (item == null)
                return null;

            var dipSwitch = new Models.Listxml.DipSwitch
            {
                Name = item.ReadString(DipSwitch.NameKey),
                Tag = item.ReadString(DipSwitch.TagKey),
                Mask = item.ReadString(DipSwitch.MaskKey),
            };

            var condition = item.Read<Condition>(DipSwitch.ConditionKey);
            dipSwitch.Condition = ConvertToListxml(condition);

            var dipLocations = item.Read<DipLocation[]>(DipSwitch.DipLocationKey);
            dipSwitch.DipLocation = dipLocations?.Select(ConvertToListxml)?.ToArray();

            var dipValues = item.Read<DipValue[]>(DipSwitch.DipValueKey);
            dipSwitch.DipValue = dipValues?.Select(ConvertToListxml)?.ToArray();

            return dipSwitch;
        }

        /// <summary>
        /// Convert from <cref="DipValue"/> to <cref="Models.Listxml.DipValue"/>
        /// </summary>
        private static Models.Listxml.DipValue? ConvertToListxml(DipValue? item)
        {
            if (item == null)
                return null;

            var dipValue = new Models.Listxml.DipValue
            {
                Name = item.ReadString(DipValue.NameKey),
                Value = item.ReadString(DipValue.ValueKey),
                Default = item.ReadString(DipValue.DefaultKey),
            };

            var condition = item.Read<Condition>(DipValue.ConditionKey);
            dipValue.Condition = ConvertToListxml(condition);

            return dipValue;
        }

        /// <summary>
        /// Convert from <cref="Disk"/> to <cref="Models.Listxml.Disk"/>
        /// </summary>
        private static Models.Listxml.Disk? ConvertToListxml(Disk? item)
        {
            if (item == null)
                return null;

            var disk = new Models.Listxml.Disk
            {
                Name = item.ReadString(Disk.NameKey),
                MD5 = item.ReadString(Disk.MD5Key),
                SHA1 = item.ReadString(Disk.SHA1Key),
                Merge = item.ReadString(Disk.MergeKey),
                Region = item.ReadString(Disk.RegionKey),
                Index = item.ReadString(Disk.IndexKey),
                Writable = item.ReadString(Disk.WritableKey),
                Status = item.ReadString(Disk.StatusKey),
                Optional = item.ReadString(Disk.OptionalKey),
            };
            return disk;
        }

        /// <summary>
        /// Convert from <cref="Display"/> to <cref="Models.Listxml.Display"/>
        /// </summary>
        private static Models.Listxml.Display? ConvertToListxml(Display? item)
        {
            if (item == null)
                return null;

            var display = new Models.Listxml.Display
            {
                Tag = item.ReadString(Display.TagKey),
                Type = item.ReadString(Display.TypeKey),
                Rotate = item.ReadString(Display.RotateKey),
                FlipX = item.ReadString(Display.FlipXKey),
                Width = item.ReadString(Display.WidthKey),
                Height = item.ReadString(Display.HeightKey),
                Refresh = item.ReadString(Display.RefreshKey),
                PixClock = item.ReadString(Display.PixClockKey),
                HTotal = item.ReadString(Display.HTotalKey),
                HBEnd = item.ReadString(Display.HBEndKey),
                HBStart = item.ReadString(Display.HBStartKey),
                VTotal = item.ReadString(Display.VTotalKey),
                VBEnd = item.ReadString(Display.VBEndKey),
                VBStart = item.ReadString(Display.VBStartKey),
            };
            return display;
        }

        /// <summary>
        /// Convert from <cref="Driver"/> to <cref="Models.Listxml.Driver"/>
        /// </summary>
        private static Models.Listxml.Driver? ConvertToListxml(Driver? item)
        {
            if (item == null)
                return null;

            var driver = new Models.Listxml.Driver
            {
                Status = item.ReadString(Driver.StatusKey),
                Color = item.ReadString(Driver.ColorKey),
                Sound = item.ReadString(Driver.SoundKey),
                PaletteSize = item.ReadString(Driver.PaletteSizeKey),
                Emulation = item.ReadString(Driver.EmulationKey),
                Cocktail = item.ReadString(Driver.CocktailKey),
                SaveState = item.ReadString(Driver.SaveStateKey),
                RequiresArtwork = item.ReadString(Driver.RequiresArtworkKey),
                Unofficial = item.ReadString(Driver.UnofficialKey),
                NoSoundHardware = item.ReadString(Driver.NoSoundHardwareKey),
                Incomplete = item.ReadString(Driver.IncompleteKey),
            };
            return driver;
        }

        /// <summary>
        /// Convert from <cref="Extension"/> to <cref="Models.Listxml.Extension"/>
        /// </summary>
        private static Models.Listxml.Extension? ConvertToListxml(Extension? item)
        {
            if (item == null)
                return null;

            var extension = new Models.Listxml.Extension
            {
                Name = item.ReadString(Extension.NameKey),
            };
            return extension;
        }

        /// <summary>
        /// Convert from <cref="Feature"/> to <cref="Models.Listxml.Feature"/>
        /// </summary>
        private static Models.Listxml.Feature? ConvertToListxml(Feature? item)
        {
            if (item == null)
                return null;

            var feature = new Models.Listxml.Feature
            {
                Type = item.ReadString(Feature.TypeKey),
                Status = item.ReadString(Feature.StatusKey),
                Overall = item.ReadString(Feature.OverallKey),
            };
            return feature;
        }

        /// <summary>
        /// Convert from <cref="Input"/> to <cref="Models.Listxml.Input"/>
        /// </summary>
        private static Models.Listxml.Input? ConvertToListxml(Input? item)
        {
            if (item == null)
                return null;

            var input = new Models.Listxml.Input
            {
                Service = item.ReadString(Input.ServiceKey),
                Tilt = item.ReadString(Input.TiltKey),
                Players = item.ReadString(Input.PlayersKey),
                ControlAttr = item.ReadString(Input.ControlKey),
                Buttons = item.ReadString(Input.ButtonsKey),
                Coins = item.ReadString(Input.CoinsKey),
            };

            var controls = item.Read<Control[]>(Input.ControlKey);
            input.Control = controls?.Select(ConvertToListxml)?.ToArray();

            return input;
        }

        /// <summary>
        /// Convert from <cref="Instance"/> to <cref="Models.Listxml.Instance"/>
        /// </summary>
        private static Models.Listxml.Instance? ConvertToListxml(Instance? item)
        {
            if (item == null)
                return null;

            var instance = new Models.Listxml.Instance
            {
                Name = item.ReadString(Instance.NameKey),
                BriefName = item.ReadString(Instance.BriefNameKey),
            };
            return instance;
        }

        /// <summary>
        /// Convert from <cref="Port"/> to <cref="Models.Listxml.Port"/>
        /// </summary>
        private static Models.Listxml.Port? ConvertToListxml(Port? item)
        {
            if (item == null)
                return null;

            var port = new Models.Listxml.Port
            {
                Tag = item.ReadString(Port.TagKey),
            };

            var analogs = item.Read<Analog[]>(Port.AnalogKey);
            port.Analog = analogs?.Select(ConvertToListxml)?.ToArray();

            return port;
        }

        /// <summary>
        /// Convert from <cref="RamOption"/> to <cref="Models.Listxml.RamOption"/>
        /// </summary>
        private static Models.Listxml.RamOption? ConvertToListxml(RamOption? item)
        {
            if (item == null)
                return null;

            var ramOption = new Models.Listxml.RamOption
            {
                Name = item.ReadString(RamOption.NameKey),
                Default = item.ReadString(RamOption.DefaultKey),
            };
            return ramOption;
        }

        /// <summary>
        /// Convert from <cref="Rom"/> to <cref="Models.Listxml.Rom"/>
        /// </summary>
        private static Models.Listxml.Rom? ConvertToListxml(Rom? item)
        {
            if (item == null)
                return null;

            var rom = new Models.Listxml.Rom
            {
                Name = item.ReadString(Rom.NameKey),
                Bios = item.ReadString(Rom.BiosKey),
                Size = item.ReadString(Rom.SizeKey),
                CRC = item.ReadString(Rom.CRCKey),
                SHA1 = item.ReadString(Rom.SHA1Key),
                Merge = item.ReadString(Rom.MergeKey),
                Region = item.ReadString(Rom.RegionKey),
                Offset = item.ReadString(Rom.OffsetKey),
                Status = item.ReadString(Rom.StatusKey),
                Optional = item.ReadString(Rom.OptionalKey),
                Dispose = item.ReadString(Rom.DisposeKey),
                SoundOnly = item.ReadString(Rom.SoundOnlyKey),
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Sample"/> to <cref="Models.Listxml.Sample"/>
        /// </summary>
        private static Models.Listxml.Sample? ConvertToListxml(Sample? item)
        {
            if (item == null)
                return null;

            var sample = new Models.Listxml.Sample
            {
                Name = item.ReadString(Sample.NameKey),
            };
            return sample;
        }

        /// <summary>
        /// Convert from <cref="Slot"/> to <cref="Models.Listxml.Slot"/>
        /// </summary>
        private static Models.Listxml.Slot? ConvertToListxml(Slot? item)
        {
            if (item == null)
                return null;

            var slot = new Models.Listxml.Slot
            {
                Name = item.ReadString(Slot.NameKey),
            };

            var slotOptions = item.Read<SlotOption[]>(Slot.SlotOptionKey);
            slot.SlotOption = slotOptions?.Select(ConvertToListxml)?.ToArray();

            return slot;
        }

        /// <summary>
        /// Convert from <cref="SlotOption"/> to <cref="Models.Listxml.SlotOption"/>
        /// </summary>
        private static Models.Listxml.SlotOption? ConvertToListxml(SlotOption? item)
        {
            if (item == null)
                return null;

            var slotOption = new Models.Listxml.SlotOption
            {
                Name = item.ReadString(SlotOption.NameKey),
                DevName = item.ReadString(SlotOption.DevNameKey),
                Default = item.ReadString(SlotOption.DefaultKey),
            };
            return slotOption;
        }

        /// <summary>
        /// Convert from <cref="SoftwareList"/> to <cref="Models.Listxml.SoftwareList"/>
        /// </summary>
        private static Models.Listxml.SoftwareList? ConvertToListxml(SoftwareList? item)
        {
            if (item == null)
                return null;

            var softwareList = new Models.Listxml.SoftwareList
            {
                Tag = item.ReadString(SoftwareList.TagKey),
                Name = item.ReadString(SoftwareList.NameKey),
                Status = item.ReadString(SoftwareList.StatusKey),
                Filter = item.ReadString(SoftwareList.FilterKey),
            };
            return softwareList;
        }

        /// <summary>
        /// Convert from <cref="Sound"/> to <cref="Models.Listxml.Sound"/>
        /// </summary>
        private static Models.Listxml.Sound? ConvertToListxml(Sound? item)
        {
            if (item == null)
                return null;

            var sound = new Models.Listxml.Sound
            {
                Channels = item.ReadString(Sound.ChannelsKey),
            };
            return sound;
        }

        /// <summary>
        /// Convert from <cref="Video"/> to <cref="Models.Listxml.Video"/>
        /// </summary>
        private static Models.Listxml.Video? ConvertToListxml(Video? item)
        {
            if (item == null)
                return null;

            var video = new Models.Listxml.Video
            {
                Screen = item.ReadString(Video.ScreenKey),
                Orientation = item.ReadString(Video.OrientationKey),
                Width = item.ReadString(Video.WidthKey),
                Height = item.ReadString(Video.HeightKey),
                AspectX = item.ReadString(Video.AspectXKey),
                AspectY = item.ReadString(Video.AspectYKey),
                Refresh = item.ReadString(Video.RefreshKey),
            };
            return video;
        }

        #endregion
    }
}