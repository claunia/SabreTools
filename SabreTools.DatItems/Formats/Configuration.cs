using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents which Configuration(s) is associated with a set
    /// </summary>
    [JsonObject("configuration"), XmlRoot("configuration")]
    public class Configuration : DatItem
    {
        #region Fields

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name"), XmlElement("name")]
        public string? Name
        {
            get => _configuration.ReadString(Models.Internal.Configuration.NameKey);
            set => _configuration[Models.Internal.Configuration.NameKey] = value;
        }

        /// <summary>
        /// Tag associated with the configuration
        /// </summary>
        [JsonProperty("tag", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("tag")]
        public string? Tag
        {
            get => _configuration.ReadString(Models.Internal.Configuration.TagKey);
            set => _configuration[Models.Internal.Configuration.TagKey] = value;
        }

        /// <summary>
        /// Mask associated with the configuration
        /// </summary>
        [JsonProperty("mask", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("mask")]
        public string? Mask
        {
            get => _configuration.ReadString(Models.Internal.Configuration.MaskKey);
            set => _configuration[Models.Internal.Configuration.MaskKey] = value;
        }

        /// <summary>
        /// Conditions associated with the configuration
        /// </summary>
        [JsonProperty("conditions", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("conditions")]
        public List<Condition>? Conditions
        {
            get => _configuration.Read<Condition[]>(Models.Internal.Configuration.ConditionKey)?.ToList();
            set => _configuration[Models.Internal.Configuration.ConditionKey] = value?.ToArray();
        }

        [JsonIgnore]
        public bool ConditionsSpecified { get { return Conditions != null && Conditions.Count > 0; } }

        /// <summary>
        /// Locations associated with the configuration
        /// </summary>
        [JsonProperty("locations", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("locations")]
        public List<ConfLocation>? Locations
        {
            get => _configuration.Read<ConfLocation[]>(Models.Internal.Configuration.ConfLocationKey)?.ToList();
            set => _configuration[Models.Internal.Configuration.ConfLocationKey] = value?.ToArray();
        }

        [JsonIgnore]
        public bool LocationsSpecified { get { return Locations != null && Locations.Count > 0; } }

        /// <summary>
        /// Settings associated with the configuration
        /// </summary>
        [JsonProperty("settings", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("settings")]
        public List<ConfSetting>? Settings
        {
            get => _configuration.Read<List<ConfSetting>>(Models.Internal.Configuration.ConfSettingKey);
            set => _configuration[Models.Internal.Configuration.ConfSettingKey] = value;
        }

        [JsonIgnore]
        public bool SettingsSpecified { get { return Settings != null && Settings.Count > 0; } }

        /// <summary>
        /// Internal Configuration model
        /// </summary>
        [JsonIgnore]
        private Models.Internal.Configuration _configuration = new();

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Configuration object
        /// </summary>
        public Configuration()
        {
            Name = string.Empty;
            ItemType = ItemType.Configuration;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Configuration()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine?.Clone() as Machine,
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _configuration = this._configuration?.Clone() as Models.Internal.Configuration ?? new Models.Internal.Configuration(),
            };
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If we don't have a Configuration, return false
            if (ItemType != other?.ItemType || other is not Configuration otherInternal)
                return false;

            // Compare the internal models
            return _configuration.EqualTo(otherInternal._configuration);
        }

        #endregion
    }
}
