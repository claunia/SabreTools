using System;

namespace SabreTools.Models
{
    /// <summary>
    /// Represents a mapping to internal models
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class MappingAttribute : Attribute
    {
        /// <summary>
        /// Internal name for the mapping
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Determines if the mapped value is required or not
        /// </summary>
        public bool Required { get; set; }
        
        public MappingAttribute(string name, bool required = false)
        {
            this.Name = name;
            this.Required = required;
        }
    }
}