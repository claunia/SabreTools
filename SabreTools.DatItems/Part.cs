using System.Collections.Generic;
using System.Xml.Serialization;

using SabreTools.Core;
using Newtonsoft.Json;

namespace SabreTools.DatItems
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

        /// <inheritdoc/>
        public override string GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string name) => Name = name;

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

        #region Sorting and Merging

        /// <inheritdoc/>
        public override void ReplaceFields(
            DatItem item,
            List<DatItemField> datItemFields,
            List<MachineField> machineFields)
        {
            // Replace common fields first
            base.ReplaceFields(item, datItemFields, machineFields);

            // If we don't have a Part to replace from, ignore specific fields
            if (item.ItemType != ItemType.Part)
                return;

            // Cast for easier access
            Part newItem = item as Part;

            // Replace the fields
            if (datItemFields.Contains(DatItemField.Part_Name))
                Name = newItem.Name;

            if (datItemFields.Contains(DatItemField.Part_Interface))
                Interface = newItem.Interface;

            // DatItem_Part_Feature_* doesn't make sense here
            // since not every part feature under the other item
            // can replace every part feature under this item
        }

        #endregion
    }
}
