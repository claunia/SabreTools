using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.SoftwareList
{
    [XmlRoot("dataarea")]
    public class DataArea
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("size")]
        public long Size { get; set; }
        
        /// <remarks>(8|16|32|64) "8"</remarks>
        [XmlAttribute("width")]
        public long? Width { get; set; }
        
        /// <remarks>(big|little) "little"</remarks>
        [XmlAttribute("endianness")]
        public string? Endianness { get; set; }

        [XmlElement("rom")]
        public Rom[]? Rom { get; set; }
    }
}