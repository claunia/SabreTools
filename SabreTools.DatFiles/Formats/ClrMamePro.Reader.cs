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
    /// Represents parsing of a ClrMamePro DAT
    /// </summary>
    internal partial class ClrMamePro : DatFile
    {
        /// <inheritdoc/>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            try
            {
                // Deserialize the input file
                var metadataFile = new Serialization.Files.ClrMamePro().Deserialize(filename, this.Quotes);

                // Convert the header to the internal format
                ConvertHeader(metadataFile?.ClrMamePro, keep);

                // Convert the game data to the internal format
                ConvertGames(metadataFile?.Game, filename, indexId, statsOnly);
            }
            catch (Exception ex) when (!throwOnError)
            {
                string message = $"'{filename}' - An error occurred during parsing";
                logger.Error(ex, message);
            }
        }

        #region Converters

        /// <summary>
        /// Convert header information
        /// </summary>
        /// <param name="cmp">Deserialized model to convert</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        private void ConvertHeader(Models.ClrMamePro.ClrMamePro? cmp, bool keep)
        {
            // If the header is missing, we can't do anything
            if (cmp == null)
                return;

            Header.Name ??= cmp.Name;
            Header.Description ??= cmp.Description;
            Header.RootDir ??= cmp.RootDir;
            Header.Category ??= cmp.Category;
            Header.Version ??= cmp.Version;
            Header.Date ??= cmp.Date;
            Header.Author ??= cmp.Author;
            Header.Homepage ??= cmp.Homepage;
            Header.Url ??= cmp.Url;
            Header.Comment ??= cmp.Comment;
            Header.HeaderSkipper ??= cmp.Header;
            Header.Type ??= cmp.Type;
            if (Header.ForceMerging == MergingFlag.None)
                Header.ForceMerging = cmp.ForceMerging?.AsEnumValue<MergingFlag>() ?? MergingFlag.None;
            if (Header.ForcePacking == PackingFlag.None)
                Header.ForcePacking = cmp.ForceZipping?.AsEnumValue<PackingFlag>() ?? PackingFlag.None;
            if (Header.ForcePacking == PackingFlag.None)
                Header.ForcePacking = cmp.ForcePacking?.AsEnumValue<PackingFlag>() ?? PackingFlag.None;

            // Handle implied SuperDAT
            if (cmp.Name?.Contains(" - SuperDAT") == true && keep)
                Header.Type ??= "SuperDAT";
        }

        /// <summary>
        /// Convert games information
        /// </summary>
        /// <param name="games">Array of deserialized models to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertGames(Models.ClrMamePro.GameBase?[]? games, string filename, int indexId, bool statsOnly)
        {
            // If the game array is missing, we can't do anything
            if (games == null || !games.Any())
                return;

            // Loop through the games and add
            foreach (var game in games)
            {
                ConvertGame(game, filename, indexId, statsOnly);
            }
        }

        /// <summary>
        /// Convert game information
        /// </summary>
        /// <param name="game">Deserialized model to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertGame(Models.ClrMamePro.GameBase? game, string filename, int indexId, bool statsOnly)
        {
            // If the game is missing, we can't do anything
            if (game == null)
                return;

            // Create the machine for copying information
            var machine = new Machine
            {
                Name = game.Name,
                Description = game.Description,
                Year = game.Year,
                Manufacturer = game.Manufacturer,
                Category = game.Category,
                CloneOf = game.CloneOf,
                RomOf = game.RomOf,
                SampleOf = game.SampleOf,
                MachineType = (game is Models.ClrMamePro.Resource ? MachineType.Bios : MachineType.None),
            };

            // Check if there are any items
            bool containsItems = false;

            // Loop through each type of item
            ConvertReleases(game.Release, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertBiosSets(game.BiosSet, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertRoms(game.Rom, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertDisks(game.Disk, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertMedia(game.Media, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertArchives(game.Archive, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertChips(game.Chip, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertVideos(game.Video, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertSound(game.Sound, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertInput(game.Input, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertDipSwitches(game.DipSwitch, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertDriver(game.Driver, machine, filename, indexId, statsOnly, ref containsItems);

            // If we had no items, create a Blank placeholder
            if (!containsItems)
            {
                var blank = new Blank
                {
                    Source = new Source { Index = indexId, Name = filename },
                };

                blank.CopyMachineInformation(machine);
                ParseAddHelper(blank, statsOnly);
            }
        }

        /// <summary>
        /// Convert Release information
        /// </summary>
        /// <param name="releases">Array of deserialized models to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertReleases(Models.ClrMamePro.Release[]? releases, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the release array is missing, we can't do anything
            if (releases == null || !releases.Any())
                return;

            containsItems = true;
            foreach (var release in releases)
            {
                var item = new Release
                {
                    Source = new Source { Index = indexId, Name = filename },
                };
                item.SetName(release.Name);
                item.SetFieldValue<string?>(Models.Metadata.Release.DateKey, release.Date);
                item.SetFieldValue<bool?>(Models.Metadata.Release.DefaultKey, release.Default?.AsYesNo());
                item.SetFieldValue<string?>(Models.Metadata.Release.LanguageKey, release.Language);
                item.SetFieldValue<string?>(Models.Metadata.Release.RegionKey, release.Region);

                item.CopyMachineInformation(machine);
                ParseAddHelper(item, statsOnly);
            }
        }

        /// <summary>
        /// Convert BiosSet information
        /// </summary>
        /// <param name="biossets">Array of deserialized models to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertBiosSets(Models.ClrMamePro.BiosSet[]? biossets, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the biosset array is missing, we can't do anything
            if (biossets == null || !biossets.Any())
                return;

            containsItems = true;
            foreach (var biosset in biossets)
            {
                var item = new BiosSet
                {
                    Source = new Source { Index = indexId, Name = filename },
                };
                item.SetName(biosset.Name);
                item.SetFieldValue<bool?>(Models.Metadata.BiosSet.DefaultKey, biosset.Default?.AsYesNo());
                item.SetFieldValue<string?>(Models.Metadata.BiosSet.DescriptionKey, biosset.Description);

                item.CopyMachineInformation(machine);
                ParseAddHelper(item, statsOnly);
            }
        }

        /// <summary>
        /// Convert Rom information
        /// </summary>
        /// <param name="roms">Array of deserialized models to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertRoms(Models.ClrMamePro.Rom[]? roms, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the rom array is missing, we can't do anything
            if (roms == null || !roms.Any())
                return;

            containsItems = true;
            foreach (var rom in roms)
            {
                var item = new Rom
                {
                    Source = new Source { Index = indexId, Name = filename },
                };
                item.SetName(rom.Name);
                item.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, rom.CRC);
                item.SetFieldValue<string?>(Models.Metadata.Rom.DateKey, rom.Date);
                item.SetFieldValue<string?>(Models.Metadata.Rom.FlagsKey, rom.Flags);
                item.SetFieldValue<string?>(Models.Metadata.Rom.HeaderKey, rom.Header);
                item.SetFieldValue<bool?>(Models.Metadata.Rom.InvertedKey, rom.Inverted?.AsYesNo());
                item.SetFieldValue<bool?>(Models.Metadata.Rom.MIAKey, rom.MIA?.AsYesNo());
                item.SetFieldValue<string?>(Models.Metadata.Rom.MD5Key, rom.MD5);
                item.SetFieldValue<string?>(Models.Metadata.Rom.MergeKey, rom.Merge);
                item.SetFieldValue<string?>(Models.Metadata.Rom.OffsetKey, rom.Offs);
                item.SetFieldValue<string?>(Models.Metadata.Rom.RegionKey, rom.Region);
                item.SetFieldValue<string?>(Models.Metadata.Rom.SerialKey, rom.Serial);
                item.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, rom.SHA1);
                item.SetFieldValue<string?>(Models.Metadata.Rom.SHA256Key, rom.SHA256);
                item.SetFieldValue<string?>(Models.Metadata.Rom.SHA384Key, rom.SHA384);
                item.SetFieldValue<string?>(Models.Metadata.Rom.SHA512Key, rom.SHA512);
                item.SetFieldValue<long?>(Models.Metadata.Rom.SizeKey, NumberHelper.ConvertToInt64(rom.Size));
                item.SetFieldValue<string?>(Models.Metadata.Rom.SpamSumKey, rom.SpamSum);
                item.SetFieldValue<ItemStatus?>(Models.Metadata.Rom.StatusKey, rom.Status?.AsEnumValue<ItemStatus>() ?? ItemStatus.NULL);
                item.SetFieldValue<string?>(Models.Metadata.Rom.xxHash364Key, rom.xxHash364);
                item.SetFieldValue<string?>(Models.Metadata.Rom.xxHash3128Key, rom.xxHash3128);

                item.CopyMachineInformation(machine);
                ParseAddHelper(item, statsOnly);
            }
        }

        /// <summary>
        /// Convert Disk information
        /// </summary>
        /// <param name="disks">Array of deserialized models to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertDisks(Models.ClrMamePro.Disk[]? disks, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the disk array is missing, we can't do anything
            if (disks == null || !disks.Any())
                return;

            containsItems = true;
            foreach (var disk in disks)
            {
                var item = new Disk
                {
                    Source = new Source { Index = indexId, Name = filename },
                };
                item.SetName(disk.Name);
                item.SetFieldValue<string?>(Models.Metadata.Disk.FlagsKey, disk.Flags);
                item.SetFieldValue<string?>(Models.Metadata.Disk.MD5Key, disk.MD5);
                item.SetFieldValue<string?>(Models.Metadata.Disk.MergeKey, disk.Merge);
                item.SetFieldValue<string?>(Models.Metadata.Disk.SHA1Key, disk.SHA1);
                item.SetFieldValue<ItemStatus>(Models.Metadata.Disk.StatusKey, disk.Status?.AsEnumValue<ItemStatus>() ?? ItemStatus.NULL);

                item.CopyMachineInformation(machine);
                ParseAddHelper(item, statsOnly);
            }
        }

        /// <summary>
        /// Convert Media information
        /// </summary>
        /// <param name="media">Array of deserialized models to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertMedia(Models.ClrMamePro.Media[]? media, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the media array is missing, we can't do anything
            if (media == null || !media.Any())
                return;

            containsItems = true;
            foreach (var medium in media)
            {
                var item = new Media
                {
                    Source = new Source { Index = indexId, Name = filename },
                };
                item.SetName(medium.Name);
                item.SetFieldValue<string?>(Models.Metadata.Media.MD5Key, medium.MD5);
                item.SetFieldValue<string?>(Models.Metadata.Media.SHA1Key, medium.SHA1);
                item.SetFieldValue<string?>(Models.Metadata.Media.SHA256Key, medium.SHA256);
                item.SetFieldValue<string?>(Models.Metadata.Media.SpamSumKey, medium.SpamSum);

                item.CopyMachineInformation(machine);
                ParseAddHelper(item, statsOnly);
            }
        }

        /// <summary>
        /// Convert Archive information
        /// </summary>
        /// <param name="archives">Array of deserialized models to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertArchives(Models.ClrMamePro.Archive[]? archives, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the archive array is missing, we can't do anything
            if (archives == null || !archives.Any())
                return;

            containsItems = true;
            foreach (var archive in archives)
            {
                var item = new Archive
                {
                    Source = new Source { Index = indexId, Name = filename },
                };
                item.SetName(archive.Name);

                item.CopyMachineInformation(machine);
                ParseAddHelper(item, statsOnly);
            }
        }

        /// <summary>
        /// Convert Chip information
        /// </summary>
        /// <param name="chips">Array of deserialized models to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertChips(Models.ClrMamePro.Chip[]? chips, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the chip array is missing, we can't do anything
            if (chips == null || !chips.Any())
                return;

            containsItems = true;
            foreach (var chip in chips)
            {
                var item = new Chip
                {
                    Source = new Source { Index = indexId, Name = filename },
                };
                item.SetName(chip.Name);
                item.SetFieldValue<ChipType>(Models.Metadata.Chip.ChipTypeKey, chip.Type?.AsEnumValue<ChipType>() ?? ChipType.NULL);
                item.SetFieldValue<long?>(Models.Metadata.Chip.ClockKey, NumberHelper.ConvertToInt64(chip.Clock));
                item.SetFieldValue<string?>(Models.Metadata.Chip.FlagsKey, chip.Flags);

                item.CopyMachineInformation(machine);
                ParseAddHelper(item, statsOnly);
            }
        }

        /// <summary>
        /// Convert Video information
        /// </summary>
        /// <param name="video">Array of deserialized models to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertVideos(Models.ClrMamePro.Video[]? videos, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the video array is missing, we can't do anything
            if (videos == null || !videos.Any())
                return;

            containsItems = true;
            foreach (var video in videos)
            {
                var item = new Display
                {
                    Source = new Source { Index = indexId, Name = filename },
                };
                item.SetFieldValue<long?>("ASPECTX", NumberHelper.ConvertToInt64(video.AspectX));
                item.SetFieldValue<long?>("ASPECTY", NumberHelper.ConvertToInt64(video.AspectY));
                item.SetFieldValue<DisplayType>(Models.Metadata.Display.DisplayTypeKey, video.Screen?.AsEnumValue<DisplayType>() ?? DisplayType.NULL);
                item.SetFieldValue<long?>(Models.Metadata.Display.HeightKey, NumberHelper.ConvertToInt64(video.Y));
                item.SetFieldValue<double?>(Models.Metadata.Display.RefreshKey, NumberHelper.ConvertToDouble(video.Freq));
                item.SetFieldValue<long?>(Models.Metadata.Display.WidthKey, NumberHelper.ConvertToInt64(video.X));

                switch (video.Orientation)
                {
                    case "horizontal":
                        item.SetFieldValue<long?>(Models.Metadata.Display.RotateKey, 0);
                        break;
                    case "vertical":
                        item.SetFieldValue<long?>(Models.Metadata.Display.RotateKey, 90);
                        break;
                }

                item.CopyMachineInformation(machine);
                ParseAddHelper(item, statsOnly);
            }
        }

        /// <summary>
        /// Convert Sound information
        /// </summary>
        /// <param name="sound">Deserialized model to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertSound(Models.ClrMamePro.Sound? sound, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the sound is missing, we can't do anything
            if (sound == null)
                return;

            containsItems = true;
            var item = new Sound
            {
                Source = new Source { Index = indexId, Name = filename },
            };
            item.SetFieldValue<long?>(Models.Metadata.Sound.ChannelsKey, NumberHelper.ConvertToInt64(sound.Channels));

            item.CopyMachineInformation(machine);
            ParseAddHelper(item, statsOnly);
        }

        /// <summary>
        /// Convert Input information
        /// </summary>
        /// <param name="input">Deserialized model to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertInput(Models.ClrMamePro.Input? input, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the input is missing, we can't do anything
            if (input == null)
                return;

            containsItems = true;
            var item = new Input
            {
                Source = new Source { Index = indexId, Name = filename },
            };
            item.SetFieldValue<long?>(Models.Metadata.Input.CoinsKey, NumberHelper.ConvertToInt64(input.Coins));
            //item.SetFieldValue<string?>(Models.Metadata.Input.ControlKey, input.Control);
            item.SetFieldValue<long?>(Models.Metadata.Input.PlayersKey, NumberHelper.ConvertToInt64(input.Players));
            item.SetFieldValue<bool?>(Models.Metadata.Input.ServiceKey, input.Service?.AsYesNo());
            item.SetFieldValue<bool?>(Models.Metadata.Input.TiltKey, input.Tilt?.AsYesNo());

            var control = new Control();
            control.SetFieldValue<long?>(Models.Metadata.Control.ButtonsKey, NumberHelper.ConvertToInt64(input.Buttons));

            item.SetFieldValue<Control[]?>(Models.Metadata.Input.ControlKey, [control]);

            item.CopyMachineInformation(machine);
            ParseAddHelper(item, statsOnly);
        }

        /// <summary>
        /// Convert DipSwitch information
        /// </summary>
        /// <param name="dipswitches">Array of deserialized models to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertDipSwitches(Models.ClrMamePro.DipSwitch[]? dipswitches, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the dipswitch array is missing, we can't do anything
            if (dipswitches == null || !dipswitches.Any())
                return;

            containsItems = true;
            foreach (var dipswitch in dipswitches)
            {
                var item = new DipSwitch
                {
                    Source = new Source { Index = indexId, Name = filename },
                };
                item.SetName(dipswitch.Name);

                var values = new List<DipValue>();
                foreach (string entry in dipswitch.Entry ?? [])
                {
                    var dipValue = new DipValue();
                    dipValue.SetName(dipswitch.Name);
                    dipValue.SetFieldValue<bool?>(Models.Metadata.DipValue.DefaultKey, entry == dipswitch.Default);
                    dipValue.SetFieldValue<string?>(Models.Metadata.DipValue.ValueKey, entry);

                    values.Add(dipValue);
                }

                item.SetFieldValue<DipValue[]?>(Models.Metadata.DipSwitch.DipValueKey, [.. values]);

                item.CopyMachineInformation(machine);
                ParseAddHelper(item, statsOnly);
            }
        }

        /// <summary>
        /// Convert Driver information
        /// </summary>
        /// <param name="driver">Deserialized model to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertDriver(Models.ClrMamePro.Driver? driver, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the driver is missing, we can't do anything
            if (driver == null)
                return;

            containsItems = true;
            var item = new Driver
            {
                Source = new Source { Index = indexId, Name = filename },
            };
            item.SetFieldValue<string?>(Models.Metadata.Driver.BlitKey, driver.Blit);
            item.SetFieldValue<SupportStatus>(Models.Metadata.Driver.ColorKey, driver.Color?.AsEnumValue<SupportStatus>() ?? SupportStatus.NULL);
            item.SetFieldValue<long?>(Models.Metadata.Driver.PaletteSizeKey, NumberHelper.ConvertToInt64(driver.PaletteSize));
            item.SetFieldValue<SupportStatus>(Models.Metadata.Driver.SoundKey, driver.Sound?.AsEnumValue<SupportStatus>() ?? SupportStatus.NULL);
            item.SetFieldValue<SupportStatus>(Models.Metadata.Driver.StatusKey, driver.Status?.AsEnumValue<SupportStatus>() ?? SupportStatus.NULL);

            item.CopyMachineInformation(machine);
            ParseAddHelper(item, statsOnly);
        }

        #endregion
    }
}
