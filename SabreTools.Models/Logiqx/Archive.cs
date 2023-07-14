using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.Logiqx
{
    [XmlRoot("archive")]
    public class Archive : ItemBase
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
    }
}