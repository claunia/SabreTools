using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// SoftwareList part information
    /// </summary>
    /// <remarks>One Part can contain multiple PartFeature, DataArea, DiskArea, and DipSwitch items</remarks>
    [JsonObject("part"), XmlRoot("part")]
    public class Part : DatItem
    {
        #region Fields

        [JsonProperty("name"), XmlElement("name")]
        public string? Name
        {
            get => _part.ReadString(Models.Internal.Part.NameKey);
            set => _part[Models.Internal.Part.NameKey] = value;
        }

        [JsonProperty("interface"), XmlElement("interface")]
        public string? Interface
        {
            get => _part.ReadString(Models.Internal.Part.InterfaceKey);
            set => _part[Models.Internal.Part.InterfaceKey] = value;
        }
    
        [JsonProperty("features", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("features")]
        public List<PartFeature>? Features
        {
            get => _part.Read<PartFeature[]>(Models.Internal.Part.FeatureKey)?.ToList();
            set => _part[Models.Internal.Part.FeatureKey] = value?.ToArray();
        }

        [JsonIgnore]
        public bool FeaturesSpecified { get { return Features != null && Features.Count > 0; } }

        /// <summary>
        /// Internal Part model
        /// </summary>
        [JsonIgnore]
        private Models.Internal.Part _part = new();

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

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

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Part()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine?.Clone() as Machine,
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _part = this._part?.Clone() as Models.Internal.Part ?? new Models.Internal.Part(),
            };
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If we don't have a Part, return false
            if (ItemType != other?.ItemType || other is not Part otherInternal)
                return false;

            // Compare the internal models
            return _part.EqualTo(otherInternal._part);
        }

        #endregion
    }
}
