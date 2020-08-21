using SabreTools.Library.DatFiles;
using SabreTools.Library.DatItems;
using SabreTools.Library.Reports;

namespace SabreTools.Library.Tools
{
    public static class Converters
    {
        /// <summary>
        /// Get DatFormat value from input string
        /// </summary>
        /// <param name="input">String to get value from</param>
        /// <returns>DatFormat value corresponding to the string</returns>
        public static DatFormat AsDatFormat(this string input)
        {
            switch (input?.Trim().ToLowerInvariant())
            {
                case "all":
                    return DatFormat.ALL;
                case "am":
                case "attractmode":
                    return DatFormat.AttractMode;
                case "cmp":
                case "clrmamepro":
                    return DatFormat.ClrMamePro;
                case "csv":
                    return DatFormat.CSV;
                case "dc":
                case "doscenter":
                    return DatFormat.DOSCenter;
                case "json":
                    return DatFormat.Json;
                case "lr":
                case "listrom":
                    return DatFormat.Listrom;
                case "lx":
                case "listxml":
                    return DatFormat.Listxml;
                case "md5":
                    return DatFormat.RedumpMD5;
                case "miss":
                case "missfile":
                    return DatFormat.MissFile;
                case "msx":
                case "openmsx":
                    return DatFormat.OpenMSX;
                case "ol":
                case "offlinelist":
                    return DatFormat.OfflineList;
                case "rc":
                case "romcenter":
                    return DatFormat.RomCenter;
#if NET_FRAMEWORK
                case "ripemd160":
                    return DatFormat.RedumpRIPEMD160;
#endif
                case "sd":
                case "sabredat":
                    return DatFormat.SabreDat;
                case "sfv":
                    return DatFormat.RedumpSFV;
                case "sha1":
                    return DatFormat.RedumpSHA1;
                case "sha256":
                    return DatFormat.RedumpSHA256;
                case "sha384":
                    return DatFormat.RedumpSHA384;
                case "sha512":
                    return DatFormat.RedumpSHA512;
                case "sl":
                case "softwarelist":
                    return DatFormat.SoftwareList;
                case "smdb":
                case "everdrive":
                    return DatFormat.EverdriveSMDB;
                case "ssv":
                    return DatFormat.SSV;
                case "tsv":
                    return DatFormat.TSV;
                case "xml":
                case "logiqx":
                    return DatFormat.Logiqx;
                default:
                    return 0x0;
            }
        }

        /// <summary>
        /// Get the field associated with each hash type
        /// </summary>
        public static Field AsField(this Hash hash)
        {
            switch (hash)
            {
                case Hash.CRC:
                    return Field.CRC;
                case Hash.MD5:
                    return Field.MD5;
#if NET_FRAMEWORK
                case Hash.RIPEMD160:
                    return Field.RIPEMD160;
#endif
                case Hash.SHA1:
                    return Field.SHA1;
                case Hash.SHA256:
                    return Field.SHA256;
                case Hash.SHA384:
                    return Field.SHA384;
                case Hash.SHA512:
                    return Field.SHA512;

                default:
                    return Field.NULL;
            }
        }

