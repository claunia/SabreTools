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
        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => GetFieldValue<string>(Models.Metadata.Info.NameKey);

        /// <inheritdoc/>
        public override void SetName(string? name) => SetFieldValue(Models.Metadata.Info.NameKey, name);

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Info object
        /// </summary>
        public Info()
        {
            _internal = new Models.Metadata.Info();
            Machine = new Machine();

            SetName(string.Empty);
            ItemType = ItemType.Info;
        }

        /// <summary>
        /// Create an Info object from the internal model
        /// </summary>
        public Info(Models.Metadata.Info? item)
        {
            _internal = item ?? [];
            Machine = new Machine();

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

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.Info ?? [],
            };
        }

        #endregion
    }
}
