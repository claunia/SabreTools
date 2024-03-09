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
        /// Tag associated with the configuration
        /// </summary>
        [JsonProperty("tag", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("tag")]
        public string? Tag
        {
            get => _internal.ReadString(Models.Metadata.Configuration.TagKey);
            set => _internal[Models.Metadata.Configuration.TagKey] = value;
        }

        /// <summary>
        /// Mask associated with the configuration
        /// </summary>
        [JsonProperty("mask", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("mask")]
        public string? Mask
        {
            get => _internal.ReadString(Models.Metadata.Configuration.MaskKey);
            set => _internal[Models.Metadata.Configuration.MaskKey] = value;
        }

        /// <summary>
        /// Conditions associated with the configuration
        /// </summary>
        [JsonProperty("conditions", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("conditions")]
        public List<Condition>? Conditions
        {
            get => _internal.Read<Condition[]>(Models.Metadata.Configuration.ConditionKey)?.ToList();
            set => _internal[Models.Metadata.Configuration.ConditionKey] = value?.ToArray();
        }

        [JsonIgnore]
        public bool ConditionsSpecified { get { return Conditions != null && Conditions.Count > 0; } }

        /// <summary>
        /// Locations associated with the configuration
        /// </summary>
        [JsonProperty("locations", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("locations")]
        public List<ConfLocation>? Locations
        {
            get => _internal.Read<ConfLocation[]>(Models.Metadata.Configuration.ConfLocationKey)?.ToList();
            set => _internal[Models.Metadata.Configuration.ConfLocationKey] = value?.ToArray();
        }

        [JsonIgnore]
        public bool LocationsSpecified { get { return Locations != null && Locations.Count > 0; } }

        /// <summary>
        /// Settings associated with the configuration
        /// </summary>
        [JsonProperty("settings", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("settings")]
        public List<ConfSetting>? Settings
        {
            get => _internal.Read<List<ConfSetting>>(Models.Metadata.Configuration.ConfSettingKey);
            set => _internal[Models.Metadata.Configuration.ConfSettingKey] = value;
        }

        [JsonIgnore]
        public bool SettingsSpecified { get { return Settings != null && Settings.Count > 0; } }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => GetFieldValue<string>(Models.Metadata.Configuration.NameKey);

        /// <inheritdoc/>
        public override void SetName(string? name) => SetFieldValue(Models.Metadata.Configuration.NameKey, name);

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Configuration object
        /// </summary>
        public Configuration()
        {
            _internal = new Models.Metadata.Configuration();
            Machine = new Machine();

            SetName(string.Empty);
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

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.Configuration ?? [],
            };
        }

        #endregion
    }
}
