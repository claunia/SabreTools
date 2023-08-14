using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

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
            get => _input.ReadBool(Models.Internal.Input.ServiceKey);
            set => _input[Models.Internal.Input.ServiceKey] = value;
        }

        [JsonIgnore]
        public bool ServiceSpecified { get { return Service != null; } }

        /// <summary>
        /// Determins if this has a tilt sensor
        /// </summary>
        [JsonProperty("tilt", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("tilt")]
        public bool? Tilt
        {
            get => _input.ReadBool(Models.Internal.Input.TiltKey);
            set => _input[Models.Internal.Input.TiltKey] = value;
        }

        [JsonIgnore]
        public bool TiltSpecified { get { return Tilt != null; } }

        /// <summary>
        /// Number of players on the input
        /// </summary>
        [JsonProperty("players", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("players")]
        public long? Players
        {
            get => _input.ReadLong(Models.Internal.Input.PlayersKey);
            set => _input[Models.Internal.Input.PlayersKey] = value;
        }

        [JsonIgnore]
        public bool PlayersSpecified { get { return Players != null; } }

        /// <summary>
        /// Number of coins required
        /// </summary>
        [JsonProperty("coins", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("coins")]
        public long? Coins
        {
            get => _input.ReadLong(Models.Internal.Input.CoinsKey);
            set => _input[Models.Internal.Input.CoinsKey] = value;
        }

        [JsonIgnore]
        public bool CoinsSpecified { get { return Coins != null; } }

        /// <summary>
        /// Set of controls for the input
        /// </summary>
        [JsonProperty("controls", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("controls")]
        public List<Control>? Controls
        {
            get => _input.Read<Control[]>(Models.Internal.Input.ControlKey)?.ToList();
            set => _input[Models.Internal.Input.ControlKey] = value?.ToArray();
        }

        [JsonIgnore]
        public bool ControlsSpecified { get { return Controls != null && Controls.Count > 0; } }

        /// <summary>
        /// Internal Input model
        /// </summary>
        [JsonIgnore]
        private Models.Internal.Input _input = new();

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Input object
        /// </summary>
        public Input()
        {
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

                Machine = this.Machine?.Clone() as Machine,
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _input = this._input?.Clone() as Models.Internal.Input ?? new Models.Internal.Input(),
            };
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If we don't have a Input, return false
            if (ItemType != other?.ItemType || other is not Input otherInternal)
                return false;

            // Compare the internal models
            return _input.EqualTo(otherInternal._input);
        }

        #endregion
    }
}
