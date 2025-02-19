using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.DatTools;
using Xunit;

namespace SabreTools.Test
{
    public class CleaningTests
    {
        [Fact]
        public void CleanDatItem_Normalize()
        {
            var datItem = CreateDatItem("name");
            var machine = CreateMachine("\"ÁБ\"", "ä|/Ж");

            var cleaner = new Cleaner { Normalize = true };
            cleaner.CleanDatItem(datItem, machine);

            Assert.Equal("name", datItem.GetName());
            Assert.Equal("'AB'", datItem.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey));
            Assert.Equal("ae-Zh", datItem.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.DescriptionKey));
        }

        [Fact]
        public void CleanDatItem_RemoveUnicode()
        {
            var datItem = CreateDatItem("nam诶");
            var machine = CreateMachine("nam诶-2", "nam诶-3");


            var cleaner = new Cleaner { RemoveUnicode = true };
            cleaner.CleanDatItem(datItem, machine);

            Assert.Equal("nam", datItem.GetName());
            Assert.Equal("nam-2", datItem.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey));
            Assert.Equal("nam-3", datItem.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.DescriptionKey));
        }

        [Fact]
        public void CleanDatItem_Single()
        {
            var datItem = CreateDatItem("name");
            var machine = CreateMachine("name-2", "name-3");

            var cleaner = new Cleaner { Single = true };
            cleaner.CleanDatItem(datItem, machine);

            Assert.Equal("name", datItem.GetName());
            Assert.Equal("!", datItem.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey));
            Assert.Equal("!", datItem.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.DescriptionKey));
        }

        [Theory]
        [InlineData(null, "name")]
        [InlineData("", "name")]
        [InlineData("C:\\Normal\\Depth\\Path", "name")]
        [InlineData("C:\\AbnormalFolderLengthPath\\ThatReallyPushesTheLimit\\OfHowLongYou\\ReallyShouldNameThings\\AndItGetsEvenWorse\\TheMoreSubfoldersThatYouTraverse\\BecauseWhyWouldYouStop\\AtSomethingReasonable\\LikeReallyThisIsGettingDumb\\AndIKnowItsJustATest\\ButNotAsMuchAsMe", "nam")]
        public void CleanDatItem_TrimRoot(string? root, string expected)
        {
            var datItem = CreateDatItem("name");
            var machine = CreateMachine("name-2", "name-3");

            var cleaner = new Cleaner
            {
                Trim = true,
                Root = root,
            };
            cleaner.CleanDatItem(datItem, machine);

            Assert.Equal(expected, datItem.GetName());
            Assert.Equal("name-2", datItem.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey));
            Assert.Equal("name-3", datItem.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.DescriptionKey));
        }

        /// <summary>
        /// Generate a consistent DatItem for testing
        /// </summary>
        private static DatItem CreateDatItem(string name)
        {
            var rom = new Rom();

            rom.SetName(name);

            return rom;
        }

        /// <summary>
        /// Generate a consistent Machine for testing
        /// </summary>
        private static Machine CreateMachine(string machName, string desc)
        {
            var machine = new Machine();

            machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, machName);
            machine.SetFieldValue<string?>(Models.Metadata.Machine.DescriptionKey, desc);

            return machine;
        }
    }
}