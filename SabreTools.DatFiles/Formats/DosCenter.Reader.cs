using System;

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
                var metadata = new Serialization.CrossModel.DosCenter().Serialize(metadataFile);

                // Convert the header to the internal format
                ConvertHeader(metadataFile?.DosCenter, keep);

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
    
        #endregion
    }
}
