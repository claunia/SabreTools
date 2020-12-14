using System.Xml.Serialization;

using SabreTools.Core;
using Newtonsoft.Json;

namespace SabreTools.DatItems
{
    /// <summary>
    /// Represents the sound output for a machine
    /// </summary>
    [JsonObject("sound"), XmlRoot("sound")]
    public class Sound : DatItem
    {
        #region Fields

        /// <summary>
        /// Number of speakers or channels
        /// </summary>
        [JsonProperty("channels", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("channels")]
        public long? Channels { get; set; }

        [JsonIgnore]
        public bool ChannelsSpecified { get { return Channels != null; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Sound object
        /// </summary>
        public Sound()
        {
            ItemType = ItemType.Sound;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new Sound()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Channels = this.Channels,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a Sound, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Sound
            Sound newOther = other as Sound;

            // If the Sound information matches
            return (Channels == newOther.Channels);
        }

        #endregion
    }
}
