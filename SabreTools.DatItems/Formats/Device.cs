using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.Filter;

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
            get => _internal.ReadString(Models.Metadata.Device.DeviceTypeKey).AsDeviceType();
            set => _internal[Models.Metadata.Device.DeviceTypeKey] = value.FromDeviceType();
        }

        [JsonIgnore]
        public bool DeviceTypeSpecified { get { return DeviceType != DeviceType.NULL; } }

        /// <summary>
        /// Device tag
        /// </summary>
        [JsonProperty("tag", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("tag")]
        public string? Tag
        {
            get => _internal.ReadString(Models.Metadata.Device.TagKey);
            set => _internal[Models.Metadata.Device.TagKey] = value;
        }

        /// <summary>
        /// Fixed image format
        /// </summary>
        [JsonProperty("fixed_image", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("fixed_image")]
        public string? FixedImage
        {
            get => _internal.ReadString(Models.Metadata.Device.FixedImageKey);
            set => _internal[Models.Metadata.Device.FixedImageKey] = value;
        }

        /// <summary>
        /// Determines if the devices is mandatory
        /// </summary>
        /// <remarks>Only value used seems to be 1. Used like bool, but actually int</remarks>
        [JsonProperty("mandatory", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("mandatory")]
        public long? Mandatory
        {
            get => _internal.ReadLong(Models.Metadata.Device.MandatoryKey);
            set => _internal[Models.Metadata.Device.MandatoryKey] = value;
        }

        [JsonIgnore]
        public bool MandatorySpecified { get { return Mandatory != null; } }

        /// <summary>
        /// Device interface
        /// </summary>
        [JsonProperty("interface", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("interface")]
        public string? Interface
        {
            get => _internal.ReadString(Models.Metadata.Device.InterfaceKey);
            set => _internal[Models.Metadata.Device.InterfaceKey] = value;
        }

        /// <summary>
        /// Device instances
        /// </summary>
        [JsonProperty("instances", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("instances")]
        public List<Instance>? Instances
        {
            get => _internal.Read<Instance[]>(Models.Metadata.Device.InstanceKey)?.ToList();
            set => _internal[Models.Metadata.Device.InstanceKey] = value?.ToArray();
        }

        [JsonIgnore]
        public bool InstancesSpecified { get { return Instances != null && Instances.Count > 0; } }

        /// <summary>
        /// Device extensions
        /// </summary>
        [JsonProperty("extensions", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("extensions")]
        public List<Extension>? Extensions
        {
            get => _internal.Read<Extension[]>(Models.Metadata.Device.ExtensionKey)?.ToList();
            set => _internal[Models.Metadata.Device.ExtensionKey] = value?.ToArray();
        }

        [JsonIgnore]
        public bool ExtensionsSpecified { get { return Extensions != null && Extensions.Count > 0; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Device object
        /// </summary>
        public Device()
        {
            _internal = new Models.Metadata.Device();
            Machine = new Machine();

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

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.Device ?? [],
            };
        }

        #endregion
    
        #region Manipulation

        /// <inheritdoc/>
        public override bool RemoveField(DatItemField datItemField)
        {
            // Get the correct internal field name
            string? fieldName = datItemField switch
            {
                DatItemField.DeviceType => Models.Metadata.Device.DeviceTypeKey,
                DatItemField.FixedImage => Models.Metadata.Device.FixedImageKey,
                DatItemField.Interface => Models.Metadata.Device.InterfaceKey,
                DatItemField.Mandatory => Models.Metadata.Device.MandatoryKey,
                DatItemField.Tag => Models.Metadata.Device.TagKey,
                _ => null,
            };

            // Remove the field and return
            return FieldManipulator.RemoveField(_internal, fieldName);
        }

        /// <inheritdoc/>
        public override bool SetField(DatItemField datItemField, string value)
        {
            // Get the correct internal field name
            string? fieldName = datItemField switch
            {
                DatItemField.DeviceType => Models.Metadata.Device.DeviceTypeKey,
                DatItemField.FixedImage => Models.Metadata.Device.FixedImageKey,
                DatItemField.Interface => Models.Metadata.Device.InterfaceKey,
                DatItemField.Mandatory => Models.Metadata.Device.MandatoryKey,
                DatItemField.Tag => Models.Metadata.Device.TagKey,
                _ => null,
            };

            // Set the field and return
            return FieldManipulator.SetField(_internal, fieldName, value);
        }

        #endregion
    }
}
