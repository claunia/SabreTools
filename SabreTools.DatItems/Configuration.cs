using System.Collections.Generic;
using System.Xml.Serialization;

using SabreTools.Core;
using Newtonsoft.Json;

namespace SabreTools.DatItems
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

        /// <inheritdoc/>
        public override string GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string name) => Name = name;

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

        #region Sorting and Merging

        /// <inheritdoc/>
        public override void ReplaceFields(
            DatItem item,
            List<DatItemField> datItemFields,
            List<MachineField> machineFields)
        {
            // Replace common fields first
            base.ReplaceFields(item, datItemFields, machineFields);

            // If we don't have a Configuration to replace from, ignore specific fields
            if (item.ItemType != ItemType.Configuration)
                return;

            // Cast for easier access
            Configuration newItem = item as Configuration;

            // Replace the fields
            if (datItemFields.Contains(DatItemField.Name))
                Name = newItem.Name;

            if (datItemFields.Contains(DatItemField.Tag))
                Tag = newItem.Tag;

            if (datItemFields.Contains(DatItemField.Mask))
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
