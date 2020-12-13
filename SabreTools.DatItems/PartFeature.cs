using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using SabreTools.Core;
using SabreTools.Filtering;
using Newtonsoft.Json;

namespace SabreTools.DatItems
{
    /// <summary>
    /// Represents one part feature object
    /// </summary>
    [JsonObject("part_feature"), XmlRoot("part_feature")]
    public class PartFeature : DatItem
    {
        #region Fields

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name")]
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// PartFeature value
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

        /// <inheritdoc/>
        public override void SetFields(
            Dictionary<DatItemField, string> datItemMappings,
            Dictionary<MachineField, string> machineMappings)
        {
            // Set base fields
            base.SetFields(datItemMappings, machineMappings);

            // Handle PartFeature-specific fields
            if (datItemMappings.Keys.Contains(DatItemField.Part_Feature_Name))
                Name = datItemMappings[DatItemField.Part_Feature_Name];

            if (datItemMappings.Keys.Contains(DatItemField.Part_Feature_Value))
                Value = datItemMappings[DatItemField.Part_Feature_Value];
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty PartFeature object
        /// </summary>
        public PartFeature()
        {
            Name = string.Empty;
            ItemType = ItemType.PartFeature;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new PartFeature()
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

            // Otherwise, treat it as a PartFeature
            PartFeature newOther = other as PartFeature;

            // If the archive information matches
            return (Name == newOther.Name && Value == newOther.Value);
        }

        #endregion

        #region Filtering

        /// <inheritdoc/>
        public override bool PassesFilter(Cleaner cleaner, bool sub = false)
        {
            // Check common fields first
            if (!base.PassesFilter(cleaner, sub))
                return false;

            // Filter on name
            if (!Filter.PassStringFilter(cleaner.DatItemFilter.Part_Feature_Name, Name))
                return false;

            // Filter on value
            if (!Filter.PassStringFilter(cleaner.DatItemFilter.Part_Feature_Value, Value))
                return false;

            return true;
        }

        /// <inheritdoc/>
        public override void RemoveFields(
            List<DatItemField> datItemFields,
            List<MachineField> machineFields)
        {
            // Remove common fields first
            base.RemoveFields(datItemFields, machineFields);

            // Remove the fields
            if (datItemFields.Contains(DatItemField.Part_Feature_Name))
                Name = null;

            if (datItemFields.Contains(DatItemField.Part_Feature_Value))
                Value = null;
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

        /// <inheritdoc/>
        public override void ReplaceFields(
            DatItem item,
            List<DatItemField> datItemFields,
            List<MachineField> machineFields)
        {
            // Replace common fields first
            base.ReplaceFields(item, datItemFields, machineFields);

            // If we don't have a PartFeature to replace from, ignore specific fields
            if (item.ItemType != ItemType.PartFeature)
                return;

            // Cast for easier access
            PartFeature newItem = item as PartFeature;

            // Replace the fields
            if (datItemFields.Contains(DatItemField.Part_Feature_Name))
                Name = newItem.Name;

            if (datItemFields.Contains(DatItemField.Part_Feature_Value))
                Value = newItem.Value;
        }

        #endregion
    }
}
