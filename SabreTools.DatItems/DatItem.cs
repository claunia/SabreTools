using System;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatItems.Formats;
using SabreTools.FileTypes;
using SabreTools.Filter;
using SabreTools.Hashing;
using SabreTools.Logging;
using SabreTools.Matching;

namespace SabreTools.DatItems
{
    /// <summary>
    /// Base class for all items included in a set
    /// </summary>
    /// <remarks>
    /// TODO: Can this be made into a `record` type for easier comparison?
    /// </remarks>
    [JsonObject("datitem"), XmlRoot("datitem")]
    [XmlInclude(typeof(Adjuster))]
    [XmlInclude(typeof(Analog))]
    [XmlInclude(typeof(Archive))]
    [XmlInclude(typeof(BiosSet))]
    [XmlInclude(typeof(Blank))]
    [XmlInclude(typeof(Chip))]
    [XmlInclude(typeof(Condition))]
    [XmlInclude(typeof(Configuration))]
    [XmlInclude(typeof(ConfLocation))]
    [XmlInclude(typeof(ConfSetting))]
    [XmlInclude(typeof(Control))]
    [XmlInclude(typeof(DataArea))]
    [XmlInclude(typeof(Device))]
    [XmlInclude(typeof(DeviceReference))]
    [XmlInclude(typeof(DipLocation))]
    [XmlInclude(typeof(DipSwitch))]
    [XmlInclude(typeof(DipValue))]
    [XmlInclude(typeof(Disk))]
    [XmlInclude(typeof(DiskArea))]
    [XmlInclude(typeof(Display))]
    [XmlInclude(typeof(Driver))]
    [XmlInclude(typeof(Extension))]
    [XmlInclude(typeof(Feature))]
    [XmlInclude(typeof(Info))]
    [XmlInclude(typeof(Input))]
    [XmlInclude(typeof(Instance))]
    [XmlInclude(typeof(Media))]
    [XmlInclude(typeof(Part))]
    [XmlInclude(typeof(PartFeature))]
    [XmlInclude(typeof(Port))]
    [XmlInclude(typeof(RamOption))]
    [XmlInclude(typeof(Release))]
    [XmlInclude(typeof(Rom))]
    [XmlInclude(typeof(Sample))]
    [XmlInclude(typeof(SharedFeature))]
    [XmlInclude(typeof(Slot))]
    [XmlInclude(typeof(SlotOption))]
    [XmlInclude(typeof(SoftwareList))]
    [XmlInclude(typeof(Sound))]
    public abstract class DatItem : IEquatable<DatItem>, IComparable<DatItem>, ICloneable
    {
        #region Constants

        /// <summary>
        /// Duplicate type when compared to another item
        /// </summary>
        public const string DupeTypeKey = "DUPETYPE";

        /// <summary>
        /// Machine associated with the item
        /// </summary>
        public const string MachineKey = "MACHINE";

        /// <summary>
        /// Flag if item should be removed
        /// </summary>
        public const string RemoveKey = "REMOVE";

        /// <summary>
        /// Source information
        /// </summary>
        public const string SourceKey = "SOURCE";

        #endregion

        #region Fields

        /// <summary>
        /// Internal model wrapped by this DatItem
        /// </summary>
        [JsonIgnore, XmlIgnore]
        protected Models.Metadata.DatItem _internal;

        #endregion

        #region Logging

        /// <summary>
        /// Static logger for static methods
        /// </summary>
        [JsonIgnore, XmlIgnore]
        protected static readonly Logger staticLogger = new();

        #endregion

        #region Instance Methods

        #region Accessors

        /// <summary>
        /// Get the value from a field based on the type provided
        /// </summary>
        /// <typeparam name="T">Type of the value to get from the internal model</typeparam>
        /// <param name="fieldName">Field to retrieve</param>
        /// <returns>Value from the field, if possible</returns>
        public T? GetFieldValue<T>(string? fieldName)
        {
            // Invalid field cannot be processed
            if (string.IsNullOrEmpty(fieldName))
                return default;

            // Get the value based on the type
            return _internal.Read<T>(fieldName!);
        }

