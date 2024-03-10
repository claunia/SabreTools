using System;
using SabreTools.Core;
using SabreTools.Core.Tools;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing a Logiqx-derived DAT
    /// </summary>
    internal partial class Logiqx : DatFile
    {
        /// <inheritdoc/>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            try
            {
                // Deserialize the input file
                var metadataFile = new Serialization.Files.Logiqx().Deserialize(filename);
                var metadata = new Serialization.CrossModel.Logiqx().Serialize(metadataFile);

                // Convert the header to the internal format
                ConvertHeader(metadataFile, keep);

                // Convert to the internal format
                ConvertMetadata(metadata, filename, indexId, statsOnly);
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
        /// <param name="datafile">Deserialized model to convert</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        private void ConvertHeader(Models.Logiqx.Datafile? datafile, bool keep)
        {
            // If the datafile is missing, we can't do anything
            if (datafile == null)
                return;

            if (Header.GetFieldValue<string?>(Models.Metadata.Header.BuildKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.BuildKey, datafile.Build);
            if (Header.GetFieldValue<bool?>(Models.Metadata.Header.DebugKey) == null)
                Header.SetFieldValue<bool?>(Models.Metadata.Header.DebugKey, datafile.Debug.AsYesNo());
            // SchemaLocation is specifically skipped

            ConvertHeader(datafile.Header, keep);
        }

        /// <summary>
        /// Convert header information
        /// </summary>
        /// <param name="header">Deserialized model to convert</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        private void ConvertHeader(Models.Logiqx.Header? header, bool keep)
        {
            // If the header is missing, we can't do anything
            if (header == null)
                return;

            if (Header.GetFieldValue<string?>(Models.Metadata.Header.IdKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.IdKey, header.Id);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, header.Name);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, header.Description);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.RootDirKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.RootDirKey, header.RootDir);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.CategoryKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.CategoryKey, header.Category);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.VersionKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.VersionKey, header.Version);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.DateKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.DateKey, header.Date);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.AuthorKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.AuthorKey, header.Author);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.EmailKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.EmailKey, header.Email);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.HomepageKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.HomepageKey, header.Homepage);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.UrlKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.UrlKey, header.Url);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.CommentKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.CommentKey, header.Comment);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.TypeKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.TypeKey, header.Type);

            ConvertSubheader(header.ClrMamePro);
            ConvertSubheader(header.RomCenter);

            // Handle implied SuperDAT
            if (header.Name?.Contains(" - SuperDAT") == true && keep)
            {
                if (Header.GetFieldValue<string?>(Models.Metadata.Header.TypeKey) == null)
                    Header.SetFieldValue<string?>(Models.Metadata.Header.TypeKey, "SuperDAT");
            }
        }

        /// <summary>
        /// Convert subheader information
        /// </summary>
        /// <param name="clrMamePro">Deserialized model to convert</param>
        private void ConvertSubheader(Models.Logiqx.ClrMamePro? clrMamePro)
        {
            // If the subheader is missing, we can't do anything
            if (clrMamePro == null)
                return;

            if (Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.ForceMergingKey) == MergingFlag.None)
                Header.SetFieldValue<MergingFlag>(Models.Metadata.Header.ForceMergingKey, clrMamePro.ForceMerging.AsEnumValue<MergingFlag>());
            if (Header.GetFieldValue<NodumpFlag>(Models.Metadata.Header.ForceNodumpKey) == NodumpFlag.None)
                Header.SetFieldValue<NodumpFlag>(Models.Metadata.Header.ForceNodumpKey, clrMamePro.ForceNodump.AsEnumValue<NodumpFlag>());
            if (Header.GetFieldValue<PackingFlag>(Models.Metadata.Header.ForcePackingKey) == PackingFlag.None)
                Header.SetFieldValue<PackingFlag>(Models.Metadata.Header.ForcePackingKey, clrMamePro.ForcePacking.AsEnumValue<PackingFlag>());
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.HeaderKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.HeaderKey, clrMamePro.Header);
        }

        /// <summary>
        /// Convert subheader information
        /// </summary>
        /// <param name="romCenter">Deserialized model to convert</param>
        private void ConvertSubheader(Models.Logiqx.RomCenter? romCenter)
        {
            // If the subheader is missing, we can't do anything
            if (romCenter == null)
                return;

            if (Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.BiosModeKey) == MergingFlag.None)
                Header.SetFieldValue<MergingFlag>(Models.Metadata.Header.BiosModeKey, romCenter.BiosMode.AsEnumValue<MergingFlag>());
            if (Header.GetFieldValue<bool?>(Models.Metadata.Header.LockBiosModeKey) == null)
                Header.SetFieldValue<bool?>(Models.Metadata.Header.LockBiosModeKey, romCenter.LockBiosMode.AsYesNo());
            if (Header.GetFieldValue<bool?>(Models.Metadata.Header.LockRomModeKey) == null)
                Header.SetFieldValue<bool?>(Models.Metadata.Header.LockRomModeKey, romCenter.LockRomMode.AsYesNo());
            if (Header.GetFieldValue<bool?>(Models.Metadata.Header.LockSampleModeKey) == null)
                Header.SetFieldValue<bool?>(Models.Metadata.Header.LockSampleModeKey, romCenter.LockSampleMode.AsYesNo());
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.PluginKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.PluginKey, romCenter.Plugin);
            if (Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.RomModeKey) == MergingFlag.None)
                Header.SetFieldValue<MergingFlag>(Models.Metadata.Header.RomModeKey, romCenter.RomMode.AsEnumValue<MergingFlag>());
            if (Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.SampleModeKey) == MergingFlag.None)
                Header.SetFieldValue<MergingFlag>(Models.Metadata.Header.SampleModeKey, romCenter.SampleMode.AsEnumValue<MergingFlag>());

        }

        #endregion
    }
}
