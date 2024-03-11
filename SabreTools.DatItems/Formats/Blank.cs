using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents a blank set from an input DAT
    /// </summary>
    [JsonObject("blank"), XmlRoot("blank")]
    public sealed class Blank : DatItem
    {
        #region Constructors

        /// <summary>
        /// Create a default, empty Archive object
        /// </summary>
        public Blank()
        {
            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType.Blank);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            var blank = new Blank();
            blank.SetFieldValue<Machine>(DatItem.MachineKey, GetFieldValue<Machine>(DatItem.MachineKey));
            blank.SetFieldValue<bool>(DatItem.RemoveKey, GetFieldValue<bool>(DatItem.RemoveKey));
            blank.SetFieldValue<Source?>(DatItem.SourceKey, GetFieldValue<Source?>(DatItem.SourceKey));
            blank.SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, GetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey));

            return blank;
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If we don't have a blank, return false
            if (GetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey) != other?.GetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey))
                return false;

            // Otherwise, treat it as a Blank
            Blank? newOther = other as Blank;

            // If the archive information matches
            return GetFieldValue<Machine>(DatItem.MachineKey) == newOther!.GetFieldValue<Machine>(DatItem.MachineKey);
        }

        #endregion
    }
}
