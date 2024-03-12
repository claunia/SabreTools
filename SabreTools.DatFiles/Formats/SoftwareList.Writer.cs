using System;
using System.Collections.Generic;
using System.Linq;
using SabreTools.Core;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents writing a SoftwareList
    /// </summary>
    internal partial class SoftwareList : DatFile
    {
        /// <inheritdoc/>
        protected override ItemType[] GetSupportedTypes()
        {
            return
            [
                ItemType.DipSwitch,
                ItemType.Disk,
                ItemType.Info,
                ItemType.PartFeature,
                ItemType.Rom,
                ItemType.SharedFeat,
            ];
        }

        /// <inheritdoc/>
        protected override List<string>? GetMissingRequiredFields(DatItem datItem)
        {
            var missingFields = new List<string>();

            switch (datItem)
            {
                case DipSwitch dipSwitch:
                    if (!dipSwitch.PartSpecified)
                    {
                        missingFields.Add(Models.Metadata.Part.NameKey);
                        missingFields.Add(Models.Metadata.Part.InterfaceKey);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(dipSwitch.GetFieldValue<Part?>(DipSwitch.PartKey)!.GetName()))
                            missingFields.Add(Models.Metadata.Part.NameKey);
                        if (string.IsNullOrEmpty(dipSwitch.GetFieldValue<Part?>(DipSwitch.PartKey)!.GetStringFieldValue(Models.Metadata.Part.InterfaceKey)))
                            missingFields.Add(Models.Metadata.Part.InterfaceKey);
                    }
                    if (string.IsNullOrEmpty(dipSwitch.GetName()))
                        missingFields.Add(Models.Metadata.DipSwitch.NameKey);
                    if (string.IsNullOrEmpty(dipSwitch.GetStringFieldValue(Models.Metadata.DipSwitch.TagKey)))
                        missingFields.Add(Models.Metadata.DipSwitch.TagKey);
                    if (string.IsNullOrEmpty(dipSwitch.GetStringFieldValue(Models.Metadata.DipSwitch.MaskKey)))
                        missingFields.Add(Models.Metadata.DipSwitch.MaskKey);
                    if (dipSwitch.ValuesSpecified)
                    {
                        if (dipSwitch.GetFieldValue<DipValue[]?>(Models.Metadata.DipSwitch.DipValueKey)!.Any(dv => string.IsNullOrEmpty(dv.GetName())))
                            missingFields.Add(Models.Metadata.DipValue.NameKey);
                        if (dipSwitch.GetFieldValue<DipValue[]?>(Models.Metadata.DipSwitch.DipValueKey)!.Any(dv => string.IsNullOrEmpty(dv.GetStringFieldValue(Models.Metadata.DipValue.ValueKey))))
                            missingFields.Add(Models.Metadata.DipValue.ValueKey);
                    }

                    break;

                case Disk disk:
                    if (!disk.PartSpecified)
                    {
                        missingFields.Add(Models.Metadata.Part.NameKey);
                        missingFields.Add(Models.Metadata.Part.InterfaceKey);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(disk.GetFieldValue<Part?>(Disk.PartKey)!.GetName()))
                            missingFields.Add(Models.Metadata.Part.NameKey);
                        if (string.IsNullOrEmpty(disk.GetFieldValue<Part?>(Disk.PartKey)!.GetStringFieldValue(Models.Metadata.Part.InterfaceKey)))
                            missingFields.Add(Models.Metadata.Part.InterfaceKey);
                    }
                    if (!disk.DiskAreaSpecified)
                    {
                        missingFields.Add(Models.Metadata.DiskArea.NameKey);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(disk.GetFieldValue<DiskArea?>(Disk.DiskAreaKey)!.GetName()))
                            missingFields.Add(Models.Metadata.DiskArea.NameKey);
                    }
                    if (string.IsNullOrEmpty(disk.GetName()))
                        missingFields.Add(Models.Metadata.Disk.NameKey);
                    break;

                case Info info:
                    if (string.IsNullOrEmpty(info.GetName()))
                        missingFields.Add(Models.Metadata.Info.NameKey);
                    break;

                case Rom rom:
                    if (!rom.PartSpecified)
                    {
                        missingFields.Add(Models.Metadata.Part.NameKey);
                        missingFields.Add(Models.Metadata.Part.InterfaceKey);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(rom.GetFieldValue<Part?>(Rom.PartKey)!.GetName()))
                            missingFields.Add(Models.Metadata.Part.NameKey);
                        if (string.IsNullOrEmpty(rom.GetFieldValue<Part?>(Rom.PartKey)!.GetStringFieldValue(Models.Metadata.Part.InterfaceKey)))
                            missingFields.Add(Models.Metadata.Part.InterfaceKey);
                    }
                    if (!rom.DataAreaSpecified)
                    {
                        missingFields.Add(Models.Metadata.DataArea.NameKey);
                        missingFields.Add(Models.Metadata.DataArea.SizeKey);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(rom.GetFieldValue<DataArea?>(Rom.DataAreaKey)!.GetName()))
                            missingFields.Add(Models.Metadata.DataArea.NameKey);
                        if (rom.GetFieldValue<DataArea?>(Rom.DataAreaKey)!.GetInt64FieldValue(Models.Metadata.DataArea.SizeKey) == null)
                            missingFields.Add(Models.Metadata.DataArea.SizeKey);
                    }
                    break;

                case SharedFeat sharedFeat:
                    if (string.IsNullOrEmpty(sharedFeat.GetName()))
                        missingFields.Add(Models.Metadata.SharedFeat.NameKey);
                    break;
                default:
                    // Unsupported ItemTypes should be caught already
                    return null;
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
                var softwarelist = new Serialization.CrossModel.SoftwareList().Deserialize(metadata);
                if (!(new Serialization.Files.SoftwareList().SerializeToFileWithDocType(softwarelist!, outfile)))
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
