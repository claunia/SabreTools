using System;
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

    #endregion //DatItem
}