        /// <summary>
        /// Get Field value from input string
        /// </summary>
        /// <param name="input">String to get value from</param>
        /// <returns>Field value corresponding to the string</returns>
        public static Field AsField(this string input)
        {
            switch (input?.ToLowerInvariant())
            {
                #region Machine

                #region Common

                case "game":
                case "gamename":
                case "game-name":
                case "machine":
                case "machinename":
                case "machine-name":
                    return Field.MachineName;

                case "comment":
                case "extra":
                    return Field.Comment;

                case "desc":
                case "description":
                case "gamedesc":
                case "gamedescription":
                case "game-description":
                case "game description":
                case "machinedesc":
                case "machinedescription":
                case "machine-description":
                case "machine description":
                    return Field.Description;

                case "year":
                    return Field.Year;

                case "manufacturer":
                    return Field.Manufacturer;

                case "publisher":
                    return Field.Publisher;

                case "category":
                case "gamecategory":
                case "game-category":
                case "machinecategory":
                case "machine-category":
                    return Field.Category;

                case "romof":
                    return Field.RomOf;

                case "cloneof":
                    return Field.CloneOf;

                case "sampleof":
                    return Field.SampleOf;

                case "gametype":
                case "game type":
                case "game-type":
                case "machinetype":
                case "machine type":
                case "machine-type":
                    return Field.MachineType;

                #endregion

                #region AttractMode

                case "players":
                    return Field.Players;

                case "rotation":
                    return Field.Rotation;

                case "control":
                    return Field.Control;

                case "amstatus":
                case "am-status":
                case "gamestatus":
                case "game-status":
                case "machinestatus":
                case "machine-status":
                case "supportstatus":
                case "support-status":
                    return Field.SupportStatus;

                case "displaycount":
                case "display-count":
                case "displays":
                    return Field.DisplayCount;

                case "displaytype":
                case "display-type":
                    return Field.DisplayType;

                case "buttons":
                    return Field.Buttons;

                #endregion

                #region ListXML

                case "sourcefile":
                case "source file":
                case "source-file":
                    return Field.SourceFile;

                case "runnable":
                    return Field.Runnable;

                case "devices":
                    return Field.Devices;

                case "slotoptions":
                case "slot options":
                case "slot-options":
                    return Field.SlotOptions;

                case "infos":
                    return Field.Infos;

                #endregion

                #region Logiqx

                case "board":
                    return Field.Board;

                case "rebuildto":
                case "rebuild to":
                case "rebuild-to":
                    return Field.RebuildTo;

                #endregion

                #region SoftwareList

                case "supported":
                    return Field.Supported;

                #endregion

                #endregion // Machine

                #region DatItem

                case "areaname":
                case "area-name":
                    return Field.AreaName;
                case "areasize":
                case "area-size":
                    return Field.AreaSize;
                case "bios":
                    return Field.Bios;
                case "biosdescription":
                case "bios-description":
                case "biossetdescription":
                case "biosset-description":
                case "bios-set-description":
                    return Field.BiosDescription;
                case "crc":
                case "crc32":
                    return Field.CRC;
                case "default":
                    return Field.Default;
                case "date":
                    return Field.Date;
                case "equal":
                case "greater":
                case "less":
                case "size":
                    return Field.Size;
                case "features":
                    return Field.Features;
                case "index":
                    return Field.Index;
                case "inverted":
                    return Field.Inverted;
                case "itemname":
                case "item-name":
                case "name":
                    return Field.Name;
                case "itemtatus":
                case "item-status":
                case "status":
                    return Field.Status;
                case "itemtype":
                case "item-type":
                case "type":
                    return Field.ItemType;
                case "language":
                    return Field.Language;
                case "md5":
                    return Field.MD5;
                case "merge":
                case "mergetag":
                case "merge-tag":
                    return Field.Merge;
                case "offset":
                    return Field.Offset;
                case "optional":
                    return Field.Optional;
                case "partinterface":
                case "part-interface":
                    return Field.PartInterface;
                case "partname":
                case "part-name":
                    return Field.PartName;
                case "region":
                    return Field.Region;
#if NET_FRAMEWORK
                case "ripemd160":
                    return Field.RIPEMD160;
#endif
                case "sha1":
                case "sha-1":
                    return Field.SHA1;
                case "sha256":
                case "sha-256":
                    return Field.SHA256;
                case "sha384":
                case "sha-384":
                    return Field.SHA384;
                case "sha512":
                case "sha-512":
                    return Field.SHA512;
                case "writable":
                    return Field.Writable;

                #endregion

                default:
                    return Field.NULL;
            }
        }

        /// <summary>
        /// Get ItemStatus value from input string
        /// </summary>
        /// <param name="status">String to get value from</param>
        /// <returns>ItemStatus value corresponding to the string</returns>
        public static ItemStatus AsItemStatus(this string status)
        {
#if NET_FRAMEWORK
            switch (status?.ToLowerInvariant())
            {
                case "good":
                    return ItemStatus.Good;
                case "baddump":
                    return ItemStatus.BadDump;
                case "nodump":
                case "yes":
                    return ItemStatus.Nodump;
                case "verified":
                    return ItemStatus.Verified;
                case "none":
                case "no":
                default:
                    return ItemStatus.None;
            }
#else
            return status?.ToLowerInvariant() switch
            {
                "good" => ItemStatus.Good,
                "baddump" => ItemStatus.BadDump,
                "nodump" => ItemStatus.Nodump,
                "yes" => ItemStatus.Nodump,
                "verified" => ItemStatus.Verified,
                "none" => ItemStatus.None,
                "no" => ItemStatus.None,
                _ => ItemStatus.None,
            };
#endif
        }

