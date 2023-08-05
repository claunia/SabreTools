namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for AttractMode models to internal structure
    /// </summary>
    public partial class Internal
    {
        #region Serialize

        /// <summary>
        /// Convert from <cref="Models.AttractMode.Row"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Rom ConvertFromAttractMode(Models.AttractMode.Row item)
        {
            var rom = new Models.Internal.Rom
            {
                [Models.Internal.Rom.NameKey] = item.Title,
                [Models.Internal.Rom.AltRomnameKey] = item.AltRomname,
                [Models.Internal.Rom.AltTitleKey] = item.AltTitle,
                [Models.Internal.Rom.FileIsAvailableKey] = item.FileIsAvailable,
            };
            return rom;
        }

        #endregion

        #region Deserialize

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.AttractMode.Row"/>
        /// </summary>
        public static Models.AttractMode.Row ConvertToAttractMode(Models.Internal.Rom item)
        {
            var row = new Models.AttractMode.Row
            {
                Title = item.ReadString(Models.Internal.Rom.NameKey),
                AltRomname = item.ReadString(Models.Internal.Rom.AltRomnameKey),
                AltTitle = item.ReadString(Models.Internal.Rom.AltTitleKey),
                FileIsAvailable = item.ReadString(Models.Internal.Rom.FileIsAvailableKey),
            };
            return row;
        }

        #endregion
    }
}