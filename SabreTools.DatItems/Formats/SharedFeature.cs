using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents one shared feature object
    /// </summary>
    [JsonObject("sharedfeat"), XmlRoot("sharedfeat")]
    public class SharedFeature : DatItem
    {
        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => GetFieldValue<string>(Models.Metadata.SharedFeat.NameKey);

        /// <inheritdoc/>
        public override void SetName(string? name) => SetFieldValue(Models.Metadata.SharedFeat.NameKey, name);

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty SharedFeature object
        /// </summary>
        public SharedFeature()
        {
            _internal = new Models.Metadata.SharedFeat();
            Machine = new Machine();

            SetName(string.Empty);
            ItemType = ItemType.SharedFeature;
        }

        /// <summary>
        /// Create a SharedFeature object from the internal model
        /// </summary>
        public SharedFeature(Models.Metadata.SharedFeat? item)
        {
            _internal = item ?? [];
            Machine = new Machine();

            ItemType = ItemType.SharedFeature;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new SharedFeature()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.SharedFeat ?? [],
            };
        }

        #endregion
    }
}
