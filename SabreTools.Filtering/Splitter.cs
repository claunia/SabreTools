using System;
using SabreTools.Core.Tools;
using SabreTools.DatFiles;
using SabreTools.DatItems;
using SabreTools.Logging;

namespace SabreTools.Filtering
{
    public class Splitter
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
        private static readonly Logger logger = new();

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
                logger.Error(ex);
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
            logger.User("Creating device non-merged sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            datFile.Items.BucketBy(ItemKey.Machine, DedupeType.None, norename: true);

            // Now we want to loop through all of the games and set the correct information
            while (datFile.Items.AddRomsFromDevices(false, false)) ;
            while (datFile.ItemsDB.AddRomsFromDevices(false, false)) ;
            while (datFile.Items.AddRomsFromDevices(true, false)) ;
            while (datFile.ItemsDB.AddRomsFromDevices(true, false)) ;

            // Then, remove the romof and cloneof tags so it's not picked up by the manager
            datFile.Items.RemoveTagsFromChild();
            datFile.ItemsDB.RemoveTagsFromChild();
        }

        /// <summary>
        /// Use cloneof tags to create merged sets and remove the tags plus deduplicating if tags don't catch everything
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        internal static void CreateFullyMergedSets(DatFile datFile)
        {
            logger.User("Creating fully merged sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            datFile.Items.BucketBy(ItemKey.Machine, DedupeType.None, norename: true);

            // Now we want to loop through all of the games and set the correct information
            datFile.Items.AddRomsFromChildren(true, false);
            datFile.ItemsDB.AddRomsFromChildren(true, false);

            // Now that we have looped through the cloneof tags, we loop through the romof tags
            datFile.Items.RemoveBiosRomsFromChild(false);
            datFile.ItemsDB.RemoveBiosRomsFromChild(false);
            datFile.Items.RemoveBiosRomsFromChild(true);
            datFile.ItemsDB.RemoveBiosRomsFromChild(true);

            // Finally, remove the romof and cloneof tags so it's not picked up by the manager
            datFile.Items.RemoveTagsFromChild();
            datFile.ItemsDB.RemoveTagsFromChild();
        }

        /// <summary>
        /// Use cloneof tags to create non-merged sets and remove the tags plus using the device_ref tags to get full sets
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        internal static void CreateFullyNonMergedSets(DatFile datFile)
        {
            logger.User("Creating fully non-merged sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            datFile.Items.BucketBy(ItemKey.Machine, DedupeType.None, norename: true);

            // Now we want to loop through all of the games and set the correct information
            while (datFile.Items.AddRomsFromDevices(true, true)) ;
            while (datFile.ItemsDB.AddRomsFromDevices(true, true)) ;
            datFile.Items.AddRomsFromDevices(false, true);
            datFile.ItemsDB.AddRomsFromDevices(false, true);
            datFile.Items.AddRomsFromParent();
            datFile.ItemsDB.AddRomsFromParent();

            // Now that we have looped through the cloneof tags, we loop through the romof tags
            datFile.Items.AddRomsFromBios();
            datFile.ItemsDB.AddRomsFromBios();

            // Then, remove the romof and cloneof tags so it's not picked up by the manager
            datFile.Items.RemoveTagsFromChild();
            datFile.ItemsDB.RemoveTagsFromChild();
        }

        /// <summary>
        /// Use cloneof tags to create merged sets and remove the tags
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        internal static void CreateMergedSets(DatFile datFile)
        {
            logger.User("Creating merged sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            datFile.Items.BucketBy(ItemKey.Machine, DedupeType.None, norename: true);

            // Now we want to loop through all of the games and set the correct information
            datFile.Items.AddRomsFromChildren(true, true);
            datFile.ItemsDB.AddRomsFromChildren(true, true);

            // Now that we have looped through the cloneof tags, we loop through the romof tags
            datFile.Items.RemoveBiosRomsFromChild(false);
            datFile.ItemsDB.RemoveBiosRomsFromChild(false);
            datFile.Items.RemoveBiosRomsFromChild(true);
            datFile.ItemsDB.RemoveBiosRomsFromChild(true);

            // Finally, remove the romof and cloneof tags so it's not picked up by the manager
            datFile.Items.RemoveTagsFromChild();
            datFile.ItemsDB.RemoveTagsFromChild();
        }

        /// <summary>
        /// Use cloneof tags to create non-merged sets and remove the tags
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        internal static void CreateNonMergedSets(DatFile datFile)
        {
            logger.User("Creating non-merged sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            datFile.Items.BucketBy(ItemKey.Machine, DedupeType.None, norename: true);

            // Now we want to loop through all of the games and set the correct information
            datFile.Items.AddRomsFromParent();
            datFile.ItemsDB.AddRomsFromParent();

            // Now that we have looped through the cloneof tags, we loop through the romof tags
            datFile.Items.RemoveBiosRomsFromChild(false);
            datFile.ItemsDB.RemoveBiosRomsFromChild(false);
            datFile.Items.RemoveBiosRomsFromChild(true);
            datFile.ItemsDB.RemoveBiosRomsFromChild(true);

            // Finally, remove the romof and cloneof tags so it's not picked up by the manager
            datFile.Items.RemoveTagsFromChild();
            datFile.ItemsDB.RemoveTagsFromChild();
        }

        /// <summary>
        /// Use cloneof and romof tags to create split sets and remove the tags
        /// </summary>
        /// <param name="datFile">Current DatFile object to run operations on</param>
        internal static void CreateSplitSets(DatFile datFile)
        {
            logger.User("Creating split sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            datFile.Items.BucketBy(ItemKey.Machine, DedupeType.None, norename: true);

            // Now we want to loop through all of the games and set the correct information
            datFile.Items.RemoveRomsFromChild();
            datFile.ItemsDB.RemoveRomsFromChild();

            // Now that we have looped through the cloneof tags, we loop through the romof tags
            datFile.Items.RemoveBiosRomsFromChild(false);
            datFile.ItemsDB.RemoveBiosRomsFromChild(false);
            datFile.Items.RemoveBiosRomsFromChild(true);
            datFile.ItemsDB.RemoveBiosRomsFromChild(true);

            // Finally, remove the romof and cloneof tags so it's not picked up by the manager
            datFile.Items.RemoveTagsFromChild();
            datFile.ItemsDB.RemoveTagsFromChild();
        }

        #endregion
    }
}
