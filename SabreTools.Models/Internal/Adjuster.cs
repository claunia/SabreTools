using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Models.Internal
{
    [JsonObject("adjuster"), XmlRoot("adjuster")]
    public class Adjuster : DatItem
    {
        #region Keys

        // <remarks>Condition</remarks>
        public const string ConditionKey = "condition";

        /// <remarks>bool</remarks>
        public const string DefaultKey = "default";

        /// <remarks>string</remarks>
        public const string NameKey = "name";

        #endregion

        public Adjuster() => Type = ItemType.Adjuster;
    }
}
