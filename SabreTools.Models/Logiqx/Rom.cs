using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.Logiqx
{
    [XmlRoot("rom")]
    public class Rom
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("size")]
        public long Size { get; set; }

        [XmlAttribute("crc")]
        public string? CRC { get; set; }

        [XmlAttribute("md5")]
        public string? MD5 { get; set; }

        [XmlAttribute("sha1")]
        public string? SHA1 { get; set; }

        [XmlAttribute("merge")]
        public string? Merge { get; set; }

        /// <remarks>(baddump|nodump|good|verified) "good"</remarks>
        [XmlAttribute("status")]
        public string? Status { get; set; }

        [XmlAttribute("date")]
        public string? Date { get; set; }

        #region Hash Extensions

        /// <remarks>Also in No-Intro spec; Appears after SHA1</remarks>
        [XmlAttribute("sha256")]
        public string? SHA256 { get; set; }

        /// <remarks>Appears after SHA256</remarks>
        [XmlAttribute("sha384")]
        public string? SHA384 { get; set; }

        /// <remarks>Appears after SHA384</remarks>
        [XmlAttribute("sha512")]
        public string? SHA512 { get; set; }

        /// <remarks>Appears after SHA512</remarks>
        [XmlAttribute("spamsum")]
        public string? SpamSum { get; set; }

        #endregion

        #region DiscImgeCreator Extensions

        /// <remarks>Appears after SpamSum</remarks>
        [XmlAttribute("xxh3_64")]
        public string? xxHash364 { get; set; }

        /// <remarks>Appears after xxHash364</remarks>
        [XmlAttribute("xxh3_128")]
        public string? xxHash3128 { get; set; }

        #endregion

        #region No-Intro Extensions

        /// <remarks>Appears after Status</remarks>
        [XmlAttribute("serial")]
        public string? Serial { get; set; }

        /// <remarks>Appears after Serial</remarks>
        [XmlAttribute("header")]
        public string? Header { get; set; }

        #endregion

        #region RomVault Extensions

        /// <remarks>Boolean; Appears after Date</remarks>
        [XmlAttribute("inverted")]
        public bool? Inverted { get; set; }

        /// <remarks>Boolean; Appears after Inverted</remarks>
        [XmlAttribute("mia")]
        public bool? MIA { get; set; }

        #endregion
    }
}