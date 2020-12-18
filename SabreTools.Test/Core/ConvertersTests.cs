using SabreTools.Core;
using SabreTools.Core.Tools;
using Xunit;

namespace SabreTools.Test.Core
{
    public class ConvertersTests
    {
        [Theory]
        [InlineData(null, DatHeaderField.NULL)]
        [InlineData("datname", DatHeaderField.NULL)]
        [InlineData("dat-datname", DatHeaderField.Name)]
        [InlineData("dat.datname", DatHeaderField.Name)]
        [InlineData("dat_datname", DatHeaderField.Name)]
        [InlineData("dat datname", DatHeaderField.Name)]
        [InlineData("datheader-datname", DatHeaderField.Name)]
        [InlineData("datheader.datname", DatHeaderField.Name)]
        [InlineData("datheader_datname", DatHeaderField.Name)]
        [InlineData("datheader datname", DatHeaderField.Name)]
        [InlineData("header-datname", DatHeaderField.Name)]
        [InlineData("header.datname", DatHeaderField.Name)]
        [InlineData("header_datname", DatHeaderField.Name)]
        [InlineData("header datname", DatHeaderField.Name)]
        [InlineData("DAT.DATNAME", DatHeaderField.Name)]
        [InlineData("dAt.DAtnamE", DatHeaderField.Name)]
        public void AsDatHeaderFieldTest(string field, DatHeaderField expected)
        {
            DatHeaderField actual = field.AsDatHeaderField();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, DatItemField.NULL)]
        [InlineData("name", DatItemField.NULL)]
        [InlineData("item-name", DatItemField.Name)]
        [InlineData("item.name", DatItemField.Name)]
        [InlineData("item_name", DatItemField.Name)]
        [InlineData("item name", DatItemField.Name)]
        [InlineData("datitem-name", DatItemField.Name)]
        [InlineData("datitem.name", DatItemField.Name)]
        [InlineData("datitem_name", DatItemField.Name)]
        [InlineData("datitem name", DatItemField.Name)]
        [InlineData("ITEM.NAME", DatItemField.Name)]
        [InlineData("iTeM.namE", DatItemField.Name)]
        public void AsDatItemFieldTest(string field, DatItemField expected)
        {
            DatItemField actual = field.AsDatItemField();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, MachineField.NULL)]
        [InlineData("name", MachineField.NULL)]
        [InlineData("game-name", MachineField.Name)]
        [InlineData("game.name", MachineField.Name)]
        [InlineData("game_name", MachineField.Name)]
        [InlineData("game name", MachineField.Name)]
        [InlineData("machine-name", MachineField.Name)]
        [InlineData("machine.name", MachineField.Name)]
        [InlineData("machine_name", MachineField.Name)]
        [InlineData("machine name", MachineField.Name)]
        [InlineData("GAME.NAME", MachineField.Name)]
        [InlineData("gAmE.namE", MachineField.Name)]
        public void AsMachineFieldTest(string field, MachineField expected)
        {
            MachineField actual = field.AsMachineField();
            Assert.Equal(expected, actual);
        }
    }
}