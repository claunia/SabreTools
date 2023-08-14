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
        /// Name of the item
        /// </summary>
        [JsonProperty("name"), XmlElement("name")]
        public string? Name
        {
            get => _instance.ReadString(Models.Internal.Instance.NameKey);
            set => _instance[Models.Internal.Instance.NameKey] = value;
        }

        /// <summary>
        /// Short name for the instance
        /// </summary>
        [JsonProperty("briefname", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("briefname")]
        public string? BriefName
        {
            get => _instance.ReadString(Models.Internal.Instance.BriefNameKey);
            set => _instance[Models.Internal.Instance.BriefNameKey] = value;
        }

        /// <summary>
        /// Internal Instance model
        /// </summary>
        [JsonIgnore]
        private Models.Internal.Instance _instance = new();

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Instance object
        /// </summary>
        public Instance()
        {
            Name = string.Empty;
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

                Machine = this.Machine?.Clone() as Machine,
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _instance = this._instance?.Clone() as Models.Internal.Instance ?? new Models.Internal.Instance(),
            };
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If we don't have a Instance, return false
            if (ItemType != other?.ItemType || other is not Instance otherInternal)
                return false;

            // Compare the internal models
            return _instance.EqualTo(otherInternal._instance);
        }

        #endregion
    }
}
