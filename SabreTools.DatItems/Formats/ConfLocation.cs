using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents one conflocation
    /// </summary>
    [JsonObject("conflocation"), XmlRoot("conflocation")]
    public class ConfLocation : DatItem
    {
        #region Fields

        /// <summary>
        /// Location name
        /// </summary>
        [JsonProperty("name"), XmlElement("name")]
        public string? Name
        {
            get => _confLocation.ReadString(Models.Internal.ConfLocation.NameKey);
            set => _confLocation[Models.Internal.ConfLocation.NameKey] = value;
        }

        /// <summary>
        /// Location ID
        /// </summary>
        [JsonProperty("number", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("number")]
        public long? Number
        {
            get => _confLocation.ReadLong(Models.Internal.ConfLocation.NumberKey);
            set => _confLocation[Models.Internal.ConfLocation.NumberKey] = value;
        }

        [JsonIgnore]
        public bool NumberSpecified { get { return Number != null; } }

        /// <summary>
        /// Determines if location is inverted or not
        /// </summary>
        [JsonProperty("inverted", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("inverted")]
        public bool? Inverted
        {
            get => _confLocation.ReadBool(Models.Internal.ConfLocation.InvertedKey);
            set => _confLocation[Models.Internal.ConfLocation.InvertedKey] = value;
        }

        [JsonIgnore]
        public bool InvertedSpecified { get { return Inverted != null; } }

        /// <summary>
        /// Internal ConfLocation model
        /// </summary>
        [JsonIgnore]
        private Models.Internal.ConfLocation _confLocation = new();

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty ConfLocation object
        /// </summary>
        public ConfLocation()
        {
            Name = string.Empty;
            ItemType = ItemType.ConfLocation;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new ConfLocation()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine?.Clone() as Machine,
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _confLocation = this._confLocation?.Clone() as Models.Internal.ConfLocation ?? new Models.Internal.ConfLocation(),
            };
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If we don't have a v, return false
            if (ItemType != other?.ItemType || other is not ConfLocation otherInternal)
                return false;

            // Compare the internal models
            return _confLocation.EqualTo(otherInternal._confLocation);
        }

        #endregion
    }
}
