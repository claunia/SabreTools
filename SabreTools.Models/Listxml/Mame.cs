using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.Listxml
{
    [XmlRoot("mame")]
    public class Mame
    {
        [XmlAttribute("build")]
        public string? Build { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("debug")]
        public string? Debug { get; set; }

        [XmlAttribute("mameconfig")]
        public string MameConfig { get; set; }

        [XmlElement("machine")]
        public Machine[] Machine { get; set; }
    }
}