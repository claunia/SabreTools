using System;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing of an AttractMode DAT
    /// </summary>
    internal partial class AttractMode : DatFile
    {
        /// <inheritdoc/>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            try
            {
                // Deserialize the input file
                var metadataFile = new Serialization.Files.AttractMode().Deserialize(filename);
                var metadata = new Serialization.CrossModel.AttractMode().Serialize(metadataFile);

                // Convert to the internal format
                ConvertMetadata(metadata, filename, indexId, keep, statsOnly);
            }
            catch (Exception ex) when (!throwOnError)
            {
                string message = $"'{filename}' - An error occurred during parsing";
                logger.Error(ex, message);
            }
        }
    }
}
