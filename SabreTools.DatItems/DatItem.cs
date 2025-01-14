using System;
using System.Reflection;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;
using SabreTools.Core.Filter;
using SabreTools.Core.Tools;
using SabreTools.DatItems.Formats;
using SabreTools.Hashing;
using SabreTools.IO.Logging;

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
        protected static readonly Logger _staticLogger = new();

        #endregion

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
            // If there is no machine
            if (!item._internal.ContainsKey(DatItem.MachineKey))
                return;

            var machine = item.GetFieldValue<Machine?>(DatItem.MachineKey);
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
            // If the other item doesn't exist
            if (other == null)
                return 1;

            // Get the names to avoid changing values
            string? selfName = GetName();
            string? otherName = other.GetName();

            // If the names are equal
            if (selfName == otherName)
                return Equals(other) ? 0 : 1;

            // If `otherName` is null, Compare will return > 0
            // If `selfName` is null, Compare will return < 0
            return string.Compare(selfName, otherName, StringComparison.Ordinal);
        }

        /// <inheritdoc/>
        public override bool Equals(ModelBackedItem? other)
        {
            // If other is null
            if (other == null)
                return false;

            // If the type is mismatched
            if (other is not DatItem otherItem)
                return false;

            // Compare internal models
            return _internal.Equals(otherItem);
        }

        /// <inheritdoc/>
        public override bool Equals(ModelBackedItem<Models.Metadata.DatItem>? other)
        {
            // If other is null
            if (other == null)
                return false;

            // If the type is mismatched
            if (other is not DatItem otherItem)
                return false;

            // Compare internal models
            return _internal.Equals(otherItem);
        }

        /// <summary>
        /// Determine if an item is a duplicate using partial matching logic
        /// </summary>
        /// <param name="other">DatItem to use as a baseline</param>
        /// <returns>True if the items are duplicates, false otherwise</returns>
        public virtual bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other == null)
                return false;

            // Get the types for comparison
            ItemType selfType = GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>();
            ItemType otherType = other.GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>();

            // If we don't have a matched type, return false
            if (selfType != otherType)
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

            // Get the sources for comparison
            var selfSource = GetFieldValue<Source?>(DatItem.SourceKey);
            var lastSource = lastItem.GetFieldValue<Source?>(DatItem.SourceKey);

            // Get the machines for comparison
            var selfMachine = GetFieldValue<Machine>(DatItem.MachineKey);
            string? selfMachineName = selfMachine?.GetStringFieldValue(Models.Metadata.Machine.NameKey);
            var lastMachine = lastItem.GetFieldValue<Machine>(DatItem.MachineKey);
            string? lastMachineName = lastMachine?.GetStringFieldValue(Models.Metadata.Machine.NameKey);

            // If the duplicate is external already
#if NET20 || NET35
            if ((lastItem.GetFieldValue<DupeType>(DatItem.DupeTypeKey) & DupeType.External) != 0)
#else
            if (lastItem.GetFieldValue<DupeType>(DatItem.DupeTypeKey).HasFlag(DupeType.External))
