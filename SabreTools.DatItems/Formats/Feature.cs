using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents the a feature of the machine
    /// </summary>
    [JsonObject("feature"), XmlRoot("feature")]
    public class Feature : DatItem
    {
        #region Constructors

        /// <summary>
        /// Create a default, empty Feature object
        /// </summary>
        public Feature()
        {
            _internal = new Models.Metadata.Feature();

            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.Feature);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        /// <summary>
        /// Create a Feature object from the internal model
        /// </summary>
        public Feature(Models.Metadata.Feature item)
        {
            _internal = item;

            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.Feature);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Feature()
            {
                _internal = this._internal?.Clone() as Models.Metadata.Feature ?? [],
            };
        }

        #endregion
    }
}
