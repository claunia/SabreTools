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

           if (Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.NameKey, mame.Build);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, mame.Build);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.BuildKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.BuildKey, mame.Build);
            if (Header.GetFieldValue<bool?>(Models.Metadata.Header.DebugKey) == null)
                Header.SetFieldValue<bool?> (Models.Metadata.Header.DebugKey, mame.Debug.AsYesNo());
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.MameConfigKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.MameConfigKey, mame.MameConfig);

            // Handle implied SuperDAT
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey)?.Contains(" - SuperDAT") == true && keep)
            {
                if (Header.GetFieldValue<string?>(Models.Metadata.Header.TypeKey) == null)
                    Header.SetFieldValue<string?>(Models.Metadata.Header.TypeKey, "SuperDAT");
            }
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
            var machine = new Machine();
            machine.SetFieldValue<string?>(Models.Metadata.Machine.CloneOfKey, game.CloneOf);
            machine.SetFieldValue<string?>(Models.Metadata.Machine.DescriptionKey, game.Description);
            machine.SetFieldValue<string?>(Models.Metadata.Machine.HistoryKey, game.History);
            machine.SetFieldValue<bool?>(Models.Metadata.Machine.IsBiosKey, game.IsBios.AsYesNo());
            machine.SetFieldValue<bool?>(Models.Metadata.Machine.IsDeviceKey, game.IsDevice.AsYesNo());
            machine.SetFieldValue<bool?>(Models.Metadata.Machine.IsMechanicalKey, game.IsMechanical.AsYesNo());
            machine.SetFieldValue<string?>(Models.Metadata.Machine.ManufacturerKey, game.Manufacturer);
            machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, game.Name);
            machine.SetFieldValue<Runnable>(Models.Metadata.Machine.RunnableKey, game.Runnable.AsEnumValue<Runnable>());
            machine.SetFieldValue<string?>(Models.Metadata.Machine.RomOfKey, game.RomOf);
            machine.SetFieldValue<string?>(Models.Metadata.Machine.SampleOfKey, game.SampleOf);
            machine.SetFieldValue<string?>(Models.Metadata.Machine.SourceFileKey, game.SourceFile);
            machine.SetFieldValue<string?>(Models.Metadata.Machine.YearKey, game.Year);

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
                    Source = new Source { Index = indexId, Name = filename },
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
                    Source = new Source { Index = indexId, Name = filename },
                };
                item.SetName(biosset.Name);
                item.SetFieldValue<bool?>(Models.Metadata.BiosSet.DefaultKey, biosset.Default?.AsYesNo());
                item.SetFieldValue<string?>(Models.Metadata.BiosSet.DescriptionKey, biosset.Description);

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
                    Source = new Source { Index = indexId, Name = filename },
                };
                item.SetName(rom.Name);
                item.SetFieldValue<long?>(Models.Metadata.Rom.SizeKey, NumberHelper.ConvertToInt64(rom.Size));
                item.SetFieldValue<string?>(Models.Metadata.Rom.BiosKey, rom.Bios);
                item.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, rom.CRC);
                item.SetFieldValue<bool?>(Models.Metadata.Rom.DisposeKey, rom.Dispose.AsYesNo());
                item.SetFieldValue<string?>(Models.Metadata.Rom.MergeKey, rom.Merge);
                item.SetFieldValue<string?>(Models.Metadata.Rom.OffsetKey, rom.Offset);
                item.SetFieldValue<bool?>(Models.Metadata.Rom.OptionalKey, rom.Optional.AsYesNo());
                item.SetFieldValue<string?>(Models.Metadata.Rom.RegionKey, rom.Region);
                item.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, rom.SHA1);
                item.SetFieldValue<bool?>(Models.Metadata.Rom.SoundOnlyKey, rom.SoundOnly.AsYesNo());
                item.SetFieldValue<ItemStatus>(Models.Metadata.Rom.StatusKey, rom.Status.AsEnumValue<ItemStatus>());

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
                    Source = new Source { Index = indexId, Name = filename },
                };
                item.SetName(disk.Name);
                item.SetFieldValue<string?>(Models.Metadata.Disk.IndexKey, disk.Index);
                item.SetFieldValue<ItemStatus>(Models.Metadata.Disk.StatusKey, disk.Status?.AsEnumValue<ItemStatus>() ?? ItemStatus.NULL);
                item.SetFieldValue<string?>(Models.Metadata.Disk.MD5Key, disk.MD5);
                item.SetFieldValue<string?>(Models.Metadata.Disk.MergeKey, disk.Merge);
                item.SetFieldValue<bool?>(Models.Metadata.Disk.OptionalKey, disk.Optional.AsYesNo());
                item.SetFieldValue<string?>(Models.Metadata.Disk.RegionKey, disk.Region);
                item.SetFieldValue<string?>(Models.Metadata.Disk.SHA1Key, disk.SHA1);
                item.SetFieldValue<bool?>(Models.Metadata.Disk.WritableKey, disk.Writable.AsYesNo());

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
                    Source = new Source { Index = indexId, Name = filename },
                };
                item.SetName(deviceref.Name);

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
                    Source = new Source { Index = indexId, Name = filename },
                };
                item.SetName(sample.Name);

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
                    Source = new Source { Index = indexId, Name = filename },
                };
                item.SetName(chip.Name);
                item.SetFieldValue<ChipType?>(Models.Metadata.Chip.ChipTypeKey, chip.Type.AsEnumValue<ChipType>());
                item.SetFieldValue<long?>(Models.Metadata.Chip.ClockKey, NumberHelper.ConvertToInt64(chip.Clock));
                item.SetFieldValue<bool?>(Models.Metadata.Chip.SoundOnlyKey, chip.Type.AsYesNo());
                item.SetFieldValue<string?>(Models.Metadata.Chip.TagKey, chip.Tag);

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
                    Source = new Source { Index = indexId, Name = filename },
                };
                item.SetFieldValue<DisplayType>(Models.Metadata.Display.DisplayTypeKey, display.Type.AsEnumValue<DisplayType>());
                item.SetFieldValue<bool?>(Models.Metadata.Display.FlipXKey, display.FlipX.AsYesNo());
                item.SetFieldValue<long?>(Models.Metadata.Display.HBEndKey, NumberHelper.ConvertToInt64(display.HBEnd));
                item.SetFieldValue<long?>(Models.Metadata.Display.HBStartKey, NumberHelper.ConvertToInt64(display.HBStart));
                item.SetFieldValue<long?>(Models.Metadata.Display.HeightKey, NumberHelper.ConvertToInt64(display.Height));
                item.SetFieldValue<long?>(Models.Metadata.Display.HTotalKey, NumberHelper.ConvertToInt64(display.HTotal));
                item.SetFieldValue<long?>(Models.Metadata.Display.PixClockKey, NumberHelper.ConvertToInt64(display.PixClock));
                item.SetFieldValue<double?>(Models.Metadata.Display.RefreshKey, NumberHelper.ConvertToDouble(display.Refresh));
                item.SetFieldValue<long?>(Models.Metadata.Display.RotateKey, NumberHelper.ConvertToInt64(display.Rotate));
                item.SetFieldValue<string?>(Models.Metadata.Display.TagKey, display.Tag);
                item.SetFieldValue<long?>(Models.Metadata.Display.VBEndKey, NumberHelper.ConvertToInt64(display.VBEnd));
                item.SetFieldValue<long?>(Models.Metadata.Display.VBStartKey, NumberHelper.ConvertToInt64(display.VBStart));
                item.SetFieldValue<long?>(Models.Metadata.Display.VTotalKey, NumberHelper.ConvertToInt64(display.VTotal));
                item.SetFieldValue<long?>(Models.Metadata.Display.WidthKey, NumberHelper.ConvertToInt64(display.Width));

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
                    Source = new Source { Index = indexId, Name = filename },
                };
                item.SetFieldValue<long?>("ASPECTX", NumberHelper.ConvertToInt64(video.AspectX));
                item.SetFieldValue<long?>("ASPECTY", NumberHelper.ConvertToInt64(video.AspectY));
                item.SetFieldValue<DisplayType>(Models.Metadata.Display.DisplayTypeKey, video.Screen?.AsEnumValue<DisplayType>() ?? DisplayType.NULL);
                item.SetFieldValue<long?>(Models.Metadata.Display.HeightKey, NumberHelper.ConvertToInt64(video.Height));
                item.SetFieldValue<double?>(Models.Metadata.Display.RefreshKey, NumberHelper.ConvertToDouble(video.Refresh));
                item.SetFieldValue<long?>(Models.Metadata.Display.WidthKey, NumberHelper.ConvertToInt64(video.Width));

                switch (video.Orientation)
                {
                    case "horizontal":
                        item.SetFieldValue<long?>(Models.Metadata.Display.RotateKey, 0);
                        break;
                    case "vertical":
                        item.SetFieldValue<long?>(Models.Metadata.Display.RotateKey, 90);
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
                Source = new Source { Index = indexId, Name = filename },
            };
            item.SetFieldValue<long?>(Models.Metadata.Sound.ChannelsKey, NumberHelper.ConvertToInt64(sound.Channels));

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
                Source = new Source { Index = indexId, Name = filename },
            };
            item.SetFieldValue<long?>(Models.Metadata.Input.ButtonsKey, NumberHelper.ConvertToInt64(input.Buttons));
            item.SetFieldValue<long?>(Models.Metadata.Input.CoinsKey, NumberHelper.ConvertToInt64(input.Coins));
            //item.SetFieldValue<string?>(Models.Metadata.Input.ControlKey, input.ControlAttr);
            item.SetFieldValue<long?>(Models.Metadata.Input.PlayersKey, NumberHelper.ConvertToInt64(input.Players));
            item.SetFieldValue<bool?>(Models.Metadata.Input.ServiceKey, input.Service?.AsYesNo());
            item.SetFieldValue<bool?>(Models.Metadata.Input.TiltKey, input.Tilt?.AsYesNo());

            var controls = new List<Control>();
            foreach (var control in input.Control ?? [])
            {
                var controlItem = new Control();
                controlItem.SetFieldValue<long?>(Models.Metadata.Control.ButtonsKey, NumberHelper.ConvertToInt64(control.Buttons));
                controlItem.SetFieldValue<ControlType?>(Models.Metadata.Control.ControlTypeKey, control.Type.AsEnumValue<ControlType>());
                controlItem.SetFieldValue<long?>(Models.Metadata.Control.KeyDeltaKey, NumberHelper.ConvertToInt64(control.KeyDelta));
                controlItem.SetFieldValue<long?>(Models.Metadata.Control.MaximumKey, NumberHelper.ConvertToInt64(control.Maximum));
                controlItem.SetFieldValue<long?>(Models.Metadata.Control.MinimumKey, NumberHelper.ConvertToInt64(control.Minimum));
                controlItem.SetFieldValue<long?>(Models.Metadata.Control.PlayerKey, NumberHelper.ConvertToInt64(control.Player));
                controlItem.SetFieldValue<long?>(Models.Metadata.Control.ReqButtonsKey, NumberHelper.ConvertToInt64(control.ReqButtons));
                controlItem.SetFieldValue<bool?>(Models.Metadata.Control.ReverseKey, control.Reverse.AsYesNo());
                controlItem.SetFieldValue<long?>(Models.Metadata.Control.SensitivityKey, NumberHelper.ConvertToInt64(control.Sensitivity));
                controlItem.SetFieldValue<string?>(Models.Metadata.Control.WaysKey, control.Ways);
                controlItem.SetFieldValue<string?>(Models.Metadata.Control.Ways2Key, control.Ways2);
                controlItem.SetFieldValue<string?>(Models.Metadata.Control.Ways3Key, control.Ways3);

                controls.Add(controlItem);
            }

            if (controls.Any())
                item.SetFieldValue<Control[]?>(Models.Metadata.Input.ControlKey, [.. controls]);

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
                    Source = new Source { Index = indexId, Name = filename },
                };
                item.SetName(dipswitch.Name);
                item.SetFieldValue<string?>(Models.Metadata.DipSwitch.MaskKey, dipswitch.Mask);
                item.SetFieldValue<string?>(Models.Metadata.DipSwitch.TagKey, dipswitch.Tag);

                if (dipswitch.Condition != null)
                {
                    var condition = new Condition();
                    condition.SetFieldValue<string?>(Models.Metadata.Condition.MaskKey, dipswitch.Condition.Mask);
                    condition.SetFieldValue<Relation?>(Models.Metadata.Condition.RelationKey, dipswitch.Condition.Relation.AsEnumValue<Relation>());
                    condition.SetFieldValue<string?>(Models.Metadata.Condition.TagKey, dipswitch.Condition.Tag);
                    condition.SetFieldValue<string?>(Models.Metadata.Condition.ValueKey, dipswitch.Condition.Value);

                    item.SetFieldValue<Condition[]?>(Models.Metadata.DipSwitch.ConditionKey, [condition]);
                }

                var locations = new List<DipLocation>();
                foreach (var diplocation in dipswitch.DipLocation ?? [])
                {
                    var locationItem = new DipLocation();
                    locationItem.SetName(diplocation.Name);
                    locationItem.SetFieldValue<bool?>(Models.Metadata.DipLocation.InvertedKey, diplocation.Inverted.AsYesNo());
                    locationItem.SetFieldValue<long?>(Models.Metadata.DipLocation.NumberKey, NumberHelper.ConvertToInt64(diplocation.Number));

                    locations.Add(locationItem);
                }

                if (locations.Any())
                    item.SetFieldValue<DipLocation[]?>(Models.Metadata.DipSwitch.DipLocationKey, [.. locations]);

                var settings = new List<DipValue>();
                foreach (var dipvalue in dipswitch.DipValue ?? [])
                {
                    var dipValueItem = new DipValue();
                    dipValueItem.SetName(dipvalue.Name);
                    dipValueItem.SetFieldValue<bool?>(Models.Metadata.DipValue.DefaultKey, dipvalue.Default.AsYesNo());
                    dipValueItem.SetFieldValue<string?>(Models.Metadata.DipValue.ValueKey, dipvalue.Value);

                    if (dipvalue.Condition != null)
                    {
                        var condition = new Condition();
                        condition.SetFieldValue<string?>(Models.Metadata.Condition.MaskKey, dipvalue.Condition.Mask);
                        condition.SetFieldValue<Relation?>(Models.Metadata.Condition.RelationKey, dipvalue.Condition.Relation.AsEnumValue<Relation>());
                        condition.SetFieldValue<string?>(Models.Metadata.Condition.TagKey, dipvalue.Condition.Tag);
                        condition.SetFieldValue<string?>(Models.Metadata.Condition.ValueKey, dipvalue.Condition.Value);

                        dipValueItem.SetFieldValue<Condition[]?>(Models.Metadata.DipValue.ConditionKey, [condition]);
                    }

                    settings.Add(dipValueItem);
                }

                if (settings.Any())
                    item.SetFieldValue<DipValue[]?>(Models.Metadata.DipSwitch.DipValueKey, [.. settings]);

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
                    Source = new Source { Index = indexId, Name = filename },
                };
                item.SetName(configuration.Name);
                item.SetFieldValue<string?>(Models.Metadata.Configuration.MaskKey, configuration.Mask);
                item.SetFieldValue<string?>(Models.Metadata.Configuration.TagKey, configuration.Tag);

                if (configuration.Condition != null)
                {
                    var condition = new DatItems.Formats.Condition();
                    condition.SetFieldValue<string?>(Models.Metadata.Condition.MaskKey, configuration.Condition.Mask);
                    condition.SetFieldValue<Relation?>(Models.Metadata.Condition.RelationKey, configuration.Condition.Relation.AsEnumValue<Relation>());
                    condition.SetFieldValue<string?>(Models.Metadata.Condition.TagKey, configuration.Condition.Tag);
                    condition.SetFieldValue<string?>(Models.Metadata.Condition.ValueKey, configuration.Condition.Value);

                    item.SetFieldValue<Condition[]?>(Models.Metadata.Configuration.ConditionKey, [condition]);
                }

                var locations = new List<ConfLocation>();
                foreach (var confLocation in configuration.ConfLocation ?? [])
                {
                    var locationItem = new ConfLocation();
                    locationItem.SetName(confLocation.Name);
                    locationItem.SetFieldValue<bool?>(Models.Metadata.ConfLocation.InvertedKey, confLocation.Inverted.AsYesNo());
                    locationItem.SetFieldValue<long?>(Models.Metadata.ConfLocation.NumberKey, NumberHelper.ConvertToInt64(confLocation.Number));

                    locations.Add(locationItem);
                }

                if (locations.Any())
                    item.SetFieldValue<ConfLocation[]?>(Models.Metadata.Configuration.ConfLocationKey, [.. locations]);

                var settings = new List<ConfSetting>();
                foreach (var dipvalue in configuration.ConfSetting ?? [])
                {
                    var settingItem = new ConfSetting();
                    settingItem.SetName(dipvalue.Name);
                    settingItem.SetFieldValue<bool?>(Models.Metadata.ConfSetting.DefaultKey, dipvalue.Default.AsYesNo());
                    settingItem.SetFieldValue<string?>(Models.Metadata.ConfSetting.ValueKey, dipvalue.Value);

                    if (dipvalue.Condition != null)
                    {
                        var condition = new Condition();
                        condition.SetFieldValue<string?>(Models.Metadata.Condition.MaskKey, dipvalue.Condition.Mask);
                        condition.SetFieldValue<Relation?>(Models.Metadata.Condition.RelationKey, dipvalue.Condition.Relation.AsEnumValue<Relation>());
                        condition.SetFieldValue<string?>(Models.Metadata.Condition.TagKey, dipvalue.Condition.Tag);
                        condition.SetFieldValue<string?>(Models.Metadata.Condition.ValueKey, dipvalue.Condition.Value);

                        settingItem.SetFieldValue<Condition[]?>(Models.Metadata.ConfSetting.ConditionKey, [condition]);
                    }

                    settings.Add(settingItem);
                }

                if (settings.Any())
                    item.SetFieldValue<ConfSetting[]?>(Models.Metadata.Configuration.ConfSettingKey, [.. settings]);

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
                    Source = new Source { Index = indexId, Name = filename },
                };
                item.SetFieldValue<string>(Models.Metadata.Port.TagKey, port.Tag);

                var analogs = new List<Analog>();
                foreach (var analog in port.Analog ?? [])
                {
                    var analogItem = new Analog();
                    analogItem.SetFieldValue<string?>(Models.Metadata.Analog.MaskKey, analog.Mask);

                    analogs.Add(analogItem);
                }

                if (analogs.Any())
                    item.SetFieldValue<Analog[]?>(Models.Metadata.Port.AnalogKey, [.. analogs]);

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
                    Source = new Source { Index = indexId, Name = filename }
                };
                item.SetName(adjuster.Name);
                item.SetFieldValue<bool?>(Models.Metadata.Adjuster.DefaultKey, adjuster.Default.AsYesNo());

                if (adjuster.Condition != null)
                {
                    var condition = new Condition();
                    condition.SetFieldValue<string?>(Models.Metadata.Condition.MaskKey, adjuster.Condition.Mask);
                    condition.SetFieldValue<Relation?>(Models.Metadata.Condition.RelationKey, adjuster.Condition.Relation.AsEnumValue<Relation>());
                    condition.SetFieldValue<string?>(Models.Metadata.Condition.TagKey, adjuster.Condition.Tag);
                    condition.SetFieldValue<string?>(Models.Metadata.Condition.ValueKey, adjuster.Condition.Value);

                    item.SetFieldValue<Condition[]?>(Models.Metadata.Adjuster.ConditionKey, [condition]);
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
                Source = new Source { Index = indexId, Name = filename },
            };
            item.SetFieldValue<SupportStatus>(Models.Metadata.Driver.CocktailKey, driver.Cocktail?.AsEnumValue<SupportStatus>() ?? SupportStatus.NULL);
            item.SetFieldValue<SupportStatus>(Models.Metadata.Driver.ColorKey, driver.Color?.AsEnumValue<SupportStatus>() ?? SupportStatus.NULL);
            item.SetFieldValue<SupportStatus>(Models.Metadata.Driver.EmulationKey, driver.Emulation?.AsEnumValue<SupportStatus>() ?? SupportStatus.NULL);
            item.SetFieldValue<bool?>(Models.Metadata.Driver.IncompleteKey, driver.Incomplete.AsYesNo());
            item.SetFieldValue<bool?>(Models.Metadata.Driver.NoSoundHardwareKey, driver.NoSoundHardware.AsYesNo());
            item.SetFieldValue<long?>(Models.Metadata.Driver.PaletteSizeKey, NumberHelper.ConvertToInt64(driver.PaletteSize));
            item.SetFieldValue<bool?>(Models.Metadata.Driver.RequiresArtworkKey, driver.RequiresArtwork.AsYesNo());
            item.SetFieldValue<Supported>(Models.Metadata.Driver.SaveStateKey, driver.SaveState?.AsEnumValue<Supported>() ?? Supported.NULL);
            item.SetFieldValue<SupportStatus>(Models.Metadata.Driver.SoundKey, driver.Sound?.AsEnumValue<SupportStatus>() ?? SupportStatus.NULL);
            item.SetFieldValue<SupportStatus>(Models.Metadata.Driver.StatusKey, driver.Status?.AsEnumValue<SupportStatus>() ?? SupportStatus.NULL);
            item.SetFieldValue<bool?>(Models.Metadata.Driver.UnofficialKey, driver.Unofficial.AsYesNo());

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
                    Source = new Source { Index = indexId, Name = filename },
                };
                item.SetFieldValue<FeatureType>(Models.Metadata.Feature.FeatureTypeKey, feature.Type.AsEnumValue<FeatureType>());
                item.SetFieldValue<FeatureStatus>(Models.Metadata.Feature.OverallKey, feature.Overall.AsEnumValue<FeatureStatus>());
                item.SetFieldValue<FeatureStatus>(Models.Metadata.Feature.StatusKey, feature.Status.AsEnumValue<FeatureStatus>());

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
                    Source = new Source { Index = indexId, Name = filename },
                };
                item.SetFieldValue<DeviceType>(Models.Metadata.Device.DeviceTypeKey, device.Type.AsEnumValue<DeviceType>());
                item.SetFieldValue<string?>(Models.Metadata.Device.FixedImageKey, device.FixedImage);
                item.SetFieldValue<string?>(Models.Metadata.Device.InterfaceKey, device.Interface);
                item.SetFieldValue<long?>(Models.Metadata.Device.MandatoryKey, NumberHelper.ConvertToInt64(device.Mandatory));
                item.SetFieldValue<string?>(Models.Metadata.Device.TagKey, device.Tag);

                if (device.Instance != null)
                {
                    var instance = new Instance();
                    instance.SetName(device.Instance.Name);
                    instance.SetFieldValue<string?>(Models.Metadata.Instance.BriefNameKey, device.Instance.BriefName);

                    item.SetFieldValue<Instance[]?>(Models.Metadata.Device.InstanceKey, [instance]);
                }

                var extensions = new List<Extension>();
                foreach (var extension in device.Extension ?? [])
                {
                    var extensionItem = new Extension();
                    extensionItem.SetName(extension.Name);

                    extensions.Add(extensionItem);
                }

                if (extensions.Any())
                    item.SetFieldValue<Extension[]?>(Models.Metadata.Device.ExtensionKey, extensions.ToArray());

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
                    Source = new Source { Index = indexId, Name = filename },
                };
                item.SetName(slot.Name);

                var slotoptions = new List<SlotOption>();
                foreach (var slotoption in slot.SlotOption ?? [])
                {
                    var slotoptionItem = new SlotOption();
                    slotoptionItem.SetName(slotoption.Name);
                    slotoptionItem.SetFieldValue<bool?>(Models.Metadata.SlotOption.DefaultKey, slotoption.Default.AsYesNo());
                    slotoptionItem.SetFieldValue<string?>(Models.Metadata.SlotOption.DevNameKey, slotoption.DevName);

                    slotoptions.Add(slotoptionItem);
                }

                if (slotoptions.Any())
                    item.SetFieldValue<SlotOption[]?>(Models.Metadata.Slot.SlotOptionKey, [.. slotoptions]);

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
                    Source = new Source { Index = indexId, Name = filename },
                };
                item.SetName(softwarelist.Name);
                item.SetFieldValue<string?>(Models.Metadata.SoftwareList.FilterKey, softwarelist.Filter);
                item.SetFieldValue<SoftwareListStatus>(Models.Metadata.SoftwareList.StatusKey, softwarelist.Status.AsEnumValue<SoftwareListStatus>());
                item.SetFieldValue<string?>(Models.Metadata.SoftwareList.TagKey, softwarelist.Tag);

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
                    Source = new Source { Index = indexId, Name = filename },
                };
                item.SetName(ramoption.Name);
                item.SetFieldValue<string?>(Models.Metadata.RamOption.ContentKey, ramoption.Content);
                item.SetFieldValue<bool?>(Models.Metadata.RamOption.DefaultKey, ramoption.Default.AsYesNo());

                item.CopyMachineInformation(machine);
                ParseAddHelper(item, statsOnly);
            }
        }

        #endregion
    }
}
