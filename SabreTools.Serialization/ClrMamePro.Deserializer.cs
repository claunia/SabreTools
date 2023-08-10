using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SabreTools.IO.Readers;
using SabreTools.Models.ClrMamePro;
namespace SabreTools.Serialization
{
    /// <summary>
    /// Deserializer for ClrMamePro metadata files
    /// </summary>
    public partial class ClrMamePro
    {
        /// <summary>
        /// Deserializes a ClrMamePro metadata file to the defined type
        /// </summary>
        /// <param name="path">Path to the file to deserialize</param>
        /// <param name="quotes">Enable quotes on read, false otherwise</param>
        /// <returns>Deserialized data on success, null on failure</returns>
        public static MetadataFile? Deserialize(string path, bool quotes)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Deserialize(stream, quotes);
        }

        /// <summary>
        /// Deserializes a ClrMamePro metadata file in a stream to the defined type
        /// </summary>
        /// <param name="stream">Stream to deserialize</param>
        /// <param name="quotes">Enable quotes on read, false otherwise</param>
        /// <returns>Deserialized data on success, null on failure</returns>
        public static MetadataFile? Deserialize(Stream? stream, bool quotes)
        {
            // If the stream is null
            if (stream == null)
                return default;

            // Setup the reader and output
            var reader = new ClrMameProReader(stream, Encoding.UTF8) { Quotes = quotes };
            var dat = new MetadataFile();

            // Loop through and parse out the values
            string lastTopLevel = reader.TopLevel;

            GameBase? game = null;
            var games = new List<GameBase>();
            var releases = new List<Release>();
            var biosSets = new List<BiosSet>();
            var roms = new List<Rom>();
            var disks = new List<Disk>();
            var medias = new List<Media>();
            var samples = new List<Sample>();
            var archives = new List<Archive>();
            var chips = new List<Chip>();
            var videos = new List<Video>();
            var dipSwitches = new List<DipSwitch>();

            var additional = new List<string>();
            var headerAdditional = new List<string>();
            var gameAdditional = new List<string>();
            while (!reader.EndOfStream)
            {
                // If we have no next line
                if (!reader.ReadNextLine())
                    break;

                // Ignore certain row types
                switch (reader.RowType)
                {
                    case CmpRowType.None:
                    case CmpRowType.Comment:
                        continue;
                    case CmpRowType.EndTopLevel:
                        switch (lastTopLevel)
                        {
                            case "doscenter":
                                dat.ClrMamePro!.ADDITIONAL_ELEMENTS = headerAdditional.ToArray();
                                headerAdditional.Clear();
                                break;
                            case "game":
                            case "machine":
                            case "resource":
                            case "set":
                                game!.Release = releases.ToArray();
                                game.BiosSet = biosSets.ToArray();
                                game.Rom = roms.ToArray();
                                game.Disk = disks.ToArray();
                                game.Media = medias.ToArray();
                                game.Sample = samples.ToArray();
                                game.Archive = archives.ToArray();
                                game.Chip = chips.ToArray();
                                game.Video = videos.ToArray();
                                game.DipSwitch = dipSwitches.ToArray();
                                game.ADDITIONAL_ELEMENTS = gameAdditional.ToArray();

                                games.Add(game);
                                game = null;

                                releases.Clear();
                                biosSets.Clear();
                                roms.Clear();
                                disks.Clear();
                                medias.Clear();
                                samples.Clear();
                                archives.Clear();
                                chips.Clear();
                                videos.Clear();
                                dipSwitches.Clear();
                                gameAdditional.Clear();
                                break;
                            default:
                                // No-op
                                break;
                        }
                        continue;
                }

                // If we're at the root
                if (reader.RowType == CmpRowType.TopLevel)
                {
                    lastTopLevel = reader.TopLevel;
                    switch (reader.TopLevel)
                    {
                        case "clrmamepro":
                            dat.ClrMamePro = new Models.ClrMamePro.ClrMamePro();
                            break;
                        case "game":
                            game = new Game();
                            break;
                        case "machine":
                            game = new Machine();
                            break;
                        case "resource":
                            game = new Resource();
                            break;
                        case "set":
                            game = new Set();
                            break;
                        default:
                            additional.Add(reader.CurrentLine);
                            break;
                    }
                }

                // If we're in the doscenter block
                else if (reader.TopLevel == "clrmamepro"
                    && reader.RowType == CmpRowType.Standalone)
                {
                    // Create the block if we haven't already
                    dat.ClrMamePro ??= new Models.ClrMamePro.ClrMamePro();

                    switch (reader.Standalone?.Key?.ToLowerInvariant())
                    {
                        case "name":
                            dat.ClrMamePro.Name = reader.Standalone?.Value;
                            break;
                        case "description":
                            dat.ClrMamePro.Description = reader.Standalone?.Value;
                            break;
                        case "rootdir":
                            dat.ClrMamePro.RootDir = reader.Standalone?.Value;
                            break;
                        case "category":
                            dat.ClrMamePro.Category = reader.Standalone?.Value;
                            break;
                        case "version":
                            dat.ClrMamePro.Version = reader.Standalone?.Value;
                            break;
                        case "date":
                            dat.ClrMamePro.Date = reader.Standalone?.Value;
                            break;
                        case "author":
                            dat.ClrMamePro.Author = reader.Standalone?.Value;
                            break;
                        case "homepage":
                            dat.ClrMamePro.Homepage = reader.Standalone?.Value;
                            break;
                        case "url":
                            dat.ClrMamePro.Url = reader.Standalone?.Value;
                            break;
                        case "comment":
                            dat.ClrMamePro.Comment = reader.Standalone?.Value;
                            break;
                        case "header":
                            dat.ClrMamePro.Header = reader.Standalone?.Value;
                            break;
                        case "type":
                            dat.ClrMamePro.Type = reader.Standalone?.Value;
                            break;
                        case "forcemerging":
                            dat.ClrMamePro.ForceMerging = reader.Standalone?.Value;
                            break;
                        case "forcezipping":
                            dat.ClrMamePro.ForceZipping = reader.Standalone?.Value;
                            break;
                        case "forcepacking":
                            dat.ClrMamePro.ForcePacking = reader.Standalone?.Value;
                            break;
                        default:
                            headerAdditional.Add(reader.CurrentLine);
                            break;
                    }
                }

                // If we're in a game, machine, resource, or set block
                else if ((reader.TopLevel == "game"
                        || reader.TopLevel == "machine"
                        || reader.TopLevel == "resource"
                        || reader.TopLevel == "set")
                    && reader.RowType == CmpRowType.Standalone)
                {
                    // Create the block if we haven't already
                    game ??= reader.TopLevel switch
                    {
                        "game" => new Game(),
                        "machine" => new Machine(),
                        "resource" => new Resource(),
                        "set" => new Set(),
                        _ => throw new FormatException($"Unknown top-level block: {reader.TopLevel}"),
                    };

                    switch (reader.Standalone?.Key?.ToLowerInvariant())
                    {
                        case "name":
                            game.Name = reader.Standalone?.Value;
                            break;
                        case "description":
                            game.Description = reader.Standalone?.Value;
                            break;
                        case "year":
                            game.Year = reader.Standalone?.Value;
                            break;
                        case "manufacturer":
                            game.Manufacturer = reader.Standalone?.Value;
                            break;
                        case "category":
                            game.Category = reader.Standalone?.Value;
                            break;
                        case "cloneof":
                            game.CloneOf = reader.Standalone?.Value;
                            break;
                        case "romof":
                            game.RomOf = reader.Standalone?.Value;
                            break;
                        case "sampleof":
                            game.SampleOf = reader.Standalone?.Value;
                            break;
                        case "sample":
                            var sample = new Sample
                            {
                                Name = reader.Standalone?.Value ?? string.Empty,
                                ADDITIONAL_ELEMENTS = Array.Empty<string>()
                            };
                            samples.Add(sample);
                            break;
                        default:
                            gameAdditional.Add(reader.CurrentLine);
                            break;
                    }
                }

                // If we're in an item block
                else if ((reader.TopLevel == "game"
                        || reader.TopLevel == "machine"
                        || reader.TopLevel == "resource"
                        || reader.TopLevel == "set")
                    && game != null
                    && reader.RowType == CmpRowType.Internal)
                {
                    // Create the block
                    switch (reader.InternalName)
                    {
                        case "release":
                            releases.Add(CreateRelease(reader));
                            break;
                        case "biosset":
                            biosSets.Add(CreateBiosSet(reader));
                            break;
                        case "rom":
                            roms.Add(CreateRom(reader));
                            break;
                        case "disk":
                            disks.Add(CreateDisk(reader));
                            break;
                        case "media":
                            medias.Add(CreateMedia(reader));
                            break;
                        case "sample":
                            samples.Add(CreateSample(reader));
                            break;
                        case "archive":
                            archives.Add(CreateArchive(reader));
                            break;
                        case "chip":
                            chips.Add(CreateChip(reader));
                            break;
                        case "video":
                            videos.Add(CreateVideo(reader));
                            break;
                        case "sound":
                            game.Sound = CreateSound(reader);
                            break;
                        case "input":
                            game.Input = CreateInput(reader);
                            break;
                        case "dipswitch":
                            dipSwitches.Add(CreateDipSwitch(reader));
                            break;
                        case "driver":
                            game.Driver = CreateDriver(reader);
                            break;
                        default:
                            gameAdditional.Add(reader.CurrentLine);
                            continue;
                    }
                }

                else
                {
                    additional.Add(reader.CurrentLine);
                }
            }

            // Add extra pieces and return
            dat.Game = games.ToArray();
            dat.ADDITIONAL_ELEMENTS = additional.ToArray();
            return dat;
        }

