using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents which Slot(s) is associated with a set
    /// </summary>
    [JsonObject("slot"), XmlRoot("slot")]
    public class Slot : DatItem
    {
        #region Fields

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name"), XmlElement("name")]
        public string? Name
        {
            get => _slot.ReadString(Models.Internal.Slot.NameKey);
            set => _slot[Models.Internal.Slot.NameKey] = value;
        }

        /// <summary>
        /// Slot options associated with the slot
        /// </summary>
        [JsonProperty("slotoptions", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("slotoptions")]
        public List<SlotOption>? SlotOptions
        {
            get => _slot.Read<SlotOption[]>(Models.Internal.Slot.SlotOptionKey)?.ToList();
            set => _slot[Models.Internal.Slot.SlotOptionKey] = value?.ToArray();
        }

        [JsonIgnore]
        public bool SlotOptionsSpecified { get { return SlotOptions != null && SlotOptions.Count > 0; } }

        /// <summary>
        /// Internal Slot model
        /// </summary>
        [JsonIgnore]
        private Models.Internal.Slot _slot = new();

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Slot object
        /// </summary>
        public Slot()
        {
            Name = string.Empty;
            ItemType = ItemType.Slot;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Slot()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine?.Clone() as Machine,
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _slot = this._slot?.Clone() as Models.Internal.Slot ?? new Models.Internal.Slot(),
            };
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If we don't have a Slot, return false
            if (ItemType != other?.ItemType || other is not Slot otherInternal)
                return false;

            // Compare the internal models
            return _slot.EqualTo(otherInternal._slot);
        }

        #endregion
    }
}
