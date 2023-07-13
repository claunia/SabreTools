using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.OpenMSX
{
    [XmlRoot("dump")]
    public class Dump
    {
        [XmlElement("original")]
        public Original? Original { get; set; }

        [XmlElement("rom")]
        public Rom? Rom { get; set; }

        [XmlElement("megarom")]
        public MegaRom? MegaRom { get; set; }

        [XmlElement("sccpluscart")]
        public SCCPlusCart? SCCPlusCart { get; set; }

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