using System.Collections.Generic;
using System.Xml.Serialization;

using SabreTools.Core;
using Newtonsoft.Json;

namespace SabreTools.DatItems
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
        [JsonProperty("service", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("service")]
        public bool? Service { get; set; }

        [JsonIgnore]
        public bool ServiceSpecified { get { return Service != null; } }

        /// <summary>
        /// Determins if this has a tilt sensor
        /// </summary>
        [JsonProperty("tilt", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("tilt")]
        public bool? Tilt { get; set; }

        [JsonIgnore]
        public bool TiltSpecified { get { return Tilt != null; } }

        /// <summary>
        /// Number of players on the input
        /// </summary>
        [JsonProperty("players", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("players")]
        public long? Players { get; set; }

        [JsonIgnore]
        public bool PlayersSpecified { get { return Players != null; } }

        /// <summary>
        /// Number of coins required
        /// </summary>
        [JsonProperty("coins", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("coins")]
        public long? Coins { get; set; }

        [JsonIgnore]
        public bool CoinsSpecified { get { return Coins != null; } }

        /// <summary>
        /// Set of controls for the input
        /// </summary>
        [JsonProperty("controls", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("controls")]
        public List<Control> Controls { get; set; }

        [JsonIgnore]
        public bool ControlsSpecified { get { return Controls != null && Controls.Count > 0; } }

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

        public override object Clone()
        {
            return new Input()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Service = this.Service,
                Tilt = this.Tilt,
                Players = this.Players,
                Coins = this.Coins,
                Controls = this.Controls,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a Input, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Input
            Input newOther = other as Input;

            // If the Input information matches
            bool match = (Service == newOther.Service
                && Tilt == newOther.Tilt
                && Players == newOther.Players
                && Coins == newOther.Coins);
            if (!match)
                return match;

            // If the controls match
            if (ControlsSpecified)
            {
                foreach (Control control in Controls)
                {
                    match &= newOther.Controls.Contains(control);
                }
            }

            return match;
        }

        #endregion
    }
}
