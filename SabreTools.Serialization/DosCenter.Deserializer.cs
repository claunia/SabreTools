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
            string lastTopLevel = reader.TopLevel;

            Game? game = null;
            var games = new List<Game?>();
            var files = new List<Models.DosCenter.File?>();

            var additional = new List<string>();
            var headerAdditional = new List<string>();
            var gameAdditional = new List<string>();
            var fileAdditional = new List<string>();
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
                                dat.DosCenter.ADDITIONAL_ELEMENTS = headerAdditional.ToArray();
                                headerAdditional.Clear();
                                break;
                            case "game":
                                game.File = files.ToArray();
                                game.ADDITIONAL_ELEMENTS = gameAdditional.ToArray();
                                games.Add(game);

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
                        gameAdditional.Add(reader.CurrentLine);
                        continue;
                    }

                    // Create the file and add to the list
                    var file = CreateFile(reader);
                    files.Add(file);
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

        /// <summary>
        /// Create a File object from the current reader context
        /// </summary>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        /// <returns>File object created from the reader context</returns>
        private static Models.DosCenter.File CreateFile(ClrMameProReader reader)
        {
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
                        itemAdditional.Add(item: reader.CurrentLine);
                        break;
                }
            }

            file.ADDITIONAL_ELEMENTS = itemAdditional.ToArray();
            return file;
        }

        #region Internal

        /// <summary>
        /// Convert from <cref="Models.Internal.MetadataFile"/> to <cref="Models.DosCenter.MetadataFile"/>
        /// </summary>
        public static MetadataFile? ConvertFromInternalModel(Models.Internal.MetadataFile? item)
        {
            if (item == null)
                return null;

            var metadataFile = new MetadataFile();

            var header = item.Read<Models.Internal.Header>(Models.Internal.MetadataFile.HeaderKey);
            if (header != null)
                metadataFile.DosCenter = ConvertHeaderFromInternalModel(header);

            var machines = item.Read<Models.Internal.Machine[]>(Models.Internal.MetadataFile.MachineKey);
            if (machines != null && machines.Any())
                metadataFile.Game = machines.Select(ConvertMachineFromInternalModel).ToArray();

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Header"/> to <cref="Models.DosCenter.DosCenter"/>
        /// </summary>
        private static Models.DosCenter.DosCenter? ConvertHeaderFromInternalModel(Models.Internal.Header? item)
        {
            if (item == null)
                return null;

            var dosCenter = new Models.DosCenter.DosCenter
            {
                Name = item.ReadString(Models.Internal.Header.NameKey),
                Description = item.ReadString(Models.Internal.Header.DescriptionKey),
                Version = item.ReadString(Models.Internal.Header.VersionKey),
                Date = item.ReadString(Models.Internal.Header.DateKey),
                Author = item.ReadString(Models.Internal.Header.AuthorKey),
                Homepage = item.ReadString(Models.Internal.Header.HomepageKey),
                Comment = item.ReadString(Models.Internal.Header.CommentKey),
            };
            return dosCenter;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Machine"/> to <cref="Models.DosCenter.Game"/>
        /// </summary>
        private static Game? ConvertMachineFromInternalModel(Models.Internal.Machine? item)
        {
            if (item == null)
                return null;

            var game = new Game
            {
                Name = item.ReadString(Models.Internal.Machine.NameKey),
            };

            var roms = item.Read<Models.Internal.Rom[]>(Models.Internal.Machine.RomKey);
            game.File = roms?.Select(ConvertFromInternalModel)?.ToArray();

            return game;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.DosCenter.File"/>
        /// </summary>
        private static Models.DosCenter.File? ConvertFromInternalModel(Models.Internal.Rom? item)
        {
            if (item == null)
                return null;

            var file = new Models.DosCenter.File
            {
                Name = item.ReadString(Models.Internal.Rom.NameKey),
                Size = item.ReadString(Models.Internal.Rom.SizeKey),
                CRC = item.ReadString(Models.Internal.Rom.CRCKey),
                Date = item.ReadString(Models.Internal.Rom.DateKey),
            };
            return file;
        }

        #endregion
    }
}