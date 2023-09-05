using System;
using System.IO;
using System.Linq;
using System.Text;
using SabreTools.IO.Writers;
using SabreTools.Models.ClrMamePro;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for ClrMamePro metadata files
    /// </summary>
    public partial class ClrMamePro
    {
        /// <summary>
        /// Serializes the defined type to a ClrMamePro metadata file
        /// </summary>
        /// <param name="metadataFile">Data to serialize</param>
        /// <param name="path">Path to the file to serialize to</param>
        /// <param name="quotes">Enable quotes on write, false otherwise</param>
        /// <returns>True on successful serialization, false otherwise</returns>
        public static bool SerializeToFile(MetadataFile? metadataFile, string path, bool quotes)
        {
            using var stream = SerializeToStream(metadataFile, quotes);
            if (stream == null)
                return false;

            using var fs = File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }

        /// <summary>
        /// Serializes the defined type to a stream
        /// </summary>
        /// <param name="metadataFile">Data to serialize</param>
        /// <param name="quotes">Enable quotes on write, false otherwise</param>
        /// <returns>Stream containing serialized data on success, null otherwise</returns>
        public static Stream? SerializeToStream(MetadataFile? metadataFile, bool quotes)
        {
            // If the metadata file is null
            if (metadataFile == null)
                return null;

            // Setup the writer and output
            var stream = new MemoryStream();
            var writer = new ClrMameProWriter(stream, Encoding.UTF8) { Quotes = quotes };

            // Write the header, if it exists
            WriteHeader(metadataFile.ClrMamePro, writer);

            // Write out the games, if they exist
            WriteGames(metadataFile.Game, writer);

            // Return the stream
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// Write header information to the current writer
        /// </summary>
        /// <param name="header">ClrMamePro representing the header information</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteHeader(Models.ClrMamePro.ClrMamePro? header, ClrMameProWriter writer)
        {
            // If the header information is missing, we can't do anything
            if (header == null)
                return;

            writer.WriteStartElement("clrmamepro");

            writer.WriteOptionalStandalone("name", header.Name);
            writer.WriteOptionalStandalone("description", header.Description);
            writer.WriteOptionalStandalone("rootdir", header.RootDir);
            writer.WriteOptionalStandalone("category", header.Category);
            writer.WriteOptionalStandalone("version", header.Version);
            writer.WriteOptionalStandalone("date", header.Date);
            writer.WriteOptionalStandalone("author", header.Author);
            writer.WriteOptionalStandalone("homepage", header.Homepage);
            writer.WriteOptionalStandalone("url", header.Url);
            writer.WriteOptionalStandalone("comment", header.Comment);
            writer.WriteOptionalStandalone("header", header.Header);
            writer.WriteOptionalStandalone("type", header.Type);
            writer.WriteOptionalStandalone("forcemerging", header.ForceMerging);
            writer.WriteOptionalStandalone("forcezipping", header.ForceZipping);
            writer.WriteOptionalStandalone("forcepacking", header.ForcePacking);

            writer.WriteEndElement(); // clrmamepro
            writer.Flush();
        }

        /// <summary>
        /// Write games information to the current writer
        /// </summary>
        /// <param name="games">Array of GameBase objects representing the games information</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteGames(GameBase[]? games, ClrMameProWriter writer)
        {
            // If the games information is missing, we can't do anything
            if (games == null || !games.Any())
                return;

            // Loop through and write out the games
            foreach (var game in games)
            {
                WriteGame(game, writer);
                writer.Flush();
            }
        }

        /// <summary>
        /// Write game information to the current writer
        /// </summary>
        /// <param name="game">GameBase object representing the game information</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteGame(GameBase game, ClrMameProWriter writer)
        {
            // If the game information is missing, we can't do anything
            if (game == null)
                return;

            switch (game)
            {
                case Game:
                    writer.WriteStartElement("game");
                    break;
                case Machine:
                    writer.WriteStartElement("machine");
                    break;
                case Resource:
                    writer.WriteStartElement("resource");
                    break;
                case Set:
                    writer.WriteStartElement(name: "set");
                    break;
            }

            // Write the standalone values
            writer.WriteRequiredStandalone("name", game.Name, throwOnError: true);
            writer.WriteOptionalStandalone("description", game.Description);
            writer.WriteOptionalStandalone("year", game.Year);
            writer.WriteOptionalStandalone("manufacturer", game.Manufacturer);
            writer.WriteOptionalStandalone("category", game.Category);
            writer.WriteOptionalStandalone("cloneof", game.CloneOf);
            writer.WriteOptionalStandalone("romof", game.RomOf);
            writer.WriteOptionalStandalone("sampleof", game.SampleOf);

            // Write the item values
            WriteReleases(game.Release, writer);
            WriteBiosSets(game.BiosSet, writer);
            WriteRoms(game.Rom, writer);
            WriteDisks(game.Disk, writer);
            WriteMedia(game.Media, writer);
            WriteSamples(game.Sample, writer);
            WriteArchives(game.Archive, writer);
            WriteChips(game.Chip, writer);
            WriteVideos(game.Video, writer);
            WriteSound(game.Sound, writer);
            WriteInput(game.Input, writer);
            WriteDipSwitches(game.DipSwitch, writer);
            WriteDriver(game.Driver, writer);

            writer.WriteEndElement(); // game, machine, resource, set
        }

        /// <summary>
        /// Write releases information to the current writer
        /// </summary>
        /// <param name="releases">Array of Release objects to write</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteReleases(Release[]? releases, ClrMameProWriter writer)
        {
            // If the array is missing, we can't do anything
            if (releases == null)
                return;

            foreach (var release in releases)
            {
                writer.WriteStartElement("release");
                writer.WriteRequiredAttributeString("name", release.Name, throwOnError: true);
                writer.WriteRequiredAttributeString("region", release.Region, throwOnError: true);
                writer.WriteOptionalAttributeString("language", release.Language);
                writer.WriteOptionalAttributeString("date", release.Date);
                writer.WriteOptionalAttributeString("default", release.Default);
                writer.WriteEndElement(); // release
            }
        }

        /// <summary>
        /// Write biossets information to the current writer
        /// </summary>
        /// <param name="biossets">Array of BiosSet objects to write</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteBiosSets(BiosSet[]? biossets, ClrMameProWriter writer)
        {
            // If the array is missing, we can't do anything
            if (biossets == null)
                return;

            foreach (var biosset in biossets)
            {
                writer.WriteStartElement("biosset");
                writer.WriteRequiredAttributeString("name", biosset.Name, throwOnError: true);
                writer.WriteRequiredAttributeString("description", biosset.Description, throwOnError: true);
                writer.WriteOptionalAttributeString("default", biosset.Default);
                writer.WriteEndElement(); // release
            }
        }

        /// <summary>
        /// Write roms information to the current writer
        /// </summary>
        /// <param name="roms">Array of Rom objects to write</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteRoms(Rom[]? roms, ClrMameProWriter writer)
        {
            // If the array is missing, we can't do anything
            if (roms == null)
                return;

            foreach (var rom in roms)
            {
                writer.WriteStartElement("rom");
                writer.WriteRequiredAttributeString("name", rom.Name, throwOnError: true);
                writer.WriteRequiredAttributeString("size", rom.Size, throwOnError: true);
                writer.WriteOptionalAttributeString("crc", rom.CRC);
                writer.WriteOptionalAttributeString("md5", rom.MD5);
                writer.WriteOptionalAttributeString("sha1", rom.SHA1);
                writer.WriteOptionalAttributeString("sha256", rom.SHA256);
                writer.WriteOptionalAttributeString("sha384", rom.SHA384);
                writer.WriteOptionalAttributeString("sha512", rom.SHA512);
                writer.WriteOptionalAttributeString("spamsum", rom.SpamSum);
                writer.WriteOptionalAttributeString("xxh3_64", rom.xxHash364);
                writer.WriteOptionalAttributeString("xxh3_128", rom.xxHash3128);
                writer.WriteOptionalAttributeString("merge", rom.Merge);
                writer.WriteOptionalAttributeString("status", rom.Status);
                writer.WriteOptionalAttributeString("region", rom.Region);
                writer.WriteOptionalAttributeString("flags", rom.Flags);
                writer.WriteOptionalAttributeString("offs", rom.Offs);
                writer.WriteOptionalAttributeString("serial", rom.Serial);
                writer.WriteOptionalAttributeString("header", rom.Header);
                writer.WriteOptionalAttributeString("date", rom.Date);
                writer.WriteOptionalAttributeString("inverted", rom.Inverted);
                writer.WriteOptionalAttributeString("mia", rom.MIA);
                writer.WriteEndElement(); // rom
            }
        }

        /// <summary>
        /// Write disks information to the current writer
        /// </summary>
        /// <param name="disks">Array of Disk objects to write</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteDisks(Disk[]? disks, ClrMameProWriter writer)
        {
            // If the array is missing, we can't do anything
            if (disks == null)
                return;

            foreach (var disk in disks)
            {
                writer.WriteStartElement("disk");
                writer.WriteRequiredAttributeString("name", disk.Name, throwOnError: true);
                writer.WriteOptionalAttributeString("md5", disk.MD5);
                writer.WriteOptionalAttributeString("sha1", disk.SHA1);
                writer.WriteOptionalAttributeString("merge", disk.Merge);
                writer.WriteOptionalAttributeString("status", disk.Status);
                writer.WriteOptionalAttributeString("flags", disk.Flags);
                writer.WriteEndElement(); // disk
            }
        }

        /// <summary>
        /// Write medias information to the current writer
        /// </summary>
        /// <param name="medias">Array of Media objects to write</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteMedia(Media[]? medias, ClrMameProWriter writer)
        {
            // If the array is missing, we can't do anything
            if (medias == null)
                return;

            foreach (var media in medias)
            {
                writer.WriteStartElement("media");
                writer.WriteRequiredAttributeString("name", media.Name, throwOnError: true);
                writer.WriteOptionalAttributeString("md5", media.MD5);
                writer.WriteOptionalAttributeString("sha1", media.SHA1);
                writer.WriteOptionalAttributeString("sha256", media.SHA256);
                writer.WriteOptionalAttributeString("spamsum", media.SpamSum);
                writer.WriteEndElement(); // media
            }
        }

        /// <summary>
        /// Write samples information to the current writer
        /// </summary>
        /// <param name="samples">Array of Sample objects to write</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteSamples(Sample[]? samples, ClrMameProWriter writer)
        {
            // If the array is missing, we can't do anything
            if (samples == null)
                return;

            foreach (var sample in samples)
            {
                writer.WriteStartElement("sample");
                writer.WriteRequiredAttributeString("name", sample.Name, throwOnError: true);
                writer.WriteEndElement(); // sample
            }
        }

        /// <summary>
        /// Write archives information to the current writer
        /// </summary>
        /// <param name="archives">Array of Archive objects to write</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteArchives(Archive[]? archives, ClrMameProWriter writer)
        {
            // If the array is missing, we can't do anything
            if (archives == null)
                return;

            foreach (var archive in archives)
            {
                writer.WriteStartElement("archive");
                writer.WriteRequiredAttributeString("name", archive.Name, throwOnError: true);
                writer.WriteEndElement(); // archive
            }
        }

        /// <summary>
        /// Write chips information to the current writer
        /// </summary>
        /// <param name="chips">Array of Chip objects to write</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteChips(Chip[]? chips, ClrMameProWriter writer)
        {
            // If the array is missing, we can't do anything
            if (chips == null)
                return;

            foreach (var chip in chips)
            {
                writer.WriteStartElement("chip");
                writer.WriteRequiredAttributeString("type", chip.Type, throwOnError: true);
                writer.WriteRequiredAttributeString("name", chip.Name, throwOnError: true);
                writer.WriteOptionalAttributeString("flags", chip.Flags);
                writer.WriteOptionalAttributeString("clock", chip.Clock);
                writer.WriteEndElement(); // chip
            }
        }

        /// <summary>
        /// Write video information to the current writer
        /// </summary>
        /// <param name="videos">Array of Video objects to write</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteVideos(Video[]? videos, ClrMameProWriter writer)
        {
            // If the item is missing, we can't do anything
            if (videos == null)
                return;

            foreach (var video in videos)
            {
                writer.WriteStartElement("video");
                writer.WriteRequiredAttributeString("screen", video.Screen, throwOnError: true);
                writer.WriteRequiredAttributeString("orientation", video.Orientation, throwOnError: true);
                writer.WriteOptionalAttributeString("x", video.X);
                writer.WriteOptionalAttributeString("y", video.Y);
                writer.WriteOptionalAttributeString("aspectx", video.AspectX);
                writer.WriteOptionalAttributeString("aspecty", video.AspectY);
                writer.WriteOptionalAttributeString("freq", video.Freq);
                writer.WriteEndElement(); // video
            }
        }

        /// <summary>
        /// Write sound information to the current writer
        /// </summary>
        /// <param name="sound">Sound object to write</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteSound(Sound? sound, ClrMameProWriter writer)
        {
            // If the item is missing, we can't do anything
            if (sound == null)
                return;

            writer.WriteStartElement("sound");
            writer.WriteRequiredAttributeString("channels", sound.Channels, throwOnError: true);
            writer.WriteEndElement(); // sound
        }

        /// <summary>
        /// Write input information to the current writer
        /// </summary>
        /// <param name="input">Input object to write</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteInput(Input? input, ClrMameProWriter writer)
        {
            // If the item is missing, we can't do anything
            if (input == null)
                return;

            writer.WriteStartElement("input");
            writer.WriteRequiredAttributeString("players", input.Players, throwOnError: true);
            writer.WriteOptionalAttributeString("control", input.Control);
            writer.WriteRequiredAttributeString("buttons", input.Buttons, throwOnError: true);
            writer.WriteOptionalAttributeString("coins", input.Coins);
            writer.WriteOptionalAttributeString("tilt", input.Tilt);
            writer.WriteOptionalAttributeString("service", input.Service);
            writer.WriteEndElement(); // input
        }

        /// <summary>
        /// Write dipswitches information to the current writer
        /// </summary>
        /// <param name="dipswitches">Array of DipSwitch objects to write</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteDipSwitches(DipSwitch[]? dipswitches, ClrMameProWriter writer)
        {
            // If the array is missing, we can't do anything
            if (dipswitches == null)
                return;

            foreach (var dipswitch in dipswitches)
            {
                writer.WriteStartElement("dipswitch");
                writer.WriteRequiredAttributeString("name", dipswitch.Name, throwOnError: true);
                foreach (var entry in dipswitch.Entry ?? Array.Empty<string>())
                {
                    writer.WriteRequiredAttributeString("entry", entry);
                }
                writer.WriteOptionalAttributeString("default", dipswitch.Default);
                writer.WriteEndElement(); // dipswitch
            }
        }

        /// <summary>
        /// Write driver information to the current writer
        /// </summary>
        /// <param name="driver">Driver object to write</param>
        /// <param name="writer">ClrMameProWriter representing the output</param>
        private static void WriteDriver(Driver? driver, ClrMameProWriter writer)
        {
            // If the item is missing, we can't do anything
            if (driver == null)
                return;

            writer.WriteStartElement("driver");
            writer.WriteRequiredAttributeString("status", driver.Status, throwOnError: true);
            writer.WriteOptionalAttributeString("color", driver.Color); // TODO: Probably actually required
            writer.WriteOptionalAttributeString("sound", driver.Sound); // TODO: Probably actually required
            writer.WriteOptionalAttributeString("palettesize", driver.PaletteSize);
            writer.WriteOptionalAttributeString("blit", driver.Blit);
            writer.WriteEndElement(); // driver
        }

        #region Internal

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.MetadataFile"/> to <cref="Models.Metadata.MetadataFile"/>
        /// </summary>
        public static Models.Metadata.MetadataFile? ConvertToInternalModel(MetadataFile? item)
        {
            if (item == null)
                return null;

            var metadataFile = new Models.Metadata.MetadataFile();

            if (item?.ClrMamePro != null)
                metadataFile[Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item.ClrMamePro);

            if (item?.Game != null && item.Game.Any())
            {
                metadataFile[Models.Metadata.MetadataFile.MachineKey] = item.Game
                    .Where(g => g != null)
                    .Select(ConvertMachineToInternalModel)
                    .ToArray();
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.ClrMamePro"/> to <cref="Models.Metadata.Header"/>
        /// </summary>
        private static Models.Metadata.Header ConvertHeaderToInternalModel(Models.ClrMamePro.ClrMamePro item)
        {
            var header = new Models.Metadata.Header
            {
                [Models.Metadata.Header.NameKey] = item.Name,
                [Models.Metadata.Header.DescriptionKey] = item.Description,
                [Models.Metadata.Header.RootDirKey] = item.RootDir,
                [Models.Metadata.Header.CategoryKey] = item.Category,
                [Models.Metadata.Header.VersionKey] = item.Version,
                [Models.Metadata.Header.DateKey] = item.Date,
                [Models.Metadata.Header.AuthorKey] = item.Author,
                [Models.Metadata.Header.HomepageKey] = item.Homepage,
                [Models.Metadata.Header.UrlKey] = item.Url,
                [Models.Metadata.Header.CommentKey] = item.Comment,
                [Models.Metadata.Header.HeaderKey] = item.Header,
                [Models.Metadata.Header.TypeKey] = item.Type,
                [Models.Metadata.Header.ForceMergingKey] = item.ForceMerging,
                [Models.Metadata.Header.ForceZippingKey] = item.ForceZipping,
                [Models.Metadata.Header.ForcePackingKey] = item.ForcePacking,
            };
            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.GameBase"/> to <cref="Models.Metadata.Machine"/>
        /// </summary>
        private static Models.Metadata.Machine ConvertMachineToInternalModel(GameBase item)
        {
            var machine = new Models.Metadata.Machine
            {
                [Models.Metadata.Machine.NameKey] = item.Name,
                [Models.Metadata.Machine.DescriptionKey] = item.Description,
                [Models.Metadata.Machine.YearKey] = item.Year,
                [Models.Metadata.Machine.ManufacturerKey] = item.Manufacturer,
                [Models.Metadata.Machine.CategoryKey] = item.Category,
                [Models.Metadata.Machine.CloneOfKey] = item.CloneOf,
                [Models.Metadata.Machine.RomOfKey] = item.RomOf,
                [Models.Metadata.Machine.SampleOfKey] = item.SampleOf,
            };

            if (item.Release != null && item.Release.Any())
            {
                machine[Models.Metadata.Machine.ReleaseKey] = item.Release
                    .Where(r => r != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            if (item.BiosSet != null && item.BiosSet.Any())
            {
                machine[Models.Metadata.Machine.BiosSetKey] = item.BiosSet
                    .Where(b => b != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            if (item.Rom != null && item.Rom.Any())
            {
                machine[Models.Metadata.Machine.RomKey] = item.Rom
                    .Where(r => r != null)
                .Select(ConvertToInternalModel)
                .ToArray();
            }

            if (item.Disk != null && item.Disk.Any())
            {
                machine[Models.Metadata.Machine.DiskKey] = item.Disk
                    .Where(d => d != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            if (item.Media != null && item.Media.Any())
            {
                machine[Models.Metadata.Machine.MediaKey] = item.Media
                    .Where(m => m != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            if (item.Sample != null && item.Sample.Any())
            {
                machine[Models.Metadata.Machine.SampleKey] = item.Sample
                    .Where(s => s != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            if (item.Archive != null && item.Archive.Any())
            {
                machine[Models.Metadata.Machine.ArchiveKey] = item.Archive
                    .Where(a => a != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            if (item.Chip != null && item.Chip.Any())
            {
                machine[Models.Metadata.Machine.ChipKey] = item.Chip
                    .Where(c => c != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            if (item.Video != null)
            {
                machine[Models.Metadata.Machine.VideoKey] = item.Video
                    .Where(v => v != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            if (item.Sound != null)
                machine[Models.Metadata.Machine.SoundKey] = ConvertToInternalModel(item.Sound);

            if (item.Input != null)
                machine[Models.Metadata.Machine.InputKey] = ConvertToInternalModel(item.Input);

            if (item.DipSwitch != null && item.DipSwitch.Any())
            {
                machine[Models.Metadata.Machine.DipSwitchKey] = item.DipSwitch
                    .Where(d => d != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            if (item.Driver != null)
                machine[Models.Metadata.Machine.DriverKey] = ConvertToInternalModel(item.Driver);

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Archive"/> to <cref="Models.Metadata.Archive"/>
        /// </summary>
        private static Models.Metadata.Archive ConvertToInternalModel(Archive item)
        {
            var archive = new Models.Metadata.Archive
            {
                [Models.Metadata.Archive.NameKey] = item.Name,
            };
            return archive;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.BiosSet"/> to <cref="Models.Metadata.BiosSet"/>
        /// </summary>
        private static Models.Metadata.BiosSet ConvertToInternalModel(BiosSet item)
        {
            var biosset = new Models.Metadata.BiosSet
            {
                [Models.Metadata.BiosSet.NameKey] = item.Name,
                [Models.Metadata.BiosSet.DescriptionKey] = item.Description,
                [Models.Metadata.BiosSet.DefaultKey] = item.Default,
            };
            return biosset;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Chip"/> to <cref="Models.Metadata.Chip"/>
        /// </summary>
        private static Models.Metadata.Chip ConvertToInternalModel(Chip item)
        {
            var chip = new Models.Metadata.Chip
            {
                [Models.Metadata.Chip.ChipTypeKey] = item.Type,
                [Models.Metadata.Chip.NameKey] = item.Name,
                [Models.Metadata.Chip.FlagsKey] = item.Flags,
                [Models.Metadata.Chip.ClockKey] = item.Clock,
            };
            return chip;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.DipSwitch"/> to <cref="Models.Metadata.DipSwitch"/>
        /// </summary>
        private static Models.Metadata.DipSwitch ConvertToInternalModel(DipSwitch item)
        {
            var dipswitch = new Models.Metadata.DipSwitch
            {
                [Models.Metadata.DipSwitch.NameKey] = item.Name,
                [Models.Metadata.DipSwitch.EntryKey] = item.Entry,
                [Models.Metadata.DipSwitch.DefaultKey] = item.Default,
            };
            return dipswitch;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Disk"/> to <cref="Models.Metadata.Disk"/>
        /// </summary>
        private static Models.Metadata.Disk ConvertToInternalModel(Disk item)
        {
            var disk = new Models.Metadata.Disk
            {
                [Models.Metadata.Disk.NameKey] = item.Name,
                [Models.Metadata.Disk.MD5Key] = item.MD5,
                [Models.Metadata.Disk.SHA1Key] = item.SHA1,
                [Models.Metadata.Disk.MergeKey] = item.Merge,
                [Models.Metadata.Disk.StatusKey] = item.Status,
                [Models.Metadata.Disk.FlagsKey] = item.Flags,
            };
            return disk;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Driver"/> to <cref="Models.Metadata.Driver"/>
        /// </summary>
        private static Models.Metadata.Driver ConvertToInternalModel(Driver item)
        {
            var driver = new Models.Metadata.Driver
            {
                [Models.Metadata.Driver.StatusKey] = item.Status,
                [Models.Metadata.Driver.ColorKey] = item.Color,
                [Models.Metadata.Driver.SoundKey] = item.Sound,
                [Models.Metadata.Driver.PaletteSizeKey] = item.PaletteSize,
                [Models.Metadata.Driver.BlitKey] = item.Blit,
            };
            return driver;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Input"/> to <cref="Models.Metadata.Input"/>
        /// </summary>
        private static Models.Metadata.Input ConvertToInternalModel(Input item)
        {
            var input = new Models.Metadata.Input
            {
                [Models.Metadata.Input.PlayersKey] = item.Players,
                [Models.Metadata.Input.ControlKey] = item.Control,
                [Models.Metadata.Input.ButtonsKey] = item.Buttons,
                [Models.Metadata.Input.CoinsKey] = item.Coins,
                [Models.Metadata.Input.TiltKey] = item.Tilt,
                [Models.Metadata.Input.ServiceKey] = item.Service,
            };
            return input;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Media"/> to <cref="Models.Metadata.Media"/>
        /// </summary>
        private static Models.Metadata.Media ConvertToInternalModel(Media item)
        {
            var media = new Models.Metadata.Media
            {
                [Models.Metadata.Media.NameKey] = item.Name,
                [Models.Metadata.Media.MD5Key] = item.MD5,
                [Models.Metadata.Media.SHA1Key] = item.SHA1,
                [Models.Metadata.Media.SHA256Key] = item.SHA256,
                [Models.Metadata.Media.SpamSumKey] = item.SpamSum,
            };
            return media;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Release"/> to <cref="Models.Metadata.Release"/>
        /// </summary>
        private static Models.Metadata.Release ConvertToInternalModel(Release item)
        {
            var release = new Models.Metadata.Release
            {
                [Models.Metadata.Release.NameKey] = item.Name,
                [Models.Metadata.Release.RegionKey] = item.Region,
                [Models.Metadata.Release.LanguageKey] = item.Language,
                [Models.Metadata.Release.DateKey] = item.Date,
                [Models.Metadata.Release.DefaultKey] = item.Default,
            };
            return release;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Rom"/> to <cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Models.Metadata.Rom ConvertToInternalModel(Rom item)
        {
            var rom = new Models.Metadata.Rom
            {
                [Models.Metadata.Rom.NameKey] = item.Name,
                [Models.Metadata.Rom.SizeKey] = item.Size,
                [Models.Metadata.Rom.CRCKey] = item.CRC,
                [Models.Metadata.Rom.MD5Key] = item.MD5,
                [Models.Metadata.Rom.SHA1Key] = item.SHA1,
                [Models.Metadata.Rom.SHA256Key] = item.SHA256,
                [Models.Metadata.Rom.SHA384Key] = item.SHA384,
                [Models.Metadata.Rom.SHA512Key] = item.SHA512,
                [Models.Metadata.Rom.SpamSumKey] = item.SpamSum,
                [Models.Metadata.Rom.xxHash364Key] = item.xxHash364,
                [Models.Metadata.Rom.xxHash3128Key] = item.xxHash3128,
                [Models.Metadata.Rom.MergeKey] = item.Merge,
                [Models.Metadata.Rom.StatusKey] = item.Status,
                [Models.Metadata.Rom.RegionKey] = item.Region,
                [Models.Metadata.Rom.FlagsKey] = item.Flags,
                [Models.Metadata.Rom.OffsetKey] = item.Offs,
                [Models.Metadata.Rom.SerialKey] = item.Serial,
                [Models.Metadata.Rom.HeaderKey] = item.Header,
                [Models.Metadata.Rom.DateKey] = item.Date,
                [Models.Metadata.Rom.InvertedKey] = item.Inverted,
                [Models.Metadata.Rom.MIAKey] = item.MIA,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Sample"/> to <cref="Models.Metadata.Sample"/>
        /// </summary>
        private static Models.Metadata.Sample ConvertToInternalModel(Sample item)
        {
            var sample = new Models.Metadata.Sample
            {
                [Models.Metadata.Sample.NameKey] = item.Name,
            };
            return sample;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Sound"/> to <cref="Models.Metadata.Sound"/>
        /// </summary>
        private static Models.Metadata.Sound ConvertToInternalModel(Sound item)
        {
            var sound = new Models.Metadata.Sound
            {
                [Models.Metadata.Sound.ChannelsKey] = item.Channels,
            };
            return sound;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Video"/> to <cref="Models.Metadata.Video"/>
        /// </summary>
        private static Models.Metadata.Video ConvertToInternalModel(Video item)
        {
            var video = new Models.Metadata.Video
            {
                [Models.Metadata.Video.ScreenKey] = item.Screen,
                [Models.Metadata.Video.OrientationKey] = item.Orientation,
                [Models.Metadata.Video.WidthKey] = item.X,
                [Models.Metadata.Video.HeightKey] = item.Y,
                [Models.Metadata.Video.AspectXKey] = item.AspectX,
                [Models.Metadata.Video.AspectYKey] = item.AspectY,
                [Models.Metadata.Video.RefreshKey] = item.Freq,
            };
            return video;
        }

        #endregion
    }
}