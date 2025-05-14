using System.Collections.Generic;
using SabreTools.Models.Metadata;

namespace SabreTools.Core.Filter
{
    /// <summary>
    /// Represents a set of filters and groups
    /// </summary>
    public class FilterGroup
    {
        /// <summary>
        /// All standalone filters in the group
        /// </summary>
        public readonly List<FilterObject> Subfilters = [];

        /// <summary>
        /// All filter groups contained in the group
        /// </summary>
        public readonly List<FilterGroup> Subgroups = [];

        /// <summary>
        /// How to apply the group filters
        /// </summary>
        public readonly GroupType GroupType;

        public FilterGroup(GroupType groupType)
        {
            GroupType = groupType;
        }

        public FilterGroup(FilterObject[] filters, GroupType groupType)
        {
            Subfilters.AddRange(filters);
            GroupType = groupType;
        }

        public FilterGroup(FilterGroup[] groups, GroupType groupType)
        {
            Subgroups.AddRange(groups);
            GroupType = groupType;
        }

        public FilterGroup(FilterObject[] filters, FilterGroup[] groups, GroupType groupType)
        {
            Subfilters.AddRange(filters);
            Subgroups.AddRange(groups);
            GroupType = groupType;
        }

        #region Matching

        /// <summary>
        /// Determine if a DictionaryBase object matches the group
        /// </summary>
        public bool Matches(DictionaryBase dictionaryBase)
        {
            return GroupType switch
            {
                GroupType.AND => MatchesAnd(dictionaryBase),
                GroupType.OR => MatchesOr(dictionaryBase),
                _ => false,
            };
        }

        /// <summary>
        /// Determines if a value matches all filters
        /// </summary>
        private bool MatchesAnd(DictionaryBase dictionaryBase)
        {
            // Run standalone filters
            foreach (var filter in Subfilters)
            {
                // One failed match fails the group
                if (!filter.Matches(dictionaryBase))
                    return false;
            }

            // Run filter subgroups
            foreach (var group in Subgroups)
            {
                // One failed match fails the group
                if (!group.Matches(dictionaryBase))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Determines if a value matches any filters
        /// </summary>
        private bool MatchesOr(DictionaryBase dictionaryBase)
        {
            // Run standalone filters
            foreach (var filter in Subfilters)
            {
                // One successful match passes the group
                if (filter.Matches(dictionaryBase))
                    return true;
            }

            // Run filter subgroups
            foreach (var group in Subgroups)
            {
                // One successful match passes the group
                if (group.Matches(dictionaryBase))
                    return true;
            }

            return false;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Derive a group type from the input string, if possible
        /// </summary>
        private static GroupType GetGroupType(string? groupType)
        {
            return groupType?.ToLowerInvariant() switch
            {
                "&" => GroupType.AND,
                "&&" => GroupType.AND,

                "|" => GroupType.OR,
                "||" => GroupType.OR,

                _ => GroupType.NONE,
            };
        }

        #endregion
    }
}