using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using SabreTools.Core;
using SabreTools.Library.Filtering;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SabreTools.Library.DatItems
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

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="mappings">Mappings dictionary</param>
        public override void SetFields(Dictionary<Field, string> mappings)
        {
            // Set base fields
            base.SetFields(mappings);

            // Handle Display-specific fields
            if (mappings.Keys.Contains(Field.DatItem_Tag))
                Tag = mappings[Field.DatItem_Tag];

            if (mappings.Keys.Contains(Field.DatItem_DisplayType))
                DisplayType = mappings[Field.DatItem_DisplayType].AsDisplayType();

            if (mappings.Keys.Contains(Field.DatItem_Rotate))
                Rotate = Sanitizer.CleanLong(mappings[Field.DatItem_Rotate]);

            if (mappings.Keys.Contains(Field.DatItem_FlipX))
                FlipX = mappings[Field.DatItem_FlipX].AsYesNo();

            if (mappings.Keys.Contains(Field.DatItem_Width))
                Width = Sanitizer.CleanLong(mappings[Field.DatItem_Width]);

            if (mappings.Keys.Contains(Field.DatItem_Height))
                Height = Sanitizer.CleanLong(mappings[Field.DatItem_Height]);

            if (mappings.Keys.Contains(Field.DatItem_Refresh))
            {
                if (Double.TryParse(mappings[Field.DatItem_Refresh], out double refresh))
                    Refresh = refresh;
            }

            if (mappings.Keys.Contains(Field.DatItem_PixClock))
                PixClock = Sanitizer.CleanLong(mappings[Field.DatItem_PixClock]);

            if (mappings.Keys.Contains(Field.DatItem_HTotal))
                HTotal = Sanitizer.CleanLong(mappings[Field.DatItem_HTotal]);

            if (mappings.Keys.Contains(Field.DatItem_HBEnd))
                HBEnd = Sanitizer.CleanLong(mappings[Field.DatItem_HBEnd]);

            if (mappings.Keys.Contains(Field.DatItem_HBStart))
                HBStart = Sanitizer.CleanLong(mappings[Field.DatItem_HBStart]);

            if (mappings.Keys.Contains(Field.DatItem_VTotal))
                VTotal = Sanitizer.CleanLong(mappings[Field.DatItem_VTotal]);

            if (mappings.Keys.Contains(Field.DatItem_VBEnd))
                VBEnd = Sanitizer.CleanLong(mappings[Field.DatItem_VBEnd]);

            if (mappings.Keys.Contains(Field.DatItem_VBStart))
                VBStart = Sanitizer.CleanLong(mappings[Field.DatItem_VBStart]);
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

        #region Filtering

        /// <summary>
        /// Check to see if a DatItem passes the filter
        /// </summary>
        /// <param name="filter">Filter to check against</param>
        /// <param name="sub">True if this is a subitem, false otherwise</param>
        /// <returns>True if the item passed the filter, false otherwise</returns>
        public override bool PassesFilter(Filter filter, bool sub = false)
        {
            // Check common fields first
            if (!base.PassesFilter(filter, sub))
                return false;

            // Filter on tag
            if (!filter.PassStringFilter(filter.DatItem_Tag, Tag))
                return false;

            // Filter on display type
            if (filter.DatItem_DisplayType.MatchesPositive(DisplayType.NULL, DisplayType) == false)
                return false;
            if (filter.DatItem_DisplayType.MatchesNegative(DisplayType.NULL, DisplayType) == true)
                return false;

            // Filter on rotation
            if (!filter.PassLongFilter(filter.DatItem_Rotate, Rotate))
                return false;

            // Filter on flipx
            if (!filter.PassBoolFilter(filter.DatItem_FlipX, FlipX))
                return false;

            // Filter on width
            if (!filter.PassLongFilter(filter.DatItem_Width, Width))
                return false;

            // Filter on height
            if (!filter.PassLongFilter(filter.DatItem_Height, Height))
                return false;

            // Filter on refresh
            if (!filter.PassDoubleFilter(filter.DatItem_Refresh, Refresh))
                return false;

            // Filter on pixclock
            if (!filter.PassLongFilter(filter.DatItem_PixClock, PixClock))
                return false;

            // Filter on htotal
            if (!filter.PassLongFilter(filter.DatItem_HTotal, HTotal))
                return false;

            // Filter on hbend
            if (!filter.PassLongFilter(filter.DatItem_HBEnd, HBEnd))
                return false;

            // Filter on hbstart
            if (!filter.PassLongFilter(filter.DatItem_HBStart, HBStart))
                return false;

            // Filter on vtotal
            if (!filter.PassLongFilter(filter.DatItem_VTotal, VTotal))
                return false;

            // Filter on vbend
            if (!filter.PassLongFilter(filter.DatItem_VBEnd, VBEnd))
                return false;

            // Filter on vbstart
            if (!filter.PassLongFilter(filter.DatItem_VBStart, VBStart))
                return false;

            return true;
        }

        /// <summary>
        /// Remove fields from the DatItem
        /// </summary>
        /// <param name="fields">List of Fields to remove</param>
        public override void RemoveFields(List<Field> fields)
        {
            // Remove common fields first
            base.RemoveFields(fields);

            // Remove the fields
            if (fields.Contains(Field.DatItem_Tag))
                Tag = null;

            if (fields.Contains(Field.DatItem_DisplayType))
                DisplayType = DisplayType.NULL;

            if (fields.Contains(Field.DatItem_Rotate))
                Rotate = null;

            if (fields.Contains(Field.DatItem_FlipX))
                FlipX = null;

            if (fields.Contains(Field.DatItem_Width))
                Width = null;

            if (fields.Contains(Field.DatItem_Height))
                Height = null;

            if (fields.Contains(Field.DatItem_Refresh))
                Refresh = null;

            if (fields.Contains(Field.DatItem_PixClock))
                PixClock = null;

            if (fields.Contains(Field.DatItem_HTotal))
                HTotal = null;

            if (fields.Contains(Field.DatItem_HBEnd))
                HBEnd = null;

            if (fields.Contains(Field.DatItem_HBStart))
                HBStart = null;

            if (fields.Contains(Field.DatItem_VTotal))
                VTotal = null;

            if (fields.Contains(Field.DatItem_VBEnd))
                VBEnd = null;

            if (fields.Contains(Field.DatItem_VBStart))
                VBStart = null;
        }

        #endregion

        #region Sorting and Merging

        /// <summary>
        /// Replace fields from another item
        /// </summary>
        /// <param name="item">DatItem to pull new information from</param>
        /// <param name="fields">List of Fields representing what should be updated</param>
        public override void ReplaceFields(DatItem item, List<Field> fields)
        {
            // Replace common fields first
            base.ReplaceFields(item, fields);

            // If we don't have a Display to replace from, ignore specific fields
            if (item.ItemType != ItemType.Display)
                return;

            // Cast for easier access
            Display newItem = item as Display;

            // Replace the fields
            if (fields.Contains(Field.DatItem_Tag))
                Tag = newItem.Tag;

            if (fields.Contains(Field.DatItem_DisplayType))
                DisplayType = newItem.DisplayType;

            if (fields.Contains(Field.DatItem_Rotate))
                Rotate = newItem.Rotate;

            if (fields.Contains(Field.DatItem_FlipX))
                FlipX = newItem.FlipX;

            if (fields.Contains(Field.DatItem_Width))
                Width = newItem.Width;

            if (fields.Contains(Field.DatItem_Height))
                Height = newItem.Height;

            if (fields.Contains(Field.DatItem_Refresh))
                Refresh = newItem.Refresh;

            if (fields.Contains(Field.DatItem_PixClock))
                PixClock = newItem.PixClock;

            if (fields.Contains(Field.DatItem_HTotal))
                HTotal = newItem.HTotal;

            if (fields.Contains(Field.DatItem_HBEnd))
                HBEnd = newItem.HBEnd;

            if (fields.Contains(Field.DatItem_HBStart))
                HBStart = newItem.HBStart;

            if (fields.Contains(Field.DatItem_VTotal))
                VTotal = newItem.VTotal;

            if (fields.Contains(Field.DatItem_VBEnd))
                VBEnd = newItem.VBEnd;

            if (fields.Contains(Field.DatItem_VBStart))
                VBStart = newItem.VBStart;
        }

        #endregion
    }
}
