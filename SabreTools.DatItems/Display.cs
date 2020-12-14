using System.Xml.Serialization;

using SabreTools.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SabreTools.DatItems
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
        [JsonProperty("tag", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("tag")]
        public string Tag { get; set; }

        /// <summary>
        /// Display type
        /// </summary>
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlElement("type")]
        public DisplayType DisplayType { get; set; }

        [JsonIgnore]
        public bool DisplayTypeSpecified { get { return DisplayType != DisplayType.NULL; } }

        /// <summary>
        /// Display rotation
        /// </summary>
        [JsonProperty("rotate", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("rotate")]
        public long? Rotate { get; set; }

        [JsonIgnore]
        public bool RotateSpecified { get { return Rotate != null; } }

        /// <summary>
        /// Determines if display is flipped in the X-coordinates
        /// </summary>
        [JsonProperty("flipx", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("flipx")]
        public bool? FlipX { get; set; }

        [JsonIgnore]
        public bool FlipXSpecified { get { return FlipX != null; } }

        /// <summary>
        /// Display width
        /// </summary>
        [JsonProperty("width", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("width")]
        public long? Width { get; set; }

        [JsonIgnore]
        public bool WidthSpecified { get { return Width != null; } }

        /// <summary>
        /// Display height
        /// </summary>
        [JsonProperty("height", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("height")]
        public long? Height { get; set; }

        [JsonIgnore]
        public bool HeightSpecified { get { return Height != null; } }

        /// <summary>
        /// Refresh rate
        /// </summary>
        [JsonProperty("refresh", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("refresh")]
        public double? Refresh { get; set; }

        [JsonIgnore]
        public bool RefreshSpecified { get { return Refresh != null; } }

        /// <summary>
        /// Pixel clock timer
        /// </summary>
        [JsonProperty("pixclock", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("pixclock")]
        public long? PixClock { get; set; }

        [JsonIgnore]
        public bool PixClockSpecified { get { return PixClock != null; } }

        /// <summary>
        /// Total horizontal lines
        /// </summary>
        [JsonProperty("htotal", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("htotal")]
        public long? HTotal { get; set; }

        [JsonIgnore]
        public bool HTotalSpecified { get { return HTotal != null; } }

        /// <summary>
        /// Horizontal blank end
        /// </summary>
        [JsonProperty("hbend", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("hbend")]
        public long? HBEnd { get; set; }

        [JsonIgnore]
        public bool HBEndSpecified { get { return HBEnd != null; } }

        /// <summary>
        /// Horizontal blank start
        /// </summary>
        [JsonProperty("hbstart", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("hbstart")]
        public long? HBStart { get; set; }

        [JsonIgnore]
        public bool HBStartSpecified { get { return HBStart != null; } }

        /// <summary>
        /// Total vertical lines
        /// </summary>
        [JsonProperty("vtotal", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("vtotal")]
        public long? VTotal { get; set; }

        [JsonIgnore]
        public bool VTotalSpecified { get { return VTotal != null; } }

        /// <summary>
        /// Vertical blank end
        /// </summary>
        [JsonProperty("vbend", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("vbend")]
        public long? VBEnd { get; set; }

        [JsonIgnore]
        public bool VBEndSpecified { get { return VBEnd != null; } }

        /// <summary>
        /// Vertical blank start
        /// </summary>
        [JsonProperty("vbstart", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("vbstart")]
        public long? VBStart { get; set; }

        [JsonIgnore]
        public bool VBStartSpecified { get { return VBStart != null; } }

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

        public override object Clone()
        {
            return new Display()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Tag = this.Tag,
                DisplayType = this.DisplayType,
                Rotate = this.Rotate,
                FlipX = this.FlipX,
                Width = this.Width,
                Height = this.Height,
                Refresh = this.Refresh,
                PixClock = this.PixClock,
                HTotal = this.HTotal,
                HBEnd = this.HBEnd,
                HBStart = this.HBStart,
                VTotal = this.VTotal,
                VBEnd = this.VBEnd,
                VBStart = this.VBStart,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a Display, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Display
            Display newOther = other as Display;

            // If the Display information matches
            return (Tag == newOther.Tag
                && DisplayType == newOther.DisplayType
                && Rotate == newOther.Rotate
                && FlipX == newOther.FlipX
                && Width == newOther.Width
                && Height == newOther.Height
                && Refresh == newOther.Refresh
                && PixClock == newOther.PixClock
                && HTotal == newOther.HTotal
                && HBEnd == newOther.HBEnd
                && HBStart == newOther.HBStart
                && VTotal == newOther.VTotal
                && VBEnd == newOther.VBEnd
                && VBStart == newOther.VBStart);
        }

        #endregion
    }
}
