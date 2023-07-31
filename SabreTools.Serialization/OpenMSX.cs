using System.IO;

namespace SabreTools.Serialization
{
    /// <summary>
    /// XML deserializer for OpenMSX software database files
    /// </summary>
    public class OpenMSX : XmlSerializer<Models.OpenMSX.SoftwareDb>
    {
        /// <summary>
        /// name field for DOCTYPE
        /// </summary>
        public const string? DocTypeName = "softwaredb";

        /// <summary>
        /// pubid field for DOCTYPE
        /// </summary>
        public const string? DocTypePubId = null;

        /// <summary>
        /// sysid field for DOCTYPE
        /// </summary>
        public const string? DocTypeSysId = "softwaredb1.dtd";

        /// <summary>
        /// subset field for DOCTYPE
        /// </summary>
        public const string? DocTypeSubset = null;

        /// <inheritdoc cref="SerializeToFile(Models.OpenMSX.SoftwareDb, string, string?, string?, string?, string?)" />
        public static bool SerializeToFileWithDocType(Models.OpenMSX.SoftwareDb obj, string path)
            => SerializeToFile(obj, path, DocTypeName, DocTypePubId, DocTypeSysId, DocTypeSysId);

        /// <inheritdoc cref="SerializeToStream(Models.OpenMSX.SoftwareDb, string?, string?, string?, string?)" />
        public static Stream? SerializeToStreamWithDocType(Models.OpenMSX.SoftwareDb obj, string path)
            => SerializeToStream(obj, DocTypeName, DocTypePubId, DocTypeSysId, DocTypeSysId);
    }
}