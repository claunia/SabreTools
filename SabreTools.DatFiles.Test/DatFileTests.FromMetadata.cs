using System;
using System.Linq;
using SabreTools.DatItems.Formats;
using SabreTools.Hashing;
using Xunit;

namespace SabreTools.DatFiles.Test
{
    public partial class DatFileTests
    {
        #region ConvertFromMetadata

        [Fact]
        public void ConvertFromMetadata_Null()
        {
            Models.Metadata.MetadataFile? item = null;

            DatFile datFile = new Formats.Logiqx(null, deprecated: false);
            datFile.ConvertFromMetadata(item, "filename", indexId: 0, keep: true, statsOnly: false);

            Assert.Empty(datFile.Items);
            Assert.Empty(datFile.ItemsDB.GetItems());
        }

        [Fact]
        public void ConvertFromMetadata_Empty()
        {
            Models.Metadata.MetadataFile? item = new Models.Metadata.MetadataFile();

            DatFile datFile = new Formats.Logiqx(null, deprecated: false);
            datFile.ConvertFromMetadata(item, "filename", indexId: 0, keep: true, statsOnly: false);

            Assert.Empty(datFile.Items);
            Assert.Empty(datFile.ItemsDB.GetItems());
        }

        [Fact]
        public void ConvertFromMetadata_FilledHeader()
        {
            Models.Metadata.Header? header = CreateHeader();
            Models.Metadata.Machine[]? machines = null;
            Models.Metadata.MetadataFile? item = new Models.Metadata.MetadataFile
            {
                [Models.Metadata.MetadataFile.HeaderKey] = header,
                [Models.Metadata.MetadataFile.MachineKey] = machines,
            };

            DatFile datFile = new Formats.Logiqx(null, deprecated: false);
            datFile.ConvertFromMetadata(item, "filename", indexId: 0, keep: true, statsOnly: false);

            ValidateHeader(datFile.Header);
        }

        [Fact]
        public void ConvertFromMetadata_FilledMachine()
        {
            Models.Metadata.Header? header = null;
            Models.Metadata.Machine machine = CreateMachine();
            Models.Metadata.Machine[]? machines = [machine];
            Models.Metadata.MetadataFile? item = new Models.Metadata.MetadataFile
            {
                [Models.Metadata.MetadataFile.HeaderKey] = header,
                [Models.Metadata.MetadataFile.MachineKey] = machines,
            };

            DatFile datFile = new Formats.Logiqx(null, deprecated: false);
            datFile.ConvertFromMetadata(item, "filename", indexId: 0, keep: true, statsOnly: false);

            DatItems.Machine actualMachine = Assert.Single(datFile.ItemsDB.GetMachines()).Value;
            ValidateMachine(actualMachine);

            // Aggregate for easier validation
            DatItems.DatItem[] datItems = datFile.Items
                .SelectMany(kvp => kvp.Value ?? [])
                .ToArray();

            Adjuster? adjuster = Array.Find(datItems, item => item is Adjuster) as Adjuster;
            ValidateAdjuster(adjuster);

            Archive? archive = Array.Find(datItems, item => item is Archive) as Archive;
            ValidateArchive(archive);

            BiosSet? biosSet = Array.Find(datItems, item => item is BiosSet) as BiosSet;
            ValidateBiosSet(biosSet);

            Chip? chip = Array.Find(datItems, item => item is Chip) as Chip;
            ValidateChip(chip);

            Configuration? configuration = Array.Find(datItems, item => item is Configuration) as Configuration;
            ValidateConfiguration(configuration);

            Device? device = Array.Find(datItems, item => item is Device) as Device;
            ValidateDevice(device);

            DeviceRef? deviceRef = Array.Find(datItems, item => item is DeviceRef) as DeviceRef;
            ValidateDeviceRef(deviceRef);

            DipSwitch? dipSwitch = Array.Find(datItems, item => item is DipSwitch) as DipSwitch;
            ValidateDipSwitch(dipSwitch);

            Disk? disk = Array.Find(datItems, item => item is Disk) as Disk;
            ValidateDisk(disk);

            Display? display = Array.Find(datItems, item => item is Display) as Display;
            ValidateDisplay(display);

            Driver? driver = Array.Find(datItems, item => item is Driver) as Driver;
            ValidateDriver(driver);

            // All other Rom fields are tested separately
            Rom? dumpRom = Array.Find(datItems, item => item is Rom rom && rom.GetStringFieldValue(Models.Metadata.Rom.OpenMSXMediaType) != null) as Rom;
            Assert.NotNull(dumpRom);
            Assert.Equal("rom", dumpRom.GetStringFieldValue(Models.Metadata.Rom.OpenMSXMediaType));

            Feature? feature = Array.Find(datItems, item => item is Feature) as Feature;
            ValidateFeature(feature);

            Info? info = Array.Find(datItems, item => item is Info) as Info;
            ValidateInfo(info);

            Input? input = Array.Find(datItems, item => item is Input) as Input;
            ValidateInput(input);

            Media? media = Array.Find(datItems, item => item is Media) as Media;
            ValidateMedia(media);

            // TODO: Validate all fields
        }

        #endregion

        #region Helpers

