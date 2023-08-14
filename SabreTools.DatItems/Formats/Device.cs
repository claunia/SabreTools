using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SabreTools.Core;
using SabreTools.Core.Tools;

namespace SabreTools.DatItems.Formats
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
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public DeviceType DeviceType
        {
            get => _device.ReadString(Models.Internal.Device.DeviceTypeKey).AsDeviceType();
            set => _device[Models.Internal.Device.DeviceTypeKey] = value.FromDeviceType();
        }

        [JsonIgnore]
        public bool DeviceTypeSpecified { get { return DeviceType != DeviceType.NULL; } }

        /// <summary>
        /// Device tag
        /// </summary>
        [JsonProperty("tag", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("tag")]
        public string? Tag
        {
            get => _device.ReadString(Models.Internal.Device.TagKey);
            set => _device[Models.Internal.Device.TagKey] = value;
        }

        /// <summary>
        /// Fixed image format
        /// </summary>
        [JsonProperty("fixed_image", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("fixed_image")]
        public string? FixedImage
        {
            get => _device.ReadString(Models.Internal.Device.FixedImageKey);
            set => _device[Models.Internal.Device.FixedImageKey] = value;
        }

        /// <summary>
        /// Determines if the devices is mandatory
        /// </summary>
        /// <remarks>Only value used seems to be 1. Used like bool, but actually int</remarks>
        [JsonProperty("mandatory", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("mandatory")]
        public long? Mandatory
        {
            get => _device.ReadLong(Models.Internal.Device.MandatoryKey);
            set => _device[Models.Internal.Device.MandatoryKey] = value;
        }

        [JsonIgnore]
        public bool MandatorySpecified { get { return Mandatory != null; } }

        /// <summary>
        /// Device interface
        /// </summary>
        [JsonProperty("interface", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("interface")]
        public string? Interface
        {
            get => _device.ReadString(Models.Internal.Device.InterfaceKey);
            set => _device[Models.Internal.Device.InterfaceKey] = value;
        }

        /// <summary>
        /// Device instances
        /// </summary>
        [JsonProperty("instances", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("instances")]
        public List<Instance>? Instances
        {
            get => _device.Read<Instance[]>(Models.Internal.Device.InstanceKey)?.ToList();
            set => _device[Models.Internal.Device.InstanceKey] = value?.ToArray();
        }

        [JsonIgnore]
        public bool InstancesSpecified { get { return Instances != null && Instances.Count > 0; } }

        /// <summary>
        /// Device extensions
        /// </summary>
        [JsonProperty("extensions", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("extensions")]
        public List<Extension>? Extensions
        {
            get => _device.Read<Extension[]>(Models.Internal.Device.ExtensionKey)?.ToList();
            set => _device[Models.Internal.Device.ExtensionKey] = value?.ToArray();
        }

        [JsonIgnore]
        public bool ExtensionsSpecified { get { return Extensions != null && Extensions.Count > 0; } }

        /// <summary>
        /// Internal Device model
        /// </summary>
        [JsonIgnore]
        private Models.Internal.Device _device = new();

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

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Device()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine?.Clone() as Machine,
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _device = this._device?.Clone() as Models.Internal.Device ?? new Models.Internal.Device(),
            };
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If we don't have a Adjuster, return false
            if (ItemType != other?.ItemType || other is not Device otherInternal)
                return false;

            // Compare the internal models
            return _device.EqualTo(otherInternal._device);
        }

        #endregion
    }
}
