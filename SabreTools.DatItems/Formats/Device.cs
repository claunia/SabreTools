using System;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;
using SabreTools.Core.Tools;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents a single device on the machine
    /// </summary>
    [JsonObject("device"), XmlRoot("device")]
    public sealed class Device : DatItem<Models.Metadata.Device>
    {
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.Device;

        /// <inheritdoc>/>
        protected override string? NameKey => null;

        [JsonIgnore]
        public bool InstancesSpecified
        {
            get
            {
                var instances = GetFieldValue<Instance[]?>(Models.Metadata.Device.InstanceKey);
                return instances != null && instances.Length > 0;
            }
        }

        [JsonIgnore]
        public bool ExtensionsSpecified
        {
            get
            {
                var extensions = GetFieldValue<Extension[]?>(Models.Metadata.Device.ExtensionKey);
                return extensions != null && extensions.Length > 0;
            }
        }

        #endregion

        #region Constructors

        public Device() : base() { }
        public Device(Models.Metadata.Device item) : base(item)
        {
            // Process flag values
            if (GetBoolFieldValue(Models.Metadata.Device.MandatoryKey) != null)
                SetFieldValue<string?>(Models.Metadata.Device.MandatoryKey, GetBoolFieldValue(Models.Metadata.Device.MandatoryKey).FromYesNo());
            if (GetStringFieldValue(Models.Metadata.Device.DeviceTypeKey) != null)
                SetFieldValue<string?>(Models.Metadata.Device.DeviceTypeKey, GetStringFieldValue(Models.Metadata.Device.DeviceTypeKey).AsEnumValue<DeviceType>().AsStringValue());

            // Handle subitems
            var instance = item.Read<Models.Metadata.Instance>(Models.Metadata.Device.InstanceKey);
            if (instance != null)
                SetFieldValue<Instance?>(Models.Metadata.Device.InstanceKey, new Instance(instance));

            var extensions = item.ReadItemArray<Models.Metadata.Extension>(Models.Metadata.Device.ExtensionKey);
            if (extensions != null)
            {
                Extension[] extensionItems = Array.ConvertAll(extensions, extension => new Extension(extension));
                SetFieldValue<Extension[]?>(Models.Metadata.Device.ExtensionKey, extensionItems);
            }
        }

        #endregion
    }
}
