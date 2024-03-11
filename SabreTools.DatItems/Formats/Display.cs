using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents one machine display
    /// </summary>
    [JsonObject("display"), XmlRoot("display")]
    public sealed class Display : DatItem<Models.Metadata.Display>
    {
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.Display;

        /// <inheritdoc>/>
        protected override string? NameKey => null;

        #endregion

        #region Constructors

        public Display() : base() { }
        public Display(Models.Metadata.Display item) : base(item) { }

        public Display(Models.Metadata.Video item) : base()
        {
            // TODO: Determine what transformation is needed here
            _internal = item;
        }

        #endregion
    }
}
