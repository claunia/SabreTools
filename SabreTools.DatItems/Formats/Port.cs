using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents a single port on a machine
    /// </summary>
    [JsonObject("port"), XmlRoot("port")]
    public sealed class Port : DatItem<Models.Metadata.Port>
    {
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.Port;

        /// <inheritdoc>/>
        protected override string? NameKey => null;

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

        public Port() : base() { }
        public Port(Models.Metadata.Port item) : base(item) { }

        #endregion
    }
}
