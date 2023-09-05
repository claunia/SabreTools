﻿using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents one shared feature object
    /// </summary>
    [JsonObject("sharedfeat"), XmlRoot("sharedfeat")]
    public class SharedFeature : DatItem
    {
        #region Fields

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name"), XmlElement("name")]
        public string? Name
        {
            get => _internal.ReadString(Models.Metadata.SharedFeat.NameKey);
            set => _internal[Models.Metadata.SharedFeat.NameKey] = value;
        }

        /// <summary>
        /// SharedFeature value
        /// </summary>
        [JsonProperty("value"), XmlElement("value")]
        public string? Value
        {
            get => _internal.ReadString(Models.Metadata.SharedFeat.ValueKey);
            set => _internal[Models.Metadata.SharedFeat.ValueKey] = value;
        }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty SharedFeature object
        /// </summary>
        public SharedFeature()
        {
            _internal = new Models.Metadata.SharedFeat();
            Machine = new Machine();

            Name = string.Empty;
            ItemType = ItemType.SharedFeature;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new SharedFeature()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.SharedFeat ?? new Models.Metadata.SharedFeat(),
            };
        }

        #endregion
    }
}
