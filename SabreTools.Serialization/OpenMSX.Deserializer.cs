using System.Linq;
using SabreTools.Models.OpenMSX;

namespace SabreTools.Serialization
{
    /// <summary>
    /// XML deserializer for OpenMSX software database files
    /// </summary>
    public partial class OpenMSX : XmlSerializer<SoftwareDb>
    {
        // TODO: Add deserialization of entire SoftwareDb
        #region Internal

        /// <summary>
        /// Convert from <cref="Models.Internal.Header"/> to <cref="Models.OpenMSX.SoftwareDb"/>
        /// </summary>
        public static SoftwareDb? ConvertHeaderFromInternalModel(Models.Internal.Header? item)
        {
            if (item == null)
                return null;

            var softwareDb = new SoftwareDb
            {
                Timestamp = item.ReadString(Models.Internal.Header.TimestampKey),
            };
            return softwareDb;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Machine"/> to <cref="Models.OpenMSX.Software"/>
        /// </summary>
        public static Software? ConvertMachineFromInternalModel(Models.Internal.Machine? item)
        {
            if (item == null)
                return null;
            
            var game = new Software
            {
                Title = item.ReadString(Models.Internal.Machine.NameKey),
                GenMSXID = item.ReadString(Models.Internal.Machine.GenMSXIDKey),
                System = item.ReadString(Models.Internal.Machine.SystemKey),
                Company = item.ReadString(Models.Internal.Machine.CompanyKey),
                Year = item.ReadString(Models.Internal.Machine.YearKey),
                Country = item.ReadString(Models.Internal.Machine.CountryKey),
            };

            var dumps = item.Read<Models.Internal.Dump[]>(Models.Internal.Machine.DumpKey);
            game.Dump = dumps?.Select(ConvertFromInternalModel)?.ToArray();

            return game;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Dump"/> to <cref="Models.OpenMSX.Dump"/>
        /// </summary>
        private static Dump? ConvertFromInternalModel(Models.Internal.Dump? item)
        {
            if (item == null)
                return null;
            
            var dump = new Dump();

            var original = item.Read<Models.Internal.Original>(Models.Internal.Dump.OriginalKey);
            dump.Original = ConvertFromInternalModel(original);

            var rom = item.Read<Models.Internal.Rom>(Models.Internal.Dump.RomKey);
            dump.Rom = ConvertRomFromInternalModel(rom);

            var megaRom = item.Read<Models.Internal.Rom>(Models.Internal.Dump.MegaRomKey);
            dump.Rom = ConvertMegaRomFromInternalModel(megaRom);

            var sccPlusCart = item.Read<Models.Internal.Rom>(Models.Internal.Dump.SCCPlusCartKey);
            dump.Rom = ConvertSCCPlusCartFromInternalModel(sccPlusCart);

            return dump;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.OpenMSX.MegaRom"/>
        /// </summary>
        private static MegaRom? ConvertMegaRomFromInternalModel(Models.Internal.Rom? item)
        {
            if (item == null)
                return null;
            
            var megaRom = new MegaRom
            {
                Start = item.ReadString(Models.Internal.Rom.StartKey),
                Type = item.ReadString(Models.Internal.Rom.TypeKey),
                Hash = item.ReadString(Models.Internal.Rom.SHA1Key),
                Remark = item.ReadString(Models.Internal.Rom.RemarkKey),
            };
            return megaRom;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Original"/> to <cref="Models.OpenMSX.Original"/>
        /// </summary>
        private static Original? ConvertFromInternalModel(Models.Internal.Original? item)
        {
            if (item == null)
                return null;
            
            var original = new Original
            {
                Value = item.ReadString(Models.Internal.Original.ValueKey),
                Content = item.ReadString(Models.Internal.Original.ContentKey),
            };
            return original;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.OpenMSX.Rom"/>
        /// </summary>
        private static Rom? ConvertRomFromInternalModel(Models.Internal.Rom? item)
        {
            if (item == null)
                return null;
            
            var rom = new Rom
            {
                Start = item.ReadString(Models.Internal.Rom.StartKey),
                Type = item.ReadString(Models.Internal.Rom.TypeKey),
                Hash = item.ReadString(Models.Internal.Rom.SHA1Key),
                Remark = item.ReadString(Models.Internal.Rom.RemarkKey),
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.OpenMSX.SCCPlusCart"/>
        /// </summary>
        private static SCCPlusCart? ConvertSCCPlusCartFromInternalModel(Models.Internal.Rom? item)
        {
            if (item == null)
                return null;
            
            var sccPlusCart = new SCCPlusCart
            {
                Start = item.ReadString(Models.Internal.Rom.StartKey),
                Type = item.ReadString(Models.Internal.Rom.TypeKey),
                Hash = item.ReadString(Models.Internal.Rom.SHA1Key),
                Remark = item.ReadString(Models.Internal.Rom.RemarkKey),
            };
            return sccPlusCart;
        }

        #endregion
    }
}