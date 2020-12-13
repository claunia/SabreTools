using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using SabreTools.Core;
using SabreTools.Filtering;
using Newtonsoft.Json;

namespace SabreTools.DatItems
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
        public override string GetName()
        {
            return Name;
        }

        /// <inheritdoc/>
        public override void SetName(string name)
        {
            Name = name;
        }

        /// <inheritdoc/>
        public override void SetFields(
            Dictionary<DatItemField, string> datItemMappings,
            Dictionary<MachineField, string> machineMappings)
        {
            // Set base fields
            base.SetFields(datItemMappings, machineMappings);

            // Handle Slot-specific fields
            if (datItemMappings.Keys.Contains(DatItemField.Name))
                Name = datItemMappings[DatItemField.Name];

            if (SlotOptionsSpecified)
            {
                foreach (SlotOption slotOption in SlotOptions)
                {
                    slotOption.SetFields(datItemMappings, machineMappings);
                }
            }
        }

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

        #region Filtering

        /// <inheritdoc/>
        public override bool PassesFilter(Cleaner cleaner, bool sub = false)
        {
            // Check common fields first
            if (!base.PassesFilter(cleaner, sub))
                return false;

            // Filter on item name
            if (!Filter.PassStringFilter(cleaner.DatItemFilter.Name, Name))
                return false;

            // Filter on individual slot options
            if (SlotOptionsSpecified)
            {
                foreach (SlotOption slotOption in SlotOptions)
                {
                    if (!slotOption.PassesFilter(cleaner, true))
                        return false;
                }
            }

            return true;
        }

        /// <inheritdoc/>
        public override void RemoveFields(
            List<DatItemField> datItemFields,
            List<MachineField> machineFields)
        {
            // Remove common fields first
            base.RemoveFields(datItemFields, machineFields);

            // Remove the fields
            if (datItemFields.Contains(DatItemField.Name))
                Name = null;

            if (SlotOptionsSpecified)
            {
                foreach (SlotOption slotOption in SlotOptions)
                {
                    slotOption.RemoveFields(datItemFields, machineFields);
                }
            }
        }

        /// <summary>
        /// Set internal names to match One Rom Per Game (ORPG) logic
        /// </summary>
        public override void SetOneRomPerGame()
        {
            string[] splitname = Name.Split('.');
            Machine.Name += $"/{string.Join(".", splitname.Take(splitname.Length > 1 ? splitname.Length - 1 : 1))}";
            Name = Path.GetFileName(Name);
        }

        #endregion

        #region Sorting and Merging

        /// <inheritdoc/>
        public override void ReplaceFields(
            DatItem item,
            List<DatItemField> datItemFields,
            List<MachineField> machineFields)
        {
            // Replace common fields first
            base.ReplaceFields(item, datItemFields, machineFields);

            // If we don't have a Slot to replace from, ignore specific fields
            if (item.ItemType != ItemType.Slot)
                return;

            // Cast for easier access
            Slot newItem = item as Slot;

            // Replace the fields
            if (datItemFields.Contains(DatItemField.Name))
                Name = newItem.Name;

            // DatItem_SlotOption_* doesn't make sense here
            // since not every slot option under the other item
            // can replace every slot option under this item
        }

        #endregion
    }
}
