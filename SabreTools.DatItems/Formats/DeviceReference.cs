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
        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => GetFieldValue<string>(Models.Metadata.DeviceRef.NameKey);

        /// <inheritdoc/>
        public override void SetName(string? name) => SetFieldValue(Models.Metadata.DeviceRef.NameKey, name);

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty DeviceReference object
        /// </summary>
        public DeviceReference()
        {
            _internal = new Models.Metadata.DeviceRef();
            Machine = new Machine();

            SetName(string.Empty);
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

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.DeviceRef ?? [],
            };
        }

        #endregion
    }
}
