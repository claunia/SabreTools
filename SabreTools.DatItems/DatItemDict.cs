using System.Collections.Generic;

namespace SabreTools.DatItems
{
    /// <summary>
    /// Format-agnostic representation of item data
    /// </summary>
    public class DatItemDict : Dictionary<string, object>
    {
        #region Common Keys

        public const string NameKey = "name";

        #endregion

        public string? Name
        {
            get => ContainsKey(NameKey) ? this[NameKey] as string : null;
            set => this[NameKey] = value;
        }
    }
}