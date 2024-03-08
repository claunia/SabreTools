using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents a matchable extension
    /// </summary>
    [JsonObject("extension"), XmlRoot("extension")]
    public class Extension : DatItem
    {
        #region Fields

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name"), XmlElement("name")]
        public string? Name
        {
            get => _internal.ReadString(Models.Metadata.Extension.NameKey);
            set => _internal[Models.Metadata.Extension.NameKey] = value;
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
        /// Create a default, empty Extension object
        /// </summary>
        public Extension()
        {
            _internal = new Models.Metadata.Extension();
            Machine = new Machine();

            Name = string.Empty;
            ItemType = ItemType.Extension;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Extension()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.Extension ?? [],
            };
        }

        #endregion
    }
}
