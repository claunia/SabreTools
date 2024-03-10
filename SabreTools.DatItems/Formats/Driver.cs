using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents the a driver of the machine
    /// </summary>
    /// <remarks>
    /// TODO: Add new fields to documentation
    /// </remarks>
    [JsonObject("driver"), XmlRoot("driver")]
    public class Driver : DatItem
    {
        #region Constructors

        /// <summary>
        /// Create a default, empty Driver object
        /// </summary>
        public Driver()
        {
            _internal = new Models.Metadata.Driver();
            Machine = new Machine();

            ItemType = ItemType.Driver;
        }

        /// <summary>
        /// Create a Driver object from the internal model
        /// </summary>
        public Driver(Models.Metadata.Driver? item)
        {
            _internal = item ?? [];
            Machine = new Machine();

            ItemType = ItemType.Driver;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Driver()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.Driver ?? [],
            };
        }

        #endregion
    }
}
