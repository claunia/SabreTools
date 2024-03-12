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
                var mame = new Serialization.Files.Listxml().Deserialize(filename);
                Models.Metadata.MetadataFile? metadata;
                if (mame == null)
                {
                    var m1 = new Serialization.Files.M1().Deserialize(filename);
                    metadata = new Serialization.CrossModel.M1().Serialize(m1);
                }
                else
                {
                    metadata = new Serialization.CrossModel.Listxml().Serialize(mame);
                }

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
