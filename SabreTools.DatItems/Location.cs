using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.Filtering;
using Newtonsoft.Json;

namespace SabreTools.DatItems
{
    /// <summary>
    /// Represents one conflocation or diplocation
    /// </summary>
    [JsonObject("location"), XmlRoot("location")]
    public class Location : DatItem
    {
        #region Fields

        /// <summary>
        /// Location name
        /// </summary>
        [JsonProperty("name")]
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Location ID
        /// </summary>
        [JsonProperty("number", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("number")]
        public long? Number { get; set; }

        [JsonIgnore]
        public bool NumberSpecified { get { return Number != null; } }

        /// <summary>
        /// Determines if location is inverted or not
        /// </summary>
        [JsonProperty("inverted", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("inverted")]
        public bool? Inverted { get; set; }

        [JsonIgnore]
        public bool InvertedSpecified { get { return Inverted != null; } }

        #endregion

        #region Accessors

        /// <summary>
        /// Gets the name to use for a DatItem
        /// </summary>
        /// <returns>Name if available, null otherwise</returns>
        public override string GetName()
        {
            return Name;
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="mappings">Mappings dictionary</param>
        public override void SetFields(Dictionary<Field, string> mappings)
        {
            // Set base fields
            base.SetFields(mappings);

            // Handle Location-specific fields
            if (mappings.Keys.Contains(Field.DatItem_Location_Name))
                Name = mappings[Field.DatItem_Location_Name];

            if (mappings.Keys.Contains(Field.DatItem_Location_Number))
                Number = Sanitizer.CleanLong(mappings[Field.DatItem_Location_Number]);

            if (mappings.Keys.Contains(Field.DatItem_Location_Inverted))
                Inverted = mappings[Field.DatItem_Location_Inverted].AsYesNo();
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Location object
        /// </summary>
        public Location()
        {
            Name = string.Empty;
            ItemType = ItemType.Location;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new Location()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Name = this.Name,
                Number = this.Number,
                Inverted = this.Inverted,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a Location, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Location
            Location newOther = other as Location;

            // If the Location information matches
            return (Name == newOther.Name
                && Number == newOther.Number
                && Inverted == newOther.Inverted);
        }

        #endregion

        #region Filtering

        /// <summary>
        /// Clean a DatItem according to the cleaner
        /// </summary>
        /// <param name="cleaner">Cleaner to implement</param>
        public override void Clean(Cleaner cleaner)
        {
            // Clean common items first
            base.Clean(cleaner);

            // If we're stripping unicode characters, strip item name
            if (cleaner?.RemoveUnicode == true)
                Name = RemoveUnicodeCharacters(Name);

            // If we are in NTFS trim mode, trim the game name
            if (cleaner?.Trim == true)
            {
                // Windows max name length is 260
                int usableLength = 260 - Machine.Name.Length - (cleaner.Root?.Length ?? 0);
                if (Name.Length > usableLength)
                {
                    string ext = Path.GetExtension(Name);
                    Name = Name.Substring(0, usableLength - ext.Length);
                    Name += ext;
                }
            }
        }

        /// <summary>
        /// Check to see if a DatItem passes the filter
        /// </summary>
        /// <param name="filter">Filter to check against</param>
        /// <param name="sub">True if this is a subitem, false otherwise</param>
        /// <returns>True if the item passed the filter, false otherwise</returns>
        public override bool PassesFilter(Filter filter, bool sub = false)
        {
            // Check common fields first
            if (!base.PassesFilter(filter, sub))
                return false;

            // Filter on item name
            if (!filter.PassStringFilter(filter.DatItem_Location_Name, Name))
                return false;

            // Filter on number
            if (!filter.PassLongFilter(filter.DatItem_Location_Number, Number))
                return false;

            // Filter on inverted
            if (!filter.PassBoolFilter(filter.DatItem_Location_Inverted, Inverted))
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
            if (fields.Contains(Field.DatItem_Location_Name))
                Name = null;

            if (fields.Contains(Field.DatItem_Location_Number))
                Number = null;

            if (fields.Contains(Field.DatItem_Location_Inverted))
                Inverted = null;
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

        /// <summary>
        /// Replace fields from another item
        /// </summary>
        /// <param name="item">DatItem to pull new information from</param>
        /// <param name="fields">List of Fields representing what should be updated</param>
        public override void ReplaceFields(DatItem item, List<Field> fields)
        {
            // Replace common fields first
            base.ReplaceFields(item, fields);

            // If we don't have a Location to replace from, ignore specific fields
            if (item.ItemType != ItemType.Location)
                return;

            // Cast for easier access
            Location newItem = item as Location;

            // Replace the fields
            if (fields.Contains(Field.DatItem_Location_Name))
                Name = newItem.Name;

            if (fields.Contains(Field.DatItem_Location_Number))
                Number = newItem.Number;

            if (fields.Contains(Field.DatItem_Location_Inverted))
                Inverted = newItem.Inverted;
        }

        #endregion
    }
}
