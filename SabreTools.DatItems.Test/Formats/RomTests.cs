using SabreTools.DatItems.Formats;
using SabreTools.Hashing;
using Xunit;

namespace SabreTools.DatItems.Test.Formats
{
    public class RomTests
    {
        #region FillMissingInformation

        [Fact]
        public void FillMissingInformation_BothEmpty()
        {
            Rom self = new Rom();
            Rom other = new Rom();

            self.FillMissingInformation(other);

            Assert.Null(self.GetStringFieldValue(Models.Metadata.Rom.CRCKey));
            Assert.Null(self.GetStringFieldValue(Models.Metadata.Rom.MD2Key));
            Assert.Null(self.GetStringFieldValue(Models.Metadata.Rom.MD4Key));
            Assert.Null(self.GetStringFieldValue(Models.Metadata.Rom.MD5Key));
            Assert.Null(self.GetStringFieldValue(Models.Metadata.Rom.SHA1Key));
            Assert.Null(self.GetStringFieldValue(Models.Metadata.Rom.SHA256Key));
            Assert.Null(self.GetStringFieldValue(Models.Metadata.Rom.SHA384Key));
            Assert.Null(self.GetStringFieldValue(Models.Metadata.Rom.SHA512Key));
            Assert.Null(self.GetStringFieldValue(Models.Metadata.Rom.SpamSumKey));
        }

        [Fact]
        public void FillMissingInformation_AllMissing()
        {
            Rom self = new Rom();

            Rom other = new Rom();
            other.SetFieldValue(Models.Metadata.Rom.CRCKey, "XXXXXX");
            other.SetFieldValue(Models.Metadata.Rom.MD2Key, "XXXXXX");
            other.SetFieldValue(Models.Metadata.Rom.MD4Key, "XXXXXX");
            other.SetFieldValue(Models.Metadata.Rom.MD5Key, "XXXXXX");
            other.SetFieldValue(Models.Metadata.Rom.SHA1Key, "XXXXXX");
            other.SetFieldValue(Models.Metadata.Rom.SHA256Key, "XXXXXX");
            other.SetFieldValue(Models.Metadata.Rom.SHA384Key, "XXXXXX");
            other.SetFieldValue(Models.Metadata.Rom.SHA512Key, "XXXXXX");
            other.SetFieldValue(Models.Metadata.Rom.SpamSumKey, "XXXXXX");

            self.FillMissingInformation(other);

            Assert.Equal("XXXXXX", self.GetStringFieldValue(Models.Metadata.Rom.CRCKey));
            Assert.Equal("XXXXXX", self.GetStringFieldValue(Models.Metadata.Rom.MD2Key));
            Assert.Equal("XXXXXX", self.GetStringFieldValue(Models.Metadata.Rom.MD4Key));
            Assert.Equal("XXXXXX", self.GetStringFieldValue(Models.Metadata.Rom.MD5Key));
            Assert.Equal("XXXXXX", self.GetStringFieldValue(Models.Metadata.Rom.SHA1Key));
            Assert.Equal("XXXXXX", self.GetStringFieldValue(Models.Metadata.Rom.SHA256Key));
            Assert.Equal("XXXXXX", self.GetStringFieldValue(Models.Metadata.Rom.SHA384Key));
            Assert.Equal("XXXXXX", self.GetStringFieldValue(Models.Metadata.Rom.SHA512Key));
            Assert.Equal("XXXXXX", self.GetStringFieldValue(Models.Metadata.Rom.SpamSumKey));
        }

        #endregion

        #region GetDuplicateSuffix

        [Fact]
        public void GetDuplicateSuffix_NoHash_Generic()
        {
            Rom self = new Rom();
            string actual = self.GetDuplicateSuffix();
            Assert.Equal("_1", actual);
        }

        [Fact]
        public void GetDuplicateSuffix_CRC()
        {
            string hash = "XXXXXX";
            Rom self = new Rom();
            self.SetFieldValue(Models.Metadata.Rom.CRCKey, hash);

            string actual = self.GetDuplicateSuffix();
            Assert.Equal($"_{hash}", actual);
        }

        [Fact]
        public void GetDuplicateSuffix_MD2()
        {
            string hash = "XXXXXX";
            Rom self = new Rom();
            self.SetFieldValue(Models.Metadata.Rom.MD2Key, hash);

            string actual = self.GetDuplicateSuffix();
            Assert.Equal($"_{hash}", actual);
        }
        