        /// <summary>
        /// Create a Release object from the current reader context
        /// </summary>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        /// <returns>Release object created from the reader context</returns>
        private static Release CreateRelease(ClrMameProReader reader)
        {
            var itemAdditional = new List<string>();
            var release = new Release();
            foreach (var kvp in reader.Internal)
            {
                switch (kvp.Key?.ToLowerInvariant())
                {
                    case "name":
                        release.Name = kvp.Value;
                        break;
                    case "region":
                        release.Region = kvp.Value;
                        break;
                    case "language":
                        release.Language = kvp.Value;
                        break;
                    case "date":
                        release.Date = kvp.Value;
                        break;
                    case "default":
                        release.Default = kvp.Value;
                        break;
                    default:
                        itemAdditional.Add($"{kvp.Key}: {kvp.Value}");
                        break;
                }
            }

            release.ADDITIONAL_ELEMENTS = itemAdditional.ToArray();
            return release;
        }

        /// <summary>
        /// Create a BiosSet object from the current reader context
        /// </summary>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        /// <returns>BiosSet object created from the reader context</returns>
        private static BiosSet CreateBiosSet(ClrMameProReader reader)
        {
            var itemAdditional = new List<string>();
            var biosset = new BiosSet();
            foreach (var kvp in reader.Internal)
            {
                switch (kvp.Key?.ToLowerInvariant())
                {
                    case "name":
                        biosset.Name = kvp.Value;
                        break;
                    case "description":
                        biosset.Description = kvp.Value;
                        break;
                    case "default":
                        biosset.Default = kvp.Value;
                        break;
                    default:
                        itemAdditional.Add($"{kvp.Key}: {kvp.Value}");
                        break;
                }
            }

            biosset.ADDITIONAL_ELEMENTS = itemAdditional.ToArray();
            return biosset;
        }