        private static Models.Metadata.Header? CreateHeader()
        {
            Models.OfflineList.CanOpen canOpen = new Models.OfflineList.CanOpen
            {
                Extension = ["ext"],
            };

            Models.OfflineList.Images images = new Models.OfflineList.Images();

            Models.OfflineList.Infos infos = new Models.OfflineList.Infos();

            Models.OfflineList.NewDat newDat = new Models.OfflineList.NewDat();

            Models.OfflineList.Search search = new Models.OfflineList.Search();

            return new Models.Metadata.Header
            {
                [Models.Metadata.Header.AuthorKey] = "author",
                [Models.Metadata.Header.BiosModeKey] = "merged",
                [Models.Metadata.Header.BuildKey] = "build",
                [Models.Metadata.Header.CanOpenKey] = canOpen,
                [Models.Metadata.Header.CategoryKey] = "category",
                [Models.Metadata.Header.CommentKey] = "comment",
                [Models.Metadata.Header.DateKey] = "date",
                [Models.Metadata.Header.DatVersionKey] = "datversion",
                [Models.Metadata.Header.DebugKey] = "yes",
                [Models.Metadata.Header.DescriptionKey] = "description",
                [Models.Metadata.Header.EmailKey] = "email",
                [Models.Metadata.Header.EmulatorVersionKey] = "emulatorversion",
                [Models.Metadata.Header.ForceMergingKey] = "merged",
                [Models.Metadata.Header.ForceNodumpKey] = "required",
                [Models.Metadata.Header.ForcePackingKey] = "zip",
                [Models.Metadata.Header.ForceZippingKey] = "yes",
                [Models.Metadata.Header.HeaderKey] = "header",
                [Models.Metadata.Header.HomepageKey] = "homepage",
                [Models.Metadata.Header.IdKey] = "id",
                [Models.Metadata.Header.ImagesKey] = images,
                [Models.Metadata.Header.ImFolderKey] = "imfolder",
                [Models.Metadata.Header.InfosKey] = infos,
                [Models.Metadata.Header.LockBiosModeKey] = "yes",
                [Models.Metadata.Header.LockRomModeKey] = "yes",
                [Models.Metadata.Header.LockSampleModeKey] = "yes",
                [Models.Metadata.Header.MameConfigKey] = "mameconfig",
                [Models.Metadata.Header.NameKey] = "name",
                [Models.Metadata.Header.NewDatKey] = newDat,
                [Models.Metadata.Header.NotesKey] = "notes",
                [Models.Metadata.Header.PluginKey] = "plugin",
                [Models.Metadata.Header.RefNameKey] = "refname",
                [Models.Metadata.Header.RomModeKey] = "merged",
                [Models.Metadata.Header.RomTitleKey] = "romtitle",
                [Models.Metadata.Header.RootDirKey] = "rootdir",
                [Models.Metadata.Header.SampleModeKey] = "merged",
                [Models.Metadata.Header.SchemaLocationKey] = "schemalocation",
                [Models.Metadata.Header.ScreenshotsHeightKey] = "screenshotsheight",
                [Models.Metadata.Header.ScreenshotsWidthKey] = "screenshotsWidth",
                [Models.Metadata.Header.SearchKey] = search,
                [Models.Metadata.Header.SystemKey] = "system",
                [Models.Metadata.Header.TimestampKey] = "timestamp",
                [Models.Metadata.Header.TypeKey] = "type",
                [Models.Metadata.Header.UrlKey] = "url",
                [Models.Metadata.Header.VersionKey] = "version",
            };
        }

