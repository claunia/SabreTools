using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

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
        public override string? GetName() => GetFieldValue<string>(Models.Metadata.Archive.NameKey);

        /// <inheritdoc/>
        public override void SetName(string? name) => SetFieldValue(Models.Metadata.Archive.NameKey, name);
        
        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Archive object
        /// </summary>
        public Archive()
        {
            _internal = new Models.Metadata.Archive();

            SetName(string.Empty);
            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.Archive);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        /// <summary>
        /// Create an Archive object from the internal model
        /// </summary>
        public Archive(Models.Metadata.Archive? item)
        {
            _internal = item ?? [];

            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.Archive);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Archive()
            {
                _internal = this._internal?.Clone() as Models.Metadata.Archive ?? [],
            };
        }

        #endregion
    }
}
