using System.Collections.Generic;
using System.Linq;

using SabreTools.Library.Filtering;
using Newtonsoft.Json;
using SabreTools.Library.Tools;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Represents release information about a set
    /// </summary>
    [JsonObject("release")]
    public class Release : DatItem
    {
        #region Fields

        /// <summary>
        /// Release region(s)
        /// </summary>
        [JsonProperty("region", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Region { get; set; }

        /// <summary>
        /// Release language(s)
        /// </summary>
        [JsonProperty("language", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Language { get; set; }

        /// <summary>
        /// Date of release
        /// </summary>
        [JsonProperty("date", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Date { get; set; }

        /// <summary>
        /// Default release, if applicable
        /// </summary>
        [JsonProperty("default", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? Default { get; set; }

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

            // Handle Release-specific fields
            if (mappings.Keys.Contains(Field.Region))
                Region = mappings[Field.Region];

            if (mappings.Keys.Contains(Field.Language))
                Language = mappings[Field.Language];

            if (mappings.Keys.Contains(Field.Date))
                Date = mappings[Field.Date];

            if (mappings.Keys.Contains(Field.Default))
                Default = mappings[Field.Default].AsYesNo();
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Release object
        /// </summary>
        public Release()
        {
            Name = string.Empty;
            ItemType = ItemType.Release;
            Region = string.Empty;
            Language = string.Empty;
            Date = string.Empty;
            Default = null;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new Release()
            {
                Name = this.Name,
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                AltName = this.AltName,
                AltTitle = this.AltTitle,

                Original = this.Original,
                OpenMSXSubType = this.OpenMSXSubType,
                OpenMSXType = this.OpenMSXType,
                Remark = this.Remark,
                Boot = this.Boot,

                PartName = this.PartName,
                PartInterface = this.PartInterface,
                Features = this.Features,
                AreaName = this.AreaName,
                AreaSize = this.AreaSize,
                AreaWidth = this.AreaWidth,
                AreaEndianness = this.AreaEndianness,
                Value = this.Value,
                LoadFlag = this.LoadFlag,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Region = this.Region,
                Language = this.Language,
                Date = this.Date,
                Default = this.Default,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a release return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Release
            Release newOther = other as Release;

            // If the archive information matches
            return (Name == newOther.Name
                && Region == newOther.Region
                && Language == newOther.Language
                && Date == newOther.Date
                && Default == newOther.Default);
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

            // Filter on region
            if (filter.Region.MatchesPositiveSet(Region) == false)
                return false;
            if (filter.Region.MatchesNegativeSet(Region) == true)
                return false;

            // Filter on language
            if (filter.Language.MatchesPositiveSet(Language) == false)
                return false;
            if (filter.Language.MatchesNegativeSet(Language) == true)
                return false;

            // Filter on date
            if (filter.Date.MatchesPositiveSet(Date) == false)
                return false;
            if (filter.Date.MatchesNegativeSet(Date) == true)
                return false;

            // Filter on default
            if (filter.Default.MatchesNeutral(null, Default) == false)
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
            if (fields.Contains(Field.Region))
                Region = null;

            if (fields.Contains(Field.Language))
                Language = null;

            if (fields.Contains(Field.Date))
                Date = null;

            if (fields.Contains(Field.Default))
                Default = null;
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

            // If we don't have a Release to replace from, ignore specific fields
            if (item.ItemType != ItemType.Release)
                return;

            // Cast for easier access
            Release newItem = item as Release;

            // Replace the fields
            if (fields.Contains(Field.Region))
                Region = newItem.Region;

            if (fields.Contains(Field.Language))
                Language = newItem.Language;

            if (fields.Contains(Field.Date))
                Date = newItem.Date;

            if (fields.Contains(Field.Default))
                Default = newItem.Default;
        }

        #endregion
    }
}
