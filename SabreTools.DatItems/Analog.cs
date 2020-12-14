using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using SabreTools.Core;
using Newtonsoft.Json;

namespace SabreTools.DatItems
{
    /// <summary>
    /// Represents a single analog item
    /// </summary>
    [JsonObject("analog"), XmlRoot("analog")]
    public class Analog : DatItem
    {
        #region Fields

        /// <summary>
        /// Analog mask value
        /// </summary>
        [JsonProperty("mask", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("mask")]
        public string Mask { get; set; }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override void SetFields(
            Dictionary<DatItemField, string> datItemMappings,
            Dictionary<MachineField, string> machineMappings)
        {
            // Set base fields
            base.SetFields(datItemMappings, machineMappings);

            // Handle Analog-specific fields
            if (datItemMappings.Keys.Contains(DatItemField.Analog_Mask))
                Mask = datItemMappings[DatItemField.Analog_Mask];
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Analog object
        /// </summary>
        public Analog()
        {
            ItemType = ItemType.Analog;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new Analog()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Mask = this.Mask,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a Analog, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Analog
            Analog newOther = other as Analog;

            // If the Feature information matches
            return (Mask == newOther.Mask);
        }

        #endregion

        #region Filtering

        /// <inheritdoc/>
        public override void RemoveFields(
            List<DatItemField> datItemFields,
            List<MachineField> machineFields)
        {
            // Remove common fields first
            base.RemoveFields(datItemFields, machineFields);

            // Remove the fields
            if (datItemFields.Contains(DatItemField.Analog_Mask))
                Mask = null;
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

            // If we don't have a Analog to replace from, ignore specific fields
            if (item.ItemType != ItemType.Analog)
                return;

            // Cast for easier access
            Analog newItem = item as Analog;

            // Replace the fields
            if (datItemFields.Contains(DatItemField.Analog_Mask))
                Mask = newItem.Mask;
        }

        #endregion
    }
}
