using System.Collections.Generic;
using SabreTools.DatFiles.Formats;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using Xunit;

namespace SabreTools.DatFiles.Test
{
    public partial class DatFileTests
    {
        #region ExecuteFilters

        // TODO: Add ExecuteFilters tests

        #endregion

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
            Machine machine = new Machine();
            machine.SetFieldValue(Models.Metadata.Machine.NameKey, "machine");
            machine.SetFieldValue(Models.Metadata.Machine.DescriptionKey, "description");

            DatFile datFile = new Logiqx(datFile: null, deprecated: false);
            _ = datFile.AddMachineDB(machine);

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

        [Fact]
        public void SetOneGamePerRegion_Items()
        {
            Machine nowhereMachine = new Machine();
            nowhereMachine.SetFieldValue(Models.Metadata.Machine.NameKey, "machine (Nowhere)");

            Machine worldMachine = new Machine();
            worldMachine.SetFieldValue(Models.Metadata.Machine.NameKey, "machine (World)");
            worldMachine.SetFieldValue(Models.Metadata.Machine.CloneOfKey, "machine (Nowhere)");

            DatItem nowhereRom = new Rom();
            nowhereRom.SetName("rom.bin");
            nowhereRom.SetFieldValue(DatItem.MachineKey, nowhereMachine);

            DatItem worldRom = new Rom();
            worldRom.SetName("rom.nib");
            worldRom.SetFieldValue(DatItem.MachineKey, worldMachine);

            DatFile datFile = new Logiqx(datFile: null, deprecated: false);
            datFile.AddItem(nowhereRom, statsOnly: false);
            datFile.AddItem(worldRom, statsOnly: false);

            List<string> regions = ["World", "Nowhere"];
            datFile.SetOneGamePerRegion(regions);

            Assert.Empty(datFile.GetItemsForBucket("machine (nowhere)"));

            var actualDatItems = datFile.GetItemsForBucket("machine (world)");
            DatItem actualWorldRom = Assert.Single(actualDatItems);
            Machine? actualWorldMachine = actualWorldRom.GetFieldValue<Machine>(DatItem.MachineKey);
            Assert.NotNull(actualWorldMachine);
            Assert.Equal("machine (World)", actualWorldMachine.GetStringFieldValue(Models.Metadata.Machine.NameKey));
        }

        [Fact]
        public void SetOneGamePerRegion_ItemsDB()
        {
            Machine nowhereMachine = new Machine();
            nowhereMachine.SetFieldValue(Models.Metadata.Machine.NameKey, "machine (Nowhere)");

            Machine worldMachine = new Machine();
            worldMachine.SetFieldValue(Models.Metadata.Machine.NameKey, "machine (World)");
            worldMachine.SetFieldValue(Models.Metadata.Machine.CloneOfKey, "machine (Nowhere)");

            DatFile datFile = new Logiqx(datFile: null, deprecated: false);
            _ = datFile.AddMachineDB(nowhereMachine);
            _ = datFile.AddMachineDB(worldMachine);

            List<string> regions = ["World", "Nowhere"];
            datFile.SetOneGamePerRegion(regions);

            var actualWorldMachine = Assert.Single(datFile.GetMachinesDB());
            Assert.NotNull(actualWorldMachine.Value);
            Assert.Equal("machine (World)", actualWorldMachine.Value.GetStringFieldValue(Models.Metadata.Machine.NameKey));
        }

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
            Machine machine = new Machine();
            machine.SetFieldValue(Models.Metadata.Machine.NameKey, "10.10.10-machine-name");

            DatFile datFile = new Logiqx(datFile: null, deprecated: false);
            _ = datFile.AddMachineDB(machine);

            datFile.StripSceneDatesFromItems();

            Machine actualMachine = Assert.Single(datFile.GetMachinesDB()).Value;
            Assert.Equal("machine-name", actualMachine.GetStringFieldValue(Models.Metadata.Machine.NameKey));
        }

        #endregion
    }
}