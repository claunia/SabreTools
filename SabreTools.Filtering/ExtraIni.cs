using System;
using System.Collections.Generic;
using System.Linq;
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
        public List<ExtraIniItem> Items { get; set; } = [];

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
            // If there are no inputs, just skip
            if (inputs == null || !inputs.Any())
                return;

            InternalStopwatch watch = new("Populating extras from list");

            foreach (string input in inputs)
            {
                ExtraIniItem item = new();

                // If we don't even have a possible field and file combination
                if (!input.Contains(':'))
                {
                    logger.Warning($"'{input}` is not a valid INI extras string. Valid INI extras strings are of the form 'key:value'. Please refer to README.1ST or the help feature for more details.");
                    return;
                }

                string inputTrimmed = input.Trim('"', ' ', '\t');
                string fieldString = inputTrimmed.Split(':')[0].ToLowerInvariant().Trim('"', ' ', '\t');
                string fileString = inputTrimmed.Substring(fieldString.Length + 1).Trim('"', ' ', '\t');

                item.FieldName = SabreTools.Filter.FilterParser.ParseFilterId(fieldString);
                if (item.PopulateFromFile(fileString))
                    Items.Add(item);
            }

            watch.Stop();
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
            // If we have no extras, don't attempt to apply and just return true
            if (Items == null || !Items.Any())
                return true;

            var watch = new InternalStopwatch("Applying extra mappings to DAT");

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
                    var datItems = datFile.Items[machine];
                    if (datItems == null)
                        continue;

                    // Try to get the map values, if possible
                    combinedMachineMaps.TryGetValue(machine, out var machineMappings);
                    combinedDatItemMaps.TryGetValue(machine, out var datItemMappings);

                    // Create a setter with the new mappings
                    var setter = new Setter();
                    setter.PopulateSettersFromList()
                    {
                        MachineFieldMappings = machineMappings,
                        ItemFieldMappings = datItemMappings,
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
            finally
            {
                watch.Stop();
            }

            return true;
        }

        /// <summary>
        /// Combine ExtraIni fields
        /// </summary>
        /// <returns>Mapping dictionary from machine name to field mapping</returns>
        private (List<string> Keys, List<string> Values) CombineExtras()
        {
            var keys = new List<string>();
            var values = new List<string>();

            // Loop through each of the extras
            foreach (ExtraIniItem item in Items)
            {
                

                foreach (var mapping in item.Mappings)
                {
                    string machineName = mapping.Key;
                    string value = mapping.Value;
                        
                    mapping[machineName] = new Dictionary<string, string>
                    {
                        [item.FieldName!] = value,
                    };
                }
            }

            return mapping;
        }

        /// <summary>
        /// Combine MachineField-based ExtraIni fields
        /// </summary>
        /// <returns>Mapping dictionary from machine name to field mapping</returns>
        private Dictionary<string, Dictionary<string, string>> CombineMachineExtras()
        {
            var machineMap = new Dictionary<string, Dictionary<string, string>>();

            // Loop through each of the extras
            foreach (ExtraIniItem item in Items.Where(i => i.MachineField != null))
            {
                foreach (var mapping in item.Mappings)
                {
                    string machineName = mapping.Key;
                    string value = mapping.Value;
                        
                    machineMap[machineName] = new Dictionary<string, string>
                    {
                        [item.MachineField!] = value,
                    };
                }
            }

            return machineMap;
        }

        /// <summary>
        /// Combine DatItemField-based ExtraIni fields
        /// </summary>
        /// <returns>Mapping dictionary from machine name to field mapping</returns>
        private Dictionary<string, Dictionary<string, string>> CombineDatItemExtras()
        {
            var datItemMap = new Dictionary<string, Dictionary<string, string>>();

            // Loop through each of the extras
            foreach (ExtraIniItem item in Items.Where(i => i.ItemField != null))
            {
                foreach (var mapping in item.Mappings)
                {
                    string machineName = mapping.Key;
                    string value = mapping.Value;
                        
                    datItemMap[machineName] = new Dictionary<string, string>()
                    {
                        [item.ItemField!] = value,
                    };
                }
            }

            return datItemMap;
        }

        #endregion
    }
}
