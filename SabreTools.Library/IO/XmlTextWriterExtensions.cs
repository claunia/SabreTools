using System.Xml;

namespace SabreTools.Library.IO
{
    /// <summary>
    /// Additional methods for XmlTextWriter
    /// </summary>
    public static class XmlTextWriterExtensions
    {
        /// <summary>
        /// Force writing separate open and start tags, even for empty elements
        /// </summary>
        /// <param name="xmlTextWriter">XmlTextWriter to write out with</param>
        /// <param name="localName">Name of the element</param>
        /// <param name="value">Value to write in the element</param>
        public static void WriteFullElementString(this XmlTextWriter xmlTextWriter, string localName, string value)
        {
            xmlTextWriter.WriteStartElement(localName);
            xmlTextWriter.WriteRaw(value);
            xmlTextWriter.WriteFullEndElement();
        }
    }
}
