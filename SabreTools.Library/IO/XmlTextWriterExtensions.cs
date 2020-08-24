using System.Xml;

namespace SabreTools.Library.IO
{
    /// <summary>
    /// Additional methods for XmlTextWriter
    /// </summary>
    public static class XmlTextWriterExtensions
    {
        /// <summary>
        /// Write an attribute, forcing empty if null
        /// </summary>
        /// <param name="writer">XmlTextWriter to write out with</param>
        /// <param name="localName">Name of the element</param>
        /// <param name="value">Value to write in the element</param>
        public static void WriteRequiredAttributeString(this XmlTextWriter writer, string localName, string value)
        {
            writer.WriteAttributeString(localName, value ?? string.Empty);
        }

        /// <summary>
        /// Force writing separate open and start tags, even for empty elements
        /// </summary>
        /// <param name="writer">XmlTextWriter to write out with</param>
        /// <param name="localName">Name of the element</param>
        /// <param name="value">Value to write in the element</param>
        public static void WriteRequiredElementString(this XmlTextWriter writer, string localName, string value)
        {
            writer.WriteStartElement(localName);
            writer.WriteRaw(value ?? string.Empty);
            writer.WriteFullEndElement();
        }

        /// <summary>
        /// Write an attribute, if the value is not null or empty
        /// </summary>
        /// <param name="writer">XmlTextWriter to write out with</param>
        /// <param name="localName">Name of the attribute</param>
        /// <param name="value">Value to write in the attribute</param>
        public static void WriteOptionalAttributeString(this XmlTextWriter writer, string localName, string value)
        {
            if (string.IsNullOrEmpty(value))
                writer.WriteAttributeString(localName, value);
        }

        /// <summary>
        /// Write an element, if the value is not null or empty
        /// </summary>
        /// <param name="writer">XmlTextWriter to write out with</param>
        /// <param name="localName">Name of the element</param>
        /// <param name="value">Value to write in the element</param>
        public static void WriteOptionalElementString(this XmlTextWriter writer, string localName, string value)
        {
            if (string.IsNullOrEmpty(value))
                writer.WriteElementString(localName, value);
        }
    }
}
