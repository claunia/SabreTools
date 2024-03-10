using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents a condition on a machine or other item
    /// </summary>
    [JsonObject("condition"), XmlRoot("condition")]
    public class Condition : DatItem
    {
        #region Constructors

        /// <summary>
        /// Create a default, empty Condition object
        /// </summary>
        public Condition()
        {
            _internal = new Models.Metadata.Condition();
            Machine = new Machine();

            ItemType = ItemType.Condition;
        }

        /// <summary>
        /// Create a Condition object from the internal model
        /// </summary>
        public Condition(Models.Metadata.Condition? item)
        {
            _internal = item ?? [];
            Machine = new Machine();

            ItemType = ItemType.Condition;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Condition()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.Condition ?? [],
            };
        }

        #endregion
    }
}
