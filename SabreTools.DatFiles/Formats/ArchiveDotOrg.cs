using System.Collections.Generic;
using SabreTools.Core;
using SabreTools.DatItems;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents a Archive.org file list
    /// </summary>
    internal sealed class ArchiveDotOrg : SerializableDatFile<Models.ArchiveDotOrg.Files, Serialization.Files.ArchiveDotOrg, Serialization.CrossModel.ArchiveDotOrg>
    {
        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public ArchiveDotOrg(DatFile? datFile)
            : base(datFile)
        {
        }

        /// <inheritdoc/>
        protected override ItemType[] GetSupportedTypes()
        {
            return
            [
                ItemType.Rom,
            ];
        }

        /// <inheritdoc/>
        protected override List<string>? GetMissingRequiredFields(DatItem datItem) => null;
    }
}
