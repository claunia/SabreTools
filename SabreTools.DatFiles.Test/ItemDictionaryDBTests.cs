using System.Collections.Generic;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using Xunit;

namespace SabreTools.DatFiles.Test
{
    public class ItemDictionaryDBTests
    {
        #region AddItem

        // TODO: Add AddItem tests
        // - Disk, with/without hashes
        // - File, with/without hashes
        // - Media, with/without hashes
        // - Rom, with/without hashes, with/without size
        // - Stats only/actual add

        #endregion

        #region AddMachine

        [Fact]
        public void AddMachineTest()
        {
            Machine machine = new Machine();
            var dict = new ItemDictionaryDB();
            long machineIndex = dict.AddMachine(machine);

            Assert.Equal(0, machineIndex);
            Assert.Single(dict.GetMachines());
        }

        #endregion

        #region AddSource

        [Fact]
        public void AddSourceTest()
        {
            Source source = new Source(0, source: null);
            var dict = new ItemDictionaryDB();
            long sourceIndex = dict.AddSource(source);

            Assert.Equal(0, sourceIndex);
            Assert.Single(dict.GetSources());
        }

        #endregion

        #region ClearMarked

        [Fact]
        public void ClearMarkedTest()
        {
            // Setup the items
            Source source = new Source(0, source: null);

            Machine machine = new Machine();
            machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "game-1");