        private static Models.Metadata.Machine CreateMachine()
        {
            // Used by multiple items
            Models.Metadata.Condition condition = new Models.Metadata.Condition
            {
                [Models.Metadata.Condition.ValueKey] = "value",
                [Models.Metadata.Condition.MaskKey] = "mask",
                [Models.Metadata.Condition.RelationKey] = "eq",
                [Models.Metadata.Condition.TagKey] = "tag",
            };

            Models.Metadata.Adjuster adjuster = new Models.Metadata.Adjuster
            {
                [Models.Metadata.Adjuster.ConditionKey] = condition,
                [Models.Metadata.Adjuster.DefaultKey] = true,
                [Models.Metadata.Adjuster.NameKey] = "name",
            };

            Models.Metadata.Archive archive = new Models.Metadata.Archive
            {
                [Models.Metadata.Archive.NameKey] = "name",
            };

            Models.Metadata.BiosSet biosSet = new Models.Metadata.BiosSet
            {
                [Models.Metadata.BiosSet.DefaultKey] = true,
                [Models.Metadata.BiosSet.DescriptionKey] = "description",
                [Models.Metadata.BiosSet.NameKey] = "name",
            };

            Models.Metadata.Chip chip = new Models.Metadata.Chip
            {
                [Models.Metadata.Chip.ClockKey] = 12345,
                [Models.Metadata.Chip.FlagsKey] = "flags",
                [Models.Metadata.Chip.NameKey] = "name",
                [Models.Metadata.Chip.SoundOnlyKey] = "yes",
                [Models.Metadata.Chip.TagKey] = "tag",
                [Models.Metadata.Chip.ChipTypeKey] = "cpu",
            };

            Models.Metadata.ConfLocation confLocation = new Models.Metadata.ConfLocation
            {
                [Models.Metadata.ConfLocation.InvertedKey] = "yes",
                [Models.Metadata.ConfLocation.NameKey] = "name",
                [Models.Metadata.ConfLocation.NumberKey] = "number",
            };

            Models.Metadata.ConfSetting confSetting = new Models.Metadata.ConfSetting
            {
                [Models.Metadata.ConfSetting.ConditionKey] = condition,
                [Models.Metadata.ConfSetting.DefaultKey] = "yes",
                [Models.Metadata.ConfSetting.NameKey] = "name",
                [Models.Metadata.ConfSetting.ValueKey] = "value",
            };

            Models.Metadata.Configuration configuration = new Models.Metadata.Configuration
            {
                [Models.Metadata.Configuration.ConditionKey] = condition,
                [Models.Metadata.Configuration.ConfLocationKey] = new Models.Metadata.ConfLocation[] { confLocation },
                [Models.Metadata.Configuration.ConfSettingKey] = new Models.Metadata.ConfSetting[] { confSetting },
                [Models.Metadata.Configuration.MaskKey] = "mask",
                [Models.Metadata.Configuration.NameKey] = "name",
                [Models.Metadata.Configuration.TagKey] = "tag",
            };

            Models.Metadata.Extension extension = new Models.Metadata.Extension
            {
                [Models.Metadata.Extension.NameKey] = "name",
            };

            Models.Metadata.Instance instance = new Models.Metadata.Instance
            {
                [Models.Metadata.Instance.BriefNameKey] = "briefname",
                [Models.Metadata.Instance.NameKey] = "name",
            };

            Models.Metadata.Device device = new Models.Metadata.Device
            {
                [Models.Metadata.Device.ExtensionKey] = new Models.Metadata.Extension[] { extension },
                [Models.Metadata.Device.FixedImageKey] = "fixedimage",
                [Models.Metadata.Device.InstanceKey] = instance,
                [Models.Metadata.Device.InterfaceKey] = "interface",
                [Models.Metadata.Device.MandatoryKey] = 1,
                [Models.Metadata.Device.TagKey] = "tag",
                [Models.Metadata.Device.DeviceTypeKey] = "punchtape",
            };

            Models.Metadata.DeviceRef deviceRef = new Models.Metadata.DeviceRef
            {
                [Models.Metadata.DeviceRef.NameKey] = "name",
            };

            Models.Metadata.DipLocation dipLocation = new Models.Metadata.DipLocation
            {
                [Models.Metadata.DipLocation.InvertedKey] = "yes",
                [Models.Metadata.DipLocation.NameKey] = "name",
                [Models.Metadata.DipLocation.NumberKey] = "number",
            };

            Models.Metadata.DipValue dipValue = new Models.Metadata.DipValue
            {
                [Models.Metadata.DipValue.ConditionKey] = condition,
                [Models.Metadata.DipValue.DefaultKey] = "yes",
                [Models.Metadata.DipValue.NameKey] = "name",
                [Models.Metadata.DipValue.ValueKey] = "value",
            };

            Models.Metadata.DipSwitch dipSwitch = new Models.Metadata.DipSwitch
            {
                [Models.Metadata.DipSwitch.ConditionKey] = condition,
                [Models.Metadata.DipSwitch.DefaultKey] = "yes",
                [Models.Metadata.DipSwitch.DipLocationKey] = new Models.Metadata.DipLocation[] { dipLocation },
                [Models.Metadata.DipSwitch.DipValueKey] = new Models.Metadata.DipValue[] { dipValue },
                [Models.Metadata.DipSwitch.EntryKey] = new string[] { "entry" },
                [Models.Metadata.DipSwitch.MaskKey] = "mask",
                [Models.Metadata.DipSwitch.NameKey] = "name",
                [Models.Metadata.DipSwitch.TagKey] = "tag",
            };

            Models.Metadata.Disk disk = new Models.Metadata.Disk
            {
                [Models.Metadata.Disk.FlagsKey] = "flags",
                [Models.Metadata.Disk.IndexKey] = "index",
                [Models.Metadata.Disk.MD5Key] = ZeroHash.MD5Str,
                [Models.Metadata.Disk.MergeKey] = "merge",
                [Models.Metadata.Disk.NameKey] = "name",
                [Models.Metadata.Disk.OptionalKey] = "yes",
                [Models.Metadata.Disk.RegionKey] = "region",
                [Models.Metadata.Disk.SHA1Key] = ZeroHash.SHA1Str,
                [Models.Metadata.Disk.WritableKey] = "yes",
            };

            Models.Metadata.Display display = new Models.Metadata.Display
            {
                [Models.Metadata.Display.FlipXKey] = "yes",
                [Models.Metadata.Display.HBEndKey] = 12345,
                [Models.Metadata.Display.HBStartKey] = 12345,
                [Models.Metadata.Display.HeightKey] = 12345,
                [Models.Metadata.Display.HTotalKey] = 12345,
                [Models.Metadata.Display.PixClockKey] = 12345,
                [Models.Metadata.Display.RefreshKey] = 12345,
                [Models.Metadata.Display.RotateKey] = 90,
                [Models.Metadata.Display.TagKey] = "tag",
                [Models.Metadata.Display.DisplayTypeKey] = "vector",
                [Models.Metadata.Display.VBEndKey] = 12345,
                [Models.Metadata.Display.VBStartKey] = 12345,
                [Models.Metadata.Display.VTotalKey] = 12345,
                [Models.Metadata.Display.WidthKey] = 12345,
            };

            Models.Metadata.Driver driver = new Models.Metadata.Driver
            {
                [Models.Metadata.Driver.BlitKey] = "plain",
                [Models.Metadata.Driver.CocktailKey] = "good",
                [Models.Metadata.Driver.ColorKey] = "good",
                [Models.Metadata.Driver.EmulationKey] = "good",
                [Models.Metadata.Driver.IncompleteKey] = "yes",
                [Models.Metadata.Driver.NoSoundHardwareKey] = "yes",
                [Models.Metadata.Driver.PaletteSizeKey] = "pallettesize",
                [Models.Metadata.Driver.RequiresArtworkKey] = "yes",
                [Models.Metadata.Driver.SaveStateKey] = "supported",
                [Models.Metadata.Driver.SoundKey] = "good",
                [Models.Metadata.Driver.StatusKey] = "good",
                [Models.Metadata.Driver.UnofficialKey] = "yes",
            };

            Models.Metadata.Original original = new Models.Metadata.Original
            {
                [Models.Metadata.Original.ContentKey] = "content",
                [Models.Metadata.Original.ValueKey] = true,
            };

            Models.Metadata.Dump dump = new Models.Metadata.Dump
            {
                [Models.Metadata.Dump.OriginalKey] = original,

                // The following are searched for in order
                // For the purposes of this test, only RomKey will be populated
                // The only difference is what OpenMSXSubType value is applied
                [Models.Metadata.Dump.RomKey] = new Models.Metadata.Rom(),
                [Models.Metadata.Dump.MegaRomKey] = null,
                [Models.Metadata.Dump.SCCPlusCartKey] = null,
            };

            Models.Metadata.Feature feature = new Models.Metadata.Feature
            {
                [Models.Metadata.Feature.NameKey] = "name",
                [Models.Metadata.Feature.OverallKey] = "imperfect",
                [Models.Metadata.Feature.StatusKey] = "imperfect",
                [Models.Metadata.Feature.FeatureTypeKey] = "protection",
                [Models.Metadata.Feature.ValueKey] = "value",
            };

            Models.Metadata.Info info = new Models.Metadata.Info
            {
                [Models.Metadata.Info.NameKey] = "name",
                [Models.Metadata.Info.ValueKey] = "value",
            };

            Models.Metadata.Control control = new Models.Metadata.Control
            {
                [Models.Metadata.Control.ButtonsKey] = 12345,
                [Models.Metadata.Control.KeyDeltaKey] = 12345,
                [Models.Metadata.Control.MaximumKey] = 12345,
                [Models.Metadata.Control.MinimumKey] = 12345,
                [Models.Metadata.Control.PlayerKey] = 12345,
                [Models.Metadata.Control.ReqButtonsKey] = 12345,
                [Models.Metadata.Control.ReverseKey] = "yes",
                [Models.Metadata.Control.SensitivityKey] = 12345,
                [Models.Metadata.Control.ControlTypeKey] = "lightgun",
                [Models.Metadata.Control.WaysKey] = "ways",
                [Models.Metadata.Control.Ways2Key] = "ways2",
                [Models.Metadata.Control.Ways3Key] = "ways3",
            };

            Models.Metadata.Input input = new Models.Metadata.Input
            {
                [Models.Metadata.Input.ButtonsKey] = 12345,
                [Models.Metadata.Input.CoinsKey] = 12345,
                [Models.Metadata.Input.ControlKey] = new Models.Metadata.Control[] { control },
                [Models.Metadata.Input.PlayersKey] = 12345,
                [Models.Metadata.Input.ServiceKey] = "yes",
                [Models.Metadata.Input.TiltKey] = "yes",
            };

            Models.Metadata.Media media = new Models.Metadata.Media
            {
                [Models.Metadata.Media.MD5Key] = ZeroHash.MD5Str,
                [Models.Metadata.Media.NameKey] = "name",
                [Models.Metadata.Media.SHA1Key] = ZeroHash.SHA1Str,
                [Models.Metadata.Media.SHA256Key] = ZeroHash.SHA256Str,
                [Models.Metadata.Media.SpamSumKey] = ZeroHash.SpamSumStr,
            };

            return new Models.Metadata.Machine
            {
                [Models.Metadata.Machine.AdjusterKey] = new Models.Metadata.Adjuster[] { adjuster },
                [Models.Metadata.Machine.ArchiveKey] = new Models.Metadata.Archive[] { archive },
                [Models.Metadata.Machine.BiosSetKey] = new Models.Metadata.BiosSet[] { biosSet },
                [Models.Metadata.Machine.BoardKey] = "board",
                [Models.Metadata.Machine.ButtonsKey] = "buttons",
                [Models.Metadata.Machine.CategoryKey] = "category",
                [Models.Metadata.Machine.ChipKey] = new Models.Metadata.Chip[] { chip },
                [Models.Metadata.Machine.CloneOfKey] = "cloneof",
                [Models.Metadata.Machine.CloneOfIdKey] = "cloneofid",
                [Models.Metadata.Machine.CommentKey] = "comment",
                [Models.Metadata.Machine.CompanyKey] = "company",
                [Models.Metadata.Machine.ConfigurationKey] = new Models.Metadata.Configuration[] { configuration },
                [Models.Metadata.Machine.ControlKey] = "control",
                [Models.Metadata.Machine.CountryKey] = "country",
                [Models.Metadata.Machine.DescriptionKey] = "description",
                [Models.Metadata.Machine.DeviceKey] = new Models.Metadata.Device[] { device },
                [Models.Metadata.Machine.DeviceRefKey] = new Models.Metadata.DeviceRef[] { deviceRef },
                [Models.Metadata.Machine.DipSwitchKey] = new Models.Metadata.DipSwitch[] { dipSwitch },
                [Models.Metadata.Machine.DirNameKey] = "dirname",
                [Models.Metadata.Machine.DiskKey] = new Models.Metadata.Disk[] { disk },
                [Models.Metadata.Machine.DisplayCountKey] = "displaycount",
                [Models.Metadata.Machine.DisplayKey] = new Models.Metadata.Display[] { display },
                [Models.Metadata.Machine.DisplayTypeKey] = "displaytype",
                [Models.Metadata.Machine.DriverKey] = driver,
                [Models.Metadata.Machine.DumpKey] = new Models.Metadata.Dump[] { dump },
                [Models.Metadata.Machine.DuplicateIDKey] = "duplicateid",
                [Models.Metadata.Machine.EmulatorKey] = "emulator",
                [Models.Metadata.Machine.ExtraKey] = "extra",
                [Models.Metadata.Machine.FavoriteKey] = "favorite",
                [Models.Metadata.Machine.FeatureKey] = new Models.Metadata.Feature[] { feature },
                [Models.Metadata.Machine.GenMSXIDKey] = "genmsxid",
                [Models.Metadata.Machine.HistoryKey] = "history",
                [Models.Metadata.Machine.IdKey] = "id",
                [Models.Metadata.Machine.Im1CRCKey] = ZeroHash.CRC32Str,
                [Models.Metadata.Machine.Im2CRCKey] = ZeroHash.CRC32Str,
                [Models.Metadata.Machine.ImageNumberKey] = "imagenumber",
                [Models.Metadata.Machine.InfoKey] = new Models.Metadata.Info[] { info },
                [Models.Metadata.Machine.InputKey] = input,
                [Models.Metadata.Machine.IsBiosKey] = "yes",
                [Models.Metadata.Machine.IsDeviceKey] = "yes",
                [Models.Metadata.Machine.IsMechanicalKey] = "yes",
                [Models.Metadata.Machine.LanguageKey] = "language",
                [Models.Metadata.Machine.LocationKey] = "location",
                [Models.Metadata.Machine.ManufacturerKey] = "manufacturer",
                [Models.Metadata.Machine.MediaKey] = new Models.Metadata.Media[] { media },
                [Models.Metadata.Machine.NameKey] = "name",
                [Models.Metadata.Machine.NotesKey] = "notes",
                [Models.Metadata.Machine.PartKey] = "REPLACE", // Type array
                [Models.Metadata.Machine.PlayedCountKey] = "playedcount",
                [Models.Metadata.Machine.PlayedTimeKey] = "playedtime",
                [Models.Metadata.Machine.PlayersKey] = "players",
                [Models.Metadata.Machine.PortKey] = "REPLACE", // Type array
                [Models.Metadata.Machine.PublisherKey] = "publisher",
                [Models.Metadata.Machine.RamOptionKey] = "REPLACE", // Type array
                [Models.Metadata.Machine.RebuildToKey] = "rebuildto",
                [Models.Metadata.Machine.ReleaseKey] = "REPLACE", // Type array
                [Models.Metadata.Machine.ReleaseNumberKey] = "releasenumber",
                [Models.Metadata.Machine.RomKey] = "REPLACE", // Type array
                [Models.Metadata.Machine.RomOfKey] = "romof",
                [Models.Metadata.Machine.RotationKey] = "rotation",
                [Models.Metadata.Machine.RunnableKey] = "yes",
                [Models.Metadata.Machine.SampleKey] = "REPLACE", // Type array
                [Models.Metadata.Machine.SampleOfKey] = "sampleof",
                [Models.Metadata.Machine.SaveTypeKey] = "savetype",
                [Models.Metadata.Machine.SharedFeatKey] = "REPLACE", // Type array
                [Models.Metadata.Machine.SlotKey] = "REPLACE", // Type array
                [Models.Metadata.Machine.SoftwareListKey] = "REPLACE", // Type array
                [Models.Metadata.Machine.SoundKey] = "REPLACE", // Type
                [Models.Metadata.Machine.SourceFileKey] = "sourcefile",
                [Models.Metadata.Machine.SourceRomKey] = "sourcerom",
                [Models.Metadata.Machine.StatusKey] = "status",
                [Models.Metadata.Machine.SupportedKey] = "yes",
                [Models.Metadata.Machine.SystemKey] = "system",
                [Models.Metadata.Machine.TagsKey] = "tags",
                [Models.Metadata.Machine.TruripKey] = "REPLACE", // Type
                [Models.Metadata.Machine.VideoKey] = "REPLACE", // Type array
                [Models.Metadata.Machine.YearKey] = "year",
            };
        }

