using SabreTools.IO;
using Xunit;

namespace SabreTools.Test.IO
{
    public class IOExtensionsTests
    {
        [Theory]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData("     ", null)]
        [InlineData("no-extension", null)]
        [InlineData("NO-EXTENSION", null)]
        [InlineData("no-extension.", null)]
        [InlineData("NO-EXTENSION.", null)]
        [InlineData("filename.ext", "ext")]
        [InlineData("FILENAME.EXT", "ext")]
        public void NormalizedExtensionTest(string path, string expected)
        {
            string actual = path.GetNormalizedExtension();
            Assert.Equal(expected, actual);
        }
    }
}