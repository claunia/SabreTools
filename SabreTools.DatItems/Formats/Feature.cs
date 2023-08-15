using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SabreTools.Core;
using SabreTools.Core.Tools;

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
            get => _internal.ReadString(Models.Internal.Feature.FeatureTypeKey).AsFeatureType();
            set => _internal[Models.Internal.Feature.FeatureTypeKey] = value.FromFeatureType();
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
            get => _internal.ReadString(Models.Internal.Feature.StatusKey).AsFeatureStatus();
            set => _internal[Models.Internal.Feature.StatusKey] = value.FromFeatureStatus();
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
            get => _internal.ReadString(Models.Internal.Feature.OverallKey).AsFeatureStatus();
            set => _internal[Models.Internal.Feature.OverallKey] = value.FromFeatureStatus();
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
            _internal = new Models.Internal.Feature();
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

                Machine = this.Machine?.Clone() as Machine,
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Internal.Feature ?? new Models.Internal.Feature(),
            };
        }

        #endregion
    }
}
