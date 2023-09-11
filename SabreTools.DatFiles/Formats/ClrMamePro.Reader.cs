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
                Header.ForceMerging = cmp.ForceMerging?.AsMergingFlag() ?? MergingFlag.None;
            if (Header.ForcePacking == PackingFlag.None)
                Header.ForcePacking = cmp.ForceZipping?.AsPackingFlag() ?? PackingFlag.None;
            if (Header.ForcePacking == PackingFlag.None)
                Header.ForcePacking = cmp.ForcePacking?.AsPackingFlag() ?? PackingFlag.None;

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
        private void ConvertGames(Models.ClrMamePro.GameBase[]? games, string filename, int indexId, bool statsOnly)
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
        private void ConvertGame(Models.ClrMamePro.GameBase game, string filename, int indexId, bool statsOnly)
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
                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
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
                    Name = release.Name,
                    Region = release.Region,
                    Language = release.Language,
                    Date = release.Date,
                    Default = release.Default?.AsYesNo(),

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

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
                    Name = biosset.Name,
                    Description = biosset.Description,
                    Default = biosset.Default?.AsYesNo(),

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

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
                    Name = rom.Name,
                    Size = NumberHelper.ConvertToInt64(rom.Size),
                    CRC = rom.CRC,
                    MD5 = rom.MD5,
                    SHA1 = rom.SHA1,
                    SHA256 = rom.SHA256,
                    SHA384 = rom.SHA384,
                    SHA512 = rom.SHA512,
                    SpamSum = rom.SpamSum,
                    //xxHash364 = rom.xxHash364, // TODO: Add to internal model
                    //xxHash3128 = rom.xxHash3128, // TODO: Add to internal model
                    MergeTag = rom.Merge,
                    ItemStatus = rom.Status?.AsItemStatus() ?? ItemStatus.NULL,
                    Region = rom.Region,
                    //Flags = rom.Flags, // TODO: Add to internal model
                    Offset = rom.Offs,
                    //Serial = rom.Serial, // TODO: Add to internal model
                    //Header = rom.Header, // TODO: Add to internal model
                    Date = rom.Date,
                    Inverted = rom.Inverted?.AsYesNo(),
                    MIA = rom.MIA?.AsYesNo(),

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

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
                    Name = disk.Name,
                    MD5 = disk.MD5,
                    SHA1 = disk.SHA1,
                    MergeTag = disk.Merge,
                    ItemStatus = disk.Status?.AsItemStatus() ?? ItemStatus.NULL,
                    //Flags = disk.Flags, // TODO: Add to internal model

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

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
                    Name = medium.Name,
                    MD5 = medium.MD5,
                    SHA1 = medium.SHA1,
                    SHA256 = medium.SHA256,
                    SpamSum = medium.SpamSum,

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

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
                    Name = archive.Name,

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

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
                    ChipType = chip.Type?.AsChipType() ?? ChipType.NULL,
                    Name = chip.Name,
                    //Flags = chip.Flags, // TODO: Add to internal model
                    Clock = NumberHelper.ConvertToInt64(chip.Clock),

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

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
                    DisplayType = video.Screen?.AsDisplayType() ?? DisplayType.NULL,
                    Width = NumberHelper.ConvertToInt64(video.X),
                    Height = NumberHelper.ConvertToInt64(video.Y),
                    //AspectX = video.AspectX, // TODO: Add to internal model or find mapping
                    //AspectY = video.AspectY, // TODO: Add to internal model or find mapping
                    Refresh = NumberHelper.ConvertToDouble(video.Freq),

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

                switch (video.Orientation)
                {
                    case "horizontal":
                        item.Rotate = 0;
                        break;
                    case "vertical":
                        item.Rotate = 90;
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
                Channels = NumberHelper.ConvertToInt64(sound.Channels),

                Source = new Source
                {
                    Index = indexId,
                    Name = filename,
                },
            };

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
                Players = NumberHelper.ConvertToInt64(input.Players),
                //Control = input.Control, // TODO: Add to internal model or find mapping
                Controls = new List<Control>
                {
                    new Control
                    {
                        Buttons = NumberHelper.ConvertToInt64(input.Buttons),
                    },
                },
                Coins = NumberHelper.ConvertToInt64(input.Coins),
                Tilt = input.Tilt?.AsYesNo(),
                Service = input.Service?.AsYesNo(),

                Source = new Source
                {
                    Index = indexId,
                    Name = filename,
                },
            };

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
                    Name = dipswitch.Name,
                    Values = new List<DipValue>(),

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

                foreach (string entry in dipswitch.Entry ?? Array.Empty<string>())
                {
                    var dipValue = new DipValue
                    {
                        Name = dipswitch.Name,
                        Value = entry,
                        Default = entry == dipswitch.Default,
                    };
                    item.Values.Add(dipValue);
                }

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
                Status = driver.Status?.AsSupportStatus() ?? SupportStatus.NULL,
                //Color = driver.Color, // TODO: Add to internal model or find mapping
                //Sound = driver.Sound, // TODO: Add to internal model or find mapping
                //PaletteSize = driver.PaletteSize, // TODO: Add to internal model or find mapping
                //Blit = driver.Blit, // TODO: Add to internal model or find mapping

                Source = new Source
                {
                    Index = indexId,
                    Name = filename,
                },
            };

            item.CopyMachineInformation(machine);
            ParseAddHelper(item, statsOnly);
        }

        #endregion
    }
}
