using System;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Source information wrapper
    /// </summary>
    public class Source : ICloneable
    {
        /// <summary>
        /// Source index
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Source name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Source ID, default 0</param>
        /// <param name="source">Source name, default null</param>
        public Source(int id = 0, string source = null)
        {
            Index = id;
            Name = source;
        }

        #region Cloning

        /// <summary>
        /// Clone the current object
        /// </summary>
        public object Clone()
        {
            return new Source(Index, Name);
        }

        #endregion
    }
}
