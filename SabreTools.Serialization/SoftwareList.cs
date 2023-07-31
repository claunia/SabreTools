using System.IO;

namespace SabreTools.Serialization
{
    /// <summary>
    /// XML deserializer for MAME softwarelist files
    /// </summary>
    public class SoftawreList : XmlSerializer<Models.SoftwareList.SoftwareList>
    {
        /// <summary>
        /// name field for DOCTYPE
        /// </summary>
        public const string? DocTypeName = "softwarelist";

        /// <summary>
        /// pubid field for DOCTYPE
        /// </summary>
        public const string? DocTypePubId = null;

        /// <summary>
        /// sysid field for DOCTYPE
        /// </summary>
        public const string? DocTypeSysId = "softwarelist.dtd";

        /// <summary>
        /// subset field for DOCTYPE
        /// </summary>
        public const string? DocTypeSubset = null;

        /// <inheritdoc cref="SerializeToFile(Models.SoftwareList.SoftwareList, string, string?, string?, string?, string?)" />
        public static bool SerializeToFileWithDocType(Models.SoftwareList.SoftwareList obj, string path)
            => SerializeToFile(obj, path, DocTypeName, DocTypePubId, DocTypeSysId, DocTypeSysId);

        /// <inheritdoc cref="SerializeToStream(Models.SoftwareList.SoftwareList, string?, string?, string?, string?)" />
        public static Stream? SerializeToStreamWithDocType(Models.SoftwareList.SoftwareList obj, string path)
            => SerializeToStream(obj, DocTypeName, DocTypePubId, DocTypeSysId, DocTypeSysId);
    }
}