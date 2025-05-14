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
        public readonly Dictionary<string, List<FilterObject>> Filters = [];

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
            foreach (var filter in filters)
            {
                // Ensure the key exists
                string key = filter.Key.ToString();
                if (!Filters.ContainsKey(filter.Key.ToString()))
                    Filters[key] = [];

                // Add the filter to the set
                Filters[key].Add(filter);
            }
        }

        public FilterRunner(string[] filterStrings)
        {
            foreach (string filterString in filterStrings)
            {
                try
                {
                    var filter = new FilterObject(filterString);

                    // Ensure the key exists
                    string key = filter.Key.ToString();
                    if (!Filters.ContainsKey(filter.Key.ToString()))
                        Filters[key] = [];

                    // Add the filter to the set
                    Filters[key].Add(filter);
                }
                catch { }
            }
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

            // Null is invalid
            if (itemName == null)
                return false;

            // TODO: REMOVE THIS ENTIRE BLOCK WHEN A PROPER IMPLEMENTATION IS FOUND
            // Handle special keys that work in tandem
            if (itemName == MetadataFile.MachineKey)
            {
                // Check that one of the special keys exists
                bool containsKey = false;
                foreach (string key in _machineTypeKeys)
                {
                    if (Filters.ContainsKey(key))
                        containsKey = true;
                }

                // If at least one exists
                if (containsKey)
                {
                    bool matchAny = false;
                    foreach (string filterKey in _machineTypeKeys)
                    {
                        // Skip missing keys
                        if (!Filters.ContainsKey(filterKey))
                            continue;

                        foreach (var filter in Filters[filterKey])
                        {
                            matchAny |= filter.Matches(dictionaryBase);
                        }
                    }

                    // If we don't get a match, it's a failure
                    if (!matchAny)
                        return false;
                }
            }

            // Loop through and run each filter in order
            foreach (var filterKey in Filters.Keys)
            {
                // Skip filters not applicable to the item
                if (filterKey.StartsWith("item.") && Array.IndexOf(_datItemTypeNames, itemName) == -1)
                    continue;
                else if (!filterKey.StartsWith("item.") && !filterKey.StartsWith(itemName))
                    continue;

                // TODO: REMOVE THIS ENTIRE BLOCK WHEN A PROPER IMPLEMENTATION IS FOUND
                if (Array.Exists(_machineTypeKeys, key => key == filterKey))
                    continue;

                bool matchOne = false;
                foreach (var filter in Filters[filterKey])
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