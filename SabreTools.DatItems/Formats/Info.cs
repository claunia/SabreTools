using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents special information about a machine
    /// </summary>
    [JsonObject("info"), XmlRoot("info")]
    public sealed class Info : DatItem<Models.Metadata.Info>
    {
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.Info;

        /// <inheritdoc>/>
        protected override string? NameKey => Models.Metadata.Instance.NameKey;

        #endregion

        #region Constructors

        public Info() : base() { }
        public Info(Models.Metadata.Info item) : base(item) { }

        #endregion
    }
}
