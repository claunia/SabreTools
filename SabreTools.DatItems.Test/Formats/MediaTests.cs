using SabreTools.DatItems.Formats;
using SabreTools.Hashing;
using Xunit;

namespace SabreTools.DatItems.Test.Formats
{
    public class MediaTests
    {
        #region ConvertToRom

        [Fact]
        public void ConvertToRomTest()
        {
            Machine machine = new Machine();
            machine.SetFieldValue(Models.Metadata.Machine.NameKey, "XXXXXX");

            Source source = new Source(0, "XXXXXX");

            Media media = new Media();
            media.SetName("XXXXXX");
            media.SetFieldValue(Models.Metadata.Media.MD5Key, "XXXXXX");
            media.SetFieldValue(Models.Metadata.Media.SHA1Key, "XXXXXX");
            media.SetFieldValue(Models.Metadata.Media.SHA256Key, "XXXXXX");
            media.SetFieldValue(Models.Metadata.Media.SpamSumKey, "XXXXXX");
            media.SetFieldValue(DatItem.DupeTypeKey, DupeType.All | DupeType.External);
            media.SetFieldValue(DatItem.MachineKey, machine);
            media.SetFieldValue(DatItem.RemoveKey, (bool?)false);
            media.SetFieldValue(DatItem.SourceKey, source);

            Rom actual = media.ConvertToRom();

            Assert.Equal("XXXXXX.aaruf", actual.GetName());
            Assert.Equal("XXXXXX", actual.GetStringFieldValue(Models.Metadata.Rom.MD5Key));
            Assert.Equal("XXXXXX", actual.GetStringFieldValue(Models.Metadata.Rom.SHA1Key));
            Assert.Equal("XXXXXX", actual.GetStringFieldValue(Models.Metadata.Rom.SHA256Key));
            Assert.Equal("XXXXXX", actual.GetStringFieldValue(Models.Metadata.Rom.SpamSumKey));
            Assert.Equal(DupeType.All | DupeType.External, actual.GetFieldValue<DupeType>(DatItem.DupeTypeKey));

            Machine? actualMachine = actual.GetFieldValue<Machine?>(DatItem.MachineKey);
            Assert.NotNull(actualMachine);
            Assert.Equal("XXXXXX", actualMachine.GetStringFieldValue(Models.Metadata.Machine.NameKey));

            Assert.Equal(false, actual.GetBoolFieldValue(DatItem.RemoveKey));

            Source? actualSource = actual.GetFieldValue<Source?>(DatItem.SourceKey);
            Assert.NotNull(actualSource);
            Assert.Equal(0, actualSource.Index);
            Assert.Equal("XXXXXX", actualSource.Name);
        }

        #endregion

        #region FillMissingInformation

        [Fact]
        public void FillMissingInformation_BothEmpty()
        {
            Media self = new Media();
            Media other = new Media();

            self.FillMissingInformation(other);

            Assert.Null(self.GetStringFieldValue(Models.Metadata.Media.MD5Key));
            Assert.Null(self.GetStringFieldValue(Models.Metadata.Media.SHA1Key));
            Assert.Null(self.GetStringFieldValue(Models.Metadata.Media.SHA256Key));
            Assert.Null(self.GetStringFieldValue(Models.Metadata.Media.SpamSumKey));
        }

        [Fact]
        public void FillMissingInformation_AllMissing()
        {
            Media self = new Media();

            Media other = new Media();
            other.SetFieldValue(Models.Metadata.Media.MD5Key, "XXXXXX");
            other.SetFieldValue(Models.Metadata.Media.SHA1Key, "XXXXXX");
            other.SetFieldValue(Models.Metadata.Media.SHA256Key, "XXXXXX");
            other.SetFieldValue(Models.Metadata.Media.SpamSumKey, "XXXXXX");

            self.FillMissingInformation(other);

            Assert.Equal("XXXXXX", self.GetStringFieldValue(Models.Metadata.Media.MD5Key));
            Assert.Equal("XXXXXX", self.GetStringFieldValue(Models.Metadata.Media.SHA1Key));
            Assert.Equal("XXXXXX", self.GetStringFieldValue(Models.Metadata.Media.SHA256Key));
            Assert.Equal("XXXXXX", self.GetStringFieldValue(Models.Metadata.Media.SpamSumKey));
        }

