using System.Collections.Generic;
using System.Linq;

using SabreTools.Library.Filtering;
using SabreTools.Library.Tools;
using Newtonsoft.Json;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Represents which RAM option(s) is associated with a set
    /// </summary>
    [JsonObject("ramoption")]
    public class RamOption : DatItem
    {
        #region Fields

        /// <summary>
        /// Determine whether the RamOption is default
        /// </summary>
        [JsonProperty("default", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? Default { get; set; }

        /// <summary>
        /// Determines the content of the RamOption
        /// </summary>
        [JsonProperty("content", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Content { get; set; }

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

            // Handle BiosSet-specific fields
            if (mappings.Keys.Contains(Field.DatItem_Default))
                Default = mappings[Field.DatItem_Default].AsYesNo();

            if (mappings.Keys.Contains(Field.DatItem_Content))
                Content = mappings[Field.DatItem_Content];
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty RamOption object
        /// </summary>
        public RamOption()
        {
            Name = string.Empty;
            ItemType = ItemType.RamOption;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new RamOption()
            {
                Name = this.Name,
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                AltName = this.AltName,
                AltTitle = this.AltTitle,

                Original = this.Original,
                OpenMSXSubType = this.OpenMSXSubType,
                OpenMSXType = this.OpenMSXType,
                Remark = this.Remark,
                Boot = this.Boot,

                Part = this.Part,
                Features = this.Features,
                AreaName = this.AreaName,
                AreaSize = this.AreaSize,
                AreaWidth = this.AreaWidth,
                AreaEndianness = this.AreaEndianness,
                Value = this.Value,
                LoadFlag = this.LoadFlag,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Default = this.Default,
                Content = this.Content,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a RamOption, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a RamOption
            RamOption newOther = other as RamOption;

            // If the BiosSet information matches
            return (Name == newOther.Name && Default == newOther.Default && Content == newOther.Content);
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

            // Filter on default
            if (filter.DatItem_Default.MatchesNeutral(null, Default) == false)
                return false;

            // Filter on content
            if (filter.DatItem_Content.MatchesPositiveSet(Content) == false)
                return false;
            if (filter.DatItem_Content.MatchesNegativeSet(Content) == true)
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
            if (fields.Contains(Field.DatItem_Default))
                Default = null;

            if (fields.Contains(Field.DatItem_Content))
                Content = null;
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
            if (item.ItemType != ItemType.RamOption)
                return;

            // Cast for easier access
            RamOption newItem = item as RamOption;

            // Replace the fields
            if (fields.Contains(Field.DatItem_Default))
                Default = newItem.Default;

            if (fields.Contains(Field.DatItem_Content))
                Content = newItem.Content;
        }

        #endregion
    }
}
