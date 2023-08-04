using System.Collections.Generic;
using System.Linq;

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

        #region Hashfile

        /// <summary>
        /// Convert from <cref="Models.Hashfile.MD5"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Rom ConvertFromMD5(Models.Hashfile.MD5 item)
        {
            var rom = new Models.Internal.Rom
            {
                [Models.Internal.Rom.MD5Key] = item.Hash,
                [Models.Internal.Rom.NameKey] = item.File,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.SFV"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Rom ConvertFromSFV(Models.Hashfile.SFV item)
        {
            var rom = new Models.Internal.Rom
            {
                [Models.Internal.Rom.NameKey] = item.File,
                [Models.Internal.Rom.CRCKey] = item.Hash,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.SHA1"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Rom ConvertFromSHA1(Models.Hashfile.SHA1 item)
        {
            var rom = new Models.Internal.Rom
            {
                [Models.Internal.Rom.SHA1Key] = item.Hash,
                [Models.Internal.Rom.NameKey] = item.File,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.SHA256"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Rom ConvertFromSHA256(Models.Hashfile.SHA256 item)
        {
            var rom = new Models.Internal.Rom
            {
                [Models.Internal.Rom.SHA256Key] = item.Hash,
                [Models.Internal.Rom.NameKey] = item.File,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.SHA384"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Rom ConvertFromSHA384(Models.Hashfile.SHA384 item)
        {
            var rom = new Models.Internal.Rom
            {
                [Models.Internal.Rom.SHA384Key] = item.Hash,
                [Models.Internal.Rom.NameKey] = item.File,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.SHA512"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Rom ConvertFromSHA512(Models.Hashfile.SHA512 item)
        {
            var rom = new Models.Internal.Rom
            {
                [Models.Internal.Rom.SHA512Key] = item.Hash,
                [Models.Internal.Rom.NameKey] = item.File,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Hashfile.SpamSum"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Rom ConvertFromSpamSum(Models.Hashfile.SpamSum item)
        {
            var rom = new Models.Internal.Rom
            {
                [Models.Internal.Rom.SpamSumKey] = item.Hash,
                [Models.Internal.Rom.NameKey] = item.File,
            };
            return rom;
        }

        #endregion

        #region Listrom

        /// <summary>
        /// Convert from <cref="Models.Listrom.Row"/> to <cref="Models.Internal.Disk"/> or <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.DatItem ConvertFromListrom(Models.Listrom.Row item)
        {
            if (item.Size == null)
            {
                var disk = new Models.Internal.Disk
                {
                    [Models.Internal.Disk.NameKey] = item.Name,
                    [Models.Internal.Disk.MD5Key] = item.MD5,
                    [Models.Internal.Disk.SHA1Key] = item.SHA1,
                };

                if (item.NoGoodDumpKnown)
                    disk[Models.Internal.Disk.StatusKey] = "nodump";
                else if (item.Bad)
                    disk[Models.Internal.Disk.StatusKey] = "baddump";

                return disk;
            }
            else
            {
                var rom = new Models.Internal.Rom
                {
                    [Models.Internal.Rom.NameKey] = item.Name,
                    [Models.Internal.Rom.SizeKey] = item.Size,
                    [Models.Internal.Rom.CRCKey] = item.CRC,
                    [Models.Internal.Rom.SHA1Key] = item.SHA1,
                };

                if (item.NoGoodDumpKnown)
                    rom[Models.Internal.Rom.StatusKey] = "nodump";
                else if (item.Bad)
                    rom[Models.Internal.Rom.StatusKey] = "baddump";

                return rom;
            }
        }

        #endregion

        #region Listxml

        /// <summary>
        /// Convert from <cref="Models.Listxml.Adjuster"/> to <cref="Models.Internal.Adjuster"/>
        /// </summary>
        public static Models.Internal.Adjuster ConvertFromListxml(Models.Listxml.Adjuster item)
        {
            var adjuster = new Models.Internal.Adjuster
            {
                [Models.Internal.Adjuster.NameKey] = item.Name,
                [Models.Internal.Adjuster.DefaultKey] = item.Default,
            };

            if (item.Condition != null)
                adjuster[Models.Internal.Adjuster.ConditionKey] = ConvertFromListxml(item.Condition);

            return adjuster;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Analog"/> to <cref="Models.Internal.Analog"/>
        /// </summary>
        public static Models.Internal.Analog ConvertFromListxml(Models.Listxml.Analog item)
        {
            var analog = new Models.Internal.Analog
            {
                [Models.Internal.Analog.MaskKey] = item.Mask,
            };
            return analog;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.BiosSet"/> to <cref="Models.Internal.BiosSet"/>
        /// </summary>
        public static Models.Internal.BiosSet ConvertFromListxml(Models.Listxml.BiosSet item)
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
        /// Convert from <cref="Models.Listxml.Chip"/> to <cref="Models.Internal.Chip"/>
        /// </summary>
        public static Models.Internal.Chip ConvertFromListxml(Models.Listxml.Chip item)
        {
            var chip = new Models.Internal.Chip
            {
                [Models.Internal.Chip.NameKey] = item.Name,
                [Models.Internal.Chip.TagKey] = item.Tag,
                [Models.Internal.Chip.TypeKey] = item.Type,
                [Models.Internal.Chip.SoundOnlyKey] = item.SoundOnly,
                [Models.Internal.Chip.ClockKey] = item.Clock,
            };
            return chip;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Condition"/> to <cref="Models.Internal.Condition"/>
        /// </summary>
        public static Models.Internal.Condition ConvertFromListxml(Models.Listxml.Condition item)
        {
            var condition = new Models.Internal.Condition
            {
                [Models.Internal.Condition.TagKey] = item.Tag,
                [Models.Internal.Condition.MaskKey] = item.Mask,
                [Models.Internal.Condition.RelationKey] = item.Relation,
                [Models.Internal.Condition.ValueKey] = item.Value,
            };
            return condition;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Configuration"/> to <cref="Models.Internal.Configuration"/>
        /// </summary>
        public static Models.Internal.Configuration ConvertFromListxml(Models.Listxml.Configuration item)
        {
            var configuration = new Models.Internal.Configuration
            {
                [Models.Internal.Configuration.NameKey] = item.Name,
                [Models.Internal.Configuration.TagKey] = item.Tag,
                [Models.Internal.Configuration.MaskKey] = item.Mask,
            };

            if (item.Condition != null)
                configuration[Models.Internal.Configuration.ConditionKey] = ConvertFromListxml(item.Condition);

            if (item.ConfLocation != null && item.ConfLocation.Any())
            {
                var confLocations = new List<Models.Internal.ConfLocation>();
                foreach (var confLocation in item.ConfLocation)
                {
                    confLocations.Add(ConvertFromListxml(confLocation));
                }
                configuration[Models.Internal.Configuration.ConfLocationKey] = confLocations.ToArray();
            }

            if (item.ConfSetting != null && item.ConfSetting.Any())
            {
                var confSettings = new List<Models.Internal.ConfSetting>();
                foreach (var confSetting in item.ConfSetting)
                {
                    confSettings.Add(ConvertFromListxml(confSetting));
                }
                configuration[Models.Internal.Configuration.ConfSettingKey] = confSettings.ToArray();
            }

            return configuration;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.ConfLocation"/> to <cref="Models.Internal.ConfLocation"/>
        /// </summary>
        public static Models.Internal.ConfLocation ConvertFromListxml(Models.Listxml.ConfLocation item)
        {
            var confLocation = new Models.Internal.ConfLocation
            {
                [Models.Internal.ConfLocation.NameKey] = item.Name,
                [Models.Internal.ConfLocation.NumberKey] = item.Number,
                [Models.Internal.ConfLocation.InvertedKey] = item.Inverted,
            };
            return confLocation;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.ConfSetting"/> to <cref="Models.Internal.ConfSetting"/>
        /// </summary>
        public static Models.Internal.ConfSetting ConvertFromListxml(Models.Listxml.ConfSetting item)
        {
            var confSetting = new Models.Internal.ConfSetting
            {
                [Models.Internal.ConfSetting.NameKey] = item.Name,
                [Models.Internal.ConfSetting.ValueKey] = item.Value,
                [Models.Internal.ConfSetting.DefaultKey] = item.Default,
            };

            if (item.Condition != null)
                confSetting[Models.Internal.ConfSetting.ConditionKey] = ConvertFromListxml(item.Condition);

            return confSetting;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Control"/> to <cref="Models.Internal.Control"/>
        /// </summary>
        public static Models.Internal.Control ConvertFromListxml(Models.Listxml.Control item)
        {
            var control = new Models.Internal.Control
            {
                [Models.Internal.Control.TypeKey] = item.Type,
                [Models.Internal.Control.PlayerKey] = item.Player,
                [Models.Internal.Control.ButtonsKey] = item.Buttons,
                [Models.Internal.Control.ReqButtonsKey] = item.ReqButtons,
                [Models.Internal.Control.MinimumKey] = item.Minimum,
                [Models.Internal.Control.MaximumKey] = item.Maximum,
                [Models.Internal.Control.SensitivityKey] = item.Sensitivity,
                [Models.Internal.Control.KeyDeltaKey] = item.KeyDelta,
                [Models.Internal.Control.ReverseKey] = item.Reverse,
                [Models.Internal.Control.WaysKey] = item.Ways,
                [Models.Internal.Control.Ways2Key] = item.Ways2,
                [Models.Internal.Control.Ways3Key] = item.Ways3,
            };
            return control;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Device"/> to <cref="Models.Internal.Device"/>
        /// </summary>
        public static Models.Internal.Device ConvertFromListxml(Models.Listxml.Device item)
        {
            var device = new Models.Internal.Device
            {
                [Models.Internal.Device.TypeKey] = item.Type,
                [Models.Internal.Device.TagKey] = item.Tag,
                [Models.Internal.Device.FixedImageKey] = item.FixedImage,
                [Models.Internal.Device.MandatoryKey] = item.Mandatory,
                [Models.Internal.Device.InterfaceKey] = item.Interface,
            };

            if (item.Instance != null)
                device[Models.Internal.Device.InstanceKey] = ConvertFromListxml(item.Instance);

            if (item.Extension != null && item.Extension.Any())
            {
                var extensions = new List<Models.Internal.Extension>();
                foreach (var extension in item.Extension)
                {
                    extensions.Add(ConvertFromListxml(extension));
                }
                device[Models.Internal.Device.ExtensionKey] = extensions.ToArray();
            }

            return device;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.DeviceRef"/> to <cref="Models.Internal.DeviceRef"/>
        /// </summary>
        public static Models.Internal.DeviceRef ConvertFromListxml(Models.Listxml.DeviceRef item)
        {
            var deviceRef = new Models.Internal.DeviceRef
            {
                [Models.Internal.DeviceRef.NameKey] = item.Name,
            };
            return deviceRef;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.DipLocation"/> to <cref="Models.Internal.DipLocation"/>
        /// </summary>
        public static Models.Internal.DipLocation ConvertFromListxml(Models.Listxml.DipLocation item)
        {
            var dipLocation = new Models.Internal.DipLocation
            {
                [Models.Internal.DipLocation.NameKey] = item.Name,
                [Models.Internal.DipLocation.NumberKey] = item.Number,
                [Models.Internal.DipLocation.InvertedKey] = item.Inverted,
            };
            return dipLocation;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.DipSwitch"/> to <cref="Models.Internal.DipSwitch"/>
        /// </summary>
        public static Models.Internal.DipSwitch ConvertFromListxml(Models.Listxml.DipSwitch item)
        {
            var dipSwitch = new Models.Internal.DipSwitch
            {
                [Models.Internal.DipSwitch.NameKey] = item.Name,
                [Models.Internal.DipSwitch.TagKey] = item.Tag,
                [Models.Internal.DipSwitch.MaskKey] = item.Mask,
            };

            if (item.Condition != null)
                dipSwitch[Models.Internal.DipSwitch.ConditionKey] = ConvertFromListxml(item.Condition);

            if (item.DipLocation != null && item.DipLocation.Any())
            {
                var dipLocations = new List<Models.Internal.DipLocation>();
                foreach (var dipLocation in item.DipLocation)
                {
                    dipLocations.Add(ConvertFromListxml(dipLocation));
                }
                dipSwitch[Models.Internal.DipSwitch.DipLocationKey] = dipLocations.ToArray();
            }

            if (item.DipValue != null && item.DipValue.Any())
            {
                var dipValues = new List<Models.Internal.DipValue>();
                foreach (var dipValue in item.DipValue)
                {
                    dipValues.Add(ConvertFromListxml(dipValue));
                }
                dipSwitch[Models.Internal.DipSwitch.DipValueKey] = dipValues.ToArray();
            }

            return dipSwitch;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.DipValue"/> to <cref="Models.Internal.DipValue"/>
        /// </summary>
        public static Models.Internal.DipValue ConvertFromListxml(Models.Listxml.DipValue item)
        {
            var dipValue = new Models.Internal.DipValue
            {
                [Models.Internal.DipValue.NameKey] = item.Name,
                [Models.Internal.DipValue.ValueKey] = item.Value,
                [Models.Internal.DipValue.DefaultKey] = item.Default,
            };

            if (item.Condition != null)
                dipValue[Models.Internal.DipValue.ConditionKey] = ConvertFromListxml(item.Condition);

            return dipValue;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Disk"/> to <cref="Models.Internal.Disk"/>
        /// </summary>
        public static Models.Internal.Disk ConvertFromListxml(Models.Listxml.Disk item)
        {
            var disk = new Models.Internal.Disk
            {
                [Models.Internal.Disk.NameKey] = item.Name,
                [Models.Internal.Disk.MD5Key] = item.MD5,
                [Models.Internal.Disk.SHA1Key] = item.SHA1,
                [Models.Internal.Disk.MergeKey] = item.Merge,
                [Models.Internal.Disk.RegionKey] = item.Region,
                [Models.Internal.Disk.IndexKey] = item.Index,
                [Models.Internal.Disk.WritableKey] = item.Writable,
                [Models.Internal.Disk.StatusKey] = item.Status,
                [Models.Internal.Disk.OptionalKey] = item.Optional,
            };
            return disk;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Display"/> to <cref="Models.Internal.Display"/>
        /// </summary>
        public static Models.Internal.Display ConvertFromListxml(Models.Listxml.Display item)
        {
            var display = new Models.Internal.Display
            {
                [Models.Internal.Display.TagKey] = item.Tag,
                [Models.Internal.Display.TypeKey] = item.Type,
                [Models.Internal.Display.RotateKey] = item.Rotate,
                [Models.Internal.Display.FlipXKey] = item.FlipX,
                [Models.Internal.Display.WidthKey] = item.Width,
                [Models.Internal.Display.HeightKey] = item.Height,
                [Models.Internal.Display.RefreshKey] = item.Refresh,
                [Models.Internal.Display.PixClockKey] = item.PixClock,
                [Models.Internal.Display.HTotalKey] = item.HTotal,
                [Models.Internal.Display.HBEndKey] = item.HBEnd,
                [Models.Internal.Display.HBStartKey] = item.HBStart,
                [Models.Internal.Display.VTotalKey] = item.VTotal,
                [Models.Internal.Display.VBEndKey] = item.VBEnd,
                [Models.Internal.Display.VBStartKey] = item.VBStart,
            };
            return display;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Driver"/> to <cref="Models.Internal.Driver"/>
        /// </summary>
        public static Models.Internal.Driver ConvertFromListxml(Models.Listxml.Driver item)
        {
            var driver = new Models.Internal.Driver
            {
                [Models.Internal.Driver.StatusKey] = item.Status,
                [Models.Internal.Driver.ColorKey] = item.Color,
                [Models.Internal.Driver.SoundKey] = item.Sound,
                [Models.Internal.Driver.PaletteSizeKey] = item.PaletteSize,
                [Models.Internal.Driver.EmulationKey] = item.Emulation,
                [Models.Internal.Driver.CocktailKey] = item.Cocktail,
                [Models.Internal.Driver.SaveStateKey] = item.SaveState,
                [Models.Internal.Driver.RequiresArtworkKey] = item.RequiresArtwork,
                [Models.Internal.Driver.UnofficialKey] = item.Unofficial,
                [Models.Internal.Driver.NoSoundHardwareKey] = item.NoSoundHardware,
                [Models.Internal.Driver.IncompleteKey] = item.Incomplete,
            };
            return driver;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Extension"/> to <cref="Models.Internal.Extension"/>
        /// </summary>
        public static Models.Internal.Extension ConvertFromListxml(Models.Listxml.Extension item)
        {
            var extension = new Models.Internal.Extension
            {
                [Models.Internal.Extension.NameKey] = item.Name,
            };
            return extension;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Feature"/> to <cref="Models.Internal.Feature"/>
        /// </summary>
        public static Models.Internal.Feature ConvertFromListxml(Models.Listxml.Feature item)
        {
            var feature = new Models.Internal.Feature
            {
                [Models.Internal.Feature.TypeKey] = item.Type,
                [Models.Internal.Feature.StatusKey] = item.Status,
                [Models.Internal.Feature.OverallKey] = item.Overall,
            };
            return feature;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Input"/> to <cref="Models.Internal.Input"/>
        /// </summary>
        public static Models.Internal.Input ConvertFromListxml(Models.Listxml.Input item)
        {
            var input = new Models.Internal.Input
            {
                [Models.Internal.Input.ServiceKey] = item.Service,
                [Models.Internal.Input.TiltKey] = item.Tilt,
                [Models.Internal.Input.PlayersKey] = item.Players,
                [Models.Internal.Input.ControlKey] = item.ControlAttr,
                [Models.Internal.Input.ButtonsKey] = item.Buttons,
                [Models.Internal.Input.CoinsKey] = item.Coins,
            };

            if (item.Control != null && item.Control.Any())
            {
                var controls = new List<Models.Internal.Control>();
                foreach (var control in item.Control)
                {
                    controls.Add(ConvertFromListxml(control));
                }
                input[Models.Internal.Input.ControlKey] = controls.ToArray();
            }

            return input;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Instance"/> to <cref="Models.Internal.Instance"/>
        /// </summary>
        public static Models.Internal.Instance ConvertFromListxml(Models.Listxml.Instance item)
        {
            var instance = new Models.Internal.Instance
            {
                [Models.Internal.Instance.NameKey] = item.Name,
                [Models.Internal.Instance.BriefNameKey] = item.BriefName,
            };
            return instance;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Port"/> to <cref="Models.Internal.Port"/>
        /// </summary>
        public static Models.Internal.Port ConvertFromListxml(Models.Listxml.Port item)
        {
            var port = new Models.Internal.Port
            {
                [Models.Internal.Port.TagKey] = item.Tag,
            };

            if (item.Analog != null && item.Analog.Any())
            {
                var analogs = new List<Models.Internal.Analog>();
                foreach (var analog in item.Analog)
                {
                    analogs.Add(ConvertFromListxml(analog));
                }
                port[Models.Internal.Port.AnalogKey] = analogs.ToArray();
            }

            return port;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.RamOption"/> to <cref="Models.Internal.RamOption"/>
        /// </summary>
        public static Models.Internal.RamOption ConvertFromListxml(Models.Listxml.RamOption item)
        {
            var ramOption = new Models.Internal.RamOption
            {
                [Models.Internal.RamOption.NameKey] = item.Name,
                [Models.Internal.RamOption.DefaultKey] = item.Default,
            };
            return ramOption;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Rom"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Rom ConvertFromListxml(Models.Listxml.Rom item)
        {
            var rom = new Models.Internal.Rom
            {
                [Models.Internal.Rom.NameKey] = item.Name,
                [Models.Internal.Rom.BiosKey] = item.Bios,
                [Models.Internal.Rom.SizeKey] = item.Size,
                [Models.Internal.Rom.CRCKey] = item.CRC,
                [Models.Internal.Rom.SHA1Key] = item.SHA1,
                [Models.Internal.Rom.MergeKey] = item.Merge,
                [Models.Internal.Rom.RegionKey] = item.Region,
                [Models.Internal.Rom.OffsetKey] = item.Offset,
                [Models.Internal.Rom.StatusKey] = item.Status,
                [Models.Internal.Rom.OptionalKey] = item.Optional,
                [Models.Internal.Rom.DisposeKey] = item.Dispose,
                [Models.Internal.Rom.SoundOnlyKey] = item.SoundOnly,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Sample"/> to <cref="Models.Internal.Sample"/>
        /// </summary>
        public static Models.Internal.Sample ConvertFromListxml(Models.Listxml.Sample item)
        {
            var sample = new Models.Internal.Sample
            {
                [Models.Internal.Sample.NameKey] = item.Name,
            };
            return sample;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Slot"/> to <cref="Models.Internal.Slot"/>
        /// </summary>
        public static Models.Internal.Slot ConvertFromListxml(Models.Listxml.Slot item)
        {
            var slot = new Models.Internal.Slot
            {
                [Models.Internal.Slot.NameKey] = item.Name,
            };

            if (item.SlotOption != null && item.SlotOption.Any())
            {
                var slotOptions = new List<Models.Internal.SlotOption>();
                foreach (var slotOption in item.SlotOption)
                {
                    slotOptions.Add(ConvertFromListxml(slotOption));
                }
                slot[Models.Internal.Slot.SlotOptionKey] = slotOptions.ToArray();
            }

            return slot;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.SlotOption"/> to <cref="Models.Internal.SlotOption"/>
        /// </summary>
        public static Models.Internal.SlotOption ConvertFromListxml(Models.Listxml.SlotOption item)
        {
            var slotOption = new Models.Internal.SlotOption
            {
                [Models.Internal.SlotOption.NameKey] = item.Name,
                [Models.Internal.SlotOption.DevNameKey] = item.DevName,
                [Models.Internal.SlotOption.DefaultKey] = item.Default,
            };
            return slotOption;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.SoftwareList"/> to <cref="Models.Internal.SoftwareList"/>
        /// </summary>
        public static Models.Internal.SoftwareList ConvertFromListxml(Models.Listxml.SoftwareList item)
        {
            var softwareList = new Models.Internal.SoftwareList
            {
                [Models.Internal.SoftwareList.TagKey] = item.Tag,
                [Models.Internal.SoftwareList.NameKey] = item.Name,
                [Models.Internal.SoftwareList.StatusKey] = item.Status,
                [Models.Internal.SoftwareList.FilterKey] = item.Filter,
            };
            return softwareList;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Sound"/> to <cref="Models.Internal.Sound"/>
        /// </summary>
        public static Models.Internal.Sound ConvertFromListxml(Models.Listxml.Sound item)
        {
            var sound = new Models.Internal.Sound
            {
                [Models.Internal.Sound.ChannelsKey] = item.Channels,
            };
            return sound;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Video"/> to <cref="Models.Internal.Video"/>
        /// </summary>
        public static Models.Internal.Video ConvertFromListxml(Models.Listxml.Video item)
        {
            var video = new Models.Internal.Video
            {
                [Models.Internal.Video.ScreenKey] = item.Screen,
                [Models.Internal.Video.OrientationKey] = item.Orientation,
                [Models.Internal.Video.WidthKey] = item.Width,
                [Models.Internal.Video.HeightKey] = item.Height,
                [Models.Internal.Video.AspectXKey] = item.AspectX,
                [Models.Internal.Video.AspectYKey] = item.AspectY,
                [Models.Internal.Video.RefreshKey] = item.Refresh,
            };
            return video;
        }

        #endregion
        
        #region Logiqx

        /// <summary>
        /// Convert from <cref="Models.Logiqx.Archive"/> to <cref="Models.Internal.Archive"/>
        /// </summary>
        public static Models.Internal.Archive ConvertFromLogiqx(Models.Logiqx.Archive item)
        {
            var archive = new Models.Internal.Archive
            {
                [Models.Internal.Archive.NameKey] = item.Name,
            };
            return archive;
        }

        /// <summary>
        /// Convert from <cref="Models.Logiqx.BiosSet"/> to <cref="Models.Internal.BiosSet"/>
        /// </summary>
        public static Models.Internal.BiosSet ConvertFromLogiqx(Models.Logiqx.BiosSet item)
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
        /// Convert from <cref="Models.Logiqx.DeviceRef"/> to <cref="Models.Internal.DeviceRef"/>
        /// </summary>
        public static Models.Internal.DeviceRef ConvertFromLogiqx(Models.Logiqx.DeviceRef item)
        {
            var deviceRef = new Models.Internal.DeviceRef
            {
                [Models.Internal.DeviceRef.NameKey] = item.Name,
            };
            return deviceRef;
        }

        /// <summary>
        /// Convert from <cref="Models.Logiqx.Disk"/> to <cref="Models.Internal.Disk"/>
        /// </summary>
        public static Models.Internal.Disk ConvertFromLogiqx(Models.Logiqx.Disk item)
        {
            var disk = new Models.Internal.Disk
            {
                [Models.Internal.Disk.NameKey] = item.Name,
                [Models.Internal.Disk.MD5Key] = item.MD5,
                [Models.Internal.Disk.SHA1Key] = item.SHA1,
                [Models.Internal.Disk.MergeKey] = item.Merge,
                [Models.Internal.Disk.StatusKey] = item.Status,
                [Models.Internal.Disk.RegionKey] = item.Region,
            };
            return disk;
        }

        /// <summary>
        /// Convert from <cref="Models.Logiqx.Driver"/> to <cref="Models.Internal.Driver"/>
        /// </summary>
        public static Models.Internal.Driver ConvertFromLogiqx(Models.Logiqx.Driver item)
        {
            var driver = new Models.Internal.Driver
            {
                [Models.Internal.Driver.StatusKey] = item.Status,
                [Models.Internal.Driver.EmulationKey] = item.Emulation,
                [Models.Internal.Driver.CocktailKey] = item.Cocktail,
                [Models.Internal.Driver.SaveStateKey] = item.SaveState,
                [Models.Internal.Driver.RequiresArtworkKey] = item.RequiresArtwork,
                [Models.Internal.Driver.UnofficialKey] = item.Unofficial,
                [Models.Internal.Driver.NoSoundHardwareKey] = item.NoSoundHardware,
                [Models.Internal.Driver.IncompleteKey] = item.Incomplete,
            };
            return driver;
        }

        /// <summary>
        /// Convert from <cref="Models.Logiqx.Media"/> to <cref="Models.Internal.Media"/>
        /// </summary>
        public static Models.Internal.Media ConvertFromLogiqx(Models.Logiqx.Media item)
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
        /// Convert from <cref="Models.Logiqx.Release"/> to <cref="Models.Internal.Release"/>
        /// </summary>
        public static Models.Internal.Release ConvertFromLogiqx(Models.Logiqx.Release item)
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
        /// Convert from <cref="Models.Logiqx.Rom"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Rom ConvertFromLogiqx(Models.Logiqx.Rom item)
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
                [Models.Internal.Rom.SerialKey] = item.Serial,
                [Models.Internal.Rom.HeaderKey] = item.Header,
                [Models.Internal.Rom.DateKey] = item.Date,
                [Models.Internal.Rom.InvertedKey] = item.Inverted,
                [Models.Internal.Rom.MIAKey] = item.MIA,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Logiqx.Sample"/> to <cref="Models.Internal.Sample"/>
        /// </summary>
        public static Models.Internal.Sample ConvertFromLogiqx(Models.Logiqx.Sample item)
        {
            var sample = new Models.Internal.Sample
            {
                [Models.Internal.Sample.NameKey] = item.Name,
            };
            return sample;
        }

        /// <summary>
        /// Convert from <cref="Models.Logiqx.SoftwareList"/> to <cref="Models.Internal.SoftwareList"/>
        /// </summary>
        public static Models.Internal.SoftwareList ConvertFromLogiqx(Models.Logiqx.SoftwareList item)
        {
            var softwareList = new Models.Internal.SoftwareList
            {
                [Models.Internal.SoftwareList.TagKey] = item.Tag,
                [Models.Internal.SoftwareList.NameKey] = item.Name,
                [Models.Internal.SoftwareList.StatusKey] = item.Status,
                [Models.Internal.SoftwareList.FilterKey] = item.Filter,
            };
            return softwareList;
        }

        #endregion
    }
}