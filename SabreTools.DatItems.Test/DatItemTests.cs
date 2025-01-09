using SabreTools.DatItems.Formats;
using Xunit;

namespace SabreTools.DatItems.Test
{
    public class DatItemTests
    {
        #region Private Testing Classes

        /// <summary>
        /// Testing implementation of Models.Metadata.DatItem
        /// </summary>
        private class TestDatItemModel : Models.Metadata.DatItem
        {
            public const string NameKey = "__NAME__";
        }

        /// <summary>
        /// Testing implementation of DatItem
        /// </summary>
        private class TestDatItem : DatItem<TestDatItemModel>
        {
            protected override string? NameKey => _nameKey;
            private readonly string? _nameKey;

            protected override ItemType ItemType => ItemType.Blank;

            public TestDatItem() => _nameKey = TestDatItemModel.NameKey;

            public TestDatItem(string? nameKey) => _nameKey = nameKey;
        }

        #endregion

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

        [Fact]
        public void CompareTo_NullOther_Returns1()
        {
            DatItem self = new Rom();
            DatItem? other = null;

            int actual = self.CompareTo(other);
            Assert.Equal(1, actual);
        }

        [Fact]
        public void CompareTo_DifferentOther_Returns1()
        {
            DatItem self = new Rom();
            self.SetName("name");

            DatItem? other = new Disk();
            other.SetName("name");

            int actual = self.CompareTo(other);
            Assert.Equal(1, actual);
        }

        [Fact]
        public void CompareTo_Empty_Returns1()
        {
            DatItem self = new Rom();
            DatItem? other = new Rom();

            int actual = self.CompareTo(other);
            Assert.Equal(1, actual);
        }

        [Theory]
        [InlineData(null, null, 0)]
        [InlineData("name", null, 1)]
        [InlineData("name", "other", -1)]
        [InlineData(null, "name", -1)]
        [InlineData("other", "name", 1)]
        [InlineData("name", "name", 0)]
        public void CompareTo_NamesOnly(string? selfName, string? otherName, int expected)
        {
            DatItem self = new Rom();
            self.SetName(selfName);
            self.SetFieldValue(Models.Metadata.Rom.CRCKey, "DEADBEEF");

            DatItem? other = new Rom();
            other.SetName(otherName);
            other.SetFieldValue(Models.Metadata.Rom.CRCKey, "DEADBEEF");

            int actual = self.CompareTo(other);
            Assert.Equal(expected, actual);
        }

        #endregion

        #region Equals

        [Fact]
        public void Equals_Null_False()
        {
            DatItem self = new TestDatItem();
            DatItem? other = null;

            bool actual = self.Equals(other);
            Assert.False(actual);
        }

        [Fact]
        public void Equals_MismatchedType_False()
        {
            DatItem self = new TestDatItem();
            DatItem? other = new Rom();

            bool actual = self.Equals(other);
            Assert.False(actual);
        }

        [Fact]
        public void Equals_DefaultInternal_True()
        {
            DatItem self = new TestDatItem();
            DatItem? other = new TestDatItem();

            bool actual = self.Equals(other);
            Assert.True(actual);
        }

        [Fact]
        public void Equals_MismatchedInternal_False()
        {
            DatItem self = new TestDatItem();
            self.SetName("self");

            DatItem? other = new TestDatItem();
            other.SetName("other");

            bool actual = self.Equals(other);
            Assert.False(actual);
        }

        [Fact]
        public void Equals_EqualInternal_True()
        {
            DatItem self = new TestDatItem();
            self.SetName("name");

            DatItem? other = new TestDatItem();
            other.SetName("name");

            bool actual = self.Equals(other);
            Assert.True(actual);
        }

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

        // TODO: Change when Machine retrieval gets fixed
        #region GetDuplicateStatusDB

        [Fact]
        public void GetDuplicateStatusDB_NullOther_NoDupe()
        {
            Source? selfSource = null;
            Source? lastSource = null;

            DatItem item = new Rom();
            DatItem? lastItem = null;
            var actual = item.GetDuplicateStatusDB(selfSource, lastItem, lastSource);
            Assert.Equal((DupeType)0x00, actual);
        }

        [Fact]
        public void GetDuplicateStatusDB_DifferentTypes_NoDupe()
        {
            Source? selfSource = null;
            Source? lastSource = null;

            var rom = new Rom();
            DatItem? lastItem = new Disk();
            var actual = rom.GetDuplicateStatusDB(selfSource, lastItem, lastSource);
            Assert.Equal((DupeType)0x00, actual);
        }

