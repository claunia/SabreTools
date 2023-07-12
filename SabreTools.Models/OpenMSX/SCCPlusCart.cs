using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.OpenMSX
{
    [XmlRoot("sccpluscart")]
    public class SCCPlusCart
    {
        [XmlElement("boot")]
        public string? Boot { get; set; }

        [XmlElement("hash")]
        public string? Hash { get; set; }

        [XmlElement("remark")]
        public string? Remark { get; set; }
    }
}