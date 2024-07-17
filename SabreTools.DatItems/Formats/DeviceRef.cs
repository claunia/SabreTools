using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents which Device Reference(s) is associated with a set
    /// </summary>
    [JsonObject("device_ref"), XmlRoot("device_ref")]
    public sealed class DeviceRef : DatItem<Models.Metadata.DeviceRef>
    {
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.DeviceRef;

        /// <inheritdoc>/>
        protected override string? NameKey => Models.Metadata.DeviceRef.NameKey;

        #endregion

        #region Constructors

        public DeviceRef() : base() { }
        public DeviceRef(Models.Metadata.DeviceRef item) : base(item) { }

        #endregion
    }
}
