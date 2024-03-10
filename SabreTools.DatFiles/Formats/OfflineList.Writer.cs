using System;
using System.Collections.Generic;
using System.Linq;
using SabreTools.Core;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents writing an OfflineList XML DAT
    /// </summary>
    internal partial class OfflineList : DatFile
    {
        /// <inheritdoc/>
        protected override ItemType[] GetSupportedTypes()
        {
            return
            [
                ItemType.Rom
            ];
        }

        /// <inheritdoc/>
        protected override List<string>? GetMissingRequiredFields(DatItem datItem)
        {
            var missingFields = new List<string>();

            // Check item name
            if (string.IsNullOrEmpty(datItem.GetName()))
                missingFields.Add(Models.Metadata.Rom.NameKey);

            switch (datItem)
            {
                case Rom rom:
                    if (rom.GetFieldValue<long?>(Models.Metadata.Rom.SizeKey) == null || rom.GetFieldValue<long?>(Models.Metadata.Rom.SizeKey) < 0)
                        missingFields.Add(Models.Metadata.Rom.SizeKey);
                    if (string.IsNullOrEmpty(rom.GetFieldValue<string?>(Models.Metadata.Rom.CRCKey)))
                        missingFields.Add(Models.Metadata.Rom.CRCKey);
                    break;
            }

            return missingFields;
        }

        /// <inheritdoc/>
        public override bool WriteToFile(string outfile, bool ignoreblanks = false, bool throwOnError = false)
        {
            try
            {
                logger.User($"Writing to '{outfile}'...");

                var datafile = CreateDat(ignoreblanks);
                if (!(new Serialization.Files.OfflineList().Serialize(datafile, outfile)))
                {
                    logger.Warning($"File '{outfile}' could not be written! See the log for more details.");
                    return false;
                }
            }
            catch (Exception ex) when (!throwOnError)
            {
                logger.Error(ex);
                return false;
            }

            logger.User($"'{outfile}' written!{Environment.NewLine}");
            return true;
        }

        #region Converters

        /// <summary>
        /// Create a Dat from the current internal information
        /// <summary>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise</param>
        private Models.OfflineList.Dat CreateDat(bool ignoreblanks)
        {
            var dat = new Models.OfflineList.Dat
            {
                NoNamespaceSchemaLocation = "datas.xsd",
                Configuration = CreateConfiguration(),
                Games = CreateGames(ignoreblanks),
                GUI = CreateGUI(),
            };

            return dat;
        }

        /// <summary>
        /// Create a Configuration from the current internal information
        /// <summary>
        private Models.OfflineList.Configuration? CreateConfiguration()
        {
            // If we don't have a header, we can't do anything
            if (this.Header == null)
                return null;

            var configuration = new Models.OfflineList.Configuration
            {
                DatName = Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey),
                ImFolder = Header.GetFieldValue<string?>(Models.Metadata.Header.ImFolderKey),
                DatVersion = Header.GetFieldValue<string?>(Models.Metadata.Header.DatVersionKey),
                System = Header.GetFieldValue<string?>(Models.Metadata.Header.SystemKey),
                ScreenshotsWidth = Header.GetFieldValue<string?>(Models.Metadata.Header.ScreenshotsWidthKey),
                ScreenshotsHeight = Header.GetFieldValue<string?>(Models.Metadata.Header.ScreenshotsHeightKey),
                Infos = CreateInfos(),
                CanOpen = CreateCanOpen(),
                NewDat = CreateNewDat(),
                Search = CreateSearch(),
                RomTitle = Header.GetFieldValue<string?>(Models.Metadata.Header.RomTitleKey),
            };

            return configuration;
        }

        /// <summary>
        /// Create a Infos from the current internal information
        /// <summary>
        private Models.OfflineList.Infos? CreateInfos()
        {
            // If we don't have infos, we can't do anything
            if (!Header.InfosSpecified)
                return null;

            var infos = new Models.OfflineList.Infos();
            foreach (var info in Header.GetFieldValue<OfflineListInfo[]?>(Models.Metadata.Header.InfosKey)!)
            {
                switch (info.Name)
                {
                    case "title":
                        infos.Title = new Models.OfflineList.Title
                        {
                            Visible = info.Visible?.ToString(),
                            InNamingOption = info.InNamingOption?.ToString(),
                            Default = info.Default?.ToString(),
                        };
                        break;

                    case "location":
                        infos.Location = new Models.OfflineList.Location
                        {
                            Visible = info.Visible?.ToString(),
                            InNamingOption = info.InNamingOption?.ToString(),
                            Default = info.Default?.ToString(),
                        };
                        break;

                    case "publisher":
                        infos.Publisher = new Models.OfflineList.Publisher
                        {
                            Visible = info.Visible?.ToString(),
                            InNamingOption = info.InNamingOption?.ToString(),
                            Default = info.Default?.ToString(),
                        };
                        break;

                    case "sourceRom":
                        infos.SourceRom = new Models.OfflineList.SourceRom
                        {
                            Visible = info.Visible?.ToString(),
                            InNamingOption = info.InNamingOption?.ToString(),
                            Default = info.Default?.ToString(),
                        };
                        break;

                    case "saveType":
                        infos.SaveType = new Models.OfflineList.SaveType
                        {
                            Visible = info.Visible?.ToString(),
                            InNamingOption = info.InNamingOption?.ToString(),
                            Default = info.Default?.ToString(),
                        };
                        break;

                    case "romSize":
                        infos.RomSize = new Models.OfflineList.RomSize
                        {
                            Visible = info.Visible?.ToString(),
                            InNamingOption = info.InNamingOption?.ToString(),
                            Default = info.Default?.ToString(),
                        };
                        break;

                    case "releaseNumber":
                        infos.ReleaseNumber = new Models.OfflineList.ReleaseNumber
                        {
                            Visible = info.Visible?.ToString(),
                            InNamingOption = info.InNamingOption?.ToString(),
                            Default = info.Default?.ToString(),
                        };
                        break;

                    case "languageNumber":
                        infos.LanguageNumber = new Models.OfflineList.LanguageNumber
                        {
                            Visible = info.Visible?.ToString(),
                            InNamingOption = info.InNamingOption?.ToString(),
                            Default = info.Default?.ToString(),
                        };
                        break;

                    case "comment":
                        infos.Comment = new Models.OfflineList.Comment
                        {
                            Visible = info.Visible?.ToString(),
                            InNamingOption = info.InNamingOption?.ToString(),
                            Default = info.Default?.ToString(),
                        };
                        break;

                    case "romCRC":
                        infos.RomCRC = new Models.OfflineList.RomCRC
                        {
                            Visible = info.Visible?.ToString(),
                            InNamingOption = info.InNamingOption?.ToString(),
                            Default = info.Default?.ToString(),
                        };
                        break;

                    case "im1CRC":
                        infos.Im1CRC = new Models.OfflineList.Im1CRC
                        {
                            Visible = info.Visible?.ToString(),
                            InNamingOption = info.InNamingOption?.ToString(),
                            Default = info.Default?.ToString(),
                        };
                        break;

                    case "im2CRC":
                        infos.Im2CRC = new Models.OfflineList.Im2CRC
                        {
                            Visible = info.Visible?.ToString(),
                            InNamingOption = info.InNamingOption?.ToString(),
                            Default = info.Default?.ToString(),
                        };
                        break;

                    case "languages":
                        infos.Languages = new Models.OfflineList.Languages
                        {
                            Visible = info.Visible?.ToString(),
                            InNamingOption = info.InNamingOption?.ToString(),
                            Default = info.Default?.ToString(),
                        };
                        break;
                }
            }

            return infos;
        }

        /// <summary>
        /// Create a CanOpen from the current internal information
        /// <summary>
        private Models.OfflineList.CanOpen? CreateCanOpen()
        {
            // If we don't have a canopen, we can't do anything
            if (!Header.CanOpenSpecified || Header.GetFieldValue<string[]?>(Models.Metadata.Header.CanOpenKey) == null)
                return null;

            var canOpen = new Models.OfflineList.CanOpen
            {
                Extension = [.. Header.GetFieldValue<string[]?>(Models.Metadata.Header.CanOpenKey)],
            };

            return canOpen;
        }

        /// <summary>
        /// Create a NewDat from the current internal information
        /// <summary>
        private Models.OfflineList.NewDat? CreateNewDat()
        {
            // If we don't have a Header, we can't do anything
            if (Header == null)
                return null;

            var newDat = new Models.OfflineList.NewDat
            {
                DatVersionUrl = Header.GetFieldValue<string?>("DATVERSIONURL"),
                //DatUrl = Header.GetFieldValue<Models.OfflineList.DatUrl?>("DATURL"); // TODO: Add to internal model
                ImUrl = Header.GetFieldValue<string?>("IMURL"),
            };

            return newDat;
        }

        /// <summary>
        /// Create a Search from the current internal information
        /// <summary>
        private Models.OfflineList.Search? CreateSearch()
        {
            // If we don't have a Header, we can't do anything
            if (Header == null)
                return null;

            // TODO: Add to internal model
            return null;
        }

        /// <summary>
        /// Create a Games from the current internal information
        /// <summary>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise</param>
        private Models.OfflineList.Games? CreateGames(bool ignoreblanks)
        {
            // If we don't have items, we can't do anything
            if (this.Items == null || !this.Items.Any())
                return null;

            // Create a list of hold the games
            var games = new List<Models.OfflineList.Game>();

            // Loop through the sorted items and create games for them
            foreach (string key in Items.SortedKeys)
            {
                var items = Items.FilteredItems(key);
                if (items == null || !items.Any())
                    continue;

                // Get the first item for game information
                var machine = items[0].Machine;
                var game = OfflineList.CreateGame(machine!);

                // Create holders for all item types
                var romCRCs = new List<Models.OfflineList.FileRomCRC>();

                // Loop through and convert the items to respective lists
                for (int index = 0; index < items.Count; index++)
                {
                    // Get the item
                    var item = items[index];

                    // Check for a "null" item
                    item = ProcessNullifiedItem(item);

                    // Skip if we're ignoring the item
                    if (ShouldIgnore(item, ignoreblanks))
                        continue;

                    switch (item)
                    {
                        case Rom rom:
                            romCRCs.Add(CreateRomCRC(rom));
                            break;
                    }
                }

                // Assign the values to the game
                game.Files = new Models.OfflineList.Files { RomCRC = [.. romCRCs] };

                // Add the game to the list
                games.Add(game);
            }

            return new Models.OfflineList.Games { Game = [.. games] };
        }

        /// <summary>
        /// Create a Machine from the current internal information
        /// <summary>
        private static Models.OfflineList.Game CreateGame(Machine machine)
        {
            var game = new Models.OfflineList.Game
            {
                ImageNumber = machine.GetFieldValue<string?>(Models.Metadata.Machine.ImageNumberKey),
                ReleaseNumber = machine.GetFieldValue<string?>(Models.Metadata.Machine.ReleaseNumberKey),
                Title = machine.GetFieldValue<string?>(Models.Metadata.Machine.NameKey),
                SaveType = machine.GetFieldValue<string?>(Models.Metadata.Machine.SaveTypeKey),
                Publisher = machine.GetFieldValue<string?>(Models.Metadata.Machine.PublisherKey),
                Location = machine.GetFieldValue<string?>(Models.Metadata.Machine.LocationKey),
                SourceRom = machine.GetFieldValue<string?>(Models.Metadata.Machine.SourceRomKey),
                Language = machine.GetFieldValue<string?>(Models.Metadata.Machine.LanguageKey),
                Im1CRC = machine.GetFieldValue<string?>(Models.Metadata.Machine.Im1CRCKey),
                Im2CRC = machine.GetFieldValue<string?>(Models.Metadata.Machine.Im2CRCKey),
                Comment = machine.GetFieldValue<string?>(Models.Metadata.Machine.CommentKey),
                DuplicateID = machine.GetFieldValue<string?>(Models.Metadata.Machine.CloneOfKey),
            };

            return game;
        }

        /// <summary>
        /// Create a RomCRC from the current Rom DatItem
        /// <summary>
        private static Models.OfflineList.FileRomCRC CreateRomCRC(Rom item)
        {
            var romCRC = new Models.OfflineList.FileRomCRC
            {
                Content = item.GetFieldValue<string?>(Models.Metadata.Rom.CRCKey),
            };

            return romCRC;
        }

        /// <summary>
        /// Create a GUI from the current internal information
        /// <summary>
        private Models.OfflineList.GUI? CreateGUI()
        {
            // If we don't have a header, we can't do anything
            if (this.Header == null)
                return null;

            // TODO: Add to internal model
            return null;
        }

        #endregion
    }
}
