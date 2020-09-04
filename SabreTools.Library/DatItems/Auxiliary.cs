using System.Collections.Generic;
using System.IO;
using System.Linq;

using SabreTools.Library.DatItems;
using SabreTools.Library.Filtering;
using SabreTools.Library.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

/// <summary>
/// This holds all of the auxiliary types needed for proper parsing
/// </summary>
namespace SabreTools.Library.DatItems
{
    #region DatItem

    #region OpenMSX

    /// <summary>
    /// Represents the OpenMSX original value
    /// </summary>
    [JsonObject("original")]
    public class Original
    {
        [JsonProperty("value")]
        public bool? Value { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
    }

    #endregion

    #region SoftwareList

    /// <summary>
    /// Represents one SoftwareList dataarea object
    /// </summary>
    /// <remarks>
    /// One DataArea can contain multiple Rom items
    /// </remarks>
    [JsonObject("dataarea")]
    public class DataArea
    {
        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>
        /// Total size of the area
        /// </summary>
        [JsonProperty("size", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long? Size { get; set; }

        /// <summary>
        /// Word width for the area
        /// </summary>
        [JsonProperty("width", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Width { get; set; } // TODO: (8|16|32|64) "8"

        /// <summary>
        /// Byte endianness of the area
        /// </summary>
        [JsonProperty("endianness", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Endianness Endianness { get; set; }
    }

    /// <summary>
    /// Represents one SoftwareList diskarea object
    /// </summary>
    /// <remarks>
    /// One DiskArea can contain multiple Disk items
    /// </remarks>
    [JsonObject("diskarea")]
    public class DiskArea
    {
        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Name { get; set; }
    }

    /// <summary>
    /// Represents one SoftwareList part object
    /// </summary>
    /// <remarks>
    /// One Part can contain multiple PartFeature, DataArea, DiskArea, and DipSwitch items
    /// </remarks>
    [JsonObject("part")]
    public class Part
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("interface")]
        public string Interface { get; set; }
    
        [JsonProperty("features", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<PartFeature> Features { get; set; }
    }

    #endregion

    #endregion //DatItem
}
