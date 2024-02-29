using System;
using System.Linq;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing a Logiqx-derived DAT
    /// </summary>
    internal partial class Logiqx : DatFile
    {
        /// <inheritdoc/>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            try
            {
                // Deserialize the input file
                var metadataFile = new Serialization.Files.Logiqx().Deserialize(filename);

                // Convert the header to the internal format
                ConvertHeader(metadataFile, keep);

                // Convert the game data to the internal format
                ConvertGames(metadataFile?.Game, filename, indexId, statsOnly);

                // Convert the dir data to the internal format
                ConvertDirs(metadataFile?.Dir, filename, indexId, statsOnly);
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
        /// <param name="datafile">Deserialized model to convert</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        private void ConvertHeader(Models.Logiqx.Datafile? datafile, bool keep)
        {
            // If the datafile is missing, we can't do anything
            if (datafile == null)
                return;

            Header.Build ??= datafile.Build;
            Header.Debug ??= datafile.Debug.AsYesNo();
            // SchemaLocation is specifically skipped

            ConvertHeader(datafile.Header, keep);
        }

        /// <summary>
        /// Convert header information
        /// </summary>
        /// <param name="header">Deserialized model to convert</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        private void ConvertHeader(Models.Logiqx.Header? header, bool keep)
        {
            // If the header is missing, we can't do anything
            if (header == null)
                return;

            Header.NoIntroID ??= header.Id;
            Header.Name ??= header.Name;
            Header.Description ??= header.Description;
            Header.RootDir ??= header.RootDir;
            Header.Category ??= header.Category;
            Header.Version ??= header.Version;
            Header.Date ??= header.Date;
            Header.Author ??= header.Author;
            Header.Email ??= header.Email;
            Header.Homepage ??= header.Homepage;
            Header.Url ??= header.Url;
            Header.Comment ??= header.Comment;
            Header.Type ??= header.Type;

            ConvertSubheader(header.ClrMamePro);
            ConvertSubheader(header.RomCenter);

            // Handle implied SuperDAT
            if (header.Name?.Contains(" - SuperDAT") == true && keep)
                Header.Type ??= "SuperDAT";
        }

        /// <summary>
        /// Convert subheader information
        /// </summary>
        /// <param name="clrMamePro">Deserialized model to convert</param>
        private void ConvertSubheader(Models.Logiqx.ClrMamePro? clrMamePro)
        {
            // If the subheader is missing, we can't do anything
            if (clrMamePro == null)
                return;

            Header.HeaderSkipper ??= clrMamePro.Header;

            if (Header.ForceMerging == MergingFlag.None)
                Header.ForceMerging = clrMamePro.ForceMerging.AsMergingFlag();
            if (Header.ForceNodump == NodumpFlag.None)
                Header.ForceNodump = clrMamePro.ForceNodump.AsNodumpFlag();
            if (Header.ForcePacking == PackingFlag.None)
                Header.ForcePacking = clrMamePro.ForcePacking.AsPackingFlag();
        }

        /// <summary>
        /// Convert subheader information
        /// </summary>
        /// <param name="romCenter">Deserialized model to convert</param>
        private void ConvertSubheader(Models.Logiqx.RomCenter? romCenter)
        {
            // If the subheader is missing, we can't do anything
            if (romCenter == null)
                return;

            Header.System ??= romCenter.Plugin;

            if (Header.RomMode == MergingFlag.None)
                Header.RomMode = romCenter.RomMode.AsMergingFlag();
            if (Header.BiosMode == MergingFlag.None)
                Header.BiosMode = romCenter.BiosMode.AsMergingFlag();
            if (Header.SampleMode == MergingFlag.None)
                Header.SampleMode = romCenter.SampleMode.AsMergingFlag();

            Header.LockRomMode ??= romCenter.LockRomMode.AsYesNo();
            Header.LockBiosMode ??= romCenter.LockBiosMode.AsYesNo();
            Header.LockSampleMode ??= romCenter.LockSampleMode.AsYesNo();
        }

        /// <summary>
        /// Convert dirs information
        /// </summary>
        /// <param name="dirs">Array of deserialized models to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertDirs(Models.Logiqx.Dir[]? dirs, string filename, int indexId, bool statsOnly)
        {
            // If the dir array is missing, we can't do anything
            if (dirs == null || !dirs.Any())
                return;

            // Loop through the dirs and add
            foreach (var dir in dirs)
            {
                ConvertDir(dir, filename, indexId, statsOnly);
            }
        }

        /// <summary>
        /// Convert dir information
        /// </summary>
        /// <param name="dir">Deserialized model to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertDir(Models.Logiqx.Dir dir, string filename, int indexId, bool statsOnly)
        {
            // If the game array is missing, we can't do anything
            if (dir.Game == null || !dir.Game.Any())
                return;

            // Loop through the games and add
            foreach (var game in dir.Game)
            {
                ConvertGame(game, filename, indexId, statsOnly, dir.Name);
            }
        }

        /// <summary>
        /// Convert games information
        /// </summary>
        /// <param name="games">Array of deserialized models to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertGames(Models.Logiqx.GameBase[]? games, string filename, int indexId, bool statsOnly)
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
        private void ConvertGame(Models.Logiqx.GameBase game, string filename, int indexId, bool statsOnly, string? dirname = null)
        {
            // If the game is missing, we can't do anything
            if (game == null)
                return;

            // Create the machine for copying information
            var machine = new Machine
            {
                Name = game.Name,
                SourceFile = game.SourceFile,
                CloneOf = game.CloneOf,
                RomOf = game.RomOf,
                SampleOf = game.SampleOf,
                Board = game.Board,
                RebuildTo = game.RebuildTo,
                NoIntroId = game.Id,
                NoIntroCloneOfId = game.CloneOfId,
                Runnable = game.Runnable.AsRunnable(),

                Description = game.Description,
                Year = game.Year,
                Manufacturer = game.Manufacturer,
                Publisher = game.Publisher,
            };

            if (!string.IsNullOrEmpty(dirname))
                machine.Name = $"{dirname}/{machine.Name}";

            if (game.IsBios.AsYesNo() == true)
                machine.MachineType |= MachineType.Bios;
            if (game.IsDevice.AsYesNo() == true)
                machine.MachineType |= MachineType.Device;
            if (game.IsMechanical.AsYesNo() == true)
                machine.MachineType |= MachineType.Mechanical;

#if NETFRAMEWORK
            if (game.Comment != null && game.Comment.Any())
                machine.Comment = string.Join(";", game.Comment);
            if (game.Category != null && game.Category.Any())
                machine.Category = string.Join(";", game.Category);
#else
            if (game.Comment != null && game.Comment.Any())
                machine.Comment = string.Join(';', game.Comment);
            if (game.Category != null && game.Category.Any())
                machine.Category = string.Join(';', game.Category);
#endif

            if (game.Trurip != null)
            {
                var trurip = game.Trurip;

                machine.TitleID = trurip.TitleID;
                machine.Publisher = trurip.Publisher;
                machine.Developer = trurip.Developer;
                machine.Year = trurip.Year;
                machine.Genre = trurip.Genre;
                machine.Subgenre = trurip.Subgenre;
                machine.Ratings = trurip.Ratings;
                machine.Score = trurip.Score;
                machine.Players = trurip.Players;
                machine.Enabled = trurip.Enabled;
                machine.Crc = trurip.CRC.AsYesNo();
                machine.SourceFile = trurip.Source;
                machine.CloneOf = trurip.CloneOf;
                machine.RelatedTo = trurip.RelatedTo;
            }

            // Check if there are any items
            bool containsItems = false;

            // Loop through each type of item
            ConvertReleases(game.Release, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertBiosSets(game.BiosSet, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertRoms(game.Rom, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertDisks(game.Disk, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertMedia(game.Media, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertDeviceRefs(game.DeviceRef, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertSamples(game.Sample, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertArchives(game.Archive, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertDriver(game.Driver, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertSoftwareLists(game.SoftwareList, machine, filename, indexId, statsOnly, ref containsItems);

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
        private void ConvertReleases(Models.Logiqx.Release[]? releases, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
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
        private void ConvertBiosSets(Models.Logiqx.BiosSet[]? biossets, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
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
        private void ConvertRoms(Models.Logiqx.Rom[]? roms, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
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
        private void ConvertDisks(Models.Logiqx.Disk[]? disks, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
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
        private void ConvertMedia(Models.Logiqx.Media[]? media, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
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
        /// Convert DeviceRef information
        /// </summary>
        /// <param name="devicerefs">Array of deserialized models to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertDeviceRefs(Models.Logiqx.DeviceRef[]? devicerefs, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the devicerefs array is missing, we can't do anything
            if (devicerefs == null || !devicerefs.Any())
                return;

            containsItems = true;
            foreach (var deviceref in devicerefs)
            {
                var item = new DeviceReference
                {
                    Name = deviceref.Name,

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
        /// Convert DeviceRef information
        /// </summary>
        /// <param name="samples">Array of deserialized models to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertSamples(Models.Logiqx.Sample[]? samples, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the samples array is missing, we can't do anything
            if (samples == null || !samples.Any())
                return;

            containsItems = true;
            foreach (var sample in samples)
            {
                var item = new Sample
                {
                    Name = sample.Name,

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
        private void ConvertArchives(Models.Logiqx.Archive[]? archives, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
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
        /// Convert Driver information
        /// </summary>
        /// <param name="driver">Deserialized model to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertDriver(Models.Logiqx.Driver? driver, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the driver is missing, we can't do anything
            if (driver == null)
                return;

            containsItems = true;
            var item = new Driver
            {
                Status = driver.Status?.AsSupportStatus() ?? SupportStatus.NULL,
                Emulation = driver.Emulation?.AsSupportStatus() ?? SupportStatus.NULL,
                Cocktail = driver.Cocktail?.AsSupportStatus() ?? SupportStatus.NULL,
                SaveState = driver.SaveState?.AsSupported() ?? Supported.NULL,
                RequiresArtwork = driver.RequiresArtwork?.AsYesNo(),
                Unofficial = driver.Unofficial?.AsYesNo(),
                NoSoundHardware = driver.NoSoundHardware?.AsYesNo(),
                Incomplete = driver.Incomplete?.AsYesNo(),

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
        /// Convert SoftwareList information
        /// </summary>
        /// <param name="softwarelists">Array of deserialized models to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertSoftwareLists(Models.Logiqx.SoftwareList[]? softwarelists, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the softwarelists array is missing, we can't do anything
            if (softwarelists == null || !softwarelists.Any())
                return;

            containsItems = true;
            foreach (var softwarelist in softwarelists)
            {
                var item = new DatItems.Formats.SoftwareList
                {
                    Tag = softwarelist.Tag,
                    Name = softwarelist.Name,
                    Status = softwarelist.Status?.AsSoftwareListStatus() ?? SoftwareListStatus.None,
                    Filter = softwarelist.Filter,

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

        #endregion
    }
}
