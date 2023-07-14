using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.Logiqx
{
    /// <summary>
    /// Base class to unify the various item types
    /// </summary>
    public abstract class ItemBase
    {
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