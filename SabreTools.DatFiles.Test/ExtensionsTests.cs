using Xunit;

namespace SabreTools.DatFiles.Test
{
    public class ExtensionsTests
    {
        #region String to Enum

        [Theory]
        [InlineData(null, MergingFlag.None)]
        [InlineData("none", MergingFlag.None)]
        [InlineData("split", MergingFlag.Split)]
        [InlineData("merged", MergingFlag.Merged)]
        [InlineData("nonmerged", MergingFlag.NonMerged)]
        [InlineData("unmerged", MergingFlag.NonMerged)]
        [InlineData("fullmerged", MergingFlag.FullMerged)]
        [InlineData("device", MergingFlag.DeviceNonMerged)]
        [InlineData("devicenonmerged", MergingFlag.DeviceNonMerged)]
        [InlineData("deviceunmerged", MergingFlag.DeviceNonMerged)]
        [InlineData("full", MergingFlag.FullNonMerged)]
        [InlineData("fullnonmerged", MergingFlag.FullNonMerged)]
        [InlineData("fullunmerged", MergingFlag.FullNonMerged)]
        public void AsMergingFlagTest(string? field, MergingFlag expected)
        {
            MergingFlag actual = field.AsMergingFlag();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, NodumpFlag.None)]
        [InlineData("none", NodumpFlag.None)]
        [InlineData("obsolete", NodumpFlag.Obsolete)]
        [InlineData("required", NodumpFlag.Required)]
        [InlineData("ignore", NodumpFlag.Ignore)]
        public void AsNodumpFlagTest(string? field, NodumpFlag expected)
        {
            NodumpFlag actual = field.AsNodumpFlag();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, PackingFlag.None)]
        [InlineData("none", PackingFlag.None)]
        [InlineData("yes", PackingFlag.Zip)]
        [InlineData("zip", PackingFlag.Zip)]
        [InlineData("no", PackingFlag.Unzip)]
        [InlineData("unzip", PackingFlag.Unzip)]
        [InlineData("partial", PackingFlag.Partial)]
        [InlineData("flat", PackingFlag.Flat)]
        [InlineData("fileonly", PackingFlag.FileOnly)]
        public void AsPackingFlagTest(string? field, PackingFlag expected)
        {
            PackingFlag actual = field.AsPackingFlag();
            Assert.Equal(expected, actual);
        }

        #endregion
    }
}