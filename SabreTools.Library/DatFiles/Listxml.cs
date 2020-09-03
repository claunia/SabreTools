using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

using SabreTools.Library.Data;
using SabreTools.Library.DatItems;
using SabreTools.Library.IO;
using SabreTools.Library.Tools;

namespace SabreTools.Library.DatFiles
{
    /// <summary>
    /// Represents parsing and writing of a MAME XML DAT
    /// </summary>
    internal class Listxml : DatFile
    {
        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public Listxml(DatFile datFile)
            : base(datFile)
        {
        }

        /// <summary>
        /// Parse a MAME XML DAT and return all found games and roms within
        /// </summary>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
        /// <remarks>
        /// </remarks>
        protected override void ParseFile(string filename, int indexId, bool keep)
        {
            // Prepare all internal variables
            XmlReader xtr = filename.GetXmlTextReader();

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
                            Header.Name = (Header.Name == null ? xtr.GetAttribute("build") : Header.Name);
                            Header.Description = (Header.Description == null ? Header.Name : Header.Description);
                            Header.Debug = (Header.Debug == null ? xtr.GetAttribute("debug").AsYesNo() : Header.Debug);
                            Header.MameConfig = (Header.MameConfig == null ? xtr.GetAttribute("mameconfig") : Header.MameConfig);
                            xtr.Read();
                            break;

                        // Handle M1 DATs since they're 99% the same as a SL DAT
                        case "m1":
                            Header.Name = (Header.Name == null ? "M1" : Header.Name);
                            Header.Description = (Header.Description == null ? "M1" : Header.Description);
                            Header.Version = (Header.Version == null ? xtr.GetAttribute("version") ?? string.Empty : Header.Version);
                            xtr.Read();
                            break;

                        // We want to process the entire subtree of the machine
                        case "game": // Some older DATs still use "game"
                        case "machine":
                            ReadMachine(xtr.ReadSubtree(), filename, indexId);

                            // Skip the machine now that we've processed it
                            xtr.Skip();
                            break;

                        default:
                            xtr.Read();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Globals.Logger.Warning($"Exception found while parsing '{filename}': {ex}");

                // For XML errors, just skip the affected node
                xtr?.Read();
            }

            xtr.Dispose();
        }

        /// <summary>
        /// Read machine information
        /// </summary>
        /// <param name="reader">XmlReader representing a machine block</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        private void ReadMachine(XmlReader reader, string filename, int indexId)
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

            Machine machine = new Machine
            {
                Name = reader.GetAttribute("name"),
                Description = reader.GetAttribute("name"),
                CloneOf = reader.GetAttribute("cloneof"),
                RomOf = reader.GetAttribute("romof"),
                SampleOf = reader.GetAttribute("sampleof"),
                MachineType = (machineType == 0x0 ? MachineType.NULL : machineType),

                SourceFile = reader.GetAttribute("sourcefile"),
                Runnable = reader.GetAttribute("runnable").AsRunnable(),
            };

            // Get list for new DatItems
            List<DatItem> datItems = new List<DatItem>();

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
                        datItems.Add(new Chip
                        {
                            Name = reader.GetAttribute("name"),
                            Tag = reader.GetAttribute("tag"),
                            ChipType = reader.GetAttribute("type").AsChipType(),
                            Clock = reader.GetAttribute("clock"),

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        });

                        reader.Read();
                        break;

