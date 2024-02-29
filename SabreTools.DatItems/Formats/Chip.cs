using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SabreTools.Core;
using SabreTools.Core.Tools;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents which Chip(s) is associated with a set
    /// </summary>
    [JsonObject("chip"), XmlRoot("chip")]
    public class Chip : DatItem
    {
        #region Fields

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name"), XmlElement("name")]
        public string? Name
        {
            get => _internal.ReadString(Models.Metadata.Chip.NameKey);
            set => _internal[Models.Metadata.Chip.NameKey] = value;
        }

        /// <summary>
        /// Internal tag
        /// </summary>
        [JsonProperty("tag", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("tag")]
        public string? Tag
        {
            get => _internal.ReadString(Models.Metadata.Chip.TagKey);
            set => _internal[Models.Metadata.Chip.TagKey] = value;
        }

        /// <summary>
        /// Type of the chip
        /// </summary>
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ChipType ChipType
        {
            get => _internal.ReadString(Models.Metadata.Chip.ChipTypeKey).AsChipType();
            set => _internal[Models.Metadata.Chip.ChipTypeKey] = value.FromChipType();
        }

        [JsonIgnore]
        public bool ChipTypeSpecified { get { return ChipType != ChipType.NULL; } }

        /// <summary>
        /// Clock speed
        /// </summary>
        [JsonProperty("clock", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("clock")]
        public long? Clock
        {
            get => _internal.ReadLong(Models.Metadata.Chip.ClockKey);
            set => _internal[Models.Metadata.Chip.ClockKey] = value;
        }

        [JsonIgnore]
        public bool ClockSpecified { get { return Clock != null; } }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Chip object
        /// </summary>
        public Chip()
        {
            _internal = new Models.Metadata.Chip();
            Machine = new Machine();

            Name = string.Empty;
            ItemType = ItemType.Chip;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Chip()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.Chip ?? [],
            };
        }

        #endregion
    }
}
