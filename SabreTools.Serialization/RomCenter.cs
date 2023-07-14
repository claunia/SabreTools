using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SabreTools.IO.Readers;
using SabreTools.Models.RomCenter;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for RomCenter INI files
    /// </summary>
    public class RomCenter
    {
        /// <summary>
        /// Deserializes a RomCenter INI file to the defined type
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
        /// Deserializes a RomCenter INI file in a stream to the defined type
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
                var reader = new IniReader(stream, Encoding.UTF8)
                {
                    ValidateRows = false,
                };
                var dat = new MetadataFile();

                // Loop through and parse out the values
                var roms = new List<Rom>();
                var additional = new List<string>();
                var creditsAdditional = new List<string>();
                var datAdditional = new List<string>();
                var emulatorAdditional = new List<string>();
                var gamesAdditional = new List<string>();
                while (!reader.EndOfStream)
                {
                    // If we have no next line
                    if (!reader.ReadNextLine())
                        break;

                    // Ignore certain row types
                    switch (reader.RowType)
                    {
                        case IniRowType.None:
                        case IniRowType.Comment:
                            continue;
                        case IniRowType.SectionHeader:
                            switch (reader.Section.ToLowerInvariant())
                            {
                                case "credits":
                                    dat.Credits ??= new Credits();
                                    break;
                                case "dat":
                                    dat.Dat ??= new Dat();
                                    break;
                                case "emulator":
                                    dat.Emulator ??= new Emulator();
                                    break;
                                case "games":
                                    dat.Games ??= new Games();
                                    break;
                                default:
                                    additional.Add(reader.CurrentLine);
                                    break;
                            }
                            continue;
                    }

                    // If we're in credits
                    if (reader.Section.ToLowerInvariant() == "credits")
                    {
                        // Create the section if we haven't already
                        dat.Credits ??= new Credits();

                        switch (reader.KeyValuePair?.Key?.ToLowerInvariant())
                        {
                            case "author":
                                dat.Credits.Author = reader.KeyValuePair?.Value;
                                break;
                            case "version":
                                dat.Credits.Version = reader.KeyValuePair?.Value;
                                break;
                            case "email":
                                dat.Credits.Email = reader.KeyValuePair?.Value;
                                break;
                            case "homepage":
                                dat.Credits.Homepage = reader.KeyValuePair?.Value;
                                break;
                            case "url":
                                dat.Credits.Url = reader.KeyValuePair?.Value;
                                break;
                            case "date":
                                dat.Credits.Date = reader.KeyValuePair?.Value;
                                break;
                            case "comment":
                                dat.Credits.Comment = reader.KeyValuePair?.Value;
                                break;
                            default:
                                creditsAdditional.Add(reader.CurrentLine);
                                break;
                        }
                    }

                    // If we're in dat
                    else if (reader.Section.ToLowerInvariant() == "dat")
                    {
                        // Create the section if we haven't already
                        dat.Dat ??= new Dat();

                        switch (reader.KeyValuePair?.Key?.ToLowerInvariant())
                        {
                            case "version":
                                dat.Dat.Version = reader.KeyValuePair?.Value;
                                break;
                            case "plugin":
                                dat.Dat.Plugin = reader.KeyValuePair?.Value;
                                break;
                            case "split":
                                dat.Dat.Split = reader.KeyValuePair?.Value;
                                break;
                            case "merge":
                                dat.Dat.Merge = reader.KeyValuePair?.Value;
                                break;
                            default:
                                datAdditional.Add(reader.CurrentLine);
                                break;
                        }
                    }

                    // If we're in emulator
                    else if (reader.Section.ToLowerInvariant() == "emulator")
                    {
                        // Create the section if we haven't already
                        dat.Emulator ??= new Emulator();

                        switch (reader.KeyValuePair?.Key?.ToLowerInvariant())
                        {
                            case "refname":
                                dat.Emulator.RefName = reader.KeyValuePair?.Value;
                                break;
                            case "version":
                                dat.Emulator.Version = reader.KeyValuePair?.Value;
                                break;
                            default:
                                emulatorAdditional.Add(reader.CurrentLine);
                                break;
                        }
                    }

                    // If we're in games
                    else if (reader.Section.ToLowerInvariant() == "games")
                    {
                        // Create the section if we haven't already
                        dat.Games ??= new Games();

                        // If the line doesn't contain the delimiter
                        if (!reader.CurrentLine.Contains('¬'))
                        {
                            gamesAdditional.Add(reader.CurrentLine);
                            continue;
                        }

                        // Otherwise, separate out the line
                        string[] splitLine = reader.CurrentLine.Split('¬');
                        var rom = new Rom
                        {
                            // EMPTY = splitLine[0]
                            ParentName = splitLine[1],
                            ParentDescription = splitLine[2],
                            GameName = splitLine[3],
                            GameDescription = splitLine[4],
                            RomName = splitLine[5],
                            RomCRC = splitLine[6],
                            RomSize = splitLine[7],
                            RomOf = splitLine[8],
                            MergeName = splitLine[9],
                            // EMPTY = splitLine[10]
                        };

                        if (splitLine.Length > 11)
                            rom.ADDITIONAL_ELEMENTS = splitLine.Skip(11).ToArray();

                        roms.Add(rom);
                    }

                    else
                    {
                        additional.Add(item: reader.CurrentLine);
                    }
                }

                // Add extra pieces and return
                dat.ADDITIONAL_ELEMENTS = additional.ToArray();
                if (dat.Credits != null)
                    dat.Credits.ADDITIONAL_ELEMENTS = creditsAdditional.ToArray();
                if (dat.Dat != null)
                    dat.Dat.ADDITIONAL_ELEMENTS = datAdditional.ToArray();
                if (dat.Emulator != null)
                    dat.Emulator.ADDITIONAL_ELEMENTS = emulatorAdditional.ToArray();
                if (dat.Games != null)
                {
                    dat.Games.Rom = roms.ToArray();
                    dat.Games.ADDITIONAL_ELEMENTS = gamesAdditional.ToArray();
                }
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