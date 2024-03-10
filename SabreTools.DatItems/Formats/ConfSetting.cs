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

        [JsonIgnore]
        public bool ConditionsSpecified
        {
            get
            {
                var conditions = GetFieldValue<Condition[]?>(Models.Metadata.ConfSetting.ConditionKey);
                return conditions != null && conditions.Length > 0;
            }
        }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => GetFieldValue<string>(Models.Metadata.ConfSetting.NameKey);

        /// <inheritdoc/>
        public override void SetName(string? name) => SetFieldValue(Models.Metadata.ConfSetting.NameKey, name);

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty ConfSetting object
        /// </summary>
        public ConfSetting()
        {
            _internal = new Models.Metadata.ConfSetting();
            Machine = new Machine();

            SetName(string.Empty);
            ItemType = ItemType.ConfSetting;
        }

        /// <summary>
        /// Create a ConfSetting object from the internal model
        /// </summary>
        public ConfSetting(Models.Metadata.ConfSetting? item)
        {
            _internal = item ?? [];
            Machine = new Machine();

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

                _internal = this._internal?.Clone() as Models.Metadata.ConfSetting ?? [],
            };
        }

        #endregion
    }
}
