using System.Collections.Generic;

using SabreTools.Core;
using SabreTools.DatItems;
using Xunit;

namespace SabreTools.Test.DatItems
{
    public class DatItemToolTests
    {
        [Fact]
        public void RemoveFieldsDatItemTest()
        {
            var datItem = CreateDatItem();
            var fields = new List<DatItemField> { DatItemField.Name };
            DatItemTool.RemoveFields(datItem, datItemFields: fields);
            Assert.Null(datItem.GetName());
        }

        [Fact]
        public void RemoveFieldsMachineTest()
        {
            var datItem = CreateDatItem();
            var fields = new List<MachineField> { MachineField.Name };
            DatItemTool.RemoveFields(datItem, machineFields: fields);
            Assert.Null(datItem.Machine.Name);
        }

        [Fact]
        public void ReplaceFieldsDatItemTest()
        {
            var datItem = CreateDatItem();
            var repDatItem = CreateDatItem();
            repDatItem.SetName("bar");
            var fields = new List<DatItemField> { DatItemField.Name };
            DatItemTool.ReplaceFields(datItem, repDatItem, fields);
            Assert.Equal("bar", datItem.GetName());
        }

        [Fact]
        public void ReplaceFieldsMachineTest()
        {
            var datItem = CreateDatItem();
            var repDatItem = CreateDatItem();
            repDatItem.Machine.Name = "foo";
            var fields = new List<MachineField> { MachineField.Name };
            DatItemTool.ReplaceFields(datItem.Machine, repDatItem.Machine, fields, false);
            Assert.Equal("foo", datItem.Machine.Name);
        }

        [Fact]
        public void SetFieldsDatItemTest()
        {
            var datItem = CreateDatItem();
            var mappings = new Dictionary<DatItemField, string> { [DatItemField.Name] = "bar" };
            DatItemTool.SetFields(datItem, mappings, null);
            Assert.Equal("bar", datItem.GetName());
        }

        [Fact]
        public void SetFieldsMachineTest()
        {
            var datItem = CreateDatItem();
            var mappings = new Dictionary<MachineField, string> { [MachineField.Name] = "foo" };
            DatItemTool.SetFields(datItem, null, mappings);
            Assert.Equal("foo", datItem.Machine.Name);
        }

        [Fact]
        public void SetOneRomPerGameTest()
        {
            var datItem = CreateDatItem();
            datItem.SetName("foo.bin");
            DatItemTool.SetOneRomPerGame(datItem);
            Assert.Equal("foo.bin", datItem.GetName());
            Assert.Equal("bar/foo", datItem.Machine.Name);
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