        private static void ValidateHeader(DatHeader datHeader)
        {
            Assert.Equal("author", datHeader.GetStringFieldValue(Models.Metadata.Header.AuthorKey));
            Assert.Equal("merged", datHeader.GetStringFieldValue(Models.Metadata.Header.BiosModeKey));
            Assert.Equal("build", datHeader.GetStringFieldValue(Models.Metadata.Header.BuildKey));
            Assert.Equal("ext", datHeader.GetStringFieldValue(Models.Metadata.Header.CanOpenKey));
            Assert.Equal("category", datHeader.GetStringFieldValue(Models.Metadata.Header.CategoryKey));
            Assert.Equal("comment", datHeader.GetStringFieldValue(Models.Metadata.Header.CommentKey));
            Assert.Equal("date", datHeader.GetStringFieldValue(Models.Metadata.Header.DateKey));
            Assert.Equal("datversion", datHeader.GetStringFieldValue(Models.Metadata.Header.DatVersionKey));
            Assert.True(datHeader.GetBoolFieldValue(Models.Metadata.Header.DebugKey));
            Assert.Equal("description", datHeader.GetStringFieldValue(Models.Metadata.Header.DescriptionKey));
            Assert.Equal("email", datHeader.GetStringFieldValue(Models.Metadata.Header.EmailKey));
            Assert.Equal("emulatorversion", datHeader.GetStringFieldValue(Models.Metadata.Header.EmulatorVersionKey));
            Assert.Equal("merged", datHeader.GetStringFieldValue(Models.Metadata.Header.ForceMergingKey));
            Assert.Equal("required", datHeader.GetStringFieldValue(Models.Metadata.Header.ForceNodumpKey));
            Assert.Equal("zip", datHeader.GetStringFieldValue(Models.Metadata.Header.ForcePackingKey));
            Assert.True(datHeader.GetBoolFieldValue(Models.Metadata.Header.ForceZippingKey));
            Assert.Equal("header", datHeader.GetStringFieldValue(Models.Metadata.Header.HeaderKey));
            Assert.Equal("homepage", datHeader.GetStringFieldValue(Models.Metadata.Header.HomepageKey));
            Assert.Equal("id", datHeader.GetStringFieldValue(Models.Metadata.Header.IdKey));
            Assert.NotNull(datHeader.GetStringFieldValue(Models.Metadata.Header.ImagesKey));
            Assert.Equal("imfolder", datHeader.GetStringFieldValue(Models.Metadata.Header.ImFolderKey));
            Assert.NotNull(datHeader.GetStringFieldValue(Models.Metadata.Header.InfosKey));
            Assert.True(datHeader.GetBoolFieldValue(Models.Metadata.Header.LockBiosModeKey));
            Assert.True(datHeader.GetBoolFieldValue(Models.Metadata.Header.LockRomModeKey));
            Assert.True(datHeader.GetBoolFieldValue(Models.Metadata.Header.LockSampleModeKey));
            Assert.Equal("mameconfig", datHeader.GetStringFieldValue(Models.Metadata.Header.MameConfigKey));
            Assert.Equal("name", datHeader.GetStringFieldValue(Models.Metadata.Header.NameKey));
            Assert.NotNull(datHeader.GetStringFieldValue(Models.Metadata.Header.NewDatKey));
            Assert.Equal("notes", datHeader.GetStringFieldValue(Models.Metadata.Header.NotesKey));
            Assert.Equal("plugin", datHeader.GetStringFieldValue(Models.Metadata.Header.PluginKey));
            Assert.Equal("refname", datHeader.GetStringFieldValue(Models.Metadata.Header.RefNameKey));
            Assert.Equal("merged", datHeader.GetStringFieldValue(Models.Metadata.Header.RomModeKey));
            Assert.Equal("romtitle", datHeader.GetStringFieldValue(Models.Metadata.Header.RomTitleKey));
            Assert.Equal("rootdir", datHeader.GetStringFieldValue(Models.Metadata.Header.RootDirKey));
            Assert.Equal("merged", datHeader.GetStringFieldValue(Models.Metadata.Header.SampleModeKey));
            Assert.Equal("schemalocation", datHeader.GetStringFieldValue(Models.Metadata.Header.SchemaLocationKey));
            Assert.Equal("screenshotsheight", datHeader.GetStringFieldValue(Models.Metadata.Header.ScreenshotsHeightKey));
            Assert.Equal("screenshotsWidth", datHeader.GetStringFieldValue(Models.Metadata.Header.ScreenshotsWidthKey));
            Assert.NotNull(datHeader.GetStringFieldValue(Models.Metadata.Header.SearchKey));
            Assert.Equal("system", datHeader.GetStringFieldValue(Models.Metadata.Header.SystemKey));
            Assert.Equal("timestamp", datHeader.GetStringFieldValue(Models.Metadata.Header.TimestampKey));
            Assert.Equal("type", datHeader.GetStringFieldValue(Models.Metadata.Header.TypeKey));
            Assert.Equal("url", datHeader.GetStringFieldValue(Models.Metadata.Header.UrlKey));
            Assert.Equal("version", datHeader.GetStringFieldValue(Models.Metadata.Header.VersionKey));
        }

