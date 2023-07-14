using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.Logiqx
{
    [XmlRoot("softwarelist")]
    public class SoftwareList : ItemBase
    {
        [XmlAttribute("tag")]
        public string Tag { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <remarks>(original|compatible)</remarks>
        [XmlAttribute("status")]
        public string Status { get; set; }

        [XmlAttribute("filter")]
        public string? Filter { get; set; }
    }
}