using System.Collections.Generic;
using System.IO;
using System.Linq;

using SabreTools.Library.DatItems;
using SabreTools.Library.Filtering;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SabreTools.Library.Tools;

/// <summary>
/// This holds all of the auxiliary types needed for proper parsing
/// </summary>
namespace SabreTools.Library.DatItems
{
    #region Machine

    #region SoftwareList

    /// <summary>
    /// Represents one SoftwareList shared feature object
    /// </summary>
    [JsonObject("sharedfeat")]
    public class SharedFeature
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    #endregion

    #endregion // Machine

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
    /// Represents one SoftwareList feature object
    /// </summary>
    [JsonObject("feature")]
    public class SoftwareListFeature
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    /// <summary>
    /// Represents one SoftwareList part object
    /// </summary>
    [JsonObject("part")]
    public class SoftwareListPart
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("interface")]
        public string Interface { get; set; }
    }

    #endregion

    #endregion //DatItem
}
