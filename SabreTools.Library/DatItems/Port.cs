using System.Collections.Generic;
using System.IO;
using System.Linq;

using SabreTools.Library.Filtering;
using SabreTools.Library.Tools;
using Newtonsoft.Json;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Represents a single port on a machine
    /// </summary>
    [JsonObject("port")]
    public class Port : DatItem
    {
        #region Fields

        /// <summary>
        /// Tag for the port
        /// </summary>
        [JsonProperty("tag", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Tag { get; set; }

        /// <summary>
        /// List of analogs on the port
        /// </summary>
        [JsonProperty("analogs", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<Analog> Analogs { get; set; }

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

            // Handle Port-specific fields
            if (mappings.Keys.Contains(Field.DatItem_Tag))
                Tag = mappings[Field.DatItem_Tag];

            // TODO: Handle DatItem_Analog*
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Port object
        /// </summary>
        public Port()
        {
            ItemType = ItemType.Port;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new Port()
            {
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
                Analogs = this.Analogs,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a Port, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Port
            Port newOther = other as Port;

            // If the Port information matches
            return (Tag == newOther.Tag); // TODO: Handle DatItem_Analog*
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

            // Filter on tag
            if (filter.DatItem_Tag.MatchesPositiveSet(Tag) == false)
                return false;
            if (filter.DatItem_Tag.MatchesNegativeSet(Tag) == true)
                return false;

            // TODO: Handle DatItem_Analog*

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

            if (fields.Contains(Field.DatItem_Analogs))
                Analogs = null;

            // TODO: Handle DatItem_Analog*
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

            // If we don't have a Port to replace from, ignore specific fields
            if (item.ItemType != ItemType.Port)
                return;

            // Cast for easier access
            Port newItem = item as Port;

            // Replace the fields
            if (fields.Contains(Field.DatItem_Name))
                Tag = newItem.Tag;

            if (fields.Contains(Field.DatItem_Analogs))
                Analogs = newItem.Analogs;

            // TODO: Handle DatItem_Analog*
        }

        #endregion
    }
}
