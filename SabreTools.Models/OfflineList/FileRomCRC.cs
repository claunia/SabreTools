using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.OfflineList
{
    [XmlRoot("romCRC")]
    public class FileRomCRC
    {
        [XmlAttribute("extension")]
        public string? Extension { get; set; }

        public string? Content { get; set; }
    }
}