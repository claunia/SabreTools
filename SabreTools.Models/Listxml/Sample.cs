using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.Listxml
{
    [XmlRoot("sample")]
    public class Sample
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
    }
}