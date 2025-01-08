using System;
using SabreTools.Models.Metadata;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.DatFiles
{
    /// <summary>
    /// Represents a DAT that can be serialized
    /// </summary>
    /// <typeparam name="TModel">Base internal model for the DAT type</typeparam>
    /// <typeparam name="TFileDeserializer">IFileDeserializer type to use for conversion</typeparam>
    /// <typeparam name="TFileSerializer">IFileSerializer type to use for conversion</typeparam>
    /// <typeparam name="TModelSerializer">IModelSerializer for cross-model serialization</typeparam>
    public abstract class SerializableDatFile<TModel, TFileDeserializer, TFileSerializer, TModelSerializer> : DatFile
        where TFileDeserializer : IFileDeserializer<TModel>
        where TFileSerializer : IFileSerializer<TModel>
        where TModelSerializer : IModelSerializer<TModel, MetadataFile>
    {
        /// <inheritdoc/>
        protected SerializableDatFile(DatFile? datFile) : base(datFile) { }

        /// <inheritdoc/>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            try
            {
                // Deserialize the input file in two steps
                var specificFormat = Activator.CreateInstance<TFileDeserializer>().Deserialize(filename);
                var internalFormat = Activator.CreateInstance<TModelSerializer>().Serialize(specificFormat);

                // Convert to the internal format
                ConvertMetadata(internalFormat, filename, indexId, keep, statsOnly);
            }
            catch (Exception ex) when (!throwOnError)
            {
                string message = $"'{filename}' - An error occurred during parsing";
                _logger.Error(ex, message);
            }
        }

        /// <inheritdoc/>
        public override bool WriteToFile(string outfile, bool ignoreblanks = false, bool throwOnError = false)
        {
            try
            {
                _logger.User($"Writing to '{outfile}'...");

                // Serialize the input file in two steps
                var internalFormat = ConvertMetadata(ignoreblanks);
                var specificFormat = Activator.CreateInstance<TModelSerializer>().Deserialize(internalFormat);
                if (!Activator.CreateInstance<TFileSerializer>().Serialize(specificFormat, outfile))
                {
                    _logger.Warning($"File '{outfile}' could not be written! See the log for more details.");
                    return false;
                }
            }
            catch (Exception ex) when (!throwOnError)
            {
                _logger.Error(ex);
                return false;
            }

            _logger.User($"'{outfile}' written!{Environment.NewLine}");
            return true;
        }

        /// <inheritdoc/>
        public override bool WriteToFileDB(string outfile, bool ignoreblanks = false, bool throwOnError = false)
        {
            try
            {
                _logger.User($"Writing to '{outfile}'...");

                // Serialize the input file in two steps
                var internalFormat = ConvertMetadataDB(ignoreblanks);
                var specificFormat = Activator.CreateInstance<TModelSerializer>().Deserialize(internalFormat);
                if (!Activator.CreateInstance<TFileSerializer>().Serialize(specificFormat, outfile))
                {
                    _logger.Warning($"File '{outfile}' could not be written! See the log for more details.");
                    return false;
                }
            }
            catch (Exception ex) when (!throwOnError)
            {
                _logger.Error(ex);
                return false;
            }

            _logger.User($"'{outfile}' written!{Environment.NewLine}");
            return true;
        }
    }
}
