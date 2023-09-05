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
            string? lastTopLevel = reader.TopLevel;

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
                                if (dat.ClrMamePro != null)
                                    dat.ClrMamePro.ADDITIONAL_ELEMENTS = headerAdditional.ToArray();

                                headerAdditional.Clear();
                                break;
                            case "game":
                            case "machine":
                            case "resource":
                            case "set":
                                if (game != null)
                                {
                                    game.Release = releases.ToArray();
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
                                }

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
                            if (reader.CurrentLine != null)
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
                            if (reader.CurrentLine != null)
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
                            if (reader.CurrentLine != null)
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
                            var release = CreateRelease(reader);
                            if (release != null)
                                releases.Add(release);
                            break;
                        case "biosset":
                            var biosSet = CreateBiosSet(reader);
                            if (biosSet != null)
                                biosSets.Add(biosSet);
                            break;
                        case "rom":
                            var rom = CreateRom(reader);
                            if (rom != null)
                                roms.Add(rom);
                            break;
                        case "disk":
                            var disk = CreateDisk(reader);
                            if (disk != null)
                                disks.Add(disk);
                            break;
                        case "media":
                            var media = CreateMedia(reader);
                            if (media != null)
                                medias.Add(media);
                            break;
                        case "sample":
                            var sample = CreateSample(reader);
                            if (sample != null)
                                samples.Add(sample);
                            break;
                        case "archive":
                            var archive = CreateArchive(reader);
                            if (archive != null)
                                archives.Add(archive);
                            break;
                        case "chip":
                            var chip = CreateChip(reader);
                            if (chip != null)
                                chips.Add(chip);
                            break;
                        case "video":
                            var video = CreateVideo(reader);
                            if (video != null)
                                videos.Add(video);
                            break;
                        case "sound":
                            var sound = CreateSound(reader);
                            if (sound != null)
                                game.Sound = sound;
                            break;
                        case "input":
                            var input = CreateInput(reader);
                            if (input != null)
                                game.Input = input;
                            break;
                        case "dipswitch":
                            var dipSwitch = CreateDipSwitch(reader);
                            if (dipSwitch != null)
                                dipSwitches.Add(dipSwitch);
                            break;
                        case "driver":
                            var driver = CreateDriver(reader);
                            if (driver != null)
                                game.Driver = driver;
                            break;
                        default:
                            if (reader.CurrentLine != null)
                                gameAdditional.Add(reader.CurrentLine);
                            continue;
                    }
                }

                else
                {
                    if (reader.CurrentLine != null)
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
        private static Release? CreateRelease(ClrMameProReader reader)
        {
            if (reader.Internal == null)
                return null;

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
        private static BiosSet? CreateBiosSet(ClrMameProReader reader)
        {
            if (reader.Internal == null)
                return null;

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
        private static Rom? CreateRom(ClrMameProReader reader)
        {
            if (reader.Internal == null)
                return null;

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
        private static Disk? CreateDisk(ClrMameProReader reader)
        {
            if (reader.Internal == null)
                return null;

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
        private static Media? CreateMedia(ClrMameProReader reader)
        {
            if (reader.Internal == null)
                return null;

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
        private static Sample? CreateSample(ClrMameProReader reader)
        {
            if (reader.Internal == null)
                return null;

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
        private static Archive? CreateArchive(ClrMameProReader reader)
        {
            if (reader.Internal == null)
                return null;

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
        private static Chip? CreateChip(ClrMameProReader reader)
        {
            if (reader.Internal == null)
                return null;

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
        private static Video? CreateVideo(ClrMameProReader reader)
        {
            if (reader.Internal == null)
                return null;

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
        private static Sound? CreateSound(ClrMameProReader reader)
        {
            if (reader.Internal == null)
                return null;

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
        private static Input? CreateInput(ClrMameProReader reader)
        {
            if (reader.Internal == null)
                return null;

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
        private static DipSwitch? CreateDipSwitch(ClrMameProReader reader)
        {
            if (reader.Internal == null)
                return null;

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
        private static Driver? CreateDriver(ClrMameProReader reader)
        {
            if (reader.Internal == null)
                return null;

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
        /// Convert from <cref="Models.Metadata.MetadataFile"/> to <cref="Models.ClrMamePro.MetadataFile"/>
        /// </summary>
        public static MetadataFile? ConvertFromInternalModel(Models.Metadata.MetadataFile? item, bool game = false)
        {
            if (item == null)
                return null;

            var metadataFile = new MetadataFile();

            var header = item.Read<Models.Metadata.Header>(Models.Metadata.MetadataFile.HeaderKey);
            if (header != null)
                metadataFile.ClrMamePro = ConvertHeaderFromInternalModel(header);

            var machines = item.Read<Models.Metadata.Machine[]>(Models.Metadata.MetadataFile.MachineKey);
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
        /// Convert from <cref="Models.Metadata.Header"/> to <cref="Models.ClrMamePro.ClrMamePro"/>
        /// </summary>
        private static Models.ClrMamePro.ClrMamePro ConvertHeaderFromInternalModel(Models.Metadata.Header item)
        {
            var clrMamePro = new Models.ClrMamePro.ClrMamePro
            {
                Name = item.ReadString(Models.Metadata.Header.NameKey),
                Description = item.ReadString(Models.Metadata.Header.DescriptionKey),
                RootDir = item.ReadString(Models.Metadata.Header.RootDirKey),
                Category = item.ReadString(Models.Metadata.Header.CategoryKey),
                Version = item.ReadString(Models.Metadata.Header.VersionKey),
                Date = item.ReadString(Models.Metadata.Header.DateKey),
                Author = item.ReadString(Models.Metadata.Header.AuthorKey),
                Homepage = item.ReadString(Models.Metadata.Header.HomepageKey),
                Url = item.ReadString(Models.Metadata.Header.UrlKey),
                Comment = item.ReadString(Models.Metadata.Header.CommentKey),
                Header = item.ReadString(Models.Metadata.Header.HeaderKey),
                Type = item.ReadString(Models.Metadata.Header.TypeKey),
                ForceMerging = item.ReadString(Models.Metadata.Header.ForceMergingKey),
                ForceZipping = item.ReadString(Models.Metadata.Header.ForceZippingKey),
                ForcePacking = item.ReadString(Models.Metadata.Header.ForcePackingKey),
            };
            return clrMamePro;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Archive"/> to <cref="Models.ClrMamePro.Machine"/>
        /// </summary>
        private static GameBase ConvertMachineFromInternalModel(Models.Metadata.Machine item, bool game = false)
        {
            GameBase gameBase = game ? new Models.ClrMamePro.Game() : new Models.ClrMamePro.Machine();

            gameBase.Name = item.ReadString(Models.Metadata.Machine.NameKey);
            gameBase.Description = item.ReadString(Models.Metadata.Machine.DescriptionKey);
            gameBase.Year = item.ReadString(Models.Metadata.Machine.YearKey);
            gameBase.Manufacturer = item.ReadString(Models.Metadata.Machine.ManufacturerKey);
            gameBase.Category = item.ReadString(Models.Metadata.Machine.CategoryKey);
            gameBase.CloneOf = item.ReadString(Models.Metadata.Machine.CloneOfKey);
            gameBase.RomOf = item.ReadString(Models.Metadata.Machine.RomOfKey);
            gameBase.SampleOf = item.ReadString(Models.Metadata.Machine.SampleOfKey);

            var releases = item.Read<Models.Metadata.Release[]>(Models.Metadata.Machine.ReleaseKey);
            if (releases != null && releases.Any())
            {
                gameBase.Release = releases
                    .Where(r => r != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var biosSets = item.Read<Models.Metadata.BiosSet[]>(Models.Metadata.Machine.BiosSetKey);
            if (biosSets != null && biosSets.Any())
            {
                gameBase.BiosSet = biosSets
                    .Where(r => r != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var roms = item.Read<Models.Metadata.Rom[]>(Models.Metadata.Machine.RomKey);
            if (roms != null && roms.Any())
            {
                gameBase.Rom = roms
                    .Where(r => r != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var disks = item.Read<Models.Metadata.Disk[]>(Models.Metadata.Machine.DiskKey);
            if (disks != null && disks.Any())
            {
                gameBase.Disk = disks
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var medias = item.Read<Models.Metadata.Media[]>(Models.Metadata.Machine.MediaKey);
            if (medias != null && medias.Any())
            {
                gameBase.Media = medias
                    .Where(m => m != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var samples = item.Read<Models.Metadata.Sample[]>(Models.Metadata.Machine.SampleKey);
            if (samples != null && samples.Any())
            {
                gameBase.Sample = samples
                    .Where(m => m != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var archives = item.Read<Models.Metadata.Archive[]>(Models.Metadata.Machine.ArchiveKey);
            if (archives != null && archives.Any())
            {
                gameBase.Archive = archives
                    .Where(m => m != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var chips = item.Read<Models.Metadata.Chip[]>(Models.Metadata.Machine.ChipKey);
            if (chips != null && chips.Any())
            {
                gameBase.Chip = chips
                    .Where(m => m != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var videos = item.Read<Models.Metadata.Video[]>(Models.Metadata.Machine.VideoKey);
            if (videos != null && videos.Any())
            {
                gameBase.Video = videos
                    .Where(m => m != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var sound = item.Read<Models.Metadata.Sound>(Models.Metadata.Machine.SoundKey);
            if (sound != null)
                gameBase.Sound = ConvertFromInternalModel(sound);

            var input = item.Read<Models.Metadata.Input>(Models.Metadata.Machine.InputKey);
            if (input != null)
                gameBase.Input = ConvertFromInternalModel(input);

            var dipSwitches = item.Read<Models.Metadata.DipSwitch[]>(Models.Metadata.Machine.DipSwitchKey);
            if (dipSwitches != null && dipSwitches.Any())
            {
                gameBase.DipSwitch = dipSwitches
                    .Where(m => m != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var driver = item.Read<Models.Metadata.Driver>(Models.Metadata.Machine.DriverKey);
            if (driver != null)
                gameBase.Driver = ConvertFromInternalModel(driver);

            return gameBase;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Archive"/> to <cref="Models.ClrMamePro.Archive"/>
        /// </summary>
        private static Archive ConvertFromInternalModel(Models.Metadata.Archive item)
        {
            var archive = new Archive
            {
                Name = item.ReadString(Models.Metadata.Archive.NameKey),
            };
            return archive;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.BiosSet"/> to <cref="Models.ClrMamePro.BiosSet"/>
        /// </summary>
        private static BiosSet ConvertFromInternalModel(Models.Metadata.BiosSet item)
        {
            var biosset = new BiosSet
            {
                Name = item.ReadString(Models.Metadata.BiosSet.NameKey),
                Description = item.ReadString(Models.Metadata.BiosSet.DescriptionKey),
                Default = item.ReadString(Models.Metadata.BiosSet.DefaultKey),
            };
            return biosset;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Chip"/> to <cref="Models.ClrMamePro.Chip"/>
        /// </summary>
        private static Chip ConvertFromInternalModel(Models.Metadata.Chip item)
        {
            var chip = new Chip
            {
                Type = item.ReadString(Models.Metadata.Chip.ChipTypeKey),
                Name = item.ReadString(Models.Metadata.Chip.NameKey),
                Flags = item.ReadString(Models.Metadata.Chip.FlagsKey),
                Clock = item.ReadString(Models.Metadata.Chip.ClockKey),
            };
            return chip;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.DipSwitch"/> to <cref="Models.ClrMamePro.DipSwitch"/>
        /// </summary>
        private static DipSwitch ConvertFromInternalModel(Models.Metadata.DipSwitch item)
        {
            var dipswitch = new DipSwitch
            {
                Name = item.ReadString(Models.Metadata.DipSwitch.NameKey),
                Entry = item[Models.Metadata.DipSwitch.EntryKey] as string[],
                Default = item.ReadString(Models.Metadata.DipSwitch.DefaultKey),
            };
            return dipswitch;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Disk"/> to <cref="Models.ClrMamePro.Disk"/>
        /// </summary>
        private static Disk ConvertFromInternalModel(Models.Metadata.Disk item)
        {
            var disk = new Disk
            {
                Name = item.ReadString(Models.Metadata.Disk.NameKey),
                MD5 = item.ReadString(Models.Metadata.Disk.MD5Key),
                SHA1 = item.ReadString(Models.Metadata.Disk.SHA1Key),
                Merge = item.ReadString(Models.Metadata.Disk.MergeKey),
                Status = item.ReadString(Models.Metadata.Disk.StatusKey),
                Flags = item.ReadString(Models.Metadata.Disk.FlagsKey),
            };
            return disk;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Driver"/> to <cref="Models.ClrMamePro.Driver"/>
        /// </summary>
        private static Driver ConvertFromInternalModel(Models.Metadata.Driver item)
        {
            var driver = new Driver
            {
                Status = item.ReadString(Models.Metadata.Driver.StatusKey),
                Color = item.ReadString(Models.Metadata.Driver.ColorKey),
                Sound = item.ReadString(Models.Metadata.Driver.SoundKey),
                PaletteSize = item.ReadString(Models.Metadata.Driver.PaletteSizeKey),
                Blit = item.ReadString(Models.Metadata.Driver.BlitKey),
            };
            return driver;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Input"/> to <cref="Models.ClrMamePro.Input"/>
        /// </summary>
        private static Input ConvertFromInternalModel(Models.Metadata.Input item)
        {
            var input = new Input
            {
                Players = item.ReadString(Models.Metadata.Input.PlayersKey),
                Control = item.ReadString(Models.Metadata.Input.ControlKey),
                Buttons = item.ReadString(Models.Metadata.Input.ButtonsKey),
                Coins = item.ReadString(Models.Metadata.Input.CoinsKey),
                Tilt = item.ReadString(Models.Metadata.Input.TiltKey),
                Service = item.ReadString(Models.Metadata.Input.ServiceKey),
            };
            return input;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Media"/> to <cref="Models.ClrMamePro.Media"/>
        /// </summary>
        private static Media ConvertFromInternalModel(Models.Metadata.Media item)
        {
            var media = new Media
            {
                Name = item.ReadString(Models.Metadata.Media.NameKey),
                MD5 = item.ReadString(Models.Metadata.Media.MD5Key),
                SHA1 = item.ReadString(Models.Metadata.Media.SHA1Key),
                SHA256 = item.ReadString(Models.Metadata.Media.SHA256Key),
                SpamSum = item.ReadString(Models.Metadata.Media.SpamSumKey),
            };
            return media;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Release"/> to <cref="Models.ClrMamePro.Release"/>
        /// </summary>
        private static Release ConvertFromInternalModel(Models.Metadata.Release item)
        {
            var release = new Release
            {
                Name = item.ReadString(Models.Metadata.Release.NameKey),
                Region = item.ReadString(Models.Metadata.Release.RegionKey),
                Language = item.ReadString(Models.Metadata.Release.LanguageKey),
                Date = item.ReadString(Models.Metadata.Release.DateKey),
                Default = item.ReadString(Models.Metadata.Release.DefaultKey),
            };
            return release;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Rom"/> to <cref="Models.ClrMamePro.Rom"/>
        /// </summary>
        private static Rom ConvertFromInternalModel(Models.Metadata.Rom item)
        {
            var rom = new Rom
            {
                Name = item.ReadString(Models.Metadata.Rom.NameKey),
                Size = item.ReadString(Models.Metadata.Rom.SizeKey),
                CRC = item.ReadString(Models.Metadata.Rom.CRCKey),
                MD5 = item.ReadString(Models.Metadata.Rom.MD5Key),
                SHA1 = item.ReadString(Models.Metadata.Rom.SHA1Key),
                SHA256 = item.ReadString(Models.Metadata.Rom.SHA256Key),
                SHA384 = item.ReadString(Models.Metadata.Rom.SHA384Key),
                SHA512 = item.ReadString(Models.Metadata.Rom.SHA512Key),
                SpamSum = item.ReadString(Models.Metadata.Rom.SpamSumKey),
                xxHash364 = item.ReadString(Models.Metadata.Rom.xxHash364Key),
                xxHash3128 = item.ReadString(Models.Metadata.Rom.xxHash3128Key),
                Merge = item.ReadString(Models.Metadata.Rom.MergeKey),
                Status = item.ReadString(Models.Metadata.Rom.StatusKey),
                Region = item.ReadString(Models.Metadata.Rom.RegionKey),
                Flags = item.ReadString(Models.Metadata.Rom.FlagsKey),
                Offs = item.ReadString(Models.Metadata.Rom.OffsetKey),
                Serial = item.ReadString(Models.Metadata.Rom.SerialKey),
                Header = item.ReadString(Models.Metadata.Rom.HeaderKey),
                Date = item.ReadString(Models.Metadata.Rom.DateKey),
                Inverted = item.ReadString(Models.Metadata.Rom.InvertedKey),
                MIA = item.ReadString(Models.Metadata.Rom.MIAKey),
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Sample"/> to <cref="Models.ClrMamePro.Sample"/>
        /// </summary>
        private static Sample ConvertFromInternalModel(Models.Metadata.Sample item)
        {
            var sample = new Sample
            {
                Name = item.ReadString(Models.Metadata.Sample.NameKey),
            };
            return sample;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Sound"/> to <cref="Models.ClrMamePro.Sound"/>
        /// </summary>
        private static Sound ConvertFromInternalModel(Models.Metadata.Sound item)
        {
            var sound = new Sound
            {
                Channels = item.ReadString(Models.Metadata.Sound.ChannelsKey),
            };
            return sound;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Video"/> to <cref="Models.ClrMamePro.Video"/>
        /// </summary>
        private static Video ConvertFromInternalModel(Models.Metadata.Video item)
        {
            var video = new Video
            {
                Screen = item.ReadString(Models.Metadata.Video.ScreenKey),
                Orientation = item.ReadString(Models.Metadata.Video.OrientationKey),
                X = item.ReadString(Models.Metadata.Video.WidthKey),
                Y = item.ReadString(Models.Metadata.Video.HeightKey),
                AspectX = item.ReadString(Models.Metadata.Video.AspectXKey),
                AspectY = item.ReadString(Models.Metadata.Video.AspectYKey),
                Freq = item.ReadString(Models.Metadata.Video.RefreshKey),
            };
            return video;
        }

        #endregion
    }
}