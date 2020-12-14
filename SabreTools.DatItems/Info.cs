using System.Collections.Generic;
using System.Xml.Serialization;

using SabreTools.Core;
using Newtonsoft.Json;

namespace SabreTools.DatItems
{
    /// <summary>
    /// Represents special information about a machine
    /// </summary>
    [JsonObject("info"), XmlRoot("info")]
    public class Info : DatItem
    {
        #region Fields

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name")]
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Information value
        /// </summary>
        [JsonProperty("value")]
        [XmlElement("value")]
        public string Value { get; set; }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string GetName()
        {
            return Name;
        }

        /// <inheritdoc/>
        public override void SetName(string name)
        {
            Name = name;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Info object
        /// </summary>
        public Info()
        {
            Name = string.Empty;
            ItemType = ItemType.Info;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new Info()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Name = this.Name,
                Value = this.Value,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a sample, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Info
            Info newOther = other as Info;

            // If the archive information matches
            return (Name == newOther.Name && Value == newOther.Value);
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

            // If we don't have a Info to replace from, ignore specific fields
            if (item.ItemType != ItemType.Info)
                return;

            // Cast for easier access
            Info newItem = item as Info;

            // Replace the fields
            if (datItemFields.Contains(DatItemField.Name))
                Name = newItem.Name;

            if (datItemFields.Contains(DatItemField.Value))
                Value = newItem.Value;
        }

        #endregion
    }
}
