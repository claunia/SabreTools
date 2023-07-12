using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.Listxml
{
    [XmlRoot("slotoption")]
    public class SlotOption
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("devname")]
        public string DevName { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("default")]
        public string? Default { get; set; }
    }
}