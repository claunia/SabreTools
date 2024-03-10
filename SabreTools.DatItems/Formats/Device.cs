using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents a single device on the machine
    /// </summary>
    [JsonObject("device"), XmlRoot("device")]
    public class Device : DatItem
    {
        #region Fields

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
    }
}
