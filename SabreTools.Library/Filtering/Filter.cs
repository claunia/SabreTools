using System;
using System.Collections.Generic;

using SabreTools.Library.Data;
using SabreTools.Library.DatFiles;
using SabreTools.Library.DatItems;
using SabreTools.Library.Tools;

namespace SabreTools.Library.Filtering
{
    /// <summary>
    /// Represents the filtering operations that need to be performed on a set of items, usually a DAT
    /// </summary>
    /// TODO: Can clever use of Filtering allow for easier external splitting methods?
    /// TODO: Field name for filter population needs to be overhauled
    public class Filter
    {
        #region Fields

        #region Machine Filters

        #region Common

        /// <summary>
        /// Include or exclude machine names
        /// </summary>
        public FilterItem<string> MachineName { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude machine comments
        /// </summary>
        public FilterItem<string> Comment { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude machine descriptions
        /// </summary>
        public FilterItem<string> MachineDescription { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude machine years
        /// </summary>
        public FilterItem<string> Year { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude machine manufacturers
        /// </summary>
        public FilterItem<string> Manufacturer { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude machine publishers
        /// </summary>
        public FilterItem<string> Publisher { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude machine categories
        /// </summary>
        public FilterItem<string> Category { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude machine romof
        /// </summary>
        public FilterItem<string> RomOf { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude machine cloneof
        /// </summary>
        public FilterItem<string> CloneOf { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude machine sampleof
        /// </summary>
        public FilterItem<string> SampleOf { get; private set; } = new FilterItem<string>();

        #endregion

        #region AttractMode

        /// <summary>
        /// Include or exclude machine players
        /// </summary>
        public FilterItem<string> Players { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude machine rotation
        /// </summary>
        public FilterItem<string> Rotation { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude machine control
        /// </summary>
        public FilterItem<string> Control { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude machine support status
        /// </summary>
        public FilterItem<string> SupportStatus { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude machine display count
        /// </summary>
        public FilterItem<string> DisplayCount { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude machine display type
        /// </summary>
        public FilterItem<string> DisplayType { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude machine buttons
        /// </summary>
        public FilterItem<string> Buttons { get; private set; } = new FilterItem<string>();

        #endregion

        #region ListXML

        /// <summary>
        /// Include or exclude machine source file
        /// </summary>
        public FilterItem<string> SourceFile { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude items with the "Runnable" tag
        /// </summary>
        public FilterItem<Runnable> Runnables { get; private set; } = new FilterItem<Runnable>() { Positive = Runnable.NULL, Negative = Runnable.NULL };

        /// <summary>
        /// Include or exclude machine devices
        /// </summary>
        public FilterItem<string> Devices { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude machine slotoptions
        /// </summary>
        public FilterItem<string> SlotOptions { get; private set; } = new FilterItem<string>();

        // TODO: Machine.Infos - List<ListXmlInfo>

        /// <summary>
        /// Include or exclude machine types
        /// </summary>
        public FilterItem<MachineType> MachineTypes { get; private set; } = new FilterItem<MachineType>() { Positive = MachineType.NULL, Negative = MachineType.NULL };

        #endregion

        #region Logiqx

        /// <summary>
        /// Include or exclude machine board
        /// </summary>
        public FilterItem<string> Board { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude machine rebuildto
        /// </summary>
        public FilterItem<string> RebuildTo { get; private set; } = new FilterItem<string>();

        #endregion

        #region Logiqx EmuArc

        /// <summary>
        /// Include or exclude machine title ID
        /// </summary>
        public FilterItem<string> TitleID { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude machine developer
        /// </summary>
        public FilterItem<string> Developer { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude machine genre
        /// </summary>
        public FilterItem<string> Genre { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude machine subgenre
        /// </summary>
        public FilterItem<string> Subgenre { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude machine ratings
        /// </summary>
        public FilterItem<string> Ratings { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude machine score
        /// </summary>
        public FilterItem<string> Score { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude machine enabled
        /// </summary>
        public FilterItem<string> Enabled { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude items with the "crc" tag
        /// </summary>
        public FilterItem<bool?> HasCrc { get; private set; } = new FilterItem<bool?>() { Neutral = null };

        /// <summary>
        /// Include or exclude machine related to
        /// </summary>
        public FilterItem<string> RelatedTo { get; private set; } = new FilterItem<string>();

        #endregion

        #region OpenMSX

        /// <summary>
        /// Include or exclude machine Generation MSX ID
        /// </summary>
        public FilterItem<string> GenMSXID { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude machine system
        /// </summary>
        public FilterItem<string> System { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude machine country
        /// </summary>
        public FilterItem<string> Country { get; private set; } = new FilterItem<string>();

        #endregion

        #region SoftwareList

        /// <summary>
        /// Include or exclude items with the "Supported" tag
        /// </summary>
        public FilterItem<Supported> SupportedStatus { get; private set; } = new FilterItem<Supported>() { Positive = Supported.NULL, Negative = Supported.NULL };

        // TODO: Machine.SharedFeatures - List<SoftwareListSharedFeature>

        #endregion

        #endregion // Machine Filters

        #region DatItem Filters

        #region Common

        /// <summary>
        /// Include or exclude item names
        /// </summary>
        public FilterItem<string> ItemName { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude item types
        /// </summary>
        public FilterItem<string> ItemTypes { get; private set; } = new FilterItem<string>();

        #endregion

        #region AttractMode

        /// <summary>
        /// Include or exclude alt names
        /// </summary>
        public FilterItem<string> AltName { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude alt titles
        /// </summary>
        public FilterItem<string> AltTitle { get; private set; } = new FilterItem<string>();

        #endregion

        #region OpenMSX

        /// <summary>
        /// Include or exclude original value
        /// </summary>
        public FilterItem<string> Original { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude items of a certain OpenMSX subtype
        /// </summary>
        public FilterItem<OpenMSXSubType> SubType { get; private set; } = new FilterItem<OpenMSXSubType>() { Positive = OpenMSXSubType.NULL, Negative = OpenMSXSubType.NULL };

        /// <summary>
        /// Include or exclude OpenMSX type
        /// </summary>
        public FilterItem<string> OpenMSXType { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude remarks
        /// </summary>
        public FilterItem<string> Remark { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude boots
        /// </summary>
        public FilterItem<string> Boot { get; private set; } = new FilterItem<string>();

        #endregion

        #region SoftwareList

        /// <summary>
        /// Include or exclude part names
        /// </summary>
        public FilterItem<string> PartName { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude part interfaces
        /// </summary>
        public FilterItem<string> PartInterface { get; private set; } = new FilterItem<string>();

        // TODO: DatItem.Features - List<SoftwareListFeature>

        /// <summary>
        /// Include or exclude area names
        /// </summary>
        public FilterItem<string> AreaName { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude area sizes
        /// </summary>
        /// <remarks>Positive means "Greater than or equal", Negative means "Less than or equal", Neutral means "Equal"</remarks>
        public FilterItem<long?> AreaSize { get; private set; } = new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null };

        /// <summary>
        /// Include or exclude area byte widths
        /// </summary>
        public FilterItem<string> AreaWidth { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude area endianness
        /// </summary>
        public FilterItem<string> AreaEndianness { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude softwarelist value
        /// </summary>
        public FilterItem<string> Value { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude load flag
        /// </summary>
        public FilterItem<string> LoadFlag { get; private set; } = new FilterItem<string>();

        #endregion

        /// <summary>
        /// Include or exclude items with the "Default" tag
        /// </summary>
        public FilterItem<bool?> Default { get; private set; } = new FilterItem<bool?>() { Neutral = null };

        /// <summary>
        /// Include or exclude descriptions
        /// </summary>
        public FilterItem<string> Description { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude item sizes
        /// </summary>
        /// <remarks>Positive means "Greater than or equal", Negative means "Less than or equal", Neutral means "Equal"</remarks>
        public FilterItem<long> Size { get; private set; } = new FilterItem<long>() { Positive = -1, Negative = -1, Neutral = -1 };

        /// <summary>
        /// Include or exclude CRC32 hashes
        /// </summary>
        public FilterItem<string> CRC { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude MD5 hashes
        /// </summary>
        public FilterItem<string> MD5 { get; private set; } = new FilterItem<string>();

#if NET_FRAMEWORK
        /// <summary>
        /// Include or exclude RIPEMD160 hashes
        /// </summary>
        public FilterItem<string> RIPEMD160 { get; private set; } = new FilterItem<string>();
#endif

        /// <summary>
        /// Include or exclude SHA-1 hashes
        /// </summary>
        public FilterItem<string> SHA1 { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude SHA-256 hashes
        /// </summary>
        public FilterItem<string> SHA256 { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude SHA-384 hashes
        /// </summary>
        public FilterItem<string> SHA384 { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude SHA-512 hashes
        /// </summary>
        public FilterItem<string> SHA512 { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude merge tags
        /// </summary>
        public FilterItem<string> MergeTag { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude regions
        /// </summary>
        public FilterItem<string> Region { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude indexes
        /// </summary>
        public FilterItem<string> Index { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude items with the "Writable" tag
        /// </summary>
        public FilterItem<bool?> Writable { get; private set; } = new FilterItem<bool?>() { Neutral = null };

        /// <summary>
        /// Include or exclude items with the "Writable" tag
        /// </summary>
        public FilterItem<bool?> Optional { get; private set; } = new FilterItem<bool?>() { Neutral = null };

        /// <summary>
        /// Include or exclude item statuses
        /// </summary>
        public FilterItem<ItemStatus> Status { get; private set; } = new FilterItem<ItemStatus>() { Positive = ItemStatus.NULL, Negative = ItemStatus.NULL };

        /// <summary>
        /// Include or exclude languages
        /// </summary>
        public FilterItem<string> Language { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude dates
        /// </summary>
        public FilterItem<string> Date { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude bioses
        /// </summary>
        public FilterItem<string> Bios { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude offsets
        /// </summary>
        public FilterItem<string> Offset { get; private set; } = new FilterItem<string>();

        /// <summary>
        /// Include or exclude offsets
        /// </summary>
        public FilterItem<bool?> Inverted { get; private set; } = new FilterItem<bool?>();

        #endregion // DatItem Filters

        #region Manipulation Flags

        /// <summary>
        /// Clean all names to WoD standards
        /// </summary>
        public bool Clean { get; set; }

        /// <summary>
        /// Set Machine Description from Machine Name
        /// </summary>
        public bool DescriptionAsName { get; set; }

        /// <summary>
        /// Include romof and cloneof when filtering machine names
        /// </summary>
        public bool IncludeOfInGame { get; set; }

        /// <summary>
        /// Internally split a DatFile
        /// </summary>
        public MergingFlag InternalSplit { get; set; }

        /// <summary>
        /// Remove all unicode characters
        /// </summary>
        public bool RemoveUnicode { get; set; }

        /// <summary>
        /// Include root directory when determing trim sizes
        /// </summary>
        public string Root { get; set; }

        /// <summary>
        /// Change all machine names to "!"
        /// </summary>
        public bool Single { get; set; }

        /// <summary>
        /// Trim total machine and item name to not exceed NTFS limits
        /// </summary>
        public bool Trim { get; set; }

        #endregion

        #endregion // Fields

        #region Instance methods

        #region Filter Population

        /// <summary>
        /// Populate the filters object using a set of key:value filters
        /// </summary>
        /// <param name="filters">List of key:value where ~key/!key is negated</param>
        public void PopulateFromList(List<string> filters)
        {
            foreach (string filterPair in filters)
            {
                // If we don't even have a possible filter pair
                if (!filterPair.Contains(":"))
                {
                    Globals.Logger.Warning($"'{filterPair}` is not a valid filter string. Valid filter strings are of the form 'key:value'. Please refer to README.1ST or the help feature for more details.");
                    continue;
                }

                string filterPairTrimmed = filterPair.Trim('"', ' ', '\t');
                bool negate = filterPairTrimmed.StartsWith("!")
                    || filterPairTrimmed.StartsWith("~")
                    || filterPairTrimmed.StartsWith("not-");
                filterPairTrimmed = filterPairTrimmed.TrimStart('!', '~');
                filterPairTrimmed = filterPairTrimmed.StartsWith("not-") ? filterPairTrimmed.Substring(4) : filterPairTrimmed;

                string filterFieldString = filterPairTrimmed.Split(':')[0].ToLowerInvariant().Trim('"', ' ', '\t');
                string filterValue = filterPairTrimmed.Substring(filterFieldString.Length + 1).Trim('"', ' ', '\t');

                Field filterField = filterFieldString.AsField();
                SetFilter(filterField, filterValue, negate);
            }
        }

        /// <summary>
        /// Set multiple filters from key
        /// </summary>
        /// <param name="key">Key for the filter to be set</param>
        /// <param name="values">List of values for the filter</param>
        /// <param name="negate">True if negative filter, false otherwise</param>
        public void SetFilter(Field key, List<string> values, bool negate)
        {
            foreach (string value in values)
            {
                SetFilter(key, value, negate);
            }
        }

        /// <summary>
        /// Set a single filter from key
        /// </summary>
        /// <param name="key">Key for the filter to be set</param>
        /// <param name="value">Value of the filter</param>
        /// <param name="negate">True if negative filter, false otherwise</param>
        public void SetFilter(Field key, string value, bool negate)
        {
            switch (key)
            {
                #region Machine Filters

                #region Common

                case Field.Machine_Name:
                    if (negate)
                        MachineName.NegativeSet.Add(value);
                    else
                        MachineName.PositiveSet.Add(value);
                    break;

                case Field.Machine_Comment:
                    if (negate)
                        Comment.NegativeSet.Add(value);
                    else
                        Comment.PositiveSet.Add(value);
                    break;

                case Field.Machine_Description:
                    if (negate)
                        MachineDescription.NegativeSet.Add(value);
                    else
                        MachineDescription.PositiveSet.Add(value);
                    break;

                case Field.Machine_Year:
                    if (negate)
                        Year.NegativeSet.Add(value);
                    else
                        Year.PositiveSet.Add(value);
                    break;

                case Field.Machine_Manufacturer:
                    if (negate)
                        Manufacturer.NegativeSet.Add(value);
                    else
                        Manufacturer.PositiveSet.Add(value);
                    break;

                case Field.Machine_Publisher:
                    if (negate)
                        Publisher.NegativeSet.Add(value);
                    else
                        Publisher.PositiveSet.Add(value);
                    break;

                case Field.Machine_Category:
                    if (negate)
                        Category.NegativeSet.Add(value);
                    else
                        Category.PositiveSet.Add(value);
                    break;

                case Field.Machine_RomOf:
                    if (negate)
                        RomOf.NegativeSet.Add(value);
                    else
                        RomOf.PositiveSet.Add(value);
                    break;

                case Field.Machine_CloneOf:
                    if (negate)
                        CloneOf.NegativeSet.Add(value);
                    else
                        CloneOf.PositiveSet.Add(value);
                    break;

                case Field.Machine_SampleOf:
                    if (negate)
                        SampleOf.NegativeSet.Add(value);
                    else
                        SampleOf.PositiveSet.Add(value);
                    break;

                case Field.Machine_Type:
                    if (negate)
                        MachineTypes.Negative |= value.AsMachineType();
                    else
                        MachineTypes.Positive |= value.AsMachineType();
                    break;

                #endregion

                #region AttractMode

                case Field.Machine_Players:
                    if (negate)
                        Players.NegativeSet.Add(value);
                    else
                        Players.PositiveSet.Add(value);
                    break;

                case Field.Machine_Rotation:
                    if (negate)
                        Rotation.NegativeSet.Add(value);
                    else
                        Rotation.PositiveSet.Add(value);
                    break;

                case Field.Machine_Control:
                    if (negate)
                        Control.NegativeSet.Add(value);
                    else
                        Control.PositiveSet.Add(value);
                    break;

                case Field.Machine_Status:
                    if (negate)
                        SupportStatus.NegativeSet.Add(value);
                    else
                        SupportStatus.PositiveSet.Add(value);
                    break;

                case Field.Machine_DisplayCount:
                    if (negate)
                        DisplayCount.NegativeSet.Add(value);
                    else
                        DisplayCount.PositiveSet.Add(value);
                    break;

                case Field.Machine_DisplayType:
                    if (negate)
                        DisplayType.NegativeSet.Add(value);
                    else
                        DisplayType.PositiveSet.Add(value);
                    break;

                case Field.Machine_Buttons:
                    if (negate)
                        Buttons.NegativeSet.Add(value);
                    else
                        Buttons.PositiveSet.Add(value);
                    break;

                #endregion

                #region ListXML

                case Field.Machine_SourceFile:
                    if (negate)
                        SourceFile.NegativeSet.Add(value);
                    else
                        SourceFile.PositiveSet.Add(value);
                    break;

                case Field.Machine_Runnable:
                    if (negate)
                        Runnables.Negative |= value.AsRunnable();
                    else
                        Runnables.Positive |= value.AsRunnable();
                    break;

                case Field.Machine_DeviceReference_Name:
                    if (negate)
                        Devices.NegativeSet.Add(value);
                    else
                        Devices.PositiveSet.Add(value);
                    break;

                case Field.Machine_Slots:
                    if (negate)
                        SlotOptions.NegativeSet.Add(value);
                    else
                        SlotOptions.PositiveSet.Add(value);
                    break;

                #endregion

                #region Logiqx

                case Field.Machine_Board:
                    if (negate)
                        Board.NegativeSet.Add(value);
                    else
                        Board.PositiveSet.Add(value);
                    break;

                case Field.Machine_RebuildTo:
                    if (negate)
                        RebuildTo.NegativeSet.Add(value);
                    else
                        RebuildTo.PositiveSet.Add(value);
                    break;

                #endregion

                #region Logiqx EmuArc

                case Field.Machine_TitleID:
                    if (negate)
                        TitleID.NegativeSet.Add(value);
                    else
                        TitleID.PositiveSet.Add(value);
                    break;

                case Field.Machine_Developer:
                    if (negate)
                        Developer.NegativeSet.Add(value);
                    else
                        Developer.PositiveSet.Add(value);
                    break;

                case Field.Machine_Genre:
                    if (negate)
                        Genre.NegativeSet.Add(value);
                    else
                        Genre.PositiveSet.Add(value);
                    break;

                case Field.Machine_Subgenre:
                    if (negate)
                        Subgenre.NegativeSet.Add(value);
                    else
                        Subgenre.PositiveSet.Add(value);
                    break;

                case Field.Machine_Ratings:
                    if (negate)
                        Ratings.NegativeSet.Add(value);
                    else
                        Ratings.PositiveSet.Add(value);
                    break;

                case Field.Machine_Score:
                    if (negate)
                        Score.NegativeSet.Add(value);
                    else
                        Score.PositiveSet.Add(value);
                    break;

                case Field.Machine_Enabled:
                    if (negate)
                        Enabled.NegativeSet.Add(value);
                    else
                        Enabled.PositiveSet.Add(value);
                    break;

                case Field.Machine_CRC:
                    if (negate || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                        HasCrc.Neutral = false;
                    else
                        HasCrc.Neutral = true;
                    break;

                case Field.Machine_RelatedTo:
                    if (negate)
                        RelatedTo.NegativeSet.Add(value);
                    else
                        RelatedTo.PositiveSet.Add(value);
                    break;

                #endregion

                #region OpenMSX

                case Field.Machine_GenMSXID:
                    if (negate)
                        GenMSXID.NegativeSet.Add(value);
                    else
                        GenMSXID.PositiveSet.Add(value);
                    break;

                case Field.Machine_System:
                    if (negate)
                        System.NegativeSet.Add(value);
                    else
                        System.PositiveSet.Add(value);
                    break;

                case Field.Machine_Country:
                    if (negate)
                        Country.NegativeSet.Add(value);
                    else
                        Country.PositiveSet.Add(value);
                    break;

                #endregion

                #region SoftwareList

                case Field.Machine_Supported:
                    if (negate)
                        SupportedStatus.Negative |= value.AsSupported();
                    else
                        SupportedStatus.Positive |= value.AsSupported();
                    break;

                #endregion

                #endregion // Machine Filters

                #region DatItem Filters

                #region Common

                case Field.DatItem_Name:
                    if (negate)
                        ItemName.NegativeSet.Add(value);
                    else
                        ItemName.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Type:
                    if (value.AsItemType() == null)
                        return;

                    if (negate)
                        ItemTypes.NegativeSet.Add(value);
                    else
                        ItemTypes.PositiveSet.Add(value);
                    break;

                #endregion

                #region AttractMode

                case Field.DatItem_AltName:
                    if (negate)
                        AltName.NegativeSet.Add(value);
                    else
                        AltName.PositiveSet.Add(value);
                    break;

                case Field.DatItem_AltTitle:
                    if (negate)
                        AltTitle.NegativeSet.Add(value);
                    else
                        AltTitle.PositiveSet.Add(value);
                    break;

                #endregion

                #region OpenMSX

                case Field.DatItem_Original:
                    if (negate)
                        Original.NegativeSet.Add(value);
                    else
                        Original.PositiveSet.Add(value);
                    break;

                case Field.DatItem_OpenMSXSubType:
                    if (negate)
                        SubType.Negative |= value.AsOpenMSXSubType();
                    else
                        SubType.Positive |= value.AsOpenMSXSubType();
                    break;

                case Field.DatItem_OpenMSXType:
                    if (negate)
                        OpenMSXType.NegativeSet.Add(value);
                    else
                        OpenMSXType.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Remark:
                    if (negate)
                        Remark.NegativeSet.Add(value);
                    else
                        Remark.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Boot:
                    if (negate)
                        Boot.NegativeSet.Add(value);
                    else
                        Boot.PositiveSet.Add(value);
                    break;

                #endregion

                #region SoftwareList

                case Field.DatItem_Part_Name:
                    if (negate)
                        PartName.NegativeSet.Add(value);
                    else
                        PartName.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Part_Interface:
                    if (negate)
                        PartInterface.NegativeSet.Add(value);
                    else
                        PartInterface.PositiveSet.Add(value);
                    break;

                case Field.DatItem_AreaName:
                    if (negate)
                        AreaName.NegativeSet.Add(value);
                    else
                        AreaName.PositiveSet.Add(value);
                    break;

                case Field.DatItem_AreaSize:
                    bool? asOperation = null;
                    if (value.StartsWith(">"))
                        asOperation = true;
                    else if (value.StartsWith("<"))
                        asOperation = false;
                    else if (value.StartsWith("="))
                        asOperation = null;

                    string areasizeString = value.TrimStart('>', '<', '=');
                    if (!Int64.TryParse(areasizeString, out long areasize))
                        return;

                    // Equal
                    if (asOperation == null && !negate)
                    {
                        AreaSize.Neutral = areasize;
                    }

                    // Not Equal
                    else if (asOperation == null && negate)
                    {
                        AreaSize.Negative = areasize - 1;
                        AreaSize.Positive = areasize + 1;
                    }

                    // Greater Than or Equal
                    else if (asOperation == true && !negate)
                    {
                        AreaSize.Positive = areasize;
                    }

                    // Strictly Less Than
                    else if (asOperation == true && negate)
                    {
                        AreaSize.Negative = areasize - 1;
                    }

                    // Less Than or Equal
                    else if (asOperation == false && !negate)
                    {
                        AreaSize.Negative = areasize;
                    }

                    // Strictly Greater Than
                    else if (asOperation == false && negate)
                    {
                        AreaSize.Positive = areasize + 1;
                    }

                    break;

                case Field.DatItem_AreaWidth:
                    if (negate)
                        AreaWidth.NegativeSet.Add(value);
                    else
                        AreaWidth.PositiveSet.Add(value);
                    break;

                case Field.DatItem_AreaEndianness:
                    if (negate)
                        AreaEndianness.NegativeSet.Add(value);
                    else
                        AreaEndianness.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Value:
                    if (negate)
                        Value.NegativeSet.Add(value);
                    else
                        Value.PositiveSet.Add(value);
                    break;

                case Field.DatItem_LoadFlag:
                    if (negate)
                        LoadFlag.NegativeSet.Add(value);
                    else
                        LoadFlag.PositiveSet.Add(value);
                    break;

                #endregion

                case Field.DatItem_Default:
                    if (negate || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                        Default.Neutral = false;
                    else
                        Default.Neutral = true;
                    break;

                case Field.DatItem_Description:
                    if (negate)
                        Description.NegativeSet.Add(value);
                    else
                        Description.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Size:
                    bool? sOperation = null;
                    if (value.StartsWith(">"))
                        sOperation = true;
                    else if (value.StartsWith("<"))
                        sOperation = false;
                    else if (value.StartsWith("="))
                        sOperation = null;

                    string sizeString = value.TrimStart('>', '<', '=');
                    if (!Int64.TryParse(sizeString, out long size))
                        return;

                    // Equal
                    if (sOperation == null && !negate)
                    {
                        Size.Neutral = size;
                    }

                    // Not Equal
                    else if (sOperation == null && negate)
                    {
                        Size.Negative = size - 1;
                        Size.Positive = size + 1;
                    }

                    // Greater Than or Equal
                    else if (sOperation == true && !negate)
                    {
                        Size.Positive = size;
                    }

                    // Strictly Less Than
                    else if (sOperation == true && negate)
                    {
                        Size.Negative = size - 1;
                    }

                    // Less Than or Equal
                    else if (sOperation == false && !negate)
                    {
                        Size.Negative = size;
                    }

                    // Strictly Greater Than
                    else if (sOperation == false && negate)
                    {
                        Size.Positive = size + 1;
                    }

                    break;

                case Field.DatItem_CRC:
                    if (negate)
                        CRC.NegativeSet.Add(value);
                    else
                        CRC.PositiveSet.Add(value);
                    break;

                case Field.DatItem_MD5:
                    if (negate)
                        MD5.NegativeSet.Add(value);
                    else
                        MD5.PositiveSet.Add(value);
                    break;

#if NET_FRAMEWORK
                case Field.DatItem_RIPEMD160:
                    if (negate)
                        RIPEMD160.NegativeSet.Add(value);
                    else
                        RIPEMD160.PositiveSet.Add(value);
                    break;
#endif

                case Field.DatItem_SHA1:
                    if (negate)
                        SHA1.NegativeSet.Add(value);
                    else
                        SHA1.PositiveSet.Add(value);
                    break;

                case Field.DatItem_SHA256:
                    if (negate)
                        SHA256.NegativeSet.Add(value);
                    else
                        SHA256.PositiveSet.Add(value);
                    break;

                case Field.DatItem_SHA384:
                    if (negate)
                        SHA384.NegativeSet.Add(value);
                    else
                        SHA384.PositiveSet.Add(value);
                    break;

                case Field.DatItem_SHA512:
                    if (negate)
                        SHA512.NegativeSet.Add(value);
                    else
                        SHA512.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Merge:
                    if (negate)
                        MergeTag.NegativeSet.Add(value);
                    else
                        MergeTag.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Region:
                    if (negate)
                        Region.NegativeSet.Add(value);
                    else
                        Region.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Index:
                    if (negate)
                        Index.NegativeSet.Add(value);
                    else
                        Index.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Writable:
                    if (negate || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                        Writable.Neutral = false;
                    else
                        Writable.Neutral = true;
                    break;

                case Field.DatItem_Optional:
                    if (negate || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                        Optional.Neutral = false;
                    else
                        Optional.Neutral = true;
                    break;

                case Field.DatItem_Status:
                    if (negate)
                        Status.Negative |= value.AsItemStatus();
                    else
                        Status.Positive |= value.AsItemStatus();
                    break;

                case Field.DatItem_Language:
                    if (negate)
                        Language.NegativeSet.Add(value);
                    else
                        Language.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Date:
                    if (negate)
                        Date.NegativeSet.Add(value);
                    else
                        Date.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Bios:
                    if (negate)
                        Bios.NegativeSet.Add(value);
                    else
                        Bios.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Offset:
                    if (negate)
                        Offset.NegativeSet.Add(value);
                    else
                        Offset.PositiveSet.Add(value);
                    break;

                case Field.DatItem_Inverted:
                    if (negate || value.Equals("false", StringComparison.OrdinalIgnoreCase))
                        Inverted.Neutral = false;
                    else
                        Inverted.Neutral = true;
                    break;

                    #endregion // DatItem Filters
            }
        }

        #endregion

        #endregion // Instance Methods
    }
}
