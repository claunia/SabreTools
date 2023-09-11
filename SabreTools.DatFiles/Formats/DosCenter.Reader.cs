using System;
using System.Linq;
using SabreTools.Core.Tools;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing and writing of a DosCenter DAT
    /// </summary>
    internal partial class DosCenter : DatFile
    {
        /// <inheritdoc/>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            try
            {
                // Deserialize the input file
                var metadataFile = new Serialization.Files.DosCenter().Deserialize(filename);

                // Convert the header to the internal format
                ConvertHeader(metadataFile?.DosCenter, keep);

                // Convert the game data to the internal format
                ConvertGames(metadataFile?.Game, filename, indexId, statsOnly);
            }
            catch (Exception ex) when (!throwOnError)
            {
                string message = $"'{filename}' - An error occurred during parsing";
                logger.Error(ex, message);
            }
        }

        #region Converters

        /// <summary>
        /// Convert header information
        /// </summary>
        /// <param name="doscenter">Deserialized model to convert</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        private void ConvertHeader(Models.DosCenter.DosCenter? doscenter, bool keep)
        {
            // If the header is missing, we can't do anything
            if (doscenter == null)
                return;

            Header.Name ??= doscenter.Name;
            Header.Description ??= doscenter.Description;
            Header.Version ??= doscenter.Version;
            Header.Date ??= doscenter.Date;
            Header.Author ??= doscenter.Author;
            Header.Homepage ??= doscenter.Homepage;
            Header.Comment ??= doscenter.Comment;

            // Handle implied SuperDAT
            if (doscenter.Name?.Contains(" - SuperDAT") == true && keep)
                Header.Type ??= "SuperDAT";
        }

        /// <summary>
        /// Convert games information
        /// </summary>
        /// <param name="games">Array of deserialized models to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertGames(Models.DosCenter.Game[]? games, string filename, int indexId, bool statsOnly)
        {
            // If the game array is missing, we can't do anything
            if (games == null || !games.Any())
                return;

            // Loop through the games and add
            foreach (var game in games)
            {
                ConvertGame(game, filename, indexId, statsOnly);
            }
        }

        /// <summary>
        /// Convert game information
        /// </summary>
        /// <param name="game">Deserialized model to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertGame(Models.DosCenter.Game game, string filename, int indexId, bool statsOnly)
        {
            // If the game is missing, we can't do anything
            if (game == null)
                return;

            // Create the machine for copying information
            string? machineName = game.Name?.Trim('"');
            if (machineName?.EndsWith(".zip") == true)
                machineName = System.IO.Path.GetFileNameWithoutExtension(machineName);

            var machine = new Machine { Name = machineName };

            // Check if there are any items
            bool containsItems = false;

            // Loop through each type of item
            ConvertFiles(game.File, machine, filename, indexId, statsOnly, ref containsItems);

            // If we had no items, create a Blank placeholder
            if (!containsItems)
            {
                var blank = new Blank
                {
                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

                blank.CopyMachineInformation(machine);
                ParseAddHelper(blank, statsOnly);
            }
        }

        /// <summary>
        /// Convert Rom information
        /// </summary>
        /// <param name="files">Array of deserialized models to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertFiles(Models.DosCenter.File[]? files, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the files array is missing, we can't do anything
            if (files == null || !files.Any())
                return;

            containsItems = true;
            foreach (var rom in files)
            {
                var item = new Rom
                {
                    Name = rom.Name,
                    Size = NumberHelper.ConvertToInt64(rom.Size),
                    CRC = rom.CRC,
                    Date = rom.Date,

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

                item.CopyMachineInformation(machine);
                ParseAddHelper(item, statsOnly);
            }
        }
    
        #endregion
    }
}
