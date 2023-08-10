using System.Linq;
using SabreTools.Models.Listxml;

namespace SabreTools.Serialization
{
    /// <summary>
    /// XML serializer for MAME listxml files
    /// </summary>
    public partial class Listxml : XmlSerializer<Mame>
    {
        #region Internal

        /// <summary>
        /// Convert from <cref="Models.Listxml.M1"/> to <cref="Models.Internal.MetadataFile"/>
        /// </summary>
        public static Models.Internal.MetadataFile ConvertToInternalModel(M1 item)
        {
            var metadataFile = new Models.Internal.MetadataFile
            {
                [Models.Internal.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item),
            };

            if (item?.Game != null && item.Game.Any())
                metadataFile[Models.Internal.MetadataFile.MachineKey] = item.Game.Select(ConvertMachineToInternalModel).ToArray();

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Mame"/> to <cref="Models.Internal.MetadataFile"/>
        /// </summary>
        public static Models.Internal.MetadataFile ConvertToInternalModel(Mame item)
        {
            var metadataFile = new Models.Internal.MetadataFile
            {
                [Models.Internal.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item),
            };

            if (item?.Game != null && item.Game.Any())
                metadataFile[Models.Internal.MetadataFile.MachineKey] = item.Game.Select(ConvertMachineToInternalModel).ToArray();

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.M1"/> to <cref="Models.Internal.Header"/>
        /// </summary>
        private static Models.Internal.Header ConvertHeaderToInternalModel(M1 item)
        {
            var header = new Models.Internal.Header
            {
                [Models.Internal.Header.VersionKey] = item.Version,
            };
            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Mame"/> to <cref="Models.Internal.Header"/>
        /// </summary>
        private static Models.Internal.Header ConvertHeaderToInternalModel(Mame item)
        {
            var header = new Models.Internal.Header
            {
                [Models.Internal.Header.BuildKey] = item.Build,
                [Models.Internal.Header.DebugKey] = item.Debug,
                [Models.Internal.Header.MameConfigKey] = item.MameConfig,
            };
            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.GameBase"/> to <cref="Models.Internal.Machine"/>
        /// </summary>
        private static Models.Internal.Machine ConvertMachineToInternalModel(GameBase item)
        {
            var machine = new Models.Internal.Machine
            {
                [Models.Internal.Machine.NameKey] = item.Name,
                [Models.Internal.Machine.SourceFileKey] = item.SourceFile,
                [Models.Internal.Machine.IsBiosKey] = item.IsBios,
                [Models.Internal.Machine.IsDeviceKey] = item.IsDevice,
                [Models.Internal.Machine.IsMechanicalKey] = item.IsMechanical,
                [Models.Internal.Machine.RunnableKey] = item.Runnable,
                [Models.Internal.Machine.CloneOfKey] = item.CloneOf,
                [Models.Internal.Machine.RomOfKey] = item.RomOf,
                [Models.Internal.Machine.SampleOfKey] = item.SampleOf,
                [Models.Internal.Machine.DescriptionKey] = item.Description,
                [Models.Internal.Machine.YearKey] = item.Year,
                [Models.Internal.Machine.ManufacturerKey] = item.Manufacturer,
                [Models.Internal.Machine.HistoryKey] = item.History,
            };

            if (item.BiosSet != null && item.BiosSet.Any())
                machine[Models.Internal.Machine.BiosSetKey] = item.BiosSet.Select(ConvertToInternalModel).ToArray();

            if (item.Rom != null && item.Rom.Any())
                machine[Models.Internal.Machine.RomKey] = item.Rom.Select(ConvertToInternalModel).ToArray();

            if (item.Disk != null && item.Disk.Any())
                machine[Models.Internal.Machine.DiskKey] = item.Disk.Select(ConvertToInternalModel).ToArray();

            if (item.DeviceRef != null && item.DeviceRef.Any())
                machine[Models.Internal.Machine.DeviceRefKey] = item.DeviceRef.Select(ConvertToInternalModel).ToArray();

            if (item.Sample != null && item.Sample.Any())
                machine[Models.Internal.Machine.SampleKey] = item.Sample.Select(ConvertToInternalModel).ToArray();

            if (item.Chip != null && item.Chip.Any())
                machine[Models.Internal.Machine.ChipKey] = item.Chip.Select(ConvertToInternalModel).ToArray();

            if (item.Display != null && item.Display.Any())
                machine[Models.Internal.Machine.DisplayKey] = item.Display.Select(ConvertToInternalModel).ToArray();

            if (item.Video != null && item.Video.Any())
                machine[Models.Internal.Machine.VideoKey] = item.Video.Select(ConvertToInternalModel).ToArray();

            if (item.Sound != null)
                machine[Models.Internal.Machine.SoundKey] = ConvertToInternalModel(item.Sound);

            if (item.Input != null)
                machine[Models.Internal.Machine.InputKey] = ConvertToInternalModel(item.Input);

            if (item.DipSwitch != null && item.DipSwitch.Any())
                machine[Models.Internal.Machine.DipSwitchKey] = item.DipSwitch.Select(ConvertToInternalModel).ToArray();

            if (item.Configuration != null && item.Configuration.Any())
                machine[Models.Internal.Machine.ConfigurationKey] = item.Configuration.Select(ConvertToInternalModel).ToArray();

            if (item.Port != null && item.Port.Any())
                machine[Models.Internal.Machine.PortKey] = item.Port.Select(ConvertToInternalModel).ToArray();

            if (item.Adjuster != null && item.Adjuster.Any())
                machine[Models.Internal.Machine.AdjusterKey] = item.Adjuster.Select(ConvertToInternalModel).ToArray();

            if (item.Driver != null)
                machine[Models.Internal.Machine.DriverKey] = ConvertToInternalModel(item.Driver);

            if (item.Feature != null && item.Feature.Any())
                machine[Models.Internal.Machine.FeatureKey] = item.Feature.Select(ConvertToInternalModel).ToArray();

            if (item.Device != null && item.Device.Any())
                machine[Models.Internal.Machine.DeviceKey] = item.Device.Select(ConvertToInternalModel).ToArray();

            if (item.Slot != null && item.Slot.Any())
                machine[Models.Internal.Machine.SlotKey] = item.Slot.Select(ConvertToInternalModel).ToArray();

            if (item.SoftwareList != null && item.SoftwareList.Any())
                machine[Models.Internal.Machine.SoftwareListKey] = item.SoftwareList.Select(ConvertToInternalModel).ToArray();

            if (item.RamOption != null && item.RamOption.Any())
                machine[Models.Internal.Machine.RamOptionKey] = item.RamOption.Select(ConvertToInternalModel).ToArray();

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Adjuster"/> to <cref="Models.Internal.Adjuster"/>
        /// </summary>
        private static Models.Internal.Adjuster ConvertToInternalModel(Adjuster item)
        {
            var adjuster = new Models.Internal.Adjuster
            {
                [Models.Internal.Adjuster.NameKey] = item.Name,
                [Models.Internal.Adjuster.DefaultKey] = item.Default,
            };

            if (item.Condition != null)
                adjuster[Models.Internal.Adjuster.ConditionKey] = ConvertToInternalModel(item.Condition);

            return adjuster;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Analog"/> to <cref="Models.Internal.Analog"/>
        /// </summary>
        private static Models.Internal.Analog ConvertToInternalModel(Analog item)
        {
            var analog = new Models.Internal.Analog
            {
                [Models.Internal.Analog.MaskKey] = item.Mask,
            };
            return analog;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.BiosSet"/> to <cref="Models.Internal.BiosSet"/>
        /// </summary>
        private static Models.Internal.BiosSet ConvertToInternalModel(BiosSet item)
        {
            var biosset = new Models.Internal.BiosSet
            {
                [Models.Internal.BiosSet.NameKey] = item.Name,
                [Models.Internal.BiosSet.DescriptionKey] = item.Description,
                [Models.Internal.BiosSet.DefaultKey] = item.Default,
            };
            return biosset;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Chip"/> to <cref="Models.Internal.Chip"/>
        /// </summary>
        private static Models.Internal.Chip ConvertToInternalModel(Chip item)
        {
            var chip = new Models.Internal.Chip
            {
                [Models.Internal.Chip.NameKey] = item.Name,
                [Models.Internal.Chip.TagKey] = item.Tag,
                [Models.Internal.Chip.TypeKey] = item.Type,
                [Models.Internal.Chip.SoundOnlyKey] = item.SoundOnly,
                [Models.Internal.Chip.ClockKey] = item.Clock,
            };
            return chip;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Condition"/> to <cref="Models.Internal.Condition"/>
        /// </summary>
        private static Models.Internal.Condition ConvertToInternalModel(Condition item)
        {
            var condition = new Models.Internal.Condition
            {
                [Models.Internal.Condition.TagKey] = item.Tag,
                [Models.Internal.Condition.MaskKey] = item.Mask,
                [Models.Internal.Condition.RelationKey] = item.Relation,
                [Models.Internal.Condition.ValueKey] = item.Value,
            };
            return condition;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Configuration"/> to <cref="Models.Internal.Configuration"/>
        /// </summary>
        private static Models.Internal.Configuration ConvertToInternalModel(Configuration item)
        {
            var configuration = new Models.Internal.Configuration
            {
                [Models.Internal.Configuration.NameKey] = item.Name,
                [Models.Internal.Configuration.TagKey] = item.Tag,
                [Models.Internal.Configuration.MaskKey] = item.Mask,
            };

            if (item.Condition != null)
                configuration[Models.Internal.Configuration.ConditionKey] = ConvertToInternalModel(item.Condition);

            if (item.ConfLocation != null && item.ConfLocation.Any())
                configuration[Models.Internal.Configuration.ConfLocationKey] = item.ConfLocation.Select(ConvertToInternalModel).ToArray();

            if (item.ConfSetting != null && item.ConfSetting.Any())
                configuration[Models.Internal.Configuration.ConfSettingKey] = item.ConfSetting.Select(ConvertToInternalModel).ToArray();

            return configuration;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.ConfLocation"/> to <cref="Models.Internal.ConfLocation"/>
        /// </summary>
        private static Models.Internal.ConfLocation ConvertToInternalModel(ConfLocation item)
        {
            var confLocation = new Models.Internal.ConfLocation
            {
                [Models.Internal.ConfLocation.NameKey] = item.Name,
                [Models.Internal.ConfLocation.NumberKey] = item.Number,
                [Models.Internal.ConfLocation.InvertedKey] = item.Inverted,
            };
            return confLocation;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.ConfSetting"/> to <cref="Models.Internal.ConfSetting"/>
        /// </summary>
        private static Models.Internal.ConfSetting ConvertToInternalModel(ConfSetting item)
        {
            var confSetting = new Models.Internal.ConfSetting
            {
                [Models.Internal.ConfSetting.NameKey] = item.Name,
                [Models.Internal.ConfSetting.ValueKey] = item.Value,
                [Models.Internal.ConfSetting.DefaultKey] = item.Default,
            };

            if (item.Condition != null)
                confSetting[Models.Internal.ConfSetting.ConditionKey] = ConvertToInternalModel(item.Condition);

            return confSetting;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Control"/> to <cref="Models.Internal.Control"/>
        /// </summary>
        private static Models.Internal.Control ConvertToInternalModel(Control item)
        {
            var control = new Models.Internal.Control
            {
                [Models.Internal.Control.TypeKey] = item.Type,
                [Models.Internal.Control.PlayerKey] = item.Player,
                [Models.Internal.Control.ButtonsKey] = item.Buttons,
                [Models.Internal.Control.ReqButtonsKey] = item.ReqButtons,
                [Models.Internal.Control.MinimumKey] = item.Minimum,
                [Models.Internal.Control.MaximumKey] = item.Maximum,
                [Models.Internal.Control.SensitivityKey] = item.Sensitivity,
                [Models.Internal.Control.KeyDeltaKey] = item.KeyDelta,
                [Models.Internal.Control.ReverseKey] = item.Reverse,
                [Models.Internal.Control.WaysKey] = item.Ways,
                [Models.Internal.Control.Ways2Key] = item.Ways2,
                [Models.Internal.Control.Ways3Key] = item.Ways3,
            };
            return control;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Device"/> to <cref="Models.Internal.Device"/>
        /// </summary>
        private static Models.Internal.Device ConvertToInternalModel(Device item)
        {
            var device = new Models.Internal.Device
            {
                [Models.Internal.Device.TypeKey] = item.Type,
                [Models.Internal.Device.TagKey] = item.Tag,
                [Models.Internal.Device.FixedImageKey] = item.FixedImage,
                [Models.Internal.Device.MandatoryKey] = item.Mandatory,
                [Models.Internal.Device.InterfaceKey] = item.Interface,
            };

            if (item.Instance != null)
                device[Models.Internal.Device.InstanceKey] = ConvertToInternalModel(item.Instance);

            if (item.Extension != null && item.Extension.Any())
                device[Models.Internal.Device.ExtensionKey] = item.Extension.Select(ConvertToInternalModel).ToArray();

            return device;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.DeviceRef"/> to <cref="Models.Internal.DeviceRef"/>
        /// </summary>
        private static Models.Internal.DeviceRef ConvertToInternalModel(DeviceRef item)
        {
            var deviceRef = new Models.Internal.DeviceRef
            {
                [Models.Internal.DeviceRef.NameKey] = item.Name,
            };
            return deviceRef;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.DipLocation"/> to <cref="Models.Internal.DipLocation"/>
        /// </summary>
        private static Models.Internal.DipLocation ConvertToInternalModel(DipLocation item)
        {
            var dipLocation = new Models.Internal.DipLocation
            {
                [Models.Internal.DipLocation.NameKey] = item.Name,
                [Models.Internal.DipLocation.NumberKey] = item.Number,
                [Models.Internal.DipLocation.InvertedKey] = item.Inverted,
            };
            return dipLocation;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.DipSwitch"/> to <cref="Models.Internal.DipSwitch"/>
        /// </summary>
        private static Models.Internal.DipSwitch ConvertToInternalModel(DipSwitch item)
        {
            var dipSwitch = new Models.Internal.DipSwitch
            {
                [Models.Internal.DipSwitch.NameKey] = item.Name,
                [Models.Internal.DipSwitch.TagKey] = item.Tag,
                [Models.Internal.DipSwitch.MaskKey] = item.Mask,
            };

            if (item.Condition != null)
                dipSwitch[Models.Internal.DipSwitch.ConditionKey] = ConvertToInternalModel(item.Condition);

            if (item.DipLocation != null && item.DipLocation.Any())
                dipSwitch[Models.Internal.DipSwitch.DipLocationKey] = item.DipLocation.Select(ConvertToInternalModel).ToArray();

            if (item.DipValue != null && item.DipValue.Any())
                dipSwitch[Models.Internal.DipSwitch.DipValueKey] = item.DipValue.Select(ConvertToInternalModel).ToArray();

            return dipSwitch;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.DipValue"/> to <cref="Models.Internal.DipValue"/>
        /// </summary>
        private static Models.Internal.DipValue ConvertToInternalModel(DipValue item)
        {
            var dipValue = new Models.Internal.DipValue
            {
                [Models.Internal.DipValue.NameKey] = item.Name,
                [Models.Internal.DipValue.ValueKey] = item.Value,
                [Models.Internal.DipValue.DefaultKey] = item.Default,
            };

            if (item.Condition != null)
                dipValue[Models.Internal.DipValue.ConditionKey] = ConvertToInternalModel(item.Condition);

            return dipValue;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Disk"/> to <cref="Models.Internal.Disk"/>
        /// </summary>
        private static Models.Internal.Disk ConvertToInternalModel(Disk item)
        {
            var disk = new Models.Internal.Disk
            {
                [Models.Internal.Disk.NameKey] = item.Name,
                [Models.Internal.Disk.MD5Key] = item.MD5,
                [Models.Internal.Disk.SHA1Key] = item.SHA1,
                [Models.Internal.Disk.MergeKey] = item.Merge,
                [Models.Internal.Disk.RegionKey] = item.Region,
                [Models.Internal.Disk.IndexKey] = item.Index,
                [Models.Internal.Disk.WritableKey] = item.Writable,
                [Models.Internal.Disk.StatusKey] = item.Status,
                [Models.Internal.Disk.OptionalKey] = item.Optional,
            };
            return disk;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Display"/> to <cref="Models.Internal.Display"/>
        /// </summary>
        private static Models.Internal.Display ConvertToInternalModel(Display item)
        {
            var display = new Models.Internal.Display
            {
                [Models.Internal.Display.TagKey] = item.Tag,
                [Models.Internal.Display.TypeKey] = item.Type,
                [Models.Internal.Display.RotateKey] = item.Rotate,
                [Models.Internal.Display.FlipXKey] = item.FlipX,
                [Models.Internal.Display.WidthKey] = item.Width,
                [Models.Internal.Display.HeightKey] = item.Height,
                [Models.Internal.Display.RefreshKey] = item.Refresh,
                [Models.Internal.Display.PixClockKey] = item.PixClock,
                [Models.Internal.Display.HTotalKey] = item.HTotal,
                [Models.Internal.Display.HBEndKey] = item.HBEnd,
                [Models.Internal.Display.HBStartKey] = item.HBStart,
                [Models.Internal.Display.VTotalKey] = item.VTotal,
                [Models.Internal.Display.VBEndKey] = item.VBEnd,
                [Models.Internal.Display.VBStartKey] = item.VBStart,
            };
            return display;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Driver"/> to <cref="Models.Internal.Driver"/>
        /// </summary>
        private static Models.Internal.Driver ConvertToInternalModel(Driver item)
        {
            var driver = new Models.Internal.Driver
            {
                [Models.Internal.Driver.StatusKey] = item.Status,
                [Models.Internal.Driver.ColorKey] = item.Color,
                [Models.Internal.Driver.SoundKey] = item.Sound,
                [Models.Internal.Driver.PaletteSizeKey] = item.PaletteSize,
                [Models.Internal.Driver.EmulationKey] = item.Emulation,
                [Models.Internal.Driver.CocktailKey] = item.Cocktail,
                [Models.Internal.Driver.SaveStateKey] = item.SaveState,
                [Models.Internal.Driver.RequiresArtworkKey] = item.RequiresArtwork,
                [Models.Internal.Driver.UnofficialKey] = item.Unofficial,
                [Models.Internal.Driver.NoSoundHardwareKey] = item.NoSoundHardware,
                [Models.Internal.Driver.IncompleteKey] = item.Incomplete,
            };
            return driver;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Extension"/> to <cref="Models.Internal.Extension"/>
        /// </summary>
        private static Models.Internal.Extension ConvertToInternalModel(Extension item)
        {
            var extension = new Models.Internal.Extension
            {
                [Models.Internal.Extension.NameKey] = item.Name,
            };
            return extension;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Feature"/> to <cref="Models.Internal.Feature"/>
        /// </summary>
        private static Models.Internal.Feature ConvertToInternalModel(Feature item)
        {
            var feature = new Models.Internal.Feature
            {
                [Models.Internal.Feature.TypeKey] = item.Type,
                [Models.Internal.Feature.StatusKey] = item.Status,
                [Models.Internal.Feature.OverallKey] = item.Overall,
            };
            return feature;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Input"/> to <cref="Models.Internal.Input"/>
        /// </summary>
        private static Models.Internal.Input ConvertToInternalModel(Input item)
        {
            var input = new Models.Internal.Input
            {
                [Models.Internal.Input.ServiceKey] = item.Service,
                [Models.Internal.Input.TiltKey] = item.Tilt,
                [Models.Internal.Input.PlayersKey] = item.Players,
                [Models.Internal.Input.ControlKey] = item.ControlAttr,
                [Models.Internal.Input.ButtonsKey] = item.Buttons,
                [Models.Internal.Input.CoinsKey] = item.Coins,
            };

            if (item.Control != null && item.Control.Any())
                input[Models.Internal.Input.ControlKey] = item.Control.Select(ConvertToInternalModel).ToArray();

            return input;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Instance"/> to <cref="Models.Internal.Instance"/>
        /// </summary>
        private static Models.Internal.Instance ConvertToInternalModel(Instance item)
        {
            var instance = new Models.Internal.Instance
            {
                [Models.Internal.Instance.NameKey] = item.Name,
                [Models.Internal.Instance.BriefNameKey] = item.BriefName,
            };
            return instance;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Port"/> to <cref="Models.Internal.Port"/>
        /// </summary>
        private static Models.Internal.Port ConvertToInternalModel(Port item)
        {
            var port = new Models.Internal.Port
            {
                [Models.Internal.Port.TagKey] = item.Tag,
            };

            if (item.Analog != null && item.Analog.Any())
                port[Models.Internal.Port.AnalogKey] = item.Analog.Select(ConvertToInternalModel).ToArray();

            return port;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.RamOption"/> to <cref="Models.Internal.RamOption"/>
        /// </summary>
        private static Models.Internal.RamOption ConvertToInternalModel(RamOption item)
        {
            var ramOption = new Models.Internal.RamOption
            {
                [Models.Internal.RamOption.NameKey] = item.Name,
                [Models.Internal.RamOption.DefaultKey] = item.Default,
            };
            return ramOption;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Rom"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        private static Models.Internal.Rom ConvertToInternalModel(Rom item)
        {
            var rom = new Models.Internal.Rom
            {
                [Models.Internal.Rom.NameKey] = item.Name,
                [Models.Internal.Rom.BiosKey] = item.Bios,
                [Models.Internal.Rom.SizeKey] = item.Size,
                [Models.Internal.Rom.CRCKey] = item.CRC,
                [Models.Internal.Rom.SHA1Key] = item.SHA1,
                [Models.Internal.Rom.MergeKey] = item.Merge,
                [Models.Internal.Rom.RegionKey] = item.Region,
                [Models.Internal.Rom.OffsetKey] = item.Offset,
                [Models.Internal.Rom.StatusKey] = item.Status,
                [Models.Internal.Rom.OptionalKey] = item.Optional,
                [Models.Internal.Rom.DisposeKey] = item.Dispose,
                [Models.Internal.Rom.SoundOnlyKey] = item.SoundOnly,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Sample"/> to <cref="Models.Internal.Sample"/>
        /// </summary>
        private static Models.Internal.Sample ConvertToInternalModel(Sample item)
        {
            var sample = new Models.Internal.Sample
            {
                [Models.Internal.Sample.NameKey] = item.Name,
            };
            return sample;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Slot"/> to <cref="Models.Internal.Slot"/>
        /// </summary>
        private static Models.Internal.Slot ConvertToInternalModel(Slot item)
        {
            var slot = new Models.Internal.Slot
            {
                [Models.Internal.Slot.NameKey] = item.Name,
            };

            if (item.SlotOption != null && item.SlotOption.Any())
                slot[Models.Internal.Slot.SlotOptionKey] = item.SlotOption.Select(ConvertToInternalModel).ToArray();

            return slot;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.SlotOption"/> to <cref="Models.Internal.SlotOption"/>
        /// </summary>
        private static Models.Internal.SlotOption ConvertToInternalModel(SlotOption item)
        {
            var slotOption = new Models.Internal.SlotOption
            {
                [Models.Internal.SlotOption.NameKey] = item.Name,
                [Models.Internal.SlotOption.DevNameKey] = item.DevName,
                [Models.Internal.SlotOption.DefaultKey] = item.Default,
            };
            return slotOption;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.SoftwareList"/> to <cref="Models.Internal.SoftwareList"/>
        /// </summary>
        private static Models.Internal.SoftwareList ConvertToInternalModel(SoftwareList item)
        {
            var softwareList = new Models.Internal.SoftwareList
            {
                [Models.Internal.SoftwareList.TagKey] = item.Tag,
                [Models.Internal.SoftwareList.NameKey] = item.Name,
                [Models.Internal.SoftwareList.StatusKey] = item.Status,
                [Models.Internal.SoftwareList.FilterKey] = item.Filter,
            };
            return softwareList;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Sound"/> to <cref="Models.Internal.Sound"/>
        /// </summary>
        private static Models.Internal.Sound ConvertToInternalModel(Sound item)
        {
            var sound = new Models.Internal.Sound
            {
                [Models.Internal.Sound.ChannelsKey] = item.Channels,
            };
            return sound;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Video"/> to <cref="Models.Internal.Video"/>
        /// </summary>
        private static Models.Internal.Video ConvertToInternalModel(Video item)
        {
            var video = new Models.Internal.Video
            {
                [Models.Internal.Video.ScreenKey] = item.Screen,
                [Models.Internal.Video.OrientationKey] = item.Orientation,
                [Models.Internal.Video.WidthKey] = item.Width,
                [Models.Internal.Video.HeightKey] = item.Height,
                [Models.Internal.Video.AspectXKey] = item.AspectX,
                [Models.Internal.Video.AspectYKey] = item.AspectY,
                [Models.Internal.Video.RefreshKey] = item.Refresh,
            };
            return video;
        }

        #endregion
    
    }
}