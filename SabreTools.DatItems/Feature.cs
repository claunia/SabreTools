using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.Filtering;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SabreTools.DatItems
{
    /// <summary>
    /// Represents the a feature of the machine
    /// </summary>
    [JsonObject("feature"), XmlRoot("feature")]
    public class Feature : DatItem
    {
        #region Fields

        /// <summary>
        /// Type of feature
        /// </summary>
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlElement("type")]
        public FeatureType Type { get; set; }

        [JsonIgnore]
        public bool TypeSpecified { get { return Type != FeatureType.NULL; } }

        /// <summary>
        /// Emulation status
        /// </summary>
        [JsonProperty("status", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlElement("status")]
        public FeatureStatus Status { get; set; }

        [JsonIgnore]
        public bool StatusSpecified { get { return Status != FeatureStatus.NULL; } }

        /// <summary>
        /// Overall status
        /// </summary>
        [JsonProperty("overall", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlElement("overall")]
        public FeatureStatus Overall { get; set; }

        [JsonIgnore]
        public bool OverallSpecified { get { return Overall != FeatureStatus.NULL; } }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override void SetFields(
            Dictionary<DatItemField, string> datItemMappings,
            Dictionary<MachineField, string> machineMappings)
        {
            // Set base fields
            base.SetFields(datItemMappings, machineMappings);

            // Handle Feature-specific fields
            if (datItemMappings.Keys.Contains(DatItemField.FeatureType))
                Type = datItemMappings[DatItemField.FeatureType].AsFeatureType();

            if (datItemMappings.Keys.Contains(DatItemField.FeatureStatus))
                Status = datItemMappings[DatItemField.FeatureStatus].AsFeatureStatus();

            if (datItemMappings.Keys.Contains(DatItemField.FeatureOverall))
                Overall = datItemMappings[DatItemField.FeatureOverall].AsFeatureStatus();
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Feature object
        /// </summary>
        public Feature()
        {
            ItemType = ItemType.Feature;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new Feature()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Type = this.Type,
                Status = this.Status,
                Overall = this.Overall,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a Feature, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Feature
            Feature newOther = other as Feature;

            // If the Feature information matches
            return (Type == newOther.Type && Status == newOther.Status && Overall == newOther.Overall);
        }

        #endregion

        #region Filtering

        /// <inheritdoc/>
        public override bool PassesFilter(Cleaner cleaner, bool sub = false)
        {
            // Check common fields first
            if (!base.PassesFilter(cleaner, sub))
                return false;

            // Filter on type
            if (cleaner.DatItemFilter.FeatureType.MatchesPositive(FeatureType.NULL, Type) == false)
                return false;
            if (cleaner.DatItemFilter.FeatureType.MatchesNegative(FeatureType.NULL, Type) == true)
                return false;

            // Filter on status
            if (cleaner.DatItemFilter.FeatureStatus.MatchesPositive(FeatureStatus.NULL, Status) == false)
                return false;
            if (cleaner.DatItemFilter.FeatureStatus.MatchesNegative(FeatureStatus.NULL, Status) == true)
                return false;

            // Filter on overall
            if (cleaner.DatItemFilter.FeatureOverall.MatchesPositive(FeatureStatus.NULL, Overall) == false)
                return false;
            if (cleaner.DatItemFilter.FeatureOverall.MatchesNegative(FeatureStatus.NULL, Overall) == true)
                return false;

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
            if (datItemFields.Contains(DatItemField.FeatureType))
                Type = FeatureType.NULL;

            if (datItemFields.Contains(DatItemField.FeatureStatus))
                Status = FeatureStatus.NULL;

            if (datItemFields.Contains(DatItemField.FeatureOverall))
                Overall = FeatureStatus.NULL;
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

            // If we don't have a Feature to replace from, ignore specific fields
            if (item.ItemType != ItemType.Feature)
                return;

            // Cast for easier access
            Feature newItem = item as Feature;

            // Replace the fields
            if (datItemFields.Contains(DatItemField.FeatureType))
                Type = newItem.Type;

            if (datItemFields.Contains(DatItemField.FeatureStatus))
                Status = newItem.Status;

            if (datItemFields.Contains(DatItemField.FeatureOverall))
                Overall = newItem.Overall;
        }

        #endregion
    }
}
