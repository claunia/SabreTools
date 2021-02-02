using System.Collections.Generic;
using System.Xml.Serialization;

using SabreTools.Core;
using Newtonsoft.Json;

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
        [JsonProperty("name")]
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Slot options associated with the slot
        /// </summary>
        [JsonProperty("slotoptions", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("slotoptions")]
        public List<SlotOption> SlotOptions { get; set; }

        [JsonIgnore]
        public bool SlotOptionsSpecified { get { return SlotOptions != null && SlotOptions.Count > 0; } }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string name) => Name = name;

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

        public override object Clone()
        {
            return new Slot()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Name = this.Name,
                SlotOptions = this.SlotOptions,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a Slot, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Slot
            Slot newOther = other as Slot;

            // If the Slot information matches
            bool match = (Name == newOther.Name);
            if (!match)
                return match;

            // If the slot options match
            if (SlotOptionsSpecified)
            {
                foreach (SlotOption slotOption in SlotOptions)
                {
                    match &= newOther.SlotOptions.Contains(slotOption);
                }
            }

            return match;
        }

        #endregion
    }
}
