using System.Collections.Generic;
using System.Linq;
using SabreTools.Library.Filtering;
using Newtonsoft.Json;
using SabreTools.Library.Tools;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Represents one machine display
    /// </summary>
    [JsonObject("display")]
    public class Display : DatItem
    {
        #region Fields

        /// <summary>
        /// Display tag
        /// </summary>
        [JsonProperty("tag", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Tag { get; set; }

        /// <summary>
        /// Display type
        /// </summary>
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string DisplayType { get; set; } // TODO: (raster|vector|lcd|svg|unknown)

        /// <summary>
        /// Display rotation
        /// </summary>
        [JsonProperty("rotate", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Rotate { get; set; } // TODO: (0|90|180|270) Int32?

        /// <summary>
        /// Determines if display is flipped in the X-coordinates
        /// </summary>
        [JsonProperty("flipx", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? FlipX { get; set; }

        /// <summary>
        /// Display width
        /// </summary>
        [JsonProperty("width", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Width { get; set; } // TODO: Int32?

        /// <summary>
        /// Display height
        /// </summary>
        [JsonProperty("height", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Height { get; set; } // TODO: Int32?

        /// <summary>
        /// Refresh rate
        /// </summary>
        [JsonProperty("refresh", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Refresh { get; set; } // TODO: Int32? Float?

        /// <summary>
        /// Pixel clock timer
        /// </summary>
        [JsonProperty("pixclock", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string PixClock { get; set; } // TODO: Int32? Float?

        /// <summary>
        /// Total horizontal lines
        /// </summary>
        [JsonProperty("htotal", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string HTotal { get; set; } // TODO: Int32? Float?

        /// <summary>
        /// Horizontal blank end
        /// </summary>
        [JsonProperty("hbend", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string HBEnd { get; set; } // TODO: Int32? Float?

        /// <summary>
        /// Horizontal blank start
        /// </summary>
        [JsonProperty("hbstart", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string HBStart { get; set; } // TODO: Int32? Float?

        /// <summary>
        /// Total vertical lines
        /// </summary>
        [JsonProperty("vtotal", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string VTotal { get; set; } // TODO: Int32? Float?

        /// <summary>
        /// Vertical blank end
        /// </summary>
        [JsonProperty("vbend", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string VBEnd { get; set; } // TODO: Int32? Float?

        /// <summary>
        /// Vertical blank start
        /// </summary>
        [JsonProperty("vbstart", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string VBStart { get; set; } // TODO: Int32? Float?

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
                DisplayType = mappings[Field.DatItem_DisplayType];

            if (mappings.Keys.Contains(Field.DatItem_Rotate))
                Rotate = mappings[Field.DatItem_Rotate];

            if (mappings.Keys.Contains(Field.DatItem_FlipX))
                FlipX = mappings[Field.DatItem_FlipX].AsYesNo();

            if (mappings.Keys.Contains(Field.DatItem_Width))
                Width = mappings[Field.DatItem_Width];

            if (mappings.Keys.Contains(Field.DatItem_Height))
                Height = mappings[Field.DatItem_Height];

            if (mappings.Keys.Contains(Field.DatItem_Refresh))
                Refresh = mappings[Field.DatItem_Refresh];

            if (mappings.Keys.Contains(Field.DatItem_PixClock))
                PixClock = mappings[Field.DatItem_PixClock];

            if (mappings.Keys.Contains(Field.DatItem_HTotal))
                HTotal = mappings[Field.DatItem_HTotal];

            if (mappings.Keys.Contains(Field.DatItem_HBEnd))
                HBEnd = mappings[Field.DatItem_HBEnd];

            if (mappings.Keys.Contains(Field.DatItem_HBStart))
                HBStart = mappings[Field.DatItem_HBStart];

            if (mappings.Keys.Contains(Field.DatItem_VTotal))
                VTotal = mappings[Field.DatItem_VTotal];

            if (mappings.Keys.Contains(Field.DatItem_VBEnd))
                VBEnd = mappings[Field.DatItem_VBEnd];

            if (mappings.Keys.Contains(Field.DatItem_VBStart))
                VBStart = mappings[Field.DatItem_VBStart];
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

                AltName = this.AltName,
                AltTitle = this.AltTitle,

                Original = this.Original,
                OpenMSXSubType = this.OpenMSXSubType,
                OpenMSXType = this.OpenMSXType,
                Remark = this.Remark,
                Boot = this.Boot,

                LoadFlag = this.LoadFlag,

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
        /// <returns>True if the item passed the filter, false otherwise</returns>
        public override bool PassesFilter(Filter filter)
        {
            // Check common fields first
            if (!base.PassesFilter(filter))
                return false;

            // Filter on tag
            if (filter.DatItem_Tag.MatchesPositiveSet(Tag) == false)
                return false;
            if (filter.DatItem_Tag.MatchesNegativeSet(Tag) == true)
                return false;

            // Filter on display type
            if (filter.DatItem_DisplayType.MatchesPositiveSet(DisplayType) == false)
                return false;
            if (filter.DatItem_DisplayType.MatchesNegativeSet(DisplayType) == true)
                return false;

            // Filter on rotation
            if (filter.DatItem_Rotate.MatchesPositiveSet(Rotate) == false)
                return false;
            if (filter.DatItem_Rotate.MatchesNegativeSet(Rotate) == true)
                return false;

            // Filter on flipx
            if (filter.DatItem_FlipX.MatchesNeutral(null, FlipX) == false)
                return false;

            // Filter on width
            if (filter.DatItem_Width.MatchesPositiveSet(Width) == false)
                return false;
            if (filter.DatItem_Width.MatchesNegativeSet(Width) == true)
                return false;

            // Filter on height
            if (filter.DatItem_Height.MatchesPositiveSet(Height) == false)
                return false;
            if (filter.DatItem_Height.MatchesNegativeSet(Height) == true)
                return false;

            // Filter on refresh
            if (filter.DatItem_Refresh.MatchesPositiveSet(Refresh) == false)
                return false;
            if (filter.DatItem_Refresh.MatchesNegativeSet(Refresh) == true)
                return false;

            // Filter on pixclock
            if (filter.DatItem_PixClock.MatchesPositiveSet(PixClock) == false)
                return false;
            if (filter.DatItem_PixClock.MatchesNegativeSet(PixClock) == true)
                return false;

            // Filter on htotal
            if (filter.DatItem_HTotal.MatchesPositiveSet(HTotal) == false)
                return false;
            if (filter.DatItem_HTotal.MatchesNegativeSet(HTotal) == true)
                return false;

            // Filter on hbend
            if (filter.DatItem_HBEnd.MatchesPositiveSet(HBEnd) == false)
                return false;
            if (filter.DatItem_HBEnd.MatchesNegativeSet(HBEnd) == true)
                return false;

            // Filter on hbstart
            if (filter.DatItem_HBStart.MatchesPositiveSet(HBStart) == false)
                return false;
            if (filter.DatItem_HBStart.MatchesNegativeSet(HBStart) == true)
                return false;

            // Filter on vtotal
            if (filter.DatItem_VTotal.MatchesPositiveSet(VTotal) == false)
                return false;
            if (filter.DatItem_VTotal.MatchesNegativeSet(VTotal) == true)
                return false;

            // Filter on vbend
            if (filter.DatItem_VBEnd.MatchesPositiveSet(VBEnd) == false)
                return false;
            if (filter.DatItem_VBEnd.MatchesNegativeSet(VBEnd) == true)
                return false;

            // Filter on vbstart
            if (filter.DatItem_VBStart.MatchesPositiveSet(VBStart) == false)
                return false;
            if (filter.DatItem_VBStart.MatchesNegativeSet(VBStart) == true)
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
                DisplayType = null;

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
