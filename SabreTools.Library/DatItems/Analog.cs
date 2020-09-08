using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using SabreTools.Library.Filtering;
using Newtonsoft.Json;

namespace SabreTools.Library.DatItems
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

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="mappings">Mappings dictionary</param>
        public override void SetFields(Dictionary<Field, string> mappings)
        {
            // Set base fields
            base.SetFields(mappings);

            // Handle Analog-specific fields
            if (mappings.Keys.Contains(Field.DatItem_Analog_Mask))
                Mask = mappings[Field.DatItem_Analog_Mask];
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

        /// <summary>
        /// Check to see if a DatItem passes the filter
        /// </summary>
        /// <param name="filter">Filter to check against</param>
        /// <returns>True if the item passed the filter, false otherwise</returns>
        public override bool PassesFilter(Filter filter)
        {
            // Check common fields first
            if (!base.PassesFilter(filter))
                return false;

            // Filter on mask
            if (!filter.PassStringFilter(filter.DatItem_Analog_Mask, Mask))
                return false;

            return true;
        }

        /// <summary>
        /// Remove fields from the DatItem
        /// </summary>
        /// <param name="fields">List of Fields to remove</param>
        public override void RemoveFields(List<Field> fields)
        {
            // Remove common fields first
            base.RemoveFields(fields);

            // Remove the fields
            if (fields.Contains(Field.DatItem_Analog_Mask))
                Mask = null;
        }

        #endregion

        #region Sorting and Merging

        /// <summary>
        /// Replace fields from another item
        /// </summary>
        /// <param name="item">DatItem to pull new information from</param>
        /// <param name="fields">List of Fields representing what should be updated</param>
        public override void ReplaceFields(DatItem item, List<Field> fields)
        {
            // Replace common fields first
            base.ReplaceFields(item, fields);

            // If we don't have a Analog to replace from, ignore specific fields
            if (item.ItemType != ItemType.Analog)
                return;

            // Cast for easier access
            Analog newItem = item as Analog;

            // Replace the fields
            if (fields.Contains(Field.DatItem_Analog_Mask))
                Mask = newItem.Mask;
        }

        #endregion
    }
}
