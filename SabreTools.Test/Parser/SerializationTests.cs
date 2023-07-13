using System;
using Xunit;

namespace SabreTools.Test.Parser
{
    public class SerializationTests
    {
        [Fact]
        public void ArchiveDotOrgDeserializeTest()
        {
            // Open the file for reading
            string filename = System.IO.Path.Combine(Environment.CurrentDirectory, "TestData", "test-archivedotorg-files.xml");

            // Deserialize the file
            var dat = Serialization.ArchiveDotOrg.Deserialize(filename);

            // Validate the values
            Assert.NotNull(dat?.File);
            Assert.Equal(22, dat.File.Length);

            // Validate we're not missing any attributes or elements
            Assert.Null(dat.ADDITIONAL_ATTRIBUTES);
            Assert.Null(dat.ADDITIONAL_ELEMENTS);
            foreach (var file in dat.File)
            {
                Assert.Null(file.ADDITIONAL_ATTRIBUTES);
                Assert.Null(file.ADDITIONAL_ELEMENTS);
            }
        }

        [Fact]
        public void AttractModeDeserializeTest()
        {
            // Open the file for reading
            string filename = System.IO.Path.Combine(Environment.CurrentDirectory, "TestData", "test-attractmode-files.txt");

            // Deserialize the file
            var dat = Serialization.AttractMode.Deserialize(filename);

            // Validate the values
            Assert.NotNull(dat?.Row);
            Assert.Equal(11, dat.Row.Length);

            // Validate we're not missing any attributes or elements
            foreach (var file in dat.Row)
            {
                Assert.Null(file.ADDITIONAL_ELEMENTS);
            }
        }

