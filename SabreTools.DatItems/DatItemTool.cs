using System.Collections.Generic;
using System.IO;
using SabreTools.Core.Tools;
using SabreTools.DatItems.Formats;
using SabreTools.FileTypes;
using SabreTools.FileTypes.Aaru;
using SabreTools.FileTypes.CHD;
using SabreTools.IO.Extensions;
using SabreTools.IO.Logging;
using SabreTools.Matching.Compare;

namespace SabreTools.DatItems
{
    public static class DatItemTool
    {
        #region Logging

        /// <summary>
        /// Static logger for static methods
        /// </summary>
        private static readonly Logger staticLogger = new();

        #endregion

        #region Creation

        /// <summary>
        /// Create a specific type of DatItem to be used based on a BaseFile
        /// </summary>
        /// <param name="baseFile">BaseFile containing information to be created</param>
        /// <param name="asFile">TreatAsFile representing special format scanning</param>
        /// <returns>DatItem of the specific internal type that corresponds to the inputs</returns>
        public static DatItem? CreateDatItem(BaseFile? baseFile, TreatAsFile asFile = 0x00)
        {
            return baseFile switch
            {
                // Disk
#if NET20 || NET35
                CHDFile when (asFile & TreatAsFile.CHD) == 0 => baseFile.ConvertToDisk(),
#else
                CHDFile when !asFile.HasFlag(TreatAsFile.CHD) => baseFile.ConvertToDisk(),
#endif

                // Media
#if NET20 || NET35
                AaruFormat when (asFile & TreatAsFile.AaruFormat) == 0 => baseFile.ConvertToMedia(),
#else
                AaruFormat when !asFile.HasFlag(TreatAsFile.AaruFormat) => baseFile.ConvertToMedia(),
#endif

                // Rom
                BaseArchive => baseFile.ConvertToRom(),
                Folder => null, // Folders cannot be a DatItem
                BaseFile => baseFile.ConvertToRom(),

                // Miscellaneous
                _ => null,
            };
        }

        #endregion

        #region Conversion

        /// <summary>
        /// Convert a BaseFile value to a Disk
        /// </summary>
        /// <param name="baseFile">BaseFile to convert</param>
        /// <returns>Disk containing original BaseFile information</returns>
        public static Disk ConvertToDisk(this BaseFile baseFile)
        {
            var disk = new Disk();

            disk.SetName(baseFile.Filename);
            if (baseFile is CHDFile chd)
            {
                disk.SetFieldValue<string?>(Models.Metadata.Disk.MD5Key, chd.InternalMD5.ToHexString());
                disk.SetFieldValue<string?>(Models.Metadata.Disk.SHA1Key, chd.InternalSHA1.ToHexString());
            }
            else
            {
                disk.SetFieldValue<string?>(Models.Metadata.Disk.MD5Key, baseFile.MD5.ToHexString());
                disk.SetFieldValue<string?>(Models.Metadata.Disk.SHA1Key, baseFile.SHA1.ToHexString());
            }

            disk.SetFieldValue<DupeType>(DatItem.DupeTypeKey, 0x00);

            return disk;
        }

        /// <summary>
        /// Convert a BaseFile value to a File
        /// </summary>
        /// <param name="baseFile">BaseFile to convert</param>
        /// <returns>File containing original BaseFile information</returns>
        public static Formats.File ConvertToFile(this BaseFile baseFile)
        {
            var file = new Formats.File();

            file.CRC = baseFile.CRC.ToHexString();
            file.MD5 = baseFile.MD5.ToHexString();
            file.SHA1 = baseFile.SHA1.ToHexString();
            file.SHA256 = baseFile.SHA256.ToHexString();

            file.SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.File);
            file.SetFieldValue<DupeType>(DatItem.DupeTypeKey, 0x00);

            return file;
        }

