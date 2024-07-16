using SabreTools.Core.Tools;
using Xunit;

namespace SabreTools.Test.Core
{
    public class UtiltiesTests
    {
        [Theory]
        [InlineData(null, null)]
        [InlineData("null", null)]
        [InlineData("0b00001", null)]
        [InlineData("12345", 12345L)]
        [InlineData("00000000012345", 12345L)]
        [InlineData("10h", null)]
        [InlineData("0x10", 16L)]
        [InlineData(" 12345 ", 12345L)]
        public void CleanLongTest(string? input, long? expected)
        {
            long? actual = NumberHelper.ConvertToInt64(input);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, 0, null)]
        [InlineData(null, 4, null)]
        [InlineData("123456", 0, null)]
        [InlineData("123456", 4, null)]
        [InlineData("da39a3ee5e6b4b0d3255bfef95601890afd80709", -1, "da39a3ee5e6b4b0d3255bfef95601890afd80709.gz")]
        [InlineData("da39a3ee5e6b4b0d3255bfef95601890afd80709", 0, "da39a3ee5e6b4b0d3255bfef95601890afd80709.gz")]
        [InlineData("da39a3ee5e6b4b0d3255bfef95601890afd80709", 1, "da\\da39a3ee5e6b4b0d3255bfef95601890afd80709.gz")]
        public void GetDepotPathTest(string? hash, int depth, string? expected)
        {
            string? actual = Utilities.GetDepotPath(hash, depth);
            if (System.IO.Path.DirectorySeparatorChar == '/')
                expected = expected?.Replace('\\', '/');

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("no-extension", false)]
        [InlineData("invalid.ext", false)]
        [InlineData("INVALID.EXT", false)]
        [InlineData("valid_extension.dat", true)]
        [InlineData("valid_extension.DAT", true)]
        public void HasValidDatExtensionTest(string? path, bool expected)
        {
            bool actual = Utilities.HasValidDatExtension(path);
            Assert.Equal(expected, actual);
        }
    }
}