using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.Filtering;
using Newtonsoft.Json;

namespace SabreTools.DatItems
{
    /// <summary>
    /// Represents one ListXML slotoption
    /// </summary>
    [JsonObject("slotoption"), XmlRoot("slotoption")]
    public class SlotOption : DatItem
    {
        #region Fields

        /// <summary>
        /// Slot option name
        /// </summary>
        [JsonProperty("name")]
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Referenced device name
        /// </summary>
        [JsonProperty("devname")]
        [XmlElement("devname")]
        public string DeviceName { get; set; }

        /// <summary>
        /// Determines if this slot option is default or not
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

            // Handle SlotOption-specific fields
            if (mappings.Keys.Contains(Field.DatItem_SlotOption_Name))
                Name = mappings[Field.DatItem_SlotOption_Name];

            if (mappings.Keys.Contains(Field.DatItem_SlotOption_DeviceName))
                DeviceName = mappings[Field.DatItem_SlotOption_DeviceName];

            if (mappings.Keys.Contains(Field.DatItem_SlotOption_Default))
                Default = mappings[Field.DatItem_SlotOption_Default].AsYesNo();
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty SlotOption object
        /// </summary>
        public SlotOption()
        {
            Name = string.Empty;
            ItemType = ItemType.SlotOption;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new SlotOption()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Name = this.Name,
                DeviceName = this.DeviceName,
                Default = this.Default,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a SlotOption, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a SlotOption
            SlotOption newOther = other as SlotOption;

            // If the SlotOption information matches
            return (Name == newOther.Name
                && DeviceName == newOther.DeviceName
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
            if (!filter.PassStringFilter(filter.DatItem_SlotOption_Name, Name))
                return false;

            // Filter on device name
            if (!filter.PassStringFilter(filter.DatItem_SlotOption_DeviceName, DeviceName))
                return false;

            // Filter on default
            if (!filter.PassBoolFilter(filter.DatItem_SlotOption_Default, Default))
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
            if (fields.Contains(Field.DatItem_SlotOption_Name))
                Name = null;

            if (fields.Contains(Field.DatItem_SlotOption_DeviceName))
                DeviceName = null;

            if (fields.Contains(Field.DatItem_SlotOption_Default))
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

            // If we don't have a SlotOption to replace from, ignore specific fields
            if (item.ItemType != ItemType.SlotOption)
                return;

            // Cast for easier access
            SlotOption newItem = item as SlotOption;

            // Replace the fields
            if (fields.Contains(Field.DatItem_SlotOption_Name))
                Name = newItem.Name;

            if (fields.Contains(Field.DatItem_SlotOption_DeviceName))
                DeviceName = newItem.DeviceName;

            if (fields.Contains(Field.DatItem_SlotOption_Default))
                Default = newItem.Default;
        }

        #endregion
    }
}
