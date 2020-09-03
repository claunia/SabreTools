using System.Collections.Generic;
using System.IO;
using System.Linq;

using SabreTools.Library.Filtering;
using SabreTools.Library.Tools;
using Newtonsoft.Json;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Represents which DIP Switch(es) is associated with a set
    /// </summary>
    [JsonObject("dipswitch")]
    public class DipSwitch : DatItem
    {
        #region Fields

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Tag associated with the dipswitch
        /// </summary>
        [JsonProperty("tag", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Tag { get; set; }

        /// <summary>
        /// Mask associated with the dipswitch
        /// </summary>
        [JsonProperty("mask", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Mask { get; set; }

        /// <summary>
        /// Conditions associated with the dipswitch
        /// </summary>
        [JsonProperty("conditions", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<Condition> Conditions { get; set; }

        /// <summary>
        /// Locations associated with the dipswitch
        /// </summary>
        [JsonProperty("locations", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<Location> Locations { get; set; }

        /// <summary>
        /// Settings associated with the dipswitch
        /// </summary>
        [JsonProperty("values", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<Setting> Values { get; set; }

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

            // Handle DipSwitch-specific fields
            if (mappings.Keys.Contains(Field.DatItem_Name))
                Name = mappings[Field.DatItem_Name];

            if (mappings.Keys.Contains(Field.DatItem_Tag))
                Tag = mappings[Field.DatItem_Tag];

            if (mappings.Keys.Contains(Field.DatItem_Mask))
                Mask = mappings[Field.DatItem_Mask];

            // TODO: Handle DatItem_Condition*
            // TODO: Handle DatItem_Location*
            // TODO: Handle DatItem_Value*
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty DipSwitch object
        /// </summary>
        public DipSwitch()
        {
            Name = string.Empty;
            ItemType = ItemType.DipSwitch;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new DipSwitch()
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

                Part = this.Part,
                Value = this.Value,
                LoadFlag = this.LoadFlag,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Tag = this.Tag,
                Mask = this.Mask,
                Conditions = this.Conditions,
                Locations = this.Locations,
                Values = this.Values,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a DipSwitch, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a DipSwitch
            DipSwitch newOther = other as DipSwitch;

            // If the DipSwitch information matches
            return (Name == newOther.Name && Tag == newOther.Tag && Mask == newOther.Mask);
            
            // TODO: Handle DatItem_Condition*
            // TODO: Handle DatItem_Location*
            // TODO: Handle DatItem_Value*
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

            // Filter on tag
            if (filter.DatItem_Tag.MatchesPositiveSet(Tag) == false)
                return false;
            if (filter.DatItem_Tag.MatchesNegativeSet(Tag) == true)
                return false;

            // Filter on mask
            if (filter.DatItem_Mask.MatchesPositiveSet(Mask) == false)
                return false;
            if (filter.DatItem_Mask.MatchesNegativeSet(Mask) == true)
                return false;

            // TODO: Handle DatItem_Condition*
            // TODO: Handle DatItem_Location*
            // TODO: Handle DatItem_Value*

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

            if (fields.Contains(Field.DatItem_Tag))
                Tag = null;

            if (fields.Contains(Field.DatItem_Mask))
                Mask = null;

            if (fields.Contains(Field.DatItem_Conditions))
                Conditions = null;

            if (fields.Contains(Field.DatItem_Locations))
                Locations = null;

            if (fields.Contains(Field.DatItem_Values))
                Values = null;

            // TODO: Handle DatItem_Condition*
            // TODO: Handle DatItem_Location*
            // TODO: Handle DatItem_Value*
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

            // If we don't have a DipSwitch to replace from, ignore specific fields
            if (item.ItemType != ItemType.DipSwitch)
                return;

            // Cast for easier access
            DipSwitch newItem = item as DipSwitch;

            // Replace the fields
            if (fields.Contains(Field.DatItem_Name))
                Name = newItem.Name;

            if (fields.Contains(Field.DatItem_Tag))
                Tag = newItem.Tag;

            if (fields.Contains(Field.DatItem_Mask))
                Mask = newItem.Mask;

            if (fields.Contains(Field.DatItem_Conditions))
                Conditions = newItem.Conditions;

            if (fields.Contains(Field.DatItem_Locations))
                Locations = newItem.Locations;

            if (fields.Contains(Field.DatItem_Values))
                Values = newItem.Values;

            // TODO: Handle DatItem_Condition*
            // TODO: Handle DatItem_Location*
            // TODO: Handle DatItem_Value*
        }

        #endregion
    }
}
