using System.Collections.Generic;
using System.Xml.Serialization;

using SabreTools.Core;
using Newtonsoft.Json;

namespace SabreTools.DatItems
{
    /// <summary>
    /// Represents a single instance of another item
    /// </summary>
    [JsonObject("instance"), XmlRoot("instance")]
    public class Instance : DatItem
    {
        #region Fields

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name")]
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Short name for the instance
        /// </summary>
        [JsonProperty("briefname", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("briefname")]
        public string BriefName { get; set; }

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
        /// Create a default, empty Instance object
        /// </summary>
        public Instance()
        {
            Name = string.Empty;
            ItemType = ItemType.Instance;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new Instance()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Name = this.Name,
                BriefName = this.BriefName,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a Instance, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Instance
            Instance newOther = other as Instance;

            // If the Instance information matches
            return (Name == newOther.Name && BriefName == newOther.BriefName);
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

            // If we don't have a Instance to replace from, ignore specific fields
            if (item.ItemType != ItemType.Instance)
                return;

            // Cast for easier access
            Instance newItem = item as Instance;

            // Replace the fields
            if (datItemFields.Contains(DatItemField.Instance_Name))
                Name = newItem.Name;

            if (datItemFields.Contains(DatItemField.Instance_BriefName))
                BriefName = newItem.BriefName;
        }

        #endregion
    }
}
