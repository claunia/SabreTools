using System.Collections.Generic;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using Xunit;

namespace SabreTools.DatFiles.Test
{
    // TODO: Add tests for 4 special items and one generic item
    public class ReplacerTests
    {
        [Fact]
        public void ReplaceFields_DatItem()
        {
            var datItem = CreateDatItem();
            var repDatItem = CreateDatItem();
            repDatItem.SetName("bar");
            var fields = new Dictionary<string, List<string>>
            {
                ["item"] = [Models.Metadata.Rom.NameKey]
            };

            Replacer.ReplaceFields(datItem, repDatItem, fields);

            Assert.Equal("bar", datItem.GetName());
        }

        [Fact]
        public void ReplaceFields_Machine()
        {
            var datItem = CreateDatItem();
            var machine = datItem.GetFieldValue<Machine>(DatItem.MachineKey);
            Assert.NotNull(machine);

            var repDatItem = CreateDatItem();
            var repMachine = repDatItem.GetFieldValue<Machine>(DatItem.MachineKey);
            Assert.NotNull(repMachine);
            
            repMachine!.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "foo");
            List<string> fields = [Models.Metadata.Machine.NameKey];

            Replacer.ReplaceFields(machine, repMachine, fields, false);

            Assert.Equal("foo", machine.GetStringFieldValue(Models.Metadata.Machine.NameKey));
        }

        /// <summary>
        /// Generate a consistent DatItem for testing
        /// </summary>
        private static DatItem CreateDatItem()
        {
            var machine = new Machine();
            machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "bar");
            machine.SetFieldValue<string?>(Models.Metadata.Machine.DescriptionKey, "bar");

            var rom = new Rom();
            rom.SetName("foo");
            rom.SetFieldValue<Machine>(DatItem.MachineKey, machine);

            return rom;
        }
    }
}