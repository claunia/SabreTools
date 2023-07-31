using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Serialization
{
    /// <summary>
    /// XML serializer for nullable types
    /// </summary>
    public abstract partial class XmlSerializer<T>
    {
        /// <summary>
        /// Serializes the defined type to an XML file
        /// </summary>
        /// <param name="obj">Data to serialize</param>
        /// <param name="path">Path to the file to serialize to</param>
        /// <returns>True on successful serialization, false otherwise</returns>
        public static bool SerializeToFile(T? obj, string path)
        {
            using var stream = SerializeToStream(obj);
            if (stream == null)
                return false;

            using var fs = File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }

        /// <summary>
        /// Serializes the defined type to a stream
        /// </summary>
        /// <param name="obj">Data to serialize</param>
        /// <returns>Stream containing serialized data on success, null otherwise</returns>
        public static Stream? SerializeToStream(T? obj)
        {
            // If the object is null
            if (obj == null)
                return null;

            // Setup the serializer and the reader
            var serializer = new XmlSerializer(typeof(T));
            var settings = new XmlWriterSettings
            {
                CheckCharacters = false,
                Encoding = Encoding.UTF8,
                Indent = true,
                NewLineChars = "\n",
            };
            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);
            var xmlWriter = XmlWriter.Create(streamWriter, settings);

            // Perform the deserialization and return
            serializer.Serialize(xmlWriter, obj);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }
}