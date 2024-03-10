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

            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.Port);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        /// <summary>
        /// Create a Port object from the internal model
        /// </summary>
        public Port(Models.Metadata.Port item)
        {
            _internal = item;

            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.Port);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Port()
            {
                _internal = this._internal?.Clone() as Models.Metadata.Port ?? [],
            };
        }

        #endregion
    }
}
