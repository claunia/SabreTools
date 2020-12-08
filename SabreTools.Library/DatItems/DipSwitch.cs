using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using SabreTools.Data;
using SabreTools.Library.Filtering;
using SabreTools.Library.Tools;
using Newtonsoft.Json;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Represents which DIP Switch(es) is associated with a set
    /// </summary>
    [JsonObject("dipswitch"), XmlRoot("dipswitch")]
    public class DipSwitch : DatItem
    {
        #region Fields

        #region Common

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name")]
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Tag associated with the dipswitch
        /// </summary>
        [JsonProperty("tag", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("tag")]
        public string Tag { get; set; }

        /// <summary>
        /// Mask associated with the dipswitch
        /// </summary>
        [JsonProperty("mask", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("mask")]
        public string Mask { get; set; }

        /// <summary>
        /// Conditions associated with the dipswitch
        /// </summary>
        [JsonProperty("conditions", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("conditions")]
        public List<Condition> Conditions { get; set; }

        [JsonIgnore]
        public bool ConditionsSpecified { get { return Conditions != null && Conditions.Count > 0; } }

        /// <summary>
        /// Locations associated with the dipswitch
        /// </summary>
        [JsonProperty("locations", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("locations")]
        public List<Location> Locations { get; set; }

        [JsonIgnore]
        public bool LocationsSpecified { get { return Locations != null && Locations.Count > 0; } }

        /// <summary>
        /// Settings associated with the dipswitch
        /// </summary>
        [JsonProperty("values", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("values")]
        public List<Setting> Values { get; set; }

        [JsonIgnore]
        public bool ValuesSpecified { get { return Values != null && Values.Count > 0; } }

        #endregion

        #region SoftwareList

        /// <summary>
        /// Original hardware part associated with the item
        /// </summary>
        [JsonProperty("part", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("part")]
        public Part Part { get; set; } = null;

        [JsonIgnore]
        public bool PartSpecified
        {
            get
            {
                return Part != null
                    && (!string.IsNullOrEmpty(Part.Name)
                        || !string.IsNullOrEmpty(Part.Interface));
            }
        }

        #endregion

        #endregion // Fields

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

            // Handle DipSwitch-specific fields

            #region Common

            if (mappings.Keys.Contains(Field.DatItem_Name))
                Name = mappings[Field.DatItem_Name];

            if (mappings.Keys.Contains(Field.DatItem_Tag))
                Tag = mappings[Field.DatItem_Tag];

            if (mappings.Keys.Contains(Field.DatItem_Mask))
                Mask = mappings[Field.DatItem_Mask];

            if (ConditionsSpecified)
            {
                foreach (Condition condition in Conditions)
                {
                    condition.SetFields(mappings, true);
                }
            }

            if (LocationsSpecified)
            {
                foreach (Location location in Locations)
                {
                    location.SetFields(mappings);
                }
            }

            if (ValuesSpecified)
            {
                foreach (Setting value in Values)
                {
                    value.SetFields(mappings);
                }
            }

            #endregion

            #region SoftwareList

            // Handle Part-specific fields
            if (Part == null)
                Part = new Part();

            Part.SetFields(mappings);

            #endregion
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty DipSwitch object
        /// </summary>
        public DipSwitch()
        {
            Name = string.Empty;
            ItemType = ItemType.DipSwitch;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new DipSwitch()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Name = this.Name,
                Tag = this.Tag,
                Mask = this.Mask,
                Conditions = this.Conditions,
                Locations = this.Locations,
                Values = this.Values,

                Part = this.Part,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a DipSwitch, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a DipSwitch
            DipSwitch newOther = other as DipSwitch;

            // If the DipSwitch information matches
            bool match = (Name == newOther.Name
                && Tag == newOther.Tag
                && Mask == newOther.Mask);
            if (!match)
                return match;

            // If the part matches
            if (PartSpecified)
                match &= (Part == newOther.Part);

            // If the conditions match
            if (ConditionsSpecified)
            {
                foreach (Condition condition in Conditions)
                {
                    match &= newOther.Conditions.Contains(condition);
                }
            }

            // If the locations match
            if (LocationsSpecified)
            {
                foreach (Location location in Locations)
                {
                    match &= newOther.Locations.Contains(location);
                }
            }

            // If the values match
            if (ValuesSpecified)
            {
                foreach (Setting value in Values)
                {
                    match &= newOther.Values.Contains(value);
                }
            }

            return match;
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
                Name = Sanitizer.RemoveUnicodeCharacters(Name);

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

            #region Common

            // Filter on item name
            if (!filter.PassStringFilter(filter.DatItem_Name, Name))
                return false;

            // Filter on tag
            if (!filter.PassStringFilter(filter.DatItem_Tag, Tag))
                return false;

            // Filter on mask
            if (!filter.PassStringFilter(filter.DatItem_Mask, Mask))
                return false;

            // Filter on individual conditions
            if (ConditionsSpecified)
            {
                foreach (Condition condition in Conditions)
                {
                    if (!condition.PassesFilter(filter, true))
                        return false;
                }
            }

            // Filter on individual locations
            if (LocationsSpecified)
            {
                foreach (Location location in Locations)
                {
                    if (!location.PassesFilter(filter, true))
                        return false;
                }
            }

            // Filter on individual conditions
            if (ValuesSpecified)
            {
                foreach (Setting value in Values)
                {
                    if (!value.PassesFilter(filter, true))
                        return false;
                }
            }

            #endregion

            #region SoftwareList

            // Filter on Part
            if (PartSpecified)
            {
                if (!Part.PassesFilter(filter, true))
                    return false;
            }

            #endregion

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

            #region Common

            if (fields.Contains(Field.DatItem_Name))
                Name = null;

            if (fields.Contains(Field.DatItem_Tag))
                Tag = null;

            if (fields.Contains(Field.DatItem_Mask))
                Mask = null;

            if (ConditionsSpecified)
            {
                foreach (Condition condition in Conditions)
                {
                    condition.RemoveFields(fields, true);
                }
            }

            if (LocationsSpecified)
            {
                foreach (Location location in Locations)
                {
                    location.RemoveFields(fields);
                }
            }

            if (ValuesSpecified)
            {
                foreach (Setting value in Values)
                {
                    value.RemoveFields(fields);
                }
            }

            #endregion

            #region SoftwareList

            if (PartSpecified)
                Part.RemoveFields(fields);

            #endregion
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

            // If we don't have a DipSwitch to replace from, ignore specific fields
            if (item.ItemType != ItemType.DipSwitch)
                return;

            // Cast for easier access
            DipSwitch newItem = item as DipSwitch;

            // Replace the fields

            #region Common

            if (fields.Contains(Field.DatItem_Name))
                Name = newItem.Name;

            if (fields.Contains(Field.DatItem_Tag))
                Tag = newItem.Tag;

            if (fields.Contains(Field.DatItem_Mask))
                Mask = newItem.Mask;

            // DatItem_Condition_* doesn't make sense here
            // since not every condition under the other item
            // can replace every condition under this item

            // DatItem_Location_* doesn't make sense here
            // since not every location under the other item
            // can replace every location under this item

            // DatItem_Setting_* doesn't make sense here
            // since not every value under the other item
            // can replace every value under this item

            #endregion

            #region SoftwareList

            if (PartSpecified && newItem.PartSpecified)
                Part.ReplaceFields(newItem.Part, fields);

            #endregion
        }

        #endregion
    }
}
