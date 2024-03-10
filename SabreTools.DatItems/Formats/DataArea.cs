using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// SoftwareList dataarea information
    /// </summary>
    /// <remarks>One DataArea can contain multiple Rom items</remarks>
    [JsonObject("dataarea"), XmlRoot("dataarea")]
    public class DataArea : DatItem
    {
        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => GetFieldValue<string>(Models.Metadata.DataArea.NameKey);

        /// <inheritdoc/>
        public override void SetName(string? name) => SetFieldValue(Models.Metadata.DataArea.NameKey, name);

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty DataArea object
        /// </summary>
        public DataArea()
        {
            _internal = new Models.Metadata.DataArea();
            Machine = new Machine();

            SetName(string.Empty);
            ItemType = ItemType.DataArea;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new DataArea()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.DataArea ?? [],
            };
        }

        #endregion
    }
}
