using System;
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
    /// SoftwareList part information
    /// </summary>
    /// <remarks>One Part can contain multiple PartFeature, DataArea, DiskArea, and DipSwitch items</remarks>
    [JsonObject("part"), XmlRoot("part")]
    public class Part : DatItem
    {
        #region Fields

        [JsonProperty("name")]
        [XmlElement("name")]
        public string Name { get; set; }

        [JsonProperty("interface")]
        [XmlElement("interface")]
        public string Interface { get; set; }
    
        [JsonProperty("features", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("features")]
        public List<PartFeature> Features { get; set; }

        [JsonIgnore]
        public bool FeaturesSpecified { get { return Features != null && Features.Count > 0; } }

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

            // Handle Part-specific fields
            if (mappings.Keys.Contains(Field.DatItem_Part_Name))
                Name = mappings[Field.DatItem_Part_Name];

            if (mappings.Keys.Contains(Field.DatItem_Part_Interface))
                Interface = mappings[Field.DatItem_Part_Interface];

            // Handle Feature-specific fields
            if (FeaturesSpecified)
            {
                foreach (PartFeature partFeature in Features)
                {
                    partFeature.SetFields(mappings);
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Part object
        /// </summary>
        public Part()
        {
            Name = string.Empty;
            ItemType = ItemType.Part;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new Part()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Name = this.Name,
                Interface = this.Interface,
                Features = this.Features,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a Part, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Part
            Part newOther = other as Part;

            // If the Part information matches
            bool match = (Name == newOther.Name
                && Interface == newOther.Interface);
            if (!match)
                return match;

            // If the features match
            if (FeaturesSpecified)
            {
                foreach (PartFeature partFeature in Features)
                {
                    match &= newOther.Features.Contains(partFeature);
                }
            }

            return match;
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

            // Filter on part name
            if (!filter.PassStringFilter(filter.DatItem_Part_Name, Name))
                return false;

            // Filter on part interface
            if (!filter.PassStringFilter(filter.DatItem_Part_Interface, Interface))
                return false;

            // Filter on features
            if (FeaturesSpecified)
            {
                foreach (PartFeature partFeature in Features)
                {
                    if (!partFeature.PassesFilter(filter, true))
                        return false;
                }
            }

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
            if (fields.Contains(Field.DatItem_Part_Name))
                Name = null;

            if (fields.Contains(Field.DatItem_Part_Interface))
                Interface = null;

            if (FeaturesSpecified)
            {
                foreach (PartFeature partFeature in Features)
                {
                    partFeature.RemoveFields(fields);
                }
            }
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

            // If we don't have a Part to replace from, ignore specific fields
            if (item.ItemType != ItemType.Part)
                return;

            // Cast for easier access
            Part newItem = item as Part;

            // Replace the fields
            if (fields.Contains(Field.DatItem_Part_Name))
                Name = newItem.Name;

            if (fields.Contains(Field.DatItem_Part_Interface))
                Interface = newItem.Interface;

            // DatItem_Part_Feature_* doesn't make sense here
            // since not every part feature under the other item
            // can replace every part feature under this item
        }

        #endregion
    }
}