        /// <summary>
        /// Create a Rom object from the current reader context
        /// </summary>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        /// <returns>Rom object created from the reader context</returns>
        private static Rom CreateRom(ClrMameProReader reader)
        {
            var itemAdditional = new List<string>();
            var rom = new Rom();
            foreach (var kvp in reader.Internal)
            {
                switch (kvp.Key?.ToLowerInvariant())
                {
                    case "name":
                        rom.Name = kvp.Value;
                        break;
                    case "size":
                        rom.Size = kvp.Value;
                        break;
                    case "crc":
                        rom.CRC = kvp.Value;
                        break;
                    case "md5":
                        rom.MD5 = kvp.Value;
                        break;
                    case "sha1":
                        rom.SHA1 = kvp.Value;
                        break;
                    case "sha256":
                        rom.SHA256 = kvp.Value;
                        break;
                    case "sha384":
                        rom.SHA384 = kvp.Value;
                        break;
                    case "sha512":
                        rom.SHA512 = kvp.Value;
                        break;
                    case "spamsum":
                        rom.SpamSum = kvp.Value;
                        break;
                    case "xxh3_64":
                        rom.xxHash364 = kvp.Value;
                        break;
                    case "xxh3_128":
                        rom.xxHash3128 = kvp.Value;
                        break;
                    case "merge":
                        rom.Merge = kvp.Value;
                        break;
                    case "status":
                        rom.Status = kvp.Value;
                        break;
                    case "region":
                        rom.Region = kvp.Value;
                        break;
                    case "flags":
                        rom.Flags = kvp.Value;
                        break;
                    case "offs":
                        rom.Offs = kvp.Value;
                        break;
                    case "serial":
                        rom.Serial = kvp.Value;
                        break;
                    case "header":
                        rom.Header = kvp.Value;
                        break;
                    case "date":
                        rom.Date = kvp.Value;
                        break;
                    case "inverted":
                        rom.Inverted = kvp.Value;
                        break;
                    case "mia":
                        rom.MIA = kvp.Value;
                        break;
                    default:
                        itemAdditional.Add($"{kvp.Key}: {kvp.Value}");
                        break;
                }
            }

            rom.ADDITIONAL_ELEMENTS = itemAdditional.ToArray();
            return rom;
        }

