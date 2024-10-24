using System;
using SabreTools.Core.Filter;
using Xunit;

namespace SabreTools.Test.Filter
{
    public class PopulationTests
    {
        [Fact]
        public void PopulateFilterRunnerEmptyListTest()
        {
            // Setup the list
            string[]? filters = [];

            // Setup the filter runner
            var filterRunner = new FilterRunner(filters);

            // Check the filters
            Assert.NotNull(filterRunner.Filters);
        }

        [Fact]
        public void PopulateFilterMachineFieldTest()
        {
            // Setup the list
            string[] filters =
            [
                "machine.name:foo",
                "machine.name!:bar",
            ];

            // Setup the filter
            var filter = new FilterRunner(filters);

            // Check the filters
            Assert.Equal("machine.name", filter.Filters[0].Key.ToString());
            Assert.Equal("foo", filter.Filters[0].Value);
            Assert.Equal(Operation.Equals, filter.Filters[0].Operation);

            Assert.Equal("machine.name", filter.Filters[1].Key.ToString());
            Assert.Equal("bar", filter.Filters[1].Value);
            Assert.Equal(Operation.NotEquals, filter.Filters[1].Operation);
        }

        [Fact]
        public void PopulateFilterDatItemFieldTest()
        {
            // Setup the list
            string[] filters =
            [
                "rom.name:foo",
                "datitem.name!:bar"
            ];

            // Setup the filter
            var filter = new FilterRunner(filters);

            // Check the filters
            Assert.Equal("rom.name", filter.Filters[0].Key.ToString());
            Assert.Equal("foo", filter.Filters[0].Value);
            Assert.Equal(Operation.Equals, filter.Filters[0].Operation);

            Assert.Equal("item.name", filter.Filters[1].Key.ToString());
            Assert.Equal("bar", filter.Filters[1].Value);
            Assert.Equal(Operation.NotEquals, filter.Filters[1].Operation);
        }
    }
}