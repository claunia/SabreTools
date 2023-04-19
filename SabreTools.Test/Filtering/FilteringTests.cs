using System.Collections.Generic;

using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.Filtering;
using Xunit;

namespace SabreTools.Test.Filtering
{
    public class FilteringTests
    {
        [Fact]
        public void PassesFiltersDatItemFilterPass()
        {
            // Setup filter
            var filter = new Filter();
            filter.PopulateFiltersFromList(new List<string> { "item.name:foo" });

            // Setup DatItem
            var datItem = CreateDatItem();

            // Run filters
            bool actual = filter.PassesAllFilters(datItem);
            Assert.True(actual);
        }

        [Fact]
        public void PassesFiltersDatItemFilterFail()
        {
            // Setup filter
            var filter = new Filter();
            filter.PopulateFiltersFromList(new List<string> { "item.name:bar" });

            // Setup DatItem
            var datItem = CreateDatItem();

            // Run filters
            bool actual = filter.PassesAllFilters(datItem);
            Assert.False(actual);
        }

        [Fact]
        public void PassesFiltersMachineFilterPass()
        {
            // Setup filter
            var filter = new Filter();
            filter.PopulateFiltersFromList(new List<string> { "machine.name:bar" });

            // Setup DatItem
            var datItem = CreateDatItem();

            // Run filters
            bool actual = filter.PassesAllFilters(datItem);
            Assert.True(actual);
        }

        [Fact]
        public void PassesFiltersMachineFilterFail()
        {
            // Setup filter
            var filter = new Filter();
            filter.PopulateFiltersFromList(new List<string> { "machine.name:foo" });

            // Setup DatItem
            var datItem = CreateDatItem();

            // Run filters
            bool actual = filter.PassesAllFilters(datItem);
            Assert.False(actual);
        }

        /// <summary>
        /// Generate a consistent DatItem for testing
        /// </summary>
        private static DatItem CreateDatItem()
        {
            return new Rom
            {
                Name = "foo",
                Machine = new Machine
                {
                    Name = "bar",
                    Description = "bar",
                }
            };
        }
    }
}