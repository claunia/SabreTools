using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.Filter;

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
            get => _internal.ReadString(Models.Metadata.Control.ControlTypeKey).AsEnumValue<ControlType>();
            set => _internal[Models.Metadata.Control.ControlTypeKey] = value.AsStringValue<ControlType>();
        }

        [JsonIgnore]
        public bool ControlTypeSpecified { get { return ControlType != ControlType.NULL; } }

        /// <summary>
        /// Player which the input belongs to
        /// </summary>
        [JsonProperty("player", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("player")]
        public long? Player
        {
            get => _internal.ReadLong(Models.Metadata.Control.PlayerKey);
            set => _internal[Models.Metadata.Control.PlayerKey] = value;
        }

        [JsonIgnore]
        public bool PlayerSpecified { get { return Player != null; } }

        /// <summary>
        /// Total number of buttons
        /// </summary>
        [JsonProperty("buttons", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("buttons")]
        public long? Buttons
        {
            get => _internal.ReadLong(Models.Metadata.Control.ButtonsKey);
            set => _internal[Models.Metadata.Control.ButtonsKey] = value;
        }

        [JsonIgnore]
        public bool ButtonsSpecified { get { return Buttons != null; } }

        /// <summary>
        /// Total number of non-optional buttons
        /// </summary>
        [JsonProperty("reqbuttons", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("reqbuttons")]
        public long? RequiredButtons
        {
            get => _internal.ReadLong(Models.Metadata.Control.ReqButtonsKey);
            set => _internal[Models.Metadata.Control.ReqButtonsKey] = value;
        }

        [JsonIgnore]
        public bool RequiredButtonsSpecified { get { return RequiredButtons != null; } }

        /// <summary>
        /// Analog minimum value
        /// </summary>
        [JsonProperty("minimum", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("minimum")]
        public long? Minimum
        {
            get => _internal.ReadLong(Models.Metadata.Control.MinimumKey);
            set => _internal[Models.Metadata.Control.MinimumKey] = value;
        }

        [JsonIgnore]
        public bool MinimumSpecified { get { return Minimum != null; } }

        /// <summary>
        /// Analog maximum value
        /// </summary>
        [JsonProperty("maximum", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("maximum")]
        public long? Maximum
        {
            get => _internal.ReadLong(Models.Metadata.Control.MaximumKey);
            set => _internal[Models.Metadata.Control.MaximumKey] = value;
        }

        [JsonIgnore]
        public bool MaximumSpecified { get { return Maximum != null; } }

        /// <summary>
        /// Default analog sensitivity
        /// </summary>
        [JsonProperty("sensitivity", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("sensitivity")]
        public long? Sensitivity
        {
            get => _internal.ReadLong(Models.Metadata.Control.SensitivityKey);
            set => _internal[Models.Metadata.Control.SensitivityKey] = value;
        }

        [JsonIgnore]
        public bool SensitivitySpecified { get { return Sensitivity != null; } }

        /// <summary>
        /// Default analog keydelta
        /// </summary>
        [JsonProperty("keydelta", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("keydelta")]
        public long? KeyDelta
        {
            get => _internal.ReadLong(Models.Metadata.Control.KeyDeltaKey);
            set => _internal[Models.Metadata.Control.KeyDeltaKey] = value;
        }

        [JsonIgnore]
        public bool KeyDeltaSpecified { get { return KeyDelta != null; } }

        /// <summary>
        /// Default analog reverse setting
        /// </summary>
        [JsonProperty("reverse", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("reverse")]
        public bool? Reverse
        {
            get => _internal.ReadBool(Models.Metadata.Control.ReverseKey);
            set => _internal[Models.Metadata.Control.ReverseKey] = value;
        }

        [JsonIgnore]
        public bool ReverseSpecified { get { return Reverse != null; } }

        /// <summary>
        /// First set of ways
        /// </summary>
        [JsonProperty("ways", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("ways")]
        public string? Ways
        {
            get => _internal.ReadString(Models.Metadata.Control.WaysKey);
            set => _internal[Models.Metadata.Control.WaysKey] = value;
        }

        /// <summary>
        /// Second set of ways
        /// </summary>
        [JsonProperty("ways2", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("ways2")]
        public string? Ways2
        {
            get => _internal.ReadString(Models.Metadata.Control.Ways2Key);
            set => _internal[Models.Metadata.Control.Ways2Key] = value;
        }

        /// <summary>
        /// Third set of ways
        /// </summary>
        [JsonProperty("ways3", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("ways3")]
        public string? Ways3
        {
            get => _internal.ReadString(Models.Metadata.Control.Ways3Key);
            set => _internal[Models.Metadata.Control.Ways3Key] = value;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Control object
        /// </summary>
        public Control()
        {
            _internal = new Models.Metadata.Control();
            Machine = new Machine();

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

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.Control ?? [],
            };
        }

        #endregion

        #region Manipulation

        /// <inheritdoc/>
        public override bool SetField(DatItemField datItemField, string value)
        {
            // Get the correct internal field name
            string? fieldName = datItemField switch
            {
                DatItemField.Control_Buttons => Models.Metadata.Control.ButtonsKey,
                DatItemField.Control_KeyDelta => Models.Metadata.Control.KeyDeltaKey,
                DatItemField.Control_Maximum => Models.Metadata.Control.MaximumKey,
                DatItemField.Control_Minimum => Models.Metadata.Control.MinimumKey,
                DatItemField.Control_Player => Models.Metadata.Control.PlayerKey,
                DatItemField.Control_RequiredButtons => Models.Metadata.Control.ReqButtonsKey,
                DatItemField.Control_Reverse => Models.Metadata.Control.ReverseKey,
                DatItemField.Control_Sensitivity => Models.Metadata.Control.SensitivityKey,
                DatItemField.Control_Type => Models.Metadata.Control.ControlTypeKey,
                DatItemField.Control_Ways => Models.Metadata.Control.WaysKey,
                DatItemField.Control_Ways2 => Models.Metadata.Control.Ways2Key,
                DatItemField.Control_Ways3 => Models.Metadata.Control.Ways3Key,
                _ => null,
            };

            // Set the field and return
            return FieldManipulator.SetField(_internal, fieldName, value);
        }

        #endregion
    }
}
