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
        /// Add a new extension to all items
        /// </summary>
        public const string AddExtensionKey = "ADDEXTENSION";

        /// <summary>
        /// Read or write format
        /// </summary>
        public const string DatFormatKey = "DATFORMAT";

        /// <summary>
        /// External name of the DAT
        /// </summary>
        public const string FileNameKey = "FILENAME";

        /// <summary>
        /// Output the machine name
        /// </summary>
        public const string GameNameKey = "GAMENAME";

        /// <summary>
        /// Input depot information
        /// </summary>
        public const string InputDepotKey = "INPUTDEPOT";

        /// <summary>
        /// Output depot information
        /// </summary>
        public const string OutputDepotKey = "OUTPUTDEPOT";

        /// <summary>
        /// Text to append to all outputted lines
        /// </summary>
        public const string PostfixKey = "POSTFIX";

        /// <summary>
        /// Text to prepend to all outputted lines
        /// </summary>
        public const string PrefixKey = "PREFIX";

        /// <summary>
        /// Wrap quotes around the entire line, sans prefix and postfix
        /// </summary>
        public const string QuotesKey = "QUOTES";

        /// <summary>
        /// Remove all item extensions
        /// </summary>
        public const string RemoveExtensionKey = "REMOVEEXTENSION";

        /// <summary>
        /// Replace all item extensions
        /// </summary>
        public const string ReplaceExtensionKey = "REPLACEEXTENSION";

        /// <summary>
        /// Output the item name
        /// </summary>
        public const string UseRomNameKey = "USEROMNAME";

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
        /// Clone the standard parts of the current header
        /// </summary>
        public DatHeader CloneStandard()
        {
            var header = new DatHeader();
            header.SetFieldValue<string?>(Models.Metadata.Header.AuthorKey,
                GetStringFieldValue(Models.Metadata.Header.AuthorKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.CategoryKey,
                GetStringFieldValue(Models.Metadata.Header.CategoryKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.CommentKey,
                GetStringFieldValue(Models.Metadata.Header.CommentKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.DateKey,
                GetStringFieldValue(Models.Metadata.Header.DateKey));
            header.SetFieldValue<DatFormat>(DatHeader.DatFormatKey,
                GetFieldValue<DatFormat>(DatHeader.DatFormatKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey,
                GetStringFieldValue(Models.Metadata.Header.DescriptionKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.EmailKey,
                GetStringFieldValue(Models.Metadata.Header.EmailKey));
            header.SetFieldValue<string?>(DatHeader.FileNameKey,
                GetStringFieldValue(DatHeader.FileNameKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.ForceMergingKey,
                GetStringFieldValue(Models.Metadata.Header.ForceMergingKey).AsEnumValue<MergingFlag>().AsStringValue());
            header.SetFieldValue<string?>(Models.Metadata.Header.ForceNodumpKey,
                GetStringFieldValue(Models.Metadata.Header.ForceNodumpKey).AsEnumValue<NodumpFlag>().AsStringValue());
            header.SetFieldValue<string?>(Models.Metadata.Header.ForcePackingKey,
                GetStringFieldValue(Models.Metadata.Header.ForcePackingKey).AsEnumValue<PackingFlag>().AsStringValue());
            header.SetFieldValue<string?>(Models.Metadata.Header.HeaderKey,
                GetStringFieldValue(Models.Metadata.Header.HeaderKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.HomepageKey,
                GetStringFieldValue(Models.Metadata.Header.HomepageKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.NameKey,
                GetStringFieldValue(Models.Metadata.Header.NameKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.RootDirKey,
                GetStringFieldValue(Models.Metadata.Header.RootDirKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.TypeKey,
                GetStringFieldValue(Models.Metadata.Header.TypeKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.UrlKey,
                GetStringFieldValue(Models.Metadata.Header.UrlKey));
            header.SetFieldValue<string?>(Models.Metadata.Header.VersionKey,
                GetStringFieldValue(Models.Metadata.Header.VersionKey));

            return header;
        }

        /// <summary>
        /// Clone the filtering parts of the current header
        /// </summary>
        public DatHeader CloneFiltering()
        {
            var header = new DatHeader();
            header.SetFieldValue<string?>(DatHeader.AddExtensionKey,
                GetStringFieldValue(DatHeader.AddExtensionKey));
            header.SetFieldValue<DatFormat>(DatHeader.DatFormatKey,
                GetFieldValue<DatFormat>(DatHeader.DatFormatKey));
            header.SetFieldValue<bool?>(DatHeader.GameNameKey,
                GetBoolFieldValue(DatHeader.GameNameKey));
            header.SetFieldValue<DepotInformation?>(DatHeader.InputDepotKey,
                GetFieldValue<DepotInformation?>(DatHeader.InputDepotKey)?.Clone() as DepotInformation);
            header.SetFieldValue<DepotInformation?>(DatHeader.OutputDepotKey,
                GetFieldValue<DepotInformation?>(DatHeader.OutputDepotKey)?.Clone() as DepotInformation);
            header.SetFieldValue<string?>(DatHeader.PostfixKey,
                GetStringFieldValue(DatHeader.PostfixKey));
            header.SetFieldValue<string?>(DatHeader.PrefixKey,
                GetStringFieldValue(DatHeader.PrefixKey));
            header.SetFieldValue<bool?>(DatHeader.RemoveExtensionKey,
                GetBoolFieldValue(DatHeader.RemoveExtensionKey));
            header.SetFieldValue<string?>(DatHeader.ReplaceExtensionKey,
                GetStringFieldValue(DatHeader.ReplaceExtensionKey));
            header.SetFieldValue<bool?>(DatHeader.QuotesKey,
                GetBoolFieldValue(DatHeader.QuotesKey));
            header.SetFieldValue<bool?>(DatHeader.UseRomNameKey,
                GetBoolFieldValue(DatHeader.UseRomNameKey));

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
            if (datHeader == null)
                return;

            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(DatHeader.FileNameKey)))
                SetFieldValue<string?>(DatHeader.FileNameKey,
                    datHeader.GetStringFieldValue(DatHeader.FileNameKey));

            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(Models.Metadata.Header.NameKey)))
                SetFieldValue<string?>(Models.Metadata.Header.NameKey,
                    datHeader.GetStringFieldValue(Models.Metadata.Header.NameKey));

            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(Models.Metadata.Header.DescriptionKey)))
                SetFieldValue<string?>(Models.Metadata.Header.DescriptionKey,
                    datHeader.GetStringFieldValue(Models.Metadata.Header.DescriptionKey));

            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(Models.Metadata.Header.RootDirKey)))
                SetFieldValue<string?>(Models.Metadata.Header.RootDirKey,
                    datHeader.GetStringFieldValue(Models.Metadata.Header.RootDirKey));

            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(Models.Metadata.Header.CategoryKey)))
                SetFieldValue<string?>(Models.Metadata.Header.CategoryKey,
                    datHeader.GetStringFieldValue(Models.Metadata.Header.CategoryKey));

            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(Models.Metadata.Header.VersionKey)))
                SetFieldValue<string?>(Models.Metadata.Header.VersionKey,
                    datHeader.GetStringFieldValue(Models.Metadata.Header.VersionKey));

            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(Models.Metadata.Header.DateKey)))
                SetFieldValue<string?>(Models.Metadata.Header.DateKey,
                    datHeader.GetStringFieldValue(Models.Metadata.Header.DateKey));

            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(Models.Metadata.Header.AuthorKey)))
                SetFieldValue<string?>(Models.Metadata.Header.AuthorKey,
                    datHeader.GetStringFieldValue(Models.Metadata.Header.AuthorKey));

            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(Models.Metadata.Header.EmailKey)))
                SetFieldValue<string?>(Models.Metadata.Header.EmailKey,
                    datHeader.GetStringFieldValue(Models.Metadata.Header.EmailKey));

            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(Models.Metadata.Header.HomepageKey)))
                SetFieldValue<string?>(Models.Metadata.Header.HomepageKey,
                datHeader.GetStringFieldValue(Models.Metadata.Header.HomepageKey));

            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(Models.Metadata.Header.UrlKey)))
                SetFieldValue<string?>(Models.Metadata.Header.UrlKey,
                    datHeader.GetStringFieldValue(Models.Metadata.Header.UrlKey));

            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(Models.Metadata.Header.CommentKey)))
                SetFieldValue<string?>(Models.Metadata.Header.CommentKey,
                    datHeader.GetStringFieldValue(Models.Metadata.Header.CommentKey));

            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(Models.Metadata.Header.HeaderKey)))
                SetFieldValue<string?>(Models.Metadata.Header.HeaderKey,
                    datHeader.GetStringFieldValue(Models.Metadata.Header.HeaderKey));

            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(Models.Metadata.Header.TypeKey)))
                SetFieldValue<string?>(Models.Metadata.Header.TypeKey,
                    datHeader.GetStringFieldValue(Models.Metadata.Header.TypeKey));

            if (datHeader.GetStringFieldValue(Models.Metadata.Header.ForceMergingKey).AsEnumValue<MergingFlag>() != MergingFlag.None)
                SetFieldValue<string?>(Models.Metadata.Header.ForceMergingKey,
                    datHeader.GetStringFieldValue(Models.Metadata.Header.ForceMergingKey).AsEnumValue<MergingFlag>().AsStringValue());

            if (datHeader.GetStringFieldValue(Models.Metadata.Header.ForceNodumpKey).AsEnumValue<NodumpFlag>() != NodumpFlag.None)
                SetFieldValue<string?>(Models.Metadata.Header.ForceNodumpKey,
                    datHeader.GetStringFieldValue(Models.Metadata.Header.ForceNodumpKey).AsEnumValue<NodumpFlag>().AsStringValue());

            if (datHeader.GetStringFieldValue(Models.Metadata.Header.ForcePackingKey).AsEnumValue<PackingFlag>() != PackingFlag.None)
                SetFieldValue<string?>(Models.Metadata.Header.ForcePackingKey,
                    datHeader.GetStringFieldValue(Models.Metadata.Header.ForcePackingKey).AsEnumValue<PackingFlag>().AsStringValue());

            if (datHeader.GetFieldValue<DatFormat>(DatHeader.DatFormatKey) != 0x00)
                SetFieldValue<DatFormat>(DatHeader.DatFormatKey,
                    datHeader.GetFieldValue<DatFormat>(DatHeader.DatFormatKey));

            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(DatHeader.PrefixKey)))
                SetFieldValue<string?>(DatHeader.PrefixKey,
                    datHeader.GetStringFieldValue(DatHeader.PrefixKey));

            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(DatHeader.PostfixKey)))
                SetFieldValue<string?>(DatHeader.PostfixKey,
                    datHeader.GetStringFieldValue(DatHeader.PostfixKey));

            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(DatHeader.AddExtensionKey)))
                SetFieldValue<string?>(DatHeader.AddExtensionKey,
                    datHeader.GetStringFieldValue(DatHeader.AddExtensionKey));

            if (!string.IsNullOrEmpty(datHeader.GetStringFieldValue(DatHeader.ReplaceExtensionKey)))
                SetFieldValue<string?>(DatHeader.ReplaceExtensionKey,
                    datHeader.GetStringFieldValue(DatHeader.ReplaceExtensionKey));

            SetFieldValue<DepotInformation?>(DatHeader.InputDepotKey,
                datHeader.GetFieldValue<DepotInformation?>(DatHeader.InputDepotKey)?.Clone() as DepotInformation);
            SetFieldValue<DepotInformation?>(DatHeader.OutputDepotKey,
                datHeader.GetFieldValue<DepotInformation?>(DatHeader.OutputDepotKey)?.Clone() as DepotInformation);
            SetFieldValue<bool?>(DatHeader.GameNameKey,
                datHeader.GetBoolFieldValue(DatHeader.GameNameKey));
            SetFieldValue<bool?>(DatHeader.QuotesKey,
                datHeader.GetBoolFieldValue(DatHeader.QuotesKey));
            SetFieldValue<bool?>(DatHeader.RemoveExtensionKey,
                datHeader.GetBoolFieldValue(DatHeader.RemoveExtensionKey));
            SetFieldValue<bool?>(DatHeader.UseRomNameKey,
                datHeader.GetBoolFieldValue(DatHeader.UseRomNameKey));
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
