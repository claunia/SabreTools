using SabreTools.DatFiles;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using Xunit;

namespace SabreTools.Test.DatFiles
{
    public class SetterTests
    {
        [Fact]
        public void SetFieldsDatItemTest()
        {
            var datItem = CreateDatItem();
            var setter = new Setter();
            setter.PopulateSetters("datitem.name", "bar");
            setter.SetFields(datItem);
            Assert.Equal("bar", datItem.GetName());
        }

        [Fact]
        public void SetFieldsMachineTest()
        {
            var datItem = CreateDatItem();
            var setter = new Setter();
            setter.PopulateSetters("machine.name", "foo");
            setter.SetFields(datItem.Machine);
            Assert.Equal("foo", datItem.Machine.GetFieldValue<string?>(Models.Metadata.Machine.NameKey));
        }

        /// <summary>
        /// Generate a consistent DatItem for testing
        /// </summary>
        private static DatItem CreateDatItem()
        {
            var machine = new Machine();
            machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "bar");
            machine.SetFieldValue<string?>(Models.Metadata.Machine.DescriptionKey, "bar");

            var rom = new Rom { Machine = machine };
            rom.SetName("foo");

            return rom;
        }
    }
}