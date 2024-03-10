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

            SetName(string.Empty);
            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.PartFeature);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        /// <summary>
        /// Create a PartFeature object from the internal model
        /// </summary>
        public PartFeature(Models.Metadata.Feature item)
        {
            _internal = item;

            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.PartFeature);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new PartFeature()
            {
                _internal = this._internal?.Clone() as Models.Metadata.Feature ?? [],
            };
        }

        #endregion
    }
}
