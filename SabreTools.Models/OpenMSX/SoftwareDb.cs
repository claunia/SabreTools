using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.OpenMSX
{
    [XmlRoot("softwaredb")]
    public class SoftwareDb
    {
        [XmlAttribute("timestamp")]
        public string? Timestamp { get; set; }

        [XmlAttribute("software")]
        public Software[]? Software { get; set; }
    }
}