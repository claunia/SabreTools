using SabreTools.Core;
using SabreTools.DatItems;
using SabreTools.FileTypes;
using SabreTools.FileTypes.Aaru;
using SabreTools.FileTypes.Archives;
using SabreTools.FileTypes.CHD;
using Xunit;

namespace SabreTools.Test.DatItems
{
    public class DatItemTests
    {
        [Theory]
        [InlineData(null, ItemType.Rom)]
        [InlineData(ItemType.Disk, ItemType.Disk)]
        [InlineData(ItemType.Media, ItemType.Media)]
        [InlineData(ItemType.Rom, ItemType.Rom)]
        public void CreateItemTypeTest(ItemType? itemType, ItemType expected)
        {
            var actual = DatItem.Create(itemType);
            Assert.Equal(expected, actual.ItemType);
        }

        [Theory]
        [InlineData(FileType.None, ItemType.Rom)]
        [InlineData(FileType.Folder, null)]
        [InlineData(FileType.AaruFormat, ItemType.Media)]
        [InlineData(FileType.CHD, ItemType.Disk)]
        [InlineData(FileType.ZipArchive, ItemType.Rom)]
        public void CreateBaseFileTest(FileType fileType, ItemType? expected)
        {
            var baseFile = CreateBaseFile(fileType);
            var actual = DatItem.Create(baseFile);
            Assert.Equal(expected, actual?.ItemType);
        }
    
        [Fact]
        public void DuplicateStatusUnequalTest()
        {
            var rom = new Rom();
            var disk = new Disk();
            var actual = rom.GetDuplicateStatus(disk);
            Assert.Equal((DupeType)0x00, actual);
        }

        [Fact]
        public void DuplicateStatusExternalAllTest()
        {
            var romA = new Rom
            {
                Name = "same-name",
                CRC = "DEADBEEF",
                Machine = new Machine
                {
                    Name = "name-same",
                },
                Source = new Source
                {
                    Index = 0,
                },
            };
            var romB = new Rom
            {
                Name = "same-name",
                CRC = "DEADBEEF",
                Machine = new Machine
                {
                    Name = "name-same",
                },
                Source = new Source
                {
                    Index = 1,
                },
            };

            var actual = romA.GetDuplicateStatus(romB);
            Assert.Equal(DupeType.External | DupeType.All, actual);
        }

        [Fact]
        public void DuplicateStatusExternalHashTest()
        {
            var romA = new Rom
            {
                Name = "same-name",
                CRC = "DEADBEEF",
                Machine = new Machine
                {
                    Name = "name-same",
                },
                Source = new Source
                {
                    Index = 0,
                },
            };
            var romB = new Rom
            {
                Name = "same-name",
                CRC = "DEADBEEF",
                Machine = new Machine
                {
                    Name = "not-name-same",
                },
                Source = new Source
                {
                    Index = 1,
                },
            };

            var actual = romA.GetDuplicateStatus(romB);
            Assert.Equal(DupeType.External | DupeType.Hash, actual);
        }

        [Fact]
        public void DuplicateStatusInternalAllTest()
        {
            var romA = new Rom
            {
                Name = "same-name",
                CRC = "DEADBEEF",
                Machine = new Machine
                {
                    Name = "name-same",
                },
                Source = new Source
                {
                    Index = 0,
                },
            };
            var romB = new Rom
            {
                Name = "same-name",
                CRC = "DEADBEEF",
                Machine = new Machine
                {
                    Name = "name-same",
                },
                Source = new Source
                {
                    Index = 0,
                },
            };

            var actual = romA.GetDuplicateStatus(romB);
            Assert.Equal(DupeType.Internal | DupeType.All, actual);
        }

        [Fact]
        public void DuplicateStatusInternalHashTest()
        {
            var romA = new Rom
            {
                Name = "same-name",
                CRC = "DEADBEEF",
                Machine = new Machine
                {
                    Name = "name-same",
                },
                Source = new Source
                {
                    Index = 0,
                },
            };
            var romB = new Rom
            {
                Name = "same-name",
                CRC = "DEADBEEF",
                Machine = new Machine
                {
                    Name = "not-name-same",
                },
                Source = new Source
                {
                    Index = 0,
                },
            };

            var actual = romA.GetDuplicateStatus(romB);
            Assert.Equal(DupeType.Internal | DupeType.Hash, actual);
        }
    
        [Theory]
        [InlineData(null, null, true)]
        [InlineData(null, new byte[0], true)]
        [InlineData(new byte[0], null, true)]
        [InlineData(new byte[] { 0x00 }, new byte[] { 0x00, 0x01 }, false)]
        [InlineData(new byte[] { 0x00 }, new byte[] { 0x01 }, false)]
        [InlineData(new byte[] { 0x00 }, new byte[] { 0x00 }, true)]
        public void ConditionalHashEqualsTest(byte[] first, byte[] second, bool expected)
        {
            bool actual = DatItem.ConditionalHashEquals(first, second);
            Assert.Equal(expected, actual);
        }
    
        // TODO: Add tests for DatItem.Merge
        // TODO: Add tests for ResolveNames
        // TODO: Add tests for Sort

        /// <summary>
        /// Create a BaseFile for testing
        /// </summary>
        private BaseFile CreateBaseFile(FileType fileType)
        {
            return fileType switch
            {
                FileType.Folder => new Folder(),
                FileType.AaruFormat => new AaruFormat(),
                FileType.CHD => new CHDFileV5(),
                FileType.ZipArchive => new ZipArchive(),
                _ => new BaseFile(),
            };
        }
    }
}