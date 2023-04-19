using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.Filtering;
using Xunit;

namespace SabreTools.Test.Filtering
{
    public class RemoverTests
    {
        [Fact]
        public void RemoveFieldsDatItemTest()
        {
            var datItem = CreateDatItem();
            var remover = new DatItemRemover();
            remover.SetRemover("DatItem.Name");
            remover.RemoveFields(datItem);
            Assert.Null(datItem.GetName());
        }

        [Fact]
        public void RemoveFieldsMachineTest()
        {
            var datItem = CreateDatItem();
            var remover = new DatItemRemover();
            remover.SetRemover("Machine.Name");
            remover.RemoveFields(datItem);
            Assert.Null(datItem.Machine.Name);
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