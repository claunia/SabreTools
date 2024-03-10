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

        [JsonIgnore]
        public bool SlotOptionsSpecified
        {
            get
            {
                var slotOptions = GetFieldValue<SlotOption[]?>(Models.Metadata.Slot.SlotOptionKey);
                return slotOptions != null && slotOptions.Length > 0;
            }
        }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => GetFieldValue<string>(Models.Metadata.Slot.NameKey);

        /// <inheritdoc/>
        public override void SetName(string? name) => SetFieldValue(Models.Metadata.Slot.NameKey, name);

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Slot object
        /// </summary>
        public Slot()
        {
            _internal = new Models.Metadata.Slot();
            Machine = new Machine();

            SetName(string.Empty);
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

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.Slot ?? [],
            };
        }

        #endregion
    }
}
