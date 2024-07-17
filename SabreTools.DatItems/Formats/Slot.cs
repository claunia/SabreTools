using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents which Slot(s) is associated with a set
    /// </summary>
    [JsonObject("slot"), XmlRoot("slot")]
    public sealed class Slot : DatItem<Models.Metadata.Slot>
    {
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.Slot;

        /// <inheritdoc>/>
        protected override string? NameKey => Models.Metadata.Slot.NameKey;

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

        #region Constructors

        public Slot() : base() { }
        public Slot(Models.Metadata.Slot item) : base(item) { }

        #endregion
    }
}