        /// <summary>
        /// Set the value from a field based on the type provided
        /// </summary>
        /// <typeparam name="T">Type of the value to set in the internal model</typeparam>
        /// <param name="fieldName">Field to set</param>
        /// <param name="value">Value to set</param>
        /// <returns>True if the value was set, false otherwise</returns>
        public bool SetFieldValue<T>(string? fieldName, T? value)
        {
            // Invalid field cannot be processed
            if (string.IsNullOrEmpty(fieldName))
                return false;

            // Set the value based on the type
            _internal[fieldName!] = value;
            return true;
        }

        /// <summary>
        /// Gets the name to use for a DatItem
        /// </summary>
        /// <returns>Name if available, null otherwise</returns>
        public virtual string? GetName() => null;

        /// <summary>
        /// Sets the name to use for a DatItem
        /// </summary>
        /// <param name="name">Name to set for the item</param>
        public virtual void SetName(string? name) { }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public DatItem()
        {
            _internal = new Models.Metadata.Blank();
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        /// <summary>
        /// Create a specific type of DatItem to be used based on a BaseFile
        /// </summary>
        /// <param name="baseFile">BaseFile containing information to be created</param>
        /// <returns>DatItem of the specific internal type that corresponds to the inputs</returns>
        public static DatItem? Create(BaseFile? baseFile)
        {
            return baseFile?.Type switch
            {
                FileType.AaruFormat => new Media(baseFile),
                FileType.CHD => new Disk(baseFile),
                FileType.GZipArchive => new Rom(baseFile),
                FileType.LRZipArchive => new Rom(baseFile),
                FileType.LZ4Archive => new Rom(baseFile),
                FileType.None => new Rom(baseFile),
                FileType.RarArchive => new Rom(baseFile),
                FileType.SevenZipArchive => new Rom(baseFile),
                FileType.TapeArchive => new Rom(baseFile),
                FileType.XZArchive => new Rom(baseFile),
                FileType.ZipArchive => new Rom(baseFile),
                FileType.ZPAQArchive => new Rom(baseFile),
                FileType.ZstdArchive => new Rom(baseFile),
                _ => null,
            };
        }

        #endregion

        #region Cloning Methods

        /// <summary>
        /// Clone the DatItem
        /// </summary>
        /// <returns>Clone of the DatItem</returns>
        public abstract object Clone();

        /// <summary>
        /// Copy all machine information over in one shot
        /// </summary>
        /// <param name="item">Existing item to copy information from</param>
        public void CopyMachineInformation(DatItem item)
        {
            if (item?.GetFieldValue<Machine>(DatItem.MachineKey) == null)
                return;

            if (item.GetFieldValue<Machine>(DatItem.MachineKey)!.Clone() is Machine cloned)
                SetFieldValue<Machine>(DatItem.MachineKey, cloned);
        }

        /// <summary>
        /// Copy all machine information over in one shot
        /// </summary>
        /// <param name="machine">Existing machine to copy information from</param>
        public void CopyMachineInformation(Machine? machine)
        {
            if (machine == null)
                return;

            if (machine.Clone() is Machine cloned)
                SetFieldValue<Machine>(DatItem.MachineKey, cloned);
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public int CompareTo(DatItem? other)
        {
            try
            {
                if (GetName() == other?.GetName())
                    return Equals(other) ? 0 : 1;

                return string.Compare(GetName(), other?.GetName());
            }
            catch
            {
                return 1;
            }
        }

        /// <summary>
        /// Determine if an item is a duplicate using partial matching logic
        /// </summary>
        /// <param name="other">DatItem to use as a baseline</param>
        /// <returns>True if the items are duplicates, false otherwise</returns>
        public virtual bool Equals(DatItem? other)
        {
            // If we don't have a matched type, return false
            if (GetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey) != other?.GetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey))
                return false;

            // Compare the internal models
            return _internal.EqualTo(other._internal);
        }

