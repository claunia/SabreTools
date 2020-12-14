using System.Collections.Generic;
using System.Xml.Serialization;

using SabreTools.Core;
using Newtonsoft.Json;

namespace SabreTools.DatItems
{
    /// <summary>
    /// Represents one ListXML confsetting or dipvalue
    /// </summary>
    [JsonObject("setting"), XmlRoot("setting")]
    public class Setting : DatItem
    {
        #region Fields

        /// <summary>
        /// Setting name
        /// </summary>
        [JsonProperty("name")]
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Setting value
        /// </summary>
        [JsonProperty("value", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("value")]
        public string Value { get; set; }

        /// <summary>
        /// Determines if the setting is default or not
        /// </summary>
        [JsonProperty("default", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("default")]
        public bool? Default { get; set; }

        [JsonIgnore]
        public bool DefaultSpecified { get { return Default != null; } }

        /// <summary>
        /// List of conditions on the setting
        /// </summary>
        [JsonProperty("conditions", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("conditions")]
        public List<Condition> Conditions { get; set; }

        [JsonIgnore]
        public bool ConditionsSpecified { get { return Conditions != null && Conditions.Count > 0; } }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string name) => Name = name;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Setting object
        /// </summary>
        public Setting()
        {
            Name = string.Empty;
            ItemType = ItemType.Setting;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new Setting()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Name = this.Name,
                Value = this.Value,
                Default = this.Default,
                Conditions = this.Conditions,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a Setting, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Setting
            Setting newOther = other as Setting;

            // If the Setting information matches
            bool match = (Name == newOther.Name
                && Value == newOther.Value
                && Default == newOther.Default);
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

            // If we don't have a Setting to replace from, ignore specific fields
            if (item.ItemType != ItemType.Setting)
                return;

            // Cast for easier access
            Setting newItem = item as Setting;

            // Replace the fields
            if (datItemFields.Contains(DatItemField.Setting_Name))
                Name = newItem.Name;

            if (datItemFields.Contains(DatItemField.Setting_Value))
                Value = newItem.Value;

            if (datItemFields.Contains(DatItemField.Setting_Default))
                Default = newItem.Default;

            // DatItem_Condition_* doesn't make sense here
            // since not every condition under the other item
            // can replace every condition under this item
        }

        #endregion
    }
}
