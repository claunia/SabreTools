using System.Collections.Generic;
using System.Linq;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for ClrMamePro models to internal structure
    /// </summary>
    public partial class Internal
    {
        #region Serialize

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.GameBase"/> to <cref="Models.Internal.Machine"/>
        /// </summary>
        public static Models.Internal.Machine ConvertMachineFromClrMamePro(Models.ClrMamePro.GameBase item)
        {
            var machine = new Models.Internal.Machine
            {
                [Models.Internal.Machine.NameKey] = item.Name,
                [Models.Internal.Machine.DescriptionKey] = item.Description,
                [Models.Internal.Machine.YearKey] = item.Year,
                [Models.Internal.Machine.ManufacturerKey] = item.Manufacturer,
                [Models.Internal.Machine.CategoryKey] = item.Category,
                [Models.Internal.Machine.CloneOfKey] = item.CloneOf,
                [Models.Internal.Machine.RomOfKey] = item.RomOf,
                [Models.Internal.Machine.SampleOfKey] = item.SampleOf,
            };

            if (item.Release != null && item.Release.Any())
            {
                var releases = new List<Models.Internal.Release>();
                foreach (var release in item.Release)
                {
                    releases.Add(ConvertFromClrMamePro(release));
                }
                machine[Models.Internal.Machine.ReleaseKey] = releases.ToArray();
            }

            if (item.BiosSet != null && item.BiosSet.Any())
            {
                var biosSets = new List<Models.Internal.BiosSet>();
                foreach (var biosSet in item.BiosSet)
                {
                    biosSets.Add(ConvertFromClrMamePro(biosSet));
                }
                machine[Models.Internal.Machine.BiosSetKey] = biosSets.ToArray();
            }

            if (item.Rom != null && item.Rom.Any())
            {
                var roms = new List<Models.Internal.Rom>();
                foreach (var rom in item.Rom)
                {
                    roms.Add(ConvertFromClrMamePro(rom));
                }
                machine[Models.Internal.Machine.RomKey] = roms.ToArray();
            }

            if (item.Disk != null && item.Disk.Any())
            {
                var disks = new List<Models.Internal.Disk>();
                foreach (var disk in item.Disk)
                {
                    disks.Add(ConvertFromClrMamePro(disk));
                }
                machine[Models.Internal.Machine.DiskKey] = disks.ToArray();
            }

            if (item.Media != null && item.Media.Any())
            {
                var medias = new List<Models.Internal.Media>();
                foreach (var media in item.Media)
                {
                    medias.Add(ConvertFromClrMamePro(media));
                }
                machine[Models.Internal.Machine.MediaKey] = medias.ToArray();
            }

            if (item.Sample != null && item.Sample.Any())
            {
                var samples = new List<Models.Internal.Sample>();
                foreach (var sample in item.Sample)
                {
                    samples.Add(ConvertFromClrMamePro(sample));
                }
                machine[Models.Internal.Machine.SampleKey] = samples.ToArray();
            }

            if (item.Archive != null && item.Archive.Any())
            {
                var archives = new List<Models.Internal.Archive>();
                foreach (var archive in item.Archive)
                {
                    archives.Add(ConvertFromClrMamePro(archive));
                }
                machine[Models.Internal.Machine.ArchiveKey] = archives.ToArray();
            }

            if (item.Chip != null && item.Chip.Any())
            {
                var chips = new List<Models.Internal.Chip>();
                foreach (var chip in item.Chip)
                {
                    chips.Add(ConvertFromClrMamePro(chip));
                }
                machine[Models.Internal.Machine.ChipKey] = chips.ToArray();
            }

            if (item.Video != null)
                machine[Models.Internal.Machine.VideoKey] = ConvertFromClrMamePro(item.Video);

            if (item.Sound != null)
                machine[Models.Internal.Machine.SoundKey] = ConvertFromClrMamePro(item.Sound);

            if (item.Input != null)
                machine[Models.Internal.Machine.InputKey] = ConvertFromClrMamePro(item.Input);

            if (item.DipSwitch != null && item.DipSwitch.Any())
            {
                var dipSwitches = new List<Models.Internal.DipSwitch>();
                foreach (var dipSwitch in item.DipSwitch)
                {
                    dipSwitches.Add(ConvertFromClrMamePro(dipSwitch));
                }
                machine[Models.Internal.Machine.DipSwitchKey] = dipSwitches.ToArray();
            }

            if (item.Driver != null)
                machine[Models.Internal.Machine.DriverKey] = ConvertFromClrMamePro(item.Driver);

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Archive"/> to <cref="Models.Internal.Archive"/>
        /// </summary>
        public static Models.Internal.Archive ConvertFromClrMamePro(Models.ClrMamePro.Archive item)
        {
            var archive = new Models.Internal.Archive
            {
                [Models.Internal.Archive.NameKey] = item.Name,
            };
            return archive;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.BiosSet"/> to <cref="Models.Internal.BiosSet"/>
        /// </summary>
        public static Models.Internal.BiosSet ConvertFromClrMamePro(Models.ClrMamePro.BiosSet item)
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
        /// Convert from <cref="Models.ClrMamePro.Chip"/> to <cref="Models.Internal.Chip"/>
        /// </summary>
        public static Models.Internal.Chip ConvertFromClrMamePro(Models.ClrMamePro.Chip item)
        {
            var chip = new Models.Internal.Chip
            {
                [Models.Internal.Chip.ChipTypeKey] = item.Type,
                [Models.Internal.Chip.NameKey] = item.Name,
                [Models.Internal.Chip.FlagsKey] = item.Flags,
                [Models.Internal.Chip.ClockKey] = item.Clock,
            };
            return chip;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.DipSwitch"/> to <cref="Models.Internal.DipSwitch"/>
        /// </summary>
        public static Models.Internal.DipSwitch ConvertFromClrMamePro(Models.ClrMamePro.DipSwitch item)
        {
            var dipswitch = new Models.Internal.DipSwitch
            {
                [Models.Internal.DipSwitch.NameKey] = item.Name,
                [Models.Internal.DipSwitch.EntryKey] = item.Entry,
                [Models.Internal.DipSwitch.DefaultKey] = item.Default,
            };
            return dipswitch;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Disk"/> to <cref="Models.Internal.Disk"/>
        /// </summary>
        public static Models.Internal.Disk ConvertFromClrMamePro(Models.ClrMamePro.Disk item)
        {
            var disk = new Models.Internal.Disk
            {
                [Models.Internal.Disk.NameKey] = item.Name,
                [Models.Internal.Disk.MD5Key] = item.MD5,
                [Models.Internal.Disk.SHA1Key] = item.SHA1,
                [Models.Internal.Disk.MergeKey] = item.Merge,
                [Models.Internal.Disk.StatusKey] = item.Status,
                [Models.Internal.Disk.FlagsKey] = item.Flags,
            };
            return disk;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Driver"/> to <cref="Models.Internal.Driver"/>
        /// </summary>
        public static Models.Internal.Driver ConvertFromClrMamePro(Models.ClrMamePro.Driver item)
        {
            var driver = new Models.Internal.Driver
            {
                [Models.Internal.Driver.StatusKey] = item.Status,
                [Models.Internal.Driver.ColorKey] = item.Color,
                [Models.Internal.Driver.SoundKey] = item.Sound,
                [Models.Internal.Driver.PaletteSizeKey] = item.PaletteSize,
                [Models.Internal.Driver.BlitKey] = item.Blit,
            };
            return driver;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Input"/> to <cref="Models.Internal.Input"/>
        /// </summary>
        public static Models.Internal.Input ConvertFromClrMamePro(Models.ClrMamePro.Input item)
        {
            var input = new Models.Internal.Input
            {
                [Models.Internal.Input.PlayersKey] = item.Players,
                [Models.Internal.Input.ControlKey] = item.Control,
                [Models.Internal.Input.ButtonsKey] = item.Buttons,
                [Models.Internal.Input.CoinsKey] = item.Coins,
                [Models.Internal.Input.TiltKey] = item.Tilt,
                [Models.Internal.Input.ServiceKey] = item.Service,
            };
            return input;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Media"/> to <cref="Models.Internal.Media"/>
        /// </summary>
        public static Models.Internal.Media ConvertFromClrMamePro(Models.ClrMamePro.Media item)
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
        /// Convert from <cref="Models.ClrMamePro.Release"/> to <cref="Models.Internal.Release"/>
        /// </summary>
        public static Models.Internal.Release ConvertFromClrMamePro(Models.ClrMamePro.Release item)
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
        /// Convert from <cref="Models.ClrMamePro.Rom"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Rom ConvertFromClrMamePro(Models.ClrMamePro.Rom item)
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
                [Models.Internal.Rom.RegionKey] = item.Region,
                [Models.Internal.Rom.FlagsKey] = item.Flags,
                [Models.Internal.Rom.OffsetKey] = item.Offs,
                [Models.Internal.Rom.SerialKey] = item.Serial,
                [Models.Internal.Rom.HeaderKey] = item.Header,
                [Models.Internal.Rom.DateKey] = item.Date,
                [Models.Internal.Rom.InvertedKey] = item.Inverted,
                [Models.Internal.Rom.MIAKey] = item.MIA,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Sample"/> to <cref="Models.Internal.Sample"/>
        /// </summary>
        public static Models.Internal.Sample ConvertFromClrMamePro(Models.ClrMamePro.Sample item)
        {
            var sample = new Models.Internal.Sample
            {
                [Models.Internal.Sample.NameKey] = item.Name,
            };
            return sample;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Sound"/> to <cref="Models.Internal.Sound"/>
        /// </summary>
        public static Models.Internal.Sound ConvertFromClrMamePro(Models.ClrMamePro.Sound item)
        {
            var sound = new Models.Internal.Sound
            {
                [Models.Internal.Sound.ChannelsKey] = item.Channels,
            };
            return sound;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Video"/> to <cref="Models.Internal.Video"/>
        /// </summary>
        public static Models.Internal.Video ConvertFromClrMamePro(Models.ClrMamePro.Video item)
        {
            var video = new Models.Internal.Video
            {
                [Models.Internal.Video.ScreenKey] = item.Screen,
                [Models.Internal.Video.OrientationKey] = item.Orientation,
                [Models.Internal.Video.WidthKey] = item.X,
                [Models.Internal.Video.HeightKey] = item.Y,
                [Models.Internal.Video.AspectXKey] = item.AspectX,
                [Models.Internal.Video.AspectYKey] = item.AspectY,
                [Models.Internal.Video.RefreshKey] = item.Freq,
            };
            return video;
        }

        #endregion

        #region Deserialize

        /// <summary>
        /// Convert from <cref="Models.Internal.Archive"/> to <cref="Models.ClrMamePro.Archive"/>
        /// </summary>
        public static Models.ClrMamePro.GameBase? ConvertMachineToClrMamePro(Models.Internal.Machine? item, bool game = false)
        {
            if (item == null)
                return null;

            Models.ClrMamePro.GameBase gameBase = game ? new Models.ClrMamePro.Game() : new Models.ClrMamePro.Machine();

            gameBase.Name = item.ReadString(Models.Internal.Machine.NameKey);
            gameBase.Description = item.ReadString(Models.Internal.Machine.DescriptionKey);
            gameBase.Year = item.ReadString(Models.Internal.Machine.YearKey);
            gameBase.Manufacturer = item.ReadString(Models.Internal.Machine.ManufacturerKey);
            gameBase.Category = item.ReadString(Models.Internal.Machine.CategoryKey);
            gameBase.CloneOf = item.ReadString(Models.Internal.Machine.CloneOfKey);
            gameBase.RomOf = item.ReadString(Models.Internal.Machine.RomOfKey);
            gameBase.SampleOf = item.ReadString(Models.Internal.Machine.SampleOfKey);

            var releases = item.Read<Models.Internal.Release[]>(Models.Internal.Machine.ReleaseKey);
            gameBase.Release = releases?.Select(ConvertToClrMamePro)?.ToArray();

            var biosSets = item.Read<Models.Internal.BiosSet[]>(Models.Internal.Machine.BiosSetKey);
            gameBase.BiosSet = biosSets?.Select(ConvertToClrMamePro)?.ToArray();

            var roms = item.Read<Models.Internal.Rom[]>(Models.Internal.Machine.RomKey);
            gameBase.Rom = roms?.Select(ConvertToClrMamePro)?.ToArray();

            var disks = item.Read<Models.Internal.Disk[]>(Models.Internal.Machine.DiskKey);
            gameBase.Disk = disks?.Select(ConvertToClrMamePro)?.ToArray();

            var medias = item.Read<Models.Internal.Media[]>(Models.Internal.Machine.MediaKey);
            gameBase.Media = medias?.Select(ConvertToClrMamePro)?.ToArray();

            var samples = item.Read<Models.Internal.Sample[]>(Models.Internal.Machine.SampleKey);
            gameBase.Sample = samples?.Select(ConvertToClrMamePro)?.ToArray();

            var archives = item.Read<Models.Internal.Archive[]>(Models.Internal.Machine.ArchiveKey);
            gameBase.Archive = archives?.Select(ConvertToClrMamePro)?.ToArray();

            var chips = item.Read<Models.Internal.Chip[]>(Models.Internal.Machine.ChipKey);
            gameBase.Chip = chips?.Select(ConvertToClrMamePro)?.ToArray();

            var video = item.Read<Models.Internal.Video>(Models.Internal.Machine.VideoKey);
            gameBase.Video = ConvertToClrMamePro(video);

            var sound = item.Read<Models.Internal.Sound>(Models.Internal.Machine.SoundKey);
            gameBase.Sound = ConvertToClrMamePro(sound);

            var input = item.Read<Models.Internal.Input>(Models.Internal.Machine.InputKey);
            gameBase.Input = ConvertToClrMamePro(input);

            var dipSwitches = item.Read<Models.Internal.DipSwitch[]>(Models.Internal.Machine.DipSwitchKey);
            gameBase.DipSwitch = dipSwitches?.Select(ConvertToClrMamePro)?.ToArray();

            var driver = item.Read<Models.Internal.Driver>(Models.Internal.Machine.DriverKey);
            gameBase.Driver = ConvertToClrMamePro(driver);

            return gameBase;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Archive"/> to <cref="Models.ClrMamePro.Archive"/>
        /// </summary>
        private static Models.ClrMamePro.Archive? ConvertToClrMamePro(Models.Internal.Archive? item)
        {
            if (item == null)
                return null;

            var archive = new Models.ClrMamePro.Archive
            {
                Name = item.ReadString(Models.Internal.Archive.NameKey),
            };
            return archive;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.BiosSet"/> to <cref="Models.ClrMamePro.BiosSet"/>
        /// </summary>
        private static Models.ClrMamePro.BiosSet? ConvertToClrMamePro(Models.Internal.BiosSet? item)
        {
            if (item == null)
                return null;

            var biosset = new Models.ClrMamePro.BiosSet
            {
                Name = item.ReadString(Models.Internal.BiosSet.NameKey),
                Description = item.ReadString(Models.Internal.BiosSet.DescriptionKey),
                Default = item.ReadString(Models.Internal.BiosSet.DefaultKey),
            };
            return biosset;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Chip"/> to <cref="Models.ClrMamePro.Chip"/>
        /// </summary>
        private static Models.ClrMamePro.Chip? ConvertToClrMamePro(Models.Internal.Chip? item)
        {
            if (item == null)
                return null;

            var chip = new Models.ClrMamePro.Chip
            {
                Type = item.ReadString(Models.Internal.Chip.ChipTypeKey),
                Name = item.ReadString(Models.Internal.Chip.NameKey),
                Flags = item.ReadString(Models.Internal.Chip.FlagsKey),
                Clock = item.ReadString(Models.Internal.Chip.ClockKey),
            };
            return chip;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.DipSwitch"/> to <cref="Models.ClrMamePro.DipSwitch"/>
        /// </summary>
        private static Models.ClrMamePro.DipSwitch? ConvertToClrMamePro(Models.Internal.DipSwitch? item)
        {
            if (item == null)
                return null;

            var dipswitch = new Models.ClrMamePro.DipSwitch
            {
                Name = item.ReadString(Models.Internal.DipSwitch.NameKey),
                Entry = item[Models.Internal.DipSwitch.EntryKey] as string[],
                Default = item.ReadString(Models.Internal.DipSwitch.DefaultKey),
            };
            return dipswitch;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Disk"/> to <cref="Models.ClrMamePro.Disk"/>
        /// </summary>
        private static Models.ClrMamePro.Disk? ConvertToClrMamePro(Models.Internal.Disk? item)
        {
            if (item == null)
                return null;

            var disk = new Models.ClrMamePro.Disk
            {
                Name = item.ReadString(Models.Internal.Disk.NameKey),
                MD5 = item.ReadString(Models.Internal.Disk.MD5Key),
                SHA1 = item.ReadString(Models.Internal.Disk.SHA1Key),
                Merge = item.ReadString(Models.Internal.Disk.MergeKey),
                Status = item.ReadString(Models.Internal.Disk.StatusKey),
                Flags = item.ReadString(Models.Internal.Disk.FlagsKey),
            };
            return disk;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Driver"/> to <cref="Models.ClrMamePro.Driver"/>
        /// </summary>
        private static Models.ClrMamePro.Driver? ConvertToClrMamePro(Models.Internal.Driver? item)
        {
            if (item == null)
                return null;

            var driver = new Models.ClrMamePro.Driver
            {
                Status = item.ReadString(Models.Internal.Driver.StatusKey),
                Color = item.ReadString(Models.Internal.Driver.ColorKey),
                Sound = item.ReadString(Models.Internal.Driver.SoundKey),
                PaletteSize = item.ReadString(Models.Internal.Driver.PaletteSizeKey),
                Blit = item.ReadString(Models.Internal.Driver.BlitKey),
            };
            return driver;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Input"/> to <cref="Models.ClrMamePro.Input"/>
        /// </summary>
        private static Models.ClrMamePro.Input? ConvertToClrMamePro(Models.Internal.Input? item)
        {
            if (item == null)
                return null;

            var input = new Models.ClrMamePro.Input
            {
                Players = item.ReadString(Models.Internal.Input.PlayersKey),
                Control = item.ReadString(Models.Internal.Input.ControlKey),
                Buttons = item.ReadString(Models.Internal.Input.ButtonsKey),
                Coins = item.ReadString(Models.Internal.Input.CoinsKey),
                Tilt = item.ReadString(Models.Internal.Input.TiltKey),
                Service = item.ReadString(Models.Internal.Input.ServiceKey),
            };
            return input;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Media"/> to <cref="Models.ClrMamePro.Media"/>
        /// </summary>
        private static Models.ClrMamePro.Media? ConvertToClrMamePro(Models.Internal.Media? item)
        {
            if (item == null)
                return null;

            var media = new Models.ClrMamePro.Media
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
        /// Convert from <cref="Models.Internal.Release"/> to <cref="Models.ClrMamePro.Release"/>
        /// </summary>
        private static Models.ClrMamePro.Release? ConvertToClrMamePro(Models.Internal.Release? item)
        {
            if (item == null)
                return null;

            var release = new Models.ClrMamePro.Release
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
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.ClrMamePro.Rom"/>
        /// </summary>
        private static Models.ClrMamePro.Rom? ConvertToClrMamePro(Models.Internal.Rom? item)
        {
            if (item == null)
                return null;

            var rom = new Models.ClrMamePro.Rom
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
                Region = item.ReadString(Models.Internal.Rom.RegionKey),
                Flags = item.ReadString(Models.Internal.Rom.FlagsKey),
                Offs = item.ReadString(Models.Internal.Rom.OffsetKey),
                Serial = item.ReadString(Models.Internal.Rom.SerialKey),
                Header = item.ReadString(Models.Internal.Rom.HeaderKey),
                Date = item.ReadString(Models.Internal.Rom.DateKey),
                Inverted = item.ReadString(Models.Internal.Rom.InvertedKey),
                MIA = item.ReadString(Models.Internal.Rom.MIAKey),
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Sample"/> to <cref="Models.ClrMamePro.Sample"/>
        /// </summary>
        private static Models.ClrMamePro.Sample? ConvertToClrMamePro(Models.Internal.Sample? item)
        {
            if (item == null)
                return null;

            var sample = new Models.ClrMamePro.Sample
            {
                Name = item.ReadString(Models.Internal.Sample.NameKey),
            };
            return sample;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Sound"/> to <cref="Models.ClrMamePro.Sound"/>
        /// </summary>
        private static Models.ClrMamePro.Sound? ConvertToClrMamePro(Models.Internal.Sound? item)
        {
            if (item == null)
                return null;

            var sound = new Models.ClrMamePro.Sound
            {
                Channels = item.ReadString(Models.Internal.Sound.ChannelsKey),
            };
            return sound;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Video"/> to <cref="Models.ClrMamePro.Video"/>
        /// </summary>
        private static Models.ClrMamePro.Video? ConvertToClrMamePro(Models.Internal.Video? item)
        {
            if (item == null)
                return null;

            var video = new Models.ClrMamePro.Video
            {
                Screen = item.ReadString(Models.Internal.Video.ScreenKey),
                Orientation = item.ReadString(Models.Internal.Video.OrientationKey),
                X = item.ReadString(Models.Internal.Video.WidthKey),
                Y = item.ReadString(Models.Internal.Video.HeightKey),
                AspectX = item.ReadString(Models.Internal.Video.AspectXKey),
                AspectY = item.ReadString(Models.Internal.Video.AspectYKey),
                Freq = item.ReadString(Models.Internal.Video.RefreshKey),
            };
            return video;
        }

        #endregion
    }
}