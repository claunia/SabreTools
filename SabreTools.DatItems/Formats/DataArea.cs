using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// SoftwareList dataarea information
    /// </summary>
    /// <remarks>One DataArea can contain multiple Rom items</remarks>
    [JsonObject("dataarea"), XmlRoot("dataarea")]
    public sealed class DataArea : DatItem<Models.Metadata.DataArea>
    {
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.DataArea;

        /// <inheritdoc>/>
        protected override string? NameKey => Models.Metadata.DataArea.NameKey;

        #endregion

        #region Constructors

        public DataArea() : base() { }
        public DataArea(Models.Metadata.DataArea item) : base(item) { }

        #endregion
    }
}
