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

        [XmlElement("release", typeof(Release))]
        [XmlElement("biosset", typeof(BiosSet))]
        [XmlElement("rom", typeof(Rom))]
        [XmlElement("disk", typeof(Disk))]
        [XmlElement("media", typeof(Media))] // Aaru extension
        [XmlElement("device_ref", typeof(DeviceRef))] // MAME extension
        [XmlElement("sample", typeof(Sample))]
        [XmlElement("archive", typeof(Archive))]
        [XmlElement("driver", typeof(Driver))] // MAME extension
        [XmlElement("softwarelist", typeof(SoftwareList))] // MAME extension
        public ItemBase[]? Item { get; set; }

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