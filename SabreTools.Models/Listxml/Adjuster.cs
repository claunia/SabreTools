using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.Listxml
{
    [XmlRoot("adjuster")]
    public class Adjuster
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("default")]
        public string Default { get; set; }

        [XmlElement("condition")]
        public Condition? Condition { get; set; }
    }
}