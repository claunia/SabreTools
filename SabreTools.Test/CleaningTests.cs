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
            var datItem = new Rom();
            datItem.SetName("name");

            var machine = new Machine();
            machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "\"ÁБ\"");
            machine.SetFieldValue<string?>(Models.Metadata.Machine.DescriptionKey, "ä|/Ж");

            var cleaner = new Cleaner { Normalize = true };
            cleaner.CleanDatItem(datItem, machine);

            Assert.Equal("name", datItem.GetName());
            Assert.Equal("'AB'", datItem.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey));
            Assert.Equal("ae-Zh", datItem.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.DescriptionKey));
        }

        [Fact]
        public void CleanDatItem_RemoveUnicode()
        {
            var datItem = new Rom();
            datItem.SetName("nam诶");

            var machine = new Machine();
            machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "nam诶-2");
            machine.SetFieldValue<string?>(Models.Metadata.Machine.DescriptionKey, "nam诶-3");

            var cleaner = new Cleaner { RemoveUnicode = true };
            cleaner.CleanDatItem(datItem, machine);

            Assert.Equal("nam", datItem.GetName());
            Assert.Equal("nam-2", datItem.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey));
            Assert.Equal("nam-3", datItem.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.DescriptionKey));
        }

        [Fact]
        public void CleanDatItem_Single()
        {
            var datItem = new Rom();
            datItem.SetName("name");

            var machine = new Machine();
            machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "name-2");
            machine.SetFieldValue<string?>(Models.Metadata.Machine.DescriptionKey, "name-3");

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
            var datItem = new Rom();
            datItem.SetName("name");

            var machine = new Machine();
            machine.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, "name-2");
            machine.SetFieldValue<string?>(Models.Metadata.Machine.DescriptionKey, "name-3");

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
    }
}