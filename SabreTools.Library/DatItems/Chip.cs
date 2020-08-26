using System.Collections.Generic;
using System.Linq;

using SabreTools.Library.Filtering;
using SabreTools.Library.Tools;
using Newtonsoft.Json;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Represents which Chip(s) is associated with a set
    /// </summary>
    [JsonObject("chip")]
    public class Chip : DatItem
    {
        #region Fields

        /// <summary>
        /// Internal tag
        /// </summary>
        [JsonProperty("tag")]
        public string Tag { get; set; }

        /// <summary>
        /// Type of the chip
        /// </summary>
        [JsonProperty("chiptype")]
        public string ChipType { get; set; } // TODO: (cpu|audio)

        /// <summary>
        /// Clock speed
        /// </summary>
        [JsonProperty("clock")]
        public string Clock { get; set; }

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

            // Handle Chip-specific fields
            if (mappings.Keys.Contains(Field.DatItem_Tag))
                Tag = mappings[Field.DatItem_Tag];

            if (mappings.Keys.Contains(Field.DatItem_ChipType))
                ChipType = mappings[Field.DatItem_ChipType];

            if (mappings.Keys.Contains(Field.DatItem_Clock))
                Clock = mappings[Field.DatItem_Clock];
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Chip object
        /// </summary>
        public Chip()
        {
            Name = string.Empty;
            ItemType = ItemType.Chip;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new Chip()
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

                Tag = this.Tag,
                ChipType = this.ChipType,
                Clock = this.Clock,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a chip, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a chip
            Chip newOther = other as Chip;

            // If the chip information matches
            return (Name == newOther.Name
                && Tag == newOther.Tag
                && ChipType == newOther.ChipType
                && Clock == newOther.Clock);
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

            // DatItem_Tag
            if (filter.DatItem_Tag.MatchesPositiveSet(Tag) == false)
                return false;
            if (filter.DatItem_Tag.MatchesNegativeSet(Tag) == true)
                return false;

            // DatItem_ChipType
            if (filter.DatItem_ChipType.MatchesPositiveSet(ChipType) == false)
                return false;
            if (filter.DatItem_ChipType.MatchesNegativeSet(ChipType) == true)
                return false;

            // DatItem_Clock
            if (filter.DatItem_Clock.MatchesPositiveSet(Clock) == false)
                return false;
            if (filter.DatItem_Clock.MatchesNegativeSet(Clock) == true)
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
            if (fields.Contains(Field.DatItem_Tag))
                Tag = null;

            if (fields.Contains(Field.DatItem_ChipType))
                ChipType = null;

            if (fields.Contains(Field.DatItem_Clock))
                Clock = null;
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

            // If we don't have a Chip to replace from, ignore specific fields
            if (item.ItemType != ItemType.Chip)
                return;

            // Cast for easier access
            Chip newItem = item as Chip;

            // Replace the fields
            if (fields.Contains(Field.DatItem_Tag))
                Tag = newItem.Tag;

            if (fields.Contains(Field.DatItem_ChipType))
                ChipType = newItem.ChipType;

            if (fields.Contains(Field.DatItem_Clock))
                Clock = newItem.Clock;
        }

        #endregion
    }
}
