using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using SabreTools.Data;
using SabreTools.Library.Filtering;
using SabreTools.Library.Tools;
using Newtonsoft.Json;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Represents release information about a set
    /// </summary>
    [JsonObject("release"), XmlRoot("release")]
    public class Release : DatItem
    {
        #region Fields

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name")]
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Release region(s)
        /// </summary>
        [JsonProperty("region", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("region")]
        public string Region { get; set; }

        /// <summary>
        /// Release language(s)
        /// </summary>
        [JsonProperty("language", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("language")]
        public string Language { get; set; }

        /// <summary>
        /// Date of release
        /// </summary>
        [JsonProperty("date", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("date")]
        public string Date { get; set; }

        /// <summary>
        /// Default release, if applicable
        /// </summary>
        [JsonProperty("default", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("default")]
        public bool? Default { get; set; }

        [JsonIgnore]
        public bool DefaultSpecified { get { return Default != null; } }

        #endregion

        #region Accessors

        /// <summary>
        /// Gets the name to use for a DatItem
        /// </summary>
        /// <returns>Name if available, null otherwise</returns>
        public override string GetName()
        {
            return Name;
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="mappings">Mappings dictionary</param>
        public override void SetFields(Dictionary<Field, string> mappings)
        {
            // Set base fields
            base.SetFields(mappings);

            // Handle Release-specific fields
            if (mappings.Keys.Contains(Field.DatItem_Name))
                Name = mappings[Field.DatItem_Name];

            if (mappings.Keys.Contains(Field.DatItem_Region))
                Region = mappings[Field.DatItem_Region];

            if (mappings.Keys.Contains(Field.DatItem_Language))
                Language = mappings[Field.DatItem_Language];

            if (mappings.Keys.Contains(Field.DatItem_Date))
                Date = mappings[Field.DatItem_Date];

            if (mappings.Keys.Contains(Field.DatItem_Default))
                Default = mappings[Field.DatItem_Default].AsYesNo();
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
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Name = this.Name,
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
        /// Clean a DatItem according to the cleaner
        /// </summary>
        /// <param name="cleaner">Cleaner to implement</param>
        public override void Clean(Cleaner cleaner)
        {
            // Clean common items first
            base.Clean(cleaner);

            // If we're stripping unicode characters, strip item name
            if (cleaner?.RemoveUnicode == true)
                Name = Sanitizer.RemoveUnicodeCharacters(Name);

            // If we are in NTFS trim mode, trim the game name
            if (cleaner?.Trim == true)
            {
                // Windows max name length is 260
                int usableLength = 260 - Machine.Name.Length - (cleaner.Root?.Length ?? 0);
                if (Name.Length > usableLength)
                {
                    string ext = Path.GetExtension(Name);
                    Name = Name.Substring(0, usableLength - ext.Length);
                    Name += ext;
                }
            }
        }

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

            // Filter on item name
            if (!filter.PassStringFilter(filter.DatItem_Name, Name))
                return false;

            // Filter on region
            if (!filter.PassStringFilter(filter.DatItem_Region, Region))
                return false;

            // Filter on language
            if (!filter.PassStringFilter(filter.DatItem_Language, Language))
                return false;

            // Filter on date
            if (!filter.PassStringFilter(filter.DatItem_Date, Date))
                return false;

            // Filter on default
            if (!filter.PassBoolFilter(filter.DatItem_Default, Default))
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
            if (fields.Contains(Field.DatItem_Name))
                Name = null;

            if (fields.Contains(Field.DatItem_Region))
                Region = null;

            if (fields.Contains(Field.DatItem_Language))
                Language = null;

            if (fields.Contains(Field.DatItem_Date))
                Date = null;

            if (fields.Contains(Field.DatItem_Default))
                Default = null;
        }

        /// <summary>
        /// Set internal names to match One Rom Per Game (ORPG) logic
        /// </summary>
        public override void SetOneRomPerGame()
        {
            string[] splitname = Name.Split('.');
            Machine.Name += $"/{string.Join(".", splitname.Take(splitname.Length > 1 ? splitname.Length - 1 : 1))}";
            Name = Path.GetFileName(Name);
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
            if (fields.Contains(Field.DatItem_Name))
                Name = newItem.Name;

            if (fields.Contains(Field.DatItem_Region))
                Region = newItem.Region;

            if (fields.Contains(Field.DatItem_Language))
                Language = newItem.Language;

            if (fields.Contains(Field.DatItem_Date))
                Date = newItem.Date;

            if (fields.Contains(Field.DatItem_Default))
                Default = newItem.Default;
        }

        #endregion
    }
}
