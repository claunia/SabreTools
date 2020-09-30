using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using SabreTools.Library.Filtering;
using SabreTools.Library.Tools;
using Newtonsoft.Json;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Represents which Configuration(s) is associated with a set
    /// </summary>
    [JsonObject("configuration"), XmlRoot("configuration")]
    public class Configuration : DatItem
    {
        #region Fields

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name")]
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Tag associated with the configuration
        /// </summary>
        [JsonProperty("tag", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("tag")]
        public string Tag { get; set; }

        /// <summary>
        /// Mask associated with the configuration
        /// </summary>
        [JsonProperty("mask", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("mask")]
        public string Mask { get; set; }

        /// <summary>
        /// Conditions associated with the configuration
        /// </summary>
        [JsonProperty("conditions", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("conditions")]
        public List<Condition> Conditions { get; set; }

        [JsonIgnore]
        public bool ConditionsSpecified { get { return Conditions != null && Conditions.Count > 0; } }

        /// <summary>
        /// Locations associated with the configuration
        /// </summary>
        [JsonProperty("locations", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("locations")]
        public List<Location> Locations { get; set; }

        [JsonIgnore]
        public bool LocationsSpecified { get { return Locations != null && Locations.Count > 0; } }

        /// <summary>
        /// Settings associated with the configuration
        /// </summary>
        [JsonProperty("settings", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("settings")]
        public List<Setting> Settings { get; set; }

        [JsonIgnore]
        public bool SettingsSpecified { get { return Settings != null && Settings.Count > 0; } }

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

            // Handle Configuration-specific fields
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

            if (SettingsSpecified)
            {
                foreach (Setting setting in Settings)
                {
                    setting.SetFields(mappings);
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Configuration object
        /// </summary>
        public Configuration()
        {
            Name = string.Empty;
            ItemType = ItemType.Configuration;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new Configuration()
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
                Settings = this.Settings,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a Configuration, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Configuration
            Configuration newOther = other as Configuration;

            // If the Configuration information matches
            bool match = (Name == newOther.Name
                && Tag == newOther.Tag
                && Mask == newOther.Mask);
            if (!match)
                return match;

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

            // If the settings match
            if (SettingsSpecified)
            {
                foreach (Setting setting in Settings)
                {
                    match &= newOther.Settings.Contains(setting);
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
            if (SettingsSpecified)
            {
                foreach (Setting setting in Settings)
                {
                    if (!setting.PassesFilter(filter, true))
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

            if (SettingsSpecified)
            {
                foreach (Setting setting in Settings)
                {
                    setting.RemoveFields(fields);
                }
            }
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

            // If we don't have a Configuration to replace from, ignore specific fields
            if (item.ItemType != ItemType.Configuration)
                return;

            // Cast for easier access
            Configuration newItem = item as Configuration;

            // Replace the fields
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
            // since not every setting under the other item
            // can replace every setting under this item
        }

        #endregion
    }
}