        private static void ValidateMachine(DatItems.Machine machine)
        {
            Assert.Equal("board", machine.GetStringFieldValue(Models.Metadata.Machine.BoardKey));
            Assert.Equal("buttons", machine.GetStringFieldValue(Models.Metadata.Machine.ButtonsKey));
            Assert.Equal("category", machine.GetStringFieldValue(Models.Metadata.Machine.CategoryKey));
            Assert.Equal("cloneof", machine.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey));
            Assert.Equal("cloneofid", machine.GetStringFieldValue(Models.Metadata.Machine.CloneOfIdKey));
            Assert.Equal("comment", machine.GetStringFieldValue(Models.Metadata.Machine.CommentKey));
            Assert.Equal("company", machine.GetStringFieldValue(Models.Metadata.Machine.CompanyKey));
            Assert.Equal("control", machine.GetStringFieldValue(Models.Metadata.Machine.ControlKey));
            Assert.Equal("country", machine.GetStringFieldValue(Models.Metadata.Machine.CountryKey));
            Assert.Equal("description", machine.GetStringFieldValue(Models.Metadata.Machine.DescriptionKey));
            Assert.Equal("dirname", machine.GetStringFieldValue(Models.Metadata.Machine.DirNameKey));
            Assert.Equal("displaycount", machine.GetStringFieldValue(Models.Metadata.Machine.DisplayCountKey));
            Assert.Equal("displaytype", machine.GetStringFieldValue(Models.Metadata.Machine.DisplayTypeKey));
            Assert.Equal("duplicateid", machine.GetStringFieldValue(Models.Metadata.Machine.DuplicateIDKey));
            Assert.Equal("emulator", machine.GetStringFieldValue(Models.Metadata.Machine.EmulatorKey));
            Assert.Equal("extra", machine.GetStringFieldValue(Models.Metadata.Machine.ExtraKey));
            Assert.Equal("favorite", machine.GetStringFieldValue(Models.Metadata.Machine.FavoriteKey));
            Assert.Equal("genmsxid", machine.GetStringFieldValue(Models.Metadata.Machine.GenMSXIDKey));
            Assert.Equal("history", machine.GetStringFieldValue(Models.Metadata.Machine.HistoryKey));
            Assert.Equal("id", machine.GetStringFieldValue(Models.Metadata.Machine.IdKey));
            Assert.Equal(ZeroHash.CRC32Str, machine.GetStringFieldValue(Models.Metadata.Machine.Im1CRCKey));
            Assert.Equal(ZeroHash.CRC32Str, machine.GetStringFieldValue(Models.Metadata.Machine.Im2CRCKey));
            Assert.Equal("imagenumber", machine.GetStringFieldValue(Models.Metadata.Machine.ImageNumberKey));
            Assert.Equal("yes", machine.GetStringFieldValue(Models.Metadata.Machine.IsBiosKey));
            Assert.Equal("yes", machine.GetStringFieldValue(Models.Metadata.Machine.IsDeviceKey));
            Assert.Equal("yes", machine.GetStringFieldValue(Models.Metadata.Machine.IsMechanicalKey));
            Assert.Equal("language", machine.GetStringFieldValue(Models.Metadata.Machine.LanguageKey));
            Assert.Equal("location", machine.GetStringFieldValue(Models.Metadata.Machine.LocationKey));
            Assert.Equal("manufacturer", machine.GetStringFieldValue(Models.Metadata.Machine.ManufacturerKey));
            Assert.Equal("name", machine.GetStringFieldValue(Models.Metadata.Machine.NameKey));
            Assert.Equal("notes", machine.GetStringFieldValue(Models.Metadata.Machine.NotesKey));
            Assert.Equal("playedcount", machine.GetStringFieldValue(Models.Metadata.Machine.PlayedCountKey));
            Assert.Equal("playedtime", machine.GetStringFieldValue(Models.Metadata.Machine.PlayedTimeKey));
            Assert.Equal("players", machine.GetStringFieldValue(Models.Metadata.Machine.PlayersKey));
            Assert.Equal("publisher", machine.GetStringFieldValue(Models.Metadata.Machine.PublisherKey));
            Assert.Equal("rebuildto", machine.GetStringFieldValue(Models.Metadata.Machine.RebuildToKey));
            Assert.Equal("releasenumber", machine.GetStringFieldValue(Models.Metadata.Machine.ReleaseNumberKey));
            Assert.Equal("romof", machine.GetStringFieldValue(Models.Metadata.Machine.RomOfKey));
            Assert.Equal("rotation", machine.GetStringFieldValue(Models.Metadata.Machine.RotationKey));
            Assert.Equal("yes", machine.GetStringFieldValue(Models.Metadata.Machine.RunnableKey));
            Assert.Equal("sampleof", machine.GetStringFieldValue(Models.Metadata.Machine.SampleOfKey));
            Assert.Equal("savetype", machine.GetStringFieldValue(Models.Metadata.Machine.SaveTypeKey));
            Assert.Equal("sourcefile", machine.GetStringFieldValue(Models.Metadata.Machine.SourceFileKey));
            Assert.Equal("sourcerom", machine.GetStringFieldValue(Models.Metadata.Machine.SourceRomKey));
            Assert.Equal("status", machine.GetStringFieldValue(Models.Metadata.Machine.StatusKey));
            Assert.Equal("yes", machine.GetStringFieldValue(Models.Metadata.Machine.SupportedKey));
            Assert.Equal("system", machine.GetStringFieldValue(Models.Metadata.Machine.SystemKey));
            Assert.Equal("tags", machine.GetStringFieldValue(Models.Metadata.Machine.TagsKey));
            // Assert.Equal("REPLACE", actualMachine.GetStringFieldValue(Models.Metadata.Machine.TruripKey)); // Type
            Assert.Equal("year", machine.GetStringFieldValue(Models.Metadata.Machine.YearKey));
        }

