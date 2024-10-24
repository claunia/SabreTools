using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;
using SabreTools.Core.Filter;
using SabreTools.Core.Tools;
using SabreTools.DatItems.Formats;
using SabreTools.FileTypes;
using SabreTools.Hashing;
using SabreTools.IO.Logging;
using SabreTools.Matching.Compare;

namespace SabreTools.DatItems
{
    /// <summary>
    /// Base class for all items included in a set
    /// </summary>
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
    [XmlInclude(typeof(DeviceRef))]
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
    [XmlInclude(typeof(SharedFeat))]
    [XmlInclude(typeof(Slot))]
    [XmlInclude(typeof(SlotOption))]
    [XmlInclude(typeof(SoftwareList))]
    [XmlInclude(typeof(Sound))]
    public abstract class DatItem : ModelBackedItem<Models.Metadata.DatItem>, IEquatable<DatItem>, IComparable<DatItem>, ICloneable
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
        /// Item type for the object
        /// </summary>
        protected abstract ItemType ItemType { get; }

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
        /// Create a specific type of DatItem to be used based on a BaseFile
        /// </summary>
        /// <param name="baseFile">BaseFile containing information to be created</param>
        /// <returns>DatItem of the specific internal type that corresponds to the inputs</returns>
        public static DatItem? Create(BaseFile? baseFile)
        {
            return baseFile switch
            {
                // Disk
                FileTypes.CHD.CHDFile => new Disk(baseFile),

                // Media
                FileTypes.Aaru.AaruFormat => new Media(baseFile),

                // Rom
                FileTypes.Archives.GZipArchive => new Rom(baseFile),
                FileTypes.Archives.RarArchive => new Rom(baseFile),
                FileTypes.Archives.SevenZipArchive => new Rom(baseFile),
                FileTypes.Archives.TapeArchive => new Rom(baseFile),
                FileTypes.Archives.XZArchive => new Rom(baseFile),
                FileTypes.Archives.ZipArchive => new Rom(baseFile),
                FileTypes.BaseArchive => new Rom(baseFile),
                FileTypes.Folder => null, // Folders cannot be a DatItem
                FileTypes.BaseFile => new Rom(baseFile),

                // Miscellaneous
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
            var machine = item.GetFieldValue<Machine>(DatItem.MachineKey);
            CopyMachineInformation(machine);
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
            if (GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>() != other?.GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>())
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
            if ((lastItem.GetFieldValue<DupeType>(DatItem.DupeTypeKey) & DupeType.External) != 0
                || lastItem?.GetFieldValue<Source?>(DatItem.SourceKey)?.Index != GetFieldValue<Source?>(DatItem.SourceKey)?.Index)
#else
            if (lastItem.GetFieldValue<DupeType>(DatItem.DupeTypeKey).HasFlag(DupeType.External)
                || lastItem?.GetFieldValue<Source?>(DatItem.SourceKey)?.Index != GetFieldValue<Source?>(DatItem.SourceKey)?.Index)
#endif
            {
                var currentMachine = GetFieldValue<Machine>(DatItem.MachineKey);
                var lastMachine = lastItem?.GetFieldValue<Machine>(DatItem.MachineKey);
                if (lastMachine?.GetStringFieldValue(Models.Metadata.Machine.NameKey) == currentMachine?.GetStringFieldValue(Models.Metadata.Machine.NameKey) && lastItem?.GetName() == GetName())
                    output = DupeType.External | DupeType.All;
                else
                    output = DupeType.External | DupeType.Hash;
            }

            // Otherwise, it's considered an internal dupe
            else
            {
                var currentMachine = GetFieldValue<Machine>(DatItem.MachineKey);
                var lastMachine = lastItem?.GetFieldValue<Machine>(DatItem.MachineKey);
                if (lastMachine?.GetStringFieldValue(Models.Metadata.Machine.NameKey) == currentMachine?.GetStringFieldValue(Models.Metadata.Machine.NameKey) && lastItem?.GetName() == GetName())
                    output = DupeType.Internal | DupeType.All;
                else
                    output = DupeType.Internal | DupeType.Hash;
            }

            return output;
        }

        /// <summary>
        /// Return the duplicate status of two items
        /// </summary>
        /// <param name="source">Source associated with this item</param>
        /// <param name="lastItem">DatItem to check against</param>
        /// <param name="lastSource">Source associated with the last item</param>
        /// <returns>The DupeType corresponding to the relationship between the two</returns>
        public DupeType GetDuplicateStatus(Source? source, DatItem? lastItem, Source? lastSource)
        {
            DupeType output = 0x00;

            // If we don't have a duplicate at all, return none
            if (!Equals(lastItem))
                return output;

            // If the duplicate is external already or should be, set it
#if NETFRAMEWORK
            if ((lastItem.GetFieldValue<DupeType>(DatItem.DupeTypeKey) & DupeType.External) != 0
                || lastSource?.Index != source?.Index)
#else
            if (lastItem.GetFieldValue<DupeType>(DatItem.DupeTypeKey).HasFlag(DupeType.External)
                || lastSource?.Index != source?.Index)
#endif
            {
                var currentMachine = GetFieldValue<Machine>(DatItem.MachineKey);
                var lastMachine = lastItem?.GetFieldValue<Machine>(DatItem.MachineKey);
                if (lastMachine?.GetStringFieldValue(Models.Metadata.Machine.NameKey) == currentMachine?.GetStringFieldValue(Models.Metadata.Machine.NameKey) && lastItem?.GetName() == GetName())
                    output = DupeType.External | DupeType.All;
                else
                    output = DupeType.External | DupeType.Hash;
            }

            // Otherwise, it's considered an internal dupe
            else
            {
                var currentMachine = GetFieldValue<Machine>(DatItem.MachineKey);
                var lastMachine = lastItem?.GetFieldValue<Machine>(DatItem.MachineKey);
                if (lastMachine?.GetStringFieldValue(Models.Metadata.Machine.NameKey) == currentMachine?.GetStringFieldValue(Models.Metadata.Machine.NameKey) && lastItem?.GetName() == GetName())
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
                    string sourceString = string.Empty;
                    if (!norename)
                    {
                        var source = GetFieldValue<Source?>(DatItem.SourceKey);
                        if (source != null)
                            sourceString = source.Index.ToString().PadLeft(10, '0') + "-";
                    }

                    string machineString = "Default";
                    var machine = GetFieldValue<Machine>(DatItem.MachineKey);
                    if (machine != null)
                    {
                        var machineName = machine.GetStringFieldValue(Models.Metadata.Machine.NameKey);
                        if (!string.IsNullOrEmpty(machineName))
                            machineString = machineName!;
                    }

                    key = $"{sourceString}{machineString}";
                    if (lower)
                        key = key.ToLowerInvariant();

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

        /// <summary>
        /// Get the dictionary key that should be used for a given item and bucketing type
        /// </summary>
        /// <param name="bucketedBy">ItemKey value representing what key to get</param>
        /// <param name="source">Source associated with the item for renaming</param>
        /// <param name="lower">True if the key should be lowercased (default), false otherwise</param>
        /// <param name="norename">True if games should only be compared on game and file name, false if system and source are counted</param>
        /// <returns>String representing the key to be used for the DatItem</returns>
        public virtual string GetKey(ItemKey bucketedBy, Source? source, bool lower = true, bool norename = true)
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
                    string sourceString = string.Empty;
                    if (!norename && source != null)
                        sourceString = source.Index.ToString().PadLeft(10, '0') + "-";

                    string machineString = "Default";
                    var machine = GetFieldValue<Machine>(DatItem.MachineKey);
                    if (machine != null)
                    {
                        var machineName = machine.GetStringFieldValue(Models.Metadata.Machine.NameKey);
                        if (!string.IsNullOrEmpty(machineName))
                            machineString = machineName!;
                    }

                    key = $"{sourceString}{machineString}";
                    if (lower)
                        key = key.ToLowerInvariant();

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
                        if (saveditem.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.CloneOfKey) == item.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey)
                            || saveditem.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.RomOfKey) == item.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey))
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
                string lastItemName = lastItem.GetName()
                    ?? lastItem.GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>().AsStringValue()
                    ?? string.Empty;

                // Get the current item name, if applicable
                string datItemName = datItem.GetName()
                    ?? datItem.GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>().AsStringValue()
                    ?? string.Empty;

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
        /// Resolve name duplicates in an arbitrary set of ROMs based on the supplied information
        /// </summary>
        /// <param name="infiles">List of File objects representing the roms to be merged</param>
        /// <returns>A List of DatItem objects representing the renamed roms</returns>
        public static ConcurrentList<(long, DatItem)> ResolveNamesDB(ConcurrentList<(long, DatItem)> infiles)
        {
            // Create the output list
            ConcurrentList<(long, DatItem)> output = [];

            // First we want to make sure the list is in alphabetical order
            Sort(ref infiles, true);

            // Now we want to loop through and check names
            (long, DatItem?) lastItem = (-1, null);
            string? lastrenamed = null;
            int lastid = 0;
            for (int i = 0; i < infiles.Count; i++)
            {
                var datItem = infiles[i];

                // If we have the first item, we automatically add it
                if (lastItem.Item2 == null)
                {
                    output.Add(datItem);
                    lastItem = datItem;
                    continue;
                }

                // Get the last item name, if applicable
                string lastItemName = lastItem.Item2.GetName()
                    ?? lastItem.Item2.GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>().AsStringValue()
                    ?? string.Empty;

                // Get the current item name, if applicable
                string datItemName = datItem.Item2.GetName()
                    ?? datItem.Item2.GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>().AsStringValue()
                    ?? string.Empty;

                // If the current item exactly matches the last item, then we don't add it
#if NETFRAMEWORK
                if ((datItem.Item2.GetDuplicateStatus(lastItem.Item2) & DupeType.All) != 0)
#else
                if (datItem.Item2.GetDuplicateStatus(lastItem.Item2).HasFlag(DupeType.All))
#endif
                {
                    staticLogger.Verbose($"Exact duplicate found for '{datItemName}'");
                    continue;
                }

                // If the current name matches the previous name, rename the current item
                else if (datItemName == lastItemName)
                {
                    staticLogger.Verbose($"Name duplicate found for '{datItemName}'");

                    if (datItem.Item2 is Disk || datItem.Item2 is Formats.File || datItem.Item2 is Media || datItem.Item2 is Rom)
                    {
                        datItemName += GetDuplicateSuffix(datItem.Item2);
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
                    datItem.Item2.SetName(datItemName);

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
        public static bool Sort(ref ConcurrentList<(long, DatItem)> roms, bool norename)
        {
            roms.Sort(delegate ((long, DatItem) x, (long, DatItem) y)
            {
                try
                {
                    var nc = new NaturalComparer();

                    // If machine names don't match
                    string? xMachineName = x.Item2.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey);
                    string? yMachineName = y.Item2.GetFieldValue<Machine>(DatItem.MachineKey)!.GetStringFieldValue(Models.Metadata.Machine.NameKey);
                    if (xMachineName != yMachineName)
                        return nc.Compare(xMachineName, yMachineName);

                    // If types don't match
                    string? xType = x.Item2.GetStringFieldValue(Models.Metadata.DatItem.TypeKey);
                    string? yType = y.Item2.GetStringFieldValue(Models.Metadata.DatItem.TypeKey);
                    if (xType != yType)
                        return xType.AsEnumValue<ItemType>() - yType.AsEnumValue<ItemType>();

                    // If directory names don't match
                    string? xDirectoryName = Path.GetDirectoryName(TextHelper.RemovePathUnsafeCharacters(x.Item2.GetName() ?? string.Empty));
                    string? yDirectoryName = Path.GetDirectoryName(TextHelper.RemovePathUnsafeCharacters(y.Item2.GetName() ?? string.Empty));
                    if (xDirectoryName != yDirectoryName)
                        return nc.Compare(xDirectoryName, yDirectoryName);

                    // If item names don't match
                    string? xName = Path.GetFileName(TextHelper.RemovePathUnsafeCharacters(x.Item2.GetName() ?? string.Empty));
                    string? yName = Path.GetFileName(TextHelper.RemovePathUnsafeCharacters(y.Item2.GetName() ?? string.Empty));
                    if (xName != yName)
                        return nc.Compare(xName, yName);

                    // Otherwise, compare on machine or source, depending on the flag
                    int? xSourceIndex = x.Item2.GetFieldValue<Source?>(DatItem.SourceKey)?.Index;
                    int? ySourceIndex = y.Item2.GetFieldValue<Source?>(DatItem.SourceKey)?.Index;
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

        #endregion // Static Methods
    }

    /// <summary>
    /// Base class for all items included in a set that are backed by an internal model
    /// </summary>
    public abstract class DatItem<T> : DatItem, IEquatable<DatItem<T>>, IComparable<DatItem<T>>, ICloneable where T : Models.Metadata.DatItem
    {
        #region Fields

        /// <summary>
        /// Key for accessing the item name, if it exists
        /// </summary>
        protected abstract string? NameKey { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty object
        /// </summary>
        public DatItem()
        {
            _internal = Activator.CreateInstance<T>();

            SetName(string.Empty);
            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        /// <summary>
        /// Create an object from the internal model
        /// </summary>
        public DatItem(T item)
        {
            _internal = item;

            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType);
            SetFieldValue<Machine>(DatItem.MachineKey, new Machine());
        }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName()
        {
            if (NameKey != null)
                return GetStringFieldValue(NameKey);

            return null;
        }

        /// <inheritdoc/>
        public override void SetName(string? name)
        {
            if (NameKey != null)
                SetFieldValue(NameKey, name);
        }

        /// <summary>
        /// Get a clone of the current internal model
        /// </summary>
        public T GetInternalClone() => (_internal.Clone() as T)!;

        #endregion

        #region Cloning Methods

        /// <summary>
        /// Clone the DatItem
        /// </summary>
        /// <returns>Clone of the DatItem</returns>
        public override object Clone()
        {
            var concrete = Assembly.GetExecutingAssembly()
                .GetTypes()
                .FirstOrDefault(t => !t.IsAbstract && t.IsClass && t.BaseType == typeof(DatItem<T>));

            var clone = Activator.CreateInstance(concrete!);
            (clone as DatItem<T>)!._internal = _internal?.Clone() as T ?? Activator.CreateInstance<T>();
            return clone;
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public int CompareTo(DatItem<T>? other)
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
        public virtual bool Equals(DatItem<T>? other)
        {
            // If we don't have a matched type, return false
            if (GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>() != other?.GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>())
                return false;

            // Compare the internal models
            return _internal.EqualTo(other._internal);
        }

        #endregion
    }
}
