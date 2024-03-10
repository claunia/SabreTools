using System;
using System.Collections.Generic;
using System.Linq;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing an OfflineList XML DAT
    /// </summary>
    internal partial class OfflineList : DatFile
    {
        /// <inheritdoc/>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            try
            {
                // Deserialize the input file
                var dat = new Serialization.Files.OfflineList().Deserialize(filename);

                // Convert the header to the internal format
                OfflineList.ConvertHeader(dat);

                // Convert the configuration to the internal format
                ConvertConfiguration(dat?.Configuration, keep);

                // Convert the games to the internal format
                ConvertGames(dat?.Games, filename, indexId, statsOnly);

                // Convert the GUI to the internal format
                ConvertGUI(dat?.GUI);
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
        /// <param name="dat">Deserialized model to convert</param>
        private static void ConvertHeader(Models.OfflineList.Dat? dat)
        {
            // If the datafile is missing, we can't do anything
            if (dat == null)
                return;

            //Header.NoNamespaceSchemaLocation = dat.NoNamespaceSchemaLocation; // TODO: Add to internal model
        }

        /// <summary>
        /// Convert configuration information
        /// </summary>
        /// <param name="config">Deserialized model to convert</param>
        private void ConvertConfiguration(Models.OfflineList.Configuration? config, bool keep)
        {
            // If the config is missing, we can't do anything
            if (config == null)
                return;

            Header.Name ??= config.DatName;
            //Header.ImFolder ??= config.ImFolder; // TODO: Add to internal model
            Header.Version = config.DatVersion;
            Header.System = config.System;
            Header.ScreenshotsWidth = config.ScreenshotsWidth;
            Header.ScreenshotsHeight = config.ScreenshotsHeight;
            ConvertInfos(config.Infos);
            ConvertCanOpen(config.CanOpen);
            ConvertNewDat(config.NewDat);
            ConvertSearch(config.Search);
            Header.RomTitle = config.RomTitle;

            // Handle implied SuperDAT
            if (config.DatName?.Contains(" - SuperDAT") == true && keep)
                Header.Type ??= "SuperDAT";
        }

        /// <summary>
        /// Convert infos information
        /// </summary>
        /// <param name="infos">Deserialized model to convert</param>
        private void ConvertInfos(Models.OfflineList.Infos? infos)
        {
            // If the infos is missing, we can't do anything
            if (infos == null)
                return;

            var offlineListInfos = new List<OfflineListInfo>();

            if (infos.Title != null)
            {
                offlineListInfos.Add(new OfflineListInfo
                {
                    Name = "title",
                    Visible = infos.Title.Visible.AsYesNo(),
                    InNamingOption = infos.Title.InNamingOption.AsYesNo(),
                    Default = infos.Title.Default.AsYesNo(),
                });
            }
            if (infos.Location != null)
            {
                offlineListInfos.Add(new OfflineListInfo
                {
                    Name = "location",
                    Visible = infos.Location.Visible.AsYesNo(),
                    InNamingOption = infos.Location.InNamingOption.AsYesNo(),
                    Default = infos.Location.Default.AsYesNo(),
                });
            }
            if (infos.Publisher != null)
            {
                offlineListInfos.Add(new OfflineListInfo
                {
                    Name = "publisher",
                    Visible = infos.Publisher.Visible.AsYesNo(),
                    InNamingOption = infos.Publisher.InNamingOption.AsYesNo(),
                    Default = infos.Publisher.Default.AsYesNo(),
                });
            }
            if (infos.SourceRom != null)
            {
                offlineListInfos.Add(new OfflineListInfo
                {
                    Name = "sourceRom",
                    Visible = infos.SourceRom.Visible.AsYesNo(),
                    InNamingOption = infos.SourceRom.InNamingOption.AsYesNo(),
                    Default = infos.SourceRom.Default.AsYesNo(),
                });
            }
            if (infos.SaveType != null)
            {
                offlineListInfos.Add(new OfflineListInfo
                {
                    Name = "saveType",
                    Visible = infos.SaveType.Visible.AsYesNo(),
                    InNamingOption = infos.SaveType.InNamingOption.AsYesNo(),
                    Default = infos.SaveType.Default.AsYesNo(),
                });
            }
            if (infos.RomSize != null)
            {
                offlineListInfos.Add(new OfflineListInfo
                {
                    Name = "romSize",
                    Visible = infos.RomSize.Visible.AsYesNo(),
                    InNamingOption = infos.RomSize.InNamingOption.AsYesNo(),
                    Default = infos.RomSize.Default.AsYesNo(),
                });
            }
            if (infos.ReleaseNumber != null)
            {
                offlineListInfos.Add(new OfflineListInfo
                {
                    Name = "releaseNumber",
                    Visible = infos.ReleaseNumber.Visible.AsYesNo(),
                    InNamingOption = infos.ReleaseNumber.InNamingOption.AsYesNo(),
                    Default = infos.ReleaseNumber.Default.AsYesNo(),
                });
            }
            if (infos.LanguageNumber != null)
            {
                offlineListInfos.Add(new OfflineListInfo
                {
                    Name = "languageNumber",
                    Visible = infos.LanguageNumber.Visible.AsYesNo(),
                    InNamingOption = infos.LanguageNumber.InNamingOption.AsYesNo(),
                    Default = infos.LanguageNumber.Default.AsYesNo(),
                });
            }
            if (infos.Comment != null)
            {
                offlineListInfos.Add(new OfflineListInfo
                {
                    Name = "comment",
                    Visible = infos.Comment.Visible.AsYesNo(),
                    InNamingOption = infos.Comment.InNamingOption.AsYesNo(),
                    Default = infos.Comment.Default.AsYesNo(),
                });
            }
            if (infos.RomCRC != null)
            {
                offlineListInfos.Add(new OfflineListInfo
                {
                    Name = "romCRC",
                    Visible = infos.RomCRC.Visible.AsYesNo(),
                    InNamingOption = infos.RomCRC.InNamingOption.AsYesNo(),
                    Default = infos.RomCRC.Default.AsYesNo(),
                });
            }
            if (infos.Im1CRC != null)
            {
                offlineListInfos.Add(new OfflineListInfo
                {
                    Name = "im1CRC",
                    Visible = infos.Im1CRC.Visible.AsYesNo(),
                    InNamingOption = infos.Im1CRC.InNamingOption.AsYesNo(),
                    Default = infos.Im1CRC.Default.AsYesNo(),
                });
            }
            if (infos.Im2CRC != null)
            {
                offlineListInfos.Add(new OfflineListInfo
                {
                    Name = "im2CRC",
                    Visible = infos.Im2CRC.Visible.AsYesNo(),
                    InNamingOption = infos.Im2CRC.InNamingOption.AsYesNo(),
                    Default = infos.Im2CRC.Default.AsYesNo(),
                });
            }
            if (infos.Languages != null)
            {
                offlineListInfos.Add(new OfflineListInfo
                {
                    Name = "languages",
                    Visible = infos.Languages.Visible.AsYesNo(),
                    InNamingOption = infos.Languages.InNamingOption.AsYesNo(),
                    Default = infos.Languages.Default.AsYesNo(),
                });
            }

            Header.Infos = offlineListInfos;
        }

        /// <summary>
        /// Convert canopen information
        /// </summary>
        /// <param name="canOpen">Deserialized model to convert</param>
        private void ConvertCanOpen(Models.OfflineList.CanOpen? canOpen)
        {
            // If the canOpen is missing, we can't do anything
            if (canOpen?.Extension == null)
                return;

            Header.CanOpen = new List<string>(canOpen.Extension);
        }

        /// <summary>
        /// Convert newdat information
        /// </summary>
        /// <param name="newDat">Deserialized model to convert</param>
        private void ConvertNewDat(Models.OfflineList.NewDat? newDat)
        {
            // If the canOpen is missing, we can't do anything
            if (newDat == null)
                return;

            Header.Url = newDat.DatVersionUrl;
            //Header.DatUrl = newDat.DatUrl; // TODO: Add to internal model
            //Header.ImUrl = newDat.ImUrl; // TODO: Add to internal model
        }

        /// <summary>
        /// Convert search information
        /// </summary>
        /// <param name="search">Deserialized model to convert</param>
        private static void ConvertSearch(Models.OfflineList.Search? search)
        {
            // If the search or to array is missing, we can't do anything
            if (search?.To == null)
                return;

            // TODO: Add to internal model
        }

        /// <summary>
        /// Convert games information
        /// </summary>
        /// <param name="search">Deserialized model to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertGames(Models.OfflineList.Games? games, string filename, int indexId, bool statsOnly)
        {
            // If the games array is missing, we can't do anything
            if (games?.Game == null || !games.Game.Any())
                return;

            foreach (var game in games.Game)
            {
                ConvertGame(game, filename, indexId, statsOnly);
            }
        }

        /// <summary>
        /// Convert game information
        /// </summary>
        /// <param name="search">Deserialized model to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertGame(Models.OfflineList.Game? game, string filename, int indexId, bool statsOnly)
        {
            // If the game is missing, we can't do anything
            if (game == null)
                return;

            var machine = new Machine
            {
                //ImageNumber = game.ImageNumber, // TODO: Add to internal model
                //ReleaseNumber = game.ReleaseNumber, // TODO: Add to internal model
                Name = game.Title,
                //SaveType = game.SaveType, // TODO: Add to internal model
                Publisher = game.Publisher,
                //Location = game.Location, // TODO: Add to internal model
                //SourceRom = game.SourceRom, // TODO: Add to internal model
                //Language = game.Language, // TODO: Add to internal model
                //Im1CRC = game.Im1CRC, // TODO: Add to internal model
                //Im2CRC = game.Im2CRC, // TODO: Add to internal model
                Comment = game.Comment,
            };

            long? size = NumberHelper.ConvertToInt64(game.RomSize);
            if (game.DuplicateID != "0")
                machine.CloneOf = game.DuplicateID;

            // Check if there are any items
            bool containsItems = false;

            // Loop through each file
            ConvertFiles(game.Files, machine, size, game.ReleaseNumber, filename, indexId, statsOnly, ref containsItems);

            // If we had no items, create a Blank placeholder
            if (!containsItems)
            {
                var blank = new Blank
                {
                    Source = new Source { Index = indexId, Name = filename },
                };

                blank.CopyMachineInformation(machine);
                ParseAddHelper(blank, statsOnly);
            }
        }

        /// <summary>
        /// Convert Files information
        /// </summary>
        /// <param name="files">Array of deserialized models to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="size">Item size to use</param>
        /// <param name="releaseNumber">Release number to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertFiles(Models.OfflineList.Files? files, Machine machine, long? size, string? releaseNumber, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the files array is missing, we can't do anything
            if (files?.RomCRC == null || !files.RomCRC.Any())
                return;

            containsItems = true;
            foreach (var crc in files.RomCRC)
            {
                string name = string.Empty;
                if (!string.IsNullOrEmpty(releaseNumber) && releaseNumber != "0")
                    name += $"{releaseNumber} - ";
                name += $"{machine.Name}{crc.Extension}";

                var item = new Rom
                {
                    Source = new Source { Index = indexId, Name = filename },
                };
                item.SetName(name);
                item.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, crc.Content);
                item.SetFieldValue<long?>(Models.Metadata.Rom.SizeKey, size);
                item.SetFieldValue<ItemStatus>(Models.Metadata.Rom.StatusKey, ItemStatus.None);

                item.CopyMachineInformation(machine);
                ParseAddHelper(item, statsOnly);
            }
        }

        /// <summary>
        /// Convert GUI information
        /// </summary>
        /// <param name="gui">Deserialized model to convert</param>
        private static void ConvertGUI(Models.OfflineList.GUI? gui)
        {
            // If the gui or Images are missing, we can't do anything
            if (gui?.Images?.Image == null || !gui.Images.Image.Any())
                return;

            // TODO: Add to internal model
        }

        #endregion

    }
}