        private static void ValidateAdjuster(Adjuster? adjuster)
        {
            Assert.NotNull(adjuster);
            Assert.True(adjuster.GetBoolFieldValue(Models.Metadata.Adjuster.DefaultKey));
            Assert.Equal("name", adjuster.GetStringFieldValue(Models.Metadata.Adjuster.NameKey));

            Condition? condition = adjuster.GetFieldValue<Condition>(Models.Metadata.Adjuster.ConditionKey);
            ValidateCondition(condition);
        }

        private static void ValidateArchive(Archive? archive)
        {
            Assert.NotNull(archive);
            Assert.Equal("name", archive.GetStringFieldValue(Models.Metadata.Archive.NameKey));
        }

        private static void ValidateBiosSet(BiosSet? biosSet)
        {
            Assert.NotNull(biosSet);
            Assert.True(biosSet.GetBoolFieldValue(Models.Metadata.BiosSet.DefaultKey));
            Assert.Equal("description", biosSet.GetStringFieldValue(Models.Metadata.BiosSet.DescriptionKey));
            Assert.Equal("name", biosSet.GetStringFieldValue(Models.Metadata.BiosSet.NameKey));
        }

        private static void ValidateChip(Chip? chip)
        {
            Assert.NotNull(chip);
            Assert.Equal(12345, chip.GetInt64FieldValue(Models.Metadata.Chip.ClockKey));
            Assert.Equal("flags", chip.GetStringFieldValue(Models.Metadata.Chip.FlagsKey));
            Assert.Equal("name", chip.GetStringFieldValue(Models.Metadata.Chip.NameKey));
            Assert.True(chip.GetBoolFieldValue(Models.Metadata.Chip.SoundOnlyKey));
            Assert.Equal("tag", chip.GetStringFieldValue(Models.Metadata.Chip.TagKey));
            Assert.Equal("cpu", chip.GetStringFieldValue(Models.Metadata.Chip.ChipTypeKey));
        }

        private static void ValidateCondition(Condition? condition)
        {
            Assert.NotNull(condition);
            Assert.Equal("value", condition.GetStringFieldValue(Models.Metadata.Condition.ValueKey));
            Assert.Equal("mask", condition.GetStringFieldValue(Models.Metadata.Condition.MaskKey));
            Assert.Equal("eq", condition.GetStringFieldValue(Models.Metadata.Condition.RelationKey));
            Assert.Equal("tag", condition.GetStringFieldValue(Models.Metadata.Condition.TagKey));
        }

        private static void ValidateConfiguration(Configuration? configuration)
        {
            Assert.NotNull(configuration);
            Assert.Equal("mask", configuration.GetStringFieldValue(Models.Metadata.Configuration.MaskKey));
            Assert.Equal("name", configuration.GetStringFieldValue(Models.Metadata.Configuration.NameKey));
            Assert.Equal("tag", configuration.GetStringFieldValue(Models.Metadata.Configuration.TagKey));

            Condition? condition = configuration.GetFieldValue<Condition>(Models.Metadata.Configuration.ConditionKey);
            ValidateCondition(condition);

            ConfLocation[]? confLocations = configuration.GetFieldValue<ConfLocation[]>(Models.Metadata.Configuration.ConfLocationKey);
            Assert.NotNull(confLocations);
            ConfLocation? confLocation = Assert.Single(confLocations);
            ValidateConfLocation(confLocation);

            ConfSetting[]? confSettings = configuration.GetFieldValue<ConfSetting[]>(Models.Metadata.Configuration.ConfSettingKey);
            Assert.NotNull(confSettings);
            ConfSetting? confSetting = Assert.Single(confSettings);
            ValidateConfSetting(confSetting);
        }

