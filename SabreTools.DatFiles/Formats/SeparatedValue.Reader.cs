using System;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing a value-separated DAT
    /// </summary>
    internal partial class SeparatedValue : DatFile
    {
        /// <inheritdoc/>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            try
            {
                // Deserialize the input file
                var metadataFile = new Serialization.Files.SeparatedValue().Deserialize(filename, _delim);
                var metadata = new Serialization.CrossModel.SeparatedValue().Serialize(metadataFile);

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
