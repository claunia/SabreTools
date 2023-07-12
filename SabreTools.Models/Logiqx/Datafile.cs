using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.Logiqx
{
    [XmlRoot("datafile")]
    public class Datafile
    {
        [XmlAttribute("build")]
        public string? Build { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("debug")]
        public string? Debug { get; set; }

        [XmlElement("header")]
        public Header? Header { get; set; }

        [XmlElement("game")]
        public Game[]? Game { get; set; }

        [XmlElement("machine")]
        public Machine[]? Machine { get; set; }

        #region RomVault Extensions

        /// <remarks>Boolean; Appears after Header</remarks>
        [XmlAttribute("dir")]
        public Dir[]? Dir { get; set; }

        #endregion
    }
}