using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using Xunit;

namespace SabreTools.Test.DatItems
{
    public class DatItemTests
    {
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
    
        // TODO: Add tests for DatItem.Merge
        // TODO: Add tests for ResolveNames
        // TODO: Add tests for Sort
    }
}