using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents control for an input
    /// </summary>
    [JsonObject("control"), XmlRoot("control")]
    public class Control : DatItem
    {
        #region Constructors

        /// <summary>
        /// Create a default, empty Control object
        /// </summary>
        public Control()
        {
            _internal = new Models.Metadata.Control();
            Machine = new Machine();

            ItemType = ItemType.Control;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Control()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.Control ?? [],
            };
        }

        #endregion
    }
}
