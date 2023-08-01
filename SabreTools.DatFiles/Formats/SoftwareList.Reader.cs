using System;
using System.Collections.Generic;
using System.Linq;
using SabreTools.Core.Tools;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing a SoftwareList
    /// </summary>
    internal partial class SoftwareList : DatFile
    {
        /// <inheritdoc/>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            try
            {
                // Deserialize the input file
                var softwarelist = Serialization.SoftawreList.Deserialize(filename);

                // Convert the header to the internal format
                ConvertHeader(softwarelist, keep);

                // Convert the software data to the internal format
                ConvertSoftware(softwarelist?.Software, filename, indexId, statsOnly);
            }
            catch (Exception ex) when (!throwOnError)
            {
                string message = $"'{filename}' - An error occurred during parsing";
                logger.Error(ex, message);
            }
        }

        #region Converters

        /// <summary>
        /// Convert header information
        /// </summary>
        /// <param name="softwarelist">Deserialized model to convert</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        private void ConvertHeader(Models.SoftwareList.SoftwareList? softwarelist, bool keep)
        {
            // If the datafile is missing, we can't do anything
            if (softwarelist == null)
                return;

            Header.Name ??= softwarelist.Name;
            Header.Description ??= softwarelist.Description;
            Header.Comment ??= softwarelist.Notes;

            // Handle implied SuperDAT
            if (Header.Name.Contains(" - SuperDAT") && keep)
                Header.Type ??= "SuperDAT";
        }

        /// <summary>
        /// Convert software information
        /// </summary>
        /// <param name="software">Array of deserialized models to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertSoftware(Models.SoftwareList.Software[]? software, string filename, int indexId, bool statsOnly)
        {
            // If the game array is missing, we can't do anything
            if (software == null || !software.Any())
                return;

            // Loop through the software and add
            foreach (var sw in software)
            {
                ConvertSoftware(sw, filename, indexId, statsOnly);
            }
        }

        /// <summary>
        /// Convert software information
        /// </summary>
        /// <param name="software">Deserialized model to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertSoftware(Models.SoftwareList.Software software, string filename, int indexId, bool statsOnly)
        {
            // If the game is missing, we can't do anything
            if (software == null)
                return;

            // Create the machine for copying information
            var machine = new Machine
            {
                Name = software.Name,
                CloneOf = software.CloneOf,
                Supported = software.Supported.AsSupported(),
                Description = software.Description,
                Year = software.Year,
                Publisher = software.Publisher,
                Comment = software.Notes,
            };

            // Add all Info objects
            foreach (var info in software.Info ?? Array.Empty<Models.SoftwareList.Info>())
            {
                var infoItem = new Info
                {
                    Name = info.Name,
                    Value = info.Value,

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

                infoItem.CopyMachineInformation(machine);
                ParseAddHelper(infoItem, statsOnly);
            }

            // Add all SharedFeat objects
            foreach (var sharedfeat in software.SharedFeat ?? Array.Empty<Models.SoftwareList.SharedFeat>())
            {
                var sharedfeatItem = new SharedFeature
                {
                    Name = sharedfeat.Name,
                    Value = sharedfeat.Value,

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

                sharedfeatItem.CopyMachineInformation(machine);
                ParseAddHelper(sharedfeatItem, statsOnly);
            }

            // Check if there are any items
            bool containsItems = false;

            // Loop through each type of item
            ConvertPart(software.Part, machine, filename, indexId, statsOnly, ref containsItems);

            // If we had no items, create a Blank placeholder
            if (!containsItems)
            {
                var blank = new Blank
                {
                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

                blank.CopyMachineInformation(machine);
                ParseAddHelper(blank, statsOnly);
            }
        }

        /// <summary>
        /// Convert Part information
        /// </summary>
        /// <param name="parts">Array of deserialized models to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertPart(Models.SoftwareList.Part[]? parts, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the parts array is missing, we can't do anything
            if (parts == null || !parts.Any())
                return;

            foreach (var part in parts)
            {
                var item = new Part
                {
                    Name = part.Name,
                    Interface = part.Interface,
                    Features = CreateFeatures(part.Feature, machine, filename, indexId, statsOnly),

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

                item.CopyMachineInformation(machine);

                ConvertDataArea(part.DataArea, item, machine, filename, indexId, statsOnly, ref containsItems);
                ConvertDiskArea(part.DiskArea, item, machine, filename, indexId, statsOnly, ref containsItems);
                ConvertDipSwitch(part.DipSwitch, item, machine, filename, indexId, statsOnly, ref containsItems);
            }
        }

        /// <summary>
        /// Convert Feature information
        /// </summary>
        /// <param name="features">Array of deserialized models to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private static List<PartFeature>? CreateFeatures(Models.SoftwareList.Feature[]? features, Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the feature array is missing, we can't do anything
            if (features == null || !features.Any())
                return null;

            var partFeatures = new List<PartFeature>();
            foreach (var feature in features)
            {
                var item = new PartFeature
                {
                    Name = feature.Name,
                    Value = feature.Value,

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

                item.CopyMachineInformation(machine);
                partFeatures.Add(item);
            }

            return partFeatures;
        }

        /// <summary>
        /// Convert DataArea information
        /// </summary>
        /// <param name="dataareas">Array of deserialized models to convert</param>
        /// <param name="part">Parent Part to use</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertDataArea(Models.SoftwareList.DataArea[]? dataareas, Part part, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the dataarea array is missing, we can't do anything
            if (dataareas == null || !dataareas.Any())
                return;

            foreach (var dataarea in dataareas)
            {
                var item = new DataArea
                {
                    Name = dataarea.Name,
                    Size = Utilities.CleanLong(dataarea.Size),
                    Width = Utilities.CleanLong(dataarea.Width),
                    Endianness = dataarea.Endianness.AsEndianness(),

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

                item.CopyMachineInformation(machine);
                ConvertRoms(dataarea.Rom, part, item, machine, filename, indexId, statsOnly, ref containsItems);
            }
        }

        /// <summary>
        /// Convert Rom information
        /// </summary>
        /// <param name="roms">Array of deserialized models to convert</param>
        /// <param name="part">Parent Part to use</param>
        /// <param name="dataarea">Parent DataArea to use</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertRoms(Models.SoftwareList.Rom[]? roms, Part part, DataArea dataarea, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the rom array is missing, we can't do anything
            if (roms == null || !roms.Any())
                return;

            containsItems = true;
            foreach (var rom in roms)
            {
                var item = new Rom
                {
                    Name = rom.Name,
                    Size = Utilities.CleanLong(rom.Size ?? rom.Length),
                    CRC = rom.CRC,
                    SHA1 = rom.SHA1,
                    Offset = rom.Offset,
                    Value = rom.Value,
                    ItemStatus = rom.Status.AsItemStatus(),
                    LoadFlag = rom.LoadFlag.AsLoadFlag(),

                    Part = part,
                    DataArea = dataarea,

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

                item.CopyMachineInformation(machine);
                ParseAddHelper(item, statsOnly);
            }
        }

        /// <summary>
        /// Convert DiskArea information
        /// </summary>
        /// <param name="diskareas">Array of deserialized models to convert</param>
        /// <param name="part">Parent Part to use</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertDiskArea(Models.SoftwareList.DiskArea[]? diskareas, Part part, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the diskarea array is missing, we can't do anything
            if (diskareas == null || !diskareas.Any())
                return;

            foreach (var diskarea in diskareas)
            {
                var item = new DiskArea
                {
                    Name = diskarea.Name,

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

                item.CopyMachineInformation(machine);
                ConvertDisks(diskarea.Disk, part, item, machine, filename, indexId, statsOnly, ref containsItems);
            }
        }

        /// <summary>
        /// Convert Disk information
        /// </summary>
        /// <param name="disks">Array of deserialized models to convert</param>
        /// <param name="part">Parent Part to use</param>
        /// <param name="diskarea">Parent DiskArea to use</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertDisks(Models.SoftwareList.Disk[]? disks, Part part, DiskArea diskarea, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the rom array is missing, we can't do anything
            if (disks == null || !disks.Any())
                return;

            containsItems = true;
            foreach (var rom in disks)
            {
                var item = new Disk
                {
                    Name = rom.Name,
                    MD5 = rom.MD5,
                    SHA1 = rom.SHA1,
                    ItemStatus = rom.Status.AsItemStatus(),
                    Writable = rom.Writeable.AsYesNo(),

                    Part = part,
                    DiskArea = diskarea,

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

                item.CopyMachineInformation(machine);
                ParseAddHelper(item, statsOnly);
            }
        }

        /// <summary>
        /// Convert DipSwitch information
        /// </summary>
        /// <param name="dipswitches">Array of deserialized models to convert</param>
        /// <param name="part">Parent Part to use</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertDipSwitch(Models.SoftwareList.DipSwitch[]? dipswitches, Part part, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the dipswitch array is missing, we can't do anything
            if (dipswitches == null || !dipswitches.Any())
                return;

            foreach (var dipswitch in dipswitches)
            {
                var item = new DipSwitch
                {
                    Name = dipswitch.Name,
                    Tag = dipswitch.Tag,
                    Mask = dipswitch.Mask,
                    Values = CreateSettings(dipswitch.DipValue, machine, filename, indexId),
                    
                    Part = part,

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

                item.CopyMachineInformation(machine);
                ParseAddHelper(item, statsOnly);
            }
        }

        /// <summary>
        /// Convert DipValue information
        /// </summary>
        /// <param name="dipvalues">Array of deserialized models to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        private static List<Setting>? CreateSettings(Models.SoftwareList.DipValue[]? dipvalues, Machine machine, string filename, int indexId)
        {
            // If the feature array is missing, we can't do anything
            if (dipvalues == null || !dipvalues.Any())
                return null;

            var settings = new List<Setting>();
            foreach (var dipvalue in dipvalues)
            {
                var item = new Setting
                {
                    Name = dipvalue.Name,
                    Value = dipvalue.Value,
                    Default = dipvalue.Default.AsYesNo(),

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

                item.CopyMachineInformation(machine);
                settings.Add(item);
            }

            return settings;
        }

        #endregion
    }
}
