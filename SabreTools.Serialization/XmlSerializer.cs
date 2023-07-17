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

        /// <summary>
        /// Serializes the defined type to an XML file
        /// </summary>
        /// <param name="obj">Data to serialize</param>
        /// <param name="path">Path to the file to serialize to</param>
        /// <returns>True on successful serialization, false otherwise</returns>
        public static bool SerializeToFile(T? obj, string path)
        {
            try
            {
                using var stream = SerializeToStream(obj);
                if (stream == null)
                    return false;

                using var fs = File.OpenWrite(path);
                stream.CopyTo(fs);
                return true;
            }
            catch
            {
                // TODO: Handle logging the exception
                return false;
            }
        }

        /// <summary>
        /// Serializes the defined type to a stream
        /// </summary>
        /// <param name="obj">Data to serialize</param>
        /// <returns>Stream containing serialized data on success, null otherwise</returns>
        public static Stream? SerializeToStream(T? obj)
        {
            try
            {
                // If the object is null
                if (obj == null)
                    return null;

                // Setup the serializer and the reader
                var serializer = new XmlSerializer(typeof(T));
                var settings = new XmlWriterSettings
                {
                    CheckCharacters = false,
                };
                var stream = new MemoryStream();
                var streamWriter = new StreamWriter(stream);
                var xmlWriter = XmlWriter.Create(streamWriter, settings);

                // Perform the deserialization and return
                serializer.Serialize(xmlWriter, obj);
                return stream;
            }
            catch
            {
                // TODO: Handle logging the exception
                return null;
            }
        }
    }
}