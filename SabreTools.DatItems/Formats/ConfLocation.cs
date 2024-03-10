using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents one conflocation
    /// </summary>
    [JsonObject("conflocation"), XmlRoot("conflocation")]
    public class ConfLocation : DatItem
    {
        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => GetFieldValue<string>(Models.Metadata.ConfLocation.NameKey);

        /// <inheritdoc/>
        public override void SetName(string? name) => SetFieldValue(Models.Metadata.ConfLocation.NameKey, name);

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty ConfLocation object
        /// </summary>
        public ConfLocation()
        {
            _internal = new Models.Metadata.ConfLocation();
            Machine = new Machine();

            SetName(string.Empty);
            ItemType = ItemType.ConfLocation;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new ConfLocation()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.ConfLocation ?? [],
            };
        }

        #endregion
    }
}
