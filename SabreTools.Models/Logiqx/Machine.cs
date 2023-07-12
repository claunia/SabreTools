using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.Logiqx
{
    [XmlRoot("machine")]
    public class Machine
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("sourcefile")]
        public string? SourceFile { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("isbios")]
        public string? IsBios { get; set; }

        [XmlAttribute("cloneof")]
        public string? CloneOf { get; set; }

        [XmlAttribute("romof")]
        public string? RomOf { get; set; }

        [XmlAttribute("sampleof")]
        public string? SampleOf { get; set; }

        [XmlAttribute("board")]
        public string? Board { get; set; }

        [XmlAttribute("rebuildto")]
        public string? RebuildTo { get; set; }

        /// <remarks>(no|partial|yes) "no"</remarks>
        [XmlAttribute("runnable")]
        public string? Runnable { get; set; }

        [XmlElement("comment")]
        public string[]? Comment { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlElement("year")]
        public string? Year { get; set; }

        [XmlElement("manufacturer")]
        public string? Manufacturer { get; set; }

        [XmlElement("publisher")]
        public string? Publisher { get; set; }

        [XmlElement("category")]
        public string? Category { get; set; }

        [XmlElement("release")]
        public Release[]? Release { get; set; }

        [XmlElement("biosset")]
        public BiosSet[]? BiosSet { get; set; }

        [XmlElement("rom")]
        public Rom[]? Rom { get; set; }

        [XmlElement("disk")]
        public Disk[]? Disk { get; set; }

        [XmlElement("sample")]
        public Sample[]? Sample { get; set; }

        [XmlElement("archive")]
        public Archive[]? Archive { get; set; }

        #region Aaru Extensions

        /// <remarks>Appears after Disk</remarks>
        [XmlElement("media")]
        public Media[]? Media { get; set; }

        #endregion

        #region No-Intro Extensions

        /// <remarks>Appears after RebuildTo</remarks>
        [XmlAttribute("id")]
        public string? Id { get; set; }

        /// <remarks>Appears after Id</remarks>
        [XmlAttribute("cloneofid")]
        public string? CloneOfId { get; set; }

        #endregion

        #region Trurip Extensions

        /// <remarks>Appears after Category</remarks>
        [XmlElement("trurip")]
        public Trurip? Trurip { get; set; }

        #endregion
    }
}