        /// <summary>
        /// Get ItemType? value from input string
        /// </summary>
        /// <param name="itemType">String to get value from</param>
        /// <returns>ItemType? value corresponding to the string</returns>
        public static ItemType? AsItemType(this string itemType)
        {
#if NET_FRAMEWORK
            switch (itemType?.ToLowerInvariant())
            {
                case "archive":
                    return ItemType.Archive;
                case "biosset":
                    return ItemType.BiosSet;
                case "blank":
                    return ItemType.Blank;
                case "disk":
                    return ItemType.Disk;
                case "release":
                    return ItemType.Release;
                case "rom":
                    return ItemType.Rom;
                case "sample":
                    return ItemType.Sample;
                default:
                    return null;
            }
#else
            return itemType?.ToLowerInvariant() switch
            {
                "archive" => ItemType.Archive,
                "biosset" => ItemType.BiosSet,
                "blank" => ItemType.Blank,
                "disk" => ItemType.Disk,
                "release" => ItemType.Release,
                "rom" => ItemType.Rom,
                "sample" => ItemType.Sample,
                _ => null,
            };
#endif
        }

        /// <summary>
        /// Get MachineType value from input string
        /// </summary>
        /// <param name="gametype">String to get value from</param>
        /// <returns>MachineType value corresponding to the string</returns>
        public static MachineType AsMachineType(this string gametype)
        {
#if NET_FRAMEWORK
            switch (gametype?.ToLowerInvariant())
            {
                case "bios":
                    return MachineType.Bios;
                case "dev":
                case "device":
                    return MachineType.Device;
                case "mech":
                case "mechanical":
                    return MachineType.Mechanical;
                case "none":
                default:
                    return MachineType.None;
            }
#else
            return gametype?.ToLowerInvariant() switch
            {
                "bios" => MachineType.Bios,
                "dev" => MachineType.Device,
                "device" => MachineType.Device,
                "mech" => MachineType.Mechanical,
                "mechanical" => MachineType.Mechanical,
                "none" => MachineType.None,
                _ => MachineType.None,
            };
#endif
        }

        /// <summary>
        /// Get MergingFlag value from input string
        /// </summary>
        /// <param name="merging">String to get value from</param>
        /// <returns>MergingFlag value corresponding to the string</returns>
        public static MergingFlag AsMergingFlag(this string merging)
        {
#if NET_FRAMEWORK
            switch (merging?.ToLowerInvariant())
            {
                case "split":
                    return MergingFlag.Split;
                case "merged":
                    return MergingFlag.Merged;
                case "nonmerged":
                case "unmerged":
                    return MergingFlag.NonMerged;
                case "full":
                    return MergingFlag.Full;
                case "none":
                default:
                    return MergingFlag.None;
            }
#else
            return merging?.ToLowerInvariant() switch
            {
                "split" => MergingFlag.Split,
                "merged" => MergingFlag.Merged,
                "nonmerged" => MergingFlag.NonMerged,
                "unmerged" => MergingFlag.NonMerged,
                "full" => MergingFlag.Full,
                "none" => MergingFlag.None,
                _ => MergingFlag.None,
            };
#endif
        }

        /// <summary>
        /// Get NodumpFlag value from input string
        /// </summary>
        /// <param name="nodump">String to get value from</param>
        /// <returns>NodumpFlag value corresponding to the string</returns>
        public static NodumpFlag AsNodumpFlag(this string nodump)
        {
#if NET_FRAMEWORK
            switch (nodump?.ToLowerInvariant())
            {
                case "obsolete":
                    return NodumpFlag.Obsolete;
                case "required":
                    return NodumpFlag.Required;
                case "ignore":
                    return NodumpFlag.Ignore;
                case "none":
                default:
                    return NodumpFlag.None;
            }
#else
            return nodump?.ToLowerInvariant() switch
            {
                "obsolete" => NodumpFlag.Obsolete,
                "required" => NodumpFlag.Required,
                "ignore" => NodumpFlag.Ignore,
                "none" => NodumpFlag.None,
                _ => NodumpFlag.None,
            };
#endif
        }

