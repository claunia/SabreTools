using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;
using SabreTools.Filter;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents generic archive files to be included in a set
    /// </summary>
    [JsonObject("archive"), XmlRoot("archive")]
    public class Archive : DatItem
    {
        #region Fields

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name"), XmlElement("name")]
        public string? Name
        {
            get => _internal.ReadString(Models.Metadata.Archive.NameKey);
            set => _internal[Models.Metadata.Archive.NameKey] = value;
        }

        /// <summary>
        /// Archive ID number
        /// </summary>
        /// <remarks>TODO: No-Intro database export only</remarks>
        [JsonProperty("number"), XmlElement("number")]
        public string? Number { get; set; }

        /// <summary>
        /// Clone value
        /// </summary>
        /// <remarks>TODO: No-Intro database export only</remarks>
        [JsonProperty("clone"), XmlElement("clone")]
        public string? CloneValue { get; set; }

        /// <summary>
        /// Regional parent value
        /// </summary>
        /// <remarks>TODO: No-Intro database export only</remarks>
        [JsonProperty("regparent"), XmlElement("regparent")]
        public string? RegParent { get; set; }

        /// <summary>
        /// Region value
        /// </summary>
        /// <remarks>TODO: No-Intro database export only</remarks>
        [JsonProperty("region"), XmlElement("region")]
        public string? Region { get; set; }

        /// <summary>
        /// Languages value
        /// </summary>
        /// <remarks>TODO: No-Intro database export only</remarks>
        [JsonProperty("languages"), XmlElement("languages")]
        public string? Languages { get; set; }

        /// <summary>
        /// Development status value
        /// </summary>
        /// <remarks>TODO: No-Intro database export only</remarks>
        [JsonProperty("devstatus"), XmlElement("devstatus")]
        public string? DevStatus { get; set; }

        /// <summary>
        /// Physical value
        /// </summary>
        /// <remarks>TODO: No-Intro database export only</remarks>
        /// <remarks>TODO: Is this numeric or a flag?</remarks>
        [JsonProperty("physical"), XmlElement("physical")]
        public string? Physical { get; set; }

        /// <summary>
        /// Complete value
        /// </summary>
        /// <remarks>TODO: No-Intro database export only</remarks>
        /// <remarks>TODO: Is this numeric or a flag?</remarks>
        [JsonProperty("complete"), XmlElement("complete")]
        public string? Complete { get; set; }

        /// <summary>
        /// Categories value
        /// </summary>
        /// <remarks>TODO: No-Intro database export only</remarks>
        [JsonProperty("categories"), XmlElement("categories")]
        public string? Categories { get; set; }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;
        
        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Archive object
        /// </summary>
        public Archive()
        {
            _internal = new Models.Metadata.Archive();
            Machine = new Machine();

            Name = string.Empty;
            ItemType = ItemType.Archive;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Archive()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.Archive ?? [],
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
                _ => null,
            };

            // Set the field and return
            return FieldManipulator.SetField(_internal, fieldName, value);
        }

        #endregion
    }
}
