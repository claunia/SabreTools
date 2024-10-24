using System;
using SabreTools.Core.Filter;
using Xunit;

namespace SabreTools.Test.Filter
{
    public class PopulationTests
    {
        [Fact]
        public void PopulateFilterRunnerNullListTest()
        {
            // Setup the list
            string[]? filters = null;

            // Setup the filter
            Assert.Throws<ArgumentNullException>(() => new FilterRunner(filters));
        }

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
            Assert.Equal("machine", filter.Filters[0].ItemName);
            Assert.Equal("name", filter.Filters[0].FieldName);
            Assert.Equal("foo", filter.Filters[0].Value);
            Assert.Equal(Operation.Equals, filter.Filters[0].Operation);

            Assert.Equal("machine", filter.Filters[1].ItemName);
            Assert.Equal("name", filter.Filters[1].FieldName);
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
            Assert.Equal("rom", filter.Filters[0].ItemName);
            Assert.Equal("name", filter.Filters[0].FieldName);
            Assert.Equal("foo", filter.Filters[0].Value);
            Assert.Equal(Operation.Equals, filter.Filters[0].Operation);

            Assert.Equal("item", filter.Filters[1].ItemName);
            Assert.Equal("name", filter.Filters[1].FieldName);
            Assert.Equal("bar", filter.Filters[1].Value);
            Assert.Equal(Operation.NotEquals, filter.Filters[1].Operation);
        }
    }
}