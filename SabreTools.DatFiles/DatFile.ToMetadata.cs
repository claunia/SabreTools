using System.Collections.Generic;
using System.Linq;
using SabreTools.Core;
using SabreTools.Core.Tools;

namespace SabreTools.DatFiles
{
    public partial class DatFile
    {
        #region To Metadata

        /// <summary>
        /// Convert metadata information
        /// </summary>
        public Models.Metadata.MetadataFile? ConvertMetadata()
        {
            // If we don't have items, we can't do anything
            if (this.Items == null || !this.Items.Any())
                return null;

            // Create an object to hold the data
            var metadataFile = new Models.Metadata.MetadataFile();

            // Convert and assign the header
            var header = ConvertHeader();
            if (header != null)
                metadataFile[Models.Metadata.MetadataFile.HeaderKey] = header;

            // TODO: Implement machine creation

            return metadataFile;
        }

        /// <summary>
        /// Convert header information
        /// </summary>
        private Models.Metadata.Header? ConvertHeader()
        {
            // If the header is invalid, we can't do anything
            if (Header == null)
                return null;

            // Create an internal header
            var header = new Models.Metadata.Header();

            if (Header.GetFieldValue<string?>(Models.Metadata.Header.AuthorKey) == null)
                header[Models.Metadata.Header.AuthorKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.AuthorKey);
            if (Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.BiosModeKey) == MergingFlag.None)
                header[Models.Metadata.Header.BiosModeKey] = Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.BiosModeKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.BuildKey) == null)
                header[Models.Metadata.Header.BuildKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.BuildKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.CategoryKey) == null)
                header[Models.Metadata.Header.CategoryKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.CategoryKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.CommentKey) == null)
                header[Models.Metadata.Header.CommentKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.CommentKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.DateKey) == null)
                header[Models.Metadata.Header.DateKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.DateKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.DatVersionKey) == null)
                header[Models.Metadata.Header.DatVersionKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.DatVersionKey);
            if (Header.GetFieldValue<bool?>(Models.Metadata.Header.DebugKey) == null)
                header[Models.Metadata.Header.DebugKey] = Header.GetFieldValue<bool?>(Models.Metadata.Header.DebugKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey) == null)
                header[Models.Metadata.Header.DescriptionKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.EmailKey) == null)
                header[Models.Metadata.Header.EmailKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.EmailKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.EmulatorVersionKey) == null)
                header[Models.Metadata.Header.EmulatorVersionKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.EmulatorVersionKey);
            if (Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.ForceMergingKey) == MergingFlag.None)
                header[Models.Metadata.Header.ForceMergingKey] = Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.ForceMergingKey);
            if (Header.GetFieldValue<NodumpFlag>(Models.Metadata.Header.ForceNodumpKey) == NodumpFlag.None)
                header[Models.Metadata.Header.ForceNodumpKey] = Header.GetFieldValue<NodumpFlag>(Models.Metadata.Header.ForceNodumpKey);
            if (Header.GetFieldValue<PackingFlag>(Models.Metadata.Header.ForcePackingKey) == PackingFlag.None)
                header[Models.Metadata.Header.ForcePackingKey] = Header.GetFieldValue<PackingFlag>(Models.Metadata.Header.ForcePackingKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.HeaderKey) == null)
                header[Models.Metadata.Header.HeaderKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.HeaderKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.HomepageKey) == null)
                header[Models.Metadata.Header.HomepageKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.HomepageKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.IdKey) == null)
                header[Models.Metadata.Header.IdKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.IdKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.ImFolderKey) == null)
                header[Models.Metadata.Header.ImFolderKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.ImFolderKey);
            if (Header.GetFieldValue<bool?>(Models.Metadata.Header.LockBiosModeKey) == null)
                header[Models.Metadata.Header.LockBiosModeKey] = Header.GetFieldValue<bool?>(Models.Metadata.Header.LockBiosModeKey);
            if (Header.GetFieldValue<bool?>(Models.Metadata.Header.LockRomModeKey) == null)
                header[Models.Metadata.Header.LockRomModeKey] = Header.GetFieldValue<bool?>(Models.Metadata.Header.LockRomModeKey);
            if (Header.GetFieldValue<bool?>(Models.Metadata.Header.LockSampleModeKey) == null)
                header[Models.Metadata.Header.LockSampleModeKey] = Header.GetFieldValue<bool?>(Models.Metadata.Header.LockSampleModeKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.MameConfigKey) == null)
                header[Models.Metadata.Header.MameConfigKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey) == null)
                header[Models.Metadata.Header.NameKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.MameConfigKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.PluginKey) == null)
                header[Models.Metadata.Header.PluginKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.PluginKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.RefNameKey) == null)
                header[Models.Metadata.Header.RefNameKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.RefNameKey);
            if (Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.RomModeKey) == MergingFlag.None)
                header[Models.Metadata.Header.RomModeKey] = Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.RomModeKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.RomTitleKey) == null)
                header[Models.Metadata.Header.RomTitleKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.RomTitleKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.RootDirKey) == null)
                header[Models.Metadata.Header.RootDirKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.RootDirKey);
            if (Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.SampleModeKey) == MergingFlag.None)
                header[Models.Metadata.Header.SampleModeKey] = Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.SampleModeKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.ScreenshotsHeightKey) == null)
                header[Models.Metadata.Header.ScreenshotsHeightKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.ScreenshotsHeightKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.ScreenshotsWidthKey) == null)
                header[Models.Metadata.Header.ScreenshotsWidthKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.ScreenshotsWidthKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.SystemKey) == null)
                header[Models.Metadata.Header.SystemKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.SystemKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.TypeKey) == null)
                header[Models.Metadata.Header.TypeKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.TypeKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.UrlKey) == null)
                header[Models.Metadata.Header.UrlKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.UrlKey);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.VersionKey) == null)
                header[Models.Metadata.Header.VersionKey] = Header.GetFieldValue<string?>(Models.Metadata.Header.VersionKey);

            // Convert subheader values
            if (Header.CanOpenSpecified)
                header[Models.Metadata.Header.CanOpenKey] = Header.GetFieldValue<Models.OfflineList.CanOpen[]?>(Models.Metadata.Header.CanOpenKey);
            // if (Header.ImagesSpecified)
            //     // TODO: Add to internal model
            if (Header.InfosSpecified)
            {
                var infoItem = new Models.OfflineList.Infos();
                var infos = Header.GetFieldValue<Formats.OfflineListInfo[]?>(Models.Metadata.Header.InfosKey)!;
                foreach (var info in infos)
                {
                    switch (info.Name)
                    {
                        case "title":
                            infoItem.Title = new Models.OfflineList.Title
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                        case "location":
                            infoItem.Location = new Models.OfflineList.Location
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                        case "publisher":
                            infoItem.Publisher = new Models.OfflineList.Publisher
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                        case "sourceRom":
                            infoItem.SourceRom = new Models.OfflineList.SourceRom
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                        case "saveType":
                            infoItem.SaveType = new Models.OfflineList.SaveType
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                        case "romSize":
                            infoItem.RomSize = new Models.OfflineList.RomSize
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                        case "releaseNumber":
                            infoItem.ReleaseNumber = new Models.OfflineList.ReleaseNumber
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                        case "languageNumber":
                            infoItem.LanguageNumber = new Models.OfflineList.LanguageNumber
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                        case "comment":
                            infoItem.Comment = new Models.OfflineList.Comment
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                        case "romCRC":
                            infoItem.RomCRC = new Models.OfflineList.RomCRC
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                        case "im1CRC":
                            infoItem.Im1CRC = new Models.OfflineList.Im1CRC
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                        case "im2CRC":
                            infoItem.Im2CRC = new Models.OfflineList.Im2CRC
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                        case "languages":
                            infoItem.Languages = new Models.OfflineList.Languages
                            {
                                Visible = info.Visible.FromYesNo(),
                                InNamingOption = info.InNamingOption.FromYesNo(),
                                Default = info.Default.FromYesNo(),
                            };
                            break;
                    }
                }

                header[Models.Metadata.Header.InfosKey] = infoItem;
            }
            if (Header.NewDatSpecified)
            {
                var newDat = new Models.OfflineList.NewDat
                {
                    DatVersionUrl = Header.GetFieldValue<string?>("DATVERSIONURL"),
                    //DatUrl = Header.GetFieldValue<Models.OfflineList.DatUrl?>("DATURL"), // TODO: Add to internal model
                    ImUrl = Header.GetFieldValue<string?>("IMURL"),
                };
                header[Models.Metadata.Header.NewDatKey] = newDat;
            }
            // if (Header.SearchSpecified)
            //     // TODO: Add to internal model

            return header;
        }

        #endregion
    }
}