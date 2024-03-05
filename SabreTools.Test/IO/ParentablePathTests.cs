using System;

using SabreTools.IO;
using Xunit;

namespace SabreTools.Test.IO
{
    public class ParentablePathTests
    {
        // TODO: Re-enable test when SabreTools.IO is updated
        //[Theory]
        [InlineData(null, null, false, null)]
        [InlineData(null, null, true, null)]
        [InlineData("", null, false, null)]
        [InlineData("", null, true, null)]
        [InlineData("      ", null, false, null)]
        [InlineData("      ", null, true, null)]
        [InlineData("C:\\Directory\\Filename.ext", null, false, "Filename.ext")]
        [InlineData("C:\\Directory\\Filename.ext", null, true, "Filename.ext")]
        [InlineData("C:\\Directory\\Filename.ext", "C:\\Directory\\Filename.ext", false, "Filename.ext")]
        [InlineData("C:\\Directory\\Filename.ext", "C:\\Directory\\Filename.ext", true, "Filename.ext")]
        [InlineData("C:\\Directory\\SubDir\\Filename.ext", "C:\\Directory", false, "SubDir\\Filename.ext")]
        [InlineData("C:\\Directory\\SubDir\\Filename.ext", "C:\\Directory", true, "SubDir-Filename.ext")]
        public void NormalizedFileNameTest(string current, string parent, bool sanitize, string expected)
        {
            // TODO: Fix SabreTools.IO to trim the paths automatically
            // TODO: Fix SabreTools.IO to normalize more safely
            var path = new ParentablePath(current?.Trim(), parent?.Trim());
            string actual = path.GetNormalizedFileName(sanitize);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, null, null, false, null)]
        [InlineData(null, null, null, true, null)]
        [InlineData("", null, null, false, null)]
        [InlineData("", null, null, true, null)]
        [InlineData("      ", null, null, false, null)]
        [InlineData("      ", null, null, true, null)]
        [InlineData("C:\\Directory\\Filename.ext", null, null, false, null)]
        [InlineData("C:\\Directory\\Filename.ext", null, null, true, "C:\\Directory")]
        [InlineData("C:\\Directory\\Filename.ext", "C:\\Directory\\Filename.ext", null, false, null)]
        [InlineData("C:\\Directory\\Filename.ext", "C:\\Directory\\Filename.ext", null, true, "C:\\Directory")]
        [InlineData("C:\\Directory\\SubDir\\Filename.ext", "C:\\Directory", null, false, null)]
        [InlineData("C:\\Directory\\SubDir\\Filename.ext", "C:\\Directory", null, true, "C:\\Directory\\SubDir")]
        [InlineData(null, null, "D:\\OutputDirectory", false, null)]
        [InlineData(null, null, "D:\\OutputDirectory", true, null)]
        [InlineData("", null, "D:\\OutputDirectory", false, null)]
        [InlineData("", null, "D:\\OutputDirectory", true, null)]
        [InlineData("      ", null, "D:\\OutputDirectory", false, null)]
        [InlineData("      ", null, "D:\\OutputDirectory", true, null)]
        [InlineData("C:\\Directory\\Filename.ext", null, "D:\\OutputDirectory", false, "D:\\OutputDirectory")]
        [InlineData("C:\\Directory\\Filename.ext", null, "D:\\OutputDirectory", true, "C:\\Directory")]
        [InlineData("C:\\Directory\\Filename.ext", "C:\\Directory\\Filename.ext", "D:\\OutputDirectory", false, "D:\\OutputDirectory")]
        [InlineData("C:\\Directory\\Filename.ext", "C:\\Directory\\Filename.ext", "D:\\OutputDirectory", true, "C:\\Directory")]
        [InlineData("C:\\Directory\\SubDir\\Filename.ext", "C:\\Directory", "D:\\OutputDirectory", false, "D:\\OutputDirectory\\SubDir")]
        [InlineData("C:\\Directory\\SubDir\\Filename.ext", "C:\\Directory", "D:\\OutputDirectory", true, "C:\\Directory\\SubDir")]
        [InlineData(null, null, "%cd%", false, null)]
        [InlineData(null, null, "%cd%", true, null)]
        [InlineData("", null, "%cd%", false, null)]
        [InlineData("", null, "%cd%", true, null)]
        [InlineData("      ", null, "%cd%", false, null)]
        [InlineData("      ", null, "%cd%", true, null)]
        [InlineData("C:\\Directory\\Filename.ext", null, "%cd%", false, "%cd%")]
        [InlineData("C:\\Directory\\Filename.ext", null, "%cd%", true, "C:\\Directory")]
        [InlineData("C:\\Directory\\Filename.ext", "C:\\Directory\\Filename.ext", "%cd%", false, "%cd%")]
        [InlineData("C:\\Directory\\Filename.ext", "C:\\Directory\\Filename.ext", "%cd%", true, "C:\\Directory")]
        [InlineData("C:\\Directory\\SubDir\\Filename.ext", "C:\\Directory", "%cd%", false, "%cd%\\Directory\\SubDir")]
        [InlineData("C:\\Directory\\SubDir\\Filename.ext", "C:\\Directory", "%cd%", true, "C:\\Directory\\SubDir")]
        public void GetOutputPathTest(string current, string parent, string outDir, bool inplace, string expected)
        {
            // Hacks because I can't use environment vars as parameters
            if (outDir == "%cd%")
                outDir = Environment.CurrentDirectory.TrimEnd('\\');
            if (expected?.Contains("%cd%") == true)
                expected = expected.Replace("%cd%", Environment.CurrentDirectory.TrimEnd('\\'));

            // TODO: Fix SabreTools.IO to trim the paths automatically
            var path = new ParentablePath(current?.Trim(), parent?.Trim());
            string actual = path.GetOutputPath(outDir, inplace);
            Assert.Equal(expected, actual);
        }
    }
}