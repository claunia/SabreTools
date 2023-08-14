using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents a single analog item
    /// </summary>
    [JsonObject("analog"), XmlRoot("analog")]
    public class Analog : DatItem
    {
        #region Fields

        /// <summary>
        /// Analog mask value
        /// </summary>
        [JsonProperty("mask", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("mask")]
        public string? Mask
        {
            get => _analog.ReadString(Models.Internal.Analog.MaskKey);
            set => _analog[Models.Internal.Analog.MaskKey] = value;
        }

        /// <summary>
        /// Internal Analog model
        /// </summary>
        [JsonIgnore]
        private Models.Internal.Analog _analog = new();

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Analog object
        /// </summary>
        public Analog()
        {
            ItemType = ItemType.Analog;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Analog()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine?.Clone() as Machine,
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _analog = this._analog?.Clone() as Models.Internal.Analog ?? new Models.Internal.Analog(),
            };
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If we don't have a Analog, return false
            if (ItemType != other?.ItemType || other is not Analog otherInternal)
                return false;

            // Compare the internal models
            return _analog.EqualTo(otherInternal._analog);
        }

        #endregion
    }
}
