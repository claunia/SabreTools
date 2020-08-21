using System.Collections.Generic;

using SabreTools.Library.Filtering;
using Newtonsoft.Json;
using System.Linq;
using SabreTools.Library.Tools;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Represents which BIOS(es) is associated with a set
    /// </summary>
    public class BiosSet : DatItem
    {
        #region Fields

        /// <summary>
        /// Description of the BIOS
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Determine whether the BIOS is default
        /// </summary>
        [JsonProperty("default")]
        public bool? Default { get; set; }

        #endregion

        #region Accessors

        /// <summary>
        /// Get the value of that field as a string, if possible
        /// </summary>
        public override string GetField(Field field, List<Field> excludeFields)
        {
            // If the field is to be excluded, return empty string
            if (excludeFields.Contains(field))
                return string.Empty;

            // Handle BiosSet-specific fields
            string fieldValue;
            switch (field)
            {
                case Field.Default:
                    fieldValue = Default?.ToString();
                    break;
                case Field.BiosDescription:
                    fieldValue = Description;
                    break;

                // For everything else, use the base method
                default:
                    return base.GetField(field, excludeFields);
            }

            // Make sure we don't return null
            if (string.IsNullOrEmpty(fieldValue))
                fieldValue = string.Empty;

            return fieldValue;
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="mappings">Mappings dictionary</param>
        public override void SetFields(Dictionary<Field, string> mappings)
        {
            // Set base fields
            base.SetFields(mappings);

            // Handle BiosSet-specific fields
            if (mappings.Keys.Contains(Field.Default))
                Default = mappings[Field.Default].AsYesNo();

            if (mappings.Keys.Contains(Field.BiosDescription))
                Description = mappings[Field.BiosDescription];
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Sample object
        /// </summary>
        public BiosSet()
        {
            Name = string.Empty;
            ItemType = ItemType.BiosSet;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new BiosSet()
            {
                Name = this.Name,
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                AltName = this.AltName,
                AltTitle = this.AltTitle,

                PartName = this.PartName,
                PartInterface = this.PartInterface,
                Features = this.Features,
                AreaName = this.AreaName,
                AreaSize = this.AreaSize,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Description = this.Description,
                Default = this.Default,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a biosset, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a biosset
            BiosSet newOther = other as BiosSet;

            // If the archive information matches
            return (Name == newOther.Name && Description == newOther.Description && Default == newOther.Default);
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

            // Filter on description
            if (filter.Description.MatchesNeutral(null, Description) == false)
                return false;

            // Filter on default
            if (filter.Default.MatchesNeutral(null, Default) == false)
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
            if (fields.Contains(Field.BiosDescription))
                Description = null;

            if (fields.Contains(Field.Default))
                Default = null;
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

            // If we don't have a BiosSet to replace from, ignore specific fields
            if (item.ItemType != ItemType.BiosSet)
                return;

            // Cast for easier access
            BiosSet newItem = item as BiosSet;

            // Replace the fields
            if (fields.Contains(Field.BiosDescription))
                Description = newItem.Description;

            if (fields.Contains(Field.Default))
                Default = newItem.Default;
        }

        #endregion
    }
}