        /// <summary>
        /// Convert a BaseFile value to a Media
        /// </summary>
        /// <param name="baseFile">BaseFile to convert</param>
        /// <returns>Media containing original BaseFile information</returns>
        public static Media ConvertToMedia(this BaseFile baseFile)
        {
            var media = new Media();

            media.SetName(baseFile.Filename);
            if (baseFile is AaruFormat aif)
            {
                media.SetFieldValue<string?>(Models.Metadata.Media.MD5Key, aif.InternalMD5.ToHexString());
                media.SetFieldValue<string?>(Models.Metadata.Media.SHA1Key, aif.InternalSHA1.ToHexString());
                media.SetFieldValue<string?>(Models.Metadata.Media.SHA256Key, aif.InternalSHA256.ToHexString());
                media.SetFieldValue<string?>(Models.Metadata.Media.SpamSumKey, System.Text.Encoding.UTF8.GetString(aif.InternalSpamSum ?? []));
            }
            else
            {
                media.SetFieldValue<string?>(Models.Metadata.Media.MD5Key, baseFile.MD5.ToHexString());
                media.SetFieldValue<string?>(Models.Metadata.Media.SHA1Key, baseFile.SHA1.ToHexString());
                media.SetFieldValue<string?>(Models.Metadata.Media.SHA256Key, baseFile.SHA256.ToHexString());
                media.SetFieldValue<string?>(Models.Metadata.Media.SpamSumKey, System.Text.Encoding.UTF8.GetString(baseFile.SpamSum ?? []));
            }

            media.SetFieldValue<DupeType>(DatItem.DupeTypeKey, 0x00);

            return media;
        }

