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
            _ = datFile.AddItemDB(biosItem, biosMachineIndex, sourceIndex, statsOnly: false);
            _ = datFile.AddItemDB(deviceItem, deviceMachineIndex, sourceIndex, statsOnly: false);

            datFile.BucketBy(ItemKey.Machine, DedupeType.None);
            datFile.RemoveBiosAndDeviceSets();

            Assert.Empty(datFile.GetMachinesDB());
        }

        #endregion

        #region RemoveItemsFromCloneOfChild

        [Fact]
        public void RemoveItemsFromCloneOfChild_Items()
        {
            Source source = new Source(0, source: null);

            Machine parentMachine = new Machine();
            parentMachine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "parent");
            parentMachine.SetFieldValue<string?>(Models.Metadata.Machine.RomOfKey, "romof");

            Machine childMachine = new Machine();
            childMachine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "child");
            childMachine.SetFieldValue<string?>(Models.Metadata.Machine.CloneOfKey, "parent");

            DatItem parentItem = new Rom();
            parentItem.SetName("parent_rom");
            parentItem.SetFieldValue<long>(Models.Metadata.Rom.SizeKey, 12345);
            parentItem.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "deadbeef");
            parentItem.SetFieldValue<Machine>(DatItem.MachineKey, parentMachine);
            parentItem.SetFieldValue<Source>(DatItem.SourceKey, source);

            DatItem matchChildItem = new Rom();
            matchChildItem.SetName("match_child_rom");
            matchChildItem.SetFieldValue<long>(Models.Metadata.Rom.SizeKey, 12345);
            matchChildItem.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "deadbeef");
            matchChildItem.SetFieldValue<Machine>(DatItem.MachineKey, childMachine);
            matchChildItem.SetFieldValue<Source>(DatItem.SourceKey, source);

            DatItem noMatchChildItem = new Rom();
            noMatchChildItem.SetName("no_match_child_rom");
            noMatchChildItem.SetFieldValue<long>(Models.Metadata.Rom.SizeKey, 12345);
            noMatchChildItem.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "beefdead");
            noMatchChildItem.SetFieldValue<Machine>(DatItem.MachineKey, childMachine);
            noMatchChildItem.SetFieldValue<Source>(DatItem.SourceKey, source);

            DatFile datFile = new Logiqx(datFile: null, deprecated: false);
            datFile.AddItem(parentItem, statsOnly: false);
            datFile.AddItem(matchChildItem, statsOnly: false);
            datFile.AddItem(noMatchChildItem, statsOnly: false);

            datFile.BucketBy(ItemKey.Machine, DedupeType.None);
            datFile.RemoveItemsFromCloneOfChild();

            Assert.Single(datFile.GetItemsForBucket("parent"));
            DatItem actual = Assert.Single(datFile.GetItemsForBucket("child"));
            Machine? actualMachine = actual.GetFieldValue<Machine>(DatItem.MachineKey);
            Assert.NotNull(actualMachine);
            Assert.Equal("child", actualMachine.GetStringFieldValue(Models.Metadata.Machine.NameKey));
            Assert.Equal("romof", actualMachine.GetStringFieldValue(Models.Metadata.Machine.RomOfKey));
        }

        [Fact]
        public void RemoveItemsFromCloneOfChild_ItemsDB()
        {
            Source source = new Source(0, source: null);

            Machine parentMachine = new Machine();
            parentMachine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "parent");
            parentMachine.SetFieldValue<string?>(Models.Metadata.Machine.RomOfKey, "romof");

            Machine childMachine = new Machine();
            childMachine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "child");
            childMachine.SetFieldValue<string?>(Models.Metadata.Machine.CloneOfKey, "parent");

            DatItem parentItem = new Rom();
            parentItem.SetName("parent_rom");
            parentItem.SetFieldValue<long>(Models.Metadata.Rom.SizeKey, 12345);
            parentItem.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "deadbeef");
            parentItem.SetFieldValue<Machine>(DatItem.MachineKey, parentMachine);
            parentItem.SetFieldValue<Source>(DatItem.SourceKey, source);

            DatItem matchChildItem = new Rom();
            matchChildItem.SetName("match_child_rom");
            matchChildItem.SetFieldValue<long>(Models.Metadata.Rom.SizeKey, 12345);
            matchChildItem.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "deadbeef");
            matchChildItem.SetFieldValue<Machine>(DatItem.MachineKey, childMachine);
            matchChildItem.SetFieldValue<Source>(DatItem.SourceKey, source);

            DatItem noMatchChildItem = new Rom();
            noMatchChildItem.SetName("no_match_child_rom");
            noMatchChildItem.SetFieldValue<long>(Models.Metadata.Rom.SizeKey, 12345);
            noMatchChildItem.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "beefdead");
            noMatchChildItem.SetFieldValue<Machine>(DatItem.MachineKey, childMachine);
            noMatchChildItem.SetFieldValue<Source>(DatItem.SourceKey, source);

            DatFile datFile = new Logiqx(datFile: null, deprecated: false);
            long biosMachineIndex = datFile.AddMachineDB(parentMachine);
            long deviceMachineIndex = datFile.AddMachineDB(childMachine);
            long sourceIndex = datFile.AddSourceDB(source);
            _ = datFile.AddItemDB(parentItem, biosMachineIndex, sourceIndex, statsOnly: false);
            _ = datFile.AddItemDB(matchChildItem, deviceMachineIndex, sourceIndex, statsOnly: false);
            _ = datFile.AddItemDB(noMatchChildItem, deviceMachineIndex, sourceIndex, statsOnly: false);

            datFile.BucketBy(ItemKey.Machine, DedupeType.None);
            datFile.RemoveItemsFromCloneOfChild();

            Assert.Single(datFile.GetItemsForBucketDB("parent"));
            long actual = Assert.Single(datFile.GetItemsForBucketDB("child")).Key;
            Machine? actualMachine = datFile.ItemsDB.GetMachineForItem(actual).Value;
            Assert.NotNull(actualMachine);
            Assert.Equal("child", actualMachine.GetStringFieldValue(Models.Metadata.Machine.NameKey));
            Assert.Equal("romof", actualMachine.GetStringFieldValue(Models.Metadata.Machine.RomOfKey));
        }

        #endregion

        #region RemoveItemsFromRomOfChild

        [Fact]
        public void RemoveItemsFromRomOfChild_Items()
        {
            Source source = new Source(0, source: null);

            Machine parentMachine = new Machine();
            parentMachine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "parent");

            Machine childMachine = new Machine();
            childMachine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "child");
            childMachine.SetFieldValue<string?>(Models.Metadata.Machine.RomOfKey, "parent");
            childMachine.SetFieldValue<bool>(Models.Metadata.Machine.IsBiosKey, true);

            DatItem parentItem = new Rom();
            parentItem.SetName("parent_rom");
            parentItem.SetFieldValue<long>(Models.Metadata.Rom.SizeKey, 12345);
            parentItem.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "deadbeef");
            parentItem.SetFieldValue<Machine>(DatItem.MachineKey, parentMachine);
            parentItem.SetFieldValue<Source>(DatItem.SourceKey, source);

            DatItem matchChildItem = new Rom();
            matchChildItem.SetName("match_child_rom");
            matchChildItem.SetFieldValue<long>(Models.Metadata.Rom.SizeKey, 12345);
            matchChildItem.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "deadbeef");
            matchChildItem.SetFieldValue<Machine>(DatItem.MachineKey, childMachine);
            matchChildItem.SetFieldValue<Source>(DatItem.SourceKey, source);

            DatItem noMatchChildItem = new Rom();
            noMatchChildItem.SetName("no_match_child_rom");
            noMatchChildItem.SetFieldValue<long>(Models.Metadata.Rom.SizeKey, 12345);
            noMatchChildItem.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "beefdead");
            noMatchChildItem.SetFieldValue<Machine>(DatItem.MachineKey, childMachine);
            noMatchChildItem.SetFieldValue<Source>(DatItem.SourceKey, source);

            DatFile datFile = new Logiqx(datFile: null, deprecated: false);
            datFile.AddItem(parentItem, statsOnly: false);
            datFile.AddItem(matchChildItem, statsOnly: false);
            datFile.AddItem(noMatchChildItem, statsOnly: false);

            datFile.BucketBy(ItemKey.Machine, DedupeType.None);
            datFile.RemoveItemsFromRomOfChild();

            Assert.Single(datFile.GetItemsForBucket("parent"));
            DatItem actual = Assert.Single(datFile.GetItemsForBucket("child"));
            Machine? actualMachine = actual.GetFieldValue<Machine>(DatItem.MachineKey);
            Assert.NotNull(actualMachine);
            Assert.Equal("child", actualMachine.GetStringFieldValue(Models.Metadata.Machine.NameKey));
        }

        [Fact]
        public void RemoveItemsFromRomOfChild_ItemsDB()
        {
            Source source = new Source(0, source: null);

            Machine parentMachine = new Machine();
            parentMachine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "parent");

            Machine childMachine = new Machine();
            childMachine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "child");
            childMachine.SetFieldValue<string?>(Models.Metadata.Machine.RomOfKey, "parent");
            childMachine.SetFieldValue<bool>(Models.Metadata.Machine.IsBiosKey, true);

            DatItem parentItem = new Rom();
            parentItem.SetName("parent_rom");
            parentItem.SetFieldValue<long>(Models.Metadata.Rom.SizeKey, 12345);
            parentItem.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "deadbeef");
            parentItem.SetFieldValue<Machine>(DatItem.MachineKey, parentMachine);
            parentItem.SetFieldValue<Source>(DatItem.SourceKey, source);

            DatItem matchChildItem = new Rom();
            matchChildItem.SetName("match_child_rom");
            matchChildItem.SetFieldValue<long>(Models.Metadata.Rom.SizeKey, 12345);
            matchChildItem.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "deadbeef");
            matchChildItem.SetFieldValue<Machine>(DatItem.MachineKey, childMachine);
            matchChildItem.SetFieldValue<Source>(DatItem.SourceKey, source);

            DatItem noMatchChildItem = new Rom();
            noMatchChildItem.SetName("no_match_child_rom");
            noMatchChildItem.SetFieldValue<long>(Models.Metadata.Rom.SizeKey, 12345);
            noMatchChildItem.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "beefdead");
            noMatchChildItem.SetFieldValue<Machine>(DatItem.MachineKey, childMachine);
            noMatchChildItem.SetFieldValue<Source>(DatItem.SourceKey, source);

            DatFile datFile = new Logiqx(datFile: null, deprecated: false);
            long biosMachineIndex = datFile.AddMachineDB(parentMachine);
            long deviceMachineIndex = datFile.AddMachineDB(childMachine);
            long sourceIndex = datFile.AddSourceDB(source);
            _ = datFile.AddItemDB(parentItem, biosMachineIndex, sourceIndex, statsOnly: false);
            _ = datFile.AddItemDB(matchChildItem, deviceMachineIndex, sourceIndex, statsOnly: false);
            _ = datFile.AddItemDB(noMatchChildItem, deviceMachineIndex, sourceIndex, statsOnly: false);

            datFile.BucketBy(ItemKey.Machine, DedupeType.None);
            datFile.RemoveItemsFromRomOfChild();

            Assert.Single(datFile.GetItemsForBucketDB("parent"));
            long actual = Assert.Single(datFile.GetItemsForBucketDB("child")).Key;
            Machine? actualMachine = datFile.ItemsDB.GetMachineForItem(actual).Value;
            Assert.NotNull(actualMachine);
            Assert.Equal("child", actualMachine.GetStringFieldValue(Models.Metadata.Machine.NameKey));
        }

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
            _ = datFile.AddItemDB(datItem, machineIndex, sourceIndex, statsOnly: false);

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