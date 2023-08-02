using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Models.Internal
{
    [JsonObject("port"), XmlRoot("port")]
    public class Port : DatItem
    {
        #region Keys

        /// <remarks>Analog[]</remarks>
        public const string AnalogKey = "analog";

        /// <remarks>string</remarks>
        public const string TagKey = "tag";

        #endregion

        public Port() => Type = ItemType.Port;
    }
}
