using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.Listxml
{
    [XmlRoot("conflocation")]
    public class ConfLocation
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <remarks>Numeric?</remarks>
        [XmlAttribute("number")]
        public string Number { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("inverted")]
        public string? Inverted { get; set; }
    }
}