using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Models.Internal
{
    [JsonObject("dump"), XmlRoot("dump")]
    public class Dump : DatItem
    {
        #region Keys

        /// <remarks>Rom</remarks>
        public const string MegaRomKey = "megarom";

        /// <remarks>Original</remarks>
        public const string OriginalKey = "original";

        /// <remarks>Rom</remarks>
        public const string RomKey = "rom";

        /// <remarks>Rom</remarks>
        public const string SCCPlusCartKey = "sccpluscart";

        #endregion

        public Dump() => Type = ItemType.Dump;
    }
}
