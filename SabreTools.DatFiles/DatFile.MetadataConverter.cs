using System.Linq;

namespace SabreTools.DatFiles
{
    // TODO: Have the converters return a value OR port functionality to DatFile
    // TODO: Figure out if there's a way to condense the various processing methods
    // TODO: Convert nested items (e.g. Configuration, DipLocation)
    // TODO: Determine which items need to have their values flipped (e.g. Part, DiskArea, DataArea)
    public partial class DatFile
    {
         #region Converters

        /// <summary>
        /// Convert metadata information
        /// </summary>
        /// <param name="item">Metadata file to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        public void ConvertMetadata(Models.Metadata.MetadataFile? item, string filename, int indexId, bool statsOnly)
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
        private void ConvertMachine(Models.Metadata.Machine? item, string filename, int indexId, bool statsOnly)
        {
            // If the machine is invalid, we can't do anything
            if (item == null || !item.Any())
                return;

            // Create an internal machine
            var machine = new DatItems.Machine(item);

            // Convert items in the machine
            if (item.ContainsKey(Models.Metadata.Machine.AdjusterKey))
            {
                var items = ReadItemArray<Models.Metadata.Adjuster>(item, Models.Metadata.Machine.AdjusterKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.ArchiveKey))
            {
                var items = ReadItemArray<Models.Metadata.Archive>(item, Models.Metadata.Machine.ArchiveKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.BiosSetKey))
            {
                var items = ReadItemArray<Models.Metadata.BiosSet>(item, Models.Metadata.Machine.BiosSetKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.ChipKey))
            {
                var items = ReadItemArray<Models.Metadata.Chip>(item, Models.Metadata.Machine.ChipKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.ConfigurationKey))
            {
                var items = ReadItemArray<Models.Metadata.Configuration>(item, Models.Metadata.Machine.ConfigurationKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.DeviceKey))
            {
                var items = ReadItemArray<Models.Metadata.Device>(item, Models.Metadata.Machine.DeviceKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.DeviceRefKey))
            {
                var items = ReadItemArray<Models.Metadata.DeviceRef>(item, Models.Metadata.Machine.DeviceRefKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.DipSwitchKey))
            {
                var items = ReadItemArray<Models.Metadata.DipSwitch>(item, Models.Metadata.Machine.DipSwitchKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.DiskKey))
            {
                var items = ReadItemArray<Models.Metadata.Disk>(item, Models.Metadata.Machine.DiskKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.DisplayKey))
            {
                var items = ReadItemArray<Models.Metadata.Display>(item, Models.Metadata.Machine.DisplayKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.DriverKey))
            {
                var items = ReadItemArray<Models.Metadata.Driver>(item, Models.Metadata.Machine.DriverKey);
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
                var items = ReadItemArray<Models.Metadata.Feature>(item, Models.Metadata.Machine.FeatureKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.InfoKey))
            {
                var items = ReadItemArray<Models.Metadata.Info>(item, Models.Metadata.Machine.InfoKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.InputKey))
            {
                var items = ReadItemArray<Models.Metadata.Input>(item, Models.Metadata.Machine.InputKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.MediaKey))
            {
                var items = ReadItemArray<Models.Metadata.Media>(item, Models.Metadata.Machine.MediaKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.PartKey))
            {
                var items = ReadItemArray<Models.Metadata.Part>(item, Models.Metadata.Machine.PartKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.PortKey))
            {
                var items = ReadItemArray<Models.Metadata.Port>(item, Models.Metadata.Machine.PortKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.RamOptionKey))
            {
                var items = ReadItemArray<Models.Metadata.RamOption>(item, Models.Metadata.Machine.RamOptionKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.ReleaseKey))
            {
                var items = ReadItemArray<Models.Metadata.Release>(item, Models.Metadata.Machine.ReleaseKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.RomKey))
            {
                var items = ReadItemArray<Models.Metadata.Rom>(item, Models.Metadata.Machine.RomKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.SampleKey))
            {
                var items = ReadItemArray<Models.Metadata.Sample>(item, Models.Metadata.Machine.SampleKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.SharedFeatKey))
            {
                var items = ReadItemArray<Models.Metadata.SharedFeat>(item, Models.Metadata.Machine.SharedFeatKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.SoftwareListKey))
            {
                var items = ReadItemArray<Models.Metadata.SoftwareList>(item, Models.Metadata.Machine.SoftwareListKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.SoundKey))
            {
                var items = ReadItemArray<Models.Metadata.Sound>(item, Models.Metadata.Machine.SoundKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
            if (item.ContainsKey(Models.Metadata.Machine.TruripKey))
            {
                // TODO: Figure out what this maps to
            }
            if (item.ContainsKey(Models.Metadata.Machine.VideoKey))
            {
                var items = ReadItemArray<Models.Metadata.Video>(item, Models.Metadata.Machine.VideoKey);
                ProcessItems(items, machine, filename, indexId, statsOnly);
            }
        }

        /// <summary>
        /// Read an item array from a given key, if possible
        /// </summary>
        private static T[]? ReadItemArray<T>(Models.Metadata.DictionaryBase item, string key) where T : Models.Metadata.DictionaryBase
        {
            var items = item.Read<T[]>(key);
            if (items == default)
            {
                var single = item.Read<T>(key);
                if (single != default)
                    items = [single];
            }

            return items;
        }

        /// <summary>
        /// Convert Adjuster information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.Adjuster[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
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
                ParseAddHelper(datItem, statsOnly);
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
        private void ProcessItems(Models.Metadata.Archive[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
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
                ParseAddHelper(datItem, statsOnly);
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
        private void ProcessItems(Models.Metadata.BiosSet[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
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
                ParseAddHelper(datItem, statsOnly);
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
        private void ProcessItems(Models.Metadata.Chip[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
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
                ParseAddHelper(datItem, statsOnly);
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
        private void ProcessItems(Models.Metadata.Configuration[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
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
                ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert DataArea information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.DataArea[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // TODO: Extract Roms

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.DataArea(item)
                {
                    Source = new DatItems.Source { Index = indexId, Name = filename }
                };
                datItem.CopyMachineInformation(machine);
                ParseAddHelper(datItem, statsOnly);
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
        private void ProcessItems(Models.Metadata.Device[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
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
                ParseAddHelper(datItem, statsOnly);
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
        private void ProcessItems(Models.Metadata.DeviceRef[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
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
                ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert DipLocation information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.DipLocation[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.DipLocation(item)
                {
                    Source = new DatItems.Source { Index = indexId, Name = filename }
                };
                datItem.CopyMachineInformation(machine);
                ParseAddHelper(datItem, statsOnly);
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
        private void ProcessItems(Models.Metadata.DipSwitch[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
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
                ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert DipValue information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.DipValue[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.DipValue(item)
                {
                    Source = new DatItems.Source { Index = indexId, Name = filename }
                };
                datItem.CopyMachineInformation(machine);
                ParseAddHelper(datItem, statsOnly);
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
        private void ProcessItems(Models.Metadata.Disk[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
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
                ParseAddHelper(datItem, statsOnly);
            }
        }

        /// <summary>
        /// Convert DiskArea information 
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        private void ProcessItems(Models.Metadata.DiskArea[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // TODO: Extract Disks

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.DiskArea(item)
                {
                    Source = new DatItems.Source { Index = indexId, Name = filename }
                };
                datItem.CopyMachineInformation(machine);
                ParseAddHelper(datItem, statsOnly);
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
        private void ProcessItems(Models.Metadata.Display[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
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
                ParseAddHelper(datItem, statsOnly);
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
        private void ProcessItems(Models.Metadata.Driver[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
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
                ParseAddHelper(datItem, statsOnly);
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
        private void ProcessItems(Models.Metadata.Feature[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
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
                ParseAddHelper(datItem, statsOnly);
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
        private void ProcessItems(Models.Metadata.Info[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
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
                ParseAddHelper(datItem, statsOnly);
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
        private void ProcessItems(Models.Metadata.Input[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
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
                ParseAddHelper(datItem, statsOnly);
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
        private void ProcessItems(Models.Metadata.Media[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
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
                ParseAddHelper(datItem, statsOnly);
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
        private void ProcessItems(Models.Metadata.Part[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
        {
            // If the array is null or empty, return without processing
            if (items == null || items.Length == 0)
                return;

            // TODO: Extract DataAreas
            // TODO: Extract DiskAreas
            // TODO: Extract DipSwitches

            // Loop through the items and add
            foreach (var item in items)
            {
                var datItem = new DatItems.Formats.Part(item)
                {
                    Source = new DatItems.Source { Index = indexId, Name = filename }
                };
                datItem.CopyMachineInformation(machine);
                ParseAddHelper(datItem, statsOnly);
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
        private void ProcessItems(Models.Metadata.Port[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
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
                ParseAddHelper(datItem, statsOnly);
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
        private void ProcessItems(Models.Metadata.RamOption[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
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
                ParseAddHelper(datItem, statsOnly);
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
        private void ProcessItems(Models.Metadata.Release[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
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
                ParseAddHelper(datItem, statsOnly);
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
        private void ProcessItems(Models.Metadata.Rom[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
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
                ParseAddHelper(datItem, statsOnly);
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
        private void ProcessItems(Models.Metadata.Sample[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
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
                ParseAddHelper(datItem, statsOnly);
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
        private void ProcessItems(Models.Metadata.SharedFeat[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
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
                ParseAddHelper(datItem, statsOnly);
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
        private void ProcessItems(Models.Metadata.SoftwareList[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
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
                ParseAddHelper(datItem, statsOnly);
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
        private void ProcessItems(Models.Metadata.Sound[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
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
                ParseAddHelper(datItem, statsOnly);
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
        private void ProcessItems(Models.Metadata.Video[]? items, DatItems.Machine machine, string filename, int indexId, bool statsOnly)
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
                ParseAddHelper(datItem, statsOnly);
            }
        }

        #endregion
    }
}