        /// <summary>
        /// Create a Disk object from the current reader context
        /// </summary>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        /// <returns>Disk object created from the reader context</returns>
        private static Disk CreateDisk(ClrMameProReader reader)
        {
            var itemAdditional = new List<string>();
            var disk = new Disk();
            foreach (var kvp in reader.Internal)
            {
                switch (kvp.Key?.ToLowerInvariant())
                {
                    case "name":
                        disk.Name = kvp.Value;
                        break;
                    case "md5":
                        disk.MD5 = kvp.Value;
                        break;
                    case "sha1":
                        disk.SHA1 = kvp.Value;
                        break;
                    case "merge":
                        disk.Merge = kvp.Value;
                        break;
                    case "status":
                        disk.Status = kvp.Value;
                        break;
                    case "flags":
                        disk.Flags = kvp.Value;
                        break;
                    default:
                        itemAdditional.Add($"{kvp.Key}: {kvp.Value}");
                        break;
                }
            }

            disk.ADDITIONAL_ELEMENTS = itemAdditional.ToArray();
            return disk;
        }

        /// <summary>
        /// Create a Media object from the current reader context
        /// </summary>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        /// <returns>Media object created from the reader context</returns>
        private static Media CreateMedia(ClrMameProReader reader)
        {
            var itemAdditional = new List<string>();
            var media = new Media();
            foreach (var kvp in reader.Internal)
            {
                switch (kvp.Key?.ToLowerInvariant())
                {
                    case "name":
                        media.Name = kvp.Value;
                        break;
                    case "md5":
                        media.MD5 = kvp.Value;
                        break;
                    case "sha1":
                        media.SHA1 = kvp.Value;
                        break;
                    case "sha256":
                        media.SHA256 = kvp.Value;
                        break;
                    case "spamsum":
                        media.SpamSum = kvp.Value;
                        break;
                    default:
                        itemAdditional.Add($"{kvp.Key}: {kvp.Value}");
                        break;
                }
            }

            media.ADDITIONAL_ELEMENTS = itemAdditional.ToArray();
            return media;
        }

