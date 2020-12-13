using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using SabreTools.Core;
using SabreTools.Filtering;
using Newtonsoft.Json;

namespace SabreTools.DatItems
{
    /// <summary>
    /// Represents which DIP Switch(es) is associated with a set
    /// </summary>
    [JsonObject("dipswitch"), XmlRoot("dipswitch")]
    public class DipSwitch : DatItem
    {
        #region Fields

        #region Common

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name")]
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Tag associated with the dipswitch
        /// </summary>
        [JsonProperty("tag", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("tag")]
        public string Tag { get; set; }

        /// <summary>
        /// Mask associated with the dipswitch
        /// </summary>
        [JsonProperty("mask", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("mask")]
        public string Mask { get; set; }

        /// <summary>
        /// Conditions associated with the dipswitch
        /// </summary>
        [JsonProperty("conditions", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("conditions")]
        public List<Condition> Conditions { get; set; }

        [JsonIgnore]
        public bool ConditionsSpecified { get { return Conditions != null && Conditions.Count > 0; } }

        /// <summary>
        /// Locations associated with the dipswitch
        /// </summary>
        [JsonProperty("locations", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("locations")]
        public List<Location> Locations { get; set; }

        [JsonIgnore]
        public bool LocationsSpecified { get { return Locations != null && Locations.Count > 0; } }

        /// <summary>
        /// Settings associated with the dipswitch
        /// </summary>
        [JsonProperty("values", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("values")]
        public List<Setting> Values { get; set; }

        [JsonIgnore]
        public bool ValuesSpecified { get { return Values != null && Values.Count > 0; } }

        #endregion

        #region SoftwareList

        /// <summary>
        /// Original hardware part associated with the item
        /// </summary>
        [JsonProperty("part", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("part")]
        public Part Part { get; set; } = null;

        [JsonIgnore]
        public bool PartSpecified
        {
            get
            {
                return Part != null
                    && (!string.IsNullOrEmpty(Part.Name)
                        || !string.IsNullOrEmpty(Part.Interface));
            }
        }

        #endregion

        #endregion // Fields

        #region Accessors

        /// <inheritdoc/>
        public override string GetName()
        {
            return Name;
        }

        /// <inheritdoc/>
        public override void SetName(string name)
        {
            Name = name;
        }

        /// <inheritdoc/>
        public override void SetFields(
            Dictionary<DatItemField, string> datItemMappings,
            Dictionary<MachineField, string> machineMappings)
        {
            // Set base fields
            base.SetFields(datItemMappings, machineMappings);

            // Handle DipSwitch-specific fields

            #region Common

            if (datItemMappings.Keys.Contains(DatItemField.Name))
                Name = datItemMappings[DatItemField.Name];

            if (datItemMappings.Keys.Contains(DatItemField.Tag))
                Tag = datItemMappings[DatItemField.Tag];

            if (datItemMappings.Keys.Contains(DatItemField.Mask))
                Mask = datItemMappings[DatItemField.Mask];

            if (ConditionsSpecified)
            {
                foreach (Condition condition in Conditions)
                {
                    condition.SetFields(datItemMappings, machineMappings, true);
                }
            }

            if (LocationsSpecified)
            {
                foreach (Location location in Locations)
                {
                    location.SetFields(datItemMappings, machineMappings);
                }
            }

            if (ValuesSpecified)
            {
                foreach (Setting value in Values)
                {
                    value.SetFields(datItemMappings, machineMappings);
                }
            }

            #endregion

            #region SoftwareList

            // Handle Part-specific fields
            if (Part == null)
                Part = new Part();

            Part.SetFields(datItemMappings, machineMappings);

            #endregion
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
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Name = this.Name,
                Tag = this.Tag,
                Mask = this.Mask,
                Conditions = this.Conditions,
                Locations = this.Locations,
                Values = this.Values,

                Part = this.Part,
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
            bool match = (Name == newOther.Name
                && Tag == newOther.Tag
                && Mask == newOther.Mask);
            if (!match)
                return match;

            // If the part matches
            if (PartSpecified)
                match &= (Part == newOther.Part);

            // If the conditions match
            if (ConditionsSpecified)
            {
                foreach (Condition condition in Conditions)
                {
                    match &= newOther.Conditions.Contains(condition);
                }
            }

            // If the locations match
            if (LocationsSpecified)
            {
                foreach (Location location in Locations)
                {
                    match &= newOther.Locations.Contains(location);
                }
            }

            // If the values match
            if (ValuesSpecified)
            {
                foreach (Setting value in Values)
                {
                    match &= newOther.Values.Contains(value);
                }
            }

            return match;
        }

        #endregion

        #region Filtering

        /// <inheritdoc/>
        public override bool PassesFilter(Cleaner cleaner, bool sub = false)
        {
            // Check common fields first
            if (!base.PassesFilter(cleaner, sub))
                return false;

            #region Common

            // Filter on item name
            if (!Filter.PassStringFilter(cleaner.DatItemFilter.Name, Name))
                return false;

            // Filter on tag
            if (!Filter.PassStringFilter(cleaner.DatItemFilter.Tag, Tag))
                return false;

            // Filter on mask
            if (!Filter.PassStringFilter(cleaner.DatItemFilter.Mask, Mask))
                return false;

            // Filter on individual conditions
            if (ConditionsSpecified)
            {
                foreach (Condition condition in Conditions)
                {
                    if (!condition.PassesFilter(cleaner, true))
                        return false;
                }
            }

            // Filter on individual locations
            if (LocationsSpecified)
            {
                foreach (Location location in Locations)
                {
                    if (!location.PassesFilter(cleaner, true))
                        return false;
                }
            }

            // Filter on individual conditions
            if (ValuesSpecified)
            {
                foreach (Setting value in Values)
                {
                    if (!value.PassesFilter(cleaner, true))
                        return false;
                }
            }

            #endregion

            #region SoftwareList

            // Filter on Part
            if (PartSpecified)
            {
                if (!Part.PassesFilter(cleaner, true))
                    return false;
            }

            #endregion

            return true;
        }

        /// <inheritdoc/>
        public override void RemoveFields(
            List<DatItemField> datItemFields,
            List<MachineField> machineFields)
        {
            // Remove common fields first
            base.RemoveFields(datItemFields, machineFields);

            // Remove the fields

            #region Common

            if (datItemFields.Contains(DatItemField.Name))
                Name = null;

            if (datItemFields.Contains(DatItemField.Tag))
                Tag = null;

            if (datItemFields.Contains(DatItemField.Mask))
                Mask = null;

            if (ConditionsSpecified)
            {
                foreach (Condition condition in Conditions)
                {
                    condition.RemoveFields(datItemFields, machineFields, true);
                }
            }

            if (LocationsSpecified)
            {
                foreach (Location location in Locations)
                {
                    location.RemoveFields(datItemFields, machineFields);
                }
            }

            if (ValuesSpecified)
            {
                foreach (Setting value in Values)
                {
                    value.RemoveFields(datItemFields, machineFields);
                }
            }

            #endregion

            #region SoftwareList

            if (PartSpecified)
                Part.RemoveFields(datItemFields, machineFields);

            #endregion
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

        /// <inheritdoc/>
        public override void ReplaceFields(
            DatItem item,
            List<DatItemField> datItemFields,
            List<MachineField> machineFields)
        {
            // Replace common fields first
            base.ReplaceFields(item, datItemFields, machineFields);

            // If we don't have a DipSwitch to replace from, ignore specific fields
            if (item.ItemType != ItemType.DipSwitch)
                return;

            // Cast for easier access
            DipSwitch newItem = item as DipSwitch;

            // Replace the fields

            #region Common

            if (datItemFields.Contains(DatItemField.Name))
                Name = newItem.Name;

            if (datItemFields.Contains(DatItemField.Tag))
                Tag = newItem.Tag;

            if (datItemFields.Contains(DatItemField.Mask))
                Mask = newItem.Mask;

            // DatItem_Condition_* doesn't make sense here
            // since not every condition under the other item
            // can replace every condition under this item

            // DatItem_Location_* doesn't make sense here
            // since not every location under the other item
            // can replace every location under this item

            // DatItem_Setting_* doesn't make sense here
            // since not every value under the other item
            // can replace every value under this item

            #endregion

            #region SoftwareList

            if (PartSpecified && newItem.PartSpecified)
                Part.ReplaceFields(newItem.Part, datItemFields, machineFields);

            #endregion
        }

        #endregion
    }
}
