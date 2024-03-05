using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.Filter;

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
            get => _internal.ReadString(Models.Metadata.Display.TagKey);
            set => _internal[Models.Metadata.Display.TagKey] = value;
        }

        /// <summary>
        /// Display type
        /// </summary>
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public DisplayType DisplayType
        {
            get => _internal.ReadString(Models.Metadata.Display.DisplayTypeKey).AsDisplayType();
            set => _internal[Models.Metadata.Display.DisplayTypeKey] = value.FromDisplayType();
        }

        [JsonIgnore]
        public bool DisplayTypeSpecified { get { return DisplayType != DisplayType.NULL; } }

        /// <summary>
        /// Display rotation
        /// </summary>
        [JsonProperty("rotate", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("rotate")]
        public long? Rotate
        {
            get => _internal.ReadLong(Models.Metadata.Display.RotateKey);
            set => _internal[Models.Metadata.Display.RotateKey] = value;
        }

        [JsonIgnore]
        public bool RotateSpecified { get { return Rotate != null; } }

        /// <summary>
        /// Determines if display is flipped in the X-coordinates
        /// </summary>
        [JsonProperty("flipx", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("flipx")]
        public bool? FlipX
        {
            get => _internal.ReadBool(Models.Metadata.Display.FlipXKey);
            set => _internal[Models.Metadata.Display.FlipXKey] = value;
        }

        [JsonIgnore]
        public bool FlipXSpecified { get { return FlipX != null; } }

        /// <summary>
        /// Display width
        /// </summary>
        [JsonProperty("width", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("width")]
        public long? Width
        {
            get => _internal.ReadLong(Models.Metadata.Display.WidthKey);
            set => _internal[Models.Metadata.Display.WidthKey] = value;
        }

        [JsonIgnore]
        public bool WidthSpecified { get { return Width != null; } }

        /// <summary>
        /// Display height
        /// </summary>
        [JsonProperty("height", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("height")]
        public long? Height
        {
            get => _internal.ReadLong(Models.Metadata.Display.HeightKey);
            set => _internal[Models.Metadata.Display.HeightKey] = value;
        }

        [JsonIgnore]
        public bool HeightSpecified { get { return Height != null; } }

        /// <summary>
        /// Refresh rate
        /// </summary>
        [JsonProperty("refresh", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("refresh")]
        public double? Refresh
        {
            get => _internal.ReadDouble(Models.Metadata.Display.RefreshKey);
            set => _internal[Models.Metadata.Display.RefreshKey] = value;
        }

        [JsonIgnore]
        public bool RefreshSpecified { get { return Refresh != null; } }

        /// <summary>
        /// Pixel clock timer
        /// </summary>
        [JsonProperty("pixclock", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("pixclock")]
        public long? PixClock
        {
            get => _internal.ReadLong(Models.Metadata.Display.PixClockKey);
            set => _internal[Models.Metadata.Display.PixClockKey] = value;
        }

        [JsonIgnore]
        public bool PixClockSpecified { get { return PixClock != null; } }

        /// <summary>
        /// Total horizontal lines
        /// </summary>
        [JsonProperty("htotal", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("htotal")]
        public long? HTotal
        {
            get => _internal.ReadLong(Models.Metadata.Display.HTotalKey);
            set => _internal[Models.Metadata.Display.HTotalKey] = value;
        }

        [JsonIgnore]
        public bool HTotalSpecified { get { return HTotal != null; } }

        /// <summary>
        /// Horizontal blank end
        /// </summary>
        [JsonProperty("hbend", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("hbend")]
        public long? HBEnd
        {
            get => _internal.ReadLong(Models.Metadata.Display.HBEndKey);
            set => _internal[Models.Metadata.Display.HBEndKey] = value;
        }

        [JsonIgnore]
        public bool HBEndSpecified { get { return HBEnd != null; } }

        /// <summary>
        /// Horizontal blank start
        /// </summary>
        [JsonProperty("hbstart", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("hbstart")]
        public long? HBStart
        {
            get => _internal.ReadLong(Models.Metadata.Display.HBStartKey);
            set => _internal[Models.Metadata.Display.HBStartKey] = value;
        }

        [JsonIgnore]
        public bool HBStartSpecified { get { return HBStart != null; } }

        /// <summary>
        /// Total vertical lines
        /// </summary>
        [JsonProperty("vtotal", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("vtotal")]
        public long? VTotal
        {
            get => _internal.ReadLong(Models.Metadata.Display.VTotalKey);
            set => _internal[Models.Metadata.Display.VTotalKey] = value;
        }

        [JsonIgnore]
        public bool VTotalSpecified { get { return VTotal != null; } }

        /// <summary>
        /// Vertical blank end
        /// </summary>
        [JsonProperty("vbend", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("vbend")]
        public long? VBEnd
        {
            get => _internal.ReadLong(Models.Metadata.Display.VBEndKey);
            set => _internal[Models.Metadata.Display.VBEndKey] = value;
        }

        [JsonIgnore]
        public bool VBEndSpecified { get { return VBEnd != null; } }

        /// <summary>
        /// Vertical blank start
        /// </summary>
        [JsonProperty("vbstart", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("vbstart")]
        public long? VBStart
        {
            get => _internal.ReadLong(Models.Metadata.Display.VBStartKey);
            set => _internal[Models.Metadata.Display.VBStartKey] = value;
        }

        [JsonIgnore]
        public bool VBStartSpecified { get { return VBStart != null; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Display object
        /// </summary>
        public Display()
        {
            _internal = new Models.Metadata.Display();
            Machine = new Machine();

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

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.Display ?? [],
            };
        }

        #endregion
    
        #region Manipulation

        /// <inheritdoc/>
        public override bool RemoveField(DatItemField datItemField)
        {
            // Get the correct internal field name
            string? fieldName = datItemField switch
            {
                DatItemField.DisplayType => Models.Metadata.Display.DisplayTypeKey,
                DatItemField.FlipX => Models.Metadata.Display.FlipXKey,
                DatItemField.HBEnd => Models.Metadata.Display.HBEndKey,
                DatItemField.HBStart => Models.Metadata.Display.HBStartKey,
                DatItemField.Height => Models.Metadata.Display.HeightKey,
                DatItemField.HTotal => Models.Metadata.Display.HTotalKey,
                DatItemField.PixClock => Models.Metadata.Display.PixClockKey,
                DatItemField.Refresh => Models.Metadata.Display.RefreshKey,
                DatItemField.Rotate => Models.Metadata.Display.RotateKey,
                DatItemField.Tag => Models.Metadata.Display.TagKey,
                DatItemField.VBEnd => Models.Metadata.Display.VBEndKey,
                DatItemField.VBStart => Models.Metadata.Display.VBStartKey,
                DatItemField.VTotal => Models.Metadata.Display.VTotalKey,
                DatItemField.Width => Models.Metadata.Display.WidthKey,
                _ => null,
            };

            // Remove the field and return
            return FieldManipulator.RemoveField(_internal, fieldName);
        }

        /// <inheritdoc/>
        public override bool SetField(DatItemField datItemField, string value)
        {
            // Get the correct internal field name
            string? fieldName = datItemField switch
            {
                DatItemField.DisplayType => Models.Metadata.Display.DisplayTypeKey,
                DatItemField.FlipX => Models.Metadata.Display.FlipXKey,
                DatItemField.HBEnd => Models.Metadata.Display.HBEndKey,
                DatItemField.HBStart => Models.Metadata.Display.HBStartKey,
                DatItemField.Height => Models.Metadata.Display.HeightKey,
                DatItemField.HTotal => Models.Metadata.Display.HTotalKey,
                DatItemField.PixClock => Models.Metadata.Display.PixClockKey,
                DatItemField.Refresh => Models.Metadata.Display.RefreshKey,
                DatItemField.Rotate => Models.Metadata.Display.RotateKey,
                DatItemField.Tag => Models.Metadata.Display.TagKey,
                DatItemField.VBEnd => Models.Metadata.Display.VBEndKey,
                DatItemField.VBStart => Models.Metadata.Display.VBStartKey,
                DatItemField.VTotal => Models.Metadata.Display.VTotalKey,
                DatItemField.Width => Models.Metadata.Display.WidthKey,
                _ => null,
            };

            // Set the field and return
            return FieldManipulator.SetField(_internal, fieldName, value);
        }

        #endregion
    }
}
