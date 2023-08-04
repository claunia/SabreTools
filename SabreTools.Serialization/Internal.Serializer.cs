namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for all relevant models to internal structure
    /// </summary>
    public partial class Internal
    {
        #region ArchiveDotOrg

        /// <summary>
        /// Convert from <cref="Models.ArchiveDotOrg.File"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Rom ConvertFromArchiveDotOrg(Models.ArchiveDotOrg.File item)
        {
            var rom = new Models.Internal.Rom
            {
                [Models.Internal.Rom.NameKey] = item.Name,
                [Models.Internal.Rom.SourceKey] = item.Source,
                [Models.Internal.Rom.BitTorrentMagnetHashKey] = item.BitTorrentMagnetHash,
                [Models.Internal.Rom.LastModifiedTimeKey] = item.LastModifiedTime,
                [Models.Internal.Rom.SizeKey] = item.Size,
                [Models.Internal.Rom.MD5Key] = item.MD5,
                [Models.Internal.Rom.CRCKey] = item.CRC32,
                [Models.Internal.Rom.SHA1Key] = item.SHA1,
                [Models.Internal.Rom.FileCountKey] = item.FileCount,
                [Models.Internal.Rom.FormatKey] = item.Format,
                [Models.Internal.Rom.OriginalKey] = item.Original,
                [Models.Internal.Rom.SummationKey] = item.Summation,
                [Models.Internal.Rom.MatrixNumberKey] = item.MatrixNumber,
                [Models.Internal.Rom.CollectionCatalogNumberKey] = item.CollectionCatalogNumber,
                [Models.Internal.Rom.PublisherKey] = item.Publisher,
                [Models.Internal.Rom.CommentKey] = item.Comment,

                [Models.Internal.Rom.ASRDetectedLangKey] = item.ASRDetectedLang,
                [Models.Internal.Rom.ASRDetectedLangConfKey] = item.ASRDetectedLangConf,
                [Models.Internal.Rom.ASRTranscribedLangKey] = item.ASRTranscribedLang,
                [Models.Internal.Rom.WhisperASRModuleVersionKey] = item.WhisperASRModuleVersion,
                [Models.Internal.Rom.WhisperModelHashKey] = item.WhisperModelHash,
                [Models.Internal.Rom.WhisperModelNameKey] = item.WhisperModelName,
                [Models.Internal.Rom.WhisperVersionKey] = item.WhisperVersion,

                [Models.Internal.Rom.ClothCoverDetectionModuleVersionKey] = item.ClothCoverDetectionModuleVersion,
                [Models.Internal.Rom.hOCRCharToWordhOCRVersionKey] = item.hOCRCharToWordhOCRVersion,
                [Models.Internal.Rom.hOCRCharToWordModuleVersionKey] = item.hOCRCharToWordModuleVersion,
                [Models.Internal.Rom.hOCRFtsTexthOCRVersionKey] = item.hOCRFtsTexthOCRVersion,
                [Models.Internal.Rom.hOCRFtsTextModuleVersionKey] = item.hOCRFtsTextModuleVersion,
                [Models.Internal.Rom.hOCRPageIndexhOCRVersionKey] = item.hOCRPageIndexhOCRVersion,
                [Models.Internal.Rom.hOCRPageIndexModuleVersionKey] = item.hOCRPageIndexModuleVersion,
                [Models.Internal.Rom.TesseractOCRKey] = item.TesseractOCR,
                [Models.Internal.Rom.TesseractOCRConvertedKey] = item.TesseractOCRConverted,
                [Models.Internal.Rom.TesseractOCRDetectedLangKey] = item.TesseractOCRDetectedLang,
                [Models.Internal.Rom.TesseractOCRDetectedLangConfKey] = item.TesseractOCRDetectedLangConf,
                [Models.Internal.Rom.TesseractOCRDetectedScriptKey] = item.TesseractOCRDetectedScript,
                [Models.Internal.Rom.TesseractOCRDetectedScriptConfKey] = item.TesseractOCRDetectedScriptConf,
                [Models.Internal.Rom.TesseractOCRModuleVersionKey] = item.TesseractOCRModuleVersion,
                [Models.Internal.Rom.TesseractOCRParametersKey] = item.TesseractOCRParameters,
                [Models.Internal.Rom.PDFModuleVersionKey] = item.PDFModuleVersion,
                [Models.Internal.Rom.WordConfidenceInterval0To10Key] = item.WordConfidenceInterval0To10,
                [Models.Internal.Rom.WordConfidenceInterval11To20Key] = item.WordConfidenceInterval11To20,
                [Models.Internal.Rom.WordConfidenceInterval21To30Key] = item.WordConfidenceInterval21To30,
                [Models.Internal.Rom.WordConfidenceInterval31To40Key] = item.WordConfidenceInterval31To40,
                [Models.Internal.Rom.WordConfidenceInterval41To50Key] = item.WordConfidenceInterval41To50,
                [Models.Internal.Rom.WordConfidenceInterval51To60Key] = item.WordConfidenceInterval51To60,
                [Models.Internal.Rom.WordConfidenceInterval61To70Key] = item.WordConfidenceInterval61To70,
                [Models.Internal.Rom.WordConfidenceInterval71To80Key] = item.WordConfidenceInterval71To80,
                [Models.Internal.Rom.WordConfidenceInterval81To90Key] = item.WordConfidenceInterval81To90,
                [Models.Internal.Rom.WordConfidenceInterval91To100Key] = item.WordConfidenceInterval91To100,

                [Models.Internal.Rom.AlbumKey] = item.Album,
                [Models.Internal.Rom.ArtistKey] = item.Artist,
                [Models.Internal.Rom.BitrateKey] = item.Bitrate,
                [Models.Internal.Rom.CreatorKey] = item.Creator,
                [Models.Internal.Rom.HeightKey] = item.Height,
                [Models.Internal.Rom.LengthKey] = item.Length,
                [Models.Internal.Rom.PreviewImageKey] = item.PreviewImage,
                [Models.Internal.Rom.RotationKey] = item.Rotation,
                [Models.Internal.Rom.TitleKey] = item.Title,
                [Models.Internal.Rom.TrackKey] = item.Track,
                [Models.Internal.Rom.WidthKey] = item.Width,
            };
            return rom;
        }

        #endregion

        #region AttractMode

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
            };
            return rom;
        }

        #endregion

        #region ClrMamePro

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Archive"/> to <cref="Models.Internal.Archive"/>
        /// </summary>
        public static Models.Internal.Archive ConvertFromClrMamePro(Models.ClrMamePro.Archive item)
        {
            var archive = new Models.Internal.Archive
            {
                [Models.Internal.Archive.NameKey] = item.Name,
            };
            return archive;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.BiosSet"/> to <cref="Models.Internal.BiosSet"/>
        /// </summary>
        public static Models.Internal.BiosSet ConvertFromClrMamePro(Models.ClrMamePro.BiosSet item)
        {
            var biosset = new Models.Internal.BiosSet
            {
                [Models.Internal.BiosSet.NameKey] = item.Name,
                [Models.Internal.BiosSet.DescriptionKey] = item.Description,
                [Models.Internal.BiosSet.DefaultKey] = item.Default,
            };
            return biosset;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Chip"/> to <cref="Models.Internal.Chip"/>
        /// </summary>
        public static Models.Internal.Chip ConvertFromClrMamePro(Models.ClrMamePro.Chip item)
        {
            var chip = new Models.Internal.Chip
            {
                [Models.Internal.Chip.ChipTypeKey] = item.Type,
                [Models.Internal.Chip.NameKey] = item.Name,
                [Models.Internal.Chip.FlagsKey] = item.Flags,
                [Models.Internal.Chip.ClockKey] = item.Clock,
            };
            return chip;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.DipSwitch"/> to <cref="Models.Internal.DipSwitch"/>
        /// </summary>
        public static Models.Internal.DipSwitch ConvertFromClrMamePro(Models.ClrMamePro.DipSwitch item)
        {
            var dipswitch = new Models.Internal.DipSwitch
            {
                [Models.Internal.DipSwitch.NameKey] = item.Name,
                [Models.Internal.DipSwitch.EntryKey] = item.Entry,
                [Models.Internal.DipSwitch.DefaultKey] = item.Default,
            };
            return dipswitch;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Disk"/> to <cref="Models.Internal.Disk"/>
        /// </summary>
        public static Models.Internal.Disk ConvertFromClrMamePro(Models.ClrMamePro.Disk item)
        {
            var disk = new Models.Internal.Disk
            {
                [Models.Internal.Disk.NameKey] = item.Name,
                [Models.Internal.Disk.MD5Key] = item.MD5,
                [Models.Internal.Disk.SHA1Key] = item.SHA1,
                [Models.Internal.Disk.MergeKey] = item.Merge,
                [Models.Internal.Disk.StatusKey] = item.Status,
                [Models.Internal.Disk.FlagsKey] = item.Flags,
            };
            return disk;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Driver"/> to <cref="Models.Internal.Driver"/>
        /// </summary>
        public static Models.Internal.Driver ConvertFromClrMamePro(Models.ClrMamePro.Driver item)
        {
            var driver = new Models.Internal.Driver
            {
                [Models.Internal.Driver.StatusKey] = item.Status,
                [Models.Internal.Driver.ColorKey] = item.Color,
                [Models.Internal.Driver.SoundKey] = item.Sound,
                [Models.Internal.Driver.PaletteSizeKey] = item.PaletteSize,
                [Models.Internal.Driver.BlitKey] = item.Blit,
            };
            return driver;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Input"/> to <cref="Models.Internal.Input"/>
        /// </summary>
        public static Models.Internal.Input ConvertFromClrMamePro(Models.ClrMamePro.Input item)
        {
            var input = new Models.Internal.Input
            {
                [Models.Internal.Input.PlayersKey] = item.Players,
                [Models.Internal.Input.ControlKey] = item.Control,
                [Models.Internal.Input.ButtonsKey] = item.Buttons,
                [Models.Internal.Input.CoinsKey] = item.Coins,
                [Models.Internal.Input.TiltKey] = item.Tilt,
                [Models.Internal.Input.ServiceKey] = item.Service,
            };
            return input;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Media"/> to <cref="Models.Internal.Media"/>
        /// </summary>
        public static Models.Internal.Media ConvertFromClrMamePro(Models.ClrMamePro.Media item)
        {
            var media = new Models.Internal.Media
            {
                [Models.Internal.Media.NameKey] = item.Name,
                [Models.Internal.Media.MD5Key] = item.MD5,
                [Models.Internal.Media.SHA1Key] = item.SHA1,
                [Models.Internal.Media.SHA256Key] = item.SHA256,
                [Models.Internal.Media.SpamSumKey] = item.SpamSum,
            };
            return media;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Release"/> to <cref="Models.Internal.Release"/>
        /// </summary>
        public static Models.Internal.Release ConvertFromClrMamePro(Models.ClrMamePro.Release item)
        {
            var release = new Models.Internal.Release
            {
                [Models.Internal.Release.NameKey] = item.Name,
                [Models.Internal.Release.RegionKey] = item.Region,
                [Models.Internal.Release.LanguageKey] = item.Language,
                [Models.Internal.Release.DateKey] = item.Date,
                [Models.Internal.Release.DefaultKey] = item.Default,
            };
            return release;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Rom"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Rom ConvertFromClrMamePro(Models.ClrMamePro.Rom item)
        {
            var rom = new Models.Internal.Rom
            {
                [Models.Internal.Rom.NameKey] = item.Name,
                [Models.Internal.Rom.SizeKey] = item.Size,
                [Models.Internal.Rom.CRCKey] = item.CRC,
                [Models.Internal.Rom.MD5Key] = item.MD5,
                [Models.Internal.Rom.SHA1Key] = item.SHA1,
                [Models.Internal.Rom.SHA256Key] = item.SHA256,
                [Models.Internal.Rom.SHA384Key] = item.SHA384,
                [Models.Internal.Rom.SHA512Key] = item.SHA512,
                [Models.Internal.Rom.SpamSumKey] = item.SpamSum,
                [Models.Internal.Rom.xxHash364Key] = item.xxHash364,
                [Models.Internal.Rom.xxHash3128Key] = item.xxHash3128,
                [Models.Internal.Rom.MergeKey] = item.Merge,
                [Models.Internal.Rom.StatusKey] = item.Status,
                [Models.Internal.Rom.RegionKey] = item.Region,
                [Models.Internal.Rom.FlagsKey] = item.Flags,
                [Models.Internal.Rom.OffsetKey] = item.Offs,
                [Models.Internal.Rom.SerialKey] = item.Serial,
                [Models.Internal.Rom.HeaderKey] = item.Header,
                [Models.Internal.Rom.DateKey] = item.Date,
                [Models.Internal.Rom.InvertedKey] = item.Inverted,
                [Models.Internal.Rom.MIAKey] = item.MIA,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Sample"/> to <cref="Models.Internal.Sample"/>
        /// </summary>
        public static Models.Internal.Sample ConvertFromClrMamePro(Models.ClrMamePro.Sample item)
        {
            var sample = new Models.Internal.Sample
            {
                [Models.Internal.Sample.NameKey] = item.Name,
            };
            return sample;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Sound"/> to <cref="Models.Internal.Sound"/>
        /// </summary>
        public static Models.Internal.Sound ConvertFromClrMamePro(Models.ClrMamePro.Sound item)
        {
            var sound = new Models.Internal.Sound
            {
                [Models.Internal.Sound.ChannelsKey] = item.Channels,
            };
            return sound;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Video"/> to <cref="Models.Internal.Video"/>
        /// </summary>
        public static Models.Internal.Video ConvertFromClrMamePro(Models.ClrMamePro.Video item)
        {
            var video = new Models.Internal.Video
            {
                [Models.Internal.Video.ScreenKey] = item.Screen,
                [Models.Internal.Video.OrientationKey] = item.Orientation,
                [Models.Internal.Video.WidthKey] = item.X,
                [Models.Internal.Video.HeightKey] = item.Y,
                [Models.Internal.Video.AspectXKey] = item.AspectX,
                [Models.Internal.Video.AspectYKey] = item.AspectY,
                [Models.Internal.Video.RefreshKey] = item.Freq,
            };
            return video;
        }

        #endregion

        #region DosCenter

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
    
        #region EverdriveSMDB

        /// <summary>
        /// Convert from <cref="Models.EverdriveSMDB.Row"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Rom ConvertFromEverdriveSMDB(Models.EverdriveSMDB.Row item)
        {
            var rom = new Models.Internal.Rom
            {
                [Models.Internal.Rom.SHA256Key] = item.SHA256,
                [Models.Internal.Rom.NameKey] = item.Name,
                [Models.Internal.Rom.SHA1Key] = item.SHA1,
                [Models.Internal.Rom.MD5Key] = item.MD5,
                [Models.Internal.Rom.CRCKey] = item.CRC32,
                [Models.Internal.Rom.SizeKey] = item.Size,
            };
            return rom;
        }

        #endregion
    }
}