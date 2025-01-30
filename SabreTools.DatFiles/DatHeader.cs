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
            // Get all fields to automatically copy without processing
            var nonItemFields = TypeHelper.GetConstants(typeof(Models.Metadata.Header));
            if (nonItemFields == null)
                return;

            // Populate the internal machine from non-filter fields
            _internal = new Models.Metadata.Header();
            foreach (string fieldName in nonItemFields)
            {
                if (header.ContainsKey(fieldName))
                    _internal[fieldName] = header[fieldName];
            }

            // Get all fields specific to the DatFiles implementation
            var nonStandardFields = TypeHelper.GetConstants(typeof(DatHeader));
            if (nonStandardFields == null)
                return;

            // Populate the internal machine from filter fields
            foreach (string fieldName in nonStandardFields)
            {
                if (header.ContainsKey(fieldName))
                    _internal[fieldName] = header[fieldName];
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

        /// <summary>
        /// Overwrite local values from another DatHeader if not default
        /// </summary>
        /// <param name="datHeader">DatHeader to get the values from</param>
        public void ConditionalCopy(DatHeader? datHeader)
        {
            // If the header is invalid, we can't do anything
            if (datHeader == null)
                return;

            // Convert subheader values
            if (datHeader._internal.ContainsKey(Models.Metadata.Header.CanOpenKey))
            {
                var canOpen = datHeader.GetFieldValue<Models.OfflineList.CanOpen>(Models.Metadata.Header.CanOpenKey);
                if (canOpen?.Extension != null)
                    SetFieldValue<string[]?>(Models.Metadata.Header.CanOpenKey, canOpen.Extension);
            }
            if (datHeader._internal.ContainsKey(Models.Metadata.Header.ImagesKey))
            {
                var images = datHeader.GetFieldValue<Models.OfflineList.Images>(Models.Metadata.Header.ImagesKey);
                SetFieldValue<Models.OfflineList.Images?>(Models.Metadata.Header.ImagesKey, images);
            }
            if (datHeader._internal.ContainsKey(Models.Metadata.Header.InfosKey))
            {
                var infos = datHeader.GetFieldValue<Models.OfflineList.Infos>(Models.Metadata.Header.InfosKey);
                SetFieldValue<Models.OfflineList.Infos?>(Models.Metadata.Header.InfosKey, infos);
            }
            if (datHeader._internal.ContainsKey(Models.Metadata.Header.NewDatKey))
            {
                var newDat = datHeader.GetFieldValue<Models.OfflineList.NewDat>(Models.Metadata.Header.NewDatKey);
                SetFieldValue<Models.OfflineList.NewDat?>(Models.Metadata.Header.NewDatKey, newDat);
            }
            if (datHeader._internal.ContainsKey(Models.Metadata.Header.SearchKey))
            {
                var search = datHeader.GetFieldValue<Models.OfflineList.Search>(Models.Metadata.Header.SearchKey);
                SetFieldValue<Models.OfflineList.Search?>(Models.Metadata.Header.SearchKey, search);
            }

            // Selectively set non-standard fields
            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(DatHeader.FileNameKey)))
                SetFieldValue<string?>(DatHeader.FileNameKey, datHeader.GetStringFieldValue(DatHeader.FileNameKey));
            if (datHeader.GetFieldValue<DatFormat>(DatHeader.DatFormatKey) != 0x00)
                SetFieldValue<DatFormat>(DatHeader.DatFormatKey, datHeader.GetFieldValue<DatFormat>(DatHeader.DatFormatKey));

            // Selectively set all possible fields -- TODO: Figure out how to make this less manual
            if (GetStringFieldValue(Models.Metadata.Header.AuthorKey) == null)
                SetFieldValue<string?>(Models.Metadata.Header.AuthorKey, datHeader.GetStringFieldValue(Models.Metadata.Header.AuthorKey));
            if (GetStringFieldValue(Models.Metadata.Header.BiosModeKey).AsEnumValue<MergingFlag>() == MergingFlag.None)
                SetFieldValue<string?>(Models.Metadata.Header.BiosModeKey, datHeader.GetStringFieldValue(Models.Metadata.Header.BiosModeKey).AsEnumValue<MergingFlag>().AsStringValue());
            if (GetStringFieldValue(Models.Metadata.Header.BuildKey) == null)
                SetFieldValue<string?>(Models.Metadata.Header.BuildKey, datHeader.GetStringFieldValue(Models.Metadata.Header.BuildKey));
            if (GetStringFieldValue(Models.Metadata.Header.CategoryKey) == null)
                SetFieldValue<string?>(Models.Metadata.Header.CategoryKey, datHeader.GetStringFieldValue(Models.Metadata.Header.CategoryKey));
            if (GetStringFieldValue(Models.Metadata.Header.CommentKey) == null)
                SetFieldValue<string?>(Models.Metadata.Header.CommentKey, datHeader.GetStringFieldValue(Models.Metadata.Header.CommentKey));
            if (GetStringFieldValue(Models.Metadata.Header.DateKey) == null)
                SetFieldValue<string?>(Models.Metadata.Header.DateKey, datHeader.GetStringFieldValue(Models.Metadata.Header.DateKey));
            if (GetStringFieldValue(Models.Metadata.Header.DatVersionKey) == null)
                SetFieldValue<string?>(Models.Metadata.Header.DatVersionKey, datHeader.GetStringFieldValue(Models.Metadata.Header.DatVersionKey));
            if (GetBoolFieldValue(Models.Metadata.Header.DebugKey) == null)
                SetFieldValue<bool?>(Models.Metadata.Header.DebugKey, datHeader.GetBoolFieldValue(Models.Metadata.Header.DebugKey));
            if (GetStringFieldValue(Models.Metadata.Header.DescriptionKey) == null)
                SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey, datHeader.GetStringFieldValue(Models.Metadata.Header.DescriptionKey));
            if (GetStringFieldValue(Models.Metadata.Header.EmailKey) == null)
                SetFieldValue<string?>(Models.Metadata.Header.EmailKey, datHeader.GetStringFieldValue(Models.Metadata.Header.EmailKey));
            if (GetStringFieldValue(Models.Metadata.Header.EmulatorVersionKey) == null)
                SetFieldValue<string?>(Models.Metadata.Header.EmulatorVersionKey, datHeader.GetStringFieldValue(Models.Metadata.Header.EmulatorVersionKey));
            if (GetStringFieldValue(Models.Metadata.Header.ForceMergingKey).AsEnumValue<MergingFlag>() == MergingFlag.None)
                SetFieldValue<string?>(Models.Metadata.Header.ForceMergingKey, datHeader.GetStringFieldValue(Models.Metadata.Header.ForceMergingKey).AsEnumValue<MergingFlag>().AsStringValue());
            if (GetStringFieldValue(Models.Metadata.Header.ForceNodumpKey).AsEnumValue<NodumpFlag>() == NodumpFlag.None)
                SetFieldValue<string?>(Models.Metadata.Header.ForceNodumpKey, datHeader.GetStringFieldValue(Models.Metadata.Header.ForceNodumpKey).AsEnumValue<NodumpFlag>().AsStringValue());
            if (GetStringFieldValue(Models.Metadata.Header.ForcePackingKey).AsEnumValue<PackingFlag>() == PackingFlag.None)
                SetFieldValue<string?>(Models.Metadata.Header.ForcePackingKey, datHeader.GetStringFieldValue(Models.Metadata.Header.ForcePackingKey).AsEnumValue<PackingFlag>().AsStringValue());
            if (GetBoolFieldValue(Models.Metadata.Header.ForceZippingKey) == null)
                SetFieldValue<bool?>(Models.Metadata.Header.ForceZippingKey, datHeader.GetBoolFieldValue(Models.Metadata.Header.ForceZippingKey));
            if (GetStringFieldValue(Models.Metadata.Header.HeaderKey) == null)
                SetFieldValue<string?>(Models.Metadata.Header.HeaderKey, datHeader.GetStringFieldValue(Models.Metadata.Header.HeaderKey));
            if (GetStringFieldValue(Models.Metadata.Header.HomepageKey) == null)
                SetFieldValue<string?>(Models.Metadata.Header.HomepageKey, datHeader.GetStringFieldValue(Models.Metadata.Header.HomepageKey));
            if (GetStringFieldValue(Models.Metadata.Header.IdKey) == null)
                SetFieldValue<string?>(Models.Metadata.Header.IdKey, datHeader.GetStringFieldValue(Models.Metadata.Header.IdKey));
            if (GetStringFieldValue(Models.Metadata.Header.ImFolderKey) == null)
                SetFieldValue<string?>(Models.Metadata.Header.ImFolderKey, datHeader.GetStringFieldValue(Models.Metadata.Header.ImFolderKey));
            if (GetBoolFieldValue(Models.Metadata.Header.LockBiosModeKey) == null)
                SetFieldValue<bool?>(Models.Metadata.Header.LockBiosModeKey, datHeader.GetBoolFieldValue(Models.Metadata.Header.LockBiosModeKey));
            if (GetBoolFieldValue(Models.Metadata.Header.LockRomModeKey) == null)
                SetFieldValue<bool?>(Models.Metadata.Header.LockRomModeKey, datHeader.GetBoolFieldValue(Models.Metadata.Header.LockRomModeKey));
            if (GetBoolFieldValue(Models.Metadata.Header.LockSampleModeKey) == null)
                SetFieldValue<bool?>(Models.Metadata.Header.LockSampleModeKey, datHeader.GetBoolFieldValue(Models.Metadata.Header.LockSampleModeKey));
            if (GetStringFieldValue(Models.Metadata.Header.MameConfigKey) == null)
                SetFieldValue<string?>(Models.Metadata.Header.MameConfigKey, datHeader.GetStringFieldValue(Models.Metadata.Header.MameConfigKey));
            if (GetStringFieldValue(Models.Metadata.Header.NameKey) == null)
                SetFieldValue<string?>(Models.Metadata.Header.NameKey, datHeader.GetStringFieldValue(Models.Metadata.Header.NameKey));
            if (GetStringFieldValue(Models.Metadata.Header.NotesKey) == null)
                SetFieldValue<string?>(Models.Metadata.Header.NotesKey, datHeader.GetStringFieldValue(Models.Metadata.Header.NotesKey));
            if (GetStringFieldValue(Models.Metadata.Header.PluginKey) == null)
                SetFieldValue<string?>(Models.Metadata.Header.PluginKey, datHeader.GetStringFieldValue(Models.Metadata.Header.PluginKey));
            if (GetStringFieldValue(Models.Metadata.Header.RefNameKey) == null)
                SetFieldValue<string?>(Models.Metadata.Header.RefNameKey, datHeader.GetStringFieldValue(Models.Metadata.Header.RefNameKey));
            if (GetStringFieldValue(Models.Metadata.Header.RomModeKey).AsEnumValue<MergingFlag>() == MergingFlag.None)
                SetFieldValue<string?>(Models.Metadata.Header.RomModeKey, datHeader.GetStringFieldValue(Models.Metadata.Header.RomModeKey).AsEnumValue<MergingFlag>().AsStringValue());
            if (GetStringFieldValue(Models.Metadata.Header.RomTitleKey) == null)
                SetFieldValue<string?>(Models.Metadata.Header.RomTitleKey, datHeader.GetStringFieldValue(Models.Metadata.Header.RomTitleKey));
            if (GetStringFieldValue(Models.Metadata.Header.RootDirKey) == null)
                SetFieldValue<string?>(Models.Metadata.Header.RootDirKey, datHeader.GetStringFieldValue(Models.Metadata.Header.RootDirKey));
            if (GetStringFieldValue(Models.Metadata.Header.SampleModeKey).AsEnumValue<MergingFlag>() == MergingFlag.None)
                SetFieldValue<string?>(Models.Metadata.Header.SampleModeKey, datHeader.GetStringFieldValue(Models.Metadata.Header.SampleModeKey).AsEnumValue<MergingFlag>().AsStringValue());
            if (GetStringFieldValue(Models.Metadata.Header.SchemaLocationKey) == null)
                SetFieldValue<string?>(Models.Metadata.Header.SchemaLocationKey, datHeader.GetStringFieldValue(Models.Metadata.Header.SchemaLocationKey));
            if (GetStringFieldValue(Models.Metadata.Header.ScreenshotsHeightKey) == null)
                SetFieldValue<string?>(Models.Metadata.Header.ScreenshotsHeightKey, datHeader.GetStringFieldValue(Models.Metadata.Header.ScreenshotsHeightKey));
            if (GetStringFieldValue(Models.Metadata.Header.ScreenshotsWidthKey) == null)
                SetFieldValue<string?>(Models.Metadata.Header.ScreenshotsWidthKey, datHeader.GetStringFieldValue(Models.Metadata.Header.ScreenshotsWidthKey));
            if (GetStringFieldValue(Models.Metadata.Header.SystemKey) == null)
                SetFieldValue<string?>(Models.Metadata.Header.SystemKey, datHeader.GetStringFieldValue(Models.Metadata.Header.SystemKey));
            if (GetStringFieldValue(Models.Metadata.Header.TimestampKey) == null)
                SetFieldValue<string?>(Models.Metadata.Header.TimestampKey, datHeader.GetStringFieldValue(Models.Metadata.Header.TimestampKey));
            if (GetStringFieldValue(Models.Metadata.Header.TypeKey) == null)
                SetFieldValue<string?>(Models.Metadata.Header.TypeKey, datHeader.GetStringFieldValue(Models.Metadata.Header.TypeKey));
            if (GetStringFieldValue(Models.Metadata.Header.UrlKey) == null)
                SetFieldValue<string?>(Models.Metadata.Header.UrlKey, datHeader.GetStringFieldValue(Models.Metadata.Header.UrlKey));
            if (GetStringFieldValue(Models.Metadata.Header.VersionKey) == null)
                SetFieldValue<string?>(Models.Metadata.Header.VersionKey, datHeader.GetStringFieldValue(Models.Metadata.Header.VersionKey));
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
