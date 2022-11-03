using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents the a feature of the machine
    /// </summary>
    [JsonObject("feature"), XmlRoot("feature")]
    public class Feature : DatItem
    {
        #region Fields

        /// <summary>
        /// Type of feature
        /// </summary>
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public FeatureType Type { get; set; }

        [JsonIgnore]
        public bool TypeSpecified { get { return Type != FeatureType.NULL; } }

        /// <summary>
        /// Emulation status
        /// </summary>
        [JsonProperty("status", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public FeatureStatus Status { get; set; }

        [JsonIgnore]
        public bool StatusSpecified { get { return Status != FeatureStatus.NULL; } }

        /// <summary>
        /// Overall status
        /// </summary>
        [JsonProperty("overall", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("overall")]
        [JsonConverter(typeof(StringEnumConverter))]
        public FeatureStatus Overall { get; set; }

        [JsonIgnore]
        public bool OverallSpecified { get { return Overall != FeatureStatus.NULL; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Feature object
        /// </summary>
        public Feature()
        {
            ItemType = ItemType.Feature;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Feature()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Type = this.Type,
                Status = this.Status,
                Overall = this.Overall,
            };
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem other)
        {
            // If we don't have a Feature, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Feature
            Feature newOther = other as Feature;

            // If the Feature information matches
            return (Type == newOther.Type
                && Status == newOther.Status
                && Overall == newOther.Overall);
        }

        #endregion
    }
}
