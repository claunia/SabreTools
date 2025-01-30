using System;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;
using SabreTools.Core.Filter;
using SabreTools.Core.Tools;

namespace SabreTools.DatFiles
{
    /// <summary>
    /// Represents all possible DAT header information
    /// </summary>
    [JsonObject("header"), XmlRoot("header")]
    public sealed class DatHeader : ModelBackedItem<Models.Metadata.Header>, ICloneable
    {
        #region Constants

        /// <summary>
        /// Read or write format
        /// </summary>
        public const string DatFormatKey = "DATFORMAT";

        /// <summary>
        /// External name of the DAT
        /// </summary>
        public const string FileNameKey = "FILENAME";

        #endregion

        #region Fields

        [JsonIgnore]
        public bool CanOpenSpecified
        {
            get
            {
                var canOpen = GetStringArrayFieldValue(Models.Metadata.Header.CanOpenKey);
                return canOpen != null && canOpen.Length > 0;
            }
        }

        [JsonIgnore]
        public bool ImagesSpecified
        {
            get
            {
                return GetFieldValue<Models.OfflineList.Images?>(Models.Metadata.Header.ImagesKey) != null;
            }
        }

        [JsonIgnore]
        public bool InfosSpecified
        {
            get
            {
                return GetFieldValue<Models.OfflineList.Infos?>(Models.Metadata.Header.InfosKey) != null;
            }
        }

        [JsonIgnore]
        public bool NewDatSpecified
        {
            get
            {
                return GetFieldValue<Models.OfflineList.NewDat?>(Models.Metadata.Header.NewDatKey) != null;
            }
        }

        [JsonIgnore]
        public bool SearchSpecified
        {
            get
            {
                return GetFieldValue<Models.OfflineList.Search?>(Models.Metadata.Header.SearchKey) != null;
            }
        }

        #endregion

        #region Constructors

        public DatHeader() { }

        public DatHeader(Models.Metadata.Header header)
        {
            // Create a new internal model
            _internal = new Models.Metadata.Header();

            // Get all fields to automatically copy without processing
            var nonItemFields = TypeHelper.GetConstants(typeof(Models.Metadata.Header));
            if (nonItemFields != null)
            {
                // Populate the internal machine from non-filter fields
                foreach (string fieldName in nonItemFields)
                {
                    if (header.ContainsKey(fieldName))
                        _internal[fieldName] = header[fieldName];
                }
            }

            // Get all fields specific to the DatFiles implementation
            var nonStandardFields = TypeHelper.GetConstants(typeof(DatHeader));
            if (nonStandardFields != null)
            {
                // Populate the internal machine from filter fields
                foreach (string fieldName in nonStandardFields)
                {
                    if (header.ContainsKey(fieldName))
                        _internal[fieldName] = header[fieldName];
                }
            }
        }

        #endregion

        #region Cloning Methods

        /// <summary>
        /// Clone the current header
        /// </summary>
        public object Clone() => new DatHeader(GetInternalClone());

        /// <summary>
        /// Clone just the format from the current header
        /// </summary>
        public DatHeader CloneFormat()
        {
            var header = new DatHeader();

            header.SetFieldValue<DatFormat>(DatHeader.DatFormatKey,
                GetFieldValue<DatFormat>(DatHeader.DatFormatKey));

            return header;
        }

        /// <summary>
        /// Get a clone of the current internal model
        /// </summary>
        public Models.Metadata.Header GetInternalClone()
        {
            var header = (_internal.Clone() as Models.Metadata.Header)!;

            // Remove fields with default values
            if (header.ReadString(Models.Metadata.Header.ForceMergingKey).AsEnumValue<MergingFlag>() == MergingFlag.None)
                header.Remove(Models.Metadata.Header.ForceMergingKey);
            if (header.ReadString(Models.Metadata.Header.ForceNodumpKey).AsEnumValue<NodumpFlag>() == NodumpFlag.None)
                header.Remove(Models.Metadata.Header.ForceNodumpKey);
            if (header.ReadString(Models.Metadata.Header.ForcePackingKey).AsEnumValue<PackingFlag>() == PackingFlag.None)
                header.Remove(Models.Metadata.Header.ForcePackingKey);

            // Convert subheader values
            if (CanOpenSpecified)
                header[Models.Metadata.Header.CanOpenKey] = new Models.OfflineList.CanOpen { Extension = GetStringArrayFieldValue(Models.Metadata.Header.CanOpenKey) };
            if (ImagesSpecified)
                header[Models.Metadata.Header.ImagesKey] = GetFieldValue<Models.OfflineList.Images>(Models.Metadata.Header.ImagesKey);
            if (InfosSpecified)
                header[Models.Metadata.Header.InfosKey] = GetFieldValue<Models.OfflineList.Infos>(Models.Metadata.Header.InfosKey);
            if (NewDatSpecified)
                header[Models.Metadata.Header.NewDatKey] = GetFieldValue<Models.OfflineList.NewDat>(Models.Metadata.Header.NewDatKey);
            if (SearchSpecified)
                header[Models.Metadata.Header.SearchKey] = GetFieldValue<Models.OfflineList.Search>(Models.Metadata.Header.SearchKey);

            return header;
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(ModelBackedItem? other)
        {
            // If other is null
            if (other == null)
                return false;

            // If the type is mismatched
            if (other is not DatHeader otherItem)
                return false;

            // Compare internal models
            return _internal.EqualTo(otherItem._internal);
        }

        /// <inheritdoc/>
        public override bool Equals(ModelBackedItem<Models.Metadata.Header>? other)
        {
            // If other is null
            if (other == null)
                return false;

            // If the type is mismatched
            if (other is not DatHeader otherItem)
                return false;

            // Compare internal models
            return _internal.EqualTo(otherItem._internal);
        }

        #endregion

        #region Manipulation

        /// <summary>
        /// Runs a filter and determines if it passes or not
        /// </summary>
        /// <param name="filterRunner">Filter runner to use for checking</param>
        /// <returns>True if the Machine passes the filter, false otherwise</returns>
        public bool PassesFilter(FilterRunner filterRunner) => filterRunner.Run(_internal);

        #endregion
    }
}
