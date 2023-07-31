using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.Logiqx
{
    /// <summary>
    /// Base class to unify the various game-like types
    /// </summary>
    public abstract class GameBase
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

        /// <remarks>No-Intro extension includes more than 1 instance</remarks>
        [XmlElement("category")]
        public string[]? Category { get; set; }

        [XmlElement(elementName: "release")]
        public Release[]? Release { get; set; }

        [XmlElement("biosset")]
        public BiosSet[]? BiosSet { get; set; }

        [XmlElement("rom")]
        public Rom[]? Rom { get; set; }

        [XmlElement("disk")]
        public Disk[]? Disk { get; set; }

        [XmlElement("media")] // Aaru extension
        public Media[]? Media { get; set; }

        [XmlElement("device_ref")] // MAME extension
        public DeviceRef[]? DeviceRef { get; set; }

        [XmlElement("sample")]
        public Sample[]? Sample { get; set; }

        [XmlElement("archive")]
        public Archive[]? Archive { get; set; }

        [XmlElement("driver")] // MAME extension
        public Driver[]? Driver { get; set; }

        [XmlElement("softwarelist")] // MAME extension
        public SoftwareList[]? SoftwareList { get; set; }

        #region MAME Extensions

        /// <remarks>(yes|no) "no", Appears after IsBios</remarks>
        [XmlAttribute("isdevice")]
        public string? IsDevice { get; set; }

        /// <remarks>(yes|no) "no", Appears after IsDevice</remarks>
        [XmlAttribute("ismechanical")]
        public string? IsMechanical { get; set; }

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