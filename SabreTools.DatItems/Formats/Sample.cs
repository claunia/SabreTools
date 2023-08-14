using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents a (usually WAV-formatted) sample to be included for use in the set
    /// </summary>
    [JsonObject("sample"), XmlRoot("sample")]
    public class Sample : DatItem
    {
        #region Fields

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name"), XmlElement("name")]
        public string? Name
        {
            get => _sample.ReadString(Models.Internal.Sample.NameKey);
            set => _sample[Models.Internal.Sample.NameKey] = value;
        }

        /// <summary>
        /// Internal Sample model
        /// </summary>
        [JsonIgnore]
        private Models.Internal.Sample _sample = new();

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Sample object
        /// </summary>
        public Sample()
        {
            Name = string.Empty;
            ItemType = ItemType.Sample;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new Sample()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine?.Clone() as Machine,
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _sample = this._sample?.Clone() as Models.Internal.Sample ?? new Models.Internal.Sample(),
            };
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If we don't have a Sample, return false
            if (ItemType != other?.ItemType || other is not Sample otherInternal)
                return false;

            // Compare the internal models
            return _sample.EqualTo(otherInternal._sample);
        }

        #endregion
    }
}
