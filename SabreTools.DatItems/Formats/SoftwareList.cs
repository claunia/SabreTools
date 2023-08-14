using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SabreTools.Core;
using SabreTools.Core.Tools;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents which SoftwareList(s) is associated with a set
    /// </summary>
    /// <remarks>
    /// TODO: Add new fields to documentation
    /// </remarks>
    [JsonObject("softwarelist"), XmlRoot("softwarelist")]
    public class SoftwareList : DatItem
    {
        #region Fields

        /// <summary>
        /// Tag for the software list
        /// </summary>
        [JsonProperty("tag", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("tag")]
        public string? Tag
        {
            get => _softwareList.ReadString(Models.Internal.SoftwareList.TagKey);
            set => _softwareList[Models.Internal.SoftwareList.TagKey] = value;
        }

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name")]
        [XmlElement("name")]
        public string? Name
        {
            get => _softwareList.ReadString(Models.Internal.SoftwareList.NameKey);
            set => _softwareList[Models.Internal.SoftwareList.NameKey] = value;
        }

        /// <summary>
        /// Status of the softare list according to the machine
        /// </summary>
        [JsonProperty("status", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlElement("status")]
        public SoftwareListStatus Status
        {
            get => _softwareList.ReadString(Models.Internal.SoftwareList.StatusKey).AsSoftwareListStatus();
            set => _softwareList[Models.Internal.SoftwareList.StatusKey] = value.FromSoftwareListStatus();
        }

        [JsonIgnore]
        public bool StatusSpecified { get { return Status != SoftwareListStatus.None; } }

        /// <summary>
        /// Filter to apply to the software list
        /// </summary>
        [JsonProperty("filter", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("filter")]
        public string? Filter
        {
            get => _softwareList.ReadString(Models.Internal.SoftwareList.FilterKey);
            set => _softwareList[Models.Internal.SoftwareList.FilterKey] = value;
        }

        /// <summary>
        /// Internal SoftwareList model
        /// </summary>
        [JsonIgnore]
        private Models.Internal.SoftwareList _softwareList = new();

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty SoftwareList object
        /// </summary>
        public SoftwareList()
        {
            Name = string.Empty;
            ItemType = ItemType.SoftwareList;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new SoftwareList()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine?.Clone() as Machine,
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _softwareList = this._softwareList?.Clone() as Models.Internal.SoftwareList ?? new Models.Internal.SoftwareList(),
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem? other)
        {
            // If we don't have a Adjuster, return false
            if (ItemType != other?.ItemType || other is not SoftwareList otherInternal)
                return false;

            // Compare the internal models
            return _softwareList.EqualTo(otherInternal._softwareList);
        }

        #endregion
    }
}
