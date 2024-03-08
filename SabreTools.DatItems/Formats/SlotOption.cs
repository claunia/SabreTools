using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents one ListXML slotoption
    /// </summary>
    [JsonObject("slotoption"), XmlRoot("slotoption")]
    public class SlotOption : DatItem
    {
        #region Fields

        /// <summary>
        /// Slot option name
        /// </summary>
        [JsonProperty("name"), XmlElement("name")]
        public string? Name
        {
            get => _internal.ReadString(Models.Metadata.SlotOption.NameKey);
            set => _internal[Models.Metadata.SlotOption.NameKey] = value;
        }

        /// <summary>
        /// Referenced device name
        /// </summary>
        [JsonProperty("devname"), XmlElement("devname")]
        public string? DeviceName
        {
            get => _internal.ReadString(Models.Metadata.SlotOption.DevNameKey);
            set => _internal[Models.Metadata.SlotOption.DevNameKey] = value;
        }

        /// <summary>
        /// Determines if this slot option is default or not
        /// </summary>
        [JsonProperty("default", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("default")]
        public bool? Default
        {
            get => _internal.ReadBool(Models.Metadata.SlotOption.DefaultKey);
            set => _internal[Models.Metadata.SlotOption.DefaultKey] = value;
        }

        [JsonIgnore]
        public bool DefaultSpecified { get { return Default != null; } }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty SlotOption object
        /// </summary>
        public SlotOption()
        {
            _internal = new Models.Metadata.SlotOption();
            Machine = new Machine();

            Name = string.Empty;
            ItemType = ItemType.SlotOption;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new SlotOption()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.SlotOption ?? [],
            };
        }

        #endregion
    }
}
