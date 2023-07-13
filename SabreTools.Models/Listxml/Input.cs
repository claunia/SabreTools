using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.Listxml
{
    [XmlRoot("input")]
    public class Input
    {
        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("service")]
        public string? Service { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("tilt")]
        public string? Tilt { get; set; }

        /// <remarks>Numeric?</remarks>
        [XmlAttribute("players")]
        public string Players { get; set; }

        /// <remarks>Numeric?</remarks>
        [XmlAttribute("coins")]
        public string? Coins { get; set; }

        /// <remarks>Numeric?</remarks>
        [XmlElement("control")]
        public Control[]? Control { get; set; }

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