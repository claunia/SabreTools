using System.Collections.Generic;

using SabreTools.Core;
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
            // Setup the dictionary
            var dict = new ItemDictionary
            {
                ["game-1"] =
                [
                    new Rom
                    {
                        Name = "rom-1",
                        Size = 1024,
                        CRC = "DEADBEEF",
                        SHA1 = "0000000fbbb37f8488100b1b4697012de631a5e6",
                        Machine = new Machine { Name = "game-1" },
                    },
                    new Rom
                    {
                        Name = "rom-2",
                        Size = 1024,
                        CRC = "DEADBEEF",
                        SHA1 = "000000e948edcb4f7704b8af85a77a3339ecce44",
                        Machine = new Machine { Name = "game-1" },
                    },
                ],
                ["game-2"] = new ConcurrentList<DatItem>
                {
                    new Rom
                    {
                        Name = "rom-3",
                        Size = 1024,
                        CRC = "DEADBEEF",
                        SHA1 = "00000ea4014ce66679e7e17d56ac510f67e39e26",
                        Machine = new Machine { Name = "game-2" },
                    },
                    new Rom
                    {
                        Name = "rom-4",
                        Size = 1024,
                        CRC = "DEADBEEF",
                        SHA1 = "00000151d437442e74e5134023fab8bf694a2487",
                        Machine = new Machine { Name = "game-2" },
                    },
                },
            };

            dict.BucketBy(itemKey, DedupeType.None);
            Assert.Equal(expected, dict.Keys.Count);
        }
    
        [Fact]
        public void ClearEmptyTest()
        {
            // Setup the dictionary
            var dict = new ItemDictionary
            {
                ["game-1"] = new ConcurrentList<DatItem> { new Rom(), },
                ["game-2"] = new ConcurrentList<DatItem>(),
                ["game-3"] = null,
            };

            dict.ClearEmpty();
            Assert.Single(dict.Keys);
        }

        [Fact]
        public void ClearMarkedTest()
        {
            // Setup the dictionary
            var dict = new ItemDictionary
            {
                ["game-1"] = new ConcurrentList<DatItem>
                {
                    new Rom
                    {
                        Name = "rom-1",
                        Size = 1024,
                        CRC = "DEADBEEF",
                        SHA1 = "0000000fbbb37f8488100b1b4697012de631a5e6",
                        Machine = new Machine { Name = "game-1" },
                    },
                    new Rom
                    {
                        Name = "rom-2",
                        Size = 1024,
                        CRC = "DEADBEEF",
                        SHA1 = "000000e948edcb4f7704b8af85a77a3339ecce44",
                        Machine = new Machine { Name = "game-1" },
                        Remove = true,
                    },
                },
            };

            dict.ClearMarked();
            string key = Assert.Single(dict.Keys);
            Assert.Equal("game-1", key);
            Assert.NotNull(dict[key]);
            Assert.Single(dict[key]!);
        }
    
        [Theory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void GetDuplicatesTest(bool hasDuplicate, int expected)
        {
            // Setup the dictionary
            var dict = new ItemDictionary
            {
                ["game-1"] = new ConcurrentList<DatItem>
                {
                    new Rom
                    {
                        Name = "rom-1",
                        Size = 1024,
                        SHA1 = "0000000fbbb37f8488100b1b4697012de631a5e6",
                        Machine = new Machine { Name = "game-1" },
                    },
                    new Rom
                    {
                        Name = "rom-2",
                        Size = 1024,
                        SHA1 = "000000e948edcb4f7704b8af85a77a3339ecce44",
                        Machine = new Machine { Name = "game-1" },
                    },
                },
            };

            var rom = new Rom
            {
                Name = "rom-1",
                Size = hasDuplicate ? 1024 : 2048,
                SHA1 = "0000000fbbb37f8488100b1b4697012de631a5e6",
                Machine = new Machine { Name = "game-1" },
            };

            var actual = dict.GetDuplicates(rom);
            Assert.Equal(expected, actual.Count);
        }
    
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void HasDuplicatesTest(bool expected)
        {
            // Setup the dictionary
            var dict = new ItemDictionary
            {
                ["game-1"] = new ConcurrentList<DatItem>
                {
                    new Rom
                    {
                        Name = "rom-1",
                        Size = 1024,
                        SHA1 = "0000000fbbb37f8488100b1b4697012de631a5e6",
                        Machine = new Machine { Name = "game-1" },
                    },
                    new Rom
                    {
                        Name = "rom-2",
                        Size = 1024,
                        SHA1 = "000000e948edcb4f7704b8af85a77a3339ecce44",
                        Machine = new Machine { Name = "game-1" },
                    },
                },
            };

            var rom = new Rom
            {
                Name = "rom-1",
                Size = expected ? 1024 : 2048,
                SHA1 = "0000000fbbb37f8488100b1b4697012de631a5e6",
                Machine = new Machine { Name = "game-1" },
            };

            bool actual = dict.HasDuplicates(rom);
            Assert.Equal(expected, actual);
        }
    
        [Fact]
        public void ResetStatisticsTest()
        {
            var dict = new ItemDictionary { GameCount = 1 };
            dict.ResetStatistics();
            Assert.Equal(0, dict.GameCount);
        }
    }
}