using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.Listxml
{
    [XmlRoot("analog")]
    public class Analog
    {
        [XmlAttribute("mask")]
        public string Mask { get; set; }
    }
}