        /// <summary>
        /// Return the duplicate status of two items
        /// </summary>
        /// <param name="lastItem">DatItem to check against</param>
        /// <returns>The DupeType corresponding to the relationship between the two</returns>
        public DupeType GetDuplicateStatus(DatItem? lastItem)
        {
            DupeType output = 0x00;

            // If we don't have a duplicate at all, return none
            if (!Equals(lastItem))
                return output;

            // If the duplicate is external already or should be, set it
#if NETFRAMEWORK
            if ((lastItem.GetFieldValue<DupeType>(DatItem.DupeTypeKey) & DupeType.External) != 0 || lastItem?.GetFieldValue<Source?>(DatItem.SourceKey)?.Index != GetFieldValue<Source?>(DatItem.SourceKey)?.Index)
#else
            if (lastItem.GetFieldValue<DupeType>(DatItem.DupeTypeKey).HasFlag(DupeType.External) || lastItem?.GetFieldValue<Source?>(DatItem.SourceKey)?.Index != GetFieldValue<Source?>(DatItem.SourceKey)?.Index)
#endif
            {
                if (lastItem?.GetFieldValue<Machine>(DatItem.MachineKey)?.GetFieldValue<string?>(Models.Metadata.Machine.NameKey) == GetFieldValue<Machine>(DatItem.MachineKey)?.GetFieldValue<string?>(Models.Metadata.Machine.NameKey) && lastItem?.GetName() == GetName())
                    output = DupeType.External | DupeType.All;
                else
                    output = DupeType.External | DupeType.Hash;
            }

            // Otherwise, it's considered an internal dupe
            else
            {
                if (lastItem?.GetFieldValue<Machine>(DatItem.MachineKey)?.GetFieldValue<string?>(Models.Metadata.Machine.NameKey) == GetFieldValue<Machine>(DatItem.MachineKey)?.GetFieldValue<string?>(Models.Metadata.Machine.NameKey) && lastItem?.GetName() == GetName())
                    output = DupeType.Internal | DupeType.All;
                else
                    output = DupeType.Internal | DupeType.Hash;
            }

            return output;
        }

        #endregion

        #region Manipulation

        /// <summary>
        /// Runs a filter and determines if it passes or not
        /// </summary>
        /// <param name="filterRunner">Filter runner to use for checking</param>
        /// <returns>True if the item passes the filter, false otherwise</returns>
        public bool PassesFilter(FilterRunner filterRunner)
        {
            if (!GetFieldValue<Machine>(DatItem.MachineKey)!.PassesFilter(filterRunner))
                return false;

            return filterRunner.Run(_internal);
        }

        /// <summary>
        /// Remove a field from the DatItem
        /// </summary>
        /// <param name="fieldName">Field to remove</param>
        /// <returns>True if the removal was successful, false otherwise</returns>
        public bool RemoveField(string? fieldName)
            => FieldManipulator.RemoveField(_internal, fieldName);

        /// <summary>
        /// Replace a field from another DatItem
        /// </summary>
        /// <param name="other">DatItem to replace field from</param>
        /// <param name="fieldName">Field to replace</param>
        /// <returns>True if the replacement was successful, false otherwise</returns>
        public bool ReplaceField(DatItem? other, string? fieldName)
            => FieldManipulator.ReplaceField(other?._internal, _internal, fieldName);

        /// <summary>
        /// Set a field in the DatItem from a mapping string
        /// </summary>
        /// <param name="fieldName">Field to set</param>
        /// <param name="value">String representing the value to set</param>
        /// <returns>True if the removal was successful, false otherwise</returns>
        /// <remarks>This only performs minimal validation before setting</remarks>
        public bool SetField(string? fieldName, string value)
            => FieldManipulator.SetField(_internal, fieldName, value);

        #endregion

        #region Sorting and Merging

