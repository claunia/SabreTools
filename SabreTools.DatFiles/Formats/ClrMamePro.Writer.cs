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
    /// Represents writing of a ClrMamePro DAT
    /// </summary>
    internal partial class ClrMamePro : DatFile
    {
        /// <inheritdoc/>
        protected override ItemType[] GetSupportedTypes()
        {
            return
            [
                ItemType.Archive,
                ItemType.BiosSet,
                ItemType.Chip,
                ItemType.DipSwitch,
                ItemType.Disk,
                ItemType.Display,
                ItemType.Driver,
                ItemType.Input,
                ItemType.Media,
                ItemType.Release,
                ItemType.Rom,
                ItemType.Sample,
                ItemType.Sound,
            ];
        }

        /// <inheritdoc/>
        protected override List<string>? GetMissingRequiredFields(DatItem datItem)
        {
            var missingFields = new List<string>();
            switch (datItem)
            {
                case Release release:
                    if (string.IsNullOrEmpty(release.GetName()))
                        missingFields.Add(Models.Metadata.Release.NameKey);
                    if (string.IsNullOrEmpty(release.GetStringFieldValue(Models.Metadata.Release.RegionKey)))
                        missingFields.Add(Models.Metadata.Release.RegionKey);
                    break;

                case BiosSet biosset:
                    if (string.IsNullOrEmpty(biosset.GetName()))
                        missingFields.Add(Models.Metadata.BiosSet.NameKey);
                    if (string.IsNullOrEmpty(biosset.GetStringFieldValue(Models.Metadata.BiosSet.DescriptionKey)))
                        missingFields.Add(Models.Metadata.BiosSet.DescriptionKey);
                    break;

                case Rom rom:
                    if (string.IsNullOrEmpty(rom.GetName()))
                        missingFields.Add(Models.Metadata.Rom.NameKey);
                    if (rom.GetInt64FieldValue(Models.Metadata.Rom.SizeKey) == null || rom.GetInt64FieldValue(Models.Metadata.Rom.SizeKey) < 0)
                        missingFields.Add(Models.Metadata.Rom.SizeKey);
                    if (string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.CRCKey))
                        && string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.MD5Key))
                        && string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA1Key))
                        && string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA256Key))
                        && string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA384Key))
                        && string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA512Key))
                        && string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SpamSumKey)))
                    {
                        missingFields.Add(Models.Metadata.Rom.SHA1Key);
                    }
                    break;

                case Disk disk:
                    if (string.IsNullOrEmpty(disk.GetName()))
                        missingFields.Add(Models.Metadata.Disk.NameKey);
                    if (string.IsNullOrEmpty(disk.GetStringFieldValue(Models.Metadata.Disk.MD5Key))
                        && string.IsNullOrEmpty(disk.GetStringFieldValue(Models.Metadata.Disk.SHA1Key)))
                    {
                        missingFields.Add(Models.Metadata.Disk.SHA1Key);
                    }
                    break;

                case Sample sample:
                    if (string.IsNullOrEmpty(sample.GetName()))
                        missingFields.Add(Models.Metadata.Sample.NameKey);
                    break;

                case Archive archive:
                    if (string.IsNullOrEmpty(archive.GetName()))
                        missingFields.Add(Models.Metadata.Archive.NameKey);
                    break;

                case Chip chip:
                    if (chip.GetFieldValue<ChipType>(Models.Metadata.Chip.ChipTypeKey) == ChipType.NULL)
                        missingFields.Add(Models.Metadata.Chip.ChipTypeKey);
                    if (string.IsNullOrEmpty(chip.GetName()))
                        missingFields.Add(Models.Metadata.Chip.NameKey);
                    break;

                case Display display:
                    if (display.GetFieldValue<DisplayType>(Models.Metadata.Display.DisplayTypeKey) == DisplayType.NULL)
                        missingFields.Add(Models.Metadata.Display.DisplayTypeKey);
                    if (display.GetInt64FieldValue(Models.Metadata.Display.RotateKey) == null)
                        missingFields.Add(Models.Metadata.Display.RotateKey);
                    break;

                case Sound sound:
                    if (sound.GetInt64FieldValue(Models.Metadata.Sound.ChannelsKey) == null)
                        missingFields.Add(Models.Metadata.Sound.ChannelsKey);
                    break;

                case Input input:
                    if (input.GetInt64FieldValue(Models.Metadata.Input.PlayersKey) == null)
                        missingFields.Add(Models.Metadata.Input.PlayersKey);
                    if (!input.ControlsSpecified)
                        missingFields.Add(Models.Metadata.Input.ControlKey);
                    break;

                case DipSwitch dipswitch:
                    if (string.IsNullOrEmpty(dipswitch.GetName()))
                        missingFields.Add(Models.Metadata.DipSwitch.NameKey);
                    break;

                case Driver driver:
                    if (driver.GetFieldValue<SupportStatus>(Models.Metadata.Driver.StatusKey) == SupportStatus.NULL)
                        missingFields.Add(Models.Metadata.Driver.StatusKey);
                    if (driver.GetFieldValue<SupportStatus>(Models.Metadata.Driver.EmulationKey) == SupportStatus.NULL)
                        missingFields.Add(Models.Metadata.Driver.EmulationKey);
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

                //var metadataFile = CreateMetadataFile(ignoreblanks);

                // Serialize the input file
                var metadata = ConvertMetadata(ignoreblanks);
                var metadataFile = new Serialization.CrossModel.ClrMamePro().Deserialize(metadata);
                if (!(new Serialization.Files.ClrMamePro().Serialize(metadataFile, outfile, Quotes)))
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
        /// Create a MetadataFile from the current internal information
        /// <summary>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise</param>
        private Models.ClrMamePro.MetadataFile CreateMetadataFile(bool ignoreblanks)
        {
            var metadataFile = new Models.ClrMamePro.MetadataFile
            {
                ClrMamePro = CreateClrMamePro(),
                Game = CreateGames(ignoreblanks)
            };
            return metadataFile;
        }

        /// <summary>
        /// Create a ClrMamePro from the current internal information
        /// <summary>
        private Models.ClrMamePro.ClrMamePro? CreateClrMamePro()
        {
            // If we don't have a header, we can't do anything
            if (this.Header == null)
                return null;

            var clrMamePro = new Models.ClrMamePro.ClrMamePro
            {
                Name = Header.GetStringFieldValue(Models.Metadata.Header.NameKey),
                Description = Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey),
                RootDir = Header.GetStringFieldValue(Models.Metadata.Header.RootDirKey),
                Category = Header.GetStringFieldValue(Models.Metadata.Header.CategoryKey),
                Version = Header.GetStringFieldValue(Models.Metadata.Header.VersionKey),
                Date = Header.GetStringFieldValue(Models.Metadata.Header.DateKey),
                Author = Header.GetStringFieldValue(Models.Metadata.Header.AuthorKey),
                Homepage = Header.GetStringFieldValue(Models.Metadata.Header.HomepageKey),
                Url = Header.GetStringFieldValue(Models.Metadata.Header.UrlKey),
                Comment = Header.GetStringFieldValue(Models.Metadata.Header.CommentKey),
                Header = Header.GetStringFieldValue(Models.Metadata.Header.HeaderKey),
                Type = Header.GetStringFieldValue(Models.Metadata.Header.TypeKey),
            };

            if (Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.ForceMergingKey) != MergingFlag.None)
                clrMamePro.ForceMerging = Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.ForceMergingKey).AsStringValue<MergingFlag>(useSecond: false);
            if (Header.GetFieldValue<PackingFlag>(Models.Metadata.Header.ForcePackingKey) != PackingFlag.None)
                clrMamePro.ForcePacking = Header.GetFieldValue<PackingFlag>(Models.Metadata.Header.ForcePackingKey).AsStringValue<PackingFlag>(useSecond: false);

            return clrMamePro;
        }

        /// <summary>
        /// Create an array of GameBase from the current internal information
        /// <summary>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise</param>
        private Models.ClrMamePro.GameBase[]? CreateGames(bool ignoreblanks)
        {
            // If we don't have items, we can't do anything
            if (this.Items == null || !this.Items.Any())
                return null;

            // Create a list of hold the games
            var games = new List<Models.ClrMamePro.GameBase>();

            // Loop through the sorted items and create games for them
            foreach (string key in Items.SortedKeys)
            {
                var items = Items.FilteredItems(key);
                if (items == null || !items.Any())
                    continue;

                // Get the first item for game information
                var machine = items[0].GetFieldValue<Machine>(DatItem.MachineKey);

                // We normalize to all "game"
                var game = new Models.ClrMamePro.Game
                {
                    Name = machine?.GetStringFieldValue(Models.Metadata.Machine.NameKey),
                    Description = machine?.GetStringFieldValue(Models.Metadata.Machine.DescriptionKey),
                    Year = machine?.GetStringFieldValue(Models.Metadata.Machine.YearKey),
                    Manufacturer = machine?.GetStringFieldValue(Models.Metadata.Machine.ManufacturerKey),
                    Category = machine?.GetStringFieldValue(Models.Metadata.Machine.CategoryKey),
                    CloneOf = machine?.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey),
                    RomOf = machine?.GetStringFieldValue(Models.Metadata.Machine.RomOfKey),
                    SampleOf = machine?.GetStringFieldValue(Models.Metadata.Machine.SampleOfKey),
                };

                // Create holders for all item types
                var releases = new List<Models.ClrMamePro.Release>();
                var biossets = new List<Models.ClrMamePro.BiosSet>();
                var roms = new List<Models.ClrMamePro.Rom>();
                var disks = new List<Models.ClrMamePro.Disk>();
                var medias = new List<Models.ClrMamePro.Media>();
                var samples = new List<Models.ClrMamePro.Sample>();
                var archives = new List<Models.ClrMamePro.Archive>();
                var chips = new List<Models.ClrMamePro.Chip>();
                var videos = new List<Models.ClrMamePro.Video>();
                var dipswitches = new List<Models.ClrMamePro.DipSwitch>();

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
                        case Release release:
                            releases.Add(CreateRelease(release));
                            break;
                        case BiosSet biosset:
                            biossets.Add(CreateBiosSet(biosset));
                            break;
                        case Rom rom:
                            roms.Add(CreateRom(rom));
                            break;
                        case Disk disk:
                            disks.Add(CreateDisk(disk));
                            break;
                        case Media media:
                            medias.Add(CreateMedia(media));
                            break;
                        case Sample sample:
                            samples.Add(CreateSample(sample));
                            break;
                        case Archive archive:
                            archives.Add(CreateArchive(archive));
                            break;
                        case Chip chip:
                            chips.Add(CreateChip(chip));
                            break;
                        case Display display:
                            videos.Add(CreateVideo(display));
                            break;
                        case Sound sound:
                            game.Sound = CreateSound(sound);
                            break;
                        case Input input:
                            game.Input = CreateInput(input);
                            break;
                        case DipSwitch dipswitch:
                            dipswitches.Add(CreateDipSwitch(dipswitch));
                            break;
                        case Driver driver:
                            game.Driver = CreateDriver(driver);
                            break;
                    }
                }

                // Assign the values to the game
                game.Release = [.. releases];
                game.BiosSet = [.. biossets];
                game.Rom = [.. roms];
                game.Disk = [.. disks];
                game.Media = [.. medias];
                game.Sample = [.. samples];
                game.Archive = [.. archives];
                game.Chip = [.. chips];
                game.Video = [.. videos];
                game.DipSwitch = [.. dipswitches];

                // Add the game to the list
                games.Add(game);
            }

            return [.. games];
        }

        /// <summary>
        /// Create a Release from the current Release DatItem
        /// <summary>
        private static Models.ClrMamePro.Release CreateRelease(Release item)
        {
            var release = new Models.ClrMamePro.Release
            {
                Name = item.GetName(),
                Region = item.GetStringFieldValue(Models.Metadata.Release.RegionKey),
                Language = item.GetStringFieldValue(Models.Metadata.Release.LanguageKey),
                Date = item.GetStringFieldValue(Models.Metadata.Release.DateKey),
                Default = item.GetBoolFieldValue(Models.Metadata.Release.DefaultKey).FromYesNo(),
            };

            return release;
        }

        /// <summary>
        /// Create a BiosSet from the current BiosSet DatItem
        /// <summary>
        private static Models.ClrMamePro.BiosSet CreateBiosSet(BiosSet item)
        {
            var biosset = new Models.ClrMamePro.BiosSet
            {
                Name = item.GetName(),
                Default = item.GetBoolFieldValue(Models.Metadata.BiosSet.DefaultKey).FromYesNo(),
                Description = item.GetStringFieldValue(Models.Metadata.BiosSet.DescriptionKey),
            };

            return biosset;
        }

        /// <summary>
        /// Create a Rom from the current Rom DatItem
        /// <summary>
        private static Models.ClrMamePro.Rom CreateRom(Rom item)
        {
            var rom = new Models.ClrMamePro.Rom
            {
                Name = item.GetName(),
                Size = item.GetStringFieldValue(Models.Metadata.Rom.SizeKey),
                CRC = item.GetStringFieldValue(Models.Metadata.Rom.CRCKey),
                MD5 = item.GetStringFieldValue(Models.Metadata.Rom.MD5Key),
                SHA1 = item.GetStringFieldValue(Models.Metadata.Rom.SHA1Key),
                SHA256 = item.GetStringFieldValue(Models.Metadata.Rom.SHA256Key),
                SHA384 = item.GetStringFieldValue(Models.Metadata.Rom.SHA384Key),
                SHA512 = item.GetStringFieldValue(Models.Metadata.Rom.SHA512Key),
                SpamSum = item.GetStringFieldValue(Models.Metadata.Rom.SpamSumKey),
                xxHash364 = item.GetStringFieldValue(Models.Metadata.Rom.xxHash364Key),
                xxHash3128 = item.GetStringFieldValue(Models.Metadata.Rom.xxHash3128Key),
                Merge = item.GetStringFieldValue(Models.Metadata.Rom.MergeKey),
                Region = item.GetStringFieldValue(Models.Metadata.Rom.RegionKey),
                Flags = item.GetStringFieldValue(Models.Metadata.Rom.FlagsKey),
                Date = item.GetStringFieldValue(Models.Metadata.Rom.DateKey),
                Offs = item.GetStringFieldValue(Models.Metadata.Rom.OffsetKey),
                Serial = item.GetStringFieldValue(Models.Metadata.Rom.SerialKey),
                Header = item.GetStringFieldValue(Models.Metadata.Rom.HeaderKey),
                Inverted = item.GetBoolFieldValue(Models.Metadata.Rom.InvertedKey).FromYesNo(),
                MIA = item.GetBoolFieldValue(Models.Metadata.Rom.MIAKey).FromYesNo(),
            };

            if (item.ItemStatusSpecified)
                rom.Status = item.GetFieldValue<ItemStatus>(Models.Metadata.Rom.StatusKey).AsStringValue<ItemStatus>(useSecond: false);

            return rom;
        }

        /// <summary>
        /// Create a Disk from the current Disk DatItem
        /// <summary>
        private static Models.ClrMamePro.Disk CreateDisk(Disk item)
        {
            var disk = new Models.ClrMamePro.Disk
            {
                Name = item.GetName(),
                MD5 = item.GetStringFieldValue(Models.Metadata.Disk.MD5Key),
                SHA1 = item.GetStringFieldValue(Models.Metadata.Disk.SHA1Key),
                Merge = item.GetStringFieldValue(Models.Metadata.Disk.MergeKey),
                Flags = item.GetStringFieldValue(Models.Metadata.Disk.FlagsKey),
            };

            if (item.GetFieldValue<ItemStatus>(Models.Metadata.Disk.StatusKey) != ItemStatus.NULL)
                disk.Status = item.GetFieldValue<ItemStatus>(Models.Metadata.Disk.StatusKey).AsStringValue<ItemStatus>(useSecond: false);

            return disk;
        }

        /// <summary>
        /// Create a Media from the current Media DatItem
        /// <summary>
        private static Models.ClrMamePro.Media CreateMedia(Media item)
        {
            var media = new Models.ClrMamePro.Media
            {
                Name = item.GetName(),
                MD5 = item.GetStringFieldValue(Models.Metadata.Media.MD5Key),
                SHA1 = item.GetStringFieldValue(Models.Metadata.Media.SHA1Key),
                SHA256 = item.GetStringFieldValue(Models.Metadata.Media.SHA256Key),
                SpamSum = item.GetStringFieldValue(Models.Metadata.Media.SpamSumKey),
            };
            return media;
        }

        /// <summary>
        /// Create a Sample from the current Sample DatItem
        /// <summary>
        private static Models.ClrMamePro.Sample CreateSample(Sample item)
        {
            var sample = new Models.ClrMamePro.Sample
            {
                Name = item.GetName(),
            };
            return sample;
        }

        /// <summary>
        /// Create a Archive from the current Archive DatItem
        /// <summary>
        private static Models.ClrMamePro.Archive CreateArchive(Archive item)
        {
            var archive = new Models.ClrMamePro.Archive
            {
                Name = item.GetName(),
            };
            return archive;
        }

        /// <summary>
        /// Create a Chip from the current Chip DatItem
        /// <summary>
        private static Models.ClrMamePro.Chip CreateChip(Chip item)
        {
            var chip = new Models.ClrMamePro.Chip
            {
                Type = item.GetFieldValue<ChipType>(Models.Metadata.Chip.ChipTypeKey).AsStringValue<ChipType>(),
                Name = item.GetName(),
                Flags = item.GetStringFieldValue(Models.Metadata.Chip.FlagsKey),
                Clock = item.GetInt64FieldValue(Models.Metadata.Chip.ClockKey)?.ToString(),
            };
            return chip;
        }

        /// <summary>
        /// Create a Video from the current Display DatItem
        /// <summary>
        private static Models.ClrMamePro.Video CreateVideo(Display item)
        {
            var video = new Models.ClrMamePro.Video
            {
                Screen = item.GetFieldValue<DisplayType>(Models.Metadata.Display.DisplayTypeKey).AsStringValue<DisplayType>(),
                X = item.GetInt64FieldValue(Models.Metadata.Display.WidthKey)?.ToString(),
                Y = item.GetInt64FieldValue(Models.Metadata.Display.HeightKey)?.ToString(),
                AspectX = item.GetStringFieldValue("ASPECTX"),
                AspectY = item.GetStringFieldValue("ASPECTY"),
                Freq = item.GetDoubleFieldValue(Models.Metadata.Display.RefreshKey)?.ToString(),
            };

            switch (item.GetInt64FieldValue(Models.Metadata.Display.RotateKey))
            {
                case 0:
                case 180:
                    video.Orientation = "horizontal";
                    break;
                case 90:
                case 270:
                    video.Orientation = "vertical";
                    break;
            }

            return video;
        }

        /// <summary>
        /// Create a Sound from the current Sound DatItem
        /// <summary>
        private static Models.ClrMamePro.Sound CreateSound(Sound item)
        {
            var sound = new Models.ClrMamePro.Sound
            {
                Channels = item.GetInt64FieldValue(Models.Metadata.Sound.ChannelsKey)?.ToString(),
            };
            return sound;
        }

        /// <summary>
        /// Create a Input from the current Input DatItem
        /// <summary>
        private static Models.ClrMamePro.Input CreateInput(Input item)
        {
            var input = new Models.ClrMamePro.Input
            {
                Players = item.GetInt64FieldValue(Models.Metadata.Input.PlayersKey)?.ToString(),
                //Control = item.GetStringFieldValue(Models.Metadata.Input.ControlKey),
                Coins = item.GetInt64FieldValue(Models.Metadata.Input.CoinsKey)?.ToString(),
                Tilt = item.GetBoolFieldValue(Models.Metadata.Input.TiltKey).FromYesNo(),
                Service = item.GetBoolFieldValue(Models.Metadata.Input.ServiceKey).FromYesNo(),
            };

            if (item.ControlsSpecified)
                input.Buttons = item.GetFieldValue<Control[]?>(Models.Metadata.Input.ControlKey)![0].GetInt64FieldValue(Models.Metadata.Control.ButtonsKey)?.ToString();

            return input;
        }

        /// <summary>
        /// Create a DipSwitch from the current DipSwitch DatItem
        /// <summary>
        private static Models.ClrMamePro.DipSwitch CreateDipSwitch(DipSwitch item)
        {
            var dipswitch = new Models.ClrMamePro.DipSwitch
            {
                Name = item.GetName(),
            };

            if (item.ValuesSpecified)
            {
                var entries = new List<string>();
                foreach (var setting in item.GetFieldValue<DipValue[]?>(Models.Metadata.DipSwitch.DipValueKey)!)
                {
                    entries.Add(setting.GetStringFieldValue(Models.Metadata.DipValue.ValueKey)!);
                    if (setting.GetBoolFieldValue(Models.Metadata.DipValue.DefaultKey) == true)
                        dipswitch.Default = setting.GetStringFieldValue(Models.Metadata.DipValue.ValueKey);
                }

                dipswitch.Entry = [.. entries];
            }

            return dipswitch;
        }

        /// <summary>
        /// Create a Driver from the current Driver DatItem
        /// <summary>
        private static Models.ClrMamePro.Driver CreateDriver(Driver item)
        {
            var driver = new Models.ClrMamePro.Driver
            {
                Status = item.GetFieldValue<SupportStatus>(Models.Metadata.Driver.StatusKey).AsStringValue<SupportStatus>(),
                Color = item.GetFieldValue<SupportStatus>(Models.Metadata.Driver.ColorKey).AsStringValue<SupportStatus>(),
                Sound = item.GetFieldValue<SupportStatus>(Models.Metadata.Driver.SoundKey).AsStringValue<SupportStatus>(),
                PaletteSize = item.GetInt64FieldValue(Models.Metadata.Driver.PaletteSizeKey)?.ToString(),
                Blit = item.GetStringFieldValue(Models.Metadata.Driver.BlitKey),
            };
            return driver;
        }

        #endregion
    }
}
