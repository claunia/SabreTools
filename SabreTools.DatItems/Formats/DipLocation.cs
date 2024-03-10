using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents one diplocation
    /// </summary>
    [JsonObject("diplocation"), XmlRoot("diplocation")]
    public class DipLocation : DatItem
    {
        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => GetFieldValue<string>(Models.Metadata.DipLocation.NameKey);

        /// <inheritdoc/>
        public override void SetName(string? name) => SetFieldValue(Models.Metadata.DipLocation.NameKey, name);

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty DipLocation object
        /// </summary>
        public DipLocation()
        {
            _internal = new Models.Metadata.DipLocation();
            Machine = new Machine();

            SetName(string.Empty);
            ItemType = ItemType.DipLocation;
        }

        /// <summary>
        /// Create a DipLocation object from the internal model
        /// </summary>
        public DipLocation(Models.Metadata.DipLocation? item)
        {
            _internal = item ?? [];
            Machine = new Machine();

            ItemType = ItemType.DipLocation;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new DipLocation()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.DipLocation ?? [],
            };
        }

        #endregion
    }
}
