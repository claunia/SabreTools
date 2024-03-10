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

        [JsonIgnore]
        public bool ConditionsSpecified
        {
            get
            {
                var conditions = GetFieldValue<Condition[]?>(Models.Metadata.Configuration.ConditionKey);
                return conditions != null && conditions.Length > 0;
            }
        }

        [JsonIgnore]
        public bool LocationsSpecified
        {
            get
            {
                var locations = GetFieldValue<ConfLocation[]?>(Models.Metadata.Configuration.ConfLocationKey);
                return locations != null && locations.Length > 0;
            }
        }

        [JsonIgnore]
        public bool SettingsSpecified
        {
            get
            {
                var settings = GetFieldValue<ConfSetting[]?>(Models.Metadata.Configuration.ConfSettingKey);
                return settings != null && settings.Length > 0;
            }
        }

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

            SetName(string.Empty);
            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.Configuration);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        /// <summary>
        /// Create a Configuration object from the internal model
        /// </summary>
        public Configuration(Models.Metadata.Configuration item)
        {
            _internal = item;

            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.Configuration);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Configuration()
            {
                _internal = this._internal?.Clone() as Models.Metadata.Configuration ?? [],
            };
        }

        #endregion
    }
}