        [Fact]
        public void GetDuplicateSuffix_MD4()
        {
            string hash = "XXXXXX";
            Rom self = new Rom();
            self.SetFieldValue(Models.Metadata.Rom.MD4Key, hash);

            string actual = self.GetDuplicateSuffix();
            Assert.Equal($"_{hash}", actual);
        }

        [Fact]
        public void GetDuplicateSuffix_MD5()
        {
            string hash = "XXXXXX";
            Rom self = new Rom();
            self.SetFieldValue(Models.Metadata.Rom.MD5Key, hash);

            string actual = self.GetDuplicateSuffix();
            Assert.Equal($"_{hash}", actual);
        }

        [Fact]
        public void GetDuplicateSuffix_SHA1()
        {
            string hash = "XXXXXX";
            Rom self = new Rom();
            self.SetFieldValue(Models.Metadata.Rom.SHA1Key, hash);

            string actual = self.GetDuplicateSuffix();
            Assert.Equal($"_{hash}", actual);
        }

        [Fact]
        public void GetDuplicateSuffix_SHA256()
        {
            string hash = "XXXXXX";
            Rom self = new Rom();
            self.SetFieldValue(Models.Metadata.Rom.SHA256Key, hash);

            string actual = self.GetDuplicateSuffix();
            Assert.Equal($"_{hash}", actual);
        }

        [Fact]
        public void GetDuplicateSuffix_SHA384()
        {
            string hash = "XXXXXX";
            Rom self = new Rom();
            self.SetFieldValue(Models.Metadata.Rom.SHA384Key, hash);

            string actual = self.GetDuplicateSuffix();
            Assert.Equal($"_{hash}", actual);
        }

        [Fact]
        public void GetDuplicateSuffix_SHA512()
        {
            string hash = "XXXXXX";
            Rom self = new Rom();
            self.SetFieldValue(Models.Metadata.Rom.SHA512Key, hash);

            string actual = self.GetDuplicateSuffix();
            Assert.Equal($"_{hash}", actual);
        }

        [Fact]
        public void GetDuplicateSuffix_SpamSum()
        {
            string hash = "XXXXXX";
            Rom self = new Rom();
            self.SetFieldValue(Models.Metadata.Rom.SpamSumKey, hash);

            string actual = self.GetDuplicateSuffix();
            Assert.Equal($"_{hash}", actual);
        }

        #endregion

        #region HasHashes

        [Fact]
        public void HasHashes_NoHash_False()
        {
            Rom self = new Rom();
            bool actual = self.HasHashes();
            Assert.False(actual);
        }

