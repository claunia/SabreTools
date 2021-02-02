using System.Xml.Serialization;

using SabreTools.Core;
using Newtonsoft.Json;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents a blank set from an input DAT
    /// </summary>
    [JsonObject("blank"), XmlRoot("blank")]
    public class Blank : DatItem
    {
        #region Constructors

        /// <summary>
        /// Create a default, empty Archive object
        /// </summary>
        public Blank()
        {
            ItemType = ItemType.Blank;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new Blank()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a blank, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Blank
            Blank newOther = other as Blank;

            // If the archive information matches
            return (Machine == newOther.Machine);
        }

        #endregion
    }
}
