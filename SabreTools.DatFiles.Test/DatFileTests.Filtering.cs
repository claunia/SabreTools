using SabreTools.DatFiles.Formats;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using Xunit;

namespace SabreTools.DatFiles.Test
{
    public partial class DatFileTests
    {
        #region MachineDescriptionToName

        [Fact]
        public void MachineDescriptionToName_Items()
        {
            Source source = new Source(0, source: null);

            Machine machine = new Machine();
            machine.SetFieldValue(Models.Metadata.Machine.NameKey, "machine");
            machine.SetFieldValue(Models.Metadata.Machine.DescriptionKey, "description");

            DatItem datItem = new Rom();
            datItem.SetFieldValue(DatItem.MachineKey, machine);
            datItem.SetFieldValue(DatItem.SourceKey, source);

            DatFile datFile = new Logiqx(datFile: null, deprecated: false);
            datFile.AddItem(datItem, statsOnly: false);

            datFile.MachineDescriptionToName();

            // The name of the bucket is not expected to change
            DatItem actual = Assert.Single(datFile.GetItemsForBucket("machine"));
            Machine? actualMachine = actual.GetFieldValue<Machine?>(DatItem.MachineKey);
            Assert.NotNull(actualMachine);
            Assert.Equal("description", actualMachine.GetStringFieldValue(Models.Metadata.Machine.NameKey));
            Assert.Equal("description", actualMachine.GetStringFieldValue(Models.Metadata.Machine.DescriptionKey));
        }

        [Fact]
        public void MachineDescriptionToName_ItemsDB()
        {
            Source source = new Source(0, source: null);

            Machine machine = new Machine();
            machine.SetFieldValue(Models.Metadata.Machine.NameKey, "machine");
            machine.SetFieldValue(Models.Metadata.Machine.DescriptionKey, "description");

            DatItem datItem = new Rom();

            DatFile datFile = new Logiqx(datFile: null, deprecated: false);
            long sourceIndex = datFile.AddSourceDB(source);
            long machineIndex = datFile.AddMachineDB(machine);
            _ = datFile.AddItemDB(datItem, machineIndex, sourceIndex, statsOnly: false);

            datFile.MachineDescriptionToName();

            Machine actualMachine = Assert.Single(datFile.GetMachinesDB()).Value;
            Assert.Equal("description", actualMachine.GetStringFieldValue(Models.Metadata.Machine.NameKey));
            Assert.Equal("description", actualMachine.GetStringFieldValue(Models.Metadata.Machine.DescriptionKey));
        }

        #endregion

        #region SetOneRomPerGame

        [Fact]
        public void SetOneRomPerGame_Items()
        {
            Source source = new Source(0, source: null);

            Machine machine = new Machine();
            machine.SetFieldValue(Models.Metadata.Machine.NameKey, "machine");

            DatItem rom = new Rom();
            rom.SetName("rom.bin");
            rom.SetFieldValue(DatItem.MachineKey, machine);
            rom.SetFieldValue(DatItem.SourceKey, source);

            DatItem disk = new Disk();
            disk.SetName("disk");
            disk.SetFieldValue(DatItem.MachineKey, machine);
            disk.SetFieldValue(DatItem.SourceKey, source);

            DatFile datFile = new Logiqx(datFile: null, deprecated: false);
            datFile.AddItem(rom, statsOnly: false);
            datFile.AddItem(disk, statsOnly: false);

            datFile.BucketBy(ItemKey.Machine, DedupeType.None);
            datFile.SetOneRomPerGame();

            var actualDatItems = datFile.GetItemsForBucket("machine");
            Assert.Equal(2, actualDatItems.Count);

            DatItem actualRom = Assert.Single(actualDatItems.FindAll(i => i is Rom));
            Machine? actualRomMachine = actualRom.GetFieldValue<Machine>(DatItem.MachineKey);
            Assert.NotNull(actualRomMachine);
            Assert.Equal("machine/rom", actualRomMachine.GetStringFieldValue(Models.Metadata.Machine.NameKey));

            DatItem actualDisk = Assert.Single(actualDatItems.FindAll(i => i is Disk));
            Machine? actualDiskMachine = actualDisk.GetFieldValue<Machine>(DatItem.MachineKey);
            Assert.NotNull(actualDiskMachine);
            Assert.Equal("machine/disk", actualDiskMachine.GetStringFieldValue(Models.Metadata.Machine.NameKey));
        }

