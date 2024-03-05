using System.Collections.Generic;
using SabreTools.Filtering;
using Xunit;

namespace SabreTools.Test.Filtering
{
    public class PopulationTests
    {
        [Fact]
        public void PopulateExclusionNullListTest()
        {
            // Setup the list
            List<string>? exclusions = null;

            // Setup the remover
            var remover = new Remover();
            remover.PopulateExclusionsFromList(exclusions);

            // Check the exclusion lists
            Assert.Empty(remover.HeaderFieldNames);
            Assert.Empty(remover.MachineFieldNames);
            Assert.Empty(remover.ItemFieldNames);
        }

        [Fact]
        public void PopulateExclusionEmptyListTest()
        {
            // Setup the list
            List<string> exclusions = [];

            // Setup the remover
            var remover = new Remover();
            remover.PopulateExclusionsFromList(exclusions);

            // Check the exclusion lists
            Assert.Empty(remover.HeaderFieldNames);
            Assert.Empty(remover.MachineFieldNames);
            Assert.Empty(remover.ItemFieldNames);
        }

        [Fact]
        public void PopulateExclusionHeaderFieldTest()
        {
            // Setup the list
            List<string> exclusions =
            [
                "header.datname",
            ];

            // Setup the remover
            var remover = new Remover();
            remover.PopulateExclusionsFromList(exclusions);

            // Check the exclusion lists
            Assert.Empty(remover.HeaderFieldNames);
            Assert.Empty(remover.MachineFieldNames);
            Assert.Empty(remover.ItemFieldNames);
        }

        [Fact]
        public void PopulateExclusionMachineFieldTest()
        {
            // Setup the list
            List<string> exclusions =
            [
                "machine.name",
            ];

            // Setup the remover
            var remover = new Remover();
            remover.PopulateExclusionsFromList(exclusions);

            // Check the exclusion lists
            Assert.Empty(remover.HeaderFieldNames);
            Assert.Single(remover.MachineFieldNames);
            Assert.Empty(remover.ItemFieldNames);
        }

        [Fact]
        public void PopulateExclusionDatItemFieldTest()
        {
            // Setup the list
            List<string> exclusions =
            [
                "item.name",
            ];

            // Setup the remover
            var remover = new Remover();
            remover.PopulateExclusionsFromList(exclusions);

            // Check the exclusion lists
            Assert.Empty(remover.HeaderFieldNames);
            Assert.Empty(remover.MachineFieldNames);
            Assert.Single(remover.ItemFieldNames);
        }
    
        [Fact]
        public void PopulateFilterNullListTest()
        {
            // Setup the list
            List<string>? filters = null;

            // Setup the filter
            var filter = new SabreTools.Filtering.Filter();
            filter.PopulateFiltersFromList(filters);

            // Check the filters
            Assert.NotNull(filter.MachineFilter);
            Assert.NotNull(filter.DatItemFilter);
        }

        [Fact]
        public void PopulateFilterEmptyListTest()
        {
            // Setup the list
            List<string> filters = [];

            // Setup the filter
            var filter = new SabreTools.Filtering.Filter();
            filter.PopulateFiltersFromList(filters);

            // Check the filters
            Assert.NotNull(filter.MachineFilter);
            Assert.NotNull(filter.DatItemFilter);
        }

        [Fact]
        public void PopulateFilterMachineFieldTest()
        {
            // Setup the list
            List<string> filters =
            [
                "machine.name:foo",
                "!machine.name:bar",
            ];

            // Setup the filter
            var filter = new SabreTools.Filtering.Filter();
            filter.PopulateFiltersFromList(filters);

            // Check the filters
            Assert.NotNull(filter.MachineFilter);
            Assert.NotNull(filter.DatItemFilter);
            Assert.Contains("foo", filter.MachineFilter.Name.PositiveSet);
            Assert.Contains("bar", filter.MachineFilter.Name.NegativeSet);
        }

        [Fact]
        public void PopulateFilterDatItemFieldTest()
        {
            // Setup the list
            List<string> filters =
            [
                "item.name:foo",
                "!item.name:bar"
            ];

            // Setup the filter
            var filter = new SabreTools.Filtering.Filter();
            filter.PopulateFiltersFromList(filters);

            // Check the filters
            Assert.NotNull(filter.MachineFilter);
            Assert.NotNull(filter.DatItemFilter);
            Assert.Contains("foo", filter.DatItemFilter.Name.PositiveSet);
            Assert.Contains("bar", filter.DatItemFilter.Name.NegativeSet);
        }
    }
}