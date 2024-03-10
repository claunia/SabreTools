using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents which BIOS(es) is associated with a set
    /// </summary>
    [JsonObject("biosset"), XmlRoot("biosset")]
    public class BiosSet : DatItem
    {
        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => GetFieldValue<string>(Models.Metadata.BiosSet.NameKey);

        /// <inheritdoc/>
        public override void SetName(string? name) => SetFieldValue(Models.Metadata.BiosSet.NameKey, name);

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty BiosSet object
        /// </summary>
        public BiosSet()
        {
            _internal = new Models.Metadata.BiosSet();

            SetName(string.Empty);
            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.BiosSet);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        /// <summary>
        /// Create a BiosSet object from the internal model
        /// </summary>
        public BiosSet(Models.Metadata.BiosSet? item)
        {
            _internal = item ?? [];

            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.BiosSet);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new BiosSet()
            {
                _internal = this._internal?.Clone() as Models.Metadata.BiosSet ?? [],
            };
        }

        #endregion
    }
}