        /// <summary>
        /// Create a Sample object from the current reader context
        /// </summary>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        /// <returns>Sample object created from the reader context</returns>
        private static Sample CreateSample(ClrMameProReader reader)
        {
            var itemAdditional = new List<string>();
            var sample = new Sample();
            foreach (var kvp in reader.Internal)
            {
                switch (kvp.Key?.ToLowerInvariant())
                {
                    case "name":
                        sample.Name = kvp.Value;
                        break;
                    default:
                        itemAdditional.Add($"{kvp.Key}: {kvp.Value}");
                        break;
                }
            }

            sample.ADDITIONAL_ELEMENTS = itemAdditional.ToArray();
            return sample;
        }

        /// <summary>
        /// Create a Archive object from the current reader context
        /// </summary>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        /// <returns>Archive object created from the reader context</returns>
        private static Archive CreateArchive(ClrMameProReader reader)
        {
            var itemAdditional = new List<string>();
            var archive = new Archive();
            foreach (var kvp in reader.Internal)
            {
                switch (kvp.Key?.ToLowerInvariant())
                {
                    case "name":
                        archive.Name = kvp.Value;
                        break;
                    default:
                        itemAdditional.Add($"{kvp.Key}: {kvp.Value}");
                        break;
                }
            }

            archive.ADDITIONAL_ELEMENTS = itemAdditional.ToArray();
            return archive;
        }

        /// <summary>
        /// Create a Chip object from the current reader context
        /// </summary>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        /// <returns>Chip object created from the reader context</returns>
        private static Chip CreateChip(ClrMameProReader reader)
        {
            var itemAdditional = new List<string>();
            var chip = new Chip();
            foreach (var kvp in reader.Internal)
            {
                switch (kvp.Key?.ToLowerInvariant())
                {
                    case "type":
                        chip.Type = kvp.Value;
                        break;
                    case "name":
                        chip.Name = kvp.Value;
                        break;
                    case "flags":
                        chip.Flags = kvp.Value;
                        break;
                    case "clock":
                        chip.Clock = kvp.Value;
                        break;
                    default:
                        itemAdditional.Add($"{kvp.Key}: {kvp.Value}");
                        break;
                }
            }

            chip.ADDITIONAL_ELEMENTS = itemAdditional.ToArray();
            return chip;
        }

        /// <summary>
        /// Create a Video object from the current reader context
        /// </summary>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        /// <returns>Video object created from the reader context</returns>
        private static Video CreateVideo(ClrMameProReader reader)
        {
            var itemAdditional = new List<string>();
            var video = new Video();
            foreach (var kvp in reader.Internal)
            {
                switch (kvp.Key?.ToLowerInvariant())
                {
                    case "screen":
                        video.Screen = kvp.Value;
                        break;
                    case "orientation":
                        video.Orientation = kvp.Value;
                        break;
                    case "x":
                        video.X = kvp.Value;
                        break;
                    case "y":
                        video.Y = kvp.Value;
                        break;
                    case "aspectx":
                        video.AspectX = kvp.Value;
                        break;
                    case "aspecty":
                        video.AspectY = kvp.Value;
                        break;
                    case "freq":
                        video.Freq = kvp.Value;
                        break;
                    default:
                        itemAdditional.Add($"{kvp.Key}: {kvp.Value}");
                        break;
                }
            }

            video.ADDITIONAL_ELEMENTS = itemAdditional.ToArray();
            return video;
        }

