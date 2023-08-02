using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Models.Internal
{
    [JsonObject("configuration"), XmlRoot("configuration")]
    public class Configuration : DatItem
    {
        #region Keys

        /// <remarks>Condition</remarks>
        public const string ConditionKey = "condition";

        /// <remarks>ConfLocation[]</remarks>
        public const string ConfLocationKey = "conflocation";

        /// <remarks>ConfSetting[]</remarks>
        public const string ConfSettingKey = "confsetting";

        /// <remarks>string</remarks>
        public const string MaskKey = "mask";

        /// <remarks>string</remarks>
        public const string NameKey = "name";

        /// <remarks>string</remarks>
        public const string TagKey = "tag";

        #endregion

        public Configuration() => Type = ItemType.Configuration;
    }
}
