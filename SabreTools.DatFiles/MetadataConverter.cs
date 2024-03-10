using System.Linq;
using SabreTools.DatItems;
using SabreTools.Filter;

namespace SabreTools.DatFiles
{
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
            var machine = new Machine(item);

            // Process all possible items
            /*
            AdjusterKey
            ArchiveKey
            BiosSetKey
            ChipKey
            ConfigurationKey
            DeviceKey
            DeviceRefKey
            DipSwitchKey
            DiskKey
            DisplayKey
            DriverKey
            DumpKey
            FeatureKey
            InfoKey
            InputKey
            MediaKey
            PartKey
            PortKey
            RamOptionKey
            ReleaseKey
            RomKey
            SampleKey
            SharedFeatKey
            SoftwareListKey
            SoundKey
            TruripKey
            VideoKey
            */
        }

        #endregion
    }
}