        /// <summary>
        /// Create a Sound object from the current reader context
        /// </summary>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        /// <returns>Sound object created from the reader context</returns>
        private static Sound CreateSound(ClrMameProReader reader)
        {
            var itemAdditional = new List<string>();
            var sound = new Sound();
            foreach (var kvp in reader.Internal)
            {
                switch (kvp.Key?.ToLowerInvariant())
                {
                    case "channels":
                        sound.Channels = kvp.Value;
                        break;
                    default:
                        itemAdditional.Add($"{kvp.Key}: {kvp.Value}");
                        break;
                }
            }

            sound.ADDITIONAL_ELEMENTS = itemAdditional.ToArray();
            return sound;
        }

        /// <summary>
        /// Create a Input object from the current reader context
        /// </summary>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        /// <returns>Input object created from the reader context</returns>
        private static Input CreateInput(ClrMameProReader reader)
        {
            var itemAdditional = new List<string>();
            var input = new Input();
            foreach (var kvp in reader.Internal)
            {
                switch (kvp.Key?.ToLowerInvariant())
                {
                    case "players":
                        input.Players = kvp.Value;
                        break;
                    case "control":
                        input.Control = kvp.Value;
                        break;
                    case "buttons":
                        input.Buttons = kvp.Value;
                        break;
                    case "coins":
                        input.Coins = kvp.Value;
                        break;
                    case "tilt":
                        input.Tilt = kvp.Value;
                        break;
                    case "service":
                        input.Service = kvp.Value;
                        break;
                    default:
                        itemAdditional.Add($"{kvp.Key}: {kvp.Value}");
                        break;
                }
            }

            input.ADDITIONAL_ELEMENTS = itemAdditional.ToArray();
            return input;
        }

        /// <summary>
        /// Create a DipSwitch object from the current reader context
        /// </summary>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        /// <returns>DipSwitch object created from the reader context</returns>
        private static DipSwitch CreateDipSwitch(ClrMameProReader reader)
        {
            var itemAdditional = new List<string>();
            var dipswitch = new DipSwitch();
            var entries = new List<string>();
            foreach (var kvp in reader.Internal)
            {
                switch (kvp.Key?.ToLowerInvariant())
                {
                    case "name":
                        dipswitch.Name = kvp.Value;
                        break;
                    case "entry":
                        entries.Add(kvp.Value);
                        break;
                    case "default":
                        dipswitch.Default = kvp.Value;
                        break;
                    default:
                        itemAdditional.Add($"{kvp.Key}: {kvp.Value}");
                        break;
                }
            }

            dipswitch.Entry = entries.ToArray();
            dipswitch.ADDITIONAL_ELEMENTS = itemAdditional.ToArray();
            return dipswitch;
        }

        /// <summary>
        /// Create a Driver object from the current reader context
        /// </summary>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        /// <returns>Driver object created from the reader context</returns>
        private static Driver CreateDriver(ClrMameProReader reader)
        {
            var itemAdditional = new List<string>();
            var driver = new Driver();
            foreach (var kvp in reader.Internal)
            {
                switch (kvp.Key?.ToLowerInvariant())
                {
                    case "status":
                        driver.Status = kvp.Value;
                        break;
                    case "color":
                        driver.Color = kvp.Value;
                        break;
                    case "sound":
                        driver.Sound = kvp.Value;
                        break;
                    case "palettesize":
                        driver.PaletteSize = kvp.Value;
                        break;
                    case "blit":
                        driver.Blit = kvp.Value;
                        break;
                    default:
                        itemAdditional.Add($"{kvp.Key}: {kvp.Value}");
                        break;
                }
            }

            driver.ADDITIONAL_ELEMENTS = itemAdditional.ToArray();
            return driver;
        }
    
        #region Internal

