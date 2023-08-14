using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents one ListXML dipvalue
    /// </summary>
    [JsonObject("dipvalue"), XmlRoot("dipvalue")]
    public class DipValue : DatItem
    {
        #region Fields

        /// <summary>
        /// Setting name
        /// </summary>
        [JsonProperty("name"), XmlElement("name")]
        public string? Name
        {
            get => _dipValue.ReadString(Models.Internal.DipValue.NameKey);
            set => _dipValue[Models.Internal.DipValue.NameKey] = value;
        }

        /// <summary>
        /// Setting value
        /// </summary>
        [JsonProperty("value", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("value")]
        public string? Value
        {
            get => _dipValue.ReadString(Models.Internal.DipValue.ValueKey);
            set => _dipValue[Models.Internal.DipValue.ValueKey] = value;
        }

        /// <summary>
        /// Determines if the setting is default or not
        /// </summary>
        [JsonProperty("default", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("default")]
        public bool? Default
        {
            get => _dipValue.ReadBool(Models.Internal.DipValue.DefaultKey);
            set => _dipValue[Models.Internal.DipValue.DefaultKey] = value;
        }

        [JsonIgnore]
        public bool DefaultSpecified { get { return Default != null; } }

        /// <summary>
        /// List of conditions on the setting
        /// </summary>
        [JsonProperty("conditions", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("conditions")]
        public List<Condition>? Conditions
        {
            get => _dipValue.Read<Condition[]>(Models.Internal.DipValue.ConditionKey)?.ToList();
            set => _dipValue[Models.Internal.DipValue.ConditionKey] = value?.ToArray();
        }

        [JsonIgnore]
        public bool ConditionsSpecified { get { return Conditions != null && Conditions.Count > 0; } }

        /// <summary>
        /// Internal DipValue model
        /// </summary>
        [JsonIgnore]
        private Models.Internal.DipValue _dipValue = new();

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty DipValue object
        /// </summary>
        public DipValue()
        {
            Name = string.Empty;
            ItemType = ItemType.DipValue;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new DipValue()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine?.Clone() as Machine,
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _dipValue = this._dipValue?.Clone() as Models.Internal.DipValue ?? new Models.Internal.DipValue(),
            };
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If we don't have a DipValue, return false
            if (ItemType != other?.ItemType || other is not DipValue otherInternal)
                return false;

            // Compare the internal models
            return _dipValue.EqualTo(otherInternal._dipValue);
        }

        #endregion
    }
}
