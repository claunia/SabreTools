using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents special information about a machine
    /// </summary>
    [JsonObject("info"), XmlRoot("info")]
    public class Info : DatItem
    {
        #region Fields

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name"), XmlElement("name")]
        public string? Name
        {
            get => _info.ReadString(Models.Internal.Info.NameKey);
            set => _info[Models.Internal.Info.NameKey] = value;
        }

        /// <summary>
        /// Information value
        /// </summary>
        [JsonProperty("value"), XmlElement("value")]
        public string? Value
        {
            get => _info.ReadString(Models.Internal.Info.ValueKey);
            set => _info[Models.Internal.Info.ValueKey] = value;
        }

        /// <summary>
        /// Internal Info model
        /// </summary>
        [JsonIgnore]
        private Models.Internal.Info _info = new();

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Info object
        /// </summary>
        public Info()
        {
            Name = string.Empty;
            ItemType = ItemType.Info;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Info()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine?.Clone() as Machine,
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _info = this._info?.Clone() as Models.Internal.Info ?? new Models.Internal.Info(),
            };
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If we don't have a Info, return false
            if (ItemType != other?.ItemType || other is not Info otherInternal)
                return false;

            // Compare the internal models
            return _info.EqualTo(otherInternal._info);
        }

        #endregion
    }
}
