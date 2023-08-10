namespace SabreTools.Models.Internal
{
    /// <summary>
    /// Format-agnostic representation of game, machine, and set data
    /// </summary>
    public class Machine : DictionaryBase
    {
        #region Keys

        /// <remarks>Adjuster[]</remarks>
        public const string AdjusterKey = "adjuster";

        /// <remarks>Archive[]</remarks>
        public const string ArchiveKey = "archive";

        /// <remarks>BiosSet[]</remarks>
        public const string BiosSetKey = "biosset";

        /// <remarks>string</remarks>
        public const string BoardKey = "board";

        /// <remarks>string</remarks>
        public const string ButtonsKey = "buttons";

        /// <remarks>string, string[]</remarks>
        public const string CategoryKey = "category";

        /// <remarks>Chip[]</remarks>
        public const string ChipKey = "chip";

        /// <remarks>string</remarks>
        public const string CloneOfKey = "cloneof";

        /// <remarks>string</remarks>
        public const string CloneOfIdKey = "cloneofid";

        /// <remarks>string, string[]</remarks>
        public const string CommentKey = "comment";

        /// <remarks>string</remarks>
        public const string CompanyKey = "company";

        /// <remarks>Configuration[]</remarks>
        public const string ConfigurationKey = "configuration";

        /// <remarks>string</remarks>
        public const string ControlKey = "control";

        /// <remarks>string</remarks>
        public const string CountryKey = "country";

        /// <remarks>string</remarks>
        public const string DescriptionKey = "description";

        /// <remarks>Device[]</remarks>
        public const string DeviceKey = "device";

        /// <remarks>DeviceRef[]</remarks>
        public const string DeviceRefKey = "device_ref";

        /// <remarks>DipSwitch[]</remarks>
        public const string DipSwitchKey = "dipswitch";

        /// <remarks>string</remarks>
        public const string DirNameKey = "dirName";

        /// <remarks>Disk[]</remarks>
        public const string DiskKey = "disk";

        /// <remarks>string</remarks>
        public const string DisplayCountKey = "displaycount";

        /// <remarks>Display[]</remarks>
        public const string DisplayKey = "display";

        /// <remarks>string</remarks>
        public const string DisplayTypeKey = "displaytype";

        /// <remarks>Driver</remarks>
        public const string DriverKey = "driver";

        /// <remarks>Dump[]</remarks>
        public const string DumpKey = "dump";

        /// <remarks>string</remarks>
        public const string DuplicateIDKey = "duplicateID";

        /// <remarks>string</remarks>
        public const string EmulatorKey = "emulator";

        /// <remarks>string</remarks>
        public const string ExtraKey = "extra";

        /// <remarks>string</remarks>
        public const string FavoriteKey = "favorite";

        /// <remarks>Feature[]</remarks>
        public const string FeatureKey = "feature";

        /// <remarks>string</remarks>
        public const string GenMSXIDKey = "genmsxid";

        /// <remarks>string</remarks>
        public const string HistoryKey = "history";

        /// <remarks>string</remarks>
        public const string IdKey = "id";

        /// <remarks>string</remarks>
        public const string Im1CRCKey = "im1CRC";

        /// <remarks>string</remarks>
        public const string Im2CRCKey = "im2CRC";

        /// <remarks>string</remarks>
        public const string ImageNumberKey = "imageNumber";

        /// <remarks>Info[]</remarks>
        public const string InfoKey = "info";

        /// <remarks>Input</remarks>
        public const string InputKey = "input";

        /// <remarks>(yes|no) "no"</remarks>
        public const string IsBiosKey = "isbios";

        /// <remarks>(yes|no) "no"</remarks>
        public const string IsDeviceKey = "isdevice";

        /// <remarks>(yes|no) "no"</remarks>
        public const string IsMechanicalKey = "ismechanical";

        /// <remarks>string</remarks>
        public const string LanguageKey = "language";

        /// <remarks>string</remarks>
        public const string LocationKey = "location";

        /// <remarks>string</remarks>
        public const string ManufacturerKey = "manufacturer";

        /// <remarks>Media[]</remarks>
        public const string MediaKey = "media";

        /// <remarks>string</remarks>
        public const string NameKey = "name";

        /// <remarks>string</remarks>
        public const string NotesKey = "notes";

        /// <remarks>Part[]</remarks>
        public const string PartKey = "part";

        /// <remarks>string</remarks>
        public const string PlayedCountKey = "playedcount";

        /// <remarks>string</remarks>
        public const string PlayedTimeKey = "playedtime";

        /// <remarks>string</remarks>
        public const string PlayersKey = "players";

        /// <remarks>Port[]</remarks>
        public const string PortKey = "port";

        /// <remarks>string</remarks>
        public const string PublisherKey = "publisher";

        /// <remarks>RamOption[]</remarks>
        public const string RamOptionKey = "ramoption";

        /// <remarks>string</remarks>
        public const string RebuildToKey = "rebuildto";

        /// <remarks>Release[]</remarks>
        public const string ReleaseKey = "release";

        /// <remarks>string</remarks>
        public const string ReleaseNumberKey = "releaseNumber";

        /// <remarks>Rom[]</remarks>
        public const string RomKey = "rom";

        /// <remarks>string</remarks>
        public const string RomOfKey = "romof";

        /// <remarks>string</remarks>
        public const string RotationKey = "rotation";

        /// <remarks>(yes|no) "no"</remarks>
        public const string RunnableKey = "runnable";

        /// <remarks>Sample[]</remarks>
        public const string SampleKey = "sample";

        /// <remarks>string</remarks>
        public const string SampleOfKey = "sampleof";

        /// <remarks>string</remarks>
        public const string SaveTypeKey = "saveType";

        /// <remarks>SharedFeat[]</remarks>
        public const string SharedFeatKey = "sharedfeat";

        /// <remarks>Slot[]</remarks>
        public const string SlotKey = "slot";

        /// <remarks>SoftwareList[]</remarks>
        public const string SoftwareListKey = "softwarelist";

        /// <remarks>Sound</remarks>
        public const string SoundKey = "sound";

        /// <remarks>string</remarks>
        public const string SourceFileKey = "sourcefile";

        /// <remarks>string</remarks>
        public const string SourceRomKey = "sourceRom";

        /// <remarks>string</remarks>
        public const string StatusKey = "status";

        /// <remarks>(yes|partial|no) "yes"</remarks>
        public const string SupportedKey = "supported";

        /// <remarks>string</remarks>
        public const string SystemKey = "system";

        /// <remarks>string</remarks>
        public const string TagsKey = "tags";

        /// TODO: This needs an internal model OR mapping to fields
        /// <remarks>Trurip</remarks>
        public const string TruripKey = "trurip";

        /// <remarks>Video[]</remarks>
        public const string VideoKey = "video";

        /// <remarks>string</remarks>
        public const string YearKey = "year";

        #endregion
    }
}