#endif
                output |= DupeType.External;

            // If the duplicate should be external
            else if (lastSource?.Index != selfSource?.Index)
                output |= DupeType.External;

            // Otherwise, it's considered an internal dupe
            else
                output |= DupeType.Internal;

            // If the item and machine names match
            if (lastMachineName == selfMachineName && lastItem.GetName() == GetName())
                output |= DupeType.All;

            // Otherwise, hash match is assumed
            else
                output |= DupeType.Hash;

            return output;
        }

        /// <summary>
        /// Return the duplicate status of two items
        /// </summary>
        /// <param name="selfSource">Source associated with this item</param>
        /// <param name="lastItem">DatItem to check against</param>
        /// <param name="lastSource">Source associated with the last item</param>
        /// <returns>The DupeType corresponding to the relationship between the two</returns>
        public DupeType GetDuplicateStatusDB(Source? selfSource, DatItem? lastItem, Source? lastSource)
        {
            DupeType output = 0x00;

            // If we don't have a duplicate at all, return none
            if (!Equals(lastItem))
                return output;

            // TODO: Fix this since machines are determined in a different way
            // Get the machines for comparison
            var selfMachine = GetFieldValue<Machine>(DatItem.MachineKey);
            string? selfMachineName = selfMachine?.GetStringFieldValue(Models.Metadata.Machine.NameKey);
            var lastMachine = lastItem.GetFieldValue<Machine>(DatItem.MachineKey);
            string? lastMachineName = lastMachine?.GetStringFieldValue(Models.Metadata.Machine.NameKey);

            // If the duplicate is external already
#if NET20 || NET35
            if ((lastItem.GetFieldValue<DupeType>(DatItem.DupeTypeKey) & DupeType.External) != 0)
#else
            if (lastItem.GetFieldValue<DupeType>(DatItem.DupeTypeKey).HasFlag(DupeType.External))
#endif
                output |= DupeType.External;

            // If the duplicate should be external
            else if (lastSource?.Index != selfSource?.Index)
                output |= DupeType.External;

            // Otherwise, it's considered an internal dupe
            else
                output |= DupeType.Internal;

            // If the item and machine names match
            if (lastMachineName == selfMachineName && lastItem.GetName() == GetName())
                output |= DupeType.All;

            // Otherwise, hash match is assumed
            else
                output |= DupeType.Hash;

            return output;
        }

        #endregion

        #region Manipulation

        /// <summary>
        /// Runs a filter and determines if it passes or not
        /// </summary>
        /// <param name="filterRunner">Filter runner to use for checking</param>
        /// <returns>True if the item and its machine passes the filter, false otherwise</returns>
        public bool PassesFilter(FilterRunner filterRunner)
        {
            var machine = GetFieldValue<Machine>(DatItem.MachineKey);
            if (machine != null && !machine.PassesFilter(filterRunner))
                return false;

            return filterRunner.Run(_internal);
        }

        /// <summary>
        /// Runs a filter and determines if it passes or not
        /// </summary>
        /// <param name="filterRunner">Filter runner to use for checking</param>
        /// <returns>True if the item passes the filter, false otherwise</returns>
        public bool PassesFilterDB(FilterRunner filterRunner)
            => filterRunner.Run(_internal);

        #endregion

        #region Sorting and Merging

        /// <summary>
        /// Get unique duplicate suffix on name collision
        /// </summary>
        /// <returns>String representing the suffix</returns>
        public virtual string GetDuplicateSuffix() => _internal.GetDuplicateSuffix();

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
                    key = ZeroHash.CRC32Str;
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
                    break;

                case ItemKey.MD2:
                    key = ZeroHash.GetString(HashType.MD2);
                    break;

                case ItemKey.MD4:
                    key = ZeroHash.GetString(HashType.MD4);
                    break;

                case ItemKey.MD5:
                    key = ZeroHash.MD5Str;
                    break;

                case ItemKey.SHA1:
                    key = ZeroHash.SHA1Str;
                    break;

                case ItemKey.SHA256:
                    key = ZeroHash.SHA256Str;
                    break;

                case ItemKey.SHA384:
                    key = ZeroHash.SHA384Str;
                    break;

                case ItemKey.SHA512:
                    key = ZeroHash.SHA512Str;
                    break;

                case ItemKey.SpamSum:
                    key = ZeroHash.SpamSumStr;
                    break;
            }

            // Double and triple check the key for corner cases
            key ??= string.Empty;
            if (lower)
                key = key.ToLowerInvariant();

            return key;
        }

        /// <summary>
        /// Get the dictionary key that should be used for a given item and bucketing type
        /// </summary>
        /// <param name="bucketedBy">ItemKey value representing what key to get</param>
        /// <param name="machine">Machine associated with the item for renaming</param>
        /// <param name="source">Source associated with the item for renaming</param>
        /// <param name="lower">True if the key should be lowercased (default), false otherwise</param>
        /// <param name="norename">True if games should only be compared on game and file name, false if system and source are counted</param>
        /// <returns>String representing the key to be used for the DatItem</returns>
        public virtual string GetKeyDB(ItemKey bucketedBy, Machine? machine, Source? source, bool lower = true, bool norename = true)
        {
            // Set the output key as the default blank string
            string key = string.Empty;

            string sourceKeyPadded = source?.Index.ToString().PadLeft(10, '0') + '-';
            string machineName = machine?.GetStringFieldValue(Models.Metadata.Machine.NameKey) ?? "Default";

            // Now determine what the key should be based on the bucketedBy value
            switch (bucketedBy)
            {
                case ItemKey.CRC:
                    key = ZeroHash.CRC32Str;
                    break;

                case ItemKey.Machine:
                    key = (norename ? string.Empty : sourceKeyPadded) + machineName;
                    break;

                case ItemKey.MD2:
                    key = ZeroHash.GetString(HashType.MD2);
                    break;

                case ItemKey.MD4:
                    key = ZeroHash.GetString(HashType.MD4);
                    break;

                case ItemKey.MD5:
                    key = ZeroHash.MD5Str;
                    break;

                case ItemKey.SHA1:
                    key = ZeroHash.SHA1Str;
                    break;

                case ItemKey.SHA256:
                    key = ZeroHash.SHA256Str;
                    break;

                case ItemKey.SHA384:
                    key = ZeroHash.SHA384Str;
                    break;

                case ItemKey.SHA512:
                    key = ZeroHash.SHA512Str;
                    break;

                case ItemKey.SpamSum:
                    key = ZeroHash.SpamSumStr;
                    break;
            }

            // Double and triple check the key for corner cases
            key ??= string.Empty;
            if (lower)
                key = key.ToLowerInvariant();

            return key;
        }

        #endregion
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
            if (!string.IsNullOrEmpty(NameKey))
                return GetStringFieldValue(NameKey);

            return null;
        }

        /// <inheritdoc/>
        public override void SetName(string? name)
        {
            if (!string.IsNullOrEmpty(NameKey))
                SetFieldValue(NameKey, name);
        }

        #endregion

        #region Cloning Methods

        /// <summary>
        /// Clone the DatItem
        /// </summary>
        /// <returns>Clone of the DatItem</returns>
        /// <remarks>
        /// Throws an exception if there is a DatItem implementation
        /// that is not a part of this library.
        /// </remarks>
        public override object Clone()
        {
            var concrete = Array.Find(Assembly.GetExecutingAssembly().GetTypes(),
                t => !t.IsAbstract && t.IsClass && t.BaseType == typeof(DatItem<T>));

            var clone = Activator.CreateInstance(concrete!);
            (clone as DatItem<T>)!._internal = _internal?.Clone() as T ?? Activator.CreateInstance<T>();
            return clone;
        }

        /// <summary>
        /// Get a clone of the current internal model
        /// </summary>
        /// TODO: Make virtual so individuals can handle conversion
        public virtual T GetInternalClone() => (_internal.Clone() as T)!;

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public int CompareTo(DatItem<T>? other)
        {
            // If the other item doesn't exist
            if (other == null)
                return 1;

            // Get the names to avoid changing values
            string? selfName = GetName();
            string? otherName = other.GetName();

            // If the names are equal
            if (selfName == otherName)
                return Equals(other) ? 0 : 1;

            // If `otherName` is null, Compare will return > 0
            // If `selfName` is null, Compare will return < 0
            return string.Compare(selfName, otherName, StringComparison.Ordinal);
        }

        /// <summary>
        /// Determine if an item is a duplicate using partial matching logic
        /// </summary>
        /// <param name="other">DatItem to use as a baseline</param>
        /// <returns>True if the items are duplicates, false otherwise</returns>
        public virtual bool Equals(DatItem<T>? other)
        {
            // If the other value is null
            if (other == null)
                return false;

            // Get the types for comparison
            ItemType selfType = GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>();
            ItemType otherType = other.GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>();

            // If we don't have a matched type, return false
            if (selfType != otherType)
                return false;

            // Compare the internal models
            return _internal.EqualTo(other._internal);
        }

        #endregion
    }
}
