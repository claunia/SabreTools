using System.Xml.Serialization;

using SabreTools.Core;
using Newtonsoft.Json;

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
        [JsonProperty("name")]
        [XmlElement("name")]
        public string Name { get; set; }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string name) => Name = name;

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

        public override object Clone()
        {
            return new DeviceReference()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Name = this.Name,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a device reference, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a device reference
            DeviceReference newOther = other as DeviceReference;

            // If the device reference information matches
            return (Name == newOther.Name);
        }

        #endregion
    }
}
