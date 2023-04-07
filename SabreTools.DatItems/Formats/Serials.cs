using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

// TODO: Add item mappings for all fields
namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents a single serials item
    /// </summary>
    [JsonObject("serials"), XmlRoot("serials")]
    public class Serials : DatItem
    {
        #region Fields

        /// <summary>
        /// Media serial 1 value
        /// </summary>
        [JsonProperty("media_serial1", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("media_serial1")]
        public string MediaSerial1 { get; set; }

        /// <summary>
        /// Media serial 2 value
        /// </summary>
        [JsonProperty("media_serial2", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("media_serial2")]
        public string MediaSerial2 { get; set; }

        /// <summary>
        /// PCB serial value
        /// </summary>
        [JsonProperty("pcb_serial", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("pcb_serial")]
        public string PcbSerial { get; set; }

        /// <summary>
        /// Rom chip serial 1 value
        /// </summary>
        [JsonProperty("romchip_serial1", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("romchip_serial1")]
        public string RomChipSerial1 { get; set; }

        /// <summary>
        /// Rom chip serial 2 value
        /// </summary>
        [JsonProperty("romchip_serial2", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("romchip_serial2")]
        public string RomChipSerial2 { get; set; }

        /// <summary>
        /// Chip serial value
        /// </summary>
        [JsonProperty("chip_serial", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("chip_serial")]
        public string ChipSerial { get; set; }

        /// <summary>
        /// Lockout serial value
        /// </summary>
        [JsonProperty("lockout_serial", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("lockout_serial")]
        public string LockoutSerial { get; set; }

        /// <summary>
        /// Save chip serial value
        /// </summary>
        [JsonProperty("savechip_serial", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("savechip_serial")]
        public string SaveChipSerial { get; set; }

        /// <summary>
        /// Media stamp value
        /// </summary>
        [JsonProperty("mediastamp", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("mediastamp")]
        public string MediaStamp { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Serials object
        /// </summary>
        public Serials()
        {
            ItemType = ItemType.Serials;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Serials()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                MediaSerial1 = this.MediaSerial1,
                MediaSerial2 = this.MediaSerial2,
                PcbSerial = this.PcbSerial,
                RomChipSerial1 = this.RomChipSerial1,
                RomChipSerial2 = this.RomChipSerial2,
                ChipSerial = this.ChipSerial,
                LockoutSerial = this.LockoutSerial,
                SaveChipSerial = this.SaveChipSerial,
                MediaStamp = this.MediaStamp,
            };
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem other)
        {
            // If we don't have a Serials, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Serials
            Serials newOther = other as Serials;

            // If the Serials information matches
            return (MediaSerial1 == newOther.MediaSerial1
                && MediaSerial2 == newOther.MediaSerial2
                && PcbSerial == newOther.PcbSerial
                && RomChipSerial1 == newOther.RomChipSerial1
                && RomChipSerial2 == newOther.RomChipSerial2
                && ChipSerial == newOther.ChipSerial
                && LockoutSerial == newOther.LockoutSerial
                && SaveChipSerial == newOther.SaveChipSerial
                && MediaStamp == newOther.MediaStamp);
        }

        #endregion
    }
}
