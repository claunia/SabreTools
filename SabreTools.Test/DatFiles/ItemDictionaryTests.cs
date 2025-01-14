using System.Collections.Generic;
using SabreTools.DatFiles;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using Xunit;

namespace SabreTools.Test.DatFiles
{
    public class ItemDictionaryTests
    {
        [Theory]
        [InlineData(ItemKey.NULL, 2)]
        [InlineData(ItemKey.Machine, 2)]
        [InlineData(ItemKey.CRC, 1)]
        [InlineData(ItemKey.SHA1, 4)]
        public void BucketByTest(ItemKey itemKey, int expected)
        {
            // Setup the items
            var machine1 = new Machine();
            machine1.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "game-1");

            var machine2 = new Machine();
            machine2.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "game-2");

            var rom1 = new Rom();
            rom1.SetName("rom-1");
            rom1.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            rom1.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "0000000fbbb37f8488100b1b4697012de631a5e6");
            rom1.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, "1024");
            rom1.CopyMachineInformation(machine1);

            var rom2 = new Rom();
            rom2.SetName("rom-2");
            rom2.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            rom2.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "000000e948edcb4f7704b8af85a77a3339ecce44");
            rom2.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, "1024");
            rom1.CopyMachineInformation(machine1);

            var rom3 = new Rom();
            rom3.SetName("rom-3");
            rom3.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            rom3.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "00000ea4014ce66679e7e17d56ac510f67e39e26");
            rom3.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, "1024");
            rom1.CopyMachineInformation(machine2);

            var rom4 = new Rom();
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

            dict.BucketBy(itemKey, DedupeType.None);
            Assert.Equal(expected, dict.SortedKeys.Count);
        }

        [Fact]
        public void ClearEmptyTest()
        {
            // Setup the dictionary
            var dict = new ItemDictionary();
            dict.AddItem("game-1", new Rom());

            dict.ClearEmpty();
            Assert.Single(dict.SortedKeys);
        }

        [Fact]
        public void ClearMarkedTest()
        {
            // Setup the items
            var machine1 = new Machine();
            machine1.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "game-1");

            var rom1 = new Rom();
            rom1.SetName("rom-1");
            rom1.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            rom1.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "0000000fbbb37f8488100b1b4697012de631a5e6");
            rom1.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, "1024");
            rom1.CopyMachineInformation(machine1);

            var rom2 = new Rom();
            rom2.SetName("rom-2");
            rom2.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            rom2.SetFieldValue<bool?>(DatItem.RemoveKey, true);
            rom2.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "000000e948edcb4f7704b8af85a77a3339ecce44");
            rom2.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, "1024");
            rom1.CopyMachineInformation(machine1);

            // Setup the dictionary
            var dict = new ItemDictionary();
            dict.AddItem("game-1", rom1);
            dict.AddItem("game-1", rom2);

            dict.ClearMarked();
            string key = Assert.Single(dict.SortedKeys);
            Assert.Equal("game-1", key);
            List<DatItem> items = dict.GetItemsForBucket(key);
            Assert.Single(items);
        }

        [Theory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void GetDuplicatesTest(bool hasDuplicate, int expected)
        {
            // Setup the items
            var machine1 = new Machine();
            machine1.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "game-1");

            var rom1 = new Rom();
            rom1.SetName("rom-1");
            rom1.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "0000000fbbb37f8488100b1b4697012de631a5e6");
            rom1.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, "1024");
            rom1.CopyMachineInformation(machine1);

            var rom2 = new Rom();
            rom2.SetName("rom-2");
            rom2.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "000000e948edcb4f7704b8af85a77a3339ecce44");
            rom2.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, "1024");
            rom1.CopyMachineInformation(machine1);

            // Setup the dictionary
            var dict = new ItemDictionary();
            dict.AddItem("game-1", rom1);
            dict.AddItem("game-1", rom2);

            // Setup the test item
            var rom = new Rom();
            rom.SetName("rom-1");
            rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "0000000fbbb37f8488100b1b4697012de631a5e6");
            rom.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, hasDuplicate ? "1024" : "2048");
            rom1.CopyMachineInformation(machine1);

            var actual = dict.GetDuplicates(rom);
            Assert.Equal(expected, actual.Count);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void HasDuplicatesTest(bool expected)
        {
            // Setup the items
            var machine1 = new Machine();
            machine1.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "game-1");

            var rom1 = new Rom();
            rom1.SetName("rom-1");
            rom1.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "0000000fbbb37f8488100b1b4697012de631a5e6");
            rom1.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, "1024");
            rom1.CopyMachineInformation(machine1);

            var rom2 = new Rom();
            rom2.SetName("rom-2");
            rom2.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "000000e948edcb4f7704b8af85a77a3339ecce44");
            rom2.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, "1024");
            rom1.CopyMachineInformation(machine1);

            // Setup the dictionary
            var dict = new ItemDictionary();
            dict.AddItem("game-1", rom1);
            dict.AddItem("game-1", rom2);

            // Setup the test item
            var rom = new Rom();
            rom.SetName("rom-1");
            rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, "0000000fbbb37f8488100b1b4697012de631a5e6");
            rom.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, expected ? "1024" : "2048");
            rom1.CopyMachineInformation(machine1);

            bool actual = dict.HasDuplicates(rom);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ResetStatisticsTest()
        {
            var dict = new ItemDictionary();
            dict.DatStatistics.GameCount = 1;
            dict.DatStatistics.ResetStatistics();
            Assert.Equal(0, dict.DatStatistics.GameCount);
        }
    }
}