using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.OpenMSX
{
    [XmlRoot("software")]
    public class Software
    {
        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("genmsxid")]
        public string? GenMSXID { get; set; }

        [XmlElement("system")]
        public string System { get; set; }

        [XmlElement("company")]
        public string Company { get; set; }

        [XmlElement("year")]
        public string Year { get; set; }

        [XmlElement("country")]
        public string Country { get; set; }

        [XmlElement("dump")]
        public Dump[]? Dump { get; set; }
    }
}