        [Fact]
        public void HasHashes_CRC_True()
        {
            Rom self = new Rom();
            self.SetFieldValue(Models.Metadata.Rom.CRCKey, "XXXXXX");
            self.SetFieldValue(Models.Metadata.Rom.MD2Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD4Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD5Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA1Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA256Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA384Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA512Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SpamSumKey, string.Empty);

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        [Fact]
        public void HasHashes_MD2_True()
        {
            Rom self = new Rom();
            self.SetFieldValue(Models.Metadata.Rom.CRCKey, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD2Key, "XXXXXX");
            self.SetFieldValue(Models.Metadata.Rom.MD4Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD5Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA1Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA256Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA384Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA512Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SpamSumKey, string.Empty);

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        [Fact]
        public void HasHashes_MD4_True()
        {
            Rom self = new Rom();
            self.SetFieldValue(Models.Metadata.Rom.CRCKey, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD2Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD4Key, "XXXXXX");
            self.SetFieldValue(Models.Metadata.Rom.MD5Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA1Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA256Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA384Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA512Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SpamSumKey, string.Empty);

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        [Fact]
        public void HasHashes_MD5_True()
        {
            Rom self = new Rom();
            self.SetFieldValue(Models.Metadata.Rom.CRCKey, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD2Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD4Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD5Key, "XXXXXX");
            self.SetFieldValue(Models.Metadata.Rom.SHA1Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA256Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA384Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA512Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SpamSumKey, string.Empty);

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        [Fact]
        public void HasHashes_SHA1_True()
        {
            Rom self = new Rom();
            self.SetFieldValue(Models.Metadata.Rom.CRCKey, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD2Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD4Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD5Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA1Key, "XXXXXX");
            self.SetFieldValue(Models.Metadata.Rom.SHA256Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA384Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA512Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SpamSumKey, string.Empty);

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        [Fact]
        public void HasHashes_SHA256_True()
        {
            Rom self = new Rom();
            self.SetFieldValue(Models.Metadata.Rom.CRCKey, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD2Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD4Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD5Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA1Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA256Key, "XXXXXX");
            self.SetFieldValue(Models.Metadata.Rom.SHA384Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA512Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SpamSumKey, string.Empty);

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        [Fact]
        public void HasHashes_SHA384_True()
        {
            Rom self = new Rom();
            self.SetFieldValue(Models.Metadata.Rom.CRCKey, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD2Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD4Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD5Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA1Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA256Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA384Key, "XXXXXX");
            self.SetFieldValue(Models.Metadata.Rom.SHA512Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SpamSumKey, string.Empty);

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        [Fact]
        public void HasHashes_SHA512_True()
        {
            Rom self = new Rom();
            self.SetFieldValue(Models.Metadata.Rom.CRCKey, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD2Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD4Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD5Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA1Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA256Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA384Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA512Key, "XXXXXX");
            self.SetFieldValue(Models.Metadata.Rom.SpamSumKey, string.Empty);

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        [Fact]
        public void HasHashes_SpamSum_True()
        {
            Rom self = new Rom();
            self.SetFieldValue(Models.Metadata.Rom.CRCKey, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD2Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD4Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD5Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA1Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA256Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA384Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA512Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SpamSumKey, "XXXXXX");

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        [Fact]
        public void HasHashes_All_True()
        {
            Rom self = new Rom();
            self.SetFieldValue(Models.Metadata.Rom.CRCKey, "XXXXXX");
            self.SetFieldValue(Models.Metadata.Rom.MD2Key, "XXXXXX");
            self.SetFieldValue(Models.Metadata.Rom.MD4Key, "XXXXXX");
            self.SetFieldValue(Models.Metadata.Rom.MD5Key, "XXXXXX");
            self.SetFieldValue(Models.Metadata.Rom.SHA1Key, "XXXXXX");
            self.SetFieldValue(Models.Metadata.Rom.SHA256Key, "XXXXXX");
            self.SetFieldValue(Models.Metadata.Rom.SHA384Key, "XXXXXX");
            self.SetFieldValue(Models.Metadata.Rom.SHA512Key, "XXXXXX");
            self.SetFieldValue(Models.Metadata.Rom.SpamSumKey, "XXXXXX");

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        #endregion

        #region HasZeroHash

        [Fact]
        public void HasZeroHash_NoHash_True()
        {
            Rom self = new Rom();
            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_NonZeroHash_False()
        {
            Rom self = new Rom();
            self.SetFieldValue(Models.Metadata.Rom.CRCKey, "XXXXXX");
            self.SetFieldValue(Models.Metadata.Rom.MD5Key, "XXXXXX");
            self.SetFieldValue(Models.Metadata.Rom.SHA1Key, "XXXXXX");
            self.SetFieldValue(Models.Metadata.Rom.SHA256Key, "XXXXXX");
            self.SetFieldValue(Models.Metadata.Rom.SHA384Key, "XXXXXX");
            self.SetFieldValue(Models.Metadata.Rom.SHA512Key, "XXXXXX");
            self.SetFieldValue(Models.Metadata.Rom.SpamSumKey, "XXXXXX");

            bool actual = self.HasZeroHash();
            Assert.False(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroCRC_True()
        {
            Rom self = new Rom();
            self.SetFieldValue(Models.Metadata.Rom.CRCKey, ZeroHash.CRC32Str);
            self.SetFieldValue(Models.Metadata.Rom.MD5Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA1Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA256Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA384Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA512Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SpamSumKey, string.Empty);

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroMD2_True()
        {
            Rom self = new Rom();
            self.SetFieldValue(Models.Metadata.Rom.CRCKey, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD2Key, ZeroHash.GetString(HashType.MD2));
            self.SetFieldValue(Models.Metadata.Rom.MD4Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD5Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA1Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA256Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA384Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA512Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SpamSumKey, string.Empty);

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroMD4_True()
        {
            Rom self = new Rom();
            self.SetFieldValue(Models.Metadata.Rom.CRCKey, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD2Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD4Key, ZeroHash.GetString(HashType.MD4));
            self.SetFieldValue(Models.Metadata.Rom.MD5Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA1Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA256Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA384Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA512Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SpamSumKey, string.Empty);

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroMD5_True()
        {
            Rom self = new Rom();
            self.SetFieldValue(Models.Metadata.Rom.CRCKey, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD2Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD4Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD5Key, ZeroHash.MD5Str);
            self.SetFieldValue(Models.Metadata.Rom.SHA1Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA256Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA384Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA512Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SpamSumKey, string.Empty);

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroSHA1_True()
        {
            Rom self = new Rom();
            self.SetFieldValue(Models.Metadata.Rom.CRCKey, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD2Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD4Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD5Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA1Key, ZeroHash.SHA1Str);
            self.SetFieldValue(Models.Metadata.Rom.SHA256Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA384Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA512Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SpamSumKey, string.Empty);

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroSHA256_True()
        {
            Rom self = new Rom();
            self.SetFieldValue(Models.Metadata.Rom.CRCKey, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD2Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD4Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD5Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA1Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA256Key, ZeroHash.SHA256Str);
            self.SetFieldValue(Models.Metadata.Rom.SHA384Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA512Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SpamSumKey, string.Empty);

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroSHA384_True()
        {
            Rom self = new Rom();
            self.SetFieldValue(Models.Metadata.Rom.CRCKey, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD2Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD4Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD5Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA1Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA256Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA384Key, ZeroHash.SHA384Str);
            self.SetFieldValue(Models.Metadata.Rom.SHA512Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SpamSumKey, string.Empty);

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroSHA512_True()
        {
            Rom self = new Rom();
            self.SetFieldValue(Models.Metadata.Rom.CRCKey, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD2Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD4Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD5Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA1Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA256Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA384Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA512Key, ZeroHash.SHA512Str);
            self.SetFieldValue(Models.Metadata.Rom.SpamSumKey, string.Empty);

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroSpamSum_True()
        {
            Rom self = new Rom();
            self.SetFieldValue(Models.Metadata.Rom.CRCKey, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD2Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD4Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.MD5Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA1Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA256Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA384Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SHA512Key, string.Empty);
            self.SetFieldValue(Models.Metadata.Rom.SpamSumKey, ZeroHash.SpamSumStr);

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroAll_True()
        {
            Rom self = new Rom();
            self.SetFieldValue(Models.Metadata.Rom.CRCKey, ZeroHash.CRC32Str);
            self.SetFieldValue(Models.Metadata.Rom.MD2Key, ZeroHash.GetString(HashType.MD2));
            self.SetFieldValue(Models.Metadata.Rom.MD4Key, ZeroHash.GetString(HashType.MD4));
            self.SetFieldValue(Models.Metadata.Rom.MD5Key, ZeroHash.MD5Str);
            self.SetFieldValue(Models.Metadata.Rom.SHA1Key, ZeroHash.SHA1Str);
            self.SetFieldValue(Models.Metadata.Rom.SHA256Key, ZeroHash.SHA256Str);
            self.SetFieldValue(Models.Metadata.Rom.SHA384Key, ZeroHash.SHA384Str);
            self.SetFieldValue(Models.Metadata.Rom.SHA512Key, ZeroHash.SHA512Str);
            self.SetFieldValue(Models.Metadata.Rom.SpamSumKey, ZeroHash.SpamSumStr);

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
        [InlineData(ItemKey.SpamSum, false, false, "DEADBEEF")]
        [InlineData(ItemKey.SpamSum, false, true, "DEADBEEF")]
        [InlineData(ItemKey.SpamSum, true, false, "deadbeef")]
        [InlineData(ItemKey.SpamSum, true, true, "deadbeef")]
        public void GetKeyTest(ItemKey bucketedBy, bool lower, bool norename, string expected)
        {
            Machine machine = new Machine();
            machine.SetFieldValue(Models.Metadata.Machine.NameKey, "Machine");

            DatItem datItem = new Rom();
            datItem.SetFieldValue(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Rom.MD2Key, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Rom.MD4Key, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Rom.MD5Key, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Rom.SHA1Key, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Rom.SHA256Key, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Rom.SHA384Key, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Rom.SHA512Key, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Rom.SpamSumKey, "DEADBEEF");
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
        [InlineData(ItemKey.SpamSum, false, false, "DEADBEEF")]
        [InlineData(ItemKey.SpamSum, false, true, "DEADBEEF")]
        [InlineData(ItemKey.SpamSum, true, false, "deadbeef")]
        [InlineData(ItemKey.SpamSum, true, true, "deadbeef")]
        public void GetKeyDBTest(ItemKey bucketedBy, bool lower, bool norename, string expected)
        {
            Source source = new Source(0);

            Machine machine = new Machine();
            machine.SetFieldValue(Models.Metadata.Machine.NameKey, "Machine");

            DatItem datItem = new Rom();
            datItem.SetFieldValue(Models.Metadata.Rom.CRCKey, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Rom.MD2Key, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Rom.MD4Key, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Rom.MD5Key, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Rom.SHA1Key, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Rom.SHA256Key, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Rom.SHA384Key, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Rom.SHA512Key, "DEADBEEF");
            datItem.SetFieldValue(Models.Metadata.Rom.SpamSumKey, "DEADBEEF");
            datItem.SetFieldValue(DatItem.MachineKey, machine);

            string actual = datItem.GetKeyDB(bucketedBy, source, lower, norename);
            Assert.Equal(expected, actual);
        }

        #endregion
    }
}