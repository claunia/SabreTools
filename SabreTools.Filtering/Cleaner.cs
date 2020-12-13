using System.Collections.Generic;

using SabreTools.Core;

namespace SabreTools.Filtering
{
    /// <summary>
    /// Represents the cleaning operations that need to be performed on a set of items, usually a DAT
    /// </summary>
    public class Cleaner
    {
        #region Filter Fields

        /// <summary>
        /// Filter for DatHeader fields
        /// </summary>
        public DatHeaderFilter DatHeaderFilter { get; set; }

        /// <summary>
        /// Filter for DatItem fields
        /// </summary>
        public DatItemFilter DatItemFilter { get; set; }

        /// <summary>
        /// Filter for Machine fields
        /// </summary>
        public MachineFilter MachineFilter { get; set; }

        #endregion

        #region Flag Fields

        /// <summary>
        /// Clean all names to WoD standards
        /// </summary>
        public bool Clean { get; set; }

        /// <summary>
        /// Deduplicate items using the given method
        /// </summary>
        public DedupeType DedupeRoms { get; set; }

        /// <summary>
        /// Set Machine Description from Machine Name
        /// </summary>
        public bool DescriptionAsName { get; set; }

        /// <summary>
        /// Dictionary of DatHeader fields to exclude from writing
        /// </summary>
        public List<DatHeaderField> ExcludeDatHeaderFields { get; set; } = new List<DatHeaderField>();

        /// <summary>
        /// Dictionary of DatItem fields to exclude from writing
        /// </summary>
        public List<DatItemField> ExcludeDatItemFields { get; set; } = new List<DatItemField>();

        /// <summary>
        /// Dictionary of Machine fields to exclude from writing
        /// </summary>
        public List<MachineField> ExcludeMachineFields { get; set; } = new List<MachineField>();

        /// <summary>
        /// Keep machines that don't contain any items
        /// </summary>
        public bool KeepEmptyGames { get; set; }

        /// <summary>
        /// Enable "One Rom, One Region (1G1R)" mode
        /// </summary>
        public bool OneGamePerRegion { get; set; }

        /// <summary>
        /// Ordered list of regions for "One Rom, One Region (1G1R)" mode
        /// </summary>
        public List<string> RegionList { get; set; }

        /// <summary>
        /// Ensure each rom is in their own game
        /// </summary>
        public bool OneRomPerGame { get; set; }

        /// <summary>
        /// Remove all unicode characters
        /// </summary>
        public bool RemoveUnicode { get; set; }

        /// <summary>
        /// Include root directory when determing trim sizes
        /// </summary>
        public string Root { get; set; }

        /// <summary>
        /// Remove scene dates from the beginning of machine names
        /// </summary>
        public bool SceneDateStrip { get; set; }

        /// <summary>
        /// Change all machine names to "!"
        /// </summary>
        public bool Single { get; set; }

        /// <summary>
        /// Trim total machine and item name to not exceed NTFS limits
        /// </summary>
        public bool Trim { get; set; }
    
        #endregion
    }
}
