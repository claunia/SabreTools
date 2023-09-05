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
            get => _internal.ReadString(Models.Metadata.Driver.StatusKey).AsSupportStatus();
            set => _internal[Models.Metadata.Driver.StatusKey] = value.FromSupportStatus();
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
            get => _internal.ReadString(Models.Metadata.Driver.EmulationKey).AsSupportStatus();
            set => _internal[Models.Metadata.Driver.EmulationKey] = value.FromSupportStatus();
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
            get => _internal.ReadString(Models.Metadata.Driver.CocktailKey).AsSupportStatus();
            set => _internal[Models.Metadata.Driver.CocktailKey] = value.FromSupportStatus();
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
            get => _internal.ReadString(Models.Metadata.Driver.SaveStateKey).AsSupported();
            set => _internal[Models.Metadata.Driver.SaveStateKey] = value.FromSupported(verbose: true);
        }

        [JsonIgnore]
        public bool SaveStateSpecified { get { return SaveState != Supported.NULL; } }

        /// <summary>
        /// Requires artwork
        /// </summary>
        [JsonProperty("requiresartwork", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("requiresartwork")]
        public bool? RequiresArtwork
        {
            get => _internal.ReadBool(Models.Metadata.Driver.RequiresArtworkKey);
            set => _internal[Models.Metadata.Driver.RequiresArtworkKey] = value;
        }

        [JsonIgnore]
        public bool RequiresArtworkSpecified { get { return RequiresArtwork != null; } }

        /// <summary>
        /// Unofficial
        /// </summary>
        [JsonProperty("unofficial", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("unofficial")]
        public bool? Unofficial
        {
            get => _internal.ReadBool(Models.Metadata.Driver.UnofficialKey);
            set => _internal[Models.Metadata.Driver.UnofficialKey] = value;
        }

        [JsonIgnore]
        public bool UnofficialSpecified { get { return Unofficial != null; } }

        /// <summary>
        /// No sound hardware
        /// </summary>
        [JsonProperty("nosoundhardware", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("nosoundhardware")]
        public bool? NoSoundHardware
        {
            get => _internal.ReadBool(Models.Metadata.Driver.NoSoundHardwareKey);
            set => _internal[Models.Metadata.Driver.NoSoundHardwareKey] = value;
        }

        [JsonIgnore]
        public bool NoSoundHardwareSpecified { get { return NoSoundHardware != null; } }

        /// <summary>
        /// Incomplete
        /// </summary>
        [JsonProperty("incomplete", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("incomplete")]
        public bool? Incomplete
        {
            get => _internal.ReadBool(Models.Metadata.Driver.IncompleteKey);
            set => _internal[Models.Metadata.Driver.IncompleteKey] = value;
        }

        [JsonIgnore]
        public bool IncompleteSpecified { get { return Incomplete != null; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Driver object
        /// </summary>
        public Driver()
        {
            _internal = new Models.Metadata.Driver();
            Machine = new Machine();

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

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.Driver ?? new Models.Metadata.Driver(),
            };
        }

        #endregion
    }
}
