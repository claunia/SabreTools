using System;
using System.Collections.Generic;
using System.Linq;

using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatFiles;
using SabreTools.DatItems;
using SabreTools.Logging;

namespace SabreTools.Filtering
{
    public class ExtraIni
    {
        #region Fields

        /// <summary>
        /// List of extras to apply
        /// </summary>
        public List<ExtraIniItem> Items { get; set; } = new List<ExtraIniItem>();

        #endregion

        #region Logging

        /// <summary>
        /// Logging object
        /// </summary>
        private readonly Logger logger;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public ExtraIni()
        {
            logger = new Logger(this);
        }

        #endregion

        #region Population

        /// <summary>
        /// Populate item using field:file inputs
        /// </summary>
        /// <param name="inputs">Field and file combinations</param>
        public void PopulateFromList(List<string> inputs)
        {
            foreach (string input in inputs)
            {
                ExtraIniItem item = new ExtraIniItem();

                // If we don't even have a possible field and file combination
                if (!input.Contains(":"))
                {
                    logger.Warning($"'{input}` is not a valid INI extras string. Valid INI extras strings are of the form 'key:value'. Please refer to README.1ST or the help feature for more details.");
                    return;
                }

                string inputTrimmed = input.Trim('"', ' ', '\t');
                string fieldString = inputTrimmed.Split(':')[0].ToLowerInvariant().Trim('"', ' ', '\t');
                string fileString = inputTrimmed[(fieldString.Length + 1)..].Trim('"', ' ', '\t');

                item.DatItemField = fieldString.AsDatItemField();
                item.MachineField = fieldString.AsMachineField();
                if (item.PopulateFromFile(fileString))
                    Items.Add(item);
            }
        }

        #endregion
    
        #region Running

        /// <summary>
        /// Apply a set of Extra INIs on the DatFile
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        /// <returns>True if the extras were applied, false on error</returns>
        public bool ApplyExtras(DatFile datFile, bool throwOnError = false)
        {
            try
            {
                // Bucket by game first
                datFile.Items.BucketBy(ItemKey.Machine, DedupeType.None);

                // Create mappings based on the extra items
                var combinedMachineMaps = CombineMachineExtras();
                var combinedDatItemMaps = CombineDatItemExtras();

                // Now get the combined set of keys
                var machines = combinedMachineMaps.Keys.Concat(combinedDatItemMaps.Keys).Distinct();

                // Apply the mappings
                foreach (string machine in machines)
                {
                    // If the key doesn't exist, continue
                    if (!datFile.Items.ContainsKey(machine))
                        continue;

                    // Get the list of DatItems for the machine
                    List<DatItem> datItems = datFile.Items[machine];

                    // Try to get the map values, if possible
                    combinedMachineMaps.TryGetValue(machine, out Dictionary<MachineField, string> machineMappings);
                    combinedDatItemMaps.TryGetValue(machine, out Dictionary<DatItemField, string> datItemMappings);

                    // Create a setter with the new mappings
                    Setter setter = new Setter
                    {
                        MachineMappings = machineMappings,
                        DatItemMappings = datItemMappings,
                    };

                    // Loop through and set the fields accordingly
                    foreach (var datItem in datItems)
                    {
                        setter.SetFields(datItem.Machine);
                        setter.SetFields(datItem);
                    }
                }
            }
            catch (Exception ex) when (!throwOnError)
            {
                logger.Error(ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Combine MachineField-based ExtraIni fields
        /// </summary>
        /// <returns>Mapping dictionary from machine name to field mapping</returns>
        private Dictionary<string, Dictionary<MachineField, string>> CombineMachineExtras()
        {
            var machineMap = new Dictionary<string, Dictionary<MachineField, string>>();

            // Loop through each of the extras
            foreach (ExtraIniItem item in Items.Where(i => i.MachineField != MachineField.NULL))
            {
                foreach (var mapping in item.Mappings)
                {
                    string machineName = mapping.Key;
                    string value = mapping.Value;
                        
                    machineMap[machineName] = new Dictionary<MachineField, string>
                    {
                        [item.MachineField] = value,
                    };
                }
            }

            return machineMap;
        }

        /// <summary>
        /// Combine DatItemField-based ExtraIni fields
        /// </summary>
        /// <returns>Mapping dictionary from machine name to field mapping</returns>
        private Dictionary<string, Dictionary<DatItemField, string>> CombineDatItemExtras()
        {
            var datItemMap = new Dictionary<string, Dictionary<DatItemField, string>>();

            // Loop through each of the extras
            foreach (ExtraIniItem item in Items.Where(i => i.DatItemField != DatItemField.NULL))
            {
                foreach (var mapping in item.Mappings)
                {
                    string machineName = mapping.Key;
                    string value = mapping.Value;
                        
                    datItemMap[machineName] = new Dictionary<DatItemField, string>()
                    {
                        [item.DatItemField] = value,
                    };
                }
            }

            return datItemMap;
        }

        #endregion
    }
}