        /// <summary>
        /// Convert from <cref="Models.Internal.MetadataFile"/> to <cref="Models.ClrMamePro.MetadataFile"/>
        /// </summary>
        public static MetadataFile? ConvertFromInternalModel(Models.Internal.MetadataFile? item, bool game = false)
        {
            if (item == null)
                return null;
            
            var metadataFile = new MetadataFile();

            var header = item.Read<Models.Internal.Header>(Models.Internal.MetadataFile.HeaderKey);
            if (header != null)
                metadataFile.ClrMamePro = ConvertHeaderFromInternalModel(header);

            var machines = item.Read<Models.Internal.Machine[]>(Models.Internal.MetadataFile.MachineKey);
            if (machines != null && machines.Any())
            {
                metadataFile.Game = machines
                    .Where(m => m != null)
                    .Select(machine => ConvertMachineFromInternalModel(machine, game))
                    .ToArray();
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Header"/> to <cref="Models.ClrMamePro.ClrMamePro"/>
        /// </summary>
        private static Models.ClrMamePro.ClrMamePro ConvertHeaderFromInternalModel(Models.Internal.Header item)
        {
            var clrMamePro = new Models.ClrMamePro.ClrMamePro
            {
                Name = item.ReadString(Models.Internal.Header.NameKey),
                Description = item.ReadString(Models.Internal.Header.DescriptionKey),
                RootDir = item.ReadString(Models.Internal.Header.RootDirKey),
                Category = item.ReadString(Models.Internal.Header.CategoryKey),
                Version = item.ReadString(Models.Internal.Header.VersionKey),
                Date = item.ReadString(Models.Internal.Header.DateKey),
                Author = item.ReadString(Models.Internal.Header.AuthorKey),
                Homepage = item.ReadString(Models.Internal.Header.HomepageKey),
                Url = item.ReadString(Models.Internal.Header.UrlKey),
                Comment = item.ReadString(Models.Internal.Header.CommentKey),
                Header = item.ReadString(Models.Internal.Header.HeaderKey),
                Type = item.ReadString(Models.Internal.Header.TypeKey),
                ForceMerging = item.ReadString(Models.Internal.Header.ForceMergingKey),
                ForceZipping = item.ReadString(Models.Internal.Header.ForceZippingKey),
                ForcePacking = item.ReadString(Models.Internal.Header.ForcePackingKey),
            };
            return clrMamePro;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Archive"/> to <cref="Models.ClrMamePro.Machine"/>
        /// </summary>
        private static GameBase ConvertMachineFromInternalModel(Models.Internal.Machine item, bool game = false)
        {
            GameBase gameBase = game ? new Models.ClrMamePro.Game() : new Models.ClrMamePro.Machine();

            gameBase.Name = item.ReadString(Models.Internal.Machine.NameKey);
            gameBase.Description = item.ReadString(Models.Internal.Machine.DescriptionKey);
            gameBase.Year = item.ReadString(Models.Internal.Machine.YearKey);
            gameBase.Manufacturer = item.ReadString(Models.Internal.Machine.ManufacturerKey);
            gameBase.Category = item.ReadString(Models.Internal.Machine.CategoryKey);
            gameBase.CloneOf = item.ReadString(Models.Internal.Machine.CloneOfKey);
            gameBase.RomOf = item.ReadString(Models.Internal.Machine.RomOfKey);
            gameBase.SampleOf = item.ReadString(Models.Internal.Machine.SampleOfKey);

            var releases = item.Read<Models.Internal.Release[]>(Models.Internal.Machine.ReleaseKey);
            if (releases != null && releases.Any())
            {
                gameBase.Release = releases
                    .Where(r => r != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var biosSets = item.Read<Models.Internal.BiosSet[]>(Models.Internal.Machine.BiosSetKey);
            if (biosSets != null && biosSets.Any())
            {
                gameBase.BiosSet = biosSets
                    .Where(r => r != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var roms = item.Read<Models.Internal.Rom[]>(Models.Internal.Machine.RomKey);
            if (roms != null && roms.Any())
            {
                gameBase.Rom = roms
                    .Where(r => r != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var disks = item.Read<Models.Internal.Disk[]>(Models.Internal.Machine.DiskKey);
            if (disks != null && disks.Any())
            {
                gameBase.Disk = disks
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var medias = item.Read<Models.Internal.Media[]>(Models.Internal.Machine.MediaKey);
            if (medias != null && medias.Any())
            {
                gameBase.Media = medias
                    .Where(m => m != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var samples = item.Read<Models.Internal.Sample[]>(Models.Internal.Machine.SampleKey);
            if (samples != null && samples.Any())
            {
                gameBase.Sample = samples
                    .Where(m => m != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var archives = item.Read<Models.Internal.Archive[]>(Models.Internal.Machine.ArchiveKey);
            if (archives != null && archives.Any())
            {
                gameBase.Archive = archives
                    .Where(m => m != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var chips = item.Read<Models.Internal.Chip[]>(Models.Internal.Machine.ChipKey);
            if (chips != null && chips.Any())
            {
                gameBase.Chip = chips
                    .Where(m => m != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var videos = item.Read<Models.Internal.Video[]>(Models.Internal.Machine.VideoKey);
            if (videos != null && videos.Any())
            {
                gameBase.Video = videos
                    .Where(m => m != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var sound = item.Read<Models.Internal.Sound>(Models.Internal.Machine.SoundKey);
            if (sound != null)
                gameBase.Sound = ConvertFromInternalModel(sound);

            var input = item.Read<Models.Internal.Input>(Models.Internal.Machine.InputKey);
            if (input != null)
                gameBase.Input = ConvertFromInternalModel(input);

            var dipSwitches = item.Read<Models.Internal.DipSwitch[]>(Models.Internal.Machine.DipSwitchKey);
            if (dipSwitches != null && dipSwitches.Any())
            {
                gameBase.DipSwitch = dipSwitches
                    .Where(m => m != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var driver = item.Read<Models.Internal.Driver>(Models.Internal.Machine.DriverKey);
            if (driver != null)
                gameBase.Driver = ConvertFromInternalModel(driver);

            return gameBase;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Archive"/> to <cref="Models.ClrMamePro.Archive"/>
        /// </summary>
        private static Archive ConvertFromInternalModel(Models.Internal.Archive item)
        {
            var archive = new Archive
            {
                Name = item.ReadString(Models.Internal.Archive.NameKey),
            };
            return archive;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.BiosSet"/> to <cref="Models.ClrMamePro.BiosSet"/>
        /// </summary>
        private static BiosSet ConvertFromInternalModel(Models.Internal.BiosSet item)
        {
            var biosset = new BiosSet
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
        private static Chip ConvertFromInternalModel(Models.Internal.Chip item)
        {
            var chip = new Chip
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
        private static DipSwitch ConvertFromInternalModel(Models.Internal.DipSwitch item)
        {
            var dipswitch = new DipSwitch
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
        private static Disk ConvertFromInternalModel(Models.Internal.Disk item)
        {
            var disk = new Disk
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
        private static Driver ConvertFromInternalModel(Models.Internal.Driver item)
        {
            var driver = new Driver
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
        private static Input ConvertFromInternalModel(Models.Internal.Input item)
        {
            var input = new Input
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
        private static Media ConvertFromInternalModel(Models.Internal.Media item)
        {
            var media = new Media
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
        private static Release ConvertFromInternalModel(Models.Internal.Release item)
        {
            var release = new Release
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
        private static Rom ConvertFromInternalModel(Models.Internal.Rom item)
        {
            var rom = new Rom
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
        private static Sample ConvertFromInternalModel(Models.Internal.Sample item)
        {
            var sample = new Sample
            {
                Name = item.ReadString(Models.Internal.Sample.NameKey),
            };
            return sample;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Sound"/> to <cref="Models.ClrMamePro.Sound"/>
        /// </summary>
        private static Sound ConvertFromInternalModel(Models.Internal.Sound item)
        {
            var sound = new Sound
            {
                Channels = item.ReadString(Models.Internal.Sound.ChannelsKey),
            };
            return sound;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Video"/> to <cref="Models.ClrMamePro.Video"/>
        /// </summary>
        private static Video ConvertFromInternalModel(Models.Internal.Video item)
        {
            var video = new Video
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