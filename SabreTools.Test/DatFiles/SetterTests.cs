using System.Collections.Generic;

using SabreTools.Core;
using SabreTools.DatFiles;
using SabreTools.DatItems;
using Xunit;

namespace SabreTools.Test.DatFiles
{
    public class SetterTests
    {
        [Fact]
        public void SetFieldsDatItemTest()
        {
            var datItem = CreateDatItem();
            Setter setter = new Setter
            {
                DatItemMappings = new Dictionary<DatItemField, string> { [DatItemField.Name] = "bar" }
            };
            setter.SetFields(datItem);
            Assert.Equal("bar", datItem.GetName());
        }

        [Fact]
        public void SetFieldsMachineTest()
        {
            var datItem = CreateDatItem();
            Setter setter = new Setter
            {
                MachineMappings = new Dictionary<MachineField, string> { [MachineField.Name] = "foo" }
            };
            setter.SetFields(datItem.Machine);
            Assert.Equal("foo", datItem.Machine.Name);
        }

        /// <summary>
        /// Generate a consistent DatItem for testing
        /// </summary>
        private DatItem CreateDatItem()
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