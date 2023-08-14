using System.Linq;
using SabreTools.Models.Listxml;

namespace SabreTools.Serialization
{
    /// <summary>
    /// XML deserializer for MAME listxml files
    /// </summary>
    public partial class Listxml : XmlSerializer<Mame>
    {
        #region Internal

        /// <summary>
        /// Convert from <cref="Models.Internal.Models.Internal.MetadataFile"/> to <cref="Models.Listxml.M1"/>
        /// </summary>
        public static M1? ConvertM1FromInternalModel(Models.Internal.MetadataFile? item)
        {
            if (item == null)
                return null;

            var header = item.Read<Models.Internal.Header>(Models.Internal.MetadataFile.HeaderKey);
            var m1 = header != null ? ConvertM1FromInternalModel(header) : new M1();

            var machines = item.Read<Models.Internal.Machine[]>(Models.Internal.MetadataFile.MachineKey);
            if (machines != null && machines.Any())
            {
                m1.Game = machines
                    .Where(m => m != null)
                    .Select(ConvertMachineFromInternalModel)
                    .ToArray();
            }

            return m1;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Models.Internal.MetadataFile"/> to <cref="Models.Listxml.Mame"/>
        /// </summary>
        public static Mame? ConvertMameFromInternalModel(Models.Internal.MetadataFile? item)
        {
            if (item == null)
                return null;

            var header = item.Read<Models.Internal.Header>(Models.Internal.MetadataFile.HeaderKey);
            var mame = header != null ? ConvertMameFromInternalModel(header) : new Mame();

            var machines = item.Read<Models.Internal.Machine[]>(Models.Internal.MetadataFile.MachineKey);
            if (machines != null && machines.Any())
            {
                mame.Game = machines
                    .Where(m => m != null)
                    .Select(ConvertMachineFromInternalModel)
                    .ToArray();
            }

            return mame;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Models.Internal.Header"/> to <cref="Models.Listxml.M1"/>
        /// </summary>
        private static M1 ConvertM1FromInternalModel(Models.Internal.Header item)
        {
            var m1 = new M1
            {
                Version = item.ReadString(Models.Internal.Header.VersionKey),
            };
            return m1;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Models.Internal.Header"/> to <cref="Models.Listxml.Mame"/>
        /// </summary>
        private static Mame ConvertMameFromInternalModel(Models.Internal.Header item)
        {
            var mame = new Mame
            {
                Build = item.ReadString(Models.Internal.Header.BuildKey),
                Debug = item.ReadString(Models.Internal.Header.DebugKey),
                MameConfig = item.ReadString(Models.Internal.Header.MameConfigKey),
            };

            return mame;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Models.Internal.Machine"/> to <cref="Models.Listxml.GameBase"/>
        /// </summary>
        private static GameBase ConvertMachineFromInternalModel(Models.Internal.Machine item)
        {
            var machine = new Machine
            {
                Name = item.ReadString(Models.Internal.Machine.NameKey),
                SourceFile = item.ReadString(Models.Internal.Machine.SourceFileKey),
                IsBios = item.ReadString(Models.Internal.Machine.IsBiosKey),
                IsDevice = item.ReadString(Models.Internal.Machine.IsDeviceKey),
                IsMechanical = item.ReadString(Models.Internal.Machine.IsMechanicalKey),
                Runnable = item.ReadString(Models.Internal.Machine.RunnableKey),
                CloneOf = item.ReadString(Models.Internal.Machine.CloneOfKey),
                RomOf = item.ReadString(Models.Internal.Machine.RomOfKey),
                SampleOf = item.ReadString(Models.Internal.Machine.SampleOfKey),
                Description = item.ReadString(Models.Internal.Machine.DescriptionKey),
                Year = item.ReadString(Models.Internal.Machine.YearKey),
                Manufacturer = item.ReadString(Models.Internal.Machine.ManufacturerKey),
                History = item.ReadString(Models.Internal.Machine.HistoryKey),
            };

            var biosSets = item.Read<Models.Internal.BiosSet[]>(Models.Internal.Machine.BiosSetKey);
            if (biosSets != null && biosSets.Any())
            {
                machine.BiosSet = biosSets
                    .Where(b => b != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var roms = item.Read<Models.Internal.Rom[]>(Models.Internal.Machine.RomKey);
            if (roms != null && roms.Any())
            {
                machine.Rom = roms
                    .Where(r => r != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var disks = item.Read<Models.Internal.Disk[]>(Models.Internal.Machine.DiskKey);
            if (disks != null && disks.Any())
            {
                machine.Disk = disks
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var deviceRefs = item.Read<Models.Internal.DeviceRef[]>(Models.Internal.Machine.DeviceRefKey);
            if (deviceRefs != null && deviceRefs.Any())
            {
                machine.DeviceRef = deviceRefs
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var samples = item.Read<Models.Internal.Sample[]>(Models.Internal.Machine.SampleKey);
            if (samples != null && samples.Any())
            {
                machine.Sample = samples
                    .Where(s => s != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var chips = item.Read<Models.Internal.Chip[]>(Models.Internal.Machine.ChipKey);
            if (chips != null && chips.Any())
            {
                machine.Chip = chips
                    .Where(c => c != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var displays = item.Read<Models.Internal.Display[]>(Models.Internal.Machine.DisplayKey);
            if (displays != null && displays.Any())
            {
                machine.Display = displays
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var videos = item.Read<Models.Internal.Video[]>(Models.Internal.Machine.VideoKey);
            if (videos != null && videos.Any())
            {
                machine.Video = videos
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var sound = item.Read<Models.Internal.Sound>(Models.Internal.Machine.SoundKey);
            if (sound != null)
                machine.Sound = ConvertFromInternalModel(sound);

            var input = item.Read<Models.Internal.Input>(Models.Internal.Machine.InputKey);
            if (input != null)
                machine.Input = ConvertFromInternalModel(input);

            var dipSwitches = item.Read<Models.Internal.DipSwitch[]>(Models.Internal.Machine.DipSwitchKey);
            if (dipSwitches != null && dipSwitches.Any())
            {
                machine.DipSwitch = dipSwitches
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var configurations = item.Read<Models.Internal.Configuration[]>(Models.Internal.Machine.ConfigurationKey);
            if (configurations != null && configurations.Any())
            {
                machine.Configuration = configurations
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var ports = item.Read<Models.Internal.Port[]>(Models.Internal.Machine.PortKey);
            if (ports != null && ports.Any())
            {
                machine.Port = ports
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var adjusters = item.Read<Models.Internal.Adjuster[]>(Models.Internal.Machine.AdjusterKey);
            if (adjusters != null && adjusters.Any())
            {
                machine.Adjuster = adjusters
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var driver = item.Read<Models.Internal.Driver>(Models.Internal.Machine.DriverKey);
            if (driver != null)
                machine.Driver = ConvertFromInternalModel(driver);

            var features = item.Read<Models.Internal.Feature[]>(Models.Internal.Machine.FeatureKey);
            if (features != null && features.Any())
            {
                machine.Feature = features
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var devices = item.Read<Models.Internal.Device[]>(Models.Internal.Machine.DeviceKey);
            if (devices != null && devices.Any())
            {
                machine.Device = devices
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var slots = item.Read<Models.Internal.Slot[]>(Models.Internal.Machine.SlotKey);
            if (slots != null && slots.Any())
            {
                machine.Slot = slots
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var softwareLists = item.Read<Models.Internal.SoftwareList[]>(Models.Internal.Machine.SoftwareListKey);
            if (softwareLists != null && softwareLists.Any())
            {
                machine.SoftwareList = softwareLists
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var ramOptions = item.Read<Models.Internal.RamOption[]>(Models.Internal.Machine.RamOptionKey);
            if (ramOptions != null && ramOptions.Any())
            {
                machine.RamOption = ramOptions
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Models.Internal.Adjuster"/> to <cref="Models.Listxml.Adjuster"/>
        /// </summary>
        private static Adjuster ConvertFromInternalModel(Models.Internal.Adjuster item)
        {
            var adjuster = new Adjuster
            {
                Name = item.ReadString(Models.Internal.Adjuster.NameKey),
                Default = item.ReadString(Models.Internal.Adjuster.DefaultKey),
            };

            var condition = item.Read<Models.Internal.Condition>(Models.Internal.Adjuster.ConditionKey);
            if (condition != null)
                adjuster.Condition = ConvertFromInternalModel(condition);

            return adjuster;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Models.Internal.Analog"/> to <cref="Models.Listxml.Analog"/>
        /// </summary>
        private static Analog ConvertFromInternalModel(Models.Internal.Analog item)
        {
            var analog = new Analog
            {
                Mask = item.ReadString(Models.Internal.Analog.MaskKey),
            };
            return analog;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Models.Internal.BiosSet"/> to <cref="Models.Listxml.BiosSet"/>
        /// </summary>
        private static BiosSet ConvertFromInternalModel(Models.Internal.BiosSet item)
        {
            var biosset = new BiosSet
            {
                Name = item.ReadString(Models.Internal.BiosSet.NameKey),
                Description = item.ReadString(Models.Internal.BiosSet.DescriptionKey),
                Default = item.ReadString(Models.Internal.BiosSet.DefaultKey),
            };
            return biosset;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Models.Internal.Chip"/> to <cref="Models.Listxml.Chip"/>
        /// </summary>
        private static Chip ConvertFromInternalModel(Models.Internal.Chip item)
        {
            var chip = new Chip
            {
                Name = item.ReadString(Models.Internal.Chip.NameKey),
                Tag = item.ReadString(Models.Internal.Chip.TagKey),
                Type = item.ReadString(Models.Internal.Chip.TypeKey),
                SoundOnly = item.ReadString(Models.Internal.Chip.SoundOnlyKey),
                Clock = item.ReadString(Models.Internal.Chip.ClockKey),
            };
            return chip;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Models.Internal.Condition"/> to <cref="Models.Listxml.Condition"/>
        /// </summary>
        private static Condition ConvertFromInternalModel(Models.Internal.Condition item)
        {
            var condition = new Condition
            {
                Tag = item.ReadString(Models.Internal.Condition.TagKey),
                Mask = item.ReadString(Models.Internal.Condition.MaskKey),
                Relation = item.ReadString(Models.Internal.Condition.RelationKey),
                Value = item.ReadString(Models.Internal.Condition.ValueKey),
            };
            return condition;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Models.Internal.Configuration"/> to <cref="Models.Listxml.Configuration"/>
        /// </summary>
        private static Configuration ConvertFromInternalModel(Models.Internal.Configuration item)
        {
            var configuration = new Configuration
            {
                Name = item.ReadString(Models.Internal.Configuration.NameKey),
                Tag = item.ReadString(Models.Internal.Configuration.TagKey),
                Mask = item.ReadString(Models.Internal.Configuration.MaskKey),
            };

            var condition = item.Read<Models.Internal.Condition>(Models.Internal.Configuration.ConditionKey);
            if (condition != null)
                configuration.Condition = ConvertFromInternalModel(condition);

            var confLocations = item.Read<Models.Internal.ConfLocation[]>(Models.Internal.Configuration.ConfLocationKey);
            if (confLocations != null && confLocations.Any())
            {
                configuration.ConfLocation = confLocations
                    .Where(c => c != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var confSettings = item.Read<Models.Internal.ConfSetting[]>(Models.Internal.Configuration.ConfSettingKey);
            if (confSettings != null && confSettings.Any())
            {
                configuration.ConfSetting = confSettings
                    .Where(c => c != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            return configuration;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Models.Internal.ConfLocation"/> to <cref="Models.Listxml.ConfLocation"/>
        /// </summary>
        private static ConfLocation ConvertFromInternalModel(Models.Internal.ConfLocation item)
        {
            var confLocation = new ConfLocation
            {
                Name = item.ReadString(Models.Internal.ConfLocation.NameKey),
                Number = item.ReadString(Models.Internal.ConfLocation.NumberKey),
                Inverted = item.ReadString(Models.Internal.ConfLocation.InvertedKey),
            };
            return confLocation;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Models.Internal.ConfSetting"/> to <cref="Models.Listxml.ConfSetting"/>
        /// </summary>
        private static ConfSetting ConvertFromInternalModel(Models.Internal.ConfSetting item)
        {
            var confSetting = new ConfSetting
            {
                Name = item.ReadString(Models.Internal.ConfSetting.NameKey),
                Value = item.ReadString(Models.Internal.ConfSetting.ValueKey),
                Default = item.ReadString(Models.Internal.ConfSetting.DefaultKey),
            };

            var condition = item.Read<Models.Internal.Condition>(Models.Internal.ConfSetting.ConditionKey);
            if (condition != null)
                confSetting.Condition = ConvertFromInternalModel(condition);

            return confSetting;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Models.Internal.Control"/> to <cref="Models.Listxml.Control"/>
        /// </summary>
        private static Control ConvertFromInternalModel(Models.Internal.Control item)
        {
            var control = new Control
            {
                Type = item.ReadString(Models.Internal.Control.TypeKey),
                Player = item.ReadString(Models.Internal.Control.PlayerKey),
                Buttons = item.ReadString(Models.Internal.Control.ButtonsKey),
                ReqButtons = item.ReadString(Models.Internal.Control.ReqButtonsKey),
                Minimum = item.ReadString(Models.Internal.Control.MinimumKey),
                Maximum = item.ReadString(Models.Internal.Control.MaximumKey),
                Sensitivity = item.ReadString(Models.Internal.Control.SensitivityKey),
                KeyDelta = item.ReadString(Models.Internal.Control.KeyDeltaKey),
                Reverse = item.ReadString(Models.Internal.Control.ReverseKey),
                Ways = item.ReadString(Models.Internal.Control.WaysKey),
                Ways2 = item.ReadString(Models.Internal.Control.Ways2Key),
                Ways3 = item.ReadString(Models.Internal.Control.Ways3Key),
            };
            return control;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Models.Internal.Device"/> to <cref="Models.Listxml.Device"/>
        /// </summary>
        private static Device ConvertFromInternalModel(Models.Internal.Device item)
        {
            var device = new Device
            {
                Type = item.ReadString(Models.Internal.Device.TypeKey),
                Tag = item.ReadString(Models.Internal.Device.TagKey),
                FixedImage = item.ReadString(Models.Internal.Device.FixedImageKey),
                Mandatory = item.ReadString(Models.Internal.Device.MandatoryKey),
                Interface = item.ReadString(Models.Internal.Device.InterfaceKey),
            };

            var instance = item.Read<Models.Internal.Instance>(Models.Internal.Device.InstanceKey);
            if (instance != null)
                device.Instance = ConvertFromInternalModel(instance);

            var extensions = item.Read<Models.Internal.Extension[]>(Models.Internal.Device.ExtensionKey);
            if (extensions != null && extensions.Any())
            {
                device.Extension = extensions
                    .Where(e => e != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            return device;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.DeviceRef"/> to <cref="Models.Listxml.DeviceRef"/>
        /// </summary>
        private static DeviceRef ConvertFromInternalModel(Models.Internal.DeviceRef item)
        {
            var deviceRef = new DeviceRef
            {
                Name = item.ReadString(Models.Internal.DeviceRef.NameKey),
            };
            return deviceRef;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.DipLocation"/> to <cref="Models.Listxml.DipLocation"/>
        /// </summary>
        private static DipLocation ConvertFromInternalModel(Models.Internal.DipLocation item)
        {
            var dipLocation = new DipLocation
            {
                Name = item.ReadString(Models.Internal.DipLocation.NameKey),
                Number = item.ReadString(Models.Internal.DipLocation.NumberKey),
                Inverted = item.ReadString(Models.Internal.DipLocation.InvertedKey),
            };
            return dipLocation;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.DipSwitch"/> to <cref="Models.Listxml.DipSwitch"/>
        /// </summary>
        private static DipSwitch ConvertFromInternalModel(Models.Internal.DipSwitch item)
        {
            var dipSwitch = new DipSwitch
            {
                Name = item.ReadString(Models.Internal.DipSwitch.NameKey),
                Tag = item.ReadString(Models.Internal.DipSwitch.TagKey),
                Mask = item.ReadString(Models.Internal.DipSwitch.MaskKey),
            };

            var condition = item.Read<Models.Internal.Condition>(Models.Internal.DipSwitch.ConditionKey);
            if (condition != null)
                dipSwitch.Condition = ConvertFromInternalModel(condition);

            var dipLocations = item.Read<Models.Internal.DipLocation[]>(Models.Internal.DipSwitch.DipLocationKey);
            if (dipLocations != null && dipLocations.Any())
            {
                dipSwitch.DipLocation = dipLocations
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var dipValues = item.Read<Models.Internal.DipValue[]>(Models.Internal.DipSwitch.DipValueKey);
            if (dipValues != null && dipValues.Any())
            {
                dipSwitch.DipValue = dipValues
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            return dipSwitch;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.DipValue"/> to <cref="Models.Listxml.DipValue"/>
        /// </summary>
        private static DipValue ConvertFromInternalModel(Models.Internal.DipValue item)
        {
            var dipValue = new DipValue
            {
                Name = item.ReadString(Models.Internal.DipValue.NameKey),
                Value = item.ReadString(Models.Internal.DipValue.ValueKey),
                Default = item.ReadString(Models.Internal.DipValue.DefaultKey),
            };

            var condition = item.Read<Models.Internal.Condition>(Models.Internal.DipValue.ConditionKey);
            if (condition != null)
                dipValue.Condition = ConvertFromInternalModel(condition);

            return dipValue;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Disk"/> to <cref="Models.Listxml.Disk"/>
        /// </summary>
        private static Disk ConvertFromInternalModel(Models.Internal.Disk item)
        {
            var disk = new Disk
            {
                Name = item.ReadString(Models.Internal.Disk.NameKey),
                MD5 = item.ReadString(Models.Internal.Disk.MD5Key),
                SHA1 = item.ReadString(Models.Internal.Disk.SHA1Key),
                Merge = item.ReadString(Models.Internal.Disk.MergeKey),
                Region = item.ReadString(Models.Internal.Disk.RegionKey),
                Index = item.ReadString(Models.Internal.Disk.IndexKey),
                Writable = item.ReadString(Models.Internal.Disk.WritableKey),
                Status = item.ReadString(Models.Internal.Disk.StatusKey),
                Optional = item.ReadString(Models.Internal.Disk.OptionalKey),
            };
            return disk;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Display"/> to <cref="Models.Listxml.Display"/>
        /// </summary>
        private static Display ConvertFromInternalModel(Models.Internal.Display item)
        {
            var display = new Display
            {
                Tag = item.ReadString(Models.Internal.Display.TagKey),
                Type = item.ReadString(Models.Internal.Display.TypeKey),
                Rotate = item.ReadString(Models.Internal.Display.RotateKey),
                FlipX = item.ReadString(Models.Internal.Display.FlipXKey),
                Width = item.ReadString(Models.Internal.Display.WidthKey),
                Height = item.ReadString(Models.Internal.Display.HeightKey),
                Refresh = item.ReadString(Models.Internal.Display.RefreshKey),
                PixClock = item.ReadString(Models.Internal.Display.PixClockKey),
                HTotal = item.ReadString(Models.Internal.Display.HTotalKey),
                HBEnd = item.ReadString(Models.Internal.Display.HBEndKey),
                HBStart = item.ReadString(Models.Internal.Display.HBStartKey),
                VTotal = item.ReadString(Models.Internal.Display.VTotalKey),
                VBEnd = item.ReadString(Models.Internal.Display.VBEndKey),
                VBStart = item.ReadString(Models.Internal.Display.VBStartKey),
            };
            return display;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Driver"/> to <cref="Models.Listxml.Driver"/>
        /// </summary>
        private static Driver ConvertFromInternalModel(Models.Internal.Driver item)
        {
            var driver = new Driver
            {
                Status = item.ReadString(Models.Internal.Driver.StatusKey),
                Color = item.ReadString(Models.Internal.Driver.ColorKey),
                Sound = item.ReadString(Models.Internal.Driver.SoundKey),
                PaletteSize = item.ReadString(Models.Internal.Driver.PaletteSizeKey),
                Emulation = item.ReadString(Models.Internal.Driver.EmulationKey),
                Cocktail = item.ReadString(Models.Internal.Driver.CocktailKey),
                SaveState = item.ReadString(Models.Internal.Driver.SaveStateKey),
                RequiresArtwork = item.ReadString(Models.Internal.Driver.RequiresArtworkKey),
                Unofficial = item.ReadString(Models.Internal.Driver.UnofficialKey),
                NoSoundHardware = item.ReadString(Models.Internal.Driver.NoSoundHardwareKey),
                Incomplete = item.ReadString(Models.Internal.Driver.IncompleteKey),
            };
            return driver;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Extension"/> to <cref="Models.Listxml.Extension"/>
        /// </summary>
        private static Extension ConvertFromInternalModel(Models.Internal.Extension item)
        {
            var extension = new Extension
            {
                Name = item.ReadString(Models.Internal.Extension.NameKey),
            };
            return extension;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Feature"/> to <cref="Models.Listxml.Feature"/>
        /// </summary>
        private static Feature ConvertFromInternalModel(Models.Internal.Feature item)
        {
            var feature = new Feature
            {
                Type = item.ReadString(Models.Internal.Feature.TypeKey),
                Status = item.ReadString(Models.Internal.Feature.StatusKey),
                Overall = item.ReadString(Models.Internal.Feature.OverallKey),
            };
            return feature;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Input"/> to <cref="Models.Listxml.Input"/>
        /// </summary>
        private static Input ConvertFromInternalModel(Models.Internal.Input item)
        {
            var input = new Input
            {
                Service = item.ReadString(Models.Internal.Input.ServiceKey),
                Tilt = item.ReadString(Models.Internal.Input.TiltKey),
                Players = item.ReadString(Models.Internal.Input.PlayersKey),
                ControlAttr = item.ReadString(Models.Internal.Input.ControlKey),
                Buttons = item.ReadString(Models.Internal.Input.ButtonsKey),
                Coins = item.ReadString(Models.Internal.Input.CoinsKey),
            };

            var controls = item.Read<Models.Internal.Control[]>(Models.Internal.Input.ControlKey);
            if (controls != null && controls.Any())
            {
                input.Control = controls
                    .Where(c => c != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            return input;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Instance"/> to <cref="Models.Listxml.Instance"/>
        /// </summary>
        private static Instance ConvertFromInternalModel(Models.Internal.Instance item)
        {
            var instance = new Instance
            {
                Name = item.ReadString(Models.Internal.Instance.NameKey),
                BriefName = item.ReadString(Models.Internal.Instance.BriefNameKey),
            };
            return instance;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Port"/> to <cref="Models.Listxml.Port"/>
        /// </summary>
        private static Port ConvertFromInternalModel(Models.Internal.Port item)
        {
            var port = new Port
            {
                Tag = item.ReadString(Models.Internal.Port.TagKey),
            };

            var analogs = item.Read<Models.Internal.Analog[]>(Models.Internal.Port.AnalogKey);
            if (analogs != null && analogs.Any())
            {
                port.Analog = analogs
                    .Where(a => a != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            return port;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.RamOption"/> to <cref="Models.Listxml.RamOption"/>
        /// </summary>
        private static RamOption ConvertFromInternalModel(Models.Internal.RamOption item)
        {
            var ramOption = new RamOption
            {
                Name = item.ReadString(Models.Internal.RamOption.NameKey),
                Default = item.ReadString(Models.Internal.RamOption.DefaultKey),
                Content = item.ReadString(Models.Internal.RamOption.ContentKey),
            };
            return ramOption;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.Listxml.Rom"/>
        /// </summary>
        private static Rom ConvertFromInternalModel(Models.Internal.Rom item)
        {
            var rom = new Rom
            {
                Name = item.ReadString(Models.Internal.Rom.NameKey),
                Bios = item.ReadString(Models.Internal.Rom.BiosKey),
                Size = item.ReadString(Models.Internal.Rom.SizeKey),
                CRC = item.ReadString(Models.Internal.Rom.CRCKey),
                SHA1 = item.ReadString(Models.Internal.Rom.SHA1Key),
                Merge = item.ReadString(Models.Internal.Rom.MergeKey),
                Region = item.ReadString(Models.Internal.Rom.RegionKey),
                Offset = item.ReadString(Models.Internal.Rom.OffsetKey),
                Status = item.ReadString(Models.Internal.Rom.StatusKey),
                Optional = item.ReadString(Models.Internal.Rom.OptionalKey),
                Dispose = item.ReadString(Models.Internal.Rom.DisposeKey),
                SoundOnly = item.ReadString(Models.Internal.Rom.SoundOnlyKey),
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Sample"/> to <cref="Models.Listxml.Sample"/>
        /// </summary>
        private static Sample ConvertFromInternalModel(Models.Internal.Sample item)
        {
            var sample = new Sample
            {
                Name = item.ReadString(Models.Internal.Sample.NameKey),
            };
            return sample;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Slot"/> to <cref="Models.Listxml.Slot"/>
        /// </summary>
        private static Slot ConvertFromInternalModel(Models.Internal.Slot item)
        {
            var slot = new Slot
            {
                Name = item.ReadString(Models.Internal.Slot.NameKey),
            };

            var slotOptions = item.Read<Models.Internal.SlotOption[]>(Models.Internal.Slot.SlotOptionKey);
            if (slotOptions != null && slotOptions.Any())
            {
                slot.SlotOption = slotOptions
                    .Where(s => s != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            return slot;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.SlotOption"/> to <cref="Models.Listxml.SlotOption"/>
        /// </summary>
        private static SlotOption ConvertFromInternalModel(Models.Internal.SlotOption item)
        {
            var slotOption = new SlotOption
            {
                Name = item.ReadString(Models.Internal.SlotOption.NameKey),
                DevName = item.ReadString(Models.Internal.SlotOption.DevNameKey),
                Default = item.ReadString(Models.Internal.SlotOption.DefaultKey),
            };
            return slotOption;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.SoftwareList"/> to <cref="Models.Listxml.SoftwareList"/>
        /// </summary>
        private static SoftwareList ConvertFromInternalModel(Models.Internal.SoftwareList item)
        {
            var softwareList = new SoftwareList
            {
                Tag = item.ReadString(Models.Internal.SoftwareList.TagKey),
                Name = item.ReadString(Models.Internal.SoftwareList.NameKey),
                Status = item.ReadString(Models.Internal.SoftwareList.StatusKey),
                Filter = item.ReadString(Models.Internal.SoftwareList.FilterKey),
            };
            return softwareList;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Sound"/> to <cref="Models.Listxml.Sound"/>
        /// </summary>
        private static Sound ConvertFromInternalModel(Models.Internal.Sound item)
        {
            var sound = new Sound
            {
                Channels = item.ReadString(Models.Internal.Sound.ChannelsKey),
            };
            return sound;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Video"/> to <cref="Models.Listxml.Video"/>
        /// </summary>
        private static Video ConvertFromInternalModel(Models.Internal.Video item)
        {
            var video = new Video
            {
                Screen = item.ReadString(Models.Internal.Video.ScreenKey),
                Orientation = item.ReadString(Models.Internal.Video.OrientationKey),
                Width = item.ReadString(Models.Internal.Video.WidthKey),
                Height = item.ReadString(Models.Internal.Video.HeightKey),
                AspectX = item.ReadString(Models.Internal.Video.AspectXKey),
                AspectY = item.ReadString(Models.Internal.Video.AspectYKey),
                Refresh = item.ReadString(Models.Internal.Video.RefreshKey),
            };
            return video;
        }

        #endregion
    }
}