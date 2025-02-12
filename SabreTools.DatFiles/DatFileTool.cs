using System.Collections.Generic;
using SabreTools.Core.Tools;
using SabreTools.DatFiles.Formats;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.IO.Logging;

namespace SabreTools.DatFiles
{
    /// <summary>
    /// Helper methods for updating and converting DatFiles
    /// </summary>
    public static class DatFileTool
    {
        #region Logging

        /// <summary>
        /// Logging object
        /// </summary>
        private static readonly Logger _staticLogger = new();

        #endregion

        #region Creation

        /// <summary>
        /// Create a specific type of DatFile to be used based on a format and a base DAT
        /// </summary>
        /// <param name="datFormat">Format of the DAT to be created, default is <see cref="DatFormat.Logiqx"/> </param>
        /// <param name="baseDat">DatFile containing the information to use in specific operations, default is null</param>
        /// <returns>DatFile of the specific internal type that corresponds to the inputs</returns>
        public static DatFile CreateDatFile(DatFormat datFormat = DatFormat.Logiqx, DatFile? baseDat = null)
        {
            return datFormat switch
            {
                DatFormat.ArchiveDotOrg => new ArchiveDotOrg(baseDat),
                DatFormat.AttractMode => new AttractMode(baseDat),
                DatFormat.ClrMamePro => new ClrMamePro(baseDat),
                DatFormat.CSV => new CommaSeparatedValue(baseDat),
                DatFormat.DOSCenter => new DosCenter(baseDat),
                DatFormat.EverdriveSMDB => new EverdriveSMDB(baseDat),
                DatFormat.Listrom => new Listrom(baseDat),
                DatFormat.Listxml => new Listxml(baseDat),
                DatFormat.Logiqx => new Logiqx(baseDat, false),
                DatFormat.LogiqxDeprecated => new Logiqx(baseDat, true),
                DatFormat.MissFile => new Missfile(baseDat),
                DatFormat.OfflineList => new OfflineList(baseDat),
                DatFormat.OpenMSX => new OpenMSX(baseDat),
                DatFormat.RedumpMD2 => new Md2File(baseDat),
                DatFormat.RedumpMD4 => new Md4File(baseDat),
                DatFormat.RedumpMD5 => new Md5File(baseDat),
                DatFormat.RedumpSFV => new SfvFile(baseDat),
                DatFormat.RedumpSHA1 => new Sha1File(baseDat),
                DatFormat.RedumpSHA256 => new Sha256File(baseDat),
                DatFormat.RedumpSHA384 => new Sha384File(baseDat),
                DatFormat.RedumpSHA512 => new Sha512File(baseDat),
                DatFormat.RedumpSpamSum => new SpamSumFile(baseDat),
                DatFormat.RomCenter => new RomCenter(baseDat),
                DatFormat.SabreJSON => new SabreJSON(baseDat),
                DatFormat.SabreXML => new SabreXML(baseDat),
                DatFormat.SoftwareList => new Formats.SoftwareList(baseDat),
                DatFormat.SSV => new SemicolonSeparatedValue(baseDat),
                DatFormat.TSV => new TabSeparatedValue(baseDat),

                // We use new-style Logiqx as a backup for generic DatFile
                _ => new Logiqx(baseDat, false),
            };
        }

        /// <summary>
        /// Create a new DatFile from an existing DatHeader
        /// </summary>
        /// <param name="datHeader">DatHeader to get the values from</param>
        /// <param name="datModifiers">DatModifiers to get the values from</param>
        public static DatFile CreateDatFile(DatHeader datHeader, DatModifiers datModifiers)
        {
            DatFormat format = datHeader.GetFieldValue<DatFormat>(DatHeader.DatFormatKey);
            DatFile datFile = CreateDatFile(format);
            datFile.SetHeader(datHeader);
            datFile.SetModifiers(datModifiers);
            return datFile;
        }

        #endregion

        #region Sorting and Merging