                    case "condition":
                        datItems.Add(new Condition
                        {
                            Tag = reader.GetAttribute("tag"),
                            Mask = reader.GetAttribute("mask"),
                            Relation = reader.GetAttribute("relation"),
                            ConditionValue = reader.GetAttribute("value"),

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
                            DeviceType = reader.GetAttribute("type"),
                            Tag = reader.GetAttribute("tag"),
                            FixedImage = reader.GetAttribute("fixed_image"),
                            Mandatory = reader.GetAttribute("mandatory"),
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
                        datItems.Add(new Display
                        {
                            Tag = reader.GetAttribute("tag"),
                            DisplayType = reader.GetAttribute("type"),
                            Rotate = reader.GetAttribute("rotate"),
                            FlipX = reader.GetAttribute("flipx").AsYesNo(),
                            Width = reader.GetAttribute("width"),
                            Height = reader.GetAttribute("height"),
                            Refresh = reader.GetAttribute("refresh"),
                            PixClock = reader.GetAttribute("pixclock"),
                            HTotal = reader.GetAttribute("htotal"),
                            HBEnd = reader.GetAttribute("hbend"),
                            HBStart = reader.GetAttribute("hbstart"),
                            VTotal = reader.GetAttribute("vtotal"),
                            VBEnd = reader.GetAttribute("vbend"),
                            VBStart = reader.GetAttribute("vbstart"),

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        });

                        reader.Read();
                        break;

                    case "driver":
                        datItems.Add(new Driver
                        {
                            Status = reader.GetAttribute("status").AsSupportStatus(),
                            Emulation = reader.GetAttribute("emulation").AsSupportStatus(),
                            Cocktail = reader.GetAttribute("cocktail").AsSupportStatus(),
                            SaveState = reader.GetAttribute("savestate").AsSupported(),

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
                            Players = reader.GetAttribute("players"),
                            Coins = reader.GetAttribute("coins"),

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

                    case "rom":
                        datItems.Add(new Rom
                        {
                            Name = reader.GetAttribute("name"),
                            Bios = reader.GetAttribute("bios"),
                            Size = Sanitizer.CleanSize(reader.GetAttribute("size")),
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
                        datItems.Add(new DatItems.SoftwareList
                        {
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
                        datItems.Add(new Sound
                        {
                            Channels = reader.GetAttribute("channels"),

                            Source = new Source
                            {
                                Index = indexId,
                                Name = filename,
                            },
                        });

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
                    ParseAddHelper(datItem);
                }
            }

            // If no items were found for this machine, add a Blank placeholder
            else
            {
                Blank blank = new Blank()
                {
                    Source = new Source
                    {
                        Index = indexId,
                        Name = filename,
                    },
                };

                blank.CopyMachineInformation(machine);

                // Now process and add the rom
                ParseAddHelper(blank);
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
                        var slotOption = new SlotOption();
                        slotOption.Name = reader.GetAttribute("name");
                        slotOption.DeviceName = reader.GetAttribute("devname");
                        slotOption.Default = reader.GetAttribute("default").AsYesNo();

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
                        var control = new Control();
                        control.ControlType = reader.GetAttribute("type");
                        control.Player = reader.GetAttribute("player");
                        control.Buttons = reader.GetAttribute("buttons");
                        control.RegButtons = reader.GetAttribute("regbuttons");
                        control.Minimum = reader.GetAttribute("minimum");
                        control.Maximum = reader.GetAttribute("maximum");
                        control.Sensitivity = reader.GetAttribute("sensitivity");
                        control.KeyDelta = reader.GetAttribute("keydelta");
                        control.Reverse = reader.GetAttribute("reverse").AsYesNo();
                        control.Ways = reader.GetAttribute("ways");
                        control.Ways2 = reader.GetAttribute("ways2");
                        control.Ways3 = reader.GetAttribute("ways3");

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
                        var condition = new Condition();
                        condition.Tag = reader.GetAttribute("tag");
                        condition.Mask = reader.GetAttribute("mask");
                        condition.Relation = reader.GetAttribute("relation");
                        condition.Value = reader.GetAttribute("value");

                        dipSwitch.Conditions.Add(condition);

                        reader.Read();
                        break;

                    case "diplocation":
                        var dipLocation = new Location();
                        dipLocation.Name = reader.GetAttribute("name");
                        dipLocation.Number = reader.GetAttribute("number");
                        dipLocation.Inverted = reader.GetAttribute("inverted").AsYesNo();

                        dipSwitch.Locations.Add(dipLocation);

                        reader.Read();
                        break;

                    case "dipvalue":
                        var dipValue = new Setting();
                        dipValue.Name = reader.GetAttribute("name");
                        dipValue.Value = reader.GetAttribute("value");
                        dipValue.Default = reader.GetAttribute("default").AsYesNo();

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
                        var condition = new Condition();
                        condition.Tag = reader.GetAttribute("tag");
                        condition.Mask = reader.GetAttribute("mask");
                        condition.Relation = reader.GetAttribute("relation");
                        condition.Value = reader.GetAttribute("value");

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
                        var condition = new Condition();
                        condition.Tag = reader.GetAttribute("tag");
                        condition.Mask = reader.GetAttribute("mask");
                        condition.Relation = reader.GetAttribute("relation");
                        condition.Value = reader.GetAttribute("value");

                        configuration.Conditions.Add(condition);

                        reader.Read();
                        break;

                    case "conflocation":
                        var confLocation = new Location();
                        confLocation.Name = reader.GetAttribute("name");
                        confLocation.Number = reader.GetAttribute("number");
                        confLocation.Inverted = reader.GetAttribute("inverted").AsYesNo();

                        configuration.Locations.Add(confLocation);

                        reader.Read();
                        break;

                    case "confsetting":
                        var confSetting = new Setting();
                        confSetting.Name = reader.GetAttribute("name");
                        confSetting.Value = reader.GetAttribute("value");
                        confSetting.Default = reader.GetAttribute("default").AsYesNo();

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
                        var condition = new Condition();
                        condition.Tag = reader.GetAttribute("tag");
                        condition.Mask = reader.GetAttribute("mask");
                        condition.Relation = reader.GetAttribute("relation");
                        condition.Value = reader.GetAttribute("value");

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
                        var analog = new Analog();
                        analog.Mask = reader.GetAttribute("mask");

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
                        var condition = new Condition();
                        condition.Tag = reader.GetAttribute("tag");
                        condition.Mask = reader.GetAttribute("mask");
                        condition.Relation = reader.GetAttribute("relation");
                        condition.Value = reader.GetAttribute("value");

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

        /// <summary>
        /// Create and open an output file for writing direct from a dictionary
        /// </summary>
        /// <param name="outfile">Name of the file to write to</param>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise (default)</param>
        /// <returns>True if the DAT was written correctly, false otherwise</returns>
        public override bool WriteToFile(string outfile, bool ignoreblanks = false)
        {
            try
            {
                Globals.Logger.User($"Opening file for writing: {outfile}");
                FileStream fs = FileExtensions.TryCreate(outfile);

                // If we get back null for some reason, just log and return
                if (fs == null)
                {
                    Globals.Logger.Warning($"File '{outfile}' could not be created for writing! Please check to see if the file is writable");
                    return false;
                }

                XmlTextWriter xtw = new XmlTextWriter(fs, new UTF8Encoding(false))
                {
                    Formatting = Formatting.Indented,
                    IndentChar = '\t',
                    Indentation = 1
                };

                // Write out the header
                WriteHeader(xtw);

                // Write out each of the machines and roms
                string lastgame = null;

                // Use a sorted list of games to output
                foreach (string key in Items.SortedKeys)
                {
                    List<DatItem> datItems = Items.FilteredItems(key);

                    // Resolve the names in the block
                    datItems = DatItem.ResolveNames(datItems);

                    for (int index = 0; index < datItems.Count; index++)
                    {
                        DatItem datItem = datItems[index];

                        // If we have a different game and we're not at the start of the list, output the end of last item
                        if (lastgame != null && lastgame.ToLowerInvariant() != datItem.Machine.Name.ToLowerInvariant())
                            WriteEndGame(xtw);

                        // If we have a new game, output the beginning of the new item
                        if (lastgame == null || lastgame.ToLowerInvariant() != datItem.Machine.Name.ToLowerInvariant())
                            WriteStartGame(xtw, datItem);

                        // Check for a "null" item
                        datItem = ProcessNullifiedItem(datItem);

                        // Write out the item if we're not ignoring
                        if (!ShouldIgnore(datItem, ignoreblanks))
                            WriteDatItem(xtw, datItem);

                        // Set the new data to compare against
                        lastgame = datItem.Machine.Name;
                    }
                }

                // Write the file footer out
                WriteFooter(xtw);

                Globals.Logger.Verbose("File written!" + Environment.NewLine);
                xtw.Dispose();
                fs.Dispose();
            }
            catch (Exception ex)
            {
                Globals.Logger.Error(ex.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Write out DAT header using the supplied StreamWriter
        /// </summary>
        /// <param name="xtw">XmlTextWriter to output to</param>
        /// <returns>True if the data was written, false on error</returns>
        private bool WriteHeader(XmlTextWriter xtw)
        {
            try
            {
                xtw.WriteStartDocument();

                xtw.WriteStartElement("mame");
                xtw.WriteRequiredAttributeString("build", Header.Name);
                xtw.WriteOptionalAttributeString("debug", Header.Debug.FromYesNo());
                xtw.WriteOptionalAttributeString("mameconfig", Header.MameConfig);

                xtw.Flush();
            }
            catch (Exception ex)
            {
                Globals.Logger.Error(ex.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Write out Game start using the supplied StreamWriter
        /// </summary>
        /// <param name="xtw">XmlTextWriter to output to</param>
        /// <param name="datItem">DatItem object to be output</param>
        /// <returns>True if the data was written, false on error</returns>
        private bool WriteStartGame(XmlTextWriter xtw, DatItem datItem)
        {
            try
            {
                // No game should start with a path separator
                datItem.Machine.Name = datItem.Machine.Name.TrimStart(Path.DirectorySeparatorChar);

                // Build the state
                xtw.WriteStartElement("machine");
                xtw.WriteRequiredAttributeString("name", datItem.Machine.Name);
                xtw.WriteOptionalAttributeString("sourcefile", datItem.Machine.SourceFile);

                if (datItem.Machine.MachineType.HasFlag(MachineType.Bios))
                    xtw.WriteAttributeString("isbios", "yes");
                if (datItem.Machine.MachineType.HasFlag(MachineType.Device))
                    xtw.WriteAttributeString("isdevice", "yes");
                if (datItem.Machine.MachineType.HasFlag(MachineType.Mechanical))
                    xtw.WriteAttributeString("ismechanical", "yes");

                xtw.WriteOptionalAttributeString("runnable", datItem.Machine.Runnable.FromRunnable());

                if (!string.Equals(datItem.Machine.Name, datItem.Machine.CloneOf, StringComparison.OrdinalIgnoreCase))
                    xtw.WriteOptionalAttributeString("cloneof", datItem.Machine.CloneOf);
                if (!string.Equals(datItem.Machine.Name, datItem.Machine.RomOf, StringComparison.OrdinalIgnoreCase))
                    xtw.WriteOptionalAttributeString("romof", datItem.Machine.RomOf);
                if (!string.Equals(datItem.Machine.Name, datItem.Machine.SampleOf, StringComparison.OrdinalIgnoreCase))
                    xtw.WriteOptionalAttributeString("sampleof", datItem.Machine.SampleOf);

                xtw.WriteOptionalElementString("description", datItem.Machine.Description);
                xtw.WriteOptionalElementString("year", datItem.Machine.Year);
                xtw.WriteOptionalElementString("manufacturer", datItem.Machine.Manufacturer);

                xtw.Flush();
            }
            catch (Exception ex)
            {
                Globals.Logger.Error(ex.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Write out Game start using the supplied StreamWriter
        /// </summary>
        /// <param name="xtw">XmlTextWriter to output to</param>
        /// <returns>True if the data was written, false on error</returns>
        private bool WriteEndGame(XmlTextWriter xtw)
        {
            try
            {
                // End machine
                xtw.WriteEndElement();

                xtw.Flush();
            }
            catch (Exception ex)
            {
                Globals.Logger.Error(ex.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Write out DatItem using the supplied StreamWriter
        /// </summary>
        /// <param name="xtw">XmlTextWriter to output to</param>
        /// <param name="datItem">DatItem object to be output</param>
        /// <returns>True if the data was written, false on error</returns>
        private bool WriteDatItem(XmlTextWriter xtw, DatItem datItem)
        {
            try
            {
                // Pre-process the item name
                ProcessItemName(datItem, true);

                // Build the state
                switch (datItem.ItemType)
                {
                    case ItemType.Adjuster:
                        var adjuster = datItem as Adjuster;
                        xtw.WriteStartElement("adjuster");
                        xtw.WriteRequiredAttributeString("name", adjuster.Name);
                        xtw.WriteOptionalAttributeString("default", adjuster.Default.FromYesNo());
                        if (adjuster.Conditions != null)
                        {
                            foreach (var adjusterCondition in adjuster.Conditions)
                            {
                                xtw.WriteStartElement("condition");
                                xtw.WriteOptionalAttributeString("tag", adjusterCondition.Tag);
                                xtw.WriteOptionalAttributeString("mask", adjusterCondition.Mask);
                                xtw.WriteOptionalAttributeString("relation", adjusterCondition.Relation);
                                xtw.WriteOptionalAttributeString("value", adjusterCondition.Value);
                                xtw.WriteEndElement();
                            }
                        }
                        xtw.WriteEndElement();
                        break;

                    case ItemType.BiosSet:
                        var biosSet = datItem as BiosSet;
                        xtw.WriteStartElement("biosset");
                        xtw.WriteRequiredAttributeString("name", biosSet.Name);
                        xtw.WriteOptionalAttributeString("description", biosSet.Description);
                        xtw.WriteOptionalAttributeString("default", biosSet.Default?.ToString().ToLowerInvariant());
                        xtw.WriteEndElement();
                        break;

                    case ItemType.Chip:
                        var chip = datItem as Chip;
                        xtw.WriteStartElement("chip");
                        xtw.WriteRequiredAttributeString("name", chip.Name);
                        xtw.WriteOptionalAttributeString("tag", chip.Tag);
                        xtw.WriteOptionalAttributeString("type", chip.ChipType.FromChipType());
                        xtw.WriteOptionalAttributeString("clock", chip.Clock);
                        xtw.WriteEndElement();
                        break;

                    case ItemType.Condition:
                        var condition = datItem as Condition;
                        xtw.WriteStartElement("condition");
                        xtw.WriteOptionalAttributeString("tag", condition.Tag);
                        xtw.WriteOptionalAttributeString("mask", condition.Mask);
                        xtw.WriteOptionalAttributeString("relation", condition.Relation);
                        xtw.WriteOptionalAttributeString("value", condition.Value);
                        xtw.WriteEndElement();
                        break;

                    case ItemType.Configuration:
                        var configuration = datItem as Configuration;
                        xtw.WriteStartElement("configuration");
                        xtw.WriteOptionalAttributeString("name", configuration.Name);
                        xtw.WriteOptionalAttributeString("tag", configuration.Tag);
                        xtw.WriteOptionalAttributeString("mask", configuration.Mask);

                        if (configuration.Conditions != null)
                        {
                            foreach (var configurationCondition in configuration.Conditions)
                            {
                                xtw.WriteStartElement("condition");
                                xtw.WriteOptionalAttributeString("tag", configurationCondition.Tag);
                                xtw.WriteOptionalAttributeString("mask", configurationCondition.Mask);
                                xtw.WriteOptionalAttributeString("relation", configurationCondition.Relation);
                                xtw.WriteOptionalAttributeString("value", configurationCondition.Value);
                                xtw.WriteEndElement();
                            }
                        }
                        if (configuration.Locations != null)
                        {
                            foreach (var location in configuration.Locations)
                            {
                                xtw.WriteStartElement("conflocation");
                                xtw.WriteOptionalAttributeString("name", location.Name);
                                xtw.WriteOptionalAttributeString("number", location.Number);
                                xtw.WriteOptionalAttributeString("inverted", location.Inverted.FromYesNo());
                                xtw.WriteEndElement();
                            }
                        }
                        if (configuration.Settings != null)
                        {
                            foreach (var setting in configuration.Settings)
                            {
                                xtw.WriteStartElement("confsetting");
                                xtw.WriteOptionalAttributeString("name", setting.Name);
                                xtw.WriteOptionalAttributeString("value", setting.Value);
                                xtw.WriteOptionalAttributeString("default", setting.Default.FromYesNo());
                                xtw.WriteEndElement();
                            }
                        }
                        xtw.WriteEndElement();
                        break;

                    case ItemType.Device:
                        var device = datItem as Device;
                        xtw.WriteStartElement("device");
                        xtw.WriteOptionalAttributeString("type", device.DeviceType);
                        xtw.WriteOptionalAttributeString("tag", device.Tag);
                        xtw.WriteOptionalAttributeString("fixed_image", device.FixedImage);
                        xtw.WriteOptionalAttributeString("mandatory", device.Mandatory);
                        xtw.WriteOptionalAttributeString("interface", device.Interface);
                        if (device.Instances != null)
                        {
                            foreach (var instance in device.Instances)
                            {
                                xtw.WriteStartElement("instance");
                                xtw.WriteOptionalAttributeString("name", instance.Name);
                                xtw.WriteOptionalAttributeString("briefname", instance.BriefName);
                                xtw.WriteEndElement();
                            }
                        }
                        if (device.Extensions != null)
                        {
                            foreach (var extension in device.Extensions)
                            {
                                xtw.WriteStartElement("extension");
                                xtw.WriteOptionalAttributeString("name", extension.Name);
                                xtw.WriteEndElement();
                            }
                        }
                        xtw.WriteEndElement();
                        break;

                    case ItemType.DeviceReference:
                        var deviceRef = datItem as DeviceReference;
                        xtw.WriteStartElement("device_ref");
                        xtw.WriteRequiredAttributeString("name", deviceRef.Name);
                        xtw.WriteEndElement();
                        break;

                    case ItemType.DipSwitch:
                        var dipSwitch = datItem as DipSwitch;
                        xtw.WriteStartElement("dipswitch");
                        xtw.WriteOptionalAttributeString("name", dipSwitch.Name);
                        xtw.WriteOptionalAttributeString("tag", dipSwitch.Tag);
                        xtw.WriteOptionalAttributeString("mask", dipSwitch.Mask);
                        if (dipSwitch.Conditions != null)
                        {
                            foreach (var dipSwitchCondition in dipSwitch.Conditions)
                            {
                                xtw.WriteStartElement("condition");
                                xtw.WriteOptionalAttributeString("tag", dipSwitchCondition.Tag);
                                xtw.WriteOptionalAttributeString("mask", dipSwitchCondition.Mask);
                                xtw.WriteOptionalAttributeString("relation", dipSwitchCondition.Relation);
                                xtw.WriteOptionalAttributeString("value", dipSwitchCondition.Value);
                                xtw.WriteEndElement();
                            }
                        }
                        if (dipSwitch.Locations != null)
                        {
                            foreach (var location in dipSwitch.Locations)
                            {
                                xtw.WriteStartElement("diplocation");
                                xtw.WriteOptionalAttributeString("name", location.Name);
                                xtw.WriteOptionalAttributeString("number", location.Number);
                                xtw.WriteOptionalAttributeString("inverted", location.Inverted.FromYesNo());
                                xtw.WriteEndElement();
                            }
                        }
                        if (dipSwitch.Values != null)
                        {
                            foreach (var value in dipSwitch.Values)
                            {
                                xtw.WriteStartElement("dipvalue");
                                xtw.WriteOptionalAttributeString("name", value.Name);
                                xtw.WriteOptionalAttributeString("value", value.Value);
                                xtw.WriteOptionalAttributeString("default", value.Default.FromYesNo());
                                if (value.Conditions != null)
                                {
                                    foreach (var dipValueCondition in value.Conditions)
                                    {
                                        xtw.WriteStartElement("condition");
                                        xtw.WriteOptionalAttributeString("tag", dipValueCondition.Tag);
                                        xtw.WriteOptionalAttributeString("mask", dipValueCondition.Mask);
                                        xtw.WriteOptionalAttributeString("relation", dipValueCondition.Relation);
                                        xtw.WriteOptionalAttributeString("value", dipValueCondition.Value);
                                        xtw.WriteEndElement();
                                    }
                                }
                                xtw.WriteEndElement();
                            }
                        }
                        xtw.WriteEndElement();
                        break;

                    case ItemType.Disk:
                        var disk = datItem as Disk;
                        xtw.WriteStartElement("disk");
                        xtw.WriteRequiredAttributeString("name", disk.Name);
                        xtw.WriteOptionalAttributeString("sha1", disk.SHA1?.ToLowerInvariant());
                        xtw.WriteOptionalAttributeString("merge", disk.MergeTag);
                        xtw.WriteOptionalAttributeString("region", disk.Region);
                        xtw.WriteOptionalAttributeString("index", disk.Index);
                        xtw.WriteOptionalAttributeString("writable", disk.Writable.FromYesNo());
                        xtw.WriteOptionalAttributeString("status", disk.ItemStatus.FromItemStatus(false));
                        xtw.WriteOptionalAttributeString("optional", disk.Optional.FromYesNo());
                        xtw.WriteEndElement();
                        break;

                    case ItemType.Display:
                        var display = datItem as Display;
                        xtw.WriteStartElement("display");
                        xtw.WriteOptionalAttributeString("tag", display.Tag);
                        xtw.WriteOptionalAttributeString("type", display.DisplayType);
                        xtw.WriteOptionalAttributeString("rotate", display.Rotate);
                        xtw.WriteOptionalAttributeString("flipx", display.FlipX.FromYesNo());
                        xtw.WriteOptionalAttributeString("width", display.Width);
                        xtw.WriteOptionalAttributeString("height", display.Height);
                        xtw.WriteOptionalAttributeString("refresh", display.Refresh);
                        xtw.WriteOptionalAttributeString("pixclock", display.PixClock);
                        xtw.WriteOptionalAttributeString("htotal", display.HTotal);
                        xtw.WriteOptionalAttributeString("hbend", display.HBEnd);
                        xtw.WriteOptionalAttributeString("hstart", display.HBStart);
                        xtw.WriteOptionalAttributeString("vtotal", display.VTotal);
                        xtw.WriteOptionalAttributeString("vbend", display.VBEnd);
                        xtw.WriteOptionalAttributeString("vbstart", display.VBStart);
                        xtw.WriteEndElement();
                        break;

                    case ItemType.Driver:
                        var driver = datItem as Driver;
                        xtw.WriteStartElement("driver");
                        xtw.WriteOptionalAttributeString("status", driver.Status.FromSupportStatus());
                        xtw.WriteOptionalAttributeString("emulation", driver.Emulation.FromSupportStatus());
                        xtw.WriteOptionalAttributeString("cocktail", driver.Cocktail.FromSupportStatus());
                        xtw.WriteOptionalAttributeString("savestate", driver.SaveState.FromSupported(true));
                        xtw.WriteEndElement();
                        break;

                    case ItemType.Feature:
                        var feature = datItem as Feature;
                        xtw.WriteStartElement("feature");
                        xtw.WriteOptionalAttributeString("type", feature.Type.FromFeatureType());
                        xtw.WriteOptionalAttributeString("status", feature.Status.FromFeatureStatus());
                        xtw.WriteOptionalAttributeString("overall", feature.Overall.FromFeatureStatus());
                        xtw.WriteEndElement();
                        break;

                    case ItemType.Input:
                        var input = datItem as Input;
                        xtw.WriteStartElement("input");
                        xtw.WriteOptionalAttributeString("service", input.Service.FromYesNo());
                        xtw.WriteOptionalAttributeString("tilt", input.Tilt.FromYesNo());
                        xtw.WriteOptionalAttributeString("players", input.Players);
                        xtw.WriteOptionalAttributeString("coins", input.Coins);
                        if (input.Controls != null)
                        {
                            foreach (var control in input.Controls)
                            {
                                xtw.WriteStartElement("control");
                                xtw.WriteOptionalAttributeString("type", control.ControlType);
                                xtw.WriteOptionalAttributeString("player", control.Player);
                                xtw.WriteOptionalAttributeString("buttons", control.Buttons);
                                xtw.WriteOptionalAttributeString("regbuttons", control.RegButtons);
                                xtw.WriteOptionalAttributeString("minimum", control.Minimum);
                                xtw.WriteOptionalAttributeString("maximum", control.Maximum);
                                xtw.WriteOptionalAttributeString("sensitivity", control.Sensitivity);
                                xtw.WriteOptionalAttributeString("keydelta", control.KeyDelta);
                                xtw.WriteOptionalAttributeString("reverse", control.Reverse.FromYesNo());
                                xtw.WriteOptionalAttributeString("ways", control.Ways);
                                xtw.WriteOptionalAttributeString("ways2", control.Ways2);
                                xtw.WriteOptionalAttributeString("ways3", control.Ways3);
                                xtw.WriteEndElement();
                            }
                        }
                        xtw.WriteEndElement();
                        break;

                    case ItemType.Port:
                        var port = datItem as Port;
                        xtw.WriteStartElement("port");
                        xtw.WriteOptionalAttributeString("tag", port.Tag);
                        if (port.Analogs != null)
                        {
                            foreach (var analog in port.Analogs)
                            {
                                xtw.WriteStartElement("analog");
                                xtw.WriteOptionalAttributeString("mask", analog.Mask);
                                xtw.WriteEndElement();
                            }
                        }
                        xtw.WriteEndElement();
                        break;

                    case ItemType.RamOption:
                        var ramOption = datItem as RamOption;
                        xtw.WriteStartElement("ramoption");
                        xtw.WriteRequiredAttributeString("name", ramOption.Name);
                        xtw.WriteOptionalAttributeString("default", ramOption.Default.FromYesNo());
                        xtw.WriteRaw(ramOption.Content ?? string.Empty);
                        xtw.WriteFullEndElement();
                        break;

                    case ItemType.Rom:
                        var rom = datItem as Rom;
                        xtw.WriteStartElement("rom");
                        xtw.WriteRequiredAttributeString("name", rom.Name);
                        if (rom.Size != -1) xtw.WriteAttributeString("size", rom.Size.ToString());
                        xtw.WriteOptionalAttributeString("crc", rom.CRC?.ToLowerInvariant());
                        xtw.WriteOptionalAttributeString("sha1", rom.SHA1?.ToLowerInvariant());
                        xtw.WriteOptionalAttributeString("bios", rom.Bios);
                        xtw.WriteOptionalAttributeString("merge", rom.MergeTag);
                        xtw.WriteOptionalAttributeString("region", rom.Region);
                        xtw.WriteOptionalAttributeString("offset", rom.Offset);
                        xtw.WriteOptionalAttributeString("status", rom.ItemStatus.FromItemStatus(false));
                        xtw.WriteOptionalAttributeString("optional", rom.Optional.FromYesNo());
                        xtw.WriteEndElement();
                        break;

                    case ItemType.Sample:
                        var sample = datItem as Sample;
                        xtw.WriteStartElement("sample");
                        xtw.WriteRequiredAttributeString("name", sample.Name);
                        xtw.WriteEndElement();
                        break;

                    case ItemType.Slot:
                        var slot = datItem as Slot;
                        xtw.WriteStartElement("slot");
                        xtw.WriteOptionalAttributeString("name", slot.Name);
                        if (slot.SlotOptions != null)
                        {
                            foreach (var slotOption in slot.SlotOptions)
                            {
                                xtw.WriteStartElement("slotoption");
                                xtw.WriteOptionalAttributeString("name", slotOption.Name);
                                xtw.WriteOptionalAttributeString("devname", slotOption.DeviceName);
                                xtw.WriteOptionalAttributeString("default", slotOption.Default.FromYesNo());
                                xtw.WriteEndElement();
                            }
                        }
                        xtw.WriteEndElement();
                        break;

                    case ItemType.SoftwareList:
                        var softwareList = datItem as DatItems.SoftwareList;
                        xtw.WriteStartElement("softwarelist");
                        xtw.WriteRequiredAttributeString("name", softwareList.Name);
                        xtw.WriteOptionalAttributeString("status", softwareList.Status.FromSoftwareListStatus());
                        xtw.WriteOptionalAttributeString("filter", softwareList.Filter);
                        xtw.WriteEndElement();
                        break;

                    case ItemType.Sound:
                        var sound = datItem as Sound;
                        xtw.WriteStartElement("sound");
                        xtw.WriteOptionalAttributeString("channels", sound.Channels);
                        xtw.WriteEndElement();
                        break;
                }

                xtw.Flush();
            }
            catch (Exception ex)
            {
                Globals.Logger.Error(ex.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Write out DAT footer using the supplied StreamWriter
        /// </summary>
        /// <param name="xtw">XmlTextWriter to output to</param>
        /// <returns>True if the data was written, false on error</returns>
        private bool WriteFooter(XmlTextWriter xtw)
        {
            try
            {
                // End machine
                xtw.WriteEndElement();

                // End mame
                xtw.WriteEndElement();

                xtw.Flush();
            }
            catch (Exception ex)
            {
                Globals.Logger.Error(ex.ToString());
                return false;
            }

            return true;
        }
    }
}
