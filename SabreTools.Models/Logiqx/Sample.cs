using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.Logiqx
{
    [XmlRoot("sample")]
    public class Sample
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
    }
}