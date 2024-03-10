using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents a single port on a machine
    /// </summary>
    [JsonObject("port"), XmlRoot("port")]
    public class Port : DatItem
    {
        #region Fields

        [JsonIgnore]
        public bool AnalogsSpecified
        {
            get
            {
                var analogs = GetFieldValue<Analog[]?>(Models.Metadata.Port.AnalogKey);
                return analogs != null && analogs.Length > 0;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Port object
        /// </summary>
        public Port()
        {
            _internal = new Models.Metadata.Port();
            Machine = new Machine();

            ItemType = ItemType.Port;
        }

        /// <summary>
        /// Create a Port object from the internal model
        /// </summary>
        public Port(Models.Metadata.Port? item)
        {
            _internal = item ?? [];
            Machine = new Machine();

            ItemType = ItemType.Port;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Port()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.Port ?? [],
            };
        }

        #endregion
    }
}
