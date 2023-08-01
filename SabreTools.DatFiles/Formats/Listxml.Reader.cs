using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
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
            // Prepare all internal variables
            XmlReader xtr = XmlReader.Create(filename, new XmlReaderSettings
            {
                CheckCharacters = false,
                DtdProcessing = DtdProcessing.Ignore,
                IgnoreComments = true,
                IgnoreWhitespace = true,
                ValidationFlags = XmlSchemaValidationFlags.None,
                ValidationType = ValidationType.None,
            });

            // If we got a null reader, just return
            if (xtr == null)
                return;

            // Otherwise, read the file to the end
            try
            {
                xtr.MoveToContent();
                while (!xtr.EOF)
                {
                    // We only want elements
                    if (xtr.NodeType != XmlNodeType.Element)
                    {
                        xtr.Read();
                        continue;
                    }

                    switch (xtr.Name)
                    {
                        case "mame":
                            Header.Name ??= xtr.GetAttribute("build");
                            Header.Description ??= Header.Name;
                            Header.Debug ??= xtr.GetAttribute("debug").AsYesNo();
                            Header.MameConfig ??= xtr.GetAttribute("mameconfig");
                            xtr.Read();
                            break;

                        // Handle M1 DATs since they're 99% the same as a SL DAT
                        case "m1":
                            Header.Name ??= "M1";
                            Header.Description ??= "M1";
                            Header.Version ??= xtr.GetAttribute("version") ?? string.Empty;
                            xtr.Read();
                            break;

                        // We want to process the entire subtree of the machine
                        case "game": // Some older DATs still use "game"
                        case "machine":
                            ReadMachine(xtr.ReadSubtree(), statsOnly, filename, indexId);

                            // Skip the machine now that we've processed it
                            xtr.Skip();
                            break;

                        default:
                            xtr.Read();
                            break;
                    }
                }
            }
            catch (Exception ex) when (!throwOnError)
            {
                logger.Warning(ex, $"Exception found while parsing '{filename}'");

                // For XML errors, just skip the affected node
                xtr?.Read();
            }

            xtr.Dispose();
        }

        /// <summary>
        /// Read machine information
        /// </summary>
        /// <param name="reader">XmlReader representing a machine block</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        private void ReadMachine(XmlReader reader, bool statsOnly, string filename, int indexId)
        {
            // If we have an empty machine, skip it
            if (reader == null)
                return;

            // Otherwise, add what is possible
            reader.MoveToContent();

            // Create a new machine
            MachineType machineType = 0x0;
            if (reader.GetAttribute("isbios").AsYesNo() == true)
                machineType |= MachineType.Bios;

            if (reader.GetAttribute("isdevice").AsYesNo() == true)
                machineType |= MachineType.Device;

            if (reader.GetAttribute("ismechanical").AsYesNo() == true)
                machineType |= MachineType.Mechanical;

            Machine machine = new()
            {
                Name = reader.GetAttribute("name"),
                Description = reader.GetAttribute("name"),
                CloneOf = reader.GetAttribute("cloneof"),
                RomOf = reader.GetAttribute("romof"),
                SampleOf = reader.GetAttribute("sampleof"),
                MachineType = (machineType == 0x0 ? MachineType.None : machineType),

                SourceFile = reader.GetAttribute("sourcefile"),
                Runnable = reader.GetAttribute("runnable").AsRunnable(),
            };

            // Get list for new DatItems
            List<DatItem> datItems = new();

            while (!reader.EOF)
            {
                // We only want elements
                if (reader.NodeType != XmlNodeType.Element)
                {
                    reader.Read();
                    continue;
                }

                // Get the roms from the machine
                switch (reader.Name)
                {
                    case "description":
                        machine.Description = reader.ReadElementContentAsString();
                        break;

                    case "year":
                        machine.Year = reader.ReadElementContentAsString();
                        break;

                    case "manufacturer":
                        machine.Manufacturer = reader.ReadElementContentAsString();
                        break;

                    case "history":
                        machine.History = reader.ReadElementContentAsString();
                        break;

                    case "adjuster":
                        var adjuster = new Adjuster
                        {
                            Name = reader.GetAttribute("name"),
                            Default = reader.GetAttribute("default").AsYesNo(),
                            Conditions = new List<Condition>(),

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        };

                        // Now read the internal tags
                        ReadAdjuster(reader.ReadSubtree(), adjuster);

                        datItems.Add(adjuster);

                        // Skip the adjuster now that we've processed it
                        reader.Skip();
                        break;

                    case "biosset":
                        datItems.Add(new BiosSet
                        {
                            Name = reader.GetAttribute("name"),
                            Description = reader.GetAttribute("description"),
                            Default = reader.GetAttribute("default").AsYesNo(),

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        });

                        reader.Read();
                        break;

                    case "chip":
                        var chip = new Chip
                        {
                            Name = reader.GetAttribute("name"),
                            Tag = reader.GetAttribute("tag"),
                            ChipType = reader.GetAttribute("type").AsChipType(),
                            Clock = Utilities.CleanLong(reader.GetAttribute("clock")),

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        };

                        datItems.Add(chip);

                        reader.Read();
                        break;

                    case "condition":
                        datItems.Add(new Condition
                        {
                            Tag = reader.GetAttribute("tag"),
                            Mask = reader.GetAttribute("mask"),
                            Relation = reader.GetAttribute("relation").AsRelation(),
                            Value = reader.GetAttribute("value"),

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        });

                        reader.Read();
                        break;

                    case "configuration":
                        var configuration = new Configuration
                        {
                            Name = reader.GetAttribute("name"),
                            Tag = reader.GetAttribute("tag"),
                            Mask = reader.GetAttribute("mask"),
                            Conditions = new List<Condition>(),
                            Locations = new List<Location>(),
                            Settings = new List<Setting>(),

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        };

                        // Now read the internal tags
                        ReadConfiguration(reader.ReadSubtree(), configuration);

                        datItems.Add(configuration);

                        // Skip the configuration now that we've processed it
                        reader.Skip();
                        break;

                    case "device":
                        var device = new Device
                        {
                            DeviceType = reader.GetAttribute("type").AsDeviceType(),
                            Tag = reader.GetAttribute("tag"),
                            FixedImage = reader.GetAttribute("fixed_image"),
                            Mandatory = Utilities.CleanLong(reader.GetAttribute("mandatory")),
                            Interface = reader.GetAttribute("interface"),

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        };

                        // Now read the internal tags
                        ReadDevice(reader.ReadSubtree(), device);

                        datItems.Add(device);

                        // Skip the device now that we've processed it
                        reader.Skip();
                        break;

                    case "device_ref":
                        datItems.Add(new DeviceReference
                        {
                            Name = reader.GetAttribute("name"),

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        });

                        reader.Read();
                        break;

                    case "dipswitch":
                        var dipSwitch = new DipSwitch
                        {
                            Name = reader.GetAttribute("name"),
                            Tag = reader.GetAttribute("tag"),
                            Mask = reader.GetAttribute("mask"),
                            Conditions = new List<Condition>(),
                            Locations = new List<Location>(),
                            Values = new List<Setting>(),

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        };

                        // Now read the internal tags
                        ReadDipSwitch(reader.ReadSubtree(), dipSwitch);

                        datItems.Add(dipSwitch);

                        // Skip the dipswitch now that we've processed it
                        reader.Skip();
                        break;

                    case "disk":
                        datItems.Add(new Disk
                        {
                            Name = reader.GetAttribute("name"),
                            SHA1 = reader.GetAttribute("sha1"),
                            MergeTag = reader.GetAttribute("merge"),
                            Region = reader.GetAttribute("region"),
                            Index = reader.GetAttribute("index"),
                            Writable = reader.GetAttribute("writable").AsYesNo(),
                            ItemStatus = reader.GetAttribute("status").AsItemStatus(),
                            Optional = reader.GetAttribute("optional").AsYesNo(),

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        });

                        reader.Read();
                        break;

                    case "display":
                        var display = new Display
                        {
                            Tag = reader.GetAttribute("tag"),
                            DisplayType = reader.GetAttribute("type").AsDisplayType(),
                            Rotate = Utilities.CleanLong(reader.GetAttribute("rotate")),
                            FlipX = reader.GetAttribute("flipx").AsYesNo(),
                            Width = Utilities.CleanLong(reader.GetAttribute("width")),
                            Height = Utilities.CleanLong(reader.GetAttribute("height")),
                            Refresh = Utilities.CleanDouble(reader.GetAttribute("refresh")),
                            PixClock = Utilities.CleanLong(reader.GetAttribute("pixclock")),
                            HTotal = Utilities.CleanLong(reader.GetAttribute("htotal")),
                            HBEnd = Utilities.CleanLong(reader.GetAttribute("hbend")),
                            HBStart = Utilities.CleanLong(reader.GetAttribute("hbstart")),
                            VTotal = Utilities.CleanLong(reader.GetAttribute("vtotal")),
                            VBEnd = Utilities.CleanLong(reader.GetAttribute("vbend")),
                            VBStart = Utilities.CleanLong(reader.GetAttribute("vbstart")),

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        };

                        datItems.Add(display);

                        reader.Read();
                        break;

                    case "driver":
                        datItems.Add(new Driver
                        {
                            Status = reader.GetAttribute("status").AsSupportStatus(),
                            Emulation = reader.GetAttribute("emulation").AsSupportStatus(),
                            Cocktail = reader.GetAttribute("cocktail").AsSupportStatus(),
                            SaveState = reader.GetAttribute("savestate").AsSupported(),
                            RequiresArtwork = reader.GetAttribute("requiresartwork").AsYesNo(),
                            Unofficial = reader.GetAttribute("unofficial").AsYesNo(),
                            NoSoundHardware = reader.GetAttribute("nosoundhardware").AsYesNo(),
                            Incomplete = reader.GetAttribute("incomplete").AsYesNo(),

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        });

                        reader.Read();
                        break;

                    case "feature":
                        datItems.Add(new Feature
                        {
                            Type = reader.GetAttribute("type").AsFeatureType(),
                            Status = reader.GetAttribute("status").AsFeatureStatus(),
                            Overall = reader.GetAttribute("overall").AsFeatureStatus(),

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        });

                        reader.Read();
                        break;

                    case "input":
                        var input = new Input
                        {
                            Service = reader.GetAttribute("service").AsYesNo(),
                            Tilt = reader.GetAttribute("tilt").AsYesNo(),
                            Players = Utilities.CleanLong(reader.GetAttribute("players")),
                            Coins = Utilities.CleanLong(reader.GetAttribute("coins")),

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        };

                        // Now read the internal tags
                        ReadInput(reader.ReadSubtree(), input);

                        datItems.Add(input);

                        // Skip the input now that we've processed it
                        reader.Skip();
                        break;

                    case "port":
                        var port = new Port
                        {
                            Tag = reader.GetAttribute("tag"),

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        };

                        // Now read the internal tags
                        ReadPort(reader.ReadSubtree(), port);

                        datItems.Add(port);

                        // Skip the port now that we've processed it
                        reader.Skip();
                        break;

                    case "ramoption":
                        datItems.Add(new RamOption
                        {
                            Name = reader.GetAttribute("name"),
                            Default = reader.GetAttribute("default").AsYesNo(),
                            Content = reader.ReadElementContentAsString(),

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        });

                        break;

                    case "rom":
                        datItems.Add(new Rom
                        {
                            Name = reader.GetAttribute("name"),
                            Bios = reader.GetAttribute("bios"),
                            Size = Utilities.CleanLong(reader.GetAttribute("size")),
                            CRC = reader.GetAttribute("crc"),
                            SHA1 = reader.GetAttribute("sha1"),
                            MergeTag = reader.GetAttribute("merge"),
                            Region = reader.GetAttribute("region"),
                            Offset = reader.GetAttribute("offset"),
                            ItemStatus = reader.GetAttribute("status").AsItemStatus(),
                            Optional = reader.GetAttribute("optional").AsYesNo(),

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        });

                        reader.Read();
                        break;

                    case "sample":
                        datItems.Add(new Sample
                        {
                            Name = reader.GetAttribute("name"),

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        });

                        reader.Read();
                        break;

                    case "slot":
                        var slot = new Slot
                        {
                            Name = reader.GetAttribute("name"),
                            SlotOptions = new List<SlotOption>(),

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        };

                        // Now read the internal tags
                        ReadSlot(reader.ReadSubtree(), slot);

                        datItems.Add(slot);

                        // Skip the slot now that we've processed it
                        reader.Skip();
                        break;

                    case "softwarelist":
                        datItems.Add(new DatItems.Formats.SoftwareList
                        {
                            Tag = reader.GetAttribute("tag"),
                            Name = reader.GetAttribute("name"),
                            Status = reader.GetAttribute("status").AsSoftwareListStatus(),
                            Filter = reader.GetAttribute("filter"),

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        });

                        reader.Read();
                        break;

                    case "sound":
                        var sound = new Sound
                        {
                            Channels = Utilities.CleanLong(reader.GetAttribute("channels")),

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        };

                        datItems.Add(sound);

                        reader.Read();
                        break;

                    default:
                        reader.Read();
                        break;
                }
            }

            // If we found items, copy the machine info and add them
            if (datItems.Any())
            {
                foreach (DatItem datItem in datItems)
                {
                    datItem.CopyMachineInformation(machine);
                    ParseAddHelper(datItem, statsOnly);
                }
            }

            // If no items were found for this machine, add a Blank placeholder
            else
            {
                Blank blank = new()
                {
                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

                blank.CopyMachineInformation(machine);

                // Now process and add the rom
                ParseAddHelper(blank, statsOnly);
            }
        }

        /// <summary>
        /// Read slot information
        /// </summary>
        /// <param name="reader">XmlReader representing a machine block</param>
        /// <param name="slot">ListXmlSlot to populate</param>
        private void ReadSlot(XmlReader reader, Slot slot)
        {
            // If we have an empty machine, skip it
            if (reader == null)
                return;

            // Get list ready
            slot.SlotOptions = new List<SlotOption>();

            // Otherwise, add what is possible
            reader.MoveToContent();

            while (!reader.EOF)
            {
                // We only want elements
                if (reader.NodeType != XmlNodeType.Element)
                {
                    reader.Read();
                    continue;
                }

                // Get the roms from the machine
                switch (reader.Name)
                {
                    case "slotoption":
                        var slotOption = new SlotOption
                        {
                            Name = reader.GetAttribute("name"),
                            DeviceName = reader.GetAttribute("devname"),
                            Default = reader.GetAttribute("default").AsYesNo()
                        };

                        slot.SlotOptions.Add(slotOption);

                        reader.Read();
                        break;

                    default:
                        reader.Read();
                        break;
                }
            }
        }

        /// <summary>
        /// Read Input information
        /// </summary>
        /// <param name="reader">XmlReader representing a diskarea block</param>
        /// <param name="input">ListXmlInput to populate</param>
        private void ReadInput(XmlReader reader, Input input)
        {
            // If we have an empty input, skip it
            if (reader == null)
                return;

            // Get list ready
            input.Controls = new List<Control>();

            // Otherwise, add what is possible
            reader.MoveToContent();

            while (!reader.EOF)
            {
                // We only want elements
                if (reader.NodeType != XmlNodeType.Element)
                {
                    reader.Read();
                    continue;
                }

                // Get the information from the dipswitch
                switch (reader.Name)
                {
                    case "control":
                        var control = new Control
                        {
                            ControlType = reader.GetAttribute("type").AsControlType(),
                            Player = Utilities.CleanLong(reader.GetAttribute("player")),
                            Buttons = Utilities.CleanLong(reader.GetAttribute("buttons")),
                            RequiredButtons = Utilities.CleanLong(reader.GetAttribute("reqbuttons")),
                            Minimum = Utilities.CleanLong(reader.GetAttribute("minimum")),
                            Maximum = Utilities.CleanLong(reader.GetAttribute("maximum")),
                            Sensitivity = Utilities.CleanLong(reader.GetAttribute("sensitivity")),
                            KeyDelta = Utilities.CleanLong(reader.GetAttribute("keydelta")),
                            Reverse = reader.GetAttribute("reverse").AsYesNo(),
                            Ways = reader.GetAttribute("ways"),
                            Ways2 = reader.GetAttribute("ways2"),
                            Ways3 = reader.GetAttribute("ways3"),
                        };

                        input.Controls.Add(control);

                        reader.Read();
                        break;

                    default:
                        reader.Read();
                        break;
                }
            }
        }

        /// <summary>
        /// Read DipSwitch information
        /// </summary>
        /// <param name="reader">XmlReader representing a diskarea block</param>
        /// <param name="dipSwitch">DipSwitch to populate</param>
        private void ReadDipSwitch(XmlReader reader, DipSwitch dipSwitch)
        {
            // If we have an empty dipswitch, skip it
            if (reader == null)
                return;

            // Get lists ready
            dipSwitch.Conditions = new List<Condition>();
            dipSwitch.Locations = new List<Location>();
            dipSwitch.Values = new List<Setting>();

            // Otherwise, add what is possible
            reader.MoveToContent();

            while (!reader.EOF)
            {
                // We only want elements
                if (reader.NodeType != XmlNodeType.Element)
                {
                    reader.Read();
                    continue;
                }

                // Get the information from the dipswitch
                switch (reader.Name)
                {
                    case "condition":
                        var condition = new Condition
                        {
                            Tag = reader.GetAttribute("tag"),
                            Mask = reader.GetAttribute("mask"),
                            Relation = reader.GetAttribute("relation").AsRelation(),
                            Value = reader.GetAttribute("value")
                        };

                        dipSwitch.Conditions.Add(condition);

                        reader.Read();
                        break;

                    case "diplocation":
                        var dipLocation = new Location
                        {
                            Name = reader.GetAttribute("name"),
                            Number = Utilities.CleanLong(reader.GetAttribute("number")),
                            Inverted = reader.GetAttribute("inverted").AsYesNo()
                        };

                        dipSwitch.Locations.Add(dipLocation);

                        reader.Read();
                        break;

                    case "dipvalue":
                        var dipValue = new Setting
                        {
                            Name = reader.GetAttribute("name"),
                            Value = reader.GetAttribute("value"),
                            Default = reader.GetAttribute("default").AsYesNo()
                        };

                        // Now read the internal tags
                        ReadDipValue(reader, dipValue);

                        dipSwitch.Values.Add(dipValue);

                        // Skip the dipvalue now that we've processed it
                        reader.Read();
                        break;

                    default:
                        reader.Read();
                        break;
                }
            }
        }

        /// <summary>
        /// Read DipValue information
        /// </summary>
        /// <param name="reader">XmlReader representing a diskarea block</param>
        /// <param name="dipValue">Setting to populate</param>
        private void ReadDipValue(XmlReader reader, Setting dipValue)
        {
            // If we have an empty dipvalue, skip it
            if (reader == null)
                return;

            // Get list ready
            dipValue.Conditions = new List<Condition>();

            // Otherwise, add what is possible
            reader.MoveToContent();

            while (!reader.EOF)
            {
                // We only want elements
                if (reader.NodeType != XmlNodeType.Element)
                {
                    reader.Read();
                    continue;
                }

                // Get the information from the dipvalue
                switch (reader.Name)
                {
                    case "condition":
                        var condition = new Condition
                        {
                            Tag = reader.GetAttribute("tag"),
                            Mask = reader.GetAttribute("mask"),
                            Relation = reader.GetAttribute("relation").AsRelation(),
                            Value = reader.GetAttribute("value")
                        };

                        dipValue.Conditions.Add(condition);

                        reader.Read();
                        break;

                    default:
                        reader.Read();
                        break;
                }
            }
        }

        /// <summary>
        /// Read Configuration information
        /// </summary>
        /// <param name="reader">XmlReader representing a diskarea block</param>
        /// <param name="configuration">Configuration to populate</param>
        private void ReadConfiguration(XmlReader reader, Configuration configuration)
        {
            // If we have an empty configuration, skip it
            if (reader == null)
                return;

            // Get lists ready
            configuration.Conditions = new List<Condition>();
            configuration.Locations = new List<Location>();
            configuration.Settings = new List<Setting>();

            // Otherwise, add what is possible
            reader.MoveToContent();

            while (!reader.EOF)
            {
                // We only want elements
                if (reader.NodeType != XmlNodeType.Element)
                {
                    reader.Read();
                    continue;
                }

                // Get the information from the dipswitch
                switch (reader.Name)
                {
                    case "condition":
                        var condition = new Condition
                        {
                            Tag = reader.GetAttribute("tag"),
                            Mask = reader.GetAttribute("mask"),
                            Relation = reader.GetAttribute("relation").AsRelation(),
                            Value = reader.GetAttribute("value")
                        };

                        configuration.Conditions.Add(condition);

                        reader.Read();
                        break;

                    case "conflocation":
                        var confLocation = new Location
                        {
                            Name = reader.GetAttribute("name"),
                            Number = Utilities.CleanLong(reader.GetAttribute("number")),
                            Inverted = reader.GetAttribute("inverted").AsYesNo()
                        };

                        configuration.Locations.Add(confLocation);

                        reader.Read();
                        break;

                    case "confsetting":
                        var confSetting = new Setting
                        {
                            Name = reader.GetAttribute("name"),
                            Value = reader.GetAttribute("value"),
                            Default = reader.GetAttribute("default").AsYesNo()
                        };

                        // Now read the internal tags
                        ReadConfSetting(reader, confSetting);

                        configuration.Settings.Add(confSetting);

                        // Skip the dipvalue now that we've processed it
                        reader.Read();
                        break;

                    default:
                        reader.Read();
                        break;
                }
            }
        }

        /// <summary>
        /// Read ConfSetting information
        /// </summary>
        /// <param name="reader">XmlReader representing a diskarea block</param>
        /// <param name="confSetting">Setting to populate</param>
        private void ReadConfSetting(XmlReader reader, Setting confSetting)
        {
            // If we have an empty confsetting, skip it
            if (reader == null)
                return;

            // Get list ready
            confSetting.Conditions = new List<Condition>();

            // Otherwise, add what is possible
            reader.MoveToContent();

            while (!reader.EOF)
            {
                // We only want elements
                if (reader.NodeType != XmlNodeType.Element)
                {
                    reader.Read();
                    continue;
                }

                // Get the information from the confsetting
                switch (reader.Name)
                {
                    case "condition":
                        var condition = new Condition
                        {
                            Tag = reader.GetAttribute("tag"),
                            Mask = reader.GetAttribute("mask"),
                            Relation = reader.GetAttribute("relation").AsRelation(),
                            Value = reader.GetAttribute("value")
                        };

                        confSetting.Conditions.Add(condition);

                        reader.Read();
                        break;

                    default:
                        reader.Read();
                        break;
                }
            }
        }

        /// <summary>
        /// Read Port information
        /// </summary>
        /// <param name="reader">XmlReader representing a diskarea block</param>
        /// <param name="port">ListXmlPort to populate</param>
        private void ReadPort(XmlReader reader, Port port)
        {
            // If we have an empty port, skip it
            if (reader == null)
                return;

            // Get list ready
            port.Analogs = new List<Analog>();

            // Otherwise, add what is possible
            reader.MoveToContent();

            while (!reader.EOF)
            {
                // We only want elements
                if (reader.NodeType != XmlNodeType.Element)
                {
                    reader.Read();
                    continue;
                }

                // Get the information from the port
                switch (reader.Name)
                {
                    case "analog":
                        var analog = new Analog
                        {
                            Mask = reader.GetAttribute("mask")
                        };

                        port.Analogs.Add(analog);

                        reader.Read();
                        break;

                    default:
                        reader.Read();
                        break;
                }
            }
        }

        /// <summary>
        /// Read Adjuster information
        /// </summary>
        /// <param name="reader">XmlReader representing a diskarea block</param>
        /// <param name="adjuster">Adjuster to populate</param>
        private void ReadAdjuster(XmlReader reader, Adjuster adjuster)
        {
            // If we have an empty port, skip it
            if (reader == null)
                return;

            // Get list ready
            adjuster.Conditions = new List<Condition>();

            // Otherwise, add what is possible
            reader.MoveToContent();

            while (!reader.EOF)
            {
                // We only want elements
                if (reader.NodeType != XmlNodeType.Element)
                {
                    reader.Read();
                    continue;
                }

                // Get the information from the adjuster
                switch (reader.Name)
                {
                    case "condition":
                        var condition = new Condition
                        {
                            Tag = reader.GetAttribute("tag"),
                            Mask = reader.GetAttribute("mask"),
                            Relation = reader.GetAttribute("relation").AsRelation(),
                            Value = reader.GetAttribute("value")
                        };

                        adjuster.Conditions.Add(condition);

                        reader.Read();
                        break;

                    default:
                        reader.Read();
                        break;
                }
            }
        }

        /// <summary>
        /// Read Device information
        /// </summary>
        /// <param name="reader">XmlReader representing a diskarea block</param>
        /// <param name="device">ListXmlDevice to populate</param>
        private void ReadDevice(XmlReader reader, Device device)
        {
            // If we have an empty port, skip it
            if (reader == null)
                return;

            // Get lists ready
            device.Instances = new List<Instance>();
            device.Extensions = new List<Extension>();

            // Otherwise, add what is possible
            reader.MoveToContent();

            while (!reader.EOF)
            {
                // We only want elements
                if (reader.NodeType != XmlNodeType.Element)
                {
                    reader.Read();
                    continue;
                }

                // Get the information from the adjuster
                switch (reader.Name)
                {
                    case "instance":
                        var instance = new Instance
                        {
                            Name = reader.GetAttribute("name"),
                            BriefName = reader.GetAttribute("briefname"),
                        };

                        device.Instances.Add(instance);

                        reader.Read();
                        break;

                    case "extension":
                        var extension = new Extension
                        {
                            Name = reader.GetAttribute("name"),
                        };

                        device.Extensions.Add(extension);

                        reader.Read();
                        break;

                    default:
                        reader.Read();
                        break;
                }
            }
        }
    }
}
