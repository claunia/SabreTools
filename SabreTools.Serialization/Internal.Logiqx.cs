using System.Collections.Generic;
using System.Linq;
using SabreTools.Models.Internal;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for Logiqx models to internal structure
    /// </summary>
    public partial class Internal
    {
        #region Serialize

        /// <summary>
        /// Convert from <cref="Models.Logiqx.Datafile"/> to <cref="MetadataFile"/>
        /// </summary>
        public static MetadataFile ConvertFromLogiqx(Models.Logiqx.Datafile item)
        {
            var metadataFile = new MetadataFile
            {
                [MetadataFile.HeaderKey] = ConvertHeaderFromLogiqx(item),
            };

            // TODO: Handle Dir items
            if (item?.Game != null && item.Game.Any())
                metadataFile[MetadataFile.MachineKey] = item.Game.Select(ConvertMachineFromLogiqx).ToArray();

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.Logiqx.Datafile"/> to <cref="Header"/>
        /// </summary>
        private static Header ConvertHeaderFromLogiqx(Models.Logiqx.Datafile item)
        {
            var header = ConvertHeaderFromLogiqx(item.Header);

            header[Header.BuildKey] = item.Build;
            header[Header.DebugKey] = item.Debug;
            header[Header.SchemaLocationKey] = item.SchemaLocation;

            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.Logiqx.Header"/> to <cref="Header"/>
        /// </summary>
        private static Header ConvertHeaderFromLogiqx(Models.Logiqx.Header item)
        {
            var header = new Header
            {
                [Header.IdKey] = item.Id,
                [Header.NameKey] = item.Name,
                [Header.DescriptionKey] = item.Description,
                [Header.RootDirKey] = item.RootDir,
                [Header.CategoryKey] = item.Category,
                [Header.VersionKey] = item.Version,
                [Header.DateKey] = item.Date,
                [Header.AuthorKey] = item.Author,
                [Header.EmailKey] = item.Email,
                [Header.HomepageKey] = item.Homepage,
                [Header.UrlKey] = item.Url,
                [Header.CommentKey] = item.Comment,
                [Header.TypeKey] = item.Type,
            };

            if (item.ClrMamePro != null)
            {
                header[Header.HeaderKey] = item.ClrMamePro.Header;
                header[Header.ForceMergingKey] = item.ClrMamePro.ForceMerging;
                header[Header.ForceNodumpKey] = item.ClrMamePro.ForceNodump;
                header[Header.ForcePackingKey] = item.ClrMamePro.ForcePacking;
            }

            if (item.RomCenter != null)
            {
                header[Header.PluginKey] = item.RomCenter.Plugin;
                header[Header.RomModeKey] = item.RomCenter.RomMode;
                header[Header.BiosModeKey] = item.RomCenter.BiosMode;
                header[Header.SampleModeKey] = item.RomCenter.SampleMode;
                header[Header.LockRomModeKey] = item.RomCenter.LockRomMode;
                header[Header.LockBiosModeKey] = item.RomCenter.LockBiosMode;
                header[Header.LockSampleModeKey] = item.RomCenter.LockSampleMode;
            }

            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.Logiqx.GameBase"/> to <cref="Machine"/>
        /// </summary>
        private static Machine ConvertMachineFromLogiqx(Models.Logiqx.GameBase item)
        {
            var machine = new Machine
            {
                [Machine.NameKey] = item.Name,
                [Machine.SourceFileKey] = item.SourceFile,
                [Machine.IsBiosKey] = item.IsBios,
                [Machine.IsDeviceKey] = item.IsDevice,
                [Machine.IsMechanicalKey] = item.IsMechanical,
                [Machine.CloneOfKey] = item.CloneOf,
                [Machine.RomOfKey] = item.RomOf,
                [Machine.SampleOfKey] = item.SampleOf,
                [Machine.BoardKey] = item.Board,
                [Machine.RebuildToKey] = item.RebuildTo,
                [Machine.IdKey] = item.Id,
                [Machine.CloneOfIdKey] = item.CloneOfId,
                [Machine.RunnableKey] = item.Runnable,
                [Machine.CommentKey] = item.Comment,
                [Machine.DescriptionKey] = item.Description,
                [Machine.YearKey] = item.Year,
                [Machine.ManufacturerKey] = item.Manufacturer,
                [Machine.PublisherKey] = item.Publisher,
                [Machine.CategoryKey] = item.Category,
                [Machine.TruripKey] = item.Trurip,
            };

            if (item.Release != null && item.Release.Any())
            {
                var releases = new List<Release>();
                foreach (var release in item.Release)
                {
                    releases.Add(ConvertFromLogiqx(release));
                }
                machine[Machine.ReleaseKey] = releases.ToArray();
            }

            if (item.BiosSet != null && item.BiosSet.Any())
            {
                var biosSets = new List<BiosSet>();
                foreach (var biosSet in item.BiosSet)
                {
                    biosSets.Add(ConvertFromLogiqx(biosSet));
                }
                machine[Machine.BiosSetKey] = biosSets.ToArray();
            }

            if (item.Rom != null && item.Rom.Any())
            {
                var roms = new List<Rom>();
                foreach (var rom in item.Rom)
                {
                    roms.Add(ConvertFromLogiqx(rom));
                }
                machine[Machine.RomKey] = roms.ToArray();
            }

            if (item.Disk != null && item.Disk.Any())
            {
                var disks = new List<Disk>();
                foreach (var disk in item.Disk)
                {
                    disks.Add(ConvertFromLogiqx(disk));
                }
                machine[Machine.DiskKey] = disks.ToArray();
            }

            if (item.Media != null && item.Media.Any())
            {
                var medias = new List<Media>();
                foreach (var media in item.Media)
                {
                    medias.Add(ConvertFromLogiqx(media));
                }
                machine[Machine.MediaKey] = medias.ToArray();
            }

            if (item.DeviceRef != null && item.DeviceRef.Any())
            {
                var deviceRefs = new List<DeviceRef>();
                foreach (var deviceRef in item.DeviceRef)
                {
                    deviceRefs.Add(ConvertFromLogiqx(deviceRef));
                }
                machine[Machine.DeviceRefKey] = deviceRefs.ToArray();
            }

            if (item.Sample != null && item.Sample.Any())
            {
                var samples = new List<Sample>();
                foreach (var sample in item.Sample)
                {
                    samples.Add(ConvertFromLogiqx(sample));
                }
                machine[Machine.SampleKey] = samples.ToArray();
            }

            if (item.Archive != null && item.Archive.Any())
            {
                var archives = new List<Archive>();
                foreach (var archive in item.Archive)
                {
                    archives.Add(ConvertFromLogiqx(archive));
                }
                machine[Machine.ArchiveKey] = archives.ToArray();
            }

            if (item.Driver != null && item.Driver.Any())
            {
                var drivers = new List<Driver>();
                foreach (var driver in item.Driver)
                {
                    drivers.Add(ConvertFromLogiqx(driver));
                }
                machine[Machine.DriverKey] = drivers.ToArray();
            }

            if (item.SoftwareList != null && item.SoftwareList.Any())
            {
                var softwareLists = new List<SoftwareList>();
                foreach (var softwareList in item.SoftwareList)
                {
                    softwareLists.Add(ConvertFromLogiqx(softwareList));
                }
                machine[Machine.SoftwareListKey] = softwareLists.ToArray();
            }

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.Logiqx.Archive"/> to <cref="Archive"/>
        /// </summary>
        private static Archive ConvertFromLogiqx(Models.Logiqx.Archive item)
        {
            var archive = new Archive
            {
                [Archive.NameKey] = item.Name,
            };
            return archive;
        }

        /// <summary>
        /// Convert from <cref="Models.Logiqx.BiosSet"/> to <cref="BiosSet"/>
        /// </summary>
        private static BiosSet ConvertFromLogiqx(Models.Logiqx.BiosSet item)
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
        /// Convert from <cref="Models.Logiqx.DeviceRef"/> to <cref="DeviceRef"/>
        /// </summary>
        private static DeviceRef ConvertFromLogiqx(Models.Logiqx.DeviceRef item)
        {
            var deviceRef = new DeviceRef
            {
                [DeviceRef.NameKey] = item.Name,
            };
            return deviceRef;
        }

        /// <summary>
        /// Convert from <cref="Models.Logiqx.Disk"/> to <cref="Disk"/>
        /// </summary>
        private static Disk ConvertFromLogiqx(Models.Logiqx.Disk item)
        {
            var disk = new Disk
            {
                [Disk.NameKey] = item.Name,
                [Disk.MD5Key] = item.MD5,
                [Disk.SHA1Key] = item.SHA1,
                [Disk.MergeKey] = item.Merge,
                [Disk.StatusKey] = item.Status,
                [Disk.RegionKey] = item.Region,
            };
            return disk;
        }

        /// <summary>
        /// Convert from <cref="Models.Logiqx.Driver"/> to <cref="Driver"/>
        /// </summary>
        private static Driver ConvertFromLogiqx(Models.Logiqx.Driver item)
        {
            var driver = new Driver
            {
                [Driver.StatusKey] = item.Status,
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
        /// Convert from <cref="Models.Logiqx.Media"/> to <cref="Media"/>
        /// </summary>
        private static Media ConvertFromLogiqx(Models.Logiqx.Media item)
        {
            var media = new Media
            {
                [Media.NameKey] = item.Name,
                [Media.MD5Key] = item.MD5,
                [Media.SHA1Key] = item.SHA1,
                [Media.SHA256Key] = item.SHA256,
                [Media.SpamSumKey] = item.SpamSum,
            };
            return media;
        }

        /// <summary>
        /// Convert from <cref="Models.Logiqx.Release"/> to <cref="Release"/>
        /// </summary>
        private static Release ConvertFromLogiqx(Models.Logiqx.Release item)
        {
            var release = new Release
            {
                [Release.NameKey] = item.Name,
                [Release.RegionKey] = item.Region,
                [Release.LanguageKey] = item.Language,
                [Release.DateKey] = item.Date,
                [Release.DefaultKey] = item.Default,
            };
            return release;
        }

        /// <summary>
        /// Convert from <cref="Models.Logiqx.Rom"/> to <cref="Rom"/>
        /// </summary>
        private static Rom ConvertFromLogiqx(Models.Logiqx.Rom item)
        {
            var rom = new Rom
            {
                [Rom.NameKey] = item.Name,
                [Rom.SizeKey] = item.Size,
                [Rom.CRCKey] = item.CRC,
                [Rom.MD5Key] = item.MD5,
                [Rom.SHA1Key] = item.SHA1,
                [Rom.SHA256Key] = item.SHA256,
                [Rom.SHA384Key] = item.SHA384,
                [Rom.SHA512Key] = item.SHA512,
                [Rom.SpamSumKey] = item.SpamSum,
                [Rom.xxHash364Key] = item.xxHash364,
                [Rom.xxHash3128Key] = item.xxHash3128,
                [Rom.MergeKey] = item.Merge,
                [Rom.StatusKey] = item.Status,
                [Rom.SerialKey] = item.Serial,
                [Rom.HeaderKey] = item.Header,
                [Rom.DateKey] = item.Date,
                [Rom.InvertedKey] = item.Inverted,
                [Rom.MIAKey] = item.MIA,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Logiqx.Sample"/> to <cref="Sample"/>
        /// </summary>
        private static Sample ConvertFromLogiqx(Models.Logiqx.Sample item)
        {
            var sample = new Sample
            {
                [Sample.NameKey] = item.Name,
            };
            return sample;
        }

        /// <summary>
        /// Convert from <cref="Models.Logiqx.SoftwareList"/> to <cref="SoftwareList"/>
        /// </summary>
        private static SoftwareList ConvertFromLogiqx(Models.Logiqx.SoftwareList item)
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

        #endregion

        #region Deserialize

        /// <summary>
        /// Convert from <cref="Header"/> to <cref="Models.Logiqx.Header"/>
        /// </summary>
        public static Models.Logiqx.Header? ConvertHeaderToLogiqx(Header? item)
        {
            if (item == null)
                return null;

            var header = new Models.Logiqx.Header
            {
                Id = item.ReadString(Header.IdKey),
                Name = item.ReadString(Header.NameKey),
                Description = item.ReadString(Header.DescriptionKey),
                RootDir = item.ReadString(Header.RootDirKey),
                Category = item.ReadString(Header.CategoryKey),
                Version = item.ReadString(Header.VersionKey),
                Date = item.ReadString(Header.DateKey),
                Author = item.ReadString(Header.AuthorKey),
                Email = item.ReadString(Header.EmailKey),
                Homepage = item.ReadString(Header.HomepageKey),
                Url = item.ReadString(Header.UrlKey),
                Comment = item.ReadString(Header.CommentKey),
                Type = item.ReadString(Header.TypeKey),
            };

            if (item.ContainsKey(Header.HeaderKey)
                || item.ContainsKey(Header.ForceMergingKey)
                || item.ContainsKey(Header.ForceNodumpKey)
                || item.ContainsKey(Header.ForcePackingKey))
            {
                header.ClrMamePro = new Models.Logiqx.ClrMamePro
                {
                    Header = item.ReadString(Header.HeaderKey),
                    ForceMerging = item.ReadString(Header.ForceMergingKey),
                    ForceNodump = item.ReadString(Header.ForceNodumpKey),
                    ForcePacking = item.ReadString(Header.ForcePackingKey),
                };
            }

            if (item.ContainsKey(Header.PluginKey)
                || item.ContainsKey(Header.RomModeKey)
                || item.ContainsKey(Header.BiosModeKey)
                || item.ContainsKey(Header.SampleModeKey)
                || item.ContainsKey(Header.LockRomModeKey)
                || item.ContainsKey(Header.LockBiosModeKey)
                || item.ContainsKey(Header.LockSampleModeKey))
            {
                header.RomCenter = new Models.Logiqx.RomCenter
                {
                    Plugin = item.ReadString(Header.PluginKey),
                    RomMode = item.ReadString(Header.RomModeKey),
                    BiosMode = item.ReadString(Header.BiosModeKey),
                    SampleMode = item.ReadString(Header.SampleModeKey),
                    LockRomMode = item.ReadString(Header.LockRomModeKey),
                    LockBiosMode = item.ReadString(Header.LockBiosModeKey),
                    LockSampleMode = item.ReadString(Header.LockSampleModeKey),
                };
            }

            return header;
        }

        /// <summary>
        /// Convert from <cref="Archive"/> to <cref="Models.Logiqx.Archive"/>
        /// </summary>
        public static Models.Logiqx.GameBase? ConvertMachineToLogiqx(Machine? item, bool game = false)
        {
            if (item == null)
                return null;

            Models.Logiqx.GameBase gameBase = game ? new Models.Logiqx.Game() : new Models.Logiqx.Machine();

            gameBase.Name = item.ReadString(Machine.NameKey);
            gameBase.SourceFile = item.ReadString(Machine.SourceFileKey);
            gameBase.IsBios = item.ReadString(Machine.IsBiosKey);
            gameBase.IsDevice = item.ReadString(Machine.IsDeviceKey);
            gameBase.IsMechanical = item.ReadString(Machine.IsMechanicalKey);
            gameBase.CloneOf = item.ReadString(Machine.CloneOfKey);
            gameBase.RomOf = item.ReadString(Machine.RomOfKey);
            gameBase.SampleOf = item.ReadString(Machine.SampleOfKey);
            gameBase.Board = item.ReadString(Machine.BoardKey);
            gameBase.RebuildTo = item.ReadString(Machine.RebuildToKey);
            gameBase.Id = item.ReadString(Machine.IdKey);
            gameBase.CloneOfId = item.ReadString(Machine.CloneOfIdKey);
            gameBase.Runnable = item.ReadString(Machine.RunnableKey);
            gameBase.Comment = item.ReadStringArray(Machine.CommentKey);
            gameBase.Description = item.ReadString(Machine.DescriptionKey);
            gameBase.Year = item.ReadString(Machine.YearKey);
            gameBase.Manufacturer = item.ReadString(Machine.ManufacturerKey);
            gameBase.Publisher = item.ReadString(Machine.PublisherKey);
            gameBase.Category = item.ReadStringArray(Machine.CategoryKey);

            var trurip = item.Read<Models.Logiqx.Trurip>(Machine.TruripKey);
            gameBase.Trurip = trurip;

            var releases = item.Read<Release[]>(Machine.ReleaseKey);
            gameBase.Release = releases?.Select(ConvertToLogiqx)?.ToArray();

            var biosSets = item.Read<BiosSet[]>(Machine.BiosSetKey);
            gameBase.BiosSet = biosSets?.Select(ConvertToLogiqx)?.ToArray();

            var roms = item.Read<Rom[]>(Machine.RomKey);
            gameBase.Rom = roms?.Select(ConvertToLogiqx)?.ToArray();

            var disks = item.Read<Disk[]>(Machine.DiskKey);
            gameBase.Disk = disks?.Select(ConvertToLogiqx)?.ToArray();

            var medias = item.Read<Media[]>(Machine.MediaKey);
            gameBase.Media = medias?.Select(ConvertToLogiqx)?.ToArray();

            var deviceRefs = item.Read<DeviceRef[]>(Machine.DeviceRefKey);
            gameBase.DeviceRef = deviceRefs?.Select(ConvertToLogiqx)?.ToArray();

            var samples = item.Read<Sample[]>(Machine.SampleKey);
            gameBase.Sample = samples?.Select(ConvertToLogiqx)?.ToArray();

            var archives = item.Read<Archive[]>(Machine.ArchiveKey);
            gameBase.Archive = archives?.Select(ConvertToLogiqx)?.ToArray();

            var drivers = item.Read<Driver[]>(Machine.DriverKey);
            gameBase.Driver = drivers?.Select(ConvertToLogiqx)?.ToArray();

            var softwareLists = item.Read<SoftwareList[]>(Machine.SoftwareListKey);
            gameBase.SoftwareList = softwareLists?.Select(ConvertToLogiqx)?.ToArray();

            return gameBase;
        }

        /// <summary>
        /// Convert from <cref="Archive"/> to <cref="Models.Logiqx.Archive"/>
        /// </summary>
        private static Models.Logiqx.Archive? ConvertToLogiqx(Archive? item)
        {
            if (item == null)
                return null;

            var archive = new Models.Logiqx.Archive
            {
                Name = item.ReadString(Archive.NameKey),
            };
            return archive;
        }

        /// <summary>
        /// Convert from <cref="BiosSet"/> to <cref="Models.Logiqx.BiosSet"/>
        /// </summary>
        private static Models.Logiqx.BiosSet? ConvertToLogiqx(BiosSet? item)
        {
            if (item == null)
                return null;

            var biosset = new Models.Logiqx.BiosSet
            {
                Name = item.ReadString(BiosSet.NameKey),
                Description = item.ReadString(BiosSet.DescriptionKey),
                Default = item.ReadString(BiosSet.DefaultKey),
            };
            return biosset;
        }

        /// <summary>
        /// Convert from <cref="DeviceRef"/> to <cref="Models.Logiqx.DeviceRef"/>
        /// </summary>
        private static Models.Logiqx.DeviceRef? ConvertToLogiqx(DeviceRef? item)
        {
            if (item == null)
                return null;

            var deviceRef = new Models.Logiqx.DeviceRef
            {
                Name = item.ReadString(DipSwitch.NameKey),
            };
            return deviceRef;
        }

        /// <summary>
        /// Convert from <cref="Disk"/> to <cref="Models.Logiqx.Disk"/>
        /// </summary>
        private static Models.Logiqx.Disk? ConvertToLogiqx(Disk? item)
        {
            if (item == null)
                return null;

            var disk = new Models.Logiqx.Disk
            {
                Name = item.ReadString(Disk.NameKey),
                MD5 = item.ReadString(Disk.MD5Key),
                SHA1 = item.ReadString(Disk.SHA1Key),
                Merge = item.ReadString(Disk.MergeKey),
                Status = item.ReadString(Disk.StatusKey),
                Region = item.ReadString(Disk.RegionKey),
            };
            return disk;
        }

        /// <summary>
        /// Convert from <cref="Driver"/> to <cref="Models.Logiqx.Driver"/>
        /// </summary>
        private static Models.Logiqx.Driver? ConvertToLogiqx(Driver? item)
        {
            if (item == null)
                return null;

            var driver = new Models.Logiqx.Driver
            {
                Status = item.ReadString(Driver.StatusKey),
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
        /// Convert from <cref="Media"/> to <cref="Models.Logiqx.Media"/>
        /// </summary>
        private static Models.Logiqx.Media? ConvertToLogiqx(Media? item)
        {
            if (item == null)
                return null;

            var media = new Models.Logiqx.Media
            {
                Name = item.ReadString(Media.NameKey),
                MD5 = item.ReadString(Media.MD5Key),
                SHA1 = item.ReadString(Media.SHA1Key),
                SHA256 = item.ReadString(Media.SHA256Key),
                SpamSum = item.ReadString(Media.SpamSumKey),
            };
            return media;
        }

        /// <summary>
        /// Convert from <cref="Release"/> to <cref="Models.Logiqx.Release"/>
        /// </summary>
        private static Models.Logiqx.Release? ConvertToLogiqx(Release? item)
        {
            if (item == null)
                return null;

            var release = new Models.Logiqx.Release
            {
                Name = item.ReadString(Release.NameKey),
                Region = item.ReadString(Release.RegionKey),
                Language = item.ReadString(Release.LanguageKey),
                Date = item.ReadString(Release.DateKey),
                Default = item.ReadString(Release.DefaultKey),
            };
            return release;
        }

        /// <summary>
        /// Convert from <cref="Rom"/> to <cref="Models.Logiqx.Rom"/>
        /// </summary>
        private static Models.Logiqx.Rom? ConvertToLogiqx(Rom? item)
        {
            if (item == null)
                return null;

            var rom = new Models.Logiqx.Rom
            {
                Name = item.ReadString(Rom.NameKey),
                Size = item.ReadString(Rom.SizeKey),
                CRC = item.ReadString(Rom.CRCKey),
                MD5 = item.ReadString(Rom.MD5Key),
                SHA1 = item.ReadString(Rom.SHA1Key),
                SHA256 = item.ReadString(Rom.SHA256Key),
                SHA384 = item.ReadString(Rom.SHA384Key),
                SHA512 = item.ReadString(Rom.SHA512Key),
                SpamSum = item.ReadString(Rom.SpamSumKey),
                xxHash364 = item.ReadString(Rom.xxHash364Key),
                xxHash3128 = item.ReadString(Rom.xxHash3128Key),
                Merge = item.ReadString(Rom.MergeKey),
                Status = item.ReadString(Rom.StatusKey),
                Serial = item.ReadString(Rom.SerialKey),
                Header = item.ReadString(Rom.HeaderKey),
                Date = item.ReadString(Rom.DateKey),
                Inverted = item.ReadString(Rom.InvertedKey),
                MIA = item.ReadString(Rom.MIAKey),
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Sample"/> to <cref="Models.Logiqx.Sample"/>
        /// </summary>
        private static Models.Logiqx.Sample? ConvertToLogiqx(Sample? item)
        {
            if (item == null)
                return null;

            var sample = new Models.Logiqx.Sample
            {
                Name = item.ReadString(Sample.NameKey),
            };
            return sample;
        }

        /// <summary>
        /// Convert from <cref="SoftwareList"/> to <cref="Models.Logiqx.SoftwareList"/>
        /// </summary>
        private static Models.Logiqx.SoftwareList? ConvertToLogiqx(SoftwareList? item)
        {
            if (item == null)
                return null;

            var softwareList = new Models.Logiqx.SoftwareList
            {
                Tag = item.ReadString(SoftwareList.TagKey),
                Name = item.ReadString(SoftwareList.NameKey),
                Status = item.ReadString(SoftwareList.StatusKey),
                Filter = item.ReadString(SoftwareList.FilterKey),
            };
            return softwareList;
        }

        #endregion
    }
}