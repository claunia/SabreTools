using SabreTools.DatFiles.Formats;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using Xunit;

namespace SabreTools.DatFiles.Test
{
    public partial class DatFileTests
    {
        #region AddRomsFromBios

        // TODO: Implement AddRomsFromBios tests

        #endregion

        #region AddRomsFromChildren

        // TODO: Implement AddRomsFromChildren tests

        #endregion

        #region AddRomsFromDevices

        // TODO: Implement AddRomsFromDevices tests

        #endregion

        #region AddRomsFromParent

        // TODO: Implement AddRomsFromParent tests

        #endregion

        #region RemoveBiosAndDeviceSets

        // TODO: Implement RemoveBiosAndDeviceSets tests

        #endregion

        #region RemoveBiosRomsFromChild

        // TODO: Implement RemoveBiosRomsFromChild tests

        #endregion

        #region RemoveRomsFromChild

        // TODO: Implement RemoveRomsFromChild tests

        #endregion

        #region RemoveMachineRelationshipTags

        [Fact]
        public void RemoveMachineRelationshipTags_Items()
        {
            Source source = new Source(0, source: null);

            Machine machine = new Machine();
            machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "machine");
            machine.SetFieldValue<string?>(Models.Metadata.Machine.CloneOfKey, "XXXXXX");
            machine.SetFieldValue<string?>(Models.Metadata.Machine.RomOfKey, "XXXXXX");
            machine.SetFieldValue<string?>(Models.Metadata.Machine.SampleOfKey, "XXXXXX");

            DatItem datItem = new Rom();
            datItem.SetFieldValue<Machine>(DatItem.MachineKey, machine);
            datItem.SetFieldValue<Source>(DatItem.SourceKey, source);

            DatFile datFile = new Logiqx(datFile: null, deprecated: false);
            datFile.AddItem(datItem, statsOnly: false);

            datFile.RemoveMachineRelationshipTags();

            DatItem actualItem = Assert.Single(datFile.GetItemsForBucket("machine"));
            Machine? actual = actualItem.GetFieldValue<Machine>(DatItem.MachineKey);
            Assert.NotNull(actual);
            Assert.Null(actual.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey));
            Assert.Null(actual.GetStringFieldValue(Models.Metadata.Machine.RomOfKey));
            Assert.Null(actual.GetStringFieldValue(Models.Metadata.Machine.SampleOfKey));
        }

        [Fact]
        public void RemoveMachineRelationshipTags_ItemsDB()
        {
            Source source = new Source(0, source: null);

            Machine machine = new Machine();
            machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "machine");
            machine.SetFieldValue<string?>(Models.Metadata.Machine.CloneOfKey, "XXXXXX");
            machine.SetFieldValue<string?>(Models.Metadata.Machine.RomOfKey, "XXXXXX");
            machine.SetFieldValue<string?>(Models.Metadata.Machine.SampleOfKey, "XXXXXX");

            DatItem datItem = new Rom();

            DatFile datFile = new Logiqx(datFile: null, deprecated: false);
            long machineIndex = datFile.AddMachineDB(machine);
            long sourceIndex = datFile.AddSourceDB(source);
            long itemId = datFile.AddItemDB(datItem, machineIndex, sourceIndex, statsOnly: false);

            datFile.RemoveMachineRelationshipTags();

            Machine actual = Assert.Single(datFile.GetMachinesDB()).Value;
            Assert.Null(actual.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey));
            Assert.Null(actual.GetStringFieldValue(Models.Metadata.Machine.RomOfKey));
            Assert.Null(actual.GetStringFieldValue(Models.Metadata.Machine.SampleOfKey));
        }

        #endregion
    }
}