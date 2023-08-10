using System.Collections.Generic;
using System.Linq;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for Logiqx models to internal structure
    /// </summary>
    public partial class Internal
    {
        #region Serialize

        /// <summary>
        /// Convert from <cref="Models.Logiqx.Header"/> to <cref="Models.Internal.Header"/>
        /// </summary>
        public static Models.Internal.Header ConvertHeaderFromLogiqx(Models.Logiqx.Header item)
        {
            var header = new Models.Internal.Header
            {
                [Models.Internal.Header.IdKey] = item.Id,
                [Models.Internal.Header.NameKey] = item.Name,
                [Models.Internal.Header.DescriptionKey] = item.Description,
                [Models.Internal.Header.RootDirKey] = item.RootDir,
                [Models.Internal.Header.CategoryKey] = item.Category,
                [Models.Internal.Header.VersionKey] = item.Version,
                [Models.Internal.Header.DateKey] = item.Date,
                [Models.Internal.Header.AuthorKey] = item.Author,
                [Models.Internal.Header.EmailKey] = item.Email,
                [Models.Internal.Header.HomepageKey] = item.Homepage,
                [Models.Internal.Header.UrlKey] = item.Url,
                [Models.Internal.Header.CommentKey] = item.Comment,
                [Models.Internal.Header.TypeKey] = item.Type,
            };

            if (item.ClrMamePro != null)
            {
                header[Models.Internal.Header.HeaderKey] = item.ClrMamePro.Header;
                header[Models.Internal.Header.ForceMergingKey] = item.ClrMamePro.ForceMerging;
                header[Models.Internal.Header.ForceNodumpKey] = item.ClrMamePro.ForceNodump;
                header[Models.Internal.Header.ForcePackingKey] = item.ClrMamePro.ForcePacking;
            }

            if (item.RomCenter != null)
            {
                header[Models.Internal.Header.PluginKey] = item.RomCenter.Plugin;
                header[Models.Internal.Header.RomModeKey] = item.RomCenter.RomMode;
                header[Models.Internal.Header.BiosModeKey] = item.RomCenter.BiosMode;
                header[Models.Internal.Header.SampleModeKey] = item.RomCenter.SampleMode;
                header[Models.Internal.Header.LockRomModeKey] = item.RomCenter.LockRomMode;
                header[Models.Internal.Header.LockBiosModeKey] = item.RomCenter.LockBiosMode;
                header[Models.Internal.Header.LockSampleModeKey] = item.RomCenter.LockSampleMode;
            }

            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.Logiqx.GameBase"/> to <cref="Models.Internal.Machine"/>
        /// </summary>
        public static Models.Internal.Machine ConvertMachineFromLogiqx(Models.Logiqx.GameBase item)
        {
            var machine = new Models.Internal.Machine
            {
                [Models.Internal.Machine.NameKey] = item.Name,
                [Models.Internal.Machine.SourceFileKey] = item.SourceFile,
                [Models.Internal.Machine.IsBiosKey] = item.IsBios,
                [Models.Internal.Machine.IsDeviceKey] = item.IsDevice,
                [Models.Internal.Machine.IsMechanicalKey] = item.IsMechanical,
                [Models.Internal.Machine.CloneOfKey] = item.CloneOf,
                [Models.Internal.Machine.RomOfKey] = item.RomOf,
                [Models.Internal.Machine.SampleOfKey] = item.SampleOf,
                [Models.Internal.Machine.BoardKey] = item.Board,
                [Models.Internal.Machine.RebuildToKey] = item.RebuildTo,
                [Models.Internal.Machine.IdKey] = item.Id,
                [Models.Internal.Machine.CloneOfIdKey] = item.CloneOfId,
                [Models.Internal.Machine.RunnableKey] = item.Runnable,
                [Models.Internal.Machine.CommentKey] = item.Comment,
                [Models.Internal.Machine.DescriptionKey] = item.Description,
                [Models.Internal.Machine.YearKey] = item.Year,
                [Models.Internal.Machine.ManufacturerKey] = item.Manufacturer,
                [Models.Internal.Machine.PublisherKey] = item.Publisher,
                [Models.Internal.Machine.CategoryKey] = item.Category,
                [Models.Internal.Machine.TruripKey] = item.Trurip,
            };

            if (item.Release != null && item.Release.Any())
            {
                var releases = new List<Models.Internal.Release>();
                foreach (var release in item.Release)
                {
                    releases.Add(ConvertFromLogiqx(release));
                }
                machine[Models.Internal.Machine.ReleaseKey] = releases.ToArray();
            }

            if (item.BiosSet != null && item.BiosSet.Any())
            {
                var biosSets = new List<Models.Internal.BiosSet>();
                foreach (var biosSet in item.BiosSet)
                {
                    biosSets.Add(ConvertFromLogiqx(biosSet));
                }
                machine[Models.Internal.Machine.BiosSetKey] = biosSets.ToArray();
            }

            if (item.Rom != null && item.Rom.Any())
            {
                var roms = new List<Models.Internal.Rom>();
                foreach (var rom in item.Rom)
                {
                    roms.Add(ConvertFromLogiqx(rom));
                }
                machine[Models.Internal.Machine.RomKey] = roms.ToArray();
            }

            if (item.Disk != null && item.Disk.Any())
            {
                var disks = new List<Models.Internal.Disk>();
                foreach (var disk in item.Disk)
                {
                    disks.Add(ConvertFromLogiqx(disk));
                }
                machine[Models.Internal.Machine.DiskKey] = disks.ToArray();
            }

            if (item.Media != null && item.Media.Any())
            {
                var medias = new List<Models.Internal.Media>();
                foreach (var media in item.Media)
                {
                    medias.Add(ConvertFromLogiqx(media));
                }
                machine[Models.Internal.Machine.MediaKey] = medias.ToArray();
            }

            if (item.DeviceRef != null && item.DeviceRef.Any())
            {
                var deviceRefs = new List<Models.Internal.DeviceRef>();
                foreach (var deviceRef in item.DeviceRef)
                {
                    deviceRefs.Add(ConvertFromLogiqx(deviceRef));
                }
                machine[Models.Internal.Machine.DeviceRefKey] = deviceRefs.ToArray();
            }

            if (item.Sample != null && item.Sample.Any())
            {
                var samples = new List<Models.Internal.Sample>();
                foreach (var sample in item.Sample)
                {
                    samples.Add(ConvertFromLogiqx(sample));
                }
                machine[Models.Internal.Machine.SampleKey] = samples.ToArray();
            }

            if (item.Archive != null && item.Archive.Any())
            {
                var archives = new List<Models.Internal.Archive>();
                foreach (var archive in item.Archive)
                {
                    archives.Add(ConvertFromLogiqx(archive));
                }
                machine[Models.Internal.Machine.ArchiveKey] = archives.ToArray();
            }

            if (item.Driver != null && item.Driver.Any())
            {
                var drivers = new List<Models.Internal.Driver>();
                foreach (var driver in item.Driver)
                {
                    drivers.Add(ConvertFromLogiqx(driver));
                }
                machine[Models.Internal.Machine.DriverKey] = drivers.ToArray();
            }

            if (item.SoftwareList != null && item.SoftwareList.Any())
            {
                var softwareLists = new List<Models.Internal.SoftwareList>();
                foreach (var softwareList in item.SoftwareList)
                {
                    softwareLists.Add(ConvertFromLogiqx(softwareList));
                }
                machine[Models.Internal.Machine.SoftwareListKey] = softwareLists.ToArray();
            }

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.Logiqx.Archive"/> to <cref="Models.Internal.Archive"/>
        /// </summary>
        public static Models.Internal.Archive ConvertFromLogiqx(Models.Logiqx.Archive item)
        {
            var archive = new Models.Internal.Archive
            {
                [Models.Internal.Archive.NameKey] = item.Name,
            };
            return archive;
        }

        /// <summary>
        /// Convert from <cref="Models.Logiqx.BiosSet"/> to <cref="Models.Internal.BiosSet"/>
        /// </summary>
        public static Models.Internal.BiosSet ConvertFromLogiqx(Models.Logiqx.BiosSet item)
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
        /// Convert from <cref="Models.Logiqx.DeviceRef"/> to <cref="Models.Internal.DeviceRef"/>
        /// </summary>
        public static Models.Internal.DeviceRef ConvertFromLogiqx(Models.Logiqx.DeviceRef item)
        {
            var deviceRef = new Models.Internal.DeviceRef
            {
                [Models.Internal.DeviceRef.NameKey] = item.Name,
            };
            return deviceRef;
        }

        /// <summary>
        /// Convert from <cref="Models.Logiqx.Disk"/> to <cref="Models.Internal.Disk"/>
        /// </summary>
        public static Models.Internal.Disk ConvertFromLogiqx(Models.Logiqx.Disk item)
        {
            var disk = new Models.Internal.Disk
            {
                [Models.Internal.Disk.NameKey] = item.Name,
                [Models.Internal.Disk.MD5Key] = item.MD5,
                [Models.Internal.Disk.SHA1Key] = item.SHA1,
                [Models.Internal.Disk.MergeKey] = item.Merge,
                [Models.Internal.Disk.StatusKey] = item.Status,
                [Models.Internal.Disk.RegionKey] = item.Region,
            };
            return disk;
        }

        /// <summary>
        /// Convert from <cref="Models.Logiqx.Driver"/> to <cref="Models.Internal.Driver"/>
        /// </summary>
        public static Models.Internal.Driver ConvertFromLogiqx(Models.Logiqx.Driver item)
        {
            var driver = new Models.Internal.Driver
            {
                [Models.Internal.Driver.StatusKey] = item.Status,
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
        /// Convert from <cref="Models.Logiqx.Media"/> to <cref="Models.Internal.Media"/>
        /// </summary>
        public static Models.Internal.Media ConvertFromLogiqx(Models.Logiqx.Media item)
        {
            var media = new Models.Internal.Media
            {
                [Models.Internal.Media.NameKey] = item.Name,
                [Models.Internal.Media.MD5Key] = item.MD5,
                [Models.Internal.Media.SHA1Key] = item.SHA1,
                [Models.Internal.Media.SHA256Key] = item.SHA256,
                [Models.Internal.Media.SpamSumKey] = item.SpamSum,
            };
            return media;
        }

        /// <summary>
        /// Convert from <cref="Models.Logiqx.Release"/> to <cref="Models.Internal.Release"/>
        /// </summary>
        public static Models.Internal.Release ConvertFromLogiqx(Models.Logiqx.Release item)
        {
            var release = new Models.Internal.Release
            {
                [Models.Internal.Release.NameKey] = item.Name,
                [Models.Internal.Release.RegionKey] = item.Region,
                [Models.Internal.Release.LanguageKey] = item.Language,
                [Models.Internal.Release.DateKey] = item.Date,
                [Models.Internal.Release.DefaultKey] = item.Default,
            };
            return release;
        }

        /// <summary>
        /// Convert from <cref="Models.Logiqx.Rom"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Rom ConvertFromLogiqx(Models.Logiqx.Rom item)
        {
            var rom = new Models.Internal.Rom
            {
                [Models.Internal.Rom.NameKey] = item.Name,
                [Models.Internal.Rom.SizeKey] = item.Size,
                [Models.Internal.Rom.CRCKey] = item.CRC,
                [Models.Internal.Rom.MD5Key] = item.MD5,
                [Models.Internal.Rom.SHA1Key] = item.SHA1,
                [Models.Internal.Rom.SHA256Key] = item.SHA256,
                [Models.Internal.Rom.SHA384Key] = item.SHA384,
                [Models.Internal.Rom.SHA512Key] = item.SHA512,
                [Models.Internal.Rom.SpamSumKey] = item.SpamSum,
                [Models.Internal.Rom.xxHash364Key] = item.xxHash364,
                [Models.Internal.Rom.xxHash3128Key] = item.xxHash3128,
                [Models.Internal.Rom.MergeKey] = item.Merge,
                [Models.Internal.Rom.StatusKey] = item.Status,
                [Models.Internal.Rom.SerialKey] = item.Serial,
                [Models.Internal.Rom.HeaderKey] = item.Header,
                [Models.Internal.Rom.DateKey] = item.Date,
                [Models.Internal.Rom.InvertedKey] = item.Inverted,
                [Models.Internal.Rom.MIAKey] = item.MIA,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Logiqx.Sample"/> to <cref="Models.Internal.Sample"/>
        /// </summary>
        public static Models.Internal.Sample ConvertFromLogiqx(Models.Logiqx.Sample item)
        {
            var sample = new Models.Internal.Sample
            {
                [Models.Internal.Sample.NameKey] = item.Name,
            };
            return sample;
        }

        /// <summary>
        /// Convert from <cref="Models.Logiqx.SoftwareList"/> to <cref="Models.Internal.SoftwareList"/>
        /// </summary>
        public static Models.Internal.SoftwareList ConvertFromLogiqx(Models.Logiqx.SoftwareList item)
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

        #endregion

        #region Deserialize

        /// <summary>
        /// Convert from <cref="Models.Internal.Header"/> to <cref="Models.Logiqx.Header"/>
        /// </summary>
        public static Models.Logiqx.Header? ConvertHeaderToLogiqx(Models.Internal.Header? item)
        {
            if (item == null)
                return null;

            var header = new Models.Logiqx.Header
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
        /// Convert from <cref="Models.Internal.Archive"/> to <cref="Models.Logiqx.Archive"/>
        /// </summary>
        public static Models.Logiqx.GameBase? ConvertMachineToLogiqx(Models.Internal.Machine? item, bool game = false)
        {
            if (item == null)
                return null;

            Models.Logiqx.GameBase gameBase = game ? new Models.Logiqx.Game() : new Models.Logiqx.Machine();

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

            var trurip = item.Read<Models.Logiqx.Trurip>(Models.Internal.Machine.TruripKey);
            gameBase.Trurip = trurip;

            var releases = item.Read<Models.Internal.Release[]>(Models.Internal.Machine.ReleaseKey);
            gameBase.Release = releases?.Select(ConvertToLogiqx)?.ToArray();

            var biosSets = item.Read<Models.Internal.BiosSet[]>(Models.Internal.Machine.BiosSetKey);
            gameBase.BiosSet = biosSets?.Select(ConvertToLogiqx)?.ToArray();

            var roms = item.Read<Models.Internal.Rom[]>(Models.Internal.Machine.RomKey);
            gameBase.Rom = roms?.Select(ConvertToLogiqx)?.ToArray();

            var disks = item.Read<Models.Internal.Disk[]>(Models.Internal.Machine.DiskKey);
            gameBase.Disk = disks?.Select(ConvertToLogiqx)?.ToArray();

            var medias = item.Read<Models.Internal.Media[]>(Models.Internal.Machine.MediaKey);
            gameBase.Media = medias?.Select(ConvertToLogiqx)?.ToArray();

            var deviceRefs = item.Read<Models.Internal.DeviceRef[]>(Models.Internal.Machine.DeviceRefKey);
            gameBase.DeviceRef = deviceRefs?.Select(ConvertToLogiqx)?.ToArray();

            var samples = item.Read<Models.Internal.Sample[]>(Models.Internal.Machine.SampleKey);
            gameBase.Sample = samples?.Select(ConvertToLogiqx)?.ToArray();

            var archives = item.Read<Models.Internal.Archive[]>(Models.Internal.Machine.ArchiveKey);
            gameBase.Archive = archives?.Select(ConvertToLogiqx)?.ToArray();

            var drivers = item.Read<Models.Internal.Driver[]>(Models.Internal.Machine.DriverKey);
            gameBase.Driver = drivers?.Select(ConvertToLogiqx)?.ToArray();

            var softwareLists = item.Read<Models.Internal.SoftwareList[]>(Models.Internal.Machine.SoftwareListKey);
            gameBase.SoftwareList = softwareLists?.Select(ConvertToLogiqx)?.ToArray();

            return gameBase;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Archive"/> to <cref="Models.Logiqx.Archive"/>
        /// </summary>
        private static Models.Logiqx.Archive? ConvertToLogiqx(Models.Internal.Archive? item)
        {
            if (item == null)
                return null;

            var archive = new Models.Logiqx.Archive
            {
                Name = item.ReadString(Models.Internal.Archive.NameKey),
            };
            return archive;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.BiosSet"/> to <cref="Models.Logiqx.BiosSet"/>
        /// </summary>
        private static Models.Logiqx.BiosSet? ConvertToLogiqx(Models.Internal.BiosSet? item)
        {
            if (item == null)
                return null;

            var biosset = new Models.Logiqx.BiosSet
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
        private static Models.Logiqx.DeviceRef? ConvertToLogiqx(Models.Internal.DeviceRef? item)
        {
            if (item == null)
                return null;

            var deviceRef = new Models.Logiqx.DeviceRef
            {
                Name = item.ReadString(Models.Internal.DipSwitch.NameKey),
            };
            return deviceRef;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Disk"/> to <cref="Models.Logiqx.Disk"/>
        /// </summary>
        private static Models.Logiqx.Disk? ConvertToLogiqx(Models.Internal.Disk? item)
        {
            if (item == null)
                return null;

            var disk = new Models.Logiqx.Disk
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
        private static Models.Logiqx.Driver? ConvertToLogiqx(Models.Internal.Driver? item)
        {
            if (item == null)
                return null;

            var driver = new Models.Logiqx.Driver
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
        private static Models.Logiqx.Media? ConvertToLogiqx(Models.Internal.Media? item)
        {
            if (item == null)
                return null;

            var media = new Models.Logiqx.Media
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
        private static Models.Logiqx.Release? ConvertToLogiqx(Models.Internal.Release? item)
        {
            if (item == null)
                return null;

            var release = new Models.Logiqx.Release
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
        private static Models.Logiqx.Rom? ConvertToLogiqx(Models.Internal.Rom? item)
        {
            if (item == null)
                return null;

            var rom = new Models.Logiqx.Rom
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
        private static Models.Logiqx.Sample? ConvertToLogiqx(Models.Internal.Sample? item)
        {
            if (item == null)
                return null;

            var sample = new Models.Logiqx.Sample
            {
                Name = item.ReadString(Models.Internal.Sample.NameKey),
            };
            return sample;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.SoftwareList"/> to <cref="Models.Logiqx.SoftwareList"/>
        /// </summary>
        private static Models.Logiqx.SoftwareList? ConvertToLogiqx(Models.Internal.SoftwareList? item)
        {
            if (item == null)
                return null;

            var softwareList = new Models.Logiqx.SoftwareList
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