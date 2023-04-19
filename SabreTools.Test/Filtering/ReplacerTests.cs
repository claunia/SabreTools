using System.Collections.Generic;

using SabreTools.Core;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.Filtering;
using Xunit;

namespace SabreTools.Test.Filtering
{
    public class ReplacerTests
    {
        [Fact]
        public void ReplaceFieldsDatItemTest()
        {
            var datItem = CreateDatItem();
            var repDatItem = CreateDatItem();
            repDatItem.SetName("bar");
            var fields = new List<DatItemField> { DatItemField.Name };
            Replacer.ReplaceFields(datItem, repDatItem, fields);
            Assert.Equal("bar", datItem.GetName());
        }

        [Fact]
        public void ReplaceFieldsMachineTest()
        {
            var datItem = CreateDatItem();
            var repDatItem = CreateDatItem();
            repDatItem.Machine.Name = "foo";
            var fields = new List<MachineField> { MachineField.Name };
            Replacer.ReplaceFields(datItem.Machine, repDatItem.Machine, fields, false);
            Assert.Equal("foo", datItem.Machine.Name);
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