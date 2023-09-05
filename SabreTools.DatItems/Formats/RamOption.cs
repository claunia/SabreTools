using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents which RAM option(s) is associated with a set
    /// </summary>
    [JsonObject("ramoption"), XmlRoot("ramoption")]
    public class RamOption : DatItem
    {
        #region Fields

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name"), XmlElement("name")]
        public string? Name
        {
            get => _internal.ReadString(Models.Metadata.RamOption.NameKey);
            set => _internal[Models.Metadata.RamOption.NameKey] = value;
        }

        /// <summary>
        /// Determine whether the RamOption is default
        /// </summary>
        [JsonProperty("default", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("default")]
        public bool? Default
        {
            get => _internal.ReadBool(Models.Metadata.RamOption.DefaultKey);
            set => _internal[Models.Metadata.RamOption.DefaultKey] = value;
        }

        [JsonIgnore]
        public bool DefaultSpecified { get { return Default != null; } }

        /// <summary>
        /// Determines the content of the RamOption
        /// </summary>
        [JsonProperty("content", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("content")]
        public string? Content
        {
            get => _internal.ReadString(Models.Metadata.RamOption.ContentKey);
            set => _internal[Models.Metadata.RamOption.ContentKey] = value;
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
        /// Create a default, empty RamOption object
        /// </summary>
        public RamOption()
        {
            _internal = new Models.Metadata.RamOption();
            Machine = new Machine();

            Name = string.Empty;
            ItemType = ItemType.RamOption;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new RamOption()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.RamOption ?? new Models.Metadata.RamOption(),
            };
        }

        #endregion
    }
}
