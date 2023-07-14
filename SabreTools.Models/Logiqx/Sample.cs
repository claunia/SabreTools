using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.Logiqx
{
    [XmlRoot("sample")]
    public class Sample : ItemBase
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
    }
}