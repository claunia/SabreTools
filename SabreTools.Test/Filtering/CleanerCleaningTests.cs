using SabreTools.DatItems;
using SabreTools.Filtering;
using Xunit;

namespace SabreTools.Test.Filtering
{
    public class CleanerCleaningTests
    {
        [Fact]
        public void CleanDatItemRemoveUnicodeTest()
        {
            // Setup cleaner
            var cleaner = new Cleaner
            {
                RemoveUnicode = true,
            };

            // Setup DatItem
            var datItem = new Rom
            {
                Name = "nam诶",
                Machine = new Machine
                {
                    Name = "nam诶-2",
                    Description = "nam诶-3",
                }
            };

            // Run cleaning
            cleaner.CleanDatItem(datItem);

            // Check the fields
            Assert.Equal("nam", datItem.Name);
            Assert.Equal("nam-2", datItem.Machine.Name);
            Assert.Equal("nam-3", datItem.Machine.Description);
        }
    
        [Fact]
        public void CleanDatItemCleanTest()
        {
            // Setup cleaner
            var cleaner = new Cleaner
            {
                Clean = true,
            };

            // Setup DatItem
            var datItem = new Rom
            {
                Name = "name",
                Machine = new Machine
                {
                    Name = "\"ÁБ\"",
                    Description = "ä|/Ж",
                }
            };

            // Run cleaning
            cleaner.CleanDatItem(datItem);

            // Check the fields
            Assert.Equal("name", datItem.Name);
            Assert.Equal("'AB'", datItem.Machine.Name);
            Assert.Equal("ae-Zh", datItem.Machine.Description);
        }
    
        [Fact]
        public void CleanDatItemSingleTest()
        {
            // Setup cleaner
            var cleaner = new Cleaner
            {
                Single = true,
            };

            // Setup DatItem
            var datItem = new Rom
            {
                Name = "name",
                Machine = new Machine
                {
                    Name = "name-2",
                    Description = "name-3",
                }
            };

            // Run cleaning
            cleaner.CleanDatItem(datItem);

            // Check the fields
            Assert.Equal("name", datItem.Name);
            Assert.Equal("!", datItem.Machine.Name);
            Assert.Equal("name-3", datItem.Machine.Description);
        }

        [Theory]
        [InlineData(null, "name")]
        [InlineData("", "name")]
        [InlineData("C:\\Normal\\Depth\\Path", "name")]
        [InlineData("C:\\AbnormalFolderLengthPath\\ThatReallyPushesTheLimit\\OfHowLongYou\\ReallyShouldNameThings\\AndItGetsEvenWorse\\TheMoreSubfoldersThatYouTraverse\\BecauseWhyWouldYouStop\\AtSomethingReasonable\\LikeReallyThisIsGettingDumb\\AndIKnowItsJustATest\\ButNotAsMuchAsMe", "nam")]
        public void CleanDatItemTrimTest(string root, string expected)
        {
            // Setup cleaner
            var cleaner = new Cleaner
            {
                Trim = true,
                Root = root,
            };

            // Setup DatItem
            var datItem = new Rom
            {
                Name = "name",
                Machine = new Machine
                {
                    Name = "name-2",
                    Description = "name-3",
                }
            };

            // Run cleaning
            cleaner.CleanDatItem(datItem);

            // Check the fields
            Assert.Equal(expected, datItem.Name);
            Assert.Equal("name-2", datItem.Machine.Name);
            Assert.Equal("name-3", datItem.Machine.Description);
        }
    }
}