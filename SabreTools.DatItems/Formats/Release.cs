using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents release information about a set
    /// </summary>
    [JsonObject("release"), XmlRoot("release")]
    public class Release : DatItem
    {
        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => GetFieldValue<string>(Models.Metadata.Release.NameKey);

        /// <inheritdoc/>
        public override void SetName(string? name) => SetFieldValue(Models.Metadata.Release.NameKey, name);

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Release object
        /// </summary>
        public Release()
        {
            _internal = new Models.Metadata.Release();
            Machine = new Machine();

            SetName(string.Empty);
            ItemType = ItemType.Release;
        }

        /// <summary>
        /// Create a Release object from the internal model
        /// </summary>
        public Release(Models.Metadata.Release? item)
        {
            _internal = item ?? [];
            Machine = new Machine();

            ItemType = ItemType.Release;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Release()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.Release ?? [],
            };
        }

        #endregion
    }
}
