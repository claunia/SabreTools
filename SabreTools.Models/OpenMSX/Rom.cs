using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.OpenMSX
{
    [XmlRoot("rom")]
    public class Rom
    {
        [XmlElement("start")]
        public string? Start { get; set; }

        [XmlElement("type")]
        public string? Type { get; set; }

        [XmlElement("hash")]
        public string? Hash { get; set; }

        [XmlElement("remark")]
        public string? Remark { get; set; }
    }
}