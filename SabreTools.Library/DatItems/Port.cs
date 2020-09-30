using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using SabreTools.Library.Filtering;
using Newtonsoft.Json;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Represents a single port on a machine
    /// </summary>
    [JsonObject("port"), XmlRoot("port")]
    public class Port : DatItem
    {
        #region Fields

        /// <summary>
        /// Tag for the port
        /// </summary>
        [JsonProperty("tag", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("tag")]
        public string Tag { get; set; }

        /// <summary>
        /// List of analogs on the port
        /// </summary>
        [JsonProperty("analogs", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("analogs")]
        public List<Analog> Analogs { get; set; }

        [JsonIgnore]
        public bool AnalogsSpecified { get { return Analogs != null && Analogs.Count > 0; } }

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

            if (AnalogsSpecified)
            {
                foreach (Analog analog in Analogs)
                {
                    analog.SetFields(mappings);
                }
            }
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
            bool match = (Tag == newOther.Tag);
            if (!match)
                return match;

            // If the analogs match
            if (AnalogsSpecified)
            {
                foreach (Analog analog in Analogs)
                {
                    match &= newOther.Analogs.Contains(analog);
                }
            }

            return match;
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
            if (!filter.PassStringFilter(filter.DatItem_Tag, Tag))
                return false;

            // Filter on individual analogs
            if (AnalogsSpecified)
            {
                foreach (Analog analog in Analogs)
                {
                    if (!analog.PassesFilter(filter, true))
                        return false;
                }
            }

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

            if (AnalogsSpecified)
            {
                foreach (Analog analog in Analogs)
                {
                    analog.RemoveFields(fields);
                }
            }
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

            // DatItem_Analog_* doesn't make sense here
            // since not every analog under the other item
            // can replace every analog under this item
        }

        #endregion
    }
}
