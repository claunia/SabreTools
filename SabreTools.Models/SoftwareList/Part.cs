using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.SoftwareList
{
    [XmlRoot("part")]
    public class Part
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("interface")]
        public string Interface { get; set; }

        [XmlElement("feature")]
        public Feature[]? Feature { get; set; }

        [XmlElement("dataarea")]
        public DataArea[]? DataArea { get; set; }

        [XmlElement("diskarea")]
        public DiskArea[]? DiskArea { get; set; }

        [XmlElement("dipswitch")]
        public DipSwitch[]? DipSwitch { get; set; }
    }
}