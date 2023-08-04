namespace SabreTools.Serialization
{
    /// <summary>
    /// Deserializer for all relevant models from internal structure
    /// </summary>
    public partial class Internal
    {
        #region ArchiveDotOrg

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.ArchiveDotOrg.File"/>
        /// </summary>
        public static Models.ArchiveDotOrg.File ConvertToArchiveDotOrg(Models.Internal.Rom item)
        {
            var file = new Models.ArchiveDotOrg.File
            {
                Name = item.ReadString(Models.Internal.Rom.NameKey),
                Source = item.ReadString(Models.Internal.Rom.SourceKey),
                BitTorrentMagnetHash = item.ReadString(Models.Internal.Rom.BitTorrentMagnetHashKey),
                LastModifiedTime = item.ReadString(Models.Internal.Rom.LastModifiedTimeKey),
                Size = item.ReadString(Models.Internal.Rom.SizeKey),
                MD5 = item.ReadString(Models.Internal.Rom.MD5Key),
                CRC32 = item.ReadString(Models.Internal.Rom.CRCKey),
                SHA1 = item.ReadString(Models.Internal.Rom.SHA1Key),
                FileCount = item.ReadString(Models.Internal.Rom.FileCountKey),
                Format = item.ReadString(Models.Internal.Rom.FormatKey),
                Original = item.ReadString(Models.Internal.Rom.OriginalKey),
                Summation = item.ReadString(Models.Internal.Rom.SummationKey),
                MatrixNumber = item.ReadString(Models.Internal.Rom.MatrixNumberKey),
                CollectionCatalogNumber = item.ReadString(Models.Internal.Rom.CollectionCatalogNumberKey),
                Comment = item.ReadString(Models.Internal.Rom.CommentKey),

                ASRDetectedLang = item.ReadString(Models.Internal.Rom.ASRDetectedLangKey),
                ASRDetectedLangConf = item.ReadString(Models.Internal.Rom.ASRDetectedLangConfKey),
                ASRTranscribedLang = item.ReadString(Models.Internal.Rom.ASRTranscribedLangKey),
                WhisperASRModuleVersion = item.ReadString(Models.Internal.Rom.WhisperASRModuleVersionKey),
                WhisperModelHash = item.ReadString(Models.Internal.Rom.WhisperModelHashKey),
                WhisperModelName = item.ReadString(Models.Internal.Rom.WhisperModelNameKey),
                WhisperVersion = item.ReadString(Models.Internal.Rom.WhisperVersionKey),

                ClothCoverDetectionModuleVersion = item.ReadString(Models.Internal.Rom.ClothCoverDetectionModuleVersionKey),
                hOCRCharToWordhOCRVersion = item.ReadString(Models.Internal.Rom.hOCRCharToWordhOCRVersionKey),
                hOCRCharToWordModuleVersion = item.ReadString(Models.Internal.Rom.hOCRCharToWordModuleVersionKey),
                hOCRFtsTexthOCRVersion = item.ReadString(Models.Internal.Rom.hOCRFtsTexthOCRVersionKey),
                hOCRFtsTextModuleVersion = item.ReadString(Models.Internal.Rom.hOCRFtsTextModuleVersionKey),
                hOCRPageIndexhOCRVersion = item.ReadString(Models.Internal.Rom.hOCRPageIndexhOCRVersionKey),
                hOCRPageIndexModuleVersion = item.ReadString(Models.Internal.Rom.hOCRPageIndexModuleVersionKey),
                TesseractOCR = item.ReadString(key: Models.Internal.Rom.TesseractOCRKey),
                TesseractOCRConverted = item.ReadString(Models.Internal.Rom.TesseractOCRConvertedKey),
                TesseractOCRDetectedLang = item.ReadString(Models.Internal.Rom.TesseractOCRDetectedLangKey),
                TesseractOCRDetectedLangConf = item.ReadString(Models.Internal.Rom.TesseractOCRDetectedLangConfKey),
                TesseractOCRDetectedScript = item.ReadString(Models.Internal.Rom.TesseractOCRDetectedScriptKey),
                TesseractOCRDetectedScriptConf = item.ReadString(Models.Internal.Rom.TesseractOCRDetectedScriptConfKey),
                TesseractOCRModuleVersion = item.ReadString(Models.Internal.Rom.TesseractOCRModuleVersionKey),
                TesseractOCRParameters = item.ReadString(Models.Internal.Rom.TesseractOCRParametersKey),
                PDFModuleVersion = item.ReadString(Models.Internal.Rom.PDFModuleVersionKey),
                WordConfidenceInterval0To10 = item.ReadString(Models.Internal.Rom.WordConfidenceInterval0To10Key),
                WordConfidenceInterval11To20 = item.ReadString(Models.Internal.Rom.WordConfidenceInterval11To20Key),
                WordConfidenceInterval21To30 = item.ReadString(Models.Internal.Rom.WordConfidenceInterval21To30Key),
                WordConfidenceInterval31To40 = item.ReadString(Models.Internal.Rom.WordConfidenceInterval31To40Key),
                WordConfidenceInterval41To50 = item.ReadString(Models.Internal.Rom.WordConfidenceInterval41To50Key),
                WordConfidenceInterval51To60 = item.ReadString(Models.Internal.Rom.WordConfidenceInterval51To60Key),
                WordConfidenceInterval61To70 = item.ReadString(Models.Internal.Rom.WordConfidenceInterval61To70Key),
                WordConfidenceInterval71To80 = item.ReadString(Models.Internal.Rom.WordConfidenceInterval71To80Key),
                WordConfidenceInterval81To90 = item.ReadString(Models.Internal.Rom.WordConfidenceInterval81To90Key),
                WordConfidenceInterval91To100 = item.ReadString(Models.Internal.Rom.WordConfidenceInterval91To100Key),

                Album = item.ReadString(Models.Internal.Rom.AlbumKey),
                Artist = item.ReadString(Models.Internal.Rom.ArtistKey),
                Bitrate = item.ReadString(Models.Internal.Rom.BitrateKey),
                Creator = item.ReadString(Models.Internal.Rom.CreatorKey),
                Height = item.ReadString(Models.Internal.Rom.HeightKey),
                Length = item.ReadString(Models.Internal.Rom.LengthKey),
                PreviewImage = item.ReadString(Models.Internal.Rom.PreviewImageKey),
                Rotation = item.ReadString(Models.Internal.Rom.RotationKey),
                Title = item.ReadString(Models.Internal.Rom.TitleKey),
                Track = item.ReadString(Models.Internal.Rom.TrackKey),
                Width = item.ReadString(Models.Internal.Rom.WidthKey),
            };
            return file;
        }

        #endregion

        #region AttractMode

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
            };
            return row;
        }

        #endregion

        #region ClrMamePro

        /// <summary>
        /// Convert from <cref="Models.Internal.Archive"/> to <cref="Models.ClrMamePro.Archive"/>
        /// </summary>
        public static Models.ClrMamePro.Archive ConvertToClrMamePro(Models.Internal.Archive item)
        {
            var archive = new Models.ClrMamePro.Archive
            {
                Name = item.ReadString(Models.Internal.Archive.NameKey),
            };
            return archive;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.BiosSet"/> to <cref="Models.ClrMamePro.BiosSet"/>
        /// </summary>
        public static Models.ClrMamePro.BiosSet ConvertToClrMamePro(Models.Internal.BiosSet item)
        {
            var biosset = new Models.ClrMamePro.BiosSet
            {
                Name = item.ReadString(Models.Internal.BiosSet.NameKey),
                Description = item.ReadString(Models.Internal.BiosSet.DescriptionKey),
                Default = item.ReadString(Models.Internal.BiosSet.DefaultKey),
            };
            return biosset;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Chip"/> to <cref="Models.ClrMamePro.Chip"/>
        /// </summary>
        public static Models.ClrMamePro.Chip ConvertToClrMamePro(Models.Internal.Chip item)
        {
            var chip = new Models.ClrMamePro.Chip
            {
                Type = item.ReadString(Models.Internal.Chip.ChipTypeKey),
                Name = item.ReadString(Models.Internal.Chip.NameKey),
                Flags = item.ReadString(Models.Internal.Chip.FlagsKey),
                Clock = item.ReadString(Models.Internal.Chip.ClockKey),
            };
            return chip;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.DipSwitch"/> to <cref="Models.ClrMamePro.DipSwitch"/>
        /// </summary>
        public static Models.ClrMamePro.DipSwitch ConvertToClrMamePro(Models.Internal.DipSwitch item)
        {
            var dipswitch = new Models.ClrMamePro.DipSwitch
            {
                Name = item.ReadString(Models.Internal.DipSwitch.NameKey),
                Entry = item[Models.Internal.DipSwitch.EntryKey] as string[],
                Default = item.ReadString(Models.Internal.DipSwitch.DefaultKey),
            };
            return dipswitch;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Disk"/> to <cref="Models.ClrMamePro.Disk"/>
        /// </summary>
        public static Models.ClrMamePro.Disk ConvertToClrMamePro(Models.Internal.Disk item)
        {
            var disk = new Models.ClrMamePro.Disk
            {
                Name = item.ReadString(Models.Internal.Disk.NameKey),
                MD5 = item.ReadString(Models.Internal.Disk.MD5Key),
                SHA1 = item.ReadString(Models.Internal.Disk.SHA1Key),
                Merge = item.ReadString(Models.Internal.Disk.MergeKey),
                Status = item.ReadString(Models.Internal.Disk.StatusKey),
                Flags = item.ReadString(Models.Internal.Disk.FlagsKey),
            };
            return disk;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Driver"/> to <cref="Models.ClrMamePro.Driver"/>
        /// </summary>
        public static Models.ClrMamePro.Driver ConvertToClrMamePro(Models.Internal.Driver item)
        {
            var driver = new Models.ClrMamePro.Driver
            {
                Status = item.ReadString(Models.Internal.Driver.StatusKey),
                Color = item.ReadString(Models.Internal.Driver.ColorKey),
                Sound = item.ReadString(Models.Internal.Driver.SoundKey),
                PaletteSize = item.ReadString(Models.Internal.Driver.PaletteSizeKey),
                Blit = item.ReadString(Models.Internal.Driver.BlitKey),
            };
            return driver;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Input"/> to <cref="Models.ClrMamePro.Input"/>
        /// </summary>
        public static Models.ClrMamePro.Input ConvertToClrMamePro(Models.Internal.Input item)
        {
            var input = new Models.ClrMamePro.Input
            {
                Players = item.ReadString(Models.Internal.Input.PlayersKey),
                Control = item.ReadString(Models.Internal.Input.ControlKey),
                Buttons = item.ReadString(Models.Internal.Input.ButtonsKey),
                Coins = item.ReadString(Models.Internal.Input.CoinsKey),
                Tilt = item.ReadString(Models.Internal.Input.TiltKey),
                Service = item.ReadString(Models.Internal.Input.ServiceKey),
            };
            return input;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Media"/> to <cref="Models.ClrMamePro.Media"/>
        /// </summary>
        public static Models.ClrMamePro.Media ConvertToClrMamePro(Models.Internal.Media item)
        {
            var media = new Models.ClrMamePro.Media
            {
                Name = item.ReadString(Models.Internal.Media.NameKey),
                MD5 = item.ReadString(Models.Internal.Media.MD5Key),
                SHA1 = item.ReadString(Models.Internal.Media.SHA1Key),
                SHA256 = item.ReadString(Models.Internal.Media.SHA256Key),
                SpamSum = item.ReadString(Models.Internal.Media.SpamSumKey),
            };
            return media;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Release"/> to <cref="Models.ClrMamePro.Release"/>
        /// </summary>
        public static Models.ClrMamePro.Release ConvertToClrMamePro(Models.Internal.Release item)
        {
            var release = new Models.ClrMamePro.Release
            {
                Name = item.ReadString(Models.Internal.Release.NameKey),
                Region = item.ReadString(Models.Internal.Release.RegionKey),
                Language = item.ReadString(Models.Internal.Release.LanguageKey),
                Date = item.ReadString(Models.Internal.Release.DateKey),
                Default = item.ReadString(Models.Internal.Release.DefaultKey),
            };
            return release;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.ClrMamePro.Rom"/>
        /// </summary>
        public static Models.ClrMamePro.Rom ConvertToClrMamePro(Models.Internal.Rom item)
        {
            var rom = new Models.ClrMamePro.Rom
            {
                Name = item.ReadString(Models.Internal.Rom.NameKey),
                Size = item.ReadString(Models.Internal.Rom.SizeKey),
                CRC = item.ReadString(Models.Internal.Rom.CRCKey),
                MD5 = item.ReadString(Models.Internal.Rom.MD5Key),
                SHA1 = item.ReadString(Models.Internal.Rom.SHA1Key),
                SHA256 = item.ReadString(Models.Internal.Rom.SHA256Key),
                SHA384 = item.ReadString(Models.Internal.Rom.SHA384Key),
                SHA512 = item.ReadString(Models.Internal.Rom.SHA512Key),
                SpamSum = item.ReadString(Models.Internal.Rom.SpamSumKey),
                xxHash364 = item.ReadString(Models.Internal.Rom.xxHash364Key),
                xxHash3128 = item.ReadString(Models.Internal.Rom.xxHash3128Key),
                Merge = item.ReadString(Models.Internal.Rom.MergeKey),
                Status = item.ReadString(Models.Internal.Rom.StatusKey),
                Region = item.ReadString(Models.Internal.Rom.RegionKey),
                Flags = item.ReadString(Models.Internal.Rom.FlagsKey),
                Offs = item.ReadString(Models.Internal.Rom.OffsetKey),
                Serial = item.ReadString(Models.Internal.Rom.SerialKey),
                Header = item.ReadString(Models.Internal.Rom.HeaderKey),
                Date = item.ReadString(Models.Internal.Rom.DateKey),
                Inverted = item.ReadString(Models.Internal.Rom.InvertedKey),
                MIA = item.ReadString(Models.Internal.Rom.MIAKey),
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Sample"/> to <cref="Models.ClrMamePro.Sample"/>
        /// </summary>
        public static Models.ClrMamePro.Sample ConvertToClrMamePro(Models.Internal.Sample item)
        {
            var sample = new Models.ClrMamePro.Sample
            {
                Name = item.ReadString(Models.Internal.Sample.NameKey),
            };
            return sample;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Sound"/> to <cref="Models.ClrMamePro.Sound"/>
        /// </summary>
        public static Models.ClrMamePro.Sound ConvertToClrMamePro(Models.Internal.Sound item)
        {
            var sound = new Models.ClrMamePro.Sound
            {
                Channels = item.ReadString(Models.Internal.Sound.ChannelsKey),
            };
            return sound;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Video"/> to <cref="Models.ClrMamePro.Video"/>
        /// </summary>
        public static Models.ClrMamePro.Video ConvertToClrMamePro(Models.Internal.Video item)
        {
            var video = new Models.ClrMamePro.Video
            {
                Screen = item.ReadString(Models.Internal.Video.ScreenKey),
                Orientation = item.ReadString(Models.Internal.Video.OrientationKey),
                X = item.ReadString(Models.Internal.Video.WidthKey),
                Y = item.ReadString(Models.Internal.Video.HeightKey),
                AspectX = item.ReadString(Models.Internal.Video.AspectXKey),
                AspectY = item.ReadString(Models.Internal.Video.AspectYKey),
                Freq = item.ReadString(Models.Internal.Video.RefreshKey),
            };
            return video;
        }

        #endregion

        #region DosCenter

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
        
        #region EverdriveSMDB

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.EverdriveSMDB.Row"/>
        /// </summary>
        public static Models.EverdriveSMDB.Row ConvertToEverdriveSMDB(Models.Internal.Rom item)
        {
            var row = new Models.EverdriveSMDB.Row
            {
                SHA256 = item.ReadString(Models.Internal.Rom.SHA256Key),
                Name = item.ReadString(Models.Internal.Rom.NameKey),
                SHA1 = item.ReadString(Models.Internal.Rom.SHA1Key),
                MD5 = item.ReadString(Models.Internal.Rom.MD5Key),
                CRC32 = item.ReadString(Models.Internal.Rom.CRCKey),
                Size = item.ReadString(Models.Internal.Rom.SizeKey),
            };
            return row;
        }

        #endregion
        
        #region Hashfile

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.Hashfile.MD5"/>
        /// </summary>
        public static Models.Hashfile.MD5 ConvertToMD5(Models.Internal.Rom item)
        {
            var md5 = new Models.Hashfile.MD5
            {
                Hash = item.ReadString(Models.Internal.Rom.MD5Key),
                File = item.ReadString(Models.Internal.Rom.NameKey),
            };
            return md5;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.Hashfile.SFV"/>
        /// </summary>
        public static Models.Hashfile.SFV ConvertToSFV(Models.Internal.Rom item)
        {
            var sfv = new Models.Hashfile.SFV
            {
                File = item.ReadString(Models.Internal.Rom.NameKey),
                Hash = item.ReadString(Models.Internal.Rom.CRCKey),
            };
            return sfv;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.Hashfile.SHA1"/>
        /// </summary>
        public static Models.Hashfile.SHA1 ConvertToSHA1(Models.Internal.Rom item)
        {
            var sha1 = new Models.Hashfile.SHA1
            {
                Hash = item.ReadString(Models.Internal.Rom.SHA1Key),
                File = item.ReadString(Models.Internal.Rom.NameKey),
            };
            return sha1;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.Hashfile.SHA256"/>
        /// </summary>
        public static Models.Hashfile.SHA256 ConvertToSHA256(Models.Internal.Rom item)
        {
            var sha256 = new Models.Hashfile.SHA256
            {
                Hash = item.ReadString(Models.Internal.Rom.SHA256Key),
                File = item.ReadString(Models.Internal.Rom.NameKey),
            };
            return sha256;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.Hashfile.SHA384"/>
        /// </summary>
        public static Models.Hashfile.SHA384 ConvertToSHA384(Models.Internal.Rom item)
        {
            var sha384 = new Models.Hashfile.SHA384
            {
                Hash = item.ReadString(Models.Internal.Rom.SHA384Key),
                File = item.ReadString(Models.Internal.Rom.NameKey),
            };
            return sha384;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.Hashfile.SHA512"/>
        /// </summary>
        public static Models.Hashfile.SHA512 ConvertToSHA512(Models.Internal.Rom item)
        {
            var sha512 = new Models.Hashfile.SHA512
            {
                Hash = item.ReadString(Models.Internal.Rom.SHA512Key),
                File = item.ReadString(Models.Internal.Rom.NameKey),
            };
            return sha512;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.Hashfile.SpamSum"/>
        /// </summary>
        public static Models.Hashfile.SpamSum ConvertToSpamSum(Models.Internal.Rom item)
        {
            var spamsum = new Models.Hashfile.SpamSum
            {
                Hash = item.ReadString(Models.Internal.Rom.SpamSumKey),
                File = item.ReadString(Models.Internal.Rom.NameKey),
            };
            return spamsum;
        }

        #endregion
        
        #region Listrom

        /// <summary>
        /// Convert from <cref="Models.Internal.Disk"/> to <cref="Models.Listrom.Row"/>
        /// </summary>
        public static Models.Listrom.Row ConvertToListrom(Models.Internal.Disk item)
        {
            var row = new Models.Listrom.Row
            {
                Name = item.ReadString(Models.Internal.Disk.NameKey),
                MD5 = item.ReadString(Models.Internal.Disk.MD5Key),
                SHA1 = item.ReadString(Models.Internal.Disk.SHA1Key),
            };

            if (item[Models.Internal.Disk.StatusKey] as string == "nodump")
                row.NoGoodDumpKnown = true;
            else if (item[Models.Internal.Disk.StatusKey] as string == "baddump")
                row.Bad = true;

            return row;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.Listrom.Row"/>
        /// </summary>
        public static Models.Listrom.Row ConvertToListrom(Models.Internal.Rom item)
        {
            var row = new Models.Listrom.Row
            {
                Name = item.ReadString(Models.Internal.Rom.NameKey),
                Size = item.ReadString(Models.Internal.Rom.SizeKey),
                CRC = item.ReadString(Models.Internal.Rom.CRCKey),
                SHA1 = item.ReadString(Models.Internal.Rom.SHA1Key),
            };

            if (item[Models.Internal.Rom.StatusKey] as string == "nodump")
                row.NoGoodDumpKnown = true;
            else if (item[Models.Internal.Rom.StatusKey] as string == "baddump")
                row.Bad = true;

            return row;
        }

        #endregion
    }
}