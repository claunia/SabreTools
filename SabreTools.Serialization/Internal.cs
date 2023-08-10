using System.Collections.Generic;
using System.Linq;
using SabreTools.Models.Internal;

namespace SabreTools.Serialization
{
    public class Internal
    {
        /// <summary>
        /// Extract nested items from a Part
        /// </summary>
        public static DatItem[]? ExtractItems(Part? item)
        {
            if (item == null)
                return null;

            var datItems = new List<DatItem>();

            var features = item.Read<Feature[]>(Part.FeatureKey);
            if (features != null && features.Any())
                datItems.AddRange(features);

            var dataAreas = item.Read<DataArea[]>(Part.DataAreaKey);
            if (dataAreas != null && dataAreas.Any())
                datItems.AddRange(dataAreas.SelectMany(ExtractItems));

            var diskAreas = item.Read<DiskArea[]>(Part.DiskAreaKey);
            if (diskAreas != null && diskAreas.Any())
                datItems.AddRange(diskAreas.SelectMany(ExtractItems));

            var dipSwitches = item.Read<DipSwitch[]>(Part.DipSwitchKey);
            if (dipSwitches != null && dipSwitches.Any())
                datItems.AddRange(dipSwitches);

            return datItems.ToArray();
        }

        /// <summary>
        /// Extract nested items from a DataArea
        /// </summary>
        private static Rom[]? ExtractItems(DataArea? item)
        {
            if (item == null)
                return null;

            return item.Read<Rom[]>(DataArea.RomKey);
        }

        /// <summary>
        /// Extract nested items from a DiskArea
        /// </summary>
        private static Disk[]? ExtractItems(DiskArea? item)
        {
            if (item == null)
                return null;

            return item.Read<Disk[]>(DiskArea.DiskKey);
        }
    }
}