        /// <summary>
        /// Get the dictionary key that should be used for a given item and bucketing type
        /// </summary>
        /// <param name="bucketedBy">ItemKey value representing what key to get</param>
        /// <param name="lower">True if the key should be lowercased (default), false otherwise</param>
        /// <param name="norename">True if games should only be compared on game and file name, false if system and source are counted</param>
        /// <returns>String representing the key to be used for the DatItem</returns>
        public virtual string GetKey(ItemKey bucketedBy, bool lower = true, bool norename = true)
        {
            // Set the output key as the default blank string
            string key = string.Empty;

            // Now determine what the key should be based on the bucketedBy value
            switch (bucketedBy)
            {
                case ItemKey.CRC:
                    key = Constants.CRCZero;
                    break;

                case ItemKey.Machine:
                    key = (norename ? string.Empty
                        : GetFieldValue<Source?>(DatItem.SourceKey)?.Index.ToString().PadLeft(10, '0')
                            + "-")
                    + (string.IsNullOrEmpty(GetFieldValue<Machine>(DatItem.MachineKey)?.GetFieldValue<string?>(Models.Metadata.Machine.NameKey))
                            ? "Default"
                            : GetFieldValue<Machine>(DatItem.MachineKey)!.GetFieldValue<string?>(Models.Metadata.Machine.NameKey)!);
                    if (lower)
                        key = key.ToLowerInvariant();

                    key ??= "null";

                    break;

                case ItemKey.MD5:
                    key = Constants.MD5Zero;
                    break;

                case ItemKey.SHA1:
                    key = Constants.SHA1Zero;
                    break;

                case ItemKey.SHA256:
                    key = Constants.SHA256Zero;
                    break;

                case ItemKey.SHA384:
                    key = Constants.SHA384Zero;
                    break;

                case ItemKey.SHA512:
                    key = Constants.SHA512Zero;
                    break;

                case ItemKey.SpamSum:
                    key = Constants.SpamSumZero;
                    break;
            }

            // Double and triple check the key for corner cases
            key ??= string.Empty;

            return key;
        }

        #endregion

        #endregion // Instance Methods

        #region Static Methods

        #region Sorting and Merging

        /// <summary>
        /// Merge an arbitrary set of ROMs based on the supplied information
        /// </summary>
        /// <param name="infiles">List of File objects representing the roms to be merged</param>
        /// <returns>A List of DatItem objects representing the merged roms</returns>
        public static ConcurrentList<DatItem> Merge(ConcurrentList<DatItem>? infiles)
        {
            // Check for null or blank roms first
            if (infiles == null || infiles.Count == 0)
                return [];

            // Create output list
            ConcurrentList<DatItem> outfiles = [];

            // Then deduplicate them by checking to see if data matches previous saved roms
            int nodumpCount = 0;
            for (int f = 0; f < infiles.Count; f++)
            {
                DatItem item = infiles[f];

                // If we somehow have a null item, skip
                if (item == null)
                    continue;

                // If we don't have a Disk, File, Media, or Rom, we skip checking for duplicates
                if (item is not Disk && item is not Formats.File && item is not Media && item is not Rom)
                {
                    continue;
                }

                // If it's a nodump, add and skip
                if (item is Rom rom && rom.GetFieldValue<ItemStatus>(Models.Metadata.Rom.StatusKey) == ItemStatus.Nodump)
                {
                    outfiles.Add(item);
                    nodumpCount++;
                    continue;
                }
                else if (item is Disk disk && disk.GetFieldValue<ItemStatus>(Models.Metadata.Disk.StatusKey) == ItemStatus.Nodump)
                {
                    outfiles.Add(item);
                    nodumpCount++;
                    continue;
                }
                // If it's the first non-nodump rom in the list, don't touch it
                else if (outfiles.Count == 0 || outfiles.Count == nodumpCount)
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
                    DatItem lastrom = outfiles[i];

                    // Get the duplicate status
                    dupetype = item.GetDuplicateStatus(lastrom);

                    // If it's a duplicate, skip adding it to the output but add any missing information
                    if (dupetype != 0x00)
                    {
                        saveditem = lastrom;
                        pos = i;

                        // Disks, Media, and Roms have more information to fill
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
                        if (saveditem.GetFieldValue<Machine>(DatItem.MachineKey)!.GetFieldValue<string?>(Models.Metadata.Machine.CloneOfKey) == item.GetFieldValue<Machine>(DatItem.MachineKey)!.GetFieldValue<string?>(Models.Metadata.Machine.NameKey)
                            || saveditem.GetFieldValue<Machine>(DatItem.MachineKey)!.GetFieldValue<string?>(Models.Metadata.Machine.RomOfKey) == item.GetFieldValue<Machine>(DatItem.MachineKey)!.GetFieldValue<string?>(Models.Metadata.Machine.NameKey))
                        {
                            saveditem.CopyMachineInformation(item);
                            saveditem.SetName(item.GetName());
                        }

                        break;
                    }
                }

                // If no duplicate is found, add it to the list
                if (dupetype == 0x00)
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
        /// Resolve name duplicates in an arbitrary set of ROMs based on the supplied information
        /// </summary>
        /// <param name="infiles">List of File objects representing the roms to be merged</param>
        /// <returns>A List of DatItem objects representing the renamed roms</returns>
        public static ConcurrentList<DatItem> ResolveNames(ConcurrentList<DatItem> infiles)
        {
            // Create the output list
            ConcurrentList<DatItem> output = [];

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
                string lastItemName = lastItem.GetName() ?? lastItem.GetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey).ToString();

                // Get the current item name, if applicable
                string datItemName = datItem.GetName() ?? datItem.GetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey).ToString();

