using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.Filtering;
using Xunit;

namespace SabreTools.Test.Filtering
{
    public class CleaningTests
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
            var datItem = CreateDatItem("nam诶", "nam诶-2", "nam诶-3");

            // Run cleaning
            cleaner.CleanDatItem(datItem);

            // Check the fields
            Assert.Equal("nam", datItem.GetName());
            Assert.Equal("nam-2", datItem.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey));
            Assert.Equal("nam-3", datItem.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.DescriptionKey));
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
            var datItem = CreateDatItem("name", "\"ÁБ\"", "ä|/Ж");

            // Run cleaning
            cleaner.CleanDatItem(datItem);

            // Check the fields
            Assert.Equal("name", datItem.GetName());
            Assert.Equal("'AB'", datItem.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey));
            Assert.Equal("ae-Zh", datItem.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.DescriptionKey));
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
            var datItem = CreateDatItem("name", "name-2", "name-3");

            // Run cleaning
            cleaner.CleanDatItem(datItem);

            // Check the fields
            Assert.Equal("name", datItem.GetName());
            Assert.Equal("!", datItem.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey));
            Assert.Equal("!", datItem.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.DescriptionKey));
        }

        [Theory]
        [InlineData(null, "name")]
        [InlineData("", "name")]
        [InlineData("C:\\Normal\\Depth\\Path", "name")]
        [InlineData("C:\\AbnormalFolderLengthPath\\ThatReallyPushesTheLimit\\OfHowLongYou\\ReallyShouldNameThings\\AndItGetsEvenWorse\\TheMoreSubfoldersThatYouTraverse\\BecauseWhyWouldYouStop\\AtSomethingReasonable\\LikeReallyThisIsGettingDumb\\AndIKnowItsJustATest\\ButNotAsMuchAsMe", "nam")]
        public void CleanDatItemTrimTest(string? root, string expected)
        {
            // Setup cleaner
            var cleaner = new Cleaner
            {
                Trim = true,
                Root = root,
            };

            // Setup DatItem
            var datItem = CreateDatItem("name", "name-2", "name-3");

            // Run cleaning
            cleaner.CleanDatItem(datItem);

            // Check the fields
            Assert.Equal(expected, datItem.GetName());
            Assert.Equal("name-2", datItem.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey));
            Assert.Equal("name-3", datItem.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.DescriptionKey));
        }
    
        // TODO: Reenable when there's a reasonable way of doing so
        //[Fact]
        //public void SetOneRomPerGameTest()
        //{
        //    // Setup DatItem
        //    var datItem = CreateDatItem("name", "name-2", "name-3");

        //    // Run cleaning
        //    Cleaner.SetOneRomPerGame(datItem);

        //    // Check the fields
        //    Assert.Equal("name", datItem.GetName());
        //    Assert.Equal("name-2/name", datItem.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey));
        //}

        /// <summary>
        /// Generate a consistent DatItem for testing
        /// </summary>
        private static DatItem CreateDatItem(string name, string machName, string desc)
        {
            var machine = new Machine();
            machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, machName);
            machine.SetFieldValue<string?>(Models.Metadata.Machine.DescriptionKey, desc);

            var rom = new Rom();
            rom.SetName(name);
            rom.SetFieldValue<Machine>(DatItem.MachineKey, machine);

            return rom;
        }
    }
}