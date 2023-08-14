using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
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
        [JsonProperty("channels", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("channels")]
        public long? Channels
        {
            get => _sound.ReadLong(Models.Internal.Sound.ChannelsKey);
            set => _sound[Models.Internal.Sound.ChannelsKey] = value;
        }

        [JsonIgnore]
        public bool ChannelsSpecified { get { return Channels != null; } }

        /// <summary>
        /// Internal Sound model
        /// </summary>
        [JsonIgnore]
        private Models.Internal.Sound _sound = new();

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

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Sound()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine?.Clone() as Machine,
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _sound = this._sound?.Clone() as Models.Internal.Sound ?? new Models.Internal.Sound(),
            };
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If we don't have a Sound, return false
            if (ItemType != other?.ItemType || other is not Sound otherInternal)
                return false;

            // Compare the internal models
            return _sound.EqualTo(otherInternal._sound);
        }

        #endregion
    }
}
