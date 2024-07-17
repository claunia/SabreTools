using System.Xml.Serialization;
using Newtonsoft.Json;

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
        public Device(Models.Metadata.Device item) : base(item) { }

        #endregion
    }
}
