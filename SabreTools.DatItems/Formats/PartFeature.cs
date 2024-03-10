using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents one part feature object
    /// </summary>
    [JsonObject("part_feature"), XmlRoot("part_feature")]
    public class PartFeature : DatItem
    {
        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => GetFieldValue<string>(Models.Metadata.Feature.NameKey);

        /// <inheritdoc/>
        public override void SetName(string? name) => SetFieldValue(Models.Metadata.Feature.NameKey, name);

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty PartFeature object
        /// </summary>
        public PartFeature()
        {
            _internal = new Models.Metadata.Feature();
            Machine = new Machine();

            SetName(string.Empty);
            ItemType = ItemType.PartFeature;
        }

        /// <summary>
        /// Create a PartFeature object from the internal model
        /// </summary>
        public PartFeature(Models.Metadata.Feature? item)
        {
            _internal = item ?? [];
            Machine = new Machine();

            ItemType = ItemType.PartFeature;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new PartFeature()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.Feature ?? [],
            };
        }

        #endregion
    }
}