                // If the current item exactly matches the last item, then we don't add it
#if NETFRAMEWORK
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
        public static bool Sort(ref ConcurrentList<DatItem> roms, bool norename)
        {
            roms.Sort(delegate (DatItem x, DatItem y)
            {
                try
                {
                    NaturalComparer nc = new();

                    // If machine names match, more refinement is needed
                    if (x.GetFieldValue<Machine>(DatItem.MachineKey)!.GetFieldValue<string?>(Models.Metadata.Machine.NameKey) == y.GetFieldValue<Machine>(DatItem.MachineKey)!.GetFieldValue<string?>(Models.Metadata.Machine.NameKey))
                    {
                        // If item types match, more refinement is needed
                        if (x.GetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey) == y.GetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey))
                        {
                            string? xDirectoryName = Path.GetDirectoryName(TextHelper.RemovePathUnsafeCharacters(x.GetName() ?? string.Empty));
                            string? yDirectoryName = Path.GetDirectoryName(TextHelper.RemovePathUnsafeCharacters(y.GetName() ?? string.Empty));

                            // If item directory names match, more refinement is needed
                            if (xDirectoryName == yDirectoryName)
                            {
                                string? xName = Path.GetFileName(TextHelper.RemovePathUnsafeCharacters(x.GetName() ?? string.Empty));
                                string? yName = Path.GetFileName(TextHelper.RemovePathUnsafeCharacters(y.GetName() ?? string.Empty));

                                // If item names match, then compare on machine or source, depending on the flag
                                if (xName == yName)
                                    return (norename ? nc.Compare(x.GetFieldValue<Machine>(DatItem.MachineKey)!.GetFieldValue<string?>(Models.Metadata.Machine.NameKey), y.GetFieldValue<Machine>(DatItem.MachineKey)!.GetFieldValue<string?>(Models.Metadata.Machine.NameKey)) : (x.GetFieldValue<Source?>(DatItem.SourceKey)?.Index - y.GetFieldValue<Source?>(DatItem.SourceKey)?.Index) ?? 0);

                                // Otherwise, just sort based on item names
                                return nc.Compare(xName, yName);
                            }

                            // Otherwise, just sort based on directory name
                            return nc.Compare(xDirectoryName, yDirectoryName);
                        }

                        // Otherwise, just sort based on item type
                        return x.GetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey) - y.GetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey);
                    }

                    // Otherwise, just sort based on machine name
                    return nc.Compare(x.GetFieldValue<Machine>(DatItem.MachineKey)!.GetFieldValue<string?>(Models.Metadata.Machine.NameKey), y.GetFieldValue<Machine>(DatItem.MachineKey)!.GetFieldValue<string?>(Models.Metadata.Machine.NameKey));
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

        #endregion // Static Methods
    }
}
