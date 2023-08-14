using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
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
        [JsonProperty("name"), XmlElement("name")]
        public string? Name
        {
            get => _deviceRef.ReadString(Models.Internal.DeviceRef.NameKey);
            set => _deviceRef[Models.Internal.DeviceRef.NameKey] = value;
        }

        /// <summary>
        /// Internal DeviceRef model
        /// </summary>
        [JsonIgnore]
        private Models.Internal.DeviceRef _deviceRef = new();

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

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

        /// <inheritdoc/>
        public override object Clone()
        {
            return new DeviceReference()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine?.Clone() as Machine,
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _deviceRef = this._deviceRef?.Clone() as Models.Internal.DeviceRef ?? new Models.Internal.DeviceRef(),
            };
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If we don't have a Adjuster, return false
            if (ItemType != other?.ItemType || other is not DeviceReference otherInternal)
                return false;

            // Compare the internal models
            return _deviceRef.EqualTo(otherInternal._deviceRef);
        }

        #endregion
    }
}
