using System;
using System.Collections.Generic;
using System.Linq;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing a MAME XML DAT
    /// </summary>
    internal partial class Listxml : DatFile
    {
        /// <inheritdoc/>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            try
            {
                // Deserialize the input file
                // TODO: Support M1 DATs again
                var mame = new Serialization.Files.Listxml().Deserialize(filename);

                // Convert the header to the internal format
                ConvertHeader(mame, keep);

                // Convert the game data to the internal format
                ConvertGames(mame?.Game, filename, indexId, statsOnly);
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
        /// <param name="mame">Deserialized model to convert</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        private void ConvertHeader(Models.Listxml.Mame? mame, bool keep)
        {
            // If the mame is missing, we can't do anything
            if (mame == null)
                return;

            Header.Name ??= mame.Build;
            Header.Description ??= mame.Build;
            Header.Build ??= mame.Build;
            Header.Debug ??= mame.Debug.AsYesNo();
            Header.MameConfig ??= mame.MameConfig;

            // Handle implied SuperDAT
            if (Header.Name?.Contains(" - SuperDAT") == true && keep)
                Header.Type ??= "SuperDAT";
        }

        /// <summary>
        /// Convert games information
        /// </summary>
        /// <param name="games">Array of deserialized models to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertGames(Models.Listxml.GameBase[]? games, string filename, int indexId, bool statsOnly)
        {
            // If the game array is missing, we can't do anything
            if (games == null || !games.Any())
                return;

            // Loop through the games and add
            foreach (var game in games)
            {
                ConvertGame(game, filename, indexId, statsOnly);
            }
        }

        /// <summary>
        /// Convert game information
        /// </summary>
        /// <param name="game">Deserialized model to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ConvertGame(Models.Listxml.GameBase game, string filename, int indexId, bool statsOnly)
        {
            // If the game is missing, we can't do anything
            if (game == null)
                return;

            // Create the machine for copying information
            var machine = new Machine
            {
                Name = game.Name,
                SourceFile = game.SourceFile,
                Runnable = game.Runnable.AsRunnable(),
                CloneOf = game.CloneOf,
                RomOf = game.RomOf,
                SampleOf = game.SampleOf,
                Description = game.Description,
                Year = game.Year,
                Manufacturer = game.Manufacturer,
                History = game.History,
            };

            if (game.IsBios.AsYesNo() == true)
                machine.MachineType |= MachineType.Bios;
            if (game.IsDevice.AsYesNo() == true)
                machine.MachineType |= MachineType.Device;
            if (game.IsMechanical.AsYesNo() == true)
                machine.MachineType |= MachineType.Mechanical;

            // Check if there are any items
            bool containsItems = false;

            // Loop through each type of item
            ConvertBiosSets(game.BiosSet, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertRoms(game.Rom, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertDisks(game.Disk, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertDeviceRefs(game.DeviceRef, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertSamples(game.Sample, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertChips(game.Chip, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertDisplays(game.Display, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertVideos(game.Video, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertSound(game.Sound, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertInput(game.Input, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertDipSwitches(game.DipSwitch, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertConfigurations(game.Configuration, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertPorts(game.Port, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertAdjusters(game.Adjuster, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertDriver(game.Driver, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertFeatures(game.Feature, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertDevices(game.Device, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertSlots(game.Slot, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertSoftwareLists(game.SoftwareList, machine, filename, indexId, statsOnly, ref containsItems);
            ConvertRamOptions(game.RamOption, machine, filename, indexId, statsOnly, ref containsItems);

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
        /// Convert BiosSet information
        /// </summary>
        /// <param name="biossets">Array of deserialized models to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertBiosSets(Models.Listxml.BiosSet[]? biossets, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the BiosSet array is missing, we can't do anything
            if (biossets == null || !biossets.Any())
                return;

            containsItems = true;
            foreach (var biosset in biossets)
            {
                var item = new BiosSet
                {
                    Name = biosset.Name,
                    Description = biosset.Description,
                    Default = biosset.Default?.AsYesNo(),

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
        /// Convert Rom information
        /// </summary>
        /// <param name="roms">Array of deserialized models to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertRoms(Models.Listxml.Rom[]? roms, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the Rom array is missing, we can't do anything
            if (roms == null || !roms.Any())
                return;

            containsItems = true;
            foreach (var rom in roms)
            {
                var item = new Rom
                {
                    Name = rom.Name,
                    Bios = rom.Bios,
                    Size = NumberHelper.ConvertToInt64(rom.Size),
                    CRC = rom.CRC,
                    SHA1 = rom.SHA1,
                    MergeTag = rom.Merge,
                    Region = rom.Region,
                    Offset = rom.Offset,
                    ItemStatus = rom.Status.AsItemStatus(),
                    Optional = rom.Optional.AsYesNo(),
                    //Dispose = rom.Dispose.AsYesNo(), // TODO: Add to internal model
                    //SoundOnly = rom.SoundOnly.AsYesNo(), // TODO: Add to internal model

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
        /// Convert Disk information
        /// </summary>
        /// <param name="disks">Array of deserialized models to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertDisks(Models.Listxml.Disk[]? disks, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the Disk array is missing, we can't do anything
            if (disks == null || !disks.Any())
                return;

            containsItems = true;
            foreach (var disk in disks)
            {
                var item = new Disk
                {
                    Name = disk.Name,
                    MD5 = disk.MD5,
                    SHA1 = disk.SHA1,
                    MergeTag = disk.Merge,
                    Region = disk.Region,
                    Index = disk.Index,
                    Writable = disk.Writable.AsYesNo(),
                    ItemStatus = disk.Status.AsItemStatus(),
                    Optional = disk.Optional.AsYesNo(),

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
        /// Convert DeviceRef information
        /// </summary>
        /// <param name="devicerefs">Array of deserialized models to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertDeviceRefs(Models.Listxml.DeviceRef[]? devicerefs, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the DeviceRef array is missing, we can't do anything
            if (devicerefs == null || !devicerefs.Any())
                return;

            containsItems = true;
            foreach (var deviceref in devicerefs)
            {
                var item = new DeviceReference
                {
                    Name = deviceref.Name,

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
        /// Convert Sample information
        /// </summary>
        /// <param name="samples">Array of deserialized models to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertSamples(Models.Listxml.Sample[]? samples, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the Sample array is missing, we can't do anything
            if (samples == null || !samples.Any())
                return;

            containsItems = true;
            foreach (var sample in samples)
            {
                var item = new Sample
                {
                    Name = sample.Name,

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
        /// Convert Chip information
        /// </summary>
        /// <param name="chips">Array of deserialized models to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertChips(Models.Listxml.Chip[]? chips, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the Chip array is missing, we can't do anything
            if (chips == null || !chips.Any())
                return;

            containsItems = true;
            foreach (var chip in chips)
            {
                var item = new Chip
                {
                    Name = chip.Name,
                    Tag = chip.Tag,
                    ChipType = chip.Type.AsChipType(),
                    //SoundOnly = chip.SoundOnly, // TODO: Add to internal model
                    Clock = NumberHelper.ConvertToInt64(chip.Clock),

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
        /// Convert Display information
        /// </summary>
        /// <param name="displays">Array of deserialized models to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertDisplays(Models.Listxml.Display[]? displays, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the Display array is missing, we can't do anything
            if (displays == null || !displays.Any())
                return;

            containsItems = true;
            foreach (var display in displays)
            {
                var item = new Display
                {
                    Tag = display.Tag,
                    DisplayType = display.Type.AsDisplayType(),
                    Rotate = NumberHelper.ConvertToInt64(display.Rotate),
                    FlipX = display.FlipX.AsYesNo(),
                    Width = NumberHelper.ConvertToInt64(display.Width),
                    Height = NumberHelper.ConvertToInt64(display.Height),
                    Refresh = NumberHelper.ConvertToDouble(display.Refresh),
                    PixClock = NumberHelper.ConvertToInt64(display.PixClock),
                    HTotal = NumberHelper.ConvertToInt64(display.HTotal),
                    HBEnd = NumberHelper.ConvertToInt64(display.HBEnd),
                    HBStart = NumberHelper.ConvertToInt64(display.HBStart),
                    VTotal = NumberHelper.ConvertToInt64(display.VTotal),
                    VBEnd = NumberHelper.ConvertToInt64(display.VBEnd),
                    VBStart = NumberHelper.ConvertToInt64(display.VBStart),

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
        /// Convert Video information
        /// </summary>
        /// <param name="videos">Array of deserialized models to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertVideos(Models.Listxml.Video[]? videos, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the Video array is missing, we can't do anything
            if (videos == null || !videos.Any())
                return;

            containsItems = true;
            foreach (var video in videos)
            {
                var item = new Display
                {
                    DisplayType = video.Screen?.AsDisplayType() ?? DisplayType.NULL,
                    Width = NumberHelper.ConvertToInt64(video.Width),
                    Height = NumberHelper.ConvertToInt64(video.Height),
                    //AspectX = video.AspectX, // TODO: Add to internal model or find mapping
                    //AspectY = video.AspectY, // TODO: Add to internal model or find mapping
                    Refresh = NumberHelper.ConvertToDouble(video.Refresh),

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

                switch (video.Orientation)
                {
                    case "horizontal":
                        item.Rotate = 0;
                        break;
                    case "vertical":
                        item.Rotate = 90;
                        break;
                }

                item.CopyMachineInformation(machine);
                ParseAddHelper(item, statsOnly);
            }
        }

        /// <summary>
        /// Convert Sound information
        /// </summary>
        /// <param name="sound">Deserialized model to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertSound(Models.Listxml.Sound? sound, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the Sound is missing, we can't do anything
            if (sound == null)
                return;

            containsItems = true;
            var item = new Sound
            {
                Channels = NumberHelper.ConvertToInt64(sound.Channels),

                Source = new Source
                {
                    Index = indexId,
                    Name = filename,
                },
            };

            item.CopyMachineInformation(machine);
            ParseAddHelper(item, statsOnly);
        }

        /// <summary>
        /// Convert Input information
        /// </summary>
        /// <param name="input">Deserialized model to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertInput(Models.Listxml.Input? input, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the Input is missing, we can't do anything
            if (input == null)
                return;

            containsItems = true;
            var item = new Input
            {
                Service = input.Service.AsYesNo(),
                Tilt = input.Tilt.AsYesNo(),
                Players = NumberHelper.ConvertToInt64(input.Players),
                //ControlAttr = input.ControlAttr, // TODO: Add to internal model
                //Buttons = input.Buttons, // TODO: Add to internal model
                Coins = NumberHelper.ConvertToInt64(input.Coins),

                Source = new Source
                {
                    Index = indexId,
                    Name = filename,
                },
            };

            var controls = new List<Control>();
            foreach (var control in input.Control ?? Array.Empty<Models.Listxml.Control>())
            {
                var controlItem = new Control
                {
                    ControlType = control.Type.AsControlType(),
                    Player = NumberHelper.ConvertToInt64(control.Player),
                    Buttons = NumberHelper.ConvertToInt64(control.Buttons),
                    RequiredButtons = NumberHelper.ConvertToInt64(control.ReqButtons),
                    Minimum = NumberHelper.ConvertToInt64(control.Minimum),
                    Maximum = NumberHelper.ConvertToInt64(control.Maximum),
                    Sensitivity = NumberHelper.ConvertToInt64(control.Sensitivity),
                    KeyDelta = NumberHelper.ConvertToInt64(control.KeyDelta),
                    Reverse = control.Reverse.AsYesNo(),
                    Ways = control.Ways,
                    Ways2 = control.Ways2,
                    Ways3 = control.Ways3,
                };
                controls.Add(controlItem);
            }

            if (controls.Any())
                item.Controls = controls;

            item.CopyMachineInformation(machine);
            ParseAddHelper(item, statsOnly);
        }

        /// <summary>
        /// Convert DipSwitch information
        /// </summary>
        /// <param name="dipswitches">Array of deserialized models to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertDipSwitches(Models.Listxml.DipSwitch[]? dipswitches, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the DipSwitch array is missing, we can't do anything
            if (dipswitches == null || !dipswitches.Any())
                return;

            containsItems = true;
            foreach (var dipswitch in dipswitches)
            {
                var item = new DipSwitch
                {
                    Name = dipswitch.Name,
                    Tag = dipswitch.Tag,
                    Mask = dipswitch.Mask,

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

                if (dipswitch.Condition != null)
                {
                    var condition = new Condition
                    {
                        Tag = dipswitch.Condition.Tag,
                        Mask = dipswitch.Condition.Mask,
                        Relation = dipswitch.Condition.Relation.AsRelation(),
                        Value = dipswitch.Condition.Value,
                    };
                    item.Conditions = new List<Condition> { condition };
                }

                var locations = new List<DipLocation>();
                foreach (var diplocation in dipswitch.DipLocation ?? Array.Empty<Models.Listxml.DipLocation>())
                {
                    var locationItem = new DipLocation
                    {
                        Name = diplocation.Name,
                        Number = NumberHelper.ConvertToInt64(diplocation.Number),
                        Inverted = diplocation.Inverted.AsYesNo(),
                    };
                    locations.Add(locationItem);
                }

                if (locations.Any())
                    item.Locations = locations;

                var settings = new List<DipValue>();
                foreach (var dipvalue in dipswitch.DipValue ?? Array.Empty<Models.Listxml.DipValue>())
                {
                    var dipValueItem = new DipValue
                    {
                        Name = dipvalue.Name,
                        Value = dipvalue.Value,
                        Default = dipvalue.Default.AsYesNo(),
                    };

                    if (dipvalue.Condition != null)
                    {
                        var condition = new Condition
                        {
                            Tag = dipvalue.Condition.Tag,
                            Mask = dipvalue.Condition.Mask,
                            Relation = dipvalue.Condition.Relation.AsRelation(),
                            Value = dipvalue.Condition.Value,
                        };
                        dipValueItem.Conditions = new List<Condition> { condition };
                    }

                    settings.Add(dipValueItem);
                }

                if (settings.Any())
                    item.Values = settings;

                item.CopyMachineInformation(machine);
                ParseAddHelper(item, statsOnly);
            }
        }

        /// <summary>
        /// Convert Configuration information
        /// </summary>
        /// <param name="configurations">Array of deserialized models to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertConfigurations(Models.Listxml.Configuration[]? configurations, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the Configuration array is missing, we can't do anything
            if (configurations == null || !configurations.Any())
                return;

            containsItems = true;
            foreach (var configuration in configurations)
            {
                var item = new Configuration
                {
                    Name = configuration.Name,
                    Tag = configuration.Tag,
                    Mask = configuration.Mask,

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

                if (configuration.Condition != null)
                {
                    var condition = new DatItems.Formats.Condition
                    {
                        Tag = configuration.Condition.Tag,
                        Mask = configuration.Condition.Mask,
                        Relation = configuration.Condition.Relation.AsRelation(),
                        Value = configuration.Condition.Value,
                    };
                    item.Conditions = new List<DatItems.Formats.Condition> { condition };
                }

                var locations = new List<ConfLocation>();
                foreach (var confLocation in configuration.ConfLocation ?? Array.Empty<Models.Listxml.ConfLocation>())
                {
                    var locationItem = new ConfLocation
                    {
                        Name = confLocation.Name,
                        Number = NumberHelper.ConvertToInt64(confLocation.Number),
                        Inverted = confLocation.Inverted.AsYesNo(),
                    };
                    locations.Add(locationItem);
                }

                if (locations.Any())
                    item.Locations = locations;

                var settings = new List<ConfSetting>();
                foreach (var dipvalue in configuration.ConfSetting ?? Array.Empty<Models.Listxml.ConfSetting>())
                {
                    var settingItem = new ConfSetting
                    {
                        Name = dipvalue.Name,
                        Value = dipvalue.Value,
                        Default = dipvalue.Default.AsYesNo(),
                    };

                    if (dipvalue.Condition != null)
                    {
                        var condition = new Condition
                        {
                            Tag = dipvalue.Condition.Tag,
                            Mask = dipvalue.Condition.Mask,
                            Relation = dipvalue.Condition.Relation.AsRelation(),
                            Value = dipvalue.Condition.Value,
                        };
                        settingItem.Conditions = new List<Condition> { condition };
                    }

                    settings.Add(settingItem);
                }

                if (settings.Any())
                    item.Settings = settings;

                item.CopyMachineInformation(machine);
                ParseAddHelper(item, statsOnly);
            }
        }

        /// <summary>
        /// Convert Port information
        /// </summary>
        /// <param name="ports">Array of deserialized models to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertPorts(Models.Listxml.Port[]? ports, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the Port array is missing, we can't do anything
            if (ports == null || !ports.Any())
                return;

            containsItems = true;
            foreach (var port in ports)
            {
                var item = new Port
                {
                    Tag = port.Tag,

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

                var analogs = new List<Analog>();
                foreach (var analog in port.Analog ?? Array.Empty<Models.Listxml.Analog>())
                {
                    var analogItem = new Analog
                    {
                        Mask = analog.Mask,
                    };
                    analogs.Add(analogItem);
                }

                if (analogs.Any())
                    item.Analogs = analogs;

                item.CopyMachineInformation(machine);
                ParseAddHelper(item, statsOnly);
            }
        }

        /// <summary>
        /// Convert Adjuster information
        /// </summary>
        /// <param name="adjusters">Array of deserialized models to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertAdjusters(Models.Listxml.Adjuster[]? adjusters, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the Adjuster array is missing, we can't do anything
            if (adjusters == null || !adjusters.Any())
                return;

            containsItems = true;
            foreach (var adjuster in adjusters)
            {
                var item = new Adjuster
                {
                    Name = adjuster.Name,
                    Default = adjuster.Default.AsYesNo(),

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

                if (adjuster.Condition != null)
                {
                    var condition = new Condition
                    {
                        Tag = adjuster.Condition.Tag,
                        Mask = adjuster.Condition.Mask,
                        Relation = adjuster.Condition.Relation.AsRelation(),
                        Value = adjuster.Condition.Value,
                    };
                    item.Conditions = new List<Condition> { condition };
                }

                item.CopyMachineInformation(machine);
                ParseAddHelper(item, statsOnly);
            }
        }

        /// <summary>
        /// Convert Driver information
        /// </summary>
        /// <param name="driver">Deserialized model to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertDriver(Models.Listxml.Driver? driver, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the Driver is missing, we can't do anything
            if (driver == null)
                return;

            containsItems = true;
            var item = new Driver
            {
                Status = driver.Status.AsSupportStatus(),
                //Color = driver.Color.AsSupportStatus(), // TODO: Add to internal model
                //Sound = driver.Sound.AsSupportStatus(), // TODO: Add to internal model
                //PaletteSize = NumberHelper.ConvertToInt64(driver.PaletteSize), // TODO: Add to internal model
                Emulation = driver.Emulation.AsSupportStatus(),
                Cocktail = driver.Cocktail.AsSupportStatus(),
                SaveState = driver.SaveState.AsSupported(),
                RequiresArtwork = driver.RequiresArtwork.AsYesNo(),
                Unofficial = driver.Unofficial.AsYesNo(),
                NoSoundHardware = driver.NoSoundHardware.AsYesNo(),
                Incomplete = driver.Incomplete.AsYesNo(),

                Source = new Source
                {
                    Index = indexId,
                    Name = filename,
                },
            };

            item.CopyMachineInformation(machine);
            ParseAddHelper(item, statsOnly);
        }

        /// <summary>
        /// Convert Feature information
        /// </summary>
        /// <param name="features">Array of deserialized models to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertFeatures(Models.Listxml.Feature[]? features, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the Feature array is missing, we can't do anything
            if (features == null || !features.Any())
                return;

            containsItems = true;
            foreach (var feature in features)
            {
                var item = new Feature
                {
                    Type = feature.Type.AsFeatureType(),
                    Status = feature.Status.AsFeatureStatus(),
                    Overall = feature.Overall.AsFeatureStatus(),

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
        /// Convert Device information
        /// </summary>
        /// <param name="devices">Array of deserialized models to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertDevices(Models.Listxml.Device[]? devices, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the Device array is missing, we can't do anything
            if (devices == null || !devices.Any())
                return;

            containsItems = true;
            foreach (var device in devices)
            {
                var item = new Device
                {
                    DeviceType = device.Type.AsDeviceType(),
                    Tag = device.Tag,
                    FixedImage = device.FixedImage,
                    Mandatory = NumberHelper.ConvertToInt64(device.Mandatory),
                    Interface = device.Interface,

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

                if (device.Instance != null)
                {
                    var instance = new Instance
                    {
                        Name = device.Instance.Name,
                        BriefName = device.Instance.BriefName,
                    };
                    item.Instances = new List<Instance> { instance };
                }

                var extensions = new List<Extension>();
                foreach (var extension in device.Extension ?? Array.Empty<Models.Listxml.Extension>())
                {
                    var extensionItem = new Extension
                    {
                        Name = extension.Name,
                    };
                    extensions.Add(extensionItem);
                }

                if (extensions.Any())
                    item.Extensions = extensions;

                item.CopyMachineInformation(machine);
                ParseAddHelper(item, statsOnly);
            }
        }

        /// <summary>
        /// Convert Slot information
        /// </summary>
        /// <param name="slots">Array of deserialized models to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertSlots(Models.Listxml.Slot[]? slots, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the Slot array is missing, we can't do anything
            if (slots == null || !slots.Any())
                return;

            containsItems = true;
            foreach (var slot in slots)
            {
                var item = new Slot
                {
                    Name = slot.Name,

                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

                var slotoptions = new List<SlotOption>();
                foreach (var slotoption in slot.SlotOption ?? Array.Empty<Models.Listxml.SlotOption>())
                {
                    var slotoptionItem = new SlotOption
                    {
                        Name = slotoption.Name,
                        DeviceName = slotoption.DevName,
                        Default = slotoption.Default.AsYesNo(),
                    };
                    slotoptions.Add(slotoptionItem);
                }

                if (slotoptions.Any())
                    item.SlotOptions = slotoptions;

                item.CopyMachineInformation(machine);
                ParseAddHelper(item, statsOnly);
            }
        }

        /// <summary>
        /// Convert SoftwareList information
        /// </summary>
        /// <param name="softwarelists">Array of deserialized models to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertSoftwareLists(Models.Listxml.SoftwareList[]? softwarelists, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the SoftwareList array is missing, we can't do anything
            if (softwarelists == null || !softwarelists.Any())
                return;

            containsItems = true;
            foreach (var softwarelist in softwarelists)
            {
                var item = new DatItems.Formats.SoftwareList
                {
                    Tag = softwarelist.Tag,
                    Name = softwarelist.Name,
                    Status = softwarelist.Status.AsSoftwareListStatus(),
                    Filter = softwarelist.Filter,

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
        /// Convert RamOption information
        /// </summary>
        /// <param name="ramoptions">Array of deserialized models to convert</param>
        /// <param name="machine">Prefilled machine to use</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="containsItems">True if there were any items in the array, false otherwise</param>
        private void ConvertRamOptions(Models.Listxml.RamOption[]? ramoptions, Machine machine, string filename, int indexId, bool statsOnly, ref bool containsItems)
        {
            // If the RamOption array is missing, we can't do anything
            if (ramoptions == null || !ramoptions.Any())
                return;

            containsItems = true;
            foreach (var ramoption in ramoptions)
            {
                var item = new RamOption
                {
                    Name = ramoption.Name,
                    Default = ramoption.Default.AsYesNo(),
                    Content = ramoption.Content,

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

        #endregion
    }
}
