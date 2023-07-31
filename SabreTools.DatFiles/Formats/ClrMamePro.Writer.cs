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
            return new ItemType[]
            {
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
            };
        }

        /// <inheritdoc/>
        protected override List<DatItemField> GetMissingRequiredFields(DatItem datItem)
        {
            var missingFields = new List<DatItemField>();
            switch (datItem)
            {
                case Release release:
                    if (string.IsNullOrWhiteSpace(release.Name))
                        missingFields.Add(DatItemField.Name);
                    if (string.IsNullOrWhiteSpace(release.Region))
                        missingFields.Add(DatItemField.Region);
                    break;

                case BiosSet biosset:
                    if (string.IsNullOrWhiteSpace(biosset.Name))
                        missingFields.Add(DatItemField.Name);
                    if (string.IsNullOrWhiteSpace(biosset.Description))
                        missingFields.Add(DatItemField.Description);
                    break;

                case Rom rom:
                    if (string.IsNullOrWhiteSpace(rom.Name))
                        missingFields.Add(DatItemField.Name);
                    if (rom.Size == null || rom.Size < 0)
                        missingFields.Add(DatItemField.Size);
                    if (string.IsNullOrWhiteSpace(rom.CRC)
                        && string.IsNullOrWhiteSpace(rom.MD5)
                        && string.IsNullOrWhiteSpace(rom.SHA1)
                        && string.IsNullOrWhiteSpace(rom.SHA256)
                        && string.IsNullOrWhiteSpace(rom.SHA384)
                        && string.IsNullOrWhiteSpace(rom.SHA512)
                        && string.IsNullOrWhiteSpace(rom.SpamSum))
                    {
                        missingFields.Add(DatItemField.SHA1);
                    }
                    break;

                case Disk disk:
                    if (string.IsNullOrWhiteSpace(disk.Name))
                        missingFields.Add(DatItemField.Name);
                    if (string.IsNullOrWhiteSpace(disk.MD5)
                        && string.IsNullOrWhiteSpace(disk.SHA1))
                    {
                        missingFields.Add(DatItemField.SHA1);
                    }
                    break;

                case Sample sample:
                    if (string.IsNullOrWhiteSpace(sample.Name))
                        missingFields.Add(DatItemField.Name);
                    break;

                case Archive archive:
                    if (string.IsNullOrWhiteSpace(archive.Name))
                        missingFields.Add(DatItemField.Name);
                    break;

                case Chip chip:
                    if (!chip.ChipTypeSpecified)
                        missingFields.Add(DatItemField.ChipType);
                    if (string.IsNullOrWhiteSpace(chip.Name))
                        missingFields.Add(DatItemField.Name);
                    break;

                case Display display:
                    if (!display.DisplayTypeSpecified)
                        missingFields.Add(DatItemField.DisplayType);
                    if (!display.RotateSpecified)
                        missingFields.Add(DatItemField.Rotate);
                    break;

                case Sound sound:
                    if (!sound.ChannelsSpecified)
                        missingFields.Add(DatItemField.Channels);
                    break;

                case Input input:
                    if (!input.PlayersSpecified)
                        missingFields.Add(DatItemField.Players);
                    if (!input.ControlsSpecified)
                        missingFields.Add(DatItemField.Control_Buttons);
                    break;

                case DipSwitch dipswitch:
                    if (string.IsNullOrWhiteSpace(dipswitch.Name))
                        missingFields.Add(DatItemField.Name);
                    break;

                case Driver driver:
                    if (!driver.StatusSpecified)
                        missingFields.Add(DatItemField.SupportStatus);
                    if (!driver.EmulationSpecified)
                        missingFields.Add(DatItemField.EmulationStatus);
                    break;
            }

            // TODO: Check required fields
            return missingFields;
        }

        /// <inheritdoc/>
        public override bool WriteToFile(string outfile, bool ignoreblanks = false, bool throwOnError = false)
        {
            try
            {
                logger.User($"Writing to '{outfile}'...");

                var metadataFile = CreateMetadataFile(ignoreblanks);
                if (!Serialization.ClrMamePro.SerializeToFile(metadataFile, outfile, Quotes))
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
                Name = Header.Name,
                Description = Header.Description,
                RootDir = Header.RootDir,
                Category = Header.Category,
                Version = Header.Version,
                Date = Header.Date,
                Author = Header.Author,
                Homepage = Header.Homepage,
                Url = Header.Url,
                Comment = Header.Comment,
                Header = Header.HeaderSkipper,
                Type = Header.Type,
            };

            if (Header.ForceMergingSpecified)
                clrMamePro.ForceMerging = Header.ForceMerging.FromMergingFlag(romCenter: false);
            if (Header.ForcePackingSpecified)
                clrMamePro.ForcePacking = Header.ForcePacking.FromPackingFlag(yesno: false);

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
                var machine = items[0].Machine;

                // We normalize to all "game"
                var game = new Models.ClrMamePro.Game
                {
                    Name = machine.Name,
                    Description = machine.Description,
                    Year = machine.Year,
                    Manufacturer = machine.Manufacturer,
                    Category = machine.Category,
                    CloneOf = machine.CloneOf,
                    RomOf = machine.RomOf,
                    SampleOf = machine.SampleOf,
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
                var dipswitches = new List<Models.ClrMamePro.DipSwitch>();

                // Loop through and convert the items to respective lists
                foreach (var item in items)
                {
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
                            game.Video = CreateVideo(display);
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
                game.Release = releases.ToArray();
                game.BiosSet = biossets.ToArray();
                game.Rom = roms.ToArray();
                game.Disk = disks.ToArray();
                game.Media = medias.ToArray();
                game.Sample = samples.ToArray();
                game.Archive = archives.ToArray();

                // Add the game to the list
                games.Add(game);
            }

            return games.ToArray();
        }

        /// <summary>
        /// Create a Release from the current Release DatItem
        /// <summary>
        private static Models.ClrMamePro.Release CreateRelease(Release item)
        {
            var release = new Models.ClrMamePro.Release
            {
                Name = item.Name,
                Region = item.Region,
                Language = item.Language,
                Date = item.Date,
            };

            if (item.DefaultSpecified)
                release.Default = item.Default.FromYesNo();

            return release;
        }

        /// <summary>
        /// Create a BiosSet from the current BiosSet DatItem
        /// <summary>
        private static Models.ClrMamePro.BiosSet CreateBiosSet(BiosSet item)
        {
            var biosset = new Models.ClrMamePro.BiosSet
            {
                Name = item.Name,
                Description = item.Description,
            };

            if (item.DefaultSpecified)
                biosset.Default = item.Default.FromYesNo();

            return biosset;
        }

        /// <summary>
        /// Create a Rom from the current Rom DatItem
        /// <summary>
        private static Models.ClrMamePro.Rom CreateRom(Rom item)
        {
            var rom = new Models.ClrMamePro.Rom
            {
                Name = item.Name,
                Size = item.Size?.ToString(),
                CRC = item.CRC,
                MD5 = item.MD5,
                SHA1 = item.SHA1,
                SHA256 = item.SHA256,
                SHA384 = item.SHA384,
                SHA512 = item.SHA512,
                SpamSum = item.SpamSum,
                //xxHash364 = item.xxHash364, // TODO: Add to internal model
                //xxHash3128 = item.xxHash3128, // TODO: Add to internal model
                Merge = item.MergeTag,
                Region = item.Region,
                //Flags = item.Flags, // TODO: Add to internal model
                Offs = item.Offset,
                //Serial = item.Serial, // TODO: Add to internal model
                //Header = item.Header, // TODO: Add to internal model
                Date = item.Date,
            };

            if (item.ItemStatusSpecified)
                rom.Status = item.ItemStatus.FromItemStatus(yesno: false);
            if (item.InvertedSpecified)
                rom.Inverted = item.Inverted.FromYesNo();
            if (item.MIASpecified)
                rom.MIA = item.MIA.FromYesNo();

            return rom;
        }

        /// <summary>
        /// Create a Disk from the current Disk DatItem
        /// <summary>
        private static Models.ClrMamePro.Disk CreateDisk(Disk item)
        {
            var disk = new Models.ClrMamePro.Disk
            {
                Name = item.Name,
                MD5 = item.MD5,
                SHA1 = item.SHA1,
                Merge = item.MergeTag,
                //Flags = item.Flags, // TODO: Add to internal model
            };

            if (item.ItemStatusSpecified)
                disk.Status = item.ItemStatus.FromItemStatus(yesno: false);

            return disk;
        }

        /// <summary>
        /// Create a Media from the current Media DatItem
        /// <summary>
        private static Models.ClrMamePro.Media CreateMedia(Media item)
        {
            var media = new Models.ClrMamePro.Media
            {
                Name = item.Name,
                MD5 = item.MD5,
                SHA1 = item.SHA1,
                SHA256 = item.SHA256,
                SpamSum = item.SpamSum,
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
                Name = item.Name,
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
                Name = item.Name,
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
                Type = item.ChipType.FromChipType(),
                Name = item.Name,
                //Flags = item.Flags, // TODO: Add to internal model
                Clock = item.Clock?.ToString(),
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
                Screen = item.DisplayType.FromDisplayType(),
                X = item.Width?.ToString(),
                Y = item.Height?.ToString(),
                //AspectX = item.AspectX, // TODO: Add to internal model or find mapping
                //AspectY = item.AspectY, // TODO: Add to internal model or find mapping
                Freq = item.Refresh?.ToString(),
            };

            switch (item.Rotate)
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
                Channels = item.Channels?.ToString(),
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
                Players = item.Players?.ToString(),
                //Control = item.Control, // TODO: Add to internal model or find mapping
                Coins = item.Coins?.ToString(),
                Tilt = item.Tilt.FromYesNo(),
                Service = item.Service.FromYesNo(),
            };

            if (item.ControlsSpecified)
                input.Buttons = item.Controls[0].Buttons?.ToString();

            return input;
        }

        /// <summary>
        /// Create a DipSwitch from the current DipSwitch DatItem
        /// <summary>
        private static Models.ClrMamePro.DipSwitch CreateDipSwitch(DipSwitch item)
        {
            var dipswitch = new Models.ClrMamePro.DipSwitch
            {
                Name = item.Name,
            };

            if (item.ValuesSpecified)
            {
                var entries = new List<string>();
                foreach (var setting in item.Values)
                {
                    entries.Add(setting.Value);
                    if (setting.Default == true)
                        dipswitch.Default = setting.Value;
                }

                dipswitch.Entry = entries.ToArray();
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
                Status = item.Status.FromSupportStatus(),
                //Color = item.Color, // TODO: Add to internal model or find mapping
                //Sound = item.Sound, // TODO: Add to internal model or find mapping
                //PaletteSize = item.PaletteSize, // TODO: Add to internal model or find mapping
                //Blit = item.Blit, // TODO: Add to internal model or find mapping

            };
            return driver;
        }

        #endregion
    }
}