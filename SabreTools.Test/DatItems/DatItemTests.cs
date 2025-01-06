using SabreTools.Core.Tools;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
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
        [InlineData(FileType.None, ItemType.Rom)]
        [InlineData(FileType.Folder, null)]
        [InlineData(FileType.AaruFormat, ItemType.Media)]
        [InlineData(FileType.CHD, ItemType.Disk)]
        [InlineData(FileType.ZipArchive, ItemType.Rom)]
        public void CreateBaseFileTest(FileType fileType, ItemType? expected)
        {
            var baseFile = CreateBaseFile(fileType);
            var actual = DatItemTool.CreateDatItem(baseFile);
            Assert.Equal(expected, actual?.GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>());
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
            var machineA = new Machine();
            machineA.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "name-same");

            var machineB = new Machine();
            machineB.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "name-same");

            var romA = new Rom();
            romA.SetName("same-name");
            romA.SetFieldValue(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            romA.SetFieldValue<Source?>(DatItem.SourceKey, new Source(0));
            romA.CopyMachineInformation(machineA);

            var romB = new Rom();
            romB.SetName("same-name");
            romB.SetFieldValue(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            romB.SetFieldValue<Source?>(DatItem.SourceKey, new Source(1));
            romB.CopyMachineInformation(machineB);

            var actual = romA.GetDuplicateStatus(romB);
            Assert.Equal(DupeType.External | DupeType.All, actual);
        }

        [Fact]
        public void DuplicateStatusExternalHashTest()
        {
            var machineA = new Machine();
            machineA.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "name-same");

            var machineB = new Machine();
            machineB.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "not-name-same");

            var romA = new Rom();
            romA.SetName("same-name");
            romA.SetFieldValue(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            romA.SetFieldValue<Source?>(DatItem.SourceKey, new Source(0));
            romA.CopyMachineInformation(machineA);

            var romB = new Rom();
            romB.SetName("same-name");
            romB.SetFieldValue(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            romB.SetFieldValue<Source?>(DatItem.SourceKey, new Source(1));
            romB.CopyMachineInformation(machineB);

            var actual = romA.GetDuplicateStatus(romB);
            Assert.Equal(DupeType.External | DupeType.Hash, actual);
        }

        [Fact]
        public void DuplicateStatusInternalAllTest()
        {
            var machineA = new Machine();
            machineA.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "name-same");

            var machineB = new Machine();
            machineB.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "name-same");

            var romA = new Rom();
            romA.SetName("same-name");
            romA.SetFieldValue(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            romA.SetFieldValue<Source?>(DatItem.SourceKey, new Source(0));
            romA.CopyMachineInformation(machineA);

            var romB = new Rom();
            romB.SetName("same-name");
            romB.SetFieldValue(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            romB.SetFieldValue<Source?>(DatItem.SourceKey, new Source(0));
            romB.CopyMachineInformation(machineB);

            var actual = romA.GetDuplicateStatus(romB);
            Assert.Equal(DupeType.Internal | DupeType.All, actual);
        }

        [Fact]
        public void DuplicateStatusInternalHashTest()
        {
            var machineA = new Machine();
            machineA.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "name-same");

            var machineB = new Machine();
            machineB.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "not-name-same");

            var romA = new Rom();
            romA.SetName("same-name");
            romA.SetFieldValue(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            romA.SetFieldValue<Source?>(DatItem.SourceKey, new Source(0));
            romA.CopyMachineInformation(machineA);

            var romB = new Rom();
            romB.SetName("same-name");
            romB.SetFieldValue(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            romB.SetFieldValue<Source?>(DatItem.SourceKey, new Source(0));
            romB.CopyMachineInformation(machineB);

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
        public void ConditionalHashEqualsTest(byte[]? first, byte[]? second, bool expected)
        {
            bool actual = SabreTools.Core.Tools.Utilities.ConditionalHashEquals(first, second);
            Assert.Equal(expected, actual);
        }
    
        // TODO: Add tests for DatItem.Merge
        // TODO: Add tests for ResolveNames
        // TODO: Add tests for Sort

        /// <summary>
        /// Create a BaseFile for testing
        /// </summary>
        private static BaseFile CreateBaseFile(FileType fileType)
        {
            return fileType switch
            {
                FileType.Folder => new Folder(),
                FileType.AaruFormat => new AaruFormat(),
                FileType.CHD => new CHDFile(),
                FileType.ZipArchive => new ZipArchive(),
                _ => new BaseFile(),
            };
        }
    }
}