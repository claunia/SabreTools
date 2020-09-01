using Newtonsoft.Json;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Represents which Device Reference(s) is associated with a set
    /// </summary>
    [JsonObject("device_ref")]
    public class DeviceReference : DatItem
    {
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
                Name = this.Name,
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                AltName = this.AltName,
                AltTitle = this.AltTitle,

                Original = this.Original,
                OpenMSXSubType = this.OpenMSXSubType,
                OpenMSXType = this.OpenMSXType,
                Remark = this.Remark,
                Boot = this.Boot,

                Part = this.Part,
                Features = this.Features,
                AreaName = this.AreaName,
                AreaSize = this.AreaSize,
                AreaWidth = this.AreaWidth,
                AreaEndianness = this.AreaEndianness,
                Value = this.Value,
                LoadFlag = this.LoadFlag,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,
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
