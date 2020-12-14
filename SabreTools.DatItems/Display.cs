using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using SabreTools.Core;
using SabreTools.Core.Tools;
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

        #region Accessors

        /// <inheritdoc/>
        public override void SetFields(
            Dictionary<DatItemField, string> datItemMappings,
            Dictionary<MachineField, string> machineMappings)
        {
            // Set base fields
            base.SetFields(datItemMappings, machineMappings);

            // Handle Display-specific fields
            if (datItemMappings.Keys.Contains(DatItemField.Tag))
                Tag = datItemMappings[DatItemField.Tag];

            if (datItemMappings.Keys.Contains(DatItemField.DisplayType))
                DisplayType = datItemMappings[DatItemField.DisplayType].AsDisplayType();

            if (datItemMappings.Keys.Contains(DatItemField.Rotate))
                Rotate = Utilities.CleanLong(datItemMappings[DatItemField.Rotate]);

            if (datItemMappings.Keys.Contains(DatItemField.FlipX))
                FlipX = datItemMappings[DatItemField.FlipX].AsYesNo();

            if (datItemMappings.Keys.Contains(DatItemField.Width))
                Width = Utilities.CleanLong(datItemMappings[DatItemField.Width]);

            if (datItemMappings.Keys.Contains(DatItemField.Height))
                Height = Utilities.CleanLong(datItemMappings[DatItemField.Height]);

            if (datItemMappings.Keys.Contains(DatItemField.Refresh))
            {
                if (Double.TryParse(datItemMappings[DatItemField.Refresh], out double refresh))
                    Refresh = refresh;
            }

            if (datItemMappings.Keys.Contains(DatItemField.PixClock))
                PixClock = Utilities.CleanLong(datItemMappings[DatItemField.PixClock]);

            if (datItemMappings.Keys.Contains(DatItemField.HTotal))
                HTotal = Utilities.CleanLong(datItemMappings[DatItemField.HTotal]);

            if (datItemMappings.Keys.Contains(DatItemField.HBEnd))
                HBEnd = Utilities.CleanLong(datItemMappings[DatItemField.HBEnd]);

            if (datItemMappings.Keys.Contains(DatItemField.HBStart))
                HBStart = Utilities.CleanLong(datItemMappings[DatItemField.HBStart]);

            if (datItemMappings.Keys.Contains(DatItemField.VTotal))
                VTotal = Utilities.CleanLong(datItemMappings[DatItemField.VTotal]);

            if (datItemMappings.Keys.Contains(DatItemField.VBEnd))
                VBEnd = Utilities.CleanLong(datItemMappings[DatItemField.VBEnd]);

            if (datItemMappings.Keys.Contains(DatItemField.VBStart))
                VBStart = Utilities.CleanLong(datItemMappings[DatItemField.VBStart]);
        }

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

        #region Sorting and Merging

        /// <inheritdoc/>
        public override void ReplaceFields(
            DatItem item,
            List<DatItemField> datItemFields,
            List<MachineField> machineFields)
        {
            // Replace common fields first
            base.ReplaceFields(item, datItemFields, machineFields);

            // If we don't have a Display to replace from, ignore specific fields
            if (item.ItemType != ItemType.Display)
                return;

            // Cast for easier access
            Display newItem = item as Display;

            // Replace the fields
            if (datItemFields.Contains(DatItemField.Tag))
                Tag = newItem.Tag;

            if (datItemFields.Contains(DatItemField.DisplayType))
                DisplayType = newItem.DisplayType;

            if (datItemFields.Contains(DatItemField.Rotate))
                Rotate = newItem.Rotate;

            if (datItemFields.Contains(DatItemField.FlipX))
                FlipX = newItem.FlipX;

            if (datItemFields.Contains(DatItemField.Width))
                Width = newItem.Width;

            if (datItemFields.Contains(DatItemField.Height))
                Height = newItem.Height;

            if (datItemFields.Contains(DatItemField.Refresh))
                Refresh = newItem.Refresh;

            if (datItemFields.Contains(DatItemField.PixClock))
                PixClock = newItem.PixClock;

            if (datItemFields.Contains(DatItemField.HTotal))
                HTotal = newItem.HTotal;

            if (datItemFields.Contains(DatItemField.HBEnd))
                HBEnd = newItem.HBEnd;

            if (datItemFields.Contains(DatItemField.HBStart))
                HBStart = newItem.HBStart;

            if (datItemFields.Contains(DatItemField.VTotal))
                VTotal = newItem.VTotal;

            if (datItemFields.Contains(DatItemField.VBEnd))
                VBEnd = newItem.VBEnd;

            if (datItemFields.Contains(DatItemField.VBStart))
                VBStart = newItem.VBStart;
        }

        #endregion
    }
}
