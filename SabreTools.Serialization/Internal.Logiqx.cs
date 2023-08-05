namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for Logiqx models to internal structure
    /// </summary>
    public partial class Internal
    {
        #region Serialize

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
    }
}