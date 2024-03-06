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
    /// Represents parsing and writing of a Logiqx-derived DAT
    /// </summary>
    internal partial class Logiqx : DatFile
    {
        /// <inheritdoc/>
        protected override ItemType[] GetSupportedTypes()
        {
            return
            [
                ItemType.Archive,
                ItemType.BiosSet,
                ItemType.Disk,
                ItemType.Media,
                ItemType.Release,
                ItemType.Rom,
                ItemType.Sample,
            ];
        }

        /// <inheritdoc/>
        protected override List<string>? GetMissingRequiredFields(DatItem datItem)
        {
            var missingFields = new List<string>();
            switch (datItem)
            {
                case Release release:
                    if (string.IsNullOrEmpty(release.Name))
                        missingFields.Add(Models.Metadata.Release.NameKey);
                    if (string.IsNullOrEmpty(release.Region))
                        missingFields.Add(Models.Metadata.Release.RegionKey);
                    break;

                case BiosSet biosset:
                    if (string.IsNullOrEmpty(biosset.Name))
                        missingFields.Add(Models.Metadata.BiosSet.NameKey);
                    if (string.IsNullOrEmpty(biosset.Description))
                        missingFields.Add(Models.Metadata.BiosSet.DescriptionKey);
                    break;

                case Rom rom:
                    if (string.IsNullOrEmpty(rom.Name))
                        missingFields.Add(Models.Metadata.Rom.NameKey);
                    if (rom.Size == null || rom.Size < 0)
                        missingFields.Add(Models.Metadata.Rom.SizeKey);
                    if (string.IsNullOrEmpty(rom.CRC)
                        && string.IsNullOrEmpty(rom.MD5)
                        && string.IsNullOrEmpty(rom.SHA1)
                        && string.IsNullOrEmpty(rom.SHA256)
                        && string.IsNullOrEmpty(rom.SHA384)
                        && string.IsNullOrEmpty(rom.SHA512)
                        && string.IsNullOrEmpty(rom.SpamSum))
                    {
                        missingFields.Add(Models.Metadata.Rom.SHA1Key);
                    }
                    break;

                case Disk disk:
                    if (string.IsNullOrEmpty(disk.Name))
                        missingFields.Add(Models.Metadata.Disk.NameKey);
                    if (string.IsNullOrEmpty(disk.MD5)
                        && string.IsNullOrEmpty(disk.SHA1))
                    {
                        missingFields.Add(Models.Metadata.Disk.SHA1Key);
                    }
                    break;

                case Media media:
                    if (string.IsNullOrEmpty(media.Name))
                        missingFields.Add(Models.Metadata.Media.NameKey);
                    if (string.IsNullOrEmpty(media.MD5)
                        && string.IsNullOrEmpty(media.SHA1)
                        && string.IsNullOrEmpty(media.SHA256)
                        && string.IsNullOrEmpty(media.SpamSum))
                    {
                        missingFields.Add(Models.Metadata.Media.SHA1Key);
                    }
                    break;

                case DeviceReference deviceref:
                    if (string.IsNullOrEmpty(deviceref.Name))
                        missingFields.Add(Models.Metadata.DeviceRef.NameKey);
                    break;

                case Sample sample:
                    if (string.IsNullOrEmpty(sample.Name))
                        missingFields.Add(Models.Metadata.Sample.NameKey);
                    break;

                case Archive archive:
                    if (string.IsNullOrEmpty(archive.Name))
                        missingFields.Add(Models.Metadata.Archive.NameKey);
                    break;

                case Driver driver:
                    if (!driver.StatusSpecified)
                        missingFields.Add(Models.Metadata.Driver.StatusKey);
                    if (!driver.EmulationSpecified)
                        missingFields.Add(Models.Metadata.Driver.EmulationKey);
                    if (!driver.CocktailSpecified)
                        missingFields.Add(Models.Metadata.Driver.CocktailKey);
                    if (!driver.SaveStateSpecified)
                        missingFields.Add(Models.Metadata.Driver.SaveStateKey);
                    break;

                case DatItems.Formats.SoftwareList softwarelist:
                    if (string.IsNullOrEmpty(softwarelist.Tag))
                        missingFields.Add(Models.Metadata.SoftwareList.TagKey);
                    if (string.IsNullOrEmpty(softwarelist.Name))
                        missingFields.Add(Models.Metadata.SoftwareList.NameKey);
                    if (!softwarelist.StatusSpecified)
                        missingFields.Add(Models.Metadata.SoftwareList.StatusKey);
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

                var datafile = CreateDatafile(ignoreblanks);

                // Only write the doctype if we don't have No-Intro data
                bool success;
                if (string.IsNullOrEmpty(Header.NoIntroID))
                    success = new Serialization.Files.Logiqx().SerializeToFileWithDocType(datafile, outfile);
                else
                    success = new Serialization.Files.Logiqx().Serialize(datafile, outfile);

                if (!success)
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
        /// Create a Datafile from the current internal information
        /// <summary>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise</param>
        private Models.Logiqx.Datafile CreateDatafile(bool ignoreblanks)
        {
            var datafile = new Models.Logiqx.Datafile
            {
                Build = Header.Build,
                Debug = Header.Debug.FromYesNo(),

                Header = CreateHeader(),
                Game = CreateGames(ignoreblanks)
            };

            if (!string.IsNullOrEmpty(Header.NoIntroID))
                datafile.SchemaLocation = "https://datomatic.no-intro.org/stuff https://datomatic.no-intro.org/stuff/schema_nointro_datfile_v3.xsd";

            return datafile;
        }

        /// <summary>
        /// Create a Header from the current internal information
        /// <summary>
        private Models.Logiqx.Header? CreateHeader()
        {
            // If we don't have a header, we can't do anything
            if (this.Header == null)
                return null;

            var header = new Models.Logiqx.Header
            {
                Id = Header.NoIntroID,
                Name = Header.Name,
                Description = Header.Description,
                RootDir = Header.RootDir,
                Category = Header.Category,
                Version = Header.Version,
                Date = Header.Date,
                Author = Header.Author,
                Email = Header.Email,
                Homepage = Header.Homepage,
                Url = Header.Url,
                Comment = Header.Comment,
                Type = Header.Type,

                ClrMamePro = CreateClrMamePro(),
                RomCenter = CreateRomCenter(),
            };

            return header;
        }

        /// <summary>
        /// Create a ClrMamePro from the current internal information
        /// <summary>
        private Models.Logiqx.ClrMamePro? CreateClrMamePro()
        {
            // If we don't have subheader values, we can't do anything
            if (!Header.ForceMergingSpecified
                && !Header.ForceNodumpSpecified
                && !Header.ForcePackingSpecified
                && string.IsNullOrEmpty(Header.HeaderSkipper))
            {
                return null;
            }

            var subheader = new Models.Logiqx.ClrMamePro
            {
                Header = Header.HeaderSkipper,
            };

            if (Header.ForceMergingSpecified)
                subheader.ForceMerging = Header.ForceMerging.AsStringValue<MergingFlag>(useSecond: false);
            if (Header.ForceNodumpSpecified)
                subheader.ForceNodump = Header.ForceNodump.AsStringValue<NodumpFlag>();
            if (Header.ForcePackingSpecified)
                subheader.ForcePacking = Header.ForcePacking.AsStringValue<PackingFlag>(useSecond: false);

            return subheader;
        }

        /// <summary>
        /// Create a RomCenter from the current internal information
        /// <summary>
        private Models.Logiqx.RomCenter? CreateRomCenter()
        {
            // If we don't have subheader values, we can't do anything
            if (string.IsNullOrEmpty(Header.System)
                && !Header.RomModeSpecified
                && !Header.BiosModeSpecified
                && !Header.SampleModeSpecified
                && !Header.LockRomModeSpecified
                && !Header.LockBiosModeSpecified
                && !Header.LockSampleModeSpecified)
            {
                return null;
            }

            var subheader = new Models.Logiqx.RomCenter
            {
                Plugin = Header.System,
            };

            if (Header.RomModeSpecified)
                subheader.RomMode = Header.RomMode.AsStringValue<MergingFlag>(useSecond: true);
            if (Header.BiosModeSpecified)
                subheader.BiosMode = Header.BiosMode.AsStringValue<MergingFlag>(useSecond: true);
            if (Header.SampleModeSpecified)
                subheader.SampleMode = Header.SampleMode.AsStringValue<MergingFlag>(useSecond: true);

            if (Header.LockRomModeSpecified)
                subheader.LockRomMode = Header.LockRomMode.FromYesNo();
            if (Header.LockBiosModeSpecified)
                subheader.LockBiosMode = Header.LockBiosMode.FromYesNo();
            if (Header.LockSampleModeSpecified)
                subheader.LockSampleMode = Header.LockSampleMode.FromYesNo();

            return subheader;
        }

        /// <summary>
        /// Create an array of GameBase from the current internal information
        /// <summary>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise</param>
        private Models.Logiqx.GameBase[]? CreateGames(bool ignoreblanks)
        {
            // If we don't have items, we can't do anything
            if (this.Items == null || !this.Items.Any())
                return null;

            // Create a list of hold the games
            var games = new List<Models.Logiqx.GameBase>();

            // Loop through the sorted items and create games for them
            foreach (string key in Items.SortedKeys)
            {
                var items = Items.FilteredItems(key);
                if (items == null || !items.Any())
                    continue;

                // Get the first item for game information
                var machine = items[0].Machine;
                var game = CreateGame(machine!);

                // Create holders for all item types
                var releases = new List<Models.Logiqx.Release>();
                var biossets = new List<Models.Logiqx.BiosSet>();
                var roms = new List<Models.Logiqx.Rom>();
                var disks = new List<Models.Logiqx.Disk>();
                var medias = new List<Models.Logiqx.Media>();
                var samples = new List<Models.Logiqx.Sample>();
                var archives = new List<Models.Logiqx.Archive>();
                var devicerefs = new List<Models.Logiqx.DeviceRef>();
                var softwarelists = new List<Models.Logiqx.SoftwareList>();

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
                        case DeviceReference deviceref:
                            devicerefs.Add(CreateDeviceRef(deviceref));
                            break;
                        case Driver driver:
                            game.Driver = CreateDriver(driver);
                            break;
                        case DatItems.Formats.SoftwareList softwarelist:
                            softwarelists.Add(CreateSoftwareList(softwarelist));
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
                game.DeviceRef = [.. devicerefs];
                game.SoftwareList = [.. softwarelists];

                // Add the game to the list
                games.Add(game);
            }

            return [.. games];
        }

        /// <summary>
        /// Create a GameBase from the current internal information
        /// <summary>
        private Models.Logiqx.GameBase CreateGame(Machine machine)
        {
            Models.Logiqx.GameBase game = _deprecated ? new Models.Logiqx.Game() : new Models.Logiqx.Machine();

            game.Name = machine.Name;
            game.SourceFile = machine.SourceFile;
#if NETFRAMEWORK
            if ((machine.MachineType & MachineType.Bios) != 0)
                game.IsBios = "yes";
            if ((machine.MachineType & MachineType.Device) != 0)
                game.IsDevice = "yes";
            if ((machine.MachineType & MachineType.Mechanical) != 0)
                game.IsMechanical = "yes";
#else
            if (machine.MachineType.HasFlag(MachineType.Bios))
                game.IsBios = "yes";
            if (machine.MachineType.HasFlag(MachineType.Device))
                game.IsDevice = "yes";
            if (machine.MachineType.HasFlag(MachineType.Mechanical))
                game.IsMechanical = "yes";
#endif
            game.CloneOf = machine.CloneOf;
            game.RomOf = machine.RomOf;
            game.SampleOf = machine.SampleOf;
            game.Board = machine.Board;
            game.RebuildTo = machine.RebuildTo;
            game.Id = machine.NoIntroId;
            game.CloneOfId = machine.NoIntroCloneOfId;
            game.Runnable = machine.Runnable.AsStringValue<Runnable>();
            if (machine.Comment != null)
            {
                if (machine.Comment.Contains(';'))
                    game.Comment = machine.Comment.Split(';');
                else
                    game.Comment = [machine.Comment];
            }
            game.Description = machine.Description;
            game.Year = machine.Year;
            game.Manufacturer = machine.Manufacturer;
            game.Publisher = machine.Publisher;
            if (machine.Category != null)
            {
                if (machine.Category.Contains(';'))
                    game.Category = machine.Category.Split(';');
                else
                    game.Category = [machine.Category];
            }
            game.Trurip = CreateTrurip(machine);

            return game;
        }

        /// <summary>
        /// Create a Trurip from the current internal information
        /// <summary>
        private static Models.Logiqx.Trurip? CreateTrurip(Machine machine)
        {
            // If we don't have subheader values, we can't do anything
            if (string.IsNullOrEmpty(machine.TitleID)
                && string.IsNullOrEmpty(machine.Developer)
                && string.IsNullOrEmpty(machine.Genre)
                && string.IsNullOrEmpty(machine.Subgenre)
                && string.IsNullOrEmpty(machine.Ratings)
                && string.IsNullOrEmpty(machine.Score)
                && string.IsNullOrEmpty(machine.Enabled)
                && !machine.CrcSpecified
                && string.IsNullOrEmpty(machine.RelatedTo))
            {
                return null;
            }

            var trurip = new Models.Logiqx.Trurip
            {
                TitleID = machine.TitleID,
                Publisher = machine.Publisher,
                Developer = machine.Developer,
                Year = machine.Year,
                Genre = machine.Genre,
                Subgenre = machine.Subgenre,
                Ratings = machine.Ratings,
                Score = machine.Score,
                Players = machine.Players,
                Enabled = machine.Enabled,
                CRC = machine.Crc.FromYesNo(),
                Source = machine.SourceFile,
                CloneOf = machine.CloneOf,
                RelatedTo = machine.RelatedTo,
            };

            return trurip;
        }

        /// <summary>
        /// Create a Release from the current Release DatItem
        /// <summary>
        private static Models.Logiqx.Release CreateRelease(Release item)
        {
            var release = new Models.Logiqx.Release
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
        private static Models.Logiqx.BiosSet CreateBiosSet(BiosSet item)
        {
            var biosset = new Models.Logiqx.BiosSet
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
        private static Models.Logiqx.Rom CreateRom(Rom item)
        {
            var rom = new Models.Logiqx.Rom
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
                //Serial = item.Serial, // TODO: Add to internal model
                //Header = item.Header, // TODO: Add to internal model
                Date = item.Date,
            };

            if (item.ItemStatusSpecified)
                rom.Status = item.ItemStatus.AsStringValue<ItemStatus>(useSecond: false);
            if (item.InvertedSpecified)
                rom.Inverted = item.Inverted.FromYesNo();
            if (item.MIASpecified)
                rom.MIA = item.MIA.FromYesNo();

            return rom;
        }

        /// <summary>
        /// Create a Disk from the current Disk DatItem
        /// <summary>
        private static Models.Logiqx.Disk CreateDisk(Disk item)
        {
            var disk = new Models.Logiqx.Disk
            {
                Name = item.Name,
                MD5 = item.MD5,
                SHA1 = item.SHA1,
                Merge = item.MergeTag,
                Region = item.Region,
            };

            if (item.ItemStatusSpecified)
                disk.Status = item.ItemStatus.AsStringValue<ItemStatus>(useSecond: false);

            return disk;
        }

        /// <summary>
        /// Create a Media from the current Media DatItem
        /// <summary>
        private static Models.Logiqx.Media CreateMedia(Media item)
        {
            var media = new Models.Logiqx.Media
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
        private static Models.Logiqx.Sample CreateSample(Sample item)
        {
            var sample = new Models.Logiqx.Sample
            {
                Name = item.Name,
            };
            return sample;
        }

        /// <summary>
        /// Create a Archive from the current Archive DatItem
        /// <summary>
        private static Models.Logiqx.Archive CreateArchive(Archive item)
        {
            var archive = new Models.Logiqx.Archive
            {
                Name = item.Name,
            };
            return archive;
        }

        /// <summary>
        /// Create a DeviceRef from the current Chip DatItem
        /// <summary>
        private static Models.Logiqx.DeviceRef CreateDeviceRef(DeviceReference item)
        {
            var deviceref = new Models.Logiqx.DeviceRef
            {
                Name = item.Name,
            };
            return deviceref;
        }

        /// <summary>
        /// Create a Driver from the current Driver DatItem
        /// <summary>
        private static Models.Logiqx.Driver CreateDriver(Driver item)
        {
            var driver = new Models.Logiqx.Driver
            {
                Status = item.Status.AsStringValue<SupportStatus>(),
                Emulation = item.Emulation.AsStringValue<SupportStatus>(),
                Cocktail = item.Cocktail.AsStringValue<SupportStatus>(),
                SaveState = item.SaveState.AsStringValue<Supported>(useSecond: true),
            };

            if (item.RequiresArtworkSpecified)
                driver.RequiresArtwork = item.RequiresArtwork.FromYesNo();
            if (item.UnofficialSpecified)
                driver.Unofficial = item.Unofficial.FromYesNo();
            if (item.NoSoundHardwareSpecified)
                driver.NoSoundHardware = item.NoSoundHardware.FromYesNo();
            if (item.IncompleteSpecified)
                driver.Incomplete = item.Incomplete.FromYesNo();

            return driver;
        }

        /// <summary>
        /// Create a SoftwareList from the current SoftwareList DatItem
        /// <summary>
        private static Models.Logiqx.SoftwareList CreateSoftwareList(DatItems.Formats.SoftwareList item)
        {
            var softwarelist = new Models.Logiqx.SoftwareList
            {
                Tag = item.Tag,
                Name = item.Name,
                Filter = item.Filter,
            };

            if (item.StatusSpecified)
                softwarelist.Status = item.Status.AsStringValue<SoftwareListStatus>();

            return softwarelist;
        }

        #endregion
    }
}
