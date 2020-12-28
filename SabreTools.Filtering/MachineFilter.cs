using System.Collections.Generic;

using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatItems;
using SabreTools.Logging;

namespace SabreTools.Filtering
{
    /// <summary>
    /// Represents the filtering operations that need to be performed on a set of items, usually a DAT
    /// </summary>
    public class MachineFilter : Filter
    {
        #region Fields

        #region Filters

        #region Common

        public FilterItem<string> Name { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Comment { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Description { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Year { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Manufacturer { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Publisher { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Category { get; private set; } = new FilterItem<string>();
        public FilterItem<string> RomOf { get; private set; } = new FilterItem<string>();
        public FilterItem<string> CloneOf { get; private set; } = new FilterItem<string>();
        public FilterItem<string> SampleOf { get; private set; } = new FilterItem<string>();
        public FilterItem<MachineType> Type { get; private set; } = new FilterItem<MachineType>() { Positive = 0x0, Negative = 0x0 };

        #endregion

        #region AttractMode

        public FilterItem<string> Players { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Rotation { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Control { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Status { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DisplayCount { get; private set; } = new FilterItem<string>();
        public FilterItem<string> DisplayType { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Buttons { get; private set; } = new FilterItem<string>();

        #endregion

        #region ListXML

        public FilterItem<string> History { get; private set; } = new FilterItem<string>();
        public FilterItem<string> SourceFile { get; private set; } = new FilterItem<string>();
        public FilterItem<Runnable> Runnable { get; private set; } = new FilterItem<Runnable>() { Positive = Core.Runnable.NULL, Negative = Core.Runnable.NULL };

        #endregion

        #region Logiqx

        public FilterItem<string> Board { get; private set; } = new FilterItem<string>();
        public FilterItem<string> RebuildTo { get; private set; } = new FilterItem<string>();

        #endregion

        #region Logiqx EmuArc

        public FilterItem<string> TitleID { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Developer { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Genre { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Subgenre { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Ratings { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Score { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Enabled { get; private set; } = new FilterItem<string>();
        public FilterItem<bool?> CRC { get; private set; } = new FilterItem<bool?>() { Neutral = null };
        public FilterItem<string> RelatedTo { get; private set; } = new FilterItem<string>();

        #endregion

        #region OpenMSX

        public FilterItem<string> GenMSXID { get; private set; } = new FilterItem<string>();
        public FilterItem<string> System { get; private set; } = new FilterItem<string>();
        public FilterItem<string> Country { get; private set; } = new FilterItem<string>();

        #endregion

        #region SoftwareList

        public FilterItem<Supported> Supported { get; private set; } = new FilterItem<Supported>() { Positive = Core.Supported.NULL, Negative = Core.Supported.NULL };

        #endregion

        #endregion // Machine Filters

        #region Additional Flags

        /// <summary>
        /// Include romof and cloneof when filtering machine names
        /// </summary>
        public bool IncludeOfInGame { get; set; }

        #endregion

        #endregion // Fields

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public MachineFilter()
        {
            logger = new Logger(this);
        }

        #endregion

        #region Filter Population

        /// <summary>
        /// Set multiple filters from key
        /// </summary>
        /// <param name="key">Key for the filter to be set</param>
        /// <param name="values">List of values for the filter</param>
        /// <param name="negate">True if negative filter, false otherwise</param>
        public void SetFilter(MachineField key, List<string> values, bool negate)
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
        public void SetFilter(MachineField key, string value, bool negate)
        {
            switch (key)
            {
                #region Common

                case MachineField.Name:
                    SetStringFilter(Name, value, negate);
                    break;

                case MachineField.Comment:
                    SetStringFilter(Comment, value, negate);
                    break;

                case MachineField.Description:
                    SetStringFilter(Description, value, negate);
                    break;

                case MachineField.Year:
                    SetStringFilter(Year, value, negate);
                    break;

                case MachineField.Manufacturer:
                    SetStringFilter(Manufacturer, value, negate);
                    break;

                case MachineField.Publisher:
                    SetStringFilter(Publisher, value, negate);
                    break;

                case MachineField.Category:
                    SetStringFilter(Category, value, negate);
                    break;

                case MachineField.RomOf:
                    SetStringFilter(RomOf, value, negate);
                    break;

                case MachineField.CloneOf:
                    SetStringFilter(CloneOf, value, negate);
                    break;

                case MachineField.SampleOf:
                    SetStringFilter(SampleOf, value, negate);
                    break;

                case MachineField.Type:
                    if (negate)
                        Type.Negative |= value.AsMachineType();
                    else
                        Type.Positive |= value.AsMachineType();
                    break;

                #endregion

                #region AttractMode

                case MachineField.Players:
                    SetStringFilter(Players, value, negate);
                    break;

                case MachineField.Rotation:
                    SetStringFilter(Rotation, value, negate);
                    break;

                case MachineField.Control:
                    SetStringFilter(Control, value, negate);
                    break;

                case MachineField.Status:
                    SetStringFilter(Status, value, negate);
                    break;

                case MachineField.DisplayCount:
                    SetStringFilter(DisplayCount, value, negate);
                    break;

                case MachineField.DisplayType:
                    SetStringFilter(DisplayType, value, negate);
                    break;

                case MachineField.Buttons:
                    SetStringFilter(Buttons, value, negate);
                    break;

                #endregion

                #region ListXML

                case MachineField.History:
                    SetStringFilter(History, value, negate);
                    break;

                case MachineField.SourceFile:
                    SetStringFilter(SourceFile, value, negate);
                    break;

                case MachineField.Runnable:
                    if (negate)
                        Runnable.Negative |= value.AsRunnable();
                    else
                        Runnable.Positive |= value.AsRunnable();
                    break;          

                #endregion

                #region Logiqx

                case MachineField.Board:
                    SetStringFilter(Board, value, negate);
                    break;

                case MachineField.RebuildTo:
                    SetStringFilter(RebuildTo, value, negate);
                    break;

                #endregion

                #region Logiqx EmuArc

                case MachineField.TitleID:
                    SetStringFilter(TitleID, value, negate);
                    break;

                case MachineField.Developer:
                    SetStringFilter(Developer, value, negate);
                    break;

                case MachineField.Genre:
                    SetStringFilter(Genre, value, negate);
                    break;

                case MachineField.Subgenre:
                    SetStringFilter(Subgenre, value, negate);
                    break;

                case MachineField.Ratings:
                    SetStringFilter(Ratings, value, negate);
                    break;

                case MachineField.Score:
                    SetStringFilter(Score, value, negate);
                    break;

                case MachineField.Enabled:
                    SetStringFilter(Enabled, value, negate);
                    break;

                case MachineField.CRC:
                    SetBooleanFilter(CRC, value, negate);
                    break;

                case MachineField.RelatedTo:
                    SetStringFilter(RelatedTo, value, negate);
                    break;

                #endregion

                #region OpenMSX

                case MachineField.GenMSXID:
                    SetStringFilter(GenMSXID, value, negate);
                    break;

                case MachineField.System:
                    SetStringFilter(System, value, negate);
                    break;

                case MachineField.Country:
                    SetStringFilter(Country, value, negate);
                    break;

                #endregion

                #region SoftwareList

                case MachineField.Supported:
                    if (negate)
                        Supported.Negative |= value.AsSupported();
                    else
                        Supported.Positive |= value.AsSupported();
                    break;

                #endregion
            }
        }

        #endregion
    }
}
