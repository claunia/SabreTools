using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.Filter;
using Xunit;

namespace SabreTools.Test.Filter
{
    public class FilteringTests
    {
        [Fact]
        public void PassesFiltersDatItemFilterPass()
        {
            // Setup filter
            var filter = new FilterRunner(["rom.name:foo", "item.name:foo"]);

            // Setup DatItem
            var datItem = CreateDatItem();

            // Run filters
            bool actual = datItem.PassesFilter(filter);
            Assert.True(actual);
        }

        [Fact]
        public void PassesFiltersDatItemFilterFail()
        {
            // Setup filter
            var filter = new FilterRunner(["rom.name:bar", "item.name:bar"]);

            // Setup DatItem
            var datItem = CreateDatItem();

            // Run filters
            bool actual = datItem.PassesFilter(filter);
            Assert.False(actual);
        }

        [Fact]
        public void PassesFiltersMachineFilterPass()
        {
            // Setup filter
            var filter = new FilterRunner(["machine.name:bar"]);

            // Setup DatItem
            var datItem = CreateDatItem();

            // Run filters
            bool actual = datItem.PassesFilter(filter);
            Assert.True(actual);
        }

        [Fact]
        public void PassesFiltersMachineFilterFail()
        {
            // Setup filter
            var filter = new FilterRunner(["machine.name:foo"]);

            // Setup DatItem
            var datItem = CreateDatItem();

            // Run filters
            bool actual = datItem.PassesFilter(filter);
            Assert.False(actual);
        }

        /// <summary>
        /// Generate a consistent DatItem for testing
        /// </summary>
        private static DatItem CreateDatItem()
        {
            var rom = new Rom
            {
                Machine = new Machine
                {
                    Name = "bar",
                    Description = "bar",
                }
            };
            rom.SetName("foo");
            return rom;
        }
    }
}