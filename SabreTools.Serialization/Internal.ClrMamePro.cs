using System.Collections.Generic;
using System.Linq;
using SabreTools.Models.Internal;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for ClrMamePro models to internal structure
    /// </summary>
    public partial class Internal
    {
        #region Serialize

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.MetadataFile"/> to <cref="MetadataFile"/>
        /// </summary>
        public static MetadataFile ConvertToInternalModel(Models.ClrMamePro.MetadataFile item)
        {
            var metadataFile = new MetadataFile();

            if (item?.ClrMamePro != null)
                metadataFile[MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item.ClrMamePro);

            if (item?.Game != null && item.Game.Any())
                metadataFile[MetadataFile.MachineKey] = item.Game.Select(ConvertMachineToInternalModel).ToArray();

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.ClrMamePro"/> to <cref="Header"/>
        /// </summary>
        private static Header ConvertHeaderToInternalModel(Models.ClrMamePro.ClrMamePro item)
        {
            var header = new Header
            {
                [Header.NameKey] = item.Name,
                [Header.DescriptionKey] = item.Description,
                [Header.RootDirKey] = item.RootDir,
                [Header.CategoryKey] = item.Category,
                [Header.VersionKey] = item.Version,
                [Header.DateKey] = item.Date,
                [Header.AuthorKey] = item.Author,
                [Header.HomepageKey] = item.Homepage,
                [Header.UrlKey] = item.Url,
                [Header.CommentKey] = item.Comment,
                [Header.HeaderKey] = item.Header,
                [Header.TypeKey] = item.Type,
                [Header.ForceMergingKey] = item.ForceMerging,
                [Header.ForceZippingKey] = item.ForceZipping,
                [Header.ForcePackingKey] = item.ForcePacking,
            };
            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.GameBase"/> to <cref="Machine"/>
        /// </summary>
        private static Machine ConvertMachineToInternalModel(Models.ClrMamePro.GameBase item)
        {
            var machine = new Machine
            {
                [Machine.NameKey] = item.Name,
                [Machine.DescriptionKey] = item.Description,
                [Machine.YearKey] = item.Year,
                [Machine.ManufacturerKey] = item.Manufacturer,
                [Machine.CategoryKey] = item.Category,
                [Machine.CloneOfKey] = item.CloneOf,
                [Machine.RomOfKey] = item.RomOf,
                [Machine.SampleOfKey] = item.SampleOf,
            };

            if (item.Release != null && item.Release.Any())
            {
                var releases = new List<Release>();
                foreach (var release in item.Release)
                {
                    releases.Add(ConvertToInternalModel(release));
                }
                machine[Machine.ReleaseKey] = releases.ToArray();
            }

            if (item.BiosSet != null && item.BiosSet.Any())
            {
                var biosSets = new List<BiosSet>();
                foreach (var biosSet in item.BiosSet)
                {
                    biosSets.Add(ConvertToInternalModel(biosSet));
                }
                machine[Machine.BiosSetKey] = biosSets.ToArray();
            }

            if (item.Rom != null && item.Rom.Any())
            {
                var roms = new List<Rom>();
                foreach (var rom in item.Rom)
                {
                    roms.Add(ConvertToInternalModel(rom));
                }
                machine[Machine.RomKey] = roms.ToArray();
            }

            if (item.Disk != null && item.Disk.Any())
            {
                var disks = new List<Disk>();
                foreach (var disk in item.Disk)
                {
                    disks.Add(ConvertToInternalModel(disk));
                }
                machine[Machine.DiskKey] = disks.ToArray();
            }

            if (item.Media != null && item.Media.Any())
            {
                var medias = new List<Media>();
                foreach (var media in item.Media)
                {
                    medias.Add(ConvertToInternalModel(media));
                }
                machine[Machine.MediaKey] = medias.ToArray();
            }

            if (item.Sample != null && item.Sample.Any())
            {
                var samples = new List<Sample>();
                foreach (var sample in item.Sample)
                {
                    samples.Add(ConvertToInternalModel(sample));
                }
                machine[Machine.SampleKey] = samples.ToArray();
            }

            if (item.Archive != null && item.Archive.Any())
            {
                var archives = new List<Archive>();
                foreach (var archive in item.Archive)
                {
                    archives.Add(ConvertToInternalModel(archive));
                }
                machine[Machine.ArchiveKey] = archives.ToArray();
            }

            if (item.Chip != null && item.Chip.Any())
            {
                var chips = new List<Chip>();
                foreach (var chip in item.Chip)
                {
                    chips.Add(ConvertToInternalModel(chip));
                }
                machine[Machine.ChipKey] = chips.ToArray();
            }

            if (item.Video != null)
                machine[Machine.VideoKey] = ConvertToInternalModel(item.Video);

            if (item.Sound != null)
                machine[Machine.SoundKey] = ConvertToInternalModel(item.Sound);

            if (item.Input != null)
                machine[Machine.InputKey] = ConvertToInternalModel(item.Input);

            if (item.DipSwitch != null && item.DipSwitch.Any())
            {
                var dipSwitches = new List<DipSwitch>();
                foreach (var dipSwitch in item.DipSwitch)
                {
                    dipSwitches.Add(ConvertToInternalModel(dipSwitch));
                }
                machine[Machine.DipSwitchKey] = dipSwitches.ToArray();
            }

            if (item.Driver != null)
                machine[Machine.DriverKey] = ConvertToInternalModel(item.Driver);

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Archive"/> to <cref="Archive"/>
        /// </summary>
        private static Archive ConvertToInternalModel(Models.ClrMamePro.Archive item)
        {
            var archive = new Archive
            {
                [Archive.NameKey] = item.Name,
            };
            return archive;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.BiosSet"/> to <cref="BiosSet"/>
        /// </summary>
        private static BiosSet ConvertToInternalModel(Models.ClrMamePro.BiosSet item)
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
        /// Convert from <cref="Models.ClrMamePro.Chip"/> to <cref="Chip"/>
        /// </summary>
        private static Chip ConvertToInternalModel(Models.ClrMamePro.Chip item)
        {
            var chip = new Chip
            {
                [Chip.ChipTypeKey] = item.Type,
                [Chip.NameKey] = item.Name,
                [Chip.FlagsKey] = item.Flags,
                [Chip.ClockKey] = item.Clock,
            };
            return chip;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.DipSwitch"/> to <cref="DipSwitch"/>
        /// </summary>
        private static DipSwitch ConvertToInternalModel(Models.ClrMamePro.DipSwitch item)
        {
            var dipswitch = new DipSwitch
            {
                [DipSwitch.NameKey] = item.Name,
                [DipSwitch.EntryKey] = item.Entry,
                [DipSwitch.DefaultKey] = item.Default,
            };
            return dipswitch;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Disk"/> to <cref="Disk"/>
        /// </summary>
        private static Disk ConvertToInternalModel(Models.ClrMamePro.Disk item)
        {
            var disk = new Disk
            {
                [Disk.NameKey] = item.Name,
                [Disk.MD5Key] = item.MD5,
                [Disk.SHA1Key] = item.SHA1,
                [Disk.MergeKey] = item.Merge,
                [Disk.StatusKey] = item.Status,
                [Disk.FlagsKey] = item.Flags,
            };
            return disk;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Driver"/> to <cref="Driver"/>
        /// </summary>
        private static Driver ConvertToInternalModel(Models.ClrMamePro.Driver item)
        {
            var driver = new Driver
            {
                [Driver.StatusKey] = item.Status,
                [Driver.ColorKey] = item.Color,
                [Driver.SoundKey] = item.Sound,
                [Driver.PaletteSizeKey] = item.PaletteSize,
                [Driver.BlitKey] = item.Blit,
            };
            return driver;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Input"/> to <cref="Input"/>
        /// </summary>
        private static Input ConvertToInternalModel(Models.ClrMamePro.Input item)
        {
            var input = new Input
            {
                [Input.PlayersKey] = item.Players,
                [Input.ControlKey] = item.Control,
                [Input.ButtonsKey] = item.Buttons,
                [Input.CoinsKey] = item.Coins,
                [Input.TiltKey] = item.Tilt,
                [Input.ServiceKey] = item.Service,
            };
            return input;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Media"/> to <cref="Media"/>
        /// </summary>
        private static Media ConvertToInternalModel(Models.ClrMamePro.Media item)
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
        /// Convert from <cref="Models.ClrMamePro.Release"/> to <cref="Release"/>
        /// </summary>
        private static Release ConvertToInternalModel(Models.ClrMamePro.Release item)
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
        /// Convert from <cref="Models.ClrMamePro.Rom"/> to <cref="Rom"/>
        /// </summary>
        private static Rom ConvertToInternalModel(Models.ClrMamePro.Rom item)
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
                [Rom.RegionKey] = item.Region,
                [Rom.FlagsKey] = item.Flags,
                [Rom.OffsetKey] = item.Offs,
                [Rom.SerialKey] = item.Serial,
                [Rom.HeaderKey] = item.Header,
                [Rom.DateKey] = item.Date,
                [Rom.InvertedKey] = item.Inverted,
                [Rom.MIAKey] = item.MIA,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Sample"/> to <cref="Sample"/>
        /// </summary>
        private static Sample ConvertToInternalModel(Models.ClrMamePro.Sample item)
        {
            var sample = new Sample
            {
                [Sample.NameKey] = item.Name,
            };
            return sample;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Sound"/> to <cref="Sound"/>
        /// </summary>
        private static Sound ConvertToInternalModel(Models.ClrMamePro.Sound item)
        {
            var sound = new Sound
            {
                [Sound.ChannelsKey] = item.Channels,
            };
            return sound;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Video"/> to <cref="Video"/>
        /// </summary>
        private static Video ConvertToInternalModel(Models.ClrMamePro.Video item)
        {
            var video = new Video
            {
                [Video.ScreenKey] = item.Screen,
                [Video.OrientationKey] = item.Orientation,
                [Video.WidthKey] = item.X,
                [Video.HeightKey] = item.Y,
                [Video.AspectXKey] = item.AspectX,
                [Video.AspectYKey] = item.AspectY,
                [Video.RefreshKey] = item.Freq,
            };
            return video;
        }

        #endregion

        #region Deserialize

        /// <summary>
        /// Convert from <cref="Header"/> to <cref="Models.ClrMamePro.ClrMamePro"/>
        /// </summary>
        public static Models.ClrMamePro.ClrMamePro? ConvertHeaderToClrMamePro(Header? item)
        {
            if (item == null)
                return null;

            var clrMamePro = new Models.ClrMamePro.ClrMamePro
            {
                Name = item.ReadString(Header.NameKey),
                Description = item.ReadString(Header.DescriptionKey),
                RootDir = item.ReadString(Header.RootDirKey),
                Category = item.ReadString(Header.CategoryKey),
                Version = item.ReadString(Header.VersionKey),
                Date = item.ReadString(Header.DateKey),
                Author = item.ReadString(Header.AuthorKey),
                Homepage = item.ReadString(Header.HomepageKey),
                Url = item.ReadString(Header.UrlKey),
                Comment = item.ReadString(Header.CommentKey),
                Header = item.ReadString(Header.HeaderKey),
                Type = item.ReadString(Header.TypeKey),
                ForceMerging = item.ReadString(Header.ForceMergingKey),
                ForceZipping = item.ReadString(Header.ForceZippingKey),
                ForcePacking = item.ReadString(Header.ForcePackingKey),
            };
            return clrMamePro;
        }

        /// <summary>
        /// Convert from <cref="Archive"/> to <cref="Models.ClrMamePro.Machine"/>
        /// </summary>
        public static Models.ClrMamePro.GameBase? ConvertMachineToClrMamePro(Machine? item, bool game = false)
        {
            if (item == null)
                return null;

            Models.ClrMamePro.GameBase gameBase = game ? new Models.ClrMamePro.Game() : new Models.ClrMamePro.Machine();

            gameBase.Name = item.ReadString(Machine.NameKey);
            gameBase.Description = item.ReadString(Machine.DescriptionKey);
            gameBase.Year = item.ReadString(Machine.YearKey);
            gameBase.Manufacturer = item.ReadString(Machine.ManufacturerKey);
            gameBase.Category = item.ReadString(Machine.CategoryKey);
            gameBase.CloneOf = item.ReadString(Machine.CloneOfKey);
            gameBase.RomOf = item.ReadString(Machine.RomOfKey);
            gameBase.SampleOf = item.ReadString(Machine.SampleOfKey);

            var releases = item.Read<Release[]>(Machine.ReleaseKey);
            gameBase.Release = releases?.Select(ConvertToClrMamePro)?.ToArray();

            var biosSets = item.Read<BiosSet[]>(Machine.BiosSetKey);
            gameBase.BiosSet = biosSets?.Select(ConvertToClrMamePro)?.ToArray();

            var roms = item.Read<Rom[]>(Machine.RomKey);
            gameBase.Rom = roms?.Select(ConvertToClrMamePro)?.ToArray();

            var disks = item.Read<Disk[]>(Machine.DiskKey);
            gameBase.Disk = disks?.Select(ConvertToClrMamePro)?.ToArray();

            var medias = item.Read<Media[]>(Machine.MediaKey);
            gameBase.Media = medias?.Select(ConvertToClrMamePro)?.ToArray();

            var samples = item.Read<Sample[]>(Machine.SampleKey);
            gameBase.Sample = samples?.Select(ConvertToClrMamePro)?.ToArray();

            var archives = item.Read<Archive[]>(Machine.ArchiveKey);
            gameBase.Archive = archives?.Select(ConvertToClrMamePro)?.ToArray();

            var chips = item.Read<Chip[]>(Machine.ChipKey);
            gameBase.Chip = chips?.Select(ConvertToClrMamePro)?.ToArray();

            var video = item.Read<Video>(Machine.VideoKey);
            gameBase.Video = ConvertToClrMamePro(video);

            var sound = item.Read<Sound>(Machine.SoundKey);
            gameBase.Sound = ConvertToClrMamePro(sound);

            var input = item.Read<Input>(Machine.InputKey);
            gameBase.Input = ConvertToClrMamePro(input);

            var dipSwitches = item.Read<DipSwitch[]>(Machine.DipSwitchKey);
            gameBase.DipSwitch = dipSwitches?.Select(ConvertToClrMamePro)?.ToArray();

            var driver = item.Read<Driver>(Machine.DriverKey);
            gameBase.Driver = ConvertToClrMamePro(driver);

            return gameBase;
        }

        /// <summary>
        /// Convert from <cref="Archive"/> to <cref="Models.ClrMamePro.Archive"/>
        /// </summary>
        private static Models.ClrMamePro.Archive? ConvertToClrMamePro(Archive? item)
        {
            if (item == null)
                return null;

            var archive = new Models.ClrMamePro.Archive
            {
                Name = item.ReadString(Archive.NameKey),
            };
            return archive;
        }

        /// <summary>
        /// Convert from <cref="BiosSet"/> to <cref="Models.ClrMamePro.BiosSet"/>
        /// </summary>
        private static Models.ClrMamePro.BiosSet? ConvertToClrMamePro(BiosSet? item)
        {
            if (item == null)
                return null;

            var biosset = new Models.ClrMamePro.BiosSet
            {
                Name = item.ReadString(BiosSet.NameKey),
                Description = item.ReadString(BiosSet.DescriptionKey),
                Default = item.ReadString(BiosSet.DefaultKey),
            };
            return biosset;
        }

        /// <summary>
        /// Convert from <cref="Chip"/> to <cref="Models.ClrMamePro.Chip"/>
        /// </summary>
        private static Models.ClrMamePro.Chip? ConvertToClrMamePro(Chip? item)
        {
            if (item == null)
                return null;

            var chip = new Models.ClrMamePro.Chip
            {
                Type = item.ReadString(Chip.ChipTypeKey),
                Name = item.ReadString(Chip.NameKey),
                Flags = item.ReadString(Chip.FlagsKey),
                Clock = item.ReadString(Chip.ClockKey),
            };
            return chip;
        }

        /// <summary>
        /// Convert from <cref="DipSwitch"/> to <cref="Models.ClrMamePro.DipSwitch"/>
        /// </summary>
        private static Models.ClrMamePro.DipSwitch? ConvertToClrMamePro(DipSwitch? item)
        {
            if (item == null)
                return null;

            var dipswitch = new Models.ClrMamePro.DipSwitch
            {
                Name = item.ReadString(DipSwitch.NameKey),
                Entry = item[DipSwitch.EntryKey] as string[],
                Default = item.ReadString(DipSwitch.DefaultKey),
            };
            return dipswitch;
        }

        /// <summary>
        /// Convert from <cref="Disk"/> to <cref="Models.ClrMamePro.Disk"/>
        /// </summary>
        private static Models.ClrMamePro.Disk? ConvertToClrMamePro(Disk? item)
        {
            if (item == null)
                return null;

            var disk = new Models.ClrMamePro.Disk
            {
                Name = item.ReadString(Disk.NameKey),
                MD5 = item.ReadString(Disk.MD5Key),
                SHA1 = item.ReadString(Disk.SHA1Key),
                Merge = item.ReadString(Disk.MergeKey),
                Status = item.ReadString(Disk.StatusKey),
                Flags = item.ReadString(Disk.FlagsKey),
            };
            return disk;
        }

        /// <summary>
        /// Convert from <cref="Driver"/> to <cref="Models.ClrMamePro.Driver"/>
        /// </summary>
        private static Models.ClrMamePro.Driver? ConvertToClrMamePro(Driver? item)
        {
            if (item == null)
                return null;

            var driver = new Models.ClrMamePro.Driver
            {
                Status = item.ReadString(Driver.StatusKey),
                Color = item.ReadString(Driver.ColorKey),
                Sound = item.ReadString(Driver.SoundKey),
                PaletteSize = item.ReadString(Driver.PaletteSizeKey),
                Blit = item.ReadString(Driver.BlitKey),
            };
            return driver;
        }

        /// <summary>
        /// Convert from <cref="Input"/> to <cref="Models.ClrMamePro.Input"/>
        /// </summary>
        private static Models.ClrMamePro.Input? ConvertToClrMamePro(Input? item)
        {
            if (item == null)
                return null;

            var input = new Models.ClrMamePro.Input
            {
                Players = item.ReadString(Input.PlayersKey),
                Control = item.ReadString(Input.ControlKey),
                Buttons = item.ReadString(Input.ButtonsKey),
                Coins = item.ReadString(Input.CoinsKey),
                Tilt = item.ReadString(Input.TiltKey),
                Service = item.ReadString(Input.ServiceKey),
            };
            return input;
        }

        /// <summary>
        /// Convert from <cref="Media"/> to <cref="Models.ClrMamePro.Media"/>
        /// </summary>
        private static Models.ClrMamePro.Media? ConvertToClrMamePro(Media? item)
        {
            if (item == null)
                return null;

            var media = new Models.ClrMamePro.Media
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
        /// Convert from <cref="Release"/> to <cref="Models.ClrMamePro.Release"/>
        /// </summary>
        private static Models.ClrMamePro.Release? ConvertToClrMamePro(Release? item)
        {
            if (item == null)
                return null;

            var release = new Models.ClrMamePro.Release
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
        /// Convert from <cref="Rom"/> to <cref="Models.ClrMamePro.Rom"/>
        /// </summary>
        private static Models.ClrMamePro.Rom? ConvertToClrMamePro(Rom? item)
        {
            if (item == null)
                return null;

            var rom = new Models.ClrMamePro.Rom
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
                Region = item.ReadString(Rom.RegionKey),
                Flags = item.ReadString(Rom.FlagsKey),
                Offs = item.ReadString(Rom.OffsetKey),
                Serial = item.ReadString(Rom.SerialKey),
                Header = item.ReadString(Rom.HeaderKey),
                Date = item.ReadString(Rom.DateKey),
                Inverted = item.ReadString(Rom.InvertedKey),
                MIA = item.ReadString(Rom.MIAKey),
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Sample"/> to <cref="Models.ClrMamePro.Sample"/>
        /// </summary>
        private static Models.ClrMamePro.Sample? ConvertToClrMamePro(Sample? item)
        {
            if (item == null)
                return null;

            var sample = new Models.ClrMamePro.Sample
            {
                Name = item.ReadString(Sample.NameKey),
            };
            return sample;
        }

        /// <summary>
        /// Convert from <cref="Sound"/> to <cref="Models.ClrMamePro.Sound"/>
        /// </summary>
        private static Models.ClrMamePro.Sound? ConvertToClrMamePro(Sound? item)
        {
            if (item == null)
                return null;

            var sound = new Models.ClrMamePro.Sound
            {
                Channels = item.ReadString(Sound.ChannelsKey),
            };
            return sound;
        }

        /// <summary>
        /// Convert from <cref="Video"/> to <cref="Models.ClrMamePro.Video"/>
        /// </summary>
        private static Models.ClrMamePro.Video? ConvertToClrMamePro(Video? item)
        {
            if (item == null)
                return null;

            var video = new Models.ClrMamePro.Video
            {
                Screen = item.ReadString(Video.ScreenKey),
                Orientation = item.ReadString(Video.OrientationKey),
                X = item.ReadString(Video.WidthKey),
                Y = item.ReadString(Video.HeightKey),
                AspectX = item.ReadString(Video.AspectXKey),
                AspectY = item.ReadString(Video.AspectYKey),
                Freq = item.ReadString(Video.RefreshKey),
            };
            return video;
        }

        #endregion
    }
}