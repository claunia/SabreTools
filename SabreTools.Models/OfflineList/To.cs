using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.OfflineList
{
    [XmlRoot("to")]
    public class To
    {
        [XmlAttribute("value")]
        public string? Value { get; set; }

        [XmlAttribute("default")]
        public bool? Default { get; set; }

        [XmlAttribute("auto")]
        public bool? Auto { get; set; }

        [XmlElement("find")]
        public Find[]? Find { get; set; }
    }
}