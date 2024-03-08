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
            get => _internal.ReadString(Models.Metadata.DipLocation.NameKey);
            set => _internal[Models.Metadata.DipLocation.NameKey] = value;
        }

        /// <summary>
        /// Location ID
        /// </summary>
        [JsonProperty("number", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("number")]
        public long? Number
        {
            get => _internal.ReadLong(Models.Metadata.DipLocation.NameKey);
            set => _internal[Models.Metadata.DipLocation.NameKey] = value;
        }

        [JsonIgnore]
        public bool NumberSpecified { get { return Number != null; } }

        /// <summary>
        /// Determines if location is inverted or not
        /// </summary>
        [JsonProperty("inverted", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("inverted")]
        public bool? Inverted
        {
            get => _internal.ReadBool(Models.Metadata.DipLocation.InvertedKey);
            set => _internal[Models.Metadata.DipLocation.InvertedKey] = value;
        }

        [JsonIgnore]
        public bool InvertedSpecified { get { return Inverted != null; } }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => GetFieldValue<string>(Models.Metadata.DipLocation.NameKey);

        /// <inheritdoc/>
        public override void SetName(string? name) => SetFieldValue(Models.Metadata.DipLocation.NameKey, name);

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty DipLocation object
        /// </summary>
        public DipLocation()
        {
            _internal = new Models.Metadata.DipLocation();
            Machine = new Machine();

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

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.DipLocation ?? [],
            };
        }

        #endregion
    }
}
