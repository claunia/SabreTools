using System;
using System.Collections.Generic;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents writing of a ClrMamePro DAT
    /// </summary>
    internal partial class ClrMamePro : DatFile
    {
        /// <inheritdoc/>
        protected override ItemType[] GetSupportedTypes()
        {
            return
            [
                ItemType.Archive,
                ItemType.BiosSet,
                ItemType.Chip,
                ItemType.DipSwitch,
                ItemType.Disk,
                ItemType.Display,
                ItemType.Driver,
                ItemType.Input,
                ItemType.Media,
                ItemType.Release,
                ItemType.Rom,
                ItemType.Sample,
                ItemType.Sound,
            ];
        }

        /// <inheritdoc/>
        protected override List<string>? GetMissingRequiredFields(DatItem datItem)
        {
            var missingFields = new List<string>();
            switch (datItem)
            {
                case Release release:
                    if (string.IsNullOrEmpty(release.GetName()))
                        missingFields.Add(Models.Metadata.Release.NameKey);
                    if (string.IsNullOrEmpty(release.GetStringFieldValue(Models.Metadata.Release.RegionKey)))
                        missingFields.Add(Models.Metadata.Release.RegionKey);
                    break;

                case BiosSet biosset:
                    if (string.IsNullOrEmpty(biosset.GetName()))
                        missingFields.Add(Models.Metadata.BiosSet.NameKey);
                    if (string.IsNullOrEmpty(biosset.GetStringFieldValue(Models.Metadata.BiosSet.DescriptionKey)))
                        missingFields.Add(Models.Metadata.BiosSet.DescriptionKey);
                    break;

                case Rom rom:
                    if (string.IsNullOrEmpty(rom.GetName()))
                        missingFields.Add(Models.Metadata.Rom.NameKey);
                    if (rom.GetInt64FieldValue(Models.Metadata.Rom.SizeKey) == null || rom.GetInt64FieldValue(Models.Metadata.Rom.SizeKey) < 0)
                        missingFields.Add(Models.Metadata.Rom.SizeKey);
                    if (string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.CRCKey))
                        && string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.MD5Key))
                        && string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA1Key))
                        && string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA256Key))
                        && string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA384Key))
                        && string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA512Key))
                        && string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SpamSumKey)))
                    {
                        missingFields.Add(Models.Metadata.Rom.SHA1Key);
                    }
                    break;

                case Disk disk:
                    if (string.IsNullOrEmpty(disk.GetName()))
                        missingFields.Add(Models.Metadata.Disk.NameKey);
                    if (string.IsNullOrEmpty(disk.GetStringFieldValue(Models.Metadata.Disk.MD5Key))
                        && string.IsNullOrEmpty(disk.GetStringFieldValue(Models.Metadata.Disk.SHA1Key)))
                    {
                        missingFields.Add(Models.Metadata.Disk.SHA1Key);
                    }
                    break;

                case Sample sample:
                    if (string.IsNullOrEmpty(sample.GetName()))
                        missingFields.Add(Models.Metadata.Sample.NameKey);
                    break;

                case Archive archive:
                    if (string.IsNullOrEmpty(archive.GetName()))
                        missingFields.Add(Models.Metadata.Archive.NameKey);
                    break;

                case Chip chip:
                    if (chip.GetStringFieldValue(Models.Metadata.Chip.ChipTypeKey).AsEnumValue<ChipType>() == ChipType.NULL)
                        missingFields.Add(Models.Metadata.Chip.ChipTypeKey);
                    if (string.IsNullOrEmpty(chip.GetName()))
                        missingFields.Add(Models.Metadata.Chip.NameKey);
                    break;

                case Display display:
                    if (display.GetStringFieldValue(Models.Metadata.Display.DisplayTypeKey).AsEnumValue<DisplayType>() == DisplayType.NULL)
                        missingFields.Add(Models.Metadata.Display.DisplayTypeKey);
                    if (display.GetInt64FieldValue(Models.Metadata.Display.RotateKey) == null)
                        missingFields.Add(Models.Metadata.Display.RotateKey);
                    break;

                case Sound sound:
                    if (sound.GetInt64FieldValue(Models.Metadata.Sound.ChannelsKey) == null)
                        missingFields.Add(Models.Metadata.Sound.ChannelsKey);
                    break;

                case Input input:
                    if (input.GetInt64FieldValue(Models.Metadata.Input.PlayersKey) == null)
                        missingFields.Add(Models.Metadata.Input.PlayersKey);
                    if (!input.ControlsSpecified)
                        missingFields.Add(Models.Metadata.Input.ControlKey);
                    break;

                case DipSwitch dipswitch:
                    if (string.IsNullOrEmpty(dipswitch.GetName()))
                        missingFields.Add(Models.Metadata.DipSwitch.NameKey);
                    break;

                case Driver driver:
                    if (driver.GetStringFieldValue(Models.Metadata.Driver.StatusKey).AsEnumValue<SupportStatus>() == SupportStatus.NULL)
                        missingFields.Add(Models.Metadata.Driver.StatusKey);
                    if (driver.GetStringFieldValue(Models.Metadata.Driver.EmulationKey).AsEnumValue<SupportStatus>() == SupportStatus.NULL)
                        missingFields.Add(Models.Metadata.Driver.EmulationKey);
                    break;
            }

            return missingFields;
        }

        /// <inheritdoc/>
        public override bool WriteToFile(string outfile, bool ignoreblanks = false, bool throwOnError = false)
        {
            try
            {
                logger.User($"Writing to '{outfile}'...");

                // Serialize the input file
                var metadata = ConvertMetadata(ignoreblanks);
                var metadataFile = new Serialization.CrossModel.ClrMamePro().Deserialize(metadata);
                if (!(new Serialization.Files.ClrMamePro().Serialize(metadataFile, outfile, Quotes)))
                {
                    logger.Warning($"File '{outfile}' could not be written! See the log for more details.");
                    return false;
                }
            }
            catch (Exception ex) when (!throwOnError)
            {
                logger.Error(ex);
                return false;
            }

            logger.User($"'{outfile}' written!{Environment.NewLine}");
            return true;
        }
    }
}
