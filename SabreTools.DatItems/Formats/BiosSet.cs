using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents which BIOS(es) is associated with a set
    /// </summary>
    [JsonObject("biosset"), XmlRoot("biosset")]
    public class BiosSet : DatItem
    {
        #region Fields

        /// <summary>
        /// Description of the BIOS
        /// </summary>
        [JsonProperty("description", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("description")]
        public string? Description
        {
            get => _internal.ReadString(Models.Metadata.BiosSet.DescriptionKey);
            set => _internal[Models.Metadata.BiosSet.DescriptionKey] = value;
        }

        /// <summary>
        /// Determine whether the BIOS is default
        /// </summary>
        [JsonProperty("default", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("default")]
        public bool? Default
        {
            get => _internal.ReadBool(Models.Metadata.BiosSet.DefaultKey);
            set => _internal[Models.Metadata.BiosSet.DefaultKey] = value;
        }

        [JsonIgnore]
        public bool DefaultSpecified { get { return Default != null; } }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => GetFieldValue<string>(Models.Metadata.BiosSet.NameKey);

        /// <inheritdoc/>
        public override void SetName(string? name) => SetFieldValue(Models.Metadata.BiosSet.NameKey, name);

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty BiosSet object
        /// </summary>
        public BiosSet()
        {
            _internal = new Models.Metadata.BiosSet();
            Machine = new Machine();

            SetName(string.Empty);
            ItemType = ItemType.BiosSet;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new BiosSet()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.BiosSet ?? [],
            };
        }

        #endregion
    }
}
