using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.IO.Readers;
using SabreTools.Models.DosCenter;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for DosCenter metadata files
    /// </summary>
    public class DosCenter
    {
        /// <summary>
        /// Deserializes a DosCenter metadata file to the defined type
        /// </summary>
        /// <param name="path">Path to the file to deserialize</param>
        /// <returns>Deserialized data on success, null on failure</returns>
        public static DatFile? Deserialize(string path)
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
        /// Deserializes a DosCenter metadata file in a stream to the defined type
        /// </summary>
        /// <param name="stream">Stream to deserialize</param>
        /// <returns>Deserialized data on success, null on failure</returns>
        public static DatFile? Deserialize(Stream? stream)
        {
            try
            {
                // If the stream is null
                if (stream == null)
                    return default;

                // Setup the reader and output
                var reader = new ClrMameProReader(stream, Encoding.UTF8) { DosCenter = true };
                var dat = new DatFile();

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
                        // Create the block
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
                                    fileAdditional.Add(item: reader.CurrentLine);
                                    break;
                            }
                        }

                        // Add the file to the list
                        file.ADDITIONAL_ELEMENTS = fileAdditional.ToArray();
                        files.Add(file);
                        fileAdditional.Clear();
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