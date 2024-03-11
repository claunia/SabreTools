using System;
using SabreTools.Core;
using SabreTools.Core.Tools;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing of a ClrMamePro DAT
    /// </summary>
    internal partial class ClrMamePro : DatFile
    {
        /// <inheritdoc/>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            try
            {
                // Deserialize the input file
                var metadataFile = new Serialization.Files.ClrMamePro().Deserialize(filename, this.Quotes);
                var metadata = new Serialization.CrossModel.ClrMamePro().Serialize(metadataFile);

                // Convert the header to the internal format
                ConvertHeader(metadataFile?.ClrMamePro, keep);

                // Convert to the internal format
                ConvertMetadata(metadata, filename, indexId, keep, statsOnly);
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
        /// <param name="cmp">Deserialized model to convert</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        private void ConvertHeader(Models.ClrMamePro.ClrMamePro? cmp, bool keep)
        {
            // If the header is missing, we can't do anything
            if (cmp == null)
                return;

            if (Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, cmp.Name);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, cmp.Description);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.RootDirKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.RootDirKey, cmp.RootDir);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.CategoryKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.CategoryKey, cmp.Category);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.VersionKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.VersionKey, cmp.Version);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.DateKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.DateKey, cmp.Date);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.AuthorKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.AuthorKey, cmp.Author);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.HomepageKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.HomepageKey, cmp.Homepage);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.UrlKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.UrlKey, cmp.Url);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.CommentKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.CommentKey, cmp.Comment);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.HeaderKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.HeaderKey, cmp.Header);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.TypeKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.TypeKey, cmp.Type);
            if (Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.ForceMergingKey) == MergingFlag.None)
                Header.SetFieldValue<MergingFlag>(Models.Metadata.Header.ForcePackingKey, cmp.ForceMerging?.AsEnumValue<MergingFlag>() ?? MergingFlag.None);
            if (Header.GetFieldValue<PackingFlag>(Models.Metadata.Header.ForcePackingKey) == PackingFlag.None)
                Header.SetFieldValue<PackingFlag>(Models.Metadata.Header.ForcePackingKey, cmp.ForceZipping?.AsEnumValue<PackingFlag>() ?? PackingFlag.None);
            if (Header.GetFieldValue<PackingFlag>(Models.Metadata.Header.ForcePackingKey) == PackingFlag.None)
                Header.SetFieldValue<PackingFlag>(Models.Metadata.Header.ForcePackingKey, cmp.ForcePacking?.AsEnumValue<PackingFlag>() ?? PackingFlag.None);

            // Handle implied SuperDAT
            if (cmp.Name?.Contains(" - SuperDAT") == true && keep)
            {
                if (Header.GetFieldValue<string?>(Models.Metadata.Header.TypeKey) == null)
                    Header.SetFieldValue<string?>(Models.Metadata.Header.TypeKey, "SuperDAT");
            }
        }

        #endregion
    }
}
