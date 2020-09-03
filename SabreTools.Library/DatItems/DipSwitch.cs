using System.Collections.Generic;
using System.IO;
using System.Linq;

using SabreTools.Library.Filtering;
using SabreTools.Library.Tools;
using Newtonsoft.Json;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Represents which DIP Switch(es) is associated with a set
    /// </summary>
    [JsonObject("dipswitch")]
    public class DipSwitch : DatItem
    {
        #region Fields

        #region Common

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Tag associated with the dipswitch
        /// </summary>
        [JsonProperty("tag", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Tag { get; set; }

        /// <summary>
        /// Mask associated with the dipswitch
        /// </summary>
        [JsonProperty("mask", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Mask { get; set; }

        /// <summary>
        /// Conditions associated with the dipswitch
        /// </summary>
        [JsonProperty("conditions", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<Condition> Conditions { get; set; }

        /// <summary>
        /// Locations associated with the dipswitch
        /// </summary>
        [JsonProperty("locations", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<Location> Locations { get; set; }

        /// <summary>
        /// Settings associated with the dipswitch
        /// </summary>
        [JsonProperty("values", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<Setting> Values { get; set; }

        #endregion

        #region SoftwareList

        /// <summary>
        /// Original hardware part associated with the item
        /// </summary>
        [JsonProperty("part", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Part Part { get; set; }

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

            if (Conditions != null)
            {
                foreach (Condition condition in Conditions)
                {
                    condition.SetFields(mappings, true);
                }
            }

            if (Locations != null)
            {
                foreach (Location location in Locations)
                {
                    location.SetFields(mappings);
                }
            }

            if (Values != null)
            {
                foreach (Setting value in Values)
                {
                    value.SetFields(mappings);
                }
            }

            #endregion

            #region SoftwareList

            if (mappings.Keys.Contains(Field.DatItem_Part_Name))
            {
                if (Part == null)
                    Part = new Part();

                Part.Name = mappings[Field.DatItem_Part_Name];
            }

            if (mappings.Keys.Contains(Field.DatItem_Part_Interface))
            {
                if (Part == null)
                    Part = new Part();

                Part.Interface = mappings[Field.DatItem_Part_Interface];
            }

            // TODO: Handle DatItem_Feature*

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

            // TODO: Handle Part*

            // If the conditions match
            if (Conditions != null)
            {
                foreach (Condition condition in Conditions)
                {
                    match &= newOther.Conditions.Contains(condition);
                }
            }

            // If the locations match
            if (Locations != null)
            {
                foreach (Location location in Locations)
                {
                    match &= newOther.Locations.Contains(location);
                }
            }

            // If the values match
            if (Values != null)
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
        /// <returns>True if the item passed the filter, false otherwise</returns>
        public override bool PassesFilter(Filter filter)
        {
            // Check common fields first
            if (!base.PassesFilter(filter))
                return false;

            #region Common

            // Filter on item name
            if (filter.DatItem_Name.MatchesPositiveSet(Name) == false)
                return false;
            if (filter.DatItem_Name.MatchesNegativeSet(Name) == true)
                return false;

            // Filter on tag
            if (filter.DatItem_Tag.MatchesPositiveSet(Tag) == false)
                return false;
            if (filter.DatItem_Tag.MatchesNegativeSet(Tag) == true)
                return false;

            // Filter on mask
            if (filter.DatItem_Mask.MatchesPositiveSet(Mask) == false)
                return false;
            if (filter.DatItem_Mask.MatchesNegativeSet(Mask) == true)
                return false;

            // Filter on individual conditions
            if (Conditions != null)
            {
                foreach (Condition condition in Conditions)
                {
                    if (!condition.PassesFilter(filter, true))
                        return false;
                }
            }

            // Filter on individual locations
            if (Locations != null)
            {
                foreach (Location location in Locations)
                {
                    if (!location.PassesFilter(filter))
                        return false;
                }
            }

            // Filter on individual conditions
            if (Values != null)
            {
                foreach (Setting value in Values)
                {
                    if (!value.PassesFilter(filter))
                        return false;
                }
            }

            #endregion

            #region SoftwareList

            // Filter on part name
            if (filter.DatItem_Part_Name.MatchesPositiveSet(Part?.Name) == false)
                return false;
            if (filter.DatItem_Part_Name.MatchesNegativeSet(Part?.Name) == true)
                return false;

            // Filter on part interface
            if (filter.DatItem_Part_Interface.MatchesPositiveSet(Part?.Interface) == false)
                return false;
            if (filter.DatItem_Part_Interface.MatchesNegativeSet(Part?.Interface) == true)
                return false;

            // TODO: Handle DatItem_Feature*

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

            if (Conditions != null)
            {
                foreach (Condition condition in Conditions)
                {
                    condition.RemoveFields(fields, true);
                }
            }

            if (Locations != null)
            {
                foreach (Location location in Locations)
                {
                    location.RemoveFields(fields);
                }
            }

            if (Values != null)
            {
                foreach (Setting value in Values)
                {
                    value.RemoveFields(fields);
                }
            }

            #endregion

            #region SoftwareList

            if (fields.Contains(Field.DatItem_Part_Name) && Part != null)
                Part.Name = null;

            if (fields.Contains(Field.DatItem_Part_Interface) && Part != null)
                Part.Interface = null;

            // TODO: Handle DatItem_Feature*

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

            if (fields.Contains(Field.DatItem_Part_Name))
            {
                if (Part == null)
                    Part = new Part();

                Part.Name = newItem.Part?.Name;
            }

            if (fields.Contains(Field.DatItem_Part_Interface))
            {
                if (Part == null)
                    Part = new Part();

                Part.Interface = newItem.Part?.Interface;
            }

            // TODO: Handle DatItem_Part_Feature*

            #endregion
        }

        #endregion
    }
}
