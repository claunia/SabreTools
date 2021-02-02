using System.Xml.Serialization;

using SabreTools.Core;
using Newtonsoft.Json;

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
        [JsonProperty("mask", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("mask")]
        public string Mask { get; set; }

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

        public override object Clone()
        {
            return new Analog()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Mask = this.Mask,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a Analog, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Analog
            Analog newOther = other as Analog;

            // If the Feature information matches
            return (Mask == newOther.Mask);
        }

        #endregion
    }
}
