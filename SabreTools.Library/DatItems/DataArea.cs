using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using SabreTools.Library.Filtering;
using SabreTools.Library.Tools;
using Newtonsoft.Json;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// SoftwareList dataarea information
    /// </summary>
    /// <remarks>One DataArea can contain multiple Rom items</remarks>
    [JsonObject("dataarea"), XmlRoot("dataarea")]
    public class DataArea : DatItem
    {
        #region Fields

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Total size of the area
        /// </summary>
        [JsonProperty("size", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("size")]
        public long? Size { get; set; }

        [JsonIgnore]
        public bool SizeSpecified { get { return Size != null; } }

        /// <summary>
        /// Word width for the area
        /// </summary>
        [JsonProperty("width", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("width")]
        public long? Width { get; set; }

        [JsonIgnore]
        public bool WidthSpecified { get { return Width != null; } }

        /// <summary>
        /// Byte endianness of the area
        /// </summary>
        [JsonProperty("endianness", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("endianness")]
        public Endianness Endianness { get; set; }

        [JsonIgnore]
        public bool EndiannessSpecified { get { return Endianness != Endianness.NULL; } }

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

            // Handle DataArea-specific fields
            if (mappings.Keys.Contains(Field.DatItem_AreaName))
                Name = mappings[Field.DatItem_AreaName];

            if (mappings.Keys.Contains(Field.DatItem_AreaSize))
                Size = Sanitizer.CleanLong(mappings[Field.DatItem_AreaSize]);

            if (mappings.Keys.Contains(Field.DatItem_AreaWidth))
                Width = Sanitizer.CleanLong(mappings[Field.DatItem_AreaWidth]);

            if (mappings.Keys.Contains(Field.DatItem_AreaEndianness))
                Endianness = mappings[Field.DatItem_AreaEndianness].AsEndianness();
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty DataArea object
        /// </summary>
        public DataArea()
        {
            Name = string.Empty;
            ItemType = ItemType.DataArea;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new DataArea()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Name = this.Name,
                Size = this.Size,
                Width = this.Width,
                Endianness = this.Endianness,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a DataArea, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a DataArea
            DataArea newOther = other as DataArea;

            // If the DataArea information matches
            return (Name == newOther.Name
                && Size == newOther.Size
                && Width == newOther.Width
                && Endianness == newOther.Endianness);
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
        /// <returns>True if the item passed the filter, false otherwise</returns>
        public override bool PassesFilter(Filter filter)
        {
            // Check common fields first
            if (!base.PassesFilter(filter))
                return false;

            // Filter on area name
            if (!PassStringFilter(filter.DatItem_AreaName, Name))
                return false;

            // Filter on area size
            if (!PassLongFilter(filter.DatItem_AreaSize, Size))
                return false;

            // Filter on area width
            if (!PassLongFilter(filter.DatItem_AreaWidth, Width))
                return false;

            // Filter on area endianness
            if (filter.DatItem_AreaEndianness.MatchesPositive(Endianness.NULL, Endianness) == false)
                return false;
            if (filter.DatItem_AreaEndianness.MatchesNegative(Endianness.NULL, Endianness) == true)
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
            if (fields.Contains(Field.DatItem_AreaName))
                Name = null;

            if (fields.Contains(Field.DatItem_AreaSize))
                Size = null;

            if (fields.Contains(Field.DatItem_AreaWidth))
                Width = null;

            if (fields.Contains(Field.DatItem_AreaEndianness))
                Endianness = Endianness.NULL;
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

            // If we don't have a DataArea to replace from, ignore specific fields
            if (item.ItemType != ItemType.DataArea)
                return;

            // Cast for easier access
            DataArea newItem = item as DataArea;

            // Replace the fields
            if (fields.Contains(Field.DatItem_AreaName))
                Name = newItem.Name;

            if (fields.Contains(Field.DatItem_AreaSize))
                Size = newItem.Size;

            if (fields.Contains(Field.DatItem_AreaWidth))
                Width = newItem.Width;

            if (fields.Contains(Field.DatItem_AreaEndianness))
                Endianness = newItem.Endianness;
        }

        #endregion
    }
}
