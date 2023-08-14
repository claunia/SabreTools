using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents one diplocation
    /// </summary>
    [JsonObject("diplocation"), XmlRoot("diplocation")]
    public class DipLocation : DatItem
    {
        #region Fields

        /// <summary>
        /// Location name
        /// </summary>
        [JsonProperty("name"), XmlElement("name")]
        public string? Name
        {
            get => _dipLocation.ReadString(Models.Internal.DipLocation.NameKey);
            set => _dipLocation[Models.Internal.DipLocation.NameKey] = value;
        }

        /// <summary>
        /// Location ID
        /// </summary>
        [JsonProperty("number", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("number")]
        public long? Number
        {
            get => _dipLocation.ReadLong(Models.Internal.DipLocation.NameKey);
            set => _dipLocation[Models.Internal.DipLocation.NameKey] = value;
        }

        [JsonIgnore]
        public bool NumberSpecified { get { return Number != null; } }

        /// <summary>
        /// Determines if location is inverted or not
        /// </summary>
        [JsonProperty("inverted", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("inverted")]
        public bool? Inverted
        {
            get => _dipLocation.ReadBool(Models.Internal.DipLocation.InvertedKey);
            set => _dipLocation[Models.Internal.DipLocation.InvertedKey] = value;
        }

        [JsonIgnore]
        public bool InvertedSpecified { get { return Inverted != null; } }

        /// <summary>
        /// Internal DipLocation model
        /// </summary>
        [JsonIgnore]
        private Models.Internal.DipLocation _dipLocation = new();

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty DipLocation object
        /// </summary>
        public DipLocation()
        {
            Name = string.Empty;
            ItemType = ItemType.DipLocation;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new DipLocation()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine?.Clone() as Machine,
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _dipLocation = this._dipLocation?.Clone() as Models.Internal.DipLocation ?? new Models.Internal.DipLocation(),
            };
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If we don't have a DipLocation, return false
            if (ItemType != other?.ItemType || other is not DipLocation otherInternal)
                return false;

            // Compare the internal models
            return _dipLocation.EqualTo(otherInternal._dipLocation);
        }

        #endregion
    }
}
