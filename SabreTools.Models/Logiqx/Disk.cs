using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.Logiqx
{
    [XmlRoot("disk")]
    public class Disk : ItemBase
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("md5")]
        public string? MD5 { get; set; }

        [XmlAttribute("sha1")]
        public string? SHA1 { get; set; }

        [XmlAttribute("merge")]
        public string? Merge { get; set; }

        /// <remarks>(baddump|nodump|good|verified) "good"</remarks>
        [XmlAttribute("status")]
        public string? Status { get; set; }

        #region MAME Extensions

        /// <remarks>Appears after Status</remarks>
        [XmlAttribute("region")]
        public string? Region { get; set; }

        #endregion
    }
}