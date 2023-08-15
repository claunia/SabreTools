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
        /// Name of the item
        /// </summary>
        [JsonProperty("name"), XmlElement("name")]
        public string? Name
        {
            get => _internal.ReadString(Models.Internal.BiosSet.NameKey);
            set => _internal[Models.Internal.BiosSet.NameKey] = value;
        }

        /// <summary>
        /// Description of the BIOS
        /// </summary>
        [JsonProperty("description", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("description")]
        public string? Description
        {
            get => _internal.ReadString(Models.Internal.BiosSet.DescriptionKey);
            set => _internal[Models.Internal.BiosSet.DescriptionKey] = value;
        }

        /// <summary>
        /// Determine whether the BIOS is default
        /// </summary>
        [JsonProperty("default", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("default")]
        public bool? Default
        {
            get => _internal.ReadBool(Models.Internal.BiosSet.DefaultKey);
            set => _internal[Models.Internal.BiosSet.DefaultKey] = value;
        }

        [JsonIgnore]
        public bool DefaultSpecified { get { return Default != null; } }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty BiosSet object
        /// </summary>
        public BiosSet()
        {
            _internal = new Models.Internal.BiosSet();
            Machine = new Machine();

            Name = string.Empty;
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

                _internal = this._internal?.Clone() as Models.Internal.BiosSet ?? new Models.Internal.BiosSet(),
            };
        }

        #endregion
    }
}
