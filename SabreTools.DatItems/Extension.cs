using System.Collections.Generic;
using System.Xml.Serialization;

using SabreTools.Core;
using Newtonsoft.Json;

namespace SabreTools.DatItems
{
    /// <summary>
    /// Represents a matchable extension
    /// </summary>
    [JsonObject("extension"), XmlRoot("extension")]
    public class Extension : DatItem
    {
        #region Fields

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name")]
        [XmlElement("name")]
        public string Name { get; set; }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string name) => Name = name;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Extension object
        /// </summary>
        public Extension()
        {
            Name = string.Empty;
            ItemType = ItemType.Extension;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new Extension()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Name = this.Name,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a Extension, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Extension
            Extension newOther = other as Extension;

            // If the Extension information matches
            return (Name == newOther.Name);
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

            // If we don't have a Extension to replace from, ignore specific fields
            if (item.ItemType != ItemType.Extension)
                return;

            // Cast for easier access
            Extension newItem = item as Extension;

            // Replace the fields
            if (datItemFields.Contains(DatItemField.Extension_Name))
                Name = newItem.Name;
        }

        #endregion
    }
}
