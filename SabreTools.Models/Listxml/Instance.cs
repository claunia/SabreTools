using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.Listxml
{
    [XmlRoot("instance")]
    public class Instance
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("briefname")]
        public string BriefName { get; set; }
    }
}