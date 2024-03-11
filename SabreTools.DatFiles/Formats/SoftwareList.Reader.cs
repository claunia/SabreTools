using System;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing a SoftwareList
    /// </summary>
    internal partial class SoftwareList : DatFile
    {
        /// <inheritdoc/>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            try
            {
                // Deserialize the input file
                var softwarelist = new Serialization.Files.SoftwareList().Deserialize(filename);
                var metadata = new Serialization.CrossModel.SoftwareList().Serialize(softwarelist);

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
