using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SabreTools.Core;
using SabreTools.Core.Tools;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents control for an input
    /// </summary>
    [JsonObject("control"), XmlRoot("control")]
    public class Control : DatItem
    {
        #region Fields

        /// <summary>
        /// General type of input
        /// </summary>
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ControlType ControlType
        {
            get => _internal.ReadString(Models.Internal.Control.ControlTypeKey).AsControlType();
            set => _internal[Models.Internal.Control.ControlTypeKey] = value.FromControlType();
        }

        [JsonIgnore]
        public bool ControlTypeSpecified { get { return ControlType != ControlType.NULL; } }

        /// <summary>
        /// Player which the input belongs to
        /// </summary>
        [JsonProperty("player", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("player")]
        public long? Player
        {
            get => _internal.ReadLong(Models.Internal.Control.PlayerKey);
            set => _internal[Models.Internal.Control.PlayerKey] = value;
        }

        [JsonIgnore]
        public bool PlayerSpecified { get { return Player != null; } }

        /// <summary>
        /// Total number of buttons
        /// </summary>
        [JsonProperty("buttons", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("buttons")]
        public long? Buttons
        {
            get => _internal.ReadLong(Models.Internal.Control.ButtonsKey);
            set => _internal[Models.Internal.Control.ButtonsKey] = value;
        }

        [JsonIgnore]
        public bool ButtonsSpecified { get { return Buttons != null; } }

        /// <summary>
        /// Total number of non-optional buttons
        /// </summary>
        [JsonProperty("reqbuttons", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("reqbuttons")]
        public long? RequiredButtons
        {
            get => _internal.ReadLong(Models.Internal.Control.ReqButtonsKey);
            set => _internal[Models.Internal.Control.ReqButtonsKey] = value;
        }

        [JsonIgnore]
        public bool RequiredButtonsSpecified { get { return RequiredButtons != null; } }

        /// <summary>
        /// Analog minimum value
        /// </summary>
        [JsonProperty("minimum", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("minimum")]
        public long? Minimum
        {
            get => _internal.ReadLong(Models.Internal.Control.MinimumKey);
            set => _internal[Models.Internal.Control.MinimumKey] = value;
        }

        [JsonIgnore]
        public bool MinimumSpecified { get { return Minimum != null; } }

        /// <summary>
        /// Analog maximum value
        /// </summary>
        [JsonProperty("maximum", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("maximum")]
        public long? Maximum
        {
            get => _internal.ReadLong(Models.Internal.Control.MaximumKey);
            set => _internal[Models.Internal.Control.MaximumKey] = value;
        }

        [JsonIgnore]
        public bool MaximumSpecified { get { return Maximum != null; } }

        /// <summary>
        /// Default analog sensitivity
        /// </summary>
        [JsonProperty("sensitivity", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("sensitivity")]
        public long? Sensitivity
        {
            get => _internal.ReadLong(Models.Internal.Control.SensitivityKey);
            set => _internal[Models.Internal.Control.SensitivityKey] = value;
        }

        [JsonIgnore]
        public bool SensitivitySpecified { get { return Sensitivity != null; } }

        /// <summary>
        /// Default analog keydelta
        /// </summary>
        [JsonProperty("keydelta", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("keydelta")]
        public long? KeyDelta
        {
            get => _internal.ReadLong(Models.Internal.Control.KeyDeltaKey);
            set => _internal[Models.Internal.Control.KeyDeltaKey] = value;
        }

        [JsonIgnore]
        public bool KeyDeltaSpecified { get { return KeyDelta != null; } }

        /// <summary>
        /// Default analog reverse setting
        /// </summary>
        [JsonProperty("reverse", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("reverse")]
        public bool? Reverse
        {
            get => _internal.ReadBool(Models.Internal.Control.ReverseKey);
            set => _internal[Models.Internal.Control.ReverseKey] = value;
        }

        [JsonIgnore]
        public bool ReverseSpecified { get { return Reverse != null; } }

        /// <summary>
        /// First set of ways
        /// </summary>
        [JsonProperty("ways", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("ways")]
        public string? Ways
        {
            get => _internal.ReadString(Models.Internal.Control.WaysKey);
            set => _internal[Models.Internal.Control.WaysKey] = value;
        }

        /// <summary>
        /// Second set of ways
        /// </summary>
        [JsonProperty("ways2", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("ways2")]
        public string? Ways2
        {
            get => _internal.ReadString(Models.Internal.Control.Ways2Key);
            set => _internal[Models.Internal.Control.Ways2Key] = value;
        }

        /// <summary>
        /// Third set of ways
        /// </summary>
        [JsonProperty("ways3", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("ways3")]
        public string? Ways3
        {
            get => _internal.ReadString(Models.Internal.Control.Ways3Key);
            set => _internal[Models.Internal.Control.Ways3Key] = value;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Control object
        /// </summary>
        public Control()
        {
            _internal = new Models.Internal.Control();
            ItemType = ItemType.Control;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Control()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine?.Clone() as Machine,
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Internal.Control ?? new Models.Internal.Control(),
            };
        }

        #endregion
    }
}
