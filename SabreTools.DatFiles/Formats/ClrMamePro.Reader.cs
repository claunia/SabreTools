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
        /// <param name="cmp">Deserialized model to convert</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        private void ConvertHeader(Models.ClrMamePro.ClrMamePro? cmp, bool keep)
        {
            // If the header is missing, we can't do anything
            if (cmp == null)
                return;

            Header.Name ??= cmp.Name;
            Header.Description ??= cmp.Description;
            Header.RootDir ??= cmp.RootDir;
            Header.Category ??= cmp.Category;
            Header.Version ??= cmp.Version;
            Header.Date ??= cmp.Date;
            Header.Author ??= cmp.Author;
            Header.Homepage ??= cmp.Homepage;
            Header.Url ??= cmp.Url;
            Header.Comment ??= cmp.Comment;
            Header.HeaderSkipper ??= cmp.Header;
            Header.Type ??= cmp.Type;
            if (Header.ForceMerging == MergingFlag.None)
                Header.ForceMerging = cmp.ForceMerging?.AsEnumValue<MergingFlag>() ?? MergingFlag.None;
            if (Header.ForcePacking == PackingFlag.None)
                Header.ForcePacking = cmp.ForceZipping?.AsEnumValue<PackingFlag>() ?? PackingFlag.None;
            if (Header.ForcePacking == PackingFlag.None)
                Header.ForcePacking = cmp.ForcePacking?.AsEnumValue<PackingFlag>() ?? PackingFlag.None;

            // Handle implied SuperDAT
            if (cmp.Name?.Contains(" - SuperDAT") == true && keep)
                Header.Type ??= "SuperDAT";
        }

        #endregion
    }
}