        [Fact]
        public void ListxmlDeserializeTest()
        {
            // Open the file for reading
            string filename = System.IO.Path.Combine(Environment.CurrentDirectory, "TestData", "test-listxml-files.xml.gz");

            // Deserialize the file
            var dat = Serialization.Listxml.Deserialize(filename);

            // Validate the values
            Assert.NotNull(dat?.Machine);
            Assert.Equal(45861, dat.Machine.Length);

            // Validate we're not missing any attributes or elements
            Assert.Null(dat.ADDITIONAL_ATTRIBUTES);
            Assert.Null(dat.ADDITIONAL_ELEMENTS);
            foreach (var machine in dat.Machine)
            {
                Assert.Null(machine.ADDITIONAL_ATTRIBUTES);
                Assert.Null(machine.ADDITIONAL_ELEMENTS);

                foreach (var biosset in machine.BiosSet ?? Array.Empty<Models.Listxml.BiosSet>())
                {
                    Assert.Null(biosset.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(biosset.ADDITIONAL_ELEMENTS);
                }

                foreach (var rom in machine.Rom ?? Array.Empty<Models.Listxml.Rom>())
                {
                    Assert.Null(rom.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(rom.ADDITIONAL_ELEMENTS);
                }

                foreach (var disk in machine.Disk ?? Array.Empty<Models.Listxml.Disk>())
                {
                    Assert.Null(disk.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(disk.ADDITIONAL_ELEMENTS);
                }

                foreach (var deviceRef in machine.DeviceRef ?? Array.Empty<Models.Listxml.DeviceRef>())
                {
                    Assert.Null(deviceRef.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(deviceRef.ADDITIONAL_ELEMENTS);
                }

                foreach (var sample in machine.Sample ?? Array.Empty<Models.Listxml.Sample>())
                {
                    Assert.Null(sample.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(sample.ADDITIONAL_ELEMENTS);
                }

                foreach (var chip in machine.Chip ?? Array.Empty<Models.Listxml.Chip>())
                {
                    Assert.Null(chip.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(chip.ADDITIONAL_ELEMENTS);
                }

                foreach (var display in machine.Display ?? Array.Empty<Models.Listxml.Display>())
                {
                    Assert.Null(display.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(display.ADDITIONAL_ELEMENTS);
                }

                if (machine.Sound != null)
                {
                    Assert.Null(machine.Sound.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(machine.Sound.ADDITIONAL_ELEMENTS);
                }

                if (machine.Input != null)
                {
                    Assert.Null(machine.Input.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(machine.Input.ADDITIONAL_ELEMENTS);

                    foreach (var control in machine.Input.Control ?? Array.Empty<Models.Listxml.Control>())
                    {
                        Assert.Null(control.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(control.ADDITIONAL_ELEMENTS);
                    }
                }

                foreach (var dipswitch in machine.DipSwitch ?? Array.Empty<Models.Listxml.DipSwitch>())
                {
                    Assert.Null(dipswitch.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(dipswitch.ADDITIONAL_ELEMENTS);

                    if (dipswitch.Condition != null)
                    {
                        Assert.Null(dipswitch.Condition.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(dipswitch.Condition.ADDITIONAL_ELEMENTS);
                    }

                    foreach (var diplocation in dipswitch.DipLocation ?? Array.Empty<Models.Listxml.DipLocation>())
                    {
                        Assert.Null(diplocation.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(diplocation.ADDITIONAL_ELEMENTS);
                    }

                    foreach (var dipvalue in dipswitch.DipValue ?? Array.Empty<Models.Listxml.DipValue>())
                    {
                        Assert.Null(dipvalue.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(dipvalue.ADDITIONAL_ELEMENTS);

                        if (dipvalue.Condition != null)
                        {
                            Assert.Null(dipvalue.Condition.ADDITIONAL_ATTRIBUTES);
                            Assert.Null(dipvalue.Condition.ADDITIONAL_ELEMENTS);
                        }
                    }
                }

                foreach (var configuration in machine.Configuration ?? Array.Empty<Models.Listxml.Configuration>())
                {
                    Assert.Null(configuration.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(configuration.ADDITIONAL_ELEMENTS);

                    if (configuration.Condition != null)
                    {
                        Assert.Null(configuration.Condition.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(configuration.Condition.ADDITIONAL_ELEMENTS);
                    }

                    foreach (var conflocation in configuration.ConfLocation ?? Array.Empty<Models.Listxml.ConfLocation>())
                    {
                        Assert.Null(conflocation.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(conflocation.ADDITIONAL_ELEMENTS);
                    }

                    foreach (var confsetting in configuration.ConfSetting ?? Array.Empty<Models.Listxml.ConfSetting>())
                    {
                        Assert.Null(confsetting.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(confsetting.ADDITIONAL_ELEMENTS);

                        if (confsetting.Condition != null)
                        {
                            Assert.Null(confsetting.Condition.ADDITIONAL_ATTRIBUTES);
                            Assert.Null(confsetting.Condition.ADDITIONAL_ELEMENTS);
                        }
                    }
                }

                foreach (var port in machine.Port ?? Array.Empty<Models.Listxml.Port>())
                {
                    Assert.Null(port.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(port.ADDITIONAL_ELEMENTS);

                    foreach (var analog in port.Analog ?? Array.Empty<Models.Listxml.Analog>())
                    {
                        Assert.Null(analog.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(analog.ADDITIONAL_ELEMENTS);
                    }
                }

                foreach (var adjuster in machine.Adjuster ?? Array.Empty<Models.Listxml.Adjuster>())
                {
                    Assert.Null(adjuster.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(adjuster.ADDITIONAL_ELEMENTS);

                    if (adjuster.Condition != null)
                    {
                        Assert.Null(adjuster.Condition.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(adjuster.Condition.ADDITIONAL_ELEMENTS);
                    }
                }

                if (machine.Driver != null)
                {
                    Assert.Null(machine.Driver.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(machine.Driver.ADDITIONAL_ELEMENTS);
                }

                foreach (var feature in machine.Feature ?? Array.Empty<Models.Listxml.Feature>())
                {
                    Assert.Null(feature.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(feature.ADDITIONAL_ELEMENTS);
                }

                foreach (var device in machine.Device ?? Array.Empty<Models.Listxml.Device>())
                {
                    Assert.Null(device.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(device.ADDITIONAL_ELEMENTS);

                    if (device.Instance != null)
                    {
                        Assert.Null(device.Instance.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(device.Instance.ADDITIONAL_ELEMENTS);
                    }

                    foreach (var extension in device.Extension ?? Array.Empty<Models.Listxml.Extension>())
                    {
                        Assert.Null(extension.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(extension.ADDITIONAL_ELEMENTS);
                    }
                }

                foreach (var slot in machine.Slot ?? Array.Empty<Models.Listxml.Slot>())
                {
                    Assert.Null(slot.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(slot.ADDITIONAL_ELEMENTS);

                    foreach (var slotoption in slot.SlotOption ?? Array.Empty<Models.Listxml.SlotOption>())
                    {
                        Assert.Null(slotoption.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(slotoption.ADDITIONAL_ELEMENTS);
                    }
                }

                foreach (var softwarelist in machine.SoftwareList ?? Array.Empty<Models.Listxml.SoftwareList>())
                {
                    Assert.Null(softwarelist.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(softwarelist.ADDITIONAL_ELEMENTS);
                }

                foreach (var ramoption in machine.RamOption ?? Array.Empty<Models.Listxml.RamOption>())
                {
                    Assert.Null(ramoption.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(ramoption.ADDITIONAL_ELEMENTS);
                }
            }
        }

        [Fact]
        public void OfflineListDeserializeTest()
        {
            // Open the file for reading
            string filename = System.IO.Path.Combine(Environment.CurrentDirectory, "TestData", "test-offlinelist-files.xml");

            // Deserialize the file
            var dat = Serialization.OfflineList.Deserialize(filename);

            // Validate the values
            Assert.NotNull(dat?.Games?.Game);
            Assert.Equal(6750, dat.Games.Game.Length);

            // Validate we're not missing any attributes or elements
            Assert.Null(dat.ADDITIONAL_ATTRIBUTES);
            Assert.Null(dat.ADDITIONAL_ELEMENTS);
            if (dat.Configuration != null)
            {
                var configuration = dat.Configuration;
                Assert.Null(configuration.ADDITIONAL_ATTRIBUTES);
                Assert.Null(configuration.ADDITIONAL_ELEMENTS);

                if (configuration.Infos != null)
                {
                    var infos = configuration.Infos;
                    Assert.Null(infos.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(infos.ADDITIONAL_ELEMENTS);

                    if (infos.Title != null)
                    {
                        var title = infos.Title;
                        Assert.Null(title.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(title.ADDITIONAL_ELEMENTS);
                    }

                    if (infos.Location != null)
                    {
                        var location = infos.Location;
                        Assert.Null(location.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(location.ADDITIONAL_ELEMENTS);
                    }

                    if (infos.Publisher != null)
                    {
                        var publisher = infos.Publisher;
                        Assert.Null(publisher.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(publisher.ADDITIONAL_ELEMENTS);
                    }

                    if (infos.SourceRom != null)
                    {
                        var sourceRom = infos.SourceRom;
                        Assert.Null(sourceRom.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(sourceRom.ADDITIONAL_ELEMENTS);
                    }

                    if (infos.SaveType != null)
                    {
                        var saveType = infos.SaveType;
                        Assert.Null(saveType.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(saveType.ADDITIONAL_ELEMENTS);
                    }

                    if (infos.RomSize != null)
                    {
                        var romSize = infos.RomSize;
                        Assert.Null(romSize.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(romSize.ADDITIONAL_ELEMENTS);
                    }

                    if (infos.ReleaseNumber != null)
                    {
                        var releaseNumber = infos.ReleaseNumber;
                        Assert.Null(releaseNumber.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(releaseNumber.ADDITIONAL_ELEMENTS);
                    }

                    if (infos.LanguageNumber != null)
                    {
                        var languageNumber = infos.LanguageNumber;
                        Assert.Null(languageNumber.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(languageNumber.ADDITIONAL_ELEMENTS);
                    }

                    if (infos.Comment != null)
                    {
                        var comment = infos.Comment;
                        Assert.Null(comment.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(comment.ADDITIONAL_ELEMENTS);
                    }

                    if (infos.RomCRC != null)
                    {
                        var romCRC = infos.RomCRC;
                        Assert.Null(romCRC.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(romCRC.ADDITIONAL_ELEMENTS);
                    }

                    if (infos.Im1CRC != null)
                    {
                        var im1CRC = infos.Im1CRC;
                        Assert.Null(im1CRC.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(im1CRC.ADDITIONAL_ELEMENTS);
                    }

                    if (infos.Im2CRC != null)
                    {
                        var im2CRC = infos.Im2CRC;
                        Assert.Null(im2CRC.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(im2CRC.ADDITIONAL_ELEMENTS);
                    }

                    if (infos.Languages != null)
                    {
                        var languages = infos.Languages;
                        Assert.Null(languages.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(languages.ADDITIONAL_ELEMENTS);
                    }
                }

                if (configuration.CanOpen != null)
                {
                    var canOpen = configuration.CanOpen;
                    Assert.Null(canOpen.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(canOpen.ADDITIONAL_ELEMENTS);
                }

                if (configuration.NewDat != null)
                {
                    var newDat = configuration.NewDat;
                    Assert.Null(newDat.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(newDat.ADDITIONAL_ELEMENTS);

                    if (newDat.DatUrl != null)
                    {
                        var datURL = newDat.DatUrl;
                        Assert.Null(datURL.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(datURL.ADDITIONAL_ELEMENTS);
                    }
                }

                if (configuration.Search != null)
                {
                    var search = configuration.Search;
                    Assert.Null(search.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(search.ADDITIONAL_ELEMENTS);

                    foreach (var to in search.To ?? Array.Empty<Models.OfflineList.To>())
                    {
                        Assert.Null(to.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(to.ADDITIONAL_ELEMENTS);

                        foreach (var find in to.Find ?? Array.Empty<Models.OfflineList.Find>())
                        {
                            Assert.Null(find.ADDITIONAL_ATTRIBUTES);
                            Assert.Null(find.ADDITIONAL_ELEMENTS);
                        }
                    }
                }
            }

            Assert.Null(dat.Games.ADDITIONAL_ATTRIBUTES);
            Assert.Null(dat.Games.ADDITIONAL_ELEMENTS);

            foreach (var game in dat.Games.Game)
            {
                Assert.Null(game.ADDITIONAL_ATTRIBUTES);
                Assert.Null(game.ADDITIONAL_ELEMENTS);

                if (game.Files != null)
                {
                    var files = game.Files;
                    Assert.Null(files.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(files.ADDITIONAL_ELEMENTS);

                    foreach (var romCRC in files.RomCRC ?? Array.Empty<Models.OfflineList.FileRomCRC>())
                    {
                        Assert.Null(romCRC.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(romCRC.ADDITIONAL_ELEMENTS);
                    }
                }
            }

            if (dat.GUI != null)
            {
                var gui = dat.GUI;
                Assert.Null(gui.ADDITIONAL_ATTRIBUTES);
                Assert.Null(gui.ADDITIONAL_ELEMENTS);

                if (gui.Images != null)
                {
                    var images = gui.Images;
                    Assert.Null(images.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(images.ADDITIONAL_ELEMENTS);

                    foreach (var image in images.Image ?? Array.Empty<Models.OfflineList.Image>())
                    {
                        Assert.Null(image.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(image.ADDITIONAL_ELEMENTS);
                    }
                }
            }
        }

        [Fact]
        public void OpenMSXDeserializeTest()
        {
            // Open the file for reading
            string filename = System.IO.Path.Combine(Environment.CurrentDirectory, "TestData", "test-openmsx-files.xml");

            // Deserialize the file
            var dat = Serialization.OpenMSX.Deserialize(filename);

            // Validate the values
            Assert.NotNull(dat);
            Assert.NotNull(dat.Software);
            Assert.Equal(2550, dat.Software.Length);

            // Validate we're not missing any attributes or elements
            Assert.Null(dat.ADDITIONAL_ATTRIBUTES);
            Assert.Null(dat.ADDITIONAL_ELEMENTS);
            foreach (var software in dat.Software)
            {
                Assert.Null(software.ADDITIONAL_ATTRIBUTES);
                Assert.Null(software.ADDITIONAL_ELEMENTS);

                foreach (var dump in software.Dump ?? Array.Empty<Models.OpenMSX.Dump>())
                {
                    Assert.Null(dump.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(dump.ADDITIONAL_ELEMENTS);

                    if (dump.Original != null)
                    {
                        Assert.Null(dump.Original.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(dump.Original.ADDITIONAL_ELEMENTS);
                    }

                    if (dump.Rom != null)
                    {
                        Assert.Null(dump.Rom.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(dump.Rom.ADDITIONAL_ELEMENTS);
                    }

                    if (dump.MegaRom != null)
                    {
                        Assert.Null(dump.MegaRom.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(dump.MegaRom.ADDITIONAL_ELEMENTS);
                    }

                    if (dump.SCCPlusCart != null)
                    {
                        Assert.Null(dump.SCCPlusCart.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(dump.SCCPlusCart.ADDITIONAL_ELEMENTS);
                    }
                }
            }
        }

        [Fact]
        public void SoftwareListDeserializeTest()
        {
            // Open the file for reading
            string filename = System.IO.Path.Combine(Environment.CurrentDirectory, "TestData", "test-softwarelist-files.xml");

            // Deserialize the file
            var dat = Serialization.SoftawreList.Deserialize(filename);

            // Validate the values
            Assert.NotNull(dat);
            Assert.NotNull(dat.Software);
            Assert.Equal(5447, dat.Software.Length);

            // Validate we're not missing any attributes or elements
            Assert.Null(dat.ADDITIONAL_ATTRIBUTES);
            Assert.Null(dat.ADDITIONAL_ELEMENTS);
            foreach (var software in dat.Software)
            {
                Assert.Null(software.ADDITIONAL_ATTRIBUTES);
                Assert.Null(software.ADDITIONAL_ELEMENTS);

                foreach (var info in software.Info ?? Array.Empty<Models.SoftwareList.Info>())
                {
                    Assert.Null(info.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(info.ADDITIONAL_ELEMENTS);
                }

                foreach (var sharedfeat in software.SharedFeat ?? Array.Empty<Models.SoftwareList.SharedFeat>())
                {
                    Assert.Null(sharedfeat.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(sharedfeat.ADDITIONAL_ELEMENTS);
                }

                foreach (var part in software.Part ?? Array.Empty<Models.SoftwareList.Part>())
                {
                    Assert.Null(part.ADDITIONAL_ATTRIBUTES);
                    Assert.Null(part.ADDITIONAL_ELEMENTS);

                    foreach (var feature in part.Feature ?? Array.Empty<Models.SoftwareList.Feature>())
                    {
                        Assert.Null(feature.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(feature.ADDITIONAL_ELEMENTS);
                    }

                    foreach (var dataarea in part.DataArea ?? Array.Empty<Models.SoftwareList.DataArea>())
                    {
                        Assert.Null(dataarea.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(dataarea.ADDITIONAL_ELEMENTS);

                        foreach (var rom in dataarea.Rom ?? Array.Empty<Models.SoftwareList.Rom>())
                        {
                            Assert.Null(rom.ADDITIONAL_ATTRIBUTES);
                            Assert.Null(rom.ADDITIONAL_ELEMENTS);
                        }
                    }

                    foreach (var diskarea in part.DiskArea ?? Array.Empty<Models.SoftwareList.DiskArea>())
                    {
                        Assert.Null(diskarea.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(diskarea.ADDITIONAL_ELEMENTS);

                        foreach (var disk in diskarea.Disk ?? Array.Empty<Models.SoftwareList.Disk>())
                        {
                            Assert.Null(disk.ADDITIONAL_ATTRIBUTES);
                            Assert.Null(disk.ADDITIONAL_ELEMENTS);
                        }
                    }

                    foreach (var dipswitch in part.DipSwitch ?? Array.Empty<Models.SoftwareList.DipSwitch>())
                    {
                        Assert.Null(dipswitch.ADDITIONAL_ATTRIBUTES);
                        Assert.Null(dipswitch.ADDITIONAL_ELEMENTS);

                        foreach (var dipvalue in dipswitch.DipValue ?? Array.Empty<Models.SoftwareList.DipValue>())
                        {
                            Assert.Null(dipvalue.ADDITIONAL_ATTRIBUTES);
                            Assert.Null(dipvalue.ADDITIONAL_ELEMENTS);
                        }
                    }
                }
            }
        }
    }
}