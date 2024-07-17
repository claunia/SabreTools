using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core.Tools;

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
            SetFieldValue<long?>(Models.Metadata.Video.AspectXKey, NumberHelper.ConvertToInt64(item.ReadString(Models.Metadata.Video.AspectXKey)));
            SetFieldValue<long?>(Models.Metadata.Video.AspectYKey, NumberHelper.ConvertToInt64(item.ReadString(Models.Metadata.Video.AspectYKey)));
            SetFieldValue<string?>(Models.Metadata.Display.DisplayTypeKey, item.ReadString(Models.Metadata.Video.ScreenKey).AsEnumValue<DisplayType>().AsStringValue());
            SetFieldValue<long?>(Models.Metadata.Display.HeightKey, NumberHelper.ConvertToInt64(item.ReadString(Models.Metadata.Video.HeightKey)));
            SetFieldValue<double?>(Models.Metadata.Display.RefreshKey, NumberHelper.ConvertToDouble(item.ReadString(Models.Metadata.Video.RefreshKey)));
            SetFieldValue<long?>(Models.Metadata.Display.WidthKey, NumberHelper.ConvertToInt64(item.ReadString(Models.Metadata.Video.WidthKey)));

            switch (item.ReadString(Models.Metadata.Video.OrientationKey))
            {
                case "horizontal":
                    SetFieldValue<long?>(Models.Metadata.Display.RotateKey, 0);
                    break;
                case "vertical":
                    SetFieldValue<long?>(Models.Metadata.Display.RotateKey, 90);
                    break;
            }
        }

        #endregion
    }
}
