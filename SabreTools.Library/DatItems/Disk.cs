using System.Collections.Generic;
using System.IO;
using System.Linq;

using SabreTools.Library.FileTypes;
using SabreTools.Library.Filtering;
using SabreTools.Library.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Represents Compressed Hunks of Data (CHD) formatted disks which use internal hashes
    /// </summary>
    [JsonObject("disk")]
    public class Disk : DatItem
    {
        #region Private instance variables

        private byte[] _md5; // 16 bytes
        private byte[] _sha1; // 20 bytes

        #endregion

        #region Fields

        #region Common

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Data MD5 hash
        /// </summary>
        [JsonProperty("md5", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string MD5
        {
            get { return _md5.IsNullOrEmpty() ? null : Utilities.ByteArrayToString(_md5); }
            set { _md5 = Utilities.StringToByteArray(Sanitizer.CleanMD5(value)); }
        }

        /// <summary>
        /// Data SHA-1 hash
        /// </summary>
        [JsonProperty("sha1", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string SHA1
        {
            get { return _sha1.IsNullOrEmpty() ? null : Utilities.ByteArrayToString(_sha1); }
            set { _sha1 = Utilities.StringToByteArray(Sanitizer.CleanSHA1(value)); }
        }

        /// <summary>
        /// Disk name to merge from parent
        /// </summary>
        [JsonProperty("merge", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string MergeTag { get; set; }

        /// <summary>
        /// Disk region
        /// </summary>
        [JsonProperty("region", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Region { get; set; }

        /// <summary>
        /// Disk index
        /// </summary>
        [JsonProperty("index", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Index { get; set; }

        /// <summary>
        /// Disk writable flag
        /// </summary>
        [JsonProperty("writable", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? Writable { get; set; }

        /// <summary>
        /// Disk dump status
        /// </summary>
        [JsonProperty("status", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ItemStatus ItemStatus { get; set; }

        /// <summary>
        /// Determine if the disk is optional in the set
        /// </summary>
        [JsonProperty("optional", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? Optional { get; set; }

        #endregion

        #region SoftwareList

        /// <summary>
        /// Disk area information
        /// </summary>
        [JsonProperty("diskarea", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DiskArea DiskArea { get; set; }

        /// <summary>
        /// Original hardware part associated with the item
        /// </summary>
        [JsonProperty("part", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Part Part { get; set; }

        #endregion

        #endregion // Fields

        #region Accessors

        /// <summary>
        /// Gets the name to use for a DatItem
        /// </summary>
        /// <returns>Name if available, null otherwise</returns>
        public override string GetName()
        {
            return Name;
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="mappings">Mappings dictionary</param>
        public override void SetFields(Dictionary<Field, string> mappings)
        {
            // Set base fields
            base.SetFields(mappings);

            // Handle Disk-specific fields
            if (mappings.Keys.Contains(Field.DatItem_Name))
                Name = mappings[Field.DatItem_Name];

            if (mappings.Keys.Contains(Field.DatItem_MD5))
                MD5 = mappings[Field.DatItem_MD5];

            if (mappings.Keys.Contains(Field.DatItem_SHA1))
                SHA1 = mappings[Field.DatItem_SHA1];

            if (mappings.Keys.Contains(Field.DatItem_Merge))
                MergeTag = mappings[Field.DatItem_Merge];

            if (mappings.Keys.Contains(Field.DatItem_Region))
                Region = mappings[Field.DatItem_Region];

            if (mappings.Keys.Contains(Field.DatItem_Index))
                Index = mappings[Field.DatItem_Index];

            if (mappings.Keys.Contains(Field.DatItem_Writable))
                Writable = mappings[Field.DatItem_Writable].AsYesNo();

            if (mappings.Keys.Contains(Field.DatItem_Status))
                ItemStatus = mappings[Field.DatItem_Status].AsItemStatus();

            if (mappings.Keys.Contains(Field.DatItem_Optional))
                Optional = mappings[Field.DatItem_Optional].AsYesNo();

            if (mappings.Keys.Contains(Field.DatItem_AreaName))
            {
                if (DiskArea == null)
                    DiskArea = new DiskArea();

                DiskArea.Name = mappings[Field.DatItem_AreaName];
            }

            if (mappings.Keys.Contains(Field.DatItem_Part_Name))
            {
                if (Part == null)
                    Part = new Part();

                Part.Name = mappings[Field.DatItem_Part_Name];
            }

            if (mappings.Keys.Contains(Field.DatItem_Part_Interface))
            {
                if (Part == null)
                    Part = new Part();

                Part.Interface = mappings[Field.DatItem_Part_Interface];
            }

            // TODO: Handle DatItem_Feature*
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Disk object
        /// </summary>
        public Disk()
        {
            Name = string.Empty;
            ItemType = ItemType.Disk;
            DupeType = 0x00;
            ItemStatus = ItemStatus.None;
        }

        /// <summary>
        /// Create a Disk object from a BaseFile
        /// </summary>
        /// <param name="baseFile"></param>
        public Disk(BaseFile baseFile)
        {
            Name = baseFile.Filename;
            _md5 = baseFile.MD5;
            _sha1 = baseFile.SHA1;

            ItemType = ItemType.Disk;
            DupeType = 0x00;
            ItemStatus = ItemStatus.None;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new Disk()
            {
                Name = this.Name,
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                AltName = this.AltName,
                AltTitle = this.AltTitle,

                Original = this.Original,
                OpenMSXSubType = this.OpenMSXSubType,
                OpenMSXType = this.OpenMSXType,
                Remark = this.Remark,
                Boot = this.Boot,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                _md5 = this._md5,
                _sha1 = this._sha1,
                MergeTag = this.MergeTag,
                Region = this.Region,
                Index = this.Index,
                Writable = this.Writable,
                ItemStatus = this.ItemStatus,
                Optional = this.Optional,

                DiskArea = this.DiskArea,
                Part = this.Part,
            };
        }

        /// <summary>
        /// Convert a disk to the closest Rom approximation
        /// </summary>
        /// <returns></returns>
        public Rom ConvertToRom()
        {
            var rom = new Rom()
            {
                Name = this.Name + ".chd",
                ItemType = ItemType.Rom,
                DupeType = this.DupeType,

                AltName = this.AltName,
                AltTitle = this.AltTitle,

                Original = this.Original,
                OpenMSXSubType = this.OpenMSXSubType,
                OpenMSXType = this.OpenMSXType,
                Remark = this.Remark,
                Boot = this.Boot,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                MergeTag = this.MergeTag,
                Region = this.Region,
                ItemStatus = this.ItemStatus,
                Optional = this.Optional,

                MD5 = this.MD5,
                SHA1 = this.SHA1,

                DataArea = new DataArea { Name = this.DiskArea.Name },
                Part = this.Part,
            };

            return rom;
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            bool dupefound = false;

            // If we don't have a rom, return false
            if (ItemType != other.ItemType)
                return dupefound;

            // Otherwise, treat it as a Disk
            Disk newOther = other as Disk;

            // If all hashes are empty but they're both nodump and the names match, then they're dupes
            if ((ItemStatus == ItemStatus.Nodump && newOther.ItemStatus == ItemStatus.Nodump)
                && Name == newOther.Name
                && !HasHashes() && !newOther.HasHashes())
            {
                dupefound = true;
            }

            // Otherwise if we get a partial match
            else if (HashMatch(newOther))
            {
                dupefound = true;
            }

            return dupefound;
        }

        /// <summary>
        /// Fill any missing size and hash information from another Disk
        /// </summary>
        /// <param name="other">Disk to fill information from</param>
        public void FillMissingInformation(Disk other)
        {
            if (_md5.IsNullOrEmpty() && !other._md5.IsNullOrEmpty())
                _md5 = other._md5;

            if (_sha1.IsNullOrEmpty() && !other._sha1.IsNullOrEmpty())
                _sha1 = other._sha1;
        }

        /// <summary>
        /// Get unique duplicate suffix on name collision
        /// </summary>
        /// <returns>String representing the suffix</returns>
        public string GetDuplicateSuffix()
        {
            if (!_md5.IsNullOrEmpty())
                return $"_{MD5}";
            else if (!_sha1.IsNullOrEmpty())
                return $"_{SHA1}";
            else
                return "_1";
        }

        /// <summary>
        /// Returns if there are no, non-empty hashes in common with another Disk
        /// </summary>
        /// <param name="other">Disk to compare against</param>
        /// <returns>True if at least one hash is not mutually exclusive, false otherwise</returns>
        private bool HasCommonHash(Disk other)
        {
            return !(_md5.IsNullOrEmpty() ^ other._md5.IsNullOrEmpty())
                || !(_sha1.IsNullOrEmpty() ^ other._sha1.IsNullOrEmpty());
        }

        /// <summary>
        /// Returns if the Disk contains any hashes
        /// </summary>
        /// <returns>True if any hash exists, false otherwise</returns>
        private bool HasHashes()
        {
            return !_md5.IsNullOrEmpty()
                || !_sha1.IsNullOrEmpty();
        }

        /// <summary>
        /// Returns if any hashes are common with another Disk
        /// </summary>
        /// <param name="other">Disk to compare against</param>
        /// <returns>True if any hashes are in common, false otherwise</returns>
        private bool HashMatch(Disk other)
        {
            // If either have no hashes, we return false, otherwise this would be a false positive
            if (!HasHashes() || !other.HasHashes())
                return false;

            // If neither have hashes in common, we return false, otherwise this would be a false positive
            if (!HasCommonHash(other))
                return false;

            // Return if all hashes match according to merge rules
            return ConditionalHashEquals(_md5, other._md5)
                && ConditionalHashEquals(_sha1, other._sha1);
        }

        #endregion

        #region Filtering

        /// <summary>
        /// Clean a DatItem according to the cleaner
        /// </summary>
        /// <param name="cleaner">Cleaner to implement</param>
        public override void Clean(Cleaner cleaner)
        {
            // Clean common items first
            base.Clean(cleaner);

            // If we're stripping unicode characters, strip item name
            if (cleaner?.RemoveUnicode == true)
                Name = Sanitizer.RemoveUnicodeCharacters(Name);

            // If we are in NTFS trim mode, trim the game name
            if (cleaner?.Trim == true)
            {
                // Windows max name length is 260
                int usableLength = 260 - Machine.Name.Length - (cleaner.Root?.Length ?? 0);
                if (Name.Length > usableLength)
                {
                    string ext = Path.GetExtension(Name);
                    Name = Name.Substring(0, usableLength - ext.Length);
                    Name += ext;
                }
            }
        }

        /// <summary>
        /// Check to see if a DatItem passes the filter
        /// </summary>
        /// <param name="filter">Filter to check against</param>
        /// <returns>True if the item passed the filter, false otherwise</returns>
        public override bool PassesFilter(Filter filter)
        {
            // Check common fields first
            if (!base.PassesFilter(filter))
                return false;

            #region Common

            // Filter on item name
            if (filter.DatItem_Name.MatchesPositiveSet(Name) == false)
                return false;
            if (filter.DatItem_Name.MatchesNegativeSet(Name) == true)
                return false;

            // Filter on MD5
            if (filter.DatItem_MD5.MatchesPositiveSet(MD5) == false)
                return false;
            if (filter.DatItem_MD5.MatchesNegativeSet(MD5) == true)
                return false;

            // Filter on SHA-1
            if (filter.DatItem_SHA1.MatchesPositiveSet(SHA1) == false)
                return false;
            if (filter.DatItem_SHA1.MatchesNegativeSet(SHA1) == true)
                return false;

            // Filter on merge tag
            if (filter.DatItem_Merge.MatchesPositiveSet(MergeTag) == false)
                return false;
            if (filter.DatItem_Merge.MatchesNegativeSet(MergeTag) == true)
                return false;

            // Filter on region
            if (filter.DatItem_Region.MatchesPositiveSet(Region) == false)
                return false;
            if (filter.DatItem_Region.MatchesNegativeSet(Region) == true)
                return false;

            // Filter on index
            if (filter.DatItem_Index.MatchesPositiveSet(Index) == false)
                return false;
            if (filter.DatItem_Index.MatchesNegativeSet(Index) == true)
                return false;

            // Filter on writable
            if (filter.DatItem_Writable.MatchesNeutral(null, Writable) == false)
                return false;

            // Filter on status
            if (filter.DatItem_Status.MatchesPositive(ItemStatus.NULL, ItemStatus) == false)
                return false;
            if (filter.DatItem_Status.MatchesNegative(ItemStatus.NULL, ItemStatus) == true)
                return false;

            // Filter on optional
            if (filter.DatItem_Optional.MatchesNeutral(null, Optional) == false)
                return false;

            #endregion

            #region SoftwareList

            // Filter on area name
            if (filter.DatItem_AreaName.MatchesPositiveSet(DiskArea?.Name) == false)
                return false;
            if (filter.DatItem_AreaName.MatchesNegativeSet(DiskArea?.Name) == true)
                return false;

            // Filter on part name
            if (filter.DatItem_Part_Name.MatchesPositiveSet(Part?.Name) == false)
                return false;
            if (filter.DatItem_Part_Name.MatchesNegativeSet(Part?.Name) == true)
                return false;

            // Filter on part interface
            if (filter.DatItem_Part_Interface.MatchesPositiveSet(Part?.Interface) == false)
                return false;
            if (filter.DatItem_Part_Interface.MatchesNegativeSet(Part?.Interface) == true)
                return false;

            // TODO: Handle DatItem_Feature*

            #endregion

            return true;
        }

        /// <summary>
        /// Remove fields from the DatItem
        /// </summary>
        /// <param name="fields">List of Fields to remove</param>
        public override void RemoveFields(List<Field> fields)
        {
            // Remove common fields first
            base.RemoveFields(fields);

            // Remove the fields

            #region Common

            if (fields.Contains(Field.DatItem_Name))
                Name = null;

            if (fields.Contains(Field.DatItem_MD5))
                MD5 = null;

            if (fields.Contains(Field.DatItem_SHA1))
                SHA1 = null;

            if (fields.Contains(Field.DatItem_Merge))
                MergeTag = null;

            if (fields.Contains(Field.DatItem_Region))
                Region = null;

            if (fields.Contains(Field.DatItem_Index))
                Index = null;

            if (fields.Contains(Field.DatItem_Writable))
                Writable = null;

            if (fields.Contains(Field.DatItem_Status))
                ItemStatus = ItemStatus.NULL;

            if (fields.Contains(Field.DatItem_Optional))
                Optional = null;

            #endregion

            #region SoftwareList

            if (fields.Contains(Field.DatItem_AreaName))
            {
                if (DiskArea != null)
                    DiskArea.Name = null;
            }

            if (fields.Contains(Field.DatItem_Part_Name) && Part != null)
                Part.Name = null;

            if (fields.Contains(Field.DatItem_Part_Interface) && Part != null)
                Part.Interface = null;

            if (fields.Contains(Field.DatItem_Features) && Part != null)
                Part.Features = null;

            // TODO: Handle DatItem_Feature*

            #endregion
        }

        /// <summary>
        /// Set internal names to match One Rom Per Game (ORPG) logic
        /// </summary>
        public override void SetOneRomPerGame()
        {
            string[] splitname = Name.Split('.');
            Machine.Name += $"/{string.Join(".", splitname.Take(splitname.Length > 1 ? splitname.Length - 1 : 1))}";
            Name = Path.GetFileName(Name);
        }

        #endregion

        #region Sorting and Merging

        /// <summary>
        /// Get the dictionary key that should be used for a given item and bucketing type
        /// </summary>
        /// <param name="bucketedBy">Field enum representing what key to get</param>
        /// <param name="lower">True if the key should be lowercased (default), false otherwise</param>
        /// <param name="norename">True if games should only be compared on game and file name, false if system and source are counted</param>
        /// <returns>String representing the key to be used for the DatItem</returns>
        public override string GetKey(Field bucketedBy, bool lower = true, bool norename = true)
        {
            // Set the output key as the default blank string
            string key = string.Empty;

            // Now determine what the key should be based on the bucketedBy value
            switch (bucketedBy)
            {
                case Field.DatItem_MD5:
                    key = MD5;
                    break;

                case Field.DatItem_SHA1:
                    key = SHA1;
                    break;

                // Let the base handle generic stuff
                default:
                    return base.GetKey(bucketedBy, lower, norename);
            }

            // Double and triple check the key for corner cases
            if (key == null)
                key = string.Empty;

            return key;
        }

        /// <summary>
        /// Replace fields from another item
        /// </summary>
        /// <param name="item">DatItem to pull new information from</param>
        /// <param name="fields">List of Fields representing what should be updated</param>
        public override void ReplaceFields(DatItem item, List<Field> fields)
        {
            // Replace common fields first
            base.ReplaceFields(item, fields);

            // If we don't have a Disk to replace from, ignore specific fields
            if (item.ItemType != ItemType.Disk)
                return;

            // Cast for easier access
            Disk newItem = item as Disk;

            // Replace the fields

            #region Common

            if (fields.Contains(Field.DatItem_Name))
                Name = newItem.Name;

            if (fields.Contains(Field.DatItem_MD5))
            {
                if (string.IsNullOrEmpty(MD5) && !string.IsNullOrEmpty(newItem.MD5))
                    MD5 = newItem.MD5;
            }

            if (fields.Contains(Field.DatItem_SHA1))
            {
                if (string.IsNullOrEmpty(SHA1) && !string.IsNullOrEmpty(newItem.SHA1))
                    SHA1 = newItem.SHA1;
            }

            if (fields.Contains(Field.DatItem_Merge))
                MergeTag = newItem.MergeTag;

            if (fields.Contains(Field.DatItem_Region))
                Region = newItem.Region;

            if (fields.Contains(Field.DatItem_Index))
                Index = newItem.Index;

            if (fields.Contains(Field.DatItem_Writable))
                Writable = newItem.Writable;

            if (fields.Contains(Field.DatItem_Status))
                ItemStatus = newItem.ItemStatus;

            if (fields.Contains(Field.DatItem_Optional))
                Optional = newItem.Optional;

            #endregion

            #region SoftwareList

            if (fields.Contains(Field.DatItem_AreaName))
            {
                if (DiskArea == null)
                    DiskArea = new DiskArea();

                DiskArea.Name = newItem.DiskArea?.Name;
            }

            if (fields.Contains(Field.DatItem_Part_Name))
            {
                if (Part == null)
                    Part = new Part();

                Part.Name = newItem.Part?.Name;
            }

            if (fields.Contains(Field.DatItem_Part_Interface))
            {
                if (Part == null)
                    Part = new Part();

                Part.Interface = newItem.Part?.Interface;
            }

            if (fields.Contains(Field.DatItem_Features))
            {
                if (Part == null)
                    Part = new Part();

                Part.Features = newItem.Part?.Features;
            }

            #endregion
        }

        #endregion
    }
}
