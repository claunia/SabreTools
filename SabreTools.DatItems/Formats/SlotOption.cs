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
            get => _slotOption.ReadString(Models.Internal.SlotOption.NameKey);
            set => _slotOption[Models.Internal.SlotOption.NameKey] = value;
        }

        /// <summary>
        /// Referenced device name
        /// </summary>
        [JsonProperty("devname"), XmlElement("devname")]
        public string? DeviceName
        {
            get => _slotOption.ReadString(Models.Internal.SlotOption.DevNameKey);
            set => _slotOption[Models.Internal.SlotOption.DevNameKey] = value;
        }

        /// <summary>
        /// Determines if this slot option is default or not
        /// </summary>
        [JsonProperty("default", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("default")]
        public bool? Default
        {
            get => _slotOption.ReadBool(Models.Internal.SlotOption.DefaultKey);
            set => _slotOption[Models.Internal.SlotOption.DefaultKey] = value;
        }

        [JsonIgnore]
        public bool DefaultSpecified { get { return Default != null; } }

        /// <summary>
        /// Internal SlotOption model
        /// </summary>
        [JsonIgnore]
        private Models.Internal.SlotOption _slotOption = new();

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

                Machine = this.Machine?.Clone() as Machine,
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _slotOption = this._slotOption?.Clone() as Models.Internal.SlotOption ?? new Models.Internal.SlotOption(),
            };
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If we don't have a Adjuster, return false
            if (ItemType != other?.ItemType || other is not SlotOption otherInternal)
                return false;

            // Compare the internal models
            return _slotOption.EqualTo(otherInternal._slotOption);
        }

        #endregion
    }
}
