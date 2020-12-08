using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using SabreTools.Data;
using SabreTools.Library.Filtering;
using SabreTools.Library.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SabreTools.Library.DatItems
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

        #region Accessors

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="mappings">Mappings dictionary</param>
        public override void SetFields(Dictionary<Field, string> mappings)
        {
            // Set base fields
            base.SetFields(mappings);

            // Handle Device-specific fields
            if (mappings.Keys.Contains(Field.DatItem_DeviceType))
                DeviceType = mappings[Field.DatItem_DeviceType].AsDeviceType();

            if (mappings.Keys.Contains(Field.DatItem_Tag))
                Tag = mappings[Field.DatItem_Tag];

            if (mappings.Keys.Contains(Field.DatItem_FixedImage))
                FixedImage = mappings[Field.DatItem_FixedImage];

            if (mappings.Keys.Contains(Field.DatItem_Mandatory))
                Mandatory = Sanitizer.CleanLong(mappings[Field.DatItem_Mandatory]);

            if (mappings.Keys.Contains(Field.DatItem_Interface))
                Interface = mappings[Field.DatItem_Interface];

            if (InstancesSpecified)
            {
                foreach (Instance instance in Instances)
                {
                    instance.SetFields(mappings);
                }
            }

            if (ExtensionsSpecified)
            {
                foreach (Extension extension in Extensions)
                {
                    extension.SetFields(mappings);
                }
            }
        }

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

        #region Filtering

        /// <summary>
        /// Check to see if a DatItem passes the filter
        /// </summary>
        /// <param name="filter">Filter to check against</param>
        /// <param name="sub">True if this is a subitem, false otherwise</param>
        /// <returns>True if the item passed the filter, false otherwise</returns>
        public override bool PassesFilter(Filter filter, bool sub = false)
        {
            // Check common fields first
            if (!base.PassesFilter(filter, sub))
                return false;

            // Filter on device type
            if (filter.DatItem_DeviceType.MatchesPositive(DeviceType.NULL, DeviceType) == false)
                return false;
            if (filter.DatItem_DeviceType.MatchesNegative(DeviceType.NULL, DeviceType) == true)
                return false;

            // Filter on tag
            if (!filter.PassStringFilter(filter.DatItem_Tag, Tag))
                return false;

            // Filter on fixed image
            if (!filter.PassStringFilter(filter.DatItem_FixedImage, FixedImage))
                return false;

            // Filter on mandatory
            if (!filter.PassLongFilter(filter.DatItem_Mandatory, Mandatory))
                return false;

            // Filter on interface
            if (!filter.PassStringFilter(filter.DatItem_Interface, Interface))
                return false;

            // Filter on individual instances
            if (InstancesSpecified)
            {
                foreach (Instance instance in Instances)
                {
                    if (!instance.PassesFilter(filter, true))
                        return false;
                }
            }

            // Filter on individual extensions
            if (ExtensionsSpecified)
            {
                foreach (Extension extension in Extensions)
                {
                    if (!extension.PassesFilter(filter, true))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Remove fields from the DatItem
        /// </summary>
        /// <param name="fields">List of Fields to remove</param>
        public override void RemoveFields(List<Field> fields)
        {
            // Remove common fields first
            base.RemoveFields(fields);

            // Remove the fields
            if (fields.Contains(Field.DatItem_DeviceType))
                DeviceType = DeviceType.NULL;

            if (fields.Contains(Field.DatItem_Tag))
                Tag = null;

            if (fields.Contains(Field.DatItem_FixedImage))
                FixedImage = null;

            if (fields.Contains(Field.DatItem_Mandatory))
                Mandatory = null;

            if (fields.Contains(Field.DatItem_Interface))
                Interface = null;

            if (InstancesSpecified)
            {
                foreach (Instance instance in Instances)
                {
                    instance.RemoveFields(fields);
                }
            }

            if (ExtensionsSpecified)
            {
                foreach (Extension extension in Extensions)
                {
                    extension.RemoveFields(fields);
                }
            }
        }

        #endregion

        #region Sorting and Merging

        /// <summary>
        /// Replace fields from another item
        /// </summary>
        /// <param name="item">DatItem to pull new information from</param>
        /// <param name="fields">List of Fields representing what should be updated</param>
        public override void ReplaceFields(DatItem item, List<Field> fields)
        {
            // Replace common fields first
            base.ReplaceFields(item, fields);

            // If we don't have a Device to replace from, ignore specific fields
            if (item.ItemType != ItemType.Device)
                return;

            // Cast for easier access
            Device newItem = item as Device;

            // Replace the fields
            if (fields.Contains(Field.DatItem_DeviceType))
                DeviceType = newItem.DeviceType;

            if (fields.Contains(Field.DatItem_Tag))
                Tag = newItem.Tag;

            if (fields.Contains(Field.DatItem_FixedImage))
                FixedImage = newItem.FixedImage;

            if (fields.Contains(Field.DatItem_Mandatory))
                Mandatory = newItem.Mandatory;

            if (fields.Contains(Field.DatItem_Interface))
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
