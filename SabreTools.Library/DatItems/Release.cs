using System.Collections.Generic;
using SabreTools.Library.Filtering;
using Newtonsoft.Json;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Represents release information about a set
    /// </summary>
    public class Release : DatItem
    {
        #region Publicly facing variables

        /// <summary>
        /// Release region(s)
        /// </summary>
        [JsonProperty("region")]
        public string Region { get; set; }

        /// <summary>
        /// Release language(s)
        /// </summary>
        [JsonProperty("language")]
        public string Language { get; set; }

        /// <summary>
        /// Date of release
        /// </summary>
        [JsonProperty("date")]
        public string Date { get; set; }

        /// <summary>
        /// Default release, if applicable
        /// </summary>
        [JsonProperty("default")]
        public bool? Default { get; set; }

        #endregion

        #region Accessors

        /// <summary>
        /// Get the value of that field as a string, if possible
        /// </summary>
        public override string GetField(Field field, List<Field> excludeFields)
        {
            // If the field is to be excluded, return empty string
            if (excludeFields.Contains(field))
                return string.Empty;

            // Handle Release-specific fields
            string fieldValue;
            switch (field)
            {
                case Field.Region:
                    fieldValue = Region;
                    break;
                case Field.Language:
                    fieldValue = Language;
                    break;
                case Field.Date:
                    fieldValue = Date;
                    break;
                case Field.Default:
                    fieldValue = Default?.ToString();
                    break;

                // For everything else, use the base method
                default:
                    return base.GetField(field, excludeFields);
            }

            // Make sure we don't return null
            if (string.IsNullOrEmpty(fieldValue))
                fieldValue = string.Empty;

            return fieldValue;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Release object
        /// </summary>
        public Release()
        {
            this.Name = string.Empty;
            this.ItemType = ItemType.Release;
            this.Region = string.Empty;
            this.Language = string.Empty;
            this.Date = string.Empty;
            this.Default = null;
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

                Supported = this.Supported,
                Publisher = this.Publisher,
                Category = this.Category,
                Infos = this.Infos,
                PartName = this.PartName,
                PartInterface = this.PartInterface,
                Features = this.Features,
                AreaName = this.AreaName,
                AreaSize = this.AreaSize,

                MachineName = this.MachineName,
                Comment = this.Comment,
                MachineDescription = this.MachineDescription,
                Year = this.Year,
                Manufacturer = this.Manufacturer,
                RomOf = this.RomOf,
                CloneOf = this.CloneOf,
                SampleOf = this.SampleOf,
                SourceFile = this.SourceFile,
                Runnable = this.Runnable,
                Board = this.Board,
                RebuildTo = this.RebuildTo,
                Devices = this.Devices,
                MachineType = this.MachineType,

                IndexId = this.IndexId,
                IndexSource = this.IndexSource,

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
            if (this.ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Release
            Release newOther = other as Release;

            // If the archive information matches
            return (this.Name == newOther.Name
                && this.Region == newOther.Region
                && this.Language == newOther.Language
                && this.Date == newOther.Date
                && this.Default == newOther.Default);
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
