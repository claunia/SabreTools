using System.Collections.Generic;
using System.Xml.Serialization;

using SabreTools.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SabreTools.DatItems
{
    /// <summary>
    /// Represents a single device on the machine
    /// </summary>
    [JsonObject("device"), XmlRoot("device")]
    public class Device : DatItem
    {
        #region Fields

        /// <summary>
        /// Device type
        /// </summary>
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlElement("type")]
        public DeviceType DeviceType { get; set; }

        [JsonIgnore]
        public bool DeviceTypeSpecified { get { return DeviceType != DeviceType.NULL; } }

        /// <summary>
        /// Device tag
        /// </summary>
        [JsonProperty("tag", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("tag")]
        public string Tag { get; set; }

        /// <summary>
        /// Fixed image format
        /// </summary>
        [JsonProperty("fixed_image", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("fixed_image")]
        public string FixedImage { get; set; }

        /// <summary>
        /// Determines if the devices is mandatory
        /// </summary>
        /// <remarks>Only value used seems to be 1. Used like bool, but actually int</remarks>
        [JsonProperty("mandatory", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("mandatory")]
        public long? Mandatory { get; set; }

        [JsonIgnore]
        public bool MandatorySpecified { get { return Mandatory != null; } }

        /// <summary>
        /// Device interface
        /// </summary>
        [JsonProperty("interface", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("interface")]
        public string Interface { get; set; }

        /// <summary>
        /// Device instances
        /// </summary>
        [JsonProperty("instances", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("instances")]
        public List<Instance> Instances { get; set; }

        [JsonIgnore]
        public bool InstancesSpecified { get { return Instances != null && Instances.Count > 0; } }

        /// <summary>
        /// Device extensions
        /// </summary>
        [JsonProperty("extensions", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("extensions")]
        public List<Extension> Extensions { get; set; }

        [JsonIgnore]
        public bool ExtensionsSpecified { get { return Extensions != null && Extensions.Count > 0; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Device object
        /// </summary>
        public Device()
        {
            ItemType = ItemType.Device;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new Device()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                DeviceType = this.DeviceType,
                Tag = this.Tag,
                FixedImage = this.FixedImage,
                Mandatory = this.Mandatory,
                Interface = this.Interface,
                Instances = this.Instances,
                Extensions = this.Extensions,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a Device, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Device
            Device newOther = other as Device;

            // If the Device information matches
            bool match = (DeviceType == newOther.DeviceType
                && Tag == newOther.Tag
                && FixedImage == newOther.FixedImage
                && Mandatory == newOther.Mandatory
                && Interface == newOther.Interface);
            if (!match)
                return match;

            // If the instances match
            if (InstancesSpecified)
            {
                foreach (Instance instance in Instances)
                {
                    match &= newOther.Instances.Contains(instance);
                }
            }

            // If the extensions match
            if (ExtensionsSpecified)
            {
                foreach (Extension extension in Extensions)
                {
                    match &= newOther.Extensions.Contains(extension);
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

            // If we don't have a Device to replace from, ignore specific fields
            if (item.ItemType != ItemType.Device)
                return;

            // Cast for easier access
            Device newItem = item as Device;

            // Replace the fields
            if (datItemFields.Contains(DatItemField.DeviceType))
                DeviceType = newItem.DeviceType;

            if (datItemFields.Contains(DatItemField.Tag))
                Tag = newItem.Tag;

            if (datItemFields.Contains(DatItemField.FixedImage))
                FixedImage = newItem.FixedImage;

            if (datItemFields.Contains(DatItemField.Mandatory))
                Mandatory = newItem.Mandatory;

            if (datItemFields.Contains(DatItemField.Interface))
                Interface = newItem.Interface;

            // DatItem_Instance_* doesn't make sense here
            // since not every instance under the other item
            // can replace every instance under this item

            // DatItem_Extension_* doesn't make sense here
            // since not every extension under the other item
            // can replace every extension under this item
        }

        #endregion
    }
}