        #endregion

        #region GetDuplicateSuffix

        [Fact]
        public void GetDuplicateSuffix_NoHash_Generic()
        {
            Media self = new Media();
            string actual = self.GetDuplicateSuffix();
            Assert.Equal("_1", actual);
        }

        [Fact]
        public void GetDuplicateSuffix_MD5()
        {
            string hash = "XXXXXX";
            Media self = new Media();
            self.SetFieldValue(Models.Metadata.Media.MD5Key, hash);

            string actual = self.GetDuplicateSuffix();
            Assert.Equal($"_{hash}", actual);
        }

        [Fact]
        public void GetDuplicateSuffix_SHA1()
        {
            string hash = "XXXXXX";
            Media self = new Media();
            self.SetFieldValue(Models.Metadata.Media.SHA1Key, hash);

            string actual = self.GetDuplicateSuffix();
            Assert.Equal($"_{hash}", actual);
        }

        [Fact]
        public void GetDuplicateSuffix_SHA256()
        {
            string hash = "XXXXXX";
            Media self = new Media();
            self.SetFieldValue(Models.Metadata.Media.SHA256Key, hash);

            string actual = self.GetDuplicateSuffix();
            Assert.Equal($"_{hash}", actual);
        }

        [Fact]
        public void GetDuplicateSuffix_SpamSum()
        {
            string hash = "XXXXXX";
            Media self = new Media();
            self.SetFieldValue(Models.Metadata.Media.SpamSumKey, hash);

            string actual = self.GetDuplicateSuffix();
            Assert.Equal($"_{hash}", actual);
        }

        #endregion

        #region HasHashes

        [Fact]
        public void HasHashes_NoHash_False()
        {
            Media self = new Media();
            bool actual = self.HasHashes();
            Assert.False(actual);
        }

