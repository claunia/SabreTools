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
    /// Represents one ListXML slotoption
    /// </summary>
    [JsonObject("slotoption"), XmlRoot("slotoption")]
    public class SlotOption : DatItem
    {
        #region Fields

        /// <summary>
        /// Slot option name
        /// </summary>
        [JsonProperty("name")]
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Referenced device name
        /// </summary>
        [JsonProperty("devname")]
        [XmlElement("devname")]
        public string DeviceName { get; set; }

        /// <summary>
        /// Determines if this slot option is default or not
        /// </summary>
        [JsonProperty("default", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("default")]
        public bool? Default { get; set; }

        [JsonIgnore]
        public bool DefaultSpecified { get { return Default != null; } }

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

            // Handle SlotOption-specific fields
            if (datItemMappings.Keys.Contains(DatItemField.SlotOption_Name))
                Name = datItemMappings[DatItemField.SlotOption_Name];

            if (datItemMappings.Keys.Contains(DatItemField.SlotOption_DeviceName))
                DeviceName = datItemMappings[DatItemField.SlotOption_DeviceName];

            if (datItemMappings.Keys.Contains(DatItemField.SlotOption_Default))
                Default = datItemMappings[DatItemField.SlotOption_Default].AsYesNo();
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty SlotOption object
        /// </summary>
        public SlotOption()
        {
            Name = string.Empty;
            ItemType = ItemType.SlotOption;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new SlotOption()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Name = this.Name,
                DeviceName = this.DeviceName,
                Default = this.Default,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a SlotOption, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a SlotOption
            SlotOption newOther = other as SlotOption;

            // If the SlotOption information matches
            return (Name == newOther.Name
                && DeviceName == newOther.DeviceName
                && Default == newOther.Default);
        }

        #endregion

        #region Filtering

        /// <inheritdoc/>
        public override bool PassesFilter(Cleaner cleaner, bool sub = false)
        {
            // Check common fields first
            if (!base.PassesFilter(cleaner, sub))
                return false;

            // Filter on item name
            if (!Filter.PassStringFilter(cleaner.DatItemFilter.SlotOption_Name, Name))
                return false;

            // Filter on device name
            if (!Filter.PassStringFilter(cleaner.DatItemFilter.SlotOption_DeviceName, DeviceName))
                return false;

            // Filter on default
            if (!Filter.PassBoolFilter(cleaner.DatItemFilter.SlotOption_Default, Default))
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
            if (datItemFields.Contains(DatItemField.SlotOption_Name))
                Name = null;

            if (datItemFields.Contains(DatItemField.SlotOption_DeviceName))
                DeviceName = null;

            if (datItemFields.Contains(DatItemField.SlotOption_Default))
                Default = null;
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

            // If we don't have a SlotOption to replace from, ignore specific fields
            if (item.ItemType != ItemType.SlotOption)
                return;

            // Cast for easier access
            SlotOption newItem = item as SlotOption;

            // Replace the fields
            if (datItemFields.Contains(DatItemField.SlotOption_Name))
                Name = newItem.Name;

            if (datItemFields.Contains(DatItemField.SlotOption_DeviceName))
                DeviceName = newItem.DeviceName;

            if (datItemFields.Contains(DatItemField.SlotOption_Default))
                Default = newItem.Default;
        }

        #endregion
    }
}
