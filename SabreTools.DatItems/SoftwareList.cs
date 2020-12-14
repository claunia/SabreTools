using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using SabreTools.Core;
using SabreTools.Core.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SabreTools.DatItems
{
    /// <summary>
    /// Represents which SoftwareList(s) is associated with a set
    /// </summary>
    [JsonObject("softwarelist"), XmlRoot("softwarelist")]
    public class SoftwareList : DatItem
    {
        #region Fields

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name")]
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Status of the softare list according to the machine
        /// </summary>
        [JsonProperty("status", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlElement("status")]
        public SoftwareListStatus Status { get; set; }

        [JsonIgnore]
        public bool StatusSpecified { get { return Status != SoftwareListStatus.NULL; } }

        /// <summary>
        /// Filter to apply to the software list
        /// </summary>
        [JsonProperty("filter", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("filter")]
        public string Filter { get; set; }

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

            // Handle SoftwareList-specific fields
            if (datItemMappings.Keys.Contains(DatItemField.Name))
                Name = datItemMappings[DatItemField.Name];

            if (datItemMappings.Keys.Contains(DatItemField.SoftwareListStatus))
                Status = datItemMappings[DatItemField.SoftwareListStatus].AsSoftwareListStatus();

            if (datItemMappings.Keys.Contains(DatItemField.Filter))
                Filter = datItemMappings[DatItemField.Filter];
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty SoftwareList object
        /// </summary>
        public SoftwareList()
        {
            Name = string.Empty;
            ItemType = ItemType.SoftwareList;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new SoftwareList()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Name = this.Name,
                Status = this.Status,
                Filter = this.Filter,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a sample, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a SoftwareList
            SoftwareList newOther = other as SoftwareList;

            // If the SoftwareList information matches
            return (Name == newOther.Name
                && Status == newOther.Status
                && Filter == newOther.Filter);
        }

        #endregion

        #region Filtering

        /// <inheritdoc/>
        public override void RemoveFields(
            List<DatItemField> datItemFields,
            List<MachineField> machineFields)
        {
            // Remove common fields first
            base.RemoveFields(datItemFields, machineFields);

            // Remove the fields
            if (datItemFields.Contains(DatItemField.Name))
                Name = null;

            if (datItemFields.Contains(DatItemField.SoftwareListStatus))
                Status = SoftwareListStatus.NULL;

            if (datItemFields.Contains(DatItemField.Filter))
                Filter = null;
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

            // If we don't have a SoftwareList to replace from, ignore specific fields
            if (item.ItemType != ItemType.SoftwareList)
                return;

            // Cast for easier access
            SoftwareList newItem = item as SoftwareList;

            // Replace the fields
            if (datItemFields.Contains(DatItemField.Name))
                Name = newItem.Name;

            if (datItemFields.Contains(DatItemField.SoftwareListStatus))
                Status = newItem.Status;

            if (datItemFields.Contains(DatItemField.Filter))
                Filter = newItem.Filter;
        }

        #endregion
    }
}