        [Fact]
        public void GetDuplicateStatusDB_MismatchedHashes_NoDupe()
        {
            Source? sourceA = new Source(0);
            Source? sourceB = new Source(1);

            Machine? machineA = new Machine();
            machineA.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "name-same");

            Machine? machineB = new Machine();
            machineB.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "name-same");

            var romA = new Rom();
            romA.SetName("same-name");
            romA.SetFieldValue(Models.Metadata.Rom.CRCKey, "BEEFDEAD");
            romA.CopyMachineInformation(machineA);

            var romB = new Rom();
            romB.SetName("same-name");
            romB.SetFieldValue(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            romB.CopyMachineInformation(machineB);

            var actual = romA.GetDuplicateStatusDB(sourceA, romB, sourceB);
            Assert.Equal((DupeType)0x00, actual);
        }

        [Fact]
        public void GetDuplicateStatusDB_DifferentSource_NameMatch_ExternalAll()
        {
            Source? sourceA = new Source(0);
            Source? sourceB = new Source(1);

            Machine? machineA = new Machine();
            machineA.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "name-same");

            Machine? machineB = new Machine();
            machineB.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "name-same");

            var romA = new Rom();
            romA.SetName("same-name");
            romA.SetFieldValue(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            romA.CopyMachineInformation(machineA);

            var romB = new Rom();
            romB.SetName("same-name");
            romB.SetFieldValue(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            romB.CopyMachineInformation(machineB);

            var actual = romA.GetDuplicateStatusDB(sourceA, romB, sourceB);
            Assert.Equal(DupeType.External | DupeType.All, actual);
        }

        [Fact]
        public void GetDuplicateStatusDB_DifferentSource_NoNameMatch_ExternalHash()
        {
            Source? sourceA = new Source(0);
            Source? sourceB = new Source(1);

            Machine? machineA = new Machine();
            machineA.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "name-same");

            Machine? machineB = new Machine();
            machineB.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "not-name-same");

            var romA = new Rom();
            romA.SetName("same-name");
            romA.SetFieldValue(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            romA.CopyMachineInformation(machineA);

            var romB = new Rom();
            romB.SetName("same-name");
            romB.SetFieldValue(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            romB.CopyMachineInformation(machineB);

            var actual = romA.GetDuplicateStatusDB(sourceA, romB, sourceB);
            Assert.Equal(DupeType.External | DupeType.Hash, actual);
        }

        [Fact]
        public void GetDuplicateStatusDB_SameSource_NameMatch_InternalAll()
        {
            Source? sourceA = new Source(0);
            Source? sourceB = new Source(0);

            Machine? machineA = new Machine();
            machineA.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "name-same");

            Machine? machineB = new Machine();
            machineB.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "name-same");

            var romA = new Rom();
            romA.SetName("same-name");
            romA.SetFieldValue(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            romA.CopyMachineInformation(machineA);

            var romB = new Rom();
            romB.SetName("same-name");
            romB.SetFieldValue(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            romB.CopyMachineInformation(machineB);

            var actual = romA.GetDuplicateStatusDB(sourceA, romB, sourceB);
            Assert.Equal(DupeType.Internal | DupeType.All, actual);
        }

        [Fact]
        public void GetDuplicateStatusDB_SameSource_NoNameMatch_InternalHash()
        {
            Source? sourceA = new Source(0);
            Source? sourceB = new Source(0);

            Machine? machineA = new Machine();
            machineA.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "name-same");

            Machine? machineB = new Machine();
            machineB.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "not-name-same");

            var romA = new Rom();
            romA.SetName("same-name");
            romA.SetFieldValue(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            romA.CopyMachineInformation(machineA);

            var romB = new Rom();
            romB.SetName("same-name");
            romB.SetFieldValue(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            romB.CopyMachineInformation(machineB);

            var actual = romA.GetDuplicateStatusDB(sourceA, romB, sourceB);
            Assert.Equal(DupeType.Internal | DupeType.Hash, actual);
        }

        #endregion

        #region GetKey

        [Theory]
        [InlineData(ItemKey.NULL, false, false, "")]
        [InlineData(ItemKey.NULL, false, true, "")]
        [InlineData(ItemKey.NULL, true, false, "")]
        [InlineData(ItemKey.NULL, true, true, "")]
        [InlineData(ItemKey.Machine, false, false, "0000000000-Machine")]
        [InlineData(ItemKey.Machine, false, true, "Machine")]
        [InlineData(ItemKey.Machine, true, false, "0000000000-machine")]
        [InlineData(ItemKey.Machine, true, true, "machine")]
        [InlineData(ItemKey.CRC, false, false, "00000000")]
        [InlineData(ItemKey.CRC, false, true, "00000000")]
        [InlineData(ItemKey.CRC, true, false, "00000000")]
        [InlineData(ItemKey.CRC, true, true, "00000000")]
        [InlineData(ItemKey.MD5, false, false, "d41d8cd98f00b204e9800998ecf8427e")]
        [InlineData(ItemKey.MD5, false, true, "d41d8cd98f00b204e9800998ecf8427e")]
        [InlineData(ItemKey.MD5, true, false, "d41d8cd98f00b204e9800998ecf8427e")]
        [InlineData(ItemKey.MD5, true, true, "d41d8cd98f00b204e9800998ecf8427e")]
        [InlineData(ItemKey.SHA1, false, false, "da39a3ee5e6b4b0d3255bfef95601890afd80709")]
        [InlineData(ItemKey.SHA1, false, true, "da39a3ee5e6b4b0d3255bfef95601890afd80709")]
        [InlineData(ItemKey.SHA1, true, false, "da39a3ee5e6b4b0d3255bfef95601890afd80709")]
        [InlineData(ItemKey.SHA1, true, true, "da39a3ee5e6b4b0d3255bfef95601890afd80709")]
        [InlineData(ItemKey.SHA256, false, false, "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855")]
        [InlineData(ItemKey.SHA256, false, true, "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855")]
        [InlineData(ItemKey.SHA256, true, false, "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855")]
        [InlineData(ItemKey.SHA256, true, true, "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855")]
        [InlineData(ItemKey.SHA384, false, false, "38b060a751ac96384cd9327eb1b1e36a21fdb71114be07434c0cc7bf63f6e1da274edebfe76f65fbd51ad2f14898b95b")]
        [InlineData(ItemKey.SHA384, false, true, "38b060a751ac96384cd9327eb1b1e36a21fdb71114be07434c0cc7bf63f6e1da274edebfe76f65fbd51ad2f14898b95b")]
        [InlineData(ItemKey.SHA384, true, false, "38b060a751ac96384cd9327eb1b1e36a21fdb71114be07434c0cc7bf63f6e1da274edebfe76f65fbd51ad2f14898b95b")]
        [InlineData(ItemKey.SHA384, true, true, "38b060a751ac96384cd9327eb1b1e36a21fdb71114be07434c0cc7bf63f6e1da274edebfe76f65fbd51ad2f14898b95b")]
        [InlineData(ItemKey.SHA512, false, false, "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e")]
        [InlineData(ItemKey.SHA512, false, true, "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e")]
        [InlineData(ItemKey.SHA512, true, false, "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e")]
        [InlineData(ItemKey.SHA512, true, true, "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e")]
        [InlineData(ItemKey.SpamSum, false, false, "3::")]
        [InlineData(ItemKey.SpamSum, false, true, "3::")]
        [InlineData(ItemKey.SpamSum, true, false, "3::")]
        [InlineData(ItemKey.SpamSum, true, true, "3::")]
        public void GetKey_DefaultImplementation(ItemKey bucketedBy, bool lower, bool norename, string expected)
        {
            Machine machine = new Machine();
            machine.SetFieldValue(Models.Metadata.Machine.NameKey, "Machine");

            DatItem datItem = new Blank();
            datItem.SetFieldValue(DatItem.SourceKey, new Source(0));
            datItem.SetFieldValue(DatItem.MachineKey, machine);

            string actual = datItem.GetKey(bucketedBy, lower, norename);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(ItemKey.NULL, false, false, "")]
        [InlineData(ItemKey.NULL, false, true, "")]
        [InlineData(ItemKey.NULL, true, false, "")]
        [InlineData(ItemKey.NULL, true, true, "")]
        [InlineData(ItemKey.Machine, false, false, "0000000000-Machine")]
        [InlineData(ItemKey.Machine, false, true, "Machine")]
        [InlineData(ItemKey.Machine, true, false, "0000000000-machine")]
        [InlineData(ItemKey.Machine, true, true, "machine")]
        [InlineData(ItemKey.CRC, false, false, "DEADBEEF")]
        [InlineData(ItemKey.CRC, false, true, "DEADBEEF")]
        [InlineData(ItemKey.CRC, true, false, "deadbeef")]
        [InlineData(ItemKey.CRC, true, true, "deadbeef")]
        [InlineData(ItemKey.MD5, false, false, "DEADBEEF")]
        [InlineData(ItemKey.MD5, false, true, "DEADBEEF")]
        [InlineData(ItemKey.MD5, true, false, "deadbeef")]
        [InlineData(ItemKey.MD5, true, true, "deadbeef")]
        [InlineData(ItemKey.SHA1, false, false, "DEADBEEF")]
        [InlineData(ItemKey.SHA1, false, true, "DEADBEEF")]
        [InlineData(ItemKey.SHA1, true, false, "deadbeef")]
        [InlineData(ItemKey.SHA1, true, true, "deadbeef")]
        [InlineData(ItemKey.SHA256, false, false, "DEADBEEF")]
        [InlineData(ItemKey.SHA256, false, true, "DEADBEEF")]
        [InlineData(ItemKey.SHA256, true, false, "deadbeef")]
        [InlineData(ItemKey.SHA256, true, true, "deadbeef")]
        [InlineData(ItemKey.SHA384, false, false, "DEADBEEF")]
        [InlineData(ItemKey.SHA384, false, true, "DEADBEEF")]
        [InlineData(ItemKey.SHA384, true, false, "deadbeef")]
        [InlineData(ItemKey.SHA384, true, true, "deadbeef")]
        [InlineData(ItemKey.SHA512, false, false, "DEADBEEF")]
        [InlineData(ItemKey.SHA512, false, true, "DEADBEEF")]
        [InlineData(ItemKey.SHA512, true, false, "deadbeef")]
        [InlineData(ItemKey.SHA512, true, true, "deadbeef")]
        [InlineData(ItemKey.SpamSum, false, false, "BASE64")]
        [InlineData(ItemKey.SpamSum, false, true, "BASE64")]
        [InlineData(ItemKey.SpamSum, true, false, "base64")]
        [InlineData(ItemKey.SpamSum, true, true, "base64")]
        public void GetKey_CustomImplementation(ItemKey bucketedBy, bool lower, bool norename, string expected)
        {
            Machine machine = new Machine();
            machine.SetFieldValue(Models.Metadata.Machine.NameKey, "Machine");

            DatItem datItem = new Rom();
            datItem.SetFieldValue(DatItem.SourceKey, new Source(0));
            datItem.SetFieldValue(DatItem.MachineKey, machine);
            datItem.SetFieldValue(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Rom.MD2Key, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Rom.MD4Key, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Rom.MD5Key, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Rom.SHA1Key, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Rom.SHA256Key, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Rom.SHA384Key, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Rom.SHA512Key, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Rom.SpamSumKey, "BASE64");

            string actual = datItem.GetKey(bucketedBy, lower, norename);
            Assert.Equal(expected, actual);
        }

        #endregion

        // TODO: Change when Machine retrieval gets fixed
        #region GetKeyDB

        [Theory]
        [InlineData(ItemKey.NULL, false, false, "")]
        [InlineData(ItemKey.NULL, false, true, "")]
        [InlineData(ItemKey.NULL, true, false, "")]
        [InlineData(ItemKey.NULL, true, true, "")]
        [InlineData(ItemKey.Machine, false, false, "0000000000-Machine")]
        [InlineData(ItemKey.Machine, false, true, "Machine")]
        [InlineData(ItemKey.Machine, true, false, "0000000000-machine")]
        [InlineData(ItemKey.Machine, true, true, "machine")]
        [InlineData(ItemKey.CRC, false, false, "00000000")]
        [InlineData(ItemKey.CRC, false, true, "00000000")]
        [InlineData(ItemKey.CRC, true, false, "00000000")]
        [InlineData(ItemKey.CRC, true, true, "00000000")]
        [InlineData(ItemKey.MD5, false, false, "d41d8cd98f00b204e9800998ecf8427e")]
        [InlineData(ItemKey.MD5, false, true, "d41d8cd98f00b204e9800998ecf8427e")]
        [InlineData(ItemKey.MD5, true, false, "d41d8cd98f00b204e9800998ecf8427e")]
        [InlineData(ItemKey.MD5, true, true, "d41d8cd98f00b204e9800998ecf8427e")]
        [InlineData(ItemKey.SHA1, false, false, "da39a3ee5e6b4b0d3255bfef95601890afd80709")]
        [InlineData(ItemKey.SHA1, false, true, "da39a3ee5e6b4b0d3255bfef95601890afd80709")]
        [InlineData(ItemKey.SHA1, true, false, "da39a3ee5e6b4b0d3255bfef95601890afd80709")]
        [InlineData(ItemKey.SHA1, true, true, "da39a3ee5e6b4b0d3255bfef95601890afd80709")]
        [InlineData(ItemKey.SHA256, false, false, "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855")]
        [InlineData(ItemKey.SHA256, false, true, "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855")]
        [InlineData(ItemKey.SHA256, true, false, "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855")]
        [InlineData(ItemKey.SHA256, true, true, "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855")]
        [InlineData(ItemKey.SHA384, false, false, "38b060a751ac96384cd9327eb1b1e36a21fdb71114be07434c0cc7bf63f6e1da274edebfe76f65fbd51ad2f14898b95b")]
        [InlineData(ItemKey.SHA384, false, true, "38b060a751ac96384cd9327eb1b1e36a21fdb71114be07434c0cc7bf63f6e1da274edebfe76f65fbd51ad2f14898b95b")]
        [InlineData(ItemKey.SHA384, true, false, "38b060a751ac96384cd9327eb1b1e36a21fdb71114be07434c0cc7bf63f6e1da274edebfe76f65fbd51ad2f14898b95b")]
        [InlineData(ItemKey.SHA384, true, true, "38b060a751ac96384cd9327eb1b1e36a21fdb71114be07434c0cc7bf63f6e1da274edebfe76f65fbd51ad2f14898b95b")]
        [InlineData(ItemKey.SHA512, false, false, "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e")]
        [InlineData(ItemKey.SHA512, false, true, "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e")]
        [InlineData(ItemKey.SHA512, true, false, "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e")]
        [InlineData(ItemKey.SHA512, true, true, "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e")]
        [InlineData(ItemKey.SpamSum, false, false, "3::")]
        [InlineData(ItemKey.SpamSum, false, true, "3::")]
        [InlineData(ItemKey.SpamSum, true, false, "3::")]
        [InlineData(ItemKey.SpamSum, true, true, "3::")]
        public void GetKeyDB_DefaultImplementation(ItemKey bucketedBy, bool lower, bool norename, string expected)
        {
            Source source = new Source(0);

            Machine machine = new Machine();
            machine.SetFieldValue(Models.Metadata.Machine.NameKey, "Machine");

            DatItem datItem = new Blank();
            datItem.SetFieldValue(DatItem.MachineKey, machine);

            string actual = datItem.GetKeyDB(bucketedBy, source, lower, norename);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(ItemKey.NULL, false, false, "")]
        [InlineData(ItemKey.NULL, false, true, "")]
        [InlineData(ItemKey.NULL, true, false, "")]
        [InlineData(ItemKey.NULL, true, true, "")]
        [InlineData(ItemKey.Machine, false, false, "0000000000-Machine")]
        [InlineData(ItemKey.Machine, false, true, "Machine")]
        [InlineData(ItemKey.Machine, true, false, "0000000000-machine")]
        [InlineData(ItemKey.Machine, true, true, "machine")]
        [InlineData(ItemKey.CRC, false, false, "DEADBEEF")]
        [InlineData(ItemKey.CRC, false, true, "DEADBEEF")]
        [InlineData(ItemKey.CRC, true, false, "deadbeef")]
        [InlineData(ItemKey.CRC, true, true, "deadbeef")]
        [InlineData(ItemKey.MD5, false, false, "DEADBEEF")]
        [InlineData(ItemKey.MD5, false, true, "DEADBEEF")]
        [InlineData(ItemKey.MD5, true, false, "deadbeef")]
        [InlineData(ItemKey.MD5, true, true, "deadbeef")]
        [InlineData(ItemKey.SHA1, false, false, "DEADBEEF")]
        [InlineData(ItemKey.SHA1, false, true, "DEADBEEF")]
        [InlineData(ItemKey.SHA1, true, false, "deadbeef")]
        [InlineData(ItemKey.SHA1, true, true, "deadbeef")]
        [InlineData(ItemKey.SHA256, false, false, "DEADBEEF")]
        [InlineData(ItemKey.SHA256, false, true, "DEADBEEF")]
        [InlineData(ItemKey.SHA256, true, false, "deadbeef")]
        [InlineData(ItemKey.SHA256, true, true, "deadbeef")]
        [InlineData(ItemKey.SHA384, false, false, "DEADBEEF")]
        [InlineData(ItemKey.SHA384, false, true, "DEADBEEF")]
        [InlineData(ItemKey.SHA384, true, false, "deadbeef")]
        [InlineData(ItemKey.SHA384, true, true, "deadbeef")]
        [InlineData(ItemKey.SHA512, false, false, "DEADBEEF")]
        [InlineData(ItemKey.SHA512, false, true, "DEADBEEF")]
        [InlineData(ItemKey.SHA512, true, false, "deadbeef")]
        [InlineData(ItemKey.SHA512, true, true, "deadbeef")]
        [InlineData(ItemKey.SpamSum, false, false, "BASE64")]
        [InlineData(ItemKey.SpamSum, false, true, "BASE64")]
        [InlineData(ItemKey.SpamSum, true, false, "base64")]
        [InlineData(ItemKey.SpamSum, true, true, "base64")]
        public void GetKeyDB_CustomImplementation(ItemKey bucketedBy, bool lower, bool norename, string expected)
        {
            Source source = new Source(0);

            Machine machine = new Machine();
            machine.SetFieldValue(Models.Metadata.Machine.NameKey, "Machine");

            DatItem datItem = new Rom();
            datItem.SetFieldValue(DatItem.MachineKey, machine);
            datItem.SetFieldValue(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Rom.MD2Key, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Rom.MD4Key, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Rom.MD5Key, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Rom.SHA1Key, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Rom.SHA256Key, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Rom.SHA384Key, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Rom.SHA512Key, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Rom.SpamSumKey, "BASE64");

            string actual = datItem.GetKeyDB(bucketedBy, source, lower, norename);
            Assert.Equal(expected, actual);
        }

        #endregion

        #region GetName

        [Fact]
        public void GetName_NoNameKey_Null()
        {
            DatItem item = new TestDatItem(nameKey: null);
            item.SetFieldValue(TestDatItemModel.NameKey, "name");

            string? actual = item.GetName();
            Assert.Null(actual);
        }

        [Fact]
        public void GetName_EmptyNameKey_Null()
        {
            DatItem item = new TestDatItem(nameKey: string.Empty);
            item.SetFieldValue(TestDatItemModel.NameKey, "name");

            string? actual = item.GetName();
            Assert.Null(actual);
        }

        [Fact]
        public void GetName_NameKeyNotExists_Null()
        {
            DatItem item = new TestDatItem(nameKey: "INVALID");
            item.SetFieldValue(TestDatItemModel.NameKey, "name");

            string? actual = item.GetName();
            Assert.Null(actual);
        }

        [Fact]
        public void GetName_NameKeyExists_Filled()
        {
            DatItem item = new TestDatItem(nameKey: TestDatItemModel.NameKey);
            item.SetFieldValue(TestDatItemModel.NameKey, "name");

            string? actual = item.GetName();
            Assert.Equal("name", actual);
        }

        #endregion

        #region SetName

        [Fact]
        public void SetName_NoNameKey_Null()
        {
            DatItem item = new TestDatItem(nameKey: null);
            item.SetName("name");

            string? actual = item.GetName();
            Assert.Null(actual);
        }

        [Fact]
        public void SetName_EmptyNameKey_Null()
        {
            DatItem item = new TestDatItem(nameKey: string.Empty);
            item.SetName("name");

            string? actual = item.GetName();
            Assert.Null(actual);
        }

        [Fact]
        public void SetName_NameKeyNonEmpty_Filled()
        {
            DatItem item = new TestDatItem(nameKey: TestDatItemModel.NameKey);
            item.SetName("name");

            string? actual = item.GetName();
            Assert.Equal("name", actual);
        }

        #endregion

        #region Clone

        [Fact]
        public void CloneTest()
        {
            DatItem item = new Sample();
            item.SetName("name");

            object clone = item.Clone();
            Sample? actual = clone as Sample;
            Assert.NotNull(actual);
            Assert.Equal("name", actual.GetName());
        }

        #endregion

        #region GetInternalClone

        [Fact]
        public void GetInternalCloneTest()
        {
            DatItem<TestDatItemModel> item = new TestDatItem();
            item.SetName("name");

            TestDatItemModel actual = item.GetInternalClone();
            Assert.Equal("name", actual[TestDatItemModel.NameKey]);
        }

        #endregion
    }
}