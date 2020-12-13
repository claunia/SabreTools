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
    /// Represents the a driver of the machine
    /// </summary>
    [JsonObject("driver"), XmlRoot("driver")]
    public class Driver : DatItem
    {
        #region Fields

        /// <summary>
        /// Overall driver status
        /// </summary>
        [JsonProperty("status", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlElement("status")]
        public SupportStatus Status { get; set; }

        [JsonIgnore]
        public bool StatusSpecified { get { return Status != SupportStatus.NULL; } }

        /// <summary>
        /// Driver emulation status
        /// </summary>
        [JsonProperty("emulation", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlElement("emulation")]
        public SupportStatus Emulation { get; set; }

        [JsonIgnore]
        public bool EmulationSpecified { get { return Emulation != SupportStatus.NULL; ; } }

        /// <summary>
        /// Cocktail orientation status
        /// </summary>
        [JsonProperty("cocktail", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlElement("cocktail")]
        public SupportStatus Cocktail { get; set; }

        [JsonIgnore]
        public bool CocktailSpecified { get { return Cocktail != SupportStatus.NULL; ; } }

        /// <summary>
        /// Save state support status
        /// </summary>
        [JsonProperty("savestate", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlElement("savestate")]
        public Supported SaveState { get; set; }

        [JsonIgnore]
        public bool SaveStateSpecified { get { return SaveState != Supported.NULL; } }

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
            if (datItemMappings.Keys.Contains(DatItemField.SupportStatus))
                Status = datItemMappings[DatItemField.SupportStatus].AsSupportStatus();

            if (datItemMappings.Keys.Contains(DatItemField.EmulationStatus))
                Emulation = datItemMappings[DatItemField.EmulationStatus].AsSupportStatus();

            if (datItemMappings.Keys.Contains(DatItemField.CocktailStatus))
                Cocktail = datItemMappings[DatItemField.CocktailStatus].AsSupportStatus();

            if (datItemMappings.Keys.Contains(DatItemField.SaveStateStatus))
                SaveState = datItemMappings[DatItemField.SaveStateStatus].AsSupported();
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Driver object
        /// </summary>
        public Driver()
        {
            ItemType = ItemType.Driver;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new Driver()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Status = this.Status,
                Emulation = this.Emulation,
                Cocktail = this.Cocktail,
                SaveState = this.SaveState,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a Driver, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Driver
            Driver newOther = other as Driver;

            // If the Feature information matches
            return (Status == newOther.Status
                && Emulation == newOther.Emulation
                && Cocktail == newOther.Cocktail
                && SaveState == newOther.SaveState);
        }

        #endregion

        #region Filtering

        /// <inheritdoc/>
        public override bool PassesFilter(Cleaner cleaner, bool sub = false)
        {
            // Check common fields first
            if (!base.PassesFilter(cleaner, sub))
                return false;

            // Filter on status
            if (cleaner.DatItemFilter.SupportStatus.MatchesPositive(SupportStatus.NULL, Status) == false)
                return false;
            if (cleaner.DatItemFilter.SupportStatus.MatchesNegative(SupportStatus.NULL, Status) == true)
                return false;

            // Filter on emulation
            if (cleaner.DatItemFilter.EmulationStatus.MatchesPositive(SupportStatus.NULL, Emulation) == false)
                return false;
            if (cleaner.DatItemFilter.EmulationStatus.MatchesNegative(SupportStatus.NULL, Emulation) == true)
                return false;

            // Filter on cocktail
            if (cleaner.DatItemFilter.CocktailStatus.MatchesPositive(SupportStatus.NULL, Cocktail) == false)
                return false;
            if (cleaner.DatItemFilter.CocktailStatus.MatchesNegative(SupportStatus.NULL, Cocktail) == true)
                return false;

            // Filter on savestate
            if (cleaner.DatItemFilter.SaveStateStatus.MatchesPositive(Supported.NULL, SaveState) == false)
                return false;
            if (cleaner.DatItemFilter.SaveStateStatus.MatchesNegative(Supported.NULL, SaveState) == true)
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
            if (datItemFields.Contains(DatItemField.SupportStatus))
                Status = SupportStatus.NULL;

            if (datItemFields.Contains(DatItemField.EmulationStatus))
                Emulation = SupportStatus.NULL;

            if (datItemFields.Contains(DatItemField.CocktailStatus))
                Cocktail = SupportStatus.NULL;

            if (datItemFields.Contains(DatItemField.SaveStateStatus))
                SaveState = Supported.NULL;
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

            // If we don't have a Driver to replace from, ignore specific fields
            if (item.ItemType != ItemType.Driver)
                return;

            // Cast for easier access
            Driver newItem = item as Driver;

            // Replace the fields
            if (datItemFields.Contains(DatItemField.SupportStatus))
                Status = newItem.Status;

            if (datItemFields.Contains(DatItemField.EmulationStatus))
                Emulation = newItem.Emulation;

            if (datItemFields.Contains(DatItemField.CocktailStatus))
                Cocktail = newItem.Cocktail;

            if (datItemFields.Contains(DatItemField.SaveStateStatus))
                SaveState = newItem.SaveState;
        }

        #endregion
    }
}
