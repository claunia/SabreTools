using System.Collections.Generic;
using System.IO;
using System.Linq;
using SabreTools.Library.Filtering;
using Newtonsoft.Json;
using SabreTools.Library.Tools;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Represents special information about a machine
    /// </summary>
    [JsonObject("info")]
    public class Info : DatItem
    {
        #region Fields

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Information value
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }

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

            // Handle Info-specific fields
            if (mappings.Keys.Contains(Field.DatItem_Name))
                Name = mappings[Field.DatItem_Name];

            if (mappings.Keys.Contains(Field.DatItem_Value))
                Value = mappings[Field.DatItem_Value];
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Info object
        /// </summary>
        public Info()
        {
            Name = string.Empty;
            ItemType = ItemType.Info;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new Info()
            {
                Name = this.Name,
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                AltName = this.AltName,
                AltTitle = this.AltTitle,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Value = this.Value,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a sample, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Info
            Info newOther = other as Info;

            // If the archive information matches
            return (Name == newOther.Name && Value == newOther.Value);
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

            // Filter on item name
            if (filter.DatItem_Name.MatchesPositiveSet(Name) == false)
                return false;
            if (filter.DatItem_Name.MatchesNegativeSet(Name) == true)
                return false;

            // Filter on info value
            if (filter.DatItem_Value.MatchesPositiveSet(Value) == false)
                return false;
            if (filter.DatItem_Value.MatchesNegativeSet(Value) == true)
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

            if (fields.Contains(Field.DatItem_Value))
                Value = null;
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

            // If we don't have a Info to replace from, ignore specific fields
            if (item.ItemType != ItemType.Info)
                return;

            // Cast for easier access
            Info newItem = item as Info;

            // Replace the fields
            if (fields.Contains(Field.DatItem_Name))
                Name = newItem.Name;

            if (fields.Contains(Field.DatItem_Value))
                Value = newItem.Value;
        }

        #endregion
    }
}