        /// <summary>
        /// Merge an arbitrary set of DatItems based on the supplied information
        /// </summary>
        /// <param name="items">List of DatItem objects representing the items to be merged</param>
        /// <returns>A List of DatItem objects representing the merged items</returns>
        public static List<DatItem> Merge(List<DatItem>? items)
        {
            // Check for null or blank inputs first
            if (items == null || items.Count == 0)
                return [];

            // Create output list
            List<DatItem> output = [];

            // Then deduplicate them by checking to see if data matches previous saved roms
            int nodumpCount = 0;
            foreach (DatItem datItem in items)
            {
                // If we don't have a Disk, File, Media, or Rom, we skip checking for duplicates
                if (datItem is not Disk && datItem is not DatItems.Formats.File && datItem is not Media && datItem is not Rom)
                    continue;

                // If it's a nodump, add and skip
                if (datItem is Rom rom && rom.GetStringFieldValue(Models.Metadata.Rom.StatusKey).AsEnumValue<ItemStatus>() == ItemStatus.Nodump)
                {
                    output.Add(datItem);
                    nodumpCount++;
                    continue;
                }
                else if (datItem is Disk disk && disk.GetStringFieldValue(Models.Metadata.Disk.StatusKey).AsEnumValue<ItemStatus>() == ItemStatus.Nodump)
                {
                    output.Add(datItem);
                    nodumpCount++;
                    continue;
                }

                // If it's the first non-nodump item in the list, don't touch it
                if (output.Count == nodumpCount)
                {
                    output.Add(datItem);
                    continue;
                }

                // Find the index of the first duplicate, if one exists
                int pos = output.FindIndex(lastItem => datItem.GetDuplicateStatus(lastItem) != 0x00);
                if (pos < 0)
                {
                    output.Add(datItem);
                    continue;
                }

                // Get the duplicate item
                DatItem savedItem = output[pos];
                DupeType dupetype = datItem.GetDuplicateStatus(savedItem);

                // Disks, File, Media, and Roms have more information to fill
                if (datItem is Disk diskItem && savedItem is Disk savedDisk)
                    savedDisk.FillMissingInformation(diskItem);
                else if (datItem is DatItems.Formats.File fileItem && savedItem is DatItems.Formats.File savedFile)
                    savedFile.FillMissingInformation(fileItem);
                else if (datItem is Media mediaItem && savedItem is Media savedMedia)
                    savedMedia.FillMissingInformation(mediaItem);
                else if (datItem is Rom romItem && savedItem is Rom savedRom)
                    savedRom.FillMissingInformation(romItem);

                // Set the duplicate type on the saved item
                savedItem.SetFieldValue<DupeType>(DatItem.DupeTypeKey, dupetype);

                // Get the sources associated with the items
                var savedSource = savedItem.GetFieldValue<Source?>(DatItem.SourceKey);
                var itemSource = datItem.GetFieldValue<Source?>(DatItem.SourceKey);

                // Get the machines associated with the items
                var savedMachine = savedItem.GetFieldValue<Machine>(DatItem.MachineKey);
                var itemMachine = datItem.GetFieldValue<Machine>(DatItem.MachineKey);

                // If the current source has a lower ID than the saved, use the saved source
                if (itemSource?.Index < savedSource?.Index)
                {
                    datItem.SetFieldValue<Source?>(DatItem.SourceKey, savedSource.Clone() as Source);
                    savedItem.CopyMachineInformation(datItem);
                    savedItem.SetName(datItem.GetName());
                }

                // If the saved machine is a child of the current machine, use the current machine instead
                if (savedMachine?.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey) == itemMachine?.GetStringFieldValue(Models.Metadata.Machine.NameKey)
                    || savedMachine?.GetStringFieldValue(Models.Metadata.Machine.RomOfKey) == itemMachine?.GetStringFieldValue(Models.Metadata.Machine.NameKey))
                {
                    savedItem.CopyMachineInformation(datItem);
                    savedItem.SetName(datItem.GetName());
                }

                // Replace the original item in the list
                output.RemoveAt(pos);
                output.Insert(pos, savedItem);
            }

            // Then return the result
            return output;
        }

        #endregion
    }
}
