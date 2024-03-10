using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

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
        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => GetFieldValue<string>(Models.Metadata.SoftwareList.NameKey);

        /// <inheritdoc/>
        public override void SetName(string? name) => SetFieldValue(Models.Metadata.SoftwareList.NameKey, name);

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty SoftwareList object
        /// </summary>
        public SoftwareList()
        {
            _internal = new Models.Metadata.SoftwareList();
            Machine = new Machine();

            SetName(string.Empty);
            ItemType = ItemType.SoftwareList;
        }

        /// <summary>
        /// Create a SoftwareList object from the internal model
        /// </summary>
        public SoftwareList(Models.Metadata.SoftwareList? item)
        {
            _internal = item ?? [];
            Machine = new Machine();

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
    }
}
