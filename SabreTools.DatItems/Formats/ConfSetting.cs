using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents one ListXML confsetting
    /// </summary>
    [JsonObject("confsetting"), XmlRoot("confsetting")]
    public class ConfSetting : DatItem
    {
        #region Fields

        /// <summary>
        /// Setting name
        /// </summary>
        [JsonProperty("name"), XmlElement("name")]
        public string? Name
        {
            get => _internal.ReadString(Models.Metadata.ConfSetting.NameKey);
            set => _internal[Models.Metadata.ConfSetting.NameKey] = value;
        }

        /// <summary>
        /// Setting value
        /// </summary>
        [JsonProperty("value", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("value")]
        public string? Value
        {
            get => _internal.ReadString(Models.Metadata.ConfSetting.ValueKey);
            set => _internal[Models.Metadata.ConfSetting.ValueKey] = value;
        }

        /// <summary>
        /// Determines if the setting is default or not
        /// </summary>
        [JsonProperty("default", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("default")]
        public bool? Default
        {
            get => _internal.ReadBool(Models.Metadata.ConfSetting.DefaultKey);
            set => _internal[Models.Metadata.ConfSetting.DefaultKey] = value;
        }

        [JsonIgnore]
        public bool DefaultSpecified { get { return Default != null; } }

        /// <summary>
        /// List of conditions on the setting
        /// </summary>
        [JsonProperty("conditions", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("conditions")]
        public List<Condition>? Conditions
        {
            get => _internal.Read<Condition[]>(Models.Metadata.ConfSetting.ConditionKey)?.ToList();
            set => _internal[Models.Metadata.ConfSetting.ConditionKey] = value?.ToArray();
        }

        [JsonIgnore]
        public bool ConditionsSpecified { get { return Conditions != null && Conditions.Count > 0; } }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty ConfSetting object
        /// </summary>
        public ConfSetting()
        {
            _internal = new Models.Metadata.ConfSetting();
            Machine = new Machine();

            Name = string.Empty;
            ItemType = ItemType.ConfSetting;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new ConfSetting()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.ConfSetting ?? new Models.Metadata.ConfSetting(),
            };
        }

        #endregion
    }
}
