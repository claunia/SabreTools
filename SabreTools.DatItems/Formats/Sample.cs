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
        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => GetFieldValue<string>(Models.Metadata.Sample.NameKey);

        /// <inheritdoc/>
        public override void SetName(string? name) => SetFieldValue(Models.Metadata.Sample.NameKey, name);

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Sample object
        /// </summary>
        public Sample()
        {
            _internal = new Models.Metadata.Sample();
            Machine = new Machine();

            SetName(string.Empty);
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

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.Sample ?? [],
            };
        }

        #endregion
    }
}
