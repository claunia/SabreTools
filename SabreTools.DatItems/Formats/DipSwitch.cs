using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
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
        [JsonProperty("name"), XmlElement("name")]
        public string? Name
        {
            get => _dipSwitch.ReadString(Models.Internal.DipSwitch.NameKey);
            set => _dipSwitch[Models.Internal.DipSwitch.NameKey] = value;
        }

        /// <summary>
        /// Tag associated with the dipswitch
        /// </summary>
        [JsonProperty("tag", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("tag")]
        public string? Tag
        {
            get => _dipSwitch.ReadString(Models.Internal.DipSwitch.TagKey);
            set => _dipSwitch[Models.Internal.DipSwitch.TagKey] = value;
        }

        /// <summary>
        /// Mask associated with the dipswitch
        /// </summary>
        [JsonProperty("mask", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("mask")]
        public string? Mask
        {
            get => _dipSwitch.ReadString(Models.Internal.DipSwitch.MaskKey);
            set => _dipSwitch[Models.Internal.DipSwitch.MaskKey] = value;
        }

        /// <summary>
        /// Conditions associated with the dipswitch
        /// </summary>
        [JsonProperty("conditions", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("conditions")]
        public List<Condition>? Conditions
        {
            get => _dipSwitch.Read<Condition[]>(Models.Internal.DipSwitch.ConditionKey)?.ToList();
            set => _dipSwitch[Models.Internal.DipSwitch.ConditionKey] = value?.ToArray();
        }

        [JsonIgnore]
        public bool ConditionsSpecified { get { return Conditions != null && Conditions.Count > 0; } }

        /// <summary>
        /// Locations associated with the dipswitch
        /// </summary>
        [JsonProperty("locations", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("locations")]
        public List<DipLocation>? Locations
        {
            get => _dipSwitch.Read<DipLocation[]>(Models.Internal.DipSwitch.DipLocationKey)?.ToList();
            set => _dipSwitch[Models.Internal.DipSwitch.DipLocationKey] = value?.ToArray();
        }

        [JsonIgnore]
        public bool LocationsSpecified { get { return Locations != null && Locations.Count > 0; } }

        /// <summary>
        /// Settings associated with the dipswitch
        /// </summary>
        [JsonProperty("values", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("values")]
        public List<DipValue>? Values
        {
            get => _dipSwitch.Read<DipValue[]>(Models.Internal.DipSwitch.DipValueKey)?.ToList();
            set => _dipSwitch[Models.Internal.DipSwitch.DipValueKey] = value?.ToArray();
        }

        [JsonIgnore]
        public bool ValuesSpecified { get { return Values != null && Values.Count > 0; } }

        #endregion

        #region SoftwareList

        /// <summary>
        /// Original hardware part associated with the item
        /// </summary>
        /// <remarks>This is inverted from the internal model</remarks>
        [JsonProperty("part", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("part")]
        public Part? Part { get; set; }

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

        /// <summary>
        /// Internal DipSwitch model
        /// </summary>
        [JsonIgnore]
        private Models.Internal.DipSwitch _dipSwitch = new();

        #endregion // Fields

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

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

        /// <inheritdoc/>
        public override object Clone()
        {
            return new DipSwitch()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine?.Clone() as Machine,
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _dipSwitch = this._dipSwitch?.Clone() as Models.Internal.DipSwitch ?? new Models.Internal.DipSwitch(),

                Part = this.Part,
            };
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If we don't have a DipSwitch, return false
            if (ItemType != other?.ItemType || other is not DipSwitch otherInternal)
                return false;

            // Compare the internal models
            return _dipSwitch.EqualTo(otherInternal._dipSwitch);
        }

        #endregion
    }
}