        /// <summary>
        /// Get PackingFlag value from input string
        /// </summary>
        /// <param name="packing">String to get value from</param>
        /// <returns>PackingFlag value corresponding to the string</returns>
        public static PackingFlag AsPackingFlag(this string packing)
        {
#if NET_FRAMEWORK
            switch (packing?.ToLowerInvariant())
            {
                case "yes":
                case "zip":
                    return PackingFlag.Zip;
                case "no":
                case "unzip":
                    return PackingFlag.Unzip;
                case "none":
                default:
                    return PackingFlag.None;
            }
#else
            return packing?.ToLowerInvariant() switch
            {
                "yes" => PackingFlag.Zip,
                "zip" => PackingFlag.Zip,
                "no" => PackingFlag.Unzip,
                "unzip" => PackingFlag.Unzip,
                "none" => PackingFlag.None,
                _ => PackingFlag.None,
            };
#endif
        }

        /// <summary>
        /// Get SplitType value from input ForceMerging
        /// </summary>
        /// <param name="forceMerging">ForceMerging to get value from</param>
        /// <returns>SplitType value corresponding to the string</returns>
        public static SplitType AsSplitType(this MergingFlag forceMerging)
        {
#if NET_FRAMEWORK
            switch (forceMerging)
            {
                case MergingFlag.Split:
                    return SplitType.Split;
                case MergingFlag.Merged:
                    return SplitType.Merged;
                case MergingFlag.NonMerged:
                    return SplitType.NonMerged;
                case MergingFlag.Full:
                    return SplitType.FullNonMerged;
                case MergingFlag.None:
                default:
                    return SplitType.None;
            }
#else
            return forceMerging switch
            {
                MergingFlag.Split => SplitType.Split,
                MergingFlag.Merged => SplitType.Merged,
                MergingFlag.NonMerged => SplitType.NonMerged,
                MergingFlag.Full => SplitType.FullNonMerged,
                MergingFlag.None => SplitType.None,
                _ => SplitType.None,
            };
#endif
        }

        /// <summary>
        /// Get StatReportFormat value from input string
        /// </summary>
        /// <param name="input">String to get value from</param>
        /// <returns>StatReportFormat value corresponding to the string</returns>
        public static StatReportFormat AsStatReportFormat(this string input)
        {
#if NET_FRAMEWORK
            switch (input?.Trim().ToLowerInvariant())
            {
                case "all":
                    return StatReportFormat.All;
                case "csv":
                    return StatReportFormat.CSV;
                case "html":
                    return StatReportFormat.HTML;
                case "ssv":
                    return StatReportFormat.SSV;
                case "text":
                    return StatReportFormat.Textfile;
                case "tsv":
                    return StatReportFormat.TSV;
                default:
                    return 0x0;
            }
#else
            return input?.Trim().ToLowerInvariant() switch
            {
                "all" => StatReportFormat.All,
                "csv" => StatReportFormat.CSV,
                "html" => StatReportFormat.HTML,
                "ssv" => StatReportFormat.SSV,
                "text" => StatReportFormat.Textfile,
                "tsv" => StatReportFormat.TSV,
                _ => 0x0,
            };
#endif
        }

        /// <summary>
        /// Get bool? value from input string
        /// </summary>
        /// <param name="yesno">String to get value from</param>
        /// <returns>bool? corresponding to the string</returns>
        public static bool? AsYesNo(this string yesno)
        {
#if NET_FRAMEWORK
            switch (yesno?.ToLowerInvariant())
            {
                case "yes":
                case "true":
                    return true;
                case "no":
                case "false":
                    return false;
                case "partial":
                default:
                    return null;
            }
#else
            return yesno?.ToLowerInvariant() switch
            {
                "yes" => true,
                "true" => true,
                "no" => false,
                "false" => false,
                "partial" => null,
                _ => null,
            };
#endif
        }
    }
}
