using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;
using SabreTools.Filter;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents a single port on a machine
    /// </summary>
    [JsonObject("port"), XmlRoot("port")]
    public class Port : DatItem
    {
        #region Fields

        /// <summary>
        /// Tag for the port
        /// </summary>
        [JsonProperty("tag", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("tag")]
        public string? Tag
        {
            get => _internal.ReadString(Models.Metadata.Port.TagKey);
            set => _internal[Models.Metadata.Port.TagKey] = value;
        }

        /// <summary>
        /// List of analogs on the port
        /// </summary>
        [JsonProperty("analogs", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("analogs")]
        public List<Analog>? Analogs
        {
            get => _internal.Read<Analog[]>(Models.Metadata.Port.AnalogKey)?.ToList();
            set => _internal[Models.Metadata.Port.AnalogKey] = value?.ToArray();
        }

        [JsonIgnore]
        public bool AnalogsSpecified { get { return Analogs != null && Analogs.Count > 0; } }

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
    
        #region Manipulation

        /// <inheritdoc/>
        public override bool SetField(DatItemField datItemField, string value)
        {
            // Get the correct internal field name
            string? fieldName = datItemField switch
            {
                DatItemField.Tag => Models.Metadata.Port.TagKey,
                _ => null,
            };

            // Set the field and return
            return FieldManipulator.SetField(_internal, fieldName, value);
        }

        #endregion
    }
}
