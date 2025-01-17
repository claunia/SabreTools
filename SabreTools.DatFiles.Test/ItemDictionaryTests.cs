using System.Collections.Generic;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.Hashing;
using Xunit;

namespace SabreTools.DatFiles.Test
{
    public class ItemDictionaryTests
    {
        #region AddItem

        // TODO: Add AddItem tests
        // - Disk, with/without hashes
        // - File, with/without hashes
        // - Media, with/without hashes
        // - Rom, with/without hashes, with/without size
        // - Stats only/actual add

        #endregion

        #region ClearMarked

        [Fact]
        public void ClearMarkedTest()
        {
            // Setup the items
            Machine machine = new Machine();
            machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "game-1");

            DatItem rom1 = new Rom();
            rom1.SetName("rom-1");
            rom1.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            rom1.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "0000000fbbb37f8488100b1b4697012de631a5e6");
            rom1.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, "1024");
            rom1.CopyMachineInformation(machine);

            DatItem rom2 = new Rom();
            rom2.SetName("rom-2");
            rom2.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            rom2.SetFieldValue<bool?>(DatItem.RemoveKey, true);
            rom2.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "000000e948edcb4f7704b8af85a77a3339ecce44");
            rom2.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, "1024");
            rom2.CopyMachineInformation(machine);

            // Setup the dictionary
            var dict = new ItemDictionary();
            dict.AddItem(rom1, statsOnly: false);
            dict.AddItem(rom2, statsOnly: false);

            dict.ClearMarked();
            Assert.Empty(dict.GetItemsForBucket("default"));
            List<DatItem> items = dict.GetItemsForBucket("game-1");
            Assert.Single(items);
        }

        #endregion

        #region GetItemsForBucket

        [Fact]
        public void GetItemsForBucket_NullBucketName()
        {
            Source source = new Source(0, source: null);

            Machine machine = new Machine();
            machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "machine");

            DatItem item = new Rom();
            item.SetFieldValue<Source?>(DatItem.SourceKey, source);
            item.SetFieldValue<Machine?>(DatItem.MachineKey, machine);

            var dict = new ItemDictionary();
            _ = dict.AddItem(item, statsOnly: false);

            var actual = dict.GetItemsForBucket(null, filter: false);

            Assert.Empty(actual);
        }

        [Fact]
        public void GetItemsForBucket_InvalidBucketName()
        {
            Source source = new Source(0, source: null);

            Machine machine = new Machine();
            machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "machine");

            DatItem item = new Rom();
            item.SetFieldValue<Source?>(DatItem.SourceKey, source);
            item.SetFieldValue<Machine?>(DatItem.MachineKey, machine);

            var dict = new ItemDictionary();
            _ = dict.AddItem(item, statsOnly: false);

            var actual = dict.GetItemsForBucket("INVALID", filter: false);

            Assert.Empty(actual);
        }

        [Fact]
        public void GetItemsForBucket_RemovedFilter()
        {
            Source source = new Source(0, source: null);

            Machine machine = new Machine();
            machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "machine");

            DatItem item = new Rom();
            item.SetFieldValue<bool?>(DatItem.RemoveKey, true);
            item.SetFieldValue<Source?>(DatItem.SourceKey, source);
            item.SetFieldValue<Machine?>(DatItem.MachineKey, machine);

            var dict = new ItemDictionary();
            _ = dict.AddItem(item, statsOnly: false);

            var actual = dict.GetItemsForBucket("machine", filter: true);

            Assert.Empty(actual);
        }

        [Fact]
        public void GetItemsForBucket_RemovedNoFilter()
        {
            Source source = new Source(0, source: null);

            Machine machine = new Machine();
            machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "machine");

            DatItem item = new Rom();
            item.SetFieldValue<bool?>(DatItem.RemoveKey, true);
            item.SetFieldValue<Source?>(DatItem.SourceKey, source);
            item.SetFieldValue<Machine?>(DatItem.MachineKey, machine);

            var dict = new ItemDictionary();
            _ = dict.AddItem(item, statsOnly: false);

            var actual = dict.GetItemsForBucket("machine", filter: false);

            Assert.Single(actual);
        }

        [Fact]
        public void GetItemsForBucket_Standard()
        {
            Source source = new Source(0, source: null);

            Machine machine = new Machine();
            machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "machine");

            DatItem item = new Rom();
            item.SetFieldValue<Source?>(DatItem.SourceKey, source);
            item.SetFieldValue<Machine?>(DatItem.MachineKey, machine);

            var dict = new ItemDictionary();
            _ = dict.AddItem(item, statsOnly: false);

            var actual = dict.GetItemsForBucket("machine", filter: false);

            Assert.Single(actual);
        }

        #endregion

        #region RemoveBucket

        [Fact]
        public void RemoveBucketTest()
        {
            Machine machine = new Machine();
            machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "game-1");

            DatItem datItem = new Rom();
            datItem.SetName("rom-1");
            datItem.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            datItem.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "0000000fbbb37f8488100b1b4697012de631a5e6");
            datItem.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, "1024");
            datItem.CopyMachineInformation(machine);

            var dict = new ItemDictionary();
            dict.AddItem(datItem, statsOnly: false);

            dict.RemoveBucket("game-1");

            Assert.Empty(dict.GetItemsForBucket("default"));
            Assert.Empty(dict.GetItemsForBucket("game-1"));
        }

        #endregion

        #region RemoveItem

        [Fact]
        public void RemoveItemTest()
        {
            Machine machine = new Machine();
            machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "game-1");

            DatItem datItem = new Rom();
            datItem.SetName("rom-1");
            datItem.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            datItem.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "0000000fbbb37f8488100b1b4697012de631a5e6");
            datItem.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, "1024");
            datItem.CopyMachineInformation(machine);

            var dict = new ItemDictionary();
            dict.AddItem(datItem, statsOnly: false);

            dict.RemoveItem("game-1", (Rom)datItem.Clone());

            Assert.Empty(dict.GetItemsForBucket("default"));
            Assert.Empty(dict.GetItemsForBucket("game-1"));
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
            Machine machine1 = new Machine();
            machine1.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "game-1");

            Machine machine2 = new Machine();
            machine2.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "game-2");

            DatItem rom1 = new Rom();
            rom1.SetName("rom-1");
            rom1.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            rom1.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "0000000fbbb37f8488100b1b4697012de631a5e6");
            rom1.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, "1024");
            rom1.CopyMachineInformation(machine1);

            DatItem rom2 = new Rom();
            rom2.SetName("rom-2");
            rom2.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            rom2.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "000000e948edcb4f7704b8af85a77a3339ecce44");
            rom2.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, "1024");
            rom1.CopyMachineInformation(machine1);

            DatItem rom3 = new Rom();
            rom3.SetName("rom-3");
            rom3.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            rom3.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "00000ea4014ce66679e7e17d56ac510f67e39e26");
            rom3.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, "1024");
            rom1.CopyMachineInformation(machine2);

            DatItem rom4 = new Rom();
            rom4.SetName("rom-4");
            rom4.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            rom4.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "00000151d437442e74e5134023fab8bf694a2487");
            rom4.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, "1024");
            rom1.CopyMachineInformation(machine2);

            // Setup the dictionary
            var dict = new ItemDictionary();
            dict.AddItem(rom1, statsOnly: false);
            dict.AddItem(rom2, statsOnly: false);
            dict.AddItem(rom3, statsOnly: false);
            dict.AddItem(rom4, statsOnly: false);

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
            Machine machine = new Machine();
            machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "game-1");

            DatItem rom1 = new Rom();
            rom1.SetName("rom-1");
            rom1.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "0000000fbbb37f8488100b1b4697012de631a5e6");
            rom1.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, "1024");
            rom1.CopyMachineInformation(machine);

            DatItem rom2 = new Rom();
            rom2.SetName("rom-2");
            rom2.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "000000e948edcb4f7704b8af85a77a3339ecce44");
            rom2.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, "1024");
            rom1.CopyMachineInformation(machine);

            // Setup the dictionary
            var dict = new ItemDictionary();
            dict.AddItem(rom1, statsOnly: false);
            dict.AddItem(rom2, statsOnly: false);

            // Setup the test item
            DatItem rom = new Rom();
            rom.SetName("rom-1");
            rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "0000000fbbb37f8488100b1b4697012de631a5e6");
            rom.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, hasDuplicate ? "1024" : "2048");
            rom1.CopyMachineInformation(machine);

            var actual = dict.GetDuplicates(rom);
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
            Machine machine = new Machine();
            machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "game-1");

            DatItem rom1 = new Rom();
            rom1.SetName("rom-1");
            rom1.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "0000000fbbb37f8488100b1b4697012de631a5e6");
            rom1.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, "1024");
            rom1.CopyMachineInformation(machine);

            DatItem rom2 = new Rom();
            rom2.SetName("rom-2");
            rom2.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "000000e948edcb4f7704b8af85a77a3339ecce44");
            rom2.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, "1024");
            rom1.CopyMachineInformation(machine);

            // Setup the dictionary
            var dict = new ItemDictionary();
            dict.AddItem("game-1", rom1);
            dict.AddItem("game-1", rom2);

            // Setup the test item
            DatItem rom = new Rom();
            rom.SetName("rom-1");
            rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "0000000fbbb37f8488100b1b4697012de631a5e6");
            rom.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, expected ? "1024" : "2048");
            rom1.CopyMachineInformation(machine);

            bool actual = dict.HasDuplicates(rom);
            Assert.Equal(expected, actual);
        }

        #endregion

        #region RecalculateStats

        [Fact]
        public void RecalculateStatsTest()
        {
            Source source = new Source(0, source: null);

            Machine machine = new Machine();
            machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "machine");

            DatItem item = new Rom();
            item.SetName("rom");
            item.SetFieldValue<long?>(Models.Metadata.Rom.SizeKey, 12345);
            item.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "deadbeef");
            item.SetFieldValue<Source?>(DatItem.SourceKey, source);
            item.SetFieldValue<Machine?>(DatItem.MachineKey, machine);

            var dict = new ItemDictionary();
            _ = dict.AddItem(item, statsOnly: false);

            Assert.Equal(1, dict.DatStatistics.TotalCount);
            Assert.Equal(1, dict.DatStatistics.ItemCounts[ItemType.Rom]);
            Assert.Equal(12345, dict.DatStatistics.TotalSize);
            Assert.Equal(1, dict.DatStatistics.HashCounts[HashType.CRC32]);
            Assert.Equal(0, dict.DatStatistics.HashCounts[HashType.MD5]);

            item.SetFieldValue<string?>(Models.Metadata.Rom.MD5Key, "deadbeef");

            dict.RecalculateStats();

            Assert.Equal(1, dict.DatStatistics.TotalCount);
            Assert.Equal(1, dict.DatStatistics.ItemCounts[ItemType.Rom]);
            Assert.Equal(12345, dict.DatStatistics.TotalSize);
            Assert.Equal(1, dict.DatStatistics.HashCounts[HashType.CRC32]);
            Assert.Equal(1, dict.DatStatistics.HashCounts[HashType.MD5]);
        }

        #endregion
    }
}