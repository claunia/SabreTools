using System;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing an openMSX softawre list XML DAT
    /// </summary>
    internal partial class OpenMSX : DatFile
    {
        /// <inheritdoc/>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            try
            {
                // Deserialize the input file
                var softwareDb = new Serialization.Files.OpenMSX().Deserialize(filename);
                var metadata = new Serialization.CrossModel.OpenMSX().Serialize(softwareDb);

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
