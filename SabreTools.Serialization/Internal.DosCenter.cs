namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for DosCenter models to internal structure
    /// </summary>
    public partial class Internal
    {
        #region Serialize

        /// <summary>
        /// Convert from <cref="Models.DosCenter.File"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Rom ConvertFromDosCenter(Models.DosCenter.File item)
        {
            var rom = new Models.Internal.Rom
            {
                [Models.Internal.Rom.NameKey] = item.Name,
                [Models.Internal.Rom.SizeKey] = item.Size,
                [Models.Internal.Rom.CRCKey] = item.CRC,
                [Models.Internal.Rom.DateKey] = item.Date,
            };
            return rom;
        }

        #endregion

        #region Deserialize

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.DosCenter.File"/>
        /// </summary>
        public static Models.DosCenter.File ConvertToDosCenter(Models.Internal.Rom item)
        {
            var file = new Models.DosCenter.File
            {
                Name = item.ReadString(Models.Internal.Rom.NameKey),
                Size = item.ReadString(Models.Internal.Rom.SizeKey),
                CRC = item.ReadString(Models.Internal.Rom.CRCKey),
                Date = item.ReadString(Models.Internal.Rom.DateKey),
            };
            return file;
        }

        #endregion
    }
}