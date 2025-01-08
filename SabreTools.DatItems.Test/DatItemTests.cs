using SabreTools.DatItems.Formats;
using Xunit;

namespace SabreTools.DatItems.Test
{
    public class DatItemTests
    {
        #region CopyMachineInformation

        [Fact]
        public void CopyMachineInformation_NewItem_Overwrite()
        {
            Machine? machineA = new Machine();
            machineA.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "machineA");

            var romA = new Rom();

            var romB = new Rom();
            romB.RemoveField(DatItem.MachineKey);

            romA.CopyMachineInformation(romB);
            var actualMachineA = romA.GetFieldValue<Machine>(DatItem.MachineKey);
            Assert.NotNull(actualMachineA);
            Assert.Null(actualMachineA.GetStringFieldValue(Models.Metadata.Machine.NameKey));
        }

        [Fact]
        public void CopyMachineInformation_EmptyItem_NoChange()
        {
            Machine? machineA = new Machine();
            machineA.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "machineA");

            var romA = new Rom();
            romA.SetFieldValue(DatItem.MachineKey, machineA);

            var romB = new Rom();
            romB.RemoveField(DatItem.MachineKey);

            romA.CopyMachineInformation(romB);
            var actualMachineA = romA.GetFieldValue<Machine>(DatItem.MachineKey);
            Assert.NotNull(actualMachineA);
            Assert.Equal("machineA", actualMachineA.GetStringFieldValue(Models.Metadata.Machine.NameKey));
        }

        [Fact]
        public void CopyMachineInformation_NullMachine_NoChange()
        {
            Machine? machineA = new Machine();
            machineA.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "machineA");

            Machine? machineB = null;

            var romA = new Rom();
            romA.SetFieldValue(DatItem.MachineKey, machineA);

            var romB = new Rom();
            romB.SetFieldValue(DatItem.MachineKey, machineB);

            romA.CopyMachineInformation(romB);
            var actualMachineA = romA.GetFieldValue<Machine>(DatItem.MachineKey);
            Assert.NotNull(actualMachineA);
            Assert.Equal("machineA", actualMachineA.GetStringFieldValue(Models.Metadata.Machine.NameKey));
        }

        [Fact]
        public void CopyMachineInformation_EmptyMachine_Overwrite()
        {
            Machine? machineA = new Machine();
            machineA.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "machineA");

            Machine? machineB = new Machine();

            var romA = new Rom();
            romA.SetFieldValue(DatItem.MachineKey, machineA);

            var romB = new Rom();
            romB.SetFieldValue(DatItem.MachineKey, machineB);

            romA.CopyMachineInformation(romB);
            var actualMachineA = romA.GetFieldValue<Machine>(DatItem.MachineKey);
            Assert.NotNull(actualMachineA);
            Assert.Null(actualMachineA.GetStringFieldValue(Models.Metadata.Machine.NameKey));
        }

        [Fact]
        public void CopyMachineInformation_FilledMachine_Overwrite()
        {
            Machine? machineA = new Machine();
            machineA.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "machineA");

            Machine? machineB = new Machine();
            machineB.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "machineB");

            var romA = new Rom();
            romA.SetFieldValue(DatItem.MachineKey, machineA);

            var romB = new Rom();
            romB.SetFieldValue(DatItem.MachineKey, machineB);

            romA.CopyMachineInformation(romB);
            var actualMachineA = romA.GetFieldValue<Machine>(DatItem.MachineKey);
            Assert.NotNull(actualMachineA);
            Assert.Equal("machineB", actualMachineA.GetStringFieldValue(Models.Metadata.Machine.NameKey));
        }

        [Fact]
        public void CopyMachineInformation_MismatchedType_Overwrite()
        {
            Machine? machineA = new Machine();
            machineA.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "machineA");

            Machine? machineB = new Machine();
            machineB.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "machineB");

            var romA = new Rom();
            romA.SetFieldValue(DatItem.MachineKey, machineA);

            var diskB = new Disk();
            diskB.SetFieldValue(DatItem.MachineKey, machineB);

            romA.CopyMachineInformation(diskB);
            var actualMachineA = romA.GetFieldValue<Machine>(DatItem.MachineKey);
            Assert.NotNull(actualMachineA);
            Assert.Equal("machineB", actualMachineA.GetStringFieldValue(Models.Metadata.Machine.NameKey));
        }

        #endregion

        #region CompareTo

        // TODO: Implement CompareTo tests

        #endregion

        #region Equals

        // TODO: Implement Equals tests

        #endregion

        #region GetDuplicateStatus

        [Fact]
        public void GetDuplicateStatus_NullOther_NoDupe()
        {
            DatItem item = new Rom();
            DatItem? lastItem = null;
            var actual = item.GetDuplicateStatus(lastItem);
            Assert.Equal((DupeType)0x00, actual);
        }

        [Fact]
        public void GetDuplicateStatus_DifferentTypes_NoDupe()
        {
            var rom = new Rom();
            DatItem? lastItem = new Disk();
            var actual = rom.GetDuplicateStatus(lastItem);
            Assert.Equal((DupeType)0x00, actual);
        }

        [Fact]
        public void GetDuplicateStatus_MismatchedHashes_NoDupe()
        {
            Machine? machineA = new Machine();
            machineA.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "name-same");

            Machine? machineB = new Machine();
            machineB.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "name-same");

            var romA = new Rom();
            romA.SetName("same-name");
            romA.SetFieldValue(Models.Metadata.Rom.CRCKey, "BEEFDEAD");
            romA.SetFieldValue<Source?>(DatItem.SourceKey, new Source(0));
            romA.CopyMachineInformation(machineA);

            var romB = new Rom();
            romB.SetName("same-name");
            romB.SetFieldValue(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            romB.SetFieldValue<Source?>(DatItem.SourceKey, new Source(1));
            romB.CopyMachineInformation(machineB);

            var actual = romA.GetDuplicateStatus(romB);
            Assert.Equal((DupeType)0x00, actual);
        }

        [Fact]
        public void GetDuplicateStatus_DifferentSource_NameMatch_ExternalAll()
        {
            Machine? machineA = new Machine();
            machineA.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "name-same");

            Machine? machineB = new Machine();
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
        public void GetDuplicateStatus_DifferentSource_NoNameMatch_ExternalHash()
        {
            Machine? machineA = new Machine();
            machineA.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "name-same");

            Machine? machineB = new Machine();
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
        public void GetDuplicateStatus_SameSource_NameMatch_InternalAll()
        {
            Machine? machineA = new Machine();
            machineA.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "name-same");

            Machine? machineB = new Machine();
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
        public void GetDuplicateStatus_SameSource_NoNameMatch_InternalHash()
        {
            Machine? machineA = new Machine();
            machineA.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "name-same");

            Machine? machineB = new Machine();
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

        #endregion

        #region GetDuplicateStatusDB

        // TODO: Implement GetDuplicateStatusDB tests

        #endregion

        #region PassesFilter

        // TODO: Implement PassesFilter tests

        #endregion

        #region GetKey

        // TODO: Implement GetKey tests

        #endregion

        #region GetName

        // TODO: Implement GetName tests

        #endregion

        #region SetName

        // TODO: Implement SetName tests

        #endregion

        #region Clone

        // TODO: Implement Clone tests

        #endregion

        #region GetInternalClone

        // TODO: Implement GetInternalClone tests

        #endregion
    }
}