            DatItem rom1 = new Rom();
            rom1.SetName("rom-1");
            rom1.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            rom1.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "0000000fbbb37f8488100b1b4697012de631a5e6");
            rom1.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, "1024");

            DatItem rom2 = new Rom();
            rom2.SetName("rom-2");
            rom2.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            rom2.SetFieldValue<bool?>(DatItem.RemoveKey, true);
            rom2.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "000000e948edcb4f7704b8af85a77a3339ecce44");
            rom2.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, "1024");

            // Setup the dictionary
            var dict = new ItemDictionaryDB();
            long sourceIndex = dict.AddSource(source);
            long machineIndex = dict.AddMachine(machine);
            dict.AddItem(rom1, machineIndex, sourceIndex, statsOnly: false);
            dict.AddItem(rom2, machineIndex, sourceIndex, statsOnly: false);

            dict.ClearMarked();
            string key = Assert.Single(dict.SortedKeys);
            Assert.Equal("game-1", key);
            Dictionary<long, DatItem> items = dict.GetItemsForBucket(key);
            Assert.Single(items);
        }

        #endregion

        #region GetItemsForBucket

        // TODO: Add GetItemsForBucket tests
        // - Null/empty bucket name
        // - Invalid bucket
        // - Removed items with filter
        // - Removed items without filter
        // - Normal items

        #endregion

        #region GetMachine

        [Fact]
        public void GetMachineTest()
        {
            Machine machine = new Machine();
            var dict = new ItemDictionaryDB();
            long machineIndex = dict.AddMachine(machine);

            Assert.Equal(0, machineIndex);
            var actual = dict.GetMachine(machineIndex);
            Assert.NotNull(actual);
        }

        #endregion

        #region GetMachineForItem

        [Fact]
        public void GetMachineForItemTest()
        {
            Source source = new Source(0, source: null);
            Machine machine = new Machine();
            DatItem item = new Rom();

            var dict = new ItemDictionaryDB();
            long machineIndex = dict.AddMachine(machine);
            long sourceIndex = dict.AddSource(source);
            long itemIndex = dict.AddItem(item, machineIndex, sourceIndex, statsOnly: false);

            var actual = dict.GetMachineForItem(itemIndex);
            Assert.Equal(0, actual.Key);
            Assert.NotNull(actual.Value);
        }

        #endregion

        #region GetSource

        [Fact]
        public void GetSourceTest()
        {
            Source source = new Source(0, source: null);
            var dict = new ItemDictionaryDB();
            long sourceIndex = dict.AddSource(source);

            Assert.Equal(0, sourceIndex);
            var actual = dict.GetSource(sourceIndex);
            Assert.NotNull(actual);
        }

        #endregion

        #region GetSourceForItem

        [Fact]
        public void GetSourceForItemTest()
        {
            Source source = new Source(0, source: null);
            Machine machine = new Machine();
            DatItem item = new Rom();

            var dict = new ItemDictionaryDB();
            long machineIndex = dict.AddMachine(machine);
            long sourceIndex = dict.AddSource(source);
            long itemIndex = dict.AddItem(item, machineIndex, sourceIndex, statsOnly: false);

            var actual = dict.GetSourceForItem(itemIndex);
            Assert.Equal(0, actual.Key);
            Assert.NotNull(actual.Value);
        }

        #endregion

        #region RemapDatItemToMachine

        [Fact]
        public void RemapDatItemToMachineTest()
        {
            Source source = new Source(0, source: null);

            Machine origMachine = new Machine();
            origMachine.SetFieldValue(Models.Metadata.Machine.NameKey, "original");

            Machine newMachine = new Machine();
            newMachine.SetFieldValue(Models.Metadata.Machine.NameKey, "new");

            DatItem datItem = new Rom();

            var dict = new ItemDictionaryDB();
            long sourceIndex = dict.AddSource(source);
            long origMachineIndex = dict.AddMachine(origMachine);
            long newMachineIndex = dict.AddMachine(newMachine);
            long itemIndex = dict.AddItem(datItem, origMachineIndex, sourceIndex, statsOnly: false);

            dict.RemapDatItemToMachine(itemIndex, newMachineIndex);

            var actual = dict.GetMachineForItem(itemIndex);
            Assert.Equal(1, actual.Key);
            Assert.NotNull(actual.Value);
            Assert.Equal("new", actual.Value.GetStringFieldValue(Models.Metadata.Machine.NameKey));
        }

        #endregion

        #region RemoveBucket

        // TODO: Add RemoveBucket tests

        #endregion

        #region RemoveItem

        // TODO: Add RemoveItem tests

        #endregion

        #region RemoveMachine

        [Fact]
        public void RemoveMachineTest()
        {
            Machine machine = new Machine();
            var dict = new ItemDictionaryDB();
            long machineIndex = dict.AddMachine(machine);

            bool actual = dict.RemoveMachine(machineIndex);
            Assert.True(actual);
            Assert.Empty(dict.GetMachines());
        }

        #endregion

        #region BucketBy

        [Theory]
        [InlineData(ItemKey.NULL, 2)]
        [InlineData(ItemKey.Machine, 2)]
        [InlineData(ItemKey.CRC, 1)]
        [InlineData(ItemKey.SHA1, 4)]
        public void BucketByTest(ItemKey itemKey, int expected)
        {
            // Setup the items
            Source source = new Source(0, source: null);

            Machine machine1 = new Machine();
            machine1.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "game-1");

            Machine machine2 = new Machine();
            machine2.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "game-2");

            DatItem rom1 = new Rom();
            rom1.SetName("rom-1");
            rom1.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            rom1.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "0000000fbbb37f8488100b1b4697012de631a5e6");
            rom1.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, "1024");

            DatItem rom2 = new Rom();
            rom2.SetName("rom-2");
            rom2.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            rom2.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "000000e948edcb4f7704b8af85a77a3339ecce44");
            rom2.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, "1024");

            DatItem rom3 = new Rom();
            rom3.SetName("rom-3");
            rom3.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            rom3.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "00000ea4014ce66679e7e17d56ac510f67e39e26");
            rom3.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, "1024");

            DatItem rom4 = new Rom();
            rom4.SetName("rom-4");
            rom4.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            rom4.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "00000151d437442e74e5134023fab8bf694a2487");
            rom4.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, "1024");

            // Setup the dictionary
            var dict = new ItemDictionaryDB();
            long sourceIndex = dict.AddSource(source);
            long machine1Index = dict.AddMachine(machine1);
            long machine2Index = dict.AddMachine(machine2);
            dict.AddItem(rom1, machine1Index, sourceIndex, statsOnly: false);
            dict.AddItem(rom2, machine1Index, sourceIndex, statsOnly: false);
            dict.AddItem(rom3, machine2Index, sourceIndex, statsOnly: false);
            dict.AddItem(rom4, machine2Index, sourceIndex, statsOnly: false);

            dict.BucketBy(itemKey);
            Assert.Equal(expected, dict.SortedKeys.Length);
        }

        #endregion

        #region Deduplicate

        // TODO: Add Deduplicate tests

        #endregion

        #region GetDuplicates

        [Theory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void GetDuplicatesTest(bool hasDuplicate, int expected)
        {
            // Setup the items
            Source source = new Source(0, source: null);

            Machine machine = new Machine();
            machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "game-1");

            DatItem rom1 = new Rom();
            rom1.SetName("rom-1");
            rom1.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "0000000fbbb37f8488100b1b4697012de631a5e6");
            rom1.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, "1024");

            DatItem rom2 = new Rom();
            rom2.SetName("rom-2");
            rom2.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "000000e948edcb4f7704b8af85a77a3339ecce44");
            rom2.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, "1024");

            // Setup the dictionary
            var dict = new ItemDictionaryDB();
            long sourceIndex = dict.AddSource(source);
            long machineIndex = dict.AddMachine(machine);
            dict.AddItem(rom1, machineIndex, sourceIndex, statsOnly: false);
            dict.AddItem(rom2, machineIndex, sourceIndex, statsOnly: false);

            // Setup the test item
            DatItem rom = new Rom();
            rom.SetName("rom-1");
            rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "0000000fbbb37f8488100b1b4697012de631a5e6");
            rom.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, hasDuplicate ? "1024" : "2048");

            var actual = dict.GetDuplicates(new KeyValuePair<long, DatItem>(-1, rom));
            Assert.Equal(expected, actual.Count);
        }

        #endregion

        #region HasDuplicates

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void HasDuplicatesTest(bool expected)
        {
            // Setup the items
            Source source = new Source(0, source: null);

            Machine machine = new Machine();
            machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "game-1");

            DatItem rom1 = new Rom();
            rom1.SetName("rom-1");
            rom1.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "0000000fbbb37f8488100b1b4697012de631a5e6");
            rom1.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, "1024");

            DatItem rom2 = new Rom();
            rom2.SetName("rom-2");
            rom2.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "000000e948edcb4f7704b8af85a77a3339ecce44");
            rom2.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, "1024");

            // Setup the dictionary
            var dict = new ItemDictionaryDB();
            long sourceIndex = dict.AddSource(source);
            long machineIndex = dict.AddMachine(machine);
            dict.AddItem(rom1, machineIndex, sourceIndex, statsOnly: false);
            dict.AddItem(rom2, machineIndex, sourceIndex, statsOnly: false);

            // Setup the test item
            DatItem rom = new Rom();
            rom.SetName("rom-1");
            rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "0000000fbbb37f8488100b1b4697012de631a5e6");
            rom.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, expected ? "1024" : "2048");

            bool actual = dict.HasDuplicates(new KeyValuePair<long, DatItem>(-1, rom));
            Assert.Equal(expected, actual);
        }

        #endregion

        #region RecalculateStats

        // TODO: Add RecalculateStats tests

        #endregion
    }
}