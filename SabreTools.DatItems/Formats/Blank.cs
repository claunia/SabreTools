using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core.Tools;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents a blank set from an input DAT
    /// </summary>
    [JsonObject("blank"), XmlRoot("blank")]
    public sealed class Blank : DatItem
    {
        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.Blank;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Blank object
        /// </summary>
        public Blank()
        {
            SetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey, ItemType);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            var blank = new Blank();
            blank.SetFieldValue<Machine>(DatItem.MachineKey, GetFieldValue<Machine>(DatItem.MachineKey));
            blank.SetFieldValue<bool?>(DatItem.RemoveKey, GetBoolFieldValue(DatItem.RemoveKey));
            blank.SetFieldValue<Source?>(DatItem.SourceKey, GetFieldValue<Source?>(DatItem.SourceKey));
            blank.SetFieldValue<string?>(Models.Metadata.DatItem.TypeKey, GetStringFieldValue(Models.Metadata.DatItem.TypeKey).AsEnumValue<ItemType>().AsStringValue());

            return blank;
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If we don't have a blank, return false
            if (GetStringFieldValue(Models.Metadata.DatItem.TypeKey) != other?.GetStringFieldValue(Models.Metadata.DatItem.TypeKey))
                return false;

            // Otherwise, treat it as a Blank
            Blank? newOther = other as Blank;

            // If the archive information matches
            return GetFieldValue<Machine>(DatItem.MachineKey) == newOther!.GetFieldValue<Machine>(DatItem.MachineKey);
        }

        #endregion
    }
}
