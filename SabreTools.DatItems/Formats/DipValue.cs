using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents one ListXML dipvalue
    /// </summary>
    [JsonObject("dipvalue"), XmlRoot("dipvalue")]
    public class DipValue : DatItem
    {
        #region Fields

        [JsonIgnore]
        public bool ConditionsSpecified
        {
            get
            {
                var conditions = GetFieldValue<Condition[]?>(Models.Metadata.DipValue.ConditionKey);
                return conditions != null && conditions.Length > 0;
            }
        }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => GetFieldValue<string>(Models.Metadata.DipValue.NameKey);

        /// <inheritdoc/>
        public override void SetName(string? name) => SetFieldValue(Models.Metadata.DipValue.NameKey, name);

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty DipValue object
        /// </summary>
        public DipValue()
        {
            _internal = new Models.Metadata.DipValue();
            Machine = new Machine();

            SetName(string.Empty);
            ItemType = ItemType.DipValue;
        }

        /// <summary>
        /// Create a DipValue object from the internal model
        /// </summary>
        public DipValue(Models.Metadata.DipValue? item)
        {
            _internal = item ?? [];
            Machine = new Machine();

            ItemType = ItemType.DipValue;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new DipValue()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.DipValue ?? [],
            };
        }

        #endregion
    }
}
