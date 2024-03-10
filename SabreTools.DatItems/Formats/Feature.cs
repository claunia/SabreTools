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
            Machine = new Machine();

            ItemType = ItemType.Feature;
        }

        /// <summary>
        /// Create a Feature object from the internal model
        /// </summary>
        public Feature(Models.Metadata.Feature? item)
        {
            _internal = item ?? [];
            Machine = new Machine();

            ItemType = ItemType.Feature;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Feature()
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
