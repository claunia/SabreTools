using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.Logiqx
{
    [XmlRoot("header")]
    public class Header
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlElement("category")]
        public string? Category { get; set; }

        [XmlElement("version")]
        public string Version { get; set; }

        [XmlElement("date")]
        public string? Date { get; set; }

        [XmlElement("author")]
        public string Author { get; set; }

        [XmlElement("email")]
        public string? Email { get; set; }

        [XmlElement("homepage")]
        public string? Homepage { get; set; }

        [XmlElement("url")]
        public string? Url { get; set; }

        [XmlElement("comment")]
        public string? Comment { get; set; }

        [XmlElement("clrmamepro")]
        public ClrMamePro? ClrMamePro { get; set; }

        [XmlElement("romcenter")]
        public RomCenter? RomCenter { get; set; }

        #region No-Intro Extensions

        /// <remarks>Appears at very top</remarks>
        [XmlElement("id")]
        public string? Id { get; set; }

        #endregion

        #region Trurip Extensions

        /// <remarks>Appears after Description</remarks>
        [XmlElement("rootdir")]
        public string? RootDir { get; set; }

        /// <remarks>Appears after Comment</remarks>
        [XmlElement("type")]
        public string? Type { get; set; }

        #endregion
    }
}