using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.OpenMSX
{
    [XmlRoot("dump")]
    public class Dump
    {
        [XmlElement("original")]
        public Original? Original { get; set; }

        [XmlElement("rom")]
        public Rom? Rom { get; set; }

        [XmlElement("megarom")]
        public MegaRom? MegaRom { get; set; }

        [XmlElement("sccpluscart")]
        public SCCPlusCart? SCCPlusCart { get; set; }
    }
}