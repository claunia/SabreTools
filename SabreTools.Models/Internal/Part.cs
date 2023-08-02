using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Models.Internal
{
    [JsonObject("part"), XmlRoot("part")]
    public class Part : DatItem
    {
        #region Keys

        /// <remarks>DataArea[]</remarks>
        public const string DataAreaKey = "dataarea";

        /// <remarks>DiskArea[]</remarks>
        public const string DiskAreaKey = "diskarea";

        /// <remarks>DipSwitch[]</remarks>
        public const string DipSwitchKey = "dipswitch";

        /// <remarks>Feature[]</remarks>
        public const string FeatureKey = "feature";

        /// <remarks>string</remarks>
        public const string InterfaceKey = "interface";

        /// <remarks>string</remarks>
        public const string NameKey = "name";

        #endregion

        public Part() => Type = ItemType.Part;
    }
}
