using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.SoftwareList
{
    [XmlRoot("software")]
    public class Software
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("cloneof")]
        public string? CloneOf { get; set; }

        /// <remarks>(yes|partial|no) "yes"</remarks>
        [XmlAttribute("supported")]
        public string? Supported { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlElement("year")]
        public string Year { get; set; }

        [XmlElement("publisher")]
        public string Publisher { get; set; }

        [XmlElement("notes")]
        public string? Notes { get; set; }

        [XmlElement("info")]
        public Info[]? Info { get; set; }

        [XmlElement("sharedfeat")]
        public SharedFeat[]? SharedFeat { get; set; }

        [XmlElement("part")]
        public Part[]? Part { get; set; }

        #region DO NOT USE IN PRODUCTION

        /// <remarks>Should be empty</remarks>
        [XmlAnyAttribute]
        public XmlAttribute[]? ADDITIONAL_ATTRIBUTES { get; set; }

        /// <remarks>Should be empty</remarks>
        [XmlAnyElement]
        public object[]? ADDITIONAL_ELEMENTS { get; set; }

        #endregion
    }
}