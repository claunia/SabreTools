using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.Filter;

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
        public FeatureType Type
        {
            get => _internal.ReadString(Models.Metadata.Feature.FeatureTypeKey).AsEnumValue<FeatureType>();
            set => _internal[Models.Metadata.Feature.FeatureTypeKey] = value.AsStringValue<FeatureType>();
        }

        [JsonIgnore]
        public bool TypeSpecified { get { return Type != FeatureType.NULL; } }

        /// <summary>
        /// Emulation status
        /// </summary>
        [JsonProperty("status", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public FeatureStatus Status
        {
            get => _internal.ReadString(Models.Metadata.Feature.StatusKey).AsEnumValue<FeatureStatus>();
            set => _internal[Models.Metadata.Feature.StatusKey] = value.AsStringValue<FeatureStatus>();
        }

        [JsonIgnore]
        public bool StatusSpecified { get { return Status != FeatureStatus.NULL; } }

        /// <summary>
        /// Overall status
        /// </summary>
        [JsonProperty("overall", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("overall")]
        [JsonConverter(typeof(StringEnumConverter))]
        public FeatureStatus Overall
        {
            get => _internal.ReadString(Models.Metadata.Feature.OverallKey).AsEnumValue<FeatureStatus>();
            set => _internal[Models.Metadata.Feature.OverallKey] = value.AsStringValue<FeatureStatus>();
        }

        [JsonIgnore]
        public bool OverallSpecified { get { return Overall != FeatureStatus.NULL; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Feature object
        /// </summary>
        public Feature()
        {
            _internal = new Models.Metadata.Feature();
            Machine = new Machine();

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

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.Feature ?? [],
            };
        }

        #endregion
    }
}
