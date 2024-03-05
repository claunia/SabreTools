using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;
using SabreTools.Filter;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents one ListXML input
    /// </summary>
    [JsonObject("input"), XmlRoot("input")]
    public class Input : DatItem
    {
        #region Fields

        /// <summary>
        /// Input service ID
        /// </summary>
        [JsonProperty("service", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("service")]
        public bool? Service
        {
            get => _internal.ReadBool(Models.Metadata.Input.ServiceKey);
            set => _internal[Models.Metadata.Input.ServiceKey] = value;
        }

        [JsonIgnore]
        public bool ServiceSpecified { get { return Service != null; } }

        /// <summary>
        /// Determins if this has a tilt sensor
        /// </summary>
        [JsonProperty("tilt", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("tilt")]
        public bool? Tilt
        {
            get => _internal.ReadBool(Models.Metadata.Input.TiltKey);
            set => _internal[Models.Metadata.Input.TiltKey] = value;
        }

        [JsonIgnore]
        public bool TiltSpecified { get { return Tilt != null; } }

        /// <summary>
        /// Number of players on the input
        /// </summary>
        [JsonProperty("players", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("players")]
        public long? Players
        {
            get => _internal.ReadLong(Models.Metadata.Input.PlayersKey);
            set => _internal[Models.Metadata.Input.PlayersKey] = value;
        }

        [JsonIgnore]
        public bool PlayersSpecified { get { return Players != null; } }

        /// <summary>
        /// Number of coins required
        /// </summary>
        [JsonProperty("coins", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("coins")]
        public long? Coins
        {
            get => _internal.ReadLong(Models.Metadata.Input.CoinsKey);
            set => _internal[Models.Metadata.Input.CoinsKey] = value;
        }

        [JsonIgnore]
        public bool CoinsSpecified { get { return Coins != null; } }

        /// <summary>
        /// Set of controls for the input
        /// </summary>
        [JsonProperty("controls", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("controls")]
        public List<Control>? Controls
        {
            get => _internal.Read<Control[]>(Models.Metadata.Input.ControlKey)?.ToList();
            set => _internal[Models.Metadata.Input.ControlKey] = value?.ToArray();
        }

        [JsonIgnore]
        public bool ControlsSpecified { get { return Controls != null && Controls.Count > 0; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Input object
        /// </summary>
        public Input()
        {
            _internal = new Models.Metadata.Input();
            Machine = new Machine();

            ItemType = ItemType.Input;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Input()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.Input ?? [],
            };
        }

        #endregion
    
        #region Manipulation

        /// <inheritdoc/>
        public override bool RemoveField(DatItemField datItemField)
        {
            // Get the correct internal field name
            string? fieldName = datItemField switch
            {
                DatItemField.Coins => Models.Metadata.Input.CoinsKey,
                DatItemField.Players => Models.Metadata.Input.PlayersKey,
                DatItemField.Service => Models.Metadata.Input.ServiceKey,
                DatItemField.Tilt => Models.Metadata.Input.TiltKey,
                _ => null,
            };

            // Remove the field and return
            return FieldManipulator.RemoveField(_internal, fieldName);
        }

        #endregion
    }
}
