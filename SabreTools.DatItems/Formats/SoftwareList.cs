using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.Filter;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents which SoftwareList(s) is associated with a set
    /// </summary>
    /// <remarks>
    /// TODO: Add new fields to documentation
    /// </remarks>
    [JsonObject("softwarelist"), XmlRoot("softwarelist")]
    public class SoftwareList : DatItem
    {
        #region Fields

        /// <summary>
        /// Tag for the software list
        /// </summary>
        [JsonProperty("tag", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("tag")]
        public string? Tag
        {
            get => _internal.ReadString(Models.Metadata.SoftwareList.TagKey);
            set => _internal[Models.Metadata.SoftwareList.TagKey] = value;
        }

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name")]
        [XmlElement("name")]
        public string? Name
        {
            get => _internal.ReadString(Models.Metadata.SoftwareList.NameKey);
            set => _internal[Models.Metadata.SoftwareList.NameKey] = value;
        }

        /// <summary>
        /// Status of the softare list according to the machine
        /// </summary>
        [JsonProperty("status", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlElement("status")]
        public SoftwareListStatus Status
        {
            get => _internal.ReadString(Models.Metadata.SoftwareList.StatusKey).AsEnumValue<SoftwareListStatus>();
            set => _internal[Models.Metadata.SoftwareList.StatusKey] = value.AsStringValue<SoftwareListStatus>();
        }

        [JsonIgnore]
        public bool StatusSpecified { get { return Status != SoftwareListStatus.None; } }

        /// <summary>
        /// Filter to apply to the software list
        /// </summary>
        [JsonProperty("filter", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("filter")]
        public string? Filter
        {
            get => _internal.ReadString(Models.Metadata.SoftwareList.FilterKey);
            set => _internal[Models.Metadata.SoftwareList.FilterKey] = value;
        }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty SoftwareList object
        /// </summary>
        public SoftwareList()
        {
            _internal = new Models.Metadata.SoftwareList();
            Machine = new Machine();

            Name = string.Empty;
            ItemType = ItemType.SoftwareList;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new SoftwareList()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.SoftwareList ?? [],
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
                DatItemField.Filter => Models.Metadata.SoftwareList.FilterKey,
                DatItemField.SoftwareListStatus => Models.Metadata.SoftwareList.StatusKey,
                DatItemField.Tag => Models.Metadata.SoftwareList.TagKey,
                _ => null,
            };

            // Set the field and return
            return FieldManipulator.SetField(_internal, fieldName, value);
        }

        #endregion
    }
}
