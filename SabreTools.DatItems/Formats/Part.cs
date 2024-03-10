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

        [JsonIgnore]
        public bool FeaturesSpecified
        {
            get
            {
                var features = GetFieldValue<PartFeature[]?>(Models.Metadata.Part.FeatureKey);
                return features != null && features.Length > 0;
            }
        }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => GetFieldValue<string>(Models.Metadata.Part.NameKey);

        /// <inheritdoc/>
        public override void SetName(string? name) => SetFieldValue(Models.Metadata.Part.NameKey, name);

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Part object
        /// </summary>
        public Part()
        {
            _internal = new Models.Metadata.Part();
            Machine = new Machine();

            SetName(string.Empty);
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

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.Part ?? [],
            };
        }

        #endregion
    }
}
