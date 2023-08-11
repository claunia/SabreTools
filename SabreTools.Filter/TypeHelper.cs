using System;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using SabreTools.Models;
using SabreTools.Models.Internal;

namespace SabreTools.Filter
{
    internal static class TypeHelper
    {
        /// <summary>
        /// Get constant values for the given type, if possible
        /// </summary>
        public static string[]? GetConstants(Type? type)
        {
            if (type == null)
                return null;

            var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            if (fields == null)
                return null;

            return fields
                .Where(f => f.IsLiteral && !f.IsInitOnly)
                .Where(f => f.CustomAttributes.Any(a => a.AttributeType == typeof(NoFilterAttribute)))
                .Select(f => f.GetRawConstantValue() as string)
                .Where(v => v != null)
                .ToArray()!;
        }

        /// <summary>
        /// Attempt to get the DatItem type from the name
        /// </summary>
        public static Type? GetDatItemType(string? itemType)
        {
            if (string.IsNullOrWhiteSpace(itemType))
                return null;

            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsAssignableFrom(typeof(DatItem)) && t.IsClass)
                .FirstOrDefault(t => t.GetCustomAttribute<XmlRootAttribute>()?.ElementName == itemType);
        }
    }
}
