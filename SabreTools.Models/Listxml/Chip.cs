using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.Listxml
{
    [XmlRoot("chip")]
    public class Chip
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("tag")]
        public string? Tag { get; set; }

        /// <remarks>(cpu|audio)</remarks>
        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("clock")]
        public string? Clock { get; set; }
    }
}