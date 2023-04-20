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
        public FilterItem<string> NoIntroId { get; private set; } = new FilterItem<string>();
        public FilterItem<string> NoIntroCloneOfId { get; private set; } = new FilterItem<string>();

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
        
        /// <summary>
        /// Determines if any filters have been set
        /// </summary>
        public bool HasFilters { get; set; } = false;

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

        #region Population

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

                case MachineField.ID:
                    SetStringFilter(NoIntroId, value, negate);
                    break;

                case MachineField.CloneOfID:
                    SetStringFilter(NoIntroCloneOfId, value, negate);
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
    
        #region Running

        /// <summary>
        /// Check to see if a Machine passes the filters
        /// </summary>
        /// <param name="machine">Machine to check</param>
        /// <returns>True if the machine passed the filter, false otherwise</returns>
        public bool PassesFilters(Machine machine)
        {
            if (machine == null)
                return false;

            #region Common

            // Machine_Name
            bool passes = PassStringFilter(Name, machine.Name);
            if (IncludeOfInGame)
            {
                passes |= PassStringFilter(Name, machine.CloneOf);
                passes |= PassStringFilter(Name, machine.RomOf);
            }
            if (!passes)
                return false;

            // Machine_Comment
            if (!PassStringFilter(Comment, machine.Comment))
                return false;

            // Machine_Description
            if (!PassStringFilter(Description, machine.Description))
                return false;

            // Machine_Year
            if (!PassStringFilter(Year, machine.Year))
                return false;

            // Machine_Manufacturer
            if (!PassStringFilter(Manufacturer, machine.Manufacturer))
                return false;

            // Machine_Publisher
            if (!PassStringFilter(Publisher, machine.Publisher))
                return false;

            // Machine_Category
            if (!PassStringFilter(Category, machine.Category))
                return false;

            // Machine_RomOf
            if (!PassStringFilter(RomOf, machine.RomOf))
                return false;

            // Machine_CloneOf
            if (!PassStringFilter(CloneOf, machine.CloneOf))
                return false;

            // Machine_SampleOf
            if (!PassStringFilter(SampleOf, machine.SampleOf))
                return false;

            // Machine_Type
            if (Type.MatchesPositive(0x0, machine.MachineType) == false)
                return false;
            if (Type.MatchesNegative(0x0, machine.MachineType) == true)
                return false;

            #endregion

            #region AttractMode

            // Machine_Players
            if (!PassStringFilter(Players, machine.Players))
                return false;

            // Machine_Rotation
            if (!PassStringFilter(Rotation, machine.Rotation))
                return false;

            // Machine_Control
            if (!PassStringFilter(Control, machine.Control))
                return false;

            // Machine_Status
            if (!PassStringFilter(Status, machine.Status))
                return false;

            // Machine_DisplayCount
            if (!PassStringFilter(DisplayCount, machine.DisplayCount))
                return false;

            // Machine_DisplayType
            if (!PassStringFilter(DisplayType, machine.DisplayType))
                return false;

            // Machine_Buttons
            if (!PassStringFilter(Buttons, machine.Buttons))
                return false;

            #endregion

            #region ListXML

            // Machine_History
            if (!PassStringFilter(History, machine.History))
                return false;

            // Machine_SourceFile
            if (!PassStringFilter(SourceFile, machine.SourceFile))
                return false;

            // Machine_Runnable
            if (Runnable.MatchesPositive(Core.Runnable.NULL, machine.Runnable) == false)
                return false;
            if (Runnable.MatchesNegative(Core.Runnable.NULL, machine.Runnable) == true)
                return false;

            #endregion

            #region Logiqx

            // Machine_Board
            if (!PassStringFilter(Board, machine.Board))
                return false;

            // Machine_RebuildTo
            if (!PassStringFilter(RebuildTo, machine.RebuildTo))
                return false;

            // Machine_NoIntroId
            if (!PassStringFilter(NoIntroId, machine.NoIntroId))
                return false;

            // Machine_NoIntroCloneOfId
            if (!PassStringFilter(NoIntroCloneOfId, value: machine.NoIntroCloneOfId))
                return false;


            #endregion

            #region Logiqx EmuArc

            // Machine_TitleID
            if (!PassStringFilter(TitleID, machine.TitleID))
                return false;

            // Machine_Developer
            if (!PassStringFilter(Developer, machine.Developer))
                return false;

            // Machine_Genre
            if (!PassStringFilter(Genre, machine.Genre))
                return false;

            // Machine_Subgenre
            if (!PassStringFilter(Subgenre, machine.Subgenre))
                return false;

            // Machine_Ratings
            if (!PassStringFilter(Ratings, machine.Ratings))
                return false;

            // Machine_Score
            if (!PassStringFilter(Score, machine.Score))
                return false;

            // Machine_Enabled
            if (!PassStringFilter(Enabled, machine.Enabled))
                return false;

            // Machine_CRC
            if (!PassBoolFilter(CRC, machine.Crc))
                return false;

            // Machine_RelatedTo
            if (!PassStringFilter(RelatedTo, machine.RelatedTo))
                return false;

            #endregion

            #region OpenMSX

            // Machine_GenMSXID
            if (!PassStringFilter(GenMSXID, machine.GenMSXID))
                return false;

            // Machine_System
            if (!PassStringFilter(System, machine.System))
                return false;

            // Machine_Country
            if (!PassStringFilter(Country, machine.Country))
                return false;

            #endregion

            #region SoftwareList

            // Machine_Supported
            if (Supported.MatchesPositive(Core.Supported.NULL, machine.Supported) == false)
                return false;
            if (Supported.MatchesNegative(Core.Supported.NULL, machine.Supported) == true)
                return false;

            #endregion // SoftwareList

            return true;
        }

        #endregion
    }
}
