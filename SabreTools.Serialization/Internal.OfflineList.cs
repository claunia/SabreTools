namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for OfflineList models to internal structure
    /// </summary>
    public partial class Internal
    {
        #region Serialize

        /// <summary>
        /// Convert from <cref="Models.OfflineList.FileRomCRC"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Rom ConvertFromOfflineList(Models.OfflineList.FileRomCRC item)
        {
            var rom = new Models.Internal.Rom
            {
                [Models.Internal.Rom.ExtensionKey] = item.Extension,
                [Models.Internal.Rom.CRCKey] = item.Content,
            };
            return rom;
        }

        #endregion

        #region Deserialize

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.OfflineList.FileRomCRC"/>
        /// </summary>
        public static Models.OfflineList.FileRomCRC ConvertToOfflineList(Models.Internal.Rom item)
        {
            var fileRomCRC = new Models.OfflineList.FileRomCRC
            {
                Extension = item.ReadString(Models.Internal.Rom.ExtensionKey),
                Content = item.ReadString(Models.Internal.Rom.CRCKey),
            };
            return fileRomCRC;
        }

        #endregion
    }
}