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
                    if (string.IsNullOrEmpty(release.GetName()))
                        missingFields.Add(Models.Metadata.Release.NameKey);
                    if (string.IsNullOrEmpty(release.GetFieldValue<string?>(Models.Metadata.Release.RegionKey)))
                        missingFields.Add(Models.Metadata.Release.RegionKey);
                    break;

                case BiosSet biosset:
                    if (string.IsNullOrEmpty(biosset.GetName()))
                        missingFields.Add(Models.Metadata.BiosSet.NameKey);
                    if (string.IsNullOrEmpty(biosset.GetFieldValue<string?>(Models.Metadata.BiosSet.DescriptionKey)))
                        missingFields.Add(Models.Metadata.BiosSet.DescriptionKey);
                    break;

                case Rom rom:
                    if (string.IsNullOrEmpty(rom.GetName()))
                        missingFields.Add(Models.Metadata.Rom.NameKey);
                    if (rom.GetFieldValue<long?>(Models.Metadata.Rom.SizeKey) == null || rom.GetFieldValue<long?>(Models.Metadata.Rom.SizeKey) < 0)
                        missingFields.Add(Models.Metadata.Rom.SizeKey);
                    if (string.IsNullOrEmpty(rom.GetFieldValue<string?>(Models.Metadata.Rom.CRCKey))
                        && string.IsNullOrEmpty(rom.GetFieldValue<string?>(Models.Metadata.Rom.MD5Key))
                        && string.IsNullOrEmpty(rom.GetFieldValue<string?>(Models.Metadata.Rom.SHA1Key))
                        && string.IsNullOrEmpty(rom.GetFieldValue<string?>(Models.Metadata.Rom.SHA256Key))
                        && string.IsNullOrEmpty(rom.GetFieldValue<string?>(Models.Metadata.Rom.SHA384Key))
                        && string.IsNullOrEmpty(rom.GetFieldValue<string?>(Models.Metadata.Rom.SHA512Key))
                        && string.IsNullOrEmpty(rom.GetFieldValue<string?>(Models.Metadata.Rom.SpamSumKey)))
                    {
                        missingFields.Add(Models.Metadata.Rom.SHA1Key);
                    }
                    break;

                case Disk disk:
                    if (string.IsNullOrEmpty(disk.GetName()))
                        missingFields.Add(Models.Metadata.Disk.NameKey);
                    if (string.IsNullOrEmpty(disk.GetFieldValue<string?>(Models.Metadata.Disk.MD5Key))
                        && string.IsNullOrEmpty(disk.GetFieldValue<string?>(Models.Metadata.Disk.SHA1Key)))
                    {
                        missingFields.Add(Models.Metadata.Disk.SHA1Key);
                    }
                    break;

                case Media media:
                    if (string.IsNullOrEmpty(media.GetName()))
                        missingFields.Add(Models.Metadata.Media.NameKey);
                    if (string.IsNullOrEmpty(media.GetFieldValue<string?>(Models.Metadata.Media.MD5Key))
                        && string.IsNullOrEmpty(media.GetFieldValue<string?>(Models.Metadata.Media.SHA1Key))
                        && string.IsNullOrEmpty(media.GetFieldValue<string?>(Models.Metadata.Media.SHA256Key))
                        && string.IsNullOrEmpty(media.GetFieldValue<string?>(Models.Metadata.Media.SpamSumKey)))
                    {
                        missingFields.Add(Models.Metadata.Media.SHA1Key);
                    }
                    break;

                case DeviceReference deviceref:
                    if (string.IsNullOrEmpty(deviceref.GetName()))
                        missingFields.Add(Models.Metadata.DeviceRef.NameKey);
                    break;

                case Sample sample:
                    if (string.IsNullOrEmpty(sample.GetName()))
                        missingFields.Add(Models.Metadata.Sample.NameKey);
                    break;

                case Archive archive:
                    if (string.IsNullOrEmpty(archive.GetName()))
                        missingFields.Add(Models.Metadata.Archive.NameKey);
                    break;

                case Driver driver:
                    if (driver.GetFieldValue<SupportStatus>(Models.Metadata.Driver.StatusKey) == SupportStatus.NULL)
                        missingFields.Add(Models.Metadata.Driver.StatusKey);
                    if (driver.GetFieldValue<SupportStatus>(Models.Metadata.Driver.EmulationKey) == SupportStatus.NULL)
                        missingFields.Add(Models.Metadata.Driver.EmulationKey);
                    if (driver.GetFieldValue<SupportStatus>(Models.Metadata.Driver.CocktailKey) == SupportStatus.NULL)
                        missingFields.Add(Models.Metadata.Driver.CocktailKey);
                    if (driver.GetFieldValue<SupportStatus>(Models.Metadata.Driver.SaveStateKey) == SupportStatus.NULL)
                        missingFields.Add(Models.Metadata.Driver.SaveStateKey);
                    break;

                case DatItems.Formats.SoftwareList softwarelist:
                    if (string.IsNullOrEmpty(softwarelist.GetFieldValue<string?>(Models.Metadata.SoftwareList.TagKey)))
                        missingFields.Add(Models.Metadata.SoftwareList.TagKey);
                    if (string.IsNullOrEmpty(softwarelist.GetName()))
                        missingFields.Add(Models.Metadata.SoftwareList.NameKey);
                    if (softwarelist.GetFieldValue<SoftwareListStatus?>(Models.Metadata.SoftwareList.StatusKey) == SoftwareListStatus.None)
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
                if (string.IsNullOrEmpty(Header.GetFieldValue<string?>(Models.Metadata.Header.IdKey)))
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
                Build = Header.GetFieldValue<string?>(Models.Metadata.Header.BuildKey),
                Debug = Header.GetFieldValue<bool?>(Models.Metadata.Header.DebugKey).FromYesNo(),

                Header = CreateHeader(),
                Game = CreateGames(ignoreblanks)
            };

            if (!string.IsNullOrEmpty(Header.GetFieldValue<string?>(Models.Metadata.Header.IdKey)))
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
                Id = Header.GetFieldValue<string?>(Models.Metadata.Header.IdKey),
                Name = Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey),
                Description = Header.GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey),
                RootDir = Header.GetFieldValue<string?>(Models.Metadata.Header.RootDirKey),
                Category = Header.GetFieldValue<string?>(Models.Metadata.Header.CategoryKey),
                Version = Header.GetFieldValue<string?>(Models.Metadata.Header.VersionKey),
                Date = Header.GetFieldValue<string?>(Models.Metadata.Header.DateKey),
                Author = Header.GetFieldValue<string?>(Models.Metadata.Header.AuthorKey),
                Email = Header.GetFieldValue<string?>(Models.Metadata.Header.EmailKey),
                Homepage = Header.GetFieldValue<string?>(Models.Metadata.Header.HomepageKey),
                Url = Header.GetFieldValue<string?>(Models.Metadata.Header.UrlKey),
                Comment = Header.GetFieldValue<string?>(Models.Metadata.Header.CommentKey),
                Type = Header.GetFieldValue<string?>(Models.Metadata.Header.TypeKey),

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
            if (Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.ForceMergingKey) == MergingFlag.None
                && Header.GetFieldValue<NodumpFlag>(Models.Metadata.Header.ForceNodumpKey) == NodumpFlag.None
                && Header.GetFieldValue<PackingFlag>(Models.Metadata.Header.ForcePackingKey) == PackingFlag.None
                && string.IsNullOrEmpty(Header.GetFieldValue<string?>(Models.Metadata.Header.HeaderKey)))
            {
                return null;
            }

            var subheader = new Models.Logiqx.ClrMamePro
            {
                Header = Header.GetFieldValue<string?>(Models.Metadata.Header.HeaderKey),
            };

            if (Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.ForceMergingKey) != MergingFlag.None)
                subheader.ForceMerging = Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.ForceMergingKey).AsStringValue<MergingFlag>(useSecond: false);
            if (Header.GetFieldValue<NodumpFlag>(Models.Metadata.Header.ForceNodumpKey) != NodumpFlag.None)
                subheader.ForceNodump = Header.GetFieldValue<NodumpFlag>(Models.Metadata.Header.ForceNodumpKey).AsStringValue<NodumpFlag>();
            if (Header.GetFieldValue<PackingFlag>(Models.Metadata.Header.ForcePackingKey) != PackingFlag.None)
                subheader.ForcePacking = Header.GetFieldValue<PackingFlag>(Models.Metadata.Header.ForcePackingKey).AsStringValue<PackingFlag>(useSecond: false);

            return subheader;
        }

        /// <summary>
        /// Create a RomCenter from the current internal information
        /// <summary>
        private Models.Logiqx.RomCenter? CreateRomCenter()
        {
            // If we don't have subheader values, we can't do anything
            if (string.IsNullOrEmpty(Header.GetFieldValue<string?>(Models.Metadata.Header.SystemKey))
                && Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.BiosModeKey) == MergingFlag.None
                && Header.GetFieldValue<bool?>(Models.Metadata.Header.LockBiosModeKey) == null
                && Header.GetFieldValue<bool?>(Models.Metadata.Header.LockRomModeKey) == null
                && Header.GetFieldValue<bool?>(Models.Metadata.Header.LockSampleModeKey) == null
                && Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.RomModeKey) == MergingFlag.None
                && Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.SampleModeKey) == MergingFlag.None)
            {
                return null;
            }

            var subheader = new Models.Logiqx.RomCenter
            {
                Plugin = Header.GetFieldValue<string?>(Models.Metadata.Header.PluginKey),
            };

            if (Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.RomModeKey) != MergingFlag.None)
                subheader.RomMode = Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.RomModeKey).AsStringValue<MergingFlag>(useSecond: true);
            if (Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.BiosModeKey) != MergingFlag.None)
                subheader.BiosMode = Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.BiosModeKey).AsStringValue<MergingFlag>(useSecond: true);
            if (Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.SampleModeKey) != MergingFlag.None)
                subheader.SampleMode = Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.SampleModeKey).AsStringValue<MergingFlag>(useSecond: true);

            if (Header.GetFieldValue<bool?>(Models.Metadata.Header.LockRomModeKey) != null)
                subheader.LockRomMode = Header.GetFieldValue<bool?>(Models.Metadata.Header.LockRomModeKey).FromYesNo();
            if (Header.GetFieldValue<bool?>(Models.Metadata.Header.LockBiosModeKey) != null)
                subheader.LockBiosMode = Header.GetFieldValue<bool?>(Models.Metadata.Header.LockBiosModeKey).FromYesNo();
            if (Header.GetFieldValue<bool?>(Models.Metadata.Header.LockSampleModeKey) != null)
                subheader.LockSampleMode = Header.GetFieldValue<bool?>(Models.Metadata.Header.LockSampleModeKey).FromYesNo();

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
                var machine = items[0].GetFieldValue<Machine>(DatItem.MachineKey);
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

            game.Name = machine.GetFieldValue<string?>(Models.Metadata.Machine.NameKey);
            game.SourceFile = machine.GetFieldValue<string?>(Models.Metadata.Machine.SourceFileKey);
            if (machine.GetFieldValue<bool?>(Models.Metadata.Machine.IsBiosKey) == true)
                game.IsBios = "yes";
            if (machine.GetFieldValue<bool?>(Models.Metadata.Machine.IsDeviceKey) == true)
                game.IsDevice = "yes";
            if (machine.GetFieldValue<bool?>(Models.Metadata.Machine.IsMechanicalKey) == true)
                game.IsMechanical = "yes";
            game.CloneOf = machine.GetFieldValue<string?>(Models.Metadata.Machine.CloneOfKey);
            game.RomOf = machine.GetFieldValue<string?>(Models.Metadata.Machine.RomOfKey);
            game.SampleOf = machine.GetFieldValue<string?>(Models.Metadata.Machine.SampleOfKey);
            game.Board = machine.GetFieldValue<string?>(Models.Metadata.Machine.BoardKey);
            game.RebuildTo = machine.GetFieldValue<string?>(Models.Metadata.Machine.RebuildToKey);
            game.Id = machine.GetFieldValue<string?>(Models.Metadata.Machine.IdKey);
            game.CloneOfId = machine.GetFieldValue<string?>(Models.Metadata.Machine.CloneOfIdKey);
            game.Runnable = machine.GetFieldValue<Runnable>(Models.Metadata.Machine.RunnableKey).AsStringValue<Runnable>();
            if (machine.GetFieldValue<string?>(Models.Metadata.Machine.CommentKey) != null)
            {
                if (machine.GetFieldValue<string?>(Models.Metadata.Machine.CommentKey)!.Contains(';'))
                    game.Comment = machine.GetFieldValue<string?>(Models.Metadata.Machine.CommentKey)!.Split(';');
                else
                    game.Comment = [machine.GetFieldValue<string?>(Models.Metadata.Machine.CommentKey)!];
            }
            game.Description = machine.GetFieldValue<string?>(Models.Metadata.Machine.DescriptionKey);
            game.Year = machine.GetFieldValue<string?>(Models.Metadata.Machine.YearKey);
            game.Manufacturer = machine.GetFieldValue<string?>(Models.Metadata.Machine.ManufacturerKey);
            game.Publisher = machine.GetFieldValue<string?>(Models.Metadata.Machine.PublisherKey);
            if (machine.GetFieldValue<string?>(Models.Metadata.Machine.CategoryKey) != null)
            {
                if (machine.GetFieldValue<string?>(Models.Metadata.Machine.CategoryKey)!.Contains(';'))
                    game.Category = machine.GetFieldValue<string?>(Models.Metadata.Machine.CategoryKey)!.Split(';');
                else
                    game.Category = [machine.GetFieldValue<string?>(Models.Metadata.Machine.CategoryKey)!];
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
                Publisher = machine.GetFieldValue<string?>(Models.Metadata.Machine.PublisherKey),
                Developer = machine.Developer,
                Year = machine.GetFieldValue<string?>(Models.Metadata.Machine.YearKey),
                Genre = machine.Genre,
                Subgenre = machine.Subgenre,
                Ratings = machine.Ratings,
                Score = machine.Score,
                Players = machine.GetFieldValue<string?>(Models.Metadata.Machine.PlayersKey),
                Enabled = machine.Enabled,
                CRC = machine.Crc.FromYesNo(),
                Source = machine.GetFieldValue<string?>(Models.Metadata.Machine.SourceFileKey),
                CloneOf = machine.GetFieldValue<string?>(Models.Metadata.Machine.CloneOfKey),
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
                Name = item.GetName(),
                Region = item.GetFieldValue<string?>(Models.Metadata.Release.RegionKey),
                Language = item.GetFieldValue<string?>(Models.Metadata.Release.LanguageKey),
                Date = item.GetFieldValue<string?>(Models.Metadata.Release.DateKey),
                Default = item.GetFieldValue<bool?>(Models.Metadata.Release.DefaultKey).FromYesNo(),
            };

            return release;
        }

        /// <summary>
        /// Create a BiosSet from the current BiosSet DatItem
        /// <summary>
        private static Models.Logiqx.BiosSet CreateBiosSet(BiosSet item)
        {
            var biosset = new Models.Logiqx.BiosSet
            {
                Name = item.GetName(),
                Default = item.GetFieldValue<bool?>(Models.Metadata.BiosSet.DefaultKey).FromYesNo(),
                Description = item.GetFieldValue<string?>(Models.Metadata.BiosSet.DescriptionKey),
            };

            return biosset;
        }

        /// <summary>
        /// Create a Rom from the current Rom DatItem
        /// <summary>
        private static Models.Logiqx.Rom CreateRom(Rom item)
        {
            var rom = new Models.Logiqx.Rom
            {
                Name = item.GetName(),
                Size = item.GetFieldValue<long?>(Models.Metadata.Rom.SizeKey)?.ToString(),
                CRC = item.GetFieldValue<string?>(Models.Metadata.Rom.CRCKey),
                MD5 = item.GetFieldValue<string?>(Models.Metadata.Rom.MD5Key),
                SHA1 = item.GetFieldValue<string?>(Models.Metadata.Rom.SHA1Key),
                SHA256 = item.GetFieldValue<string?>(Models.Metadata.Rom.SHA256Key),
                SHA384 = item.GetFieldValue<string?>(Models.Metadata.Rom.SHA384Key),
                SHA512 = item.GetFieldValue<string?>(Models.Metadata.Rom.SHA512Key),
                SpamSum = item.GetFieldValue<string?>(Models.Metadata.Rom.SpamSumKey),
                xxHash364 = item.GetFieldValue<string?>(Models.Metadata.Rom.xxHash364Key),
                xxHash3128 = item.GetFieldValue<string?>(Models.Metadata.Rom.xxHash3128Key),
                Merge = item.GetFieldValue<string?>(Models.Metadata.Rom.MergeKey),
                Serial = item.GetFieldValue<string?>(Models.Metadata.Rom.SerialKey),
                Header = item.GetFieldValue<string?>(Models.Metadata.Rom.HeaderKey),
                Date = item.GetFieldValue<string?>(Models.Metadata.Rom.DateKey),
                Inverted = item.GetFieldValue<bool?>(Models.Metadata.Rom.InvertedKey).FromYesNo(),
                MIA = item.GetFieldValue<bool?>(Models.Metadata.Rom.MIAKey).FromYesNo(),
            };

            if (item.ItemStatusSpecified)
                rom.Status = item.GetFieldValue<ItemStatus>(Models.Metadata.Rom.StatusKey).AsStringValue<ItemStatus>(useSecond: false);

            return rom;
        }

        /// <summary>
        /// Create a Disk from the current Disk DatItem
        /// <summary>
        private static Models.Logiqx.Disk CreateDisk(Disk item)
        {
            var disk = new Models.Logiqx.Disk
            {
                Name = item.GetName(),
                MD5 = item.GetFieldValue<string?>(Models.Metadata.Disk.MD5Key),
                SHA1 = item.GetFieldValue<string?>(Models.Metadata.Disk.SHA1Key),
                Merge = item.GetFieldValue<string?>(Models.Metadata.Disk.MergeKey),
                Region = item.GetFieldValue<string?>(Models.Metadata.Disk.RegionKey),
            };

            if (item.GetFieldValue<ItemStatus>(Models.Metadata.Disk.StatusKey) != ItemStatus.NULL)
                disk.Status = item.GetFieldValue<ItemStatus>(Models.Metadata.Disk.StatusKey).AsStringValue<ItemStatus>(useSecond: false);

            return disk;
        }

        /// <summary>
        /// Create a Media from the current Media DatItem
        /// <summary>
        private static Models.Logiqx.Media CreateMedia(Media item)
        {
            var media = new Models.Logiqx.Media
            {
                Name = item.GetName(),
                MD5 = item.GetFieldValue<string?>(Models.Metadata.Media.MD5Key),
                SHA1 = item.GetFieldValue<string?>(Models.Metadata.Media.SHA1Key),
                SHA256 = item.GetFieldValue<string?>(Models.Metadata.Media.SHA256Key),
                SpamSum = item.GetFieldValue<string?>(Models.Metadata.Media.SpamSumKey),
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
                Name = item.GetName(),
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
                Name = item.GetName(),
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
                Name = item.GetName(),
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
                Status = item.GetFieldValue<SupportStatus>(Models.Metadata.Driver.StatusKey).AsStringValue<SupportStatus>(),
                Emulation = item.GetFieldValue<SupportStatus>(Models.Metadata.Driver.EmulationKey).AsStringValue<SupportStatus>(),
                Cocktail = item.GetFieldValue<SupportStatus>(Models.Metadata.Driver.CocktailKey).AsStringValue<SupportStatus>(),
                SaveState = item.GetFieldValue<Supported>(Models.Metadata.Driver.SaveStateKey).AsStringValue<Supported>(useSecond: true),
                RequiresArtwork = item.GetFieldValue<bool?>(Models.Metadata.Driver.RequiresArtworkKey).FromYesNo(),
                Unofficial = item.GetFieldValue<bool?>(Models.Metadata.Driver.UnofficialKey).FromYesNo(),
                NoSoundHardware = item.GetFieldValue<bool?>(Models.Metadata.Driver.NoSoundHardwareKey).FromYesNo(),
                Incomplete = item.GetFieldValue<bool?>(Models.Metadata.Driver.IncompleteKey).FromYesNo(),
            };

            return driver;
        }

        /// <summary>
        /// Create a SoftwareList from the current SoftwareList DatItem
        /// <summary>
        private static Models.Logiqx.SoftwareList CreateSoftwareList(DatItems.Formats.SoftwareList item)
        {
            var softwarelist = new Models.Logiqx.SoftwareList
            {
                Tag = item.GetFieldValue<string?>(Models.Metadata.SoftwareList.TagKey),
                Name = item.GetName(),
                Filter = item.GetFieldValue<string?>(Models.Metadata.SoftwareList.FilterKey),
            };

            if (item.GetFieldValue<SoftwareListStatus?>(Models.Metadata.SoftwareList.StatusKey) != SoftwareListStatus.None)
                softwarelist.Status = item.GetFieldValue<SoftwareListStatus>(Models.Metadata.SoftwareList.StatusKey).AsStringValue<SoftwareListStatus>();

            return softwarelist;
        }

        #endregion
    }
}
