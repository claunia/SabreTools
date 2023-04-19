using System;

namespace SabreTools.Core
{
    /// <summary>
    /// Maps a set of strings to an enum value
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class MappingAttribute : Attribute
    {
        /// <summary>
        /// Set of mapping strings
        /// </summary>
        public string[] Mappings { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public MappingAttribute(params string[] mappings)
        {
            this.Mappings = mappings;
        }
    }
}