using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents generic archive files to be included in a set
    /// </summary>
    [JsonObject("archive"), XmlRoot("archive")]
    public class Archive : DatItem
    {
        #region Fields

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name"), XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Archive ID number
        /// </summary>
        /// <remarks>No-Intro database export only</remarks>
        [JsonProperty("number"), XmlElement("number")]
        public string Number { get; set; }

        /// <summary>
        /// Clone value
        /// </summary>
        /// <remarks>No-Intro database export only</remarks>
        [JsonProperty("clone"), XmlElement("clone")]
        public string CloneValue { get; set; }

        /// <summary>
        /// Regional parent value
        /// </summary>
        /// <remarks>No-Intro database export only</remarks>
        [JsonProperty("regparent"), XmlElement("regparent")]
        public string RegParent { get; set; }

        /// <summary>
        /// Region value
        /// </summary>
        /// <remarks>No-Intro database export only</remarks>
        [JsonProperty("region"), XmlElement("region")]
        public string Region { get; set; }

        /// <summary>
        /// Languages value
        /// </summary>
        /// <remarks>No-Intro database export only</remarks>
        [JsonProperty("languages"), XmlElement("languages")]
        public string Languages { get; set; }

        /// <summary>
        /// Development status value
        /// </summary>
        /// <remarks>No-Intro database export only</remarks>
        [JsonProperty("devstatus"), XmlElement("devstatus")]
        public string DevStatus { get; set; }

        /// <summary>
        /// Physical value
        /// </summary>
        /// <remarks>No-Intro database export only</remarks>
        /// <remarks>TODO: Is this numeric or a flag?</remarks>
        [JsonProperty("physical"), XmlElement("physical")]
        public string Physical { get; set; }

        /// <summary>
        /// Complete value
        /// </summary>
        /// <remarks>No-Intro database export only</remarks>
        /// <remarks>TODO: Is this numeric or a flag?</remarks>
        [JsonProperty("complete"), XmlElement("complete")]
        public string Complete { get; set; }

        /// <summary>
        /// Categories value
        /// </summary>
        /// <remarks>No-Intro database export only</remarks>
        [JsonProperty("categories"), XmlElement("categories")]
        public string Categories { get; set; }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string name) => Name = name;
        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Archive object
        /// </summary>
        public Archive()
        {
            Name = string.Empty;
            ItemType = ItemType.Archive;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Archive()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Name = this.Name,
                Number = this.Number,
                CloneValue = this.CloneValue,
                RegParent = this.RegParent,
                Region = this.Region,
                Languages = this.Languages,
                DevStatus = this.DevStatus,
                Physical = this.Physical,
                Complete = this.Complete,
                Categories = this.Categories,
            };
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem other)
        {
            // If we don't have an archive, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as an archive
            Archive newOther = other as Archive;

            // If the archive information matches
            return (Name == newOther.Name
                && Number == newOther.Number
                && CloneValue == newOther.CloneValue
                && RegParent == newOther.RegParent
                && Region == newOther.Region
                && Languages == newOther.Languages
                && DevStatus == newOther.DevStatus
                && Physical == newOther.Physical
                && Complete == newOther.Complete
                && Categories == newOther.Categories);
        }

        #endregion
    }
}
