using System.IO;

namespace SabreTools.Serialization
{
    /// <summary>
    /// XML deserializer for Logiqx-derived metadata files
    /// </summary>
    public class Logiqx : XmlSerializer<Models.Logiqx.Datafile>
    {
        /// <summary>
        /// name field for DOCTYPE
        /// </summary>
        public const string? DocTypeName = "datafile";

        /// <summary>
        /// pubid field for DOCTYPE
        /// </summary>
        public const string? DocTypePubId = "-//Logiqx//DTD ROM Management Datafile//EN";

        /// <summary>
        /// sysid field for DOCTYPE
        /// </summary>
        public const string? DocTypeSysId = "http://www.logiqx.com/Dats/datafile.dtd";

        /// <summary>
        /// subset field for DOCTYPE
        /// </summary>
        public const string? DocTypeSubset = null;

        /// <inheritdoc cref="SerializeToFile(Models.Logiqx.Datafile, string, string?, string?, string?, string?)" />
        public static bool SerializeToFileWithDocType(Models.Logiqx.Datafile obj, string path)
            => SerializeToFile(obj, path, DocTypeName, DocTypePubId, DocTypeSysId, DocTypeSysId);

        /// <inheritdoc cref="SerializeToStream(Models.Logiqx.Datafile, string?, string?, string?, string?)" />
        public static Stream? SerializeToStreamWithDocType(Models.Logiqx.Datafile obj, string path)
            => SerializeToStream(obj, DocTypeName, DocTypePubId, DocTypeSysId, DocTypeSysId);
    }
}