using System;
using SabreTools.Core.Tools;
using SabreTools.DatFiles;
using SabreTools.IO.Logging;

namespace SabreTools.DatTools
{
    public class MergeSplit
    {
        #region Fields

        /// <summary>
        /// Splitting mode to apply
        /// </summary>
        public MergingFlag SplitType { get; set; }

        #endregion

        #region Logging

        /// <summary>
        /// Logging object
        /// </summary>
        private static readonly Logger _staticLogger = new();

        #endregion

        #region Running

        /// <summary>
        /// Apply splitting on the DatFile
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        /// <param name="useTags">True if DatFile tags override splitting, false otherwise</param>
        /// <param name="throwOnError">True if the error that is thrown should be thrown back to the caller, false otherwise</param>
        /// <returns>True if the DatFile was split, false on error</returns>
        public bool ApplySplitting(DatFile datFile, bool useTags, bool throwOnError = false)
        {
            InternalStopwatch watch = new("Applying splitting to DAT");

            try
            {
                // If we are using tags from the DAT, set the proper input for split type unless overridden
                if (useTags && SplitType == MergingFlag.None)
                    SplitType = datFile.Header.GetStringFieldValue(Models.Metadata.Header.ForceMergingKey).AsEnumValue<MergingFlag>();

                // Run internal splitting
                switch (SplitType)
                {
                    // Standard
                    case MergingFlag.None:
                        // No-op
                        break;
                    case MergingFlag.Split:
                        datFile.ApplySplit();
                        break;
                    case MergingFlag.Merged:
                        datFile.ApplyMerged();
                        break;
                    case MergingFlag.NonMerged:
                        datFile.ApplyNonMerged();
                        break;

                    // Nonstandard
                    case MergingFlag.FullMerged:
                        datFile.ApplyFullyMerged();
                        break;
                    case MergingFlag.DeviceNonMerged:
                        datFile.ApplyDeviceNonMerged();
                        break;
                    case MergingFlag.FullNonMerged:
                        datFile.ApplyFullyNonMerged();
                        break;
                }
            }
            catch (Exception ex) when (!throwOnError)
            {
                _staticLogger.Error(ex);
                return false;
            }
            finally
            {
                watch.Stop();
            }

            return true;
        }

        #endregion
    }
}