        private static void ValidateConfLocation(ConfLocation? confLocation)
        {
            Assert.NotNull(confLocation);
            Assert.True(confLocation.GetBoolFieldValue(Models.Metadata.ConfLocation.InvertedKey));
            Assert.Equal("name", confLocation.GetStringFieldValue(Models.Metadata.ConfLocation.NameKey));
            Assert.Equal("number", confLocation.GetStringFieldValue(Models.Metadata.ConfLocation.NumberKey));
        }

        private static void ValidateConfSetting(ConfSetting? confSetting)
        {
            Assert.NotNull(confSetting);
            Assert.True(confSetting.GetBoolFieldValue(Models.Metadata.ConfSetting.DefaultKey));
            Assert.Equal("name", confSetting.GetStringFieldValue(Models.Metadata.ConfSetting.NameKey));
            Assert.Equal("value", confSetting.GetStringFieldValue(Models.Metadata.ConfSetting.ValueKey));

            Condition? condition = confSetting.GetFieldValue<Condition>(Models.Metadata.ConfSetting.ConditionKey);
            ValidateCondition(condition);
        }

        private static void ValidateControl(Control? control)
        {
            Assert.NotNull(control);
            Assert.Equal(12345, control.GetInt64FieldValue(Models.Metadata.Control.ButtonsKey));
            Assert.Equal(12345, control.GetInt64FieldValue(Models.Metadata.Control.KeyDeltaKey));
            Assert.Equal(12345, control.GetInt64FieldValue(Models.Metadata.Control.MaximumKey));
            Assert.Equal(12345, control.GetInt64FieldValue(Models.Metadata.Control.MinimumKey));
            Assert.Equal(12345, control.GetInt64FieldValue(Models.Metadata.Control.PlayerKey));
            Assert.Equal(12345, control.GetInt64FieldValue(Models.Metadata.Control.ReqButtonsKey));
            Assert.True(control.GetBoolFieldValue(Models.Metadata.Control.ReverseKey));
            Assert.Equal(12345, control.GetInt64FieldValue(Models.Metadata.Control.SensitivityKey));
            Assert.Equal("lightgun", control.GetStringFieldValue(Models.Metadata.Control.ControlTypeKey));
            Assert.Equal("ways", control.GetStringFieldValue(Models.Metadata.Control.WaysKey));
            Assert.Equal("ways2", control.GetStringFieldValue(Models.Metadata.Control.Ways2Key));
            Assert.Equal("ways3", control.GetStringFieldValue(Models.Metadata.Control.Ways3Key));
        }

        private static void ValidateDevice(Device? device)
        {
            Assert.NotNull(device);
            Assert.Equal("fixedimage", device.GetStringFieldValue(Models.Metadata.Device.FixedImageKey));
            Assert.Equal("interface", device.GetStringFieldValue(Models.Metadata.Device.InterfaceKey));
            Assert.Equal(1, device.GetInt64FieldValue(Models.Metadata.Device.MandatoryKey));
            Assert.Equal("tag", device.GetStringFieldValue(Models.Metadata.Device.TagKey));
            Assert.Equal("punchtape", device.GetStringFieldValue(Models.Metadata.Device.DeviceTypeKey));

            Extension[]? extensions = device.GetFieldValue<Extension[]>(Models.Metadata.Device.ExtensionKey);
            Assert.NotNull(extensions);
            Extension? extension = Assert.Single(extensions);
            ValidateExtension(extension);

            Instance? instance = device.GetFieldValue<Instance>(Models.Metadata.Device.InstanceKey);
            ValidateInstance(instance);
        }

        private static void ValidateDeviceRef(DeviceRef? deviceRef)
        {
            Assert.NotNull(deviceRef);
            Assert.Equal("name", deviceRef.GetStringFieldValue(Models.Metadata.DeviceRef.NameKey));
        }

        private static void ValidateDipLocation(DipLocation? dipLocation)
        {
            Assert.NotNull(dipLocation);
            Assert.True(dipLocation.GetBoolFieldValue(Models.Metadata.DipLocation.InvertedKey));
            Assert.Equal("name", dipLocation.GetStringFieldValue(Models.Metadata.DipLocation.NameKey));
            Assert.Equal("number", dipLocation.GetStringFieldValue(Models.Metadata.DipLocation.NumberKey));
        }

        private static void ValidateDipSwitch(DipSwitch? dipSwitch)
        {
            Assert.NotNull(dipSwitch);
            Assert.True(dipSwitch.GetBoolFieldValue(Models.Metadata.DipSwitch.DefaultKey));
            Assert.Equal("mask", dipSwitch.GetStringFieldValue(Models.Metadata.DipSwitch.MaskKey));
            Assert.Equal("name", dipSwitch.GetStringFieldValue(Models.Metadata.DipSwitch.NameKey));
            Assert.Equal("tag", dipSwitch.GetStringFieldValue(Models.Metadata.DipSwitch.TagKey));

            Condition? condition = dipSwitch.GetFieldValue<Condition>(Models.Metadata.DipSwitch.ConditionKey);
            ValidateCondition(condition);

            DipLocation[]? dipLocations = dipSwitch.GetFieldValue<DipLocation[]>(Models.Metadata.DipSwitch.DipLocationKey);
            Assert.NotNull(dipLocations);
            DipLocation? dipLocation = Assert.Single(dipLocations);
            ValidateDipLocation(dipLocation);

            DipValue[]? dipValues = dipSwitch.GetFieldValue<DipValue[]>(Models.Metadata.DipSwitch.DipValueKey);
            Assert.NotNull(dipValues);
            DipValue? dipValue = Assert.Single(dipValues);
            ValidateDipValue(dipValue);

            string[]? entries = dipSwitch.GetStringArrayFieldValue(Models.Metadata.DipSwitch.EntryKey);
            Assert.NotNull(entries);
            string entry = Assert.Single(entries);
            Assert.Equal("entry", entry);
        }

        private static void ValidateDipValue(DipValue? dipValue)
        {
            Assert.NotNull(dipValue);
            Assert.True(dipValue.GetBoolFieldValue(Models.Metadata.DipValue.DefaultKey));
            Assert.Equal("name", dipValue.GetStringFieldValue(Models.Metadata.DipValue.NameKey));
            Assert.Equal("value", dipValue.GetStringFieldValue(Models.Metadata.DipValue.ValueKey));

            Condition? condition = dipValue.GetFieldValue<Condition>(Models.Metadata.DipValue.ConditionKey);
            ValidateCondition(condition);
        }

