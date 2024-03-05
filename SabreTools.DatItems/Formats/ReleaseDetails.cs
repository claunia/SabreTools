using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;
using SabreTools.Filter;

// TODO: Add item mappings for all fields
namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents a single release details item
    /// </summary>
    [JsonObject("release_details"), XmlRoot("release_details")]
    public class ReleaseDetails : DatItem
    {
        #region Fields

        /// <summary>
        /// Id value
        /// </summary>
        /// <remarks>TODO: Is this required?</remarks>
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("id")]
        public string? Id { get; set; }

        /// <summary>
        /// Directory name value
        /// </summary>
        [JsonProperty("dirname", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("dirname")]
        public string? DirName { get; set; }

        /// <summary>
        /// Rom info value
        /// </summary>
        [JsonProperty("rominfo", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("rominfo")]
        public string? RomInfo { get; set; }

        /// <summary>
        /// Category value
        /// </summary>
        [JsonProperty("category", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("category")]
        public string? Category { get; set; }

        /// <summary>
        /// NFO name value
        /// </summary>
        [JsonProperty("nfoname", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("nfoname")]
        public string? NfoName { get; set; }

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
        public string? NfoCrc { get; set; }

        /// <summary>
        /// Archive name value
        /// </summary>
        [JsonProperty("archivename", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("archivename")]
        public string? ArchiveName { get; set; }

        /// <summary>
        /// Original format value
        /// </summary>
        [JsonProperty("originalformat", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("originalformat")]
        public string? OriginalFormat { get; set; }

        /// <summary>
        /// Date value
        /// </summary>
        [JsonProperty("date", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("date")]
        public string? Date { get; set; }

        /// <summary>
        /// Grpup value
        /// </summary>
        [JsonProperty("group", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("group")]
        public string? Group { get; set; }

        /// <summary>
        /// Comment value
        /// </summary>
        [JsonProperty("comment", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("comment")]
        public string? Comment { get; set; }

        /// <summary>
        /// Tool value
        /// </summary>
        [JsonProperty("tool", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("tool")]
        public string? Tool { get; set; }

        /// <summary>
        /// Region value
        /// </summary>
        [JsonProperty("region", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("region")]
        public string? Region { get; set; }

        /// <summary>
        /// Origin value
        /// </summary>
        [JsonProperty("origin", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("origin")]
        public string? Origin { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty ReleaseDetails object
        /// </summary>
        public ReleaseDetails()
        {
            ItemType = ItemType.ReleaseDetails;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new ReleaseDetails()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                Id = this.Id,
                DirName = this.DirName,
                RomInfo = this.RomInfo,
                Category = this.Category,
                NfoName = this.NfoName,
                NfoSize = this.NfoSize,
                NfoCrc = this.NfoCrc,
                ArchiveName = this.ArchiveName,
                OriginalFormat = this.OriginalFormat,
                Date = this.Date,
                Group = this.Group,
                Comment = this.Comment,
                Tool = this.Tool,
                Region = this.Region,
                Origin = this.Origin,
            };
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If we don't have a Details, return false
            if (ItemType != other?.ItemType)
                return false;

            // Otherwise, treat it as a Details
            ReleaseDetails? newOther = other as ReleaseDetails;

            // If the Details information matches
            return (Id == newOther!.Id
                && DirName == newOther.DirName
                && RomInfo == newOther.RomInfo
                && Category == newOther.Category
                && NfoName == newOther.NfoName
                && NfoSize == newOther.NfoSize
                && NfoCrc == newOther.NfoCrc
                && ArchiveName == newOther.ArchiveName
                && OriginalFormat == newOther.OriginalFormat
                && Date == newOther.Date
                && Group == newOther.Group
                && Comment == newOther.Comment
                && Tool == newOther.Tool
                && Region == newOther.Region
                && Origin == newOther.Origin);
        }

        #endregion

        #region Manipulation

        /// <inheritdoc/>
        public override bool SetField(DatItemField datItemField, string value)
        {
           // Get the correct internal field name
            string? fieldName = datItemField switch
            {
                // TODO: Figure out what fields go here
                _ => null,
            };

            // Set the field and return
            return FieldManipulator.SetField(_internal, fieldName, value);
        }

        #endregion
    }
}
