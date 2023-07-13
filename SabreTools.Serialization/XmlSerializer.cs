using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization
{
    /// <summary>
    /// XML serializer for nullable types
    /// </summary>
    public abstract class XmlSerializer<T>
    {
        /// <summary>
        /// Deserializes an XML file to the defined type
        /// </summary>
        /// <param name="path">Path to the file to deserialize</param>
        /// <returns>Deserialized data on success, null on failure</returns>
        public static T? Deserialize(string path)
        {
            try
            {
                using var stream = PathProcessor.OpenStream(path);
                return Deserialize(stream);
            }
            catch
            {
                // TODO: Handle logging the exception
                return default;
            }
        }

        /// <summary>
        /// Deserializes an XML file in a stream to the defined type
        /// </summary>
        /// <param name="stream">Stream to deserialize</param>
        /// <returns>Deserialized data on success, null on failure</returns>
        public static T? Deserialize(Stream? stream)
        {
            try
            {
                // If the stream is null
                if (stream == null)
                    return default;

                // Setup the serializer and the reader
                var serializer = new XmlSerializer(typeof(T));
                var settings = new XmlReaderSettings
                {
                    CheckCharacters = false,
                    DtdProcessing = DtdProcessing.Ignore,
                };
                var streamReader = new StreamReader(stream);
                var xmlReader = XmlReader.Create(streamReader, settings);

                // Perform the deserialization and return
                return (T?)serializer.Deserialize(xmlReader);
            }
            catch
            {
                // TODO: Handle logging the exception
                return default;
            }
        }
    }
}