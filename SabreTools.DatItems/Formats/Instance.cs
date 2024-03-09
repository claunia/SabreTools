using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents a single instance of another item
    /// </summary>
    [JsonObject("instance"), XmlRoot("instance")]
    public class Instance : DatItem
    {
        #region Fields

        /// <summary>
        /// Short name for the instance
        /// </summary>
        [JsonProperty("briefname", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("briefname")]
        public string? BriefName
        {
            get => _internal.ReadString(Models.Metadata.Instance.BriefNameKey);
            set => _internal[Models.Metadata.Instance.BriefNameKey] = value;
        }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => GetFieldValue<string>(Models.Metadata.Instance.NameKey);

        /// <inheritdoc/>
        public override void SetName(string? name) => SetFieldValue(Models.Metadata.Instance.NameKey, name);

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Instance object
        /// </summary>
        public Instance()
        {
            _internal = new Models.Metadata.Instance();
            Machine = new Machine();

            SetName(string.Empty);
            ItemType = ItemType.Instance;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Instance()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.Instance ?? [],
            };
        }

        #endregion
    }
}
