using System.Collections.Generic;
using SabreTools.DatTools;
using Xunit;

namespace SabreTools.Test
{
    public class PopulationTests
    {
        [Fact]
        public void PopulateExclusionsFromList_Null()
        {
            // Setup the list
            List<string>? exclusions = null;

            // Setup the remover
            var remover = new Remover();
            remover.PopulateExclusionsFromList(exclusions);

            // Check the exclusion lists
            Assert.Empty(remover.HeaderFieldNames);
            Assert.Empty(remover.MachineFieldNames);
            Assert.Empty(remover.ItemFieldNames);
        }

        [Fact]
        public void PopulateExclusionsFromList_Empty()
        {
            // Setup the list
            List<string> exclusions = [];

            // Setup the remover
            var remover = new Remover();
            remover.PopulateExclusionsFromList(exclusions);

            // Check the exclusion lists
            Assert.Empty(remover.HeaderFieldNames);
            Assert.Empty(remover.MachineFieldNames);
            Assert.Empty(remover.ItemFieldNames);
        }

        [Fact]
        public void PopulateExclusionsFromList_HeaderField()
        {
            // Setup the list
            List<string> exclusions =
            [
                "header.datname",
            ];

            // Setup the remover
            var remover = new Remover();
            remover.PopulateExclusionsFromList(exclusions);

            // Check the exclusion lists
            Assert.Empty(remover.HeaderFieldNames);
            Assert.Empty(remover.MachineFieldNames);
            Assert.Empty(remover.ItemFieldNames);
        }

        [Fact]
        public void PopulateExclusionsFromList_MachineField()
        {
            // Setup the list
            List<string> exclusions =
            [
                "machine.name",
            ];

            // Setup the remover
            var remover = new Remover();
            remover.PopulateExclusionsFromList(exclusions);

            // Check the exclusion lists
            Assert.Empty(remover.HeaderFieldNames);
            Assert.Single(remover.MachineFieldNames);
            Assert.Empty(remover.ItemFieldNames);
        }

        [Fact]
        public void PopulateExclusionsFromList_ItemField()
        {
            // Setup the list
            List<string> exclusions =
            [
                "item.name",
            ];

            // Setup the remover
            var remover = new Remover();
            remover.PopulateExclusionsFromList(exclusions);

            // Check the exclusion lists
            Assert.Empty(remover.HeaderFieldNames);
            Assert.Empty(remover.MachineFieldNames);
            Assert.Single(remover.ItemFieldNames);
        }
    }
}