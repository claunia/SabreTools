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
            return new ItemType[]
            {
                ItemType.Archive,
                ItemType.BiosSet,
                ItemType.Disk,
                ItemType.Media,
                ItemType.Release,
                ItemType.Rom,
                ItemType.Sample,
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

                case Media media:
                    if (string.IsNullOrWhiteSpace(media.Name))
                        missingFields.Add(DatItemField.Name);
                    if (string.IsNullOrWhiteSpace(media.MD5)
                        && string.IsNullOrWhiteSpace(media.SHA1)
                        && string.IsNullOrWhiteSpace(media.SHA256)
                        && string.IsNullOrWhiteSpace(media.SpamSum))
                    {
                        missingFields.Add(DatItemField.SHA1);
                    }
                    break;

                case DeviceReference deviceref:
                    if (string.IsNullOrWhiteSpace(deviceref.Name))
                        missingFields.Add(DatItemField.Name);
                    break;

                case Sample sample:
                    if (string.IsNullOrWhiteSpace(sample.Name))
                        missingFields.Add(DatItemField.Name);
                    break;

                case Archive archive:
                    if (string.IsNullOrWhiteSpace(archive.Name))
                        missingFields.Add(DatItemField.Name);
                    break;

                case Driver driver:
                    if (!driver.StatusSpecified)
                        missingFields.Add(DatItemField.SupportStatus);
                    if (!driver.EmulationSpecified)
                        missingFields.Add(DatItemField.EmulationStatus);
                    if (!driver.CocktailSpecified)
                        missingFields.Add(DatItemField.CocktailStatus);
                    if (!driver.SaveStateSpecified)
                        missingFields.Add(DatItemField.SaveStateStatus);
                    break;

                case DatItems.Formats.SoftwareList softwarelist:
                    if (string.IsNullOrWhiteSpace(softwarelist.Tag))
                        missingFields.Add(DatItemField.Tag);
                    if (string.IsNullOrWhiteSpace(softwarelist.Name))
                        missingFields.Add(DatItemField.Name);
                    if (!softwarelist.StatusSpecified)
                        missingFields.Add(DatItemField.SoftwareListStatus);
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
                if (string.IsNullOrWhiteSpace(Header.NoIntroID))
                    success = Serialization.Logiqx.SerializeToFileWithDocType(datafile, outfile);
                else
                    success = Serialization.Logiqx.SerializeToFile(datafile, outfile);

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
                && string.IsNullOrWhiteSpace(Header.HeaderSkipper))
            {
                return null;
            }

            var subheader = new Models.Logiqx.ClrMamePro
            {
                Header = Header.HeaderSkipper,
            };

            if (Header.ForceMergingSpecified)
                subheader.ForceMerging = Header.ForceMerging.FromMergingFlag(romCenter: false);
            if (Header.ForceNodumpSpecified)
                subheader.ForceNodump = Header.ForceNodump.FromNodumpFlag();
            if (Header.ForcePackingSpecified)
                subheader.ForcePacking = Header.ForcePacking.FromPackingFlag(yesno: false);

            return subheader;
        }

        /// <summary>
        /// Create a RomCenter from the current internal information
        /// <summary>
        private Models.Logiqx.RomCenter? CreateRomCenter()
        {
            // If we don't have subheader values, we can't do anything
            if (string.IsNullOrWhiteSpace(Header.System)
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
                subheader.RomMode = Header.RomMode.FromMergingFlag(romCenter: true);
            if (Header.BiosModeSpecified)
                subheader.BiosMode = Header.BiosMode.FromMergingFlag(romCenter: true);
            if (Header.SampleModeSpecified)
                subheader.SampleMode = Header.SampleMode.FromMergingFlag(romCenter: true);

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
                var game = CreateGame(machine);

                // Create holders for all item types
                var releases = new List<Models.Logiqx.Release>();
                var biossets = new List<Models.Logiqx.BiosSet>();
                var roms = new List<Models.Logiqx.Rom>();
                var disks = new List<Models.Logiqx.Disk>();
                var medias = new List<Models.Logiqx.Media>();
                var samples = new List<Models.Logiqx.Sample>();
                var archives = new List<Models.Logiqx.Archive>();
                var devicerefs = new List<Models.Logiqx.DeviceRef>();
                var drivers = new List<Models.Logiqx.Driver>();
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
                            drivers.Add(CreateDriver(driver));
                            break;
                        case DatItems.Formats.SoftwareList softwarelist:
                            softwarelists.Add(CreateSoftwareList(softwarelist));
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
                game.DeviceRef = devicerefs.ToArray();
                game.Driver = drivers.ToArray();
                game.SoftwareList = softwarelists.ToArray();

                // Add the game to the list
                games.Add(game);
            }

            return games.ToArray();
        }

        /// <summary>
        /// Create a GameBase from the current internal information
        /// <summary>
        private Models.Logiqx.GameBase? CreateGame(Machine machine)
        {
            Models.Logiqx.GameBase game = _deprecated ? new Models.Logiqx.Game() : new Models.Logiqx.Machine();

            game.Name = machine.Name;
            game.SourceFile = machine.SourceFile;
            if (machine.MachineType.HasFlag(MachineType.Bios))
                game.IsBios = "yes";
            if (machine.MachineType.HasFlag(MachineType.Device))
                game.IsDevice = "yes";
            if (machine.MachineType.HasFlag(MachineType.Mechanical))
                game.IsMechanical = "yes";
            game.CloneOf = machine.CloneOf;
            game.RomOf = machine.RomOf;
            game.SampleOf = machine.SampleOf;
            game.Board = machine.Board;
            game.RebuildTo = machine.RebuildTo;
            game.Id = machine.NoIntroId;
            game.CloneOfId = machine.NoIntroCloneOfId;
            game.Runnable = machine.Runnable.FromRunnable();
            if (machine.Comment != null)
            {
                if (machine.Comment.Contains(';'))
                    game.Comment = machine.Comment.Split(';');
                else
                    game.Comment = new[] { machine.Comment };
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
                    game.Category = new[] { machine.Category };
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
            if (string.IsNullOrWhiteSpace(machine.TitleID)
                && string.IsNullOrWhiteSpace(machine.Developer)
                && string.IsNullOrWhiteSpace(machine.Genre)
                && string.IsNullOrWhiteSpace(machine.Subgenre)
                && string.IsNullOrWhiteSpace(machine.Ratings)
                && string.IsNullOrWhiteSpace(machine.Score)
                && string.IsNullOrWhiteSpace(machine.Enabled)
                && !machine.CrcSpecified
                && string.IsNullOrWhiteSpace(machine.RelatedTo))
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
                disk.Status = item.ItemStatus.FromItemStatus(yesno: false);

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
                Status = item.Status.FromSupportStatus(),
                Emulation = item.Emulation.FromSupportStatus(),
                Cocktail = item.Cocktail.FromSupportStatus(),
                SaveState = item.SaveState.FromSupported(true),
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
                softwarelist.Status = item.Status.FromSoftwareListStatus();

            return softwarelist;
        }

        #endregion
    }
}
