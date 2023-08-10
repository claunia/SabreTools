namespace SabreTools.Serialization
{
    /// <summary>
    /// XML deserializer for OpenMSX software database files
    /// </summary>
    public partial class OpenMSX : XmlSerializer<Models.OpenMSX.SoftwareDb>
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
    }
}