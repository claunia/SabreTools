using System;
using SabreTools.Core;
using SabreTools.Models.RomCenter;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing a RomCenter INI file
    /// </summary>
    internal partial class RomCenter : DatFile
    {
        /// <inheritdoc/>
        public override void ParseFile(string filename, int indexId, bool keep, bool statsOnly = false, bool throwOnError = false)
        {
            try
            {
                // Deserialize the input file
                var metadataFile = new Serialization.Files.RomCenter().Deserialize(filename);
                var metadata = new Serialization.CrossModel.RomCenter().Serialize(metadataFile);

                // Convert the credits data to the internal format
                ConvertCredits(metadataFile?.Credits);

                // Convert the dat data to the internal format
                ConvertDat(metadataFile?.Dat);

                // Convert the emulator data to the internal format
                ConvertEmulator(metadataFile?.Emulator);

                // Convert to the internal format
                ConvertMetadata(metadata, filename, indexId, statsOnly);
            }
            catch (Exception ex) when (!throwOnError)
            {
                string message = $"'{filename}' - An error occurred during parsing";
                logger.Error(ex, message);
            }
        }

        #region Converters

        /// <summary>
        /// Convert credits information
        /// </summary>
        /// <param name="credits">Deserialized model to convert</param>
        private void ConvertCredits(Models.RomCenter.Credits? credits)
        {
            // If the credits is missing, we can't do anything
            if (credits == null)
                return;

            if (Header.GetFieldValue<string?>(Models.Metadata.Header.AuthorKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.AuthorKey, credits.Author);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.CommentKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.CommentKey, credits.Comment);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.DateKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.DateKey, credits.Date);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.EmailKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.EmailKey, credits.Email);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.HomepageKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.HomepageKey, credits.Homepage);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.UrlKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.UrlKey, credits.Url);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.VersionKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.VersionKey, credits.Version);
        }

        /// <summary>
        /// Convert dat information
        /// </summary>
        /// <param name="dat">Deserialized model to convert</param>
        private void ConvertDat(Models.RomCenter.Dat? dat)
        {
            // If the dat is missing, we can't do anything
            if (dat == null)
                return;

            if (Header.GetFieldValue<string?>(Models.Metadata.Header.DatVersionKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.DatVersionKey, dat.Version);
            if (Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.ForceMergingKey) == MergingFlag.None && dat.Split == "1")
                Header.SetFieldValue<MergingFlag>(Models.Metadata.Header.ForceMergingKey, MergingFlag.Split);
            if (Header.GetFieldValue<MergingFlag>(Models.Metadata.Header.ForceMergingKey) == MergingFlag.None && dat.Merge == "1")
                Header.SetFieldValue<MergingFlag>(Models.Metadata.Header.ForceMergingKey, MergingFlag.Merged);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.SystemKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.SystemKey, dat.Plugin);
        }

        /// <summary>
        /// Convert emulator information
        /// </summary>
        /// <param name="games">Deserialized model to convert</param>
        private void ConvertEmulator(Models.RomCenter.Emulator? emulator)
        {
            // If the emulator is missing, we can't do anything
            if (emulator == null)
                return;

            if (Header.GetFieldValue<string?>(Models.Metadata.Header.EmulatorVersionKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.EmulatorVersionKey, emulator.Version);
            if (Header.GetFieldValue<string?>(Models.Metadata.Header.RefNameKey) == null)
                Header.SetFieldValue<string?>(Models.Metadata.Header.RefNameKey, emulator.RefName);
        }

        #endregion
    }
}
