using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SabreTools.IO.Readers;
using SabreTools.IO.Writers;
using SabreTools.Models.ClrMamePro;
namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for ClrMamePro metadata files
    /// </summary>
    public class ClrMamePro
    {
        #region Deserialization

        /// <summary>
        /// Deserializes a ClrMamePro metadata file to the defined type
        /// </summary>
        /// <param name="path">Path to the file to deserialize</param>
        /// <param name="quotes">Enable quotes on read, false otherwise</param>
        /// <returns>Deserialized data on success, null on failure</returns>
        public static MetadataFile? Deserialize(string path, bool quotes)
        {
            try
            {
                using var stream = PathProcessor.OpenStream(path);
                return Deserialize(stream, quotes);
            }
            catch
            {
                // TODO: Handle logging the exception
                return default;
            }
        }

        /// <summary>
        /// Deserializes a ClrMamePro metadata file in a stream to the defined type
        /// </summary>
        /// <param name="stream">Stream to deserialize</param>
        /// <param name="quotes">Enable quotes on read, false otherwise</param>
        /// <returns>Deserialized data on success, null on failure</returns>
        public static MetadataFile? Deserialize(Stream? stream, bool quotes)
        {
            try
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
                                game.Video = CreateVideo(reader);
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
            catch
            {
                // TODO: Handle logging the exception
                return default;
            }
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

        #endregion

        #region Serialization

        /// <summary>
        /// Serializes the defined type to a ClrMamePro metadata file
        /// </summary>
        /// <param name="metadataFile">Data to serialize</param>
        /// <param name="path">Path to the file to serialize to</param>
        /// <param name="quotes">Enable quotes on write, false otherwise</param>
        /// <returns>True on successful serialization, false otherwise</returns>
        public static bool SerializeToFile(MetadataFile? metadataFile, string path, bool quotes)
        {
            try
            {
                using var stream = SerializeToStream(metadataFile, quotes);
                if (stream == null)
                    return false;

                using var fs = File.OpenWrite(path);
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fs);
                return true;
            }
            catch
            {
                // TODO: Handle logging the exception
                return false;
            }
        }

        /// <summary>
        /// Serializes the defined type to a stream
        /// </summary>
        /// <param name="metadataFile">Data to serialize</param>
        /// <param name="quotes">Enable quotes on write, false otherwise</param>
        /// <returns>Stream containing serialized data on success, null otherwise</returns>
        public static Stream? SerializeToStream(MetadataFile? metadataFile, bool quotes)
        {
            try
            {
                // If the metadata file is null
                if (metadataFile == null)
                    return null;

                // Setup the writer and output
                var stream = new MemoryStream();
                var writer = new ClrMameProWriter(stream, Encoding.UTF8) { Quotes = quotes };

                // Write the header, if it exists
                WriteHeader(metadataFile.ClrMamePro, writer);

                // Write out the games, if they exist
                WriteGames(metadataFile.Game, writer);

                // Return the stream
                return stream;
            }
            catch
            {
                // TODO: Handle logging the exception
                return null;
            }
        }

        /// <summary>
        /// Write header information to the current writer
        /// </summary>
        /// <param name="header">ClrMamePro representing the header information</param>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        private static void WriteHeader(Models.ClrMamePro.ClrMamePro? header, ClrMameProWriter writer)
        {
            // If the header information is missing, we can't do anything
            if (header == null)
                return;

            writer.WriteStartElement("clrmamepro");

            writer.WriteOptionalStandalone("name", header.Name);
            writer.WriteOptionalStandalone("description", header.Description);
            writer.WriteOptionalStandalone("rootdir", header.RootDir);
            writer.WriteOptionalStandalone("category", header.Category);
            writer.WriteOptionalStandalone("version", header.Version);
            writer.WriteOptionalStandalone("date", header.Date);
            writer.WriteOptionalStandalone("author", header.Author);
            writer.WriteOptionalStandalone("homepage", header.Homepage);
            writer.WriteOptionalStandalone("url", header.Url);
            writer.WriteOptionalStandalone("comment", header.Comment);
            writer.WriteOptionalStandalone("header", header.Header);
            writer.WriteOptionalStandalone("type", header.Type);
            writer.WriteOptionalStandalone("forcemerging", header.ForceMerging);
            writer.WriteOptionalStandalone("forcezipping", header.ForceZipping);
            writer.WriteOptionalStandalone("forcepacking", header.ForcePacking);

            writer.WriteEndElement(); // clrmamepro
            writer.Flush();
        }

        /// <summary>
        /// Write games information to the current writer
        /// </summary>
        /// <param name="games">Array of GameBase objects representing the games information</param>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        private static void WriteGames(GameBase[]? games, ClrMameProWriter writer)
        {
            // If the games information is missing, we can't do anything
            if (games == null || !games.Any())
                return;

            // Loop through and write out the games
            foreach (var game in games)
            {
                WriteGame(game, writer);
                writer.Flush();
            }
        }

        /// <summary>
        /// Write game information to the current writer
        /// </summary>
        /// <param name="game">GameBase object representing the game information</param>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        private static void WriteGame(GameBase game, ClrMameProWriter writer)
        {
            // If the game information is missing, we can't do anything
            if (game == null)
                return;

            switch (game)
            {
                case Game:
                    writer.WriteStartElement("game");
                    break;
                case Machine:
                    writer.WriteStartElement("machine");
                    break;
                case Resource:
                    writer.WriteStartElement("resource");
                    break;
                case Set:
                    writer.WriteStartElement(name: "set");
                    break;
            }

            // Write the standalone values
            writer.WriteRequiredStandalone("name", game.Name, throwOnError: true);
            writer.WriteOptionalStandalone("description", game.Description);
            writer.WriteOptionalStandalone("year", game.Year);
            writer.WriteOptionalStandalone("manufacturer", game.Manufacturer);
            writer.WriteOptionalStandalone("category", game.Category);
            writer.WriteOptionalStandalone("cloneof", game.CloneOf);
            writer.WriteOptionalStandalone("romof", game.RomOf);
            writer.WriteOptionalStandalone("sampleof", game.SampleOf);

            // Write the item values
            WriteReleases(game.Release, writer);
            WriteBiosSets(game.BiosSet, writer);
            WriteRoms(game.Rom, writer);
            WriteDisks(game.Disk, writer);
            WriteMedia(game.Media, writer);
            WriteSamples(game.Sample, writer);
            WriteArchives(game.Archive, writer);
            WriteChips(game.Chip, writer);
            WriteVideo(game.Video, writer);
            WriteSound(game.Sound, writer);
            WriteInput(game.Input, writer);
            WriteDipSwitches(game.DipSwitch, writer);
            WriteDriver(game.Driver, writer);

            writer.WriteEndElement(); // game, machine, resource, set
        }

        /// <summary>
        /// Write releases information to the current writer
        /// </summary>
        /// <param name="releases">Array of Release objects to write</param>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        private static void WriteReleases(Release[]? releases, ClrMameProWriter writer)
        {
            // If the array is missing, we can't do anything
            if (releases == null)
                return;

            foreach (var release in releases)
            {
                writer.WriteStartElement("release");
                writer.WriteRequiredAttributeString("name", release.Name, throwOnError: true);
                writer.WriteRequiredAttributeString("region", release.Region, throwOnError: true);
                writer.WriteOptionalAttributeString("language", release.Language);
                writer.WriteOptionalAttributeString("date", release.Date);
                writer.WriteOptionalAttributeString("default", release.Default);
                writer.WriteEndElement(); // release
            }
        }

        /// <summary>
        /// Write biossets information to the current writer
        /// </summary>
        /// <param name="biossets">Array of BiosSet objects to write</param>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        private static void WriteBiosSets(BiosSet[]? biossets, ClrMameProWriter writer)
        {
            // If the array is missing, we can't do anything
            if (biossets == null)
                return;

            foreach (var biosset in biossets)
            {
                writer.WriteStartElement("biosset");
                writer.WriteRequiredAttributeString("name", biosset.Name, throwOnError: true);
                writer.WriteRequiredAttributeString("description", biosset.Description, throwOnError: true);
                writer.WriteOptionalAttributeString("default", biosset.Default);
                writer.WriteEndElement(); // release
            }
        }

        /// <summary>
        /// Write roms information to the current writer
        /// </summary>
        /// <param name="roms">Array of Rom objects to write</param>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        private static void WriteRoms(Rom[]? roms, ClrMameProWriter writer)
        {
            // If the array is missing, we can't do anything
            if (roms == null)
                return;

            foreach (var rom in roms)
            {
                writer.WriteStartElement("rom");
                writer.WriteRequiredAttributeString("name", rom.Name, throwOnError: true);
                writer.WriteRequiredAttributeString("size", rom.Size, throwOnError: true);
                writer.WriteOptionalAttributeString("crc", rom.CRC);
                writer.WriteOptionalAttributeString("md5", rom.MD5);
                writer.WriteOptionalAttributeString("sha1", rom.SHA1);
                writer.WriteOptionalAttributeString("sha256", rom.SHA256);
                writer.WriteOptionalAttributeString("sha384", rom.SHA384);
                writer.WriteOptionalAttributeString("sha512", rom.SHA512);
                writer.WriteOptionalAttributeString("spamsum", rom.SpamSum);
                writer.WriteOptionalAttributeString("xxh3_64", rom.xxHash364);
                writer.WriteOptionalAttributeString("xxh3_128", rom.xxHash3128);
                writer.WriteOptionalAttributeString("merge", rom.Merge);
                writer.WriteOptionalAttributeString("status", rom.Status);
                writer.WriteOptionalAttributeString("region", rom.Region);
                writer.WriteOptionalAttributeString("flags", rom.Flags);
                writer.WriteOptionalAttributeString("offs", rom.Offs);
                writer.WriteOptionalAttributeString("serial", rom.Serial);
                writer.WriteOptionalAttributeString("header", rom.Header);
                writer.WriteOptionalAttributeString("date", rom.Date);
                writer.WriteOptionalAttributeString("inverted", rom.Inverted);
                writer.WriteOptionalAttributeString("mia", rom.MIA);
                writer.WriteEndElement(); // rom
            }
        }

        /// <summary>
        /// Write disks information to the current writer
        /// </summary>
        /// <param name="disks">Array of Disk objects to write</param>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        private static void WriteDisks(Disk[]? disks, ClrMameProWriter writer)
        {
            // If the array is missing, we can't do anything
            if (disks == null)
                return;

            foreach (var disk in disks)
            {
                writer.WriteStartElement("disk");
                writer.WriteRequiredAttributeString("name", disk.Name, throwOnError: true);
                writer.WriteOptionalAttributeString("md5", disk.MD5);
                writer.WriteOptionalAttributeString("sha1", disk.SHA1);
                writer.WriteOptionalAttributeString("merge", disk.Merge);
                writer.WriteOptionalAttributeString("status", disk.Status);
                writer.WriteOptionalAttributeString("flags", disk.Flags);
                writer.WriteEndElement(); // disk
            }
        }

        /// <summary>
        /// Write medias information to the current writer
        /// </summary>
        /// <param name="medias">Array of Media objects to write</param>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        private static void WriteMedia(Media[]? medias, ClrMameProWriter writer)
        {
            // If the array is missing, we can't do anything
            if (medias == null)
                return;

            foreach (var media in medias)
            {
                writer.WriteStartElement("media");
                writer.WriteRequiredAttributeString("name", media.Name, throwOnError: true);
                writer.WriteOptionalAttributeString("md5", media.MD5);
                writer.WriteOptionalAttributeString("sha1", media.SHA1);
                writer.WriteOptionalAttributeString("sha256", media.SHA256);
                writer.WriteOptionalAttributeString("spamsum", media.SpamSum);
                writer.WriteEndElement(); // media
            }
        }

        /// <summary>
        /// Write samples information to the current writer
        /// </summary>
        /// <param name="samples">Array of Sample objects to write</param>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        private static void WriteSamples(Sample[]? samples, ClrMameProWriter writer)
        {
            // If the array is missing, we can't do anything
            if (samples == null)
                return;

            foreach (var sample in samples)
            {
                writer.WriteStartElement("sample");
                writer.WriteRequiredAttributeString("name", sample.Name, throwOnError: true);
                writer.WriteEndElement(); // sample
            }
        }

        /// <summary>
        /// Write archives information to the current writer
        /// </summary>
        /// <param name="archives">Array of Archive objects to write</param>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        private static void WriteArchives(Archive[]? archives, ClrMameProWriter writer)
        {
            // If the array is missing, we can't do anything
            if (archives == null)
                return;

            foreach (var archive in archives)
            {
                writer.WriteStartElement("archive");
                writer.WriteRequiredAttributeString("name", archive.Name, throwOnError: true);
                writer.WriteEndElement(); // archive
            }
        }

        /// <summary>
        /// Write chips information to the current writer
        /// </summary>
        /// <param name="chips">Array of Chip objects to write</param>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        private static void WriteChips(Chip[]? chips, ClrMameProWriter writer)
        {
            // If the array is missing, we can't do anything
            if (chips == null)
                return;

            foreach (var chip in chips)
            {
                writer.WriteStartElement("chip");
                writer.WriteRequiredAttributeString("type", chip.Type, throwOnError: true);
                writer.WriteRequiredAttributeString("name", chip.Name, throwOnError: true);
                writer.WriteOptionalAttributeString("flags", chip.Flags);
                writer.WriteOptionalAttributeString("clock", chip.Clock);
                writer.WriteEndElement(); // chip
            }
        }

        /// <summary>
        /// Write video information to the current writer
        /// </summary>
        /// <param name="video">Video object to write</param>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        private static void WriteVideo(Video? video, ClrMameProWriter writer)
        {
            // If the item is missing, we can't do anything
            if (video == null)
                return;

            writer.WriteStartElement("video");
            writer.WriteRequiredAttributeString("screen", video.Screen, throwOnError: true);
            writer.WriteRequiredAttributeString("orientation", video.Orientation, throwOnError: true);
            writer.WriteOptionalAttributeString("x", video.X);
            writer.WriteOptionalAttributeString("y", video.Y);
            writer.WriteOptionalAttributeString("aspectx", video.AspectX);
            writer.WriteOptionalAttributeString("aspecty", video.AspectY);
            writer.WriteOptionalAttributeString("freq", video.Freq);
            writer.WriteEndElement(); // video
        }

        /// <summary>
        /// Write sound information to the current writer
        /// </summary>
        /// <param name="sound">Sound object to write</param>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        private static void WriteSound(Sound? sound, ClrMameProWriter writer)
        {
            // If the item is missing, we can't do anything
            if (sound == null)
                return;

            writer.WriteStartElement("sound");
            writer.WriteRequiredAttributeString("channels", sound.Channels, throwOnError: true);
            writer.WriteEndElement(); // sound
        }

        /// <summary>
        /// Write input information to the current writer
        /// </summary>
        /// <param name="input">Input object to write</param>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        private static void WriteInput(Input? input, ClrMameProWriter writer)
        {
            // If the item is missing, we can't do anything
            if (input == null)
                return;

            writer.WriteStartElement("input");
            writer.WriteRequiredAttributeString("players", input.Players, throwOnError: true);
            writer.WriteOptionalAttributeString("control", input.Control);
            writer.WriteRequiredAttributeString("buttons", input.Buttons, throwOnError: true);
            writer.WriteOptionalAttributeString("coins", input.Coins);
            writer.WriteOptionalAttributeString("tilt", input.Tilt);
            writer.WriteOptionalAttributeString("service", input.Service);
            writer.WriteEndElement(); // input
        }

        /// <summary>
        /// Write dipswitches information to the current writer
        /// </summary>
        /// <param name="dipswitches">Array of DipSwitch objects to write</param>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        private static void WriteDipSwitches(DipSwitch[]? dipswitches, ClrMameProWriter writer)
        {
            // If the array is missing, we can't do anything
            if (dipswitches == null)
                return;

            foreach (var dipswitch in dipswitches)
            {
                writer.WriteStartElement("dipswitch");
                writer.WriteRequiredAttributeString("name", dipswitch.Name, throwOnError: true);
                foreach (var entry in dipswitch.Entry ?? Array.Empty<string>())
                {
                    writer.WriteRequiredAttributeString("entry", entry);
                }
                writer.WriteOptionalAttributeString("default", dipswitch.Default);
                writer.WriteEndElement(); // dipswitch
            }
        }

        /// <summary>
        /// Write driver information to the current writer
        /// </summary>
        /// <param name="driver">Driver object to write</param>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        private static void WriteDriver(Driver? driver, ClrMameProWriter writer)
        {
            // If the item is missing, we can't do anything
            if (driver == null)
                return;

            writer.WriteStartElement("driver");
            writer.WriteRequiredAttributeString("status", driver.Status, throwOnError: true);
            writer.WriteOptionalAttributeString("color", driver.Color); // TODO: Probably actually required
            writer.WriteOptionalAttributeString("sound", driver.Sound); // TODO: Probably actually required
            writer.WriteOptionalAttributeString("palettesize", driver.PaletteSize);
            writer.WriteOptionalAttributeString("blit", driver.Blit);
            writer.WriteEndElement(); // driver
        }

        #endregion
    }
}