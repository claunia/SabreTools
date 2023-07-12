using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.Listxml
{
    [XmlRoot("extension")]
    public class Extension
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
    }
}