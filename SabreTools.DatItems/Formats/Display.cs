using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SabreTools.Core;
using SabreTools.Core.Tools;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents one machine display
    /// </summary>
    [JsonObject("display"), XmlRoot("display")]
    public class Display : DatItem
    {
        #region Fields

        /// <summary>
        /// Display tag
        /// </summary>
        [JsonProperty("tag", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("tag")]
        public string? Tag
        {
            get => _display.ReadString(Models.Internal.Display.TagKey);
            set => _display[Models.Internal.Display.TagKey] = value;
        }

        /// <summary>
        /// Display type
        /// </summary>
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public DisplayType DisplayType
        {
            get => _display.ReadString(Models.Internal.Display.DisplayTypeKey).AsDisplayType();
            set => _display[Models.Internal.Display.DisplayTypeKey] = value.FromDisplayType();
        }

        [JsonIgnore]
        public bool DisplayTypeSpecified { get { return DisplayType != DisplayType.NULL; } }

        /// <summary>
        /// Display rotation
        /// </summary>
        [JsonProperty("rotate", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("rotate")]
        public long? Rotate
        {
            get => _display.ReadLong(Models.Internal.Display.RotateKey);
            set => _display[Models.Internal.Display.RotateKey] = value;
        }

        [JsonIgnore]
        public bool RotateSpecified { get { return Rotate != null; } }

        /// <summary>
        /// Determines if display is flipped in the X-coordinates
        /// </summary>
        [JsonProperty("flipx", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("flipx")]
        public bool? FlipX
        {
            get => _display.ReadBool(Models.Internal.Display.FlipXKey);
            set => _display[Models.Internal.Display.FlipXKey] = value;
        }

        [JsonIgnore]
        public bool FlipXSpecified { get { return FlipX != null; } }

        /// <summary>
        /// Display width
        /// </summary>
        [JsonProperty("width", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("width")]
        public long? Width
        {
            get => _display.ReadLong(Models.Internal.Display.WidthKey);
            set => _display[Models.Internal.Display.WidthKey] = value;
        }

        [JsonIgnore]
        public bool WidthSpecified { get { return Width != null; } }

        /// <summary>
        /// Display height
        /// </summary>
        [JsonProperty("height", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("height")]
        public long? Height
        {
            get => _display.ReadLong(Models.Internal.Display.HeightKey);
            set => _display[Models.Internal.Display.HeightKey] = value;
        }

        [JsonIgnore]
        public bool HeightSpecified { get { return Height != null; } }

        /// <summary>
        /// Refresh rate
        /// </summary>
        [JsonProperty("refresh", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("refresh")]
        public double? Refresh
        {
            get => _display.ReadDouble(Models.Internal.Display.RefreshKey);
            set => _display[Models.Internal.Display.RefreshKey] = value;
        }

        [JsonIgnore]
        public bool RefreshSpecified { get { return Refresh != null; } }

        /// <summary>
        /// Pixel clock timer
        /// </summary>
        [JsonProperty("pixclock", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("pixclock")]
        public long? PixClock
        {
            get => _display.ReadLong(Models.Internal.Display.PixClockKey);
            set => _display[Models.Internal.Display.PixClockKey] = value;
        }

        [JsonIgnore]
        public bool PixClockSpecified { get { return PixClock != null; } }

        /// <summary>
        /// Total horizontal lines
        /// </summary>
        [JsonProperty("htotal", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("htotal")]
        public long? HTotal
        {
            get => _display.ReadLong(Models.Internal.Display.HTotalKey);
            set => _display[Models.Internal.Display.HTotalKey] = value;
        }

        [JsonIgnore]
        public bool HTotalSpecified { get { return HTotal != null; } }

        /// <summary>
        /// Horizontal blank end
        /// </summary>
        [JsonProperty("hbend", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("hbend")]
        public long? HBEnd
        {
            get => _display.ReadLong(Models.Internal.Display.HBEndKey);
            set => _display[Models.Internal.Display.HBEndKey] = value;
        }

        [JsonIgnore]
        public bool HBEndSpecified { get { return HBEnd != null; } }

        /// <summary>
        /// Horizontal blank start
        /// </summary>
        [JsonProperty("hbstart", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("hbstart")]
        public long? HBStart
        {
            get => _display.ReadLong(Models.Internal.Display.HBStartKey);
            set => _display[Models.Internal.Display.HBStartKey] = value;
        }

        [JsonIgnore]
        public bool HBStartSpecified { get { return HBStart != null; } }

        /// <summary>
        /// Total vertical lines
        /// </summary>
        [JsonProperty("vtotal", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("vtotal")]
        public long? VTotal
        {
            get => _display.ReadLong(Models.Internal.Display.VTotalKey);
            set => _display[Models.Internal.Display.VTotalKey] = value;
        }

        [JsonIgnore]
        public bool VTotalSpecified { get { return VTotal != null; } }

        /// <summary>
        /// Vertical blank end
        /// </summary>
        [JsonProperty("vbend", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("vbend")]
        public long? VBEnd
        {
            get => _display.ReadLong(Models.Internal.Display.VBEndKey);
            set => _display[Models.Internal.Display.VBEndKey] = value;
        }

        [JsonIgnore]
        public bool VBEndSpecified { get { return VBEnd != null; } }

        /// <summary>
        /// Vertical blank start
        /// </summary>
        [JsonProperty("vbstart", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("vbstart")]
        public long? VBStart
        {
            get => _display.ReadLong(Models.Internal.Display.VBStartKey);
            set => _display[Models.Internal.Display.VBStartKey] = value;
        }

        [JsonIgnore]
        public bool VBStartSpecified { get { return VBStart != null; } }

        /// <summary>
        /// Internal Display model
        /// </summary>
        [JsonIgnore]
        private Models.Internal.Display _display = new();

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Display object
        /// </summary>
        public Display()
        {
            ItemType = ItemType.Display;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Display()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine?.Clone() as Machine,
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _display = this._display?.Clone() as Models.Internal.Display ?? new Models.Internal.Display(),
            };
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If we don't have a Display, return false
            if (ItemType != other?.ItemType || other is not Display otherInternal)
                return false;

            // Compare the internal models
            return _display.EqualTo(otherInternal._display);
        }

        #endregion
    }
}
