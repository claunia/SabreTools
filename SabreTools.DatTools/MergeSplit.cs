using System;
using SabreTools.Core.Tools;
using SabreTools.DatFiles;
using SabreTools.DatItems;
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

        // TODO: Should any of these create a new DatFile in the process?
        // The reason this comes up is that doing any of the splits or merges
        // is an inherently destructive process. Making it output a new DatFile
        // might make it easier to deal with multiple internal steps. On the other
        // hand, this will increase memory usage significantly and would force the
        // existing paths to behave entirely differently
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
                        CreateSplitSets(datFile);
                        break;
                    case MergingFlag.Merged:
                        CreateMergedSets(datFile);
                        break;
                    case MergingFlag.NonMerged:
                        CreateNonMergedSets(datFile);
                        break;

                    // Nonstandard
                    case MergingFlag.FullMerged:
                        CreateFullyMergedSets(datFile);
                        break;
                    case MergingFlag.DeviceNonMerged:
                        CreateDeviceNonMergedSets(datFile);
                        break;
                    case MergingFlag.FullNonMerged:
                        CreateFullyNonMergedSets(datFile);
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

        /// <summary>
        /// Use cdevice_ref tags to get full non-merged sets and remove parenting tags
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        internal static void CreateDeviceNonMergedSets(DatFile datFile)
        {
            _staticLogger.User("Creating device non-merged sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            datFile.BucketBy(ItemKey.Machine, DedupeType.None, norename: true);

            // Now we want to loop through all of the games and set the correct information
            while (datFile.AddItemsFromDevices(false, false)) ;
            while (datFile.AddItemsFromDevices(true, false)) ;

            // Then, remove the romof and cloneof tags so it's not picked up by the manager
            datFile.RemoveMachineRelationshipTags();
        }

        /// <summary>
        /// Use cloneof tags to create merged sets and remove the tags plus deduplicating if tags don't catch everything
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        internal static void CreateFullyMergedSets(DatFile datFile)
        {
            _staticLogger.User("Creating fully merged sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            datFile.BucketBy(ItemKey.Machine, DedupeType.None, norename: true);

            // Now we want to loop through all of the games and set the correct information
            datFile.AddItemsFromChildren(true, false);

            // Now that we have looped through the cloneof tags, we loop through the romof tags
            datFile.RemoveBiosItemsFromChild(false);
            datFile.RemoveBiosItemsFromChild(true);

            // Finally, remove the romof and cloneof tags so it's not picked up by the manager
            datFile.RemoveMachineRelationshipTags();
        }

        /// <summary>
        /// Use cloneof tags to create non-merged sets and remove the tags plus using the device_ref tags to get full sets
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        internal static void CreateFullyNonMergedSets(DatFile datFile)
        {
            _staticLogger.User("Creating fully non-merged sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            datFile.BucketBy(ItemKey.Machine, DedupeType.None, norename: true);

            // Now we want to loop through all of the games and set the correct information
            while (datFile.AddItemsFromDevices(true, true)) ;
            datFile.AddItemsFromDevices(false, true);
            datFile.AddItemsFromParent();

            // Now that we have looped through the cloneof tags, we loop through the romof tags
            datFile.AddItemsFromBios();

            // Then, remove the romof and cloneof tags so it's not picked up by the manager
            datFile.RemoveMachineRelationshipTags();
        }

        /// <summary>
        /// Use cloneof tags to create merged sets and remove the tags
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        internal static void CreateMergedSets(DatFile datFile)
        {
            _staticLogger.User("Creating merged sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            datFile.BucketBy(ItemKey.Machine, DedupeType.None, norename: true);

            // Now we want to loop through all of the games and set the correct information
            datFile.AddItemsFromChildren(true, true);

            // Now that we have looped through the cloneof tags, we loop through the romof tags
            datFile.RemoveBiosItemsFromChild(false);
            datFile.RemoveBiosItemsFromChild(true);

            // Finally, remove the romof and cloneof tags so it's not picked up by the manager
            datFile.RemoveMachineRelationshipTags();
        }

        /// <summary>
        /// Use cloneof tags to create non-merged sets and remove the tags
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        internal static void CreateNonMergedSets(DatFile datFile)
        {
            _staticLogger.User("Creating non-merged sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            datFile.BucketBy(ItemKey.Machine, DedupeType.None, norename: true);

            // Now we want to loop through all of the games and set the correct information
            datFile.AddItemsFromParent();

            // Now that we have looped through the cloneof tags, we loop through the romof tags
            datFile.RemoveBiosItemsFromChild(false);
            datFile.RemoveBiosItemsFromChild(true);

            // Finally, remove the romof and cloneof tags so it's not picked up by the manager
            datFile.RemoveMachineRelationshipTags();
        }

        /// <summary>
        /// Use cloneof and romof tags to create split sets and remove the tags
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        internal static void CreateSplitSets(DatFile datFile)
        {
            _staticLogger.User("Creating split sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            datFile.BucketBy(ItemKey.Machine, DedupeType.None, norename: true);

            // Now we want to loop through all of the games and set the correct information
            datFile.RemoveItemsFromChild();

            // Now that we have looped through the cloneof tags, we loop through the romof tags
            datFile.RemoveBiosItemsFromChild(false);
            datFile.RemoveBiosItemsFromChild(true);

            // Finally, remove the romof and cloneof tags so it's not picked up by the manager
            datFile.RemoveMachineRelationshipTags();
        }

        #endregion
    }
}
