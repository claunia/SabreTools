using SabreTools.DatFiles.Formats;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using Xunit;

namespace SabreTools.DatFiles.Test
{
    public partial class DatFileTests
    {
        #region AddItemsFromChildren

        // TODO: Implement AddItemsFromChildren tests

        #endregion

        #region AddItemsFromDevices

        // TODO: Implement AddItemsFromDevices tests

        #endregion

        #region AddItemsFromCloneOfParent

        // TODO: Implement AddItemsFromCloneOfParent tests

        #endregion

        #region AddItemsFromRomOfParent

        // TODO: Implement AddItemsFromRomOfParent tests

        #endregion

        #region RemoveBiosAndDeviceSets

        // TODO: Implement RemoveBiosAndDeviceSets tests

        #endregion

        #region RemoveItemsFromCloneOfChild

        // TODO: Implement RemoveItemsFromCloneOfChild tests

        #endregion

        #region RemoveItemsFromRomOfChild

        // TODO: Implement RemoveItemsFromRomOfChild tests

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