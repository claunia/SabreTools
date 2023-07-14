using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.IO.Readers;
using SabreTools.Models.ClrMamePro;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for ClrMamePro metadata files
    /// </summary>
    public class ClrMamePro
    {
        /// <summary>
        /// Deserializes a ClrMamePro metadata file to the defined type
        /// </summary>
        /// <param name="path">Path to the file to deserialize</param>
        /// <returns>Deserialized data on success, null on failure</returns>
        public static MetadataFile? Deserialize(string path)
        {
            try
            {
                using var stream = PathProcessor.OpenStream(path);
                return Deserialize(stream);
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
        /// <returns>Deserialized data on success, null on failure</returns>
        public static MetadataFile? Deserialize(Stream? stream)
        {
            try
            {
                // If the stream is null
                if (stream == null)
                    return default;

                // Setup the reader and output
                var reader = new ClrMameProReader(stream, Encoding.UTF8);
                var dat = new MetadataFile();

                // Loop through and parse out the values
                string lastTopLevel = reader.TopLevel;

                GameBase? game = null;
                var games = new List<GameBase?>();
                var items = new List<ItemBase?>();

                var additional = new List<string>();
                var headerAdditional = new List<string>();
                var gameAdditional = new List<string>();
                var itemAdditional = new List<string>();
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
                                    dat.ClrMamePro.ADDITIONAL_ELEMENTS = headerAdditional.ToArray();
                                    headerAdditional.Clear();
                                    break;
                                case "game":
                                case "machine":
                                case "resource":
                                case "set":
                                    game.Item = items.ToArray();
                                    game.ADDITIONAL_ELEMENTS = gameAdditional.ToArray();
                                    games.Add(game);
                                    game = null;
                                    items.Clear();
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
                    else if (reader.TopLevel == "clrmamepro" && reader.RowType == CmpRowType.Standalone)
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
                                headerAdditional.Add(item: reader.CurrentLine);
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
                            _ => null,
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
                            default:
                                gameAdditional.Add(item: reader.CurrentLine);
                                break;
                        }
                    }

                    // If we're in an item block
                    else if (reader.TopLevel == "game" && reader.RowType == CmpRowType.Internal)
                    {
                        // Create the block
                        switch (reader.InternalName)
                        {
                            case "release":
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
                                            itemAdditional.Add(item: reader.CurrentLine);
                                            break;
                                    }
                                }

                                // Add the release to the list
                                release.ADDITIONAL_ELEMENTS = itemAdditional.ToArray();
                                items.Add(release);
                                itemAdditional.Clear();
                                break;

                            case "biosset":
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
                                            itemAdditional.Add(item: reader.CurrentLine);
                                            break;
                                    }
                                }

                                // Add the biosset to the list
                                biosset.ADDITIONAL_ELEMENTS = itemAdditional.ToArray();
                                items.Add(biosset);
                                itemAdditional.Clear();
                                break;

                            case "rom":
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
                                        case "flags":
                                            rom.Flags = kvp.Value;
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
                                            itemAdditional.Add(item: reader.CurrentLine);
                                            break;
                                    }
                                }

                                // Add the rom to the list
                                rom.ADDITIONAL_ELEMENTS = itemAdditional.ToArray();
                                items.Add(rom);
                                itemAdditional.Clear();
                                break;
                                
                            case "disk":
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
                                            itemAdditional.Add(item: reader.CurrentLine);
                                            break;
                                    }
                                }

                                // Add the disk to the list
                                disk.ADDITIONAL_ELEMENTS = itemAdditional.ToArray();
                                items.Add(disk);
                                itemAdditional.Clear();
                                break;
                                
                            case "media":
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
                                            itemAdditional.Add(item: reader.CurrentLine);
                                            break;
                                    }
                                }

                                // Add the media to the list
                                media.ADDITIONAL_ELEMENTS = itemAdditional.ToArray();
                                items.Add(media);
                                itemAdditional.Clear();
                                break;
                                
                            case "sample":
                                var sample = new Sample();
                                foreach (var kvp in reader.Internal)
                                {
                                    switch (kvp.Key?.ToLowerInvariant())
                                    {
                                        case "name":
                                            sample.Name = kvp.Value;
                                            break;
                                        default:
                                            itemAdditional.Add(item: reader.CurrentLine);
                                            break;
                                    }
                                }

                                // Add the sample to the list
                                sample.ADDITIONAL_ELEMENTS = itemAdditional.ToArray();
                                items.Add(sample);
                                itemAdditional.Clear();
                                break;
                                
                            case "archive":
                                var archive = new Archive();
                                foreach (var kvp in reader.Internal)
                                {
                                    switch (kvp.Key?.ToLowerInvariant())
                                    {
                                        case "name":
                                            archive.Name = kvp.Value;
                                            break;
                                        default:
                                            itemAdditional.Add(item: reader.CurrentLine);
                                            break;
                                    }
                                }

                                // Add the archive to the list
                                archive.ADDITIONAL_ELEMENTS = itemAdditional.ToArray();
                                items.Add(archive);
                                itemAdditional.Clear();
                                break;
                                
                            default:
                                gameAdditional.Add(reader.CurrentLine);
                                continue;
                        }
                    }

                    else
                    {
                        additional.Add(item: reader.CurrentLine);
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
    }
}