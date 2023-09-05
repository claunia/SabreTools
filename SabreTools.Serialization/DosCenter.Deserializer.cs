using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SabreTools.IO.Readers;
using SabreTools.Models.DosCenter;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Deserializer for DosCenter metadata files
    /// </summary>
    public partial class DosCenter
    {
        /// <summary>
        /// Deserializes a DosCenter metadata file to the defined type
        /// </summary>
        /// <param name="path">Path to the file to deserialize</param>
        /// <returns>Deserialized data on success, null on failure</returns>
        public static MetadataFile? Deserialize(string path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Deserialize(stream);
        }

        /// <summary>
        /// Deserializes a DosCenter metadata file in a stream to the defined type
        /// </summary>
        /// <param name="stream">Stream to deserialize</param>
        /// <returns>Deserialized data on success, null on failure</returns>
        public static MetadataFile? Deserialize(Stream? stream)
        {
            // If the stream is null
            if (stream == null)
                return default;

            // Setup the reader and output
            var reader = new ClrMameProReader(stream, Encoding.UTF8) { DosCenter = true };
            var dat = new MetadataFile();

            // Loop through and parse out the values
            string? lastTopLevel = reader.TopLevel;

            Game? game = null;
            var games = new List<Game>();
            var files = new List<Models.DosCenter.File>();

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
                                if (dat.DosCenter != null)
                                    dat.DosCenter.ADDITIONAL_ELEMENTS = headerAdditional.ToArray();

                                headerAdditional.Clear();
                                break;
                            case "game":
                                if (game != null)
                                {
                                    game.File = files.ToArray();
                                    game.ADDITIONAL_ELEMENTS = gameAdditional.ToArray();
                                    games.Add(game);
                                }

                                game = null;
                                files.Clear();
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
                        case "doscenter":
                            dat.DosCenter = new Models.DosCenter.DosCenter();
                            break;
                        case "game":
                            game = new Game();
                            break;
                        default:
                            if (reader.CurrentLine != null)
                                additional.Add(reader.CurrentLine);
                            break;
                    }
                }

                // If we're in the doscenter block
                else if (reader.TopLevel == "doscenter" && reader.RowType == CmpRowType.Standalone)
                {
                    // Create the block if we haven't already
                    dat.DosCenter ??= new Models.DosCenter.DosCenter();

                    switch (reader.Standalone?.Key?.ToLowerInvariant())
                    {
                        case "name:":
                            dat.DosCenter.Name = reader.Standalone?.Value;
                            break;
                        case "description:":
                            dat.DosCenter.Description = reader.Standalone?.Value;
                            break;
                        case "version:":
                            dat.DosCenter.Version = reader.Standalone?.Value;
                            break;
                        case "date:":
                            dat.DosCenter.Date = reader.Standalone?.Value;
                            break;
                        case "author:":
                            dat.DosCenter.Author = reader.Standalone?.Value;
                            break;
                        case "homepage:":
                            dat.DosCenter.Homepage = reader.Standalone?.Value;
                            break;
                        case "comment:":
                            dat.DosCenter.Comment = reader.Standalone?.Value;
                            break;
                        default:
                            if (reader.CurrentLine != null)
                                headerAdditional.Add(item: reader.CurrentLine);
                            break;
                    }
                }

                // If we're in a game block
                else if (reader.TopLevel == "game" && reader.RowType == CmpRowType.Standalone)
                {
                    // Create the block if we haven't already
                    game ??= new Game();

                    switch (reader.Standalone?.Key?.ToLowerInvariant())
                    {
                        case "name":
                            game.Name = reader.Standalone?.Value;
                            break;
                        default:
                            if (reader.CurrentLine != null)
                                gameAdditional.Add(item: reader.CurrentLine);
                            break;
                    }
                }

                // If we're in a file block
                else if (reader.TopLevel == "game" && reader.RowType == CmpRowType.Internal)
                {
                    // If we have an unknown type, log it
                    if (reader.InternalName != "file")
                    {
                        if (reader.CurrentLine != null)
                            gameAdditional.Add(reader.CurrentLine);
                        continue;
                    }

                    // Create the file and add to the list
                    var file = CreateFile(reader);
                    if (file != null)
                        files.Add(file);
                }

                else
                {
                    if (reader.CurrentLine != null)
                        additional.Add(item: reader.CurrentLine);
                }
            }

            // Add extra pieces and return
            dat.Game = games.ToArray();
            dat.ADDITIONAL_ELEMENTS = additional.ToArray();
            return dat;
        }

        /// <summary>
        /// Create a File object from the current reader context
        /// </summary>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        /// <returns>File object created from the reader context</returns>
        private static Models.DosCenter.File? CreateFile(ClrMameProReader reader)
        {
            if (reader.Internal == null)
                return null;

            var itemAdditional = new List<string>();
            var file = new Models.DosCenter.File();
            foreach (var kvp in reader.Internal)
            {
                switch (kvp.Key?.ToLowerInvariant())
                {
                    case "name":
                        file.Name = kvp.Value;
                        break;
                    case "size":
                        file.Size = kvp.Value;
                        break;
                    case "crc":
                        file.CRC = kvp.Value;
                        break;
                    case "date":
                        file.Date = kvp.Value;
                        break;
                    default:
                        if (reader.CurrentLine != null)
                            itemAdditional.Add(item: reader.CurrentLine);
                        break;
                }
            }

            file.ADDITIONAL_ELEMENTS = itemAdditional.ToArray();
            return file;
        }

        #region Internal

        /// <summary>
        /// Convert from <cref="Models.Metadata.MetadataFile"/> to <cref="Models.DosCenter.MetadataFile"/>
        /// </summary>
        public static MetadataFile? ConvertFromInternalModel(Models.Metadata.MetadataFile? item)
        {
            if (item == null)
                return null;

            var metadataFile = new MetadataFile();

            var header = item.Read<Models.Metadata.Header>(Models.Metadata.MetadataFile.HeaderKey);
            if (header != null)
                metadataFile.DosCenter = ConvertHeaderFromInternalModel(header);

            var machines = item.Read<Models.Metadata.Machine[]>(Models.Metadata.MetadataFile.MachineKey);
            if (machines != null && machines.Any())
            {
                metadataFile.Game = machines
                    .Where(m => m != null)
                    .Select(ConvertMachineFromInternalModel)
                    .ToArray();
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Header"/> to <cref="Models.DosCenter.DosCenter"/>
        /// </summary>
        private static Models.DosCenter.DosCenter ConvertHeaderFromInternalModel(Models.Metadata.Header item)
        {
            var dosCenter = new Models.DosCenter.DosCenter
            {
                Name = item.ReadString(Models.Metadata.Header.NameKey),
                Description = item.ReadString(Models.Metadata.Header.DescriptionKey),
                Version = item.ReadString(Models.Metadata.Header.VersionKey),
                Date = item.ReadString(Models.Metadata.Header.DateKey),
                Author = item.ReadString(Models.Metadata.Header.AuthorKey),
                Homepage = item.ReadString(Models.Metadata.Header.HomepageKey),
                Comment = item.ReadString(Models.Metadata.Header.CommentKey),
            };
            return dosCenter;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Machine"/> to <cref="Models.DosCenter.Game"/>
        /// </summary>
        private static Game ConvertMachineFromInternalModel(Models.Metadata.Machine item)
        {
            var game = new Game
            {
                Name = item.ReadString(Models.Metadata.Machine.NameKey),
            };

            var roms = item.Read<Models.Metadata.Rom[]>(Models.Metadata.Machine.RomKey);
            if (roms != null && roms.Any())
            {
                game.File = roms
                    .Where(r => r != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            return game;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Rom"/> to <cref="Models.DosCenter.File"/>
        /// </summary>
        private static Models.DosCenter.File ConvertFromInternalModel(Models.Metadata.Rom item)
        {
            var file = new Models.DosCenter.File
            {
                Name = item.ReadString(Models.Metadata.Rom.NameKey),
                Size = item.ReadString(Models.Metadata.Rom.SizeKey),
                CRC = item.ReadString(Models.Metadata.Rom.CRCKey),
                Date = item.ReadString(Models.Metadata.Rom.DateKey),
            };
            return file;
        }

        #endregion
    }
}