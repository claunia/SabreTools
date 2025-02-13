using System.Collections.Generic;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing and writing of a DosCenter DAT
    /// </summary>
    public sealed class DosCenter : SerializableDatFile<Models.DosCenter.MetadataFile, Serialization.Deserializers.DosCenter, Serialization.Serializers.DosCenter, Serialization.CrossModel.DosCenter>
    {
        /// <inheritdoc/>
        public override ItemType[] SupportedTypes
            => [
                ItemType.Rom,
            ];

        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public DosCenter(DatFile? datFile) : base(datFile)
        {
        }

        /// <inheritdoc/>
        protected internal override List<string>? GetMissingRequiredFields(DatItem datItem)
        {
            List<string> missingFields = [];

            // Check item name
            if (string.IsNullOrEmpty(datItem.GetName()))
                missingFields.Add(Models.Metadata.Rom.NameKey);

            switch (datItem)
            {
                case Rom rom:
                    if (rom.GetInt64FieldValue(Models.Metadata.Rom.SizeKey) == null || rom.GetInt64FieldValue(Models.Metadata.Rom.SizeKey) < 0)
                        missingFields.Add(Models.Metadata.Rom.SizeKey);
                    // if (string.IsNullOrEmpty(rom.Date))
                    //     missingFields.Add(Models.Metadata.Rom.DateKey);
                    if (string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.CRCKey)))
                        missingFields.Add(Models.Metadata.Rom.CRCKey);
                    // if (string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA1Key)))
                    //     missingFields.Add(Models.Metadata.Rom.SHA1Key);
                    break;
            }

            return missingFields;
        }
    }
}
