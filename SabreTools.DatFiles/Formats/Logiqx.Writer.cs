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

                case Media media:
                    if (string.IsNullOrEmpty(media.GetName()))
                        missingFields.Add(Models.Metadata.Media.NameKey);
                    if (string.IsNullOrEmpty(media.GetStringFieldValue(Models.Metadata.Media.MD5Key))
                        && string.IsNullOrEmpty(media.GetStringFieldValue(Models.Metadata.Media.SHA1Key))
                        && string.IsNullOrEmpty(media.GetStringFieldValue(Models.Metadata.Media.SHA256Key))
                        && string.IsNullOrEmpty(media.GetStringFieldValue(Models.Metadata.Media.SpamSumKey)))
                    {
                        missingFields.Add(Models.Metadata.Media.SHA1Key);
                    }
                    break;

                case DeviceRef deviceref:
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
                    if (driver.GetStringFieldValue(Models.Metadata.Driver.StatusKey).AsEnumValue<SupportStatus>() == SupportStatus.NULL)
                        missingFields.Add(Models.Metadata.Driver.StatusKey);
                    if (driver.GetStringFieldValue(Models.Metadata.Driver.EmulationKey).AsEnumValue<SupportStatus>() == SupportStatus.NULL)
                        missingFields.Add(Models.Metadata.Driver.EmulationKey);
                    if (driver.GetStringFieldValue(Models.Metadata.Driver.CocktailKey).AsEnumValue<SupportStatus>() == SupportStatus.NULL)
                        missingFields.Add(Models.Metadata.Driver.CocktailKey);
                    if (driver.GetStringFieldValue(Models.Metadata.Driver.SaveStateKey).AsEnumValue<SupportStatus>() == SupportStatus.NULL)
                        missingFields.Add(Models.Metadata.Driver.SaveStateKey);
                    break;

                case DatItems.Formats.SoftwareList softwarelist:
                    if (string.IsNullOrEmpty(softwarelist.GetStringFieldValue(Models.Metadata.SoftwareList.TagKey)))
                        missingFields.Add(Models.Metadata.SoftwareList.TagKey);
                    if (string.IsNullOrEmpty(softwarelist.GetName()))
                        missingFields.Add(Models.Metadata.SoftwareList.NameKey);
                    if (softwarelist.GetStringFieldValue(Models.Metadata.SoftwareList.StatusKey).AsEnumValue<SoftwareListStatus>() == SoftwareListStatus.None)
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
                if (string.IsNullOrEmpty(Header.GetStringFieldValue(Models.Metadata.Header.IdKey)))
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
                Build = Header.GetStringFieldValue(Models.Metadata.Header.BuildKey),
                Debug = Header.GetBoolFieldValue(Models.Metadata.Header.DebugKey).FromYesNo(),

                Header = CreateHeader(),
                Game = CreateGames(ignoreblanks)
            };

            if (!string.IsNullOrEmpty(Header.GetStringFieldValue(Models.Metadata.Header.IdKey)))
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
                Id = Header.GetStringFieldValue(Models.Metadata.Header.IdKey),
                Name = Header.GetStringFieldValue(Models.Metadata.Header.NameKey),
                Description = Header.GetStringFieldValue(Models.Metadata.Header.DescriptionKey),
                RootDir = Header.GetStringFieldValue(Models.Metadata.Header.RootDirKey),
                Category = Header.GetStringFieldValue(Models.Metadata.Header.CategoryKey),
                Version = Header.GetStringFieldValue(Models.Metadata.Header.VersionKey),
                Date = Header.GetStringFieldValue(Models.Metadata.Header.DateKey),
                Author = Header.GetStringFieldValue(Models.Metadata.Header.AuthorKey),
                Email = Header.GetStringFieldValue(Models.Metadata.Header.EmailKey),
                Homepage = Header.GetStringFieldValue(Models.Metadata.Header.HomepageKey),
                Url = Header.GetStringFieldValue(Models.Metadata.Header.UrlKey),
                Comment = Header.GetStringFieldValue(Models.Metadata.Header.CommentKey),
                Type = Header.GetStringFieldValue(Models.Metadata.Header.TypeKey),

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
            if (Header.GetStringFieldValue(Models.Metadata.Header.ForceMergingKey).AsEnumValue<MergingFlag>() == MergingFlag.None
                && Header.GetStringFieldValue(Models.Metadata.Header.ForceNodumpKey).AsEnumValue<NodumpFlag>() == NodumpFlag.None
                && Header.GetStringFieldValue(Models.Metadata.Header.ForcePackingKey).AsEnumValue<PackingFlag>() == PackingFlag.None
                && string.IsNullOrEmpty(Header.GetStringFieldValue(Models.Metadata.Header.HeaderKey)))
            {
                return null;
            }

            var subheader = new Models.Logiqx.ClrMamePro
            {
                Header = Header.GetStringFieldValue(Models.Metadata.Header.HeaderKey),
            };

            if (Header.GetStringFieldValue(Models.Metadata.Header.ForceMergingKey).AsEnumValue<MergingFlag>() != MergingFlag.None)
                subheader.ForceMerging = Header.GetStringFieldValue(Models.Metadata.Header.ForceMergingKey).AsEnumValue<MergingFlag>().AsStringValue(useSecond: false);
            if (Header.GetStringFieldValue(Models.Metadata.Header.ForceNodumpKey).AsEnumValue<NodumpFlag>() != NodumpFlag.None)
                subheader.ForceNodump = Header.GetStringFieldValue(Models.Metadata.Header.ForceNodumpKey).AsEnumValue<NodumpFlag>().AsStringValue();
            if (Header.GetStringFieldValue(Models.Metadata.Header.ForcePackingKey).AsEnumValue<PackingFlag>() != PackingFlag.None)
                subheader.ForcePacking = Header.GetStringFieldValue(Models.Metadata.Header.ForcePackingKey).AsEnumValue<PackingFlag>().AsStringValue(useSecond: false);

            return subheader;
        }

        /// <summary>
        /// Create a RomCenter from the current internal information
        /// <summary>
        private Models.Logiqx.RomCenter? CreateRomCenter()
        {
            // If we don't have subheader values, we can't do anything
            if (string.IsNullOrEmpty(Header.GetStringFieldValue(Models.Metadata.Header.SystemKey))
                && Header.GetStringFieldValue(Models.Metadata.Header.BiosModeKey).AsEnumValue<MergingFlag>() == MergingFlag.None
                && Header.GetBoolFieldValue(Models.Metadata.Header.LockBiosModeKey) == null
                && Header.GetBoolFieldValue(Models.Metadata.Header.LockRomModeKey) == null
                && Header.GetBoolFieldValue(Models.Metadata.Header.LockSampleModeKey) == null
                && Header.GetStringFieldValue(Models.Metadata.Header.RomModeKey).AsEnumValue<MergingFlag>() == MergingFlag.None
                && Header.GetStringFieldValue(Models.Metadata.Header.SampleModeKey).AsEnumValue<MergingFlag>() == MergingFlag.None)
            {
                return null;
            }

            var subheader = new Models.Logiqx.RomCenter
            {
                Plugin = Header.GetStringFieldValue(Models.Metadata.Header.PluginKey),
            };

            if (Header.GetStringFieldValue(Models.Metadata.Header.RomModeKey).AsEnumValue<MergingFlag>() != MergingFlag.None)
                subheader.RomMode = Header.GetStringFieldValue(Models.Metadata.Header.RomModeKey).AsEnumValue<MergingFlag>().AsStringValue(useSecond: true);
            if (Header.GetStringFieldValue(Models.Metadata.Header.BiosModeKey).AsEnumValue<MergingFlag>() != MergingFlag.None)
                subheader.BiosMode = Header.GetStringFieldValue(Models.Metadata.Header.BiosModeKey).AsEnumValue<MergingFlag>().AsStringValue(useSecond: true);
            if (Header.GetStringFieldValue(Models.Metadata.Header.SampleModeKey).AsEnumValue<MergingFlag>() != MergingFlag.None)
                subheader.SampleMode = Header.GetStringFieldValue(Models.Metadata.Header.SampleModeKey).AsEnumValue<MergingFlag>().AsStringValue(useSecond: true);

            if (Header.GetBoolFieldValue(Models.Metadata.Header.LockRomModeKey) != null)
                subheader.LockRomMode = Header.GetBoolFieldValue(Models.Metadata.Header.LockRomModeKey).FromYesNo();
            if (Header.GetBoolFieldValue(Models.Metadata.Header.LockBiosModeKey) != null)
                subheader.LockBiosMode = Header.GetBoolFieldValue(Models.Metadata.Header.LockBiosModeKey).FromYesNo();
            if (Header.GetBoolFieldValue(Models.Metadata.Header.LockSampleModeKey) != null)
                subheader.LockSampleMode = Header.GetBoolFieldValue(Models.Metadata.Header.LockSampleModeKey).FromYesNo();

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
                        case DeviceRef deviceref:
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

            game.Name = machine.GetStringFieldValue(Models.Metadata.Machine.NameKey);
            game.SourceFile = machine.GetStringFieldValue(Models.Metadata.Machine.SourceFileKey);
            if (machine.GetBoolFieldValue(Models.Metadata.Machine.IsBiosKey) == true)
                game.IsBios = "yes";
            if (machine.GetBoolFieldValue(Models.Metadata.Machine.IsDeviceKey) == true)
                game.IsDevice = "yes";
            if (machine.GetBoolFieldValue(Models.Metadata.Machine.IsMechanicalKey) == true)
                game.IsMechanical = "yes";
            game.CloneOf = machine.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey);
            game.RomOf = machine.GetStringFieldValue(Models.Metadata.Machine.RomOfKey);
            game.SampleOf = machine.GetStringFieldValue(Models.Metadata.Machine.SampleOfKey);
            game.Board = machine.GetStringFieldValue(Models.Metadata.Machine.BoardKey);
            game.RebuildTo = machine.GetStringFieldValue(Models.Metadata.Machine.RebuildToKey);
            game.Id = machine.GetStringFieldValue(Models.Metadata.Machine.IdKey);
            game.CloneOfId = machine.GetStringFieldValue(Models.Metadata.Machine.CloneOfIdKey);
            game.Runnable = machine.GetStringFieldValue(Models.Metadata.Machine.RunnableKey).AsEnumValue<Runnable>().AsStringValue();
            if (machine.GetStringFieldValue(Models.Metadata.Machine.CommentKey) != null)
            {
                if (machine.GetStringFieldValue(Models.Metadata.Machine.CommentKey)!.Contains(';'))
                    game.Comment = machine.GetStringFieldValue(Models.Metadata.Machine.CommentKey)!.Split(';');
                else
                    game.Comment = [machine.GetStringFieldValue(Models.Metadata.Machine.CommentKey)!];
            }
            game.Description = machine.GetStringFieldValue(Models.Metadata.Machine.DescriptionKey);
            game.Year = machine.GetStringFieldValue(Models.Metadata.Machine.YearKey);
            game.Manufacturer = machine.GetStringFieldValue(Models.Metadata.Machine.ManufacturerKey);
            game.Publisher = machine.GetStringFieldValue(Models.Metadata.Machine.PublisherKey);
            if (machine.GetStringFieldValue(Models.Metadata.Machine.CategoryKey) != null)
            {
                if (machine.GetStringFieldValue(Models.Metadata.Machine.CategoryKey)!.Contains(';'))
                    game.Category = machine.GetStringFieldValue(Models.Metadata.Machine.CategoryKey)!.Split(';');
                else
                    game.Category = [machine.GetStringFieldValue(Models.Metadata.Machine.CategoryKey)!];
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
                Publisher = machine.GetStringFieldValue(Models.Metadata.Machine.PublisherKey),
                Developer = machine.Developer,
                Year = machine.GetStringFieldValue(Models.Metadata.Machine.YearKey),
                Genre = machine.Genre,
                Subgenre = machine.Subgenre,
                Ratings = machine.Ratings,
                Score = machine.Score,
                Players = machine.GetStringFieldValue(Models.Metadata.Machine.PlayersKey),
                Enabled = machine.Enabled,
                CRC = machine.Crc.FromYesNo(),
                Source = machine.GetStringFieldValue(Models.Metadata.Machine.SourceFileKey),
                CloneOf = machine.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey),
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
        private static Models.Logiqx.BiosSet CreateBiosSet(BiosSet item)
        {
            var biosset = new Models.Logiqx.BiosSet
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
        private static Models.Logiqx.Rom CreateRom(Rom item)
        {
            var rom = new Models.Logiqx.Rom
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
                Serial = item.GetStringFieldValue(Models.Metadata.Rom.SerialKey),
                Header = item.GetStringFieldValue(Models.Metadata.Rom.HeaderKey),
                Date = item.GetStringFieldValue(Models.Metadata.Rom.DateKey),
                Inverted = item.GetBoolFieldValue(Models.Metadata.Rom.InvertedKey).FromYesNo(),
                MIA = item.GetBoolFieldValue(Models.Metadata.Rom.MIAKey).FromYesNo(),
            };

            if (item.ItemStatusSpecified)
                rom.Status = item.GetStringFieldValue(Models.Metadata.Rom.StatusKey).AsEnumValue<ItemStatus>().AsStringValue(useSecond: false);

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
                MD5 = item.GetStringFieldValue(Models.Metadata.Disk.MD5Key),
                SHA1 = item.GetStringFieldValue(Models.Metadata.Disk.SHA1Key),
                Merge = item.GetStringFieldValue(Models.Metadata.Disk.MergeKey),
                Region = item.GetStringFieldValue(Models.Metadata.Disk.RegionKey),
            };

            if (item.GetStringFieldValue(Models.Metadata.Disk.StatusKey).AsEnumValue<ItemStatus>() != ItemStatus.NULL)
                disk.Status = item.GetStringFieldValue(Models.Metadata.Disk.StatusKey).AsEnumValue<ItemStatus>().AsStringValue(useSecond: false);

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
        private static Models.Logiqx.DeviceRef CreateDeviceRef(DeviceRef item)
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
                Status = item.GetStringFieldValue(Models.Metadata.Driver.StatusKey).AsEnumValue<SupportStatus>().AsStringValue<SupportStatus>(),
                Emulation = item.GetStringFieldValue(Models.Metadata.Driver.EmulationKey).AsEnumValue<SupportStatus>().AsStringValue<SupportStatus>(),
                Cocktail = item.GetStringFieldValue(Models.Metadata.Driver.CocktailKey).AsEnumValue<SupportStatus>().AsStringValue<SupportStatus>(),
                SaveState = item.GetStringFieldValue(Models.Metadata.Driver.SaveStateKey).AsEnumValue<Supported>().AsStringValue<Supported>(useSecond: true),
                RequiresArtwork = item.GetBoolFieldValue(Models.Metadata.Driver.RequiresArtworkKey).FromYesNo(),
                Unofficial = item.GetBoolFieldValue(Models.Metadata.Driver.UnofficialKey).FromYesNo(),
                NoSoundHardware = item.GetBoolFieldValue(Models.Metadata.Driver.NoSoundHardwareKey).FromYesNo(),
                Incomplete = item.GetBoolFieldValue(Models.Metadata.Driver.IncompleteKey).FromYesNo(),
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
                Tag = item.GetStringFieldValue(Models.Metadata.SoftwareList.TagKey),
                Name = item.GetName(),
                Filter = item.GetStringFieldValue(Models.Metadata.SoftwareList.FilterKey),
            };

            if (item.GetStringFieldValue(Models.Metadata.SoftwareList.StatusKey).AsEnumValue<SoftwareListStatus>() != SoftwareListStatus.None)
                softwarelist.Status = item.GetStringFieldValue(Models.Metadata.SoftwareList.StatusKey).AsEnumValue<SoftwareListStatus>().AsStringValue();

            return softwarelist;
        }

        #endregion
    }
}
