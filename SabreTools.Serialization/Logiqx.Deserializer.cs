using System.Linq;
using SabreTools.Models.Logiqx;

namespace SabreTools.Serialization
{
    /// <summary>
    /// XML deserializer for Logiqx-derived metadata files
    /// </summary>
    public partial class Logiqx : XmlSerializer<Datafile>
    {
        #region Internal

        /// <summary>
        /// Convert from <cref="Models.Internal.MetadataFile"/> to <cref="Models.Logiqx.Datafile"/>
        /// </summary>
        public static Datafile? ConvertFromInternalModel(Models.Internal.MetadataFile? item, bool game = false)
        {
            if (item == null)
                return null;

            var datafile = new Datafile
            {
                Build = item.ReadString(Models.Internal.Header.BuildKey),
                Debug = item.ReadString(Models.Internal.Header.DebugKey),
                SchemaLocation = item.ReadString(Models.Internal.Header.SchemaLocationKey),
            };

            var header = item.Read<Models.Internal.Header>(Models.Internal.MetadataFile.HeaderKey);
            if (header != null)
                datafile.Header = ConvertHeaderFromInternalModel(header);

            // TODO: Handle Dir items
            var machines = item.Read<Models.Internal.Machine[]>(Models.Internal.MetadataFile.MachineKey);
            if (machines != null && machines.Any())
                datafile.Game = machines.Select(machine => ConvertMachineFromInternalModel(machine, game)).ToArray();
            
            return datafile;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Header"/> to <cref="Models.Logiqx.Header"/>
        /// </summary>
        private static Header ConvertHeaderFromInternalModel(Models.Internal.Header item)
        {
            if (item == null)
                return null;

            var header = new Header
            {
                Id = item.ReadString(Models.Internal.Header.IdKey),
                Name = item.ReadString(Models.Internal.Header.NameKey),
                Description = item.ReadString(Models.Internal.Header.DescriptionKey),
                RootDir = item.ReadString(Models.Internal.Header.RootDirKey),
                Category = item.ReadString(Models.Internal.Header.CategoryKey),
                Version = item.ReadString(Models.Internal.Header.VersionKey),
                Date = item.ReadString(Models.Internal.Header.DateKey),
                Author = item.ReadString(Models.Internal.Header.AuthorKey),
                Email = item.ReadString(Models.Internal.Header.EmailKey),
                Homepage = item.ReadString(Models.Internal.Header.HomepageKey),
                Url = item.ReadString(Models.Internal.Header.UrlKey),
                Comment = item.ReadString(Models.Internal.Header.CommentKey),
                Type = item.ReadString(Models.Internal.Header.TypeKey),
            };

            if (item.ContainsKey(Models.Internal.Header.HeaderKey)
                || item.ContainsKey(Models.Internal.Header.ForceMergingKey)
                || item.ContainsKey(Models.Internal.Header.ForceNodumpKey)
                || item.ContainsKey(Models.Internal.Header.ForcePackingKey))
            {
                header.ClrMamePro = new Models.Logiqx.ClrMamePro
                {
                    Header = item.ReadString(Models.Internal.Header.HeaderKey),
                    ForceMerging = item.ReadString(Models.Internal.Header.ForceMergingKey),
                    ForceNodump = item.ReadString(Models.Internal.Header.ForceNodumpKey),
                    ForcePacking = item.ReadString(Models.Internal.Header.ForcePackingKey),
                };
            }

            if (item.ContainsKey(Models.Internal.Header.PluginKey)
                || item.ContainsKey(Models.Internal.Header.RomModeKey)
                || item.ContainsKey(Models.Internal.Header.BiosModeKey)
                || item.ContainsKey(Models.Internal.Header.SampleModeKey)
                || item.ContainsKey(Models.Internal.Header.LockRomModeKey)
                || item.ContainsKey(Models.Internal.Header.LockBiosModeKey)
                || item.ContainsKey(Models.Internal.Header.LockSampleModeKey))
            {
                header.RomCenter = new Models.Logiqx.RomCenter
                {
                    Plugin = item.ReadString(Models.Internal.Header.PluginKey),
                    RomMode = item.ReadString(Models.Internal.Header.RomModeKey),
                    BiosMode = item.ReadString(Models.Internal.Header.BiosModeKey),
                    SampleMode = item.ReadString(Models.Internal.Header.SampleModeKey),
                    LockRomMode = item.ReadString(Models.Internal.Header.LockRomModeKey),
                    LockBiosMode = item.ReadString(Models.Internal.Header.LockBiosModeKey),
                    LockSampleMode = item.ReadString(Models.Internal.Header.LockSampleModeKey),
                };
            }

            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Machine"/> to <cref="Models.Logiqx.GameBase"/>
        /// </summary>
        private static GameBase? ConvertMachineFromInternalModel(Models.Internal.Machine? item, bool game = false)
        {
            if (item == null)
                return null;

            GameBase gameBase = game ? new Game() : new Machine();

            gameBase.Name = item.ReadString(Models.Internal.Machine.NameKey);
            gameBase.SourceFile = item.ReadString(Models.Internal.Machine.SourceFileKey);
            gameBase.IsBios = item.ReadString(Models.Internal.Machine.IsBiosKey);
            gameBase.IsDevice = item.ReadString(Models.Internal.Machine.IsDeviceKey);
            gameBase.IsMechanical = item.ReadString(Models.Internal.Machine.IsMechanicalKey);
            gameBase.CloneOf = item.ReadString(Models.Internal.Machine.CloneOfKey);
            gameBase.RomOf = item.ReadString(Models.Internal.Machine.RomOfKey);
            gameBase.SampleOf = item.ReadString(Models.Internal.Machine.SampleOfKey);
            gameBase.Board = item.ReadString(Models.Internal.Machine.BoardKey);
            gameBase.RebuildTo = item.ReadString(Models.Internal.Machine.RebuildToKey);
            gameBase.Id = item.ReadString(Models.Internal.Machine.IdKey);
            gameBase.CloneOfId = item.ReadString(Models.Internal.Machine.CloneOfIdKey);
            gameBase.Runnable = item.ReadString(Models.Internal.Machine.RunnableKey);
            gameBase.Comment = item.ReadStringArray(Models.Internal.Machine.CommentKey);
            gameBase.Description = item.ReadString(Models.Internal.Machine.DescriptionKey);
            gameBase.Year = item.ReadString(Models.Internal.Machine.YearKey);
            gameBase.Manufacturer = item.ReadString(Models.Internal.Machine.ManufacturerKey);
            gameBase.Publisher = item.ReadString(Models.Internal.Machine.PublisherKey);
            gameBase.Category = item.ReadStringArray(Models.Internal.Machine.CategoryKey);

            var trurip = item.Read<Trurip>(Models.Internal.Machine.TruripKey);
            gameBase.Trurip = trurip;

            var releases = item.Read<Models.Internal.Release[]>(Models.Internal.Machine.ReleaseKey);
            gameBase.Release = releases?.Select(ConvertFromInternalModel)?.ToArray();

            var biosSets = item.Read<Models.Internal.BiosSet[]>(Models.Internal.Machine.BiosSetKey);
            gameBase.BiosSet = biosSets?.Select(ConvertFromInternalModel)?.ToArray();

            var roms = item.Read<Models.Internal.Rom[]>(Models.Internal.Machine.RomKey);
            gameBase.Rom = roms?.Select(ConvertFromInternalModel)?.ToArray();

            var disks = item.Read<Models.Internal.Disk[]>(Models.Internal.Machine.DiskKey);
            gameBase.Disk = disks?.Select(ConvertFromInternalModel)?.ToArray();

            var medias = item.Read<Models.Internal.Media[]>(Models.Internal.Machine.MediaKey);
            gameBase.Media = medias?.Select(ConvertFromInternalModel)?.ToArray();

            var deviceRefs = item.Read<Models.Internal.DeviceRef[]>(Models.Internal.Machine.DeviceRefKey);
            gameBase.DeviceRef = deviceRefs?.Select(ConvertFromInternalModel)?.ToArray();

            var samples = item.Read<Models.Internal.Sample[]>(Models.Internal.Machine.SampleKey);
            gameBase.Sample = samples?.Select(ConvertFromInternalModel)?.ToArray();

            var archives = item.Read<Models.Internal.Archive[]>(Models.Internal.Machine.ArchiveKey);
            gameBase.Archive = archives?.Select(ConvertFromInternalModel)?.ToArray();

            var drivers = item.Read<Models.Internal.Driver[]>(Models.Internal.Machine.DriverKey);
            gameBase.Driver = drivers?.Select(ConvertFromInternalModel)?.ToArray();

            var softwareLists = item.Read<Models.Internal.SoftwareList[]>(Models.Internal.Machine.SoftwareListKey);
            gameBase.SoftwareList = softwareLists?.Select(ConvertFromInternalModel)?.ToArray();

            return gameBase;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Archive"/> to <cref="Models.Logiqx.Archive"/>
        /// </summary>
        private static Archive? ConvertFromInternalModel(Models.Internal.Archive? item)
        {
            if (item == null)
                return null;

            var archive = new Archive
            {
                Name = item.ReadString(Models.Internal.Archive.NameKey),
            };
            return archive;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.BiosSet"/> to <cref="Models.Logiqx.BiosSet"/>
        /// </summary>
        private static BiosSet? ConvertFromInternalModel(Models.Internal.BiosSet? item)
        {
            if (item == null)
                return null;

            var biosset = new BiosSet
            {
                Name = item.ReadString(Models.Internal.BiosSet.NameKey),
                Description = item.ReadString(Models.Internal.BiosSet.DescriptionKey),
                Default = item.ReadString(Models.Internal.BiosSet.DefaultKey),
            };
            return biosset;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.DeviceRef"/> to <cref="Models.Logiqx.DeviceRef"/>
        /// </summary>
        private static DeviceRef? ConvertFromInternalModel(Models.Internal.DeviceRef? item)
        {
            if (item == null)
                return null;

            var deviceRef = new DeviceRef
            {
                Name = item.ReadString(Models.Internal.DipSwitch.NameKey),
            };
            return deviceRef;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Disk"/> to <cref="Models.Logiqx.Disk"/>
        /// </summary>
        private static Disk? ConvertFromInternalModel(Models.Internal.Disk? item)
        {
            if (item == null)
                return null;

            var disk = new Disk
            {
                Name = item.ReadString(Models.Internal.Disk.NameKey),
                MD5 = item.ReadString(Models.Internal.Disk.MD5Key),
                SHA1 = item.ReadString(Models.Internal.Disk.SHA1Key),
                Merge = item.ReadString(Models.Internal.Disk.MergeKey),
                Status = item.ReadString(Models.Internal.Disk.StatusKey),
                Region = item.ReadString(Models.Internal.Disk.RegionKey),
            };
            return disk;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Driver"/> to <cref="Models.Logiqx.Driver"/>
        /// </summary>
        private static Driver? ConvertFromInternalModel(Models.Internal.Driver? item)
        {
            if (item == null)
                return null;

            var driver = new Driver
            {
                Status = item.ReadString(Models.Internal.Driver.StatusKey),
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
        /// Convert from <cref="Models.Internal.Media"/> to <cref="Models.Logiqx.Media"/>
        /// </summary>
        private static Media? ConvertFromInternalModel(Models.Internal.Media? item)
        {
            if (item == null)
                return null;

            var media = new Media
            {
                Name = item.ReadString(Models.Internal.Media.NameKey),
                MD5 = item.ReadString(Models.Internal.Media.MD5Key),
                SHA1 = item.ReadString(Models.Internal.Media.SHA1Key),
                SHA256 = item.ReadString(Models.Internal.Media.SHA256Key),
                SpamSum = item.ReadString(Models.Internal.Media.SpamSumKey),
            };
            return media;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Release"/> to <cref="Models.Logiqx.Release"/>
        /// </summary>
        private static Release? ConvertFromInternalModel(Models.Internal.Release? item)
        {
            if (item == null)
                return null;

            var release = new Release
            {
                Name = item.ReadString(Models.Internal.Release.NameKey),
                Region = item.ReadString(Models.Internal.Release.RegionKey),
                Language = item.ReadString(Models.Internal.Release.LanguageKey),
                Date = item.ReadString(Models.Internal.Release.DateKey),
                Default = item.ReadString(Models.Internal.Release.DefaultKey),
            };
            return release;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.Logiqx.Rom"/>
        /// </summary>
        private static Rom? ConvertFromInternalModel(Models.Internal.Rom? item)
        {
            if (item == null)
                return null;

            var rom = new Rom
            {
                Name = item.ReadString(Models.Internal.Rom.NameKey),
                Size = item.ReadString(Models.Internal.Rom.SizeKey),
                CRC = item.ReadString(Models.Internal.Rom.CRCKey),
                MD5 = item.ReadString(Models.Internal.Rom.MD5Key),
                SHA1 = item.ReadString(Models.Internal.Rom.SHA1Key),
                SHA256 = item.ReadString(Models.Internal.Rom.SHA256Key),
                SHA384 = item.ReadString(Models.Internal.Rom.SHA384Key),
                SHA512 = item.ReadString(Models.Internal.Rom.SHA512Key),
                SpamSum = item.ReadString(Models.Internal.Rom.SpamSumKey),
                xxHash364 = item.ReadString(Models.Internal.Rom.xxHash364Key),
                xxHash3128 = item.ReadString(Models.Internal.Rom.xxHash3128Key),
                Merge = item.ReadString(Models.Internal.Rom.MergeKey),
                Status = item.ReadString(Models.Internal.Rom.StatusKey),
                Serial = item.ReadString(Models.Internal.Rom.SerialKey),
                Header = item.ReadString(Models.Internal.Rom.HeaderKey),
                Date = item.ReadString(Models.Internal.Rom.DateKey),
                Inverted = item.ReadString(Models.Internal.Rom.InvertedKey),
                MIA = item.ReadString(Models.Internal.Rom.MIAKey),
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Sample"/> to <cref="Models.Logiqx.Sample"/>
        /// </summary>
        private static Sample? ConvertFromInternalModel(Models.Internal.Sample? item)
        {
            if (item == null)
                return null;

            var sample = new Sample
            {
                Name = item.ReadString(Models.Internal.Sample.NameKey),
            };
            return sample;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.SoftwareList"/> to <cref="Models.Logiqx.SoftwareList"/>
        /// </summary>
        private static SoftwareList? ConvertFromInternalModel(Models.Internal.SoftwareList? item)
        {
            if (item == null)
                return null;

            var softwareList = new SoftwareList
            {
                Tag = item.ReadString(Models.Internal.SoftwareList.TagKey),
                Name = item.ReadString(Models.Internal.SoftwareList.NameKey),
                Status = item.ReadString(Models.Internal.SoftwareList.StatusKey),
                Filter = item.ReadString(Models.Internal.SoftwareList.FilterKey),
            };
            return softwareList;
        }

        #endregion
    }
}