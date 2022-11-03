using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents the a driver of the machine
    /// </summary>
    /// <remarks>
    /// TODO: Add new fields to documentation
    /// </remarks>
    [JsonObject("driver"), XmlRoot("driver")]
    public class Driver : DatItem
    {
        #region Fields

        /// <summary>
        /// Overall driver status
        /// </summary>
        [JsonProperty("status", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public SupportStatus Status { get; set; }

        [JsonIgnore]
        public bool StatusSpecified { get { return Status != SupportStatus.NULL; } }

        /// <summary>
        /// Driver emulation status
        /// </summary>
        [JsonProperty("emulation", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("emulation")]
        [JsonConverter(typeof(StringEnumConverter))]
        public SupportStatus Emulation { get; set; }

        [JsonIgnore]
        public bool EmulationSpecified { get { return Emulation != SupportStatus.NULL; } }

        /// <summary>
        /// Cocktail orientation status
        /// </summary>
        [JsonProperty("cocktail", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("cocktail")]
        [JsonConverter(typeof(StringEnumConverter))]
        public SupportStatus Cocktail { get; set; }

        [JsonIgnore]
        public bool CocktailSpecified { get { return Cocktail != SupportStatus.NULL; } }

        /// <summary>
        /// Save state support status
        /// </summary>
        [JsonProperty("savestate", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("savestate")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Supported SaveState { get; set; }

        [JsonIgnore]
        public bool SaveStateSpecified { get { return SaveState != Supported.NULL; } }

        /// <summary>
        /// Requires artwork
        /// </summary>
        [JsonProperty("requiresartwork", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("requiresartwork")]
        public bool? RequiresArtwork { get; set; }

        [JsonIgnore]
        public bool RequiresArtworkSpecified { get { return RequiresArtwork != null; } }

        /// <summary>
        /// Unofficial
        /// </summary>
        [JsonProperty("unofficial", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("unofficial")]
        public bool? Unofficial { get; set; }

        [JsonIgnore]
        public bool UnofficialSpecified { get { return Unofficial != null; } }

        /// <summary>
        /// No sound hardware
        /// </summary>
        [JsonProperty("nosoundhardware", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("nosoundhardware")]
        public bool? NoSoundHardware { get; set; }

        [JsonIgnore]
        public bool NoSoundHardwareSpecified { get { return NoSoundHardware != null; } }

        /// <summary>
        /// Incomplete
        /// </summary>
        [JsonProperty("incomplete", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("incomplete")]
        public bool? Incomplete { get; set; }

        [JsonIgnore]
        public bool IncompleteSpecified { get { return Incomplete != null; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Driver object
        /// </summary>
        public Driver()
        {
            ItemType = ItemType.Driver;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Driver()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Status = this.Status,
                Emulation = this.Emulation,
                Cocktail = this.Cocktail,
                SaveState = this.SaveState,
                RequiresArtwork = this.RequiresArtwork,
                Unofficial = this.Unofficial,
                NoSoundHardware = this.NoSoundHardware,
                Incomplete = this.Incomplete,
            };
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem other)
        {
            // If we don't have a Driver, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Driver
            Driver newOther = other as Driver;

            // If the Feature information matches
            return (Status == newOther.Status
                && Emulation == newOther.Emulation
                && Cocktail == newOther.Cocktail
                && SaveState == newOther.SaveState
                && RequiresArtwork == newOther.RequiresArtwork
                && Unofficial == newOther.Unofficial
                && NoSoundHardware == newOther.NoSoundHardware
                && Incomplete == newOther.Incomplete);
        }

        #endregion
    }
}
