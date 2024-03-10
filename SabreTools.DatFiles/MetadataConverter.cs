using System.Linq;

namespace SabreTools.DatFiles
{
    // TODO: Have the converters return a value OR port functionality to DatFile
    // TODO: Figure out if there's a way to condense the various processing methods
    // TODO: Convert nested items (e.g. Configuration, DipLocation)
    // TODO: Determine which items need to have their values flipped (e.g. Part, DiskArea, DataArea)
    public static class MetadataConverter
    {
         #region Converters

        /// <summary>
        /// Convert metadata information
        /// </summary>
        /// <param name="item">Metadata file to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        public static void ConvertMetadata(Models.Metadata.MetadataFile? item, string filename, int indexId, bool statsOnly)
        {
            // If the metadata file is invalid, we can't do anything
            if (item == null || !item.Any())
                return;

            // Get the machines from the metadata
            var machines = item.Read<Models.Metadata.Machine[]>(Models.Metadata.MetadataFile.MachineKey);
            if (machines == null)
                return;

            // Loop through the machines and add
            foreach (var machine in machines)
            {
                ConvertMachine(machine, filename, indexId, statsOnly);
            }
        }

        /// <summary>
        /// Convert machine information
        /// </summary>
        /// <param name="metadata">Metadata file to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private static void ConvertMachine(Models.Metadata.Machine? item, string filename, int indexId, bool statsOnly)
        {
            // If the machine is invalid, we can't do anything
            if (item == null || !item.Any())
                return;

            // Create an internal machine
            var machine = new DatItems.Machine(item);

            // Convert items in the machine
            if (item.ContainsKey(Models.Metadata.Machine.AdjusterKey))
            {
                var items = item.Read<Models.Metadata.Adjuster[]>(Models.Metadata.Machine.AdjusterKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.ArchiveKey))
            {
                var items = item.Read<Models.Metadata.Archive[]>(Models.Metadata.Machine.ArchiveKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.BiosSetKey))
            {
                var items = item.Read<Models.Metadata.BiosSet[]>(Models.Metadata.Machine.BiosSetKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.ChipKey))
            {
                var items = item.Read<Models.Metadata.Chip[]>(Models.Metadata.Machine.ChipKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.ConfigurationKey))
            {
                var items = item.Read<Models.Metadata.Configuration[]>(Models.Metadata.Machine.ConfigurationKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.DeviceKey))
            {
                var items = item.Read<Models.Metadata.Device[]>(Models.Metadata.Machine.DeviceKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.DeviceRefKey))
            {
                var items = item.Read<Models.Metadata.DeviceRef[]>(Models.Metadata.Machine.DeviceRefKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.DipSwitchKey))
            {
                var items = item.Read<Models.Metadata.DipSwitch[]>(Models.Metadata.Machine.DipSwitchKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.DiskKey))
            {
                var items = item.Read<Models.Metadata.Disk[]>(Models.Metadata.Machine.DiskKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.DisplayKey))
            {
                var items = item.Read<Models.Metadata.Display[]>(Models.Metadata.Machine.DisplayKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.DriverKey))
            {
                var items = item.Read<Models.Metadata.Driver[]>(Models.Metadata.Machine.DriverKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.DumpKey))
            {
                // TODO: Figure out what this maps to
                // var items = item.Read<Models.Metadata.Dump[]>(Models.Metadata.Machine.DumpKey);
                // ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.FeatureKey))
            {
                var items = item.Read<Models.Metadata.Feature[]>(Models.Metadata.Machine.FeatureKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.InfoKey))
            {
                var items = item.Read<Models.Metadata.Info[]>(Models.Metadata.Machine.InfoKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.InputKey))
            {
                var items = item.Read<Models.Metadata.Input[]>(Models.Metadata.Machine.InputKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.MediaKey))
            {
                var items = item.Read<Models.Metadata.Media[]>(Models.Metadata.Machine.MediaKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.PartKey))
            {
                var items = item.Read<Models.Metadata.Part[]>(Models.Metadata.Machine.PartKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.PortKey))
            {
                var items = item.Read<Models.Metadata.Port[]>(Models.Metadata.Machine.PortKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.RamOptionKey))
            {
                var items = item.Read<Models.Metadata.RamOption[]>(Models.Metadata.Machine.RamOptionKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.ReleaseKey))
            {
                var items = item.Read<Models.Metadata.Release[]>(Models.Metadata.Machine.ReleaseKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.RomKey))
            {
                var items = item.Read<Models.Metadata.Rom[]>(Models.Metadata.Machine.RomKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.SampleKey))
            {
                var items = item.Read<Models.Metadata.Sample[]>(Models.Metadata.Machine.SampleKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.SharedFeatKey))
            {
                var items = item.Read<Models.Metadata.SharedFeat[]>(Models.Metadata.Machine.SharedFeatKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.SoftwareListKey))
            {
                var items = item.Read<Models.Metadata.SoftwareList[]>(Models.Metadata.Machine.SoftwareListKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.SoundKey))
            {
                var items = item.Read<Models.Metadata.Sound[]>(Models.Metadata.Machine.SoundKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.TruripKey))
            {
                // TODO: Figure out what this maps to
            }
            if (item.ContainsKey(Models.Metadata.Machine.VideoKey))
            {
                var items = item.Read<Models.Metadata.Video[]>(Models.Metadata.Machine.VideoKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
        }

        /// <summary>
        /// Convert Adjuster information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private static void ProcessItems(Models.Metadata.Adjuster[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Adjuster(item)
                {
                    Source = new DatItems.Source { Index = indexId, Name = filename }
                };
                datItem.CopyMachineInformation(machine);
                //ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert Archive information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private static void ProcessItems(Models.Metadata.Archive[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Archive(item)
                {
                    Source = new DatItems.Source { Index = indexId, Name = filename }
                };
                datItem.CopyMachineInformation(machine);
                //ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert BiosSet information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private static void ProcessItems(Models.Metadata.BiosSet[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.BiosSet(item)
                {
                    Source = new DatItems.Source { Index = indexId, Name = filename }
                };
                datItem.CopyMachineInformation(machine);
                //ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert Chip information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private static void ProcessItems(Models.Metadata.Chip[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Chip(item)
                {
                    Source = new DatItems.Source { Index = indexId, Name = filename }
                };
                datItem.CopyMachineInformation(machine);
                //ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert Configuration information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private static void ProcessItems(Models.Metadata.Configuration[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Configuration(item)
                {
                    Source = new DatItems.Source { Index = indexId, Name = filename }
                };
                datItem.CopyMachineInformation(machine);
                //ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert Device information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private static void ProcessItems(Models.Metadata.Device[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Device(item)
                {
                    Source = new DatItems.Source { Index = indexId, Name = filename }
                };
                datItem.CopyMachineInformation(machine);
                //ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert DeviceRef information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private static void ProcessItems(Models.Metadata.DeviceRef[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.DeviceReference(item)
                {
                    Source = new DatItems.Source { Index = indexId, Name = filename }
                };
                datItem.CopyMachineInformation(machine);
                //ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert DipSwitch information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private static void ProcessItems(Models.Metadata.DipSwitch[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.DipSwitch(item)
                {
                    Source = new DatItems.Source { Index = indexId, Name = filename }
                };
                datItem.CopyMachineInformation(machine);
                //ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert Disk information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private static void ProcessItems(Models.Metadata.Disk[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Disk(item)
                {
                    Source = new DatItems.Source { Index = indexId, Name = filename }
                };
                datItem.CopyMachineInformation(machine);
                //ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert Display information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private static void ProcessItems(Models.Metadata.Display[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Display(item)
                {
                    Source = new DatItems.Source { Index = indexId, Name = filename }
                };
                datItem.CopyMachineInformation(machine);
                //ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert Driver information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private static void ProcessItems(Models.Metadata.Driver[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Driver(item)
                {
                    Source = new DatItems.Source { Index = indexId, Name = filename }
                };
                datItem.CopyMachineInformation(machine);
                //ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert Feature information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private static void ProcessItems(Models.Metadata.Feature[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Feature(item)
                {
                    Source = new DatItems.Source { Index = indexId, Name = filename }
                };
                datItem.CopyMachineInformation(machine);
                //ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert Info information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private static void ProcessItems(Models.Metadata.Info[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Info(item)
                {
                    Source = new DatItems.Source { Index = indexId, Name = filename }
                };
                datItem.CopyMachineInformation(machine);
                //ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert Input information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private static void ProcessItems(Models.Metadata.Input[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Input(item)
                {
                    Source = new DatItems.Source { Index = indexId, Name = filename }
                };
                datItem.CopyMachineInformation(machine);
                //ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert Media information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private static void ProcessItems(Models.Metadata.Media[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Media(item)
                {
                    Source = new DatItems.Source { Index = indexId, Name = filename }
                };
                datItem.CopyMachineInformation(machine);
                //ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert Part information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private static void ProcessItems(Models.Metadata.Part[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Part(item)
                {
                    Source = new DatItems.Source { Index = indexId, Name = filename }
                };
                datItem.CopyMachineInformation(machine);
                //ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert Port information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private static void ProcessItems(Models.Metadata.Port[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Port(item)
                {
                    Source = new DatItems.Source { Index = indexId, Name = filename }
                };
                datItem.CopyMachineInformation(machine);
                //ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert RamOption information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private static void ProcessItems(Models.Metadata.RamOption[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.RamOption(item)
                {
                    Source = new DatItems.Source { Index = indexId, Name = filename }
                };
                datItem.CopyMachineInformation(machine);
                //ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert Release information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private static void ProcessItems(Models.Metadata.Release[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Release(item)
                {
                    Source = new DatItems.Source { Index = indexId, Name = filename }
                };
                datItem.CopyMachineInformation(machine);
                //ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert Rom information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private static void ProcessItems(Models.Metadata.Rom[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Rom(item)
                {
                    Source = new DatItems.Source { Index = indexId, Name = filename }
                };
                datItem.CopyMachineInformation(machine);
                //ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert Sample information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private static void ProcessItems(Models.Metadata.Sample[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Sample(item)
                {
                    Source = new DatItems.Source { Index = indexId, Name = filename }
                };
                datItem.CopyMachineInformation(machine);
                //ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert SharedFeat information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private static void ProcessItems(Models.Metadata.SharedFeat[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.SharedFeature(item)
                {
                    Source = new DatItems.Source { Index = indexId, Name = filename }
                };
                datItem.CopyMachineInformation(machine);
                //ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert SoftwareList information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private static void ProcessItems(Models.Metadata.SoftwareList[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.SoftwareList(item)
                {
                    Source = new DatItems.Source { Index = indexId, Name = filename }
                };
                datItem.CopyMachineInformation(machine);
                //ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert Sound information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private static void ProcessItems(Models.Metadata.Sound[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Sound(item)
                {
                    Source = new DatItems.Source { Index = indexId, Name = filename }
                };
                datItem.CopyMachineInformation(machine);
                //ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert Video information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private static void ProcessItems(Models.Metadata.Video[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Display(item)
                {
                    Source = new DatItems.Source { Index = indexId, Name = filename }
                };
                datItem.CopyMachineInformation(machine);
                //ParseAddHelper(datItem, statsOnly);
            }
        }

        #endregion
    }
}