        /// <summary>
        /// Convert a BaseFile value to a Rom
        /// </summary>
        /// <param name="baseFile">BaseFile to convert</param>
        /// <returns>Rom containing original BaseFile information</returns>
        public static Rom ConvertToRom(this BaseFile baseFile)
        {
            var rom = new Rom();

            rom.SetName(baseFile.Filename);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.DateKey, baseFile.Date);
            rom.SetFieldValue<string?>(Models.Metadata.Rom.CRCKey, baseFile.CRC.ToHexString());
            rom.SetFieldValue<string?>(Models.Metadata.Rom.MD5Key, baseFile.MD5.ToHexString());
            rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA1Key, baseFile.SHA1.ToHexString());
            rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA256Key, baseFile.SHA256.ToHexString());
            rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA384Key, baseFile.SHA384.ToHexString());
            rom.SetFieldValue<string?>(Models.Metadata.Rom.SHA512Key, baseFile.SHA512.ToHexString());
            rom.SetFieldValue<string?>(Models.Metadata.Rom.SizeKey, baseFile.Size.ToString());
            if (baseFile.SpamSum != null)
                rom.SetFieldValue<string?>(Models.Metadata.Rom.SpamSumKey, System.Text.Encoding.UTF8.GetString(baseFile.SpamSum));

            rom.SetFieldValue<DupeType>(DatItem.DupeTypeKey, 0x00);

            return rom;
        }

        /// <summary>
        /// Convert a Disk value to a BaseFile
        /// </summary>
        /// <param name="disk">Disk to convert</param>
        /// <returns>BaseFile containing original Disk information</returns>
        public static BaseFile ConvertToBaseFile(this Disk disk)
        {
            string? machineName = null;
            var machine = disk.GetFieldValue<Machine>(DatItem.MachineKey);
            if (machine != null)
                machineName = machine.GetStringFieldValue(Models.Metadata.Machine.NameKey);

            return new CHDFile()
            {
                Filename = disk.GetName(),
                Parent = machineName,
                MD5 = disk.GetStringFieldValue(Models.Metadata.Disk.MD5Key).FromHexString(),
                InternalMD5 = disk.GetStringFieldValue(Models.Metadata.Disk.MD5Key).FromHexString(),
                SHA1 = disk.GetStringFieldValue(Models.Metadata.Disk.SHA1Key).FromHexString(),
                InternalSHA1 = disk.GetStringFieldValue(Models.Metadata.Disk.SHA1Key).FromHexString(),
            };
        }

        /// <summary>
        /// Convert a File value to a BaseFile
        /// </summary>
        /// <param name="file">File to convert</param>
        /// <returns>BaseFile containing original File information</returns>
        public static BaseFile ConvertToBaseFile(this Formats.File file)
        {
            string? machineName = null;
            var machine = file.GetFieldValue<Machine>(DatItem.MachineKey);
            if (machine != null)
                machineName = machine.GetStringFieldValue(Models.Metadata.Machine.NameKey);

            return new BaseFile()
            {
                Parent = machineName,
                CRC = file.CRC.FromHexString(),
                MD5 = file.MD5.FromHexString(),
                SHA1 = file.SHA1.FromHexString(),
                SHA256 = file.SHA256.FromHexString(),
            };
        }

        /// <summary>
        /// Convert a Media value to a BaseFile
        /// </summary>
        /// <param name="media">Media to convert</param>
        /// <returns>BaseFile containing original Media information</returns>
        public static BaseFile ConvertToBaseFile(this Media media)
        {
            string? machineName = null;
            var machine = media.GetFieldValue<Machine>(DatItem.MachineKey);
            if (machine != null)
                machineName = machine.GetStringFieldValue(Models.Metadata.Machine.NameKey);

            return new AaruFormat()
            {
                Filename = media.GetName(),
                Parent = machineName,
                MD5 = media.GetStringFieldValue(Models.Metadata.Media.MD5Key).FromHexString(),
                InternalMD5 = media.GetStringFieldValue(Models.Metadata.Media.MD5Key).FromHexString(),
                SHA1 = media.GetStringFieldValue(Models.Metadata.Media.SHA1Key).FromHexString(),
                InternalSHA1 = media.GetStringFieldValue(Models.Metadata.Media.SHA1Key).FromHexString(),
                SHA256 = media.GetStringFieldValue(Models.Metadata.Media.SHA256Key).FromHexString(),
                InternalSHA256 = media.GetStringFieldValue(Models.Metadata.Media.SHA256Key).FromHexString(),
                SpamSum = System.Text.Encoding.UTF8.GetBytes(media.GetStringFieldValue(Models.Metadata.Media.SpamSumKey) ?? string.Empty),
                InternalSpamSum = System.Text.Encoding.UTF8.GetBytes(media.GetStringFieldValue(Models.Metadata.Media.SpamSumKey) ?? string.Empty),
            };
        }

        /// <summary>
        /// Convert a Rom value to a BaseFile
        /// </summary>
        /// <param name="rom">Rom to convert</param>
        /// <returns>BaseFile containing original Rom information</returns>
        public static BaseFile ConvertToBaseFile(this Rom rom)
        {
            string? machineName = null;
            var machine = rom.GetFieldValue<Machine>(DatItem.MachineKey);
            if (machine != null)
                machineName = machine.GetStringFieldValue(Models.Metadata.Machine.NameKey);

            string? spamSum = rom.GetStringFieldValue(Models.Metadata.Rom.SpamSumKey);
            return new BaseFile()
            {
                Filename = rom.GetName(),
                Parent = machineName,
                Date = rom.GetStringFieldValue(Models.Metadata.Rom.DateKey),
                Size = NumberHelper.ConvertToInt64(rom.GetStringFieldValue(Models.Metadata.Rom.SizeKey)),
                CRC = rom.GetStringFieldValue(Models.Metadata.Rom.CRCKey).FromHexString(),
                MD5 = rom.GetStringFieldValue(Models.Metadata.Rom.MD5Key).FromHexString(),
                SHA1 = rom.GetStringFieldValue(Models.Metadata.Rom.SHA1Key).FromHexString(),
                SHA256 = rom.GetStringFieldValue(Models.Metadata.Rom.SHA256Key).FromHexString(),
                SHA384 = rom.GetStringFieldValue(Models.Metadata.Rom.SHA384Key).FromHexString(),
                SHA512 = rom.GetStringFieldValue(Models.Metadata.Rom.SHA512Key).FromHexString(),
                SpamSum = spamSum != null ? System.Text.Encoding.UTF8.GetBytes(spamSum) : null,
            };
        }

        #endregion

        #region Sorting and Merging

        /// <summary>
        /// Merge an arbitrary set of DatItems based on the supplied information
        /// </summary>
        /// <param name="infiles">List of File objects representing the roms to be merged</param>
        /// <returns>A List of DatItem objects representing the merged roms</returns>
        public static List<DatItem> Merge(List<DatItem>? infiles)
        {
            // Check for null or blank roms first
            if (infiles == null || infiles.Count == 0)
                return [];

            // Create output list
            List<DatItem> outfiles = [];

            // Then deduplicate them by checking to see if data matches previous saved roms
            int nodumpCount = 0;
            foreach (DatItem item in infiles)
            {
                // If we don't have a Disk, File, Media, or Rom, we skip checking for duplicates
                if (item is not Disk && item is not Formats.File && item is not Media && item is not Rom)
                    continue;

                // If it's a nodump, add and skip
                if (item is Rom rom && rom.GetStringFieldValue(Models.Metadata.Rom.StatusKey).AsEnumValue<ItemStatus>() == ItemStatus.Nodump)
                {
                    outfiles.Add(item);
                    nodumpCount++;
                    continue;
                }
                else if (item is Disk disk && disk.GetStringFieldValue(Models.Metadata.Disk.StatusKey).AsEnumValue<ItemStatus>() == ItemStatus.Nodump)
                {
                    outfiles.Add(item);
                    nodumpCount++;
                    continue;
                }

                // If it's the first non-nodump rom in the list, don't touch it
                if (outfiles.Count == 0 || outfiles.Count == nodumpCount)
                {
                    outfiles.Add(item);
                    continue;
                }

                // Check if the rom is a duplicate
                DupeType dupetype = 0x00;
                DatItem saveditem = new Blank();
                int pos = -1;
                for (int i = 0; i < outfiles.Count; i++)
                {
                    // Get the next item
                    DatItem lastrom = outfiles[i];

                    // Get the duplicate status
                    dupetype = item.GetDuplicateStatus(lastrom);
                    if (dupetype == 0x00)
                        continue;

                    // If it's a duplicate, skip adding it to the output but add any missing information
                    saveditem = lastrom;
                    pos = i;

                    // Disks, File, Media, and Roms have more information to fill
                    if (item is Disk disk && saveditem is Disk savedDisk)
                        savedDisk.FillMissingInformation(disk);
                    else if (item is Formats.File fileItem && saveditem is Formats.File savedFile)
                        savedFile.FillMissingInformation(fileItem);
                    else if (item is Media media && saveditem is Media savedMedia)
                        savedMedia.FillMissingInformation(media);
                    else if (item is Rom romItem && saveditem is Rom savedRom)
                        savedRom.FillMissingInformation(romItem);

                    saveditem.SetFieldValue<DupeType>(DatItem.DupeTypeKey, dupetype);

                    // If the current system has a lower ID than the previous, set the system accordingly
                    if (item.GetFieldValue<Source?>(DatItem.SourceKey)?.Index < saveditem.GetFieldValue<Source?>(DatItem.SourceKey)?.Index)
                    {
                        item.SetFieldValue<Source?>(DatItem.SourceKey, item.GetFieldValue<Source?>(DatItem.SourceKey)!.Clone() as Source);
                        saveditem.CopyMachineInformation(item);
                        saveditem.SetName(item.GetName());
                    }

                    // If the current machine is a child of the new machine, use the new machine instead
                    if (saveditem.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey) == item.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey)
                        || saveditem.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.RomOfKey) == item.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey))
                    {
                        saveditem.CopyMachineInformation(item);
                        saveditem.SetName(item.GetName());
                    }

                    break;
                }

                // If no duplicate is found, add it to the list
                if (dupetype == 0x00 || pos < 0)
                {
                    outfiles.Add(item);
                }
                // Otherwise, if a new rom information is found, add that
                else
                {
                    outfiles.RemoveAt(pos);
                    outfiles.Insert(pos, saveditem);
                }
            }

            // Then return the result
            return outfiles;
        }

        /// <summary>
        /// Resolve name duplicates in an arbitrary set of DatItems based on the supplied information
        /// </summary>
        /// <param name="infiles">List of File objects representing the roms to be merged</param>
        /// <returns>A List of DatItem objects representing the renamed roms</returns>
        public static List<DatItem> ResolveNames(List<DatItem> infiles)
        {
            // Create the output list
            List<DatItem> output = [];

            // First we want to make sure the list is in alphabetical order
            Sort(ref infiles, true);

            // Now we want to loop through and check names
            DatItem? lastItem = null;
            string? lastrenamed = null;
            int lastid = 0;
            for (int i = 0; i < infiles.Count; i++)
            {
                DatItem datItem = infiles[i];

                // If we have the first item, we automatically add it
                if (lastItem == null)
                {
                    output.Add(datItem);
                    lastItem = datItem;
                    continue;
                }

                // Get the last item name, if applicable
                string lastItemName = lastItem.GetName()
                    ?? lastItem.GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>().AsStringValue()
                    ?? string.Empty;

                // Get the current item name, if applicable
                string datItemName = datItem.GetName()
                    ?? datItem.GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>().AsStringValue()
                    ?? string.Empty;

                // If the current item exactly matches the last item, then we don't add it
#if NET20 || NET35
                if ((datItem.GetDuplicateStatus(lastItem) & DupeType.All) != 0)
#else
                if (datItem.GetDuplicateStatus(lastItem).HasFlag(DupeType.All))
#endif
                {
                    staticLogger.Verbose($"Exact duplicate found for '{datItemName}'");
                    continue;
                }

                // If the current name matches the previous name, rename the current item
                else if (datItemName == lastItemName)
                {
                    staticLogger.Verbose($"Name duplicate found for '{datItemName}'");

                    if (datItem is Disk || datItem is Formats.File || datItem is Media || datItem is Rom)
                    {
                        datItemName += GetDuplicateSuffix(datItem);
                        lastrenamed ??= datItemName;
                    }

                    // If we have a conflict with the last renamed item, do the right thing
                    if (datItemName == lastrenamed)
                    {
                        lastrenamed = datItemName;
                        datItemName += (lastid == 0 ? string.Empty : "_" + lastid);
                        lastid++;
                    }
                    // If we have no conflict, then we want to reset the lastrenamed and id
                    else
                    {
                        lastrenamed = null;
                        lastid = 0;
                    }

                    // Set the item name back to the datItem
                    datItem.SetName(datItemName);

                    output.Add(datItem);
                }

                // Otherwise, we say that we have a valid named file
                else
                {
                    output.Add(datItem);
                    lastItem = datItem;
                    lastrenamed = null;
                    lastid = 0;
                }
            }

            // One last sort to make sure this is ordered
            Sort(ref output, true);

            return output;
        }

        /// <summary>
        /// Resolve name duplicates in an arbitrary set of DatItems based on the supplied information
        /// </summary>
        /// <param name="infiles">List of File objects representing the roms to be merged</param>
        /// <returns>A List of DatItem objects representing the renamed roms</returns>
        public static List<KeyValuePair<long, DatItem>> ResolveNamesDB(List<KeyValuePair<long, DatItem>> infiles)
        {
            // Create the output dict
            List<KeyValuePair<long, DatItem>> output = [];

            // First we want to make sure the list is in alphabetical order
            Sort(ref infiles, true);

            // Now we want to loop through and check names
            DatItem? lastItem = null;
            string? lastrenamed = null;
            int lastid = 0;
            foreach (var datItem in infiles)
            {
                // If we have the first item, we automatically add it
                if (lastItem == null)
                {
                    output.Add(datItem);
                    lastItem = datItem.Value;
                    continue;
                }

                // Get the last item name, if applicable
                string lastItemName = lastItem.GetName()
                    ?? lastItem.GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>().AsStringValue()
                    ?? string.Empty;

                // Get the current item name, if applicable
                string datItemName = datItem.Value.GetName()
                    ?? datItem.Value.GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>().AsStringValue()
                    ?? string.Empty;

                // If the current item exactly matches the last item, then we don't add it
#if NET20 || NET35
                if ((datItem.Value.GetDuplicateStatus(lastItem) & DupeType.All) != 0)
#else
                if (datItem.Value.GetDuplicateStatus(lastItem).HasFlag(DupeType.All))
#endif
                {
                    staticLogger.Verbose($"Exact duplicate found for '{datItemName}'");
                    continue;
                }

                // If the current name matches the previous name, rename the current item
                else if (datItemName == lastItemName)
                {
                    staticLogger.Verbose($"Name duplicate found for '{datItemName}'");

                    if (datItem.Value is Disk || datItem.Value is Formats.File || datItem.Value is Media || datItem.Value is Rom)
                    {
                        datItemName += GetDuplicateSuffix(datItem.Value);
                        lastrenamed ??= datItemName;
                    }

                    // If we have a conflict with the last renamed item, do the right thing
                    if (datItemName == lastrenamed)
                    {
                        lastrenamed = datItemName;
                        datItemName += (lastid == 0 ? string.Empty : "_" + lastid);
                        lastid++;
                    }
                    // If we have no conflict, then we want to reset the lastrenamed and id
                    else
                    {
                        lastrenamed = null;
                        lastid = 0;
                    }

                    // Set the item name back to the datItem
                    datItem.Value.SetName(datItemName);
                    output.Add(datItem);
                }

                // Otherwise, we say that we have a valid named file
                else
                {
                    output.Add(datItem);
                    lastItem = datItem.Value;
                    lastrenamed = null;
                    lastid = 0;
                }
            }

            // One last sort to make sure this is ordered
            Sort(ref output, true);

            return output;
        }

        /// <summary>
        /// Get duplicate suffix based on the item type
        /// </summary>
        private static string GetDuplicateSuffix(DatItem datItem)
        {
            return datItem switch
            {
                Disk disk => disk.GetDuplicateSuffix(),
                Formats.File file => file.GetDuplicateSuffix(),
                Media media => media.GetDuplicateSuffix(),
                Rom rom => rom.GetDuplicateSuffix(),
                _ => "_1",
            };
        }

        /// <summary>
        /// Sort a list of File objects by SourceID, Game, and Name (in order)
        /// </summary>
        /// <param name="roms">List of File objects representing the roms to be sorted</param>
        /// <param name="norename">True if files are not renamed, false otherwise</param>
        /// <returns>True if it sorted correctly, false otherwise</returns>
        public static bool Sort(ref List<DatItem> roms, bool norename)
        {
            roms.Sort(delegate (DatItem x, DatItem y)
            {
                try
                {
                    var nc = new NaturalComparer();

                    // If machine names don't match
                    string? xMachineName = x.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey);
                    string? yMachineName = y.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey);
                    if (xMachineName != yMachineName)
                        return nc.Compare(xMachineName, yMachineName);

                    // If types don't match
                    string? xType = x.GetStringFieldValue(Models.Metadata.DatItem.TypeKey);
                    string? yType = y.GetStringFieldValue(Models.Metadata.DatItem.TypeKey);
                    if (xType != yType)
                        return xType.AsEnumValue<ItemType>() - yType.AsEnumValue<ItemType>();

                    // If directory names don't match
                    string? xDirectoryName = Path.GetDirectoryName(TextHelper.RemovePathUnsafeCharacters(x.GetName() ?? string.Empty));
                    string? yDirectoryName = Path.GetDirectoryName(TextHelper.RemovePathUnsafeCharacters(y.GetName() ?? string.Empty));
                    if (xDirectoryName != yDirectoryName)
                        return nc.Compare(xDirectoryName, yDirectoryName);

                    // If item names don't match
                    string? xName = Path.GetFileName(TextHelper.RemovePathUnsafeCharacters(x.GetName() ?? string.Empty));
                    string? yName = Path.GetFileName(TextHelper.RemovePathUnsafeCharacters(y.GetName() ?? string.Empty));
                    if (xName != yName)
                        return nc.Compare(xName, yName);

                    // Otherwise, compare on machine or source, depending on the flag
                    int? xSourceIndex = x.GetFieldValue<Source?>(DatItem.SourceKey)?.Index;
                    int? ySourceIndex = y.GetFieldValue<Source?>(DatItem.SourceKey)?.Index;
                    return (norename ? nc.Compare(xMachineName, yMachineName) : (xSourceIndex - ySourceIndex) ?? 0);
                }
                catch
                {
                    // Absorb the error
                    return 0;
                }
            });

            return true;
        }

        /// <summary>
        /// Sort a list of File objects by SourceID, Game, and Name (in order)
        /// </summary>
        /// <param name="roms">List of File objects representing the roms to be sorted</param>
        /// <param name="norename">True if files are not renamed, false otherwise</param>
        /// <returns>True if it sorted correctly, false otherwise</returns>
        public static bool Sort(ref List<KeyValuePair<long, DatItem>> roms, bool norename)
        {
            roms.Sort(delegate (KeyValuePair<long, DatItem> x, KeyValuePair<long, DatItem> y)
            {
                try
                {
                    var nc = new NaturalComparer();

                    // If machine names don't match
                    string? xMachineName = x.Value.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey);
                    string? yMachineName = y.Value.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey);
                    if (xMachineName != yMachineName)
                        return nc.Compare(xMachineName, yMachineName);

                    // If types don't match
                    string? xType = x.Value.GetStringFieldValue(Models.Metadata.DatItem.TypeKey);
                    string? yType = y.Value.GetStringFieldValue(Models.Metadata.DatItem.TypeKey);
                    if (xType != yType)
                        return xType.AsEnumValue<ItemType>() - yType.AsEnumValue<ItemType>();

                    // If directory names don't match
                    string? xDirectoryName = Path.GetDirectoryName(TextHelper.RemovePathUnsafeCharacters(x.Value.GetName() ?? string.Empty));
                    string? yDirectoryName = Path.GetDirectoryName(TextHelper.RemovePathUnsafeCharacters(y.Value.GetName() ?? string.Empty));
                    if (xDirectoryName != yDirectoryName)
                        return nc.Compare(xDirectoryName, yDirectoryName);

                    // If item names don't match
                    string? xName = Path.GetFileName(TextHelper.RemovePathUnsafeCharacters(x.Value.GetName() ?? string.Empty));
                    string? yName = Path.GetFileName(TextHelper.RemovePathUnsafeCharacters(y.Value.GetName() ?? string.Empty));
                    if (xName != yName)
                        return nc.Compare(xName, yName);

                    // Otherwise, compare on machine or source, depending on the flag
                    int? xSourceIndex = x.Value.GetFieldValue<Source?>(DatItem.SourceKey)?.Index;
                    int? ySourceIndex = y.Value.GetFieldValue<Source?>(DatItem.SourceKey)?.Index;
                    return (norename ? nc.Compare(xMachineName, yMachineName) : (xSourceIndex - ySourceIndex) ?? 0);
                }
                catch
                {
                    // Absorb the error
                    return 0;
                }
            });

            return true;
        }

        #endregion
    }
}