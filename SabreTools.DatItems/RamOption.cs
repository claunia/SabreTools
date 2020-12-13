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
    /// Represents which RAM option(s) is associated with a set
    /// </summary>
    [JsonObject("ramoption"), XmlRoot("ramoption")]
    public class RamOption : DatItem
    {
        #region Fields

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name")]
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Determine whether the RamOption is default
        /// </summary>
        [JsonProperty("default", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("default")]
        public bool? Default { get; set; }

        [JsonIgnore]
        public bool DefaultSpecified { get { return Default != null; } }

        /// <summary>
        /// Determines the content of the RamOption
        /// </summary>
        [JsonProperty("content", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("content")]
        public string Content { get; set; }

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

            // Handle BiosSet-specific fields
            if (datItemMappings.Keys.Contains(DatItemField.Name))
                Name = datItemMappings[DatItemField.Name];

            if (datItemMappings.Keys.Contains(DatItemField.Default))
                Default = datItemMappings[DatItemField.Default].AsYesNo();

            if (datItemMappings.Keys.Contains(DatItemField.Content))
                Content = datItemMappings[DatItemField.Content];
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty RamOption object
        /// </summary>
        public RamOption()
        {
            Name = string.Empty;
            ItemType = ItemType.RamOption;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new RamOption()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Name = this.Name,
                Default = this.Default,
                Content = this.Content,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a RamOption, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a RamOption
            RamOption newOther = other as RamOption;

            // If the BiosSet information matches
            return (Name == newOther.Name && Default == newOther.Default && Content == newOther.Content);
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
            if (!Filter.PassStringFilter(cleaner.DatItemFilter.Name, Name))
                return false;

            // Filter on default
            if (!Filter.PassBoolFilter(cleaner.DatItemFilter.Default, Default))
                return false;

            // Filter on content
            if (!Filter.PassStringFilter(cleaner.DatItemFilter.Content, Content))
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
            if (datItemFields.Contains(DatItemField.Name))
                Name = null;

            if (datItemFields.Contains(DatItemField.Default))
                Default = null;

            if (datItemFields.Contains(DatItemField.Content))
                Content = null;
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

            // If we don't have a RamOption to replace from, ignore specific fields
            if (item.ItemType != ItemType.RamOption)
                return;

            // Cast for easier access
            RamOption newItem = item as RamOption;

            // Replace the fields
            if (datItemFields.Contains(DatItemField.Name))
                Name = newItem.Name;

            if (datItemFields.Contains(DatItemField.Default))
                Default = newItem.Default;

            if (datItemFields.Contains(DatItemField.Content))
                Content = newItem.Content;
        }

        #endregion
    }
}