        [Fact]
        public void HasHashes_MD5_True()
        {
            Media self = new Media();
            self.SetFieldValue(Models.Metadata.Media.MD5Key, "XXXXXX");
            self.SetFieldValue(Models.Metadata.Media.SHA1Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Media.SHA256Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Media.SpamSumKey, string.Empty);

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        [Fact]
        public void HasHashes_SHA1_True()
        {
            Media self = new Media();
            self.SetFieldValue(Models.Metadata.Media.MD5Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Media.SHA1Key, "XXXXXX");
            self.SetFieldValue(Models.Metadata.Media.SHA256Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Media.SpamSumKey, string.Empty);

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        [Fact]
        public void HasHashes_SHA256_True()
        {
            Media self = new Media();
            self.SetFieldValue(Models.Metadata.Media.MD5Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Media.SHA1Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Media.SHA256Key, "XXXXXX");
            self.SetFieldValue(Models.Metadata.Media.SpamSumKey, string.Empty);

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        [Fact]
        public void HasHashes_SpamSum_True()
        {
            Media self = new Media();
            self.SetFieldValue(Models.Metadata.Media.MD5Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Media.SHA1Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Media.SHA256Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Media.SpamSumKey, "XXXXXX");

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        [Fact]
        public void HasHashes_All_True()
        {
            Media self = new Media();
            self.SetFieldValue(Models.Metadata.Media.MD5Key, "XXXXXX");
            self.SetFieldValue(Models.Metadata.Media.SHA1Key, "XXXXXX");
            self.SetFieldValue(Models.Metadata.Media.SHA256Key, "XXXXXX");
            self.SetFieldValue(Models.Metadata.Media.SpamSumKey, "XXXXXX");

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        #endregion

        #region HasZeroHash

        [Fact]
        public void HasZeroHash_NoHash_True()
        {
            Media self = new Media();
            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_NonZeroHash_False()
        {
            Media self = new Media();
            self.SetFieldValue(Models.Metadata.Media.MD5Key, "XXXXXX");
            self.SetFieldValue(Models.Metadata.Media.SHA1Key, "XXXXXX");
            self.SetFieldValue(Models.Metadata.Media.SHA256Key, "XXXXXX");
            self.SetFieldValue(Models.Metadata.Media.SpamSumKey, "XXXXXX");

            bool actual = self.HasZeroHash();
            Assert.False(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroMD5_True()
        {
            Media self = new Media();
            self.SetFieldValue(Models.Metadata.Media.MD5Key, ZeroHash.MD5Str);
            self.SetFieldValue(Models.Metadata.Media.SHA1Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Media.SHA256Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Media.SpamSumKey, string.Empty);

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroSHA1_True()
        {
            Media self = new Media();
            self.SetFieldValue(Models.Metadata.Media.MD5Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Media.SHA1Key, ZeroHash.SHA1Str);
            self.SetFieldValue(Models.Metadata.Media.SHA256Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Media.SpamSumKey, string.Empty);

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroSHA256_True()
        {
            Media self = new Media();
            self.SetFieldValue(Models.Metadata.Media.MD5Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Media.SHA1Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Media.SHA256Key, ZeroHash.SHA256Str);
            self.SetFieldValue(Models.Metadata.Media.SpamSumKey, string.Empty);

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroSpamSum_True()
        {
            Media self = new Media();
            self.SetFieldValue(Models.Metadata.Media.MD5Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Media.SHA1Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Media.SHA256Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Media.SpamSumKey, ZeroHash.SpamSumStr);

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroAll_True()
        {
            Media self = new Media();
            self.SetFieldValue(Models.Metadata.Media.MD5Key, ZeroHash.MD5Str);
            self.SetFieldValue(Models.Metadata.Media.SHA1Key, ZeroHash.SHA1Str);
            self.SetFieldValue(Models.Metadata.Media.SHA256Key, ZeroHash.SHA256Str);
            self.SetFieldValue(Models.Metadata.Media.SpamSumKey, ZeroHash.SpamSumStr);

            bool actual = self.HasZeroHash();
            Assert.True(actual);
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
        [InlineData(ItemKey.MD2, false, false, "8350e5a3e24c153df2275c9f80692773")]
        [InlineData(ItemKey.MD2, false, true, "8350e5a3e24c153df2275c9f80692773")]
        [InlineData(ItemKey.MD2, true, false, "8350e5a3e24c153df2275c9f80692773")]
        [InlineData(ItemKey.MD2, true, true, "8350e5a3e24c153df2275c9f80692773")]
        [InlineData(ItemKey.MD4, false, false, "31d6cfe0d16ae931b73c59d7e0c089c0")]
        [InlineData(ItemKey.MD4, false, true, "31d6cfe0d16ae931b73c59d7e0c089c0")]
        [InlineData(ItemKey.MD4, true, false, "31d6cfe0d16ae931b73c59d7e0c089c0")]
        [InlineData(ItemKey.MD4, true, true, "31d6cfe0d16ae931b73c59d7e0c089c0")]
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
        [InlineData(ItemKey.SHA384, false, false, "38b060a751ac96384cd9327eb1b1e36a21fdb71114be07434c0cc7bf63f6e1da274edebfe76f65fbd51ad2f14898b95b")]
        [InlineData(ItemKey.SHA384, false, true, "38b060a751ac96384cd9327eb1b1e36a21fdb71114be07434c0cc7bf63f6e1da274edebfe76f65fbd51ad2f14898b95b")]
        [InlineData(ItemKey.SHA384, true, false, "38b060a751ac96384cd9327eb1b1e36a21fdb71114be07434c0cc7bf63f6e1da274edebfe76f65fbd51ad2f14898b95b")]
        [InlineData(ItemKey.SHA384, true, true, "38b060a751ac96384cd9327eb1b1e36a21fdb71114be07434c0cc7bf63f6e1da274edebfe76f65fbd51ad2f14898b95b")]
        [InlineData(ItemKey.SHA512, false, false, "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e")]
        [InlineData(ItemKey.SHA512, false, true, "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e")]
        [InlineData(ItemKey.SHA512, true, false, "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e")]
        [InlineData(ItemKey.SHA512, true, true, "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e")]
        [InlineData(ItemKey.SpamSum, false, false, "DEADBEEF")]
        [InlineData(ItemKey.SpamSum, false, true, "DEADBEEF")]
        [InlineData(ItemKey.SpamSum, true, false, "deadbeef")]
        [InlineData(ItemKey.SpamSum, true, true, "deadbeef")]
        public void GetKeyTest(ItemKey bucketedBy, bool lower, bool norename, string expected)
        {
            Machine machine = new Machine();
            machine.SetFieldValue(Models.Metadata.Machine.NameKey, "Machine");

            DatItem datItem = new Media();
            datItem.SetFieldValue(Models.Metadata.Media.MD5Key, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Media.SHA1Key, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Media.SHA256Key, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Media.SpamSumKey, "DEADBEEF");
            datItem.SetFieldValue(DatItem.SourceKey, new Source(0));
            datItem.SetFieldValue(DatItem.MachineKey, machine);

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
        [InlineData(ItemKey.MD2, false, false, "8350e5a3e24c153df2275c9f80692773")]
        [InlineData(ItemKey.MD2, false, true, "8350e5a3e24c153df2275c9f80692773")]
        [InlineData(ItemKey.MD2, true, false, "8350e5a3e24c153df2275c9f80692773")]
        [InlineData(ItemKey.MD2, true, true, "8350e5a3e24c153df2275c9f80692773")]
        [InlineData(ItemKey.MD4, false, false, "31d6cfe0d16ae931b73c59d7e0c089c0")]
        [InlineData(ItemKey.MD4, false, true, "31d6cfe0d16ae931b73c59d7e0c089c0")]
        [InlineData(ItemKey.MD4, true, false, "31d6cfe0d16ae931b73c59d7e0c089c0")]
        [InlineData(ItemKey.MD4, true, true, "31d6cfe0d16ae931b73c59d7e0c089c0")]
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
        [InlineData(ItemKey.SHA384, false, false, "38b060a751ac96384cd9327eb1b1e36a21fdb71114be07434c0cc7bf63f6e1da274edebfe76f65fbd51ad2f14898b95b")]
        [InlineData(ItemKey.SHA384, false, true, "38b060a751ac96384cd9327eb1b1e36a21fdb71114be07434c0cc7bf63f6e1da274edebfe76f65fbd51ad2f14898b95b")]
        [InlineData(ItemKey.SHA384, true, false, "38b060a751ac96384cd9327eb1b1e36a21fdb71114be07434c0cc7bf63f6e1da274edebfe76f65fbd51ad2f14898b95b")]
        [InlineData(ItemKey.SHA384, true, true, "38b060a751ac96384cd9327eb1b1e36a21fdb71114be07434c0cc7bf63f6e1da274edebfe76f65fbd51ad2f14898b95b")]
        [InlineData(ItemKey.SHA512, false, false, "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e")]
        [InlineData(ItemKey.SHA512, false, true, "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e")]
        [InlineData(ItemKey.SHA512, true, false, "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e")]
        [InlineData(ItemKey.SHA512, true, true, "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e")]
        [InlineData(ItemKey.SpamSum, false, false, "DEADBEEF")]
        [InlineData(ItemKey.SpamSum, false, true, "DEADBEEF")]
        [InlineData(ItemKey.SpamSum, true, false, "deadbeef")]
        [InlineData(ItemKey.SpamSum, true, true, "deadbeef")]
        public void GetKeyDBTest(ItemKey bucketedBy, bool lower, bool norename, string expected)
        {
            Source source = new Source(0);

            Machine machine = new Machine();
            machine.SetFieldValue(Models.Metadata.Machine.NameKey, "Machine");

            DatItem datItem = new Media();
            datItem.SetFieldValue(Models.Metadata.Media.MD5Key, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Media.SHA1Key, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Media.SHA256Key, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Media.SpamSumKey, "DEADBEEF");
            datItem.SetFieldValue(DatItem.MachineKey, machine);

            string actual = datItem.GetKeyDB(bucketedBy, source, lower, norename);
            Assert.Equal(expected, actual);
        }

        #endregion
    }
}