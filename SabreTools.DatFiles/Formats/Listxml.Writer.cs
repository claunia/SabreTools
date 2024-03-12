using System;
using System.Collections.Generic;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents writing a MAME XML DAT
    /// </summary>
    internal partial class Listxml : DatFile
    {
        /// <inheritdoc/>
        protected override ItemType[] GetSupportedTypes()
        {
            return
            [
                ItemType.Adjuster,
                ItemType.BiosSet,
                ItemType.Chip,
                ItemType.Condition,
                ItemType.Configuration,
                ItemType.Device,
                ItemType.DeviceRef,
                ItemType.DipSwitch,
                ItemType.Disk,
                ItemType.Display,
                ItemType.Driver,
                ItemType.Feature,
                ItemType.Input,
                ItemType.Port,
                ItemType.RamOption,
                ItemType.Rom,
                ItemType.Sample,
                ItemType.Slot,
                ItemType.SoftwareList,
                ItemType.Sound,
            ];
        }

        /// <inheritdoc/>
        protected override List<string>? GetMissingRequiredFields(DatItem datItem)
        {
            var missingFields = new List<string>();
            switch (datItem)
            {
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
                        && string.IsNullOrEmpty(rom.GetStringFieldValue(Models.Metadata.Rom.SHA1Key)))
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

                case DeviceRef deviceref:
                    if (string.IsNullOrEmpty(deviceref.GetName()))
                        missingFields.Add(Models.Metadata.DeviceRef.NameKey);
                    break;

                case Sample sample:
                    if (string.IsNullOrEmpty(sample.GetName()))
                        missingFields.Add(Models.Metadata.Sample.NameKey);
                    break;

                case Chip chip:
                    if (string.IsNullOrEmpty(chip.GetName()))
                        missingFields.Add(Models.Metadata.Chip.NameKey);
                    if (chip.GetStringFieldValue(Models.Metadata.Chip.ChipTypeKey).AsEnumValue<ChipType>() == ChipType.NULL)
                        missingFields.Add(Models.Metadata.Chip.ChipTypeKey);
                    break;

                case Display display:
                    if (display.GetStringFieldValue(Models.Metadata.Display.DisplayTypeKey).AsEnumValue<DisplayType>() == DisplayType.NULL)
                        missingFields.Add(Models.Metadata.Display.DisplayTypeKey);
                    if (display.GetDoubleFieldValue(Models.Metadata.Display.RefreshKey) == null)
                        missingFields.Add(Models.Metadata.Display.RefreshKey);
                    break;

                case Sound sound:
                    if (sound.GetInt64FieldValue(Models.Metadata.Sound.ChannelsKey) == null)
                        missingFields.Add(Models.Metadata.Sound.ChannelsKey);
                    break;

                case Input input:
                    if (input.GetInt64FieldValue(Models.Metadata.Input.PlayersKey) == null)
                        missingFields.Add(Models.Metadata.Input.PlayersKey);
                    break;

                case DipSwitch dipswitch:
                    if (string.IsNullOrEmpty(dipswitch.GetName()))
                        missingFields.Add(Models.Metadata.DipSwitch.NameKey);
                    if (string.IsNullOrEmpty(dipswitch.GetStringFieldValue(Models.Metadata.DipSwitch.TagKey)))
                        missingFields.Add(Models.Metadata.DipSwitch.TagKey);
                    break;

                case Configuration configuration:
                    if (string.IsNullOrEmpty(configuration.GetName()))
                        missingFields.Add(Models.Metadata.Configuration.NameKey);
                    if (string.IsNullOrEmpty(configuration.GetStringFieldValue(Models.Metadata.Configuration.TagKey)))
                        missingFields.Add(Models.Metadata.Configuration.TagKey);
                    break;

                case Port port:
                    if (string.IsNullOrEmpty(port.GetStringFieldValue(Models.Metadata.Port.TagKey)))
                        missingFields.Add(Models.Metadata.Port.TagKey);
                    break;

                case Adjuster adjuster:
                    if (string.IsNullOrEmpty(adjuster.GetName()))
                        missingFields.Add(Models.Metadata.Adjuster.NameKey);
                    break;

                case Driver driver:
                    if (driver.GetStringFieldValue(Models.Metadata.Driver.StatusKey).AsEnumValue<SupportStatus>() == SupportStatus.NULL)
                        missingFields.Add(Models.Metadata.Driver.StatusKey);
                    if (driver.GetStringFieldValue(Models.Metadata.Driver.EmulationKey).AsEnumValue<SupportStatus>() == SupportStatus.NULL)
                        missingFields.Add(Models.Metadata.Driver.EmulationKey);
                    if (driver.GetStringFieldValue(Models.Metadata.Driver.CocktailKey).AsEnumValue<SupportStatus>() == SupportStatus.NULL)
                        missingFields.Add(Models.Metadata.Driver.CocktailKey);
                    if (driver.GetStringFieldValue(Models.Metadata.Driver.SaveStateKey).AsEnumValue<SupportStatus>() == SupportStatus.NULL)
                        missingFields.Add(Models.Metadata.Driver.SaveStateKey);
                    break;

                case Feature feature:
                    if (feature.GetStringFieldValue(Models.Metadata.Feature.FeatureTypeKey).AsEnumValue<FeatureType>() == FeatureType.NULL)
                        missingFields.Add(Models.Metadata.Feature.FeatureTypeKey);
                    break;

                case Device device:
                    if (device.GetStringFieldValue(Models.Metadata.Device.DeviceTypeKey).AsEnumValue<DeviceType>() != DeviceType.NULL)
                        missingFields.Add(Models.Metadata.Device.DeviceTypeKey);
                    break;

                case Slot slot:
                    if (string.IsNullOrEmpty(slot.GetName()))
                        missingFields.Add(Models.Metadata.Slot.NameKey);
                    break;

                case DatItems.Formats.SoftwareList softwarelist:
                    if (string.IsNullOrEmpty(softwarelist.GetStringFieldValue(Models.Metadata.SoftwareList.TagKey)))
                        missingFields.Add(Models.Metadata.SoftwareList.TagKey);
                    if (string.IsNullOrEmpty(softwarelist.GetName()))
                        missingFields.Add(Models.Metadata.SoftwareList.NameKey);
                    if (softwarelist.GetStringFieldValue(Models.Metadata.SoftwareList.StatusKey).AsEnumValue<SoftwareListStatus>() == SoftwareListStatus.None)
                        missingFields.Add(Models.Metadata.SoftwareList.StatusKey);
                    break;

                case RamOption ramoption:
                    if (string.IsNullOrEmpty(ramoption.GetName()))
                        missingFields.Add(Models.Metadata.RamOption.NameKey);
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
                var mame = new Serialization.CrossModel.Listxml().Deserialize(metadata);
                if (!(new Serialization.Files.Listxml().Serialize(mame, outfile)))
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
