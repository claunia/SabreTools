using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using SabreTools.Core.Tools;

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
                var metadata = new Serialization.CrossModel.OfflineList().Serialize(dat);

                // Convert the header to the internal format
                OfflineList.ConvertHeader(dat);

                // Convert the configuration to the internal format
                ConvertConfiguration(dat?.Configuration, keep);

                // Convert to the internal format
                ConvertMetadata(metadata, filename, indexId, keep, statsOnly);

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

            Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, config.DatName);
            Header.SetFieldValue<string?>(Models.Metadata.Header.DatVersionKey, config.DatVersion);
            Header.SetFieldValue<string?>(Models.Metadata.Header.ImFolderKey, config.ImFolder);
            Header.SetFieldValue<string?>(Models.Metadata.Header.RomTitleKey, config.RomTitle);
            Header.SetFieldValue<string?>(Models.Metadata.Header.ScreenshotsHeightKey, config.ScreenshotsHeight);
            Header.SetFieldValue<string?>(Models.Metadata.Header.ScreenshotsWidthKey, config.ScreenshotsWidth);
            Header.SetFieldValue<string?>(Models.Metadata.Header.SystemKey, config.System);
            ConvertCanOpen(config.CanOpen);
            ConvertInfos(config.Infos);
            ConvertNewDat(config.NewDat);
            ConvertSearch(config.Search);

            // Handle implied SuperDAT
            if (config.DatName?.Contains(" - SuperDAT") == true && keep)
            {
                if (Header.GetFieldValue<string?>(Models.Metadata.Header.TypeKey) == null)
                    Header.SetFieldValue<string?>(Models.Metadata.Header.TypeKey, "SuperDAT");
            }
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

            Header.SetFieldValue<OfflineListInfo[]?>(Models.Metadata.Header.InfosKey, [.. offlineListInfos]);
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

            Header.SetFieldValue<string[]?>(Models.Metadata.Header.CanOpenKey, canOpen.Extension); 
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

            Header.SetFieldValue<string?>("DATVERSIONURL", newDat.DatVersionUrl);
            //Header.SetFieldValue<Models.OfflineList.DatUrl?>("DATURL", newDat.DatUrl); // TODO: Add to internal model
            Header.SetFieldValue<string?>("IMURL", newDat.DatVersionUrl);
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
