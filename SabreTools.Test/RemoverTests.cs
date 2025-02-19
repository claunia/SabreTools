using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.DatTools;
using Xunit;

namespace SabreTools.Test
{
    public class RemoverTests
    {
        [Fact]
        public void RemoveFields_DatItem()
        {
            var datItem = new Rom();
            datItem.SetName("foo");

            var remover = new Remover();
            remover.PopulateExclusions("DatItem.Name");
            remover.RemoveFields(datItem);

            Assert.Null(datItem.GetName());
        }

        [Fact]
        public void RemoveFields_Machine()
        {
            var machine = new Machine();
            machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "bar");
            machine.SetFieldValue<string?>(Models.Metadata.Machine.DescriptionKey, "bar");

            var remover = new Remover();
            remover.PopulateExclusions("Machine.Name");
            remover.RemoveFields(machine);

            Assert.Null(machine.GetStringFieldValue(Models.Metadata.Machine.NameKey));
        }
    }
}