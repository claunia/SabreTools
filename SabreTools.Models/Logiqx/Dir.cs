using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.Logiqx
{
    [XmlRoot("dir")]
    public class Dir
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("machine")]
        public Game[]? Game { get; set; }

        [XmlAttribute("machine")]
        public Machine[]? Machine { get; set; }
    }
}