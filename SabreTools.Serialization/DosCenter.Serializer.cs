using System.IO;
using System.Linq;
using System.Text;
using SabreTools.IO.Writers;
using SabreTools.Models.DosCenter;
namespace SabreTools.Serialization
{
    /// <summary>
    /// Deserializer for DosCenter metadata files
    /// </summary>
    public partial class DosCenter
    {
        /// <summary>
        /// Serializes the defined type to a DosCenter metadata file
        /// </summary>
        /// <param name="metadataFile">Data to serialize</param>
        /// <param name="path">Path to the file to serialize to</param>
        /// <returns>True on successful serialization, false otherwise</returns>
        public static bool SerializeToFile(MetadataFile? metadataFile, string path)
        {
            using var stream = SerializeToStream(metadataFile);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }

        /// <summary>
        /// Serializes the defined type to a stream
        /// </summary>
        /// <param name="metadataFile">Data to serialize</param>
        /// <returns>Stream containing serialized data on success, null otherwise</returns>
        public static Stream? SerializeToStream(MetadataFile? metadataFile)
        {
            // If the metadata file is null
            if (metadataFile == null)
                return null;

            // Setup the writer and output
            var stream = new MemoryStream();
            var writer = new ClrMameProWriter(stream, Encoding.UTF8)
            {
                Quotes = false,
            };

            // Write the header, if it exists
            WriteHeader(metadataFile.DosCenter, writer);

            // Write out the games, if they exist
            WriteGames(metadataFile.Game, writer);

            // Return the stream
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// Write header information to the current writer
        /// </summary>
        /// <param name="header">DosCenter representing the header information</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteHeader(Models.DosCenter.DosCenter? header, ClrMameProWriter writer)
        {
            // If the header information is missing, we can't do anything
            if (header == null)
                return;

            writer.WriteStartElement("DOSCenter");

            writer.WriteOptionalStandalone("Name:", header.Name);
            writer.WriteOptionalStandalone("Description:", header.Description);
            writer.WriteOptionalStandalone("Version:", header.Version);
            writer.WriteOptionalStandalone("Date:", header.Date);
            writer.WriteOptionalStandalone("Author:", header.Author);
            writer.WriteOptionalStandalone("Homepage:", header.Homepage);
            writer.WriteOptionalStandalone("Comment:", header.Comment);

            writer.WriteEndElement(); // doscenter
            writer.Flush();
        }

        /// <summary>
        /// Write games information to the current writer
        /// </summary>
        /// <param name="games">Array of Game objects representing the games information</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteGames(Game[]? games, ClrMameProWriter writer)
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
        /// <param name="game">Game object representing the game information</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteGame(Game game, ClrMameProWriter writer)
        {
            // If the game information is missing, we can't do anything
            if (game == null)
                return;

            writer.WriteStartElement("game");

            // Write the standalone values
            writer.WriteRequiredStandalone("name", game.Name, throwOnError: true);

            // Write the item values
            WriteFiles(game.File, writer);

            writer.WriteEndElement(); // game
        }

        /// <summary>
        /// Write files information to the current writer
        /// </summary>
        /// <param name="files">Array of File objects to write</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteFiles(Models.DosCenter.File[]? files, ClrMameProWriter writer)
        {
            // If the array is missing, we can't do anything
            if (files == null)
                return;

            foreach (var file in files)
            {
                writer.WriteStartElement("file");

                writer.WriteRequiredAttributeString("name", file.Name, throwOnError: true);
                writer.WriteRequiredAttributeString("size", file.Size, throwOnError: true);
                writer.WriteOptionalAttributeString("date", file.Date);
                writer.WriteRequiredAttributeString("crc", file.CRC?.ToUpperInvariant(), throwOnError: true);

                writer.WriteEndElement(); // file
            }
        }

        #region Internal

        /// <summary>
        /// Convert from <cref="Models.DosCenter.MetadataFile"/> to <cref="Models.Metadata.MetadataFile"/>
        /// </summary>
        public static Models.Metadata.MetadataFile? ConvertToInternalModel(MetadataFile? item)
        {
            if (item == null)
                return null;
            
            var metadataFile = new Models.Metadata.MetadataFile();

            if (item?.DosCenter != null)
                metadataFile[Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item.DosCenter);

            if (item?.Game != null && item.Game.Any())
            {
                metadataFile[Models.Metadata.MetadataFile.MachineKey] = item.Game
                    .Where(g => g != null)
                    .Select(ConvertMachineToInternalModel)
                    .ToArray();
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.DosCenter.DosCenter"/> to <cref="Models.Metadata.Header"/>
        /// </summary>
        private static Models.Metadata.Header ConvertHeaderToInternalModel(Models.DosCenter.DosCenter item)
        {
            var header = new Models.Metadata.Header
            {
                [Models.Metadata.Header.NameKey] = item.Name,
                [Models.Metadata.Header.DescriptionKey] = item.Description,
                [Models.Metadata.Header.VersionKey] = item.Version,
                [Models.Metadata.Header.DateKey] = item.Date,
                [Models.Metadata.Header.AuthorKey] = item.Author,
                [Models.Metadata.Header.HomepageKey] = item.Homepage,
                [Models.Metadata.Header.CommentKey] = item.Comment,
            };
            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.DosCenter.Game"/> to <cref="Models.Metadata.Machine"/>
        /// </summary>
        private static Models.Metadata.Machine ConvertMachineToInternalModel(Game item)
        {
            var machine = new Models.Metadata.Machine
            {
                [Models.Metadata.Machine.NameKey] = item.Name,
            };

            if (item.File != null && item.File.Any())
            {
                machine[Models.Metadata.Machine.RomKey] = item.File
                    .Where(f => f != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.DosCenter.File"/> to <cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Models.Metadata.Rom ConvertToInternalModel(Models.DosCenter.File item)
        {
            var rom = new Models.Metadata.Rom
            {
                [Models.Metadata.Rom.NameKey] = item.Name,
                [Models.Metadata.Rom.SizeKey] = item.Size,
                [Models.Metadata.Rom.CRCKey] = item.CRC,
                [Models.Metadata.Rom.DateKey] = item.Date,
            };
            return rom;
        }

        #endregion
    }
}