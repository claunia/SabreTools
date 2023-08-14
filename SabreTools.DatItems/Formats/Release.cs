using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents release information about a set
    /// </summary>
    [JsonObject("release"), XmlRoot("release")]
    public class Release : DatItem
    {
        #region Fields

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name"), XmlElement("name")]
        public string? Name
        {
            get => _release.ReadString(Models.Internal.Release.NameKey);
            set => _release[Models.Internal.Release.NameKey] = value;
        }

        /// <summary>
        /// Release region(s)
        /// </summary>
        [JsonProperty("region", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("region")]
        public string? Region
        {
            get => _release.ReadString(Models.Internal.Release.RegionKey);
            set => _release[Models.Internal.Release.RegionKey] = value;
        }

        /// <summary>
        /// Release language(s)
        /// </summary>
        [JsonProperty("language", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("language")]
        public string? Language
        {
            get => _release.ReadString(Models.Internal.Release.LanguageKey);
            set => _release[Models.Internal.Release.LanguageKey] = value;
        }

        /// <summary>
        /// Date of release
        /// </summary>
        [JsonProperty("date", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("date")]
        public string? Date
        {
            get => _release.ReadString(Models.Internal.Release.DateKey);
            set => _release[Models.Internal.Release.DateKey] = value;
        }

        /// <summary>
        /// Default release, if applicable
        /// </summary>
        [JsonProperty("default", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("default")]
        public bool? Default
        {
            get => _release.ReadBool(Models.Internal.Release.DefaultKey);
            set => _release[Models.Internal.Release.DefaultKey] = value;
        }

        [JsonIgnore]
        public bool DefaultSpecified { get { return Default != null; } }

        /// <summary>
        /// Internal Release model
        /// </summary>
        [JsonIgnore]
        private Models.Internal.Release _release = new();

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Release object
        /// </summary>
        public Release()
        {
            Name = string.Empty;
            ItemType = ItemType.Release;
            Region = string.Empty;
            Language = string.Empty;
            Date = string.Empty;
            Default = null;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Release()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine?.Clone() as Machine,
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _release = this._release?.Clone() as Models.Internal.Release ?? new Models.Internal.Release(),
            };
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If we don't have a Release, return false
            if (ItemType != other?.ItemType || other is not Release otherInternal)
                return false;

            // Compare the internal models
            return _release.EqualTo(otherInternal._release);
        }

        #endregion
    }
}
