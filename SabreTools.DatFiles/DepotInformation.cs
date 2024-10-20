using System;
using SabreTools.Hashing;

namespace SabreTools.DatFiles
{
    /// <summary>
    /// Depot information wrapper
    /// </summary>
    public class DepotInformation : ICloneable
    {
        /// <summary>
        /// Name or path of the Depot
        /// </summary>
        public string? Name { get; }

        /// <summary>
        /// Whether to use this Depot or not
        /// </summary>
        public bool IsActive { get; }

        /// <summary>
        /// Depot byte-depth
        /// </summary>
        public int Depth { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="isActive">Set active state</param>
        /// <param name="depth">Set depth between 0 and SHA-1's byte length</param>
        public DepotInformation(bool isActive, int depth = 4)
            : this(null, isActive, depth)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Set active state</param>
        /// <param name="isActive">Set active state</param>
        /// <param name="depth">Set depth between 0 and SHA-1's byte length</param>
        public DepotInformation(string? name, bool isActive, int depth = 4)
        {
            Name = name;
            IsActive = isActive;
            Depth = depth;

            // Limit depth value
            if (Depth == Int32.MinValue)
                Depth = 4;
            else if (Depth < 0)
                Depth = 0;
            else if (Depth > Constants.SHA1Zero.Length)
                Depth = Constants.SHA1Zero.Length;
        }

        #region Cloning

        /// <summary>
        /// Clone the current object
        /// </summary>
        public object Clone() => new DepotInformation(Name, IsActive, Depth);

        #endregion
    }
}
