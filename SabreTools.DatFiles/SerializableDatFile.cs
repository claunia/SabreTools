using System;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.DatFiles
{
    /// <summary>
    /// Represents a DAT that can be serialized
    /// </summary>
    /// <typeparam name="T">Base internal model for the DAT type</typeparam>
    /// <typeparam name="U">IFileSerializer type to use for conversion</typeparam>
    /// <typeparam name="V">IModelSerializer for cross-model serialization</typeparam>
    public abstract class SerializableDatFile<T, U, V> : DatFile
        where U : IFileSerializer<T>
        where V : IModelSerializer<T, Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        protected SerializableDatFile(DatFile? datFile) : base(datFile) { }

        /// <inheritdoc/>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            try
            {
                // Deserialize the input file in two steps
                var specificFormat = Activator.CreateInstance<U>().Deserialize(filename);
                var internalFormat = Activator.CreateInstance<V>().Serialize(specificFormat);

                // Convert to the internal format
                ConvertMetadata(internalFormat, filename, indexId, keep, statsOnly);
            }
            catch (Exception ex) when (!throwOnError)
            {
                string message = $"'{filename}' - An error occurred during parsing";
                logger.Error(ex, message);
            }
        }

        /// <inheritdoc/>
        public override bool WriteToFile(string outfile, bool ignoreblanks = false, bool throwOnError = false)
        {
            try
            {
                logger.User($"Writing to '{outfile}'...");

                // Serialize the input file in two steps
                var internalFormat = ConvertMetadata(ignoreblanks);
                var specificFormat = Activator.CreateInstance<V>().Deserialize(internalFormat);
                if (!Activator.CreateInstance<U>().Serialize(specificFormat, outfile))
                {
                    logger.Warning($"File '{outfile}' could not be written! See the log for more details.");
                    return false;
                }
            }
            catch (Exception ex) when (!throwOnError)
            {
                logger.Error(ex);
                return false;
            }

            logger.User($"'{outfile}' written!{Environment.NewLine}");
            return true;
        }
    }
}
