using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents a single port on a machine
    /// </summary>
    [JsonObject("port"), XmlRoot("port")]
    public class Port : DatItem
    {
        #region Fields

        /// <summary>
        /// Tag for the port
        /// </summary>
        [JsonProperty("tag", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("tag")]
        public string? Tag
        {
            get => _port.ReadString(Models.Internal.Port.TagKey);
            set => _port[Models.Internal.Port.TagKey] = value;
        }

        /// <summary>
        /// List of analogs on the port
        /// </summary>
        [JsonProperty("analogs", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("analogs")]
        public List<Analog>? Analogs
        {
            get => _port.Read<Analog[]>(Models.Internal.Port.AnalogKey)?.ToList();
            set => _port[Models.Internal.Port.AnalogKey] = value?.ToArray();
        }

        [JsonIgnore]
        public bool AnalogsSpecified { get { return Analogs != null && Analogs.Count > 0; } }

        /// <summary>
        /// Internal Port model
        /// </summary>
        [JsonIgnore]
        private Models.Internal.Port _port = new();

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Port object
        /// </summary>
        public Port()
        {
            ItemType = ItemType.Port;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Port()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine?.Clone() as Machine,
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _port = this._port?.Clone() as Models.Internal.Port ?? new Models.Internal.Port(),
            };
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If we don't have a Port, return false
            if (ItemType != other?.ItemType || other is not Port otherInternal)
                return false;

            // Compare the internal models
            return _port.EqualTo(otherInternal._port);
        }

        #endregion
    }
}
