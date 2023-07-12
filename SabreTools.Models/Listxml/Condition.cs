using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.Listxml
{
    [XmlRoot("condition")]
    public class Condition
    {
        [XmlAttribute("tag")]
        public string Tag { get; set; }

        [XmlAttribute("mask")]
        public string Mask { get; set; }

        /// <remarks>(eq|ne|gt|le|lt|ge)</remarks>
        [XmlAttribute("relation")]
        public string Relation { get; set; }

        [XmlAttribute("value")]
        public string Value { get; set; }
    }
}