        [Fact]
        public void SetOneRomPerGame_ItemsDB()
        {
            Source source = new Source(0, source: null);

            Machine machine = new Machine();
            machine.SetFieldValue(Models.Metadata.Machine.NameKey, "machine");

            DatItem rom = new Rom();
            rom.SetName("rom.bin");

            DatItem disk = new Disk();
            disk.SetName("disk");

            DatFile datFile = new Logiqx(datFile: null, deprecated: false);
            long sourceIndex = datFile.AddSourceDB(source);
            long machineIndex = datFile.AddMachineDB(machine);
            _ = datFile.AddItemDB(rom, machineIndex, sourceIndex, statsOnly: false);
            _ = datFile.AddItemDB(disk, machineIndex, sourceIndex, statsOnly: false);

            datFile.BucketBy(ItemKey.Machine, DedupeType.None);
            datFile.SetOneRomPerGame();

            var actualDatItems = datFile.GetItemsForBucketDB("machine");
            Assert.Equal(2, actualDatItems.Count);

            var actualRom = Assert.Single(actualDatItems, i => i.Value is Rom);
            var actualRomMachine = datFile.ItemsDB.GetMachineForItem(actualRom.Key);
            Assert.NotNull(actualRomMachine.Value);
            Assert.Equal("machine/rom", actualRomMachine.Value.GetStringFieldValue(Models.Metadata.Machine.NameKey));

            var actualDisk = Assert.Single(actualDatItems, i => i.Value is Disk);
            var actualDiskMachine = datFile.ItemsDB.GetMachineForItem(actualDisk.Key);
            Assert.NotNull(actualDiskMachine.Value);
            Assert.Equal("machine/disk", actualDiskMachine.Value.GetStringFieldValue(Models.Metadata.Machine.NameKey));
        }

        #endregion

        #region SetOneGamePerRegion

        // TODO: Implement SetOneGamePerRegion tests

        #endregion

        #region StripSceneDatesFromItems

        [Fact]
        public void StripSceneDatesFromItems_Items()
        {
            Source source = new Source(0, source: null);

            Machine machine = new Machine();
            machine.SetFieldValue(Models.Metadata.Machine.NameKey, "10.10.10-machine-name");

            DatItem datItem = new Rom();
            datItem.SetFieldValue(DatItem.MachineKey, machine);
            datItem.SetFieldValue(DatItem.SourceKey, source);

            DatFile datFile = new Logiqx(datFile: null, deprecated: false);
            datFile.AddItem(datItem, statsOnly: false);

            datFile.StripSceneDatesFromItems();

            // The name of the bucket is not expected to change
            DatItem actual = Assert.Single(datFile.GetItemsForBucket("10.10.10-machine-name"));
            Machine? actualMachine = actual.GetFieldValue<Machine?>(DatItem.MachineKey);
            Assert.NotNull(actualMachine);
            Assert.Equal("machine-name", actualMachine.GetStringFieldValue(Models.Metadata.Machine.NameKey));
        }

        [Fact]
        public void StripSceneDatesFromItems_ItemsDB()
        {
            Source source = new Source(0, source: null);

            Machine machine = new Machine();
            machine.SetFieldValue(Models.Metadata.Machine.NameKey, "10.10.10-machine-name");

            DatItem datItem = new Rom();

            DatFile datFile = new Logiqx(datFile: null, deprecated: false);
            long sourceIndex = datFile.AddSourceDB(source);
            long machineIndex = datFile.AddMachineDB(machine);
            _ = datFile.AddItemDB(datItem, machineIndex, sourceIndex, statsOnly: false);

            datFile.StripSceneDatesFromItems();

            Machine actualMachine = Assert.Single(datFile.GetMachinesDB()).Value;
            Assert.Equal("machine-name", actualMachine.GetStringFieldValue(Models.Metadata.Machine.NameKey));
        }

        #endregion
    }
}