using SabreTools.DatFiles.Formats;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using Xunit;

namespace SabreTools.DatFiles.Test
{
    public partial class DatFileTests
    {
        #region MachineDescriptionToName

        // TODO: Implement MachineDescriptionToName tests

        #endregion

        #region SetOneRomPerGame

        // TODO: Implement SetOneRomPerGame tests

        #endregion

        #region SetOneGamePerRegion

        // TODO: Implement SetOneGamePerRegion tests

        #endregion

        #region StripSceneDatesFromItems

        [Fact]
        public void StripSceneDatesFromItems_Items()
        {
            Source source = new Source(0, source: null);

            Machine machine = new Machine();
            machine.SetFieldValue(Models.Metadata.Machine.NameKey, "10.10.10-machine-name");

            DatItem datItem = new Rom();
            datItem.SetFieldValue(DatItem.MachineKey, machine);
            datItem.SetFieldValue(DatItem.SourceKey, source);

            DatFile datFile = new Logiqx(datFile: null, deprecated: false);
            datFile.AddItem(datItem, statsOnly: false);

            datFile.StripSceneDatesFromItems();

            // The name of the bucket is not expected to change
            DatItem actual = Assert.Single(datFile.GetItemsForBucket("10.10.10-machine-name"));
            Machine? actualMachine = actual.GetFieldValue<Machine?>(DatItem.MachineKey);
            Assert.NotNull(actualMachine);
            Assert.Equal("machine-name", actualMachine.GetStringFieldValue(Models.Metadata.Machine.NameKey));
        }

        [Fact]
        public void StripSceneDatesFromItems_ItemsDB()
        {
            Source source = new Source(0, source: null);

            Machine machine = new Machine();
            machine.SetFieldValue(Models.Metadata.Machine.NameKey, "10.10.10-machine-name");

            DatItem datItem = new Rom();

            DatFile datFile = new Logiqx(datFile: null, deprecated: false);
            long sourceIndex = datFile.AddSourceDB(source);
            long machineIndex = datFile.AddMachineDB(machine);
            _ = datFile.AddItemDB(datItem, machineIndex, sourceIndex, statsOnly: false);

            datFile.StripSceneDatesFromItems();

            Machine actualMachine = Assert.Single(datFile.GetMachinesDB()).Value;
            Assert.Equal("machine-name", actualMachine.GetStringFieldValue(Models.Metadata.Machine.NameKey));
        }

        #endregion
    }
}