using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SabreTools.Models.Logiqx;

namespace SabreTools.Serialization
{
    /// <summary>
    /// XML serializer for Logiqx-derived metadata files
    /// </summary>
    public partial class Logiqx : XmlSerializer<Datafile>
    {
        /// <inheritdoc cref="SerializeToFile(Datafile, string, string?, string?, string?, string?)" />
        public static bool SerializeToFileWithDocType(Datafile obj, string path)
            => SerializeToFile(obj, path, DocTypeName, DocTypePubId, DocTypeSysId, DocTypeSysId);

        /// <inheritdoc cref="SerializeToStream(Datafile, string?, string?, string?, string?)" />
        public static Stream? SerializeToStreamWithDocType(Datafile obj, string path)
            => SerializeToStream(obj, DocTypeName, DocTypePubId, DocTypeSysId, DocTypeSysId);

        #region Internal

        /// <summary>
        /// Convert from <cref="Models.Logiqx.Datafile"/> to <cref="Models.Internal.MetadataFile"/>
        /// </summary>
        public static Models.Internal.MetadataFile? ConvertToInternalModel(Datafile? item)
        {
            if (item == null)
                return null;

            var metadataFile = new Models.Internal.MetadataFile
            {
                [Models.Internal.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item),
            };

            var machines = new List<Models.Internal.Machine>();

            if (item.Game != null && item.Game.Any())
            {
               machines.AddRange(item.Game
                    .Where(g => g != null)
                    .Select(ConvertMachineToInternalModel));
            }

            if (item.Dir != null && item.Dir.Any())
            {
                machines.AddRange(item.Dir
                    .Where(d => d != null)
                    .SelectMany(ConvertDirToInternalModel));
            }

            if (machines.Any())
                metadataFile[Models.Internal.MetadataFile.MachineKey] = machines.ToArray();

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.Logiqx.Datafile"/> to <cref="Models.Internal.Header"/>
        /// </summary>
        private static Models.Internal.Header ConvertHeaderToInternalModel(Datafile item)
        {
            var header = item.Header != null ? ConvertHeaderToInternalModel(item.Header) : new Models.Internal.Header();

            header[Models.Internal.Header.BuildKey] = item.Build;
            header[Models.Internal.Header.DebugKey] = item.Debug;
            header[Models.Internal.Header.SchemaLocationKey] = item.SchemaLocation;

            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.Logiqx.Header"/> to <cref="Models.Internal.Header"/>
        /// </summary>
        private static Models.Internal.Header ConvertHeaderToInternalModel(Header item)
        {
            var header = new Models.Internal.Header
            {
                [Models.Internal.Header.IdKey] = item.Id,
                [Models.Internal.Header.NameKey] = item.Name,
                [Models.Internal.Header.DescriptionKey] = item.Description,
                [Models.Internal.Header.RootDirKey] = item.RootDir,
                [Models.Internal.Header.CategoryKey] = item.Category,
                [Models.Internal.Header.VersionKey] = item.Version,
                [Models.Internal.Header.DateKey] = item.Date,
                [Models.Internal.Header.AuthorKey] = item.Author,
                [Models.Internal.Header.EmailKey] = item.Email,
                [Models.Internal.Header.HomepageKey] = item.Homepage,
                [Models.Internal.Header.UrlKey] = item.Url,
                [Models.Internal.Header.CommentKey] = item.Comment,
                [Models.Internal.Header.TypeKey] = item.Type,
            };

            if (item.ClrMamePro != null)
            {
                header[Models.Internal.Header.HeaderKey] = item.ClrMamePro.Header;
                header[Models.Internal.Header.ForceMergingKey] = item.ClrMamePro.ForceMerging;
                header[Models.Internal.Header.ForceNodumpKey] = item.ClrMamePro.ForceNodump;
                header[Models.Internal.Header.ForcePackingKey] = item.ClrMamePro.ForcePacking;
            }

            if (item.RomCenter != null)
            {
                header[Models.Internal.Header.PluginKey] = item.RomCenter.Plugin;
                header[Models.Internal.Header.RomModeKey] = item.RomCenter.RomMode;
                header[Models.Internal.Header.BiosModeKey] = item.RomCenter.BiosMode;
                header[Models.Internal.Header.SampleModeKey] = item.RomCenter.SampleMode;
                header[Models.Internal.Header.LockRomModeKey] = item.RomCenter.LockRomMode;
                header[Models.Internal.Header.LockBiosModeKey] = item.RomCenter.LockBiosMode;
                header[Models.Internal.Header.LockSampleModeKey] = item.RomCenter.LockSampleMode;
            }

            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.Logiqx.Dir"/> to an array of <cref="Models.Internal.Machine"/>
        /// </summary>
        private static Models.Internal.Machine[] ConvertDirToInternalModel(Dir item)
        {
            if (item.Game == null || !item.Game.Any())
                return Array.Empty<Models.Internal.Machine>();

            return item.Game
                .Where(g => g != null)
                .Select(game =>
                {
                    var machine = ConvertMachineToInternalModel(game);
                    machine[Models.Internal.Machine.DirNameKey] = item.Name;
                    return machine;
                })
                .ToArray();
        }

        /// <summary>
        /// Convert from <cref="Models.Logiqx.GameBase"/> to <cref="Models.Internal.Machine"/>
        /// </summary>
        private static Models.Internal.Machine ConvertMachineToInternalModel(GameBase item)
        {
            var machine = new Models.Internal.Machine
            {
                [Models.Internal.Machine.NameKey] = item.Name,
                [Models.Internal.Machine.SourceFileKey] = item.SourceFile,
                [Models.Internal.Machine.IsBiosKey] = item.IsBios,
                [Models.Internal.Machine.IsDeviceKey] = item.IsDevice,
                [Models.Internal.Machine.IsMechanicalKey] = item.IsMechanical,
                [Models.Internal.Machine.CloneOfKey] = item.CloneOf,
                [Models.Internal.Machine.RomOfKey] = item.RomOf,
                [Models.Internal.Machine.SampleOfKey] = item.SampleOf,
                [Models.Internal.Machine.BoardKey] = item.Board,
                [Models.Internal.Machine.RebuildToKey] = item.RebuildTo,
                [Models.Internal.Machine.IdKey] = item.Id,
                [Models.Internal.Machine.CloneOfIdKey] = item.CloneOfId,
                [Models.Internal.Machine.RunnableKey] = item.Runnable,
                [Models.Internal.Machine.CommentKey] = item.Comment,
                [Models.Internal.Machine.DescriptionKey] = item.Description,
                [Models.Internal.Machine.YearKey] = item.Year,
                [Models.Internal.Machine.ManufacturerKey] = item.Manufacturer,
                [Models.Internal.Machine.PublisherKey] = item.Publisher,
                [Models.Internal.Machine.CategoryKey] = item.Category,
                [Models.Internal.Machine.TruripKey] = item.Trurip,
            };

            if (item.Release != null && item.Release.Any())
                machine[Models.Internal.Machine.ReleaseKey] = item.Release.Select(ConvertToInternalModel).ToArray();

            if (item.BiosSet != null && item.BiosSet.Any())
                machine[Models.Internal.Machine.BiosSetKey] = item.BiosSet.Select(ConvertToInternalModel).ToArray();

            if (item.Rom != null && item.Rom.Any())
                machine[Models.Internal.Machine.RomKey] = item.Rom.Select(ConvertToInternalModel).ToArray();

            if (item.Disk != null && item.Disk.Any())
                machine[Models.Internal.Machine.DiskKey] = item.Disk.Select(ConvertToInternalModel).ToArray();

            if (item.Media != null && item.Media.Any())
                machine[Models.Internal.Machine.MediaKey] = item.Media.Select(ConvertToInternalModel).ToArray();

            if (item.DeviceRef != null && item.DeviceRef.Any())
                machine[Models.Internal.Machine.DeviceRefKey] = item.DeviceRef.Select(ConvertToInternalModel).ToArray();

            if (item.Sample != null && item.Sample.Any())
                machine[Models.Internal.Machine.SampleKey] = item.Sample.Select(ConvertToInternalModel).ToArray();

            if (item.Archive != null && item.Archive.Any())
                machine[Models.Internal.Machine.ArchiveKey] = item.Archive.Select(ConvertToInternalModel).ToArray();

            if (item.Driver != null)
                machine[Models.Internal.Machine.DriverKey] = ConvertToInternalModel(item.Driver);

            if (item.SoftwareList != null && item.SoftwareList.Any())
                machine[Models.Internal.Machine.SoftwareListKey] = item.SoftwareList.Select(ConvertToInternalModel).ToArray();

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.Logiqx.Archive"/> to <cref="Models.Internal.Archive"/>
        /// </summary>
        private static Models.Internal.Archive ConvertToInternalModel(Archive item)
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
        private static Models.Internal.BiosSet ConvertToInternalModel(BiosSet item)
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
        private static Models.Internal.DeviceRef ConvertToInternalModel(DeviceRef item)
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
        private static Models.Internal.Disk ConvertToInternalModel(Disk item)
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
        private static Models.Internal.Driver ConvertToInternalModel(Driver item)
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
        private static Models.Internal.Media ConvertToInternalModel(Media item)
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
        private static Models.Internal.Release ConvertToInternalModel(Release item)
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
        private static Models.Internal.Rom ConvertToInternalModel(Rom item)
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
        private static Models.Internal.Sample ConvertToInternalModel(Sample item)
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
        private static Models.Internal.SoftwareList ConvertToInternalModel(SoftwareList item)
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