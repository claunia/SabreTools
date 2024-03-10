using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents which DIP Switch(es) is associated with a set
    /// </summary>
    [JsonObject("dipswitch"), XmlRoot("dipswitch")]
    public class DipSwitch : DatItem
    {
        #region Fields

        [JsonIgnore]
        public bool ConditionsSpecified
        {
            get
            {
                var conditions = GetFieldValue<Condition[]?>(Models.Metadata.DipSwitch.ConditionKey);
                return conditions != null && conditions.Length > 0;
            }
        }

        [JsonIgnore]
        public bool LocationsSpecified
        {
            get
            {
                var locations = GetFieldValue<DipLocation[]?>(Models.Metadata.DipSwitch.DipLocationKey);
                return locations != null && locations.Length > 0;
            }
        }

        [JsonIgnore]
        public bool ValuesSpecified
        {
            get
            {
                var values = GetFieldValue<DipValue[]?>(Models.Metadata.DipSwitch.DipValueKey);
                return values != null && values.Length > 0;
            }
        }

        [JsonIgnore]
        public bool PartSpecified
        {
            get
            {
                var part = GetFieldValue<Part?>("PART");
                return part != null
                    && (!string.IsNullOrEmpty(part.GetName())
                        || !string.IsNullOrEmpty(part.GetFieldValue<string?>(Models.Metadata.Part.InterfaceKey)));
            }
        }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => GetFieldValue<string>(Models.Metadata.DipSwitch.NameKey);

        /// <inheritdoc/>
        public override void SetName(string? name) => SetFieldValue(Models.Metadata.DipSwitch.NameKey, name);

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty DipSwitch object
        /// </summary>
        public DipSwitch()
        {
            _internal = new Models.Metadata.DipSwitch();
            Machine = new Machine();

            SetName(string.Empty);
            ItemType = ItemType.DipSwitch;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new DipSwitch()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.DipSwitch ?? [],
            };
        }

        #endregion
    }
}