        private static void ValidateDisk(Disk? disk)
        {
            Assert.NotNull(disk);
            Assert.Equal("flags", disk.GetStringFieldValue(Models.Metadata.Disk.FlagsKey));
            Assert.Equal("index", disk.GetStringFieldValue(Models.Metadata.Disk.IndexKey));
            Assert.Equal(ZeroHash.MD5Str, disk.GetStringFieldValue(Models.Metadata.Disk.MD5Key));
            Assert.Equal("merge", disk.GetStringFieldValue(Models.Metadata.Disk.MergeKey));
            Assert.Equal("name", disk.GetStringFieldValue(Models.Metadata.Disk.NameKey));
            Assert.True(disk.GetBoolFieldValue(Models.Metadata.Disk.OptionalKey));
            Assert.Equal("region", disk.GetStringFieldValue(Models.Metadata.Disk.RegionKey));
            Assert.Equal(ZeroHash.SHA1Str, disk.GetStringFieldValue(Models.Metadata.Disk.SHA1Key));
            Assert.True(disk.GetBoolFieldValue(Models.Metadata.Disk.WritableKey));
        }

        private static void ValidateDisplay(Display? display)
        {
            Assert.NotNull(display);
            Assert.True(display.GetBoolFieldValue(Models.Metadata.Display.FlipXKey));
            Assert.Equal(12345, display.GetInt64FieldValue(Models.Metadata.Display.HBEndKey));
            Assert.Equal(12345, display.GetInt64FieldValue(Models.Metadata.Display.HBStartKey));
            Assert.Equal(12345, display.GetInt64FieldValue(Models.Metadata.Display.HeightKey));
            Assert.Equal(12345, display.GetInt64FieldValue(Models.Metadata.Display.HTotalKey));
            Assert.Equal(12345, display.GetInt64FieldValue(Models.Metadata.Display.PixClockKey));
            Assert.Equal(12345, display.GetInt64FieldValue(Models.Metadata.Display.RefreshKey));
            Assert.Equal(90, display.GetInt64FieldValue(Models.Metadata.Display.RotateKey));
            Assert.Equal("tag", display.GetStringFieldValue(Models.Metadata.Display.TagKey));
            Assert.Equal("vector", display.GetStringFieldValue(Models.Metadata.Display.DisplayTypeKey));
            Assert.Equal(12345, display.GetInt64FieldValue(Models.Metadata.Display.VBEndKey));
            Assert.Equal(12345, display.GetInt64FieldValue(Models.Metadata.Display.VBStartKey));
            Assert.Equal(12345, display.GetInt64FieldValue(Models.Metadata.Display.VTotalKey));
            Assert.Equal(12345, display.GetInt64FieldValue(Models.Metadata.Display.WidthKey));
        }

        private static void ValidateDriver(Driver? driver)
        {
            Assert.NotNull(driver);
            Assert.Equal("plain", driver.GetStringFieldValue(Models.Metadata.Driver.BlitKey));
            Assert.Equal("good", driver.GetStringFieldValue(Models.Metadata.Driver.CocktailKey));
            Assert.Equal("good", driver.GetStringFieldValue(Models.Metadata.Driver.ColorKey));
            Assert.Equal("good", driver.GetStringFieldValue(Models.Metadata.Driver.EmulationKey));
            Assert.True(driver.GetBoolFieldValue(Models.Metadata.Driver.IncompleteKey));
            Assert.True(driver.GetBoolFieldValue(Models.Metadata.Driver.NoSoundHardwareKey));
            Assert.Equal("pallettesize", driver.GetStringFieldValue(Models.Metadata.Driver.PaletteSizeKey));
            Assert.True(driver.GetBoolFieldValue(Models.Metadata.Driver.RequiresArtworkKey));
            Assert.Equal("supported", driver.GetStringFieldValue(Models.Metadata.Driver.SaveStateKey));
            Assert.Equal("good", driver.GetStringFieldValue(Models.Metadata.Driver.SoundKey));
            Assert.Equal("good", driver.GetStringFieldValue(Models.Metadata.Driver.StatusKey));
            Assert.True(driver.GetBoolFieldValue(Models.Metadata.Driver.UnofficialKey));
        }

        private static void ValidateExtension(Extension? extension)
        {
            Assert.NotNull(extension);
            Assert.Equal("name", extension.GetStringFieldValue(Models.Metadata.Extension.NameKey));
        }

        private static void ValidateFeature(Feature? feature)
        {
            Assert.NotNull(feature);
            Assert.Equal("name", feature.GetStringFieldValue(Models.Metadata.Feature.NameKey));
            Assert.Equal("imperfect", feature.GetStringFieldValue(Models.Metadata.Feature.OverallKey));
            Assert.Equal("imperfect", feature.GetStringFieldValue(Models.Metadata.Feature.StatusKey));
            Assert.Equal("protection", feature.GetStringFieldValue(Models.Metadata.Feature.FeatureTypeKey));
            Assert.Equal("value", feature.GetStringFieldValue(Models.Metadata.Feature.ValueKey));
        }

        private static void ValidateInfo(Info? info)
        {
            Assert.NotNull(info);
            Assert.Equal("name", info.GetStringFieldValue(Models.Metadata.Info.NameKey));
            Assert.Equal("value", info.GetStringFieldValue(Models.Metadata.Info.ValueKey));
        }

        private static void ValidateInput(Input? input)
        {
            Assert.NotNull(input);
            Assert.Equal(12345, input.GetInt64FieldValue(Models.Metadata.Input.ButtonsKey));
            Assert.Equal(12345, input.GetInt64FieldValue(Models.Metadata.Input.CoinsKey));
            Assert.Equal(12345, input.GetInt64FieldValue(Models.Metadata.Input.PlayersKey));
            Assert.True(input.GetBoolFieldValue(Models.Metadata.Input.ServiceKey));
            Assert.True(input.GetBoolFieldValue(Models.Metadata.Input.TiltKey));

            Control[]? controls = input.GetFieldValue<Control[]>(Models.Metadata.Input.ControlKey);
            Assert.NotNull(controls);
            Control? control = Assert.Single(controls);
            ValidateControl(control);
        }

        private static void ValidateInstance(Instance? instance)
        {
            Assert.NotNull(instance);
            Assert.Equal("briefname", instance.GetStringFieldValue(Models.Metadata.Instance.BriefNameKey));
            Assert.Equal("name", instance.GetStringFieldValue(Models.Metadata.Instance.NameKey));
        }

        private static void ValidateMedia(Media? media)
        {
            Assert.NotNull(media);
            Assert.Equal(ZeroHash.MD5Str, media.GetStringFieldValue(Models.Metadata.Media.MD5Key));
            Assert.Equal("name", media.GetStringFieldValue(Models.Metadata.Media.NameKey));
            Assert.Equal(ZeroHash.SHA1Str, media.GetStringFieldValue(Models.Metadata.Media.SHA1Key));
            Assert.Equal(ZeroHash.SHA256Str, media.GetStringFieldValue(Models.Metadata.Media.SHA256Key));
            Assert.Equal(ZeroHash.SpamSumStr, media.GetStringFieldValue(Models.Metadata.Media.SpamSumKey));
        }

        #endregion
    }
}