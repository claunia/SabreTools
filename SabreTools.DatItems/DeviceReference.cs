using System.Collections.Generic;
using System.Xml.Serialization;

using SabreTools.Core;
using Newtonsoft.Json;

namespace SabreTools.DatItems
{
    /// <summary>
    /// Represents which Device Reference(s) is associated with a set
    /// </summary>
    [JsonObject("device_ref"), XmlRoot("device_ref")]
    public class DeviceReference : DatItem
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
        /// Create a default, empty DeviceReference object
        /// </summary>
        public DeviceReference()
        {
            Name = string.Empty;
            ItemType = ItemType.DeviceReference;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new DeviceReference()
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
            // If we don't have a device reference, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a device reference
            DeviceReference newOther = other as DeviceReference;

            // If the device reference information matches
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

            // If we don't have a DeviceReference to replace from, ignore specific fields
            if (item.ItemType != ItemType.DeviceReference)
                return;

            // Cast for easier access
            DeviceReference newItem = item as DeviceReference;

            // Replace the fields
            if (datItemFields.Contains(DatItemField.Name))
                Name = newItem.Name;
        }

        #endregion
    }
}
