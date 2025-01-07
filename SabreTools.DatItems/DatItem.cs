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
        protected static readonly Logger staticLogger = new();

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
#if NET20 || NET35
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
#if NET20 || NET35
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
                    if (lower)
                        key = key.ToLowerInvariant();

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
                    key = ZeroHash.CRC32Str;
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
            var concrete = Array.Find(Assembly.GetExecutingAssembly().GetTypes(),
                t => !t.IsAbstract && t.IsClass && t.BaseType == typeof(DatItem<T>));

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
