using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents which RAM option(s) is associated with a set
    /// </summary>
    [JsonObject("ramoption"), XmlRoot("ramoption")]
    public class RamOption : DatItem
    {
        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => GetFieldValue<string>(Models.Metadata.RamOption.NameKey);

        /// <inheritdoc/>
        public override void SetName(string? name) => SetFieldValue(Models.Metadata.RamOption.NameKey, name);

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty RamOption object
        /// </summary>
        public RamOption()
        {
            _internal = new Models.Metadata.RamOption();
            Machine = new Machine();

            SetName(string.Empty);
            ItemType = ItemType.RamOption;
        }

        /// <summary>
        /// Create a RamOption object from the internal model
        /// </summary>
        public RamOption(Models.Metadata.RamOption? item)
        {
            _internal = item ?? [];
            Machine = new Machine();

            ItemType = ItemType.RamOption;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new RamOption()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.RamOption ?? [],
            };
        }

        #endregion
    }
}
