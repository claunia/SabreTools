namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for RomCenter models to internal structure
    /// </summary>
    public partial class Internal
    {
        #region Serialize

        /// <summary>
        /// Convert from <cref="Models.RomCenter.Rom"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Rom ConvertFromRomCenter(Models.RomCenter.Rom item)
        {
            var rom = new Models.Internal.Rom
            {
                [Models.Internal.Rom.NameKey] = item.RomName,
                [Models.Internal.Rom.CRCKey] = item.RomCRC,
                [Models.Internal.Rom.SizeKey] = item.RomSize,
                [Models.Internal.Rom.MergeKey] = item.MergeName,
            };
            return rom;
        }

        #endregion

        #region Deserialize

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.RomCenter.Rom"/>
        /// </summary>
        public static Models.RomCenter.Rom ConvertToRomCenter(Models.Internal.Rom item)
        {
            var row = new Models.RomCenter.Rom
            {
                RomName = item.ReadString(Models.Internal.Rom.NameKey),
                RomCRC = item.ReadString(Models.Internal.Rom.CRCKey),
                RomSize = item.ReadString(Models.Internal.Rom.SizeKey),
                MergeName = item.ReadString(Models.Internal.Rom.MergeKey),
            };
            return row;
        }

        #endregion
    }
}