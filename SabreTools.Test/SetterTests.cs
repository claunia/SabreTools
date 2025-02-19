using SabreTools.Core.Filter;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.DatTools;
using Xunit;

namespace SabreTools.Test
{
    public class SetterTests
    {
        [Fact]
        public void SetFields_DatItem()
        {
            var datItem = new Rom();
            datItem.SetName("foo");

            var setter = new Setter();
            setter.PopulateSetters(new FilterKey("datitem", "name"), "bar");
            setter.SetFields(datItem);

            Assert.Equal("bar", datItem.GetName());
        }

        [Fact]
        public void SetFields_Machine()
        {
            var machine = new Machine();
            machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "bar");
            machine.SetFieldValue<string?>(Models.Metadata.Machine.DescriptionKey, "bar");

            var setter = new Setter();
            setter.PopulateSetters(new FilterKey("machine", "name"), "foo");
            setter.SetFields(machine);

            Assert.Equal("foo", machine.GetStringFieldValue(Models.Metadata.Machine.NameKey));
        }
    }
}