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

        #region AddItemsFromCloneOfParent

        // TODO: Implement AddItemsFromCloneOfParent tests

        #endregion

        #region AddItemsFromDevices

        // TODO: Implement AddItemsFromDevices tests

        #endregion

        #region AddItemsFromRomOfParent

        // TODO: Implement AddItemsFromRomOfParent tests

        #endregion

        #region RemoveBiosAndDeviceSets

        [Fact]
        public void RemoveBiosAndDeviceSets_Items()
        {
            Source source = new Source(0, source: null);

            Machine biosMachine = new Machine();
            biosMachine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "bios");
            biosMachine.SetFieldValue<bool>(Models.Metadata.Machine.IsBiosKey, true);

            Machine deviceMachine = new Machine();
            deviceMachine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "device");
            deviceMachine.SetFieldValue<bool>(Models.Metadata.Machine.IsDeviceKey, true);

            DatItem biosItem = new Rom();
            biosItem.SetFieldValue<Machine>(DatItem.MachineKey, biosMachine);
            biosItem.SetFieldValue<Source>(DatItem.SourceKey, source);

            DatItem deviceItem = new Rom();
            deviceItem.SetFieldValue<Machine>(DatItem.MachineKey, deviceMachine);
            deviceItem.SetFieldValue<Source>(DatItem.SourceKey, source);

            DatFile datFile = new Logiqx(datFile: null, deprecated: false);
            datFile.AddItem(biosItem, statsOnly: false);
            datFile.AddItem(deviceItem, statsOnly: false);

            datFile.BucketBy(ItemKey.Machine, DedupeType.None);
            datFile.RemoveBiosAndDeviceSets();

            Assert.Empty(datFile.GetItemsForBucket("bios"));
            Assert.Empty(datFile.GetItemsForBucket("device"));
        }

        [Fact]
        public void RemoveBiosAndDeviceSets_ItemsDB()
        {
            Source source = new Source(0, source: null);

            Machine biosMachine = new Machine();
            biosMachine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "bios");
            biosMachine.SetFieldValue<bool>(Models.Metadata.Machine.IsBiosKey, true);

            Machine deviceMachine = new Machine();
            deviceMachine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "device");
            deviceMachine.SetFieldValue<bool>(Models.Metadata.Machine.IsDeviceKey, true);

            DatItem biosItem = new Rom();
            DatItem deviceItem = new Rom();

            DatFile datFile = new Logiqx(datFile: null, deprecated: false);
            long biosMachineIndex = datFile.AddMachineDB(biosMachine);
            long deviceMachineIndex = datFile.AddMachineDB(deviceMachine);
            long sourceIndex = datFile.AddSourceDB(source);
            long biosItemId = datFile.AddItemDB(biosItem, biosMachineIndex, sourceIndex, statsOnly: false);
            long deviceItemId = datFile.AddItemDB(deviceItem, deviceMachineIndex, sourceIndex, statsOnly: false);

            datFile.BucketBy(ItemKey.Machine, DedupeType.None);
            datFile.RemoveBiosAndDeviceSets();

            Assert.Empty(datFile.GetMachinesDB());
        }

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

            datFile.BucketBy(ItemKey.Machine, DedupeType.None);
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

            datFile.BucketBy(ItemKey.Machine, DedupeType.None);
            datFile.RemoveMachineRelationshipTags();

            Machine actual = Assert.Single(datFile.GetMachinesDB()).Value;
            Assert.Null(actual.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey));
            Assert.Null(actual.GetStringFieldValue(Models.Metadata.Machine.RomOfKey));
            Assert.Null(actual.GetStringFieldValue(Models.Metadata.Machine.SampleOfKey));
        }

        #endregion
    }
}