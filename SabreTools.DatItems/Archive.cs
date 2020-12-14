using System.Collections.Generic;
using System.Xml.Serialization;

using SabreTools.Core;
using Newtonsoft.Json;

namespace SabreTools.DatItems
{
    /// <summary>
    /// Represents generic archive files to be included in a set
    /// </summary>
    [JsonObject("archive"), XmlRoot("archive")]
    public class Archive : DatItem
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
        /// Create a default, empty Archive object
        /// </summary>
        public Archive()
        {
            Name = string.Empty;
            ItemType = ItemType.Archive;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new Archive()
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
            // If we don't have an archive, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as an archive
            Archive newOther = other as Archive;

            // If the archive information matches
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

            // If we don't have a Archive to replace from, ignore specific fields
            if (item.ItemType != ItemType.Archive)
                return;

            // Cast for easier access
            Archive newItem = item as Archive;

            // Replace the fields
            if (datItemFields.Contains(DatItemField.Name))
                Name = newItem.Name;
        }

        #endregion
    }
}
