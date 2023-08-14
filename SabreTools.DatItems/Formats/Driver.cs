using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SabreTools.Core;
using SabreTools.Core.Tools;

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
        public SupportStatus Status
        {
            get => _driver.ReadString(Models.Internal.Driver.StatusKey).AsSupportStatus();
            set => _driver[Models.Internal.Driver.StatusKey] = value.FromSupportStatus();
        }

        [JsonIgnore]
        public bool StatusSpecified { get { return Status != SupportStatus.NULL; } }

        /// <summary>
        /// Driver emulation status
        /// </summary>
        [JsonProperty("emulation", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("emulation")]
        [JsonConverter(typeof(StringEnumConverter))]
        public SupportStatus Emulation
        {
            get => _driver.ReadString(Models.Internal.Driver.EmulationKey).AsSupportStatus();
            set => _driver[Models.Internal.Driver.EmulationKey] = value.FromSupportStatus();
        }

        [JsonIgnore]
        public bool EmulationSpecified { get { return Emulation != SupportStatus.NULL; } }

        /// <summary>
        /// Cocktail orientation status
        /// </summary>
        [JsonProperty("cocktail", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("cocktail")]
        [JsonConverter(typeof(StringEnumConverter))]
        public SupportStatus Cocktail
        {
            get => _driver.ReadString(Models.Internal.Driver.CocktailKey).AsSupportStatus();
            set => _driver[Models.Internal.Driver.CocktailKey] = value.FromSupportStatus();
        }

        [JsonIgnore]
        public bool CocktailSpecified { get { return Cocktail != SupportStatus.NULL; } }

        /// <summary>
        /// Save state support status
        /// </summary>
        [JsonProperty("savestate", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("savestate")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Supported SaveState
        {
            get => _driver.ReadString(Models.Internal.Driver.SaveStateKey).AsSupported();
            set => _driver[Models.Internal.Driver.SaveStateKey] = value.FromSupported(verbose: true);
        }

        [JsonIgnore]
        public bool SaveStateSpecified { get { return SaveState != Supported.NULL; } }

        /// <summary>
        /// Requires artwork
        /// </summary>
        [JsonProperty("requiresartwork", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("requiresartwork")]
        public bool? RequiresArtwork
        {
            get => _driver.ReadBool(Models.Internal.Driver.RequiresArtworkKey);
            set => _driver[Models.Internal.Driver.RequiresArtworkKey] = value;
        }

        [JsonIgnore]
        public bool RequiresArtworkSpecified { get { return RequiresArtwork != null; } }

        /// <summary>
        /// Unofficial
        /// </summary>
        [JsonProperty("unofficial", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("unofficial")]
        public bool? Unofficial
        {
            get => _driver.ReadBool(Models.Internal.Driver.UnofficialKey);
            set => _driver[Models.Internal.Driver.UnofficialKey] = value;
        }

        [JsonIgnore]
        public bool UnofficialSpecified { get { return Unofficial != null; } }

        /// <summary>
        /// No sound hardware
        /// </summary>
        [JsonProperty("nosoundhardware", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("nosoundhardware")]
        public bool? NoSoundHardware
        {
            get => _driver.ReadBool(Models.Internal.Driver.NoSoundHardwareKey);
            set => _driver[Models.Internal.Driver.NoSoundHardwareKey] = value;
        }

        [JsonIgnore]
        public bool NoSoundHardwareSpecified { get { return NoSoundHardware != null; } }

        /// <summary>
        /// Incomplete
        /// </summary>
        [JsonProperty("incomplete", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("incomplete")]
        public bool? Incomplete
        {
            get => _driver.ReadBool(Models.Internal.Driver.IncompleteKey);
            set => _driver[Models.Internal.Driver.IncompleteKey] = value;
        }

        [JsonIgnore]
        public bool IncompleteSpecified { get { return Incomplete != null; } }

        /// <summary>
        /// Internal Driver model
        /// </summary>
        [JsonIgnore]
        private Models.Internal.Driver _driver = new();

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

                Machine = this.Machine?.Clone() as Machine,
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _driver = this._driver?.Clone() as Models.Internal.Driver ?? new Models.Internal.Driver(),
            };
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If we don't have a Driver, return false
            if (ItemType != other?.ItemType || other is not Driver otherInternal)
                return false;

            // Compare the internal models
            return _driver.EqualTo(otherInternal._driver);
        }

        #endregion
    }
}
