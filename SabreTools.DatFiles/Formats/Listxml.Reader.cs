using System;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing a MAME XML DAT
    /// </summary>
    internal partial class Listxml : DatFile
    {
        /// <inheritdoc/>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            try
            {
                // Deserialize the input file
                // TODO: Support M1 DATs again
                var mame = new Serialization.Files.Listxml().Deserialize(filename);
                var metadata = new Serialization.CrossModel.Listxml().Serialize(mame);

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
