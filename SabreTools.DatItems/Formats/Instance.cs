using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents a single instance of another item
    /// </summary>
    [JsonObject("instance"), XmlRoot("instance")]
    public sealed class Instance : DatItem<Models.Metadata.Instance>
    {
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.Instance;

        #endregion

        #region Constructors

        public Instance() : base() { }

        public Instance(Models.Metadata.Instance item) : base(item) { }

        #endregion
    }
}
