using System.Collections.Generic;
using SabreTools.DatItems;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents an AttractMode DAT
    /// </summary>
    internal sealed class AttractMode : SerializableDatFile<Models.AttractMode.MetadataFile, Serialization.Deserializers.AttractMode, Serialization.Serializers.AttractMode, Serialization.CrossModel.AttractMode>
    {
        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public AttractMode(DatFile? datFile)
            : base(datFile)
        {
        }

        /// <inheritdoc/>
        protected override ItemType[] GetSupportedTypes()
        {
            return
            [
                ItemType.Rom
            ];
        }

        /// <inheritdoc/>
        protected override List<string>? GetMissingRequiredFields(DatItem datItem)
        {
            var missingFields = new List<string>();

            // Check item name
            if (string.IsNullOrEmpty(datItem.GetName()))
                missingFields.Add(Models.Metadata.Rom.NameKey);

            return missingFields;
        }
    }
}
