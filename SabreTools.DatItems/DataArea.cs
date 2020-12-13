using System;
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

            // Handle DataArea-specific fields
            if (datItemMappings.Keys.Contains(DatItemField.AreaName))
                Name = datItemMappings[DatItemField.AreaName];

            if (datItemMappings.Keys.Contains(DatItemField.AreaSize))
                Size = Utilities.CleanLong(datItemMappings[DatItemField.AreaSize]);

            if (datItemMappings.Keys.Contains(DatItemField.AreaWidth))
                Width = Utilities.CleanLong(datItemMappings[DatItemField.AreaWidth]);

            if (datItemMappings.Keys.Contains(DatItemField.AreaEndianness))
                Endianness = datItemMappings[DatItemField.AreaEndianness].AsEndianness();
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

        /// <inheritdoc/>
        public override bool PassesFilter(Cleaner cleaner, bool sub = false)
        {
            // Check common fields first
            if (!base.PassesFilter(cleaner, sub))
                return false;

            // Filter on area name
            if (!Filter.PassStringFilter(cleaner.DatItemFilter.AreaName, Name))
                return false;

            // Filter on area size
            if (!Filter.PassLongFilter(cleaner.DatItemFilter.AreaSize, Size))
                return false;

            // Filter on area width
            if (!Filter.PassLongFilter(cleaner.DatItemFilter.AreaWidth, Width))
                return false;

            // Filter on area endianness
            if (cleaner.DatItemFilter.AreaEndianness.MatchesPositive(Endianness.NULL, Endianness) == false)
                return false;
            if (cleaner.DatItemFilter.AreaEndianness.MatchesNegative(Endianness.NULL, Endianness) == true)
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
            if (datItemFields.Contains(DatItemField.AreaName))
                Name = null;

            if (datItemFields.Contains(DatItemField.AreaSize))
                Size = null;

            if (datItemFields.Contains(DatItemField.AreaWidth))
                Width = null;

            if (datItemFields.Contains(DatItemField.AreaEndianness))
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

        /// <inheritdoc/>
        public override void ReplaceFields(
            DatItem item,
            List<DatItemField> datItemFields,
            List<MachineField> machineFields)
        {
            // Replace common fields first
            base.ReplaceFields(item, datItemFields, machineFields);

            // If we don't have a DataArea to replace from, ignore specific fields
            if (item.ItemType != ItemType.DataArea)
                return;

            // Cast for easier access
            DataArea newItem = item as DataArea;

            // Replace the fields
            if (datItemFields.Contains(DatItemField.AreaName))
                Name = newItem.Name;

            if (datItemFields.Contains(DatItemField.AreaSize))
                Size = newItem.Size;

            if (datItemFields.Contains(DatItemField.AreaWidth))
                Width = newItem.Width;

            if (datItemFields.Contains(DatItemField.AreaEndianness))
                Endianness = newItem.Endianness;
        }

        #endregion
    }
}
