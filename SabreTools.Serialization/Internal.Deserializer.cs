using System.Collections.Generic;

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

        #region Listxml

        /// <summary>
        /// Convert from <cref="Models.Internal.Adjuster"/> to <cref="Models.Listxml.Adjuster"/>
        /// </summary>
        public static Models.Listxml.Adjuster ConvertToListxml(Models.Internal.Adjuster item)
        {
            var adjuster = new Models.Listxml.Adjuster
            {
                Name = item.ReadString(Models.Internal.Adjuster.NameKey),
                Default = item.ReadString(Models.Internal.Adjuster.DefaultKey),
            };

            if (item.ContainsKey(Models.Internal.Adjuster.ConditionKey) && item[Models.Internal.Adjuster.ConditionKey] is Models.Internal.Condition condition)
                adjuster.Condition = ConvertToListxml(condition);

            return adjuster;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Analog"/> to <cref="Models.Listxml.Analog"/>
        /// </summary>
        public static Models.Listxml.Analog ConvertToListxml(Models.Internal.Analog item)
        {
            var analog = new Models.Listxml.Analog
            {
                Mask = item.ReadString(Models.Internal.Analog.MaskKey),
            };
            return analog;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.BiosSet"/> to <cref="Models.Listxml.BiosSet"/>
        /// </summary>
        public static Models.Listxml.BiosSet ConvertToListxml(Models.Internal.BiosSet item)
        {
            var biosset = new Models.Listxml.BiosSet
            {
                Name = item.ReadString(Models.Internal.BiosSet.NameKey),
                Description = item.ReadString(Models.Internal.BiosSet.DescriptionKey),
                Default = item.ReadString(Models.Internal.BiosSet.DefaultKey),
            };
            return biosset;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Chip"/> to <cref="Models.Listxml.Chip"/>
        /// </summary>
        public static Models.Listxml.Chip ConvertToListxml(Models.Internal.Chip item)
        {
            var chip = new Models.Listxml.Chip
            {
                Name = item.ReadString(Models.Internal.Chip.NameKey),
                Tag = item.ReadString(Models.Internal.Chip.TagKey),
                Type = item.ReadString(Models.Internal.Chip.TypeKey),
                SoundOnly = item.ReadString(Models.Internal.Chip.SoundOnlyKey),
                Clock = item.ReadString(Models.Internal.Chip.ClockKey),
            };
            return chip;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Condition"/> to <cref="Models.Listxml.Condition"/>
        /// </summary>
        public static Models.Listxml.Condition ConvertToListxml(Models.Internal.Condition item)
        {
            var condition = new Models.Listxml.Condition
            {
                Tag = item.ReadString(Models.Internal.Condition.TagKey),
                Mask = item.ReadString(Models.Internal.Condition.MaskKey),
                Relation = item.ReadString(Models.Internal.Condition.RelationKey),
                Value = item.ReadString(Models.Internal.Condition.ValueKey),
            };
            return condition;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Configuration"/> to <cref="Models.Listxml.Configuration"/>
        /// </summary>
        public static Models.Listxml.Configuration ConvertToListxml(Models.Internal.Configuration item)
        {
            var configuration = new Models.Listxml.Configuration
            {
                Name = item.ReadString(Models.Internal.Configuration.NameKey),
                Tag = item.ReadString(Models.Internal.Configuration.TagKey),
                Mask = item.ReadString(Models.Internal.Configuration.MaskKey),
            };

            if (item.ContainsKey(Models.Internal.Configuration.ConditionKey) && item[Models.Internal.Configuration.ConditionKey] is Models.Internal.Condition condition)
                configuration.Condition = ConvertToListxml(condition);

            if (item.ContainsKey(Models.Internal.Configuration.ConfLocationKey) && item[Models.Internal.Configuration.ConfLocationKey] is Models.Internal.ConfLocation[] confLocations)
            {
                var confLocationItems = new List<Models.Listxml.ConfLocation>();
                foreach (var confLocation in confLocations)
                {
                    confLocationItems.Add(ConvertToListxml(confLocation));
                }
                configuration.ConfLocation = confLocationItems.ToArray();
            }

            if (item.ContainsKey(Models.Internal.Configuration.ConfSettingKey) && item[Models.Internal.Configuration.ConfSettingKey] is Models.Internal.ConfSetting[] confSettings)
            {
                var confSettingItems = new List<Models.Listxml.ConfSetting>();
                foreach (var confSetting in confSettings)
                {
                    confSettingItems.Add(ConvertToListxml(confSetting));
                }
                configuration.ConfSetting = confSettingItems.ToArray();
            }

            return configuration;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.ConfLocation"/> to <cref="Models.Listxml.ConfLocation"/>
        /// </summary>
        public static Models.Listxml.ConfLocation ConvertToListxml(Models.Internal.ConfLocation item)
        {
            var confLocation = new Models.Listxml.ConfLocation
            {
                Name = item.ReadString(Models.Internal.ConfLocation.NameKey),
                Number = item.ReadString(Models.Internal.ConfLocation.NumberKey),
                Inverted = item.ReadString(Models.Internal.ConfLocation.InvertedKey),
            };
            return confLocation;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.ConfSetting"/> to <cref="Models.Listxml.ConfSetting"/>
        /// </summary>
        public static Models.Listxml.ConfSetting ConvertToListxml(Models.Internal.ConfSetting item)
        {
            var confSetting = new Models.Listxml.ConfSetting
            {
                Name = item.ReadString(Models.Internal.ConfSetting.NameKey),
                Value = item.ReadString(Models.Internal.ConfSetting.ValueKey),
                Default = item.ReadString(Models.Internal.ConfSetting.DefaultKey),
            };

            if (item.ContainsKey(Models.Internal.ConfSetting.ConditionKey) && item[Models.Internal.ConfSetting.ConditionKey] is Models.Internal.Condition condition)
                confSetting.Condition = ConvertToListxml(condition);

            return confSetting;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Control"/> to <cref="Models.Listxml.Control"/>
        /// </summary>
        public static Models.Listxml.Control ConvertToListxml(Models.Internal.Control item)
        {
            var control = new Models.Listxml.Control
            {
                Type = item.ReadString(Models.Internal.Control.TypeKey),
                Player = item.ReadString(Models.Internal.Control.PlayerKey),
                Buttons = item.ReadString(Models.Internal.Control.ButtonsKey),
                ReqButtons = item.ReadString(Models.Internal.Control.ReqButtonsKey),
                Minimum = item.ReadString(Models.Internal.Control.MinimumKey),
                Maximum = item.ReadString(Models.Internal.Control.MaximumKey),
                Sensitivity = item.ReadString(Models.Internal.Control.SensitivityKey),
                KeyDelta = item.ReadString(Models.Internal.Control.KeyDeltaKey),
                Reverse = item.ReadString(Models.Internal.Control.ReverseKey),
                Ways = item.ReadString(Models.Internal.Control.WaysKey),
                Ways2 = item.ReadString(Models.Internal.Control.Ways2Key),
                Ways3 = item.ReadString(Models.Internal.Control.Ways3Key),
            };
            return control;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Device"/> to <cref="Models.Listxml.Device"/>
        /// </summary>
        public static Models.Listxml.Device ConvertToListxml(Models.Internal.Device item)
        {
            var device = new Models.Listxml.Device
            {
                Type = item.ReadString(Models.Internal.Device.TypeKey),
                Tag = item.ReadString(Models.Internal.Device.TagKey),
                FixedImage = item.ReadString(Models.Internal.Device.FixedImageKey),
                Mandatory = item.ReadString(Models.Internal.Device.MandatoryKey),
                Interface = item.ReadString(Models.Internal.Device.InterfaceKey),
            };

            if (item.ContainsKey(Models.Internal.Device.InstanceKey) && item[Models.Internal.Device.InstanceKey] is Models.Internal.Instance instance)
                device.Instance = ConvertToListxml(instance);

            if (item.ContainsKey(Models.Internal.Device.ExtensionKey) && item[Models.Internal.Device.ExtensionKey] is Models.Internal.Extension[] extensions)
            {
                var extensionItems = new List<Models.Listxml.Extension>();
                foreach (var extension in extensions)
                {
                    extensionItems.Add(ConvertToListxml(extension));
                }
                device.Extension = extensionItems.ToArray();
            }

            return device;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.DeviceRef"/> to <cref="Models.Listxml.DeviceRef"/>
        /// </summary>
        public static Models.Listxml.DeviceRef ConvertToListxml(Models.Internal.DeviceRef item)
        {
            var deviceRef = new Models.Listxml.DeviceRef
            {
                Name = item.ReadString(Models.Internal.DeviceRef.NameKey),
            };
            return deviceRef;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.DipLocation"/> to <cref="Models.Listxml.DipLocation"/>
        /// </summary>
        public static Models.Listxml.DipLocation ConvertToListxml(Models.Internal.DipLocation item)
        {
            var dipLocation = new Models.Listxml.DipLocation
            {
                Name = item.ReadString(Models.Internal.DipLocation.NameKey),
                Number = item.ReadString(Models.Internal.DipLocation.NumberKey),
                Inverted = item.ReadString(Models.Internal.DipLocation.InvertedKey),
            };
            return dipLocation;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.DipSwitch"/> to <cref="Models.Listxml.DipSwitch"/>
        /// </summary>
        public static Models.Listxml.DipSwitch ConvertToListxml(Models.Internal.DipSwitch item)
        {
            var dipSwitch = new Models.Listxml.DipSwitch
            {
                Name = item.ReadString(Models.Internal.DipSwitch.NameKey),
                Tag = item.ReadString(Models.Internal.DipSwitch.TagKey),
                Mask = item.ReadString(Models.Internal.DipSwitch.MaskKey),
            };

            if (item.ContainsKey(Models.Internal.DipSwitch.ConditionKey) && item[Models.Internal.DipSwitch.ConditionKey] is Models.Internal.Condition condition)
                dipSwitch.Condition = ConvertToListxml(condition);

            if (item.ContainsKey(Models.Internal.DipSwitch.DipLocationKey) && item[Models.Internal.DipSwitch.DipLocationKey] is Models.Internal.DipLocation[] dipLocations)
            {
                var dipLocationItems = new List<Models.Listxml.DipLocation>();
                foreach (var dipLocation in dipLocations)
                {
                    dipLocationItems.Add(ConvertToListxml(dipLocation));
                }
                dipSwitch.DipLocation = dipLocationItems.ToArray();
            }

            if (item.ContainsKey(Models.Internal.DipSwitch.DipValueKey) && item[Models.Internal.DipSwitch.DipValueKey] is Models.Internal.DipValue[] dipValues)
            {
                var dipValueItems = new List<Models.Listxml.DipValue>();
                foreach (var dipValue in dipValues)
                {
                    dipValueItems.Add(ConvertToListxml(dipValue));
                }
                dipSwitch.DipValue = dipValueItems.ToArray();
            }

            return dipSwitch;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.DipValue"/> to <cref="Models.Listxml.DipValue"/>
        /// </summary>
        public static Models.Listxml.DipValue ConvertToListxml(Models.Internal.DipValue item)
        {
            var dipValue = new Models.Listxml.DipValue
            {
                Name = item.ReadString(Models.Internal.DipValue.NameKey),
                Value = item.ReadString(Models.Internal.DipValue.ValueKey),
                Default = item.ReadString(Models.Internal.DipValue.DefaultKey),
            };

            if (item.ContainsKey(Models.Internal.DipValue.ConditionKey) && item[Models.Internal.DipValue.ConditionKey] is Models.Internal.Condition condition)
                dipValue.Condition = ConvertToListxml(condition);

            return dipValue;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Disk"/> to <cref="Models.Listxml.Disk"/>
        /// </summary>
        public static Models.Listxml.Disk ConvertToListxml(Models.Internal.Disk item)
        {
            var disk = new Models.Listxml.Disk
            {
                Name = item.ReadString(Models.Internal.Disk.NameKey),
                MD5 = item.ReadString(Models.Internal.Disk.MD5Key),
                SHA1 = item.ReadString(Models.Internal.Disk.SHA1Key),
                Merge = item.ReadString(Models.Internal.Disk.MergeKey),
                Region = item.ReadString(Models.Internal.Disk.RegionKey),
                Index = item.ReadString(Models.Internal.Disk.IndexKey),
                Writable = item.ReadString(Models.Internal.Disk.WritableKey),
                Status = item.ReadString(Models.Internal.Disk.StatusKey),
                Optional = item.ReadString(Models.Internal.Disk.OptionalKey),
            };
            return disk;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Display"/> to <cref="Models.Listxml.Display"/>
        /// </summary>
        public static Models.Listxml.Display ConvertToListxml(Models.Internal.Display item)
        {
            var display = new Models.Listxml.Display
            {
                Tag = item.ReadString(Models.Internal.Display.TagKey),
                Type = item.ReadString(Models.Internal.Display.TypeKey),
                Rotate = item.ReadString(Models.Internal.Display.RotateKey),
                FlipX = item.ReadString(Models.Internal.Display.FlipXKey),
                Width = item.ReadString(Models.Internal.Display.WidthKey),
                Height = item.ReadString(Models.Internal.Display.HeightKey),
                Refresh = item.ReadString(Models.Internal.Display.RefreshKey),
                PixClock = item.ReadString(Models.Internal.Display.PixClockKey),
                HTotal = item.ReadString(Models.Internal.Display.HTotalKey),
                HBEnd = item.ReadString(Models.Internal.Display.HBEndKey),
                HBStart = item.ReadString(Models.Internal.Display.HBStartKey),
                VTotal = item.ReadString(Models.Internal.Display.VTotalKey),
                VBEnd = item.ReadString(Models.Internal.Display.VBEndKey),
                VBStart = item.ReadString(Models.Internal.Display.VBStartKey),
            };
            return display;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Driver"/> to <cref="Models.Listxml.Driver"/>
        /// </summary>
        public static Models.Listxml.Driver ConvertToListxml(Models.Internal.Driver item)
        {
            var driver = new Models.Listxml.Driver
            {
                Status = item.ReadString(Models.Internal.Driver.StatusKey),
                Color = item.ReadString(Models.Internal.Driver.ColorKey),
                Sound = item.ReadString(Models.Internal.Driver.SoundKey),
                PaletteSize = item.ReadString(Models.Internal.Driver.PaletteSizeKey),
                Emulation = item.ReadString(Models.Internal.Driver.EmulationKey),
                Cocktail = item.ReadString(Models.Internal.Driver.CocktailKey),
                SaveState = item.ReadString(Models.Internal.Driver.SaveStateKey),
                RequiresArtwork = item.ReadString(Models.Internal.Driver.RequiresArtworkKey),
                Unofficial = item.ReadString(Models.Internal.Driver.UnofficialKey),
                NoSoundHardware = item.ReadString(Models.Internal.Driver.NoSoundHardwareKey),
                Incomplete = item.ReadString(Models.Internal.Driver.IncompleteKey),
            };
            return driver;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Extension"/> to <cref="Models.Listxml.Extension"/>
        /// </summary>
        public static Models.Listxml.Extension ConvertToListxml(Models.Internal.Extension item)
        {
            var extension = new Models.Listxml.Extension
            {
                Name = item.ReadString(Models.Internal.Extension.NameKey),
            };
            return extension;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Feature"/> to <cref="Models.Listxml.Feature"/>
        /// </summary>
        public static Models.Listxml.Feature ConvertToListxml(Models.Internal.Feature item)
        {
            var feature = new Models.Listxml.Feature
            {
                Type = item.ReadString(Models.Internal.Feature.TypeKey),
                Status = item.ReadString(Models.Internal.Feature.StatusKey),
                Overall = item.ReadString(Models.Internal.Feature.OverallKey),
            };
            return feature;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Input"/> to <cref="Models.Listxml.Input"/>
        /// </summary>
        public static Models.Listxml.Input ConvertToListxml(Models.Internal.Input item)
        {
            var input = new Models.Listxml.Input
            {
                Service = item.ReadString(Models.Internal.Input.ServiceKey),
                Tilt = item.ReadString(Models.Internal.Input.TiltKey),
                Players = item.ReadString(Models.Internal.Input.PlayersKey),
                ControlAttr = item.ReadString(Models.Internal.Input.ControlKey),
                Buttons = item.ReadString(Models.Internal.Input.ButtonsKey),
                Coins = item.ReadString(Models.Internal.Input.CoinsKey),
            };

            if (item.ContainsKey(Models.Internal.Input.ControlKey) && item[Models.Internal.Input.ControlKey] is Models.Internal.Control[] controls)
            {
                var controlItems = new List<Models.Listxml.Control>();
                foreach (var control in controls)
                {
                    controlItems.Add(ConvertToListxml(control));
                }
                input.Control = controlItems.ToArray();
            }

            return input;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Instance"/> to <cref="Models.Listxml.Instance"/>
        /// </summary>
        public static Models.Listxml.Instance ConvertToListxml(Models.Internal.Instance item)
        {
            var instance = new Models.Listxml.Instance
            {
                Name = item.ReadString(Models.Internal.Instance.NameKey),
                BriefName = item.ReadString(Models.Internal.Instance.BriefNameKey),
            };
            return instance;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Port"/> to <cref="Models.Listxml.Port"/>
        /// </summary>
        public static Models.Listxml.Port ConvertToListxml(Models.Internal.Port item)
        {
            var input = new Models.Listxml.Port
            {
                Tag = item.ReadString(Models.Internal.Port.TagKey),
            };

            if (item.ContainsKey(Models.Internal.Port.AnalogKey) && item[Models.Internal.Port.AnalogKey] is Models.Internal.Analog[] analogs)
            {
                var analogItems = new List<Models.Listxml.Analog>();
                foreach (var analog in analogs)
                {
                    analogItems.Add(ConvertToListxml(analog));
                }
                input.Analog = analogItems.ToArray();
            }

            return input;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.RamOption"/> to <cref="Models.Listxml.RamOption"/>
        /// </summary>
        public static Models.Listxml.RamOption ConvertToListxml(Models.Internal.RamOption item)
        {
            var ramOption = new Models.Listxml.RamOption
            {
                Name = item.ReadString(Models.Internal.RamOption.NameKey),
                Default = item.ReadString(Models.Internal.RamOption.DefaultKey),
            };
            return ramOption;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.Listxml.Rom"/>
        /// </summary>
        public static Models.Listxml.Rom ConvertToListxml(Models.Internal.Rom item)
        {
            var rom = new Models.Listxml.Rom
            {
                Name = item.ReadString(Models.Internal.Rom.NameKey),
                Bios = item.ReadString(Models.Internal.Rom.BiosKey),
                Size = item.ReadString(Models.Internal.Rom.SizeKey),
                CRC = item.ReadString(Models.Internal.Rom.CRCKey),
                SHA1 = item.ReadString(Models.Internal.Rom.SHA1Key),
                Merge = item.ReadString(Models.Internal.Rom.MergeKey),
                Region = item.ReadString(Models.Internal.Rom.RegionKey),
                Offset = item.ReadString(Models.Internal.Rom.OffsetKey),
                Status = item.ReadString(Models.Internal.Rom.StatusKey),
                Optional = item.ReadString(Models.Internal.Rom.OptionalKey),
                Dispose = item.ReadString(Models.Internal.Rom.DisposeKey),
                SoundOnly = item.ReadString(Models.Internal.Rom.SoundOnlyKey),
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Sample"/> to <cref="Models.Listxml.Sample"/>
        /// </summary>
        public static Models.Listxml.Sample ConvertToListxml(Models.Internal.Sample item)
        {
            var sample = new Models.Listxml.Sample
            {
                Name = item.ReadString(Models.Internal.Sample.NameKey),
            };
            return sample;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Slot"/> to <cref="Models.Listxml.Slot"/>
        /// </summary>
        public static Models.Listxml.Slot ConvertToListxml(Models.Internal.Slot item)
        {
            var slot = new Models.Listxml.Slot
            {
                Name = item.ReadString(Models.Internal.Slot.NameKey),
            };

            if (item.ContainsKey(Models.Internal.Slot.SlotOptionKey) && item[Models.Internal.Slot.SlotOptionKey] is Models.Internal.SlotOption[] slotOptions)
            {
                var slotOptionItems = new List<Models.Listxml.SlotOption>();
                foreach (var slotOption in slotOptions)
                {
                    slotOptionItems.Add(ConvertToListxml(slotOption));
                }
                slot.SlotOption = slotOptionItems.ToArray();
            }

            return slot;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.SlotOption"/> to <cref="Models.Listxml.SlotOption"/>
        /// </summary>
        public static Models.Listxml.SlotOption ConvertToListxml(Models.Internal.SlotOption item)
        {
            var slotOption = new Models.Listxml.SlotOption
            {
                Name = item.ReadString(Models.Internal.SlotOption.NameKey),
                DevName = item.ReadString(Models.Internal.SlotOption.DevNameKey),
                Default = item.ReadString(Models.Internal.SlotOption.DefaultKey),
            };
            return slotOption;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.SoftwareList"/> to <cref="Models.Listxml.SoftwareList"/>
        /// </summary>
        public static Models.Listxml.SoftwareList ConvertToListxml(Models.Internal.SoftwareList item)
        {
            var softwareList = new Models.Listxml.SoftwareList
            {
                Tag = item.ReadString(Models.Internal.SoftwareList.TagKey),
                Name = item.ReadString(Models.Internal.SoftwareList.NameKey),
                Status = item.ReadString(Models.Internal.SoftwareList.StatusKey),
                Filter = item.ReadString(Models.Internal.SoftwareList.FilterKey),
            };
            return softwareList;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Sound"/> to <cref="Models.Listxml.Sound"/>
        /// </summary>
        public static Models.Listxml.Sound ConvertToListxml(Models.Internal.Sound item)
        {
            var sound = new Models.Listxml.Sound
            {
                Channels = item.ReadString(Models.Internal.Sound.ChannelsKey),
            };
            return sound;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Video"/> to <cref="Models.Listxml.Video"/>
        /// </summary>
        public static Models.Listxml.Video ConvertToListxml(Models.Internal.Video item)
        {
            var video = new Models.Listxml.Video
            {
                Screen = item.ReadString(Models.Internal.Video.ScreenKey),
                Orientation = item.ReadString(Models.Internal.Video.OrientationKey),
                Width = item.ReadString(Models.Internal.Video.WidthKey),
                Height = item.ReadString(Models.Internal.Video.HeightKey),
                AspectX = item.ReadString(Models.Internal.Video.AspectXKey),
                AspectY = item.ReadString(Models.Internal.Video.AspectYKey),
                Refresh = item.ReadString(Models.Internal.Video.RefreshKey),
            };
            return video;
        }

        #endregion

        #region Logiqx

        /// <summary>
        /// Convert from <cref="Models.Internal.Archive"/> to <cref="Models.Logiqx.Archive"/>
        /// </summary>
        public static Models.Logiqx.Archive ConvertToLogiqx(Models.Internal.Archive item)
        {
            var archive = new Models.Logiqx.Archive
            {
                Name = item.ReadString(Models.Internal.Archive.NameKey),
            };
            return archive;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.BiosSet"/> to <cref="Models.Logiqx.BiosSet"/>
        /// </summary>
        public static Models.Logiqx.BiosSet ConvertToLogiqx(Models.Internal.BiosSet item)
        {
            var biosset = new Models.Logiqx.BiosSet
            {
                Name = item.ReadString(Models.Internal.BiosSet.NameKey),
                Description = item.ReadString(Models.Internal.BiosSet.DescriptionKey),
                Default = item.ReadString(Models.Internal.BiosSet.DefaultKey),
            };
            return biosset;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.DeviceRef"/> to <cref="Models.Logiqx.DeviceRef"/>
        /// </summary>
        public static Models.Logiqx.DeviceRef ConvertToLogiqx(Models.Internal.DeviceRef item)
        {
            var deviceRef = new Models.Logiqx.DeviceRef
            {
                Name = item.ReadString(Models.Internal.DipSwitch.NameKey),
            };
            return deviceRef;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Disk"/> to <cref="Models.Logiqx.Disk"/>
        /// </summary>
        public static Models.Logiqx.Disk ConvertToLogiqx(Models.Internal.Disk item)
        {
            var disk = new Models.Logiqx.Disk
            {
                Name = item.ReadString(Models.Internal.Disk.NameKey),
                MD5 = item.ReadString(Models.Internal.Disk.MD5Key),
                SHA1 = item.ReadString(Models.Internal.Disk.SHA1Key),
                Merge = item.ReadString(Models.Internal.Disk.MergeKey),
                Status = item.ReadString(Models.Internal.Disk.StatusKey),
                Region = item.ReadString(Models.Internal.Disk.RegionKey),
            };
            return disk;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Driver"/> to <cref="Models.Logiqx.Driver"/>
        /// </summary>
        public static Models.Logiqx.Driver ConvertToLogiqx(Models.Internal.Driver item)
        {
            var driver = new Models.Logiqx.Driver
            {
                Status = item.ReadString(Models.Internal.Driver.StatusKey),
                Emulation = item.ReadString(Models.Internal.Driver.EmulationKey),
                Cocktail = item.ReadString(Models.Internal.Driver.CocktailKey),
                SaveState = item.ReadString(Models.Internal.Driver.SaveStateKey),
                RequiresArtwork = item.ReadString(Models.Internal.Driver.RequiresArtworkKey),
                Unofficial = item.ReadString(Models.Internal.Driver.UnofficialKey),
                NoSoundHardware = item.ReadString(Models.Internal.Driver.NoSoundHardwareKey),
                Incomplete = item.ReadString(Models.Internal.Driver.IncompleteKey),
            };
            return driver;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Media"/> to <cref="Models.Logiqx.Media"/>
        /// </summary>
        public static Models.Logiqx.Media ConvertToLogiqx(Models.Internal.Media item)
        {
            var media = new Models.Logiqx.Media
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
        /// Convert from <cref="Models.Internal.Release"/> to <cref="Models.Logiqx.Release"/>
        /// </summary>
        public static Models.Logiqx.Release ConvertToLogiqx(Models.Internal.Release item)
        {
            var release = new Models.Logiqx.Release
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
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.Logiqx.Rom"/>
        /// </summary>
        public static Models.Logiqx.Rom ConvertToLogiqx(Models.Internal.Rom item)
        {
            var rom = new Models.Logiqx.Rom
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
                Serial = item.ReadString(Models.Internal.Rom.SerialKey),
                Header = item.ReadString(Models.Internal.Rom.HeaderKey),
                Date = item.ReadString(Models.Internal.Rom.DateKey),
                Inverted = item.ReadString(Models.Internal.Rom.InvertedKey),
                MIA = item.ReadString(Models.Internal.Rom.MIAKey),
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Sample"/> to <cref="Models.Logiqx.Sample"/>
        /// </summary>
        public static Models.Logiqx.Sample ConvertToLogiqx(Models.Internal.Sample item)
        {
            var sample = new Models.Logiqx.Sample
            {
                Name = item.ReadString(Models.Internal.Sample.NameKey),
            };
            return sample;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.SoftwareList"/> to <cref="Models.Logiqx.SoftwareList"/>
        /// </summary>
        public static Models.Logiqx.SoftwareList ConvertToLogiqx(Models.Internal.SoftwareList item)
        {
            var softwareList = new Models.Logiqx.SoftwareList
            {
                Tag = item.ReadString(Models.Internal.SoftwareList.TagKey),
                Name = item.ReadString(Models.Internal.SoftwareList.NameKey),
                Status = item.ReadString(Models.Internal.SoftwareList.StatusKey),
                Filter = item.ReadString(Models.Internal.SoftwareList.FilterKey),
            };
            return softwareList;
        }

        #endregion

        #region OfflineList

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

        #region OpenMSX

        /// <summary>
        /// Convert from <cref="Models.Internal.Dump"/> to <cref="Models.OpenMSX.Dump"/>
        /// </summary>
        public static Models.OpenMSX.Dump ConvertToOpenMSX(Models.Internal.Dump item)
        {
            var dump = new Models.OpenMSX.Dump();

            if (item.ContainsKey(Models.Internal.Dump.OriginalKey) && item[Models.Internal.Dump.OriginalKey] is Models.Internal.Original original)
                dump.Original = ConvertToOpenMSX(original);

            if (item.ContainsKey(Models.Internal.Dump.RomKey) && item[Models.Internal.Dump.RomKey] is Models.Internal.Rom rom)
            {
                dump.Rom = ConvertToOpenMSXRom(rom);
            }
            else if (item.ContainsKey(Models.Internal.Dump.MegaRomKey) && item[Models.Internal.Dump.MegaRomKey] is Models.Internal.Rom megaRom)
            {
                dump.Rom = ConvertToOpenMSXMegaRom(megaRom);
            }
            else if (item.ContainsKey(Models.Internal.Dump.SCCPlusCartKey) && item[Models.Internal.Dump.SCCPlusCartKey] is Models.Internal.Rom sccPlusCart)
            {
                dump.Rom = ConvertToOpenMSXSCCPlusCart(sccPlusCart);
            }

            return dump;
        }
        
        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.OpenMSX.MegaRom"/>
        /// </summary>
        public static Models.OpenMSX.MegaRom ConvertToOpenMSXMegaRom(Models.Internal.Rom item)
        {
            var megaRom = new Models.OpenMSX.MegaRom
            {
                Start = item.ReadString(Models.Internal.Rom.StartKey),
                Type = item.ReadString(Models.Internal.Rom.TypeKey),
                Hash = item.ReadString(Models.Internal.Rom.SHA1Key),
                Remark = item.ReadString(Models.Internal.Rom.RemarkKey),
            };
            return megaRom;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Original"/> to <cref="Models.OpenMSX.Original"/>
        /// </summary>
        public static Models.OpenMSX.Original ConvertToOpenMSX(Models.Internal.Original item)
        {
            var original = new Models.OpenMSX.Original
            {
                Value = item.ReadString(Models.Internal.Original.ValueKey),
                Content = item.ReadString(Models.Internal.Original.ContentKey),
            };
            return original;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.OpenMSX.Rom"/>
        /// </summary>
        public static Models.OpenMSX.Rom ConvertToOpenMSXRom(Models.Internal.Rom item)
        {
            var rom = new Models.OpenMSX.Rom
            {
                Start = item.ReadString(Models.Internal.Rom.StartKey),
                Type = item.ReadString(Models.Internal.Rom.TypeKey),
                Hash = item.ReadString(Models.Internal.Rom.SHA1Key),
                Remark = item.ReadString(Models.Internal.Rom.RemarkKey),
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.OpenMSX.SCCPlusCart"/>
        /// </summary>
        public static Models.OpenMSX.SCCPlusCart ConvertToOpenMSXSCCPlusCart(Models.Internal.Rom item)
        {
            var sccPlusCart = new Models.OpenMSX.SCCPlusCart
            {
                Start = item.ReadString(Models.Internal.Rom.StartKey),
                Type = item.ReadString(Models.Internal.Rom.TypeKey),
                Hash = item.ReadString(Models.Internal.Rom.SHA1Key),
                Remark = item.ReadString(Models.Internal.Rom.RemarkKey),
            };
            return sccPlusCart;
        }

        #endregion
        
        #region RomCenter

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
    
        #region SeparatedValue

        /// <summary>
        /// Convert from <cref="Models.Internal.Disk"/> to <cref="Models.SeparatedValue.Row"/>
        /// </summary>
        public static Models.SeparatedValue.Row ConvertToSeparatedValue(Models.Internal.Disk item)
        {
            var row = new Models.SeparatedValue.Row
            {
                Type = "disk",
                DiskName = item.ReadString(Models.Internal.Disk.NameKey),
                MD5 = item.ReadString(Models.Internal.Disk.MD5Key),
                SHA1 = item.ReadString(Models.Internal.Disk.SHA1Key),
                Status = item.ReadString(Models.Internal.Disk.StatusKey),
            };
            return row;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Media"/> to <cref="Models.SeparatedValue.Row"/>
        /// </summary>
        public static Models.SeparatedValue.Row ConvertToSeparatedValue(Models.Internal.Media item)
        {
            var row = new Models.SeparatedValue.Row
            {
                Type = "media",
                DiskName = item.ReadString(Models.Internal.Media.NameKey),
                MD5 = item.ReadString(Models.Internal.Media.MD5Key),
                SHA1 = item.ReadString(Models.Internal.Media.SHA1Key),
                SHA256 = item.ReadString(Models.Internal.Media.SHA256Key),
                SpamSum = item.ReadString(Models.Internal.Media.SpamSumKey),
            };
            return row;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.SeparatedValue.Row"/>
        /// </summary>
        public static Models.SeparatedValue.Row ConvertToSeparatedValue(Models.Internal.Rom item)
        {
            var row = new Models.SeparatedValue.Row
            {
                Type = "rom",
                RomName = item.ReadString(Models.Internal.Rom.NameKey),
                Size = item.ReadString(Models.Internal.Rom.SizeKey),
                CRC = item.ReadString(Models.Internal.Rom.CRCKey),
                MD5 = item.ReadString(Models.Internal.Rom.MD5Key),
                SHA1 = item.ReadString(Models.Internal.Rom.SHA1Key),
                SHA256 = item.ReadString(Models.Internal.Rom.SHA256Key),
                SHA384 = item.ReadString(Models.Internal.Rom.SHA384Key),
                SHA512 = item.ReadString(Models.Internal.Rom.SHA512Key),
                SpamSum = item.ReadString(Models.Internal.Rom.SpamSumKey),
                Status = item.ReadString(Models.Internal.Rom.StatusKey),
            };
            return row;
        }

        #endregion
    }
}