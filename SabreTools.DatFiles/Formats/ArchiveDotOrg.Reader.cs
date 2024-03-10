using System;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing a Archive.org file list
    /// </summary>
    internal partial class ArchiveDotOrg : DatFile
    {
        /// <inheritdoc/>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            try
            {
                // Deserialize the input file
                var files = new Serialization.Files.ArchiveDotOrg().Deserialize(filename);
                var metadata = new Serialization.CrossModel.ArchiveDotOrg().Serialize(files);

                // Convert to the internal format
                ConvertMetadata(metadata, filename, indexId, statsOnly);
            }
            catch (Exception ex) when (!throwOnError)
            {
                string message = $"'{filename}' - An error occurred during parsing";
                logger.Error(ex, message);
            }
        }
    }
}
