using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

// TODO: Add item mappings for all fields
namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents a single details item
    /// </summary>
    [JsonObject("details"), XmlRoot("details")]
    public class Details : DatItem
    {
        #region Fields

        /// <summary>
        /// Id value
        /// </summary>
        /// <remarks>TODO: Is this required?</remarks>
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("id")]
        public string Id { get; set; }

        /// <summary>
        /// Section value
        /// </summary>
        [JsonProperty("section", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("section")]
        public string Section { get; set; }

        /// <summary>
        /// Dumping date value
        /// </summary>
        [JsonProperty("d_date", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("d_date")]
        public string DDate { get; set; }

        /// <summary>
        /// Directory name value
        /// </summary>
        [JsonProperty("dirname", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("dirname")]
        public string DirName { get; set; }

        /// <summary>
        /// NFO name value
        /// </summary>
        [JsonProperty("nfoname", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("nfoname")]
        public string NfoName { get; set; }

        /// <summary>
        /// NFO size value
        /// </summary>
        [JsonProperty("nfosize", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("nfosize")]
        public long? NfoSize { get; set; }

        [JsonIgnore]
        public bool NfoSizeSpecified { get { return NfoSize != null; } }

        /// <summary>
        /// NFO CRC value
        /// </summary>
        [JsonProperty("nfocrc", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("nfocrc")]
        public string NfoCrc { get; set; }

        /// <summary>
        /// Archive name value
        /// </summary>
        [JsonProperty("archivename", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("archivename")]
        public string ArchiveName { get; set; }

        /// <summary>
        /// Date value
        /// </summary>
        [JsonProperty("date", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("date")]
        public string Date { get; set; }

        /// <summary>
        /// Grpup value
        /// </summary>
        [JsonProperty("group", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("group")]
        public string Group { get; set; }

        /// <summary>
        /// Region value
        /// </summary>
        [JsonProperty("region", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("region")]
        public string Region { get; set; }

        /// <summary>
        /// Media title value
        /// </summary>
        [JsonProperty("media_title", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("media_title")]
        public string MediaTitle { get; set; }

        /// <summary>
        /// Dumper value
        /// </summary>
        [JsonProperty("dumper", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("dumper")]
        public string Dumper { get; set; }

        /// <summary>
        /// Project value
        /// </summary>
        [JsonProperty("project", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("project")]
        public string Project { get; set; }

        /// <summary>
        /// Original format value
        /// </summary>
        [JsonProperty("originalformat", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("originalformat")]
        public string OriginalFormat { get; set; }

        /// <summary>
        /// Tool value
        /// </summary>
        [JsonProperty("tool", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("tool")]
        public string Tool { get; set; }

        /// <summary>
        /// Comment 1 value
        /// </summary>
        [JsonProperty("comment1", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("comment1")]
        public string Comment1 { get; set; }

        /// <summary>
        /// Link 2 value
        /// </summary>
        [JsonProperty("comment2", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("comment2")]
        public string Comment2 { get; set; }

        /// <summary>
        /// Link 1 value
        /// </summary>
        [JsonProperty("link1", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("link1")]
        public string Link1 { get; set; }

        /// <summary>
        /// Link 2 value
        /// </summary>
        [JsonProperty("link2", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("link2")]
        public string Link2 { get; set; }

        /// <summary>
        /// Link 3 value
        /// </summary>
        [JsonProperty("link3", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("link3")]
        public string Link3 { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Details object
        /// </summary>
        public Details()
        {
            ItemType = ItemType.Serials;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Details()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Id = this.Id,
                Section = this.Section,
                DDate = this.DDate,
                DirName = this.DirName,
                NfoName = this.NfoName,
                NfoSize = this.NfoSize,
                NfoCrc = this.NfoCrc,
                ArchiveName = this.ArchiveName,
                Date = this.Date,
                Group = this.Group,
                Region = this.Region,
                MediaTitle = this.MediaTitle,
                Dumper = this.Dumper,
                Project = this.Project,
                OriginalFormat = this.OriginalFormat,
                Tool = this.Tool,
                Comment1 = this.Comment1,
                Comment2 = this.Comment2,
                Link1 = this.Link1,
                Link2 = this.Link2,
                Link3 = this.Link3,
            };
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem other)
        {
            // If we don't have a Details, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Details
            Details newOther = other as Details;

            // If the Details information matches
            return (Id == newOther.Id
                && Section == newOther.Section
                && DDate == newOther.DDate
                && DirName == newOther.DirName
                && NfoName == newOther.NfoName
                && NfoSize == newOther.NfoSize
                && NfoCrc == newOther.NfoCrc
                && ArchiveName == newOther.ArchiveName
                && Date == newOther.Date
                && Group == newOther.Group
                && Region == newOther.Region
                && MediaTitle == newOther.MediaTitle
                && Dumper == newOther.Dumper
                && Project == newOther.Project
                && OriginalFormat == newOther.OriginalFormat
                && Tool == newOther.Tool
                && Comment1 == newOther.Comment1
                && Comment2 == newOther.Comment2
                && Link1 == newOther.Link1
                && Link2 == newOther.Link2
                && Link3 == newOther.Link3);
        }

        #endregion
    }
}
