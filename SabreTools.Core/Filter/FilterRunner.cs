using System;
using System.Collections.Generic;
using SabreTools.Core.Tools;
using SabreTools.Models.Metadata;

namespace SabreTools.Core.Filter
{
    /// <summary>
    /// Represents a set of filters that can be run against an object
    /// </summary>
    public class FilterRunner
    {
        /// <summary>
        /// Set of filters to be run against an object
        /// </summary>
        public readonly FilterObject[] Filters;

        /// <summary>
        /// Cached item type names for filter selection
        /// </summary>
        private readonly string[] _datItemTypeNames = TypeHelper.GetDatItemTypeNames();

        /// <summary>
        /// Set of machine type keys that are logically grouped
        /// </summary>
        /// TODO: REMOVE THISWHEN A PROPER IMPLEMENTATION IS FOUND
        private readonly string[] _machineTypeKeys =
        [
            $"{MetadataFile.MachineKey}.{Machine.IsBiosKey}",
            $"{MetadataFile.MachineKey}.{Machine.IsDeviceKey}",
            $"{MetadataFile.MachineKey}.{Machine.IsMechanicalKey}",
        ];

        public FilterRunner(FilterObject[] filters)
        {
            Filters = filters;
        }

        public FilterRunner(string[] filterStrings)
        {
            List<FilterObject> filters = [];
            foreach (string filterString in filterStrings)
            {
                try
                {
                    var filter = new FilterObject(filterString);
                    filters.Add(filter);
                }
                catch { }
            }

            Filters = [.. filters];
        }

        /// <summary>
        /// Run filtering on a DictionaryBase item
        /// </summary>
        public bool Run(DictionaryBase dictionaryBase)
        {
            string? itemName = dictionaryBase switch
            {
                Header => MetadataFile.HeaderKey,
                Machine => MetadataFile.MachineKey,
                DatItem => TypeHelper.GetXmlRootAttributeElementName(dictionaryBase.GetType()),
                _ => null,
            };

            // Group filters by key
            var filterDictionary = new Dictionary<string, List<FilterObject>>();
            foreach (var filter in Filters)
            {
                // Skip filters not applicable to the item
                if (filter.Key.ItemName == "item" && Array.IndexOf(_datItemTypeNames, itemName) == -1)
                    continue;
                else if (filter.Key.ItemName != "item" && filter.Key.ItemName != itemName)
                    continue;

                // Ensure the key exists
                string key = filter.Key.ToString();
                if (!filterDictionary.ContainsKey(filter.Key.ToString()))
                    filterDictionary[key] = [];

                // Add the filter to the set
                filterDictionary[key].Add(filter);
            }

            // TODO: REMOVE THIS ENTIRE BLOCK WHEN A PROPER IMPLEMENTATION IS FOUND
            // Handle special keys that work in tandem
            if (itemName == MetadataFile.MachineKey)
            {
                // Check that one of the special keys exists
                bool containsKey = false;
                foreach (string key in _machineTypeKeys)
                {
                    if (filterDictionary.ContainsKey(key))
                        containsKey = true;
                }

                // If at least one exists
                if (containsKey)
                {
                    bool matchAny = false;
                    foreach (string filterKey in _machineTypeKeys)
                    {
                        // Skip missing keys
                        if (!filterDictionary.ContainsKey(filterKey))
                            continue;

                        foreach (var filter in filterDictionary[filterKey])
                        {
                            matchAny |= filter.Matches(dictionaryBase);
                        }
                    }

                    // If we don't get a match, it's a failure
                    if (!matchAny)
                        return false;

                    // Remove the keys from the dictionary
                    foreach (string key in _machineTypeKeys)
                    {
                        filterDictionary.Remove(key);
                    }
                }
            }

            // Loop through and run each filter in order
            foreach (var filterKey in filterDictionary.Keys)
            {
                bool matchOne = false;
                foreach (var filter in filterDictionary[filterKey])
                {
                    matchOne |= filter.Matches(dictionaryBase);
                }

                // If we don't get a match, it's a failure
                if (!matchOne)
                    return false;
            }

            return true;
        }
    }
}